using System.Collections.Generic;
using GTANetworkAPI;
namespace NeptuneEvo.Core.FakeCars
{
    public class FakeCarInfo
    {
        public uint Model { get; set; }
        public Vector3 Position { get; set; }
        public float Rotation { get; set; }
        public int Color1 { get; set; }
        public int Color2 { get; set; }
        public string PlateNumber { get; set; }

        public FakeCarInfo()
        {
            
        }
        public FakeCarInfo(uint model, Vector3 pos, float rot, int color1, int color2, string plate)
        {
            (Model, Position, Rotation, Color1, Color2, PlateNumber) = (model, pos, rot, color1, color2, plate);
        }
        public FakeCarInfo(string model, Vector3 pos, float rot, int color1, int color2, string plate)
        {
            (Model, Position, Rotation, Color1, Color2, PlateNumber) = (NAPI.Util.GetHashKey(model), pos, rot, color1, color2, plate);
        }
        
    }
}
