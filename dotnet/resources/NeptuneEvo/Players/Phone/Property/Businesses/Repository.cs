using System;
using System.Collections.Generic;
using System.Linq;
using Localization;
using NeptuneEvo.Accounts;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Handles;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.Players.Phone.Messages.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Players.Phone.Property.Businesses
{
    public class Repository
    {
        public static void GetData(ExtPlayer player, int bizId)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            if (!characterData.BizIDs.Contains(bizId))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoBusiness), 3000);
                return;
            }

            sessionData.SelectData.SelectedBiz = bizId;
                
            var biz = BusinessManager.BizList[bizId];

            var whCount = 0;
            var whMaxCount = 0;
            var whPriceMaxCount = 0;
            
            var stats = new List<object>()
            {
                biz.Type,//type
                Convert.ToInt32(biz.SellPrice / 100 * biz.Tax),//tax
                Bank.GetBalance(biz.BankID),//cash
                biz.Pribil,//pribil
                biz.Zatratq,//zatratq
            };

            var stocks = new List<List<object>>();
            foreach (var product in biz.Products)
            {
                if (!BusinessManager.BusProductsData.ContainsKey(product.Name))
                    continue;
                    
                var productsData = BusinessManager.BusProductsData[product.Name];
                var itemPrice = productsData.OtherPrice > 0 ? productsData.OtherPrice : productsData.Price;
                
                stocks.Add(new List<object>()
                {
                    product.Name,//name
                    product.Lefts,//count
                    productsData.MaxCount,//maxCount
                    product.Price,//price
                    productsData.OtherPrice,//otherPrice
                    productsData.Price,//defaultPrice
                    GetPriceMin(product.Name, biz.Type, productsData.Price),//minPrice
                    GetPriceMax(product.Name, biz.Type, productsData.Price),//maxPrice
                    productsData.ItemId,//
                    productsData.Type
                });

                whCount += product.Lefts;
                whMaxCount += productsData.MaxCount;

                if (!product.Ordered)
                {
                    var count = 0;

                    if (biz.Type >= 2 && biz.Type <= 5)
                        count = 3;
                    else
                    {
                        if (product.Lefts >= productsData.MaxCount) continue;
                        count = productsData.MaxCount - product.Lefts;
                    }

                    whPriceMaxCount += count * itemPrice;
                }
            }
            
            //
            
            stats.Add(whCount);//whCount
            stats.Add(whMaxCount);//whMaxCount
            stats.Add(whPriceMaxCount);//whPriceMaxCount
            stats.Add(Wallet.GetPriceToVip(player, biz.SellPrice));//sellPrice
            
            //GetHistory(player);
            
            //Orders
            
            var orders = new List<List<object>>();
            foreach (var order in biz.Orders)
            {
                orders.Add(new List<object>
                {
                    order.UID,
                    order.Name,
                    order.Amount
                });
            }
            
            //
            
            //Console.WriteLine(JsonConvert.SerializeObject(stats));
            //Console.WriteLine(JsonConvert.SerializeObject(stocks));
            //Console.WriteLine(JsonConvert.SerializeObject(orders));
            
            Trigger.ClientEvent(player, "client.phone.business.init", JsonConvert.SerializeObject(stats), JsonConvert.SerializeObject(stocks), JsonConvert.SerializeObject(orders));
        }
        
        private static int GetPriceMin(string productName, int bizType, int price)
        {
            if (productName == "Лотерейный билет")
                return price;
            
            var minPrice = price * Main.BusinessMinPrice;
            if (bizType == 1) minPrice = Main.PricesSettings.ZapravkaMinPrice;
            else if (bizType == 7) minPrice = Main.PricesSettings.ClothesMinPrice;
            else if (bizType == 9 || bizType == 10 || bizType == 11 || bizType == 12) minPrice = Main.PricesSettings.TattooBarberMasksLscMinPrice;

            return Convert.ToInt32(minPrice);
        }

        private static int GetPriceMax(string productName, int bizType, int price)
        {
            if (productName == "Лотерейный билет")
                return price;

            var maxPrice = price * Main.BusinessMaxPrice;
            
            if (bizType == 1) maxPrice = Main.PricesSettings.ZapravkaMaxPrice;
            else if (bizType == 7) maxPrice = Main.PricesSettings.ClothesMaxPrice;
            else if (bizType == 9 || bizType == 10 || bizType == 11 || bizType == 12) maxPrice = Main.PricesSettings.TattooBarberMasksLscMaxPrice;
            
            return Convert.ToInt32(maxPrice);
        }
        
        private static int CountMinOrder(int bizType) => (bizType == 14 || (bizType >= 2 && bizType <= 5)) ? 1 : 10;

        public static void AddOrder(ExtPlayer player, string name, int value)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var bizId = sessionData.SelectData.SelectedBiz;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            if (!characterData.BizIDs.Contains(bizId))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoBusiness), 3000);
                return;
            }

            var biz = BusinessManager.BizList[bizId];
            
            var product = biz.Products.FirstOrDefault(p => p.Name == name);
            
            if (product == null)
                return;

            if (product.Ordered)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyOrderedThis), 3000);
                return;
            }
            
            var productsData = BusinessManager.BusProductsData[product.Name];
            
            if (value < CountMinOrder(biz.Type))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Enter10To, productsData.MaxCount - product.Lefts), 5000);
                return;
            }
                        
            if (product.Lefts + value > productsData.MaxCount)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxCol, productsData.MaxCount - product.Lefts), 5000);
                return;
            }
            
            var price = productsData.OtherPrice > 0 ? productsData.OtherPrice : productsData.Price;
            price = value * price;
            
            if (!Bank.Change(characterData.Bank, -price))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoBankMoney), 3000);
                return;
            }
            
            biz.Zatratq += price;
            GameLog.Money($"bank({characterData.Bank})", $"server", price, "bizOrder");
            var order = new Order(product.Name, value);
            
            var random = new Random();
            do
            {
                order.UID = random.Next(000000, 999999);
            } while (BusinessManager.Orders.ContainsKey(order.UID));
            
            product.Ordered = true;
            biz.Orders.Add(order);
            BusinessManager.Orders.TryAdd(order.UID, biz.ID);
            
            Trigger.ClientEvent(player, "client.phone.business.cAddOrder", order.UID, order.Name, order.Amount);
            
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OrderedAmount, product.Name, value, order.UID), 3000);
            Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.ZakazNumber, order.UID));
        }

        public static void MaxProducts(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var bizId = sessionData.SelectData.SelectedBiz;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            if (!characterData.BizIDs.Contains(bizId))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoBusiness), 3000);
                return;
            }
            
            var biz = BusinessManager.BizList[bizId];
            
            var message = "Вы действительно хотите купить:<br/>";

            var products = biz.Products
                .Where(p => !p.Ordered)
                .ToList();
            
            var whCount = 0;
            var whCost = 0;
            var success = false;

            foreach (var product in products)
            {
                if (!BusinessManager.BusProductsData.ContainsKey(product.Name))
                    continue;
                
                var productsData = BusinessManager.BusProductsData[product.Name];
                var itemPrice = productsData.OtherPrice > 0 ? productsData.OtherPrice : productsData.Price;
                
                var count = 0;
                
                if (biz.Type >= 2 && biz.Type <= 5) 
                    count = 3;
                else
                {
                    if (product.Lefts >= productsData.MaxCount) continue;
                    count = productsData.MaxCount - product.Lefts;
                }

                if (count > 0)
                {
                    whCount += count;
                    whCost += (count * itemPrice);
                    
                    message += LangFunc.GetText(LangType.Ru, DataName.AmountCosts, product.Name, count, (count * itemPrice));
                    success = true;
                }
            }
            
            if (!success)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoTovarZakaz), 3000);
                return;
            }
            
            var bankBalance = Bank.GetBalance(characterData.Bank);
            if ((bankBalance - whCost) < 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoBankMoney), 3000);
                return;
            }
            
            Trigger.ClientEvent(player, "openDialog", "BIZ_CONFIRM_BUY_ORDERS", $"Вы желаете заказать товаров в количестве <font style='color: red'>{whCount}</font> на сумму <font style='color: green'>{whCost}$</font>.<br/><br/>{message}");
        }
        public static void MaxProductsConfirm(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null)
                return;

            var bizId = sessionData.SelectData.SelectedBiz;

            var characterData = player.GetCharacterData();
            if (characterData == null)
                return;

            if (!characterData.BizIDs.Contains(bizId))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                    LangFunc.GetText(LangType.Ru, DataName.YouHaveNoBusiness), 3000);
                return;
            }

            var biz = BusinessManager.BizList[bizId];
            var random = new Random();

            foreach (var product in biz.Products)
            {
                if (product.Ordered)
                    continue;
                
                var productsData = BusinessManager.BusProductsData[product.Name];
                var itemPrice = productsData.OtherPrice > 0 ? productsData.OtherPrice : productsData.Price;
                
                var count = 0;
                
                if (biz.Type >= 2 && biz.Type <= 5) 
                    count = 3;
                else
                {
                    if (product.Lefts >= productsData.MaxCount) continue;
                    count = productsData.MaxCount - product.Lefts;
                }

                if (count > 0)
                {
                    var cost = count * itemPrice;
                    
                    if (!Bank.Change(characterData.Bank, -cost))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoBankMoney), 3000);
                        return;
                    }
                    biz.Zatratq += cost;
                    
                    GameLog.Money($"bank({characterData.Bank})", $"server", cost, "bizOrder");
                    var order = new Order(product.Name, count);
                    do
                    {
                        order.UID = random.Next(000000, 999999);
                    } while (BusinessManager.Orders.ContainsKey(order.UID));
                    
                    product.Ordered = true;
                    biz.Orders.Add(order);
                    BusinessManager.Orders.TryAdd(order.UID, biz.ID);
                }
            }
            
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AllTovarSucOrder), 3000);

            GetData(player, bizId);

        }
        public static void CancelOrder(ExtPlayer player, int uid)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var bizId = sessionData.SelectData.SelectedBiz;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            if (!characterData.BizIDs.Contains(bizId))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoBusiness), 3000);
                return;
            }

            var biz = BusinessManager.BizList[bizId];

            var order = biz.Orders.FirstOrDefault(o => o.UID == uid);

            if (order == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не заказывали этот товар", 3000);
                return;
            }
            
            var product = biz.Products.FirstOrDefault(p => p.Name == order.Name);
            if (product == null)
                return;

            var productsData = BusinessManager.BusProductsData[order.Name];

            if (order.Taked)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете отменить заказ, пока его доставляют", 3000);
                return;
            }
            
            product.Ordered = false;
            biz.Orders.Remove(order);
            if (BusinessManager.Orders.ContainsKey(order.UID))
                BusinessManager.Orders.TryRemove(order.UID, out _);
            
            Trigger.ClientEvent(player, "client.phone.business.successCancel", order.UID);
            
            var price = productsData.OtherPrice > 0 ? productsData.OtherPrice : productsData.Price;
            price = order.Amount * price;

            Bank.Change(characterData.Bank, price);
            //Wallet.Change(player, price);
            GameLog.Money($"server", $"player({characterData.UUID})", price, $"orderCancel");
            biz.Zatratq -= price;
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы отменили заказ на {order.Name}", 3000);
        }


        public static void ExtraCharge(ExtPlayer player, string name, int value)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var bizId = sessionData.SelectData.SelectedBiz;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            if (!characterData.BizIDs.Contains(bizId))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoBusiness), 3000);
                return;
            }
            
            if (name == "Лотерейный билет" || name == "Расходники")
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantLotteryPriceChange), 3000);
                return;
            }
            
            var biz = BusinessManager.BizList[bizId];
            
            var product = biz.Products.FirstOrDefault(p => p.Name == name);
            
            if (product == null)
                return;

            var productsData = BusinessManager.BusProductsData[product.Name];

            var minPrice = GetPriceMin(product.Name, biz.Type, productsData.Price);
            var maxPrice = GetPriceMax(product.Name, biz.Type, productsData.Price);
            
            if (value < minPrice || value > maxPrice)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantSetThisPrice), 3000);
                return;
            }
            
            product.Price = value;
            
            var ch = (biz.Type == 7 || biz.Type == 11 || biz.Type == 12 || product.Name == "Татуировки" || product.Name == "Парики") ? "%" : "$";
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ThenThisCosts, product.Name, product.Price, ch), 3000);
            
            if (product.Name == "Бензин") 
                biz.UpdateLabel();
            
            Trigger.ClientEvent(player, "client.phone.business.cExtraCharge", product.Name, value);
        }

        public static void OnSell(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var bizId = sessionData.SelectData.SelectedBiz;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            if (!characterData.BizIDs.Contains(bizId))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoBusiness), 3000);
                return;
            }
            
            var biz = BusinessManager.BizList[bizId];

            var price = Wallet.GetPriceToVip(player, biz.SellPrice);
            
            Trigger.ClientEvent(player, "openDialog", "BIZ_SELL_TOGOV", $"Вы действительно хотите продать недвижимость за ${MoneySystem.Wallet.Format(price)}?");   
        }

        public static void OnSellConfirm(ExtPlayer player)
        {
            var accountData = player.GetAccountData();
            if (accountData == null) return;
            
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var bizId = sessionData.SelectData.SelectedBiz;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            if (!characterData.BizIDs.Contains(bizId))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoBusiness), 3000);
                return;
            }
            
            var biz = BusinessManager.BizList[bizId];
            
            int price = Wallet.GetPriceToVip(player, biz.SellPrice);

            MoneySystem.Wallet.Change(player, price);
            GameLog.Money($"server", $"player({characterData.UUID})", price, $"sellBiz({biz.ID})");
            characterData.BizIDs.Remove(bizId);
            biz.ClearOwner();
            
            Houses.Rieltagency.Repository.OnPayDay(new List<Houses.House>(), new List<Business>()
            {
                biz
            });
            //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы продали бизнес государству за {MoneySystem.Wallet.Format(price)}$", 3000);
            Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Bank, LangFunc.GetText(LangType.Ru, DataName.SellBizGos, MoneySystem.Wallet.Format(price)), DateTime.Now);
        }

    }
}