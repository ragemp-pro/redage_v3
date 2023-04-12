using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using Redage.SDK;
using System;
using System.Threading.Tasks;

namespace NeptuneEvo.Accounts.Autorization
{
    class Events : Script
    {
        private static readonly nLog Log = new nLog("Accounts.Autorization.Events");

        [RemoteEvent("signin")]
        private void TryAutorize(ExtPlayer player, string loginOrEmail, string password)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            
            if (Players.Queue.Repository.List.Contains(player))
                return;
            
            Trigger.SetTask(async () =>
            {
                password = Accounts.Repository.GetSha256(password);
                await Repository.AutorizationAccount(player, loginOrEmail, password);
            });
        }
    }
}
