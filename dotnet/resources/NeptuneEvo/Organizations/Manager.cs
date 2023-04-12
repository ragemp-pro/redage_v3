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
using NeptuneEvo.Core;

using NeptuneEvo.Functions;
using NeptuneEvo.GUI;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Localization;
using NeptuneEvo.Fractions;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Organizations.Models;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.Players.Popup.List.Models;
using NeptuneEvo.Quests.Models;
using NeptuneEvo.Table.Models;
using NeptuneEvo.Utils;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;
using NeptuneEvo.VehicleData.Models;

namespace NeptuneEvo.Organizations
{
    class Manager : Script
    {
        private static readonly nLog Log = new nLog("Organizations.Manager");

        public static RankToAccess[] OrganizationDefaultAccess = new RankToAccess[]
        {
            RankToAccess.Invite,
            RankToAccess.UnInvite,
            RankToAccess.SetRank,
            RankToAccess.SetVehicleRank,
            RankToAccess.OrgTuning,
            RankToAccess.OrgUpgrate,
            RankToAccess.OrgBuyCars,
            RankToAccess.TableWall,
            RankToAccess.EditAllTabletWall,
            RankToAccess.OrgSellCar,

            RankToAccess.Logs,
            RankToAccess.Reprimand,
            
            RankToAccess.CreateDepartment,
            RankToAccess.DeleteDepartment
        };

        public static Dictionary<int, RankData> OrganizationDefaultRanks =
            new Dictionary<int, RankData>();
        private static Dictionary<int, OrganizationData> Organizations = new Dictionary<int, OrganizationData>();
        public static Dictionary<int, List<OrganizationMemberData>> AllMembers = new Dictionary<int, List<OrganizationMemberData>>();

        public static OrganizationData GetOrganizationData(int organizationId)
        {
            if (Organizations.ContainsKey(organizationId))
                return Organizations[organizationId];

            return null;
        }

        public static int MaxMats = 50_000;
        
        //public static Blip OfficeCreate;

        public static Dictionary<string, int> WeaponsOrgPrice = new Dictionary<string, int>()
        {
            {"Pistol", 5000},
            {"PistolMk2", 5500},
            {"Pistol50", 5500},
            {"HeavyPistol", 7500},
            {"PumpShotgun", 8000},
            {"DoubleBarrelShotgun", 7000},
            {"SawnOffShotgun", 7500},
            {"MiniSMG", 10000},
            {"SMGMk2", 15000},
            {"MachinePistol", 9000},
            {"MicroSMG", 10000},
            {"CombatPDW", 13000},
            {"CompactRifle", 15000},
            {"AssaultRifle", 20000},
            {"Armor", 50000},
        };

        public static Vector3 GarageModShop = new Vector3(-1391.366, -471.936, 77.91);

        private static byte Step = 0;
        private static Vector3[] OutsideGarage = new Vector3[17] 
        {
            new Vector3(-810.205, 372.7328, 88.08846),
            new Vector3(-806.5412, 372.5238, 88.08907),
            new Vector3(-803.2227, 372.4598, 88.08852),
            new Vector3(-799.6732, 373.0827, 88.08889),
            new Vector3(-796.1635, 372.6285, 88.08764),
            new Vector3(-792.678, 372.5224, 88.08839),
            new Vector3(-789.2848, 372.6759, 88.08798),
            new Vector3(-781.207, 372.794, 88.08726),
            new Vector3(-776.9878, 372.9521, 88.0882),
            new Vector3(-773.7839, 373.1432, 88.08797),
            new Vector3(-770.1907, 373.1289, 88.08805),
            new Vector3(-766.8118, 372.8154, 88.08694),
            new Vector3(-759.7816, 372.8338, 88.08759),
            new Vector3(-756.2082, 372.9322, 88.0881),
            new Vector3(-752.9163, 372.6143, 88.08644),
            new Vector3(-749.2435, 372.9106, 88.08632),
            new Vector3(-745.915, 372.9658, 88.08605),
        };

        public static Vector3[] StockPositions = new Vector3[3]
        {
            new Vector3(-1014.00415, -769.8181, 70.49417),
            new Vector3(),
            new Vector3(),

        };

        public static Vector3[] CraftPositions = new Vector3[3]
        {
            new Vector3(-997.15295, -748.20026, 70.29419),
            new Vector3(),
            new Vector3(),

        };

        public static (float, Vector3, int)[] OrganizationTypeList = new (float, Vector3, int)[3] {
            (1.5302082f, new Vector3(-1003.16095, -774.5083, 61.8944), 238593), //New interier organization 
            (1.5302082f, new Vector3(-1003.16095, -774.5083, 61.8944), 238593),
            (1.5302082f, new Vector3(-1003.16095, -774.5083, 61.8944), 238593),
        };

        public static Vector3[] GaragePositions = new Vector3[15]
        {
            //1A
            new Vector3(-1389.815, -477.88, 57.31421),
            new Vector3(-1385.699, -472.8283, 57.31377),
            new Vector3(-1377.489, -472.0624, 57.31357),
            new Vector3(-1371.994, -475.9771, 57.31249),
            new Vector3(-1370.637, -482.9035, 57.31341),
            //1B
            new Vector3(-1389.815, -477.88, 62.65846),
            new Vector3(-1385.699, -472.8283, 62.65843),
            new Vector3(-1377.489, -472.0624, 62.65893),
            new Vector3(-1371.994, -475.9771, 62.65728),
            new Vector3(-1370.637, -482.9035, 62.65884),
            //1C
            new Vector3(-1389.815, -477.88, 68.00451),
            new Vector3(-1385.699, -472.8283, 68.00307),
            new Vector3(-1377.489, -472.0624, 68.0032),
            new Vector3(-1371.994, -475.9771, 68.00385),
            new Vector3(-1370.637, -482.9035, 68.0045),
        };
        public static float[] GarageRotations = new float[15]
        {
            //1A
            252.1797f,
            223.5804f,
            168.2739f,
            122.8532f,
            74.71387f,
            //1B
           -43.0244f,
           -77.36455f,
           -91.713135f,
           116.91544f,
           110.80071f,
            //1C
            252.1797f,
            223.5804f,
            168.2739f,
            122.8532f,
            74.71387f,
        };

        public static int DefaultDimension = 500000;
        
