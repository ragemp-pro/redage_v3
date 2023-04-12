using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Accounts;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using Redage.SDK;
using System;
using System.Collections.Generic;
using NeptuneEvo.Core;



namespace NeptuneEvo.Events
{
    class CustomDamage : Script
    {
        private static readonly nLog Log = new nLog("Functions.CommandsAccess");

        private static IReadOnlyDictionary<uint, float> WeaponDamage = new Dictionary<uint, float>()
        {
            { (uint)WeaponHash.Unarmed, 7f },
            /* Handguns */
            { (uint)WeaponHash.Knife, 18f },
            { (uint)WeaponHash.Nightstick, 12f },
            { (uint)WeaponHash.Hammer, 16f },
            { (uint)WeaponHash.Bat, 13f },
            { (uint)WeaponHash.Crowbar, 15f },
            { (uint)WeaponHash.Golfclub, 17f },
            { (uint)WeaponHash.Bottle, 18f },
            { (uint)WeaponHash.Dagger, 16f },
            { (uint)WeaponHash.Hatchet, 18f },
            { (uint)WeaponHash.Knuckle, 15f },
            { (uint)WeaponHash.Machete, 22f },
            { (uint)WeaponHash.Flashlight, 13f },
            { (uint)WeaponHash.Switchblade, 18f },
            { (uint)WeaponHash.Poolcue, 12f },
            { (uint)WeaponHash.Wrench, 17f },
            { (uint)WeaponHash.Battleaxe, 20f },
            { (uint)WeaponHash.Stone_hatchet, 19f },
            /* Pistols */
            { (uint)WeaponHash.Pistol, 9f },
            { (uint)WeaponHash.Combatpistol, 11f },
            { (uint)WeaponHash.Pistol50, 15f },
            { (uint)WeaponHash.Snspistol, 9f },
            { (uint)WeaponHash.Heavypistol, 12f },
            { (uint)WeaponHash.Vintagepistol, 12f },
            { (uint)WeaponHash.Marksmanpistol, 60f },
            { (uint)WeaponHash.Revolver, 50f },
            { (uint)WeaponHash.Appistol, 7f },
            { (uint)WeaponHash.Stungun, 1f },
            { (uint)WeaponHash.Flaregun, 4f },
            { (uint)WeaponHash.Doubleaction, 29f },
            { (uint)WeaponHash.Pistol_mk2, 11f },
            { (uint)WeaponHash.Snspistol_mk2, 11 },
            { (uint)WeaponHash.Revolver_mk2, 57f },
            { (uint)WeaponHash.Raypistol, 0f },
            { (uint)WeaponHash.CeramicPistol, 15f },
            { (uint)WeaponHash.NavyRevolver, 55f },
            /* SMG */
            { (uint)WeaponHash.Microsmg, 6f },
            { (uint)WeaponHash.Machinepistol, 7f },
            { (uint)WeaponHash.Smg, 7.5f },
            { (uint)WeaponHash.Assaultsmg, 8f },
            { (uint)WeaponHash.Combatpdw, 7f },
            { (uint)WeaponHash.Mg, 12f },
            { (uint)WeaponHash.Combatmg, 13.5f },
            { (uint)WeaponHash.Gusenberg, 9.5f },
            { (uint)WeaponHash.Minismg, 7f },
            { (uint)WeaponHash.Smg_mk2, 8f },
            { (uint)WeaponHash.Combatmg_mk2, 14.5f },
            { (uint)WeaponHash.Raycarbine, 0f },
            /* Rifles */
            { (uint)WeaponHash.Assaultrifle, 9f },
            { (uint)WeaponHash.Carbinerifle, 9f },
            { (uint)WeaponHash.Advancedrifle, 8.5f },
            { (uint)WeaponHash.Specialcarbine, 9.5f },
            { (uint)WeaponHash.Bullpuprifle, 8f },
            { (uint)WeaponHash.Compactrifle, 8f },
            { (uint)WeaponHash.Assaultrifle_mk2, 9.5f },
            { (uint)WeaponHash.Carbinerifle_mk2, 9.5f },
            { (uint)WeaponHash.Specialcarbine_mk2, 10f },
            { (uint)WeaponHash.Bullpuprifle_mk2, 9.5f },

            /* Sniper */
            { (uint)WeaponHash.Sniperrifle, 50f },
            { (uint)WeaponHash.Heavysniper, 70f },
            { (uint)WeaponHash.Marksmanrifle, 30f },
            { (uint)WeaponHash.Heavysniper_mk2, 91f },
            { (uint)WeaponHash.Marksmanrifle_mk2, 35f },
            { 0x9D1F17E6, 16.5f },
            /* Shotguns */
            { (uint)WeaponHash.Pumpshotgun, 22f },
            { (uint)WeaponHash.Sawnoffshotgun, 25f },
            { (uint)WeaponHash.Bullpupshotgun, 28f },
            { (uint)WeaponHash.Assaultshotgun, 13f },
            { (uint)WeaponHash.Musket, 60f },
            { (uint)WeaponHash.Heavyshotgun, 33f },
            { (uint)WeaponHash.Dbshotgun, 50f },
            { (uint)WeaponHash.Autoshotgun, 20f },
            { (uint)WeaponHash.Pumpshotgun_mk2, 26f },
            /* Heavy */
            { (uint)WeaponHash.Grenadelauncher, 0f },
            { (uint)WeaponHash.Rpg, 0f },
            { (uint)WeaponHash.Minigun, 0f },
            { (uint)WeaponHash.Firework, 0f },
            { (uint)WeaponHash.Railgun, 0f },
            { (uint)WeaponHash.Hominglauncher, 0f },
            { (uint)WeaponHash.Grenadelauncher_smoke, 0f },
            { (uint)WeaponHash.Compactlauncher, 0f },
            { (uint)WeaponHash.Rayminigun, 0f },
            /* Throwables & Misc */
            { (uint)WeaponHash.Grenade, 0f },
            { (uint)WeaponHash.Stickybomb, 0f },
            { (uint)WeaponHash.Proximine, 0f },
            { (uint)WeaponHash.Bzgas, 1 },
            { (uint)WeaponHash.Molotov, 0f },
            { (uint)WeaponHash.Fireextinguisher, 0f },
            { (uint)WeaponHash.Petrolcan, 0f },
            { (uint)WeaponHash.Flare, 1f },
            { (uint)WeaponHash.Ball, 0f },
            { (uint)WeaponHash.Snowball, 0f },
            { (uint)WeaponHash.Smokegrenade, 0f },
            { (uint)WeaponHash.Pipebomb, 0f },
            { (uint)WeaponHash.Parachute, 0f }
        };

