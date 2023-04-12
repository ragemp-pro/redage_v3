using NeptuneEvo.Core;
using Redage.SDK;
using System;
using System.Linq;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using System.Collections.Generic;
using Localization;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Chars;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Organizations.Models;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.Quests;
using NeptuneEvo.Table.Models;
using NeptuneEvo.Table.Tasks.Models;
using NeptuneEvo.Table.Tasks.Player;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;

namespace NeptuneEvo.Fractions
{
    class FractionCommands : Script
    {
        private static readonly nLog Log = new nLog("Fractions.FractionCommands");

        [ServerEvent(Event.PlayerEnterVehicle)]
        public void onPlayerEnterVehicleHandler(ExtPlayer player, ExtVehicle vehicle, sbyte seatid)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (sessionData.CuffedData.Cuffed && player.VehicleSeat == (int)VehicleSeat.Driver)
                {
                    VehicleManager.WarpPlayerOutOfVehicle(player);
                    return;
                }
                if (sessionData.DeathData.InDeath)
                {
                    VehicleManager.WarpPlayerOutOfVehicle(player);
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantSitVehCritical), 3000);
                    return;
                }
                if (sessionData.Follower != null)
                {
                    VehicleManager.WarpPlayerOutOfVehicle(player);
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ReleaseBeforeSitveh), 3000);
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"onPlayerEnterVehicleHandler Exception: {e.ToString()}");
            }
        }
        public static void respawnFractionCars(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                			
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                    
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null)
                    return;
                
                if (fractionData.Id == 0 || memberFractionData.Rank < (fractionData.LeaderRank() - 1)) return;

                if (DateTime.Now < Main.NextCarRespawn[fractionData.Id] && characterData.AdminLVL < 6)
                {
                    long ticks = Main.NextCarRespawn[fractionData.Id].Ticks - DateTime.Now.Ticks;
                    if (ticks <= 0) return;
                    DateTime g = new DateTime(ticks);
                    if (g.Hour >= 1) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CanDoByTime1h, g.Hour, g.Minute, g.Second), 3000);
                    else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CanDoByTime,  g.Minute, g.Second), 3000);
                    return;
                }

                var vehiclesLocalData = RAGE.Entities.Vehicles.All.Cast<ExtVehicle>()
                    .Where(v => v.VehicleLocalData != null)
                    .Where(v => v.VehicleLocalData.Access == VehicleAccess.Fraction && v.VehicleLocalData.Fraction == fractionData.Id)
                    .ToList();

                foreach (var vehicle in vehiclesLocalData)
                {
                    var vehicleLocalData = vehicle.GetVehicleLocalData();
                    if (vehicleLocalData != null)
                    {
                        if (vehicleLocalData.Occupants.Count >= 1) continue;
                        Admin.RespawnFractionCar(vehicle);
                    }
                }
                GameLog.FracLog(fractionData.Id, characterData.UUID, -1, player.Name, "-1", "respawnCars");
                Main.NextCarRespawn[fractionData.Id] = DateTime.Now.AddMinutes(30);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RespawnFractionVehs), 3000);

                if (fractionData.IsLeader(memberFractionData.Rank))
                    Manager.sendFractionMessage(fractionData.Id, "!{#FF8C00}[F] " + LangFunc.GetText(LangType.Ru, DataName.WhoRespawnFractionVehs, player.Name, player.Value), true);
            }
            catch (Exception e)
            {
                Log.Write($"respawnFractionCars Exception: {e.ToString()}");
            }
        }
        public static void playerPressCuffBut(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var fracId = player.GetFractionId();
                if (sessionData.CuffedData.Cuffed)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsCuffed), 3000);
                    return;
                }
                ExtPlayer target = Main.GetNearestPlayer(player, 2);
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) return;
                if (Main.IHaveDemorgan(player, true) || Main.IHaveDemorgan(target)) return;
                if (player.IsInVehicle || target.IsInVehicle || sessionData.InAirsoftLobby >= 0) return;
                if (NAPI.Player.IsPlayerInAnyVehicle(player))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouInCar), 3000);
                    return;
                }
                if (NAPI.Player.IsPlayerInAnyVehicle(target))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerInCar), 3000);
                    return;
                }
                if (targetSessionData.IsCasinoGame != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantCuffWhenCasino, targetSessionData.IsCasinoGame), 3000);
                    return;
                }
                if (sessionData.DeathData.InDeath) return;
                if (sessionData.Follower != null || sessionData.Following != null || targetCharacterData.ArrestTime >= 1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantUseOnThisPlayer), 3000);
                    return;
                }
                if (targetSessionData.Following != null || targetSessionData.Follower != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantUseOnThisPlayerWhenCuffed), 3000);
                    return;
                }
                string cuffmesp = ""; // message for Player after cuff
                string cuffmest = ""; // message for Target after cuff
                string uncuffmesp = ""; // message for Player after uncuff
                string uncuffmest = ""; // message for Target after uncuff
                string cuffme = ""; // message /me after cuff
                string uncuffme = ""; // message /me after uncuff
                string sound = ""; // message /me after uncuff
                CuffedData cdata = targetSessionData.CuffedData;
                if (characterData.AdminLVL != 0 || Manager.FractionTypes[fracId] == FractionsType.Gov || Manager.FractionTypes[fracId] == FractionsType.Nongov) // for gov factions or admins
                {
                    if (!sessionData.WorkData.OnDuty && characterData.AdminLVL == 0)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000);
                        return;
                    }
                    if (cdata.CuffedByMafia)
                    {
                        uncuffmesp = LangFunc.GetText(LangType.Ru, DataName.YouUncuffPlayer, target.Value);
                        uncuffmest = LangFunc.GetText(LangType.Ru, DataName.SomeoneUncuffsYou, player.Value);
                        uncuffme = "развязал" + (characterData.Gender ? "" : "а") + " игрока {name}";
                    }
                    else
                    {
                        cuffmesp = LangFunc.GetText(LangType.Ru, DataName.NaruchnikiTiOdel, target.Value);
                        cuffmest = LangFunc.GetText(LangType.Ru, DataName.NaruchnikiNaVasOdel, player.Value);
                        cuffme = "надел" + (characterData.Gender ? "" : "а") + " наручники на игрока {name}"; //
                        uncuffmesp = LangFunc.GetText(LangType.Ru, DataName.ViSnyaliNaruchniki, target.Value);
                        uncuffmest = LangFunc.GetText(LangType.Ru, DataName.OnSnyalNaruchniki, player.Value);
                        uncuffme = "снял" + (characterData.Gender ? "" : "а") + " наручники с игрока {name}"; //
                        sound = "cuffs";
                    }
                }
                else // for mafia
                {
                    if (cdata.CuffedByCop)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoKeysFromCuffs), 3000);
                        return;
                    }
                    else if (!targetSessionData.HandsUp && !cdata.Cuffed)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerMustHandsUp), 3000);
                        return;
                    }
                    ItemStruct cuffs = Chars.Repository.isItem(player, "inventory", ItemId.Cuffs);
                    int count = (cuffs == null) ? 0 : cuffs.Item.Count;
                    if (!cdata.Cuffed && count == 0)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoCuffs), 3000);
                        return;
                    }
                    else if (!cdata.Cuffed) Chars.Repository.RemoveIndex(player, cuffs.Location, cuffs.Index, 1);
                    sound = "zips";
                    cuffmesp = LangFunc.GetText(LangType.Ru, DataName.YouCuffed, target.Value);
                    cuffmest = LangFunc.GetText(LangType.Ru, DataName.PlayerCuffsYou, player.Value);
                    cuffme = "связал" + (characterData.Gender ? "" : "а") + " игрока {name}"; //SomeoneCuffsPlayer
                    uncuffmesp = LangFunc.GetText(LangType.Ru, DataName.YouUncuffPlayer, target.Value);
                    uncuffmest = LangFunc.GetText(LangType.Ru, DataName.SomeoneUncuffsYou, player.Value);
                    uncuffme = "развязал" + (characterData.Gender ? "" : "а") + " игрока {name}"; //SomeoneUncuffsPlayer
                }
                if (!cdata.Cuffed)
                {
                    ItemId Bags = Chars.Repository.GetItemData(target, "accessories", 8).ItemId;
                    if (Bags == ItemId.BagWithMoney || Bags == ItemId.BagWithDrill) Chars.Repository.ItemsDropToIndex(target, "accessories", 8);

                    cdata.Cuffed = true;
                    Players.Phone.Call.Repository.OnPut(target);

                    targetSessionData.IsHicjacking = false;
                    Trigger.ClientEvent(target, "dial", "close");
                    Trigger.ClientEvent(target, "fullblockMove", false);
                    Trigger.ClientEvent(target, "freeze", false);

                    Animations.AnimationStop(target);
                    Main.OnAntiAnim(target);
                    Trigger.PlayAnimation(target, "mp_arresting", "idle", 49);
                    // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "arresting");
                    Attachments.AddAttachment(target, Attachments.AttachmentsName.Cuffs);

                    Trigger.ClientEvent(target, "CUFFED", true);
                    Sounds.PlayPlayer3d(player, sound, new SoundData());
                    if (fracId == (int) Models.Fractions.CITY || Configs.IsFractionPolic(fracId) || fracId == (int) Models.Fractions.ARMY || fracId == (int) Models.Fractions.MERRYWEATHER) cdata.CuffedByCop = true;
                    else cdata.CuffedByMafia = true;

                    Chars.Repository.ItemsClose(target, true);
                    Trigger.ClientEvent(target, "blockMove", true);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, cuffmesp, 3000);
                    Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, cuffmest, 3000);
                    Commands.RPChat("sme", player, cuffme, target);
                    return;
                }
                unCuffPlayer(target);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, uncuffmesp, 3000);
                Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, uncuffmest, 3000);
                cdata.CuffedByCop = false;
                cdata.CuffedByMafia = false;
                Commands.RPChat("sme", player, uncuffme, target);
            }
            catch (Exception e)
            {
                Log.Write($"playerPressCuffBut Exception: {e.ToString()}");
            }
        }
        
        public static void onPlayerDeathHandler(ExtPlayer player, ExtPlayer entityKiller, uint weapon)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (sessionData.CuffedData.Cuffed)
                {
                    unCuffPlayer(player);
                    sessionData.CuffedData.CuffedByCop = false;
                    sessionData.CuffedData.CuffedByMafia = false;
                }
                if (sessionData.Follower != null)
                {
                    ExtPlayer target = sessionData.Follower;
                    if (!target.IsCharacterData()) return;
                    unFollow(player, target);
                }
                if (sessionData.Following != null)
                {
                    ExtPlayer target = sessionData.Following;
                    if (!target.IsCharacterData()) return;
                    unFollow(target, player);
                }
                if (sessionData.HeadPocket)
                {
                    ClothesComponents.ClearAccessory(player, 1);
                    ClothesComponents.SetSpecialClothes(player, 1, 0, 0);
                    ClothesComponents.UpdateClothes(player);
                    Trigger.ClientEvent(player, "setPocketEnabled", false);
                    sessionData.HeadPocket = false;
                }
            }
            catch (Exception e)
            {
                Log.Write($"onPlayerDeathHandler Exception: {e.ToString()}");
            }
        }

        #region every fraction commands

        [Command("openstock")]
        public static void CMD_OpenFractionStock(ExtPlayer player)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.OpenStock)) return;
                var fractionData = player.GetFractionData();
                if (fractionData != null && fractionData.Id >= (int) Models.Fractions.FAMILY)
                {
                    if (!player.IsFractionAccess(RankToAccess.OpenStock)) return;
                    if (fractionData.IsOpenStock)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Склад уже открыт", 3000);
                        return;
                    }
                    GameLog.FracLog(fractionData.Id, player.GetUUID(), -1, player.Name, "-1", "openStock");
                    fractionData.IsOpenStock = true;
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы открыли склад фракции", 3000);
                    Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.OpenStock, "Открыл склад");
                    Manager.sendFractionMessage(fractionData.Id, "!{#ADFF2F}[F] " + $"{player.Name} ({player.Value}) открыл склад.", true);
                    Trigger.ClientEvent(player, "client.frac.main.isStock", true);
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_OpenFractionStock Exception: {e.ToString()}");
            }
        }
        
        [Command("fopenstock")]
        public static void CMD_fopenstock(ExtPlayer player)
        {
            try
            {
                if (!player.IsOrganizationAccess(RankToAccess.OpenStock)) return;

                var organizationData = player.GetOrganizationData();
                if (organizationData == null)
                    return;
                
                if (organizationData.IsOpenStock)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Склад уже открыт", 3000);
                    return;
                }
                organizationData.IsOpenStock = true;
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы открыли склад семьи", 3000);
                Organizations.Table.Logs.Repository.AddLogs(player, OrganizationLogsType.OpenStock, "Открыл склад");
   
            }
            catch (Exception e)
            {
                Log.Write($"CMD_fopenstock Exception: {e.ToString()}");
            }
        }

        [Command("closestock")]
        public static void CMD_CloseFractionStock(ExtPlayer player)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.OpenStock)) return;
   

                var fractionData = player.GetFractionData();
                if (fractionData == null)
                    return;
                if (!fractionData.IsOpenStock)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Склад уже закрыт", 3000);
                    return;
                }
                GameLog.FracLog(fractionData.Id, player.GetUUID(), -1, player.Name, "-1", "closeStock");
                fractionData.IsOpenStock = false;
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы закрыли склад фракции", 3000);
                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.CloseStock, "Закрыл склад");
                Manager.sendFractionMessage(fractionData.Id, "!{#ADFF2F}[F] " + $"{player.Name} ({player.Value}) закрыл склад.", true);
                Trigger.ClientEvent(player, "client.frac.main.isStock", false);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_CloseFractionStock Exception: {e.ToString()}");
            }
        }
        
        [Command("fclosestock")]
        public static void CMD_fclosestock(ExtPlayer player)
        {
            try
            {
                if (!player.IsOrganizationAccess(RankToAccess.OpenStock)) return;
                
                var organizationData = player.GetOrganizationData();
                if (organizationData == null)
                    return;
                
                if (!organizationData.IsOpenStock)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Склад уже закрыт", 3000);
                    return;
                }
                organizationData.IsOpenStock = false;
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы закрыли склад семьи", 3000);
                Organizations.Table.Logs.Repository.AddLogs(player, OrganizationLogsType.CloseStock, "Закрыл склад");
  
            }
            catch (Exception e)
            {
                Log.Write($"CMD_fclosestock Exception: {e.ToString()}");
            }
        }

        [Command("opengs")]
        public static void CMD_opengs(ExtPlayer player)
        {
            try
            {	
                var fractionData = player.GetFractionData();
                if (fractionData == null)
                    return;

                if (fractionData.Id == (int) Models.Fractions.CITY || fractionData.Id == (int) Models.Fractions.POLICE || fractionData.Id == (int) Models.Fractions.FIB || fractionData.Id == (int) Models.Fractions.ARMY || fractionData.Id == (int) Models.Fractions.SHERIFF)
                {
                    if (!player.IsFractionAccess(RankToAccess.OpenGunStock)) return;
                    if (fractionData.IsOpenGunStock)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Cклад №2 уже открыт", 3000);
                        return;
                    }
                    GameLog.FracLog(fractionData.Id, player.GetUUID(), -1, player.Name, "-1", "openGunStock");
                    fractionData.IsOpenGunStock = true;
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы открыли склад №2", 3000);
                    Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.OpenStock, "Открыл склад №2");
                    Manager.sendFractionMessage(fractionData.Id, "!{#ADFF2F}[F] " + $"{player.Name} ({player.Value}) открыл склад №2.", true);
                    Trigger.ClientEvent(player, "client.frac.main.isGunStock", true);
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_opengs Exception: {e.ToString()}");
            }
        }

        [Command("closegs")]
        public static void CMD_closegs(ExtPlayer player)
        {
            try
            {		
                var fractionData = player.GetFractionData();
                if (fractionData == null)
                    return;

                if (fractionData.Id == (int) Models.Fractions.CITY || fractionData.Id == (int) Models.Fractions.POLICE || fractionData.Id == (int) Models.Fractions.FIB || fractionData.Id == (int) Models.Fractions.ARMY || fractionData.Id == (int) Models.Fractions.SHERIFF)
                {
                    if (!player.IsFractionAccess(RankToAccess.OpenGunStock)) return;
                    if (!fractionData.IsOpenGunStock)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Склад №2 уже закрыт", 3000);
                        return;
                    }
                    GameLog.FracLog(fractionData.Id, player.GetUUID(), -1, player.Name, "-1", "closeStock");
                    fractionData.IsOpenGunStock = false;
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы закрыли склад №2", 3000);
                    Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.CloseStock, "Закрыл склад №2");
                    Manager.sendFractionMessage(fractionData.Id, "!{#ADFF2F}[F] " + $"{player.Name} ({player.Value}) закрыл склад №2.", true);
                    Trigger.ClientEvent(player, "client.frac.main.isGunStock", false);
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_closegs Exception: {e.ToString()}");
            }
        }

        public static void SetFracRankOffline(ExtPlayer player, int uuid, int rank)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.SetRank))
                    return;
            
                if (rank < 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нельзя установить отрицательный или нулевой ранг", 3000);
                    return;
                }

                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null) 
                    return;

                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null) 
                    return;
                
                var targetMemberFractionData = Manager.GetFractionMemberData(uuid, memberFractionData.Id);
                if (targetMemberFractionData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрока не найдено в Вашей семье", 3000);
                    return;
                }
                
                if (targetMemberFractionData.Rank >= memberFractionData.Rank)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете изменить ранг игроку, у которого ранг выше, чем ваш собственный..", 8000);
                    return;
                }
                
                if (rank >= memberFractionData.Rank)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете установить такой ранг", 3000);
                    return;
                }
                if (!fractionData.Ranks.ContainsKey(rank))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ранг не найден", 3000);
                    return;
                }


                if (targetMemberFractionData.Rank > rank) Table.Logs.Repository.AddLogs(player, FractionLogsType.SetRank, $"Понизил оффлайн {targetMemberFractionData.Name} ({memberFractionData.UUID}) в должности ({targetMemberFractionData.Rank} -> {rank})");
                else Table.Logs.Repository.AddLogs(player, FractionLogsType.SetRank, $"Повысил оффлайн {targetMemberFractionData.Name} ({memberFractionData.UUID}) в должности ({targetMemberFractionData.Rank} -> {rank})");

                Player.Repository.SetRank(fractionData.Id, uuid, rank);
                
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы изменили ранг игрока игрока {targetMemberFractionData.Name} на {fractionData.Ranks[rank].Name}", 6000);
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
                if (!player.IsFractionAccess(RankToAccess.SetRank))
                    return;

                if (newRank < 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нельзя установить отрицательный или нулевой ранг", 3000);
                    return;
                }
                
                if (player == target) return;
                
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null) 
                    return;

                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null) 
                    return;
                
                var targetMemberFractionData = target.GetFractionMemberData();
                if (targetMemberFractionData == null) 
                    return;
                
                if (memberFractionData.Id != targetMemberFractionData.Id) return;
                
                if (newRank >= memberFractionData.Rank)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantUpToRank), 3000);
                    return;
                }
                if (targetMemberFractionData.Rank >= memberFractionData.Rank)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете повысить этого игрока", 3000);
                    return;
                }
                if (!fractionData.Ranks.ContainsKey(newRank))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ранг не найден", 3000);
                    return;
                }

                if (targetMemberFractionData.Rank > newRank) Table.Logs.Repository.AddLogs(player, FractionLogsType.SetRank, $"Понизил {target.Name} ({targetMemberFractionData.UUID}) в должности ({targetMemberFractionData.Rank} -> {newRank})");
                else Table.Logs.Repository.AddLogs(player, FractionLogsType.SetRank, $"Повысил {target.Name} ({targetMemberFractionData.UUID}) в должности ({targetMemberFractionData.Rank} -> {newRank})");
                
                Player.Repository.SetRank(fractionData.Id, target.GetUUID(), newRank);
                    
                Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, $"Теперь вы {Manager.GetFractionRankName (memberFractionData.Id, newRank)} во фракции", 6000);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы изменили ранг игрока {target.Name} на {Manager.GetFractionRankName(memberFractionData.Id, newRank)}", 6000);
            }
            catch (Exception e)
            {
                Log.Write($"SetFracRank Exception: {e.ToString()}");
            }
        }

        public static void InviteToFraction(ExtPlayer sender, ExtPlayer target)
        {
            try
            {
                if (!sender.IsFractionAccess(RankToAccess.Invite))
                    return;
                
                var memberFractionData = sender.GetFractionMemberData();
                if (memberFractionData == null)
                {
                    Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не состоите во фракции", 3000);
                    return;
                }
                
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null)
                {
                    Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не состоите во фракции", 3000);
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
                
                //var targetOrganizationData = target.GetOrganizationData();
                if (target.IsFractionMemberData()/* || (targetOrganizationData != null && targetOrganizationData.CrimeOptions)*/)
                {
                    Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок уже состоит в организации", 3000);
                    return;
                }
                if (targetCharacterData.LVL < 1)
                {
                    Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, "Необходим как минимум 1 уровень для приглашения игрока во фракцию", 3000);
                    return;
                }
                if (targetCharacterData.Warns > 0)
                {
                    Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, "Невозможно принять этого игрока", 3000);
                    return;
                }
                if (Manager.FractionTypes[memberFractionData.Id] == FractionsType.Gov && !targetCharacterData.Licenses[7])
                {
                    Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, "У игрока нет мед.карты", 3000);
                    return;
                }
                targetSessionData.InviteData.Fraction = memberFractionData.Id;
                targetSessionData.InviteData.Sender = sender;
                Trigger.ClientEvent(target, "openDialog", "INVITED_FRAC", $"{sender.Name} пригласил Вас в {Manager.FractionNames[memberFractionData.Id]}");

                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы пригласили во фракцию {target.Name}", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"InviteToFraction Exception: {e.ToString()}");
            }
        }

        public static void UnInviteFromFraction(ExtPlayer sender, ExtPlayer target, bool mayor = false)
        {
            try
            {
                if (!sender.IsFractionAccess(RankToAccess.UnInvite)) return;
                
                var memberFractionData = sender.GetFractionMemberData();
                if (memberFractionData == null) 
                    return;
                
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null) 
                    return;
                
                var targetMemberFractionData = target.GetFractionMemberData();
                if (targetMemberFractionData == null)
                    return;
                
                if (memberFractionData.Id != targetMemberFractionData.Id) return;
                if (memberFractionData.Rank <= targetMemberFractionData.Rank)
                {
                    Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете выгнать этого игрока", 3000);
                    return;
                }
                
                NeptuneEvo.Fractions.Player.Repository.RemoveFractionMemberData(memberFractionData.Id, target.GetUUID());

                Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, $"Вас выгнали из семьи {fractionData.Name}", 3000);
                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы выгнали из семьи {target.Name}", 3000);
                Table.Logs.Repository.AddLogs(sender, FractionLogsType.UnInvite, $"Выгнал {target.Name} ({target.GetUUID()})");
            }
            catch (Exception e)
            {
                Log.Write($"UnInviteFromFraction Exception: {e.ToString()}");
            }
        }

        #endregion

        #region cops and cityhall commands
        public static void ticketToTarget(ExtPlayer player, ExtPlayer target, int sum, string reason)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.Ticket)) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;			
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;

                if (!sessionData.WorkData.OnDuty && Manager.FractionTypes[memberFractionData.Id] == FractionsType.Gov)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WorkDayNotStarted), 3000);
                    return;
                }
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) return;
                if (memberFractionData.Id == (int) Models.Fractions.CITY && !SafeZones.IsSafeZone(sessionData.InsideSafeZone, SafeZones.ZoneName.Court))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны находиться в здании суда", 3000);
                    return;
                }
                if (sum <= 0 || sum > Main.TicketLimit)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ограничение по штрафу {Main.TicketLimit}", 3000);
                    return;
                }
                if (reason.Length > 100)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Слишком большая причина", 3000);
                    return;
                }
                if (targetCharacterData.Money < sum && MoneySystem.Bank.GetBalance(targetCharacterData.Bank) < sum)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerNotEnoughMoney), 3000);
                    return;
                }
                TicketsData ticketsData = targetSessionData.TicketsData;
                ticketsData.Price = sum;
                ticketsData.Target = player;
                ticketsData.Reason = reason;
                Trigger.ClientEvent(target, "openDialog", "TICKET", $"{player.Name} выписал Вам штраф в размере {sum}$ за {reason}. Оплатить?");
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы выписали штраф для {target.Name} в размере {sum}$ за {reason}", 3000);
                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.Ticket, $"Выписал штраф {target.Name} ({targetCharacterData.UUID})");
            }
            catch (Exception e)
            {
                Log.Write($"ticketToTarget Exception: {e.ToString()}");
            }
        }
        public static void ticketConfirm(ExtPlayer target, bool confirm)
        {
            try
            {
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) return;
                TicketsData ticketsData = targetSessionData.TicketsData;
                ExtPlayer player = ticketsData.Target;
                int sum = ticketsData.Price;
                string reason = ticketsData.Reason;
                ticketsData = new TicketsData();
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (confirm)
                {
                    if (UpdateData.CanIChange(target, sum, true) != 255) return;
                    if (!MoneySystem.Wallet.Change(target, -sum) && !MoneySystem.Bank.Change(targetCharacterData.Bank, -sum, false))
                    {
                        Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMoney), 3000);
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerNotEnoughMoney), 3000);
                    }

                    var fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.CITY);
                    if (fractionData != null)
                        fractionData.Money += Convert.ToInt32(sum * 0.6);
                    
                    MoneySystem.Wallet.Change(player, Convert.ToInt32(sum * 0.4));
                    
                    EventSys.SendCoolMsg(target,"Штраф", "Успешная оплата", $"Вы оплатили штраф в размере {sum}$ за {reason}", "", 7000);
                    EventSys.SendCoolMsg(player,"Штраф", "Успешная оплата", $"{target.Name} оплатил штраф в размере {sum}$ за {reason}", "", 7000);
                    //Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы оплатили штраф в размере {sum}$ за {reason}", 3000);
                    //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"{target.Name} оплатил штраф в размере {sum}$ за {reason}", 3000);
                    Commands.RPChat("sme", player, " выписал штраф для {name}", target);
                    Manager.sendFractionMessage((int) Models.Fractions.POLICE, "!{#FF8C00}[F] " + $"{player.Name} оштрафовал {target.Name} на {sum}$. Причина: {reason}", true);
                    Manager.sendFractionMessage((int) Models.Fractions.SHERIFF, "!{#FF8C00}[F] " + $"{player.Name} оштрафовал {target.Name} на {sum}$ Причина: {reason}", true);
                    GameLog.Ticket(characterData.UUID, targetCharacterData.UUID, sum, reason, player.Name, target.Name);
                }
                else
                {
                    EventSys.SendCoolMsg(target,"Штраф", "Отказ!", $"Вы отказались платить штраф в размере {sum}$ за {reason}", "", 7000);
                    EventSys.SendCoolMsg(player,"Штраф", "Отказ!",  $"{target.Name} отказался платить штраф в размере {sum}$ за {reason}", "", 7000);
                    //Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы отказались платить штраф в размере {sum}$ за {reason}", 3000);
                   // Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"{target.Name} отказался платить штраф в размере {sum}$ за {reason}", 3000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"ticketConfirm Exception: {e.ToString()}");
            }
        }
        public static void arrestTarget(ExtPlayer player, ExtPlayer target)
        {
            try
            {      
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;			
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;

                if (!sessionData.WorkData.OnDuty && Manager.FractionTypes[memberFractionData.Id] == FractionsType.Gov)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WorkDayNotStarted), 3000);
                    return;
                }
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) return;
                if (!player.IsFractionAccess(RankToAccess.Arrest)) return;
                if (player == target)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Невозможно применить на себе", 3000);
                    return;
                }
                if (player.Position.DistanceTo(target.Position) > 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Человек слишком далеко", 3000);
                    return;
                }
                if (!sessionData.WorkData.OnDuty)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны начать рабочий день", 3000);
                    return;
                }
                if (sessionData.InArrestArea == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны быть возле камеры", 3000);
                    return;
                }
                if (targetCharacterData.ArrestTime != 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок уже в тюрьме", 3000);
                    return;
                }
                if (targetCharacterData.WantedLVL == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок не в розыске", 3000);
                    return;
                }
                if (!targetSessionData.CuffedData.Cuffed)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoCuffed), 3000);
                    return;
                }
                if (targetSessionData.Following != null) unFollow(targetSessionData.Following, target);
                unCuffPlayer(target);
                targetSessionData.CuffedData.CuffedByCop = false;
                targetSessionData.CuffedData.CuffedByMafia = false;

                Commands.RPChat("sme", player, " поместил {name} в КПЗ", target);
                Manager.sendFractionMessage((int) Models.Fractions.POLICE, "!{#FF8C00}[F] " + $"{player.Name} посадил в КПЗ {target.Name} ({targetCharacterData.WantedLVL.Reason})", true);
                Manager.sendFractionMessage((int) Models.Fractions.SHERIFF, "!{#FF8C00}[F] " + $"{player.Name} посадил в КПЗ {target.Name} ({targetCharacterData.WantedLVL.Reason})", true);
                Manager.sendFractionMessage((int) Models.Fractions.FIB, "!{#FF8C00}[F] " + $"{player.Name} посадил в КПЗ {target.Name} ({targetCharacterData.WantedLVL.Reason})", true);
                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.Arrest, $"Посадил в КПЗ {target.Name} ({targetCharacterData.UUID})");

                target.Eval($"mp.game.audio.playSoundFrontend(-1, \"Mission_Pass_Notify\", \"DLC_HEISTS_GENERAL_FRONTEND_SOUNDS\", true);");

                targetCharacterData.ArrestTime = (targetCharacterData.WantedLVL.Level >= 5) ? 3600 : (targetCharacterData.WantedLVL.Level * 10 * 60);
                //if (Functions.Other.IsPlayerToSquare(player, -3740.761f, -7140.5205f, 4145.6113f, 3638.1814f) || Manager.AllMembers[(int) Models.Fractions.SHERIFF].Count == 0) targetCharacterData.ArrestType = 0;
                //else targetCharacterData.ArrestType = 1;
                targetCharacterData.ArrestType = (sbyte)sessionData.InArrestArea;
                int minutes = Convert.ToInt32(targetCharacterData.ArrestTime / 60);
               // Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы посадили игрока ({target.Value}) на {minutes} минут", 3000);
                //Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) посадил Вас на {minutes} минут", 3000);
                EventSys.SendCoolMsg(target,"WASTED", $"Арест", $"Игрок ({player.Value}) посадил Вас на {minutes} минут", "", 15000);  
                EventSys.SendCoolMsg(player,"WADTED", $"Арест", $"Вы посадили игрока ({target.Value}) на {minutes} минут", "", 10000);  

                GameLog.Arrest(player.GetUUID(), targetCharacterData.UUID, targetCharacterData.WantedLVL.Reason, targetCharacterData.WantedLVL.Level, player.Name, target.Name);
                GameLog.FracLog(memberFractionData.Id, player.GetUUID(), targetCharacterData.UUID, player.Name, target.Name, $"arrest({targetCharacterData.WantedLVL.Reason})");
                arrestPlayer(target);
            }
            catch (Exception e)
            {
                Log.Write($"arrestTarget Exception: {e.ToString()}");
            }
        }

        public static void releasePlayerFromPrison(ExtPlayer player, ExtPlayer target)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.Rfp)) return;           
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;    
                if (!sessionData.WorkData.OnDuty && Manager.FractionTypes[memberFractionData.Id] == FractionsType.Gov)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WorkDayNotStarted), 3000);
                    return;
                }
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) return;
                if (!sessionData.WorkData.OnDuty)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000);
                    return;
                }
                if (player == target)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Невозможно применить на себе", 3000);
                    return;
                }
                if (player.Position.DistanceTo(target.Position) > 3)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                    return;
                }
                if (sessionData.InArrestArea == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны быть возле камеры", 3000);
                    return;
                }
                if (targetCharacterData.ArrestTime == 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок не в тюрьме", 3000);
                    return;
                }
                freePlayer(target, false);
                //targetCharacterData.ArrestTime = 0;
                //targetCharacterData.ArrestType = 0;
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы освободили игрока ({target.Value}) из тюрьмы", 3000);
                Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) освободил Вас из тюрьмы", 3000);
                Manager.sendFractionMessage((int) Models.Fractions.POLICE, "!{#FF8C00}[F] " + $"{player.Name.Replace('_', ' ')} выпустил из КПЗ {target.Name.Replace('_', ' ')}", true);
                Manager.sendFractionMessage((int) Models.Fractions.SHERIFF, "!{#FF8C00}[F] " + $"{player.Name.Replace('_', ' ')} выпустил из КПЗ {target.Name.Replace('_', ' ')}", true);
                Manager.sendFractionMessage((int) Models.Fractions.FIB, "!{#FF8C00}[F] " + $"{player.Name.Replace('_', ' ')} выпустил из КПЗ {target.Name.Replace('_', ' ')}", true);
                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.Rfp, $"Выпустил из КПЗ {target.Name} ({targetCharacterData.UUID})");
                Commands.RPChat("sme", player, " освободил {name} из КПЗ", target);
                GameLog.FracLog(memberFractionData.Id, player.GetUUID(), targetCharacterData.UUID, player.Name, target.Name, "rfp");
            }
            catch (Exception e)
            {
                Log.Write($"releasePlayerFromPrison Exception: {e.ToString()}");
            }
        }

        public static void arrestTimer(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.ArrestTime-- <= 0) freePlayer(player, false);
            }
            catch (Exception e)
            {
                Log.Write($"arrestTimer Exception: {e.ToString()}");
            }
        }

        public static void freePlayer(ExtPlayer player, bool demorgan)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (sessionData.TimersData.ArrestTimer != null)
                {
                    Timers.Stop(sessionData.TimersData.ArrestTimer);
                    sessionData.TimersData.ArrestTimer = null;
                    NAPI.Task.Run(() =>
                    {
                        try
                        {
                            var characterData = player.GetCharacterData();
                            if (characterData == null) return;
                            if (demorgan)
                            {
                                characterData.DemorganInfo = new DemorganInfo();
                                player.ResetSharedData("HideNick");
                                Trigger.ClientEvent(player, "client.demorgan", false);
                                player.SetDefaultSkin();
                                if (characterData.Unmute <= 0)
                                {
                                    characterData.VoiceMuted = false;
                                    player.SetSharedData("vmuted", false);
                                }
                                else Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Наказание было снято, но воспользоваться голосовым чатом Вы сможете после истечения срока Mute.", 10000);
                            }
                            else
                            {
                                SeatingArrangements.LandingEnd(player);
                            }
                            
                            if (characterData.ArrestType == 1) player.Position = Sheriff.FirstExitPrisonPosition;
                            else if (characterData.ArrestType == 2) player.Position = Sheriff.SecondExitPrisonPosition;
                            else player.Position = Police.ExitPrisonPosition;
                            
                            characterData.ArrestTime = 0;
                            characterData.ArrestType = 0;
                            characterData.DemorganTime = 0;
                            Police.setPlayerWantedLevel(player, null);
                            Trigger.Dimension(player);
                            Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Вы были освобождены из тюрьмы", 3000);
                        }
                        catch (Exception e)
                        {
                            Log.Write($"freePlayer Task Exception: {e.ToString()}");
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Log.Write($"freePlayer Exception: {e.ToString()}");
            }
        }

        public static void arrestPlayer(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                if (characterData.ArrestType == 1) player.Position = Sheriff.FirstPrisonPosition;
                else if (characterData.ArrestType == 2) player.Position = Sheriff.SecondPrisonPosition;
                else player.Position = Police.PrisonPosition;
                
                Police.setPlayerWantedLevel(player, null);
                sessionData.TimersData.ArrestTimer = Timers.Start(1000, () => arrestTimer(player));
                //Chars.Repository.RemoveAllWeapons(player, true, armour: true);
                Chars.Repository.RemoveAllIllegalStuff(player);
            }
            catch (Exception e)
            {
                Log.Write($"arrestPlayer Exception: {e.ToString()}");
            }
        }

        public static void unCuffPlayer(ExtPlayer player, bool withanim = true)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                sessionData.CuffedData.Cuffed = false;
                sessionData.HandsUp = false;
                if (withanim)
                {
                    Trigger.StopAnimation(player);
                    Main.OffAntiAnim(player);
                }
                Trigger.ClientEvent(player, "CUFFED", false);
                Attachments.RemoveAttachment(player, Attachments.AttachmentsName.Cuffs);
                Trigger.ClientEvent(player, "blockMove", false);
            }
            catch (Exception e)
            {
                Log.Write($"unCuffPlayer Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("playerPressFollowBut")]
        public void ClientEvent_playerPressFollow(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (!player.IsFractionAccess(RankToAccess.Follow, false)) return;
                if (Main.IHaveDemorgan(player, true)) return;
                if (sessionData.CuffedData.Cuffed == true || sessionData.DeathData.InDeath) return;
                ExtPlayer target;
                if (sessionData.Follower != null)
                {
                    target = sessionData.Follower;
                    if (!target.IsCharacterData()) return;
                    if (Main.IHaveDemorgan(target)) return;
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы отпустили игрока ({target.Value})", 3000);
                    Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) отпустил Вас", 3000);
                    unFollow(player, target);
                }
                else
                {
                    target = Main.GetNearestPlayer(player, 2);
                    if (!target.IsCharacterData()) return;
                    if (Main.IHaveDemorgan(target)) return;
                    targetFollowPlayer(player, target);
                }
            }
            catch (Exception e)
            {
                Log.Write($"ClientEvent_playerPressFollow Exception: {e.ToString()}");
            }
        }

        public static void unFollow(ExtPlayer player, ExtPlayer target)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;		
                
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;

                if (!sessionData.WorkData.OnDuty && Manager.FractionTypes[memberFractionData.Id] == FractionsType.Gov)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WorkDayNotStarted), 3000);
                    return;
                }

                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                sessionData.Follower = null;
                targetSessionData.Following = null;
                Trigger.ClientEvent(target, "setFollow", false);
            }
            catch (Exception e)
            {
                Log.Write($"unFollow Exception: {e.ToString()}");
            }
        }

        public static void targetFollowPlayer(ExtPlayer player, ExtPlayer target, bool force = false)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;

                if (!sessionData.WorkData.OnDuty && Manager.FractionTypes[memberFractionData.Id] == FractionsType.Gov)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WorkDayNotStarted), 3000);
                    return;
                }

                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                if (!target.IsCharacterData()) return;
                if (!force && !player.IsFractionAccess(RankToAccess.Follow)) return;
                int fracid = memberFractionData.Id;
                if (Manager.FractionTypes[fracid] == FractionsType.Gov) // for gov factions
                {
                    if (!sessionData.WorkData.OnDuty)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны сначала начать рабочий день", 3000);
                        return;
                    }
                }
                if (player.IsInVehicle || target.IsInVehicle) return;
                if (player == target)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Невозможно применить на себе", 3000);
                    return;
                }
                if (sessionData.Follower != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы уже тащите за собой игрока", 3000);
                    return;
                }
                if (player.Position.DistanceTo(target.Position) > 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                    return;
                }
                if (!targetSessionData.CuffedData.Cuffed)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoCuffed), 3000);
                    return;
                }
                if (targetSessionData.Following != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрока уже тащат", 3000);
                    return;
                }
                if (targetSessionData.DeathData.InDeath)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок мёртв", 3000);
                    return;
                }
                sessionData.Follower = target;
                targetSessionData.Following = player;
                Trigger.ClientEvent(target, "setFollow", true, player);
                Commands.RPChat("sme", player, $"повел" + (characterData.Gender ? "" : "а") + " {name} за собой", target);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы потащили за собой игрока ({target.Value})", 3000);
                Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) потащил Вас за собой", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"targetFollowPlayer Exception: {e.ToString()}");
            }
        }
        public static void targetUnFollowPlayer(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!player.IsFractionAccess(RankToAccess.Follow)) return;
                if (sessionData.Follower != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы никого не тащите за собой", 3000);
                    return;
                }
                ExtPlayer target = sessionData.Follower;
                if (!target.IsCharacterData()) return;
                unFollow(player, target);
                Commands.RPChat("sme", player, $"отпустил" + (characterData.Gender ? "" : "а") + " {name}", target);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы отпустили игрока ({target.Value})", 3000);
                Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) отпустил Вас", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"targetUnFollowPlayer Exception: {e.ToString()}");
            }
        }

        public static void carSuPlayer(ExtPlayer player, string number, string reason)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.Su)) return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;	
                
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;

                if (!sessionData.WorkData.OnDuty && Manager.FractionTypes[memberFractionData.Id] == FractionsType.Gov)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WorkDayNotStarted), 3000);
                    return;
                }
                
                var vehicleData = VehicleManager.GetVehicleToNumber(number);
                if (vehicleData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Личной машины с таким номером не существует", 3000);
                    return;
                }
                if (Police.WantedVehicles.ContainsKey(number))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Машина с этим номером уже в розыске, причина: {Police.WantedVehicles[number].Item1}", 3000);
                    return;
                }
                Police.WantedVehicles.Add(number, (reason, vehicleData.Model));
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы объявили транспортное средство [{number}] в розыск", 3000);
                foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                {
                    var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                    if (foreachMemberFractionData == null) 
                        continue;
                    
                    if (Configs.IsFractionPolic(foreachMemberFractionData.Id)) 
                        Trigger.ClientEvent(foreachPlayer, "setVehiclesWanted", number);
                }
                Manager.sendFractionMessage((int) Models.Fractions.POLICE, "!{#FF8C00}[F] " + $"{player.Name.Replace('_', ' ')} объявил транспортное средство [{number}] в розыск. Причина: {reason}", true);
                Manager.sendFractionMessage((int) Models.Fractions.SHERIFF, "!{#FF8C00}[F] " + $"{player.Name.Replace('_', ' ')} объявил транспортное средство [{number}] в розыск. Причина: {reason}", true);
                Manager.sendFractionMessage((int) Models.Fractions.FIB, "!{#FF8C00}[F] " + $"{player.Name.Replace('_', ' ')} объявил транспортное средство [{number}] в розыск. Причина: {reason}", true);
                GameLog.FracLog(memberFractionData.Id, player.GetUUID(), -1, player.Name, reason, $"carSu");
                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.Su, $"Объявил в розыск т/с {vehicleData.Model} ({number})");
            }
            catch (Exception e)
            {
                Log.Write($"carSuPlayer Exception: {e.ToString()}");
            }
        }

        public static void suPlayer(ExtPlayer player, int pasport, int stars, string reason)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.Su)) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
					
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                if (!sessionData.WorkData.OnDuty && Manager.FractionTypes[memberFractionData.Id] == FractionsType.Gov)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WorkDayNotStarted), 3000);
                    return;
                }
                if (!Main.PlayerNames.ContainsKey(pasport))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Паспорта с таким номером не существует", 3000);
                    return;
                }
                ExtPlayer target = (ExtPlayer) NAPI.Player.GetPlayerFromName(Main.PlayerNames[pasport]);
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Владелец паспорта должен быть в сети", 3000);
                    return;
                }
                if (player != target)
                {
                    if (!sessionData.WorkData.OnDuty)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000);
                        return;
                    }
                    if (memberFractionData.Id == (int) Models.Fractions.CITY && !SafeZones.IsSafeZone(sessionData.InsideSafeZone, SafeZones.ZoneName.Court))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны находиться в здании суда", 3000);
                        return;
                    }
                    if (targetCharacterData.ArrestTime != 0)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок в тюрьме", 3000);
                        return;
                    }
                    if (stars < 1 || stars > 6)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете выдать такое количество звёзд", 3000);
                        return;
                    }
                    if (targetCharacterData.WantedLVL == null || targetCharacterData.WantedLVL.Level + stars <= 6)
                    {
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы объявили игрока " + target.Name.Replace('_', ' ') + " в розыск", 3000);
                        Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, $"{player.Name.Replace('_', ' ')} объявил Вас в розыск. Причина: {reason}", 3000);
                        Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.Su, $"Объявил в розыск {target.Name} ({targetCharacterData.UUID})");
                        int oldStars = (targetCharacterData.WantedLVL == null) ? 0 : targetCharacterData.WantedLVL.Level;
                        WantedLevel wantedLevel = new WantedLevel(oldStars + stars, player.Name, DateTime.Now, reason);
                        Police.setPlayerWantedLevel(target, wantedLevel);
                        target.Eval($"mp.game.audio.playSoundFrontend(-1, \"LOOSE_MATCH\", \"HUD_MINI_GAME_SOUNDSET\", true);");
                        GameLog.FracLog(memberFractionData.Id, player.GetUUID(), targetCharacterData.UUID, player.Name, target.Name, $"su({wantedLevel.Level})");
                        
                        foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                        {
                            var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                            if (foreachMemberFractionData == null) 
                                continue;
                            
                            if (foreachMemberFractionData.Id == (int) Models.Fractions.POLICE || foreachMemberFractionData.Id == (int) Models.Fractions.FIB)
                                Trigger.SendChatMessage(foreachPlayer, "!{#FF8C00}[F] " + $"{player.Name} объявил в розыск {target.Name}. Причина: {reason}");
                        }
                        
                        return;
                    }
                    else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете выдать такое кол-во звёзд", 3000);
                }
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете объявить в розыск самого себя", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"suPlayer Exception: {e.ToString()}");
            }
        }

        public static void playerInCar(ExtPlayer player, ExtPlayer target)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.InCar, false) && !player.IsOrganizationAccess(RankToAccess.InCar)) return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;            
                if (!sessionData.WorkData.OnDuty && Manager.FractionTypes[player.GetFractionId()] == FractionsType.Gov)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WorkDayNotStarted), 3000);
                    return;
                }
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                if (!target.IsCharacterData()) return;
                if (Main.IHaveDemorgan(player)) return;
                if (player.IsInVehicle) return;
                if (player == target)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Невозможно использовать на себе", 3000);
                    return;
                }
                var vehicle = (ExtVehicle) VehicleManager.getNearestVehicle(player, 3);
                if (vehicle == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Рядом нет машин", 3000);
                    return;
                }
                if (player.Position.DistanceTo(target.Position) > 5)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                    return;
                }
                if (!targetSessionData.CuffedData.Cuffed)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок должен быть в наручниках", 3000);
                    return;
                }
                if (targetSessionData.Following != null && targetSessionData.Following != player)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Человека ведут за собой", 3000);
                    return;
                } 
                else unFollow(player, target);
                List<int> emptySlots = new List<int>
                {
                    (int)VehicleSeat.LeftRear,
                    (int)VehicleSeat.RightFront,
                    (int)VehicleSeat.Driver
                };
                foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                {
                    if (!foreachPlayer.IsCharacterData() || !foreachPlayer.IsInVehicle || foreachPlayer.Vehicle != vehicle) continue;
                    if (emptySlots.Contains(foreachPlayer.VehicleSeat)) emptySlots.Remove(foreachPlayer.VehicleSeat);
                }
                if (emptySlots.Count == 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInCar), 3000);
                    return;
                }
                //Trigger.ClientEvent(target, "setIntoVehicle", vehicle, emptySlots[0] - 1);
                target.SetIntoVehicle(vehicle, emptySlots[0]);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы запихали игрока ({target.Value}) в машину", 3000);
                Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) запихал Вас в машину", 3000);
                Commands.RPChat("sme", player, " открыл дверь и усадил {name} в машину", target);
            }
            catch (Exception e)
            {
                Log.Write($"playerInCar Exception: {e.ToString()}");
            }
        }

        public static void playerOutCar(ExtPlayer player, ExtPlayer target)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (!sessionData.WorkData.OnDuty && Manager.FractionTypes[player.GetFractionId()] == FractionsType.Gov)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WorkDayNotStarted), 3000);
                    return;
                }
                
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                if (!target.IsCharacterData()) return;
                if (sessionData.StreetRaceTime > DateTime.Now || targetSessionData.StreetRaceTime > DateTime.Now) return;
                if (Main.IHaveDemorgan(player)) return;
                if (player != target)
                {
                    if (!player.IsFractionAccess(RankToAccess.Pull, false) && !player.IsOrganizationAccess(RankToAccess.Pull)) return;
                    if (sessionData.CuffedData.Cuffed || sessionData.AttachToVehicle != null) return;
                    if (player.Position.DistanceTo(target.Position) <= 2f)
                    {
                        if (target.IsInVehicle)
                        {
                            if (player.IsInVehicle)
                            {
                                if (player.Vehicle == target.Vehicle && target.VehicleSeat == (int)VehicleSeat.Driver)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нельзя выкинуть водителя", 3000);
                                    return;
                                }
                                else if (player.Vehicle != target.Vehicle)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нельзя выкинуть человека, сидя в другой машине.", 3000);
                                    return;
                                }
                            }
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы выкинули игрока ({target.Value}) из машины", 3000);
                            Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) Выкинул Вас из машины", 3000);
                            VehicleManager.WarpPlayerOutOfVehicle(target);
                            Commands.RPChat("sme", player, " открыл дверь и вытащил {name} из машины", target);
                        }
                        else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок не в машине", 3000);
                    }
                    else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок слишком далеко от Вас", 3000);
                }
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете Выкинуть сами себя из машины", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"playerOutCar Exception: {e.ToString()}");
            }
        }

        public static void setWargPoliceMode(ExtPlayer player)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.Warg)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var fracId = player.GetFractionId();
                if (fracId == (int) Models.Fractions.ARMY)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Общая тревога включается через кнопку, которая находится в вышке Fort Zancudo", 5000);
                    return;
                }
                string message = "";
                if (fracId == (int) Models.Fractions.POLICE)
                {
                    Police.is_warg = !Police.is_warg;
                    if (Police.is_warg)
                    {
                        message = $"{player.Name} объявил режим ЧП!!!";
                        GameLog.FracLog(fracId, characterData.UUID, -1, player.Name, "-1", "enableWarg");
                    }
                    else
                    {
                        message = $"{player.Name} отключил режим ЧП.";
                        GameLog.FracLog(fracId, characterData.UUID, -1, player.Name, "-1", "disableWarg");
                    }
                    Manager.sendFractionMessage((int) Models.Fractions.POLICE, message);
                }
                else if (fracId == (int) Models.Fractions.SHERIFF)
                {
                    Sheriff.is_warg = !Sheriff.is_warg;
                    if (Sheriff.is_warg)
                    {
                        message = $"{player.Name} объявил режим ЧП!!!";
                        GameLog.FracLog(fracId, characterData.UUID, -1, player.Name, "-1", "enableWarg");
                    }
                    else
                    {
                        message = $"{player.Name} отключил режим ЧП.";
                        GameLog.FracLog(fracId, characterData.UUID, -1, player.Name, "-1", "disableWarg");
                    }
                    Manager.sendFractionMessage((int) Models.Fractions.SHERIFF, message);
                }
                else if (fracId == (int) Models.Fractions.FIB)
                {
                    Fbi.warg_mode = !Fbi.warg_mode;
                    if (Fbi.warg_mode)
                    {
                        message = $"{player.Name} объявил режим ЧП!!!";
                        GameLog.FracLog(fracId, characterData.UUID, -1, player.Name, "-1", "enableWarg");
                    }
                    else
                    {
                        message = $"{player.Name} отключил режим ЧП.";
                        GameLog.FracLog(fracId, characterData.UUID, -1, player.Name, "-1", "disableWarg");
                    }
                    Manager.sendFractionMessage((int) Models.Fractions.FIB, message);
                }
            }
            catch (Exception e)
            {
                Log.Write($"setWargPoliceMode Exception: {e.ToString()}");
            }
        }

        public static void takeGunLic(ExtPlayer player, ExtPlayer target)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) return;
                if (!player.IsFractionAccess(RankToAccess.TakeGunLic)) return;
                if (player.Position.DistanceTo(target.Position) > 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                    return;
                }

                var fracId = player.GetFractionId();
                if (fracId == (int) Models.Fractions.CITY)
                {
                    if (!SafeZones.IsSafeZone(sessionData.InsideSafeZone, SafeZones.ZoneName.Court))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны находиться в здании суда", 3000);
                        return;
                    }
                }
                else
                {
                    if ((!SafeZones.IsSafeZone(sessionData.InsideSafeZone, SafeZones.ZoneName.lspd) || !SafeZones.IsSafeZone(targetSessionData.InsideSafeZone, SafeZones.ZoneName.lspd)) && 
                        (!SafeZones.IsSafeZone(sessionData.InsideSafeZone, SafeZones.ZoneName.Sheriffs) || !SafeZones.IsSafeZone(targetSessionData.InsideSafeZone, SafeZones.ZoneName.Sheriffs)) && 
                        (!SafeZones.IsSafeZone(sessionData.InsideSafeZone, SafeZones.ZoneName.govfib) || !SafeZones.IsSafeZone(targetSessionData.InsideSafeZone, SafeZones.ZoneName.govfib)) && 
                        (!SafeZones.IsSafeZone(sessionData.InsideSafeZone, SafeZones.ZoneName.lesnik3) || !SafeZones.IsSafeZone(targetSessionData.InsideSafeZone, SafeZones.ZoneName.lesnik3)) && 
                        (!SafeZones.IsSafeZone(sessionData.InsideSafeZone, SafeZones.ZoneName.Sheriffs2) || !SafeZones.IsSafeZone(targetSessionData.InsideSafeZone, SafeZones.ZoneName.Sheriffs2)))
                    {
                        Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Изымать лицензию на оружие можно только в участке.", 3000);
                        return;
                    }
                }

                if (!targetCharacterData.Licenses[6])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока нет лицензии на оружие", 3000);
                    return;
                }

                GameLog.FracLog(fracId, characterData.UUID, targetCharacterData.UUID, player.Name, target.Name, "takeGunLic");
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы отобрали лицензию на оружие у игрока ({target.Value})", 3000);
                Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) отобрал у Вас лицензию на оружие", 3000);
                targetCharacterData.Licenses[6] = false;
                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeGunLic, $"Изъял лицензию на оружие у {target.Value} ({targetCharacterData.UUID})");
            }
            catch (Exception e)
            {
                Log.Write($"takeGunLic Exception: {e.ToString()}");
            }
        }

        public static void takeHelLic(ExtPlayer player, ExtPlayer target)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.TakeHelLic)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) return;
                if (player.Position.DistanceTo(target.Position) > 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                    return;
                }
                if (!targetCharacterData.Licenses[4])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока нет лицензии на вертолёт", 3000);
                    return;
                }
                GameLog.FracLog(player.GetFractionId(), characterData.UUID, targetCharacterData.UUID, player.Name, target.Name, "takeHelLic");
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы отобрали лицензию на вертолёт у игрока ({target.Value})", 3000);
                Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) отобрал у Вас лицензию на вертолёт", 3000);
                targetCharacterData.Licenses[4] = false;
                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeHelLic, $"Изъял лицензию на вертолёт у {target.Value} ({targetCharacterData.UUID})");
            }
            catch (Exception e)
            {
                Log.Write($"takeHelLic Exception: {e.ToString()}");
            }
        }
        public static void takePlaneLic(ExtPlayer player, ExtPlayer target)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.TakePlaneLic)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) return;
                if (player.Position.DistanceTo(target.Position) > 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок слишком далеко", 3000);
                    return;
                }
                if (!targetCharacterData.Licenses[5])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У игрока нет лицензии на самолёт", 3000);
                    return;
                }
                GameLog.FracLog(player.GetFractionId(), characterData.UUID, targetCharacterData.UUID, player.Name, target.Name, "takePlaneLic");
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы отобрали лицензию на самолёт у игрока ({target.Value})", 3000);
                Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) отобрал у Вас лицензию на самолёт", 3000);
                targetCharacterData.Licenses[5] = false;
                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakePlaneLic, $"Изъял лицензию на самолёт у {target.Value} ({targetCharacterData.UUID})");
            }
            catch (Exception e)
            {
                Log.Write($"takePlaneLic Exception: {e.ToString()}");
            }
        }

        public static void giveGunLic(ExtPlayer player, ExtPlayer target, int price)
        {
            try
            {      
                if (!player.IsFractionAccess(RankToAccess.GiveGunLic)) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;            
                if (!sessionData.WorkData.OnDuty && Manager.FractionTypes[player.GetFractionId()] == FractionsType.Gov)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WorkDayNotStarted), 3000);
                    return;
                }
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) return;
                if (player == target) return;
                if (player.Position.DistanceTo(target.Position) > 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                    return;
                }
                if (price < Main.MinGunLic || price > Main.MaxGunLic)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GunLicLimit, Main.MinGunLic, Main.MaxGunLic), 5000);
                    return;
                }
                if (targetCharacterData.Licenses[6])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока уже есть лицензия на оружие", 3000);
                    return;
                }
                if (targetCharacterData.Money < price)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerNotEnoughMoney), 3000);
                    return;
                }
                if (targetSessionData.SellItemData.Seller != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "В данный момент невозможно предложить этому человеку что-либо", 3000);
                    return;
                }
                //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы предложили купить лицензию на оружие игроку ({target.Value}) за ${MoneySystem.Wallet.Format(price)}", 3000);
                Trigger.ClientEvent(target, "openDialog", "GUN_LIC", $"Игрок ({player.Value}) предложил Вам купить лицензию на оружие за ${MoneySystem.Wallet.Format(price)}");
                EventSys.SendCoolMsg(player,"Предложение", "Покупка", $"Вы предложили купить лицензию на оружие игроку ({target.Value}) за ${MoneySystem.Wallet.Format(price)}", "", 5000);
                targetSessionData.SellItemData.Seller = player;
                targetSessionData.SellItemData.Price = price;
            }
            catch (Exception e)
            {
                Log.Write($"giveGunLic Exception: {e.ToString()}");
            }
        }

        public static void acceptGunLic(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                ExtPlayer seller = sessionData.SellItemData.Seller;
                var sellerSessionData = seller.GetSessionData();
                if (sellerSessionData == null) return;
                var sellerCharacterData = seller.GetCharacterData();
                if (sellerCharacterData == null)
                {
                    sessionData.SellItemData = new SellItemData();
                    return;
                }
                int price = sessionData.SellItemData.Price;
                if (player.Position.DistanceTo(seller.Position) > 2)
                {
                    sellerSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Продавец слишком далеко", 3000);
                    return;
                }
                if (characterData.Licenses[6])
                {
                    sellerSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    return;
                }
                if (UpdateData.CanIChange(player, price, true) != 255)
                {
                    sellerSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    return;
                }
                MoneySystem.Wallet.Change(player, -price);
                MoneySystem.Wallet.Change(seller, price / 20);
                
                var fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.CITY);
                if (fractionData != null)
                    fractionData.Money += Convert.ToInt32(price * 0.95);
                
                GameLog.Money($"player({characterData.UUID})", $"frac(6)", price, $"buyGunlic({sellerCharacterData.UUID})");
                GameLog.Money($"frac(6)", $"player({sellerCharacterData.UUID})", price / 20, $"sellGunlic({characterData.UUID})");
                characterData.Licenses[6] = true;
                qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage33, 2, isUpdateHud: true);
                qMain.UpdateQuestsComplete(player, Zdobich.QuestName, (int) zdobich_quests.Stage33, true);
                Fractions.Table.Logs.Repository.AddLogs(seller, FractionLogsType.GivePmLic, $"Выдал лицензию на оружие {player.Value} ({characterData.UUID})");
                //Chars.Repository.PlayerStats(player);
                GameLog.FracLog(seller.GetFractionId(), sellerCharacterData.UUID, characterData.UUID, seller.Name, player.Name, "giveGunLic");
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили лицензию на оружие у игрока ({seller.Value}) за {MoneySystem.Wallet.Format(price)}$", 3000);
                Notify.Send(seller, NotifyType.Info, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) купил у Вас лицензию на оружие", 3000);
                player.AddTableScore(TableTaskId.Item14);
                sellerSessionData.SellItemData = new SellItemData();
                sessionData.SellItemData = new SellItemData();
            }
            catch (Exception e)
            {
                Log.Write($"acceptGunLic Exception: {e.ToString()}");
            }
        }

        public static void playerTakeGuns(ExtPlayer player, ExtPlayer target, bool force = false)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (player.IsInVehicle) return;
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) return;
                if (target.IsInVehicle) return;
                if (!force && !player.IsFractionAccess(RankToAccess.StealGuns)) return;
                if (!targetSessionData.CuffedData.Cuffed && !targetSessionData.HandsUp)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок должен быть связан или с поднятыми руками", 3000);
                    return;
                }
                if (!player.HasSharedData("IS_MASK") || player.GetSharedData<bool>("IS_MASK") == false)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ограбление возможно только в маске", 3000);
                    return;
                }
                
                player.AddTableScore(TableTaskId.Item26);
                
                if (DateTime.Now < sessionData.TimingsData.NextGunRob || DateTime.Now < targetSessionData.TimingsData.NextGunRob)
                {
                    Commands.RPChat("sme", player, "хорошенько обшарив {name}, ничего не нашёл", target);
                    return;
                }
                List<ItemStruct> _ItemsToRemove = new List<ItemStruct>();

                string locationName = $"char_{targetCharacterData.UUID}";
                InventoryItemData Bags = null;
                if (Chars.Repository.ItemsData.ContainsKey(locationName))
                {
                    foreach (string Location in Chars.Repository.ItemsData[locationName].Keys)
                    {
                        //if (Location.Key == "accessories") continue;
                        foreach (KeyValuePair<int, InventoryItemData> itemData in Chars.Repository.ItemsData[locationName][Location])//Todo
                        {
                            InventoryItemData sItem = itemData.Value;
                            ItemsInfo ItemsInfo = Chars.Repository.ItemsInfo[sItem.ItemId];

                            if (sItem.ItemId == ItemId.Bag) Bags = sItem;
                            if (ItemsInfo.functionType == newItemType.Weapons ||
                                ItemsInfo.functionType == newItemType.MeleeWeapons ||
                                sItem.ItemId == ItemId.StunGun)
                            {
                                if (!Chars.Repository.HeavyWeapons.Contains(sItem.ItemId) && !Chars.Repository.StrongWeapons.Contains(sItem.ItemId) && sItem.ItemId != ItemId.Flashlight && sItem.ItemId != ItemId.Crowbar && sItem.ItemId != ItemId.Wrench && sItem.ItemId != ItemId.Hammer) _ItemsToRemove.Add(new ItemStruct(Location, itemData.Key, sItem));
                            }
                        }
                    }
                }

                if (Bags != null && Chars.Repository.ItemsData.ContainsKey($"backpack_{Bags.SqlId}") && Chars.Repository.ItemsData[$"backpack_{Bags.SqlId}"].ContainsKey("backpack"))
                {
                    foreach (KeyValuePair<int, InventoryItemData> itemData in Chars.Repository.ItemsData[$"backpack_{Bags.SqlId}"]["backpack"])//Todo
                    {
                        InventoryItemData sItem = itemData.Value;
                        ItemsInfo ItemsInfo = Chars.Repository.ItemsInfo[sItem.ItemId];

                        if (ItemsInfo.functionType == newItemType.Weapons ||
                            ItemsInfo.functionType == newItemType.MeleeWeapons ||
                            sItem.ItemId == ItemId.StunGun)
                        {
                            if (!Chars.Repository.HeavyWeapons.Contains(sItem.ItemId) && !Chars.Repository.StrongWeapons.Contains(sItem.ItemId) && sItem.ItemId != ItemId.Flashlight && sItem.ItemId != ItemId.Crowbar && sItem.ItemId != ItemId.Wrench && sItem.ItemId != ItemId.Hammer) _ItemsToRemove.Add(new ItemStruct("backpack", itemData.Key, sItem));
                        }
                    }
                }
                if (_ItemsToRemove.Count <= 0)
                {
                    Commands.RPChat("sme", player, "хорошенько обшарив {name}, ничего не нашёл", target);
                    return;
                }
                ItemStruct item = _ItemsToRemove[Main.rnd.Next(0, _ItemsToRemove.Count)];
                if (item.Location != "backpack") Chars.Repository.ItemsDropToIndex(target, item.Location, item.Index);
                else
                {

                    Chars.Repository.ItemsDrop(target, new InventoryItemData(0, item.Item.ItemId, item.Item.Count, item.Item.Data));
                    if (Chars.Repository.ItemsData.ContainsKey($"backpack_{Bags.SqlId}") && Chars.Repository.ItemsData[$"backpack_{Bags.SqlId}"].ContainsKey("backpack") && Chars.Repository.ItemsData[$"backpack_{Bags.SqlId}"]["backpack"].ContainsKey(item.Index))
                    {
                        ItemId ItemIdDell = Chars.Repository.ItemsData[$"backpack_{Bags.SqlId}"]["backpack"][item.Index].ItemId;
                        Chars.Repository.ItemsData[$"backpack_{Bags.SqlId}"]["backpack"][item.Index].ItemId = ItemId.Debug;
                        Chars.Repository.UpdateSqlItemData(locationName, "backpack", item.Index, Chars.Repository.ItemsData[$"backpack_{Bags.SqlId}"]["backpack"][item.Index], ItemIdDell);
                        Chars.Repository.ItemsData[$"backpack_{Bags.SqlId}"]["backpack"].TryRemove(item.Index, out _);

                        Chars.Repository.isBackpackItemsData(target, true);
                    }
                }


                string itemName = Chars.Repository.ItemsInfo[item.Item.ItemId].Name;
                sessionData.TimingsData.NextGunRob = DateTime.Now.AddMinutes(60);
                targetSessionData.TimingsData.NextGunRob = DateTime.Now.AddMinutes(60);
                Trigger.ClientEvent(target, "removeAllWeapons");
                target.RemoveAllWeapons();
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы украли {itemName} у игрока ({target.Value})", 3000);
                Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) украл у Вас {itemName}", 3000);
                Commands.RPChat("sme", player, " выкрал оружие у {name}", target);
            }
            catch (Exception e)
            {
                Log.Write($"playerTakeGuns Exception: {e.ToString()}");
            }
        }

        public static void playerTakeoffMask(ExtPlayer player, ExtPlayer target)
        {
            try
            { 
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (player.IsInVehicle) return;          
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (!sessionData.WorkData.OnDuty && Manager.FractionTypes[player.GetFractionId()] == FractionsType.Gov)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WorkDayNotStarted), 3000);
                    return;
                }
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) return;
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                if (target.IsInVehicle) return;
                if (!targetCharacterData.IsAlive)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerDying), 3000);
                    return;
                }
                ItemStruct maskItem = Chars.Repository.isItem(target, "accessories", ItemId.Mask);
                if (targetSessionData.HeadPocket)
                {
                    ClothesComponents.ClearAccessory(target, 1);
                    ClothesComponents.SetSpecialClothes(target, 1, 0, 0);
                    ClothesComponents.UpdateClothes(player);
                    Trigger.ClientEvent(target, "setPocketEnabled", false);
                    targetSessionData.HeadPocket = false;
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы сняли мешок с игрока ({target.Value})", 3000);
                    Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) снял с Вас мешок", 3000);
                    Commands.RPChat("sme", player, $"снял" + (characterData.Gender ? "" : "а") + " мешок с {name}", target);
                }
                else if (target.HasSharedData("IS_MASK") && target.GetSharedData<bool>("IS_MASK") == true && maskItem != null && Chars.Repository.IsBeard(characterData.Gender, maskItem.Item))
                {
                    Chars.Repository.RemoveIndex(target, maskItem.Location, maskItem.Index, 1);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы сорвали маску с игрока ({target.Value})", 3000);
                    Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) сорвал с Вас маску", 3000);
                    Commands.RPChat("sme", player, " сорвал маску с {name}", target);
                }
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У игрока нет маски/мешка", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"playerTakeoffMask Exception: {e.ToString()}");
            }
        }
        #endregion

        #region crimeCommands

        public static void playerHandsupOffer(ExtPlayer player, ExtPlayer target)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                
                if (!sessionData.CuffedData.Cuffed && !sessionData.DeathData.InDeath && !sessionData.HandsUp)
                {
                    if (!targetSessionData.CuffedData.Cuffed && !targetSessionData.DeathData.InDeath && !targetSessionData.HandsUp)
                    {
                        if (targetSessionData.RequestData.IsRequested)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете заставить игрока поднять руки в данный момент", 3000);
                            return;
                        }

                        targetSessionData.RequestData.IsRequested = true;
                        targetSessionData.RequestData.Request = "HANDSUP";
                        targetSessionData.RequestData.From = player;
                        targetSessionData.RequestData.Time = DateTime.Now.AddSeconds(10);
                        
                        //Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) заставляет вас поднять руки. Вы желаете подчиниться требованиям и поднять руки? Y/N - принять/отклонить", 3000);
                        EventSys.SendCoolMsg(target,"Предложение", "Поднять руки", $"Игрок ({player.Value}) заставляет вас поднять руки. Вы желаете подчиниться требованиям и поднять руки? Y/N - принять/отклонить", "", 10000);
                        //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы заставляете игрока ({target.Value}) поднять руки.", 3000);
                        EventSys.SendCoolMsg(player,"Предложение", "Поднять руки", $"Вы заставляете игрока ({target.Value}) поднять руки.", "", 7000);
                        Commands.RPChat("sme", player, LangFunc.GetText(LangType.Ru, DataName.HansOn, target.Name));
                    }
                    else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете заставить игрока поднять руки в данный момент", 3000);
                }
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете заставить игрока поднять руки в данный момент", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"playerHandsupOffer Exception: {e.ToString()}");
            }
        }
        
        public static void playerHandsupOfferAgree(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                ExtPlayer target = sessionData.RequestData.From;
                sessionData.RequestData = new RequestData();

                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                
                if (!targetSessionData.CuffedData.Cuffed && !targetSessionData.DeathData.InDeath && !targetSessionData.HandsUp && !target.IsInVehicle)
                {
                    if (!sessionData.CuffedData.Cuffed && !sessionData.DeathData.InDeath && !sessionData.HandsUp && !player.IsInVehicle)
                    {
                        if (targetSessionData.RequestData.IsRequested)
                        {
                            Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете заставить игрока поднять руки в данный момент", 3000);
                            return;
                        }

                        Animations.AnimationPlay(player, "2_1");
                        Commands.RPChat("sme", player, " поднял руки");
                        
                    }
                    else Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете заставить игрока поднять руки в данный момент", 3000);
                }
                else Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете заставить игрока поднять руки в данный момент", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"playerHandsupOfferAgree Exception: {e.ToString()}");
            }
        }

        public static void robberyTarget(ExtPlayer player, ExtPlayer target)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (player.IsInVehicle) return;
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) return;
                if (target.IsInVehicle) return;
                if (!targetSessionData.CuffedData.Cuffed && !targetSessionData.HandsUp)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок должен быть связан или с поднятыми руками", 3000);
                    return;
                }
                if (!player.HasSharedData("IS_MASK"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ограбление возможно только в маске", 3000);
                    return;
                }
                
                player.AddTableScore(TableTaskId.Item26);
                
                if (targetCharacterData.AdminLVL != 0 || targetCharacterData.LVL < 2 || targetCharacterData.Money <= 100 || DateTime.Now < targetSessionData.TimingsData.NextRob)
                {
                    Commands.RPChat("sdo", target, "Ничего нет при себе");
                    Commands.RPChat("sme", player, "хорошенько обшарив {name}, ничего не нашёл", target);
                    return;
                }
                int max = (targetCharacterData.Money >= Main.PricesSettings.MaxOgrableine) ? Main.PricesSettings.MaxOgrableine : Convert.ToInt32(targetCharacterData.Money) - 20;
                int min = (max - 20 < 0) ? max : max - 20;
                int found = Main.rnd.Next(min, max + 1);
                if (targetCharacterData.Money >= 10000) Commands.RPChat("sdo", target, $"При себе большое количество денег");
                else Commands.RPChat("sdo", target, $"При себе ${MoneySystem.Wallet.Format(targetCharacterData.Money)}");
                MoneySystem.Wallet.Change(target, -found);
                MoneySystem.Wallet.Change(player, found);
                GameLog.Money($"player({targetCharacterData.UUID})", $"player({characterData.UUID})", found, $"robbery");
                targetSessionData.TimingsData.NextRob = DateTime.Now.AddMinutes(60);
                Commands.RPChat("sme", player, "хорошенько обшарив {name}" + $", нашёл ${found}", target);
            }
            catch (Exception e)
            {
                Log.Write($"robberyTarget Exception: {e.ToString()}");
            }
        }
        public static void  playerChangePocket(ExtPlayer player, ExtPlayer target, bool force = false)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (player.IsInVehicle) return;
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                if (!target.IsCharacterData() || target.IsInVehicle) return;
                if (player.Position.DistanceTo(target.Position) > 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                    return;
                }
                if (!force && !player.IsFractionAccess(RankToAccess.Pocket)) return;
                if (targetSessionData.IsCasinoGame != null) return;
                if (targetSessionData.HeadPocket)
                {
                    ClothesComponents.ClearAccessory(target, 1);
                    ClothesComponents.SetSpecialClothes(target, 1, 0, 0);
                    ClothesComponents.UpdateClothes(player);
                    Trigger.ClientEvent(target, "setPocketEnabled", false);
                    targetSessionData.HeadPocket = false;
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы сняли мешок с игрока ({target.Value})", 3000);
                    Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) снял с Вас мешок", 3000);
                    Commands.RPChat("sme", player, $"снял" + (characterData.Gender ? "" : "а") + " мешок с {name}", target);
                }
                else
                {
                    ItemStruct aItem = Chars.Repository.isItem(player, "inventory", ItemId.Pocket);
                    if (aItem == null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет мешков", 3000);
                        return;
                    }
                    ClothesComponents.SetSpecialAccessories(target, 1, 24, 2);
                    ClothesComponents.SetSpecialClothes(target, 1, 56, 1);
                    ClothesComponents.UpdateClothes(player);
                    Trigger.ClientEvent(target, "setPocketEnabled", true);
                    targetSessionData.HeadPocket = true;
                    Chars.Repository.RemoveIndex(player, aItem.Location, aItem.Index, 1);
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы надели мешок на игрока ({target.Value})", 3000);
                    Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) надел на Вас мешок", 3000);
                    Commands.RPChat("sme", player, $"надел" + (characterData.Gender ? "" : "а") + " мешок на {name}", target);
                }
            }
            catch (Exception e)
            {
                Log.Write($"playerChangePocket Exception: {e.ToString()}");
            }
        }
        #endregion
        
        public static void giveParamedicLic(ExtPlayer player, ExtPlayer target, int price)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.GivePmLic)) return;
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) return;
                if (target.Position.DistanceTo(player.Position) > 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                    return;
                }
                if (targetCharacterData.LVL < 10)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MinorYearsToDoThis), 3000);
                    return;
                }
                if (price < Main.MinPMLic || price > Main.MaxPMLic)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PriceCanBeFromTo, Main.MinPMLic, Main.MaxPMLic), 3000);
                    return;
                }
                if (targetCharacterData.Licenses[8])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerHavePMLic), 3000);
                    return;
                }
                if (targetCharacterData.Money < price)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerNotEnoughMoney), 3000);
                    return;
                }
                if (targetSessionData.SellItemData.Seller != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerCantAcceptOffers), 3000);
                    return;
                }
                //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouOfferedPMLicTo, target.Value, MoneySystem.Wallet.Format(price)), 3000);
                EventSys.SendCoolMsg(player,"Предложение", "Покупка", $"{LangFunc.GetText(LangType.Ru, DataName.YouOfferedPMLicTo, target.Value, MoneySystem.Wallet.Format(price))}", "", 5000);
                Trigger.ClientEvent(target, "openDialog", "PM_LIC", LangFunc.GetText(LangType.Ru, DataName.PlayerOfferedPMLic, player.Value, MoneySystem.Wallet.Format(price)));
                targetSessionData.SellItemData.Seller = player;
                targetSessionData.SellItemData.Price = price;
            }
            catch (Exception e)
            {
                Log.Write($"giveParamedicLic Exception: {e.ToString()}");
            }
        }

        public static void acceptPmLic(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                ExtPlayer seller = sessionData.SellItemData.Seller;
                var sellerSessionData = seller.GetSessionData();
                if (sellerSessionData == null) return;
                var sellerCharacterData = seller.GetCharacterData();
                if (sellerCharacterData == null)
                {
                    sessionData.SellItemData = new SellItemData();
                    return;
                }
                int price = sessionData.SellItemData.Price;
                if (player.Position.DistanceTo(seller.Position) > 2)
                {
                    sellerSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SellerTooFar), 3000);
                    return;
                }
                if (characterData.Licenses[8])
                {
                    sellerSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    return;
                }
                if (UpdateData.CanIChange(player, price, true) != 255)
                {
                    sellerSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    return;
                }
                MoneySystem.Wallet.Change(player, -price);
                MoneySystem.Wallet.Change(seller, price / 20);
                
                var fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.CITY);
                if (fractionData != null)
                    fractionData.Money += Convert.ToInt32(price * 0.95);
                
                GameLog.Money($"player({characterData.UUID})", $"frac(6)", price, $"buyPmlic({sellerCharacterData.UUID})");
                GameLog.Money($"frac(6)", $"player({sellerCharacterData.UUID})", price / 20, $"sellPmlic({characterData.UUID})");
                characterData.Licenses[8] = true;
                Fractions.Table.Logs.Repository.AddLogs(seller, FractionLogsType.GiveGunLic, LangFunc.GetText(LangType.Ru, DataName.GivenPmLicTo, player.Value, characterData.UUID));
                //Chars.Repository.PlayerStats(player);
                GameLog.FracLog(seller.GetFractionId(), sellerCharacterData.UUID, characterData.UUID, seller.Name, player.Name, "givePmLic");
                Notify.Send(seller, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGivePmLic, player.Name), 3000);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouBoughtPmLic, seller.Name), 3000);
                sellerSessionData.SellItemData = new SellItemData();
                sessionData.SellItemData = new SellItemData();
            }
            catch (Exception e)
            {
                Log.Write($"acceptPmLic Exception: {e.ToString()}");
            }
        }

        #region EMS commands
        public static void giveMedicalLic(ExtPlayer player, ExtPlayer target)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.GiveMedLic)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) return;
                if (target.Position.DistanceTo(player.Position) > 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                    return;
                }
                if (targetCharacterData.Licenses[7])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока уже есть мед. карта", 3000);
                    return;
                }
                targetCharacterData.Licenses[7] = true;
                qMain.UpdateQuestsStage(target, Zdobich.QuestName, (int)zdobich_quests.Stage31, 2, isUpdateHud: true);
                qMain.UpdateQuestsComplete(target, Zdobich.QuestName, (int) zdobich_quests.Stage31, true);
                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.GiveMedLic, $"Выдал медицинскую карту {target.Value} ({targetCharacterData.UUID})");
                //Chars.Repository.PlayerStats(target);
                GameLog.FracLog(player.GetFractionId(), characterData.UUID, targetCharacterData.UUID, player.Name, target.Name, "giveMedLic");
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы выдали игроку {target.Name} медицинскую карту", 3000);
                Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, $"{player.Name} выдал Вам медицинскую карту", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"giveMedicalLic Exception: {e.ToString()}");
            }
        }
        
        public static void giveQr(ExtPlayer player, ExtPlayer target)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                var fracId = player.GetFractionId();
                if (fracId != (int) Models.Fractions.EMS) return;
                
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) return;
                
                if (target.Position.DistanceTo(player.Position) > 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                    return;
                }
                
                if (Chars.Repository.isItem(target, "inventory", ItemId.Qr) != null) 
                { 
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouAlreadyHaveQR), 3000); 
                    return; 
                }
                
                if (player == target)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantQrSebe), 3000);
                    return;
                }
                
                if (Chars.Repository.AddNewItem(target, $"char_{targetCharacterData.UUID}", "inventory", ItemId.Qr, 1) == -1)
                {
                    Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                    return;
                } 
                
                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.GiveMedLic, $"Выдал QR код {target.Name}({targetCharacterData.UUID})");
                player.AddTableScore(TableTaskId.Item23);
                GameLog.FracLog(fracId, characterData.UUID, targetCharacterData.UUID, player.Name, target.Name, "giveQr");
                
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы выдали игроку {target.Name} QR-код", 3000);
                Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, $"{player.Name} выдал Вам QR-код", 3000);
                
                Trigger.PlayAnimation(player, "missfbi3_syringe", "syringe_use_player", 50);
                
                Timers.StartOnce(3000, () => {
                    Trigger.StopAnimation(player);
                });
            }
            catch (Exception e)
            {
                Log.Write($"giveQr Exception: {e.ToString()}");
            }
        }
          
        public static void sellMedKitToTarget(ExtPlayer player, ExtPlayer target, int price)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) return;
                if (player.IsFractionAccess(RankToAccess.Medkit))
                {
                    if ((sessionData.SellItemData.Buyer != null || sessionData.SellItemData.Seller != null) && Chars.Repository.TradeGet(player))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantTrade), 3000);
                        return;
                    }
                    if (!sessionData.WorkData.OnDuty)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000);
                        return;
                    }
                    if (Chars.Repository.isItem(player, "inventory", ItemId.HealthKit) == null || Chars.Repository.TradeGet(target))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет аптечек с собой", 3000);
                        return;
                    }
                    if (price < 500 || price > 1500)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны установить цену от 500$ до 1500$", 3000);
                        return;
                    }
                    if (player.Position.DistanceTo(target.Position) > 2)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                        return;
                    }
                    if (targetCharacterData.Money < price)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока нет столько денег", 3000);
                        return;
                    }
                    if ((targetSessionData.SellItemData.Seller != null || targetSessionData.SellItemData.Buyer != null) && Chars.Repository.TradeGet(target))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerAlreadyTraded), 3000);
                        return;
                    }
                    if (Main.IHaveDemorgan(player, true)) return;
                    Trigger.ClientEvent(target, "openDialog", "PAY_MEDKIT", $"Медик ({player.Value}) предложил купить Вам аптечку за ${MoneySystem.Wallet.Format(price)}.");
                    targetSessionData.SellItemData.Seller = player;
                    targetSessionData.SellItemData.Buyer = target;
                    targetSessionData.SellItemData.Price = price;
                    sessionData.SellItemData.Seller = player;
                    sessionData.SellItemData.Buyer = target;
                    sessionData.SellItemData.Price = price;
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы предложили купить игроку ({target.Value}) аптечку за {MoneySystem.Wallet.Format(price)}$", 3000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"sellMedKitToTarget Exception: {e.ToString()}");
            }
        }

        public static void healTarget(ExtPlayer player, ExtPlayer target, int price)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                if (!target.IsCharacterData()) return;
                if (player.IsFractionAccess(RankToAccess.Heal))
                {
                    if (!sessionData.WorkData.OnDuty)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WorkDayNotStarted), 3000);
                        return;
                    }
                    if (player.Position.DistanceTo(target.Position) > 2)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок слишком далеко.", 3000);
                        return;
                    }
                    if (price < Main.MinHealLimit || price > Main.MaxHealLimit)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны установить цену от {Main.MinHealLimit}$ до {Main.MaxHealLimit}$", 3000);
                        return;
                    }
                    if ((targetSessionData.SellItemData.Seller != null || targetSessionData.SellItemData.Buyer != null) && Chars.Repository.TradeGet(target))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У данного человека уже есть предложение.", 3000);
                        return;
                    }
                    if (Main.IHaveDemorgan(player, true)) return;
                    if ((sessionData.SellItemData.Buyer != null || sessionData.SellItemData.Seller != null) && Chars.Repository.TradeGet(player))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "В данный момент вы не предложить лечение.", 3000);
                        return;
                    }
                    if (NAPI.Player.IsPlayerInAnyVehicle(player) && NAPI.Player.IsPlayerInAnyVehicle(target))
                    {
                        var veh = (ExtVehicle) player.Vehicle;
                        if (veh != target.Vehicle)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок сидит в другой машине.", 3000);
                            return;
                        }
                        var vehicleLocalData = veh.GetVehicleLocalData();
                        if (vehicleLocalData != null)
                        {
                            if (vehicleLocalData.Access == VehicleAccess.Fraction && vehicleLocalData.Fraction == (int) Models.Fractions.EMS && (veh.Model == NAPI.Util.GetHashKey("emsnspeedo") || veh.Model == NAPI.Util.GetHashKey("emsroamer") || veh.Model == NAPI.Util.GetHashKey("vapidse") || veh.Model == (uint)VehicleHash.Ambulance || veh.Model == (uint)VehicleHash.Frogger || veh.Model == (uint)VehicleHash.Supervolito || veh.Model == (uint)VehicleHash.Maverick || veh.Model == (uint)VehicleHash.Lguard))
                            {
                                targetSessionData.SellItemData.Seller = player;
                                targetSessionData.SellItemData.Price = price;
                                targetSessionData.SellItemData.Buyer = target;

                                sessionData.SellItemData.Seller = player;
                                sessionData.SellItemData.Price = price;
                                sessionData.SellItemData.Buyer = target;
                                Trigger.ClientEvent(target, "openDialog", "PAY_HEAL", $"Медик ({player.Value}) предложил лечение за ${MoneySystem.Wallet.Format(price)}");
                                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы предложили лечение игроку ({target.Value}) за {MoneySystem.Wallet.Format(price)}$", 3000);
                            }
                            else
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы сидите не в карете/вертолёте EMS.", 3000);
                                return;
                            }
                        }
                        return;
                    }
                    else if (SafeZones.IsSafeZone(sessionData.InsideSafeZone, SafeZones.ZoneName.EMS) && SafeZones.IsSafeZone(targetSessionData.InsideSafeZone, SafeZones.ZoneName.EMS))
                    {
                        targetSessionData.SellItemData.Seller = player;
                        targetSessionData.SellItemData.Price = price;
                        targetSessionData.SellItemData.Buyer = target;

                        sessionData.SellItemData.Seller = player;
                        sessionData.SellItemData.Price = price;
                        sessionData.SellItemData.Buyer = target;
                        Trigger.ClientEvent(target, "openDialog", "PAY_HEAL", $"Медик ({player.Value}) предложил лечение за ${MoneySystem.Wallet.Format(price)}");
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы предложили лечение игроку ({target.Value}) за {MoneySystem.Wallet.Format(price)}", 3000);
                        return;
                    }
                    else
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны быть в больнице или карете скорой помощи.", 3000); ;
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"healTarget Exception: {e.ToString()}");
            }
        }
        #endregion

    }
}