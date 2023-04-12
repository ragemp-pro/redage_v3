using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions;
using NeptuneEvo.Functions;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.Players;
using NeptuneEvo.Quests;
using NeptuneEvo.Quests.Models;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using Localization;

namespace NeptuneEvo.Jobs
{
    class TreesData
    {
        public int TreeIndex = 0;
        public Vector3 TreePosition;
        
        public ExtColShape TreeShape;
        public GTANetworkAPI.Object TreeObject;
        public byte TreeStage = 0;
        public short TreeHP = 100;
        public string RebirthTimer = null;
        public ExtPlayer StartedTreeHitPlayerInfo { set; get; } = null;
        public ExtPlayer LastTreeHitPlayer { set; get; } = null;
        public DateTime LastTreeHitTime { set; get; } = DateTime.MinValue;

        public TreesData(int index, Vector3 TreePosition)
        {
            this.TreeIndex = index;
            this.TreePosition = TreePosition;
        }
    }

    class Lumberjack : Script
    {
        private static readonly nLog Log = new nLog("Jobs.Lumberjack");

        private static Dictionary<int, TreesData> TreesData = new Dictionary<int, TreesData>();

        private static List<(string, Vector3)> TreesPositions = new List<(string, Vector3)>()
        {
           
            ("prop_snow_tree_04_f", new Vector3(-481.63535, -1160.133, 21.63349)), 
            ("prop_snow_tree_04_f", new Vector3(-464.7431, -1172.6416, 22.41592)), 
            ("prop_snow_tree_04_f", new Vector3(-468.1925, -1151.3473, 23.663685)), 
            ("prop_snow_tree_04_f", new Vector3(-476.75443, -1127.6227, 23.67057)), 
            ("prop_snow_tree_04_f", new Vector3(-490.60272, -1124.4965, 23.138412)), 
            ("prop_snow_tree_04_f", new Vector3(-458.86343, -1188.6072, 21.689891)),
            ("prop_snow_tree_04_f", new Vector3(-447.7554, -1182.7928, 22.635908)), 
            ("prop_snow_tree_04_f", new Vector3(-442.88788, -1196.6096, 22.664852)), 
            ("prop_snow_tree_04_f", new Vector3(-445.90454, -1151.6956, 23.298677)),  
            ("prop_snow_tree_04_f", new Vector3(-486.94104, -1143.0538, 23.219059)),
            ("prop_snow_tree_04_f", new Vector3(-474.45114, -1135.6971, 25.190186)),
            ("prop_snow_tree_04_f", new Vector3(-456.11792, -1151.8817, 24.563269)),
            ("prop_snow_tree_04_f", new Vector3(-458.828, -1175.5568, 22.576423)),
            ("prop_snow_tree_04_f", new Vector3(-450.9154, -1175.1796, 22.51234)),
            ("prop_snow_tree_04_f", new Vector3(-446.41922, -1166.6053, 23.078728)),
            ("prop_snow_tree_04_f", new Vector3(-468.37744, -1124.925, 26.565672)),
            ("prop_snow_tree_04_f", new Vector3(-484.61398, -1153.391, 21.5925)),
            ("prop_snow_tree_04_f", new Vector3(-460.06134, -1199.1066, 21.20713)),
            ("prop_snow_tree_04_f", new Vector3(-472.6164, -1182.0721, 20.361935)),
            ("prop_snow_tree_04_f", new Vector3(-448.71597, -1136.526, 25.326338))
        };

        private static List<int> TreeStock = new List<int>()
        {
            0, // Дуб
            0, // Клен
            0  // Сосна
        };

