using System.Collections.Generic;
using NeptuneEvo.Chars;
using NeptuneEvo.Table.Models;

namespace NeptuneEvo.Organizations.Models
{
    public class OrganizationRankData
    {
        public string Name = "";
        public int Salary = 0;
        public int MaxScore = 0;
        public List<RankToAccess> Access = new List<RankToAccess>();
    }
}