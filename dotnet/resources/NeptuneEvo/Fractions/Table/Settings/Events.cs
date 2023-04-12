using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Fractions.Table.Settings
{
    public class Events : Script
    {
        [RemoteEvent("server.frac.main.updateStock")]
        public void UpdateStock(ExtPlayer player) => 
            Repository.UpdateStock(player);
        
        [RemoteEvent("server.frac.main.updateGunStock")]
        public void UpdateGunStock(ExtPlayer player) => 
            Repository.UpdateGunStock(player);
    }
}