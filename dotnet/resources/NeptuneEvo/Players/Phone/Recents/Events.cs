using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Phone.Recents
{
    public class Events : Script
    {
        [RemoteEvent("server.phone.recentsClear")]
        public void RecentsClear(ExtPlayer player) => Repository.Clear(player);
    }
}