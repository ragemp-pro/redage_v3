using System;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Accounts.Email.Models
{
    public class EmailVerification
    {
        public ExtPlayer Player;
        public string Login;
        public string Password;
        public string Email;
        public string Promo;
        public DateTime Time;
        public bool IsRegistered;
    }
}