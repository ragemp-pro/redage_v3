using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using GTANetworkAPI;
using LinqToDB;
using MySqlConnector;
using NeptuneEvo.Chars;
using NeptuneEvo.Handles;
using NeptuneEvo.Table.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Organizations.Models
{
    public class OrganizationData : TableData
    {
        public int OwnerUUID { get; set; } = -1;
        
        public byte Salary = 0;
        
        public Color Color = new Color();
        public byte OfficeUpgrade { get; set; } = 0;
        public bool Stock { get; set; } = false;
        public bool CrimeOptions { get; set; } = false;
        public Dictionary<string, bool> Schemes { get; set; } = new Dictionary<string, bool>()
        {
            {"Pistol", false},
            {"PistolMk2", false},
            {"Pistol50", false},
            {"HeavyPistol", false},
            {"PumpShotgun", false},
            {"DoubleBarrelShotgun", false},
            {"SawnOffShotgun", false},
            {"MiniSMG", false},
            {"SMGMk2", false},
            {"MachinePistol", false},
            {"MicroSMG", false},
            {"CombatPDW", false},
            {"CompactRifle", false},
            {"AssaultRifle", false},
            {"Armor", false},
        };
        public bool Status { get; set; }
        public bool[] Used { get; set; } = new bool[15] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
        public ExtBlip Blip { get; set; } = null;
        public int BlipId { get; set; }  = -1;
        public byte BlipColor { get; set; } = 0;
        public Vector3 BlipPosition { get; set; } = new Vector3();

        public Dictionary<string, OrganizationVehicleData> Vehicles =
            new Dictionary<string, OrganizationVehicleData>();
        public string Slogan = "";
        public DateTime Date;
        public Dictionary<int, byte> AttackingCount = new Dictionary<int, byte>();
        public Dictionary<int, byte> ProtectingCount = new Dictionary<int, byte>();
        
        public async Task Save(ServerBD db)
        {
            await db.Organizations
                .Where(o => o.Organization == this.Id)
                .Set(o => o.Drugs, this.Drugs)
                .Set(o => o.Mats, this.Materials)
                .Set(o => o.MedKits, this.MedKits)
                .Set(o => o.Money, this.Money)
                .Set(o => o.IsOpen, Convert.ToSByte(this.IsOpenStock))
                .Set(o => o.AttackingCount, JsonConvert.SerializeObject(this.AttackingCount))
                .Set(o => o.ProtectingCount, JsonConvert.SerializeObject(this.ProtectingCount))
                .UpdateAsync();
        }

        public void SaveRank()
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    await db.Organizations
                        .Where(v => v.Organization == this.Id)
                        .Set(v => v.Ranks, JsonConvert.SerializeObject(this.Ranks))
                        .UpdateAsync();
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
        }
        
        public void SaveLeader()
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    await db.Organizations
                        .Where(v => v.Organization == this.Id)
                        .Set(v => v.OwnerUUID, this.OwnerUUID)
                        .UpdateAsync();
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
        }
        
        public uint GetDimension() =>
            (uint) (Organizations.Manager.DefaultDimension + Id);

        public void SaveDepartment()
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    await db.Organizations
                        .Where(v => v.Organization == this.Id)
                        .Set(v => v.Departments, JsonConvert.SerializeObject(this.Departments))
                        .UpdateAsync();
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
        }

        public void SaveSettings()
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    await db.Organizations
                        .Where(v => v.Organization == this.Id)
                        .Set(v => v.Discord, this.Discord)
                        .Set(v => v.Salary, Convert.ToSByte(this.Salary))
                        .Set(v => v.Color, JsonConvert.SerializeObject(this.Color))
                        .Set(v => v.Slogan, this.Slogan)
                        .UpdateAsync();
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
        }

        public void SaveCrimeOptions()
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    await db.Organizations
                        .Where(o => o.Organization == this.Id)
                        .Set(o => o.CrimeOptions, Convert.ToSByte(this.CrimeOptions))
                        .UpdateAsync();
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
        }
        
        public bool IsLeader(int uuid) => uuid == this.OwnerUUID;
        //
        
        public int MoneyMultiplier()
        {

            if (!this.Status) 
                return 0;
            
            if (this.Money < 50000) return 0; // 0-50
            if (this.Money < 100000) return 1; // 50-100
            if (this.Money < 150000) return 2; // 100-150
            if (this.Money < 200000) return 3; // 150-200
            if (this.Money < 250000) return 4; // 200-250
            if (this.Money < 300000) return 5; // 250-300
            if (this.Money < 350000) return 6; // 300-350
            if (this.Money < 400000) return 7; // 350-400
            if (this.Money < 450000) return 8; // 400-450
            if (this.Money < 500000) return 9; // 450-500
            if (this.Money < 550000) return 10; // 500-550
            if (this.Money < 600000) return 11; // 550-600
            if (this.Money < 650000) return 12; // 600-650
            if (this.Money < 700000) return 13; // 650-700
            if (this.Money < 750000) return 14; // 700-750
            if (this.Money < 800000) return 15; // 750-800
            if (this.Money < 850000) return 16; // 800-850
            if (this.Money < 900000) return 17; // 850-900
            if (this.Money < 950000) return 18; // 900-950
            if (this.Money < 1000000) return 19; // 950-1
            if (this.Money < 2000000) return 20; // 1-2
            if (this.Money < 3000000) return 21; // 2-3
            if (this.Money < 4000000) return 22; // 3-4
            if (this.Money < 5000000) return 23; // 4-5
            return 24; // 5+
        }
        public int DrugsMultiplier()
        {
            if (!this.Status) 
                return 0;

            if (this.Drugs < 100) return 0; // 0-100
            if (this.Drugs < 250) return 1; // 100-250
            if (this.Drugs < 500) return 2; // 250-500
            if (this.Drugs < 1000) return 3; // 500-1000
            if (this.Drugs < 2500) return 4; // 1000-2500
            if (this.Drugs < 5000) return 5; // 2500-5000
            
            return 6; // 5000+
        }

        public int MaterialsMultiplier()
        {
            if (!this.Status) 
                return 0;
            
            if (this.Materials < 5000) return 0; // 0-5000
            if (this.Materials < 25000) return 1; // 5000-25000
            if (this.Materials < 45000) return 2; // 25000-45000
            
            return 3; // 45000+
        }

        public int MedKitsMultiplier()
        {
            if (!this.Status) 
                return 0;
            
            if (this.MedKits < 100) return 0; // 0-100
            if (this.MedKits < 250) return 1; // 100-250
            if (this.MedKits < 500) return 2; // 250-500
            if (this.MedKits < 1000) return 3; // 500-1000
            if (this.MedKits < 2500) return 4; // 1000-2500
            if (this.MedKits < 5000) return 5; // 2500-5000
            
            return 6; // 5000+
        }
    }
}