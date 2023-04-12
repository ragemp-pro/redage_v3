using GTANetworkAPI;
using NeptuneEvo.Handles;
using Redage.SDK;
using System;

namespace NeptuneEvo.Core
{
    class Fingerpointing : Script
    {
        private static readonly nLog Log = new nLog("Core.Finger");
        [RemoteEvent("server.fpsync.update")]
        public void FingerSyncUpdate(ExtPlayer player, float camPitch, float camHeading)
        {
            try
            {
                if (player == null) return;
                BattlePass.Repository.UpdateReward(player, 89);
                Trigger.ClientEventInRange(player.Position, 250f, "client.fpsync.update", player.Value, camPitch, camHeading);
            }
            catch (Exception e)
            {
                Log.Write($"FingerSyncUpdate Exception: {e.ToString()}");
            }
        }
    }
}
