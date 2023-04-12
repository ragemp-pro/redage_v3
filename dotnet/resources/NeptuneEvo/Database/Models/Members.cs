using System;
using System.Linq;
using System.Threading;
using Database;
using Redage.SDK;

namespace NeptuneEvo.Database.Models
{
    public class Members
    {
        private static readonly nLog Log = new nLog("Database.Members");
                
        public static void Start()
        {
            var thread = new Thread(Worker);
            thread.IsBackground = true;
            thread.Name = "MembersSave";
            thread.Start();
        }
        private static async void Worker()
        {
            while (true)
            {
                try
                {
                    await using var db = new ServerBD("MainDB");
                    
                    foreach (var fractionId in Fractions.Manager.AllMembers.Keys.ToList())
                    {
                        foreach (var fractionData in Fractions.Manager.AllMembers[fractionId].ToList())
                        {
                            if (!fractionData.IsSave)
                                continue;

                            await fractionData.Save(db);
                        }
                    }

                    foreach (var orgId in Organizations.Manager.AllMembers.Keys.ToList())
                    {
                        foreach (var organizationData in Organizations.Manager.AllMembers[orgId].ToList())
                        {
                            if (!organizationData.IsSave)
                                continue;

                            await organizationData.Save(db);
                        }
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