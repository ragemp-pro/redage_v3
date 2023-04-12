using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Phone.Property.Businesses.Orders
{
    public class Events : Script
    {
        [RemoteEvent("server.phone.truck.load")]
        public void Open(ExtPlayer player) => Repository.Open(player);
        
        [RemoteEvent("server.phone.truck.take")]
        public void Take(ExtPlayer player, int uid) => Repository.Take(player, uid);
        
        [RemoteEvent("server.phone.truck.cancel")]
        public void Cancel(ExtPlayer player) => Repository.Cancel(player);
    }
}