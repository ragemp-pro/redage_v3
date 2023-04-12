using System.Collections.Generic;

namespace NeptuneEvo.Table.Models
{
    public class RankData
    {
        public string Name = "";
        public int Salary = 0;
        public int MaxScore = 0;
        public List<RankToAccess> Access = new List<RankToAccess>();
    }
}