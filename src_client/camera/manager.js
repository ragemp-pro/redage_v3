let CamerasManagerInfo = {
    gameplayCamera: null,
    activeCamera: null,
    interpCamera: null,
    interpActive: false,
    _events: new Map(),
    cameras: new Map([])
};

gm.events.add("render", () => {
	if (CamerasManagerInfo.interpCamera && cameraManager.doesExist(CamerasManagerInfo.interpCamera) && CamerasManagerInfo.activeCamera && !CamerasManagerInfo.activeCamera.isInterpolating()) {

		cameraManager.fireEvent('stopInterp', CamerasManagerInfo.activeCamera);

		//CamerasManagerInfo.interpCamera.setActive(false);
		//CamerasManagerInfo.interpCamera.destroy();
		//CamerasManagerInfo.interpCamera = null;
	}
});

const cameraSerialize = (camera) => {
	try 
	{
		camera.setActiveCamera = (toggle) => {
			cameraManager.setActiveCamera(camera, toggle);
		};

		camera.setActiveCameraWithInterp = (position, rotation, duration, easeLocation, easeRotation) => {
			cameraManager.setActiveCameraWithInterp(camera, position, rotation, duration, easeLocation, easeRotation);
		};
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "camera/manager", "cameraSerialize", e.toString());
	}
};

