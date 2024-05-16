using System.Collections.Generic;

namespace NeptuneEvo.Players.Phone.Forbes.Models
{
    public class ForbesData
    {
        public List<ForbesList> List = new List<ForbesList>();
        public string Name = "";
        public long Money = 0;
        public long SumMoney = 0;
        public int Lvl = 0;
        public bool IsShowForbes = false;
    }
}