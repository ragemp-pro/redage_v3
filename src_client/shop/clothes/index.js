let clothes = {
    data: {},
    type: 0,
    Variation: 0,
    Textures: 0,
    isDonate: 0,
    isGloves: 0,
    price: 0
};

const TypeToId = {
    "Hat": {
        title: translateText("Головной убор"),
        icon: "ic-st-c-hat",
        index: 0,
        camToBone: "hair",
        isProp: true,
        boneId: 0
    },
    "Tops": {
        title: translateText("Верхняя одежда"),
        icon: "ic-st-c-coat",
        index: 1,
        camToBone: "tops",
        isProp: false,
        boneId: 11
    },
    "Undershort": {
        title: translateText("Рубашка"),
        icon: "ic-st-c-shirt",
        index: 2,
        camToBone: "tops",
        isProp: false,
        boneId: 11
    },
    "Legs": {
        title: translateText("Штаны"),
        icon: "ic-st-c-pants",
        index: 3,
        camToBone: "legs",
        isProp: false,
        boneId: 4
    },
    "Shoes": {
        title: translateText("Обувь"),
        icon: "ic-st-c-shoe",
        index: 4,
        camToBone: "shoes",
        isProp: false,
        boneId: 6
    },
    "Watches": {
        title: translateText("Часы"),
        icon: "ic-st-c-watch",
        index: 5,
        camToBone: "watches",
        isProp: true,
        boneId: 6
    },
    "Glasses": {
        title: translateText("Очки"),
        icon: "ic-st-c-glasses",
        index: 6,
        camToBone: "glasses",
        isProp: true,
        boneId: 1
    },
    "Accessories": {
        title: translateText("Аксессуары"),
        icon: "ic-st-c-necklace",
        index: 7,
        camToBone: "tops",
        isProp: false,
        boneId: 7
    },
    "Ears": {
        title: translateText("Наушники"),
        icon: "ic-st-c-ears",
        index: 8,
        camToBone: "glasses",
        isProp: true,
        boneId: 2
    },
    "Gloves": {
        title: translateText("Перчатки"),
        icon: "ic-st-c-glove",
    },
    "Mask": {
        title: translateText("Маски"),
        icon: "inv-item-mask",
        index: 13,
        camToBone: "glasses",
        isProp: false,
        boneId: 1
    },
    "Bug": {
        title: translateText("Сумки"),
        icon: "inv-item-backpack",
        index: 14,
        camToBone: "tops",
        isProp: false,
        boneId: 5
    },
}

const IndexToType = [
    "Hat",
    "Tops",
    "Undershort",
    "Legs",
    "Shoes",
    "Gloves",
    "Watches",
    "Glasses",
    "Accessories",
    "Ears",
    "Mask",
    "Bug",
]

//types: styles, colors, prices
function setClothes(type, jsonstr) {
    mp.gui.emmit(`window.clothes.setVariable("${type}",'${jsonstr}');`);

    if (type == 'sVariation') mp.gui.emmit(`window.clothes.setVariable("activeTextures", 0);`);
    else if (type == 'sCatigories') mp.gui.emmit(`window.clothes.setVariable("activeVariation", 0);`);
}

gm.events.add('openClothes', async (price, isDonate, isGloves, json) => {
    if (global.menuCheck()) return;
    clothes.price = price;
    clothes.isDonate = isDonate;
    clothes.isGloves = isGloves;
    clothes.data = JSON.parse (json);
    global.createCamera ("char", global.localplayer);

    clothes.type = -1;

    let ListData = [];

    IndexToType.forEach((name) => {
        if ((name === "Gloves" && isGloves) || (name !== "Gloves" && TypeToId [name] && clothes.data [TypeToId [name].index] && clothes.data [TypeToId [name].index].length > 0)) {
            if (clothes.type === -1)
                clothes.type = name;
            ListData.push({
                title: TypeToId [name].title,
                icon: TypeToId [name].icon,
                type: name
            });
        }
    })
    if (clothes.type != -1) {
        global.menuOpen();
        mp.gui.emmit(`window.router.setView("BusinessClothes", ${isDonate});`);
        await global.wait(50);
        mp.gui.emmit(`window.clothes.updateMenu('${JSON.stringify (ListData)}');`);
        //getVariation (gender, "Hats")
        getTexturesName(TypeToId[clothes.type].index, getVariation (TypeToId[clothes.type].index));

        clearClothes ();

        UpdateComponent ();
        global.updateBoneToPos (TypeToId[clothes.type].camToBone);
    } else {        
        mp.events.call('closeClothes');
    }
});

