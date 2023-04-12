

global.cameraInfo = {
	activeMovementCamera: false,
    overlayToggled: true,
	polarAngleDeg: 0,
	azimuthAngleDeg: 0,
	azimutMax: 0,
    azimutMin: 0,
    lookZ: 0,
    radius: 0,
    radiusMax: 0,
    radiusMin: 0,
    toggled: false
};

global.cameraPosition = {
	poistionPoint: new mp.Vector3(-816.306, -182.914, 37.8927)
};

let useBone = "hat";
global.updateCameraToBone = (bone, entity = null, isBone = false, speed = 500) => {
	try
	{
		if (!isBone && useBone === bone) return;

		if (!entity) 
			entity = global.localplayer;

		useBone = bone;
		if (bone == "hat") calculateCameraParamsAndStartInterp(entity, 12844, new mp.Vector3(), 80, 160, speed);
		else if (bone == "top") calculateCameraParamsAndStartInterp(entity, 11816, new mp.Vector3(-0.13, 0.13, 0), 80, 160, speed);
		else if (bone == "legs") calculateCameraParamsAndStartInterp(entity, 11816, new mp.Vector3(0.5, 0.05, 0), 80, 120, speed);
		else if (bone == "shoes") calculateCameraParamsAndStartInterp(entity, 65245, new mp.Vector3(-0.168, 0, 0.1), 100, 130, speed);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "camera/other", "global.updateCameraToBone", e.toString());
	}
}

const calculateCameraParamsAndStartInterp = (entity, boneId, boneOffset, azimutMin, azimutMax, speed) => {
	try 
	{
		const camera = global.cameraManager.getCamera ('createCharacter');

		let coords = entity.getBoneCoords(boneId, boneOffset.x, boneOffset.y, boneOffset.z);

		const oldPoistionPoint = cameraPosition.poistionPoint;
		
		cameraPosition.poistionPoint = coords;

		coords = new mp.Vector3(
			coords.x,
			coords.y,
			coords.z + cameraInfo.lookZ
		);

		cameraInfo.azimutMin = azimutMin + fixRotation;

		cameraInfo.azimutMax = azimutMax + fixRotation;

		cameraInfo.azimuthAngleDeg = clamp(cameraInfo.azimuthAngleDeg, cameraInfo.azimutMin, cameraInfo.azimutMax);

		const polarAngleRad = cameraInfo.polarAngleDeg * Math.PI / 180.0;

		const azimuthAngleRad = cameraInfo.azimuthAngleDeg * Math.PI / 180.0;

		const nextCamLocation = new mp.Vector3(
			coords.x + cameraInfo.radius * (Math.sin(azimuthAngleRad) * Math.cos(polarAngleRad)),
			coords.y - cameraInfo.radius * (Math.sin(azimuthAngleRad) * Math.sin(polarAngleRad)),
			coords.z - cameraInfo.radius * Math.cos(azimuthAngleRad)

		);
		//camera.pointAtCoord(coords.x, coords.y, coords.z);
		
		global.cameraManager.setActiveCameraWithInterp(camera, nextCamLocation, camera.getRot(2), speed, 0, 0);

		startInterpCameraPointing(oldPoistionPoint, coords, speed);

		cameraInfo.activeMovementCamera = false;
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "camera/other", "calculateCameraParamsAndStartInterp", e.toString());
	}
}


global.clamp = (value, min, max) => {
    return Math.max( min, Math.min( max, value ) );
}

global.roundNumber = (number, exponent = 2) => {
    const pow = Math.pow(10, exponent);
    return Math.round(number * pow) / pow;
}

global.midVector = (pos1, pos2) => new mp.Vector3((pos1.x + pos2.x) / 2, (pos1.y + pos2.y) / 2, (pos1.z + pos2.z) / 2);

global.cameraManager.on('stopInterp', (camera) => {
	try 
	{
		if (camera === global.cameraManager.getCamera('createCharacter', true)) {
			cameraInfo.activeMovementCamera = true;
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "camera/other", "global.cameraManager.on", e.toString());
	}
});


gm.events.add('client.camera.toggled', (toggled) => {
    global.cameraInfo.toggled = toggled;
});

