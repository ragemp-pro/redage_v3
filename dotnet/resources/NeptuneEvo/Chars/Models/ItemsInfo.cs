using Redage.SDK;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using Newtonsoft.Json;

namespace NeptuneEvo.Chars.Models
{
    #region Тип
    public enum newItemType
    {
        /// <summary>
        /// Ошибка
        /// </summary>
        None,
        /// <summary>
        /// Проверка на активные предложения
        /// </summary>
        Clothes,
        /// <summary>
        /// Успешно
        /// </summary>
        Weapons,
        /// <summary>
        /// Успешно
        /// </summary>
        MeleeWeapons,
        /// <summary>
        /// Успешно
        /// </summary>
        Ammo,
        /// <summary>
        /// Успешно
        /// </summary>
        Alco,
        /// <summary>
        /// Успешно
        /// </summary>
        Eat,
        /// <summary>
        /// Успешно
        /// </summary>
        Water,
        /// <summary>
        /// Успешно
        /// </summary>
        Modification,
        /// <summary>
        /// Успешно
        /// </summary>
        Cases,
    }
    #endregion
    #region Объекты
    public enum ItemId
    {
        Mask = -1, // Маска
        Ears = -2, // На ухо
        Gloves = -3, // Перчатки
        Leg = -4, // Штанишки
        Bag = -5, // Рюкзачок
        Feet = -6, // Обуточки 
        Jewelry = -7, // Аксессуарчики всякие там
        Undershit = -8, // Рубашечки
        BodyArmor = -9, // Бронька
        Decals = -10, // Вообще хер пойми что это
        Top = -11, // Верх
        Hat = -12, // Шляпы
        Glasses = -13, // Очочки
        Watches = -14, // Часы
        Bracelets = -15, // Браслеты

        Debug = 0,
        BagWithMoney = 12,// Сумка с деньгами
        Material = 13,    // Материалы
        Drugs = 14,       // Наркота
        BagWithDrill = 15,// Сумка с дрелью
        HealthKit = 1,    // Аптечка
        GasCan = 2,       // Канистра
        Crisps = 3,       // Чипсы
        Beer = 4,         // Пиво
        Pizza = 5,        // Пицца
        Burger = 6,       // Бургер
        HotDog = 7,       // Хот-Дог
        Sandwich = 8,     // Сэндвич
        eCola = 9,        // Кока-Кола
        Sprunk = 10,      // Спрайт
        Lockpick = 11,    // Отмычка для замка
        ArmyLockpick = 16,// Военная отмычка
        Pocket = 17,      // Мешок
        Cuffs = 18,       // Стяжки
        CarKey = 19,      // Ключи от личной машины
        Present = 40,     // Подарок
        KeyRing = 41,     // Связка ключей


        /* Drinks */

        RusDrink1 = 20,
        RusDrink2 = 21,
        RusDrink3 = 22,
        YakDrink1 = 23,
        YakDrink2 = 24,
        YakDrink3 = 25,
        LcnDrink1 = 26,
        LcnDrink2 = 27,
        LcnDrink3 = 28,
        ArmDrink1 = 29,
        ArmDrink2 = 30,
        ArmDrink3 = 31,


        /* Weapons */
        /* Pistols */

        Pistol = 100,
        CombatPistol = 101,
        Pistol50 = 102,
        SNSPistol = 103,
        HeavyPistol = 104,
        VintagePistol = 105,
        MarksmanPistol = 106,
        Revolver = 107,
        APPistol = 108,
        FlareGun = 110,
        DoubleAction = 111,
        PistolMk2 = 112,
        SNSPistolMk2 = 113,
        RevolverMk2 = 114,

        /* SMG */

        MicroSMG = 115,
        MachinePistol = 116,
        SMG = 117,
        AssaultSMG = 118,
        CombatPDW = 119,
        MG = 120,
        CombatMG = 121,
        Gusenberg = 122,
        MiniSMG = 123,
        SMGMk2 = 124,
        CombatMGMk2 = 125,

        /* Rifles */

        AssaultRifle = 126,
        CarbineRifle = 127,
        AdvancedRifle = 128,
        SpecialCarbine = 129,
        BullpupRifle = 130,
        CompactRifle = 131,
        AssaultRifleMk2 = 132,
        CarbineRifleMk2 = 133,
        SpecialCarbineMk2 = 134,
        BullpupRifleMk2 = 135,
        MilitaryRifle = 266,

