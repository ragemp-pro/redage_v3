
const activeResolution1 = mp.game.graphics.getScreenActiveResolution(0, 0);
const activeResolution2 = mp.game.graphics.getScreenActiveResolution(1, 1);

const editorType = {
	free: false,
	exact: true,
}


let objectEditor = {
    toggled: false,
    entity: null,
    prop: "",
    index: 0,
    arrayObject: [],
    call: null,
	callEsq: null,
	type: editorType.free
};

//

global.isEditor = false;
let selectedButton = "";
let selectedMod = false;
let isDowned = false;

let Buttons = {
	x: [0.0, 0.0, 0.0, 0.0],
	y: [0.0, 0.0, 0.0, 0.0],
	z: [0.0, 0.0, 0.0, 0.0],
};

let MouseSensitivity = 25.0;

let MouseRotSensitivity = 800.0;


const objectCreate = async (type) => {           
            
	if (objectEditor.entity)
		objectEditor.entity.destroy(), objectEditor.entity = null;
		
	if (objectEditor.petModel) {
		objectEditor.entity = mp.peds.new(objectEditor.petModel, global.localplayer.position, objectEditor.rot, global.localplayer.dimension);
	} else {
		const position = global.localplayer.position;

		position.x += 1;
		position.y += 1;

		await global.loadModel(objectEditor.prop);

		objectEditor.entity = mp.objects.new(objectEditor.prop, position, {
			'dimension': global.localplayer.dimension
		});
		
		if (!type) {
			objectEditor.marker = mp.markers.new(2, new mp.Vector3(position.x, position.y, position.z + 1.3), 0.3, {
				'rotation': new mp.Vector3(180, 0, 0),
				'color': [
					255,
					255,
					255,
					185
				],
				'visible': true,
				'dimension': global.localplayer.dimension
			});
		}
	}


	
	objectEditor.entity.setCollision(false, false);
}

let LastCursorPos = [0, 0];

