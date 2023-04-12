using System;
using System.Collections.Generic;

namespace NeptuneEvo.Players.Phone.Auction.Models
{
    public class AuctionData
    {
        public int Id;
        public AuctionType Type;
        public int ElementId;
        public DateTime Time;
        public int CreateUUID;
        public int BetCount;
        public int LastBetUUID;
        public string Title;
        public string Text;
        public string Image;
        public int CreatePrice;
        public int LastPrice;
        public List<AuctionBetData> BetsData = new List<AuctionBetData>();
        public bool IsSave;
        public bool IsEnd;
    }
}