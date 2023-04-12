using System;
using System.Linq;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Handles;
using NeptuneEvo.Organizations.Models;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.Players;
using NeptuneEvo.Table.Models;
using Redage.SDK;

namespace NeptuneEvo.Organizations.Table.Settings
{
    public class Repository
    {
        public static void UpdateStock(ExtPlayer player)
        {
            try
            {
                if (!player.IsOrganizationAccess(RankToAccess.OpenStock)) return;
                
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;

                organizationData.IsOpenStock = !organizationData.IsOpenStock;

                if (organizationData.IsOpenStock)
                {
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы открыли склад семьи",
                        3000);
                    Organizations.Table.Logs.Repository.AddLogs(player, OrganizationLogsType.OpenStock, "Открыл склад");
                }
                else
                {
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы закрыли склад семьи", 3000);
                    Organizations.Table.Logs.Repository.AddLogs(player, OrganizationLogsType.CloseStock, "Закрыл склад");
                }

                Trigger.ClientEvent(player, "client.org.main.isStock", organizationData.IsOpenStock);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public static void SaveSetting(ExtPlayer player, string slogan, byte salary, string discord, int colorR, int colorG, int colorB)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                if (DateTime.Now < sessionData.TimingsData.NextGlobalChat)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Block10Min), 4500);
                    return;
                }
                
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
                
                if (!organizationData.IsLeader(player.GetUUID()))
                    return;
                
                slogan = Main.BlockSymbols(Main.RainbowExploit(slogan));
                if (slogan.Length > 65)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Слоган слишком длинный", 4500);
                    return;
                }
                
                string testmsg = slogan.ToLower();
                if (Main.stringGlobalBlock.Any(c => testmsg.Contains(c)))
                {
                    sessionData.TimingsData.NextGlobalChat = DateTime.Now.AddMinutes(10);
                    Trigger.SendToAdmins(3, "!{#636363}[A] " + LangFunc.GetText(LangType.Ru, DataName.AdminAlertFTableNews, player.Name, player.Value, slogan));
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RestrictedWordsTableNews), 15000);
                    return;
                }
                
                organizationData.Slogan = slogan;
                
                if (salary > 3)
                    salary = 3;
                
                organizationData.Salary = salary;
                
                organizationData.Discord = discord;

                if (colorR != -1)
                    organizationData.Color = new Color(colorR, colorG, colorB);
                
                organizationData.SaveSettings();
                
                Table.Player.Repository.MainLoad(player);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
    }
}