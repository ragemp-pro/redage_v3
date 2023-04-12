using System;
using System.Linq;
using Database;
using GTANetworkAPI;
using LinqToDB;

namespace NeptuneEvo.Organizations.FamilyZones.Models
{
    public class FamilyZoneData
    {
        public byte Id;
        public string Name;
        public int OrganizationId;
        public Vector3 Position;

        public FamilyZoneData(byte id, string name, int organizationId, Vector3 position)
        {
            this.Id = id;
            this.Name = name;
            this.OrganizationId = organizationId;
            this.Position = position;
        }
        
        public void Save()
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    await db.Familyzones
                        .Where(v => v.Id == this.Id)
                        .Set(v => v.Orgid,  Convert.ToInt16(this.OrganizationId))
                        .UpdateAsync();
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
        }
    }
}