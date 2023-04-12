using System;
using Database;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Handles;
using NeptuneEvo.Accounts;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Events;
using NeptuneEvo.Fractions;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Functions;
using NeptuneEvo.GUI;
using NeptuneEvo.Houses;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.Players.Models;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;
using Redage.SDK;
using NeptuneEvo.Fractions.LSNews;

namespace NeptuneEvo.Players.Disconnect
{
    public class Repository
    {
        private static readonly nLog Log = new nLog("Players.Disconnect.Repository");
        public static void OnPlayerDisconnect(ExtPlayer player, DisconnectionType type, string reason, bool isSave = true)
        {
            try
            {
                var playerId = player.Value;

                var accountData = player.GetAccountData();
                var characterData = player.GetCharacterData();
                var sessionData = player.GetSessionData();
                
                if (sessionData != null) 
                    sessionData.IsConnect = false;

                if (Main.PlayersOnLogin.Contains(player))
                    Main.PlayersOnLogin.Remove(player);
                
                var login = "";

                if (accountData != null)
                    login = accountData.Login.ToLower();
                
                if (Main.Characters.Contains(player))
                    Main.Characters.Remove(player);

                if (Queue.Repository.List.Contains(player)) 
                    Queue.Repository.List.Remove(player);

                if (Main.AllAdminsOnline.Contains(player)) 
                    Main.AllAdminsOnline.Remove(player);

                Chars.Repository.OtherClose(player);
                StreetRace.ClearData(player, 2);
                EventSys.OnPlayerDisconnected(player);
                Jobs.Miner.OnPlayerDisconnected(player);
                Voice.Voice.OnPlayerDisconnected(player);
                
                Inventory.Drop.Repository.OnPlayerDisconnected(player);
                Inventory.Tent.Repository.Disconnect(player);
                Admin.RemoveQueue(player, "выходом игрока");
                Houses.Rieltagency.Repository.OnClose(player);
                
                WeaponRepository.OnClearTimerWeaponUpdate(player);

                NeptuneEvo.Events.HeliCrash.Repository.ClearBox(player);

                World.War.Repository.OnPlayerDisconnect(player);
                
                try
                {
                    if (Commands.ActionLabels.ContainsKey(playerId))
                    {
                        if (Commands.ActionLabels[player.Value].Item1 != null && Commands.ActionLabels[player.Value].Item1.Exists) Commands.ActionLabels[player.Value].Item1.Delete();
                        CustomColShape.DeleteColShape(Commands.ActionLabels[player.Value].Item2);
                        Commands.ActionLabels.Remove(playerId);
                    }
                }
                catch (Exception e)
                {
                    Log.Write($"Event_OnPlayerDisconnected - ActionLabels.Delete() Exception: {e.ToString()}");
                }

                if (characterData != null)
                {
                    player.DisconnectFraction();
                    player.DisconnectOrganization();
                    
                    PedSystem.Pet.Repository.UnLoad(player);
                    if (sessionData.SelectData.SelectedVeh != null)
                    {
                        ExtVehicle selveh = sessionData.SelectData.SelectedVeh;
                        var vehicleLocalData = selveh.GetVehicleLocalData();
                        if (vehicleLocalData != null) 
                            vehicleLocalData.BagInUse = false;
                    }
                    if (sessionData.DicePlayingWith != null)
                    {
                        ExtPlayer target = sessionData.DicePlayingWith;
                        var targetSessionData = target.GetSessionData();
                        if (targetSessionData != null)
                        {
                            targetSessionData.DicePlayingWith = null;
                            targetSessionData.DiceMoney = 0;
                            Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ChelVishelNoGame), 3000);
                        }
                        sessionData.DicePlayingWith = null;
                        sessionData.DiceMoney = 0;
                    }
                    if ((sessionData.SellItemData.Seller != null || sessionData.SellItemData.Buyer != null) && Chars.Repository.TradeGet(player))
                    {
                        ExtPlayer seller = sessionData.SellItemData.Seller;
                        var sellerSessionData = seller.GetSessionData();
                        ExtPlayer buyer = sessionData.SellItemData.Buyer;
                        var buyerSessionData = buyer.GetSessionData();
                        if (seller == player)
                        {
                            if (buyerSessionData != null)
                            {
                                buyerSessionData.SellItemData = new SellItemData();
                                Notify.Send(buyer, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DisconnectedSellClose), 3000);
                            }
                        }
                        else if (buyer == player)
                        {
                            if (sellerSessionData != null)
                            {
                                sellerSessionData.SellItemData = new SellItemData();
                                Notify.Send(seller, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DisconnectedSellClose), 3000);
                            }
                        }
                        sessionData.SellItemData = new SellItemData();
                    }
                    if (sessionData.AttachToVehicle != null)
                    {
                        var vehicle = (ExtVehicle) sessionData.AttachToVehicle;
                        var vehicleLocalData = vehicle.GetVehicleLocalData();
                        if (vehicleLocalData != null)
                        {
                            vehicleLocalData.AttachToPlayer = null;
                        }
                    }
                    VehicleManager.WarpPlayerOutOfVehicle(player, false);
                    if (sessionData.WorkData.OnDuty)
                    {
                        characterData.OnDutyName = sessionData.WorkData.OnDutyName;
                    }
                    else
                    {
                        characterData.OnDutyName = String.Empty;
                    }

