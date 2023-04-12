using GTANetworkAPI;
using NeptuneEvo.Handles;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Redage.SDK;
using NeptuneEvo.Chars;
using System;
using NeptuneEvo.Functions;
using NeptuneEvo.Quests;
using Database;
using Localization;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Organizations.Models;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.Players.Phone.Messages.Models;
using NeptuneEvo.Quests.Models;
using NeptuneEvo.Table.Models;
using NeptuneEvo.VehicleData.LocalData.Models;

namespace NeptuneEvo.Core
{
    class CarRoom : Script
    {
        private static readonly nLog Log = new nLog("Core.Carroom");

        public static Vector3 CamPosition = new Vector3(1123.929, -639.87, 56.724);

        private static byte Step = 0;
        private static Vector3[] TestDrivePositions = new Vector3[7]
        {
            new Vector3(972.56433, 190.15034, 80.61795),
            new Vector3(969.5283, 184.34729, 80.61813),
            new Vector3(958.94745, 173.3307, 80.618034),
            new Vector3(954.6524, 168.34235, 80.619064),
            new Vector3(951.8739, 162.38771, 80.61864),
            new Vector3(954.8739, 142.38771, 80.61864),
            new Vector3(962.8739, 152.38771, 80.61864),
        };
        public static void enterCarroom(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (sessionData.TempBizID == -1 || !BusinessManager.BizList.ContainsKey(sessionData.TempBizID)) return;
                
                var biz = BusinessManager.BizList[sessionData.TempBizID];
                
                characterData.ExteriorPos = new Vector3(biz.EnterPoint.X, biz.EnterPoint.Y, biz.EnterPoint.Z + 1.5);
                //NAPI.Entity.SetEntityPosition(player, new Vector3(CamPosition.X, CamPosition.Y - 2, CamPosition.Z));
                Trigger.UniqueDimension(player);
                OpenCarromMenu(player, biz);
            }
            catch (Exception e)
            {
                Log.Write($"enterCarroom Exception: {e.ToString()}");
            }
        }

        #region Menu
        private static IReadOnlyDictionary<string, Color> carColors = new Dictionary<string, Color>
        {
            { "Черный", new Color(0, 0, 0) },
            { "Белый", new Color(225, 225, 225) },
            { "Красный", new Color(230, 0, 0) },
            { "Оранжевый", new Color(255, 115, 0) },
            { "Желтый", new Color(240, 240, 0) },
            { "Зеленый", new Color(0, 230, 0) },
            { "Голубой", new Color(0, 205, 255) },
            { "Синий", new Color(0, 0, 230) },
            { "Фиолетовый", new Color(190, 60, 165) },
        };

        public static void OpenCarromMenu(ExtPlayer player, Business biz)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (sessionData.TempBizID == -1 || !BusinessManager.BizList.ContainsKey(sessionData.TempBizID)) return;

                var biztype = biz.Type;
                if (biztype == 15) biztype = 4;
                else biztype -= 2;
                
                var prices = new List<int>();
                var gosPrices = new List<int>();
                var names = new List<string>();
                var bagageSlots = new List<int>();
                
