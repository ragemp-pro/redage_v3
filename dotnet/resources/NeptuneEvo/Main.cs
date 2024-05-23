using GTANetworkAPI;
using NeptuneEvo.Handles;
using Redage.SDK;
using Database;
using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;
using MySqlConnector;
using Newtonsoft.Json;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Database;
using NeptuneEvo.Fractions;
using NeptuneEvo.GUI;
using NeptuneEvo.Houses;
using NeptuneEvo.Events;
using NeptuneEvo.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Mail;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Localization;
using NeptuneEvo.Accounts.Models;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Jobs.Models;
using NeptuneEvo.Organizations.Models;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.PedSystem.Pet.Models;
using NeptuneEvo.Players.Phone.Messages.Models;
using NeptuneEvo.Players.Popup.List.Models;
using NeptuneEvo.Table.Models;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;
using Redage.SDK.Models;
using Group = NeptuneEvo.Core.Group;
using Org.BouncyCastle.Asn1.X509;

namespace NeptuneEvo
{
    public class Main : Script
    {
        public static string Codename { get; } = "RedAge Classic";
        public static string Version { get; } = "v1.00.00";
        public static string Build { get; } = "#0000";
        public static string Full { get; } = $"{Codename} {Version} {Build}";
        public static DateTime StartDate { get; } = DateTime.Now;
        public static DateTime CompileDate { get; } = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;
        public static int WeekInfo { get; set; } = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

        public static ServerSettings ServerSettings = new ServerSettings();
        public static byte ServerNumber = ServerSettings.ServerId;
        public static MoneySettings MoneySettings = new MoneySettings();
        public static DonationsSettings DonateSettings = new DonationsSettings();
        public static LumberjackPrice LumberjackPrice = new LumberjackPrice();
        public static DonatePack DonatePack = new DonatePack();
        public static PricesSettings PricesSettings = new PricesSettings();
        public static void LoadServerSettings()
        {
            ServerSettings = Settings.ReadAsync("serverSettings", ServerSettings);
            ServerNumber = ServerSettings.ServerId;
            DonateSettings = Settings.ReadAsync("donationsSettings", DonateSettings);
            MoneySettings = Settings.ReadAsync("moneySettings", MoneySettings);
            LumberjackPrice = Settings.ReadAsync("lumberjackPrice", LumberjackPrice);
            DonatePack = Settings.ReadAsync("donatePack", DonatePack);
            
            
            //
            
            BusinessManager.AmmoPrices = Settings.ReadAsync("ammoPrices", BusinessManager.AmmoPrices);
            
            //
            
            PricesSettings = Settings.ReadAsync("pricesSettings", PricesSettings);

            var index = 0;
            foreach (var shopFurniture in FurnitureManager.NameModels.Values)
            {
                shopFurniture.Price = PricesSettings.FurtinurePrices[index];
                index++;
            }
            
            Manager.FractionDataMats[70].Price = $"{Main.PricesSettings.FireworkPrices[0]}$";
            Manager.FractionDataMats[71].Price = $"{Main.PricesSettings.FireworkPrices[1]}$";
            Manager.FractionDataMats[72].Price = $"{Main.PricesSettings.FireworkPrices[2]}$";
            Manager.FractionDataMats[73].Price = $"{Main.PricesSettings.FireworkPrices[3]}$";
            
            Manager.FractionDataMats[74].Price = $"{Main.PricesSettings.InstrumentPrices[0]}$";
            Manager.FractionDataMats[75].Price = $"{Main.PricesSettings.InstrumentPrices[1]}$";
            Manager.FractionDataMats[76].Price = $"{Main.PricesSettings.InstrumentPrices[2]}$";
            Manager.FractionDataMats[77].Price = $"{Main.PricesSettings.InstrumentPrices[3]}$";
            
            //Arenda

            var rentCarsPrice = Settings.ReadAsync("rentCarsPrice", new List<int>());

            var isUpdateRentCarsPrice = rentCarsPrice.Count != Rentcar.RentCarsData.Length;
            
            for (var i = 0; i < rentCarsPrice.Count; i++)
            {
                if (i >= Rentcar.RentCarsData.Length)
                    continue;

                Rentcar.RentCarsData[i].Price = rentCarsPrice[i];
            }
            
            if (isUpdateRentCarsPrice)
            {
                for (var i = 0; i < Rentcar.RentCarsData.Length; i++)
                {
                    if (rentCarsPrice.Count > i)
                        continue;
                    
                    rentCarsPrice.Add(Rentcar.RentCarsData[i].Price);
                }
                
                Settings.Save("rentCarsPrice", rentCarsPrice);
            }
            
            //
            var petsShop = Settings.ReadAsync("petsShop", new Dictionary<int, PetShop>());

            if (petsShop.Count == 0)
                Settings.Save("petsShop", PedSystem.Pet.Repository.PetsShop);
            else
                PedSystem.Pet.Repository.PetsShop = petsShop;

            //
            
            var jobsMinLvl = Settings.ReadAsync("jobsMinLvl", new SortedList<int, int>());//Jobs.WorkManager;

            if (jobsMinLvl.Count == 0)
                Settings.Save("jobsMinLvl", Jobs.WorkManager.JobsMinLvl);
            else
                Jobs.WorkManager.JobsMinLvl = jobsMinLvl;
                
            
            //
            
            Donate.DonatesPremiumData = Settings.ReadAsync("donatesPremiumData", Donate.DonatesPremiumData);

            
            //

            var isBlock = WhiteList.Logins.Count > 0;
            
            WhiteList.Logins = Settings.ReadAsync("whiteList", WhiteList.Logins);

            if (isBlock && WhiteList.Logins.Count == 0)
            {
                foreach (var foreachPlayer in RAGE.Entities.Players.All.Cast<ExtPlayer>())
                {
                    var foreachSessionData = foreachPlayer.GetSessionData();
                    if (foreachSessionData == null) 
                        continue;

                    var foreachAuntificationData = foreachSessionData.AuntificationData;
                    if (foreachAuntificationData == null || !foreachAuntificationData.IsBlockAuth)
                        continue;
                    
                    Trigger.ClientEvent(foreachPlayer, "client.init",
                        Main.ServerSettings.ServerId,
                        Main.ServerSettings.ServerName,
                        Main.DonateSettings.Multiplier,
                        Main.DonateSettings.Convert,
                        Main.ServerSettings.IsMerger);
                    
                    Trigger.SetTask(async () =>
                    {           
                        
                        
                        await Players.Connect.Repository.PlayerToAuntidication(foreachPlayer);
                    });
                }
            }
            else if (!isBlock && WhiteList.Logins.Count > 0)
            {
                foreach (var foreachPlayer in RAGE.Entities.Players.All.Cast<ExtPlayer>())
                {                    
                    var foreachSessionData = foreachPlayer.GetSessionData();
                    if (foreachSessionData == null) 
                        continue;

                    var foreachAuntificationData = foreachSessionData.AuntificationData;
                    if (foreachAuntificationData == null || !foreachAuntificationData.IsBlockAuth)
                        continue;
                    
                    WhiteList.Check(foreachPlayer, foreachAuntificationData.Login);
                }
            }


            //Manager.FractionDataMats = Settings.ReadAsync("fractionDataMats", Manager.FractionDataMats);//Jobs.WorkManager;
            
            //

            SaveServerSettings();
        }
        public static void SaveServerSettings()
        {
            Settings.Save("serverSettings", ServerSettings);
            Settings.Save("donationsSettings", DonateSettings);
            Settings.Save("moneySettings", MoneySettings);
            Settings.Save("pricesSettings", PricesSettings);
            Settings.Save("donatePack", DonatePack);
            Settings.Save("lumberjackPrice", LumberjackPrice);
            Settings.Save("donatesPremiumData", Donate.DonatesPremiumData);
            Settings.Save("fractionDataMats", Manager.FractionDataMats);
        }
        #region ECONOMYCONFIG
        public static float BusinessMinPrice = 0.95f;
        public static float BusinessMaxPrice = 1.75f;
        public static int DrugsPrice = 30;
        public static int[] BuswaysPayments = new int[6] { 4, 5, 4, 5, 5, 8 };
        public static int CollectorPayment = 7;
        public static int ElectricianPayment = 6;
        public static int PostalPayment = 6;
        public static int LawnmowerPayment = 4;
        public static int GangCarDelivery = 500;
        public static int MafiaCarDelivery = 500;
        public static int PoliceAward = 100;
        public static int MinGunLic = 2500;
        public static int MaxGunLic = 5000;
        public static int MinPMLic = 1500;
        public static int MaxPMLic = 10000;
        public static int TicketLimit = 5000;
        public static int MinHealLimit = 50;
        public static int MaxHealLimit = 500;
        public static int CaptureWin = 1000;
        public static int BizwarWin = 1000;
        public static int MafiaForBiz = 90;
        public static int GangForPoint = 90;
        public static int[] LicPrices = new int[6] { 200, 500, 1500, 10000, 20000, 20000 }; // мотоциклы, легковые машины, грузовые, водный, вертолёты, самолёты
        public static int HotelRent = 8;
        public static int SMSCost = 7;
        public static int AdSymbCost = 70;
        public static int EvacCar = 200;
        public static float AdEditorCost = 0.7f;
        public static int MinDice = 100;
        public static int MaxDice = 15000;
        public static int BlackMarketDrill = 2000;
        public static int BlackMarketLockPick = 200;
        public static int BlackMarketArmyLockPick = 700;
        public static int BlackMarketCuffs = 200;
        public static int BlackMarketPocket = 100;
        public static int BlackMarketWanted = 500;
        public static int BusPay = 3;
        public static int BlackMarketUnCuff = 200;
        public static int BlackMarketGunLic = 30000;
        public static int BlackMarketMedCard = 15000;
        public static int BlackRadioInterceptord = 20000;
        public static int BlackQrFake = 8000;
        
        #endregion

        public static bool AutoRestart = false;

        public static List<string> TodayUniqueHWIDs = new List<string>();
        public static int PlayersAtOnce = 0;

        // Characters
        public static List<int> UUIDs = new List<int>(); // characters UUIDs
        public static ConcurrentDictionary<string, int> RefCodes = new ConcurrentDictionary<string, int>(); // characters RefCodes
        public static ConcurrentDictionary<int, string> PlayerNames = new ConcurrentDictionary<int, string>(); // character uuid - character name
        public static ConcurrentDictionary<string, int> PlayerBankAccs = new ConcurrentDictionary<string, int>(); // character name - character bank
        public static ConcurrentDictionary<string, int> PlayerUUIDs = new ConcurrentDictionary<string, int>(); // character name - character uuid

        public static List<ExtPlayer> Characters = new List<ExtPlayer>();
        
        public static ConcurrentDictionary<int, ExtPlayer> PlayerIdToEntity = new ConcurrentDictionary<int, ExtPlayer>();
        public static ConcurrentDictionary<int, int> PlayerUUIDToPlayerId = new ConcurrentDictionary<int, int>();

        public static ConcurrentDictionary<int, int> SimCards = new ConcurrentDictionary<int, int>();

        public static DateTime NextFixcarPlane = DateTime.Now;
        public static DateTime NextFixcarVeh = DateTime.Now;
        public static DateTime NextFixcarPoliceVeh = DateTime.Now;
        public static DateTime NextFixcarSheriffVeh = DateTime.Now;
        public static DateTime NextFixcarFIBVeh = DateTime.Now;

        public static Dictionary<int, DateTime> NextCarRespawn = new Dictionary<int, DateTime>()
        {
            { 0, DateTime.Now },
            { 1, DateTime.Now },
            { 2, DateTime.Now },
            { 3, DateTime.Now },
            { 4, DateTime.Now },
            { 5, DateTime.Now },
            { 6, DateTime.Now },
            { 7, DateTime.Now },
            { 8, DateTime.Now },
            { 9, DateTime.Now },
            { 10, DateTime.Now },
            { 11, DateTime.Now },
            { 12, DateTime.Now },
            { 13, DateTime.Now },
            { 14, DateTime.Now },
            { 15, DateTime.Now },
            { 16, DateTime.Now },
            { 17, DateTime.Now },
            { 18, DateTime.Now },
        };
        

        public class BlipData
        {
            public int id { get; set; }
            public string name { get; set; }
            public Vector3 position { get; set; }
            public int color { get; set; }
            public bool shortRange { get; set; }
            public float scale { get; set; }
            public BlipData(int id1, string name1, Vector3 position1, int color1, bool shortRange1, float scale1 = 1.0f)
            {
                id = id1;
                name = name1;
                position = position1;
                color = color1;
                shortRange = shortRange1;
                scale = scale1;
            }
        }

        public static void CreateBlip(BlipData blipData)
        {
            NAPI.Blip.CreateBlip(blipData.id, blipData.position, blipData.scale, (byte)blipData.color, blipData.name, shortRange: blipData.shortRange);
        }
        // Accounts
        public static List<int> Media = new List<int>(); // MediaList
        public static ConcurrentDictionary<string, List<int>> Usernames = new ConcurrentDictionary<string, List<int>>(); // usernames
        public static List<ExtPlayer> PlayersOnLogin = new List<ExtPlayer>(); // client's accounts
        public static ConcurrentDictionary<string, string> LoginToEmail = new ConcurrentDictionary<string, string>(); // client's accounts
        
        public static readonly string[] stringDefaultBlock = {
            "даун", "daun", "д а у н", "d a u n", "d.a.u.n", "д.а.у.н",
            "пидр", "пiдр", "педер", "пидар", "пидор", "педик", "педрик", "педрил", "п и д а р", "п и д о р", "pidor", "pedik", "pidrila", "pidoras", "pidaras", "p i d o r", "pedor", "p.i.d.o.r", "п.и.д.о.р", "п.и.д.а.р",
            "нигер", "ниггер", "нига", "нигга", "ниgа", "ниggа", "негр", "нeгр", "nigg", "niga", "niger", "nigger", "negr", "n i g g a", "n i g a", "n e g r", "н.е.г.р", "н.и.г.е.р", "n.e.g.r",
            "majestic", "маджестик", "radmir", "радмир",
        };

        public static readonly string[] stringGlobalBlock = {
            "e6aл", "ebal", "eblan", "eбaть", "eбyч", "eбёт", "eблан",
            "fuck",
            "рукоблуд", "ссанина", "очко", "блядун", "вагина", "сука", "ебланище", "влагалище", "пердун", "дрочила", "пидор", "пизда", "туз", "малафья", "гомик", "мудила", "пилотка", "манда", "анус", "вагина", "путана", "пидрила", "шалава", "хуила", "машонка", "елда",
            "lox", "loh",
            "xyёв", "xyй", "xyя", "xуе", "xуй", "xую", "хуйня",
            "zaeb",
            "пизд", "ахуел", "ахуеть",
            "бздение", "бздеть", "бздех", "бздецы", "бздит", "бздло", "бзднуть", "бздун", "бздюха", "бздюшка", "бздюшко",
            "вафел", "вафлёр", "взъеб", "взьеб", "въеб", "выеб",
            "гавно", "гавню", "гамно", "гандон", "гнид", "говенка", "говеный", "говешка", "говна", "говне", "говни", "говно",
            "говню", "говня", "гондон",
            "дебил", "доебываться", "долбоеб", "долбоёб", "долбоящер", "дрисня", "дрист", "дроч", "даун", "daun", "д а у н", "d a u n", "d.a.u.n", "д.а.у.н",
            "дура",
            "е6ал", "е6ут", "ёбaн", "ебaт", "ебyч", "ебал", "ебан",
            "ебаш", "ебёт", "ебец", "ебик", "ебин", "ебис", "ебки", "ебла", "ебли",
            "ебло", "еблы", "ебош", "ебск", "ебун", "ебут", "ебуч",
            "ебыр", "елда",
            "жопа", "жопу", "говнят", "драчиват", "дрист", "задрот", "зае6", "заё6",
            "залуп",
            "пиздяч", "засер", "засир", "засрун",
            "злоеб",
            "идиот", "ипать",
            "курва",
            "лох", "лошар", "лошок", "лярва",
            "малафья", "минет",
            "млять", "мокрощелка", "мокрощёлка", "мразь", "мудak", "мудaк", "мудаг", "муде", "муди",
            "мудо",
            "хер","бздел", "бздеть", "говня", "дрист", "дроч", "наеб", "пизд",
            "срать", "хрен",
            "нехуй", "никуя",
            "обосра", "обсир", "объеб", "обьеб", "опездал", "опизде",
            "пизд", "ебись", "охуев",
            "охуел", "охуен", "охует", "охуит", "охуян", "очкун", "хуесос",
            "падла", "падонки", "падонок", "паскуда", "педер", "пидар", "пидор", "педик", "педрик", "педрил", "пезде", "пезди", "пезд", "п и д а р", "п и д о р", "pidor", "pedik", "pidrila", "pidoras", "pidaras", "p i d o r", "pedor", "p.i.d.o.r", "п.и.д.о.р", "п.и.д.а.р",
            "пердан", "пердеж", "перден", "пердет", "пердил", "перднут", "пёрднут", "пердух",
            "пи3д", "пиzде",
            "пидр", "пiдр", "педр", "пизда", "пизде",
            "пизди", "пиздо",
            "пизду", "пизды", "пиздю","пиздя",
            "письк", "писюн", "писюш", "подонки", "подонок", "ебнут", "ебень",
            "ёбыва", "поскуда", "срать", "потаскуха", "потаскуш", "похер",
            "придурок", "блядь", "проеб",
            "пиздел", "пиздеть",
            "раздолбай", "разъеб", "пиздяй",
            "сволота", "сволоч", "серун", "сирать", "соси", "саси", "пиздел", "пизди",
            "срака", "сраку", "сраный", "сранье", "срать", "срун", "ссака", "ссышь", "стерва", "суки",
            "сучара", "сучий", "сучка", "сучонок", "сучье","сцышь", "съебать", "сыкун",
            "трах",
            "ублюдок", "ебать", "уёбищ","уебк", "уёбк", "уебок", "уёбок", "срать", "ушлепок",
            "целка", "чмо", "чмырь", "шалав", "шлюх", "шлюш",
            "nahui", "нахуй", "нахуя",
            "нигер", "ниггер", "нига", "нигга", "ниgа", "ниggа", "негр", "нeгр", "nigg", "niga", "niger", "nigger", "negr", "n i g g a", "n i g a", "n e g r", "н.е.г.р", "н.и.г.е.р", "n.e.g.r",
            "продам", "вирты", "виртов", "majestic", "маджестик", "radmir", "радмир", "подслухин", "подслушан", "sliv",
            "sokol", "соколян", "zak"
        };
        public static readonly char[] stringBlock = { '\'', '@', '[', ']', ':', '"', '[', ']', '{', '}', '|', '`', '%', '\\' };

        public static string BlockSymbols(string check)
        {
            for (int i = check.IndexOfAny(stringBlock); i >= 0;)
            {
                check = check.Replace(check[i], ' ');
                i = check.IndexOfAny(stringBlock);
            }
            return check;
        }

        public static Random rnd = new Random();

        public static readonly string[] LicWords = new string[]
        {
            "A",
            "B",
            "C",
            "V",
            "LV",
            "LS",
            "G",
            "MED",
            "PM"
        };

        public class BonusCodesData
        {
            public ulong UsedTimes { get; set; }
            public ulong UsedLimit { get; set; }
            public string RewardMessage { get; set; }
            public uint RewardMoney { get; set; }
            public byte RewardExp { get; set; }
            public byte RewardVipLvl { get; set; }
            public ushort RewardVipDays { get; set; }
            public List<InventoryItemData> RewardItemsMale { get; set; }
            public List<InventoryItemData> RewardItemsFemale { get; set; }

            public BonusCodesData(ulong used, ulong limit, string msg, byte exp, uint money, byte vip, ushort days, List<InventoryItemData> items, List<InventoryItemData> items2)
            {
                UsedTimes = used;
                UsedLimit = limit;
                RewardMessage = msg;
                RewardExp = exp;
                RewardMoney = money;
                RewardVipLvl = vip;
                RewardVipDays = days;
                RewardItemsMale = items;
                RewardItemsFemale = items2;
            }
        }

        public static ConcurrentDictionary<string, BonusCodesData> BonusCodes = new ConcurrentDictionary<string, BonusCodesData>();

        public class PromoCodesData
        {
            public uint CreatorUUID { get; set; }
            public ulong UsedTimes { get; set; }
            public ulong RewardReceived { get; set; }
            public ulong RewardLimit { get; set; }
            public string RewardMessage { get; set; }
            public uint RewardMoney { get; set; }
            public byte RewardVipLvl { get; set; }
            public ushort RewardVipDays { get; set; }
            public List<InventoryItemData> RewardItems { get; set; }
            public double DonatePercent { get; set; }
            public string DonateLogin { get; set; }
            public ulong DonateReceivedByStreamer { get; set; }

            public PromoCodesData(uint uuid, ulong used, ulong received, ulong limit, string msg, uint money, byte vip, ushort days, List<InventoryItemData> items, double donate, string donatel, ulong donate2)
            {
                CreatorUUID = uuid;
                UsedTimes = used;
                RewardReceived = received;
                RewardLimit = limit;
                RewardMessage = msg;
                RewardMoney = money;
                RewardVipLvl = vip;
                RewardVipDays = days;
                RewardItems = items;
                DonatePercent = donate;
                DonateLogin = donatel;
                DonateReceivedByStreamer = donate2;
            }
        }

        public static ConcurrentDictionary<string, PromoCodesData> PromoCodes = new ConcurrentDictionary<string, PromoCodesData>();


        public static List<ExtPlayer> AllAdminsOnline = new List<ExtPlayer>();
        public static List<string> AdminSocials = new List<string>();
        public static List<string> MediaSocials = new List<string>();

        public static readonly nLog Log = new nLog("Main");

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                //if (ServerSettings.MoneyMultiplier >= 2) NAPI.Server.SetServerName($"{ServerName} | X{ServerSettings.MoneyMultiplier}"); // NOT WORKING AT 0.3.7
                //CreateBlip(new BlipData(621, "Quests", new Vector3(343.895447, -1399.03381, 32.509285), 1, true, 1f));
                
                /*CreateBlip(new BlipData(267, LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(-358.4868, -52.37103, 53.29886), 38, true, 0.5f));
                CreateBlip(new BlipData(267, LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(22.71732, -1737.653, 28.18297), 38, true, 0.5f));
                CreateBlip(new BlipData(267, LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(-1646.822, -228.72, 55.92), 38, true, 0.5f));
                CreateBlip(new BlipData(267, LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(-2025.384, -469.42, 11.42), 38, true, 0.5f));
                CreateBlip(new BlipData(267, LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(1188.66, -1548.76, 39.37), 38, true, 0.5f));
                CreateBlip(new BlipData(267, LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(-975.0394, -1474.377, 5.020052), 38, true, 0.5f));
                CreateBlip(new BlipData(267, LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(-39.57354, 211.735, 106.1348), 38, true, 0.5f));

                if (ServerNumber <= 1) // Поставлены только на Black'е и отображать на Test'овом
                {
                    CreateBlip(new BlipData(267, LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(-740.8164, -67.55, 41.75), 38, true, 0.5f));
                    CreateBlip(new BlipData(267, LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(-927.72, -164.84, 41.87), 38, true, 0.5f));
                    CreateBlip(new BlipData(267, LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(-482.2281, -612.549, 32.47), 38, true, 0.5f));
                    CreateBlip(new BlipData(267, LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(1032.372, -771.5231, 56.94999), 38, true, 0.5f));
                    CreateBlip(new BlipData(267, LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(1151.805, -477.1126, 65.16711), 38, true, 0.5f));
                    CreateBlip(new BlipData(267, LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(1230.796, -435.7309, 66.61465), 38, true, 0.5f));
                    CreateBlip(new BlipData(267, LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(1096.052, -335.8769, 66.10432), 38, true, 0.5f));
                    CreateBlip(new BlipData(267, LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(1142.26, -392.178, 65.92946), 38, true, 0.5f));
                    CreateBlip(new BlipData(267, LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(-2162.425, -399.813, 12.26453), 38, true, 0.5f));
                    CreateBlip(new BlipData(267, LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(-463.0861, -806.9484, 29.41866), 38, true, 0.5f));
                    CreateBlip(new BlipData(267, LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(-1245.905, -1413.595, 4.322968), 38, true, 0.5f));
                    CreateBlip(new BlipData(267, LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(373.6721, 279.976, 103.3404), 38, true, 0.5f));
                    CreateBlip(new BlipData(267, LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(281.2544, -194.9666, 61.57069), 38, true, 0.5f));
                    CreateBlip(new BlipData(267, LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(-204.1797, 308.1642, 96.94662), 38, true, 0.5f));
                    CreateBlip(new BlipData(267, LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(-354.1708, 286.2279, 84.74282), 38, true, 0.5f));
                }*/

                CreateBlip(new BlipData(304, LangFunc.GetText(LangType.Ru, DataName.Theather), new Vector3(683.739, 570.40, 130.46), 7, true));
                CreateBlip(new BlipData(455, LangFunc.GetText(LangType.Ru, DataName.Yacht), new Vector3(-2070.151, -1023.134, 11.9), 81, true));
                //CreateBlip(new BlipData(311, LangFunc.GetText(LangType.Ru, DataName.Gym), new Vector3(-1202.281, -1568.798, 3.488338), 4, true));
                
                CreateBlip(new BlipData(93, "Пляжные вечеринки", new Vector3(-1497.7688, -1484.525, 5.7608714), 43, true)); // todo LangFunc
                CreateBlip(new BlipData(766, "Остров", new Vector3(5024.207, -5122.8962, 3.701595), 43, true)); // todo LangFunc
                CreateBlip(new BlipData(93, "Пляжные вечеринки", new Vector3(-1705.88, -970.7998, 8.648665), 43, true)); // todo LangFunc

                NAPI.Server.SetAutoRespawnAfterDeath(false);
                NAPI.Server.SetGlobalServerChat(false);
                NAPI.World.SetTime(DateTime.Now.Hour, 0, 0);

