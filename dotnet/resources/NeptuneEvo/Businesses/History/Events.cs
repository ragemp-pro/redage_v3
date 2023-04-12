using GTANetworkAPI;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using Redage.SDK;

namespace NeptuneEvo.Businesses.History
{
    public class Events : Script
    {
        [RemoteEvent("server.phone.business.loadStats")]
        public void LoadStats(ExtPlayer player)
        {            
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var bizId = sessionData.SelectData.SelectedBiz;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            if (!characterData.BizIDs.Contains(bizId))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoBusiness), 3000);
                return;
            }
            
            Trigger.SetTask(() => Repository.GetHistory(player, bizId));
        }
    }
}