using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;

namespace NeptuneEvo.World.War
{
    public class Events : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void OnResource() =>
            Repository.OnResource();


        [RemoteEvent("server.war")]
        public void War(ExtPlayer player, sbyte typeBattle, sbyte composition, sbyte weaponsCategory, sbyte day,
            sbyte hour, sbyte min)
        {
            var sessionData = player.GetSessionData(); 
            if (sessionData == null) 
                return;
            
            Repository.War(player, sessionData.WarData, typeBattle, composition, weaponsCategory, day, hour, min);
        }
    }
}