var pentloaded = false;
var garageloaded = false;

gm.events.add('pentload', () => {
	try 
	{
		if(pentloaded == false) 
		{
			pentloaded = true;
			// Enable Penthouse interior // Thanks & Credits to root <3
			let phIntID = mp.game.interior.getInteriorAtCoords(976.636, 70.295, 115.164);
			let phPropList = [
				"Set_Pent_Tint_Shell",
				"Set_Pent_Pattern_01",
				"Set_Pent_Spa_Bar_Open",
				"Set_Pent_Media_Bar_Open",
				"Set_Pent_Dealer",
				"Set_Pent_Arcade_Modern",
				"Set_Pent_Bar_Clutter",
				"Set_Pent_Clutter_01",
				"set_pent_bar_light_01",
				"set_pent_bar_party_0"
			];
			for (const propName of phPropList) 
			{
				mp.game.interior.enableInteriorProp(phIntID, propName);
				mp.game.invoke("0xC1F1920BAF281317", phIntID, propName, 1);
			}
			mp.game.interior.refreshInterior(phIntID);
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "world/other", "pentload", e.toString());
	}
});

const OfficeGunsPropList = [
	"swag_guns",
	"swag_guns2",
	"swag_guns3"
];

const OfficeMedPropList = [
	"swag_med",
	"swag_med2",
	"swag_med3",
	"swag_pills",
	"swag_pills2",
	"swag_pills3"
];

const OfficeDrugsPropList = [
	"swag_drugbags",
	"swag_drugbags2",
	"swag_drugbags3",
	"swag_drugstatue",
	"swag_drugstatue2",
	"swag_drugstatue3"
];

const OfficeMoneyPropList = [
	"cash_set_01",
	"cash_set_02",
	"cash_set_03",
	"cash_set_04",
	"cash_set_05",
	"cash_set_06",
	"cash_set_07",
	"cash_set_08",
	"cash_set_09",
	"cash_set_10",
	"cash_set_11",
	"cash_set_12",
	"cash_set_13",
	"cash_set_14",
	"cash_set_15",
	"cash_set_16",
	"cash_set_17",
	"cash_set_18",
	"cash_set_19",
	"cash_set_20",
	"cash_set_21",
	"cash_set_22",
	"cash_set_23",
	"cash_set_24"
];

gm.events.add('OfficePropLoad', (intid, type, count, unload, load) => {
	try 
	{
		if (unload) 
		{
			if(type == 0) 
			{
				for (const propName of OfficeMoneyPropList) 
				{
					if (mp.game.interior.isInteriorPropEnabled(intid, propName))  
					{
						mp.game.interior.disableInteriorProp(intid, propName);
						mp.game.interior.refreshInterior(intid);
					}
				}
			} else if(type == 1) {
				for (const propName of OfficeDrugsPropList) 
				{
					if (mp.game.interior.isInteriorPropEnabled(intid, propName))  
					{
						mp.game.interior.disableInteriorProp(intid, propName);
						mp.game.interior.refreshInterior(intid);
					}
				}
			} else if(type == 2) {
				for (const propName of OfficeMedPropList) 
				{
					if (mp.game.interior.isInteriorPropEnabled(intid, propName))  
					{
						mp.game.interior.disableInteriorProp(intid, propName);
						mp.game.interior.refreshInterior(intid);
					}
				}
			} else if(type == 3) {
				for (const propName of OfficeGunsPropList) 
				{
					if (mp.game.interior.isInteriorPropEnabled(intid, propName))  
					{
						mp.game.interior.disableInteriorProp(intid, propName);
						mp.game.interior.refreshInterior(intid);
					}
				}
			}
		}
		
		if(load) 
		{
			if (count == 0) return;
			let changed = false;
			let loaded = 0;
			if(type == 0) 
			{
				for (const propName of OfficeMoneyPropList) 
				{
					loaded++;
					if (!mp.game.interior.isInteriorPropEnabled(intid, propName))  
					{
						changed = true;
						mp.game.interior.enableInteriorProp(intid, propName);
						mp.game.invoke("0xC1F1920BAF281317", intid, propName, 1);
					}
					if(loaded == count) break;
				}
			}
			else if(type == 1) 
			{
				for (const propName of OfficeDrugsPropList) 
				{
					loaded++;
					if (!mp.game.interior.isInteriorPropEnabled(intid, propName))  
					{
						changed = true;
						mp.game.interior.enableInteriorProp(intid, propName);
						mp.game.invoke("0xC1F1920BAF281317", intid, propName, 1);
					}
					if(loaded == count) break;
				}
			}
			else if(type == 2) 
			{
				for (const propName of OfficeMedPropList) 
				{
					loaded++;
					if (!mp.game.interior.isInteriorPropEnabled(intid, propName))  
					{
						changed = true;
						mp.game.interior.enableInteriorProp(intid, propName);
						mp.game.invoke("0xC1F1920BAF281317", intid, propName, 1);
					}
					if(loaded == count) break;
				}
			}
			else if(type == 3) 
			{
				for (const propName of OfficeGunsPropList) 
				{
					loaded++;
					if (!mp.game.interior.isInteriorPropEnabled(intid, propName))  
					{
						changed = true;
						mp.game.interior.enableInteriorProp(intid, propName);
						mp.game.invoke("0xC1F1920BAF281317", intid, propName, 1);
					}
					if(loaded == count) break;
				}
			}
			if(changed) 
			{
				mp.game.interior.refreshInterior(intid);
				changed = false;
			}
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "world/other", "OfficePropLoad", e.toString());
	}
});

