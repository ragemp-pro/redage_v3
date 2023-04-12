
let isGreenZone = false;

gm.events.add('greenzone', () => {
	if (!isGreenZone) {
		isGreenZone = true;
		mp.events.add("render", GreenZoneRender);
	} else {
		isGreenZone = false;
		mp.events.remove("render", GreenZoneRender);

	}
});

const GreenZoneRender = () => {
	try {
		if (!global.loggedin) return;

		mp.polygons.pool.forEach((polygon) => {
			const { vertices, height, lineColorRGBA } = polygon;

			vertices.forEach((vertex, index) => {
				const nextVertex = index === vertices.length - 1 ? vertices[0] : vertices[index + 1];

				// Deepness lower line
				mp.game.graphics.drawLine(vertex.x, vertex.y, vertex.z, nextVertex.x, nextVertex.y, nextVertex.z, ...lineColorRGBA);

				// Current vertex height line
				mp.game.graphics.drawLine(vertex.x, vertex.y, vertex.z, vertex.x, vertex.y, vertex.z + height, ...lineColorRGBA);

				// Next vertex height line
				mp.game.graphics.drawLine(nextVertex.x, nextVertex.y, nextVertex.z, nextVertex.x, nextVertex.y, nextVertex.z + height, ...lineColorRGBA);

				// Deepness higher line
				mp.game.graphics.drawLine(vertex.x, vertex.y, vertex.z + height, nextVertex.x, nextVertex.y, nextVertex.z + height, ...lineColorRGBA);
			});

			const vertex = vertices [0];
			const idName = polygon.isGreenZone ? `GreenZoneId: ${polygon.id}` : `VoiceZoneId: ${polygon.id}`;

			mp.game.graphics.drawText(idName, [vertex.x, vertex.y, vertex.z + height], {
				scale: [0.3, 0.3],
				outline: true,
				color: lineColorRGBA,
				font: 4
			});
		});
	} catch (e) {
		if (new Date().getTime() - global.trycatchtime["polygons/render"] < 60000) return;
		global.trycatchtime["polygons/render"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "polygons/render", "render", e.toString());
	}
}


let clTest = false;
gm.events.add('cltest1', () => {
	if (!clTest) {
		clTest = true;
		gm.events.add("render", Render);
	} else {

		clTest = false;
		mp.events.remove("render", Render);
	}
});


const Render = () => {
	try {
		if (!global.loggedin) return;

		mp.polygons.pool.forEach((polygon) => {
			const { vertices, height, lineColorRGBA } = polygon;

			const vertex = vertices [0];

			var vPos = mp.game.graphics.world3dToScreen2d(new mp.Vector3(vertex.x, vertex.y, 120));
			global.DrawSprite("redage_textures_001", "ticket", [255, 255, 255, 255], vPos.x, vPos.y);
		});
	} catch (e) {
		if (new Date().getTime() - global.trycatchtime["polygons/render"] < 60000) return;
		global.trycatchtime["polygons/render"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "polygons/render", "render", e.toString());
	}
}

/*
const DrawSprite = (textureDict, textureName, colour, x, y) => {
	const drawData = drawSpritesSize [textureName];
	if (drawData)
		mp.game.graphics.drawSprite(textureDict, textureName, x, y, drawData.width, drawData.height, drawData.heading, colour[0], colour[1], colour[2], colour[3]);
}*/