
let carshopcam = null;
var effect = '';
global.loggedin = false;
global.lastCheck = 0;
global.pocketEnabled = false;
global.esptoggle = 0;
global.freeze = false;
global.trycatchtime = [];


mp.game.audio.stopAllAlarms(true);
mp.game.graphics.setNightvision(false);
mp.game.graphics.setSeethrough(false);

var Peds = [
    /*{ Hash: -39239064, Pos: new mp.Vector3(1395.184, 3613.144, 34.9892), Angle: 270.0 }, // Caleb Baker
    { Hash: -1176698112, Pos: new mp.Vector3(166.6278, 2229.249, 90.73845), Angle: 47.0 }, // Matthew Allen
    { Hash: 1161072059, Pos: new mp.Vector3(2887.687, 4387.17, 50.65578), Angle: 174.0 }, // Owen Nelson
    { Hash: -1398552374, Pos: new mp.Vector3(2192.614, 5596.246, 53.75177), Angle: 318.0 }, // Daniel Roberts
    { Hash: -459818001, Pos: new mp.Vector3(-215.4299, 6445.921, 31.30351), Angle: 262.0 }, // Michael Turner*/

    { Hash: -39239064, Pos: new mp.Vector3(-2.1323678, -1821.9778, 29.543238), Angle: -135.49453 }, // Caleb Baker

    { Hash: 0x9D0087A8, Pos: new mp.Vector3(480.9385, -1302.576, 29.24353), Angle: 224.0 }, // jimmylishman
    { Hash: 1706635382, Pos: new mp.Vector3(-209.004, -1598.8934, 34.86), Angle: 109.549934 }, // Lamar_Davis
    { Hash: 588969535, Pos: new mp.Vector3(112.52232, -1961.4207, 20.94), Angle: 18.294962 }, // Carl_Ballard
    { Hash: -1660909656, Pos: new mp.Vector3(486.2756, -1528.2778, 29.292677), Angle: 65.77025 }, // Chiraq_Bloody
	{ Hash: 653210662, Pos: new mp.Vector3(1435.6263, -1491.5796, 63.62193), Angle: 192.2974 }, // Riki_Veronas
    { Hash: 663522487, Pos: new mp.Vector3(944.5905, -2161.819, 31.188383), Angle: 172.3141 }, // Santano_Amorales
    { Hash: 645279998, Pos: new mp.Vector3(-114.937965, 987.44073, 235.7), Angle: 104.27872 }, // Kirill Orlov
    { Hash: -236444766, Pos: new mp.Vector3(1389.9807, 1140.2769, 114.3), Angle: 87.27906 }, // Aram Mkhitaryan
    { Hash: -1427838341, Pos: new mp.Vector3(-1467.7257, -30.005075, 54.6), Angle: -48.08219 }, // Benjiro Takahashi
    { Hash: -2034368986, Pos: new mp.Vector3(1392.098, 1155.892, 114.4433), Angle: 82.24557 }, // Mario Pellegrini
	
    //{ Hash: -1920001264, Pos: new mp.Vector3(452.2527, -993.119, 30.68958), Angle: 357.7483 }, // Alonzo_Harris
    //{ Hash: 368603149, Pos: new mp.Vector3(441.169, -978.3074, 30.6896), Angle: 160.1411 }, // Nancy_Spungen
    //{ Hash: 1581098148, Pos: new mp.Vector3(454.121, -980.0575, 30.68959), Angle: 86.12 }, // Bones_Bulldog

    //{ Hash: 941695432, Pos: new mp.Vector3(149.1317, -758.3485, 242.152), Angle: 66.82055 }, //  Steve_Hain
    //{ Hash: 1558115333, Pos: new mp.Vector3(120.0836, -726.7773, 242.152), Angle: 248.3546 }, // Michael Bisping
    //{ Hash: 988062523, Pos: new mp.Vector3(253.9357, 228.9332, 101.6832), Angle: 250.3564 }, // Anthony_Young
    //{ Hash: 2120901815, Pos: new mp.Vector3(262.7953, 220.5285, 101.6832), Angle: 337.26 }, // Lorens_Hope
    //{ Hash: 826475330, Pos: new mp.Vector3(247.6933, 219.5379, 106.2869), Angle: 65.78249 }, // Heady_Hunter
    { Hash: -1420211530, Pos: new mp.Vector3(322.785, -586.43, 43.26), Angle: 187.57 }, // Donna_Hart
    //{ Hash: 1092080539, Pos: new mp.Vector3(315.63,-585.912, 43.26), Angle: 75.82 }, // Bruce Hicks
    { Hash: -1306051250, Pos: new mp.Vector3(332.85, -594.66, 43.26), Angle: 2.6 }, // Gregory_Parks
    { Hash: -907676309, Pos: new mp.Vector3(724.8585, 134.1029, 80.95643), Angle: 245.0083 }, // Ronny_Bolls
	//{ Hash: 940330470, Pos: new mp.Vector3(458.7059, -995.118, 25.35196), Angle: 176.8092 }, // Rashkovsky
	//{ Hash: 1596003233, Pos: new mp.Vector3(459.7471, -1000.333, 24.91329), Angle: 177.2829 }, // Muscle Prisoner
	//{ Hash: -265970301, Pos: new mp.Vector3(-244.0758, -2028.765, 29.90606), Angle: 230.1622 }, // Worldof Tankist
	//{ Hash: 1535236204, Pos: new mp.Vector3(-773.945, 313.0294, 85.70606), Angle: 177.9122 }, // Family Register
	//{ Hash: -2063996617, Pos: new mp.Vector3(-696.4318, -1386.611, 5.495), Angle: 77.821 }, // Racing NPC
	{ Hash: mp.game.joaat("s_f_y_ranger_01"), Pos: new mp.Vector3(-2348.598, 3210.923, 29.224812), Angle: 146.52292, PedId: 1, Dimension: 3244522}, // Ketrin Kellerman

	//{ Hash: mp.game.joaat("ig_ramp_hic"), Pos: new mp.Vector3(-521.6952, -244.57239, 36.07903), Angle: -60.142956 }, // Devid Parks
];

