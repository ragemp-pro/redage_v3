using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.Players;
using NeptuneEvo.Quests;
using NeptuneEvo.Quests.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Houses.Rieltagency
{
    public class Repository
    {
        private static List<ExtPlayer> OpenInterfacePlayer = new List<ExtPlayer>();
        private static int BuyPrice = 500;


        public static void OnResourceStart()
        {
            Main.CreateBlip(new Main.BlipData(374, LangFunc.GetText(LangType.Ru, DataName.Rieltorskoye), new Vector3(-710.49, 267.8656, 83.14731), 2, true));
            PedSystem.Repository.CreateQuest("a_m_y_business_02", new Vector3(-710.49, 267.8656, 83.14731), -92.04525f, title: "~y~NPC~w~ Илон Таск\nРиэлтор", colShapeEnums: ColShapeEnums.Rieltagency);
        }
        
        
        [Interaction(ColShapeEnums.Rieltagency)]
        public void OnRieltagency(ExtPlayer player, int index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null)
                return;

            var characterData = player.GetCharacterData();
            if (characterData == null)
                return;

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


            player.SelectQuest(new PlayerQuestModel("npc_rieltor", 0, 0, false, DateTime.Now));
            Trigger.ClientEvent(player, "client.quest.open", index, "npc_rieltor", 0, 0, 0);
        }
        
        
        public static void OnOpen(ExtPlayer player)
        {
            if (!player.IsCharacterData())
                return;
            
            var shapeData = CustomColShape.GetData(player, ColShapeEnums.Rieltagency);
            if (shapeData == null)
                return;
            
            var houses = HouseManager.Houses
                .Where(h => h.Owner == string.Empty && !h.IsAuction)
                .Where(h => h.Type != 7)
                .Where(h => h.Price > 0)
                .ToList();

            var housesData = new List<List<object>>();
            foreach (var house in houses)
            {
                var houseData = GetHouseData(house);
                if (houseData != null) 
                    housesData.Add(houseData);
            }
            
            var businesses = BusinessManager.BizList.Values
                .Where(b => !b.IsOwner() && !b.IsAuction)
                .Where(b => b.SellPrice > 0)
                .ToList();

            var businessesData = new List<List<object>>();
            foreach (var business in businesses)
            {
                var businessData = GetBusinessData(business);
                if (businessData != null) 
                    businessesData.Add(businessData);
            }
            
            OpenInterfacePlayer.Add(player);

            BattlePass.Repository.UpdateReward(player, 147);
            
            Trigger.ClientEvent(player, "client.rieltagency.open", 
                BuyPrice,
                JsonConvert.SerializeObject(housesData), houses.Count, 
                JsonConvert.SerializeObject(businessesData), BusinessManager.BizList.Count);
        }

        [Interaction(ColShapeEnums.Rieltagency, Out: true)]
        public static void OutRieltagency(ExtPlayer player)
        {
            if (OnClose(player))
                Trigger.ClientEvent(player, "client.rieltagency.close", false);
        }
        
        public static bool OnClose(ExtPlayer player)
        {
            if (!player.IsCharacterData())
                return false;

            if (OpenInterfacePlayer.Contains(player))
            {
                OpenInterfacePlayer.Remove(player);
                
                return true;
            }

            return false;
        }
        
        private static List<object> GetHouseData(House house)
        {
            if (house == null)
                return null;
            
            var garage = house.GetGarageData();

            var cars = 0;

            if (garage != null && GarageManager.GarageTypes.ContainsKey(garage.Type))
                cars = GarageManager.GarageTypes[garage.Type].MaxCars;
            
            return new List<object>()
            {
                house.ID,
                house.Type,
                house.Price,
                Convert.ToInt32(house.Price / 100 * 0.026),
                cars,
            };
        }
        private static List<object> GetBusinessData(Business business)
        {
            if (business == null)
                return null;
            
            return new List<object>()
            {
                business.ID,
                business.Type,
                business.SellPrice,
                Convert.ToInt32(business.SellPrice / 100 * business.Tax),
            };
        }
        
        
        public static void OnPayDay(List<House> houses, List<Business> businesses)
        {
            if (OpenInterfacePlayer.Count == 0)
                return;
            
            var housesData = new List<List<object>>();
            foreach (var house in houses)
            {
                if (house.Type == 7)
                    continue;
                
                var houseData = GetHouseData(house);
                if (houseData != null) 
                    housesData.Add(houseData);
            }
            
            var businessesData = new List<List<object>>();
            foreach (var business in businesses)
            {
                var businessData = GetBusinessData(business);
                if (businessData != null) 
                    businessesData.Add(businessData);
            }

            foreach (var foreachPlayer in OpenInterfacePlayer)
            {
                if (foreachPlayer.IsCharacterData())
                    Trigger.ClientEvent(foreachPlayer, "client.rieltagency.addRange", 
                        JsonConvert.SerializeObject(housesData), 
                        JsonConvert.SerializeObject(businessesData));
            }
        }

        public static void OnBuy(ExtPlayer player, int id, int type)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return;

            var price = -1;

            if (type == 0)
            {
                var house = HouseManager.Houses
                    .FirstOrDefault(h => h.ID == id);
                
                if (house == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NotFindHome), 3000);
                    return;
                }
                
                if (house.Owner != String.Empty)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.HomeAlreadyBought), 3000);
                    return;
                }

                price = Convert.ToInt32(BuyPrice * (house.Type + 1));

                if (UpdateData.CanIChange(player, price, true) != 255) return;
                    
                Trigger.ClientEvent(player, "client.rieltagency.addBlip", 374, 82, id, house.Position.X, house.Position.Y, LangFunc.GetText(LangType.Ru, DataName.Rieltorskoye));
            }
            else
            {
                if (!BusinessManager.BizList.ContainsKey(id))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NotFindBiz), 3000);
                    return;
                }

                var business = BusinessManager.BizList[id];
                if (business.IsOwner())
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BizAlreadyBought), 3000);
                    return;
                }
                
                price = BuyPrice;
                
                if (UpdateData.CanIChange(player, price, true) != 255) return;
                
                Trigger.ClientEvent(player, "client.rieltagency.addBlip", 464, 82, id, business.EnterPoint.X, business.EnterPoint.Y, LangFunc.GetText(LangType.Ru, DataName.Rieltorskoye));
                
            }
            EventSys.SendCoolMsg(player,"Риэлтор", "Покупка информации", $"{LangFunc.GetText(LangType.Ru, DataName.YouBoughtInfo)}", "", 15000);
           // Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouBoughtInfo), 10000);
            BattlePass.Repository.UpdateReward(player, 43);
            
            GameLog.Money($"player({characterData.UUID})", $"buyRieltagency({type})", price, $"buyRieltagency({id},{type}))");
            Wallet.Change(player, -price);
        }
        
    }
}