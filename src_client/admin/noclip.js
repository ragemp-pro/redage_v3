// credits to ragempdev

const controlsIds = {
    F11: 0x7A,
    W: 32,
    S: 33,
    A: 34,
    D: 35, 
    Space: 321,
    LCtrl: 326,
    LMB: 24,
	RMB: 25
};

global.fly = {
	flying: false,
	f: 2.0,
	w: 2.0,
	h: 2.0,
	point_distance: 1000,
	time: new Date().getTime()
};

let direction = null;
let coords = null;

mp.events.addDataHandler("INVISIBLE", (entity, value, oldValue) => {
	try
	{
		if (entity && mp.players.exists(entity) && entity.type === 'player' && entity.handle !== 0) {
			value = Boolean (value);

			if (value) {
				entity.setVisible(false, false);
			} else {				
				entity.setVisible(true, false);
			}
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "synchronization/state", "INVISIBLE", e.toString());
	}
});

global.fly.flying = false;

gm.events.add('SetINVISIBLE', (toggled) => {
	global.fly.flying = toggled;

	SetINVISIBLE ();
});

const SetINVISIBLE = () => {    
	const controls = mp.game.controls;
	direction = global.cameraManager.gameplayCam().getDirection();
	coords = global.cameraManager.gameplayCam().getCoord();
	global.fly.time = new Date().getTime();
	
	if(!global.admingm) 
		global.localplayer.setInvincible(global.fly.flying);

	global.localplayer.freezePosition(global.fly.flying);
	global.localplayer.setVisible(!global.fly.flying, false);

	if (!global.fly.flying && !controls.isControlPressed(0, controlsIds.Space)) {
		global.setPlayerToGround ();
	}

	if (!global.fly.flying && global.localplayer['REDNAME']) {
		setTimeout(() => {
			global.localplayer.setAlpha(100);
		}, 50)			
	}
}

global.setPlayerToGround = (entity = global.localplayer) => {
	if (entity) {
		let position = entity.position;
		position.z = mp.game.gameplay.getGroundZFor3dCoord(position.x, position.y, position.z, 0.0, false);
		entity.setCoordsNoOffset(position.x, position.y, position.z, false, false, false);
	}
}

global.binderFunctions.noclip = () => {
	try 
	{
		if (!global.loggedin || global.chatActive || global.isAdmin !== true) return;
		global.fly.flying = !global.fly.flying;
		mp.events.callRemote('invisible', global.fly.flying);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "admin/noclip", "binderFunctions.noclip", e.toString());
	}
};

gm.events.add("render", () => {
	if (fly.flying && new Date().getTime() - global.fly.time > 150) {
		const controls = mp.game.controls;
		const fly = global.fly;
		direction = global.cameraManager.gameplayCam().getDirection();
		coords = global.cameraManager.gameplayCam().getCoord();

		const position = global.localplayer.position;
		let speed;
		if(controls.isControlPressed(0, controlsIds.LMB)) speed = 1.0
		else if(controls.isControlPressed(0, controlsIds.RMB)) speed = 0.02
		else speed = 0.2
		if (controls.isControlPressed(0, controlsIds.W)) {
			if (fly.f < 8.0) fly.f *= 1.025;
			position.x += direction.x * fly.f * speed;
			position.y += direction.y * fly.f * speed;
			position.z += direction.z * fly.f * speed;
		} else if (controls.isControlPressed(0, controlsIds.S)) {
			if (fly.f < 8.0) fly.f *= 1.025;
			position.x -= direction.x * fly.f * speed;
			position.y -= direction.y * fly.f * speed;
			position.z -= direction.z * fly.f * speed;
		} else fly.f = 2.0;
		if (controls.isControlPressed(0, controlsIds.A)) {
			if (fly.l < 8.0) fly.l *= 1.025;
			position.x += (-direction.y) * fly.l * speed;
			position.y += direction.x * fly.l * speed;
		} else if (controls.isControlPressed(0, controlsIds.D)) {
			if (fly.l < 8.0) fly.l *= 1.05;
			position.x -= (-direction.y) * fly.l * speed;
			position.y -= direction.x * fly.l * speed;
		} else
			fly.l = 2.0;

		if (controls.isControlPressed(0, controlsIds.Space)) {
			if (fly.h < 8.0) fly.h *= 1.025;
			position.z += fly.h * speed;
		} else if (controls.isControlPressed(0, controlsIds.LCtrl)) {
			if (fly.h < 8.0) fly.h *= 1.05;
			position.z -= fly.h * speed;
		} else
			fly.h = 2.0;

		global.localplayer.setCoordsNoOffset(position.x, position.y, position.z, false, false, false);
	}
});