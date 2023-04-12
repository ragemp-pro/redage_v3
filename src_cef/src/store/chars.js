import { writable } from 'svelte/store';

window.charStore = {};

export const charData = writable({
    UUID: 0,
    CreateDate: 0,
    Name: "Nikita",
    Gender: true,
    LVL: 1,
    EXP: 1,
    Vip: "123",
    Status: 0,//Status
    Sim: 0,
    Work: 0,
    Lic: [1,0,0,0,0,0,0,0,0],

    Warns: 0,

    Money: 0,

    Bank: 0,
    BankMoney: 0,

    Fraction: 1,
    FractionLVL: 6,

    HouseId: 0,
    HouseCash: 0,
    HouseCopiesHour: 0,
    HousePaid: 0,
    HouseType: 0,
    MaxCars: 0,

    BizId: 0,
    BizCash: 0,
    BizCopiesHour: 0,
    BizPaid: 0,

    Time: {
        TotalTime: 0,
        Day: 0,
        TodayTime: 0,
        Month: 0,
        MonthTime: 0,
        Year: 0,
        YearTime: 0,
    },

    Kills: 0,
    Deaths: 0,
    EarnedMoney: 0,
    EatTimes: 0,
    Revived: 0,
    Handshaked: 0,

    IsLeader: false,

    IsMute: false,
    WeddingName: "Source",
    Licenses: [0, 0, 0, 0, 0, 0, 0, 0, 0],

    JobsSkills: [
        {
            name: "Электрик",
            max: 15000,
            nextLevel: 1000,
            currentLevel: 0,
            current: 0
        },
        {
            name: "Газонокосильщик",
            max: 40000,
            nextLevel: 1000,
            currentLevel: 0,
            current: 0
        },
        {
            name: "Почтальон",
            max: 4000,
            nextLevel: 1000,
            currentLevel: 0,
            current: 0
        },
        {
            name: "Таксист",
            max: 1000,
            nextLevel: 5000,
            currentLevel: 0,
            current: 0
        },
        {
            name: "Водитель автобуса",
            max: 70000,
            nextLevel: 1000,
            currentLevel: 0,
            current: 0
        },
        {
            name: "Автомеханик",
            max: 250,
            nextLevel: 100,
            currentLevel: 0,
            current: 0
        },
        {
            name: "Дальнобойщик",
            max: 700,
            nextLevel: 500,
            currentLevel: 0,
            current: 0
        },
        {
            name: "Инкассатор",
            max: 3000,
            nextLevel: 1000,
            currentLevel: 0,
            current: 0
        }
    ]
});
window.charStore.charData = (value) => {
    value = JSON.parse (value);
    charData.set (value);
}
let localCharData = {};
charData.subscribe(value => {
	localCharData = value;
}); 

window.charStore.updateNameWithJSON = (name, value) => {    
    value = JSON.parse (value);
    if (localCharData[name] === value) return;
    localCharData[name] = value;
    charData.set (localCharData);
}

export const charUUID = writable(0);
window.charStore.charUUID = (value) => charUUID.set (value);

export const charName = writable("");
window.charStore.charName = (value) => charName.set (value);

export const charGender = writable(false);
window.charStore.charGender = (value) => charGender.set (value);

export const charMoney = writable(0);
window.charStore.charMoney = (value) => charMoney.set (value);

export const charBankMoney = writable(0);
window.charStore.charBankMoney = (value) => charBankMoney.set (value);

export const charWorkID = writable(0);
window.charStore.charWorkID = (value) => charWorkID.set (value);

export const charIsLeader = writable(false);
window.charStore.charIsLeader = (value) => charIsLeader.set (value);

export const charFractionID = writable(0);
window.charStore.charFractionID = (value) => charFractionID.set (value);

export const charFractionLVL = writable(0);
window.charStore.charFractionLVL = (value) => charFractionLVL.set (value);

export const charOrganizationID = writable(0);
window.charStore.charOrganizationID = (value) => charOrganizationID.set (value);

export const charEXP = writable(1);
window.charStore.charEXP = (value) => charEXP.set (value);

export const charLVL = writable(1);
window.charStore.charLVL = (value) => charLVL.set (value);

export const charWanted = writable(0);
window.charStore.charWanted = (value) => charWanted.set (value);

export const charIsPet = writable(false);
window.charStore.charIsPet = (value) => charIsPet.set (value);

export const charCreateDate = writable("");
window.charStore.charCreateDate = (value) => charCreateDate.set (value);

export const isOrgTable = writable(false);
window.charStore.isOrgTable = (value) => isOrgTable.set (value);

export const charSim = writable(-1);
window.charStore.charSim = (value) => charSim.set (value);