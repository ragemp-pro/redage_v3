
let SATM = false;
let timesatm = new Date().getTime();

global.binderFunctions.SitWalkSyle = () => {
	if (!global.loggedin || global.chatActive || global.menuCheck() || global.isDeath == true || global.isDemorgan == true || global.attachedtotrunk || global.localplayer.isInAnyVehicle(false)) return;
	else if (global.localplayer.isFalling() || global.localplayer.isCuffed() || global.localplayer.isFatallyInjured() || global.localplayer.isShooting() || global.localplayer.isSwimming() || global.localplayer.isClimbing()) return;
	if (new Date().getTime() - timesatm > 2000) {
		timesatm = new Date().getTime();
		SATM = !SATM;
		mp.events.callRemote('SitWalkSyle', SATM);
	}
};


gm.events.add('setClientRotation', function (player, rots) {
	try
	{
		player = mp.players.atRemoteId(player);
		if (mp.players.exists(player)) player.setRotation(0, 0, rots, 2, true);
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/sync", "setClientRotation", e.toString());
    }
});

global.followedfor = null;

gm.events.add('setFollow', function (toggle, entity) {
	try
	{
		if (toggle) {
			if (entity && mp.players.exists(entity)) {
				localplayer.taskFollowToOffsetOf(entity.handle, 0, 0, 0, 1, -1, 1, true);
				global.followedfor = entity;
			}
		}
		else {
			localplayer.clearTasks();
			global.followedfor = null;
		}
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/sync", "setFollow", e.toString());
    }
});

var checktimer = null;
gm.events.add("playerStartEnterVehicle", (entity, enginestate) => {
	try 
	{
		if(global.followedfor != null) {
			if (mp.players.exists(global.followedfor)) {

				localplayer.taskFollowToOffsetOf(global.followedfor.handle, 0, 0, 0, 1, -1, 1, true);
				
				if(checktimer == null) {
					checktimer = setTimeout(() => {
						checktimer = null;
						if(global.followedfor != null) {
							if (mp.players.exists(global.followedfor)) localplayer.taskFollowToOffsetOf(global.followedfor.handle, 0, 0, 0, 1, -1, 1, true);
						}
					}, 1000);
				}
			}
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "playerStartEnterVehicle", e.toString());
	}
});