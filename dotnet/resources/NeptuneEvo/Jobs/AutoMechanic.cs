using GTANetworkAPI;
using NeptuneEvo.Handles;
using System.Collections.Generic;
using System;
using NeptuneEvo.Core;
using Redage.SDK;
using System.Linq;
using Localization;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Functions;
using NeptuneEvo.Jobs.Models;
using NeptuneEvo.Quests;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;

namespace NeptuneEvo.Jobs
{
    class AutoMechanic : Script
    {
        private static readonly nLog Log = new nLog("Jobs.AutoMechanic");

        public static void mechanicRepair(ExtPlayer player, ExtPlayer target, int price)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) return;
                if (!player.IsInVehicle)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkCar), 3000);
                    return;
                }
                if (characterData.WorkID != (int)JobsId.CarMechanic || !sessionData.WorkData.OnWork)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YourNotAutoMech), 3000);
                    return;
                }
                if (player == target)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantRepairYourVehicle), 3000);
                    return;
                }
                var vehicle = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if ((sessionData.SellItemData.Buyer != null || sessionData.SellItemData.Seller != null) && Chars.Repository.TradeGet(player))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantTrade), 3000);
                        return;
                    }
                    if (vehicleLocalData.WorkId != JobsId.CarMechanic)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkCar), 3000);
                        return;
                    }
                    if (!target.IsInVehicle)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerMustBeInVeh), 3000);
                        return;
                    }
                    if (player.Vehicle.Position.DistanceTo(target.Vehicle.Position) > 5)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                        return;
                    }
                    if (price < 1000 || price > 1500)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Price5to30), 3000);
                        return;
                    }
                    if (targetCharacterData.Money < price)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerNotEnoughMoney), 3000);
                        return;
                    }
                    SellItemData sellItemData = targetSessionData.SellItemData;
                    if ((sellItemData.Buyer != null || sellItemData.Seller != null) && Chars.Repository.TradeGet(target))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PersonCantTrade), 3000);
                        return;
                    }
                    sellItemData.Seller = player;
                    sellItemData.Buyer = target;
                    sellItemData.Price = price;
                    sessionData.SellItemData.Seller = player;
                    sessionData.SellItemData.Buyer = target;
                    sessionData.SellItemData.Price = price;
                    Trigger.ClientEvent(target, "openDialog", "REPAIR_CAR", LangFunc.GetText(LangType.Ru, DataName.MechanikRepairTo, player.Value, MoneySystem.Wallet.Format(price)));
                    //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MechanikRepairFrom, target.Value, MoneySystem.Wallet.Format(price)), 3000);
                    EventSys.SendCoolMsg(player,"Предложение", "Механик", LangFunc.GetText(LangType.Ru, DataName.MechanikRepairFrom, target.Value, MoneySystem.Wallet.Format(price)), "", 5000);
                    BattlePass.Repository.UpdateReward(player, 91);
                    BattlePass.Repository.UpdateReward(player, 58);
                }
            }
            catch (Exception e)
            {
                Log.Write($"mechanicRepair Exception: {e.ToString()}");
            }
        }

        /*public static void mechanicRent(Player player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!player.IsInVehicle || player.VehicleSeat != (int)VehicleSeat.Driver) return;
                Vehicle vehicle = player.Vehicle;
                    VehicleStreaming.VehiclesData data = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if (data.Type != "MECHANIC") return;
                    MoneySystem.Wallet.Change(player, -mechanicRentCost);
                    GameLog.Money($"player({characterData.UUID})", $"server", mechanicRentCost, $"mechanicRent");
                    sessionData.WorkData.WorkId = vehicle;
                    sessionData.WorkData.InWorkCar = true;
                    sessionData.WorkData.OnWork = true;
                    data.WorkDriver = player;
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы арендовали рабочий транспорт. Ожидайте заказ", 3000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"mechanicRent Exception: {e.ToString()}");
            }
        }*/

        public static void mechanicPay(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                int price = sessionData.SellItemData.Price;
                ExtPlayer driver = sessionData.SellItemData.Seller;
                var driverSessionData = driver.GetSessionData();
                if (driverSessionData == null) return;
                var driverCharacterData = driver.GetCharacterData();
                if (driverCharacterData == null)
                {
                    sessionData.SellItemData = new SellItemData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SellerNotOnline), 3000);
                    return;
                }
                if (!player.IsInVehicle)
                {
                    driverSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustInCar), 3000);
                    return;
                }

                if (Chars.UpdateData.CanIChange(player, price, true) != 255)
                {
                    driverSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    return;
                }
                sessionData.WorkData.Player = null;
                VehicleManager.RepairCar((ExtVehicle) player.Vehicle);
                NAPI.Entity.SetEntityPosition(player.Vehicle, player.Vehicle.Position + new Vector3(0, 0, 0.5f));
                NAPI.Entity.SetEntityRotation(player.Vehicle, new Vector3(0, 0, player.Vehicle.Rotation.Z));
                MoneySystem.Wallet.Change(player, -price);
                MoneySystem.Wallet.Change(driver, price);
                GameLog.Money($"player({characterData.UUID})", $"player({driverCharacterData.UUID})", price, $"mechanicRepair");
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RepairPayed), 3000);
                Notify.Send(driver, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerPayedRepair, player.Value), 3000);
                Commands.RPChat("sme", driver, LangFunc.GetText(LangType.Ru, DataName.RepairVehi));
                driverSessionData.SellItemData = new SellItemData();
                sessionData.SellItemData = new SellItemData();

                if (qMain.GetQuestsLine(driver, Zdobich.QuestName) == (int)zdobich_quests.Stage11)
                {
                    driverSessionData.WorkData.PointsCount += price;
                    if (driverSessionData.WorkData.PointsCount < qMain.GetQuestsData(player, Zdobich.QuestName, (int) zdobich_quests.Stage11))
                        driverSessionData.WorkData.PointsCount = qMain.GetQuestsData(player, Zdobich.QuestName, (int) zdobich_quests.Stage11) + price;
                    
                    if (driverSessionData.WorkData.PointsCount >= 500)
                    {
                        qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage11, 1, isUpdateHud: true);
                        qMain.UpdateQuestsComplete(player, Zdobich.QuestName, (int) zdobich_quests.Stage11, true);
                        Trigger.SendChatMessage(driver, "!{#fc0}" + LangFunc.GetText(LangType.Ru, DataName.QuestPartComplete));
                    }
                    else
                    {
                        qMain.UpdateQuestsData(driver, Zdobich.QuestName, (int)zdobich_quests.Stage11, driverSessionData.WorkData.PointsCount.ToString());
                        //todo translate (было DataName.PointsQuestGot)
                        Trigger.SendChatMessage(driver, $"Вы заработали ${driverSessionData.WorkData.PointsCount}. Осталось ещё ${500 - driverSessionData.WorkData.PointsCount}.");
                    }
                }

                if (driverSessionData.WorkData.LastClientTime < DateTime.Now)
                {
                    if (driverCharacterData.JobSkills.ContainsKey(5))
                    {
                        if (driverCharacterData.JobSkills[5] < 250)
                            driverCharacterData.JobSkills[5] += 1;
                    }
                    else driverCharacterData.JobSkills.Add(5, 1);

                    driverSessionData.WorkData.LastClientTime = DateTime.Now.AddMinutes(5);
                }
            }
            catch (Exception e)
            {
                Log.Write($"mechanicPay Exception: {e.ToString()}");
            }
        }
        
        public static void buyFuel(ExtPlayer player, int fuel)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var rentData = sessionData.RentData;
                if (rentData == null || characterData.WorkID != (int)JobsId.CarMechanic || !sessionData.WorkData.OnWork || !player.IsInVehicle || rentData.Vehicle != player.Vehicle)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustBeMechanikAndWorkCar), 3000);
                    return;
                }

                (byte, float) jobLevelInfo = characterData.JobSkills.ContainsKey(5) ? Main.GetPlayerJobLevelBonus(5, characterData.JobSkills[5]) : (0, 1);
                if (jobLevelInfo.Item1 < 3)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Command3lvl), 3000);
                    return;
                }

                int BizID = CustomColShape.GetDataToEnum(player, ColShapeEnums.BusinessAction);
                if (BizID == (int)ColShapeData.Error || !BusinessManager.BizList.ContainsKey(BizID) || BusinessManager.BizList[BizID].Type != 1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustBeAtGas), 3000);
                    return;
                }
                if (fuel <= 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VvedireCorrect), 3000);
                    return;
                }
                Business biz = BusinessManager.BizList[BizID];
                int amount = biz.Products[0].Price * fuel;
                if (Chars.UpdateData.CanIChange(player, amount, true) != 255) return;
                var vehicle = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.UnavaibleForThisVeh), 3000);
                    return;
                }
                if (vehicleLocalData.VehLoadedFuel + fuel > 1000)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FillCanFuel), 3000);
                    return;
                }
                if (!BusinessManager.takeProd(biz.ID, fuel, biz.Products[0].Name, amount))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoFuelAtGasStation), 3000);
                    return;
                }
                MoneySystem.Wallet.Change(player, -amount);
                GameLog.Money($"player({characterData.UUID})", $"biz({biz.ID})", amount, $"mechanicBuyFuel");
                vehicleLocalData.VehLoadedFuel += fuel;
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouFillBack, vehicleLocalData.VehLoadedFuel), 5000);
            }
            catch (Exception e)
            {
                Log.Write($"buyFuel Exception: {e.ToString()}");
            }
        }

        public static void mechanicFuel(ExtPlayer player, ExtPlayer target, int fuel, int pricePerLitr)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) return;
                if (characterData.WorkID != (int)JobsId.CarMechanic || !sessionData.WorkData.OnWork)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YourNotAutoMech), 3000);
                    return;
                }
                if (!player.IsInVehicle)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkCar), 3000);
                    return;
                }

                (byte, float) jobLevelInfo = characterData.JobSkills.ContainsKey(5) ? Main.GetPlayerJobLevelBonus(5, characterData.JobSkills[5]) : (0, 1);
                if (jobLevelInfo.Item1 < 3)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Command3lvl), 3000);
                    return;
                }

                var veh = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = veh.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if (vehicleLocalData.WorkId != JobsId.CarMechanic)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkCar), 3000);
                        return;
                    }
                    if ((sessionData.SellItemData.Buyer != null || sessionData.SellItemData.Seller != null) && Chars.Repository.TradeGet(player))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantTrade), 3000);
                        return;
                    }
                    if (!target.IsInVehicle)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerMustBeInVeh), 3000);
                        return;
                    }
                    if (player.Vehicle.Position.DistanceTo(target.Vehicle.Position) > 5)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                        return;
                    }
                    if (fuel < 1)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FillErrorLiter), 3000);
                        return;
                    }
                    if (pricePerLitr < 1 || pricePerLitr > 10)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FillPriceError), 3000);
                        return;
                    }
                    if (targetCharacterData.Money < pricePerLitr * fuel)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerNotEnoughMoney), 3000);
                        return;
                    }
                    SellItemData sellItemData = targetSessionData.SellItemData;
                    if ((sellItemData.Buyer != null || sellItemData.Seller != null) && Chars.Repository.TradeGet(target))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PersonCantTrade), 3000);
                        return;
                    }
                    sellItemData.Seller = player;
                    sellItemData.Price = pricePerLitr;
                    sellItemData.Count = fuel;
                    sessionData.SellItemData.Seller = player;
                    sessionData.SellItemData.Price = pricePerLitr;
                    sessionData.SellItemData.Count = fuel;
                    Trigger.ClientEvent(target, "openDialog", "FUEL_CAR", LangFunc.GetText(LangType.Ru, DataName.PlayerOfferFill, player.Value, fuel, fuel * pricePerLitr));
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouOfferFill, target.Value, fuel, fuel * pricePerLitr), 7000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"mechanicFuel Exception: {e.ToString()}");
            }
        }

        public static void mechanicPayFuel(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                int price = sessionData.SellItemData.Price;
                int fuel = sessionData.SellItemData.Count;
                ExtPlayer driver = sessionData.SellItemData.Seller;
                var driverSessionData = driver.GetSessionData();
                if (driverSessionData == null) return;
                var driverCharacterData = driver.GetCharacterData();
                if (driverCharacterData == null)
                {
                    sessionData.SellItemData = new SellItemData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SellerNotOnline), 3000);
                    return;
                }
                if (!player.IsInVehicle)
                {
                    driverSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustInCar), 3000);
                    return;
                }
                price = price * fuel;

                if (Chars.UpdateData.CanIChange(player, price, true) != 255) 
                {
                    driverSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    return;
                }
                if (!driver.IsInVehicle)
                {
                    driverSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MechanikMustBeInJobCar), 3000);
                    return;
                }
                var veh = (ExtVehicle) driver.Vehicle;
                var vehicleLocalData1 = veh.GetVehicleLocalData();
                var veh1 = (ExtVehicle) player.Vehicle;
                var vehicleLocalData2 = veh1.GetVehicleLocalData();
                if (vehicleLocalData1 != null && vehicleLocalData2 != null)
                {

                    if (vehicleLocalData1.WorkId != JobsId.CarMechanic)
                    {
                        driverSessionData.SellItemData = new SellItemData();
                        sessionData.SellItemData = new SellItemData();
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MechanikMustBeInJobCar), 3000);
                        return;
                    }
                    
                    if (vehicleLocalData1.VehLoadedFuel < fuel)
                    {
                        driverSessionData.SellItemData = new SellItemData();
                        sessionData.SellItemData = new SellItemData();
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MechanikNoFuel), 3000);
                        return;
                    }
                    sessionData.WorkData.Player = null;
                    MoneySystem.Wallet.Change(player, -price);
                    MoneySystem.Wallet.Change(driver, price);
                    GameLog.Money($"player({characterData.UUID})", $"player({driverCharacterData.UUID})", price, $"mechanicFuel");
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouPayFillRepairVeh), 3000);
                    Notify.Send(driver, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerPayedFillVeh, player.Value), 3000);
                    Commands.RPChat("sme", driver, LangFunc.GetText(LangType.Ru, DataName.FilledVeh));
                    int carFuel = (vehicleLocalData2.Petrol + fuel > vehicleLocalData2.MaxPetrol) ? vehicleLocalData2.MaxPetrol : vehicleLocalData2.Petrol + fuel;

                    player.Vehicle.SetSharedData("PETROL", carFuel);
                    vehicleLocalData2.Petrol = carFuel;
                    if (vehicleLocalData2.Access == VehicleAccess.Personal)
                    {
                        var number = vehicleLocalData1.NumberPlate;
                        var vehicleData = VehicleManager.GetVehicleToNumber(number);
                        if (vehicleData != null) vehicleData.Fuel = carFuel;
                    }

                    vehicleLocalData1.VehLoadedFuel -= fuel;
                    driverSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                }
            }
            catch (Exception e)
            {
                Log.Write($"mechanicPayFuel Exception: {e.ToString()}");
            }
        }
    }
}