global.cameraManager = new class {

    on(eventName, eventFunction) {
		try 
		{
			if (CamerasManagerInfo._events.has(eventName)) {
				const event = CamerasManagerInfo._events.get(eventName);

				if (!event.has(eventFunction)) {
					event.add(eventFunction);
				}
			} else {
				CamerasManagerInfo._events.set(eventName, new Set([eventFunction]));
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "camera/manager", "global.cameraManager.on", e.toString());
		}
    }

    fireEvent(eventName, ...args) {
		try 
		{
			if (CamerasManagerInfo._events.has(eventName)) {
				const event = CamerasManagerInfo._events.get(eventName);

				event.forEach(eventFunction => {
					eventFunction(...args);
				});
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "camera/manager", "global.cameraManager.fireEvent", e.toString());
		}
    }

    getCamera(name, isGet = false) {  
		let camera = CamerasManagerInfo.cameras.get(name);	
		try 
		{
			if (!isGet && !this.hasCamera (name)) {
				global.cameraInfo.toggled = true;
				camera = this.createCamera (name);
			} else if (!isGet && this.hasCamera (name) && typeof camera.setActiveCamera !== 'function') {
				global.cameraInfo.toggled = true;
				cameraSerialize(camera);
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "camera/manager", "global.cameraManager.getCamera", e.toString());
		}
        return camera;
    }

    setCamera(name, camera) {
        CamerasManagerInfo.cameras.set(name, camera);
    }

    hasCamera(name) {
        return CamerasManagerInfo.cameras.has(name);
    }

    deleteCamera(name, ease = false, easeTime = 0) {
		try 
		{
			if (this.hasCamera (name)) {
				const camera = this.getCamera (name);
				this.setActiveCamera (camera, false, ease, easeTime);
				this.destroyCamera (camera);
				CamerasManagerInfo.cameras.delete (name);
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "camera/manager", "global.cameraManager.deleteCamera", e.toString());
		}
    }
    
    destroyCamera(camera) {
		try 
		{
			global.speedCamera = undefined;
			if (this.doesExist(camera)) {
				if (camera === this.activeCamera()) {
					CamerasManagerInfo.activeCamera.setActive(false);
					CamerasManagerInfo.activeCamera = null;
				}
				camera.destroy();
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "camera/manager", "global.cameraManager.destroyCamera", e.toString());
		}
    }

    stopCamera(ease = false, easeTime = 0) {
		try 
		{
			if (this.activeCamera() !== null) {
				global.cameraInfo.activeMovementCamera = false;
				this.setActiveCamera (this.activeCamera(), false, ease, easeTime);
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "camera/manager", "global.cameraManager.stopCamera", e.toString());
		}
    }

    createCamera(name, position = new mp.Vector3(0, 0, 100), rotation = new mp.Vector3(), fov = 50) {      
        let camera;
		try 
		{
			global.cameraInfo.toggled = true;
			if (!this.hasCamera (name)) {
				camera = mp.cameras.new('default', position, rotation, fov);
				cameraSerialize(camera);
				CamerasManagerInfo.cameras.set(name, camera);
			} else {
				camera = CamerasManagerInfo.cameras.get(name);
				if (typeof camera.setActiveCamera !== 'function') {
					cameraSerialize(camera);
				}           
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "camera/manager", "global.cameraManager.createCamera", e.toString());
		}
        return camera;
    }

    setActiveCamera(activeCamera, toggle, ease = false, easeTime = 0) {
		try
		{
			if (!toggle) {
				if (this.doesExist(CamerasManagerInfo.activeCamera)) {
					CamerasManagerInfo.activeCamera = null;
					activeCamera.setActive(false);
					mp.game.cam.renderScriptCams(false, ease, easeTime, ease, ease);
				}

				if (this.doesExist(this.activeCamera())) {
					CamerasManagerInfo.interpCamera.setActive(false);
					CamerasManagerInfo.interpCamera.destroy();
					CamerasManagerInfo.interpCamera = null;
				}
			} else {
				if (this.doesExist(this.activeCamera())) {
					CamerasManagerInfo.activeCamera.setActive(false);
				}
				CamerasManagerInfo.activeCamera = activeCamera;
				activeCamera.setActive(true);
				mp.game.cam.renderScriptCams(true, ease, easeTime, false, false);
				global.cameraInfo.toggled = true;
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "camera/manager", "global.cameraManager.setActiveCamera", e.toString());
		}
    }
    

    _setActiveCamera(activeCamera) {
		try
		{
			if (this.doesExist(this.activeCamera())) {
				CamerasManagerInfo.activeCamera.setActive(false);
			}
			CamerasManagerInfo.activeCamera = activeCamera;
			activeCamera.setActive(true);
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "camera/manager", "global.cameraManager._setActiveCamera", e.toString());
		}
    }

    setActiveCameraWithInterp(activeCamera, position, rotation, duration, easeLocation, easeRotation) {
		try 
		{
			if (this.doesExist(this.activeCamera())) {
				CamerasManagerInfo.activeCamera.setActive(false);
			}

			if (this.doesExist(CamerasManagerInfo.interpCamera)) {

				cameraManager.fireEvent('stopInterp', CamerasManagerInfo.interpCamera);

				CamerasManagerInfo.interpCamera.setActive(false);
				CamerasManagerInfo.interpCamera.destroy();
				CamerasManagerInfo.interpCamera = null;
			}
			const interpCamera = mp.cameras.new('default', activeCamera.getCoord(), activeCamera.getRot(2), activeCamera.getFov());
			activeCamera.setCoord(position.x, position.y, position.z);
			activeCamera.setRot(rotation.x, rotation.y, rotation.z, 2);
			activeCamera.stopPointing();

			CamerasManagerInfo.activeCamera = activeCamera;
			CamerasManagerInfo.interpCamera = interpCamera;
			activeCamera.setActiveWithInterp(interpCamera.handle, duration, easeLocation, easeRotation);
			mp.game.cam.renderScriptCams(true, false, 0, false, false);

			cameraManager.fireEvent('startInterp', CamerasManagerInfo.interpCamera);
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "camera/manager", "global.cameraManager.setActiveCameraWithInterp", e.toString());
		}
    }

    doesExist(camera) {
        return mp.cameras.exists(camera) && camera.doesExist();
    }

    activeCamera() {
        return CamerasManagerInfo.activeCamera;
    }

    gameplayCam() {
        if (!CamerasManagerInfo.gameplayCamera) {
            CamerasManagerInfo.gameplayCamera = mp.cameras.new("gameplay");
        }
        return CamerasManagerInfo.gameplayCamera;
    }

    
	normilizeHeading(value) {
        if (value > 360) return value - 360;
        else if (value < 0) return value + 360;
		return value;
	}
}