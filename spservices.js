SPWS = new SPServices();

function SPServices(){
	this.generateCAMLViewString = function(viewArrays){
		var string = "<ViewFields>";
		for (var key in viewArrays){
			var value = viewArrays[key];
			string += "<FieldRef Name='" + value + "' />";
		}
		return string + "</ViewFields>";
	},

	this.getList = function(listName,viewArrays,successCallback){
		var CAMLViewFieldsCall = this.generateCAMLViewString(viewArrays);
		$().SPServices({
                operation: "GetListItems",
                async: false,
                listName: "Configuration",
                CAMLViewFields: CAMLViewFieldsCall,
                completefunc: successCallback
				
				/*function (xData, Status) {
                    $(xData.responseXML).SPFilterNode("z:row").each(function () {
                        var xitem = $(this);
                        var Title = xitem.attr('ows_Title');
                        var HeadOfficeLongLat = xitem.attr('ows_HeadOfficeLongLat');
                        var temp = HeadOfficeLongLat.split(",");
                        HeadOfficeLat = temp[0];
                        HeadOfficeLong = temp[1];
                        var MapZoomLevel = xitem.attr('ows_MapZoomLevel');
                        //alert(Title + ' ' + HeadOfficeLongLat + ' ' + MapZoomLevel);
                        //Initialize the Map
                        map = L.map('map').setView([HeadOfficeLat, HeadOfficeLong], MapZoomLevel);
                        L.tileLayer('http://{s}.tile.osm.org/{z}/{x}/{y}.png', { maxZoom: 18 }).addTo(map);
                        //http://{s}.tile.cloudmade.com/BC9A493B41014CAABB98F0471D759707/997/256/{z}/{x}/{y}.png
                    });
                }*/
            });
	}
}