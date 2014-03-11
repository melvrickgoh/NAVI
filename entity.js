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
				var pdtName = xitem.attr('ows_Product Name');
				var destination = xitem.attr('ows_Destination');
				var tonnage = parseFloat(xitem.attr('ows_Tonnage'));
				var price = parseFloat(xitem.attr('ows_Price'));
				var client = xitem.attr('ows_Client');
				self.allShipments.push(new Shipment(pdtName,destination,tonnage,price,client));
            });
		});
	}
}

function Ship(shipName,destinationString,timeString,capacity,currentGoods,currentCapacity){
	this.parseDestinations = function(destinationString){
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
	};
	
	this.shipName = shipName,
	this.destinations = this.parseDestinations(destinationString),
	this.arrivalTime = this.parseTime(timeString),
	this.capacity = this.parseCapacity(capacity),
	this.currentGoods = this.parseGoods(currentGoods),
	this.currentCapacity = this.parseCapacity(currentCapacity);
	
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

function ShipManager(){
	this.allShips = undefined;
	
	this.getAllIncomingShips = function(){
		var viewValues = ["Ship Name","Destinations","Arrival Time","Capacity","Current Goods","Current Capacity"];
		var self = this;
		SPWS.getList("Incoming Ships",viewValues,function(xData, Status){
			self.allShipments = [];
			$(xData.responseXML).SPFilterNode("z:row").each(function () {
				var xitem = $(this);
				var shipName = xitem.attr('ows_Ship Name');
				var destinations = xitem.attr('ows_Destinations');
				var arrivalTime = xitem.attr('ows_Arrival Time');
				var capacity = xitem.attr('ows_Capacity');
				var currentGoods = xitem.attr('ows_Current Goods');
				var currentCapacity = xitem.attr('ows_Current Capacity');
				self.allShips.push(new Ship(shipName,destinations,arrivalTime,capacity,currentGoods,currentCapacity));
            });
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
	
}

function ShipAllocation(ship,shipment){
	this.ship = ship,
	this.shipment = shipment;
}