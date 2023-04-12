using System;
using System.Collections.Generic;
using System.Linq;
using Localization;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using NeptuneEvo.Table.Models;
using NeptuneEvo.Utils;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Fractions.Table.Department
{
    public class Repository
    {
        public static void DepartmentsLoad(ExtPlayer player)
        {
            try
            {
                var fractionData = player.GetFractionData();
                if (fractionData == null) 
                    return;
            
                var fracId = fractionData.Id;

                var chiefRank = NeptuneEvo.Table.Repository.DefaultDepartments.Keys.LastOrDefault();
                
                var departmentsMembersCount = new Dictionary<int, int>();
                var departmentsChiefsName = new Dictionary<int, string>();
                foreach (var foreachMemberFractionData in Manager.AllMembers[fracId].ToList())
                {
                    if (foreachMemberFractionData.DepartmentId >= 0)
                    {
                        if (!departmentsMembersCount.ContainsKey(foreachMemberFractionData.DepartmentId))
                            departmentsMembersCount[foreachMemberFractionData.DepartmentId] = 0;

                        departmentsMembersCount[foreachMemberFractionData.DepartmentId]++;
                        
                        if (foreachMemberFractionData.DepartmentRank == chiefRank)
                            departmentsChiefsName[foreachMemberFractionData.DepartmentId] = foreachMemberFractionData.Name;
                    }
                }

                var departments = NeptuneEvo.Table.Repository.GetDepartmentData(fractionData.Departments, departmentsMembersCount, departmentsChiefsName);

                Trigger.ClientEvent(player, "client.frac.main.departmentsInit", JsonConvert.SerializeObject(departments));
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
                if (!player.IsFractionAccess(RankToAccess.CreateDepartment))
                    return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                if (DateTime.Now < sessionData.TimingsData.NextGlobalChat)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Block10Min), 4500);
                    return;
                }
                
                var fractionData = player.GetFractionData();
                if (fractionData == null) 
                    return;

                if (fractionData.Departments.Count >= NeptuneEvo.Table.Repository.DepartmentsMax)
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
                
                if (fractionData.Departments.Values.Any(p => p.Name.ToLower() == name.ToLower()))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ExistNameRank), 4500);
                    return;
                }
                
                if (fractionData.Departments.Values.Any(p => p.Tag.ToLower() == tag.ToLower()))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ExistNameRank), 4500);
                    return;
                }

                var index = 0;
                for (var i = 1; i <= NeptuneEvo.Table.Repository.DepartmentsMax; i++)
                {
                    if (!fractionData.Departments.ContainsKey(i))
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

                fractionData.Departments[index] = department;
                fractionData.SaveDepartment();

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
                if (!player.IsFractionDepartmentAccess(index, 1))
                    return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                if (DateTime.Now < sessionData.TimingsData.NextGlobalChat)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Block10Min), 4500);
                    return;
                }
                
                var fractionData = player.GetFractionData();
                if (fractionData == null) 
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
                
                if (fractionData.Departments.Any(p => p.Key != index && p.Value.Name.ToLower() == name.ToLower()))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ExistNameRank), 4500);
                    return;
                }
                
                if (fractionData.Departments.Any(p => p.Key != index && p.Value.Tag.ToLower() == tag.ToLower()))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ExistNameRank), 4500);
                    return;
                }

                var departmentData = fractionData.Departments[index];
                
                departmentData.Name = name;
                departmentData.Tag = tag;
                fractionData.SaveDepartment();
                
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
                if (!player.IsFractionAccess(RankToAccess.DeleteDepartment))
                    return;
                
                var fractionData = player.GetFractionData();
                if (fractionData == null) 
                    return;
            
                if (!fractionData.Departments.ContainsKey(index))
                    return;
            
                var fracId = fractionData.Id;
                
                foreach (var foreachMemberFractionData in Manager.AllMembers[fracId].ToList())
                {
                    if (foreachMemberFractionData.DepartmentId != index)
                        continue;
  
                    Fractions.Player.Repository.SetDepartment(fracId, foreachMemberFractionData.UUID, 0, 0);
                }
                
                fractionData.Departments.Remove(index);
                fractionData.SaveDepartment();
                
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
                var fractionMemberData = player.GetFractionMemberData();
                if (fractionMemberData == null) 
                    return;
                
                var fractionData = Manager.GetFractionData(fractionMemberData.Id);
                if (fractionData == null) 
                    return;
                
                if (!fractionData.Departments.ContainsKey(index))
                    return;

                var fracId = fractionData.Id;
                
                var membersOnlineCount = 0;
                var membersOfflineCount = 0;
                var membersCount = 0;
                var ranksMembersCount = new Dictionary<int, int>();
                var playersList = new List<List<object>>();
                foreach (var foreachMemberFractionData in Manager.AllMembers[fracId].ToList())
                {
                    if (foreachMemberFractionData.DepartmentId != index)
                        continue;
                    
                    if (!ranksMembersCount.ContainsKey(foreachMemberFractionData.DepartmentRank))
                        ranksMembersCount[foreachMemberFractionData.DepartmentRank] = 0;

                    ranksMembersCount[foreachMemberFractionData.DepartmentRank]++;
                    
                    membersCount++;
                    if (foreachMemberFractionData.PlayerId != -1)
                        membersOnlineCount++;
                    else
                        membersOfflineCount++;
                    
                    playersList.Add(NeptuneEvo.Table.Repository.GetMember(foreachMemberFractionData));
                }
                
                var department = new List<object>();
                
                var departmentData = fractionData.Departments[index];
                department.Add(index);
                department.Add(departmentData.Name);
                department.Add(departmentData.Tag);
                department.Add(departmentData.Date);
                department.Add(membersCount);

                department.Add(NeptuneEvo.Table.Repository.GetRanksData(departmentData.Ranks, ranksMembersCount));
                
                department.Add(player.IsFractionDepartmentAccess(index, 1)); // isSettings
                department.Add(fractionMemberData.DepartmentRank); // isSettings

                var membersOnlineStats = new int[]
                {
                    membersOnlineCount,
                    membersOfflineCount,
                    membersCount
                };
                
                Trigger.ClientEvent(player, "client.frac.main.departmentInit", JsonConvert.SerializeObject(department), JsonConvert.SerializeObject(playersList), JsonConvert.SerializeObject(membersOnlineStats));
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
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null) 
                    return;
                
                if (!fractionData.IsLeader(memberFractionData.Rank))
                    return;
            
                if (!fractionData.Departments.ContainsKey(index))
                    return;

                var departmentData = fractionData.Departments[index];

                var ranksData = NeptuneEvo.Table.Repository.GetRanksName(departmentData.Ranks);
                
                Trigger.ClientEvent(player, "client.frac.main.departmentRankInit", JsonConvert.SerializeObject(ranksData));
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
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null) 
                    return;
                
                if (!fractionData.IsLeader(memberFractionData.Rank))
                    return;
                
                if (!fractionData.Departments.ContainsKey(index))
                    return;

                var departmentData = fractionData.Departments[index];

                var ranksData = NeptuneEvo.Table.Repository.GetRanksName(departmentData.Ranks);
                
                if (!departmentData.Ranks.ContainsKey(id))
                    return;
                
                Trigger.ClientEvent(player, "client.frac.main.rankAccessInit", JsonConvert.SerializeObject(departmentData.Ranks[id].Access), JsonConvert.SerializeObject(departmentData.Ranks[id].Lock));
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
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null) 
                    return;
                
                if (!fractionData.IsLeader(memberFractionData.Rank))
                    return;
                
                if (!fractionData.Departments.ContainsKey(index))
                    return;

                var departmentData = fractionData.Departments[index];
                
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
                    fractionData.SaveDepartment();

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
                if (!player.IsFractionDepartmentAccess(index, 1))
                    return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null) 
                    return;
                
                var departmentData = fractionData.Departments[index];
                
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
                fractionData.SaveDepartment();

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
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null) 
                    return;

                if (!player.IsFractionDepartmentAccess(index, 3))
                    return;
                
                if (!fractionData.Departments.ContainsKey(index))
                    return;
                
                var rankToUUid = JsonConvert.DeserializeObject<Dictionary<int, int>>(json);
                
                if (rankToUUid == null)
                    return;
                
                var fracId = fractionData.Id;
                var isSave = false;

                foreach (var foreachMemberFractionData in Manager.AllMembers[fracId].ToList())
                {
                    if (!fractionData.IsLeader(memberFractionData.Rank))
                    {
                        if (memberFractionData.UUID == foreachMemberFractionData.UUID)
                            continue;
                        
                        if (foreachMemberFractionData.DepartmentId != 0 && memberFractionData.DepartmentId != foreachMemberFractionData.DepartmentId)
                            continue;
                        
                        if (foreachMemberFractionData.DepartmentId != 0 && memberFractionData.DepartmentRank <= foreachMemberFractionData.DepartmentRank)
                            continue;
                    }
                    
                    if (foreachMemberFractionData.DepartmentId == index && rankToUUid.ContainsKey(foreachMemberFractionData.DepartmentRank) &&
                        rankToUUid[foreachMemberFractionData.DepartmentRank] != foreachMemberFractionData.UUID)
                    {
                        Fractions.Player.Repository.SetDepartment(fracId, foreachMemberFractionData.UUID, index, 0);
                        isSave = true;
                    }
                }
                
                foreach (var foreachMemberFractionData in Manager.AllMembers[fracId].ToList())
                {
                    if (!fractionData.IsLeader(memberFractionData.Rank))
                    {
                        if (memberFractionData.UUID == foreachMemberFractionData.UUID)
                            continue;
                        
                        if (foreachMemberFractionData.DepartmentId != 0 && memberFractionData.DepartmentId != foreachMemberFractionData.DepartmentId)
                            continue;
                        
                        if (foreachMemberFractionData.DepartmentId != 0 && memberFractionData.DepartmentRank <= foreachMemberFractionData.DepartmentRank)
                            continue;
                    }
                    
                    if (rankToUUid.ContainsValue(foreachMemberFractionData.UUID)) {
                        var rank = rankToUUid.FirstOrDefault(f => f.Value == foreachMemberFractionData.UUID).Key;

                        Fractions.Player.Repository.SetDepartment(fracId, foreachMemberFractionData.UUID, index, rank);
                        isSave = true;
                    }
                }
                
                if (isSave)
                {
                    fractionData.SaveDepartment();

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
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null) 
                    return;
                
                if (!fractionData.Departments.ContainsKey(index))
                    return;
                
                var fracId = fractionData.Id;
                var targetMemberFractionData = Manager.GetFractionMemberData(uuid, fracId);
                if (targetMemberFractionData == null)
                    return;

                if (!fractionData.IsLeader(memberFractionData.Rank))
                {
                    if (memberFractionData.DepartmentId != targetMemberFractionData.DepartmentId)
                    {
                        return;
                    }
                
                    if (memberFractionData.DepartmentRank <= targetMemberFractionData.DepartmentRank)
                    {
                        return;
                    }
                }

                if (Fractions.Player.Repository.SetDepartment(fracId, uuid, 0, 0))
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
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null) 
                    return;
                
                if (index > 0 && !fractionData.Departments.ContainsKey(index))
                    return;
                
                var fracId = fractionData.Id;
                var targetMemberFractionData = Manager.GetFractionMemberData(uuid, fracId);
                if (targetMemberFractionData == null)
                    return;

                if (!fractionData.IsLeader(memberFractionData.Rank))
                {
                    if ((index > 0 && memberFractionData.DepartmentId != index) || (index == 0 && memberFractionData.DepartmentId != targetMemberFractionData.DepartmentId))
                    {
                        //Это не ваш отряд
                        
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Это не ваш отряд", 3000);
                        return;
                    }
                    
                    if (index > 0 && targetMemberFractionData.DepartmentId != 0)
                    {
                        //Игрок в другом отряде
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок в другом отряде", 3000);
                        return;
                    }
                }
                
                if (Fractions.Player.Repository.SetDepartment(fracId, uuid, index, 0))
                    Player.Repository.UpdateMember(player, fracId, uuid);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
    }
}