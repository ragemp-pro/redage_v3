using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Houses.Rieltagency
{
    public class Events : Script
    { 
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            Repository.OnResourceStart();
        }
        [RemoteEvent("server.rieltagency.buy")]
        private void OnBuyInfo(ExtPlayer player, int id, int type)
        {
            Repository.OnBuy(player, id, type);
        }

        [RemoteEvent("server.rieltagency.close")]
        private void OnClose(ExtPlayer player)
        {
            Repository.OnClose(player);
        }
    }
}