gm.events.add('OfficeAllPropLoad', (intid, count1, count2, count3, count4) => {
	try 
	{
		for (const propName of OfficeMoneyPropList) 
		{
			if (mp.game.interior.isInteriorPropEnabled(intid, propName))  
			{
				mp.game.interior.disableInteriorProp(intid, propName);
				mp.game.interior.refreshInterior(intid);
			}
		}
		for (const propName of OfficeDrugsPropList) 
		{
			if (mp.game.interior.isInteriorPropEnabled(intid, propName))  
			{
				mp.game.interior.disableInteriorProp(intid, propName);
				mp.game.interior.refreshInterior(intid);
			}
		}
		for (const propName of OfficeMedPropList) 
		{
			if (mp.game.interior.isInteriorPropEnabled(intid, propName))  
			{
				mp.game.interior.disableInteriorProp(intid, propName);
				mp.game.interior.refreshInterior(intid);
			}
		}
		for (const propName of OfficeGunsPropList) 
		{
			if (mp.game.interior.isInteriorPropEnabled(intid, propName))  
			{
				mp.game.interior.disableInteriorProp(intid, propName);
				mp.game.interior.refreshInterior(intid);
			}
		}
		let changed = false;
		let loaded = 0;
		if(count1 >= 1) 
		{
			loaded = 0;
			for (const propName of OfficeMoneyPropList) 
			{
				loaded++;
				if (!mp.game.interior.isInteriorPropEnabled(intid, propName))  
				{
					changed = true;
					mp.game.interior.enableInteriorProp(intid, propName);
					mp.game.invoke("0xC1F1920BAF281317", intid, propName, 1);
				}
				if(loaded == count1) break;
			}
		}
		if(count2 >= 1) 
		{
			loaded = 0;
			for (const propName of OfficeDrugsPropList) 
			{
				loaded++;
				if (!mp.game.interior.isInteriorPropEnabled(intid, propName))  
				{
					changed = true;
					mp.game.interior.enableInteriorProp(intid, propName);
					mp.game.invoke("0xC1F1920BAF281317", intid, propName, 1);
				}
				if(loaded == count2) break;
			}
		}
		if(count3 >= 1) 
		{
			loaded = 0;
			for (const propName of OfficeMedPropList) 
			{
				loaded++;
				if (!mp.game.interior.isInteriorPropEnabled(intid, propName))  
				{
					changed = true;
					mp.game.interior.enableInteriorProp(intid, propName);
					mp.game.invoke("0xC1F1920BAF281317", intid, propName, 1);
				}
				if(loaded == count3) break;
			}
		}
		if(count4 >= 1) 
		{
			loaded = 0;
			for (const propName of OfficeGunsPropList) 
			{
				loaded++;
				if (!mp.game.interior.isInteriorPropEnabled(intid, propName))  
				{
					changed = true;
					mp.game.interior.enableInteriorProp(intid, propName);
					mp.game.invoke("0xC1F1920BAF281317", intid, propName, 1);
				}
				if(loaded == count4) break;
			}
		}
		
		if(changed) 
		{
			mp.game.interior.refreshInterior(intid);
			changed = false;
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "world/other", "OfficeAllPropLoad", e.toString());
	}
});

gm.events.add('garageload', () => {
	try 
	{
		if(garageloaded == false) {
			garageloaded = true;
			let phIntID = mp.game.interior.getInteriorAtCoords(-1386.466, -477.74, 55.98);
			let phPropList = [
				"garage_decor_04",
				"lighting_option09",
				"numbering_style09_n1"
			];
			for (const propName of phPropList) 
			{
				mp.game.interior.enableInteriorProp(phIntID, propName);
				mp.game.invoke("0xC1F1920BAF281317", phIntID, propName, 1);
			}
			mp.game.interior.refreshInterior(phIntID);
			
			phIntID = mp.game.interior.getInteriorAtCoords(-1389.609, -471.7082, 77.08);
			mp.game.interior.enableInteriorProp(phIntID, "floor_vinyl_01");
			mp.game.invoke("0xC1F1920BAF281317", phIntID, "floor_vinyl_01", 1);
			mp.game.interior.refreshInterior(phIntID);
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "world/other", "garageload", e.toString());
	}
});

