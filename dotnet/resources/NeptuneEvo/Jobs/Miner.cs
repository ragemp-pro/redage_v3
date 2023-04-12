using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Database;
using GTANetworkAPI;
using LinqToDB;
using Localization;
using NeptuneEvo.Handles;
using MySqlConnector;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions;
using NeptuneEvo.Functions;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Quests;
using Newtonsoft.Json;
using Redage.SDK;
using NeptuneEvo.Quests.Models;
using NeptuneEvo.Table.Tasks.Models;
using NeptuneEvo.Table.Tasks.Player;
using NeptuneEvo.VehicleData.LocalData;

namespace NeptuneEvo.Jobs
{
    class Miner : Script
    {
        private static readonly nLog Log = new nLog("Jobs.Miner");

        private static Dictionary<int, int> MinerJobPlayersOreTypeInfo = new Dictionary<int, int>();

        private static List<int> MineStockOres = new List<int>()
        {
            0, // Ископаемый уголь
            0, // Железная руда
            0, // Золотая руда
            0, // Серная руда
            0, // Изумруд
            0 // Рубин
        };

        private static List<int> PlantStockOres = new List<int>()
        {
            0, // Ископаемый уголь
            0, // Железная руда
            0, // Золотая руда
            0, // Серная руда
            0, // Изумруд
            0 // Рубин
        };

        public static ExtTextLabel MineStockLabel { get; set; } = null;
        public static ExtTextLabel PlantStockLabel { get; set; } = null;

        [ServerEvent(Event.ResourceStart)]
        public void Event_ResourceStart()
        {
            try
            {
                
                Main.CreateBlip(new Main.BlipData(527, "Гражданская шахта", new Vector3(124.831116, -397.94046, 41.263977), 4, true));
                //Main.CreateBlip(new Main.BlipData(527, "Государственная шахта", new Vector3(-596, 2089, 131), 4, true));
               // Main.CreateBlip(new Main.BlipData(566, "Автомастерская самообслуживания", new Vector3(501.32233, -1336.5303, 29.32083), 4, true));
                //Main.CreateBlip(new Main.BlipData(648, "Металлургический завод", new Vector3(1100.942, -2008.321, 47.42521), 4, true));
               // Main.CreateBlip(new Main.BlipData(618, "Скупщик драгоценностей", new Vector3(1054.20, -1952.84, 32.09), 70, true));

                UpdateOres();

                using MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "SELECT * FROM mine_stocks"
                };

                using DataTable result = MySQL.QueryRead(cmd);
                if (result == null || result.Rows.Count == 0)
                {
                    Log.Write("Table 'mine_stocks' returns null result");
                }
                else
                {
                    foreach (DataRow Row in result.Rows)
                    {
                        if (Convert.ToInt32(Row["id"]) == 1)
                        {
                            MineStockOres[0] = Convert.ToInt32(Row["coal"]);
                            MineStockOres[1] = Convert.ToInt32(Row["iron"]);
                            MineStockOres[2] = Convert.ToInt32(Row["gold"]);
                            MineStockOres[3] = Convert.ToInt32(Row["sulfur"]);
                            MineStockOres[4] = Convert.ToInt32(Row["emerald"]);
                            MineStockOres[5] = Convert.ToInt32(Row["ruby"]);
                        }
                        else if (Convert.ToInt32(Row["id"]) == 2)
                        {
                            PlantStockOres[0] = Convert.ToInt32(Row["coal"]);
                            PlantStockOres[1] = Convert.ToInt32(Row["iron"]);
                            PlantStockOres[2] = Convert.ToInt32(Row["gold"]);
                            PlantStockOres[3] = Convert.ToInt32(Row["sulfur"]);
                            PlantStockOres[4] = Convert.ToInt32(Row["emerald"]);
                            PlantStockOres[5] = Convert.ToInt32(Row["ruby"]);
                        }
                    }
                }

                MineStockLabel = (ExtTextLabel) NAPI.TextLabel.CreateTextLabel(Main.StringToU16($"~y~Состояние хранилища:\n\nИскопаемого угля {MineStockOres[0]} ед.\nЖелезной руды {MineStockOres[1]} ед.\nСерной руды {MineStockOres[3]} ед.\nДрагоценных камней {MineStockOres[2] + MineStockOres[4] + MineStockOres[5]} ед."), new Vector3(-595.855, 2094.609, 131.4356), 5F, 0.5F, 0, new Color(255, 255, 255), true, 0);
                PlantStockLabel = (ExtTextLabel) NAPI.TextLabel.CreateTextLabel(Main.StringToU16($"~y~Состояние хранилища:\n\nИскопаемого угля {PlantStockOres[0]} ед.\nЖелезной руды {PlantStockOres[1]} ед.\nЗолотой руды {PlantStockOres[2]} ед.\nДрагоценных камней {PlantStockOres[4] + PlantStockOres[5]} ед."), new Vector3(1066.399, -1980.138, 31.01464), 5F, 0.5F, 0, new Color(255, 255, 255), true, 0);

                PedSystem.Repository.CreateQuest("s_m_y_construct_01", new Vector3(-1637.993, -800.12006, 10.246064), 57.71f, title: "~y~NPC~w~ Марк\nСкупщик руды", colShapeEnums: ColShapeEnums.OresSell);

                CustomColShape.CreateSphereColShape(new Vector3(-595.855, 2094.609, 131.4356), 5f, 0, ColShapeEnums.GovMineStock);
            }
            catch (Exception e)
            {
                Log.Write($"Event_ResourceStart Exception: {e.ToString()}");
            }
        }

