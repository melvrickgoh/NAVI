var berthRecords;

function Shipment(productName,destination,tonnage,price,client){
	this.productName = productName,
	this.destination = destination,
	this.tonnage = tonnage,
	this.price = price,
	this.client = client;
	
}

function ShipmentManager(){
	this.allShipments = undefined;
	
	this.getAllShipments = function(){
		var viewValues = ["Product Name","Destination","Tonnage","Price","Client"];
		var self = this;
		SPWS.getList("Shipments",viewValues,function(xData, Status){
			self.allShipments = [];
			$(xData.responseXML).SPFilterNode("z:row").each(function () {
				var xitem = $(this);
				var pdtName = xitem.attr('ows_ProductName');
				var destination = xitem.attr('ows_Destination');
				var tonnage = parseFloat(xitem.attr('ows_Tonnage'));
				var price = parseFloat(xitem.attr('ows_Price'));
				var client = xitem.attr('ows_Client');
				self.allShipments.push(new Shipment(pdtName,destination,tonnage,price,client));
            });
		});
	}
}

function Ship(shipID,shipName,destinationString,timeString,capacity,currentGoods,currentCapacity,workflowURL,SPRowID){
	this.parseDestinations = function(destinationString){
		if (destinationString==undefined){
			return '';
		}
		return destinationString.split(",");
	},
	
	this.parseTime = function(timeString){
		return Date.parse(timeString);
	},
	
	this.parseCapacity = function(capacityString){
		if(typeof capacityString !== 'number'){
			return parseFloat(capacityString);
		}
	},
	
	this.parseGoods = function(goodsString){
		if (goodsString==undefined){
			return [];
		}
		var goodsArr = goodsString.split(";");
		var goods = [];
		for (var key in goodsArr){
			var goodStr = goodsArr[key];
			var goodSingleArr = goodStr.split(",");
			goods.push({
				productName:goodSingleArr[0],
				destination:goodSingleArr[1],
				tonnage:goodSingleArr[2],
				price:goodSingleArr[3],
				client:goodSingleArr[4]
			});
		}
		return goods;
	};
	
	this.shipID = shipID,
	this.shipName = shipName,
	this.destinations = this.parseDestinations(destinationString),
	this.arrivalTime = this.parseTime(timeString),
	this.capacity = this.parseCapacity(capacity),
	this.currentGoods = this.parseGoods(currentGoods),
	this.currentCapacity = this.parseCapacity(currentCapacity),
	this.workflowURL = workflowURL;
	this.SPRowID = SPRowID;
	
	this.isHeadingTo = function(destinationName){
		if (this.destinations.indexOf(destinationName)!=0){
			return true;
		}
		return false;
	},
	
	this.tonnageLeft = function(){
		return this.capacity - this.currentCapacity;
	},
	
	this.addShipment = function(productName,destination,tonnage,price,client){
		this.currentGoods.push({
			productName:productName,
			destination:destination,
			tonnage:tonnage,
			price:price,
			client:client
		});
	}
}

