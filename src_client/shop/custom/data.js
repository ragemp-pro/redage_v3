const _ModIndexs = {
    /// <summary>
    /// Спойлер +
    /// </summary>
    Spoiler: 0,

    /// <summary>
    /// Передний бампер +
    /// </summary>
    FrontBumper: 1,

    /// <summary>
    /// Задний бампер +
    /// </summary>
    RearBumper: 2,

    /// <summary>
    /// Боковая юбка +
    /// </summary>
    SideSkirt: 3,

    /// <summary>
    /// Глушитель +
    /// </summary>
    Muffler: 4,

    /// <summary>
    /// Каркас безопасности
    /// </summary>
    Frame: 5,

    /// <summary>
    /// Решетка
    /// </summary>
    Lattice: 6,

    /// <summary>
    /// Капот
    /// </summary>
    Hood: 7,

    /// <summary>
    /// Крыло
    /// </summary>
    Wings: 8,

    /// <summary>
    /// Правое крыло
    /// </summary>
    RightWings: 9,

    /// <summary>
    /// Крыша
    /// </summary>
    Roof: 10,

    /// <summary>
    /// Двигатель
    /// </summary>
    Engine: 11,

    /// <summary>
    /// Тормоза
    /// </summary>
    Brakes: 12,

    /// <summary>
    /// Коробка передач
    /// </summary>
    Transmission: 13,

    /// <summary>
    /// Клаксон
    /// </summary>
    Horn: 14,

    /// <summary>
    /// Подвеска
    /// </summary>
    Suspension: 15,

    /// <summary>
    /// Броня
    /// </summary>
    Armor: 16,

    /// <summary>
    /// Турбо
    /// </summary>
    Turbo: 18,

    /// <summary>
    /// UtilShadowSilver
    /// </summary>
    UtilShadowSilver: 20,

    /// <summary>
    /// Неон
    /// </summary>
    Xenon: 22,

    /// <summary>
    /// Передние колеса
    /// </summary>
    FrontWheels: 23,

    /// <summary>
    /// Задние колеса (ТОЛЬКО ДЛЯ МОТОЦИКЛОВ)
    /// </summary>
    BackWheels: 24,

    /// <summary>
    /// Рамка номерных знаков
    /// </summary>
    Plateholders: 25,

    /// <summary>
    /// TrimDesign
    /// </summary>
    TrimDesign: 27,

    /// <summary>
    /// Украшения
    /// </summary>
    Ornaments: 28,

    /// <summary>
    /// DialDesign
    /// </summary>
    DialDesign: 30,

    /// <summary>
    /// Руль
    /// </summary>
    SteeringWheel: 33,

    /// <summary>
    /// Рычаг коробки передач
    /// </summary>
    ShiftLever: 34,

    /// <summary>
    /// Бляшки
    /// </summary>
    Plaques: 35,

    /// <summary>
    /// Гидравлика
    /// </summary>
    Hydraulics: 38,

    /// <summary>
    /// Увеличение
    /// </summary>
    Boost: 40,

    /// <summary>
    /// Винила
    /// </summary>
    Vinyls: 48,

    /// <summary>
    /// Цвет стекла
    /// </summary>
    WindowTint: 55,

    /// <summary>
    /// Номерные знаки
    /// </summary>
    NumberPlate: 62
}

export const ModIndexs = _ModIndexs;

const vehicleToWeapon = [
    mp.game.joaat("Brutus"),
    mp.game.joaat("Imperator"),
    mp.game.joaat("ZR380"),
    mp.game.joaat("Deathbike"),
    mp.game.joaat("Comet4"),
    mp.game.joaat("Savestra"),
    mp.game.joaat("Viseris"),
    mp.game.joaat("Revolter"),
    mp.game.joaat("Speedo4"),
    mp.game.joaat("Mule4"),
    mp.game.joaat("Pounder2"),
    mp.game.joaat("Issi4"),
    mp.game.joaat("vapidse"),
    mp.game.joaat("Havok"),
    mp.game.joaat("Seasparrow"),
    mp.game.joaat("Seasparrow2"),
];