        [Interaction(ColShapeEnums.OresSell)]
        public static void OpenDialog(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (sessionData.CuffedData.Cuffed)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsCuffed), 3000);
                    return;
                }
                else if (sessionData.DeathData.InDeath)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsDying), 3000);
                    return;
                }
                else if (Main.IHaveDemorgan(player, true)) return;

                player.SelectQuest(new PlayerQuestModel("npc_oressale", 0, 0, false, DateTime.Now));
                Trigger.ClientEvent(player, "client.quest.open", index, "npc_oressale", 0, 0, 0);
            }
            catch (Exception e)
            {
                Log.Write($"OpenDialog Exception: {e.ToString()}");
            }
        }
        public static void Perform(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Trigger.ClientEvent(player, "resourceSell_openMenu", 0, JsonConvert.SerializeObject(OresData));
            }
            catch (Exception e)
            {
                Log.Write($"Perform Exception: {e.ToString()}");
            }
        }

        private static List<ItemId> GetItems()
        {
            return new List<ItemId>()
            {
                ItemId.Coal,
                ItemId.Iron,
                ItemId.Gold,
                ItemId.Sulfur,
                ItemId.Emerald,
                ItemId.Ruby,
            };
        }
        [Interaction(ColShapeEnums.GovMineStock)]
        public static void GovMineStockLoad(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (!FunctionsAccess.IsWorking("GovMineStockLoad"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
		
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null || memberFractionData.Id != (int)Fractions.Models.Fractions.ARMY) return;

                if (!sessionData.WorkData.OnDuty)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PickaxeBroken), 3000);
                    return;
                }

                if (!player.IsInVehicle)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMustBeInVeh), 3000);
                    return;
                }

                var vehicle = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if (!vehicleLocalData.CanMats)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehNoResources), 3000);
                        return;
                    }

                    if (vehicleLocalData.Fraction != (int)Fractions.Models.Fractions.ARMY)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantGruzitVehResc), 3000);
                        return;
                    }

                    int count = Chars.Repository.getCountItem(VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), GetItems());
                    if (count >= 3200)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehMaxResc), 3000);
                        return;
                    }

                    if (MineStockOres[0] == 0 && MineStockOres[1] == 0 && MineStockOres[2] == 0 && MineStockOres[3] == 0 && MineStockOres[4] == 0 && MineStockOres[5] == 0)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseEmpty), 3000);
                        return;
                    }

                    string LoadInfo = $"";

                    for (int i = 0; i < 6; i++)
                    {
                        if (MineStockOres[i] >= 1)
                        {
                            ItemId item_type = ItemId.Coal;

                            if (i == 1) item_type = ItemId.Iron;
                            else if (i == 2) item_type = ItemId.Gold;
                            else if (i == 3) item_type = ItemId.Sulfur;
                            else if (i == 4) item_type = ItemId.Emerald;
                            else if (i == 5) item_type = ItemId.Ruby;

                            if ((MineStockOres[i] + count) < 3200)
                            {
                                Chars.Repository.AddNewItem(null, VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), "vehicle", item_type, MineStockOres[i]);

                                if (item_type == ItemId.Coal) LoadInfo += LangFunc.GetText(LangType.Ru, DataName.Puglya, MineStockOres[i]);
                                else if (item_type == ItemId.Iron) LoadInfo += LangFunc.GetText(LangType.Ru, DataName.Piron, MineStockOres[i]);
                                else if (item_type == ItemId.Sulfur) LoadInfo += LangFunc.GetText(LangType.Ru, DataName.Psera, MineStockOres[i]);
                                else if (item_type == ItemId.Emerald) LoadInfo += LangFunc.GetText(LangType.Ru, DataName.Pizumrud, MineStockOres[i]);

                                MineStockOres[i] = 0;
                            }
                            else
                            {
                                Chars.Repository.AddNewItem(null, VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), "vehicle", item_type, (3200 - count));

                                if (item_type == ItemId.Coal) LoadInfo += LangFunc.GetText(LangType.Ru, DataName.Puglya, 3200 - count);
                                else if (item_type == ItemId.Iron) LoadInfo += LangFunc.GetText(LangType.Ru, DataName.Piron, 3200 - count);
                                else if (item_type == ItemId.Sulfur) LoadInfo += LangFunc.GetText(LangType.Ru, DataName.Psera, 3200 - count);
                                else if (item_type == ItemId.Emerald) LoadInfo += LangFunc.GetText(LangType.Ru, DataName.Pizumrud, 3200 - count);

                                MineStockOres[i] -= (3200 - count);
                            }
                        }
                    }

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucLoadResToVeh), 3000);
                    SaveMineStocks(1);

                    LoadInfo = LoadInfo.Remove(LoadInfo.Length - 2);
                    Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeOre, LangFunc.GetText(LangType.Ru, DataName.LoadedFromWarehouse, LoadInfo));
                }
            }
            catch (Exception e)
            {
                Log.Write($"GovMineStockLoad Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("PlayerEnteredMineShape")]
        public static void PlayerEnteredMineShape(ExtPlayer player, int zone)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null || memberFractionData.Id != (int)Fractions.Models.Fractions.ARMY) 
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OnlyGosMine), 3000);
                    return;
                }

                if (!sessionData.WorkData.OnDuty)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000);
                    return;
                }

                Trigger.ClientEvent(player, "confirmPlayerEnteredMineShape", zone);
            }
            catch (Exception e)
            {
                Log.Write($"PlayerEnteredMineShape Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("PlayerStartedMining")]
        public static void RemoteEvent_PlayerStartedMining(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;

                if (!FunctionsAccess.IsWorking("PlayerStartedMining"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                if (sessionData.AnimationUse != null || Main.IHaveDemorgan(player, true)) return;
                else if (sessionData.CuffedData.Cuffed)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsCuffed), 3000);
                    return;
                }
                else if (sessionData.DeathData.InDeath)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsDying), 3000);
                    return;
                }

                if (MinerJobPlayersOreTypeInfo.ContainsKey(player.Value))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы переносите груз.", 3000);
                    return;
                }
                
                /*if (DateTime.Now.Hour > 10 && DateTime.Now.Hour != 22 && DateTime.Now.Hour != 23)
                { // todo translate
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OgranVremyaMiner), 3000);
                    return;
                }*/

                if (!sessionData.CuffedData.Cuffed && !sessionData.DeathData.InDeath && !sessionData.AntiAnimDown)
                {
                    /*if (Repository.isItem(player, "inventory", ItemId.Pickaxe3) != null) Trigger.ClientEvent(player, "mineJob_updateToolInfo", 3);
                    else if (Repository.isItem(player, "inventory", ItemId.Pickaxe2) != null) Trigger.ClientEvent(player, "mineJob_updateToolInfo", 2);
                    else if (Repository.isItem(player, "inventory", ItemId.Pickaxe1) != null) Trigger.ClientEvent(player, "mineJob_updateToolInfo", 1);
                    else
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoPickaxe), 3000);
                        return;
                    }*/
                    Trigger.StopAnimation(player);

                    Attachments.AddAttachment(player, Attachments.AttachmentsName.Pickaxe);
                    Trigger.PlayAnimation(player, "melee@large_wpn@streamed_core", "car_side_attack_a", 1);
                    // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "miner");

                    Trigger.ClientEvent(player, "mineJob_startMining");
                }
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantStartJobNow), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"PlayerStartedMining Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("PlayerFinishedMining")]
        public static void RemoteEvent_PlayerFinishedMining(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Attachments.RemoveAttachment(player, Attachments.AttachmentsName.Pickaxe);
                Trigger.StopAnimation(player);
                Main.OffAntiAnim(player);
            }
            catch (Exception e)
            {
                Log.Write($"PlayerFinishedMining Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("PlayerStopCarryOre")]
        public static void PlayerStopCarryOre(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (MinerJobPlayersOreTypeInfo.ContainsKey(player.Value)) MinerJobPlayersOreTypeInfo.Remove(player.Value);
                Trigger.StopAnimation(player);
                Main.OffAntiAnim(player);
                Attachments.RemoveAttachment(player, Attachments.AttachmentsName.MineRock);
                Trigger.ClientEvent(player, "mineJob_updateOreCarryStatus", false);
                Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "mineJob");
            }
            catch (Exception e)
            {
                Log.Write($"PlayerStopCarryOre Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("PlayerMinedGovResources")]
        public static void RemoteEvent_PlayerMinedGovResources(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                Trigger.StopAnimation(player);
                Attachments.RemoveAttachment(player, Attachments.AttachmentsName.Pickaxe);

                ItemStruct MainPickaxe = Chars.Repository.isItem(player, "inventory", ItemId.Pickaxe3);

                if (MainPickaxe == null) MainPickaxe = Chars.Repository.isItem(player, "inventory", ItemId.Pickaxe2);
                if (MainPickaxe == null) MainPickaxe = Chars.Repository.isItem(player, "inventory", ItemId.Pickaxe1);
                if (MainPickaxe == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoPickaxe), 3000);
                    return;
                }

                if (MainPickaxe.Item.Data == "" || MainPickaxe.Item.Data.Length < 1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PickaxeBroken), 3000);
                    Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", MainPickaxe.Item.ItemId, 1);
                }
                else if (MainPickaxe.Item.ItemId == ItemId.Pickaxe1 && (Convert.ToInt32(MainPickaxe.Item.Data) - 1) <= 0 || MainPickaxe.Item.ItemId == ItemId.Pickaxe2 && (Convert.ToInt32(MainPickaxe.Item.Data) - 1) <= 0 || MainPickaxe.Item.ItemId == ItemId.Pickaxe3 && (Convert.ToInt32(MainPickaxe.Item.Data) - 1) <= 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PickaxeBroken), 3000);
                    Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", MainPickaxe.Item.ItemId, 1);
                }
                else
                {
                    if (MainPickaxe.Item.ItemId == ItemId.Pickaxe1) MainPickaxe.Item.Data = $"{Convert.ToInt32(MainPickaxe.Item.Data) - 1}";
                    else if (MainPickaxe.Item.ItemId == ItemId.Pickaxe2) MainPickaxe.Item.Data = $"{Convert.ToInt32(MainPickaxe.Item.Data) - 1}";
                    else if (MainPickaxe.Item.ItemId == ItemId.Pickaxe3) MainPickaxe.Item.Data = $"{Convert.ToInt32(MainPickaxe.Item.Data) - 1}";

                    Chars.Repository.SetItemData(player, MainPickaxe.Location, MainPickaxe.Index, MainPickaxe.Item, true);
                }

                Attachments.AddAttachment(player, Attachments.AttachmentsName.MineRock);

                int rand = new Random().Next(1, 101);

                if (rand > 17 && rand <= 32) // ore_iron (Железная руда)
                {
                    if (!MinerJobPlayersOreTypeInfo.ContainsKey(player.Value)) MinerJobPlayersOreTypeInfo.Add(player.Value, 1);
                    else MinerJobPlayersOreTypeInfo[player.Value] = 1;
                }
                else if (rand > 1 && rand <= 17) // ore_sulfur (Серная руда)
                {
                    if (!MinerJobPlayersOreTypeInfo.ContainsKey(player.Value)) MinerJobPlayersOreTypeInfo.Add(player.Value, 3);
                    else MinerJobPlayersOreTypeInfo[player.Value] = 3;
                }
                else if (rand == 1) // ore_emerald (Изумруд)
                {
                    if (!MinerJobPlayersOreTypeInfo.ContainsKey(player.Value)) MinerJobPlayersOreTypeInfo.Add(player.Value, 4);
                    else MinerJobPlayersOreTypeInfo[player.Value] = 4;
                }
                else // ore_coal (Ископаемый уголь)
                {
                    if (!MinerJobPlayersOreTypeInfo.ContainsKey(player.Value)) MinerJobPlayersOreTypeInfo.Add(player.Value, 0);
                    else MinerJobPlayersOreTypeInfo[player.Value] = 0;
                }

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucResDobil), 3000);
                player.AddTableScore(TableTaskId.Item19);
                Trigger.ClientEvent(player, "mineJob_updateOreCarryStatus", true);

                Main.OnAntiAnim(player);
                Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, true, "mineJob");
            }
            catch (Exception e)
            {
                Log.Write($"PlayerMinedGovResources Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("PlayerMinedResources")]
        public static void RemoteEvent_PlayerMinedResources(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                //Trigger.StopAnimation(player);
                //Main.OffAntiAnim(player);
                //Attachments.RemoveAttachment(player, Attachments.AttachmentsName.Pickaxe);

                var mainPickaxe = Chars.Repository.isItem(player, "inventory", ItemId.Pickaxe3);

                if (mainPickaxe == null) mainPickaxe = Chars.Repository.isItem(player, "inventory", ItemId.Pickaxe2);
                if (mainPickaxe == null) mainPickaxe = Chars.Repository.isItem(player, "inventory", ItemId.Pickaxe1);
                if (mainPickaxe == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoPickaxe), 3000);
                    return;
                }
                
                /*if (DateTime.Now.Hour > 10 && DateTime.Now.Hour != 22 && DateTime.Now.Hour != 23)
                { // todo translate
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OgranVremyaMiner), 3000);
                    Trigger.ClientEvent(player, "mineJob_stopMining");
                    return;
                }*/

                if (mainPickaxe.Item.Data == "" || mainPickaxe.Item.Data.Length < 1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PickaxeBroken), 3000);
                    Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", mainPickaxe.Item.ItemId, 1);
                    Trigger.ClientEvent(player, "mineJob_stopMining");
                }
                else if (mainPickaxe.Item.ItemId == ItemId.Pickaxe1 && (Convert.ToInt32(mainPickaxe.Item.Data) - 1) <= 0 || mainPickaxe.Item.ItemId == ItemId.Pickaxe2 && (Convert.ToInt32(mainPickaxe.Item.Data) - 1) <= 0 || mainPickaxe.Item.ItemId == ItemId.Pickaxe3 && (Convert.ToInt32(mainPickaxe.Item.Data) - 1) <= 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PickaxeBroken), 3000);
                    Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", mainPickaxe.Item.ItemId, 1);
                    Trigger.ClientEvent(player, "mineJob_stopMining");
                }
                else
                {
                    if (mainPickaxe.Item.ItemId == ItemId.Pickaxe1) mainPickaxe.Item.Data = $"{Convert.ToInt32(mainPickaxe.Item.Data) - 1}";
                    else if (mainPickaxe.Item.ItemId == ItemId.Pickaxe2) mainPickaxe.Item.Data = $"{Convert.ToInt32(mainPickaxe.Item.Data) - 1}";
                    else if (mainPickaxe.Item.ItemId == ItemId.Pickaxe3) mainPickaxe.Item.Data = $"{Convert.ToInt32(mainPickaxe.Item.Data) - 1}";

                    Chars.Repository.SetItemData(player, mainPickaxe.Location, mainPickaxe.Index, mainPickaxe.Item, true);
                }

                int rand = new Random().Next(1, 101);

                //string item_name = "ископаемый уголь";
                var item_type = ItemId.Coal; // ore_coal (Ископаемый уголь)

                if (rand > 9 && rand <= 23) // ore_iron (Железная руда)
                {
                    //item_name = "железную руду";
                    item_type = ItemId.Iron;
                }
                else if (rand > 2 && rand <= 9) // ore_gold (Золотая руда)
                {
                    //item_name = "золотую руду";
                    item_type = ItemId.Gold;
                }
                else if (rand == 2) // ore_emerald (Изумруд)
                {
                    //item_name = "изумруд";
                    item_type = ItemId.Emerald;
                }
                else if (rand == 1) // ore_ruby (Рубин)
                {
                    //item_name = "рубин";
                    item_type = ItemId.Ruby;
                }

                if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", item_type, 1) == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"PlayerMinedResources Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("PlayerPutGovResources")]
        public static void RemoteEvent_PlayerPutGovResources(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                Trigger.StopAnimation(player);
                Main.OffAntiAnim(player);
                Attachments.RemoveAttachment(player, Attachments.AttachmentsName.MineRock);

                if (MinerJobPlayersOreTypeInfo.ContainsKey(player.Value))
                {
                    MineStockOres[MinerJobPlayersOreTypeInfo[player.Value]] += 1;
                    SaveMineStocks(1);
                    MinerJobPlayersOreTypeInfo.Remove(player.Value);
                }

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SklafFulled), 3000);
                Trigger.ClientEvent(player, "mineJob_updateOreCarryStatus", false);

                Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "mineJob");
            }
            catch (Exception e)
            {
                Log.Write($"PlayerPutGovResources Exception: {e.ToString()}");
            }
        }

        [ServerEvent(Event.PlayerDeath)]
        public void onPlayerDeathHandler(ExtPlayer player, ExtPlayer entityKiller, uint weapon)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                
                if (MinerJobPlayersOreTypeInfo.ContainsKey(player.Value)) MinerJobPlayersOreTypeInfo.Remove(player.Value);
                
                if (Attachments.HasAttachment(player, Attachments.AttachmentsName.Pickaxe))
                {
                    Trigger.StopAnimation(player);
                    Attachments.RemoveAttachment(player, Attachments.AttachmentsName.Pickaxe);
                    Trigger.ClientEvent(player, "mineJob_stopMining");
                    Main.OffAntiAnim(player);
                }
                else if (Attachments.HasAttachment(player, Attachments.AttachmentsName.MineRock))
                {
                    Trigger.StopAnimation(player);
                    Attachments.RemoveAttachment(player, Attachments.AttachmentsName.MineRock);
                    Trigger.ClientEvent(player, "mineJob_updateOreCarryStatus", false);
                    Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "mineJob");
                    Main.OffAntiAnim(player);
                }
            }
            catch (Exception e)
            {
                Log.Write($"onPlayerDeathHandler Exception: {e.ToString()}");
            }
        }

        public static void OnPlayerDisconnected(ExtPlayer player)
        {
            try
            {
                if (MinerJobPlayersOreTypeInfo.ContainsKey(player.Value)) 
                    MinerJobPlayersOreTypeInfo.Remove(player.Value);
            }
            catch (Exception e)
            {
                Log.Write($"OnPlayerDisconnected Exception: {e.ToString()}");
            }
        }
        class OreData
        {
            public int Price { get; set; }
            public ItemId ItemId { get; set; }

            public OreData(int Price, ItemId ItemId)
            {
                this.Price = Price;
                this.ItemId = ItemId;
            }
        }
        private static Dictionary<int, OreData> OresData = new Dictionary<int, OreData>();

        public static void UpdateOres()
        {
            using (var db = new ConfigBD("ConfigDB"))
            {
                Dictionary<int, OreData> _OresData = new Dictionary<int, OreData>();
                List<Ores> OresList = db.Ores.ToList();
                foreach(Ores ore in OresList)
                {
                    _OresData.Add(ore.Index, new OreData(ore.Price, (ItemId)ore.ItemId));
                }
                OresData = _OresData;
            }
        }
        [RemoteEvent("PlayerSellOres")]
        public static void RemoteEvent_PlayerSellOres(ExtPlayer player, int stock_item_index, int amount)
        {
            try
            {
                if (!FunctionsAccess.IsWorking("PlayerSellOres"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                else if (!OresData.ContainsKey(stock_item_index)) return;

                ItemId item_type = OresData [stock_item_index].ItemId;
                int price = OresData[stock_item_index].Price;

                ItemStruct oreamount = Chars.Repository.isItem(player, "inventory", item_type);
                int count = (oreamount == null) ? 0 : oreamount.Item.Count;

                if (count < amount) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NotEnoughRes), 3000);
                else
                {
                    Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", item_type, amount);

                    int totalmoney = Convert.ToInt32(price * amount);
                    Wallet.Change(player, totalmoney);
                    GameLog.Money($"server", $"player({characterData.UUID})", totalmoney, $"sellOre({stock_item_index},{amount})");

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SellResources, Wallet.Format(totalmoney)), 3000);
                    //BattlePass.Repository.UpdateReward(player, 58);

                    PlantStockOres[stock_item_index] += amount;
                    SaveMineStocks(2);

                    Perform(player);
                    
                    if (qMain.GetQuestsLine(player, Zdobich.QuestName) == (int)zdobich_quests.Stage11)
                    {
                        sessionData.WorkData.PointsCount += totalmoney;
                        if (sessionData.WorkData.PointsCount < qMain.GetQuestsData(player, Zdobich.QuestName, (int) zdobich_quests.Stage11))
                            sessionData.WorkData.PointsCount = qMain.GetQuestsData(player, Zdobich.QuestName, (int) zdobich_quests.Stage11) + totalmoney;
                    
                        if (sessionData.WorkData.PointsCount >= 500)
                        {
                            qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage11, 1, isUpdateHud: true);
                            qMain.UpdateQuestsComplete(player, Zdobich.QuestName, (int) zdobich_quests.Stage11, true);
                            Trigger.SendChatMessage(player, "!{#fc0}" + LangFunc.GetText(LangType.Ru, DataName.QuestPartComplete));
                        }
                        else
                        {
                            qMain.UpdateQuestsData(player, Zdobich.QuestName, (int)zdobich_quests.Stage11, sessionData.WorkData.PointsCount.ToString());
                            //todo translate (было DataName.PointsQuestGot)
                            Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.YouEarnedJob, sessionData.WorkData.PointsCount, 500 - sessionData.WorkData.PointsCount));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"PlayerSellOres Exception: {e.ToString()}");
            }
        }

        public static void SaveMineStocks(int mine_index)
        {
            try
            {
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.MineStocks
                            .Where(v => v.Id == mine_index)
                            .Set(v => v.Coal, mine_index == 1 ? MineStockOres[0] : PlantStockOres[0])
                            .Set(v => v.Iron, mine_index == 1 ? MineStockOres[1] : PlantStockOres[1])
                            .Set(v => v.Gold, mine_index == 1 ? MineStockOres[2] : PlantStockOres[2])
                            .Set(v => v.Sulfur, mine_index == 1 ? MineStockOres[3] : PlantStockOres[3])
                            .Set(v => v.Emerald, mine_index == 1 ? MineStockOres[4] : PlantStockOres[4])
                            .Set(v => v.Ruby, mine_index == 1 ? MineStockOres[5] : PlantStockOres[5])
                            .UpdateAsync();
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });

                if (mine_index == 1 && MineStockLabel != null)
                {
                    MineStockLabel.Text = Main.StringToU16($"~y~Состояние хранилища:\n\nИскопаемого угля {MineStockOres[0]} ед.\nЖелезной руды {MineStockOres[1]} ед.\nСерной руды {MineStockOres[3]} ед.\nДрагоценных камней {MineStockOres[2] + MineStockOres[4] + MineStockOres[5]} ед.");
                }
                else if (mine_index == 2 && PlantStockLabel != null)
                {
                    PlantStockLabel.Text = Main.StringToU16($"~y~Состояние хранилища:\n\nИскопаемого угля {PlantStockOres[0]} ед.\nЖелезной руды {PlantStockOres[1]} ед.\nЗолотой руды {PlantStockOres[2]} ед.\nДрагоценных камней {PlantStockOres[4] + PlantStockOres[5]} ед.");
                }
            }
            catch (Exception e)
            {
                Log.Write($"SaveMineStocks Exception: {e.ToString()}");
            }
        }
    }
}