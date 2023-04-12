using System.Collections.Generic;
using System.Linq;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Handles;
using NeptuneEvo.Jobs.Models;
using NeptuneEvo.Players.Phone.Messages.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Players.Phone.Mechanic.Orders
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
            
            var orders = new List<List<object>>();
            
            foreach (var order in Phone.Mechanic.Repository.OrdersList)
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
            
            sessionData.WorkData.OnWork = true;
            
            Trigger.ClientEvent(player, "client.phone.mechjob.init", JsonConvert.SerializeObject(orders));
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
                
                Trigger.ClientEvent(player, "client.phone.mechjob.jobEnd");
                if (sessionData.WorkData.Player != null)
                {
                    var target = sessionData.WorkData.Player;
                    var targetSessionData = target.GetSessionData();
                    if (targetSessionData == null) return;
                    Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AutomechLeave), 3000);
                }
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
            
            if (characterData.WorkID == (int)JobsId.CarMechanic && (sessionData.RentData != null || sessionData.WorkData.OnWork))                       
            { 
                
                var taxiData = Phone.Mechanic.Repository.OrdersList.FirstOrDefault(ol => ol.Driver == player);
                
                if (taxiData != null)
                {
                    selectedOrders.Add(taxiData.Player.Name);
                    selectedOrders.Add(taxiData.Player.Position.X);
                    selectedOrders.Add(taxiData.Player.Position.Y);
                    selectedOrders.Add(taxiData.Player.Position.Z);
                }
                
                
            }                                                                                                                                    
            else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NotWorkTaxi), 3000);  
            
            Trigger.ClientEvent(player, "client.phone.mechjob.initSelect", JsonConvert.SerializeObject(selectedOrders), isTake);
        }
        public static void OnTake(ExtPlayer player, int id)
        {
            var sessionData = player.GetSessionData();       
            if (sessionData == null) 
                return;
            
            var characterData = player.GetCharacterData();         
            if (characterData == null) 
                return;

            var vehicle = (ExtVehicle) player.Vehicle;
            
            if (vehicle != null && characterData.WorkID == (int) JobsId.CarMechanic &&
                (sessionData.RentData != null || sessionData.WorkData.OnWork))
            {
                
                var mechanicData = Phone.Mechanic.Repository.OrdersList.FirstOrDefault(ol => ol.PlayerUUID == id);

                if (mechanicData != null && mechanicData.Player.IsCharacterData())
                {
                    mechanicData.Driver = player;

                    GetSelect(player, isTake: true);
                    Trigger.ClientEvent(mechanicData.Player, "client.phone.mech.updateOrder", sessionData.Name, vehicle.NumberPlate, player.Position.X, player.Position.Y, player.Position.Z);
                    
                    mechanicData.DriverTimer = Timers.Start(2500, () => OnUpdatePos(player, mechanicData.Player), isnapitask: true);
                    
                    foreach (var foreachPlayer in Players.Phone.Mechanic.Repository.GetListMechanic())
                    {
                        if (!foreachPlayer.IsCharacterData())
                            continue;

                        Trigger.ClientEvent(foreachPlayer, "client.phone.mechjob.dell", mechanicData.PlayerUUID);
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
                //Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, "Механик подъехал.", 8000);
                Trigger.ClientEvent(target, "phone.notify", (int) DefaultNumber.Mech, $"Механик подъехал!", 8); 
                BattlePass.Repository.UpdateReward(player, 24);
                Phone.Mechanic.Repository.OnCancel(target, isNotRemove: false);
                return;
            }
            
            var vehicle = (ExtVehicle) player.Vehicle;

            if (vehicle != null && characterData.WorkID == (int) JobsId.CarMechanic &&
                (sessionData.RentData != null || sessionData.WorkData.OnWork))
            {
                Trigger.ClientEvent(target, "client.phone.mech.updatePosOrder", player.Position.X, player.Position.Y, player.Position.Z);
            }
        }
        
    }
}