        /* Sniper */

        SniperRifle = 136,
        HeavySniper = 137,
        MarksmanRifle = 138,
        HeavySniperMk2 = 139,
        MarksmanRifleMk2 = 140,

        /* Shotguns */

        PumpShotgun = 141,
        SawnOffShotgun = 142,
        BullpupShotgun = 143,
        AssaultShotgun = 144,
        Musket = 145,
        HeavyShotgun = 146,
        DoubleBarrelShotgun = 147,
        SweeperShotgun = 148,
        PumpShotgunMk2 = 149,

        /* NEW WEAPONS */
        RayPistol = 150,
        CeramicPistol = 151,
        NavyRevolver = 152,
        RayCarbine = 153,
        GrenadeLauncher = 154,
        RPG = 155,
        Minigun = 156,
        Firework = 157,
        Railgun = 158,
        HomingLauncher = 159,
        GrenadeLauncherSmoke = 160,
        CompactGrenadeLauncher = 161,
        Widowmaker = 162,

        /* MELEE WEAPONS */

        StunGun = 109,
        Knife = 180,
        Nightstick = 181,
        Hammer = 182,
        Bat = 183,
        Crowbar = 184,
        GolfClub = 185,
        Bottle = 186,
        Dagger = 187,
        Hatchet = 188,
        KnuckleDuster = 189,
        Machete = 190,
        Flashlight = 191,
        SwitchBlade = 192,
        PoolCue = 193,
        Wrench = 194,
        BattleAxe = 195,

        /* Ammo */

        PistolAmmo = 200,
        SMGAmmo = 201,
        RiflesAmmo = 202,
        SniperAmmo = 203,
        ShotgunsAmmo = 204,

        /* NEW */

        Snowball = 205,

        /* NEW */
        cVarmod = 206,
        cClip = 207,
        cSuppressor = 208,
        cScope = 209,
        cMuzzlebrake = 210,
        cBarrel = 211,
        cFlashlight = 212,
        cGrip = 213,
        cCamo = 214,

        HalloweenCoin = 215,

        Firework1 = 216,
        Firework2 = 217,
        Firework3 = 218,
        Firework4 = 219,

        CarCoupon = 220,
        MerryChristmasCoin = 221,

        Bear = 222, 

        Note = 223,
        LoveNote = 224,

        Vape = 225,
        Rose = 226,
        Barbell = 227,
        Binoculars = 228,
        Bong = 229,
        Umbrella = 230,
        Camera = 231,
        Microphone = 232,
        Guitar = 233,

        Pickaxe1 = 234,
        Pickaxe2 = 235,
        Pickaxe3 = 236,
        Coal = 237,
        Iron = 238,
        Gold = 239,
        Sulfur = 240,
        Emerald = 241,
        Ruby = 242,

        Radio = 243,

        WorkAxe = 244,
        WoodOak = 245,
        WoodMaple = 246,
        WoodPine = 247,

        Boombox = 248,
        Hookah = 249,

        Case0 = 250,
        Case1 = 251,
        Case2 = 252,
        Case3 = 253,
        Case4 = 254,
        Case5 = 255,
        Case6 = 256,
        Case7 = 257,
        Case8 = 258,
        Case9 = 259,
        Case10 = 260,
        Case11 = 261,
        Case12 = 262,
        Case13 = 265,
        Case14 = 268,
        Case15 = 281,
        Case16 = 282,
        Case17 = 283,
        Case18 = 284,
        Case19 = 285,
        Case20 = 286,
        Case21 = 287,

        Ball = 263,
        SummerCoin = 264,
        CandyCane = 267,
        Qr = 269,
        QrFake = 270,
        SimCard = 271,
        VehicleNumber = 272,
        Bint = 273,
        Cocaine = 274,
        Rub100 = 275,
        Rub200 = 276,
        Rub500 = 277,
        Rub1000 = 278,
        