                using MySqlCommand cmd = new MySqlCommand()
                {
                    CommandText = "SELECT `login`,`socialclub`,`email`,`hwid`,`character1`,`character2`,`character3`,`characters` FROM `accounts`"
                };
                using DataTable result = MySQL.QueryRead(cmd);
                if (result != null)
                {
                    string login;
                    string socialclub;
                    string email;
                    string hwid;
                    List<int> uuids;
                    foreach (DataRow Row in result.Rows)
                    {
                        try
                        {
                            login = Convert.ToString(Row["login"]).ToLower();
                            socialclub = Convert.ToString(Row["socialclub"]);
                            email = Convert.ToString(Row["email"]);
                            hwid = Convert.ToString(Row["hwid"]);
                            try
                            {
                                uuids = JsonConvert.DeserializeObject<List<int>>(Convert.ToString(Row["characters"]));
                            }
                            catch
                            {
                                uuids = new List<int>() { -2, -2, -2, -2, -2, -2 };
                            }
                            Usernames.TryAdd(login, new List<int>
                            {
                                Convert.ToInt32(Row["character1"]),
                                Convert.ToInt32(Row["character2"]),
                                Convert.ToInt32(Row["character3"]),
                                uuids[0],
                                uuids[1],
                                uuids[2],
                                uuids[3],
                                uuids[4],
                                uuids[5]
                            });

                            LoginToEmail[login] = email;
                        }
                        catch (Exception e)
                        {
                            Log.Write($"onResourceStart Foreach#5 Exception: {e.ToString()}");
                        }
                    }
                    Log.Write($"Accounts loaded.", nLog.Type.Success);
                }
                else Log.Write("DB `accounts` return null result", nLog.Type.Warn);

                using MySqlCommand cmdCharacters = new MySqlCommand()
                {
                    CommandText = "SELECT `uuid`,`firstname`,`lastname`,`sim`,`lvl`,`exp`,`fraction`,`fractionlvl`,`money`,`bank`,`adminlvl`,`refcode` FROM `characters`"
                };

                var fractionPlayers = new List<List<object>>();
                var isFractionTable = true;
                
                using DataTable resultCharacters = MySQL.QueryRead(cmdCharacters);
                if (resultCharacters != null)
                {
                    int uuid;
                    string name;
                    string lastname;
                    int lvl;
                    int exp;
                    long money;
                    int adminlvl;
                    int bank;
                    int sim;
                    string fullname;
                    string socialclub;
                    string refcode;
                    foreach (DataRow Row in resultCharacters.Rows)
                    {
                        try
                        {
                            uuid = Convert.ToInt32(Row["uuid"]);
                            name = Convert.ToString(Row["firstname"]);
                            lastname = Convert.ToString(Row["lastname"]);
                            lvl = Convert.ToInt32(Row["lvl"]);
                            exp = Convert.ToInt32(Row["exp"]);

                            if (isFractionTable)
                            {
                                try
                                {
                                    var fraction = Convert.ToInt32(Row["fraction"]);
                                    var fractionlvl = Convert.ToInt32(Row["fractionlvl"]);
                                    
                                    if (fraction > 0)
                                    {
                                        fractionPlayers.Add(new List<object>
                                        {
                                            uuid,
                                            name,
                                            fraction,
                                            fractionlvl
                                        });
                                    }
                                }
                                catch
                                {
                                    isFractionTable = false;
                                }
                            }
                            
                            money = Convert.ToInt64(Row["money"]);
                            adminlvl = Convert.ToInt32(Row["adminlvl"]);
                            bank = Convert.ToInt32(Row["bank"]);
                            sim = Convert.ToInt32(Row["sim"]);
                            refcode = Convert.ToString(Row["refcode"]);
                            fullname = $"{name}_{lastname}";

                            if (!UUIDs.Contains(uuid)) UUIDs.Add(uuid);
                            if (refcode != null && !RefCodes.ContainsKey(refcode)) RefCodes.TryAdd(refcode, uuid);
                            
                            if (sim != -1)
                            {
                                Players.Phone.Sim.Repository.Add(sim);
                                if (!SimCards.ContainsKey(sim))
                                    SimCards.TryAdd(sim, uuid);
                            }
                            
                            if (!PlayerNames.ContainsKey(uuid)) PlayerNames.TryAdd(uuid, fullname);
                            if (!PlayerUUIDs.ContainsKey(fullname)) PlayerUUIDs.TryAdd(fullname, uuid);
                            if (!PlayerBankAccs.ContainsKey(fullname)) PlayerBankAccs.TryAdd(fullname, bank);
                            /*if (fraction != 0)
                            {
                                if (!Manager.AllMembers.ContainsKey(fraction))
                                    Manager.AllMembers.TryAdd(fraction, new List<Fr>());

                                Manager.AllMembers[fraction].Add(new Manager.MemberData
                                {
                                    UUID = uuid,
                                    Name = fullname,
                                    Id = fraction,
                                    Rank = fractionlvl
                                });
                            }*/
                            if (adminlvl > 0)
                            {
                                using MySqlCommand cmdAccounts = new MySqlCommand
                                {
                                    CommandText = "SELECT `socialclub` FROM `accounts` WHERE `character1`=@val0 OR `character2`=@val1 OR `character3`=@val2 OR `characters` LIKE @val3"
                                };
                                cmdAccounts.Parameters.AddWithValue("@val0", uuid);
                                cmdAccounts.Parameters.AddWithValue("@val1", uuid);
                                cmdAccounts.Parameters.AddWithValue("@val2", uuid);
                                cmdAccounts.Parameters.AddWithValue("@val3", $"%{uuid}%");

                                using DataTable result2 = MySQL.QueryRead(cmdAccounts);
                                if (result2 == null || result2.Rows.Count == 0) continue;
                                socialclub = Convert.ToString(result2.Rows[0]["socialclub"]);
                                if (!AdminSocials.Contains(socialclub)) AdminSocials.Add(socialclub);
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Write($"onResourceStart Foreach#1 Exception: {e.ToString()}");
                        }
                    }
                    Log.Write($"Characters loaded.", nLog.Type.Success);
                }
                else Log.Write("DB `characters` return null result", nLog.Type.Warn);

                if (isFractionTable && fractionPlayers.Count > 0)
                {
                    using var db = new ServerBD("MainDB");//В отдельном потоке
                    db.Characters
                        .Set(c => c.Fraction, 0)
                        .Set(c => c.Fractionlvl, 0)
                        .Update();
                    
                    foreach (var fractionData in fractionPlayers)
                    {
                        var uuid = Convert.ToInt32(fractionData[0]);
                        var name = fractionData[1].ToString();
                        var fraction = Convert.ToInt32(fractionData[2]);
                        var fractionlvl = Convert.ToInt32(fractionData[3]);
                        
                        db.Insert(new Fracranks
                        {
                            Uuid = uuid,
                            Name = name,
                            Id = fraction,
                            Rank = (sbyte) fractionlvl,
                            Avatar = "",
                            Access = "[]",
                            @lock = "[]",
                            Date = DateTime.Now,
                            LastLoginDate = DateTime.Now,
                            Time = "{}",
                            Tasks = "[]"
                        });
                    }
                    Log.Write($"Add Frasction.", nLog.Type.Success);
                }
                
                
                foreach (var number in Enum.GetValues(typeof(Players.Phone.Messages.Models.DefaultNumber)))
                    Players.Phone.Sim.Repository.Add((int)number);
                
                //
                SyncThread.PromoSync().Wait();
                //
                SyncThread.BonusSync().Wait();
                //
                Ban.Delete();

                
                int time = 3600 - (DateTime.Now.Minute * 60) - DateTime.Now.Second;

                Timers.StartOnce("paydayFirst", time * 1000, () =>
                {
                    Timers.Start("payday", 3600000, () =>
                    {
                        payDayTrigger(true);
                        //TankRoyale.StartEvent();
                        //if (AirDrop.AirDropEventStatus == 0) AirDrop.StartEvent(true, false);
                    }, true);
                    payDayTrigger(true);
                    //TankRoyale.StartEvent();
                    //if (AirDrop.AirDropEventStatus == 0) AirDrop.StartEvent(true, false);
                }, true);

                /*using MySqlCommand cmdOthervehicles = new MySqlCommand()
                {
                    CommandText = "SELECT * FROM `othervehicles`"
                };

                using DataTable resultOthervehicles = MySQL.QueryRead(cmdOthervehicles);
                if (resultOthervehicles != null)
                {
                    int type;
                    string number;
                    string name;
                    VehicleHash model;
                    Vector3 position;
                    Vector3 rotation;
                    int color1;
                    int color2;
                    int price;
                    CarInfo data;
                    foreach (DataRow Row in resultOthervehicles.Rows)
                    {
                        try
                        {
                            type = Convert.ToInt32(Row["type"]);
                            number = Row["number"].ToString();
                            name = Row["model"].ToString();
                            model = NAPI.Util.VehicleNameToModel(name);
                            position = JsonConvert.DeserializeObject<Vector3>(Row["position"].ToString());
                            rotation = JsonConvert.DeserializeObject<Vector3>(Row["rotation"].ToString());
                            color1 = Convert.ToInt32(Row["color1"]);
                            color2 = Convert.ToInt32(Row["color2"]);
                            price = Convert.ToInt32(Row["price"]);
                            data = new CarInfo(number, model, position, rotation, color1, color2, price, type, name);

                            switch (type)
                            {
                                //case 0:
                                //case 10:
                                //case 11:
                                //case 12:
                                //    Rentcar.CarInfos.Add(data);
                                //    break;
                                case 7:
                                    Jobs.Collector.CarInfos.Add(data);
                                    break;
                                case 8:
                                    Jobs.AutoMechanic.CarInfos.Add(data);
                                    break;
                                default:
                                    // Not supposed to end up here. 
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Write($"onResourceStart Foreach#8 Exception: {e.ToString()}");
                        }
                    }

                    Log.Write($"Job vehicles loaded.", nLog.Type.Success);
                }
                else Log.Write("DB `othervehicles` return null result", nLog.Type.Warn);*/

                Players.Phone.Tinder.Repository.Init();
                
                Configs.LoadFractionConfigs();
                Fractions.Ticket.OnResourceStart();
                Log.Write($"Fractions loaded.", nLog.Type.Success);
                Organizations.Manager.onResourceStart();
                Log.Write($"Organizations loaded.", nLog.Type.Success);

                Timers.Start("savedb", 1000 * (60 * 60), () => Admin.SaveServer(), true);
                Timers.Start("ClearCollect", 1000 * (60 * 30), () => GarbageCollector(), true);
                Timers.Start("playedMins", 1000 * 60, () => playedMinutesTrigger(), true);
                Timers.Start("envTimer", 1000, () => enviromentChangeTrigger(), true);
                MoneySystem.Lottery.OnResourceStart();

                var weatherId = 13;//config.TryGet<byte>("Weather", 1);
                var dateTime = DateTime.Now;
                World.Weather.Repository.Init(weatherId, dateTime.Hour, dateTime.Minute);
                
                MoneySystem.Donations.Start();
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }

        public static string GetLoginFromUUID(int uuid)
        {
            try
            {
                if (uuid < 0) return null;
                return Usernames.FirstOrDefault(u => u.Value.Contains(uuid)).Key;
            }
            catch (Exception e)
            {
                Log.Write($"ReturnLoginFromUUID Exception: {e.ToString()}");
                return null;
            }
        }
        #region Player
        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnected(ExtPlayer player, DisconnectionType type, string reason)
        {
            NeptuneEvo.Players.Disconnect.Repository.OnPlayerDisconnect(player, type, reason);
        }
        public static void SetUpEverything(ExtPlayer player)
        {
            if (player == null) return;
            NAPI.Task.Run(() =>
            {
                if (player == null) return;
                player.SetSharedData("FacialClipset", 0);
                player.SetSharedData("WalkStyle", 0);
                player.SetSharedData("isDeaf", false);
                player.SetSharedData("weaponComponents", "null");
                player.SetSharedData("leader", false);
                player.SetSharedData("organization", 0);
                player.SetSharedData("fraction", 0);
                player.SetSharedData("VoiceZone", 0);

                player.SetSharedData("PlayerAirsoftTeam", -1);
                player.SetSharedData("killsWeapon", 0);
                player.SetSharedData("weaponLevel", 0);

                //player.SetSharedData("mafiaGameRole", 0);
                //player.SetSharedData("mafiaGameNumber", 0);

            });
        }

