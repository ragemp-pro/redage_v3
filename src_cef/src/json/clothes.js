
import BugsJson from './clothes/clothes_Bugs.json';


import Male_MasksJson from './clothes/clothes_Male_Masks.json';
import Male_AccessoriesJson from './clothes/clothes_Male_Accessories.json';
import Male_EarsJson from './clothes/clothes_Male_Ears.json';
import Male_GlassesJson from './clothes/clothes_Male_Glasses.json';
import Male_HatsJson from './clothes/clothes_Male_Hats.json';
import Male_LegsJson from './clothes/clothes_Male_Legs.json';
import Male_ShoesJson from './clothes/clothes_Male_Shoes.json';
import Male_TopsJson from './clothes/clothes_Male_Tops.json';
import Male_UndershortJson from './clothes/clothes_Male_Undershort.json';
import Male_WatchesJson from './clothes/clothes_Male_Watches.json';
import Male_TorsosJson from './clothes/clothes_Male_Torsos.json';
import Male_BraceletsJson from './clothes/clothes_Male_Bracelets.json';
import Male_BodyArmorsJson from './clothes/clothes_Male_BodyArmors.json';
import Male_DecalsJson from './clothes/clothes_Male_Decals.json';

import Female_MasksJson from './clothes/clothes_Female_Masks.json';
import Female_AccessoriesJson from './clothes/clothes_Female_Accessories.json';
import Female_EarsJson from './clothes/clothes_Female_Ears.json';
import Female_GlassesJson from './clothes/clothes_Female_Glasses.json';
import Female_HatsJson from './clothes/clothes_Female_Hats.json';
import Female_LegsJson from './clothes/clothes_Female_Legs.json';
import Female_ShoesJson from './clothes/clothes_Female_Shoes.json';
import Female_TopsJson from './clothes/clothes_Female_Tops.json';
import Female_UndershortJson from './clothes/clothes_Female_Undershort.json';
import Female_WatchesJson from './clothes/clothes_Female_Watches.json';
import Female_TorsosJson from './clothes/clothes_Female_Torsos.json';
import Female_BraceletsJson from './clothes/clothes_Female_Bracelets.json';
import Female_BodyArmorsJson from './clothes/clothes_Female_BodyArmors.json';
import Female_DecalsJson from './clothes/clothes_Female_Decals.json';

const jsonClothesData = {
    "Male": {
        "Masks": Male_MasksJson,
        "Bugs": BugsJson,
        "Hat": Male_HatsJson,
        "Tops": Male_TopsJson,
        "Undershort": Male_UndershortJson,
        "Legs": Male_LegsJson,
        "Shoes": Male_ShoesJson,
        "Watches": Male_WatchesJson,
        "Bracelets": Male_BraceletsJson,
        "Glasses": Male_GlassesJson,
        "Accessories": Male_AccessoriesJson,
        "Ears": Male_EarsJson,
        "Torsos": Male_TorsosJson,
        "BodyArmors": Male_BodyArmorsJson,
        "Decals": Male_DecalsJson,
    },    
    "Female": {
        "Masks": Female_MasksJson,
        "Bugs": BugsJson,
        "Hat": Female_HatsJson,
        "Tops": Female_TopsJson,
        "Undershort": Female_UndershortJson,
        "Legs": Female_LegsJson,
        "Shoes": Female_ShoesJson,
        "Watches": Female_WatchesJson,
        "Bracelets": Female_BraceletsJson,
        "Glasses": Female_GlassesJson,
        "Accessories": Female_AccessoriesJson,
        "Ears": Female_EarsJson,
        "Torsos": Female_TorsosJson,
        "BodyArmors": Female_BodyArmorsJson,
        "Decals": Female_DecalsJson,
    }
}

export const getClothesDictionary = (gender, name) => {
    return JSON.stringify (jsonClothesData[gender][name]);
}


import barber_Male_BeardJson from './clothes/barber_Male_Beard.json';
import barber_Male_BodyJson from './clothes/barber_Male_Body.json';
import barber_Male_EyebrowsJson from './clothes/barber_Male_Eyebrows.json';
import barber_Male_EyesJson from './clothes/barber_Male_Eyes.json';
import barber_Male_HairJson from './clothes/barber_Male_Hair.json';
import barber_Male_LipsJson from './clothes/barber_Male_Lips.json';
import barber_Male_MakeupJson from './clothes/barber_Male_Makeup.json';
import barber_Male_PaletteJson from './clothes/barber_Male_Palette.json';

import barber_Female_BeardJson from './clothes/barber_Female_Beard.json';
import barber_Female_BodyJson from './clothes/barber_Female_Body.json';
import barber_Female_EyebrowsJson from './clothes/barber_Female_Eyebrows.json';
import barber_Female_EyesJson from './clothes/barber_Female_Eyes.json';
import barber_Female_HairJson from './clothes/barber_Female_Hair.json';
import barber_Female_LipsJson from './clothes/barber_Female_Lips.json';
import barber_Female_MakeupJson from './clothes/barber_Female_Makeup.json';
import barber_Female_PaletteJson from './clothes/barber_Female_Palette.json';

