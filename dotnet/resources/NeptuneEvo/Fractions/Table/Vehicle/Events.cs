using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Fractions.Table.Vehicle
{
    public class Events : Script
    {
        [RemoteEvent("server.frac.main.vehiclesLoad")]
        public void VehiclesLoad(ExtPlayer player) => 
            Repository.GetVehicles(player);
        
        [RemoteEvent("server.frac.main.evacuation")]
        public void Evacuation(ExtPlayer player, string number) => 
            Repository.Evacuation(player, number);
        
        [RemoteEvent("server.frac.main.gps")]
        public void Gps(ExtPlayer player, string number) => 
            Repository.Gps(player, number);
        
        [RemoteEvent("server.frac.main.updateVehicleRank")]
        public void UpdateRank(ExtPlayer player, string number, int rank) => 
            Repository.UpdateRank(player, number, rank);
        
    }
}