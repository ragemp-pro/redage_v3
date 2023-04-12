using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Organizations.Table.Vehicle
{
    public class Events : Script
    {
        [RemoteEvent("server.org.main.vehiclesLoad")]
        public void VehiclesLoad(ExtPlayer player) => 
            Repository.GetVehicles(player);
        
        [RemoteEvent("server.org.main.evacuation")]
        public void Evacuation(ExtPlayer player, string number) => 
            Repository.Evacuation(player, number);
        
        [RemoteEvent("server.org.main.gps")]
        public void Gps(ExtPlayer player, string number) => 
            Repository.Gps(player, number);
        
        [RemoteEvent("server.org.main.updateVehicleRank")]
        public void UpdateRank(ExtPlayer player, string number, int rank) => 
            Repository.UpdateRank(player, number, rank);
        
        [RemoteEvent("server.org.main.sellCar")]
        public void SellCar(ExtPlayer player, string number) => 
            Organizations.Manager.DeleteVehicle(player, number, false);
    }
}