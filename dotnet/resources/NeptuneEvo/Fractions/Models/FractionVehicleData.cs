using GTANetworkAPI;
using NeptuneEvo.VehicleData.Models;

namespace NeptuneEvo.Fractions.Models
{
    public class FractionVehicleData
    {
        public string model = "";
        public Vector3 position = new Vector3();
        public Vector3 rotation = new Vector3();
        public int rank = 0;
        public int defaultRank = 0;
        public int color1 = 0;
        public int color2 = 0;
        public VehicleCustomization customization = null;
        public uint Dimension = 0;

        public FractionVehicleData (string model, Vector3 position, Vector3 rotation, int rank, int defaultRank, int color1, int color2, VehicleCustomization customization)
        {
            this.model = model.ToLower();
            this.position = position;
            this.rotation = rotation;
            this.rank = rank;
            this.defaultRank = defaultRank;
            this.color1 = color1;
            this.color2 = color2;
            this.customization = customization;
        }
    }
}