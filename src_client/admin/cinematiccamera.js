global.flymode = false;
var flycam = null;
var fov = 50.0;

var ticks_ctrl = 0.0;
var ticks_space = 0.0;
var ticks_left = 0.0;
var ticks_right = 0.0;
var ticks_up = 0.0;
var ticks_down = 0.0;
var ticks_turn_r = 0.0;
var ticks_turn_l = 0.0;
var ticks_a = 0.0;
var ticks_d = 0.0;
var ticks_w = 0.0;
var ticks_s = 0.0;

var movement_up = 3;
var movement_down = 3;
var movement_ctrl = 3;
var movement_space = 3;
var movement_a = 3;
var movement_d = 3;
var movement_w = 3;
var movement_s = 3;
var movement_left = 3;
var movement_right = 3;
var movement_turn_l = 3;
var movement_turn_r = 3;

var movement_speed = 20.0;
var mousesensetive_speed = 5.5;

global.toggleFlyCam = (toggle) =>
{
	try 
	{
		if(toggle)
		{
			var position = global.localplayer.position;// global.cameraManager.gameplayCam().getCoord();
			var rotation = global.cameraManager.gameplayCam().getRot(2);
			flycam = mp.cameras.new('default', new mp.Vector3(position.x, position.y, position.z), new mp.Vector3(rotation.x, rotation.y, rotation.z), fov);
			flycam.setActive(true);
			mp.game.cam.renderScriptCams(true, false, 0, true, false);
			global.flymode = true;
			mp.gui.cursor.visible = false;
		}
		else
		{
			global.flymode = false;
			flycam.destroy();
			mp.game.cam.renderScriptCams(false, false, 500, true, false);
			flycam = null;

			if(!mp.keys.isDown(global.Keys.VK_SPACE))
			{
				var position = global.localplayer.position;
				global.getSafeZCoords(position.x, position.y, position.z, (z) =>
				{
					global.localplayer.setCoordsNoOffset(position.x, position.y, z, false, false, false);
				});
			}        

			nativeInvoke("UNLOCK_MINIMAP_ANGLE");
			nativeInvoke("UNLOCK_MINIMAP_POSITION");
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "admin/cinematiccamera", "toggleFlyCam", e.toString());
	}
}

function Lerp(firstFloat, secondFloat, by)
{
    return (firstFloat + (secondFloat - firstFloat) * by);
}

gm.events.add("client.flycam", () => {
	try 
	{
		if (!global.loggedin) return;
		if (!global.flymode && !global.menuCheck()) global.toggleFlyCam(true);
		else global.toggleFlyCam(false);
		mp.events.callRemote('invisible', global.flymode);
		global.localplayer.setVisible(!global.flymode, false);
		global.localplayer.freezePosition(global.flymode);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "admin/cinematiccamera", "client.flycam", e.toString());
	}
});

gm.events.add("client.flycam.time", (value) => {
	try 
	{
		if (!global.loggedin) return;
		else if (global.flymode && flycam !== null && !mp.game.ui.isPauseMenuActive() && !mp.gui.cursor.visible) {
			movement_speed = value;
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "admin/cinematiccamera", "client.flycam.time", e.toString());
	}
});

