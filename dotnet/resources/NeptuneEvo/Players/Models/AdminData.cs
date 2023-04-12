using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Models
{
    public class AdminData
    {
        public bool SpMode { get; set; } = false;
        public bool SpInvise { get; set; } = false;
        public uint SPDimension { get; set; } = 0;
        public Vector3 SPPosition { get; set; } = new Vector3();
        public int SPPlayer { get; set; } = -1;
        public bool IsRemoveObject { get; set; } = false;
        public int LastPunishMinute { get; set; } = -1;
        public int MuteCount { get; set; } = -1;
        public int KickCount { get; set; } = -1;
        public int JailCount { get; set; } = -1;
        public int WarnCount { get; set; } = -1;
        public int BansCount { get; set; } = -1;
        public int AclearCount { get; set; } = -1;
    }
}
