using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Phone.Mechanic
{
    public class Events : Script
    {
        [RemoteEvent("server.phone.mech.order")]
        public void OnOrder(ExtPlayer player) => Repository.OnOrder(player);
        [RemoteEvent("server.phone.mech.cancel")]
        public void OnCancel(ExtPlayer player) => Repository.OnCancel(player);
    }
}