using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Core;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;
using Newtonsoft.Json;

namespace NeptuneEvo.VehicleData.Data
{
    public static class Repository
    {
        public static void SavePosition(this ExtVehicle vehicle)
        {
            if (vehicle == null)
                return;
            var vehicleLocalData = vehicle.GetVehicleLocalData();
            if (vehicleLocalData == null)
                return;
            if (vehicleLocalData.Access != VehicleAccess.Garage && vehicleLocalData.Access != VehicleAccess.Personal)
                return;
            
            string number = vehicleLocalData.NumberPlate;
            
            var vehicleData = VehicleManager.GetVehicleToNumber(number);
            if (vehicleData == null)
                return;
            
            var vehicleStateData = vehicle.GetVehicleLocalStateData();
            
            if (vehicleLocalData.Access == VehicleAccess.Garage || vehicleData.Health == 0)
            {
                vehicleData.Position = null;
                vehicleData.Rotation = null;
                //if (vehicleData.Health == 0) Chars.Repository.RemoveAll(VehicleManager.GetVehicleToInventory(number));
            }
            else
            {
                vehicleData.Position = JsonConvert.SerializeObject(vehicle.Position);
                vehicleData.Rotation = JsonConvert.SerializeObject(vehicle.Rotation);
                            
                if (vehicleStateData != null) 
                    vehicleData.Dirt = vehicleStateData.Dirt;
            }
        }
    }
}