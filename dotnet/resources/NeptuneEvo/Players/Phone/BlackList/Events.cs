using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Phone.BlackList
{
    public class Events : Script
    {
        [RemoteEvent("server.phone.addBlackList")]
        public void AddBlackList(ExtPlayer player, int number) => Repository.AddBlackList(player, number);
        
        [RemoteEvent("server.phone.dellBlackList")]
        public void DeleteBlackList(ExtPlayer player, int number) => Repository.DeleteBlackList(player, number);
    }
}