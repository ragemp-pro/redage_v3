using System;
using System.Collections.Generic;
using System.Data;
using Database;
using GTANetworkAPI;
using LinqToDB;
using Localization;
using NeptuneEvo.Handles;
using Redage.SDK;
using MySqlConnector;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Chars;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Events;
using NeptuneEvo.Fractions.Models;

namespace NeptuneEvo.Core
{
    class WeaponRepository : Script
    {
        private static readonly nLog Log = new nLog("Core.Weapons");

        internal enum Hash : Int32
        {
            //Hands = 2725352035,
            /* Handguns */
            Knife = -1716189206,
            Nightstick = 1737195953,
            Hammer = 1317494643,
            Bat = -1786099057,
            Crowbar = -2067956739,
            GolfClub = 1141786504,
            Bottle = -102323637,
            Dagger = -1834847097,
            Hatchet = -102973651,
            KnuckleDuster = -656458692,
            Machete = -581044007,
            Flashlight = -1951375401,
            SwitchBlade = -538741184,
            PoolCue = -1810795771,
            Wrench = 419712736,
            BattleAxe = -853065399,
            StoneHatchet = 940833800,
            /* Pistols */
            Pistol = 453432689,
            CombatPistol = 1593441988,
            Pistol50 = -1716589765,
            SNSPistol = -1076751822,
            HeavyPistol = -771403250,
            VintagePistol = 137902532,
            MarksmanPistol = -598887786,
            Revolver = -1045183535,
            APPistol = 584646201,
            StunGun = 911657153,
            FlareGun = 1198879012,
            DoubleAction = -1746263880,
            PistolMk2 = -1075685676,
            SNSPistolMk2 = -2009644972,
            RevolverMk2 = -879347409,
            RayPistol = -1355376991, 
            CeramicPistol = 727643628,
            NavyRevolver = -1853920116,
            /* SMG */
            MicroSMG = 324215364,
            MachinePistol = -619010992,
            SMG = 736523883,
            AssaultSMG = -270015777,
            CombatPDW = 171789620,
            MG = -1660422300,
            CombatMG = 2144741730,
            Gusenberg = 1627465347,
            MiniSMG = -1121678507,
            SMGMk2 = 2024373456,
            CombatMGMk2 = -608341376,
            RayCarbine = 1198256469,
            /* Rifles */
            AssaultRifle = -1074790547,
            CarbineRifle = -2084633992,
            AdvancedRifle = -1357824103,
            SpecialCarbine = -1063057011,
            BullpupRifle = 2132975508,
            CompactRifle = 1649403952,
            AssaultRifleMk2 = 961495388,
            CarbineRifleMk2 = -86904375,
            SpecialCarbineMk2 = -1768145561,
            BullpupRifleMk2 = -2066285827,
            MilitaryRifle = -1658906650,
            TacticalRifle = -774507221,
            HeavyRifle = -947031628,
            PrecisionRifle = 1853742572,
            CombatShotgun = 94989220,
            CombatRifle = -621661739,

            /* Sniper */
            SniperRifle = 100416529,
            HeavySniper = 205991906,
            MarksmanRifle = -952879014,
            HeavySniperMk2 = 177293209,
            MarksmanRifleMk2 = 1785463520,
            /* Shotguns */
            PumpShotgun = 487013001,
            SawnOffShotgun = 2017895192,
            BullpupShotgun = -1654528753,
            AssaultShotgun = -494615257,
            Musket = -1466123874,
            HeavyShotgun = 984333226,
            DoubleBarrelShotgun = -275439685,
            SweeperShotgun = 317205821,
            PumpShotgunMk2 = 1432025498,
            /* Heavy */
            GrenadeLauncher = -1568386805,
            RPG = -1312131151,
            Minigun = 1119849093,
            Firework = 2138347493,
            Railgun = 1834241177,
            HomingLauncher = 1672152130,
            GrenadeLauncherSmoke = 1305664598,
            CompactGrenadeLauncher = 125959754,
            Widowmaker = -1238556825,
            /* Throwables & Misc */
            Grenade = -1813897027,
            StickyBomb = 741814745,
            ProximityMine = -1420407917,
            BZGas = -1600701090,
            Molotov = 615608432,
            FireExtinguisher = 101631238,
            PetrolCan = 883325847,
            Flare = 1233104067,
            Ball = 600439132,
            Snowball = 126349499,
            SmokeGrenade = -37975472,
            PipeBomb = -1169823560,
            Parachute = -72657034,


