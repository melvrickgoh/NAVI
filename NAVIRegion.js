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

L.NAVIRegion = L.Polygon.extend({
	visible: false,
	timeslots:new Hashtable(),//12 slots, each slot contains a berth record
	berthID:undefined,
	$container: undefined,
	currentSlot: 0,
	
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
	
	initialize: function (berthID,slotsHashtable, boundaryString,options) {
		L.setOptions(this, options);
		this.berthID = berthID;
		this.timeslots = slotsHashtable;
		var latlngs = this.parseBoundaryString(boundaryString);
		
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

	getJQueryContainer: function(){
		return this.$container;
	},

	renderPopupData: function(){
		var self = this;
		var htmlData = '<p>';
		var containerElement = '';
		var date = new Date();
		htmlData += '<span class=\'berthtime\'><b>' + date + '</b></span></br>';
		
		containerElement = '<div id=\'droppableContainer\' >';
		containerElement += '<div id=\'assigned\' class=\'' + Math.round(this.options.title) + '\'>';
		containerElement += '<label class=\'content_font-16 ui-widget-header\'> Berth ' + Math.round(this.berthID) + '</label><div id=\'berth'+Math.round(this.berthID)+'\' style=\'width:300px;height:180px;\'  ></div></div>';

		
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
			L.Polygon.prototype.bindPopup.apply(this, [this.renderPopupData(), options]);
				var self = this;
				
				if (this.timeslots.get(this.berthID+'.'+this.currentSlot)){
					this._setOccupiedStyle();
				}
				
				self.on("click", function(e) {	
					var date = new Date();
					date.setHours(this.currentSlot);
					date.setMinutes(0);
					date.setSeconds(0);
					$('.berthtime').html(date);
					var target = e.toElement || e.relatedTarget;
					self.$container = $('#berth' + (Math.round(self.options.title)));
					var $container = self.$container;
					if (target){//check if there's a parent. parent represents that there is already a popup
						self.closePopup();
					}else{				

							var droppableOptions = {
									accept: "#incomingShips > li",
									activeClass: "ui-state-highlight",
									drop: function( event, ui ) {
										var liAttributes = ui.draggable.context.attributes;
										
										var shipName = liAttributes.getNamedItem("name").value;
										var shipID = liAttributes.getNamedItem("id").value;
										var ship = shipManager.allShips.get(shipName);
										
										var message = 'Hi, do you wish to assign ' + shipName + ' to Berth ' + Math.round(self.berthID);
											
										var $notificationOverlay = self.generateNotificationOverlay();
										var $notification = self.generateNotificationModal(message, function(){
											//on successful assignment
												
											var messageToSend = $('#notification-box textarea').val();
											//messageToSend = self.encodeEscapeURI(messageToSend);
											console.log(self.berthID + '.' + self.currentSlot);
											self.timeslots.put(self.berthID+'.'+self.currentSlot,new BerthRecord(shipID,shipName,Math.round(self.berthID),self.currentSlot));
			
											//self.updateDBAddUser(user,messageToSend);
												
											self.removeNotificationUI();
												
											//self.addUser(user);
												
											self.deleteImage( ui.draggable );
										},function(){
											self.removeNotificationUI();
										});
											
										$notificationOverlay.css('display','block');
										$notificationOverlay.append($notification);
									}
							};
							
							var $ul=$(document.createElement('ul'));
							$ul.attr('id', 'incomingShips');
							$ul.addClass('incomingShips ui-helper-reset');				
							
							$container.append($ul);
							var berthRecord = self.timeslots.get(self.berthID+'.'+self.currentSlot);
							if (berthRecord == null || berthRecord == undefined){
								$container.droppable(droppableOptions);
							}else{
								self.addShipDOM($ul,berthRecord);
								self._setOccupiedStyle();
								//self.color('#b23d35');
								//self.fillColor('#fb9e25');
							}
							//console.log('not ready');
						
					}		
					return true;
				}, self);
		}

	},
	
	_setOriginalStyle: function(){
		this.setStyle({
			color:'#03F',
			fillColor:'#03F'
		});
	},

	_setOccupiedStyle: function(){
		this.setStyle({
			color:'#b23d35',
			fillColor:'#fb9e25'
		});
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

	addShipDOM: function($ul,berthRecord){
		var $li = $(document.createElement('li'));

		$li.addClass('ui-widget-content ui-corner-tr ui-draggable');
		var shipName = berthRecord.shipName;
		var $label = $(document.createElement('span'));
		$label.addClass('content_font-18');
		$label.html(shipName);
		
		
		$li.attr('shipName',shipName);
		$li.attr('shipID',berthRecord.shipID);
		$li.append($label);
		
		this.applyFade($li);
		
		$ul.append($li);
	},

	applyFade: function ( $item ){
		$item.fadeIn(function() {
			$item
			.animate({ width: "100%" })
			.find( "img" )
			.animate({ height: "36px" });
		});
	},

	recycleImage: function ($item) {
		this.removeUser($item.attr('username'));
		//alert($item.attr('username') + ' ' + this.users.size());
		console.log(this.users.keys());
		censusContainer.recycleImage($item);
	},
	
	generateNotificationOverlay: function(){
		return $('#overlay');
	},
	
	generateNotificationModal: function(defaultMsg,assignmentHandler,cancelHandler){
		var $notification = $(document.createElement('div'));
		$notification.attr('id','notification-box');
		
		var $label = $(document.createElement('label'));
		$label.addClass('title_font-25');
		$label.css('position','absolute');
		$label.css('top','10%');
		$label.css('left','10%');
		$label.html('Ship Berth Assignment');
		$label.appendTo($notification);
				
		var $textarea = $(document.createElement('textarea'));
		$textarea.addClass('logininput content_font-18');
		$textarea.attr('name','notification-comments');
		$textarea.attr('cols','70');
		$textarea.attr('rows','5');
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
		$('#overlay').css('display','none');
		$('#notification-box').remove();
	},
	
	deleteImage: function( $item ) {
		var self = this;
		var assignedContainer = $( "#berth" + Math.round(this.berthID));
		
		var trashIcon = "<a title='Remove Census Taker' class='trash_Position ui-icon ui-icon-trash'>Remove Census Taker</a>";
		this._setOccupiedStyle();
		$item.fadeOut(function() {
			//Remove user from censusManager at OO level
			shipManager.allShips.remove($item.attr('name'));

			var $list = $( "ul", assignedContainer ).length ?
					$( "ul", assignedContainer ) :
						$( "<ul class='incomingShips ui-helper-reset'/>" ).appendTo( assignedContainer );
					/*if(!$item.has('a')){
						console.log($item.has('a'));
						$item.append( trashIcon );
					}else*/ 

					/*if ($item.parent().parent().attr('class').split(" ")[0] == 'contentarea'){
						$item.append( trashIcon );
					};*/
					$item.appendTo( $list ).fadeIn(function() {
						$item
						.animate({ width: "100%" })
						.find( "img" )
						.animate({ height: "36px" });
					});

		});
	}
});

L.naviregion = function (berthID,slotsHashtable,boundaryString,options) {
	return new L.NAVIRegionRegion(berthID,slotsHashtable,boundaryString,options);
};