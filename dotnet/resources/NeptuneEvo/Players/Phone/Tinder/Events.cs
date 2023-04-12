using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Players.Phone.Tinder.Models;

namespace NeptuneEvo.Players.Phone.Tinder
{
    public class Events : Script
    {
        [RemoteEvent("server.phone.tinder.load")]
        public void OnLoad(ExtPlayer player) => 
            Repository.Init(player);
        
        [RemoteEvent("server.phone.tinder.action")]
        public void OnAction(ExtPlayer player, int uuid, bool isLove) => 
            Repository.OnAction(player, uuid, isLove);
        
        [RemoteEvent("server.phone.tinder.save")]
        public void OnSave(ExtPlayer player, string avatar, string text, int type, bool isVisible) => 
            Repository.OnSave(player, avatar, text, (TinderType) type, isVisible);
    }
} 