const defaultData = () => {
    return {
        motherid: 0,
        fatherid: 0,
        shapemix: 0,
        skintone: 0,

        // characteristics
        hair: {
            variation: 0,
            color: 0
        },
        eyebrows: {
            variation: 0,
            color: 0,
            opacity: 0
        },
        facialhair: {
            variation: 0,
            color: 0,
            opacity: 0
        },
        blemishes: {
            variation: 0,
            opacity: 0
        },
        ageing: {
            variation: 0,
            opacity: 0
        },
        complexion: {
            variation: 0,
            opacity: 0
        },
        molesfreckles: {
            variation: 0,
            opacity: 0
        },
        sundamage: {
            variation: 0,
            opacity: 0
        },
        eyescolor: {
            variation: 0
        },
        lipstick: {
            variation: 0,
            color: 0,
            opacity: 0
        },
        head: {
            drawable: 0,
            texture: 0
        }, 
        tops: {
            drawable: 0,
            texture: 0
        }, 
        legs: {
            drawable: 0,
            texture: 0
        }, 
        shoes: {
            drawable: 0,
            texture: 0
        },
        Facesettings: new Array(20).fill(0.0)
    }
}

const maxSlots = 3;//Максимальное колличество чаров

let selectIndex = 0;//Выбранный чар

const defaultCustomizationData = {
    gender: false,
    false: defaultData (),
    true: defaultData ()
}

let initCustomization = false;

let CustomizationData = new Array (maxSlots).fill (defaultCustomizationData);

gm.events.add('client.characters.customization.updateIndex', (index) => {
    try {
        if (selectIndex === index)
            return;
        selectIndex = index;//Выбранный чар
        const ped = getSelectPed (index);
        if (initCustomization && ped && mp.peds.exists (ped)) {            
            global.createCamera ("char", ped);
            global.updateCameraToBone ("hat", ped, true, 750);
        }
    } catch (e) {
        mp.events.callRemote("client_trycatch", "player/character", "updateIndex", e.toString());
    }
});

gm.events.add('client.characters.customization.updateCam', (type) => {
    try {

        const ped = getSelectPed (selectIndex);   
        if (ped && mp.peds.exists (ped))  
            global.updateCameraToBone (type, ped);
    } catch (e) {
        mp.events.callRemote("client_trycatch", "player/character", "updateIndex", e.toString());
    }
});

gm.events.add('client.characters.customization.UpdateGender', (gender) => {
    try {
        SetGender (gender);
        updateHeadBlendData (gender);
    } catch (e) {
        mp.events.callRemote("client_trycatch", "player/character", "clUpdateGender", e.toString());
    }
});

gm.events.add('client.characters.customization.UpdateParents', (gender, motherId, fatherId) => {
    try {
        CustomizationData[selectIndex][gender].fatherid = motherId;
        CustomizationData[selectIndex][gender].motherid = fatherId;
        updateHeadBlendData (gender);
    } catch (e) {
		mp.events.callRemote("client_trycatch", "player/character", "clUpdateParents", e.toString());
	}
});

gm.events.add('client.characters.customization.UpdateShapeMix', (gender, shapemix) => {
    try {
        CustomizationData[selectIndex][gender].shapemix = shapemix;
        updateHeadBlendData (gender);
    } catch (e) {
        mp.events.callRemote("client_trycatch", "player/character", "clUpdateShapeMix", e.toString());
    }
});

gm.events.add('client.characters.customization.UpdateSkinTone', (gender, skintone) => {
    try {
        CustomizationData[selectIndex][gender].skintone = skintone;
        updateHeadBlendData (gender);
    } catch (e) {
        mp.events.callRemote("client_trycatch", "player/character", "clUpdateSkinTone", e.toString());
    }
});

gm.events.add('client.characters.customization.UpdateAppearance', (gender, key, id, color, opacity) => {
    try {
        SetAppearanceChange (gender, key, id, color, opacity);
    } catch (e) {
        mp.events.callRemote("client_trycatch", "player/character", "clUpdateAppearance", e.toString());
    }
});

gm.events.add('client.characters.customization.UpdateCharacteristic', (gender, xindex, x, yindex, y) => {
    try {
        SetCharacteristicChange (gender, xindex, x, yindex, y);
    } catch (e) {
        mp.events.callRemote("client_trycatch", "player/character", "clUpdateCharacteristic", e.toString());
    }
});