gm.events.add("render", function () {
	try 
	{
        if (!global.loggedin) return;
        if (!objectEditor.toggled) return;

		if (objectEditor.type == editorType.free) {
			mp.game.controls.disableControlAction(2, 24, true);
			mp.game.controls.disableControlAction(2, 69, true);
			mp.game.controls.disableControlAction(2, 70, true);
			mp.game.controls.disableControlAction(2, 92, true);
			mp.game.controls.disableControlAction(2, 114, true);
			mp.game.controls.disableControlAction(2, 121, true);
			mp.game.controls.disableControlAction(2, 140, true);
			mp.game.controls.disableControlAction(2, 141, true);
			mp.game.controls.disableControlAction(2, 142, true);
			mp.game.controls.disableControlAction(2, 257, true);
			mp.game.controls.disableControlAction(2, 263, true);
			mp.game.controls.disableControlAction(2, 264, true);
			mp.game.controls.disableControlAction(2, 331, true);
			mp.game.controls.disableControlAction(2, 25, true);
			mp.game.controls.disableControlAction(2, 66, true);
			mp.game.controls.disableControlAction(2, 67, true);
			mp.game.controls.disableControlAction(2, 68, true);
            mp.game.controls.disableControlAction(2, 91, true);


			let position = mp.game.graphics.screen2dToWorld3d(new mp.Vector3(activeResolution2.x / 2, activeResolution2.y / 2, 0));
			let zPos = mp.game.gameplay.getGroundZFor3dCoord(position.x, position.y, position.z, 0, false);

			for (var i = 1; i < 11; i++) {
				if (zPos == 0) {
					zPos = mp.game.gameplay.getGroundZFor3dCoord(position.x, position.y, position.z + i, 0, false);
					if (zPos != 0) {
						break;
					}
				}
			}
			if (zPos == 0) {
				zPos = mp.game.gameplay.getGroundZFor3dCoord(position.x, position.y, position.z + 50, 0, false);
			}

			position.z = zPos;
            
            if (objectEditor.entity && objectEditor.entity.handle != 0) {
				objectEditor.entity.placeOnGroundProperly();
				position = objectEditor.entity.getCoords(true);
				const rot = objectEditor.entity.getRotation(2);
				objectEditor.entity.position = new mp.Vector3(position.x, position.y, position.z);
				objectEditor.entity.rotation = new mp.Vector3(rot.x, rot.y, rot.z);

                objectEditor.entity.setCollision(false, false);

				if (objectEditor.marker && objectEditor.marker.handle != 0) {

					objectEditor.marker.position = new mp.Vector3(position.x, position.y, position.z + 1.3);
				}
			}            
		} else if (objectEditor.entity != null && mp.objects.exists (objectEditor.entity) && objectEditor.entity.handle != 0) {
			//if (!objectEditor.entity.hasLosTo)
			//	return;
			//if (!objectEditor.entity.isOnScreen())
			//	return;
				
			/*const playerPosition = global.localplayer.position;
			const objectPosition = objectEditor.entity.position;

			let dist = mp.game.system.vdist(playerPosition.x, playerPosition.y, playerPosition.z, objectPosition.x, objectPosition.y, objectPosition.z);

			if (dist > 2)
				return;*/

			const 
				cursorPos = mp.gui.cursor.position,
				resX = activeResolution1.x,
				resY = activeResolution1.y;
	
			const position = objectEditor.entity.position;

			const xStart = new mp.Vector3(position.x + (-0.85), position.y, position.z);
			const xEnd = new mp.Vector3(position.x + (0.85), position.y, position.z);
			Buttons["x"] = global.DrawAxis(!selectedMod ? "X" : "RX", xStart, xEnd, [153, 153, 204, (selectedButton == "x" ? 255 : 150)]);	
			if (!Buttons["x"])
				return;

			const yStart = new mp.Vector3(position.x, position.y + (-0.85), position.z);
			const yEnd = new mp.Vector3(position.x, position.y + (0.85), position.z);
			Buttons["y"] = global.DrawAxis(!selectedMod ? "Y" : "RY", yStart, yEnd, [190, 143, 143, (selectedButton == "y" ? 255 : 150)]);
			if (!Buttons["y"])
				return;
	
			const zStart = new mp.Vector3(position.x, position.y, position.z + (-0.85));
			const zEnd = new mp.Vector3(position.x, position.y, position.z + (0.85));
			Buttons["z"] = global.DrawAxis(!selectedMod ? "Z" : "RZ", zStart, zEnd, [140, 180, 139, (selectedButton == "z" ? 255 : 150)]);
			if (!Buttons["z"])
				return;
	
			mp.game.controls.enableControlAction(0, 237, true);
					
			if (objectEditor.entity.isOnScreen() && mp.game.controls.isControlPressed(0, 237)) {
				if (!isDowned) {
					
					isDowned = true;
	
					if (selectedButton == "") {
						const x = cursorPos[0] / resX;
						const y = cursorPos[1] / resY;
						for (let key in Buttons) {
							const position = Buttons [key];
							const dist = mp.game.system.vdist(x, y, 0.0, position[0], position[1], 0.0);
							if (0.015 >= dist)
								selectedButton = key;
						}
					}
				}
			} else {
				isDowned = false;
				selectedButton = "";
			}
			
			if (selectedButton !== "") {
				const cursorDirX = (cursorPos[0] - LastCursorPos[0]) / resX;
				const cursorDirY = (cursorPos[1] - LastCursorPos[1]) / resY;
	
				const position = objectEditor.entity.position;
				const rotation = objectEditor.entity.rotation;
				const screen = mp.game.graphics.world3dToScreen2d(position.x, position.y, position.z);
	
				const
					mainScreenX = screen.x,
					mainScreenY = screen.y;
					
				if(selectedButton == 'x') { 
					if(!selectedMod) {
						const magnitude = global.GetMagnitudeOffset(position, cursorDirX, cursorDirY, mainScreenX, mainScreenY, 1.0);
						position.x += magnitude*MouseSensitivity;
					} else {
						const magnitude = global.GetMagnitudeOffset(position, cursorDirX, cursorDirY, mainScreenX, mainScreenY, 0, 1);
						rotation.x -= magnitude*MouseRotSensitivity;
					}
				} else if(selectedButton == 'y') {
					if(!selectedMod) {
						const magnitude = global.GetMagnitudeOffset(position, cursorDirX, cursorDirY, mainScreenX, mainScreenY, 0, 1);
						position.y += magnitude*MouseSensitivity;
					} else {
						const magnitude = global.GetMagnitudeOffset(position, cursorDirX, cursorDirY, mainScreenX, mainScreenY, 1);
						rotation.y += magnitude*MouseRotSensitivity;
					}
				} else if(selectedButton == 'z') {
					const magnitude = global.GetMagnitudeOffset(position, cursorDirX, cursorDirY, mainScreenX, mainScreenY, 0, 0, 1);  
					if(!selectedMod) {
						position.z += magnitude*MouseSensitivity;
					} else {
						rotation.z += cursorDirX*MouseRotSensitivity*0.2;
					}
				}

				objectEditor.entity.position = position;
				objectEditor.entity.rotation = rotation;
			}

			if (!mp.keys.isDown(global.Keys.VK_SPACE)) {
				mp.game.controls.disableAllControlActions(0);
				mp.gui.cursor.visible = true;
			} else {
				mp.gui.cursor.visible = false;
			}

			LastCursorPos = cursorPos;
		}
	} 
	catch (e) 
	{
		if(new Date().getTime() - global.trycatchtime["inventory/objectEditor"] < 60000) return;
		global.trycatchtime["inventory/objectEditor"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "inventory/objectEditor", "render", e.toString());
	}
});

