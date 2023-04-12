let spcam = null;
const spmenu = new global.NativeMenu("Spectate", "Spectate Menu", new Point(30, 500)); // Мы не выключаем чат, чтобы видеть переписку тех, за кем следим, поэтому двигаем саму табличку вниз
spmenu.Close();
spmenu.AddItem(new global.UIMenuListItem("Spectate", translateText("Переключение игрока по ID"),  new global.ItemsCollection(["Previous", "Next"])));
spmenu.AddItem(new global.UIMenuItem("Refresh", translateText("Обновить слежение за текущим игроком")));
spmenu.AddItem(new global.UIMenuItem("Unspectate", translateText("Выключить режим наблюдателя")));

var lastflystate = false;

function DestroySPCam() 
{
	try 
	{
		if(spcam !== null) spcam.destroy();
		mp.game.cam.renderScriptCams(false, false, 3000, true, true);
		spcam = null;
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "admin/spectate", "DestroySPCam", e.toString());
	}
}

gm.events.add("render", () => {
	if(!spectating || spcam == null) return;
	var rotation = spcam.getRot(2);
	var xMagnitude = 0.0;
	var yMagnitude = 0.0;
	xMagnitude = mp.game.controls.getDisabledControlNormal(0, 1);
	yMagnitude = mp.game.controls.getDisabledControlNormal(0, 2);
	if (xMagnitude != 0) rotation.z = rotation.z + (-xMagnitude) * 5.5;
	if (yMagnitude != 0)
	{
		rotation.x = rotation.x + (-yMagnitude) * 5.5;
		if (rotation.x <= -89.0) rotation.x = -89.0;
		else if (rotation.x >= 89.0) rotation.x = 89.0;
	}
	spcam.setRot(rotation.x, rotation.y, rotation.z, 2);
});

gm.events.add("spmode", (target, toggle) => {
	try 
	{
		if(toggle) 
		{
			if (target && mp.players.exists(target)) 
			{
				lastflystate = global.flymode;
				if(global.flymode == true) global.toggleFlyCam(false);
				global.localplayer.attachTo(target.handle,  -1, 0, 0, -6, 0, 0, 0, true, false, false, false, 0, false);
				global.sptarget = target;
				global.spectating = true;
				
				let rotation = global.cameraManager.gameplayCam().getRot(2);
				if(spcam == null) spcam = mp.cameras.new('default', new mp.Vector3(target.position.x, target.position.y, target.position.z), new mp.Vector3(rotation.x, rotation.y, rotation.z), 50);
				spcam.setActive(true);
				spcam.attachToPedBone(target.handle, 31086, -2, -6, 5, false);
				mp.game.cam.renderScriptCams(true, false, 0, true, false);
		
				spmenu.Open();
			} else {
				DestroySPCam();
				mp.events.callRemote("UnSpectate");
			}
		} 
		else 
		{
			DestroySPCam();
			global.sptarget = null;
			global.localplayer.detach(true, true);
			global.spectating = false;
			spmenu.Close();
		}
		global.localplayer.freezePosition(toggle);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "admin/spectate", "spmode", e.toString());
	}
});

spmenu.ItemSelect.on(item => 
{
	try 
	{
		if (item instanceof global.UIMenuListItem) {
			if(item.Text == "Spectate") {
				if(item.Index == 0) mp.events.callRemote("SpectateSelect", false);
				else mp.events.callRemote("SpectateSelect", true);
			}
		} else if (item instanceof global.UIMenuItem) {
			if(item.Text == "Refresh") mp.events.call("spmode", sptarget, true);
			else if(item.Text == "Unspectate") mp.events.callRemote("UnSpectate");
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "admin/spectate", "spmenu.ItemSelect", e.toString());
	}
});

spmenu.MenuClose.on(() => {
	try 
	{
		if(spectating) spmenu.Open();
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "admin/spectate", "spmenu.MenuClose", e.toString());
	}
});
