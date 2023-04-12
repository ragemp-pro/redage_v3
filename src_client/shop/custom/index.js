
//import places from './places'
import { ModIndexs, Components, ModUses, ComponentsPrice, blockedModLabels, getStockValue, getModLabel, ComponentsName, OtherComponents, OtherCategory, PlacePos, vehicleComponentToUse } from './data'
let selectCategory = false;
let defaultComponents = {};
let eventId = 0;
let selectData = {
    red: 255,
    green: 255,
    blue: 255,
    otherIndex: 0,
    index: 0
}
let pricePercent = 0;
let priceVehicle = 0;

/*
let LeaveShape = false;
PlacePos.forEach((place, index) => {
    const colshape = mp.colshapes.newSphere(place.Position.x, place.Position.y, place.Position.z, 2);
    colshape.customIndex = index;  
});

gm.events.add({
    'playerEnterColshape': (shape)=> {
        if(shape && shape.customIndex !== undefined && global.localplayer.vehicle && !LeaveShape) {
            const vehicle = global.localplayer.vehicle;
            //const vehModel = vehicle.model;

            //if (!mp.game.vehicle.isThisModelABike(vehModel) && !mp.game.vehicle.isThisModelACar(vehModel)) return;
            mp.events.callRemote('server.ls_customs.start', false, shape.customIndex);
        }
    },
    'playerExitColshape': (shape)=> {
        if(shape && shape.customIndex !== undefined && global.localplayer.vehicle) {
            LeaveShape = false;
        }
    },
});*/

//gm.events.add('vehicle.teleport', (vehicleId) => {
//    const vehicle = mp.vehicles.atRemoteId(vehicleId);
//    if (!vehicle || !mp.vehicles.exists(vehicle)) return;
//    vehicle.setOnGroundProperly();
//    vehicle.setForwardSpeed(0);
//});


gm.events.add('client.custom.open', async (pPercent, pVehicle, components, _eventId = 0) => {
    try
    {
        await global.IsSeatVehicle ();

        if (!global.localplayer.vehicle)
            return;
        global.FadeScreen (true, 50);
        pricePercent = pPercent / 100;
        priceVehicle = pVehicle / 100;
        defaultComponents = JSON.parse (components);
        eventId = _eventId;
        //LeaveShape = true;
        const vehicle = global.localplayer.vehicle;
        vehicle.setOnGroundProperly();
        vehicle.setForwardSpeed(0);
        global.menuOpen();
        mp.gui.emmit(`window.router.setView("VehicleLsCustom");`);
        await global.wait(50); 
        gm.discord(translateText("Подбирает тюнинг на авто"));
        OpenCustom ();
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/custom/index", "client.custom.open", e.toString());
    }
});

gm.events.add('client.custom.category', (category) => {
    try
    {
        if (!global.localplayer.vehicle)
            return;
        if (category === "back") {
            selectData.otherIndex = 0;
            selectData = {
                red: 0,
                green: 0,
                blue: 0,
                otherIndex: 0,
                index: 0
            }
            OpenCustom ();
        }
        else if (selectCategory === "FrontWheels") OnSelectCategoryToWheels (category);
        else if (selectCategory === "Horn") OnSelectCategoryToHorn (category);
        else OnSelectCategory (category);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/custom/index", "client.custom.category", e.toString());
    }
});

gm.events.add('client.custom.item', (index) => {
    OnSetTuneComponent (index);
});

gm.events.add('client.custom.coloritem', (index) => {
    OnSetTuneComponentColorToIndex (index);
});


gm.events.add('client.custom.color', (red, green, blue) => {
    OnSetTuneComponentColor (red, green, blue);
});

gm.events.add('client.custom.updatecomponents', (components) => {
    defaultComponents = JSON.parse (components);
});



