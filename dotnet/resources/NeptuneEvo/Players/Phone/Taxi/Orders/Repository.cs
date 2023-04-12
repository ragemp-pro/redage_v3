using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Handles;
using NeptuneEvo.Jobs.Models;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players.Phone.Messages.Models;
using NeptuneEvo.Quests;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Players.Phone.Taxi.Orders
{
    public class Repository
    {        
        public static void StartWork(ExtPlayer player)
        {
                                                         
            var sessionData = player.GetSessionData();       
            if (sessionData == null) 
                return;
            
            var characterData = player.GetCharacterData();         
            if (characterData == null) 
                return;
            
            sessionData.WorkData.OnWork = true;
            
            var orders = new List<List<object>>();
            
            foreach (var order in Phone.Taxi.Repository.OrdersList)
            {
                if (order.Driver != null)
                    continue;
                        
                var foreachPlayer = order.Player;
                    
                var foreachSessionData = foreachPlayer.GetSessionData();
                if (foreachSessionData == null) 
                    continue;

                var foreachCharacterData = foreachPlayer.GetCharacterData();
                if (foreachCharacterData == null) 
                    continue;
                    
                orders.Add(new List<object>
                {
                    foreachCharacterData.UUID,
                    foreachSessionData.Name,
                    foreachPlayer.Position.X,
                    foreachPlayer.Position.Y,
                    foreachPlayer.Position.Z,
                });
            }
            
            Trigger.ClientEvent(player, "client.phone.taxijob.init", JsonConvert.SerializeObject(orders));
        }
        
        public static void OnTake(ExtPlayer player, int id)
        {
            var sessionData = player.GetSessionData();       
            if (sessionData == null) 
                return;
            
            if (sessionData.TaxiData.Driver != null || sessionData.TaxiData.Passager != null) 
                return;
            
            var characterData = player.GetCharacterData();         
            if (characterData == null) 
                return;

            var vehicle = (ExtVehicle) player.Vehicle;
            
            if (vehicle != null && characterData.WorkID == (int) JobsId.Taxi &&
                (sessionData.RentData != null || sessionData.WorkData.OnWork))
            {
                
                var taxiData = Phone.Taxi.Repository.OrdersList.FirstOrDefault(ol => ol.PlayerUUID == id);

                if (taxiData != null && taxiData.Player.IsCharacterData())
                {
                    taxiData.Driver = player;

                    GetSelect(player, isTake: true);
                    Trigger.ClientEvent(taxiData.Player, "client.phone.taxi.updateOrder", sessionData.Name, vehicle.NumberPlate, player.Position.X, player.Position.Y, player.Position.Z);
                    
                    taxiData.DriverTimer = Timers.Start(2500, () => OnUpdatePos(player, taxiData.Player), isnapitask: true);
                    
                    foreach (var foreachPlayer in Players.Phone.Taxi.Repository.GetListTaxi())
                    {
                        if (!foreachPlayer.IsCharacterData())
                            continue;

                        Trigger.ClientEvent(foreachPlayer, "client.phone.taxijob.dell", taxiData.PlayerUUID);
                    }
                }
                
            }
        }

        public static void OnUpdatePos(ExtPlayer player, ExtPlayer target)
        {
                
            if (!target.IsCharacterData()) 
                return;
            
            var sessionData = player.GetSessionData();       
            if (sessionData == null) 
                return;
            
            var characterData = player.GetCharacterData();         
            if (characterData == null) 
                return;

            if (player.Position.DistanceTo2D(target.Position) < 10f)
            {
                //Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, "Такси подъехало.", 8000);
                Trigger.ClientEvent(player, "phone.notify", (int) DefaultNumber.Taxi, $"Ваше такси подъехало!", 8); 
                Phone.Taxi.Repository.OnCancel(target, isNotRemove: false);
                return;
            }
            
            var vehicle = (ExtVehicle) player.Vehicle;

            if (vehicle != null && characterData.WorkID == (int) JobsId.Taxi &&
                (sessionData.RentData != null || sessionData.WorkData.OnWork))
            {
                Trigger.ClientEvent(target, "client.phone.taxi.updatePosOrder", player.Position.X, player.Position.Y, player.Position.Z);
            }
        }
        
        public static void GetSelect(ExtPlayer player, bool isTake = false)
        {
            var sessionData = player.GetSessionData();       
            if (sessionData == null) 
                return;
            
            var characterData = player.GetCharacterData();         
            if (characterData == null) 
                return;
            
            var selectedOrders = new List<object>();
            
            if (characterData.WorkID == (int)JobsId.Taxi && (sessionData.RentData != null || sessionData.WorkData.OnWork))                       
            { 
                
                var taxiData = Phone.Taxi.Repository.OrdersList.FirstOrDefault(ol => ol.Driver == player);
                
                if (taxiData != null)
                {
                    selectedOrders.Add(taxiData.Player.Name);
                    selectedOrders.Add(taxiData.Player.Position.X);
                    selectedOrders.Add(taxiData.Player.Position.Y);
                    selectedOrders.Add(taxiData.Player.Position.Z);
                }
            }                                                                                                                                    
            else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NotWorkTaxi), 3000);  
            
            Trigger.ClientEvent(player, "client.phone.taxijob.initSelect", JsonConvert.SerializeObject(selectedOrders), isTake);
        }
        
        
        public static void OnPlayerEnterVehicle(ExtPlayer player, ExtVehicle vehicle, sbyte seatId)
        {
            if (seatId == (int)VehicleSeat.Driver)
                return;
            
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            var vehicleLocalData = vehicle.GetVehicleLocalData();
            if (vehicleLocalData == null || vehicleLocalData.WorkId != JobsId.Taxi)
                return;

            if (vehicleLocalData.WorkDriver != -1)
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GiveTochkaZ), 5000);

                var driverPlayer = Main.GetPlayerByUUID(vehicleLocalData.WorkDriver);
                
                var driverSessionData = driverPlayer.GetSessionData();
                if (driverSessionData != null)
                {
                    var taxiData = Phone.Taxi.Repository.OrdersList.FirstOrDefault(ol => ol.Driver == driverPlayer);

                    if (taxiData != null)
                    {
                        if (taxiData.Player != player)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                                LangFunc.GetText(LangType.Ru, DataName.TaxiNoDriver), 3000);
                            VehicleManager.WarpPlayerOutOfVehicle(player);
                            return;
                        }

                        Phone.Taxi.Repository.OnCancel(player, isNotRemove: false);
                    }

                    driverSessionData.TaxiData.Passager = player;
                    Trigger.ClientEvent(driverPlayer, "client.phone.taxi.openCounter", player.Name, false);
                    Trigger.ClientEvent(driverPlayer, "vehicles.mileage.onUpdate");
                    BattlePass.Repository.UpdateReward(driverPlayer, 65);

                    sessionData.TaxiData.Driver = driverPlayer;
                    Trigger.ClientEvent(player, "client.phone.taxi.openCounter", driverPlayer.Name, true);
                    return;
                }
            }
            
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TaxiNoDriver), 3000);
            VehicleManager.WarpPlayerOutOfVehicle(player);
        }
        
        public static void OnPlayerDisconnect(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            if (sessionData.TaxiData.Driver != null)
            {
                var driver = sessionData.TaxiData.Driver;
                var driverSessionData = driver.GetSessionData();
                if (driverSessionData != null)
                {
                    driverSessionData.TaxiData.Passager = null;
                    Trigger.ClientEvent(driver, "client.phone.taxi.closeCounter");
                    Notify.Send(driver, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PassengerCancel), 5000);
                }
            }
            
            if (characterData.WorkID == (int)JobsId.Taxi)
            {           
                if (sessionData.TaxiData.Passager != null)
                {
                    var target = sessionData.TaxiData.Passager;
                    var targetSessionData = target.GetSessionData();
                    if (targetSessionData != null)
                    {
                        targetSessionData.TaxiData.Driver = null;
                        Trigger.ClientEvent(target, "client.phone.taxi.closeCounter");
                        Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DriverCancel), 10000);
                    }
                }
            }
        }

        public static void OnPlayerExitVehicle(ExtPlayer player, ExtVehicle vehicle)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            var vehicleLocalData = vehicle.GetVehicleLocalData();
            if (vehicleLocalData == null || vehicleLocalData.WorkId != JobsId.Taxi)
                return;
                
            if (sessionData.TaxiData.Driver != null)
            {
                var driver = sessionData.TaxiData.Driver;
                var driverSessionData = driver.GetSessionData();
                if (driverSessionData != null)
                {
                    driverSessionData.TaxiData.Passager = null;
                    Trigger.ClientEvent(driver, "client.phone.taxi.closeCounter");
                    Notify.Send(driver, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PassengerCancel), 5000);
                }
                sessionData.TaxiData.Driver = null;
                Trigger.ClientEvent(player, "client.phone.taxi.closeCounter");
            }
        } 
        
        public static void EndWork(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            if (sessionData.WorkData.OnWork)
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.EndWorkDay), 3000);
                sessionData.WorkData.OnWork = false;
                
                Trigger.ClientEvent(player, "client.phone.taxijob.jobEnd");
                if (sessionData.TaxiData.Passager != null)
                {
                    var target = sessionData.TaxiData.Passager;
                    sessionData.TaxiData.Passager = null;
                    Trigger.ClientEvent(player, "client.phone.taxi.closeCounter");

                    var targetSessionData = target.GetSessionData();

                    if (targetSessionData == null) return;
                    targetSessionData.TaxiData.Driver = null;
                    Trigger.ClientEvent(target, "client.phone.taxi.closeCounter");
                
                    Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TaxiDriverLost), 3000);
                }
            }
        }

        private static int OneMileagePrice = 30;//Цена за милю
        
        public static void TaxiPay(ExtPlayer player, int mileage)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;

            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            var target = sessionData.TaxiData.Passager;
            
            var targetSessionData = target.GetSessionData();
            if (targetSessionData == null) 
                return;

            var targetCharacterData = target.GetCharacterData();
            if (targetCharacterData == null) 
                return;

            var price = mileage * OneMileagePrice;
            
            
            if (sessionData.WorkData.LastClientTime < DateTime.Now)
            {
                if (characterData.JobSkills.ContainsKey((int)JobsId.Taxi))
                {
                    if (characterData.JobSkills[(int)JobsId.Taxi] < 1000)
                        characterData.JobSkills[(int)JobsId.Taxi] += 1;
                }
                else characterData.JobSkills.Add((int)JobsId.Taxi, 1);

                sessionData.WorkData.LastClientTime = DateTime.Now.AddMinutes(5);
            }

            if (Chars.UpdateData.CanIChange(target, price, true) != 255)
            {
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "У игрока не хватает денег для оплаты.", 6000);
                return;
            }
            
            Trigger.ClientEvent(player, "client.phone.taxi.updateCounter", price);
            Trigger.ClientEvent(target, "client.phone.taxi.updateCounter", price);
            BattlePass.Repository.UpdateReward(player, 59);
            BattlePass.Repository.UpdateReward(target, 95);
            BattlePass.Repository.UpdateReward(player, 2);
            BattlePass.Repository.UpdateReward(player, 159);
            
            MoneySystem.Wallet.Change(player, price);
            MoneySystem.Wallet.Change(target, -price);
            GameLog.Money($"player({targetCharacterData.UUID})", $"player({characterData.UUID})", price, $"taxiPay");
            

            if (qMain.GetQuestsLine(player, Zdobich.QuestName) == (int)zdobich_quests.Stage11)
            {
                sessionData.WorkData.PointsCount += price;
                if (sessionData.WorkData.PointsCount < qMain.GetQuestsData(player, Zdobich.QuestName, (int) zdobich_quests.Stage11))
                    sessionData.WorkData.PointsCount = qMain.GetQuestsData(player, Zdobich.QuestName, (int) zdobich_quests.Stage11) + price;
                
                if (sessionData.WorkData.PointsCount >= 500)
                {
                    qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage11, 1, isUpdateHud: true);
                    qMain.UpdateQuestsComplete(player, Zdobich.QuestName, (int) zdobich_quests.Stage11, true);
                    Trigger.SendChatMessage(player, "!{#fc0}" + LangFunc.GetText(LangType.Ru, DataName.QuestPartComplete));
                }
                else
                {
                    qMain.UpdateQuestsData(player, Zdobich.QuestName, (int)zdobich_quests.Stage11, sessionData.WorkData.PointsCount.ToString());
                    //todo translate (было DataName.PointsQuestGot)
                    Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.YouEarnedJob, sessionData.WorkData.PointsCount, 500 - sessionData.WorkData.PointsCount));
                }
            }
        }
    }
}