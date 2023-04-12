using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using GTANetworkAPI;
using GTANetworkMethods;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Handles;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Businesses
{
    public class Repository
    {
        public static void Open(ExtPlayer player)
        {
            
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            
            if (characterData.BizIDs.Count == 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoBusiness), 3000);
                return;
            }

            var id = characterData.BizIDs[0];
            var biz = BusinessManager.BizList[id];

            var whCount = 0;
            var whMaxCount = 0;
            var whPriceMaxCount = 0;
            var stats = new Dictionary<string, object>()
            {
                { "tax", Convert.ToInt32(biz.SellPrice / 100 * biz.Tax) },
                { "cash", MoneySystem.Bank.GetBalance(biz.BankID) },
                { "pribil", biz.Pribil },
                { "zatratq", biz.Zatratq },
            };

            var stocks = new List<Dictionary<string, object>>();
            foreach (var product in biz.Products)
            {
                var productsData = BusinessManager.BusProductsData[product.Name];
                int itemPrice = productsData.OtherPrice > 0 ? productsData.OtherPrice : productsData.Price;
                
                stocks.Add(new Dictionary<string, object>()
                {
                    { "Name", product.Name },
                    { "Count", product.Lefts },
                    { "MaxCount", productsData.MaxCount },
                    { "Price", product.Price },
                    { "OrderPrice", productsData.OtherPrice },
                    { "DefaultPrice", productsData.Price },
                    { "MinPrice", GetPriceMin(product.Name, biz.Type, productsData.Price) },
                    { "MaxPrice", GetPriceMax(product.Name, biz.Type, productsData.Price) },
                });

                whCount += product.Lefts;
                whMaxCount += productsData.MaxCount;
                whPriceMaxCount += (productsData.MaxCount - product.Lefts) * itemPrice;
            }
            
            //
            
            stats.Add("whCount", whCount);
            stats.Add("whMaxCount", whMaxCount);
            
            //

            foreach (var order in biz.Orders)
            {
                
            }

            GetHistory(player);
            
            Trigger.ClientEvent(player, "client.businessmanage.open", JsonConvert.SerializeObject(stats), JsonConvert.SerializeObject(stocks), JsonConvert.SerializeObject(biz.Orders));
            
            
        }
        public static void GetHistory(ExtPlayer player)
        {
            var historyList = new List<List<object>>();
            historyList.Add(new List<object>
            {
                1, 0, "2021-12-22 18:23:57", 1018322, "Монтировка", 45
            });
            historyList.Add(new List<object>
            {
                2, 0, "2021-12-22 18:23:57", 1018322, "Пиво", 45
            });
            historyList.Add(new List<object>
            {
                3, 0, "2021-12-22 18:23:57", 1018322, "Пицца", 45
            });
            historyList.Add(new List<object>
            {
                4, 0, "2021-12-21 18:23:57", 1018322, "Круглая", 45
            });
            historyList.Add(new List<object>
            {
                5, 0, "2021-12-22 18:23:57", 1018321, "Крыса", 45
            });
            
            Trigger.ClientEvent(player, "client.businessmanage.sethistory", JsonConvert.SerializeObject(historyList));
        }
        /*public static async Task GetProduct(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            
            if (characterData.BizIDs.Count == 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoBusiness), 3000);
                return;
            }

            var id = characterData.BizIDs[0];
            var biz = BusinessManager.BizList[id];
            
            await using var db = new ServerBD("MainDB");
            
            var history = await db.Businesshistory
                .Where(b => b.Bizid == id)
        }*/
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
    }
}