gm.events.add('client.characters.customization.UpdateClothes', (gender, key, drawable, texture) => {
    try {
        SetClothesChange (gender, key, drawable, texture);
    } catch (e) {
        mp.events.callRemote("client_trycatch", "player/character", "clUpdateClothes", e.toString());
    }
});

let customizationCharsData = {
    face: {},
    clothes: {},
    accessory: {},
    charsSlot: [],
}

gm.events.add('initCustomizationCharsData', (customizationsData, clothes, accessory, charsSlot) => {
    try {
        global.cameraManager.deleteCamera ('authentication');
        customizationCharsData = {
            face: JSON.parse (customizationsData),
            clothes: JSON.parse (clothes),
            accessory: JSON.parse (accessory),
            charsSlot: JSON.parse (charsSlot),
        }
        mp.events.call('CreatorCamera');
    } catch (e) {
        mp.events.callRemote("client_trycatch", "player/character", "CreatorCamera", e.toString());
    }
});

gm.events.add('CreatorCamera', (isCreate = false) => {
    StartCharacterCustomization (isCreate);
});

const posCreate = new mp.Vector3(-1410.821, -994.527, 19.380);

const pedPosition = [
    [-2635.406, 1894.9324, 157.87411, 51],
    [-2633.8286, 1892.0219, 159.54903, 54],
    [-2632.374, 1894.3527, 160.06157, 95],
    [-2629.908, 1892.279, 160.64325, 74],
    [-2627.1514, 1893.1362, 161.08452, 94],
    [-2626.007, 1890.8905, 161.52815, 71],
    [-2623.83, 1888.7576, 162.07239, 63],
    [-2622.5635, 1891.7043, 162.66278, 63],
    [-2619.404, 1892.1887, 163.1601, 71],
]

let pedData = [];
let propLogo = null;

const getSelectPed = (index = -1) => {
    return pedData [index !== -1 ? index : selectIndex];
}

const animStart = [
    {
        animDictionary: "anim@mp_player_intincarphotographylow@ds@",
        animationName: "idle_a",
        flag: 50
    },
    {
        animDictionary: "anim@mp_player_intincarrockbodhi@ds@",
        animationName: "idle_a",
        flag: 50
    },
    {
        animDictionary: "anim@mp_player_intincarsalutebodhi@ds@",
        animationName: "enter",
        flag: 50
    },
    {
        animDictionary: "anim@mp_player_intincarshushbodhi@ds@ ",
        animationName: "enter",
        flag: 50
    },
    {
        animDictionary: "clothingtie",
        animationName: "check_out_a",
        flag: 49
    },
    {
        animDictionary: "friends@frj@ig_1",
        animationName: "wave_d",
        flag: 49
    },
    {
        animDictionary: "gestures@f@standing@casual",
        animationName: "gesture_no_way",
        flag: 49
    },
    {
        animDictionary: "gestures@f@standing@casual",
        animationName: "gesture_nod_no_hard",
        flag: 49
    },
    {
        animDictionary: "gestures@f@standing@casual ",
        animationName: "gesture_shrug_hard",
        flag: 49
    },
    {
        animDictionary: "gestures@miss@fbi_5",
        animationName: "fbi5_gesture_sniff",
        flag: 49
    },
    {
        animDictionary: "friends@frj@ig_1",
        animationName: "wave_c",
        flag: 49
    },
    {
        animDictionary: "friends@frl@ig_1",
        animationName: "idle_a_lamar",
        flag: 49
    },
    {
        animDictionary: "friends@frl@ig_1",
        animationName: "waive_a_lamar",
        flag: 49
    },
    {
        animDictionary: "switch@franklin@bed",
        animationName: "stretch_short",
        flag: 49
    },
    {
        animDictionary: "amb@world_human_cheering@female_d",
        animationName: "base",
        flag: 50
    },
    {
        animDictionary: "mini@hookers_sp",
        animationName: "idle_reject_loop_a",
        flag: 1
    },
    {
        animDictionary: "mini@shop@",
        animationName: "base",
        flag: 1
    },
    {
        animDictionary: "mini@strip_club@idles@bouncer@base",
        animationName: "base",
        flag: 1
    },
    {
        animDictionary: "mini@strip_club@idles@bouncer@idle_a",
        animationName: "idle_a",
        flag: 1
    },
    {
        animDictionary: "friends@fra@ig_1",
        animationName: "impatient_idle_b",
        flag: 1
    },
    {
        animDictionary: "friends@frj@ig_1",
        animationName: "idle_d",
        flag: 1
    },
    {
        animDictionary: "friends@frl@ig_1",
        animationName: "idle_c_lamar",
        flag: 1
    },
    {
        animDictionary: "friends@frm@ig_1",
        animationName: "base_idle",
        flag: 1
    },
    {
        animDictionary: "friends@frt@ig_1",
        animationName: "trevor_base",
        flag: 1
    },
    {
        animDictionary: "amb@world_human_cop_idles@female@base",
        animationName: "base",
        flag: 1
    },
    {
        animDictionary: "amb@world_human_drug_dealer_hard@male@base",
        animationName: "base",
        flag: 1
    },
    {
        animDictionary: "amb@world_human_hang_out_street@female_arm_side@base",
        animationName: "base",
        flag: 1
    },
    {
        animDictionary: "amb@world_human_hang_out_street@female_arm_side@idle_a",
        animationName: "idle_a",
        flag: 1
    },
    {
        animDictionary: "amb@world_human_hang_out_street@female_arm_side@idle_a",
        animationName: "idle_b",
        flag: 1
    },
    {
        animDictionary: "mini@triathlon",
        animationName: "ig_2_gen_warmup_01",
        flag: 1
    },
    {
        animDictionary: "mini@triathlon",
        animationName: "ig_2_gen_warmup_04",
        flag: 1
    },
    {
        animDictionary: "missfam5_yoga",
        animationName: "c3_pose",
        flag: 1
    },
    {
        animDictionary: "anim@amb@business@cfm@cfm_drying_notes@",
        animationName: "stretch_worker",
        flag: 1
    },
    {
        animDictionary: "rcmepsilonism8",
        animationName: "security_greet",
        flag: 1
    },
    {
        animDictionary: "random@bicycle_thief@ask_help",
        animationName: "i_cant_catch_him_on_foot",
        flag: 1
    },
    {
        animDictionary: "random@bicycle_thief@ask_help",
        animationName: "my_dads_going_to_kill_me",
        flag: 1
    },
    {
        animDictionary: "random@bus_tour_guide@idle_a",
        animationName: "idle_b",
        flag: 1
    },
    {
        animDictionary: "random@car_thief@agitated@idle_a",
        animationName: "agitated_idle_b",
        flag: 1
    },
    {
        animDictionary: "random@shop_gunstore",
        animationName: "_positive_goodbye",
        flag: 1
    },
    {
        animDictionary: "anim@random@shop_clothes@watches",
        animationName: "base",
        flag: 1
    }      
]

