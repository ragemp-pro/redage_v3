using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using LinqToDB;
using NeptuneEvo.Accounts.Email.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using Redage.SDK;

namespace NeptuneEvo.Accounts.Email
{
    public class Repository
    {
        private static Dictionary<string, EmailVerification> EmailsVerification =
            new Dictionary<string, EmailVerification>();


        public static async Task<string> Add(ExtPlayer player, string login, string password, string email, string promo, bool isRegistered = true, int type = 0)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return String.Empty;
            
            var hash = EmailsVerification
                .Where(ev => ev.Value.Player == player)
                .Select(ev => ev.Key)
                .FirstOrDefault();
                
            if (hash != null && EmailsVerification.ContainsKey(hash))
                EmailsVerification.Remove(hash);

            hash = Accounts.Repository.GetSha256($"{DateTime.Now.Ticks}_{Main.ServerNumber}_{login}_{email}");
            var time = DateTime.Now.AddMinutes(15);
            /*
            await using var webSiteBD = new WebSiteBD("WebSiteBD");

            await webSiteBD
                .VerifyConfirm
                .Where(vc => vc.Socialclub == sessionData.RealSocialClub)
                .DeleteAsync();
            
            await webSiteBD.InsertAsync(new VerifyConfirms
            {
                Hash = hash,
                Email = email,
                Socialclub = sessionData.RealSocialClub,
                Data = time,
                ServerId = (sbyte) Main.ServerNumber,
                Type = (sbyte) type
            });
                */
            EmailsVerification.Add(hash, new EmailVerification
            {
                Player = player,
                Login = login,
                Password = password,
                Email = email,
                Promo = promo,
                Time = time,
                IsRegistered = isRegistered
            });

            return hash;
        }
        
        public static void VerificationDelete(ExtPlayer player)
        {                
            
            var hash = EmailsVerification
                .Where(ev => ev.Value.Player == player)
                .Select(ev => ev.Key)
                .FirstOrDefault();

            if (hash != null && EmailsVerification.ContainsKey(hash))
            {
                EmailsVerification.Remove(hash);            
                /*
                Trigger.SetTask(async () =>
                {
                    await using var webSiteBD = new WebSiteBD("WebSiteBD");
                    
                    await webSiteBD
                        .VerifyConfirm
                        .Where(vc => vc.ServerId == Main.ServerNumber)
                        .Where(vc => vc.Hash == hash)
                        .DeleteAsync();
                });
                */
            }
        }
        public static async Task VerificationsDelete()
        {            
            /*
            await using var webSiteBD = new WebSiteBD("WebSiteBD");
                    
            await webSiteBD
                .VerifyConfirm
                .Where(vc => vc.ServerId == Main.ServerNumber)
                .DeleteAsync();
            */
        }
        public static EmailVerification GetVerification(string hash, bool isRegistered = true)
        {
            if (EmailsVerification.ContainsKey(hash))
            {
                var emailVerification = EmailsVerification[hash];

                if (emailVerification.IsRegistered == isRegistered)
                {
                    EmailsVerification.Remove(hash);

                    return emailVerification;
                }
            }

            return null;
        }
        
        public static async Task DeleteToTime()
        {
            var confirms = EmailsVerification
                .Where(ev => ev.Value.Time < DateTime.Now)
                .Select(ev => ev.Key)
                .ToList();

            if (confirms.Count > 0)
            {
                /*
                await using var webSiteBD = new WebSiteBD("WebSiteBD");

                await webSiteBD.VerifyConfirm
                    .Where(vc => vc.ServerId == Main.ServerNumber)
                    .Where(vc => vc.Data < DateTime.Now)
                    .DeleteAsync();
                */
                Trigger.SetMainTask(() =>
                {
                    foreach (var hash in confirms)
                    {
                        var emailVerification = GetVerification(hash);

                        if (emailVerification != null && emailVerification.Player != null)
                        {
                            if (!emailVerification.IsRegistered) 
                                Notify.Send(emailVerification.Player, NotifyType.Success, NotifyPosition.BottomCenter, "Истекло время на подтверждение почты. Попробуйте еще раз!", 5000);
                            else 
                                Accounts.Registration.Repository.MessageError(emailVerification.Player, "Истекло время на подтверждение почты. Попробуйте еще раз!");
                        }
                    }
                });
            }

            //Trigger.ClientEvent(foreachPlayer, "client.roullete.updateCase", 2);

        }
    }
}