const { barberPlacesData, hairIDList, barberIdsNew, HeadOverlayData, topsComponentsToRemove } = require('./data.js');

let barberCharData = {};
let barberData = {};

const barberInteriors = barberPlacesData.map((place) => place.interior);

let pedBarber = null;
let scissorsObj = null;
let currentPlace;
let startAnim = false;

let selectToBuyComponent = "";

const onCreateBarberScene = async () => {
	try
	{
		const interior = getCurrentInterior();
		let placeIndex;
		
		if (interior === 0 || (placeIndex = barberInteriors.indexOf(interior)) < 0)
			return;

		currentPlace = barberPlacesData[placeIndex];

		const playerPos = global.localplayer.position;
		const playerDimension = global.localplayer.dimension;
		const chairInfo = currentPlace.chair;
		const exitPos = currentPlace.exit.position;
		//Создаем клона игрока
		pedBarber = mp.peds.new(currentPlace.pedModel, playerPos, 0, playerDimension);
		scissorsObj = mp.objects.new(0xB7087FA9, currentPlace.scissorsPosition, {
			dimension: playerDimension
		});
		
		await global.IsLoadEntity (scissorsObj);
		
		global.localplayer.position = new mp.Vector3(exitPos.x, exitPos.y, exitPos.z);
		global.localplayer.setHeading(currentPlace.exit.heading);
		global.localplayer.setCollision(false, true);

		playVoice("SHOP_HAIR_WHAT_WANT");
		
		global.requestAnimDict(currentPlace.animDict).then(async () => {

			global.localplayer.taskPlayAnimAdvanced(currentPlace.animDict, "player_enterchair", chairInfo.position.x, chairInfo.position.y, 
				chairInfo.position.z, 0, 0, chairInfo.heading, 1000, -1000, -1, 5642, 0, 2, 1);
			playPedBarberAnim("keeper_enterchair", "scissors_enterchair");

			global.createCamera ("barbershop");

			global.cameraPosition.poistionPoint = currentPlace.cam.position;
			global.cameraInfo.polarAngleDeg = currentPlace.cam.heading;
			
			startAnim = false;
		});
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "shop/barber/index", "onCreateBarberScene", e.toString());
	}
}

function onBarberFinished() {
	try
	{
		global.localplayer.stopAnimTask(currentPlace.animDict, "player_exitchair", 3);
		global.localplayer.clearTasksImmediately();
		global.localplayer.setCollision(true, true);
		mp.events.callRemote("cancelBody");
		stage = -1;
		isBarberStarted = false;
		barberCharData = {};
		barberData = {};
		destroyEntities();
		global.menuClose();
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "shop/barber/index", "onBarberFinished", e.toString());
	}
}

function destroyEntities() {
	try
	{
		if(pedBarber != null) pedBarber.destroy();
		if(scissorsObj != null) scissorsObj.destroy();

		pedBarber = null;
		scissorsObj = null;
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "shop/barber/index", "destroyEntities", e.toString());
	}
}

function getCurrentInterior() {
	return mp.game.invoke("0x2107BA504071A6BB", global.localplayer.handle);
}

function playVoice(speechName) {
	try
	{
		if (!pedBarber || pedBarber.type !== 'ped') return;
		const voice = currentPlace.pedModel === 0x418DFF92 ? "S_M_M_HAIRDRESSER_01_BLACK_MINI_01" : "S_F_M_FEMBARBER_BLACK_MINI_01";

		mp.game.audio.playAmbientSpeechWithVoice(pedBarber.handle, speechName, voice, "SPEECH_PARAMS_FORCE", false);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "shop/barber/index", "playVoice", e.toString());
	}
}

let sceneId = -1;

function createScene(looped = false) {
	try
	{
		if (sceneId !== -1) {
			mp.game.ped.detachSynchronizedScene(sceneId);
			mp.game.ped.disposeSynchronizedScene(sceneId);
			sceneId = -1;
		}

		const chairInfo = currentPlace.chair;

		sceneId = mp.game.ped.createSynchronizedScene(chairInfo.position.x, chairInfo.position.y, chairInfo.position.z, 0, 0, chairInfo.heading, 2);

		mp.game.invoke("0x394B9CD12435C981", sceneId, true);
		mp.game.ped.setSynchronizedSceneLooped(sceneId, looped);

		return sceneId;
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "shop/barber/index", "createScene", e.toString());
		return -1;
	}
}