setTimeout(function () {
    Peds.forEach(ped => {
		let pedEntity = mp.peds.new(ped.Hash, ped.Pos, ped.Angle, ped.Dimension === undefined ? 0 : ped.Dimension);
		pedEntity.setLodDist(100);

		if (ped.PedId !== undefined && ped.PedId > 0) {
			pedEntity.PedId = ped.PedId;
		}
    });
}, 10000);

gm.events.add('freeze', function (toggle) {
    global.freeze = toggle;
	localplayer.freezePosition(toggle);
	if (localplayer.vehicle) localplayer.vehicle.freezePosition(toggle);
});

gm.events.add('alarm', function (alarm, state) {
	if(!state) mp.game.audio.stopAlarm(alarm, true);
	else {
		while(!mp.game.audio.prepareAlarm(alarm)) mp.game.wait(0);
		mp.game.audio.startAlarm(alarm, true);
	}
});

gm.events.add('setHUDVisible', function (arg) {
    mp.game.ui.displayHud(arg);
    mp.game.ui.displayRadar(arg);
});

var mycambefore = 3;

gm.events.add('setPocketEnabled', function (state) {
    pocketEnabled = state;
    if (state) {
        mp.gui.emmit("fx.set('inpocket')");
		if(mycambefore == 3) mycambefore = mp.game.invoke('0x8D4D46230B2C353A');
        mp.game.cam.setFollowPedCamViewMode(4);
    } else {
        mp.gui.emmit("fx.reset()");
		if(mycambefore != 3) 
		{
			mp.game.cam.setFollowPedCamViewMode(mycambefore);
			mycambefore = 3;
		}
    }
});

global.binderFunctions.acceptPressed = () => {
    if (!global.loggedin || global.chatActive || global.editing || new Date().getTime() - global.lastCheck < 1000 || global.menuCheck() || global.isDemorgan) return;
    mp.events.callRemote('acceptPressed');
    global.lastCheck = new Date().getTime();
};

global.binderFunctions.cancelPressed = () => {
    if (!global.loggedin || global.chatActive || global.editing || new Date().getTime() - global.lastCheck < 1000 || global.menuCheck() || global.isDemorgan) return;
    mp.events.callRemote('cancelPressed');
    global.lastCheck = new Date().getTime();
};

gm.events.add('ready', function () {
    mp.game.ui.displayHud(true);
});