gm.events.add('client.custom.buy', () => {
    try
    {
        if (!global.localplayer.vehicle || selectCategory == false)
            return;
        
        if (selectCategory === "FrontWheels" && defaultComponents ["WheelsType"] == selectData.otherIndex && defaultComponents ["FrontWheels"] !== undefined && defaultComponents ["FrontWheels"] === selectData.index) return mp.events.call('notify', 1, 9, translateText("У Вас уже установлена данная модификация"), 3000);
        else if (selectCategory !== "FrontWheels" && 
            selectCategory !== "ColorAdditional" && 
            selectCategory !== "Headlights" && 
            selectCategory !== "Xenon" && 
            selectCategory !== "Color1" && 
            selectCategory !== "Color2" && 
            defaultComponents [selectCategory] !== undefined && defaultComponents [selectCategory] === selectData.index) return mp.events.call('notify', 1, 9, translateText("У Вас уже установлена данная модификация"), 3000);
        let text;
        if(selectCategory == "Color1" || selectCategory == "Color2") text = translateText("Вы действительно хотите покрасить машину в данный цвет?");
        else text = translateText("Вы действительно хотите установить данную модификацию?");
        mp.events.call('openDialog', 'tuningbuy', text);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/custom/index", "client.custom.buy", e.toString());
    }
});

const eventList = [
    'server.custom.buy',
    'server.custom.orgBuy',
    'server.custom.gosBuy',
];

gm.events.add('client.custom.sbuy', (state) => {
    try
    {
        if (!global.localplayer.vehicle || selectCategory == false)
            return;
        
        if (state)
            mp.events.callRemote(eventList [eventId], selectCategory, selectData.index, selectData.otherIndex, selectData.red, selectData.green, selectData.blue);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/custom/index", "client.custom.sbuy", e.toString());
    }
});

mp.keys.bind(global.Keys.VK_ESCAPE, false, () => { //Space
	lscutomsExit ();
});

const lscutomsExit = () => {
    if (!pricePercent && !priceVehicle) return;
    
    if (selectCategory === "FrontWheels" || selectCategory === "Horn") {
        selectData.otherIndex = 0;
        selectData = {
            red: 0,
            green: 0,
            blue: 0,
            otherIndex: 0,
            index: 0
        }
        OpenCustom ();
        return;
    } else Exit ();
}
gm.events.add('client.custom.exit', () => {
    Exit ();
});

const Exit = () => {
    mp.events.callRemote('server.custom.exit', eventId);
    setDefaultComponent ("Headlights");
    setDefaultComponent ("Xenon");
    selectCategory = false;
    defaultComponents = {};
    eventId = 0;
    selectData = {
        red: 0,
        green: 0,
        blue: 0,
        otherIndex: 0,
        index: 0
    }
    pricePercent = 0;
    priceVehicle = 0;
    mp.gui.emmit(`window.router.setHud();`);
    global.cameraManager.stopCamera ();
    global.menuClose();
}

const OpenCustom = () => {
    selectCategory = false;
    let dunamicMod = [];
    const vehicle = global.localplayer.vehicle;
    Components.forEach((component, index) => {
        const modType = ModIndexs [component.category];

        if (vehicleComponentToUse (vehicle.model, modType ? modType : component.category)) {
            if (modType !== undefined && ModUses.includes(modType) && vehicle.getNumMods(modType)) {
                dunamicMod.push(component);
            } else if (modType !== undefined && !ModUses.includes(modType)) {
                dunamicMod.push(component);
            } else if (modType === undefined) {
                dunamicMod.push(component);
            }
        }
    });
    mp.gui.emmit(`window.events.callEvent("cef.custom.categories", '${JSON.stringify (dunamicMod)}');`);
    UpdateMaxVehicleStats ();

    setTimeout(() => {        
	    global.createCamera ("autoshop", vehicle);
        global.FadeScreen (false, 50);
    }, 500)
}
const UpdateVehicleStats = () => {
    const vehicle = global.localplayer.vehicle;
    mp.gui.emmit(
        `window.events.callEvent("cef.custom.vehicleStats", ${Math.round (mp.game.vehicle.getVehicleModelMaxSpeed(vehicle.model) / 1.2)}, ${vehicle.getMaxBraking() * 100}, ${vehicle.getAcceleration() * 100}, ${vehicle.getMaxTraction() * 10});`
    );
}

