using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Organizations.Table.Player
{
    public class Events : Script
    {
        [RemoteEvent("server.org.main.mainLoad")]
        public void MainLoad(ExtPlayer player) => 
            Repository.MainLoad(player);
        
        [RemoteEvent("server.org.main.membersLoad")]
        public void GetMembers(ExtPlayer player) => 
            Repository.GetMembers(player);
        
        [RemoteEvent("server.org.main.invitePlayer")]
        public void InvitePlayer(ExtPlayer player, string name) => 
            Repository.InvitePlayer(player, name);
        
        [RemoteEvent("server.org.main.addPlayerScore")]
        public void AddPlayerScore(ExtPlayer player, int uuid, int value) => 
            Repository.AddPlayerScore(player, uuid, value);
        
        [RemoteEvent("server.org.main.reprimand")]
        public void Reprimand(ExtPlayer player, int uuid, string name, string text) => 
            Repository.Reprimand(player, uuid, name, text);  
        
        [RemoteEvent("server.org.main.avatar")]
        public void Avatar(ExtPlayer player, string url) => 
            Repository.Avatar(player, url);  
        
        
        [RemoteEvent("server.org.main.leave")]
        public void Leave(ExtPlayer player) => 
            Repository.Leave(player);  
        
        [RemoteEvent("server.org.main.disolver")]
        public void Disolver(ExtPlayer player) => 
            Repository.Disolver(player);   
    }
}