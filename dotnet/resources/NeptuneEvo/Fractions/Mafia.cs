using System;
using System.Collections.Generic;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Functions;
using Redage.SDK;

namespace NeptuneEvo.Fractions
{
    class Mafia : Script
    {
        private static readonly nLog Log = new nLog("Fractions.Mafia");

        [ServerEvent(Event.ResourceStart)]
        public void Event_ResourceStart()
        {
            try
            {
                NAPI.TextLabel.CreateTextLabel("~w~Kirill Orlov", new Vector3(-114.937965, 987.44073, 235.7), 5f, 0.3f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension);
                NAPI.TextLabel.CreateTextLabel("~w~Aram Mkhitaryan", new Vector3(1389.9807, 1140.2769, 114.3), 5f, 0.3f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension);
                NAPI.TextLabel.CreateTextLabel("~w~Benjiro Takahashi", new Vector3(-1467.7257, -30.005075, 54.6), 5f, 0.3f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension);
                NAPI.TextLabel.CreateTextLabel("~w~Mario Pellegrini", new Vector3(1392.098, 1155.892, 115.4433), 5f, 0.3f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension);
            }
            catch (Exception e)
            {
                Log.Write($"Event_ResourceStart Exception: {e.ToString()}");
            }
        }
    }
}
