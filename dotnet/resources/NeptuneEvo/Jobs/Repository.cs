using GTANetworkAPI;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Handles;
using NeptuneEvo.Jobs.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Players.Models;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;
using Redage.SDK;

namespace NeptuneEvo.Jobs
{
    public class Repository
    {
        public static void OnPlayerEnterVehicle(ExtPlayer player, ExtVehicle vehicle, sbyte seatId)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            if (seatId != (int)VehicleSeat.Driver)
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            var vehicleLocalData = vehicle.GetVehicleLocalData();
            if (vehicleLocalData == null || vehicleLocalData.Access != VehicleAccess.Work)
                return;

            if (vehicleLocalData.WorkDriver != characterData.UUID)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehBusy), 3000);
                VehicleManager.WarpPlayerOutOfVehicle(player);
                return;
            }
            
            if (sessionData.TimersData.WorkExitTimer != null)
            {
                Timers.Stop(sessionData.TimersData.WorkExitTimer);
                sessionData.TimersData.WorkExitTimer = null;
                sessionData.WorkData.TimerCount = 0;
            }
        }

        public static void OnPlayerExitVehicle(ExtPlayer player, ExtVehicle vehicle)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            var vehicleLocalData = vehicle.GetVehicleLocalData();
            if (vehicleLocalData == null || vehicleLocalData.WorkId == JobsId.None || vehicleLocalData.WorkDriver != characterData.UUID)
                return;

            Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.FjobNotify));
            Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehjobNotify), 8000);
   
            if (sessionData.TimersData.WorkExitTimer != null)
            {
                Timers.Stop(sessionData.TimersData.WorkExitTimer);
                sessionData.TimersData.WorkExitTimer = null;
            }
            
            sessionData.WorkData.TimerCount = 0;
            sessionData.TimersData.WorkExitTimer = Timers.Start(1000, () => timer_playerExitWorkVehicle(player, vehicle));
        }
        private static void timer_playerExitWorkVehicle(ExtPlayer player, ExtVehicle vehicle)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            if (sessionData.TimersData.WorkExitTimer == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            var vehicleLocalData = vehicle.GetVehicleLocalData();
            if (vehicleLocalData == null || vehicleLocalData.WorkId == JobsId.None || vehicleLocalData.WorkDriver != characterData.UUID)
                return;
            
            /*if (player.Vehicle == vehicle)
            {
                Timers.Stop(sessionData.TimersData.WorkExitTimer);
                sessionData.TimersData.WorkExitTimer = null;
                return;
            }*/
            if (sessionData.WorkData.TimerCount++ >= 600)
            {
                if (vehicleLocalData.SpecialTaxiVeh)
                {
                    vehicleLocalData.WorkDriver = -1;
                    vehicleLocalData.WorkId = JobsId.None;
                    vehicleLocalData.SpecialTaxiVeh = false;
                }
                
                Trigger.SetMainTask(() => JobEnd(player));
            }
        }

        public static void JobEnd(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            if (!sessionData.WorkData.OnWork)
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            sessionData.WorkData.TimerCount = 0;
            
            if (sessionData.TimersData.WorkExitTimer != null)
            {
                Timers.Stop(sessionData.TimersData.WorkExitTimer);
                sessionData.TimersData.WorkExitTimer = null;
            }

            switch ((JobsId) characterData.WorkID)
            {
                case JobsId.Electrician:
                    Electrician.EndWork(player);
                    break;
                case JobsId.Lawnmower:
                    Lawnmower.EndWork(player);
                    break;
                case JobsId.Postman:
                    Gopostal.EndWork(player);
                    break;
                case JobsId.Taxi:
                    Players.Phone.Taxi.Orders.Repository.EndWork(player);
                    break;
                case JobsId.Bus:
                    Bus.EndWork(player);
                    break;
                case JobsId.CarMechanic:
                    Players.Phone.Mechanic.Orders.Repository.EndWork(player);
                    break;
                case JobsId.Trucker:
                    Truckers.EndWork(player);
                    break;
                case JobsId.CashCollector:
                    Collector.EndWork(player);
                    break;
                
            }

            sessionData.WorkData = new WorkData();
            
            if (sessionData.RentData != null)
            {
                var vehicleRent = sessionData.RentData.Vehicle;
                
                var vehicleLocalData = vehicleRent.GetVehicleLocalData();
                if (vehicleLocalData == null || vehicleLocalData.Access != VehicleAccess.Work)
                    return;
                
                Rentcar.OnReturnVehicle(player);
            }
        }
    }
}