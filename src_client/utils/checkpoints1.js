let markersData = {};

gm.events.add('createCheckpoint', function (id, type, position, scale, dimension, r, g, b, dir) {
	if (markersData[id])
		delete markersData [id];

	if (dir)
	{
		/*markersData[id] = {
			type: type,
			position: position,
			scale: scale,
			color: [r, g, b, 20],
			direction: dir,
			dimension: dimension
		}*/

		mp.game.graphics.createCheckpoint(type,
			position.x, position.y, position.z,
			dir.x, dir.y, dir.z,
			scale,
			r, g, b, 200, false);
	}
	else
	{
		markersData[id] = {
			type: type,
			position: position,
			scale: scale,
			color: [r, g, b, 255],
			dimension: dimension
		}
	}
});


gm.events.add("render", () => {
	if (!global.loggedin)
		return;

	if (!markersData)
		return;

	const markers = Object.values(markersData);

	if (!markers.length)
		return;

	const dimension = global.localplayer.dimension;

	markers.forEach((marker) => {
		if (marker.dimension < 0 || marker.dimension === dimension) {
			if (marker.direction) {
				/*mp.game.graphics.createCheckpoint(marker.type,
					marker.position.x, marker.position.y, marker.position.z,
					marker.direction.x, marker.direction.y, marker.direction.z,
					marker.scale,
					marker.color[0], marker.color[1], marker.color[2], marker.color[3], false);*/
			} else {
				mp.game.graphics.drawMarker(marker.type, marker.position.x, marker.position.y, marker.position.z,
					0, 0, 0,
					0, 0, 0,
					marker.scale, marker.scale, marker.scale,
					marker.color[0], marker.color[1], marker.color[2], marker.color[3],
					false, false, 2, false, null, null, false);
			}


		}
	})
});

gm.events.add('deleteCheckpoint', function (id) {
	if (markersData[id])
		delete markersData [id];
});

gm.events.add('createWaypoint', async (x, y) => {
	mp.game.ui.setNewWaypoint(x, y);
});

let workBlip = null;
gm.events.add('createWorkBlip', function (position, blipColor = 49) {
	if (workBlip != null)
		workBlip.destroy();

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
});

gm.events.add('deleteWorkBlip', function () {
	if (workBlip != null)
	{
		workBlip.destroy();
		workBlip = null;
	}
});

let garageBlip = null;
gm.events.add('createGarageBlip', function (position) {
	if (garageBlip != null)
		garageBlip.destroy();

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
});

gm.events.add('deleteGarageBlip', function () {
	if (garageBlip != null)
		garageBlip.destroy();

	garageBlip = null;
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