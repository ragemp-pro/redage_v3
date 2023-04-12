require('./scaleform.js');
let horses = [
    {
        "id": 1,
        "name": "Sokolyansky",
        "primaryColour": 15553363,
        "secondaryColour": 5474797,
        "horseColour": 9858144,
        "maneColour": 4671302,
        "rate": "x 3"
    },
    {
        "id": 2,
        "name": "Maya",
        "primaryColour": 16724530,
        "secondaryColour": 3684408,
        "horseColour": 14807026,
        "maneColour": 16777215,
        "rate": "x 3"
    },
    {
        "id": 3,
        "name": "Source",
        "primaryColour": 13560920,
        "secondaryColour": 15582764,
        "horseColour": 16770746,
        "maneColour": 7500402,
        "rate": "x 3"
    },
    {
        "id": 4,
        "name": "Shaman",
        "primaryColour": 16558591,
        "secondaryColour": 5090807,
        "horseColour": 10446437,
        "maneColour": 7493977,
        "rate": "x 3"
    },
    {
        "id": 5,
        "name": "Deluxe",
        "primaryColour": 5090807,
        "secondaryColour": 16558591,
        "horseColour": 3815994,
        "maneColour": 9393493,
        "rate": "x 3"
    },
    {
        "id": 6,
        "name": "Mip",
        "primaryColour": 16269415,
        "secondaryColour": 16767010,
        "horseColour": 10329501,
        "maneColour": 16777215,
        "rate": "x 3"
    }
];
let seats = [
    {
        'x': 1099.582,
        'y': 265.6582,
        'z': -52.2409,
        'r': 45,
        'side': 2
    },
    {
        'x': 1098.928,
        'y': 265.0042,
        'z': -52.24094,
        'r': 45,
        'side': 1
    },
    {
        'x': 1096.506,
        'y': 262.6327,
        'z': -52.24094,
        'r': 45,
        'side': 2
    },
    {
        'x': 1095.852,
        'y': 261.9783,
        'z': -52.24094,
        'r': 45,
        'side': 1
    },
    {
        'x': 1095.127,
        'y': 261.2535,
        'z': -52.24094,
        'r': 45,
        'side': 0x2
    },
    {
        'x': 1094.473,
        'y': 260.5995,
        'z': -52.24094,
        'r': 45,
        'side': 1
    },
    {
        'x': 1092.098,
        'y': 258.1739,
        'z': -52.24094,
        'r': 45,
        'side': 0x2
    },
    {
        'x': 1091.443,
        'y': 257.5198,
        'z': -52.24094,
        'r': 45,
        'side': 1
    },
    {
        'x': 1101.915,
        'y': 264.1026,
        'z': -52.24094,
        'r': 45,
        'side': 0x2
    },
    {
        'x': 1101.261,
        'y': 263.4485,
        'z': -52.24094,
        'r': 45,
        'side': 1
    },
    {
        'x': 1098.451,
        'y': 260.6878,
        'z': -52.24094,
        'r': 45,
        'side': 0x2
    },
    {
        'x': 1097.797,
        'y': 260.0338,
        'z': -52.24094,
        'r': 45,
        'side': 1
    },
    {
        'x': 1097.072,
        'y': 259.309,
        'z': -52.24094,
        'r': 45,
        'side': 0x2
    },
    {
        'x': 1096.417,
        'y': 258.6552,
        'z': -52.24114,
        'r': 45,
        'side': 1
    },
    {
        'x': 1093.653,
        'y': 255.8405,
        'z': -52.24094,
        'r': 45,
        'side': 0x2
    },
    {
        'x': 1092.999,
        'y': 255.1864,
        'z': -52.24094,
        'r': 45,
        'side': 1
    }
];

const localPlayer = global.localplayer;
localPlayer.freezePosition(false);

let scaleFormRacing = new global.ScaleFormRacing('HORSE_RACING_WALL');

let isLoad = false;

