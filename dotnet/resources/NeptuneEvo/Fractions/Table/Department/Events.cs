using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Fractions.Table.Department
{
    public class Events : Script
    {
        [RemoteEvent("server.frac.main.departmentsLoad")]
        public void DepartmentsLoad(ExtPlayer player) => 
            Repository.DepartmentsLoad(player);
        
        [RemoteEvent("server.frac.main.createDepartment")]
        public void CreateDepartment(ExtPlayer player, string name, string tag) => 
            Repository.CreateDepartment(player, name, tag);
        
        [RemoteEvent("server.frac.main.updateDepartment")]
        public void UpdateDepartment(ExtPlayer player, int index, string name, string tag) => 
            Repository.UpdateDepartment(player, index, name, tag);

        [RemoteEvent("server.frac.main.removeDepartment")]
        public void RemoveDepartment(ExtPlayer player, int index) => 
            Repository.RemoveDepartment(player, index);
        
        [RemoteEvent("server.frac.main.departmentLoad")]
        public void DepartmentLoad(ExtPlayer player, int index) => 
            Repository.DepartmentLoad(player, index);
        
        [RemoteEvent("server.frac.main.departmentRankLoad")]
        public void DepartmentRankLoad(ExtPlayer player, int index) => 
            Repository.DepartmentRankLoad(player, index);

        [RemoteEvent("server.frac.main.departmentRankAccessLoad")]
        public void DepartmentRankAccessLoad(ExtPlayer player, int index, int id) => 
            Repository.DepartmentRankAccessLoad(player, index, id);
       
        [RemoteEvent("server.frac.main.updateDepartmentRankAccess")]
        public void UpdateDepartmentRankAccess(ExtPlayer player, int index, int id, string json) => 
            Repository.UpdateDepartmentRankAccess(player, index, id, json);
        
        [RemoteEvent("server.frac.main.updateDepartmentRankName")]
        public void UpdateDepartmentRankName(ExtPlayer player, int index, int id, string name) => 
            Repository.UpdateDepartmentRankName(player, index, id, name);
        
        //

        [RemoteEvent("server.frac.main.setLeadersDepartment")]
        public void SetLeadersDepartment(ExtPlayer player, int departmentId, string json) => 
            Repository.SetLeadersDepartment(player, departmentId, json);
        //

        [RemoteEvent("server.frac.main.deletePlayerDepartment")]
        public void DeletePlayerDepartment(ExtPlayer player, int departmentId, int uuid) => 
            Repository.DeletePlayerDepartment(player, departmentId, uuid);

        [RemoteEvent("server.frac.main.invitePlayerDepartment")]
        public void InvitePlayerDepartment(ExtPlayer player, int departmentId, int uuid) => 
            Repository.InvitePlayerDepartment(player, departmentId, uuid);
    }
}