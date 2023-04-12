using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Phone.Gallery
{
    public class Events : Script
    {
        [RemoteEvent("server.phone.addGallery")]
        public void AddGallery(ExtPlayer player, string link) =>
            Repository.AddGallery(player, link);   
        
        [RemoteEvent("server.phone.dellGallery")]
        public void DellGallery(ExtPlayer player, string link) =>
            Repository.DellGallery(player, link);       
    }
}