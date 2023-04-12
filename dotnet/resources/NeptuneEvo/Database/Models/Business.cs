using System;
using System.Linq;
using System.Threading;
using Database;
using Redage.SDK;

namespace NeptuneEvo.Database.Models
{
    public class Business
    {
        private static readonly nLog Log = new nLog("Database.Business");
                
        public static void Start()
        {
            var thread = new Thread(Worker);
            thread.IsBackground = true;
            thread.Name = "BusinessSave";
            thread.Start();
        }
        private static async void Worker()
        {
            while (true)
            {
                try
                {
                    var businessId = Core.BusinessManager.BizList
                        .Where(b => b.Value.IsSave)
                        .Select(b => b.Key)
                        .ToList();
                
                    if (Businesses.History.Repository.IsCountInsertHistory() || businessId.Count > 0)
                    {
                        await using var db = new ServerBD("MainDB"); //В отдельном потоке

                        await Businesses.History.Repository.InsertHistory(db);

                        foreach (var id in businessId)
                            await Core.BusinessManager.BizList[id].Save(db);
                    }
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }

                Thread.Sleep(1000 * 30);
            }
        }
    }
}