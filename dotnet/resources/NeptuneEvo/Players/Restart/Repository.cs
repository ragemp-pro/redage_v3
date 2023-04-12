using System;
using System.Linq;
using GTANetworkAPI;
using NeptuneEvo.Character;
using NeptuneEvo.Handles;
using Redage.SDK;

namespace NeptuneEvo.Players.Restart
{
    public class Repository
    {
        public static void Add(ExtPlayer player, string text)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            if (sessionData.Value == -1) return;
            
            string testmsg = text.ToLower();
            if (Main.stringGlobalBlock.Any(c => testmsg.Contains(c)) ||
                Main.stringDefaultBlock.Any(c => testmsg.Contains(c)))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы ввели запрещенное слово", 3000);
                return;
            }
            
            //var characterData = player.GetCharacterData();

            //if (characterData != null && characterData.AdminLVL > 0)
             //   text = "!{{#F08080}}" + text;
            
            NAPI.ClientEvent.TriggerClientEvent(player, "restart.send", "Я", text);
            
            foreach (var foreachPlayer in RAGE.Entities.Players.All.Cast<ExtPlayer>())
            {
                var foreachSessionData = foreachPlayer.GetSessionData();
                if (foreachSessionData == null) continue;
                if (foreachPlayer == player) continue;
                
                NAPI.ClientEvent.TriggerClientEvent(foreachPlayer, "restart.send", sessionData.Name, text);
            }
        }
    }
}