        RadioInterceptor = 279,
        Epinephrine = 280,
        AppleCoin = 288,
        Fire = 289,
        Matras = 290,
        Tent = 291,
        Lezhak = 292,
        Towel = 293,
        Flag = 294,
        Barrell = 295,
        Surf = 296,
        Vedro = 297,
        Flagstok = 298,
        Tenttwo = 299,
        Polotence = 300,
        Beachbag = 301,
        Zontik = 302,
        Zontiktwo = 303,
        Zontikthree = 304,
        Closedzontik = 305,
        Vball = 306,
        Bball = 307,
        Boomboxxx = 308,
        Table = 309,
        Tabletwo = 310,
        Tablethree = 311,
        Tablefour = 312,
        Chair = 313,
        Chairtwo = 314,
        Chaierthree = 315,
        Chaierfour = 316,
        Chairtable = 317,
        Korzina = 318,
        Light = 319,
        Alco = 320,
        Alcotwo = 321,
        Alcothree = 322,
        Alcofour = 323,
        Cocktail = 324,
        Cocktailtwo = 325,
        Fruit = 326,
        Fruittwo = 327,
        Packet = 328,
        Buter = 329,
        Patatoes = 330,
        Coffee = 331,
        Podnosfood = 332,
        Bbqtwo = 333,
        Bbq = 334,
        Vaza = 335,
        Flagwtokk = 336,
        Flagau = 337,
        Flagbr = 338,
        Flagch = 339,
        Flagcz = 340,
        Flageng = 341,
        Flageu = 342,
        Flagfr = 343,
        Flagger = 344,
        Flagire = 345,
        Flagisr = 346,
        Flagit = 347,
        Flagjam = 348,
        Flagjap = 349,
        Flagmex = 350,
        Flagnet = 351,
        Flagnig = 352,
        Flagnorw = 353,
        Flagpol = 354,
        Flagrus = 355,
        Flagbel = 356,
        Flagscot = 357,
        Flagscr = 358,
        Flagslovak = 359,
        Flagslov = 360,
        Flagsou = 361,
        Flagspain = 362,
        Flagswede = 363,
        Flagswitz = 364,
        Flagturk = 365,
        Flaguk = 366,
        Flagus = 367,
        Flagwales = 368,
        Flagfin = 369,
        Flowerrr = 370,
        Konus = 371,
        Konuss = 372,
        Otboynik1 = 373,
        Otboynik2 = 374,
        Dontcross = 375,
        Stop = 376,
        NetProezda = 377,
        Kpp = 378,
        Zabor1 = 379,
        Zabor2 = 380,
        Airlight = 381,
        Camera1 = 382,
        Camera2 = 383,
        TacticalRifle = 384,
        PrecisionRifle = 385,
        CombatShotgun = 386,
        HeavyRifle = 387,
        NeonStick = 388,
        GlowStick = 389,
        Giftcoin = 390,
        CombatRifle = 391,
        Glock = 392,

    }
    #endregion
    /// <summary>
    /// Данные пользователя
    /// </summary>
    public class ItemsInfo
    {
        #region Свойства

        /// <summary>
        /// Колличество
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Колличество
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Колличество
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// Колличество
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Дополнительная дата
        /// </summary>
        [JsonIgnore]
        public uint Model { get; set; }
        /// <summary>
        /// Дополнительная дата
        /// </summary>
        public int Stack { get; set; }
        /// <summary>
        /// Дополнительная дата
        /// </summary>
        [JsonIgnore]
        public Vector3 PosOffset { get; set; }
        /// <summary>
        /// Дополнительная дата
        /// </summary>
        [JsonIgnore]
        public Vector3 RotOffset { get; set; }
        /// <summary>
        /// Дополнительная дата
        /// </summary>
        public newItemType functionType { get; set; }

        #endregion

        /// <summary>
        /// Конструктор данных пользователя
        /// </summary>
        /// <param name="Name"><see cref="Name"/></param>
        /// <param name="Description"><see cref="Description"/></param>
        /// <param name="Icon"><see cref="Icon"/></param>
        /// <param name="Type"><see cref="Type"/></param>
        /// <param name="Model"><see cref="Model"/></param>
        /// <param name="Stack"><see cref="Stack"/></param>
        /// <param name="PosOffset"><see cref="PosOffset"/></param>
        /// <param name="RotOffset"><see cref="RotOffset"/></param>
        /// <param name="functionType"><see cref="functionType"/></param>
        public ItemsInfo(string Name, string Description, string Icon, string Type, uint Model, int Stack, Vector3 PosOffset, Vector3 RotOffset, newItemType functionType)
        {
            this.Name = Name;
            this.Description = Description;
            this.Icon = Icon;
            this.Type = Type;
            this.Model = Model;
            this.Stack = Stack;
            this.PosOffset = PosOffset;
            this.RotOffset = RotOffset;
            this.functionType = functionType;
        }
    }
}
