using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using LinqToDB;
using NeptuneEvo.Character;
using NeptuneEvo.Handles;
using NeptuneEvo.Organizations.Models;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.Players;
using NeptuneEvo.Table.Models;
using Newtonsoft.Json;

namespace NeptuneEvo.Organizations.Table.Logs
{
    public class Repository
    {
                
        public static void AddLogs(ExtPlayer player, OrganizationLogsType type, string text)
        {
            try
            {
                var organizationMemberData = player.GetOrganizationMemberData();
                if (organizationMemberData == null) 
                    return;

                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.InsertAsync(new Orglogs
                        {
                            Organization = (short) organizationMemberData.Id,
                            Name = organizationMemberData.Name,
                            Uuid = organizationMemberData.UUID,
                            Rank = (sbyte) organizationMemberData.Rank,
                            Text = text,
                            Type = (sbyte) type,
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
        
                
        public static void GetLogs(ExtPlayer player, int uuid = -1, bool isStock = false, string text = "", int pageId = 0)
        {
            try
            {
                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberOrganizationData == null)
                    return;
                
                if (memberOrganizationData.UUID != uuid && !player.IsOrganizationAccess(RankToAccess.Logs)) return;
                
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        var test = db.Orglogs
                            .Select(v => new
                            {
                                v.Organization,
                                v.Uuid,
                                v.Name,
                                v.Rank,
                                v.AutoId,
                                v.Text,
                                v.Time,
                                v.Type,
                            })
                            .Where(v => v.Organization == memberOrganizationData.Id);
                        
                        if (uuid > 0)
                            test = test
                                .Where(v => v.Uuid == uuid);

                        if (isStock)
                        {
                            var stocks = new sbyte[]
                            {
                                (sbyte) OrganizationLogsType.TakeStock,
                                (sbyte) OrganizationLogsType.OpenStock,
                                (sbyte) OrganizationLogsType.CloseStock
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
                                
                            Trigger.ClientEvent(player, "client.org.main.logs", JsonConvert.SerializeObject(logsList));
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
        /*public static void GetLogs(ExtPlayer player, int uuid = -1, bool isStock = false, string text = "", int pageId = 0)
        {
            try
            {
                
                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberOrganizationData == null) 
                    return;
                
                if (memberOrganizationData.UUID != uuid && !player.IsOrganizationAccess(RankToAccess.Logs)) return;
                                
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        var test = db.Orglogs
                            .Select(v => new
                            {
                                v.Organization,
                                v.Uuid,
                                v.Name,
                                v.Rank,
                                v.AutoId,
                                v.Text,
                                v.Time,
                                v.Type,
                            })
                            .Where(v => v.Organization == memberOrganizationData.Id);



                        if (uuid > 0)
                            test = test
                                .Where(v => v.Uuid == uuid);

                        if (isStock)
                        {
                            var stocks = new sbyte[]
                            {
                                (sbyte) OrganizationLogsType.TakeStock,
                                (sbyte) OrganizationLogsType.OpenStock,
                                (sbyte) OrganizationLogsType.CloseStock
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
                                
                            Trigger.ClientEvent(player, "client.org.main.logs", JsonConvert.SerializeObject(logsList));
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
        }*/
    }
}