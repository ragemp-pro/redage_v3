using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Phone.Cars
{
    public class Events : Script
    {
        [RemoteEvent("server.phone.cars.load")]
        public void GetData(ExtPlayer player) => Repository.GetCarData(player);
    }
}