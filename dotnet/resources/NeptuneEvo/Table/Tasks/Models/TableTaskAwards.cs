using NeptuneEvo.BattlePass.Models;

namespace NeptuneEvo.Table.Tasks.Models
{
    public class TableTaskAwards
    {
        public BattlePassRewardType Type { get; set; }
        public int ItemId { get; set; }
        public int Count { get; set; }
        public string Data { get; set; }
        public BattlePassRewardGender Gender { get; set; }

        public TableTaskAwards(BattlePassRewardType type, int itemId = (int) Chars.Models.ItemId.Debug, int count = 1, string data = "", BattlePassRewardGender gender = BattlePassRewardGender.None)
        {
            this.Type = type;
            this.ItemId = itemId;
            this.Count = count;
            this.Data = data;
            this.Gender = gender;
        }
    }
}