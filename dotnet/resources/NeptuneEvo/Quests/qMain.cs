using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Chars;

using System.Linq;
using LinqToDB;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NeptuneEvo.Events;
using NeptuneEvo.Jobs;
using System.Collections.Concurrent;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Quests.Models;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Fractions;
using NeptuneEvo.Functions;
using NeptuneEvo.Players;

namespace NeptuneEvo.Quests
{
    class qMain : Script
    {
        private static readonly nLog Log = new nLog("Quests.Main");

        private static string[] DefaultQuests = new string[]
        {
            Zdobich.QuestName,
        };


        [RemoteEvent("server.quest.perform")]
        public static void QuestPerform(ExtPlayer player, bool isClose)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                var questData = player.GetQuest();
                if (questData == null) 
                    return;

                if (isClose)
                {
                    player.ClearQuest();
                    
                    BattlePass.Repository.UpdateReward(player, 97);
                }

               
                if (questData.ActorName == "npc_airdrop") Events.AirDrop.Repository.Perform(player);
                else if (questData.ActorName == "npc_oressale") Miner.Perform(player);
                else if (questData.ActorName == "npc_fracpolic") Fractions.Police.Perform(player);
                else if (questData.ActorName == "npc_fracsheriff") Fractions.Sheriff.Perform(player);
                else if (questData.ActorName == "npc_premium") 
                    Core.BusinessManager.OpenClothes(player, null, true, addClothes: new List<string> { ClothesComponent.Bugs.ToString(), ClothesComponent.Masks.ToString() });
                else if (questData.ActorName == "npc_stock") Stock.Perform(player, 0);
                else if (questData.ActorName == VehicleModel.DonateAutoRoom.NpcName) VehicleModel.DonateAutoRoom.Perform(player);
                else if (questData.ActorName == VehicleModel.AirAutoRoom.NpcName) VehicleModel.AirAutoRoom.Perform(player);
                else if (questData.ActorName == VehicleModel.EliteAutoRoom.NpcName) VehicleModel.EliteAutoRoom.Perform(player);
                else if (questData.ActorName == "npc_fracems") Fractions.Ems.Perform(player);
                else if (questData.ActorName == "npc_pet") PedSystem.Pet.Repository.RespawnPet(player);
                else if (questData.ActorName == "npc_petshop") PedSystem.Pet.Repository.OpenPetShop(player);
                else if (questData.ActorName == "npc_huntingshop") Lumberjack.Perform(player, 1);
                else if (questData.ActorName == "npc_treessell") Lumberjack.Perform(player, 2);
                else if (questData.ActorName == "npc_wedding") Wedding.Perform(player);
                else if (questData.ActorName == "npc_rieltor") Houses.Rieltagency.Repository.OnOpen(player);
                else if (questData.ActorName == Zdobich.QuestName) Zdobich.Perform(player, questData);
                else if (questData.ActorName == Houses.FurnitureManager.QuestName) Houses.HouseManager.OpenFurniture(player);
                else if (questData.ActorName == Fractions.Ticket.QuestName) Fractions.Ticket.OpenTickets(player);
                else if (questData.ActorName == "npc_org") Organizations.Manager.Perform(player);
                else if (questData.ActorName == "npc_birthday") Wedding.OpenBonus(player, 0);
                
                
            }
            catch (Exception e)
            {
                Log.Write($"QuestPerform Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.quest.take")]
        public static void QuestTake(ExtPlayer player, int index)
        {
            try
            {
                var characterData = player.GetCharacterData();

                if (characterData == null) return;
                var questData = player.GetQuest();
                if (questData == null) 
                    return;
                
                player.ClearQuest();
                BattlePass.Repository.UpdateReward(player, 97);

                if (questData.ActorName == "npc_stock") Stock.Perform(player, 1);
                else if (questData.ActorName == "npc_org") Organizations.Manager.Take(player);
                else if (questData.ActorName == "npc_petshop")
                {
                    switch (index)
                    {
                        case 0:
                            PedSystem.Pet.Repository.OpenPetMenu(player);
                            break;
                        case 1:
                            PedSystem.Pet.Repository.Sell(player);
                            break;
                    }
                    return;
                }
                else if (questData.ActorName == "npc_airdrop")
                {
                    Events.AirDrop.Repository.BuyInfo(player);
                    return;
                }
                else if (questData.ActorName == "npc_fracpolic" || questData.ActorName == "npc_fracsheriff")
                {
                    switch(index)
                    {
                        case 0:
                            Fractions.Police.PlayerSoloArrest(player);
                            break;
                        case 1:
                            Fractions.Police.TakeIllegalStuff(player);
                            break;
                    }
                }
                else if (questData.ActorName == Zdobich.QuestName) 
                    Zdobich.Take(player, questData.Line);

                UpdateTake(player, questData.ActorName);

            }
            catch (Exception e)
            {
                Log.Write($"QuestTake Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.quest.action")]
        public static void QuestAction(ExtPlayer player, bool isClose)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                var questData = player.GetQuest();
                if (questData == null) 
                    return;

                if (isClose)
                    player.ClearQuest();

                if (questData.ActorName == "npc_granny") Granny.Action(player, questData.Line);
                else if (questData.ActorName == "npc_fd_zak") Zak.Action(player, questData.Line);

            }
            catch (Exception e)
            {
                Log.Write($"QuestAction Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.quest.clear")]
        public static void QuestClear(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                BattlePass.Repository.UpdateReward(player, 97);
                player.ClearQuest();
            }
            catch (Exception e)
            {
                Log.Write($"QuestClear Exception: {e.ToString()}");
            }
        }
        /// <summary>
        /// Нужно для инициализации квеста
        /// </summary>
        /// <param name="player"></param>
        /// <param name="ActorName"></param>
        /// <param name="isInsert"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public static bool SetQuests(ExtPlayer player, string ActorName, bool isInsert = false, sbyte Status = 0, bool isReturn = true)
        {
            try
            {
                var characterData = player.GetCharacterData();

                if (characterData == null) 
                    return false;

                player.ClearQuest();

                var questData = characterData.QuestsData
                    .FirstOrDefault(v => v.ActorName == ActorName);

                if (questData == null)
                {
                    DateTime QuestTime = DateTime.Now;
                    questData = new QuestData
                    {
                        ActorName = ActorName,
                        Line = 0,
                        Status = Status,
                        Time = QuestTime,
                        Data = "0",
                        Use = true
                    };
                    characterData.QuestsData.Add(questData);
                    if (isInsert)
                    {
                        Trigger.SetTask(async () =>
                        {
                            try
                            {
                                await using var db = new ServerBD("MainDB");//В отдельном потоке

                                var sqlId = await db.InsertWithInt32IdentityAsync(new Questschars
                                {
                                    QActorName = ActorName,
                                    QStatus = Status,
                                    QTime = QuestTime,
                                    CharId = characterData.UUID,
                                    QData = "0",
                                    QUse = true
                                });

                                questData = characterData.QuestsData
                                    .FirstOrDefault(v => v.ActorName == ActorName);

                                if (questData == null) return;

                                questData.AutoId = sqlId;
                            }
                            catch (Exception e)
                            {
                                Debugs.Repository.Exception(e);
                            }
                        });
                    }
                    if (isReturn)
                        player.SelectQuest(new PlayerQuestModel(ActorName, isInsert ? 0 : -1, 0, false, QuestTime));
                    return true;
                }
                
                if (isReturn)
                    player.SelectQuest(new PlayerQuestModel(ActorName, questData.Line, questData.Status, questData.Complete, questData.Time));
                
                return true;
            }
            catch (Exception e)
            {
                Log.Write($"SetQuests Exception: {e.ToString()}");
                return false;
            }
        }
        public static async Task<List<QuestData>> Load(ServerBD db, int uuid)
        {
            try
            {
                var questsData = await db.Questschar
                    .Where(v => v.CharId == uuid)
                    .ToListAsync();

                var returnQuestsData = new List<QuestData>();
                foreach (var questData in questsData)
                {
                    var qData = new QuestData
                    {
                        AutoId = questData.QAutoId,
                        ActorName = questData.QActorName,
                        Line = questData.QLine,
                        Status = questData.QStatus,
                        Time = questData.QTime,
                        Complete = questData.QComplete,
                        Stage = questData.QStage,
                        Data = questData.QData,
                        Use = questData.QUse
                    };
                    
                    returnQuestsData.Add(qData);
                }
                return returnQuestsData;
            }
            catch (Exception e)
            {
                Log.Write($"Load Exception: {e.ToString()}");
            }
            return new List<QuestData>();
        }

        public static async Task Save(ServerBD db, ExtPlayer player, int uuid)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                foreach (var questData in characterData.QuestsData)
                {
                    await db.Questschar
                        .Where(q => q.CharId == uuid && q.QActorName == questData.ActorName)
                        .Set(q => q.QLine, questData.Line)
                        .Set(q => q.QStatus, questData.Status)
                        .Set(q => q.QTime, questData.Time)
                        .Set(q => q.QComplete, questData.Complete)
                        .Set(q => q.QStage, questData.Stage)
                        .Set(q => q.QData, questData.Data)
                        .Set(q => q.QUse, questData.Use)
                        .UpdateAsync();
                }
            }
            catch (Exception e)
            {
                Log.Write($"Save Exception: {e.ToString()}");
            }
        }
        public static void InitQuests(ExtPlayer player, List<QuestData> questsData, bool isSpawn = false)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                //Инициализация стандартных кейсов

                if (isSpawn)
                {
                    foreach (var questName in DefaultQuests)
                    {
                        if (!questsData.Any(q => q.ActorName == questName))
                        {
                            SetQuests(player, questName, true, 0, isReturn: false);
                        }
                    }
                    foreach (var qData in characterData.QuestsData)
                    {
                        if (qData.ActorName == "npc_granny" && qData.Line == (short)granny_quests.Bear && int.TryParse(qData.Data, out int count) && count < 5)
                        {
                            Trigger.ClientEvent(player, "client.start.collecting_items", count);
                        }
                        else if (qData.ActorName == "npc_fd_dada" && qData.Line == (short)data_quests.Boat)
                        {
                            Trigger.ClientEvent(player, "client.create.npc_dfday_mission", 1);
                        }
                        else if (qData.ActorName == "npc_fd_edward" && qData.Line == (short)edward_quests.Docs)
                        {
                            Trigger.ClientEvent(player, "client.create.npc_dfday_mission", 2);
                        }
                    }

                    var questData = characterData.QuestsData
                        .FirstOrDefault(qd => qd.ActorName == Zdobich.QuestName);
                    
                    if (questData != null &&
                        new List<zdobich_quests> { zdobich_quests.Stage12, zdobich_quests.Stage13, zdobich_quests.Stage14, zdobich_quests.Stage15, zdobich_quests.Stage16, zdobich_quests.Stage17, zdobich_quests.Stage18, zdobich_quests.Stage19 }.Contains((zdobich_quests) questData.Line))
                    {
                        questData.Line = (int)zdobich_quests.Stage11;
                        questData.Status = 0;
                        questData.Complete = false;
                        questData.Stage = 0;
                        questData.Data = "0";
                    }
                    
                    Trigger.ClientEvent(player, "client.quest.selectedQuest", characterData.SelectedQuest);
                }

                //

                questsData = characterData.QuestsData
                    .Where(q => q.Use == true)
                    .ToList();

                
                var jsonQuestsData = JsonConvert.SerializeObject(questsData);

                Trigger.ClientEvent(player, "client.questStore.init", jsonQuestsData);
            }
            catch (Exception e)
            {
                Log.Write($"InitQuests Exception: {e.ToString()}");
            }
        }
        public static bool UpdateQuestsStage(ExtPlayer player, string ActorName, int Line, sbyte Stage, bool isUpdateHud = false)
        {
            try
            {
                var characterData = player.GetCharacterData();

                if (characterData == null) 
                    return false;

                var questData = characterData.QuestsData
                    .FirstOrDefault(v => v.ActorName == ActorName && v.Line == Line);

                if (questData == null)
                    return false;

                questData.Stage = Stage;
                
                if (isUpdateHud)
                    InitQuests(player, characterData.QuestsData);
                
                return true;

            }
            catch (Exception e)
            {
                Log.Write($"UpdateQuestsStage Exception: {e.ToString()}");
            }
            return false;
        }
        public static bool UpdateQuestsStage(ExtPlayer player, string ActorName, int Line, sbyte oldStage, sbyte newStage, bool isUpdateHud = false)
        {
            try
            {
                var characterData = player.GetCharacterData();

                if (characterData == null) 
                    return false;

                var questData = characterData.QuestsData
                    .FirstOrDefault(v => v.ActorName == ActorName && v.Line == Line && v.Stage == oldStage);

                if (questData == null)
                    return false;

                questData.Stage = newStage;
                
                if (isUpdateHud)
                    InitQuests(player, characterData.QuestsData);
                
                return true;
            }
            catch (Exception e)
            {
                Log.Write($"UpdateQuestsStage Exception: {e.ToString()}");
            }
            return false;
        }
        public static void UpdateQuestsComplete(ExtPlayer player, string ActorName, int Line, bool Complete)
        {
            try
            {

                var characterData = player.GetCharacterData();

                if (characterData == null) 
                    return;

                var questData = characterData.QuestsData
                    .FirstOrDefault(v => v.ActorName == ActorName && v.Line == Line);

                if (questData == null)
                    return;

                questData.Complete = Complete;
            }
            catch (Exception e)
            {
                Log.Write($"UpdateQuestsComplete Exception: {e.ToString()}");
            }
        }
        public static void UpdateQuestsLine(ExtPlayer player, string ActorName, int Line, int newLine)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
                
                var useQuestData = player.GetQuest();

                if (useQuestData != null)
                    useQuestData.Line = newLine;

                var questData = characterData.QuestsData
                    .FirstOrDefault(v => v.ActorName == ActorName && v.Line == Line);

                if (questData == null)
                    return;

                //questData.Line = (short)newLine;
                UpdatePerform(player, ActorName, newLine);
                InitQuests(player, characterData.QuestsData);
            }
            catch (Exception e)
            {
                Log.Write($"UpdateQuestsLine Exception: {e.ToString()}");
            }
        }
        public static int GetQuestsLine(ExtPlayer player, string ActorName)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return -9999;
                
                var questData = characterData.QuestsData
                    .FirstOrDefault(v => v.ActorName == ActorName && v.Complete == false);

                if (questData == null)
                    return -9999;

                return questData.Line;
            }
            catch (Exception e)
            {
                Log.Write($"UpdateQuestsLine Exception: {e.ToString()}");
            }
            return -9999;
        }
        public static void UpdateQuestsStatus(ExtPlayer player, string ActorName, int Line, short Status)
        {
            try
            {
                var characterData = player.GetCharacterData();

                if (characterData == null) return;

                var questData = characterData.QuestsData
                    .FirstOrDefault(v => v.ActorName == ActorName && v.Line == Line);

                if (questData == null)
                    return;

                questData.Status = (sbyte)Status;
            }
            catch (Exception e)
            {
                Log.Write($"UpdateQuestsStatus Exception: {e.ToString()}");
            }
        }
        /// <summary>
        /// Обновление информации если в квесте несколько условий (false,false,false)
        /// </summary>
        /// <param name="player"></param>
        /// <param name="ActorName"></param>
        /// <param name="Line"></param>
        /// <param name="IndexData"></param>
        /// <param name="Toggled"></param>
        public static void UpdateQuestsData(ExtPlayer player, string ActorName, int Line, string Data)
        {
            try
            {
                var characterData = player.GetCharacterData();

                if (characterData == null) 
                    return;

                var questData = characterData.QuestsData
                    .FirstOrDefault(v => v.ActorName == ActorName && v.Line == Line);

                if (questData == null)
                    return;

                questData.Data = Data;
            }
            catch (Exception e)
            {
                Log.Write($"UpdateQuestsData Exception: {e.ToString()}");
            }
        }
        public static int GetQuestsData(ExtPlayer player, string ActorName)
        {
            try
            {
                var characterData = player.GetCharacterData();

                if (characterData == null) 
                    return 0;

                var questData = characterData.QuestsData
                    .FirstOrDefault(v => v.ActorName == ActorName);

                if (questData == null)
                    return 0;

                return Convert.ToInt32(questData.Data);
            }
            catch (Exception e)
            {
                Log.Write($"UpdateQuestsData Exception: {e.ToString()}");
            }
            return 0;
        }
        public static int GetQuestsData(ExtPlayer player, string ActorName, int Line)
        {
            try
            {
                var characterData = player.GetCharacterData();

                if (characterData == null) 
                    return 0;

                var questData = characterData.QuestsData
                    .FirstOrDefault(v => v.ActorName == ActorName && v.Line == Line);

                if (questData == null)
                    return 0;

                return Convert.ToInt32(questData.Data);
            }
            catch (Exception e)
            {
                Log.Write($"UpdateQuestsData Exception: {e.ToString()}");
            }
            return 0;
        }
        public static void UpdatePerform(ExtPlayer player, string ActorName, int Line)
        {
            try
            {
                var characterData = player.GetCharacterData();

                if (characterData == null) 
                    return;

                var questData = characterData.QuestsData
                    .FirstOrDefault(v => v.ActorName == ActorName);

                if (questData == null)
                    return;

                questData.Line = (short)Line;
                questData.Status = 0;
                questData.Complete = false;
                questData.Stage = 0;
                questData.Data = "0";
                
                if (questData.Line == -1)
                    questData.Use = false;
                
                InitQuests(player, characterData.QuestsData);
            }
            catch (Exception e)
            {
                Log.Write($"UpdatePerform Exception: {e.ToString()}");
            }
        }
        public static void UpdateDisplayInHood(ExtPlayer player, string ActorName, bool isUse)
        {
            try
            {
                var characterData = player.GetCharacterData();

                if (characterData == null)
                    return;

                var questData = characterData.QuestsData
                    .FirstOrDefault(v => v.ActorName == ActorName);

                if (questData == null)
                    return;

                questData.Use = isUse;

                if (isUse)
                    InitQuests(player, characterData.QuestsData);
            }
            catch (Exception e)
            {
                Log.Write($"UpdatePerform Exception: {e.ToString()}");
            }
        }
        public static void UpdateTake(ExtPlayer player, string ActorName)
        {
            try
            {
                var characterData = player.GetCharacterData();

                if (characterData == null)
                    return;

                var questData = characterData.QuestsData
                    .FirstOrDefault(v => v.ActorName == ActorName);

                if (questData == null)
                    return;

                questData.Status = 1;
                questData.Complete = false;
                questData.Use = true;

                InitQuests(player, characterData.QuestsData);
            }
            catch (Exception e)
            {
                Log.Write($"UpdatePerform Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.quest.selectQuest")]
        public static void SelectQuest(ExtPlayer player, string actorName)
        {
            
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            
            characterData.SelectedQuest = actorName;
        }
        [Command(AdminCommands.SkipQuest)]
        public static void SkipQuest(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.SkipQuest)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                ExtPlayer target = Main.GetPlayerByID(id);
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }

                var questData = targetCharacterData.QuestsData
                    .FirstOrDefault(q => q.ActorName == targetCharacterData.SelectedQuest);
                
                if (questData == null) 
                    return;
                
                questData.Status = 1;
                questData.Complete = true;
                UpdateQuestsStage(player, questData.ActorName, questData.Line, 1, isUpdateHud: true);
                UpdateQuestsComplete(player, questData.ActorName, questData.Line, true);
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы пропустили квест {questData.ActorName} игроку {target.Name}", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_Clear Exception: {e.ToString()}");
            }
        }
    }
}