const UpdateMaxVehicleStats = () => {
    const vehicle = global.localplayer.vehicle;
    vehicle.setMod(ModIndexs["Engine"], 3);
    vehicle.setMod(ModIndexs["Turbo"], 0);
    vehicle.setMod(ModIndexs["Transmission"], 2);
    vehicle.setMod(ModIndexs["Suspension"], 3);
    vehicle.setMod(ModIndexs["Brakes"], 2);

    mp.gui.emmit(
        `window.events.callEvent("cef.custom.vehicleMaxStats", ${Math.round (mp.game.vehicle.getVehicleModelMaxSpeed(vehicle.model) / 1.2)}, ${vehicle.getMaxBraking() * 100}, ${vehicle.getAcceleration() * 100}, ${vehicle.getMaxTraction() * 10});`
    );
    setDefaultComponent ("Engine");
    setDefaultComponent ("Turbo");
    setDefaultComponent ("Transmission");
    setDefaultComponent ("Suspension");
    setDefaultComponent ("Brakes");
    UpdateVehicleStats ();
}

//Категории
const OnSelectCategoryToWheels = (index) => {
    const vehicle = global.localplayer.vehicle;
    selectData.otherIndex = index;
    vehicle.setWheelType(index);
    OpenComponentToMod (selectCategory);
    mp.gui.emmit(`window.events.callEvent("cef.custom.color", true, 1);`);
}

const OnSelectCategoryToHorn = (index) => {
    let json = [];
    selectData.otherIndex = index;
    OtherCategory ("Horn", index).forEach((item, index) => {
        json.push({
            name: item.name,
            index: index,
            price: GetPrice ("Horn", item.index)
        })
    });
    if (json && json.length >= 1) {
        mp.gui.emmit(`window.events.callEvent("cef.custom.lists", '${JSON.stringify (json)}')`);
        return;
    }
    OnSetTuneComponent (0);
}

const OnSelectCategory = (category) => {
    if (!global.localplayer.vehicle)
        return;

    setDefaultComponent (selectCategory);
    if (selectCategory === category) {
        selectCategory = false;
        return;
    }

    selectCategory = category;
    selectData = {
        red: 0,
        green: 0,
        blue: 0,
        otherIndex: 0,
        index: 0
    }
    const modType = ModIndexs [category];
    const vehicle = global.localplayer.vehicle;  

    if (modType !== undefined && ModUses.includes(modType))
        OpenComponentToMod (category);        
    else if (category == "Color1" || category == "Color2") {
        mp.gui.emmit(`window.events.callEvent("cef.custom.color", true, 0)`);
        mp.gui.emmit(`window.events.callEvent("cef.custom.lists", '${JSON.stringify ([{
            name: translateText("Покрасить"),
            index: 0,
            price: GetPrice (category)
        }])}')`);
    } else if (category == "Cover")
        OpenComponentToCover ();
    else if (category == "Xenon")
        OpenComponentToXenon ();
    else if (category == "ColorAdditional") {
        mp.gui.emmit(`window.events.callEvent("cef.custom.color", true, 1)`);
        mp.gui.emmit(`window.events.callEvent("cef.custom.lists", '${JSON.stringify ([{
            name: translateText("Купить"),
            index: 0,
            price: GetPrice ("ColorAdditional")
        }])}')`);
    } else if (category == "Headlights") {
        vehicle.setLights(2);
        mp.gui.emmit(`window.events.callEvent("cef.custom.color", true, 2)`);

        mp.gui.emmit(`window.events.callEvent("cef.custom.lists", '${JSON.stringify ([{
                name: translateText("Купить"),
                index: 0,
                price: GetPrice ("Headlights")
            }])}')`);
    } else if (category == "FrontWheels")
        OpenComponentToWheels ();
    else if (category == "Horn")
        OpenComponentToHorns ();
    else if (category == "Turbo" || category == "Engine" || category == "Brakes" || category == "Transmission" || category == "Suspension")
        OpenComponentToOther (category);
    else if (category == "WindowTint")
        OpenComponentToWindowTint ();
    else if (category == "NumberPlate")
        OpenComponentToNumberPlate ();
}

