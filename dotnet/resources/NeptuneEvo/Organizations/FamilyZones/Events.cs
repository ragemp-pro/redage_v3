using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Organizations.FamilyZones
{
    public class Events : Script
    {
        
        [ServerEvent(Event.ResourceStart)]
        public void OnResource() =>
            Repository.OnResource();
        
        [RemoteEvent("server.familyZoneOpen")]
        public void Open(ExtPlayer player) => 
            Repository.Open(player);
        
        [RemoteEvent("server.familyZoneAttack")]
        public void Attack(ExtPlayer player, byte id) => 
            Repository.Attack(player, id);
    }
}