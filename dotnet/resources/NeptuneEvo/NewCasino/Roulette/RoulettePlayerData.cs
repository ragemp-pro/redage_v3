using System;
using System.Collections.Generic;

namespace NeptuneEvo.NewCasino
{
    public class RoulettePlayerData
    {
        public List<BetData> AllBets = new List<BetData>();
        public int SelectedTable = -1;
        public DateTime Time = DateTime.Now;

        public GTANetworkAPI.Object FBetObject = null;
        public GTANetworkAPI.Object SBetObject = null;
        public GTANetworkAPI.Object TBetObject = null;

        public int WinMoney = 0;


        public RoulettePlayerData(int SelectedTable)
        {
            this.SelectedTable = SelectedTable;
        }
    }
}
