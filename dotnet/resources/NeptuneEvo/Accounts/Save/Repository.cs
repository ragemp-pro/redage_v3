using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using NeptuneEvo.Core;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NeptuneEvo.Accounts.Save
{
    public class Repository
    {
        private static readonly nLog Log = new nLog("Accounts.Save.Repository");
        public static async Task SaveSql(ServerBD db, ExtPlayer player)
        {
            try
            {
                var accountData = player.GetAccountData();
                if (accountData == null) return;

                await Players.Session.Repository.SaveSession(db, player);
                
                var accountSave = db.Accounts
                    .Where(v => v.Login == accountData.Login)
                    .Set(v => v.Password, accountData.Password)
                    .Set(v => v.Email, accountData.Email)
                    .Set(v => v.Socialclub, accountData.SocialClub)
                    .Set(v => v.Redbucks, accountData.RedBucks)
                    .Set(v => v.Viplvl, accountData.VipLvl)
                    .Set(v => v.Hwid, accountData.HWID)
                    .Set(v => v.Ip, accountData.IP)
                    .Set(v => v.Vipdate, accountData.VipDate)
                    .Set(v => v.Promocodes, JsonConvert.SerializeObject(accountData.PromoCodes))
                    .Set(v => v.Bonuscodes, JsonConvert.SerializeObject(accountData.BonusCodes))
                    .Set(v => v.Character1, accountData.Chars[0])
                    .Set(v => v.Character2, accountData.Chars[1])
                    .Set(v => v.Character3, accountData.Chars[2])
                    .Set(v => v.Present, Convert.ToSByte(accountData.PresentGet))
                    .Set(v => v.Refpresent, Convert.ToSByte(accountData.RefPresentGet))
                    .Set(v => v.@case, JsonConvert.SerializeObject(accountData.FreeCase))
                    .Set(v => v.IsSubscribe, accountData.isSubscribe)
                    .Set(v => v.SubscribeEndTime, accountData.SubscribeEndTime)
                    .Set(v => v.SubscribeTime, accountData.SubscribeTime)
                    .Set(v => v.CollectionGifts, JsonConvert.SerializeObject(accountData.CollectionGifts))
                    .Set(v => v.ReceivedAward, JsonConvert.SerializeObject(accountData.ReceivedAward))
                    .Set(v => v.ReceivedAwardWeek, accountData.ReceivedAwardWeek)
                    .Set(v => v.ReceivedAwardDonate, accountData.ReceivedAwardDonate)
                    .Set(v => v.Unique, accountData.Unique)
                    .Set(v => v.LastSelectCharUUID, accountData.LastSelectCharUUID)
                    .Set(v => v.ExitDate, DateTime.Now);

                if (Main.ServerSettings.IsMerger)
                    accountSave = accountSave
                        .Set(v => v.Characters,
                            JsonConvert.SerializeObject(new List<int>()
                            {
                                accountData.Chars[3], accountData.Chars[4], accountData.Chars[5], accountData.Chars[6],
                                accountData.Chars[7], accountData.Chars[8]
                            }));
                    
                    
                await accountSave
                    .UpdateAsync();

                if (Admin.IsServerStoping && player != null)
                    player.IsRestartSaveAccountData = true;

            }
            catch (Exception e)
            {
                Log.Write($"SaveSql Exception: {e.ToString()}");
            }
        }
        public static void SaveReceived(ExtPlayer player)
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    var accountData = player.GetAccountData();
                    if (accountData == null) return;
                
                    await using var db = new ServerBD("MainDB");//В отдельном потоке
                
                    await db.Accounts
                        .Where(v => v.Login == accountData.Login)
                        .Set(v => v.ReceivedAward, JsonConvert.SerializeObject(accountData.ReceivedAward))
                        .Set(v => v.ReceivedAwardWeek, accountData.ReceivedAwardWeek)
                        .Set(v => v.ReceivedAwardDonate, accountData.ReceivedAwardDonate)
                        .UpdateAsync();
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
        }
    }
}
