using System;
using System.Linq;
using System.Threading.Tasks;
using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using Localization;
using NeptuneEvo.Accounts;
using NeptuneEvo.Character;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players.Session.Models;
using Redage.SDK;

namespace NeptuneEvo.Players.Session
{
    public class Repository
    {
        private static readonly nLog Log = new nLog("Players.Session");
        private static int SessionDays = 30;
        public static async Task<SessionEnum> GetSession(ServerBD db, ExtPlayer player)
        {                
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return SessionEnum.Error;
                
                var session = await db.Sessions
                    .Where(s => s.Hash == Accounts.Repository.GetSha256($"{sessionData.RealSocialClub}_{sessionData.Address}") && s.Data >= DateTime.Now)
                    .FirstOrDefaultAsync();

                if (session != null)
                {
                    var account = await db.Accounts
                        .Select(a => new {
                            a.Login,
                            a.Email,
                            a.Password,
                            a.Ip,
                            a.Socialclub
                        })
                        .Where(a => a.Login == session.Login)
                        .FirstOrDefaultAsync();

                    if (account != null)
                    {
                        if (Main.ServerNumber > 0 && account.Socialclub != sessionData.SocialClubName &&
                            account.Socialclub != sessionData.RealSocialClub) 
                            return SessionEnum.Error;
                        
                        var ban = await db.Banned
                            .Where(v => v.Account == account.Login || v.Socialclub == account.Socialclub/* || v.Ip == account.Ip*/)
                            .Where(v => v.Until > DateTime.Now)
                            .Where(v => v.Ishard > 0)
                            .FirstOrDefaultAsync();

                        if (ban != null)
                        {
                            player.setBan($"Вы заблокированы {ban.Time.Day} {ban.Time.ToString("MMMM")} {ban.Time.Year}г. {ban.Time.Hour}:{ban.Time.Minute}:{ban.Time.Second} до {ban.Until.Day} {ban.Until.ToString("MMMM")} {ban.Until.Year}г. {ban.Until.Hour}:{ban.Until.Minute}:{ban.Until.Second} (UTC+3). Причина: {ban.Reason} ({ban.Byadmin})");
                            return SessionEnum.Ban;
                        }

                        sessionData.Name = account.Login;
                        sessionData.AuntificationData = new AuntificationData
                        {
                            Login = account.Login,
                            Email = account.Email,
                            Password = account.Password,
                            IsCreateAccount = true
                        };

                        if (session.OneTime)
                        {
                            sessionData.IsSessionOneTime = true;
                            
                            await db.Sessions
                                .Where(s => s.Hash == Accounts.Repository.GetSha256($"{sessionData.RealSocialClub}_{sessionData.Address}"))
                                .DeleteAsync();
                        }
                        else
                        {
                            sessionData.IsSession = true;
                        
                            await SaveSession(db, player);
                        
                            Trigger.ClientEvent(player, "client.character.accountIsSession", true);
                        }
                        
                        return SessionEnum.Success;
                    }
                    return SessionEnum.NoAccount;
                }
            }
            catch (Exception e)
            {
                Log.Write($"GetSession Exception: {e.ToString()}");
            }
            return SessionEnum.Error;
        }

        public static async Task SaveSession(ServerBD db, ExtPlayer player)
        {               
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null)
                    return;
                
                var isSession = await db.Sessions.AnyAsync(s =>
                    s.Hash == Accounts.Repository.GetSha256($"{sessionData.RealSocialClub}_{sessionData.Address}"));
                
                if (isSession)
                {
                    await db.Sessions
                        .Where(s => s.Hash ==
                                    Accounts.Repository.GetSha256($"{sessionData.RealSocialClub}_{sessionData.Address}"))
                        .Set(s => s.Data, DateTime.Now.AddDays(SessionDays))
                        .UpdateAsync();
                }
                else
                {
                    var accountData = player.GetAccountData();
                    if (accountData == null) return;
                    
                    var characterData = player.GetCharacterData();
                    
                    if (characterData == null) return;
                    if (!characterData.IsSpawned) return;
                    
                    await db.InsertAsync(new Sessions
                    {
                        Hash = Accounts.Repository.GetSha256($"{sessionData.RealSocialClub}_{sessionData.Address}"),
                        Login = accountData.Login,
                        Data = DateTime.Now.AddMinutes(SessionDays),
                        OneTime = true
                    });
                }
            }
            catch (Exception e)
            {
                Log.Write($"DeleteSessions Exception: {e.ToString()}");
            }
        }
        public static void UpdateSession(ExtPlayer player)
        {             
            Trigger.SetTask(async () =>
            {
                try
                {
                    var sessionData = player.GetSessionData();
                    if (sessionData == null) return;
                    
                    var accountData = player.GetAccountData();
                    if (accountData == null) return;
                    
                    await using var db = new ServerBD("MainDB");

                    var isSession = await db.Sessions.AnyAsync(s =>
                        s.Hash == Accounts.Repository.GetSha256($"{sessionData.RealSocialClub}_{sessionData.Address}"));
                    
                    if (!isSession)
                    {
                        await db.InsertAsync(new Sessions
                        {
                            Hash = Accounts.Repository.GetSha256($"{sessionData.RealSocialClub}_{sessionData.Address}"),
                            Login = accountData.Login,
                            Data = DateTime.Now.AddDays(SessionDays)
                        });

                        sessionData.IsSession = true;
                        
                        Trigger.ClientEvent(player, "client.character.accountIsSession", true);
                        
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SessionSaved), 3000);
                    }
                    else
                    {
                        await db.Sessions
                            .Where(s => s.Hash == Accounts.Repository.GetSha256($"{sessionData.RealSocialClub}_{sessionData.Address}"))
                            .DeleteAsync();
                        
                        sessionData.IsSession = false;
                        
                        Trigger.ClientEvent(player, "client.character.accountIsSession", false);
                        
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SessionBroken), 3000);
                    }
                }
                catch (Exception e)
                {
                    Log.Write($"DeleteSessions Exception: {e.ToString()}");
                }
            });
        }
        
        
        public static async Task DeleteSessions(ServerBD db)
        {
            try
            {
                await db.Sessions
                    .Where(s => s.Data < DateTime.Now)
                    .DeleteAsync();
            }
            catch (Exception e)
            {
                Log.Write($"DeleteSessions Exception: {e.ToString()}");
            }
        }
    }
}