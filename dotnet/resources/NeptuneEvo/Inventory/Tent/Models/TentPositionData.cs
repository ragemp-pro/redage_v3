using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Inventory.Tent.Models
{
    public class TentPositionData
    {        
        public Vector3 shopPosition { get; set; }
        public Vector3 tradePosition { get; set; }
        public bool isBlack { get; set; }

        public TentPositionData(Vector3 Position, Vector3 Rotation, bool isBlack = false)
        {
            this.shopPosition = Position;
            this.tradePosition = Rotation;
            this.isBlack = isBlack;
        }
    }
}