using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Phone.News
{
    public class Events : Script
    {
        [RemoteEvent("server.phone.loadNews")]
        public void LoadNews(ExtPlayer player) =>
            Repository.LoadNews(player);
        
        [RemoteEvent("server.phone.addNews")]
        public void LoadNews(ExtPlayer player, string text, string link, sbyte type, bool isPremium) => 
            Repository.Add(player, text, link, type, isPremium);
    }
}