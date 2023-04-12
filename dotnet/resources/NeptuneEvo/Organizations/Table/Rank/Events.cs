using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Organizations.Table.Rank
{
    public class Events : Script
    {
        
        
        [RemoteEvent("server.org.main.rankAccessLoad")]
        public void RankAccessLoad(ExtPlayer player, int id) => 
            Repository.RankAccessLoad(player, id);
        
        [RemoteEvent("server.org.main.updateRanksId")]
        public void UpdateRanksId(ExtPlayer player, string json) => 
            Repository.UpdateRanksId(player, json);
        
        [RemoteEvent("server.org.main.updateRankAccess")]
        public void UpdateRankAccess(ExtPlayer player, int id, string json) => 
            Repository.UpdateRankAccess(player, id, json);
        
        [RemoteEvent("server.org.main.createRank")]
        public void CreateRank(ExtPlayer player, string name, int score) => 
            Repository.CreateRank(player, name, score);
        
        [RemoteEvent("server.org.main.removeRank")]
        public void RemoveRank(ExtPlayer player, int id) => 
            Repository.RemoveRank(player, id);
        
        [RemoteEvent("server.org.main.updateRankName")]
        public void UpdateRankName(ExtPlayer player, int id, string name) => 
            Repository.UpdateRankName(player, id, name);
        
        [RemoteEvent("server.org.main.updateRankScore")]
        public void UpdateRankScore(ExtPlayer player, int id, int score) => 
            Repository.UpdateRankScore(player, id, score);

        [RemoteEvent("server.org.main.deletePlayer")]
        public void DeletePlayer(ExtPlayer player, int uuid) => 
            Repository.DeletePlayer(player, uuid);
        
        [RemoteEvent("server.org.main.updateMemberRankAccess")]
        public void UpdateMemberRankAccess(ExtPlayer player, int uuid, string json) => 
            Repository.UpdateMemberRankAccess(player, uuid, json);
        
        [RemoteEvent("server.org.main.updatePlayerRank")]
        public void UpdatePlayerRank(ExtPlayer player, int uuid, int rank) => 
            Repository.UpdatePlayerRank(player, uuid, rank);
        
        [RemoteEvent("server.org.main.defaultRanks")]
        public void DefaultRanks(ExtPlayer player) => 
            Repository.DefaultRanks(player);
        
                
        [RemoteEvent("server.org.main.setLeader")]
        public void SetLeader(ExtPlayer player, int uuid) => 
            Repository.SetLeader(player, uuid);
        
    }
}