gm.events.add('client.kick', function (notify) {
	if (!global.loggedin)
		global.setStartCam ();

	mp.gui.emmit(`window.router.close()`);
	global.menuOpen();
    //mp.events.call('notify', 4, 9, notify, 10000);
	mp.gui.emmit(`window.router.setPopUp("PopupMain", {Type: "kick", Title: '${translateText("вы были кикнуты")}', Text: '${notify}'})`);
	setTimeout(() => {
		mp.events.callRemote('kickclient');
	}, 1000 * 60 * 3)
});

gm.events.add('client.ban', function (notify) {
	if (!global.loggedin)
		global.setStartCam ();

	mp.gui.emmit(`window.router.close()`);
	global.menuOpen();
	//mp.events.call('notify', 4, 9, notify, 10000);
	mp.gui.emmit(`window.router.setPopUp("PopupMain", {Type: "ban", Title: '${translateText("вы были забанены")}', Text: '${notify}'})`);
	setTimeout(() => {
		mp.events.callRemote('kickclient');
	}, 1000 * 60 * 3)
});

gm.events.add('restart', function (text = translateText("Происходит рестарт сервера. Вам не нужно выходить из игры, ожидайте подключения!")) {
	global.setStartCam ();

	//mp.gui.emmit(`window.router.close()`);
	global.binderFunctions.c_globalEscape (true);
	mp.events.call("client.phone.close");
	global.menuOpen();
	mp.gui.emmit(`window.router.setView("PlayerRestart", '${text}');`);
	//mp.gui.emmit(`window.router.setPopUp("PopupMain", {Type: "restart", Title: translateText("Рестарт сервера"), Text: translateText("В данный момент происходит плановый рестарт сервера, обычно это занимает всего несколько минут. Не отключайтесь от сервера, вас подключит автоматически.")})`);
});

gm.events.add('restart.add', (message) => {
	if (!global.antiFlood("restart_add", 250))
		return;

	mp.events.callRemote('restart.add', message);
});

gm.events.add('restart.send', (name, message) => {
	mp.gui.emmit(`window.listernEvent ('restart.addMessage', '${name}', '${message}');`);
});

let f1pressed = false;
let lastCursorState = false;
let f1Count = 0;

setInterval(() => {
	if (++f1Count > 5 && !f1pressed) {
		f1pressed = true;

		if (!mp.gui.cursor.visible)
			mp.gui.cursor.visible = true;
	}
}, 0);

gm.events.add("render", () => {
	if (f1pressed) {
		f1pressed = false;
		mp.gui.cursor.visible = lastCursorState;
		lastCursorState = false;
	}

	f1Count = 0;
});

mp.keys.bind(global.Keys.VK_F1, true, function () {
	if (!f1pressed) {
		lastCursorState = mp.gui.cursor.visible;
	}
});


global.binderFunctions.interactionPressed = () => {// E key
	if (!global.loggedin || global.chatActive || global.editing || new Date().getTime() - global.lastCheck < 1000 || (global.menuCheck () && !global.isSartMetro)) return;
	if (global.selectFestive != undefined && global.selectFestive != -1 && global.selectFestive.fId != undefined && !global.localplayer.vehicle) 
	{
		if (global.ANTIANIM) return;
		if (new Date().getTime() - global.lastCheckKeyToEvents < (1000 * 60)) 
		{
            mp.events.call('notify', 4, 9, translateText("Немного подождите... на вас действует минутное ограничение, вы очень быстрый."), 10000);
            return;
        }
        mp.events.callRemote('server.events.collect', global.selectFestive.fId);
		return;
	} else if (global.selectFightId != undefined && global.selectFightId != -1 && !global.localplayer.vehicle && !global.canHackAirdrop) {
		if (global.ANTIANIM) return;
		mp.events.callRemote('server.fight.open', global.selectFightId);
		return;	
	} else if (global.selectMatwarFightId != undefined && global.selectMatwarFightId != -1 && !global.localplayer.vehicle && !global.canHackMatWarDrop) {
		if (global.ANTIANIM) return;
		mp.events.callRemote('server.matwar.fight.open', global.selectMatwarFightId);
		return;	
	}/* else if (global.selectPed != undefined && global.selectPed != -1 && global.selectPed.questName && !global.localplayer.vehicle) {
		if (global.ANTIANIM) return; 
		mp.events.callRemote('server.quest.open.' + global.selectPed.questName);
		return;
	} */else if (global.selectBear != undefined && global.selectBear != -1) {
		if (global.ANTIANIM) return; 
		mp.events.callRemote('server.take_quest_item', global.selectBear);
		return;
	} else if (global.dfdayMissionCanPress === true) {
		if (global.ANTIANIM) return; 
		mp.events.call('client.update.npc_dfday_mission');
		return;
	} else if (global.isSeat) {
		if (global.localplayer.vehicle) return; 
		mp.events.call('client.seat');
	} else {
		if (global.isEnter === "buyMetro" || global.isEnter === "exitMetro") mp.events.call('metroEnter');
		else mp.events.callRemote('server.useEvent');
	}
	global.lastCheck = new Date().getTime();
};

