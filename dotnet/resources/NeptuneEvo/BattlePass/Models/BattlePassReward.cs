using System;

namespace NeptuneEvo.BattlePass.Models
{
    public enum BattlePassRewardGender
    {
        None = 0,
        Man,
        Woman
    }
    public class BattlePassReward
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Png { get; set; }
        public BattlePassRewardType Type { get; set; }
        public int ItemId { get; set; }
        public int Count { get; set; }
        public string Data { get; set; }
        public BattlePassRewardGender Gender { get; set; }

        public BattlePassReward(int id, string name = "", string png = "", BattlePassRewardType type = BattlePassRewardType.None, int itemId = 0, int count = 0, string data = "", BattlePassRewardGender gender = BattlePassRewardGender.None)
        {
            Id = id;
            Name = name;
            Png = png;
            Type = type;
            ItemId = itemId;
            Count = count;
            Data = data;
            Gender = gender;
        }
    }
}