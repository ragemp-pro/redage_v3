using System.Collections.Generic;

namespace NeptuneEvo.BattlePass.Models
{
    public class BattlePassTasks
    {
        public int Index = 0;
        public int Count = 0;
        public bool IsDone = false;
    }
    public class BattlePassData
    {
        public List<BattlePassTasks> TasksDay = new List<BattlePassTasks>();
        public List<BattlePassTasks> TasksWeek = new List<BattlePassTasks>();
        public int Lvl = 0;
        public int Exp = 0;
        public bool IsPremium = false;
        public List<int> TookReward = new List<int>();
        public List<int> TookRewardPremium = new List<int>();
        public int Time = 0;
    }
}