export let vehicleComponentToUse = (vehicleHash, modType) => {


    if (!["Color1", "Color2", ModIndexs.WindowTint].includes(modType) && global.AirCarList.includes(vehicleHash))
        return false;

    if (modType === ModIndexs.Roof && vehicleToWeapon.includes(vehicleHash))
        return false;

    if (modType === ModIndexs.FrontWheels && [mp.game.joaat("journey"), mp.game.joaat("Buzzard2"), mp.game.joaat("Seasparrow"), mp.game.joaat("Havok"), mp.game.joaat("Seasparrow2"), mp.game.joaat("Supervolito"), mp.game.joaat("snowbike"),, mp.game.joaat("Frogger"), mp.game.joaat("Maverick")].includes(vehicleHash))
        return false;

    if (modType === ModIndexs.NumberPlate && [mp.game.joaat("Buzzard2"), mp.game.joaat("Seasparrow"), mp.game.joaat("Havok"), mp.game.joaat("Seasparrow2"), mp.game.joaat("Supervolito"), mp.game.joaat("Frogger"), mp.game.joaat("Maverick"), mp.game.joaat("Volatus"), mp.game.joaat("Swift")].includes(vehicleHash))
        return false;

    if (modType === ModIndexs.FrontBumper && [mp.game.joaat("Havok")].includes(vehicleHash))
        return false;

    if (modType === ModIndexs.Horn && [mp.game.joaat("Buzzard2"), mp.game.joaat("Seasparrow"), mp.game.joaat("Havok"), mp.game.joaat("Seasparrow2"), mp.game.joaat("Supervolito"), mp.game.joaat("Frogger"), mp.game.joaat("Maverick"), mp.game.joaat("Volatus"), mp.game.joaat("Swift")].includes(vehicleHash))
        return false;

    if (modType === ModIndexs.Xenon && [mp.game.joaat("Buzzard2"), mp.game.joaat("Seasparrow"), mp.game.joaat("Havok"), mp.game.joaat("Seasparrow2"), mp.game.joaat("Supervolito"), mp.game.joaat("Frogger"), mp.game.joaat("Maverick"), mp.game.joaat("Volatus"), mp.game.joaat("Swift")].includes(vehicleHash))
        return false;


    return true;
}

export const Components = [
    {//0
        title: translateText("Спойлер"),
        category: "Spoiler",
        desc: translateText("Обеспечивает лучшую управляемость, за счет давления воздуха на заднюю часть авто")
    },
    {//1
        title: translateText("Передний бампер"),
        category: "FrontBumper",
        desc: translateText("Позволяет установить на Ваш автомобиль спортивные бамперы")
    },
    {//2
        title: translateText("Задний бампер"),
        category: "RearBumper",
        desc: translateText("Позволяет установить на Ваш автомобиль спортивные бамперы")
    },
    {//3
        title: translateText("Пороги"),
        category: "SideSkirt",
        desc: ''
    },
    {//4
        title: translateText("Глушитель"),
        category: "Muffler",
        desc: translateText("Позволяет осуществить замену стандартного глушителя на более продвинутый")
    },
    {//5
        title: translateText("Каркас безопасности"),
        category: "Frame",
        desc: translateText("Позволяет установить защиту в салоне, как на раллийных авто")
    },
    {//6
        title: translateText("Решетка радиатора"),
        category: "Lattice",
        desc: translateText("Позволяет изменить оформление радиаторной решетки Вашего авто")
    },
    {//7
        title: translateText("Капот"),
        category: "Hood",
        desc: translateText("Позволяет изменить стиль капота и добавить воздухозаборники")
    },
    {
        title: translateText("Крыло"),
        category: "Wings",
        desc: translateText("Позволяет установить фирменные крылья на Ваше авто")
    },
    {
        title: translateText("Крыша"),
        category: "Roof",
        desc: translateText("Позволяет модифицировать крышу Вашего авто")
    },
    {//Доп меню
        title: translateText("Колёса"),
        category: "FrontWheels",
        desc: translateText("Позволяет изменить дизайн шин и поставить новые диски")
    },
    {//+
        title: translateText("Клаксон"),
        category: "Horn",
        desc: translateText("Вы можете заменить стоковый сигнал своего автомобиля на новый")
    },
    {//Доп меню
        title: translateText("Неоновые трубки"),
        category: "Xenon",
        desc: ''
    },
    {//+
        title: translateText("Цвет фар"),
        category: "Headlights",
        desc: ''
    },
    {
        title: translateText("Двигатель"),
        category: "Engine",
        desc: translateText("Увеличение мощности двигателя,засчет установки чип - тюнинга")
    },
    {
        title: translateText("Турбо"),
        category: "Turbo",
        desc: translateText("Позволяет увеличить динамику набора скорости Вашего ТС")
    },
    {
        title: translateText("Коробка передач"),
        category: "Transmission",
        desc: translateText("Сокращает время передачи крутящего момента от двигателя к колесам")
    },
    {
        title: translateText("Подвеска"),
        category: "Suspension",
        desc: translateText("Улучшает управляемость ТС за счет установки более жесткой подвески")
    },
    {
        title: translateText("Тормоза"),
        category: "Brakes",
        desc: translateText("Сокращает тормозной путь, за счет установки новых тормозных дисков")
    },
    {//+
        title: translateText("Тонировка"),
        category: "WindowTint",
        desc: ''
    },
    {
        title: translateText("Основной цвет"),
        category: "Color1",
        desc: translateText("Покраска основных деталей кузова")
    },
    {
        title: translateText("Дополнительный цвет"),
        category: "Color2",
        desc: translateText("Покраска мелких деталей кузова")
    },
    {//+
        title: translateText("Градиент"),
        category: "ColorAdditional",
        desc: translateText("Градиент позволяет сделать Ваш основной цвет уникальным, накладываясь на него")
    },
    {//+
        title: translateText("Покрытие"),
        category: "Cover",
        desc: ''
    },
    {//+
        title: translateText("Винилы"),
        category: "Vinyls",
        desc: translateText("Усовершенствуйте внешний вид своего авто путем нанесения на него винила")
    },
    {//+
        title: translateText("Номер"),
        category: "NumberPlate",
        desc: translateText("Позволяет изменить оформление Ваших номерных знаков")
    }
]

