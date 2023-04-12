using System;

namespace NeptuneEvo.Fractions.Models
{
    public class VehicleTicket
    {
        public int AutoId;
        public int VehAutoId;
        public string VehNumber;
        public string Model;
        public int HolderAutoId;
        public string HolderName;
        public int PolicAutoId;
        public string PolicName;
        public string Text;
        public string Link;
        public DateTime Time;
        public int Price;
        public bool IsEvac;
        public VehicleTicketType Type;
    }
}