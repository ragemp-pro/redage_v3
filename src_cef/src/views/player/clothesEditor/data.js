export const clothesData = [
    {
        name: "Маски",
        type: "masks",
        props: false,
        id: 1,
    },
    {
        name: "Прически",
        type: "hair",
        props: false,
        id: 2,
    },
    {
        name: "Аксессуары",
        type: "accessories",
        props: false,
        id: 7,
    },
    {
        name: "Браслеты",
        type: "bracelets",
        props: true,
        id: 7,
    },
    {
        name: "На уши",
        type: "ears",
        props: true,
        id: 2,
    },
    {
        name: "На глаза",
        type: "glasses",
        props: true,
        id: 1,
    },
    {
        name: "На голову",
        type: "hats",
        props: true,
        id: 0,
    },
    {
        name: "На ноги",
        type: "legs",
        props: false,
        id: 4,
    },
    {
        name: "Ботинки",
        type: "shoes",
        props: false,
        id: 6,
    },
    {
        name: "Верх",
        type: "tops",
        props: false,
        id: 11,
    },
    {
        name: "Часы",
        type: "watches",
        props: true,
        id: 6,
    },
    {
        name: "На спину",
        type: "bags",
        props: false,
        id: 5,
    },
    {
        name: "Undershirts",
        type: "undershirts",
        props: false,
        id: 8,
    },
    {
        name: "Броники",
        type: "bodyarmors",
        props: false,
        id: 9,
    },
    {
        name: "Декали",
        type: "decals",
        props: false,
        id: 10,
    },
]

export const clothesDataToName = {
    undershirts: {
        name: "Маски",
        type: "undershirts",
        props: false,
        id: 8,
    },
    torsos: {
        name: "Маски",
        type: "torsos",
        props: false,
        id: 3,
    },
}

export const MaxClothesComponent = {
    "men": {
        ["bracelets"]: 8,
        ["ears"]: 40,
        ["glasses"]: 33,
        ["watches"]: 40,
        ["bags"]: 99,

        ["hats"]: 155,
        ["hair"]: 74,
        ["torsos"]: 183,
        ["legs"]: 137,
        ["shoes"]: 101,
        ["accessories"]: 151,
        ["bodyarmors"]: 55,
        ["tops"]: 381,
        ["undershirts"]: 183,
        ["masks"]: 194,
        ["decals"]: 119
    },
    "women": {
        ["bracelets"]: 15,
        ["ears"]: 21,
        ["glasses"]: 35,
        ["watches"]: 29,
        ["bags"]: 99,

        ["hats"]: 154,
        ["hair"]: 78,
        ["torsos"]: 228,
        ["legs"]: 144,
        ["shoes"]: 105,
        ["accessories"]: 120,
        ["bodyarmors"]: 55,
        ["tops"]: 399,
        ["undershirts"]: 221,
        ["masks"]: 195,
        ["decals"]: 127
    }
}

export const UndershortData = {
    "men": [
        15,
        15,
        0,
        2
    ],
    "women": [
        15,
        15,
        0,
        1
    ]
}

export const UndershortName = [
    "Не может быть вверхом",
    "полностью закрыта",
    "Полностью растегнута",
    "растегнута на половину",
]

export const TorsosData = {
    "men": [
        0,
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9,
        10,
        11,
        12,
        13,
        14,
        15,
        112,
        113,
        114
    ],
    "women": [
        0,
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9,
        10,
        11,
        12,
        13,
        14,
        15,
        129,
        130,
        131,
        153,
        161
    ]
}