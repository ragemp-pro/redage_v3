using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using GTANetworkAPI;
using LinqToDB;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Handles;
using NeptuneEvo.Table.Models;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Fractions.Table.Vehicle
{
    public class Repository
    {
                
        public static void GetVehicles(ExtPlayer player)
        {
            var memberFractionData = player.GetFractionMemberData();
            if (memberFractionData == null) 
                return;
            
            var fractionData = Fractions.Manager.GetFractionData(memberFractionData.Id);
            if (fractionData == null) 
                return;
            
            var vehiclesList = new List<List<object>>();

            foreach (var v in fractionData.Vehicles)
            {
                var model = v.Value.model;
                if (fractionData.Id == (int) Models.Fractions.MERRYWEATHER && (model.Equals("SUBMERSIBLE") || model.Equals("THRUSTER"))) continue;
                var vehicleData =
                    NeptuneEvo.Table.Repository.GetVehicles(memberFractionData.Rank, model, v.Key, v.Value.rank);
                
                if (vehicleData == null)
                    continue;
                
                vehiclesList.Add(vehicleData);
            }

            var isVehicleUpdateRank = player.IsFractionAccess(RankToAccess.SetVehicleRank, false);

            Trigger.ClientEvent(player, "client.frac.main.vehicles", isVehicleUpdateRank, JsonConvert.SerializeObject(vehiclesList));
        }
        
        public static void UpdateRank(ExtPlayer player, string number, int rank)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.SetVehicleRank)) return;
                var vehicle = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Fraction, number);
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData == null) 
                    return;
                
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null) 
                    return;
                
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null) 
                    return;

                if (!fractionData.Vehicles.ContainsKey(number)) 
                    return;
                
                var vData = fractionData.Vehicles[number];
                if (vData.rank > memberFractionData.Rank)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantUpToRank), 3000);
                    return;
                }
                if (rank > memberFractionData.Rank)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantUpVehicleRank), 3000);
                    return;
                }
                if (rank < 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantSetNullRank), 3000);
                    return;
                }
                Commands.SetFracVehRank(player, vehicle, rank);
                GetVehicles(player);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static void Evacuation(ExtPlayer player, string number) 
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null) 
                    return;
                
                var fractionData = Fractions.Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null) 
                    return;
                
                var vehicle = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Fraction, number);
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData == null) return;
                
                if (vehicleLocalData.Fraction == memberFractionData.Id && memberFractionData.Rank >= vehicleLocalData.MinRank)
                {
                    if (characterData.Money < 20)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMoneyNeed, 20 - characterData.Money), 3000);
                        return;
                    }
                    if (vehicleLocalData.Occupants.Count >= 1)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarUsed), 3000);
                        return;
                    }
                    if (vehicleLocalData.CanMats || vehicleLocalData.CanDrugs || vehicleLocalData.CanMedKits)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarCantOtognat), 3000);
                        return;
                    }
                    Admin.RespawnFractionCar(vehicle);
                    MoneySystem.Wallet.Change(player, -20);
                    GameLog.Money($"player({characterData.UUID})", $"server", 20, $"fracCarEvac");
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarOtognana), 3000);
                    GetVehicles(player);
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static void Gps(ExtPlayer player, string number)
        {
            try
            {
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null) 
                    return;
                
                var vehicle = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Fraction, number);
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData == null) 
                    return;

                if (vehicleLocalData.Fraction == memberFractionData.Id && memberFractionData.Rank >= vehicleLocalData.MinRank)
                {
                    Trigger.ClientEvent(player, "createWaypoint", vehicle.Position.X, vehicle.Position.Y);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GPSOtmechenoCarFraction), 3000);
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        public static void DefaultRanks(ExtPlayer player)
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

                if (!fractionData.IsLeader(memberFractionData.Rank)) 
                    return;
                
                var fractionVehicles = fractionData.Vehicles;
                
                var vehiclesNumber = new List<string>();
                foreach (var vehicleNumber in fractionVehicles.Keys.ToList())
                {
                    if (!VehicleData.LocalData.Repository.IsVehicleToNumber(VehicleAccess.Fraction, vehicleNumber)) continue;
                    vehiclesNumber.Add(vehicleNumber);
                }

                foreach (string vehicleNumber in vehiclesNumber)
                {
                    if (!fractionVehicles.ContainsKey(vehicleNumber)) continue;
                    var vehicle = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Fraction, vehicleNumber);
                    if (vehicle == null) continue;
                    Commands.SetFracVehRank(player, vehicle, fractionVehicles[vehicleNumber].defaultRank);
                }

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucResetDefaultVehRanks), 4500);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
    }
}