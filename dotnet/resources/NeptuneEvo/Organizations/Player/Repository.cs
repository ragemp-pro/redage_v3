using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using LinqToDB;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Handles;
using NeptuneEvo.Organizations.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Table.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Organizations.Player
{
    public static class Repository
    {
        public static void Init(ExtPlayer player)
        {
            try
            {
                var memberOrganizationData = Manager.GetOrganizationMemberData(player.GetUUID());
                if (memberOrganizationData == null)
                    return;
                
                var organizationData = Manager.GetOrganizationData(memberOrganizationData.Id);
                if (organizationData == null) 
                    return;

                memberOrganizationData.SetPlayerId(player.Id);
                
                player.SetOrganizationData(memberOrganizationData);
                
                player.SetSharedData("organization", memberOrganizationData.Id);
                if(organizationData.OwnerUUID == player.GetUUID()) 
                    player.SetSharedData("leader", true);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public static void DisconnectOrganization(this ExtPlayer player)
        {
            var memberOrganizationData = Manager.GetOrganizationMemberData(player.GetUUID());
            if (memberOrganizationData == null)
                return;
                
            memberOrganizationData.SetPlayerId();
            memberOrganizationData.IsSave = true;
        }
        
        public static void AddOrganizationMemberData(this ExtPlayer player, int orgId, int rank = 0)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                var name = player.GetName();

                var date = DateTime.Now;
            
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.InsertAsync(new Orgranks
                        {
                            Uuid = characterData.UUID,
                            Name = name,
                            Id = orgId,
                            Rank = (sbyte) rank,
                            Avatar = "",
                            Access = "[]",
                            @lock = "[]",
                            Date = date,
                            LastLoginDate = date,
                            Time = "{}",
                            Tasks = "[]"
                        });
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
                
                if (!Manager.AllMembers.ContainsKey(orgId))
                    Manager.AllMembers.Add(orgId, new List<OrganizationMemberData>());
                
                var memberOrganizationData = new OrganizationMemberData
                {
                    UUID = characterData.UUID,
                    Name = name,
                    Id = orgId,
                    Rank = (byte) rank,
                    Date = date,
                    LastLoginDate = date
                };
                
                Manager.AllMembers[orgId].Add(memberOrganizationData);
            
                Init(player);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static void RemoveOrganizationMemberData(this ExtPlayer player, bool isBdDell = true)
        {
            var memberOrganizationData = Manager.GetOrganizationMemberData(player.GetUUID());
            if (memberOrganizationData == null)
                return;

            RemoveOrganizationMemberData(memberOrganizationData.Id, memberOrganizationData.UUID, isBdDell);
        }
        public static void RemoveOrganizationMemberData(int orgId, int uuid, bool isBdDell = true)
        {
            try
            {
                var memberOrganizationData = Manager.GetOrganizationMemberData(uuid, orgId);
                if (memberOrganizationData == null)
                    return;

                Manager.RemoveOrganizationMemberData(uuid, orgId);

                var player = Main.GetPlayerByUUID(uuid);
                if (player != null)
                {
                
                    player.SetSharedData("organization", 0);
                    player.SetSharedData("leader", false);
            
                    player.SetOrganizationData();

                    Trigger.ClientEvent(player, "LeaveRadio");
                    Chars.Repository.isRadio(player);
                
                    Manager.RemovePlayer(player);
                }
                
                if (isBdDell)
                {
                    Trigger.SetTask(async () =>
                    {
                        try
                        {
                            await using var db = new ServerBD("MainDB"); //В отдельном потоке

                            await db.Orgranks
                                .Where(v => v.Uuid == memberOrganizationData.UUID && v.Id == memberOrganizationData.Id)
                                .DeleteAsync();
                        }
                        catch (Exception e)
                        {
                            Debugs.Repository.Exception(e);
                        }
                    });
                }
                
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        public static OrganizationMemberData GetOrganizationMemberData(this ExtPlayer player)
        {
            if (player != null)
                return player.OrganizationData;
            
            return null;
        }
        public static OrganizationData GetOrganizationData(this ExtPlayer player)
        {
            var memberOrganizationData = player.GetOrganizationMemberData();

            if (memberOrganizationData != null)
                return Manager.GetOrganizationData(memberOrganizationData.Id);

            return null;
        }
        public static string GetOrganizationName(this ExtPlayer player)
        {
            var organizationData = player.GetOrganizationData();
            if (organizationData != null)
                return organizationData.Name;

            return String.Empty;
        }
        public static string GetOrganizationRankName(this ExtPlayer player)
        {
            var memberOrganizationData = player.GetOrganizationMemberData();
            if (memberOrganizationData != null)
                return Manager.GetOrganizationRankName(memberOrganizationData.Id, memberOrganizationData.Rank);

            return String.Empty;
        }

        public static bool IsOrganizationMemberData(this ExtPlayer player) =>
            player.GetOrganizationMemberData() != null;
        
        public static void SetName(string oldName, string newName, bool isSave = true)
        {
            try
            {
                var memberOrganizationData = Manager.GetOrganizationMemberData(oldName);
                if (memberOrganizationData == null)
                    return;

                memberOrganizationData.Name = newName;
                if (isSave)
                    memberOrganizationData.IsSave = true;
                
                var player = Main.GetPlayerByUUID(memberOrganizationData.UUID);

                if (player != null)
                {
                    memberOrganizationData = player.GetOrganizationMemberData();

                    if (memberOrganizationData != null)
                        memberOrganizationData.Name = newName;
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static bool SetRank(int orgId, int uuid, int newRank, bool isSave = true)
        {
            
            var memberOrganizationData = Manager.GetOrganizationMemberData(uuid, orgId);
            if (memberOrganizationData == null)
                return false;

            if (memberOrganizationData.Rank == newRank)
                return false;
            
            memberOrganizationData.Rank = (byte) newRank;
            if (isSave)
                memberOrganizationData.IsSave = true;
            
            var player = Main.GetPlayerByUUID(uuid);

            if (player != null)
            {
                memberOrganizationData = player.GetOrganizationMemberData();

                if (memberOrganizationData != null)
                    memberOrganizationData.Rank = (byte) newRank;
            }
            
            return true;
        }

        public static void UpdateAccess(int orgId, int uuid,  List<RankToAccess> access, List<RankToAccess> locks, bool isSave = true)
        {
            var memberOrganizationData = Manager.GetOrganizationMemberData(uuid, orgId);
            if (memberOrganizationData == null)
                return;

            memberOrganizationData.Access = access;
            memberOrganizationData.Lock = locks;
            if (isSave)
                memberOrganizationData.IsSave = true;
            
            var player = Main.GetPlayerByUUID(uuid);

            if (player != null)
            {
                memberOrganizationData = player.GetOrganizationMemberData();

                if (memberOrganizationData != null)
                {
                    memberOrganizationData.Access = access;
                    memberOrganizationData.Lock = locks;
                }
            }
        }
        public static bool SetDepartment(int orgId, int uuid, int departmentId, int rank, bool isSave = true)
        {
            var memberOrganizationData = Manager.GetOrganizationMemberData(uuid, orgId);
            if (memberOrganizationData == null)
                return false;
            
            if (memberOrganizationData.DepartmentId == departmentId && memberOrganizationData.DepartmentRank == rank)
                return false;

            memberOrganizationData.DepartmentId = departmentId;
            memberOrganizationData.DepartmentRank = rank;
            if (isSave)
                memberOrganizationData.IsSave = true;

            var player = Main.GetPlayerByUUID(uuid);

            if (player != null)
            {
                memberOrganizationData = player.GetOrganizationMemberData();

                if (memberOrganizationData != null)
                {
                    memberOrganizationData.DepartmentId = departmentId;
                    memberOrganizationData.DepartmentRank = rank;
                }
            }

            return true;
        }
        
        //
        
        public static bool IsOrganizationAccess(this ExtPlayer player, RankToAccess command, bool notify = true)
        {
            try
            {
                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberOrganizationData == null)
                {
                    if (notify) 
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не состоите в семье", 3000);
                    
                    return false;
                }
                
                var organizationData = Manager.GetOrganizationData(memberOrganizationData.Id);
                if (organizationData == null) 
                {
                    if (notify) 
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не состоите в семье", 3000);
                    
                    return false;
                }
                
                //Владелец
                
                if (organizationData.OwnerUUID == player.GetUUID() && organizationData.DefaultAccess.Contains(command)) 
                    return true;
                        
                //Персональный
                
                if (memberOrganizationData.Access.Contains(command)) 
                    return true;
                
                if (memberOrganizationData.Lock.Contains(command)) 
                {
                    if (notify) 
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не состоите в семье", 3000);
                    
                    return false;
                }
                
                //Отряд

                var departmentId = memberOrganizationData.DepartmentId;

                if (organizationData.Departments.ContainsKey(departmentId))
                {
                    var departmentRank = memberOrganizationData.DepartmentRank;
                    var departmentData = organizationData.Departments[departmentId];

                    if (departmentData.Ranks.ContainsKey(departmentRank))
                    {
                        var departmentDataRank = departmentData.Ranks[departmentRank];
                     
                        if (departmentDataRank.Access.Contains(command)) 
                            return true;
                
                        if (departmentDataRank.Lock.Contains(command)) 
                        {
                            if (notify) 
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не состоите в семье", 3000);
                    
                            return false;
                        }
                    }
                }
                
                //Ранг
                
                var rank = memberOrganizationData.Rank;
                if (organizationData.Ranks.ContainsKey(rank) && organizationData.Ranks[rank].Access.Contains(command))
                    return true;
                
                if (notify) 
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoDostup), 3000);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
            return false;
        }
        
        
        public static bool IsOrganizationDepartmentAccess(this ExtPlayer player, int departmentId, int rank)
        {
            var memberOrganizationData = player.GetOrganizationMemberData();
            if (memberOrganizationData == null)
                return false;
                        
            var organizationData = Manager.GetOrganizationData(memberOrganizationData.Id);
            if (organizationData == null)    
                return false; 
            
            if (organizationData.IsLeader(memberOrganizationData.UUID))
                return true;
            
            if (!organizationData.Departments.ContainsKey(departmentId))
                return false;
            
            if (memberOrganizationData.DepartmentId == departmentId && memberOrganizationData.DepartmentRank >= rank)
                return true;

            return false;
        }
        
        public static void SetAvatar(int orgId, int uuid, string png)
        {
            var memberOrganizationData = Manager.GetOrganizationMemberData(uuid, orgId);
            if (memberOrganizationData == null)
                return;
            
            memberOrganizationData.Avatar = png;
            memberOrganizationData.IsSave = true;

            var player = Main.GetPlayerByUUID(uuid);

            if (player != null)
            {
                memberOrganizationData = player.GetOrganizationMemberData();

                if (memberOrganizationData != null)
                    memberOrganizationData.Avatar = png;
            }
        }
        public static void UpdateOrganizationTime(this ExtPlayer player)
        {
            var memberOrganizationData = player.GetOrganizationMemberData();
            if (memberOrganizationData == null)
                return;
            
            DateTime now = DateTime.Now;
            int thisday = now.Day;
            int thismonth = now.Month;
            int thisyear = now.Year;

            if (memberOrganizationData.Time.Year != thisyear)
            {
                memberOrganizationData.Time.Year = thisyear;
                memberOrganizationData.Time.YearTime = 0;
            }
            
            if (memberOrganizationData.Time.Month != thismonth)
            {
                memberOrganizationData.Time.Month = thismonth;
                memberOrganizationData.Time.MonthTime = 0;
            }
            
            if (memberOrganizationData.Time.Day != thisday)
            {
                memberOrganizationData.Time.Day = thisday;
                memberOrganizationData.Time.TodayTime = 0;
            }
            
            if (memberOrganizationData.Time.Week != Main.WeekInfo)
            {
                memberOrganizationData.Time.Week = Main.WeekInfo;
                memberOrganizationData.Time.WeekTime = 0;
            }

            memberOrganizationData.Time.TotalTime++;
            memberOrganizationData.Time.TodayTime++;
            memberOrganizationData.Time.WeekTime++;
            memberOrganizationData.Time.MonthTime++;
            memberOrganizationData.Time.YearTime++;

            var time = memberOrganizationData.Time;
            
            memberOrganizationData = Manager.GetOrganizationMemberData(memberOrganizationData.UUID, memberOrganizationData.Id);
            if (memberOrganizationData == null)
                return;
            
            memberOrganizationData.Time = time;
        }
    }
}