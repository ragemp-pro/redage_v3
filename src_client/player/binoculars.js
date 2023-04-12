global.binocularsEnabled = false;

let isLoad = false;
let camFov = 37.5;
let binocularsCamera;
let binocularsScaleForm;
gm.events.add("binoculars.start", async () => {
	try
	{
        if (isLoad || global.binocularsEnabled) return;
        else if (global.localplayer.vehicle) return;
        isLoad = true;
        await global.requestScaleformMovie('BINOCULARS');
        binocularsScaleForm = new global.ScaleFormRacing('BINOCULARS');
        isLoad = false;
        global.menuOpened = true;
        mp.gui.emmit(`window.router.close()`);
        Natives.SET_TIMECYCLE_MODIFIER ("default");
        Natives.SET_TIMECYCLE_MODIFIER_STRENGTH(0.3);
        const camRotZ = global.cameraManager.gameplayCam().getRot(2).z;
        binocularsCamera = mp.cameras.new("DEFAULT_SCRIPTED_FLY_CAMERA", global.localplayer.position, new mp.Vector3(0, 0, 0), camFov);
        binocularsCamera.attachTo(global.localplayer.handle, 0.1, 0.35, 0.75, true);
        binocularsCamera.setRot(0, 0, camRotZ, 2);
        binocularsCamera.setFov(camFov);
        global.localplayer.setHeading(camRotZ);
        mp.game.cam.renderScriptCams(true, false, 0, true, false);
        binocularsScaleForm.callFunction("SET_CAM_LOGO", 0);
        global.binocularsEnabled = true;
        gm.discord(translateText("Смотрит в бинокль"));
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/binoculars", "start", e.toString());
    }
});

gm.events.add("binoculars.stop", () => {
	try
	{
        if (!global.binocularsEnabled) return;
        global.binocularsEnabled = false;
        camFov = 37.5;
        Natives.CLEAR_TIMECYCLE_MODIFIER ();
        mp.game.cam.renderScriptCams(false, false, 0, false, false);
        if (binocularsScaleForm) {
            binocularsScaleForm.dispose ();
            binocularsScaleForm = void 0;
        }
        binocularsCamera.destroy();
        mp.game.graphics.setNightvision(false);
        mp.game.graphics.setSeethrough(false);    
        //mp.attachments.removeLocal("binoculars");
        //global.localplayer.taskPlayAnim("oddjobs@hunter", "binoculars_outro", 8.0, 1, -1, 50, 0.0, false, false, false);
        //nativeInvoke ("CLEAR_PED_SECONDARY_TASK", global.localplayer.handle);
        mp.gui.emmit(`window.router.setHud()`);
        global.menuClose ();
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/binoculars", "stop", e.toString());
    }
})

gm.events.add("playerDeath", (player, reason, killer) =>  {
	try
	{
        if (!global.loggedin) return;
        else if (player !== global.localplayer) return;
        else if (!global.binocularsEnabled)
            return;
		mp.events.callRemote("TakeWeapon", 0);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/binoculars", "playerDeath", e.toString());
	}
});

gm.events.add("render", () => {	
    try
	{
        if (!global.loggedin) return;
        else if (!global.binocularsEnabled) return;
        else if (global.cuffed || global.isDemorgan == true) {
            mp.events.callRemote("TakeWeapon", 0);
            return;
        }
        else if (global.localplayer.vehicle) {
            global.binocularsEnabled = false;
            mp.events.callRemote("TakeWeapon", 0);
            return;
        }
        mp.game.controls.disableAllControlActions(0);

        mp.game.ui.hideHudComponentThisFrame(1);
        mp.game.ui.hideHudComponentThisFrame(2);
        mp.game.ui.hideHudComponentThisFrame(3);
        mp.game.ui.hideHudComponentThisFrame(4);
        mp.game.ui.hideHudComponentThisFrame(6);
        mp.game.ui.hideHudComponentThisFrame(7);
        mp.game.ui.hideHudComponentThisFrame(8);
        mp.game.ui.hideHudComponentThisFrame(9);
        mp.game.ui.hideHudComponentThisFrame(13);
        mp.game.ui.hideHudComponentThisFrame(11);
        mp.game.ui.hideHudComponentThisFrame(12);
        mp.game.ui.hideHudComponentThisFrame(15);
        mp.game.ui.hideHudComponentThisFrame(18);
        mp.game.ui.hideHudComponentThisFrame(19);

        distanceBin (binocularsCamera, (1 / 65) * (camFov - 5));
        setFov (binocularsCamera);
        binocularsScaleForm.render2D(0.5, 0.5, 1, 1);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/binoculars", "render", e.toString());
    }
});

const distanceBin = (cam, fov) => {
	try
	{
        let
            xMagnitude = mp.game.controls.getDisabledControlNormal(0, 1),
            yMagnitude = mp.game.controls.getDisabledControlNormal(0, 2),
            rot = cam.getRot(2);
        if (0 != xMagnitude || 0 != yMagnitude) {
            const 
                heading = rot.z + -1 * xMagnitude * 8 * (fov + (0.1)),
                clamp = global.clamp(rot.x + -1 * yMagnitude * 8 * (fov + (1439.1 - 1439)), -89.5, 89.5);
            global.localplayer.setHeading(heading);
            cam.setRot(clamp, 0, heading, 2);
            global.cameraManager.gameplayCam().setRot(clamp, 0, heading, 2);
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/binoculars", "distanceBin", e.toString());
    }
}


const setFov = (cam) => {
	try
	{
        if (global.localplayer.vehicle) {
            if (mp.game.controls.isDisabledControlJustPressed(0, 17))
                camFov = Math.max(camFov - 10, 5);
            if (mp.game.controls.isDisabledControlJustPressed(0, 16))
                camFov = Math.min(camFov + 10, 70);
            const rot = cam.getFov();
            if (Math.abs(camFov - rot) < 0.1) 
                camFov = rot;
            cam.setFov(rot + (camFov - rot) * 0.05);
        } else {
            if (mp.game.controls.isDisabledControlJustPressed(0, 241))
                camFov = Math.max(camFov - 10, 5);
            if (mp.game.controls.isDisabledControlJustPressed(0, 242))
                camFov = Math.min(camFov + 10, 70);
            const rot = cam.getFov();
            if (Math.abs(camFov - rot) < 0.1)
                camFov = rot;
            cam.setFov(rot + (camFov - rot) * 0.05);
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/binoculars", "setFov", e.toString());
    }
};