using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Organizations.Table.Upgrate
{
    public class Events : Script
    {
        
        [RemoteEvent("server.org.main.getUpgrate")]
        public void GetUpgrate(ExtPlayer player) => 
            Repository.GetData(player);
        
        [RemoteEvent("server.org.main.buyUpgrate")]
        public void OnBuy(ExtPlayer player, string type) => 
            Repository.OnBuy(player, type);
    }
}