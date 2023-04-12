using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Models
{
    public class DeliveryData
    {
        public ExtVehicle Vehicle { get; set; } = null;
        public int Point { get; set; } = -1;
    }
}
