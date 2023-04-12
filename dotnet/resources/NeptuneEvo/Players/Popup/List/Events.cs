using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Popup.List
{
    public class Events : Script
    {
        [RemoteEvent("popup.list.callback")]
        public void Callback(ExtPlayer player, object listItem) =>
            Repository.Callback(player, listItem);
    }
}