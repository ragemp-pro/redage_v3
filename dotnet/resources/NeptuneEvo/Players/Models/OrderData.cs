using NeptuneEvo.Core;

namespace NeptuneEvo.Players.Models
{
    public class OrderData
    {
        public int Order { get; set; } = -1;
        public Business WayPoint { get; set; } = null;
        public bool GotProduct { get; set; } = false;
    }
}
