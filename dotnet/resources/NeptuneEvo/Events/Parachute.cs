using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using Redage.SDK;
using System;
using NeptuneEvo.Character;

namespace NeptuneEvo.Events
{
    class Parachute : Script
    {
        private static readonly nLog Log = new nLog("Events.Parachute");
        [RemoteEvent("server.parachute.state")]
        public static void ParachuteState(ExtPlayer player, int state)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Trigger.ClientEventInRange(player.Position, 250f, "client.parachute.state", player, state);
            }
            catch (Exception e)
            {
                Log.Write($"ParachuteState Exception: {e.ToString()}");
            }
        }
    }
}