global.binderFunctions.playerPressCuffBut = () => {// X key
    if (!global.loggedin || global.chatActive || global.editing || new Date().getTime() - global.lastCheck < 1000 || global.menuCheck () || global.isDemorgan) return;
    mp.events.callRemote('playerPressCuffBut');
    global.lastCheck = new Date().getTime();
};

global.GetWaypointCoords = () => {
	try {
        if(Natives.IS_WAYPOINT_ACTIVE ()) 
        {
			let blipIterator = Natives.GET_WAYPOINT_BLIP_ENUM_ID ();
			let totalBlipsFound = Natives.GET_NUMBER_OF_ACTIVE_BLIPS ();
			let FirstInfoId = Natives.GET_FIRST_BLIP_INFO_ID (blipIterator);
			let NextInfoId = Natives.GET_NEXT_BLIP_INFO_ID (blipIterator);
			for (let i = FirstInfoId, blipCount = 0; blipCount != totalBlipsFound; blipCount++, i = NextInfoId) {
				if (Natives.GET_BLIP_SPRITE (i) == 8) {//Natives.GET_BLIP_INFO_ID_TYPE (i) == 4
					return mp.game.ui.getBlipInfoIdCoord(i);
				}
			}
			return null;
		} else
			return null;
	} catch (e) { 
        return null;
    }
}

global.ap = false;
var apinterval = null;

global.binderFunctions.playerPressFollowBut = () => {// Z key
    if (!global.loggedin || global.chatActive || global.editing || new Date().getTime() - global.lastCheck < 1000 || global.menuCheck () || global.isDemorgan || global.localplayer.vehicle) return;
	mp.events.callRemote('playerPressFollowBut');
	global.lastCheck = new Date().getTime();
};

global.binderFunctions.onAutoPilot = () => {
	try {
		if (!global.loggedin || global.chatActive || global.editing || new Date().getTime() - global.lastCheck < 1000 || global.menuCheck () || global.isDemorgan || !global.localplayer.vehicle) return;
		
		if (global.localplayer.vehicle.getPedInSeat(-1) == global.localplayer.handle) {
			if (global.VehicleSeatFix > new Date().getTime()) return;
			else if (!global.ap) {
				switch(global.localplayer.vehicle.getClass()) {
					case 13: // Cycles
					case 14: // Boats
					case 15: // Helicopters
					case 16: // Planes
					case 18: // Emergency
					case 21: // Trains
						mp.events.call('notify', 1, 9, translateText("Автопилот недоступен."), 1000);
						return;
				}
	
				let engine = global.localplayer.vehicle.getIsEngineRunning();
				if (engine != null && !engine) return;
	
				let petrol = global.Petrol;
				if (petrol !== undefined && petrol <= 0) return;
	
				let coord = GetWaypointCoords();
				if (coord !== null) {
					global.localplayer.taskVehicleDriveToCoordLongrange(global.localplayer.vehicle.handle, coord.x, coord.y, coord.z, 40.0, 831, 50.0);
					global.ap = true;
	
					mp.events.call('notify', 2, 9, translateText("Вы включили автопилот. Обратите внимание, что данная функция не освобождает Вас от RP ситуаций."), 5000);
					mp.gui.emmit(`window.vehicleState.autoPilot (true)`);
	
					if (apinterval == null) {
						apinterval = setInterval(function() {
							let petrol1 = 1;
							let engine1 = true;
	
							if (global.localplayer.vehicle) {
								petrol1 = global.Petrol;
								engine1 = global.localplayer.vehicle.getIsEngineRunning();
							}
	
							if (!Natives.IS_WAYPOINT_ACTIVE () || !global.localplayer.vehicle || (petrol1 !== undefined && petrol1 <= 0) || (engine1 != null && !engine1)) {
								if (global.localplayer.vehicle) {
									if ((petrol1 !== undefined && petrol1 <= 0) || (engine1 != null && !engine1))
										global.localplayer.vehicle.setEngineOn(false, true, false);
	
									global.localplayer.clearTasks();
									mp.events.call('notify', 2, 9, translateText("Автопилот отключён."), 1500);
								}
	
								global.ap = false;
								mp.gui.emmit(`window.vehicleState.autoPilot (false)`);
								clearInterval(apinterval);
								apinterval = null;
							}
						}, 2500);
					}
				}
			} else {
				global.localplayer.clearTasks();
				mp.events.call('notify', 2, 9, translateText("Автопилот отключён."), 1500);
				
				global.ap = false;
				mp.gui.emmit(`window.vehicleState.autoPilot (false)`);
	
				if (apinterval != null) {
					clearInterval(apinterval);
					apinterval = null;
				}
			}
		}
		
		global.lastCheck = new Date().getTime();
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "main", "onAutoPilot", e.toString());
    }
};

