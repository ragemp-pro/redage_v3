using System;
using System.Linq;
using Database;
using LinqToDB;
using NeptuneEvo.Core;
using NeptuneEvo.VehicleData.Models;
using Newtonsoft.Json;

namespace NeptuneEvo.Organizations.Models
{
    public class OrganizationVehicleData
    {
        public string model = "";
        public int rank = 0;
        public byte garageId = 0;
        public float dirt = 0;
        public int petrol = 0;
        public VehicleCustomization customization = null;

        public OrganizationVehicleData(string model, int rank, byte garageId, float dirt, int petrol, VehicleCustomization customization)
        {
            this.model = model;
            this.rank = rank;
            this.garageId = garageId;
            this.dirt = dirt;
            this.petrol = petrol;
            this.customization = customization;
        }

        public void SaveCustomization(string number)
        {
            Trigger.SetTask(async () =>
            {
                try
                {
	
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    await db.Orgvehicles
                        .Where(o => o.Number == number)
                        .Set(o => o.Components, JsonConvert.SerializeObject(customization))
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