        public static void onResourceStart()
        {
            try
            {
                OrganizationDefaultRanks = new Dictionary<int, RankData>
                {
                    {
                        0, new RankData
                        {
                            Name = "Участник",
                            Salary = 0,
                            MaxScore = 100,
                            Access = new List<RankToAccess>()
                        }
                    },
                    {
                        1, new RankData
                        {
                            Name = "Заместитель",
                            Salary = 0,
                            MaxScore = 250,
                            Access = OrganizationDefaultAccess.ToList()
                        }

                    },
                    {
                        2, new RankData
                        {
                            Name = "Владелец",
                            Salary = 0,
                            MaxScore = 500,
                            Access = OrganizationDefaultAccess.ToList()
                        }

                    }
                };
                //OfficeCreate = (ExtBlip) NAPI.Blip.CreateBlip(685, new Vector3(-773.8601,312.2175, 84.57813), 1, 4, "Organization", 255, 0, true, 0, 0);
                Main.CreateBlip(new Main.BlipData(685, "Семья", new Vector3(-773.8601, 312.2175, 84.57813), 4, true));

                //Armor Craft
                for (byte i = 0; i < 3; i++)
                {
                    CustomColShape.CreateCylinderColShape(CraftPositions[i], 2, 2, NAPI.GlobalDimension, ColShapeEnums.Organizations, 1);//Крафт
                    NAPI.Marker.CreateMarker(1, CraftPositions[i] - new Vector3(0, 0, 1.5), new Vector3(), new Vector3(), 2f, new Color(255, 255, 255, 220));
                }
                //Stock
                for (byte i = 0; i < 3; i++)
                {
                    CustomColShape.CreateCylinderColShape(StockPositions[i], 2, 2, NAPI.GlobalDimension, ColShapeEnums.Organizations, 2);
                    NAPI.Marker.CreateMarker(1, StockPositions[i] - new Vector3(0, 0, 1.5), new Vector3(), new Vector3(), 2f, new Color(255, 255, 255, 220));
                }

                //Garage Enter
                NAPI.Marker.CreateMarker(1, new Vector3(-793.943, 303.561, 84.70), new Vector3(), new Vector3(), 3f, new Color(255, 255, 0, 60));
                CustomColShape.CreateCylinderColShape(new Vector3(-793.943, 303.561, 85.70), 3f, 3f, NAPI.GlobalDimension, ColShapeEnums.Organizations, 3);


                //Garage Exit
                //(ExtTextLabel) NAPI.TextLabel.CreateTextLabel("~w~В офис", new Vector3(-1396.274, -480.7186, 57.100), 5f, 0.3f, 0, new Color(255, 255, 255), true, (uint)fraction + 1);
                NAPI.Marker.CreateMarker(1, new Vector3(-1067.1245, -88.08039, -97.89984), new Vector3(), new Vector3(), 2f, new Color(255, 255, 255, 60), dimension: NAPI.GlobalDimension);
                //CustomColShape.CreateCylinderColShape(new Vector3(-1396.274, -480.7186, 57.100), 2f, 2f, NAPI.GlobalDimension, ColShapeEnums.Organizations, 4);
                //
                NAPI.TextLabel.CreateTextLabel("~w~В гараж", new Vector3(-1000.14856, -774.0971, 61.734413), 5f, 0.3f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension);
                NAPI.Marker.CreateMarker(1, new Vector3(-1000.14856, -774.0971, 60.734413), new Vector3(), new Vector3(), 2f, new Color(255, 255, 255, 60));
                CustomColShape.CreateCylinderColShape(new Vector3(-1000.14856, -774.0971, 60.734413), 2f, 2f, NAPI.GlobalDimension, ColShapeEnums.Organizations, 5);
                //
                NAPI.TextLabel.CreateTextLabel("~w~В гараж", new Vector3(-1000.14856, -774.0971, 60.734413), 5f, 0.3f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension);
                NAPI.Marker.CreateMarker(1, new Vector3(-1000.14856, -774.0971, 60.734413), new Vector3(), new Vector3(), 2f, new Color(255, 255, 255, 60));
                CustomColShape.CreateCylinderColShape(new Vector3(-1000.14856, -774.0971, 60.734413), 2f, 2f, NAPI.GlobalDimension, ColShapeEnums.Organizations, 5);
                //
                NAPI.TextLabel.CreateTextLabel("~w~В гараж", new Vector3(-1000.14856, -774.0971, 60.734413), 5f, 0.3f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension);
                NAPI.Marker.CreateMarker(1, new Vector3(-1000.14856, -774.0971, 60.734413), new Vector3(), new Vector3(), 2f, new Color(255, 255, 255, 60));
                CustomColShape.CreateCylinderColShape(new Vector3(-1000.14856, -774.0971, 60.734413), 2f, 2f, NAPI.GlobalDimension, ColShapeEnums.Organizations, 5);
               
                //Office Exit
                NAPI.TextLabel.CreateTextLabel("~w~На улицу", new Vector3(-1003.197, -774.2839, 61.8944), 5f, 0.3f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension);
                NAPI.Marker.CreateMarker(1, new Vector3(-1003.197, -774.2839, 60.8944), new Vector3(), new Vector3(), 2f, new Color(255, 255, 255, 60));
                CustomColShape.CreateCylinderColShape(new Vector3(-1003.197, -774.2839, 61.8944), 2f, 2f, NAPI.GlobalDimension, ColShapeEnums.Organizations, 6);
                //
                NAPI.TextLabel.CreateTextLabel("~w~На улицу", new Vector3(-1003.197, -774.2839, 61.8944), 5f, 0.3f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension);
                NAPI.Marker.CreateMarker(1, new Vector3(-1003.197, -774.2839, 60.8944), new Vector3(), new Vector3(), 2f, new Color(255, 255, 255, 60));
                CustomColShape.CreateCylinderColShape(new Vector3(-1003.197, -774.2839, 61.8944), 2f, 2f, NAPI.GlobalDimension, ColShapeEnums.Organizations, 6);
                //
                NAPI.TextLabel.CreateTextLabel("~w~На улицу", new Vector3(-1003.197, -774.2839, 61.8944), 5f, 0.3f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension);
                NAPI.Marker.CreateMarker(1, new Vector3(-1003.197, -774.2839, 60.8944), new Vector3(), new Vector3(), 2f, new Color(255, 255, 255, 60));
                CustomColShape.CreateCylinderColShape(new Vector3(-1003.197, -774.2839, 60.8944), 2f, 2f, NAPI.GlobalDimension, ColShapeEnums.Organizations, 6);
               

                //Office register
                //NAPI.Marker.CreateMarker(1, new Vector3(-773.945, 313.0294, 83.80606) + new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 2f, new Color(255, 255, 255, 60));
                PedSystem.Repository.CreateQuest("ig_jackie", new Vector3(-773.945, 313.0294, 85.70606), 177.9122f, 0,"npc_org", ColShapeEnums.OrgCreate, "~y~NPC~w~ Полли\n Директор Организации", false);
                
                //CustomColShape.CreateCylinderColShape(new Vector3(-773.945, 313.0294, 85.70606), 2f, 1f, NAPI.GlobalDimension, ColShapeEnums.Organizations, 7);

                //
                
                using MySqlCommand cmd = new MySqlCommand()
                {
                    CommandText = "SELECT * FROM `organizations`"
                };
                using DataTable result = MySQL.QueryRead(cmd);
                if (result == null || result.Rows.Count == 0) return;
                foreach (DataRow Row in result.Rows)
                {
                    int orgId = Convert.ToInt32(Row["Organization"]);
                    int OwnerUUID = Convert.ToInt32(Row["OwnerUUID"]);
                    string name = Row["Name"].ToString();
                    byte upgrade = Convert.ToByte(Row["OfficeUP"]);
                    bool customs = Convert.ToBoolean(Row["Customs"]);
                    bool stock = Convert.ToBoolean(Row["Stock"]);
                    bool crimeoptions = Convert.ToBoolean(Row["CrimeOptions"]);
                    bool pistolscheme = Convert.ToBoolean(Row["PistolScheme"]);
                    bool pistolmk2scheme = Convert.ToBoolean(Row["PistolMk2Scheme"]);
                    bool pistol50scheme = Convert.ToBoolean(Row["Pistol50Scheme"]);
                    bool heavypistolscheme = Convert.ToBoolean(Row["HeavyPistolScheme"]);
                    bool pumpshotgunscheme = Convert.ToBoolean(Row["PumpShotgunScheme"]);
                    bool dbarrelshotgunscheme = Convert.ToBoolean(Row["DoubleBarrelShotgunScheme"]);
                    bool sawnoffscheme = Convert.ToBoolean(Row["SawnOffShotgunScheme"]);
                    bool minismgscheme = Convert.ToBoolean(Row["MiniSMGScheme"]);
                    bool smgmk2scheme = Convert.ToBoolean(Row["SMGMk2Scheme"]);
                    bool machinepistolscheme = Convert.ToBoolean(Row["MachinePistolScheme"]);
                    bool microsmgscheme = Convert.ToBoolean(Row["MicroSMGScheme"]);
                    bool combatpdwscheme = Convert.ToBoolean(Row["CombatPDWScheme"]);
                    bool compactriflescheme = Convert.ToBoolean(Row["CompactRifleScheme"]);
                    bool assaultriflescheme = Convert.ToBoolean(Row["AssaultRifleScheme"]);
                    bool armorscheme = Convert.ToBoolean(Row["ArmorScheme"]);
                    int drugs = Convert.ToInt32(Row["Drugs"]);
                    int mats = Convert.ToInt32(Row["Mats"]);
                    int medkits = Convert.ToInt32(Row["MedKits"]);
                    int money = Convert.ToInt32(Row["Money"]);
                    bool isopen = Convert.ToBoolean(Row["IsOpen"]);
                    bool status = Convert.ToBoolean(Row["Status"]);
                    int blipid = Convert.ToInt32(Row["BlipID"]);
                    byte blipc = Convert.ToByte(Row["BlipColor"]);
                    Vector3 blippos = JsonConvert.DeserializeObject<Vector3>(Row["BlipXYZ"].ToString());
                    string ranks = Row["Ranks"].ToString();

                    var organizationData = new OrganizationData
                    {
                        Id = orgId,
                        OwnerUUID = OwnerUUID,
                        Name = name,
                        OfficeUpgrade = upgrade,
                        Stock = stock,
                        CrimeOptions = crimeoptions,
                        Schemes = new Dictionary<string, bool>()
                        {
                            {"Pistol", pistolscheme},
                            {"PistolMk2", pistolmk2scheme},
                            {"Pistol50", pistol50scheme},
                            {"HeavyPistol", heavypistolscheme},
                            {"PumpShotgun", pumpshotgunscheme},
                            {"DoubleBarrelShotgun", dbarrelshotgunscheme},
                            {"SawnOffShotgun", sawnoffscheme},
                            {"MiniSMG", minismgscheme},
                            {"SMGMk2", smgmk2scheme},
                            {"MachinePistol", machinepistolscheme},
                            {"MicroSMG", microsmgscheme},
                            {"CombatPDW", combatpdwscheme},
                            {"CompactRifle", compactriflescheme},
                            {"AssaultRifle", assaultriflescheme},
                            {"Armor", armorscheme},
                        },
                        Materials = mats,
                        Drugs = drugs,
                        MedKits = medkits,
                        Money = money,
                        IsOpenStock = isopen,
                        Status = status,
                        BlipId = blipid,
                        BlipColor = blipc,
                        BlipPosition = blippos
                    };

                    organizationData.Departments = JsonConvert.DeserializeObject<Dictionary<int, DepartmentData>>(Row["departments"].ToString());
                    organizationData.Discord = Row["discord"].ToString();
                    organizationData.Salary = Convert.ToByte(Row["salary"]);
                    organizationData.Color = JsonConvert.DeserializeObject<Color>(Row["color"].ToString());
                    organizationData.Date = Convert.ToDateTime(Row["date"]);
                    
                    CustomColShape.CreateCylinderColShape(new Vector3(-1396.274, -480.7186, 57.100), 2f, 2f, organizationData.GetDimension(), ColShapeEnums.Organizations, 4);
                    
                    try
                    {
                        if (ranks != "-1" && ranks.Length > 1)
                        {
                            organizationData.Ranks = JsonConvert.DeserializeObject<Dictionary<int, RankData>>(ranks);
                        }
                        else organizationData.Ranks = OrganizationDefaultRanks.Clone();
                    }
                    catch
                    {
                        organizationData.Ranks = OrganizationDefaultRanks.Clone();
                    }
                    organizationData.DefaultAccess = OrganizationDefaultAccess.ToList();

                    if (organizationData.Stock)
                        organizationData.DefaultAccess.Add(RankToAccess.OpenStock);
                    
                    if (organizationData.CrimeOptions)
                    {
                        organizationData.DefaultAccess.Add(RankToAccess.OrgCrime);
                        organizationData.DefaultAccess.Add(RankToAccess.InCar);
                        organizationData.DefaultAccess.Add(RankToAccess.Pull);
                        organizationData.DefaultAccess.Add(RankToAccess.Cuff);
                        organizationData.DefaultAccess.Add(RankToAccess.FamilyZone);
                        organizationData.DefaultAccess.Add(RankToAccess.IsWar);
                    }

                    if (status)
                    {
                        if (stock) 
                            organizationData.StockLabel = (ExtTextLabel) NAPI.TextLabel.CreateTextLabel(Main.StringToU16($"~y~Склад {name}"), StockPositions[upgrade], 5F, 0.5F, 0, new Color(255, 255, 255), true, organizationData.GetDimension());
                        else 
                            organizationData.StockLabel = (ExtTextLabel) NAPI.TextLabel.CreateTextLabel(Main.StringToU16($"~y~Склад {name}\n~r~Не активен"), StockPositions[upgrade], 5F, 0.5F, 0, new Color(255, 255, 255), true, organizationData.GetDimension());
                        
                        if(blipid != -1) 
                            organizationData.Blip = (ExtBlip) NAPI.Blip.CreateBlip(organizationData.BlipId, organizationData.BlipPosition, 1, organizationData.BlipColor, name, 255, 0, true, 0, 0);
                    }
                    
                    if (!AllMembers.ContainsKey(orgId))
                        AllMembers.Add(orgId, new List<OrganizationMemberData>());
                    
                    Organizations.Add(orgId, organizationData);
                }

                using MySqlCommand cmdSelect = new MySqlCommand()
                {
                    CommandText = "SELECT * FROM `orgranks`"
                };
                using DataTable resultSelect = MySQL.QueryRead(cmdSelect);
                if (resultSelect != null)
                {
                    foreach (DataRow Row in resultSelect.Rows)
                    {
                        var orgId = Convert.ToInt32(Row["id"]);
                    
                        if (!AllMembers.ContainsKey(orgId))
                            AllMembers.Add(orgId, new List<OrganizationMemberData>());

                        var memberOrganizationData = new OrganizationMemberData
                        {
                            UUID = Convert.ToInt32(Row["uuid"]),
                            Name = Row["name"].ToString(),
                            Id = orgId,
                            Rank = Convert.ToByte(Row["rank"]),
                            Date = Convert.ToDateTime(Row["date"]),
                            LastLoginDate = Convert.ToDateTime(Row["lastLoginDate"]),
                            Avatar = Row["avatar"].ToString(),
                            DepartmentId = Convert.ToInt32(Row["departmentId"]),
                            DepartmentRank = Convert.ToInt32(Row["departmentRank"]),
                            Score = Convert.ToInt32(Row["score"]),
                            Access = JsonConvert.DeserializeObject<List<RankToAccess>>(Row["access"].ToString()),
                            Lock = JsonConvert.DeserializeObject<List<RankToAccess>>(Row["lock"].ToString()),
                        };
                    
                        AllMembers[orgId].Add(memberOrganizationData);
                    }
                }

                using MySqlCommand cmdOrgvehicles = new MySqlCommand()
                {
                    CommandText = "SELECT * FROM `orgvehicles`"
                };
                using DataTable resultOrgvehicles = MySQL.QueryRead(cmdOrgvehicles);
                if (resultOrgvehicles == null || resultOrgvehicles.Rows.Count == 0) return;
                foreach (DataRow Row in resultOrgvehicles.Rows)
                {
                    int orgId = Convert.ToInt32(Row["organization"]);
                    
                    var organizationData = GetOrganizationData(orgId);
                    if (organizationData == null) 
                        continue;
                    
                    string number = Row["number"].ToString();
                    string model = Row["model"].ToString();
                    byte position = Convert.ToByte(Row["position"]);
                    int minrank = Convert.ToInt32(Row["rank"]);
                    var components = JsonConvert.DeserializeObject<VehicleCustomization>(Row["components"].ToString());
                    float dirt = (float)Row["dirt"];
                    int petrol = Convert.ToInt32(Row["petrol"]);
                    organizationData.Vehicles.Add(number, new OrganizationVehicleData(model, minrank, position, dirt, petrol, components));
                }
                foreach (var organizationData in Organizations.Values)
                {
                    if (organizationData.Status)
                    {
                        foreach (var organizationVehicle in organizationData.Vehicles)
                            SpawnOrganizationCar(organizationData.Id, organizationVehicle.Key, organizationVehicle.Value);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }

        private static Random Rnd = new Random();
        public static void DeleteVehicle(ExtPlayer player, string number, bool type)
        {
            try
            {
                if (!player.IsOrganizationAccess(RankToAccess.OrgSellCar)) return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) return;
                if (!organizationData.Vehicles.ContainsKey(number))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Такого транспорта более нет в Вашей семье.", 3000);
                    return;
                }
                if (!type)
                {
                    var price = 0;
                    var model = organizationData.Vehicles[number].model;
                    if (BusinessManager.BusProductsData.ContainsKey(model))
                    {
                        switch (accountData.VipLvl)
                        {
                            case 0: // None
                            case 1: // Bronze
                            case 2: // Silver
                                price = Convert.ToInt32(BusinessManager.BusProductsData[model].Price * 0.4);
                                break;
                            case 3: // Gold
                                price = Convert.ToInt32(BusinessManager.BusProductsData[model].Price * 0.5);
                                break;
                            case 4: // Platinum
                            case 5: // Media Platinum
                                price = Convert.ToInt32(BusinessManager.BusProductsData[model].Price * 0.6);
                                break;
                            default:
                                price = Convert.ToInt32(BusinessManager.BusProductsData[model].Price * 0.4);
                                break;
                        }
                    }
                    sessionData.CarSellGov = number;
                    Trigger.ClientEvent(player, "openDialog", "ORGCAR_SELL_TOGOV", $"Вы действительно хотите продать государству {model} ({number}) за ${MoneySystem.Wallet.Format(price)}?");
                }
                else
                {
                    sessionData.CarSellGov = null;
                    var organizationVehicle = organizationData.Vehicles[number];
                    int price = 0;
                    string model = organizationVehicle.model;
                    if (BusinessManager.BusProductsData.ContainsKey(model))
                    {
                        switch (accountData.VipLvl)
                        {
                            case 0: // None
                            case 1: // Bronze
                            case 2: // Silver
                                price = Convert.ToInt32(BusinessManager.BusProductsData[model].Price * 0.4);
                                break;
                            case 3: // Gold
                                price = Convert.ToInt32(BusinessManager.BusProductsData[model].Price * 0.5);
                                break;
                            case 4: // Platinum
                            case 5: // Media Platinum
                                price = Convert.ToInt32(BusinessManager.BusProductsData[model].Price * 0.6);
                                break;
                            default:
                                price = Convert.ToInt32(BusinessManager.BusProductsData[model].Price * 0.4);
                                break;
                        }
                    }
                    MoneySystem.Wallet.Change(player, price);
                    GameLog.Money($"server", $"player({player.GetUUID()})", price, $"orgCarSell({model})");
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы продали {model} ({number}) за {MoneySystem.Wallet.Format(price)}$", 3000);
                    Table.Logs.Repository.AddLogs(player, OrganizationLogsType.SellCar, $"Продал {model} ({number})");
                    //Admin.AdminsLog(1, $"[ВНИМАНИЕ] Игрок {player.Name} продал {model} ({number}) за {MoneySystem.Wallet.Format(price)}$", 1, "#FF0000");
                    organizationData.Used[organizationVehicle.garageId] = false;
                    var vehicle = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Organization, number);
                    if (vehicle != null) 
                        VehicleStreaming.DeleteVehicle(vehicle);
                    else
                    {
                        Log.Write($"Organization {organizationData.Id} error in selling vehicle {model} ({number}) for ${price}", nLog.Type.Error);
                        Admin.ErrorLog($"[Organizations] {organizationData.Name}: can not find vehicle({number}) (selling for ${price})");
                    }
                    organizationData.Vehicles.Remove(number);
                    Table.Vehicle.Repository.GetVehicles(player);
                    Trigger.SetTask(async () =>
                    {
                        try
                        {
                            await using var db = new ServerBD("MainDB");//В отдельном потоке

                            await db.Orgvehicles
                                .Where(v => v.Number == number)
                                .DeleteAsync();
                        }
                        catch (Exception e)
                        {
                            Debugs.Repository.Exception(e);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Log.Write($"DeleteVehicle Exception: {e.ToString()}");
            }
        }
        public static string CreateVehicle(int orgId, string model, Color color)
        {
            try
            {
                var organizationData = GetOrganizationData(orgId);
                if (organizationData == null) return "";
                
                var data = new VehicleCustomization
                {
                    PrimColor = color,
                    SecColor = color,
                };
                var number = GenerateNumber(orgId);
                byte position = 0;
                while (organizationData.Used[position]) position++;
                organizationData.Used[position] = true;
                
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.InsertAsync(new Orgvehicles
                        {
                            Organization = orgId,
                            Number = number,
                            Model = model,
                            Position = (sbyte) position,
                            Rank = 0,
                            Components = JsonConvert.SerializeObject(data),
                            Dirt = 0,
                            Petrol = 100,
                        });
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
                
                organizationData.Vehicles.Add(number, new OrganizationVehicleData(model, 0, position, 0f, 100, data));
                var veh = VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey(model), GaragePositions[position], GarageRotations[position], 0, 0, number, dimension: organizationData.GetDimension(), locked: true, acc: VehicleAccess.OrganizationGarage, fr: orgId, minrank: 0, petrol: 100, dirt: 0.0f);
                VehicleManager.OrgApplyCustomization(veh, data);
                return number;
            }
            catch (Exception e)
            {
                Log.Write($"CreateVehicle Exception: {e.ToString()}");
                return "ERROR";
            }
        }
        
        [Command(AdminCommands.setfamily)]
        public static void CMD_setfamily(ExtPlayer player, int orgId, int status) {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.setfamily)) return;
                
                var organizationData = GetOrganizationData(orgId);
                if (organizationData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Такой семьи не существует", 3000);
                    return;
                }
                
                if (!organizationData.Status)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Эта семья не активна", 3000);
                    return;
                }
                
                if (status == 1)
                {
                    if (organizationData.CrimeOptions)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У этой семьи уже активны нелегальные возможности.", 3000);
                        return;
                    }

                    organizationData.CrimeOptions = true;
                    organizationData.DefaultAccess.Add(RankToAccess.OrgCrime);
                    organizationData.DefaultAccess.Add(RankToAccess.InCar);
                    organizationData.DefaultAccess.Add(RankToAccess.Pull);
                    organizationData.DefaultAccess.Add(RankToAccess.Cuff);
                    organizationData.DefaultAccess.Add(RankToAccess.FamilyZone);
                    organizationData.DefaultAccess.Add(RankToAccess.IsWar);
                    
                    organizationData.SaveCrimeOptions();
                    
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы включили нелегальные возможности для семьи #{orgId}.", 3000);
                }
                else if (status == 0)
                {
                    if (!organizationData.CrimeOptions)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У этой семьи отключены нелегальные возможности.", 3000);
                        return;
                    }