gm.events.add('closeClothes', () => {
	if(new Date().getTime() - global.lastCheck < 50) return; 
	global.lastCheck = new Date().getTime();
    global.menuClose();
    mp.gui.emmit('window.router.setHud();');
	global.cameraManager.stopCamera ();

    mp.events.callRemote('cancelClothes');
    clearClothes();
    clothes = {
        data: {},
        type: 0,
        Variation: 0,
        Textures: 0,
        isDonate: 0,
        price: 0,
    }
});


const getTexturesName = (component, _id) => {
    try {
        let data = [];

        clothes.Textures = -1;
        const cData = clothes.data[component][_id];
        
        Object.values (cData[4]).forEach((item, index) => {
            if (item.Name !== "NO_LABEL"/* && mp.game.ui.getLabelText(item.Name).length > 1*/) {                
                if (clothes.Textures == -1) clothes.Textures = index;
                data.push({
                    Name: getIndexToTextureName (cData[3], index, cData[0]),
                    Index: index
                })
            }
        })
        setClothes("sVariation", JSON.stringify(data));
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/clothes/index", "getTexturesName", e.toString());
    }
}

const getIndexToTextureName = (TName, index, id) => {
    try {
        if (TName.length > 1) {
            const name = mp.game.ui.getLabelText(`${TName}${index}`);
            if (name && name.toLowerCase() !== "null" && name.length > 3)
                return global.escapeHtml (name) + `[${id}]`
        }
        return id;
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/clothes/index", "getIndexToTextureName", e.toString());
        return id;
    }
}

const getVariation = (component) => {
    try {
        let firstItem = -1;
        let data = [];
        clothes.data[component].forEach((item, index) => {
            if (firstItem == -1) {
                firstItem = index;
                clothes.Variation = index;
                clothes.Textures = item[4][0];
            }
            data.push({
                ID: index,
                Price: item[5],
                Name: getIndexToTextureName (item[3], item[4][0], item[0])
            })
        })
        setClothes("sCatigories", JSON.stringify(data));
        return firstItem;
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/clothes/index", "getVariation", e.toString());
    }
}

const UpdateComponent = () => {
    try {
        const typeData = TypeToId [clothes.type];
        if (clothes.type !== "Gloves" && typeData) {
            const clothesData = clothes.data[typeData.index][clothes.Variation];
            const texture = clothesData[4][clothes.Textures] ? clothesData[4][clothes.Textures] : clothes.Textures;
            if (typeData.isProp)
                global.localplayer.setPropIndex(typeData.boneId,
                    clothesData[1], 
                    texture,
                    true);
            else {
                global.localplayer.setComponentVariation(typeData.boneId,
                    clothesData[1], 
                    texture,
                    0);
                if (typeData.boneId === 11)
                    global.localplayer.setComponentVariation(3, clothesData[2], 0, 0);
            }
        } else if (clothes.type === "Gloves") {
            const gender = (global.GetGender (global.localplayer)) ? 1 : 0;
            global.localplayer.setComponentVariation(3, correctGloves[gender][clothes.Variation][15], clothes.Textures, 0);        
        }
            
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/clothes/index", "UpdateComponent", e.toString());
    }
}

gm.events.add('clothes', (act, value) => {
    try {
        switch (act) {
            case "style":                
                if (clothes.type === "Gloves") {
                    const gender = (global.GetGender (global.localplayer)) ? 1 : 0;
                    let colors = [];
                    clothesGloves[gender][value].Colors.forEach((item, index) => {
                        colors.push({
                            Name: item,
                            Index: index
                        })
                    })
                    setClothes("sVariation", JSON.stringify(colors));
                    clothes.Variation = clothesGloves[gender][value].Variation;
                } else {
                    clothes.Variation = value;
                    if (clothes.type) {
                        getTexturesName (TypeToId[clothes.type].index, value);                        
                    }
                }
                UpdateComponent ();
                break;
            case "color":
                clothes.Textures = value;
                UpdateComponent ();
                break;
            case "cat": //category
                clothes.type = value;
                if (clothes.type === "Gloves") {
                    if (!clothes.isGloves)
                        return;
                    const gender = (global.GetGender (global.localplayer)) ? 1 : 0;
                    clothes.Variation = clothesGloves[gender][0].Variation;
                    let data = [];
                    clothesGloves[gender].forEach((item, index) => {
            
                        data.push({
                            ID: index,
                            Price: Math.round (item.Price / 100 * clothes.price),
                            Name: "#"+item.Variation
                        })
                    })
                    setClothes("sCatigories", JSON.stringify(data));
                    data = [];
                    clothesGloves[gender][0].Colors.forEach((item, index) => {

                        data.push({
                            Name: index,
                            Index: index
                        })
                    })
                    setClothes("sVariation", JSON.stringify(data));
                    global.updateBoneToPos ('tops');
                } else if (TypeToId[clothes.type]) {
                    getTexturesName(TypeToId[clothes.type].index, getVariation (TypeToId[clothes.type].index));
                    global.updateBoneToPos (TypeToId[clothes.type].camToBone);
                }
                UpdateComponent ();
                break;
        }
        
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/clothes/index", "clothes", e.toString());
    }
});

