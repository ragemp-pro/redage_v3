using System;
using System.Collections.Generic;
using System.Linq;
using NeptuneEvo.Character;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Handles;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.Table.Tasks.Models;
using Newtonsoft.Json;

namespace NeptuneEvo.Table.Tasks.Player
{
    public static class Repository
    {
        public static void InitTableFraction(this ExtPlayer player, string fractionMyTasksData, bool isMyUpdate = false, bool isUpdate = false)
        {
            try
            {
                if (player == null)
                    return;
                
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                var fractionData = Fractions.Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null)
                    return;
                
                var fracId = (Fractions.Models.Fractions) fractionData.Id;
                if (!Table.Tasks.Repository.FractionTaskIds.ContainsKey(fracId))
                    return;
                
                var ids = Table.Tasks.Repository.FractionTaskIds[fracId].ToList();
                
                memberFractionData.TasksData = Table.Tasks.Repository.GetData(memberFractionData.TasksData, fractionData.TasksData, isUpdate);
                player.FractionTasksData = Table.Tasks.Repository.GetData(fractionMyTasksData, ids, isMyUpdate);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static void AddTableScore(this ExtPlayer player, TableTaskId tableTaskId)
        {
            try
            {
                if (player == null)
                    return;
                
                if (!Table.Tasks.Repository.TasksList.ContainsKey(tableTaskId))
                    return;

                var task = Table.Tasks.Repository.TasksList[tableTaskId];

                player.AddOrganizationScore(tableTaskId, task);
                //
                player.AddFractionScore(tableTaskId, task);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        private static void AddOrganizationScore(this ExtPlayer player, TableTaskId tableTaskId, TableTaskData task)
        {
            try
            {
                if (player == null)
                    return;
                
                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberOrganizationData == null)
                    return;

                if (player.OrganizationTasksData != null)
                {
                    var tableTaskData = player.OrganizationTasksData
                        .FirstOrDefault(t => t.Id == tableTaskId);

                    if (tableTaskData != null && !tableTaskData.Success)
                    {
                        tableTaskData.Count++;

                        if (tableTaskData.Count >= task.MaxCount)
                        {
                            tableTaskData.Count = task.MaxCount;
                            tableTaskData.Success = true;
                            
                            Trigger.ClientEvent(player, "client.battlepass.missionComplite", $"Фракционное задание выполнено", $"{task.Name}");

                            foreach (var award in task.Awards)
                            {
                                Table.Tasks.Repository.GiveBonus(player, award);
                            }
                        }
                    }
                }
                //

                if (memberOrganizationData.TasksData != null)
                {
                    var tableTaskData = memberOrganizationData.TasksData
                        .FirstOrDefault(t => t.Id == tableTaskId);

                    if (tableTaskData != null && !tableTaskData.Success)
                    {
                        tableTaskData.Count++;

                        if (tableTaskData.Count >= task.MaxCount)
                        {
                            tableTaskData.Count = task.MaxCount;
                            tableTaskData.Success = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        private static void AddFractionScore(this ExtPlayer player, TableTaskId tableTaskId, TableTaskData task)
        {
            try
            {
                if (player == null)
                    return;
                
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;

                var fractionData = Fractions.Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null)
                    return;
                
                if (player.FractionTasksData != null)
                {
                    var tableTaskData = player.FractionTasksData
                        .FirstOrDefault(t => t.Id == tableTaskId);

                    if (tableTaskData != null && !tableTaskData.Success)
                    {
                        tableTaskData.Count++;

                        if (tableTaskData.Count >= task.MaxCount)
                        {
                            tableTaskData.Count = task.MaxCount;
                            tableTaskData.Success = true;
                            
                            Trigger.ClientEvent(player, "client.battlepass.missionComplite", $"Фракционное задание выполнено", $"{task.Name}");

                            foreach (var award in task.Awards)
                            {
                                Table.Tasks.Repository.GiveBonus(player, award);
                            }
                        }
                    }
                }
                //

                if (memberFractionData.TasksData != null)
                {
                    var tableTaskData = memberFractionData.TasksData
                        .FirstOrDefault(t => t.Id == tableTaskId);

                    if (tableTaskData != null && !tableTaskData.Success)
                    {
                        tableTaskData.Count++;

                        if (tableTaskData.Count >= task.MaxCount)
                        {
                            tableTaskData.Count = task.MaxCount;
                            tableTaskData.Success = true;

                            var fractionTasksData = fractionData.TasksData.FirstOrDefault(t => t.Id == tableTaskId);
                            if (fractionTasksData != null && !fractionTasksData.Success)
                            {
                                fractionTasksData.Count++;
                                if (fractionTasksData.Count >= (task.MaxCount * NeptuneEvo.Table.Tasks.Repository.MaxCountPlayerSuccess))
                                {
                                    fractionTasksData.Success = true;
                                }
                            }
                        }
                    }
                }
                
                memberFractionData = Fractions.Manager.GetFractionMemberData(memberFractionData.UUID, memberFractionData.Id);
                
                if (memberFractionData.TasksData != null)
                {
                    var tableTaskData = memberFractionData.TasksData
                        .FirstOrDefault(t => t.Id == tableTaskId);

                    if (tableTaskData != null && !tableTaskData.Success)
                    {
                        tableTaskData.Count++;

                        if (tableTaskData.Count >= task.MaxCount)
                        {
                            tableTaskData.Count = task.MaxCount;
                            tableTaskData.Success = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
    }
}