let crushStartCharacterCustomization = 0;
const StartCharacterCustomization = async (isCreate) => {
    try {
        crushStartCharacterCustomization = 1;
        await global.wait(100);
        global.localplayer.setVisible(false, false);
        global.localplayer.setAlpha(0);
        crushStartCharacterCustomization = 2;
        gm.discord(translateText("Выбирает персонажа"));

        const playerDimension = global.localplayer.dimension;
		//Создаем клона игрока
        global.localplayer.position = new mp.Vector3(pedPosition [0][0], pedPosition [0][1], pedPosition [0][2]);
        crushStartCharacterCustomization = 3;

        await global.wait(200);

        global.createCamera ("char", global.localplayer);

        propLogo = mp.objects.new(mp.game.joaat ("redagelogo"), new mp.Vector3 (-2636.347, 1887.127, 157.8161), {
            rotation: new mp.Vector3 (0, 0, 36.3374 - 180),
            dimension: playerDimension
        });
        //await global.IsLoadEntity (propLogo);

        crushStartCharacterCustomization = 4;
        await global.requestAnimDict("amb@world_human_stand_guard@male@base");
        await global.loadModel('mp_m_freemode_01');
        await global.loadModel('mp_f_freemode_01');

        if (!isCreate) {
            crushStartCharacterCustomization = 5;
            for (let i = 0; i < maxSlots; i++) {
                if (customizationCharsData.charsSlot [i] <= -1) {
                    const ped = mp.peds.new(mp.game.joaat('mp_m_freemode_01'), new mp.Vector3(pedPosition [i][0], pedPosition [i][1], pedPosition [i][2]), pedPosition [i][3], playerDimension);
                    await global.IsLoadEntity (ped);
                    //if (customizationCharsData.charsSlot [i] === -2)
                    //    ped.setAlpha(0.0);

                    pedData.push (ped);
                    ped.taskPlayAnim("amb@world_human_stand_guard@male@base", "base", 1, 1, -1, 1, 0.0, false, false, false);
                } else {
                    pedData.push (false);
               }
            }
            crushStartCharacterCustomization = 6;

            await global.wait(100);
        } else {
            crushStartCharacterCustomization = 7;
            const ped = mp.peds.new(mp.game.joaat('mp_m_freemode_01'), new mp.Vector3(pedPosition [0][0], pedPosition [0][1], pedPosition [0][2]), pedPosition [0][3], playerDimension);             
            await global.IsLoadEntity (ped);
            pedData.push (ped);
            ped.taskPlayAnim("amb@world_human_stand_guard@male@base", "base", 1, 1, -1, 1, 0.0, false, false, false); 
            await global.wait(100);
            crushStartCharacterCustomization = 8;
        }
    } catch (e) {
        mp.events.callRemote("client_trycatch", "player/character", "StartCharacterCustomization - " + crushStartCharacterCustomization, e.toString());
    }
    if (!isCreate) {
        mp.gui.emmit(`window.initCustomizations ();`);
    } else {
        initInterface (isCreate);
    }
}

