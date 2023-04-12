using GTANetworkAPI;
using NeptuneEvo.Handles;
using MySqlConnector;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Fractions;
using NeptuneEvo.Functions; 
using NeptuneEvo.Houses;
using NeptuneEvo.MoneySystem;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Database;
using LinqToDB;
using Localization;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Jobs.Models;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.Players.Phone.Messages.Models;
using NeptuneEvo.Table.Models;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;
using NeptuneEvo.VehicleData.Models;
using NeptuneEvo.World;

namespace NeptuneEvo.Core
{
    class Commands : Script
    {
        private static readonly nLog Log = new nLog("Core.Commands");
        #region Chat logic

        public static string RainbowExploit(string message)
        {
            if (message.Contains("{")) return message.Replace("{", string.Empty);
            if (message.Contains("~")) return message.Replace("~", string.Empty);
            return message;
        }



        #endregion Chat logic

        public static Dictionary<int, (ExtTextLabel, ExtColShape)> ActionLabels = new Dictionary<int, (ExtTextLabel, ExtColShape)>();

        #region AdminCommands
        [Command("mypoint")]
        public static void CMD_findPoint(ExtPlayer player)
        {
            SessionData sessionData = player.GetSessionData();
            if (sessionData == null) return;
            if (!player.IsCharacterData()) return;
            if (sessionData.DeliveryData.Vehicle == null || sessionData.DeliveryData.Point == -1) return;

            ExtVehicle vehicle = sessionData.DeliveryData.Vehicle;
            VehicleLocalData vehicleLocalData = vehicle.GetVehicleLocalData();
            if (vehicleLocalData == null) return;

            VehicleAccess vehaccess = vehicleLocalData.Access;
            int end = sessionData.DeliveryData.Point;
            if (!vehicleLocalData.DeliveryData.JStage) // Машину еще не взяли, нужна точка до машины 
            {
                if (vehaccess == VehicleAccess.DeliveryGang) Trigger.ClientEvent(player, "createWaypoint", CarDelivery.GangSpawnAutos[end].X, CarDelivery.GangSpawnAutos[end].Y);
                else if (vehaccess == VehicleAccess.DeliveryMafia || vehaccess == VehicleAccess.DeliveryBike) Trigger.ClientEvent(player, "createWaypoint", CarDelivery.MafiaEndDelivery[end].X, CarDelivery.MafiaEndDelivery[end].Y);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucMarkerInstallSbyt), 5000);
                return;
            }
            // Машину взяли, нужна точка до места сдачи
            if (vehaccess == VehicleAccess.DeliveryGang) Trigger.ClientEvent(player, "createWaypoint", CarDelivery.GangEndDelivery[end].X, CarDelivery.GangEndDelivery[end].Y);
            else if (vehaccess == VehicleAccess.DeliveryMafia || vehaccess == VehicleAccess.DeliveryBike) Trigger.ClientEvent(player, "createWaypoint", CarDelivery.MafiaEndDelivery[end].X, CarDelivery.MafiaEndDelivery[end].Y);
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucMarkerInstallSdacha), 5000);
        }

        [Command(AdminCommands.Giveammo)]
        public static void CMD_ammo(ExtPlayer player, int ID, int type, int amount = 1)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Giveammo)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                ExtPlayer target = Main.GetPlayerByID(ID); 
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }

                ItemId[] types = new ItemId[5]
                {
                    ItemId.PistolAmmo,
                    ItemId.RiflesAmmo,
                    ItemId.ShotgunsAmmo,
                    ItemId.SMGAmmo,
                    ItemId.SniperAmmo
                };

                if (type > 5 || type < -1) return;
                if (Chars.Repository.isFreeSlots(target, types[type], amount) != 0) return;
                Chars.Repository.AddNewItem(target, $"char_{targetCharacterData.UUID}", "inventory", types[type], amount);
                GameLog.Admin($"{player.Name}", $"giveAmmo({type},{amount})", $"{target.Name}");
                Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) выдал патроны ({type}|{amount}) {target.Name} ({target.Value})");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_ammo Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Givecarnumber)]
        public static void CMD_newVehicleNumber(ExtPlayer player, string oldNum, string newNum)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Givecarnumber)) return;
                var vehicleData = VehicleManager.GetVehicleToNumber(oldNum);
                if (vehicleData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehDoesntExist), 3000);
                    return;
                }
                if (VehicleManager.IsVehicleToNumber(newNum))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NumberExists), 3000);
                    return;
                }
                Regex rg = new Regex(@"^[a-z0-9]+$", RegexOptions.IgnoreCase);
                if (!rg.IsMatch(newNum))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IncorrectInputNewNumber), 3000);
                    return;
                }

                VehicleManager.RemoveVehicleNumber(oldNum);
                VehicleManager.Vehicles.TryRemove(oldNum, out _);

                vehicleData.Number = newNum;
                VehicleManager.Vehicles[newNum] = vehicleData;
                VehicleManager.AddVehicleNumber(newNum);
                VehicleManager.VehiclesSqlIdToNumber[vehicleData.SqlId] = newNum;

                var house = HouseManager.GetHouse(vehicleData.Holder, true);
                if (house != null)
                {
                    var garage = house.GetGarageData();
                    if (garage != null)
                    {
                        garage.DeleteCar(oldNum);
                        
                        if (garage.Type != -1 && garage.Type != 6) 
                            garage.SpawnCar(newNum);
                        else 
                            garage.GetVehicleFromGarage(newNum);
                    }
                }
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NewvNum, oldNum, newNum), 3000);

                VehicleManager.SaveNumber(newNum);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_newVehicleNumber Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Kl)]
        public static void CMD_killlist(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Kl)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;

                var adminConfig = characterData.ConfigData.AdminOption;

                if (adminConfig.KillList == 0)
                {
                    Trigger.SendChatMessage(player, "~o~KillList ON");
                    adminConfig.KillList = 1;
                }
                else if (adminConfig.KillList == 1)
                {
                    Trigger.SendChatMessage(player, "~o~KillList ON (Distance Check)");
                    adminConfig.KillList = 2;
                }
                else
                {
                    Trigger.SendChatMessage(player, "~o~KillList OFF");
                    adminConfig.KillList = 0;
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_killlist Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Redname)]
        public static void CMD_redname(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Redname)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;

                var adminConfig = characterData.ConfigData.AdminOption;
                if (!adminConfig.RedName)
                {
                    Trigger.SendChatMessage(player, "~r~Redname ON");
                    player.SetSharedData("REDNAME", true);
                    adminConfig.RedName = true;
                }
                else
                {
                    Trigger.SendChatMessage(player, "~r~Redname OFF");
                    player.ResetSharedData("REDNAME");
                    adminConfig.RedName = false;
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_redname Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Hidenick)]
        public static void CMD_hidenick(ExtPlayer player) {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Hidenick)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;

                var adminConfig = characterData.ConfigData.AdminOption;
                if (!adminConfig.HideNick)
                {
                    Trigger.SendChatMessage(player, "~g~HideNick ON");
                    player.SetSharedData("HideNick", true);
                    adminConfig.HideNick = true;
                }
                else
                {
                    Trigger.SendChatMessage(player, "~g~HideNick OFF");
                    player.ResetSharedData("HideNick");
                    adminConfig.HideNick = false;
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_hidenick Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Hideme)]
        public static void CMD_hideme(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Hideme)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;

                var adminConfig = characterData.ConfigData.AdminOption;
                if (!adminConfig.HideMe)
                {
                    Trigger.SendChatMessage(player, "~g~Hide ON");
                    adminConfig.HideMe = true;
                }
                else
                {
                    Trigger.SendChatMessage(player, "~g~Hide OFF");
                    adminConfig.HideMe = false;
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_hideme Exception: {e.ToString()}");
            }

        }
        [Command(AdminCommands.Givereds)]
        public static void CMD_givereds(ExtPlayer player, int id, int amount)
        {
            try
            {
                Admin.sendRedbucks(player, Main.GetPlayerByID(id), amount);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_givereds Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Giveredsall)]
        public static void CMD_giveredsall(ExtPlayer player, int amount)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Giveredsall)) return;

                if (amount < 1 || amount > 500)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Выдать можно минимум 1 RB и максимум 500 RB.", 3000);
                    return;
                }
                
                foreach (ExtPlayer foreachPlayer in NeptuneEvo.Character.Repository.GetPlayers())
                {
                    if (!foreachPlayer.IsCharacterData()) return;
                    
                    var foreachAccountData = foreachPlayer.GetAccountData();
                    if (foreachAccountData == null) return;

                    var correctValue = amount;
                    if (foreachAccountData.RedBucks + correctValue < 0)
                        correctValue = 0;
                    
                    UpdateData.RedBucks(foreachPlayer, correctValue, msg: "Отправка RB");
                    Players.Phone.Messages.Repository.AddSystemMessage(foreachPlayer, (int)DefaultNumber.RedAge, LangFunc.GetText(LangType.Ru, DataName.RbIncome, amount, player.Name.Replace('_', ' ')), DateTime.Now);
                }
                
                NAPI.Chat.SendChatMessageToAll($"{CommandsAccess.AdminPrefixChat}{player.Name.Replace('_', ' ')} выдал всем игрокам {amount} RedBucks.");
                GameLog.Admin(player.Name, $"giveredsall({amount})", "allOnlinePlayers");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_giveredsall Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Givecase)]
        public static void CMD_givereds(ExtPlayer player, int id, byte caseid)
        {
            try
            {
                Admin.giveFreeCase(player, Main.GetPlayerByID(id), caseid);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_givereds Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Check)]
        public static void CMD_checkProperety(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Check)) return;
                ExtPlayer target = Main.GetPlayerByID(id);
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }
                if (targetCharacterData.BizIDs.Count > 0)
                {
                    try
                    {
                        Business biz = BusinessManager.BizList[targetCharacterData.BizIDs[0]];
                        Trigger.SendChatMessage(player, $"Игрок владеет бизнесом {BusinessManager.BusinessTypeNames[biz.Type]} (ID:{biz.ID})");
                    }
                    catch { Log.Write("CMD_checkProperety ERROR"); }
                }
                var house = HouseManager.GetHouse(target);
                if (house != null)
                {
                    if (house.Owner == target.Name)
                    {
                        Trigger.SendChatMessage(player, $"Игрок имеет недвижимость (ID{house.ID}) стоимостью ${house.Price} класса '{HouseManager.HouseTypeList[house.Type].Name}'");
                        var vehiclesNumber = VehicleManager.GetVehiclesCarNumberToPlayer(target.Name);
                        foreach (string number in vehiclesNumber)
                        {
                            var vehicleData = VehicleManager.GetVehicleToNumber(number);
                            if (vehicleData == null) continue;
                            Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.PlayerHaveCar, vehicleData.Model, number));
                        }
                    }
                    else
                    {
                        Trigger.SendChatMessage(player, $"Игрок заселен в дом (ID{house.ID}) к {house.Owner} стоимостью ${house.Price} класса '{HouseManager.HouseTypeList[house.Type].Name}'");
                        var vehiclesNumber = VehicleManager.GetVehiclesCarNumberToPlayer(house.Owner);
                        foreach (string number in vehiclesNumber)
                        {
                            var vehicleData = VehicleManager.GetVehicleToNumber(number);
                            if (vehicleData == null) continue;
                            Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.PlayerHaveCar, vehicleData.Model, number));
                        }
                        vehiclesNumber = VehicleManager.GetVehiclesCarNumberToPlayer(target.Name);
                        foreach (string number in vehiclesNumber)
                        {
                            var vehicleData = VehicleManager.GetVehicleToNumber(number);
                            if (vehicleData == null) continue;
                            Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.PlayerHaveCar, vehicleData.Model, number));
                        }
                    }
                }
                else
                {
                    Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.PersonNoHome));
                    var vehiclesNumber = VehicleManager.GetVehiclesCarNumberToPlayer(target.Name);
                    foreach (string number in vehiclesNumber)
                    {
                        var vehicleData = VehicleManager.GetVehicleToNumber(number);
                        if (vehicleData == null) continue;
                        Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.PlayerHaveCar, vehicleData.Model, number));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_checkProperety Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Id)]
        public static void CMD_checkId(ExtPlayer player, string target)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Id)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                int id;
                if (int.TryParse(target, out id))
                {
                    foreach (ExtPlayer foreachPlayer in NeptuneEvo.Character.Repository.GetPlayers())
                    {
                        var foreachCharacterData = foreachPlayer.GetCharacterData();
                        if (foreachCharacterData == null) 
                            continue;

                        if (foreachPlayer.Value == id)
                        {
                            if (foreachCharacterData.AdminLVL >= 6)
                            {
                                var foreachAdminConfig = foreachCharacterData.ConfigData.AdminOption;
                                if (foreachAdminConfig.HideMe && foreachCharacterData.AdminLVL > characterData.AdminLVL) break;
                            }
                            Trigger.SendChatMessage(player, $"ID: {foreachPlayer.Value} | {foreachPlayer.Name}");
                            return;
                        }
                    }
                    Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId));
                }
                else
                {
                    int players = 0;
                    if (target.Length < 3)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Name3Symb), 3000);
                        return;
                    }
                    foreach (ExtPlayer foreachPlayer in NeptuneEvo.Character.Repository.GetPlayers())
                    {
                        var foreachCharacterData = foreachPlayer.GetCharacterData();
                        if (foreachCharacterData == null) continue;

                        if (foreachPlayer.Name.ToUpper().Contains(target.ToUpper()))
                        {
                            if (foreachCharacterData.AdminLVL >= 6)
                            {
                                var foreachAdminConfig = foreachCharacterData.ConfigData.AdminOption;
                                if (foreachAdminConfig.HideMe && foreachCharacterData.AdminLVL > characterData.AdminLVL) continue;
                            }
                            Trigger.SendChatMessage(player, $"ID: {foreachPlayer.Value} | {foreachPlayer.Name}");
                            players++;
                        }
                    }
                    if (players == 0) Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.CantFindMan));
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_checkId Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Setdim)]
        public static void CMD_setDim(ExtPlayer player, int id, int dim)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Setdim)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                ExtPlayer target = Main.GetPlayerByID(id);
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }
                if (targetSessionData.TestDriveVehicle != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerOnTestDrive), 3000);
                    return;
                }
                if (targetCharacterData.InsideHouseID != -1 && targetCharacterData.AdminLVL == 0) return;
                Trigger.Dimension(target, (uint)dim);
                Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) выставил Dimension({dim}) {target.Name} ({target.Value})");
                GameLog.Admin($"{player.Name}", $"setDim({dim})", $"{target.Name}");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_setDim Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Checkdim)]
        public static void CMD_checkDim(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Checkdim)) return;
                ExtPlayer target = Main.GetPlayerByID(id);
                if (!target.IsCharacterData())
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }
                GameLog.Admin($"{player.Name}", $"checkDim", $"{target.Name}");
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerDimIs, UpdateData.GetPlayerDimension(target).ToString()), 4000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_checkDim Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Setbizmafia)]
        public static void CMD_setBizMafia(ExtPlayer player, int mafia)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Setbizmafia)) return;
                int BizID = CustomColShape.GetDataToEnum(player, ColShapeEnums.BusinessAction);
                if (BizID == (int)ColShapeData.Error)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouNotOnBiz), 3000);
                    return;
                }
                if (mafia < 10 || mafia > 13) return;
                if(!BusinessManager.BizList.ContainsKey(BizID)) return;
                Business biz = BusinessManager.BizList[BizID];
                biz.Mafia = mafia;
                biz.UpdateLabel();
                GameLog.Admin($"{player.Name}", $"setBizMafia({biz.ID},{mafia})", $"");
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MafiaOwnsBiz, mafia, biz.ID), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_setBizMafia Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Newsimcard)]
        public static void CMD_newsimcard(ExtPlayer player, int id, int newnumber)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Newsimcard)) return;
                var target = Main.GetPlayerByID(id);      
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }
                if (Players.Phone.Sim.Repository.Contains(newnumber))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NumberExists), 3000);
                    return;
                }

                Chars.Repository.AddNewItem(target, $"char_{targetCharacterData.UUID}", "inventory", ItemId.SimCard, 1, newnumber.ToString());
                
                GameLog.Admin($"{player.Name}", $"newsim({newnumber})", $"{target.Name}");
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NewvNum, target.Name, newnumber), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_newsimcard Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Takeoffbiz)]
        public static void CMD_takeOffBusiness(ExtPlayer admin, int bizid, bool byaclear = false)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(admin, AdminCommands.Takeoffbiz)) return;
                Business biz = BusinessManager.BizList[bizid];
                string owner = biz.Owner;
                ExtPlayer player = (ExtPlayer) NAPI.Player.GetPlayerFromName(owner);
                var characterData = player.GetCharacterData();
                if (characterData == null)
                {
                    if (!Main.PlayerNames.Values.Contains(biz.Owner) || !Main.PlayerUUIDs.ContainsKey(biz.Owner))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок не найден", 3000);
                        return;
                    }
                    int targetUuid = Main.PlayerUUIDs[biz.Owner];
                    Trigger.SetTask(async () =>
                    {
                        try
                        {
                            await using var db = new ServerBD("MainDB");//В отдельном потоке

                            var character = await db.Characters
                                .Select(c => new
                                {
                                    c.Uuid,
                                    c.Biz,
                                    c.Money,
                                })
                                .Where(v => v.Uuid == targetUuid)
                                .FirstOrDefaultAsync();

                            if (character == null)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Человек не найден", 3000);
                                return;
                            }
                            var ownerBizs = JsonConvert.DeserializeObject<List<int>>(character.Biz);
                            ownerBizs.Remove(biz.ID);

                            await db.Characters
                                .Where(c => c.Uuid == character.Uuid)
                                .Set(c => c.Biz, JsonConvert.SerializeObject(ownerBizs))
                                .Set(c => c.Money, character.Money + Convert.ToInt32(biz.SellPrice * 0.8))
                                .UpdateAsync();
                        }
                        catch (Exception e)
                        {
                            Debugs.Repository.Exception(e);
                        }
                    });
                }
                else
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AdminRejectBiz), 3000);
                    Wallet.Change(player, Convert.ToInt32(biz.SellPrice * 0.8));
                    characterData.BizIDs.Remove(biz.ID);
                    //Chars.Repository.PlayerStats(player);
                }

                var bizBalance = Bank.Accounts[biz.BankID];
                bizBalance.Balance = 0;
                bizBalance.IsSave = true;
                
                biz.ClearOwner();
                
                Houses.Rieltagency.Repository.OnPayDay(new List<House>(), new List<Business>()
                {
                    biz
                });
                GameLog.Money($"server", $"player({Main.PlayerUUIDs[owner]})", Convert.ToInt32(biz.SellPrice * 0.8), $"takeoffBiz({biz.ID})");
                Notify.Send(admin, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouRejectBiz, owner), 3000);
                if (!byaclear) 
                    GameLog.Admin($"{admin.Name}", $"takeoffBiz({biz.ID})", $"");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_takeOffBusiness Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.MoneyMultiplier)]
        public static void CMD_MoneyMultiplier(ExtPlayer player, int multi)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.MoneyMultiplier)) return;
                if (multi < 1 || multi > 5)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Возможно установить только от 1 до 5", 3000);
                    return;
                }
                Main.ServerSettings.MoneyMultiplier = multi;
                /* // NOT WORKING AT 0.3.7
                if(multi >= 2) NAPI.Server.SetServerName($"{Main.ServerName} | X{multi}");
                else NAPI.Server.SetServerName(Main.ServerName);
                */
                GameLog.Admin($"{player.Name}", $"MoneyMultiplier({multi})", $"");
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"MoneyMultiplier изменен на {multi}", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_MoneyMultiplier Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Expmultiplier)]
        public static void CMD_expmultiplier(ExtPlayer player, int multi)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Expmultiplier)) return;
                if (multi < 1 || multi > 5)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Возможно установить только от 1 до 5", 3000);
                    return;
                }

                Main.ServerSettings.ExpMultiplier = multi;
                GameLog.Admin($"{player.Name}", $"expMultiplier({multi})", $"");
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"ExpMultiplier изменен на {multi}", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_expmultiplier Exception: {e.ToString()}");
            }
        }


        [Command(AdminCommands.Offdelfrac)] 
        public static void CMD_offlineDelFraction(ExtPlayer player, string name)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Offdelfrac)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!Main.PlayerNames.Values.Contains(name) || !Main.PlayerUUIDs.ContainsKey(name))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок не найден", 3000);
                    return;
                }
                int targetUuid = Main.PlayerUUIDs[name];
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы уволили игрока {name} из фракции", 3000);
                Manager.RemoveFractionMemberData(name);
                GameLog.Admin($"{player.Name}", $"delfrac", $"{name}");
                Admin.AdminsLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) уволил из фракции {name}");
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы сняли фракцию с {name}", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_offlineDelFraction Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Delobj)]
        public static void CMD_removeObject(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Delobj)) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                sessionData.AdminData.IsRemoveObject = true;
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Следующий подобранный предмет будет в бане", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_removeObject Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Unwarn)]
        public static void CMD_unwarn(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Unwarn)) return;
                ExtPlayer target = Main.GetPlayerByID(id);
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }
                if (targetCharacterData.Warns <= 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока нет варнов", 3000);
                    return;
                }
                targetCharacterData.Warns--;

                targetCharacterData.WarnInfo.Admin[targetCharacterData.Warns] = "-1";
                targetCharacterData.WarnInfo.Reason[targetCharacterData.Warns] = "-1";

                NAPI.Chat.SendChatMessageToAll("!{#DF5353}" + $"Администратор {player.Name} снял предупреждение с игрока {target.Name}({target.Value}).");

                //Chars.Repository.PlayerStats(target);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы сняли варн у игрока {target.Name}, у него осталось {targetCharacterData.Warns} варнов", 3000);
                Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"С Вас было снято предупреждение, осталось {targetCharacterData.Warns} варнов", 3000);
                GameLog.Admin($"{player.Name}", AdminCommands.Unwarn, $"{target.Name}");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_unwarn Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Offunwarn)]
        public static void CMD_offunwarn(ExtPlayer player, string targetName)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Offunwarn)) return;
                if (!Main.PlayerNames.Values.Contains(targetName) || !Main.PlayerUUIDs.ContainsKey(targetName))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок не найден", 3000);
                    return;
                }
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                ExtPlayer target = (ExtPlayer) NAPI.Player.GetPlayerFromName(targetName);
                if (target.IsCharacterData())
                {
                    CMD_unwarn(player, target.Value);
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Игрок был онлайн, поэтому offunwarn заменён на unwarn", 3000);
                    return;
                }
                int targetUuid = Main.PlayerUUIDs[targetName];
                
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        var character = await db.Characters
                            .Select(c => new
                            {
                                c.Uuid,
                                c.Warns,
                                c.Warninfo,
                            })
                            .Where(v => v.Uuid == targetUuid)
                            .FirstOrDefaultAsync();

                        if (character == null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Человек не найден", 3000);
                            return;
                        }
                        var warns = Convert.ToInt32(character.Warns);
                        var warninfo = JsonConvert.DeserializeObject<WarnInfo>(character.Warninfo);
          
                        if (warninfo == null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ошибка при снятии Warn'а. Сообщите разработчику", 3000);
                            return;
                        }
                        if (warns <= 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока нет варнов", 3000);
                            return;
                        }
                        warns--;
                        warninfo.Admin[warns] = "-1";
                        warninfo.Reason[warns] = "-1";

                        Trigger.SetMainTask(() =>
                        {
                            NAPI.Chat.SendChatMessageToAll("!{#DF5353}" + $"Администратор {sessionData.Name} снял предупреждение с игрока {targetName} в оффлайне");
                            GameLog.Admin($"{sessionData.Name}", $"offUnwarn", $"{targetName}");
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы сняли варн у игрока {targetName}, у него {warns} варнов", 3000); 
                        });

                        await db.Characters
                            .Where(c => c.Uuid == character.Uuid)
                            .Set(c => c.Warns, warns)
                            .Set(c => c.Warninfo, JsonConvert.SerializeObject(warninfo))
                            .UpdateAsync();
                    }	
                    catch (Exception e)
                    {
                        Log.Write($"CMD_offunwarn SetTask Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Log.Write($"CMD_offunwarn Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Spvehs)]
        public static void CMD_respawnCars(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Spvehs)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                var vehiclesLocalData = RAGE.Entities.Vehicles.All.Cast<ExtVehicle>()
                    .Where(v => v.VehicleLocalData != null)
                    .Where(v => v.VehicleLocalData.Access == VehicleAccess.Fraction || 
                                v.VehicleLocalData.Access == VehicleAccess.Organization ||
                                v.VehicleLocalData.Access == VehicleAccess.DeliveryGang||
                                v.VehicleLocalData.Access == VehicleAccess.DeliveryMafia ||
                                v.VehicleLocalData.Access == VehicleAccess.DeliveryBike ||
                                v.VehicleLocalData.Access == VehicleAccess.Admin)
                    .Where(v => v.VehicleLocalData.Occupants.Count == 0)
                    .ToList();

                int countToSpawn = 0;

                foreach (var vehicleAdmin in vehiclesLocalData)
                {
                    var localData = vehicleAdmin.VehicleLocalData;
                    switch (localData.Access)
                    {
                        case VehicleAccess.Fraction:
                            Admin.RespawnFractionCar(vehicleAdmin);
                            countToSpawn++;
                            continue;
                        case VehicleAccess.DeliveryGang:
                        case VehicleAccess.DeliveryMafia:
                        case VehicleAccess.DeliveryBike:
                        case VehicleAccess.Admin:
                            VehicleStreaming.DeleteVehicle(vehicleAdmin);
                            countToSpawn++;
                            continue;
                        case VehicleAccess.Organization:
                            if (Ticket.IsVehicleTickets(localData.NumberPlate, VehicleTicketType.Organization))
                                continue;
                            
                            int orgId = localData.Fraction;
                            var organizationData = Organizations.Manager.GetOrganizationData(orgId);
                            if (organizationData == null) 
                                continue;
                            
                            int petrol = localData.Petrol;
                            var dirt = 0f;
                            
                            var vehicleStateData = vehicleAdmin.GetVehicleLocalStateData();
                            if (vehicleStateData != null)
                                dirt = vehicleStateData.Dirt;
                            
                            VehicleStreaming.DeleteVehicle(vehicleAdmin);
                            var organizationVehicle = organizationData.Vehicles[localData.NumberPlate];

                            var vehicleCreate = VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey(organizationVehicle.model), Organizations.Manager.GaragePositions[organizationVehicle.garageId], Organizations.Manager.GarageRotations[organizationVehicle.garageId], 0, 0, localData.NumberPlate, dimension: (uint) (Organizations.Manager.DefaultDimension + orgId), locked: true, acc: VehicleAccess.OrganizationGarage, fr: orgId, minrank: organizationVehicle.rank, petrol: petrol, dirt: dirt);

                            VehicleManager.OrgApplyCustomization(vehicleCreate, organizationVehicle.customization);
                            countToSpawn++;
                            continue;
                    }
                }
                Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) отправил все машины на спавн ({countToSpawn} шт.)");
                GameLog.Admin($"{player.Name}", AdminCommands.Spvehs, $"All");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_respawnCar Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Spveh)]
        public static void CMD_respawnCar(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Spveh)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!player.IsInVehicle) return;
                var vehicle = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    switch (vehicleLocalData.Access)
                    {
                        case VehicleAccess.Fraction:
                            Admin.RespawnFractionCar(vehicle);
                            break;
                        case VehicleAccess.DeliveryGang:
                        case VehicleAccess.DeliveryMafia:
                        case VehicleAccess.DeliveryBike:
                            VehicleStreaming.DeleteVehicle(vehicle);
                            return;
                        default:
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Этот транспорт нельзя отправить на спавн.", 3000);
                            return;
                    }
                    Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) отправил машину на спавн ({vehicle.Value})");
                    GameLog.Admin($"{player.Name}", AdminCommands.Spveh, $"{vehicle.NumberPlate}");
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_respawnCar Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Spvehid)]
        public static void CMD_respawnCarID(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Spvehid)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var vehs = RAGE.Entities.Vehicles.All.Cast<ExtVehicle>();
                foreach (ExtVehicle veh in vehs)
                {
                    if (veh.Value != id) continue;

                    var vehicleLocalData = veh.GetVehicleLocalData();
                    if (vehicleLocalData != null)
                    {
                        switch (vehicleLocalData.Access)
                        {
                            case VehicleAccess.Fraction:
                                Admin.RespawnFractionCar(veh);
                                break;
                            default:
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Этот транспорт нельзя отправить на спавн.", 3000);
                                return;
                        }
                        Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) отправил машину на спавн ({veh.Value})");
                        GameLog.Admin($"{player.Name}", AdminCommands.Spvehid, $"{veh.NumberPlate}");
                    }
                    break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_respawnCarID Exception: {e.ToString()}");
            }
        }
        /*
        [Command(AdminCommands.Spvehall)]
        public static void CMD_Spvehall(Player player)
        {
            try
            {
                Admin.respawnAllCars(player);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_delMyCars Exception: {e.ToString()}");
            }
        }
        */
        [Command("promo")]
        public static void CMD_GetMyPromo(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                Main.PromoCodesData pcdata = Main.PromoCodes.Values.FirstOrDefault(p => p.CreatorUUID == characterData.UUID);
                if (pcdata == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Промокода, который привязан к Вашему персонажу, не найдено", 3000);
                    return;
                }
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Ваш промокод использовали {pcdata.UsedTimes} раз.", 10000);
                switch (pcdata.RewardVipLvl)
                {
                    case 1:
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Бонусы: Silver VIP на {pcdata.RewardVipDays} дней, {pcdata.RewardMoney}$ и {pcdata.RewardItems.Count} вещей.", 10000);
                        break;
                    case 2:
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Бонусы: Gold VIP на {pcdata.RewardVipDays} дней, {pcdata.RewardMoney}$ и {pcdata.RewardItems.Count} вещей.", 10000);
                        break;
                    case 3:
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Бонусы: Platinum VIP на {pcdata.RewardVipDays} дней, {pcdata.RewardMoney}$ и {pcdata.RewardItems.Count} вещей.", 10000);
                        break;
                    case 4:
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Бонусы: Diamond VIP на {pcdata.RewardVipDays} дней, {pcdata.RewardMoney}$ и {pcdata.RewardItems.Count} вещей.", 10000);
                        break;
                    default:
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Бонусы: {pcdata.RewardMoney}$ и {pcdata.RewardItems.Count} вещей.", 10000);
                        break;
                }
                if (pcdata.DonatePercent != 0)
                {
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Помимо прочего Вы получаете {pcdata.DonatePercent * 100}% от доната в виде RedBucks на аккаунт {pcdata.DonateLogin}, когда игрок с Вашим промо-кодом пополнит свой счёт (Пример: при 1% за пополнение игроком 100RB Вы получите 1RB).", 15000);
                    if (pcdata.DonateReceivedByStreamer != 0) Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"За всё время Вы получили {pcdata.DonateReceivedByStreamer} RedBucks за других игроков.", 15000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_GetMyPromo Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Promosync)]
        public static void CMD_PromocmdSync(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Promosync)) return;
                Task.Run(async () =>
                {
                    await SyncThread.PromoSync();
                    NAPI.Task.Run(() =>
                    {
                        try
                        {
                            foreach (ExtPlayer foreachPlayer in NeptuneEvo.Character.Repository.GetPlayers())
                            {
                                try
                                {
                                    var foreachCharacterData = foreachPlayer.GetCharacterData();
                                    if (foreachCharacterData == null) continue;
                                    if (Main.Media.Contains(foreachCharacterData.UUID)) foreachPlayer.SetSharedData("IS_MEDIA", true);
                                    else foreachPlayer.SetSharedData("IS_MEDIA", false);
                                }
                                catch (Exception e)
                                {
                                    Log.Write($"PromoSync Task Foreach Exception: {e.ToString()}");
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Write($"PromoSync Task Exception: {e.ToString()}");
                        }
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Промокоды перезагружены успешно!", 3000);
                    });
                });
            }
            catch (Exception e)
            {
                Log.Write($"CMD_PromocmdSync Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Bonussync)]
        public static void CMD_BonuscmdSync(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Bonussync)) return;
                Task.Run(async () =>
                {
                    await SyncThread.BonusSync();
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Бонус коды перезагружены успешно!", 3000);
                });
            }
            catch (Exception e)
            {
                Log.Write($"CMD_BonuscmdSync Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Ecosync)]
        public static void CMD_EconomySync(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Ecosync)) return;
                Economy.Init();
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно перезагрузили экономику сервера из базы данных.", 5000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_EconomySync Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Fsetcmd)]
        public static void CMD_FracCmdSync(ExtPlayer player, string cmd, byte fractionId, byte access)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Fsetcmd)) return;
                var fractionData = Manager.GetFractionData(fractionId);
                if (fractionData == null)
                    return;

                if (access <= 0 || access > fractionData.Ranks.Count) return;
                /*if (Configs.FractionCommands[frac].ContainsKey(cmd))
                {
                    Configs.FractionCommands[frac][cmd] = access;
                    using MySqlCommand mcmd = new MySqlCommand
                    {
                        CommandText = "UPDATE `fractionaccess` SET `commands`=@cmds WHERE `fraction`=@fra"
                    };
                    mcmd.Parameters.AddWithValue("@cmds", JsonConvert.SerializeObject(Configs.FractionCommands[frac]));
                    mcmd.Parameters.AddWithValue("@fra", frac);
                    MySQL.Query(mcmd);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы установили новый уровень доступа ({access}) на команду {cmd}", 3000);
                }
                else*/
                if (Configs.FractionWeapons[fractionId].ContainsKey(cmd))
                {
                    Configs.FractionWeapons[fractionId][cmd] = access;

                    Trigger.SetTask(async () =>
                    {
                        try
                        {
	
                            await using var db = new ServerBD("MainDB");//В отдельном потоке

                            await db.Fractionaccess
                                .Where(v => v.Fraction == fractionId)
                                .Set(v => v.Weapons, JsonConvert.SerializeObject(Configs.FractionWeapons[fractionId]))
                                .UpdateAsync();
                        }
                        catch (Exception e)
                        {
                            Debugs.Repository.Exception(e);
                        }
                    });

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы установили новый уровень доступа ({access}) на оружие/патроны {cmd}", 3000);
                }
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Такого доступа не найдено в этой фракции", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_FracCmdSync Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Setcolour)]
        public static void CMD_setTerritoryColor(ExtPlayer player, int gangid)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Setcolour)) return;
                
                var shapeData = CustomColShape.GetData(player, ColShapeEnums.GangZone);
                if (shapeData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не находитесь ни на одном из регионов", 3000);
                    return;
                }
                int terrid = shapeData.Index;
                if (!GangsCapture.gangPointsColor.ContainsKey(gangid))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Банды с таким ID нет", 3000);
                    return;
                }

                var region = GangsCapture.GangPoints[terrid];
                region.GangOwner = gangid;
                region.Save();
                Trigger.ClientEventForAll("setZoneColor", region.ID, Fractions.GangsCapture.gangPointsColor[gangid]);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Теперь территорией №{terrid} владеет {Fractions.Manager.FractionNames[gangid]}", 3000);
                GameLog.Admin($"{player.Name}", $"setColour({terrid},{gangid})", $"");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_setTerritoryColor Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Sc)]
        public static void CMD_setClothes(ExtPlayer player, int id, int draw, int texture)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Sc)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                bool gender = characterData.Gender;
                ConcurrentDictionary<int, Chars.ClothesData> clothesData = null;
                switch (id)
                {
                    case 1: // Mask
                        clothesData = ClothesComponents.ClothesComponentData[gender][ClothesComponent.Masks];
                        if (clothesData.ContainsKey(draw)) draw = clothesData[draw].Variation;
                        break;
                    case 4: // Legs
                        clothesData = ClothesComponents.ClothesComponentData[gender][ClothesComponent.Legs];
                        if (clothesData.ContainsKey(draw)) draw = clothesData[draw].Variation;
                        break;
                    case 6: // Shoes
                        clothesData = ClothesComponents.ClothesComponentData[gender][ClothesComponent.Shoes];
                        if (clothesData.ContainsKey(draw)) draw = clothesData[draw].Variation;
                        break;
                    case 7: // Accessories
                        clothesData = ClothesComponents.ClothesComponentData[gender][ClothesComponent.Accessories];
                        if (clothesData.ContainsKey(draw)) draw = clothesData[draw].Variation;
                        break;
                    case 11: // Top
                        clothesData = ClothesComponents.ClothesComponentData[gender][ClothesComponent.Tops];
                        if (clothesData.ContainsKey(draw)) draw = clothesData[draw].Variation;
                        break;
                    default:
                        break;
                }
                ClothesComponents.SetSpecialClothes(player, id, draw, texture);
                ClothesComponents.UpdateClothes(player);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_setClothes Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Sac)]
        public static void CMD_setAccessories(ExtPlayer player, int id, int draw, int texture)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Sac)) return;
                if (draw > -1) ClothesComponents.SetSpecialAccessories(player, id, draw, texture);
                else ClothesComponents.ClearAccessory(player, id);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_setAccessories Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Checkwanted)]
        public static void CMD_checkwanted(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Checkwanted)) return;
                ExtPlayer target = Main.GetPlayerByID(id);
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }
                int stars = (targetCharacterData.WantedLVL == null) ? 0 : targetCharacterData.WantedLVL.Level;
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Кол-во звёзд - {stars}", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_checkwanted Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Fixcar)]
        public static void CMD_fixcar(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Fixcar)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!player.IsInVehicle) return;
                var vehicle = (ExtVehicle) player.Vehicle;
                VehicleManager.RepairCar(vehicle);
                Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) починил машину ({vehicle.Value})");
                GameLog.Admin($"{player.Name}", $"fixcar", $"{vehicle.DisplayName}");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_fixcar Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Fixcarid)]
        public static void CMD_fixcarID(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Fixcarid)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var vehs = RAGE.Entities.Vehicles.All.Cast<ExtVehicle>();
                foreach (ExtVehicle veh in vehs)
                {
                    if (veh.Value != id) continue;
                    VehicleManager.RepairCar(veh);
                    Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) починил машину по ID ({veh.Value})");
                    GameLog.Admin($"{player.Name}", $"fixcarid({id})", $"{veh.DisplayName}");
                    break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_fixcarID Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Offstats)]
        public static void CMD_showPlayerOffStats(ExtPlayer player, string Name)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Offstats)) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!Main.PlayerNames.Values.Contains(Name) || !Main.PlayerUUIDs.ContainsKey(Name))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок не найден", 3000);
                    return;
                }
                ExtPlayer target = (ExtPlayer) NAPI.Player.GetPlayerFromName(Name);
                if (target != null)
                {
                    CMD_showPlayerStats(player, target.Value);
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Игрок с такими именем был онлайн, поэтому offstats заменён на stats", 3000);
                    return;
                }
                Trigger.SetTask(() =>
                {
                    using MySqlCommand cmd = new MySqlCommand
                    {
                        CommandText = $"SELECT * FROM characters WHERE uuid=@val0"
                    };
                    cmd.Parameters.AddWithValue("@val0", Main.PlayerUUIDs[Name]);

                    using DataTable result = MySQL.QueryRead(cmd);
                    if (result == null || result.Rows.Count == 0)
                    {
                        Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"Игрока с именем {Name} нет в системе", 3000);
                        return;
                    }
                    var charRow = result.Rows[0];
                    
                    var uuid = Convert.ToInt32(charRow["uuid"]);
                    
                    var login = "";
                    var vipLvl = 0;
                    var vipDate = DateTime.Now;

                    using MySqlCommand cmdSELECT = new MySqlCommand
                    {
                        CommandText = $"SELECT login, viplvl, vipdate FROM accounts WHERE character1=@val0 OR character2=@val0 OR character3=@val0"
                    };

                    cmdSELECT.Parameters.AddWithValue("@val0", uuid);
                    using DataTable resultSELECT = MySQL.QueryRead(cmdSELECT);

                    if (resultSELECT != null && resultSELECT.Rows.Count >= 1)
                    {
                        var accRow = resultSELECT.Rows[0];
                        login = accRow["login"].ToString();
                        vipLvl = Convert.ToInt32(accRow["viplvl"]);
                        vipDate = Convert.ToDateTime(accRow["vipdate"]);
                    }
                    
                                    
                    var charData = new List<object>();
                
                    charData.Add(login);//0
                    charData.Add(vipLvl);//1
                    charData.Add(vipDate);//2
                    charData.Add(Convert.ToInt32(charRow["warns"]));//3
                    charData.Add(Convert.ToDateTime(charRow["unwarn"]));//4
                    var time = JsonConvert.DeserializeObject<TimeInfo>(charRow["time"].ToString());
                    time = Main.GetCurrencyTime(null, time);
                    charData.Add(time.TodayTime);//5
                    charData.Add(time.MonthTime);//6
                    charData.Add(time.YearTime);//7
                    charData.Add(time.TotalTime);//8
                    //Skils
                    charData.Add(null);//9
                    charData.Add(null);//10
                    charData.Add(null);//11
                    //
                    var firstname = Convert.ToString(charRow["firstname"]);
                    var lastname = Convert.ToString(charRow["lastname"]);
                    charData.Add($"{firstname} {lastname}");//12
                    charData.Add(Convert.ToInt32(charRow["adminlvl"]) > 0);//13
                    var weddingName = Convert.ToString(charRow["WeddingName"]);
                    charData.Add(weddingName.Length > 5 ? weddingName : "Нет");//14
                    charData.Add(Convert.ToBoolean(charRow["gender"]));//15
                    charData.Add(Convert.ToInt32(charRow["lvl"]));//16
                    charData.Add(Convert.ToInt32(charRow["exp"]));//17
                    charData.Add(Convert.ToInt32(charRow["sim"]));//18
                    charData.Add(Convert.ToInt32(charRow["work"]));//19
                    
                    var targetMemberFractionData = Fractions.Manager.GetFractionMemberData(uuid);
                    if (targetMemberFractionData != null)
                    {
                        charData.Add(targetMemberFractionData.Id);//20
                        charData.Add(Fractions.Manager.GetFractionRankName(targetMemberFractionData.Id, targetMemberFractionData.Rank));//21
                    }
                    else
                    {
                        charData.Add(null);//20
                        charData.Add(null);//21
                    }

                    var targetMemberOrganizationData = Organizations.Manager.GetOrganizationMemberData(uuid);
                    if (targetMemberOrganizationData != null)
                    {
                        charData.Add(targetMemberOrganizationData.Id);//22
                        charData.Add(Organizations.Manager.GetOrganizationRankName(targetMemberOrganizationData.Id, targetMemberOrganizationData.Rank));//23
                        /*var targetOrganizationData = Organizations.Manager.GetOrganizationData(targetMemberFractionData.Id);
                        if (targetOrganizationData != null)
                        {
                            charData.Add(targetOrganizationData.Name);//22
                            charData.Add(Organizations.Manager.GetOrganizationRankName(targetMemberOrganizationData.Id, targetMemberOrganizationData.Rank));//23
                        }
                        else
                        {
                            charData.Add(null);//22
                            charData.Add(null);//23
                        }*/
                    }
                    else
                    {
                        charData.Add(null);//22
                        charData.Add(null);//23
                    }


                    charData.Add(uuid);//24
                    var bank = Convert.ToInt32(charRow["bank"]);
                    charData.Add(bank);//25
                    charData.Add(Bank.GetBalance(bank));//26
                    charData.Add(Convert.ToInt64(charRow["money"]));//27
                    charData.Add(Convert.ToDateTime(charRow["createdate"]));//28
                    
                    //
                    
                    
                    var house = HouseManager.GetHouse($"{firstname}_{lastname}", false);
                    var garage = house?.GetGarageData();
                    if (house != null)
                    {
                        charData.Add(house.ID);//28
                        int houseBank = (int)Bank.GetBalance(house.BankID);
                        charData.Add(HouseManager.HouseTypeList[house.Type].Name);//29
                        charData.Add(house.Price == 0 ? "$0 / $0" : $"${Wallet.Format(houseBank)} / ${Wallet.Format(Chars.Repository.GetTax(house.Price, vipLvl))}");//30
                        var tax = Convert.ToInt32(house.Price / 100 * 0.026);
                        charData.Add(house.Price == 0 ? "$0" : $"${Wallet.Format(tax)}");//31
                        int paid = (houseBank == 0 || tax == 0) ? 0 : Convert.ToInt32(houseBank / tax);
                        charData.Add(paid);//32
                        charData.Add(garage != null ? GarageManager.GarageTypes[garage.Type].MaxCars : 0);//33
                    }
                    else
                    {
                        charData.Add(null);//28
                        charData.Add(null);//29
                        charData.Add(null);//30
                        charData.Add(null);//31
                        charData.Add(null);//32
                        charData.Add(null);//33
                    }

                    var bizIDs = JsonConvert.DeserializeObject<List<int>>(charRow["biz"].ToString());
                    if (bizIDs.Count > 0)
                    {
                        Business biz = BusinessManager.BizList[bizIDs[0]];
                        charData.Add(biz.ID);//34
                        int BizBank = (int)Bank.GetBalance(biz.BankID);
                        charData.Add(biz.SellPrice == 0 ? "$0 / $0" : $"${Wallet.Format(BizBank)} / ${Wallet.Format(Chars.Repository.GetTax(biz.SellPrice, vipLvl, biz.Tax))}");//35
                        charData.Add(biz.SellPrice == 0 ? "$0" : $"${Wallet.Format(Convert.ToInt32(biz.SellPrice / 100 * biz.Tax))}");//36
                        int paid = (BizBank == 0 || biz.SellPrice == 0) ? 0 : BizBank / Convert.ToInt32(biz.SellPrice / 100 * biz.Tax);
                        charData.Add(paid);//37
                    }
                    else
                    {
                        charData.Add(null);//34
                        charData.Add(null);//35
                        charData.Add(null);//36
                        charData.Add(null);//37
                    }
                    
                    charData.Add(JsonConvert.DeserializeObject<List<bool>>(charRow["licenses"].ToString()));//38
                    charData.Add(null);//39
                    charData.Add(null);//40

                    var statsData = JsonConvert.SerializeObject(charData);
                    
                    Trigger.ClientEvent(player, "client.accountStore.otherStatsData", statsData);

                    
                    //

                    int SqlBag = 0;
                    List<InventoryItemData> _JsonAccessoriesItemData = new List<InventoryItemData>();
                    List<InventoryItemData> _JsonInventoryItemData = new List<InventoryItemData>();
                    List<InventoryItemData> _JsonFastSlotsItemData = new List<InventoryItemData>();
                    List<InventoryItemData> _JsonBackPackItemData = new List<InventoryItemData>();

                    string locationName = $"char_{uuid}";
                    if (Chars.Repository.ItemsData.ContainsKey(locationName))
                    {
                        foreach (string Location in Chars.Repository.ItemsData[locationName].Keys)
                        {
                            var sortedItemData = Chars.Repository.ItemsData[locationName][Location].Values.OrderBy(i => i.Index).ToList();
                            foreach (InventoryItemData item in sortedItemData)
                            {
                                if (item.ItemId == ItemId.Debug) continue;
                                if (item.ItemId == ItemId.Bag) SqlBag = item.SqlId;

                                if (Location == "accessories") _JsonAccessoriesItemData.Add(item);
                                else if (Location == "inventory") _JsonInventoryItemData.Add(item);
                                else if (Location == "fastSlots") _JsonFastSlotsItemData.Add(item);
                            }
                        }
                    }
                    if (SqlBag != 0)
                    {
                        _JsonBackPackItemData = JsonConvert.DeserializeObject<List<InventoryItemData>>(Chars.Repository.ClientEventLoadItemsData($"backpack_{SqlBag}", "backpack", 0).Item1);
                    }
                        
                    Dictionary<string, List<InventoryItemData>> _ItemsData = new Dictionary<string, List<InventoryItemData>>
                    {
                        { "accessories", _JsonAccessoriesItemData },
                        { "inventory", _JsonInventoryItemData },
                        { "fastSlots", _JsonFastSlotsItemData },
                        { "backpack", _JsonBackPackItemData },
                    };


                    Trigger.ClientEvent(player, "client.inventory.InitData", JsonConvert.SerializeObject(_ItemsData), false);

                    Trigger.ClientEvent(player, "client.inventory.Open");

                    sessionData.LookingStats = true;
                    Admin.AdminLog(characterData.AdminLVL, $"{sessionData.Name} ({sessionData.Value}) смотрит offline статистику {Name}");
                });
                
            }
            catch (Exception e)
            {
                Log.Write($"CMD_showPlayerOffStats Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Stats)]
        public static void CMD_showPlayerStats(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Stats)) return;

                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                ExtPlayer target = Main.GetPlayerByID(id);
                var targetAccountData = target.GetAccountData();
                if (targetAccountData == null) 
                    return;

                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }
                if (target == player) return;
                if (targetCharacterData.AdminLVL >= 6)
                {
                    var targetAdminConfig = targetCharacterData.ConfigData.AdminOption;
                    if (targetAdminConfig.HideMe && characterData.AdminLVL < targetCharacterData.AdminLVL)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                        Notify.Send(target, NotifyType.Alert, NotifyPosition.BottomCenter, $"{player.Name} ({player.Value}) попытался посмотреть Вашу статистику (/stats)", 3000);
                        return;
                    }
                }
                Chars.Repository.InitInventory(player, target);
                Trigger.ClientEvent(player, "client.inventory.Open");
                sessionData.LookingStats = true;
                Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) смотрит статистику {target.Name} ({target.Value})");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_showPlayerStats Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Admins)]
        public static void CMD_AllAdmins(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Admins)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                Trigger.SendChatMessage(player, "=== ADMINS ONLINE ===");
                var currentDate = DateTime.Now;
                foreach (ExtPlayer foreachPlayer in Main.AllAdminsOnline)
                {
                    var foreachSessionData = foreachPlayer.GetSessionData();
                    if (foreachSessionData == null) 
                        continue;

                    var foreachCharacterData = foreachPlayer.GetCharacterData();
                    if (foreachCharacterData == null) 
                        continue;

                    if (foreachCharacterData.AdminLVL < 1) 
                        continue;

                    if (foreachCharacterData.AdminLVL >= 6)
                    {
                        var foreachAdminConfig = foreachCharacterData.ConfigData.AdminOption;
                        if (foreachAdminConfig.HideMe && foreachCharacterData.AdminLVL > characterData.AdminLVL) continue;
                    }

                    string afkTimeInfo = "";
                    var afkData = foreachSessionData.AfkData;
                    if (afkData.IsAfk)
                    {
                        var inAFK = currentDate - afkData.Time;
                        afkTimeInfo = $"[AFK {inAFK.Minutes + 1} мин]";
                    }

                    Trigger.SendChatMessage(player, $"[{foreachPlayer.Value}] {foreachPlayer.Name} - {foreachCharacterData.AdminLVL} {afkTimeInfo}");
                }
                Trigger.SendChatMessage(player, "=== ADMINS ONLINE ===");
                Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) проверяет список администрации в сети");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_AllAdmins Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Medialist)]
        public static void CMD_AllMedias(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Medialist)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                Trigger.SendChatMessage(player, "=== MEDIA ONLINE ===");
                foreach (ExtPlayer foreachPlayer in NeptuneEvo.Character.Repository.GetPlayers())
                {
                    var foreachCharacterData = foreachPlayer.GetCharacterData();
                    if (foreachCharacterData == null) continue;
                    if (Main.Media.Contains(foreachCharacterData.UUID)) Trigger.SendChatMessage(player, $"[{foreachPlayer.Value}] {foreachPlayer.Name}");
                }
                Trigger.SendChatMessage(player, "=== MEDIA ONLINE ===");
                Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) проверяет список медиа в сети");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_AllMedias Exception: {e.ToString()}");
            }
        }

        [Command(VipCommands.Cam)]
        public static void CMD_Flycam_Cinema(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (!CommandsAccess.CanUseCmd(player, VipCommands.Cam)) return;
                Trigger.ClientEvent(player, "client.flycam");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_Flycam_Cinema Exception: {e.ToString()}");
            }
        }

        [Command(VipCommands.Camtime)]
        public static void CMD_Flycam_CinemaTime(ExtPlayer player, float Value)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (!CommandsAccess.CanUseCmd(player, VipCommands.Camtime)) return;
                Trigger.ClientEvent(player, "client.flycam.time", Value);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_Flycam_Cinema Exception: {e.ToString()}");
            }
        }

        private static string[] ModelsAllowedToMedia = new string[30]
        {
            "imporage", "boar", "cat", "chickenhawk", "chimp", "chop", "cormorant", "cow", "coyote", "crow", "deer", "hen", "husky", 
            "mountainlion", "pig", "pigeon", "poodle", "pug", "rabbit", "rat", "retriever", "rhesus", "rottweiler", "seagull", "shepherd", 
            "westy", "jesus01", "movalien01", "orleans", "orleanscutscene"
        };

        [Command(AdminCommands.Skin)]
        public static void CMD_SetMySkin(ExtPlayer player, string pedModel)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Skin)) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (pedModel.Equals("-1"))
                {
                    if (sessionData.AdminSkin)
                    {
                        sessionData.AdminSkin = false;
                        player.SetDefaultSkin();
                    }
                }
                else
                {
                    if (ModelsAllowedToMedia.Contains(pedModel.ToLower()))
                    {
                        PedHash pedHash = NAPI.Util.PedNameToModel(pedModel);
                        sessionData.AdminSkin = true;
                        if (pedHash != 0) player.SetSkin(pedHash);
                        else player.SetSkin(NAPI.Util.GetHashKey(pedModel));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_SetMySkin Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Clear)]
        public static void CMD_Clear(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Clear)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                ExtPlayer target = Main.GetPlayerByID(id);
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }
                targetCharacterData.WantedLVL = null;
                Trigger.ClientEvent(target, "client.charStore.Wanted", 0);
                Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) снял розыск {target.Name} ({target.Value})");
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы сняли розыск {target.Name} ({target.Value})", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_Clear Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Agl)]
        public static void CMD_Agl(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Agl)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                ExtPlayer target = Main.GetPlayerByID(id);
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }
                targetCharacterData.Licenses = new List<bool>() { true, true, true, true, true, true, true, true, true };
                Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) выдал комплект лицензий {target.Name} ({target.Value})");
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы выдали комплект лицензий {target.Name} ({target.Value})", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_Clear Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Vlist)]
        public static void CMD_AllVList(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Vlist)) return;
                Trigger.ClientEvent(player, "CheckMyVList");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_AllVList Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Additem)]
        public static void CMD_additem(ExtPlayer player, string Name, int Value, string Data)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Additem)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                ItemId ItemId = (ItemId)Enum.Parse(typeof(ItemId), Name);
                //if (Chars.Repository.isFreeSlots(player, ItemId, Value) != 0) return;
                Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId, Value, Data);
                Trigger.SendChatMessage(player, $"~r~Вы выдали себе {Chars.Repository.ItemsInfo[ItemId].Name}!");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_additem Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Goadditem)]
        public static void CMD_goadditem(ExtPlayer player, string Name, int Value, string Data)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Goadditem)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                ItemId ItemId = (ItemId)Enum.Parse(typeof(ItemId), Name);
                //if (Chars.Repository.isFreeSlots(player, ItemId, Value) != 0) return;
                Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId, Value, Data);
                Trigger.SendChatMessage(player, $"~r~Вы выдали себе {Chars.Repository.ItemsInfo[ItemId].Name}!");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_goadditem Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Nalog)]
        public static void CMD_nalog(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Nalog)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var toggled = !Main.ServerSettings.IsHouseTax;
                Main.ServerSettings.IsBusinessTax = toggled;
                Main.ServerSettings.IsHouseTax = toggled;
                if (toggled)
                    Trigger.SendChatMessage(player, $"Списывание налогов включено!");
                else 
                    Trigger.SendChatMessage(player, $"Списывание налогов выключено!");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_goadditem Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Setprod)]
        public static void CMD_setprod(ExtPlayer player, int id)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                else if (!CommandsAccess.CanUseCmd(player, AdminCommands.Setprod)) return;
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                string login = accountData.Login;
                if (!BusinessManager.BizList.ContainsKey(id)) return;
                Business biz = BusinessManager.BizList[id];
                foreach (Product p in biz.Products) p.Lefts = BusinessManager.BusProductsData[p.Name].MaxCount;
                Trigger.SendChatMessage(player, "~r~Вы полностью пополнили склад бизнеса!");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_setprod Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Setproductbyindex)]
        public static void CMD_setproductbyindex(ExtPlayer player, int id, int index, int product)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Setproductbyindex)) return;
                if (!BusinessManager.BizList.ContainsKey(id)) return;
                else if (BusinessManager.BizList[id].Products.Count <= index) return;
                BusinessManager.BizList[id].Products[index].Lefts = product;
            }
            catch (Exception e)
            {
                Log.Write($"CMD_setproductbyindex Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Deleteproducts)]
        public static void CMD_deleteproducts(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Deleteproducts)) return;
                if (!BusinessManager.BizList.ContainsKey(id)) return;
                Business biz = BusinessManager.BizList[id];
                foreach (Product p in biz.Products) p.Lefts = 0;
            }
            catch (Exception e)
            {
                Log.Write($"CMD_deleteproducts Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Changebizprice)]
        public static void CMD_changeBusinessPrice(ExtPlayer player, int newPrice)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Changebizprice)) return;
                int BizID = CustomColShape.GetDataToEnum(player, ColShapeEnums.BusinessAction);
                if (BizID == (int)ColShapeData.Error)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouNotOnBiz), 3000);
                    return;
                }
                Business biz = BusinessManager.BizList[BizID];
                biz.SellPrice = newPrice;
                biz.UpdateLabel();
            }
            catch (Exception e)
            {
                Log.Write($"CMD_changeBusinessPrice Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Changehouseprice)]
        public static void CMD_changeHousePrice(ExtPlayer player, int newPrice)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Changehouseprice)) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (sessionData.HouseID == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны стоять на маркере дома", 3000);
                    return;
                }

                var house = HouseManager.Houses.FirstOrDefault(h => h.ID == sessionData.HouseID);
                if (house == null) return;
                house.Price = newPrice;
                house.UpdateLabel();
            }
            catch (Exception e)
            {
                Log.Write($"CMD_changeHousePrice Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Pa)]
        public static void CMD_playAnimation(ExtPlayer player, string dict, string anim, int flag)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Pa)) return;
                Trigger.PlayAnimation(player, dict, anim, flag);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_playAnimation Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Sa)]
        public static void CMD_stopAnimation(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Sa)) return;
                Trigger.StopAnimation(player);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_stopAnimation Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Changestock)]
        public static void CMD_changeStock(ExtPlayer player, int fractionId, string item, int amount, bool add = false)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Changestock)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var fractionData = Manager.GetFractionData(fractionId);
                if (fractionData == null)
                {
                    Trigger.SendChatMessage(player, "~r~Склада такой фракции нет");
                    return;
                }
                switch (item)
                {
                    case "mats":
                        if (!add) fractionData.Materials = amount;
                        else fractionData.Materials += amount;
                        break;
                    case "drugs":
                        if (!add) fractionData.Drugs = amount;
                        else fractionData.Drugs += amount;
                        break;
                    case "medkits":
                        if (!add) fractionData.MedKits = amount;
                        else fractionData.MedKits += amount;
                        break;
                    case "money":
                        if (characterData.AdminLVL < 8)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно прав", 3000);
                            return;
                        }
                        if (!add) fractionData.Money = amount;
                        else fractionData.Money += amount;
                        break;
                    default:
                        Trigger.SendChatMessage(player, "~r~mats - материалы");
                        Trigger.SendChatMessage(player, "~r~drugs - наркотики");
                        Trigger.SendChatMessage(player, "~r~medkits - мед. аптечки");
                        Trigger.SendChatMessage(player, "~r~money - деньги");
                        return;
                }
                fractionData.UpdateLabel();
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы успешно изменили склад.", 1500);
                GameLog.Admin($"{player.Name}", $"changeStock({item},{amount})", $"");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_changeStock Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Tpc, GreedyArg = true)]
        public static void CMD_tpCoord(ExtPlayer player, string cords)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Tpc)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                cords = cords.Replace(",", "");
                string[] result = cords.Split(' ');
                float[] resultCords = new float[3];
                for (int i = 0; i < 3; i++)
                {
                    if (float.TryParse(result[i], out resultCords[i]) == false)
                    {
                        player.SendChatMessage("Ошибка! Введите корректные координаты");
                        return;
                    }
                }
                Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) телепортировался по координатам {resultCords[0]} {resultCords[1]} {resultCords[2]}");
                NAPI.Entity.SetEntityPosition(player, new Vector3(resultCords[0], resultCords[1], resultCords[2]));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_tpCoord Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Delfrac)]
        public static void CMD_delFrac(ExtPlayer player, int id)
        {
            try
            {
                Admin.delFrac(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_delFrac Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Sendcreator)]
        public static void CMD_SendToCreator(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Sendcreator)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                ExtPlayer target = Main.GetPlayerByID(id);
                if (!target.IsCharacterData())
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }
                if (characterData.AdminLVL >= 6 || Admin.SendCreatorQueue.ContainsKey(target))
                {
                    if (characterData.AdminLVL >= 6)
                    {
                        Admin.AdminsLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) отправил в creator {target.Name} ({target.Value})");
                        GameLog.Admin($"{player.Name}", $"sendCreator", $"{target.Name}");
                    }
                    else
                    {
                        if (!Admin.SendCreatorQueue.ContainsKey(target)) return;
                        if (Admin.SendCreatorQueue[target].Item1 == player.Name)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Подтвердить может только другой администратор!", 3000);
                            return;
                        }
                        Admin.AdminsLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) подтвердил запрос смены внешности от администратора {Admin.SendCreatorQueue[target].Item1}");
                        Admin.AdminsLog(Admin.SendCreatorQueue[target].Item2, $"{Admin.SendCreatorQueue[target].Item1} отправил в creator {target.Name} ({target.Value})");
                        GameLog.Admin($"{player.Name}", $"sendCreatorAccept", $"{Admin.SendCreatorQueue[target].Item1}");
                        GameLog.Admin($"{Admin.SendCreatorQueue[target].Item1}", $"sendCreator", $"{target.Name}");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы подтвердили запрос на смену внешности!", 3000);
                    }
                    if (Admin.SendCreatorQueue.ContainsKey(target)) Admin.SendCreatorQueue.Remove(target);
                    Character.Friend.Repository.ClearFriends(target, target.Name);
                    Customization.SendToCreator(target);
                }
                else
                {
                    if (Admin.SendCreatorQueue.ContainsKey(target)) return;
                    Admin.SendCreatorQueue.Add(target, (player.Name, characterData.AdminLVL));
                    Trigger.SendToAdmins(2, $"{ChatColors.StrongOrange}[A] Запрос от {player.Name} ({player.Value}) на смену внешности {target.Name}. Чтобы подтвердить действие - введите: /sendcreator {id}");
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_SendToCreator Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Vehchange)]
        public static void CMD_vehchage(ExtPlayer player, string newmodel)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Vehchange)) return;
                if (!player.IsInVehicle) return;
                var vehicle = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if (vehicleLocalData.Access == VehicleAccess.Personal)
                    {
                        var vehicleData = VehicleManager.GetVehicleToNumber(player.Vehicle.NumberPlate);
                        vehicleData.Model = newmodel;
                        Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Машина будет доступна после респавна", 3000);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_vehchage Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Afuel)]
        public static void CMD_setVehiclePetrol(ExtPlayer player, int fuel)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Afuel)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!player.IsInVehicle) return;
                if (characterData.AdminLVL <= 5 && (fuel >= 200 || fuel < 0))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Заправить машину можно только до 200 литров!", 3000);
                    return;
                }
                var veh = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = veh.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if (vehicleLocalData.Access == VehicleAccess.Personal || vehicleLocalData.Access == VehicleAccess.Garage)
                    {
                        string number = veh.NumberPlate;
                        var vehicleData = VehicleManager.GetVehicleToNumber(number);
                        if (vehicleData != null) 
                            vehicleData.Fuel = fuel;
                    }
                    vehicleLocalData.Petrol = fuel;
                    player.Vehicle.SetSharedData("PETROL", fuel);
                    GameLog.Admin($"{player.Name}", $"afuel({fuel})", $"");
                    Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) заправил машину ({player.Vehicle.Value},{fuel}л)");
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_setVehiclePetrol Exception: {e.ToString()}");
            }
        }
        /*

        [Command(AdminCommands.Afuelall)]
        public static void CMD_Afuelall(Player player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Afuelall)) return;
                foreach (Vehicle veh in NAPI.Pools.GetAllVehicles())
                {
                        VehicleStreaming.VehiclesData data = veh.GetVehicleLocalData();
                    if (vehicleLocalData != null)
                    {
                        if (data.Access == VehicleAccess.Personal || data.Access == VehicleAccess.Garage)
                        {
                            string number = veh.NumberPlate;
                            if (VehicleManager.Vehicles.ContainsKey(number)) vehicleData.Fuel = VehicleManager.VehicleTank[veh.Class];
                        }
                        data.Petrol = VehicleManager.VehicleTank[veh.Class];
                        veh.SetSharedData("PETROL", VehicleManager.VehicleTank[veh.Class]);
                    }
                }
                Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) заправил все машины");
                GameLog.Admin($"{player.Name}", AdminCommands.Afuelall, $"");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_delMyCars Exception: {e.ToString()}");
            }
        }
        */
        [Command(AdminCommands.Setname, GreedyArg = true)]
        public static void CMD_changeNamebyId(ExtPlayer player, int id, string newName)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Setname)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                ExtPlayer target = Main.GetPlayerByID(id);
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }
                if (!newName.Contains('_') || newName.Contains(' '))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Неверный ввод нового имени, отсутствует нужный символ!", 3000);
                    return;
                }
                if (Main.PlayerNames.Values.Contains(newName))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NameExists), 3000);
                    return;
                }
                string[] split = newName.Split("_");
                if (split[0].Length < 3 || split[1].Length < 3)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Слишком маленькое имя или фамилия!", 3000);
                    return;
                }
                Regex rg = new Regex(@"^[a-z]+$", RegexOptions.IgnoreCase);
                if (!rg.IsMatch(split[0]))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Неверный ввод нового имени!", 3000);
                    return;
                }
                if (!rg.IsMatch(split[1]))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Неверный ввод новой фамилии!", 3000);
                    return;
                }
                string curient = target.Name;
                if (characterData.AdminLVL >= 5 || (Admin.ChangeNameQueue.ContainsKey(curient) && Admin.ChangeNameQueue[curient].Item1 == newName))
                {
                    if (targetCharacterData.AdminLVL >= characterData.AdminLVL && target != player)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нельзя изменить ник администратора, который выше уровнем!", 3000);
                        return;
                    }
                    if (characterData.AdminLVL >= 5)
                    {
                        Admin.AdminsLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) изменил имя {curient} на {newName}");
                        GameLog.Admin($"{player.Name}", $"changeName({newName})", $"{curient}");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Ник изменен!", 3000);
                    }
                    else
                    {
                        if (!Admin.ChangeNameQueue.ContainsKey(curient)) return;
                        if (Admin.ChangeNameQueue[curient].Item2 == player.Name)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Подтвердить может только другой администратор!", 3000);
                            return;
                        }
                        Admin.AdminsLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) подтвердил запрос изменения администратора {Admin.ChangeNameQueue[curient].Item2}");
                        Admin.AdminsLog(Admin.ChangeNameQueue[curient].Item3, $"{Admin.ChangeNameQueue[curient].Item2} изменил имя {curient} на {newName}");
                        GameLog.Admin($"{player.Name}", $"changeNameAccept", $"{Admin.ChangeNameQueue[curient].Item2}");
                        GameLog.Admin($"{Admin.ChangeNameQueue[curient].Item2}", $"changeName({newName})", $"{curient}");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы подтвердили запрос на изменение имени!", 3000);
                    }

                    if (Admin.ChangeNameQueue.ContainsKey(curient)) 
                        Admin.ChangeNameQueue.Remove(curient);

                    Character.Change.Repository.ChangeName(target, newName);
                }
                else
                {
                    if (Admin.ChangeNameQueue.ContainsKey(curient) && Admin.ChangeNameQueue[curient].Item1 != newName)
                    {
                        Admin.ChangeNameQueue.Remove(curient);
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Старый запрос на изменение ника для этого персонажа удалён", 3000);
                    }
                    Admin.ChangeNameQueue.Add(curient, (newName, player.Name, characterData.AdminLVL));
                    Trigger.SendToAdmins(2, $"{ChatColors.StrongOrange}[A] Запрос от {player.Name} ({player.Value}) на смену имени {curient} -> {newName}. Чтобы подтвердить действие - введите: /setname {curient} {newName}");
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_changeNamebyId Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Setnameoff, GreedyArg = true)]
        public static void CMD_changeName(ExtPlayer player, string curient, string newName)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Setnameoff)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!Main.PlayerNames.Values.Contains(curient)) return;
                if (!newName.Contains('_') || newName.Contains(' '))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Неверный ввод нового имени, отсутствует нужный символ!", 3000);
                    return;
                }
                if (Main.PlayerNames.Values.Contains(newName))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NameExists), 3000);
                    return;
                }
                string[] split = newName.Split("_");
                if (split[0].Length < 3 || split[1].Length < 3)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Неверный ввод нового имени!", 3000);
                    return;
                }
                Regex rg = new Regex(@"^[a-z]+$", RegexOptions.IgnoreCase);
                if (!rg.IsMatch(split[0]))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Неверный ввод нового имени!", 3000);
                    return;
                }
                if (!rg.IsMatch(split[1]))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Неверный ввод новой фамилии!", 3000);
                    return;
                }
                if (characterData.AdminLVL >= 6 || (Admin.ChangeNameQueue.ContainsKey(curient) && Admin.ChangeNameQueue[curient].Item1 == newName))
                {
                    ExtPlayer target = (ExtPlayer) NAPI.Player.GetPlayerFromName(curient);
                    var targetCharacterData = target.GetCharacterData();
                    if (targetCharacterData != null && targetCharacterData.AdminLVL >= characterData.AdminLVL && target != player)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нельзя изменить ник администратора, который выше уровнем!", 3000);
                        return;
                    }
                    if (characterData.AdminLVL >= 6)
                    {
                        Admin.AdminsLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) изменил имя {curient} на {newName} в оффлайне");
                        GameLog.Admin($"{player.Name}", $"changeName({newName})", $"{curient}");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Ник изменен!", 3000);
                    }
                    else
                    {
                        if (!Admin.ChangeNameQueue.ContainsKey(curient)) return;
                        if (Admin.ChangeNameQueue[curient].Item2 == player.Name)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Подтвердить может только другой администратор!", 3000);
                            return;
                        }
                        Admin.AdminsLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) подтвердил запрос изменения администратора {Admin.ChangeNameQueue[curient].Item2}");
                        Admin.AdminsLog(Admin.ChangeNameQueue[curient].Item3, $"{Admin.ChangeNameQueue[curient].Item2} изменил имя {curient} на {newName} в оффлайне");
                        GameLog.Admin($"{player.Name}", $"changeNameAccept", $"{Admin.ChangeNameQueue[curient].Item2}");
                        GameLog.Admin($"{Admin.ChangeNameQueue[curient].Item2}", $"changeName({newName})", $"{curient}");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы подтвердили запрос на изменение имени!", 3000);
                    }

                    if (Admin.ChangeNameQueue.ContainsKey(curient)) 
                        Admin.ChangeNameQueue.Remove(curient);

                    if (target == null) Character.Change.Repository.ChangeNameOffline(curient, newName);
                    else Character.Change.Repository.ChangeName(target, newName);
                    
                    return;
                }
                else
                {
                    if (Admin.ChangeNameQueue.ContainsKey(curient) && Admin.ChangeNameQueue[curient].Item1 != newName)
                    {
                        Admin.ChangeNameQueue.Remove(curient);
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Старый запрос на изменение ника для этого персонажа удалён", 3000);
                    }
                    Admin.ChangeNameQueue.Add(curient, (newName, player.Name, characterData.AdminLVL));
                    Trigger.SendToAdmins(2, $"{ChatColors.StrongOrange}[A] Запрос от {player.Name} ({player.Value}) на смену имени {curient} -> {newName}. Чтобы подтвердить действие - введите: /setnameoff {curient} {newName}");
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_changeName Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Startmatwars)]
        public static void CMD_startMatWars(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Startmatwars)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (MatsWar.isWar)
                {
                    Trigger.SendChatMessage(player, "~r~Война за маты уже идёт");
                    return;
                }
                MatsWar.startWar();
                Trigger.SendToAdmins(3, $"{ChatColors.StrongOrange}[A] {player.Name} запустил" + (characterData.Gender ? "" : "а") + " ВЗМ!");
                GameLog.Admin($"{player.Name}", $"startMatwars", $"");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_startMatWars Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Stopmatwars)]
        public static void CMD_Stopmatwars(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Stopmatwars)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (MatsWar.isWar == false)
                {
                    Trigger.SendChatMessage(player, "~r~ВЗМ еще не начат, он должен быть запущен.");
                    return;
                }
                MatsWar.endWar();
                Trigger.SendToAdmins(3, $"{ChatColors.StrongOrange}[A] {player.Name} закончил" + (characterData.Gender ? "" : "а") + " ВЗМ");
                GameLog.Admin($"{player.Name}", $"stopMatwars", $"");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_startMatWars Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.StartCam)]
        public static void CMD_StartCam(ExtPlayer player)
        {
            try
            {
                Trigger.ClientEvent(player, "enablehelicam");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_StartCam Exception: {e.ToString()}");

            }
        }
        [Command(AdminCommands.Giveexp)]
        public static void CMD_giveExp(ExtPlayer player, int id, int exp)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Giveexp)) return;
                ExtPlayer target = Main.GetPlayerByID(id);
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }
                targetCharacterData.EXP += exp;
                if (exp >= 1)
                {
                    if (targetCharacterData.EXP >= 3 + targetCharacterData.LVL * 3)
                    {
                        targetCharacterData.EXP = targetCharacterData.EXP - (3 + targetCharacterData.LVL * 3);
                        UpdateData.Level(target, 1);
                    }
                } 
                else if(exp < 0)
                {
                    if (targetCharacterData.EXP <= 0)
                    {
                        targetCharacterData.EXP = 0;
                        if(targetCharacterData.LVL >= 1) UpdateData.Level(target, -1);
                    }
                }
                Trigger.ClientEvent(target, "client.charStore.EXP", targetCharacterData.EXP);
                Trigger.ClientEvent(target, "client.charStore.LVL", targetCharacterData.LVL);
                //Chars.Repository.PlayerStats(target);
                GameLog.Admin($"{player.Name}", $"giveExp({exp})", $"{target.Name}");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_giveExp Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Givelvl)]
        public static void CMD_giveLvl(ExtPlayer player, int id, int lvl)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Givelvl)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                ExtPlayer target = Main.GetPlayerByID(id);
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }
                targetCharacterData.LVL += lvl;
                if (lvl >= 1)
                {
                    if (targetCharacterData.LVL >= 3 + targetCharacterData.LVL * 3)
                    {
                        targetCharacterData.LVL = targetCharacterData.LVL - (3 + targetCharacterData.LVL * 3);
                        UpdateData.Level(target, 1);
                    }
                }
                else if (lvl < 0)
                {
                    if (targetCharacterData.LVL <= 0)
                    {
                        targetCharacterData.LVL = 0;
                        if (targetCharacterData.LVL >= 1) UpdateData.Level(target, -1);
                    }
                }
                Trigger.ClientEvent(target, "client.charStore.EXP", targetCharacterData.EXP);
                Trigger.ClientEvent(target, "client.charStore.LVL", targetCharacterData.LVL);
                //Repository.PlayerStats(target);
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы выдали игроку {target.Name} ({target.Value}) {lvl} уровней", 3000);
                Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) выдал {target.Name} ({target.Value}) {lvl} уровней");
                GameLog.Admin($"{player.Name}", $"giveLvl({lvl})", $"{target.Name}");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_giveLvl Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Housetypeprice)]
        public static void CMD_replaceHousePrices(ExtPlayer player, int type, int newPrice)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Housetypeprice)) return;
                if (type < 0 || type >= 7) return;
                foreach (House h in HouseManager.Houses)
                {
                    if (h.Type != type) continue;
                    h.Price = newPrice;
                    h.UpdateLabel();
                    h.IsSave = true;
                }
                Trigger.SendChatMessage(player, $"~r~Вы успешно установили цену для домов - {newPrice}");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_replaceHousePrices Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Delhouseowner)]
        public static void CMD_deleteHouseOwner(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Delhouseowner)) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                else if (sessionData.HouseID == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться на маркере недвижимости", 3000);
                    return;
                }

                var house = HouseManager.Houses.FirstOrDefault(h => h.ID == sessionData.HouseID);
                if (house == null) return;
                house.ClearOwner();
                Houses.Rieltagency.Repository.OnPayDay(new List<House>()
                {
                    house
                }, new List<Business>());
                if (house.Type != 7) GameLog.Admin($"{player.Name}", $"delHouseOwner({house.ID})", $"");
                else GameLog.Admin($"{player.Name}", $"delParkOwner({house.ID})", $"");
                Trigger.SendChatMessage(player, $"~r~Вы успешно очистили владельца у недвижимости {house.ID}");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_deleteHouseOwner Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Stt)]
        public static void CMD_SetTurboTorque(ExtPlayer player, float power = 0f, float torque = 1f)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Stt)) return;
                Trigger.ClientEvent(player, "svem", power, torque);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_SetTurboTorque Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Dmgmodif)]
        public static void CMD_SetDMGdif(ExtPlayer player, int id, float dmg)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Dmgmodif)) return;
                ExtPlayer target = Main.GetPlayerByID(id);
                if (!target.IsCharacterData())
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"DAMAGE MODIFIED TO: {dmg}", 3000);
                Trigger.ClientEvent(target, "dmgmodif", dmg);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_SetDMGdif Exception: {e.ToString()}");
            }
        }


        [Command(AdminCommands.Svm)]
        public static void CMD_SetVehicleMod(ExtPlayer player, int type, string index)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Svm)) return;
                if (!player.IsInVehicle) return;
                if (!int.TryParse(index, out int index1))
                {
                    if (type == 133722822) NAPI.Entity.SetEntityModel(player.Vehicle, NAPI.Util.GetHashKey(index));
                    else if (type == 133722823) player.Vehicle.NumberPlate = index;
                }
                else
                {
                    switch (type)
                    {
                        case 25:
                            player.Vehicle.NumberPlateStyle = index1;
                            break;
                        case 46:
                            player.Vehicle.WindowTint = index1;
                            break;
                        case 66:
                            player.Vehicle.PrimaryColor = index1;
                            break;
                        case 67:
                            player.Vehicle.SecondaryColor = index1;
                            break;
                        default:
                            player.Vehicle.SetMod(type, index1);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_SetVehicleMod Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Svn)]
        public static void CMD_SetVehicleNeon(ExtPlayer player, byte r, byte g, byte b, byte alpha)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Svn)) return;
                if (!player.IsInVehicle) return;
                var vehicle = (ExtVehicle) player.Vehicle;
                if (alpha != 0)
                {
                    NAPI.Vehicle.SetVehicleNeonState(vehicle, true);
                    NAPI.Vehicle.SetVehicleNeonColor(vehicle, r, g, b);
                }
                else
                {
                    NAPI.Vehicle.SetVehicleNeonColor(vehicle, 255, 255, 255);
                    NAPI.Vehicle.SetVehicleNeonState(vehicle, false);
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_SetVehicleNeon Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Svh)]
        public static void CMD_SetVehicleHealth(ExtPlayer player, int health = 100)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Svh)) return;
                if (!player.IsInVehicle) return;
                var vehicle = (ExtVehicle) player.Vehicle;
                vehicle.Repair();
                vehicle.Health = health;
            }
            catch (Exception e)
            {
                Log.Write($"CMD_SetVehicleHealth Exception: {e.ToString()}");
            }

        }
        [Command(AdminCommands.Delveh)]
        public static void CMD_deleteThisAdminCar(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Delveh)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!player.IsInVehicle) return;
                var vehicle = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if (vehicleLocalData.Access == VehicleAccess.Admin)
                    {
                        Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) удалил админ-машину ({vehicle.Value})");
                        VehicleStreaming.DeleteVehicle(vehicle);
                        GameLog.Admin($"{player.Name}", AdminCommands.Delveh, $"");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_deleteThisAdminCar Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Delvehid)]
        public static void CMD_deleteVehByID(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Delveh)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                ExtVehicle vehicle = null;

                foreach (var veh in RAGE.Entities.Vehicles.All.Cast<ExtVehicle>())
                {
                    if (veh.Value != id) continue;

                    if (veh.IsVehicleLocalData())
                    {
                        vehicle = veh;
                        break;
                    }
                }

                if (vehicle != null)
                {
                    var vehicleLocalData = vehicle.GetVehicleLocalData();
                    if (vehicleLocalData.Access == VehicleAccess.Admin)
                    {
                        Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) удалил админ-машину ({vehicle.Value})");
                        VehicleStreaming.DeleteVehicle(vehicle);
                        GameLog.Admin($"{player.Name}", AdminCommands.Delvehid, $"");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_deleteVehByID Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Delvehall)]
        public static void CMD_delMyCars(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Delvehall)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var vehiclesLocalData = RAGE.Entities.Vehicles.All.Cast<ExtVehicle>()
                    .Where(v => v.VehicleLocalData != null)
                    .Where(v => v.VehicleLocalData.Access == VehicleAccess.Admin)
                    .ToList();
                
                foreach (var vehicleAdmin in vehiclesLocalData)
                {
                    var vehicleLocalData = vehicleAdmin.GetVehicleLocalData();
                    if (vehicleLocalData != null)
                    {
                        if (vehicleLocalData.Access == VehicleAccess.Admin) 
                            VehicleStreaming.DeleteVehicle(vehicleAdmin);
                    }
                }
                Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) удалил все админ-машины");
                GameLog.Admin($"{player.Name}", AdminCommands.Delvehall, $"");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_delMyCars Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Delmyveh)]
        public static void CMD_Delmyveh(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Delmyveh)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var vehiclesLocalData = RAGE.Entities.Vehicles.All.Cast<ExtVehicle>()
                    .Where(v => v.VehicleLocalData != null)
                    .Where(v => v.VehicleLocalData.Access == VehicleAccess.Admin)
                    .Where(v => v.VehicleLocalData.By == player.Name)
                    .ToList();
                
                foreach (var vehicleAdmin in vehiclesLocalData)
                    VehicleStreaming.DeleteVehicle(vehicleAdmin);
                
                Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) удалил все свои админ-машины");
                GameLog.Admin($"{player.Name}", AdminCommands.Delmyveh, $"");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_delMyCars Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Save)]
        public static void CMD_saveCoord(ExtPlayer player, string name)
        {
            try
            {
                Admin.saveCoords(player, name);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_saveCoord Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Mypos)]
        public static void CMD_GetMyPosition(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Mypos)) return;
                Vector3 pos = player.Position;
                Vector3 rot = player.Rotation;
                if (player.IsInVehicle)
                {
                    var vehicle = (ExtVehicle) player.Vehicle;
                    pos = vehicle.Position;
                    rot = vehicle.Rotation;
                }
                Trigger.SendChatMessage(player, "~b~Position: " + pos.X + ", " + pos.Y + ", " + pos.Z);
                Trigger.SendChatMessage(player, "~b~Rotation: " + rot.X + ", " + rot.Y + ", " + rot.Z);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_GetMyPosition Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Setfractun)]
        public static void ACMD_setfractun(ExtPlayer player, int cat = -1, int id = -1) {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Setfractun)) return;
                if (!player.IsInVehicle)
                {
                    Trigger.SendChatMessage(player, "Вы должны сидеть в машине фракции, которую хотите изменить");
                    return;
                }
                var vehicle = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if (vehicleLocalData.Access == VehicleAccess.Fraction)
                    {
                        var fractionData = Manager.GetFractionData(vehicleLocalData.Fraction);
                        if (fractionData == null)
                            return;
                        
                        if (!fractionData.Vehicles.ContainsKey(vehicle.NumberPlate)) return;
                        
                        if (cat < 0)
                        {
                            VehicleManager.FracApplyCustomization(vehicle, fractionData.Id);
                            return;
                        }

                        string number = vehicle.NumberPlate;
                        FractionVehicleData oldtuple = fractionData.Vehicles[number];
                        string oldvehhash = oldtuple.model;
                        Vector3 oldvehpos = oldtuple.position;
                        Vector3 oldvehrot = oldtuple.rotation;
                        int oldvehrank = oldtuple.rank;
                        int oldvehc1 = oldtuple.color1;
                        int oldvehc2 = oldtuple.color2;
                        var oldvehdata = oldtuple.customization;
                        switch (cat)
                        {
                            case 0:
                                oldvehdata.Spoiler = id;
                                break;
                            case 1:
                                oldvehdata.FrontBumper = id;
                                break;
                            case 2:
                                oldvehdata.RearBumper = id;
                                break;
                            case 3:
                                oldvehdata.SideSkirt = id;
                                break;
                            case 4:
                                oldvehdata.Muffler = id;
                                break;
                            case 5:
                                oldvehdata.Wings = id;
                                break;
                            case 6:
                                oldvehdata.Roof = id;
                                break;
                            case 7:
                                oldvehdata.Hood = id;
                                break;
                            case 8:
                                oldvehdata.Vinyls = id;
                                break;
                            case 9:
                                oldvehdata.Lattice = id;
                                break;
                            case 10:
                                oldvehdata.Engine = id;
                                break;
                            case 11:
                                oldvehdata.Turbo = id;
                                bool turbo = (oldvehdata.Turbo == 0);
                                player.Vehicle.SetSharedData("TURBO", turbo);
                                break;
                            case 12:
                                oldvehdata.Horn = id;
                                break;
                            case 13:
                                oldvehdata.Transmission = id;
                                break;
                            case 14:
                                oldvehdata.WindowTint = id;
                                break;
                            case 15:
                                oldvehdata.Suspension = id;
                                break;
                            case 16:
                                oldvehdata.Brakes = id;
                                break;
                            case 17:
                                oldvehdata.Headlights = id;
                                break;
                            case 18:
                                oldvehdata.NumberPlate = id;
                                break;
                            case 19:
                                oldvehdata.NeonColor.Red = id;
                                break;
                            case 20:
                                oldvehdata.NeonColor.Green = id;
                                break;
                            case 21:
                                oldvehdata.NeonColor.Blue = id;
                                break;
                            case 22:
                                oldvehdata.NeonColor.Alpha = id;
                                break;
                            case 23:
                                oldvehdata.WheelsType = id;
                                break;
                            case 24:
                                oldvehdata.Wheels = id;
                                break;
                            case 25:
                                oldvehdata.WheelsColor = id;
                                break;
                            default:
                                // Not supposed to end up here. 
                                break;
                        }
                        fractionData.Vehicles[number] = new FractionVehicleData(oldvehhash, oldvehpos, oldvehrot, oldvehrank, oldtuple.defaultRank, oldvehc1, oldvehc2, oldvehdata);
                        Trigger.SetTask(async () =>
                        {
                            try
                            {
                                await using var db = new ServerBD("MainDB");//В отдельном потоке

                                await db.Fractionvehicles
                                    .Where(v => v.Number == number)
                                    .Set(v => v.Components, JsonConvert.SerializeObject(oldvehdata))
                                    .UpdateAsync();
                            }
                            catch (Exception e)
                            {
                                Debugs.Repository.Exception(e);
                            }
                        });
                        VehicleManager.GetVehicleCustomization(null, vehicle);
                        VehicleManager.FracApplyCustomization(vehicle, fractionData.Id);
                        Trigger.SendChatMessage(player, "Вы изменили тюнинг этой машины для фракции.");
                    }
                    else Trigger.SendChatMessage(player, "Вы должны сидеть в машине фракции, которую хотите изменить");
                }
            }
            catch (Exception e)
            {
                Log.Write($"ACMD_setfractun Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Setfracveh)]
        public static void ACMD_setfracveh(ExtPlayer player, string vehname, int rank, int c1, int c2) {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Setfracveh)) return;
                if (!player.IsInVehicle)
                {
                    Trigger.SendChatMessage(player, "Вы должны сидеть в машине фракции, которую хотите изменить");
                    return;
                }
                if (rank <= 0 || c1 < 0 || c1 >= 160 || c2 < 0 || c2 >= 160) return;
                vehname = vehname.ToLower();
                var vehicle = (ExtVehicle) player.Vehicle;
                SetFracVeh(player, vehicle, vehname, rank, c1, c2);
            }
            catch (Exception e)
            {
                Log.Write($"ACMD_setfracveh Exception: {e.ToString()}");
            }
        }

        public static void SetFracVehRank(ExtPlayer player, ExtVehicle vehicle, int rank)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {

                    if (vehicleLocalData.Access == VehicleAccess.Fraction)
                    {
                        var number = vehicle.NumberPlate;
                        var fractionData = Manager.GetFractionData(vehicleLocalData.Fraction);
                        if (fractionData == null)
                            return;
                        if (!fractionData.Vehicles.ContainsKey(number)) 
                            return;

                        vehicleLocalData.MinRank = rank;
                        fractionData.Vehicles[number].rank = rank;
  
                        Trigger.SetTask(async () =>
                        {
                            try
                            {
                                await using var db = new ServerBD("MainDB");//В отдельном потоке

                                await db.Fractionvehicles
                                    .Where(v => v.Number == number)
                                    .Set(v => v.Rank, rank)
                                    .UpdateAsync();
                            }
                            catch (Exception e)
                            {
                                Debugs.Repository.Exception(e);
                            }
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_stopServer Exception: {e.ToString()}");
            }
        }

        public static void SetFracVeh(ExtPlayer player, ExtVehicle vehicle, string vehname, int rank, int c1, int c2, bool isMessage = true)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if (vehicleLocalData.Access == VehicleAccess.Fraction)
                    {
                        var number = vehicle.NumberPlate;
                        var fractionData = Manager.GetFractionData(vehicleLocalData.Fraction);
                        if (fractionData == null)
                            return;
                        if (!fractionData.Vehicles.ContainsKey(number)) return;

                        bool canmats = (vehname.Equals("barracks") || vehname.Equals("cargobob") || vehname.Equals("brickade") || vehname.Equals("youga") || vehname.Equals("gburrito2") || vehname.Equals("burrito3") || vehname.Equals("gburrito") || vehname.Equals("terbyte"));
                        bool candrugs = (vehname.Equals("youga") || vehname.Equals("burrito3") || vehname.Equals("gburrito"));
                        bool canmeds = (vehname.Equals("ambulance") || vehname.Equals("vapidse") || vehname.Equals("rumpo2") || vehname.Equals("emsnspeedo") || vehname.Equals("emsroamer"));

                        
                        string model = fractionData.Vehicles[number].model;

                        vehicleLocalData.CanMats = canmats;
                        vehicleLocalData.CanDrugs = candrugs;
                        vehicleLocalData.CanMedKits = canmeds;
                        vehicleLocalData.MinRank = rank;

                        Vector3 pos = NAPI.Entity.GetEntityPosition(vehicle) + new Vector3(0, 0, 0.5);
                        Vector3 rot = NAPI.Entity.GetEntityRotation(vehicle);
                        var data = fractionData.Vehicles[number].customization;
                        if (!vehname.Equals(model))
                        {
                            data = new VehicleCustomization();
                            data.PrimModColor = c1;
                            data.SecModColor = c2;
                            NAPI.Entity.SetEntityModel(vehicle, NAPI.Util.GetHashKey(vehname));
                            VehicleManager.FracApplyCustomization(vehicle, fractionData.Id);
                        }
                        int defrank = fractionData.Vehicles[number].defaultRank;
                        fractionData.Vehicles[number] = new FractionVehicleData(model, pos, rot, rank, defrank, c1, c2, data);

                        Trigger.SetTask(async () =>
                        {
                            try
                            {
                                await using var db = new ServerBD("MainDB");//В отдельном потоке

                                await db.Fractionvehicles
                                    .Where(v => v.Number == number)
                                    .Set(v => v.Model, vehname)
                                    .Set(v => v.Position, JsonConvert.SerializeObject(pos))
                                    .Set(v => v.Rotation, JsonConvert.SerializeObject(rot))
                                    .Set(v => v.Rank, rank)
                                    .Set(v => v.Colorprim, c1)
                                    .Set(v => v.Colorsec, c2)
                                    .Set(v => v.Components, JsonConvert.SerializeObject(data))
                                    .UpdateAsync();
                            }
                            catch (Exception e)
                            {
                                Debugs.Repository.Exception(e);
                            }
                        });

                        vehicle.PrimaryColor = c1;
                        vehicle.SecondaryColor = c2;
                        if (isMessage) Trigger.SendChatMessage(player, "Вы изменили данные этой машины для фракции.");
                    }
                    else if (isMessage) Trigger.SendChatMessage(player, "Вы должны сидеть в машине фракции, которую хотите изменить");
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_stopServer Exception: {e.ToString()}");
            }
        }

        /*[Command(AdminCommands.Setvehcord)]
        public static void ACMD_setvehcord(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Setvehcord)) return;
                
                if (!player.IsInVehicle)
                {
                    Trigger.SendChatMessage(player, "Вы должны сидеть в транспорте, положение которого хотите изменить.");
                    return;
                }
                
                var vehicle = player.Vehicle;

                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {

                    if (vehicleLocalData.Access == VehicleAccess.WorkId)
                    {
                        string numb = vehicle.NumberPlate;
                        Vector3 pos = NAPI.Entity.GetEntityPosition(vehicle) + new Vector3(0, 0, 0.5);
                        Vector3 rot = NAPI.Entity.GetEntityRotation(vehicle);

                        using MySqlCommand cmd = new MySqlCommand
                        {
                            CommandText = "UPDATE `othervehicles` SET `position`=@pos,`rotation`=@rot WHERE `number`=@num"
                        };
                        
                        cmd.Parameters.AddWithValue("@pos", JsonConvert.SerializeObject(pos));
                        cmd.Parameters.AddWithValue("@rot", JsonConvert.SerializeObject(rot));
                        cmd.Parameters.AddWithValue("@num", numb);
                        
                        MySQL.Query(cmd);

                        switch (vehicleLocalData.WorkId)
                        {
                            case 7:
                                if (Jobs.Collector.CarInfos.Count > data1.Number)
                                {
                                    Jobs.Collector.CarInfos[data1.Number].Position = pos;
                                    Jobs.Collector.CarInfos[data1.Number].Rotation = rot;
                                }
                                break;
                            case 8:
                                if (Jobs.AutoMechanic.CarInfos.Count > data1.Number)
                                {
                                    Jobs.AutoMechanic.CarInfos[data1.Number].Position = pos;
                                    Jobs.AutoMechanic.CarInfos[data1.Number].Rotation = rot;
                                }
                                break;
                            default:
                                break;
                        }

                        Trigger.SendChatMessage(player, "Вы успешно изменили положение транспорта.");
                    }
                    else Trigger.SendChatMessage(player, "Вы должны сидеть в транспорте для работы, положение которого хотите изменить.");
                }
            }
            catch (Exception e)
            {
                Log.Write($"ACMD_setvehcord Exception: {e.ToString()}");
            }
        }*/


        [Command(AdminCommands.Restart)]
        public static void CMD_stopServer(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Restart)) return;
                Trigger.ClientEvent(player, "openDialog", "STOP_SERVER", $"Вы уверены, что хотите остановить сервер?");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_stopServer Exception: {e.ToString()}");
            }
        }
        
        [Command(AdminCommands.SaveServer)]
        public static void CMD_SaveServer(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.SaveServer)) return;
                Admin.SaveServer();
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Сервер сохранен", 1000);
                
            }
            catch (Exception e)
            {
                Log.Write($"CMD_SaveServer Exception: {e.ToString()}");
            }
        }
        
        [Command(AdminCommands.Payday)]
        public static void payDay(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Payday)) return;
                GameLog.Admin($"{player.Name}", $"payDay", "");
                Main.payDayTrigger(false);
            }
            catch (Exception e)
            {
                Log.Write($"payDay Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Setleader)]
        public static void CMD_setLeader(ExtPlayer player, int id, int fracid)
        {
            try
            {
                Admin.setFracLeader(player, Main.GetPlayerByID(id), fracid);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_setLeader Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Inv)]
        public static void CMD_inv(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Inv)) return;

                if (!player.OutgoingSyncDisabled)
                {
                    player.OutgoingSyncDisabled = true;
                    player.Position = new Vector3(99999f, 9999f, 9999f);
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Теперь игроки и читеры вас не видят.", 1000);
                }
                else
                {
                    player.OutgoingSyncDisabled = false;
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Теперь вас все видят.", 1000);
                }

                Trigger.ClientEvent(player, "clientSyncHandle", player.OutgoingSyncDisabled);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_inv Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Sp)]
        public static void CMD_spectateMode(ExtPlayer player, int id)
        {
            try
            {
                AdminSP.Spectate(player, id);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_spectateMode Exception: {e.ToString()}");
            }
        }
        [Command("usp")]
        public static void CMD_unspectateMode(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Sp)) return;
                AdminSP.UnSpectate(player);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_unspectateMode Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Spawn)]
        public static void CMD_Spawn(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Spawn)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                ExtPlayer target = Main.GetPlayerByID(id);
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) return;
                if (targetCharacterData.InsideHouseID == -1 && target.Dimension == 0 && targetCharacterData.DemorganTime <= 0 && targetCharacterData.ArrestTime <= 0 && !targetSessionData.CuffedData.Cuffed && !targetSessionData.DeathData.InDeath && targetSessionData.SitPos == -1)
                { 
                    target.Position = Customization.GetSpawnPos();
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы отправили игрока на спавн!", 3000);
                    //Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Администратор {player.Name} телепортировал Вас на спавн", 7000);
                    EventSys.SendCoolMsg(target,"Администрация", $"{player.Name}", $"Телепортировал Вас на спавн", "", 10000);
                    Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) отправил на спавн {target.Name} ({target.Value})");
                    GameLog.Admin($"{player.Name}", $"spawn", $"{target.Name}");
                } else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недоступно в данный момент.", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_Spawn Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Metp)]
        public static void CMD_teleportToMe(ExtPlayer player, int id)
        {
            try
            {
                Admin.teleportTargetToPlayer(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_teleportToMe Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Kill)]
        public static void CMD_kill(ExtPlayer player, int id)
        {
            try
            {
                Admin.killTarget(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_kill Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Revive)]
        public static void CMD_revive(ExtPlayer player, int id)
        {
            try
            {
                Admin.reviveTarget(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_revive Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Hp)]
        public static void CMD_adminHeal(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Hp)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) повысил себе здоровье");
                Admin.ReviveMe(player);
                NAPI.Player.SetPlayerHealth(player, 100);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_adminHeal Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Sethp)]
        public static void CMD_adminHeal(ExtPlayer player, int id, int hp)
        {
            try
            {
                Admin.healTarget(player, Main.GetPlayerByID(id), hp);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_adminHeal Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Delad)]
        public static void CMD_Delad(ExtPlayer player, int id, string reason)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Delad)) return;
                Fractions.LSNews.LsNewsSystem.UpdateAnswer(player, id, reason, true);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_adminHeal Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Setar)]
        public static void CMD_adminArmor(ExtPlayer player, int id, int ar)
        {
            try
            {
                Admin.armorTarget(player, Main.GetPlayerByID(id), ar);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_adminArmor Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Fz)]
        public static void CMD_adminFreeze(ExtPlayer player, int id)
        {
            try
            {
                Admin.freezeTarget(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_adminFreeze Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.UnFz)]
        public static void CMD_adminUnFreeze(ExtPlayer player, int id)
        {
            try
            {
                Admin.unFreezeTarget(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_adminUnFreeze Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Setadmin)]
        public static void CMD_setAdmin(ExtPlayer player, int id)
        {
            try
            {
                Admin.setPlayerAdminGroup(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_setAdmin Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Offdeladmin)]
        public static void CMD_OffDelAdmin(ExtPlayer player, string target)
        {
            try
            {
                Admin.OffdelPlayerAdminGroup(player, target);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_OffDelAdmin Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Deladmin)]
        public static void CMD_delAdmin(ExtPlayer player, int id)
        {
            try
            {
                Admin.delPlayerAdminGroup(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_delAdmin Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Arank)]
        public static void CMD_setAdminRank(ExtPlayer player, int id, int rank)
        {
            try
            {
                Admin.setPlayerAdminRank(player, Main.GetPlayerByID(id), rank);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_setAdminRank Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Offarank)]
        public static void CMD_offSetAdminRank(ExtPlayer player, string target, int rank)
        {
            try
            {
                Admin.offSetPlayerAdminRank(player, target, rank);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_offSetAdminRank Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Givegun)]
        public static void CMD_adminGuns(ExtPlayer player, int id, string wname, string serial)
        {
            try
            {
                Admin.giveTargetGun(player, Main.GetPlayerByID(id), wname, serial);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_adminGuns Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Carcoupon)]
        public static void CMD_adminCars(ExtPlayer player, int id, string vehicle)
        {
            try
            {
                Admin.giveTargetCar(player, Main.GetPlayerByID(id), vehicle);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_adminGuns Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Giveclothes)]
        public static void CMD_adminClothes(ExtPlayer player, int id, string wname, string serial)
        {
            try
            {
                Admin.giveTargetClothes(player, Main.GetPlayerByID(id), wname, serial);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_adminClothes Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Setskin)]
        public static void CMD_adminSetSkin(ExtPlayer player, int id, string pedModel)
        {
            try
            {
                Admin.giveTargetSkin(player, Main.GetPlayerByID(id), pedModel);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_adminSetSkin Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Delgun)]
        public static void CMD_adminOGuns(ExtPlayer player, int id)
        {
            try
            {
                Admin.takeTargetGun(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_adminOGuns Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Givemoney)]
        public static void CMD_adminGiveMoney(ExtPlayer player, int id, int money)
        {
            try
            {
                ExtPlayer target = Main.GetPlayerByID(id);
                if (!target.IsCharacterData()) Admin.giveBankMoney(player, id, money);
                else Admin.giveMoney(player, target, money);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_adminGiveMoney Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Offgivemoney)]
        public static void CMD_adminOffGiveMoney(ExtPlayer player, string name, int money)
        {
            try
            {
                Admin.OffGiveMoney(player, name, money);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_adminOffGiveMoney Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Delleader)]
        public static void CMD_delleader(ExtPlayer player, int id)
        {
            try
            {
                Admin.delFracLeader(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_delleader Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Deljob)]
        public static void CMD_deljob(ExtPlayer player, int id)
        {
            try
            {
                Admin.delJob(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_deljob Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Veh)]
        public static void CMD_createVehicle(ExtPlayer player, string name, int a, int b)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Veh)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                ExtVehicle veh;
                var vehHash = (uint)NAPI.Util.VehicleNameToModel(name);
                if (vehHash != 0) veh = VehicleStreaming.CreateVehicle(vehHash, player.Position, player.Rotation.Z, 0, 0, acc: VehicleAccess.Admin, by: player.Name, petrol: 9999, engine: true);
                else
                {
                    if (characterData.AdminLVL <= 5)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Транспорта с таким названием не было найдено", 3000);
                        return;
                    }
                    else veh = VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey(name), player.Position, player.Rotation.Z, 0, 0, acc: VehicleAccess.Admin, by: player.Name, petrol: 9999, engine: true);
                }
                Trigger.Dimension(veh, UpdateData.GetPlayerDimension(player));
                veh.NumberPlate = "ADMIN";
                veh.PrimaryColor = a;
                veh.SecondaryColor = b;
                Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) создал машину({name},{veh.Value})");
                GameLog.Admin($"{player.Name}", $"vehCreate({name})", $"");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_createVehicle Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Vehs)]
        public static void CMD_createVehicles(ExtPlayer player, string name, int a, int b, byte count)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Vehs)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (count >= 1 && count <= 20)
                {
                    var vehHash = (uint)NAPI.Util.VehicleNameToModel(name);
                    if (vehHash != 0)
                    {
                        for (byte i = 0; i != count; i++)
                        {
                            var veh = (ExtVehicle) VehicleStreaming.CreateVehicle(vehHash, new Vector3((player.Position.X + (4 * i)), player.Position.Y, player.Position.Z), player.Rotation.Z, a, b, "ADMIN", acc: VehicleAccess.Admin, by: player.Name, petrol: 9999, engine: true);
                            Trigger.Dimension(veh, UpdateData.GetPlayerDimension(player));
                            veh.NumberPlate = "ADMIN";
                            veh.PrimaryColor = a;
                            veh.SecondaryColor = b;
                        }
                        Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) создал {count} машин({name})");
                        GameLog.Admin($"{player.Name}", $"vehsCreate({count} {name})", $"");
                    }
                    else
                    {
                        if (characterData.AdminLVL <= 5) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Транспорта с таким названием не было найдено", 3000);
                        else
                        {
                            uint model = NAPI.Util.GetHashKey(name);
                            for (byte i = 0; i != count; i++)
                            {
                                var veh = (ExtVehicle) VehicleStreaming.CreateVehicle(model, new Vector3((player.Position.X + (4 * i)), player.Position.Y, player.Position.Z), player.Rotation.Z, a, b, "ADMIN", acc: VehicleAccess.Admin, by: player.Name, petrol: 9999, engine: true);
                                Trigger.Dimension(veh, UpdateData.GetPlayerDimension(player));
                                veh.NumberPlate = "ADMIN";
                                veh.PrimaryColor = a;
                                veh.SecondaryColor = b;
                            }
                            Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) создал {count} машин({name})");
                            GameLog.Admin($"{player.Name}", $"vehsCreate({count} {name})", $"");
                        }
                        return;
                    }
                }
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Одновременно создать можно до 20 машин", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_createVehicles Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Aclear, GreedyArg = true)]
        public static void ACMD_aclear(ExtPlayer player, string target, string reason) {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Aclear)) return;
                aclear(player, target, reason);
            }
            catch (Exception e)
            {
                Log.Write($"ACMD_aclear Exception: {e.ToString()}");
            }
        }
        public static void aclear(ExtPlayer player, string targetName, string reason)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
                
                if (!Main.PlayerNames.Values.Contains(targetName) || !Main.PlayerUUIDs.ContainsKey(targetName))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Человек с таким именем не найден", 3000);
                    return;
                }
                ExtPlayer target = (ExtPlayer) NAPI.Player.GetPlayerFromName(targetName);
                if (target.IsCharacterData())
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Невозможно очистить персонажа, который находится в игре", 3000);
                    return;
                }

                if (Main.stringGlobalBlock.Any(c => reason.Contains(c)))
                {
                    Trigger.SendToAdmins(1, $"{ChatColors.Red}[A] {sessionData.Name} ({sessionData.Value}) был снят системой за причину в наказании: {reason}");
                    Character.BindConfig.Repository.DeleteAdmin(player);
                    return;
                }
                if (!Admin.CheckMe(player, 5)) return;
                int tuuid = Main.PlayerUUIDs[targetName];
                
                Trigger.SetTask(() =>
                {
                    try
                    {
                        // CLEAR BIZ

                        using MySqlCommand cmd1 = new MySqlCommand
                        {
                            CommandText = "SELECT adminlvl,biz FROM `characters` WHERE uuid=@val0"
                        };
                        cmd1.Parameters.AddWithValue("@val0", tuuid);

                        using DataTable result = MySQL.QueryRead(cmd1);
                        if (result != null && result.Rows.Count != 0)
                        {
                            DataRow row = result.Rows[0];
                            if (Convert.ToInt32(row["adminlvl"]) >= characterData.AdminLVL)
                            {
                                Trigger.SendToAdmins(3, "!{#FF0000}" + $"[A] {sessionData.Name} ({sessionData.Value}) попытался очистить {targetName} (offline), который имеет выше уровень администратора.");
                                return;
                            }
                            List<int> TBiz = JsonConvert.DeserializeObject<List<int>>(row["biz"].ToString());
                            if (TBiz.Count >= 1 && TBiz[0] >= 1)
                            {
                                Business biz = BusinessManager.BizList[TBiz[0]];
                                string owner = biz.Owner;

                                if (Main.PlayerUUIDs.ContainsKey(owner))
                                {
                                    int bizownerUuid = Main.PlayerUUIDs[owner];
                                    using MySqlCommand cmd2 = new MySqlCommand
                                    {
                                        CommandText = "SELECT biz,money FROM characters WHERE uuid=@val0"
                                    };
                                    cmd2.Parameters.AddWithValue("@val0", bizownerUuid);

                                    using DataTable data = MySQL.QueryRead(cmd2);
                                    List<int> ownerBizs = new List<int>();
                                    int money = 0;

                                    foreach (DataRow Row in data.Rows)
                                    {
                                        ownerBizs = JsonConvert.DeserializeObject<List<int>>(Row["biz"].ToString());
                                        money = Convert.ToInt32(Row["money"]);
                                    }

                                    ownerBizs.Remove(biz.ID);

                                    using MySqlCommand cmd3 = new MySqlCommand
                                    {
                                        CommandText = "UPDATE characters SET biz=@val0,money=@val1 WHERE uuid=@val2"
                                    };
                                    cmd3.Parameters.AddWithValue("@val0", JsonConvert.SerializeObject(ownerBizs));
                                    cmd3.Parameters.AddWithValue("@val1", money + Convert.ToInt32(biz.SellPrice * 0.8));
                                    cmd3.Parameters.AddWithValue("@val2", bizownerUuid);
                                    MySQL.Query(cmd3);
                                }

                                var bizBalance = Bank.Accounts[biz.BankID];
                                bizBalance.Balance = 0;
                                bizBalance.IsSave = true;
                                
                                biz.ClearOwner();
                                
                                Houses.Rieltagency.Repository.OnPayDay(new List<House>(), new List<Business>()
                                {
                                    biz
                                });
                                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы отобрали бизнес у {owner}", 3000);
                            }
                        }
                        else
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Не удалось найти персонажа в базе данных", 3000);
                            return;
                        }
                        // CLEAR HOUSE
                        using MySqlCommand cmd4 = new MySqlCommand
                        {
                            CommandText = "SELECT id FROM `houses` WHERE `owner`=@val0"
                        };
                        cmd4.Parameters.AddWithValue("@val0", targetName);
                        using DataTable result4 = MySQL.QueryRead(cmd4);
                        if (result4 != null && result4.Rows.Count != 0)
                        {
                            int hid = Convert.ToInt32(result4.Rows[0][0]);
                            var house = HouseManager.Houses.FirstOrDefault(h => h.ID == hid);
                            if (house != null)
                            {
                                if (FurnitureManager.HouseFurnitures.ContainsKey(hid))
                                {
                                    house.DestroyFurnitures();
                                    var furnitures = FurnitureManager.HouseFurnitures[hid];
                                    foreach (KeyValuePair<int, HouseFurniture> p in furnitures)
                                    {
                                        if (Chars.Repository.ItemsData.ContainsKey($"furniture_{hid}_{p.Key}") && Chars.Repository.ItemsData[$"furniture_{hid}_{p.Key}"].ContainsKey("furniture") && Chars.Repository.ItemsData[$"furniture_{hid}_{p.Key}"]["furniture"].Count != 0)
                                        {
                                            Chars.Repository.RemoveAll($"furniture_{hid}_{p.Key}");
                                        }
                                    }
                                    furnitures = new Dictionary<int, HouseFurniture>();
                                    house.IsFurnitureSave = true;
                                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы удалили из дома {targetName} всю мебель", 3000);
                                }
                                house.ClearOwner();
                                Houses.Rieltagency.Repository.OnPayDay(new List<House>()
                                {
                                    house
                                }, new List<Business>());
                                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы отобрали недвижимость у {targetName}", 3000);
                                //Chars.Repository.PlayerStats(player);
                            }
                        }
                        // CLEAR VEHICLES
                        var vehiclesNumber = VehicleManager.GetVehiclesCarNumberToPlayer(targetName);
                        if (vehiclesNumber.Count > 0)
                        {
                            foreach (string number in vehiclesNumber)
                            {
                                if (!VehicleManager.IsVehicleToNumber(number)) continue;
                                VehicleManager.Remove(number);
                            }
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы отобрали у {targetName} все машины.", 3000);
                        }
                        // CLEAR MONEY, HOTEL, FRACTION, SIMCARD, PET

                        using MySqlCommand cmd6 = new MySqlCommand
                        {
                            CommandText = "UPDATE `characters` SET `money`=0,`fraction`=0,`fractionlvl`=0,`hotel`=-1,`hotelleft`=0,`sim`=-1,`PetName`='null' WHERE uuid=@val0"
                        };
                        cmd6.Parameters.AddWithValue("@val0", tuuid);
                        MySQL.Query(cmd6);
                        // CLEAR BANK MONEY
                        Bank.Data bankAcc = Bank.Get(Main.PlayerBankAccs[targetName]);
                        if (bankAcc != null)
                        {
                            bankAcc.Balance = 0;
                            bankAcc.IsSave = true;
                        }
                        // CLEAR REDBUCKS
                        using MySqlCommand cmd8 = new MySqlCommand
                        {
                            CommandText = $"UPDATE accounts SET redbucks=0 WHERE character1=@val0 OR character2=@val0 OR character3=@val0"
                        };
                        cmd8.Parameters.AddWithValue("@val0", tuuid);
                        MySQL.Query(cmd8);
                        // CLEAR ITEMS
                        if (tuuid != 0) Chars.Repository.RemoveAll($"char_{tuuid}");
                        Trigger.SendPunishment($"{CommandsAccess.AdminPrefix}{sessionData.Name} полностью обнулил персонажа {targetName}. Причина: {reason}");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы забрали у человека все вещи, деньги с рук и банковского счёта у {targetName}", 3000);
                        GameLog.Admin($"{sessionData.Name}", $"aClear({reason})", $"{targetName}");
                    }
                    catch (Exception e)
                    {
                        Log.Write($"aclear SetTask Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Log.Write($"ACMD_aclear Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Fclear)]
        public static void CMD_ClearFraction(ExtPlayer player, byte fraction)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Fclear)) return;
                if (fraction <= (int)Fractions.Models.Fractions.None || fraction > Fractions.Configs.FractionCount) return;
                GameLog.FracLog(fraction, -1, -1, player.Name, "-1", "fClear");
                Notify.Send(player, NotifyType.Success, NotifyPosition.Center, $"Вы начали очищение фракции {Manager.FractionNames[fraction]}!", 3000);

                SyncThread.FClearBackground(player, fraction);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_ClearFraction Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Carnumber)]
        public static void CMD_Carnumber(ExtPlayer player, string number) {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Carnumber)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (number.Length > 8)
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Количество символов в номерном знаке не может превышать 8.", 3000);
                    return;
                }
                var vehicleData = VehicleManager.GetVehicleToNumber(number);
                if (vehicleData != null)
                {
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Номер машины: {number} | Модель: {vehicleData.Model} | Владелец: {vehicleData.Holder}", 6000);
                    Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) проверяет владельца машины {number} ({vehicleData.Holder})");
                }
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Не найдено машины с таким номерным знаком.", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_Carnumber Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Vehcustom)]
        public static void CMD_ApplyCustom(ExtPlayer player, int cat = -1, int id = -1)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Vehcustom)) return;
                if (!player.IsInVehicle) return;
                var vehicle = (ExtVehicle) player.Vehicle;
                var number = vehicle.NumberPlate;
                var vehicleData = VehicleManager.GetVehicleToNumber(number);
                if (vehicleData == null) return;
                if (cat < 0)
                {
                    VehicleManager.ApplyCustomization(vehicle);
                    VehicleManager.GetVehicleCustomization(player, vehicle);
                    return;
                }
                switch (cat)
                {
                    case 0:
                        vehicleData.Components.Muffler = id;
                        break;
                    case 1:
                        vehicleData.Components.SideSkirt = id;
                        break;
                    case 2:
                        vehicleData.Components.Hood = id;
                        break;
                    case 3:
                        vehicleData.Components.Spoiler = id;
                        break;
                    case 4:
                        vehicleData.Components.Lattice = id;
                        break;
                    case 5:
                        vehicleData.Components.Wings = id;
                        break;
                    case 6:
                        vehicleData.Components.Roof = id;
                        break;
                    case 7:
                        vehicleData.Components.Vinyls = id;
                        break;
                    case 8:
                        vehicleData.Components.FrontBumper = id;
                        break;
                    case 9:
                        vehicleData.Components.RearBumper = id;
                        break;
                    case 10:
                        vehicleData.Components.Engine = id;
                        break;
                    case 11:
                        vehicleData.Components.Turbo = id;
                        bool turbo = (vehicleData.Components.Turbo == 0);
                        player.Vehicle.SetSharedData("TURBO", turbo);
                        break;
                    case 12:
                        vehicleData.Components.Horn = id;
                        break;
                    case 13:
                        vehicleData.Components.Transmission = id;
                        break;
                    case 14:
                        vehicleData.Components.WindowTint = id;
                        break;
                    case 15:
                        vehicleData.Components.Suspension = id;
                        break;
                    case 16:
                        vehicleData.Components.Brakes = id;
                        break;
                    case 17:
                        vehicleData.Components.Headlights = id;
                        break;
                    case 18:
                        vehicleData.Components.NumberPlate = id;
                        break;
                    case 19:
                        vehicleData.Components.NeonColor.Red = id;
                        break;
                    case 20:
                        vehicleData.Components.NeonColor.Green = id;
                        break;
                    case 21:
                        vehicleData.Components.NeonColor.Blue = id;
                        break;
                    case 22:
                        vehicleData.Components.NeonColor.Alpha = id;
                        break;
                    case 23:
                        vehicleData.Components.WheelsType = id;
                        break;
                    case 24:
                        vehicleData.Components.Wheels = id;
                        break;
                    case 25:
                        vehicleData.Components.WheelsColor = id;
                        break;
                    default:
                        return;
                }
                VehicleManager.ApplyCustomization(vehicle);
                VehicleManager.GetVehicleCustomization(player, vehicle);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_ApplyCustom Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Vehcustompcolor)]
        public static void CMD_ApplyCustomPColor(ExtPlayer player, int r, int g, int b, int mod = -1)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Vehcustompcolor)) return;
                var vehicle = (ExtVehicle) player.Vehicle;
                var number = vehicle.NumberPlate;
                var vehicleData = VehicleManager.GetVehicleToNumber(number);
                if (vehicleData == null) return;
                if (mod != -1) vehicleData.Components.PrimModColor = mod;
                else
                {
                    vehicleData.Components.PrimModColor = -1;
                    Color color = new Color(r, g, b);
                    vehicleData.Components.PrimColor = color;
                }
                VehicleManager.ApplyCustomization(vehicle);
                VehicleManager.GetVehicleCustomization(player, vehicle);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_ApplyCustomPColor Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Vehcustomscolor)]
        public static void CMD_ApplyCustomSColor(ExtPlayer player, int r, int g, int b, int mod = -1)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Vehcustomscolor)) return;
                var vehicle = (ExtVehicle) player.Vehicle;
                var number = vehicle.NumberPlate;
                var vehicleData = VehicleManager.GetVehicleToNumber(number);
                if (vehicleData == null) return;
                if (mod != -1) vehicleData.Components.SecModColor = mod;
                else
                {
                    vehicleData.Components.SecModColor = -1;
                    Color color = new Color(r, g, b);
                    vehicleData.Components.SecColor = color;
                }
                VehicleManager.ApplyCustomization(vehicle);
                VehicleManager.GetVehicleCustomization(player, vehicle);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_ApplyCustomSColor Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Sl)]
        public static void CMD_setWeather(ExtPlayer player, bool toggle)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Sl)) return;
                Trigger.ClientEventForAll("setWorldLights", toggle);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_setWeather Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Sw)]
        public static void CMD_setWeather(ExtPlayer player, int weather)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Sw)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                if (!World.Weather.Repository.Update(weather)) 
                    return;
                
                Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) изменил погоду({weather})");
                
                //World.Weather.Repository.Update(weather);
                GameLog.Admin($"{player.Name}", $"setWeather({weather})", $"");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_setWeather Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.St)]
        public static void CMD_setTime(ExtPlayer player, short hours, short minutes, short seconds)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.St)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (hours >= 24 || minutes >= 60 || seconds >= 60 || hours <= -2 || minutes <= -2 || seconds <= -2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Некорректные данные, часы 0-23, минуты 0-59, секунды 0-59", 3000);
                    return;
                }
                Admin.TimeChanged = false;
                Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) установил игровое время({hours} {minutes} {seconds})");
                Trigger.ClientEventForAll("setTimeCmd", hours, minutes, seconds);
                if (hours != -1 && minutes != -1 && seconds != -1)
                {
                    Admin.TimeChanged = true;
                    Admin.SetTime[0] = hours;
                    Admin.SetTime[1] = minutes;
                    Admin.SetTime[2] = seconds;
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы заморозили игровое время на {hours}ч. {minutes}м. {seconds}с. Чтобы вернуться, установите -1 -1 -1", 3000);
                    GameLog.Admin($"{player.Name}", $"setTime({hours} {minutes} {seconds})", $"");
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_setTime Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.DelObjects)]
        public static void CMD_DelObjects(ExtPlayer player, string locationName)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.DelObjects)) return;

                if (!NeptuneEvo.Chars.Repository.ItemsData.ContainsKey(locationName))
                {
                    return;
                }
                    
                var sessionData = player.GetSessionData();
                if (sessionData == null)
                    return;

                sessionData.DelObjects = locationName;
                    
                Trigger.ClientEvent(player, "openDialog", "DelObjects", $"Вы хотите удалить {locationName}?");
                
            }
            catch (Exception e)
            {
                Log.Write($"CMD_teleport Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Tp)]
        public static void CMD_teleport(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Tp)) return;

                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                ExtPlayer target = Main.GetPlayerByID(id);
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }
                if (targetCharacterData.AdminLVL >= 6)
                {
                    var targetAdminConfig = targetCharacterData.ConfigData.AdminOption;
                    if (targetAdminConfig.HideMe && characterData.AdminLVL < targetCharacterData.AdminLVL)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                        Notify.Send(target, NotifyType.Alert, NotifyPosition.BottomCenter, $"{player.Name} ({player.Value}) попытался телепортироваться к Вам (/tp)", 3000);
                        return;
                    }
                }
                Admin.AdminsLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) телепортировался к {target.Name} ({target.Value})", 1, "#636363");
                GameLog.Admin($"{player.Name}", $"tp({target.Position.X}, {target.Position.Y}, {target.Position.Z})", $"{target.Name}");

                NAPI.Entity.SetEntityPosition(player, target.Position + new Vector3(2.5f, 2.5f, 0f));
                Trigger.Dimension(player, NAPI.Entity.GetEntityDimension(target));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_teleport Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Metpcar)]
        public static void CMD_Metpcar(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Metpcar)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var vehs = RAGE.Entities.Vehicles.All.Cast<ExtVehicle>();
                foreach (ExtVehicle veh in vehs)
                {
                    if (veh.Value != id) continue;
                    Admin.AdminsLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) телепортировал машину к себе {veh.DisplayName} ({id})", 1, "#636363", hideAdminLevel: 9);
                    NAPI.Entity.SetEntityPosition(veh, player.Position);
                    NAPI.Entity.SetEntityRotation(veh, new Vector3(0, 0, veh.Rotation.Z));
                    GameLog.Admin($"{player.Name}", $"tpcar({id})", $"{veh.DisplayName}");
                    break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_tpcar Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Tpcar)]
        public static void CMD_tpTocar(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Tpcar)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var vehs = RAGE.Entities.Vehicles.All.Cast<ExtVehicle>();
                foreach (ExtVehicle veh in vehs)
                {
                    if (veh.Value != id) continue;
                    Admin.AdminsLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) телепортировался к машине {veh.DisplayName} ({id})", 1, "#636363", hideAdminLevel: 9);
                    NAPI.Entity.SetEntityPosition(player, veh.Position);
                    Trigger.Dimension(player, UpdateData.GetVehicleDimension(veh));
                    GameLog.Admin($"{player.Name}", $"tptocar({id})", $"{veh.DisplayName}");
                    break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_tpTocar Exception: {e.ToString()}");
            }
        }
        
        [Command(AdminCommands.Tpcarnumber)]
        public static void CMD_tpcarnumber(ExtPlayer player, string veh_number)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Tpcarnumber)) return;

                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                var vehs = RAGE.Entities.Vehicles.All.Cast<ExtVehicle>();
                foreach (ExtVehicle veh in vehs)
                {
                    if (veh.NumberPlate == null || veh.NumberPlate != veh_number) continue;
                    Admin.AdminsLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) телепортировался к машине {veh.DisplayName} (ID: {veh.Value}; Number: {veh.NumberPlate})", 1, "#636363", hideAdminLevel: 9);
                    NAPI.Entity.SetEntityPosition(player, veh.Position);
                    Trigger.Dimension(player, UpdateData.GetVehicleDimension(veh));
                    GameLog.Admin($"{player.Name}", $"tpcarnumber({veh_number})", $"{veh.DisplayName}");
                    break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_tpcarnumber Exception: {e.ToString()}");
            }
        }
        
        [Command(AdminCommands.Setdimcar)]
        public static void CMD_Setdimcar(ExtPlayer player, int id, int newDimensionNumber)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Setdimcar)) return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                var vehs = RAGE.Entities.Vehicles.All.Cast<ExtVehicle>();
                foreach (ExtVehicle veh in vehs)
                {
                    if (veh.Value != id) continue;
                    Admin.AdminsLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) изменил dimension машины {veh.DisplayName} ({id}) на {newDimensionNumber}.", 1, "#636363", hideAdminLevel: 9);
                    veh.Dimension = (uint)newDimensionNumber;
                    GameLog.Admin($"{player.Name}", $"setdimcar({id},{newDimensionNumber})", $"{veh.DisplayName}");
                    break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_Setdimcar Exception: {e.ToString()}");
            }
        }
        
        [Command(AdminCommands.Crimeban, "Используйте: /crimeban [ID игрока] [Причина]", GreedyArg = true)]
        public static void BanCrime(ExtPlayer player, int pid, string reason)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Crimeban)) return;
                
                ExtPlayer target = Main.GetPlayerByID(pid);
                var targetCharacterData = target.GetCharacterData();
                if (target != null && targetCharacterData != null)
                {
                    if (targetCharacterData.IsBannedCrime == true)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок уже заблокирован в системах capture и bizwar.", 5000);
                        return;
                    }
                    
                    Trigger.SendPunishment($"~r~{CommandsAccess.AdminPrefix}{player.Name} ограничил доступ к системам capture и bizwar персонажу {target.Name}({target.Value}) навсегда. Причина: {reason}.", target);
                    GameLog.Admin($"{player.Name}", $"crimeban({reason})", $"{target.Name}");
                    targetCharacterData.IsBannedCrime = true;
                    targetCharacterData.BanCrimeReason = reason;
                }
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрока с таким ID не было найдено.", 5000);
            }
            catch (Exception e)
            {
                Log.Write($"BanCrime Exception: {e.ToString()}");
            }
        }
        
        [Command(AdminCommands.Uncrimeban, "Используйте: /uncrimeban [ID игрока]")]
        public static void Uncrimeban(ExtPlayer player, int pid)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Uncrimeban)) return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                ExtPlayer target = Main.GetPlayerByID(pid);
                var targetCharacterData = target.GetCharacterData();
                if (target != null && targetCharacterData != null)
                {
                    if (targetCharacterData.IsBannedCrime == false)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок не заблокирован в системах capture и bizwar.", 5000);
                        return;
                    }
                    
                    Trigger.SendToAdmins(1, $"~r~[A] {player.Name} ({player.Value}) разблокировал в системах capture и bizwar персонажа {target.Name} ({target.Value})");
                    Trigger.SendChatMessage(target, $"Администратор {player.Name} разблокировал вас в системах capture и bizwar.");
                    targetCharacterData.IsBannedCrime = false;
                }
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрока с таким ID не было найдено.", 5000);
            }
            catch (Exception e)
            {
                Log.Write($"Uncrimeban Exception: {e.ToString()}");
            }
        }
        
        [Command(AdminCommands.Offcrimeban, "Используйте: /offcrimeban [Имя_Фамилия] [Причина]", GreedyArg = true)]
        public static void OffBanCrime(ExtPlayer player, string name, string reason)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Offcrimeban)) return;
                if (player.Name == name) return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                ExtPlayer target = (ExtPlayer) NAPI.Player.GetPlayerFromName(name);
                if (target.IsCharacterData())
                {
                    BanCrime(player, target.Value, reason);
                }
                else
                {
                    if (!Main.PlayerUUIDs.ContainsKey(name))
                    {
                        Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindMan), 3000);
                        return;
                    }
                    var targetUuid = Main.PlayerUUIDs[name];
                    Trigger.SetTask(async () =>
                    {
                        try
                        {
                            await using var db = new ServerBD("MainDB");//В отдельном потоке

                            var character = await db.Characters
                                .Select(c => new
                                {
                                    c.Uuid,
                                    c.IsBannedCrime
                                })
                                .Where(v => v.Uuid == targetUuid)
                                .FirstOrDefaultAsync();

                            if (character == null)
                            {
                                Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindMan), 3000);
                                return;
                            }
                            if (character.IsBannedCrime)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Персонаж уже заблокирован в системах capture и bizwar.", 3000);
                                return;
                            }

                            Trigger.SendPunishment(
                                $"~r~{CommandsAccess.AdminPrefix}{sessionData.Name} ограничил доступ к системам capture и bizwar персонажу {name} в оффлайне навсегда. Причина: {reason}.");
                            GameLog.Admin($"{sessionData.Name}", $"crimeban({reason})", $"{name}");

                            await db.Characters
                                .Where(c => c.Uuid == character.Uuid)
                                .Set(c => c.IsBannedCrime, true)
                                .Set(c => c.BanCrimeReason, reason)
                                .UpdateAsync();
                        }
                        catch (Exception e)
                        {
                            Log.Write($"Offcrimeban SetTask Exception: {e.ToString()}");
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Log.Write($"OffBanCrime Exception: {e.ToString()}");
            }
        }
        
        [Command(AdminCommands.Offuncrimeban, "Используйте: /offuncrimeban [Имя_Фамилия]")]
        public static void OffUnbanCrime(ExtPlayer player, string name)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Offcrimeban)) return;
                if (player.Name == name) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                ExtPlayer target = (ExtPlayer) NAPI.Player.GetPlayerFromName(name);
                if (target.IsCharacterData())
                {
                    Uncrimeban(player, target.Value);
                }
                else
                {
                    if (!Main.PlayerUUIDs.ContainsKey(name))
                    {
                        Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindMan), 3000);
                        return;
                    }
                    var targetUuid = Main.PlayerUUIDs[name];
                    Trigger.SetTask(async () =>
                    {
                        try
                        {
                            await using var db = new ServerBD("MainDB");//В отдельном потоке

                            var character = await db.Characters
                                .Select(c => new
                                {
                                    c.Uuid,
                                    c.IsBannedCrime
                                })
                                .Where(v => v.Uuid == targetUuid)
                                .FirstOrDefaultAsync();

                            if (character == null)
                            {
                                Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindMan), 3000);
                                return;
                            }
                            if (!character.IsBannedCrime)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Персонаж не заблокирован в системах capture и bizwar.", 3000);
                                return;
                            }

                            Trigger.SendToAdmins(1, LangFunc.GetText(LangType.Ru, DataName.UnblockBizAdmin, CommandsAccess.AdminPrefix, sessionData.Name, name));
                            GameLog.Admin($"{sessionData.Name}", $"offuncrimeban()", $"{name}");

                            await db.Characters
                                .Where(c => c.Uuid == character.Uuid)
                                .Set(c => c.IsBannedCrime, false)
                                .UpdateAsync();
                        }
                        catch (Exception e)
                        {
                            Log.Write($"Offuncrimeban SetTask Exception: {e.ToString()}");
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Log.Write($"OffUnbanCrime Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Flip)]
        public static void CMD_flipveh(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Flip)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var vehs = RAGE.Entities.Vehicles.All.Cast<ExtVehicle>();
                foreach (ExtVehicle veh in vehs)
                {
                    if (veh.Value != id) continue;
                    Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) перевернул машину {veh.DisplayName} ({id})");
                    NAPI.Entity.SetEntityPosition(veh, veh.Position + new Vector3(0, 0, 2.5f));
                    NAPI.Entity.SetEntityRotation(veh, new Vector3(0, 0, veh.Rotation.Z));
                    GameLog.Admin($"{player.Name}", $"flipVeh({id})", $"{veh.DisplayName}");
                    break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_flipveh Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Createbusiness)]
        public static void CMD_createBiz(ExtPlayer player, int govPrice, int type, double taxes = 0.026)
        {
            try
            {
                BusinessManager.createBusinessCommand(player, govPrice, type, taxes);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_createBiz Exception: {e.ToString()}");
            }
        }
        [Command("createunloadpoint")]
        public static void CMD_createUnloadPoint(ExtPlayer player, int bizid)
        {
            try
            {
                BusinessManager.createBusinessUnloadpoint(player, bizid);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_createUnloadPoint Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Deletebusiness)]
        public static void CMD_deleteBiz(ExtPlayer player, int bizid)
        {
            try
            {
                BusinessManager.deleteBusinessCommand(player, bizid);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_deleteBiz Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Changebiztax)]
        public static void CMD_changeBizTax(ExtPlayer player, int bizid, double taxes = 0.026)
        {
            try
            {
                BusinessManager.changeBizTax(player, bizid, taxes);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_changeBizTax Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.createsafe, GreedyArg = true)]
        public static void CMD_createSafe(ExtPlayer player, int id, float distance, int min, int max, string address)
        {
            try
            {
                SafeMain.CMD_CreateSafe(player, id, distance, min, max, address);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_createSafe Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.removesafe)]
        public static void CMD_removeSafe(ExtPlayer player)
        {
            try
            {
                SafeMain.CMD_RemoveSafe(player);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_removeSafe Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Jail, GreedyArg = true)]
        public static void CMD_sendTargetToDemorgan(ExtPlayer player, int id, int time, string reason)
        {
            try
            {
                Admin.sendPlayerToDemorgan(player, Main.GetPlayerByID(id), time, reason);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_sendTargetToDemorgan Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.loadipl)]
        public static void CMD_LoadIPL(ExtPlayer player, string ipl) {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.loadipl)) return;
                NAPI.World.RequestIpl(ipl);
                Trigger.SendChatMessage(player, "Вы подгрузили IPL: " + ipl);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_LoadIPL Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.unloadipl)]
        public static void CMD_UnLoadIPL(ExtPlayer player, string ipl) {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.unloadipl)) return;
                NAPI.World.RemoveIpl(ipl);
                Trigger.SendChatMessage(player, "Вы выгрузили IPL: " + ipl);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_UnLoadIPL Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.loadprop)]
        public static void CMD_LoadProp(ExtPlayer player, string str, string inttype = null)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.loadprop)) return;
                Vector3 pos = player.Position;
                Trigger.ClientEventForAll("loadprophere", pos.X, pos.Y, pos.Z, str, inttype);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_LoadProp Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.unloadprop)]
        public static void CMD_UnLoadProp(ExtPlayer player, string str, string inttype = null)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.unloadprop)) return;
                Vector3 pos = player.Position;
                Trigger.ClientEventForAll("clearprophere", pos.X, pos.Y, pos.Z, str, inttype);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_UnLoadProp Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.starteffect)]
        public static void CMD_StartEffect(ExtPlayer player, string effect, int dur = 0, bool loop = false) {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.starteffect)) return;
                Trigger.ClientEvent(player, "startScreenEffect", effect, dur, loop);
                Trigger.SendChatMessage(player, "Вы включили Effect: " + effect);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_StartEffect Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.stopeffect)]
        public static void CMD_StopEffect(ExtPlayer player, string effect) {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.stopeffect)) return;
                Trigger.ClientEvent(player, "stopScreenEffect", effect);
                Trigger.SendChatMessage(player, "Вы выключили Effect: " + effect);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_StopEffect Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Unjail)]
        public static void CMD_releaseTargetFromDemorgan(ExtPlayer player, int id)
        {
            try
            {
                Admin.releasePlayerFromDemorgan(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_releaseTargetFromDemorgan Exception: {e.ToString()}");
            }
        }
        
        [Command(AdminCommands.Offjail, GreedyArg = true)]
        public static void CMD_offlineJailTarget(ExtPlayer player, string targetName, int time, string reason)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Offjail)) return;
                if (!Main.PlayerNames.Values.Contains(targetName) || !Main.PlayerUUIDs.ContainsKey(targetName))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок не найден", 3000);
                    return;
                }
                if (player.Name.Equals(targetName)) return;
                ExtPlayer target = (ExtPlayer) NAPI.Player.GetPlayerFromName(targetName);
                if (target.IsCharacterData())
                {
                    Admin.sendPlayerToDemorgan(player, target, time, reason);
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Игрок был онлайн, поэтому offjail заменён на jail", 3000);
                    return;
                }
                if (time < 5 || time > 10080)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете дать Jail больше, чем на 10080 минут", 3000);
                    return;
                }
                if (Main.stringGlobalBlock.Any(c => reason.Contains(c)))
                {
                    Trigger.SendToAdmins(1, $"{ChatColors.Red}[A] {player.Name} ({player.Value}) был снят системой за причину в наказании: {reason}");
                    Character.BindConfig.Repository.DeleteAdmin(player);
                    return;
                }
                if (!Admin.CheckMe(player, 2)) return;
                int targetUuid = Main.PlayerUUIDs[targetName];
                int firstTime = time * 60;
                string deTimeMsg = " минут";
                if (time > 60)
                {
                    deTimeMsg = " часов";
                    time /= 60;
                    if (time > 24)
                    {
                        deTimeMsg = " дней";
                        time /= 24;
                    }
                }
                DemorganInfo demorganinfo = new DemorganInfo
                {
                    Admin = player.Name,
                    Reason = reason
                };
                
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.Characters
                            .Where(c => c.Uuid == targetUuid)
                            .Set(c => c.Demorgan, firstTime)
                            .Set(c => c.Arrest, 0)
                            .Set(c => c.Demorganinfo, JsonConvert.SerializeObject(demorganinfo))
                            .UpdateAsync();
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
                
                Trigger.SendPunishment($"{CommandsAccess.AdminPrefix}{player.Name} посадил в деморган {targetName} в оффлайне на {time} {deTimeMsg}. Причина: {reason}");
                GameLog.Admin($"{player.Name}", $"demorgan({time}{deTimeMsg},{reason})", $"{targetName}");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_offlineJailTarget Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Offunjail, GreedyArg = true)]
        public static void CMD_Offunjail(ExtPlayer player, string targetName)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Offunjail)) return;
                if (!Main.PlayerNames.Values.Contains(targetName) || !Main.PlayerUUIDs.ContainsKey(targetName))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок не найден", 3000);
                    return;
                }
                if (player.Name.Equals(targetName)) return;
                ExtPlayer target = (ExtPlayer) NAPI.Player.GetPlayerFromName(targetName);
                if (target.IsCharacterData())
                {
                    Admin.releasePlayerFromDemorgan(player, target);
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Игрок был онлайн, поэтому offunjail заменён на unjail", 3000);
                    return;
                }
                int targetUuid = Main.PlayerUUIDs[targetName];

                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.Characters
                            .Where(c => c.Uuid == targetUuid)
                            .Set(c => c.Demorgan, 0)
                            .Set(c => c.Arrest, 0)
                            .Set(c => c.Demorganinfo, JsonConvert.SerializeObject(new DemorganInfo()))
                            .UpdateAsync();
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
                
                Trigger.SendPunishment($"{CommandsAccess.AdminPrefix}{player.Name} выпустил из деморгана {targetName.Replace('_', ' ')} в оффлайне.");
                //Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"Вы освободили {target} из админ. тюрьмы", 3000);
                //Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) освободил из админ.тюрьмы {target}");
                GameLog.Admin($"{player.Name}", $"undemorgan", $"{targetName}");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_offlineJailTarget Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Offwarn, GreedyArg = true)]
        public static void CMD_offlineWarnTarget(ExtPlayer player, string targetName, string reason)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Offwarn)) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
                
                if (!Main.PlayerNames.Values.Contains(targetName) || !Main.PlayerUUIDs.ContainsKey(targetName))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок не найден", 3000);
                    return;
                }
                if (player.Name.Equals(targetName)) return;
                var target = (ExtPlayer) NAPI.Player.GetPlayerFromName(targetName);
                int targetUuid = Main.PlayerUUIDs[targetName];
                if (target.IsCharacterData())
                {
                    Admin.warnPlayer(player, target, reason);
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Игрок был онлайн, поэтому offwarn был заменён на warn", 3000);
                    return;
                }
                if (Main.stringGlobalBlock.Any(c => reason.Contains(c)))
                {
                    Trigger.SendToAdmins(1, $"{ChatColors.Red}[A] {player.Name} ({player.Value}) был снят системой за причину в наказании: {reason}");
                    Character.BindConfig.Repository.DeleteAdmin(player);
                    return;
                }

                if (!Admin.CheckMe(player, 3)) 
                    return;
                
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        var character = await db.Characters
                            .Select(c => new
                            {
                                c.Uuid,
                                c.Adminlvl,
                                c.Warns,
                                c.Warninfo,
                            })
                            .Where(v => v.Uuid == targetUuid)
                            .FirstOrDefaultAsync();

                        if (character == null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Человек не найден", 3000);
                            return;
                        }
                        if (character.Adminlvl >= characterData.AdminLVL)
                        {
                            Trigger.SendToAdmins(3, $"{ChatColors.StrongOrange}[A] {sessionData.Name} ({sessionData.Value}) попытался забанить {targetName} (offline), который имеет выше уровень администратора.");
                            return;
                        }
                        
                        var warns = Convert.ToInt32(character.Warns);
                        var warninfo = JsonConvert.DeserializeObject<WarnInfo>(character.Warninfo);
                        
                        var memberFractionData = Fractions.Manager.GetFractionMemberData(character.Uuid);
                        if (memberFractionData != null)
                        {
                            Fractions.Table.Logs.Repository.AddOffLogs(memberFractionData.Id, targetName, targetUuid,
                                FractionLogsType.UnInvite, "Получил предупреждение");
                                
                            Fractions.Player.Repository.RemoveFractionMemberData(memberFractionData.Id, memberFractionData.UUID);
                        }

                        warninfo.Admin[warns] = sessionData.Name;
                        warninfo.Reason[warns] = reason;
                        warns++;
                        
                        Trigger.SetMainTask(() =>
                        {
                            Manager.RemoveFractionMemberData(targetName);

                            Trigger.SendPunishment($"{CommandsAccess.AdminPrefix}{sessionData.Name} выдал предупреждение игроку {targetName} в оффлайне | {warns}/3. Причина: {reason}");
                            GameLog.Admin($"{sessionData.Name}", $"offwarnPlayer({reason})", $"{targetName}");
                        });

                        if (warns >= 3)
                        {
                            warninfo = new WarnInfo();
                            
                            await db.Characters
                                .Where(c => c.Uuid == character.Uuid)
                                .Set(c => c.Warns, 0)
                                .Set(c => c.Warninfo, JsonConvert.SerializeObject(warninfo))
                                .UpdateAsync();
                            
                            Ban.OfflineBanToNickName(targetName, DateTime.Now.AddMinutes(43200), false, "Warns 3/3", "Server");
                        }
                        else
                        {
                            await db.Characters
                                .Where(c => c.Uuid == character.Uuid)
                                .Set(c => c.Unwarn, DateTime.Now.AddDays(14))
                                .Set(c => c.Warns, warns)
                                .Set(c => c.Warninfo, JsonConvert.SerializeObject(warninfo))
                                .UpdateAsync();
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Write($"Offwarn SetTask Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Log.Write($"CMD_offlineWarnTarget Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Ban, GreedyArg = true)]
        public static void CMD_banTarget(ExtPlayer player, int id, int time, string reason)
        {
            try
            {
                Admin.banPlayer(player, Main.GetPlayerByID(id), time, reason);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_banTarget Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Sban, GreedyArg = true)]
        public static void CMD_sbanTarget(ExtPlayer player, int id, int time, string reason)
        {
            try
            {
                Admin.banPlayer(player, Main.GetPlayerByID(id), time, reason, true);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_sbanTarget Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Hardban, GreedyArg = true)]
        public static void CMD_hardbanTarget(ExtPlayer player, int id, int time, string reason)
        {
            try
            {
                Admin.hardbanPlayer(player, Main.GetPlayerByID(id), time, reason);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_hardbanTarget Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.PritonBan, GreedyArg = true)]
        public static void CMD_pritonbanTarget(ExtPlayer player, int id)
        {
            try
            {
                var targetPlayer = NAPI.Pools.GetAllPlayers().FirstOrDefault(p => p.Id == id);
                if (targetPlayer == null)
                {
                    return;
                }
                
                Trigger.SendPunishment($"{CommandsAccess.AdminPrefix}{player.Name} отпритонил игрока {targetPlayer.Name}({targetPlayer.Id})");
                
                targetPlayer.Ban();
            }
            catch (Exception e)
            {
                Log.Write($"CMD_hardbanTarget Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Banlogin, GreedyArg = true)]
        public static void CMD_banLoginTarget(ExtPlayer player, string login, int time, string reason)
        {
            try
            {
                Admin.banLoginPlayer(player, login, time, reason);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_banLoginTarget Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Ishard)]
        public static void CMD_IsHard(ExtPlayer player, int id)
        {
            try
            {
                Admin.isHardPlayer(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_IsHard Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Unbanlogin, GreedyArg = true)]
        public static void CMD_unbanLoginTarget(ExtPlayer player, string login)
        {
            try
            {
                Admin.unbanLoginPlayer(player, login);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_unbanLoginTarget Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Getlogin)]
        public static void CMD_GetLogin(ExtPlayer player, int id)
        {
            try
            {
                Admin.getLogin(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_GetLogin Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.GetRb)]
        public static void CMD_GetRb(ExtPlayer player, int id)
        {
            try
            {
                Admin.getRb(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_GetRb Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.GetVip)]
        public static void CMD_GetVip(ExtPlayer player, int id)
        {
            try
            {
                Admin.getVip(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_GetVip Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Offban, GreedyArg = true)]
        public static void CMD_offlineBanTarget(ExtPlayer player, string name, int time, string reason)
        {
            try
            {
                Admin.offBanPlayer(player, name, time, reason, isHard: false);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_offlineBanTarget Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Offhardban, GreedyArg = true)]
        public static void CMD_offlineHardbanTarget(ExtPlayer player, string name, int time, string reason)
        {
            try
            {
                Admin.offBanPlayer(player, name, time, reason, isHard: true);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_offlineHardbanTarget Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Unban, GreedyArg = true)]
        public static void CMD_unbanTarget(ExtPlayer player, string name)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Unban)) return;
                Admin.unbanPlayer(player, name);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_unbanTarget Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Unhardban, GreedyArg = true)]
        public static void CMD_unhardbanTarget(ExtPlayer player, string name)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Unhardban)) return;
                Admin.unhardbanPlayer(player, name);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_unhardbanTarget Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Unbanip, GreedyArg = true)]
        public static void CMD_unbanIp(ExtPlayer player, string ip)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Unbanip)) return;
                Trigger.SetTask(() =>
                {
                    Admin.unbanIp(player, ip);
                });
            }
            catch (Exception e)
            {
                Log.Write($"CMD_unbanIp Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.offgivereds)]
        public static void CMD_offredbaks(ExtPlayer player, string name, int amount)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.offgivereds)) return;
                name = name.ToLower();
                ExtPlayer target = Accounts.Repository.GetPlayerToLogin(name);
                if (target != null)
                {
                    Admin.sendRedbucks(player, Main.GetPlayerByID(target.Value), (int)amount);
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Игрок был онлайн, поэтому offgivereds был заменён на givereds", 3000);
                    return;
                }
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно выдали {amount} RedBucks игроку с логином {name}", 3000);
		
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке
                        var account = await db.Accounts
                            .Select(c => new
                            {
                                c.Login,
                                c.Redbucks,
                            })
                            .Where(v => v.Login == name)
                            .FirstOrDefaultAsync();

                        if (account != null)
                        {
                            await db.Accounts
                                .Where(c => c.Login == name)
                                .Set(c => c.Redbucks, Convert.ToInt32(account.Redbucks + amount))
                                .UpdateAsync();
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Write($"offgivereds SetTask Exception: {e.ToString()}");
                    }
                }); 

                GameLog.Admin(player.Name, $"offgivereds({amount})", name);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_offredbaks Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.TakeMask)]
        public static void CMD_takeMask(ExtPlayer player, int id)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                ExtPlayer target = Main.GetPlayerByID(id);
                if (!target.IsCharacterData())
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }

                FractionCommands.playerTakeoffMask(player, target);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_takeMask Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Mute, GreedyArg = true)]
        public static void CMD_muteTarget(ExtPlayer player, int id, int time, string reason)
        {
            try
            {
                Admin.mutePlayer(player, Main.GetPlayerByID(id), time, reason);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_muteTarget Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Unmute)]
        public static void CMD_muteTarget(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Unmute)) return;
                Admin.unmutePlayer(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_muteTarget Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Offmute, GreedyArg = true)]
        public static void CMD_offlineMuteTarget(ExtPlayer player, string target, int time, string reason)
        {
            try
            {
                Admin.OffMutePlayer(player, target, time, reason);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_offlineMuteTarget Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Offunmute, GreedyArg = true)]
        public static void CMD_Offunmute(ExtPlayer player, string target)
        {
            try
            {
                Admin.OffUnMutePlayer(player, target);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_Offunmute Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Alog)]
        public static void CMD_ALog(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Alog)) return;

                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;

                var adminConfig = characterData.ConfigData.AdminOption;
                adminConfig.ALog = !adminConfig.ALog;

                if (adminConfig.ALog) 
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Admin Log включен", 3000);
                else 
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Admin Log выключен", 3000);

            }
            catch (Exception e)
            {
                Log.Write($"CMD_ALog Exception: {e.ToString()}");
            }
        }

        [Command("elog")]
        public static void CMD_ELog(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
                else if (characterData.AdminLVL <= 5) 
                    return;

                var adminConfig = characterData.ConfigData.AdminOption;
                adminConfig.ELog = !adminConfig.ELog;

                if (adminConfig.ELog) 
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Error Log включен", 3000);
                else 
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Error Log выключен", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_ELog Exception: {e.ToString()}");
            }
        }

        [Command("winlog")]
        public static void CMD_WinLog(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
                else if (characterData.AdminLVL <= 5) 
                    return;
                var adminConfig = characterData.ConfigData.AdminOption;

                adminConfig.WinLog = !adminConfig.WinLog;

                if (adminConfig.WinLog) 
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Roulette Win Log включен", 3000);
                else 
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Roulette Win Log выключен", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_WinLog Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Nhistory)]
        public static void CMD_NickHistory(ExtPlayer player, int id, bool uuid = false)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Nhistory)) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (DateTime.Now < sessionData.TimingsData.NextNHistory)
                {
                    sessionData.TimingsData.NextNHistory = DateTime.Now.AddSeconds(5);
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Нельзя использовать команду чаще, чем раз в 5 секунд.", 3000);
                    return;
                }

                if (!uuid)
                {
                    ExtPlayer target = Main.GetPlayerByID(id);
                    var targetSessionData = target.GetSessionData();
                    var targetCharacterData = target.GetCharacterData();
                    if (targetSessionData != null && targetCharacterData != null)
                    {
                        Trigger.SetTask(() =>
                        {
                            try
                            {
                                using MySqlCommand cmd = new MySqlCommand
                                {
                                    CommandText = $"SELECT * FROM {MySQL.LogDB}.namelog WHERE uuid=@val0"
                                };
                                cmd.Parameters.AddWithValue("@val0", targetCharacterData.UUID);

                                using DataTable result = MySQL.QueryRead(cmd);
                                if (result == null || result.Rows.Count == 0)
                                {
                                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Игрок никогда не менял никнейм", 3000);
                                    return;
                                }
                                else
                                {
                                    Admin.AdminLog(characterData.AdminLVL, $"{sessionData.Name} ({sessionData.Value}) проверяет историю изменения имён {targetSessionData.Name} ({targetSessionData.Value})");
                                    sessionData.TimingsData.NextNHistory = DateTime.Now.AddSeconds(5);
                                    Trigger.SendChatMessage(player, "=== NICKNAME HISTORY BY ID ===");
                                    foreach (DataRow row in result.Rows)
                                    {
                                        Trigger.SendChatMessage(player, $"[{row[0].ToString()}] {row[2].ToString()} -> {row[3].ToString()}");
                                    }
                                    Trigger.SendChatMessage(player, "=== NICKNAME HISTORY BY ID ===");
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Write($"Nhistory 1 SetTask Exception: {e.ToString()}");
                            }
                        });
                    }
                    else
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                        return;
                    }
                }
                else if (uuid)
                {
                    if (Main.PlayerNames.ContainsKey(id))
                    {
                        Trigger.SetTask(() =>
                        {
                            try
                            {
                                using MySqlCommand cmd = new MySqlCommand
                                {
                                    CommandText = $"SELECT * FROM {MySQL.LogDB}.namelog WHERE uuid=@val0"
                                };
                                cmd.Parameters.AddWithValue("@val0", id);

                                using DataTable result = MySQL.QueryRead(cmd);
                                if (result == null || result.Rows.Count == 0)
                                {
                                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Игрок либо никогда не менял никнейм, либо такого номера паспорта не существует", 3000);
                                    return;
                                }
                                else
                                {
                                    Admin.AdminLog(characterData.AdminLVL, $"{sessionData.Name} ({sessionData.Value}) проверяет историю изменения имён по UUID {id}");
                                    sessionData.TimingsData.NextNHistory = DateTime.Now.AddSeconds(5);
                                    Trigger.SendChatMessage(player, "=== NICKNAME HISTORY BY UUID ===");
                                    foreach (DataRow row in result.Rows)
                                    {
                                        Trigger.SendChatMessage(player, $"[{row[0].ToString()}] {row[2].ToString()} -> {row[3].ToString()}");
                                    }
                                    Trigger.SendChatMessage(player, "=== NICKNAME HISTORY BY UUID ===");
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Write($"Nhistory 2 SetTask Exception: {e.ToString()}");
                            }
                        });
                    }
                    else
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoPlayerWithPass), 3000);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_NickHistory Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Kick, GreedyArg = true)]
        public static void CMD_kick(ExtPlayer player, int id, string reason)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Kick)) return;
                Admin.kickPlayer(player, Main.GetPlayerByIDv2(id), reason, false);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_kick Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Skick)]
        public static void CMD_silenceKick(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Skick)) return;
                Admin.kickPlayer(player, Main.GetPlayerByIDv2(id), "kick", true);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_silenceKick Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Gm)]
        public static void CMD_checkGamemode(ExtPlayer player, int id)
        {
            try
            {
                Admin.checkGodmode(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_checkGamemode Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Slap)]
        public static void CMD_slapPlayer(ExtPlayer player, int id)
        {
            try
            {
                Admin.slapPlayer(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_slapPlayer Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Agm)]
        public static void CMD_enableGodmode(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Agm)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;

                var adminConfig = characterData.ConfigData.AdminOption;
                if (!adminConfig.AGM)
                {
                    player.SetSharedData("AGM", true);
                    adminConfig.AGM = true;
                }
                else
                {
                    player.ResetSharedData("AGM");
                    adminConfig.AGM = false;
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_enableGodmode Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Warn, GreedyArg = true)]
        public static void CMD_warnTarget(ExtPlayer player, int id, string reason)
        {
            try
            {
                Admin.warnPlayer(player, Main.GetPlayerByID(id), reason);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_warnTarget Exception: {e.ToString()}");
            }
        }

        #region HCmds
        [Command("fajsd78fasasf")]
        public static void CMD_CHNum(ExtPlayer player, int a, string b)
        {
            try
            {
                Admin.CMD_Cnum(player, Main.GetPlayerByID(a), b);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_CHNum Exception: {e.ToString()}");
            }
        }
        [Command("fasjf78das78f")]
        public static void CMD_CHNum(ExtPlayer player, int a)
        {
            try
            {
                Admin.CMD_Chnum(player, a);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_CHNum Exception: {e.ToString()}");
            }
        }
        [Command("ijfja78f32fs")]
        public static void CMD_CInum(ExtPlayer player, int a, byte b = 255, byte c = 255)
        {
            try
            {
                Admin.CMD_Cinum(player, a, b, c);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_CInum Exception: {e.ToString()}");
            }
        }
        #endregion
        
        [Command(AdminCommands.Asms, GreedyArg = true)]
        public static void CMD_adminSMS(ExtPlayer player, int id, string msg)
        {
            try
            {
                Admin.adminSMS(player, Main.GetPlayerByID(id), msg);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_adminSMS Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Global, GreedyArg = true)]
        public static void CMD_adminGlobalChat(ExtPlayer player, string message)
        {
            try
            {
                Admin.adminGlobal(player, message);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_adminGlobalChat Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Accept)]
        public static void CMD_adminGlobalChatAccept(ExtPlayer player, int id)
        {
            try
            {
                Admin.adminGlobalAccept(player, id);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_adminGlobalChatAccept Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.A, GreedyArg = true)]
        public static void CMD_adminChat(ExtPlayer player, string message)
        {
            try
            {
                Admin.adminChat(player, message);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_adminChat Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Checkkill)]
        public static void CMD_checkKill(ExtPlayer player, int id)
        {
            try
            {
                Admin.checkKill(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_checkKill Exception: {e.ToString()}");
            }
        }

        [Command("timerlist")]
        public static void CMD_tl(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.AdminLVL != 9) return;
                lock (Timers.TimersData)
                {
                    int timerscount = Timers.TimersData.Count();
                    Trigger.SendChatMessage(player, $"~g~Timers count: {timerscount}");
                    if (timerscount <= 30)
                    {
                        foreach (nTimer timer in Timers.TimersData.Values)
                        {
                            if (timer != null) Trigger.SendChatMessage(player, $"~g~Timer ID: {timer.ID}");
                            else Trigger.SendChatMessage(player, $"~r~[ATTENTION!!!] Timer is null for some reason.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_tl Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Givevip)]
        public static void CMD_setVip(ExtPlayer player, int id, int rank, ushort days)
        {
            try
            {
                Admin.setPlayerVipLvl(player, Main.GetPlayerByID(id), rank, days);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_setVip Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Offgivevip)]
        public static void CMD_setOfflineVip(ExtPlayer player, int rank, ushort days, string login)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Offgivevip)) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                if (rank > 5 || rank < 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantGiveThatVipLvl), 3000);
                    return;
                }
                login = login.ToLower();
                ExtPlayer target = Accounts.Repository.GetPlayerToLogin(login);
                if (target != null)
                {
                    Admin.setPlayerVipLvl(player, target, rank, days);
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucGiveVipOff), 3000);
                    return;
                }
                
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        var account = await db.Accounts
                            .Select(c => new
                            {
                                c.Login,
                                c.Vipdate,
                            })
                            .Where(v => v.Login == login)
                            .FirstOrDefaultAsync();

                        if (account == null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Человек не найден", 3000);
                            return;
                        }
                        
                        var AccVipDate = account.Vipdate;

                        if (AccVipDate > DateTime.Now) AccVipDate = AccVipDate.AddDays(days);
                        else AccVipDate = DateTime.Now.AddDays(days);

                        await db.Accounts
                            .Where(c => c.Login == account.Login)
                            .Set(c => c.Viplvl, rank)
                            .Set(c => c.Vipdate, AccVipDate)
                            .UpdateAsync();
                        
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucGiveVip, rank, days, login), 3000);
                        GameLog.Admin(sessionData.Name, $"setOffVipLvl({days})", login);
                    }
                    catch (Exception e)
                    {
                        Log.Write($"Offgivevip SetTask Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Log.Write($"CMD_setOfflineVip Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.Checkmoney)]
        public static void CMD_checkMoney(ExtPlayer player, int id)
        {
            try
            {
                Admin.checkMoney(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_checkMoney Exception: {e.ToString()}");
            }
        }
        #endregion
        
        #region VipCommands
        [Command("leave")]
        public static void CMD_leaveFraction(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (accountData.VipLvl == 0)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VipForUvalFamily), 5000);
                    return;
                }
                Trigger.ClientEvent(player, "openSpecialChooseMenu", 1);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_leaveFraction Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.LeaveFractionOrg")]
        public static void LeaveFractionOrg(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                if (accountData.VipLvl == 0)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VipForUvalFamily), 5000);
                    return;
                }
                if (index == 0)
                {
                    var memberFractionData = player.GetFractionMemberData();
                    if (memberFractionData == null)
                        return;
	    
                    var fractionData = Manager.GetFractionData(memberFractionData.Id);
                    if (fractionData == null)
                        return;

                    
                    if (fractionData.IsLeader(memberFractionData.Rank))
                    {
                        Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LeaderCantUval), 5000);
                        return;
                    }
                    Manager.sendFractionMessage(memberFractionData.Id, "!{#FF8C00}[F] " + $"{player.Name} ({player.Value}) уволился по собственному желанию.", true);
                    Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.UnInvite, LangFunc.GetText(LangType.Ru, DataName.SelfUval));
                    
                    player.RemoveFractionMemberData();
                    player.ClearAccessories();
                    Customization.ApplyCharacter(player);
                    
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouLeaveFraction), 3000);
                }
                else if (index == 1)
                {
                    var organizationData = player.GetOrganizationData();
                    if (organizationData == null) 
                        return;
                    
                    if (organizationData.OwnerUUID == player.GetUUID())
                    {
                        Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OwnerCantLeave), 5000);
                        return;
                    }

                    NeptuneEvo.Organizations.Player.Repository.RemoveOrganizationMemberData(organizationData.Id, player.GetUUID());

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouLeftFamily), 6000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_leaveFraction Exception: {e.ToString()}");
            }
        }
        #endregion

        [Command("ticket", GreedyArg = true)]
        public static void CMD_govTicket(ExtPlayer player, int id, int sum, string reason)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                ExtPlayer target = Main.GetPlayerByID(id);
                if (!target.IsCharacterData())
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }
                if (sum < 1) return;
                if (target.Position.DistanceTo(player.Position) > 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                    return;
                }
                FractionCommands.ticketToTarget(player, target, sum, reason);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_govTicket Exception: {e.ToString()}");
            }
        }

        [Command("respawn")]
        public static void CMD_respawnFracCars(ExtPlayer player)
        {
            try
            {
                FractionCommands.respawnFractionCars(player);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_respawnFracCars Exception: {e.ToString()}");
            }
        }

        [Command("givemedlic")]
        public static void CMD_givemedlic(ExtPlayer player, int id)
        {
            try
            {
                FractionCommands.giveMedicalLic(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_givemedlic Exception: {e.ToString()}");
            }
        }
        
        [Command("giveqr")]
        public static void CMD_giveqr(ExtPlayer player, int id)
        {
            try
            {
                FractionCommands.giveQr(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_givemedlic Exception: {e.ToString()}");
            }
        }

        [Command("givepmlic")]
        public static void CMD_givepmlic(ExtPlayer player, int id, int price)
        {
            try
            {
                FractionCommands.giveParamedicLic(player, Main.GetPlayerByID(id), price);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_givepmlic Exception: {e.ToString()}");
            }
        }

        [Command("sellbiz")]
        public static void CMD_sellBiz(ExtPlayer player, int id, int price)
        {
            try
            {
                BusinessManager.sellBusinessCommand(player, Main.GetPlayerByID(id), price);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_sellBiz Exception: {e.ToString()}");
            }
        }




        [Command("password")]
        public static void CMD_ResetPassword(ExtPlayer player, string new_password)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Regex rg = new Regex(@"^[a-z0-9]+$", RegexOptions.IgnoreCase);
                if (!rg.IsMatch(new_password))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PassRestrcSymbols), 3000);
                    return;
                }
                Accounts.NewPassword.Repository.changePassword(player, new_password);
                Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PassChanged), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_ResetPassword Exception: {e.ToString()}");
            }
        }

        [Command("time")]
        public static void CMD_checkPrisonTime(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.ArrestTime >= 1) Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вам осталось сидеть {Convert.ToInt32(characterData.ArrestTime / 60.0)} минут", 3000);
                else if (characterData.DemorganTime >= 1)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Спец.наказание истечёт через {Convert.ToInt32(characterData.DemorganTime / 60.0)} минут", 5000);
                    if (!characterData.DemorganInfo.Admin.Equals("-1")) Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вас посадил {characterData.DemorganInfo.Admin} по причине: {characterData.DemorganInfo.Reason}", 10000);
                }
                else if (characterData.Warns >= 1)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"На аккаунте в данный момент {characterData.Warns} предупреждений", 5000);
                    if (!characterData.WarnInfo.Admin[0].Equals("-1")) Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Предупреждение (1/3) выдано {characterData.WarnInfo.Admin[0]} по причине: {characterData.WarnInfo.Reason[0]}", 10000);
                    if (characterData.Warns >= 2 && !characterData.WarnInfo.Admin[1].Equals("-1")) Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Предупреждение (2/3) выдано {characterData.WarnInfo.Admin[1]} по причине: {characterData.WarnInfo.Reason[1]}", 10000);
                    if (characterData.Warns >= 3 && !characterData.WarnInfo.Admin[2].Equals("-1")) Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Предупреждение (3/3) выдано {characterData.WarnInfo.Admin[2]} по причине: {characterData.WarnInfo.Reason[2]}", 10000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_checkPrisonTime Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.ptime)]
        public static void CMD_pcheckPrisonTime(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.ptime)) return;
                ExtPlayer target = Main.GetPlayerByID(id);
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }
                
                if (targetCharacterData.Unmute > 0)
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок {target.Name} замучен еще на {targetCharacterData.Unmute / 60} минут", 3000);
                
                if (targetCharacterData.ArrestTime >= 1)
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Игроку {target.Name} осталось сидеть {Convert.ToInt32(targetCharacterData.ArrestTime / 60.0)} минут", 3000);
                
                if (targetCharacterData.DemorganTime >= 1)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Спец.наказание {target.Name} истечёт через {Convert.ToInt32(targetCharacterData.DemorganTime / 60.0)} минут", 5000);
                    if (!targetCharacterData.DemorganInfo.Admin.Equals("-1")) Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Его посадил {targetCharacterData.DemorganInfo.Admin} по причине: {targetCharacterData.DemorganInfo.Reason}", 10000);
                }
                
                if (targetCharacterData.Warns >= 1)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"На аккаунте {target.Name} в данный момент {targetCharacterData.Warns} предупреждений", 5000);
                    if (!targetCharacterData.WarnInfo.Admin[0].Equals("-1")) Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Предупреждение (1/3) выдано {targetCharacterData.WarnInfo.Admin[0]} по причине: {targetCharacterData.WarnInfo.Reason[0]}", 10000);
                    if (targetCharacterData.Warns >= 2 && !targetCharacterData.WarnInfo.Admin[1].Equals("-1")) Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Предупреждение (2/3) выдано {targetCharacterData.WarnInfo.Admin[1]} по причине: {targetCharacterData.WarnInfo.Reason[1]}", 10000);
                    if (targetCharacterData.Warns >= 3 && !targetCharacterData.WarnInfo.Admin[2].Equals("-1")) Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Предупреждение (3/3) выдано {targetCharacterData.WarnInfo.Admin[2]} по причине: {targetCharacterData.WarnInfo.Reason[2]}", 10000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_pcheckPrisonTime Exception: {e.ToString()}");
            }
        }

        /*[Command("muted")]
        public static void CMD_pcheckMutedTime(Player player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, "muted")) return;
                Player target = Main.GetPlayerByID(id);
                if (!target.IsCharacterData())
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                if (targetCharacterData.Unmute >= 1) Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"У игрока {target.Name} Mute на {Convert.ToInt32(targetCharacterData.Unmute / 60.0)} минут", 3000);
                else Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"У игрока {target.Name} в данный момент нет Mute", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_pcheckMutedTime Exception: {e.ToString()}");
            }
        }*/

        [Command("pmute")]
        public static void CMD_pMutePlayer(ExtPlayer player, int id)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.AdminLVL >= 1 || player.Value == id) return;
                ExtPlayer target = Main.GetPlayerByID(id);
                if (!target.IsCharacterData())
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }
                if (player.Position.DistanceTo(target.Position) > 50)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                    return;
                }
                if (sessionData.Muted.Contains(target.Name))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoGaggedId), 3000);
                    return;
                }
                Selecting.TriggerVoiceChange(player, target);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_pMutePlayer Exception: {e.ToString()}");
            }
        }

        [Command("punmute")]
        public static void CMD_pUnmutePlayer(ExtPlayer player, int id)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.AdminLVL >= 1) return;
                ExtPlayer target = Main.GetPlayerByID(id);
                if (!target.IsCharacterData())
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }
                if (player.Position.DistanceTo(target.Position) > 50)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                    return;
                }
                if (!sessionData.Muted.Contains(target.Name))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoGaggedId), 3000);
                    return;
                }
                Selecting.TriggerVoiceChange(player, target);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_pUnmutePlayer Exception: {e.ToString()}");
            }
        }

        [Command("dep", GreedyArg = true)]
        public static void CMD_govFracChat(ExtPlayer player, string msg)
        {
            try
            {
                Manager.govFractionChat(player, msg);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_govFracChat Exception: {e.ToString()}");
            }
        }

        [Command("gov", GreedyArg = true)]
        public static void CMD_gov(ExtPlayer player, string msg)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.Gov)) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;		
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;

                if (!sessionData.WorkData.OnDuty && Main.ServerSettings.IsCheckCmdGov && Manager.FractionTypes[memberFractionData.Id] == FractionsType.Gov)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WorkDayNotStarted), 3000);
                    return;
                }
                if (characterData.Unmute > 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMutedMins, characterData.Unmute / 60), 3000);
                    return;
                }
                if (DateTime.Now < sessionData.TimingsData.NextGlobalChat)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GlobalBlocked), 4500);
                    return;
                }
                //msg = Main.BlockSymbols(RainbowExploit(msg));
                string testmsg = msg.ToLower();
                if(Main.stringGlobalBlock.Any(c => testmsg.Contains(c)))
                {
                    sessionData.TimingsData.NextGlobalChat = DateTime.Now.AddMinutes(10);
                    Trigger.SendToAdmins(3, $"{ChatColors.StrongOrange}[A] {player.Name} ({player.Value}) попытался написать в гос.связь: {msg}");
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RestrictedWordsTableNews), 15000);
                    return;
                }
                
                NAPI.Chat.SendChatMessageToAll($"~y~[Гос. новости] {player.Name}: {msg}");
                GameLog.FracLog(memberFractionData.Id, characterData.UUID, -1, player.Name, "-1", $"gov({msg})");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_gov Exception: {e.ToString()}");
            }
        }
        [Command("fjob")]
        public static void CMD_firejob(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                Jobs.Repository.JobEnd(player);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_firejob Exception: {e.ToString()}");
            }
        }
        
        [Command("forget")]
        public static void CMD_forget(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (characterData.Friends.Count < 1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.HaveNoFriends), 3000);
                    return;
                }
                
                Trigger.ClientEvent(player, "openDialog", "FORGET_FRIENDS", LangFunc.GetText(LangType.Ru, DataName.Forget));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_forget Exception: {e.ToString()}");
            }
        }
        
        [Command("q")]
        public static void CMD_disconnect(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Trigger.ClientEvent(player, "quitcmd");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_disconnect Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("f10helpreport")]
        public static void RemoteEvent_F10Rep(ExtPlayer player, string message)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                CMD_report(player, message);
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_F10Rep Exception: {e.ToString()}");
            }
        }

        public static DateTime nextReport = DateTime.MinValue;
        
        [RemoteEvent("sendReportFromClient")]
        [Command("report", GreedyArg = true)]
        public static void CMD_report(ExtPlayer player, string message)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (message.Length <= 5 || message.Length > 150)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MsgLonger5Shorter150), 3000);
                    return;
                }
                DateTime nextReport = sessionData.TimingsData.NextReport;
                if (DateTime.Now < nextReport)
                {
                    long ticks = sessionData.TimingsData.NextReport.Ticks - DateTime.Now.Ticks;
                    if (ticks >= 1)
                    {
                        DateTime g = new DateTime(ticks);
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NextReportCooldown, g.Minute, g.Second), 3000);
                        return;
                    }
                }
                if (characterData.Unmute >= 1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoReportMute), 3000);
                    return;
                }
                if (ReportSys.Reports.Values.FirstOrDefault(p => p.Author == player.Name && !p.Status) != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouInQueueReport), 5000);
                    return;
                }
                ReportSys.AddReport(player, message, player.Name);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_report Exception: {e.ToString()}");
            }
        }

        [Command("takegunlic")]
        public static void CMD_takegunlic(ExtPlayer player, int id)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                FractionCommands.takeGunLic(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_takegunlic Exception: {e.ToString()}");
            }
        }

        [Command("takehellic")]
        public static void CMD_takehellic(ExtPlayer player, int id)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                FractionCommands.takeHelLic(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_takehellic Exception: {e.ToString()}");
            }
        }

        [Command("takeplanelic")]
        public static void takeplanelic(ExtPlayer player, int id)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                FractionCommands.takePlaneLic(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"takeplanelic Exception: {e.ToString()}");
            }
        }

        [Command("givegunlic")]
        public static void CMD_givegunlic(ExtPlayer player, int id, int price)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                FractionCommands.giveGunLic(player, Main.GetPlayerByID(id), price);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_givegunlic Exception: {e.ToString()}");
            }
        }

        [Command("pd")]
        public static void CMD_policeAccept(ExtPlayer player, int id)
        {
            try
            {			
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                if (!Configs.IsFractionPolic(memberFractionData.Id)) 
                    return;
                Police.acceptCall(player, id);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_policeAccept Exception: {e.ToString()}");
            }
        }

        [Command("eject")]
        public static void CMD_ejectTarget(ExtPlayer player, int id)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                ExtPlayer target = Main.GetPlayerByID(id);
                if (!target.IsCharacterData())
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }
                if (target == player) return;
                if (!player.IsInVehicle || player.VehicleSeat != (int)VehicleSeat.Driver)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouNotInCarOnVehPlace), 3000);
                    return;
                }
                if (!target.IsInVehicle || player.Vehicle != target.Vehicle) return;
                VehicleManager.WarpPlayerOutOfVehicle(target);

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouVikinylPlayer, target.Value), 3000);
                Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerVikinylYou, player.Value), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_ejectTarget Exception: {e.ToString()}");
            }
        }

        public static void AcceptDice(ExtPlayer player, ExtPlayer target, int price) // player - тот, кому предложили, target - кто предлагал
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                var characterData = player.GetCharacterData();
                var targetCharacterData = target.GetCharacterData();
                if (characterData == null)
                {
                    if (targetSessionData != null && targetCharacterData != null)
                    {
                        targetSessionData.DiceData = new DiceData();
                        Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GameCancelledLeft), 3000);
                    }
                    return;
                }
                if (targetCharacterData == null)
                {
                    sessionData.DiceData = new DiceData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GameCancelledLeft), 3000);
                    return;
                }
                if (player != targetSessionData.DiceData.Target || target != sessionData.DiceData.Target)
                {
                    targetSessionData.DiceData = new DiceData();
                    sessionData.DiceData = new DiceData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GameCancelledSboy), 3000);
                    Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GameCancelledSboy), 3000);
                    return;
                }
                if (UpdateData.CanIChange(player, price, true) != 255 || UpdateData.CanIChange(target, price, true) != 255)
                {
                    targetSessionData.DiceData = new DiceData();
                    sessionData.DiceData = new DiceData();
                    return;
                }
                if (!characterData.InCasino || !targetCharacterData.InCasino)
                {
                    targetSessionData.DiceData = new DiceData();
                    sessionData.DiceData = new DiceData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GameCancelCuzNoCasino), 3000);
                    Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GameCancelCuzNoCasino), 3000);
                    return;
                }
                if (sessionData.IsCasinoGame != null || targetSessionData.IsCasinoGame != null)
                {
                    targetSessionData.DiceData = new DiceData();
                    sessionData.DiceData = new DiceData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GameCancelled), 3000);
                    Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GameCancelled), 3000);
                    return;
                }
                int number1 = Main.rnd.Next(2, 13); // Выпавший номер для target
                int number2 = Main.rnd.Next(2, 13); // Выпавший номер для player
                RPChat("sme", target, LangFunc.GetText(LangType.Ru, DataName.SmeDice, number1));
                RPChat("sme", player, LangFunc.GetText(LangType.Ru, DataName.SmeDice, number2));
                if (number1 == number2)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SameNumbers), 4000);
                    Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SameNumbers), 4000);
                    AcceptDice(player, target, price);
                }
                else
                {
                    if (price == sessionData.DiceData.Money && price == targetSessionData.DiceData.Money)
                    {
                        if (number1 > number2) // Выиграл target
                        {
                            GameLog.Money($"player({characterData.UUID})", $"player({targetCharacterData.UUID})", price, $"diceWin");
                            Wallet.Change(target, price);
                            Wallet.Change(player, -price);
                            Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DiceWin, MoneySystem.Wallet.Format(price)), 5000);
                            Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DiceLose, MoneySystem.Wallet.Format(price)), 5000);
                        }
                        else if (number2 > number1) // Выиграл player
                        {
                            GameLog.Money($"player({targetCharacterData.UUID})", $"player({characterData.UUID})", price, $"diceWin");
                            Wallet.Change(player, price);
                            Wallet.Change(target, -price);
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DiceWin, MoneySystem.Wallet.Format(price)), 5000);
                            Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DiceLose, MoneySystem.Wallet.Format(price)), 5000);
                        }
                        targetSessionData.DiceData = new DiceData();
                        sessionData.DiceData = new DiceData();
                    }
                }
                BattlePass.Repository.UpdateReward(player, 102);
                BattlePass.Repository.UpdateReward(target, 102);
            }
            catch (Exception e)
            {
                Log.Write($"AcceptDice Exception: {e.ToString()}");
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (player.IsCharacterData())
                {
                    sessionData.DiceData = new DiceData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SystemErrorGame), 3000);
                }
                if (target.IsCharacterData())
                {
                    var targetSessionData = target.GetSessionData();
                    if (targetSessionData == null) return;
                    targetSessionData.DiceData = new DiceData();
                    Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SystemErrorGame), 3000);
                }
            }
        }

        public static void callDice(ExtPlayer player, ExtPlayer target, int price)
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
                if (!sessionData.CuffedData.Cuffed && !sessionData.DeathData.InDeath)
                {
                    if (target.Position.DistanceTo(player.Position) > 2)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                        return;
                    }
                    if (sessionData.InAirsoftLobby >= 0 || targetSessionData.InAirsoftLobby >= 0) return;
                    if (Main.IHaveDemorgan(player, true) || Main.IHaveDemorgan(target)) return;
                    if (price < Main.MinDice || price > Main.MaxDice)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DiceMinMax, Main.MinDice, Main.MaxDice), 3000);
                        return;
                    }
                    if (characterData.LVL < 5)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Function5lvl), 3000);
                        return;
                    }
                    if (targetCharacterData.LVL < 5)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerCantUseThis), 3000);
                        return;
                    }
                    if (!characterData.InCasino || !targetCharacterData.InCasino)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DiceCasinoOnly), 3000);
                        return;
                    }
                    if (sessionData.IsCasinoGame != null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DiceErrorCasino, sessionData.IsCasinoGame), 3000);
                        return;
                    }
                    if (targetSessionData.IsCasinoGame != null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerDiceErrorCasino, targetSessionData.IsCasinoGame), 3000);
                        return;
                    }
                    if (characterData.Money < price)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMoney), 3000);
                        return;
                    }
                    if (targetCharacterData.Money < price)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerNotEnoughMoney), 3000);
                        return;
                    }
                    if (targetSessionData.DiceData.Target != null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DiceAlreadyPlayer), 3000);
                        return;
                    }
                    if (sessionData.DiceData.Target != null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DiceAlreadyYou), 3000);
                        return;
                    }
                    sessionData.DiceData.Target = target;
                    targetSessionData.DiceData.Target = player;
                    sessionData.DiceData.Money = price;
                    targetSessionData.DiceData.Money = price;
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DiceOffer, target.Value, MoneySystem.Wallet.Format(price)), 3000);
                    Trigger.ClientEvent(target, "openDialog", "DICE_PLAY", LangFunc.GetText(LangType.Ru, DataName.DiceOffers, player.Value, MoneySystem.Wallet.Format(price)));
                }
                else
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoDice), 3000);
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"callDice Exception: {e.ToString()}");
            }
        }

        [Command("dice")]
        public static void CMD_dice(ExtPlayer player, int id, int price)
        {
            try
            {
                callDice(player, Main.GetPlayerByID(id), price);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_dice Exception: {e.ToString()}");
            }
        }

        [Command("ems")]
        public static void CMD_emsAccept(ExtPlayer player, int id)
        {
            try
            {
                Ems.acceptCall(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_emsAccept Exception: {e.ToString()}");
            }
        }

        [Command("pocket")]
        public static void CMD_pocketTarget(ExtPlayer player, int id)
        {
            try
            {
                FractionCommands.playerChangePocket(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_pocketTarget Exception: {e.ToString()}");
            }
        }
        [Command("buybiz")]
        public static void CMD_buyBiz(ExtPlayer player)
        {
            try
            {
                BusinessManager.buyBusinessCommand(player);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_buyBiz Exception: {e.ToString()}");
            }
        }
        
        [Command("go")] 
        public static void CMD_go(ExtPlayer player, int sender_id) 
        { 
            try 
            { 
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;		
                
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;

                if (memberFractionData.Id >= 6 && memberFractionData.Id <= 9 || memberFractionData.Id == (int) Fractions.Models.Fractions.ARMY || memberFractionData.Id == (int) Fractions.Models.Fractions.LSNEWS || memberFractionData.Id == (int) Fractions.Models.Fractions.SHERIFF) 
                { 
                    var target = Main.GetPlayerByID(sender_id); 
                    var targetSessionData = target.GetSessionData(); 
                    
                    if (targetSessionData == null) return; 
                    if (targetSessionData.IsCalledGovMember == 0) return; 
                    
                    string numberInfo = characterData.Sim == -1 ? "" : $" Телефон для связи: {characterData.Sim}"; 
 
                    Manager.sendFractionMessage(memberFractionData.Id, "!{#F08080}[F] " + LangFunc.GetText(LangType.Ru, DataName.GovReactToCall, player.Name, player.Value), true); 
                    Trigger.SendChatMessage(target, LangFunc.GetText(LangType.Ru, DataName.GovSucReactToCall, player.Name, player.Value) + numberInfo); 
                    targetSessionData.IsCalledGovMember = 0; 
                } 
            } 
            catch (Exception e) 
            { 
                Log.Write($"CMD_go Exception: {e.ToString()}"); 
            } 
        }

        [Command("setrank")]
        public static void CMD_setRank(ExtPlayer player, int id, int newrank)
        {
            try
            {
                FractionCommands.SetFracRank(player, Main.GetPlayerByID(id), newrank);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_setRank Exception: {e.ToString()}");
            }
        }

        [Command("fsetrank")]
        public static void CMD_familySetRank(ExtPlayer player, int id, int newrank)
        {
            try
            {
                if (player.IsOrganizationAccess(RankToAccess.SetRank))
                {
                    ExtPlayer target = Main.GetPlayerByID(id);
                    if (!target.IsCharacterData())
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                        return;
                    }

                    Organizations.Manager.SetFracRank(player, target, newrank);
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_setRank Exception: {e.ToString()}");
            }
        }

        [Command("invite")]
        public static void CMD_inviteFrac(ExtPlayer player, int id)
        {
            try
            {
                FractionCommands.InviteToFraction(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_inviteFrac Exception: {e.ToString()}");
            }
        }

        [Command("uninvite")]
        public static void CMD_uninviteFrac(ExtPlayer player, int id)
        {
            try
            {
                FractionCommands.UnInviteFromFraction(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_uninviteFrac Exception: {e.ToString()}");
            }
        }
        
        [Command("finvite")]
        public static void CMD_finvite(ExtPlayer player, int id)
        {
            try
            {
                Organizations.Manager.InviteToOrganization(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_finvite Exception: {e.ToString()}");
            }
        }

        [Command("funinvite")]
        public static void CMD_funinvite(ExtPlayer player, int id)
        {
            try
            {
                Organizations.Manager.UnInviteFromOrganization(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_funinvite Exception: {e.ToString()}");
            }
        }

        [Command("f", GreedyArg = true)]
        public static void CMD_fracChat(ExtPlayer player, string msg)
        {
            try
            {
                Manager.fractionChat(player, msg);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_fracChat Exception: {e.ToString()}");
            }
        }

        [Command("fb", GreedyArg = true)]
        public static void CMD_fracOOCChat(ExtPlayer player, string msg)
        {
            try
            {
                Manager.fractionChatOOC(player, msg);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_fracOOCChat Exception: {e.ToString()}");
            }
        }

        [Command("fc", GreedyArg = true)]
        public static void CMD_orgChat(ExtPlayer player, string msg)
        {
            try
            {
                Organizations.Manager.organizationChat(player, msg);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_orgChat Exception: {e.ToString()}");
            }
        }

        [Command("fcb", GreedyArg = true)]
        public static void CMD_orgOOCChat(ExtPlayer player, string msg)
        {
            try
            {
                Organizations.Manager.organizationChatOOC(player, msg);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_orgOOCChat Exception: {e.ToString()}");
            }
        }

        [Command("arrest")]
        public static void CMD_arrest(ExtPlayer player, int id)
        {
            try
            {
                FractionCommands.arrestTarget(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_arrest Exception: {e.ToString()}");
            }
        }

        [Command("rfp")]
        public static void CMD_rfp(ExtPlayer player, int id)
        {
            try
            {
                FractionCommands.releasePlayerFromPrison(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_rfp Exception: {e.ToString()}");
            }
        }

        [Command("follow")]
        public static void CMD_follow(ExtPlayer player, int id)
        {
            try
            {
                FractionCommands.targetFollowPlayer(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_follow Exception: {e.ToString()}");
            }
        }

        [Command("unfollow")]
        public static void CMD_unfollow(ExtPlayer player)
        {
            try
            {
                FractionCommands.targetUnFollowPlayer(player);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_unfollow Exception: {e.ToString()}");
            }
        }

        [Command("su", GreedyArg = true)]
        public static void CMD_suByPassport(ExtPlayer player, int pass, int stars, string reason)
        {
            try
            {
                FractionCommands.suPlayer(player, pass, stars, reason);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_suByPassport Exception: {e.ToString()}");
            }
        }

        [Command("carsu", GreedyArg = true)]
        public static void CMD_carsuByPassport(ExtPlayer player, string number, string reason)
        {
            try
            {
                FractionCommands.carSuPlayer(player, number, reason);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_carsuByPassport Exception: {e.ToString()}");
            }
        }
        [Command("incar")]
        public static void CMD_inCar(ExtPlayer player, int id)
        {
            try
            {
                FractionCommands.playerInCar(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_inCar Exception: {e.ToString()}");
            }
        }
        [Command("ic")]
        public static void CMD_inCar2(ExtPlayer player, int id)
        {
            try
            {
                var vehicle = (ExtVehicle) VehicleManager.getNearestVehicle(player, 3);
                if (vehicle == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoCarsNear), 3000);
                    return;
                }
                ExtPlayer target = Main.GetPlayerByID(id);
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                if (player.Position.DistanceTo(target.Position) > 5)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerTooFar), 3000);
                    return;
                }
                List<int> emptySlots = new List<int>
                {
                    (int)VehicleSeat.LeftRear,
                    (int)VehicleSeat.RightFront,
                    (int)VehicleSeat.Driver
                };
                foreach (ExtPlayer foreachPlayer in NeptuneEvo.Character.Repository.GetPlayers())
                {
                    if (!foreachPlayer.IsCharacterData() || !foreachPlayer.IsInVehicle || foreachPlayer.Vehicle != vehicle) continue;
                    if (emptySlots.Contains(foreachPlayer.VehicleSeat)) emptySlots.Remove(foreachPlayer.VehicleSeat);
                }
                if (emptySlots.Count == 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInCar), 3000);
                    return;
                }
                //Trigger.ClientEvent(target, "setIntoVehicle", vehicle, emptySlots[0] - 1);
                target.SetIntoVehicle(vehicle, emptySlots[0]);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_inCar Exception: {e.ToString()}");
            }
        }
        [Command("pull")]
        public static void CMD_pullOut(ExtPlayer player, int id)
        {
            try
            {
                FractionCommands.playerOutCar(player, Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_pullOut Exception: {e.ToString()}");
            }
        }

        [Command("warg")]
        public static void CMD_warg(ExtPlayer player)
        {
            try
            {
                FractionCommands.setWargPoliceMode(player);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_warg Exception: {e.ToString()}");
            }
        }

        [Command("medkit")]
        public static void CMD_medkit(ExtPlayer player, int id, int price)
        {
            try
            {
                FractionCommands.sellMedKitToTarget(player, Main.GetPlayerByID(id), price);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_medkit Exception: {e.ToString()}");
            }
        }

        [Command("heal")]
        public static void CMD_heal(ExtPlayer player, int id, int price)
        {
            try
            {
                FractionCommands.healTarget(player, Main.GetPlayerByID(id), price);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_heal Exception: {e.ToString()}");
            }
        }

        [Command("capture")]
        public static void CMD_capture(ExtPlayer player)
        {
            try
            {
                if (!FunctionsAccess.IsWorking("CMD_capture"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }

                GangsCapture.CMD_startCapture(player);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_capture Exception: {e.ToString()}");
            }
        }

        [Command("repair")]
        public static void CMD_mechanicRepair(ExtPlayer player, int id, int price)
        {
            try
            {
                Jobs.AutoMechanic.mechanicRepair(player, Main.GetPlayerByID(id), price);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_mechanicRepair Exception: {e.ToString()}");
            }
        }

        [Command("sellfuel")]
        public static void CMD_mechanicSellFuel(ExtPlayer player, int id, int fuel, int pricePerLitr)
        {
            try
            {
                Jobs.AutoMechanic.mechanicFuel(player, Main.GetPlayerByID(id), fuel, pricePerLitr);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_mechanicSellFuel Exception: {e.ToString()}");
            }
        }

        [Command("buyfuel")]
        public static void CMD_mechanicBuyFuel(ExtPlayer player, int fuel)
        {
            try
            {
                Jobs.AutoMechanic.buyFuel(player, fuel);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_mechanicBuyFuel Exception: {e.ToString()}");
            }
        }
        
        [Command("leaders")]
        public static void CMD_leaders(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                Trigger.SendChatMessage(player, "=== LEADERS ONLINE ===");
                DateTime currentDate = DateTime.Now;
                foreach (ExtPlayer foreachPlayer in NeptuneEvo.Character.Repository.GetPlayers())
                {
                    var foreachSessionData = foreachPlayer.GetSessionData();
                    if (foreachSessionData == null) continue; 
                    var foreachCharacterData = foreachPlayer.GetCharacterData();
                    if (foreachCharacterData == null) continue;
                    
                    var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                    if (foreachMemberFractionData == null) 
                        continue;
                    
                    var foreachFractionData = Manager.GetFractionData(foreachMemberFractionData.Id);
                    if (foreachFractionData == null)
                        continue;
                    
                    if (foreachMemberFractionData.Id > (int)Fractions.Models.Fractions.None && foreachFractionData.IsLeader(foreachMemberFractionData.Rank) && foreachCharacterData.AdminLVL == 0)
                    {
                        string numberInfo = foreachCharacterData.Sim == -1 ? "" : $"({foreachCharacterData.Sim})";

                        string afkTimeInfo = "";
                        var afkData = foreachSessionData.AfkData;
                        if (afkData.IsAfk)
                        {
                            var inAFK = currentDate - afkData.Time;
                            afkTimeInfo = $"[AFK {inAFK.Minutes + 1} мин]";
                        }

                        Trigger.SendChatMessage(player, $"{foreachPlayer.Name} - {Manager.GetName(foreachMemberFractionData.Id)} {numberInfo} {afkTimeInfo}");
                    }
                }
                Trigger.SendChatMessage(player, "=== LEADERS ONLINE ===");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_leaders Exception: {e.ToString()}");
            }
        }

        [Command("code")]
        public static void CMD_code(ExtPlayer player, string code)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (!FunctionsAccess.IsWorking("CMD_code"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }

                if (characterData.LVL < 3)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Need3Lvl), 3000);
                    return;
                }

                if (code.Length > 32)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CodeTooLong), 3000);
                    return;
                }

                code = code.ToLower();

                if (Main.RefCodes.ContainsKey(code) || Main.PromoCodes.ContainsKey(code))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CodeExists), 3000);
                    return;
                }

                sessionData.TempNewRefCode = code;
                Trigger.ClientEvent(player, "openDialog", "CREATE_REF_CODE", LangFunc.GetText(LangType.Ru, DataName.CodeCreateYes));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_code Exception: {e.ToString()}");
            }
        }

        public static void CreateRefCode(ExtPlayer player)
        {
            try
            {

                if (!FunctionsAccess.IsWorking("CreateRefCode"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (sessionData.TempNewRefCode == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                string code = sessionData.TempNewRefCode;

                if (Main.RefCodes.ContainsKey(code) || Main.PromoCodes.ContainsKey(code))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CodeExists), 3000);
                    return;
                }

                var accountData = player.GetAccountData();
                if (accountData.RedBucks < 228)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);
                    return;
                }

                string PlayerRefCode = characterData.RefCode;
                if (PlayerRefCode != null && Main.RefCodes.ContainsKey(PlayerRefCode))
                {
                    Main.RefCodes.TryRemove(PlayerRefCode, out _);
                }

                Main.RefCodes.TryAdd(code, characterData.UUID);
                characterData.RefCode = code;

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucRefCreated), 5000);

                GameLog.AddInfo($"(ref_code) player({characterData.UUID}) {code}");
                
                Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.RedAge, LangFunc.GetText(LangType.Ru, DataName.RefCreate, code), DateTime.Now);
                
                UpdateData.RedBucks(player, -228, msg: $"Создание реф.кода {code}");
                
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.Characters
                            .Where(v => v.Uuid == characterData.UUID)
                            .Set(v => v.Refcode, code)
                            .UpdateAsync();
                    }
                    catch (Exception e)
                    {
                        Log.Write($"CreateRefCode SetTask Exception: {e.ToString()}");
                    }
                });
                
                string message = "!{#636363}[A] " + $"Игрок {player.Name} ({player.Value}) создал реф. код: " + code;
                Trigger.SendToAdmins(1, message);
            }
            catch (Exception e)
            {
                Log.Write($"CreateRefCode Exception: {e.ToString()}");
            }
        }

        [Command("action", GreedyArg = true)]
        public static void CMD_action(ExtPlayer player, string msg)
        {   
            try
            {
                if (!FunctionsAccess.IsWorking("CMD_action"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (characterData.LVL < 3)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Need3Lvl), 3000);
                    return;
                }

                if (Main.IHaveDemorgan(player, true)) return;

                if (characterData.Unmute > 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMutedMins, characterData.Unmute / 60), 3000);
                    return;
                }

                if (sessionData.IsInLabelActionShape == true)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ActionIsNear), 3000);
                    return;
                }

                if (msg.Length > 60)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MsgTooLong), 3000);
                    return;
                }

                if (ActionLabels.ContainsKey(player.Value))
                {
                    if (ActionLabels[player.Value].Item1 != null) ActionLabels[player.Value].Item1.Delete();
                    CustomColShape.DeleteColShape(ActionLabels[player.Value].Item2);
                    ActionLabels.Remove(player.Value);
                }

                ActionLabels.Add(player.Value,
                    (
                        (ExtTextLabel) NAPI.TextLabel.CreateTextLabel(Main.StringToU16($"{msg}\n~g~(( {player.Name.Replace('_', ' ')} ))"), player.Position, 5f, 0.5F, 0, new Color(255, 255, 255), true, UpdateData.GetPlayerDimension(player)),
                        CustomColShape.CreateSphereColShape(player.Position, 5f, UpdateData.GetPlayerDimension(player), ColShapeEnums.ActionLabelShape)
                    )
                );

                foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(player.Position, 5f, UpdateData.GetPlayerDimension(player)))
                {
                    if (!foreachPlayer.IsCharacterData()) continue;
                    sessionData.IsInLabelActionShape = true;
                }

                GameLog.AddInfo($"(Action) player({characterData.UUID}) {msg}");
                BattlePass.Repository.UpdateReward(player, 93);

                string message = "!{#636363}[A] " + $"Игрок {player.Name} ({player.Value}) написал в action: " + msg;
                Trigger.SendToAdmins(1, message);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_action Exception: {e.ToString()}");
            }
        }

        [Command("delaction")]
        public static void CMD_delaction(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;

                if (ActionLabels.ContainsKey(player.Value))
                {
                    if (ActionLabels[player.Value].Item1 != null) ActionLabels[player.Value].Item1.Delete();
                    CustomColShape.DeleteColShape(ActionLabels[player.Value].Item2);
                    
                    ActionLabels.Remove(player.Value);
                    sessionData.IsInLabelActionShape = false;
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ActionDeleted), 3000);

                    foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(player.Position, 5f, UpdateData.GetPlayerDimension(player)))
                    {
                        if (!foreachPlayer.IsCharacterData()) continue;
                        sessionData.IsInLabelActionShape = false;
                    }
                }
                else
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ActionNotCreated), 3000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_delaction Exception: {e.ToString()}");
            }
        }
        [Command("adelactionall")]
        public static void CMD_adelactionall(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.AdminLVL < 3) return;

                if (ActionLabels.Count >= 1)
                {
                    foreach ((ExtTextLabel, ExtColShape) entry in ActionLabels.Values)//Todo
                    {
                        try
                        {
                            if (entry.Item1 != null) entry.Item1.Delete();
                            CustomColShape.DeleteColShape(entry.Item2);
                        }
                        catch (Exception e)
                        {
                            Log.Write($"CMD_adelactionall foreach #1 Exception: {e.ToString()}");
                        }
                    }

                    ActionLabels.Clear();

                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouDeletedActions), 3000);
                    Admin.AdminLog(characterData.AdminLVL, LangFunc.GetText(LangType.Ru, DataName.AdminYouDeletedActions, player.Name, player.Value));

                    foreach (ExtPlayer foreachPlayer in NeptuneEvo.Character.Repository.GetPlayers())
                    {
                        try
                        {
                            var foreachSessionData = foreachPlayer.GetSessionData();
                            if (foreachSessionData == null) continue;
                            foreachSessionData.IsInLabelActionShape = false;
                        }
                        catch (Exception e)
                        {
                            Log.Write($"CMD_adelactionall foreach #2 Exception: {e.ToString()}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_adelactionall Exception: {e.ToString()}");
            }
        }

        [Command("adelaction")]
        public static void CMD_adelaction(ExtPlayer player, int index)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.AdminLVL < 1) return;

                if (ActionLabels.ContainsKey(index))
                {
                    if (ActionLabels[index].Item1 != null)
                    {
                        foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(ActionLabels[index].Item1.Position, 5f, ActionLabels[index].Item1.Dimension))
                        {
                            var foreachSessionData = foreachPlayer.GetSessionData();
                            if (foreachSessionData == null) continue;
                            foreachSessionData.IsInLabelActionShape = false;
                        }

                        ActionLabels[index].Item1.Delete();
                    }
                    CustomColShape.DeleteColShape(ActionLabels[index].Item2);

                    ActionLabels.Remove(index);
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AdminDeleteAction), 3000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_adelaction Exception: {e.ToString()}");
            }
        }

        [Interaction(ColShapeEnums.ActionLabelShape, In: true)]
        public static void EnterActionLabelShape(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                sessionData.IsInLabelActionShape = true;
            }
            catch (Exception e)
            {
                Log.Write($"EnterActionLabelShape Exception: {e.ToString()}");
            }
        }

        [Interaction(ColShapeEnums.ActionLabelShape, Out: true)]
        public static void OutActionLabelShape(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                sessionData.IsInLabelActionShape = false;
            }
            catch (Exception e)
            {
                Log.Write($"OutActionLabelShape Exception: {e.ToString()}");
            }
        }

        [Command("me", GreedyArg = true)]
        public static void CMD_chatMe(ExtPlayer player, string msg)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.Unmute > 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMutedMins, characterData.Unmute / 60), 3000);
                    return;
                }
                RPChat("me", player, RainbowExploit(msg));
                BattlePass.Repository.UpdateReward(player, 31);
                GameLog.AddInfo($"(MEChat) player({characterData.UUID}) {msg}");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_chatMe Exception: {e.ToString()}");
            }
        }

        [Command("do", GreedyArg = true)]
        public static void CMD_chatDo(ExtPlayer player, string msg)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.Unmute > 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMutedMins, characterData.Unmute / 60), 3000);
                    return;
                }
                RPChat("do", player, RainbowExploit(msg));
                BattlePass.Repository.UpdateReward(player, 79);
                GameLog.AddInfo($"(DOChat) player({characterData.UUID}) {msg}");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_chatDo Exception: {e.ToString()}");
            }
        }

        [Command("todo", GreedyArg = true)]
        public static void CMD_chatToDo(ExtPlayer player, string msg)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.Unmute > 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMutedMins, characterData.Unmute / 60), 3000);
                    return;
                }
                RPChat("todo", player, RainbowExploit(msg));
                BattlePass.Repository.UpdateReward(player, 100);
                GameLog.AddInfo($"(TODOChat) player({characterData.UUID}) {msg}");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_chatToDo Exception: {e.ToString()}");
            }
        }

        [Command("w", GreedyArg = true)]
        public static void CMD_chatWhisper(ExtPlayer player, int id, string msg)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.Unmute > 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMutedMins, characterData.Unmute / 60), 3000);
                    return;
                }
                RPChat("w", player, RainbowExploit(msg), Main.GetPlayerByID(id));
            }
            catch (Exception e)
            {
                Log.Write($"CMD_chatWhisper Exception: {e.ToString()}");
            }
        }

        [Command("s", GreedyArg = true)]
        public static void CMD_chatS(ExtPlayer player, string msg)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.Unmute > 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMutedMins, characterData.Unmute / 60), 3000);
                    return;
                }
                string testmsg = msg.ToLower();
                if (Main.stringDefaultBlock.Any(c => testmsg.Contains(c))) return;
                RPChat("s", player, RainbowExploit(msg));
                GameLog.AddInfo($"(SChat) player({characterData.UUID}) {msg}");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_chatS Exception: {e.ToString()}");
            }
        }

        [Command("b", GreedyArg = true)]
        public static void CMD_chatB(ExtPlayer player, string msg)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.Unmute > 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMutedMins, characterData.Unmute / 60), 3000);
                    return;
                }
                RPChat("b", player, RainbowExploit(msg));
                GameLog.AddInfo($"(BChat) player({characterData.UUID}) {msg}");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_chatB Exception: {e.ToString()}");
            }
        }

        [Command("m", GreedyArg = true)]
        public static void CMD_chatM(ExtPlayer player, string msg)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.Unmute > 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMutedMins, characterData.Unmute / 60), 3000);
                    return;
                }
                RPChat("m", player, RainbowExploit(msg));
                GameLog.AddInfo($"(MChat) player({characterData.UUID}) {msg}");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_chatM Exception: {e.ToString()}");
            }
        }

        [Command("t", GreedyArg = true)]
        public static void CMD_chatT(ExtPlayer player, string msg)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.Unmute > 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMutedMins, characterData.Unmute / 60), 3000);
                    return;
                }
                RPChat("t", player, RainbowExploit(msg));
                GameLog.AddInfo($"(TChat) player({characterData.UUID}) {msg}");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_chatT Exception: {e.ToString()}");
            }
        }

        [Command("try", GreedyArg = true)]
        public static void CMD_chatTry(ExtPlayer player, string msg)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                Try(player, RainbowExploit(msg));
                GameLog.AddInfo($"(TRYChat) player({characterData.UUID}) {msg}");
                BattlePass.Repository.UpdateReward(player, 82);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_chatTry Exception: {e.ToString()}");
            }
        }

        [Command("roll")]
        public static void CMD_chatRoll(ExtPlayer player, int first, int second)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.Unmute > 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMutedMins, characterData.Unmute / 60), 3000);
                    return;
                }
                if (Main.IHaveDemorgan(player)) return;
                if (first < 0 || second < 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Минимальное значение обоих чисел - 0.", 3000);
                    return;
                }
                if (first >= 65536 || second >= 65536)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Максимальное значение чисел - 65535.", 3000);
                    return;
                }
                if (second < first)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Второе число должно быть больше, чем первое.", 3000);
                    return;
                }
                Roll(player, first, second);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_chatRoll Exception: {e.ToString()}");
            }
        }
        
        #region Try command handler
        private static void Try(ExtPlayer sender, string message)
        {
            try
            {
                var characterData = sender.GetCharacterData();
                if (characterData == null) return;
                if (characterData.Unmute > 0)
                {
                    Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMutedMins, characterData.Unmute / 60), 3000);
                    return;
                }
                if (Main.IHaveDemorgan(sender)) return;
                int result = Main.rnd.Next(4);
                switch (result)
                {
                    case 0:
                    case 2:
                        foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(sender.Position, 10f,
                                     UpdateData.GetPlayerDimension(sender)))
                        {
                            Trigger.ClientEvent(foreachPlayer, "sendRPMessage", "try", "!{#E066FF}{name} " + message + " | !{#277C6B}" + " удачно", new int[] { sender.Value });
                            ChatHeadOverlay.SendOverlayMessage(foreachPlayer, sender.Value, ChatHeadOverlay.MessageType.Try, message, true);
                        }
                        BattlePass.Repository.UpdateReward(sender, 120);
                        return;
                    default:
                        foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(sender.Position, 10f,
                                     UpdateData.GetPlayerDimension(sender)))
                        {
                            Trigger.ClientEvent(foreachPlayer, "sendRPMessage", "try", "!{#E066FF}{name} " + message + " | !{#FF0707}" + " неудачно", new int[] { sender.Value });
                            ChatHeadOverlay.SendOverlayMessage(foreachPlayer, sender.Value, ChatHeadOverlay.MessageType.Try, message, false);
                        }
                        return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"Try Exception: {e.ToString()}");
            }
        }

        private static void Roll(ExtPlayer sender, int first, int second)
        {
            try
            {
                int result = Main.rnd.Next(first, second + 1);
                foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(sender.Position, 10f, UpdateData.GetPlayerDimension(sender))) 
                    Trigger.ClientEvent(foreachPlayer, "sendRPMessage", "roll", "!{#FF8C00}{name} выбросил число " + result + " | !{#e69500}" + first + " - " + second, new int[] { sender.Value });
            }
            catch (Exception e)
            {
                Log.Write($"Roll Exception: {e.ToString()}");
            }
        }
        #endregion Try command handler

        #region RP Chat
        public static void RPChat(string cmd, ExtPlayer sender, string message, ExtPlayer target = null, string optName = "")
        {
            try
            {
                var characterData = sender.GetCharacterData();
                if (characterData == null) return;
                if (Main.IHaveDemorgan(sender)) return;
                int[] names = new int[] { sender.Value };
                if (target != null) names = new int[] { sender.Value, target.Value };
                switch (cmd)
                {
                    case "me":
                        foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(sender.Position, 10f,
                                     UpdateData.GetPlayerDimension(sender)))
                        {
                            Trigger.ClientEvent(foreachPlayer, "sendRPMessage", "me", "!{#E066FF}{name} " + message, names);
                            ChatHeadOverlay.SendOverlayMessage(foreachPlayer, sender.Value, ChatHeadOverlay.MessageType.Me, message, false);
                        }
                            
                        return;
                    case "sme":
                        foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(sender.Position, 10f,
                                     UpdateData.GetPlayerDimension(sender)))
                        {
                            Trigger.ClientEvent(foreachPlayer, "sendRPMessage", "me", "!{#9466ff}{name} " + message, names);
                            ChatHeadOverlay.SendOverlayMessage(foreachPlayer, sender.Value, ChatHeadOverlay.MessageType.Me, message, false);
                        }
                            
                        return;
                    case "todo":
                        if (message.IndexOf('*') >= 0)
                        {
                            string[] args = message.Split('*');
                            string msg = args[0];
                            string action = args[1];
                            if (msg.Length >= 1 && action.Length >= 1)
                            {
                                string genderCh = (characterData.Gender) ? "" : "а";
                                foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(sender.Position, 10f, UpdateData.GetPlayerDimension(sender)))
                                    Trigger.ClientEvent(foreachPlayer, "sendRPMessage", "todo", msg + ",!{#E066FF} - сказал" + genderCh + " {name}, " + action, names);
                            }
                        }
                        return;
                    case "do":
                        foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(sender.Position, 10f,
                                     UpdateData.GetPlayerDimension(sender)))
                        {
                            Trigger.ClientEvent(foreachPlayer, "sendRPMessage", "do", "!{#E066FF}" + message + " ({name})", names);
                            ChatHeadOverlay.SendOverlayMessage(foreachPlayer, sender.Value, ChatHeadOverlay.MessageType.Do, message, false);
                        }
                        return;
                    case "sdo":
                        foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(sender.Position, 10f,
                                     UpdateData.GetPlayerDimension(sender)))
                        {
                            Trigger.ClientEvent(foreachPlayer, "sendRPMessage", "do", "!{#9466ff}" + message + " ({name})", names);
                            ChatHeadOverlay.SendOverlayMessage(foreachPlayer, sender.Value, ChatHeadOverlay.MessageType.Do, message, false);
                        }
                        return;
                    case "s":
                        foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(sender.Position, 30f,
                                     UpdateData.GetPlayerDimension(sender)))
                        {
                            Trigger.ClientEvent(foreachPlayer, "sendRPMessage", "s", "{name} кричит: " + message, names);
                            ChatHeadOverlay.SendOverlayMessage(foreachPlayer, sender.Value, ChatHeadOverlay.MessageType.Message, message, false);
                        }
                        BattlePass.Repository.UpdateReward(sender, 81);
                        return;
                    case "b":
                        foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(sender.Position, 10f, UpdateData.GetPlayerDimension(sender)))
                            Trigger.ClientEvent(foreachPlayer, "sendRPMessage", "b", "(( {name}: " + message + " ))", names);
                        return;
                    case "sb":
                        foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(sender.Position, 10f,
                                     UpdateData.GetPlayerDimension(sender)))
                        {
                            if (string.IsNullOrEmpty(optName)) Trigger.ClientEvent(foreachPlayer, "sendRPMessage", "b", "!{#d4d4d4}(( {name} " + message + " ))", names);
                            else Trigger.ClientEvent(foreachPlayer, "sendRPMessage", "b", "!{#d4d4d4}(( "+ $"{optName} " + message + " ))", names);
                        }
                            
                        return;
                    case "m":
                        var fracId = sender.GetFractionId();
                        if ((!Configs.IsFractionPolic(fracId) && fracId != (int)Fractions.Models.Fractions.ARMY &&fracId != (int)Fractions.Models.Fractions.MERRYWEATHER) || !NAPI.Player.IsPlayerInAnyVehicle(sender)) return;
                        var vehicle = (ExtVehicle) sender.Vehicle;
                        var vehicleLocalData = vehicle.GetVehicleLocalData();
                        if (vehicleLocalData != null)
                        {
                            if (vehicleLocalData.Access != VehicleAccess.Fraction) return;
                            if (!Configs.IsFractionPolic(vehicleLocalData.Fraction) && vehicleLocalData.Fraction != (int)Fractions.Models.Fractions.ARMY && vehicleLocalData.Fraction != (int)Fractions.Models.Fractions.MERRYWEATHER) return;
                            foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(sender.Position, 120f, UpdateData.GetPlayerDimension(sender))) 
                                Trigger.ClientEvent(foreachPlayer, "sendRPMessage", "m", "!{#FFA500}[Мегафон] {name}: " + message, names);
                        }
                        return;
                    case "t":
                        if (characterData.WorkID != (int)JobsId.Trucker) return;
                        foreach (ExtPlayer foreachPlayer in NeptuneEvo.Character.Repository.GetPlayers())
                        {
                            var foreachSessionData = foreachPlayer.GetSessionData();
                            if (foreachSessionData == null) continue;
                            var foreachCharacterData = foreachPlayer.GetCharacterData();
                            if (foreachCharacterData == null) continue;
                            if (foreachCharacterData.WorkID == (int)JobsId.Trucker)
                            {
                                if (foreachSessionData.WorkData.OnWork && foreachPlayer.IsInVehicle) Trigger.SendChatMessage(foreachPlayer, $"~y~[Рация дальнобойщиков] [{sender.Name}]: {message}");
                            }
                        }
                        return;
                    case "w":
                        var targetCharacterData = target.GetCharacterData();
                        if (targetCharacterData == null || sender == target || sender.Position.DistanceTo(target.Position) >= 2)
                        {
                            Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок слишком далеко.", 3000);
                            return;
                        }
                        foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(sender.Position, 10f, UpdateData.GetPlayerDimension(sender)))
                        {
                            var foreachCharacterData = foreachPlayer.GetCharacterData();
                            if (foreachCharacterData == null) continue;
                            if (foreachPlayer.IsCharacterData() && foreachCharacterData.AdminLVL >= 1) Trigger.SendChatMessage(foreachPlayer, "!{#C5C7C7}" + $"[Шепот] {sender.Name} ({sender.Value}): {message}");
                            else if (foreachPlayer != sender && foreachPlayer != target) Trigger.ClientEvent(foreachPlayer, "sendRPMessage", "w", "!{#C5C7C7}{name} прошептал что-то {name}", names);
                        }
                        Trigger.SendChatMessage(sender, "!{#C5C7C7}" + $"[Шепот] {sender.Name.Replace('_', ' ')} ({sender.Value}): {message}");

                        var targetFriends = targetCharacterData.Friends;
                        if (targetFriends.ContainsKey(sender.Name))
                        {
                            if (!targetFriends[sender.Name])
                            {
                                Trigger.SendChatMessage(target, "!{#C5C7C7}" + $"[Шепот] {sender.Name.Split('_')[0]} ({sender.Value}): {message}");
                            }
                            else
                            {
                                Trigger.SendChatMessage(target, "!{#C5C7C7}" + $"[Шепот] {sender.Name.Replace('_', ' ')} ({sender.Value}): {message}");
                            }
                        }
                        else Trigger.SendChatMessage(target, "!{#C5C7C7}" + $"[Шепот] " + (characterData.Gender ? "Незнакомец" : "Незнакомка") + $" ({sender.Value}): {message}");
                        GameLog.AddInfo($"(WChat) player({characterData.UUID})->player({targetCharacterData.UUID}) {message}");
                        BattlePass.Repository.UpdateReward(sender, 80);
                        return;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"RPChat Exception: {e.ToString()}");
            }
        }
        #endregion RP Chat
    }
}