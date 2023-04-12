using System;

namespace NeptuneEvo.Players.Models
{
    public class AfkData
    {
        public bool IsAfk = false;
        public DateTime Time = DateTime.Now;
        public int PayDayMinute = 0;
        
    }
}