debugger;

require(["esri/map",
    "dojo/on",
    "dojo/dom",
    "dojo/parser",
    "esri/tasks/FindTask",
	"esri/tasks/FindParameters",	
	"esri/layers/FeatureLayer",
	"esri/layers/ArcGISDynamicMapServiceLayer",
	"esri/tasks/query",
	
	"esri/Color",
	"esri/symbols/SimpleMarkerSymbol",    
	"dojo/dom-construct",	
	"esri/InfoTemplate",

    "dijit/layout/TabContainer",
    "dijit/layout/ContentPane",
    "dijit/layout/BorderContainer",
    "dojo/domReady!"], 
    function(
      Map,on,dom,parser,FindTask, FindParameters, FeatureLayer, ArcGISDynamicMapServiceLayer, Query,
      Color, SimpleMarkerSymbol, domConstruct, InfoTemplate
  ) {
        
	  //declaro variables generales
    	var findTask, findParams, sUrlDemoService, lyrDemo, sUrlCamLayer, lyrCam, lyrRuta, tbDraw, sUrlRutaLayer ;
    	var map;
    	    	
    	parser.parse();
    	on(dojo.byId("progButtonNode"), "click", selectCam);
    	
    	
		//infoWindow = new InfoWindow({}, domConstruct.create("div", null, dom.byId("map")));

		map = new Map("map", {
	      basemap: "osm",		  
	      center: [-73.578487,  4.133315], // long, lat
	      zoom: 15,
	      sliderStyle: "small"
		});		
		var infoTemplate = new InfoTemplate("${Name}", "Estado: OK ");
		sUrlDemoService = "http://services.arcgis.com/Gj0bu56fK8k4dkXm/arcgis/rest/services/DemoCam/FeatureServer";
	    sUrlCamLayer = "http://services.arcgis.com/Gj0bu56fK8k4dkXm/arcgis/rest/services/DemoCam/FeatureServer/0";
	    sUrlRutaLayer = "http://services.arcgis.com/Gj0bu56fK8k4dkXm/arcgis/rest/services/DemoCam/FeatureServer/1";
	    findTask = new FindTask(sUrlDemoService);
	    lyrDemo = new ArcGISDynamicMapServiceLayer(sUrlDemoService, { opacity : 0.5 });
	    lyrDemo.setVisibleLayers([1, 2]);
	    lyrCam = new FeatureLayer(sUrlCamLayer, {
			outFields : ['*'],
			infoTemplate: infoTemplate
		});
	    lyrRuta = new FeatureLayer(sUrlRutaLayer, {
			outFields : ['*'],
			infoTemplate: infoTemplate
		});
	    
	    map.addLayers([lyrRuta, lyrCam]);
		map.infoWindow.resize(280, 75);		

		map.on("load",function(){
	      map.resize();
	      map.reposition();      
	      findParams = new FindParameters();
          findParams.returnGeometry = true;
          findParams.layerIds = [0];// 
          findParams.searchFields = ["Name"];
          findParams.outSpatialReference = map.spatialReference;		  		  
	    });
		//infoWindow = new InfoWindow({}, domConstruct.create("div"));	
		
		
		function selectCam(nomcamara) {
	    	//simbologia de la seleccion
			nomcamara= dom.byId("dtb").value;//Richi esto se quita 		
	    	var symbolSelected = new SimpleMarkerSymbol({
		        "type": "esriSMS",
		        "style": "esriSMSCircle",
		        "color": [255,115,0,128],
		        "size": 18,
		        "outline":
		        {
		          "color": [255,0,0,214],
		          "width": 2
		        }				
	        });
	        //Seleccionamos de layer haciendo query
	        lyrCam.setSelectionSymbol(symbolSelected);
	        var queryCam = new Query();
			queryCam.returnGeometry = true 	        
			queryCam.where = "Name = '" + nomcamara + "'";
	        lyrCam.selectFeatures(queryCam, FeatureLayer.SELECTION_NEW);
	        
		}
		function selectCam2() {
		    alert("ddd");

		}
		

  });