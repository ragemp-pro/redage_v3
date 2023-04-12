using GTANetworkAPI;
using NeptuneEvo.Handles;
using System;

namespace NeptuneEvo.Players.Models
{
    public class SellItemData
    {
        public ExtPlayer Buyer { get; set; } = null;
        public string Number { get; set; } = null;
        public int Sim { get; set; } = 0;
        public int Count { get; set; } = 0;
        public ExtPlayer Seller { get; set; } = null;
        public int Price { get; set; } = 0;
        public DateTime Time { get; set; } = DateTime.Now;

        /*public SellItemData() : this(DateTime.Now.AddSeconds(30))
        {

        }
        public SellItemData(DateTime time)
        {
            Console.WriteLine("SellItemData - 0");
            if (this.Buyer != null || this.Seller != null)
            {
                this.Time = time;
                Console.WriteLine("SellItemData - 1");
            }
            else this.Time = DateTime.Now;
        }*/
    }
}
