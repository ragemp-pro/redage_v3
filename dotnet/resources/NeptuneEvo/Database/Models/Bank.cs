using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Database;
using LinqToDB;
using Redage.SDK;

namespace NeptuneEvo.Database.Models
{
    public class Bank
    {
        private static readonly nLog Log = new nLog("Database.GameLog");
                
        public static void Start()
        {
            var thread = new Thread(Worker);
            thread.IsBackground = true;
            thread.Name = "BankSave";
            thread.Start();
        }
        private static async void Worker()
        {
            while (true)
            {
                try
                {
                    var banks = MoneySystem.Bank.Accounts
                        .Where(b => b.Value.IsSave)
                        .Select(b => b.Key)
                        .ToList();

                    var dellList = DellList.ToList();
                    DellList.Clear();

                    if (banks.Count > 0 || dellList.Count > 0)
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке 

                        foreach (var bankId in banks)
                            await MoneySystem.Bank.Save(db, bankId);

                        //
                    
                        foreach (var bankId in dellList)
                            await db.Money
                                .Where(m => m.Id == bankId)
                                .DeleteAsync();
                    
                    }
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
                
                Thread.Sleep(1000 * 30);
            }
        }

        //Dell
        private static List<int> DellList = new List<int>();

        public static void OnDell(int id) => DellList.Add(id);

    }
}