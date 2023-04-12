using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Phone.Taxi
{
    public class Events : Script
    {
        [RemoteEvent("server.phone.taxi.order")]
        public void OnOrder(ExtPlayer player) => Repository.OnOrder(player);
        [RemoteEvent("server.phone.taxi.cancel")]
        public void OnCancel(ExtPlayer player) => Repository.OnCancel(player);
    }
}