global.binderFunctions.onSendWaypoint = () => {
    if (!global.loggedin || global.chatActive || global.editing || new Date().getTime() - global.lastCheck < 1000 || global.menuCheck () || global.isDemorgan || !global.localplayer.vehicle) return;
    
	if (global.localplayer.vehicle.getPedInSeat(-1) != global.localplayer.handle) {
        let coord = GetWaypointCoords();
        if (coord !== null) mp.events.callRemote('syncWaypoint', coord.x, coord.y);
    }
	
    global.lastCheck = new Date().getTime();
};

global.binderFunctions.onSirenSync = () => {
    if (!global.loggedin || global.chatActive || global.editing || new Date().getTime() - global.lastCheck < 1000 || global.menuCheck () || global.isDemorgan || !global.localplayer.vehicle) return;
    
	if (global.localplayer.vehicle.getPedInSeat(-1) == global.localplayer.handle && global.localplayer.vehicle.getClass() == 18) {
		mp.events.callRemote('syncSirenSound');
	}
	
    global.lastCheck = new Date().getTime();
};

gm.events.add('syncWP', function (bX, bY, type) {
    if(!Natives.IS_WAYPOINT_ACTIVE ()) {
		mp.game.ui.setNewWaypoint(bX, bY);
		if(type == 0) mp.events.call('notify', 2, 9, translateText("Пассажир передал Вам информацию о своём маршруте!"), 3000);
		else if(type == 1) mp.events.call('notify', 2, 9, translateText("Человек из списка контактов Вашего телефона передал Вам метку его местоположения!"), 3000);
	} else {
		if(type == 0) mp.events.call('notify', 4, 9, translateText("Пассажир попытался передать Вам информацию о маршруте, но у Вас уже установлен другой маршрут."), 5000);
		else if(type == 1) mp.events.call('notify', 4, 9, translateText("Человек из списка контактов Вашего телефона попытался передать Вам метку его местоположения, но у Вас уже установлена другая метка."), 5000);
	}
});

mp.keys.bind(global.Keys.VK_OEM_3, false, function () { // ` key
	if ((mp.game.ui.isPauseMenuActive() && !mp.gui.cursor.visible) || global.isEditor || global.chatActive || f1pressed || (global.menuCheck () && !global.dropEditor && global.blackjack.selectTable == null && global.rouletteplay == false && !global.horsesplaying && mp.gui.cursor.visible)) return;
    mp.gui.cursor.visible = !mp.gui.cursor.visible;
});

var lastPos = new mp.Vector3(0, 0, 0);

gm.events.add("playerRuleTriggered", (rule, counter) => 
{
    if (rule === 'ping' && counter > 5) {
        mp.events.call('notify', 4, 2, translateText("Ваш ping слишком большой. Зайдите позже"), 5000);
        mp.events.callRemote("kickclient");
    }
    /*if (rule === 'packetLoss' && counter => 10) {
        mp.events.call('notify', 4, 2, translateText("У Вас большая потеря пакетов. Зайдите позже"), 5000);
        mp.events.callRemote("kickclient");
    }*/
});
