var dirtt = null;
var lastdirt;
//var imadriver = false;

mp.game.vehicle.defaultEngineBehaviour = false;

gm.events.add("VehStream_SetSirenSound", (veh, status) => {
    try {
        if (veh && mp.vehicles.exists(veh) && veh.getClass() == 18) {
            veh.setSirenSound(status);
        }
    } catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "VehStream_SetSirenSound", e.toString());
	}
});

let EnterVehicle = false;
gm.events.add("playerEnterVehicle", (vehicle, seat) => {
	try {
		if (seat == -1) {
			EnterVehicle = vehicle;
			setVehicleEngine (vehicle, null);
			//global.localplayer.setConfigFlag (429, true);
			lastdirt = vehicle.getDirtLevel();
			if (dirtt != null) clearInterval(dirtt);
			dirtt = setInterval(function () {
				dirtlevel(vehicle);
			}, 20000);
			if(vehicle.model == 1747439474) vehicle.setMaxSpeed(30);
			else if(vehicle.model == 2034235290) vehicle.setMaxSpeed(40);
			if (vehicle.getVariable('BOOST') != undefined) global.boosted = true;
			else global.boosted = false;
			gm.discord(translateText("За рулём"));
			//imadriver = true;
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "playerEnterVehicle", e.toString());
	}
});

gm.events.add("playerLeaveVehicle", async () => {
    try {
		//global.localplayer.setConfigFlag (429, false);
		global.boosted = false;
		if (dirtt != null) 
		{
			clearInterval(dirtt);
			dirtt = null;
		}

		if (EnterVehicle && mp.vehicles.exists(EnterVehicle)) {
			EnterVehicle.freezePosition(false);
			setVehicleEngine (EnterVehicle, null);
		}
		
		EnterVehicle = false;
    } catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "playerLeaveVehicle", e.toString());
	}
});

function dirtlevel(entity) {
    try {
        if (entity && mp.vehicles.exists(entity) && global.localplayer.vehicle == entity && entity.getPedInSeat(-1) == global.localplayer.handle) {
            mp.events.call("VehStream_GetVehicleDirtLevel", entity);
        } else if (dirtt != null) {
			clearInterval(dirtt);
			dirtt = null;
        }
    } catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "dirtlevel", e.toString());
	}
};

gm.events.add("VehStream_GetVehicleDirtLevel", (entity) => {
    try {
        if (entity && mp.vehicles.exists(entity) && entity.getPedInSeat(-1) == global.localplayer.handle) {
            let curdirt = parseFloat(entity.getDirtLevel());
			let raznica = parseFloat((curdirt - lastdirt));
			if (raznica >= 0.01) {
				raznica = raznica/3;
				let newdirt = parseFloat((lastdirt + raznica));
				if (newdirt > 15) newdirt = 15;
				lastdirt = newdirt;
				mp.events.callRemote("VehStream_SetVehicleDirt", entity, newdirt);
            }
        }
    } catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "VehStream_GetVehicleDirtLevel", e.toString());
	}
});

gm.events.add("vehicleStreamIn", (entity) => {
	setVehicleSyncData(entity);
	setVehicleCover(entity, null);
	setVehicleExtraColours(entity, null);
	setVehicleHeadlights(entity, null);
	setVehicleNeon(entity, null);
	setVehicleDirt(entity, null);
	setVehicleEngine(entity, null);
	setVehicleDoor(entity, null);
	setVehicleLock(entity, null);
	setVehicleIL(entity, null);
});


mp.events.addDataHandler("vExtraColours", (entity, value, oldValue) => {
    setVehicleExtraColours (entity, value);
});

const setVehicleExtraColours = (entity, data) => {
	try {
		if (entity && mp.vehicles.exists(entity) && entity.type === 'vehicle' && entity.handle !== 0) {
			if (!data)
				data = entity.getVariable("vExtraColours");

			if (data) {
				data = data.split("|");
				if (data && data.length > 1) {
					entity.setExtraColours(Number (data[0]), Number (data[1]));
				}
			}
		}
	} catch (e) {
		mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "setVehicleExtraColours", e.toString());
	}
}

