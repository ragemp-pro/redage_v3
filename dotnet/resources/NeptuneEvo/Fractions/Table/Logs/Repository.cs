using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using LinqToDB;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using NeptuneEvo.Table.Models;
using Newtonsoft.Json;

namespace NeptuneEvo.Fractions.Table.Logs
{
    public class Repository
    {
                
        public static void AddLogs(ExtPlayer player, FractionLogsType type, string text)
        {
            try
            {
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;

                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.InsertAsync(new Fractionlogs
                        {
                            Fraction = (sbyte) memberFractionData.Id,
                            Name = memberFractionData.Name,
                            Uuid = memberFractionData.UUID,
                            Rank = (sbyte) memberFractionData.Rank,
                            Text = text,
                            Type = (sbyte)type,
                            Time = DateTime.Now
                        });
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static void AddOffLogs(int fractionId, string name, int uuid, FractionLogsType type, string text)
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    db.Insert(new Fractionlogs
                    {
                        Fraction = (sbyte)fractionId,
                        Name = name,
                        Uuid = uuid,
                        Text = text,
                        Type = (sbyte)type,
                        Time = DateTime.Now
                    });
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
        }
        public static void GetLogs(ExtPlayer player, int uuid = -1, bool isStock = false, string text = "", int pageId = 0)
        {
            try
            {
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                if (memberFractionData.UUID != uuid && !player.IsFractionAccess(RankToAccess.Logs)) return;
                
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        var test = db.Fractionlogs
                            .Select(v => new
                            {
                                v.Fraction,
                                v.Uuid,
                                v.Name,
                                v.Rank,
                                v.AutoId,
                                v.Text,
                                v.Time,
                                v.Type,
                            })
                            .Where(v => v.Fraction == memberFractionData.Id);
                        
                        if (uuid > 0)
                            test = test
                                .Where(v => v.Uuid == uuid);

                        if (isStock)
                        {
                            var stocks = new sbyte[]
                            {
                                (sbyte) FractionLogsType.TakeStock,
                                (sbyte) FractionLogsType.OpenStock,
                                (sbyte) FractionLogsType.CloseStock
                            };
                            
                            test = test
                                .Where(v => stocks.Contains(v.Type));
                        }
                        
                        if (text.Length > 0)
                            test = test
                                .Where(v => v.Text.ToLower().Contains(text.ToLower()));


                        var logs = await test
                                    .OrderByDescending(v => v.AutoId)
                                    .Skip(pageId * NeptuneEvo.Table.Repository.MaxLogSkip)
                                    .Take(NeptuneEvo.Table.Repository.MaxLogSkip)
                                    .ToListAsync();
                        
                        if (logs.Count > 0)
                        {                               
                            var logsList = new List<List<object>>();

                            foreach (var log in logs)
                            {
                                var logList = new List<object>();
                                
                                logList.Add(log.Text);
                                logList.Add(log.Time);
                                logList.Add(log.Uuid);
                                logList.Add(log.Name);
                                logList.Add(log.Rank);
                                
                                
                                logsList.Add(logList);
                            }
                                
                            Trigger.ClientEvent(player, "client.frac.main.logs", JsonConvert.SerializeObject(logsList));
                        }

                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
    }
}