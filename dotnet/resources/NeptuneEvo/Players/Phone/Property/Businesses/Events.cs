using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Phone.Property.Businesses
{
    public class Events : Script
    {
        [RemoteEvent("server.phone.business.load")]
        public void GetData(ExtPlayer player, int bizId) => Repository.GetData(player, bizId);
        
        [RemoteEvent("server.phone.business.extraCharge")]
        public void ExtraCharge(ExtPlayer player, string name, int value) => Repository.ExtraCharge(player, name, value);
        
        [RemoteEvent("server.phone.business.addOrder")]
        public void AddOrder(ExtPlayer player, string name, int value) => Repository.AddOrder(player, name, value);
        
        [RemoteEvent("server.phone.business.cancelOrder")]
        public void CancelOrder(ExtPlayer player, int uid) => Repository.CancelOrder(player, uid);
        
        [RemoteEvent("server.phone.business.maxProducts")]
        public void MaxProducts(ExtPlayer player) => Repository.MaxProducts(player);
        
        [RemoteEvent("server.phone.business.sell")]
        public void OnSell(ExtPlayer player) => Repository.OnSell(player);
        
        
        
    }
}