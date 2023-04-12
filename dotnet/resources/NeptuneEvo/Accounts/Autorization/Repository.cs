using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using NeptuneEvo.Accounts.Autorization.Models;
using NeptuneEvo.Accounts.Models;
using NeptuneEvo.Chars;
using NeptuneEvo.Players;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Localization;

namespace NeptuneEvo.Accounts.Autorization
{
    public class Repository
    {
        private static readonly nLog Log = new nLog("Accounts.Autorization.Repository");

        public static async Task AutorizationAccount(ExtPlayer player, string loginOrEmail, string password)
        {
            /*Ban ban = Ban.Get1(player, login);
            if (ban != null)
            {
                if (ban.isHard && ban.CheckDate())
                {
                    Ban.BanMessageFormat(player, ban);
                    return;
                }
            }*/
            var testSpeedLoad = DateTime.Now;
            AutorizationEnum result = await InitAccount(player, loginOrEmail, password);
            if (result == AutorizationEnum.Authorized) LoadCharacter.Repository.Load(player, testSpeedLoad);
            else if (result == AutorizationEnum.LoadingError) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AuthorizWait), 3000);
            else if (result == AutorizationEnum.Already) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyAuthorized), 3000);
            else if (result == AutorizationEnum.Refused) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IncorrectInput), 3000);
            else if (result == AutorizationEnum.SclubError) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SocialClubDoesntCorrect), 3000);
            else if (result == AutorizationEnum.Error) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ConnectError), 3000);
            //else if (result == AutorizationEnum.MaxSlots) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "", 3000);
            //Log.Write($"{sessionData.Name} ({sessionData.SocialClubName} | {sessionData.RealSocialClub}) tryed to signin.");
        }

        public static async Task<AutorizationEnum> InitAccount(ExtPlayer player, string loginOrEmail, string password)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return AutorizationEnum.LoadingError;
                if (sessionData.RealHWID.Equals("NONE") || sessionData.RealSocialClub.Equals("NONE")) 
                    return AutorizationEnum.LoadingError;
                if (player.IsAccountData()) 
                    return AutorizationEnum.Already;
                if (Players.Queue.Repository.AddQueue(player)) 
                    return AutorizationEnum.MaxSlots;
                    
                var auntificationData = sessionData.AuntificationData;

                if (Main.ServerNumber != 0 && !auntificationData.IsCreateAccount)
                    return AutorizationEnum.SclubError;

                loginOrEmail = loginOrEmail.ToLower();

                if (loginOrEmail == auntificationData.Login.ToLower())
                {
                    if (auntificationData.Password != password)
                        return AutorizationEnum.Refused;
                }

                await using var db = new ServerBD("MainDB");//В отдельном потоке
                
                // Получаем модель пользователя по логину
                var account = await db.Accounts
                    .Where(v => (v.Login.ToLower() == loginOrEmail || v.Email.ToLower() == loginOrEmail) && v.Password == password)
                    .FirstOrDefaultAsync();

                // Если база не вернула значение, то отправляем сброс
                if (account == null)
                    return AutorizationEnum.Refused;
                
                if (Main.ServerNumber != 0 && !account.Socialclub.Equals(sessionData.RealSocialClub) && !account.Socialclub.Equals(sessionData.SocialClubName))
                    return AutorizationEnum.SclubError;
                
                var target = Accounts.Repository.GetPlayerToLogin(account.Login);
                if (Main.ServerSettings.IsCheckOnlineLogin && target != null) 
                    return AutorizationEnum.Already;

                if (Players.Queue.Repository.AddQueue(player)) 
                    return AutorizationEnum.MaxSlots;
                
                /*if (Character.Repository.GetPlayers().Any(p => p.GetAccountData()?.Login == account.Login))
                    return AutorizationEnum.Already;*/
                    
                    
                //Удаляем таймер на авторизацию
                if (sessionData.TimersData.AutoDCTimer != null)
                {
                    Timers.Stop(sessionData.TimersData.AutoDCTimer);
                    sessionData.TimersData.AutoDCTimer = null;
                }

                var accountData = new AccountData
                {
                    Login = account.Login,
                    Email = account.Email,
                    Password = account.Password,
                    HWID = sessionData.RealHWID,
                    IP = sessionData.Address,
                    SocialClub = account.Socialclub,
                    RedBucks = account.Redbucks,
                    VipLvl = account.Viplvl,
                    VipDate = account.Vipdate,

                    PresentGet = Convert.ToBoolean(account.Present),
                    RefPresentGet = Convert.ToBoolean(account.Refpresent),

                    RefferalId = account.RefferalId,

                    isSubscribe = account.IsSubscribe,
                    SubscribeEndTime = account.SubscribeEndTime,
                    SubscribeTime = account.SubscribeTime,

                    ReceivedAwardWeek = account.ReceivedAwardWeek,
                    ReceivedAwardDonate = account.ReceivedAwardDonate,

                    Unique = Donate.SetUnique(account.Unique),
                    
                    LastSelectCharUUID = account.LastSelectCharUUID,
                    
                    Ga = account.Ga,
                };

                //
                try
                {
                    accountData.PromoCodes = JsonConvert.DeserializeObject<List<string>>(account.Promocodes);
                }
                catch
                {
                    accountData.PromoCodes = new List<string>();
                }
                //
                try
                {
                    accountData.BonusCodes = JsonConvert.DeserializeObject<List<string>>(account.Bonuscodes);
                }
                catch
                {
                    accountData.BonusCodes = new List<string>();
                }
                //
                List<int> Chars = new List<int>();
                try
                {
                    Chars = JsonConvert.DeserializeObject<List<int>>(account.Characters);
                }
                catch
                {
                    Chars = new List<int>() { -2, -2, -2, -2, -2, -2 };
                }

                accountData.Chars = new List<int>()
                {
                    account.Character1,
                    account.Character2,
                    account.Character3,
                };

                if (Main.ServerSettings.IsMerger)
                {
                    foreach (int i in Chars)
                    {
                        accountData.Chars.Add(i);
                    }
                }

                //
                try
                {
                    accountData.FreeCase = JsonConvert.DeserializeObject<int[]>(account.@case);         
                }
                catch
                {
                    accountData.FreeCase = new int[3] { 0, 0, 0 };
                }
                //
                try
                {
                    accountData.CollectionGifts = JsonConvert.DeserializeObject<List<int>>(account.CollectionGifts);
                }
                catch
                {
                    accountData.CollectionGifts = new List<int>();
                }

                try
                {
                    accountData.ReceivedAward = JsonConvert.DeserializeObject<List<int>>(account.ReceivedAward);
                    if (accountData.ReceivedAward.Count < 9) accountData.ReceivedAward.Add(0);
                    if (accountData.ReceivedAward.Count < 10) accountData.ReceivedAward.Add(0);
                    if (accountData.ReceivedAward.Count < 11) accountData.ReceivedAward.Add(0);
                    if (accountData.ReceivedAward.Count < 12) accountData.ReceivedAward.Add(0);
                    if (accountData.ReceivedAward.Count < 13) accountData.ReceivedAward.Add(0);
                    if (accountData.ReceivedAward.Count < 14) accountData.ReceivedAward.Add(0);
                }
                catch
                {
                    accountData.ReceivedAward = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 };
                }
                //
                if (DateTime.Now > accountData.SubscribeEndTime)
                {
                    accountData.isSubscribe = false;
                    accountData.SubscribeEndTime = DateTime.Now;
                    accountData.SubscribeTime = DateTime.Now;
                }

                player.SetAccountData(accountData);
                
                if (!Main.TodayUniqueHWIDs.Contains(accountData.HWID)) 
                    Main.TodayUniqueHWIDs.Add(accountData.HWID);

                Main.SetUpEverything(player);//TODO

                //
                
                var ticks = Convert.ToInt64(DateTime.Now.Ticks - account.ExitDate.Ticks);

                if (ticks >= 1)
                {
                    var date = new DateTime(ticks);

                    if (date.Day > 20)
                        Utils.Analytics.HelperThread.AddEvent("login_returned_user", accountData.Email, accountData.Ga);
                    else
                        Utils.Analytics.HelperThread.AddEvent("login_current_user", accountData.Email, accountData.Ga);
                }
                //
                
                return AutorizationEnum.Authorized;
            }
            catch (Exception e)
            {
                Log.Write($"AutorizationAccount Exception: {e.ToString()}");
                return AutorizationEnum.Error;
            }
        }
    }
}
