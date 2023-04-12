using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Organizations.Table.Department
{
    public class Events : Script
    {
        [RemoteEvent("server.org.main.departmentsLoad")]
        public void DepartmentsLoad(ExtPlayer player) => 
            Repository.DepartmentsLoad(player);
        
        [RemoteEvent("server.org.main.createDepartment")]
        public void CreateDepartment(ExtPlayer player, string name, string tag) => 
            Repository.CreateDepartment(player, name, tag);
        
        [RemoteEvent("server.org.main.updateDepartment")]
        public void UpdateDepartment(ExtPlayer player, int index, string name, string tag) => 
            Repository.UpdateDepartment(player, index, name, tag);

        [RemoteEvent("server.org.main.removeDepartment")]
        public void RemoveDepartment(ExtPlayer player, int index) => 
            Repository.RemoveDepartment(player, index);
        
        [RemoteEvent("server.org.main.departmentLoad")]
        public void DepartmentLoad(ExtPlayer player, int index) => 
            Repository.DepartmentLoad(player, index);
        
        [RemoteEvent("server.org.main.departmentRankLoad")]
        public void DepartmentRankLoad(ExtPlayer player, int index) => 
            Repository.DepartmentRankLoad(player, index);
        
        [RemoteEvent("server.org.main.departmentRankAccessLoad")]
        public void DepartmentRankAccessLoad(ExtPlayer player, int index, int id) => 
            Repository.DepartmentRankAccessLoad(player, index, id);


        [RemoteEvent("server.org.main.updateDepartmentRankAccess")]
        public void UpdateDepartmentRankAccess(ExtPlayer player, int index, int id, string json) => 
            Repository.UpdateDepartmentRankAccess(player, index, id, json);
        
        [RemoteEvent("server.org.main.updateDepartmentRankName")]
        public void UpdateDepartmentRankName(ExtPlayer player, int index, int id, string name) => 
            Repository.UpdateDepartmentRankName(player, index, id, name);
        
        //

        [RemoteEvent("server.org.main.setLeadersDepartment")]
        public void SetLeadersDepartment(ExtPlayer player, int departmentId, string json) => 
            Repository.SetLeadersDepartment(player, departmentId, json);
        //

        [RemoteEvent("server.org.main.deletePlayerDepartment")]
        public void DeletePlayerDepartment(ExtPlayer player, int departmentId, int uuid) => 
            Repository.DeletePlayerDepartment(player, departmentId, uuid);

        [RemoteEvent("server.org.main.invitePlayerDepartment")]
        public void InvitePlayerDepartment(ExtPlayer player, int departmentId, int uuid) => 
            Repository.InvitePlayerDepartment(player, departmentId, uuid);
    }
}