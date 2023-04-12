using System;
using System.Linq;
using System.Threading.Tasks;
using Database;
using LinqToDB;
using NeptuneEvo.Players.Models;
using Newtonsoft.Json;

namespace NeptuneEvo.Fractions.Models
{
    public class FractionMemberData : MemberData
    {
        public DateTime PatrollingTime = DateTime.MinValue;
        
        public async Task Save(ServerBD db)
        {
            try
            {
                IsSave = false;
                
                await db.Fracranks
                    .Where(f => f.Uuid == this.UUID)
                    .Set(f => f.Name, this.Name)
                    .Set(f => f.Rank, this.Rank)
                    .Set(f => f.Avatar, this.Avatar)
                    .Set(f => f.DepartmentId, this.DepartmentId)
                    .Set(f => f.DepartmentRank, this.DepartmentRank)
                    .Set(f => f.Access, JsonConvert.SerializeObject(this.Access))
                    .Set(f => f.@lock, JsonConvert.SerializeObject(this.Lock))
                    .Set(f => f.Score, this.Score)
                    .Set(f => f.LastLoginDate, DateTime.Now)
                    .Set(f => f.Time, JsonConvert.SerializeObject(this.Time))
                    .Set(f => f.Tasks, JsonConvert.SerializeObject(this.TasksData))
                    .UpdateAsync();
                
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
    }
}