using GTANetworkAPI;
using NeptuneEvo.Handles;
using System.Collections.Generic;
using NeptuneEvo.GUI;
using System;
using NeptuneEvo.Core;
using Redage.SDK;
using System.Linq;
using Localization;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Jobs.Models;
using NeptuneEvo.Quests;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;

namespace NeptuneEvo.Jobs
{
    class Taxi : Script
    {        
        private static readonly nLog Log = new nLog("Jobs.Taxi");

        
        /*[Command("sjob")]
        public static void CMD_sjob(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (characterData.WorkID != (int)JobsId.Taxi) return;

                if (!player.IsInVehicle)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMustBeInVeh), 3000);
                    return;
                }

                if (sessionData.WorkData.OnWork)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyStartedWorkDay), 3000);
                    return;
                }

                (byte, float) jobLevelInfo = characterData.JobSkills.ContainsKey((int)JobsId.Taxi) ? Main.GetPlayerJobLevelBonus((int)JobsId.Taxi, characterData.JobSkills[(int)JobsId.Taxi]) : (0, 1);
                if (Main.ServerNumber != 0 && jobLevelInfo.Item1 < 5)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Function5lvl), 3000);
                    return;
                }

                var vehicle = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    vehicleLocalData.WorkDriver = characterData.UUID;
                    //vehicleLocalData.Access = VehicleAccess.WorkId;
                    vehicleLocalData.WorkId = (int)JobsId.Taxi;
                    vehicleLocalData.SpecialTaxiVeh = true;
                    
                    Players.Phone.Taxi.Orders.Repository.StartWork(player);
                    
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.StartWorkDay), 3000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_sjob Exception: {e.ToString()}");
            }
        }*/
    }
}
