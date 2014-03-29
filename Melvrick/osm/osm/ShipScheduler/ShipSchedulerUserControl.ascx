<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShipSchedulerUserControl.ascx.cs" Inherits="osm.VisualWebPart1.VisualWebPart1UserControl" %>

<meta name="viewport" content="width=device-width, initial-scale=1.0">

    <link rel="stylesheet" href="/Transshipment/Resource Library/leaflet.css" />
    <link rel="stylesheet" href="/Transshipment/Resource Library/style.css" />

    <style type="text/css">
        .style1
        {
            width: 100%;
        }
        .style2
        {
            width: 155px;
        }
    </style>


<table class="style1" style="z-index: 500">
    <div id="overlay" style="z-index:1000; width:855px; height:500px;display:none" ></div>
    <tr>
        <td class="style2">

    <div id="menubar_frame" style="width:150px; height:500px">
        <label style="font-family: Impact, Charcoal, sans-serif; font-weight: normal; color: #111; font-size: 20px;">Incoming Ships</label>
        <div id="contentarea" style="overflow-x:hidden;overflow-y:auto;">
        <ul id="incomingShips" style="overflow-x:hidden;overflow-y:auto;" class="incomingShips ui-helper-reset ui-helper-clearfix">
        
        </ul>
        </div>
    </div>
        </td>
        <td>
    <div id="map" style="width: 700px; height:500px">
        <div id="time" style="z-index:501; width:700px"></div>
    </div>
    
        </td>
    </tr>
    <tr>
        <td>
        
        </td>
        <td>
        </td>
    </tr>
</table>
        
        <script src="/Transshipment/Resource Library/jquery.min.js"></script>
        <script src="/Transshipment/Resource Library/jquery.SPServices-2014.01.min.js"></script>
        <script src="/Transshipment/Resource Library/jquery-ui.min.v192.js"></script>
        <link rel="stylesheet" href="/Transshipment/Resource Library/jquery-ui.css" />
        <script src="/Transshipment/Resource Library/leaflet.js"></script>
        <script src="/Transshipment/Resource Library/hashtable.js"></script>
        <script src="/Transshipment/Resource Library/spservices.js"></script>
        <script src="/Transshipment/Resource Library/NAVIRegion.js"></script>
        <script src="/Transshipment/Resource Library/entity.js"></script>
        <link rel="stylesheet" href="http://code.jquery.com/ui/1.10.4/themes/smoothness/jquery-ui.css" />

    <script type="text/javascript">
        var HeadOfficeLat = '';
    var HeadOfficeLong = '';
    var HeadOfficeName = '';
    var MapZoomLevel;
    var map;

    SPConfiguration =
    {

        load: function () {
            //Required for cross site scripting in IE
            $.support.cors = true;
            //alert('calling configuration list' + _spPageContextInfo.webServerRelativeUrl + "/_api/web/lists/GetByTitle('Configuration')/items");
            $().SPServices({
                operation: "GetListItems",
                async: false,
                listName: "PDO Configuration Data",
                CAMLViewFields: "<ViewFields><FieldRef Name='Title' /><FieldRef Name='HeadOfficeLongLat' /><FieldRef Name='MapZoomLevel' /></ViewFields>",
                completefunc: function (xData, Status) {
                    $(xData.responseXML).SPFilterNode("z:row").each(function () {
                        var xitem = $(this);
                        var Title = xitem.attr('ows_Title');
                        var HeadOfficeLongLat = xitem.attr('ows_HeadOfficeLongLat');
                        var temp = HeadOfficeLongLat.split(",");
                        HeadOfficeLat = temp[0];
                        HeadOfficeLong = temp[1];
                        var MapZoomLevel = xitem.attr('ows_MapZoomLevel');
                        //Initialize the Map
                        map = L.map('map').setView([HeadOfficeLat, HeadOfficeLong], MapZoomLevel);
                        L.tileLayer('http://{s}.tile.osm.org/{z}/{x}/{y}.png', { maxZoom: 18 }).addTo(map);
                    });
                }
            });

        },

        onSuccess: function (data) {
            alert('success of calling list');
            var temp;
            var results = data.d.results;
            for (var i = 0; i < results.length; i++) {
                HeadOfficeName = results[i].Title;
                temp = results[i].HeadOfficeLongLat.split(",");
                HeadOfficeLat = temp[0]
                HeadOfficeLong = temp[1]

                MapZoomLevel = results[i].MapZoomLevel;
            }


            //Initialize the Map
            map = L.map('map').setView([HeadOfficeLat, HeadOfficeLong], MapZoomLevel);
            L.tileLayer('http://{s}.tile.osm.org/{z}/{x}/{y}.png', { maxZoom: 18 }).addTo(map);

        },

        onError: function (err) {
            alert('onError: ' + JSON.stringify(err));
        }
    }

    SPConfiguration.load();
    var shipManager = new ShipManager();
    shipManager.populateShipContainer();

    var berthManager = new BerthManager();
    berthManager.initialize();
    </script>