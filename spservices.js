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
	
	this.generateCAMLUpdateString = function(listCommand,hashArray){
		var fieldsToUpdate = '<Batch OnError="Continue">';
		//console.log(hashArray.length);
		var counter = 1;
		for (var k in hashArray){
			var fieldHash = hashArray[k];
			fieldsToUpdate += '<Method ID="' + counter + '" Cmd="'+listCommand+'">';
			
			var keys = fieldHash.keys();
			//console.log(keys);
			for (var i in keys){
				var fieldName = keys[i];
				var fieldValue = fieldHash.get(fieldName);
				fieldsToUpdate += '<Field Name="' + fieldName + '">' + fieldValue + '</Field>';
			}
			
			fieldsToUpdate += '</Method>';
			counter++;
		}
		return fieldsToUpdate + "</Batch>";
	},

	this.getList = function(listName,viewArrays,successCallback){
		var CAMLViewFieldsCall = this.generateCAMLViewString(viewArrays);
		$().SPServices({
                operation: "GetListItems",
                async: false,
                listName: listName,
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
	},
	
	this.updateList = function (listCommand,listNameToUpdate,items,successCallback){
		var CAMLUpdateFields = this.generateCAMLUpdateString(listCommand,items);
		$().SPServices({
			  operation: "UpdateListItems",
			  listName: listNameToUpdate,
			  async: false,
			  updates: CAMLUpdateFields,
			  completefunc: successCallback
		});
	},
	
	this.updateSingleListItem = function(id,action,listName,hash,successCallback){
		 var valPairs = this.generateValuePairs(hash);
		 
		 $().SPServices({
			  operation: "UpdateListItems",
			  listName: listName,
			  ID: id,
			  async: false,
			  batchCmd: action,
			  valuepairs: valPairs,
			  completefunc: successCallback
		 });
	},
	
	this.generateValuePairs = function(hash){
		var arr = [];
		var keys = hash.keys();
		for (var i in keys){
			var k = keys[i];
			var v = hash.get(k);
			arr.push([k,v]);
		}
		return arr;
	},
	
	this.updateWorkflow = function(respItemURL){
		 $().SPServices({
			operation: "GetToDosForItem",
			item: respItemURL,
			async: false,
			completefunc: function (xData, Status) {
			 var respToDoID = '';
			 var respToDoListID = '';

			 $(xData.responseXML).find("[nodeName='z:row']").each(function() {
			  respToDoID = $(this).attr("ows_ID");
			  respToDoListID = $(this).attr("ows_TaskListId");
			 });

			 $().SPServices({
			  operation: "AlterToDo",
			  async: false,
			  todoId: respToDoID,
			  todoListId: respToDoListID,
			  item: respItemURL,
			  taskData: '<my:myFields xmlns:my="http://schemas.microsoft.com/office/infopath/2003/myXSD" >' +
				'<my:Status>Completed</my:Status>' +
				'<my:PercentComplete>1.00000000000000</my:PercentComplete>' +
				'<my:WorkflowOutcome>Completed</my:WorkflowOutcome>' +
				'<my:FormData>Completed</my:FormData>' +
				'<my:Completed>1</my:Completed>' +
			   '</my:myFields>',
			  completefunc: function (xData, Status) {
			   alert(Status);
			  }
			 });

			}
		   });
	}
}