            //CUSTOM
            Glock = 651271362,
        }
        
        public static Hash GetHash(string name)
        {
            Log.Debug($"{name} {Convert.ToString((Hash)Enum.Parse(typeof(Hash), name))}");
            return (Hash)Enum.Parse(typeof(Hash), name);
        }

        public static IReadOnlyDictionary<ItemId, ItemId> WeaponsAmmoTypes = new Dictionary<ItemId, ItemId>()
        {
            { ItemId.Pistol, ItemId.PistolAmmo },
            { ItemId.CombatPistol, ItemId.PistolAmmo },
            { ItemId.Pistol50, ItemId.PistolAmmo },
            { ItemId.SNSPistol, ItemId.PistolAmmo },
            { ItemId.HeavyPistol, ItemId.PistolAmmo },
            { ItemId.VintagePistol, ItemId.PistolAmmo },
            { ItemId.MarksmanPistol, ItemId.PistolAmmo },
            { ItemId.Revolver, ItemId.PistolAmmo },
            { ItemId.APPistol, ItemId.PistolAmmo },
            { ItemId.FlareGun, ItemId.PistolAmmo },
            { ItemId.DoubleAction, ItemId.PistolAmmo },
            { ItemId.PistolMk2, ItemId.PistolAmmo },
            { ItemId.SNSPistolMk2, ItemId.PistolAmmo },
            { ItemId.RevolverMk2, ItemId.PistolAmmo },
            { ItemId.CeramicPistol, ItemId.PistolAmmo },
            { ItemId.NavyRevolver, ItemId.PistolAmmo }, 

            { ItemId.MicroSMG, ItemId.SMGAmmo },
            { ItemId.MachinePistol, ItemId.SMGAmmo },
            { ItemId.SMG, ItemId.SMGAmmo },
            { ItemId.AssaultSMG, ItemId.SMGAmmo },
            { ItemId.CombatPDW, ItemId.SMGAmmo },
            { ItemId.MG, ItemId.SMGAmmo },
            { ItemId.CombatMG, ItemId.SMGAmmo },
            { ItemId.Gusenberg, ItemId.SMGAmmo },
            { ItemId.MiniSMG, ItemId.SMGAmmo },
            { ItemId.SMGMk2, ItemId.SMGAmmo },
            { ItemId.CombatMGMk2, ItemId.SMGAmmo },
            { ItemId.RayCarbine, ItemId.SMGAmmo },

            { ItemId.AssaultRifle, ItemId.RiflesAmmo },
            { ItemId.CarbineRifle, ItemId.RiflesAmmo },
            { ItemId.AdvancedRifle, ItemId.RiflesAmmo },
            { ItemId.SpecialCarbine, ItemId.RiflesAmmo },
            { ItemId.BullpupRifle, ItemId.RiflesAmmo },
            { ItemId.CompactRifle, ItemId.RiflesAmmo },
            { ItemId.AssaultRifleMk2, ItemId.RiflesAmmo },
            { ItemId.CarbineRifleMk2, ItemId.RiflesAmmo },
            { ItemId.SpecialCarbineMk2, ItemId.RiflesAmmo },
            { ItemId.BullpupRifleMk2, ItemId.RiflesAmmo },
            { ItemId.MilitaryRifle, ItemId.RiflesAmmo },
            { ItemId.TacticalRifle, ItemId.RiflesAmmo },
            { ItemId.HeavyRifle, ItemId.RiflesAmmo },
            { ItemId.CombatRifle, ItemId.RiflesAmmo },

            { ItemId.SniperRifle, ItemId.SniperAmmo },
            { ItemId.HeavySniper, ItemId.SniperAmmo },
            { ItemId.MarksmanRifle, ItemId.SniperAmmo },
            { ItemId.HeavySniperMk2, ItemId.SniperAmmo },
            { ItemId.MarksmanRifleMk2, ItemId.SniperAmmo },
            { ItemId.PrecisionRifle, ItemId.SniperAmmo },

            { ItemId.GrenadeLauncher, ItemId.PistolAmmo },
            { ItemId.RPG, ItemId.PistolAmmo },
            { ItemId.Minigun, ItemId.PistolAmmo },
            { ItemId.Firework, ItemId.PistolAmmo },
            { ItemId.Railgun, ItemId.PistolAmmo },
            { ItemId.HomingLauncher, ItemId.PistolAmmo },
            { ItemId.GrenadeLauncherSmoke, ItemId.PistolAmmo },
            { ItemId.CompactGrenadeLauncher, ItemId.PistolAmmo },
            { ItemId.Widowmaker, ItemId.PistolAmmo },
            { ItemId.Glock, ItemId.PistolAmmo },

            { ItemId.PumpShotgun, ItemId.ShotgunsAmmo },
            { ItemId.SawnOffShotgun, ItemId.ShotgunsAmmo },
            { ItemId.BullpupShotgun, ItemId.ShotgunsAmmo },
            { ItemId.AssaultShotgun, ItemId.ShotgunsAmmo },
            { ItemId.Musket, ItemId.ShotgunsAmmo },
            { ItemId.HeavyShotgun, ItemId.ShotgunsAmmo },
            { ItemId.DoubleBarrelShotgun, ItemId.ShotgunsAmmo },
            { ItemId.SweeperShotgun, ItemId.ShotgunsAmmo },
            { ItemId.PumpShotgunMk2, ItemId.ShotgunsAmmo },
            { ItemId.CombatShotgun, ItemId.ShotgunsAmmo },
        };
        public static IReadOnlyDictionary<ItemId, int> WeaponsClipsMax = new Dictionary<ItemId, int>()
        {
            { ItemId.Pistol, 12 },
            { ItemId.CombatPistol, 12 },
            { ItemId.Pistol50, 9 },
            { ItemId.SNSPistol, 6 },
            { ItemId.HeavyPistol, 18 },
            { ItemId.VintagePistol, 7 },
            { ItemId.MarksmanPistol, 1 },
            { ItemId.Revolver, 6 },
            { ItemId.APPistol, 18 },
            { ItemId.StunGun, 0 },
            { ItemId.FlareGun, 1 },
            { ItemId.DoubleAction, 6 },
            { ItemId.PistolMk2, 12 }, 
            { ItemId.SNSPistolMk2, 6 }, 
            { ItemId.RevolverMk2, 6 },
            { ItemId.RayPistol, 0 },
            { ItemId.CeramicPistol, 12 },
            { ItemId.NavyRevolver, 6 },
            { ItemId.Glock, 12 },

            { ItemId.MicroSMG, 16 },
            { ItemId.MachinePistol, 12 },
            { ItemId.SMG, 30 },
            { ItemId.AssaultSMG, 30 },
            { ItemId.CombatPDW, 30 },
            { ItemId.MG, 54 },
            { ItemId.CombatMG, 100 },
            { ItemId.Gusenberg, 30 },
            { ItemId.MiniSMG, 20 },
            { ItemId.SMGMk2, 30 },
            { ItemId.CombatMGMk2, 100 },
            { ItemId.RayCarbine, 9999 },

            { ItemId.AssaultRifle, 30 },
            { ItemId.CarbineRifle, 30 },
            { ItemId.AdvancedRifle, 30 },
            { ItemId.SpecialCarbine, 30 },
            { ItemId.BullpupRifle, 30 },
            { ItemId.CompactRifle, 30 },
            { ItemId.AssaultRifleMk2, 30 },
            { ItemId.CarbineRifleMk2, 30 }, 
            { ItemId.SpecialCarbineMk2, 30 },
            { ItemId.BullpupRifleMk2, 30 },
            { ItemId.MilitaryRifle, 30 },
            { ItemId.TacticalRifle, 30},
            { ItemId.HeavyRifle, 30},
            { ItemId.CombatRifle, 30},
            
            { ItemId.SniperRifle, 10 },
            { ItemId.HeavySniper, 6 },
            { ItemId.MarksmanRifle, 8 },
            { ItemId.HeavySniperMk2, 6 },
            { ItemId.MarksmanRifleMk2, 8 },
            { ItemId.PrecisionRifle, 5},

            { ItemId.GrenadeLauncher, 10 },
            { ItemId.RPG, 1 },
            { ItemId.Minigun, 15000 },
            { ItemId.Firework, 1 },
            { ItemId.Railgun, 1 },
            { ItemId.HomingLauncher, 1 },
            { ItemId.GrenadeLauncherSmoke, 10 },
            { ItemId.CompactGrenadeLauncher, 1 },
            { ItemId.Widowmaker, 15000 },

            { ItemId.PumpShotgun, 8 },
            { ItemId.SawnOffShotgun, 8 },
            { ItemId.BullpupShotgun, 14 },
            { ItemId.AssaultShotgun, 8 },
            { ItemId.Musket, 1 },
            { ItemId.HeavyShotgun, 6 },
            { ItemId.DoubleBarrelShotgun, 2 },
            { ItemId.SweeperShotgun, 10 },
            { ItemId.PumpShotgunMk2, 8 }, // closed
            { ItemId.Snowball, 10 },
            { ItemId.CombatShotgun, 6},
        };

