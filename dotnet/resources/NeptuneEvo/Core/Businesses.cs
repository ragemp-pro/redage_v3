using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using MySqlConnector;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Functions;
using NeptuneEvo.GUI;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.Quests;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Localization;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Houses;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.Players.Phone.Auction.Models;
using NeptuneEvo.Players.Phone.Messages.Models;
using NeptuneEvo.Quests.Models;
using NeptuneEvo.Table.Models;
using NeptuneEvo.Table.Tasks.Models;
using NeptuneEvo.Table.Tasks.Player;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;
using NeptuneEvo.VehicleData.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NeptuneEvo.Core
{
    class BusinessManager : Script
    {
        public static readonly nLog Log = new nLog("Core.Businesses");
        private static int lastBizID = -1;


        private static LsCustoms[] LsCustomsPlaces = new LsCustoms[5]
        {
            new LsCustoms(new Vector3(-338.5271, -136.7135, 39.06718), new Vector3(0.07063885, -0.2866833, 285.2643)),
            new LsCustoms(new Vector3(731.7056, -1088.896, 22.2262), new Vector3(0.003280999, -0.3730852, 269.6414)),
            new LsCustoms(new Vector3(-1155.194, -2005.489, 13.24107), new Vector3(-0.1381256, 0.1076123, 165.1146)),
            new LsCustoms(new Vector3(-212.5238, -1323.189, 30.94811), new Vector3(-0.2359019, 0.2521333, 130.1107)),
            new LsCustoms(new Vector3(110.9264, 6626.196, 31.84457), new Vector3(0.2491093, 0.2717469, 46.90005)),
        };

        public static Dictionary<int, int> LsCustomsPlacesToBusnessId = new Dictionary<int, int>();
        public static Dictionary<int, int> LsCustomsBusnessToPlacesId = new Dictionary<int, int>();


        private static int GetLsCustomsPlaces(Vector3 pos)
        {
            int i = 0;
            int index = 0;
            float lastDist = LsCustomsPlaces[0].Position.DistanceTo(pos);
            foreach (LsCustoms p in LsCustomsPlaces)
            {
                if (p.Position.DistanceTo(pos) < lastDist && p.Position.DistanceTo(pos) < 50.0f)
                {
                    lastDist = p.Position.DistanceTo(pos);
                    index = i;
                }
                i++;
            }
            return index;
        }

        public enum BusProductToType
        {
            Elite = -3,
            Donate = -2,
            None = -1,
            Market,
            Gun,
            Burger
        }
        public class BusProductData
        {
            public int Price { get; set; } // int(11)
            public int OtherPrice { get; set; } // int(11)
            public int Percent { get; set; } // int(11)
            public int MaxCount { get; set; } // int(11)
            public BusProductToType Type { get; set; } // tinyint(2)
            public ItemId ItemId { get; set; }
            public bool Toggled { get; set; }

            public BusProductData(int Price, int OtherPrice, int Percent, int MaxCount, sbyte Type, int ItemId, bool Toggled)
            {
                this.Price = Price;
                this.OtherPrice = OtherPrice;
                this.Percent = Percent;
                this.MaxCount = MaxCount;
                this.Type = (BusProductToType)Type;
                this.ItemId = (ItemId)ItemId;
                this.Toggled = Toggled;
            }
        }
        public static Dictionary<string, BusProductData> BusProductsData = new Dictionary<string, BusProductData>();

        public static void InitBusProducts()
        {
            using (var db = new ConfigBD("ConfigDB"))
            {
                var busProductsData = new Dictionary<string, BusProductData>();
                var busProducts = db.BusProducts.ToList();
                foreach (var busProduct in busProducts)
                {
                    busProductsData.Add(busProduct.Name, new BusProductData(busProduct.Price, busProduct.OtherPrice, busProduct.Percent, busProduct.MaxCount, busProduct.Type, busProduct.ItemId, busProduct.Toggled));
                }
                BusProductsData = busProductsData;
            }
        }

        public static BusProductData GetBusProductData(string name)
        {
            name = name.ToLower();
            
            return BusProductsData.FirstOrDefault(pd => pd.Key.ToLower() == name).Value;
        }
        public static void UpdateBusProd(Business data)
        {
            try
            {
                if (data == null) return;
                bool changed = false;

                double minPrice;
                double maxPrice;

                foreach (Product prod in data.Products)
                {
                    if (!BusProductsData.ContainsKey(prod.Name)) continue;
                    if (data.Type == 0 && prod.Name == "Лотерейный билет")
                    {
                        if (prod.Price != 500)
                        {
                            prod.Price = 500; // Цена за штуку лотерейного билета.
                            changed = true;
                        }
                    }
                    else
                    {
                        minPrice = BusProductsData[prod.Name].Price * Main.BusinessMinPrice;
                        if (data.Type == 1) minPrice = Main.PricesSettings.ZapravkaMinPrice;
                        else if (data.Type == 7) minPrice = Main.PricesSettings.ClothesMinPrice;
                        else if (data.Type == 9 || data.Type == 10 || data.Type == 11 || data.Type == 12) minPrice = Main.PricesSettings.TattooBarberMasksLscMinPrice;
                        
                        maxPrice = BusProductsData[prod.Name].Price * Main.BusinessMaxPrice;
                        if (data.Type == 1) maxPrice = Main.PricesSettings.ZapravkaMaxPrice;
                        else if (data.Type == 7) maxPrice = Main.PricesSettings.ClothesMaxPrice;
                        else if (data.Type == 9 || data.Type == 10 || data.Type == 11 || data.Type == 12) maxPrice = Main.PricesSettings.TattooBarberMasksLscMaxPrice;
                        
                        if (prod.Price < minPrice || prod.Price > maxPrice)
                        {
                            if (prod.Price < minPrice) prod.Price = Convert.ToInt32(minPrice);
                            else if (prod.Price > maxPrice) prod.Price = Convert.ToInt32(maxPrice);
                            changed = true;
                        }
                    }
                }
                // UNCOMMENT
                switch (data.Type)
                {
                    case 0:
                        foreach (KeyValuePair<string, BusProductData> busProductData in BusProductsData)
                        {
                            var product = data.Products.FirstOrDefault(x => x.Name == busProductData.Key);
                            if (product == null)
                            {
                                if (busProductData.Value.Type != BusProductToType.Market) continue;
                                if (busProductData.Key == "Лотерейный билет") data.Products.Add(new Product(500, 0, 0, busProductData.Key, false));
                                else data.Products.Add(new Product(busProductData.Value.Price, 0, 1, busProductData.Key, false));
                                changed = true;
                            }
                            else if (data.Products.Contains(product))
                            {
                                if (busProductData.Value.Type == BusProductToType.Market) continue;
                                data.Products.Remove(product);
                                changed = true;
                            }
                        }

                        //Проверяем если на нахождение продукта в базе продуктов)
                        foreach (var product in data.Products.ToList())
                        {
                            if (product == null) continue;
                            else if (BusProductsData.ContainsKey (product.Name)) continue;
                            data.Products.Remove(product);
                            changed = true;
                        }
                        break;
                    case 6:
                        foreach (KeyValuePair<string, BusProductData> busProductData in BusProductsData)
                        {
                            if (busProductData.Value.Type != BusProductToType.Gun) continue;
                            if (data.Products.FirstOrDefault(x => x.Name == busProductData.Key) == null)
                            {
                                data.Products.Add(new Product(busProductData.Value.Price, 0, 0, busProductData.Key, false));
                                changed = true;
                            }
                        }
                        break;
                    case 2:
                        foreach (string name in CarsNames[0])
                        {
                            if (!BusProductsData.ContainsKey(name)) continue;
                            if (BusProductsData[name].Price == 0) continue;
                            if (data.Products.FirstOrDefault(x => x.Name == name) == null)
                            {
                                data.Products.Add(new Product(BusProductsData[name].Price, 0, 0, name, false));
                                changed = true;
                            }
                        }
                        break;
                    case 3:
                        foreach (string name in CarsNames[1])
                        {
                            if (!BusProductsData.ContainsKey(name)) continue;
                            else if (BusProductsData[name].Price == 0) continue;
                            if (data.Products.FirstOrDefault(x => x.Name == name) == null)
                            {
                                data.Products.Add(new Product(BusProductsData[name].Price, 0, 0, name, false));
                                changed = true;
                            }
                        }
                        break;
                    case 4:
                        foreach (string name in CarsNames[2])
                        {
                            if (!BusProductsData.ContainsKey(name)) continue;
                            else if (BusProductsData[name].Price == 0) continue;
                            if (data.Products.FirstOrDefault(x => x.Name == name) == null)
                            {
                                data.Products.Add(new Product(BusProductsData[name].Price, 0, 0, name, false));
                                changed = true;
                            }
                        }
                        break;
                    case 5:
                        foreach (string name in CarsNames[3])
                        {
                            if (!BusProductsData.ContainsKey(name)) continue;
                            else if (BusProductsData[name].Price == 0) continue;
                            if (data.Products.FirstOrDefault(x => x.Name == name) == null)
                            {
                                data.Products.Add(new Product(BusProductsData[name].Price, 0, 0, name, false));
                                changed = true;
                            }
                        }
                        break;

                }
                if (data.Products.Count != 0)
                {
                    string nameprod = "";
                    for (byte i = 0; i < data.Products.Count; i++)
                    {
                        nameprod = data.Products[i].Name;
                        if (!BusProductsData.ContainsKey(nameprod)) continue;
                        
                        minPrice = BusProductsData[nameprod].Price * Main.BusinessMinPrice;
                        if (data.Type == 1) minPrice = Main.PricesSettings.ZapravkaMinPrice;
                        else if (data.Type == 7) minPrice = Main.PricesSettings.ClothesMinPrice;
                        else if (data.Type == 9 || data.Type == 10 || data.Type == 11 || data.Type == 12) minPrice = Main.PricesSettings.TattooBarberMasksLscMinPrice;
                        
                        maxPrice = BusProductsData[nameprod].Price * Main.BusinessMaxPrice;
                        if (data.Type == 1) maxPrice = Main.PricesSettings.ZapravkaMaxPrice;
                        else if (data.Type == 7) maxPrice = Main.PricesSettings.ClothesMaxPrice;
                        else if (data.Type == 9 || data.Type == 10 || data.Type == 11 || data.Type == 12) maxPrice = Main.PricesSettings.TattooBarberMasksLscMaxPrice;
                        
                        if (data.Products[i].Price < minPrice || data.Products[i].Price > maxPrice)
                        {
                            data.Products[i].Price = BusProductsData[nameprod].Price;
                            changed = true;
                        }
                    }
                }
                if (changed) 
                    data.IsSave = true;
            }
            catch (Exception e)
            {
                Log.Write($"UpdateBusProd Exception: {e.ToString()}");
            }
        }

        public static void Init()
        {
            try
            {
                Main.CreateBlip(new Main.BlipData(73, "Premium Clothes Shop", new Vector3(-1126.9141, -1440.1637, 4.108331), 35, true, 1f));
                PedSystem.Repository.CreateQuest("a_m_y_hipster_01", new Vector3(-1126.9141, -1440.1637, 4.108331 + 1.12), -63.85f, title: "~y~NPC~w~ Вовчик", colShapeEnums: ColShapeEnums.PremiumShop);

                using MySqlCommand cmd = new MySqlCommand()
                {
                    CommandText = "SELECT * FROM businesses"
                };
                using DataTable result = MySQL.QueryRead(cmd);
                if (result == null || result.Rows.Count == 0)
                {
                    Log.Write("DB biz return null result.", nLog.Type.Warn);
                    return;
                }
                foreach (DataRow Row in result.Rows)
                {
                    try
                    {

                        Vector3 enterpoint = JsonConvert.DeserializeObject<Vector3>(Row["enterpoint"].ToString());
                        Vector3 unloadpoint = JsonConvert.DeserializeObject<Vector3>(Row["unloadpoint"].ToString());
                        int bankmoney = Convert.ToInt32(Row["money"]);

                        /*
                        using DataTable result1 = MySQL.QueryRead($"SELECT * FROM money WHERE id={bankmoney}");
                        if (result1 == null || result1.Rows.Count == 0) MySQL.Query($"INSERT INTO `money`(`id`, `type`, `holder`, `balance`) VALUES ({bankmoney},3,'{string.Empty}',1000)");
                        */
                        List<Product> prodlist = JsonConvert.DeserializeObject<List<Product>>(Row["products"].ToString());

                        int id = Convert.ToInt32(Row["id"]);
                        Business data = new Business(id, Row["owner"].ToString(), Convert.ToInt32(Row["sellprice"]), Convert.ToInt32(Row["type"]), prodlist, enterpoint, unloadpoint, bankmoney,
                            Convert.ToInt32(Row["mafia"]), JsonConvert.DeserializeObject<List<Order>>(Row["orders"].ToString()), Convert.ToDouble(Row["tax"]));
                        lastBizID = id;

                        UpdateBusProd(data);
                        if (data.Type == 12)
                        {
                            int index = GetLsCustomsPlaces(data.EnterPoint);
                            if (!LsCustomsPlacesToBusnessId.ContainsKey(index)) LsCustomsPlacesToBusnessId.Add(index, id);
                            if (!LsCustomsBusnessToPlacesId.ContainsKey(id)) LsCustomsBusnessToPlacesId.Add(id, index);
                        }

                        BizList.TryAdd(id, data);
                    }
                    catch (Exception x)
                    {
                        Log.Write("Cant load Business: " + x.ToString());
                    }
                }
                
                Players.Phone.Gps.Repository.Init();
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }

        public static void SavingBusiness()
        {
            try
            {
                foreach (int b in BizList.Keys)
                    BizList[b].IsSave = true;
            }
            catch (Exception e)
            {
                Log.Write($"SavingBusiness Exception: {e.ToString()}");
            }
        }

        [ServerEvent(Event.ResourceStop)]
        public void OnResourceStop()
        {
            try
            {
                SavingBusiness();
            }
            catch (Exception e)
            {
                Log.Write($"OnResourceStop Exception: {e.ToString()}");
            }
        }

        public static ConcurrentDictionary<int, Business> BizList = new ConcurrentDictionary<int, Business>();
        public static ConcurrentDictionary<int, int> Orders = new ConcurrentDictionary<int, int>(); // key - ID заказа, value - ID бизнеса

        public static string[] BusinessTypeNames = new string[16]
        {
            "24/7", // 0
            "Petrol Station", // 1
            "Premium Autoroom", // 2
            "Luxor Autoroom", // 3
            "Low Autoroom", // 4
            "Motoroom", // 5
            "Gun shop", // 6
            "Clothes Shop", // 7
            "Burger-Shot", // 8
            "Tattoo-salon", // 9
            "Barber-Shop", // 10
            "Masks Shop", // 11
            "LS Customs", // 12
            "CarWash", // 13
            "PetShop", // 14
            "Elite Autoroom", // 15
        };
        public static int[] BlipByType = new int[16]
        {
            52, // 24/7
            361, // petrol station
            530, // premium
            523, // sport
            225, // middle
            522, // moto
            567, // gun shop
            366, // clothes shop
            628, // burger-shot
            75, // tattoo-salon
            71, // barber-shop
            362, //463, // masks shop
            643, // ls customs
            524, // carwash
            273, // Petshop
            669, // Rare Autoroom
        };
        public static int[] BlipColorByType = new int[16]
        {
            4, // 24/7
            35, //76, // petrol station
            4, // showroom
            4, // showroom
            4, // showroom
            4, // showroom
            4, // gun shop
            4, // clothes shop
            73, // burger-shot
            9, // tattoo-salon
            4, // barber-shop
            50, // masks shop
            4, // ls customs
            3, // carwash
            4, // petshop
            4, // showroom
        };

        public static string[] PetNames = new string[9]
        {
            "Husky",
            "Poodle",
            "Pug",
            "Retriever",
            "Rottweiler",
            "Shepherd",
            "Westy",
            "Cat",
            "Rabbit",
        };
        public static int[] PetHashes = new int[9]
        {
            1318032802, // Husky
            1125994524,
            1832265812,
            882848737, // Retriever
            -1788665315,
            1126154828,
            -1384627013,
            1462895032,
            -541762431,
        };
        public static List<string>[] CarsNames = new List<string>[5]
        {
            new List<string>() // premium
            {
                "cinquemila",
                "iwagen",
                "Sultan",
                "SultanRS",
                "Kuruma",
                "Fugitive",
                "Tailgater",
                "Sentinel",
                "F620",
                "Schwarzer",
                "Exemplar",
                "Felon",
                "Schafter2",
                "Jackal",
                "Oracle2",
                "Surano",
                "Zion",
                "Dominator",
                "FQ2",
                "Gresley",
                "Serrano",
                "Dubsta",
                "Rocoto",
                "Cavalcade2",
                "XLS",
                "Baller2",
                "Elegy",
                "Banshee",
                "Massacro2",
                "GP1",
                "Revolter",
                "Brawler",
                "Sandking",
                "Sandking2",
                "Feltzer2",
                "Massacro",
                "Casco",
                "Bifta",

                "Cogcabrio",
                "Dominator2",
                "Nightshade",
                "Contender",
                "Cog55",
                "Cognoscenti",
                "Alpha",
                "Furoregt",
                "Lynx",
                "Pariah",
                "Tampa2",
                "Tropos",
                "Felon2",
                "Sentinel2", 
                "Zion2", 
                "Fusilade",
                "Penumbra",
                "Rapidgt",
                "Rapidgt2", 
                "Schafter3",
                "Sentinel3", 
                "Voltic",
                "Gauntlet4", 
                "Caracara2", 
                "Issi7", 
                "Schafter4",
                "Novak",

                "Z190",
                "Gb200",
                "Sultan2",
                "Rebla",
                "Komoda",

                "Sugoi",
                "Vstr",

                "Penumbra2",
                "Landstalker2",
                "Yosemite3",
                "Gauntlet5",
                "Seminole2",

                "Trophytruck",

                "tailgater2",
                "cypher",
                "zr350",
                "calico",
                "euros",
                "dominator7",
                "sultan3",
                "datsun",
                "corsita",
                "omnisegt"
            }, // premium
            new List<string>() // Luxor
            {
                "astron",
                "baller7",
                "comet7",
                "zeno",
                "Comet2",
                "Coquette",
                "Ninef",
                "Ninef2",
                "Jester",
                "Elegy2",
                "Deveste",
                "Infernus",
                "Carbonizzare",
                "Dubsta2",
                "Baller3",
                "Huntley",
                "Superd",
                "Windsor",
                "BestiaGTS",
                "Banshee2",
                "EntityXF",
                "Neon",
                "Jester2",
                "Turismor",
                "Penetrator",
                "Omnis",
                "Reaper",
                "Italigtb2",
                "Xa21",
                "Osiris",
                "Nero",
                "Zentorno",
                "Infernus2",
                "Cheetah2",
                "Feltzer3",
                "Mamba",
                "Monroe",
                "Rapidgt3",
                "Swinger",
                "Torero",
                "Turismo2",
                "Viseris",
                "Coquette3",

                "Dubsta3",
                "Comet3",
                "Khamelion",
                "Seven70",
                "Specter",
                "Specter2",
                "Verlierer2",
                "Adder",
                "Bullet",
                "Cheetah",
                "Cyclone",
                "Fmj",
                "Pfister811",
                "Sc1",
                "T20",
                "Tempesta",
                "Vacca",
                "Vagner",
                "Visione",
                "Baller4",
                "Jester3",
                "Coquette2",
                "Gt500",
                "Stinger", 
                "Stingergt", 
                "Italigtb", 
                "Windsor2",
                "Deviant",
                "Dominator3",
                "Toros",
                "Italigto",
                "Jugular",
                "Locust",
                "Neo",
                "Paragon",
                "Schlagen",
                "Thrax",
                "Drafter",
                "Emerus",
                "Zorrusso",
                
                "Comet5",
                "Furia",
                "Taipan",
                "Entity2",
                "Imorgon",

                "Flashgt",
                "Jb7002",
                "Autarch",

                "Coquette4",
                "Tigon",

                "ffastback66",
                "fmustang95",

                "rt3000",
                "vectre",
                "growler",
                "jester4",
                "comet6",
                "sm722",
                "torero2",
                "lm87"
            }, // sport
            new List<string>() // Low
            {
                "Tornado3",
                "Tornado4",
                "Emperor2",
                "Voodoo2",
                "Regina",
                "Ingot",
                "Emperor",
                "Picador",
                "Minivan",
                "Blista2",
                "Manana",
                "Dilettante",
                "Asea",
                "Glendale",
                "Voodoo",
                "Surge",
                "Primo",
                "Stanier",
                "Stratum",
                "Tampa",
                "Prairie",
                "Radi",
                "Blista",
                "Stalion",
                "Asterope",
                "Washington",
                "Premier",
                "Intruder",
                "Ruiner",
                "Oracle",
                "Phoenix",
                "Gauntlet",
                "Buffalo",
                "RancherXL",
                "Seminole",
                "Baller",
                "Landstalker",
                "Cavalcade",
                "BJXL",
                "Patriot",
                "Bison3",
                "Issi2",
                "Panto",
                "Rhapsody",
                "Buccaneer2",
                "Dukes",
                "Faction",
                "Faction2",
                "Hermes",
                "Moonbeam2",
                "Futo",

                "Blade",
                "Chino",
                "Chino2",
                "Gauntlet2",
                "Sabregt",
                "Sabregt2",
                "Stalion2",
                "Bfinjection",
                "Bodhi2",
                "Rebel",
                "Rebel2",
                "Primo2",
                "Warrener",
                "Buffalo2",
                "Slamvan",
                "Virgo",
                "Virgo2",
                "Virgo3",
                "Blista3",
                "Buffalo3",
                "Peyote",
                "Retinue",
                "Savestra",
                "Tornado",
                "Tornado2",
                "Brioso2",
                "Weevil",
                "Tornado5",
                "Brioso", 
                "Issi3", 
                "Tulip",
                "Vamos",
                "Hellion",
                "Nebula",
                "Cheburek",
                "Zion3",
                "Gauntlet3",
                "Imperator",
                "Habanero",

                "Ellie",
                "Retinue2",
                "Yosemite2",
                "Asbo",
                "Kanjo",

                "Clique",
                "Ratloader2",
                "Yosemite",
                "Dloader",
                "Dynasty",
                "Fagaloa",

                "Club",
                "Dukes3",
                "Glendale2",
                "Peyote3",
                "Manana2",

                "Hotring",
                "Brutus",

                "warrener2",
                "previon",
                "remus",
                "futo2",
                "dominator8",
                "greenwood"

            }, // middle
            new List<string>() // moto
            {
                "reever",
                "shinobi",
                "Faggio2",
                "Sanchez2",
                "Enduro",
                "PCJ",
                "Hexer",
                "Lectro",
                "Nemesis",
                "Hakuchou",
                "Ruffian",
                "Bmx",
                "Scorcher",
                "BF400",
                "CarbonRS",
                "Bati",
                "Double",
                "Diablous",
                "Cliffhanger",
                "Akuma",
                "Thrust",
                "Nightblade",
                "Vindicator",
                "Ratbike",
                "Blazer",
                "Gargoyle",
                "Sanctus",
                "Verus",

                "Bagger",
                "Diablous2",
                "Sovereign",
                "Avarus",
                "Bati2",
                "Daemon",
                "Daemon2",
                "Defiler",
                "Vortex",
                "Vader",
                "Esskey",
                "Faggio",
                "Faggio3",
                "Manchez",
                "Wolfsbane",
                "Zombiea",
                "Zombieb",
                "Blazer4",
                "Deathbike",

                "Fcr",
                "Fcr2",
                "Hakuchou2",
                "Innovation",
                "Stryder",
                "Fixter",
                "Cruiser",
                "Tribike",
                "Tribike2",
                "Tribike3",
                "Blazer3",

                "Deathbike2",
                "Deathbike3",

                "Quad1",
            }, // moto
            new List<string>() // elite
            {
                "Caddy",
                "Romero",
                "Hotknife",
                "Ruston",
                "Outlaw",
                "Stafford",
                "Shotaro",
                "ZType",
                "RRSVR",
                "BmwM3",
                "BmwM5",
                "BmwM8",
                "MBC63S",
                "MBG63",
                "AudiR8",
                "BmwI8",
                "DodgeViper",
                "BentleyC",
                "Camry",
                "LX570",
                "AudiRS7",
                "NGTR",
                "Lambo770",
                "BMWE38",
                "MBE420",
                "BentleyB",
                "MazdaRX7",
                "Supra",
                "SubaruWRX",
                "BMWX6",
                "MBC63",
                "MLEvoX",
                "AudiTT",
                "Skyline",
                "TeslaS",
                "Regera",
                "Lambo640",
                "Ferrari488",
                "lambo580",
                "cadctsv",
                "quad1",
                "vapidse"
            } // elite
        };

        public static List<Product> fillProductList(int type)
        {
            try
            {
                List<Product> _ProductsList = new List<Product>();
                switch (type)
                {
                    case 0:
                        foreach (KeyValuePair<string, BusProductData> busProductData in BusProductsData)
                        {
                            if (busProductData.Value.Type != BusProductToType.Market) continue;
                            Product product;
                            if (busProductData.Key == "Лотерейный билет") product = new Product(500, 0, 0, busProductData.Key, false);
                            else product = new Product(busProductData.Value.Price, 0, 1, busProductData.Key, false);
                            _ProductsList.Add(product);
                        }
                        break;
                    case 1:
                        _ProductsList.Add(new Product(BusProductsData["Бензин"].Price, 0, 0, "Бензин", false));
                        break;
                    case 2:
                        foreach (string name in CarsNames[0])
                        {
                            if (!BusProductsData.ContainsKey(name)) continue;
                            var productsData = BusProductsData[name];
                            if (!productsData.Toggled) continue;
                            if (productsData.Price == 0) continue;
                            Product product = new Product(productsData.Price, 0, 0, name, false);
                            _ProductsList.Add(product);
                        }
                        break;
                    case 3:
                        foreach (string name in CarsNames[1])
                        {
                            if (!BusProductsData.ContainsKey(name)) continue;
                            var productsData = BusProductsData[name];
                            if (!productsData.Toggled) continue;
                            if (productsData.Price == 0) continue;
                            Product product = new Product(productsData.Price, 0, 0, name, false);
                            _ProductsList.Add(product);
                        }
                        break;
                    case 4:
                        foreach (string name in CarsNames[2])
                        {
                            if (!BusProductsData.ContainsKey(name)) continue;
                            var productsData = BusProductsData[name];
                            if (!productsData.Toggled) continue;
                            if (productsData.Price == 0) continue;
                            Product product = new Product(productsData.Price, 0, 0, name, false);
                            _ProductsList.Add(product);
                        }
                        break;
                    case 5:
                        foreach (string name in CarsNames[3])
                        {
                            if (!BusProductsData.ContainsKey(name)) continue;
                            var productsData = BusProductsData[name];
                            if (!productsData.Toggled) continue;
                            if (productsData.Price == 0) continue;
                            Product product = new Product(productsData.Price, 0, 0, name, false);
                            _ProductsList.Add(product);
                        }
                        break;
                    case 6:
                        foreach (KeyValuePair<string, BusProductData> busProductData in BusProductsData)
                        {
                            if (busProductData.Value.Type != BusProductToType.Gun) continue;
                            Product product = new Product(busProductData.Value.Price, 0, 5, busProductData.Key, false);
                            _ProductsList.Add(product);
                        }
                        _ProductsList.Add(new Product(BusProductsData["Патроны"].Price, 0, 5, "Патроны", false));
                        _ProductsList.Add(new Product(BusProductsData["Модификации"].Price, 0, 5, "Модификации", false));
                        break;
                    case 7:
                        _ProductsList.Add(new Product(50, 200, 10, "Одежда", false));
                        break;
                    case 8:
                        foreach (KeyValuePair<string, BusProductData> busProductData in BusProductsData)
                        {
                            if (busProductData.Value.Type != BusProductToType.Burger) continue;
                            Product product = new Product(busProductData.Value.Price, 10, 1, busProductData.Key, false);
                            _ProductsList.Add(product);
                        }
                        break;
                    case 9:
                        _ProductsList.Add(new Product(80, 100, 0, "Расходники", false));
                        _ProductsList.Add(new Product(100, 0, 0, "Татуировки", false));
                        break;
                    case 10:
                        _ProductsList.Add(new Product(80, 100, 0, "Расходники", false));
                        _ProductsList.Add(new Product(100, 0, 0, "Парики", false));
                        break;
                    case 11:
                        _ProductsList.Add(new Product(100, 50, 0, "Маски", false));
                        break;
                    case 12:
                        _ProductsList.Add(new Product(100, 1000, 0, "Запчасти", false));
                        break;
                    case 13:
                        _ProductsList.Add(new Product(200, 200, 0, "Средство для мытья", false));
                        break;
                    case 14:
                        _ProductsList.Add(new Product(45000, 20, 0, "Корм для животных", false));
                        break;
                }
                return _ProductsList;
            }
            catch (Exception e)
            {
                Log.Write($"fillProductList Exception: {e.ToString()}");
                return new List<Product>();
            }
        }
        [Interaction(ColShapeEnums.BusinessAction)]
        public static void OnBusinessAction(ExtPlayer player, int ID)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.DemorganTime >= 1) return;
                sessionData.BizID = ID; // Нужно переделать.
                int bizlistid = sessionData.BizID;
                if (bizlistid == -1) return;
                if (sessionData.Following != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomebodyYouFollow), 3000);
                    return;
                }
                if (!BizList.ContainsKey(bizlistid)) return;
                Business biz = BizList[bizlistid];
                if (biz.IsOwner() && !Main.PlayerNames.Values.Contains(biz.Owner))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BizNotWork, BusinessTypeNames[biz.Type]), 3000);
                    return;
                }
                switch (biz.Type)
                {
                    case 0:
                        OpenBizShopMenu(player);
                        return;
                    case 1:
                        if (!player.IsInVehicle) return;
                        if (player.VehicleSeat != (int)VehicleSeat.Driver) return;
                        OpenPetrolMenu(player);
                        return;
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 15:
                        if (sessionData.Follower != null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OtpustiteChela), 3000);
                            return;
                        }
                        sessionData.TempBizID = biz.ID;
                        CarRoom.enterCarroom(player);
                        return;
                    case 6:
                        sessionData.TempBizID = biz.ID;
                        OpenGunShopMenu(player);
                        return;
                    case 7:

                        if ((sessionData.WorkData.OnDuty && Fractions.Manager.FractionTypes[player.GetFractionId()] == FractionsType.Gov) || sessionData.WorkData.OnWork)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustEndWorkDay), 3000);
                            return;
                        }
                        sessionData.TempBizID = biz.ID;
                        OpenClothes(player, biz, isGloves: true);
                        return;
                    case 11:
                        if ((sessionData.WorkData.OnDuty && Fractions.Manager.FractionTypes[player.GetFractionId()] == FractionsType.Gov) || sessionData.WorkData.OnWork)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustEndWorkDay), 3000);
                            return;
                        }
                        sessionData.TempBizID = biz.ID;
                        OpenClothes(player, biz, dellClothes: new List<string>() {
                            ClothesComponent.Hat.ToString(),
                            ClothesComponent.Tops.ToString(),
                            ClothesComponent.Undershort.ToString(),
                            ClothesComponent.Legs.ToString(),
                            ClothesComponent.Shoes.ToString(),
                            ClothesComponent.Watches.ToString(),
                            ClothesComponent.Glasses.ToString(),
                            ClothesComponent.Accessories.ToString(),
                            ClothesComponent.Ears.ToString(),
                            ClothesComponent.Bracelets.ToString(),
                            ClothesComponent.Torsos.ToString()},
                        addClothes: new List<string> { ClothesComponent.Masks.ToString() });
                        Customization.ApplyMaskFace(player);
                        return;
                    case 8:
                        OpenBizShopMenu(player);
                        return;
                    case 9:
                        if ((sessionData.WorkData.OnDuty && Fractions.Manager.FractionTypes[player.GetFractionId()] == FractionsType.Gov) || sessionData.WorkData.OnWork)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustEndWorkDay), 3000);
                            return;
                        }
                        var custom = player.GetCustomization();
                        if (custom == null) return;
                        sessionData.TempBizID = biz.ID;
                        uint dim = Dimensions.RequestPrivateDimension(player.Value);
                        Trigger.Dimension(player, dim);
                        
                        var realClothes = new List<string>();
                        var tattooPriceList = ClothesComponents.TattooComponentPriceData;
                        var dataJson = new Dictionary<string, List<List<object>>>();

                        var prod = biz.Products[1];
                        
                        
                        foreach (var components in tattooPriceList)
                        {
                            var name = components.Key.ToString();
                            var clothesData = new List<List<object>>();
                            foreach (var componentData in components.Value)
                            {
                                clothesData.Add(new List<object>
                                {
                                    componentData[0],
                                    Convert.ToInt32(Convert.ToInt32(componentData[1]) / 100f * prod.Price)
                                }); 
                            }
                            
                            dataJson[name] = clothesData;
                            realClothes.Add(name);
                        }
                        
                        TryUpdateArmor(player);
                        Trigger.ClientEvent(player, "client.shop.open", "tattoo", characterData.Gender, JsonConvert.SerializeObject(realClothes), JsonConvert.SerializeObject(dataJson), 0, JsonConvert.SerializeObject(custom.Tattoos));
                        return;
                    case 10:
                        if ((sessionData.WorkData.OnDuty && Fractions.Manager.FractionTypes[player.GetFractionId()] == FractionsType.Gov) || sessionData.WorkData.OnWork)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustEndWorkDay), 3000);
                            return;
                        }
                        sessionData.TempBizID = biz.ID;
                        dim = Dimensions.RequestPrivateDimension(player.Value);
                        Trigger.Dimension(player, dim);
                        
                        realClothes = new List<string>();
                        var barberPriceList = ClothesComponents.BarberComponentPriceData[characterData.Gender];

                        dataJson = new Dictionary<string, List<List<object>>>();
                        
                        foreach (var components in barberPriceList)
                        {
                            var name = components.Key.ToString();
                            var clothesData = new List<List<object>>();
                            foreach (var componentData in components.Value)
                            {
                                clothesData.Add(new List<object>
                                {
                                    componentData[0],
                                    Convert.ToInt32(Convert.ToInt32(componentData[1]) / 100f *  biz.Products[1].Price)
                                }); 
                            }

                            dataJson[name] = clothesData;
                            realClothes.Add(name);
                        }
                        
                        TryUpdateArmor(player);
                        Trigger.ClientEvent(player, "client.shop.open", "barber", characterData.Gender, JsonConvert.SerializeObject(realClothes), JsonConvert.SerializeObject(dataJson), 0);
                        return;
                    case 12: // ERORR 21
                        StartTuning(player);
                        return;
                    case 13:
                        if (!player.IsInVehicle)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustInCar), 3000);
                            return;
                        }
                        Trigger.ClientEvent(player, "openDialog", "CARWASH_PAY", LangFunc.GetText(LangType.Ru, DataName.CarWashWant, biz.Products[0].Price));
                        return;
                    case 14:
                        if (sessionData.Follower != null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OtpustiteChela), 3000);
                            return;
                        }
                        sessionData.TempBizID = biz.ID;
                        enterPetShop(player, biz.Products[0].Name);
                        return;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"OnBusinessAction Exception: {e.ToString()}");
            }
        }

        public static void enterPetShop(ExtPlayer player, string prodname) {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (sessionData.TempBizID == -1 || !BizList.ContainsKey(sessionData.TempBizID)) return;
                characterData.ExteriorPos = player.Position;
                uint mydim = (uint)(player.Value + 500);
                Trigger.Dimension(player, mydim);
                NAPI.Entity.SetEntityPosition(player, new Vector3(-758.3929, 319.5044, 175.302));
                Trigger.PlayAnimation(player, "amb@world_human_sunbathe@male@back@base", "base", 39);
                // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "enterpet");
                List<int> _Prices = new List<int>();
                Business biz = BizList[sessionData.TempBizID];
                for (byte i = 0; i != 9; i++) _Prices.Add(biz.Products[0].Price);
                Trigger.ClientEvent(player, "openPetshop", JsonConvert.SerializeObject(PetNames), JsonConvert.SerializeObject(PetHashes), JsonConvert.SerializeObject(_Prices), mydim);
                BattlePass.Repository.UpdateReward(player, 152);
            }
            catch (Exception e)
            {
                Log.Write($"enterPetShop Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("petshopBuy")]
        public static void RemoteEvent_petshopBuy(ExtPlayer player, string petName)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (sessionData.TempBizID == -1 || !BizList.ContainsKey(sessionData.TempBizID)) return;
                Trigger.StopAnimation(player);
                Business biz = BusinessManager.BizList[sessionData.TempBizID];
                NAPI.Entity.SetEntityPosition(player, new Vector3(biz.EnterPoint.X, biz.EnterPoint.Y, biz.EnterPoint.Z + 1.5));
                Trigger.Dimension(player, 0);
                characterData.ExteriorPos = new Vector3();

                var house = Houses.HouseManager.GetHouse(player, true);
                if (house == null || house.Type == 7)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoHome), 3000);
                    return;
                }
                if (Houses.HouseManager.HouseTypeList[house.Type].PetPosition == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.HomeNotForPets), 3000);
                    return;
                }
                int price = biz.Products[0].Price;
                if (characterData.Money < price)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMoney), 3000);
                    return;
                }
                if (!takeProd(biz.ID, 1, "Корм для животных", price))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoPets), 3000);
                    return;
                }
                Wallet.Change(player, -price);
                biz.BuyItemBusiness(characterData.UUID, "Корм для животных", price);
                GameLog.Money($"player({characterData.UUID})", $"biz({biz.ID})", price, $"buyPet({petName})");
                house.PetName = petName;
                characterData.PetName = petName;
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NewPet, petName), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_petshopBuy Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("petshopCancel")]
        public static void RemoteEvent_petshopCancel(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (sessionData.TempBizID == -1 || !BizList.ContainsKey(sessionData.TempBizID)) return;
                Trigger.StopAnimation(player);
                Vector3 enterPoint = BusinessManager.BizList[sessionData.TempBizID].EnterPoint;
                Trigger.Dimension(player, 0);
                NAPI.Entity.SetEntityPosition(player, new Vector3(enterPoint.X, enterPoint.Y, enterPoint.Z + 1.5));
                characterData.ExteriorPos = new Vector3();
                sessionData.TempBizID = -1;
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_petshopCancel Exception: {e.ToString()}");
            }
        }

        public static void Carwash_Pay(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (sessionData.BizID == -1 || !BizList.ContainsKey(sessionData.BizID)) return;
                Business biz = BizList[sessionData.BizID];
                if (player.IsInVehicle)
                {
                    if (player.VehicleSeat == (int)VehicleSeat.Driver)
                    {
                        var vehicle = (ExtVehicle) player.Vehicle;
                        var vehicleLocalData = vehicle.GetVehicleLocalData();
                        if (vehicleLocalData == null)
                            return;
                        if (VehicleStreaming.GetVehicleDirt(vehicle) >= 0.01f)
                        {
                            int price = biz.Products[0].Price;
                            if (characterData.Money < price)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMoney), 3000);
                                return;
                            }

                            if (!takeProd(biz.ID, 1, "Средство для мытья", price))
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetTovaraNaSkladeBiza), 3000);
                                return;
                            }
                            GameLog.Money($"player({characterData.UUID})", $"biz({biz.ID})", price, "carwash");
                            MoneySystem.Wallet.Change(player, -price);
                            biz.BuyItemBusiness(characterData.UUID, "Средство для мытья", price);
                            VehicleStreaming.SetVehicleDirt(vehicle, 0.0f);
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WashedCar), 5000);
                            if (vehicleLocalData.Access == VehicleAccess.Fraction) 
                                player.AddTableScore(TableTaskId.Item2);
                            BattlePass.Repository.UpdateReward(player, 27);
                        }
                        else Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NotDirtyCar), 5000);
                    }
                    else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OnlyDriverWashCar), 5000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Carwash_Pay Exception: {e.ToString()}");
            }
        }


        //[RemoteEvent("server.ls_customs.start")]
        public static void StartTuning(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!player.IsInVehicle)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustInCar), 3000);
                    return;
                }
                if (player.VehicleSeat != (int)VehicleSeat.Driver)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustBeDriver), 3000);
                    return;
                }
                var vehicle = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData == null)
                    return;
                
                if (vehicleLocalData.Access == VehicleAccess.Personal)
                {
                    if (vehicle.Class == 13)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VeloCantBeTuned), 3000);
                        return;
                    }
                    if (VehicleModel.AirAutoRoom.isAirCar(vehicle.Model))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Воздушный транспорт не может быть затюнингован", 3000);
                        return;
                    }
                    //VehicleManager.VehicleData vdata = VehicleManager.Vehicles[vehicle.NumberPlate];
                    //if (!Tuning.ContainsKey(vdata.Model))
                    // {
                    //     Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"В данный момент для Вашего т/с тюнинг не доступен", 3000);
                    //     return;
                    // }
                    var bizId = sessionData.BizID;
                    if (!BizList.ContainsKey(bizId)) return;
                    if (sessionData.Following != null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomebodyYouFollow), 3000);
                        return;
                    }
                    Business biz = BizList[bizId];
                    if (biz == null) return;
                    var vehicleData = VehicleManager.GetVehicleToNumber(vehicle.NumberPlate);
                    if (vehicleData == null) return;
                    string model = vehicleData.Model;
                    if (!BusProductsData.ContainsKey(model)) return;
                    characterData.TuningShop = bizId;

                    uint dim = Dimensions.RequestPrivateDimension(player.Value);

                    Trigger.Dimension(vehicle, dim);

                    if (!LsCustomsBusnessToPlacesId.ContainsKey(bizId))
                    {
                        vehicle.Position = new Vector3(-337.7784, -136.5316, 39.4032);
                        vehicle.Rotation = new Vector3(0, 0, 148.9986);
                    }
                    else
                    {
                        var lsCustomsPlace = LsCustomsPlaces[LsCustomsBusnessToPlacesId[bizId]];
                        vehicle.Position = lsCustomsPlace.Position;
                        vehicle.Rotation = lsCustomsPlace.Rotation;
                    }
                    
                    int modelPrice = BusProductsData[model].Price;
                    modelPrice = CarsNames[2].Contains(model) ? modelPrice * 10 : modelPrice;

                    Trigger.ClientEvent(player, "client.custom.open", biz.Products[0].Price, modelPrice, JsonConvert.SerializeObject(vehicleData.Components), 0);

                }
                else if (vehicleLocalData.Access == VehicleAccess.Organization && player.IsOrganizationAccess(RankToAccess.OrgTuning))
                {
                    var bizId = sessionData.BizID;
                    if (!BizList.ContainsKey(bizId)) return;
                    if (sessionData.Following != null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomebodyYouFollow), 3000);
                        return;
                    }
                    
                    var organizationData = Organizations.Manager.GetOrganizationData(vehicleLocalData.Fraction);
                    if (organizationData == null) 
                        return;
                    
                    characterData.TuningShop = bizId;

                    var organizationVehicle = organizationData.Vehicles[vehicle.NumberPlate];
                    var model = organizationVehicle.model;
        
                    uint dim = Dimensions.RequestPrivateDimension(player.Value);
                    Trigger.Dimension(vehicle, dim);
                    if (!LsCustomsBusnessToPlacesId.ContainsKey(bizId))
                    {
                        vehicle.Position = new Vector3(-337.7784, -136.5316, 39.4032);
                        vehicle.Rotation = new Vector3(0, 0, 148.9986);
                    }
                    else
                    {
                        var lsCustomsPlace = LsCustomsPlaces[LsCustomsBusnessToPlacesId[bizId]];
                        vehicle.Position = lsCustomsPlace.Position;
                        vehicle.Rotation = lsCustomsPlace.Rotation;
                    }
                    
                    //Trigger.ClientEvent(player, "vehicle.teleport");
                    int modelPrice = BusProductsData[model].Price;
                    modelPrice = CarsNames[2].Contains(model) ? modelPrice * 10 : modelPrice;
                    //NAPI.Vehicle.SetVehicleNeonState(veh, true);
                    //NAPI.Vehicle.SetVehicleNeonColor(veh, 0, 0, 0);

                    Trigger.ClientEvent(player, "client.custom.open", 100, modelPrice, JsonConvert.SerializeObject(organizationVehicle.customization), 1);
                }
            }
            catch (Exception e)
            {
                Log.Write($"StartTuning Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.custom.exit")]
        public static void RemoteEvent_exitTuning(ExtPlayer player, int eventType)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!player.IsInVehicle) return;

                var vehicle = (ExtVehicle) player.Vehicle;
                
                var bizId = characterData.TuningShop;
                switch (eventType)
                {
                    case 0:
                    case 2:
                        if (eventType == 0)
                        {
                            if (bizId == -1) return;
                            vehicle.Position = new Vector3(BizList[bizId].EnterPoint.X, BizList[bizId].EnterPoint.Y, (BizList[bizId].EnterPoint.Z + 1.0));
                            vehicle.Rotation = new Vector3(0, 0, player.Vehicle.Rotation.Z);
                        }
                        else
                        {
                            var positionData = VehicleModel.AirAutoRoom.GetSpawnPosition();
                            vehicle.Position = positionData.Item1;
                            vehicle.Rotation = positionData.Item2;
                        }
                        Trigger.Dimension(vehicle, 0);
                        VehicleManager.ApplyCustomization(vehicle);
                        VehicleManager.GetVehicleCustomization(player, vehicle);
                        characterData.TuningShop = -1;
                        break;
                    case 1:
                        if (bizId == -1) return;
                        
                        var organizationData = player.GetOrganizationData();
                        if (organizationData == null) 
                            return;

                        vehicle.Position = new Vector3(BizList[bizId].EnterPoint.X, BizList[bizId].EnterPoint.Y, (BizList[bizId].EnterPoint.Z + 1.0));
                        vehicle.Rotation = new Vector3(0, 0, player.Vehicle.Rotation.Z);                        
                        Trigger.Dimension(vehicle, 0);

                        
                        var number = NAPI.Vehicle.GetVehicleNumberPlate(vehicle);
                        var organizationVehicle = organizationData.Vehicles[number];
                        VehicleManager.GetVehicleCustomization(player, vehicle);
                        VehicleManager.OrgApplyCustomization(vehicle, organizationVehicle.customization);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_exitTuning Exception: {e.ToString()}");
            }
        }

        public static IReadOnlyDictionary<string, float> ComponentsPrice = new Dictionary<string, float>()
        {
            { "Spoiler", 5.0f },
            { "FrontBumper", 5.0f },
            { "RearBumper", 5.0f },
            { "SideSkirt", 5.0f },
            { "Muffler", 5.0f },
            { "Frame", 5.0f },
            { "Lattice", 5.0f },
            { "Hood", 5.0f },
            { "Wings", 5.0f },
            { "Roof", 5.0f },
            { "FrontWheels", 6.0f },
            { "Horn", 4.0f },
            { "Xenon", 10.0f },
            { "Headlights", 10.0f },
            { "Engine", 25.0f },
            { "Turbo", 25.0f },
            { "Transmission", 20.0f },
            { "Suspension", 25.0f },
            { "Brakes", 25.0f },
            { "WindowTint", 15.0f },
            { "Color1", 5.0f },
            { "Color2", 5.0f },
            { "ColorAdditional", 10.0f },
            { "Cover", 10.0f },
            { "Vinyls", 10.0f },
            { "NumberPlate", 10.0f },
        };

        public static IReadOnlyDictionary<int, int> WindowTintModToIndex = new Dictionary<int, int>()
        {
            { -1, -1 },
            { 3, 1 },
            { 2, 2 },
            { 1, 3 },
        };
        public static int GetPrice(string category, int index, int pricePercent, int priceVehicle)
        {
            try
            {
                if (category == "WindowTint" && WindowTintModToIndex.ContainsKey(index)) index = WindowTintModToIndex[index];
                float pricePercentf = pricePercent / 100f;
                float priceVehiclef = priceVehicle / 100f;
                if (index == -1) return Convert.ToInt32(pricePercentf * priceVehiclef * ComponentsPrice[category] * (0.5f / 100f));
                if (index == 0) index = 1;
                float indexf = index / 100f;
                return Convert.ToInt32(pricePercentf * priceVehiclef * ComponentsPrice[category] * indexf);
            }
            catch (Exception e)
            {
                Log.Write($"GetPrice Exception: {e.ToString()}");
                return 0;
            }
        }

        [RemoteEvent("server.custom.buy")]
        public static void RemoteEvent_buyTuning(ExtPlayer player, string category, int index, int otherIndex, int r, int g, int b)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!player.IsInVehicle) return;
                if (!VehicleManager.canAccessByNumber(player, player.Vehicle.NumberPlate))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoKeysFromVeh), 3000);
                    return;
                }
                int bizID = characterData.TuningShop;
                if (!BizList.ContainsKey(bizID)) return;
                Business biz = BizList[bizID];
                string number = player.Vehicle.NumberPlate;
                var vehicleData = VehicleManager.GetVehicleToNumber(number);
                if (vehicleData == null)
                    return;
                string vehModel = vehicleData.Model;
                int modelPrice = BusProductsData[vehModel].Price;
                modelPrice = BusinessManager.CarsNames[2].Contains(vehModel) ? modelPrice * 10 : modelPrice;
                int price = GetPrice(category, index, biz.Products[0].Price, modelPrice);
                if (price == 0)
                {
                    if (modelPrice < 100000)
                    {
                        price = 1;
                    }
                    else
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoDetailsError), 3000);
                        return;
                    }
                }
                int amount = Convert.ToInt32(price * 0.45 / 2000);
                if (amount <= 0) amount = 1;
                if (!takeProd(biz.ID, amount, "Запчасти", price))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoDetails), 3000);
                    return;
                }
                
                if (SetCustomization(player, category, index, otherIndex, r, g, b, price, $"biz({biz.ID})"))
                    biz.BuyItemBusiness(characterData.UUID, "Запчасти", price);
                
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_buyTuning Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.custom.gosBuy")]
        public static void RemoteEvent_buyGosTuning(ExtPlayer player, string category, int index, int otherIndex, int r, int g, int b)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!player.IsInVehicle) return;
                if (!VehicleManager.canAccessByNumber(player, player.Vehicle.NumberPlate))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoKeysFromVeh), 3000);
                    return;
                }
                
                string number = player.Vehicle.NumberPlate;
                var vehicleData = VehicleManager.GetVehicleToNumber(number);
                if (vehicleData == null)
                    return;
                string vehModel = vehicleData.Model;                
                int modelPrice = BusProductsData[vehModel].Price;
                modelPrice = CarsNames[2].Contains(vehModel) ? modelPrice * 10 : modelPrice;
                int price = GetPrice(category, index, 100, modelPrice);
                if (price == 0)
                {
                    if (modelPrice < 100000)
                    {
                        price = 1;
                    }
                    else
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoDetailsError), 3000);
                        return;
                    }
                }

                SetCustomization(player, category, index, otherIndex, r, g, b, price);
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_buyGosTuning Exception: {e.ToString()}");
            }
        }
        private static bool SetCustomization(ExtPlayer player, string category, int index, int otherIndex, int r, int g, int b, int price, string textLogs = "ExoticAutoRoom")
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return false;
            if (!player.IsInVehicle) return false;
            
            string number = player.Vehicle.NumberPlate;
            var vehicleData = VehicleManager.GetVehicleToNumber(number);
            if (vehicleData == null)
                return false;
            
            if (UpdateData.CanIChange(player, price, true) != 255) 
                return false;
            
            GameLog.Money($"player({characterData.UUID})", textLogs, price, $"buyTuning({player.Vehicle.NumberPlate},{category},{index} ({r}, {g}, {b}))");
            Wallet.Change(player, -price);
            
            vehicleData.Components = GetCustomization(vehicleData.Components, category, index, otherIndex, r, g, b);
                
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BuyTunNotify, Wallet.Format(price)), 3000);
            BattlePass.Repository.UpdateReward(player, 44);
            Trigger.ClientEvent(player, "client.custom.updatecomponents", JsonConvert.SerializeObject(vehicleData.Components));
            
            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    await db.Vehicles
                        .Where(v => v.AutoId == vehicleData.SqlId)
                        .Set(v => v.Components, JsonConvert.SerializeObject(vehicleData.Components))
                        .UpdateAsync();
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
            
            qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage28, 1, isUpdateHud: true);
            qMain.UpdateQuestsComplete(player, Zdobich.QuestName, (int) zdobich_quests.Stage28, true);
            return true;
        }
        
        [RemoteEvent("server.custom.orgBuy")]
        public static void RemoteEvent_orgBuyTuning(ExtPlayer player, string category, int index, int otherIndex, int r, int g, int b)
        {
            try
            {
                if (!player.IsOrganizationAccess(RankToAccess.OrgTuning)) return;
                
                var vehicle = (ExtVehicle) player.Vehicle;
                if (vehicle == null) return;
                
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoKeysFromVeh), 3000);
                    Trigger.ClientEvent(player, "tunBuySuccess", -2);
                    return;
                }
                
                var number = NAPI.Vehicle.GetVehicleNumberPlate(vehicle);
                if (!organizationData.Vehicles.ContainsKey(number))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoKeysFromVeh), 3000);
                    Trigger.ClientEvent(player, "tunBuySuccess", -2);
                    return;
                }

                var organizationVehicle = organizationData.Vehicles[number];
                var vehModel = organizationVehicle.model;
                int modelPrice = BusProductsData[vehModel].Price;
                modelPrice = CarsNames[2].Contains(vehModel) ? modelPrice * 10 : modelPrice;
                int price = GetPrice(category, index, 100, modelPrice);
                if (price == 0)
                {
                    if (modelPrice < 100000)
                    {
                        price = 1;
                    }
                    else
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoDetailsError), 3000);
                        return;
                    }
                }
                if (UpdateData.CanIChange(player, price, true) != 255) return;

                GameLog.Money($"player({player.GetUUID()})", $"server", price, $"buyOrgTuning({number},{category},{index} ({r}, {g}, {b}))");
                Wallet.Change(player, -price);

                var customization = organizationVehicle.customization;
                organizationVehicle.customization = GetCustomization(customization, category, index, otherIndex, r, g, b);
                organizationVehicle.SaveCustomization(number);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BuyTunNotify, Wallet.Format(price)), 3000);
                Trigger.ClientEvent(player, "client.custom.updatecomponents", JsonConvert.SerializeObject(customization));
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_orgBuyTuning Exception: {e.ToString()}");
            }
        }

        private static VehicleCustomization GetCustomization(VehicleCustomization myvehcustom, string category, int index, int otherIndex, int r, int g, int b)
        {                
            switch (category)
            {
                case "Spoiler":
                    myvehcustom.Spoiler = index;
                    break;
                case "FrontBumper":
                    myvehcustom.FrontBumper = index;
                    break;
                case "RearBumper":
                    myvehcustom.RearBumper = index;
                    break;
                case "SideSkirt":
                    myvehcustom.SideSkirt = index;
                    break;
                case "Muffler":
                    myvehcustom.Muffler = index;
                    break;
                case "Frame":
                    myvehcustom.Frame = index;
                    break;
                case "Lattice":
                    myvehcustom.Lattice = index;
                    break;
                case "Hood":
                    myvehcustom.Hood = index;
                    break;
                case "Wings":
                    myvehcustom.Wings = index;
                    break;
                case "Roof":
                    myvehcustom.Roof = index;
                    break;
                case "FrontWheels":
                    myvehcustom.Wheels = index;
                    myvehcustom.WheelsType = otherIndex;
                    myvehcustom.WheelsColor = r;
                    break;
                case "Horn":
                    myvehcustom.Horn = index;
                    break;
                case "Xenon":
                    myvehcustom.NeonIndex = index;
                    myvehcustom.NeonColor = new Color(r, g, b, 255);
                    break;
                case "Headlights":
                    myvehcustom.Headlights = r - 1;
                    //player.Vehicle.SetSharedData("hlcolor", id);
                    //Trigger.ClientEvent(player, "VehStream_SetVehicleHeadLightColor", player.Vehicle.Handle, id);
                    break;
                case "Engine":
                    myvehcustom.Engine = index;
                    break;
                case "Turbo":
                    myvehcustom.Turbo = index;
                    break;
                case "Transmission":
                    myvehcustom.Transmission = index;
                    break;
                case "Suspension":
                    myvehcustom.Suspension = index;
                    break;
                case "Brakes":
                    myvehcustom.Brakes = index;
                    break;
                case "WindowTint":
                    myvehcustom.WindowTint = index;
                    break;
                case "Color1":
                    myvehcustom.PrimColor = new Color(r, g, b);
                    break;
                case "Color2":
                    myvehcustom.SecColor = new Color(r, g, b);
                    break;
                case "ColorAdditional":
                    myvehcustom.ColorAdditional = r;
                    //SetVehiclePearlescentColor(NetHandle vehicle, int color);
                    break;
                case "Cover":
                    myvehcustom.Cover = index;
                    myvehcustom.CoverColor = r;
                    //SetVehiclePrimaryPaint(NetHandle vehicle, int paintType, int color);
                    break;
                case "Vinyls":
                    myvehcustom.Vinyls = index;
                    break;
                case "NumberPlate":
                    myvehcustom.NumberPlate = index;
                    break;
                default:
                    // Not supposed to end up here. 
                    break;
            }
            return myvehcustom;
        }

        public static bool takeProd(int bizid, int amount, string prodname, int addMoney)
        {
            try
            {
                if (!BizList.ContainsKey(bizid)) return false;
                if (addMoney < 0) return false;
                Business biz = BizList[bizid];
                foreach (Product p in biz.Products)
                {
                    if (p.Name != prodname) continue;
                    if (p.Lefts - amount < 0) return false;
                    p.Lefts -= amount;
                    if (!biz.IsOwner()) break;
                    Bank.Data bData = Bank.Get(Main.PlayerBankAccs[biz.Owner]);
                    if (bData.ID == 0) return false;
                    if (!Bank.Change(bData.ID, addMoney, false)) return false;
                    GameLog.Money($"biz({bizid})", $"bank({bData.ID})", addMoney, "bizProfit");
                    break;
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Write($"takeProd Exception: {e.ToString()}");
                return false;
            }
        }
        
        public static Vector3 getNearestBiz(ExtPlayer player, int type)
        {
            try
            {
                if (!player.IsCharacterData()) return null;
                Vector3 nearestBiz = new Vector3();
                foreach (Business biz in BizList.Values)
                {
                    if (biz.Type != type) continue;
                    if (nearestBiz == new Vector3()) nearestBiz = biz.EnterPoint;
                    if (player.Position.DistanceTo(biz.EnterPoint) < player.Position.DistanceTo(nearestBiz)) nearestBiz = biz.EnterPoint;
                }
                return nearestBiz;
            }
            catch (Exception e)
            {
                Log.Write($"getNearestBiz Exception: {e.ToString()}");
                return null;
            }
        }
        [Interaction(ColShapeEnums.PremiumShop)]
        public static void OpenPremiumShop(ExtPlayer player, int index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            if (!player.IsCharacterData()) return;
            if (sessionData.CuffedData.Cuffed)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsCuffed), 3000);
                return;
            }
            else if (sessionData.DeathData.InDeath)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsDying), 3000);
                return;
            }
            else if (Main.IHaveDemorgan(player, true)) return;

            player.SelectQuest(new PlayerQuestModel("npc_premium", 0, 0, false, DateTime.Now));
            Trigger.ClientEvent(player, "client.quest.open", index, "npc_premium", 0, 0, 0);
        }
        public static string[] ClothesShopToType = new string[]
        {
            ClothesComponent.Hat.ToString(),
            ClothesComponent.Tops.ToString(),
            ClothesComponent.Undershort.ToString(),
            ClothesComponent.Legs.ToString(),
            ClothesComponent.Shoes.ToString(),
            ClothesComponent.Watches.ToString(),
            ClothesComponent.Glasses.ToString(),
            ClothesComponent.Accessories.ToString(),
            ClothesComponent.Ears.ToString(),
            ClothesComponent.Bracelets.ToString(),
            ClothesComponent.Torsos.ToString(),
        };
        public static void OpenClothes(ExtPlayer player, Business biz, bool isDonate = false, bool isGloves = false, List<string> dellClothes = null, List<string> addClothes = null)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!FunctionsAccess.IsWorking("ClothesShop"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }

                if ((sessionData.WorkData.OnDuty && Fractions.Manager.FractionTypes[player.GetFractionId()] == FractionsType.Gov) || sessionData.WorkData.OnWork)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustEndWorkDay), 3000);
                    return;
                }

                var realClothes = ClothesShopToType.ToList();
                if (dellClothes != null)
                {
                    foreach (var clothId in dellClothes)
                    {
                        if (realClothes.Contains(clothId)) 
                            realClothes.Remove(clothId);
                    }
                }
                if (addClothes != null)
                {
                    foreach (var clothId in addClothes)
                    {
                        if (!realClothes.Contains(clothId)) 
                            realClothes.Add(clothId);
                    }
                }

                var clothesPriceList = !isDonate ? ClothesComponents.ClothesComponentPriceData[characterData.Gender] : ClothesComponents.ClothesComponentDonateData[characterData.Gender];
                var dataJson = new Dictionary<string, List<List<object>>>();
                foreach (var name in realClothes.ToList())
                {
                    if (!clothesPriceList.ContainsKey(name))
                    {
                        realClothes.Remove(name);
                        continue;
                    }

                    if (!isDonate)
                    {
                        var clothesData = new List<List<object>>();
                        foreach (var componentData in clothesPriceList[name])
                        {
                            clothesData.Add(new List<object>
                            {
                                componentData[0],
                                Convert.ToInt32(Convert.ToInt32(componentData[1]) / 100f *  biz.Products[0].Price)
                            }); 
                        }

                        if (clothesData.Count > 0)
                            dataJson[name] = clothesData;
                        else
                            realClothes.Remove(name);
                    }
                    else
                    {
                        if (clothesPriceList[name].Count > 0)
                            dataJson[name] = clothesPriceList[name];
                        else
                        {
                            realClothes.Remove(name);
                        }
                    }
                }
                
                TryUpdateArmor(player);
                Trigger.ClientEvent(player, "client.shop.open", "clothes", characterData.Gender, JsonConvert.SerializeObject(realClothes), JsonConvert.SerializeObject(dataJson), Convert.ToByte(isDonate));
                Trigger.Dimension(player, Dimensions.RequestPrivateDimension(player.Value));
                //Trigger.PlayAnimation(player, "amb@world_human_stand_guard@male@base", "base", 39, false);
            }
            catch (Exception e)
            {
                Log.Write($"OpenClothes Exception: {e.ToString()}");
            }
        }
        private static void TryUpdateArmor(ExtPlayer player)
        {
            InventoryItemData item = NeptuneEvo.Chars.Repository.GetItemData(player, "accessories", 7);
            if (item.ItemId == Chars.Models.ItemId.BodyArmor)
            {
                item.Data = player.Armor.ToString();
                NeptuneEvo.Chars.Repository.SetItemData(player, "accessories", 7, item, true);
            }
        }
        
        private static Dictionary<ClothesComponent, int> ClothesComponentToSlot = new Dictionary<ClothesComponent, int>()
        {
            { ClothesComponent.Hat, 0 },//
            { ClothesComponent.Torsos, 12 },//
            { ClothesComponent.Legs, 9 },//
            { ClothesComponent.Shoes, 13 },//
            { ClothesComponent.Accessories, 4 },//
            { ClothesComponent.BodyArmors, 7 },//
            { ClothesComponent.Tops, 5 },//
            { ClothesComponent.Undershort, 6 },//
            { ClothesComponent.Masks, 1 },//
            { ClothesComponent.Ears, 2 },//
            { ClothesComponent.Watches, 11 },//
            { ClothesComponent.Glasses, 3 },///
            { ClothesComponent.Bugs, 8 },//
            { ClothesComponent.Bracelets, 10 },//
        };
        [RemoteEvent("server.clothes.buy")]
        public static void ClothesBuy(ExtPlayer player, string name, int id, int texture, bool isDonate = false)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null)
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;

                var accountData = player.GetAccountData();
                if (accountData == null)
                    return;

                if (!isDonate && (sessionData.TempBizID == -1 || !BizList.ContainsKey(sessionData.TempBizID))) return;

                Business biz = !isDonate ? BizList[sessionData.TempBizID] : null;
                Product prod = !isDonate ? biz.Products[0] : null;

                bool gender = characterData.Gender;

                var dictionary = (ClothesComponent)Enum.Parse(typeof(ClothesComponent), name);
                int slotId = ClothesComponentToSlot[dictionary];

                if (dictionary == ClothesComponent.Undershort)
                    dictionary = ClothesComponent.Tops;

                ClothesData shoesData = null;

                if (dictionary == ClothesComponent.Bugs)
                {
                    if (!ClothesComponents.ClothesBugsData.ContainsKey(id))
                        return;
                    shoesData = ClothesComponents.ClothesBugsData[id];
                }
                else
                {
                    if (!ClothesComponents.ClothesComponentData[gender].ContainsKey(dictionary))
                        return;
                    if (!ClothesComponents.ClothesComponentData[gender][dictionary].ContainsKey(id))
                        return;
                    shoesData = ClothesComponents.ClothesComponentData[gender][dictionary][id];
                }

                int tempPrice = !isDonate ? shoesData.Price : shoesData.Donate;
                
                int price = !isDonate ? Convert.ToInt32((tempPrice / 100f) * prod.Price) : tempPrice;

                if (price <= 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantBuyThis), 3000);
                    return;
                }
                if (!isDonate && UpdateData.CanIChange(player, price, true) != 255) return;
                if (isDonate && accountData.RedBucks < price)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);
                    return;
                }
                int amount = Convert.ToInt32(price * 10 * 0.75 / 100); // * 10, чтобы больше матов снимать
                if (amount <= 0) amount = 1;

                if (slotId == -1 || Chars.Repository.isFreeSlots(player, Chars.Repository.AccessoriesInfo[slotId]) != 0) return;
                if (!isDonate)
                {
                    if (dictionary == ClothesComponent.Masks && !takeProd(biz.ID, (price >= 15000 ? 6 : 1), "Маски", price))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetTovaraNaSkladeBiza), 3000);
                        return;
                    }
                    else if (!takeProd(biz.ID, amount, "Одежда", price))
                    { 
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetTovaraNaSkladeBiza), 3000);
                        return;
                    }
                }
                string Data = $"{id}_{texture}_{gender}";
                Chars.Repository.ChangeAccessoriesItem(player, slotId, Data);
                if (!isDonate)
                {
                    GameLog.Money($"player({characterData.UUID})", $"biz({biz.ID})", price, $"buyClothes({name}{Data})");
                    Wallet.Change(player, -price);
                    biz.BuyItemBusiness(characterData.UUID, "Одежда", price);
                }
                else
                {
                    Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.RedAge, LangFunc.GetText(LangType.Ru, DataName.PremClothB, price), DateTime.Now);
                    UpdateData.RedBucks(player, -price, msg: LangFunc.GetText(LangType.Ru, DataName.PremClothB, price));
                }
                
                EventSys.SendCoolMsg(player,"Покупка", "Покупка одежды", $"{LangFunc.GetText(LangType.Ru, DataName.BuyNewClothes)}", "", 6000);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BuyNewClothesView, price), 2000);
                
                if (dictionary == ClothesComponent.Masks)
                    BattlePass.Repository.UpdateReward(player, 34);
                else
                    BattlePass.Repository.UpdateReward(player, 8);
                
                if (qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int) zdobich_quests.Stage20, 2, 3, isUpdateHud: true))
                    qMain.UpdateQuestsComplete(player, Zdobich.QuestName, (int) zdobich_quests.Stage20, true);
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_buyClothes Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.shop.close")]
        public static void RemoteEvent_cancelClothes(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                sessionData.TempBizID = -1;
                //Trigger.StopAnimation(player);
                player.SetDefaultSkin();
                Trigger.Dimension(player, 0);
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_cancelClothes Exception: {e.ToString()}");
            }
        }

        private static Dictionary<ClothesComponents.BarberComponent, int> BarberComponentToSlot = new Dictionary<ClothesComponents.BarberComponent, int>()
        {
            { ClothesComponents.BarberComponent.Beard, 1 },//
            { ClothesComponents.BarberComponent.Body, 10 },//
            { ClothesComponents.BarberComponent.Eyebrows, 2 },//
            { ClothesComponents.BarberComponent.Lips, 8 },//
            { ClothesComponents.BarberComponent.Makeup, 4 },//
            { ClothesComponents.BarberComponent.Palette, 5 },//
        };

        [RemoteEvent("server.barber.buy")]
        public static void BarberBuy(ExtPlayer player, string name, int id, int color, int colorHighlight, float opacity, bool isDonate = false)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                var accountData = player.GetAccountData();
                if (accountData == null)
                    return;

                if (sessionData.TempBizID == -1 || !BizList.ContainsKey(sessionData.TempBizID)) return;

                Business biz = BizList[sessionData.TempBizID];

                bool gender = characterData.Gender;

                var dictionary = (ClothesComponents.BarberComponent)Enum.Parse(typeof(ClothesComponents.BarberComponent), name);

                if (!ClothesComponents.BarberComponentData[gender].ContainsKey(dictionary))
                    return;
                if (!ClothesComponents.BarberComponentData[gender][dictionary].ContainsKey(id))
                    return;

                var barberData = ClothesComponents.BarberComponentData[gender][dictionary][id];

                Product prod = biz.Products[1];
                var custom = player.GetCustomization();
                if (prod == null || custom == null) return;

                int tempPrice = !isDonate ? barberData.Price : barberData.Donate;
                int price = !isDonate ? Convert.ToInt32((tempPrice / 100f) * prod.Price) : tempPrice;
                if (!isDonate && UpdateData.CanIChange(player, price, true) != 255) return;
                else if (isDonate && accountData.RedBucks < price)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);
                    return;
                }
                if (!isDonate)
                {
                    if (!takeProd(biz.ID, 1, "Расходники", price))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetTovaraNaSkladeBiza), 3000);
                        return;
                    }

                    GameLog.Money($"player({characterData.UUID})", $"biz({biz.ID})", price, $"buyBarber({name})");
                    Wallet.Change(player, -price);
                    biz.BuyItemBusiness(characterData.UUID, "Расходники", price);
                }
                else
                {
                    UpdateData.RedBucks(player, -price, msg: LangFunc.GetText(LangType.Ru, DataName.PremiumClothesBuy));
                }

                switch (dictionary)
                {
                    case ClothesComponents.BarberComponent.Hair:
                        custom.Hair = new HairData(id, color, colorHighlight);
                        ClothesComponents.SetSpecialClothes(player, 2, custom.Hair.Hair, 0);
                        ClothesComponents.UpdateClothes(player);
                        break;
                    case ClothesComponents.BarberComponent.Eyes:
                        custom.EyeColor = id;
                        NAPI.Player.SetPlayerEyeColor(player, (byte)custom.EyeColor);
                        break;
                    default:
                        int slotId = BarberComponentToSlot[dictionary];
                        custom.Appearance[slotId].Value = id;
                        custom.Appearance[slotId].Color = color;
                        custom.Appearance[slotId].Opacity = opacity;

                        HeadOverlay headOverlay = new HeadOverlay();
                        headOverlay.Index = (byte)custom.Appearance[slotId].Value;
                        headOverlay.Opacity = custom.Appearance[slotId].Opacity;
                        headOverlay.Color = (byte)custom.Appearance[slotId].Color;
                        headOverlay.SecondaryColor = 100;
                        NAPI.Player.SetPlayerHeadOverlay(player, slotId, headOverlay);

                        break;
                }
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BarberBuy, price), 5000);
                BattlePass.Repository.UpdateReward(player, 53);
                qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int) zdobich_quests.Stage20, 0, 1,
                    isUpdateHud: true);
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_buyBarber Exception: {e.ToString()}");
            }
        }


        private static Dictionary<ClothesComponents.TattooComponent, int> TattooComponentToSlot = new Dictionary<ClothesComponents.TattooComponent, int>()
        {
            { ClothesComponents.TattooComponent.Torso, 0 },//
            { ClothesComponents.TattooComponent.Head, 1 },//
            { ClothesComponents.TattooComponent.LeftArm, 2 },//
            { ClothesComponents.TattooComponent.RightArm, 3 },//
            { ClothesComponents.TattooComponent.LeftLeg, 4 },//
            { ClothesComponents.TattooComponent.RightLeg, 5 },//
        };

        [RemoteEvent("server.tattoo.buy")]//"tattoo"
        public static void TattooBuy(ExtPlayer player, string name, int id, bool isDonate = false)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                var accountData = player.GetAccountData();
                if (accountData == null)
                    return;

                var custom = player.GetCustomization();
                if (custom == null)
                    return;
                
                if (sessionData.TempBizID == -1 || !BizList.ContainsKey(sessionData.TempBizID)) return;


                var dictionary = (ClothesComponents.TattooComponent)Enum.Parse(typeof(ClothesComponents.TattooComponent), name);

                if (!ClothesComponents.TattooComponentData.ContainsKey(dictionary))
                    return;
                if (!ClothesComponents.TattooComponentData[dictionary].ContainsKey(id)) 
                    return;

                var tattooData = ClothesComponents.TattooComponentData[dictionary][id];
                
                var tattooHash = (characterData.Gender) ? tattooData.MaleHash : tattooData.FemaleHash;

                var customTattoos = custom.Tattoos;

                var newTattoo = new Tattoo(tattooData.Dictionary, tattooHash, tattooData.Slots);

                var countTattoo = 0;

                foreach (var t in customTattoos.Values)
                {
                    countTattoo += t.Count();

                    if (t.Any(c => c.Dictionary == newTattoo.Dictionary && c.Hash == newTattoo.Hash))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ThisTattooIs), 3000);
                        return;
                    }
                }

                if (countTattoo >= 60)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxTattoo), 3000);
                    return;
                }

                var biz = BizList[sessionData.TempBizID];
                var prod = biz.Products[1];
                if (prod == null) return;

                
                int tempPrice = !isDonate ? tattooData.Price : tattooData.Donate;
                int price = !isDonate ? Convert.ToInt32((tempPrice / 100f) * prod.Price) : tempPrice;
                
                if (!isDonate && UpdateData.CanIChange(player, price, true) != 255) return;
                else if (isDonate && accountData.RedBucks < price)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);
                    return;
                }
                if (!isDonate)
                {
                    int amount = Convert.ToInt32(price * 0.75 / 100);
                    if (amount <= 0) amount = 1;
                    if (!takeProd(biz.ID, amount, "Расходники", price))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetTovaraNaSkladeBiza), 3000);
                        return;
                    }

                    GameLog.Money($"player({characterData.UUID})", $"biz({biz.ID})", price, $"buyBarber({name})");
                    Wallet.Change(player, -price);
                    biz.BuyItemBusiness(characterData.UUID, "Расходники", price);
                }
                else
                {
                    UpdateData.RedBucks(player, -price, msg:LangFunc.GetText(LangType.Ru, DataName.PremiumClothesBuy));
                }

                customTattoos[TattooComponentToSlot[dictionary]].Add(newTattoo);

                Trigger.ClientEvent(player, "client.shop.tattoos", JsonConvert.SerializeObject(customTattoos));

                Decoration decoration = new Decoration();
                decoration.Collection = NAPI.Util.GetHashKey(newTattoo.Dictionary);
                decoration.Overlay = NAPI.Util.GetHashKey(newTattoo.Hash);
                player.SetDecoration(decoration);

                player.Eval($"mp.game.audio.playSoundFrontend(-1, \"Tattooing_Oneshot\", \"TATTOOIST_SOUNDS\", true);");
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TattooBuy, tattooData.Name, price), 8000);
                BattlePass.Repository.UpdateReward(player, 18);
                qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int) zdobich_quests.Stage20, 1, 2,
                    isUpdateHud: true);
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_buyTattoo Exception: {e.ToString()}");
            }
        }















        /*public static void OpenClothes(Player player, Business biz, bool isDonate = false, bool isGloves = false, List<int> DellClothes = null, List<int> AddClothes = null)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                else if (!FunctionsAccess.IsWorking("ClothesShop"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }

                if ((sessionData.WorkData.OnDuty && Fractions.Manager.FractionTypes[memberFractionData.Id] == FractionsType.Gov) || sessionData.WorkData.OnWork)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны закончить рабочий день", 3000);
                    return;
                }

                bool gender = characterData.Gender;
                Dictionary<int, List<List<object>>> _ShosessionData = new Dictionary<int, List<List<object>>>();

                List<int> _RealClothes = ClothesShopToType.ToList();
                if (DellClothes != null)
                {
                    foreach (int clothId in DellClothes)
                    {
                        if (_RealClothes.Contains(clothId)) _RealClothes.Remove(clothId);
                    }
                }
                if (AddClothes != null)
                {
                    foreach (int clothId in AddClothes)
                    {
                        if (!_RealClothes.Contains(clothId)) _RealClothes.Add(clothId);
                    }
                }

                foreach (int type in _RealClothes)
                {
                    ConcurrentDictionary<int, Chars.ClothesData> _ShoesData = null;
                    _ShosessionData.Add(type, new List<List<object>>());
                    if (type == (int)ClothesComponent.Masks) _ShoesData = ClothesComponents.ClothesMasksData;
                    else if (type == (int)ClothesComponent.Bugs) _ShoesData = ClothesComponents.ClothesBugsData;
                    else if (type == (int)ClothesComponent.Undershort) _ShoesData = ClothesComponents.ClothesComponentData[gender][ClothesComponent.Tops];
                    else _ShoesData = ClothesComponents.ClothesComponentData[gender][(ClothesComponents.ClothesComponent)type];
                    foreach (KeyValuePair<int, Chars.ClothesData> clData in _ShoesData)
                    {
                        if (type == (int)ClothesComponent.Tops && clData.Value.Type == -1) continue;
                        else if (type == (int)ClothesComponent.Undershort && clData.Value.Type != -1) continue;
                        else if (clData.Value.Gender != -1 && clData.Value.Gender != Convert.ToInt32(gender)) continue;
                        else if (clData.Value.Textures.Count == 0) continue;
                        int GetPrice = !isDonate ? Convert.ToInt32(clData.Value.Price / 100 * biz.Products[0].Price) : clData.Value.Donate;
                        if (GetPrice > 0)
                        {
                            List<object> _ClothesData = new List<object>();
                            _ClothesData.Add(clData.Key);//0
                            _ClothesData.Add(clData.Value.Variation);//1
                            _ClothesData.Add(clData.Value.Torso);//2
                            _ClothesData.Add(clData.Value.TName);//3
                            _ClothesData.Add(clData.Value.Textures);//4
                            _ClothesData.Add(GetPrice);//5
                            _ShosessionData[type].Add(_ClothesData);
                        }
                    }
                }
                Trigger.ClientEvent(player, "openClothes", !isDonate ? biz.Products[0].Price : 0, isDonate, isGloves, JsonConvert.SerializeObject(_ShosessionData));
                Trigger.Dimension(player, Dimensions.RequestPrivateDimension(player.Value));
                NAPI.Entity.SetEntityPosition(player, player.Position + new Vector3(0, 0, 0.2));
                //Trigger.PlayAnimation(player, "amb@world_human_stand_guard@male@base", "base", 39, false);
            }
            catch (Exception e)
            {
                Log.Write($"OpenClothes Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("buyClothes")]
        public static void RemoteEvent_buyClothes(Player player, string type, int variation, int texture, bool isDonate = false)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                if (!isDonate && (sessionData.TempBizID == -1 || !BizList.ContainsKey(sessionData.TempBizID))) return;
                Business biz = !isDonate ? BizList[sessionData.TempBizID] : null;
                Product prod = !isDonate ? biz.Products[0] : null;
                int tempPrice = 0;
                int SlotId = -1;
                
                bool gender = characterData.Gender;
                string Data = $"{variation}_{texture}_{gender}";
                ConcurrentDictionary<int, Chars.ClothesData> ShoesData = null;
                switch (type)
                {
                    case "Hat":
                        SlotId = 0;
                        ShoesData = ClothesComponents.ClothesComponentData[gender][ClothesComponent.Hat];
                        break;
                    case "Tops":
                        SlotId = 5;
                        ShoesData = ClothesComponents.ClothesComponentData[gender][ClothesComponent.Tops];
                        break;
                    case "Undershort":
                        SlotId = 6;
                        ShoesData = ClothesComponents.ClothesComponentData[gender][ClothesComponent.Tops];
                        break;
                    case "Legs":
                        SlotId = 9;
                        ShoesData = ClothesComponents.ClothesComponentData[gender][ClothesComponent.Legs];
                        break;
                    case "Shoes":
                        SlotId = 13;
                        ShoesData = ClothesComponents.ClothesComponentData[gender][ClothesComponent.Shoes];
                        break;
                    case "Gloves":
                        SlotId = 12;
                        tempPrice = Customization.Gloves[gender].FirstOrDefault(f => f.Variation == variation).Price;
                        break;
                    case "Watches":
                        SlotId = 11;
                        ShoesData = ClothesComponents.ClothesComponentData[gender][ClothesComponent.Watches];
                        break;
                    case "Glasses":
                        SlotId = 3;
                        ShoesData = ClothesComponents.ClothesComponentData[gender][ClothesComponent.Glasses];
                        break;
                    case "Accessories":
                        SlotId = 4;
                        ShoesData = ClothesComponents.ClothesComponentData[gender][ClothesComponent.Accessories];
                        break;
                    case "Ears":
                        SlotId = 2;
                        ShoesData = ClothesComponents.ClothesComponentData[gender][ClothesComponent.Ears];
                        break;
                    case "Mask":
                        SlotId = 1;
                        ShoesData = ClothesComponents.ClothesMasksData;
                        break;
                    case "Bug":
                        SlotId = 8;
                        ShoesData = ClothesComponents.ClothesBugsData;
                        break;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
                if (tempPrice == 0 && ShoesData != null)
                {
                    if (!ShoesData.ContainsKey(variation)) return;
                    tempPrice = !isDonate ? ShoesData[variation].Price : ShoesData[variation].Donate;
                }
                int price = !isDonate ? Convert.ToInt32((tempPrice / 100f) * prod.Price) : tempPrice;
                if (!isDonate && UpdateData.CanIChange(player, price, true) != 255) return;
                else if (isDonate && accountData.RedBucks < price)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);
                    return;
                }
                int amount = Convert.ToInt32(price * 0.75 / 100);
                if (amount <= 0) amount = 1;

                if (SlotId == -1 || Chars.Repository.isFreeSlots(player, Chars.Repository.AccessoriesInfo[SlotId]) != 0) return;
                if (!isDonate) 
                {
                    if(type == "Mask" && !takeProd(biz.ID, (price >= 15000 ? 6 : 1), "Маски", price))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно товара на складе", 3000);
                        return;
                    }
                    else if (!takeProd(biz.ID, amount, "Одежда", price))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно товара на складе", 3000);
                        return;
                    }
                }
                Chars.Repository.ChangeAccessoriesItem(player, SlotId, Data);
                if (!isDonate)
                {
                    GameLog.Money($"player({characterData.UUID})", $"biz({biz.ID})", price, $"buyClothes({type})");
                    Wallet.Change(player, -price);
                    biz.Pribil += (uint)price;
                }
                else
                {
                    GameLog.AccountLog(accountData.Login, accountData.HWID, accountData.IP, accountData.SocialClub, $"Куплена премиум одежда (-{price} RedBucks)");
                    UpdateData.RedBucks(player, -price);
                }

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы купили новую одежду. Она была добавлена в Ваш инвентарь.", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_buyClothes Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("cancelClothes")]
        public static void RemoteEvent_cancelClothes(Player player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                sessionData.TempBizID = -1;
                Trigger.StopAnimation(player);
                Customization.ApplyCharacter(player);
                Trigger.Dimension(player, 0);
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_cancelClothes Exception: {e.ToString()}");
            }
        }*/

        [RemoteEvent("cancelBody")]
        public static void RemoteEvent_cancelTattoo(ExtPlayer player) // BARBER EXIT
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                Trigger.Dimension(player);
                Trigger.StopAnimation(player);
                characterData.ExteriorPos = new Vector3();
                sessionData.TempBizID = -1;
                Customization.ApplyCharacter(player);
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_cancelTattoo Exception: {e.ToString()}");
            }
        }

        /*public static IReadOnlyDictionary<string, List<int>> BarberPrices = new Dictionary<string, List<int>>()
        {
            { "hair", new List<int>() {
                4000,3500,3500,4500,4500,6000,4500,11000,4500,6000,6000,4000,3500,15000,7500,10000,4500,6000,6000,4000,3500,20000,7500,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,15000,10000,10000,10000,15000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000
            }},
            { "beard", new List<int>() {
                1200,1200,1200,1200,1200,1600,1600,1600,1200,1200,2400,2400,1200,1200,2400,2000,1200,1600,3800,3600,3600,1800,1800,2600,1200,1200,2400,2000,1200,1600,3800,3600,3600,1800,1800,2600,1200,1800,1800
            }},
            { "eyebrows", new List<int>() {
                1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,100
            }},
            { "chesthair", new List<int>() {
                1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000
            }},
            { "lenses", new List<int>() {
                2000,4000,4000,2000,2000,4000,2000,4000,10000,1000
            }},
            { "lipstick", new List<int>() {
                2000,4000,4000,2000,2000,4000,2000,4000,10000,3000
            }},
            { "blush", new List<int>() {
                2000,4000,4000,2000,2000,4000,2000
            }},
            { "makeup", new List<int>() {
                1200,1200,1200,1200,1200,1600,1600,1600,1200,1200,2400,2400,1200,1200,2400,2000,1200,1600,3800,3600,3600,1800,1800,2600,1200,1200,2400,2000,1200,1600,3800,3600,3600,1800,1800,2600,1200,1800,1800
            }},
        };

        [RemoteEvent("server.barber.buy")]
        public static void RemoteEvent_buyBarber(Player player, string id, int style, int color)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (sessionData.TempBizID == -1 || !BizList.ContainsKey(sessionData.TempBizID)) return;
                Business biz = BizList[sessionData.TempBizID];
                if ((id == "lipstick" || id == "blush" || id == "makeup") && characterData.Gender && style != 255)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Доступно только для персонажей женского пола", 3000);
                    return;
                }
                Product prod = biz.Products.FirstOrDefault(p => p.Name == "Парики");
                if (prod == null || !BarberPrices.ContainsKey(id) || !Customization.CustomPlayerData.ContainsKey(player)) return;
                double price;
                if (id == "hair" && style >= 23) price = BarberPrices[id][23] / 100f * prod.Price;
                else
                {
                    if (style != 255 && style >= BarberPrices[id].Count) return;
                    price = (style == 255) ? BarberPrices[id][0] / 100f * prod.Price : BarberPrices[id][style] / 100f * prod.Price;
                }
                int totalprice = Convert.ToInt32(price);
                if (UpdateData.CanIChange(player, totalprice, true) != 255) return;
                if (!takeProd(biz.ID, 1, "Расходники", totalprice))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Этот барбер-шоп не может оказать эту услугу в данный момент", 3000);
                    return;
                }
                GameLog.Money($"player({characterData.UUID})", $"biz({biz.ID})", totalprice, "buyBarber");
                MoneySystem.Wallet.Change(player, -totalprice);
                biz.Pribil += (uint)totalprice;
                PlayerCustomization custom = Customization.CustomPlayerData[player];
                switch (id)
                {
                    case "hair":
                        custom.Hair = new HairData(style, color, color);
                        break;
                    case "beard":
                        custom.Appearance[1].Value = style;
                        custom.Appearance[1].Opacity = 1f;
                        custom.BeardColor = color;
                        break;
                    case "eyebrows":
                        custom.Appearance[2].Value = style;
                        custom.Appearance[2].Opacity = 1f;
                        custom.EyebrowColor = color;
                        break;
                    case "chesthair":
                        custom.Appearance[10].Value = style;
                        custom.Appearance[10].Opacity = 1f;
                        custom.ChestHairColor = color;
                        break;
                    case "lenses":
                        custom.EyeColor = style;
                        break;
                    case "lipstick":
                        custom.Appearance[8].Value = style;
                        custom.Appearance[8].Opacity = 1f;
                        custom.LipstickColor = color;
                        break;
                    case "blush":
                        custom.Appearance[5].Value = style;
                        custom.Appearance[5].Opacity = 1f;
                        custom.BlushColor = color;
                        break;
                    case "makeup":
                        custom.Appearance[4].Value = style;
                        custom.Appearance[4].Opacity = 1f;
                        break;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы оплатили услугу Барбер-Шопа ({totalprice}$)", 3000);

                Trigger.ClientEvent(player, "client.barber.startAnim", id);
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_buyBarber Exception: {e.ToString()}");
            }
        }*/

        [RemoteEvent("petrol")]
        public static void fillCar(ExtPlayer player, int lvl)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (sessionData.BizID == -1 || !BizList.ContainsKey(sessionData.BizID) || !player.IsInVehicle) return;
                var vehicle = (ExtVehicle) player.Vehicle;
                if (vehicle == null) return;
                if (player.VehicleSeat != (int)VehicleSeat.Driver) return;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if (lvl <= 0)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                        return;
                    }
                    if (vehicleLocalData.Petrol <= -1)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantZapravit), 3000);
                        return;
                    }
                    if (VehicleStreaming.GetEngineState(vehicle))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Zaglushite), 3000);
                        return;
                    }
                    int fuel = vehicleLocalData.Petrol;
                    if (fuel >= VehicleManager.VehicleTank[vehicle.Class])
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FullFuel), 3000);
                        return;
                    }

                    bool isGov = false;
                    if (lvl == 9999) lvl = VehicleManager.VehicleTank[vehicle.Class] - fuel;
                    else if (lvl == 99999)
                    {
                        isGov = true;
                        lvl = VehicleManager.VehicleTank[vehicle.Class] - fuel;
                    }

                    if (lvl < 0) return;

                    int tfuel = fuel + lvl;
                    if (tfuel > VehicleManager.VehicleTank[vehicle.Class])
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                        return;
                    }
                    var fractionData = player.GetFractionData();
                    Business biz = BizList[sessionData.BizID];
                    int price = lvl * biz.Products[0].Price;
                    if (isGov)
                    {
                        if (fractionData == null || Fractions.Manager.FractionTypes[fractionData.Id] != FractionsType.Gov)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GosZapravit), 6000);
                            return;
                        }
                        if (vehicleLocalData.Access != VehicleAccess.Fraction || vehicleLocalData.Fraction != fractionData.Id)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GosGosZapravit), 6000);
                            return;
                        }
                        if (lvl > fractionData.FuelLeft) 
                            lvl = fractionData.FuelLeft;
                        //if (lvl <= 0 || Fractions.Stocks.fracStocks[frac].FuelLeft < lvl * biz.Products[0].Price)
                        //{
                        //    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Лимит на заправку гос. транспорта за день исчерпан", 3000);
                        //    return;
                        //}
                    }
                    else if (UpdateData.CanIChange(player, price, true) != 255) return;
                    if (!takeProd(biz.ID, lvl, "Бензин", price))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ZapravkaIs, biz.Products[0].Lefts), 7000);
                        return;
                    }
                    biz.BuyItemBusiness(characterData.UUID, "Бензин", price);
                    if (isGov)
                    {
                        GameLog.Money($"frac(6)", $"biz({biz.ID})", price, "buyPetrol");
                        
                        if ((fractionData.FuelLeft - price) >= 0)
                            fractionData.FuelLeft -= price;
                        
                        fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.CITY);
                        if (fractionData != null)
                            fractionData.Money -= price;

                        //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ZapravkaGos), 3000);
                        Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Bank, LangFunc.GetText(LangType.Ru, DataName.ZapravkaGos), DateTime.Now); 
                    }
                    else
                    {
                        GameLog.Money($"player({characterData.UUID})", $"biz({biz.ID})", price, "buyPetrol");
                        MoneySystem.Wallet.Change(player, -price);
                        //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ZapravkaSuccess, price), 6000);
                        Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Bank, LangFunc.GetText(LangType.Ru, DataName.ZapravkaSuccess, price), DateTime.Now); 
                        BattlePass.Repository.UpdateReward(player, 40);
                        if (vehicleLocalData.Access == VehicleAccess.Fraction)
                            player.AddTableScore(TableTaskId.Item1);
                        if (fuel >= 20)
                            BattlePass.Repository.UpdateReward(player, 19);
                    }

                    vehicleLocalData.Petrol = tfuel;
                    vehicle.SetSharedData("PETROL", tfuel);
                    
                    //qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage8, 0, 1, isUpdateHud: true);
                    
                    if (vehicleLocalData.Access == VehicleAccess.Personal)
                    {
                        string number = NAPI.Vehicle.GetVehicleNumberPlate(vehicle);
                        var vehicleData = VehicleManager.GetVehicleToNumber(number);
                        if (vehicleData != null) vehicleData.Fuel = tfuel;
                    }
                    Commands.RPChat("sme", player, $"заправил" + (characterData.Gender ? "" : "а") + LangFunc.GetText(LangType.Ru, DataName.veh));
                }
            }
            catch (Exception e)
            {
                Log.Write($"fillCar Exception: {e.ToString()}");
            }
        }
        public static void buyBusinessCommand(ExtPlayer player)
        {
            try
            {
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (player.IsInVehicle) return;
                if (characterData.DemorganTime >= 1) return;
                int id = CustomColShape.GetDataToEnum(player, ColShapeEnums.BusinessAction);
                if (id == (int)ColShapeData.Error)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NeedBeNearBiz), 3000);
                    return;
                }
                var biz = BusinessManager.BizList[id];
                if (!Main.PlayerUUIDs.ContainsKey(player.Name)) return;
                if (characterData.BizIDs.Count >= Group.GroupMaxBusinesses[accountData.VipLvl])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantGetMoreThanBiz, Group.GroupMaxBusinesses[accountData.VipLvl]), 3000);
                    return;
                }
                if (biz.IsAuction)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BizAuc), 3000);
                    return;
                }

                if (Players.Phone.Auction.Repository.IsBet(characterData.UUID, AuctionType.Biz))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouBizAuc), 6000);
                    return;
                }
                if (biz.Owner.Equals(player.Name))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BizYours), 3000);
                    return;
                }
                if (biz.IsOwner())
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BizOtherPlayer), 3000);
                    return;
                }
                if (UpdateData.CanIChange(player, biz.SellPrice, true) != 255) return;
                if (Main.ServerNumber != 0 && (characterData.AdminLVL >= 1 && characterData.AdminLVL <= 6))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AdminTransferRestricted), 3000);
                    return;
                }
                Wallet.Change(player, -biz.SellPrice);
                GameLog.Money($"player({characterData.UUID})", $"server", biz.SellPrice, $"buyBiz({biz.ID})");
                //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BizBuy, BusinessManager.BusinessTypeNames[biz.Type]), 3000);
                Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Bank, LangFunc.GetText(LangType.Ru, DataName.BizBuy, BusinessManager.BusinessTypeNames[biz.Type], biz.SellPrice), DateTime.Now);
                Trigger.ClientEvent(player, "client.rieltagency.delBlip", 464, biz.ID);

                foreach (var product in biz.Products) 
                    product.Lefts = 0;
                
                var orders = new List<Order>();
                
                foreach (var order in biz.Orders)
                {
                    if (order.Taked) 
                        orders.Add(order);
                    else 
                        Orders.TryRemove(order.UID, out _);
                }
                
                biz.Orders = orders;
                biz.SetOwner (player.Name);
                characterData.BizIDs.Add(id);
                
                int tax = Convert.ToInt32(biz.SellPrice / 100f * biz.Tax);
                var bizBalance = Bank.Accounts[biz.BankID];
                bizBalance.Balance = tax * 2;
                bizBalance.IsSave = true;
                
                Character.Save.Repository.SaveBiz(player);
            }
            catch (Exception e)
            {
                Log.Write($"buyBusinessCommand Exception: {e.ToString()}");
            }
        }

        public static async void createBusinessCommand(ExtPlayer player, int govPrice, int type, double taxes)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Createbusiness)) return;
                if (taxes < 0.001 || taxes > 0.999)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NalogLimit), 3000);
                    return;
                }
                Vector3 pos = player.Position;
                pos.Z -= 1.12F;
                List<Product> products_list = BusinessManager.fillProductList(type);
                string productlist = JsonConvert.SerializeObject(products_list);
                lastBizID++;

                int bankID = await Bank.Create("", 3, 1000);

                using MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "INSERT INTO businesses (id, owner, sellprice, type, products, enterpoint, unloadpoint, money, mafia, orders, tax) VALUES (@val0,@val1,@val2,@val3,@val4,@val5,@val6,@val7,@val8,@val9,@val10)"
                };
                cmd.Parameters.AddWithValue("@val0", lastBizID);
                cmd.Parameters.AddWithValue("@val1", "Государство");
                cmd.Parameters.AddWithValue("@val2", govPrice);
                cmd.Parameters.AddWithValue("@val3", type);
                cmd.Parameters.AddWithValue("@val4", productlist);
                cmd.Parameters.AddWithValue("@val5", JsonConvert.SerializeObject(pos));
                cmd.Parameters.AddWithValue("@val6", JsonConvert.SerializeObject(new Vector3()));
                cmd.Parameters.AddWithValue("@val7", bankID);
                cmd.Parameters.AddWithValue("@val8", -1);
                cmd.Parameters.AddWithValue("@val9", JsonConvert.SerializeObject(new List<Order>()));
                cmd.Parameters.AddWithValue("@val10", taxes);
                MySQL.Query(cmd);

                NAPI.Task.Run(() =>
                {
                    Business biz = new Business(lastBizID, "Государство", govPrice, type, products_list, pos, new Vector3(), bankID, -1, new List<Order>(), taxes);
                    biz.UpdateLabel();
                    BizList.TryAdd(lastBizID, biz);

                    if (type == 6)
                    {
                        using MySqlCommand cmd1 = new MySqlCommand
                        {
                            CommandText = "INSERT INTO `weapons`(`id`,`lastserial`) VALUES(@val0,@val1)"
                        };
                        cmd1.Parameters.AddWithValue("@val0", biz.ID);
                        cmd1.Parameters.AddWithValue("@val1", 0);
                        MySQL.Query(cmd1);
                    }

                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BizCreated, biz.ID), 3000);
                });
            }
            catch (Exception e)
            {
                Log.Write($"createBusinessCommand Exception: {e.ToString()}");
            }
        }

        public static void createBusinessUnloadpoint(ExtPlayer player, int bizid)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Createunloadpoint)) return;
                Vector3 pos = player.Position;
                BizList[bizid].UnloadPoint = pos;

                using MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "UPDATE businesses SET unloadpoint=@val0 WHERE id=@val1"
                };
                cmd.Parameters.AddWithValue("@val0", JsonConvert.SerializeObject(pos));
                cmd.Parameters.AddWithValue("@val1", bizid);
                MySQL.Query(cmd);

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PointUnloadBuy, bizid), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"createBusinessUnloadpoint Exception: {e.ToString()}");
            }
        }
        public static void changeBizTax(ExtPlayer player, int id, double taxes)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Changebiztax)) return;

                if (taxes < 0.001 || taxes > 0.999)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NalogLimit), 3000);
                    return;
                }
                Business biz = BusinessManager.BizList.FirstOrDefault(b => b.Value.ID == id).Value;

                if (biz != null)
                {
                    using MySqlCommand cmd = new MySqlCommand
                    {
                        CommandText = "UPDATE businesses SET tax=@val0 WHERE id=@val1"
                    };
                    cmd.Parameters.AddWithValue("@val0", taxes);
                    cmd.Parameters.AddWithValue("@val1", id);
                    MySQL.Query(cmd);

                    ExtPlayer owner = (ExtPlayer) NAPI.Player.GetPlayerFromName(biz.Owner);
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NalogChasIzmenen, id, taxes * 100), 3000);
                    if (owner != null)
                    {
                        Notify.Send(owner, NotifyType.Alert, NotifyPosition.Center, LangFunc.GetText(LangType.Ru, DataName.NalogIzmenen, biz.Tax * 100, taxes * 100), 10000);
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VladeletNotify, owner.Name, owner.Value), 5000);
                    }
                    biz.Tax = taxes;
                }
            }
            catch (Exception e)
            {
                Log.Write($"changeBizTax Exception: {e.ToString()}");
            }
        }

        public static void deleteBusinessCommand(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Deletebusiness)) return;
                Business biz = BizList.FirstOrDefault(b => b.Value.ID == id).Value;
                if (biz == null)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BizIdNotFound), 3000);
                    return;
                }

                using MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "DELETE FROM businesses WHERE id=@val0"
                };
                cmd.Parameters.AddWithValue("@val0", id);
                MySQL.Query(cmd);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BizDeleted), 3000);
                if (biz.Type == 6)
                {
                    using MySqlCommand cmd1 = new MySqlCommand
                    {
                        CommandText = "DELETE FROM `weapons` WHERE id=@val0"
                    };
                    cmd1.Parameters.AddWithValue("@val0", id);
                    MySQL.Query(cmd1);
                }

                ExtPlayer owner = (ExtPlayer) NAPI.Player.GetPlayerFromName(biz.Owner); 
                var ownerCharacterData = owner.GetCharacterData();
                if (ownerCharacterData == null)
                {
                    string[] split = biz.Owner.Split('_');

                    using MySqlCommand cmd1 = new MySqlCommand
                    {
                        CommandText = "SELECT biz FROM characters WHERE firstname=@val0 AND lastname=@val1"
                    };
                    cmd1.Parameters.AddWithValue("@val0", split[0]);
                    cmd1.Parameters.AddWithValue("@val1", split[1]);

                    using DataTable data = MySQL.QueryRead(cmd1);
                    List<int> ownerBizs = new List<int>();

                    ownerBizs = JsonConvert.DeserializeObject<List<int>>(data.Rows[0]["biz"].ToString());
                    ownerBizs.Remove(biz.ID);

                    using MySqlCommand updateCmd = new MySqlCommand
                    {
                        CommandText = "UPDATE characters SET biz=@val0 WHERE firstname=@val1 AND lastname=@val2"
                    };
                    updateCmd.Parameters.AddWithValue("@val0", JsonConvert.SerializeObject(ownerBizs));
                    updateCmd.Parameters.AddWithValue("@val1", split[0]);
                    updateCmd.Parameters.AddWithValue("@val2", split[1]);
                    MySQL.Query(updateCmd);
                }
                else
                {
                    ownerCharacterData.BizIDs.Remove(id);
                    Wallet.Change(owner, biz.SellPrice);
                }
                biz.Destroy();
                BizList.TryRemove(biz.ID, out _);
            }
            catch (Exception e)
            {
                Log.Write($"deleteBusinessCommand Exception: {e.ToString()}");
            }
        }

        public static void sellBusinessCommand(ExtPlayer player, ExtPlayer target, int price)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                SellItemData sellItemData = sessionData.SellItemData;
                if ((sellItemData.Buyer != null || sellItemData.Seller != null) && Chars.Repository.TradeGet(player))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantTrade), 3000);
                    return;
                }
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                var targetAccountData = target.GetAccountData();
                if (targetAccountData == null) return;
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) return;
                SellItemData targetSellItemData = targetSessionData.SellItemData;
                if ((targetSellItemData.Buyer != null || targetSellItemData.Seller != null) && Chars.Repository.TradeGet(target))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PersonCantTrade), 3000);
                    return;
                }
                if (player.Position.DistanceTo(target.Position) > 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                    return;
                }
                if (targetCharacterData.DemorganTime >= 1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PersonCantTrade), 3000);
                    return;
                }
                if (characterData.BizIDs.Count == 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoBusiness), 3000);
                    return;
                }
                if (targetCharacterData.BizIDs.Count >= Group.GroupMaxBusinesses[targetAccountData.VipLvl])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BizMaxPlayer), 3000);
                    return;
                }
                Business biz = BizList[characterData.BizIDs[0]];
                if (price < biz.SellPrice)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BizCantSellPrice, biz.SellPrice), 3000);
                    return;
                }
                if (!biz.Owner.Equals(player.Name))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BizNotYour), 3000);
                    return;
                }
                if (targetCharacterData.Money < price)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerNotEnoughMoney), 3000);
                    return;
                }
                sellItemData.Seller = player;
                sellItemData.Price = price;
                sellItemData.Count = biz.ID;
                targetSellItemData.Seller = player;
                targetSellItemData.Price = price;
                targetSellItemData.Count = biz.ID;
                Trigger.ClientEvent(target, "openDialog", "BUSINESS_BUY", LangFunc.GetText(LangType.Ru, DataName.YouWantSellBiz, player.Name, BusinessTypeNames[biz.Type], MoneySystem.Wallet.Format(price)));
                EventSys.SendCoolMsg(player,"Предложение", "Покупка", $"{LangFunc.GetText(LangType.Ru, DataName.BizWantSellToYou, target.Value, MoneySystem.Wallet.Format(price))}", "", 10000);
                //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BizWantSellToYou, target.Value, MoneySystem.Wallet.Format(price)), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"sellBusinessCommand Exception: {e.ToString()}");
            }
        }

        public static void acceptBuyBusiness(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                SellItemData sellItemData = sessionData.SellItemData;
                ExtPlayer target = sellItemData.Seller;
                var targetSessionData = target.GetSessionData();
                var targetCharacterData = target.GetCharacterData();
                if (targetSessionData == null || targetCharacterData == null)
                {
                    sessionData.SellItemData = new SellItemData();
                    return;
                }
                int price = sellItemData.Price;
                Business biz = BizList[sellItemData.Count];
                if (player.Position.DistanceTo(target.Position) > 2)
                {
                    sessionData.SellItemData = new SellItemData();
                    targetSessionData.SellItemData = new SellItemData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                    return;
                }

                if (UpdateData.CanIChange(player, price, true) != 255)
                {
                    sessionData.SellItemData = new SellItemData();
                    targetSessionData.SellItemData = new SellItemData();
                    return;
                }
                else if (Main.ServerNumber != 0 && (characterData.AdminLVL >= 1 && characterData.AdminLVL <= 6))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AdminTransferRestricted), 3000);
                    return;
                }

                if (!targetCharacterData.BizIDs.Contains(biz.ID))
                {
                    sessionData.SellItemData = new SellItemData();
                    targetSessionData.SellItemData = new SellItemData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BizNoBelongsTo), 3000);
                    return;
                }
                if (!Main.PlayerUUIDs.ContainsKey(target.Name))
                {
                    sessionData.SellItemData = new SellItemData();
                    targetSessionData.SellItemData = new SellItemData();
                    return;
                }
                if (!biz.Owner.Equals(target.Name))
                {
                    sessionData.SellItemData = new SellItemData();
                    targetSessionData.SellItemData = new SellItemData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BizNoBelongsTo), 3000);
                    return;
                }
                if (characterData.BizIDs.Count >= Group.GroupMaxBusinesses[accountData.VipLvl])
                {
                    sessionData.SellItemData = new SellItemData();
                    targetSessionData.SellItemData = new SellItemData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxBizAmount), 3000);
                    return;
                }
                
                characterData.BizIDs.Add(biz.ID);
                Character.Save.Repository.SaveBiz(player);
                
                targetCharacterData.BizIDs.Remove(biz.ID);
                Character.Save.Repository.SaveBiz(target);
                
                biz.SetOwner(player.Name);
                
                Wallet.Change(player, -price);
                Wallet.Change(target, price);
                GameLog.Money($"player({characterData.UUID})", $"player({targetCharacterData.UUID})", price, $"buyBiz({biz.ID})");
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы купили у {target.Name.Replace('_', ' ')} {BusinessTypeNames[biz.Type]} за {MoneySystem.Wallet.Format(price)}$", 3000);
                Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"{player.Name.Replace('_', ' ')} купил у Вас {BusinessTypeNames[biz.Type]} за {MoneySystem.Wallet.Format(price)}$", 3000);
                //Chars.Repository.PlayerStats(player);
                //Chars.Repository.PlayerStats(seller);
                sessionData.SellItemData = new SellItemData();
                targetSessionData.SellItemData = new SellItemData();
            }
            catch (Exception e)
            {
                Log.Write($"acceptBuyBusiness Exception: {e.ToString()}");
            }
        }

        #region Menus

        public static void OpenBizShopMenu(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (sessionData.BizID == -1 || !BizList.ContainsKey(sessionData.BizID)) return;
                var biz = BizList[sessionData.BizID];
                var jsonData = new List<Fractions.Manager.FracMatsData>();
                int index = 0;
                foreach (var p in biz.Products)
                {
                    var busProductData = BusProductsData[p.Name];
                    if (busProductData.ItemId != ItemId.Debug) jsonData.Add(new Fractions.Manager.FracMatsData(index, Chars.Repository.ItemsInfo[busProductData.ItemId].Name, Chars.Repository.ItemsInfo[busProductData.ItemId].Icon, $"{p.Price}$", (int)busProductData.ItemId));
                    else if (p.Name == "Сим-карта") jsonData.Add(new Fractions.Manager.FracMatsData(index, "Сим-карта", "sm-icon-sim", $"{p.Price}$"));
                    else if (p.Name == "Лотерейный билет") jsonData.Add(new Fractions.Manager.FracMatsData(index, "Лотерейный билет", "sm-icon-lotary", $"{p.Price}$"));
                    index++;
                }
                Trigger.ClientEvent(player, "client.sm.openShop", JsonConvert.SerializeObject(jsonData));
            }
            catch (Exception e)
            {
                Log.Write($"OpenBizShopMenu Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.sm.shop")]
        public static void Event_ShopCallback(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (sessionData.BizID == -1 || !BizList.ContainsKey(sessionData.BizID)) return;
                var biz = BizList[sessionData.BizID];
                var prod = biz.Products[index];
                
                if (UpdateData.CanIChange(player, prod.Price, true) != 255) return;
                
                if (prod.Name == "Сим-карта")
                {
                    if (Chars.Repository.isFreeSlots(player, ItemId.SimCard) != 0) return;
                    
                    if (!takeProd(biz.ID, 1, prod.Name, prod.Price))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetTovaraNaSkladeBiza), 3000);
                        return;
                    }

                    var number = Players.Phone.Sim.Repository.GenerateSimCard();
                    
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.SimCard, 1, number.ToString());

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили сим-карту с номером {number}. Она добавлена вам в инвентарь", 3000);
                    //Chars.Repository.PlayerStats(player);
                    biz.BuyItemBusiness(characterData.UUID, "Сим-карта", prod.Price);
                    Wallet.Change(player, -prod.Price);
                    GameLog.Money($"player({characterData.UUID})", $"biz({biz.ID})", prod.Price, $"buyShopSim({number})");
                
                }
                else if (prod.Name == "Лотерейный билет")
                {

                    uint mynumb = (uint)Lottery.LotteryBought.Count() + 1;
                    if (!Lottery.LotteryBought.ContainsKey(mynumb))
                    {
                        if (!takeProd(biz.ID, 1, prod.Name, prod.Price))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetTovaraNaSkladeBiza), 3000);
                            return;
                        }
                        Lottery.LotteryBought.Add(mynumb, characterData.UUID);
                        Lottery.Price = 350 * mynumb;
                        switch (mynumb)
                        {
                            case 51:
                            case 101:
                            case 201:
                            case 501:
                            case 1001:
                            case 1501:
                            case 2001:
                            case 2501:
                            case 3001:
                            case 5001:
                                NAPI.Chat.SendChatMessageToAll($"~g~[ЛОТЕРЕЯ] Продано более {mynumb - 1} лотерейных билетов. Сумма джекпота: {Wallet.Format(Lottery.Price)}$!");
                                NAPI.Chat.SendChatMessageToAll($"~g~[ЛОТЕРЕЯ] Билеты продаются во всех магазинах 24/7.");
                                break;
                            default:
                                break;
                        }

                        biz.BuyItemBusiness(characterData.UUID, "Лотерейный билет", 500);
                        Wallet.Change(player, -500);
                        GameLog.Money($"player({characterData.UUID})", $"biz({biz.ID})", prod.Price, $"buyLottery({Lottery.ID} | {mynumb})");
                        BattlePass.Repository.UpdateReward(player, 6);
                        
                        Trigger.SetTask(async () =>
                        {
                            try
                            {
                                await using var db = new ServerBD("MainDB");//В отдельном потоке

                                await db.InsertAsync(new LotteryPlayers
                                {
                                    Number = Lottery.ID,
                                    Ticket = mynumb,
                                    Player = characterData.UUID
                                });
                            }
                            catch (Exception e)
                            {
                                Debugs.Repository.Exception(e);
                            }
                        });
                        
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили лотерейный билет #{mynumb}. Дополнительная информация: /lottery", 7000);
                        qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage29, 1, isUpdateHud: true);
                        qMain.UpdateQuestsComplete(player, Zdobich.QuestName, (int) zdobich_quests.Stage29, true);
                    }
                }
                else
                {
                    var busProductData = BusProductsData[prod.Name];
                    if (busProductData.ItemId != ItemId.Debug)
                    {
                        /*if ((ItemId)type == ItemId.KeyRing && Chars.Repository.isItem(player, $"char_{characterData.UUID}", "inventory", ItemId.KeyRing) != null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У Вас уже есть связка ключей", 3000);
                            return;
                        }
                        else */
                        if (Chars.Repository.isFreeSlots(player, busProductData.ItemId) != 0) return;
                        if (!takeProd(biz.ID, 1, prod.Name, prod.Price))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetTovaraNaSkladeBiza), 3000);
                            return;
                        }

                        if (ItemId.Beer == busProductData.ItemId)
                        {
                            qMain.UpdateQuestsStage(player, "npc_granny", (int) granny_quests.Beer, 1,
                                isUpdateHud: true);

                            BattlePass.Repository.UpdateReward(player, 29);
                        }

                        //
                        
                        if (ItemId.Pickaxe1 == busProductData.ItemId) Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", busProductData.ItemId, 1, "300");
                        else if (ItemId.Pickaxe2 == busProductData.ItemId) Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", busProductData.ItemId, 1, "1248");
                        else if (ItemId.Pickaxe3 == busProductData.ItemId) Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", busProductData.ItemId, 1, "2250");
                        else if (ItemId.WorkAxe == busProductData.ItemId) Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", busProductData.ItemId, 1, "1250");
                        else if (ItemId.Bong == busProductData.ItemId) Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", busProductData.ItemId, 1, "420");
                        else if (ItemId.Vape == busProductData.ItemId) Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", busProductData.ItemId, 1, "100");
                        else if (ItemId.Hookah == busProductData.ItemId) Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", busProductData.ItemId, 1, "1000");
                        else Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", busProductData.ItemId, 1, busProductData.ItemId == ItemId.Bag ? "40_0_true" : "");

                        var itemInfo = Chars.Repository.ItemsInfo[busProductData.ItemId];
                        
                        EventSys.SendCoolMsg(player,"Покупка", "Покупка предмета", $"{LangFunc.GetText(LangType.Ru, DataName.YouBuy, itemInfo.Name, prod.Price)}", "", 6000); 
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"{LangFunc.GetText(LangType.Ru, DataName.YouBuy, itemInfo.Name, prod.Price)}", 1000);

                        if (itemInfo.functionType == newItemType.Eat/* && biz.Type == 8*/)
                        {
                            qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage21, 1, isUpdateHud: true);
                            qMain.UpdateQuestsComplete(player, Zdobich.QuestName, (int) zdobich_quests.Stage21, true);
                        }
                        
                        if (ItemId.GasCan == busProductData.ItemId)
                            qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage8, 0, 1, isUpdateHud: true);
                        
                        if (ItemId.Wrench == busProductData.ItemId)
                            qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage8, 1, 2, isUpdateHud: true);
                        
                        if (ItemId.KeyRing == busProductData.ItemId)
                            BattlePass.Repository.UpdateReward(player, 45);
                        
                        if (ItemId.Flashlight == busProductData.ItemId)
                            BattlePass.Repository.UpdateReward(player, 104);
                        
                        if (ItemId.Wrench == busProductData.ItemId)
                            BattlePass.Repository.UpdateReward(player, 105);
                        
                        if (ItemId.Hammer == busProductData.ItemId)
                            BattlePass.Repository.UpdateReward(player, 106);
                        
                        if (ItemId.Crisps == busProductData.ItemId)
                            BattlePass.Repository.UpdateReward(player, 107);
                        
                        if (ItemId.Pizza == busProductData.ItemId)
                            BattlePass.Repository.UpdateReward(player, 108);
                        
                        if (ItemId.GasCan == busProductData.ItemId)
                            BattlePass.Repository.UpdateReward(player, 109);
                        
                        if (ItemId.LoveNote == busProductData.ItemId)
                            BattlePass.Repository.UpdateReward(player, 110);
                            
                        if (ItemId.Binoculars == busProductData.ItemId)
                            BattlePass.Repository.UpdateReward(player, 111);
                        
                        if (ItemId.Vape == busProductData.ItemId)
                            BattlePass.Repository.UpdateReward(player, 112);
                        
                        if (ItemId.Umbrella == busProductData.ItemId)
                            BattlePass.Repository.UpdateReward(player, 113);
                        
                        if (ItemId.Guitar == busProductData.ItemId)
                            BattlePass.Repository.UpdateReward(player, 114);
                        
                        if (ItemId.Camera == busProductData.ItemId)
                            BattlePass.Repository.UpdateReward(player, 115);
                        
                        if (ItemId.Microphone == busProductData.ItemId)
                            BattlePass.Repository.UpdateReward(player, 116);

                        if (ItemId.Ball == busProductData.ItemId)
                            BattlePass.Repository.UpdateReward(player, 117);

                        if (ItemId.Crowbar == busProductData.ItemId)
                            BattlePass.Repository.UpdateReward(player, 103);
                        
                        if (ItemId.Sandwich == busProductData.ItemId)
                            BattlePass.Repository.UpdateReward(player, 127);
                        
                        if (ItemId.Burger == busProductData.ItemId)
                            BattlePass.Repository.UpdateReward(player, 118);
                        
                        if (ItemId.HotDog == busProductData.ItemId)
                            BattlePass.Repository.UpdateReward(player, 119);
                        
                        if (ItemId.Sprunk == busProductData.ItemId)
                            BattlePass.Repository.UpdateReward(player, 121);
                        
                        if (ItemId.eCola == busProductData.ItemId)
                            BattlePass.Repository.UpdateReward(player, 128);
                    }
                    Wallet.Change(player, -prod.Price);
                    biz.BuyItemBusiness(characterData.UUID, prod.Name, prod.Price);
                    GameLog.Money($"player({characterData.UUID})", $"biz({biz.ID})", prod.Price, $"buyShop({prod.Name})");
                }
            }
            catch (Exception e)
            {
                Log.Write($"Event_ShopCallback Exception: {e.ToString()}");
            }
        }

        public static void OpenPetrolMenu(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (sessionData.BizID == -1 || !BizList.ContainsKey(sessionData.BizID)) return;

                Business biz = BizList[sessionData.BizID];
                Product prod = biz.Products[0];

                Trigger.ClientEvent(player, "openPetrol");
                
                EventSys.SendCoolMsg(player,"Заправка", "Добро пожаловать!", $"Цена за литр: {prod.Price}$", "", 10000);
               // Notify.Send(player, NotifyType.Info, NotifyPosition.Top, $"Цена за литр: {prod.Price}$", 7000);
            }
            catch (Exception e)
            {
                Log.Write($"OpenPetrolMenu Exception: {e.ToString()}");
            }
        }

        public static void OpenGunShopMenu(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (sessionData.TempBizID == -1 || !BizList.ContainsKey(sessionData.TempBizID)) return;
                List<List<Fractions.Manager.CraftData>> _WeaponsData = new List<List<Fractions.Manager.CraftData>>();
                Business biz = BizList[sessionData.TempBizID];
                for (int i = 0; i < gunsCat.Count; i++)
                {
                    List<Fractions.Manager.CraftData> _CraftsData = new List<Fractions.Manager.CraftData>();

                    foreach (Product g in biz.Products)
                    {
                        if (gunsCat[i].Contains(g.Name))
                        {
                            ItemId wType = (ItemId)Enum.Parse(typeof(ItemId), g.Name);
                            if (Chars.Repository.ItemsInfo.ContainsKey(wType))
                            {
                                _CraftsData.Add(new Fractions.Manager.CraftData(Chars.Repository.ItemsInfo[wType].Name, Chars.Repository.ItemsInfo[wType].Icon, g.Price));
                            }
                        }
                    }
                    _WeaponsData.Add(_CraftsData);
                }
                int ammoPrice = biz.Products.FirstOrDefault(p => p.Name == "Патроны").Price;
                List<int> _AmmoData = new List<int>();
                foreach (int ammo in AmmoPrices) _AmmoData.Add(Convert.ToInt32(ammo / 100f * ammoPrice));

                Trigger.ClientEvent(player, "client.gunshop.open", JsonConvert.SerializeObject(_WeaponsData), JsonConvert.SerializeObject(_AmmoData), biz.Products.FirstOrDefault(p => p.Name == "Модификации").Price);
                BattlePass.Repository.UpdateReward(player, 129);
            }
            catch (Exception e)
            {
                Log.Write($"OpenGunShopMenu Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.weaponshop.buyComponent")]
        public static void Event_WShopComponent(ExtPlayer player, int cat, int index, string componentId)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (sessionData.TempBizID == -1 || !BizList.ContainsKey(sessionData.TempBizID)) return;
                int bizid = sessionData.TempBizID;
                if (!characterData.Licenses[6])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет лицензии на оружие. Получить её можно в полицейском департаменте.", 10000);
                    return;
                }
                Business biz = BizList[bizid];
                Product prod = biz.Products.FirstOrDefault(p => p.Name == "Модификации");
                string prodName = gunsCat[cat][index];
                if (!WeaponComponents.WeaponsComponents.ContainsKey((uint)(Hash) WeaponRepository.GetHash(prodName))) return;
                int totalPrice = 0;
                uint componentHash = 0;
                ItemId wType = ItemId.Debug;
                foreach (KeyValuePair<uint, wComponentData> w in WeaponComponents.WeaponsComponents[(uint)(Hash) WeaponRepository.GetHash(prodName)].Components)
                {
                    if (w.Key.ToString() == componentId)
                    {
                        totalPrice = Convert.ToInt32(w.Value.Price / 100f * prod.Price);
                        componentHash = w.Key;
                        wType = (ItemId)Enum.Parse(typeof(ItemId), "c" + w.Value.Type.ToString("F"));
                    }
                }

                if (wType == ItemId.Debug || Chars.Repository.isFreeSlots(player, wType) != 0) return;
                if (componentHash == 0) return;
                if (UpdateData.CanIChange(player, totalPrice, true) != 255) return;
                int amount = Convert.ToInt32(totalPrice * 0.75 / 100);
                if (amount <= 0) amount = 1;
                if (!takeProd(bizid, amount, prod.Name, totalPrice))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetTovaraNaSkladeBiza), 3000);
                    return;
                }
                biz.BuyItemBusiness(characterData.UUID, prod.Name, totalPrice);
                Wallet.Change(player, -totalPrice);
                GameLog.Money($"player({characterData.UUID})", $"biz({biz.ID})", totalPrice, $"buyWShop(component({1}))");
                Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", wType, 1, $"{(uint)(Hash) WeaponRepository.GetHash(prodName)}_{componentHash}");
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили {Chars.Repository.ItemsInfo[wType].Name} за {totalPrice}$", 3000);
                BattlePass.Repository.UpdateReward(player, 1);
            }
            catch (Exception e)
            {
                Log.Write($"Event_WShopComponent Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.weaponshop.buyAmmo")]
        public static void Event_WShopAmmo(ExtPlayer player, int category, int ammo)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (sessionData.TempBizID == -1 || !BizList.ContainsKey(sessionData.TempBizID)) return;
                int bizid = sessionData.TempBizID;
                if (!characterData.Licenses[6])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет лицензии на оружие. Получить её можно в полицейском департаменте.", 10000);
                    return;
                }
                Business biz = BizList[bizid];
                Product prod = biz.Products.FirstOrDefault(p => p.Name == "Патроны");
                if (category < 0 || category > 4) return;
                if (ammo <= 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoInputAmmos), 3000);
                    return;
                }
                if (Chars.Repository.isFreeSlots(player, AmmoTypes[category], ammo) != 0) return;
                int totalPrice = ammo * Convert.ToInt32(AmmoPrices[category] / 100f * prod.Price);
                if (UpdateData.CanIChange(player, totalPrice, true) != 255) return;
                int prodamount = Convert.ToInt32(AmmoPrices[category] * ammo / 10);
                if (prodamount <= 1) prodamount = 1;
                
                if (!takeProd(bizid, prodamount, prod.Name, totalPrice))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetTovaraNaSkladeBiza), 3000);
                    return;
                }
                biz.BuyItemBusiness(characterData.UUID, prod.Name, totalPrice);
                Wallet.Change(player, -totalPrice);
                GameLog.Money($"player({characterData.UUID})", $"biz({biz.ID})", totalPrice, $"buyWShop(ammo({ammo}))");
                Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", AmmoTypes[category], ammo);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили {Chars.Repository.ItemsInfo[AmmoTypes[category]].Name} x{ammo} за {totalPrice}$", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"Event_WShopAmmo Exception: {e.ToString()}");
            }
        }
        public static List<int> AmmoPrices = new List<int>()
        {
            4, // pistol
            8, // shotguns
            8, // smg
            15, // rifles
            110, // sniperrifles
        };
        private static List<ItemId> AmmoTypes = new List<ItemId>()
        {
            ItemId.PistolAmmo, // pistol
            ItemId.ShotgunsAmmo, // shotguns
            ItemId.SMGAmmo, // smg
            ItemId.RiflesAmmo, // rifles
            ItemId.SniperAmmo, // sniperrifles
        };
        [RemoteEvent("server.weaponshop.buy")]
        public static void Event_WShop(ExtPlayer player, int cat, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (sessionData.TempBizID == -1 || !BizList.ContainsKey(sessionData.TempBizID)) return;
                int bizid = sessionData.TempBizID;
                if (!characterData.Licenses[6])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет лицензии на оружие. Получить её можно в полицейском департаменте.", 10000);
                    return;
                }
                Business biz = BizList[bizid];
                string prodName = gunsCat[cat][index];
                Product prod = biz.Products.FirstOrDefault(p => p.Name == prodName);
                if (UpdateData.CanIChange(player, prod.Price, true) != 255) return;
                ItemId wType = (ItemId)Enum.Parse(typeof(ItemId), prod.Name);
                if (Chars.Repository.isFreeSlots(player, wType) != 0) return;
                if (!takeProd(bizid, 1, prod.Name, prod.Price))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetTovaraNaSkladeBiza), 3000);
                    return;
                }
                Wallet.Change(player, -prod.Price);
                biz.BuyItemBusiness(characterData.UUID, prod.Name, prod.Price);
                GameLog.Money($"player({characterData.UUID})", $"biz({biz.ID})", prod.Price, $"buyWShop({prod.Name})");
                WeaponRepository.GiveWeapon(player, wType, WeaponRepository.GetSerial(false, biz.ID));
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили {prod.Name} за {prod.Price}$", 3000);
                BattlePass.Repository.UpdateReward(player, 130);
            }
            catch (Exception e)
            {
                Log.Write($"Event_WShop Exception: {e.ToString()}");
            }
        }
        private static List<List<string>> gunsCat = new List<List<string>>()
        {
            new List<string>()
            {
                "Pistol",
                "CombatPistol",
                "Revolver",
                "HeavyPistol",
                "APPistol",
                "Pistol50",
            },
            new List<string>()
            {
                "BullpupShotgun",
                "PumpShotgun",
                "SawnOffShotgun",
                "DoubleBarrelShotgun",
                "PumpShotgunMk2",
            },
            new List<string>()
            {
                "CombatPDW",
                "MachinePistol",
                "SMGMk2",
                "MiniSMG",
                "MicroSMG",
                "SMG",
            },
            new List<string>()
            {
                "AssaultRifle",
                "CompactRifle",
                "CarbineRifle",
                "AdvancedRifle",
                "BullpupRifle",
            },
            /*new List<string>()
            {
                "MarksmanRifle",
                "SniperRifle",
            },*/
        };
        #endregion

        public static void changeOwner(string oldName, string newName)
        {
            try
            {
                var toChange = BizList
                    .Where(b => b.Value.Owner == oldName)
                    .Select(b => b.Key)
                    .ToList();

                foreach (int id in toChange)
                {
                    if (BizList.ContainsKey(id))
                        BizList[id].SetOwner(newName);
                }
            }
            catch (Exception e)
            {
                Log.Write($"changeOwner NAPI.Task Exception: {e.ToString()}");
            }
        }
    }

    public class Order
    {
        public Order(string name, int amount)
        {
            Name = name;
            Amount = amount;
            Taked = false;
        }

        public string Name { get; set; }
        public int Amount { get; set; }
        [JsonIgnore]
        public bool Taked { get; set; }
        [JsonIgnore]
        public int UID { get; set; }
    }

    public class Product
    {
        public Product(int price, int left, int autosell, string name, bool ordered)
        {
            Price = price;
            Lefts = left;
            Autosell = autosell;
            Name = name;
            Ordered = ordered;
        }

        public int Price { get; set; }
        public int Lefts { get; set; }
        public int Autosell { get; set; }
        public string Name { get; set; }
        public bool Ordered { get; set; }
    }

    public class Business
    {
        public int ID { get; set; }
        public string Owner { get; set; }
        public int SellPrice { get; set; }
        public int Type { get; set; }
        public string Address { get; set; }
        public List<Product> Products { get; set; }
        public int BankID { get; set; }
        public Vector3 EnterPoint { get; set; }
        public Vector3 UnloadPoint { get; set; }
        public int Mafia { get; set; }
        public double Tax { get; set; }

        public List<Order> Orders { get; set; }

        //[JsonIgnore]
        //private Blip blip = null;
        [JsonIgnore]
        private ExtMarker marker = null;
        [JsonIgnore]
        private ExtTextLabel label = null;
        [JsonIgnore]
        private ExtTextLabel mafiaLabel = null;
        [JsonIgnore]
        private ExtColShape shape = null;
        [JsonIgnore]
        private ExtColShape truckerShape = null;
        [JsonIgnore]
        public int Zatratq = 0;
        [JsonIgnore]
        public int Pribil = 0;

        public bool IsAuction = false;
        public bool IsSave = false;

        public Business(int id, string owner, int sellPrice, int type, List<Product> products, Vector3 enterPoint, Vector3 unloadPoint, int bankID, int mafia, List<Order> orders, double tax = 0.026)
        {
            ID = id;
            Owner = owner;
            SellPrice = sellPrice;
            Type = type;

            foreach (var product in products)
            {
                if (!product.Ordered)
                    continue;
                
                if (orders.Any(o => o.Name == product.Name))
                    continue;

                product.Ordered = false;
            }
            
            Products = products;
            EnterPoint = enterPoint;
            UnloadPoint = unloadPoint;
            BankID = bankID;
            Mafia = mafia;
            Orders = orders;
            Zatratq = 0;
            Pribil = 0;
            Tax = tax;
            IsAuction = Players.Phone.Auction.Repository.IsElement(AuctionType.Biz, id);

            var random = new Random();
            foreach (var order in orders)
            {
                do
                {
                    order.UID = random.Next(000000, 999999);
                } while (BusinessManager.Orders.ContainsKey(order.UID));
                
                BusinessManager.Orders.TryAdd(order.UID, ID);
            }

            truckerShape = CustomColShape.CreateCylinderColShape(UnloadPoint - new Vector3(0, 0, 1), 8, 10, NAPI.GlobalDimension, ColShapeEnums.Trucker, ID);

            float range;
            if (Type == 1) range = 10f;
            else if (Type == 12) range = 5f;
            else range = 1f;
            shape = CustomColShape.CreateCylinderColShape(EnterPoint, range, 3, 0, ColShapeEnums.BusinessAction, ID);

            Main.CreateBlip(new Main.BlipData(BusinessManager.BlipByType[Type], BusinessManager.BusinessTypeNames[Type], EnterPoint, BusinessManager.BlipColorByType[Type], true));
            //blip = (ExtBlip) NAPI.Blip.CreateBlip(Convert.ToUInt32(BusinessManager.BlipByType[Type]), EnterPoint, 1, Convert.ToByte(BusinessManager.BlipColorByType[Type]), Main.StringToU16(BusinessManager.BusinessTypeNames[Type]), 255, 0, true);
            float textrange = (Type == 1) ? 5F : 20F;
            label = (ExtTextLabel) NAPI.TextLabel.CreateTextLabel(Main.StringToU16("Business"), new Vector3(EnterPoint.X, EnterPoint.Y, EnterPoint.Z + 1.5), textrange, 0.5F, 0, new Color(255, 255, 255), true, 0);
            mafiaLabel = (ExtTextLabel) NAPI.TextLabel.CreateTextLabel(Main.StringToU16("Mafia: none"), new Vector3(EnterPoint.X, EnterPoint.Y, EnterPoint.Z + 2), 5F, 0.5F, 0, new Color(255, 255, 255), true, 0);
            UpdateLabel();
            if (Type != 1) marker = (ExtMarker) NAPI.Marker.CreateMarker(1, EnterPoint - new Vector3(0, 0, range - 0.3f), new Vector3(), new Vector3(), range, new Color(255, 255, 255, 220), false, 0);
        }

        public void UpdateLabel()
        {
            Trigger.SetMainTask(() =>
            {
                try
                {
                    string text = $"~w~{BusinessManager.BusinessTypeNames[Type]}\n";

                    if (IsAuction) text += $"~w~Выставлен на аукцион\n";
                    else if (IsOwner()) text += $"~p~{Owner}\n";
                    else text += $"~w~Цена: ~g~{Wallet.Format(SellPrice)}$\n";
                    if (Type == 1) text += $"~w~Цена за 1л: ~g~{Products[0].Price}$\n";
                    text += $"~c~ID{ID}";
                    label.Text = text;
                    if (Mafia != -1) mafiaLabel.Text = $"~w~Мафия: ~r~{Fractions.Manager.GetName(Mafia)}";
                    else mafiaLabel.Text = "~w~Мафия: ~w~пусто";
                }
                catch (Exception e)
                {
                    BusinessManager.Log.Write($"UpdateLabel Exception: {e.ToString()}");
                }
            });
        }

        public void Destroy()
        {
            try
            {
                if (marker != null && marker.Exists) marker.Delete();
                marker = null;
                if (label != null && label.Exists) label.Delete();
                label = null;
                CustomColShape.DeleteColShape(shape);
                shape = null;
                CustomColShape.DeleteColShape(truckerShape);
                truckerShape = null;
            }
            catch (Exception e)
            {
                BusinessManager.Log.Write($"Destroy Exception: {e.ToString()}");
            }
        }

        public async Task Save(ServerBD db)
        {
            try
            {
                IsSave = false;
                
                Bank.SetSave(BankID);

                await db.Businesses
                    .Where(b => b.Id == ID)
                    .Set(b => b.Owner, Owner)
                    .Set(b => b.Sellprice, SellPrice)
                    .Set(b => b.Products, JsonConvert.SerializeObject(Products))
                    .Set(b => b.Money, BankID)
                    .Set(b => b.Mafia, Mafia)
                    .Set(b => b.Orders, JsonConvert.SerializeObject(Orders))
                    .UpdateAsync();
                
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public void BuyItemBusiness(int uuid, string itemName, int cost)
        {
            Businesses.History.Repository.AddHistory(uuid, this.ID, itemName, cost);
            Pribil += cost;
        }

        public void SetOwner(string name)
        {
            Owner = name;
            UpdateLabel();
            IsSave = true;
        }
        
        public void ClearOwner()
        {
            Owner = "Государство";
            UpdateLabel();
            IsSave = true;
        }

        public bool IsOwner() => Owner != "Государство";
    }

    public class LsCustoms
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        public LsCustoms(Vector3 Position, Vector3 Rotation)
        {
            this.Position = Position;
            this.Rotation = Rotation;
        }
    }
}

