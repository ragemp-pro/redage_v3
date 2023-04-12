using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Fractions.Table.Tasks.Models;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Fractions.Table.Tasks
{
    public class Repository
    {
        public static void TasksMyLoad(ExtPlayer player)
        {
            try
            {
                var fracId = (Fractions.Models.Fractions) player.GetFractionId();
                if (fracId == Fractions.Models.Fractions.None)
                    return;
            
                //if (NeptuneEvo.Table.Tasks.Repository.FractionTaskIds.ContainsKey(fracId))
                //    return;
            
                var tasksData = player.FractionTasksData;
                if (tasksData == null)
                    return;
                
                var tasks = new List<List<object>>();
                foreach (var taskData in tasksData)
                {
                    if (taskData == null)
                        continue;
                    if (!NeptuneEvo.Table.Tasks.Repository.TasksList.ContainsKey(taskData.Id))
                        continue;

                    var taskInfo = NeptuneEvo.Table.Tasks.Repository.TasksList[taskData.Id];
                
                    var task = new List<object>();
                    task.Add(taskInfo.Name);
                    task.Add(taskInfo.Desc);
                    task.Add(taskData.Count);
                    task.Add(taskInfo.MaxCount);
                    task.Add(taskData.Success);
                    task.Add(taskInfo.PersonExp);
                    task.Add(NeptuneEvo.Table.Repository.GetAwards(taskInfo.Awards));
                    task.Add(NeptuneEvo.Table.Tasks.Repository.GetTime(1) - NeptuneEvo.Table.Tasks.Repository.GetTime(isRealTime: true));
                    
                    tasks.Add(task);
                }
            
                Trigger.ClientEvent(player, "client.frac.main.tasks", JsonConvert.SerializeObject(tasks));
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static void TasksLoad(ExtPlayer player)
        {
            try
            {
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
            
                var fractionData = Fractions.Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null)
                    return;
                
                //if (NeptuneEvo.Table.Tasks.Repository.FractionTaskIds.ContainsKey(fracId))
                //    return;
            
                var tasksData = memberFractionData.TasksData;
                if (tasksData == null)
                    return;
                
                var tasks = new List<List<object>>();
                foreach (var taskData in tasksData)
                {
                    if (taskData == null)
                        continue;
                    if (!NeptuneEvo.Table.Tasks.Repository.TasksList.ContainsKey(taskData.Id))
                        continue;

                    var fractionTasksData = fractionData.TasksData.FirstOrDefault(t => t.Id == taskData.Id);
                    if (fractionTasksData == null)
                        continue;
                    
                    var taskInfo = NeptuneEvo.Table.Tasks.Repository.TasksList[taskData.Id];
                
                    var task = new List<object>();
                    task.Add(taskInfo.Name);
                    task.Add(taskInfo.Desc);
                    task.Add(taskData.Count);
                    task.Add(taskInfo.MaxCount * NeptuneEvo.Table.Tasks.Repository.MaxCountPlayerSuccess);
                    task.Add(fractionTasksData.Success);
                    task.Add(taskInfo.OrgExp);
                    task.Add(NeptuneEvo.Table.Repository.GetAwards(taskInfo.Awards));
                    task.Add(NeptuneEvo.Table.Tasks.Repository.GetTime(1) - NeptuneEvo.Table.Tasks.Repository.GetTime(isRealTime: true));
                
                    tasks.Add(task);
                }
            
                Trigger.ClientEvent(player, "client.frac.main.tasks", JsonConvert.SerializeObject(tasks));
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public static void MissionsLoad(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                var tableTaskData = sessionData.TableTaskData;
            
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
            
                var fractionData = Fractions.Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null)
                    return;
            
                var patrollingData = NeptuneEvo.Table.Tasks.Patrolling.Repository.Patrollings[tableTaskData.PatrollingIndex];
            
                var missions = new List<List<object>>();

                var mission = new List<object>
                {
                    0,//id
                    NeptuneEvo.Table.Tasks.Patrolling.Repository.Payment//bonus
                };
                if (NeptuneEvo.Table.Tasks.Patrolling.Repository.IsFractionPatrolling((Fractions.Models.Fractions) fractionData.Id, false))
                {
                    mission.Add(true);
                    mission.Add(tableTaskData.IsPatrolling && !patrollingData.IsAir ? patrollingData.Position : null);
                    mission.Add(!tableTaskData.IsPatrolling);
                }
                else
                {
                    mission.Add(false);//isShow
                    mission.Add(false);//gps
                    mission.Add(false);//isTake
                }
                missions.Add(mission);
            
                //
            
                mission = new List<object>
                {
                    1,
                    NeptuneEvo.Table.Tasks.Patrolling.Repository.Payment
                };
                if (NeptuneEvo.Table.Tasks.Patrolling.Repository.IsFractionPatrolling((Fractions.Models.Fractions) fractionData.Id, true))
                {
                    mission.Add(true);
                    mission.Add(tableTaskData.IsPatrolling && patrollingData.IsAir ? patrollingData.Position : null);
                    mission.Add(!tableTaskData.IsPatrolling);
                }
                else
                {
                    mission.Add(false);
                    mission.Add(false);
                    mission.Add(false);
                }
                missions.Add(mission);
            
                //
            
                mission = new List<object>
                {
                    2,
                    Convert.ToInt32(Main.GangCarDelivery / 2)
                };
                if (Manager.FractionTypes[fractionData.Id] == FractionsType.Mafia && CarDelivery.MafiaStartDelivery.ContainsKey(fractionData.Id))
                {
                    mission.Add(true);
                    mission.Add(CarDelivery.MafiaStartDelivery[fractionData.Id]);
                    mission.Add(false);
                }
                else
                {
                    mission.Add(false);
                    mission.Add(false);
                    mission.Add(false);
                }
                missions.Add(mission);
                
                //
            
                mission = new List<object>
                {
                    3,
                    0
                };
                if (fractionData.Id == (int) Fractions.Models.Fractions.EMS)
                {
                    mission.Add(true);
                    mission.Add(new Vector3(3595.796, 3661.733, 32.75175));
                    mission.Add(false);
                }
                else
                {
                    mission.Add(false);
                    mission.Add(false);
                    mission.Add(false);
                }
                missions.Add(mission);
                
                //
            
                mission = new List<object>
                {
                    4,
                    0
                };
                if (fractionData.Id == (int) Fractions.Models.Fractions.ARMY)
                {
                    mission.Add(true);
                    mission.Add(Fractions.Army.ArmyCheckpoints[2]);
                    mission.Add(false);
                }
                else
                {
                    mission.Add(false);
                    mission.Add(false);
                    mission.Add(false);
                }
                missions.Add(mission);
                
                //
                //
            
                mission = new List<object>
                {
                    5,
                    0
                };
                mission.Add(true);
                mission.Add(new Vector3(-550.6449, -216.2233, 37.649826));
                mission.Add(false);
                missions.Add(mission);
                
                
                Trigger.ClientEvent(player, "client.frac.main.missions", JsonConvert.SerializeObject(missions));
       
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        private static List<MissionDelay> MissionsDelay = new List<MissionDelay>();

        public static void MissionUse(ExtPlayer player, int index)
        {
            try
            {
                var missionDelay = MissionsDelay.FirstOrDefault(m => m.Id == index && m.UuId == player.GetUUID());
                if (missionDelay != null && missionDelay.Date > DateTime.Now)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы взяли задание, метка уже стоит на карте.", 7000);
                    return;
                }

                var isSuccess = false;
                switch (index)
                {
                    case 0:
                        isSuccess = NeptuneEvo.Table.Tasks.Patrolling.Repository.CreatePatrolling(player, false);
                        break;
                    case 1:
                        isSuccess = NeptuneEvo.Table.Tasks.Patrolling.Repository.CreatePatrolling(player, true);
                        break;
                }

                if (isSuccess)
                {
                    MissionsLoad(player);
                }
            }            
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public static void MissionAdd(ExtPlayer player, int index)
        {
            try
            {
                var missionDelay = MissionsDelay.FirstOrDefault(m => m.Id == index && m.UuId == player.GetUUID());
                if (missionDelay != null)
                    return;
                
                MissionsDelay.Add(new MissionDelay
                {
                    Id = index,
                    UuId = player.GetUUID(),
                    Date = DateTime.Now.AddHours(3)
                });
            }           
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
    }
}