using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using LinqToDB;
using NeptuneEvo.Handles;
using NeptuneEvo.Table.Models;
using Newtonsoft.Json;

namespace NeptuneEvo.Fractions.Models
{
    public class FractionData : TableData
    {
        public int CoalOre { get; set; }
        public int IronOre { get; set; }
        public int SulfurOre { get; set; }
        public int PreciousOre { get; set; }
        public bool IsOpenGunStock { get; set; }
        public int MaxMats { get; set; }
        public int FuelLimit { get; set; }
        public int FuelLeft { get; set; }
        public bool HaveSpecialStock { get; set; } = false;
        public Dictionary<int, List<RankToAccess>> RanksDefaultAccess { get; set; } = new Dictionary<int, List<RankToAccess>>();
        public Dictionary<int, RankData> DefaultRanks { get; set; } = new Dictionary<int, RankData>();
        public Dictionary<string, FractionVehicleData> Vehicles = new Dictionary<string, FractionVehicleData>();
        
        public void UpdateLabel()
        {
            if (StockLabel == null) return;
                
            string text = $"~w~";
                
            if (Drugs == 0 && Materials == 0 && MedKits == 0) text += "Склад пуст";
            else
            {
                if (Drugs > 0) text += $"Наркотики: ~r~{Drugs}/10000\n";
                if (Materials > 0) text += $"Материалы: ~r~{Materials}/{MaxMats}\n";
                if (MedKits > 0) text += $"Аптечки: ~r~{MedKits}";
            }

            if (HaveSpecialStock == true) text += $"\n\nНеобработанная руда:\n~w~Ископаемого угля ~r~{CoalOre} ~w~ед.\nЖелезной руды ~r~{IronOre} ~w~ед.\nСерной руды ~r~{SulfurOre} ~w~ед.\nДрагоценных камней ~r~{PreciousOre} ~w~ед.";

            StockLabel.Text = text;
        }
        public bool IsLeader(int rank) => rank == this.LeaderRank();

        public void SaveRank()
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    foreach (var rankData in this.Ranks)
                    {
                        await db.Fractionranks
                            .Where(v => v.Fraction == this.Id && v.Rank == rankData.Key)
                            .Set(v => v.Name, rankData.Value.Name)
                            //.Set(v => v.Payday, rankData.Value.Salary)
                            .Set(v => v.Access, JsonConvert.SerializeObject(rankData.Value.Access))
                            .UpdateAsync();   
                    }
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
        }
        public void SaveRank(int id)
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    if (!this.Ranks.ContainsKey(id))
                        return;
                    
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    var rankData = this.Ranks[id];
                 
                    await db.Fractionranks
                        .Where(v => v.Fraction == this.Id && v.Rank == id)
                        .Set(v => v.Name, rankData.Name)
                        //.Set(v => v.Payday, rankData.Salary)
                        .Set(v => v.Access, JsonConvert.SerializeObject(rankData.Access))
                        .UpdateAsync();   
                    
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
        }

        public void SaveDepartment()
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    await db.Fractions
                        .Where(v => v.Id == this.Id)
                        .Set(v => v.Departments, JsonConvert.SerializeObject(this.Departments))
                        .UpdateAsync();
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
        }
        public void SaveTasksData()
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    await db.Fractions
                        .Where(v => v.Id == this.Id)
                        .Set(v => v.TasksData, JsonConvert.SerializeObject(this.TasksData))
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