gm.events.add('click', (x, y, upOrDown, leftOrRight, relativeX, relativeY, worldPosition, hitEntity) => {
	try
	{
		if (!objectEditor.toggled) return;
		else if (leftOrRight !== 'left' && leftOrRight !== 'right') return;
		if (leftOrRight == 'left' && global.localplayer.isInWater()) return mp.events.call('notify', 1, 9, translateText("Нельзя устанавливать объекты здесь"), 3000);
		else if (objectEditor.type === editorType.exact) {
			if (upOrDown == 'up' && leftOrRight === 'right') {
				mp.events.call('client.dropinfo.mod');
			}
			return;
		}
		//if (global.isInSafeZone) return mp.events.call('notify', 1, 9, translateText("Нельзя ставить в зеленой зоне"), 3000);
		if (leftOrRight == 'left' && mp.objects.exists(objectEditor.entity)) {
			mp.events.call('client.dropinfo.enter');
			return;
		}

		mp.events.call('client.dropinfo.close');

	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "inventory/objectEditor", "click", e.toString());
	}
});

gm.events.add('client.dropinfo.enter', () => {
	if (!objectEditor.toggled) 
		return;
	
	if (objectEditor.entity && mp.objects.exists (objectEditor.entity)) {
		const
			pos = objectEditor.entity.position,
			rot = objectEditor.entity.rotation,
			index = objectEditor.index;

		objectEditor.call (pos, rot, index)
	}
	mp.events.call('client.dropinfo.close', true);
});

gm.events.add('client.dropinfo.close', (isEnter = false) => {

	if (objectEditor.entity && mp.objects.exists(objectEditor.entity))
		objectEditor.entity.destroy();

	if (objectEditor.marker)
		objectEditor.marker.destroy();

	//if (global.isInSafeZone)
	// mp.events.call('notify', 1, 9, translateText("Нельзя ставить в зеленой зоне"), 3000);

	if (!isEnter && objectEditor.callEsq != null && typeof objectEditor.callEsq === "function")
		objectEditor.callEsq ()

	mp.gui.emmit(`window.router.setHud()`);
	global.menuClose(); 
	objectEditor = {
		toggled: false,
		entity: null,
		prop: "",
		index: 0,
		arrayObject: [],
		call: null,
		callEsq: null,
		type: editorType.free
	}

	global.isEditor = false;
});

