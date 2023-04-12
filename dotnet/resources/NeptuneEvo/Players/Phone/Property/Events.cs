using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Phone.Property
{
    public class Events : Script
    {
        [RemoteEvent("server.phone.loadProperty")]
        public void LoadProperty(ExtPlayer player) => Repository.LoadProperty(player);
    }
}