using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Phone.Call
{
    public class Events : Script
    {
        [RemoteEvent("server.phone.call")]
        public void OnCall(ExtPlayer player, int number) =>
            Repository.OnCall(player, number);  
        
        [RemoteEvent("server.phone.take")]
        public void OnTake(ExtPlayer player) =>
            Repository.OnTake(player);  
        
        [RemoteEvent("server.phone.put")]
        public void OnPut(ExtPlayer player) =>
            Repository.OnPut(player);  
    }
}