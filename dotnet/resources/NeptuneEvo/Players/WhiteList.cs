using System;
using System.Collections.Generic;
using System.Linq;
using NeptuneEvo.Core;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players
{
    public class WhiteList
    {
        public static List<string> Logins = new List<string>();

        public static bool Check(ExtPlayer player, string login)
        {
            if (login == String.Empty)
                return true;
            
            login = login.ToLower();

            if (Logins.Count > 0 && !Logins.Any(l => l.ToLower() == login))
            {
                Trigger.ClientEvent(player, "restart", "На сервере проходят технические работы. Вернёмся через несколько минут :)");
                
                var sessionData = player.GetSessionData();
                if (sessionData != null)
                {
                    Trigger.Dimension(player, Dimensions.RequestPrivateDimension(sessionData.Value));
                    
                    var auntificationData = sessionData.AuntificationData;

                    auntificationData.IsBlockAuth = true;
                }
                
                return false;
            }

            return true;
        }
    }
}