mp.keys.bind(global.Keys.VK_DOWN, true, () => {
	try
	{
		if (!objectEditor.toggled) return;
		if (objectEditor.entity && mp.objects.exists (objectEditor.entity)) {
			let rot = objectEditor.entity.rotation.z;

			rot  -= 5;

			if (rot < 0)
				rot = 180;

			objectEditor.entity.rotation = new mp.Vector3(0, 0, rot);
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "inventory/objectEditor", "VK_DOWN", e.toString());
	}
});

mp.keys.bind(global.Keys.VK_UP, true, () => {
	try
	{
		if (!objectEditor.toggled) return;
		if (objectEditor.entity && mp.objects.exists (objectEditor.entity)) {
			let rot = objectEditor.entity.rotation.z;

			rot += 5;

			if (rot > 180)
				rot = 0;

			objectEditor.entity.rotation = new mp.Vector3(0, 0, rot);
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "inventory/objectEditor", "VK_UP", e.toString());
	}
});

mp.keys.bind(global.Keys.VK_RIGHT, true, () => {
	try
	{
		if (!objectEditor.toggled) return;
		else if (!objectEditor.arrayObject || !objectEditor.arrayObject.length) return;

		if (++objectEditor.index === objectEditor.arrayObject.length)
			objectEditor.index = 0;
		
		objectEditor.prop = mp.game.joaat (objectEditor.arrayObject [objectEditor.index]);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "inventory/objectEditor", "VK_RIGHT", e.toString());
	}
});

mp.keys.bind(global.Keys.VK_LEFT, true, () => {
	try
	{
		if (!objectEditor.toggled) return;
		else if (!objectEditor.arrayObject || !objectEditor.arrayObject.length) return;
		
		if (--objectEditor.index === -1)
			objectEditor.index = objectEditor.arrayObject.length - 1;
		
		objectEditor.prop = mp.game.joaat (objectEditor.arrayObject [objectEditor.index]);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "inventory/objectEditor", "VK_LEFT", e.toString());
	}
});

mp.keys.bind(global.Keys.VK_OEM_3, false, function () { // ` key
    if (!objectEditor.toggled) return;
	if (objectEditor.petModel) 
		return;
	if (objectEditor.type !== editorType.exact) 
		return;

    mp.gui.cursor.visible = !mp.gui.cursor.visible;
});

gm.events.add('client.dropinfo.updateType', (type) => {
	if (!objectEditor.toggled) return;
	if (objectEditor.petModel) return;

	objectEditor.type = type;

	if (objectEditor.type === editorType.exact) {
		if (objectEditor.marker)
			objectEditor.marker.destroy(), objectEditor.marker = null;

		mp.gui.cursor.visible = true;
	} else {
		mp.gui.cursor.visible = false;
		if (objectEditor.marker)
			objectEditor.marker.destroy(), objectEditor.marker = null;

		if (objectEditor.entity && mp.objects.exists (objectEditor.entity)) {
			const position = objectEditor.entity.position;

			objectEditor.marker = mp.markers.new(2, new mp.Vector3(position.x, position.y, position.z + 1.3), 0.3, {
				'rotation': new mp.Vector3(180, 0, 0),
				'color': [
					255,
					255,
					255,
					185
				],
				'visible': true,
				'dimension': global.localplayer.dimension
			});
		}
	}
});

gm.events.add('client.dropinfo.mod', () => {
	if (!objectEditor.toggled) 
		return;
	if (objectEditor.petModel) 
		return;
	if (objectEditor.type !== editorType.exact) 
		return;
	
	selectedMod = !selectedMod;
});

