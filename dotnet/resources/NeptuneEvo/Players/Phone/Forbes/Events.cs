using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Phone.Forbes
{
    public class Events : Script
    {
        [RemoteEvent("server.phone.forbes.load")]
        public void OnLoad(ExtPlayer player)
            => Repository.OnLoad(player);
    }
}