//Цена за начальный компонент (VehiclePrice / 100 * ComponentsPrice)
export const ComponentsPrice = {
    Spoiler: 5.0,
    FrontBumper: 5.0,
    RearBumper: 5.0,
    SideSkirt: 5.0,
    Muffler: 5.0,
    Frame: 5.0,
    Lattice: 5.0,
    Hood: 5.0,
    Wings: 5.0,
    Roof: 5.0,
    FrontWheels: 6.0,
    Horn: 4.0,
    Xenon: 10.0,
    Headlights: 10.0,
    Engine: 25.0,
    Turbo: 25.0,
    Transmission: 20.0,
    Suspension: 25.0,
    Brakes: 25.0,
    WindowTint: 15.0,
    Color1: 5.0,
    Color2: 5.0,
    ColorAdditional: 10.0,
    Cover: 10.0,
    Vinyls: 10.0,
    NumberPlate: 10.0,
}

export const ComponentsName = {
    Spoiler: translateText("Спойлер #"),
    FrontBumper: translateText("Передний бампер #"),
    RearBumper: translateText("Задний бампер #"),//
    SideSkirt: translateText("Боковая юбка #"),
    Muffler: translateText("Глушитель #"),
    Frame: translateText("Каркас безопасности #"),
    Lattice: translateText("Решетка радиатора #"),
    Hood: "Капот #",
    Wings: translateText("Крыло #"),
    Roof: translateText("Крыша #"),
    FrontWheels: translateText("Колёса #"),
    Horn: translateText("Клаксон #"),
    Xenon: "Неон #",//
    Headlights: translateText("Цвет фар #"),
    Engine: translateText("Двигатель #"),
    Turbo: translateText("Турбо #"),
    Transmission: translateText("Коробка передач #"),
    Suspension: translateText("Подвеска #"),
    Brakes: translateText("Тормоза #"),
    WindowTint: translateText("Тонировка #"),
    Color1: translateText("Основной цвет #"),
    Color2: translateText("Дополнительный цвет #"),
    ColorAdditional: "Градиент #",
    Cover: translateText("Покрытие #"),
    Vinyls: translateText("Винилы #"),
    NumberPlate: translateText("Номер #"),
}

