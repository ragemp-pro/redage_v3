using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Accounts.Registration.Models
{
    public enum RegistrationEnum
    {
        Registered,
        SocialReg,
        UserReg,
        EmailReg,
        DataError,
        Error,
        PromoError,
        PromoLimitError,
        ABError,
        LoadingError,
        ReffError
    }
}