        public static ExtTextLabel TreeStockLabel { get; set; } = null;

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStartHandler()
        {
            try
            {
                for (int i = 0; i < TreesPositions.Count; i++)
                {
                    TreesData.Add(i, new TreesData(i, new Vector3(TreesPositions[i].Item2.X, TreesPositions[i].Item2.Y, TreesPositions[i].Item2.Z + 1.25)));
                    TreesData[i].TreeShape = CustomColShape.CreateSphereColShape(new Vector3(TreesPositions[i].Item2.X, TreesPositions[i].Item2.Y, TreesPositions[i].Item2.Z + 1.25), 1.75f, 0, ColShapeEnums.LumberjackTree, i);
                    TreesData[i].TreeObject = NAPI.Object.CreateObject(NAPI.Util.GetHashKey(TreesPositions[i].Item1), new Vector3(TreesPositions[i].Item2.X, TreesPositions[i].Item2.Y, TreesPositions[i].Item2.Z - 0.45), new Vector3(0, 0, 0), 255, 0);
                }

                Main.CreateBlip(new Main.BlipData(468, LangFunc.GetText(LangType.Ru, DataName.LesniyeResi), new Vector3(-490.60272, -1124.4965, 25.138412), 2, true));
               /*  Main.CreateBlip(new Main.BlipData(468, LangFunc.GetText(LangType.Ru, DataName.LesniyeResi), new Vector3(3370.0408, 4945.8154, 33.202995), 2, true));
                Main.CreateBlip(new Main.BlipData(468, LangFunc.GetText(LangType.Ru, DataName.LesniyeResi), new Vector3(-1319.7822, 4444.8164, 23.27308), 2, true));
                Main.CreateBlip(new Main.BlipData(468, LangFunc.GetText(LangType.Ru, DataName.LesniyeResi), new Vector3(-1988.7891, 2584.667, 3.311179), 2, true));
                Main.CreateBlip(new Main.BlipData(468, LangFunc.GetText(LangType.Ru, DataName.LesniyeResi), new Vector3(160.4275, 6895.9033, 20.979313), 2, true)); */

                //Main.CreateBlip(new Main.BlipData(569, LangFunc.GetText(LangType.Ru, DataName.SkladLesa), new Vector3(-540.13434, 5380.038, 70.48429), 4, true));
/*                 Main.CreateBlip(new Main.BlipData(480, LangFunc.GetText(LangType.Ru, DataName.SkladDereva), new Vector3(-575.54004, 5350.516, 70.214424), 70, true));
 */
                Main.CreateBlip(new Main.BlipData(119, LangFunc.GetText(LangType.Ru, DataName.OhotnMagaz), new Vector3(-758.8592, -618.1002, 30.2762), 21, true));

                TreeStockLabel = (ExtTextLabel) NAPI.TextLabel.CreateTextLabel(Main.StringToU16(LangFunc.GetText(LangType.Ru, DataName.Hranilishe, TreeStock[0], TreeStock[1], TreeStock[2])), new Vector3(-540.13434, 5380.038, 70.48429), 5F, 0.5F, 0, new Color(255, 255, 255), true, 0);

                PedSystem.Repository.CreateQuest("s_m_y_construct_01", new Vector3(-758.8592, -618.1002, 30.2762), -90.1f, title: LangFunc.GetText(LangType.Ru, DataName.BearGrylls), colShapeEnums: ColShapeEnums.HuntingShop);
                PedSystem.Repository.CreateQuest("s_m_y_construct_01", new Vector3(-1640.2056, -802.55475, 10.231294), 58.71f, title: LangFunc.GetText(LangType.Ru, DataName.DmitrySkupshik), colShapeEnums: ColShapeEnums.TreesSell);
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStartHandler Exception: {e.ToString()}");
            }
        }
        [Interaction(ColShapeEnums.HuntingShop, In: true)]
        public void OpenDialogIn(ExtPlayer player, int index)
        {
            BattlePass.Repository.UpdateReward(player, 38);
        }
        [Interaction(ColShapeEnums.HuntingShop)]
        public void OpenDialog(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

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

                player.SelectQuest(new PlayerQuestModel("npc_huntingshop", 0, 0, false, DateTime.Now));
                Trigger.ClientEvent(player, "client.quest.open", index, "npc_huntingshop", 0, 0, 0);
            }
            catch (Exception e)
            {
                Log.Write($"OpenDialog Exception: {e.ToString()}");
            }
        }

