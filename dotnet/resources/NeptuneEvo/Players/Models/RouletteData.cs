using NeptuneEvo.Chars.Models;
using System;
using Newtonsoft.Json;

namespace NeptuneEvo.Players.Models
{
    public class RouletteData
    {
        public bool Done { get; set; }
        public int IndexList { get; set; }
        [JsonIgnore]
        public RouletteItemData Item { get; set; }
        public int ItemIndex { get; set; }
        public int Index { get; set; }
        public int Amount { get; set; }
        public int Price { get; set; }
        public string Text { get; set; }
        public bool BtnTake { get; set; }
        public bool TypeCurrency { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public int CaseID { get; set; }
        public byte FreeCaseBonus { get; set; }
        public RouletteData(RouletteItemData Item, int ItemIndex, int Index, int Amount, int Price, string Text, bool BtnTake, bool TypeCurrency, DateTime CreateTime, int CaseID, byte BonusCase)
        {
            this.Item = Item;
            this.ItemIndex = ItemIndex;
            this.Index = Index;
            this.Amount = Amount;
            this.Price = Price;
            this.Text = Text;
            this.BtnTake = BtnTake;
            this.TypeCurrency = TypeCurrency;
            this.CreateTime = CreateTime;
            this.CaseID = CaseID;
            this.FreeCaseBonus = BonusCase;
            this.Done = false;
            this.IndexList = 0;
        }
    }
}