export const ModUses = [
    _ModIndexs.Spoiler,//Спойлер
    _ModIndexs.FrontBumper,
    _ModIndexs.RearBumper,
    _ModIndexs.SideSkirt,
    _ModIndexs.Muffler,
    _ModIndexs.Frame,
    _ModIndexs.Lattice,
    _ModIndexs.Hood,
    _ModIndexs.Wings,
    _ModIndexs.Roof,
    //_ModIndexs.Engine,
    //_ModIndexs.Brakes,
    //_ModIndexs.Transmission,
    //_ModIndexs.Suspension,
    _ModIndexs.Armor,
    _ModIndexs.Vinyls
]

export const getModLabel = (modType, modIndex) => {
    switch(modType) {
        case _ModIndexs.SideSkirt:
            return `CMOD_SKI_${modIndex + 1}`;
        case _ModIndexs.Engine:
            return modIndex === 0 ? "collision_55wey9g" : `CMOD_ENG_${modIndex + 2}`;
        case _ModIndexs.Brakes:
            return `CMOD_BRA_${modIndex + 1}`;
        case _ModIndexs.Transmission:
            return `CMOD_GBX_${modIndex + 1}`;
        case _ModIndexs.Suspension:
            return modIndex === 3 ? "collision_84hts2y" : `CMOD_SUS_${modIndex + 1}`;
        case _ModIndexs.Armor:
            return `CMOD_ARM_${modIndex + 1}`;
    }
    return false;
}

export const getStockValue = (modType) => {
    switch (modType) {
        case _ModIndexs.FrontBumper:
            return "CMOD_BUM_0";
        case _ModIndexs.RearBumper:
            return "CMOD_BUM_3";
        case _ModIndexs.SideSkirt:
            return "CMOD_SKI_0";
        case _ModIndexs.Muffler:
            return "CMOD_EXH_0";
        case _ModIndexs.Frame:
            return "CMOD_DEF_RC";
        case _ModIndexs.Lattice:
            return "CMOD_GRL_0";
        case _ModIndexs.Hood:
            return "CMOD_BON_0";
        case _ModIndexs.Roof:
            return "CMOD_ROF_0";
        case _ModIndexs.Brakes:
            return "collision_9ld0k5x";
        case _ModIndexs.Transmission:
            return "collision_34vak0";
        case _ModIndexs.Suspension:
            return "CMOD_SUS_0";
        case _ModIndexs.Armor:
        case _ModIndexs.Spoiler:
        case _ModIndexs.Wings:
        case _ModIndexs.Vinyls:
        case _ModIndexs.FrontWheels:
            return "HO_NONE";
    }
    return false;
}

export const blockedModLabels = [ "WTD_V_COM_MG", "WT_V_AKU_MN" ];


export const OtherComponents = (modType) => {
    switch (modType) {
        case _ModIndexs.FrontWheels: {
            return [
                
                {
                    title: translateText("Назад"),
                    category: "back",
                    desc: ''
                },
                {
                    title: translateText("Дорогие"),
                    category: 7,
                    desc: ''
                },
                {
                    title: translateText("Лоурайдер"),
                    category: 2,
                    desc: ''
                },
                {
                    title: translateText("Маслкар"),
                    category: 1,
                    desc: ''
                },
                {
                    title: translateText("Вездеход"),
                    category: 3,
                    desc: ''
                },
                {
                    title: translateText("Спорт"),
                    category: 0,
                    desc: ''
                },
                {
                    title: translateText("Внедорожник"),
                    category: 4,
                    desc: ''
                },
                {
                    title: translateText("Тюнер"),
                    category: 5,
                    desc: ''
                },
               
            ]
            
        }
        case _ModIndexs.Horn: {
            return [
                {
                    title: translateText("Назад"),
                    category: "back",
                    desc: ''
                },
                {
                    title: translateText("Стандарт"),
                    category: 0,
                    desc: ''
                },
                {
                    title: translateText("Музыкальные"),
                    category: 1,
                    desc: ''
                },
                {
                    title: translateText("С повтором"),
                    category: 2,
                    desc: ''
                }

            ]
        }

        case _ModIndexs.Cover: {//+
            return [
                {
                    title: translateText("Назад"),
                    category: "back",
                    desc: ''
                },
                {
                    title: translateText("Убрать"),
                    category: -1,
                    desc: ''
                },
                {
                    title: "Normal",
                    category: 0,
                    desc: ''
                },
                {
                    title: "Metallic",
                    category: 1,
                    desc: ''
                },
                {
                    title: "Pearl",
                    category: 2,
                    desc: ''
                },
                {
                    title: "Matte",
                    category: 3,
                    desc: ''
                },
                {
                    title: "Metal",
                    category: 4,
                    desc: ''
                },
                {
                    title: "Chrome",
                    category: 5,
                    desc: ''
                },
            ];
        }
    }
    return false;
}

