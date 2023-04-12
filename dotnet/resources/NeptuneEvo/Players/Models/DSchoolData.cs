using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Models
{
    public class DSchoolData
    {
        public ExtVehicle Vehicle { get; set; } = null;
        public byte License { get; set; } = 255;
        public short Check { get; set; } = -1;
        public bool IsDriving { get; set; } = false;
    }
}
