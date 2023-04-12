import { writable } from 'svelte/store';
import { executeClient } from 'api/rage'
import { accountData } from 'store/account' 

import appearances from '@/views/player/newauthentication/chars/create/elements/appearance/appearances.js';
import parents from '@/views/player/newauthentication/chars/create/elements/info/parents.js';
import characteristicsIndexes from '@/views/player/newauthentication/chars/create/elements/characteristics/indexes.js';

import clothes from '@/views/player/newauthentication/chars/create/elements/clothes/clothes.js';

import CreateNewCustomization from './random/index.js'

const defaultData = {
    gender: true,
    false: {},
    true: {},
};

const maxSlots = 3;//Максимальное колличество чаров

let selectIndex = 0;//Выбранный чар
const customizations = new Array (maxSlots).fill (defaultData);//Локальные данные

let init = false;

export const customization = writable({});
export const gender = writable(true);


const updateCustomizationData = () => {
    if (!init)
        return;
    const gender = customizations[selectIndex].gender;
    customization.set (customizations[selectIndex][gender])
}

const updateName = () => {
    if (!init)
        return;
    const gender = customizations[selectIndex].gender;
    FirstName.set (customizations[selectIndex][gender].FirstName);
    LastName.set (customizations[selectIndex][gender].LastName);
}

let chars = [];
accountData.subscribe(value => {
	if (value.charsSlot)
        chars = value.charsSlot;
});

export const updateIndex = index => {
    //if (selectIndex === index)
    //    return;
    selectIndex = index;
    executeClient("client.characters.customization.updateIndex", index);
    const char = chars [index];
    if (char === -1) {
        updateCustomizationData();
        updateName();
    }
}

export const updateGender = async (index, newgender) => {
    if (index !== selectIndex)
        return;
    customizations[selectIndex].gender = newgender;
    executeClient("client.characters.customization.UpdateGender", newgender);
    gender.set (newgender);
    updateCustomizationData();
    updateName();
    await window.wait(50);
    initCustom (selectIndex);
}

export const initCustom = (index) => {
    if (!init)
        return;
    const gender = customizations[index].gender;
    const data = customizations[index][gender];
    executeClient("client.characters.customization.UpdateParents", gender, parents[false][data.motherId].gtaId, parents[true][data.fatherId].gtaId);
    executeClient("client.characters.customization.UpdateShapeMix", gender, data.shapeMix);
    executeClient("client.characters.customization.UpdateSkinTone", gender, data.skinTone);
    for (let key in data) {
        const element = data [key];
        if (element != undefined && element.preset != undefined && characteristicsIndexes[key] != undefined) {
            const indexes = characteristicsIndexes[key];
            executeClient("client.characters.customization.UpdateCharacteristic", gender, indexes.xindex, element.x, indexes.yindex, element.y);
        } else if (element != undefined && element.color != undefined && element.opacity != undefined) {
            let appearance = Array.isArray(appearances[key]) ? appearances[key] : appearances[key][gender];
            executeClient("client.characters.customization.UpdateAppearance", gender, key, appearance[element.id].id, element.color, element.opacity);
        } else if (["head", "tops", "legs", "shoes"].includes (key)) {            
            let cloth = clothes[gender][key][element];
            executeClient("client.characters.customization.UpdateClothes", gender, key, cloth.drawable, cloth.texture);
        }
    }
}

export const updateParents = (gender, mother, father) => {
    customizations[selectIndex][gender].motherId = mother;
    customizations[selectIndex][gender].fatherId = father;
    if (init)
        executeClient("client.characters.customization.UpdateParents", gender, parents[false][mother].gtaId, parents[true][father].gtaId);
    updateCustomizationData();
}

export const updateShapeMix = (gender, shapemix) => {
    customizations[selectIndex][gender].shapeMix = shapemix;
    executeClient("client.characters.customization.UpdateShapeMix", gender, shapemix);
    updateCustomizationData();
}

export const updateSkinTone = (gender, skintone) => {
    customizations[selectIndex][gender].skinTone = skintone;
    executeClient("client.characters.customization.UpdateSkinTone", gender, skintone);
    updateCustomizationData();
}

export const updateCharacteristic = (gender, key, preset, xvalue, yvalue) => {
    customizations[selectIndex][gender][key] = { preset: preset, x: xvalue, y: yvalue };
    const indexes = characteristicsIndexes[key];

    if (init)
        executeClient("client.characters.customization.UpdateCharacteristic", gender, indexes.xindex, xvalue, indexes.yindex, yvalue);

    updateCustomizationData();
}

export const updateAppearance = (gender, key, id, color, opacity) => {
    customizations[selectIndex][gender][key] = { id: id, color: color, opacity: opacity };
    let appearance = Array.isArray(appearances[key]) ? appearances[key] : appearances[key][gender];

    if (init)
        executeClient("client.characters.customization.UpdateAppearance", gender, key, appearance[id].id, color, opacity);

    updateCustomizationData();
}

export const updateClothes = (gender, key, newcloth) => {
    customizations[selectIndex][gender][key] = newcloth;
    let cloth = clothes[gender][key][newcloth];

    if (init)
        executeClient("client.characters.customization.UpdateClothes", gender, key, cloth.drawable, cloth.texture);

    updateCustomizationData();
}

export const FirstName = writable("");
export const updateFirstName = (value) => {
    const gender = customizations[selectIndex].gender;
    customizations[selectIndex][gender].FirstName = value;
    updateName();
}

export const LastName = writable("");
export const updateLastName = (value) => {
    const gender = customizations[selectIndex].gender;
    customizations[selectIndex][gender].LastName = value;
    updateName();
}

window.initCustomizations = async () => {
    for (let i = 0; i < maxSlots; i++) {
        if (chars [i] <= -1) {
            updateIndex (i);
            CreateNewCustomization (false);
            CreateNewCustomization (true);
            updateGender (true);
        }
    }
    
    init = true;

    await window.wait(50);
    
    for (let i = 0; i < maxSlots; i++) {
        if (chars [i] <= -1) {
            updateIndex (i);
            await window.wait(50);
            initCustom (i);
            await window.wait(50);
        }
    }

    await window.wait(100);
    
    updateGender (true);

    executeClient("client.characters.initChars");
}