using GTANetworkAPI;
using NeptuneEvo.VehicleData.LocalData.Models;

namespace NeptuneEvo.Handles
{
    public class ExtVehicle : Vehicle
    {
        public ExtVehicle(NetHandle handle) : base(handle)
        {
        }  
        
        public VehicleLocalData VehicleLocalData;
        public void SetVehicleLocalData(VehicleLocalData vehicleLocalData)
        {
            VehicleLocalData = vehicleLocalData;
        }   
        
        public VehicleLocalStateData VehicleLocalStateData;
        public void SetVehicleLocalStateData(VehicleLocalStateData vehicleLocalStateData)
        {
            VehicleLocalStateData = vehicleLocalStateData;
        }
    }
}