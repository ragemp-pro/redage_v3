using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Accounts;
using NeptuneEvo.Functions;
using NeptuneEvo.Players;
using Redage.SDK;
using System;
using System.Threading.Tasks;
using Localization;

namespace NeptuneEvo.Character.Delete
{
    class Events : Script
    {
        private static readonly nLog Log = new nLog("Accounts.Delete.Events");

        [RemoteEvent("server.character.delete")]
        private void DeleteCharacter(ExtPlayer player, int slot)
        {
            if (!FunctionsAccess.IsWorking("DeleteCharacter"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                return;
            }
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            Trigger.SetTask(async () =>
            {
                await Repository.IsDeleteCharacter(player, slot);
            });
        }
    }
}