let viewcam = null;

gm.events.add('setmyview', (type) => { // organizations camera
	try 
	{
		if(viewcam !== null) viewcam.destroy();
		if(type == 0) { //garage
			viewcam = mp.cameras.new('default', new mp.Vector3(-1393.059, -489.5243, 60.6802), new mp.Vector3(0, 0, 0), 50);
			viewcam.pointAtCoord(-1382.944, -474.2304, 55.9804);
			viewcam.setActive(true);
			mp.game.cam.renderScriptCams(true, false, 0, true, false);
		} else if(type == 1) { //office minimum 
			viewcam = mp.cameras.new('default', new mp.Vector3(-58.88321, -808.342, 244.266), new mp.Vector3(0, 0, 0), 50);
			viewcam.pointAtCoord(-73.4893, -807.94, 242.266);
			viewcam.setActive(true);
			mp.game.cam.renderScriptCams(true, false, 0, true, false);
		} else if(type == 2) { //office medium
			viewcam = mp.cameras.new('default', new mp.Vector3(-1573.404, -589.33, 109.52), new mp.Vector3(0, 0, 0), 50);
			viewcam.pointAtCoord(-1564.884, -577.37, 108.523);
			viewcam.setActive(true);
			mp.game.cam.renderScriptCams(true, false, 0, true, false);
		} else if(type == 3) { //office maximum
			viewcam = mp.cameras.new('default', new mp.Vector3(-147.934, -645.15, 169.82), new mp.Vector3(0, 0, 0), 50);
			viewcam.pointAtCoord(-135.24, -639.37, 168.82);
			viewcam.setActive(true);
			mp.game.cam.renderScriptCams(true, false, 0, true, false);
		} else if(type == 4) { //garage tuning
			viewcam = mp.cameras.new('default', new mp.Vector3(-1385.194, -479.872, 80.20), new mp.Vector3(0, 0, 0), 50);
			viewcam.pointAtCoord(-1391.366, -471.936, 77.91);
			viewcam.setActive(true);
			mp.game.cam.renderScriptCams(true, false, 0, true, false);
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "world/other", "setmyview", e.toString());
	}
});

gm.events.add('resetmyview', () => {
	try 
	{
		if(viewcam !== null)
		{
			viewcam.destroy();
			mp.game.cam.renderScriptCams(false, false, 3000, true, true);
			viewcam = null;
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "world/other", "resetmyview", e.toString());
	}
});

gm.events.add('loadprophere', (myposx, myposy, myposz, stri, inttype) => {
	try 
	{
		let phIntID;
		if(inttype !== null) phIntID = mp.game.interior.getInteriorAtCoordsWithType(myposx, myposy, myposz, inttype);
		else phIntID = mp.game.interior.getInteriorAtCoords(myposx, myposy, myposz);
		mp.game.interior.enableInteriorProp(phIntID, stri);
		mp.game.invoke("0xC1F1920BAF281317", phIntID, stri, 1);
		mp.game.interior.refreshInterior(phIntID);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "world/other", "loadprophere", e.toString());
	}
});

gm.events.add('loadpropbyint', (phIntID, stri) => {
	try 
	{
		mp.game.interior.enableInteriorProp(phIntID, stri);
		mp.game.invoke("0xC1F1920BAF281317", phIntID, stri, 1);
		mp.game.interior.refreshInterior(phIntID);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "world/other", "loadpropbyint", e.toString());
	}
});

gm.events.add('clearprophere', (myposx, myposy, myposz, stri, inttype) => {
	try 
	{
		let phIntID;
		if(inttype !== null) phIntID = mp.game.interior.getInteriorAtCoordsWithType(myposx, myposy, myposz, inttype);
		else phIntID = mp.game.interior.getInteriorAtCoords(myposx, myposy, myposz);
		if (mp.game.interior.isInteriorPropEnabled(phIntID, stri)) 
		{
			mp.game.interior.disableInteriorProp(phIntID, stri);
			mp.game.interior.refreshInterior(phIntID);
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "world/other", "clearprophere", e.toString());
	}
});

gm.events.add('clearpropbyint', (phIntID, stri) => {
	try 
	{
		if (mp.game.interior.isInteriorPropEnabled(phIntID, stri)) 
		{
			mp.game.interior.disableInteriorProp(phIntID, stri);
			mp.game.interior.refreshInterior(phIntID);
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "world/other", "clearpropbyint", e.toString());
	}
});