const OpenComponentToWheels = () => {
    let dunamicMod = [];
    selectData = {
        red: 0,
        green: 0,
        blue: 0,
        otherIndex: 0,
        index: 0
    }
    OtherComponents (ModIndexs.FrontWheels).forEach((component, index) => {
        dunamicMod.push (component);
    });
    mp.gui.emmit(`window.events.callEvent("cef.custom.categories", '${JSON.stringify (dunamicMod)}');`);
}

const OpenComponentToHorns = () => {
    let dunamicMod = [];
    selectData = {
        red: 0,
        green: 0,
        blue: 0,
        otherIndex: 0,
        index: 0
    }
    OtherComponents (ModIndexs.Horn).forEach((component, index) => {
        dunamicMod.push (component);
    });
    mp.gui.emmit(`window.events.callEvent("cef.custom.categories", '${JSON.stringify (dunamicMod)}');`);
}

const OpenComponentToCover = () => {
    //const vehicle = global.localplayer.vehicle;
    //vehicle.setColours(0, 0);
    //vehicle.setCustomPrimaryColour(0, 0, 0);
    //vehicle.setCustomSecondaryColour(0, 0, 0);
    //selectData.red = 1;
    let json = [];
    OtherCategory ("Cover").forEach((item, index) => {
        json.push({
            name: item.name,
            index: item.index,
            price: GetPrice ("Cover", item.index)
        })
    });
    mp.gui.emmit(`window.events.callEvent("cef.custom.lists", '${JSON.stringify (json)}')`);
}

const OpenComponentToOther = (category) => {
    let json = [];
    OtherCategory (category).forEach((item, index) => {
        json.push({
            name: item.name,
            index: item.index,
            price: GetPrice (category, item.index)
        })
    });
    mp.gui.emmit(`window.events.callEvent("cef.custom.lists", '${JSON.stringify (json)}')`);
}

const OpenComponentToWindowTint = () => {
    let json = [];
    const ModToIndex = {
        '-1': -1,
        '3': 1,
        '2': 2,
        '1': 3,
    }
    OtherCategory ("WindowTint").forEach((item, index) => {
        json.push({
            name: item.name,
            index: item.index,
            price: GetPrice ("WindowTint", ModToIndex [item.index])
        })
    });
    mp.gui.emmit(`window.events.callEvent("cef.custom.lists", '${JSON.stringify (json)}')`);
}

const OpenComponentToNumberPlate = () => {
    let json = [];
    OtherCategory ("NumberPlate").forEach((item, index) => {
        json.push({
            name: item.name,
            index: item.index,
            price: GetPrice ("NumberPlate", item.index)
        })
    });
    mp.gui.emmit(`window.events.callEvent("cef.custom.lists", '${JSON.stringify (json)}')`);
}
const OpenComponentToXenon = () => {
    const vehicle = global.localplayer.vehicle;    
    vehicle.setLights(2);

    let json = [];
    OtherCategory ("Xenon").forEach((item, index) => {
        json.push({
            name: item.name,
            index: item.index,
            price: GetPrice ("Xenon", item.index)
        })
    });
    mp.gui.emmit(`window.events.callEvent("cef.custom.color", true, 0)`);
    mp.gui.emmit(`window.events.callEvent("cef.custom.lists", '${JSON.stringify (json)}')`);
}

let OpenComponentToModFix = 0; 

