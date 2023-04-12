using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Models
{
    public class InviteData
    {
        public int Fraction { get; set; } = -1;
        public int Organization { get; set; } = -1;
        public ExtPlayer Sender { get; set; } = null;
    }
}
