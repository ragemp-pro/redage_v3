using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Phone.Taxi.Orders
{
    public class Events : Script
    {
        [RemoteEvent("server.phone.taxijob.load")]
        public void GetSelect(ExtPlayer player) => 
            Repository.GetSelect(player);
        [RemoteEvent("server.phone.taxijob.take")]
        public void OnTake(ExtPlayer player, int id) => 
            Repository.OnTake(player, id);

        [ServerEvent(Event.PlayerEnterVehicle)]
        public void OnPlayerEnterVehicle(ExtPlayer player, ExtVehicle vehicle, sbyte seatId) =>
            Repository.OnPlayerEnterVehicle(player, vehicle, seatId);
        
        [ServerEvent(Event.PlayerExitVehicle)]
        public void OnPlayerExitVehicle(ExtPlayer player, ExtVehicle vehicle) =>
            Repository.OnPlayerExitVehicle(player, vehicle);
    }
}