        public static IReadOnlyDictionary<ItemId, int> WeaponsHP = new Dictionary<ItemId, int>()

        {

            /* Pistols */

            { ItemId.Pistol, 500 },
            { ItemId.CombatPistol, 500 },
            { ItemId.Pistol50, 500 },
            { ItemId.SNSPistol, 500 },
            { ItemId.HeavyPistol, 500 },
            { ItemId.VintagePistol, 500 },
            { ItemId.MarksmanPistol, 500 },
            { ItemId.Revolver, 500 },
            { ItemId.APPistol, 500 },
            { ItemId.StunGun, 500 },
            { ItemId.FlareGun, 500 },
            { ItemId.DoubleAction, 500 },
            { ItemId.PistolMk2, 600 },
            { ItemId.SNSPistolMk2, 600 },
            { ItemId.RevolverMk2, 600 },
            
            /* SMG */

            { ItemId.MicroSMG, 1500 },
            { ItemId.MachinePistol, 1500 },
            { ItemId.SMG, 1500 },
            { ItemId.AssaultSMG, 1500 },
            { ItemId.CombatPDW, 1500 },
            { ItemId.MG, 2000 },
            { ItemId.CombatMG, 2000 },
            { ItemId.Gusenberg, 1500 },
            { ItemId.MiniSMG, 1500 },
            { ItemId.SMGMk2, 1800 },
            { ItemId.CombatMGMk2, 2500 },
            /* Rifles */

            { ItemId.AssaultRifle, 3000 },
            { ItemId.CarbineRifle, 3000 },
            { ItemId.AdvancedRifle, 3000 },
            { ItemId.SpecialCarbine, 3000 },
            { ItemId.BullpupRifle, 3000 },
            { ItemId.CompactRifle, 3000 },
            { ItemId.AssaultRifleMk2, 4000 },
            { ItemId.CarbineRifleMk2, 4000 },
            { ItemId.SpecialCarbineMk2, 4000 },
            { ItemId.BullpupRifleMk2, 4000 },
            { ItemId.MilitaryRifle, 4000 },
            { ItemId.TacticalRifle, 3500 },
            { ItemId.HeavyRifle, 4000},
            { ItemId.CombatRifle, 4000},

            /* Sniper */

            { ItemId.SniperRifle, 500 },
            { ItemId.HeavySniper, 400 },
            { ItemId.MarksmanRifle, 500 },
            { ItemId.HeavySniperMk2, 350 },
            { ItemId.MarksmanRifleMk2, 500 },
            { ItemId.PrecisionRifle, 600},
            
            /* Shotguns */

            { ItemId.PumpShotgun, 400 },
            { ItemId.SawnOffShotgun, 400 },
            { ItemId.BullpupShotgun, 400 },
            { ItemId.AssaultShotgun, 400 },
            { ItemId.Musket, 400 },
            { ItemId.HeavyShotgun, 500 },
            { ItemId.DoubleBarrelShotgun, 400 },
            { ItemId.SweeperShotgun, 400 },
            { ItemId.PumpShotgunMk2, 500 },
            { ItemId.CombatShotgun, 600},

            /* NEW WEAPONS */

            { ItemId.RayPistol, 9999 },
            { ItemId.CeramicPistol, 9999 },
            { ItemId.NavyRevolver, 9999 },
            { ItemId.RayCarbine, 9999 },
            { ItemId.GrenadeLauncher, 9999 },
            { ItemId.RPG, 9999 },
            { ItemId.Minigun, 9999 },
            { ItemId.Firework, 9999 },
            { ItemId.Railgun, 9999 },
            { ItemId.HomingLauncher, 9999 },
            { ItemId.GrenadeLauncherSmoke, 9999 },
            { ItemId.CompactGrenadeLauncher, 9999 },
            { ItemId.Widowmaker, 9999 },

            //Custom
            {ItemId.Glock, 9999 }


        };