function ShipContainer(){
	this.initialize = function(){		
		$shipContainer = $('#contentarea');
		$ul = $('#incomingShips');
		this.initializeContainerDroppable();
	};
	
	var $shipContainer;
	var $ul;
	
	this.applyClickEventHandler = function($item,func){
		$item.on('click',func);
	},
	
	this.removeClickEventHandler = function($item,func){
		$item.off();
	},
	
	//make the container droppable (Settings.js)
	this.initializeContainerDroppable = function(){
		this.appendDroppable($shipContainer);
	},
	
	this.getShipContainerJQuery = function(){
		return this.$shipContainer;
	},
	
	this.rebootContainer = function(){
		if($ul){$ul.empty()};
	},
	
	this.killLIEventHandlers = function(){
		if($ul){$ul.children('li').off()};
	},
	
	this.addShipDOM = function(ship){
		var $li = $(document.createElement('li'));
		$li.css('border-style','dotted');
		$li.css('border-width','0.5px');
		
	    var $label = $(document.createElement('span'));
	    $label.attr('class','content_font-12');
	    $label.html(ship.shipName);
		$label.attr('title','Arriving on ' + new Date(ship.arrivalTime));
	    
	    var shipName = ship.shipName;
	    
	    $li.attr('name',ship.shipName);
	    $li.attr('id',ship.shipID);
	    $li.append($label);   
	    
		this.appendDraggable($li);
	    
	    $ul.append($li);
	},
	
	this.appendDroppable = function( $item ){
		var self = this;
		$item.droppable({
			accept: "#assigned li li",
			activeClass: "custom-state-active",
			drop: function( event, ui ) {
				var parentClass = ui.draggable.parent().parent().attr('class');
				var droppableContainerTitle = parentClass.split(" ")[0];
				var assignment = mapManager.getAssignmentByTitle(droppableContainerTitle);
				var shipName = ui.draggable.context.attributes.getNamedItem("shipName").value;
								
				self.recycleImage( ui.draggable );
			}
		});
	},
	
	this.appendDraggable = function( $item ){
		$item.draggable({
			cancel: "a.ui-icon", // clicking an icon won't initiate dragging
			revert: "invalid", // when not dropped, the item will revert back to its initial position
			containment: "document",
			helper: "clone",
			cursor: "move"
		});
	},
	
	this.recycleImage = function ($item) {
		$item.fadeOut(function() {
			$item
				.find( "a.ui-icon-trash" )
					.remove()
				.end()
				.css( "width", "80%")
				.find( "img" )
					.css( "height", "72px" )
				.end()
				.appendTo( $ul )
				.fadeIn();
		});
	};
}

function ShipManager(){
	this.allShips = new Hashtable();
	this.shipContainer = new ShipContainer();
	this.shipContainer.initialize();
	
	this.shipSortAsc = function(ship1,ship2){
		//oldest to newest
		var date1 = ship1.arrivalTime,
		date2 = ship2.arrivalTime;
		  if (date1 > date2) return 1;
		  if (date1 < date2) return -1;
		  return 0;
	},
	
	this.shipSortDsc = function(ship1,ship2){
		//newest to oldest
		var date1 = ship1.arrivalTime,
		date2 = ship2.arrivalTime;
		if (date1 > date2) return -1;
		  if (date1 < date2) return 1;
		  return 0;
	},
	
	this.populateShipContainer = function(){
		this.getAllIncomingShips();
		/*var shipsArray = this.allShips.keys();
		for (var i in shipsArray){
			var ship = this.allShips.get(shipsArray[i]);
			this.shipContainer.addShipDOM(ship);
		}*/
		
		var ships = this.allShips.values().sort(this.shipSortAsc);
		for (var i in ships){
			var ship = ships[i];
			this.shipContainer.addShipDOM(ship);
		}
	},
	
	this.repopulateShipContainer = function(){
		this.shipContainer.rebootContainer();
		this.populateShipContainer();
	},	
	
	this.getAllIncomingShips = function(){
		var viewValues = ["Ship_x0020_ID","Ship_x0020_Name","Destinations","Arrival_x0020_Timing","Capacity","Current_x0020_Goods","Current_x0020_Capacity","Workflow_x0020_URL"];
		var self = this;
		SPWS.getList("Incoming Ships",viewValues,function(xData, Status){
			self.allShips.clear();
			$(xData.responseXML).SPFilterNode("z:row").each(function () {
				var xitem = $(this);
				var shipID = xitem.attr('ows_Ship_x0020_ID');
				var shipName = xitem.attr('ows_Title');
				var destinations = xitem.attr('ows_Destinations');
				var arrivalTime = xitem.attr('ows_Arrival_x0020_Timing');
				var capacity = xitem.attr('ows_Capacity');
				var currentGoods = xitem.attr('ows_Current_x0020_Goods');
				var currentCapacity = xitem.attr('ows_Current_x0020_Capacity');
				var workflowURL = xitem.attr('ows_Workflow_x0020_URL');
				var SPRowID = xitem.attr('ows_ID');
				self.allShips.put(shipName,new Ship(shipID,shipName,destinations,arrivalTime,capacity,currentGoods,currentCapacity,workflowURL,SPRowID));
            });
			return self.allShips;
		});
	},
	
	this.getShipsByDestination = function(portWanted){
		var plausibleShips = [];
		for (var key in this.allShips){
			var ship = this.allShips[key];
			if (ship.isHeadingTo(portWanted)){
				plausibleShips.push(ship);
			}
		}
		return plausibleShips;
	}
}