const updatePedGender = async (index, gender) => {    
    const playerDimension = global.localplayer.dimension;

    let ped = getSelectPed (index);

    if (ped && mp.peds.exists(ped)) {
        ped.destroy();
    }
    pedData[index] = mp.peds.new(gender ? mp.game.joaat('mp_m_freemode_01') : mp.game.joaat('mp_f_freemode_01'), new mp.Vector3(pedPosition [index][0], pedPosition [index][1], pedPosition [index][2]), pedPosition [index][3], playerDimension);
    await global.IsLoadEntity (pedData[index]);
    
    clearClothes (pedData[index], gender);

    pedData[index].taskPlayAnim("amb@world_human_stand_guard@male@base", "base", 1, 1, -1, 1, 0.0, false, false, false); 
    return true;
}


function clearClothes(ped, gender) {
    try {
        ped.clearProp(0);
        ped.clearProp(1);
        ped.clearProp(2);
        ped.clearProp(6);
        ped.clearProp(7);
        
        ped.setComponentVariation(1, global.clothesEmpty[gender][1], 0, 0);
        ped.setComponentVariation(3, global.clothesEmpty[gender][3], 0, 0);
        ped.setComponentVariation(4, global.clothesEmpty[gender][4], 0, 0);
        ped.setComponentVariation(5, global.clothesEmpty[gender][5], 0, 0);
        ped.setComponentVariation(6, global.clothesEmpty[gender][6], 0, 0);
        ped.setComponentVariation(7, global.clothesEmpty[gender][7], 0, 0);
        ped.setComponentVariation(8, global.clothesEmpty[gender][8], 0, 0);
        ped.setComponentVariation(9, global.clothesEmpty[gender][9], 0, 0);
        ped.setComponentVariation(10, global.clothesEmpty[gender][10], 0, 0);
        ped.setComponentVariation(11, global.clothesEmpty[gender][11], 0, 0);
    } catch (e) {
        mp.events.callRemote("client_trycatch", "player/character", "clearClothes", e.toString());
    }
}

