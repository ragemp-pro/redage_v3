using GTANetworkAPI;
using NeptuneEvo.Handles;
using Redage.SDK;
using System;
using NeptuneEvo.Core;
using NeptuneEvo.Houses;
using System.Collections.Generic;
using Localization;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Chars.Models;

namespace NeptuneEvo.Chars
{
    class Events : Script
    {
        /// <summary>
        /// Логгер
        /// </summary>
        private static readonly nLog Log = new nLog("Chars.Events");
        [RemoteEvent("server.character.trade")]
        public void TryTrade(ExtPlayer player, ExtPlayer target, string type)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (DateTime.Now <= sessionData.RequestData.Time && sessionData.RequestData.IsRequested)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveActiveOrders), 3000);
                    return;
                }
                if ((sessionData.SellItemData.Seller != null || sessionData.SellItemData.Buyer != null) && Repository.TradeGet(player))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantTrade), 3000);
                    return;
                }
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) return;
                else if (DateTime.Now <= targetSessionData.RequestData.Time && targetSessionData.RequestData.IsRequested)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PersonHavBeenBusy), 7000);
                    return;
                }
                else if ((targetSessionData.SellItemData.Seller != null || targetSessionData.SellItemData.Buyer != null) && Repository.TradeGet(target))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PersonCantTrade), 3000);
                    return;
                }

                if (type == "vehicle")
                {
                    var vehiclesCount = VehicleManager.GetVehiclesCarCountToPlayer(player.Name);
                    if (vehiclesCount == 0)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoCar), 3000);
                        return;
                    }
                    vehiclesCount = VehicleManager.GetVehiclesCarCountToPlayer(target.Name);
                    if (vehiclesCount == 0)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PersonHaveNoCar), 3000);
                        return;
                    }
                    targetSessionData.RequestData.IsRequested = true;
                    targetSessionData.RequestData.Request = "trade_vehicle";
                    targetSessionData.RequestData.From = player;
                    targetSessionData.RequestData.Time = DateTime.Now.AddSeconds(10);
                    //Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouWantTradeVeh), 3000);
                    //Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PersonWantTradeVeh, player.Value), 3000);
                    EventSys.SendCoolMsg(player,"Предложение", "Обмен", $"{LangFunc.GetText(LangType.Ru, DataName.YouWantTradeVeh)}", "", 10000);
                    EventSys.SendCoolMsg(target,"Предложение", "Обмен", $"{LangFunc.GetText(LangType.Ru, DataName.PersonWantTradeVeh, player.Value)}", "", 10000);
                }
                else if (type == "house")
                {

                    var house = HouseManager.GetHouse(player, true);
                    if (house == null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoHome), 3000);
                        return;
                    }
                    House houseTarget = HouseManager.GetHouse(target, true);

                    if (houseTarget == null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PersonNoHome), 3000);
                        return;
                    }

                    targetSessionData.RequestData.IsRequested = true;
                    targetSessionData.RequestData.Request = "trade_house";
                    targetSessionData.RequestData.From = player;
                    targetSessionData.RequestData.Time = DateTime.Now.AddSeconds(10);
                    //Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouWantTradeHouse), 3000);
                    //Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PersonWantTradeHouse), 3000);
                    EventSys.SendCoolMsg(player,"Предложение", "Обмен", $"{LangFunc.GetText(LangType.Ru, DataName.YouWantTradeHouse)}", "", 10000);
                    EventSys.SendCoolMsg(target,"Предложение", "Обмен", $"{LangFunc.GetText(LangType.Ru, DataName.PersonWantTradeHouse)}", "", 10000);
                }
                else if (type == "business")
                {
                    if (characterData.BizIDs.Count == 0)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoBusiness), 3000);
                        return;
                    }
                    else if (targetCharacterData.BizIDs.Count == 0)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PersonHaveNoBusiness), 3000);
                        return;
                    }
                    targetSessionData.RequestData.IsRequested = true;
                    targetSessionData.RequestData.Request = "trade_business";
                    targetSessionData.RequestData.From = player;
                    targetSessionData.RequestData.Time = DateTime.Now.AddSeconds(10);
                    //Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouWantTradeBiz), 3000);
                    //Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PersonWantTradeBiz), 3000);
                    EventSys.SendCoolMsg(player,"Предложение", "Обмен", $"{LangFunc.GetText(LangType.Ru, DataName.YouWantTradeBiz)}", "", 10000);
                    EventSys.SendCoolMsg(target,"Предложение", "Обмен", $"{LangFunc.GetText(LangType.Ru, DataName.PersonWantTradeBiz)}", "", 10000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"TryTrade Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.roullete.buy")]
        public void RoulleteBuy(ExtPlayer player, int caseid, int count)
        {
            try
            {
                Repository.RouletteBuyCase(player, caseid, count);
            }
            catch (Exception e)
            {
                Log.Write($"RoulleteOpen Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.roullete.open")]
        public void RoulleteOpen(ExtPlayer player, int caseid, int count)
        {
            try
            {
                Repository.RouletteOpenCase(player, caseid, count);
            }
            catch (Exception e)
            {
                Log.Write($"RoulleteOpen Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.roullete.confirm")]
        public static void RoulleteConfirm(ExtPlayer player, bool type, int IndexList)
        {
            try
            {
                if (IndexList == -1)
                {
                    var sessionData = player.GetSessionData();
                    if (sessionData == null) return;

                    List<int> _DoneListId = new List<int>();
                    foreach (RouletteData playerData in sessionData.RouletteData)
                    {
                        if (!playerData.Done)
                            _DoneListId.Add(playerData.IndexList);
                    }
                    foreach(int index in _DoneListId)
                    {
                        Repository.RouletteConfirmCase(player, type, index);
                    }
                }
                else Repository.RouletteConfirmCase(player, type, IndexList);
            }
            catch (Exception e)
            {
                Log.Write($"RoulleteConfirm Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.gamemenu.inventory.move")]
        public void InventoryMove(ExtPlayer player, string selectArrayName, int selectIndex, string hoverArrayName, int hoverIndex)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Repository.ItemsMove(player, selectArrayName, selectIndex, hoverArrayName, hoverIndex);
            }
            catch (Exception e)
            {
                Log.Write($"InventoryMove Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.gamemenu.inventory.move.stack")]
        public void InventoryMoveStack(ExtPlayer player, string selectArrayName, int selectIndex, string hoverArrayName, int hoverIndex, int count)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Repository.ItemsMoveStack(player, selectArrayName, selectIndex, hoverArrayName, hoverIndex, count);
            }
            catch (Exception e)
            {
                Log.Write($"InventoryMoveStack Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.gamemenu.inventory.use")]
        public void InventoryUse(ExtPlayer player, string ArrayName, int Index)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Repository.ItemsUse(player, ArrayName, Index);
            }
            catch (Exception e)
            {
                Log.Write($"InventoryUse Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.note.create")]
        public void NoteCreate(ExtPlayer player, int type, int ItemId, string nameValue, string textValue)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Repository.NoteCreate(player, type, ItemId, nameValue, textValue);
            }
            catch (Exception e)
            {
                Log.Write($"InventoryUse Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.gamemenu.inventory.drop")]
        public void InventoryDrop(ExtPlayer player, string ArrayName, int Index, float posZ)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (ArrayName == "fastSlots") return;
                if (DateTime.Now < sessionData.TimingsData.NextDropItem)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CooldownItemDrop), 3000);
                    return;
                }
                else if (player.IsInVehicle)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoDropFromCar), 3000);
                    return;
                }
                else if (sessionData.AntiAnimDown)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantDrop), 3000);
                    return;
                }
                Repository.ItemsDropToIndex(player, ArrayName, Index, true, posZ: posZ);
            }
            catch (Exception e)
            {
                Log.Write($"InventoryDrop Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.gamemenu.inventory.trade")]
        public void InventoryTrade(ExtPlayer player, int status)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Repository.ItemsTrade(player, status);
            }
            catch (Exception e)
            {
                Log.Write($"InventoryTrade Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.gamemenu.inventory.tradeMoney")]
        public void InventoryTradeMoney(ExtPlayer player, int money)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Repository.ItemsTradeMoney(player, money);
            }
            catch (Exception e)
            {
                Log.Write($"InventoryTradeMoney Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.gamemenu.inventory.close")]
        public void InventoryClose(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Repository.ItemsClose(player);
            }
            catch (Exception e)
            {
                Log.Write($"InventoryClose Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.gamemenu.inventory.otherclose")]
        public void InventoryOtherClose(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Repository.OtherClose(player);
            }
            catch (Exception e)
            {
                Log.Write($"InventoryOtherClose Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.gamemenu.inventory.stack")]
        public void InventoryStack(ExtPlayer player, string ArrayName, int Index, int Id, int Value)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (ArrayName == "fastSlots") return;
                Repository.ItemStack(player, ArrayName, Index, Id, Value);
            }
            catch (Exception e)
            {
                Log.Write($"InventoryStack Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.gamemenu.inventory.buy")]
        public void InventoryBuy(ExtPlayer player, string ArrayName, int Index, int Value)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (ArrayName == "fastSlots") return;

                InventoryItemData Item = Repository.GetItemData(player, ArrayName, Index);

                if (Item.ItemId == ItemId.Debug) return;
                if (Item.Price == 0) return;
                if (Repository.isFreeSlots(player, Item.ItemId, Value) != 0) return;

                sessionData.InventoryTentData = new InventoryTentData
                {
                    ArrayName = ArrayName,
                    Index = Index,
                    Value = Value
                };
                Trigger.ClientEvent(player, "openDialog", "buy_tent", LangFunc.GetText(LangType.Ru, DataName.DialogBuyFromTent, Repository.ItemsInfo[Item.ItemId].Name, MoneySystem.Wallet.Format(Item.Price )));

                //Repository.ItemBuy(player, ArrayName, Index, Value);
            }
            catch (Exception e)
            {
                Log.Write($"InventoryStack Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.gamemenu.updatestats")]
        public void UpdateStats(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Repository.PlayerStats(player);
            }
            catch (Exception e)
            {
                Log.Write($"UpdateStats Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.gamemenu.inventory.toput")]
        public void InventoryToput(ExtPlayer player, string ArrayName, int Index)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Repository.ItemsToput(player, ArrayName, Index);
            }
            catch (Exception e)
            {
                Log.Write($"InventoryToput Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("animationEvent")]
        public void animationEvent(ExtPlayer player, bool toggle, string dirt, string name, int flag)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (!toggle) Trigger.StopAnimation(player);
                player.PlayAnimation(dirt, name, flag);
            }
            catch (Exception e)
            {
                Log.Write($"animationEvent Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.dropeditor.finish")]
        public void dropEditorFinish(ExtPlayer player, string arrayName, int index, float posX, float posY, float posZ, float rotX, float rotY, float rotZ)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (player.IsInVehicle)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoDropFromCar), 3000);
                    return;
                }
                Repository.ItemsDropToEditor(player, arrayName, index, posX, posY, posZ, rotX, rotY, rotZ);
            }
            catch (Exception e)
            {
                Log.Write($"dropEditorFinish Exception: {e.ToString()}");
            }
        }
    }
}