        public static Dictionary<int, int> FractionsLastSerial = new Dictionary<int, int>()
        {
            { 1, 0 },
            { 2, 0 },
            { 3, 0 },
            { 4, 0 },
            { 5, 0 },
            { 6, 0 },
            { 7, 0 },
            { 8, 0 },
            { 9, 0 },
            { 10, 0 },
            { 11, 0 },
            { 12, 0 },
            { 13, 0 },
            { 14, 0 },
        };
        public static Dictionary<int, int> BusinessesLastSerial = new Dictionary<int, int>();

        public static int AirdropLastSerial = 1;

        [ServerEvent(Event.ResourceStart)]
        public void Event_ResourceStart()
        {
            try
            {
                using MySqlCommand cmd = new  MySqlCommand()
                {
                    CommandText = "SELECT * FROM `weapons`"
                };
                using DataTable result = MySQL.QueryRead(cmd);
                if (result == null || result.Rows.Count == 0)
                {
                    Log.Write("Table 'weapons' returns null result", nLog.Type.Warn);
                    return;
                }
                foreach (DataRow Row in result.Rows) BusinessesLastSerial.Add(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["lastserial"]));
            }
            catch (Exception e)
            {
                Log.Write($"Event_ResourceStart Exception: {e.ToString()}");
            }
        }