let InitPlayerCharDeb = 0;
gm.events.add('client.characters.initChars', async () => {
    try {
        InitPlayerCharDeb = 0;

        for (let key in customizationCharsData.face) {
            if (customizationCharsData.face [key]) {

                const customData = customizationCharsData.face [key];

                await updatePedGender (key, Number (customData.Gender));
            }
        }
        
        for (let key in customizationCharsData.face) {
            if (customizationCharsData.face [key]) {
                InitPlayerCharDeb = 1;
                
                const customData = customizationCharsData.face [key];

                const ped = getSelectPed (key);   
                if (ped && mp.peds.exists (ped)) {
                        
                    InitPlayerCharDeb = 2;
                    let hair = JSON.parse (customData.Hair);
                    ped.setComponentVariation(2, hair.Hair, 0, 0);
                    ped.setHairColor(hair.Color, hair.HighlightColor);
                    InitPlayerCharDeb = 3;
        
                    let tattoos = JSON.parse (customData.Tattoos);
                    for (let keyIndex in tattoos) {
                        if (tattoos [keyIndex]) {
                            tattoos [keyIndex].forEach(tattoo => {
                                ped.setDecoration(mp.game.joaat(tattoo.Dictionary), mp.game.joaat(tattoo.Hash));
                            });
                        }
                    }
                    InitPlayerCharDeb = 4;
        
                    let parents = JSON.parse (customData.Parents);
        
                    ped.setHeadBlendData(
                        parents.Mother,
                        parents.Father,
                        0,
            
                        parents.Mother,
                        parents.Father,
                        0,
            
                        parents.Similarity,
                        parents.SkinSimilarity,
                        0.0,
            
                        true
                    );
                    InitPlayerCharDeb = 5;
        
                    const features = JSON.parse (customData.Features);
                    for (let i = 0; i < features.length; i++)
                        ped.setFaceFeature(i, features [i]);
        
                    InitPlayerCharDeb = 6;
        
                    ped.setEyeColor(Number (customData.Eyec));
                    InitPlayerCharDeb = 7;

                    const appearance = JSON.parse (customData.Appearance);
                    for (let i = 0; i < appearance.length; i++) {
                        const headOverlay = appearance [i];
        
                        ped.setHeadOverlay(i, Number (headOverlay.Value), Number (headOverlay.Opacity), 1, 1);
                        if (headOverlay.Color !== undefined) {
                            if ([1, 2, 10].includes (i)) ped.setHeadOverlayColor(i, 1, Number (headOverlay.Color), 0);
                            else if ([5, 8].includes (i)) ped.setHeadOverlayColor(i, 2, Number (headOverlay.Color), 0);
                            else if ([0].includes (i)) ped.setHeadOverlayColor(i, 0, Number (headOverlay.Color), 0);
                        } else {
                            if ([1, 2, 10].includes (i)) ped.setHeadOverlayColor(i, 1, 0, 0);
                            else if ([5, 8].includes (i)) ped.setHeadOverlayColor(i, 2, 0, 0);
                            else if ([0].includes (i)) ped.setHeadOverlayColor(i, 0, 0, 0);
                        }
                    }
                    InitPlayerCharDeb = 7;
                    InitPlayerCharDeb = 8;            
                }    
            }
        }

        for (let key in customizationCharsData.clothes) {
            if (customizationCharsData.clothes [key]) {

                const ped = getSelectPed (key);

                const clothesData = customizationCharsData.clothes [key];
                for (let slot in clothesData) {
                    if (ped && mp.peds.exists (ped)) 
                        ped.setComponentVariation(Number (slot), Number (clothesData [slot].Drawable), Number (clothesData [slot].Texture), 0);
                }
            }
        }

        for (let key in customizationCharsData.accessory) {
            if (customizationCharsData.accessory [key]) {

                const ped = getSelectPed (key);

                const accessoryData = customizationCharsData.accessory [key];
                for (let slot in accessoryData) {
                    if (ped && mp.peds.exists (ped)) 
                        ped.setPropIndex(Number (slot), Number (accessoryData [slot].Drawable), Number (accessoryData [slot].Texture), true);
                }
            }
        }
        await global.wait(100);

        global.createCamera ("char", pedData [0]);
        global.updateCameraToBone ("hat", pedData [0], true, 0);

        animStart.forEach(async (anim) => {
            await global.requestAnimDict(anim.animDictionary);
        });

        for (let key in customizationCharsData.clothes) {

            const i = global.getRandomInt (0, animStart.length)
            if (animStart [i]) {
                global.requestAnimDict(animStart [i].animDictionary).then(() => {
                    const ped = getSelectPed (key);
                    if (ped && mp.peds.exists (ped)) 
                        ped.taskPlayAnim(animStart [i].animDictionary, animStart [i].animationName, 1, 1, -1, 1, 0.0, false, false, false);  
                }); 
            }
        }

        await global.wait(200);

        initInterface ();

    } catch (e) {
        mp.events.callRemote("client_trycatch", "player/character", "InitPlayerChar - " + InitPlayerCharDeb, e.toString());
    }
});

const initInterface = async (isCreate) => {

    if (!isCreate) 
        mp.gui.emmit(`events.callEvent("cef.authentication.setView", "Chars")`);
    else
        mp.gui.emmit(`window.router.setView("PlayerCustomization")`);
        
    //mp.events.call('setTimeCmd', 12, 0, 0);
    //mp.events.call('SetWeather', 0);
    
    global.menuOpen();

    initCustomization = true;

    global.createCamera ("char", pedData [0]);
    global.updateCameraToBone ("hat", pedData [0], true, 0);

    await global.wait(500);

    global.FadeScreen (false, 1500);

    initCustomization = true;
}

const UpdateGender = async (gender) => {
    try {
        await updatePedGender (selectIndex, Number (gender));
    } catch (e) {
        mp.events.callRemote("client_trycatch", "player/character", "UpdateGender", e.toString());
    }
}

const SetGender = (gender) => {
    CustomizationData[selectIndex].gender = gender;
    UpdateGender (gender);
    //global.localplayer.setComponentVariation(8, 15, 0, 0);
    UpdateAll (gender);
}

