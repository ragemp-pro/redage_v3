using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using LinqToDB;
using NeptuneEvo.Chars;
using NeptuneEvo.Handles;
using NeptuneEvo.Table.Tasks.Models;
using Newtonsoft.Json;

namespace NeptuneEvo.Table.Models
{
    public class TableData
    {
        public string Name { get; set; } 
        public int Id { get; set; }
        public bool IsOpenStock { get; set; } = false;
        public int Drugs { get; set; } = 0;
        public int Materials { get; set; } = 0;
        public int MedKits { get; set; } = 0; 
        public int Money { get; set; } = 0;
        public string Discord;

        public List<BoardData> BoardsList = new List<BoardData>();
        public Dictionary<int, DepartmentData> Departments = new Dictionary<int, DepartmentData>();
        public List<RankToAccess> DefaultAccess { get; set; } = new List<RankToAccess>();
        public Dictionary<int, RankData> Ranks { get; set; } = new Dictionary<int, RankData>();
        public List<WeaponSetData> WeaponSetsData { get; set; } = new List<WeaponSetData>();
        public TableTaskPlayerData[] TasksData = null;
        public ExtTextLabel StockLabel { get; set; } = null;

        public int LeaderRank() => this.Ranks.Keys.OrderBy(r => r).LastOrDefault();
    }
}