        public static void HelloText(ExtPlayer player)
        {
            if (player == null) return;
            Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.Greetings1)); Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.Greetings2)); Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.Greetings3)); Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.Greetings4)); Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.Greetings5)); Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.Greetings4)); Trigger.SendChatMessage(player, "");
        }
        #endregion Player

        public static (byte, float) GetPlayerJobLevelBonus(sbyte jobId, int points)
        {
            try
            {
                switch (jobId)
                {
                    case 0://electrecian
                        if (points >= 700 && points <= 2099) return (1, 1.05f);
                        else if (points >= 2100 && points <= 4499) return (2, 1.1f);
                        else if (points >= 4500 && points <= 7499) return (3, 1.12f);
                        else if (points >= 7500 && points <= 14999) return (4, 1.15f);
                        else if (points >= 15000) return (5, 1.2f);
                        else return (0, 1);
                    case 1://lawnmower
                        if (points >= 2000 && points <= 5999) return (1, 1.05f);
                        else if (points >= 6000 && points <= 11999) return (2, 1.1f);
                        else if (points >= 12000 && points <= 19999) return (3, 1.12f);
                        else if (points >= 20000 && points <= 39999) return (4, 1.15f);
                        else if (points >= 40000) return (5, 1.2f);
                        else return (0, 1);
                    case 2://postman
                        if (points >= 200 && points <= 599) return (1, 1.05f);
                        else if (points >= 600 && points <= 1199) return (2, 1.1f);
                        else if (points >= 1200 && points <= 1999) return (3, 1.12f);
                        else if (points >= 2000 && points <= 3999) return (4, 1.15f);
                        else if (points >= 4000) return (5, 1.2f);
                        else return (0, 1);
                    case 3://taxi
                        if (points >= 25 && points <= 74) return (1, 1);
                        else if (points >= 75 && points <= 199) return (2, 1);
                        else if (points >= 200 && points <= 499) return (3, 1);
                        else if (points >= 500 && points <= 999) return (4, 1);
                        else if (points >= 1000) return (5, 1);
                        else return (0, 1);
                    case 4://bus
                        if (points >= 3000 && points <= 9999) return (1, 1.05f);
                        else if (points >= 10000 && points <= 19999) return (2, 1.1f);
                        else if (points >= 20000 && points <= 34999) return (3, 1.12f);
                        else if (points >= 35000 && points <= 69999) return (4, 1.15f);
                        else if (points >= 70000) return (5, 1.2f);
                        else return (0, 1);
                    case 5://mech
                        if (points >= 10 && points <= 24) return (1, 1);
                        else if (points >= 25 && points <= 49) return (2, 1);
                        else if (points >= 50 && points <= 99) return (3, 1);
                        else if (points >= 100 && points <= 249) return (4, 1);
                        else if (points >= 250) return (5, 1);
                        else return (0, 1);
                    case 6://trucker
                        if (points >= 30 && points <= 89) return (1, 1);
                        else if (points >= 90 && points <= 199) return (2, 1);
                        else if (points >= 200 && points <= 349) return (3, 1);
                        else if (points >= 350 && points <= 699) return (4, 1);
                        else if (points >= 700) return (5, 1);
                        else return (0, 1);
                    case 7://collector
                        if (points >= 150 && points <= 399) return (1, 1.05f);
                        else if (points >= 400 && points <= 899) return (2, 1.1f);
                        else if (points >= 900 && points <= 1499) return (3, 1.12f);
                        else if (points >= 1500 && points <= 2999) return (4, 1.15f);
                        else if (points >= 3000) return (5, 1.2f);
                        else return (0, 1);
                    default: return (0, 1);
                }
            }
            catch (Exception e)
            {
                Main.Log.Write($"GetPlayerJobLevelBonus Exception: {e.ToString()}");
                return (0, 1);
            }
        }

        public static int[] GetPlayerJobsNextLevel(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return new int[] { 700, 2000, 200, 25, 3000, 10, 30, 150 };

                int[] nextLevelInfo = { 700, 2000, 200, 25, 3000, 10, 30, 150 };

                for (int i = 0; i <= 7; i++)
                {
                    if (characterData.JobSkills.ContainsKey(i))
                    {
                        int points = characterData.JobSkills[i];
                        int correctValue = 250;

                        switch (i)
                        {
                            case 0:
                                if (points >= 700 && points <= 2099) correctValue = 2100;
                                else if (points >= 2100 && points <= 4499) correctValue = 4500;
                                else if (points >= 4500 && points <= 7499) correctValue = 7500;
                                else if (points >= 7500) correctValue = 15000;
                                else correctValue = 700;
                                break;
                            case 1:
                                if (points >= 2000 && points <= 5999) correctValue = 6000;
                                else if (points >= 6000 && points <= 11999) correctValue = 12000;
                                else if (points >= 12000 && points <= 19999) correctValue = 20000;
                                else if (points >= 20000) correctValue = 40000;
                                else correctValue = 2000;
                                break;
                            case 2:
                                if (points >= 200 && points <= 599) correctValue = 600;
                                else if (points >= 600 && points <= 1199) correctValue = 1200;
                                else if (points >= 1200 && points <= 1999) correctValue = 2000;
                                else if (points >= 2000) correctValue = 4000;
                                else correctValue = 200;
                                break;
                            case 3:
                                if (points >= 25 && points <= 74) correctValue = 75;
                                else if (points >= 75 && points <= 199) correctValue = 200;
                                else if (points >= 200 && points <= 499) correctValue = 500;
                                else if (points >= 500) correctValue = 1000;
                                else correctValue = 25;
                                break;
                            case 4:
                                if (points >= 3000 && points <= 9999) correctValue = 10000;
                                else if (points >= 10000 && points <= 19999) correctValue = 20000;
                                else if (points >= 20000 && points <= 34999) correctValue = 35000;
                                else if (points >= 35000) correctValue = 70000;
                                else correctValue = 3000;
                                break;
                            case 5:
                                if (points >= 10 && points <= 24) correctValue = 25;
                                else if (points >= 25 && points <= 49) correctValue = 50;
                                else if (points >= 50 && points <= 99) correctValue = 100;
                                else if (points >= 100) correctValue = 250;
                                else correctValue = 10;
                                break;
                            case 6:
                                if (points >= 30 && points <= 89) correctValue = 90;
                                else if (points >= 90 && points <= 199) correctValue = 200;
                                else if (points >= 200 && points <= 349) correctValue = 350;
                                else if (points >= 350) correctValue = 700;
                                else correctValue = 30;
                                break;
                            case 7:
                                if (points >= 150 && points <= 399) correctValue = 400;
                                else if (points >= 400 && points <= 899) correctValue = 900;
                                else if (points >= 900 && points <= 1499) correctValue = 1500;
                                else if (points >= 1500) correctValue = 3000;
                                else correctValue = 150;
                                break;
                            default: break;
                        }

                        nextLevelInfo[i] = correctValue;
                    }
                }

                return nextLevelInfo;
            }
            catch (Exception e)
            {
                Main.Log.Write($"GetPlayerJobsNextLevel Exception: {e.ToString()}");
                return new int[] { 700, 2000, 200, 25, 3000, 10, 30, 150 };
            }
        }

        #region ClientEvents
        [RemoteEvent("__ragemp_cheat_detected")]
        public static void RagempCheatDetected(ExtPlayer player, int cheatCode)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.AdminLVL > 0) return;
                if (sessionData.AccBanned == true) return;
                sessionData.AccBanned = true;
                Trigger.SendPunishment(LangFunc.GetText(LangType.Ru, DataName.AutoACBan, player.Name, cheatCode), player);
                DateTime until = DateTime.Now.AddDays(999);
                Ban.Online(player, until, true, $"Cheats({cheatCode})", "AntiCheat");
                Notify.Send(player, NotifyType.Warning, NotifyPosition.Center, LangFunc.GetText(LangType.Ru, DataName.YouBannedPerm), 30000);
                Notify.Send(player, NotifyType.Warning, NotifyPosition.Center, LangFunc.GetText(LangType.Ru, DataName.ReasonCheats, cheatCode), 30000);
                GameLog.Ban(0, characterData.UUID, player.GetLogin(), until, LangFunc.GetText(LangType.Ru, DataName.ReasonCheats, cheatCode), true);
                player.Kick();
            }
            catch (Exception e)
            {
                Log.Write($"RagempCheatDetected Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("kickclient")]
        public void ClientEvent_Kick(ExtPlayer player)
        {
            try
            {
                if (player != null) player.Kick();
            }
            catch (Exception e)
            {
                Log.Write($"ClientEvent_Kick Exception: {e.ToString()}");
                if (player != null) player.Kick();
            }
        }
        [RemoteEvent("keyinsert")]
        public void ClientEvent_KeyInsert(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null)
                return;

            try
            {
                if (!player.IsCharacterData()) return;
                Trigger.SendToAdmins(5, LangFunc.GetText(LangType.Ru, DataName.InsertTrigger, player.Name, player.Value, characterData.LVL));
            }
            catch (Exception e)
            {
                Log.Write($"ClientEvent_KeyInsert Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("client_trycatch")]
        public void ClientEvent_TryCatch(ExtPlayer player, string path, string callback, string msg)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (DateTime.Now < sessionData.TimingsData.NextClientTryCatch) return;
                sessionData.TimingsData.NextClientTryCatch = DateTime.Now.AddSeconds(1);
                GameLog.ClientTryCatch(path, callback, msg);
            }
            catch (Exception e)
            {
                Log.Write($"ClientEvent_TryCatch Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("syncWaypoint")]
        public void ClientEvent_SyncWP(ExtPlayer player, float X, float Y)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (!player.IsInVehicle) return;
                Entity driver = NAPI.Vehicle.GetVehicleDriver(player.Vehicle);
                if (driver == player || driver == null) return;
                ExtPlayer target = (ExtPlayer)driver; // Test
                if (!target.IsCharacterData()) return;
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VoditeluPeredalDannie), 3000);
                BattlePass.Repository.UpdateReward(player, 126);
                Trigger.ClientEvent(target, "syncWP", X, Y, 0);
            }
            catch (Exception e)
            {
                Log.Write($"ClientEvent_SyncWP Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.updateAfkStatus")]
        public void UpdateAFKStatus(ExtPlayer player, bool status = false)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                var afkData = sessionData.AfkData;
                afkData.IsAfk = status;
                player.SetSharedData("AFK_STATUS", status);
                
                if (status)
                    afkData.Time = DateTime.Now;
                else
                {
                    var inAFK = DateTime.Now - afkData.Time;
                    afkData.PayDayMinute += inAFK.Minutes + 1;
                }
                
            }
            catch (Exception e)
            {
                Log.Write($"UpdateAFKStatus Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("syncSirenSound")]
        public void ClientEvent_SyncWP(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var fractionId = player.GetFractionId();
                if (!player.IsInVehicle || (Manager.FractionTypes[fractionId] != FractionsType.Gov && characterData.AdminLVL == 0)) return;
                var veh = (ExtVehicle) player.Vehicle;
                if (veh == null || !veh.Exists) return;
                if (NAPI.Vehicle.GetVehicleDriver(veh) != player) return;
                if (veh.GetSharedData<bool>("SIRENSOUND") == true)
                {
                    Trigger.ClientEventInDimension(UpdateData.GetPlayerDimension(player), "VehStream_SetSirenSound", veh, false);
                    veh.SetSharedData("SIRENSOUND", false);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WeeWeeOn), 1000);
                }
                else
                {
                    Trigger.ClientEventInDimension(UpdateData.GetPlayerDimension(player), "VehStream_SetSirenSound", veh, true);
                    veh.SetSharedData("SIRENSOUND", true);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WeeWeeOff), 1000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"ClientEvent_SyncWP Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("VehStream_updateSirenStatus")]
        public void VehStream_updateSirenStatus(ExtPlayer player, ExtVehicle veh, bool status)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (veh.IsVehicleLocalData()) 
                    veh.SetSharedData("isSirenOn", status);
            }
            catch (Exception e)
            {
                Log.Write($"VehStream_updateSirenStatus Exception: {e.ToString()}");
            }
        }

        public static void ClientEvent_Spawn(ExtPlayer player, int id)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                var accountData = player.GetAccountData();
                if (accountData == null) 
                    return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                player.Dimension = 0;

                characterData.IsAlive = true;

                House house = null;

                if (!characterData.IsSpawned)
                {
                    Trigger.ClientEvent(player, "doorsControl", JsonConvert.SerializeObject(DoorsControl));

                    if (Army.is_warg)
                        Trigger.ClientEvent(player, "alarm", "PORT_OF_LS_HEIST_FORT_ZANCUDO_ALARMS", true);

                    if (characterData.Unmute > 0)
                    {
                        if (sessionData.TimersData.MuteTimer == null)
                        {
                            sessionData.TimersData.MuteTimer = Timers.Start(1000, () => Admin.timer_mute(player));
                            characterData.VoiceMuted = true;
                            player.SetSharedData("vmuted", true);
                        }
                    }
                    house = HouseManager.GetHouse(player);
                    if (house != null)
                    {
                        var garage = house.GetGarageData();
                        garage?.SpawnCars(house.GetVehiclesCarNumber(), house);
                    }
                    Character.BindConfig.Repository.InitAdmin(player);
                }

                Trigger.ClientEvent(player, "ready");
                
                if (characterData.ArrestTime > 0)
                {
                    if (sessionData.TimersData.ArrestTimer == null)
                    {
                        sessionData.TimersData.ArrestTimer = Timers.Start(1000, () => FractionCommands.arrestTimer(player));
                        if (characterData.ArrestType == 1) player.Position = Sheriff.FirstPrisonPosition;
                        else if (characterData.ArrestType == 2) player.Position = Sheriff.SecondPrisonPosition;
                        else player.Position = Police.PrisonPosition;
                    }
                }
                else if (characterData.DemorganTime > 0)
                {
                    if (sessionData.TimersData.ArrestTimer == null)
                    {
                        sessionData.TimersData.ArrestTimer = Timers.Start(1000, () => Admin.timer_demorgan(player));
                        Chars.Repository.RemoveAllWeapons(player, true, armour: true);
                        NAPI.Entity.SetEntityPosition(player, Admin.DemorganPositions[rnd.Next(55)] + new Vector3(0, 0, 1.5));
                        player.SetSkin(Admin.DemorganSkins[rnd.Next(14)]);
                        Trigger.ClientEvent(player, "client.demorgan", true);
                        player.SetSharedData("HideNick", true);
                        NAPI.Player.SetPlayerHealth(player, 3);
                        if (!characterData.VoiceMuted)
                        {
                            characterData.VoiceMuted = true;
                            player.SetSharedData("vmuted", true);
                        }
                    }
                }
                else
                {
                    switch (id)
                    {
                        case 0:
                            player.Position = characterData.SpawnPos;
                            break;
                        case 1:
                            var fractionId = player.GetFractionId();
                            var organizationData = player.GetOrganizationData();
                            if (organizationData != null) 
                            {
                                if (organizationData.BlipId != -1) 
                                    player.Position = organizationData.BlipPosition;
                                else 
                                    player.Position = new Vector3(-774.045, 311.2569, 85.70606);
                                    
                                Chars.Repository.RemoveAllIllegalStuff(player);
                            }
                            else if (fractionId != (int) Fractions.Models.Fractions.None)
                            {
                                player.Position = Manager.FractionSpawns[fractionId];
                                
                                Chars.Repository.RemoveAllIllegalStuff(player);
                            }
                            else
                                player.Position = characterData.SpawnPos;
                            break;
                        case 2:
                            if (house != null)
                            {
                                player.Position = house.Position + new Vector3(0, 0, 1.5);
                                //Chars.Repository.RemoveAllWeapons(player, true, true, armour: true);
                                Chars.Repository.RemoveAllIllegalStuff(player);
                            }
                            else if (characterData.HotelID != -1) player.Position = Hotel.HotelEnters[characterData.HotelID] + new Vector3(0, 0, 1.12);
                            else
                            {
                                player.Position = characterData.SpawnPos;
                            }
                            break;
                        default:
                            // Not supposed to end up here. 
                            break;
                    }
                }
                
                sessionData.DeathData.InDeath = false;
                player.SetSharedData("InDeath", false);
                characterData.IsSpawned = true;
                
                ClothesComponents.UpdateClothes(player);
            }
            catch (Exception e)
            {
                Log.Write($"ClientEvent_Spawn Exception: {e.ToString()}");
            }
        }

        public static bool IHaveDemorgan(ExtPlayer player, bool withsmg = false)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return true;
                if (characterData.DemorganTime >= 1)
                {
                    if (withsmg) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IHaveDemorgan), 4000);
                    return true;
                }
                else return false;
            }
            catch (Exception e)
            {
                Log.Write($"IHaveDemorgan Exception: {e.ToString()}");
                return true;
            }
        }
        public static string RainbowExploit(string message)
        {
            if (message.Contains("{")) return message.Replace('{', ' ');
            return message;
        }
        [RemoteEvent("StartPushVehicle")]
        public void StartPushVehicle(ExtPlayer player, ExtVehicle vehicle)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (!FunctionsAccess.IsWorking("StartPushVehicle"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                if (sessionData.AnimationUse != null || IHaveDemorgan(player, true)) return;
                else if (sessionData.CuffedData.Cuffed)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsCuffed), 3000);
                    return;
                }
                else if (sessionData.DeathData.InDeath)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsDying), 3000);
                    return;
                }
                else if (VehicleManager.IsVehicleDeath(vehicle)) return;
                else if (vehicle == null || player.Position.DistanceTo(vehicle.Position) > 3 || characterData.ArrestTime >= 1) return;

                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    bool access = VehicleManager.canAccessByNumber(player, vehicle.NumberPlate);
                    if (!access && characterData.AdminLVL < 3)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoKeysFromVeh), 3000);
                        return;
                    }

                    string number = vehicle.NumberPlate;
                    var vehicleData = VehicleManager.GetVehicleToNumber(number);
                    if (vehicleData != null)
                    {
                        if (vehicleData.Health == 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarDead), 3000);
                            return;
                        }
                    }

                    OnAntiAnim(player);
                    Trigger.ClientEvent(player, "StartPushVehicle_client", vehicle);
                    Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, true, "pushVeh");
                }
            }
            catch (Exception e)
            {
                Log.Write($"StartPushVehicle Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("StopPushVehicle")]
        public void StopPushVehicle(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                Trigger.StopAnimation(player);
                OffAntiAnim(player);
            }
            catch (Exception e)
            {
                Log.Write($"StopPushVehicle Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("setStock")]
        public void ClientEvent_setStock(ExtPlayer player, string stock)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                sessionData.SelectData.SelectedStock = stock;
            }
            catch (Exception e)
            {
                Log.Write($"ClientEvent_setStock Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("inputCallback")]
        public void ClientEvent_inputCallback(ExtPlayer player, params object[] arguments)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (IHaveDemorgan(player, true))
                {
                    if (sessionData.SellItemData.Seller != null)
                    {
                        ExtPlayer target = sessionData.SellItemData.Seller;
                        sessionData.SellItemData = new SellItemData();
                        var targetSessionData = target.GetSessionData();
                        if (targetSessionData != null && target != player) targetSessionData.SellItemData = new SellItemData();
                    }
                    if (sessionData.TicketsData.Target != null)
                    {
                        ExtPlayer target = sessionData.TicketsData.Target;
                        sessionData.TicketsData = new TicketsData();
                        var targetSessionData = target.GetSessionData();
                        if (targetSessionData != null && target != player) targetSessionData.TicketsData = new TicketsData();
                    }
                    if (sessionData.DiceData.Target != null)
                    {
                        ExtPlayer target = sessionData.DiceData.Target;
                        sessionData.DiceData = new DiceData();
                        var targetSessionData = target.GetSessionData();
                        if (targetSessionData != null && target != player) targetSessionData.DiceData = new DiceData();
                    }
                    return;
                }
                string callback = arguments[0].ToString();
                string text = arguments[1].ToString();
                FractionData fractionData;
                switch (callback)
                {
                    case "fuelcontrol_city":
                    case "fuelcontrol_police":
                    case "fuelcontrol_ems":
                    case "fuelcontrol_fib":
                    case "fuelcontrol_army":
                    case "fuelcontrol_news":
                        int limit = 0;
                        if (text.Contains('.')) text = text.Replace(".", null);
                        if (!int.TryParse(text, out limit) || limit <= 0)
                        {
                            Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                            return;
                        }
                        switch (callback)
                        {
                            case "fuelcontrol_city":
                                fractionData = Manager.GetFractionData((int)Fractions.Models.Fractions.CITY);
                                if (fractionData == null)
                                    return;
                                
                                fractionData.FuelLimit = limit;
                                if (fractionData.FuelLeft > limit) 
                                    fractionData.FuelLeft = limit;
                                
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FuelcontrolCity, limit), 3000);
                                break;
                            case "fuelcontrol_police":
                                fractionData = Manager.GetFractionData((int)Fractions.Models.Fractions.POLICE);
                                if (fractionData == null)
                                    return;
                                fractionData.FuelLimit = limit;
                                //Stocks.fracStocks[(int)Fractions.Models.Fractions.SHERIFF].FuelLimit = limit;
                                if (fractionData.FuelLeft > limit)
                                {
                                    fractionData.FuelLeft = limit;
                                    //Stocks.fracStocks[(int)Fractions.Models.Fractions.SHERIFF].FuelLeft = limit;
                                }
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FuelcontrolPolice, limit), 3000);
                                break;
                            case "fuelcontrol_ems":
                                fractionData = Manager.GetFractionData((int)Fractions.Models.Fractions.EMS);
                                if (fractionData == null)
                                    return;
                                
                                fractionData.FuelLimit = limit;
                                if (fractionData.FuelLeft > limit) 
                                    fractionData.FuelLeft = limit;
                                
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FuelcontrolEms, limit), 3000);
                                break;
                            case "fuelcontrol_fib":
                                fractionData = Manager.GetFractionData((int)Fractions.Models.Fractions.FIB);
                                if (fractionData == null)
                                    return;
                                
                                fractionData.FuelLimit = limit;
                                if (fractionData.FuelLeft > limit) 
                                    fractionData.FuelLeft = limit;
                                
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FuelcontrolFib, limit), 3000);
                                break;
                            case "fuelcontrol_army":
                                fractionData = Manager.GetFractionData((int)Fractions.Models.Fractions.ARMY);
                                if (fractionData == null)
                                    return;
                                
                                fractionData.FuelLimit = limit;
                                if (fractionData.FuelLeft > limit)
                                    fractionData.FuelLeft = limit;
                                
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FuelcontrolArmy, limit), 3000);
                                break;
                            case "fuelcontrol_news":
                                fractionData = Manager.GetFractionData((int)Fractions.Models.Fractions.LSNEWS);
                                if (fractionData == null)
                                    return;
                                
                                fractionData.FuelLimit = limit;
                                if (fractionData.FuelLeft > limit) 
                                    fractionData.FuelLeft = limit;
                                
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FuelcontrolNews, limit), 3000);
                                break;
                        }
                        return;
                    case "club_setprice":
                        limit = 0;
                        if (text.Contains('.')) text = text.Replace(".", null);
                        if (!int.TryParse(text, out limit) || limit < 1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                            return;
                        }
                        AlcoFabrication.SetAlcoholPrice(player, limit);
                        return;
                    case "player_offerhousesell":
                        limit = 0;
                        if (text.Contains('.')) text = text.Replace(".", null);
                        if (!int.TryParse(text, out limit) || limit <= 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                            return;
                        }
                        ExtPlayer target = sessionData.SelectData.SelectedPlayer;
                        if (!target.IsCharacterData() || player.Position.DistanceTo(target.Position) > 2)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                            return;
                        }
                        Houses.HouseManager.OfferHouseSell(player, target, limit);
                        return;
                    case "buy_drugs":
                        limit = 0;
                        if (text.Contains('.')) text = text.Replace(".", null);
                        if (!int.TryParse(text, out limit) || limit <= 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                            return;
                        }
                        Gangs.BuyDrugs(player, limit);
                        return;
                    case "mayor_take":
                        if (!player.IsFractionLeader((int) Fractions.Models.Fractions.CITY)) return;
                        limit = 0;
                        if (text.Contains('.')) text = text.Replace(".", null);
                        if (!int.TryParse(text, out limit) || limit <= 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                            return;
                        }
                        if (limit > Cityhall.canGetMoney)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoCityhallGov, Cityhall.canGetMoney), 3000);
                            return;
                        }
                        
                        fractionData = Manager.GetFractionData((int)Fractions.Models.Fractions.CITY);
                        if (fractionData == null)
                            return;
                        
                        if (fractionData.Money < limit)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMoneyGov), 3000);
                            return;
                        }
                        MoneySystem.Bank.Change(characterData.Bank, limit);
                        fractionData.Money -= limit;
                        GameLog.Money($"frac(6)", $"bank({characterData.Bank})", limit, "treasureTake");
                        return;
                    case "mayor_put":
                        if (!player.IsFractionLeader((int) Fractions.Models.Fractions.CITY)) return;
                        limit = 0;
                        if (text.Contains('.')) text = text.Replace(".", null);
                        if (!int.TryParse(text, out limit) || limit <= 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                            return;
                        }
                        
                        fractionData = Manager.GetFractionData((int)Fractions.Models.Fractions.CITY);
                        if (fractionData == null)
                            return;
                        
                        if (!MoneySystem.Bank.Change(characterData.Bank, -limit))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMoney), 3000);
                            return;
                        }
                        fractionData.Money += limit;
                        GameLog.Money($"bank({characterData.Bank})", $"frac(6)", limit, "treasurePut");
                        return;
      
                    case "loadmats":
                    case "unloadmats":
                    case "loaddrugs":
                    case "unloaddrugs":
                    case "loadmedkits":
                    case "unloadmedkits":
                        Stocks.fracgarage(player, callback, text);
                        break;
                    case "player_givemoney":
                        Selecting.playerTransferMoney(player, text);
                        return;
                    case "player_medkit":
                        limit = 0;
                        if (text.Contains('.')) text = text.Replace(".", null);
                        if (!int.TryParse(text, out limit) || limit <= 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                            return;
                        }
                        target = sessionData.SelectData.SelectedPlayer;
                        if (!target.IsCharacterData() || player.Position.DistanceTo(target.Position) > 5)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                            return;
                        }
                        FractionCommands.sellMedKitToTarget(player, target, limit);
                        return;
                    case "player_heal":
                        limit = 0;
                        if (text.Contains('.')) text = text.Replace(".", null);
                        if (!int.TryParse(text, out limit) || limit <= 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                            return;
                        }
                        target = sessionData.SelectData.SelectedPlayer;
                        if (!target.IsCharacterData() || player.Position.DistanceTo(target.Position) > 5)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                            return;
                        }
                        FractionCommands.healTarget(player, target, limit);
                        return;
                    case "put_stock":
                    case "take_stock":
                        limit = 0;
                        if (text.Contains('.')) text = text.Replace(".", null);
                        if (!int.TryParse(text, out limit) || limit < 1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                            return;
                        }
                        if (Admin.IsServerStoping)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ServerCantAccept), 3000);
                            return;
                        }
                        Stocks.inputStocks(player, 0, callback, limit);
                        return;
                    case "streetrace":
                        limit = 0;
                        if (text.Contains('.')) text = text.Replace(".", null);
                        if (!int.TryParse(text, out limit) || limit < 0 || limit > 100000)
                        {
                            Trigger.ClientEvent(player, "openInput", "Уличная гонка", LangFunc.GetText(LangType.Ru, DataName.InputBet0100k), 6, "streetrace");
                            return;
                        }
                        StreetRace.SelectRate(player, limit);
                        return;
                    case "sellcar":
                        target = sessionData.SellItemData.Buyer;
                        string number = sessionData.SellItemData.Number; 
                        var targetCharacterData = target.GetCharacterData();
                        if (targetCharacterData == null || player.Position.DistanceTo(target.Position) > 5)
                        {
                            sessionData.SellItemData = new SellItemData();
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                            return;
                        }
                        limit = 0;
                        if (text.Contains('.')) text = text.Replace(".", null);
                        if (!int.TryParse(text, out limit) || limit < 1 || limit > 100000000)
                        {
                            sessionData.SellItemData = new SellItemData();
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                            return;
                        }
                        var house = HouseManager.GetHouse(target, true);
                        if (house != null)
                        {
                            var garage = house.GetGarageData();
                            if (garage == null)
                            {
                                sessionData.SellItemData = new SellItemData();
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,LangFunc.GetText(LangType.Ru, DataName.PlayerNoGarage), 3000);
                                return;
                            }
                            var vehiclesCount = VehicleManager.GetVehiclesCarCountToPlayer(target.Name);
                            if (vehiclesCount >= Houses.GarageManager.GarageTypes[garage.Type].MaxCars)
                            {
                                sessionData.SellItemData = new SellItemData();
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerMaxVehGarage), 3000);
                                return;
                            }
                        }
                        else
                        {
                            var vehiclesCount = VehicleManager.GetVehiclesCarCountToPlayer(target.Name);
                            if (vehiclesCount >= GarageManager.MaxGarageCars)
                            {
                                sessionData.SellItemData = new SellItemData();
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerMaxVeh), 3000);
                                return;
                            }
                        }
                        if (targetCharacterData.Money < limit)
                        {
                            sessionData.SellItemData = new SellItemData();
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerNotEnoughMoney), 3000);
                            return;
                        }
                        
                        var vehicleData = VehicleManager.GetVehicleToNumber(number);
                        if (vehicleData == null)
                        {
                            sessionData.SellItemData = new SellItemData();
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehNotExists), 3000);
                            return;
                        }

                        if (vehicleData.Holder != player.Name)
                        {
                            sessionData.SellItemData = new SellItemData();
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouNeVladeetVehTrade), 3000);
                            return;
                        }
                        var targetSessionData = target.GetSessionData();
                        if ((targetSessionData.SellItemData.Seller != null || targetSessionData.SellItemData.Buyer != null) && Chars.Repository.TradeGet(target))
                        {
                            sessionData.SellItemData = new SellItemData();
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerAlreadyTraded), 3000);
                            return;
                        }
                        string vName = vehicleData.Model;
                        EventSys.SendCoolMsg(player,"Предложение", "Покупка", $"{LangFunc.GetText(LangType.Ru, DataName.BuycarOffer, target.Name, vName, number, MoneySystem.Wallet.Format(limit))}", "", 5000);
                        //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BuycarOffer, target.Name, vName, number, MoneySystem.Wallet.Format(limit)), 3000);
                        targetSessionData.SellItemData.Seller = player;
                        targetSessionData.SellItemData.Buyer = target;
                        targetSessionData.SellItemData.Number = number;
                        targetSessionData.SellItemData.Price = limit;
                        sessionData.SellItemData.Seller = player;
                        sessionData.SellItemData.Buyer = target;
                        sessionData.SellItemData.Number = number;
                        sessionData.SellItemData.Price = limit;
                        Trigger.ClientEvent(target, "openDialog", "BUY_CAR", LangFunc.GetText(LangType.Ru, DataName.BuycarOffered, player.Name, vName, number, MoneySystem.Wallet.Format(limit)));
                        return;
                    case "weaptransfer":
                        limit = 0;
                        if (text.Contains('.')) text = text.Replace(".", null);
                        if (!int.TryParse(text, out limit) || limit <= 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                            return;
                        }
                        return;
                    case "extend_hotel_rent":
                        limit = 0;
                        if (text.Contains('.')) text = text.Replace(".", null);
                        if (!int.TryParse(text, out limit) || limit <= 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                            return;
                        }
                        Houses.Hotel.ExtendHotelRent(player, limit);
                        return;
                    case "sell_festive":
                        if (text.Contains('.')) text = text.Replace(".", null);
                        if (!int.TryParse(text, out limit))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                            return;
                        }
                        if (limit <= 0 || limit > 500)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AmountIncorrect), 3000);
                            return;
                        }
                        if (limit % 3 != 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NumberNotDelitsya), 3000);
                            return;
                        }
                        int count = Chars.Repository.getCountItem($"char_{characterData.UUID}", Festive.EventCoins, bagsToggled: false);
                        if (limit > count)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoCoins), 3000);
                            return;
                        }
                        Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", Festive.EventCoins, limit);
                        string prefix = "";
                        if (limit == 2 || limit == 3 || limit == 4) prefix = "а";
                        else if (limit > 4) prefix = "ов";
                        int rb = Convert.ToInt32(limit / 3);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucTradeCoins, limit, prefix, rb), 3000);
                        UpdateData.RedBucks(player, rb, msg:LangFunc.GetText(LangType.Ru, DataName.ExchangeCoins, Festive.EventCoinsName));
                        break;
                    case "rentname":
                        if (sessionData.TentIndex == -1) return;

                        int index = sessionData.TentIndex;

                        if (index == -1) return;
                        else if (!Inventory.Tent.Repository.TentsData.ContainsKey(index)) return;

                        if (string.IsNullOrEmpty(text))
                        {
                            Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                            return;
                        }
                        
                        text = BlockSymbols(text);
                        
                        var textWithoutEmtySpacesMoreThanOne = Regex.Replace(text, @"\s+", " ");
                        text = new Regex("[^a-zA-Zа-яА-Я0-9 -]").Replace(textWithoutEmtySpacesMoreThanOne, "");
                        
                        Inventory.Tent.Repository.UpdateTentLabel(index, text);
                        GameLog.AddInfo($"(RentName) player({characterData.UUID}) {text}");
                        Trigger.SendToAdmins(1, LangFunc.GetText(LangType.Ru, DataName.TentPostavil, player.Name, player.Value, text));
                        break;
                    case "sell_tent":

                        if (!int.TryParse(text, out limit))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                            return;
                        }
                        if (limit <= 0 || limit > 99_999_999)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AmountIncorrect), 3000);
                            return;
                        }

                        index = sessionData.TentIndex;

                        if (index == -1) return;
                        else if (!Inventory.Tent.Repository.TentsData.ContainsKey(index)) return;

                        var tentData = Inventory.Tent.Repository.TentsData[index];

                        var inventoryTentData = sessionData.InventoryTentData;

                        InventoryItemData Item = Chars.Repository.GetItemData(player, inventoryTentData.ArrayName, inventoryTentData.Index);

                        if (Item.ItemId == ItemId.Debug) return;

                        var Location = "tent";
                        var locationName = $"{Location}_{characterData.UUID}";
                        int slotId = -1;

                        Item.Price = limit;
                        
                        ItemsInfo ItemInfo = Chars.Repository.ItemsInfo[Item.ItemId];
                        if (tentData.isBlack == true && Item.ItemId != ItemId.BodyArmor && Item.ItemId != ItemId.Drugs && Item.ItemId != ItemId.Material && ItemInfo.functionType != newItemType.Weapons && ItemInfo.functionType != newItemType.MeleeWeapons && ItemInfo.functionType != newItemType.Ammo)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantSellItem), 3000);
                            return;
                        }

                        if (inventoryTentData.Value == Item.Count)
                        {
                            if (!Chars.Repository.InventoryMaxSlots.ContainsKey(Location) || (slotId = Chars.Repository.AddItem(player, locationName, Location, Item, MaxSlots: Chars.Repository.GetMaxSlots(player, Location))) == -1)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                                return;
                            }
                            //tentData.slotToPrice[slotId] = limit;
                            //Trigger.ClientEvent(player, "client.inventory.SlotToPrice", JsonConvert.SerializeObject(tentData.slotToPrice));
                            Chars.Repository.SetItemData(player, inventoryTentData.ArrayName, inventoryTentData.Index, new InventoryItemData(), true);
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SellItemTent, Chars.Repository.ItemsInfo[Item.ItemId].Name, limit), 10000);
                            return;
                        }

                        if ((slotId = Chars.Repository.AddNewItem(player, locationName, Location, Item.ItemId, inventoryTentData.Value, Item.Data, MaxSlots: Chars.Repository.GetMaxSlots(player, Location), price: limit)) == -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                        //tentData.slotToPrice[slotId] = limit;
                        //Trigger.ClientEvent(player, "client.inventory.SlotToPrice", JsonConvert.SerializeObject(tentData.slotToPrice));
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SellItemTent, Chars.Repository.ItemsInfo[Item.ItemId].Name, limit), 10000);
                        Chars.Repository.RemoveIndex(player, inventoryTentData.ArrayName, inventoryTentData.Index, inventoryTentData.Value);
                        break;
                    case "make_ped_name":
                        if (string.IsNullOrEmpty(text))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                            return;
                        }
                        text = BlockSymbols(text);
                        if (text.Length >= 36)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Name36ogran), 3000);
                            return;
                        }
                        PedSystem.Pet.Repository.UpdateName(player, sessionData.SelectPed, text);
                        sessionData.SelectPed = null;
                        break;
                    case "player_ticketsum":
                        limit = 0;
                        if (text.Contains('.')) text = text.Replace(".", null);
                        if (!int.TryParse(text, out limit) || limit <= 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                            return;
                        }
                        sessionData.TicketsData.Price = limit;
                        Trigger.ClientEvent(player, "openInput", "Выписать штраф (причина)", "Причина", 50, "player_ticketreason");
                        break;
                    case "player_ticketreason":
                        TicketsData tddata = sessionData.TicketsData;
                        if (string.IsNullOrEmpty(text))
                        {
                            tddata.Target = null;
                            tddata.Price = 0;
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                            return;
                        }
                        FractionCommands.ticketToTarget(player, tddata.Target, tddata.Price, text);
                        tddata.Target = null;
                        tddata.Price = 0;
                        break;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"ClientEvent_inputCallback Exception: {e.ToString()}");
            }
        }

        /*public static byte GetNearestDoor(Player player)
        {
            try
            {
                if (!player.IsCharacterData()) return 255;
                Vector3 mypos = player.Position;
                byte door = 0;
                foreach (Vector3 v in DoorsPositions)
                {
                    if (mypos.DistanceTo(v) <= 3) return door;
                    door++;
                }
                return 255;
            }
            catch (Exception e)
            {
                Log.Write($"GetNearestDoor Exception: {e.ToString()}");
                return 255;
            }
        }*/

        public static ConcurrentDictionary<string, bool> DoorsControl = new ConcurrentDictionary<string, bool>();

        public static async void InitDoorsControl()
        {
            await using var db = new ServerBD("MainDB");//При старте сервера

            var doorsControl = db.Doorscontrol.ToList();
            foreach (var doorControl in doorsControl)
            {
                if (!DoorsControl.ContainsKey(doorControl.Id))
                {
                    DoorsControl.TryAdd(doorControl.Id, doorControl.Toggled);
                }
            }
        }
        public static async Task SaveDoorsControl(ServerBD db)
        {
            try
            {
                foreach (var doorControl in DoorsControl)
                {
                    await db.Doorscontrol
                        .Where(dc => dc.Id == doorControl.Key)
                        .Set(dc => dc.Toggled, doorControl.Value)
                        .UpdateAsync();
                }
            }            
            catch (Exception e)
            {
                Log.Write($"SaveDoorsControl Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.doorControl")]
        public void DoorControlState(ExtPlayer player, string door)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (!DoorsControl.ContainsKey(door)) return;
                if (door == null || door.Split('_') == null || door.Split('_').Length < 2) return; 
                
                var fractionId = player.GetFractionId();
                
                switch (door.Split('_')[0])
                {
                    case "pd":
                        if (fractionId != (int)Fractions.Models.Fractions.POLICE && fractionId != (int)Fractions.Models.Fractions.FIB) return;
                        else if (!player.IsFractionAccess(RankToAccess.DoorControl)) return;
                        break;
                    case "fib":
                        if (fractionId != (int)Fractions.Models.Fractions.FIB) return;
                        else if (!player.IsFractionAccess(RankToAccess.DoorControl)) return;
                        break;
                    case "gov":
                        if (fractionId != (int)Fractions.Models.Fractions.CITY && fractionId != (int)Fractions.Models.Fractions.FIB) return;
                        else if (!player.IsFractionAccess(RankToAccess.DoorControl)) return;
                        break;
                    case "news":
                        if (fractionId != (int)Fractions.Models.Fractions.LSNEWS) return;
                        else if (!player.IsFractionAccess(RankToAccess.DoorControl)) return;
                        break;
                    case "am":
                        if (fractionId != (int)Fractions.Models.Fractions.ARMENIAN) return;
                        break;
                    case "rm":
                        if (fractionId != (int)Fractions.Models.Fractions.RUSSIAN) return;
                        break;
                    case "yk":
                        if (fractionId != (int)Fractions.Models.Fractions.YAKUZA) return;
                        break;
                    case "lcn":
                        if (fractionId != (int)Fractions.Models.Fractions.LCN) return;
                        break;
                    case "ems":
                        if (fractionId != (int)Fractions.Models.Fractions.EMS && fractionId != (int)Fractions.Models.Fractions.FIB) return;
                        break;
                    case "jud":
                        if (fractionId != (int)Fractions.Models.Fractions.CITY && fractionId != (int)Fractions.Models.Fractions.FIB) return;
                        break;
                    case "sheriff":
                        if (fractionId != (int)Fractions.Models.Fractions.SHERIFF && fractionId != (int)Fractions.Models.Fractions.FIB) return;
                        else if (!player.IsFractionAccess(RankToAccess.DoorControl)) return;
                        break;
                    case "army":
                        if (fractionId != (int)Fractions.Models.Fractions.ARMY && fractionId != (int)Fractions.Models.Fractions.FIB) return;
                        else if (!player.IsFractionAccess(RankToAccess.DoorControl)) return;
                        break;
                    case "fam":
                        if (fractionId != (int)Fractions.Models.Fractions.FAMILY) return;
                        break;
                    case "vag":
                        if (fractionId != (int)Fractions.Models.Fractions.VAGOS) return;
                        break;
                    case "blood":
                        if (fractionId != (int)Fractions.Models.Fractions.BLOOD) return;
                        break;
                    case "mara":
                        if (fractionId != (int)Fractions.Models.Fractions.MARABUNTA) return;
                        break;
                    case "ballas":
                        if (fractionId != (int)Fractions.Models.Fractions.BALLAS) return;
                        break;
                    default:
                        return;

                }

                DoorsControl[door] = !DoorsControl[door];

                if (DoorsControl[door]) 
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DoorsClosed), 3000);
                else 
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DoorsOpened), 3000);

                Trigger.ClientEventForAll("doorControl", door, DoorsControl[door]);
            }
            catch (Exception e)
            {
                Log.Write($"DoorControlState Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("engineCarPressed")]
        public void ClientEvent_engineCarPressed(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                VehicleManager.onClientEvent(player, "engineCarPressed");
            }
            catch (Exception e)
            {
                Log.Write($"ClientEvent_engineCarPressed Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("lockCarPressed")]
        public void ClientEvent_lockCarPressed(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                int id = CustomColShape.GetDataToEnum(player, ColShapeEnums.EnterHouse);
                if (id == (int)ColShapeData.Error)
                    id = CustomColShape.GetDataToEnum(player, ColShapeEnums.ExitHouse);

                if (id != (int) ColShapeData.Error)
                {            
                    var house = Houses.HouseManager.Houses.FirstOrDefault(h => h.ID == sessionData.HouseID);
                    if (house != null && (sessionData.Name == house.Owner || house.Roommates.ContainsKey(sessionData.Name)))
                    {
                        house.SetLock(!house.Locked);
                        if (house.Locked)
                        {
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.HouseClosed), 3000);
                            BattlePass.Repository.UpdateReward(player, 125);
                        }
                        else
                        {
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.HouseOpened), 3000);
                            BattlePass.Repository.UpdateReward(player, 124);
                        } 
                        return;
                    }
                }
                VehicleManager.onClientEvent(player, "lockCarPressed");
            }
            catch (Exception e)
            {
                Log.Write($"ClientEvent_lockCarPressed Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("acceptPressed")]
        public void RemoteEvent_acceptPressed(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (!sessionData.RequestData.IsRequested) return;
                if (DateTime.Now >= sessionData.RequestData.Time)
                {
                    sessionData.RequestData = new RequestData();
                    Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OfferTimeout), 1500);
                    StreetRace.ClearData(player, -1);
                    return;
                }
                if (IHaveDemorgan(player, true)) return;
                sessionData.RequestData.IsRequested = false;
                switch (sessionData.RequestData.Request)
                {
                    case "StreetRace":
                        StreetRace.Accept(player);
                        break;
                    case "acceptPass":
                        Docs.AcceptPasport(player);
                        break;
                    case "acceptLics":
                        Docs.AcceptLicenses(player);
                        break;
                    case "acceptIdcard":
                        Docs.AcceptIdcard(player);
                        break;
                    case "acceptCertificate":
                        Docs.AcceptCertificate(player);
                        break;
                    case "OFFER_ITEMS":
                        Chars.Repository.StartTrade(player, sessionData.RequestData.From);
                        break;
                    case "HANDSHAKE":
                        Character.Friend.Repository.Handshake(player);
                        break;
                    case "HANDSUP":
                        FractionCommands.playerHandsupOfferAgree(player);
                        break;
                    case "trade_vehicle":
                        Chars.Repository.TradeStart(player, sessionData.RequestData.From, "vehicle");
                        break;
                    case "trade_house":
                        Chars.Repository.TradeStart(player, sessionData.RequestData.From, "house");
                        break;
                    case "trade_business":
                        Chars.Repository.TradeStart(player, sessionData.RequestData.From, "business");
                        break;
                    case "PAIRED_EMBRACE":
                    case "PAIRED_KISS":
                    case "PAIRED_FIVE":
                    case "PAIRED_SLAP":
                        Selecting.pairedAnimationAccept(player, sessionData.RequestData.Request);
                        break;
                    case "carry_0":
                    case "carry_1":
                    case "carry_2":
                    case "carry_3":
                        Selecting.StartCarry(sessionData.RequestData.From, player, sessionData.RequestData.Request);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_acceptPressed Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("cancelPressed")]
        public void RemoteEvent_cancelPressed(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                StreetRace.ClearData(player, -1);
                if (!sessionData.RequestData.IsRequested) return;
                if (DateTime.Now >= sessionData.RequestData.Time)
                {
                    sessionData.RequestData = new RequestData();
                    Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OfferTimeout), 1500);
                    return;
                }
                if (IHaveDemorgan(player, true)) return;
                sessionData.RequestData.IsRequested = false;
                switch (sessionData.RequestData.Request)
                {
                    case "acceptPass":
                        if (characterData.Gender) Commands.RPChat("sme", player, LangFunc.GetText(LangType.Ru, DataName.HePassNo));
                        else Commands.RPChat("sme", player, LangFunc.GetText(LangType.Ru, DataName.ShePassNo));
                        break;
                    case "acceptLics":
                        if (characterData.Gender) Commands.RPChat("sme", player, LangFunc.GetText(LangType.Ru, DataName.HeLicNo));
                        else Commands.RPChat("sme", player, LangFunc.GetText(LangType.Ru, DataName.SheLicNo));
                        break;
                    case "HANDSUP":
                        if (characterData.Gender) Commands.RPChat("sme", player, LangFunc.GetText(LangType.Ru, DataName.NoHandsUp));
                        else Commands.RPChat("sme", player, LangFunc.GetText(LangType.Ru, DataName.SheNoHandsUp));
                        break;
                    default:
                        break;
                }
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CancelOffer), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_cancelPressed Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("dialogCallback")]
        public void RemoteEvent_DialogCallback(ExtPlayer player, string callback, bool yes)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (IHaveDemorgan(player, true))
                {
                    if (sessionData.SellItemData.Seller != null)
                    {
                        ExtPlayer target = sessionData.SellItemData.Seller;
                        sessionData.SellItemData = new SellItemData();
                        var targetSessionData = target.GetSessionData();
                        if (targetSessionData != null && target != player) targetSessionData.SellItemData = new SellItemData();
                    }
                    if (sessionData.TicketsData.Target != null)
                    {
                        ExtPlayer target = sessionData.TicketsData.Target;
                        sessionData.TicketsData = new TicketsData();
                        var targetSessionData = target.GetSessionData();
                        if (targetSessionData != null && target != player) targetSessionData.TicketsData = new TicketsData();
                    }
                    if (sessionData.DiceData.Target != null)
                    {
                        ExtPlayer target = sessionData.DiceData.Target;
                        sessionData.DiceData = new DiceData();
                        var targetSessionData = target.GetSessionData();
                        if (targetSessionData != null && target != player) targetSessionData.DiceData = new DiceData();
                    }
                    return;
                }

                if (yes)
                {
                    OrganizationData organizationData;
                    int fractionId;
                    switch (callback)
                    {
                        case "RepairMyVeh":
                            if (!player.IsInVehicle) return;
                            if (characterData.Money < 500)
                            {
                                Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMoneyMech), 3000);
                                return;
                            }
                            foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                            {
                                var foreachSessionData = foreachPlayer.GetSessionData();
                                if (foreachSessionData == null) continue;
                                var targetCharacterData = foreachPlayer.GetCharacterData();
                                if (targetCharacterData == null) continue;
                                    /*if (targetCharacterData.WorkID == (int)JobsId.CarMechanic && foreachSessionData.WorkData.OnWork)
                                {
                                    Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, "Сервис недоступен на данный момент, потому что в штате работают механики", 1500);
                                    return;
                                }*/
                            }    
                            var vehicle = (ExtVehicle) player.Vehicle;
                            VehicleManager.RepairCar(vehicle);
                            MoneySystem.Wallet.Change(player, -500);
                            GameLog.Money($"player({characterData.UUID})", $"server", 500, $"serviceMechanic");
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RepairPayed), 3000);
                            BattlePass.Repository.UpdateReward(player, 13);
                            Commands.RPChat("sme", player, LangFunc.GetText(LangType.Ru, DataName.RepairVehi));
                            return;
                        case "FORGET_FRIENDS":
                            if (characterData.Friends.Count < 1)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.HaveNoFriends), 3000);
                                return;
                            }
                            Character.Friend.Repository.ClearFriends(player, player.Name);
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ForgotFriends), 3000);
                            return;
                        case "PAY_MEDKIT":
                            Ems.payMedkit(player);
                            return;
                        case "PAY_HEAL":
                            Ems.payHeal(player);
                            return;
                        case "STOP_SERVER":
                            Admin.stopServer(player);
                            return;
                        case "FamilyZones":
                            Organizations.FamilyZones.Repository.ConfirmAttack(player);
                            return;
                        case "DICE_PLAY":
                            {
                                if (sessionData.DiceData.Target != null)
                                {
                                    ExtPlayer diceplayer = sessionData.DiceData.Target; 
                                    var diceplayerCharacterData = diceplayer.GetCharacterData();
                                    if (diceplayerCharacterData == null)
                                    {
                                        sessionData.DiceData = new DiceData();
                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OpponentNotFound), 3000);
                                    }
                                    else
                                    {
                                        var diceplayerSessionData = diceplayer.GetSessionData();
                                        int dicemon = sessionData.DiceData.Money;
                                        if (characterData.Money < dicemon)
                                        {
                                            diceplayerSessionData.DiceData = new DiceData();
                                            sessionData.DiceData = new DiceData();
                                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMoney), 3000);
                                            Notify.Send(diceplayer, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerNotEnoughMoney), 3000);
                                            return;
                                        }
                                        if (diceplayerCharacterData.Money < dicemon)
                                        {
                                            diceplayerSessionData.DiceData = new DiceData();
                                            sessionData.DiceData = new DiceData();
                                            Notify.Send(diceplayer, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMoney), 3000);
                                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerNotEnoughMoney), 3000);
                                            return;
                                        }
                                        if (!characterData.InCasino || !diceplayerCharacterData.InCasino)
                                        {
                                            diceplayerSessionData.DiceData = new DiceData();
                                            sessionData.DiceData = new DiceData();
                                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GameCancelCuzNoCasino), 3000);
                                            Notify.Send(diceplayer, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GameCancelCuzNoCasino), 3000);
                                            return;
                                        }
                                        if (sessionData.IsCasinoGame != null)
                                        {
                                            diceplayerSessionData.DiceData = new DiceData();
                                            sessionData.DiceData = new DiceData();
                                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GameCancelled), 3000);
                                            Notify.Send(diceplayer, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GameCancelled), 3000);
                                            return;
                                        }
                                        if (diceplayerSessionData.IsCasinoGame != null)
                                        {
                                            diceplayerSessionData.DiceData = new DiceData();
                                            sessionData.DiceData = new DiceData();
                                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GameCancelled), 3000);
                                            Notify.Send(diceplayer, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GameCancelled), 3000);
                                            return;
                                        }
                                        Notify.Send(diceplayer, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerAcceptOffer, player.Value), 3000);
                                        Commands.AcceptDice(player, diceplayer, dicemon);
                                    }
                                }
                            }
                            return;
                        case "RENTCARI_DATETIME":
                            if (sessionData.RentData != null)
                            {
                                RentData rentCar = sessionData.RentData;
                                if (rentCar.Date >= DateTime.Now.AddMinutes(60 * 8)) // 8 часа максимально
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyRentMaxTime), 8000);
                                    return;
                                }
                                int price = Rentcar.GetRentCarCash(accountData.VipLvl, characterData.LVL, rentCar.Price);
                                if (!MoneySystem.Bank.Change(characterData.Bank, -price, false))
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoBankMoney), 5000);
                                    return;
                                }
                                rentCar.Date = rentCar.Date.AddMinutes(59);
                                //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SuccessfullRentExpand, rentCar.Date), 5000);
                                Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Rent, LangFunc.GetText(LangType.Ru, DataName.SuccessfullRentExpand, rentCar.Date), DateTime.Now);  
                            }
                            return;
                        case "RENTCARI_STOPRENT":
                            Rentcar.OnReturnVehicle(player, true);
                            return;
                        case "BUY_CAR":
                            {
                                ExtPlayer seller = sessionData.SellItemData.Seller;
                                var sellerSessionData = seller.GetSessionData();
                                var sellerCharacterData = seller.GetCharacterData();
                                if (sellerSessionData == null || sellerCharacterData == null)
                                {
                                    sessionData.SellItemData = new SellItemData();
                                    return;
                                }
                                string number = sessionData.SellItemData.Number;
                                int price = sessionData.SellItemData.Price;
                                if (player.Position.DistanceTo(seller.Position) > 3)
                                {
                                    sellerSessionData.SellItemData = new SellItemData();
                                    sessionData.SellItemData = new SellItemData();
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                                    break;
                                }
                                var vehicleData = VehicleManager.GetVehicleToNumber(number);
                                if (vehicleData == null)
                                {
                                    sellerSessionData.SellItemData = new SellItemData();
                                    sessionData.SellItemData = new SellItemData();
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarBoleeNetu), 3000);
                                    break;
                                }
                                if (vehicleData.Holder != seller.Name)
                                {
                                    sellerSessionData.SellItemData = new SellItemData();
                                    sessionData.SellItemData = new SellItemData();
                                    return;
                                }
                                if (!MoneySystem.Wallet.Change(player, -price))
                                {
                                    sellerSessionData.SellItemData = new SellItemData();
                                    sessionData.SellItemData = new SellItemData();
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMoney), 3000);
                                    break;
                                }
                                vehicleData.Holder = player.Name;
                                MoneySystem.Wallet.Change(seller, price);
                                GameLog.Money($"player({characterData.UUID})", $"player({sellerCharacterData.UUID})", price, $"buyCar({vehicleData.Model}, {number})");
                                House sellerhouse = HouseManager.GetHouse(seller);
                                var sellerGarage = sellerhouse?.GetGarageData();
                                sellerGarage?.DeleteCar(number, isRevive: true);
                                
                                var house = HouseManager.GetHouse(player, true);
                                var garage = house?.GetGarageData();
                                garage?.SpawnCarToPos(number, house);

                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили {vehicleData.Model} ({number}) за {MoneySystem.Wallet.Format(price)}$ у {seller.Name}", 3000);
                                Notify.Send(seller, NotifyType.Success, NotifyPosition.BottomCenter, $"{player.Name} купил у Вас {vehicleData.Model} ({number}) за {MoneySystem.Wallet.Format(price)}$", 3000);
                                sellerSessionData.SellItemData = new SellItemData();
                                sessionData.SellItemData = new SellItemData();
                                VehicleManager.SaveHolder(vehicleData.Number);
                                break;
                            }
                        case "REMOVE_SNUM":
                            {
                                int num = sessionData.SMSNum;
                                if (!characterData.Contacts.ContainsKey(num))
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoContacts, num), 4000);
                                    return;
                                }
                                Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DelContact, num), 4000);
                                characterData.Contacts.Remove(num);
                                break;
                            }
                        case "INVITED_FRAC":
                            {
                                try
                                {
                                    if (sessionData.InviteData.Fraction != -1)
                                    {
                                        var target = sessionData.InviteData.Sender;
                                        fractionId = sessionData.InviteData.Fraction;
                                        sessionData.InviteData = new InviteData();
                                        if (!target.IsFractionMemberData()) return;
                                        player.AddFractionMemberData(fractionId, 1);
                                       // Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.JoinFrac, Fractions.Manager.FractionNames[fractionId]), 3000);
                                        EventSys.SendCoolMsg(player,"Фракция", "Добро пожаловать!", LangFunc.GetText(LangType.Ru, DataName.JoinFrac, Fractions.Manager.FractionNames[fractionId]), "", 8000); 
                                        
                                        Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AcceptJoinFrac, player.Name), 3000);
                                        //EventSys.SendCoolMsg(target,"Фракция", "Приглашение",  LangFunc.GetText(LangType.Ru, DataName.AcceptJoinFrac, player.Name), "", 8000); 
                                        GameLog.FracLog(fractionId, target.GetUUID(), characterData.UUID, target.Name, player.Name, "invite");
                                        Fractions.Table.Logs.Repository.AddLogs(target, FractionLogsType.Invite, LangFunc.GetText(LangType.Ru, DataName.AcceptedFrac, player.Name, characterData.UUID));
                                        Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.Invite, LangFunc.GetText(LangType.Ru, DataName.InvitedFrac, target.Name, target.GetUUID()));
                                    }
                                    else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.InvitedOffline), 3000);
                                }
                                catch { Log.Write("RemoteEvent_DialogCallback23 ERROR"); }

                                return;
                            }
                        case "INVITED_ORG":
                            {
                                try
                                {
                                    if (sessionData.InviteData.Organization != -1)
                                    {
                                        var target = sessionData.InviteData.Sender;
                                        var orgId = sessionData.InviteData.Organization;
                                        sessionData.InviteData = new InviteData();
                                        var targetCharacterData = target.GetCharacterData();
                                        if (targetCharacterData == null) return;
                                        if (Organizations.Manager.GetOrganizationMemberData(player.Name) != null) 
                                            return;
                                        
                                        organizationData = Organizations.Manager.GetOrganizationData(orgId);
                                        if (organizationData == null) 
                                            return;

                                        player.AddOrganizationMemberData(orgId);
                                        
                                        //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.JoinFrac, organizationData.Name), 3000);
                                        EventSys.SendCoolMsg(player,"Организация", "Добро пожаловать!", LangFunc.GetText(LangType.Ru, DataName.JoinFrac, organizationData.Name), "", 8000); 
                                        Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AcceptedOrg, player.Name), 3000);
                                        //EventSys.SendCoolMsg(target,"Организация", "Приглашение",  LangFunc.GetText(LangType.Ru, DataName.AcceptedOrg, player.Name), "", 8000); 
                                        Organizations.Table.Logs.Repository.AddLogs(target, OrganizationLogsType.Invite, LangFunc.GetText(LangType.Ru, DataName.AcceptedFrac, player.Name, characterData.UUID));
                                        Organizations.Table.Logs.Repository.AddLogs(player, OrganizationLogsType.Invite, LangFunc.GetText(LangType.Ru, DataName.InvitedFrac, target.Name, targetCharacterData.UUID));
                                    }
                                    else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.InvitedOffline), 3000);
                                }
                                catch { Log.Write("RemoteEvent_DialogCallback23 ERROR"); }

                                return;
                            }
                        case "REPAIR_CAR":
                            Jobs.AutoMechanic.mechanicPay(player);
                            return;
                        case "FUEL_CAR":
                            Jobs.AutoMechanic.mechanicPayFuel(player);
                            return;
                        case "HOUSE_SELL":
                            Houses.HouseManager.acceptHouseSell(player);
                            return;
                        case "HOUSE_SELL_TOGOV":
                            Houses.HouseManager.acceptHouseSellToGov(player);
                            return;
                        case "BIZ_SELL_TOGOV":
                            Players.Phone.Property.Businesses.Repository.OnSellConfirm(player);
                            return;
                        case "ORGCAR_SELL_TOGOV":
                            organizationData = player.GetOrganizationData();
                            if (organizationData == null)
                            {
                                sessionData.CarSellGov = null;
                                return;
                            }
                            Organizations.Manager.DeleteVehicle(player, sessionData.CarSellGov, true);
                            return;
                        case "CAR_SELL_TOGOV":
                            {
                                if (sessionData.CarSellGov != null)
                                {
                                    string number = sessionData.CarSellGov;
                                    var vehicleData = VehicleManager.GetVehicleToNumber(number);
                                    sessionData.CarSellGov = null;
                                    if (vehicleData == null)
                                    {
                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarBoleeNetu), 3000);
                                        break;
                                    }
                                    if (Fractions.Ticket.IsVehicleTickets(vehicleData.SqlId))
                                        return;
                                    int price = 0;
                                    if (BusinessManager.BusProductsData.ContainsKey(vehicleData.Model))
                                    {
                                        switch (accountData.VipLvl)
                                        {
                                            case 0: // None
                                            case 1: // Bronze
                                            case 2: // Silver
                                                price = Convert.ToInt32(BusinessManager.BusProductsData[vehicleData.Model].Price * 0.4);
                                                break;
                                            case 3: // Gold
                                                price = Convert.ToInt32(BusinessManager.BusProductsData[vehicleData.Model].Price * 0.5);
                                                break;
                                            case 4: // Platinum
                                            case 5: // Media Platinum
                                                price = Convert.ToInt32(BusinessManager.BusProductsData[vehicleData.Model].Price * 0.6);
                                                break;
                                            default:
                                                price = Convert.ToInt32(BusinessManager.BusProductsData[vehicleData.Model].Price * 0.4);
                                                break;
                                        }
                                    }
                                    MoneySystem.Wallet.Change(player, price);
                                    GameLog.Money($"server", $"player({characterData.UUID})", price, $"carSell({vehicleData.Model}, {number})");
                                    //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouSellCar, vehicleData.Model, number, MoneySystem.Wallet.Format(price)), 3000);
                                    Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Bank, LangFunc.GetText(LangType.Ru, DataName.YouSellCar, vehicleData.Model, number, MoneySystem.Wallet.Format(price)), DateTime.Now);
                                    VehicleManager.Remove(number);
                                    
                                    // if (price >= 1000000)
                                    //     Admin.AdminsLog(1, $"[ВНИМАНИЕ] Игрок {player.Name}({player.Value}) продал {vehicleData.Model} ({number}) за {MoneySystem.Wallet.Format(price)}$", 1, "#FF0000");
                                    //
                                    // if (price >= 10000 && sessionData.LastSellOperationSum == price)
                                    // {
                                    //     Admin.AdminsLog(1, $"[ВНИМАНИЕ] Игрок {player.Name}({player.Value}) два раза подряд получил по {price}$ от продажи {vehicleData.Model} ({number})", 1, "#FF0000");
                                    //     sessionData.LastSellOperationSum = 0;
                                    // }
                                    // else
                                    // {
                                    //     sessionData.LastSellOperationSum = price;
                                    // }
                                }
                            }
                            return;
                        case "GUN_LIC":
                            FractionCommands.acceptGunLic(player);
                            return;
                        case "PM_LIC":
                            FractionCommands.acceptPmLic(player);
                            return;
                        case "BUSINESS_BUY":
                            BusinessManager.acceptBuyBusiness(player);
                            return;
                        case "ROOM_INVITE":
                            HouseManager.acceptRoomInvite(player);
                            return;
                        case "CREATE_REF_CODE":
                            Commands.CreateRefCode(player);
                            return;
                        case "CARWASH_PAY":
                            BusinessManager.Carwash_Pay(player);
                            return;
                        case "TICKET":
                            FractionCommands.ticketConfirm(player, true);
                            return;
                        case "CONFIRM_BUY_BODYARMOUR":
                            fractionId = player.GetFractionId();
                            if (fractionId == (int)Fractions.Models.Fractions.None || Manager.GovIds.Contains(fractionId))
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CrimeBuyBronik), 3000);
                                return;
                            }
                            if (Chars.Repository.itemCount(player, "inventory", ItemId.BodyArmor) >= Chars.Repository.maxItemCount)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyHaveBronik), 3000);
                                return;
                            }
                            ItemStruct mItem = Chars.Repository.isItem(player, "inventory", ItemId.Material);
                            int count = (mItem == null) ? 0 : mItem.Item.Count;
                            if (count < 150)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMats), 3000);
                                return;
                            }
                            if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.BodyArmor, 1, "50") == -1)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                                return;
                            }
                            Chars.Repository.RemoveIndex(player, mItem.Location, mItem.Index, 150);
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Bronik50Crafted), 3000);
                            return;
                        case "CONFIRM_BUY_ORGBODYARMOUR":
                            Organizations.Manager.CraftBodyArmor(player);
                            return;
                        case "BIZ_CONFIRM_BUY_ORDERS":
                            Players.Phone.Property.Businesses.Repository.MaxProductsConfirm(player);
                            return;
                        /*case "CONFIRM_VEHICLE_TRADE":
                            Chars.Repository.TradeVehicleConfirmed(player, true);
                            return;
                        case "CONFIRM_HOUSE_TRADE":
                            Chars.Repository.TradeHouseConfirmed(player, true);
                            return;
                        case "CONFIRM_BUSINESSES_TRADE":
                            Chars.Repository.TradeBusinessesConfirmed(player, true);
                            return;*/
                        case "ChangeAutoNumberConfirm":
                            Chars.Repository.ChangeAutoNumberConfirm(player, true);
                            return;
                        case "PAY_AIRDROP_INFO":
                            Events.AirDrop.Repository.PayAirdropInfo(player);
                            return;
                        case "PAY_AIRDROP_ORDER":
                            Events.AirDrop.Repository.PayAirdropOrder(player);
                            return;
                        case "AcceptBankTransfer":
                            MoneySystem.ATM.AcceptTransfer(player);
                            return;
                        case "CallGovMemberDialog":
                            Manager.AcceptCallFractionMemberDialog(player, (int) Fractions.Models.Fractions.CITY);
                            return;
                        case "CallArmyMemberDialog":
                            Manager.AcceptCallFractionMemberDialog(player, (int) Fractions.Models.Fractions.ARMY);
                            return;
                        case "CallEmsMemberDialog":
                            Manager.AcceptCallFractionMemberDialog(player, (int) Fractions.Models.Fractions.EMS);
                            return;
                        case "CallNewsMemberDialog":
                            Manager.AcceptCallFractionMemberDialog(player, (int) Fractions.Models.Fractions.LSNEWS);
                            return;
                        case "CallPoliceMemberDialog":
                            Manager.AcceptCallFractionMemberDialog(player, (int) Fractions.Models.Fractions.POLICE);
                            return;
                        case "CallSheriffMemberDialog":
                            Manager.AcceptCallFractionMemberDialog(player, (int) Fractions.Models.Fractions.SHERIFF);
                            return;
                        case "CallFibMemberDialog":
                            Manager.AcceptCallFractionMemberDialog(player, (int) Fractions.Models.Fractions.FIB);
                            return;
                        
                        case "buy_tent":

                            var inventoryTentData = sessionData.InventoryTentData;
                            if (inventoryTentData == null) return;

                            Chars.Repository.ItemBuy(player, inventoryTentData.ArrayName, inventoryTentData.Index, inventoryTentData.Value);
                            return;
                        case "sell_pet":
                            PedSystem.Pet.Repository.ConfirmSell(player, sessionData.SelectPed);
                            sessionData.SelectPed = null;
                            return;
                        case "Wedding":
                            var weddingData = sessionData.WeddingData;
                            sessionData.WeddingData = null;
                            if (weddingData == null)
                                return;
                            switch (weddingData.type)
                            {
                                case 0:
                                    var targetSessionData = weddingData.player.GetSessionData();
                                    if (targetSessionData == null)
                                        return;
                                    var targetCharacterData = weddingData.player.GetCharacterData();
                                    if (targetCharacterData == null)
                                        return;
                                    else if (targetCharacterData.WeddingUUID != 0)
                                        return;
                                    targetSessionData.WeddingData = new WeddingData
                                    {
                                        player = player,
                                        type = 0,
                                        typeSurname = weddingData.typeSurname
                                    };
                                    if (weddingData.typeSurname == 0) 
                                        Trigger.ClientEvent(weddingData.player, "openDialog", "Boda", LangFunc.GetText(LangType.Ru, DataName.YouWantWedding, player.Name, characterData.LastName)); 
                                    else 
                                        Trigger.ClientEvent(weddingData.player, "openDialog", "Boda", LangFunc.GetText(LangType.Ru, DataName.YouWantWeddingTo, player.Name)); 
                                    break;
                                case 1:
                                    targetCharacterData = weddingData.player.GetCharacterData();
                                    if (targetCharacterData != null && player.Position.DistanceTo(weddingData.player.Position) <= 5f)
                                        Quests.Wedding.OnDivorcio(player, isOne: false);
                                    break;
                                case 2:
                                    Quests.Wedding.OnDivorcio(player, isOne: true);
                                    break;
                            }
                            return;
                        case "Boda":
                            var bodaData = sessionData.WeddingData;
                            sessionData.WeddingData = null;
                            if (bodaData == null)
                                return;
                            Quests.Wedding.OnBoda(bodaData.player, player, bodaData.typeSurname);
                            return;
                        case "DelObjects":
                            if (!CommandsAccess.CanUseCmd(player, AdminCommands.DelObjects)) return;

                            if (!NeptuneEvo.Chars.Repository.ItemsData.ContainsKey(sessionData.DelObjects)) return;

                            NeptuneEvo.Chars.Repository.RemoveAll(sessionData.DelObjects);
                            GameLog.Admin($"{player.Name}", $"DelObjects({sessionData.DelObjects})", $"");
                            return;
                    }
                }
                else
                {
                    ExtPlayer seller = sessionData.SellItemData.Seller;
                    var sellerSessionData = seller.GetSessionData();
                    switch (callback)
                    {
                        case "ORGCAR_SELL_TOGOV":
                        case "CAR_SELL_TOGOV":
                            sessionData.CarSellGov = null;
                            break;
                        case "GUN_LIC":
                        case "PM_LIC":
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucCancelBuyLic), 3000);
                            if (sellerSessionData == null)
                            {
                                sessionData.SellItemData = new SellItemData();
                                return;
                            }
                            Notify.Send(seller, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerCancelBuyLic), 3000);
                            sellerSessionData.SellItemData = new SellItemData();
                            sessionData.SellItemData = new SellItemData();
                            break;
                        case "BUSINESS_BUY":
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCancelBuyBiz), 3000);
                            if (sellerSessionData == null)
                            {
                                sessionData.SellItemData = new SellItemData();
                                return;
                            }
                            Notify.Send(seller, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerCancelBuyBiz), 3000);
                            sellerSessionData.SellItemData = new SellItemData();
                            sessionData.SellItemData = new SellItemData();
                            return;
                        case "FUEL_CAR":
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCancelZapravka), 3000);
                            if (sellerSessionData == null)
                            {
                                sessionData.SellItemData = new SellItemData();
                                return;
                            }
                            Notify.Send(seller, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerCancelFuel), 3000);
                            sellerSessionData.SellItemData = new SellItemData();
                            sessionData.SellItemData = new SellItemData();
                            return;
                        case "REPAIR_CAR":
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCancelRepairVeh), 3000);
                            if (sellerSessionData == null)
                            {
                                sessionData.SellItemData = new SellItemData();
                                return;
                            }
                            Notify.Send(seller, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerCancelRepairVeh), 3000);
                            sellerSessionData.SellItemData = new SellItemData();
                            sessionData.SellItemData = new SellItemData();
                            return;
                        case "HOUSE_SELL":
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCancelBuyHome), 3000);
                            if (sellerSessionData == null)
                            {
                                sessionData.SellItemData = new SellItemData();
                                return;
                            }
                            Notify.Send(seller, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerCancelBuyHome), 3000);
                            sellerSessionData.SellItemData = new SellItemData();
                            sessionData.SellItemData = new SellItemData();
                            return;
                        case "PAY_HEAL":
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCancelHeal), 3000);
                            if (sellerSessionData == null)
                            {
                                sessionData.SellItemData = new SellItemData();
                                return;
                            }
                            Notify.Send(seller, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerCancelHeal), 3000);
                            sellerSessionData.SellItemData = new SellItemData();
                            sessionData.SellItemData = new SellItemData();
                            return;
                        case "PAY_MEDKIT":
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCancelMedkit), 3000);
                            if (sellerSessionData == null)
                            {
                                sessionData.SellItemData = new SellItemData();
                                return;
                            }
                            Notify.Send(seller, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerCancelMedkit), 3000);
                            sellerSessionData.SellItemData = new SellItemData();
                            sessionData.SellItemData = new SellItemData();
                            return;
                        case "BUY_CAR":
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCancelVeh), 3000);
                            if (sellerSessionData == null)
                            {
                                sessionData.SellItemData = new SellItemData();
                                return;
                            }
                            Notify.Send(seller, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerCancelVeh), 3000);
                            sellerSessionData.SellItemData = new SellItemData();
                            sessionData.SellItemData = new SellItemData();
                            return;
                        case "DICE_PLAY":
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCancelDice), 3000);
                            if (sellerSessionData == null)
                            {
                                sessionData.DiceData = new DiceData();
                                return;
                            }
                            Notify.Send(seller, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerCancelDice), 3000);
                            sellerSessionData.DiceData = new DiceData();
                            sessionData.DiceData = new DiceData();
                            return;
                        case "ROOM_INVITE":
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCancelHomeGo), 3000);
                            if (sellerSessionData == null)
                            {
                                sessionData.SellItemData = new SellItemData();
                                return;
                            }
                            Notify.Send(seller, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerCancelHomeGo), 3000);
                            sellerSessionData.SellItemData = new SellItemData();
                            sessionData.SellItemData = new SellItemData();
                            return;
                        case "BUS_RENT":
                        case "MOWER_RENT":
                        case "TAXI_RENT":
                        case "TRUCKER_RENT":
                        case "RENT_CAR":
                            VehicleManager.WarpPlayerOutOfVehicle(player);
                            return;
                        case "TAXI_PAY":
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCancelPayBus), 3000);
                            if (sellerSessionData == null)
                            {
                                sessionData.SellItemData = new SellItemData();
                                return;
                            }
                            Notify.Send(seller, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerCancelPayBus), 3000);
                            sellerSessionData.SellItemData = new SellItemData();
                            sessionData.SellItemData = new SellItemData();
                            return;
                        case "TICKET":
                            FractionCommands.ticketConfirm(player, false);
                            return;
                        /*case "CONFIRM_VEHICLE_TRADE":
                            Chars.Repository.TradeVehicleConfirmed(player, false);
                            return;
                        case "CONFIRM_HOUSE_TRADE":
                            Chars.Repository.TradeHouseConfirmed(player, false);
                            return;
                        case "CONFIRM_BUSINESSES_TRADE":
                            Chars.Repository.TradeBusinessesConfirmed(player, false);
                            return;*/
                        case "ChangeAutoNumberConfirm":
                            Chars.Repository.ChangeAutoNumberConfirm(player, false);
                            return;
                        default:
                            // Not supposed to end up here. 
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_DialogCallback Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("playerPressCuffBut")]
        public void ClientEvent_playerPressCuffBut(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.AdminLVL == 0 && !player.IsFractionAccess(RankToAccess.Cuff, false) && !player.IsOrganizationAccess(RankToAccess.Cuff)) return;
                FractionCommands.playerPressCuffBut(player);
            }
            catch (Exception e)
            {
                Log.Write($"ClientEvent_playerPressCuffBut Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("cuffUpdate")]
        public void ClientEvent_cuffUpdate(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!sessionData.DeathData.InDeath && sessionData.AttachToVehicle == null && player.Vehicle == null)
                {
                    Trigger.PlayAnimation(player, "mp_arresting", "idle", 49);
                    // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "cuff");
                }
            }
            catch (Exception e)
            {
                Log.Write($"ClientEvent_cuffUpdate Exception: {e.ToString()}");
            }
        }
        #endregion

        private static bool IsStartGameMode = false;
        private static int GetTime(int addDay = 0, bool isRealTime = false)
        {
            var dateTime = DateTime.Now;
            if (!isRealTime)
                dateTime = DateTime.Now.AddDays(addDay);
            
            Console.WriteLine(new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, isRealTime ? dateTime.Hour : 0, isRealTime ? dateTime.Minute : 0, isRealTime ? dateTime.Second : 0));
            return (Int32) (new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, isRealTime ? dateTime.Hour : 0, isRealTime ? dateTime.Minute : 0, isRealTime ? dateTime.Second : 0).Subtract(
                new DateTime(1970, 1, 1))).TotalSeconds;
        }
        public Main()
        {
            try
            {
                NAPI.Task.Run(async () =>
                {
                    while (true)
                    {
                        await NAPI.Task.WaitForMainThread(100);
                        
                        foreach (var player in NAPI.Pools.GetAllPlayers().Cast<ExtPlayer>())
                        {
                            if (player.Dimension == 0 && player.CharacterData == null && player.Position.X != 0)
                            {
                                Trigger.SendToAdmins(1, $"~r~ Игрок {player.Name} ({player.Id}) {player.Address} автоматически отпритонен и кикнут с сервера.");
                                player.Kick();
                            }
                        }
                    }
                });
                
                /*var dateTime = DateTime.Now;
                var minute = dateTime.Minute;
                var hour = dateTime.Hour;
                
                for (var i = 0; i < 56; i++)
                {
                    minute += 30;
                
                    if (minute > 30)
                    {
                        if (++hour > 23)
                            hour = 0;
                    
                        minute = 0;
                    }
                    
                    Console.WriteLine($"hour - {hour}:{minute} - {VehicleManager.GenerateNumber()}");
                }*/
                
                //Console.WriteLine(DateTime.Now.ToString("dd.MM HH:mm"));
                
                //Console.WriteLine($"{Utils.Base36.Reposotory.Decode(Utils.Base36.Reposotory.Encode(123))}");

                
                Library.Init();
                
                LoadServerSettings();

                VehicleData.LocalData.Repository.Init();
                
                RAGE.Entities.Players.CreateEntity = (NetHandle netHandle) => new ExtPlayer(netHandle);
                RAGE.Entities.Vehicles.CreateEntity = (NetHandle netHandle) => new ExtVehicle(netHandle);
                RAGE.Entities.Peds.CreateEntity = (NetHandle netHandle) => new ExtPed(netHandle);
                RAGE.Entities.Objects.CreateEntity = (NetHandle netHandle) => new ExtObject(netHandle);
                RAGE.Entities.Colshapes.CreateEntity = (NetHandle netHandle) => new ExtColShape(netHandle);
                RAGE.Entities.Blips.CreateEntity = (NetHandle netHandle) => new ExtBlip(netHandle);
                RAGE.Entities.Checkpoints.CreateEntity = (NetHandle netHandle) => new ExtCheckpoint(netHandle);
                RAGE.Entities.DummyEntities.CreateEntity = (NetHandle netHandle) => new ExtDummyEntity(netHandle);
                RAGE.Entities.Markers.CreateEntity = (NetHandle netHandle) => new ExtMarker(netHandle);
                RAGE.Entities.Pickups.CreateEntity = (NetHandle netHandle) => new ExtPickup(netHandle);
                RAGE.Entities.TextLabels.CreateEntity = (NetHandle netHandle) => new ExtTextLabel(netHandle);
                
                NAPI.Server.SetAutoSpawnOnConnect(false);
                NAPI.Server.SetCommandErrorMessage("Команда не найдена!");
                NAPI.Server.SetGlobalDefaultCommandMessages(true);
                //NAPI.Resource.StartResource("Командаasdasd");//На перезагрузку

                Thread.CurrentThread.Name = "Main";
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                Timers.StartOnceTask(0, BackupSql.Backup);

                MySQL.Init();

                MySQL.Query("SET GLOBAL max_connections = 500;"); //500
                
                
                //Console.WriteLine(JsonConvert.SerializeObject(configMySQL));
                // Настройка Mysql соединение
                #region MySQL Connect
                
                var mainDB = Settings.ReadAsync("mainDB", new MysqlSettings());
                
                List<IConnectionStringSettings> connectionStrings = new List<IConnectionStringSettings>();
                
                connectionStrings.Add(new ConnectionInfo("MainDB", mainDB.Server,
                    mainDB.User,
                    mainDB.Password,
                    mainDB.DataBase));
                    
                connectionStrings.Add(new ConnectionInfo("ConfigDB", mainDB.Server,
                    mainDB.User,
                    mainDB.Password,
                    $"{mainDB.DataBase}config"));
                
                //connectionStrings.Add(new ConnectionInfo("WebSiteBD", "145.239.90.140", "redage_dev", "Do5Hy1Cu4", "website"));

                foreach (var otherDB in mainDB.OtherList)
                {
                    connectionStrings.Add(new ConnectionInfo(otherDB.Name, otherDB.Server,
                        otherDB.User,
                        otherDB.Password,
                        otherDB.DataBase));
                }
                
                #endregion

                DataConnection.DefaultSettings = new DatabaseSettings(connectionStrings.ToArray());
                DataConnection.TurnTraceSwitchOn();
                DataConnection.WriteTraceLine = (message, category, level) =>
                {
                    if (IsStartGameMode && Thread.CurrentThread.Name == "Main")
                    {                
                        try
                        {
                            using (var save = new StreamWriter("mainDB.txt", true, Encoding.UTF8))
                            {
                                save.Write($"[{DateTime.Now}] 1-----------------1\r\n");
                                save.Write($"{message}\r\n");
                                save.Write($"[{DateTime.Now}] 2-----------------2\r\n\n");
                                save.Close();
                            }
                        }
                        catch (Exception e)
                        {
                            Timers.Log.Write($"SaveMainDB Exception: {e.ToString()}");
                        }
                    }
                };
                
                using (var db = new ServerBD("MainDB"))//Test connection
                {
                    try
                    {
                        //LinqToDB.Common.Internal.Cache.MemoryCache
                        Log.Write($"Connect to Main DataBase - {db.Accounts.Count()}");
                    }
                    catch
                    {
                        Log.Write($"No Connect To Main DataBase");
                    }
                }

                foreach (var otherDB in mainDB.OtherList)
                {
                    using (var db = new ServerBD(otherDB.Name))//Test connection
                    {
                        try
                        {
                            //LinqToDB.Common.Internal.Cache.MemoryCache
                            Log.Write($"Connect to {otherDB.Name} DataBase - {db.Accounts.Count()}");
                        }
                        catch
                        {
                            Log.Write($"No Connect To {otherDB.Name} DataBase");
                        }
                    }
                }
                
                using (var db = new ConfigBD("ConfigDB"))
                {
                    try
                    {
                        Log.Write($"Connect to Main Config - {db.ClothesFemaleTops.Count()}");
                    }
                    catch
                    {
                        Log.Write($"No Connect To Main Config");
                    }
                }
                
                /*using (var db = new WebSiteBD("WebSiteBD"))
                {
                    try
                    {
                        Log.Write($"Connect to WebSiteBD - {db.VerifyConfirm.Count()}");
                        
                        db.VerifyConfirm
                            .Where(vc => vc.ServerId == ServerNumber)
                            .Delete();
                        
                    }
                    catch
                    {
                        Log.Write($"No Connect To Main Config");
                    }
                }*/
                
                /* // Перенос больше не нужен
                using (var db = new ServerBD("rrp2"))
                {
                    try
                    {
                        Log.Write($"Connect to WHITE DataBase - {db.Accounts.Count()}");
                    }
                    catch
                    {
                        Log.Write($"No Connect To WHITE DataBase");
                    }
                }
                using (var db = new ServerBD("rrp3"))
                {
                    try
                    {
                        Log.Write($"Connect to RED DataBase - {db.Accounts.Count()}");
                    }
                    catch
                    {
                        Log.Write($"No Connect To RED DataBase");
                    }
                }
                */
                
                Utils.Redis.Repository.Init();
                
                Timers.Init();

                // Google Analytics (high cpu load)
                //Utils.Analytics.HelperThread.Start();
                
                GameLog.DisconnectAll();

                Economy.Init();

                Chars.Repository.InitItems();
                Chars.Repository.InitRoulette();

                BattlePass.Repository.Init();
                
                Players.Queue.Repository.Start();

                MoneySystem.Bank.Init();
                
                Players.Phone.Auction.Repository.Load();
                
                BusinessManager.InitBusProducts();
                
                BusinessManager.Init();

                GarageManager.Init();
                FurnitureManager.Init();
                HouseManager.Init();

                VehicleManager.Init();
                
                Players.Phone.Forbes.Repository.UpdateData();
                
                ReportSys.Init();
                
                Fractions.LSNews.LsNewsSystem.Init();
                
                EventSys.Init();

                Festive.Init();
                fTable.Init();

                MoneySystem.Casino.OnResourceStart();

                NewCasino.Blackjack.Init();
                NewCasino.Horses.init();
                Chars.Repository.ChangeAutoNumberInit();
                ElectionsSystem.OnResourceStart();

                Island.Init();

                InitDoorsControl();
                
                Database.Models.Repository.InitSave();

                IsStartGameMode = true;
            }
            catch (Exception e)
            {
                Log.Write($"Main Exception: {e.ToString()}");
            }
        }
        private void GarbageCollector()
        {
            Log.Write($"Start Clear");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Log.Write($"End Clear");
        }

        public static DateTime ServerDateTime = DateTime.Now;
        
        private int TickServer = 0;
        [ServerEvent(Event.Update)]
        public void OnUpdate() => TickServer++;

        private void enviromentChangeTrigger()
        {
            try
            {
                TickServer = 0;

                if (ServerDateTime.Minute != DateTime.Now.Minute)
                {
                    var eventStatus = Festive.isEvent;
                    if (eventStatus)
                    {
                        Festive.isEvent = DateTime.Now < Festive.DateTimeEnd ? true : false;
                        if (!Festive.isEvent) 
                            Trigger.ClientEventForAll("EndEvent");
                    }
                    ServerDateTime = DateTime.Now;
                    Trigger.ClientEventForAll("DateTime", JsonConvert.SerializeObject(ServerDateTime));
                }

                World.Weather.Repository.Set();

                Core.VehicleManager.DeathControler();

                MafiaGame.MafiaLobbiesStartInterval();
                Airsoft.AirsoftLobbiesStartInterval();
                TankRoyale.TanksLobbiesStartInterval();

                MoneySystem.DonatePack.DeleteTime();
                
                World.War.Repository.Unix();
            }
            catch (Exception e)
            {
                Log.Write($"enviromentChangeTrigger Exception: {e.ToString()}");
            }
        }


        [RemoteEvent("IsCompensation")]
        public static void Compensation(ExtPlayer player)
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    var accountData = player.GetAccountData();
                    if (accountData == null) 
                        return;

                    var characterData = player.GetCharacterData();
                    if (characterData == null) 
                        return;

                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    var compensation = await db.Compensation
                        .Where(bc => bc.UUID == characterData.UUID || bc.Login.ToLower() == accountData.Login.ToLower())
                        .Where(bc => bc.Toggled == true)
                        .FirstOrDefaultAsync();

                    if (compensation != null)
                    {
                        await db.Compensation
                            .Where(bc => bc.AutoId == compensation.AutoId)
                            .Set(bc => bc.Toggled, false)
                            .UpdateAsync();

                        if (compensation.Money > 0)
                        {
                            GameLog.Money("system", $"player({characterData.UUID})", compensation.Money, $"Compensation({compensation.AutoId})");
                            MoneySystem.Wallet.Change(player, compensation.Money);
                        }
                        if (compensation.Donate > 0)
                        {
                            UpdateData.RedBucks(player, compensation.Donate, msg: $"получил компенсацию в виде ({compensation.Donate})");
                            GameLog.Money("system", $"player({characterData.UUID})", compensation.Donate, "CompensationDonate");
                        }
                        if (compensation.ItemID != (int)ItemId.Debug)
                        {
                            Chars.Repository.AddNewItemWarehouse(player, (ItemId)compensation.ItemID, 1, compensation.Data);
                            GameLog.Money("system", $"player({characterData.UUID})", 1, $"CompensationItem({compensation.ItemID},{compensation.Data})");
                        }
                        Dictionary<string, object> _NoteData = new Dictionary<string, object>();
                        _NoteData.Add("Type", 0);
                        _NoteData.Add("Name", compensation.Title);
                        _NoteData.Add("Text", compensation.Text);

                        Trigger.ClientEvent(player, "client.note.open", JsonConvert.SerializeObject(_NoteData));
                    }
                }
                catch (Exception e)
                {
                    Log.Write($"Compensation Exception: {e.ToString()}");
                }
            });
        }
        private static void OnlineReward(ExtPlayer player, byte hours, int redbuckses)
        {
            try
            {
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                UpdateData.RedBucks(player, redbuckses, msg: "Вознаграждение за дневной онлайн");
                Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.RedAge, LangFunc.GetText(LangType.Ru, DataName.BonusEverdayOnline, hours, redbuckses), DateTime.Now);
                //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BonusEverdayOnline, hours, redbuckses), 5000);
                EventSys.SendCoolMsg(player,"Система", "Награда за онлайн", $"{LangFunc.GetText(LangType.Ru, DataName.BonusEverdayOnline, hours, redbuckses)}", "", 10000);
                if (!characterData.Achievements[16])
                {
                    characterData.Achievements[16] = true;
                    Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RbBonus1), 30000);
                    Notify.SendToKey(player, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RbBonus2), 30000, 21);
                }
            }
            catch (Exception e)
            {
                Log.Write($"OnlineReward Exception: {e.ToString()}");
            }
        }

        private void ClearDataInNewDay()
        {
            //Обновление скринилки
            GameLog.Uniques(TodayUniqueHWIDs.Count, PlayersAtOnce);
            PlayersAtOnce = PlayerIdToEntity.Count;
            TodayUniqueHWIDs.Clear();
            //
            var vehiclesLocalData = RAGE.Entities.Vehicles.All.Cast<ExtVehicle>()
                .Where(v => v.VehicleLocalData != null)
                .Where(v => v.VehicleLocalData.Access == VehicleAccess.Fraction)
                .Where(v => v.VehicleLocalData.Petrol < 10)
                .ToList();

            foreach (var vehicle in vehiclesLocalData)
            {
                try
                {
                    var vehicleLocalData = vehicle.GetVehicleLocalData();
                    vehicleLocalData.Petrol = 10;
                    vehicle.SetSharedData("PETROL", vehicleLocalData.Petrol);
                }
                catch (Exception e)
                {
                    Log.Write($"FuelControl foreach Exception: {e.ToString()}");
                }
            }

            foreach (var house in HouseManager.Houses)
                house.ItemsGot = 5;
            
            //
            foreach (var business in BusinessManager.BizList.Values)
            {
                business.Pribil = 0;
                business.Zatratq = 0;
            }
        }
        
        private void playedMinutesTrigger()
        {
            try
            {
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");
                        await Players.Session.Repository.DeleteSessions(db);
                        await Character.Delete.Repository.DeleteCharacters(db);
                        //
                        await Accounts.Email.Repository.DeleteToTime();

                    }
                    catch (Exception e)
                    {
                        Log.Write($"playedMinutesTrigger Task Exception: {e.ToString()}");
                    }
                });
                var now = DateTime.Now;

                if (now.Hour == 22 && now.Minute == 0)
                    Utils.Analytics.HelperThread.AddEvent("server_online", DateTime.Now.ToString("s"), "-", Character.Repository.GetPlayers().Count);

                if (AutoRestart) // Check AutoRestart
                {
                    if (now.Hour == 6)
                    {
                        if (now.Minute == 0)
                        {
                            NAPI.Chat.SendChatMessageToAll("!{#FF0000}[AUTO RESTART] Дорогие игроки, в 06:15 произойдёт автоматическая перезагрузка сервера.");
                        }
                        else if (now.Minute == 5)
                        {
                            NAPI.Chat.SendChatMessageToAll("!{#FF0000}[AUTO RESTART] Дорогие игроки, напоминаем, что в 06:15 произойдёт автоматическая перезагрузка сервера.");
                        }
                        else if (now.Minute == 10)
                        {
                            if (ServerNumber != 0)
                            {
                                NewCasino.Blackjack.BlackJackWorking = false;
                                NewCasino.Roullete.RouletteWorking = false;
                                NewCasino.Spin.SpinsWorking = false;
                                NewCasino.Horses.HorsesWorking = false;
                            }
                            NAPI.Chat.SendChatMessageToAll("!{#FF0000}[AUTO RESTART] Дорогие игроки, напоминаем, что в 06:15 произойдёт автоматическая перезагрузка сервера.");

                        }
                        else if (now.Minute == 15 || now.Minute == 16)
                        {
                            if (!Admin.IsServerStoping && ServerNumber != 0)
                            {
                                NAPI.Chat.SendChatMessageToAll("!{#FF0000}[AUTO RESTART] Дорогие игроки, сейчас произойдёт автоматическая перезагрузка сервера, сервер будет доступен вновь примерно в течение пары минут.");
                                Admin.stopServer("Происходит автоматическая перезагрузка, будем рады видеть Вас снова через пару минут! :)");
                            }
                        }
                    }
                }

                Events.AirDrop.Repository.CreateEvent();
                Inventory.Tent.Repository.MinuteTimer();

                if (now.Minute == 45)
                    Events.HeliCrash.Repository.Create();

                DayOfWeek dayofweek = now.DayOfWeek;
                bool IsNewWeek = dayofweek != EverydayAward.InitDayOfWeek && dayofweek == DayOfWeek.Monday;
                if (EverydayAward.InitDayOfWeek != dayofweek)
                {
                    EverydayAward.InitDayOfWeek = dayofweek;
                    ClearDataInNewDay();
                    Table.Tasks.Repository.UpdateOrg();
                    Organizations.Manager.NewDay();
                }
                if (IsNewWeek)
                {
                    WeekInfo = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                    EverydayAward.UpdateEverydayAwardItems();
                    Stocks.CargobobMats = Stocks.MaxCargobobMats;
                }
                if (dayofweek == DayOfWeek.Wednesday || dayofweek == DayOfWeek.Friday || dayofweek == DayOfWeek.Sunday)
                {
                    if (now.Hour == 19 && now.Minute == 45)
                    {
                        NAPI.Chat.SendChatMessageToAll(LangFunc.GetText(LangType.Ru, DataName.MatwarStart)); ;
                        EventSys.SendPlayersToEvent("MATWAR", "Война за материалы!", $"В 20:00 начнется война за материалы! Фракции, приготовьтесь!", "", 15000);
                    }
                    else if (now.Hour == 20 && now.Minute == 0 && !MatsWar.isWar)
                    {
                        MatsWar.startWar();
                    }
                }
                int thisminute = now.Minute;
                foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                {
                    var foreachSessionData = foreachPlayer.GetSessionData();
                    if (foreachSessionData == null)
                        continue;
                    if (!foreachSessionData.IsConnect)
                        continue;

                    var foreachAccountData = foreachPlayer.GetAccountData();
                    if (foreachAccountData == null) continue;

                    var foreachCharacterData = foreachPlayer.GetCharacterData();
                    if (foreachCharacterData == null) continue;

                    if (foreachCharacterData.AdminLVL >= 1 && foreachCharacterData.AdminLVL <= 8)
                    {
                        if (foreachSessionData.AdminData.LastPunishMinute != thisminute)
                        {
                            foreachSessionData.AdminData.MuteCount = 0;
                            foreachSessionData.AdminData.KickCount = 0;
                            foreachSessionData.AdminData.JailCount = 0;
                            foreachSessionData.AdminData.WarnCount = 0;
                            foreachSessionData.AdminData.BansCount = 0;
                            foreachSessionData.AdminData.AclearCount = 0;
                            foreachSessionData.AdminData.LastPunishMinute = thisminute;

                        }
                    }

                    foreachCharacterData.LastHourMin++;
                    foreachCharacterData.Time = GetCurrencyTime(foreachPlayer, foreachCharacterData.Time);
                    foreachCharacterData.Time.TotalTime++;
                    foreachCharacterData.Time.TodayTime++;
                    foreachCharacterData.Time.WeekTime++;
                    foreachCharacterData.Time.MonthTime++;
                    foreachCharacterData.Time.YearTime++;

                    foreachPlayer.UpdateFractionTime();
                    foreachPlayer.UpdateOrganizationTime();

                    BattlePass.Repository.UpdateTime(foreachPlayer);

                    if (foreachCharacterData.DemorganTime <= 0)
                    {
                        switch (foreachCharacterData.Time.TodayTime)
                        {
                            case 120:
                                OnlineReward(foreachPlayer, 2, 1);
                                break;
                            case 180:
                                Chars.Repository.AddNewItemWarehouse(foreachPlayer, ItemId.Case0, 1);
                                Trigger.ClientEvent(foreachPlayer, "client.roullete.updateCase", 1);
                                if (!foreachCharacterData.Achievements[17])
                                {
                                    foreachCharacterData.Achievements[17] = true;
                                    Notify.SendToKey(foreachPlayer, NotifyType.Alert, NotifyPosition.BottomCenter, $"В донат-меню в разделе Рулетка Вам стал доступен Бесплатный Ежедневный кейс. Бесплатно открыть его можно 1 раз в день, отыграв 3 часа.", 30000, 21);
                                }
                                break;
                            case 240:
                                OnlineReward(foreachPlayer, 4, 2);
                                break;
                            case 300:
                                Chars.Repository.AddNewItemWarehouse(foreachPlayer, ItemId.Case1, 1);
                                Trigger.ClientEvent(foreachPlayer, "client.roullete.updateCase", 2);
                                if (!foreachCharacterData.Achievements[23])
                                {
                                    foreachCharacterData.Achievements[23] = true;
                                    Notify.SendToKey(foreachPlayer, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Case0get), 30000, 21);
                                }
                                break;
                            case 480:
                                OnlineReward(foreachPlayer, 8, 4);
                                Chars.Repository.AddNewItemWarehouse(foreachPlayer, ItemId.Case2, 1);
                                if (!foreachCharacterData.Achievements[24])
                                {
                                    foreachCharacterData.Achievements[24] = true;
                                    Notify.SendToKey(foreachPlayer, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Case1get), 30000, 21);
                                }
                                break;
                        }
                    }
                    if (foreachCharacterData.Time.MonthTime == 18000)
                    {
                        UpdateData.RedBucks(foreachPlayer, 500, msg: "Вознаграждение за месячный онлайн");
                        Notify.Send(foreachPlayer, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Case2get), 5000);
                    }
                    if (foreachAccountData.VipLvl > 0 && foreachAccountData.VipDate <= DateTime.Now)
                    {
                        foreachAccountData.VipLvl = 0;
                        Notify.Send(foreachPlayer, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VipIsOver), 3000);
                    }

                    try
                    {
                        if (foreachSessionData.RentData != null && !foreachSessionData.RentData.IsJob && now > foreachSessionData.RentData.Date)
                        {
                            Rentcar.OnReturnVehicle(foreachPlayer);
                            Notify.Send(foreachPlayer, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RentOver), 3000);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Write($"Rent Vehicle Delete Exception: {e.ToString()}");
                    }
                }

                VehicleManager.VehiclesDestroy();

                Players.Phone.Forbes.Repository.UpdateData();

                Players.Phone.Auction.Repository.GetEnd();
            }
            catch (Exception e)
            {
                Log.Write($"playedMinutesTrigger Exception: {e.ToString()}");
            }
        }
        public static TimeInfo GetCurrencyTime(ExtPlayer player, TimeInfo Time)
        {
            var accountData = player.GetAccountData();
            DateTime now = DateTime.Now;
            int thisday = now.Day;
            int thismonth = now.Month;
            int thisyear = now.Year;

            if (Time.Year != thisyear)
            {
                Time.Year = thisyear;
                Time.YearTime = 0;
            }
            
            if (Time.Month != thismonth)
            {
                Time.Month = thismonth;
                Time.MonthTime = 0;
            }
            
            if (Time.Day != thisday)
            {
                Time.Day = thisday;
                Time.TodayTime = 0;
                if (accountData != null)
                {
                    accountData.Unique = Donate.SetUnique(null);
                    Trigger.ClientEvent(player, "client.accountStore.Unique", accountData.Unique, true);
                    
                    BattlePass.Repository.UpdateDay(player);
                }
            }
            
            if (Time.Week != WeekInfo)
            {
                Time.Week = WeekInfo;
                Time.WeekTime = 0;
                
                if (accountData != null)
                    BattlePass.Repository.UpdateWeek(player);
            }
            return Time;
        }
        public static void CheckMyBonusCode(ExtPlayer player, string bonuscode)
        {
            try
            {
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.AdminLVL >= 1 && characterData.AdminLVL <= 7) return;
                if (bonuscode.Length == 0) return;
                string text = bonuscode.ToLower();
                if (BonusCodes.ContainsKey(text))
                {
                    lock (BonusCodes)
                    {
                        if (accountData.BonusCodes.Contains(text))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BonusUsed), 5000);
                            return;
                        }
                        BonusCodesData pcdata = BonusCodes[text];
                        if (pcdata.UsedLimit == 0 || pcdata.UsedTimes < pcdata.UsedLimit)
                        {
                            Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", ItemId.Present, 1);
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OpenGift), 3000);
                            if (characterData.Gender) Commands.RPChat("sme", player, LangFunc.GetText(LangType.Ru, DataName.OpenedGift));
                            else Commands.RPChat("sme", player, LangFunc.GetText(LangType.Ru, DataName.OpenedaGift));
                            accountData.BonusCodes.Add(text);
                            GameLog.Money($"server", $"player({characterData.UUID})", pcdata.RewardMoney, $"BonusReward({text})");
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, pcdata.RewardMessage, 15000);
                            pcdata.UsedTimes++;

                            Trigger.SetTask(async () =>
                            {
                                try
                                {
                                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                                    await db.Bonuscodes
                                        .Where(v => v.Code == text)
                                        .Set(v => v.Used, pcdata.UsedTimes)
                                        .UpdateAsync();
                                }
                                catch (Exception e)
                                {
                                    Debugs.Repository.Exception(e);
                                }
                            });
                            
                            if (pcdata.RewardMoney != 0)
                            {
                                MoneySystem.Wallet.Change(player, (int)pcdata.RewardMoney);
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"[BONUS] +{pcdata.RewardMoney}$", 15000);
                            }
                            if (pcdata.RewardVipLvl != 0)
                            {
                                if (accountData.VipLvl == 0)
                                {
                                    switch (pcdata.RewardVipLvl)
                                    {
                                        case 1:
                                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BronzeBonus, pcdata.RewardVipDays), 15000);
                                            break;
                                        case 2:
                                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SilverBonus, pcdata.RewardVipDays), 15000);
                                            break;
                                        case 3:
                                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GoldBonus, pcdata.RewardVipDays), 15000);
                                            break;
                                        case 4:
                                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlatinumBonus, pcdata.RewardVipDays), 15000);
                                            break;
                                        default:
                                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ErrorBonus, pcdata.RewardVipDays), 15000);
                                            break;
                                    }
                                    accountData.VipLvl = pcdata.RewardVipLvl;
                                    accountData.VipDate = DateTime.Now.AddDays(pcdata.RewardVipDays);
                                    GameLog.Money($"system", $"player({characterData.UUID})", 0, $"bonusVIP({accountData.VipLvl}lvl, {pcdata.RewardVipDays}d, стало до {accountData.VipDate.ToString("s")})");
                                }
                                else if (accountData.VipLvl == pcdata.RewardVipLvl)
                                {
                                    switch (pcdata.RewardVipLvl)
                                    {
                                        case 1:
                                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BronzeBonus, pcdata.RewardVipDays), 15000);
                                            break;
                                        case 2:
                                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SilverBonus, pcdata.RewardVipDays), 15000);
                                            break;
                                        case 3:
                                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GoldBonus, pcdata.RewardVipDays), 15000);
                                            break;
                                        case 4:
                                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlatinumBonus, pcdata.RewardVipDays), 15000);
                                            break;
                                        default:
                                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ErrorBonus, pcdata.RewardVipDays), 15000);
                                            break;
                                    }
                                    accountData.VipDate = accountData.VipDate.AddDays(pcdata.RewardVipDays);
                                    GameLog.Money($"system", $"player({characterData.UUID})", 0, $"bonusVIP({accountData.VipLvl}lvl, {pcdata.RewardVipDays}d, стало до {accountData.VipDate.ToString("s")})");
                                }
                                else
                                {
                                    uint days = 0;
                                    switch (pcdata.RewardVipDays)
                                    {
                                        case 1:
                                        case 2:
                                        case 3:
                                            if (accountData.VipLvl > pcdata.RewardVipLvl) days = 1;
                                            else days = 4;
                                            break;
                                        case 4:
                                        case 5:
                                            if (accountData.VipLvl > pcdata.RewardVipLvl) days = 2;
                                            else days = 8;
                                            break;
                                        case 6:
                                        case 7:
                                            if (accountData.VipLvl > pcdata.RewardVipLvl) days = 3;
                                            else days = 12;
                                            break;
                                        case 30:
                                            if (accountData.VipLvl > pcdata.RewardVipLvl) days = 15;
                                            else days = 30;
                                            break;
                                        default:
                                            if (accountData.VipLvl > pcdata.RewardVipLvl) days = 5;
                                            else days = 10;
                                            break;
                                    }
                                    accountData.VipDate = accountData.VipDate.AddDays(days);
                                    GameLog.Money($"system", $"player({characterData.UUID})", 0, $"bonusVIP({accountData.VipLvl}lvl, {days}d, стало до {accountData.VipDate.ToString("s")})");
                                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DaysDop, days), 15000);
                                }
                                //if (pcdata.RewardExp == 0) Chars.Repository.PlayerStats(player);
                            }
                            if (characterData.Gender)
                            {
                                if (pcdata.RewardItemsMale.Count != 0)
                                {
                                    foreach (InventoryItemData item in pcdata.RewardItemsMale)
                                    {
                                        try
                                        {
                                            WeaponRepository.GiveWeapon(player, item.ItemId, item.Data, item.Count);
                                        }
                                        catch (Exception e)
                                        {
                                            Log.Write($"CheckMyBonusCode Foreach #1 Exception: {e.ToString()}");
                                        }
                                    }
                                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BonusItems, pcdata.RewardItemsMale.Count), 15000);
                                }
                            }
                            else
                            {
                                if (pcdata.RewardItemsFemale.Count != 0)
                                {
                                    foreach (InventoryItemData item in pcdata.RewardItemsFemale)
                                    {
                                        try
                                        {
                                            WeaponRepository.GiveWeapon(player, item.ItemId, item.Data, item.Count);
                                        }
                                        catch (Exception e)
                                        {
                                            Log.Write($"CheckMyBonusCode Foreach #2 Exception: {e.ToString()}");
                                        }
                                    }
                                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BonusItems, pcdata.RewardItemsFemale.Count), 15000);
                                }
                            }
                            if (pcdata.RewardExp != 0)
                            {
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"[BONUS] +{pcdata.RewardExp} EXP", 15000);
                                UpdateData.Exp(player, pcdata.RewardExp);
                            }
                        }
                        else if (pcdata.UsedLimit != 0 && pcdata.UsedTimes >= pcdata.UsedLimit) Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BonusEnd, pcdata.UsedLimit), 3000);
                    }
                }
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoBonusMore), 5000);
            }
            catch (Exception e)
            {
                Log.Write($"CheckMyBonusCode Exception: {e.ToString()}");
            }
        }
        public static void CheckMyPromoCode(ExtPlayer player, bool sendstats = true)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null)
                    return;
                
                var accountData = player.GetAccountData();
                if (accountData == null) 
                    return;
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                if (accountData.PromoCodes[0] != "noref" && !accountData.PresentGet)
                {
                    string promo = accountData.PromoCodes[0];
                    if (PromoCodes.ContainsKey(promo))
                    {
                        lock (PromoCodes)
                        {
                            accountData.PresentGet = true;
                            PromoCodesData pcdata = PromoCodes[promo];
                            if (pcdata.RewardLimit == 0 || pcdata.RewardReceived < pcdata.RewardLimit)
                            {
                                GameLog.Money($"server", $"player({characterData.UUID})", pcdata.RewardMoney, $"PromoReward({promo})");
                                if (pcdata.RewardLimit != 0)
                                {
                                    pcdata.RewardReceived++;
                                    
                                    Trigger.SetTask(async () =>
                                    {
                                        try
                                        {
                                            await using var db = new ServerBD("MainDB");//В отдельном потоке

                                            await db.PromocodesNew
                                                .Where(v => v.Promo == promo)
                                                .Set(v => v.Rewardreceived, pcdata.RewardReceived)
                                                .UpdateAsync();
                                        }
                                        catch (Exception e)
                                        {
                                            Debugs.Repository.Exception(e);
                                        }
                                    });
                                }
                                if (pcdata.RewardMoney != 0) MoneySystem.Wallet.Change(player, (int)pcdata.RewardMoney);
                                if (pcdata.RewardVipLvl != 0)
                                {
                                    if (accountData.VipLvl == 0)
                                    {
                                        accountData.VipLvl = pcdata.RewardVipLvl;
                                        accountData.VipDate = DateTime.Now.AddDays(pcdata.RewardVipDays);
                                        GameLog.Money($"system", $"player({characterData.UUID})", 0, $"promoCodeVIP({accountData.VipLvl}lvl, {pcdata.RewardVipDays}d, стало до {accountData.VipDate.ToString("s")})");
                                    }
                                    else if (accountData.VipLvl == pcdata.RewardVipLvl)
                                    {
                                        accountData.VipDate = accountData.VipDate.AddDays(pcdata.RewardVipDays);
                                        GameLog.Money($"system", $"player({characterData.UUID})", 0, $"promoCodeVIP({accountData.VipLvl}lvl, {pcdata.RewardVipDays}d, стало до {accountData.VipDate.ToString("s")})");
                                    }
                                    else
                                    {
                                        uint days = 0;
                                        switch (pcdata.RewardVipDays)
                                        {
                                            case 1:
                                            case 2:
                                            case 3:
                                                days = 1;
                                                break;
                                            case 4:
                                            case 5:
                                                days = 2;
                                                break;
                                            case 6:
                                            case 7:
                                                days = 3;
                                                break;
                                            default:
                                                days = 5;
                                                break;
                                        }
                                        accountData.VipDate = accountData.VipDate.AddDays(days);
                                        GameLog.Money($"system", $"player({characterData.UUID})", 0, $"promoCodeVIP({accountData.VipLvl}lvl, {days}d, стало до {accountData.VipDate.ToString("s")})");
                                        Trigger.SendChatMessage(player, $"Так как у Вас уже был VIP статус, то вместо его замены Вам было начислено дополнительно {days} дней использования!");
                                    }
                                    //if (sendstats) Chars.Repository.PlayerStats(player);
                                }
                                if (pcdata.RewardItems.Count != 0)
                                {
                                    foreach (InventoryItemData item in pcdata.RewardItems)
                                    {
                                        try
                                        {
                                            WeaponRepository.GiveWeapon(player, item.ItemId, item.Data, item.Count);
                                        }
                                        catch (Exception e)
                                        {
                                            Log.Write($"CheckMyPromoCode Foreach #1 Exception: {e.ToString()}");
                                        }
                                    }
                                }
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, pcdata.RewardMessage, 15000);
                                if (pcdata.CreatorUUID != 0)
                                {
                                    GameLog.Money($"server", $"player({pcdata.CreatorUUID})", 500, $"PromoOwnerReward({promo})");
                                    bool isGiven = false;
                                    ExtPlayer target = GetPlayerByUUID((int)pcdata.CreatorUUID);
                                    if (target != null)
                                    {
                                        MoneySystem.Wallet.Change(target, 500);
                                        Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NagradaLvlPromo, player.Name), 5000);
                                        isGiven = true;
                                    }
                                    if (!isGiven) 
                                        Character.Save.Repository.AddMoney((int) pcdata.CreatorUUID, 500);
                                    
                                }
                            }
                            else if (pcdata.RewardLimit != 0 && pcdata.RewardReceived >= pcdata.RewardLimit) 
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BonusEnd, pcdata.RewardLimit), 3000);
                        }
                    }
                    else
                    {
                        accountData.PromoCodes[0] = "noref";
                        Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BonusOver), 7000);
                    }
                }

                if (accountData.RefferalId != 0 && !accountData.RefPresentGet)
                {
                    if (characterData.LVL >= 3)
                    {
                        GameLog.Money($"server", $"player({characterData.UUID})", 5000, $"RefReward({accountData.RefferalId})");
                        accountData.RefPresentGet = true;
                        MoneySystem.Wallet.Change(player, 1500);
                        Chars.Repository.AddNewItemWarehouse(player, ItemId.Case0, 1);
                        Chars.Repository.AddNewItemWarehouse(player, ItemId.Case1, 1);
                        Chars.Repository.AddNewItemWarehouse(player, ItemId.Case2, 1);
                        UpdateData.RedBucks(player, 100, msg:"Подарок за ввод реф-кода");
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RefBonus, characterData.LVL), 15000);
                        ExtPlayer target = GetPlayerByUUID(accountData.RefferalId);

                        Trigger.SetTask(async () =>
                        {
                            try
                            {
                                await using var db = new ServerBD("MainDB");//В отдельном потоке

                                var count = await db.Refferals
                                    .Where(v => v.Uuidref == accountData.RefferalId && v.Success == true)
                                    .CountAsync();
                                
                                
                                RefferalData refferalData = null;
                                for (int i = 0; i < Chars.Repository.RefferalsData.Length; i++)
                                {
                                    if (Chars.Repository.RefferalsData[i].Count <= count) 
                                        refferalData = Chars.Repository.RefferalsData[i];
                                }
                                if (refferalData == null) 
                                    return;
                                
                                GameLog.Money($"server", $"player({accountData.RefferalId})", refferalData.Money, $"RefOwnerReward({count})");
                                var targetAccountData = target.GetAccountData();
                                if (targetAccountData != null)
                                {
                                    var successText = LangFunc.GetText(LangType.Ru, DataName.RefBonusGet, sessionData.Name);
                                    if (refferalData.VipId > 0)
                                    {
                                        successText += $" {Group.GroupNames[refferalData.VipId]} ({refferalData.VipDays} Дней);";
                                        Chars.Repository.UpdateVipStatus(target, refferalData.VipId, refferalData.VipDays, false, true, "RefVIP");
                                    }

                                    if (refferalData.FreeCoin)
                                    {
                                        successText += $" Бесплатная прокрутка;";
                                        Chars.Repository.AddNewItemWarehouse(target, ItemId.Case0, 1);
                                        Trigger.ClientEvent(target, "client.roullete.updateCase", 1);
                                    }
                                    if (refferalData.Money > 0)
                                    {
                                        successText += $" Игровая валюта (+{MoneySystem.Wallet.Format(refferalData.Money)}$);";
                                        MoneySystem.Wallet.Change(target, +refferalData.Money);
                                    }
                                    if (refferalData.RB > 0)
                                    {
                                        successText += $" RedBucks (+{MoneySystem.Wallet.Format(refferalData.RB)}RB);";
                                        UpdateData.RedBucks(target, refferalData.RB, msg:"Бонус за рефферала.");
                                    }
                                    //Chars.Repository.PlayerStats(RefferalClient);

                                    Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, successText, 5000);

                                    await db.Refferals
                                        .Where(r => r.Uuid == characterData.UUID)
                                        .Set(r => r.Success, true)
                                        .Set(r => r.Successdate, DateTime.Now)
                                        .UpdateAsync();
                                }
                                else
                                {
                                    string login = GetLoginFromUUID(accountData.RefferalId);

                                    if (login == null) 
                                        return;

                                    var account = await db.Accounts
                                        .Select(a => new
                                        {
                                            a.Login,
                                            a.Redbucks,
                                            a.Viplvl,
                                            a.Vipdate,
                                            a.RefferalId
                                        })
                                        .Where(a => a.Login.ToLower() == login.ToLower())
                                        .FirstOrDefaultAsync();
                                    
                                    if (account == null)
                                        return;
                                    
                                    var vipLvl = Convert.ToInt32(account.Viplvl);
                                    var vipDate = account.Vipdate;
                                    var refferalId = Convert.ToInt32(account.RefferalId);
                                    if (refferalData.VipId > 0)
                                    {
                                        if (accountData.VipLvl > 0)
                                        {
                                            double unixTime = TimeSpan.FromTicks(vipDate.Ticks - DateTime.Now.Ticks).TotalSeconds;
                                            if (vipLvl < refferalData.VipId)
                                            {
                                                double addTime = unixTime / ((refferalData.VipId + 1) - vipLvl);
                                                vipDate = DateTime.Now.AddSeconds((refferalData.VipDays * 86400) + addTime);
                                                vipLvl = refferalData.VipId;
                                            }
                                            else if (vipLvl > refferalData.VipId)
                                            {
                                                double addTime = unixTime / (vipLvl - (refferalData.VipId - 1));
                                                vipDate = DateTime.Now.AddSeconds((refferalData.VipDays * 86400) + addTime);
                                                vipLvl = refferalData.VipId;
                                            }
                                            else vipDate = vipDate.AddDays(refferalData.VipDays);
                                        }
                                        else
                                        {
                                            vipLvl = refferalData.VipId;
                                            vipDate = DateTime.Now.AddDays(refferalData.VipDays);
                                        }

                                        GameLog.Money($"server", $"player({refferalId})", 1, $"RefVIP({vipLvl}lvl, стало до: {vipDate.ToString("s")})");
                                    }
                                    
                                    await db.Refferals
                                        .Where(r => r.Uuid == characterData.UUID)
                                        .Set(r => r.Success, true)
                                        .Set(r => r.Successdate, DateTime.Now)
                                        .UpdateAsync();
                                    
                                    await db.Accounts
                                        .Where(a => a.Login.ToLower() == login.ToLower())
                                        .Set(a => a.Redbucks, account.Redbucks + refferalData.RB)
                                        .Set(a => a.Viplvl, vipLvl)
                                        .Set(a => a.Vipdate, vipDate)
                                        .UpdateAsync();

                                    var character = await db.Characters
                                        .Select(c => new {c.Uuid, c.Money})
                                        .Where(c => c.Uuid == accountData.RefferalId)
                                        .FirstOrDefaultAsync();

                                    if (character != null)
                                    {
                                        await db.Characters
                                            .Where(c => c.Uuid == character.Uuid)
                                            .Set(c => c.Money, character.Money + refferalData.Money)
                                            .UpdateAsync();
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Debugs.Repository.Exception(e);
                            }
                        });
                    }
                }
                
                if (accountData.PromoCodes[0] == "noref" && accountData.RefferalId == 0 && !accountData.PresentGet && !accountData.RefPresentGet) 
                    Trigger.SendChatMessage(player, "~r~Вы не указывали промокод или реф.код при регистрации, но всё еще можете активировать любой из промокодов через телефон (M -> Подарок).");
            }
            catch (Exception e)
            {
                Log.Write($"CheckMyPromoCode Exception: {e.ToString()}");
            }
        }
        public static void payDayTrigger(bool withlottery)
        {
            try
            {
                Log.Write("Payday time started!");
                Cityhall.lastHourTax = 0;
                if (withlottery)
                {
                    try
                    {
                        if (MoneySystem.Lottery.Price == 0) 
                            NAPI.Chat.SendChatMessageToAll(LangFunc.GetText(LangType.Ru, DataName.LotteryAnnouncer));
                        else
                        {
                            // Конец этой игры
                            int winnumb = rnd.Next(1, MoneySystem.Lottery.LotteryBought.Count() + 1);
                            int winuuid = MoneySystem.Lottery.LotteryBought[(uint)winnumb];
                            if (PlayerNames.ContainsKey(winuuid)) // Если персонаж победителя еще существует
                            {
                                var target = GetPlayerByUUID(winuuid);
                                if (target != null)
                                {
                                    MoneySystem.Wallet.Change(target, (int)MoneySystem.Lottery.Price);
                                    Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LotteryWinner), 5000);
                                }
                                else
                                    Character.Save.Repository.AddMoney(winuuid, (int) MoneySystem.Lottery.Price);
                                
                                GameLog.Money($"server", $"player({winuuid})", MoneySystem.Lottery.Price, $"LotteryWin({MoneySystem.Lottery.ID})");

                                NAPI.Chat.SendChatMessageToAll($"~g~[ЛОТЕРЕЯ] {PlayerNames[winuuid]} сорвал джекпот в размере {MoneySystem.Wallet.Format(MoneySystem.Lottery.Price)}$ с шансом {(MoneySystem.Lottery.LotteryBought.Where(p => p.Value == winuuid).Count() * 100.0 / MoneySystem.Lottery.LotteryBought.Count()).ToString("0.##")}%.");
                                NAPI.Chat.SendChatMessageToAll(LangFunc.GetText(LangType.Ru, DataName.LotteryTicketSold, MoneySystem.Lottery.LotteryBought.Count(), winnumb));

                                // Старт следующей игры
                                MoneySystem.Lottery.ID += 1;
                                MoneySystem.Lottery.Price = 0;
                                MoneySystem.Lottery.LotteryBought = new Dictionary<uint, int>();
                                Trigger.SetTask(async () =>
                                {
                                    try
                                    {
                                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                                        await db.Lottery
                                            .Set(v => v.Number, MoneySystem.Lottery.ID)
                                            .UpdateAsync();
                                    }
                                    catch (Exception e)
                                    {
                                        Debugs.Repository.Exception(e);
                                    }
                                });
                            }
                            else NAPI.Chat.SendChatMessageToAll("~g~[ЛОТЕРЕЯ] Из-за технической ошибки, розыгрыш будет проведён через час.");
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Write($"payDayTrigger #1 Exception: {e.ToString()}");
                    }
                }
                
                
                //

                var familyZonesSalary = new Dictionary<int, int>();
                var familyZonesPlayers = new Dictionary<int, int>();
                
                foreach (var familyZone in Organizations.FamilyZones.Repository.FamilyZones.Values.ToList())
                {
                    if (familyZone.OrganizationId == 0)
                        continue;

                    if (!familyZonesSalary.ContainsKey(familyZone.OrganizationId))
                        familyZonesSalary[familyZone.OrganizationId] = 0;

                    familyZonesSalary[familyZone.OrganizationId] += Organizations.FamilyZones.Repository.FamilyZoneMoney;
                }

                //
                
                foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                {
                    try
                    {
                        var foreachAccountData = foreachPlayer.GetAccountData();
                        if (foreachAccountData == null) continue;
                        var foreachCharacterData = foreachPlayer.GetCharacterData();
                        if (foreachCharacterData == null) continue;
                        if (foreachCharacterData.HotelID != -1)
                        {
                            if (foreachCharacterData.HotelLeft-- <= 0)
                            {
                                Hotel.MoveOutPlayer(foreachPlayer);
                                Notify.Send(foreachPlayer, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.HotelLeftNoMoney), 15000);
                            }
                        }

                        if (foreachCharacterData.LastHourMin < 15)
                        {
                            //Chars.Repository.PlayerStats(player);
                            GameLog.Money($"server", $"player({foreachCharacterData.UUID})", 0, "nopayday(15min)");
                            Notify.Send(foreachPlayer, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustPlay15MinForPd), 3000);
                            continue;
                        }
                        if (foreachAccountData.VipLvl == 3)
                        {
                            UpdateData.RedBucks(foreachPlayer, 2, msg:String.Empty);
                            Notify.Send(foreachPlayer, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Platinum2RB), 3000);
                        }
                        if (foreachAccountData.VipLvl == 4)
                        {
                            UpdateData.RedBucks(foreachPlayer, 5, msg:String.Empty);
                            Notify.Send(foreachPlayer, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Diamond5RB), 3000);
                        }
                        int payment = 0;
                        byte type = 0;

                        //
                        
                        var foreachOrganizationData = foreachPlayer.GetOrganizationData();
                        if (foreachOrganizationData != null &&
                            familyZonesSalary.ContainsKey(foreachOrganizationData.Id))
                        {
                            payment += Convert.ToInt32(((100 - foreachOrganizationData.Salary) / 100f) * familyZonesSalary[foreachOrganizationData.Id]);
                            
                
                            if (!familyZonesPlayers.ContainsKey(foreachOrganizationData.Id))
                                familyZonesPlayers[foreachOrganizationData.Id] = 0;

                            familyZonesPlayers[foreachOrganizationData.Id]++;
                        }

                        //
                        
                        var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                        if (foreachMemberFractionData != null)
                        {
                            switch (Manager.FractionTypes[foreachMemberFractionData.Id])
                            {
                                case FractionsType.None:
                                case FractionsType.Mafia:
                                case FractionsType.Gangs:
                                case FractionsType.Bikers:
                                    type = 1;
                                    if (foreachCharacterData.WorkID != 0) break;
                                    type = 2;
                                    if (foreachCharacterData.LVL < 30)
                                    {
                                        payment += Convert.ToInt32(
                                            (Main.PricesSettings.PosobieNew * ServerSettings.MoneyMultiplier) +
                                            (Donate.AddPayment(foreachAccountData.VipLvl) *
                                             ServerSettings.MoneyMultiplier));
                                        //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы получили пособие по безработице в виде {MoneySystem.Wallet.Format(payment)}$", 3000);
                                    }
                                    else
                                    {
                                        payment += Convert.ToInt32(
                                            (Main.PricesSettings.PosobieOld * ServerSettings.MoneyMultiplier) +
                                            (Donate.AddPayment(foreachAccountData.VipLvl) *
                                             ServerSettings.MoneyMultiplier));
                                        //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы получили выплату за выслугу лет в виде {MoneySystem.Wallet.Format(payment)}$", 3000);
                                    }

                                    MoneySystem.Wallet.Change(foreachPlayer, payment);
                                    break;
                                case FractionsType.Gov:
                                case FractionsType.Nongov: // merryweather
                                    type = 3;
                                    
                                    var fractionData = Manager.GetFractionData(foreachMemberFractionData.Id);
                                    if (fractionData != null)
                                    {
                                        payment += Convert.ToInt32(
                                            (fractionData.Ranks[
                                                 foreachMemberFractionData.Rank].Salary *
                                             ServerSettings.MoneyMultiplier) +
                                            (Donate.AddPayment(foreachAccountData.VipLvl) *
                                             ServerSettings.MoneyMultiplier));
                                        //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы получили зарплату в {MoneySystem.Wallet.Format(payment)}$", 3000);
                                        MoneySystem.Wallet.Change(foreachPlayer, payment);
                                    }

                                    break;
                            }
                        } 
                        else
                        {
                            type = 1;
                            if (foreachCharacterData.WorkID == 0)
                            {
                                type = 2;
                                if (foreachCharacterData.LVL < 30)
                                {
                                    payment += Convert.ToInt32(
                                        (Main.PricesSettings.PosobieNew * ServerSettings.MoneyMultiplier) +
                                        (Donate.AddPayment(foreachAccountData.VipLvl) *
                                         ServerSettings.MoneyMultiplier));
                                    //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы получили пособие по безработице в виде {MoneySystem.Wallet.Format(payment)}$", 3000);
                                }
                                else
                                {
                                    payment += Convert.ToInt32(
                                        (Main.PricesSettings.PosobieOld * ServerSettings.MoneyMultiplier) +
                                        (Donate.AddPayment(foreachAccountData.VipLvl) *
                                         ServerSettings.MoneyMultiplier));
                                    //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы получили выплату за выслугу лет в виде {MoneySystem.Wallet.Format(payment)}$", 3000);
                                }

                                MoneySystem.Wallet.Change(foreachPlayer, payment);
                            }
                        }

                        switch (type)
                        {
                            case 0:
                                GameLog.Money($"server", $"player({foreachCharacterData.UUID})", payment, "payday(exp)");
                                break;
                            case 1:
                                GameLog.Money($"server", $"player({foreachCharacterData.UUID})", payment, "payday(exp,noallowance)");
                                break;
                            case 2:
                                GameLog.Money($"server", $"player({foreachCharacterData.UUID})", payment, "payday(exp,allowance)");
                                break;
                            case 3:
                                GameLog.Money($"server", $"player({foreachCharacterData.UUID})", payment, "payday(exp,payday)");
                                break;
                        }
                        UpdateData.Exp(foreachPlayer, 1 * Group.GroupEXP[foreachAccountData.VipLvl] * ServerSettings.ExpMultiplier, payment);
                        CheckMyPromoCode(foreachPlayer);
                        BattlePass.Repository.UpdateReward(foreachPlayer, 60);
                        BattlePass.Repository.UpdateReward(foreachPlayer, 158);
                        //Trigger.ClientEvent(foreachPlayer, "phone.notify", 4386, $"Пришёл PayDay!", 5);

                        World.PayDayBonus.Repository.AddBonus(foreachPlayer);
                        
                        foreachCharacterData.LastHourMin = 0;
                        //Chars.Repository.PlayerStats(player);
                    }
                    catch (Exception e)
                    {
                        Log.Write($"payDayTrigger Foreach #1 Exception: {e.ToString()}");
                    }
                }
                
                //
                
                foreach (KeyValuePair<int, int> familyZoneSalary in familyZonesSalary)
                {
                    var foreachOrganizationData = Organizations.Manager.GetOrganizationData(familyZoneSalary.Key);
                    if (foreachOrganizationData == null)
                        continue;
                    
                    if (!familyZonesPlayers.ContainsKey(foreachOrganizationData.Id))
                        continue;

                    var lastMulti = foreachOrganizationData.MoneyMultiplier();
                    var amount = Convert.ToInt32((foreachOrganizationData.Salary / 100f) * familyZoneSalary.Value * familyZonesPlayers[foreachOrganizationData.Id]);
                    foreachOrganizationData.Money += amount;
                    
                    var newMulti = foreachOrganizationData.MoneyMultiplier();
                    if (lastMulti != newMulti) 
                        Organizations.Manager.UpdateForMembers(foreachOrganizationData.Id, OrganizationOfficeTypeUpdate.Money, newMulti);
                            
                    GameLog.Money($"server", $"org({foreachOrganizationData.Id})", amount, $"putStock");
                }

                //
                
                World.PayDayBonus.Repository.Bonus();

                var businesses = new List<Business>();
                foreach (Business biz in BusinessManager.BizList.Values)
                {
                    try
                    {
                        /*if (biz.Owner == "Государство")
                        {
                            foreach (Product p in biz.Products)
                            {
                                if (p.Ordered) continue;
                                if (p.Lefts < Convert.ToInt32(BusinessManager.BusProductsData[p.Name].MaxCount * 0.1))
                                {
                                    int amount = Convert.ToInt32(BusinessManager.BusProductsData[p.Name].MaxCount * 0.1);

                                    Order order = new Order(p.Name, amount);
                                    p.Ordered = true;

                                    Random random = new Random();
                                    do
                                    {
                                        order.UID = random.Next(000000, 999999);
                                    } while (BusinessManager.Orders.ContainsKey(order.UID));
                                    BusinessManager.Orders.Add(order.UID, biz.ID);

                                    biz.Orders.Add(order);
                                    Log.Debug($"New Order('{order.Name}',amount={order.Amount},UID={order.UID}) by Biz {biz.ID}");
                                    continue;
                                }
                            }
                            continue;
                        }*/
                        if (!ServerSettings.IsBusinessTax) break;
                        if (!biz.IsOwner()) continue;
                        if (biz.Mafia != -1)
                        {
                            var mafiaFractionData = Fractions.Manager.GetFractionData(biz.Mafia);
                            if (mafiaFractionData != null)
                                mafiaFractionData.Money += MafiaForBiz;
                        }

                        int tax = Convert.ToInt32(biz.SellPrice / 100f * biz.Tax);
                        biz.Zatratq += tax;
                        var bizBalance = MoneySystem.Bank.Accounts[biz.BankID];
                        bizBalance.Balance -= tax;
                        
                        var fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.CITY);
                        if (fractionData != null)
                            fractionData.Money += tax;
                        
                        Cityhall.lastHourTax += tax;

                        GameLog.Money($"biz({biz.ID})", "frac(6)", tax, "bizTaxHour");

                        if (bizBalance.Balance >= 0) continue;

                        string owner = biz.Owner;
                        if (PlayerNames.Values.Contains(owner) && PlayerUUIDs.ContainsKey(owner))
                        {
                            ExtPlayer player = (ExtPlayer) NAPI.Player.GetPlayerFromName(owner);
                            var characterData = player.GetCharacterData();
                            if (characterData != null)
                            {
                                Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BizSlet), 3000);
                                MoneySystem.Wallet.Change(player, Convert.ToInt32(biz.SellPrice * 0.8));
                                characterData.BizIDs.Remove(biz.ID);
                                //Chars.Repository.PlayerStats(player);
                            }
                            else
                            {
                                int targetUuid = PlayerUUIDs[owner];

                                Trigger.SetTask(async () =>
                                {
                                    try
                                    {
                                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                                        var character = await db.Characters 
                                            .Select(c => new {c.Uuid, c.Biz, c.Money}) 
                                            .Where(c => c.Uuid == targetUuid) 
                                            .FirstOrDefaultAsync(); 
     
                                        if (character != null)
                                        {
                                            var ownerBizs = JsonConvert.DeserializeObject<List<int>>(character.Biz);
                                            
                                            if (ownerBizs != null && ownerBizs.Contains(biz.ID)) 
                                                ownerBizs.Remove(biz.ID);
                                            
                                            await db.Characters 
                                                .Where(c => c.Uuid == character.Uuid) 
                                                .Set(c => c.Money, character.Money + Convert.ToInt32(biz.SellPrice * 0.8))
                                                .Set(c => c.Biz, JsonConvert.SerializeObject(ownerBizs)) 
                                                .UpdateAsync(); 
                                        } 
                                    }
                                    catch (Exception e)
                                    {
                                        Debugs.Repository.Exception(e);
                                    }
                                });
                            }
                            GameLog.Money($"server", $"player({PlayerUUIDs[biz.Owner]})", Convert.ToInt32(biz.SellPrice * 0.8), $"bizTax({biz.ID})");
                        }

                        bizBalance.Balance = 0;
                        bizBalance.IsSave = true;
                        
                        biz.ClearOwner();
                        
                        businesses.Add(biz);
                    }
                    catch (Exception e)
                    {
                        Log.Write($"payDayTrigger Foreach #2 Exception: {e.ToString()}");
                    }
                }
                
                var houses = new List<House>();
                foreach (var house in HouseManager.Houses)
                {
                    try
                    {
                        if (!ServerSettings.IsHouseTax) break;
                        if (house.Owner == string.Empty) continue;

                        int tax = Convert.ToInt32(house.Price / 100f * HouseManager.HouseTax);
                        if (!MoneySystem.Bank.Accounts.TryGetValue(house.BankID, out var houseBalance))
                            continue;
                        houseBalance.Balance -= tax;
                        
                        var fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.CITY);
                        if (fractionData != null)
                            fractionData.Money += tax;
                        
                        Cityhall.lastHourTax += tax;
                        if (house.Type != 7) GameLog.Money($"house({house.ID})", "frac(6)", tax, "houseTaxHour");
                        else GameLog.Money($"park({house.ID})", "frac(6)", tax, "parkTaxHour");

                        if (houseBalance.Balance >= 0) continue;

                        string owner = house.Owner;
                        var player = (ExtPlayer) NAPI.Player.GetPlayerFromName(owner);

                        int price = 0;

                        var accountData = player.GetAccountData();
                        var characterData = player.GetCharacterData();
                        if (accountData != null && characterData != null)
                        {
                            switch (accountData.VipLvl)
                            {
                                case 0: // None
                                    price = Convert.ToInt32(house.Price * 0.6);
                                    break;
                                case 1: // Bronze
                                    price = Convert.ToInt32(house.Price * 0.65);
                                    break;
                                case 2: // Silver
                                    price = Convert.ToInt32(house.Price * 0.7);
                                    break;
                                case 3: // Gold
                                    price = Convert.ToInt32(house.Price * 0.75);
                                    break;
                                case 4: // Platinum
                                case 5: // Media Platinum
                                    price = Convert.ToInt32(house.Price * 0.8);
                                    break;
                                default:
                                    // Not supposed to end up here. 
                                    break;
                            }
                            Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NalogPokaPoka), 3000);
                            MoneySystem.Wallet.Change(player, price);
                        }
                        else
                        {
                            if (PlayerUUIDs.ContainsKey(owner))
                            {
                                int targetUuid = PlayerUUIDs[owner];
                                price = Convert.ToInt32(house.Price * 0.6);

                                Character.Save.Repository.AddMoney(targetUuid, price);
                            }
                        }
                        houseBalance.Balance = 0;
                        houseBalance.IsSave = true;
                        
                        house.ClearOwner(isSave: false);

                        if (PlayerUUIDs.ContainsKey(owner))
                        {
                            if (house.Type != 7)
                                GameLog.Money($"server", $"player({PlayerUUIDs[owner]})", price, $"houseTax({house.ID})");
                            else
                                GameLog.Money($"server", $"player({PlayerUUIDs[owner]})", price, $"parkTax({house.ID})");
                        }

                        houses.Add(house);
                    }
                    catch (Exception e)
                    {
                        Log.Write($"payDayTrigger Foreach #3 Exception: {e.ToString()}");
                    }
                }
                if (DateTime.Now.Hour == 0)
                {
                    var fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.CITY);
                    if (fractionData != null)
                        fractionData.FuelLeft = fractionData.FuelLimit; // city
                    
                    fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.POLICE);
                    if (fractionData != null)
                        fractionData.FuelLeft = fractionData.FuelLimit; // police
                    
                    fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.SHERIFF);
                    if (fractionData != null)
                        fractionData.FuelLeft = fractionData.FuelLimit; // SHERIFF
                    
                    fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.EMS);
                    if (fractionData != null)
                        fractionData.FuelLeft = fractionData.FuelLimit; // fib
                    
                    fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.FIB);
                    if (fractionData != null)
                        fractionData.FuelLeft = fractionData.FuelLimit; // ems
                    
                    fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.ARMY);
                    if (fractionData != null)
                        fractionData.FuelLeft = fractionData.FuelLimit; // army
                    
                    fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.LSNEWS);
                    if (fractionData != null)
                        fractionData.FuelLeft = fractionData.FuelLimit; // news
                }

                foreach (GangsCapture.GangPoint point in GangsCapture.GangPoints.Values)
                {
                    var fractionData = Manager.GetFractionData(point.GangOwner);
                    if (fractionData != null)
                        fractionData.Money += GangForPoint;
                }

                Houses.Rieltagency.Repository.OnPayDay(houses, businesses);
                
                Organizations.Manager.NewDay();
                Log.Write("Payday time ended!");
                GetStats();
            }
            catch (Exception e)
            {
                Log.Write($"payDayTrigger Exception: {e.ToString()}");
            }
        }

        [Command("autorestart")]
        public void CMD_AutoRestart(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.AdminLVL < 6) return;
                AutoRestart = !AutoRestart;
                if (!AutoRestart) Notify.Send(player, NotifyType.Success, NotifyPosition.Center, LangFunc.GetText(LangType.Ru, DataName.AutoRRoff), 1500);
                else Notify.Send(player, NotifyType.Success, NotifyPosition.Center, LangFunc.GetText(LangType.Ru, DataName.AutoRRon), 1500);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_AutoRestart Exception: {e.ToString()}");
            }
        }

        #region SPECIAL
        [Command("build")]
        public void CMD_BUILD(ExtPlayer player)
        {
            try
            {
                if (player == null) return;
                Trigger.SendChatMessage(player, $"Сборка !{{#f39c12}}{Full}!{{#FFF}} запущена !{{#f39c12}}{StartDate}");
                Trigger.SendChatMessage(player, $"Дата компиляции: !{{#f39c12}}{CompileDate}");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_BUILD Exception: {e.ToString()}");
            }
        }
        /*public static void DeletePhone(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                var characterData = player.GetCharacterData();
                
                if (characterData.Sim != -1 && SimCards.ContainsKey(characterData.Sim)) 
                    SimCards.TryRemove(characterData.Sim, out _);
                
                characterData.Sim = -1;
            }
            catch (Exception e)
            {
                Log.Write($"AddPhoneHistory Exception: {e.ToString()}");
            }
        }
        public static void AddPhone(ExtPlayer player, int NewPhoneNumber)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                else if (SimCards.ContainsKey(NewPhoneNumber)) return;
                DeletePhone(player);
                var characterData = player.GetCharacterData();
                characterData.Sim = NewPhoneNumber;
                SimCards.TryAdd(NewPhoneNumber, characterData.UUID);
            }
            catch (Exception e)
            {
                Log.Write($"AddPhone Exception: {e.ToString()}");
            }
        }

        public static int GenerateSimcard(int minValue = 1000000, int maxValue = 9999999)
        {
            try
            {
                int result = rnd.Next(minValue, maxValue);
                while (SimCards.ContainsKey(result)) result = rnd.Next(minValue, maxValue);
                return result;
            }
            catch (Exception e)
            {
                Log.Write($"GenerateSimcard Exception: {e.ToString()}");
                return -1;
            }
        }*/
        public static string StringToU16(string utf8String)
        {
            return utf8String;
        }
        public static List<ExtPlayer> GetPlayersInRadiusOfPosition(Vector3 position, float radius, uint dimension = 39999999)
        {
            try
            {
                //if (Thread.CurrentThread.Name != "Main") return new List<Player>(); // Защита от левого потока, т.к. я не могу вернуть значение из под NAPI.Task.Run.
                var players = NAPI.Player.GetPlayersInRadiusOfPosition(radius, position).Cast<ExtPlayer>().ToList();
                players.RemoveAll(P => !P.GetLoginIn());
                players.RemoveAll(P => UpdateData.GetPlayerDimension(P) != dimension && dimension != 39999999);
                return players;
            }
            catch (Exception e)
            {
                Log.Write($"GetPlayersInRadiusOfPosition Exception: {e.ToString()}");
                return new List<ExtPlayer>();
            }
        }
        public static ExtPlayer GetNearestPlayer(ExtPlayer player, int radius)
        {
            try
            {
                if (!player.IsCharacterData()) return null;
                var players = NAPI.Player.GetPlayersInRadiusOfPosition(radius, player.Position).Cast<ExtPlayer>();
                ExtPlayer nearestPlayer = null;
                foreach (var foreachPlayer in players)
                {
                    if (!foreachPlayer.IsCharacterData()) continue;
                    if (foreachPlayer == player) continue;
                    if (UpdateData.GetPlayerDimension(foreachPlayer) != UpdateData.GetPlayerDimension(player)) continue;
                    if (nearestPlayer == null)
                    {
                        nearestPlayer = foreachPlayer;
                        continue;
                    }
                    if (player.Position.DistanceTo(foreachPlayer.Position) < player.Position.DistanceTo(nearestPlayer.Position)) nearestPlayer = foreachPlayer;
                }
                return nearestPlayer;
            }
            catch (Exception e)
            {
                Log.Write($"GetNearestPlayer Exception: {e.ToString()}");
                return null;
            }
        }
        public static ExtPlayer GetPlayerByID(int id)
        {
            if (PlayerIdToEntity.ContainsKey(id))
            {
                ExtPlayer target = PlayerIdToEntity[id];
                if (target.IsCharacterData())
                    return target;
            }
            return null;
        }

        public static ExtPlayer GetPlayerByIDv2(int id)
        {
            if (PlayerIdToEntity.ContainsKey(id))
                return PlayerIdToEntity[id];
            return null;
        }
        public static ExtPlayer GetPlayerByUUID(int UUID)
        {
            if (PlayerUUIDToPlayerId.ContainsKey(UUID))
            {
                return GetPlayerByID(PlayerUUIDToPlayerId[UUID]);
            }
            return null;
        }
        public static void PlayerEnterInterior(ExtPlayer player, Vector3 pos)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (sessionData.Follower != null)
                {
                    ExtPlayer target = sessionData.Follower;
                    if (!target.IsCharacterData()) return;
                    target.Position = pos;
                    Trigger.PlayAnimation(target, "mp_arresting", "idle", 49);
                    // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "cuff");
                    Attachments.AddAttachment(target, Attachments.AttachmentsName.Cuffs);
                    Trigger.ClientEvent(target, "setFollow", true, player);
                }
            }
            catch (Exception e)
            {
                Log.Write($"PlayerEnterInterior Exception: {e.ToString()}");
            }
        }
        public static void OnAntiAnim(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                sessionData.AntiAnimDown = true;
            }
            catch (Exception e)
            {
                Log.Write($"OnAntiAnim Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("OffAnim")]
        public static void OffAntiAnim(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;

                sessionData.AntiAnimDown = false;
                VoiceData playerPhoneMeta = sessionData.VoiceData;
                //TEST на безопасность потоков, по идее всё должно быть норм
                if (playerPhoneMeta.CallingState != "callMe" && playerPhoneMeta.Target != null)
                {
                    Trigger.PlayAnimation(player, "anim@cellphone@in_car@ds", "cellphone_call_listen_base", 49);
                    // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "cuff");
                    Attachments.AddAttachment(player, Attachments.AttachmentsName.PhoneCall);
                }
            }
            catch (Exception e)
            {
                Log.Write($"OffAntiAnim Exception: {e.ToString()}");
            }
        }
        
        #region MainMenu
        
  
        [RemoteEvent("server.mayormenu.open")]
        public static void OpenMayorMenu(ExtPlayer player)
        {
            try
            {
                if (!player.IsFractionLeader((int) Fractions.Models.Fractions.CITY)) return;

                var fractionData = player.GetFractionData();
                if (fractionData == null)
                    return;

                
                var frameList = new FrameListData(); 
                frameList.Header = LangFunc.GetText(LangType.Ru, DataName.Kazna);
                frameList.Callback = callback_mayormenu;

                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.MoneyKazna, fractionData.Money), "info")); 
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.KaznaSobrano, Fractions.Cityhall.lastHourTax), "info2")); 
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.TakeMoneyy), "take")); 
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.PutMoney), "put")); 
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.Control), "header2")); 
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.GosZapravka), "fuelcontrol"));
                
                Players.Popup.List.Repository.Open(player, frameList);   
            }
            catch (Exception e)
            {
                Log.Write($"OpenMayorMenu Exception: {e.ToString()}");
            }
        }
        private static void callback_mayormenu(ExtPlayer player, object listItem) /// Никитос Чини 
        {
            try
            {	
                if (!(listItem is string))
                    return;
                
                if (!player.IsCharacterData()) return;

                switch (listItem)
                {
                    case "take":
                        Trigger.ClientEvent(player, "openInput", "Получить деньги из казны", "Количество", 6, "mayor_take");
                        return;
                    case "put":
                        Trigger.ClientEvent(player, "openInput", "Положить деньги в казну", "Количество", 6, "mayor_put");
                        return;
                    case "fuelcontrol":
                        OpenFuelcontrolMenu(player);
                        return;
                    /*default:
                        OpenMayorMenu(player);
                        break;*/
                }
            }
            catch (Exception e)
            {
                Log.Write($"callback_mayormenu Exception: {e.ToString()}");
            }
        }
        public static void OpenFuelcontrolMenu(ExtPlayer player)
        {
            try
            {
                if (!player.IsFractionLeader((int) Fractions.Models.Fractions.CITY)) return;

                var frameList = new FrameListData(); 
                frameList.Header = LangFunc.GetText(LangType.Ru, DataName.GosZapravka); 
                frameList.Callback = callback_fuelcontrol; 
                
                var fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.CITY);
                if (fractionData != null)
                    frameList.List.Add(new ListData($"Мэрия. Осталось сегодня: {fractionData.FuelLeft}/{fractionData.FuelLimit}$", "info_city"));

                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.SetLimit), "set_city"));
                
                fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.POLICE);
                if (fractionData != null)
                    frameList.List.Add(new ListData($"Полиция. Осталось сегодня: {fractionData.FuelLeft}/{fractionData.FuelLimit}$", "info_police"));

                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.SetLimit), "set_police"));
                
                fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.EMS);
                if (fractionData != null)
                    frameList.List.Add(new ListData($"EMS. Осталось сегодня: {fractionData.FuelLeft}/{fractionData.FuelLimit}$", "info_ems"));

                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.SetLimit), "set_ems"));
                
                fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.FIB);
                if (fractionData != null)
                    frameList.List.Add(new ListData($"FIB. Осталось сегодня: {fractionData.FuelLeft}/{fractionData.FuelLimit}$", "info_fib"));

                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.SetLimit), "set_fib"));

                fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.ARMY);
                if (fractionData != null)
                    frameList.List.Add(new ListData($"Армия. Осталось сегодня: {fractionData.FuelLeft}/{fractionData.FuelLimit}$", "info_army"));
                
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.SetLimit), "set_army"));
                
                fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.LSNEWS);
                if (fractionData != null)
                    frameList.List.Add(new ListData($"News. Осталось сегодня: {fractionData.FuelLeft}/{fractionData.FuelLimit}$", "info_news"));
                
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.SetLimit), "set_news"));
                
                Players.Popup.List.Repository.Open(player, frameList);  
            }
            catch (Exception e)
            {
                Log.Write($"OpenFuelcontrolMenu Exception: {e.ToString()}");
            }
        }
        private static void callback_fuelcontrol(ExtPlayer player, object listItem) /// Никитос Чини 
        {
            try
            {
                if (!(listItem is string))
                    return;
                
                if (!player.IsCharacterData()) return;
                
                switch (listItem)
                {
                    case "set_city":
                        Trigger.ClientEvent(player, "openInput", LangFunc.GetText(LangType.Ru, DataName.SetLimit), LangFunc.GetText(LangType.Ru, DataName.FuelLimitM), 5, "fuelcontrol_city");
                        return;
                    case "set_police":
                        Trigger.ClientEvent(player, "openInput", LangFunc.GetText(LangType.Ru, DataName.SetLimit), LangFunc.GetText(LangType.Ru, DataName.FuelLimitP), 5, "fuelcontrol_police");
                        return;
                    case "set_ems":
                        Trigger.ClientEvent(player, "openInput", LangFunc.GetText(LangType.Ru, DataName.SetLimit), LangFunc.GetText(LangType.Ru, DataName.FuelLimitE), 5, "fuelcontrol_ems");
                        return;
                    case "set_fib":
                        Trigger.ClientEvent(player, "openInput", LangFunc.GetText(LangType.Ru, DataName.SetLimit), LangFunc.GetText(LangType.Ru, DataName.FuelLimitF), 5, "fuelcontrol_fib");
                        return;
                    case "set_army":
                        Trigger.ClientEvent(player, "openInput", LangFunc.GetText(LangType.Ru, DataName.SetLimit), LangFunc.GetText(LangType.Ru, DataName.FuelLimitA), 5, "fuelcontrol_army");
                        return;
                    case "set_news":
                        Trigger.ClientEvent(player, "openInput", LangFunc.GetText(LangType.Ru, DataName.SetLimit), LangFunc.GetText(LangType.Ru, DataName.FuelLimitN), 5, "fuelcontrol_news");
                        return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"callback_fuelcontrol Exception: {e.ToString()}");
            }
        }
        #endregion
        #endregion

        public static ConcurrentDictionary<string, int> StatsClientToServer = new ConcurrentDictionary<string, int>();
        public static bool IsDebugEvents = false;
        [RemoteEvent("event_stats")]
        public void event_stats(ExtPlayer player, string name)
        {
            try
            {
                if (!IsDebugEvents) return;
                if (!StatsClientToServer.ContainsKey(name)) StatsClientToServer.TryAdd(name, 1);
                else StatsClientToServer[name]++;
            }
            catch (Exception e)
            {
                Log.Write($"event_stats Exception: {e.ToString()}");
            }
        }
        public static ConcurrentDictionary<string, int> StatsServerToClient = new ConcurrentDictionary<string, int>();
        public static void GetMemory(ExtPlayer player, string name)
        {
            try
            {
                if (!IsDebugEvents) return;
                if (!StatsServerToClient.ContainsKey(name)) StatsServerToClient.TryAdd(name, 1);
                else StatsServerToClient[name]++;
                /*if (!FunctionsAccess.IsWorking("GetMemory")) return;
                var proc = Process.GetCurrentProcess();
                var ram = proc.WorkingSet64 / 1_048_576;
                string playerName = "-";
                if (player.IsCharacterData()) playerName = $"{sessionData.Name}";
                GameLog.AddRam(playerName, name, ram);*/
            }
            catch (Exception e)
            {
                Log.Write($"GetMemory Exception: {e.ToString()}");
            }
        }
        [Command("getstatsserverdata")]
        public void getstatsserverdata(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            if (characterData.AdminLVL < 3) return;
            GetStats();
        }
        public static void GetStats()
        {
            try
            {
                if (!IsDebugEvents) return;
                using (StreamWriter save = new StreamWriter("StatsClientToServer.txt", true, Encoding.UTF8))
                {
                    save.Write($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}:\n\n");
                    foreach (var stats in StatsClientToServer)
                    {

                        save.Write($"{stats.Key}: {stats.Value} Count\n");
                    }
                    save.Write($"-----------------------------------------------");
                    save.Close();
                    StatsClientToServer.Clear();
                }
                using (StreamWriter save = new StreamWriter("StatsServerToClient.txt", true, Encoding.UTF8))
                {
                    save.Write($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}:\n\n");
                    foreach (var stats in StatsServerToClient)
                    {

                        save.Write($"{stats.Key}: {stats.Value} Count\n");
                    }
                    save.Write($"-----------------------------------------------");
                    save.Close();
                    StatsServerToClient.Clear();
                }
            }
            catch (Exception e)
            {
                Log.Write($"GetMemory Exception: {e.ToString()}");
            }
        }
    }
    public class Trigger : Script
    {
        
        
        public static void SendChatMessage(ExtPlayer player, string message)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (Thread.CurrentThread.Name == "Main")
                {
                    if (player == null) return;
                    else if (sessionData != null && !sessionData.IsConnect) return;
                    player.SendChatMessage(message);
                    return;
                }
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        if (player == null) return;
                        else if (sessionData != null && !sessionData.IsConnect) return;
                        player.SendChatMessage(message);
                    }
                    catch (Exception e)
                    {
                        Main.Log.Write($"SendChatMessage Task Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Main.Log.Write($"SendChatMessage Exception: {e.ToString()}");
            }
        }
        public static void Kick(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (Thread.CurrentThread.Name == "Main")
                {
                    if (player == null) return;
                    else if (sessionData != null && !sessionData.IsConnect) return;
                    player.Kick();
                    return;
                }
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        if (player == null) return;
                        else if (sessionData != null && !sessionData.IsConnect) return;
                        player.Kick();
                    }
                    catch (Exception e)
                    {
                        Main.Log.Write($"Kick Task Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Main.Log.Write($"Kick Exception: {e.ToString()}");
            }
        }
        public static void SetSharedData(Entity entity, string key, object value)
        {
            try
            {
                if (Thread.CurrentThread.Name == "Main")
                {
                    if (entity == null) return;
                    entity.SetSharedData(key, value);
                    return;
                }
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        if (entity == null) return;
                        entity.SetSharedData(key, value);
                    }
                    catch (Exception e)
                    {
                        Main.Log.Write($"SetSharedData Task Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Main.Log.Write($"SetSharedData Exception: {e.ToString()}");
            }
        }
        public static void PlayAnimation(ExtPlayer player, string animDict, string animName, int flag, bool share = true, string attachmentName = null)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (Thread.CurrentThread.Name == "Main")
                {
                    if (player == null) return;
                    else if (sessionData != null && !sessionData.IsConnect) return;
                    if (share) player.SetSharedData("ANIM_USE", $"{animDict}|{animName}|{flag}|{attachmentName}");
                    else player.PlayAnimation(animDict, animName, flag);
                    return;
                }
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        if (player == null) return;
                        else if (sessionData != null && !sessionData.IsConnect) return;
                        if (share) player.SetSharedData("ANIM_USE", $"{animDict}|{animName}|{flag}|{attachmentName}");
                        else player.PlayAnimation(animDict, animName, flag);
                    }
                    catch (Exception e)
                    {
                        Main.Log.Write($"PlayAnimation Task Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Main.Log.Write($"PlayAnimation Exception: {e.ToString()}");
            }
        }

        public static void TaskPlayAnim(ExtPlayer player, string animDict, string animName, int flag = 3, bool clearTasks = false, string attachmentName = null)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData != null && !sessionData.IsConnect) return;
                Main.OnAntiAnim(player);
                ClientEventInRange(player.Position, 250f, "taskPlayAnim", player, animDict, animName, flag, clearTasks, attachmentName);
            }
            catch (Exception e)
            {
                Main.Log.Write($"PlayAllAnimation Exception: {e.ToString()}");
            }
        }
        public static void StopAnimation(ExtPlayer player, bool force = true)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (Thread.CurrentThread.Name == "Main")
                {
                    if (player == null) return;
                    else if (sessionData != null && !sessionData.IsConnect) return;
                    player.SetSharedData("ANIM_USE", "null");
                    player.SetSharedData("AnimToKey", 0);
                    if (force) player.StopAnimation();
                    return;
                }
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        if (player == null) return;
                        else if (sessionData != null && !sessionData.IsConnect) return;
                        player.SetSharedData("ANIM_USE", "null");
                        player.SetSharedData("AnimToKey", 0);
                        if (force) player.StopAnimation();
                    }
                    catch (Exception e)
                    {
                        Main.Log.Write($"StopAnimation Task Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Main.Log.Write($"StopAnimation Exception: {e.ToString()}");
            }
        }

        public static void Position(ExtPlayer player, Vector3 pos)
        {
            try
            {
                if (Thread.CurrentThread.Name == "Main")
                {
                    if (player == null) return;
                    player.Position = pos;
                    return;
                }
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        if (player == null) return;
                        player.Position = pos;
                    }
                    catch (Exception e)
                    {
                        Main.Log.Write($"Player Position Task Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Main.Log.Write($"Player Position Exception: {e.ToString()}");
            }
        }
        public static void Position(ExtVehicle vehicle, Vector3 pos)
        {
            try
            {
                if (Thread.CurrentThread.Name == "Main")
                {
                    if (vehicle == null) return;
                    vehicle.Position = pos;
                    return;
                }
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        if (vehicle == null) return;
                        vehicle.Position = pos;
                    }
                    catch (Exception e)
                    {
                        Main.Log.Write($"Vehicle Position Task Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Main.Log.Write($"Vehicle Position Exception: {e.ToString()}");
            }
        }
        public static void SendToAdmins(int minLVL, string message, byte soundnotify = 0)
        {
            try
            {
                if (Thread.CurrentThread.Name == "Main")
                {
                    foreach (ExtPlayer foreachPlayer in Main.AllAdminsOnline)
                    {
                        var foreachCharacterData = foreachPlayer.GetCharacterData();
                        if (foreachCharacterData == null) continue;
                        if (foreachCharacterData.AdminLVL >= minLVL) foreachPlayer.SendChatMessage(message);
                    }
                    return;
                }
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        foreach (ExtPlayer foreachPlayer in Main.AllAdminsOnline)
                        {
                            var foreachCharacterData = foreachPlayer.GetCharacterData();
                            if (foreachCharacterData == null) continue;
                            if (foreachCharacterData.AdminLVL >= minLVL) foreachPlayer.SendChatMessage(message);
                        }
                    }
                    catch (Exception e)
                    {
                        Main.Log.Write($"SendToAdmins Task Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Main.Log.Write($"SendToAdmins Exception: {e.ToString()}");
            }
        }

        public static void SendPunishment(string message, ExtPlayer target = null)
        {
            try
            {
                if (Thread.CurrentThread.Name == "Main")
                {
                    if (target != null) target.SendChatMessage(message);
                    foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                    {
                        var foreachSessionData = foreachPlayer.GetSessionData();
                        if (foreachSessionData == null) continue;
                        var foreachCharacterData = foreachPlayer.GetCharacterData();
                        if (foreachCharacterData == null) continue;
                        if (target == foreachPlayer) continue;
                        if (foreachCharacterData.AdminLVL >= 1 || foreachSessionData.Punishments) foreachPlayer.SendChatMessage(message);
                    }
                    return;
                }
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        if (target != null) target.SendChatMessage(message);
                        foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                        {
                            var foreachSessionData = foreachPlayer.GetSessionData();
                            if (foreachSessionData == null) continue;
                            var foreachCharacterData = foreachPlayer.GetCharacterData();
                            if (foreachCharacterData == null) continue;
                            if (target == foreachPlayer) continue;
                            if (foreachCharacterData.AdminLVL >= 1 || foreachSessionData.Punishments) foreachPlayer.SendChatMessage(message);
                        }
                    }
                    catch (Exception e)
                    {
                        Main.Log.Write($"SendPunishment Task Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Main.Log.Write($"SendPunishment Exception: {e.ToString()}");
            }
        }

        public static void ClientEvent(ExtPlayer player, string eventName, params object[] args)
        {
            try
            {
                var sessionData = player.GetSessionData();
                Main.GetMemory(player, eventName);
                if (Thread.CurrentThread.Name == "Main")
                {
                    if (player == null) return;
                    else if (sessionData != null && !sessionData.IsConnect) return;
                    NAPI.ClientEvent.TriggerClientEvent(player, eventName, args);
                    return;
                }
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        if (player == null) return;
                        else if (sessionData != null && !sessionData.IsConnect) return;
                        NAPI.ClientEvent.TriggerClientEvent(player, eventName, args);
                    }
                    catch (Exception e)
                    {
                        Main.Log.Write($"ClientEvent Task Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Main.Log.Write($"ClientEvent({eventName}) Exception: {e.ToString()}");
            }
        }
        
        
        public static void UniqueDimension(Entity entity)
        {
            try
            {
                if (Thread.CurrentThread.Name == "Main")
                {
                    if (entity == null) return;
                    uint newDimension = (uint)(1500 + entity.Id);
                    NAPI.Entity.SetEntityDimension(entity, newDimension);

                    if (entity.Type == EntityType.Player)
                    {
                        ExtPlayer playerEntity = (ExtPlayer)entity;
                        SessionData sessionData = playerEntity.GetSessionData();
                        if (sessionData != null) sessionData.Dimension = newDimension;
                    }
                    return;
                }
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        if (entity == null) return;
                        uint newDimension = (uint)(1500 + entity.Id);
                        NAPI.Entity.SetEntityDimension(entity, newDimension);

                        if (entity.Type == EntityType.Player)
                        {
                            ExtPlayer playerEntity = (ExtPlayer)entity;
                            SessionData sessionData = playerEntity.GetSessionData();
                            if (sessionData != null) sessionData.Dimension = newDimension;
                        }
                    }
                    catch (Exception e)
                    {
                        Main.Log.Write($"Player Dimension Task Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Main.Log.Write($"Player Dimension Exception: {e.ToString()}");
            }
        }
        public static void Dimension(Entity entity, uint Dimension = 0)
        {
            try
            {
                if (Thread.CurrentThread.Name == "Main")
                {
                    if (entity == null) return;
                    NAPI.Entity.SetEntityDimension(entity, Dimension);
                        
                    if (entity.Type == EntityType.Player)
                    {
                        ExtPlayer playerEntity = (ExtPlayer)entity;
                        PedSystem.Pet.Repository.OnUpdateDim(playerEntity, Dimension);
                        SessionData sessionData = playerEntity.GetSessionData();
                        if (sessionData != null) sessionData.Dimension = Dimension;
                    }
                    return;
                }
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        if (entity == null) return;
                        NAPI.Entity.SetEntityDimension(entity, Dimension);
                        
                        if (entity.Type == EntityType.Player)
                        {
                            ExtPlayer playerEntity = (ExtPlayer)entity;
                            PedSystem.Pet.Repository.OnUpdateDim(playerEntity, Dimension);
                            SessionData sessionData = playerEntity.GetSessionData();
                            if (sessionData != null) sessionData.Dimension = Dimension;
                        }
                    }
                    catch (Exception e)
                    {
                        Main.Log.Write($"Player Dimension Task Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Main.Log.Write($"Player Dimension Exception: {e.ToString()}");
            }
        }
        public static void DamageDisable(ExtPlayer player, bool trigger)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (Thread.CurrentThread.Name == "Main")
                {
                    player.SetSharedData("DMGDisable", trigger);
                    ClientEventInRange(player.Position, 350f, "disabledmg", player, trigger);
                    sessionData.KillData.Count = 0;
                    sessionData.KillData.DamageDisabled = trigger;
                    return;
                }

                NAPI.Task.Run(() =>
                {
                    try
                    {
                        player.SetSharedData("DMGDisable", trigger);
                        ClientEventInRange(player.Position, 350f, "disabledmg", player, trigger);
                        sessionData.KillData.Count = 0;
                        sessionData.KillData.DamageDisabled = trigger;
                    }
                    catch (Exception e)
                    {
                        Main.Log.Write($"DamageDisable Task Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Main.Log.Write($"DamageDisable Exception: {e.ToString()}");
            }
        }
        public static void ClientEventInRange(Vector3 pos, float range, string eventName, params object[] args)
        {
            try
            {
                Main.GetMemory(null, eventName);
                if (Thread.CurrentThread.Name == "Main")
                {
                    NAPI.ClientEvent.TriggerClientEventInRange(pos, range, eventName, args);
                    return;
                }

                NAPI.Task.Run(() =>
                {
                    try
                    {
                        NAPI.ClientEvent.TriggerClientEventInRange(pos, range, eventName, args);
                    }
                    catch (Exception e)
                    {
                        Main.Log.Write($"ClientEventInRange Task Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Main.Log.Write($"ClientEventInRange Exception: {e.ToString()}");
            }
        }
        public static void ClientEventForAll(string eventName, params object[] args)
        {
            try
            {
                Main.GetMemory(null, eventName);
                if (Thread.CurrentThread.Name == "Main")
                {
                    NAPI.ClientEvent.TriggerClientEventForAll(eventName, args);
                    return;
                }

                NAPI.Task.Run(() =>
                {
                    try
                    {
                        NAPI.ClientEvent.TriggerClientEventForAll(eventName, args);
                    }
                    catch (Exception e)
                    {
                        Main.Log.Write($"ClientEventForAll Task Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Main.Log.Write($"ClientEventForAll Exception: {e.ToString()}");
            }
        }

        public static void ClientEventInDimension(uint dim, string eventName, params object[] args)
        {
            try
            {
                Main.GetMemory(null, eventName);
                if (Thread.CurrentThread.Name == "Main")
                {
                    NAPI.ClientEvent.TriggerClientEventInDimension(dim, eventName, args);
                    return;
                }
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        NAPI.ClientEvent.TriggerClientEventInDimension(dim, eventName, args);
                    }
                    catch (Exception e)
                    {
                        Main.Log.Write($"ClientEventInDimension Task Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Main.Log.Write($"ClientEventInDimension Exception: {e.ToString()}");
            }
        }
        public static void ClientEventToPlayers(ExtPlayer[] players, string eventName, params object[] args)
        {
            try
            {
                Main.GetMemory(null, eventName);
                if (Thread.CurrentThread.Name == "Main")
                {
                    NAPI.ClientEvent.TriggerClientEventToPlayers(players, eventName, args);
                    return;
                }
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        NAPI.ClientEvent.TriggerClientEventToPlayers(players, eventName, args);
                    }
                    catch (Exception e)
                    {
                        Main.Log.Write($"ClientEventToPlayers Task Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Main.Log.Write($"ClientEventToPlayers Exception: {e.ToString()}");
            }
        }

        public static void SetMainTask(Action action)
        {
            
            if (Thread.CurrentThread.Name == "Main")
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    Timers.Log.Write($"SetMainTask Exception: {e.ToString()}");
                }
                return;
            }
            NAPI.Task.Run(() => 
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    Timers.Log.Write($"SetMainTask Task Exception: {e.ToString()}");
                }
            });
        }

        public static void SetTask(Action action)
        {
            
            if (Thread.CurrentThread.Name != "Main")
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    Timers.Log.Write($"SetTask Exception: {e.ToString()}");
                }
                return;
            }
            Task.Factory.StartNew(() => 
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    Timers.Log.Write($"SetTask Task Exception: {e.ToString()}");
                }
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public static void SetAsyncTask(Func<Task> action)
        {

            if (Thread.CurrentThread.Name != "Main")
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    Timers.Log.Write($"SetAsyncTask Exception: {e.ToString()}");
                }
                return;
            }
            Task.Factory.StartNew(() =>
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    Timers.Log.Write($"SetAsyncTask Task Exception: {e.ToString()}");
                }
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }
}
