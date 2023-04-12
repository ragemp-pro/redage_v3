using System.Collections.Generic;

namespace NeptuneEvo.Table.Tasks.Models
{
    public class TableTaskData
    {
        public string Name;
        public string Desc;
        public int MaxCount;
        public int PersonExp;
        public int OrgExp;
        public List<TableTaskAwards> Awards;
    }
}