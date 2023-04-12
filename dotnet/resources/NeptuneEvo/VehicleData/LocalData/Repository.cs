using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NeptuneEvo.Handles;
using NeptuneEvo.VehicleData.LocalData.Models;

namespace NeptuneEvo.VehicleData.LocalData
{
    public static class Repository
    {
        public static bool IsVehicleLocalData(this ExtVehicle vehicle)
        {
            if (vehicle is null) return false;

            return vehicle.VehicleLocalData != null;
        }
        public static VehicleLocalData GetVehicleLocalData(this ExtVehicle vehicle)
        {
            if (vehicle != null) return vehicle.VehicleLocalData;

            return null;
        }
        public static ConcurrentDictionary<VehicleAccess, ConcurrentDictionary<string, ExtVehicle>> VehicleNumberToHandle = new ConcurrentDictionary<VehicleAccess, ConcurrentDictionary<string, ExtVehicle>>();

        public static void Init()
        {
            foreach (VehicleAccess vehicleAccess in Enum.GetValues(typeof(VehicleAccess)))
            {
                VehicleNumberToHandle[vehicleAccess] = new ConcurrentDictionary<string, ExtVehicle>();
            }
        }
        
        public static VehicleLocalData GetVehicleLocalDataToNumber(VehicleAccess vehicleAccess, string number)
        {
            ExtVehicle vehicle = GetVehicleToNumber(vehicleAccess, number);
            if (vehicle != null) return vehicle.VehicleLocalData;
            return null;
        }
        public static ExtVehicle GetVehicleToNumber(VehicleAccess vehicleAccess, string number)
        {
            List<VehicleAccess> vehiclesAccess = new List<VehicleAccess>();
            
            vehiclesAccess.Add(vehicleAccess);
            if (vehicleAccess == VehicleAccess.Personal) vehiclesAccess.Add(VehicleAccess.Garage);
            if (vehicleAccess == VehicleAccess.Organization) vehiclesAccess.Add(VehicleAccess.OrganizationGarage);

            foreach (VehicleAccess access in vehiclesAccess)
            {
                if (VehicleNumberToHandle[access].ContainsKey(number)) return VehicleNumberToHandle[access][number];
            }

            return null;
        }
        public static bool IsVehicleToNumber(VehicleAccess vehicleAccess, string number)
        {
            if (GetVehicleToNumber(vehicleAccess, number) == null) return false;
            return true;
        }
        public static VehicleLocalStateData GetVehicleLocalStateData(this ExtVehicle vehicle)
        {
            if (vehicle != null) return vehicle.VehicleLocalStateData;

            return null;
        }
        
        public static void Delete(VehicleAccess vehicleAccess, string number)
        {
            List<VehicleAccess> vehiclesAccess = new List<VehicleAccess>();
            
            vehiclesAccess.Add(vehicleAccess);
            if (vehicleAccess == VehicleAccess.Personal) vehiclesAccess.Add(VehicleAccess.Garage);
            if (vehicleAccess == VehicleAccess.Organization) vehiclesAccess.Add(VehicleAccess.OrganizationGarage);

            foreach (VehicleAccess access in vehiclesAccess)
            {
                if (VehicleNumberToHandle[access].ContainsKey(number)) VehicleNumberToHandle[access].TryRemove(number, out _);
            }
        }
    }
}