                    if (type != DisconnectionType.Kicked && characterData.AdminLVL == 0)
                    {
                        CuffedData cufdata = sessionData.CuffedData;
                        if (cufdata.Cuffed && (cufdata.CuffedByMafia || cufdata.CuffedByCop) && characterData.DemorganTime <= 0)
                        {
                            if (cufdata.CuffedByCop)
                            {
                                NAPI.Chat.SendChatMessageToAll(LangFunc.GetText(LangType.Ru, DataName.DemorganCuffs, player.Name));
                                GameLog.Admin($"server", $"demorgan(4ч,выход из игры в наручниках)", $"{player.Name}");
                            }
                            else if (cufdata.CuffedByMafia)
                            {
                                NAPI.Chat.SendChatMessageToAll(LangFunc.GetText(LangType.Ru, DataName.DemorganStyzahka, player.Name));
                                GameLog.Admin($"server", $"demorgan(4ч,выход из игры в стяжках)", $"{player.Name}");
                            }

                            characterData.DemorganInfo.Admin = "Server";
                            characterData.DemorganInfo.Reason = "выход из игры в наручниках или стяжках.";

                            characterData.DemorganTime = 14400;
                            Chars.Repository.RemoveAllWeapons(player, true, true, armour: true);
                        }
                    }
                    
                    //
                    
                    var house = HouseManager.GetHouse(player);

                    if (house != null) 
                        house.GaragePlayerExit(player);