                    organizationData.CrimeOptions = false;
                    organizationData.DefaultAccess.Remove(RankToAccess.OrgCrime);
                    organizationData.DefaultAccess.Remove(RankToAccess.InCar);
                    organizationData.DefaultAccess.Remove(RankToAccess.Pull);
                    organizationData.DefaultAccess.Remove(RankToAccess.Cuff);
                    organizationData.DefaultAccess.Remove(RankToAccess.FamilyZone);
                    organizationData.DefaultAccess.Remove(RankToAccess.IsWar);

                    organizationData.SaveCrimeOptions();
                    
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы отключили нелегальные возможности для семьи #{orgId}.", 3000);
                }
                
                GameLog.Admin($"{player.Name}", $"setfamily(FamilyID:{orgId},Status:{status})", $"null");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_setfamily Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.setbliporg)]
        public static void CMD_SetOrganizationBlip(ExtPlayer player, int orgId, int sprite, byte color)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.setbliporg)) return;
                var organizationData = GetOrganizationData(orgId);
                if (organizationData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Такой семьи не существует", 3000);
                    return;
                }
                if (!organizationData.Status)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Эта семья не активна", 3000);
                    return;
                }
                if (sprite <= 7 || sprite >= 752)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Данный Sprite нанести невозможно", 3000);
                    return;
                }
                if (color  < 1 || color > 85)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Данный цвет нанести невозможно", 3000);
                    return;
                }
                if(organizationData.Blip != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У семьи уже есть Blip", 3000);
                    return;
                }

                var position = player.Position;
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.Organizations
                            .Where(o => o.Organization == orgId)
                            .Set(o => o.BlipID, (sbyte) sprite)
                            .Set(o => o.BlipColor, color)
                            .Set(o => o.BlipXYZ, JsonConvert.SerializeObject(position))
                            .UpdateAsync();
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
                
                organizationData.BlipId = sprite;
                organizationData.BlipColor = color;
                organizationData.BlipPosition = player.Position;
                organizationData.Blip = (ExtBlip) NAPI.Blip.CreateBlip(organizationData.BlipId, organizationData.BlipPosition, 1, organizationData.BlipColor, organizationData.Name, 255, 0, true, 0, 0);
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Метка для семьи {organizationData.Name} установлена.", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_SetOrganizationBlip Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.delbliporg)]
        public static void CMD_DeleteOrganizationBlip(ExtPlayer player, int orgId)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.delbliporg)) return;
                var organizationData = GetOrganizationData(orgId);
                if (organizationData == null) 
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Такой семьи не существует", 3000);
                    return;
                }
                if (!organizationData.Status)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Эта семья не активна", 3000);
                    return;
                }
                if (organizationData.Blip == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У семьи нет Blip", 3000);
                    return;
                }
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.Organizations
                            .Where(o => o.Organization == orgId)
                            .Set(o => o.BlipID, -1)
                            .UpdateAsync();
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
                
                if (organizationData.Blip != null && organizationData.Blip.Exists) 
                    organizationData.Blip.Delete();
                
                organizationData.Blip = null;
                organizationData.BlipId = -1;
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Метка семьи {organizationData.Name} удалена.", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_DeleteOrganizationBlip Exception: {e.ToString()}");
            }
        }

        public static string GenerateNumber(int orgid)
        {
            var organizationData = GetOrganizationData(orgid);
            if (organizationData == null) return "";

            var vehiclesNumbers = Manager.Organizations.Values
                .Select(o => o.Vehicles.Keys.ToString())
                .ToList();
            
            
            string number;
            do
            {
                number = "";
                number += "O";
                number += (char)Rnd.Next(65, 90); 
                for (int i = 0; i < 4; i++) number += (char)Rnd.Next(48, 57); 
                number += (char)Rnd.Next(65, 90);

            } while (vehiclesNumbers.Contains(number));
            return number;
        }
        public static void SetFracRankOffline(ExtPlayer player, int uuid, int rank)
        {
            try
            {
                if (!player.IsOrganizationAccess(RankToAccess.SetRank))
                    return;
            
                if (rank < 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нельзя установить отрицательный или нулевой ранг", 3000);
                    return;
                }

                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberOrganizationData == null) 
                    return;

                var organizationData = GetOrganizationData(memberOrganizationData.Id);
                if (organizationData == null) 
                    return;
                
                var targetMemberOrganizationData = GetOrganizationMemberData(uuid, memberOrganizationData.Id);
                if (targetMemberOrganizationData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрока не найдено в Вашей семье.", 3000);
                    return;
                }
                
                if (targetMemberOrganizationData.Rank >= memberOrganizationData.Rank)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете изменить ранг игроку, у которого ранг выше, чем ваш собственный.", 8000);
                    return;
                }
                
                if (rank >= memberOrganizationData.Rank)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете установить такой ранг", 3000);
                    return;
                }
                if (!organizationData.Ranks.ContainsKey(rank))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ранг не найден", 3000);
                    return;
                }


                if (targetMemberOrganizationData.Rank > rank) Table.Logs.Repository.AddLogs(player, OrganizationLogsType.SetRank, $"Понизил оффлайн {targetMemberOrganizationData.Name} ({memberOrganizationData.UUID}) в должности ({targetMemberOrganizationData.Rank} -> {rank})");
                else Table.Logs.Repository.AddLogs(player, OrganizationLogsType.SetRank, $"Повысил оффлайн {targetMemberOrganizationData.Name} ({memberOrganizationData.UUID}) в должности ({targetMemberOrganizationData.Rank} -> {rank})");

                Player.Repository.SetRank(organizationData.Id, uuid, rank);
                
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы изменили ранг игрока {targetMemberOrganizationData.Name} на {organizationData.Ranks[rank].Name}", 6000);
            
            }
            catch (Exception e)
            {
                Log.Write($"SetFracRankOffline Exception: {e.ToString()}");
            }
        }

        public static void SetFracRank(ExtPlayer player, ExtPlayer target, int newRank)
        {
            try
            {
                if (!player.IsOrganizationAccess(RankToAccess.SetRank))
                    return;
                
                if (newRank < 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нельзя установить отрицательный или нулевой ранг", 3000);
                    return;
                }
                
                if (player == target) return;
                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberOrganizationData == null) 
                    return;

                var organizationData = GetOrganizationData(memberOrganizationData.Id);
                if (organizationData == null) 
                    return;
                
                var targetMemberOrganizationData = target.GetOrganizationMemberData();
                if (targetMemberOrganizationData == null) 
                    return;
                
                if (memberOrganizationData.Id != targetMemberOrganizationData.Id) 
                    return;
                
                if (newRank >= memberOrganizationData.Rank)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantUpToRank), 3000);
                    return;
                }
                if (targetMemberOrganizationData.Rank >= memberOrganizationData.Rank)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете повысить этого игрока", 3000);
                    return;
                }
                if (!organizationData.Ranks.ContainsKey(newRank))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ранг не найден", 3000);
                    return;
                }

                if (targetMemberOrganizationData.Rank > newRank) Table.Logs.Repository.AddLogs(player, OrganizationLogsType.SetRank, $"Понизил {target.Name} ({targetMemberOrganizationData.UUID}) в должности ({targetMemberOrganizationData.Rank} -> {newRank})");
                else Table.Logs.Repository.AddLogs(player, OrganizationLogsType.SetRank, $"Повысил {target.Name} ({targetMemberOrganizationData.UUID}) в должности ({targetMemberOrganizationData.Rank} -> {newRank})");
                //MemberData mdata = AllMembers.FirstOrDefault(p => p.Name == targetname && p.OrganizationID == orgId);
                //AllMembers.Remove(mdata);

                Player.Repository.SetRank(organizationData.Id, target.GetUUID(), newRank);
                
                Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, $"Теперь вы {organizationData.Ranks[newRank].Name} в семье", 6000);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы изменили ранг игрока {target.Name} на {organizationData.Ranks[newRank].Name}", 6000);
                
            }
            catch (Exception e)
            {
                Log.Write($"SetFracRank Exception: {e.ToString()}");
            }
        }


        public static void InviteToOrganization(ExtPlayer sender, ExtPlayer target)
        {
            try
            {
                if (!sender.IsOrganizationAccess(RankToAccess.Invite))
                    return;
                
                var memberOrganizationData = sender.GetOrganizationMemberData();
                if (memberOrganizationData == null)
                {
                    Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не состоите в семье", 3000);
                    return;
                }
                
                var organizationData = GetOrganizationData(memberOrganizationData.Id);
                if (organizationData == null)
                {
                    Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не состоите в семье", 3000);
                    return;
                }
                
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) 
                    return;
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) 
                    return;

                if (sender.Position.DistanceTo(target.Position) > 3)
                {
                    Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок слишком далеко от Вас", 3000);
                    return;
                }
                //if (Members.ContainsKey(target) || Fractions.Manager.isHaveFraction(target))
                if (target.IsOrganizationMemberData())
                {
                    Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок уже состоит в семье", 3000);
                    return;
                }
                if (targetCharacterData.AdminLVL >= 1 && targetCharacterData.AdminLVL <= 7) return;
                if (targetCharacterData.LVL < 1)
                {
                    Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, "Необходим как минимум 1 уровень для приглашcения игрока в семью", 3000);
                    return;
                }
                if (targetCharacterData.Warns > 0)
                {
                    Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, "Невозможно принять этого игрока", 3000);
                    return;
                }
                targetSessionData.InviteData.Organization = organizationData.Id;
                targetSessionData.InviteData.Sender = sender;
                Trigger.ClientEvent(target, "openDialog", "INVITED_ORG", $"{sender.Name} пригласил Вас в семью {organizationData.Name}");
                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы пригласили в семью {target.Name}", 3000);
                //Chars.Repository.PlayerStats(target);

            }
            catch (Exception e)
            {
                Log.Write($"InviteToOrganization Exception: {e.ToString()}");
            }
        }

        public static void UnInviteFromOrganization(ExtPlayer sender, ExtPlayer target)
        {
            try
            {
                if (!sender.IsOrganizationAccess(RankToAccess.UnInvite)) return;
                
                var memberOrganizationData = sender.GetOrganizationMemberData();
                if (memberOrganizationData == null) 
                    return;
                
                var organizationData = GetOrganizationData(memberOrganizationData.Id);
                if (organizationData == null) 
                    return;
                
                var targetMemberOrganizationData = target.GetOrganizationMemberData();
                if (targetMemberOrganizationData == null)
                    return;
                
                if (memberOrganizationData.Id != targetMemberOrganizationData.Id) return;
                if (memberOrganizationData.Rank <= targetMemberOrganizationData.Rank)
                {
                    Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете выгнать этого игрока", 3000);
                    return;
                }
                
                NeptuneEvo.Organizations.Player.Repository.RemoveOrganizationMemberData(memberOrganizationData.Id, target.GetUUID());

                Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, $"Вас выгнали из семьи {organizationData.Name}", 3000);
                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы выгнали из семьи {target.Name}", 3000);
                Table.Logs.Repository.AddLogs(sender, OrganizationLogsType.UnInvite, $"Выгнал {target.Name} ({target.GetUUID()})");
            }
            catch (Exception e)
            {
                Log.Write($"UnInviteFromOrganization Exception: {e.ToString()}");
            }
        }

        public static bool StockBuy(ExtPlayer player)
        {
            try
            {
                if (!player.IsOrganizationAccess(RankToAccess.OrgUpgrate)) return false;
                
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не состоите в семье", 3000);
                    return false;
                }
                if (organizationData.Stock)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Склад уже установлен для Вашей семьи.", 3000);
                    return false;
                }
                if (UpdateData.CanIChange(player, Main.PricesSettings.StockPrice, true) != 255) return false;
                MoneySystem.Wallet.Change(player, -Main.PricesSettings.StockPrice);
                organizationData.Stock = true;
                organizationData.DefaultAccess.Add(RankToAccess.OpenStock);
                
                if (organizationData.StockLabel != null && organizationData.StockLabel.Exists) 
                    organizationData.StockLabel.Delete();
                
                organizationData.StockLabel = (ExtTextLabel) NAPI.TextLabel.CreateTextLabel(Main.StringToU16($"~y~Склад {organizationData.Name}"), StockPositions[organizationData.OfficeUpgrade], 5F, 0.5F, 0, new Color(255, 255, 255), true, organizationData.GetDimension());

                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.Organizations
                            .Where(o => o.Organization == organizationData.Id)
                            .Set(o => o.Stock, (sbyte) 1)
                            .UpdateAsync();
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
                
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили склад для семьи!", 3000);
                GameLog.Money($"player({player.GetUUID()})", $"server", Main.PricesSettings.StockPrice, $"OrgStockBuy({organizationData.Id})");
                return true;
            }
            catch (Exception e)
            {
                Log.Write($"StockBuy Exception: {e.ToString()}");
            }
            return false;
        }
        public static bool UpdateCrimeOptions(ExtPlayer player) 
        {
            try
            {
                var accountData = player.GetAccountData();
                if (accountData == null) 
                    return false;
                
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не состоите в семье", 3000);
                    return false;
                }
                if (!organizationData.IsLeader(player.GetUUID()))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Склад уже установлен для Вашей семьи.", 3000);
                    return false;
                }
                if (accountData.RedBucks < Main.PricesSettings.UpdateTypeOrganization)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У Вас не хватает RedBucks для улучшения семьи.", 3000);
                    return false;
                }
                UpdateData.RedBucks(player, -Main.PricesSettings.UpdateTypeOrganization, msg:"Улучшение семьи");
                GameLog.Money($"player({player.GetUUID()})", $"server", Main.PricesSettings.UpdateTypeOrganization, $"OrgUpgrade2({organizationData.Id})");

                organizationData.CrimeOptions = !organizationData.CrimeOptions;

                if (organizationData.CrimeOptions)
                {
                    organizationData.DefaultAccess.Add(RankToAccess.OrgCrime);
                    organizationData.DefaultAccess.Add(RankToAccess.InCar);
                    organizationData.DefaultAccess.Add(RankToAccess.Pull);
                    organizationData.DefaultAccess.Add(RankToAccess.Cuff);
                    organizationData.DefaultAccess.Add(RankToAccess.FamilyZone);
                    organizationData.DefaultAccess.Add(RankToAccess.IsWar);
                }
                else
                {
                    organizationData.DefaultAccess.Remove(RankToAccess.OrgCrime);
                    organizationData.DefaultAccess.Remove(RankToAccess.InCar);
                    organizationData.DefaultAccess.Remove(RankToAccess.Pull);
                    organizationData.DefaultAccess.Remove(RankToAccess.Cuff);
                    organizationData.DefaultAccess.Remove(RankToAccess.FamilyZone);
                    organizationData.DefaultAccess.Remove(RankToAccess.IsWar);
                }
                
                organizationData.SaveCrimeOptions();
                
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы сменили тип организации!", 3000);
                return true;
            }
            catch (Exception e)
            {
                Log.Write($"CMD_setfamily Exception: {e.ToString()}");
            }
            return false;
        }
        public static bool SchemeBuy(ExtPlayer player, string scheme, int price = -1)
        {
            try
            {
                if (!player.IsOrganizationAccess(RankToAccess.OrgUpgrate)) return false;

                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не состоите в семье", 3000);
                    return false;
                }
                var orgSchemes = organizationData.Schemes;
                
                if (!orgSchemes.ContainsKey(scheme) || orgSchemes[scheme])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Эта схема уже куплена в Вашей семье.", 3000);
                    return false;
                }
                else if (UpdateData.CanIChange(player, price, true) != 255) return false; 
                MoneySystem.Wallet.Change(player, -price);

                orgSchemes[scheme] = true;
                
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.Organizations
                            .Where(o => o.Organization == organizationData.Id)
                            .Set(o => o.PistolScheme, Convert.ToSByte(orgSchemes["Pistol"]))
                            .Set(o => o.PistolMk2Scheme, Convert.ToSByte(orgSchemes["PistolMk2"]))
                            .Set(o => o.Pistol50Scheme, Convert.ToSByte(orgSchemes["Pistol50"]))
                            .Set(o => o.HeavyPistolScheme, Convert.ToSByte(orgSchemes["HeavyPistol"]))
                            .Set(o => o.PumpShotgunScheme, Convert.ToSByte(orgSchemes["PumpShotgun"]))
                            .Set(o => o.DoubleBarrelShotgunScheme, Convert.ToSByte(orgSchemes["DoubleBarrelShotgun"]))
                            .Set(o => o.SawnOffShotgunScheme, Convert.ToSByte(orgSchemes["SawnOffShotgun"]))
                            .Set(o => o.MiniSMGScheme, Convert.ToSByte(orgSchemes["MiniSMG"]))
                            .Set(o => o.SMGMk2Scheme, Convert.ToSByte(orgSchemes["SMGMk2"]))
                            .Set(o => o.MachinePistolScheme, Convert.ToSByte(orgSchemes["MachinePistol"]))
                            .Set(o => o.MicroSMGScheme, Convert.ToSByte(orgSchemes["MicroSMG"]))
                            .Set(o => o.CombatPDWScheme, Convert.ToSByte(orgSchemes["CombatPDW"]))
                            .Set(o => o.CompactRifleScheme, Convert.ToSByte(orgSchemes["CompactRifle"]))
                            .Set(o => o.AssaultRifleScheme, Convert.ToSByte(orgSchemes["AssaultRifle"]))
                            .Set(o => o.ArmorScheme, Convert.ToSByte(orgSchemes["Armor"]))
                            .UpdateAsync();
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
                
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили чертёж для семьи!", 3000);
                GameLog.Money($"player({player.GetUUID()})", $"server", Main.PricesSettings.StockPrice, $"OrgScheme{scheme}Buy({organizationData.Id})");
                return true;
            }
            catch (Exception e)
            {
                Log.Write($"SchemeBuy Exception: {e.ToString()}");
            }
            return false;
        }
        public static async Task SaveOrganizations(ServerBD db)
        {
            try
            {
                foreach (var organizationData in Organizations.Values)
                {
                    try
                    {
                        if (!organizationData.Status) continue;
                        await SaveOrgsVehicles(db, organizationData.Id);
                        await organizationData.Save(db);
                    }
                    catch (Exception e)
                    {
                        Log.Write($"SaveOrganizations Foreach Exception: {e.ToString()}");
                    }
                }
                Log.Write("Organization Vehicles and Stocks has been saved to DB", nLog.Type.Success);
            }
            catch (Exception e)
            {
                Log.Write($"SaveOrganizations Exception: {e.ToString()}");
            }
        }

        public static void NewDay()
        {
            try
            {
                foreach (var orgId in Organizations.Keys.ToList())
                {
                    try
                    {
                        var organizationData = GetOrganizationData(orgId);
                        if (organizationData == null) 
                            continue;
                        
                        var sec = NeptuneEvo.Table.Tasks.Repository.GetTime();
                        foreach (var time in organizationData.AttackingCount.Keys.ToList())
                        {
                            if (sec > time && organizationData.AttackingCount.ContainsKey(time))
                                organizationData.AttackingCount.Remove(time);
                        }
                        
                        foreach (var time in organizationData.ProtectingCount.Keys.ToList())
                        {
                            if (sec > time && organizationData.ProtectingCount.ContainsKey(time))
                                organizationData.ProtectingCount.Remove(time);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Write($"NewDay Foreach Exception: {e.ToString()}");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"NewDay Exception: {e.ToString()}");
            }
        }
        private static async Task SaveOrgsVehicles(ServerBD db, int orgId)
        {
            try
            {
                var organizationData = GetOrganizationData(orgId);
                if (organizationData == null) 
                    return;
                
                if (!organizationData.Status) 
                    return;

                foreach (var number in organizationData.Vehicles.Keys)
                {
                    var vehicle = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Organization, number);
                    if (vehicle == null) continue;
                    var vehicleLocalData = vehicle.GetVehicleLocalData();
                    if (vehicleLocalData == null) continue;
                    var vehicleStateData = vehicle.GetVehicleLocalStateData();
                    if (vehicleStateData == null) continue;

                    await db.Orgvehicles
                        .Where(o => o.Number == vehicleLocalData.NumberPlate)
                        .Set(o => o.Dirt, vehicleStateData.Dirt)
                        .Set(o => o.Petrol, vehicleLocalData.Petrol)
                        .UpdateAsync();
                }
            }
            catch (Exception e)
            {
                Log.Write($"SaveOrgsVehicles Exception: {e.ToString()}");
            }
        }

        public static bool UpgradeOrganization(ExtPlayer player)
        {
            try
            {
                if (!player.IsOrganizationAccess(RankToAccess.OrgUpgrate)) 
                    return false;
                
                var accountData = player.GetAccountData();
                if (accountData == null) 
                    return false;
                
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return false;
                
                var upgraded = organizationData.OfficeUpgrade;
                if (upgraded >= 2) return false;
                if (player.IsInVehicle)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не должны находиться в транспорте во время покупки улучшения.", 3000);
                    return false;
                }
                if (upgraded == 0)
                {
                    if (!MoneySystem.Wallet.Change(player, -Main.PricesSettings.FirstOrgPrice))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У Вас не хватает денег для улучшения семьи.", 3000);
                        return false;
                    }
                    GameLog.Money($"player({player.GetUUID()})", $"server", Main.PricesSettings.FirstOrgPrice, $"OrgUpgrade({organizationData.Id})");
                }
                else if (upgraded == 1)
                {
                    if (accountData.RedBucks < Main.PricesSettings.SecondOrgPrice)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У Вас не хватает RedBucks для улучшения семьи.", 3000);
                        return false;
                    }
                    UpdateData.RedBucks(player, -Main.PricesSettings.SecondOrgPrice, msg:"Улучшение семьи");
                    GameLog.Money($"player({player.GetUUID()})", $"server", Main.PricesSettings.SecondOrgPrice, $"OrgUpgrade2({organizationData.Id})");
                }
                
                if (organizationData.StockLabel != null && organizationData.StockLabel.Exists) 
                    organizationData.StockLabel.Delete();
                
                organizationData.OfficeUpgrade++;

                if (organizationData.Stock) 
                    organizationData.StockLabel = (ExtTextLabel) NAPI.TextLabel.CreateTextLabel(Main.StringToU16($"~y~Склад {organizationData.Name}"), StockPositions[organizationData.OfficeUpgrade], 5F, 0.5F, 0, new Color(255, 255, 255), true, organizationData.GetDimension());
                else 
                    organizationData.StockLabel = (ExtTextLabel) NAPI.TextLabel.CreateTextLabel(Main.StringToU16($"~y~Склад {organizationData.Name}\n~r~Не активен"), StockPositions[organizationData.OfficeUpgrade], 5F, 0.5F, 0, new Color(255, 255, 255), true, organizationData.GetDimension());
                

                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.Organizations
                            .Where(o => o.Organization == organizationData.Id)
                            .Set(o => o.OfficeUP, (sbyte) organizationData.OfficeUpgrade)
                            .UpdateAsync();
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
                
                foreach (var foreachPlayer in Character.Repository.GetPlayers())
                {
                    var foreachCharacterData = foreachPlayer.GetCharacterData();
                    if (foreachCharacterData == null) continue;
                    if (foreachCharacterData.InsideOrganizationID == organizationData.Id && !foreachPlayer.IsInVehicle)
                    {
                        SeatingArrangements.LandingEnd(foreachPlayer);
                        SendPlayer(foreachPlayer, true);
                    }
                }
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы улучшили семью!", 3000);
                GameLog.Money($"player({player.GetUUID()})", $"server", Main.PricesSettings.StockPrice, $"OrgStockBuy({organizationData.Id})");
                return true;
            }
            catch (Exception e)
            {
                Log.Write($"UpgradeOrganization Exception: {e.ToString()}");
            }

            return false;
        }
        [RemoteEvent("server.org.create.buy")]
        public static void CreateOrganization(ExtPlayer player, bool isCrime, string orgName)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (DateTime.Now < sessionData.TimingsData.NextGlobalChat)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Block10Min), 4500);
                    return;
                }
                if (player.IsOrganizationMemberData()) return;
                
                if (characterData.AdminLVL == 1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недоступно для администрации.", 3000);
                    Trigger.ClientEvent(player, "client.org.create.close");
                    return;
                }
                
                if (characterData.Warns >= 1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Для создания семьи Вы не должны иметь предупреждений (Warns).", 3000);
                    Trigger.ClientEvent(player, "client.org.create.close");
                    return;
                }

                orgName = Main.BlockSymbols(Main.RainbowExploit(orgName));
                if (orgName.Length < 3 || orgName.Length > 30)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxRankNameLenght), 4500);
                    return;
                }
                
                string testmsg = orgName.ToLower();
                if (Main.ServerNumber != 0 && (testmsg.Equals("admins") || testmsg.Equals("admin") || testmsg.Equals("administrator") || testmsg.Equals("administrators"))) return;
                if (Main.stringGlobalBlock.Any(c => testmsg.Contains(c)))
                {
                    sessionData.TimingsData.NextGlobalChat = DateTime.Now.AddMinutes(10);
                    Trigger.SendToAdmins(3, "!{#636363}[A] " + LangFunc.GetText(LangType.Ru, DataName.AdminAlertFTableNews, player.Name, player.Value, orgName));
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RestrictedWordsTableNews), 15000);
                    Trigger.ClientEvent(player, "client.org.create.close");
                    return;
                }
                
                var organizationData = Organizations.Values.FirstOrDefault(p => p.Name == orgName);
                if (organizationData != null && organizationData.Status)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"К сожалению, семья с названием {orgName} уже существует.", 3000);
                    return;
                }
                if (!MoneySystem.Wallet.Change(player, -Main.PricesSettings.CreateOrgPrice))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У Вас не хватает денег для создания семьи.", 3000);
                    Trigger.ClientEvent(player, "client.org.create.close");
                    return;
                }
                
                Trigger.ClientEvent(player, "client.org.create.close");
                
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        var date = DateTime.Now;
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        var id = await db.InsertWithInt32IdentityAsync(new global::Database.Organizations
                        {
                            OwnerUUID = characterData.UUID,
                            Name = orgName,
                            OfficeUP = 0,
                            Customs = 0,
                            Stock = 0,
                            CrimeOptions = Convert.ToSByte(isCrime),
                            PistolScheme = 0,
                            PistolMk2Scheme = 0,
                            Pistol50Scheme = 0,
                            HeavyPistolScheme = 0,
                            PumpShotgunScheme = 0,
                            DoubleBarrelShotgunScheme = 0,
                            SawnOffShotgunScheme = 0,
                            MiniSMGScheme = 0,
                            SMGMk2Scheme = 0,
                            MachinePistolScheme = 0,
                            MicroSMGScheme = 0,
                            CombatPDWScheme = 0,
                            CompactRifleScheme = 0,
                            AssaultRifleScheme = 0,
                            ArmorScheme = 0,
                            Drugs = 0,
                            Mats = 0,
                            MedKits = 0,
                            Money = 0,
                            Weapons = "[]",
                            IsOpen = 0,
                            Status = 1,
                            BlipID = -1,
                            BlipColor = 0,
                            BlipXYZ = JsonConvert.SerializeObject(new Vector3()),
                            Ranks = JsonConvert.SerializeObject(OrganizationDefaultRanks),
                            Departments = "{}",
                            Discord = "",
                            Salary = 0,
                            Color = "{}",
                            Date = date,
                            Slogan = "",
                            AttackingCount = "{}",
                            ProtectingCount = "{}"
                        });
                        
                        Trigger.SetMainTask(() =>
                        {
                            try
                            {
                                var organizationData = new OrganizationData
                                {
                                    Id = id,
                                    OwnerUUID = characterData.UUID,
                                    Name = orgName,
                                    Status = true,
                                    Ranks = OrganizationDefaultRanks.Clone(),
                                    DefaultAccess = OrganizationDefaultAccess.ToList(),
                                    Date = date,
                                    Discord = "",
                                    Color = new Color()
                                };
                                organizationData.CrimeOptions = isCrime;
                                if (organizationData.CrimeOptions)
                                {
                                    organizationData.DefaultAccess.Add(RankToAccess.OrgCrime);
                                    organizationData.DefaultAccess.Add(RankToAccess.InCar);
                                    organizationData.DefaultAccess.Add(RankToAccess.Pull);
                                    organizationData.DefaultAccess.Add(RankToAccess.Cuff);
                                    organizationData.DefaultAccess.Add(RankToAccess.FamilyZone);
                                    organizationData.DefaultAccess.Add(RankToAccess.IsWar);
                                }


                                CustomColShape.CreateCylinderColShape(new Vector3(-1396.274, -480.7186, 57.100), 2f, 2f, organizationData.GetDimension(), ColShapeEnums.Organizations, 4);
                                organizationData.StockLabel = (ExtTextLabel) NAPI.TextLabel.CreateTextLabel(Main.StringToU16($"~y~Склад {orgName}\n~r~Не активен"), StockPositions[0], 5F, 0.5F, 0, new Color(255, 255, 255), true, organizationData.GetDimension());
                            
                                Organizations.Add(id, organizationData);
                                
                                player.AddOrganizationMemberData(id, 2);
                   
                                SendPlayer(player);
                                NAPI.Chat.SendChatMessageToAll("!{#50C878}" + $"В штате была зарегистрирована новая семья - {orgName}!");
                                
                                EventSys.SendCoolMsg(player,"Семья", "Создание семьи", $"Поздравляем Вас с регистрацией собственной семьи - {orgName}!", "", 10000);
                                //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Поздравляем Вас с регистрацией собственной семьи - {orgName}!", 5000);
                                GameLog.Money($"player({characterData.UUID})", $"server", Main.PricesSettings.CreateOrgPrice, $"CreateOrg({id})");
                            }
                            catch (Exception e)
                            {
                                Debugs.Repository.Exception(e);
                            }
                        });
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
                
            }
            catch (Exception e)
            {
                Log.Write($"CreateOrganization Exception: {e.ToString()}");
            }
        }        

        public static void UpdateForMembers(int orgId, OrganizationOfficeTypeUpdate type, int count, bool unload = true, bool load = true)
        {
            try
            {
                var organizationData = GetOrganizationData(orgId);
                if (organizationData == null) return;
                if (!organizationData.Status) return;

                var interiorId = OrganizationTypeList[organizationData.OfficeUpgrade].Item3;
                foreach (var foreachPlayer in Character.Repository.GetPlayers())
                {
                    var foreachCharacterData = foreachPlayer.GetCharacterData();
                    if (foreachCharacterData == null) continue;
                    if (foreachCharacterData.InsideOrganizationID == orgId) 
                        Trigger.ClientEvent(foreachPlayer, "OfficePropLoad", interiorId, type, count, unload, load);
                }
            }
            catch (Exception e)
            {
                Log.Write($"UpdateForMembers Exception: {e.ToString()}");
            }
        }
        public static void OpenOrgStock(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) return;
                
                List<string> counter = new List<string>
                {
                    organizationData.Money.ToString(),
                    organizationData.MedKits.ToString(),
                    organizationData.Drugs.ToString(),
                    organizationData.Materials.ToString(),
                    Chars.Repository.getCountToStockItems($"Organization_{organizationData.Id}"),
                };
                
                string json = JsonConvert.SerializeObject(counter);
                Trigger.ClientEvent(player, "openStock", json);
                sessionData.IsOrgStockActive = true;
            }
            catch (Exception e)
            {
                Log.Write($"OpenOrgStock Exception: {e.ToString()}");
            }
        }
        public static void CraftBodyArmor(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.InsideOrganizationID == -1) return;

                var organizationData = player.GetOrganizationData();
                if (organizationData == null) return;
                if (!organizationData.Status) return;
                if (!organizationData.Schemes["Armor"])
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Недоступно для вашей семьи.", 5000);
                    return;
                }
                if (Chars.Repository.itemCount(player, "inventory", ItemId.BodyArmor) >= Chars.Repository.maxItemCount)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyHaveBronik), 3000);
                    return;
                }
                ItemStruct mItem = Chars.Repository.isItem(player, "inventory", ItemId.Material);
                int count = (mItem == null) ? 0 : mItem.Item.Count;
                if (count < 400)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMats), 3000);
                    return;
                }
                if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.BodyArmor, 1, "100") == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                    return;
                }
                Chars.Repository.RemoveIndex(player, mItem.Location, mItem.Index, 400);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы скрафтили бронежилет с 100% прочностью.", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CraftBodyArmor Exception: {e.ToString()}");
            }
        }
        public static void OpenStock(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.InsideOrganizationID == -1) return;
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
                if (!organizationData.Status) return;
                if (!organizationData.IsOpenStock)
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Склад семьи закрыт.", 3000);
                    return;
                }
                OpenOrgStock(player);
            }
            catch (Exception e)
            {
                Log.Write($"OpenStock Exception: {e.ToString()}");
            }
        }

        public static void OpenWeaponStock(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;

                if (!organizationData.Status) return;
                if (!organizationData.IsOpenStock)
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Склад семьи закрыт.", 3000);
                    return;
                }
                if (organizationData.Id != characterData.InsideOrganizationID) return;

                Chars.Repository.LoadOtherItemsData(player, "Organization", organizationData.Id.ToString(), 6, Chars.Repository.InventoryMaxSlots["Organization"]);
            }
            catch (Exception e)
            {
                Log.Write($"OpenWeaponStock Exception: {e.ToString()}");
            }
        }

        public static void inputStocks(ExtPlayer player, string action, int amount)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
                if (!organizationData.Status) return;
                if (!organizationData.IsOpenStock)
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Склад семьи закрыт.", 3000);
                    return;
                }
                if (organizationData.Id != characterData.InsideOrganizationID) return;
                switch (action)
                {
                    case "put_stock":
                        string item = sessionData.SelectData.SelectedStock;
                        int stockContains = 0;
                        int playerHave = 0;
                        if (item == "mats")
                        {
                            stockContains = organizationData.Materials;
                            if (stockContains + amount > MaxMats)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseTooMuch), 3000);
                                return;
                            }
                            playerHave = Chars.Repository.getCountToLacationItem($"char_{characterData.UUID}", "inventory", ItemId.Material);
                        }
                        else if (item == "drugs")
                        {
                            stockContains = organizationData.Drugs;
                            if (stockContains + amount > 10000)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NarkoWarehouseTooMuch), 3000);
                                return;
                            }
                            playerHave = Chars.Repository.getCountToLacationItem($"char_{characterData.UUID}", "inventory", ItemId.Drugs);
                        }
                        else if (item == "money")
                        {
                            stockContains = organizationData.Money;
                            playerHave = (int)characterData.Money;
                        }
                        else if (item == "medkits")
                        {
                            stockContains = organizationData.MedKits;
                            playerHave = Chars.Repository.getCountToLacationItem($"char_{characterData.UUID}", "inventory", ItemId.HealthKit);
                        }

                        if (playerHave < amount)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoCoins), 3000);
                            return;
                        }

                        if (item == "mats")
                        {
                            var lastMulti = organizationData.MaterialsMultiplier();
                            organizationData.Materials += amount;
                            var newMulti = organizationData.MaterialsMultiplier();
                            if (lastMulti != newMulti) 
                                UpdateForMembers(organizationData.Id, OrganizationOfficeTypeUpdate.Materials, newMulti);
                            
                            Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", ItemId.Material, amount);
                            Table.Logs.Repository.AddLogs(player, OrganizationLogsType.TakeMats, $"Положил на склад материалы (x{amount})");
                        }
                        else if (item == "drugs")
                        {
                            var lastMulti = organizationData.DrugsMultiplier();
                            organizationData.Drugs += amount;
                            var newMulti = organizationData.DrugsMultiplier();
                            if (lastMulti != newMulti) 
                                UpdateForMembers(organizationData.Id, OrganizationOfficeTypeUpdate.Drugs, newMulti);
                            
                            Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", ItemId.Drugs, amount);
                            Table.Logs.Repository.AddLogs(player, OrganizationLogsType.TakeDrugs, $"Положил на склад наркотики (x{amount})");
                        }
                        else if (item == "money")
                        {
                            var lastMulti = organizationData.MoneyMultiplier();
                            organizationData.Money += amount;
                            var newMulti = organizationData.MoneyMultiplier();
                            if (lastMulti != newMulti) 
                                UpdateForMembers(organizationData.Id, OrganizationOfficeTypeUpdate.Money, newMulti);
                            
                            MoneySystem.Wallet.Change(player, -amount);
                            GameLog.Money($"player({characterData.UUID})", $"org({organizationData.Id})", amount, $"putStock");
                            Table.Logs.Repository.AddLogs(player, OrganizationLogsType.TakeMoney, $"Положил на склад деньги (${MoneySystem.Wallet.Format(amount)})");
                        }
                        else if (item == "medkits")
                        {
                            var lastMulti = organizationData.MedKitsMultiplier();
                            organizationData.MedKits += amount;
                            var newMulti = organizationData.MedKitsMultiplier();
                            if (lastMulti != newMulti) 
                                UpdateForMembers(organizationData.Id, OrganizationOfficeTypeUpdate.MedKits, newMulti);
                            
                            Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", ItemId.HealthKit, amount);
                            Table.Logs.Repository.AddLogs(player, OrganizationLogsType.TakeMedkits, $"Положил на склад аптечки (x{amount})");
                        }
                        //GameLog.Stock(cdata.FractionID, cdata.UUID, player.Name, item, amount, "in");
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"На складе осталось {stockContains + amount}, у Вас {playerHave - amount}", 3000);
                        break;
                    case "take_stock":
                        item = sessionData.SelectData.SelectedStock;
                        stockContains = 0;
                        playerHave = 0;
                        if (item == "mats")
                        {
                            stockContains = organizationData.Materials;
                            playerHave = Chars.Repository.getCountItem($"char_{characterData.UUID}", ItemId.Material, bagsToggled: false);
                            if (playerHave + amount > Chars.Repository.ItemsInfo[ItemId.Material].Stack)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.InventoryFilled), 3000);
                                return;
                            }
                        }
                        else if (item == "drugs")
                        {
                            stockContains = organizationData.Drugs;
                            playerHave = Chars.Repository.getCountItem($"char_{characterData.UUID}", ItemId.Drugs, bagsToggled: false);
                            if (playerHave + amount > 50)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.InventoryFilled), 3000);
                                return;
                            }
                        }
                        else if (item == "money")
                        {
                            stockContains = organizationData.Money;
                            playerHave = (int)characterData.Money;
                        }
                        else if (item == "medkits")
                        {
                            stockContains = organizationData.MedKits;
                            playerHave = Chars.Repository.getCountItem($"char_{characterData.UUID}", ItemId.HealthKit, bagsToggled: false);
                        }

                        if (stockContains < amount)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseEmptyNet), 3000);
                            return;
                        }

                        if (item == "mats")
                        {
                            if (Chars.Repository.isFreeSlots(player, ItemId.Material, amount) != 0) return;
                            var lastMulti = organizationData.MaterialsMultiplier();
                            organizationData.Materials -= amount;
                            var newMulti = organizationData.MaterialsMultiplier();
                            if (lastMulti != newMulti) 
                                UpdateForMembers(organizationData.Id, OrganizationOfficeTypeUpdate.Materials, newMulti);
                            
                            Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Material, amount);
                            Table.Logs.Repository.AddLogs(player, OrganizationLogsType.TakeMats, $"Взял со склада материалы (x{amount})");
                        }
                        else if (item == "drugs")
                        {
                            if (Chars.Repository.isFreeSlots(player, ItemId.Drugs, amount) != 0) return;
                            var lastMulti = organizationData.DrugsMultiplier();
                            organizationData.Drugs -= amount;
                            var newMulti = organizationData.DrugsMultiplier();
                            if (lastMulti != newMulti) 
                                UpdateForMembers(organizationData.Id, OrganizationOfficeTypeUpdate.Drugs, newMulti);
                            
                            Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Drugs, amount);
                            Table.Logs.Repository.AddLogs(player, OrganizationLogsType.TakeDrugs, $"Взял со склада наркотики (x{amount})");
                        }
                        else if (item == "money")
                        {
                            var lastMulti = organizationData.MoneyMultiplier();
                            organizationData.Money -= amount;
                            var newMulti = organizationData.MoneyMultiplier();
                            if (lastMulti != newMulti) 
                                UpdateForMembers(organizationData.Id, OrganizationOfficeTypeUpdate.Money, newMulti);
                            
                            MoneySystem.Wallet.Change(player, amount);
                            GameLog.Money($"org({organizationData.Id})", $"player({characterData.UUID})", amount, $"takeStock");
                            Table.Logs.Repository.AddLogs(player, OrganizationLogsType.TakeMoney, $"Взял со склада деньги (${MoneySystem.Wallet.Format(amount)})");
                        }
                        else if (item == "medkits")
                        {
                            if (Chars.Repository.isFreeSlots(player, ItemId.HealthKit, amount) != 0) return;
                            var lastMulti = organizationData.MedKitsMultiplier();
                            organizationData.MedKits -= amount;
                            var newMulti = organizationData.MedKitsMultiplier();
                            if (lastMulti != newMulti) 
                                UpdateForMembers(organizationData.Id, OrganizationOfficeTypeUpdate.MedKits, newMulti);
                            
                            Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.HealthKit, amount);
                            Table.Logs.Repository.AddLogs(player, OrganizationLogsType.TakeMedkits, $"Взял со склада аптечки (x{amount})");
                        }
                        //GameLog.Stock(cdata.FractionID, cdata.UUID, player.Name, item, amount, "out");
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"На складе осталось {stockContains - amount}, у Вас {playerHave + amount}", 3000);
                        break;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"inputStocks Exception: {e.ToString()}");
            }
        }
        
        public static void Perform(ExtPlayer player)
        {
            if (player.IsInVehicle) return;
            
                Trigger.ClientEvent(player, "client.org.create.init", Main.PricesSettings.CreateOrgPrice);
        }
        
        public static void Take(ExtPlayer player)
        {
            SendPlayer(player); // Вход в офис
        }
        
                [Interaction(ColShapeEnums.OrgCreate)]
        public static void Open(ExtPlayer player, int index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            if (sessionData.Following != null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomebodyYouFollow), 3000);
                return;
            }
            else if (sessionData.Follower != null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы кого-то тащите", 3000);
                return;
            }
            else if (Main.IHaveDemorgan(player, true)) return;
            
            player.SelectQuest(new PlayerQuestModel("npc_org", 0, 0, false, DateTime.Now));
            Trigger.ClientEvent(player, "client.quest.open", index, "npc_org", 0, 0, 0);
        }
        
        [Interaction(ColShapeEnums.Organizations)]
        public static void OnOrganizations(ExtPlayer player, int intid)
        {
            try
            {

                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (sessionData.Following != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomebodyYouFollow), 3000);
                    return;
                }
                else if (sessionData.Follower != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы кого-то тащите", 3000);
                    return;
                }
                switch (intid)
                {
                    case 1:
                        if (characterData.InsideOrganizationID == -1) return;
                        var organizationData = player.GetOrganizationData();
                        if (organizationData == null) 
                            return;
                        if (!organizationData.Status) return;
                        if (!organizationData.Schemes["Armor"])
                        {
                            Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Недоступно для вашей семьи.", 5000);
                            return;
                        }
                        Trigger.ClientEvent(player, "openDialog", "CONFIRM_BUY_ORGBODYARMOUR", "Вы действительно хотите скрафтить 100% бронежилет, стомостью 400 материалов?");
                        return;
                    case 2:
                        OpenStock(player);
                        return;
                    case 3:
                        SetVehicleIntoGarage(player);
                        return;
                    case 4:
                        RemovePlayerFromGarage(player); // Выход из гаража
                        return;
                    case 5:
                        if (player.IsInVehicle) return;
                        SendPlayerToGarage(player); // Телепорт в гараж
                        return;
                    case 6:
                        if (player.IsInVehicle) return;
                        RemovePlayer(player); // Выход из офиса
                        return;
                    case 7:
                        if (player.IsInVehicle) return;
                        if (!player.IsOrganizationMemberData()) // Создание своей организации
                        {
                            Trigger.ClientEvent(player, "client.org.create.init", Main.PricesSettings.CreateOrgPrice);
                            //Trigger.ClientEvent(player, "openDialog", "CreateOrg", $"Вы уверены, что хотите создать свою семью? Названием семьи служит Ваша фамилия, при создании за сумму {Main.PricesSettings.CreateOrgPrice}$ Вы получаете гараж на 5 т.с и офис начального уровня, а так же семейный чат (/fc /fcb).");
                        }
                        else SendPlayer(player); // Вход в офис
                        return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"Interaction Exception: {e.ToString()}");
            }
        }

        private static void callback_organizations(ExtPlayer player,  object listItem) /// Никитос Чини
        {
            try
            {
                if (!(listItem is string))
                    return;
                
                if (!player.IsCharacterData()) return;
                switch (listItem)
                {
                    case "createorg":
                        var frameList = new FrameListData();
                        frameList.Header = LangFunc.GetText(LangType.Ru, DataName.Famka);
                        frameList.Callback = callback_organizations1;

                        frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.OfficePreview), "ofprev"));

                        //frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.OfficeUpg), "ofprevinfo"));

                        frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.GaragePreview), "garprev"));

                        //frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.GarageUpg), "garprevinfo"));

                        frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru,DataName.CreatePrice, Main.PricesSettings.CreateOrgPrice), "createorg"));
                        
                        Players.Popup.List.Repository.Open(player, frameList); 
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"callback_organizations Exception: {e.ToString()}");
            }
        }
        private static void callback_organizations1(ExtPlayer player, object listItem) /// Никитос Чини
        {
            try
            {
                if (!(listItem is string))
                    return;

                if (!player.IsCharacterData()) return;
                
                switch (listItem)
                {
                    case "ofprev":
                        Trigger.ClientEvent(player, "setmyview", 1);
                        Trigger.Dimension(player, Dimensions.RequestPrivateDimension(player.Value));
                        Trigger.ClientEvent(player, "freeze", true);
                        player.Position = OrganizationTypeList[0].Item2;
                        break;
                    case "garprev":
                        Trigger.ClientEvent(player, "garageload");
                        Trigger.ClientEvent(player, "setmyview", 0);
                        Trigger.Dimension(player, Dimensions.RequestPrivateDimension(player.Value));
                        Trigger.ClientEvent(player, "freeze", true);
                        player.Position = new Vector3(-1385.035, -487.5486, 56.98049);
                        break;
                    case "createorg":
                        if (player.IsOrganizationMemberData())
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы уже находитесь в семье.", 5000);
                            return;
                        }
                        Trigger.ClientEvent(player, "openDialog", "CreateOrg", $"Вы уверены, что хотите создать свою семью? Названием семьи служит Ваша фамилия, при создании за сумму {Main.PricesSettings.CreateOrgPrice}$ Вы получаете гараж на 5 т.с и офис начального уровня, а так же семейный чат (/fc /fcb).");
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"callback_organizations1 Exception: {e.ToString()}");
            }
        }

        public static void SendPlayerToGarage(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
                Trigger.ClientEvent(player, "garageload");
                player.Position = new Vector3(-1396.274, -480.7186, 57.100);
                Trigger.Dimension(player, organizationData.GetDimension());
                characterData.InsideOrganizationID = organizationData.Id;
            }
            catch (Exception e)
            {
                Log.Write($"SendPlayerToGarage Exception: {e.ToString()}");
            }
        }
        public static void RemovePlayerFromGarage(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.InsideOrganizationID == -1) return;
                characterData.InsideOrganizationID = -1;
                SendPlayer(player);
            }
            catch (Exception e)
            {
                Log.Write($"RemovePlayerFromGarage Exception: {e.ToString()}");
            }
        }

        public static void SendPlayer(ExtPlayer player, bool force = false)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!force && characterData.InsideOrganizationID != -1) return;
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
                
                player.Position = OrganizationTypeList[organizationData.OfficeUpgrade].Item2;
                player.Rotation = new Vector3(0.0f, 0.0f, OrganizationTypeList[organizationData.OfficeUpgrade].Item1);
                
                Trigger.Dimension(player, organizationData.GetDimension());
                characterData.InsideOrganizationID = organizationData.Id;
                Trigger.ClientEvent(player, "OfficeAllPropLoad", OrganizationTypeList[organizationData.OfficeUpgrade].Item3, organizationData.MoneyMultiplier(), organizationData.DrugsMultiplier(), organizationData.MedKitsMultiplier(), organizationData.MaterialsMultiplier());
            }
            catch (Exception e)
            {
                Log.Write($"SendPlayer Exception: {e.ToString()}");
            }
        }
        public static void RemovePlayer(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.InsideOrganizationID == -1) return;
                player.Position = new Vector3(-774.1445, 311.0718, 85.69826);
                player.Rotation = new Vector3(0, 0, 175.971);
                Trigger.Dimension(player);
                characterData.InsideOrganizationID = -1;
            }
            catch (Exception e)
            {
                Log.Write($"RemovePlayer Exception: {e.ToString()}");
            }
        }

        public static OrganizationMemberData GetOrganizationMemberData(int uuid)
        {
            foreach (var members in AllMembers.Values.ToList())
            {
                var memberOrganizationData = members.FirstOrDefault(p => p.UUID == uuid);

                if (memberOrganizationData != null)
                    return memberOrganizationData;
            }

            return null;
        }
        public static OrganizationMemberData GetOrganizationMemberData(string name)
        {
            foreach (var members in AllMembers.Values.ToList())
            {
                var memberOrganizationData = members.FirstOrDefault(p => p.Name.Equals(name));

                if (memberOrganizationData != null)
                    return memberOrganizationData;
            }

            return null;
        }


        public static OrganizationMemberData GetOrganizationMemberData(int uuid, int orgId)
        {
            if (AllMembers.ContainsKey(orgId))
            {
                var memberOrganizationData = AllMembers[orgId].FirstOrDefault(p => p.UUID == uuid);

                if (memberOrganizationData != null)
                    return memberOrganizationData;
            }

            return null;
        }
        public static OrganizationMemberData GetOrganizationMemberData(string name, int orgId)
        {
            if (AllMembers.ContainsKey(orgId))
            {
                var memberOrganizationData = AllMembers[orgId].FirstOrDefault(p => p.Name.Equals(name));

                if (memberOrganizationData != null)
                    return memberOrganizationData;
            }

            return null;
        }
        
        public static void RemoveOrganizationMemberData(string name)
        {
            try
            {
                var memberFractionData = GetOrganizationMemberData(name);
                if (memberFractionData != null)
                    AllMembers[memberFractionData.Id].Remove(memberFractionData);
            }
            catch (Exception e)
            {
                Log.Write($"RemoveFromAllMembersOffline Exception: {e.ToString()}");
            }
        }
        
        public static void RemoveOrganizationMemberData(int uuid)
        {
            try
            {
                var memberFractionData = GetOrganizationMemberData(uuid);
                if (memberFractionData != null)
                    AllMembers[memberFractionData.Id].Remove(memberFractionData);
            }
            catch (Exception e)
            {
                Log.Write($"RemoveFromAllMembersOffline Exception: {e.ToString()}");
            }
        }
        
        public static void RemoveOrganizationMemberData(string name, int orgId)
        {
            try
            {
                var memberFractionData = GetOrganizationMemberData(name, orgId);
                if (memberFractionData != null)
                    AllMembers[orgId].Remove(memberFractionData);
            }
            catch (Exception e)
            {
                Log.Write($"RemoveFromAllMembersOffline Exception: {e.ToString()}");
            }
        }
        
        public static void RemoveOrganizationMemberData(int uuid, int orgId)
        {
            try
            {
                var memberFractionData = GetOrganizationMemberData(uuid, orgId);
                if (memberFractionData != null)
                    AllMembers[orgId].Remove(memberFractionData);
            }
            catch (Exception e)
            {
                Log.Write($"RemoveFromAllMembersOffline Exception: {e.ToString()}");
            }
        }
        
        
        public static string GetOrganizationRankName(int orgId, int rank)
        {
            var organizationData = Manager.GetOrganizationData(orgId);
            if (organizationData != null &&
                organizationData.Ranks.ContainsKey(rank))
                return organizationData.Ranks[rank].Name;

            return "";
        }
        private static string GetDepartmentTag(int orgId, int departmentId)
        {
            var organizationData = Manager.GetOrganizationData(orgId);
            if (organizationData != null && departmentId > 0 && organizationData.Departments.ContainsKey(departmentId)) 
                return $"[{organizationData.Departments[departmentId].Tag}]";
            
            return "";
        }
        
        public static void organizationChat(ExtPlayer player, string message)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
                
                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberOrganizationData == null) 
                    return;

                var organizationData = GetOrganizationData(memberOrganizationData.Id);
                if (organizationData == null) 
                    return;

                if (characterData.Unmute > 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMutedMins, characterData.Unmute / 60), 3000);
                    return;
                }
                if (characterData.DemorganTime >= 1) return;
                string status = organizationData.Ranks.ContainsKey(memberOrganizationData.Rank) ? $"{organizationData.Ranks[memberOrganizationData.Rank].Name} " : "Нет ";
                string msgSender = "!{#50C878}" + $"[FC]{GetDepartmentTag(organizationData.Id, memberOrganizationData.DepartmentId)} " + status + player.Name.Replace('_', ' ') + " (" + player.Value + "): " + Commands.RainbowExploit(message);
                foreach (var foreachPlayer in Character.Repository.GetPlayers())
                {
                    var foreachMemberOrganizationData = foreachPlayer.GetOrganizationMemberData();
                    if (foreachMemberOrganizationData == null) 
                        continue;
                    
                    if (foreachMemberOrganizationData.Id != organizationData.Id) 
                        continue;
                    
                    NAPI.Chat.SendChatMessageToPlayer(foreachPlayer, msgSender);
                }
                GameLog.AddInfo($"(FCChat({organizationData.Id})) player({characterData.UUID}) {message}");
            }
            catch (Exception e)
            {
                Log.Write($"organizationChat Exception: {e.ToString()}");
            }
        }

        public static void organizationChatOOC(ExtPlayer player, string message)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberOrganizationData == null) 
                    return;

                var organizationData = GetOrganizationData(memberOrganizationData.Id);
                if (organizationData == null) 
                    return;
                
                if (characterData.Unmute > 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMutedMins, characterData.Unmute / 60), 3000);
                    return;
                }
                if (characterData.DemorganTime >= 1) return;
                string status = organizationData.Ranks.ContainsKey(memberOrganizationData.Rank) ? $"{organizationData.Ranks[memberOrganizationData.Rank].Name} " : "Нет ";
                string msgSender = "!{#50C878}" + $"(( [FC]{GetDepartmentTag(organizationData.Id, memberOrganizationData.DepartmentId)} " + status + player.Name.Replace('_', ' ') + " (" + player.Value + "): " + Commands.RainbowExploit(message) + " ))";
                foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                {
                    var foreachMemberOrganizationData = foreachPlayer.GetOrganizationMemberData();
                    if (foreachMemberOrganizationData == null) 
                        continue;
                    
                    if (foreachMemberOrganizationData.Id != organizationData.Id) 
                        continue;
                    
                    NAPI.Chat.SendChatMessageToPlayer(foreachPlayer, msgSender);
                }
                GameLog.AddInfo($"(FCBChat({organizationData.Id})) player({characterData.UUID}) {message}");
            }
            catch (Exception e)
            {
                Log.Write($"organizationChatOOC Exception: {e.ToString()}");
            }
        }

        // Въезд в организацию
        public static void SetVehicleIntoGarage(ExtPlayer player)
        {
            try
            {
                
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (!player.IsInVehicle)
                {
                    SendPlayerToGarage(player);
                    return;
                }
                
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
                
                var vehicle = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData == null || vehicleLocalData.Access != VehicleAccess.Organization)
                    return;
                
                Trigger.ClientEvent(player, "garageload");
                player.Position = new Vector3(-1396.274, -480.7186, 57.100);
                Trigger.Dimension(player, organizationData.GetDimension());
                characterData.InsideOrganizationID = organizationData.Id;
                
                var petrol = vehicleLocalData.Petrol;
                var dirt = 0f;
                
                var vehicleStateData = vehicle.GetVehicleLocalStateData();
                if (vehicleStateData != null)
                    dirt = vehicleStateData.Dirt;

                VehicleStreaming.DeleteVehicle(vehicle);
                
                var organizationVehicles = organizationData.Vehicles[vehicleLocalData.NumberPlate];
                var veh = VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey(organizationVehicles.model), GaragePositions[organizationVehicles.garageId], GarageRotations[organizationVehicles.garageId], 0, 0, vehicleLocalData.NumberPlate, dimension: organizationData.GetDimension(), locked: true, acc: VehicleAccess.OrganizationGarage, fr: organizationData.Id, minrank: organizationVehicles.rank, petrol: petrol, dirt: dirt);
                VehicleManager.OrgApplyCustomization(veh, organizationVehicles.customization);
            }
            catch (Exception e)
            {
                Log.Write($"SetVehicleIntoGarage Exception: {e.ToString()}");
            }
        }
        public static void GetVehicleFromGarage(ExtPlayer player, ExtVehicle vehicle, int orgId)
        {
            try
            {
                var organizationData = GetOrganizationData(orgId);
                if (organizationData == null) 
                    return;

                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData == null)
                    return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                characterData.InsideOrganizationID = -1;
                
                if (Step++ >= 16) 
                    Step = 0;
                
                vehicle.Dimension = 0;
                vehicle.Position = OutsideGarage[Step] + new Vector3(0, 0, 0.3);
                vehicle.Rotation = new Vector3(0, 0, 180f);
                var organizationVehicle = organizationData.Vehicles[vehicle.NumberPlate];
                vehicleLocalData.Access = VehicleAccess.Organization;

                VehicleManager.OrgApplyCustomization(vehicle, organizationVehicle.customization);

                if (player != null) Trigger.Dimension(player);
            }
            catch (Exception e)
            {
                Log.Write($"GetVehicleFromGarage Exception: {e.ToString()}");
            }
        }
        public static void SpawnOrganizationCar(int orgId, string number, OrganizationVehicleData vehicleOrgData, Vector3 pos = null, float rot = 0f)
        {
            try
            {
                var organizationData = GetOrganizationData(orgId);
                if (organizationData == null) 
                    return;
                
                if (!organizationData.Vehicles.ContainsKey(number)) return;
                if (vehicleOrgData.garageId >= 15) return;
                
                if (Ticket.IsVehicleTickets(number, VehicleTicketType.Organization))
                    return;
                
                var organizationVehicle = organizationData.Vehicles[number];

                organizationData.Used[vehicleOrgData.garageId] = true;
                        
                var veh = VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey(vehicleOrgData.model), pos != null ? pos : GaragePositions[vehicleOrgData.garageId], pos != null ? rot : GarageRotations[vehicleOrgData.garageId], 0, 0, number, dimension: pos != null ? 0 : organizationData.GetDimension(), locked: true, acc: VehicleAccess.OrganizationGarage, fr: orgId, minrank: vehicleOrgData.rank, petrol: vehicleOrgData.petrol, dirt: vehicleOrgData.dirt);
                VehicleManager.OrgApplyCustomization(veh, organizationVehicle.customization);
            }
            catch (Exception e)
            {
                Log.Write($"SpawnOrganizationCars Foreach Exception: {e.ToString()}");
            }
        }
    }
}