        private static void CreateBusinessesLastSerial(int id)
        {
            if (!BusinessesLastSerial.ContainsKey(id))
            {
                BusinessesLastSerial.Add(id, 1);
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB"); //В отдельном потоке

                        db.Insert(new Weapons
                        {
                            Id = id,
                            Lastserial = 1,
                        });
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
            }
        }
        
        public static string GetSerial(bool isFraction, int id, bool organization = false)
        {
            try
            {
                if (isFraction)
                {
                    if (organization) return $"{3000 + id}xxxxx";
                    else
                    {
                        
                        var fractionType = Fractions.Manager.FractionTypes[id];
                        if (fractionType == FractionsType.Mafia || fractionType == FractionsType.Gangs || fractionType == FractionsType.Nongov || fractionType == FractionsType.Bikers) return $"{1000 + id}xxxxx";
                        else
                        {
                            int serial = 100000000 + id * 100000 + FractionsLastSerial[id];
                            FractionsLastSerial[id]++;
                            if (FractionsLastSerial[id] >= 99999) FractionsLastSerial[id] = 0;
                            return serial.ToString();
                        }
                    }
                }
                else
                {
                    CreateBusinessesLastSerial(id);
                    int serial = 200000000 + id * 100000 + BusinessesLastSerial[id];
                    BusinessesLastSerial[id]++;
                    if (BusinessesLastSerial[id] >= 99999) BusinessesLastSerial[id] = 0;
                    return serial.ToString();
                }
            }
            catch (Exception e)
            {
                Log.Write($"GetSerial Exception: {e.ToString()}");
                return "000000000";
            }
        }

        public static string GetAirdropSerial()
        {
            try
            {
                int serial = 100000 + AirdropLastSerial;
                AirdropLastSerial++;
                if (AirdropLastSerial >= 900000) AirdropLastSerial = 0;
                return $"AIR{serial}";
            }
            catch (Exception e)
            {
                Log.Write($"GetSerial Exception: {e.ToString()}");
                return "AIR000000";
            }
        }

