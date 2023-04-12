using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Phone.Settings
{
    public class Events : Script
    {
        [RemoteEvent("server.phone.settings.removeSim")]
        public void OnRemoveSim(ExtPlayer player) => 
            Repository.OnRemoveSim(player);
        
        [RemoteEvent("server.phone.settings.air")]
        public void OnUpdateAir(ExtPlayer player) => 
            Repository.OnUpdateAir(player);
        
        [RemoteEvent("server.phone.settings.forbesVisible")]
        public void OnUpdateForbesVisible(ExtPlayer player) => 
            Repository.OnUpdateForbesVisible(player);
        
        [RemoteEvent("server.phone.settings.avatar")]
        public void OnUpdateAvatar(ExtPlayer player, string avatar) => 
            Repository.OnUpdateAvatar(player, avatar);
        
        [RemoteEvent("server.phone.settings.wallpaper")]
        public void OnUpdateWallpaper(ExtPlayer player, string wallpaper) => 
            Repository.OnUpdateWallpaper(player, wallpaper);
        
        [RemoteEvent("server.phone.settings.bellId")]
        public void OnUpdateBellId(ExtPlayer player, int bellId) => 
            Repository.OnUpdateBellId(player, bellId);
        
        [RemoteEvent("server.phone.settings.smsId")]
        public void OnUpdateSmsId(ExtPlayer player, int smsId) => 
            Repository.OnUpdateSmsId(player, smsId);
    }
}