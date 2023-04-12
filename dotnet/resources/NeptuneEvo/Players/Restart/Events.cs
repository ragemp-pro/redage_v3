using System.Linq;
using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Restart
{
    public class Events : Script
    {
        [RemoteEvent("restart.add")]
        public void Add(ExtPlayer player, string text) => Restart.Repository.Add(player, text);
    }
}