const OpenComponentToMod = (category) => {
    try {
        OpenComponentToModFix = 0; 
        const vehicle = global.localplayer.vehicle;
        const modType = ModIndexs [category];
        OpenComponentToModFix = 1; 
        let json = [];
        const stockValue = getStockValue(modType);
        OpenComponentToModFix = 2; 
        if(typeof stockValue === "string") {
            json.push({
                name: global.escapeHtml (mp.game.ui.getLabelText(stockValue)),
                index: -1,
                price: GetPrice (category, -1)
            })
        }
        OpenComponentToModFix = 3; 
    
        const count = vehicle.getNumMods(modType);
        for (let i = 0; i < count; i++) {
            let label = Natives.GET_MOD_TEXT_LABEL (vehicle.handle, modType, i);//vehicle.getModTextLabel(modType, i);
            if (blockedModLabels.indexOf(label) >= 0)
                continue;
    
            if (vehicle.model == mp.game.joaat("vapidse") && modType == 48)
                continue;
    
            if (typeof label !== "string" || label.length < 1) {
                label = getModLabel(modType, i);
            }
            const name = mp.game.ui.getLabelText(label);
            json.push({
                name: name.toLowerCase() !== "null" ? global.escapeHtml (name) : `${ComponentsName [category]}${i}`,
                index: i,
                price: GetPrice (category, i)
            });
        }
        OpenComponentToModFix = 4; 
        if (json && json.length >= 1) {
            mp.gui.emmit(`window.events.callEvent("cef.custom.lists", '${JSON.stringify (json)}')`);
            return;
        }
        OnSetTuneComponent (0);
    }
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "shop/custom/index", "OpenComponentToMod - " + OpenComponentToModFix, e.toString());
	}
}

const OnSetTuneComponentColor = (red, green, blue) => {
    if (!global.localplayer.vehicle || selectCategory == false)
        return;
    selectData.red = red;
    selectData.green = green;
    selectData.blue = blue;
    const vehicle = global.localplayer.vehicle;
    if (selectCategory == "Color1")
        vehicle.setCustomPrimaryColour(red, green, blue);
    else if (selectCategory == "Color2")
        vehicle.setCustomSecondaryColour(red, green, blue);
    else if (selectCategory == "Xenon")
        vehicle.setNeonLightsColour(red, green, blue);
}

const OnSetTuneComponentColorToIndex = (index) => {
    if (!global.localplayer.vehicle || selectCategory == false)
        return;
    const vehicle = global.localplayer.vehicle;
    selectData.red = index;

    if (selectCategory == "Headlights") {
        vehicle.toggleMod(22, true);
        SetVehicleLightColor(vehicle, index - 1);
    } else if (selectCategory == "ColorAdditional")
        vehicle.setExtraColours(index, defaultComponents ["FrontWheels"] ? defaultComponents ["FrontWheels"] : 0);
    else if (selectCategory == "FrontWheels")
        vehicle.setExtraColours(defaultComponents ["ColorAdditional"] ? defaultComponents ["ColorAdditional"] : 0, index);
}

const OnSetTuneComponent = (index) => {
    if (!global.localplayer.vehicle || selectCategory == false)
        return;
    const vehicle = global.localplayer.vehicle;
    selectData.index = index;
    if (selectCategory == "WindowTint")
        vehicle.setWindowTint(index);
    else if (selectCategory == "NumberPlate")
        vehicle.setNumberPlateTextIndex(index);
    else if (selectCategory == "Horn") {
        const horn = OtherCategory ("Horn", selectData.otherIndex)[index];
        vehicle.setMod(ModIndexs ["Horn"], horn.index);
        selectData.index = horn.index;
        vehicle.startHorn(horn.duration, mp.game.joaat("NORMAL"), false);
    } else if (selectCategory == "Cover")
        vehicle.setModColor1(parseInt(index), 1, 0);
    else if (selectCategory == "Xenon") {
        setNeonLight (vehicle, getNeonValuesByIndex(index));
    } else if (ModIndexs[selectCategory] !== undefined) {
        vehicle.setMod(ModIndexs[selectCategory], index);
        if (selectCategory == "Wings") vehicle.setMod(ModIndexs["RightWings"], index);
    }
    UpdateVehicleStats ();
}

