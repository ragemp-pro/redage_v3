using System;
using System.Collections.Generic;

namespace NeptuneEvo.Table.Models
{
    public class DepartmentData
    {
        public string Name;
        public string Tag;
        public DateTime Date;
        public Dictionary<int, DepartmentRankData> Ranks { get; set; } = new Dictionary<int, DepartmentRankData>();
    }
}