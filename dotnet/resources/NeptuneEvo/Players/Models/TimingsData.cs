using System;

namespace NeptuneEvo.Players.Models
{
    public class TimingsData
    {
        public DateTime NextAD { get; set; } = DateTime.MinValue;
        public DateTime NextCallEMS { get; set; } = DateTime.MinValue;
        public DateTime NextGunRob { get; set; } = DateTime.MinValue;
        public DateTime NextRob { get; set; } = DateTime.MinValue;
        public DateTime NextCallPolice { get; set; } = DateTime.MinValue;
        public DateTime NextNHistory { get; set; } = DateTime.MinValue;
        public DateTime NextReport { get; set; } = DateTime.MinValue;
        public DateTime NextBankTransfer { get; set; } = DateTime.MinValue;
        public DateTime NextTransfer { get; set; } = DateTime.MinValue;
        public DateTime NextDeathMoney { get; set; } = DateTime.MinValue;
        public DateTime NextEat { get; set; } = DateTime.MinValue;
        public DateTime NextDrugs { get; set; } = DateTime.MinValue;
        public DateTime NextMedKit { get; set; } = DateTime.MinValue;
        public DateTime NextBonusCode { get; set; } = DateTime.MinValue;
        public DateTime NextGiftDonate { get; set; } = DateTime.MinValue;
        public DateTime NextDropItem { get; set; } = DateTime.MinValue;
        public DateTime NextClientTryCatch { get; set; } = DateTime.MinValue;
        public DateTime NextGlobalChat { get; set; } = DateTime.MinValue;
        public DateTime NextRestorePass { get; set; } = DateTime.MinValue;
    }
}
