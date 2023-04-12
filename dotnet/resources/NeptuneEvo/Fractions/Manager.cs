using GTANetworkAPI;
using NeptuneEvo.Handles;
using MySqlConnector;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions.LSNews;
using NeptuneEvo.Functions;
using NeptuneEvo.GUI;
using NeptuneEvo.VehicleModel;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Database;
using LinqToDB;
using Localization;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.Players.Phone.Messages.Models;
using NeptuneEvo.Players.Popup.List.Models;
using NeptuneEvo.Table.Models;
using NeptuneEvo.Table.Tasks.Models;
using NeptuneEvo.Table.Tasks.Player;
using NeptuneEvo.VehicleData.LocalData;

namespace NeptuneEvo.Fractions
{
    class Manager : Script
    {
        public static readonly nLog Log = new nLog("Fractions.FractionManager");
        public static void onResourceStart()
        {
            try
            {
                //Main.CreateBlip(new Main.BlisessionData(487, "Merryweather Security", new Vector3(-144.3048, -593.54, 211.76), 4, true, 1.5f));
                //Main.CreateBlip(new Main.BlisessionData(661, "The Lost", FractionSpawns[16], 4, true, 1.3f));
               
                Main.CreateBlip(new Main.BlipData(459, "News", LsNewsSystem.LSNewsCoords[0], 4, true, 1.3f));

                Main.CreateBlip(new Main.BlipData(621, LangFunc.GetText(LangType.Ru, DataName.Bolnica), Ems.emsCheckpoints[0], 1, true));
                
                Main.CreateBlip(new Main.BlipData(60, "FIB", Fbi.GpsPosition, 4, true));
                
                Main.CreateBlip(new Main.BlipData(478, LangFunc.GetText(LangType.Ru, DataName.WarPort), Army.ArmyCheckpoints[2], 52, true));

                Main.CreateBlip(new Main.BlipData(437, "The Families", FractionSpawns[1], 25, true));
                Main.CreateBlip(new Main.BlipData(437, "The Ballas Gang", FractionSpawns[2], 7, true));
                Main.CreateBlip(new Main.BlipData(437, "Los Santos Vagos", FractionSpawns[3], 5, true));
                Main.CreateBlip(new Main.BlipData(437, "Marabunta Grande", FractionSpawns[4], 3, true));
                Main.CreateBlip(new Main.BlipData(437, "Blood Street", FractionSpawns[5], 1, true));

                Main.CreateBlip(new Main.BlipData(679, "La Cosa Nostra", FractionSpawns[10], 4, true));
                Main.CreateBlip(new Main.BlipData(679, "Russian Mafia", FractionSpawns[11], 4, true));
                Main.CreateBlip(new Main.BlipData(679, "Yakuza", FractionSpawns[12], 4, true));
                Main.CreateBlip(new Main.BlipData(679, "Armenian Mafia", FractionSpawns[13], 4, true));

                CreateGarageData();

                /*
                // NOW LOADING IN MAIN
                using MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "SELECT `uuid`,`firstname`,`lastname`,`fraction`,`fractionlvl` FROM `characters`"
                };

                using DataTable result = MySQL.QueryRead(cmd);
                if (result != null)
                {
                    foreach (DataRow Row in result.Rows)
                    {
                        MemberData memberData = new MemberData();
                        memberData.Name = $"{Convert.ToString(Row["firstname"])}_{Convert.ToString(Row["lastname"])}";
                        memberData.FractionID = Convert.ToInt32(Row["fraction"]);
                        memberData.FractionLVL = Convert.ToInt32(Row["fractionlvl"]);
                        memberData.inFracName = getNickname(memberData.FractionID, memberData.FractionLVL);

                        if (memberData.FractionID != (int) Models.Fractions.None) AllMembers.Add(memberData);
                    }
                }*/
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }

        public static IReadOnlyDictionary< WeaponRepository.Hash, int> matsForGun = new Dictionary< WeaponRepository.Hash, int>()
        {
            { WeaponRepository.Hash.Pistol, 50 },
            { WeaponRepository.Hash.SNSPistol, 40 },
            { WeaponRepository.Hash.CeramicPistol, 60 },
            { WeaponRepository.Hash.DoubleBarrelShotgun, 80 },
            { WeaponRepository.Hash.SawnOffShotgun, 100 },
            { WeaponRepository.Hash.MachinePistol, 110 },
            { WeaponRepository.Hash.MiniSMG, 120 },
            { WeaponRepository.Hash.Bat, 30 },
            { WeaponRepository.Hash.Machete, 30 },
            { WeaponRepository.Hash.Pistol50, 80 },
            { WeaponRepository.Hash.CombatPistol, 80 },
            { WeaponRepository.Hash.VintagePistol, 70 },
            { WeaponRepository.Hash.PumpShotgun, 120 },
            { WeaponRepository.Hash.BullpupShotgun, 200 },
            { WeaponRepository.Hash.AssaultRifle, 200 },
            { WeaponRepository.Hash.CompactRifle, 180 },
            { WeaponRepository.Hash.HeavyPistol, 90 },
            { WeaponRepository.Hash.Hatchet, 50 },
            { WeaponRepository.Hash.GolfClub, 50 },
            { WeaponRepository.Hash.SwitchBlade, 50 },
            { WeaponRepository.Hash.Hammer, 50 },
            { WeaponRepository.Hash.MicroSMG, 150 },
            { WeaponRepository.Hash.SMGMk2, 180 },
            { WeaponRepository.Hash.Nightstick, 30 },
            { WeaponRepository.Hash.SMG, 100 },
            { WeaponRepository.Hash.CombatPDW, 140 },
            { WeaponRepository.Hash.StunGun, 100 },
            { WeaponRepository.Hash.CarbineRifle, 100 },
            { WeaponRepository.Hash.SmokeGrenade, 5 },
            { WeaponRepository.Hash.HeavyShotgun, 1500 },
            { WeaponRepository.Hash.Knife, 20 },
            { WeaponRepository.Hash.SniperRifle, 4000 },
            { WeaponRepository.Hash.HeavySniper, 3000 },
            { WeaponRepository.Hash.AssaultSMG, 180 },
            { WeaponRepository.Hash.AdvancedRifle, 220 },
            { WeaponRepository.Hash.Gusenberg, 500 },
            { WeaponRepository.Hash.CombatMG, 2500 },
            { WeaponRepository.Hash.SweeperShotgun, 120 },
            { WeaponRepository.Hash.PumpShotgunMk2, 140 },
            { WeaponRepository.Hash.SNSPistolMk2, 60 },
            { WeaponRepository.Hash.BullpupRifle, 1000 },
            { WeaponRepository.Hash.PistolMk2, 75 },
            { WeaponRepository.Hash.CarbineRifleMk2, 200 },
            { WeaponRepository.Hash.APPistol, 250 },
            { WeaponRepository.Hash.GrenadeLauncherSmoke, 15000 },
            { WeaponRepository.Hash.AssaultShotgun, 3500 },
            { WeaponRepository.Hash.CombatMGMk2, 3000 },
            { WeaponRepository.Hash.BullpupRifleMk2, 260 },
            { WeaponRepository.Hash.MarksmanRifleMk2, 4500 },
            { WeaponRepository.Hash.MarksmanRifle, 4000 },
            { WeaponRepository.Hash.SpecialCarbineMk2, 380 },
            { WeaponRepository.Hash.SpecialCarbine, 350 },
            { WeaponRepository.Hash.AssaultRifleMk2, 250 },
            { WeaponRepository.Hash.MilitaryRifle, 1000 },
        };

        public static int matsForArmor { get; } = 250;
        private static List<string>[] gangGuns = new List<string>[5]
        {
            new List<string>
            {
                "Pistol",
                "SNSPistol",
                "CeramicPistol",
                "HeavyPistol",
                "VintagePistol",
                "Pistol50",
            },
            new List<string>
            {
                "DoubleBarrelShotgun",
                "SawnOffShotgun",
                "PumpShotgun",
                "AssaultShotgun"
            },
            new List<string>
            {
                "MicroSMG",
                "SMGMk2",
                "MachinePistol",
                "MiniSMG",
                "CombatPDW",
            },
            new List<string>
            {
                "CompactRifle",
                "AssaultRifle",
                "BullpupRifle",
            },
            new List<string>
            {
                "SniperRifle",
            },
        };

        private static List<string>[] nongovGuns = new List<string>[4]
        {
            new List<string>
            {
                "PistolMk2",
                "HeavyPistol",
                "StunGun",
                "CombatPistol",
                "Pistol50",
                "APPistol",
            },
            new List<string>
            {
                "PumpShotgunMk2",
                "BullpupShotgun",
                "SawnOffShotgun",
                "AssaultShotgun",
                "SweeperShotgun",
            },
            new List<string>
            {
                "SMG",
                "CombatPDW",
                "AssaultSMG",
                "SMGMk2",
            },
            new List<string>
            {
                "AdvancedRifle",
                "AssaultRifleMk2",
                "CarbineRifle",
                "CarbineRifleMk2",
                "BullpupRifle",
            },
        };
        private static List<string>[] mafiaGuns = new List<string>[5]
        {
            new List<string>
            {
                "PistolMk2",
                "Pistol50",
                "VintagePistol",
                "CeramicPistol",
                "HeavyPistol",
                "SNSPistolMk2",
            },
            new List<string>
            {
                "PumpShotgun",
                "DoubleBarrelShotgun",
                "SawnOffShotgun",
                "PumpShotgunMk2",
                "SweeperShotgun",
                "AssaultShotgun"
            },
            new List<string>
            {
                "MiniSMG",
                "MicroSMG",
                "SMGMk2",
                "CombatPDW",
                "MachinePistol",
                "Gusenberg",
            },
            new List<string>
            {
                "AssaultRifle",
                "CompactRifle",
                "AssaultRifleMk2",
                "BullpupRifle",
                "MilitaryRifle",
            },
            new List<string>
            {
                "MarksmanRifle",
            },
        };

        private static List<string>[] organizationsGuns = new List<string>[4]
        {
            new List<string>
            {
                "Pistol",
                "PistolMk2",
                "Pistol50",
                "HeavyPistol",
            },
            new List<string>
            {
                "PumpShotgun",
                "DoubleBarrelShotgun",
                "SawnOffShotgun",
            },
            new List<string>
            {
                "MiniSMG",
                "SMGMk2",
                "MachinePistol",
                "MicroSMG",
                "CombatPDW",
            },
            new List<string>
            {
                "CompactRifle",
                "AssaultRifle",
            },
        };
        private static List<string>[] bikersGuns = new List<string>[4]
        {
            new List<string>
            {
                "PistolMk2",
                "CombatPistol",
                "CeramicPistol",
                "HeavyPistol",
                "VintagePistol",
                "Pistol50",
            },
            new List<string>
            {
                "PumpShotgun",
                "SawnOffShotgun",
                "PumpShotgunMk2",
                "DoubleBarrelShotgun",
                "SweeperShotgun",
            },
            new List<string>
            {
                "MachinePistol",
                "MicroSMG",
                "SMGMk2",
                "CombatPDW",
                "MiniSMG",
            },
            new List<string>
            {
                "CompactRifle",
                "AssaultRifle",
                "CarbineRifle",
            },
        };
        
        public static Dictionary<int, FractionData> Fractions = new Dictionary<int, FractionData>();
        
        public static FractionData GetFractionData(int fractionId)
        {
            if (Fractions.ContainsKey(fractionId))
                return Fractions[fractionId];

            return null;
        }

        public static ConcurrentDictionary<int, List<FractionMemberData>> AllMembers = new ConcurrentDictionary<int, List<FractionMemberData>>();


        public static FractionMemberData GetFractionMemberData(int uuid)
        {
            foreach (var members in AllMembers.Values.ToList())
            {
                var memberFractionData = members.FirstOrDefault(p => p.UUID == uuid);

                if (memberFractionData != null)
                    return memberFractionData;
            }

            return null;
        }
        
        public static FractionMemberData GetFractionMemberData(string name)
        {
            foreach (var members in AllMembers.Values.ToList())
            {
                var memberFractionData = members.FirstOrDefault(p => p.Name.Equals(name));

                if (memberFractionData != null)
                    return memberFractionData;
            }

            return null;
        }

        public static FractionMemberData GetFractionMemberData(int uuid, int fracId)
        {
            if (AllMembers.ContainsKey(fracId))
            {
                var memberFractionData = AllMembers[fracId].FirstOrDefault(p => p.UUID == uuid);

                if (memberFractionData != null)
                    return memberFractionData;
            }

            return null;
        }
        
        public static FractionMemberData GetFractionMemberData(string name, int fracId)
        {
            if (AllMembers.ContainsKey(fracId))
            {
                var memberFractionData = AllMembers[fracId].FirstOrDefault(p => p.Name.Equals(name));

                if (memberFractionData != null)
                    return memberFractionData;
            }

            return null;
        }
        
        public static void RemoveFractionMemberData(string name)
        {
            try
            {
                var memberFractionData = GetFractionMemberData(name);
                if (memberFractionData != null)
                    AllMembers[memberFractionData.Id].Remove(memberFractionData);
            }
            catch (Exception e)
            {
                Log.Write($"RemoveFromAllMembersOffline Exception: {e.ToString()}");
            }
        }
        
        public static void RemoveFractionMemberData(int uuid)
        {
            try
            {
                var memberFractionData = GetFractionMemberData(uuid);
                if (memberFractionData != null)
                    AllMembers[memberFractionData.Id].Remove(memberFractionData);
            }
            catch (Exception e)
            {
                Log.Write($"RemoveFromAllMembersOffline Exception: {e.ToString()}");
            }
        }
        
        public static void RemoveFractionMemberData(string name, int fracId)
        {
            try
            {
                var memberFractionData = GetFractionMemberData(name, fracId);
                if (memberFractionData != null)
                    AllMembers[fracId].Remove(memberFractionData);
            }
            catch (Exception e)
            {
                Log.Write($"RemoveFromAllMembersOffline Exception: {e.ToString()}");
            }
        }
        
        public static void RemoveFractionMemberData(int uuid, int fracId)
        {
            try
            {
                var memberFractionData = GetFractionMemberData(uuid, fracId);
                if (memberFractionData != null)
                    AllMembers[fracId].Remove(memberFractionData);
            }
            catch (Exception e)
            {
                Log.Write($"RemoveFromAllMembersOffline Exception: {e.ToString()}");
            }
        }
        
        public static IReadOnlyDictionary<int, Vector3> FractionSpawns = new Dictionary<int, Vector3>()
        {
            {(int) Models.Fractions.FAMILY, new Vector3(-199.28838, -1584.5262, 35)},    // The Families
            {(int) Models.Fractions.BALLAS, new Vector3(119.04292, -1966.3086, 21.31)},     // The Ballas Gang
            {(int) Models.Fractions.VAGOS, new Vector3(1403.206, -1483.664, 60.63504)},     // Los Santos Vagos
            {(int) Models.Fractions.MARABUNTA, new Vector3(949.50433, -2151.384, 31.269314)},     // Marabunta Grande
            {(int) Models.Fractions.BLOOD, new Vector3(489.85736, -1528.8881, 23.271452)},     // Blood Street
            {(int) Models.Fractions.CITY, new Vector3(-1300.3295, -559.02313, 30.56686)},      // Cityhall
            {(int) Models.Fractions.POLICE, new Vector3(458.25644, -999.1341, 30.689493)},      // LSPD police
            {(int) Models.Fractions.EMS, new Vector3(322.68, -592.2, 43.2)},      // Emergency care
            {(int) Models.Fractions.FIB, new Vector3(152.11882, -751.9469, 242.15207)},  // FBI 
            {(int) Models.Fractions.LCN, new Vector3(1387.338, 1155.952, 115.2144)},     // La Cosa Nostra 
            {(int) Models.Fractions.RUSSIAN, new Vector3(-115.1648, 983.5231, 236.6358)},    // Russian Mafia
            {(int) Models.Fractions.YAKUZA, new Vector3( -1464.758, -34.101257, 55.20967)},    // Yakuza 
            {(int) Models.Fractions.ARMENIAN, new Vector3(-1809.738, 444.3138, 129.3889)},    // Armenian Mafia 
            {(int) Models.Fractions.ARMY, new Vector3(-2361.488, 3208.3765, 29.224823)},    // Army
            {(int) Models.Fractions.LSNEWS, new Vector3(-563.7617, -934.2492, 23.866652)},    // LSNews -1063.046, -249.463, 44.0211
            {(int) Models.Fractions.THELOST, new Vector3(982.2743, -104.14917, 73.72877)},    // The Lost
            {(int) Models.Fractions.MERRYWEATHER, new Vector3(2154.641, 2921.034, -63.02243)},    // Merryweather
            //{(int) Models.Fractions.SHERIFF, new Vector3(1849.4763, 3695.783, 34.266933)},    // SHERIFF
            {(int) Models.Fractions.SHERIFF, new Vector3(-433.1862, 6003.6655, 31.7)},    // SHERIFF
        };
        public static IReadOnlyDictionary<int, FractionsType> FractionTypes = new Dictionary<int, FractionsType>() // 0 - mafia, 1 gangs, 2 - gov, 3 -  nongov, 4 - bikers
        {
            {(int) Models.Fractions.None, FractionsType.None},
            {(int) Models.Fractions.FAMILY, FractionsType.Gangs}, // The Families
            {(int) Models.Fractions.BALLAS, FractionsType.Gangs}, // The Ballas Gang
            {(int) Models.Fractions.VAGOS, FractionsType.Gangs},  // Los Santos Vagos
            {(int) Models.Fractions.MARABUNTA, FractionsType.Gangs }, // Marabunta Grande
            {(int) Models.Fractions.BLOOD, FractionsType.Gangs}, // Blood Street
            {(int) Models.Fractions.CITY, FractionsType.Gov}, // Cityhall
            {(int) Models.Fractions.POLICE, FractionsType.Gov}, // LSPD police
            {(int) Models.Fractions.EMS, FractionsType.Gov}, // Emergency care
            {(int) Models.Fractions.FIB, FractionsType.Gov}, // FBI 
            {(int) Models.Fractions.LCN, FractionsType.Mafia}, // La Cosa Nostra 
            {(int) Models.Fractions.RUSSIAN, FractionsType.Mafia}, // Russian Mafia
            {(int) Models.Fractions.YAKUZA, FractionsType.Mafia}, // Yakuza 
            {(int) Models.Fractions.ARMENIAN, FractionsType.Mafia}, // Armenian Mafia 
            {(int) Models.Fractions.ARMY, FractionsType.Gov}, // Army
            {(int) Models.Fractions.LSNEWS, FractionsType.Gov}, // News
            {(int) Models.Fractions.THELOST, FractionsType.Bikers}, // The Lost
            {(int) Models.Fractions.MERRYWEATHER, FractionsType.Nongov}, // Merryweather
            {(int) Models.Fractions.SHERIFF, FractionsType.Gov}, // LSPD police
        };
        public static IReadOnlyDictionary<int, string> FractionNames = new Dictionary<int, string>()
        {
            {(int) Models.Fractions.None, String.Empty },
            {(int) Models.Fractions.FAMILY, "The Families" },
            {(int) Models.Fractions.BALLAS, "The Ballas Gang" },
            {(int) Models.Fractions.VAGOS, "Los Santos Vagos" },
            {(int) Models.Fractions.MARABUNTA, "Marabunta Grande" },
            {(int) Models.Fractions.BLOOD, "Blood Street" },
            {(int) Models.Fractions.CITY, "Cityhall" },
            {(int) Models.Fractions.POLICE, "Police" },
            {(int) Models.Fractions.EMS, "Hospital" },
            {(int) Models.Fractions.FIB, "FIB" },
            {(int) Models.Fractions.LCN, "La Cosa Nostra" },
            {(int) Models.Fractions.RUSSIAN, "Russian Mafia" },
            {(int) Models.Fractions.YAKUZA, "Yakuza" },
            {(int) Models.Fractions.ARMENIAN, "Armenian Mafia" },
            {(int) Models.Fractions.ARMY, "Army" },
            {(int) Models.Fractions.LSNEWS, "News" },
            {(int) Models.Fractions.THELOST, "The Lost" },
            {(int) Models.Fractions.MERRYWEATHER, "Merryweather Security" },
            {(int) Models.Fractions.SHERIFF, "Sheriff" },
        };
        //
        
        public static IReadOnlyList<List<List<Vector3>>> StaticExitGaragesData = new List<List<List<Vector3>>>
        {
            //23 Гаражных места
            new List<List<Vector3>>()
            {
                //Позиция
                new List<Vector3>()
                {
                    new Vector3(1257.2184, 222.914, -48.500614),//Спавн при въезде в гараж
                    new Vector3(1253.628, 230.11203, -48.365498),//Выезд из гаража
                    new Vector3(1295.3708, 217.50514, -49.05509)
                },
                //Поворот
                new List<Vector3>()
                {
                    new Vector3( -3.2241206, -0.03539895,-90.975426),//Спавн при въезде в гараж
                    new Vector3( ),//Выезд из гаража
                    new Vector3(0, 0, -3.9644465)
                }
            },
            //32 Гаражных места
            new List<List<Vector3>>()
            {
                //Позиция
                new List<Vector3>()
                {
                    new Vector3(1342.1973, 183.5336, -48.45059),//Спавн при въезде в гараж
                    new Vector3(1338.28, 190.72278, -48.29839),//Выезд из гаража
                    new Vector3(1380.2288, 178.03362, -48.99316)
                },
                //Поворот
                new List<Vector3>()
                {
                    new Vector3( -3.2827938, 0.05066691, -89.32653),//Спавн при въезде в гараж
                    new Vector3( ),//Выезд из гаража
                    new Vector3(0, 0, -3.8956938)
                }
            },
        };
        
        private static IReadOnlyList<GarageData> GaragesData = new List<GarageData>()
        {
            new GarageData((int) Models.Fractions.CITY,
                new Vector3(-1309.2749, -559.4525, 20.80f), new Vector3(), 
                StaticExitGaragesData[1][0][2], StaticExitGaragesData[1][1][2], 
                
                new Vector3(-1315.6322, -560.3084, 20f), 
                new Vector3(-1315.6322, -560.3084, 20f), new Vector3(0f, 0f, 128.5f), 
                StaticExitGaragesData[1][0][1], 
                StaticExitGaragesData[1][0][0], StaticExitGaragesData[1][1][0]),
            new GarageData((int) Models.Fractions.POLICE,
                new Vector3(461.55856, -1019.4882, 27.7897), new Vector3(), 
                StaticExitGaragesData[1][0][2], StaticExitGaragesData[1][1][2], 
                
                new Vector3(461.55856, -1019.4882, 27.7897), 
                new Vector3(463.19778, -1014.53796, 27.772268), new Vector3(0.067829154, 0.3221672, 89.33315), 
                StaticExitGaragesData[1][0][1], 
                StaticExitGaragesData[1][0][0], StaticExitGaragesData[1][1][0]),
            new GarageData((int) Models.Fractions.EMS,
                new Vector3(338.57315, -561.15015, 28.44394), new Vector3(), 
                StaticExitGaragesData[0][0][2], StaticExitGaragesData[0][1][2], 
                
                new Vector3(338.57315, -561.15015, 28.44394), 
                new Vector3(325.89642, -557.0457, 28.442497), new Vector3(-0.01822554, 0.059563655, -19.873772), 
                StaticExitGaragesData[0][0][1], 
                StaticExitGaragesData[0][0][0], StaticExitGaragesData[0][1][0]),
            new GarageData((int) Models.Fractions.LSNEWS,
                new Vector3(-543.7394, -889.4364, 24.71014), new Vector3(), 
                StaticExitGaragesData[0][0][2], StaticExitGaragesData[0][1][2], 
                
                new Vector3(-543.7394, -889.4364, 24.71014), 
                new Vector3(-532.5438, -889.4513, 24.555765), new Vector3(-6.857542, -0.17768654, 178.8801), 
                StaticExitGaragesData[0][0][1], 
                StaticExitGaragesData[0][0][0], StaticExitGaragesData[0][1][0]),
            new GarageData((int) Models.Fractions.SHERIFF,
                new Vector3(-455.1163, 6001.7246, 30.91623), new Vector3(), 
                StaticExitGaragesData[0][0][2], StaticExitGaragesData[0][1][2], 
                
                new Vector3(-455.1163, 6001.7246, 30.91623), 
                new Vector3(-463.02225, 6019.244, 30.915943), new Vector3(-0.24266532, 0.3693462, -42.14722), 
                StaticExitGaragesData[0][0][1], 
                StaticExitGaragesData[0][0][0], StaticExitGaragesData[0][1][0]),
            new GarageData((int) Models.Fractions.SHERIFF,
                new Vector3(1837.8867, 3699.5408, 33.693085), new Vector3(), 
                StaticExitGaragesData[0][0][2], StaticExitGaragesData[0][1][2], 
                
                new Vector3(1837.8867, 3699.5408, 33.693085), 
                new Vector3(1860.5508, 3707.3257, 32.929916), new Vector3(1.2823077, -1.3675255, -148.68239), 
                StaticExitGaragesData[0][0][1], 
                StaticExitGaragesData[0][0][0], StaticExitGaragesData[0][1][0],
                DefaultDimension: 1850),
        };

        private static void CreateGarageData()
        {
            int index = 0;
            foreach (var garageData in GaragesData)
            {
                CustomColShape.CreateCylinderColShape(garageData.VehEnterPosPoint, 3f, 2, 0, ColShapeEnums.FractionGarageEnter, index); // Телепорт из улицы в гараж
                //(ExtTextLabel) NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Гараж News"), new Vector3(LSNewsCoords[4].X, LSNewsCoords[4].Y, LSNewsCoords[4].Z + 0.7), 5F, 0.4F, 0, new Color(255, 255, 255));
                NAPI.Marker.CreateMarker(1, garageData.VehEnterPosPoint - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 3, new Color(255, 255, 255, 220), dimension: 0);

                CustomColShape.CreateCylinderColShape(garageData.VehExitPosPoint, 3f, 2, (uint)(garageData.DefaultDimension + garageData.FractionId), ColShapeEnums.FractionGarageExit, index); // Телепорт с гаража на улицу
                NAPI.Marker.CreateMarker(1, garageData.VehExitPosPoint - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 3, new Color(255, 255, 255, 220), dimension: (uint)(garageData.DefaultDimension + garageData.FractionId));
                
                CustomColShape.CreateCylinderColShape(garageData.PlayerExitPos, 1.5f, 2, (uint)(garageData.DefaultDimension + garageData.FractionId), ColShapeEnums.FractionGarageExit, index); // Телепорт с гаража на улицу
                NAPI.Marker.CreateMarker(1, garageData.PlayerExitPos - new Vector3(0, 0, 1.2), new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220), dimension: (uint)(garageData.DefaultDimension + garageData.FractionId));

                index++;
            }
        }

