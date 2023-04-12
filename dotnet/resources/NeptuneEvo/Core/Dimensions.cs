using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Core
{
    class Dimensions : Script
    {
        public static uint RequestPrivateDimension(int myid)
        {
            return (uint)(10000+myid);
        }
    }
}
