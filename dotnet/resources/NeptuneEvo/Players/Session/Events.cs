using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Session
{
    public class Events : Script
    {
        [RemoteEvent("server.session.update")]
        public void UpdateSession(ExtPlayer player)
        {
            Repository.UpdateSession(player);
        }
    }
}