global.getNeonValuesByIndex = (index) => {
	switch (index) {
		case 0: // None
			return [];
		case 1: // Front
			return [ 2 ];
		case 2: // Back
			return [ 3 ];
		case 3: // Sides
			return [ 0, 1 ];
		case 4: // Front + Back
			return [ 2, 3 ];
		case 5: // Front + Sides
			return [ 0, 1, 2 ];
		case 6: // Back + Sides
			return [ 0, 1, 3 ];
		case 7: // All
			return [ 0, 1, 2, 3 ];
		default:
			return [];
	}
}

global.setNeonLight = (vehicle, enabledValues) => {
	try 
	{
		if (vehicle && mp.vehicles.exists(vehicle)) {
			for (let i = 0; i < 4; i++) {
				vehicle.setNeonLightEnabled(i, enabledValues.indexOf(i) >= 0);
			}
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "shop/custom/index", "setNeonLight", e.toString());
	}
}

const setDefaultComponent = (category) => {
    if (!global.localplayer.vehicle || category == false)
        return;
    const vehicle = global.localplayer.vehicle;

    if (category == "WindowTint")
        vehicle.setWindowTint(defaultComponents ["WindowTint"] ? defaultComponents ["WindowTint"] : 0);
    else if (category == "NumberPlate")
        vehicle.setNumberPlateTextIndex(defaultComponents ["NumberPlate"] ? defaultComponents ["NumberPlate"] : 0);
    else if (category == "Cover")
        vehicle.setModColor1(parseInt(defaultComponents ["Cover"] ? defaultComponents ["Cover"] : 0), 1, 0);
    else if (category == "Headlights") {
        vehicle.setLights(1);
        SetVehicleLightColor(vehicle, defaultComponents ["Headlights"] ? defaultComponents ["Headlights"] : -1);
    } else if (category == "Color1" || category == "Color2") {             
        if (defaultComponents.PrimModColor === -1 && defaultComponents.SecModColor === -1) {
            vehicle.setCustomPrimaryColour(defaultComponents.PrimColor.Red, defaultComponents.PrimColor.Green, defaultComponents.PrimColor.Blue);
            vehicle.setCustomSecondaryColour(defaultComponents.SecColor.Red, defaultComponents.SecColor.Green, defaultComponents.SecColor.Blue);
        } else vehicle.setColours(defaultComponents.PrimModColor, defaultComponents.SecModColor);
			
    } else if (category == "Xenon") {
        
        vehicle.setLights(1);
        setNeonLight (vehicle, getNeonValuesByIndex(defaultComponents ["NeonIndex"] ? defaultComponents ["NeonIndex"] : 0));
        if (defaultComponents ["NeonColor"].Alpha != 0) {
            vehicle.setNeonLightsColour(defaultComponents ["NeonColor"].Red, defaultComponents ["NeonColor"].Green, defaultComponents ["NeonColor"].Blue);
        } else {
            vehicle.setNeonLightsColour(0, 0, 0);
        }
    } else if (ModIndexs[category] !== undefined) {
        vehicle.setMod(ModIndexs[category], defaultComponents [category] ? defaultComponents [category] : 0);
        if (category == "Wings") vehicle.setMod(ModIndexs["RightWings"], defaultComponents [category] ? defaultComponents [category] : 0);
    }
    if (category == "ColorAdditional" || category == "FrontWheels")
        vehicle.setExtraColours(
            defaultComponents ["ColorAdditional"] ? defaultComponents ["ColorAdditional"] : 0,
            defaultComponents ["FrontWheels"] ? defaultComponents ["FrontWheels"] : 0);
    
    UpdateVehicleStats ();
}

global.SetVehicleLightColor = (vehicle, index) => {
    if (vehicle && mp.vehicles.exists(vehicle)) {
        mp.game.invoke("0xE41033B25D003A07", vehicle.handle, parseInt(index));
    }
};
						
const GetPrice = (category, index = 0) => 
{
    if (index == -1) return Math.round (pricePercent * priceVehicle * ComponentsPrice [category] * (0.5 / 100));
    if(index == 0) index = 1;
    return Math.round (pricePercent * priceVehicle * ComponentsPrice [category] * (index / 100));
}