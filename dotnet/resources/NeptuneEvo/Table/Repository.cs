using System;
using System.Collections.Generic;
using System.Linq;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Table.Models;
using NeptuneEvo.Table.Tasks.Models;
using Newtonsoft.Json;

namespace NeptuneEvo.Table
{
    public class Repository
    {
        public static int MaxLogSkip = 20;
        public static int MaxRankCount = 30;
        public static int MaxScore = 500;
        public static int MinScore = 10;
        
        public static List<object> GetMember(MemberData memberData)
        {
            var playerData = new List<object>();
            
            try
            {
                if (memberData == null)
                {
                    playerData.Add(0);
                    playerData.Add("Нет информации");
                    playerData.Add(0);
                    playerData.Add(0);
                    playerData.Add("");
                    playerData.Add(DateTime.MinValue);
                    playerData.Add(0);
                    playerData.Add(0);
                    playerData.Add(new List<RankToAccess>());
                    playerData.Add(new List<RankToAccess>());
                    playerData.Add(0);
                    playerData.Add(DateTime.MinValue);
                    playerData.Add(null);
                    playerData.Add(null);
                    playerData.Add(null);
                    playerData.Add(null);
                }
                else 
                {
                    playerData.Add(memberData.UUID);
                    playerData.Add(memberData.Name);
                    playerData.Add(memberData.Rank);
                    playerData.Add(memberData.PlayerId);
                    playerData.Add(memberData.Avatar);
                    playerData.Add(memberData.Date);
                    playerData.Add(memberData.DepartmentId);
                    playerData.Add(memberData.DepartmentRank);
                    playerData.Add(memberData.Access);
                    playerData.Add(memberData.Lock);
                    playerData.Add(memberData.Score);
                    playerData.Add(memberData.LastLoginDate);
                    var time = memberData.Time;
                    playerData.Add(time.TodayTime);
                    playerData.Add(time.WeekTime);
                    playerData.Add(time.MonthTime);
                    playerData.Add(time.TotalTime);
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }

            return playerData;
        }
        
        
        public static List<object> GetBoard(List<BoardData> boardsData, int index)
        {
            var boards = boardsData.ToList();
            
            boards.Reverse();
            
            index--;

            var playerData = new List<object>();
            playerData.Add(boards.Count);
            
            try
            {
                if (index >= 0 && index < boards.Count)
                {
                    var boardData = boards[index];

                    playerData.Add(boardData.Title);
                    playerData.Add(boardData.Text);
                    playerData.Add(boardData.Time);
                    playerData.Add(boardData.UUId);
                    playerData.Add(boardData.Name);
                    playerData.Add(boardData.Rank);
                }
                else
                {
                    
                    playerData.Add(null);
                    playerData.Add(null);
                    playerData.Add(null);
                    playerData.Add(null);
                    playerData.Add(null);
                    playerData.Add(null);
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
            playerData.Add(index + 1);

            return playerData;
        }

        public static Dictionary<int, List<object>> GetRanksData(Dictionary<int, RankData> ranks, Dictionary<int, int> ranksMembersCount = null)
        {
            var ranksName = new Dictionary<int, List<object>>();
            foreach (var rankData in ranks)
            {
                var rank = new List<object>();
                
                if (ranksMembersCount != null && ranksMembersCount.ContainsKey(rankData.Key))
                    rank.Add(ranksMembersCount[rankData.Key]);
                else
                    rank.Add(0);
                
                rank.Add(rankData.Value.Name);
                rank.Add(rankData.Value.Salary);
                rank.Add(rankData.Value.MaxScore);
                
                ranksName.Add(rankData.Key, rank);
            }

            return ranksName;
        }

        
        public static List<int> GetAccess(List<RankToAccess> accessList)
        {
            var returnAccess = new List<int>();
            foreach (var access in accessList)
                returnAccess.Add((int) access);

            return returnAccess;
        }
        
        //
        public static Dictionary<int, string> GetDepartmentsTag(Dictionary<int, DepartmentData> departments)
        {
            var tagsName = new Dictionary<int, string>();
            foreach (var rankData in departments)
            {
                tagsName.Add(rankData.Key, rankData.Value.Tag);
            }

            return tagsName;
        }
        
        public static Dictionary<int, List<object>> GetRanksData(Dictionary<int, DepartmentRankData> ranks, Dictionary<int, int> ranksMembersCount = null)
        {
            var ranksName = new Dictionary<int, List<object>>();
            foreach (var rankData in ranks)
            {
                var rank = new List<object>();
                
                if (ranksMembersCount != null && ranksMembersCount.ContainsKey(rankData.Key))
                    rank.Add(ranksMembersCount[rankData.Key]);
                else
                    rank.Add(0);
                
                rank.Add(rankData.Value.Name);
                rank.Add(rankData.Value.Access);
                rank.Add(rankData.Value.Lock);
                
                ranksName.Add(rankData.Key, rank);
            }

            return ranksName;
        }
        
        public static Dictionary<int, List<object>> GetRanksName(Dictionary<int, DepartmentRankData> ranks)
        {
            var ranksName = new Dictionary<int, List<object>>();
            foreach (var rankData in ranks)
            {
                var rank = new List<object>();
                
                rank.Add(0);
                
                rank.Add(rankData.Value.Name);
                
                ranksName.Add(rankData.Key, rank);
            }

            return ranksName;
        }
        
        public static Dictionary<int, List<object>> GetDepartmentData(Dictionary<int, DepartmentData> departments, Dictionary<int, int> departmentsMembersCount = null, Dictionary<int, string> departmentsChiefsName = null)
        {
            var ranksName = new Dictionary<int, List<object>>();
            foreach (var departmentData in departments)
            {
                var rank = new List<object>();
                
                if (departmentsMembersCount != null && departmentsMembersCount.ContainsKey(departmentData.Key))
                    rank.Add(departmentsMembersCount[departmentData.Key]);
                else
                    rank.Add(0);
                
                if (departmentsChiefsName != null && departmentsChiefsName.ContainsKey(departmentData.Key))
                    rank.Add(departmentsChiefsName[departmentData.Key]);
                else
                    rank.Add("Нет");
                
                rank.Add(departmentData.Value.Name);
                rank.Add(departmentData.Value.Tag);
                
                ranksName.Add(departmentData.Key, rank);
            }

            return ranksName;
        }
        
        //
        
        public static Dictionary<int, List<object>> GetRanksAccessData(Dictionary<int, RankData> ranks)
        {
            var ranksName = new Dictionary<int, List<object>>();
            foreach (var rankData in ranks)
            {
                var rank = new List<object>();
                
                rank.Add(rankData.Value.Name);
                rank.Add(rankData.Value.Access);
                
                ranksName.Add(rankData.Key, rank);
            }

            return ranksName;
        }
        
        //

        public static List<object> GetVehicles(int playerRank, string model, string number, int rank, bool isTicket = false, bool isGarage = false)
        {
            if (playerRank >= rank)
            {
                var vehicleData = new List<object>();

                vehicleData.Add(model);
                vehicleData.Add(number);
                vehicleData.Add(rank);
                vehicleData.Add((!isTicket && !isGarage && playerRank >= rank)); //Evac
                vehicleData.Add((!isGarage && playerRank >= rank)); //gps

                return vehicleData;
            }

            return null;
        }
        
        //
        public static int DepartmentsMax = 15;
        public static Dictionary<int, DepartmentRankData> DefaultDepartments = new Dictionary<int, DepartmentRankData>
        {
            {0, new DepartmentRankData
                {
                    Name = "Участник",
                    Access = new List<RankToAccess>(),
                    Lock = new List<RankToAccess>()
                }
            },
            {1, new DepartmentRankData
                {
                    Name = "Заместитель №2",
                    Access = new List<RankToAccess>(),
                    Lock = new List<RankToAccess>()
                }
            },
            {2, new DepartmentRankData
                {
                    Name = "Заместитель №1",
                    Access = new List<RankToAccess>(),
                    Lock = new List<RankToAccess>()
                }
            },
            {3, new DepartmentRankData
                {
                    Name = "Начальник отдела",
                    Access = new List<RankToAccess>(),
                    Lock = new List<RankToAccess>()
                }
            }
        };
        
        //
        
        public static List<List<object>> GetAwards(List<TableTaskAwards> tableTaskAwards)
        {
            var awards = new List<List<object>>();
            foreach (var tableTaskAward in tableTaskAwards)
            {
                var award = new List<object>();

                award.Add(tableTaskAward.Type);
                award.Add(tableTaskAward.ItemId);
                award.Add(tableTaskAward.Count);
                award.Add(tableTaskAward.Data);
                award.Add(tableTaskAward.Gender);
                
                awards.Add(award);
            }


            return awards;
        }

    }
}