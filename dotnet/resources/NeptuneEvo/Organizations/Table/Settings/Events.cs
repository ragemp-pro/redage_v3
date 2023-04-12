using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Organizations.Table.Settings
{
    public class Events : Script
    {
        [RemoteEvent("server.org.main.updateStock")]
        public void UpdateStock(ExtPlayer player) => 
            Repository.UpdateStock(player);
        [RemoteEvent("server.org.main.saveSetting")]
        public void SaveSetting(ExtPlayer player, string slogan, byte salary, string discord, int colorR, int colorG, int colorB) => 
            Repository.SaveSetting(player, slogan, salary, discord, colorR, colorG, colorB);
    }
}