mp.events.addDataHandler("vHeadlights", (entity, value, oldValue) => {
    setVehicleHeadlights (entity, value);
});

const setVehicleHeadlights = (entity, data) => {
	try {
		if (entity && mp.vehicles.exists(entity) && entity.type === 'vehicle' && entity.handle !== 0) {
			if (!data)
				data = entity.getVariable("vHeadlights");
			if (data && typeof data === "number") {
				entity.toggleMod(22, true);
				global.SetVehicleLightColor(entity, Number (data));
			}
		}
	} catch (e) {
		//mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "setVehicleHeadlights", e.toString());
	}
}

mp.events.addDataHandler("vCover", (entity, value, oldValue) => {
    setVehicleCover (entity, value);
});

const setVehicleCover = (entity, data) => {
	try {
		if (entity && mp.vehicles.exists(entity) && entity.type === 'vehicle' && entity.handle !== 0) {
			if (!data)
				data = entity.getVariable("vCover");
			if (data != undefined && typeof data === "number" && Number (data) > 0) {
				entity.setModColor1(Number (data), 1, 0);
				entity.setModColor2(Number (data), 1);
			}
		}
	} catch (e) {
		mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "setVehicleCover", e.toString());
	}
}

mp.events.addDataHandler("vNeon", (entity, value, oldValue) => {
    setVehicleNeon (entity, value);
});

const setVehicleNeon = (entity, data) => {
	try {
		if (entity && mp.vehicles.exists(entity) && entity.type === 'vehicle' && entity.handle !== 0) {
			if (!data)
				data = entity.getVariable("vNeon");
			if (data) {
				data = data.split("|");
				if (data && data.length > 2) {
					global.setNeonLight (entity, global.getNeonValuesByIndex(Number (data[0])));
					entity.setNeonLightsColour(Number (data[1]), Number (data[2]), Number (data[3]));
				}
			}
		}
	} catch (e) {
		mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "setVehicleNeon", e.toString());
	}
}

mp.events.addDataHandler("vDirt", (entity, value, oldValue) => {
    setVehicleDirt (entity, value);
});

const setVehicleDirt = (entity, data) => {
	try {
		if (entity && mp.vehicles.exists(entity) && entity.type === 'vehicle' && entity.handle !== 0) {
			if (!data)
				data = entity.getVariable("vDirt");
		
			if (data != undefined && typeof data === "number") {
				entity.setDirtLevel(parseFloat(data));
				if (entity.getPedInSeat(-1) == global.localplayer.handle)
					lastdirt = data;
			}
		}
	} catch (e) {
		mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "setVehicleDirt", e.toString());
	}
}

mp.events.addDataHandler("vEngine", (entity, value, oldValue) => {
    setVehicleEngine (entity, value);
});

const setVehicleEngine = (entity, data) => {
	try {
		if (entity && mp.vehicles.exists(entity) && entity.type === 'vehicle' && entity.handle !== 0) {
			if (!data)
				data = entity.getVariable("vEngine");

			//entity.__cEngine = data;

			if (data != undefined && typeof data === "number") {
				UpdateVehicleEngine (entity, Boolean (data));
			}
		}
	} catch (e) {
		mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "setVehicleEngine", e.toString());
	}
}

mp.events.addDataHandler("vIL", (entity, value, oldValue) => {
    setVehicleIL (entity, value);
});

