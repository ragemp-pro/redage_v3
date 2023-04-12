using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using GTANetworkAPI;
using LinqToDB;
using Localization;
using NeptuneEvo.Accounts;
using NeptuneEvo.BattlePass.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Events;
using NeptuneEvo.Handles;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.Players;
using NeptuneEvo.Players.Phone.Messages.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.BattlePass
{
    public class Repository : Script
    {
        private static readonly nLog Log = new nLog("BattlePass.Repository");
        
        private static int MaxCountTasks = 3;
        
        private static SeasonId SeasonId = SeasonId.Four;
        
        private static List<BattlePassTask> BattlePassTask = Season4Models.Repository.BattlePassTask;

        private static List<BattlePassReward> BattlePassAwards = Season4Models.Repository.BattlePassAwards;
        
        private static List<BattlePassReward> BattlePassAwardsPremium = Season4Models.Repository.BattlePassAwardsPremium;
        
        public static readonly int PricePremium = 19999;

        public static async void Init()
        {
            await using var db = new ConfigBD("ConfigDB");


            /*foreach (var item in BattlePassTask)
            {
                db.Insert(new BpTasks
                {
                    Id = item.Id,
                    Money = item.MissionMoney,
                });
            }
            
            
            foreach (var item in BattlePassAwards)
            {
                db.Insert(new BpAwards
                {
                    Id = item.Id,
                    Type = (sbyte)item.Type,
                    ItemId = item.ItemId,
                    Count = item.Count,
                    Data = item.Data,
                    Gender = (sbyte)item.Gender,
                });
            }
            
            foreach (var item in BattlePassAwardsPremium)
            {
                db.Insert(new BpAwardsPremiums
                {
                    Id = item.Id,
                    Type = (sbyte)item.Type,
                    ItemId = item.ItemId,
                    Count = item.Count,
                    Data = item.Data,
                    Gender = (sbyte)item.Gender,
                });
            }*/


            var bpTask = await db.Bptask
                .ToListAsync();

            foreach (var item in bpTask)
            {
                var id = Convert.ToInt32(item.Id);
                var battlePassTask = BattlePassTask.FirstOrDefault(b => b.Id == id);
                
                if (battlePassTask == null)
                    continue;
                battlePassTask.MissionMoney = Convert.ToInt32(item.Money);
            }

            //

            var bpAwards = await db.Bpawards
                .ToListAsync();

            foreach (var item in bpAwards)
            {
                var id = Convert.ToInt32(item.Id);
                var gender = (BattlePassRewardGender) Convert.ToInt32(item.Gender);

                var bpAward = BattlePassAwards
                    .FirstOrDefault(a => a.Id == id && a.Gender == gender);
                
                if (bpAward == null)
                    continue;
                
                bpAward.Type = (BattlePassRewardType) Convert.ToInt32(item.Type);
                bpAward.ItemId = Convert.ToInt32(item.ItemId);
                bpAward.Count = Convert.ToInt32(item.Count);
                bpAward.Data = item.Data;
            }

            //

            var bpAwardsPremium = await db.Bpawardspremium
                .ToListAsync();

            foreach (var item in bpAwardsPremium)
            {
                
                var id = Convert.ToInt32(item.Id);
                var gender = (BattlePassRewardGender) Convert.ToInt32(item.Gender);

                var bpAwardPremium = BattlePassAwardsPremium
                    .FirstOrDefault(a => a.Id == id && a.Gender == gender);
                
                if (bpAwardPremium == null)
                    continue;
                
                bpAwardPremium.Type = (BattlePassRewardType) Convert.ToInt32(item.Type);
                bpAwardPremium.ItemId = Convert.ToInt32(item.ItemId);
                bpAwardPremium.Count = Convert.ToInt32(item.Count);
                bpAwardPremium.Data = item.Data;
            }

            /*foreach (var rouletteItem in rouletteList)
            {
                var rouletteCaseData = RouletteCasesData
                    .FirstOrDefault(r => r.ItemId == (ItemId) rouletteItem.CaseId);
                
                if (rouletteCaseData == null)
                    continue;
                
                //RouletteItemData(int Id, string Name, int ValueMin, int ValueMax, int ReturnRB, int Percent, RouletteColor Color = RouletteColor.Blue, bool IsChatMessage = false, bool IsHudMessage = false, ItemId ItemId = ItemId.Debug, string ItemData = "")
                rouletteCaseData.RouletteItemsData.Add(new RouletteItemData(
                    (int) rouletteItem.Id,
                    rouletteItem.Name,
                    (int) rouletteItem.ValueMin,
                    (int) rouletteItem.ValueMax,
                    (int) rouletteItem.ReturnRB,
                    (int) rouletteItem.Percent,
                    (RouletteColor) rouletteItem.Color,
                    rouletteItem.IsChatMessage,
                    rouletteItem.IsHudMessage,
                    (ItemId) rouletteItem.ItemId,
                    rouletteItem.ItemData
                ));
                
            }*/


            /*var index = 0;
            foreach (var rouletteCaseData in RouletteCasesData1)
            {
                foreach (var rouletteItemData in rouletteCaseData.RouletteItemsData)
                {
                    db.InsertAsync(new Roulettes
                    {
                        CaseId = (int)index,
                        Id = rouletteItemData.Id,
                        Name = rouletteItemData.Name,
                        ValueMin = rouletteItemData.ValueMin,
                        ValueMax = rouletteItemData.ValueMax,
                        ReturnRB = rouletteItemData.ReturnRB,
                        Percent = rouletteItemData.Count,
                        Color = (int)rouletteItemData.Color,
                        IsChatMessage = rouletteItemData.IsChatMessage,
                        IsHudMessage = rouletteItemData.IsHudMessage,
                        ItemId = (short)rouletteItemData.ItemId,
                        ItemData = rouletteItemData.ItemData,

                    });
                }
                index++;
            }*/

        }
        
        
        public static async Task<BattlePassData> Load(ServerBD db, int uuid)
        {
            var battlePassData = new BattlePassData();
            
            try
            {
                var battlePass = await db.Battlepass
                    .Where(bt => bt.UserId == uuid && bt.SeasonId == (sbyte) SeasonId)
                    .FirstOrDefaultAsync();

                if (battlePass == null)
                {
                    await db.InsertAsync(new Battlepasses
                    {
                        UserId = uuid,
                        SeasonId = (sbyte) SeasonId,
                        TasksDay = JsonConvert.SerializeObject(new List<BattlePassTasks>()),
                        TasksWeek = JsonConvert.SerializeObject(new List<BattlePassTasks>()),
                        Lvl = 0,
                        Exp = 0,
                        IsPremium = false,
                        TookReward = JsonConvert.SerializeObject(new List<int>()),
                        TookRewardPremium = JsonConvert.SerializeObject(new List<int>()),
                        Time = 0
                    });
                }
                else
                {
                    battlePassData.TasksDay = JsonConvert.DeserializeObject<List<BattlePassTasks>>(battlePass.TasksDay);
                    battlePassData.TasksWeek = JsonConvert.DeserializeObject<List<BattlePassTasks>>(battlePass.TasksWeek);
                    battlePassData.Lvl = battlePass.Lvl;
                    
                    if (battlePassData.Lvl != MaxLvl && IsMaxLvl (battlePassData.Lvl))
                        battlePassData.Lvl = MaxLvl;
                    
                    battlePassData.Exp = battlePass.Exp;
                    battlePassData.IsPremium = battlePass.IsPremium;
                    battlePassData.TookReward = JsonConvert.DeserializeObject<List<int>>(battlePass.TookReward);
                    battlePassData.TookRewardPremium = JsonConvert.DeserializeObject<List<int>>(battlePass.TookRewardPremium);
                    battlePassData.Time = battlePass.Time;
                }
            }
            catch (Exception e)
            {
                Log.Write($"Load Exception: {e.ToString()}");
            }

            return battlePassData;
        }
        
        public static async Task Save(ServerBD db, ExtPlayer player, int uuid)
        {
            try
            {
                if (!player.IsCharacterData()) 
                    return;

                var battlePassData = player.BattlePassData;

                if (battlePassData != null)
                {
                    await db.Battlepass
                        .Where(bt => bt.UserId == uuid && bt.SeasonId == (sbyte) SeasonId)
                        .Set(bt => bt.TasksDay, JsonConvert.SerializeObject (battlePassData.TasksDay))
                        .Set(bt => bt.TasksWeek, JsonConvert.SerializeObject (battlePassData.TasksWeek))
                        .Set(bt => bt.Lvl, battlePassData.Lvl)
                        .Set(bt => bt.Exp, battlePassData.Exp)
                        .Set(bt => bt.IsPremium, battlePassData.IsPremium)
                        .Set(bt => bt.TookReward, JsonConvert.SerializeObject (battlePassData.TookReward))
                        .Set(bt => bt.TookRewardPremium, JsonConvert.SerializeObject (battlePassData.TookRewardPremium))
                        .Set(bt => bt.Time, battlePassData.Time)
                        .UpdateAsync();    
                }
            }
            catch (Exception e)
            {
                Log.Write($"Save Exception: {e.ToString()}");
            }
        }
        [Command("bt")]
        public static void CMD_t1(ExtPlayer player)
        {
            try
            {
                if (Main.ServerNumber != 0) return;
                Open(player, false);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_t1 Exception: {e.ToString()}");
            }
        }
        [Command("bttest")]
        public static void CMD_bttest(ExtPlayer player)
        {
            try
            {
                if (Main.ServerNumber != 0) return;
                var battlePassData = player.BattlePassData;
                var tasks = new List<BattlePassTasks>();
                
                foreach (var data in BattlePassTask)
                {
                    var battlePassTasks = new BattlePassTasks
                    {
                        Index = data.Id,
                        Count = 0,
                    };
                    tasks.Add(battlePassTasks);
                }

                battlePassData.TasksDay = tasks;
            }
            catch (Exception e)
            {
                Log.Write($"CMD_t1 Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.battlepass.open")]
        public static void Open(ExtPlayer player, bool isInit)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            if (SeasonId == SeasonId.Close)
                return;
            
            var battlePassData = player.BattlePassData;
            
            var tasksDay = new List<Dictionary<string, object>>();

            foreach (var item in battlePassData.TasksDay)
            {
                var battlePassTask = BattlePassTask.FirstOrDefault(b => b.Id == item.Index);
                if (battlePassTask == null)
                    continue;

                tasksDay.Add(new Dictionary<string, object>
                {
                    { "name", battlePassTask.Text },
                    { "count", item.Count },
                    { "maxCount", battlePassTask.Count },
                    { "exp", !item.IsDone ? battlePassTask.Exp : -1 },
                });
            }
            
            var tasksWeek = new List<Dictionary<string, object>>();

            foreach (var item in battlePassData.TasksWeek)
            {
                var battlePassTask = BattlePassTask.FirstOrDefault(b => b.Id == item.Index);
                if (battlePassTask == null)
                    continue;
                tasksWeek.Add(new Dictionary<string, object>
                {
                    { "name", battlePassTask.Text },
                    { "count", item.Count },
                    { "maxCount", battlePassTask.Count },
                    { "exp", !item.IsDone ? battlePassTask.Exp : -1 },
                });
            }
            
            var awards = String.Empty;
            var awardsPremium = String.Empty;
            if (!isInit)
            {
                awards = JsonConvert.SerializeObject(BattlePassAwards.Where(bp => bp.Gender == BattlePassRewardGender.None || (bp.Gender == BattlePassRewardGender.Man && characterData.Gender) || (bp.Gender == BattlePassRewardGender.Woman && !characterData.Gender)));
                awardsPremium = JsonConvert.SerializeObject(BattlePassAwardsPremium.Where(bp => bp.Gender == BattlePassRewardGender.None || (bp.Gender == BattlePassRewardGender.Man && characterData.Gender) || (bp.Gender == BattlePassRewardGender.Woman && !characterData.Gender)));
            }
            
            //Миссии

            var missionData = player.MissionData;
            var missionsTask = String.Empty;
            if (!sessionData.IsInitMission)
            {
                missionsTask = GetMissionTasks(missionData.Tasks);
                
                sessionData.IsInitMission = true;
            }
                                                                                
            Trigger.ClientEvent(player, $"client.battlepass.show", 
                battlePassData.Lvl, 
                battlePassData.Exp, 
                battlePassData.IsPremium, 
                JsonConvert.SerializeObject(tasksDay), JsonConvert.SerializeObject(tasksWeek), 
                JsonConvert.SerializeObject(battlePassData.TookReward), JsonConvert.SerializeObject(battlePassData.TookRewardPremium),
                awards, awardsPremium,
                battlePassData.Time,
                //Миссии
                missionsTask,
                JsonConvert.SerializeObject(missionData.Tasks), missionData.Select);
        }


        private static string GetMissionTasks(List<MissionTasks> tasks)
        {
            var missionsTask = new List<Dictionary<string, object>>();

            foreach (var item in tasks)
            {
                var battlePassTask = BattlePassTask.FirstOrDefault(b => b.Id == item.Index);
                if (battlePassTask == null)
                    continue;
                missionsTask.Add(new Dictionary<string, object>
                {
                    { "id", battlePassTask.Id },
                    { "name", battlePassTask.MissionName },
                    { "title", battlePassTask.MissionTitle },
                    { "descr", battlePassTask.Text },
                    { "maxCount", battlePassTask.Count },
                    { "money", battlePassTask.MissionMoney },
                    { "exp", Convert.ToInt32(battlePassTask.Exp / MissionExpDivide) },
                });
            }

            return JsonConvert.SerializeObject(missionsTask);
        }
        

        private static Dictionary<int, BattlePassRewardDiff> MissionSlotIdToDiff =
            new Dictionary<int, BattlePassRewardDiff>
            {
                {0, BattlePassRewardDiff.Easy},
                {1, BattlePassRewardDiff.Easy},
                {2, BattlePassRewardDiff.Easy},
                {3, BattlePassRewardDiff.Easy},
                {4, BattlePassRewardDiff.Easy},
                {5, BattlePassRewardDiff.Medium},
                {6, BattlePassRewardDiff.Easy},
                {7, BattlePassRewardDiff.Easy},
                {8, BattlePassRewardDiff.Easy},
                {9, BattlePassRewardDiff.Medium},
                {10, BattlePassRewardDiff.Easy},
                {11, BattlePassRewardDiff.Easy},
                {12, BattlePassRewardDiff.Medium},
                {13, BattlePassRewardDiff.Medium},
                {14, BattlePassRewardDiff.Hard},
                {15, BattlePassRewardDiff.Medium},
                {16, BattlePassRewardDiff.Medium},
                {17, BattlePassRewardDiff.Hard},
                {18, BattlePassRewardDiff.Hard},
                {19, BattlePassRewardDiff.Hard},
            };

        public static List<int> RandomTasks(List<BattlePassTasks> deleteTask, int maxCount, Dictionary<int, BattlePassRewardDiff> missionSlotIdToDiff = null)
        {
            var battlePassTask = BattlePassTask.ToList();

            foreach (var battlePass in deleteTask)
            {
                var battlePassDell = battlePassTask
                                        .FirstOrDefault(bt => bt.Id == battlePass.Index);
                
                if (battlePassTask.Contains(battlePassDell))
                    battlePassTask.Remove(battlePassDell);
            }

            if (battlePassTask.Count == 0) return new List<int>();

            var rand = new Random();

            var tasks = new List<int>();
            for (var i = 0; i < maxCount; i++)
            {
                var _battlePassTask = battlePassTask;

                if (missionSlotIdToDiff != null)
                    _battlePassTask = _battlePassTask.Where(bt => bt.Diff == missionSlotIdToDiff[i]).ToList();

                var index = rand.Next(0, _battlePassTask.Count);
                
                var battlePass = _battlePassTask [index];
                battlePassTask.Remove(battlePass);
                tasks.Add(battlePass.Id);
            }
            
            return tasks;
        }
        
        public static void UpdateDay(ExtPlayer player)
        {
            var missionData = player.GetMissionData();
            if (missionData == null)
                return;

            var battlePassData = player.BattlePassData;
            if (IsMaxLvl(battlePassData.Lvl))
            {
                UpdateMission(player);
                return;
            }

            var tasks = new List<BattlePassTasks>();
            var deleteTasks = battlePassData.TasksWeek.ToList();
            deleteTasks.AddRange(battlePassData.TasksDay);
            deleteTasks.Add(new BattlePassTasks
            {
                Index = missionData.Select
            });
            
            var randomTasks = RandomTasks(deleteTasks, MaxCountTasks);

            foreach (var index in randomTasks)
            {
                var battlePassTasks = new BattlePassTasks
                {
                    Index = index,
                    Count = 0,
                };
                tasks.Add(battlePassTasks);
            }

            battlePassData.TasksDay = tasks;
            
            UpdateMission(player);
        }
        
        public static void UpdateWeek(ExtPlayer player)
        {
            var missionData = player.GetMissionData();
            if (missionData == null)
                return;

            var battlePassData = player.BattlePassData;
            if (IsMaxLvl (battlePassData.Lvl))
                return;
            
            var tasks = new List<BattlePassTasks>();            
            var deleteTasks = battlePassData.TasksDay.ToList();
            deleteTasks.AddRange(battlePassData.TasksWeek);
            deleteTasks.Add(new BattlePassTasks
            {
                Index = missionData.Select
            });
            
            var randomTasks = RandomTasks(deleteTasks, MaxCountTasks);

            foreach (var index in randomTasks)
            {
                var battlePassTasks = new BattlePassTasks
                {
                    Index = index,
                    Count = 0,
                };
                tasks.Add(battlePassTasks);
            }

            battlePassData.TasksWeek = tasks;
        }

        public static void UpdateMission(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var missionData = player.GetMissionData();
            if (missionData == null)
                return;

            var battlePassData = player.BattlePassData;
                     
            var deleteTasks = battlePassData.TasksDay.ToList();
            deleteTasks.AddRange(battlePassData.TasksWeek);
            foreach (var item in missionData.Tasks)
            {
                deleteTasks.Add(new BattlePassTasks
                {
                    Index = item.Index
                });
            }
            
            var randomTasks = RandomTasks(deleteTasks, 20, MissionSlotIdToDiff);

            if (randomTasks.Count == 0)
                return;
            
            var tasks = new List<MissionTasks>();   
            foreach (var index in randomTasks)
            {
                var missionTasks = new MissionTasks
                {
                    Index = index,
                    Count = 0,
                };
                tasks.Add(missionTasks);
            }

            missionData.Tasks = tasks;
            missionData.Select = tasks[0].Index;
            sessionData.IsInitMission = false;
        }
        //
        
        public static void UpdateReward(ExtPlayer player, int id, bool isEnd = false)
        {
            if (!player.IsCharacterData()) 
                return;

            var battlePassTask = BattlePassTask.FirstOrDefault(b => b.Id == id);
            if (battlePassTask == null)
                return;
            
            var battlePass = player.BattlePassData.TasksDay
                .FirstOrDefault(t => t.Index == id && !t.IsDone);

            if (battlePass != null)
            {
                battlePass.Count++;
                
                if (battlePass.Count >= battlePassTask.Count)
                {
                    battlePass.IsDone = true;
                    AddExp(player, battlePassTask.Exp);
                    
                    Trigger.ClientEvent(player, "client.battlepass.missionComplite", battlePassTask.Text, LangFunc.GetText(LangType.Ru, DataName.BPEverydayComplete));
                }
            }

            battlePass = player.BattlePassData.TasksWeek
                .FirstOrDefault(t => t.Index == id && !t.IsDone);

            if (battlePass != null)
            {
                battlePass.Count++;
                
                if (battlePass.Count >= battlePassTask.Count)
                {
                    battlePass.IsDone = true;
                    AddExp(player, battlePassTask.Exp);
                    
                    Trigger.ClientEvent(player, "client.battlepass.missionComplite", battlePassTask.Text, LangFunc.GetText(LangType.Ru, DataName.BPEverydayComplete));
                }
            }
            
            //Миссии

            var missionData = player.GetMissionData();

            if (missionData.Select == id)
            {
                var mission = missionData.Tasks
                    .FirstOrDefault(t => t.Index == id);

                if (mission != null && !mission.IsDone)
                {
                    mission.Count++;

                    if (isEnd)
                        mission.Count = battlePassTask.Count;
                    
                    if (mission.Count >= battlePassTask.Count)
                    {
                        mission.IsDone = true;
                        
                        Trigger.ClientEvent(player, "client.battlepass.missionComplite", battlePassTask.Text, LangFunc.GetText(LangType.Ru, DataName.MissionComplete));
                    }
                }
            }
        }

        public static string GetSerial()
        {
            try
            {
                var rand = new Random();
                int serial = rand.Next(100_000, 999_999);
                return $"BT{(int)SeasonId}{serial}";
            }
            catch (Exception e)
            {
                Log.Write($"GetSerial Exception: {e.ToString()}");
                return $"BT{(int)SeasonId}000000";
            }
        }

        public static void GiveBonus(ExtPlayer player, BattlePassReward award, bool isMessage = true)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
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

                        GameLog.Money($"system", $"player({characterData.UUID})", 1, $"BPGiveBonus({award.ItemId},{award.Data})");
                        if (isMessage) 
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGetItemSklad, Chars.Repository.ItemsInfo[(ItemId)award.ItemId].Name), 6000);
                        break;
                    case BattlePassRewardType.Vip:
                        Chars.Repository.UpdateVipStatus(player, award.ItemId, award.Count, true, true, "BonusVIP");
                        GameLog.Money($"system", $"player({characterData.UUID})", 1, $"BPGiveBonus(VIP,{award.ItemId},{award.Data})");
                        if (isMessage)
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGetVipDays, Group.GroupNames[award.ItemId], award.Count), 6000);
                        break;
                    case BattlePassRewardType.Money:
                        MoneySystem.Wallet.Change(player, +award.Count);
                        GameLog.Money($"system", $"player({characterData.UUID})", award.Count, $"BPGiveBonus");
                        //PlayerStats(player);
                        if (isMessage) 
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouWonMoneyAmount, MoneySystem.Wallet.Format(award.Count)), 6000);
                        break;
                    case BattlePassRewardType.Donate:
                        UpdateData.RedBucks(player, award.Count, msg: "BPGiveBonus");
                        if (isMessage) 
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouWinRb, award.Count), 5000);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"GiveBonus Exception: {e.ToString()}");
            }
        }
        
        [RemoteEvent("server.battlepass.take")]
        public void Take(ExtPlayer player, int index, bool isPrem)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            var battlePassData = player.BattlePassData;

            var award = (!isPrem ? BattlePassAwards : BattlePassAwardsPremium).FirstOrDefault(bp => bp.Id == index && (bp.Gender == BattlePassRewardGender.None || (bp.Gender == BattlePassRewardGender.Man && characterData.Gender) || (bp.Gender == BattlePassRewardGender.Woman && !characterData.Gender)));

            if (award != null && (!isPrem || battlePassData.IsPremium))
            {
                if (award.Id >= battlePassData.Lvl)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BpNoLevel), 6000);
                    return;
                }
                
                if (award.Type == BattlePassRewardType.None)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantTakeThisItem), 3000);
                    return;
                }
                
                if ((!isPrem && battlePassData.TookReward.Contains(award.Id)) || (isPrem && battlePassData.TookRewardPremium.Contains(award.Id)))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyTakeAward), 3000);
                    Trigger.ClientEvent(player, $"client.battlepass.takeSuccess", JsonConvert.SerializeObject(battlePassData.TookReward), JsonConvert.SerializeObject(battlePassData.TookRewardPremium));
                    return;
                }
                
                GiveBonus(player, award);
                
                if (!isPrem)
                    battlePassData.TookReward.Add(award.Id);
                else
                    battlePassData.TookRewardPremium.Add(award.Id);
                
                Trigger.ClientEvent(player, $"client.battlepass.takeSuccess", JsonConvert.SerializeObject(battlePassData.TookReward), JsonConvert.SerializeObject(battlePassData.TookRewardPremium));
                return;
            }
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoActiveAwards), 6000);
        }
        
        
        [RemoteEvent("server.battlepass.takeAll")]
        public void TakeAll(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            var isAdd = false;
            var battlePassData = player.BattlePassData;

            var awards = BattlePassAwards
                .Where(pa => pa.Id < battlePassData.Lvl && !battlePassData.TookReward.Contains(pa.Id) && pa.Type != BattlePassRewardType.None && (pa.Gender == BattlePassRewardGender.None || (pa.Gender == BattlePassRewardGender.Man && characterData.Gender) || (pa.Gender == BattlePassRewardGender.Woman && !characterData.Gender)));
            
            foreach (var award in awards)
            {
                if (battlePassData.TookReward.Contains(award.Id))
                    continue;
                
                GiveBonus(player, award, isMessage: false);
                battlePassData.TookReward.Add(award.Id);
                isAdd = true;
            }

            if (battlePassData.IsPremium)
            {
                var awardsPremium = BattlePassAwardsPremium
                    .Where(pa => pa.Id < battlePassData.Lvl && !battlePassData.TookRewardPremium.Contains(pa.Id) && pa.Type != BattlePassRewardType.None && (pa.Gender == BattlePassRewardGender.None || (pa.Gender == BattlePassRewardGender.Man && characterData.Gender) || (pa.Gender == BattlePassRewardGender.Woman && !characterData.Gender)));

                foreach (var award in awardsPremium)
                {
                    if (battlePassData.TookRewardPremium.Contains(award.Id))
                        continue;
                    
                    GiveBonus(player, award, isMessage: false);
                    battlePassData.TookRewardPremium.Add(award.Id);
                    isAdd = true;
                }
            }

            if (isAdd)
            {
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouTakeAllAwards), 3000);
                Trigger.ClientEvent(player, $"client.battlepass.takeSuccess", JsonConvert.SerializeObject(battlePassData.TookReward), JsonConvert.SerializeObject(battlePassData.TookRewardPremium));
            }
            else
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoActiveAwards), 7000);
            }
        }

        //[RemoteEvent("server.battlepass.buyPremium")]
        public static void BuyPremium(ExtPlayer player, bool isBuyDonate = false)
        {
            
            var accountData = player.GetAccountData();
            if (accountData == null)
                return;
            
            var battlePassData = player.BattlePassData;
            if (battlePassData.IsPremium)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyHaveBp), 3000);
                return;
            }
            if (DonatePack.IsDonate(player))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PaymetnLoad), 5000);
                return;
            }
            
            if (!isBuyDonate && accountData.RedBucks < PricePremium)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);
                return;
            }
            
            battlePassData.IsPremium = true;
            
            //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouBuyBp), 3000);
            Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.RedAge, LangFunc.GetText(LangType.Ru, DataName.YouBuyBp), DateTime.Now);
            
            if (!isBuyDonate)
                UpdateData.RedBucks(player, -PricePremium, msg:"BPBuyPremium");
            
            Trigger.ClientEvent(player, "client.battlepass.buyPremiumSuccess");
        }
        
        //
        
        private static int MaxExp = 50;
        private static int MissionExpDivide = 5;
        
        public static void AddExp(ExtPlayer player, int exp)
        {
            if (player == null)
                return;
            
            var battlePassData = player.BattlePassData;

            battlePassData.Exp += exp;

            if (battlePassData.Exp >= MaxExp)
            {
                battlePassData.Exp = battlePassData.Exp - MaxExp;
                AddLvl(player);
            }
        }
        
        //

        private static int MaxLvl = BattlePassAwards
                                    .GroupBy(bp => bp.Id)
                                    .ToList().Count;

        public static bool IsMaxLvl(int lvl) => lvl >= MaxLvl;

        public static void AddLvl(ExtPlayer player, int lvl = 1)
        {
            var accountData = player.GetAccountData();
            if (accountData == null)
                return;
            
            var battlePassData = player.BattlePassData;

            battlePassData.Lvl += lvl;

            GameLog.AccountLog(accountData.Login, accountData.HWID, accountData.IP, accountData.SocialClub, $"BattlePassLvlUp({battlePassData.Lvl})");
            
            if (IsMaxLvl (battlePassData.Lvl))
            {
                battlePassData.Lvl = MaxLvl;
                battlePassData.Exp = 0;
                battlePassData.TasksDay.Clear();
                battlePassData.TasksWeek.Clear();
            }
        }
        
        //
        
        private static int MaxTime = 60 * 3; //Бонус каждые 3 часы
        private static int TimeExp = 2; //Дает при кажом 3 - м часе

        public static void UpdateTime(ExtPlayer player)
        {
            if (player == null)
                return;
            
            var battlePassData = player.BattlePassData;
            if (battlePassData == null)
                return;
            
            battlePassData.Time++;

            if (battlePassData.Time >= MaxTime)
            {
                battlePassData.Time = 0;
                AddExp(player, TimeExp);
            }

        }

        //

        private static List<BattlePassBuyLvl> BattlePassBuyLvls = new List<BattlePassBuyLvl>
        {
            new BattlePassBuyLvl(1600, 1),
            new BattlePassBuyLvl(6666, 5),
            new BattlePassBuyLvl(29999, 25),
        };


        [RemoteEvent("server.battlepass.buyLvl")]
        public void BuyLvl(ExtPlayer player, int index)
        {
            
            var accountData = player.GetAccountData();
            if (accountData == null)
                return;

            var buyLvl = BattlePassBuyLvls[index];
            
            if (accountData.RedBucks < buyLvl.PriceRB)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);
                return;
            }
            var battlePassData = player.BattlePassData;
            if (IsMaxLvl (battlePassData.Lvl))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyMaxLvl), 3000);
                return;
            }

            AddLvl(player, buyLvl.Lvl);
            
            //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BpBuyLvl), 7000);
            Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.RedAge, LangFunc.GetText(LangType.Ru, DataName.BpBuyLvl, buyLvl.Lvl, buyLvl.PriceRB), DateTime.Now);
            UpdateData.RedBucks(player, -buyLvl.PriceRB, msg:$"BPBuyLvl({buyLvl.Lvl})");
            battlePassData = player.BattlePassData;
            
            Trigger.ClientEvent(player, "client.battlepass.updateLvlAndExp", battlePassData.Lvl, battlePassData.Exp, IsMaxLvl (battlePassData.Lvl));
        }
        
        
        //Миссии
        public static bool isStatusActive(List<MissionTasks> tasks, int index)
        {
            return tasks[index].IsReward;
        }
        
        public static bool isActiveBox (ExtPlayer player, int id)
        {
            var missionData = player.GetMissionData();

            if (missionData == null)
                return false;

            var index = missionData.Tasks
                .FindIndex(mt => mt.Index == id);

            if (index == -1)
                return false;
            
            if (index == 0)
                return true;
            
            if ((index == 1 || index == 2) && isStatusActive (missionData.Tasks, 0))
                return true;

            if ((index == 3) && isStatusActive (missionData.Tasks, 1))
                return true;

            if ((index == 4) && isStatusActive (missionData.Tasks, 3))
                return true;

            if ((index == 5) && isStatusActive (missionData.Tasks, 4))
                return true;

            //

            if ((index == 6) && isStatusActive (missionData.Tasks, 2))
                return true;

            if ((index == 7 || index == 10) && isStatusActive (missionData.Tasks, 6))
                return true;

            if ((index == 8) && isStatusActive (missionData.Tasks, 7))
                return true;

            if ((index == 9) && isStatusActive (missionData.Tasks, 8))
                return true;

            if ((index == 11) && isStatusActive (missionData.Tasks, 10))
                return true;

            if ((index == 12 || index == 13) && isStatusActive (missionData.Tasks, 11))
                return true;

            if ((index == 14) && isStatusActive (missionData.Tasks, 12))
                return true;

            if ((index == 15) && isStatusActive (missionData.Tasks, 13))
                return true;

            if ((index == 16) && isStatusActive (missionData.Tasks, 15))
                return true;

            if ((index == 17) && isStatusActive (missionData.Tasks, 16))
                return true;

            if ((index == 18) && isStatusActive (missionData.Tasks, 17))
                return true;

            if ((index == 19) && isStatusActive(missionData.Tasks, 18))
                return true;

            if ((index == 20) && isStatusActive (missionData.Tasks, 19))
                return true;

            return false;
        }


        private static readonly int OneMissionExp = 2;
        private static readonly int AllMissionExp = 10;
        private static readonly int AllMissionMoney = 2000;
        
        [RemoteEvent("server.battlepass.setMissions")]
        public static void SetMissions(ExtPlayer player, int id)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            var missionData = player.GetMissionData();
            if (missionData == null)
                return;

            if (!isActiveBox (player, id))
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DontCompletePrevios), 3000);
                return;
            }

            if (missionData.Tasks.Any(t => t.Index == id && t.IsReward))
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.QuestCompletedAward), 3000);
                return;
            }

            var battlePassTask = BattlePassTask.FirstOrDefault(b => b.Id == id);
            if (battlePassTask == null)
                return;
            
            if (missionData.Select != id)
            {
                missionData.Select = id;
                Trigger.ClientEvent(player, "client.battlepass.updateMissions", missionData.Select, JsonConvert.SerializeObject(missionData.Tasks));
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouTakeMissonBp, battlePassTask.Text), 3000);
            }
            else
            {
                var task = missionData.Tasks
                    .FirstOrDefault(t => t.Index == id);
                
                if (task != null && task.IsDone && !task.IsReward)
                {
                    task.IsReward = true;
                    missionData.Select = -1;

                    var count = missionData.Tasks.Where(t => t.IsReward).Count();
                    
                    if (count == 20)
                    {
                        AddExp(player, AllMissionExp);
                        Wallet.Change(player, AllMissionMoney);
                        GameLog.Money($"player({characterData.UUID})", $"system", AllMissionMoney, $"BPsetMissionsAll"); 
                        Trigger.SendChatMessage(player,LangFunc.GetText(LangType.Ru, DataName.YouGetBonusBp));
                    }
                    
                    Trigger.ClientEvent(player, "client.battlepass.updateMissions", missionData.Select, JsonConvert.SerializeObject(missionData.Tasks));
                    //
                    AddExp(player, OneMissionExp);
                    MoneySystem.Wallet.Change(player, battlePassTask.MissionMoney);
                    GameLog.Money($"player({characterData.UUID})", $"system", battlePassTask.MissionMoney, $"BPsetMissions");         
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BpTake), 3000);
                    return;
                }
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouDontCompleteQuest), 3000);
            }

        }
        
        [RemoteEvent("server.battlepass.skip")]
        public static void OnSkip(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                var accountData = player.GetAccountData();
                if (accountData == null)
                    return;
                
                var missionData = player.GetMissionData();
                if (missionData == null)
                    return;

                if (missionData.Select != -1)
                {
                    if (accountData.RedBucks < 150)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);
                        return;
                    }
            
                    UpdateData.RedBucks(player, -150, msg:"BPOnSkip");
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCompleteQuest), 3000);
                    
                    UpdateReward(player, missionData.Select, isEnd: true);
                    SetMissions(player, missionData.Select);
                }
                else
                {
                    var count = missionData.Tasks.Where(t => t.IsReward).Count();
                    
                    if (count < 20)
                    {
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantRefreshMissons), 3000);
                        return;
                    }
                    if (accountData.RedBucks < 500)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);
                        return;
                    }
            
                    UpdateData.RedBucks(player, -500, msg:"BPOnSkip");
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RefreshMissions), 3000);
                    
                    UpdateMission(player);         
                    
                    var missionsTask = GetMissionTasks(missionData.Tasks);
                
                    sessionData.IsInitMission = true;

                    Trigger.ClientEvent(player, "client.battlepass.updateMissions", missionData.Select, JsonConvert.SerializeObject(missionData.Tasks), missionsTask);
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_t1 Exception: {e.ToString()}");
            }
        }
        
        
        

    }
}