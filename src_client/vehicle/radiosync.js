var rtimer = null;
var nowplaying;

function vehradio(entity) {
	try 
	{
		if (entity && mp.vehicles.exists(entity)) {
			if (global.localplayer.vehicle == entity) {
				let vehrad = entity.getVariable('vehradio');
				nowplaying = mp.game.invoke(global.getNative("GET_PLAYER_RADIO_STATION_INDEX"));
				if (entity.getPedInSeat(-1) == global.localplayer.handle) {
					if (vehrad != nowplaying) mp.events.callRemote('VehStream_RadioChange', nowplaying);
				} else {
					if (vehrad == 255) mp.game.audio.setRadioToStationName("OFF");
					else {
						if (vehrad != nowplaying) {
							mp.game.invoke(global.getNative("SET_FRONTEND_RADIO_ACTIVE"), true);
							mp.game.invoke(global.getNative("SET_RADIO_TO_STATION_INDEX"), vehrad);
						}

					}
				}
			}
		} else {
			if (rtimer != null) {
				clearInterval(rtimer);
				rtimer = null;
			}
		}
	}
	catch (e) 
	{
		if(new Date().getTime() - global.trycatchtime["vehicle/radiosync"] < 5000) return;
		global.trycatchtime["vehicle/radiosync"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "vehicle/radiosync", "vehradio", e.toString());
	}
};

gm.events.add("playerEnterVehicle", (entity, seat) => {
	try
	{
		if (entity != null) {
			if (rtimer != null) clearInterval(rtimer);
			rtimer = setInterval(function () { vehradio(entity); }, 1000);
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/radiosync", "playerEnterVehicle", e.toString());
	}
});

gm.events.add("playerLeaveVehicle", (entity) => {
	try
	{
		if (rtimer != null) {
			clearInterval(rtimer);
			rtimer = null;
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/radiosync", "playerLeaveVehicle", e.toString());
	}
});