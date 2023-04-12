using System;
using System.Collections.Generic;
using System.Linq;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.Jobs.Models;
using NeptuneEvo.Players.Phone.Mechanic.Models;
using Redage.SDK;

namespace NeptuneEvo.Players.Phone.Mechanic
{
    public class Repository
    {
        
        public static List<MechanicData> OrdersList = new List<MechanicData>();
        
        public static List<ExtPlayer> GetListMechanic()
        {
            var players = new List<ExtPlayer>();
            
            /*var vehiclesLocalData = RAGE.Entities.Vehicles.All.Cast<ExtVehicle>()
                .Where(v => v.VehicleLocalData != null)
                .Where(v => v.VehicleLocalData.WorkDriver != -1)
                .Where(v => v.VehicleLocalData.WorkId == (int)JobsId.CarMechanic)
                .Select(d => d.VehicleLocalData)
                .ToList();

            foreach (var vehData in vehiclesLocalData)
            {
                if (vehData == null)
                    continue;

                var foreachPlayer = Main.GetPlayerByUUID(vehData.WorkDriver);
                
                if (foreachPlayer.IsCharacterData())
                    players.Add(foreachPlayer);
            }*/
            foreach (var foreachPlayer in  Character.Repository.GetPlayers())
            {
                var foreachSessionData = foreachPlayer.GetSessionData(); 
                if (foreachSessionData == null) continue; 
                var foreachCharacterData = foreachPlayer.GetCharacterData(); 
                if (foreachCharacterData == null) continue; 
                if (foreachCharacterData.WorkID == (int)JobsId.CarMechanic && foreachSessionData.WorkData.OnWork) 
                    players.Add(foreachPlayer);
            }
            

            return players;
        }
        
        public static void OnOrder(ExtPlayer player)
        {
            
            var sessionData = player.GetSessionData();       
            if (sessionData == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            if (!FunctionsAccess.IsWorking("phonemechanic"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                return;
            }
            
            if (OrdersList.Any(ol => ol.Player == player || ol.Driver == player))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyCallMechanik), 3000);
                return;
            }
            
            var mechanicCount = 0;
            foreach (var foreachPlayer in GetListMechanic ())
            {
                if (!foreachPlayer.IsCharacterData()) 
                    continue;

                mechanicCount++;
                Trigger.ClientEvent(foreachPlayer, "client.phone.mechjob.add", characterData.UUID, player.Name, player.Position.X, player.Position.Y, player.Position.Z);
                //NAPI.Chat.SendChatMessageToPlayer(foreachPlayer, $"~g~[ДИСПЕТЧЕР]: ~w~Игрок ({player.Value}) вызвал такси ~y~({player.Position.DistanceTo(foreachPlayer.Position)}м)~w~. Откройте телефон что бы принять вызов");
            }
            
            if (mechanicCount > 0)
            {
                //sessionData.WorkData.IsCallTaxi = true;

                var mechanicData = new MechanicData();
                mechanicData.Player = player;
                mechanicData.PlayerUUID = characterData.UUID;
                mechanicData.Driver = null;
                mechanicData.DriverTimer = null;
                
                OrdersList.Add(mechanicData);
                
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WaitMechanikCall, mechanicCount), 3000);
                Trigger.ClientEvent(player, "client.phone.mech.successOrder");
                
                //Trigger.ClientEvent(player, "client.phone.mech.");
                BattlePass.Repository.UpdateReward(player, 65);
                
            }
            else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMechanikNear), 3000);

        }

        public static void OnCancel(ExtPlayer player, bool isDisconnect = false, bool isNotRemove = true)//Отмена для таксиста или вызывающего
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            var mechanicData = OrdersList.FirstOrDefault(ol => ol.Player == player || ol.Driver == player);

            if (mechanicData == null)
            {
                if (!isDisconnect) 
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не вызывали автомеханика", 3000);
                
                return;
            }
            
            var target = mechanicData.Player == player ? mechanicData.Driver : mechanicData.Player;

            var isDriver = mechanicData.Driver == player;

            if (mechanicData.DriverTimer != null)
                Timers.Stop(mechanicData.DriverTimer);
            
            if (!isNotRemove)
                OrdersList.Remove(mechanicData);
      
            if (isDriver)
            {
                if (isNotRemove)
                {
                    mechanicData.Driver = null;
                    mechanicData.DriverTimer = null;
                    
                    if (!isDisconnect) 
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TripCanceled), 3000);
                }

                if (!isDisconnect)
                    Trigger.ClientEvent(player, "client.phone.mechjob.successCancel");
            }
            else
            {
                if (isNotRemove)
                {
                    OrdersList.Remove(mechanicData);

                    foreach (var foreachPlayer in GetListMechanic())
                    {
                        if (!foreachPlayer.IsCharacterData())
                            continue;

                        Trigger.ClientEvent(foreachPlayer, "client.phone.mechjob.dell", characterData.UUID);
                    }


                    if (!isDisconnect)
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCancelTripMech), 3000);
                }

                if (!isDisconnect)
                    Trigger.ClientEvent(player, "client.phone.mech.successCancel");
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
                        foreach (var foreachPlayer in GetListMechanic())
                        {
                            if (!foreachPlayer.IsCharacterData())
                                continue;

                            Trigger.ClientEvent(foreachPlayer, "client.phone.mechjob.add", targetCharacterData.UUID, target.Name, target.Position.X, target.Position.Y, target.Position.Z);
                        }
                        
                        Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DriverCancelTrip), 3000);
                        
                        Trigger.ClientEvent(target, "client.phone.mech.successOrder");
                    } else
                        Trigger.ClientEvent(target, "client.phone.mech.successCancel");
                }
                else
                {
                    if (isNotRemove)
                        Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PassengerCancel), 3000);
                    
                    Trigger.ClientEvent(target, "client.phone.mechjob.successCancel");
                }
            }
        }
        
        public static void OnPlayerDisconnect(ExtPlayer player)
        {
            OnCancel(player, isDisconnect: true);
        }
    }
}