function BerthManager(){
	this.fillInTimeUI = function(timeArr){
		$timebar = $('#time');
		for (var i = 0; i<24; i++){
			$timeButton = $(document.createElement('div'));
			$timeButton.addClass('timebutton');
			$timeButton.attr('slot',i);
			$timeButton.html(timeArr[i]);
			$timeButton.click(function(){
				var oldSeries = $(document.getElementsByClassName('timebuttonselect'));
				oldSeries.removeClass('timebuttonselect');
				oldSeries.addClass('timebutton');
				$(this).removeClass('timebutton');
				$(this).addClass('timebuttonselect');
				berthManager.changeBerthTimes(new Number($(this).attr('slot')));
			
			});
			$timebar.append($timeButton);
		}
	};
	
	this.timeDOMUI = ['12am','1am','2am','3am','4am','5am','6am','7am','8am','9am','10am','11am',
	'12pm','1pm','2pm','3pm','4pm','5pm','6pm','7pm','8pm','9pm','10pm','11pm'];
	this.fillInTimeUI(this.timeDOMUI);
	this.berths = new Hashtable();
	this.layer = new L.LayerGroup();
	this.layer.addTo(map);
	
	this.changeBerthTimes = function(time){
		var layers = this.layer.getLayers();
		for (var i in layers){
			var layer = layers[i];
			layer.currentSlot = time;
			if (layer.timeslots.get(layer.berthID+'.'+layer.currentSlot)){
				layer._setOccupiedStyle();
			}else{
				layer._setOriginalStyle();
			}
		}
	},
	
	this.initialize = function(){
		this.populateBerths();
	},
	
	this.repopulateBerths = function(){
		this.berths.clear();
		this.layer.clearLayers();
		this.populateBerths();
	},
	
	this.populateBerths = function(){
		berthRecords = this.getTodaysBerthingRecords();
		var viewValues = ["Berth","BoundaryLatLng","Point"];
		var self = this;
		SPWS.getList("Berths",viewValues,function(xData, Status){
			$(xData.responseXML).SPFilterNode("z:row").each(function () {
				var xitem = $(this);
				var berthID = xitem.attr('ows_Berth');
				var berthLatLng = xitem.attr('ows_BoundaryLatLng');
				var berthPoint = xitem.attr('ows_Point');
				var localRecords = self.getBerthIDedRecords(Math.round(berthID),berthRecords);
				var newLayer = new L.NAVIRegion(Math.round(berthID),localRecords,berthLatLng,{
					title:berthID,
					berthPoint:berthPoint
				});
				self.berths.put(Math.round(berthID),newLayer);
				
				if (newLayer.timeslots.get(newLayer.berthID+'.'+newLayer.currentSlot)){
					newLayer._setOccupiedStyle();
				}
				
				self.layer.addLayer(newLayer);
            });

		});
	},
	
	this.getTodaysBerthingRecords = function(){
		var viewValues = ["Ship_x0020_ID","Ship_x0020_Name","Berth_x0020_ID","Docking_x0020_Time"];
		var self = this;
		berthingRecords = [];
		SPWS.getList("Shipment Schedule",viewValues,function(xData, Status){
			$(xData.responseXML).SPFilterNode("z:row").each(function () {
				var xitem = $(this);
				var shipID = xitem.attr('ows_Title');
				var shipName = xitem.attr('ows_Ship_x0020_ID');
				var berthID = xitem.attr('ows_Berth_x0020_ID');
				var time = xitem.attr('ows_Docking_x0020_Time');
				//if (typeof time !== Number){
					time = new Date(time);
				//}
				
				var today = new Date();
				if (today.getDate() == time.getDate() && today.getMonth() == time.getMonth()){
					berthingRecords.push(new BerthRecord(shipID,shipName,Math.round(berthID),time));
				}
            });
		});
		return berthingRecords;
	},
	
	this.getBerthIDedRecords = function(berthID,berthRecords){
		var timeHash = new Hashtable();
		for (var i in berthRecords){
			var br = berthRecords[i];
			if (br.berthID == berthID){
				var time = br.time;
				timeHash.put(berthID+'.'+time.getHours(),br);
			}
		}
		return timeHash;
	}
	
}

