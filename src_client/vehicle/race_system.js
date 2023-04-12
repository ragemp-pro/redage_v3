const race_system = {
    started: false,
	start_seconds: 0,
	
    marker: undefined,
    blip: undefined,
    colshape: undefined,

};

gm.events.add('client.streetrace.position', async () => {
	try
	{
		Natives.IS_WAYPOINT_ACTIVE ();
		mp.events.call('notify', 3, 9, translateText("Поставьте метку на карте!"), 3000);
		getBlipPosition((position) => {
			mp.events.callRemote('server.streetrace.position', position.x, position.y, position.z);
		});
		
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/race_system", "sendRaceOffer_client", e.toString());
	}
});



const getBlipPosition = (callback) => {
	try
	{
		let intervalId = setInterval(() => 
		{
			let returnPosition = global.GetWaypointCoords ();
			if(returnPosition !== null) {
				const position = global.localplayer.position;
				const dist = mp.game.system.vdist(position.x, position.y, position.z, returnPosition.x, returnPosition.y, returnPosition.z);
				if (dist > 50.0) {	
					clearInterval (intervalId);
					callback (returnPosition);
				} else {		
					mp.events.call('notify', 3, 9, translateText("Вы не молжете поставить так близко к себе!"), 3000);
					clearInterval (intervalId);
				}
			}
		}, 250);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/race_system", "getBlipPosition", e.toString());
		return undefined;
	}
}

gm.events.add('client.streetrace.start', (sr_X, sr_Y, sr_Z, isCreated) => {
	try
	{
		//mp.game.invoke('0xD8E694757BCEA8E9');
		race_system.started = true;

		//race_system.marker = mp.markers.new(1, new mp.Vector3(sr_X, sr_Y, sr_Z - 1.25), 10, { visible: true, color: [255, 0, 0, 180] });
		/*race_system.marker = mp.checkpoints.new(4, new mp.Vector3(sr_X, sr_Y, sr_Z), 5, {
			direction: new mp.Vector3(0, 0, 0),
			color: [
				165,
				0,
				0,
				255
			],
			visible: true,
			dimension: 100
		});*/
		
		
		
		
		race_system.blip = mp.blips.new(1, new mp.Vector3(sr_X, sr_Y), { alpha: 255, color: 1, name: translateText("Финиш") });
		race_system.blip.setRoute(true);
		race_system.blip.setRouteColour(1);
		race_system.colshape = mp.colshapes.newCircle(sr_X, sr_Y, 10, global.localplayer.dimension);

		mp.events.call('freeze', true);

		race_system.start_seconds = 5;
		
		mp.gui.emmit(`window.updateGameTime (${race_system.start_seconds}, '${translateText("Гонка")}', '${translateText("Цель: Доберитесь до финиша")}');`);

		gm.discord('Участвует в уличной гонке');

		let start_interval = setInterval(() => {
			race_system.start_seconds -= 1;
			mp.gui.emmit(`window.updateGameTime (${race_system.start_seconds}, '${translateText("Гонка")}', '${translateText("Цель: Доберитесь до финиша")}');`);

			if (race_system.start_seconds <= 0) {
				clearInterval(start_interval);

				race_system.start_seconds = 0;

				mp.events.call('freeze', false);
			}
		}, 1000);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/race_system", "acceptRaceOffer_client", e.toString());
	}
});


gm.events.add('playerEnterColshape', (shape) => {
	try
	{
		if (shape === race_system.colshape && race_system.started) {
			mp.events.callRemote('server.streetrace.finished');
			mp.events.call('client.streetrace.clear');
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/race_system", "playerEnterColshape", e.toString());
	}
});

gm.events.add('client.streetrace.clear', () => {
	try
	{
		if (race_system.started) {

			race_system.started = false;

			clearPlayerInfo();
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/race_system", "clearRaceInfo", e.toString());
	}
});

function clearPlayerInfo() {
	try
	{
		if (race_system.marker !== undefined) {
			race_system.marker.destroy();
			race_system.marker = undefined;
		}
		
		if (race_system.blip !== undefined) {
			race_system.blip.setRoute(false);
			race_system.blip.destroy();
			race_system.blip = undefined;
		}

		if (race_system.colshape !== undefined) {
			race_system.colshape.destroy();
			race_system.colshape = undefined;
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/race_system", "clearPlayerInfo", e.toString());
	}
}