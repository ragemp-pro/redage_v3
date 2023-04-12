using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Fractions.Table.Player
{
    public class Events : Script
    {
        [RemoteEvent("server.frac.main.mainLoad")]
        public void MainLoad(ExtPlayer player) => 
            Repository.MainLoad(player);
        
        [RemoteEvent("server.frac.main.membersLoad")]
        public void GetMembers(ExtPlayer player) => 
            Repository.GetMembers(player);
        
        [RemoteEvent("server.frac.main.invitePlayer")]
        public void InvitePlayer(ExtPlayer player, string name) => 
            Repository.InvitePlayer(player, name);
        
        [RemoteEvent("server.frac.main.addPlayerScore")]
        public void AddPlayerScore(ExtPlayer player, int uuid, int value) => 
            Repository.AddPlayerScore(player, uuid, value);
        
        
        [RemoteEvent("server.frac.main.reprimand")]
        public void Reprimand(ExtPlayer player, int uuid, string name, string text) => 
            Repository.Reprimand(player, uuid, name, text);  
        
        [RemoteEvent("server.frac.main.avatar")]
        public void Avatar(ExtPlayer player, string url) => 
            Repository.Avatar(player, url);   
        
        [RemoteEvent("server.frac.main.leave")]
        public void Leave(ExtPlayer player) => 
            Repository.Leave(player);   
        
    }
}