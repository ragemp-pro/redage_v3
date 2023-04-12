using System;
using System.Collections.Generic;
using System.Linq;
using Localization;
using NeptuneEvo.Chars;
using NeptuneEvo.Handles;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.Players;
using NeptuneEvo.Table.Models;
using NeptuneEvo.Utils;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Organizations.Table.Department
{
    public class Repository
    {
        public static void DepartmentsLoad(ExtPlayer player)
        {
            try
            {
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
            
                var orgId = organizationData.Id;

                var chiefRank = NeptuneEvo.Table.Repository.DefaultDepartments.Keys.LastOrDefault();
                
                var departmentsMembersCount = new Dictionary<int, int>();
                var departmentsChiefsName = new Dictionary<int, string>();
                foreach (var foreachMemberOrganizationData in Manager.AllMembers[orgId].ToList())
                {
                    if (foreachMemberOrganizationData.DepartmentId >= 0)
                    {
                        if (!departmentsMembersCount.ContainsKey(foreachMemberOrganizationData.DepartmentId))
                            departmentsMembersCount[foreachMemberOrganizationData.DepartmentId] = 0;

                        departmentsMembersCount[foreachMemberOrganizationData.DepartmentId]++;
                        
                        if (foreachMemberOrganizationData.DepartmentRank == chiefRank)
                            departmentsChiefsName[foreachMemberOrganizationData.DepartmentId] = foreachMemberOrganizationData.Name;
                    }
                }

                var departments = NeptuneEvo.Table.Repository.GetDepartmentData(organizationData.Departments, departmentsMembersCount, departmentsChiefsName);

                Trigger.ClientEvent(player, "client.org.main.departmentsInit", JsonConvert.SerializeObject(departments));
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static void CreateDepartment(ExtPlayer player, string name, string tag)
        {
            try
            {
                if (!player.IsOrganizationAccess(RankToAccess.CreateDepartment))
                    return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                if (DateTime.Now < sessionData.TimingsData.NextGlobalChat)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Block10Min), 4500);
                    return;
                }
                
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
                
                if (organizationData.Departments.Count >= NeptuneEvo.Table.Repository.DepartmentsMax)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxRanksAmount), 4500);
                    return;
                }

                name = Main.BlockSymbols(Main.RainbowExploit(name));
                if (name.Length < 2 || name.Length > 25)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxRankNameLenght), 4500);
                    return;
                }
                
                string testmsg = name.ToLower();
                if (Main.stringGlobalBlock.Any(c => testmsg.Contains(c)))
                {
                    sessionData.TimingsData.NextGlobalChat = DateTime.Now.AddMinutes(10);
                    Trigger.SendToAdmins(3, "!{#636363}[A] " + LangFunc.GetText(LangType.Ru, DataName.AdminAlertFTableNews, player.Name, player.Value, name));
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RestrictedWordsTableNews), 15000);
                    return;
                }
                
                tag = Main.BlockSymbols(Main.RainbowExploit(tag));
                if (tag.Length < 2 || tag.Length > 5)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxRankNameLenght), 4500);
                    return;
                }
                
                if (organizationData.Departments.Values.Any(p => p.Name.ToLower() == name.ToLower()))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ExistNameRank), 4500);
                    return;
                }
                
                if (organizationData.Departments.Values.Any(p => p.Tag.ToLower() == tag.ToLower()))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ExistNameRank), 4500);
                    return;
                }

                var index = 0;
                for (var i = 1; i <= NeptuneEvo.Table.Repository.DepartmentsMax; i++)
                {
                    if (!organizationData.Departments.ContainsKey(i))
                    {
                        index = i;
                        break;
                    }
                }

                var defaultDepartments = NeptuneEvo.Table.Repository.DefaultDepartments.Clone();

                var department = new DepartmentData
                {
                    Name = name,
                    Tag = tag,
                    Date = DateTime.Now,
                    Ranks = defaultDepartments
                };

                organizationData.Departments[index] = department;
                organizationData.SaveDepartment();

                DepartmentsLoad(player);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static void UpdateDepartment(ExtPlayer player, int index, string name, string tag)
        {
            try
            {
                if (!player.IsOrganizationDepartmentAccess(index, 1))
                    return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                if (DateTime.Now < sessionData.TimingsData.NextGlobalChat)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Block10Min), 4500);
                    return;
                }
                
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
                
                name = Main.BlockSymbols(Main.RainbowExploit(name));
                if (name.Length > 25)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxRankNameLenght), 4500);
                    return;
                }
                
                string testmsg = name.ToLower();
                if (Main.stringGlobalBlock.Any(c => testmsg.Contains(c)))
                {
                    sessionData.TimingsData.NextGlobalChat = DateTime.Now.AddMinutes(10);
                    Trigger.SendToAdmins(3, "!{#636363}[A] " + LangFunc.GetText(LangType.Ru, DataName.AdminAlertFTableNews, player.Name, player.Value, name));
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RestrictedWordsTableNews), 15000);
                    return;
                }
                
                tag = Main.BlockSymbols(Main.RainbowExploit(tag));
                if (tag.Length > 3)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxRankNameLenght), 4500);
                    return;
                }
                
                if (organizationData.Departments.Any(p => p.Key != index && p.Value.Name.ToLower() == name.ToLower()))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ExistNameRank), 4500);
                    return;
                }
                
                if (organizationData.Departments.Any(p => p.Key != index && p.Value.Tag.ToLower() == tag.ToLower()))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ExistNameRank), 4500);
                    return;
                }

                var departmentData = organizationData.Departments[index];
                
                departmentData.Name = name;
                departmentData.Tag = tag;
                organizationData.SaveDepartment();
                
                DepartmentLoad(player, index);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static void RemoveDepartment(ExtPlayer player, int index)
        {
            try
            {
                if (!player.IsOrganizationAccess(RankToAccess.DeleteDepartment))
                    return;

                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
            
                if (!organizationData.Departments.ContainsKey(index))
                    return;
            
                var orgId = organizationData.Id;
                
                foreach (var foreachMemberOrganizationData in Manager.AllMembers[orgId].ToList())
                {
                    if (foreachMemberOrganizationData.DepartmentId != index)
                        continue;
  
                    Organizations.Player.Repository.SetDepartment(orgId, foreachMemberOrganizationData.UUID, 0, 0);
                }
                
                organizationData.Departments.Remove(index);
                organizationData.SaveDepartment();
                
                DepartmentsLoad(player);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static void DepartmentLoad(ExtPlayer player, int index)
        {
            try
            {
                var organizationMemberData = player.GetOrganizationMemberData();
                if (organizationMemberData == null) 
                    return;
                
                var organizationData = Manager.GetOrganizationData(organizationMemberData.Id);
                if (organizationData == null) 
                    return;
            
                if (!organizationData.Departments.ContainsKey(index))
                    return;

                var orgId = organizationData.Id;
                
                var membersOnlineCount = 0;
                var membersOfflineCount = 0;
                var membersCount = 0;
                var ranksMembersCount = new Dictionary<int, int>();
                var playersList = new List<List<object>>();
                foreach (var foreachMemberOrganizationData in Manager.AllMembers[orgId].ToList())
                {
                    if (foreachMemberOrganizationData.DepartmentId != index)
                        continue;
                    
                    if (!ranksMembersCount.ContainsKey(foreachMemberOrganizationData.DepartmentRank))
                        ranksMembersCount[foreachMemberOrganizationData.DepartmentRank] = 0;

                    ranksMembersCount[foreachMemberOrganizationData.DepartmentRank]++;
                    
                    membersCount++;
                    if (foreachMemberOrganizationData.PlayerId != -1)
                        membersOnlineCount++;
                    else
                        membersOfflineCount++;
                    
                    playersList.Add(NeptuneEvo.Table.Repository.GetMember(foreachMemberOrganizationData));
                }
                
                var department = new List<object>();
                
                var departmentData = organizationData.Departments[index];
                department.Add(index);
                department.Add(departmentData.Name);
                department.Add(departmentData.Tag);
                department.Add(departmentData.Date);
                department.Add(membersCount);

                department.Add(NeptuneEvo.Table.Repository.GetRanksData(departmentData.Ranks, ranksMembersCount));

                department.Add(player.IsOrganizationDepartmentAccess(index, 1)); // isSettings
                department.Add(organizationMemberData.DepartmentRank); // isSettings
                
                var membersOnlineStats = new int[]
                {
                    membersOnlineCount,
                    membersOfflineCount,
                    membersCount
                };
                
                Trigger.ClientEvent(player, "client.org.main.departmentInit", JsonConvert.SerializeObject(department), JsonConvert.SerializeObject(playersList), JsonConvert.SerializeObject(membersOnlineStats));
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        public static void DepartmentRankLoad(ExtPlayer player, int index)
        {
            try
            {
                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberOrganizationData == null) 
                    return;

                var organizationData = Organizations.Manager.GetOrganizationData(memberOrganizationData.Id);
                if (organizationData == null) 
                    return;
                
                if (!organizationData.IsLeader(memberOrganizationData.UUID))
                    return;
                
                if (!organizationData.Departments.ContainsKey(index))
                    return;

                var departmentData = organizationData.Departments[index];

                var ranksData = NeptuneEvo.Table.Repository.GetRanksName(departmentData.Ranks);
                
                Trigger.ClientEvent(player, "client.org.main.departmentRankInit", JsonConvert.SerializeObject(ranksData));
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        public static void DepartmentRankAccessLoad(ExtPlayer player, int index, int id)
        {
            try
            {
                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberOrganizationData == null) 
                    return;

                var organizationData = Organizations.Manager.GetOrganizationData(memberOrganizationData.Id);
                if (organizationData == null) 
                    return;
                
                if (!organizationData.IsLeader(memberOrganizationData.UUID))
                    return;

                var departmentData = organizationData.Departments[index];

                var ranksData = NeptuneEvo.Table.Repository.GetRanksName(departmentData.Ranks);
                
                if (!departmentData.Ranks.ContainsKey(id))
                    return;
                
                Trigger.ClientEvent(player, "client.org.main.rankAccessInit", JsonConvert.SerializeObject(departmentData.Ranks[id].Access), JsonConvert.SerializeObject(departmentData.Ranks[id].Lock));
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        
        public static void UpdateDepartmentRankAccess(ExtPlayer player, int index, int id, string json)
        {
            try
            {
                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberOrganizationData == null) 
                    return;

                var organizationData = Organizations.Manager.GetOrganizationData(memberOrganizationData.Id);
                if (organizationData == null) 
                    return;

                if (!organizationData.IsLeader(memberOrganizationData.UUID))
                    return;

                var departmentData = organizationData.Departments[index];
                
                if (!departmentData.Ranks.ContainsKey(id)) 
                    return;

                var access = departmentData.Ranks[id].Access;
                var locks = departmentData.Ranks[id].Lock;
                
                var newAccess = JsonConvert.DeserializeObject<Dictionary<int, int>>(json);

                var isSave = false;
                
                foreach (var dict in newAccess)
                {
                    var key = (RankToAccess) dict.Key;
                    var type = (AccessType) dict.Value;


                    if (type == AccessType.Skip)
                    {
                        if (access.Contains(key))
                        {
                            access.Remove(key);
                            isSave = true;
                        }

                        if (locks.Contains(key))
                        {
                            locks.Remove(key);
                            isSave = true;
                        }
                    }
                    else if (type == AccessType.Add)
                    {
                        if (!access.Contains(key))
                        {
                            access.Add(key);
                            isSave = true;
                        }

                        if (locks.Contains(key))
                        {
                            locks.Remove(key);
                            isSave = true;
                        }
                    }
                    else if (type == AccessType.Remove)
                    {
                        if (access.Contains(key))
                        {
                            access.Remove(key);
                            isSave = true;
                        }

                        if (!locks.Contains(key))
                        {
                            locks.Add(key);
                            isSave = true;
                        }
                    }
                }

                if (isSave)
                {
                    organizationData.SaveDepartment();

                    DepartmentLoad(player, index);
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static void UpdateDepartmentRankName(ExtPlayer player, int index, int id, string name)
        {
            try
            {
                if (!player.IsOrganizationDepartmentAccess(index, 1))
                    return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
                
                var departmentData = organizationData.Departments[index];
                
                if (!departmentData.Ranks.ContainsKey(id)) 
                    return;
                                
                name = Main.BlockSymbols(Main.RainbowExploit(name));
                if (name.Length > 25)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxRankNameLenght), 4500);
                    return;
                }
                
                string testmsg = name.ToLower();
                if (Main.stringGlobalBlock.Any(c => testmsg.Contains(c)))
                {
                    sessionData.TimingsData.NextGlobalChat = DateTime.Now.AddMinutes(10);
                    Trigger.SendToAdmins(3, "!{#636363}[A] " + LangFunc.GetText(LangType.Ru, DataName.AdminAlertFTableNews, player.Name, player.Value, name));
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RestrictedWordsTableNews), 15000);
                    return;
                }

                if (departmentData.Ranks.Values.Any(p => p.Name.ToLower() == name.ToLower()))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ExistNameRank), 4500);
                    return;
                }

                departmentData.Ranks[id].Name = name;
                organizationData.SaveDepartment();

                DepartmentLoad(player, index);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        //
        
        public static void SetLeadersDepartment(ExtPlayer player, int index, string json)
        {
            try
            {
                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberOrganizationData == null) 
                    return;

                var organizationData = Organizations.Manager.GetOrganizationData(memberOrganizationData.Id);
                if (organizationData == null) 
                    return;

                if (!player.IsOrganizationDepartmentAccess(index, 3))
                    return;
                
                if (!organizationData.Departments.ContainsKey(index))
                    return;
                
                var rankToUUid = JsonConvert.DeserializeObject<Dictionary<int, int>>(json);
                
                if (rankToUUid == null)
                    return;
                
                var orgId = organizationData.Id;
                var isSave = false;
                
                foreach (var foreachMemberOrganizationData in Manager.AllMembers[orgId].ToList())
                {
                    if (!organizationData.IsLeader(memberOrganizationData.UUID))
                    {
                        if (memberOrganizationData.UUID == foreachMemberOrganizationData.UUID)
                            continue;
                        
                        if (memberOrganizationData.DepartmentId != 0 && memberOrganizationData.DepartmentId != foreachMemberOrganizationData.DepartmentId)
                            continue;
                        
                        if (memberOrganizationData.DepartmentId != 0 && memberOrganizationData.DepartmentRank <= foreachMemberOrganizationData.DepartmentRank)
                            continue;
                    }
                    
                    if (foreachMemberOrganizationData.DepartmentId != index)
                        continue;
                    
                    if (rankToUUid.ContainsKey(foreachMemberOrganizationData.DepartmentRank) &&
                        rankToUUid[foreachMemberOrganizationData.DepartmentRank] != foreachMemberOrganizationData.UUID)
                    {
                        Organizations.Player.Repository.SetDepartment(orgId, foreachMemberOrganizationData.UUID, index, 0);
                        isSave = true;
                    }

                    /*if (rankToUUid.ContainsValue(foreachMemberOrganizationData.UUID)) {
                        var rank = rankToUUid.FirstOrDefault(f => f.Value == foreachMemberOrganizationData.UUID).Key;

                        Organizations.Player.Repository.SetDepartment(orgId, foreachMemberOrganizationData.UUID, index, rank);
                        isSave = true;
                    }*/
                }

                foreach (var foreachMemberOrganizationData in Manager.AllMembers[orgId].ToList())
                {
                    if (!organizationData.IsLeader(memberOrganizationData.UUID))
                    {
                        if (memberOrganizationData.UUID == foreachMemberOrganizationData.UUID)
                            continue;
                        
                        if (memberOrganizationData.DepartmentId != 0 && memberOrganizationData.DepartmentId != foreachMemberOrganizationData.DepartmentId)
                            continue;
                        
                        if (memberOrganizationData.DepartmentId != 0 && memberOrganizationData.DepartmentRank <= foreachMemberOrganizationData.DepartmentRank)
                            continue;
                    }
                    
                    if (rankToUUid.ContainsValue(foreachMemberOrganizationData.UUID)) {
                        var rank = rankToUUid.FirstOrDefault(f => f.Value == foreachMemberOrganizationData.UUID).Key;

                        Organizations.Player.Repository.SetDepartment(orgId, foreachMemberOrganizationData.UUID, index, rank);
                        isSave = true;
                    }
                }
                if (isSave)
                {
                    organizationData.SaveDepartment();

                    DepartmentLoad(player, index);
                }

            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static void DeletePlayerDepartment(ExtPlayer player, int index, int uuid)
        {
            try
            {
                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberOrganizationData == null) 
                    return;

                var organizationData = Organizations.Manager.GetOrganizationData(memberOrganizationData.Id);
                if (organizationData == null) 
                    return;
                
                if (!organizationData.Departments.ContainsKey(index))
                    return;
                
                var orgId = organizationData.Id;
                var targetMemberOrganizationData = Manager.GetOrganizationMemberData(uuid, orgId);
                if (targetMemberOrganizationData == null)
                    return;
                
                if (!organizationData.IsLeader(memberOrganizationData.UUID))
                {
                    if (memberOrganizationData.DepartmentId != targetMemberOrganizationData.DepartmentId)
                    {
                        return;
                    }
                
                    if (memberOrganizationData.DepartmentRank <= targetMemberOrganizationData.DepartmentRank)
                    {
                        return;
                    }
                }
                
                if (Organizations.Player.Repository.SetDepartment(orgId, uuid, 0, 0))
                    DepartmentLoad(player, index);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static void InvitePlayerDepartment(ExtPlayer player, int index, int uuid)
        {
            try
            {
                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberOrganizationData == null) 
                    return;

                var organizationData = Organizations.Manager.GetOrganizationData(memberOrganizationData.Id);
                if (organizationData == null) 
                    return;
                
                if (index > 0 && !organizationData.Departments.ContainsKey(index))
                    return;
                
                var orgId = organizationData.Id;
                var targetMemberOrganizationData = Manager.GetOrganizationMemberData(uuid, orgId);
                if (targetMemberOrganizationData == null)
                    return;
                
                if (!organizationData.IsLeader(memberOrganizationData.UUID))
                {
                    if ((index > 0 && memberOrganizationData.DepartmentId != index) || (index == 0 && memberOrganizationData.DepartmentId != targetMemberOrganizationData.DepartmentId))
                    {
                        //Это не ваш отряд
                        
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Это не ваш отряд", 3000);
                        return;
                    }
                    
                    if (index > 0 && targetMemberOrganizationData.DepartmentId != 0)
                    {
                        //Игрок в другом отряде
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок в другом отряде", 3000);
                        return;
                    }
                }
                
                if (Organizations.Player.Repository.SetDepartment(orgId, uuid, index, 0))
                    Player.Repository.UpdateMember(player, orgId, uuid);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
    }
}