function BerthRecord(shipID,shipName,berthID,time){
	this.shipID = shipID,
	this.shipName = shipName,
	this.berthID = berthID,
	this.time = time;
	
	this.matchHour = function(digitHour){
		if (this.time.getHours()==digitHour){
			return true;
		}
		return false;
	}
}

function ViewManager(){
	
	this.initialize = function(){
		
	},
	
	this.repopulateShipContainer = function(functionToExecute){
		this.loading();
		
	},
	
	this.populateCensusContainer = function(){
		this.loading();

		dao.getAllUsers(vManager.currentUsername,function(result){
			var response = JSON.parse(result).users;
			var users = [];
			var jsonUsers = response;
			
			//Hard coded pictures
			var picHash = getUserPics();
			
			for (var i in jsonUsers){
				var userJSON = jsonUsers[i];
				user = new User(userJSON.username,userJSON.name,userJSON.currentAssignment,userJSON.assignments,userJSON.position,picHash.get(userJSON.username),userJSON.notifications);
				users.push(user);
				censusContainer.addUserDOM(user);//populating census container with these users
				censusManager.addUser(user);
			}
			//end loading overlay; started at dao login ln70
			vManager.removeOverlay();
		});
	},
	
	this.populateCensusContainerForAssignment = function(isSupervisor){
		var isSupervisor = isSupervisor;
		if (isSupervisor){
			censusContainer.generateSidebar();
		}
		this.loading();
		dao.getAllUsers(vManager.currentUsername,function(result){
			var response = JSON.parse(result).users;
			var users = [];
			var jsonUsers = response;
			
			//Hard coded pictures
			var picHash = getUserPics();
			
			for (var i in jsonUsers){
				var userJSON = jsonUsers[i];
				user = new User(userJSON.username,userJSON.name,userJSON.currentAssignment,userJSON.assignments,userJSON.position,picHash.get(userJSON.username),userJSON.notifications);
				users.push(user);
				if (isSupervisor){
					censusContainer.addUserDOM(user);//populating census container with these users
				}
				censusManager.addUser(user);
			}
			//closing initial loading done once users are loaded; subsequent assignments will trigger its own overlay
			vManager.removeOverlay();
			
			if (isSupervisor){
				vManager.loadCurrentAssignmentsMap(new Hashtable());
			}else{
				var paramHash = new Hashtable();
				paramHash.put('username',vManager.currentUsername);
				vManager.loadCurrentAssignmentsMap(paramHash);
			}
		});
	},
	
	this.mapManagerLoadRegions = function(result){
		var response = JSON.parse(result).assignment;
		
		for (var i in response){
			var jsonR = response[i];
			var users = new Hashtable();
			var usernameArr = jsonR.users;
			//console.log(jsonR);
			//console.log(usernameArr);
			for (var k in usernameArr){
				var username = usernameArr[k].username;
				users.put(username,censusManager.users.get(vManager.trim(username)));
			}
			var parsedTopojson = jsonR.topojsonData;
			var geoJson = topojson.feature(parsedTopojson,parsedTopojson.objects.collection);
			var region = new L.NavRegion(
					jsonR.gid,
					users,
					geoJson,
					{
						title: jsonR.area,
						period: new Date(jsonR.assignmentTimestamp),
						office: 'Bali North',
						style: mapManager.regionStyle,
						status: jsonR.assignmentStatus
					}
			);
			
			var heatmap = dao.parseHeatmap(jsonR.heatmap);
			region.heatmap = heatmap;

			var geotags = dao.parseGeotags(jsonR.geoTags);
			region.setGeoTags(geotags);
			
			var validationHash = getValidationHash();
			if (validationHash.containsKey(jsonR.gid)){
				validateNavRegion(region);
			}
			
			var notifications = dao.parseNotifications(jsonR.notifications);
			region.notifications = notifications;
			
			mapManager.addNavRegion(region,true); // add and make visible
		}
	},
	
	this.toggleTeamViewUI = function(self,singleSingleBoolean){
		if (self == undefined){
			self = this;
		}
		//for clicking from single view to see individual users
		if (vManager.singleViewOn && singleSingleBoolean){
			mapManager.singleUserUILoad(vManager.currentUsername);
		}else if (vManager.singleViewOn){
			//if already single, then set to multiple
			vManager.view.switchTeamViewGlow(false);
			mapManager.multiUserUILoad();
		}else{
			//if already multiple, then set to single
			vManager.view.switchTeamViewGlow(true);
			mapManager.singleUserUILoad(vManager.currentUsername);
		}
		//tracking of current view type
		vManager.singleViewOn = !vManager.singleViewOn;
		vManager.multiViewOn = !vManager.multiViewOn;
	},
	
	this.prepareUsernameParams = function(){
		var paramsHash = new Hashtable();
		if (!this.isManagerView){
			paramsHash.put('username',this.currentUsername);
		}
		return paramsHash;
	},
	
	this.loading = function(){
		this.loadOverlay();
		this.flexiLoader('loading');
		this.loadSpinner();
	},
	
	this.loadSpinner = function(){
		var $spinnerDiv = $(document.createElement('div'));
		$spinnerDiv.addClass('spinner');
		
		var $bouncer1 = $(document.createElement('div'));
		$bouncer1.addClass('double-bounce1');
		$spinnerDiv.append($bouncer1);
		
		var $bouncer2 = $(document.createElement('div'));
		$bouncer2.addClass('double-bounce2');
		$spinnerDiv.append($bouncer2);
		
		$(document.body).append($spinnerDiv);
		
	},
	
	this.flexiLoader = function(string){
		var $loaderDiv = $(document.createElement('div'));
		$loaderDiv.addClass('loading-spinner');

		$(document.body).append($loaderDiv);
		
		this.flexiLoaderChange(string);
	},
	
	this.flexiLoaderChange = function(string){
		$loaderDiv = $($('.loading-spinner')[0]);
		$loaderDiv.empty();
		
		var stringToAdd = '';
		for (var i = 0; i<40-string.length; i++){
			stringToAdd+=' ';
		}
		string = stringToAdd + string;
		
		for (var i = 40, k = string.length; i > 40-string.length; i--, k--) {
			var $bounce = $(document.createElement('div'));

			$bounce.addClass('bounce'+i);
			$bounce.html(string.charAt(string.length-k));
			$loaderDiv.append($bounce);
		}
	},
	
	this.loadOverlay = function(closureFunction){
		this.loadFlexibleOverlay('overlay', closureFunction)
	},
	
	this.loadBackgroundOverlay = function(closureFunction){
		this.loadFlexibleOverlay('background-overlay', closureFunction)
	},
	
	this.loadFlexibleOverlay = function(id,closureFunction){
		var $overlay = $(document.createElement('div'));
		$overlay.attr('id',id);
		$overlay.on('click',closureFunction);
		$(document.body).append($overlay);
		return $overlay;
	},
	
	this.removeOverlay = function(){
		$('#overlay').remove();
		$('.spinner').remove();
		$('.loading-spinner').remove();
	},
	
	this.loadNotificationView = function(user){
		this.loadBackgroundOverlay(this.closeNotificationView);
		var $notificationView = $(document.createElement('div'));
		$notificationView.attr('id','notification-view');
		
		var $notificationHeader = $(document.createElement('div'));
		$notificationHeader.addClass('header');
		$notificationHeader.addClass('title_font-25');
		$notificationHeader.html(user.name);
		$notificationView.append($notificationHeader);
		
		var $userImage = $(document.createElement('div'));
		$userImage.addClass('profilePicture');
		var src = 'background-image:url(data:image/png;base64,';
		src += user.picture + ');';
		$userImage.attr('style',src);
		$notificationView.append($userImage);
		
		var $nListContainer = $(document.createElement('div'));
		$nListContainer.addClass('listContainer');
		$notificationView.append($nListContainer);
		
		var notificationArr = user.notifications;
		for(var i in notificationArr){
			var note = notificationArr[i];
			$nListContainer.append(this.addNotificationBlock(note.senderTime,note.senderMsg,note.sender));
		}
		
		var $done = $(document.createElement('label'));
		$done.addClass('title_font-25');
		$done.addClass('doneButton');
		$done.html('Done');
		$done.on('click',this.closeNotificationView);
		$notificationView.append($done);

		$(document.body).append($notificationView);
	},
	
	this.addNotificationBlock = function(headerTime,message,senderUsername){
		var $notificationBlock = $(document.createElement('div'));
		$notificationBlock.addClass('perBlock');
		
		var $picture = $(document.createElement('img'));
		var src = 'data:image/png;base64,';
		$picture.addClass('blockPicture');
		src += censusManager.users.get(this.trim(senderUsername)).picture;
		$picture.attr('src',src);
		$notificationBlock.append($picture);
		
		var $headerTime = $(document.createElement('span'));
		$headerTime.addClass('blockHeader');
		$headerTime.addClass('content_font-12');
		$headerTime.html(new Date(headerTime));
		$notificationBlock.append($headerTime);
		
		var $message = $(document.createElement('span'));
		$message.addClass('blockMessage');
		$message.addClass('content_font-18');
		$message.html(message);
		$notificationBlock.append($message);
		
		return $notificationBlock;
	},
	
	this.closeNotificationView = function(){
		$('#background-overlay').remove();
		$('#notification-view').remove();
	},
	
	this.trim = function(str){
		str = str.replace(/^\s+/, '');
	    for (var i = str.length - 1; i >= 0; i--) {
	        if (/\S/.test(str.charAt(i))) {
	            str = str.substring(0, i + 1);
	            break;
	        }
	    }
	    return str;
	}
}

Number.prototype.digitise = function (val) {
    return (new Array( val + 1 ).join("0") + this ).slice( - val );
}

function ConvertDateToISO8601(dtDate)
{
//*************************************************
//Converts Javascript dtDate to ISO 8601 standard for compatibility with SharePoint lists
//Inputs: dtDate = Javascript date format (optional)
//*************************************************
  var d;
  if (dtDate != null)  {
     //Date value supplied
     d = dtDate;
  }
  else  {
     //No date supplied, Create new date object
     d = new Date();
  }
  //Generate ISO 8601 date/time formatted string
  var s = "";
  s += d.getFullYear() + "-";
  s += d.getDate() + "-";
  s += d.getMonth() + 1;
  s += " " + d.getHours().digitise(2) + ":";
  s += d.getMinutes().digitise(2) + ":";
  //time zone offset ("+8:00" - Singapore EST)
  s += d.getSeconds().digitise(2);// + "+8:00";
  //Return the ISO8601 date string
  return s;
}