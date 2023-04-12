using System.Collections;
using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Inventory.Drop
{
    public class Events : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            Repository.OnResourceStart();
        }
        
        [RemoteEvent("server.raise")]
        private void OnRaise(ExtPlayer player, ExtObject obj)
        {
            Repository.ItemRaise(player, obj);
        }
        
        [RemoteEvent("server.hookahManage")]
        private void OnHookah(ExtPlayer player, ExtObject obj)
        {
            Repository.OnHookah(player, obj);
        }
    }
}