                foreach (Product p in biz.Products)
                {
                    if (!BusinessManager.CarsNames[biztype].Contains(p.Name)) continue;
                    var busProductData = BusinessManager.GetBusProductData(p.Name);
                    if (busProductData == null) continue;
                    if (!busProductData.Toggled) continue;
                    if (busProductData.Price == 0) continue;
                    prices.Add(p.Price);
                    gosPrices.Add(busProductData.Price);
                    names.Add(p.Name);
                    bagageSlots.Add(VehicleModel.vMain.GetMaxSlots(NAPI.Util.GetHashKey(p.Name)));
                }
                Trigger.ClientEvent(player, "openAuto", JsonConvert.SerializeObject(names), JsonConvert.SerializeObject(prices), JsonConvert.SerializeObject(gosPrices), JsonConvert.SerializeObject(bagageSlots), "carroomBuy");
            }
            catch (Exception e)
            {
                Log.Write($"OpenCarromMenu Exception: {e.ToString()}");
            }
        }
        public static void OpenCarromMenuGos(ExtPlayer player, List<string> Cars, bool isDonate = false)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;

                var prices = new List<int>();
                var gosPrices = new List<int>();
                var names = new List<string>();
                var bagageSlots = new List<int>();

                foreach (var name in Cars)
                {
                    var busProductData = BusinessManager.GetBusProductData(name);
                    if (busProductData == null) 
                        continue;
                    var price = !isDonate ? busProductData.Price : busProductData.OtherPrice;

                    if (!isDonate)
                        price = price + (price * busProductData.Percent / 100);
                    
                    if (price == 0) 
                        continue;
                    
                    prices.Add(price);
                    gosPrices.Add(busProductData.Price);
                    names.Add(name);
                    bagageSlots.Add(VehicleModel.vMain.GetMaxSlots(NAPI.Util.GetHashKey(name)));
                }
                
                Trigger.ClientEvent(player, "openAuto", JsonConvert.SerializeObject(names), JsonConvert.SerializeObject(prices), JsonConvert.SerializeObject(gosPrices), JsonConvert.SerializeObject(bagageSlots), isDonate ? "AutoroomGosDonateBuy" : "AutoroomGosBuy", isDonate);
            }
            catch (Exception e)
            {
                Log.Write($"OpenCarromMenu Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("carroomBuy")]
        public static void RemoteEvent_carroomBuy(ExtPlayer player, string vName, string color, int id)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (sessionData.TempBizID == -1 || !BusinessManager.BizList.ContainsKey(sessionData.TempBizID)) return;
                Business biz = BusinessManager.BizList[sessionData.TempBizID];
                if (id == 1) // Для себя
                {
                    var vehiclesCount = VehicleManager.GetVehiclesCarCountToPlayer(player.Name);
                    if (vehiclesCount >= Houses.GarageManager.MaxGarageCars)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас максимальное кол-во машин", 3000);
                        RemoteEvent_carroomCancel(player);
                        return;
                    }
                    Product prod = biz.Products.FirstOrDefault(p => p.Name == vName);
                    if (prod == null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Произошла ошибка, транспорта такой модели нет на складе.", 3000);
                        RemoteEvent_carroomCancel(player);
                        return;
                    }
                    if (UpdateData.CanIChange(player, prod.Price, true) != 255) return;
                    var house = Houses.HouseManager.GetHouse(player, true);
                    if (house != null)
                    {
                        if (vehiclesCount >= Houses.GarageManager.GarageTypes[Houses.GarageManager.Garages[house.GarageID].Type].MaxCars)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас максимальное кол-во машин, которое поддерживает Ваше место жительства.", 3000);
                            RemoteEvent_carroomCancel(player);
                            return;
                        }
                    }
                    if (biz.IsOwner())
                    {
                        if (biz.Type == 15)
                        {
                            if (prod.Lefts != 0)
                            {
                                if (!BusinessManager.takeProd(biz.ID, 1, vName, prod.Price))
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Машин этой модели больше нет на складе", 3000);
                                    RemoteEvent_carroomCancel(player);
                                    return;
                                }
                                biz.BuyItemBusiness(characterData.UUID, vName, prod.Price);
                            }
                        }
                        else
                        {
                            if (!BusinessManager.takeProd(biz.ID, 1, vName, prod.Price))
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Машин этой модели больше нет на складе", 3000);
                                RemoteEvent_carroomCancel(player);
                                return;
                            }
                            biz.BuyItemBusiness(characterData.UUID, vName, prod.Price);
                        }
                    }
                    MoneySystem.Wallet.Change(player, -prod.Price);
                    
                    VehicleManager.Create(player, vName, carColors[color], carColors[color], Logs: $"buyCar({vName}");
                    Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Bank, LangFunc.GetText(LangType.Ru, DataName.YouBuyCarV2, vName, prod.Price), DateTime.Now);
                }
                else if (id == 2) // В организацию
                {
                    if (!player.IsOrganizationAccess(RankToAccess.OrgBuyCars)) return;
                    var organizationData = player.GetOrganizationData();
                    if (organizationData == null) 
                        return;

                    int maxcars;
                    int upgraded = organizationData.OfficeUpgrade;
                    switch (upgraded)
                    {
                        case 1:
                            maxcars = 10;
                            break;
                        case 2:
                            maxcars = 15;
                            break;
                        default:
                            maxcars = 5;
                            break;
                    }
                    if (organizationData.Vehicles.Count >= maxcars)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вашей семьи уже максимальное количество транспортных средств.", 6000);
                        if (upgraded <= 1) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы можете увеличить количество транспортных средств купив улучшение для семьи!", 6000);
                        RemoteEvent_carroomCancel(player);
                        return;
                    }
                    int price = BusinessManager.BusProductsData[vName].Price;

                    if (UpdateData.CanIChange(player, price, true) != 255) return;
                    MoneySystem.Wallet.Change(player, -price);
                    GameLog.Money($"player({characterData.UUID})", $"server", price, $"buyOrgCar({vName})");
                    string vNumber = Organizations.Manager.CreateVehicle(organizationData.Id, vName, carColors[color]);
                    //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы купили {vName} с номером {vNumber} за {price}$.", 3000);
                    Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Bank, LangFunc.GetText(LangType.Ru, DataName.YouBuyCarV, vName, vNumber, price), DateTime.Now);
                    Organizations.Table.Logs.Repository.AddLogs(player, OrganizationLogsType.BuyCar, $"Купил {vName} ({vNumber})");
                }

                RemoteEvent_carroomCancel(player);
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_carroomBuy Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("AutoroomGosBuy")]
        public static void AutoroomGosBuy(ExtPlayer player, string vName, string color, int id)
        {
            OnAutoroomBuy(player, vName, color, id, isDonate: false);
        }

        [RemoteEvent("AutoroomGosDonateBuy")]
        public static void DonateAutoroomBuy(ExtPlayer player, string vName, string color, int id)
        {
            
            if (!FunctionsAccess.IsWorking("DonateAutoroomBuy"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                return;
            }

            OnAutoroomBuy(player, vName, color, id, isDonate: true);
        }

        private static void OnAutoroomBuy(ExtPlayer player, string vName, string color, int id, bool isDonate)
        {
            try
            {
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                NAPI.Entity.SetEntityPosition(player, characterData.ExteriorPos);
                characterData.ExteriorPos = new Vector3();
                Trigger.Dimension(player, 0);
                
                var busProductData = BusinessManager.GetBusProductData(vName);
                if (busProductData == null) return;
                
                int vehiclePrice = !isDonate ? busProductData.Price : busProductData.OtherPrice;

                if (!isDonate && id == 1)
                    vehiclePrice = vehiclePrice + (vehiclePrice * busProductData.Percent / 100);
                
                if (!isDonate && UpdateData.CanIChange(player, vehiclePrice, true) != 255)
                    return;
                
                if (isDonate && accountData.RedBucks < vehiclePrice)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);
                    return;
                }
                
                if (id == 1) // Для себя
                {
                    if (!VehicleModel.AirAutoRoom.isAirCar(vName))
                    {
                        var vehiclesCount = VehicleManager.GetVehiclesCarCountToPlayer(player.Name);
                        if (vehiclesCount >= Houses.GarageManager.MaxGarageCars)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                                $"У Вас максимальное кол-во машин", 3000);
                            RemoteEvent_carroomCancel(player);
                            return;
                        }

                        var house = Houses.HouseManager.GetHouse(player, true);
                        if (house != null)
                        {
                            if (vehiclesCount >= Houses.GarageManager
                                .GarageTypes[Houses.GarageManager.Garages[house.GarageID].Type].MaxCars)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                                    $"У Вас максимальное кол-во машин, которое поддерживает Ваше место жительства.",
                                    3000);
                                RemoteEvent_carroomCancel(player);
                                return;
                            }
                        }
                    }

                    VehicleManager.Create(player, vName, carColors[color], carColors[color], Logs: $"buyCar_donate({vName}");
                    Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Bank, LangFunc.GetText(LangType.Ru, DataName.YouBuyCarV3, vName), DateTime.Now);
                }
                else if (id == 2) // В организацию
                {
                    if (!player.IsOrganizationAccess(RankToAccess.OrgBuyCars)) return;
                    
                    if (VehicleModel.AirAutoRoom.isAirCar(vName))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете купить в организацию данное т/с.", 3000);
                        return;
                    }
     
                    var organizationData = player.GetOrganizationData();
                    if (organizationData == null) 
                        return;

                    int maxcars;
                    int upgraded = organizationData.OfficeUpgrade;
                    switch (upgraded)
                    {
                        case 1:
                            maxcars = 10;
                            break;
                        case 2:
                            maxcars = 15;
                            break;
                        default:
                            maxcars = 5;
                            break;
                    }

                    if (organizationData.Vehicles.Count >= maxcars)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"В Вашей семье уже максимальное количество транспортных средств.", 6000);
                        if (upgraded <= 1) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы можете увеличить количество транспортных средств купив улучшение для семьи!", 6000);
                        RemoteEvent_carroomCancel(player);
                        return;
                    }

                    string vNumber = Organizations.Manager.CreateVehicle(organizationData.Id, vName, carColors[color]);
                    //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы купили {vName} с номером {vNumber}", 3000);
                    Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Bank, LangFunc.GetText(LangType.Ru, DataName.YouBuyCarV, vName, vNumber, vehiclePrice), DateTime.Now);
                    Organizations.Table.Logs.Repository.AddLogs(player, OrganizationLogsType.BuyCar, $"Купил {vName} ({vNumber})");
                }
                
                if (!isDonate)
                    MoneySystem.Wallet.Change(player, -vehiclePrice);
                else
                    UpdateData.RedBucks(player, -vehiclePrice, msg:LangFunc.GetText(LangType.Ru, DataName.PremCarBuy, vName, vehiclePrice));
                //Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.RedAge, LangFunc.GetText(LangType.Ru, DataName.PremCarBuy, vName), DateTime.Now.AddSeconds(10));
            }
            catch (Exception e)
            {
                Log.Write($"OnAutoroomBuy Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("carroomCancel")]
        public static void RemoteEvent_carroomCancel(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
                
                sessionData.TempBizID = -1;

                NAPI.Entity.SetEntityPosition(player, characterData.ExteriorPos);
                characterData.ExteriorPos = new Vector3();
                Trigger.Dimension(player, 0);
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_carroomCancel Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("testDrive")]
        public static void RemoteEvent_testDrive(ExtPlayer player, string vName, string color, byte type)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (sessionData.TestDriveVehicle != null) return;

                int correctPrice = (type == 2) ? 300 : 100;
                if (UpdateData.CanIChange(player, correctPrice, true) != 255)
                {
                    RemoteEvent_carroomCancel(player);
                    return;
                }

                MoneySystem.Wallet.Change(player, -correctPrice);
                
                Trigger.Dimension(player, 555);

                var positionData = (new Vector3(), new Vector3());
                
                if (VehicleModel.AirAutoRoom.isAirCar(vName))
                    positionData = VehicleModel.AirAutoRoom.GetSpawnPosition();
                else
                {
                    positionData.Item1 = TestDrivePositions[Step];
                    positionData.Item2.Z = -99f;
                    
                    if (Step++ >= TestDrivePositions.Length - 1) Step = 0;
                }

                player.Position = positionData.Item1;
                var number = VehicleManager.GenerateNumber(VehicleAccess.AutoRoom, "AUTOROOM");
                var veh = (ExtVehicle) VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey(vName), positionData.Item1, positionData.Item2.Z, 1, 1, number, acc: VehicleAccess.AutoRoom, workdriv: characterData.UUID, petrol: 9999, dimension: 555, engine: true);
                veh.CustomPrimaryColor = carColors[color];
                veh.CustomSecondaryColor = carColors[color];

                if (type == 2)
                {
                    veh.SetMod(11, 3);
                    veh.SetMod(18, 0);
                    veh.SetMod(13, 2);
                    veh.SetMod(15, 3);
                    veh.SetMod(12, 2);
                }
                
                sessionData.TestDriveVehicle = veh;

                Trigger.ClientEvent(player, "startTestDrive", veh);
                //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы получили транспортное средство на тест-драйв. Тест-драйв будет окончен через 2 минуты или при выходе из транспортного средства.", 7000);
                sessionData.TimersData.TestDriveTimer = Timers.StartOnce(120000, () => timer_exitVehicle(player, false), true);
                GameLog.Money($"player({characterData.UUID})", $"server", correctPrice, $"testDrive({vName}, {sessionData.TempBizID})");
                BattlePass.Repository.UpdateReward(player, 33);
                /*NAPI.Task.Run(() =>
                {
                    try
                    {
                        if (!player.IsCharacterData()) return;
                        if (!sessionData.TestDriveVehicle || veh == null || !veh.Exists) return;
                        player.SetIntoVehicle(veh, (int)VehicleSeat.Driver);
                        VehicleStreaming.SetEngineState(veh, true);
                    }
                    catch (Exception e)
                    {
                        Log.Write($"RemoteEvent_testDrive Task Exception: {e.ToString()}");
                    }
                }, 1500);*/
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_testDrive Exception: {e.ToString()}");
            }
        }
        public static void timer_exitVehicle(ExtPlayer player, bool quit = false)
        {
            if (OnExitTestDrive (player) && !quit)
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Время, выделенное на тест-драйв, закончилось.", 3000);
        }

        [ServerEvent(Event.PlayerExitVehicle)]
        public void onPlayerExitVehicleHandler(ExtPlayer player, ExtVehicle vehicle)
        {
            if (OnExitTestDrive (player))
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Тест-драйв был окончен, так как Вы покинули транспорт.", 3000);
           
        }

        public static bool OnExitTestDrive(ExtPlayer player, bool isDeath = false)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return false;

                var characterData = player.GetCharacterData();
                if (characterData == null) return false;

                if (sessionData.TimersData.TestDriveTimer != null)
                {
                    Timers.Stop(sessionData.TimersData.TestDriveTimer);
                    sessionData.TimersData.TestDriveTimer = null;
                }

                var testDriveVehicle = sessionData.TestDriveVehicle;
                if (testDriveVehicle != null)
                {
                    VehicleStreaming.DeleteVehicle(testDriveVehicle);
                    sessionData.TestDriveVehicle = null;
                    Trigger.Dimension(player, 0);
                    if (characterData.ExteriorPos != new Vector3())
                    {
                        if (!isDeath) 
                            NAPI.Entity.SetEntityPosition(player, characterData.ExteriorPos);
                        else 
                            NAPI.Player.SpawnPlayer(player, characterData.ExteriorPos);
                    }
                    else
                    {
                        if (sessionData.TempBizID != -1 && BusinessManager.BizList.ContainsKey(sessionData.TempBizID))
                        {
                            Business biz = BusinessManager.BizList[sessionData.TempBizID];
                            if (!isDeath)
                                NAPI.Entity.SetEntityPosition(player,
                                    new Vector3(biz.EnterPoint.X, biz.EnterPoint.Y, biz.EnterPoint.Z + 1.5));
                            else
                                NAPI.Player.SpawnPlayer(player,
                                    new Vector3(biz.EnterPoint.X, biz.EnterPoint.Y, biz.EnterPoint.Z + 1.5));
                        }
                    }

                    characterData.ExteriorPos = new Vector3();

                    return true;
                }
            }
            catch (Exception e)
            {
                Log.Write($"Event_OnPlayerExitVehicle Exception: {e.ToString()}");
            }

            return false;
        }

        #endregion
    }
}