const jsonBarberData = {
    "Male": {
        "Beard": barber_Male_BeardJson,
        "Body": barber_Male_BodyJson,
        "Eyebrows": barber_Male_EyebrowsJson,
        "Eyes": barber_Male_EyesJson,
        "Hair": barber_Male_HairJson,
        "Lips": barber_Male_LipsJson,
        "Makeup": barber_Male_MakeupJson,
        "Palette": barber_Male_PaletteJson,
    },    
    "Female": {
        "Beard": barber_Female_BeardJson,
        "Body": barber_Female_BodyJson,
        "Eyebrows": barber_Female_EyebrowsJson,
        "Eyes": barber_Female_EyesJson,
        "Hair": barber_Female_HairJson,
        "Lips": barber_Female_LipsJson,
        "Makeup": barber_Female_MakeupJson,
        "Palette": barber_Female_PaletteJson,
    }
}

export const getBarberDictionary = (gender, name) => {
    return JSON.stringify (jsonBarberData[gender][name]);
}


import tattoo_Head from './clothes/tattoo_Head.json';
import tattoo_LeftArm from './clothes/tattoo_LeftArm.json';
import tattoo_RightArm from './clothes/tattoo_RightArm.json';
import tattoo_LeftLeg from './clothes/tattoo_LeftLeg.json';
import tattoo_RightLeg from './clothes/tattoo_RightLeg.json';
import tattoo_Torso from './clothes/tattoo_Torso.json';


const jsonTattooData = {
    "Head": tattoo_Head,
    "Torso": tattoo_Torso,
    "LeftArm": tattoo_LeftArm,
    "RightArm": tattoo_RightArm,
    "LeftLeg": tattoo_LeftLeg,
    "RightLeg": tattoo_RightLeg,
}

export const getTattooDictionary = (name) => {
    return JSON.stringify (jsonTattooData[name]);
}




