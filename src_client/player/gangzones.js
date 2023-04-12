const positions = [
    { 'position': { 'x': -200.8397, 'y': -1431.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': -100.8397, 'y': -1431.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': -0.8397, 'y': -1431.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 99.1603, 'y': -1431.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': -200.8397, 'y': -1531.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': -100.8397, 'y': -1531.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': -0.8397, 'y': -1531.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 99.1603, 'y': -1531.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 199.1603, 'y': -1531.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 299.1603, 'y': -1531.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 399.1603, 'y': -1531.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 499.1603, 'y': -1531.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': -200.8397, 'y': -1631.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': -100.8397, 'y': -1631.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': -0.8397, 'y': -1631.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 99.1603, 'y': -1631.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 199.1603, 'y': -1631.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 299.1603, 'y': -1631.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 399.1603, 'y': -1631.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 499.1603, 'y': -1631.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 599.1603, 'y': -1631.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': -100.8397, 'y': -1731.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': -0.8397, 'y': -1731.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 99.1603, 'y': -1731.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 199.1603, 'y': -1731.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 299.1603, 'y': -1731.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 399.1603, 'y': -1731.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 499.1603, 'y': -1731.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 599.1603, 'y': -1731.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': -0.8397, 'y': -1831.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 99.1603, 'y': -1831.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 199.1603, 'y': -1831.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 299.1603, 'y': -1831.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 399.1603, 'y': -1831.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 499.1603, 'y': -1831.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 599.1603, 'y': -1831.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 99.1603, 'y': -1931.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 199.1603, 'y': -1931.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 299.1603, 'y': -1931.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 399.1603, 'y': -1931.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 499.1603, 'y': -1931.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 599.1603, 'y': -1931.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 199.1603, 'y': -2031.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 299.1603, 'y': -2031.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 399.1603, 'y': -2031.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 499.1603, 'y': -2031.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 299.1603, 'y': -2131.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 399.1603, 'y': -2131.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 768.8984, 'y': -2401.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 868.8984, 'y': -2401.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 968.8984, 'y': -2401.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1068.898, 'y': -2401.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 768.8984, 'y': -2301.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 868.8984, 'y': -2301.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 968.8984, 'y': -2301.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1068.898, 'y': -2301.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 768.8984, 'y': -2201.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 868.8984, 'y': -2201.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 968.8984, 'y': -2201.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1068.898, 'y': -2201.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 768.8984, 'y': -2101.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 868.8984, 'y': -2101.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 968.8984, 'y': -2101.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1068.898, 'y': -2101.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 768.8984, 'y': -2001.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 868.8984, 'y': -2001.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 968.8984, 'y': -2001.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1068.898, 'y': -2001.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 768.8984, 'y': -1901.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 868.8984, 'y': -1901.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 968.8984, 'y': -1901.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1068.898, 'y': -1901.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 768.8984, 'y': -1801.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 868.8984, 'y': -1801.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 968.8984, 'y': -1801.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1168.898, 'y': -1801.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1268.898, 'y': -1801.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 768.8984, 'y': -1701.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 868.8984, 'y': -1701.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 968.8984, 'y': -1701.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1168.898, 'y': -1701.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1268.898, 'y': -1701.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1368.898, 'y': -1701.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 768.8984, 'y': -1601.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 868.8984, 'y': -1601.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1168.898, 'y': -1601.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1268.898, 'y': -1601.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1368.898, 'y': -1601.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1268.898, 'y': -1501.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1368.898, 'y': -1501.556, 'z': 28.17772 }, 'color': 10 },
]

let blips = [];
gm.events.add('loadCaptureBlips', function (json) {
	try
	{

		json = JSON.parse(json);
		for (let i = 0; i < json.length; i++) {
			positions[i].color = json[i];
		}

		positions.forEach(element => {
			const blip = mp.blips.new(5, element.position,
			{
				name: "",
				radius: 50,
				color: element.color,
				alpha: 100,
				drawDistance: 100,
				shortRange: false,
				rotation: 0,
				dimension: -1,
			});

			blips.push(blip);
		});
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/gangzones", "loadCaptureBlips", e.toString());
	}
});

gm.events.add('setZoneColor', function (id, color) {
	if (!global.loggedin)
		return;

	if (!blips[id])
		return;

	nativeInvoke ("SET_BLIP_COLOUR", blips[id].handle, color);

});

gm.events.add('setZoneFlash', function (id, state, color) {
	if (!global.loggedin)
		return;

	if (!blips[id])
		return;

	nativeInvoke ("SET_BLIP_FLASH_TIMER", blips[id].handle, 1000);
	nativeInvoke ("SET_BLIP_FLASHES", blips[id].handle, state);
});

gm.events.add("render", () => {
	if (!global.loggedin) return;
	if (blips.length !== 0)
	{
		blips.forEach(blip => {
			nativeInvoke ("SET_BLIP_ROTATION", blip.handle, 0);
		});
	}
});

gm.events.add('quitcmd', function () {
	try
	{
		setTimeout(function() { mp.events.callRemote('kickclient') }, 500);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/gangzones", "quitcmd", e.toString());
	}
});



//

/*

const isInsideArea = (self) => {

	///let range = Math.sqrt(((self.range * 1.2) * (self.range * 1.2)) + (((self.range * 1.2) / 2) * ((self.range * 1.2) / 2)));
	let range = Math.sqrt(((self.range) * (self.range)) + (((self.range)) * ((self.range))));

	let degrees = (self.rotation + 45) * (Math.PI / 180);

	let top_right = {

		x: self.position.x + range * Math.cos(degrees),

		y: self.position.y + range * Math.sin(degrees)

	}

	degrees = (self.rotation + 135) * (Math.PI / 180);

	let top_left = {

		x: self.position.x + range * Math.cos(degrees),

		y: self.position.y + range * Math.sin(degrees)

	}

	degrees = (self.rotation + 225) * (Math.PI / 180);

	let bottom_left = {

		x: self.position.x + range * Math.cos(degrees),

		y: self.position.y + range * Math.sin(degrees)

	}

	degrees = (self.rotation + 315) * (Math.PI / 180);

	let bottom_right = {

		x: self.position.x + range * Math.cos(degrees),

		y: self.position.y + range * Math.sin(degrees)

	}

	return {
		top_right: top_right,
		top_left: top_left,
		bottom_right: bottom_right,
		bottom_left: bottom_left,
	}

}

const data = [
	[254, 254, 254]
	[224, 50, 50]
	[113, 203, 113]
	[224, 50, 50]
	[93, 182, 229]
	[93, 182, 229]
	[224, 50, 50]
	[254, 254, 254]
	[224, 50, 50]
	[93, 182, 229]
	[238, 198, 78]
	[224, 50, 50]
	[93, 182, 229]
	[194, 80, 80]
	[194, 80, 80]
	[254, 122, 195]
	[245, 157, 121]
	[177, 143, 131]
	[141, 206, 167]
	[112, 168, 174]
	[211, 209, 231]
	[143, 126, 152]
	[106, 196, 191]
	[213, 195, 152]
	[234, 142, 80]
	[151, 202, 233]
	[178, 98, 135]
	[143, 141, 121]
	[166, 117, 94]
	[175, 168, 168]
	[231, 141, 154]
	[187, 214, 91]
	[12, 123, 86]
	[122, 195, 254]
	[171, 60, 230]
	[205, 168, 12]
	[69, 97, 171]
	[41, 165, 184]
	[184, 155, 123]
	[200, 224, 254]
	[240, 240, 150]
	[237, 140, 161]
	[249, 138, 138]
	[251, 238, 165]
	[254, 254, 254]
	[44, 109, 184]
	[154, 154, 154]
	[76, 76, 76]
	[242, 157, 157]
	[108, 183, 214]
	[175, 237, 174]
	[255, 167, 95]
	[241, 241, 241]
	[236, 240, 41]
	[255, 154, 24]
	[246, 68, 165]
	[224, 58, 58]
	[93, 182, 229]
	[138, 109, 227]
	[255, 139, 92]
	[65, 108, 65]
	[179, 221, 243]
	[58, 100, 121]
	[160, 160, 160]
	[132, 114, 50]
	[101, 185, 231]
	[75, 65, 117]
	[225, 59, 59]
	[240, 203, 88]
	[205, 63, 152]
	[207, 207, 207]
	[39, 106, 159]
	[216, 123, 27]
	[142, 131, 147]
	[240, 203, 87]
	[224, 50, 50]
	[93, 182, 229]
	[101, 185, 231]
	[224, 50, 50]
	[101, 185, 231]
	[224, 50, 50]
	[121, 205, 121]
	[224, 50, 50]
	[93, 182, 229]
	[239, 202, 87]
	[239, 202, 87]
	[61, 61, 61]
	[239, 202, 87]
	[101, 185, 231]
	[224, 50, 50]
	[120, 35, 35]
	[101, 185, 231]
	[58, 100, 121]
	[224, 50, 50]
	[101, 185, 231]
	[242, 164, 12]
	[164, 204, 170]
	[168, 84, 242]
	[101, 185, 231]
	[61, 61, 61]
];

const renderSquare = (pos, color) => {
	const self = isInsideArea({
		position: pos,
		rotation: 0,
		range: 50,
	});
	//let z = mp.game.gameplay.getGroundZFor3dCoord(mp.players.local.position.x, mp.players.local.position.y, mp.players.local.position.z, 0, false);
	//z = z - 20;
	let z = 35;
	let r = 255;
	let g = 0;
	let b = 0;
	let alpha = 100

	if (self && self["top_right"] != null && self["bottom_right"] != null && self["bottom_left"] != null && self["top_left"] != null) {

		for (let heading = 0; heading <= 271; heading += 90) {
			mp.game.graphics.drawPoly(self["bottom_right"].x, self["bottom_right"].y, 50, self["top_right"].x, self["top_right"].y, 50, self["top_left"].x, self["top_left"].y, 50, data[color][0], data[color][1], data[color][2], alpha);
			mp.game.graphics.drawPoly(self["top_left"].x, self["top_left"].y, 50, self["bottom_left"].x, self["bottom_left"].y, 50, self["bottom_right"].x, self["bottom_right"].y, 50, data[color][0], data[color][1], data[color][2], alpha);
		}
	}
}

gm.events.add("render", () => {
	if (!global.loggedin) return;
	positions.forEach((element) => {
		renderSquare(element.position, element.color);
	})
});
*/