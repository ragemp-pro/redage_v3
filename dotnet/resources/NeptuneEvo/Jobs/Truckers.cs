using GTANetworkAPI;
using NeptuneEvo.Handles;
using System;
using System.Linq;
using System.Collections.Generic;
using Localization;
using NeptuneEvo.Core;
using Redage.SDK;
using NeptuneEvo.Functions;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players;
using NeptuneEvo.Character;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Jobs.Models;
using NeptuneEvo.Quests;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;

namespace NeptuneEvo.Jobs
{
    class Truckers : Script
    {
        private static readonly nLog Log = new nLog("Jobs.Truckers");

        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            for (int i = 0; i < GetProduct.Count; i++)
            {
                Main.CreateBlip(new Main.BlipData(557, LangFunc.GetText(LangType.Ru, DataName.ZagruzkaTovara), GetProduct[i], 84, true));
                CustomColShape.CreateCylinderColShape(GetProduct[i], 50F, 2F, 0, ColShapeEnums.GetProductTrucker);
            }
        }

        public static List<Vector3> GetProduct = new List<Vector3>()
        {
            new Vector3(777.0558, -2974.624, 5.80066), // 24/7 products && burger-shot && clothesshop && tattoo-salon && barber-shop && masks shop && ls customs
        };

        public static Vector3 GetNearestGetProduct(Vector3 position)
        {
            Vector3 nearesetArea = new Vector3();
            foreach (var v in GetProduct)
            {
                if (nearesetArea == new Vector3()) nearesetArea = v;
                else if (position.DistanceTo(v) < position.DistanceTo(nearesetArea)) nearesetArea = v;
            }
            return nearesetArea;
        }
        
        public static bool IsPointProduct(Vector3 position)
        {
            foreach (var v in GetProduct)
            {
                if (position.DistanceTo(v) < 30.0)
                    return true;
            }
            return false;
        }
        
