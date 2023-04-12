global.entity = null;
global.nearestObject = null;
global.boosted = false;

var lastEntCheck = 0;
var checkInterval = 200;

var backlightColor = [196, 17, 21];

global.cuffed = false;

global.isInSafeZone = false;
global.isInCity = false;

var lastCuffUpdate = new Date().getTime();
var lastTyreUpdate = new Date().getTime();

gm.events.add('CUFFED', function (argument) {
	try 
	{
		global.cuffed = argument;
    } 
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/render", "CUFFED", e.toString());
	}
});

var safezonetimeout = null;

gm.events.add('safeZone', function (argument) {
	try 
	{
		global.isInSafeZone = argument;
		mp.gui.emmit(`window.hudStore.greenZone (${argument})`);
		if(global.isAdmin === true) return;
		if(!argument) 
		{
			if(safezonetimeout != null) 
			{
				clearTimeout(safezonetimeout);
				safezonetimeout = null;
			}
			global.localplayer.setInvincible(false);
			mp.events.callRemote('IsSafeZone', false);
			return;
		} 
		else 
		{
			if(safezonetimeout == null) 
			{
				safezonetimeout = setTimeout(function () 
				{
					if(global.isInSafeZone) {
						global.localplayer.setInvincible(true);
						mp.events.callRemote('IsSafeZone', true);
					}
					if(safezonetimeout != null) 
					{
						clearTimeout(safezonetimeout);
						safezonetimeout = null;
					}
				},	15000);
			}
		}
    } 
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/render", "safeZone", e.toString());
	}
});

gm.events.add('insideCity', function (argument) {
	global.isInCity = argument;
});

global.binderFunctions.openCircleMenu = () => {// G key
	try 
	{
		if (global.menuCheck() || global.isDemorgan == true || global.attachedtotrunk || global.isDeath == true || (global.inAirsoftLobby !== undefined && global.inAirsoftLobby >= 0) || new Date().getTime() - global.lastCheck < 1000) return;

		if (global.localplayer.vehicle) {
			global.entity = global.localplayer.vehicle;
			global.OpenCircle(translateText("В машине"), 0, global.entity);
			return;
		}

		global.entity = GetLooking (false, true);

		if (!global.loggedin || global.chatActive || global.entity == null || !global.entity.doesExist()) {
			global.OpenCircle(translateText("Я"), 0);
			return;
		}

		global.lastCheck = new Date().getTime();

		switch (global.entity.type) {
			case "object":
				if (global.entity != null && global.entity.doesExist() && global.entity.isAnObject() && global.entity['dropData'] && global.entity['dropData'].pId !== undefined) {
					
					switch (global.entity['dropData'].ItemId) {
						case 249: 
							global.OpenCircle(translateText("Кальян"), 0, global.entity);
							break;
						default:
							mp.gui.cursor.visible = true;
							break;
					}
				}
				return;
			case "player":
				if(global.GetGender (global.entity) !== -1) {
					mp.gui.cursor.visible = true;
					global.OpenCircle(translateText("Игрок"), 0, global.entity);
				}
				return;
			case "vehicle":
				mp.gui.cursor.visible = true;
				global.OpenCircle(translateText("Машина"), 0, global.entity);
				return;
			default:
				global.OpenCircle(translateText("Я"), 0);
				return;
		}
	} 
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/render", "global.binderFunctions.openCircleMenu", e.toString());
	}
};

global.binderFunctions.dropObject = () => {// F2 key
	try 
	{
		if (global.menuCheck() || global.isDeath == true || global.isDemorgan == true || global.attachedtotrunk) 
			return;
		// player
		if (global.circleOpen) {
			global.CloseCircle();
			return;
		}
		
		global.entity = GetLooking (true, true);

		if (!global.loggedin || global.chatActive || global.entity == null || new Date().getTime() - global.lastCheck < 1000) 
			return;

		global.lastCheck = new Date().getTime();
		if (global.entity != null && global.entity.doesExist() && global.entity.type == 'object' && global.entity.isAnObject()) {				
			if (global.entity['furniture'])
				mp.events.callRemote('oSelected', global.entity.remoteId);
			else if (global.entity['dropData'] && global.entity['dropData'].pId === undefined)
				mp.events.callRemote('server.raise', global.entity);
		} /*else if (global.entity && mp.players.exists(global.entity)) {
			if(global.GetGender (global.entity) !== -1) {
				mp.gui.cursor.visible = true;
				global.OpenCircle(translateText("Игрок"), 0, global.entity);
			}
		}*/
    } 
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/render", "global.binderFunctions.dropObject", e.toString());
	}
};

