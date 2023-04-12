using System;
using System.Collections.Generic;

namespace NeptuneEvo.Players.Phone.Sim
{
    public class Repository
    {
        private static List<int> SimCards = new List<int>();

        public static int GenerateSimCard(int minValue = 1000000, int maxValue = 9999999)
        {
            var rand = new Random();
            var result = rand.Next(minValue, maxValue);
            
            while (SimCards.Contains(result)) 
                result = rand.Next(minValue, maxValue);
            
            return result;
        }

        public static void Add(int sim)
        {
            if (!SimCards.Contains(sim))
                SimCards.Add(sim);
        }
        
        public static void Remove(int sim)
        {
            if (SimCards.Contains(sim))
                SimCards.Remove(sim);
        }

        public static bool Contains(int sim) => SimCards.Contains(sim);
    }
}