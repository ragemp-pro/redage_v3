using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Character.BindConfig.Models
{
    public class ConfigData
    {
        public Dictionary<object, int> BindConfig { get; set; } = new Dictionary<object, int>();
        public string AnimFavorites { get; set; } = "";
        public string AnimBind { get; set; } = "";
        public AdminOption AdminOption { get; set; } = new AdminOption();
    }
}
