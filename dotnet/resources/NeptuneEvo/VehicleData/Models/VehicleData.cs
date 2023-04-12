using System;

namespace NeptuneEvo.VehicleData.Models
{
    public class VehicleData
    {
        public int SqlId { get; set; }
        public string Number { get; set; }
        public string Holder { get; set; }
        public string Model { get; set; }
        public int Health { get; set; }
        public int Fuel { get; set; }
        public VehicleCustomization Components { get; set; }
        public string Position { get; set; }
        public string Rotation { get; set; }
        public int KeyNum { get; set; }
        public float Dirt { get; set; }
        public DateTime HijackTime { get; set; } = DateTime.MinValue;
        public string Tag { get; set; }
    }
}