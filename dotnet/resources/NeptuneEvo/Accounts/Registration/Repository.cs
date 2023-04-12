using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using MySqlConnector;
using NeptuneEvo.Accounts.Models;
using NeptuneEvo.Accounts.Registration.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Players;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeptuneEvo.Accounts.Registration
{
    class Repository
    {
        private static readonly nLog Log = new nLog("Accounts.Registration.Repository");

        public static void MessageError(ExtPlayer player, string message)
        {
            Trigger.ClientEvent(player, "client.registration.error", message);
        }

        public static async Task<RegistrationEnum> Register(ExtPlayer player, string login, string pass_, string email, string promo_, string ga)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return RegistrationEnum.LoadingError;
                if (player.IsAccountData()) return RegistrationEnum.LoadingError;
                if (sessionData.RealHWID.Equals("NONE") || sessionData.RealSocialClub.Equals("NONE")) return RegistrationEnum.LoadingError;
                if (login.Length < 1 || pass_.Length < 1 || email.Length < 1) return RegistrationEnum.DataError;

                login = login.ToLower();
                email = email.ToLower();

                await using var db = new ServerBD("MainDB");//В отдельном потоке

                var account = await db.Accounts
                    .Where(v => v.Login.ToLower() == login || v.Email.ToLower() == email || v.Socialclub == sessionData.SocialClubName || v.Socialclub == sessionData.RealSocialClub)
                    .FirstOrDefaultAsync();

                if (account != null)
                {
                    if (account.Login.ToLower() == login) return RegistrationEnum.UserReg;
                    if (account.Email.ToLower() == email) return RegistrationEnum.EmailReg;
                    if (Main.ServerNumber != 0 && (account.Socialclub == sessionData.SocialClubName || account.Socialclub == sessionData.RealSocialClub)) return RegistrationEnum.SocialReg;
                }
                promo_ = promo_.ToLower();

                if (string.IsNullOrEmpty(promo_)) promo_ = "noref";
                else if (!Main.PromoCodes.ContainsKey(promo_))
                {
                    if (int.TryParse(promo_, out int refuid)) return RegistrationEnum.PromoError;
                    if (Main.UUIDs.Contains(refuid)) return RegistrationEnum.ReffError;
                    return RegistrationEnum.PromoError;
                }
                else
                {
                    Main.PromoCodesData pcdata = Main.PromoCodes[promo_];
                    if (pcdata.RewardLimit != 0 && pcdata.RewardReceived >= pcdata.RewardLimit) return RegistrationEnum.PromoLimitError;
                    else
                    {
                        Main.PromoCodes[promo_].UsedTimes++;
                        await db.PromocodesNew
                            .Where(p => p.Promo == promo_)
                            .Set(p => p.Used, Main.PromoCodes[promo_].UsedTimes)
                            .UpdateAsync();
                    }
                }

                if (sessionData.TimersData.AutoDCTimer != null)
                {
                    Timers.Stop(sessionData.TimersData.AutoDCTimer);
                    sessionData.TimersData.AutoDCTimer = null;
                }
                var accountData = new AccountData
                {
                    Password = Accounts.Repository.GetSha256(pass_),
                    Login = login,
                    Email = email,
                    VipLvl = 0,
                    PromoCodes = new List<string>(),
                    BonusCodes = new List<string>(),
                    Chars = new List<int>()
                    {
                        -1,
                        -1,
                        -2,

                        -2,
                        -2,
                        -2,

                        -2,
                        -2,
                        -2
                    }, // -1 - empty slot, -2 - non-purchased slot
                    FreeCase = new int[3] { 0, 0, 0 },

                    HWID = sessionData.RealHWID,
                    IP = sessionData.Address,
                    SocialClub = sessionData.RealSocialClub,
                    Ga = ga
                };
                if (Main.MoneySettings.CreateVipLvl > 0)
                {
                    accountData.VipLvl = Main.MoneySettings.CreateVipLvl;
                    accountData.VipDate = DateTime.Now.AddDays(Main.MoneySettings.CreateVipDay);
                }
                accountData.RedBucks = Main.MoneySettings.CreateAccountRedBucks;
                accountData.PromoCodes.Add(promo_);
                accountData.Unique = Chars.Donate.SetUnique(null);
                player.SetAccountData(accountData);
                
                Main.LoginToEmail[accountData.Login.ToLower()] = accountData.Email;
                
                Utils.Analytics.HelperThread.AddEvent("login_new_user", accountData.Email, accountData.Ga);
                
                await db.InsertAsync(new global::Database.Accounts
                {
                    Login = accountData.Login,
                    Email = accountData.Email,
                    Password = accountData.Password,
                    Hwid = accountData.HWID,
                    Ip = accountData.IP,
                    Socialclub = accountData.SocialClub,
                    Redbucks = accountData.RedBucks,
                    Viplvl = accountData.VipLvl,
                    Vipdate = accountData.VipDate,
                    Promocodes = JsonConvert.SerializeObject(accountData.PromoCodes),
                    Bonuscodes = JsonConvert.SerializeObject(accountData.BonusCodes),
                    Character1 = -1,
                    Character2 = -1,
                    Character3 = -2,
                    Characters = "[-2,-2,-2,-2,-2,-2]",
                    @case = JsonConvert.SerializeObject(accountData.FreeCase),
                    CollectionGifts = "[]",
                    ReceivedAward = JsonConvert.SerializeObject(accountData.ReceivedAward),
                    Unique = accountData.Unique,
                    Ga = ga
                });
                GameLog.AccountLog(accountData.Login, accountData.HWID, accountData.IP, accountData.SocialClub, $"Время начать?");
                
                Main.Usernames[accountData.Login] = accountData.Chars;
                
                if (WhiteList.Check(player, accountData.Login))
                {
                    LoadCharacter.Repository.Load(player, DateTime.Now, isReg: true);

                    Main.SetUpEverything(player);
                }
                
                return RegistrationEnum.Registered;
            }
            catch (Exception e)
            {
                Log.Write($"Register Exception: {e.ToString()}");
                return RegistrationEnum.Error;
            }
        }
    }
}
