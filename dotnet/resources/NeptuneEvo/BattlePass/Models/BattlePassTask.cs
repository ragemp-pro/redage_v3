namespace NeptuneEvo.BattlePass.Models
{    
    public enum BattlePassRewardDiff
    {
        None = -1,
        Easy = 0,
        Medium,
        Hard,
    }
    
    public class BattlePassTask
    {
        public int Id;
        public string Text;
        public int Count;
        public int Exp;
        public string MissionName;
        public string MissionTitle;
        public int MissionMoney;
        public BattlePassRewardDiff Diff;
        
        public BattlePassTask(int id, string text, int count, int exp, string missionName = "", string missionTitle = "", int missionMoney = 0, BattlePassRewardDiff diff = BattlePassRewardDiff.None)
        {
            Id = id;
            Text = text;
            Count = count;
            Exp = exp;
            MissionName = missionName;
            MissionTitle = missionTitle;
            MissionMoney = missionMoney;
            Diff = diff;
        }
    }
}