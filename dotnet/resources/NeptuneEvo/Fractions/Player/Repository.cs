using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using LinqToDB;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Fractions.LSNews;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using NeptuneEvo.Table.Models;
using NeptuneEvo.Table.Tasks.Player;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Fractions.Player
{
    public static class Repository
    {
        public static void Init(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
                
                var memberFractionData = Manager.GetFractionMemberData(player.GetUUID());
                if (memberFractionData == null)
                    return;
                
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null)
                    return;
                
                memberFractionData.SetPlayerId(player.Id);
                
                var ranksData = fractionData.Ranks;

                if (memberFractionData.Rank > fractionData.LeaderRank())
                    memberFractionData.Rank = 1;
                
                player.SetFractionData(memberFractionData);
                
                player.SetSharedData("fraction", memberFractionData.Id);
                if(memberFractionData.Rank == fractionData.LeaderRank()) 
                    player.SetSharedData("leader", true);

                //
                
                if (memberFractionData.Id == (int) Models.Fractions.ARMY){
                    //player.Eval("mp.game.ui.setMinimapComponent(15, true, -1);");
                }
                else if (memberFractionData.Id == (int) Models.Fractions.LSNEWS){
                    LsNewsSystem.onLSNPlayerLoad(player);
                }

                if (characterData.OnDutyName != String.Empty)
                {
                    sessionData.WorkData.OnDutyName = characterData.OnDutyName;
                    Manager.SetSkin(player);
                }

                if (Configs.IsFractionPolic(memberFractionData.Id))
                {
                    lock (Police.WantedVehicles)
                    {
                        if (Police.WantedVehicles.Count >= 1)
                        {
                            foreach (string numb in Police.WantedVehicles.Keys) 
                                Trigger.ClientEvent(player, "setVehiclesWanted", numb);
                        }
                    }
                }
                
                //
                
                player.InitTableFraction(JsonConvert.SerializeObject(player.FractionTasksData));
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public static void DisconnectFraction(this ExtPlayer player)
        {
            var memberFractionData = Manager.GetFractionMemberData(player.GetUUID());
            if (memberFractionData == null)
                return;
                
            memberFractionData.SetPlayerId();
            memberFractionData.IsSave = true;
        }
        
        public static void AddFractionMemberData(this ExtPlayer player, int fracId, int rank = 0)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null)
                    return;
                    
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

                        await db.InsertAsync(new Fracranks
                        {
                            Uuid = characterData.UUID,
                            Name = name,
                            Id = fracId,
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
                        
                var memberFractionData = new FractionMemberData
                {
                    UUID = characterData.UUID,
                    Name = name,
                    Id = fracId,
                    Rank = (byte) rank,
                    Date = date,
                    LastLoginDate = date,
                    TasksData = sessionData.TasksData
                };
                
                Manager.AllMembers[fracId].Add(memberFractionData);
                characterData.OnDutyName = String.Empty;
                UpdateData.Work (player, 0);
                
                Init(player);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public static void RemoveFractionMemberData(this ExtPlayer player, bool isBdDell = true)
        {
            var memberFractionData = Manager.GetFractionMemberData(player.GetUUID());
            if (memberFractionData == null)
                return;

            RemoveFractionMemberData(memberFractionData.Id, memberFractionData.UUID, isBdDell);
        }
        public static void RemoveFractionMemberData(int fracId, int uuid, bool isBdDell = true)
        {
            try
            {
                var memberFractionData = Manager.GetFractionMemberData(uuid, fracId);
                if (memberFractionData == null)
                    return;
                
                Manager.RemoveFractionMemberData(uuid, fracId);
                
                var player = Main.GetPlayerByUUID(uuid);
                if (player != null)
                {
                    var sessionData = player.GetSessionData();
                    if (sessionData != null)
                    {
                        player.SetSharedData("fraction", 0);
                        player.SetSharedData("leader", false);

                        player.SetFractionData();
                        
                        sessionData.WorkData.OnDuty = false;
                        sessionData.WorkData.OnDutyName = String.Empty;  
                        sessionData.TasksData = memberFractionData.TasksData;

                        Trigger.ClientEvent(player, "LeaveRadio");
                        Chars.Repository.isRadio(player);
                    }
                }
                
                
                if (isBdDell)
                {
                    Trigger.SetTask(async () =>
                    {
                        try
                        {
                            await using var db = new ServerBD("MainDB"); //В отдельном потоке

                            await db.Fracranks
                                .Where(v => v.Uuid == memberFractionData.UUID && v.Id == fracId)
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
        
        public static FractionMemberData GetFractionMemberData(this ExtPlayer player)
        {
            if (player != null)
                return player.FractionData;
            
            return null;
        }
        
        public static int GetFractionId(this ExtPlayer player)
        {
            var memberFractionData = player.GetFractionMemberData();
            if (memberFractionData != null)
                return memberFractionData.Id;
            
            return (int) Fractions.Models.Fractions.None;
        }
        public static FractionData GetFractionData(this ExtPlayer player)
        {
            var memberFractionData = player.GetFractionMemberData();

            if (memberFractionData != null)
                return Manager.GetFractionData(memberFractionData.Id);

            return null;
        }
        public static string GetFractionName(this ExtPlayer player)
        {
            var memberFractionData = player.GetFractionMemberData();
            if (memberFractionData != null)
                return Manager.FractionNames[memberFractionData.Id];

            return String.Empty;
        }

        public static string GetFractionRankName(this ExtPlayer player)
        {
            var memberFractionData = player.GetFractionMemberData();
            if (memberFractionData != null)
            {
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData != null)
                    return fractionData.Ranks[memberFractionData.Rank].Name;
            }

            return LangFunc.GetText(LangType.Ru, DataName.No); //String.Empty;
        }
        
        public static bool IsFractionMemberData(this ExtPlayer player) =>
            player.GetFractionMemberData() != null;

        public static bool IsFractionLeader(this ExtPlayer player, int fracId = (int) Fractions.Models.Fractions.None)
        {
            
            var memberFractionData = player.GetFractionMemberData();
            if (memberFractionData != null)
            {
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData != null)
                {

                    if (fracId != (int) Fractions.Models.Fractions.None && fracId != memberFractionData.Id)
                        return false;

                    return fractionData.IsLeader(memberFractionData.Rank);
                }
            }
                
            return false;
        }
        public static void SetName(string oldName, string newName, bool isSave = true)
        {
            try
            {
                var memberFractionData = Manager.GetFractionMemberData(oldName);
                if (memberFractionData == null)
                    return;
                
                memberFractionData.Name = newName;
                if (isSave)
                    memberFractionData.IsSave = true;
                
                var player = Main.GetPlayerByUUID(memberFractionData.UUID);

                if (player != null)
                {
                    memberFractionData = player.GetFractionMemberData();

                    if (memberFractionData != null)
                        memberFractionData.Name = newName;
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static bool SetRank(int fracId, int uuid, int newRank, bool isSave = true)
        {
            
            var memberFractionData = Manager.GetFractionMemberData(uuid, fracId);
            if (memberFractionData == null)
                return false;

            if (memberFractionData.Rank == newRank)
                return false;
            
            memberFractionData.Rank = (byte) newRank;
            if (isSave)
                memberFractionData.IsSave = true;
            
            var player = Main.GetPlayerByUUID(uuid);

            if (player != null)
            {
                memberFractionData = player.GetFractionMemberData();

                if (memberFractionData != null)
                    memberFractionData.Rank = (byte) newRank;
            }
            
            return true;
        }
        public static void UpdateAccess(int fracId, int uuid,  List<RankToAccess> access, List<RankToAccess> locks, bool isSave = true)
        {
            var memberFractionData = Manager.GetFractionMemberData(uuid, fracId);
            if (memberFractionData == null)
                return;

            memberFractionData.Access = access;
            memberFractionData.Lock = locks;
            if (isSave)
                memberFractionData.IsSave = true;
            
            var player = Main.GetPlayerByUUID(uuid);

            if (player != null)
            {
                memberFractionData = player.GetFractionMemberData();

                if (memberFractionData != null)
                {
                    memberFractionData.Access = access;
                    memberFractionData.Lock = locks;
                }
            }
        }
        public static bool SetDepartment(int fracId, int uuid, int departmentId, int rank, bool isSave = true)
        {
            var memberFractionData = Manager.GetFractionMemberData(uuid, fracId);
            if (memberFractionData == null)
                return false;
            
            if (memberFractionData.DepartmentId == departmentId && memberFractionData.DepartmentRank == rank)
                return false;

            memberFractionData.DepartmentId = departmentId;
            memberFractionData.DepartmentRank = rank;
            if (isSave)
                memberFractionData.IsSave = true;

            var player = Main.GetPlayerByUUID(uuid);

            if (player != null)
            {
                memberFractionData = player.GetFractionMemberData();

                if (memberFractionData != null)
                {
                    memberFractionData.DepartmentId = departmentId;
                    memberFractionData.DepartmentRank = rank;
                }
            }

            return true;
        }
        public static bool IsFractionAccess(this ExtPlayer player, RankToAccess command, bool notify = true)
        {
            try
            {
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                {
                    if (notify) 
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не состоите в семье", 3000);
                    
                    return false;
                }
                
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null) 
                {
                    if (notify) 
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не состоите в семье", 3000);
                    
                    return false;
                }
                
                //Владелец
                
                if (fractionData.IsLeader(memberFractionData.Rank) && fractionData.DefaultAccess.Contains(command))
                    return true;
                        
                //Персональный
                
                if (memberFractionData.Access.Contains(command)) 
                    return true;
                
                if (memberFractionData.Lock.Contains(command))
                {
                    if (notify) 
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не состоите в семье", 3000);
                    
                    return false;
                }
                
                //Отряд

                var departmentId = memberFractionData.DepartmentId;
                var departmentRank = memberFractionData.DepartmentRank;

                if (fractionData.Departments.ContainsKey(departmentId))
                {
                    var departmentData = fractionData.Departments[departmentId];

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
                
                var rank = memberFractionData.Rank;
                if (fractionData.Ranks.ContainsKey(rank) && fractionData.Ranks[rank].Access.Contains(command))
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

        public static bool IsFractionDepartmentAccess(this ExtPlayer player, int departmentId, int rank)
        {
            var memberFractionData = player.GetFractionMemberData();
            if (memberFractionData == null)
                return false;
                        
            var fractionData = Manager.GetFractionData(memberFractionData.Id);
            if (fractionData == null)    
                return false; 
            
            if (fractionData.IsLeader(memberFractionData.Rank))
                return true;
            
            if (!fractionData.Departments.ContainsKey(departmentId))
                return false;
            
            if (memberFractionData.DepartmentId == departmentId && memberFractionData.DepartmentRank >= rank)
                return true;

            return false;
        }
        
        public static void SetAvatar(int fracId, int uuid, string png)
        {
            var memberFractionData = Manager.GetFractionMemberData(uuid, fracId);
            if (memberFractionData == null)
                return;
            
            memberFractionData.Avatar = png;
            memberFractionData.IsSave = true;

            var player = Main.GetPlayerByUUID(uuid);

            if (player != null)
            {
                memberFractionData = player.GetFractionMemberData();

                if (memberFractionData != null)
                    memberFractionData.Avatar = png;
            }
        }
        
        public static void UpdateFractionTime(this ExtPlayer player)
        {
            var memberFractionData = player.GetFractionMemberData();
            if (memberFractionData == null)
                return;
            
            DateTime now = DateTime.Now;
            int thisday = now.Day;
            int thismonth = now.Month;
            int thisyear = now.Year;

            if (memberFractionData.Time.Year != thisyear)
            {
                memberFractionData.Time.Year = thisyear;
                memberFractionData.Time.YearTime = 0;
            }
            
            if (memberFractionData.Time.Month != thismonth)
            {
                memberFractionData.Time.Month = thismonth;
                memberFractionData.Time.MonthTime = 0;
            }
            
            if (memberFractionData.Time.Day != thisday)
            {
                memberFractionData.Time.Day = thisday;
                memberFractionData.Time.TodayTime = 0;
            }
            
            if (memberFractionData.Time.Week != Main.WeekInfo)
            {
                memberFractionData.Time.Week = Main.WeekInfo;
                memberFractionData.Time.WeekTime = 0;
            }

            memberFractionData.Time.TotalTime++;
            memberFractionData.Time.TodayTime++;
            memberFractionData.Time.WeekTime++;
            memberFractionData.Time.MonthTime++;
            memberFractionData.Time.YearTime++;

            var time = memberFractionData.Time;
            
            memberFractionData = Manager.GetFractionMemberData(memberFractionData.UUID, memberFractionData.Id);
            if (memberFractionData == null)
                return;
            
            memberFractionData.Time = time;
        }
    }
}