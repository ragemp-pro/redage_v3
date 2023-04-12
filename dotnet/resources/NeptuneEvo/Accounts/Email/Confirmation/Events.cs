using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GTANetworkAPI;
using NeptuneEvo.Accounts.Email.Confirmation.Models;
using NeptuneEvo.Handles;
using Redage.SDK;

namespace NeptuneEvo.Accounts.Email.Confirmation
{
    public class Events : Script
    {

        [Command("emailconfirm")]
        public void emailconfirm(ExtPlayer player, string email)
        {        
            EmailConfirm(player, email);
        }

        [RemoteEvent("server.email.confirm")]
        public void EmailConfirm(ExtPlayer player, string email)
        {                
            var accountData = player.GetAccountData();
            if (accountData == null) 
                return;
            
            var rg = new Regex(@"[0-9]{8,11}[.][0-9]{8,11}", RegexOptions.IgnoreCase);
            
            if (rg.IsMatch(accountData.Ga))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы уже подтвердили почту!", 3000);
                return;
            }

            Trigger.SetTask(async () =>
            {
                var result = await Repository.Confirm(player, email);

                if (result == EmailConfirmEnum.Error)
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Непредвиденная ошибка!", 5000);
                else if (result == EmailConfirmEnum.LoadingError)
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                        "Подождите несколько секунд и попробуйте еще раз...", 5000);
                else if (result == EmailConfirmEnum.EmailReg)
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Данный email уже занят!", 5000);
                else if (result == EmailConfirmEnum.DataError)
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Ошибка в заполнении полей!",
                        5000);
                else if (result == EmailConfirmEnum.Success)
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter,
                        "На почту отправлено письмо со ссылкой для подтверждения аккаунта, которая будет действительна 15 минут.",
                        5000);
            });
        }
    }
}