        [Interaction(ColShapeEnums.FractionGarageEnter)]
        public static void FractionGarageEnter(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                var garageData = GaragesData[index];
                var fracId = player.GetFractionId();
                
                if (fracId != garageData.FractionId && characterData.AdminLVL == 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouNotInFraction), 3000);
                    return;
                }
                if (sessionData.Following != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsFollowing), 3000);
                    return;
                }
                if (!player.IsInVehicle)
                {
                    player.Position = garageData.PlayerExitPos;
                    player.Rotation = garageData.PlayerExitRot;
                    Trigger.Dimension(player, (uint)(garageData.DefaultDimension + garageData.FractionId));
                }
                else
                {
                    if (player.Vehicle == null) return;
                    if (player.VehicleSeat != (int)VehicleSeat.Driver)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustBeDriver), 3000);
                        return;
                    }
                    var veh = player.Vehicle;
                    veh.Position = garageData.VehExitPos;
                    veh.Rotation = garageData.VehExitRot;
                    Trigger.Dimension(veh, (uint) (garageData.DefaultDimension + garageData.FractionId));
                    Trigger.Dimension(player, (uint)(garageData.DefaultDimension + garageData.FractionId));
                }
                characterData.ExteriorPos = garageData.PlayerEnterPos;
            }
            catch (Exception e)
            {
                Log.Write($"FractionGarageEnter Exception: {e.ToString()}");
            }
        }
        [Interaction(ColShapeEnums.FractionGarageExit)]
        public static void FractionGarageExit(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                var garageData = GaragesData[index];
                var fracId = player.GetFractionId();
                
                if (fracId != garageData.FractionId && characterData.AdminLVL == 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouNotInFraction), 3000);
                    return;
                }
                if (sessionData.Following != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsFollowing), 3000);
                    return;
                }
                if (!player.IsInVehicle)
                {
                    player.Position = garageData.PlayerEnterPos;
                    player.Rotation = garageData.PlayerEnterRot;
                    Trigger.Dimension(player);
                }
                else
                {
                    if (player.Vehicle == null) return;
                    if (player.VehicleSeat != (int)VehicleSeat.Driver)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustBeDriver), 3000);
                        return;
                    }
                    var veh = player.Vehicle;
                    veh.Position = garageData.VehEnterPos;
                    veh.Rotation = garageData.VehEnterRot;
                    Trigger.Dimension(veh);
                    Trigger.Dimension(player);
                }
                characterData.ExteriorPos = new Vector3();
            }
            catch (Exception e)
            {
                Log.Write($"FractionGarageExit Exception: {e.ToString()}");
            }
        }

        public static void fractionChat(ExtPlayer player, string message)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;	
                
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                var fracId = memberFractionData.Id;
                if (characterData.Unmute > 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMutedMins, characterData.Unmute / 60), 3000);
                    return;
                }
                if (characterData.DemorganTime >= 1) return;
                string msgSender = $"~b~[F] {GetDepartmentTag(fracId, memberFractionData.DepartmentId)} {GetFractionRankName(fracId, memberFractionData.Rank)} " + player.Name.ToString().Replace('_', ' ') + " (" + player.Value + "): " + Commands.RainbowExploit(message);
                foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                {
                    var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                    if (foreachMemberFractionData == null) 
                        continue;
                    
                    if (foreachMemberFractionData.Id == fracId)
                    {
                        NAPI.Chat.SendChatMessageToPlayer(foreachPlayer, msgSender);
                        if (Configs.IsFractionPolic(fracId) || fracId == (int) Models.Fractions.ARMY) foreachPlayer.Eval("mp.game.audio.playPoliceReport(\"LAMAR_1_POLICE_LOST\", 0.0);"); // Police Report Sound for LSPD & FBI
                    }
                }
                GameLog.AddInfo($"(FChat({fracId})) player({characterData.UUID}) {message}");
            }
            catch (Exception e)
            {
                Log.Write($"fractionChat Exception: {e.ToString()}");
            }
        }

        public static void fractionChatOOC(ExtPlayer player, string message)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                var fracId = memberFractionData.Id;
                if (fracId == (int) Models.Fractions.None) return;
                if (characterData.Unmute > 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMutedMins, characterData.Unmute / 60), 3000);
                    return;
                }
                if (characterData.DemorganTime >= 1) return;
                string msgSender = $"~b~ (( [F] " + player.Name.ToString().Replace('_', ' ') + " (" + player.Value + "): " + Commands.RainbowExploit(message) + " ))";
                foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                {
                    var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                    if (foreachMemberFractionData == null) 
                        continue;
                    
                    if (foreachMemberFractionData.Id == fracId) 
                        NAPI.Chat.SendChatMessageToPlayer(foreachPlayer, msgSender);
                }
                GameLog.AddInfo($"(FBChat({fracId})) player({characterData.UUID}) {message}");
            }
            catch (Exception e)
            {
                Log.Write($"fractionChatOOC Exception: {e.ToString()}");
            }
        }

        public static List<int> GovIds = new List<int> { 6, 7, 8, 9, 14, 15, 17, 18 };
        public static Dictionary<int, string> GovTags = new Dictionary<int, string>
        {
            { 7, "LSPD" },
            { 6, "GOV" },
            { 8, "EMS" },
            { 9, "FIB" },
            { 14, "ARMY" },
            { 15, "NEWS" },
            { 17, "MWS" },
            { 18, "SHERIFF" }
        };
        public static void govFractionChat(ExtPlayer player, string message)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.Dep)) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
                
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                var fracId = memberFractionData.Id;
                
                if (characterData.Unmute > 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMutedMins, characterData.Unmute / 60), 3000);
                    return;
                }
                if (characterData.DemorganTime >= 1) return;
                if (!GovIds.Contains(fracId)) return;
                string msgSender = "!{#B8962E}" + $"[{GovTags[fracId]}] {GetFractionRankName(fracId, memberFractionData.Rank)} " + player.Name.ToString().Replace('_', ' ') + " (" + player.Value + "): " + Commands.RainbowExploit(message);
                foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                {
                    var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                    if (foreachMemberFractionData == null) 
                        continue;
                    
                    if (GovIds.Contains(foreachMemberFractionData.Id)) 
                        NAPI.Chat.SendChatMessageToPlayer(foreachPlayer, msgSender);
                }
                GameLog.FracLog(fracId, characterData.UUID, -1, player.Name, "-1", $"dep({message})");
                GameLog.AddInfo($"(DChat) player({characterData.UUID}) {message}");
            }
            catch (Exception e)
            {
                Log.Write($"govFractionChat Exception: {e.ToString()}");
            }
        }
        public static int FractionMembersCount(int fracId)
        {
            try
            {
                return Character.Repository.GetPlayers()
                    .Count(v => v.GetFractionId() == fracId);
            }
            catch (Exception e)
            {
                Log.Write($"countOfFractionMembers Exception: {e.ToString()}");
                return 0;
            }
        }
        public static int FractionMembersCount(int [] fracsId)
        {
            try
            {
                return Character.Repository.GetPlayers()
                    .Count(v => fracsId.Contains(v.GetFractionId()));
            }
            catch (Exception e)
            {
                Log.Write($"countOfFractionMembers Exception: {e.ToString()}");
                return 0;
            }
        }
        public static string GetName(int fracId)
        {
            try
            {
                if (!FractionNames.ContainsKey(fracId)) return null;
                return FractionNames[fracId];
            }
            catch (Exception e)
            {
                Log.Write($"getName Exception: {e.ToString()}");
                return null;
            }
        }
        
        public static void AcceptCallFractionMemberDialog(ExtPlayer player, int fracId) 
        { 
            try 
            { 
                var sessionData = player.GetSessionData(); 
                if (sessionData == null) return; 
 
                var characterData = player.GetCharacterData(); 
                if (characterData == null) return; 
 
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
                else if (sessionData.IsCalledGovMember == 2) 
                { 
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CallGov15sec), 3000); 
                    return; 
                } 
                else if (Main.IHaveDemorgan(player, true)) return; 
 
                string numberInfo = characterData.Sim == -1 ? "" : characterData.Sim.ToString();

                Manager.sendFractionMessage(fracId, "!{#F08080}[F] " + LangFunc.GetText(LangType.Ru, DataName.PlayerCallGov, player.Name, player.Value) + " " + numberInfo, true); 
                EventSys.SendCoolMsg(player, "Успешно", "Вызов сотрудника", $"{LangFunc.GetText(LangType.Ru, DataName.PlayerSucCallGov)}",  "", 10000);
                //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerSucCallGov), 3000); 
                //Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Ems, LangFunc.GetText(LangType.Ru, DataName.PlayerSucCallGov), DateTime.Now); 
                BattlePass.Repository.UpdateReward(player, 51);
                sessionData.IsCalledGovMember = 2; 
 
                NAPI.Task.Run(() => 
                { 
                    try 
                    { 
                        if (player.IsCharacterData()) 
                        { 
                            if (sessionData.IsCalledGovMember == 2) 
                                sessionData.IsCalledGovMember = 1; 
                        } 
                    } 
                    catch (Exception e) 
                    { 
                        Log.Write($"AcceptCallFractionMemberDialog Task Exception: {e.ToString()}"); 
                    } 
                }, 60000); 
            } 
            catch (Exception e) 
            { 
                Log.Write($"AcceptCallFractionMemberDialog Exception: {e.ToString()}"); 
            } 
        }

        public static void sendFractionMessage(int fracid, string message, bool inChat = false)
        { 
            try
            {
                if (inChat)
                {
                    foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                    {
                        try
                        {
                            var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                            if (foreachMemberFractionData == null) 
                                continue;
                            
                            if (foreachMemberFractionData.Id == fracid) 
                                Trigger.SendChatMessage(foreachPlayer, message);
                        }
                        catch (Exception e)
                        {
                            Log.Write($"sendFractionMessage Foreach #1 Exception: {e.ToString()}");
                        }
                    }
                }
                else
                {
                    foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                    {
                        try
                        {
                            var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                            if (foreachMemberFractionData == null) 
                                continue;
                            
                            if (foreachMemberFractionData.Id == fracid) 
                                Notify.Send(foreachPlayer, NotifyType.Warning, NotifyPosition.BottomCenter, message, 3000);
                        }
                        catch (Exception e)
                        {
                            Log.Write($"sendFractionMessage Foreach #2 Exception: {e.ToString()}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"sendFractionMessage Exception: {e.ToString()}");
            }
        }
        
          
        public static void SendCoolFractionMsg(int fracid, string subTitle, string title, string desc, string image, ushort timeWait)
        { 
            try
            {
                {
                    foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                    {
                        try
                        {
                            var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                            if (foreachMemberFractionData == null) 
                                continue;
                            
                            if (foreachMemberFractionData.Id == fracid) 
                                Trigger.ClientEvent(foreachPlayer, "hud.event.cool", subTitle, title, desc, image, timeWait);
                        }
                        catch (Exception e)
                        {
                            Log.Write($"sendFractionMessage Foreach #2 Exception: {e.ToString()}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"sendFractionCoolMessage Exception: {e.ToString()}");
            }
        }
        
        public static void SendCoolOrganizationMsg(int orgId, string subTitle, string title, string desc, string image, ushort timeWait)
        { 
            try
            {
                {
                    foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                    {
                        try
                        {
                            var foreachMemberOrganizationData = foreachPlayer.GetOrganizationMemberData();
                            if (foreachMemberOrganizationData == null) 
                                continue;
                            if (foreachMemberOrganizationData.Id != orgId) 
                                continue;
                            
                            Trigger.ClientEvent(foreachPlayer, "hud.event.cool", subTitle, title, desc, image, timeWait);
                        }
                        catch (Exception e)
                        {
                            Log.Write($"sendFractionMessage Foreach #2 Exception: {e.ToString()}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"sendFractionCoolMessage Exception: {e.ToString()}");
            }
        }

        public static void sendOrganizationMessage(int orgId, string message, bool inChat = false)
        {
            try
            {
                if (inChat)
                {
                    foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                    {
                        try
                        {
                            var foreachMemberOrganizationData = foreachPlayer.GetOrganizationMemberData();
                            if (foreachMemberOrganizationData == null) 
                                continue;
                            if (foreachMemberOrganizationData.Id != orgId) 
                                continue;
                            
                            Trigger.SendChatMessage(foreachPlayer, message);
                        }
                        catch (Exception e)
                        {
                            Log.Write($"sendOrganizationMessage Foreach #1 Exception: {e.ToString()}");
                        }
                    }
                }
                else
                {
                    foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                    {
                        try
                        {
                            var foreachMemberOrganizationData = foreachPlayer.GetOrganizationMemberData();
                            if (foreachMemberOrganizationData == null) 
                                continue;
                            if (foreachMemberOrganizationData.Id != orgId) 
                                continue;
                            
                            Notify.Send(foreachPlayer, NotifyType.Warning, NotifyPosition.BottomCenter, message, 3000);
                        }
                        catch (Exception e)
                        {
                            Log.Write($"sendOrganizationMessage Foreach #2 Exception: {e.ToString()}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"sendOrganizationMessage Exception: {e.ToString()}");
            }
        }

        public static void sendFractionPictureNotification(int fracid, string sender, string submessage, string message, string pic)
        {
            try
            {
                foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                {
                    var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                    if (foreachMemberFractionData == null) 
                        continue;
                    
                    if (foreachMemberFractionData.Id == fracid) 
                        NAPI.Notification.SendPictureNotificationToPlayer(foreachPlayer, message, pic, 0, 0, sender, submessage);
                }

            }
            catch (Exception e)
            {
                Log.Write($"sendFractionPictureNotification Exception: {e.ToString()}");
            }
        }
        public static bool canGetWeapon(ExtPlayer player, string weapon, bool notify = true)
        {
            try
            {
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return false;
                
                var fracId = memberFractionData.Id;
                var minRank = 100;
                if (Configs.FractionWeapons.ContainsKey(fracId) && Configs.FractionWeapons[fracId].ContainsKey(weapon)) minRank = Configs.FractionWeapons[fracId][weapon];
                if (memberFractionData.Rank < minRank)
                {
                    if (notify) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoDostup), 3000);
                    return false;
                }
                else return true;
            }
            catch (Exception e)
            {
                Log.Write($"canGetWeapon Exception: {e.ToString()}");
                return false;
            }
        }
        public static void SetSkin(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
                
                if (sessionData.WorkData.OnDutyName != String.Empty)
                {
                    if (!FractionClothingSets.SetPlayerFactionClothingSet(player, player.GetFractionId(),
                        sessionData.WorkData.OnDutyName,
                        characterData.Gender, true))
                    {                        
                        sessionData.WorkData.OnDuty = false;
                        sessionData.WorkData.OnDutyName = String.Empty;
                    }
                    return;
                }
                
                sessionData.WorkData.OnDuty = false;
                sessionData.WorkData.OnDutyName = String.Empty;  
            }
            catch (Exception e)
            {
                Log.Write($"SetSkin Exception: {e.ToString()}");
            }
        }
        public static string GetFractionRankName(int fractionId, int fracRank)
        {
            var fractionData = Manager.GetFractionData(fractionId);
            if (fractionData != null && fractionData.Ranks.ContainsKey(fracRank)) 
                return fractionData.Ranks[fracRank].Name;
            
            return "";
        }
        private static string GetDepartmentTag(int fractionId, int departmentId)
        {
            var fractionData = Manager.GetFractionData(fractionId);
            if (fractionData != null && departmentId > 0 && fractionData.Departments.ContainsKey(departmentId)) 
                return $"[{fractionData.Departments[departmentId].Tag}]";
            
            return "";
        }

        public static IReadOnlyDictionary< WeaponRepository.Hash, int> WeaponsMaxAmmo = new Dictionary< WeaponRepository.Hash, int>()
        {
            { WeaponRepository.Hash.Nightstick, 1 },
            { WeaponRepository.Hash.Pistol, 12 },
            { WeaponRepository.Hash.SMG, 30 },
            { WeaponRepository.Hash.PumpShotgun, 8 },
            { WeaponRepository.Hash.StunGun, 100 },
            { WeaponRepository.Hash.Pistol50, 9 },
            { WeaponRepository.Hash.CarbineRifle, 30 },
            { WeaponRepository.Hash.SmokeGrenade, 1 },
            { WeaponRepository.Hash.HeavyShotgun, 6 },
            { WeaponRepository.Hash.Knife, 1 },
            { WeaponRepository.Hash.SniperRifle, 10 },
            { WeaponRepository.Hash.AssaultSMG, 30 },
            { WeaponRepository.Hash.Gusenberg, 50 },
            { WeaponRepository.Hash.CombatPistol, 10 },
            { WeaponRepository.Hash.Revolver, 6 },
            { WeaponRepository.Hash.HeavyPistol, 10 },
            { WeaponRepository.Hash.SawnOffShotgun, 6 },
            { WeaponRepository.Hash.BullpupShotgun, 10 },
            { WeaponRepository.Hash.DoubleBarrelShotgun, 2 },
            { WeaponRepository.Hash.MicroSMG, 15 },
            { WeaponRepository.Hash.MachinePistol, 13 },
            { WeaponRepository.Hash.CombatPDW, 30 },
            { WeaponRepository.Hash.MiniSMG, 13 },
            { WeaponRepository.Hash.SpecialCarbine, 30 },
            { WeaponRepository.Hash.AssaultRifle, 30 },
            { WeaponRepository.Hash.BullpupRifle, 30 },
            { WeaponRepository.Hash.AdvancedRifle, 30 },
            { WeaponRepository.Hash.CompactRifle, 30 },
            { WeaponRepository.Hash.CombatMG, 100 },
        };

        public static Dictionary<int, FracMatsData> FractionDataMats = new Dictionary<int, FracMatsData>()
        {
            //для продажи нелегалок не трогать
            { 0, new FracMatsData(0, "Услуга по отмыву денег", "inv-item-briefcase", "") },
            { 1, new FracMatsData(1, Chars.Repository.ItemsInfo[ItemId.BagWithDrill].Name, Chars.Repository.ItemsInfo[ItemId.BagWithDrill].Icon, $"{Main.BlackMarketDrill}$") },
            { 2, new FracMatsData(2, Chars.Repository.ItemsInfo[ItemId.Lockpick].Name, Chars.Repository.ItemsInfo[ItemId.Lockpick].Icon, $"{Main.BlackMarketLockPick}$") },
            { 3, new FracMatsData(3, Chars.Repository.ItemsInfo[ItemId.ArmyLockpick].Name, Chars.Repository.ItemsInfo[ItemId.ArmyLockpick].Icon, $"{Main.BlackMarketArmyLockPick}$") },
            { 4, new FracMatsData(4, Chars.Repository.ItemsInfo[ItemId.Cuffs].Name, Chars.Repository.ItemsInfo[ItemId.Cuffs].Icon, $"{Main.BlackMarketCuffs}$") },
            { 5, new FracMatsData(5, Chars.Repository.ItemsInfo[ItemId.Pocket].Name, Chars.Repository.ItemsInfo[ItemId.Pocket].Icon, $"{Main.BlackMarketPocket}$") },
            { 6, new FracMatsData(6, "Понизить розыск", "sm-icon-wanted", $"{Main.BlackMarketWanted}$") },
            { 7, new FracMatsData(7, Chars.Repository.ItemsInfo[ItemId.BodyArmor].Name, Chars.Repository.ItemsInfo[ItemId.BodyArmor].Icon, $"150 мат.") },
            { 69, new FracMatsData(69, "Взломать наручники", "sm-icon-arrested", $"{Main.BlackMarketUnCuff}$") },
            { 78, new FracMatsData(78, "Лицензия на оружие", "inv-item-modifications", $"{Main.BlackMarketGunLic}$") },
            { 79, new FracMatsData(79, "Мед. карта", Chars.Repository.ItemsInfo[ItemId.Note].Icon, $"{Main.BlackMarketMedCard}$") },
            { 80, new FracMatsData(80, "QR-код", Chars.Repository.ItemsInfo[ItemId.Note].Icon, $"{Main.BlackQrFake}$") },
            { 81, new FracMatsData(81, "Радиоперехватчик", Chars.Repository.ItemsInfo[ItemId.RadioInterceptor].Icon, $"{Main.BlackRadioInterceptord}$") },

            //Для фракций
            { 8, new FracMatsData(8, Chars.Repository.ItemsInfo[ItemId.Nightstick].Name, Chars.Repository.ItemsInfo[ItemId.Nightstick].Icon, null) },
            { 9, new FracMatsData(9, Chars.Repository.ItemsInfo[ItemId.StunGun].Name, Chars.Repository.ItemsInfo[ItemId.StunGun].Icon, null) },
            { 10, new FracMatsData(10, Chars.Repository.ItemsInfo[ItemId.CombatPistol].Name, Chars.Repository.ItemsInfo[ItemId.CombatPistol].Icon, null) },
            { 11, new FracMatsData(11, Chars.Repository.ItemsInfo[ItemId.SMG].Name, Chars.Repository.ItemsInfo[ItemId.SMG].Icon, null) },
            { 12, new FracMatsData(12, Chars.Repository.ItemsInfo[ItemId.PumpShotgun].Name, Chars.Repository.ItemsInfo[ItemId.PumpShotgun].Icon, null) },
            { 13, new FracMatsData(13, Chars.Repository.ItemsInfo[ItemId.CombatPDW].Name, Chars.Repository.ItemsInfo[ItemId.CombatPDW].Icon, null) },
            { 14, new FracMatsData(14, Chars.Repository.ItemsInfo[ItemId.CarbineRifle].Name, Chars.Repository.ItemsInfo[ItemId.CarbineRifle].Icon, null) },
            { 15, new FracMatsData(15, Chars.Repository.ItemsInfo[ItemId.HeavySniper].Name, Chars.Repository.ItemsInfo[ItemId.HeavySniper].Icon, null) },
            { 16, new FracMatsData(16, Chars.Repository.ItemsInfo[ItemId.AdvancedRifle].Name, Chars.Repository.ItemsInfo[ItemId.AdvancedRifle].Icon, null) },
            { 17, new FracMatsData(17, Chars.Repository.ItemsInfo[ItemId.Gusenberg].Name, Chars.Repository.ItemsInfo[ItemId.Gusenberg].Icon, null) },
            { 18, new FracMatsData(18, Chars.Repository.ItemsInfo[ItemId.CombatMG].Name, Chars.Repository.ItemsInfo[ItemId.CombatMG].Icon, null) },
            { 19, new FracMatsData(19, Chars.Repository.ItemsInfo[ItemId.PistolAmmo].Name, Chars.Repository.ItemsInfo[ItemId.PistolAmmo].Icon, null) },
            { 20, new FracMatsData(20, Chars.Repository.ItemsInfo[ItemId.ShotgunsAmmo].Name, Chars.Repository.ItemsInfo[ItemId.ShotgunsAmmo].Icon, null) },
            { 21, new FracMatsData(21, Chars.Repository.ItemsInfo[ItemId.SMGAmmo].Name, Chars.Repository.ItemsInfo[ItemId.SMGAmmo].Icon, null) },
            //{ 22, new FracMatsData(22, $"{Chars.Repository.ItemsInfo[ItemId.SMGAmmo].Name} x100", Chars.Repository.ItemsInfo[ItemId.SMGAmmo].Icon, null) },
            { 23, new FracMatsData(23, Chars.Repository.ItemsInfo[ItemId.RiflesAmmo].Name, Chars.Repository.ItemsInfo[ItemId.RiflesAmmo].Icon, null) },
            { 24, new FracMatsData(24, Chars.Repository.ItemsInfo[ItemId.SniperAmmo].Name, Chars.Repository.ItemsInfo[ItemId.SniperAmmo].Icon, null) },



            { 25, new FracMatsData(25, "Бейдж", "sm-icon-documents", null) },
            { 26, new FracMatsData(26, "Сдать бронежилет", "sm-icon-remove-armour", null) },

            { 27, new FracMatsData(27, "Угон автотранспорта", "sm-icon-hijacking", null) },
            { 28, new FracMatsData(28, "Перевозка автотранспорта", "sm-icon-transportation", null) },


            { 29, new FracMatsData(29, "Перевозка оружия", "sm-icon-truck-ammo", null) },
            { 30, new FracMatsData(30, "Перевозка денег", "sm-icon-truck-cash", null) },
            { 31, new FracMatsData(31, "Перевозка трупов", "sm-icon-truck-corpses", null) },


            { 32, new FracMatsData(32, Chars.Repository.ItemsInfo[ItemId.HealthKit].Name, Chars.Repository.ItemsInfo[ItemId.HealthKit].Icon, null) },
            { 33, new FracMatsData(33, Chars.Repository.ItemsInfo[ItemId.BodyArmor].Name, Chars.Repository.ItemsInfo[ItemId.BodyArmor].Icon, null) },


            { 34, new FracMatsData(34, Chars.Repository.ItemsInfo[ItemId.CeramicPistol].Name, Chars.Repository.ItemsInfo[ItemId.CeramicPistol].Icon, null) },
            { 35, new FracMatsData(35, Chars.Repository.ItemsInfo[ItemId.HeavyPistol].Name, Chars.Repository.ItemsInfo[ItemId.HeavyPistol].Icon, null) },
            { 36, new FracMatsData(36, Chars.Repository.ItemsInfo[ItemId.VintagePistol]   .Name, Chars.Repository.ItemsInfo[ItemId.VintagePistol].Icon, null) },
            { 37, new FracMatsData(37, Chars.Repository.ItemsInfo[ItemId.Pistol50].Name, Chars.Repository.ItemsInfo[ItemId.Pistol50].Icon, null) },
            { 38, new FracMatsData(38, Chars.Repository.ItemsInfo[ItemId.PumpShotgun].Name, Chars.Repository.ItemsInfo[ItemId.PumpShotgun].Icon, null) },
            { 39, new FracMatsData(38, Chars.Repository.ItemsInfo[ItemId.SMGMk2].Name, Chars.Repository.ItemsInfo[ItemId.SMGMk2].Icon, null) },
            { 40, new FracMatsData(40, Chars.Repository.ItemsInfo[ItemId.MachinePistol].Name, Chars.Repository.ItemsInfo[ItemId.MachinePistol].Icon, null) },
            { 41, new FracMatsData(41, Chars.Repository.ItemsInfo[ItemId.MiniSMG].Name, Chars.Repository.ItemsInfo[ItemId.MiniSMG].Icon, null) },
            { 42, new FracMatsData(42, Chars.Repository.ItemsInfo[ItemId.CompactRifle].Name, Chars.Repository.ItemsInfo[ItemId.CompactRifle].Icon, null) },
            { 43, new FracMatsData(43, Chars.Repository.ItemsInfo[ItemId.AssaultRifle].Name, Chars.Repository.ItemsInfo[ItemId.AssaultRifle].Icon, null) },
            { 44, new FracMatsData(44, Chars.Repository.ItemsInfo[ItemId.PistolMk2].Name, Chars.Repository.ItemsInfo[ItemId.PistolMk2].Icon, null) },
            //{ 45, new FracMatsData(45, Chars.Repository.ItemsInfo[ItemId.CeramicPistol].Name, Chars.Repository.ItemsInfo[ItemId.CeramicPistol].Icon, null) },
            { 46, new FracMatsData(46, Chars.Repository.ItemsInfo[ItemId.SNSPistolMk2].Name, Chars.Repository.ItemsInfo[ItemId.SNSPistolMk2].Icon, null) },
            { 47, new FracMatsData(47, Chars.Repository.ItemsInfo[ItemId.DoubleBarrelShotgun].Name, Chars.Repository.ItemsInfo[ItemId.DoubleBarrelShotgun].Icon, null) },
            { 48, new FracMatsData(48, Chars.Repository.ItemsInfo[ItemId.SawnOffShotgun].Name, Chars.Repository.ItemsInfo[ItemId.SawnOffShotgun].Icon, null) },
            { 49, new FracMatsData(49, Chars.Repository.ItemsInfo[ItemId.PumpShotgunMk2].Name, Chars.Repository.ItemsInfo[ItemId.PumpShotgunMk2].Icon, null) },
            { 50, new FracMatsData(50, Chars.Repository.ItemsInfo[ItemId.SweeperShotgun].Name, Chars.Repository.ItemsInfo[ItemId.SweeperShotgun].Icon, null) },
            { 51, new FracMatsData(51, Chars.Repository.ItemsInfo[ItemId.MicroSMG].Name, Chars.Repository.ItemsInfo[ItemId.MicroSMG].Icon, null) },
            { 52, new FracMatsData(52, Chars.Repository.ItemsInfo[ItemId.SMGMk2].Name, Chars.Repository.ItemsInfo[ItemId.SMGMk2].Icon, null) },
            { 53, new FracMatsData(53, Chars.Repository.ItemsInfo[ItemId.AssaultRifleMk2].Name, Chars.Repository.ItemsInfo[ItemId.AssaultRifleMk2].Icon, null) },
            { 54, new FracMatsData(54, Chars.Repository.ItemsInfo[ItemId.BullpupRifle].Name, Chars.Repository.ItemsInfo[ItemId.BullpupRifle].Icon, null) },
            { 55, new FracMatsData(55, Chars.Repository.ItemsInfo[ItemId.APPistol].Name, Chars.Repository.ItemsInfo[ItemId.APPistol].Icon, null) },
            { 56, new FracMatsData(56, Chars.Repository.ItemsInfo[ItemId.GrenadeLauncherSmoke].Name, Chars.Repository.ItemsInfo[ItemId.GrenadeLauncherSmoke].Icon, null) },
            { 57, new FracMatsData(57, Chars.Repository.ItemsInfo[ItemId.AssaultShotgun].Name, Chars.Repository.ItemsInfo[ItemId.AssaultShotgun].Icon, null) },
            { 58, new FracMatsData(58, Chars.Repository.ItemsInfo[ItemId.AssaultSMG].Name, Chars.Repository.ItemsInfo[ItemId.AssaultSMG].Icon, null) },
            //59
            { 60, new FracMatsData(60, Chars.Repository.ItemsInfo[ItemId.CombatMGMk2].Name, Chars.Repository.ItemsInfo[ItemId.CombatMGMk2].Icon, null) },
            { 61, new FracMatsData(61, Chars.Repository.ItemsInfo[ItemId.CarbineRifleMk2].Name, Chars.Repository.ItemsInfo[ItemId.CarbineRifleMk2].Icon, null) },
            { 62, new FracMatsData(62, Chars.Repository.ItemsInfo[ItemId.BullpupRifleMk2].Name, Chars.Repository.ItemsInfo[ItemId.BullpupRifleMk2].Icon, null) },
            { 63, new FracMatsData(63, Chars.Repository.ItemsInfo[ItemId.MarksmanRifleMk2].Name, Chars.Repository.ItemsInfo[ItemId.MarksmanRifleMk2].Icon, null) },
            { 64, new FracMatsData(64, Chars.Repository.ItemsInfo[ItemId.HeavyShotgun].Name, Chars.Repository.ItemsInfo[ItemId.HeavyShotgun].Icon, null) },
            { 65, new FracMatsData(65, Chars.Repository.ItemsInfo[ItemId.BullpupShotgun].Name, Chars.Repository.ItemsInfo[ItemId.BullpupShotgun].Icon, null) },
            { 66, new FracMatsData(66, Chars.Repository.ItemsInfo[ItemId.SpecialCarbine].Name, Chars.Repository.ItemsInfo[ItemId.SpecialCarbine].Icon, null) },
            { 67, new FracMatsData(67, Chars.Repository.ItemsInfo[ItemId.SpecialCarbineMk2].Name, Chars.Repository.ItemsInfo[ItemId.SpecialCarbineMk2].Icon, null) },
            { 68, new FracMatsData(68, Chars.Repository.ItemsInfo[ItemId.SniperRifle].Name, Chars.Repository.ItemsInfo[ItemId.SniperRifle].Icon, null) },

            //Магазин фейверков
            { 70, new FracMatsData(70, Chars.Repository.ItemsInfo[ItemId.Firework1].Name, Chars.Repository.ItemsInfo[ItemId.Firework1].Icon, Main.PricesSettings.FireworkPrices[0]+"$") },
            { 71, new FracMatsData(71, Chars.Repository.ItemsInfo[ItemId.Firework2].Name, Chars.Repository.ItemsInfo[ItemId.Firework2].Icon, Main.PricesSettings.FireworkPrices[1]+"$") },
            { 72, new FracMatsData(72, Chars.Repository.ItemsInfo[ItemId.Firework3].Name, Chars.Repository.ItemsInfo[ItemId.Firework3].Icon, Main.PricesSettings.FireworkPrices[2]+"$") },
            { 73, new FracMatsData(73, Chars.Repository.ItemsInfo[ItemId.Firework4].Name, Chars.Repository.ItemsInfo[ItemId.Firework4].Icon, Main.PricesSettings.FireworkPrices[3]+"$") },

            //Охотничий магазин
            { 74, new FracMatsData(74, Chars.Repository.ItemsInfo[ItemId.WorkAxe].Name, Chars.Repository.ItemsInfo[ItemId.WorkAxe].Icon, Main.PricesSettings.InstrumentPrices[0]+"$") },
            { 75, new FracMatsData(75, Chars.Repository.ItemsInfo[ItemId.Pickaxe1].Name, Chars.Repository.ItemsInfo[ItemId.Pickaxe1].Icon, Main.PricesSettings.InstrumentPrices[1]+"$") },
            { 76, new FracMatsData(76, Chars.Repository.ItemsInfo[ItemId.Pickaxe2].Name, Chars.Repository.ItemsInfo[ItemId.Pickaxe2].Icon, Main.PricesSettings.InstrumentPrices[2]+"$") },
            { 77, new FracMatsData(77, Chars.Repository.ItemsInfo[ItemId.Pickaxe3].Name, Chars.Repository.ItemsInfo[ItemId.Pickaxe3].Icon, Main.PricesSettings.InstrumentPrices[3]+"$") },
            
            // Предметы новые у пд
            { 82, new FracMatsData(82, Chars.Repository.ItemsInfo[ItemId.Konus].Name, Chars.Repository.ItemsInfo[ItemId.Konus].Icon, null) },
            { 83, new FracMatsData(83, Chars.Repository.ItemsInfo[ItemId.Konuss].Name, Chars.Repository.ItemsInfo[ItemId.Konuss].Icon, null) },
            { 84, new FracMatsData(84, Chars.Repository.ItemsInfo[ItemId.Otboynik1].Name, Chars.Repository.ItemsInfo[ItemId.Otboynik1].Icon, null) },
            { 85, new FracMatsData(85, Chars.Repository.ItemsInfo[ItemId.Otboynik2].Name, Chars.Repository.ItemsInfo[ItemId.Otboynik2].Icon, null) },
            { 86, new FracMatsData(86, Chars.Repository.ItemsInfo[ItemId.Dontcross].Name, Chars.Repository.ItemsInfo[ItemId.Dontcross].Icon, null) },
            { 87, new FracMatsData(87, Chars.Repository.ItemsInfo[ItemId.Stop].Name, Chars.Repository.ItemsInfo[ItemId.Stop].Icon, null) },
            { 88, new FracMatsData(88, Chars.Repository.ItemsInfo[ItemId.NetProezda].Name, Chars.Repository.ItemsInfo[ItemId.NetProezda].Icon, null) },
            { 89, new FracMatsData(89, Chars.Repository.ItemsInfo[ItemId.Kpp].Name, Chars.Repository.ItemsInfo[ItemId.Kpp].Icon, null) },
            { 90, new FracMatsData(90, Chars.Repository.ItemsInfo[ItemId.Zabor1].Name, Chars.Repository.ItemsInfo[ItemId.Zabor1].Icon, null) },
            { 91, new FracMatsData(91, Chars.Repository.ItemsInfo[ItemId.Zabor2].Name, Chars.Repository.ItemsInfo[ItemId.Zabor2].Icon, null) },
            { 92, new FracMatsData(92, Chars.Repository.ItemsInfo[ItemId.Airlight].Name, Chars.Repository.ItemsInfo[ItemId.Airlight].Icon, null) },
            { 93, new FracMatsData(93, Chars.Repository.ItemsInfo[ItemId.Camera1].Name, Chars.Repository.ItemsInfo[ItemId.Camera1].Icon, null) },
            { 94, new FracMatsData(94, Chars.Repository.ItemsInfo[ItemId.Camera2].Name, Chars.Repository.ItemsInfo[ItemId.Camera2].Icon, null) },
        };


        public static void OpenFractionSM(ExtPlayer player, string type)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (FractionDataMats.Count == 0) return;
                List<int> ListItems = new List<int>();
                string title = "";
                string titleIcon = "";
                switch (type)
                {
                    case "fbi":
                        title = "Выдача оружия";
                        titleIcon = "sm-icon-logo-fib";
                        ListItems = new List<int>() { 9, 10, 44, 55, 35/*, 56*/, 49, 48, 57, 50, 13, 11, 58, 60, 61, 62, 16, 15, 63, 33, 32, 19, 20, 21, 23, 24, 25, 26, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94 };
                        break;
                    case "gov":
                        title = "Выдача оружия";
                        titleIcon = "sm-icon-logo-gov";
                        ListItems = new List<int>() { 9, 10, 44, 37, 35, 11, 58, 13, 16, 14, 33, 32, 19, 20, 21, 23, 24, 26 };
                        break;
                    case "polic":
                        title = "Выдача оружия";
                        titleIcon = "sm-icon-logo-polic";
                        ListItems = new List<int>() { 8, 9, 44, 37, 10, 35, 34, 49, 64, 48, 65, 11, 13, 14, 54, 33, 32, 19, 20, 21, 23, 24, 26, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94 };
                        break;
                    case "army":
                        title = "Выдача оружия";
                        titleIcon = "sm-icon-logo-army";
                        ListItems = new List<int>() { 9, 10, 44, 37, 35, 49, 48, 65, 13, 11, 18, 14, 61, 66, 67, 68, 63, 33, 32, 19, 20, 21, 23, 24, 26 };
                        break;
                    case "gang":
                        title = "Выдача миссии";
                        titleIcon = "sm-icon-logo-gang";
                        ListItems = new List<int>() { 27, 28 };
                        break;
                    case "mafia":
                        title = "Выдача миссии";
                        titleIcon = "sm-icon-mafia";
                        ListItems = new List<int>() { 29, 30, 31 };
                        break;
                    case "biker":
                        title = "Выдача оружия";
                        titleIcon = "sm-icon-logo-fib";
                        ListItems = new List<int>() { 29, 30, 31 };
                        break;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
                if (ListItems.Count == 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomethingWrong), 3000);
                    return;
                }
                List<FracMatsData> _JsonData = new List<FracMatsData>();

                foreach (int i in ListItems)
                {
                    if (FractionDataMats.ContainsKey(i)) _JsonData.Add(FractionDataMats[i]);
                }

                Trigger.ClientEvent(player, "client.sm.open", title, titleIcon, JsonConvert.SerializeObject(_JsonData));
            }
            catch (Exception e)
            {
                Log.Write($"OpenFractionSM Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.sm.fraction")]
        public static void callback_policeGuns(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (player.Position.DistanceTo(Police.GunsPosition) >= 5 &&
                    player.Position.DistanceTo(Fbi.fbiCheckpoints[5]) >= 5 &&
                    player.Position.DistanceTo(Fbi.fbiCheckpoints[13]) >= 5 &&
                    player.Position.DistanceTo(Cityhall.GiveGunPosition) >= 5 &&
                    //player.Position.DistanceTo(Cityhall.SecondGiveGunPosition) >= 5 &&
                    player.Position.DistanceTo(Army.ArmyCheckpoints[0]) >= 5 &&
                    player.Position.DistanceTo(Sheriff.FirstGunsPosition) >= 5 &&
                    player.Position.DistanceTo(Sheriff.SecondGunsPosition) >= 5 &&
                    (index < 27 || index > 31))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TooFarFromVidacha), 3000);
                    return;
                }
                var fractionData = player.GetFractionData();
                if (fractionData == null)
                    return;
                /*else if (memberFractionData.Id == (int) Models.Fractions.CITY)
                {
                    switch (memberFractionData.Rank)
                    {
                        case 4:
                        case 7:
                        case 10:
                        case 13:
                        case 16:
                        case 17:
                        case 18:
                        case 19:
                        case 20:
                            break;
                        default:
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OnlyForceGov), 3000);
                            return;
                    }
                }*/

                sessionData.ActiveIndex = index;

                switch (index)
                {
                    case 33:
                        if (!canGetWeapon(player, "Armor")) return;
                        if (fractionData.Materials < matsForArmor)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseNoMats), 3000);
                            return;
                        }
                        if (Chars.Repository.itemCount(player, "inventory", ItemId.BodyArmor) >= Chars.Repository.maxItemCount) 
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyHaveBronik), 3000);
                            return;
                        }
                        if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.BodyArmor, 1, 100.ToString()) == -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                        fractionData.Materials -= matsForArmor;
                        fractionData.UpdateLabel();
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GetBronik), 3000);
                        GameLog.Stock(fractionData.Id, characterData.UUID, player.Name, "armor", 1, "out");
                        Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMats, LangFunc.GetText(LangType.Ru, DataName.VzyalBron));

                        if (fractionData.Id == (int) Models.Fractions.POLICE || fractionData.Id == (int) Models.Fractions.SHERIFF || fractionData.Id == (int) Models.Fractions.CITY || fractionData.Id == (int) Models.Fractions.ARMY || fractionData.Id == (int) Models.Fractions.FIB)
                            sendFractionMessage(fractionData.Id, LangFunc.GetText(LangType.Ru, DataName.FTakenBronik, player.Name, player.Value), true);
                        return;
                    case 26:
                        if (!FunctionsAccess.IsWorking("armorremove"))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                            return;
                        }
                        if (!canGetWeapon(player, "Armor")) return;
                        ItemStruct ItemStruct = Chars.Repository.isItem(player, "inventory", ItemId.BodyArmor);
                        if (ItemStruct == null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoBronik), 3000);
                            return;
                        }
                        int armorp;
                        if (ItemStruct.Location == "accessories")
                        {
                            if (sessionData.ArmorHealth != -1 && player.Armor > sessionData.ArmorHealth)
                            {
                                Trigger.SendToAdmins(3, LangFunc.GetText(LangType.Ru, DataName.AdminACBronik, player.Name));
                                armorp = 0;
                            }
                            else armorp = player.Armor;
                            player.Armor = 0;
                        }
                        else armorp = Convert.ToInt32(ItemStruct.Item.Data);
                        int matstoadd;
                        if (armorp >= 76) matstoadd = 150;
                        else if (armorp >= 50) matstoadd = 100;
                        else if (armorp >= 1) matstoadd = 50;
                        else matstoadd = 0;
                        fractionData.Materials = fractionData.Materials + matstoadd > 300000 ? 300000 : fractionData.Materials + matstoadd;
                        fractionData.UpdateLabel();
                        Chars.Repository.RemoveIndex(player, ItemStruct.Location, ItemStruct.Index, 1);
                        Trigger.ClientEvent(player, "client.isArmor", false);
                        sessionData.ArmorHealth = -1;
                        ClothesComponents.SetSpecialClothes(player, 9, 0, 0);
                        ClothesComponents.UpdateClothes(player);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BronikReturn, armorp, matstoadd), 5000);
                        
                        if (fractionData.Id == (int) Models.Fractions.ARMY)
                            player.AddTableScore(TableTaskId.Item18);

                        if (fractionData.Id == (int) Models.Fractions.POLICE || fractionData.Id == (int) Models.Fractions.SHERIFF || fractionData.Id == (int) Models.Fractions.CITY || fractionData.Id == (int) Models.Fractions.ARMY || fractionData.Id == (int) Models.Fractions.FIB)
                            sendFractionMessage(fractionData.Id, LangFunc.GetText(LangType.Ru, DataName.SdalBronik, player.Name, player.Value, armorp, matstoadd), true);
                        return;
                    case 32: // medkit
                        if (!canGetWeapon(player, "Medkits")) return;
                        if (fractionData.MedKits == 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseNoAptekas), 3000);
                            return;
                        }
                        if (Chars.Repository.isFreeSlots(player, ItemId.HealthKit) != 0) return;
                        Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.HealthKit, 1);
                        fractionData.MedKits--;
                        fractionData.UpdateLabel();
                        GameLog.Stock(fractionData.Id, characterData.UUID, player.Name, "medkit", 1, "out");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GotApteka), 3000);
                        Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMedkits, LangFunc.GetText(LangType.Ru, DataName.GetApteka));

                        if (fractionData.Id == (int) Models.Fractions.POLICE || fractionData.Id == (int) Models.Fractions.SHERIFF || fractionData.Id == (int) Models.Fractions.CITY || fractionData.Id == (int) Models.Fractions.ARMY || fractionData.Id == (int) Models.Fractions.FIB)
                            sendFractionMessage(fractionData.Id, LangFunc.GetText(LangType.Ru, DataName.FTakeApteka, player.Name, player.Value), true);
                        return;

                    case 25:
                        if (!canGetWeapon(player, "FIBB")) return;
                        string data = (characterData.Gender) ? "128_0_true" : "98_0_false";
                        if (Chars.Repository.isItem(player, "inventory", ItemId.Jewelry, data) != null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouAlreadyHaveBadge), 3000);
                            return;
                        }

                        if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Jewelry, 1, data) == -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                        Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeSpecial, LangFunc.GetText(LangType.Ru, DataName.GetNewBadge));
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGotFibBadge), 3000);
                        return;

                    case 27:
                    case 28:
                        CarDelivery.Event_gangMission(player, index - 27);
                        return;

                    case 29:
                    case 30:
                    case 31:
                        CarDelivery.Event_mafiaMission(player, index - 29);
                        //CarDelivery.Event_bikerMission(player, index - 29);
                        return;
                    case 19:
                        if (!canGetWeapon(player, "PistolAmmo")) return;
                        Trigger.ClientEvent(player, "openInput", LangFunc.GetText(LangType.Ru, DataName.Ammo), LangFunc.GetText(LangType.Ru, DataName.EnterAmmoHoches), 4, "take_frac_ammo");
                        return;
                    case 20:
                        if (!canGetWeapon(player, "ShotgunsAmmo")) return;
                        Trigger.ClientEvent(player, "openInput", LangFunc.GetText(LangType.Ru, DataName.Ammo), LangFunc.GetText(LangType.Ru, DataName.EnterAmmoHoches), 4, "take_frac_ammo");
                        return;
                    case 21:
                        if (!canGetWeapon(player, "SMGAmmo")) return;
                        Trigger.ClientEvent(player, "openInput", LangFunc.GetText(LangType.Ru, DataName.Ammo), LangFunc.GetText(LangType.Ru, DataName.EnterAmmoHoches), 4, "take_frac_ammo");
                        return;
                    case 23:
                        if (!canGetWeapon(player, "RiflesAmmo")) return;
                        Trigger.ClientEvent(player, "openInput", LangFunc.GetText(LangType.Ru, DataName.Ammo), LangFunc.GetText(LangType.Ru, DataName.EnterAmmoHoches), 4, "take_frac_ammo");
                        return;
                    case 24:
                        if (!canGetWeapon(player, "SniperAmmo")) return;
                        Trigger.ClientEvent(player, "openInput", LangFunc.GetText(LangType.Ru, DataName.Ammo), LangFunc.GetText(LangType.Ru, DataName.EnterAmmoHoches), 4, "take_frac_ammo");
                        return;
                    case 18:
                        giveGun(player, WeaponRepository.Hash.CombatMG, "CombatMG");
                        return;
                    case 8:
                        giveGun(player, WeaponRepository.Hash.Nightstick, "Nightstick");
                        return;
                    case 9:
                        giveGun(player, WeaponRepository.Hash.StunGun, "StunGun");
                        return;
                    case 44:
                        giveGun(player, WeaponRepository.Hash.PistolMk2, "PistolMk2");
                        return;
                    case 10:
                        giveGun(player, WeaponRepository.Hash.CombatPistol, "CombatPistol");
                        return;
                    case 37:
                        giveGun(player, WeaponRepository.Hash.Pistol50, "Pistol50");
                        return;
                    case 35:
                        giveGun(player, WeaponRepository.Hash.HeavyPistol, "HeavyPistol");
                        return;
                    case 11:
                        giveGun(player, WeaponRepository.Hash.SMG, "SMG");
                        return;
                    case 58:
                        giveGun(player, WeaponRepository.Hash.AssaultSMG, "AssaultSMG");
                        return;
                    case 13:
                        giveGun(player, WeaponRepository.Hash.CombatPDW, "CombatPDW");
                        return;
                    case 16:
                        giveGun(player, WeaponRepository.Hash.AdvancedRifle, "AdvancedRifle");
                        return;
                    case 14:
                        giveGun(player, WeaponRepository.Hash.CarbineRifle, "CarbineRifle");
                        return;
                    case 55:
                        giveGun(player, WeaponRepository.Hash.APPistol, "APPistol");
                        return;
                    case 56:
                        giveGun(player, WeaponRepository.Hash.GrenadeLauncherSmoke, "GrenadeLauncherSmoke");
                        return;
                    case 49:
                        giveGun(player, WeaponRepository.Hash.PumpShotgunMk2, "PumpShotgunMk2");
                        return;
                    case 48:
                        giveGun(player, WeaponRepository.Hash.SawnOffShotgun, "SawnOffShotgun");
                        return;
                    case 57:
                        giveGun(player, WeaponRepository.Hash.AssaultShotgun, "AssaultShotgun");
                        return;
                    case 50:
                        giveGun(player, WeaponRepository.Hash.SweeperShotgun, "SweeperShotgun");
                        return;
                    case 60:
                        giveGun(player, WeaponRepository.Hash.CombatMGMk2, "CombatMGMk2");
                        return;
                    case 61:
                        giveGun(player, WeaponRepository.Hash.CarbineRifleMk2, "CarbineRifleMk2");
                        return;
                    case 62:
                        giveGun(player, WeaponRepository.Hash.BullpupRifleMk2, "BullpupRifleMk2");
                        return;
                    case 15:
                        giveGun(player, WeaponRepository.Hash.HeavySniper, "HeavySniper");
                        return;
                    case 63:
                        giveGun(player, WeaponRepository.Hash.MarksmanRifleMk2, "MarksmanRifleMk2");
                        return;
                    case 34:
                        giveGun(player, WeaponRepository.Hash.CeramicPistol, "CeramicPistol");
                        return;
                    case 64:
                        giveGun(player, WeaponRepository.Hash.HeavyShotgun, "HeavyShotgun");
                        return;

                    case 65:
                        giveGun(player, WeaponRepository.Hash.BullpupShotgun, "BullpupShotgun");
                        return;
                    case 54:
                        giveGun(player, WeaponRepository.Hash.BullpupRifle, "BullpupRifle");
                        return;
                    case 66:
                        giveGun(player, WeaponRepository.Hash.SpecialCarbine, "SpecialCarbine");
                        return;
                    case 67:
                        giveGun(player, WeaponRepository.Hash.SpecialCarbineMk2, "SpecialCarbineMk2");
                        return;
                    case 68:
                        giveGun(player, WeaponRepository.Hash.SniperRifle, "SniperRifle");
                        return;
                    case 82:
                        if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Konus, 1, "0") == -1)
                        {
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                     
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGotItem, Chars.Repository.ItemsInfo[ItemId.Konus].Name), 3000);
                        return;
                    case 83:
                        if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Konuss, 1, "0") == -1)
                        {
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                        
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGotItem, Chars.Repository.ItemsInfo[ItemId.Konuss].Name), 3000);
                        return;
                    case 84:
                        if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Otboynik1, 1, "0") == -1)
                        {
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                       
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGotItem, Chars.Repository.ItemsInfo[ItemId.Otboynik1].Name), 3000);
                        return;
                    case 85:
                        if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Otboynik2, 1, "0") == -1)
                        {
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                     
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGotItem, Chars.Repository.ItemsInfo[ItemId.Otboynik2].Name), 3000);
                        return;
                    case 86:
                        if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Dontcross, 1, "0") == -1)
                        {
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                      
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGotItem, Chars.Repository.ItemsInfo[ItemId.Dontcross].Name), 3000);
                        return;
                    case 87:
                        if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Stop, 1, "0") == -1)
                        {
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                      
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGotItem, Chars.Repository.ItemsInfo[ItemId.Stop].Name), 3000);
                        return;
                    case 88:
                        if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.NetProezda, 1, "0") == -1)
                        {
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                        
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGotItem, Chars.Repository.ItemsInfo[ItemId.NetProezda].Name), 3000);
                        return;
                    case 89:
                        if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Kpp, 1, "0") == -1)
                        {
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                       
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGotItem, Chars.Repository.ItemsInfo[ItemId.Kpp].Name), 3000);
                        return;
                    case 90:
                        if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Zabor1, 1, "0") == -1)
                        {
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                        
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGotItem, Chars.Repository.ItemsInfo[ItemId.Zabor1].Name), 3000);
                        return;
                    case 91:
                        if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Zabor2, 1, "0") == -1)
                        {
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                        
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGotItem, Chars.Repository.ItemsInfo[ItemId.Zabor2].Name), 3000);
                        return;
                    case 92:
                        if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Airlight, 1, "0") == -1)
                        {
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                       
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGotItem, Chars.Repository.ItemsInfo[ItemId.Airlight].Name), 3000);
                        return;
                    case 93:
                        if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Camera1, 1, "0") == -1)
                        {
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                      
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGotItem, Chars.Repository.ItemsInfo[ItemId.Camera1].Name), 3000);
                        return;
                    case 94:
                        if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Camera2, 1, "0") == -1)
                        {
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                       
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGotItem, Chars.Repository.ItemsInfo[ItemId.Camera1].Name), 3000);
                        return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"callback_policeGuns Exception: {e.ToString()}");
            }
        }

        public static void giveGun(ExtPlayer player, WeaponRepository.Hash gun, string weaponstr)
        {
            try
            {			
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null)
                    return;
                
                if (player.HasData($"GET_{gun.ToString()}") && DateTime.Now < player.GetData<DateTime>($"GET_{gun.ToString()}"))
                {
                    DateTime date = player.GetData<DateTime>($"GET_{gun.ToString()}");
                    long ticks = date.Ticks - DateTime.Now.Ticks;
                    if (ticks <= 0) return;
                    DateTime g = new DateTime(ticks);
                    if (g.Hour >= 1) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCanTakeGun1h, gun.ToString(), g.Hour, g.Minute, g.Second), 5000);
                    else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCanTakeGun, gun.ToString(), g.Minute, g.Second), 5000);
                    return;
                }
                int frac = memberFractionData.Id;
                if (!Configs.FractionWeapons[frac].ContainsKey(weaponstr))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ErrorDostupWeapon), 3000);
                    return;
                }
                if (memberFractionData.Rank < Configs.FractionWeapons[frac][weaponstr])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoDostupWeapon), 3000);
                    return;
                }
                if (fractionData.Materials < matsForGun[gun])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseNoMats), 3000);
                    return;
                }

                ItemId wType = (ItemId)Enum.Parse(typeof(ItemId), gun.ToString());
                string serial = WeaponRepository.GetSerial(true, memberFractionData.Id);
                if ( WeaponRepository.GiveWeapon(player, wType, serial) == -1) return;

                fractionData.Materials -= matsForGun[gun];
                fractionData.UpdateLabel();
                Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMats, LangFunc.GetText(LangType.Ru, DataName.YouVzyalItem, gun.ToString(), serial));
                int minutes = 5;
                if (memberFractionData.Id == (int) Models.Fractions.POLICE || memberFractionData.Id == (int) Models.Fractions.SHERIFF) minutes = 10;
                player.SetData($"GET_{gun.ToString()}", DateTime.Now.AddMinutes(minutes));

                GameLog.Stock(memberFractionData.Id, player.GetUUID(), player.Name, $"{gun.ToString()}({serial})", 1, "out");
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGotItem, wType.ToString()), 3000);

                if (memberFractionData.Id == (int) Models.Fractions.POLICE || memberFractionData.Id == (int) Models.Fractions.SHERIFF || memberFractionData.Id == (int) Models.Fractions.CITY || memberFractionData.Id == (int) Models.Fractions.ARMY || memberFractionData.Id == (int) Models.Fractions.FIB)
                    sendFractionMessage(memberFractionData.Id, LangFunc.GetText(LangType.Ru, DataName.FTakeGun, player.Name, player.Value, gun.ToString(), serial), true);
            }
            catch (Exception e)
            {
                Log.Write($"giveGun Exception: {e.ToString()}");
            }
        }

        public static void giveAmmo(ExtPlayer player, ItemId ammoType, int ammo)
        {
            try
            {		
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
   
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null)
                    return;
                
                if (fractionData.Materials < MatsForAmmoType[ammoType] * ammo)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseNoMats), 3000);
                    return;
                }
                if (Chars.Repository.isFreeSlots(player, ammoType, ammo) != 0) return;
                Chars.Repository.AddNewItem(player, $"char_{player.GetUUID()}", "inventory", ammoType, ammo);
                fractionData.Materials -= MatsForAmmoType[ammoType] * ammo;
                fractionData.UpdateLabel();
                Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMats, LangFunc.GetText(LangType.Ru, DataName.YouVzyalItem, Chars.Repository.ItemsInfo[ammoType].Name, ammo));
                GameLog.Stock(memberFractionData.Id, player.GetUUID(), player.Name, ammoType.ToString(), 1, "out");
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGotItem, Chars.Repository.ItemsInfo[ammoType].Name, ammo), 3000);

                if (memberFractionData.Id == (int) Models.Fractions.POLICE || memberFractionData.Id == (int) Models.Fractions.SHERIFF || memberFractionData.Id == (int) Models.Fractions.CITY || memberFractionData.Id == (int) Models.Fractions.ARMY || memberFractionData.Id == (int) Models.Fractions.FIB)
                    sendFractionMessage(memberFractionData.Id, LangFunc.GetText(LangType.Ru, DataName.FTakeAmmo, player.Name, player.Value, Chars.Repository.ItemsInfo[ammoType].Name, ammo), true);
            }
            catch (Exception e)
            {
                Log.Write($"giveAmmo Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.takeFractionAmmo")]
        public static void TakeFractionAmmo(ExtPlayer player, int ammo)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                if (ammo >= 1)
                {
                    switch (sessionData.ActiveIndex)
                    {
                        case 19:
                            if (!canGetWeapon(player, "PistolAmmo")) return;
                            giveAmmo(player, ItemId.PistolAmmo, ammo);
                            return;
                        case 20:
                            if (!canGetWeapon(player, "ShotgunsAmmo")) return;
                            giveAmmo(player, ItemId.ShotgunsAmmo, ammo);
                            return;
                        case 21:
                            if (!canGetWeapon(player, "SMGAmmo")) return;
                            giveAmmo(player, ItemId.SMGAmmo, ammo);
                            return;
                        case 23:
                            if (!canGetWeapon(player, "RiflesAmmo")) return;
                            giveAmmo(player, ItemId.RiflesAmmo, ammo);
                            return;
                        case 24:
                            if (!canGetWeapon(player, "SniperAmmo")) return;
                            giveAmmo(player, ItemId.SniperAmmo, ammo);
                            return;
                        default:
                            break;
                    }
                }
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IncrorectInput), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"enterInterier Exception: {e.ToString()}");
            }
        }

        public class FracMatsData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Icon { get; set; }
            public string Price { get; set; }
            public int ItemId { get; set; }

            public FracMatsData(int Id,  string Name, string Icon, string Price, int ItemId = 0)
            {
                this.Id = Id;
                this.Name = Name;
                this.Icon = Icon;
                this.Price = Price;
                this.ItemId = ItemId;
            }
        }


        
        public class CraftData
        {
            public string Name { get; set; }
            public string Icon { get; set; }
            public int Mats { get; set; }
            public CraftData(string Name, string Icon, int Mats)
            {
                this.Name = Name;
                this.Icon = Icon;
                this.Mats = Mats;
            }
        }

        #region CraftMenu
        public static void OpenGunCraftMenu(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                var fracId = player.GetFractionId();
                List<List<string>> list = null;
                if (FractionTypes[fracId] == FractionsType.None || FractionTypes[fracId] == FractionsType.Gov)
                {
                    if (FractionTypes[fracId] == FractionsType.None)
                    {
                        if (player.IsOrganizationMemberData()) list = organizationsGuns.ToList();
                        else
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantCraftWeapon), 3000);
                            return;
                        }
                    }
                    else
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantCraftWeapon), 3000);
                        return;
                    }
                }
                else if (FractionTypes[fracId] == FractionsType.Bikers) list = bikersGuns.ToList();
                else if (FractionTypes[fracId] == FractionsType.Nongov) list = nongovGuns.ToList();
                else if (FractionTypes[fracId] == FractionsType.Gangs) list = gangGuns.ToList();
                else if (FractionTypes[fracId] == FractionsType.Mafia) list = mafiaGuns.ToList();


                List<List<CraftData>> _WeaponsData = new List<List<CraftData>>();
                for (int i = 0; i < list.Count; i++)
                {
                    List<CraftData> _CraftsData = new List<CraftData>();
                    foreach (string g in list[i])
                    {
                        ItemId wType = (ItemId)Enum.Parse(typeof(ItemId), g);
                        if (Chars.Repository.ItemsInfo.ContainsKey(wType))
                        {
                            _CraftsData.Add(new CraftData(Chars.Repository.ItemsInfo[wType].Name, Chars.Repository.ItemsInfo[wType].Icon, matsForGun[ WeaponRepository.GetHash(g)]));
                        }
                    }
                    _WeaponsData.Add(_CraftsData);
                }
                List<int> _AmmoData = new List<int>();
                foreach (int ammo in MatsForAmmoType.Values) _AmmoData.Add(ammo);
                Trigger.ClientEvent(player, "client.fraction.craft.open", fracId, JsonConvert.SerializeObject(_WeaponsData), JsonConvert.SerializeObject(_AmmoData));
            }
            catch (Exception e)
            {
                Log.Write($"OpenGunCraftMenu Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.craft.create")]
        public static void Event_WCraft(ExtPlayer player, int frac, int cat, int index)
        {
            try
            {			
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                List<List<string>> list = null;
                bool org = false;
                if (FractionTypes[frac] == FractionsType.None || FractionTypes[frac] == FractionsType.Gov)
                {
                    if (FractionTypes[frac] == FractionsType.None)
                    {
                        var organizationData = player.GetOrganizationData();
                        if (organizationData != null)
                        {
                            org = true;
                            list = organizationsGuns.ToList();
                            string familySelected = list[cat][index];
                            if (!organizationData.Schemes.ContainsKey(familySelected) || !organizationData.Schemes[familySelected])
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FamilyDoesntSel, familySelected), 3000);
                                return;
                            }
                        }
                        else
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantCraftWeapon), 3000);
                            return;
                        }
                    }
                    else
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantCraftWeapon), 3000);
                        return;
                    }
                }
                else if (FractionTypes[frac] == FractionsType.Gangs) list = gangGuns.ToList();
                else if (FractionTypes[frac] == FractionsType.Mafia) list = mafiaGuns.ToList();
                else if (FractionTypes[frac] == FractionsType.Nongov) list = nongovGuns.ToList();
                else if (FractionTypes[frac] == FractionsType.Bikers) list = bikersGuns.ToList();
                if (list.Count < 1 || list.Count < cat + 1 || list[cat].Count < index + 1) return;
                ItemStruct mItem = Chars.Repository.isItem(player, "inventory", ItemId.Material);
                int count = (mItem == null) ? 0 : mItem.Item.Count;
                string selected = list[cat][index];
                if (frac == (int) Models.Fractions.MERRYWEATHER)
                {
                    switch (selected)
                    {
                        case "PistolMk2":
                        case "StunGun":
                        case "CombatPistol":
                            if (memberFractionData.Rank <= 1)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Крафт {selected} для MerryWeather доступен со 2 ранга.", 3000);
                                return;
                            }
                            break;
                        case "HeavyPistol":
                        case "Pistol50":
                            if (memberFractionData.Rank <= 2)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Крафт {selected} для MerryWeather доступен со 3 ранга.", 3000);
                                return;
                            }
                            break;
                        case "SMG":
                        case "CombatPDW":
                        case "SawnOffShotgun":
                            if (memberFractionData.Rank <= 3)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Крафт {selected} для MerryWeather доступен с 4 ранга.", 3000);
                                return;
                            }
                            break;
                        case "PumpShotgunMk2":
                        case "BullpupShotgun":
                        case "AdvancedRifle":
                        case "CarbineRifle":
                        case "SMGMk2":
                            if (memberFractionData.Rank <= 4)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Крафт {selected} для MerryWeather доступен с 5 ранга.", 3000);
                                return;
                            }
                            break;
                        case "AssaultRifleMk2":
                        case "CarbineRifleMk2":
                            if (memberFractionData.Rank <= 5)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Крафт {selected} для MerryWeather доступен с 6 ранга.", 3000);
                                return;
                            }
                            break;
                        case "APPistol":
                        case "AssaultShotgun":
                        case "SweeperShotgun":
                        case "AssaultSMG":
                        case "BullpupRifle":
                            if (memberFractionData.Rank <= 7)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Крафт {selected} для MerryWeather доступен с 8 ранга.", 3000);
                                return;
                            }
                            break;
                        default:
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CraftError), 3000);
                            return;
                    }
                }
                if (count < matsForGun[ WeaponRepository.GetHash(selected)])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMats), 3000);
                    return;
                }
                ItemId wType = (ItemId)Enum.Parse(typeof(ItemId), selected);
                if ( WeaponRepository.GiveWeapon(player, wType, WeaponRepository.GetSerial(true, frac, org)) == -1) return;
                Chars.Repository.RemoveIndex(player, mItem.Location, mItem.Index, matsForGun[ WeaponRepository.GetHash(selected)]);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы скрафтили {selected} за {matsForGun[ WeaponRepository.GetHash(selected)]} матов", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"Event_WCraft Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.craft.createAmmo")]
        public static void Event_WCraftAmmo(ExtPlayer player, int frac, int category, int ammo)
        {
            try
            {		
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                if (FractionTypes[frac] != FractionsType.Mafia && 
                    FractionTypes[frac] != FractionsType.Gangs && 
                    FractionTypes[frac] != FractionsType.Nongov && 
                    FractionTypes[frac] != FractionsType.Bikers && 
                    !player.IsOrganizationMemberData()) return;

                if (FractionTypes[frac] == FractionsType.Nongov && memberFractionData.Rank <= 1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CraftMW2lvl), 3000);
                    return;
                }

                if (frac == 0 && !player.IsOrganizationAccess(RankToAccess.OrgCrime)) return;

                if (ammo == 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoInputAmmos), 3000);
                    return;
                }

                ItemStruct mItem = Chars.Repository.isItem(player, "inventory", ItemId.Material);
                int matsCount = (mItem == null) ? 0 : mItem.Item.Count;
                if (matsCount < MatsForAmmo[category] * ammo)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMats), 3000);
                    return;
                }
                if (Chars.Repository.isFreeSlots(player, AmmoTypes[category], ammo) != 0) return;
                Chars.Repository.RemoveIndex(player, mItem.Location, mItem.Index, MatsForAmmo[category] * ammo);
                Chars.Repository.AddNewItem(player, $"char_{player.GetUUID()}", "inventory", AmmoTypes[category], ammo);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно скрафтили {Chars.Repository.ItemsInfo[AmmoTypes[category]].Name} x{ammo}", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"Event_WCraftAmmo Exception: {e.ToString()}");
            }
        }
        private static Dictionary<ItemId, int> MatsForAmmoType = new Dictionary<ItemId, int>()
        {
            { ItemId.PistolAmmo, 1 },
            { ItemId.ShotgunsAmmo, 2 },
            { ItemId.SMGAmmo, 1 },
            { ItemId.RiflesAmmo, 2 },
            { ItemId.SniperAmmo, 4 },
        };
        private static int[] MatsForAmmo = new int[5]
        {
            1, // pistol
            2, // shotgun
            1, // smg
            2, // rifles
            4, // sniper
        };
        private static ItemId[] AmmoTypes = new ItemId[5]
        {
            ItemId.PistolAmmo,
            ItemId.ShotgunsAmmo,
            ItemId.SMGAmmo,
            ItemId.RiflesAmmo,
            ItemId.SniperAmmo,
        };
        #endregion

    }

    class Stocks : Script
    {
        public static int MaxCargobobMats = 100000;
        public static int CargobobMats = MaxCargobobMats;
        public static Dictionary<int, Vector3> stockCoords = new Dictionary<int, Vector3>()
        {
            { (int) Models.Fractions.FAMILY, new Vector3(-210.14122, -1574.3514, 34.34)},
            { (int) Models.Fractions.BALLAS, new Vector3(102.41281, -1964.3258, 20.08)},
            { (int) Models.Fractions.VAGOS, new Vector3(1445.3354, -1485.2415, 63.628956)},
            { (int) Models.Fractions.MARABUNTA, new Vector3(957.788, -2158.2217, 31.269325)},
            { (int) Models.Fractions.BLOOD, new Vector3(494.45898, -1532.6841, 24.872585)},
            { (int) Models.Fractions.CITY, new Vector3()},
            { (int) Models.Fractions.POLICE, new Vector3()},
            { (int) Models.Fractions.EMS, new Vector3()},
            { (int) Models.Fractions.FIB, new Vector3()},
            { (int) Models.Fractions.LCN, new Vector3(1402.818, 1154.9333, 114.3)},
            { (int) Models.Fractions.RUSSIAN, new Vector3(-99.02331, 1014.86597, 235.7)},
            { (int) Models.Fractions.YAKUZA, new Vector3(-1487.7345, -28.693373, 54.6)},
            { (int) Models.Fractions.ARMENIAN, new Vector3(-1793.937, 421.4769, 125.6)},
            { (int) Models.Fractions.ARMY, new Vector3()},
            { (int) Models.Fractions.LSNEWS, new Vector3()},
            { (int) Models.Fractions.THELOST, new Vector3(976.768738, -103.651985, 73.725174)},
            { (int) Models.Fractions.MERRYWEATHER, new Vector3(2039.182, 2934.093, -63.02208)},
            { (int) Models.Fractions.SHERIFF, new Vector3()},
        };
        public static Dictionary<int, Vector3> matsCoords = new Dictionary<int, Vector3>()
        {
            { (int) Models.Fractions.FAMILY, new Vector3(-186.42213, -1587.7325, 33.59)},
            { (int) Models.Fractions.BALLAS, new Vector3(103.208, -1954.8503, 19.16)},
            { (int) Models.Fractions.VAGOS, new Vector3(1401.759, -1514.474, 56.26676)},
            { (int) Models.Fractions.MARABUNTA, new Vector3(954.97314, -2176.3506, 29.271986)},
            { (int) Models.Fractions.BLOOD, new Vector3(504.1188, -1496.41, 27.28829)},
            { (int) Models.Fractions.CITY, new Vector3(-1329.0919, -565.11053, 30.281132 - 1.12)},
            { (int) Models.Fractions.POLICE, new Vector3(461.32843, -1015.1932, 28.080061 - 1.12)},
            { (int) Models.Fractions.EMS, new Vector3(342.3815, -563.184, 27.62377)},
            { (int) Models.Fractions.FIB, new Vector3(175.7835, -687.82684, 32.0046)},
            { (int) Models.Fractions.LCN, new Vector3(1413.687, 1118.036, 112.838)},
            { (int) Models.Fractions.RUSSIAN, new Vector3(-128.7453, 1006.892, 234.6121)},
            { (int) Models.Fractions.YAKUZA, new Vector3(-1460.2235, -53.754063, 54.45319 - 1.12)},
            { (int) Models.Fractions.ARMENIAN, new Vector3(-1714.7026, 483.68045, 127.67392)},
            { (int) Models.Fractions.ARMY, new Vector3(-530.29456, -2875.31, 5.1817595)},
            { (int) Models.Fractions.LSNEWS, new Vector3()},
            { (int) Models.Fractions.THELOST, new Vector3(977.1458, -132.7649, 72.84191)},
            { (int) Models.Fractions.MERRYWEATHER, new Vector3(-203.054, -576.0139, 33.46199)},
            { (int) Models.Fractions.SHERIFF, new Vector3(-463.26593, 6009.509, 30f)},
        };
        public static Dictionary<uint, int> maxMats = new Dictionary<uint, int>()
        {
            { (uint)VehicleHash.Barracks, 8000 },
            { (uint)VehicleHash.Brickade, 8000 },
            { (uint)VehicleHash.Cargobob, 10000 },
            { (uint)VehicleHash.Burrito3, 5000 },
            { (uint)VehicleHash.Youga, 5000 },
            { (uint)VehicleHash.Gburrito, 3000 },
            { (uint)VehicleHash.Terbyte, 6000 },
        };
        public static int GetMaxStock(int id)
        {
            int amount = 0;
            switch (id)
            {
                case (int) Models.Fractions.FAMILY:
                case (int) Models.Fractions.BALLAS:
                case (int) Models.Fractions.VAGOS:
                case (int) Models.Fractions.MARABUNTA:
                case (int) Models.Fractions.BLOOD:
                case (int) Models.Fractions.LCN:
                case (int) Models.Fractions.RUSSIAN:
                case (int) Models.Fractions.YAKUZA:
                case (int) Models.Fractions.ARMENIAN:
                case (int) Models.Fractions.THELOST:
                    amount = 180000;
                    break;
                case (int) Models.Fractions.MERRYWEATHER:
                    amount = 100000;
                    break;
                case (int) Models.Fractions.CITY:
                    amount = 100000;
                    break;
                case (int) Models.Fractions.POLICE:
                case (int) Models.Fractions.FIB:
                case (int) Models.Fractions.SHERIFF:
                    amount = 200000;
                    break;
                case (int) Models.Fractions.ARMY:
                    amount = 400000;
                    break;
                default:
                    amount = 50000;
                    break;
            }
            return amount;
        }


        public static void inputStocks(ExtPlayer player, int where, string action, int amount)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var fractionData = player.GetFractionData();
                var memberOrganizationData = player.GetOrganizationMemberData();
                if (fractionData == null || (memberOrganizationData != null && sessionData.IsOrgStockActive))
                {
                    if (where == 0) Organizations.Manager.inputStocks(player, action, amount);
                    return;
                }
                if (where == 0)
                {
                    switch (action)
                    {
                        case "put_stock":
                            string item = sessionData.SelectData.SelectedStock; //
                            int stockContains = 0;
                            int playerHave = 0;
                            if (item == "mats")
                            {
                                stockContains = fractionData.Materials;
                                int maxstock = GetMaxStock(fractionData.Id);
                                if (stockContains + amount > maxstock)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseTooMuch), 3000);
                                    return;
                                }
                                //ItemStruct mItem = Chars.Repository.isItem(player, "inventory", ItemId.Material);
                                //playerHave = (mItem == null) ? 0 : mItem.Item.Count;
                                playerHave = Chars.Repository.getCountToLacationItem($"char_{characterData.UUID}", "inventory", ItemId.Material);
                            }
                            else if (item == "drugs")
                            {
                                stockContains = fractionData.Drugs;
                                if (stockContains + amount > 10000)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NarkoWarehouseTooMuch), 3000);
                                    return;
                                }
                                //ItemStruct dItem = Chars.Repository.isItem(player, "inventory", ItemId.BodyArmor);
                                //playerHave = (dItem == null) ? 0 : dItem.Item.Count;
                                playerHave = Chars.Repository.getCountToLacationItem($"char_{characterData.UUID}", "inventory", ItemId.Drugs);
                            }
                            else if (item == "money")
                            {
                                stockContains = fractionData.Money;
                                playerHave = (int)characterData.Money;
                            }
                            else if (item == "medkits")
                            {
                                stockContains = fractionData.MedKits;
                                //ItemStruct ItemStruct = Chars.Repository.isItem(player, "inventory", ItemId.HealthKit);
                                //if (ItemStruct == null) playerHave = 0;
                                //else playerHave += ItemStruct.Item.Count;
                                playerHave = Chars.Repository.getCountToLacationItem($"char_{characterData.UUID}", "inventory", ItemId.HealthKit);
                            }

                            if (playerHave < amount)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoCoins), 3000);
                                return;
                            }

                            if (item == "mats")
                            {
                                fractionData.Materials += amount;
                                Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", ItemId.Material, amount);
                                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMats, LangFunc.GetText(LangType.Ru, DataName.TakenMats, amount));
                                Manager.sendFractionMessage(fractionData.Id, LangFunc.GetText(LangType.Ru, DataName.FGetMats, player.Name, player.Value, amount), true);
                            }
                            else if (item == "drugs")
                            {
                                fractionData.Drugs += amount;
                                Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", ItemId.Drugs, amount);
                                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeDrugs, LangFunc.GetText(LangType.Ru, DataName.TakenDrugs, amount));
                                Manager.sendFractionMessage(fractionData.Id, LangFunc.GetText(LangType.Ru, DataName.FGetDurgs, player.Name, player.Value, amount), true);
                            }
                            else if (item == "money")
                            {
                                fractionData.Money += amount;
                                MoneySystem.Wallet.Change(player, -amount);
                                GameLog.Money($"player({characterData.UUID})", $"frac({fractionData.Id})", amount, $"putStock");
                                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMoney, LangFunc.GetText(LangType.Ru, DataName.TakenMoney, MoneySystem.Wallet.Format(amount)));
                                Manager.sendFractionMessage(fractionData.Id, LangFunc.GetText(LangType.Ru, DataName.FGetMoney, player.Name, player.Value, MoneySystem.Wallet.Format(amount)), true);
                            }
                            else if (item == "medkits")
                            {
                                fractionData.MedKits += amount;
                                Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", ItemId.HealthKit, amount);
                                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMedkits, LangFunc.GetText(LangType.Ru, DataName.TakenHealthkits, amount));
                                Manager.sendFractionMessage(fractionData.Id, LangFunc.GetText(LangType.Ru, DataName.FGetHealthKits, player.Name, player.Value, amount), true);
                            }
                            GameLog.Stock(fractionData.Id, characterData.UUID, player.Name, item, amount, "in");
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseLeft, stockContains + amount, playerHave - amount), 3000);
                            fractionData.UpdateLabel();
                            break;
                        case "take_stock":
                            item = sessionData.SelectData.SelectedStock;
                            stockContains = 0;
                            playerHave = 0;
                            if (item == "mats")
                            {
                                if (!player.IsFractionAccess(RankToAccess.TakeMats)) return;
                                stockContains = fractionData.Materials;
                                playerHave = Chars.Repository.getCountToLacationItem($"char_{characterData.UUID}", "inventory", ItemId.Material);
                                if (playerHave + amount > Chars.Repository.ItemsInfo[ItemId.Material].Stack)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.InventoryFilled), 3000);
                                    return;
                                }
                            }
                            else if (item == "drugs")
                            {
                                if (!player.IsFractionAccess(RankToAccess.TakeDrugs)) return;
                                stockContains = fractionData.Drugs;
                                playerHave = Chars.Repository.getCountToLacationItem($"char_{characterData.UUID}", "inventory", ItemId.Drugs);
                                if (playerHave + amount > 50)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.InventoryFilled), 3000);
                                    return;
                                }
                            }
                            else if (item == "money")
                            {
                                if (!player.IsFractionAccess(RankToAccess.TakeMoney)) return;
                                stockContains = fractionData.Money;
                                playerHave = (int)characterData.Money;
                            }
                            else if (item == "medkits")
                            {
                                if (!player.IsFractionAccess(RankToAccess.TakeMedkits)) return;
                                stockContains = fractionData.MedKits;
                                playerHave = Chars.Repository.getCountToLacationItem($"char_{characterData.UUID}", "inventory", ItemId.HealthKit);
                            }

                            if (stockContains < amount)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseEmptyNet), 3000);
                                return;
                            }

                            if (item == "mats")
                            {
                                if (Chars.Repository.isFreeSlots(player, ItemId.Material, amount) != 0) return;
                                fractionData.Materials -= amount;
                                Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Material, amount);
                                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMats, LangFunc.GetText(LangType.Ru, DataName.TakeMats, amount));
                                Manager.sendFractionMessage(fractionData.Id, LangFunc.GetText(LangType.Ru, DataName.FTakenMats, player.Name, player.Value, amount), true);
                            }
                            else if (item == "drugs")
                            {
                                if (Chars.Repository.isFreeSlots(player, ItemId.Drugs, amount) != 0) return;
                                fractionData.Drugs -= amount;
                                Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Drugs, amount);
                                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeDrugs, LangFunc.GetText(LangType.Ru, DataName.TakeNarko, amount));
                                Manager.sendFractionMessage(fractionData.Id, LangFunc.GetText(LangType.Ru, DataName.FTakenDrugs, player.Name, player.Value, amount), true);
                            }
                            else if (item == "money")
                            {
                                fractionData.Money -= amount;
                                MoneySystem.Wallet.Change(player, amount);
                                GameLog.Money($"frac({fractionData.Id})", $"player({characterData.UUID})", amount, $"takeStock");
                                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMoney, LangFunc.GetText(LangType.Ru, DataName.TakeMoney, MoneySystem.Wallet.Format(amount)));
                                Manager.sendFractionMessage(fractionData.Id, LangFunc.GetText(LangType.Ru, DataName.FTakenMoney, player.Name, player.Value, MoneySystem.Wallet.Format(amount)), true);
                            }
                            else if (item == "medkits")
                            {
                                if (Chars.Repository.isFreeSlots(player, ItemId.HealthKit, amount) != 0) return;
                                fractionData.MedKits -= amount;
                                Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.HealthKit, amount);
                                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMedkits, LangFunc.GetText(LangType.Ru, DataName.TakeHealthkits, amount));
                                Manager.sendFractionMessage(fractionData.Id, LangFunc.GetText(LangType.Ru, DataName.FTakenHealthKits, player.Name, player.Value, amount), true);
                            }
                            GameLog.Stock(fractionData.Id, characterData.UUID, player.Name, item, amount, "out");
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseLeft, stockContains - amount, playerHave + amount), 3000);
                            fractionData.UpdateLabel();
                            break;
                    }
                }
                else
                {
                    if (!player.IsInVehicle)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustInCar), 3000);
                        return;
                    }
                    var vehicle = (ExtVehicle) player.Vehicle;
                    var vehicleLocalData = vehicle.GetVehicleLocalData();
                    if (vehicleLocalData != null)
                    {
                        if (!vehicleLocalData.CanMats && !vehicleLocalData.CanDrugs && !vehicleLocalData.CanMedKits)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarCantMoving), 3000);
                            return;
                        }
                        var fractionId = sessionData.OnFracStock;
                        var stockFractionData = Manager.GetFractionData(fractionId);
                        if (stockFractionData == null)
                            return;
                        
                        switch (action)
                        {
                            case "load_mats":
                                if (!vehicleLocalData.CanMats) return;
                                if (fractionId != (int) Models.Fractions.ARMY && fractionData.Id != fractionId)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ClubNotInFraction, Manager.GetName(sessionData.OnFracStock)), 3000);
                                    return;
                                }
                                if (fractionId != (int) Models.Fractions.ARMY && !player.IsFractionAccess(RankToAccess.TakeStock)) return;
                                if (vehicle.Model == (uint)VehicleHash.Cargobob)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ThisVehNoMatsHere), 3000);
                                    return;
                                }
                                if (stockFractionData.Materials < amount)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SkaldNoMats), 3000);
                                    return;
                                }
                                int maxMats = (Stocks.maxMats.ContainsKey(vehicle.Model)) ? Stocks.maxMats[vehicle.Model] : 600;
                                int curmats = Chars.Repository.getCountItem(VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), ItemId.Material);
                                if (curmats + amount > maxMats)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LimitOnlyMats, maxMats - curmats), 3000);
                                    return;
                                }
                                Chars.Repository.AddNewItem(null, VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), "vehicle", ItemId.Material, amount, MaxSlots: vMain.GetMaxSlots(vehicle.Model));
                                stockFractionData.Materials -= amount;
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouLoadedMat), 3000);
                                stockFractionData.UpdateLabel();
                                GameLog.Stock(fractionData.Id, characterData.UUID, player.Name, "mats", amount, "out");
                                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMedkits, LangFunc.GetText(LangType.Ru, DataName.ZagruzilMatsSoSklad, Manager.GetName(fractionId), amount));
                                return;
                            case "unload_mats":
                                int count = Chars.Repository.getCountToLacationItem(VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), "vehicle", ItemId.Material);
                                if (count < amount)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.InCarMats, count), 3000);
                                    return;
                                }
                                int maxstock = GetMaxStock(fractionData.Id);
                                if (stockFractionData.Materials + amount > maxstock)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseTooMuch), 3000);
                                    return;
                                }
                                if (vehicle.Model == (uint)VehicleHash.Cargobob && fractionId != (int) Models.Fractions.ARMY)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OnlyZankudo), 3000);
                                    return;
                                }
                                Chars.Repository.Remove(null, VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), "vehicle", ItemId.Material, amount);
                                stockFractionData.Materials += amount;
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouLoadedMats), 3000);
                                stockFractionData.UpdateLabel();
                                GameLog.Stock(fractionData.Id, characterData.UUID, player.Name, "mats", amount, "in");
                                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMedkits, LangFunc.GetText(LangType.Ru, DataName.LoadedMats, Manager.GetName(fractionId), amount));
                                player.AddTableScore(TableTaskId.Item16);
                                return;
                            case "load_drugs":
                                if (!vehicleLocalData.CanDrugs) return;
                                if (fractionData.Id != fractionId)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ClubNotInFraction, Manager.GetName(sessionData.OnFracStock)), 3000);
                                    return;
                                }
                                if (!player.IsFractionAccess(RankToAccess.TakeStock)) return;
                                if (stockFractionData.Drugs < amount)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ErrorNarkoSklad), 3000);
                                    return;
                                }
                                count = Chars.Repository.getCountItem(VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), ItemId.Drugs);
                                if (count >= (vMain.GetMaxSlots(vehicle.Model) * Chars.Repository.ItemsInfo[ItemId.Drugs].Stack))
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarMaxDrugs), 3000);
                                    return;
                                }
                                else if ((count + amount) > (vMain.GetMaxSlots(vehicle.Model) * Chars.Repository.ItemsInfo[ItemId.Drugs].Stack))
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.InCarSpace, (vMain.GetMaxSlots(vehicle.Model) * Chars.Repository.ItemsInfo[ItemId.Drugs].Stack) - count), 3000);
                                    return;
                                }
                                Chars.Repository.AddNewItem(null, VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), "vehicle", ItemId.Drugs, amount, MaxSlots: vMain.GetMaxSlots(vehicle.Model));
                                stockFractionData.Drugs -= amount;
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LoadDrugs), 3000);
                                stockFractionData.UpdateLabel();
                                GameLog.Stock(fractionData.Id, characterData.UUID, player.Name, "drugs", amount, "out");
                                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeDrugs, LangFunc.GetText(LangType.Ru, DataName.LloadedDrugs, Manager.GetName(fractionId), amount));
                                return;
                            case "unload_drugs":
                                count = Chars.Repository.getCountToLacationItem(VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), "vehicle", ItemId.Drugs);
                                if (count < amount)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehNoDrugs), 3000);
                                    return;
                                }
                                if (stockFractionData.Drugs + amount > 10000)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NarkoWarehouseTooMuch), 3000);
                                    return;
                                }
                                Chars.Repository.Remove(null, VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), "vehicle", ItemId.Drugs, amount);
                                stockFractionData.Drugs += amount;
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.UnloadedDrugs), 3000);
                                stockFractionData.UpdateLabel();
                                GameLog.Stock(fractionData.Id, characterData.UUID, player.Name, "drugs", amount, "in");
                                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeDrugs, LangFunc.GetText(LangType.Ru, DataName.LoadedDrugs, Manager.GetName(fractionId), amount));
                                player.AddTableScore(TableTaskId.Item33);
                                return;
                            case "load_medkits":
                                if (!vehicleLocalData.CanMedKits) return;
                                if (fractionData.Id != fractionId)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ClubNotInFraction, Manager.GetName(sessionData.OnFracStock)), 3000);
                                    return;
                                }
                                if (!sessionData.WorkData.OnDuty)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000);
                                    return;
                                }
                                if (stockFractionData.MedKits < amount)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ErrorAptekasSklad), 3000);
                                    return;
                                }
                                if ((Chars.Repository.getCountItem(VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), ItemId.HealthKit) + amount) > vMain.GetMaxSlots(vehicle.Model))
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ErrorMedAmount), 3000);
                                    return;
                                }
                                /*if (Chars.Repository.isFreeSlots(player, VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), "vehicle", ItemId.HealthKit, amount) != 0)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ErrorMedAmount), 3000);
                                    return;
                                }*/
                                Chars.Repository.AddNewItem(null, VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), "vehicle", ItemId.HealthKit, amount, MaxSlots: vMain.GetMaxSlots(vehicle.Model));
                                fractionData.MedKits -= amount;
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LoadAptekas), 3000);
                                fractionData.UpdateLabel();
                                GameLog.Stock(fractionData.Id, characterData.UUID, player.Name, "medkits", amount, "out");
                                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMedkits, LangFunc.GetText(LangType.Ru, DataName.LoadedAptekasFromSklad, Manager.GetName(fractionId), amount));
                                return;
                            case "unload_medkits":
                                if (!sessionData.WorkData.OnDuty)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000);
                                    return;
                                }
                                if (Chars.Repository.getCountToLacationItem(VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), "vehicle", ItemId.HealthKit) < amount)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehNoMedkits), 3000);
                                    return;
                                }
                                if (fractionId == 8)
                                {
                                    if (stockFractionData.MedKits + amount > 1500)
                                    {
                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SkladNoApteka), 3000);
                                        return;
                                    }
                                }
                                else
                                {
                                    if (stockFractionData.MedKits + amount > 1000)
                                    {
                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SkladNoApteka), 3000);
                                        return;
                                    }
                                }
                                Chars.Repository.Remove(null, VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), "vehicle", ItemId.HealthKit, amount);
                                stockFractionData.MedKits += amount;
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.UnloadedAptekas), 3000);
                                player.AddTableScore(TableTaskId.Item22);
                                stockFractionData.UpdateLabel();
                                GameLog.Stock(fractionData.Id, characterData.UUID, player.Name, "medkits", amount, "in");
                                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMedkits, LangFunc.GetText(LangType.Ru, DataName.LoadedAptekas, Manager.GetName(fractionId), amount));
                                return;
                            default:
                                // Not supposed to end up here. 
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Manager.Log.Write($"inputStocks Exception: {e.ToString()}");
            }
        }
        [Interaction(ColShapeEnums.FractionStock)]
        public static void OnFractionStock(ExtPlayer player, int fractionId, int list)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                sessionData.OnFracStock = fractionId;
                FractionData fractionData;
                switch (list)
                {
                    case 1:
                        if (!player.IsInVehicle)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustInCar), 3000);
                            return;
                        }
                        var vehicle = (ExtVehicle) player.Vehicle;
                        var vehicleLocalData = vehicle.GetVehicleLocalData();
                        if (vehicleLocalData != null)
                        {
                            if (!vehicleLocalData.CanMats && !vehicleLocalData.CanDrugs && !vehicleLocalData.CanMedKits)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarCantMoving), 3000);
                                return;
                            }
                            fractionData = Manager.GetFractionData(fractionId);
                            if (fractionData == null)
                                return;
                            if (fractionData.Id == (int) Models.Fractions.ARMY && (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 10) && characterData.AdminLVL <= 6)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SkladFullyClosed), 3000);
                                return;
                            }
                            if (!fractionData.IsOpenStock && fractionData.Id != (int) Models.Fractions.ARMY)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseClosed), 3000);
                                return;
                            }

                            OpenFracGarageMenu(player);
                        }
                        return;
                    case 2:
                        if (sessionData.OnFracStock == 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NotOnSkladPoint), 3000);
                            return;
                        }
                        
                        fractionData = player.GetFractionData();
                        if (fractionData == null)
                            return;
                        
                        if (fractionData.Id != sessionData.OnFracStock)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ClubNotInFraction, Manager.GetName(sessionData.OnFracStock)), 3000);
                            return;
                        }
                        if (!fractionData.IsOpenStock)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseClosed), 3000);
                            return;
                        }
                        OpenFracStockMenu(player);
                        return;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
            }
            catch (Exception e)
            {
                Manager.Log.Write($"interactPressed Exception: {e.ToString()}");
            }
        }

        public static async Task SaveFractions(ServerBD db)
        {
            try
            {
                foreach (var fractionId in Manager.Fractions.Keys.ToList())
                {
                    try
                    {
                        var fractionData = Manager.GetFractionData(fractionId);
                        if (fractionData == null)
                            continue;

                        await db.Fractions
                            .Where(f => f.Id == fractionId)
                            .Set(f => f.Drugs, fractionData.Drugs)
                            .Set(f => f.Money, fractionData.Money)
                            .Set(f => f.Mats, fractionData.Materials)
                            .Set(f => f.Medkits, fractionData.MedKits)
                            .Set(f => f.Coalore, fractionData.CoalOre)
                            .Set(f => f.Ironore, fractionData.IronOre)
                            .Set(f => f.Sulfurore, fractionData.SulfurOre)
                            .Set(f => f.Preciousore, fractionData.PreciousOre)
                            .Set(f => f.Lastserial, WeaponRepository.FractionsLastSerial[fractionId])
                            .Set(f => f.Isopen, Convert.ToSByte(fractionData.IsOpenStock))
                            .Set(f => f.Isopengunstock, Convert.ToSByte(fractionData.IsOpenGunStock))
                            .Set(f => f.Fuellimit, fractionData.FuelLimit)
                            .Set(f => f.Fuelleft, fractionData.FuelLeft)
                            .Set(f => f.Clothingsets,
                                FractionClothingSets.FractionSets.ContainsKey((Models.Fractions) fractionId)
                                    ? JsonConvert.SerializeObject(
                                        FractionClothingSets.FractionSets[(Models.Fractions) fractionId])
                                    : String.Empty)
                            .UpdateAsync();
                    }
                    catch (Exception e)
                    {
                        Manager.Log.Write($"saveStocksDic Foreach Exception: {e.ToString()}");
                    }
                }
                Manager.Log.Write("Stocks has been saved to DB", nLog.Type.Success);
            }
            catch (Exception e)
            {
                Manager.Log.Write($"saveStocksDic Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("openWeaponStock")]
        public static void Event_openWeaponsStock(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                var fracId = player.GetFractionId();
                var memberOrganizationData = player.GetOrganizationMemberData();
                if (fracId == (int) Models.Fractions.None || (memberOrganizationData != null && sessionData.IsOrgStockActive))
                {
                    Organizations.Manager.OpenWeaponStock(player);
                }
                else if (fracId > 0)
                {
                    if (sessionData.OnFracStock == 0 || fracId != sessionData.OnFracStock) return;
                    if (!player.IsFractionAccess(RankToAccess.OpenWeaponStock)) return;
                    Chars.Repository.LoadOtherItemsData(player, "Fraction", fracId.ToString(), 5, Chars.Repository.InventoryMaxSlots["Fraction"]);
                }
            }
            catch (Exception e)
            {
                Manager.Log.Write($"Event_openWeaponsStock Exception: {e.ToString()}");
            }
        }

        #region menus
        public static void OpenFracGarageMenu(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                var vehicle = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    Trigger.ClientEvent(player, "matsOpen", !vehicleLocalData.CanDrugs, vehicleLocalData.CanMedKits);

                    // временное решение разгрузки ресурсов для армии:

                    if (vehicleLocalData.Fraction == (int) Models.Fractions.ARMY)
                    {
                        int count = Chars.Repository.getCountItem(VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), ItemId.Coal) + Chars.Repository.getCountItem(VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), ItemId.Iron) + Chars.Repository.getCountItem(VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), ItemId.Gold) + Chars.Repository.getCountItem(VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), ItemId.Sulfur) + Chars.Repository.getCountItem(VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), ItemId.Emerald) + Chars.Repository.getCountItem(VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), ItemId.Ruby);
                        
                        if (count >= 1)
                        {
                            if (sessionData.OnFracStock != 14)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Разгрузить ресурсы можно только на склад армии.", 3000);
                                return;
                            }

                            string LoadInfo = $"";

                            for (int i = 0; i < 4; i++)
                            {
                                ItemId item_type = ItemId.Coal;

                                if (i == 1) item_type = ItemId.Iron;
                                else if (i == 2) item_type = ItemId.Sulfur;
                                else if (i == 3) item_type = ItemId.Emerald;

                                int current_count = Chars.Repository.getCountItem(VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), item_type);
                                if (current_count >= 1)
                                {
                                    Chars.Repository.Remove(null, VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), "vehicle", item_type, current_count);
                                    var fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.ARMY);
                                    if (fractionData != null)
                                    {
                                        if (i == 0)
                                        {
                                            fractionData.CoalOre += current_count;
                                            LoadInfo += $"{current_count} шт. угля, ";
                                        }
                                        else if (i == 1)
                                        {
                                            fractionData.IronOre += current_count;
                                            LoadInfo += $"{current_count} шт. железной руды, ";
                                        }
                                        else if (i == 2)
                                        {
                                            fractionData.SulfurOre += current_count;
                                            LoadInfo += $"{current_count} шт. серной руды, ";
                                        }
                                        else if (i == 3)
                                        {
                                            fractionData.PreciousOre += current_count;
                                            LoadInfo += $"{current_count} шт. изумруда, ";
                                        }

                                        fractionData.UpdateLabel();
                                    }
                                }
                            }

                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы успешно разгрузили ресурсы на склад.", 3000);

                            LoadInfo = LoadInfo.Remove(LoadInfo.Length - 2);
                            Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeOre, $"Разгрузил на склад армии {LoadInfo}.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Manager.Log.Write($"OpenFracGarageMenu Exception: {e.ToString()}");
            }
        }
        public static void fracgarage(ExtPlayer player, string eventName, string data)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
                
                int amount = 0;
                if (!int.TryParse(data, out amount))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                    return;
                }
                if (amount < 1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                    return;
                }
                if (!player.IsInVehicle)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustInCar), 3000);
                    return;
                }
                var vehicle = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if (!vehicleLocalData.CanMats && !vehicleLocalData.CanDrugs && !vehicleLocalData.CanMedKits)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarCantMoving), 3000);
                        return;
                    }
                    switch (eventName)
                    {
                        case "loadmats":
                            if (!vehicleLocalData.CanMats)
                            {
                                return;
                            }
                            if (sessionData.OnFracStock != (int) Models.Fractions.ARMY && player.GetFractionId() != sessionData.OnFracStock)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не состоите в {Manager.GetName(sessionData.OnFracStock)}", 3000);
                                return;
                            }
                            inputStocks(player, 1, "load_mats", amount);
                            return;
                        case "unloadmats":
                            inputStocks(player, 1, "unload_mats", amount);
                            return;
                        case "loaddrugs":
                            if (!vehicleLocalData.CanDrugs)
                            {
                                return;
                            }
                            if (player.GetFractionId() != sessionData.OnFracStock)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не состоите в {Manager.GetName(sessionData.OnFracStock)}", 3000);
                                return;
                            }
                            inputStocks(player, 1, "load_drugs", amount);
                            return;
                        case "unloaddrugs":
                            inputStocks(player, 1, "unload_drugs", amount);
                            return;
                        case "loadmedkits":
                            if (!vehicleLocalData.CanMedKits)
                            {
                                return;
                            }
                            inputStocks(player, 1, "load_medkits", amount);
                            return;
                        case "unloadmedkits":
                            inputStocks(player, 1, "unload_medkits", amount);
                            return;
                        default:
                            // Not supposed to end up here. 
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Manager.Log.Write($"fracgarage Exception: {e.ToString()}");
            }
        }

        public static void OpenFracStockMenu(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                var fractionData = player.GetFractionData();
                if (fractionData == null)
                    return;
                
                var counter = new List<string>
                {
                    fractionData.Money.ToString(),
                    fractionData.MedKits.ToString(),
                    fractionData.Drugs.ToString(),
                    fractionData.Materials.ToString(),
                    Chars.Repository.getCountToStockItems($"Fraction_{fractionData.Id}"),
                };
                
                string json = JsonConvert.SerializeObject(counter);
                Manager.Log.Debug(json);
                Trigger.ClientEvent(player, "openStock", json);
                sessionData.IsOrgStockActive = false;
            }
            catch (Exception e)
            {
                Manager.Log.Write($"OpenFracStockMenu Exception: {e.ToString()}");
            }
        }
        #endregion
    }

    class MatsWar : Script
    {
        public static bool isWar = false;
        public static int matsLeft = 36000;
        private static ExtMarker warMarker = null;
        private static Vector3 warPosition = new Vector3(-326.3168, -2701.9377, 0.9251);
        private static ExtBlip warblip;
        
        public static int MatWarDropPositionIndex = 0;
        public static List<Vector3> MatWarDropsPositions = new List<Vector3>()
        {
            new Vector3(-398.41837, -2485.873, 5.988623),
            new Vector3(-394.79373, -2601.213, 13.642556),
            new Vector3(-416.5059, -2705.3623, 6.000181),
            new Vector3(-432.12888, -2812.5203, 17.454176),
            new Vector3(-362.6888, -2765.538, 6.009338),
            new Vector3(-321.4789, -2587.2744, 6.000203),
            new Vector3(-294.9209, -2637.932, 6.0484986),
            new Vector3(-349.32922, -2667.5186, 19.395124),
            new Vector3(-442.72372, -2744.6716, 6.0000896),
            new Vector3(-522.3989, -2825.25, 5.992627)
        };
        public static string MatWarDropTimer = null;
        public static int MatWarDropHealth = 100;
        public static ExtPlayer MatWarDropHackPlayerInfo = null;

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                CustomColShape.CreateCylinderColShape(warPosition, 6, 2, 0, ColShapeEnums.War);

                warblip = (ExtBlip) NAPI.Blip.CreateBlip(556, warPosition, 1, 4, Main.StringToU16("National Guard"), 255, 0, true, 0, 0);
            }
            catch (Exception e)
            {
                Manager.Log.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }

        public static void startWar()
        {
            try
            {
                if (isWar) return;
                NAPI.Chat.SendChatMessageToAll("~o~[MATWAR] \"Война за материалы\" была начата!");
                SafeZones.ChangeDamageState((int) SafeZones.ZoneName.MatWarZone, false);
                matsLeft = 36000;
                warMarker = (ExtMarker) NAPI.Marker.CreateMarker(1, warPosition - new Vector3(0, 0, 5), new Vector3(), new Vector3(), 6f, new Color(155, 0, 0, 255));
                isWar = true;
                Manager.sendFractionMessage((int) Models.Fractions.FAMILY, LangFunc.GetText(LangType.Ru, DataName.WzmShipGoing));
                Manager.sendFractionMessage((int) Models.Fractions.BALLAS, LangFunc.GetText(LangType.Ru, DataName.WzmShipGoing));
                Manager.sendFractionMessage((int) Models.Fractions.VAGOS, LangFunc.GetText(LangType.Ru, DataName.WzmShipGoing));
                Manager.sendFractionMessage((int) Models.Fractions.MARABUNTA, LangFunc.GetText(LangType.Ru, DataName.WzmShipGoing));
                Manager.sendFractionMessage((int) Models.Fractions.BLOOD, LangFunc.GetText(LangType.Ru, DataName.WzmShipGoing));
                Manager.sendFractionMessage((int) Models.Fractions.LCN, LangFunc.GetText(LangType.Ru, DataName.WzmShipGoing));
                Manager.sendFractionMessage((int) Models.Fractions.RUSSIAN, LangFunc.GetText(LangType.Ru, DataName.WzmShipGoing));
                Manager.sendFractionMessage((int) Models.Fractions.YAKUZA, LangFunc.GetText(LangType.Ru, DataName.WzmShipGoing));
                Manager.sendFractionMessage((int) Models.Fractions.THELOST, LangFunc.GetText(LangType.Ru, DataName.WzmShipGoing));
                Manager.sendFractionMessage((int) Models.Fractions.MERRYWEATHER, LangFunc.GetText(LangType.Ru, DataName.WzmShipGoing));
                
                //
                
                Chars.Repository.RemoveAll("matwar_100");
                
                MatWarDropHealth = 100;
                MatWarDropPositionIndex = new Random().Next(0, MatWarDropsPositions.Count);
                
                Chars.Repository.AddNewItem(null, "matwar_100", "matwar", ItemId.BodyArmor, 30, $"100", true, 100);
                Chars.Repository.AddNewItem(null, "matwar_100", "matwar", ItemId.Revolver, 15, WeaponRepository.GetMatwarSerial(), true, 100);
                Chars.Repository.AddNewItem(null, "matwar_100", "matwar", ItemId.AssaultSMG, 10, WeaponRepository.GetMatwarSerial(), true, 100);
                Chars.Repository.AddNewItem(null, "matwar_100", "matwar", ItemId.HeavyShotgun, 3, WeaponRepository.GetMatwarSerial(), true, 100);
                Chars.Repository.AddNewItem(null, "matwar_100", "matwar", ItemId.MilitaryRifle, 5, WeaponRepository.GetMatwarSerial(), true, 100);
                Chars.Repository.AddNewItem(null, "matwar_100", "matwar", ItemId.SMGMk2, 10, WeaponRepository.GetMatwarSerial(), true, 100);
                Chars.Repository.AddNewItem(null, "matwar_100", "matwar", ItemId.CombatMG, 2, WeaponRepository.GetMatwarSerial(), true, 100);
                Chars.Repository.AddNewItem(null, "matwar_100", "matwar", ItemId.HeavySniper, 1, WeaponRepository.GetMatwarSerial(), true, 100);

                int item_chance = new Random().Next(1, 26);
                if (item_chance >= 20)
                {
                    Chars.Repository.AddNewItem(null, "matwar_100", "matwar", ItemId.NavyRevolver, 2, WeaponRepository.GetMatwarSerial(), true, 100);
                    Chars.Repository.AddNewItem(null, "matwar_100", "matwar", ItemId.MG, 2, WeaponRepository.GetMatwarSerial(), true, 100);
                }
                else if (item_chance > 10 && item_chance < 20)
                {
                    Chars.Repository.AddNewItem(null, "matwar_100", "matwar", ItemId.CombatMGMk2, 1, WeaponRepository.GetMatwarSerial(), true, 100);
                    Chars.Repository.AddNewItem(null, "matwar_100", "matwar", ItemId.MarksmanRifle, 3, WeaponRepository.GetMatwarSerial(), true, 100);
                }
                else if (item_chance > 1 && item_chance < 10) Chars.Repository.AddNewItem(null, "matwar_100", "matwar", ItemId.MarksmanRifleMk2, 1, WeaponRepository.GetMatwarSerial(), true, 100);
                else if (item_chance == 1) Chars.Repository.AddNewItem(null, "matwar_100", "matwar", ItemId.CarCoupon, 1, "Ferrari488");
                
                if (MatWarDropTimer != null) Timers.Stop(MatWarDropTimer);
                MatWarDropTimer = Timers.StartOnce("MatWarDropTimer", 120000, () => MatWarDropEvent(), true);//change to 120000
                
                //
                
                warblip.Color = 49;
                
            }
            catch (Exception e)
            {
                Manager.Log.Write($"startWar Exception: {e.ToString()}");
            }
        }

        public static void endWar()
        {
            try
            {
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        NAPI.Chat.SendChatMessageToAll("~o~[MATWAR] \"Война за материалы\" была закончена!");
                        SafeZones.ChangeDamageState((int) SafeZones.ZoneName.MatWarZone, true);
                        if (warMarker != null && warMarker.Exists) 
                            warMarker.Delete();
                        if (warblip != null && warblip.Exists) 
                            warblip.Color = 4;
                    }
                    catch (Exception e)
                    {
                        Manager.Log.Write($"endWar Task Exception: {e.ToString()}");
                    }
                });
                
                isWar = false;
                
                Manager.sendFractionMessage((int) Models.Fractions.FAMILY, LangFunc.GetText(LangType.Ru, DataName.WzmShipGone));
                Manager.sendFractionMessage((int) Models.Fractions.BALLAS, LangFunc.GetText(LangType.Ru, DataName.WzmShipGone));
                Manager.sendFractionMessage((int) Models.Fractions.VAGOS, LangFunc.GetText(LangType.Ru, DataName.WzmShipGone));
                Manager.sendFractionMessage((int) Models.Fractions.MARABUNTA, LangFunc.GetText(LangType.Ru, DataName.WzmShipGone));
                Manager.sendFractionMessage((int) Models.Fractions.BLOOD, LangFunc.GetText(LangType.Ru, DataName.WzmShipGone));
                Manager.sendFractionMessage((int) Models.Fractions.LCN, LangFunc.GetText(LangType.Ru, DataName.WzmShipGone));
                Manager.sendFractionMessage((int) Models.Fractions.RUSSIAN, LangFunc.GetText(LangType.Ru, DataName.WzmShipGone));
                Manager.sendFractionMessage((int) Models.Fractions.YAKUZA, LangFunc.GetText(LangType.Ru, DataName.WzmShipGone));
                Manager.sendFractionMessage((int) Models.Fractions.THELOST, LangFunc.GetText(LangType.Ru, DataName.WzmShipGone));
                Manager.sendFractionMessage((int) Models.Fractions.MERRYWEATHER, LangFunc.GetText(LangType.Ru, DataName.WzmShipGone));

                if (MatWarDropTimer != null) Timers.Stop(MatWarDropTimer);

                foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                {
                    try
                    {
                        if (!foreachPlayer.IsCharacterData()) return;
                        Trigger.ClientEvent(foreachPlayer, "client.matwar.fight.dell", 100);
                    }
                    catch (Exception e)
                    {
                        Manager.Log.Write($"endWar Players-foreach Exception: {e.ToString()}");
                    }
                }
            }
            catch (Exception e)
            {
                Manager.Log.Write($"endWar Exception: {e.ToString()}");
            }
        }
        
        public static void MatWarDropEvent()
        {
            try
            {
                foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                {
                    try
                    {
                        if (!foreachPlayer.IsCharacterData()) return;
                        Trigger.ClientEvent(foreachPlayer, "client.matwar.fight.create", 100, 0, MatWarDropsPositions[MatWarDropPositionIndex], 2);
                    }
                    catch (Exception e)
                    {
                        Manager.Log.Write($"MatWarDropEvent Players-foreach Exception: {e.ToString()}");
                    }
                }
            }
            catch (Exception e)
            {
                Manager.Log.Write($"MatWarDropEvent Exception: {e.ToString()}");
            }
        }
        
        [RemoteEvent("CheckMatWarDropLockStatus")]
        public static void CheckMatWarDropLockStatus(ExtPlayer player, int ShapeId)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                if (MatWarDropHealth > 0)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PressEtoLockPick), 3000);
                    Trigger.ClientEvent(player, "client.updateMatWarHackStatus", true, MatWarDropHealth);
                }
                else Trigger.ClientEvent(player, "client.updateMatWarHackStatus", false, 0);
            }
            catch (Exception e)
            {
                Manager.Log.Write($"CheckMatWarDropLockStatus Exception: {e.ToString()}");
            }
        }
        
        [RemoteEvent("server.matWar.fight.player.start.hack")]
        public static void StartedMatWarHack(ExtPlayer player, int ShapeId)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (!FunctionsAccess.IsWorking("StartedMatWarHack"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }

                if (characterData.AdminLVL >= 1 && characterData.AdminLVL <= 5)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RestrictedForAdmin), 3000);
                    return;
                }

                ItemStruct armylockpick = Chars.Repository.isItem(player, "inventory", ItemId.ArmyLockpick);
                int count = (armylockpick == null) ? 0 : armylockpick.Item.Count;

                if (count == 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoArmyLockpick), 3000);
                    return;
                }

                if (ShapeId == 100)
                {
                    if (MatWarDropHealth > 0)
                    {
                        if (MatWarDropHackPlayerInfo != null)
                        {
                            var targetSessionData = MatWarDropHackPlayerInfo.GetSessionData();
                            if (targetSessionData == null || !targetSessionData.IsAirDropHacking) MatWarDropHackPlayerInfo = null;
                            else if (MatWarDropsPositions[MatWarDropPositionIndex].DistanceTo(MatWarDropHackPlayerInfo.Position) > 3) MatWarDropHackPlayerInfo = null;
                            else
                            {
                                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomebodyHacking), 3000);
                                return;
                            }
                        }

                        Trigger.PlayAnimation(player, "mp_weapons_deal_sting", "crackhead_bag_loop", 39);
                        Trigger.ClientEvent(player, "matWarDrop_hackStatus", 1);

                        MatWarDropHackPlayerInfo = player;
                        sessionData.IsAirDropHacking = true;
                    }
                    else
                    {
                        Trigger.ClientEvent(player, "client.updateMatWarHackStatus", false, 0);
                        Chars.Repository.LoadOtherItemsData(player, "matwar", ShapeId.ToString(), 10);
                    }
                }
            }
            catch (Exception e)
            {
                Manager.Log.Write($"StartedMatWarHack Exception: {e.ToString()}");
            }
        }
        
        [RemoteEvent("MatWarDropChangeLockStatus")]
        public static void MatWarDropChangeLockStatus(ExtPlayer player, int ShapeId, int health)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                if (!sessionData.IsAirDropHacking) return;
                
                Trigger.StopAnimation(player);
                
                if (ShapeId == 100)
                {
                    MatWarDropHealth = health;
                    MatWarDropHackPlayerInfo = null;
                    sessionData.IsAirDropHacking = false;

                    if (MatWarDropHealth <= 0)
                    {
                        int hack_chance = new Random().Next(1, 101);

                        if (hack_chance <= 50)
                        {
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucZamok), 3000);
                            var position = MatWarDropsPositions[MatWarDropPositionIndex];
                            ParticleFx.PlayFXonPos(position, 500f, position.X, position.Y, position.Z, "scr_indep_fireworks", "scr_indep_firework_shotburst", 5000);
                            Trigger.ClientEventInRange(position, 2.5f, "client.updateMatWarHackStatus", false, 0);
                        }
                        else
                        {
                            MatWarDropHealth = 20;
                            Trigger.ClientEvent(player, "client.updateMatWarHackStatus", true, MatWarDropHealth);
                            Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FailZamok), 3000);
                        }

                        ItemStruct armylockpick = Chars.Repository.isItem(player, "inventory", ItemId.ArmyLockpick);
                        int count = (armylockpick == null) ? 0 : armylockpick.Item.Count;
                        if (count > 0) Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", ItemId.ArmyLockpick, 1);
                    }
                }
            }
            catch (Exception e)
            {
                Manager.Log.Write($"MatWarDropChangeLockStatus Exception: {e.ToString()}");
            }
        }
        
        [RemoteEvent("server.matwar.fight.open")]
        public static void OpenMatWarInventory(ExtPlayer player, int ShapeId)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                if (ShapeId == 100)
                {
                    if (MatWarDropHealth > 0)
                    {
                        if (MatWarDropHackPlayerInfo != null)
                        {
                            var targetSessionData = MatWarDropHackPlayerInfo.GetSessionData();
                            if (targetSessionData == null || !targetSessionData.IsAirDropHacking) MatWarDropHackPlayerInfo = null;
                            else if (MatWarDropsPositions[MatWarDropPositionIndex].DistanceTo(MatWarDropHackPlayerInfo.Position) > 3) MatWarDropHackPlayerInfo = null;
                            else
                            {
                                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomebodyHacking), 3000);
                                return;
                            }
                        }
                            
                        Trigger.ClientEvent(player, "client.updateMatWarHackStatus", true, MatWarDropHealth);
                    }
                    else
                    {
                        Trigger.ClientEvent(player, "client.updateMatWarHackStatus", false, 0);
                        Chars.Repository.LoadOtherItemsData(player, "matwar", ShapeId.ToString(), 10);
                    }
                }
            }
            catch (Exception e)
            {
                Manager.Log.Write($"OpenMatWarInventory Exception: {e.ToString()}");
            }
        }
        
        [Interaction(ColShapeEnums.War)]
        public static void OnWar(ExtPlayer player)
        {
            try
            {
                if (!isWar) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var fracId = player.GetFractionId();
                if (!((fracId >= (int) Models.Fractions.FAMILY && fracId <= (int) Models.Fractions.BLOOD) || (fracId >= (int) Models.Fractions.LCN && fracId <= (int) Models.Fractions.ARMENIAN) || (fracId >= (int) Models.Fractions.THELOST && fracId <= (int) Models.Fractions.MERRYWEATHER)))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantMatsWar), 3000);
                    return;
                }
                if (!player.IsInVehicle)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustInCar), 3000);
                    return;
                }
                var vehicle = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if (!vehicleLocalData.CanMats)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantMoveMatsVeh), 3000);
                        return;
                    }
                    if (sessionData.TimersData.LoadMatsTimer != null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LoadingMatsvehUzhe), 3000);
                        return;
                    }
                    if (Chars.Repository.getCountItem(VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), ItemId.Material) >= Stocks.maxMats[player.Vehicle.Model])
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarMaxMats), 3000);
                        return;
                    }
                    sessionData.TimersData.LoadMatsTimer = Timers.StartOnce(20000, () => Army.loadMaterialsTimer(player), true);
                    vehicleLocalData.LoaderMats = player;
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LoadingMatsStarted), 3000);
                    Trigger.ClientEvent(player, "showLoader", LangFunc.GetText(LangType.Ru, DataName.LoadingMats), 1);
                    sessionData.VehicleMats = vehicle;
                    sessionData.WhereLoad = "WAR";
                }
            }
            catch (Exception e)
            {
                Manager.Log.Write($"OnWar Exception: {e.ToString()}");
            }
        }
        [Interaction(ColShapeEnums.War, Out: true)]
        public static void OutWar(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (!player.IsInVehicle) return;                
                var vehicle = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    ExtPlayer target = vehicleLocalData.LoaderMats;
                    if (!target.IsCharacterData()) return;
                    var targetSessionData = target.GetSessionData();
                    if (targetSessionData.TimersData.LoadMatsTimer != null) Timers.Stop(targetSessionData.TimersData.LoadMatsTimer);
                    targetSessionData.TimersData.LoadMatsTimer = null;
                    vehicleLocalData.LoaderMats = null;
                    Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MatLoadingCancelByCheckpoint), 3000);
                }
            }
            catch (Exception e)
            {
                Manager.Log.Write($"OutWar Exception: {e.ToString()}");
            }
        }

    }
}
