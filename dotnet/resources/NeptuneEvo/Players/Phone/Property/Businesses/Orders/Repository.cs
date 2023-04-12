using System;
using System.Collections.Generic;
using System.Linq;
using Localization;
using NeptuneEvo.Accounts;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.Jobs;
using NeptuneEvo.Jobs.Models;
using NeptuneEvo.VehicleData.LocalData;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Players.Phone.Property.Businesses.Orders
{
    public class Repository
    {
        public static int GetPrice(int amount, string name, float coef)
        {
            
            var price = Convert.ToInt32(amount * BusinessManager.BusProductsData[name].Price * 0.5 * Main.ServerSettings.MoneyMultiplier);
            var max = Convert.ToInt32(Main.PricesSettings.DalnoboyMoney[0] * coef * Main.ServerSettings.MoneyMultiplier);
            var min = Convert.ToInt32(Main.PricesSettings.DalnoboyMoney[1] * coef * Main.ServerSettings.MoneyMultiplier);
            
            if (price > max) 
                price = max;
            else if (price < min) 
                price = min;

            return price;
        }
        
        public static void Open(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            var accountData = player.GetAccountData();
            if (accountData == null) 
                return;
            
            var orders = new List<List<object>>();
            var selectedOrders = new List<object>();
            
            if (characterData.WorkID != (int)JobsId.Trucker)
            {
                Trigger.ClientEvent(player, "client.phone.truck.init", JsonConvert.SerializeObject(selectedOrders), JsonConvert.SerializeObject(orders));
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouAreNotDalnoboy), 3000);
                return;
            }
            /*if (!sessionData.WorkData.OnWork)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouNotWorkingNow), 3000);
                return;
            }*/

            if (sessionData.OrderData.Order != -1)
            {
                var uid = sessionData.OrderData.Order;

                if (BusinessManager.Orders.ContainsKey(uid))
                {

                    var bizId = BusinessManager.Orders[uid];
                    var biz = BusinessManager.BizList[bizId];
                    var bizOrder = biz.Orders.FirstOrDefault(o => o.UID == uid);

                    if (bizOrder != null)
                    {
                        selectedOrders.Add(uid);
                        selectedOrders.Add(biz.Owner);
                        selectedOrders.Add(biz.EnterPoint.X);
                        selectedOrders.Add(biz.EnterPoint.Y);
                        selectedOrders.Add(biz.EnterPoint.Z);
                    }
                    else
                        Jobs.Truckers.CancelOrder(player);
                }
                else
                    Jobs.Truckers.CancelOrder(player);
            }
            
            if (sessionData.OrderData.Order == -1)
            {
                var coef = Group.GroupPayAdd[accountData.VipLvl];

                foreach (var order in BusinessManager.Orders)
                {
                    var biz = BusinessManager.BizList[order.Value];
                    var bizOrder = biz.Orders.FirstOrDefault(or => or.UID == order.Key);
                    if (bizOrder == null || bizOrder.Taked ||
                        !BusinessManager.BusProductsData.ContainsKey(bizOrder.Name)) continue;

                    orders.Add(new List<object>
                    {
                        order.Key,
                        bizOrder.Name,
                        GetPrice(bizOrder.Amount, bizOrder.Name, coef),
                        biz.EnterPoint.X,
                        biz.EnterPoint.Y,
                        biz.EnterPoint.Z
                    });
                }
            }

            Trigger.ClientEvent(player, "client.phone.truck.init", JsonConvert.SerializeObject(selectedOrders), JsonConvert.SerializeObject(orders));
        }

        public static void Take(ExtPlayer player, int uid)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            var accountData = player.GetAccountData();
            if (accountData == null) 
                return;
            
            if (!FunctionsAccess.IsWorking("phonetruck"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                return;
            }
            
            if (sessionData.OrderData.Order != -1)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyTakeOrder), 3000);
                Open(player);
                return;
            }

            if (!Truckers.IsPointProduct(player.Position))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMustBeAtZagruzka), 3000);

                var pos = Truckers.GetNearestGetProduct(player.Position);
                Trigger.ClientEvent(player, "createWaypoint", pos.X, pos.Y);
                return;
            }
            
            var vehicle = (ExtVehicle)player.Vehicle; 
            var vehicleLocalData = vehicle.GetVehicleLocalData(); 
            if (vehicleLocalData == null || vehicleLocalData.WorkId != JobsId.Trucker) 
            { 
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkCar), 3000); 
                return; 
            } 
            
            //Trigger.ClientEvent(player, "createWaypoint", biz.UnloadPoint.X, biz.UnloadPoint.Y);
            if (!BusinessManager.Orders.ContainsKey(uid))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoOrderExists), 3000);
                Open(player);
                return;
            }
            
            var bizId = BusinessManager.Orders[uid];
            var biz = BusinessManager.BizList[bizId];
            var bizOrder = biz.Orders.FirstOrDefault(o => o.UID == uid);
            if (bizOrder == null || bizOrder.Taked)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ThisOrderTaken), 3000);
                Open(player);
                return;
            }
            bizOrder.Taked = true;
            sessionData.OrderData.Order = uid;
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OrderDeliveryTaken, bizOrder.Name, BusinessManager.BusinessTypeNames[biz.Type]), 3000);
            Truckers.playerGotProducts(player);
            Open(player);
        }

        public static void Cancel(ExtPlayer player)
        {
            Truckers.CancelOrder(player);
            Open(player);
        }
        
    }
}