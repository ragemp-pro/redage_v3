using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Phone.Property.House
{
    public class Events : Script
    {
        [RemoteEvent("server.phone.house.load")]
        public void GetHouseData(ExtPlayer player) => Repository.GetHouseData(player);
    }
}