gm.events.add("render", () => {
	if (global.flymode && flycam !== null && !mp.game.ui.isPauseMenuActive() && !mp.gui.cursor.visible)
	{
		let staticPosition = global.localplayer.position;
		let position = staticPosition;
		let rotation = flycam.getRot(2);
		let direction = flycam.getDirection();

		let xMagnitude = 0.0;
		let yMagnitude = 0.0;

		if (!mp.keys.isDown(global.Keys.VK_NUMPAD4) && movement_left == 1) movement_left = 2;
		if (!mp.keys.isDown(global.Keys.VK_NUMPAD6) && movement_right == 1) movement_right = 2;
		if (!mp.keys.isDown(global.Keys.VK_NUMPAD2) && movement_down == 1) movement_down = 2;
		if (!mp.keys.isDown(global.Keys.VK_NUMPAD8) && movement_up == 1) movement_up = 2;
		if (!mp.keys.isDown(global.Keys.VK_NUMPAD7) && movement_turn_l == 1) movement_turn_l = 2;
		if (!mp.keys.isDown(global.Keys.VK_NUMPAD9) && movement_turn_r == 1) movement_turn_r = 2;
		if (!mp.keys.isDown(global.Keys.VK_W) && movement_w == 1) movement_w = 2;
		if (!mp.keys.isDown(global.Keys.VK_S) && movement_s == 1) movement_s = 2;
		if (!mp.keys.isDown(global.Keys.VK_A) && movement_a == 1) movement_a = 2;
		if (!mp.keys.isDown(global.Keys.VK_D) && movement_d == 1) movement_d = 2;
		if (!mp.keys.isDown(global.Keys.VK_SPACE) && movement_space == 1) movement_space = 2;
		if (!mp.keys.isDown(global.Keys.VK_CONTROL) && movement_ctrl == 1) movement_ctrl = 2;

		if (mp.keys.isDown(global.Keys.VK_NUMPAD4) && movement_right == 3) movement_left = 1;
		if (mp.keys.isDown(global.Keys.VK_NUMPAD6) && movement_left == 3) movement_right = 1;
		if (mp.keys.isDown(global.Keys.VK_NUMPAD2) && movement_up == 3) movement_down = 1;
		if (mp.keys.isDown(global.Keys.VK_NUMPAD8) && movement_down == 3) movement_up = 1;
		if (mp.keys.isDown(global.Keys.VK_NUMPAD7) && movement_turn_r == 3) movement_turn_l = 1;
		if (mp.keys.isDown(global.Keys.VK_NUMPAD9) && movement_turn_l == 3) movement_turn_r = 1;
		if (mp.keys.isDown(global.Keys.VK_S) && movement_w == 3) movement_s = 1;
		if (mp.keys.isDown(global.Keys.VK_W) && movement_s == 3) movement_w = 1;
		if (mp.keys.isDown(global.Keys.VK_A) && movement_d == 3) movement_a = 1;
		if (mp.keys.isDown(global.Keys.VK_D) && movement_a == 3) movement_d = 1;
		if (mp.keys.isDown(global.Keys.VK_CONTROL) && movement_space == 3) movement_ctrl = 1;
		if (mp.keys.isDown(global.Keys.VK_SPACE) && movement_ctrl == 3) movement_space = 1;

		// Дополнительно на Q и E
		if (mp.keys.isDown(global.Keys.VK_E) && !mp.keys.isDown(global.Keys.VK_Q))
		{
			movement_speed += 0.5;
			if (movement_speed >= 150.0) movement_speed = 100.0;
		}
		if (mp.keys.isDown(global.Keys.VK_Q) && !mp.keys.isDown(global.Keys.VK_E))
		{
			movement_speed -= 0.5;
			if (movement_speed <= 0.0) movement_speed = 0.0;
		}

		if (mp.keys.isDown(global.Keys.VK_OEM_PLUS))
		{
			fov += 0.1;
			if (fov >= 130.0) fov = 130.0;
		}
		if (mp.keys.isDown(global.Keys.VK_OEM_MINUS))
		{
			fov -= 0.1;
			if (fov <= 0.0) fov = 0.0;
		}

		xMagnitude = mp.game.controls.getDisabledControlNormal(0, 1);
		yMagnitude = mp.game.controls.getDisabledControlNormal(0, 2);

		if (xMagnitude != 0)
		{
			rotation.z = rotation.z + (-xMagnitude) * mousesensetive_speed;
		}

		if (yMagnitude != 0)
		{
			rotation.x = rotation.x + (-yMagnitude) * mousesensetive_speed;
			if (rotation.x <= -89.0) rotation.x = -89.0;
			else if (rotation.x >= 89.0) rotation.x = 89.0;
		}

		let temp_movement_speed = movement_speed;

		if (mp.keys.isDown(global.Keys.VK_LBUTTON))
		{
			temp_movement_speed = 100.0;
		}
		if (mp.keys.isDown(global.Keys.VK_RBUTTON))
		{
			temp_movement_speed = 1.5;
		}

		if (movement_left == 1)
		{
			let staticRotation = flycam.getRot(2);
			ticks_left += 0.005 * temp_movement_speed;
			if (ticks_left >= 1)
			{
				ticks_left = 1;
			}
			rotation.z = Lerp(staticRotation.z, rotation.z + temp_movement_speed, ticks_left);
		}
		else if (movement_left == 2)
		{
			let staticRotation = flycam.getRot(2);
			ticks_left -= 0.01 * temp_movement_speed;
			if (ticks_left <= 0)
			{
				ticks_left = 0;
				movement_left = 3;
			}
			rotation.z = Lerp(staticRotation.z, rotation.z + temp_movement_speed, ticks_left);
		}

		if (movement_right == 1)
		{
			let staticRotation = flycam.getRot(2);
			ticks_right += 0.005 * temp_movement_speed;
			if (ticks_right >= 1)
			{
				ticks_right = 1;
			}
			rotation.z = Lerp(staticRotation.z, rotation.z - temp_movement_speed, ticks_right);
		}
		else if (movement_right == 2)
		{
			let staticRotation = flycam.getRot(2);
			ticks_right -= 0.01 * temp_movement_speed;
			if (ticks_right <= 0)
			{
				ticks_right = 0;
				movement_right = 3;
			}
			rotation.z = Lerp(staticRotation.z, rotation.z - temp_movement_speed, ticks_right);
		}

		if (movement_up == 1)
		{
			let staticRotation = flycam.getRot(2);
			ticks_up += 0.005 * temp_movement_speed;
			if (ticks_up >= 1)
			{
				ticks_up = 1;
			}
			rotation.x = Lerp(staticRotation.x, rotation.x + temp_movement_speed, ticks_up);
		}
		else if (movement_up == 2)
		{
			let staticRotation = flycam.getRot(2);
			ticks_up -= 0.01 * temp_movement_speed;
			if (ticks_up <= 0)
			{
				ticks_up = 0;
				movement_up = 3;
			}
			rotation.x = Lerp(staticRotation.x, rotation.x + temp_movement_speed, ticks_up);
		}

		if (movement_down == 1)
		{
			let staticRotation = flycam.getRot(2);
			ticks_down += 0.005 * temp_movement_speed;
			if (ticks_down >= 1)
			{
				ticks_down = 1;
			}
			rotation.x = Lerp(staticRotation.x, rotation.x - temp_movement_speed, ticks_down);
		}
		else if (movement_down == 2)
		{
			let staticRotation = flycam.getRot(2);
			ticks_down -= 0.01 * temp_movement_speed;
			if (ticks_down <= 0)
			{
				ticks_down = 0;
				movement_down = 3;
			}
			rotation.x = Lerp(staticRotation.x, rotation.x - temp_movement_speed, ticks_down);
		}

		if (movement_turn_r == 1)
		{
			let staticRotation = flycam.getRot(2);
			ticks_turn_r += 0.005 * temp_movement_speed;
			if (ticks_turn_r >= 1)
			{
				ticks_turn_r = 1;
			}
			rotation.y = Lerp(staticRotation.y, rotation.y + temp_movement_speed, ticks_turn_r);
		}
		else if (movement_turn_r == 2)
		{
			let staticRotation = flycam.getRot(2);
			ticks_turn_r -= 0.01 * temp_movement_speed;
			if (ticks_turn_r <= 0)
			{
				ticks_turn_r = 0;
				movement_turn_r = 3;
			}
			rotation.y = Lerp(staticRotation.y, rotation.y + temp_movement_speed, ticks_turn_r);
		}

		if (movement_turn_l == 1)
		{
			let staticRotation = flycam.getRot(2);
			ticks_turn_l += 0.005 * temp_movement_speed;
			if (ticks_turn_l >= 1)
			{
				ticks_turn_l = 1;
			}
			rotation.y = Lerp(staticRotation.y, rotation.y - temp_movement_speed, ticks_turn_l);
		}
		else if (movement_turn_l == 2)
		{
			let staticRotation = flycam.getRot(2);
			ticks_turn_l -= 0.01 * temp_movement_speed;
			if (ticks_turn_l <= 0)
			{
				ticks_turn_l = 0;
				movement_turn_l = 3;
			}
			rotation.y = Lerp(staticRotation.y, rotation.y - temp_movement_speed, ticks_turn_l);
		}

		if (movement_w == 1)
		{
			ticks_w += 0.005 * temp_movement_speed;
			if (ticks_w >= 1)
			{
				ticks_w = 1;
			}
			position.x = Lerp(position.x, position.x + (direction.x * (temp_movement_speed / 10)), ticks_w);
			position.y = Lerp(position.y, position.y + (direction.y * (temp_movement_speed / 10)), ticks_w);
			position.z = Lerp(position.z, position.z + (direction.z * (temp_movement_speed / 10)), ticks_w);
		}
		else if (movement_w == 2)
		{
			ticks_w -= 0.01 * temp_movement_speed;
			if (ticks_w <= 0)
			{
				ticks_w = 0;
				movement_w = 3;
			}
			position.x = Lerp(position.x, position.x + (direction.x * (temp_movement_speed / 10)), ticks_w);
			position.y = Lerp(position.y, position.y + (direction.y * (temp_movement_speed / 10)), ticks_w);
			position.z = Lerp(position.z, position.z + (direction.z * (temp_movement_speed / 10)), ticks_w);
		}

		if (movement_s == 1)
		{
			ticks_s += 0.005 * temp_movement_speed;
			if (ticks_s >= 1)
			{
				ticks_s = 1;
			}
			position.x = Lerp(position.x, position.x - (direction.x * (temp_movement_speed / 10)), ticks_s);
			position.y = Lerp(position.y, position.y - (direction.y * (temp_movement_speed / 10)), ticks_s);
			position.z = Lerp(position.z, position.z - (direction.z * (temp_movement_speed / 10)), ticks_s);
		}
		else if (movement_s == 2)
		{
			ticks_s -= 0.01 * temp_movement_speed;
			if (ticks_s <= 0)
			{
				ticks_s = 0;
				movement_s = 3;
			}
			position.x = Lerp(position.x, position.x - (direction.x * (temp_movement_speed / 10)), ticks_s);
			position.y = Lerp(position.y, position.y - (direction.y * (temp_movement_speed / 10)), ticks_s);
			position.z = Lerp(position.z, position.z - (direction.z * (temp_movement_speed / 10)), ticks_s);
		}

		if (movement_a == 1)
		{
			ticks_a += 0.005 * temp_movement_speed;
			if (ticks_a >= 1)
			{
				ticks_a = 1;
			}
			position.x = Lerp(position.x, position.x + (-direction.y * (temp_movement_speed / 10)), ticks_a);
			position.y = Lerp(position.y, position.y + (direction.x * (temp_movement_speed / 10)), ticks_a);
		}
		else if (movement_a == 2)
		{
			ticks_a -= 0.01 * temp_movement_speed;
			if (ticks_a <= 0)
			{
				ticks_a = 0;
				movement_a = 3;
			}
			position.x = Lerp(position.x, position.x + (-direction.y * (temp_movement_speed / 10)), ticks_a);
			position.y = Lerp(position.y, position.y + (direction.x * (temp_movement_speed / 10)), ticks_a);
		}

		if (movement_d == 1)
		{
			ticks_d += 0.005 * temp_movement_speed;
			if (ticks_d >= 1)
			{
				ticks_d = 1;
			}
			position.x = Lerp(position.x, position.x - (-direction.y * (temp_movement_speed / 10)), ticks_d);
			position.y = Lerp(position.y, position.y - (direction.x * (temp_movement_speed / 10)), ticks_d);
		}
		else if (movement_d == 2)
		{
			ticks_d -= 0.01 * temp_movement_speed;
			if (ticks_d <= 0)
			{
				ticks_d = 0;
				movement_d = 3;
			}
			position.x = Lerp(position.x, position.x - (-direction.y * (temp_movement_speed / 10)), ticks_d);
			position.y = Lerp(position.y, position.y - (direction.x * (temp_movement_speed / 10)), ticks_d);
		}

		if (movement_space == 1)
		{
			ticks_space += 0.005 * temp_movement_speed;
			if (ticks_space >= 1)
			{
				ticks_space = 1;
			}
			position.z = Lerp(position.z, position.z + (temp_movement_speed / 10), ticks_space);
		}
		else if (movement_space == 2)
		{
			ticks_space -= 0.005 * temp_movement_speed;
			if (ticks_space <= 0)
			{
				ticks_space = 0;
				movement_space = 3;
			}
			position.z = Lerp(position.z, position.z + (temp_movement_speed / 10), ticks_space);
		}

		if (movement_ctrl == 1)
		{
			ticks_ctrl += 0.005 * temp_movement_speed;
			if (ticks_ctrl >= 1)
			{
				ticks_ctrl = 1;
			}
			position.z = Lerp(position.z, position.z - (temp_movement_speed / 10), ticks_ctrl);
		}
		else if (movement_ctrl == 2)
		{
			ticks_ctrl -= 0.01 * temp_movement_speed;
			if (ticks_ctrl <= 0)
			{
				ticks_ctrl = 0;
				movement_ctrl = 3;
			}
			position.z = Lerp(position.z, position.z - (temp_movement_speed / 10), ticks_ctrl);
		}

		global.localplayer.setCoordsNoOffset(position.x, position.y, position.z, false, false, false);
		global.localplayer.setHeading(rotation.z);

		mp.game.invoke("0x1279E861A329E73F", position.x, position.y);
		mp.game.invoke("0x299FAEBB108AE05B", parseInt((rotation.z + 360.0) % 360.0));

		flycam.setCoord(position.x, position.y, position.z);
		flycam.setRot(rotation.x, rotation.y, rotation.z, 2);
		flycam.setFov(fov);


		//var interiorid = mp.game.interior.getInteriorAtCoords(position.x, position.y, position.z);
		//mp.game.interior.refreshInterior(interiorid);

		mp.game.controls.disableAllControlActions(0);
		mp.game.controls.enableControlAction(0, 199, true); // Enable pause menu (INPUT_FRONTEND_PAUSE)
		mp.game.controls.enableControlAction(0, 200, true); // Enable pause menu (INPUT_FRONTEND_PAUSE_ALTERNATE)
		mp.game.controls.enableControlAction(0, 20, true); // Enable zoom man key (INPUT_MULTIPLAYER_INFO)
	}
});