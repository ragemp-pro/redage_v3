using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Database;
using LinqToDB;
using LinqToDB.Linq;
using Redage.SDK;

namespace NeptuneEvo.Database.Models
{
    public static class AccountModels
    {
        private static readonly nLog Log = new nLog("Database.AccountModels");
        
        private static List<IUpdatable<Characters>> DBListUpdate = new List<IUpdatable<Characters>>();
        
        public static void UpdateCustom(this IUpdatable<Characters> source)
        {
            DBListUpdate.Add(source);
        }
        public static void Start()
        {
            using var db = new ServerBD("MainDB");
            var defaultStruct = new Characters
            {
                
            };
            var test = new Characters
            {
                Uuid = 123,
                Adminlvl = 33
            };

            var testUpd = db.Characters
                .Where(c => c.Uuid == 1)
                .Set(p => p.Lvl, 2);
            
            
            foreach (var charName in test.GetType().GetProperties())
            {
                var value = charName.GetValue(test);
                if (value == null)
                    continue;
             
                var key = charName.Name;   
                var defaultValue = charName.GetValue(defaultStruct);
                
                if (!value.Equals(defaultValue))
                {
                    //testUpd = testUpd.Set(c => charName.GetGetMethod(), value);
                    Console.WriteLine($"charName - {key} - {value} - {defaultValue}");
                }

                //var updatedate = db.Characters
                //    .Where(c => c.Uuid == 1);
                
                

            }

            testUpd.Update();
            
            var thread = new Thread(Worker);
            thread.IsBackground = true;
            thread.Name = "AccountModelsSave";
            thread.Start();
        }
        private static async void Worker()
        {
            while (true)
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    var updateList = DBListUpdate.ToList();
                    
                    DBListUpdate.Clear();

                    foreach (var update in updateList)
                    {
                        await update.UpdateAsync();
                    }
                }
                catch (Exception e)
                {
                    Log.Write($"LogsWorker Exception: {e.ToString()}");
                }
                Thread.Sleep(1000 * 5);
            }
        }
        
    }
}