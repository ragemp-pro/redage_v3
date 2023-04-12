using System.Collections.Generic;

namespace NeptuneEvo.Table.Models
{
    public class DepartmentRankData
    {
        public string Name = "";
        public List<RankToAccess> Access = new List<RankToAccess>();
        public List<RankToAccess> Lock = new List<RankToAccess>();
    }
}