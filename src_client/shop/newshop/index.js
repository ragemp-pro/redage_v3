global.clothesEmpty = {0: {1: 0, 3: 15, 4: 15, 5: 0, 6: 35, 7: 0, 8: 6, 9: 0, 10: 0, 11: 15},1: {1: 0, 3: 15, 4: 21, 5: 0, 6: 34, 7: 0, 8: 15, 9: 0, 10: 0, 11: 15}};

global.GetGender = (entity) => {   
    try { 
        if (entity) {
            if (entity.model == 1885233650)
                return true;
            else if (entity.model == 2627665880)
                return false;    
        }    
        return -1;
    }
    catch (e) 
    {
        return -1;
    }
}

const { barberPlacesData } = require('./data.js');

let shopData = {
    ped: null,
    type: "",
    priceType: 0,
    isOpen: false,
    priceList: [],
    tattoos: [],
    isDonate: false
}

const barberInteriors = barberPlacesData.map((place) => place.interior);

const Open = async (type, gender, menuList, priceList, priceType, tattoos) => {
    try
    {
        if (global.menuCheck()) return;
        shopData.type = type;
        shopData.priceType = priceType;
        shopData.priceList = JSON.parse(priceList);
        shopData.isDonate = priceType === 1;
        shopData.isOpen = true;

        //global.localplayer.dimension = 15000 + global.localplayer.remoteId;

        const playerPos = global.localplayer.position;
        const playerDimension = global.localplayer.dimension;
        const playerHeading = global.localplayer.getHeading();

        shopData.ped = mp.peds.new(global.localplayer.model, playerPos, playerHeading, playerDimension);

		await global.IsLoadEntity (shopData.ped);

        global.localplayer.cloneToTarget(shopData.ped.handle);
        global.localplayer.freezePosition(true);
        global.localplayer.setAlpha(0);
        global.localplayer.setCollision(false, false);
        gm.discord(translateText("Выбирает одежду в магазине"));

        global.menuOpen();

        mp.gui.emmit(`window.router.setView("BusinessClothes", {type: '${type}', menuList: '${menuList}', priceType: ${priceType}, priceList: '${priceList}', gender: ${gender}});`);

        switch (type) {
            case "barber":
                const interior = getCurrentInterior();
                let placeIndex;
                
                if (interior === 0 || (placeIndex = barberInteriors.indexOf(interior)) < 0)
                    return;

                let currentPlace = barberPlacesData[placeIndex];

                shopData.ped.position = new mp.Vector3(currentPlace.exit.position.x, currentPlace.exit.position.y, currentPlace.exit.position.z);
                shopData.ped.setHeading(currentPlace.exit.heading);
                shopData.ped.setCollision(false, true);

                global.requestAnimDict(currentPlace.animDict).then(async () => {

                    shopData.ped.taskPlayAnimAdvanced(currentPlace.animDict, "player_enterchair", currentPlace.chair.position.x, currentPlace.chair.position.y, 
                    currentPlace.chair.position.z, 0, 0, currentPlace.chair.heading, 1000, -1000, -1, 5642, 0, 2, 1);
                    //playPedBarberAnim("keeper_enterchair", "scissors_enterchair");

                    global.createCamera ("barbershop");

                    global.cameraPosition.poistionPoint = currentPlace.cam.position;
                    global.cameraInfo.polarAngleDeg = currentPlace.cam.heading;
                    gm.discord(translateText("Выбирает прическу"));                
                });
                break;
            case "tattoo":
                global.createCamera ("char", shopData.ped);
                shopData.tattoos = JSON.parse (tattoos);
                gm.discord(translateText("Присматривает новое тату"));
                break;
            default:
                global.createCamera ("char", shopData.ped);
                break;
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/newshop/index", "client.shop.open", e.toString());
    }
}

gm.events.add('client.shop.open', Open);

function getCurrentInterior() {
	return mp.game.invoke("0x2107BA504071A6BB", global.localplayer.handle);
}


const UpdateTattoos = (tattoos) => {
    shopData.tattoos = JSON.parse (tattoos);
}

gm.events.add('client.shop.tattoos', UpdateTattoos);

const Close = () => {    
    if (!shopData.isOpen) return;

    if (shopData.ped.doesExist())
        shopData.ped.destroy();

    //global.localplayer.dimension = 0;

    global.menuClose();
    mp.events.callRemote('server.shop.close');
    mp.gui.emmit('window.router.setHud();');
	global.cameraManager.stopCamera ();
    global.localplayer.freezePosition(false);
    global.localplayer.setAlpha(255);
    global.localplayer.setCollision(true, true);

    shopData = {
        ped: null,
        type: "",
        priceType: 0,
        isOpen: false,
        price: 0,
        isDonate: false
    }
}

gm.events.add('client.shop.close', Close);


const getIndexToTextureName = (Name, TName, Index, Id) => {
    try {
        if (TName && TName.length > 1) {
            const name = mp.game.ui.getLabelText(`${TName}${Index}`);
            if (name && name.toLowerCase() !== "null" && name.length > 3)
                return global.escapeHtml (name) + `[${Id}]`
        }
        if (Name && Name.length > 1)
            return global.escapeHtml (Name) + `[${Id}]`;
        return Id;
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/clothes/index", "getIndexToTextureName", e.toString());
        return Id;
    }
}

const ClothesGetDictionary = (data) => {

    data = JSON.parse (data);
    data = Object.values (data);

    let returnData = [];
    let newData;
    let index;

    data.forEach((item, _) => {
        newData = item;

        if (newData.Textures && newData.Textures [0])
            index = newData.Textures [0];
        else
            index = 0;

        newData.descName = getIndexToTextureName (newData.Name, newData.TName, index, newData.Id);

        if (newData.Name && newData.Name.length > 1)
            newData.Name = global.escapeHtml (newData.Name)

        returnData.push (newData);
    });
    
    mp.gui.emmit(`window.events.callEvent("cef.clothes.updateDictionary", '${JSON.stringify (returnData)}')`);
}

gm.events.add('client.clothes.getDictionary', ClothesGetDictionary);

//

const getComponentName = (Name, TName, Index, Id) => {
    const name = getIndexToTextureName (Name, TName, Index, Id);

    mp.gui.emmit(`window.events.callEvent("cef.clothes.setName", '${name}')`);
}

gm.events.add('client.shop.getIndexToTextureName', getComponentName);

//

const setComponentVariation = (componentId, Variation, Texture, isClone = true) => {

    if (isClone)
        global.localplayer.cloneToTarget(shopData.ped.handle);

    //

    shopData.ped.setComponentVariation(Number (componentId), Number (Variation), Number (Texture), 0);
}

gm.events.add('client.clothes.setComponentVariation', setComponentVariation);

//

const setPropIndex = (componentId, Variation, Texture, isClone = true) => {

    if (isClone)
        global.localplayer.cloneToTarget(shopData.ped.handle);

    //
    
    if (Number (Variation) !== -1)
        shopData.ped.setPropIndex(Number (componentId), Number (Variation), Number (Texture), true);
    else 
        clearProp (Number (componentId));
}

gm.events.add('client.clothes.setPropIndex', setPropIndex);
//

const clearProp = (propId) => {

    shopData.ped.clearProp(Number (propId));
}

gm.events.add('client.clothes.clearProp', clearProp);

//

const clearMask = () => {
    shopData.ped.setFaceFeature(0, -1.5);
    shopData.ped.setFaceFeature(2, 1.5);
    shopData.ped.setFaceFeature(9, -1.5);
    shopData.ped.setFaceFeature(10, -1.5);
    shopData.ped.setFaceFeature(13, -1.5);
    shopData.ped.setFaceFeature(14, -1.5);
    shopData.ped.setFaceFeature(15, -1.5);
    shopData.ped.setFaceFeature(16, -1.5);
    shopData.ped.setFaceFeature(17, -1.5);
    shopData.ped.setFaceFeature(18, 1.5);
}

gm.events.add('client.clothes.clearMask', clearMask);

//
const setHairColor = (firstColor, secondColor) => {
    //global.localplayer.cloneToTarget(shopData.ped.handle);

    //

    shopData.ped.setHairColor(Number (firstColor), Number (secondColor));
}

gm.events.add('client.clothes.setHairColor', setHairColor);

//

const setHeadOverlay = (overlayID, index, opacity) => {
    global.localplayer.cloneToTarget(shopData.ped.handle);

    //

    shopData.ped.setHeadOverlay(Number (overlayID), Number (index), Number (opacity));
}

gm.events.add('client.clothes.setHeadOverlay', setHeadOverlay);
//

const setHeadOverlayColor = (overlayID, colorType, colorID) => {
    //global.localplayer.cloneToTarget(shopData.ped.handle);

    //

    shopData.ped.setHeadOverlayColor(Number (overlayID), Number (colorType), Number (colorID), 0);
}

gm.events.add('client.clothes.setHeadOverlayColor', setHeadOverlayColor);

//
const setEyeColor = (index) => {
    global.localplayer.cloneToTarget(shopData.ped.handle);

    //

    shopData.ped.setEyeColor(Number (index));
}

gm.events.add('client.clothes.setEyeColor', setEyeColor);

let selectCamera = -1;

const updateCameraToBone = (camera) => {
    if (selectCamera == camera)
        return;
        
    selectCamera = camera;
    global.updateCameraToBone (camera, shopData.ped);
}

gm.events.add('client.clothes.updateCameraToBone', updateCameraToBone);

const setDecoration = (tId, slots, collection, overlay) => {
    global.localplayer.cloneToTarget(shopData.ped.handle);

    //

    /*slots = JSON.parse (slots)

    let playerTattoos = shopData.tattoos;
    if (playerTattoos[tId]) {
        for (let x = 0; x < playerTattoos[tId].length; x++) { // Очищаем ненужные татушки
            for (let i = 0; i < slots.length; i++) {
                if (playerTattoos[tId][x] && playerTattoos[tId][x].Slots.indexOf(slots[i]) != -1) {
                    playerTattoos[tId][x] = null;
                    break;
                }
            }
        }
    }

    for (let x = 0; x < 6; x++) // Восстанавливаем старые татуировки игрока, кроме тех, которые занимают очищенные слоты
        if (playerTattoos[x] != null)
            for (let i = 0; i < playerTattoos[x].length; i++)
                if (playerTattoos[x][i] != null)
                    global.localplayer.setDecoration(mp.game.joaat(playerTattoos[x][i].Dictionary), mp.game.joaat(playerTattoos[x][i].Hash));

    */
    shopData.ped.setDecoration(mp.game.joaat(collection), mp.game.joaat(overlay));
}

gm.events.add('client.clothes.setDecoration', setDecoration);

///

const GetTorso = () => {
    const drawable = global.localplayer.getDrawableVariation(3);
    const texture = global.localplayer.getTextureVariation(3);

    mp.gui.emmit(`window.events.callEvent("cef.clothes.getTorso", ${drawable}, ${texture})`);
}

gm.events.add('client.clothes.getTorso', GetTorso);

//

const GetTop = () => {
    const drawable = global.localplayer.getDrawableVariation(11);

    mp.gui.emmit(`window.events.callEvent("cef.clothes.getTop", ${drawable})`);
}

gm.events.add('client.clothes.getTop', GetTop);



///
const getPedMakeUpRgbColor = (colorIndex) => {
	const color = { r: [0], g: [0], b: [0] };

	Natives._GET_PED_MAKEUP_RGB_COLOR (colorIndex, color.r, color.g, color.b);
	
	return { r: color.r[0], g: color.g[0], b: color.b[0], a: 1 };
}

const getPedHairRgbColor = (colorIndex) => {
	const color = { r: [0], g: [0], b: [0] };

	Natives._GET_PED_HAIR_RGB_COLOR (colorIndex, color.r, color.g, color.b);
	
	return { r: color.r[0], g: color.g[0], b: color.b[0], a: 1 };
}

const GetColor = (isHair = false) => {
    let returnRgb = [];

    if (isHair) {
        for (let index = 0; index < Natives._GET_NUM_HAIR_COLORS (); index++) {		
            let color = getPedHairRgbColor(index);
            color.gtaid = index;
            returnRgb.push (color);
        }
    } else {
        returnRgb.push ({ r: 0, g: 0, b: 0, a: 0, gtaid: 0 });

        let i = 0;
        for (let index = 0; index < Natives._GET_NUM_MAKEUP_COLORS (); index++) {
            if (index == 55)//Багнаный цвет
                continue;

            i = index;
            if (index > 55)
                i = index - 1;

            let color = getPedMakeUpRgbColor(index);
            color.gtaid = i + 1;
            returnRgb.push (color);
        }
    }

    mp.gui.emmit(`window.events.callEvent("cef.clothes.getColor", '${JSON.stringify (returnRgb)}')`);
}

gm.events.add('client.clothes.getColor', GetColor);



/////////////

const OnClothesBuy = (dictionary, id, texture) => {
    mp.events.callRemote('server.clothes.buy', dictionary, id, texture, shopData.isDonate);
}

gm.events.add('client.clothes.buy', OnClothesBuy);

/////////////

const OnBarberBuy = (dictionary, id, color, colorHighlight, opacity) => {
    mp.events.callRemote('server.barber.buy', dictionary, id, color, colorHighlight, opacity, shopData.isDonate);
}

gm.events.add('client.barber.buy', OnBarberBuy);

/////////////

const OnTattooBuy = (dictionary, id) => {
    mp.events.callRemote('server.tattoo.buy', dictionary, id, shopData.isDonate);
}

gm.events.add('client.tattoo.buy', OnTattooBuy);