        public static string GetMatwarSerial()
        {
            try
            {
                int serial = 100000 + AirdropLastSerial;
                AirdropLastSerial++;
                if (AirdropLastSerial >= 900000) AirdropLastSerial = 0;
                return $"MW{serial}";
            }
            catch (Exception e)
            {
                Log.Write($"GetSerial Exception: {e.ToString()}");
                return "MW000000";
            }
        }
        
        public static string GetHaliSerial()
        {
            try
            {
                int serial = 100000 + AirdropLastSerial;
                AirdropLastSerial++;
                if (AirdropLastSerial >= 900000) AirdropLastSerial = 0;
                return $"HELI{serial}";
            }
            catch (Exception e)
            {
                Log.Write($"GetSerial Exception: {e.ToString()}");
                return "HELI000000";
            }
        }

        public static int GiveWeapon(ExtPlayer player, ItemId type, string serial, int count = 1)
        {
            try
            {
                var characterData = player.GetCharacterData();
                int weaponHP = CustomDamage.WeaponsHP.ContainsKey(type) ? CustomDamage.WeaponsHP[type] : 100;
                if (characterData == null) return -1;
                else if (Chars.Repository.isFreeSlots(player, type) != 0) return -1;
                return Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", type, count, $"{serial}_{weaponHP}");
            }
            catch (Exception e)
            {
                Log.Write($"GiveWeapon Exception: {e.ToString()}");
                return -1;
            }
        }

