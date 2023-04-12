using System.Threading.Tasks;
using GTANetworkAPI;
using NeptuneEvo.Accounts.Registration.Models;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using Redage.SDK;

namespace NeptuneEvo.Accounts.Email.Registration
{
    public class Events : Script
    {
        private static readonly nLog Log = new nLog("NeptuneEvo.Accounts.Email.Registration");
        
        [RemoteEvent("signup")]
        public void ClientEvent_signup(ExtPlayer player, string login_, string pass_, string email_, string promo_)
        {            
            if (Players.Queue.Repository.List.Contains(player))
                return;
            
            Trigger.SetTask(async () =>
            {
                if (!Main.ServerSettings.IsEmailConfirmed)
                {
                    var sessionData = player.GetSessionData();
                    if (sessionData == null) return;
                    
                    var result = await Accounts.Registration.Repository.Register(player, login_, pass_, email_, promo_, "");
                        
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
                else
                {
                    var result = await Repository.Verification(player, login_, pass_, email_, promo_);
                
                    if (result == RegistrationEnum.Error) Accounts.Registration.Repository.MessageError(player, "Непредвиденная ошибка!");
                    else if (result == RegistrationEnum.LoadingError) Accounts.Registration.Repository.MessageError(player, "Подождите несколько секунд и попробуйте еще раз...");
                    else if (result == RegistrationEnum.SocialReg) Accounts.Registration.Repository.MessageError(player, "На этот SocialClub уже зарегистрирован игровой аккаунт!");
                    else if (result == RegistrationEnum.UserReg) Accounts.Registration.Repository.MessageError(player, "Данное имя пользователя уже занято!");
                    else if (result == RegistrationEnum.EmailReg) Accounts.Registration.Repository.MessageError(player, "Данный email уже занят!");
                    else if (result == RegistrationEnum.DataError) Accounts.Registration.Repository.MessageError(player, "Ошибка в заполнении полей!");
                    else if (result == RegistrationEnum.PromoError) Accounts.Registration.Repository.MessageError(player, "Такого промокода на данный момент не существует, введите верный или очистите поле!");
                    else if (result == RegistrationEnum.ReffError) Accounts.Registration.Repository.MessageError(player, "Мы видим, что Вы ввели реф.код друга заместо промокода стримера, поэтому просим Вас сейчас оставить поле промокода пустым, а после создания персонажа найти нужное меню в телефоне.");
                    else if (result == RegistrationEnum.PromoLimitError) Accounts.Registration.Repository.MessageError(player, "Сожалеем, но промокод превысил лимит активаций, введите другой!");
                    else if (result == RegistrationEnum.ABError) Accounts.Registration.Repository.MessageError(player, "Ошибка регистрации, используйте Ваш основной SocialClub для входа в игру.");
                }
            });
        }
    }
}