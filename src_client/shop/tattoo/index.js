let tattooValues = [0, 0, 0, 0, 0, 0];
let tattooIds = ["torso", "head", "leftarm", "rightarm", "leftleg", "rightleg"];
let playerTattoos = {};

gm.events.add('client.tattoo.open', async (price, data) => {
	try
	{
		if (global.menuCheck()) return;

		playerTattoos = JSON.parse(data);
		global.menuOpen();
		mp.gui.emmit(`window.router.setView("BusinessBodyCustomization")`);
		await global.wait(50); 
		mp.gui.emmit(`window.bodycustomization.setType(${false});`);

		tattooValues = [0, 0, 0, 0, 0, 0];

		for (let i = 0; i < 6; i++) {
			let id = tattooIds[i];

			let tattooPrices = [];

			tattoos[id].forEach(item => {
				tattooPrices.push(Math.round (item.Price / 100 * price));
			});
			mp.gui.emmit(`window.bodycustomization.set('${id}','${JSON.stringify(tattooPrices)}')`);
		}
		global.createCamera ("tattooshop", global.localplayer);
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/tatoo/index", "client.tattoo.open", e.toString());
    }
});

gm.events.add('client.tattoo.update', (act, id, val) => {
	try
	{
		if(new Date().getTime() - global.lastCheck < 50) return; 
		global.lastCheck = new Date().getTime();
		switch (act) {
			case "buy":
				mp.events.callRemote("buyTattoo", tattooIds.indexOf(id), tattooValues[tattooIds.indexOf(id)]);
				break;
			case "style":
				const tId = tattooIds.indexOf(id);
				tattooValues[tId] = val;
				const tattoo = tattoos[id][val];
				const hash = (global.GetGender (global.localplayer)) ? tattoo.MaleHash : tattoo.FemaleHash;
				global.localplayer.clearDecorations();

				for (let x = 0; x < playerTattoos[tId].length; x++) { // Очищаем ненужные татушки

					for (let i = 0; i < tattoo.Slots.length; i++) {

						if (playerTattoos[tId][x] && playerTattoos[tId][x].Slots.indexOf(tattoo.Slots[i]) != -1) {
							playerTattoos[tId][x] = null;
							break;
						}

					}
				}

				for (let x = 0; x < 6; x++) // Восстанавливаем старые татуировки игрока, кроме тех, которые занимают очищенные слоты
					if (playerTattoos[x] != null)
						for (let i = 0; i < playerTattoos[x].length; i++)
							if (playerTattoos[x][i] != null)
								global.localplayer.setDecoration(mp.game.joaat(playerTattoos[x][i].Dictionary), mp.game.joaat(playerTattoos[x][i].Hash));

				global.localplayer.setDecoration(mp.game.joaat(tattoo.Dictionary), mp.game.joaat(hash)); // Ну и применяем выбранную татуировку

				global.updateBoneToPos (id);
				clearClothes (id);
				break;
		}
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/tatoo/index", "client.tattoo.update", e.toString());
    }
});


gm.events.add('client.tattoo.close', () => {
	try
	{
		if(new Date().getTime() - global.lastCheck < 50) return; 
		global.lastCheck = new Date().getTime();
		global.menuClose();

		global.cameraManager.stopCamera ();

		global.localplayer.clearDecorations();

		mp.gui.emmit(`window.router.setHud()`);
		mp.events.callRemote("cancelBody");
		playerTattoos = {};
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/tatoo/index", "client.tattoo.close", e.toString());
    }
});

const hatsComponentsToRemove = [ 1, 2 ];
const topsComponentsToRemove = [ 3, 5, 7, 8, 9, 10, 11 ];
const legsComponentsToRemove = [ 4, 6 ];

let removedClothing = [];
let selectId = "";
function clearClothes(id) {
	if (selectId === id)
		return;
	selectId = id;

	if (removedClothing.length !== 0)  {
		for (const _clothes of removedClothing) {
			global.localplayer.setComponentVariation(_clothes._componentId, _clothes.drawable, _clothes.texture, _clothes.palette);
		}
	
		removedClothing = [];
	}
		
	const gender = (global.GetGender (global.localplayer)) ? 1 : 0;
	if (id === "head") {
		for (const _componentId of hatsComponentsToRemove) {
			const drawable = global.localplayer.getDrawableVariation(_componentId);
			const texture = global.localplayer.getTextureVariation(_componentId);
			const palette = global.localplayer.getPaletteVariation(_componentId);
	
			removedClothing.push({ _componentId, drawable, texture, palette });
	
			global.localplayer.setComponentVariation(_componentId, clothesEmpty[gender][_componentId] !== undefined ? clothesEmpty[gender][_componentId] : 0, 0, 0);
		}
	} else if (id === "torso" || id === "leftarm" || id === "rightarm") {
		for (const _componentId of topsComponentsToRemove) {
			const drawable = global.localplayer.getDrawableVariation(_componentId);
			const texture = global.localplayer.getTextureVariation(_componentId);
			const palette = global.localplayer.getPaletteVariation(_componentId);
	
			removedClothing.push({ _componentId, drawable, texture, palette });
	
			global.localplayer.setComponentVariation(_componentId, clothesEmpty[gender][_componentId] !== undefined ? clothesEmpty[gender][_componentId] : 0, 0, 0);
		}
	} else if (id === "leftleg" || id === "rightleg") {
		for (const _componentId of legsComponentsToRemove) {
			const drawable = global.localplayer.getDrawableVariation(_componentId);
			const texture = global.localplayer.getTextureVariation(_componentId);
			const palette = global.localplayer.getPaletteVariation(_componentId);
	
			removedClothing.push({ _componentId, drawable, texture, palette });
	
			global.localplayer.setComponentVariation(_componentId, clothesEmpty[gender][_componentId] !== undefined ? clothesEmpty[gender][_componentId] : 0, 0, 0);
		}
	}
}