var engpm = 0;
var engtm = 1;
gm.events.add('svem', (pm, tm) => 
{
	engpm = pm;
	engtm = tm;
});



/*
const systemdata = mp.system;
systemdata.isFocused
*/

const isNotAttack = () => {
	if ([7, 9].includes (global.fractionId) && [911657153].includes (global.weaponData.weapon)) 
		return false;
	else if ([600439132, 126349499].includes (global.weaponData.weapon)) 
		return false;
	else if (global.isInSafeZone) 
		return true;
	else if (global.isDemorgan == true)
		return true;
	else if (global.attachedtotrunk)
		return true;
	else if (global.dmgdisabled)
		return true;
	
	return false;
}

new class extends debugRender {
	constructor() {
		super("r_player_render1");
	}

	render () {
		if (!global.loggedin) return;

		if (global.freeze) {
			global.ToggleFightControls();
			global.ToggleMovementControls();
		}

        mp.game.player.restoreStamina(100);
		global.localplayer.setRagdollFlag(2);
		global.localplayer.setAccuracy(0);
		mp.game.player.setHealthRechargeMultiplier(0.0);
        mp.game.player.setLockonRangeOverride(1.5);
        mp.game.controls.disableControlAction(1, 7, true); // SlowMo в машине

		//thanks to kemperrr
		if (mp.game.invoke(getNative('IS_CUTSCENE_ACTIVE'))) 
			mp.game.invoke(getNative('STOP_CUTSCENE_IMMEDIATELY'));
	    if (mp.game.invoke(getNative('GET_RANDOM_EVENT_FLAG'))) 
			mp.game.invoke(getNative('SET_RANDOM_EVENT_FLAG'), false);
		if (mp.game.invoke(getNative('GET_MISSION_FLAG')))
			mp.game.invoke(getNative('SET_MISSION_FLAG'), false);

		if (pocketEnabled) 
			mp.game.controls.disableControlAction(2, 0, true);

        if (isNotAttack ()) 
		{
			mp.game.controls.disableControlAction(2, 24, true);
            mp.game.controls.disableControlAction(2, 69, true);
            mp.game.controls.disableControlAction(2, 70, true);
            mp.game.controls.disableControlAction(2, 92, true);
            mp.game.controls.disableControlAction(2, 114, true);
            mp.game.controls.disableControlAction(2, 121, true);
            mp.game.controls.disableControlAction(2, 140, true);
            mp.game.controls.disableControlAction(2, 141, true);
            mp.game.controls.disableControlAction(2, 142, true);
            mp.game.controls.disableControlAction(2, 257, true);
            mp.game.controls.disableControlAction(2, 263, true);
            mp.game.controls.disableControlAction(2, 264, true);
            mp.game.controls.disableControlAction(2, 331, true);
			mp.game.player.disableFiring(true);
		}
		if (global.localplayer.isInAnyVehicle(false) && global.localplayer.vehicle)
		{
			let mvs = global.localplayer.vehicle;
			if(mvs.getPedInSeat(-1) == global.localplayer.handle) 
			{
				let mvsclass = mvs.getClass();
				mvs.setTyresCanBurst(true);
				if(engtm != 1) mvs.setEngineTorqueMultiplier(engtm);
				if(engpm == 0) 
				{
					if (new Date().getTime() - lastTyreUpdate >= 1000) 
					{
						lastTyreUpdate = new Date().getTime();
						if(mvsclass != 8 && mvsclass != 13 && (mvs.isTyreBurst(4, true) || mvs.isTyreBurst(5, true))) mvs.setEnginePowerMultiplier(-90);
						else 
						{
							if(global.isInCity) mvs.setEnginePowerMultiplier(-40);
							else if(global.boosted) mvs.setEnginePowerMultiplier(20);
							else mvs.setEnginePowerMultiplier(0);
						}
					}
				} else mvs.setEnginePowerMultiplier(engpm);
				if(mvs.isInAir() && global.localplayer.isInAnyPlane()) mp.game.controls.disableControlAction(0, global.Inputs.VEH_FLY_THROTTLE_DOWN, true);
			}
		} else if (global.cuffed && !global.localplayer.vehicle && new Date().getTime() - lastCuffUpdate >= 3000) {
			mp.events.callRemote("cuffUpdate");
	        lastCuffUpdate = new Date().getTime();
		}

		if (global.localplayer['AFK_STATUS']) {
			mp.game.graphics.drawText(translateText("Вам установлен статус AFK"), [0.5, 0.9], {
				font: 4,
				color: [255, 255, 255, 255],
				scale: [0.4, 0.4],
				centre: true,
			});
		}
	}
};


