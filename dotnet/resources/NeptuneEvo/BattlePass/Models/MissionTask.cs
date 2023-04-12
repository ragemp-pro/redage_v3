namespace NeptuneEvo.BattlePass.Models
{
    public class MissionTask
    {
        public string Name;
        public string Title;
        public int Money;
        public int BattlePassTaskId;

        public MissionTask(string name, string title, int money, int battlePassTaskId)
        {
            Name = name;
            Title = title;
            Money = money;
            BattlePassTaskId = battlePassTaskId;
        }
    }
}