        public static void CancelOrder(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (sessionData.OrderData.Order != -1)
                {
                    var uid = sessionData.OrderData.Order;
                    
                    if (!BusinessManager.Orders.ContainsKey(uid))
                        return;
                    
                    var bizId = BusinessManager.Orders[uid];
                    var biz = BusinessManager.BizList[bizId];
                    var bizOrder = biz.Orders.FirstOrDefault(o => o.UID == uid);
                    if (bizOrder != null)
                        bizOrder.Taked = false;
                    
                    sessionData.OrderData.Order = -1;
                }
            }
            catch (Exception e)
            {
                Log.Write($"OnCancel Exception: {e}");
            }
        }
        public static void StartWork(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            sessionData.WorkData.OnWork = true;
            sessionData.OrderData.WayPoint = null;
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GoToZagruz), 10000);
            var pos = GetNearestGetProduct(player.Position);
            Trigger.ClientEvent(player, "createWaypoint", pos.X, pos.Y);
        }
        
        public static bool EndWork(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return false;
            
            if (sessionData.WorkData.OnWork)
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.EndWorkDay), 3000);
                sessionData.WorkData.OnWork = false;
                sessionData.OrderData.WayPoint = null;
                CancelOrder(player);
                Trigger.ClientEvent(player, "deleteCheckpoint", 3, 0);
                Trigger.ClientEvent(player, "deleteWorkBlip");
                return true;
            }
            return false;
        }
        [ServerEvent(Event.PlayerEnterVehicle)]
        public void OnPlayerEnterVehicle(ExtPlayer player, ExtVehicle vehicle, sbyte seatId)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            if (characterData.WorkID != (int)JobsId.Trucker) 
                return;
            
            if (sessionData.OrderData.WayPoint != null)
            {
                var biz = sessionData.OrderData.WayPoint;
                Trigger.ClientEvent(player, "createWaypoint", biz.UnloadPoint.X, biz.UnloadPoint.Y);
            }
        }
        [Interaction(ColShapeEnums.GetProductTrucker, In: true)]
        public static void OnGetProductTrucker(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            
            if (characterData.WorkID == (int)JobsId.Trucker) 
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Откройте телефон для взятия заказа", 10000);
        }
        

        public static void playerGotProducts(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var rentData = sessionData.RentData;
                if (rentData == null || sessionData.OrderData.Order == -1 || !player.IsInVehicle) return;
                var vehicle = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if (vehicleLocalData.Access != VehicleAccess.Work || rentData.Vehicle != vehicle) return;
                    var uid = sessionData.OrderData.Order;
                    if (!BusinessManager.Orders.ContainsKey(uid))
                    {
                        CancelOrder(player);
                        return;
                    }
                    var bizId = BusinessManager.Orders[uid];
                    var biz = BusinessManager.BizList[bizId];
                    var bizOrder = biz.Orders.FirstOrDefault(o => o.UID == uid);
                    if (bizOrder == null)
                    {
                        CancelOrder(player);
                        return;
                    }
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DoveziteDo, BusinessManager.BusinessTypeNames[biz.Type]), 3000);
                    sessionData.OrderData.GotProduct = true;
                    sessionData.OrderData.WayPoint = biz;
                    Trigger.ClientEvent(player, "createWaypoint", biz.UnloadPoint.X, biz.UnloadPoint.Y);
                    Trigger.ClientEvent(player, "createCheckpoint", 10, 1, biz.UnloadPoint - new Vector3(0.0, 0.0, 1.5), 7, 0, 255, 0, 0);
                    
                    if (Chars.Repository.isFreeSlots(player, ItemId.Wrench, 1) == 0)
                        Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Wrench, 1);
                }
            }
            catch (Exception e)
            {
                Log.Write($"playerGotProducts1 Exception: {e}");
            }
        }
        [Interaction(ColShapeEnums.Trucker, In: true)]
        public static void OnTrucker(ExtPlayer player, int bizId)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var rentData = sessionData.RentData;
                if (rentData == null || characterData.WorkID != (int)JobsId.Trucker || characterData.WorkID == (int)JobsId.Trucker && !sessionData.WorkData.OnWork) return;
                if (rentData.Vehicle != player.Vehicle)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustBeInTruck), 3000);
                    return;
                }
                var uid = sessionData.OrderData.Order;
                if (uid == -1 || (BusinessManager.Orders.ContainsKey(uid) && BusinessManager.Orders[uid] != bizId) || !sessionData.OrderData.GotProduct) return;
                if (!BusinessManager.Orders.ContainsKey(uid))
                {
                    CancelOrder(player);
                    return;
                }

                var biz = BusinessManager.BizList[bizId];
                if (biz == null) return;
                var bizOrder = biz.Orders.FirstOrDefault(o => o.UID == uid);
                if (bizOrder == null)
                {
                    CancelOrder(player);
                    return;
                }

                var coef = Group.GroupPayAdd[accountData.VipLvl];
                var payment = Players.Phone.Property.Businesses.Orders.Repository.GetPrice(bizOrder.Amount, bizOrder.Name, coef);
                
                MoneySystem.Wallet.Change(player, payment);
                GameLog.Money($"server", $"player({characterData.UUID})", payment, $"truckerCheck");
                var ow = (ExtPlayer) NAPI.Player.GetPlayerFromName(biz.Owner);
                sessionData.OrderData.WayPoint = null;
                if (ow != null) 
                    Notify.Send(ow, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouOrderDeliveried, bizOrder.Name), 3000);
                
                if (qMain.GetQuestsLine(player, Zdobich.QuestName) == (int)zdobich_quests.Stage11)
                {
                    sessionData.WorkData.PointsCount += payment;
                    if (sessionData.WorkData.PointsCount < qMain.GetQuestsData(player, Zdobich.QuestName, (int) zdobich_quests.Stage11))
                        sessionData.WorkData.PointsCount = qMain.GetQuestsData(player, Zdobich.QuestName, (int) zdobich_quests.Stage11) + payment;
                    
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

                var product = biz.Products.FirstOrDefault(p => p.Name == bizOrder.Name);
                if (product != null) 
                {
                    product.Ordered = false;
                    product.Lefts += bizOrder.Amount;
                }
                
                biz.Orders.Remove(bizOrder);
                BusinessManager.Orders.TryRemove(uid, out _);
                Trigger.ClientEvent(player, "deleteCheckpoint", 10);
                sessionData.OrderData.GotProduct = false;
                sessionData.OrderData.Order = -1;
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TrucSuccDelivery), 3000);

                if (sessionData.LastPointTime > DateTime.Now)
                    Trigger.SendToAdmins(1, LangFunc.GetText(LangType.Ru, DataName.AnticheatDostavka, player.Name, player.Value));
                
                sessionData.LastPointTime = DateTime.Now.AddSeconds(59);

                if (characterData.JobSkills.ContainsKey((int)JobsId.Trucker))
                {
                    if (characterData.JobSkills[(int)JobsId.Trucker] < 700)
                        characterData.JobSkills[(int)JobsId.Trucker] += 1;
                }
                else characterData.JobSkills.Add((int)JobsId.Trucker, 1);
                
                var wrench = Chars.Repository.isItem(player, "inventory", ItemId.Wrench);
                if (wrench != null)
                    Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", wrench.Item.ItemId, 1);
            }
            catch (Exception e)
            {
                Log.Write($"onEntityEnterDropTrailer Exception: {e}");
            }
        }
    }
}
