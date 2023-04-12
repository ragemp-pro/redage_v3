using System;
using System.Linq;
using NeptuneEvo.Houses;
using NeptuneEvo.Players.Phone.Auction.Models;
using NeptuneEvo.Players.Phone.Messages.Models;

namespace NeptuneEvo.Players.Phone.Auction.House
{
    public class Repository
    {
        public static void SetAuction(Houses.House house)
        {
            house.IsAuction = true;
            house.RemoveAllPlayers();
            house.ClearOwner(isClearUpgraded: false);
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
                var house = HouseManager.Houses.FirstOrDefault(h => h.ID == auctionData.ElementId);

                if (house != null)
                {
                    house.SetOwner(playerName);

                    var houseBalance = MoneySystem.Bank.Accounts[house.BankID];
                    houseBalance.Balance = Convert.ToInt32(house.Price / 100f * HouseManager.HouseTax) * 72;
                    houseBalance.IsSave = true;
                }
                //var player = Main.GetPlayerByUUID(uuid);
            }
        }
        
    }
}