using System;
using System.Linq;
using Localization;
using NeptuneEvo.Chars;
using NeptuneEvo.Handles;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.Players;
using NeptuneEvo.Table.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Organizations.Table.Board
{
    public class Repository
    {
        public static void Get(ExtPlayer player, int index)
        {
            var organizationData = player.GetOrganizationData();
            if (organizationData == null) 
                return;
            
            var board = NeptuneEvo.Table.Repository.GetBoard(organizationData.BoardsList, index);

            Trigger.ClientEvent(player, "client.org.main.setBoard", JsonConvert.SerializeObject(board));
        }
        
                
        public static void Add(ExtPlayer player, string title, string text)
        {
            try
            {
                if (!player.IsOrganizationAccess(RankToAccess.TableWall)) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                if (DateTime.Now < sessionData.TimingsData.NextGlobalChat)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Block10Min), 4500);
                    return;
                }
                title = Main.BlockSymbols(Main.RainbowExploit(title));
                text = Main.BlockSymbols(Main.RainbowExploit(text));
                if (text.Length > 100)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxTableNewsText), 4500);
                    return;
                }
                string testmsg = $"{title.ToLower()} {text.ToLower()}";
                if (Main.stringGlobalBlock.Any(c => testmsg.Contains(c)))
                {
                    sessionData.TimingsData.NextGlobalChat = DateTime.Now.AddMinutes(10);
                    Trigger.SendToAdmins(3, "!{#636363}[A] " + LangFunc.GetText(LangType.Ru, DataName.AdminAlertFTableNews, player.Name, player.Value, text));
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RestrictedWordsTableNews), 15000);
                    return;
                }

                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberOrganizationData == null) 
                    return;

                var organizationData = Organizations.Manager.GetOrganizationData(memberOrganizationData.Id);
                if (organizationData == null) 
                    return;

                var boardData = new BoardData
                {
                    UUId = memberOrganizationData.UUID,
                    Name = memberOrganizationData.Name,
                    Rank = memberOrganizationData.Rank,
                    Time = DateTime.Now,
                    Title = title,
                    Text = text,
                };
                
                organizationData.BoardsList.Add(boardData);
                Fractions.Manager.sendOrganizationMessage(memberOrganizationData.Id, "[F] " + LangFunc.GetText(LangType.Ru, DataName.TableNewNews), true);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouAddedTableNews), 3000);

                Get(player, 1);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public static void Update(ExtPlayer player, int index, string title, string text)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                //else if (!Manager.canUseCommand(player, RankToAccess.TableWall)) return;
                else if (DateTime.Now < sessionData.TimingsData.NextGlobalChat)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Block10Min), 4500);
                    return;
                }
                title = Main.BlockSymbols(Main.RainbowExploit(title));
                text = Main.BlockSymbols(Main.RainbowExploit(text));
                string testmsg = $"{title.ToLower()} {text.ToLower()}";
                if (Main.stringGlobalBlock.Any(c => testmsg.Contains(c)))
                {
                    sessionData.TimingsData.NextGlobalChat = DateTime.Now.AddMinutes(10);
                    Trigger.SendToAdmins(3, "!{#636363}[A] " + LangFunc.GetText(LangType.Ru, DataName.AdminAlertFTableNews, player.Name, player.Value, text));
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RestrictedWordsTableNews), 15000);
                    return;
                }

                index--;
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
                
                var boards = organizationData.BoardsList.ToList();
                boards.Reverse();
                
                if (index < 0 || index >= boards.Count) return;
                
                var boardData = boards[index];
                if (boardData.UUId != player.GetUUID() && !player.IsOrganizationAccess(RankToAccess.EditAllTabletWall)) return;

                boardData.Title = title;
                boardData.Text = text;
                
                boards.Reverse();
                organizationData.BoardsList = boards;
                
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouEditedTableNews), 3000);
                Get(player, index + 1);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static void Delete(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                //else if (!Manager.canUseCommand(player, RankToAccess.TableWall)) return;
                else if (DateTime.Now < sessionData.TimingsData.NextGlobalChat)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Block10Min), 4500);
                    return;
                }

                index--;
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
                
                var boards = organizationData.BoardsList.ToList();
                boards.Reverse();
                
                if (index < 0 || index >= boards.Count) return;
                
                var boardData = boards[index];
                if (boardData.UUId != player.GetUUID() && !player.IsOrganizationAccess(RankToAccess.EditAllTabletWall)) return;
                
                organizationData.BoardsList.Remove(boardData);
                
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouDeletedTableNews), 3000);
                Get(player, 1);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
    }
}