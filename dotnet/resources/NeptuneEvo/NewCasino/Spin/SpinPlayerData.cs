using System;
using System.Collections.Generic;

namespace NeptuneEvo.NewCasino
{
    public class SpinPlayerData
    {
        public int SelectSpin = -1;

        public int Cash = 0;

        public DateTime Time = DateTime.Now;


        public List<int> Spins = new List<int>();


        public SpinPlayerData(int SelectSpin)
        {
            this.SelectSpin = SelectSpin;
        }
    }
}
