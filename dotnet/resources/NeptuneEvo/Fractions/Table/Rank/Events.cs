using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Fractions.Table.Rank
{
    public class Events : Script
    {
        [RemoteEvent("server.frac.main.rankAccessLoad")]
        public void RankAccessLoad(ExtPlayer player, int id) => 
            Repository.RankAccessLoad(player, id);
        
        [RemoteEvent("server.frac.main.updateRankAccess")]
        public void UpdateRankAccess(ExtPlayer player, int id, string json) => 
            Repository.UpdateRankAccess(player, id, json);
        
        [RemoteEvent("server.frac.main.updateRankName")]
        public void UpdateRankName(ExtPlayer player, int id, string name) => 
            Repository.UpdateRankName(player, id, name);
        
        [RemoteEvent("server.frac.main.updateRankScore")]
        public void UpdateRankScore(ExtPlayer player, int id, int score) => 
            Repository.UpdateRankScore(player, id, score);

        [RemoteEvent("server.frac.main.deletePlayer")]
        public void DeletePlayer(ExtPlayer player, int uuid) => 
            Repository.DeletePlayer(player, uuid);
        
        [RemoteEvent("server.frac.main.updateMemberRankAccess")]
        public void UpdateMemberRankAccess(ExtPlayer player, int uuid, string json) => 
            Repository.UpdateMemberRankAccess(player, uuid, json);
        
        [RemoteEvent("server.frac.main.updatePlayerRank")]
        public void UpdatePlayerRank(ExtPlayer player, int uuid, int rank) => 
            Repository.UpdatePlayerRank(player, uuid, rank);
        
        [RemoteEvent("server.frac.main.defaultRanks")]
        public void DefaultRanks(ExtPlayer player) => 
            Repository.DefaultRanks(player);
    }
}