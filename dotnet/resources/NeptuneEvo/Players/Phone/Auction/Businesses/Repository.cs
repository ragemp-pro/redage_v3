using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using LinqToDB;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.Players.Phone.Auction.Models;
using NeptuneEvo.Players.Phone.Messages.Models;
using Newtonsoft.Json;

namespace NeptuneEvo.Players.Phone.Auction.Businesses
{
    public class Repository
    {
        public static void SetAuction(Business biz)
        {
            //Bank.Accounts[biz.BankID].Balance = 0;
            biz.ClearOwner();
            biz.IsAuction = true;
        }

        public static void SetOwner(AuctionData auctionData)
        {
            var uuid = -1;
            var playerName = String.Empty;

            if (Main.PlayerNames.ContainsKey(auctionData.LastBetUUID))
            {
                uuid = auctionData.LastBetUUID;
                playerName = Main.PlayerNames[uuid];   
                Messages.Repository.AddSystemMessageToUuid(auctionData.LastBetUUID, (int) DefaultNumber.Auction, $"Ваша ставка победила в аукционе. Лот {auctionData.Title} добавлен в ваше имущество.", DateTime.Now);
                Messages.Repository.AddSystemMessageToUuid(auctionData.CreateUUID, (int) DefaultNumber.Auction, $"Ваша ставка победила в аукционе. Лот {auctionData.Title} добавлен в ваше имущество.", DateTime.Now);
                Auction.Repository.ReturnMoney(auctionData.CreateUUID, auctionData.LastPrice, auctionData.Id, auctionData.Type, $"Ваш лот {auctionData.Title} приобретен за {auctionData.LastPrice}. Деньги зачислены на счет.");
            }
            else if (Main.PlayerNames.ContainsKey(auctionData.CreateUUID))
            {
                uuid = auctionData.CreateUUID;
                playerName = Main.PlayerNames[uuid];
                Messages.Repository.AddSystemMessageToUuid(uuid, (int) DefaultNumber.Auction, $"Аукцион отменен. Лот {auctionData.Title} возвращён.", DateTime.Now);
            }
            
            if (uuid != -1 && playerName != String.Empty)
            {
                var biz = BusinessManager.BizList[auctionData.ElementId];
                
                biz.IsAuction = false;
                
                foreach (var product in biz.Products) 
                    product.Lefts = BusinessManager.BusProductsData[product.Name].MaxCount/10;
                
                var orders = new List<Order>();
                
                foreach (var order in biz.Orders)
                {
                    if (order.Taked) 
                        orders.Add(order);
                    else 
                        BusinessManager.Orders.TryRemove(order.UID, out _);
                }
                
                biz.Orders = orders;
                biz.SetOwner(playerName);
                
                var bizBalance = MoneySystem.Bank.Accounts[biz.BankID];
                bizBalance.Balance = Convert.ToInt32(biz.SellPrice / 100f * biz.Tax) * 72;
                bizBalance.IsSave = true;
                
                var player = Main.GetPlayerByUUID(uuid);
                var characterData = player.GetCharacterData();
                if (characterData != null)
                {
                    characterData.BizIDs.Add(auctionData.ElementId);
                    Character.Save.Repository.SaveBiz(player);
                }
                else
                {
                    Trigger.SetTask(async () =>
                    {
                        try
                        {
                            await using var db = new ServerBD("MainDB");//В отдельном потоке
                        
                            var character = await db.Characters
                                .Select(v => new { v.Uuid, v.Biz })
                                .Where(v => v.Uuid == uuid)
                                .FirstOrDefaultAsync();

                            if (character != null)
                            {
                                var bizIDs = JsonConvert.DeserializeObject<List<int>>(character.Biz);
                            
                                bizIDs.Add(auctionData.ElementId);

                                await db.Characters
                                    .Where(v => v.Uuid == uuid)
                                    .Set(v => v.Biz, JsonConvert.SerializeObject(bizIDs))
                                    .UpdateAsync();

                            }
                        }
                        catch (Exception e)
                        {
                            Debugs.Repository.Exception(e);
                        }
                    });
                }
            }
        }
    }
}