const LoadTexture = () => {
	if (!mp.game.graphics.hasStreamedTextureDictLoaded('Prop_Screen_VW_InsideTrack')) {
		mp.game.graphics.requestStreamedTextureDict('Prop_Screen_VW_InsideTrack', false);
	}

	scaleFormRacing.callbackLoad().then(()=>{
		mp.game.invoke("0xE6A9F00D4240B519", scaleFormRacing._handle, true);
		horses.forEach((horse)=>{
			horsesRacing.setHour(horse.id, horse.name, horse.rate, horse.primaryColour, horse.secondaryColour, horse.horseColour, horse.maneColour);
		})
	}).catch(err=>{
		mp.game.graphics.notify(err);
	})
}



for (let i = 0; i < seats.length; i++) {
    let colshape = mp.colshapes.newSphere(seats[i].x, seats[i].y, seats[i].z, 1.2);
    colshape.slotHoursRacing = i;
}

global.horsesplaying = false;

class horsesRacing{
    constructor() {
        this.timeRacing = 15000.0;
        this.countdown = 5;
        this.renderEvent;
        this.camera = null;
        this.timer;
        this.bets = [];
        this.gamertags = [];
        this.inGame = true;
    }
    static showHourseScreen(gamertags, horse, bets) {
		try
		{
			this.gamertags = gamertags;
			this.bets = bets;
			for (let i = 0; i < gamertags.length; i++) {
				mp.game.graphics.pushScaleformMovieFunction(scaleFormRacing._handle, 'ADD_PLAYER');
				mp.game.invoke('0x77FE3402004CD1B0', gamertags[i]);
				mp.game.graphics.pushScaleformMovieFunctionParameterInt(parseInt(horse));
				mp.game.graphics.pushScaleformMovieFunctionParameterInt(parseInt(bets[i]));
				mp.game.graphics.popScaleformMovieFunctionVoid();
			}
			scaleFormRacing.callFunction("SHOW_SCREEN", 0);
			scaleFormRacing.callFunction("SET_DETAIL_HORSE", horse);
			scaleFormRacing.callFunction("SHOW_SCREEN", 1);
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/index", "showHourseScreen", e.toString());
		}
    }
    static clearAllPlayers(){
        scaleFormRacing.callFunction("CLEAR_ALL_PLAYERS");
    }
    static setCountdown(time){
        scaleFormRacing.callFunction("SET_COUNTDOWN", time);
        this.countdown = time;
    }
    static setHour(id, name, rate, primaryColour, secondaryColour, horseColour, maneColour ){
        scaleFormRacing.callFunction("SET_HORSE", id, name, rate, primaryColour, secondaryColour, horseColour, maneColour);
    }
    static playSound(sound){
		try
		{
			let soundId = mp.game.invoke('0x430386FE9BF80B45');
			mp.game.audio.stopSound(soundId);
			mp.game.audio.releaseSoundId(soundId);
			mp.game.audio.playSoundFromCoord(soundId, sound, 1093.907, 263.1436, -49.49115, 'dlc_vw_casino_inside_track_betting_main_event_sounds', false, 0, false);
			if (mp.game.audio.isAudioSceneActive('dlc_vw_casino_inside_track_live_race')) {
				mp.game.audio.stopAudioScene('dlc_vw_casino_inside_track_live_race');
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/index", "playSound", e.toString());
		}
    }
    static showHorseResults(gamertags, results){
		try
		{
			gamertags = JSON.parse(gamertags);
			results = JSON.parse(results);
			for (let i = 0; i < gamertags.length; i++) {
				scaleFormRacing.callFunction("SET_PLAYER_RESULT", gamertags[i], results[i]);
				mp.game.invoke('0x77FE3402004CD1B0', gamertags[i]);
				mp.game.graphics.pushScaleformMovieFunctionParameterInt(parseInt(results[i]));
				mp.game.graphics.popScaleformMovieFunctionVoid();
			}
			scaleFormRacing.callFunction("SHOW_SCREEN", 4);
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/index", "showHorseResults", e.toString());
		}
    }
    static startHorseRacing(seed = 1, firstHorse = 0, secondHorse = 1, thirdHorse = 2, fourthHorse = 3, fifthHorse = 4, sixthHorse = 6, sync = true) {
		try
		{
			this.inGame = true;
			scaleFormRacing.callFunction("START_RACE", 15000.0, seed, firstHorse, secondHorse, thirdHorse, fourthHorse, fifthHorse, sixthHorse, 1.0, sync)
			mp.game.graphics.popScaleformMovieFunctionVoid();
			this.playSound('race_loop');
			setTimeout((function () {
				this.playSound('race_finish');
			}).bind(this), 15000 - 1000);
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/index", "startHorseRacing", e.toString());
		}
    }
}

scaleFormRacing.callFunction("SHOW_SCREEN", 5);

let curentSlot;

gm.events.add({
    'playerEnterColshape': (cols)=>{
		try
		{
			if(cols.slotHoursRacing !== undefined && global.horsesplaying == false && curentSlot == undefined){
				curentSlot = cols.slotHoursRacing;

				mp.game.audio.playSound(-1, "BACK", "HUD_AMMO_SHOP_SOUNDSET", true, 0, true);
				mp.game.graphics.notify(translateText("~g~E~s~ сесть за стол"));
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/index", "playerEnterColshape", e.toString());
		}
    },
    'render': () => {
		try {
			
			if (!global.loggedin) return;
			if (!isLoad && Natives.GET_INTERIOR_FROM_ENTITY (global.localplayer.handle) == 275201) {
				isLoad = true;
				LoadTexture ();
			} else if (isLoad && Natives.GET_INTERIOR_FROM_ENTITY (global.localplayer.handle) == 275201 && scaleFormRacing && scaleFormRacing.isLoaded) {
				scaleFormRacing.renderTarget('casinoscreen_02','vw_vwint01_betting_screen', 0.5, 0.5, 1.001, 1.001);
			}
		}
		catch (e) 
		{
			if(new Date().getTime() - global.trycatchtime["horses/index"] < 60000) return;
			global.trycatchtime["horses/index"] = new Date().getTime();
			mp.events.callRemote("client_trycatch", "horses/index", "render", e.toString());
		}
    },
    "playerExitColshape": (cols)=>{
		try
		{
			if(cols.slotHoursRacing !== undefined){
				curentSlot = undefined;
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/index", "playerExitColshape", e.toString());
		}
    },
    "client.horse.SLOT": async (game) => {
		try
		{
			if (Natives.GET_INTERIOR_FROM_ENTITY (global.localplayer.handle) !== 275201)
				return;
			horsesRacing.inGame = game;
			global.horsesplaying = true;
			curentSlot = undefined;
			localPlayer.freezePosition(true);
			mp.gui.emmit(`window.router.setView("CasinoHorse");`);
			gm.discord(translateText("Делает ставку на лошадь"));
			global.menuOpen();

			const position = mp.game.object.getObjectOffsetFromCoords(1098.337, 258.476, -50.12178, 45.0, 0.0, -0.87, 1);  
			const pointAtCoord = mp.game.object.getObjectOffsetFromCoords(1098.337, 258.476, -50.12178, 45.0, 0.0, -0.87, 1);

			horsesRacing.camera = mp.cameras.new('default', new mp.Vector3(position.x, position.y, position.z), new mp.Vector3(0, 0, 45.0), 50);
			horsesRacing.camera.pointAtCoord(pointAtCoord.x, pointAtCoord.y, pointAtCoord.z);
			horsesRacing.camera.setActive(true);
			mp.game.cam.renderScriptCams(true, false, 2000, true, false);
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/index", "client.horse.SLOT", e.toString());
		}

    },
    "client.horse.START_TIMER": (time)=>{
		try
		{
			if (Natives.GET_INTERIOR_FROM_ENTITY (global.localplayer.handle) !== 275201)
				return;
			horsesRacing.setCountdown(time);
			horsesRacing.timer = setInterval(()=>{
				if(horsesRacing.countdown > 0) {
					horsesRacing.setCountdown(horsesRacing.countdown-1);
				} else {
					clearInterval(horsesRacing.timer);
				}
			}, 1000)
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/index", "client.horse.START_TIMER", e.toString());
		}
    },
    "client.horse.START_RACING": (seed = 1, firstHorse = 0, secondHorse = 1, thirdHorse = 2, fourthHorse = 3, fifthHorse = 4, sixthHorse = 5)=>{
		try
		{
			if (Natives.GET_INTERIOR_FROM_ENTITY (global.localplayer.handle) !== 275201)
				return;
			if (global.horsesplaying != false) 
				mp.gui.emmit(`window.events.callEvent("cef.horse.isBtn", 0)`);
			horsesRacing.startHorseRacing(seed, firstHorse, secondHorse, thirdHorse, fourthHorse, fifthHorse, sixthHorse);
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/index", "client.horse.START_RACING", e.toString());
		}
    },
    "client.horse.SHOW_RESULTS": (gamertags, results)=>{
		try
		{
			if (Natives.GET_INTERIOR_FROM_ENTITY (global.localplayer.handle) !== 275201)
				return;
			//else if (global.horsesplaying == false) return;
			if (global.horsesplaying != false) 
				mp.gui.emmit(`window.events.callEvent("cef.horse.isBtn", 1)`);
			horsesRacing.showHorseResults(gamertags, results);
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/index", "client.horse.SHOW_RESULTS", e.toString());
		}
    },
    "client.horse.SHOW_HORSE": (gamertags, horse, bets)=>{
		try
		{
			if (Natives.GET_INTERIOR_FROM_ENTITY (global.localplayer.handle) !== 275201)
				return;
			if (horsesRacing.inGame) return;
			horsesRacing.clearAllPlayers();
			horsesRacing.showHourseScreen(JSON.parse(gamertags), horse, JSON.parse(bets));
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/index", "client.horse.SHOW_HORSE", e.toString());
		}
    },
    "client.horse.GET_HORSE": (horse)=>{
		try
		{
			if (Natives.GET_INTERIOR_FROM_ENTITY (global.localplayer.handle) !== 275201)
				return;
			if (horsesRacing.inGame) return;
			//horsesRacing.clearAllPlayers();
			//horsesRacing.showHourseScreen(horsesRacing.gamertags, horse, horsesRacing.bets);
			
			scaleFormRacing.callFunction("SHOW_SCREEN", 0);
			scaleFormRacing.callFunction("SET_DETAIL_HORSE", horse);
			scaleFormRacing.callFunction("SHOW_SCREEN", 1);
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/index", "client.horse.GET_HORSE", e.toString());
		}
    },
    "client.horse.setBet": (horse, bet)=>{
		try
		{
			if (Natives.GET_INTERIOR_FROM_ENTITY (global.localplayer.handle) !== 275201)
				return;
			else if (global.horsesplaying == false) return;
			else if (horsesRacing.inGame) return;
			mp.events.callRemote('server.horse.setBet', horse, bet);
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/index", "client.horse.setBet", e.toString());
		}
    },
    "client.horse.COUNTDOWN_SCREEN": () => {
		try
		{
			if (Natives.GET_INTERIOR_FROM_ENTITY (global.localplayer.handle) !== 275201)
				return;
			horsesRacing.clearAllPlayers();
			horses.forEach((horse, i)=>{
				horsesRacing.setHour(horse.id, horse.name, 'x 3', horse.primaryColour, horse.secondaryColour, horse.horseColour, horse.maneColour);
			})
			scaleFormRacing.callFunction("SHOW_SCREEN", 0);
			horsesRacing.inGame = false;
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/index", "client.horse.COUNTDOWN_SCREEN", e.toString());
		}
    },
    "client.horse.TEARS_SLOT": () =>{
		try
		{
			if (Natives.GET_INTERIOR_FROM_ENTITY (global.localplayer.handle) !== 275201)
				return;
			if (global.horsesplaying == false) return;
			global.horsesplaying = false;
			localPlayer.freezePosition(false);
			global.menuClose();
			mp.gui.emmit(`window.router.setHud();`);
			if (horsesRacing.camera !== null) {
				horsesRacing.camera.destroy();
				horsesRacing.camera = null;
				mp.game.cam.renderScriptCams(false, false, 3000, true, true);
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/index", "client.horse.TEARS_SLOT", e.toString());
		}
    },
    "client.horse.exit": () => {
        global.binderFunctions.horseExit ();
    },
})

// key E
mp.keys.bind(0x45, !false, function () {
    if (global.localplayer.isDead() || mp.gui.cursor.visible) return false;
    if (curentSlot != undefined) mp.events.callRemote('server.horse.SIT_SLOT');
    else global.binderFunctions.horseExit ();
});

global.binderFunctions.horseExit = () => {
    if (global.horsesplaying == true) mp.events.callRemote('server.horse.TEARS_SLOT');
}