function playPedBarberAnim(keeperAnim, scissorsAnim = undefined, looped = false) {
	try
	{
		if (!pedBarber || pedBarber.type !== 'ped') return;
		sceneId = createScene(looped);
		if (sceneId == null || sceneId == -1) return;
		pedBarber.taskSynchronizedScene(sceneId, currentPlace.animDict, keeperAnim, 1000, -1056964608, 0, 0, 1148846080, 0);

		if (scissorsAnim) {
			scissorsObj.setInvincible(false);
			scissorsObj.playSynchronizedAnim(sceneId, scissorsAnim, currentPlace.animDict, 1000, -1000, 0, 1148846080);
			scissorsObj.forceAiAndAnimationUpdate();
		} else {
			scissorsObj.setInvincible(true);
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "shop/barber/index", "playPedBarberAnim", e.toString());
	}
}


let isBarberStarted = false;
let stage = -1;
let cutSoundStarted = false;
let cutSound;

let currentCutAnim = undefined;

gm.events.add("render", () => {
	if (!global.loggedin) return;
	if (isBarberStarted) {
		if (stage === 0 && global.localplayer && currentPlace != undefined && global.localplayer.hasAnimFinished(currentPlace.animDict, "player_enterchair", 3)) {
			playBaseAnims();
		}

		if (stage === 2) {
			if (pedBarber && pedBarber.hasAnimFinished(currentPlace.animDict, currentCutAnim, 3)) {
				playBaseAnims();
			}

			if (sceneId !== -1) {
				const phase = mp.game.ped.getSynchronizedScenePhase(sceneId);

				if (phase >= 0.3 && phase <= 0.4 && !cutSoundStarted) {
					//if (cutAcceptCallback) {
					//	cutAcceptCallback();
					//	cutAcceptCallback = undefined;
					//}

					mp.game.audio.playSoundFromEntity(1488, cutSound, pedBarber.handle, "Barber_Sounds", false, 0);
					cutSoundStarted = true;
					mp.events.call("client.barber.updateData", selectToBuyComponent, JSON.stringify (barberData[selectToBuyComponent]));
				} else if (phase >= 0.6 && cutSoundStarted) {
					mp.game.audio.stopSound(1488);
					cutSoundStarted = false;
					startAnim = false;
				}
			}
		}

		if (stage === 3 && global.localplayer && (global.localplayer.hasAnimFinished(currentPlace.animDict, "player_exitchair", 3) || !global.localplayer.isPlayingAnim(currentPlace.animDict, "player_exitchair", 3))) {
			onBarberFinished();
		}
		return;
	}
});

function playBaseAnims() {
	try
	{
		stage = 1;

		const chairInfo = currentPlace.chair;

		global.localplayer.taskPlayAnimAdvanced(currentPlace.animDict, "player_base", chairInfo.position.x, chairInfo.position.y, 
			chairInfo.position.z, 0, 0, chairInfo.heading, 8, 8, -1, 5641, 0, 2, 1);
		playPedBarberAnim("keeper_base", "scissors_base", true);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "shop/barber/index", "playBaseAnims", e.toString());
	}
}

function playCutAnim(withScissors = true) {
	try
	{
		if (startAnim)
			return;
		const cutVariant = Math.random() >= 0.5 ? "a" : "b";

		currentCutAnim = getCutAnimPart() + cutVariant;

		//instructionButtonsDrawler.setActive(false);

		//mp.events.call("selectMenu.hide", "barbershop_concrete");
		//camera.setFov(47);

		playVoice("SHOP_CUTTING_HAIR");

		if (withScissors) {
			cutSound = "Scissors";
			playPedBarberAnim(currentCutAnim, currentCutAnim.replace("keeper_", "scissors_"));
		} else {
			cutSound = "Makeup";
			playPedBarberAnim(currentCutAnim);
		}

		stage = 2;
		cutSoundStarted = false;
		startAnim = true;
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "shop/barber/index", "playCutAnim", e.toString());
	}
}

function getCutAnimPart() {
	return currentPlace.animDict.indexOf("hair_dressers") >= 0 ? "keeper_hair_cut_" : "keeper_idle_";
}

gm.events.add('client.barber.open', async (price, json) => {
	try
	{
		if (global.menuCheck()) return;	
		stage = 0;
		isBarberStarted = true;
		startAnim = true;
		global.menuOpen();
		mp.gui.emmit(`window.router.setView("BusinessBodyCustomization")`);
		await global.wait(50); 
		mp.gui.emmit(`window.bodycustomization.setType(${true});`);
		barberCharData = JSON.parse (json);
		barberData = JSON.parse (json);
		for (let i = 0; i < 8; i++) {
			let id = barberIdsNew[i];
			let bizBarberPrices = [];
			global.barberPrices[id].forEach(cost => {
				bizBarberPrices.push(Math.round (cost / 100 * price));
			});
			mp.gui.emmit(`window.bodycustomization.set('${id}','${JSON.stringify(bizBarberPrices)}')`);
		}
		onCreateBarberScene ();
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "shop/barber/index", "client.barber.open", e.toString());
	}
});


