using GTANetworkAPI;

namespace NeptuneEvo.VehicleData.Models
{
    public class VehicleCustomization
    {
        public Color PrimColor { get; set; } = new Color(0, 0, 0);
        public Color SecColor { get; set; } = new Color(0, 0, 0);

        public int PrimModColor { get; set; } = -1;
        public int SecModColor { get; set; } = -1;

        public int Muffler { get; set; } = -1;
        public int SideSkirt { get; set; } = -1;
        public int Hood { get; set; } = -1;
        public int Spoiler { get; set; } = -1;
        public int Lattice { get; set; } = -1;
        public int Wings { get; set; } = -1;
        public int Roof { get; set; } = -1;
        public int Vinyls { get; set; } = -1;
        public int FrontBumper { get; set; } = -1;
        public int RearBumper { get; set; } = -1;

        public int Engine { get; set; } = -1;
        public int Turbo { get; set; } = -1;
        public int Horn { get; set; } = -1;
        public int Transmission { get; set; } = -1;
        public int WindowTint { get; set; } = 0;
        public int Suspension { get; set; } = -1;
        public int Brakes { get; set; } = -1;
        public int Headlights { get; set; } = -1;
        //public int HeadlightColor { get; set; } = 0;
        public int NumberPlate { get; set; } = 0;

        public int Wheels { get; set; } = -1;
        public int WheelsType { get; set; } = 0;
        public int WheelsColor { get; set; } = 0;

        public Color NeonColor = new Color(0, 0, 0, 0);

        public int ColorAdditional { get; set; } = 0;
        public int Cover { get; set; } = 0;
        public int CoverColor { get; set; } = 1;            
        public int Frame { get; set; } = -1;
        public int NeonIndex { get; set; } = -1;
            
        public uint Hash { get; set; }
        //public string Model { get; set; }
    }
}