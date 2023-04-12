using System.Collections.Generic;

namespace NeptuneEvo.VehicleData.LocalData.Models
{
    public class VehicleLocalStateData
    {
        public bool Locked { get; set; } = false;
        public bool Engine { get; set; } = false;
        public bool LeftIL { get; set; } = false;
        public bool RightIL { get; set; } = false;
        public float Dirt { get; set; } = 0.0f;
        //Doors 0-7 (0 = closed, 1 = open, 2 = broken) (This uses enums so don't worry about it)
        public List<int> Door { get; set; } = new List<int>(6) { 0, 0, 0, 0, 0, 0 };
        //public bool[] TyreState { get; set; } = new bool[4] { false, false, false, false };
    }
}