using System;
using System.Threading;
using Database;
using Redage.SDK;

namespace NeptuneEvo.Database.Models
{
    public class Phone
    {
        private static readonly nLog Log = new nLog("Database.Phone");
                
        public static void Start()
        {
            var thread = new Thread(Worker);
            thread.IsBackground = true;
            thread.Name = "PhoneSave";
            thread.Start();
        }
        private static async void Worker()
        {
            while (true)
            {
                try
                {
                    if (Players.Phone.Messages.Repository.IsCountInsertMessages() ||
                        Players.Phone.Messages.Repository.IsCountUpdateMessages() ||
                        Players.Phone.Auction.Repository.IsSavingAuctions())
                    {
                        await using var db = new ServerBD("MainDB"); //В отдельном потоке

                        await Players.Phone.Messages.Repository.InsertMessages(db);

                        await Players.Phone.Messages.Repository.UpdateMessages(db);
                    
                        await Players.Phone.Auction.Repository.SaveAuctions(db);
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