                    var vehiclesNumber = VehicleManager.GetVehiclesAirNumberToPlayer(player.Name);
                    foreach (string numberDell in vehiclesNumber)
                        VehicleStreaming.DeleteVehicle(VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Personal, numberDell));
                    
                    //

                    SafeMain.SafeCracker_Disconnect(player, type, reason);

                    if (sessionData.SappeData != -1) 
                        Chars.Repository.Sappe_PlayerDisconnected(player);
                    if (sessionData.ItemsTrade != null) 
                        Chars.Repository.ItemsClose(player);
                    
                    if (sessionData.RentData != null)
                    {
                        var vehicleLocalData = sessionData.RentData.Vehicle.GetVehicleLocalData();
                        if (vehicleLocalData != null)
                        {
                            vehicleLocalData.IsOwnerExit = true;
                            vehicleLocalData.RentCarDeleteTime = DateTime.Now.AddMinutes(10);
                        }
                        //VehicleStreaming.DeleteVehicle(sessionData.RentData.Vehicle);
                    }

                    Phone.Taxi.Orders.Repository.OnPlayerDisconnect(player);
                    Phone.Taxi.Repository.OnPlayerDisconnect(player);
                    Phone.Mechanic.Repository.OnPlayerDisconnect(player);
                    Jobs.Truckers.CancelOrder(player);
                    
                    CarRoom.timer_exitVehicle(player, true);
                    TankRoyale.PlayerExitFromTR(player, true, true, checkwinner: true);
                    Chars.Repository.RouletteClose(player);
                    Chars.Repository.ChangeAutoNumberClear(player);
                    ReportSys.OnAdminDisconnect(player);
                    LsNewsSystem.OnDisconnect(player);
                    Army.onPlayerDisconnected(player, type, reason);
                    Ems.onPlayerDisconnectedhandler(player, type, reason);
                    NeptuneEvo.Events.AirDrop.Repository.onPlayerDisconnectedhandler(player);
                    Selecting.onPlayerDisconnectedhandler(player);
                    Police.onPlayerDisconnectedhandler(player, type, reason);
                    CarDelivery.Event_PlayerDisconnected(player);
                    HouseManager.Event_OnPlayerDisconnected(player, type, reason);
                    Airsoft.OnPlayerDisconnected(player);
                    MafiaGame.OnPlayerDisconnected(player);
                    Hotel.Event_OnPlayerDisconnected(player);
                    NewCasino.Blackjack.Disconnect(player, type);
                    NewCasino.Roullete.Disconnect(player, type);
                    NewCasino.Horses.Disconnect(player, type);
                    NewCasino.Spin.Disconnect(player, type);
                    Voice.Voice.PlayerQuit(player, reason);
                    GameLog.Disconnected(characterData.UUID, player.Value, Enum.GetName(typeof(OwnDisconnectionType), type), login);
                    PedSystem.LivingCity.Repository.OnPlayerDisconnect(player);

                    if (characterData.AdminLVL >= 1 && characterData.AdminLVL <= 5)
                        Trigger.SendToAdmins(1, $"!{{#FFB833}}[A] {player.Name} отключился ({characterData.AdminLVL} lvl)");
                    
                    Commands.RPChat("sb", player, LangFunc.GetText(LangType.Ru, DataName.PlayerDisconnect), optName: player.Name + $" ({player.Value})");

                    if (Main.PlayerUUIDToPlayerId.ContainsKey(characterData.UUID)) 
                        Main.PlayerUUIDToPlayerId.TryRemove(characterData.UUID, out _);     
                }

                if (sessionData != null)
                {
                    //sessionData = player.GetSessionData();
                    if (sessionData.TimersData.AutoDCTimer != null)
                    {
                        Timers.Stop(sessionData.TimersData.AutoDCTimer);
                        sessionData.TimersData.AutoDCTimer = null;
                    }
                    // Подстраховка и проверка всех возможных таймеров у игрока
                    if (sessionData.TimersData.HealTimer != null)
                    {
                        Timers.Stop(sessionData.TimersData.HealTimer);
                        sessionData.TimersData.HealTimer = null;
                    }
                    if (sessionData.TimersData.ArrestTimer != null)
                    {
                        Timers.Stop(sessionData.TimersData.ArrestTimer);
                        sessionData.TimersData.ArrestTimer = null;
                    }
                    if (sessionData.TimersData.BusTimer != null)
                    {
                        Timers.Stop(sessionData.TimersData.BusTimer);
                        sessionData.TimersData.BusTimer = null;
                    }
                    if (sessionData.TimersData.DeathTimer != null)
                    {
                        Timers.Stop(sessionData.TimersData.DeathTimer);
                        sessionData.TimersData.DeathTimer = null;
                    }
                    if (sessionData.TimersData.LoadMatsTimer != null)
                    {
                        Timers.Stop(sessionData.TimersData.LoadMatsTimer);
                        sessionData.TimersData.LoadMatsTimer = null;
                    }
                    if (sessionData.TimersData.MuteTimer != null)
                    {
                        Timers.Stop(sessionData.TimersData.MuteTimer);
                        sessionData.TimersData.MuteTimer = null;
                    }
                    if (sessionData.TimersData.ResistTimer != null)
                    {
                        Timers.Stop(sessionData.TimersData.ResistTimer);
                        sessionData.TimersData.ResistTimer = null;
                    }
                    if (sessionData.TimersData.SchoolTimer != null)
                    {
                        Timers.Stop(sessionData.TimersData.SchoolTimer);
                        sessionData.TimersData.SchoolTimer = null;
                    }
                    if (sessionData.TimersData.TestDriveTimer != null)
                    {
                        Timers.Stop(sessionData.TimersData.TestDriveTimer);
                        sessionData.TimersData.TestDriveTimer = null;
                    }
                    if (sessionData.TimersData.WorkExitTimer != null)
                    {
                        Timers.Stop(sessionData.TimersData.WorkExitTimer);
                        sessionData.TimersData.WorkExitTimer = null;
                    }
                    if (sessionData.TimersData.CheckInVeh != null)
                    {
                        Timers.Stop(sessionData.TimersData.CheckInVeh);
                        sessionData.TimersData.CheckInVeh = null;
                    }
                }

                if (Main.PlayerIdToEntity.ContainsKey(playerId)) 
                    Main.PlayerIdToEntity.TryRemove(playerId, out _);

                foreach (string key in NAPI.Data.GetAllEntityData(player)) 
                    player.ResetData(key);
                
                player.SavePlayerPosition();
                
                if (characterData != null && characterData.IsSpawned && !characterData.IsAlive)
                    Chars.Repository.RemoveAllWeapons(player, true, true);

                if (characterData != null && Chars.Repository.ItemsData.ContainsKey($"char_{characterData.UUID}") && Chars.Repository.ItemsData[$"char_{characterData.UUID}"].ContainsKey("accessories") && Chars.Repository.ItemsData[$"char_{characterData.UUID}"]["accessories"].ContainsKey(7))
                {
                    var aItem = Chars.Repository.ItemsData[$"char_{characterData.UUID}"]["accessories"][7];
                    aItem.Data = $"{characterData.Armor}";
                    Chars.Repository.UpdateSqlItemData($"char_{characterData.UUID}", "accessories", 7, aItem);
                }
                
                Log.Write(player.Name + " disconnected from server. (" + reason + ")");

                if (isSave)
                {
                    Trigger.SetTask(async () =>
                    {
                        try
                        {
                            await using var db = new ServerBD("MainDB"); //В отдельном потоке

                            await Accounts.Save.Repository.SaveSql(db, player);

                            await Character.Save.Repository.SaveSql(db, player);
                        }
                        catch (Exception e)
                        {
                            Debugs.Repository.Exception(e);
                        }
                    });
                    Accounts.Email.Repository.VerificationDelete(player);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Event_OnPlayerDisconnected Exception: {e.ToString()}");
            }
        }
    }
}