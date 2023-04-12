using GTANetworkAPI;
using NeptuneEvo.World.War.Models;

namespace NeptuneEvo.Players.Models
{
    public class WarData
    {
        public ushort ObjectId;
        public WarType Type;
        public string MapName;
        public ushort MapId;
        public Vector3 Position;//Место проведения битвы
        public float Range;//Место проведения битвы
        public ushort AttackingId;
        public ushort ProtectingId;
        
        //
        public ushort WarId;
        public bool IsWarZone;
        public bool IsAttacking;
        
    }
}