gm.events.add("render", () => {
	if (!global.cameraManager.activeCamera()) return;
	else if (!global.cameraInfo.toggled) return;

	if (cameraInfo.activeMovementCamera) {
		if (cameraInfo.overlayToggled) {
			mp.game.controls.enableControlAction(0, 241, true);

			if (mp.game.controls.isControlPressed(0, 241)) {
				cameraInfo.radius = cameraInfo.radius + 0.2;
				cameraInfo.radius = clamp(cameraInfo.radius, global.cameraInfo.radiusMin, global.cameraInfo.radiusMax);
			}

			mp.game.controls.enableControlAction(0, 242, true);

			if (mp.game.controls.isControlPressed(0, 242)) {
				cameraInfo.radius = cameraInfo.radius - 0.2;
				cameraInfo.radius = clamp(cameraInfo.radius, global.cameraInfo.radiusMin, global.cameraInfo.radiusMax);
			}
		}

		const coords = new mp.Vector3(
			cameraPosition.poistionPoint.x,
			cameraPosition.poistionPoint.y,
			cameraPosition.poistionPoint.z + cameraInfo.lookZ
		);

		mp.game.controls.enableControlAction(0, 238, true);
		if (cameraInfo.heightToggled && mp.game.controls.isControlPressed(0, 238)) {
			const yMagnitude = mp.game.controls.getDisabledControlNormal(0, 2);

			//const radiusPercent = (-cameraInfo.radius) * 100 / 1;
			cameraInfo.lookZ = clamp(cameraInfo.lookZ + yMagnitude * 0.7, -0.35, 0.6);
		}

		mp.game.controls.enableControlAction(0, 237, true);
		if (mp.game.controls.isControlPressed(0, 237) || cameraInfo.init) {
			cameraInfo.init = false;
			const xMagnitude = mp.game.controls.getDisabledControlNormal(0, 1);

			const yMagnitude = mp.game.controls.getDisabledControlNormal(0, 2);

			cameraInfo.polarAngleDeg = cameraInfo.polarAngleDeg + xMagnitude * 10;

			if (cameraInfo.polarAngleDeg >= 360) {
				cameraInfo.polarAngleDeg = 0;
			}

			cameraInfo.azimuthAngleDeg = cameraInfo.azimuthAngleDeg + yMagnitude * 10;

			if (cameraInfo.azimuthAngleDeg >= cameraInfo.azimutMax) {
				cameraInfo.azimuthAngleDeg = cameraInfo.azimutMax;
			}

			if (cameraInfo.azimuthAngleDeg <= cameraInfo.azimutMin) {
				cameraInfo.azimuthAngleDeg = cameraInfo.azimutMin;
			}
			//mp.events.call('notify', 4, 9, "azimuthAngleDeg - " + cameraInfo.azimuthAngleDeg, 3000);
		}

		const polarAngleRad = cameraInfo.polarAngleDeg * Math.PI / 180.0;
		const azimuthAngleRad = cameraInfo.azimuthAngleDeg * Math.PI / 180.0;

		const nextCamLocation = new mp.Vector3(
			coords.x + cameraInfo.radius * (Math.sin(azimuthAngleRad) * Math.cos(polarAngleRad)),
			coords.y - cameraInfo.radius * (Math.sin(azimuthAngleRad) * Math.sin(polarAngleRad)),
			coords.z - cameraInfo.radius * Math.cos(azimuthAngleRad)
		);
		const camera = global.cameraManager.activeCamera ();
		//const result = mp.raycasting.testPointToPoint(coords, nextCamLocation, global.localplayer, null);

		//if (typeof result !== 'undefined') {
		//    mp.events.call('notify', 4, 9, "nextCamLocation - 1", 3000);
		//    camera.setCoord(result.position.x + result.surfaceNormal.x * 0.2, result.position.y + result.surfaceNormal.y * 0.2, result.position.z + result.surfaceNormal.z * 0.2);
		//} else {
		//    mp.events.call('notify', 4, 9, "nextCamLocation - 2", 3000);
		//    camera.setCoord(nextCamLocation.x, nextCamLocation.y, nextCamLocation.z);
		//}
		camera.setCoord(nextCamLocation.x, nextCamLocation.y, nextCamLocation.z);
		camera.pointAtCoord(coords.x, coords.y, coords.z);
	}
});


require('./bezier-easing.js');
const startInterpCameraPointing = (from, to, duration) => {

	try
	{
		const _info = {
			percentage: 0
		};
		
		const startTime = Date.now();
		
		const camera = global.cameraManager.getCamera ('createCharacter');

		const bezier = global.BezierEasing(0.42, 0.13, 0.06, 0.89);
		
		let intervalId = null
		const eventHandler = () => {

			_info.percentage = (Date.now() - startTime) / duration;

			if (_info.percentage > 1) {
				clearInterval(intervalId)
				_info.percentage = 1;
			}
			
			const coords = lerpVector3(from, to, bezier(_info.percentage));
			
			camera.pointAtCoord(coords.x, coords.y, coords.z);
		};

		intervalId = setInterval(eventHandler, 0);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "camera/other", "startInterpCameraPointing", e.toString());
	}
};

function lerpVector3 (v1, v2, alpha) {
    return new mp.Vector3(
        v1.x + ( v2.x - v1.x ) * alpha,
        v1.y + ( v2.y - v1.y ) * alpha,
        v1.z + ( v2.z - v1.z ) * alpha
    )
}

global.FadeScreen = (state, duration) => {
    mp.gui.emmit(`window.FadeScreen(${state}, ${duration})`);
}

global.updateBoneToPos = (isBone) => {
	switch (isBone) {
		case 'tops':
		case 'undershirts':
			global.updateCameraToBone ("top");
			break;
		case 'torso':
			global.updateCameraToBone ("top");
			break;
		case 'shoes':
			global.updateCameraToBone ("shoes");
			break;
		case 'legs':
		case 'leftleg':
		case 'rightleg':
			global.updateCameraToBone ("legs");
			break;
		case 'hair':
		case 'beard':
		case 'eyebrows':
		case 'lenses':
		case 'lipstick':
		case 'blush':
		case 'makeup':
		case 'head':
		case 'glasses':
		case 'ears':
		case 'masks':
			global.updateCameraToBone ("hat");
			break;
		case 'watches':
		case 'leftarm':
			global.updateCameraToBone ("top");
			break;
		case 'bracelets':
		case 'rightarm':
			global.updateCameraToBone ("top");
			break;
	}
}