        public static void SaveWeaponsDB()
        {
            try
            {
                foreach (KeyValuePair<int, int> dict in BusinessesLastSerial)
                {
                    try
                    {
                        using MySqlCommand cmd = new MySqlCommand
                        {
                            CommandText = "UPDATE `weapons` SET `lastserial`=@val0 WHERE `id`=@val1"
                        };
                        cmd.Parameters.AddWithValue("@val0", dict.Value);
                        cmd.Parameters.AddWithValue("@val1", dict.Key);
                        MySQL.Query(cmd);
                    }
                    catch (Exception e)
                    {
                        Log.Write($"SaveWeaponsDB Foreach Exception: {e.ToString()}");
                    }
                }
                Log.Write("Weapons has been saved to DB", nLog.Type.Success);
            }
            catch (Exception e)
            {
                Log.Write($"SaveWeaponsDB Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.weapon.remove")]
        public static void RemoteEvent_playerRemove(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                if (sessionData.ActiveWeap.Item == null || sessionData.ActiveWeap.Index == -1) return;
                InventoryItemData Item = Chars.Repository.GetItemData(player, "fastSlots", sessionData.ActiveWeap.Index);
                if (Item.ItemId == ItemId.Debug)
                {
                    sessionData.ActiveWeap = new ItemStruct("", -1, null);
                    return;
                }
                else if (Item.ItemId != ItemId.Ball && Item.ItemId != ItemId.Snowball)
                    return;
                //TakeWeapon(player);
                Chars.Repository.RemoveIndex(player, "fastSlots", sessionData.ActiveWeap.Index);

                sessionData.ActiveWeap = new ItemStruct("", -1, null);
            } 
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_playerRemove Exception: {e.ToString()}");
            }
}

        [RemoteEvent("server.weapon.reload")]
        public static void RemoteEvent_playerReload(ExtPlayer player, int ammoInClip)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (sessionData.ActiveWeap.Item == null || sessionData.ActiveWeap.Index == -1) return;
                InventoryItemData Item = Chars.Repository.GetItemData(player, "fastSlots", sessionData.ActiveWeap.Index);
                if (Item.ItemId == ItemId.Debug)
                {
                    sessionData.ActiveWeap = new ItemStruct("", -1, null);
                    return;
                }
                if (!WeaponsAmmoTypes.ContainsKey(Item.ItemId)) return;
                else if (ammoInClip >= WeaponsClipsMax[Item.ItemId]) return;
                ItemId wAmmoType = WeaponsAmmoTypes[Item.ItemId];
                ItemStruct wItemStruct = Chars.Repository.isItem(player, "inventory", wAmmoType);
                int ammoLefts = (wItemStruct == null) ? 0 : wItemStruct.Item.Count;
                if (ammoLefts == 0) return;
                int ammo = (ammoLefts < WeaponsClipsMax[Item.ItemId] - ammoInClip) ? ammoLefts : WeaponsClipsMax[Item.ItemId] - ammoInClip;
                Chars.Repository.RemoveIndex(player, wItemStruct.Location, wItemStruct.Index, ammo);
                Trigger.ClientEvent(player, "client.weapon.give", (int)GetHash(Item.ItemId.ToString()), ammo, true, Item.ItemId);
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_playerReload Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.weapon.ammoin")]
        public static void RemoveAmmo(ExtPlayer player, int hash, int ammoInClip)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (ammoInClip == 0) return;
                string wName = ((Hash)hash).ToString();
                ItemId ItemId = (ItemId)Enum.Parse(typeof(ItemId), wName);
                if (!WeaponsAmmoTypes.ContainsKey(ItemId)) return;
                ItemId aType = WeaponsAmmoTypes[ItemId];
                int allAmmoCount = Chars.Repository.isFreeSlots(player, aType, ammoInClip, false);
                if (allAmmoCount == -1) Chars.Repository.ItemsDrop(player, new InventoryItemData(0, aType, ammoInClip));
                else
                {
                    if (ammoInClip > WeaponsClipsMax[ItemId]) ammoInClip = WeaponsClipsMax[ItemId];
                    if (allAmmoCount == 0) Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", aType, ammoInClip, isInfo: false); //если влезают все патроны
                    else if (allAmmoCount > 0) Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", aType, allAmmoCount, isInfo: false);
                    if (allAmmoCount > 0)
                    {
                        allAmmoCount = ammoInClip - allAmmoCount;
                        if (allAmmoCount > WeaponsClipsMax[ItemId]) allAmmoCount = 1;
                        Chars.Repository.ItemsDrop(player, new InventoryItemData(0, aType, allAmmoCount));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_playerAmmoIn Exception: {e.ToString()}");
            }
        }
        public static void RemoveHands(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var ItemStruct = sessionData.ActiveWeap;
                if (ItemStruct.Item == null || ItemStruct.Index == -1) return;
                
                TakeWeapon(player);

                sessionData.ActiveWeap = new ItemStruct("", -1, null);
            }
            catch (Exception e)
            {
                Log.Write($"RemoveHands Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("TakeWeapon")]
        public static void TakeWeapon(ExtPlayer player) // Пока что думаем как реализовать instant переменную.
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                ItemStruct ItemStruct = sessionData.ActiveWeap;
                if (ItemStruct.Item == null || ItemStruct.Index == -1) return;
                ItemId Item = ItemStruct.Item.ItemId;
                if (Item == ItemId.Debug) return;

                ItemsInfo ItemInfo = Chars.Repository.ItemsInfo[Item];
                if (ItemInfo.functionType == newItemType.Weapons || ItemInfo.functionType == newItemType.MeleeWeapons)
                {
                    if (ItemInfo.functionType == newItemType.Weapons)
                        OnStartTimerWeaponUpdate(player, ItemStruct.Item.SqlId);
                    
                    Commands.RPChat("sme", player, $"убрал" + (characterData.Gender ? "" : "а") + $" {Chars.Repository.ItemsInfo[Item].Name}");
                    Trigger.ClientEvent(player, "client.weapon.take", true);
                    sessionData.LastActiveWeap = 0;
                    WeaponComponents.Remove(player);
                    //if (ammo != 0) RemoveAmmo(player, (int)Item, ammo);
                }
                else
                {
                    int isUseItem = Chars.Repository.ItemsHands(player, ItemStruct.Location, ItemStruct.Index, ItemStruct.Item);
                    if (isUseItem == 1) Commands.RPChat("sme", player, $"убрал" + (characterData.Gender ? "" : "а") + $" {Chars.Repository.ItemsInfo[Item].Name}");
                }
            }
            catch (Exception e)
            {
                Log.Write($"TakeWeapon Exception: {e.ToString()}");
            }
        }

        public static void OnClearTimerWeaponUpdate(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var lastWeapon = sessionData.LastActive;
            
            if (lastWeapon.ClearTime != String.Empty)
            {
                Timers.Stop(lastWeapon.ClearTime);
                lastWeapon.ClearTime = String.Empty;
            }
            lastWeapon.WeaponSqlId = 0;
        }
        private static void OnStartTimerWeaponUpdate(ExtPlayer player, int nextSqlId)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                //OnClearTimerWeaponUpdate(player);
                
                var lastWeapon = sessionData.LastActive;
                if (lastWeapon.WeaponSqlId != 0)
                    return;
                
                lastWeapon.WeaponSqlId = nextSqlId;
                lastWeapon.ClearTime = Timers.StartOnce(1000 * 5, () =>
                {
                    lastWeapon.WeaponSqlId = 0;
                    lastWeapon.ClearTime = String.Empty;
                });
            }
            catch (Exception e)
            {
                Log.Write($"OnStartTimerWeaponUpdate Exception: {e.ToString()}");
            }
        }
        
        public static void PlayerKickAntiCheat(ExtPlayer player, byte reason, bool type)
        {
            try
            {
                if (reason == 3) 
                    return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
                
                if (characterData.AdminLVL >= 6) 
                    return;
                
                int reas = 500 + reason;
                if (!type)
                {
                    Trigger.SendToAdmins(1, LangFunc.GetText(LangType.Ru, DataName.WasKickedAC, player.Name, player.Value, reas));
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.Center, LangFunc.GetText(LangType.Ru, DataName.YouWasKickedAc), 30000);
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.Center, LangFunc.GetText(LangType.Ru, DataName.AcIfUsure), 30000);
                    GameLog.Admin($"server", $"ACKick(0{reas})", $"{player.Name}");
                }
                else
                {
                    if (sessionData.AccBanned) return;
                    sessionData.AccBanned = true;
                    Trigger.SendToAdmins(1,"!{#DF5353}" + LangFunc.GetText(LangType.Ru, DataName.AutoACBan2, player.Name, reas));
                    //DateTime until = DateTime.Now.AddYears(100);
                   // Ban.Online(player, until, true, $"Cheats(0{reas})", "AntiCheat");
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.Center, LangFunc.GetText(LangType.Ru, DataName.YouBannedPerm), 30000);
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.Center, LangFunc.GetText(LangType.Ru, DataName.ReasonCheats), 30000);
                    GameLog.Admin($"server", $"ACBan(0{reas})", $"{player.Name}");
                }
                player.Kick();
            }
            catch (Exception e)
            {
                Log.Write($"PlayerKickAntiCheat Exception: {e.ToString()}");
                if(player != null) player.Kick();
            }
        }

        private static List<int> CheckWeapons = new List<int>()
        {
            -1045183535, // Revolver
            -1568386805, // GrenadeLauncher
            -1312131151, // RPG
            1119849093, // Minigun
            2138347493, // Firework
            1834241177, // Railgun
            1672152130, // HomingLauncher
            1305664598, // GrenadeLauncherSmoke
            125959754, // CompactGrenadeLauncher
            -1238556825, // Widowmaker
            -1813897027, // Grenade
            741814745, // StickyBomb
            -1420407917, // ProximityMine
            -1600701090, // BZGas
            615608432, // Molotov
            101631238, // FireExtinguisher
            -37975472, // SmokeGrenade
            -1169823560, // PipeBomb
            -1355376991, // RayPistol
        };

        [ServerEvent(Event.PlayerWeaponSwitch)]
        public void OnPlayerWeaponSwitch(ExtPlayer player, WeaponHash oldWeapon, WeaponHash newWeapon)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                if (!characterData.IsAlive || !CheckWeapons.Contains((int)newWeapon) || sessionData.InAirsoftLobby >= 0) return;
                if (characterData.LVL == 0)
                {
                    PlayerKickAntiCheat(player, 0, true);
                    return;
                }
                string wName = ((Hash)newWeapon).ToString();
                ItemId itemname = (ItemId)Enum.Parse(typeof(ItemId), wName);
                ItemStruct ItemStruct = Chars.Repository.isItem(player, "fastSlots", itemname);
                if (ItemStruct == null)
                {
                    PlayerKickAntiCheat(player, 1, false);
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"OnPlayerWeaponSwitch Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("changeweap")]
        public static void RemoteEvent_changeWeapon(ExtPlayer player, int key)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.DemorganTime >= 1 || sessionData.DeathData.InDeath || sessionData.AntiAnimDown) return;
                key--;
                InventoryItemData Item = Chars.Repository.GetItemData(player, "fastSlots", key);
                if (Item.ItemId == ItemId.Debug && key != 4) return;
                ItemsInfo ItemInfo = Chars.Repository.ItemsInfo[Item.ItemId];
                if (key == 4)
                {
                    InventoryItemData _hItem = Chars.Repository.GetItemData(player, "accessories", 1);
                    if (Item.ItemId != ItemId.Mask && _hItem.ItemId != ItemId.Mask) return;
                    if (Item.ItemId == ItemId.Mask) Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouPutOnMask), 3000);
                    else Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouPutOffMask), 3000);
                    Chars.Repository.SetItemData(player, "accessories", 1, Item, true);
                    Chars.Repository.SetItemData(player, "fastSlots", 4, _hItem, true);
                    return;
                }
                int isUseItem = 1;
                if (ItemInfo.functionType == newItemType.Weapons || ItemInfo.functionType == newItemType.MeleeWeapons)
                {
                    /*if (player.IsInVehicle)
                    {
                        if (player.VehicleSeat == (int)VehicleSeat.Driver)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantDoWeaponOnVeh), 3000);
                            return;
                        }
                        else if (Item.ItemId == ItemId.Revolver || Item.ItemId == ItemId.RevolverMk2 || Item.ItemId == ItemId.NavyRevolver)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantDoRevolverCar), 3000);
                            return;
                        }
                    }*/
                    if (characterData.LVL == 0)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantDoTimeWeapon), 3000);
                        return;
                    }
                    if (characterData.ArrestTime >= 1)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantDoKPZ), 3000);
                        return;
                    }
                    int index = sessionData.ActiveWeap.Index;
                    TakeWeapon(player);
                    sessionData.ActiveWeap = new ItemStruct("", -1, null);
                    if (index == key) return;
                    Hash wHash = GetHash(Item.ItemId.ToString());
                    if (WeaponsAmmoTypes.ContainsKey(Item.ItemId))
                    {
                        ItemId wAmmoType = WeaponsAmmoTypes[Item.ItemId];
                        ItemStruct wItemStruct = Chars.Repository.isItem(player, "inventory", wAmmoType);
                        int ammoLefts = (wItemStruct == null) ? 0 : wItemStruct.Item.Count;
                        int ammoin = (ammoLefts == 0 || ammoLefts < WeaponsClipsMax[Item.ItemId]) ? ammoLefts : WeaponsClipsMax[Item.ItemId];
                        if (ammoin != 0) Chars.Repository.RemoveIndex(player, wItemStruct.Location, wItemStruct.Index, ammoin);
                        Trigger.ClientEvent(player, "client.weapon.give", (int)wHash, ammoin, false, Item.ItemId);
                        WeaponComponents.Give(player, (uint)wHash, $"weapon_{Item.SqlId}", "weapon");
                    }
                    else
                    {
                        if (Item.ItemId == ItemId.Snowball) Trigger.ClientEvent(player, "client.weapon.give", (int)wHash, 10, false, Item.ItemId);
                        else Trigger.ClientEvent(player, "client.weapon.give", (int)wHash, 1, false, Item.ItemId);
                    }
                    BattlePass.Repository.UpdateReward(player, 99);
                    Commands.RPChat("sme", player, $"достал" + (characterData.Gender ? "" : "а") + $" {ItemInfo.Name}");
                    sessionData.LastActiveWeap = Item.SqlId;
                }
                else
                {
                    int index = sessionData.ActiveWeap.Index;
                    TakeWeapon(player);
                    sessionData.ActiveWeap = new ItemStruct("", -1, null);
                    if (index == key) return;
                    isUseItem = Chars.Repository.ItemsHands(player, "fastSlots", key, Item);
                    if (isUseItem == -1) Chars.Repository.ItemsUse(player, "fastSlots", key, false);
                    else
                    {
                        BattlePass.Repository.UpdateReward(player, 99);
                    }
                }
                if (isUseItem == 1)
                {
                    sessionData.ActiveWeap = new ItemStruct("", key, Item);
                    if (sessionData.HandsUp)
                    {
                        Trigger.StopAnimation(player);
                        sessionData.HandsUp = false;
                    }
                }
                else 
                    sessionData.ActiveWeap = new ItemStruct("", -1, null);
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_changeWeapon Exception: {e.ToString()}");
            }
        }
    }
}