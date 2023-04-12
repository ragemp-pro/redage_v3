global.binderFunctions.engineCarPressed = () => {// Включить/выключить двигатель
	try
	{
		if (!global.loggedin || global.chatActive || global.editing || new Date().getTime() - global.lastCheck < 400 || global.menuCheck () || global.ap) return;
		else if (global.VehicleSeatFix > new Date().getTime()) return;
		if (global.localplayer.isInAnyVehicle(false) && global.localplayer.vehicle.getSpeed() <= 3) {
			global.lastCheck = new Date().getTime();
			mp.events.callRemote('engineCarPressed');
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/control", "engineCarPressed", e.toString());
	}
};

global.binderFunctions.lockCarPressed = () => {// Открыть/закрыть дверьдвигатель
	try
	{
		if (!global.loggedin || chatActive || global.editing || new Date().getTime() - global.lastCheck < 1000 || global.menuCheck ()) return;
		else if (global.VehicleSeatFix > new Date().getTime()) return;
		
		const localPos = global.localplayer.position;
		let indexDoor = -1;
		let dist;
		let LastDist = 9999;
		let DoorData;
		for (let key in global.DoorsDynamicDataToDist) {
			DoorData = global.DoorsDynamicDataToDist [key];
			if (DoorData.hash) {
				dist = mp.game.gameplay.getDistanceBetweenCoords(DoorData.position.x, DoorData.position.y, DoorData.position.z, localPos.x, localPos.y, localPos.z, true);
				if (dist < (DoorData.distance ? DoorData.distance : 2.1) && LastDist > dist) {
					indexDoor = key;
					LastDist = dist;
				}
			}
		}
		if (indexDoor != -1)
			mp.events.callRemote('server.doorControl', indexDoor);
		else
			mp.events.callRemote('lockCarPressed');
		global.lastCheck = new Date().getTime();
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/control", "lockCarPressed", e.toString());
	}
};

global.binderFunctions.lightleft = () => {// Левый поворотникдвигатель
	try
	{
		if(mp.gui.cursor.visible || !global.loggedin) return;
		else if (global.VehicleSeatFix > new Date().getTime()) return;
		if(global.localplayer.vehicle) {
			if(global.localplayer.vehicle.getPedInSeat(-1) != global.localplayer.handle) return;
			if(new Date().getTime() - global.lastCheck > 500) {
				global.lastCheck = new Date().getTime();
				mp.events.callRemote("VehStream_SetIndicatorLightsData", global.localplayer.vehicle, true, false);
			}
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/control", "lightleft", e.toString());
	}
};

global.binderFunctions.lightright = () => {// Правый поворотникдвигатель
	try
	{
		if(mp.gui.cursor.visible || !global.loggedin) return;
		else if (global.VehicleSeatFix > new Date().getTime()) return;
		if(global.localplayer.vehicle) {
			if(global.localplayer.vehicle.getPedInSeat(-1) != global.localplayer.handle) return;
			if(new Date().getTime() - global.lastCheck > 500) {
				global.lastCheck = new Date().getTime();
				mp.events.callRemote("VehStream_SetIndicatorLightsData", global.localplayer.vehicle, false, true);
			}
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/control", "lightright", e.toString());
	}
};

global.binderFunctions.signaling = () => {// Аварийная сигнализациядвигатель
	try
	{
		if(mp.gui.cursor.visible || !global.loggedin) return;
		else if (global.VehicleSeatFix > new Date().getTime()) return;
		if(global.localplayer.vehicle) {
			if(global.localplayer.vehicle.getPedInSeat(-1) != global.localplayer.handle) return;
			if(new Date().getTime() - global.lastCheck > 500) {
				global.lastCheck = new Date().getTime();
				mp.events.callRemote("VehStream_SetIndicatorLightsData", global.localplayer.vehicle, true, true);
			}
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/control", "signaling", e.toString());
	}
};