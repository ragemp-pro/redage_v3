using GTANetworkAPI;

namespace NeptuneEvo.Table.Tasks.Patrolling
{
    public class Events : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void ResourceInit() =>
            Repository.ResourceInit();
    }
}