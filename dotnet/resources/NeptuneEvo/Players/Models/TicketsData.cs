using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Models
{
    public class TicketsData
    {
        public int Price { get; set; } = 0;
        public ExtPlayer Target { get; set; } = null;
        public string Reason { get; set; } = null;
    }
}
