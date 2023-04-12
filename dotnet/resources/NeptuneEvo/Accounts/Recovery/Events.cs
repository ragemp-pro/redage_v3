using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NeptuneEvo.Accounts.Recovery
{
    class Events : Script
    {
        private static readonly nLog Log = new nLog("Accounts.Recovery.Events");

        [RemoteEvent("restorepass")]
        private void TryRecovery(ExtPlayer player, byte state, string loginOrCode)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            if (DateTime.Now < sessionData.TimingsData.NextRestorePass)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Попробуйте еще раз через секунду.", 2500);
                Trigger.ClientEvent(player, "restorepassstep", 2);
                return;
            }
            sessionData.TimingsData.NextRestorePass = DateTime.Now.AddSeconds(1);
            Trigger.SetTask(() =>
            {
                if (state == 0) 
                    Repository.SendEmail(player, loginOrCode);
                else 
                    Repository.RecoveryPassword(player, loginOrCode);
            });
        }
    }
}
