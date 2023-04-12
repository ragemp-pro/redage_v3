using GTANetworkAPI;
using NeptuneEvo.Players.Models;

namespace NeptuneEvo.Handles
{
    public class ExtColShape : ColShape
    {
        public ExtColShape(NetHandle handle) : base(handle)
        {
        }
        public ExtColShapeData ColShapeData;
        public void SetColShapeData(ExtColShapeData сolShapeData)
        {
            ColShapeData = сolShapeData;
        }
    }
}