export const menu = [
    //
    {
        title: "головного убора",
        type: "clothes",
        dictionary: "Hat",
        icon: "inv-item-cap",
        function: [
            {
                event: "setPropIndex",
                componentId: 0
            },
        ],
        camera: "hat"
    },
    {
        title: "очков",
        type: "clothes",
        dictionary: "Glasses",
        icon: "inv-item-glasses",
        function: [
            {
                event: "setPropIndex",
                componentId: 1
            },
        ],
        camera: "hat"
    },
    {
        title: "серёжек",
        type: "clothes",
        dictionary: "Ears",
        icon: "inv-item-ears",
        function: [
            {
                event: "setPropIndex",
                componentId: 2
            },
        ],
        camera: "hat"
    },
    {
        title: "маски",
        type: "clothes",
        dictionary: "Masks",
        icon: "inv-item-mask",
        function: [
            {
                event: "setComponentVariation",
                componentId: 1
            },
        ],
        camera: "hat"
    },
    {
        title: "аксессуара",
        type: "clothes",
        dictionary: "Accessories",
        icon: "inv-item-necklace",
        function: [
            {
                event: "setComponentVariation",
                componentId: 7
            },
        ],
        camera: "hat"
    },    
    {
        title: "верхней одежды",
        type: "clothes",
        dictionary: "Tops",
        icon: "inv-item-jacket",
        function: [
            {
                event: "setComponentVariation",
                componentId: 11
            },
        ],
        camera: "top"
    },
    {
        title: "нижней одежды",
        type: "clothes",
        dictionary: "Undershort",
        icon: "inv-item-shirt",
        function: [
            {
                event: "setComponentVariation",
                componentId: 11
            },
        ],
        camera: "top"
    },
    {
        title: "рюкзаков",
        type: "clothes",
        dictionary: "Bugs",
        icon: "inv-item-backpack",
        function: [
            {
                event: "setComponentVariation",
                componentId: 5
            },
        ],
        camera: "top"
    },
    {
        title: "перчаток",
        type: "clothes",
        dictionary: "Torsos",
        icon: "inv-item-glove",
        function: [
            {
                event: "setComponentVariation",
                componentId: 3
            },
        ],
        camera: "top"
    },
    {
        title: "часов",
        type: "clothes",
        dictionary: "Watches",
        icon: "inv-item-clock",
        function: [
            {
                event: "setPropIndex",
                componentId: 6
            },
        ],
        camera: "top"
    },
    {
        title: "браслетов",
        type: "clothes",
        dictionary: "Bracelets",
        icon: "inv-item-bracelet",
        function: [
            {
                event: "setPropIndex",
                componentId: 7
            },
        ],
        camera: "top"
    },
    {
        title: "штанов",
        type: "clothes",
        dictionary: "Legs",
        icon: "inv-item-shorts",
        function: [
            {
                event: "setComponentVariation",
                componentId: 4
            },
        ],
        camera: "legs"
    },
    {
        title: "ботинок",
        type: "clothes",
        dictionary: "Shoes",
        icon: "inv-item-sneakers",
        function: [
            {
                event: "setComponentVariation",
                componentId: 6
            },
        ],
        camera: "shoes"
    },
    {
        title: "бронежелет",
        type: "clothes",
        dictionary: "BodyArmors",
        icon: "inv-item-armor",
        function: [
            {
                event: "setComponentVariation",
                componentId: 9
            },
        ],
        camera: "top"
    },
    {
        title: "декали",
        type: "clothes",
        dictionary: "Decals",
        icon: "inv-item-bracelet",
        function: [
            {
                event: "setComponentVariation",
                componentId: 10
            },
        ],
        camera: "top"
    },

    //Баребер
    {
        title: "прически",
        type: "barber",
        dictionary: "Hair",
        icon: "newbarbershopicons-hair",
        color: true,
        colorHighlight: true,
        function: [
            {
                event: "setComponentVariation",
                componentId: 2
            },
            {
                event: "setHairColor"
            }
        ],
        camera: "hat",
        isHair: true
    },
    {
        title: "бороды",
        type: "barber",
        dictionary: "Beard",
        icon: "newbarbershopicons-beard",
        color: true,
        opacity: true,
        function: [
            {
                event: "setHeadOverlay",
                overlayID: 1
            },
            {
                event: "setHeadOverlayColor",
                overlayID: 1,
                colorType: 1
            }
        ],
        camera: "hat",
        isHair: true,
        gender: "Male"
    },
    {
        title: "бровей",
        type: "barber",
        dictionary: "Eyebrows",
        icon: "newbarbershopicons-eyebrows",
        color: true,
        opacity: true,
        function: [
            {
                event: "setHeadOverlay",
                overlayID: 2
            },
            {
                event: "setHeadOverlayColor",
                overlayID: 2,
                colorType: 1
            }
        ],
        camera: "hat",
        isHair: true
    },
    {
        title: "волос на груди",
        type: "barber",
        dictionary: "Body",
        icon: "newbarbershopicons-body",
        color: true,
        opacity: true,
        function: [
            {
                event: "setHeadOverlay",
                overlayID: 10
            },
            {
                event: "setHeadOverlayColor",
                overlayID: 10,
                colorType: 1
            }
        ],
        camera: "top",
        isHair: true,
        gender: "Male"
    },
    {
        title: "линз",
        type: "barber",
        dictionary: "Eyes",
        icon: "newbarbershopicons-eyes",
        function: [
            {
                event: "setEyeColor"
            },
        ],
        camera: "hat"
    },
    {
        title: "помады",
        type: "barber",
        dictionary: "Lips",
        icon: "newbarbershopicons-lips",
        color: true,
        opacity: true,
        function: [
            {
                event: "setHeadOverlay",
                overlayID: 8
            },
            {
                event: "setHeadOverlayColor",
                overlayID: 8,
                colorType: 2
            }
        ],
        camera: "hat"
    },
    {
        title: "румянца",
        type: "barber",
        dictionary: "Palette",
        icon: "newbarbershopicons-palette",
        color: true,
        opacity: true,
        function: [
            {
                event: "setHeadOverlay",
                overlayID: 5
            },
            {
                event: "setHeadOverlayColor",
                overlayID: 5,
                colorType: 2
            }
        ],
        camera: "hat"
    },
    {
        title: "теней",
        type: "barber",
        dictionary: "Makeup",
        icon: "newbarbershopicons-makeup",
        opacity: true,
        function: [
            {
                event: "setHeadOverlay",
                overlayID: 4
            },
            {
                event: "setHeadOverlayColor",
                overlayID: 4,
                colorType: 0
            }
        ],
        camera: "hat"
    },

    //

    {
        title: "тату на голове",
        type: "tattoo",
        dictionary: "Head",
        icon: "ic-st-t-head",
        camera: "hat",
        tattooId: 1,
    },

    {
        title: "тату на торсе",
        type: "tattoo",
        dictionary: "Torso",
        icon: "newbarbershopicons-body",
        camera: "top",
        tattooId: 0,
    },

    {
        title: "тату на левой руке",
        type: "tattoo",
        dictionary: "LeftArm",
        icon: "ic-st-t-muscles",
        camera: "top",
        tattooId: 2,
    },

    {
        title: "тату на правой руке",
        type: "tattoo",
        dictionary: "RightArm",
        icon: "ic-st-t-muscler",
        camera: "top",
        tattooId: 3,
    },

    {
        title: "тату на левой ноге",
        type: "tattoo",
        dictionary: "LeftLeg",
        icon: "ic-st-t-leg2",
        camera: "legs",
        tattooId: 4,
    },

    {
        title: "тату на правой ноге",
        type: "tattoo",
        dictionary: "RightLeg",
        icon: "ic-st-t-leg1",
        camera: "legs",
        tattooId: 5,
    },

];

export let clothesEmpty = {
    "Female": {1: 0, 3: 15, 4: 15, 5: 0, 6: 35, 7: 0, 8: 6, 9: 0, 10: 0, 11: 15},
    "Male": {1: 0, 3: 15, 4: 21, 5: 0, 6: 34, 7: 0, 8: 15, 9: 0, 10: 0, 11: 15}
};