namespace NeptuneEvo.Events.AirDrop.Models
{
    public class TeamData
    {
        public int TeamFrags { set; get; } = 0;
        public int TeammatesInZone { set; get; } = 0;

        public TeamData(int frags, int matesInZone)
        {
            TeamFrags = frags;
            TeammatesInZone = matesInZone;
        }
    }
}