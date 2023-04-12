global.binderFunctions.markerteleport = () => {
	try 
	{
		if (!global.loggedin || global.chatActive  || global.isAdmin !== true) return;

		let coords = global.GetWaypointCoords();
		if(coords !== null)
		{
			global.getSafeZCoords(coords.x, coords.y, 0, (z) =>
			{
				global.localplayer.setCoordsNoOffset(coords.x, coords.y, z, false, false, false);
			});
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "admin/markerteleport", "binderFunctions.markerteleport", e.toString());
	}
    
};