using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Phone.Mechanic.Orders
{
    public class Events : Script
    {
        
        [RemoteEvent("server.phone.mechjob.load")]
        public void GetSelect(ExtPlayer player) => 
            Repository.GetSelect(player);
        [RemoteEvent("server.phone.mechjob.take")]
        public void OnTake(ExtPlayer player, int id) => 
            Repository.OnTake(player, id);
    }
}