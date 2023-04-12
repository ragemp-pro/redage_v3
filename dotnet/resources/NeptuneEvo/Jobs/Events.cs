using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Jobs
{
    public class Events : Script
    {
        [ServerEvent(Event.PlayerEnterVehicle)]
        public void OnPlayerEnterVehicle(ExtPlayer player, ExtVehicle vehicle, sbyte seatId) =>
            Repository.OnPlayerEnterVehicle(player, vehicle, seatId);
        
        [ServerEvent(Event.PlayerExitVehicle)]
        public void OnPlayerExitVehicle(ExtPlayer player, ExtVehicle vehicle) =>
            Repository.OnPlayerExitVehicle(player, vehicle);
    }
}