const UpdateAll = (gender) => {
    try {
        const ped = getSelectPed ();
        if (ped && mp.peds.exists (ped)) {
            UpdateClothesChange (gender, "head");
            UpdateClothesChange (gender, "tops");
            UpdateClothesChange (gender, "legs");
            UpdateClothesChange (gender, "shoes");
            UpdateAppearance (gender, "hair");    
            UpdateAppearance (gender, "eyebrows");
            UpdateAppearance (gender, "blemishes");
            UpdateAppearance (gender, "ageing");
            UpdateAppearance (gender, "complexion");
            UpdateAppearance (gender, "molesfreckles");
            UpdateAppearance (gender, "sundamage");
            UpdateAppearance (gender, "eyescolor");
            if (gender) UpdateAppearance (gender, "facialhair");
            else UpdateAppearance (gender, "lipstick");
            for (let i = 0; i < 20; i++) ped.setFaceFeature(i, CustomizationData[selectIndex][gender].Facesettings[i]);
            updateHeadBlendData (gender);   
        }
    } catch (e) {
        mp.events.callRemote("client_trycatch", "player/character", "UpdateAll", e.toString());
    } 
}


const updateHeadBlendData = (gender) => {
    try {
        const ped = getSelectPed ();
        if (ped && mp.peds.exists (ped)) {
            const customData = CustomizationData[selectIndex][gender];
            ped.setHeadBlendData(
                customData.motherid,
                customData.fatherid,
                0,

                customData.motherid,
                customData.fatherid,
                0,

                customData.shapemix,
                customData.skintone,
                0.0,

                true
            );
        }
    } catch (e) {
        mp.events.callRemote("client_trycatch", "player/character", "updateHeadBlendData", e.toString());
    }
}
const SetAppearanceChange = (gender, key, id, color, opacity) => {   
    try {
        if(key == "hair") {
            CustomizationData[selectIndex][gender].hair.variation = id;
            CustomizationData[selectIndex][gender].hair.color = color;
        } else if(key == "eyebrows") {
            CustomizationData[selectIndex][gender].eyebrows.variation = id;
            CustomizationData[selectIndex][gender].eyebrows.color = color;
            CustomizationData[selectIndex][gender].eyebrows.opacity = opacity;
        } else if(key == "facialhair") {
            CustomizationData[selectIndex][gender].facialhair.variation = id;
            CustomizationData[selectIndex][gender].facialhair.color = color;
            CustomizationData[selectIndex][gender].facialhair.opacity = opacity;
        } else if(key == "blemishes") {
            CustomizationData[selectIndex][gender].blemishes.variation = id;
            CustomizationData[selectIndex][gender].blemishes.opacity = opacity;
        } else if(key == "ageing") {
            CustomizationData[selectIndex][gender].ageing.variation = id;
            CustomizationData[selectIndex][gender].ageing.opacity = opacity;
        } else if(key == "complexion") {
            CustomizationData[selectIndex][gender].complexion.variation = id;
            CustomizationData[selectIndex][gender].complexion.opacity = opacity;
        } else if (key == "molesfreckles") {
            CustomizationData[selectIndex][gender].molesfreckles.variation = id;
            CustomizationData[selectIndex][gender].molesfreckles.opacity = opacity;
        } else if (key == "sundamage") {
            CustomizationData[selectIndex][gender].sundamage.variation = id;
            CustomizationData[selectIndex][gender].sundamage.opacity = opacity;
        } else if(key == "eyescolor") {
            CustomizationData[selectIndex][gender].eyescolor.variation = id;
        } else if(key == "lipstick") {
            CustomizationData[selectIndex][gender].lipstick.variation = id;
            CustomizationData[selectIndex][gender].lipstick.color = color;
            CustomizationData[selectIndex][gender].lipstick.opacity = opacity;
        }
        UpdateAppearance (gender, key);
    } catch (e) {
        mp.events.callRemote("client_trycatch", "player/character", "SetAppearanceChange", e.toString());
    }
}

