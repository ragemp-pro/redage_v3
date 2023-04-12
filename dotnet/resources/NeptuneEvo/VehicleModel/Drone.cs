using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions;
using NeptuneEvo.Functions;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using Localization;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Table.Models;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;

namespace NeptuneEvo.VehicleModel
{
    class Drone : Script
    {
        private static readonly nLog Log = new nLog("VehicleModel.Drone");
        [Command(AdminCommands.Drone)]
        public void CreateDrone(ExtPlayer player)
        {
            if (!player.IsCharacterData()) return;
            if (!CommandsAccess.CanUseCmd(player, AdminCommands.Drone)) return;
            var number = VehicleManager.GenerateNumber(VehicleAccess.Admin, "DRONE-");
            var vehicle = VehicleStreaming.CreateVehicle((uint)VehicleHash.Rcbandito, player.Position, player.Rotation.Z, 0, 0, acc: VehicleAccess.Admin, numberPlate: number, by: player.Name, petrol: 9999, engine: true);
            vehicle.SetSharedData("attachmentsData", JsonConvert.SerializeObject(new List<uint>() { NAPI.Util.GetHashKey("spec1") }));
            vehicle.SetSharedData("isDrone", true);

            Trigger.ClientEvent(player, "setIntoVehicle", vehicle, VehicleSeat.Driver - 1);
            //NAPI.Entity.AttachEntityToEntity(model, client, "IK_R_Hand", new Vector3(), new Vector3());
        }
        private static Dictionary<int, (string, int)> FractionDroneData = new Dictionary<int, (string, int)>()
        {
            {(int)Fractions.Models.Fractions.CITY, ("spec2", 2) },
            {(int)Fractions.Models.Fractions.POLICE, ("spec1", 3) },
            {(int)Fractions.Models.Fractions.FIB, ("spec1", 3) },
            {(int)Fractions.Models.Fractions.LSNEWS, ("spec2", 2) },
            {(int)Fractions.Models.Fractions.EMS, ("spec2", 2) },
            {(int)Fractions.Models.Fractions.ARMY, ("spec1", 2) },
            {(int)Fractions.Models.Fractions.SHERIFF, ("spec1", 3) },
        };

        [RemoteEvent("server.table.dron")]
        public static void CreateDroneFraction(ExtPlayer player)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.Dron)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var fracId = player.GetFractionId();
                if (!FractionDroneData.ContainsKey(fracId)) return;
                if (Configs.FractionDrones[fracId].Count >= FractionDroneData[fracId].Item2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Only2DronesAvaible, FractionDroneData[fracId].Item2), 5000);
                    return;
                }
                var vehicle = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicle == null || vehicleLocalData == null || vehicle.Model == (uint)VehicleHash.Rcbandito)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMustBeInVeh), 3000);
                    return;
                }
                if (vehicleLocalData.Access == VehicleAccess.Fraction && vehicleLocalData.Fraction == fracId)
                {
                    var number = VehicleManager.GenerateNumber(VehicleAccess.Fraction, "DRONE-");
                    var drone = VehicleStreaming.CreateVehicle((uint)VehicleHash.Rcbandito, vehicle.Position + new Vector3(2f, 2f, 0), player.Rotation.Z, 0, 0, numberPlate: number, acc: VehicleAccess.Fraction, fr: fracId, petrol: 9999, minrank: 0);
                    drone.SetSharedData("attachmentsData", JsonConvert.SerializeObject(new List<uint>() { NAPI.Util.GetHashKey(FractionDroneData[fracId].Item1) }));
                    drone.SetSharedData("isDrone", true);

                    Configs.FractionDrones[fracId].Add(drone, vehicle.NumberPlate);
                    characterData.ExteriorPos = vehicle.Position;

                    Trigger.ClientEvent(player, "setIntoVehicle", drone, VehicleSeat.Driver - 1);
                    Trigger.ClientEvent(player, "client.dron.init", vehicle.Position.X, vehicle.Position.Y, vehicle.Position.Z);
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DroneControls), 10000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"CreateDroneFraction Exception: {e.ToString()}");
            }
        }

        [ServerEvent(Event.PlayerExitVehicle)]
        public void Event_OnPlayerExitVehicle(ExtPlayer player, ExtVehicle vehicleDrone)
        {
            try
            {
                var vehicleLocalData = vehicleDrone.GetVehicleLocalData();
                if (vehicleLocalData == null) return;
                if (vehicleLocalData.Access == VehicleAccess.Fraction && Configs.FractionDrones.ContainsKey(vehicleLocalData.Fraction) && Configs.FractionDrones[vehicleLocalData.Fraction].ContainsKey(vehicleDrone))
                {
                    string number = Configs.FractionDrones[vehicleLocalData.Fraction][vehicleDrone];

                    VehicleStreaming.DeleteVehicle(vehicleDrone);
                    var characterData = player.GetCharacterData();
                    if (characterData == null) return;
                    var vehicle = VehicleData.LocalData.Repository.GetVehicleToNumber(vehicleLocalData.Access, number);
                    if (vehicle == null) return;

                    List<int> emptySlots = new List<int>
                    {
                        (int)VehicleSeat.Driver,
                        (int)VehicleSeat.RightFront,
                        (int)VehicleSeat.LeftRear,
                    };
                    foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                    {
                        if (!foreachPlayer.IsCharacterData() || !foreachPlayer.IsInVehicle || foreachPlayer.Vehicle != vehicle) continue;
                        if (emptySlots.Contains(foreachPlayer.VehicleSeat)) emptySlots.Remove(foreachPlayer.VehicleSeat);
                    }
                    characterData.ExteriorPos = new Vector3();
                    if (emptySlots.Count == 0)
                    {
                        player.Position = new Vector3(vehicle.Position.X + 3f, vehicle.Position.Y + 3f, vehicle.Position.Z);
                        return;
                    }
                    player.SetIntoVehicle(vehicle, emptySlots[0]);
                }
                else if (vehicleLocalData.Access == VehicleAccess.Admin && vehicleDrone.Model == NAPI.Util.GetHashKey("rcbandito") && vehicleDrone.HasSharedData("isDrone"))
                {
                    VehicleStreaming.DeleteVehicle(vehicleDrone);
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DroneDeleted), 3000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Event_OnPlayerExitVehicle Exception: {e.ToString()}");
            }
        }
    }
}
