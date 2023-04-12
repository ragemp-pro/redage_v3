using GTANetworkAPI;

namespace NeptuneEvo.Table.Tasks.Patrolling.Models
{
    public class PatrollingData
    {
        public Vector3 Position;
        public Fractions.Models.Fractions Fraction;
        public bool IsAir;

        public PatrollingData(Vector3 position, Fractions.Models.Fractions fraction, bool isAir)
        {
            this.Position = position;
            this.Fraction = fraction;
            this.IsAir = isAir;
        }
    }
}