using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Handles;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.GUI;
using NeptuneEvo.Inventory.Drop.Models;
using NeptuneEvo.Players;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Inventory.Drop
{
    public class Repository
    {
        private static readonly nLog Log = new nLog("Inventory.Drop.Repository");
        private static ConcurrentDictionary<GTANetworkAPI.Object, DropData> DropsData = new ConcurrentDictionary<GTANetworkAPI.Object, DropData>();


        public static void OnResourceStart()
        {
            Timers.Start("dropControl", 30000, () => DropControl(), true);
        }

        private static void DropControl()
        {
            var myObjects = DropsData
                .Where(o => DateTime.Now >= o.Value.DeleteTime)
                .Select(d => d.Key)
                .ToList();

            foreach(var obj in myObjects)
            {
                DeleteObject(obj);
            }
        }
        public static void PutToObject(ExtPlayer player, InventoryItemData item, float posX, float posY, float posZ, float rotX, float rotY, float rotZ, bool isMy = false)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
                
                var itemInfo = Chars.Repository.ItemsInfo[item.ItemId];

                var dim = UpdateData.GetPlayerDimension(player);

                var dropData = new DropData();
                
                dropData.Item = item;
                
                Chars.Repository.UpdateSqlItemData("drop", "drop", 0, item);

                dropData.DeleteTime = DateTime.Now.AddMinutes(360);
                
                GTANetworkAPI.Object obj = NAPI.Object.CreateObject(itemInfo.Model, new Vector3(posX, posY, posZ), new Vector3(rotX, rotY, rotZ), 255, dim);
                
                var data = item.Data; 
                if (item.ItemId == ItemId.CarKey) data = Chars.Repository.GetVehicleName(data); 
                
                var itemData = new Dictionary<string, object>
                {
                    { "ItemId", item.ItemId },
                    { "Count", item.Count},
                    { "Data", data }
                };

                if (isMy)
                {
                    itemData.Add("pId", player.Value);
                    dropData.Player = player;
                }
                
                obj.SetSharedData("DropData", JsonConvert.SerializeObject(itemData));

                DropsData[obj] = dropData;
            }
            catch (Exception e)
            {
                Log.Write($"PutToMyObject Exception: {e.ToString()}");
            }
        }

        private static void DeleteObject(GTANetworkAPI.Object obj)
        {
            try
            {
                if (obj == null || !DropsData.ContainsKey(obj))
                    return;
                
                var dropData = DropsData[obj];

                if (dropData.Item != null)
                {
                    var locationName = $"drop";
                    var location = "drop";
                    
                    Chars.Repository.UpdateSqlItemData(locationName, location, -1, new InventoryItemData(SqlId: dropData.Item.SqlId));
                }
                
                if (obj.Exists) 
                    obj.Delete();

                DropsData.TryRemove(obj, out _);
            }
            catch (Exception e)
            {
                Log.Write($"DeleteObject Exception: {e.ToString()}");
            }
            
        }
        public static void ItemRaise(ExtPlayer player, GTANetworkAPI.Object obj)
        {
            try
            {
                if (obj == null || !DropsData.ContainsKey(obj))
                    return;
                
                var dropData = DropsData[obj];
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
                
                if (sessionData.AdminData.IsRemoveObject)
                {
                    DeleteObject(obj);
                    sessionData.AdminData.IsRemoveObject = false;
                    return;
                }
                if (characterData.AdminLVL >= 1 && characterData.AdminLVL <= 4)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AdminCantPickupItems), 3000);
                    return;
                }
                if (dropData.Player != null && dropData.Player != player) return;

                if (player.IsInVehicle)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantPickupInCar), 3000);
                    return;
                }
                
                if (!obj.Exists || obj.Position.DistanceTo(player.Position) >= 5)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TooFar), 1500);
                    return;
                }
                var dropItem = dropData.Item;
                
                if (dropItem == null || dropItem.ItemId == ItemId.Debug)
                    return;
                
                var itemsInfo = Chars.Repository.ItemsInfo[dropItem.ItemId];
                var count = Chars.Repository.isFreeSlots(player, dropItem.ItemId, dropItem.Count, false);
                if (count < 0) return;
                if (dropItem.ItemId == ItemId.BagWithDrill || dropItem.ItemId == ItemId.BagWithMoney)
                {
                    var item = Chars.Repository.GetItemData(player, "accessories", 8);
                    if (item.ItemId != ItemId.Debug)
                    {
                        var freeSlotId = Chars.Repository.AddItem(player, $"char_{characterData.UUID}", "inventory", item);
                        if (freeSlotId == -1) return;
                    }
                    if (dropItem.SqlId == 0)
                    {
                        Chars.Repository.AddSqlItem(player, $"char_{characterData.UUID}", "accessories", dropItem.ItemId, 8, count == 0 ? dropItem.Count : count, dropItem.Data);
                    }
                    else Chars.Repository.SetItemData(player, "accessories", 8, dropItem, true);
                }
                else if (dropItem.SqlId != 0 && count == 0) Chars.Repository.AddItem(player, $"char_{characterData.UUID}", "inventory", dropItem);
                else Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", dropItem.ItemId, count == 0 ? dropItem.Count : count, dropItem.Data);
                if (count == 0)
                {
                    Commands.RPChat("sme", player, $"поднял" + (characterData.Gender ? "" : "а") + $" {itemsInfo.Name}");
                    dropData.Item = null;
                    DeleteObject(obj);
                    BattlePass.Repository.UpdateReward(player, 17);
                }
                else
                {
                    Commands.RPChat("sme", player, $"поднял" + (characterData.Gender ? "" : "а") + $" часть {itemsInfo.Name}");
                    dropItem.Count -= count;

                    try
                    {
                        var itemData =
                            JsonConvert.DeserializeObject<Dictionary<string, object>>(
                                obj.GetSharedData<string>("DropData"));

                        itemData["Count"] = dropItem.Count;
                        
                        obj.SetSharedData("DropData", JsonConvert.SerializeObject(itemData));
                    }
                    catch
                    {    
                        var data = dropItem.Data; 
                        if (dropItem.ItemId == ItemId.CarKey) data = Chars.Repository.GetVehicleName(data); 
                        
                        var itemData = new Dictionary<string, object>
                        {
                            { "ItemId", dropItem.ItemId },
                            { "Count", dropItem.Count},
                            { "Data", data }
                        };
                        
                        obj.SetSharedData("DropData", JsonConvert.SerializeObject(itemData));
                    }
                    
                    //odata.entity.SetSharedData("InteriorItem", false);
                }
                Main.OnAntiAnim(player);
                player.PlayAnimation("random@domestic", "pickup_low", 39);
                // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "itemraise");
                Timers.StartOnce(1700, () =>
                {
                    try
                    {
                        if (!player.IsCharacterData()) return;
                        Trigger.StopAnimation(player);
                        Main.OffAntiAnim(player);
                    }
                    catch { Log.Write("objectSelected11 ERROR"); }
                });
            }
            catch (Exception e)
            {
                Log.Write($"ItemRaise Exception: {e.ToString()}");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="entityidString"></param>
        public static void OnHookah(ExtPlayer player, GTANetworkAPI.Object obj)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                if (characterData.DemorganTime >= 1 || sessionData.CuffedData.Cuffed || sessionData.DeathData.InDeath || sessionData.AntiAnimDown || !characterData.IsAlive) return;
                
                
                if (!DropsData.ContainsKey(obj))
                    return;
                
                var dropData = DropsData[obj];
                
                if (!obj.Exists || obj.Position.DistanceTo(player.Position) >= 5)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TooFar), 1500);
                    return;
                }
                
                var dropItem = dropData.Item;
                
                int value = 0;
                
                if (dropItem.ItemId == ItemId.Debug)
                    return;

                if (!int.TryParse(dropItem.Data, out value) || value <= 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.HookahCantUse), 3000);
                    DeleteObject(obj);
                    return;
                }

                value--;
                dropItem.Data = value.ToString();
                
                //Chars.Repository.UpdateSqlItemData("drop", "drop", -1, dropItem);
                
                Chars.Repository.UseSmoke(player, ItemId: ItemId.Hookah);
            }
            catch (Exception e)
            {
                Log.Write($"callback_HookahManageMenu Exception: {e.ToString()}");
            }
        }

        
        public static void OnPlayerDisconnected(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;

                var myObjects = DropsData
                    .Where(o => o.Value.Player == player)
                    .Select(o => o.Key)
                    .ToList();

                foreach(var obj in myObjects)
                {
                    if (DropsData.ContainsKey(obj))
                    {
                        var dropData = DropsData[obj];
                        var dropItem = dropData.Item;

                        if (dropItem != null && dropItem.ItemId != ItemId.Debug)
                        {
                            int freeSlotId = Chars.Repository.AddItem(player, $"char_{characterData.UUID}", "inventory", dropItem);
                            if (freeSlotId != -1)
                            {
                                dropData.Item = null;
                            }
                        }
                        DeleteObject(obj);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"OnPlayerDisconnected Exception: {e.ToString()}");
            }
        }
    }
}