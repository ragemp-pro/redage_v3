using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using LinqToDB;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.Players.Phone.Auction.Models;
using NeptuneEvo.Players.Phone.Messages.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Players.Phone.Auction
{
    public class Repository
    {
        private static List<AuctionData> AuctionsData = new List<AuctionData>();
        
        private static List<ExtPlayer> Players = new List<ExtPlayer>();

        public static void OnClose(ExtPlayer player)
        {
            if (!player.IsCharacterData())
                return;

            if (Players.Contains(player))
                Players.Remove(player);
        }

        public static bool IsElement(AuctionType type, int id) =>
            AuctionsData.Any(a => a.Type == type && a.ElementId == id);

        private static List<object> GetData(AuctionData auctionData)
        {
            var auction = new List<object>();
            
            auction.Add(auctionData.Id);
            auction.Add(auctionData.Type);
            auction.Add(auctionData.BetCount);
            auction.Add(auctionData.Title);
            auction.Add(auctionData.Text);
            auction.Add(auctionData.Image);
            auction.Add(auctionData.CreatePrice);
            auction.Add(auctionData.LastPrice);
            var name = "";
            
            if (Main.PlayerNames.ContainsKey(auctionData.CreateUUID))
                name = Main.PlayerNames[auctionData.CreateUUID];
            
            auction.Add(name);
            auction.Add(auctionData.CreateUUID);
            auction.Add(auctionData.Time);

            var betsList = new List<object>();

            foreach (var betData in auctionData.BetsData)
            {
                var bets = new List<object>();
                bets.Add(betData.Name);
                bets.Add(betData.Bet);
                betsList.Add(bets);
            }
            auction.Add(betsList);
            
            auction.Add(auctionData.LastBetUUID);
            
            return auction;
        }
        
        public static void Init(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            var listAuctions = AuctionsData
                .Where(a => !a.IsEnd)
                .ToList();

            var myListAuctions = listAuctions
                .Where(a => a.CreateUUID == characterData.UUID)
                .ToList();
            
            var myAuctions = new List<List<object>>();

            foreach (var auctionData in myListAuctions)
            {
                listAuctions.Remove(auctionData);
                
                myAuctions.Add(GetData(auctionData));
            }
            
            //
            
            var partListAuctions = listAuctions
                .Where(a => a.BetsData.Any(b => b.UUID == characterData.UUID))
                .ToList();

            //

            foreach (var auctionData in partListAuctions)
            {
                listAuctions.Remove(auctionData);
                myAuctions.Add(GetData(auctionData));
            }
            
            //

            var auctions = new List<List<object>>();
            foreach (var auctionData in listAuctions)
            {
                auctions.Add(GetData(auctionData));
            }
            
            Trigger.ClientEvent(player, "client.phone.auction.init", JsonConvert.SerializeObject(myAuctions), JsonConvert.SerializeObject(auctions));
            
            if (!Players.Contains(player))
                Players.Add(player);
        }

        public static void GetItem(ExtPlayer player, AuctionType type)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            var items = new List<List<object>>();
            if (type == AuctionType.House)
            {
                var house = Houses.HouseManager.GetHouse(player, true);
                if (house != null)
                {
                    
                    items.Add(new List<object>
                    {
                        house.ID,
                        $"Дом #{house.ID}",
                    });
                }
            }
            else if (type == AuctionType.Biz)
            {
                foreach (var bizId in characterData.BizIDs)
                {
                    if (!BusinessManager.BizList.ContainsKey(bizId))
                        continue;
                    
                    var biz = BusinessManager.BizList[bizId];
                    
                    items.Add(new List<object>
                    {
                        biz.ID,
                        $"{BusinessManager.BusinessTypeNames[biz.Type]} #{biz.ID}",
                    });
                }
            }
            else if (type == AuctionType.Vehicle)
            {
                var vehiclesNumber = VehicleManager.GetVehiclesCarAndAirNumberToPlayer(player.Name);
                
                foreach (var number in vehiclesNumber)
                {
                    var vehicleData = VehicleManager.GetVehicleToNumber(number);
                    if (vehicleData == null) 
                        continue;
                    if (Ticket.IsVehicleTickets(vehicleData.SqlId))
                        continue;
                    items.Add(new List<object>
                    {
                        vehicleData.SqlId,
                        $"{vehicleData.Model.ToUpper()} - {number}",
                    });
                }
            }

            Trigger.ClientEvent(player, "client.phone.auction.setItem", JsonConvert.SerializeObject(items));
            
        }

        public static bool IsBet(int uuid, AuctionType type) => AuctionsData.Any(a => a.Type == type && (a.CreateUUID == uuid || a.BetsData.Any(b => b.UUID == uuid)));
        public static void OnCreate(ExtPlayer player, AuctionType type, int elementId, string text, string image, int price)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            if (!FunctionsAccess.IsWorking("phoneauction"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                return;
            }
            
            var title = "";
            
            if (type == AuctionType.House)
            {
                var house = Houses.HouseManager.GetHouse(player, true);
                if (house == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoHome), 3000);
                    return;
                }

                if (characterData.InsideGarageID != -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны выйти из гаража", 3000);
                    return;
                }
                title = $"Дом #{house.ID}";
                House.Repository.SetAuction(house);
                
            }
            else if (type == AuctionType.Biz)
            {
                if (!characterData.BizIDs.Contains(elementId))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoBusiness), 3000);
                    return;
                }

                var biz = BusinessManager.BizList[elementId];
                title = $"{BusinessManager.BusinessTypeNames[biz.Type]} #{biz.ID}";
                characterData.BizIDs.Remove(elementId);
                Businesses.Repository.SetAuction(biz);
            }
            else if (type == AuctionType.Vehicle)
            {
                var vehicleData = VehicleManager.Vehicles.Values
                    .FirstOrDefault(v => v.SqlId == elementId && v.Holder == player.Name);

                if (vehicleData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarBoleeNetu), 3000);
                    return;
                }

                image = vehicleData.Model.ToLower();
                title = $"{vehicleData.Model.ToUpper()} - {vehicleData.Number}";
                vehicleData.Holder = $"{player.Name}_Auction";
                VehicleManager.SaveHolder(vehicleData.Number);
                var house = Houses.HouseManager.GetHouse(player);
                var garage = house?.GetGarageData();
                garage?.DeleteCar(vehicleData.Number, isRevive: true);
            } 
            else if (type == AuctionType.Other)
            {
                var locationName = $"char_{characterData.UUID}";

                if (Chars.Repository.ItemsData.ContainsKey(locationName) && Chars.Repository.ItemsData[locationName].ContainsKey("inventory"))
                {
                    var index = Chars.Repository.ItemsData[locationName]["inventory"]
                        .FirstOrDefault(i => i.Value.SqlId == elementId).Key;
                    
                    var itemData = Chars.Repository.GetItemData(player, "inventory", index);
                    if (itemData.ItemId == ItemId.Debug)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Предмет не найден", 3000);
                        return;
                    }
                    Chars.Repository.SetItemData(player, "inventory", index, new InventoryItemData(), true);
                    Chars.Repository.UpdateSqlItemData("auction", "auction", 0, itemData);
                }
            }

            Insert(player, type, elementId, title, text, image, price);
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно создали лот", 3000);
        }
        
        private static void Insert(ExtPlayer player, AuctionType type, int elementId, string title, string text, string image, int price)
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    var characterData = player.GetCharacterData();
                    if (characterData == null) 
                        return;
                
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    var time = Main.ServerNumber == 0 ? DateTime.Now.AddMinutes(3) : DateTime.Now.AddHours(23);
                
                    var autoId = await db.InsertWithInt32IdentityAsync(new Auctions
                    {
                        Type = (int) type,
                        ElementId = elementId,
                        Time = time,
                        CreateUUID = characterData.UUID,
                        BetCount = 0,
                        LastBetUUID = 0,
                        Title = title,
                        Text = text,
                        Image = image,
                        CreatePrice = price,
                        LastPrice = price,
                        BetsData = "[]",
                        IsEnd = false
                    });

                    GameLog.Money($"player({characterData.UUID})", $"server", price, $"auction [#{autoId}]: Создание лота ({type.ToString()})");
                    
                    var auctionData = new AuctionData
                    {
                        Id = autoId,
                        Type = type,
                        ElementId = elementId,
                        Time = time,
                        CreateUUID = characterData.UUID,
                        BetCount = 0,
                        LastBetUUID = 0,
                        Title = title,
                        Text = text,
                        Image = image,
                        CreatePrice = price,
                        LastPrice = price,
                        BetsData = new List<AuctionBetData>()
                    };
                    
                    AuctionsData.Add(auctionData);
                    
                    Trigger.SetMainTask(() =>
                    {
                        Trigger.ClientEvent(player, "client.phone.auction.addMyItem", JsonConvert.SerializeObject(GetData(auctionData)));
                        SendPlayer(JsonConvert.SerializeObject(GetData(auctionData)), "addItem", characterData.UUID); 
                    });
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
           
        }
        
        public static void Load()
        {
            try
            {
                using var db = new ServerBD("MainDB");

                var auctions = db.Auctions
                    .Where(a => a.IsEnd == false)
                    .ToList();
            
                foreach (var auction in auctions)
                {
                    AuctionsData.Add(new AuctionData
                    {
                        Id = auction.AutoId,
                        Type = (AuctionType)auction.Type,
                        ElementId = auction.ElementId,
                        Time = auction.Time,
                        CreateUUID = auction.CreateUUID,
                        BetCount = auction.BetCount,
                        LastBetUUID = auction.LastBetUUID,
                        Title = auction.Title,
                        Text = auction.Text,
                        Image = auction.Image,
                        CreatePrice = auction.CreatePrice,
                        LastPrice = auction.LastPrice,
                        BetsData = JsonConvert.DeserializeObject<List<AuctionBetData>>(auction.BetsData)
                    });
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        private static void SendPlayer(object data, string eventName = "updateItem", int unUuid = 0)
        {
            foreach (var foreachPlayer in Players)
            {
                var foreachCharacterData = foreachPlayer.GetCharacterData();
                if (foreachCharacterData == null)
                    continue;
            
                if (foreachCharacterData.UUID == unUuid)
                    continue;

                Trigger.ClientEvent(foreachPlayer, $"client.phone.auction.{eventName}", data);
            }
        }

        public static bool IsSavingAuctions() => AuctionsData.Any(a => a.IsSave);
        
        public static async Task SaveAuctions(ServerBD db)
        {
            try
            {
                var auctionsData = AuctionsData
                    .Where(a => a.IsSave)
                    .ToList();
            
                foreach (var auctionData in auctionsData)
                {
                    auctionData.IsSave = false;
                    
                    await db.Auctions
                        .Where(a => a.AutoId == auctionData.Id)
                        .Set(a => a.BetCount, auctionData.BetCount)
                        .Set(a => a.LastBetUUID, auctionData.LastBetUUID)
                        .Set(a => a.LastPrice, auctionData.LastPrice)
                        .Set(a => a.BetsData, JsonConvert.SerializeObject(auctionData.BetsData))
                        .Set(a => a.IsEnd, auctionData.IsEnd)
                        .UpdateAsync();
            
                    if (auctionData.IsEnd && AuctionsData.Contains(auctionData))
                        AuctionsData.Remove(auctionData);
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        private static void End(AuctionData auctionData)
        {
            SendPlayer(auctionData.Id, "delItem");

            auctionData.IsSave = true;
            auctionData.IsEnd = true;
            
            Messages.Repository.AddSystemMessageToUuid(auctionData.CreateUUID, (int) DefaultNumber.Auction, $"Аукцион на {auctionData.Title} закончен.", DateTime.Now);
            
            if (auctionData.Type == AuctionType.House)
                House.Repository.SetOwner(auctionData);
            
            else if (auctionData.Type == AuctionType.Biz)
                Businesses.Repository.SetOwner(auctionData);
                    
            else if (auctionData.Type == AuctionType.Vehicle)
                Vehicles.Repository.SetOwner(auctionData);
            
            //var player = Main.GetPlayerByUUID(auctionData.LastBetUUID);
            
            
        }
        public static void GetEnd()
        {
            var auctionsData = AuctionsData
                .Where(a => DateTime.Now >= a.Time && !a.IsEnd)
                .ToList();

            foreach (var auctionData in auctionsData)
                End(auctionData);
        }

        public static void EndToId(int id)
        {
            var auctionData = AuctionsData
                .FirstOrDefault(a => a.Id == id && !a.IsEnd);

            if (auctionData != null)
                End(auctionData);
        }
        public static void OnBet(ExtPlayer player, int id, int price)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            
            if (!FunctionsAccess.IsWorking("phoneauction"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                return;
            }
            
            var auctionData = AuctionsData.FirstOrDefault(a => a.Id == id);

            if (auctionData == null)
            {
                Init(player);
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Лот не найден...", 3000);
                return;
            }
            
            if (auctionData.CreateUUID == characterData.UUID)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете сделать ставку на свой лот.", 3000);
                return;
            }

            if (auctionData.LastBetUUID == characterData.UUID)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Последняя ставка и так ваша.", 3000);
                return;
            }
            if (auctionData.LastPrice >= price)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ставка не может быть меньше, чем текущая.", 3000);
                return;
            }


            if (auctionData.Type == AuctionType.House)
            {
                var house = Houses.HouseManager.GetHouse(player, true);
                if (house != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас уже есть дом.", 3000);
                    return;
                }

            }
            else if (auctionData.Type == AuctionType.Biz)
            {
                if (characterData.BizIDs.Count > 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас уже есть бизнес.", 3000);
                    return;
                }
            }

            var priceAndPercent = price + Convert.ToInt32(price / 100);
            if (!Bank.Change(characterData.Bank, -priceAndPercent))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoBankMoney), 3000);
                return;
            }
            
            GameLog.Money($"player({characterData.UUID})", $"auction(Bet)", priceAndPercent, $"auction [#{auctionData.Id}]: Ставка ({auctionData.Type.ToString()})");
            ReturnMoney(auctionData.LastBetUUID, auctionData.LastPrice, auctionData.Id, auctionData.Type, "Вашу ставку перебили!");

            var isAddMyList = auctionData.BetsData.Any(b => b.UUID == characterData.UUID);
            
            auctionData.LastBetUUID = characterData.UUID;
            auctionData.LastPrice = price;
            if (!isAddMyList)
                auctionData.BetCount++;
            
            auctionData.BetsData.Add(new AuctionBetData
            {
                Bet = price,
                UUID = characterData.UUID,
                Name = player.Name
            });
            auctionData.IsSave = true;
            
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы сделали ставку.", 3000);
            //Update(auctionData);
            if (!isAddMyList)
            {
                Trigger.ClientEvent(player, "client.phone.auction.delItem", auctionData.Id, true);
                Trigger.ClientEvent(player, "client.phone.auction.addMyItem", JsonConvert.SerializeObject(GetData(auctionData)));
                SendPlayer(JsonConvert.SerializeObject(GetData(auctionData)), "updateItem", characterData.UUID);
            }
            else
                SendPlayer(JsonConvert.SerializeObject(GetData(auctionData)), "updateItem");
        }

        public static void ReturnMoney(int uuid, int price, int id, AuctionType type, string message)
        {
            GameLog.Money($"player({uuid})", $"server", price, $"auction [#{id}]: {message} ({type.ToString()})");
            
            var player = Main.GetPlayerByUUID(uuid);
            var characterData = player.GetCharacterData();
            if (characterData != null)
            {
                var accountId = characterData.Bank;
                if (Bank.Accounts.ContainsKey(accountId))
                    Bank.Change(characterData.Bank, price);
                else
                    Wallet.Change(player, price);
                
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, message, 3000);
            }
            else
            {
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке
                        
                        var character = await db.Characters
                            .Select(v => new { v.Uuid, v.Money, v.Bank })
                            .Where(v => v.Uuid == uuid)
                            .FirstOrDefaultAsync();

                        if (character != null)
                        {
                            var accountId = Convert.ToInt32(character.Bank);
                            if (Bank.Accounts.ContainsKey(accountId))
                            {
                                Trigger.SetMainTask(() => Bank.Change(accountId, price));
                            }
                            else 
                            {
                                await db.Characters
                                    .Where(v => v.Uuid == uuid)
                                    .Set(v => v.Money, (character.Money + price))
                                    .UpdateAsync();
                            }
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