/*
  Leaflet.NAVIRegion, a polygon plugin that adds purpose built region layers for NAVI
  (c) 2013-2014, Melvrick Goh

  http://leafletjs.com
  https://github.com/melvrickgoh
 */
"use strict";
/*
 * Leaflet.NavRegion assumes that you have already included the Leaflet library, custom User and custom JSHashTable scripts.
 */

L.NAVIRegion = L.polygon.extend({
	visible: false,
	slots:new Hashtable(),//12 slots, each slot contains a productname & shipname in {product:xxx,ship:xxx}
	berthID:undefined,
	$container: undefined,
	currentSlot: 1,
	
	options: {
	
	},
	
	fillSlot: function(slotNumber,product,shipname){
		slots.put(slotNumber,{
			product: product,
			ship: shipname
		});
	},
	
	parseBoundaryString: function(boundaryString){
		var latlngs = [];
		var llStrArr = boundaryString.split(";");
		for (var key in llStrArr){
			var value = llStrArr[key];
			var subArr = value.split(",");
			var lat = subArr[0];
			var lng = subArr[1];
			if (typeof lat !== 'number'){
				lat = parseFloat(lat);
			}
			if (typeof lng !== 'number'){
				lng = parseFloat(lng);
			}
			latlngs.push(new L.latLng(lat,lng));
		}
		return latlngs;
	},
	
	//Users can come in Hashtable or Array of user objects
	initialize: function (berthID,slotsHashtable, boundaryString,options) {
		this.berthID = berthID;
		var latlngs = parseBoundaryString(boundaryString);
		
		var i, len, hole;

		L.Polyline.prototype.initialize.call(this, latlngs, options);

		if (latlngs && L.Util.isArray(latlngs[0]) && (typeof latlngs[0][0] !== 'number')) {
			this._latlngs = this._convertLatLngs(latlngs[0]);
			this._holes = latlngs.slice(1);

			for (i = 0, len = this._holes.length; i < len; i++) {
				hole = this._holes[i] = this._convertLatLngs(this._holes[i]);
				if (hole[0].equals(hole[hole.length - 1])) {
					hole.pop();
				}
			}
		}

		// filter out last point if its equal to the first one
		latlngs = this._latlngs;

		if (latlngs.length >= 2 && latlngs[0].equals(latlngs[latlngs.length - 1])) {
			latlngs.pop();
		}
		
		this.bindPopup({showOnMouseClick:true});
	},
	
	updateDBAddUser: function(user,message){
		this.users.put(user.username,user);
		mapManager.visibleRegions.removeLayer(this);
		
		dao.assignCensusTaker(this.assignmentID, currentUser, user.getUsername(), message, new Date().getTime(), function(){
			//css for successful updating of addition of user to db
			console.log('user successfully assigned');
			//reload container upon successful assignment
			vManager.loading();
			censusManager.reboot();
			vManager.removeOverlay();
			
			vManager.populateCensusContainer();
		});
		
		
	},

	getJQueryContainer: function(){
		return this.$container;
	},

	renderPopupData: function(){
		var self = this;
		var htmlData = '<p>';
		var containerElement = '';
		htmlData += '<b>' + this.berthID + '</b></br>';
		
		if (!isAndroid){
			containerElement = '<div id=\'droppableContainer\' />';
			// Delegate all event handling for the container itself and its contents to the container
			containerElement += '<div id=\'assigned\' class=\'' + this.options.title + '\' style=\'width=400px;height=200px\'>';
			containerElement += '<label class=\'title_font-20 ui-widget-header\'> Assigned</label></div>';
			//Insert whatever you want into the container, using whichever approach you prefer
		}

		containerElement += htmlData + '</p>';
		return containerElement;
	},

	openPopup: function (popup, latlng, options) { // (Popup) or (String || HTMLElement, LatLng[, Object])
		this.closePopup();

		if (!(popup instanceof L.Popup)) {
			var content = popup;

			popup = new L.Popup(options)
			.setLatLng(latlng)
			.setContent(content);
		}
		popup._isOpen = true;

		this._popup = popup;
		return this.addLayer(popup);
	},

	closePopup: function (popup) {
		if (!popup || popup === this._popup) {
			popup = this._popup;
			this._popup = null;
		}
		if (popup) {
			this.removeLayer(popup);
			popup._isOpen = false;
		}
		return this;
	},

	bindPopup: function(options) {
		if (options && options.showOnMouseClick) {
			// call the super method
			L.GeoJSON.prototype.bindPopup.apply(this, [this.renderPopupData(), options]);

				var self = this;

//				bind to mouse over
				self.on("click", function(e) {	
					var target = e.toElement || e.relatedTarget;
					self.$container = $(document.getElementsByClassName(self.options.title)[0]);
					var $container = self.$container;
					if (target){//check if there's a parent. parent represents that there is already a popup
						self.closePopup();
					}else{				
						if ($container.length > 0){

							var droppableOptions = {
									accept: "#censusTakers > li",
									activeClass: "ui-state-highlight",
									drop: function( event, ui ) {
										var liAttributes = ui.draggable.context.attributes;
										
										var username = liAttributes.getNamedItem("username").value;
										var user = censusManager.users.get(vManager.trim(username));
										
										user.assignments.push(self.assignmentID);
										
										if (self.users.containsKey(user.getUsername())){
											
										}else{
											var message = 'Hi ' + user.getUsername() + ' here is your next assignment at ' + self.options.title;
											
											var $notificationOverlay = self.generateNotificationOverlay();
											var $notification = self.generateNotificationModal(message, function(){
												//on successful assignment
												
												var messageToSend = $('#notification-box textarea').val();
												//messageToSend = self.encodeEscapeURI(messageToSend);
												
												self.updateDBAddUser(user,messageToSend);
												
												self.removeNotificationUI();
												
												self.addUser(user);
												
												//self.deleteImage( ui.draggable );
											},function(){
												self.removeNotificationUI();
											});
											
											$(document.body).append($notification);
											$(document.body).append($notificationOverlay);
										}
									}
							};
							
							if (vManager.futureAssignmentsOn){
								$container.droppable(droppableOptions);
							}

							var $ul=$(document.createElement('ul'));
							$ul.attr('id', 'censusTakers');
							$ul.addClass('censusTakers ui-helper-reset');				
							
							$container.append($ul);

							var users = self.users.values();

							for (var i = 0; i<users.length; i++){
								var user = users[i];
								self.addUserDOM($ul, user);
							}
						}else{
							console.log('not ready');
						}
					}		
					return true;
				}, self);
		}

	},

	_getParent: function(element, className) {
		var parent = element.parentNode;

		while (parent != null) {

			if (parent.className && L.DomUtil.hasClass(parent, className))
				return parent;

			parent = parent.parentNode;

		}

		return false;

	},

	_popupMouseOut: function(e) {

		// detach the event
		L.DomEvent.off(this._popup, "mouseout", this._popupMouseOut, this);

		// get the element that the mouse hovered onto
		var target = e.toElement || e.relatedTarget;

		// check to see if the element is a popup
		if (this._getParent(target, "leaflet-popup"))
			return true;

		// check to see if the marker was hovered back onto
		if (target == this._icon)
			return true;

		// hide the popup
		this.closePopup();

	},

	deleteImage: function( $item ) {
		var self = this;
		var assignedContainer = $( "#assigned" );
		
		var trashIcon = "<a title='Remove Census Taker' class='trash_Position ui-icon ui-icon-trash'>Remove Census Taker</a>";

		$item.fadeOut(function() {
			//Remove user from censusManager at OO level
			censusManager.removeUser($item.attr('username'));

			var $list = $( "ul", assignedContainer ).length ?
					$( "ul", assignedContainer ) :
						$( "<ul class='censusTakers ui-helper-reset'/>" ).appendTo( assignedContainer );
					console.log($item.parent().attr('id'));
					/*if(!$item.has('a')){
						console.log($item.has('a'));
						$item.append( trashIcon );
					}else*/ 

					if ($item.parent().parent().attr('class').split(" ")[0] == 'contentarea'){
						$item.append( trashIcon );
					};
					$item.appendTo( $list ).fadeIn(function() {
						$item
						.animate({ width: "100%" })
						.find( "img" )
						.animate({ height: "36px" });
					});

					self.applyTrashClickDelete($item);
		});
	},

	addUserDOM: function($ul,user){
		var $li = $(document.createElement('li'));

		$li.addClass('ui-widget-content ui-corner-tr ui-draggable');

		var $label = $(document.createElement('span'));
		$label.addClass('content_font-18');
		$label.html(user.getName());
		
		var username = user.getUsername();
		$li.attr('name',user.getName());
		$li.attr('username',username);
		$li.attr('currentAssignment',user.getCurrentAssignment());
		$li.attr('assignments', user.getAssignmentsHTMLString());
		$li.attr('position',user.getPostion());
		$li.attr('picture', user.getPictureBase64());
		$li.append($label);
		
		var $picture = $(document.createElement('img'));
		var src = 'data:image/png;base64,';
		$picture.addClass('censusPicture');
		src += user.getPictureBase64();
		$picture.attr('src',src);
		$li.append($picture);
		
		var $currentNotifications = $(document.createElement('div'));
		$currentNotifications.addClass('notification-icon');
		$currentNotifications.on('click',function(){
			//console.log(mapManager.getNotifications(self.assignmentID));
			//console.log(mapManager.getNotifications(self.assignmentID).length);
			vManager.loadNotificationView(user);
		});
		
		$li.append($currentNotifications);
		
		if(!vManager.futureAssignmentsOn){
	    	$li.off();
			$li.on('click',function(){
				vManager.currentUsername = username;
		    	vManager.toggleTeamViewUI(undefined,true);
		    });
	    }
		
		//this.appendTrashIcon($li);
		this.applyFade($li);
		
		//only future assignments view can you assign census takers
		if (vManager.futureAssignmentsOn){
			this.appendDraggable($li);
		}
		//this.applyTrashClickDelete($li);

		$ul.append($li);
	},

	applyTrashClickDelete: function ( $item ){
		var self = this;
		$item.click(function( event ) {
			var $target = $( event.target );

			if ( $target.is( "a.ui-icon-trash" ) ) {
				self.recycleImage( $item );
			}

			return false;
		});
	},

	applyFade: function ( $item ){
		$item.fadeIn(function() {
			$item
			.animate({ width: "100%" })
			.find( "img" )
			.animate({ height: "36px" });
		});
	},

	appendTrashIcon: function ( $item ){
		var trashIcon = "<a title='Remove Census Taker' class='trash_Position ui-icon ui-icon-trash'>Remove Census Taker</a>";
		$item.append( trashIcon );
	},

	appendDraggable: function( $item ){
		$item.draggable({
			cancel: "a.ui-icon", // clicking an icon won't initiate dragging
			revert: "invalid", // when not dropped, the item will revert back to its initial position
			containment: "document",
			helper: "clone",
			cursor: "move"
		});
	},

	recycleImage: function ($item) {
		this.removeUser($item.attr('username'));
		//alert($item.attr('username') + ' ' + this.users.size());
		console.log(this.users.keys());
		censusContainer.recycleImage($item);
	},
	
	generateNotificationOverlay: function(){
		var $notificationOverlay = $(document.createElement('div'));
		$notificationOverlay.attr('id','notification-overlay');
		return $notificationOverlay;
	},
	
	generateNotificationModal: function(defaultMsg,assignmentHandler,cancelHandler){
		var $notification = $(document.createElement('div'));
		$notification.attr('id','notification-box');
		
		var $label = $(document.createElement('label'));
		$label.addClass('title_font-25');
		$label.css('position','absolute');
		$label.css('top','10%');
		$label.css('left','10%');
		$label.html('Notification Message');
		$label.appendTo($notification);
				
		var $textarea = $(document.createElement('textarea'));
		$textarea.addClass('logininput content_font-18');
		$textarea.attr('name','notification-comments');
		$textarea.attr('cols','70');
		$textarea.attr('rows','10');
		$textarea.attr('margin','0 auto 0 auto');
		$textarea.css('top','25%');
		$textarea.html(defaultMsg);
		$textarea.appendTo($notification);
		
		var $assignBtn = $(document.createElement('label'));
		$assignBtn.addClass('title_font-25');
		$assignBtn.addClass('notificationbutton');
		$assignBtn.html('Assign');
		$assignBtn.css('position','absolute');
		$assignBtn.css('bottom','10%');
		$assignBtn.css('right','15%');
		$assignBtn.click(assignmentHandler);
		$assignBtn.appendTo($notification);
		
		var $cancelBtn = $(document.createElement('label'));
		$cancelBtn.addClass('title_font-25');
		$cancelBtn.addClass('notificationbutton');
		$cancelBtn.html('Cancel');
		$cancelBtn.css('position','absolute');
		$cancelBtn.css('bottom','10%');
		$cancelBtn.css('left','15%');
		$cancelBtn.click(cancelHandler);
		$cancelBtn.appendTo($notification);
		
		var $closeBtn = $(document.createElement('a'));
		$closeBtn.attr('id','cancel-icon');
		$closeBtn.click(function(){
			var $parent = $(this).parent().parent().parent();
			$parent.remove();
		});
		$closeBtn.appendTo($notification);
		
		return $notification;
	},
	
	removeNotificationUI: function(){
		$('#notification-overlay').remove();
		$('#notification-box').remove();
	},
});

L.naviregion = function (berthID,slotsHashtable,boundaryString,options) {
	return new L.NAVIRegionRegion(berthID,slotsHashtable,boundaryString,options);
};