        [Interaction(ColShapeEnums.TreesSell)]
        public void OpenTreesSellDialog(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

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

                player.SelectQuest(new PlayerQuestModel("npc_treessell", 0, 0, false, DateTime.Now));
                Trigger.ClientEvent(player, "client.quest.open", index, "npc_treessell", 0, 0, 0);
            }
            catch (Exception e)
            {
                Log.Write($"OpenDialog Exception: {e.ToString()}");
            }
        }

        public static void Perform(ExtPlayer player, byte index)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                if (index == 1)
                {
                    List<Manager.FracMatsData> jsonData = new List<Manager.FracMatsData>();

                    if (Manager.FractionDataMats.ContainsKey(74)) jsonData.Add(Manager.FractionDataMats[74]);
                    if (Manager.FractionDataMats.ContainsKey(75)) jsonData.Add(Manager.FractionDataMats[75]);
                    if (Manager.FractionDataMats.ContainsKey(76)) jsonData.Add(Manager.FractionDataMats[76]);
                    if (Manager.FractionDataMats.ContainsKey(77)) jsonData.Add(Manager.FractionDataMats[77]);

                    Trigger.ClientEvent(player, "client.sm.openHuntingShop", JsonConvert.SerializeObject(jsonData));
                }
                else if (index == 2)
                {
                    Trigger.ClientEvent(player, "resourceSell_openMenu", 1);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Perform Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.sm.huntingShop")]
        public static void callback_HuntingShop(ExtPlayer player, int index)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                switch (index)
                {
                    case 74:
                        if (UpdateData.CanIChange(player, Main.PricesSettings.InstrumentPrices[0], true) != 255) return;
                        else if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.WorkAxe, 1, "1250") == -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                        Wallet.Change(player, -Main.PricesSettings.InstrumentPrices[0]);
                        GameLog.Money($"player({characterData.UUID})", $"server", Main.PricesSettings.InstrumentPrices[0], $"buyHuntingShop(74)");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BoughtAxe), 3000);
                        BattlePass.Repository.UpdateReward(player, 57);
                        return;
                    case 75:
                        if (UpdateData.CanIChange(player, Main.PricesSettings.InstrumentPrices[1], true) != 255) return;
                        else if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Pickaxe1, 1, "300") == -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                        Wallet.Change(player, -Main.PricesSettings.InstrumentPrices[1]);
                        GameLog.Money($"player({characterData.UUID})", $"server", Main.PricesSettings.InstrumentPrices[1], $"buyHuntingShop(75)");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BoughtPickaxe1), 3000);
                        BattlePass.Repository.UpdateReward(player, 57);
                        return;
                    case 76:
                        if (UpdateData.CanIChange(player, Main.PricesSettings.InstrumentPrices[2], true) != 255) return;
                        else if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Pickaxe2, 1, "1248") == -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                        Wallet.Change(player, -Main.PricesSettings.InstrumentPrices[2]);
                        GameLog.Money($"player({characterData.UUID})", $"server", Main.PricesSettings.InstrumentPrices[2], $"buyHuntingShop(76)");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BoughtPickaxe2), 3000);
                        BattlePass.Repository.UpdateReward(player, 57);
                        return;
                    case 77:
                        if (UpdateData.CanIChange(player, Main.PricesSettings.InstrumentPrices[3], true) != 255) return;
                        else if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Pickaxe3, 1, "2250") == -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                        Wallet.Change(player, -Main.PricesSettings.InstrumentPrices[3]);
                        GameLog.Money($"player({characterData.UUID})", $"server", Main.PricesSettings.InstrumentPrices[3], $"buyHuntingShop(77)");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BoughtPickaxe3), 3000);
                        BattlePass.Repository.UpdateReward(player, 57);
                        return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"callback_HuntingShop Exception: {e.ToString()}");
            }
        }

        [Interaction(ColShapeEnums.LumberjackTree, In: true)]
        public static void OnEnterTreeShape(ExtPlayer player, int TreeIndex)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (TreesData.ContainsKey(TreeIndex))
                {
                    var treesData = TreesData[TreeIndex];
                    if (treesData.TreeStage > 1) return;
                    Trigger.ClientEvent(player, "lumberjackJob_setTreeInfo", TreeIndex, treesData.TreeStage, treesData.TreePosition, treesData.TreeHP);
                }
            }
            catch (Exception e)
            {
                Log.Write($"OnEnterTreeShape Exception: {e.ToString()}");
            }
        }

        [Interaction(ColShapeEnums.LumberjackTree, Out: true)]
        public static void OnExitTreeShape(ExtPlayer player, int TreeIndex)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Trigger.ClientEvent(player, "lumberjackJob_clearInfo");
            }
            catch (Exception e)
            {
                Log.Write($"OnExitTreeShape Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("Lumberjack_HitTree")]
        public static void Lumberjack_HitTree(ExtPlayer player, int TreeIndex)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (!FunctionsAccess.IsWorking("Lumberjack_HitTree"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }

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
                else if (sessionData.AnimationUse != null || Main.IHaveDemorgan(player, true)) return;
                
                /*if (DateTime.Now.Hour > 9 && DateTime.Now.Hour != 22 && DateTime.Now.Hour != 23)
                { // todo translate
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OgranVremyaMiner), 3000);
                    return;
                }*/

                if (TreesData.ContainsKey(TreeIndex))
                {
                    var treesData = TreesData[TreeIndex];
                    if (treesData.TreeStage != 0) return;

                    ItemStruct WorkAxe = Chars.Repository.isItem(player, "inventory", ItemId.WorkAxe);
                    if (WorkAxe == null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoAxe), 3000);
                        return;
                    }

                    if (treesData.StartedTreeHitPlayerInfo != null)
                    {
                        if (!treesData.StartedTreeHitPlayerInfo.IsCharacterData()) treesData.StartedTreeHitPlayerInfo = null;
                        else if (treesData.StartedTreeHitPlayerInfo.Position.DistanceTo(treesData.TreePosition) >= 3) treesData.StartedTreeHitPlayerInfo = null;
                        else if (treesData.StartedTreeHitPlayerInfo == player) return;
                        else
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomebodyCutting), 3000);
                            return;
                        }
                    }

                    if (treesData.LastTreeHitPlayer != null && treesData.LastTreeHitPlayer != player)
                    {
                        if (!treesData.LastTreeHitPlayer.IsCharacterData()) treesData.LastTreeHitPlayer = null;
                        else if (treesData.LastTreeHitPlayer.Position.DistanceTo(treesData.TreePosition) >= 3) treesData.LastTreeHitPlayer = null;
                        else if (treesData.LastTreeHitTime > DateTime.Now)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomebodyCutting), 3000);
                            return;
                        }
                    }

                    //Main.OnAntiAnim(player);
                    //Trigger.PlayAnimation(player, "melee@large_wpn@streamed_core", "car_side_attack_a", 1);
                    //Attachments.AddAttachment(player, Attachments.AttachmentsName.WorkAxeProp);
                    Lumberjack_StartAnimCutTree(player);

                    if (WorkAxe.Item.Data == "" || WorkAxe.Item.Data.Length < 1)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AxeBroken), 3000);
                        Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", WorkAxe.Item.ItemId, 1);
                    }
                    else if ((Convert.ToInt32(WorkAxe.Item.Data) - 6) <= 0)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AxeBroken), 3000);
                        Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", WorkAxe.Item.ItemId, 1);
                    }
                    else
                    {
                        WorkAxe.Item.Data = $"{Convert.ToInt32(WorkAxe.Item.Data) - 6}";
                        Chars.Repository.SetItemData(player, WorkAxe.Location, WorkAxe.Index, WorkAxe.Item, true);
                    }

                    treesData.StartedTreeHitPlayerInfo = player;
                    treesData.LastTreeHitPlayer = player;
                    treesData.LastTreeHitTime = DateTime.Now.AddSeconds(30);

                    Trigger.ClientEvent(player, "lumberjackJob_startProcess");
                }
            }
            catch (Exception e)
            {
                Log.Write($"Lumberjack_HitTree Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("Lumberjack_CutTree")]
        public static void Lumberjack_CutTree(ExtPlayer player, int TreeIndex)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (!FunctionsAccess.IsWorking("Lumberjack_CutTree"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }

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

                Main.OffAntiAnim(player);
                Trigger.StopAnimation(player);
                Attachments.RemoveAttachment(player, Attachments.AttachmentsName.WorkAxeProp);

                if (TreesData.ContainsKey(TreeIndex))
                {
                    var treesData = TreesData[TreeIndex];
                    if (treesData.TreeStage != 0) return;

                    treesData.TreeHP = 0;
                    treesData.TreeStage = 1;
                    treesData.LastTreeHitPlayer = null;

                    if (treesData.TreeObject != null && treesData.TreeObject.Exists)  
                        treesData.TreeObject.Delete(); 
                    
                    treesData.TreeObject = NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_tree_stump_01"), new Vector3(TreesPositions[TreeIndex].Item2.X, TreesPositions[TreeIndex].Item2.Y, TreesPositions[TreeIndex].Item2.Z - 0.3), new Vector3(0, 0, 0), 255, 0);

                    var item_chance = new Random().Next(1, 100);
                    var item_type = ItemId.WoodOak;

                    if (item_chance <= 33) 
                        item_type = ItemId.WoodOak;
                    else if (item_chance <= 66)
                        item_type = ItemId.WoodMaple;
                    else 
                        item_type = ItemId.WoodPine;

                    if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", item_type, 1) == -1)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMoreLogs), 3000);
                    }
                    else
                    {
                        treesData.TreeStage = 2;
                    }

                    NAPI.ClientEvent.TriggerClientEventInRange(treesData.TreePosition, 2.5f, "lumberjackJob_updateTreeInfo", TreeIndex, treesData.TreeStage, treesData.TreeHP);

                    if (treesData.RebirthTimer != null)
                    {
                        Timers.Stop(treesData.RebirthTimer);
                    }

                    treesData.RebirthTimer = Timers.StartOnce($"TreeRebirthTimer{TreeIndex}", 15000, () => TreeRebirthTimerFunction(TreeIndex), true);
                    
                    //BattlePass.Repository.UpdateReward(player, 54);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Lumberjack_CutTree Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("Lumberjack_StartAnimCutTree")]
        public static void Lumberjack_StartAnimCutTree(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                
                /*if (DateTime.Now.Hour > 9 && DateTime.Now.Hour != 22 && DateTime.Now.Hour != 23)
                { // todo translate
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OgranVremyaMiner), 3000);
                    Trigger.ClientEvent(player, "lumberjackJob_stopProcess");
                    return;
                }*/

                Main.OnAntiAnim(player);
                Trigger.PlayAnimation(player, "melee@large_wpn@streamed_core", "car_side_attack_a", 1);
                // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "lumberjack");
                Attachments.AddAttachment(player, Attachments.AttachmentsName.WorkAxeProp);
            }
            catch (Exception e)
            {
                Log.Write($"Lumberjack_StartAnimCutTree Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("Lumberjack_StopCutTree")]
        public static void Lumberjack_StopCutTree(ExtPlayer player, int TreeIndex)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                Main.OffAntiAnim(player);
                Trigger.StopAnimation(player);
                Attachments.RemoveAttachment(player, Attachments.AttachmentsName.WorkAxeProp);

                if (TreesData.ContainsKey(TreeIndex))
                    TreesData[TreeIndex].StartedTreeHitPlayerInfo = null;
            }
            catch (Exception e)
            {
                Log.Write($"Lumberjack_StopCutTree Exception: {e.ToString()}");
            }
        }

        private static List<ItemId> GetItems()
        {
            return new List<ItemId>()
            {
                ItemId.WoodOak,
                ItemId.WoodMaple,
                ItemId.WoodPine,
            };
        } 
        [RemoteEvent("Lumberjack_TakeTimber")]
        public static void Lumberjack_TakeTimber(ExtPlayer player, int TreeIndex)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                var characterData = player.GetCharacterData();
                if (characterData == null) return;

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
                else if (sessionData.AnimationUse != null || Main.IHaveDemorgan(player, true)) return;

                if (TreesData.ContainsKey(TreeIndex))
                {
                    var treesData = TreesData[TreeIndex];
                    if (treesData.TreeStage != 1) return;

                    int item_chance = new Random().Next(1, 100);
                    ItemId item_type = ItemId.WoodOak;

                    if (item_chance <= 33) item_type = ItemId.WoodOak;
                    else if (item_chance > 33 && item_chance <= 66) item_type = ItemId.WoodMaple;
                    else if (item_chance > 66) item_type = ItemId.WoodPine;

                    if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", item_type, 1) == -1)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMoreLogs), 3000);
                        return;
                    }

                    Trigger.TaskPlayAnim(player, "random@domestic", "pickup_low", 39);

                    treesData.TreeStage = 2;

                    NAPI.ClientEvent.TriggerClientEventInRange(treesData.TreePosition, 2.5f, "lumberjackJob_updateTreeInfo", TreeIndex, treesData.TreeStage, treesData.TreeHP);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Lumberjack_TakeTimber Exception: {e.ToString()}");
            }
        }

        private static void TreeRebirthTimerFunction(int TreeIndex)
        {
            try
            {
                if (TreesData.ContainsKey(TreeIndex)) 
                { 
                    var treesData = TreesData[TreeIndex]; 
                    if (treesData.TreeStage == 0) return; 
 
                    treesData.TreeHP = 100; 
                    treesData.TreeStage = 0; 
                    treesData.StartedTreeHitPlayerInfo = null; 
 
                    if (treesData.TreeObject != null && treesData.TreeObject.Exists)  
                        treesData.TreeObject.Delete(); 
                     
                    treesData.TreeObject = NAPI.Object.CreateObject(NAPI.Util.GetHashKey(TreesPositions[TreeIndex].Item1), new Vector3(TreesPositions[TreeIndex].Item2.X, TreesPositions[TreeIndex].Item2.Y, TreesPositions[TreeIndex].Item2.Z - 0.45), new Vector3(0, 0, 0), 255, 0); 
 
                    NAPI.ClientEvent.TriggerClientEventInRange(treesData.TreePosition, 2.5f, "lumberjackJob_updateTreeInfo", TreeIndex, treesData.TreeStage, treesData.TreeHP); 
                } 
            }
            catch (Exception e)
            {
                Log.Write($"TreeRebirthTimerFunction Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("PlayerSellTrees")]
        public static void PlayerSellTrees(ExtPlayer player, int stock_item_index, int amount)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (!FunctionsAccess.IsWorking("PlayerSellTrees"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                else if (TreeStock.Count < stock_item_index) return;

                ItemId item_type = ItemId.WoodOak;
                int price = Main.LumberjackPrice.OakPrice;

                if (stock_item_index == 1)
                {
                    item_type = ItemId.WoodMaple;
                    price = Main.LumberjackPrice.MaplePrice;
                }
                else if (stock_item_index == 2)
                {
                    item_type = ItemId.WoodPine;
                    price = Main.LumberjackPrice.PinePrice;
                }

                ItemStruct woodamount = Chars.Repository.isItem(player, "inventory", item_type);
                int count = (woodamount == null) ? 0 : woodamount.Item.Count;
                bool success = false;

                if (count < amount)
                {
                    count = Chars.Repository.getCountToLacationItem($"char_{characterData.UUID}", "inventory", item_type);
                    if (count >= amount)
                    {
                        success = true;
                        for (int i = 0; i < amount; i++) Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", item_type, 1);
                    }
                }
                else
                {
                    success = true;
                    Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", item_type, amount);
                }
                
                if (success == true)
                {
                    int totalmoney = Convert.ToInt32(price * amount);
                    Wallet.Change(player, totalmoney);
                    GameLog.Money($"server", $"player({characterData.UUID})", totalmoney, $"sellTrees({stock_item_index},{amount})");

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SellResources, Wallet.Format(totalmoney)), 3000);
                    //BattlePass.Repository.UpdateReward(player, 59);

                    TreeStock[stock_item_index] += amount;
                    TreeStockLabel.Text = Main.StringToU16(LangFunc.GetText(LangType.Ru, DataName.Hranilishe,  TreeStock[0], TreeStock[1], TreeStock[2]));

                    Perform(player, 1);
                    
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
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NotEnoughRes), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"PlayerSellOres Exception: {e.ToString()}");
            }
        }
    }
}