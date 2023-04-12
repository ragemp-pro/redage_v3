using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using LinqToDB;
using NeptuneEvo.Accounts.Registration.Models;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using Redage.SDK;

namespace NeptuneEvo.Accounts.Email.Registration
{
    public class Repository
    {
                
        private static readonly nLog Log = new nLog("Accounts.Email.Registration.Repository");

        public static async Task<RegistrationEnum> Verification(ExtPlayer player, string login, string password, string email, string promo)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return RegistrationEnum.LoadingError;
                if (player.IsAccountData()) return RegistrationEnum.LoadingError;
                if (sessionData.RealHWID.Equals("NONE") || sessionData.RealSocialClub.Equals("NONE")) return RegistrationEnum.LoadingError;
                if (login.Length < 1 || password.Length < 1 || email.Length < 1) return RegistrationEnum.DataError;

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
                promo = promo.ToLower();

                if (!string.IsNullOrEmpty(promo))
                {
                    if (!Main.PromoCodes.ContainsKey(promo))
                    {
                        if (int.TryParse(promo, out int refuid)) return RegistrationEnum.PromoError;
                        if (Main.UUIDs.Contains(refuid)) return RegistrationEnum.ReffError;
                        return RegistrationEnum.PromoError;
                    }
                    else
                    {
                        var pcdata = Main.PromoCodes[promo];
                        if (pcdata.RewardLimit != 0 && pcdata.RewardReceived >= pcdata.RewardLimit) return RegistrationEnum.PromoLimitError;
                    }
                }

                var hash = await Email.Repository.Add(player, login, password, email, promo, type: 0);

                Utils.Analytics.HelperThread.AddUrl($"verify?email={email}&name={login}&hash={hash}&sid={Main.ServerNumber}");
                
                Trigger.ClientEvent(player, "client.registration.sendEmail");
                return RegistrationEnum.Registered;
            }
            catch (Exception e)
            {
                Log.Write($"Register Exception: {e.ToString()}");
                return RegistrationEnum.Error;
            }
        }
        
        public static void VerificationConfirm(string hash, string ga)
        {
            var emailVerification = Email.Repository.GetVerification(hash, isRegistered: true);
            
            if (emailVerification != null)
            {
                var player = emailVerification.Player;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        var result = await Accounts.Registration.Repository.Register(player, emailVerification.Login, emailVerification.Password, emailVerification.Email, emailVerification.Promo, ga);
                        
                        if (result == RegistrationEnum.Error) Accounts.Registration.Repository.MessageError(player,  "Непредвиденная ошибка!");
                        else if (result == RegistrationEnum.LoadingError) Accounts.Registration.Repository.MessageError(player,  "Подождите несколько секунд и попробуйте еще раз...");
                        else if (result == RegistrationEnum.SocialReg) Accounts.Registration.Repository.MessageError(player,  "На этот SocialClub уже зарегистрирован игровой аккаунт!");
                        else if (result == RegistrationEnum.UserReg) Accounts.Registration.Repository.MessageError(player,  "Данное имя пользователя уже занято!");
                        else if (result == RegistrationEnum.EmailReg) Accounts.Registration.Repository.MessageError(player,  "Данный email уже занят!");
                        else if (result == RegistrationEnum.DataError) Accounts.Registration.Repository.MessageError(player,  "Ошибка в заполнении полей!");
                        else if (result == RegistrationEnum.PromoError) Accounts.Registration.Repository.MessageError(player,  "Такого промокода на данный момент не существует, введите верный или очистите поле!");
                        else if (result == RegistrationEnum.ReffError) Accounts.Registration.Repository.MessageError(player,  "Мы видим, что Вы ввели реф.код друга заместо промокода стримера, поэтому просим Вас сейчас оставить поле промокода пустым, а после создания персонажа найти нужное меню в телефоне.");
                        else if (result == RegistrationEnum.PromoLimitError) Accounts.Registration.Repository.MessageError(player,  "Сожалеем, но промокод превысил лимит активаций, введите другой!");
                        else if (result == RegistrationEnum.ABError) Accounts.Registration.Repository.MessageError(player,  "Ошибка регистрации, используйте Ваш основной SocialClub для входа в игру.");
                        Log.Write($"{sessionData.Name} ({sessionData.SocialClubName} | {sessionData.RealSocialClub}) tryed to signup.");

                    }
                    catch (Exception e)
                    {
                        Log.Write($"ClientEvent_signup Exception: {e.ToString()}");
                    }
                });
            }
        }
        
    }
}