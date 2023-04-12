global.fixRotation = 180;
global.speedCamera = undefined;

global.createCamera = (name, entity = null) => {
	try 
	{
		if (name === "char") {
			const camera = global.cameraManager.getCamera ('createCharacter');
			global.cameraManager.setActiveCamera(camera, true);
			//Поворот камеры по x
			global.cameraInfo.polarAngleDeg = getPolarAngleByHeading (entity.getHeading(), 90);
			//Поворот камеры по y
			global.cameraInfo.azimutMax =  160 + global.fixRotation;
			global.cameraInfo.azimutMin = 80 + global.fixRotation;
			global.cameraInfo.azimuthAngleDeg = 90 + global.fixRotation;
			//Высота
			global.cameraInfo.lookZ = 0.0;        
			global.cameraInfo.heightToggled = true;//Возможность регулировать по высоте
			//Радиус
			global.cameraInfo.radius = -1.5;
			global.cameraInfo.radiusMax = -0.5;
			global.cameraInfo.radiusMin = -1.5;
			global.cameraInfo.overlayToggled = true;//Возможность регулировать отдаленгие
			//
			global.cameraPosition.poistionPoint = entity.getBoneCoords(12844, 0, 0, 0);
			//Возможность крутить камерой
			global.cameraInfo.activeMovementCamera = true;     
			//Инициализация
			global.cameraInfo.init = true;
		} else if (name === "autoshop") {
			const camera = global.cameraManager.getCamera ("autoshop");
			
			global.cameraManager.setActiveCamera(camera, true);

			//Поворот камеры по x
			global.cameraInfo.polarAngleDeg = getPolarAngleByHeading (entity.getHeading(), 200);
			//Поворот камеры по y
			global.cameraInfo.azimutMax =  160 + global.fixRotation;
			global.cameraInfo.azimutMin = 90 + global.fixRotation;
			global.cameraInfo.azimuthAngleDeg = 100 + global.fixRotation;
			//Высота
			global.cameraInfo.lookZ = 0.0;        
			global.cameraInfo.heightToggled = true;//Возможность регулировать по высоте
			//Радиус
			global.cameraInfo.radius = -6.5;
			global.cameraInfo.radiusMax = -2.25;
			global.cameraInfo.radiusMin = -6.5;
			global.cameraInfo.overlayToggled = true;//Возможность регулировать отдаленгие
			//
			global.cameraPosition.poistionPoint = entity.position;
			//Возможность крутить камерой
			global.cameraInfo.activeMovementCamera = true;     
			//Инициализация
			global.cameraInfo.init = true;
		} else if (name === "petshop") {
			const camera = global.cameraManager.createCamera ("petshop", new mp.Vector3(-755.5227, 320.0132, 177.302), new mp.Vector3(0, 0, 0), 50);
			global.cameraManager.setActiveCamera(camera, true);

			//Возможность крутить камерой
			global.cameraInfo.activeMovementCamera = false;
			camera.pointAtCoord(-758.2859, 320.9569, 175.7484);
		} else if (name === "barbershop") {
			const camera = global.cameraManager.createCamera ("petshop", new mp.Vector3(), new mp.Vector3(), 47);
			global.cameraManager.setActiveCamera(camera, true);
			//Поворот камеры по x
			//global.cameraInfo.polarAngleDeg = getPolarAngleByHeading (entity.getHeading(), 90);
			//Поворот камеры по y
			global.cameraInfo.azimutMax =  160 + global.fixRotation;
			global.cameraInfo.azimutMin = 80 + global.fixRotation;
			global.cameraInfo.azimuthAngleDeg = 100 + global.fixRotation;
			//Высота
			global.cameraInfo.lookZ = 0.0;        
			global.cameraInfo.heightToggled = true;//Возможность регулировать по высоте
			//Радиус
			global.cameraInfo.radius = -1.25;
			global.cameraInfo.radiusMax = -0.5;
			global.cameraInfo.radiusMin = -1.25;
			global.cameraInfo.overlayToggled = true;//Возможность регулировать отдаленгие
			//
			//global.cameraPosition.poistionPoint = entity.getBoneCoords(12844, 0, 0, 0);
			//Возможность крутить камерой
			global.cameraInfo.activeMovementCamera = true;     
			//Инициализация
			global.cameraInfo.init = true;
		} else if (name === "tattooshop") {
			const camera = global.cameraManager.getCamera ('createCharacter');
			global.cameraManager.setActiveCamera(camera, true);
			//Поворот камеры по x
			global.cameraInfo.polarAngleDeg = getPolarAngleByHeading (entity.getHeading(), 90);
			//Поворот камеры по y
			global.cameraInfo.azimutMax =  160 + global.fixRotation;
			global.cameraInfo.azimutMin = 80 + global.fixRotation;
			global.cameraInfo.azimuthAngleDeg = 90 + global.fixRotation;
			//Высота
			global.cameraInfo.lookZ = 0.0;        
			global.cameraInfo.heightToggled = true;//Возможность регулировать по высоте
			//Радиус
			global.cameraInfo.radius = -1.5;
			global.cameraInfo.radiusMax = -0.5;
			global.cameraInfo.radiusMin = -1.5;
			global.cameraInfo.overlayToggled = true;//Возможность регулировать отдаленгие
			//
			global.cameraPosition.poistionPoint = entity.getBoneCoords(11816, -0.13, 0.13, 0);

			//Возможность крутить камерой
			global.cameraInfo.activeMovementCamera = true;     
			//Инициализация
			global.cameraInfo.init = true;
		} else if (name === "spin") {
			const position = mp.game.object.getObjectOffsetFromCoords(entity.x, entity.y, entity.z, entity.rz, 0.0, -0.87, 1);  
			const pointAtCoord = mp.game.object.getObjectOffsetFromCoords(entity.x, entity.y, entity.z, entity.rz, 0.0, -0.87, 1);

			const camera = global.cameraManager.createCamera ("spin", new mp.Vector3(position.x, position.y, position.z), new mp.Vector3(0, 0, entity.rz), 40);
			
			global.cameraManager._setActiveCamera(camera);
			mp.game.cam.renderScriptCams(true, false, 3000, true, false);

			//Возможность крутить камерой
			global.cameraInfo.activeMovementCamera = false;
			camera.pointAtCoord(pointAtCoord.x, pointAtCoord.y, pointAtCoord.z);
		} else if (name === "peds") {
			const gameplayCam = global.cameraManager.gameplayCam();
			const getBoneCoords = entity.getBoneCoords(0, 0, 0, 0);
			let coords = entity.getOffsetFromGivenWorldCoords(getBoneCoords.x, getBoneCoords.y, getBoneCoords.z);
			coords = entity.getOffsetFromInWorldCoords(coords.x, coords.y, coords.z);
	
			const pedPosition = entity.position;
			
			const pedHeading = entity.getHeading ();
			
			const pointAtCoord = mp.game.object.getObjectOffsetFromCoords(pedPosition.x, pedPosition.y, pedPosition.z, pedHeading, 0, 1.5, 0);

			const camera = global.cameraManager.createCamera ("peds", new mp.Vector3(pointAtCoord.x, pointAtCoord.y, pointAtCoord.z + 0.75), new mp.Vector3(), 40);

			camera.pointAtCoord(coords.x, coords.y, coords.z + 0.5);
			camera.setActiveWithInterp(gameplayCam.handle, !global.speedCamera ? 500 : global.speedCamera, 1, 1);

			mp.game.cam.renderScriptCams(true, true, !global.speedCamera ? 500 : global.speedCamera, true, true);

			global.cameraManager._setActiveCamera(camera);

			//Возможность крутить камерой
			global.cameraInfo.activeMovementCamera = false;
		} else if (name === "testanim") {
			const camera = global.cameraManager.getCamera ('createCharacter');
			global.cameraManager.setActiveCamera(camera, true);
			//Поворот камеры по x
			global.cameraInfo.polarAngleDeg = getPolarAngleByHeading (entity.getHeading(), 90);
			//Поворот камеры по y
			global.cameraInfo.azimutMax =  160 + global.fixRotation;
			global.cameraInfo.azimutMin = 80 + global.fixRotation;
			global.cameraInfo.azimuthAngleDeg = 90 + global.fixRotation;
			//Высота
			global.cameraInfo.lookZ = 0.0;        
			global.cameraInfo.heightToggled = true;//Возможность регулировать по высоте
			//Радиус
			global.cameraInfo.radius = -3.5;
			global.cameraInfo.radiusMax = -0.5;
			global.cameraInfo.radiusMin = -3.5;
			global.cameraInfo.overlayToggled = true;//Возможность регулировать отдаленгие
			//
			global.cameraPosition.poistionPoint = entity.getBoneCoords(11816, -0.13, 0.13, 0);

			//Возможность крутить камерой
			global.cameraInfo.activeMovementCamera = true;     
			//Инициализация
			global.cameraInfo.init = true;
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "camera/create", "createCamera", e.toString());
	}
}

const getPolarAngleByHeading = (heading, fixHeading) => {
    heading = ((heading + 360.0) % 360.0) + 180.0;
    heading = heading > 360.0 ? (-360.0 + heading) : heading;
    return 360 - heading + fixHeading;
}