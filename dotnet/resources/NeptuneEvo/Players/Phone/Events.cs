using System;
using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Phone
{
    public class Events : Script
    {
        [Command("phonenotify")]
        public void notify(ExtPlayer player, int number, string text)
        {
            
            Trigger.ClientEvent(player, "phone.notify", number, text, 10);
        }
        [RemoteEvent("server.phone.open")]
        public void Open(ExtPlayer player)
            => Repository.Open(player);
        
        [RemoteEvent("server.phone.close")]
        public void Close(ExtPlayer player)
            => Repository.Close(player);        
        [RemoteEvent("server.phone.anim")]
        public void PhoneAnim(ExtPlayer player, int id)
            => Repository.PhoneAnim(player, id);
    }
}