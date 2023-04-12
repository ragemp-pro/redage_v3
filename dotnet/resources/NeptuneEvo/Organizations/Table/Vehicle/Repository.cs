using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using GTANetworkAPI;
using LinqToDB;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Handles;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.Table.Models;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Organizations.Table.Vehicle
{
    public class Repository
    {
        public static void GetVehicles(ExtPlayer player)
        {
            var memberOrganizationData = player.GetOrganizationMemberData();
            if (memberOrganizationData == null) 
                return;
            
            var organizationData = Organizations.Manager.GetOrganizationData(memberOrganizationData.Id);
            if (organizationData == null) 
                return;
            
            var vehiclesList = new List<List<object>>();

            foreach (var v in organizationData.Vehicles)
            {
                var model = v.Value.model;
                var vehicle = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.OrganizationGarage, v.Key);

                var vehicleData = NeptuneEvo.Table.Repository.GetVehicles(memberOrganizationData.Rank, model, v.Key,
                    v.Value.rank, Ticket.IsVehicleTickets(v.Key, VehicleTicketType.Organization), vehicle == null);
                
                if (vehicleData == null)
                    continue;
                
                vehiclesList.Add(vehicleData);
            }

            var isVehicleUpdateRank = player.IsOrganizationAccess(RankToAccess.SetVehicleRank, false);
            var isSellVehicle = player.IsOrganizationAccess(RankToAccess.OrgSellCar, false);

            Trigger.ClientEvent(player, "client.org.main.vehicles", isVehicleUpdateRank, JsonConvert.SerializeObject(vehiclesList), isSellVehicle);
        }
        
        public static void UpdateRank(ExtPlayer player, string number, int rank)
        {
            try
            {
                if (!player.IsOrganizationAccess(RankToAccess.SetVehicleRank)) return;
                var vehicle = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Organization, number);
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData == null) 
                    return;
                
                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberOrganizationData == null) 
                    return;
                
                var organizationData = Manager.GetOrganizationData(memberOrganizationData.Id);
                if (organizationData == null) 
                    return;

                if (!organizationData.Vehicles.ContainsKey(number)) 
                    return;
                
                var vData = organizationData.Vehicles[number];
                if (vData.rank > memberOrganizationData.Rank)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantUpToRank), 3000);
                    return;
                }
                if (rank > memberOrganizationData.Rank)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantUpVehicleRank), 3000);
                    return;
                }
                if (rank < 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantSetNullRank), 3000);
                    return;
                }
                if (vehicleLocalData.Access == VehicleAccess.Organization && vehicleLocalData.Fraction == memberOrganizationData.Id)
                {
                    vehicleLocalData.MinRank = rank;
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SuccessEditRankVehWithPrikol, vData.model, number), 3000);
                }
                vData.rank = rank;
                GetVehicles(player);
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.Orgvehicles
                            .Where(v => v.Number == number)
                            .Set(v => v.Rank, rank)
                            .UpdateAsync();
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
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
                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberOrganizationData == null) 
                    return;
                
                var organizationData = Organizations.Manager.GetOrganizationData(memberOrganizationData.Id);
                if (organizationData == null) 
                    return;
                
                var vehicle = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Organization, number);
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData == null) return;
                
                if (vehicleLocalData.Fraction == memberOrganizationData.Id && memberOrganizationData.Rank >= vehicleLocalData.MinRank)
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
                    if (vehicleLocalData.Access == VehicleAccess.OrganizationGarage)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarNoNeedEvac), 3000);
                        return;
                    }
                    if (Ticket.IsVehicleTickets(number, VehicleTicketType.Organization))
                        return;
                    
                    var petrol = vehicleLocalData.Petrol;
                    var dirt = 0f;
                    
                    var vehicleStateData = vehicle.GetVehicleLocalStateData();
                    if (vehicleStateData != null)
                        dirt = vehicleStateData.Dirt;
                    
                    VehicleStreaming.DeleteVehicle(vehicle);
                    var organizationVehicle = organizationData.Vehicles[number];

                    vehicle = VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey(organizationVehicle.model), Manager.GaragePositions[organizationVehicle.garageId], Manager.GarageRotations[organizationVehicle.garageId], 0, 0, number, dimension: organizationData.GetDimension(), locked: true, acc: VehicleAccess.OrganizationGarage, fr: organizationData.Id, minrank: organizationVehicle.rank, petrol: petrol, dirt: dirt);
                    VehicleManager.OrgApplyCustomization(vehicle, organizationVehicle.customization);
                    MoneySystem.Wallet.Change(player, -20);
                    GameLog.Money($"player({characterData.UUID})", $"server", 20, $"orgCarEvac");
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
                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberOrganizationData == null) 
                    return;
                
                var vehicle = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Organization, number);
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData == null) 
                    return;
                
                if (vehicleLocalData.Access == VehicleAccess.OrganizationGarage)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarNoNeedEvac), 3000);
                    return;
                }
                
                if (vehicleLocalData.Fraction == memberOrganizationData.Id && memberOrganizationData.Rank >= vehicleLocalData.MinRank)
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
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;

                if (!organizationData.IsLeader(player.GetUUID())) 
                    return;
                
                var organizationVehicles = organizationData.Vehicles;
                
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.Orgvehicles
                            .Where(v => v.Organization == organizationData.Id)
                            .Set(v => v.Rank, 0)
                            .UpdateAsync();
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
                var vehiclesNumber = new List<string>();
                foreach (var vehicleData in organizationVehicles)
                {
                    if (!VehicleData.LocalData.Repository.IsVehicleToNumber(VehicleAccess.Organization, vehicleData.Key)) continue;
                    vehiclesNumber.Add(vehicleData.Key);
                }

                foreach (string vehicleNumber in vehiclesNumber)
                {
                    if (!organizationVehicles.ContainsKey(vehicleNumber)) continue;
                    var vehicle = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Organization, vehicleNumber);
                    var vehicleLocalData = vehicle.GetVehicleLocalData();
                    if (vehicleLocalData == null) continue;
                    
                    if (vehicleLocalData.Access == VehicleAccess.Organization && vehicleLocalData.Fraction == organizationData.Id && vehicle.NumberPlate == vehicleNumber)
                        vehicleLocalData.MinRank = 0;
                    
                    organizationVehicles[vehicleNumber].rank = 0;
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