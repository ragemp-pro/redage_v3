using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Chars.Models;
using System;

namespace NeptuneEvo.Players.Models
{
    public class TradePropertyData
    {
        public ExtPlayer Player { get; set; }
        public TradeStage Status { get; set; } = TradeStage.none;
        public string Number { get; set; } = null;
        public DateTime Time { get; set; } = DateTime.MinValue;
        public TradePropertyData(ExtPlayer player, DateTime Time)
        {
            this.Player = player;
            this.Time = Time;
        }
    }
}