const UpdateAppearance = (gender, appearance) => {
    try {
        const ped = getSelectPed ();
        if (ped && mp.peds.exists (ped)) {
            const customData = CustomizationData[selectIndex][gender];
            if(appearance == "hair") {//+
                ped.setComponentVariation(2, customData.hair.variation, 0, 0);
                ped.setHairColor(customData.hair.color, customData.hair.color);
            } else if(appearance == "eyebrows") {//+
                ped.setHeadOverlay(2, customData.eyebrows.variation, customData.eyebrows.opacity, 1, 1);
                ped.setHeadOverlayColor(2, 1, customData.eyebrows.color, 0);
            } else if(appearance == "facialhair") {//+
                ped.setHeadOverlay(1, customData.facialhair.variation, customData.facialhair.opacity, 1, 1);
                ped.setHeadOverlayColor(1, 1, customData.facialhair.color, 0);
            } else if(appearance == "blemishes") {
                ped.setHeadOverlay(0, customData.blemishes.variation, customData.blemishes.opacity, 1, 1);
            } else if(appearance == "ageing") {
                ped.setHeadOverlay(3, customData.ageing.variation, customData.ageing.opacity, 1, 1);
            } else if(appearance == "complexion") {
                ped.setHeadOverlay(6, customData.complexion.variation, customData.complexion.opacity, 1, 1);
            } else if (appearance == "molesfreckles") {
                ped.setHeadOverlay(9, customData.molesfreckles.variation, customData.molesfreckles.opacity, 1, 1);
            } else if (appearance == "sundamage") {
                ped.setHeadOverlay(7, customData.sundamage.variation, customData.sundamage.opacity, 1, 1);
            } else if(appearance == "eyescolor") {//+
                ped.setEyeColor(customData.eyescolor.variation);
            } else if(appearance == "lipstick") {
                ped.setHeadOverlay(8, customData.lipstick.variation, customData.lipstick.opacity, 1, 1);
                ped.setHeadOverlayColor(8, 2, customData.lipstick.color, 0);
            }
        }
    } catch (e) {
        mp.events.callRemote("client_trycatch", "player/character", "UpdateAppearance", e.toString());
    }
}

const SetCharacteristicChange = (gender, xindex, x, yindex, y) => {
    try {
        const ped = getSelectPed ();
        if (ped && mp.peds.exists (ped)) {
            if (xindex != -1)
            {
                if (
                    xindex == 2 || // translateText("Мост")? носа тоже видимо наоборот.
                    xindex == 10 || // Щёки тоже разворачиваем
                    xindex == 11 || // Глаза у R* идут наоборот, -1.0 - открытые - узкие - 1.0, по этому разворачиваем
                    xindex == 12 || // Толстота губ тоже попадает под разворот
                    xindex == 17 // Заострённость подбородка
                ) x *= -1.0;

                if (xindex == 16)
                {
                    // У 16: Chin position (0 inward, 1.0 outward), от нуля до 1.0
                    x = (x + 1.0) / 2.0;
                }

                CustomizationData[selectIndex][gender].Facesettings[xindex] = x;
                ped.setFaceFeature(xindex, x);
            }
            if (yindex != -1)
            {
                CustomizationData[selectIndex][gender].Facesettings[yindex] = y;
                ped.setFaceFeature(yindex, y);
            }
        }
    } catch (e) {
        mp.events.callRemote("client_trycatch", "player/character", "SetCharacteristicChange", e.toString());
    }
}

const SetClothesChange = (gender, key, drawable, texture) => {
    try {
        CustomizationData[selectIndex][gender][key] = {
            drawable: drawable,
            texture: texture
        };
        UpdateClothesChange (gender, key);
    } catch (e) {
        mp.events.callRemote("client_trycatch", "player/character", "SetClothesChange", e.toString());
    }
}

const UpdateClothesChange = (gender, key) => {
    try {
        const ped = getSelectPed ();
        if (ped && mp.peds.exists (ped)) {
            const customData = CustomizationData[selectIndex][gender];        
            if (key === "head") {
                if (customData.head.drawable == -1)
                    ped.clearProp(0);
                else 
                    ped.setPropIndex(0, customData.head.drawable, customData.head.texture, true);
            }
            else if (key === "tops") {
                ped.setComponentVariation(8, global.clothesEmpty[Number (gender)][8], 0, 0);
                ped.setComponentVariation(11, customData.tops.drawable, customData.tops.texture, 0);
            } else if (key === "legs") ped.setComponentVariation(4, customData.legs.drawable, customData.legs.texture, 0);
            else if (key === "shoes") ped.setComponentVariation(6, customData.shoes.drawable, customData.shoes.texture, 0);
        }
    } catch (e) {
        mp.events.callRemote("client_trycatch", "player/character", "UpdateClothesChange", e.toString());
    }
}

gm.events.add('client.characters.create.error', async (text) => {
    global.FadeScreen (false, 500);
    mp.gui.emmit(`window.events.callEvent("cef.customization.error",  '${text}')`);
});