const setVehicleIL = (entity, data) => {
	try {
		if (entity && mp.vehicles.exists(entity) && entity.type === 'vehicle' && entity.handle !== 0) {
			if (!data)
				data = entity.getVariable("vIL");
			if (data) {
				data = data.split("|");
				if (data && data.length > 1) {
					data[0] = Number (data[0]);
					if (data[0]) entity.setIndicatorLights(0, true);
					else entity.setIndicatorLights(0, false);
					
					data[1] = Number (data[1]);
					if (data[1]) entity.setIndicatorLights(1, true);
					else entity.setIndicatorLights(1, false);

					if (entity.getPedInSeat(-1) == global.localplayer.handle) {
						if (data[0]) mp.gui.emmit(`window.vehicleState.rightIL (true)`);
						else mp.gui.emmit(`window.vehicleState.rightIL (false)`);
				
						if (data[1]) mp.gui.emmit(`window.vehicleState.leftIL (true)`);
						else mp.gui.emmit(`window.vehicleState.leftIL (false)`);
					}
				}
			}
		}
	} catch (e) {
		mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "setVehicleIL", e.toString());
	}
}

mp.events.addDataHandler("vLock", (entity, value, oldValue) => {
    setVehicleLock (entity, value);
});

const setVehicleLock = (entity, data) => {
	try {
		if (entity && mp.vehicles.exists(entity) && entity.type === 'vehicle' && entity.handle !== 0) {
			const isStream = data === null;
			if (!data)
				data = entity.getVariable("vLock");
			if (data != undefined && typeof data === "number") {
				data = Boolean (data);
				if (data)
					entity.setDoorsLocked(2);
				else
					entity.setDoorsLocked(1);
				if (!isStream) {
					if (data) mp.game.audio.playSoundFromEntity(1, "Remote_Control_Close", entity.handle, "PI_Menu_Sounds", true, 0);
					else mp.game.audio.playSoundFromEntity(1, "Remote_Control_Open", entity.handle, "PI_Menu_Sounds", true, 0);
				}
				if (entity.getPedInSeat(-1) == global.localplayer.handle) {
					if (data) mp.gui.emmit(`window.vehicleState.doors (true)`);
					else mp.gui.emmit(`window.vehicleState.doors (false)`);
				}
			}
		}
	} catch (e) {
		mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "setVehicleLock", e.toString());
	}
}

gm.events.add("client.vehicle.door", (vehicle, doorId, status) => {
    try {
		if (vehicle && mp.vehicles.exists(vehicle) && vehicle.type === 'vehicle' && vehicle.handle !== 0) {
			if (status === 0)
				vehicle.setDoorShut(doorId, false);
			else if (status === 1)
				vehicle.setDoorOpen(doorId, false, false);
			else
				vehicle.setDoorBroken(doorId, true);
        }
    } catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "VehStream_SetLockStatus", e.toString());
	}
});

const setVehicleDoor = (entity, data) => {
	try {
		if (entity && mp.vehicles.exists(entity) && entity.type === 'vehicle' && entity.handle !== 0) {
			if (!data)
				data = entity.getVariable("vDoor");
				
			if (global.IsJsonString (data)) {
				data = JSON.parse(data);
				for (let _d = 0; _d < 6; _d++) {
					if (data && data [_d] !== undefined) {
						if (data [_d] === 0)
							entity.setDoorShut(_d, false);
						else if (data [_d] === 1)
							entity.setDoorOpen(_d, false, false);
						else
							entity.setDoorBroken(_d, true);
					}
				}
			}
		}
	} catch (e) {
		mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "setVehicleDoor", e.toString());
	}
}

const setVehicleSyncData = (entity) => {
	try {
		if (entity && mp.vehicles.exists(entity) && entity.type === 'vehicle' && entity.handle !== 0) {

			for (let _db = 0; _db < 8; _db++) {
				entity.setDoorBreakable(_db, false);
			}
			setTimeout(function () {
				for (let _db = 0; _db < 8; _db++) {
					if (entity && mp.vehicles.exists(entity)) {
						entity.setDoorBreakable(_db, true);
					}
				}
			}, 1500);

			entity.trackVisibility();

			if (entity.getClass() == 18) {
				let sirenData = entity.getVariable('SIRENSOUND');
				if (sirenData !== undefined) 
					entity.setSirenSound(Boolean (sirenData));
			}

			//if (entity.getVariable('isSirenOn') === true) entity.setSiren(true);
		}
	} catch (e) {
		mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "setVehicleSyncData", e.toString());
	}
}

