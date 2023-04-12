using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Players.Models
{
    public class AuntificationData
    {
        public bool IsCreateAccount { get; set; } = false;
        public string Login { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public bool IsBlockAuth { get; set; } = false;
    }
}
