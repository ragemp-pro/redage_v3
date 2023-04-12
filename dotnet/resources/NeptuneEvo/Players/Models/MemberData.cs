using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using LinqToDB;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Table.Models;
using NeptuneEvo.Table.Tasks.Models;

namespace NeptuneEvo.Players.Models
{
    public class MemberData
    {
        public int UUID { get; set; } = 0;
        public string Name { get; set; } = "";
        public int Id { get; set; } = 0;
        public byte Rank { get; set; } = 0;
        public DateTime Date { get; set; } = DateTime.MinValue;
        public string Avatar { get; set; } = "";
        public int PlayerId { get; set; } = -1;
        public int DepartmentId { get; set; } = 0;
        public int DepartmentRank { get; set; } = 0;
        public List<RankToAccess> Access { get; set; } = new List<RankToAccess>();
        public List<RankToAccess> Lock { get; set; } = new List<RankToAccess>();
        public int Score { get; set; } = 0;
        public DateTime LastLoginDate { get; set; } = DateTime.MinValue;
        public bool IsSave { get; set; } = false;
        public TimeInfo Time { get; set; } = new TimeInfo();
        
        public TableTaskPlayerData[] TasksData = null;

        public void SetPlayerId(int id = -1)
        {
            this.PlayerId = id;
        }
    }
}