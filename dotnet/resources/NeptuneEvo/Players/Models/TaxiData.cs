using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Models
{
    public class TaxiData
    {
        public ExtPlayer Driver { get; set; } = null;
        public ExtPlayer Passager { get; set; } = null;
    }
}
