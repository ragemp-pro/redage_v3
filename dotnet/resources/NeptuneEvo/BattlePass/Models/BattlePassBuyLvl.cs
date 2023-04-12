namespace NeptuneEvo.BattlePass.Models
{
    public class BattlePassBuyLvl
    {
        public int PriceRB { get; set; }
        public int Lvl { get; set; }

        public BattlePassBuyLvl(int priceRB, int lvl)
        {
            PriceRB = priceRB;
            Lvl = lvl;
        }
    }
}