        private static List<uint> ExceptionToDist = new List<uint>()

        {
            /* Sniper */
            (uint)WeaponHash.Sniperrifle, 
            (uint)WeaponHash.Heavysniper,
            (uint)WeaponHash.Marksmanrifle,
            (uint)WeaponHash.Heavysniper_mk2,
            (uint)WeaponHash.Marksmanrifle_mk2
        };

        private static float[] BoneMultiplier = new float[]
        {
            0.90f, // 0 - таз
            0.90f, // 1 - нога
            0.85f, // 2 - голень
            0.60f, // 3 - ступня
            0.90f, // 4 - нога
            0.85f, // 5 - голень
            0.60f, // 6 - ступня
            0.95f, // 7 - живот-таз
            1f, // 8 - живот
            1f, // 9 - грудь
            1.15f, // 10 - грудь-горло
            0.95f, // 11 - правое плечо
            0.90f, // 12 - правое предплечье
            0.85f, // 13 - правая рука
            0.60f, // 14 - правая ладонь
            0.95f, // 15 - левое плечо
            0.90f, // 16 - левое предплечье
            0.85f, // 17 - левая рука
            0.60f, // 18 - левая ладонь
            1.20f, // 19 - горло
            1.25f, // 20 - голова
        };
        private void PlayerDamage(ExtPlayer player, ExtPlayer attackPlayer, uint weapon, int damage)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null)
                return;

            if (player.Armor > 0)
            {
                int oldArmor = player.Armor;
                if (damage > oldArmor)
                {
                    player.Armor = 0;
                    damage -= oldArmor;
                }
                else
                {
                    player.Armor -= damage;
                    damage = 0;
                }
            }

