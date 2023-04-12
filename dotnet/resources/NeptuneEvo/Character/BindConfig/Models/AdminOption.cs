using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Character.BindConfig.Models
{
    public class AdminOption
    {
        public bool ALog { get; set; } = false;
        public bool ELog { get; set; } = false;
        public bool WinLog { get; set; } = false;
        public bool AGM { get; set; } = false;
        public byte KillList { get; set; } = 0;
        public bool HideNick { get; set; } = false;
        public bool HideMe { get; set; } = false;
        public bool RedName { get; set; } = false;
        public byte ESP { get; set; } = 0;
        public bool Invisible { get; set; } = false;
    }
}
