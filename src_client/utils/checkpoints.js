var markers = [];

gm.events.add('createCheckpoint', function (uid, type, position, scale, dimension, r, g, b, dir) {
	try
	{
		if (typeof markers[uid] != "undefined") 
		{
			markers[uid].destroy();
			markers[uid] = undefined;
		}
		if (dir != undefined) 
		{
			markers[uid] = mp.checkpoints.new(type, position, scale,
				{
					direction: dir,
					color: [r, g, b, 200],
					visible: true,
					dimension: dimension
				});
		}
		else 
		{
			markers[uid] = mp.markers.new(type, position, scale,
				{
					visible: true,
					dimension: dimension,
					color: [r, g, b, 255]
				});

			//nativeInvoke("DRAW_MARKER")


			//void DRAW_MARKER(type, position.x, position.y, position.z,
			// 0.0, 0.0, 0.0,
			// 0.0, 0.0, 0.0,
			// scale, scale, scale, r, g, b, 255, false, false, 2, BOOL rotate, char* textureDict, char* textureName, BOOL drawOnEnts);
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "utils/checkpoints", "createCheckpoint", e.toString());
	}
});

gm.events.add('deleteCheckpoint', function (uid) {
	try
	{
		if (typeof markers[uid] == "undefined") return;
		markers[uid].destroy();
		markers[uid] = undefined;
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "utils/checkpoints", "deleteCheckpoint", e.toString());
	}
});

gm.events.add('createWaypoint', async (x, y) => {
	try
	{
		//if (Natives.IS_WAYPOINT_ACTIVE ())
		//	Natives._DELETE_WAYPOINT ();

		mp.game.ui.setNewWaypoint(x, y);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "utils/checkpoints", "createWaypoint", e.toString());
	}
});

var workBlip = null;
gm.events.add('createWorkBlip', function (position, blipColor = 49) {
	try
	{
		if (workBlip != null) workBlip.destroy();
		workBlip = mp.blips.new(0, position,
			{
				name: translateText("Чекпоинт"),
				scale: 1,
				color: blipColor,
				alpha: 255,
				drawDistance: 100,
				shortRange: false,
				rotation: 0,
				dimension: 0,
			});
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "utils/checkpoints", "createWorkBlip", e.toString());
	}
}); 
gm.events.add('deleteWorkBlip', function () {
	try
	{
		if (workBlip != null) 
		{
			workBlip.destroy();
			workBlip = null;
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "utils/checkpoints", "deleteWorkBlip", e.toString());
	}
});

var garageBlip = null;
gm.events.add('createGarageBlip', function (position) {
	try
	{
		if (garageBlip != null) garageBlip.destroy();
		garageBlip = mp.blips.new(473, position,
		{
			name: translateText("Гараж"),
			scale: 1,
			color: 45,
			alpha: 255,
			drawDistance: 100,
			shortRange: true,
			rotation: 0,
			dimension: 0,
		});
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "utils/checkpoints", "createGarageBlip", e.toString());
	}
});

gm.events.add('deleteGarageBlip', function () {
	try
	{
		if (garageBlip != null) garageBlip.destroy();
		garageBlip = null;
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "utils/checkpoints", "deleteGarageBlip", e.toString());
	}
});

gm.events.add('changeBlipColor', function (blip, color) {
	try
	{
		if (blip == null) return;
        blip.setColour(color);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "utils/checkpoints", "changeBlipColor", e.toString());
	}
});

gm.events.add('changeBlipAlpha', function (blip, alpha) {
	try
	{
		if (blip == null) return;
        blip.setAlpha(alpha);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "utils/checkpoints", "changeBlipAlpha", e.toString());
	}
});

let blipsData = {}

//petData.blip.setCoords(ped.position);

gm.events.add('createBlip', function (id, name, blipId, position, scale = 1, color = 49, alpha = 255, displayID = undefined, radius = false) {
	mp.events.call('deleteBlip', id);


	let blipData = {
		name: name,
		color: color,
		alpha: alpha,
		drawDistance: 100,
		shortRange: false,
		rotation: 0,
		dimension: 0,
	}

	if (scale)
		blipData.scale = scale;

	if (radius)
		blipData.radius = radius;

	blipsData [id] = mp.blips.new(blipId, position, blipData);

	if (displayID != undefined)
		blipsData [id].setDisplay(displayID);
});


gm.events.add('deleteBlip', function (id) {
	if (blipsData [id]) {
		blipsData [id].destroy();
		delete blipsData [id];
	}
});