export const OtherCategory = (category, index) => {
    switch (category) {
        case "Xenon": {
            return [
                {
                    name: translateText("Нет"),
                    index: 0,
                },
                {
                    name: translateText("Передний"),
                    index: 1,
                },
                {
                    name: translateText("Назад"),
                    index: 2,
                },
                {
                    name: translateText("По бокам"),
                    index: 3,
                },
                {
                    name: translateText("Спереди и сзади"),
                    index: 4,
                },
                {
                    name: translateText("Спереди и по бокам"),
                    index: 5,
                },
                {
                    name: translateText("Сзади и по бокам"),
                    index: 6,
                },
                {
                    name: translateText("Спереди, сзади и по бокам"),
                    index: 7,
                },
            ]
        }
        case "Cover": {//+
            return [
                {
                    name: translateText("Убрать"),
                    index: -1
                },
                {
                    name: "Normal",
                    index: 0
                },
                {
                    name: "Metallic",
                    index: 1
                },
                {
                    name: "Pearl",
                    index: 2
                },
                {
                    name: "Matte",
                    index: 3
                },
                {
                    name: "Metal",
                    index: 4
                },
                {
                    name: "Chrome",
                    index: 5
                },
            ];
        }
        case "Horn": {
            const horns = [
                [ // Стандарт
                    { name: translateText("Стандартный клаксон"), index: -1, duration: 1960 },
                    { name: translateText("Клаксон грузовика"), index: 0, duration: 1000 },
                    { name: translateText("Полицейский клаксон"), index: 1, duration: 1000 },
                    { name: translateText("Клоунский клаксон"), index: 2, duration: 1000 },
                    { name: translateText("Клаксон 1"), index: 52, duration: 1000 },
                    { name: translateText("Клаксон 2"), index: 54, duration: 1000 },
                    { name: translateText("Клаксон 3"), index: 56, duration: 1000 }
                ],
                [ // Музыкальные
                    { name: translateText("Джаз-клаксон 1"), index: 24, duration: 2000 },
                    { name: translateText("Джаз-клаксон 2"), index: 25, duration: 2000 },
                    { name: translateText("Джаз-клаксон 3"), index: 26, duration: 1500 },
                    { name: "Нота - До", index: 16, duration: 1000 },
                    { name: translateText("Нота - Ре"), index: 17, duration: 1000 },
                    { name: translateText("Нота - Ми"), index: 18, duration: 1000 },
                    { name: translateText("Нота - Фа"), index: 19, duration: 1000 },
                    { name: translateText("Нота - Соль"), index: 20, duration: 1000 },
                    { name: translateText("Нота - Ля"), index: 21, duration: 1000 },
                    { name: translateText("Нота - Си"), index: 22, duration: 1000 },
                    { name: translateText("Нота - До (Выс.)"), index: 23, duration: 1000 },
                    { name: translateText("Классический клаксон 1"), index: 9, duration: 5500 },
                    { name: translateText("Классический клаксон 2"), index: 10, duration: 5500 },
                    { name: translateText("Классический клаксон 3"), index: 11, duration: 5500 },
                    { name: translateText("Классический клаксон 4"), index: 12, duration: 4500 },
                    { name: translateText("Классический клаксон 5"), index: 13, duration: 4500 },
                    { name: translateText("Классический клаксон 6"), index: 14, duration: 4500 },
                    { name: translateText("Классический клаксон 7"), index: 15, duration: 4500 },
                    { name: "Классический клаксон 8", index: 33, duration: 4000 },
                    { name: translateText("Музыкальный клаксон 1"), index: 3, duration: 3500 },
                    { name: translateText("Музыкальный клаксон 2"), index: 4, duration: 5500 },
                    { name: translateText("Музыкальный клаксон 3"), index: 5, duration: 4500 },
                    { name: translateText("Музыкальный клаксон 4"), index: 6, duration: 4500 },
                    { name: translateText("Музыкальный клаксон 5"), index: 7, duration: 4500 },
                    { name: translateText("Печальная труба"), index: 8, duration: 4500 }
                ],
                [ // С повтором
                    { name: translateText("Джаз-клаксон (повтор)"), index: 27, duration: 2500 },
                    { name: translateText("Классический 1 (повтор)"), index: 32, duration: 2500 },
                    { name: translateText("Классический 2 (повтор)"), index: 34, duration: 5000 },
                    { name: translateText("Сан-Андреас (повтор)"), index: 43, duration: 5500 },
                    { name: translateText("Либерти-Сити (повтор)"), index: 45, duration: 9500 },
                ]
            ];
            return horns [index];
        }
        case "WindowTint": {
            return [
                {
                    name: translateText("Нет"),
                    index: -1
                },
                {
                    name: translateText("Слабое затемнение"),
                    index: 3
                },
                {
                    name: translateText("Среднее затемнение"),
                    index: 2
                },
                {
                    name: translateText("Лимузин"),
                    index: 1
                },
            ];
        }
        case "NumberPlate": {
            return [
                {
                    name: translateText("Синий на белом - 1"),
                    index: 0
                },
                {
                    name: translateText("Жёлтый на чёрном"),
                    index: 1
                },
                {
                    name: translateText("Жёлтый на синем"),
                    index: 2
                },
                {
                    name: translateText("Синий на белом - 2"),
                    index: 3
                },
                {
                    name: translateText("Синий на белом - 3"),
                    index: 4
                },
            ];
        }
        case "Turbo": {
            return [
                {
                    name: translateText("Нет"),
                    index: -1
                },
                {
                    name: translateText("Турбо-тюнинг"),
                    index: 0
                },
            ];
        }
        case "Engine": {
            return [
                {
                    name: translateText("Стандартный"),
                    index: -1
                },
                {
                    name: translateText("Улучшенный 1"),
                    index: 0
                },
                {
                    name: translateText("Улучшенный 1"),
                    index: 1
                },
                {
                    name: translateText("Улучшенный 2"),
                    index: 2
                },
                {
                    name: translateText("Улучшенный 3"),
                    index: 3
                },
            ];
        }
        case "Brakes": {
            return [
                {
                    name: translateText("Стандартные"),
                    index: -1
                },
                {
                    name: translateText("Уличные тормоза"),
                    index: 0
                },
                {
                    name: translateText("Спортивные тормоза"),
                    index: 1
                },
                {
                    name: translateText("Гоночные тормоза"),
                    index: 2
                },
            ];
        }
        case "Transmission": {
            return [
                {
                    name: translateText("Стандартная"),
                    index: -1
                },
                {
                    name: translateText("Уличная коробка передач"),
                    index: 0
                },
                {
                    name: translateText("Спортивная коробка передач"),
                    index: 1
                },
                {
                    name: translateText("Гоночная коробка передач"),
                    index: 2
                },
            ];
        }
        case "Suspension": {

            return [
                {
                    name: translateText("Стандартная"),
                    index: -1
                },
                {
                    name: translateText("Простая подвеска"),
                    index: 0
                },
                {
                    name: translateText("Уличная подвеска"),
                    index: 1
                },
                {
                    name: translateText("Спортивная подвеска"),
                    index: 2
                },
                {
                    name: translateText("Гоночная подвеска"),
                    index: 3
                },
            ];
        }
    }

    return false;
}

export const PlacePos = [
    {
        Position: new mp.Vector3(-338.5271, -136.7135, 39.06718),
        Ratation: new mp.Vector3(0.07063885, -0.2866833, 285.2643)
    },
    {
        Position: new mp.Vector3(731.7056, -1088.896, 22.2262),
        Ratation: new mp.Vector3(0.003280999, -0.3730852, 269.6414)
    },
    {
        Position: new mp.Vector3(-1155.194, -2005.489, 13.24107),
        Ratation: new mp.Vector3(-0.1381256, 0.1076123, 165.1146)
    },
    {
        Position: new mp.Vector3(-212.5238, -1323.189, 30.94811),
        Ratation: new mp.Vector3(-0.2359019, 0.2521333, 130.1107)
    },
    {
        Position: new mp.Vector3(110.9264, 6626.196, 31.84457),
        Ratation: new mp.Vector3(0.2491093, 0.2717469, 46.90005)
    },
]