gm.events.add('client.characters.customization.create', async (slot, firstName, lastName, gender) => {
    try {    
        const customData = CustomizationData[selectIndex][gender];
        
        //mp.gui.emmit(`window.router.close()`);
        global.FadeScreen (true, 0);
        await global.wait(50);
        let appearance_values = [];
        customData.blemishes ? appearance_values.push({ Value: customData.blemishes.variation, Opacity: customData.blemishes.opacity }) : appearance_values.push({ Value: 255, Opacity: 100 });
        customData.facialhair ? appearance_values.push({ Value: customData.facialhair.variation, Opacity: customData.facialhair.opacity }) : appearance_values.push({ Value: 255, Opacity: 100 });
        customData.eyebrows ? appearance_values.push({ Value: customData.eyebrows.variation, Opacity: customData.eyebrows.opacity }) : appearance_values.push({ Value: 255, Opacity: 100 });
        customData.ageing ? appearance_values.push({ Value: customData.ageing.variation, Opacity: customData.ageing.opacity }) : appearance_values.push({ Value: 255, Opacity: 100 });
        appearance_values.push({ Value: 255, Opacity: 100 });
        appearance_values.push({ Value: 255, Opacity: 100 });
        customData.complexion ? appearance_values.push({ Value: customData.complexion.variation, Opacity: customData.complexion.opacity }) : appearance_values.push({ Value: 255, Opacity: 100 });
        customData.sundamage ? appearance_values.push({ Value: customData.sundamage.variation, Opacity: customData.sundamage.opacity }) : appearance_values.push({ Value: 255, Opacity: 100 });
        customData.lipstick ? appearance_values.push({ Value: customData.lipstick.variation, Opacity: customData.lipstick.opacity }) : appearance_values.push({ Value: 255, Opacity: 100 });
        customData.molesfreckles ? appearance_values.push({ Value: customData.molesfreckles.variation, Opacity: customData.molesfreckles.opacity }) : appearance_values.push({ Value: 255, Opacity: 100 });
        appearance_values.push({ Value: 255, Opacity: 100 });

        let hair_or_colors = [];
        hair_or_colors.push(customData.hair ? customData.hair.variation : 0);
        hair_or_colors.push(customData.hair ? customData.hair.color : 0);
        hair_or_colors.push(customData.hair ? customData.hair.color : 0);
        hair_or_colors.push(customData.eyebrows ? customData.eyebrows.color : 0);
        hair_or_colors.push(customData.facialhair ? customData.facialhair.color : 0);
        hair_or_colors.push(customData.eyescolor ? customData.eyescolor.variation : 0);
        hair_or_colors.push(0);
        hair_or_colors.push(customData.lipstick ? customData.lipstick.color : 0);
        hair_or_colors.push(0);
        let clothes = [];
        
        clothes.push({ Drawable: customData.head.drawable, Texture: customData.head.texture });//head
        clothes.push({ Drawable: customData.tops.drawable, Texture: customData.tops.texture });//tops
        clothes.push({ Drawable: customData.legs.drawable, Texture: customData.legs.texture });//legs
        clothes.push({ Drawable: customData.shoes.drawable, Texture: customData.shoes.texture });//shoes

        mp.events.callRemote("CreateCharacter",
            slot, 
            firstName,
            lastName,
            gender,
            customData.fatherid,
            customData.motherid,
            customData.shapemix,
            customData.skintone,
            JSON.stringify(customData.Facesettings),
            JSON.stringify(appearance_values),
            JSON.stringify(hair_or_colors),
            JSON.stringify(clothes));
    } catch (e) {
		mp.events.callRemote("client_trycatch", "player/character", "create", e.toString());
	}
});

gm.events.add('client.charcreate.close', async (isNoEnd = false) => {
    if (!isNoEnd) 
        mp.events.call('resumeTime');
    
    global.cameraManager.stopCamera ();
    initCustomization = false;
    
    global.localplayer.setVisible(true, false);
    global.localplayer.setAlpha(255);
    global.localplayer.freezePosition(false);    
    global.setPlayerToGround ();

    customizationCharsData = {
        face: {},
        clothes: {},
        accessory: {},
        charsSlot: [],
    }

    if (propLogo && mp.objects.exists(propLogo)) {
        propLogo.destroy();
    }
    propLogo = null;
    
    pedData.forEach(ped => {
        if (ped && mp.peds.exists(ped)) {
            ped.destroy();
        }
    });
    pedData = []

    selectIndex = 0;
});