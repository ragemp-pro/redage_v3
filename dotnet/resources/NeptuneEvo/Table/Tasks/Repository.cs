using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Database;
using LinqToDB;
using Localization;
using NeptuneEvo.BattlePass.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Events;
using NeptuneEvo.Handles;
using NeptuneEvo.Table.Tasks.Models;
using NeptuneEvo.Table.Tasks.Player;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Table.Tasks
{
    public class Repository
    {
        public static byte MaxCountPlayerSuccess = 10;
        public static IReadOnlyDictionary<TableTaskId, TableTaskData> TasksList = new Dictionary<TableTaskId, TableTaskData>
        {
            {
                TableTaskId.Item1, new TableTaskData
                {
                    Desc = "Заправить рабочую машину",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item2, new TableTaskData
                {
                    Desc = "Помыть рабочую машину.",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item3, new TableTaskData
                {
                    Desc = "Успешно завершить патруль на автомобиле",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item4, new TableTaskData
                {
                    Desc = "Успешно завершить патруль на вертолёте",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item5, new TableTaskData
                {
                    Desc = "Принять любой вызов",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item6, new TableTaskData
                {
                    Desc = "Прибыть на вызов по CODE 0",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item7, new TableTaskData
                {
                    Desc = "Прибыть на вызов о взломе сейфа",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item8, new TableTaskData
                {
                    Desc = "Прибыть на вызов о взломе транспорта",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item9, new TableTaskData
                {
                    Desc = "Выписать штраф любому нарушителю",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item10, new TableTaskData
                {
                    Desc = "Изъять наркотики у любого нарушителя",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item11, new TableTaskData
                {
                    Desc = "Изъять оружие у любого нарушителя",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item12, new TableTaskData
                {
                    Desc = "Проверить документы у любого человека",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item13, new TableTaskData
                {
                    Desc = "Обыскать любого человека",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item14, new TableTaskData
                {
                    Desc = "Выдать лицензию на оружие любому человеку",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item15, new TableTaskData
                {
                    Desc = "Эвакуировать автомобиль нарушителя",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item16, new TableTaskData
                {
                    Desc = "Доставить материалы на свою базу",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item17, new TableTaskData
                {
                    Desc = "Загрузить материалы в Порту",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item18, new TableTaskData
                {
                    Desc = "Вернуть бронежилет на склад", 
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item19, new TableTaskData
                {
                    Desc = "Добыть на гос. шахте любую руду",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item20, new TableTaskData
                {
                    Desc = "Выдать таблетку находясь в больнице",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item21, new TableTaskData
                {
                    Desc = "Реанимировать игрока используя аптечку",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item22, new TableTaskData
                {
                    Desc = "Доставить аптечку на свой склад",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item23, new TableTaskData
                {
                    Desc = "Выдать QR код любому человеку",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item24, new TableTaskData
                {
                    Desc = "Принять участие в бизваре",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item25, new TableTaskData
                {
                    Desc = "Сделать убийство на бизваре",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item26, new TableTaskData
                {
                    Desc = "Ограбить любого человека",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item27, new TableTaskData
                {
                    Desc = "Выполнить любую миссию по доставке", 
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item28, new TableTaskData
                {
                    Desc = "Взломать любую машину",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item29, new TableTaskData
                {
                    Desc = "Ограбить любую мебель в любом доме",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item30, new TableTaskData
                {
                    Desc = "Принять участие в капте",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item31, new TableTaskData
                {
                    Desc = "Сделать убийство на капте",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item32, new TableTaskData
                {
                    Desc = "Ограбить магазин 24/7",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item33, new TableTaskData
                {
                    Desc = "Доставить наркотики на склад",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            },
            {
                TableTaskId.Item34, new TableTaskData
                {
                    Desc = "Отредактировать любое объявление",
                    MaxCount = 1,
                    PersonExp = 1,
                    OrgExp = 1,
                    Awards = new List<TableTaskAwards>
                    {
                        new TableTaskAwards(BattlePassRewardType.Money, count: 500)
                    }
                }
            }
        };
        
                
        public static void GiveBonus(ExtPlayer player, TableTaskAwards award, bool isMessage = true)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                
                switch (award.Type)
                {
                    case BattlePassRewardType.Item:
                        
                        if (award.Count > 1)
                        {
                            for (int i = 0; i < award.Count; i++)
                            {
                                if (CustomDamage.WeaponsHP.ContainsKey((ItemId)award.ItemId))
                                    Chars.Repository.AddNewItemWarehouse(player, (ItemId)award.ItemId, 1, $"{GetSerial()}_{CustomDamage.WeaponsHP[(ItemId)award.ItemId]}");
                                else if ((ItemId)award.ItemId == ItemId.BodyArmor)
                                    Chars.Repository.AddNewItemWarehouse(player, (ItemId)award.ItemId, 1, $"100");
                                else
                                    Chars.Repository.AddNewItemWarehouse(player, (ItemId)award.ItemId, 1, award.Data);
                            }
                        }
                        else
                        {
                            if (CustomDamage.WeaponsHP.ContainsKey((ItemId)award.ItemId))
                                Chars.Repository.AddNewItemWarehouse(player, (ItemId)award.ItemId, 1, $"{GetSerial()}_{CustomDamage.WeaponsHP[(ItemId)award.ItemId]}");
                            else if ((ItemId)award.ItemId == ItemId.BodyArmor)
                                Chars.Repository.AddNewItemWarehouse(player, (ItemId)award.ItemId, 1, $"100");
                            else
                                Chars.Repository.AddNewItemWarehouse(player, (ItemId)award.ItemId, 1, award.Data);
                        }

                        GameLog.Money($"system", $"player({player.GetUUID()})", 1, $"TSGiveBonus({award.ItemId},{award.Data})");
                        if (isMessage) 
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGetItemSklad, Chars.Repository.ItemsInfo[(ItemId)award.ItemId].Name), 6000);
                        break;
                    case BattlePassRewardType.Money:
                        MoneySystem.Wallet.Change(player, +award.Count);
                        GameLog.Money($"system", $"player({player.GetUUID()})", award.Count, $"TSGiveBonus");
                        //PlayerStats(player);
                        if (isMessage) 
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouWonMoneyAmount, MoneySystem.Wallet.Format(award.Count)), 6000);
                        break;
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        private static string GetSerial()
        {
            try
            {
                var rand = new Random();
                int serial = rand.Next(100_000, 999_999);
                return $"TS{serial}";
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
                return $"TS000000";
            }
        }

        private static int MaxTableTask = 3;

        public static TableTaskPlayerData[] GetData(string json, List<TableTaskId> taskIds, bool isUpdate = false)
        {
            try
            {
                var time = GetTime();
                if (!(json == null || json == "null" || json == "NULL" || json == "-1" || json.Length <= 1))
                {
                    try
                    {
                        var tableTasksPlayerData = JsonConvert.DeserializeObject<TableTaskPlayerData[]>(json);

                        if (tableTasksPlayerData == null || tableTasksPlayerData.Length == 0)
                            tableTasksPlayerData = new TableTaskPlayerData[MaxTableTask];

                        var _taskIds = taskIds.ToList();
                        
                        if (taskIds.Count > 0)
                        {
                            for (var i = 0; i < MaxTableTask; i++)
                            {
                                if (tableTasksPlayerData.Length <= i || isUpdate || tableTasksPlayerData[i] == null || tableTasksPlayerData[i].Time != time)
                                {
                                    if (!isUpdate && tableTasksPlayerData[i] != null && _taskIds.Contains(tableTasksPlayerData[i].Id))
                                    {
                                        if (taskIds.Contains(tableTasksPlayerData[i].Id))
                                            taskIds.Remove(tableTasksPlayerData[i].Id);
                                        
                                        continue;
                                    }
                                    
                                    var id = NewCasino.Horses.Shuffle(taskIds)[0];

                                    tableTasksPlayerData[i] = new TableTaskPlayerData
                                    {
                                        Id = id,
                                        Count = 0,
                                        Success = false,
                                        Time = time
                                    };

                                    taskIds.Remove(id);

                                    if (taskIds.Count == 0)
                                        break;
                                }

                            }
                        }

                        return tableTasksPlayerData;
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                }

                if (taskIds.Count == 0)
                    return null;
                
                //var tasks = new TableTaskPlayerData[MaxTableTask];
                var tasks = new TableTaskPlayerData[MaxTableTask];

                for (var i = 0; i < MaxTableTask; i++)
                {
                    var id = NewCasino.Horses.Shuffle(taskIds)[0];
                    
                    tasks[i] = new TableTaskPlayerData
                    {
                        Id = id,
                        Count = 0,
                        Success = false,
                        Time = time
                    };

                    taskIds.Remove(id);

                    if (taskIds.Count == 0)
                        break;
                }
                
                return tasks;
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }

            return null;
        }
        public static TableTaskPlayerData[] GetData(TableTaskPlayerData[] tasks, TableTaskPlayerData[] staticTasks, bool isUpdate = false)
        {
            try
            {
                var newTasks = new TableTaskPlayerData[MaxTableTask];

                for (var i = 0; i < MaxTableTask; i++)//Мы смотрим старый список
                {
                    if (!isUpdate && tasks != null && tasks.Length > 0)
                    {
                        var index = staticTasks.ToList().FindIndex(t => t.Id == tasks[i].Id);
                        if (index != -1)
                        {
                            newTasks[index] = tasks[i];
                        }
                        else
                        {
                            newTasks[i] = tasks[i];
                        }
                    }
                }
                
                for (var i = 0; i < MaxTableTask; i++)//Мы смотрим старый список
                {
                    if (!isUpdate && newTasks.Length > 0 && newTasks[i] != null)
                    {
                        if (newTasks[i].Id == staticTasks[i].Id && newTasks[i].Time == staticTasks[i].Time)
                            continue;
                    }
                    
                
                    newTasks[i] = new TableTaskPlayerData
                    {
                        Id = staticTasks[i].Id,
                        Count = 0,
                        Success = false,
                        Time = staticTasks[i].Time
                    };
                    
                }   
                
                /*for (var i = 0; i < MaxTableTask; i++)
                {
                    if (!isUpdate && task != null && task.Length > 0)
                    {
                        if (staticTask.Any(t => t.Id == task[i].Id))
                        {
                            tasks[i] = task[i];
                            continue;
                        }
                        
                        if (task[i].Id == staticTask[i].Id && task[i].Time == staticTask[i].Time)
                            continue;
                    }
                
                    tasks[i] = new TableTaskPlayerData
                    {
                        Id = staticTask[i].Id,
                        Count = 0,
                        Success = false,
                        Time = staticTask[i].Time
                    };
                }*/
            
                return newTasks;
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
            
            return null;
        }
        public static int GetTime(int addDay = 0, bool isRealTime = false)
        {
            var dateTime = DateTime.Now;
            if (!isRealTime)
                dateTime = DateTime.Now.AddDays(addDay);
            
            return (Int32) (new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, isRealTime ? dateTime.Hour : 0, isRealTime ? dateTime.Minute : 0, isRealTime ? dateTime.Second : 0).Subtract(
                new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public static IReadOnlyDictionary<Fractions.Models.Fractions, TableTaskId[]> FractionTaskIds = new Dictionary<Fractions.Models.Fractions, TableTaskId[]>
        {
            
            { Fractions.Models.Fractions.FAMILY, new []
                {
                    TableTaskId.Item1,
                    TableTaskId.Item2,
                    TableTaskId.Item28,
                    TableTaskId.Item29,
                    TableTaskId.Item30,
                    TableTaskId.Item31,
                    TableTaskId.Item32,
                    TableTaskId.Item33
                } 
            },
            { Fractions.Models.Fractions.BALLAS, new []
                {
                    TableTaskId.Item1,
                    TableTaskId.Item2,
                    TableTaskId.Item28,
                    TableTaskId.Item29,
                    TableTaskId.Item30,
                    TableTaskId.Item31,
                    TableTaskId.Item32,
                    TableTaskId.Item33
                } 
            },
            { Fractions.Models.Fractions.VAGOS, new []
                {
                    TableTaskId.Item1,
                    TableTaskId.Item2,
                    TableTaskId.Item28,
                    TableTaskId.Item29,
                    TableTaskId.Item30,
                    TableTaskId.Item31,
                    TableTaskId.Item32,
                    TableTaskId.Item33
                } 
            },
            { Fractions.Models.Fractions.MARABUNTA, new []
                {
                    TableTaskId.Item1,
                    TableTaskId.Item2,
                    TableTaskId.Item28,
                    TableTaskId.Item29,
                    TableTaskId.Item30,
                    TableTaskId.Item31,
                    TableTaskId.Item32,
                    TableTaskId.Item33
                } 
            },
            { Fractions.Models.Fractions.BLOOD, new []
                {
                    TableTaskId.Item1,
                    TableTaskId.Item2,
                    TableTaskId.Item28,
                    TableTaskId.Item29,
                    TableTaskId.Item30,
                    TableTaskId.Item31,
                    TableTaskId.Item32,
                    TableTaskId.Item33
                } 
            },
            { Fractions.Models.Fractions.CITY, new []
                {
                    TableTaskId.Item1,
                    TableTaskId.Item2,
                    TableTaskId.Item3,
                    TableTaskId.Item4,
                } 
            },
            { Fractions.Models.Fractions.POLICE, new []
                {
                    TableTaskId.Item1,
                    TableTaskId.Item2,
                    TableTaskId.Item3,
                    TableTaskId.Item4,
                    TableTaskId.Item5,
                    TableTaskId.Item6,
                    TableTaskId.Item7,
                    TableTaskId.Item8,
                    TableTaskId.Item9,
                    TableTaskId.Item10,
                    TableTaskId.Item11,
                    TableTaskId.Item12,
                    TableTaskId.Item13,
                    TableTaskId.Item14,
                    TableTaskId.Item15,
                } 
            },
            { Fractions.Models.Fractions.EMS, new []
                {
                    TableTaskId.Item1,
                    TableTaskId.Item2,
                    TableTaskId.Item20,
                    TableTaskId.Item21,
                    TableTaskId.Item22,
                    TableTaskId.Item23
                } 
            },
            { Fractions.Models.Fractions.FIB, new []
                {
                    TableTaskId.Item1,
                    TableTaskId.Item2,
                    TableTaskId.Item3,
                    TableTaskId.Item4,
                    TableTaskId.Item5,
                    TableTaskId.Item6,
                    TableTaskId.Item7,
                    TableTaskId.Item8,
                    TableTaskId.Item9,
                    TableTaskId.Item10,
                    TableTaskId.Item11,
                    TableTaskId.Item12,
                    TableTaskId.Item13,
                } 
            },
            
            { Fractions.Models.Fractions.LCN, new []
                {
                    TableTaskId.Item1,
                    TableTaskId.Item2,
                    TableTaskId.Item24,
                    TableTaskId.Item25,
                    TableTaskId.Item26,
                    TableTaskId.Item27,
                    TableTaskId.Item28,
                    TableTaskId.Item29
                } 
            },
            { Fractions.Models.Fractions.RUSSIAN, new []
                {
                    TableTaskId.Item1,
                    TableTaskId.Item2,
                    TableTaskId.Item24,
                    TableTaskId.Item25,
                    TableTaskId.Item26,
                    TableTaskId.Item27,
                    TableTaskId.Item28,
                    TableTaskId.Item29
                } 
            },
            { Fractions.Models.Fractions.YAKUZA, new []
                {
                    TableTaskId.Item1,
                    TableTaskId.Item2,
                    TableTaskId.Item24,
                    TableTaskId.Item25,
                    TableTaskId.Item26,
                    TableTaskId.Item27,
                    TableTaskId.Item28,
                    TableTaskId.Item29
                } 
            },
            { Fractions.Models.Fractions.ARMENIAN, new []
                {
                    TableTaskId.Item1,
                    TableTaskId.Item2,
                    TableTaskId.Item24,
                    TableTaskId.Item25,
                    TableTaskId.Item26,
                    TableTaskId.Item27,
                    TableTaskId.Item28,
                    TableTaskId.Item29
                } 
            },
            { Fractions.Models.Fractions.ARMY, new []
                {
                    TableTaskId.Item1,
                    TableTaskId.Item2,
                    TableTaskId.Item16,
                    TableTaskId.Item17,
                    TableTaskId.Item18,
                    TableTaskId.Item19
                } 
            },
            { Fractions.Models.Fractions.LSNEWS, new []
                {
                    TableTaskId.Item1,
                    TableTaskId.Item2,
                    TableTaskId.Item34
                } 
            },
            { Fractions.Models.Fractions.THELOST, new []
                {
                    TableTaskId.Item1,
                } 
            },
            { Fractions.Models.Fractions.MERRYWEATHER, new []
                {
                    TableTaskId.Item1,
                } 
            },
            { Fractions.Models.Fractions.SHERIFF, new []
                {
                    TableTaskId.Item1,
                    TableTaskId.Item2,
                    TableTaskId.Item3,
                    TableTaskId.Item4,
                    TableTaskId.Item5,
                    TableTaskId.Item6,
                    TableTaskId.Item7,
                    TableTaskId.Item8,
                    TableTaskId.Item9,
                    TableTaskId.Item10,
                    TableTaskId.Item11,
                    TableTaskId.Item12,
                    TableTaskId.Item13,
                    TableTaskId.Item14,
                    TableTaskId.Item15,
                } 
            },
        };

        public static void UpdateOrg()
        {
            try
            {
                var successTableTask = new Dictionary<int, TableTaskPlayerData[]>(); 
                
                foreach (var fractionData in Fractions.Manager.Fractions.Values)
                {
                    var ids = new List<TableTaskId>();
                    var fracId = (Fractions.Models.Fractions) fractionData.Id;
                    if (fracId != Fractions.Models.Fractions.None && FractionTaskIds.ContainsKey(fracId))
                        ids = FractionTaskIds[fracId].ToList();

                    successTableTask[(int) fracId] = fractionData.TasksData;
                    
                    fractionData.TasksData = GetData(null, ids);
                    fractionData.SaveTasksData();
                }
                //

                var userOfflineBonus = new List<List<object>>();
                foreach (var fractionId in Fractions.Manager.AllMembers.Keys.ToList())
                {
                    if (!Fractions.Manager.AllMembers.ContainsKey(fractionId))
                        continue;
                    
                    foreach (var fractionMemberData in Fractions.Manager.AllMembers[fractionId].ToList())
                    {
                        if (fractionMemberData.TasksData == null)
                            continue;

                        var fractionData = Fractions.Manager.GetFractionData(fractionMemberData.Id);
                        if (fractionData == null)
                            continue;
                        
                        var ids = new List<TableTaskId>();
                        var fracId = (Fractions.Models.Fractions) fractionData.Id;
                        if (fracId != Fractions.Models.Fractions.None && FractionTaskIds.ContainsKey(fracId))
                            ids = FractionTaskIds[fracId].ToList();
                        
                        var player = Main.GetPlayerByUUID(fractionMemberData.UUID);
                        
                        foreach (var taskData in fractionMemberData.TasksData)
                        {
                            if (!taskData.Success)
                                continue;
                            
                            if (!Table.Tasks.Repository.TasksList.ContainsKey(taskData.Id))
                                continue;
                            
                            var fractionTasksData = successTableTask[(int) fracId].FirstOrDefault(t => t.Id == taskData.Id);
                            if (fractionTasksData == null)
                                continue;
                            
                            if (!fractionTasksData.Success)
                                continue;
                            
                            var task = Table.Tasks.Repository.TasksList[taskData.Id];
                            
                            if (player == null)
                            {
                                var offlineTask = new List<object>();
                                    
                                offlineTask.Add(fractionMemberData.UUID);
                                offlineTask.Add(taskData.Id);
                                    
                                userOfflineBonus.Add(offlineTask);
                            }
                            else
                            {
                                Trigger.ClientEvent(player, "client.battlepass.missionComplite", $"Фракционное задание выполнено", $"{task.Name}");
                                foreach (var award in task.Awards)
                                    Table.Tasks.Repository.GiveBonus(player, award);
                            }
                        }

                        fractionMemberData.TasksData = Table.Tasks.Repository.GetData(JsonConvert.SerializeObject(fractionData.TasksData), ids, true);
                        
                        player.InitTableFraction(null, true, true);
                    }
                }
            
                Trigger.SetTask(async () =>
                {
                    try
                    {            
                        //offline

                        await using var db = new ServerBD("MainDB");//В отдельном потоке
                        
                        foreach (var offlineTask in userOfflineBonus)
                        {
                            var uuid = Convert.ToInt32(offlineTask[0]);
                            var taskId = (TableTaskId) Convert.ToByte(offlineTask[1]);
                            if (!Table.Tasks.Repository.TasksList.ContainsKey(taskId))
                                continue;
                        
                            var task = Table.Tasks.Repository.TasksList[taskId];
                            
                            foreach (var award in task.Awards)
                            {
                                if (award.Type == BattlePassRewardType.Money)
                                {
                                    await db.Characters
                                        .Where(c => c.Uuid == uuid)
                                        .UpdateAsync(v => v.Money, v => v.Money + award.Count);
                                    
                                    GameLog.Money($"system", $"player({uuid})", award.Count, $"TSGiveBonusOffline");
                                }
                                else if (award.Type == BattlePassRewardType.Item)
                                {
                                    if (award.Count > 1)
                                    {
                                        for (int i = 0; i < award.Count; i++)
                                        {
                                            if (CustomDamage.WeaponsHP.ContainsKey((ItemId)award.ItemId))
                                                Chars.Repository.AddNewItemWarehouseThread(db, uuid, (ItemId)award.ItemId, 1, $"{GetSerial()}_{CustomDamage.WeaponsHP[(ItemId)award.ItemId]}");
                                            else if ((ItemId)award.ItemId == ItemId.BodyArmor)
                                                Chars.Repository.AddNewItemWarehouseThread(db, uuid, (ItemId)award.ItemId, 1, $"100");
                                            else
                                                Chars.Repository.AddNewItemWarehouseThread(db, uuid, (ItemId)award.ItemId, 1, award.Data);
                                        }
                                    }
                                    else
                                    {
                                        if (CustomDamage.WeaponsHP.ContainsKey((ItemId)award.ItemId))
                                            Chars.Repository.AddNewItemWarehouseThread(db, uuid, (ItemId)award.ItemId, 1, $"{GetSerial()}_{CustomDamage.WeaponsHP[(ItemId)award.ItemId]}");
                                        else if ((ItemId)award.ItemId == ItemId.BodyArmor)
                                            Chars.Repository.AddNewItemWarehouseThread(db, uuid, (ItemId)award.ItemId, 1, $"100");
                                        else
                                            Chars.Repository.AddNewItemWarehouseThread(db, uuid, (ItemId)award.ItemId, 1, award.Data);
                                    }

                                    GameLog.Money($"system", $"player({uuid})", 1, $"TSGiveBonusOffline({award.ItemId},{award.Data})");
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }   
                });
                
                
                
                
                
                //Org
                
                
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
    }
}