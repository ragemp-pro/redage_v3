using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using LinqToDB;
using NeptuneEvo.Accounts.Email.Confirmation.Models;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using NeptuneEvo.Players.Phone.Messages.Models;
using Redage.SDK;

namespace NeptuneEvo.Accounts.Email.Confirmation
{
    public class Repository
    {
                
        private static readonly nLog Log = new nLog("Accounts.Email.Confirmation.Repository");
        
        
        public static async Task<EmailConfirmEnum> Confirm(ExtPlayer player, string email)
        {
            try
            {       
                var accountData = player.GetAccountData();
                if (accountData == null) 
                    return EmailConfirmEnum.LoadingError;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return EmailConfirmEnum.LoadingError;
                
                if (email.Length < 1) 
                    return EmailConfirmEnum.DataError;

                email = email.ToLower();

                await using var db = new ServerBD("MainDB");//В отдельном потоке

                var account = await db.Accounts
                    .Where(v => v.Login != accountData.Login && v.Email.ToLower() == email)
                    .FirstOrDefaultAsync();

                if (account != null) 
                    return EmailConfirmEnum.EmailReg;              
                
                var hash = await Email.Repository.Add(player, String.Empty, String.Empty, email, String.Empty, isRegistered: false, type: 1);

                Utils.Analytics.HelperThread.AddUrl($"verify?email={email}&name={accountData.Login}&hash={hash}&sid={Main.ServerNumber}");
                
                return EmailConfirmEnum.Success;
            }
            catch (Exception e)
            {
                Log.Write($"Register Exception: {e.ToString()}");
                return EmailConfirmEnum.Error;
            }
        }        
        
        public static void VerificationConfirm(string hash, string ga)
        {
            var emailVerification = Email.Repository.GetVerification(hash, isRegistered: false);
            
            if (emailVerification != null)
            {
                var player = emailVerification.Player;
                
                var accountData = player.GetAccountData();
                if (accountData == null) 
                    return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                accountData.Ga = ga;
                accountData.Email = emailVerification.Email;
                Main.LoginToEmail[accountData.Login.ToLower()] = accountData.Email;
                Trigger.ClientEvent(player, "client.accountStore.Email", accountData.Email); 
                Trigger.ClientEvent(player, "client.accountStore.Ga", accountData.Ga); 
                
                Chars.Repository.AddNewItemWarehouse(player, ItemId.Case4, 1);
                //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Почта подтверждена успешно. Приз отправлен на склад. (M->GPS - > Склад)", 5000);
                Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.RedAge, $"Почта подтверждена успешно. Приз отправлен на склад. (M->GPS - > Склад)", DateTime.Now);
                
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");

                        await db.Accounts
                            .Where(v => v.Login == accountData.Login)
                            .Set(v => v.Email, accountData.Email)
                            .Set(v => v.Ga, accountData.Ga)
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
}