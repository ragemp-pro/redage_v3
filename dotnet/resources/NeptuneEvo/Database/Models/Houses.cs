using System;
using System.Linq;
using System.Threading;
using Database;
using NeptuneEvo.Houses;
using Redage.SDK;

namespace NeptuneEvo.Database.Models
{
    public class Houses
    {
        /***
         * Дома
         * Гаражи
         * Фурнитура
         */
        private static readonly nLog Log = new nLog("Database.Houses");
                
        public static void Start()
        {
            var thread = new Thread(Worker);
            thread.IsBackground = true;
            thread.Name = "HousesSave";
            thread.Start();
        }
        private static async void Worker()
        {
            while (true)
            {
                try
                {
                    var houses = HouseManager.Houses
                        .Where(h => h.IsSave)
                        .ToList();
                    
                    var garages = GarageManager.Garages
                        .Select(g => g.Value)
                        .Where(g => g.IsSave)
                        .ToList();
                    
                    var houseFurnitures = HouseManager.Houses
                        .Where(h => h.IsFurnitureSave)
                        .ToList();
                    
                    if (houses.Count > 0 || garages.Count > 0 || houseFurnitures.Count > 0)
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке 
                        
                        foreach (var house in houses)
                            await house.Save(db);   
                        
                        //
                        
                        foreach (var garage in garages)
                            await garage.Save(db);
                        
                        //

                        foreach (var house in houseFurnitures)
                        {
                            house.IsFurnitureSave = false;
                            
                            await FurnitureManager.Save(db, house.ID);
                        }

                    }
                }
                catch (Exception e)
                {
                    Log.Write($"LogsWorker Exception: {e.ToString()}");
                }
                Thread.Sleep(1000 * 30);
            }
        }
    }
}