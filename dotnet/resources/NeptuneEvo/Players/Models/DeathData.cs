namespace NeptuneEvo.Players.Models
{
    public class DeathData
    {
        public bool IsReviving { get; set; } = false;
        public bool IsDying { get; set; } = false;
        public bool InDeath { get; set; } = false;
        public string LastDeath { get; set; } = null;
    }
}