            if (damage > 0)
            {
                if (damage >= player.Health)
                {
                    sessionData.KilledData.Killed = attackPlayer;
                    sessionData.KilledData.Weapon = weapon;
                    sessionData.KilledData.Time = DateTime.Now.AddSeconds(3);
                    player.Health = 0;
                }
                else
                {
                    player.Health -= damage;
                }
            }
        }

        [RemoteEvent("server.damage.playerToPet")]
        public void server_weapon_damage_playerToPet(ExtPlayer player, ExtPed target, int damage)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null)
                return;
            
            var weapon = (uint)player.CurrentWeapon;

            if (!WeaponDamage.ContainsKey(weapon))
                return;
            
            if (WeaponDamage[weapon] <= 0f)
                return;
            
            if (target == null)
                return;

            PedSystem.Pet.Repository.AttackPlayerToPet(target, player);


            //if (!ExceptionToDist.Contains(Weapon))
            //    damage = Convert.ToInt32(damage * dist);

            PedSystem.Pet.Repository.Damage(target, -damage);
        }

        private static IReadOnlyDictionary<uint, float> PedDamage = new Dictionary<uint, float>()
        {
            { (uint)PedHash.Chop, 20f },//
            { (uint)PedHash.Husky, 20f },//
            { (uint)PedHash.Poodle, 10f },//
            { (uint)PedHash.Pug, 10f },//
            { (uint)PedHash.Rottweiler, 20f },//
            { (uint)PedHash.Shepherd, 20f },//
            { (uint)PedHash.Westy, 20f },//
        };

        [RemoteEvent("server.damage.petToPet")]
        public void server_weapon_damage_petToPet(ExtPlayer player, ExtPed target, ExtPed ped, int damage)
        {
            if (ped == null)
                return;

            var model = ped.Model;

            if (!PedDamage.ContainsKey(model))
                return;
            if (PedDamage[model] <= 0f)
                return;

            PedSystem.Pet.Repository.Damage(target, -damage);

        }
        [RemoteEvent("server.damage.petToPlayer")]
        public void server_weapon_damage_petToPlayer(ExtPlayer player, ExtPlayer target, ExtPed ped, int damage)
        {

            if (ped == null)
                return;
            var targetSessionData = target.GetSessionData();
            if (targetSessionData == null)
                return;
            else if (targetSessionData.IsSafeZone)
                return;

            uint model = ped.Model;

            if (!PedDamage.ContainsKey(model))
                return;

            PlayerDamage(target, null, model, damage);

        }
        [RemoteEvent("deletearmor")]
        public void ClientEvent_DeleteArmor(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                var ItemStruct = Chars.Repository.isItem(player, "accessories", ItemId.BodyArmor);
                if (ItemStruct == null) return;
                Chars.Repository.RemoveIndex(player, ItemStruct.Location, ItemStruct.Index, 1);
                //Trigger.ClientEvent(player, "client.isArmor", false);
                sessionData.ArmorHealth = -1;
                ClothesComponents.ClearClothes(player, 9, characterData.Gender);
                ClothesComponents.UpdateClothes(player);
            }
            catch (Exception e)
            {
                Log.Write($"ClientEvent_DeleteArmor Exception: {e.ToString()}");
            }
        }



        [RemoteEvent("server.weaponShot")]
        public void ServerWeaponShot(ExtPlayer player, int count)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                var Item = Chars.Repository.GetItemData(player, "fastSlots", sessionData.ActiveWeap.Index);

                if (Item.ItemId == ItemId.Debug) return;
                
                if (!Item.Data.Contains("_"))
                {
                    int newHP = WeaponsHP.ContainsKey(Item.ItemId) ? WeaponsHP[Item.ItemId] : 100;

                    Item.Data += $"_{newHP}";

                    Chars.Repository.SetItemData(player, "fastSlots", Item.Index, Item, true, isWeaponShot:true);
                }
                else if (Item.Data.Contains("_") && Convert.ToInt32(Item.Data.Split("_")[1]) >= 1)
                {
                    int currentHP = Convert.ToInt32(Item.Data.Split("_")[1]);

                    currentHP -= count;

                    if (currentHP <= 0)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Ваше оружие сломалось.", 3000);

                        Chars.Repository.Remove(player, $"char_{characterData.UUID}", "fastSlots", Item.ItemId, 1);

                        WeaponRepository.RemoveHands(player);
                    }
                    else
                    {
                        Item.Data = $"{Item.Data.Split("_")[0]}_{currentHP}";
                        Chars.Repository.SetItemData(player, "fastSlots", sessionData.ActiveWeap.Index, Item, true, isWeaponShot:true);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"ServerWeaponShot Exception: {e.ToString()}");
            }
        }

        /*[RemoteEvent("server.armour")]
        public void Server_ChangeArmourState(Player player, int armour)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                InventoryItemData Item = Repository.GetItemData(player, "accessories", 7);

                bool isDell = true;

                if (armour > 0 && Item == null)
                {
                    if (Functions.FunctionsAccess.IsWorking("server.armour")) Weapons.PlayerKickAntiCheat(player, 3, true);
                }
                else if (Item != null && int.TryParse(Item.Data, out int armdata))
                {
                    if (armour > armdata)
                    {
                        if (Functions.FunctionsAccess.IsWorking("server.armour")) Weapons.PlayerKickAntiCheat(player, 3, true);
                    }
                    else if (armour > 0)
                    {
                        isDell = false;
                    }
                }

                if (Item != null && isDell)
                {
                    player.Armor = 0;
                    Repository.RemoveIndex(player, "accessories", 7, 1);
                    ClothesComponents.SetSpecialClothes(player, 9, 0, 0);
                    ClothesComponents.UpdateClothes(player);
                }
                //player.Armor = armour;
                //Trigger.ClientEventInRange(player.Position, 25000.0f, "client.parachute.state", player, state);
            }
            catch (Exception e)
            {
                Log.Write($"ParachuteState Exception: {e.ToString()}");
            }
        }*/
        [RemoteEvent("Server_ChangeHealthState")]
        public void Server_ChangeHealthState(ExtPlayer player, int health)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                //player.Health = health;
                //Trigger.ClientEventInRange(player.Position, 25000.0f, "client.parachute.state", player, state);
            }
            catch (Exception e)
            {
                Log.Write($"ParachuteState Exception: {e.ToString()}");
            }
        }
    }
}
