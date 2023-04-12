global.deathTimerOn = false;
let deathTimer = 0;


// Disable auto screen fade
mp.game.gameplay.disableAutomaticRespawn(true);
mp.game.gameplay.ignoreNextRestart(true);
mp.game.gameplay.setFadeOutAfterDeath(false);
mp.game.gameplay.setFadeInAfterDeathArrest(false);
mp.game.gameplay.setFadeInAfterLoad(false);

gm.events.add('DeathTimer', (time) => {
	try
	{
		if (typeof time === 'number' && Math.round(time) >= 1) {
			global.deathTimerOn = true;
			global.localplayer.setInvincible(true);
			deathTimer = new Date().getTime() + Math.round(time);			
		} else if (global.deathTimerOn) {
			mp.events.call("clearCarryng");
			global.deathTimerOn = false;
			global.localplayer.setInvincible(false);
			global.binderFunctions.c_globalEscape (true);
			mp.events.call("client.phone.close");
			//mp.events.call('hud.tip', "location", "ems");
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/death", "DeathTimer", e.toString());
	}
});

var blockcontrols = false;
var fullblockcontrols = false;

gm.events.add('blockMove', function (argument) {
	try
	{
		blockcontrols = argument;
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/death", "blockMove", e.toString());
	}
});

gm.events.add('fullblockMove', function (argument) {
	try
	{
		fullblockcontrols = argument;
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/death", "fullblockMove", e.toString());
	}
});

gm.events.add("playerDeath", async (player, reason, killer) =>  {
	try
	{
        if (!global.loggedin) return;
        else if (player !== global.localplayer) return;
		global.binderFunctions.c_globalEscape (true);
		mp.events.call("client.phone.close");
		if (global.localplayer.vehicle) {
			global.localplayer.clearTasks();
			global.localplayer.clearTasksImmediately();
		}
		if (!global.inAirsoftLobby || global.inAirsoftLobby == -1) {
			mp.game.audio.playSoundFrontend(-1, "Bed", "WastedSounds", true);
			mp.game.graphics.startScreenEffect("DeathFailMPIn", 0, true);
			mp.game.cam.setCamEffect(1);
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/death", "playerDeath", e.toString());
	}
});

gm.events.add("playerSpawn", () => {
	try
	{
        if (!global.loggedin) return;
		global.deathTimerOn = false;
		global.localplayer.setInvincible(false);
		mp.game.graphics.stopScreenEffect("DeathFailMPIn");
		mp.game.cam.setCamEffect(0);
		global.lastCheck = new Date().getTime();
		global.closeDialog();
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/death", "playerSpawn", e.toString());
	}
});

gm.events.add("render", () => {
	if (!global.loggedin) return;
	if (global.isDeath)
	{
		mp.game.controls.disableAllControlActions(2);
		mp.game.controls.enableControlAction(2, global.Inputs.LOOK_LR, true);
		mp.game.controls.enableControlAction(2, global.Inputs.LOOK_UD, true);
		mp.game.controls.enableControlAction(2, global.Inputs.LOOK_UP_ONLY, true);
		mp.game.controls.enableControlAction(2, global.Inputs.LOOK_DOWN_ONLY, true);
		mp.game.controls.enableControlAction(2, global.Inputs.LOOK_LEFT_ONLY, true);
		mp.game.controls.enableControlAction(2, global.Inputs.LOOK_RIGHT_ONLY, true);
	}
	else if (blockcontrols || fullblockcontrols)
	{
		mp.game.controls.disableAllControlActions(2);
		if(!fullblockcontrols)
		{
			mp.game.controls.enableControlAction(2, 30, true);
			mp.game.controls.enableControlAction(2, 31, true);
			mp.game.controls.enableControlAction(2, 32, true);
			mp.game.controls.enableControlAction(2, 1, true);
			mp.game.controls.enableControlAction(2, 2, true);
		}
	}
	if (global.deathTimerOn)
	{
		const secondsLeft = Math.trunc((deathTimer - new Date().getTime()) / 1000);
		if (secondsLeft >= 0)
		{
			mp.game.cam.doScreenFadeIn(500);
			gm.discord(translateText("Ждёт медиков..."));

			const minutes = Math.trunc(secondsLeft / 60);
			const seconds = secondsLeft % 60;

			mp.game.graphics.drawText(translateText("До попадания в больницу: {0}:{1}", global.formatIntZero(minutes, 2), global.formatIntZero(seconds, 2)), [0.5, 0.8], {
				font: 0,
				color: [255, 255, 255, 200],
				scale: [0.35, 0.35],
				outline: true
			});
		}
	}

	/*if (
		mp.game.controls.isControlPressed(0, 32) ||
		mp.game.controls.isControlPressed(0, 33) ||
		mp.game.controls.isControlPressed(0, 321) ||
		mp.game.controls.isControlPressed(0, 34) ||
		mp.game.controls.isControlPressed(0, 35) ||
		mp.game.controls.isControlPressed(0, 24) ||
		global.isDeath == true
	)
	{
		global.afkSecondsCount = 0;
		mp.events.call('updateAFKStatus_client', false);
	}
	else if (global.localplayer.isInAnyVehicle(false) && global.localplayer.vehicle != null && global.localplayer.vehicle.getSpeed() != 0)
	{
		global.afkSecondsCount = 0;
		mp.events.call('updateAFKStatus_client', false);
	}
	else if(global.spectating)
	{ // Чтобы не кикало администратора в режиме слежки
		//global.afkSecondsCount = 0; todo check
		//mp.events.call('updateAFKStatus_client', false);
	}*/
});