gm.events.add('client.barber.startAnim', () => {
	try
	{
		switch (selectToBuyComponent) {
			case "hair":
			case "lenses":
				playCutAnim ();
				break;
			default:
				playCutAnim (HeadOverlayData[selectToBuyComponent] <= 3);
				break;
		}
		setDafaultAppearance (selectToBuyComponent, barberCharData);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "shop/barber/index", "client.barber.startAnim", e.toString());
	}
});

gm.events.add('client.barber.updateData', (id, data) => {
	barberCharData [id] = JSON.parse (data);
	setDafaultAppearance (selectToBuyComponent, barberCharData);
});


let removedClothing = [];
gm.events.add('client.barber.update', (act, id, val) => {
	try
	{
		if (startAnim)
			return;
		else if (new Date().getTime() - global.lastCheck < 50) return; 
		global.lastCheck = new Date().getTime();
		switch (act) {
			case "buy":
				selectToBuyComponent = id;
				mp.events.callRemote("server.barber.buy", id, barberData[id][0], barberData[id][1] ? barberData[id][1] : 0);
				break;
			case "style":
				global.updateBoneToPos (id);
				if (id === "chesthair" && removedClothing.length === 0) { // Torso hair
					const gender = (global.GetGender (global.localplayer)) ? 1 : 0;
					for (const _componentId of topsComponentsToRemove) {
						const drawable = global.localplayer.getDrawableVariation(_componentId);
						const texture = global.localplayer.getTextureVariation(_componentId);
						const palette = global.localplayer.getPaletteVariation(_componentId);
		
						removedClothing.push({ _componentId, drawable, texture, palette });
		
						global.localplayer.setComponentVariation(_componentId, clothesEmpty[gender][_componentId] ? clothesEmpty[gender][_componentId] : 0, 0, 0);
					}
				} else if (id !== "chesthair" && removedClothing.length !== 0)  {
					for (const _clothes of removedClothing) {
						global.localplayer.setComponentVariation(_clothes._componentId, _clothes.drawable, _clothes.texture, _clothes.palette);
					}
				
					removedClothing = [];
				}

				switch (id) {
					case "hair":
						let gender = (global.GetGender (global.localplayer)) ? 0 : 1;
						barberData["hair"][0] = hairIDList[gender][val];
						break;
					case "lenses":
						barberData["lenses"][0] = val;
						break;
					default:
						barberData [id][0] = (val == 0) ? 255 : val - 1;
						break;
				}
				setDafaultAppearance (id, barberData);
				break;
			case "color":
				global.updateBoneToPos (id);
				switch (id) {
					case "hair":
						barberData["hair"][1] = val;
						break;
					default:
						barberData [id][1] = val;
						break;
				}
				setDafaultAppearance (id, barberData);
				break;
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "shop/barber/index", "client.barber.update", e.toString());
	}
});


const setDafaultAppearance = (id, data) => {
	try
	{
		switch (id) {
			case "hair":
				global.localplayer.setComponentVariation(2, data["hair"][0], 0, 0);
				global.localplayer.setHairColor(data["hair"][1], 0);
				break;
			case "lenses":
				global.localplayer.setEyeColor(data["lenses"][0]);
				break;
			default:
				if (!HeadOverlayData [id]) return;
				global.localplayer.setHeadOverlay(HeadOverlayData [id], data[id][0], 100, data[id][1] ? data[id][1] : 0, data[id][1] ? data[id][1] : 0);
				break;

		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "shop/barber/index", "setDafaultAppearance", e.toString());
	}
}

gm.events.add('client.barber.close', () => {
	try
	{
		if(new Date().getTime() - global.lastCheck < 50) return; 
		global.lastCheck = new Date().getTime();

		global.cameraManager.stopCamera ();

		global.localplayer.clearDecorations();

		setDafaultAppearance (selectToBuyComponent, barberCharData);
		mp.gui.emmit(`window.router.setHud()`);
		mp.gui.cursor.visible = false;
		const chairInfo = currentPlace.chair;

		playVoice("SHOP_GOODBYE");
		
		global.localplayer.taskPlayAnimAdvanced(currentPlace.animDict, "player_exitchair", chairInfo.position.x, chairInfo.position.y, 
			chairInfo.position.z, 0, 0, chairInfo.heading, 1000, -1000, -1, 5642, 0, 2, 1);
		playPedBarberAnim("keeper_exitchair", "scissors_exitchair");
		
		stage = 3;
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "shop/barber/index", "client.barber.close", e.toString());
	}
});


