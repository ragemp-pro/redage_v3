using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.Jobs.Models;
using NeptuneEvo.Players.Phone.Taxi.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Players.Phone.Taxi
{
    public class Repository
    {
        public static List<TaxiData> OrdersList = new List<TaxiData>();

        public static List<ExtPlayer> GetListTaxi()
        {
            var players = new List<ExtPlayer>();
            
            var vehiclesLocalData = RAGE.Entities.Vehicles.All.Cast<ExtVehicle>()
                .Where(v => v.VehicleLocalData != null)
                .Where(v => v.VehicleLocalData.WorkDriver != -1)
                .Where(v => /*v.VehicleLocalData.OnWork && */v.VehicleLocalData.WorkId == JobsId.Taxi)
                .Select(d => d.VehicleLocalData)
                .ToList();

            foreach (var vehData in vehiclesLocalData)
            {
                if (vehData == null)
                    continue;

                var foreachPlayer = Main.GetPlayerByUUID(vehData.WorkDriver);
                
                if (foreachPlayer.IsCharacterData())
                    players.Add(foreachPlayer);
            }

            return players;
        }
        
        public static void OnOrder(ExtPlayer player)
        {
            
            var sessionData = player.GetSessionData();       
            if (sessionData == null) 
                return;
            
            if (!FunctionsAccess.IsWorking("phonetaxi"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                return;
            }
            
            if (sessionData.TaxiData.Driver != null || sessionData.TaxiData.Passager != null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            if (OrdersList.Any(ol => ol.Player == player || ol.Driver == player))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyTaxiOrdered), 3000);
                return;
            }
            
            var taxiCount = 0;
            foreach (var foreachPlayer in GetListTaxi ())
            {
                if (!foreachPlayer.IsCharacterData()) 
                    continue;

                taxiCount++;
                Trigger.ClientEvent(foreachPlayer, "client.phone.taxijob.add", characterData.UUID, player.Name, player.Position.X, player.Position.Y, player.Position.Z);
                //NAPI.Chat.SendChatMessageToPlayer(foreachPlayer, $"~g~[ДИСПЕТЧЕР]: ~w~Игрок ({player.Value}) вызвал такси ~y~({player.Position.DistanceTo(foreachPlayer.Position)}м)~w~. Откройте телефон что бы принять вызов");
            }
            
            if (taxiCount > 0)
            {
                //sessionData.WorkData.IsCallTaxi = true;

                var taxiData = new TaxiData();
                taxiData.Player = player;
                taxiData.PlayerUUID = characterData.UUID;
                taxiData.Driver = null;
                taxiData.DriverTimer = null;
                
                OrdersList.Add(taxiData);
                
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WaitForTaxi, taxiCount), 3000);
                Trigger.ClientEvent(player, "client.phone.taxi.successOrder");
                
                //Trigger.ClientEvent(player, "client.phone.taxi.");
                BattlePass.Repository.UpdateReward(player, 62);
                
            }
            else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoTaxiHere), 3000);

        }

        public static void OnCancel(ExtPlayer player, bool isDisconnect = false, bool isNotRemove = true)//Отмена для таксиста или вызывающего
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            var taxiData = OrdersList.FirstOrDefault(ol => ol.Player == player || ol.Driver == player);

            if (taxiData == null)
            {
                if (!isDisconnect) 
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouDoesntCallTaxi), 3000);
                return;
            }
            
            var target = taxiData.Player == player ? taxiData.Driver : taxiData.Player;

            var isDriver = taxiData.Driver == player;

            if (taxiData.DriverTimer != null)
                Timers.Stop(taxiData.DriverTimer);
            
            if (!isNotRemove)
                OrdersList.Remove(taxiData);
            
          
            if (isDriver)
            {
                if (isNotRemove)
                {
                    taxiData.Driver = null;
                    taxiData.DriverTimer = null;
                    
                    if (!isDisconnect)
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TripCanceled), 3000);
                }

                if (!isDisconnect)
                    Trigger.ClientEvent(player, "client.phone.taxijob.successCancel");
            }
            else
            {
                if (isNotRemove)
                {
                    OrdersList.Remove(taxiData);

                    foreach (var foreachPlayer in GetListTaxi())
                    {
                        if (!foreachPlayer.IsCharacterData())
                            continue;

                        Trigger.ClientEvent(foreachPlayer, "client.phone.taxijob.dell", characterData.UUID);
                    }


                    if (!isDisconnect) 
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCancelTrip), 3000);
                }

                if (!isDisconnect) 
                    Trigger.ClientEvent(player, "client.phone.taxi.successCancel");
            }

            if (target != null)
            {
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) 
                    return;

                if (isDriver)
                {
                    if (isNotRemove)
                    {
                        foreach (var foreachPlayer in GetListTaxi())
                        {
                            if (!foreachPlayer.IsCharacterData())
                                continue;

                            Trigger.ClientEvent(foreachPlayer, "client.phone.taxijob.add", targetCharacterData.UUID, target.Name, target.Position.X, target.Position.Y, target.Position.Z);
                        }
                        
                        Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter,
                            LangFunc.GetText(LangType.Ru, DataName.DriverCancelTrip), 3000);
                        
                        Trigger.ClientEvent(target, "client.phone.taxi.successOrder");
                    }
                    else
                        Trigger.ClientEvent(target, "client.phone.taxi.successCancel");
                }
                else
                {
                    if (isNotRemove)
                        Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PassengerCancel), 3000);
                    
                    Trigger.ClientEvent(target, "client.phone.taxijob.successCancel");
                }
            }
        }
        
        public static void OnPlayerDisconnect(ExtPlayer player)
        {
            OnCancel(player, isDisconnect: true);
        }
    }
}