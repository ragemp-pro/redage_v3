using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Fractions.Table.Tasks
{
    public class Events : Script
    {
        [RemoteEvent("server.frac.main.tasksMyLoad")]
        public void TasksMyLoad(ExtPlayer player) => 
            Repository.TasksMyLoad(player);
        [RemoteEvent("server.frac.main.tasksLoad")]
        public void TasksLoad(ExtPlayer player) => 
            Repository.TasksLoad(player);
        
        [RemoteEvent("server.frac.main.missionsLoad")]
        public void MissionsLoad(ExtPlayer player) => 
            Repository.MissionsLoad(player);
        
        [RemoteEvent("server.frac.main.missionUse")]
        public void MissionUse(ExtPlayer player, int index) => 
            Repository.MissionUse(player, index);
        
        
    }
}