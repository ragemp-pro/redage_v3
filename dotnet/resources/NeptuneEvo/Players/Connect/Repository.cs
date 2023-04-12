using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using NeptuneEvo.Accounts;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Players.Models;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Localization;
using NeptuneEvo.Players.Session.Models;

namespace NeptuneEvo.Players.Connect
{
    class Repository
    {
        private static readonly nLog Log = new nLog("Players.Connect.Repository");
        public static async Task OnPlayerConnected(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                await using var db = new ServerBD("MainDB");//В отдельном потоке
                
                /*var session = await db.Sessions
                    .Where(s => s.Hash == Accounts.Repository.GetSha256($"{sessionData.RealSocialClub}_{sessionData.Address}"))
                    .Where(s => s.Data >= DateTime.Now)
                    .FirstOrDefaultAsync();
                    */

                var getSession = await Session.Repository.GetSession(db, player);

                if (getSession == SessionEnum.Error || getSession == SessionEnum.NoAccount)
                {
                    // Получаем модель пользователя по логину
                    var account = await db.Accounts
                        .Select(a => new
                        {
                            a.Login,
                            a.Email,
                            a.Password,
                            a.Ip,
                            a.Socialclub
                        })
                        .Where(a => a.Socialclub == sessionData.SocialClubName ||
                                    a.Socialclub == sessionData.RealSocialClub)
                        .FirstOrDefaultAsync();

                    if (account != null)
                    {
                        var ban = await db.Banned
                            .Where(
                                v => v.Account.ToLower() == account.Login.ToLower() ||
                                     v.Socialclub == account.Socialclub /* || v.Ip == account.Ip*/)
                            .Where(v => v.Until > DateTime.Now)
                            .Where(v => v.Ishard > 0)
                            .FirstOrDefaultAsync();

                        if (ban != null)
                        {
                            player.setBan($"Вы заблокированы {ban.Time.Day} {ban.Time.ToString("MMMM")} {ban.Time.Year}г. {ban.Time.Hour}:{ban.Time.Minute}:{ban.Time.Second} до {ban.Until.Day} {ban.Until.ToString("MMMM")} {ban.Until.Year}г. {ban.Until.Hour}:{ban.Until.Minute}:{ban.Until.Second} (UTC+3). Причина: {ban.Reason} ({ban.Byadmin})");
                            return;
                        }

                        sessionData.Name = account.Login;
                        sessionData.AuntificationData = new AuntificationData
                        {
                            Login = account.Login,
                            Email = account.Email,
                            Password = account.Password,
                            IsCreateAccount = true,
                        };
                    }
                }

                var playerlist = Main.PlayerIdToEntity.Count;
                
                if (playerlist > Main.PlayersAtOnce) 
                    Main.PlayersAtOnce = playerlist;
                
                if (!Queue.Repository.AddQueue(player))
                {
                    await PlayerToAuntidication(player);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Event_OnPlayerConnected Exception: {e.ToString()}");
            }
        }

        private static List<string> WhiteLogins = new List<string>()
        {
           "source1488", "sokolyansky"
        };
        private static bool IsWhiteLogin(string login)
        {
            return WhiteLogins.Contains(login.ToLower());
        }
        public static async Task PlayerToAuntidication(ExtPlayer player)
        {
            if (player.IsAccountData()) return;

            var sessionData = player.GetSessionData();
            if (sessionData == null) return;

            var auntificationData = sessionData.AuntificationData;
            
            Log.Write($"{sessionData.Name} ({sessionData.SocialClubName} | {sessionData.RealSocialClub}) joining the server.");

            /*if (!IsWhiteLogin(auntificationData.Login))
            {
                Trigger.ClientEvent(player, "client.auth", -1);
                Trigger.ClientEvent(player, "client.closeAll");
                Notify.Send(player, NotifyType.Error, NotifyPosition.Center, $"В данный момент на сервере ведутся технические работы, следите за новостями в оф. дискорде.", 1000 * 60);
                return;
            }*/
            
            if (!WhiteList.Check(player, auntificationData.Login))
                return;
            
            auntificationData.IsBlockAuth = false;
            
            if (sessionData.IsSession || sessionData.IsSessionOneTime)
            {
                sessionData.IsSessionOneTime = false;
                await Accounts.Autorization.Repository.AutorizationAccount(player, auntificationData.Login, auntificationData.Password);
            } 
            else
            {
                if (auntificationData.IsCreateAccount)
                {
                    Trigger.ClientEvent(player, "client.auth", auntificationData.Login);
                }
                else 
                    Trigger.ClientEvent(player, "client.auth", -1);

                sessionData.TimersData.AutoDCTimer = Timers.StartOnce(1000 * (60 * 10), () =>
                {
                    if (player == null || !player.IsSessionData()) return;
                    sessionData.TimersData.AutoDCTimer = null;
                    if (player.IsAccountData()) return;
                    player.setKick(LangFunc.GetText(LangType.Ru, DataName.AuthorizationTimeout));
                });
            }
        }
    }
}