//*************************************************************** */
const interactionDistance = 2.5;

const getEntityPointToPoint = (flags) => {
	const getBoneCoords = mp.players.local.getBoneCoords(12844, 0.5, 0, 0);
	
	const activeResolution = mp.game.graphics.getScreenActiveResolution(1, 1);
	const screen2dToWorld3d = mp.game.graphics.screen2dToWorld3d([
		activeResolution.x / 2,
		activeResolution.y / 2,
		0
	]);

	if (null == screen2dToWorld3d)
		return null;

	getBoneCoords.z -= 0.3;

	const testPointToPoint = mp.raycasting.testPointToPoint(getBoneCoords, screen2dToWorld3d, mp.players.local, flags);

	if (testPointToPoint) {			
		if (testPointToPoint.entity.type === void(0))
			return null;
			
		return testPointToPoint.entity;
	}
	return null;
}

const getLookingAtEntity = () => {
	try {
		const testPointToPoint = getEntityPointToPoint (global.RAYCASTING_FLAGS.vehicles | global.RAYCASTING_FLAGS.players | global.RAYCASTING_FLAGS.players2 | global.RAYCASTING_FLAGS.objects);

		if (testPointToPoint) {			
			if (testPointToPoint.type !== "player" && testPointToPoint.type !== "vehicle")
				return null;

			const ePosition = testPointToPoint.position;
			const pPosition = localplayer.position;
	
			if (mp.game.gameplay.getDistanceBetweenCoords(ePosition.x, ePosition.y, ePosition.z, pPosition.x, pPosition.y, pPosition.z, true) > 6.0)
				return null;
	
			return testPointToPoint;
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/render", "getLookingAtEntity", e.toString());
	}
	
	return null;
}

global.clearScript = (entity) => {
	if (typeof(entity) === 'number' && entity !== 0/* && mp.game.entity.isAnObject(entity)*/)
		mp.game.shapetest.releaseScriptGuidFromEntity(entity);
}

const getNearestLookingObject = () => {
	try {
		let getBoneCoords = localplayer.getBoneCoords(12844, 0.5, 0, 0);
		const activeResolution = mp.game.graphics.getScreenActiveResolution(1, 1);
		const screen2dToWorld3d = mp.game.graphics.screen2dToWorld3d([
			activeResolution.x / 2,
			activeResolution.y / 2,
			0
		]);

		if (screen2dToWorld3d == undefined)
			return null;	
			
		getBoneCoords.z -= 0.3;
	
		const testPointToPoint = mp.raycasting.testPointToPoint(getBoneCoords, screen2dToWorld3d, global.localplayer, -1);
		if (!testPointToPoint)
			return null;

		const
			position = testPointToPoint.position,
			objects = [];

		//global.clearScript (testPointToPoint.entity);

		mp.objects.forEachInStreamRangeItems(object => {
			const getCoords = object.getCoords(true),
				objectDist = global.vdist2(position, getCoords), 
				playerDist = global.vdist2(global.localplayer.position, getCoords);
			if (objectDist <= 2.5 && playerDist <= 2.5) {
				objects.push({
					'distance': objectDist,
					'entity': object
				});
			}
		});
		const object = objects.sort((object1, object2) => object1.distance - object2.distance)[0];
		
		return object ? object.entity : null;
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/render", "getNearestLookingObject", e.toString());
	}
}


gm.events.add(global.renderName ["500ms"], () => {
	if (!global.loggedin) return;
	GetLooking ();
});

gm.events.add(global.renderName ["5s"], () => {
	if (!global.loggedin) return;
	GetLooking (true);
});

let antiFloodLooking = 0; 

const GetLooking = (isObj = false, isBtn = false) => {
	try {
		if (!global.loggedin) 
			return null;
		else if (antiFloodLooking > new Date().getTime())
			return global.entity;

		if (!isBtn && global.entity !== null && global.entity.doesExist()) {

			const ePosition = global.entity.position;
			const pPosition = localplayer.position;
	
			if (mp.game.gameplay.getDistanceBetweenCoords(ePosition.x, ePosition.y, ePosition.z, pPosition.x, pPosition.y, pPosition.z, true) <= 5.0)
				return global.entity;
		}


		antiFloodLooking = new Date().getTime() + 250;

		if (!global.localplayer.vehicle) {
			let getEntity = null;

			if (!isObj)
				getEntity = getLookingAtEntity();
			
			if ((getEntity || ((isObj || isBtn) && (getEntity = getNearestLookingObject()))) && getEntity && getEntity.type && [
				"vehicle",
				"object",
				"player"
			].includes(getEntity.type)) {
				if (getEntity === global.entity) 
					return getEntity;
				if (!global.localplayer.hasClearLosTo(getEntity.handle, 17)) {
					global.entity = null;
					return null;
				}
				if ("vehicle" === getEntity.type && getEntity.getVariable("isDrone")) {
					global.entity = null;
					return null;
				}
				if ("player" === getEntity.type && getEntity['INVISIBLE']) {
					global.entity = null;
					return null;
				}
				if ("player" === getEntity.type && global.GetGender (getEntity) === -1) {
					global.entity = null;
					return null;
				}

				global.entity = getEntity;
				
				return getEntity;
			}
		}/* else {
			mp.players.forEachInStreamRange(player => {
				if(player !== global.localplayer && global.localplayer.vehicle == ) 
			});
		}*/
		global.entity = null;
	}
	catch (e) 
	{
		if(new Date().getTime() - global.trycatchtime["player/render1"] < 60000) return;
		global.trycatchtime["player/render1"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "player/render", "time.5s", e.toString());
	}
	return null;
};


const colorsData = [
	[255, 255, 255]
];

let markerToPlayer = null;
let markerRotation = 360;

let focusObject = null;

new class extends debugRender {
	constructor() {
		super("r_player_render1");
	}

	render () {
		if (!global.loggedin) return;
		else if (!global.showhud) return;
		
		if (markerToPlayer)
			markerToPlayer.destroy(),
			markerToPlayer = null;

		if (global.entity && !global.entity.doesExist()) {
			//global.entity = null;

			GetLooking ();
			return;
		}

		//

		if (global.circleOpen) {
			global.UpdateCircle (global.entity);
			return;
		}

		if (global.entity) {
			const type = global.entity.type;
			const position = global.entity.position;

			if (type == "player") {
				if (global.localplayer.isInAnyVehicle(false)) {
					mp.game.graphics.drawText(global.Keys[global.userBinder[32].keyCode], [position.x, position.y, position.z], {
						font: 0,
						color: [255, 255, 255, 185],
						scale: [0.4, 0.4],
						outline: true
					});
				} else {					
					const fractionID = 0;
					if (--markerRotation < 0) markerRotation = 380;

					markerToPlayer = mp.markers.new(27, new mp.Vector3(position.x, position.y, position.z - 0.975), 1, {
						'rotation': new mp.Vector3(0, 0, markerRotation),
						'color': [
							colorsData[fractionID][0],
							colorsData[fractionID][1],
							colorsData[fractionID][2],
							185
						],
						'visible': true,
						'dimension': global.localplayer.dimension
					});

					mp.game.graphics.drawText(global.Keys[global.userBinder[31].keyCode], [position.x, position.y, position.z], {
						font: 0,
						color: [255, 255, 255, 185],
						scale: [0.4, 0.4],
						outline: true
					});
				}
			} else if(global.entity.type == "vehicle" && global.entity != localplayer.vehicle) {
				mp.game.graphics.drawText(global.Keys[global.userBinder[31].keyCode], [position.x, position.y, position.z], {
					font: 0,
					color: [255, 255, 255, 185],
					scale: [0.4, 0.4],
					outline: true
				});
			} else if(global.entity.type == "object") {				
				if (global.entity !== focusObject) {
					global.GetItemData (global.entity);
					focusObject = global.entity;
				} else {
					mp.gui.emmit(`window.hudItem.dropFocus ()`);
				}

				if (global.entity['dropData'] && (global.entity['dropData'].pId != undefined || global.entity['dropData'].fId != undefined)) {					
					let textKey = global.Keys[global.userBinder[31].keyCode];

					if (global.isAdmin && global.entity['dropData'].pId != undefined) {
						const player = mp.players.atRemoteId(global.entity['dropData'].pId);
						if (player) {
							textKey += `\n(( ${player.name} ))`
						}
					}
					
					mp.game.graphics.drawText(textKey, [position.x, position.y, position.z], {
						font: 0,
						color: [255, 255, 255, 185],
						scale: [0.4, 0.4],
						outline: true
					});
				} else {
					mp.game.graphics.drawText(global.Keys[global.userBinder[32].keyCode], [position.x, position.y, position.z], {
						font: 0,
						color: [255, 255, 255, 185],
						scale: [0.4, 0.4],
						outline: true
					});					
				}
			}
		}
	}
};