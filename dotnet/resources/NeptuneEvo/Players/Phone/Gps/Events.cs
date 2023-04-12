using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Phone.Gps
{
    public class Events : Script
    {
        [RemoteEvent("gps.pointDefault")]
        public void OnPointDefault(ExtPlayer player, string name) => Repository.OnPointDefault(player, name);
    }
}