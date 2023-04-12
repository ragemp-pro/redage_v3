using GTANetworkAPI;

namespace NeptuneEvo.Events.HeliCrash.Models
{
    public class HeliCords
    {
        public string Name { get; private set; }
        public Vector3 HeliPosition { get; private set; }
        public Vector3 HeliRotation { get; private set; }
        public Vector3[] BoxPositions = new Vector3[3];
        public Vector3[] BoxRotations = new Vector3[3];

        public HeliCords(string name, Vector3 heliPos, Vector3 heliRot, Vector3[] boxPos, Vector3[] boxRot)
        {
            this.Name = name;
            this.HeliPosition = heliPos;
            this.HeliRotation = heliRot;
            this.BoxPositions = boxPos;
            this.BoxRotations = boxRot;
        }
    }
}