global.OnPetEditor = (model, arrayObject = [], call = null, callEsq = null) => {  
	try
	{
		mp.gui.emmit(`window.router.setView('PlayerDropinfo')`);
		global.menuOpened = true;
		objectEditor = {
			toggled: true,
			entity: null,
			marker: null,
			petModel: model,
			index: 0,
			arrayObject: arrayObject,
			call: call,
			callEsq: callEsq,
		}
		global.isEditor = true;
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "inventory/objectEditor", "global.OnObjectEditor", e.toString());
	}  
}

global.OnObjectEditor = (prop, arrayObject = [], call = null, callEsq = null, type = editorType.free) => {  
	try
	{
		mp.gui.emmit(`window.router.setView('PlayerDropinfo', ${type})`);
		global.menuOpened = true;
		selectedButton = "";
		selectedMod = false;
		isDowned = false;

		objectEditor = {
			toggled: true,
			entity: null,
			marker: null,
			prop: prop,
			index: 0,
			arrayObject: arrayObject,
			call: call,
			callEsq: callEsq,
			type: type
		}
		global.isEditor = true;
		objectCreate (type);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "inventory/objectEditor", "global.OnObjectEditor", e.toString());
	}  
}
/*
mp.keys.bind(global.Keys.VK_7, true, () => {
    if (objectEditor.toggled) return;
    
    const data = [
        "p_stinger_03",
        "prop_barrier_work01a",
        "prop_barrier_work01d",
        "prop_barier_conc_05c",
        "prop_barier_conc_02a",
        "prop_air_barrier",
        "prop_air_lights_02a",
        "prop_air_conelight",
        "prop_plas_barier_01a",
        "xm_prop_base_fence_01",
        "xm_prop_base_fence_02",
        "prop_facgate_04_r",
        "prop_fncres_03c",
        "prop_barier_conc_02b",
        "prop_barier_conc_01b",
        "prop_barrier_work05",
        "ba_prop_battle_barrier_02a",
        "prop_barriercrash_01",
        "prop_barrier_wat_03a",
        "prop_mc_conc_barrier_01",
        "prop_mp_barrier_02b"]
    global.OnObjectEditor (mp.game.joaat ("p_stinger_03"), data, (pos, rot, index) => {
        mp.events.callRemote('server.object.finish', data [index], pos.x, pos.y, pos.z, rot, 1800000);
    })
});

mp.keys.bind(global.Keys.VK_8, true, () => {
    if (objectEditor.toggled) return;
    
    const data = [
        "ind_prop_firework_01",
        "ind_prop_firework_02",
        "ind_prop_firework_04",
        "ind_prop_firework_03"]
    global.OnObjectEditor (mp.game.joaat ("ind_prop_firework_01"), data, (pos, rot, index) => {
        mp.events.callRemote('server.object.finish', data [index], pos.x, pos.y, pos.z, rot, 25000);
    })
});*/

let Editor = {
	objid: null,
	save: "",
	objects: []
}

gm.events.add('client.editor.start', (objid, save, isDell = false) => {
	try
	{
		Editor.objid = objid;
		Editor.save = save;

		if (!isDell) {
			Editor.objects.forEach((obj) => {
				if (obj && mp.objects.exists(obj))
				obj.destroy();
			});
			Editor.objects = [];
		}

		global.OnObjectEditor (mp.game.joaat(objid), null, (pos, rot, _) => {
			if (Editor.objid) {
				const obj = mp.objects.new(mp.game.joaat(objid), pos, {
					'rotation': new mp.Vector3(0, 0, rot),
					'dimension': global.localplayer.dimension
				});

				Editor.objects.push(obj)
				mp.events.callRemote('server.editor.save', Editor.objid, Editor.save, pos.x, pos.y, pos.z, rot);
				setTimeout(() => {
					mp.events.call('client.editor.start', Editor.objid, Editor.save, true);
				}, 250)
			}
		})
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "inventory/dropEditor", "client.editor.start", e.toString());
	}
})