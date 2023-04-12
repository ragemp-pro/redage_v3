using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database;
using LinqToDB;
using Redage.SDK;

namespace NeptuneEvo.Database.Models
{
    public class Money
    {
        private static readonly nLog Log = new nLog("Database.Money");
                
        public static void Start()
        {
            var thread = new Thread(Worker);
            thread.IsBackground = true;
            thread.Name = "MoneySave";
            thread.Start();
        }
        private static async void Worker()
        {
            while (true)
            {
                try
                {
                    var donateUpdateList = DonateUpdateList.Values.ToList();
                    DonateUpdateList.Clear();
                    
                    var moneyUpdateList = MoneyUpdateList.Values.ToList();
                    MoneyUpdateList.Clear();

                    if (donateUpdateList.Count > 0 || moneyUpdateList.Count > 0)
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке 

                        await DonateUpdate(db, donateUpdateList);
                        
                        await MoneyUpdate(db, moneyUpdateList);
                    }
                    
                }
                catch (Exception e)
                {
                    Log.Write($"LogsWorker Exception: {e.ToString()}");
                }
                Thread.Sleep(1000 * 1);
            }
        }
        
        //
        
        private static Dictionary<string, object[]> DonateUpdateList = new Dictionary<string, object[]>();
        
        public static void AddDonateUpdate(string login, int amount)
        {
            if (DonateUpdateList.ContainsKey(login))
                DonateUpdateList.Remove(login);
            
            DonateUpdateList.Add(login, new object[]
            {
                login,
                amount
            });
        }
        
        private static async Task DonateUpdate(ServerBD db, List<object[]> donateUpdateList)
        {
            try
            {
                foreach (var saveData in donateUpdateList)
                {
                    await db.Accounts
                        .Where(v => v.Login == saveData[0].ToString())
                        .Set(v => v.Redbucks, Convert.ToInt32(saveData[1]))
                        .UpdateAsync();
                }
            }
            catch (Exception e)
            {
                Log.Write($"DonateUpdate Exception: {e.ToString()}");
            }
        }
        
        //
        
        private static Dictionary<int, object[]> MoneyUpdateList = new Dictionary<int, object[]>();
        
        public static void AddMoneyUpdate(int uuid, long amount)
        {
            if (MoneyUpdateList.ContainsKey(uuid))
                MoneyUpdateList.Remove(uuid);
            
            MoneyUpdateList.Add(uuid, new object[]
            {
                uuid,
                amount
            });
        }
        
        private static async Task MoneyUpdate(ServerBD db, List<object[]> moneyUpdateList)
        {
            try
            {
                foreach (var saveData in moneyUpdateList)
                {
                    await db.Characters
                        .Where(v => v.Uuid == Convert.ToInt32(saveData[0]))
                        .Set(v => v.Money, Convert.ToInt32(saveData[1]))
                        .UpdateAsync();
                }
            }
            catch (Exception e)
            {
                Log.Write($"MoneyUpdate Exception: {e.ToString()}");
            }
        }
    }
}