gm.events.add('buyClothes', () => {
	if(new Date().getTime() - global.lastCheck < 50) return; 
	global.lastCheck = new Date().getTime();
    const typeData = TypeToId [clothes.type];
    if (clothes.type !== "Gloves" && typeData) {
        const clothesData = clothes.data[typeData.index][clothes.Variation];
        const texture = clothesData[4][clothes.Textures] ? clothesData[4][clothes.Textures] : clothes.Textures;
        mp.events.callRemote('buyClothes', clothes.type, clothesData[0], texture, clothes.isDonate);
    } else if (clothes.type === "Gloves") {
        mp.events.callRemote('buyClothes', clothes.type, clothes.Variation, clothes.Textures, clothes.isDonate);      
    }
});

function clearClothes() {
    const gender = (global.GetGender (global.localplayer)) ? 1 : 0;

    global.localplayer.clearProp(0);
    global.localplayer.clearProp(1);
    global.localplayer.clearProp(2);
    global.localplayer.clearProp(6);
    global.localplayer.clearProp(7);

    global.localplayer.setComponentVariation(1, clothesEmpty[gender][1], 0, 0);
    global.localplayer.setComponentVariation(3, clothesEmpty[gender][3], 0, 0);
    global.localplayer.setComponentVariation(4, clothesEmpty[gender][4], 0, 0);
    global.localplayer.setComponentVariation(5, clothesEmpty[gender][5], 0, 0);
    global.localplayer.setComponentVariation(6, clothesEmpty[gender][6], 0, 0);
    global.localplayer.setComponentVariation(7, clothesEmpty[gender][7], 0, 0);
    global.localplayer.setComponentVariation(8, clothesEmpty[gender][8], 0, 0);
    global.localplayer.setComponentVariation(9, clothesEmpty[gender][9], 0, 0);
    global.localplayer.setComponentVariation(10, clothesEmpty[gender][10], 0, 0);
    global.localplayer.setComponentVariation(11, clothesEmpty[gender][11], 0, 0);
}

let UpdateVariationData = {}
gm.events.add('setComponentVariation', async (componentId, drawableId, textureId) => {
    try {
        if (!UpdateVariationData [componentId]) {
            const oldDrawable = global.localplayer.getDrawableVariation(componentId);
            const oldTexture = global.localplayer.getTextureVariation(componentId);
        
            UpdateVariationData [componentId] = [Number (componentId), Number (oldDrawable), Number (oldTexture), (new Date().getTime() + (1000 * 10))];
        } else {
            UpdateVariationData [componentId][3] = (new Date().getTime() + (1000 * 10));
        }
        global.localplayer.setComponentVariation(Number (componentId), Number (drawableId), Number (textureId), 0);     
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/clothes/index", "setComponentVariation", e.toString());
    }
    
});

let UpdatePropData = {}
gm.events.add('setPropIndex', async (componentId, drawableId, TextureId) => {
    try {
        if (!UpdatePropData [componentId]) {
            const oldDrawable = global.localplayer.getNumberOfPropDrawableVariations(componentId);
            const oldTexture = global.localplayer.getNumberOfPropTextureVariations(componentId);
        
            UpdatePropData [componentId] = [Number (componentId), Number (oldDrawable), Number (oldTexture), (new Date().getTime() + (1000 * 10))];
        } else {
            UpdatePropData [componentId][3] = (new Date().getTime() + (1000 * 10));
        }       
    
        global.localplayer.setPropIndex(Number (componentId), Number (drawableId), Number (TextureId), true);       
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/clothes/index", "setPropIndex", e.toString());
    }
});

gm.events.add(global.renderName ["1s"], () => {
	try 
	{
		for (key in UpdateVariationData) {
            if (UpdateVariationData [key]) {
                const variationData = UpdateVariationData [key];
                if (new Date().getTime() > variationData [3]) {
                    delete UpdateVariationData [key];

                    global.localplayer.setComponentVariation(variationData [0], variationData [1], variationData [2], 0);
                }
            }
        }

		for (key in UpdatePropData) {
            if (UpdatePropData [key]) {
                const propData = UpdatePropData [key];
                if (new Date().getTime() > propData [3]) {
                    delete UpdatePropData [key];

                    global.localplayer.setPropIndex(propData [0], propData [1], propData [2], true);
                }
            }
        }
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "world/environment", global.renderName ["1s"], e.toString());
	}
})