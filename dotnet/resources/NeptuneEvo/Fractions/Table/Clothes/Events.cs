using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Fractions.Table.Clothes
{
    public class Events : Script
    {
        [RemoteEvent("server.frac.main.clothesLoad")]
        public void ClothesLoad(ExtPlayer player) => 
            Repository.ClothesLoad(player);
        
        [RemoteEvent("server.frac.main.clothesUpdate")]
        public void ClothesUpdate(ExtPlayer player, string name, string newName, int rank, bool gender) => 
            Repository.ClothesUpdate(player, name, newName, rank, gender);
        
        [RemoteEvent("server.table.editClothingSet")]
        public void EditClothingSet(ExtPlayer player, string name, int id, int texture) => 
            Repository.EditClothingSet(player, name, id, texture);

        
        
        
    }
}