global.getVehicleIndicatorsInfo = (vehicle) => {

	try {
		if (vehicle.getVariable("vLock")) mp.gui.emmit(`window.vehicleState.doors (false)`);
		else mp.gui.emmit(`window.vehicleState.doors (true)`);

		if (vehicle.getVariable("vEngine")) mp.gui.emmit(`window.vehicleState.engine (true)`);
		else mp.gui.emmit(`window.vehicleState.engine (false)`);
		
		let vIL = vehicle.getVariable("vIL");
		
		if (vIL) {
			vIL = vIL.split("|");
			if (vIL && vIL.length > 1) {
				vIL[0] = Number (vIL[0]);
				vIL[1] = Number (vIL[1]);
			} else {
				vIL = [
					0,
					0
				]
			}
		} else {
			vIL = [
				0,
				0
			]
		}
		if (vIL[0]) mp.gui.emmit(`window.vehicleState.rightIL (true)`);
		else mp.gui.emmit(`window.vehicleState.rightIL (false)`);

		if (vIL[1]) mp.gui.emmit(`window.vehicleState.leftIL (true)`);
		else mp.gui.emmit(`window.vehicleState.leftIL (false)`);

	} catch (e) {
		mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "getVehicleIndicatorsInfo", e.toString());
	}

}

const UpdateVehicleEngine = async (vehicle, toggle) => {
	try {
		if (vehicle && mp.vehicles.exists(vehicle) && vehicle.type === 'vehicle' && vehicle.getIsEngineRunning() !== toggle) {
			vehicle.setEngineOn(toggle, true, true);
			vehicle.setUndriveable(!toggle);
			vehicle.setLights(!toggle ? 1 : 0);

			if (vehicle.getPedInSeat(-1) == global.localplayer.handle) {
				if (toggle) mp.gui.emmit(`window.vehicleState.engine (true)`);
				else mp.gui.emmit(`window.vehicleState.engine (false)`);
			}
		}
	} catch (e) {
		mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "UpdateVehicleEngine", e.toString());
	}
}

mp.events.addDataHandler("PETROL", (entity, value, oldValue) => {
    try {
		if (entity && mp.vehicles.exists(entity) && entity.type === 'vehicle' && entity.getPedInSeat(-1) == global.localplayer.handle) {
			global.Petrol = Number(value);
			mp.gui.emmit(`window.vehicleState.fuel(${value}, ${entity.getClass()})`);
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "PETROL", e.toString());
	}
});
/*
let eventsRefresh = new Date().getTime();
setInterval(() => {
    try {
		if (global.localplayer.vehicle && global.localplayer.vehicle.getPedInSeat(-1) == global.localplayer.handle && global.localplayer.vehicle.isSirenOn() != global.localplayer.vehicle.getVariable('isSirenOn')) {
			if (new Date().getTime() - eventsRefresh < 10000) {
				return;
			}
			
			eventsRefresh = new Date().getTime();
			mp.events.callRemote("VehStream_updateSirenStatus", global.localplayer.vehicle, global.localplayer.vehicle.isSirenOn());
		}
    }
	catch (e) 
	{
		if(new Date().getTime() - global.trycatchtime["vehicle/vehiclesync"] < 60000) return;
		global.trycatchtime["vehicle/vehiclesync"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "sirenInterval", e.toString());
	}
}, 1000);*/

mp.events.addDataHandler("isTicket", (entity, value, oldValue) => {
	if (entity && mp.vehicles.exists(entity) && entity.type === 'vehicle') {
		entity.isTicket = !!value;
	}
});