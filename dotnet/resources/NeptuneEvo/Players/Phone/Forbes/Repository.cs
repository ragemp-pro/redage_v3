using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Database;
using LinqToDB;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.Houses;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.Players.Phone.Forbes.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Players.Phone.Forbes
{
    public class Repository
    {
        private static string ForbesTopString = JsonConvert.SerializeObject(new List<List<object>>());
        private static DateTime Time = DateTime.MinValue;

        public static void UpdateData()
        {
            if (Time > DateTime.Now)
                return;
            
            if (!FunctionsAccess.IsWorking("phoneforbes"))
                return;
            
            Time = DateTime.Now.AddMinutes(30 * 5);
            
            Trigger.SetTask(() => GetData());
        }
        private static async void GetData()
        {
            try
            {
                var forbesData = new Dictionary<string, ForbesData>();
            
                var bizList = BusinessManager.BizList.Values
                    .Where(b => b.IsOwner())
                    .ToList();

                foreach (var biz in bizList)
                {
                    var element = new ForbesList
                    {
                        Type = ForbesType.Biz,
                        Name = $"{BusinessManager.BusinessTypeNames[biz.Type]} #{biz.ID}",
                        Price = biz.SellPrice
                    };
                    if (!forbesData.ContainsKey(biz.Owner))
                        forbesData.Add(biz.Owner, new ForbesData
                        {
                            List = new List<ForbesList>
                            {
                                element
                            },
                            Name = biz.Owner,
                            SumMoney = (uint) biz.SellPrice
                        });
                    else
                    {
                        var forbes = forbesData[biz.Owner];
                        forbes.SumMoney += (uint) biz.SellPrice;
                        forbes.List.Add(element);
                    }
                }
                
                //
                
                var houseList = HouseManager.Houses
                    .Where(b => b.Owner != string.Empty)
                    .ToList();

                foreach (var house in houseList)
                {
                    var element = new ForbesList
                    {
                        Type = ForbesType.House,
                        Name = $"Дом #{house.ID}",
                        Price = house.Price
                    };
                    
                    if (!forbesData.ContainsKey(house.Owner))
                        forbesData.Add(house.Owner, new ForbesData
                        {
                            List = new List<ForbesList>
                            {
                                element
                            },
                            Name = house.Owner,
                            SumMoney = (uint)house.Price
                        });
                    else
                    {
                        var forbes = forbesData[house.Owner];
                        forbes.SumMoney += (uint)house.Price;
                        forbes.List.Add(element);
                    }
                }


                foreach (var vehicle in VehicleManager.Vehicles.Values.ToList())
                {
                    var vehicleData = BusinessManager.GetBusProductData(vehicle.Model);
                    if (vehicleData != null)
                    {
                        var element = new ForbesList
                        {
                            Type = ForbesType.Vehicle,
                            Name = vehicle.Model.ToUpper(),
                            Price = vehicleData.Price
                        };

                        if (!forbesData.ContainsKey(vehicle.Holder))
                            forbesData.Add(vehicle.Holder, new ForbesData
                            {
                                List = new List<ForbesList>
                                {
                                    element
                                },
                                Name = vehicle.Holder,
                                SumMoney = (uint)vehicleData.Price
                            });
                        else
                        {
                            var forbes = forbesData[vehicle.Holder];
                            forbes.SumMoney += (uint)vehicleData.Price;
                            forbes.List.Add(element);
                        }
                    }
                }
                
                
                await using var db = new ServerBD("MainDB");//В отдельном потоке

                var characters = await db.Characters
                    .ToListAsync();

                foreach (var character in characters)
                {
                    var name = $"{character.Firstname}_{character.Lastname}";
                    if (character.Adminlvl == 0)
                    {
                        var isShowForbes = character.IsForbesShow;
                        var money = Convert.ToInt32(character.Money);
                        var bankId = Convert.ToInt32(character.Bank);
                        var lvl = Convert.ToInt32(character.Lvl);

                        money += (int) Bank.GetBalance(bankId);
                    
                        if (!forbesData.ContainsKey(name))
                            forbesData.Add(name, new ForbesData
                            {
                                List = new List<ForbesList>(),
                                Name = name,
                                SumMoney = (uint)money,
                                Money = (uint)money,
                                Lvl = lvl,
                                IsShowForbes = isShowForbes
                            });
                        else
                        {
                            var forbes = forbesData[name];
                            forbes.SumMoney += (uint)money;
                            forbes.Money += (uint)money;
                            forbes.Lvl = lvl;
                            forbes.IsShowForbes = isShowForbes;

                            if (!isShowForbes)
                                forbes.List = new List<ForbesList>();
                        }
                    }
                    else
                    {
                        if (forbesData.ContainsKey(name))
                            forbesData.Remove(name);
                    }

                }
                
                //

                var forbesTopList = forbesData.Values
                    .OrderByDescending(f => f.SumMoney)
                    .Take(25)
                    .ToList();

                var forbesTopString = new List<List<object>>();

                foreach (var forbes in forbesTopList)
                {
                    var item = new List<object>();
                    
                    item.Add(forbes.Name);
                    item.Add(forbes.Money);
                    item.Add(forbes.SumMoney);
                    item.Add(forbes.Lvl);
                    item.Add(forbes.IsShowForbes);

                    var list = new List<object>();
                    
                    foreach (var forbesList in forbes.List)
                    {
                        var itemList = new List<object>();

                        itemList.Add(forbesList.Name);
                        itemList.Add(forbesList.Price);
                        itemList.Add(forbesList.Type);
                        
                        list.Add(itemList);
                    }
                    item.Add(list);
                    
                    forbesTopString.Add(item);
                }

                ForbesTopString = JsonConvert.SerializeObject(forbesTopString);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public static void OnLoad(ExtPlayer player)
        {
            if (!player.IsCharacterData())
                return;
            
            Trigger.ClientEvent(player, "client.phone.forbes.init", ForbesTopString);
        }
    }
}