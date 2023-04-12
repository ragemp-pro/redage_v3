using System;
using System.Collections.Generic;
using System.Data;
using GTANetworkAPI;
using NeptuneEvo.Core;
using Redage.SDK;
using NeptuneEvo.GUI;

using Newtonsoft.Json;
using MySqlConnector;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Chars;
using System.Linq;
using Database;
using LinqToDB;
using Localization;
using NeptuneEvo.Functions;
using NeptuneEvo.Quests;
using NeptuneEvo.Houses;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Handles;
using NeptuneEvo.Quests.Models;
using NeptuneEvo.Table.Models;
using NeptuneEvo.Table.Tasks.Models;
using NeptuneEvo.Table.Tasks.Player;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;

namespace NeptuneEvo.Fractions
{
    class Police : Script
    {
        private static readonly nLog Log = new nLog("Fractions.Police");

        private static int PedSecretaryId = 0;

        public static Dictionary<string, (string, string)> WantedVehicles = new Dictionary<string, (string, string)>();

        public static Dictionary<ExtPlayer, ExtBlip> PoliceCalls = new Dictionary<ExtPlayer, ExtBlip>();//ToDo
        public static Dictionary<int, ExtBlip> PoliceHouseCalls = new Dictionary<int, ExtBlip>();//ToDo
        public static Dictionary<int, ExtBlip> PoliceCarsCalls = new Dictionary<int, ExtBlip>();//ToDo
        public static Dictionary<int, ExtBlip> PoliceSafesCalls = new Dictionary<int, ExtBlip>();//ToDo

        private static Vector3 ArrestPosition = new Vector3(481.40088, -1010.83453, 26.273132);
        public static Vector3 GunsPosition = new Vector3(479.0797, -996.76697, 30.69196);
        public static Vector3 CloakroomPosition = new Vector3(463.51648, -996.40875, 30.68949);
        private static Vector3 CloakroomSpecialPosition = new Vector3(463.5043, -999.32587, 30.68949);
        public static Vector3 PrisonPosition = new Vector3(480.95743, -1014.3073, 26.27313);
        public static Vector3 ExitPrisonPosition = new Vector3(477.8819, -976.16113, 27.98412);
        private static Vector3 StockPosition = new Vector3(486.9647, -995.9626, 30.68962);
        private static Vector3 VehicleBoostPosition = new Vector3(1408.2571, 183.5331, -49.280865);//463.32733, -1019.0836, 28.10166
        private static Vector3 VehicleRepairPosition = new Vector3(447.07394, -1013.2826, 28.541023);
        

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                Main.CreateBlip(new Main.BlipData(526, "Police", GunsPosition, 38, true));

                CustomColShape.CreateCylinderColShape(ArrestPosition, 6, 3, 0, ColShapeEnums.FractionPolicArrest, 0);
                //CustomColShape.CreateCylinderColShape(policeCheckpoints[1], 1, 2, 0, ColShapeEnums.FractionPolic, 1);

                CustomColShape.CreateCylinderColShape(CloakroomPosition, 1, 2, 0, ColShapeEnums.FractionPolic, 2);
                NAPI.Marker.CreateMarker(30, CloakroomPosition, new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220));

                CustomColShape.CreateCylinderColShape(CloakroomSpecialPosition, 1, 2, 0, ColShapeEnums.FractionPolic, 3);
                NAPI.Marker.CreateMarker(30, CloakroomSpecialPosition, new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220));

                //CustomColShape.CreateCylinderColShape(policeCheckpoints[7], 1, 2, 0, ColShapeEnums.FractionPolic, 4);
                CustomColShape.CreateCylinderColShape(StockPosition, 1, 2, 0, ColShapeEnums.FractionPolic, 5);
                NAPI.Marker.CreateMarker(20, StockPosition, new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220));

                CustomColShape.CreateCylinderColShape(VehicleBoostPosition, 3, 5, 1, ColShapeEnums.FractionPolic, 6);
                NAPI.Marker.CreateMarker(44, VehicleBoostPosition, new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220), dimension: 1);

                
                CustomColShape.CreateCylinderColShape(VehicleRepairPosition, 3, 5, 0, ColShapeEnums.FractionPolic, 9); // repair
                NAPI.Marker.CreateMarker(1, new Vector3(VehicleRepairPosition.X, VehicleRepairPosition.Y, VehicleRepairPosition.Z - 1.75), new Vector3(), new Vector3(), 3f, new Color(255, 255, 255, 220));
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Ремонт наземного транспорта"), VehicleRepairPosition, 5F, 0.3F, 0, new Color(255, 255, 255));

                Ped ped = PedSystem.Repository.CreateQuest("s_m_y_cop_01", new Vector3(442.6866f, -981.88605f, 30.689486f), 89.897484f, title: "~y~NPC~w~ Офицер Энтони\nДежурный", colShapeEnums: ColShapeEnums.FracPolic);
                PedSecretaryId = ped.Value;

                PedSystem.Repository.CreateQuest("s_m_m_fibsec_01", new Vector3(480.34195, -996.7015, 30.690102), 88.88336f, title: "~y~NPC~w~ Агент Роберт\nСкладской", colShapeEnums: ColShapeEnums.FracPolic);
                PedSystem.Repository.CreateQuest("s_f_y_cop_01", new Vector3(442.5702, -978.62854, 30.689486), 100.0675f, title: "~y~NPC~w~ Офицер Лиса\nВызвать сотрудника", colShapeEnums: ColShapeEnums.CallPoliceMember);
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }
        
        [Interaction(ColShapeEnums.CallPoliceMember)] 
        public static void OpenCallPoliceMemberDialog(ExtPlayer player, int _) 
        { 
            try 
            { 
                var sessionData = player.GetSessionData(); 
                if (sessionData == null) return; 
 
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
 
                Trigger.ClientEvent(player, "openDialog", "CallPoliceMemberDialog", LangFunc.GetText(LangType.Ru, DataName.AreYouWantToCallGov)); 
            } 
            catch (Exception e) 
            { 
                Log.Write($"OpenCallPoliceMemberDialog Exception: {e.ToString()}"); 
            } 
        }
        
        [Interaction(ColShapeEnums.FracPolic)]
        public static void Open(ExtPlayer player, int index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            
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
            if (PedSecretaryId == index)
            {
                BattlePass.Repository.UpdateReward(player, 84);
                player.SelectQuest(new PlayerQuestModel("npc_fracpolic", 0, 0, false, DateTime.Now));
                Trigger.ClientEvent(player, "client.quest.open", index, "npc_fracpolic", 0, 0, 0);
            }
            else
            {
                var fractionData = player.GetFractionData();
                if (fractionData == null || fractionData.Id != (int) Models.Fractions.POLICE)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoPolice), 3000);
                    return;
                }
                if (!fractionData.IsOpenStock)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseClosed), 3000);
                    return;
                }
                if (!sessionData.WorkData.OnDuty)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000);
                    return;
                }
                OpenPoliceGunMenu(player);
                return;
            }
        }
        public static void Perform(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            InventoryItemData Bags = Chars.Repository.GetItemData(player, "accessories", 8);
            if (Bags.ItemId != ItemId.BagWithDrill && Bags.ItemId != ItemId.BagWithMoney)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoBagDrillMoney), 3000);
                return;
            }
            Chars.Repository.RemoveIndex(player, "accessories", 8);
            MoneySystem.Wallet.Change(player, Main.PoliceAward);
            GameLog.Money($"server", $"player({characterData.UUID})", Main.PoliceAward, $"policeAward");
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Reward, Main.PoliceAward), 3000);
            return;
        }

        [ServerEvent(Event.PlayerExitVehicle)]
        public void Event_OnPlayerExitVehicle(ExtPlayer player, Vehicle vehicle)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (player.VehicleSeat != (int)VehicleSeat.Driver || player.VehicleSeat != (int)VehicleSeat.RightFront) return;
                if (!Configs.IsFractionPolic(player.GetFractionId())) return;
                Trigger.ClientEvent(player, "closePc");
            }
            catch (Exception e)
            {
                Log.Write($"Event_OnPlayerExitVehicle Exception: {e.ToString()}");
            }
        }

        public static string OnCallPolice(ExtPlayer player, string reason)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return "Что то не так :-(";
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return "Что то не так :-(";
                if (PoliceCalls.ContainsKey(player)) 
                    return "Вы уже сделали вызов";

                if (Manager.FractionMembersCount(new [] {(int) Models.Fractions.POLICE, (int) Models.Fractions.FIB, (int) Models.Fractions.SHERIFF}) == 0)
                    return LangFunc.GetText(LangType.Ru, DataName.NoPoliceNear);
                
                if (characterData.AdminLVL == 0 && DateTime.Now < sessionData.TimingsData.NextCallPolice)
                    return LangFunc.GetText(LangType.Ru, DataName.AlreadyCallPolice);
                
                if (characterData.Unmute > 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMutedMins, characterData.Unmute / 60), 3000);
                    return "К сожалению, мы не можем принять ваш вызов :(";
                }
                
                sessionData.TimingsData.NextCallPolice = DateTime.Now.AddMinutes(7);

                var blip = (ExtBlip)NAPI.Blip.CreateBlip(0, player.Position, 1, 70, $"Police Call from {player.Name.Replace('_', ' ')} [{player.Value}]", 0, 0, true, 0, 0);
                blip.Transparency = 0;
                PoliceCalls.Add(player, blip);

                NAPI.Task.Run(() =>
                {
                    try
                    {
                        Selecting.onPlayerDisconnectedhandler(player);
                    }
                    catch (Exception e)
                    {
                        Log.Write($"OnCallPolice Task Exception: {e.ToString()}");
                    }
                }, 60000);

                foreach (var foreachPlayer in Character.Repository.GetPlayers())
                {
                    var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                    if (foreachMemberFractionData == null) 
                        continue;
                    
                    if (!Configs.IsFractionPolic(foreachMemberFractionData.Id))
                        continue;
                    
                    Trigger.ClientEvent(foreachPlayer, "changeBlipAlpha", blip, 255);
                }
                //Manager.sendFractionMessage((int) Models.Fractions.Models.Fractions.POLICE, $"Поступил вызов от ({player.Value}) - {reason}");
                Manager.sendFractionMessage((int) Models.Fractions.POLICE, "!{#F08080}[F] " + LangFunc.GetText(LangType.Ru, DataName.PoliceCallFrom, player.Name, player.Value, reason), true);
                //Manager.sendFractionMessage(9, $"Поступил вызов от ({player.Value}) - {reason}");
                Manager.sendFractionMessage((int) Models.Fractions.FIB, "!{#F08080}[F] " + LangFunc.GetText(LangType.Ru, DataName.PoliceCallFrom, player.Name, player.Value, reason), true);
                Manager.sendFractionMessage((int) Models.Fractions.SHERIFF, "!{#F08080}[F] " + LangFunc.GetText(LangType.Ru, DataName.PoliceCallFrom, player.Name, player.Value, reason), true);

                if (characterData.Gender) 
                    Commands.RPChat("sme", player, LangFunc.GetText(LangType.Ru, DataName.HeCallPolice));
                else 
                    Commands.RPChat("sme", player, LangFunc.GetText(LangType.Ru, DataName.SheCallPolice));
                
                return "Вызов принят";
            }
            catch (Exception e)
            {
                Log.Write($"OnCallPolice Exception: {e.ToString()}");
            }
            return "Что то не так :-(";
        }

        public static void acceptCall(ExtPlayer player, int id)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                var fracId = player.GetFractionId();

                if (!sessionData.WorkData.OnDuty && Manager.FractionTypes[fracId] == FractionsType.Gov)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000);
                    return;
                }

                ExtPlayer target = Main.GetPlayerByID(id);

                if (target != null && target.IsCharacterData() && PoliceCalls.ContainsKey(target))
                {
                    Blip blip = PoliceCalls[target];
                    if (blip != null && blip.Exists) Trigger.ClientEvent(player, "createWaypoint", blip.Position.X, blip.Position.Y);

                    if (Configs.IsFractionPolic(fracId))
                    {
                        Manager.sendFractionMessage(fracId, LangFunc.GetText(LangType.Ru, DataName.CallAccept, player.Name.Replace("_"," "), target.Value));
                        Manager.sendFractionMessage(fracId, "!{#F08080}[F] " + LangFunc.GetText(LangType.Ru, DataName.CallAccept, player.Name.Replace("_"," "), target.Value), true);
                        player.AddTableScore(TableTaskId.Item5);
                    }

                    Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCallAccepted, player.Value), 3000);
                }
                else if (PoliceHouseCalls.ContainsKey(id))
                {
                    Blip blip = PoliceHouseCalls[id];
                
                    if (blip != null && blip.Exists)
                    {
                        Trigger.ClientEvent(player, "changeBlipAlpha", blip, 255);
                        Trigger.ClientEvent(player, "createWaypoint", blip.Position.X, blip.Position.Y);
                    }

                    if (Configs.IsFractionPolic(fracId))
                    {
                        Manager.sendFractionMessage(fracId, $"{player.Name.Replace('_', ' ')} принял вызов из дома №{id}.");
                        Manager.sendFractionMessage(fracId, "!{#F08080}[F] " + $"{player.Name.Replace('_', ' ')} принял вызов из дома №{id}", true);
                        player.AddTableScore(TableTaskId.Item5);
                    }
                }
                else if (PoliceCarsCalls.ContainsKey(id))
                {
                    Blip blip = PoliceCarsCalls[id];
                
                    if (blip != null && blip.Exists)
                    {
                        Trigger.ClientEvent(player, "changeBlipAlpha", blip, 255);
                        Trigger.ClientEvent(player, "createWaypoint", blip.Position.X, blip.Position.Y);
                    }

                    if (Configs.IsFractionPolic(fracId))
                    {
                        Manager.sendFractionMessage(fracId, $"{player.Name.Replace('_', ' ')} принял вызов сигнализации машины №{id}.");
                        Manager.sendFractionMessage(fracId, "!{#F08080}[F] " + $"{player.Name.Replace('_', ' ')} принял вызов сигнализации машины №{id}", true);
                        player.AddTableScore(TableTaskId.Item5);
                        player.AddTableScore(TableTaskId.Item8);
                    }
                }
                else if (PoliceSafesCalls.ContainsKey(id))
                {
                    Blip blip = PoliceSafesCalls[id];
                
                    if (blip != null && blip.Exists)
                    {
                        Trigger.ClientEvent(player, "createWaypoint", blip.Position.X, blip.Position.Y);
                    }

                    if (Configs.IsFractionPolic(fracId))
                    {
                        Manager.sendFractionMessage(fracId, $"{player.Name.Replace('_', ' ')} принял вызов по ограблению сейфа.");
                        Manager.sendFractionMessage(fracId, "!{#F08080}[F] " + $"{player.Name.Replace('_', ' ')} принял вызов по ограблению сейфа.", true);
                        player.AddTableScore(TableTaskId.Item5);
                        player.AddTableScore(TableTaskId.Item7);
                    }
                }
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindCall), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"acceptCall Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("clearWantedLvl")]
        public static void clearWantedLvl(ExtPlayer sender, params object[] arguments)
        {
            try
            {
                if (!sender.IsCharacterData()) return;
                string target = (string)arguments[0];
                string vehiclenumb = "null";
                ExtPlayer player = null;
                try
                {
                    int pasport = Convert.ToInt32(target);
                    if (!Main.PlayerNames.ContainsKey(pasport))
                    {
                        Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomRight, LangFunc.GetText(LangType.Ru, DataName.PassNumberDoesnt), 3000);
                        return;
                    }
                    player = (ExtPlayer)NAPI.Player.GetPlayerFromName(Main.PlayerNames[pasport]);
                    target = Main.PlayerNames[pasport];
                }
                catch
                {
                    target.Replace(' ', '_');
                    if (!Main.PlayerNames.Values.Contains(target))
                    {
                        if (VehicleManager.IsVehicleToNumber(target))
                        {
                            vehiclenumb = target;
                        }
                        else
                        {
                            Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomRight, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerOrVeh), 3000);
                            return;
                        }
                    }
                    player = (ExtPlayer)NAPI.Player.GetPlayerFromName(target);
                }
                if (vehiclenumb.Equals("null"))
                {
                    if (!Main.PlayerUUIDs.ContainsKey(target))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindMan), 3000);
                        return;
                    }
                    int tauuid = Main.PlayerUUIDs[target];

                    Trigger.SetTask(async () =>
                    {
                        try
                        {
                            await using var db = new ServerBD("MainDB");//В отдельном потоке

                            await db.Characters
                                .Where(v => v.Uuid == tauuid)
                                .Set(v => v.Wanted, (string) null)
                                .UpdateAsync();
                        }
                        catch (Exception e)
                        {
                            Debugs.Repository.Exception(e);
                        }
                    });
                    
                    setPlayerWantedLevel(player, null);
                    Notify.Send(sender, NotifyType.Success, NotifyPosition.TopCenter, LangFunc.GetText(LangType.Ru, DataName.YouRemoveWantedPass, target), 3000);
                    Fractions.Table.Logs.Repository.AddLogs(sender, FractionLogsType.Su, $"Снял розыск с {target} ({tauuid})");
                    
                    foreach (var foreachPlayer in Character.Repository.GetPlayers())
                    {
                        var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                        if (foreachMemberFractionData == null) 
                            continue;
                        
                        if (foreachMemberFractionData.Id == (int) Models.Fractions.POLICE || foreachMemberFractionData.Id == (int) Models.Fractions.FIB)
                            Trigger.SendChatMessage(foreachPlayer, "!{#FF8C00}" + LangFunc.GetText(LangType.Ru, DataName.RemoveWantedFrom, sender.Name, target));
                    }
                }
                else
                {
                    if (WantedVehicles.ContainsKey(vehiclenumb))
                    {
                        WantedVehicles.Remove(vehiclenumb);
                        foreach (var foreachPlayer in Character.Repository.GetPlayers())
                        {
                            var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                            if (foreachMemberFractionData == null) 
                                continue;
                            
                            if (Configs.IsFractionPolic(foreachMemberFractionData.Id)) 
                                Trigger.ClientEvent(foreachPlayer, "clearVehicleWanted", vehiclenumb);
                        }
                        Notify.Send(sender, NotifyType.Success, NotifyPosition.TopCenter, LangFunc.GetText(LangType.Ru, DataName.YouRemoveWantedVeh, vehiclenumb), 3000);
                        Fractions.Table.Logs.Repository.AddLogs(sender, FractionLogsType.Su, $"Снял розыск с т/с ({vehiclenumb})");
                    }
                    else
                    {
                        Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomRight, LangFunc.GetText(LangType.Ru, DataName.PersonalVehWanted), 3000);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"clearWantedLvl Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("checkNumber")]
        public static void checkNumber(ExtPlayer sender, params object[] arguments)
        {
            try
            {
                if (!sender.IsCharacterData()) return;
                string number = (string)arguments[0];
                var vehicleData = VehicleManager.GetVehicleToNumber(number);
                if (vehicleData == null)
                {
                    Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoCarWithThisNumber), 3000);
                    return;
                }
                Trigger.ClientEvent(sender, "executeCarInfo", Convert.ToString(vehicleData.Model), vehicleData.Holder.Replace('_', ' '));
            }
            catch (Exception e)
            {
                Log.Write($"checkNumber Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("checkPerson")]
        public static void checkPerson(ExtPlayer player, params object[] arguments)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                string data = (string)arguments[0];
                ExtPlayer target = null;
                try
                {
                    int pasport = Convert.ToInt32(data);
                    if (!Main.PlayerNames.ContainsKey(pasport))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomRight, LangFunc.GetText(LangType.Ru, DataName.PassNumberDoesnt), 3000);
                        return;
                    }
                    target = (ExtPlayer)NAPI.Player.GetPlayerFromName(Main.PlayerNames[pasport]);
                    data = Main.PlayerNames[pasport];
                }
                catch
                {
                    data.Replace(' ', '_');
                    if (!Main.PlayerNames.Values.Contains(data))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomRight, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayer), 3000);
                        return;
                    }
                    target = (ExtPlayer)NAPI.Player.GetPlayerFromName(data);
                }

                try
                {
                    var targetCharacterData = target.GetCharacterData();
                    var fraction_name = target.GetFractionName();
                    fraction_name = fraction_name == String.Empty ? "Отсутствует" : fraction_name;

                    string numberInfo = targetCharacterData.Sim == -1 ? "Отсутствует" : targetCharacterData.Sim.ToString();

                    int wantedLvl = (targetCharacterData.WantedLVL == null) ? 0 : targetCharacterData.WantedLVL.Level;
                    string gender = (targetCharacterData.Gender) ? LangFunc.GetText(LangType.Ru, DataName.Mans) : LangFunc.GetText(LangType.Ru, DataName.Womens);
                    
                    string lic = "";
                    for (int i = 0; i < targetCharacterData.Licenses.Count; i++) if (targetCharacterData.Licenses[i]) lic += $"{Main.LicWords[i]} / ";
                    if (lic == "") lic = LangFunc.GetText(LangType.Ru, DataName.Nothing);

                    string houseInfo = "-";
                    House house = HouseManager.GetHouse($"{targetCharacterData.FirstName}_{targetCharacterData.LastName}", true);
                    if (house != null) houseInfo = $"#{house.ID}";
                    
                    Trigger.ClientEvent(player, "executePersonInfo", $"{targetCharacterData.FirstName}", $"{targetCharacterData.LastName}", $"{targetCharacterData.UUID}", $"{fraction_name}", $"{numberInfo}", $"{gender}", $"{wantedLvl}", $"{lic}", $"{houseInfo}");
                }
                catch
                {
                    if (!Main.PlayerUUIDs.ContainsKey(data))
                    {
                        Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindMan), 3000);
                        return;
                    }
                    int targetuuid = Main.PlayerUUIDs[data];

                    Trigger.SetTask(async () =>
                    {
                        try
                        {
                            await using var db = new ServerBD("MainDB");//В отдельном потоке

                            var character = await db.Characters
                                .Select(c => new
                                {
                                    c.Uuid,
                                    c.Firstname,
                                    c.Lastname,
                                    c.Adminlvl,
                                    c.Gender,
                                    c.Sim,
                                    c.Wanted,
                                    c.Licenses,
                                })
                                .Where(v => v.Uuid == targetuuid)
                                .FirstOrDefaultAsync();

                            if (character == null)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Человек не найден", 3000);
                                return;
                            }
                                                        
                            var genderBool = Convert.ToBoolean(character.Gender);
                            var gender = (genderBool) ? LangFunc.GetText(LangType.Ru, DataName.Mans) : LangFunc.GetText(LangType.Ru, DataName.Womens);

                            var fraction_name = "Отсутствует";
                            
                            var memberFractionData = Fractions.Manager.GetFractionMemberData(character.Uuid);
                            if (memberFractionData != null)
                                fraction_name = Manager.FractionNames[memberFractionData.Id];
                            
                            var sim = Convert.ToInt32(character.Sim);
                            
                            var numberInfo = sim == -1 ? "Отсутствует" : sim.ToString();

                            var wanted = JsonConvert.DeserializeObject<WantedLevel>(character.Wanted);
                            var wantedLvl = (wanted == null) ? 0 : wanted.Level;
                            
                            var licenses = JsonConvert.DeserializeObject<List<bool>>(character.Licenses);
                            var lic = "";
                            for (var i = 0; i < licenses.Count; i++) 
                                if (licenses[i]) 
                                    lic += $"{Main.LicWords[i]} / ";
                            if (lic == "") lic = LangFunc.GetText(LangType.Ru, DataName.Nothing);

                            var houseInfo = "-";
                            var house = HouseManager.GetHouse($"{character.Firstname}_{character.Lastname}", true);
                            if (house != null) 
                                houseInfo = $"#{house.ID}";

                            Trigger.ClientEvent(player, "executePersonInfo", $"{character.Firstname}", $"{character.Lastname}", $"{character.Uuid}", $"{fraction_name}", $"{numberInfo}", $"{gender}", $"{wantedLvl}", $"{lic}", $"{houseInfo}");
                    
                        }
                        catch
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Человек не найден", 3000);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Log.Write($"checkPerson Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("checkWantedList")]
        public static void checkWantedList(ExtPlayer sender)
        {
            try
            {
                if (!sender.IsCharacterData() || !NAPI.Player.IsPlayerInAnyVehicle(sender)) return;
                var vehicle = (ExtVehicle)sender.Vehicle;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if (vehicleLocalData.Access == VehicleAccess.Fraction &&
                        Configs.IsFractionPolic(vehicleLocalData.Fraction) &&
                        sender.VehicleSeat == (int)VehicleSeat.Driver)
                    {
                        List<string> _WantedList = new List<string>();
                        foreach (var foreachPlayer in Character.Repository.GetPlayers())
                        {
                            var foreachCharacterData = foreachPlayer.GetCharacterData();
                            if (foreachCharacterData == null) continue;
                            int wantedLvl = (foreachCharacterData.WantedLVL == null) ? 0 : foreachCharacterData.WantedLVL.Level;
                            if (wantedLvl != 0) _WantedList.Add($"{foreachCharacterData.FirstName} {foreachCharacterData.LastName} - {wantedLvl}*");
                        }
                        lock (WantedVehicles)
                        {
                            if (WantedVehicles.Count >= 1)
                            {
                                foreach (KeyValuePair<string, (string, string)> p in WantedVehicles) _WantedList.Add($"{p.Value.Item2} [{p.Key}]");
                            }
                        }
                        string json = JsonConvert.SerializeObject(_WantedList);
                        Log.Debug(json);
                        Trigger.ClientEvent(sender, "executeWantedList", json);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"checkWantedList Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("openCopCarMenu")]
        public static void openCopcarmenu(ExtPlayer sender)
        {
            try
            {
                var characterData = sender.GetCharacterData();
                if (characterData == null) return;
                if (!NAPI.Player.IsPlayerInAnyVehicle(sender)) return;
                var vehicle = (ExtVehicle)sender.Vehicle;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if (vehicleLocalData.Access == VehicleAccess.Fraction &&
                        Configs.IsFractionPolic(vehicleLocalData.Fraction) &&
                        sender.VehicleSeat == (int)VehicleSeat.Driver)
                    {
                        if (Configs.IsFractionPolic(sender.GetFractionId()))
                        {
                            Trigger.ClientEvent(sender, "openPc");
                            if (characterData.Gender) Commands.RPChat("sme", sender, LangFunc.GetText(LangType.Ru, DataName.HeOnComputer));
                            else Commands.RPChat("sme", sender, LangFunc.GetText(LangType.Ru, DataName.SheOnComputer));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"openCopcarmenu Exception: {e.ToString()}");
            }
        }

        public static void PlayerSoloArrest(ExtPlayer player)
        {
            try
            {
                if (!FunctionsAccess.IsWorking("PlayerSoloArrest"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (characterData.ArrestTime != 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouAlreadyInPrison), 3000);
                    return;
                }
                else if (characterData.WantedLVL == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouNotWanted), 3000);
                    return;
                }
                else if (Main.IHaveDemorgan(player, true)) return;

                player.Eval($"mp.game.audio.playSoundFrontend(-1, \"Mission_Pass_Notify\", \"DLC_HEISTS_GENERAL_FRONTEND_SOUNDS\", true);");

                characterData.ArrestTime = Convert.ToInt32(((characterData.WantedLVL.Level >= 5) ? 3600 : (characterData.WantedLVL.Level * 10 * 60)) / 2);
                
                if (player.Position.DistanceTo(Sheriff.FirstPrisonPosition) <= 150) characterData.ArrestType = 1;
                else if (player.Position.DistanceTo(Sheriff.SecondPrisonPosition) <= 150) characterData.ArrestType = 2;
                else characterData.ArrestType = 0;
                
                int minutes = Convert.ToInt32(characterData.ArrestTime / 60);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouArrested, minutes), 3000);

                FractionCommands.arrestPlayer(player);

                Manager.sendFractionMessage((int) Models.Fractions.POLICE, "!{#FF8C00}[F] " + LangFunc.GetText(LangType.Ru, DataName.PoliceSdalsya, player.Name, minutes), true);
                Manager.sendFractionMessage((int) Models.Fractions.SHERIFF, "!{#FF8C00}[F] " + LangFunc.GetText(LangType.Ru, DataName.PoliceSdalsya, player.Name, minutes), true);
                Manager.sendFractionMessage((int) Models.Fractions.FIB, "!{#FF8C00}[F] " + LangFunc.GetText(LangType.Ru, DataName.PoliceSdalsya, player.Name, minutes), true);
                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.Arrest, $"{player.Name} ({characterData.UUID}) сдался с поличным и был посажен в КПЗ на {minutes} минут.");
            }
            catch (Exception e)
            {
                Log.Write($"PlayerSoloArrest Exception: {e.ToString()}");
            }
        }

        public static readonly Dictionary<ItemId, int> IllegalsItems = new Dictionary<ItemId, int>
        {
            { ItemId.Drugs, 2 },
            { ItemId.BagWithDrill, 1000 },
            { ItemId.Lockpick, 15 },
            { ItemId.ArmyLockpick, 75 },
            { ItemId.Cuffs, 25 },
            { ItemId.Pocket, 25 },
            { ItemId.Pistol, 36 },
            { ItemId.SNSPistol, 34 },
            { ItemId.CeramicPistol, 38 },
            { ItemId.HeavyPistol, 72 },
            { ItemId.VintagePistol, 40 },
            { ItemId.Pistol50, 55 },
            { ItemId.DoubleBarrelShotgun, 91 },
            { ItemId.SawnOffShotgun, 99 },
            { ItemId.PumpShotgun, 122 },
            { ItemId.MicroSMG, 115 },
            { ItemId.SMGMk2, 190 },
            { ItemId.MachinePistol, 108 },
            { ItemId.MiniSMG, 122 },
            { ItemId.CombatPDW, 180 },
            { ItemId.CompactRifle, 207 },
            { ItemId.AssaultRifle, 215 },
            { ItemId.PistolMk2, 41 },
            { ItemId.SNSPistolMk2, 38 },
            { ItemId.PumpShotgunMk2, 148 },
            { ItemId.SweeperShotgun, 76 },
            { ItemId.Gusenberg, 495 },
            { ItemId.AssaultRifleMk2, 247 },
            { ItemId.BullpupRifle, 400 },
            { ItemId.CombatPistol, 45 },
            { ItemId.CarbineRifle, 220 },
            { ItemId.CombatRifle, 220 },
            { ItemId.StunGun, 99 },
            { ItemId.APPistol, 305 },
            { ItemId.BullpupShotgun, 144 },
            { ItemId.AssaultShotgun, 495 },
            { ItemId.SMG, 160 },
            { ItemId.AssaultSMG, 190 },
            { ItemId.AdvancedRifle, 300 },
            { ItemId.CarbineRifleMk2, 280 },
            { ItemId.MilitaryRifle, 280 },
            { ItemId.TacticalRifle, 280 },
            { ItemId.HeavyRifle, 280 },
            { ItemId.PrecisionRifle, 280 },
            { ItemId.CombatShotgun, 280 },
            { ItemId.Nightstick, 10 },
            { ItemId.HeavyShotgun, 550 },
            { ItemId.CombatMGMk2, 1000 },
            { ItemId.BullpupRifleMk2, 250 },
            { ItemId.HeavySniper, 2500 },
            { ItemId.MarksmanRifleMk2, 3000 },
            { ItemId.CombatMG, 1000 },
            { ItemId.SpecialCarbine, 330 },
            { ItemId.SpecialCarbineMk2, 350 },
            { ItemId.SniperRifle, 9000 },
            { ItemId.Revolver, 223 },
            { ItemId.MarksmanRifle, 10000 },
            { ItemId.DoubleAction, 5000 },
            { ItemId.Musket, 5000 },
            { ItemId.NavyRevolver, 7500 },
            { ItemId.HeavySniperMk2, 12500 },
            { ItemId.RevolverMk2, 4500 },
            { ItemId.MG, 5000 },
            { ItemId.BodyArmor, 50 },
            //{ ItemId.PistolAmmo, 1 },
            //{ ItemId.SMGAmmo, 1 },
            //{ ItemId.RiflesAmmo, 1 },
            //{ ItemId.SniperAmmo, 1 },
            //{ ItemId.ShotgunsAmmo, 1 }
        };
        
        public static readonly Dictionary<int, int> WeaponsMaxHP = new Dictionary<int, int>
        {
            /* Pistols */
            { 100, 300 },
            { 101, 300 },
            { 102, 300 },
            { 103, 300 },
            { 104, 300 },
            { 105, 300 },
            { 106, 300 },
            { 107, 300 },
            { 108, 300 },
            { 109, 300 },
            { 110, 300 },
            { 111, 300 },
            { 112, 400 },
            { 113, 400 },
            { 114, 400 },
            /* SMG */
            { 115, 1000 },
            { 116, 1000 },
            { 117, 1000 },
            { 118, 1000 },
            { 119, 1000 },
            { 120, 1000 },
            { 121, 1000 },
            { 122, 1000 },
            { 123, 1000 },
            { 124, 1200 },
            { 125, 1500 },
            /* Rifles */
            { 126,  2000},
            { 127,  2000},
            { 128,  2000},
            { 129,  2000},
            { 130,  2000},
            { 131,  2000},
            { 132,  3000},
            { 133,  3000},
            { 134,  3000},
            { 135,  3000},
            { 266,  3000},
            /* Sniper */
            { 136,  150},
            { 137,  150},
            { 138,  150},
            { 139,  300},
            { 140,  300},
            /* Shotguns */
            { 141, 300},
            { 142, 300},
            { 143, 300},
            { 144, 300},
            { 145, 300},
            { 146, 300},
            { 147, 300},
            { 148, 300},
            { 149, 400},
            /* NEW WEAPONS */
            { 150, 9999},
            { 151, 9999},
            { 152, 9999},
            { 153, 9999},
            { 154, 9999},
            { 155, 9999},
            { 156, 9999},
            { 157, 9999},
            { 158, 9999},
            { 159, 9999},
            { 160, 9999},
            { 161, 9999},
            { 162, 9999} 
        };

        public static void TakeIllegalStuff(ExtPlayer player)
        {
            try
            {
                if (!FunctionsAccess.IsWorking("TakeIllegalStuff"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                string locationName = $"char_{characterData.UUID}";
                int rewardSumm = 0;

                if (Chars.Repository.ItemsData.ContainsKey(locationName))
                {
                    foreach (string Location in Chars.Repository.ItemsData[locationName].Keys)
                    {
                        //if (Location == "fastSlots") continue;
                        foreach (KeyValuePair<int, InventoryItemData> itemData in Chars.Repository.ItemsData[locationName][Location])
                        {
                            if (Location == "inventory" || Location == "fastSlots")
                            {
                                if (itemData.Value.ItemId == ItemId.Debug || itemData.Value.ItemId == ItemId.Wrench || itemData.Value.ItemId == ItemId.Flashlight || itemData.Value.ItemId == ItemId.Ball || itemData.Value.ItemId == ItemId.Bag) continue;

                                if (itemData.Value.Data.Contains("_") && WeaponsMaxHP.ContainsKey((int) itemData.Value.ItemId))
                                {
                                    int weaponHP = Convert.ToInt32(itemData.Value.Data.Split("_")[1]);
                                    int condition = Convert.ToInt32(((double) weaponHP / WeaponsMaxHP[(int) itemData.Value.ItemId]) * 100);
                                    if (condition < 20) continue; 
                                }

                                InventoryItemData item = itemData.Value;
                                if (IllegalsItems.ContainsKey(item.ItemId))
                                {
                                    if (item.ItemId == ItemId.PistolAmmo || item.ItemId == ItemId.SMGAmmo || item.ItemId == ItemId.RiflesAmmo || item.ItemId == ItemId.SniperAmmo || item.ItemId == ItemId.ShotgunsAmmo) rewardSumm += IllegalsItems[item.ItemId] * item.Count;
                                    else if (item.ItemId == ItemId.Drugs)
                                    {
                                        int dCount = Chars.Repository.getCountItem($"char_{characterData.UUID}", ItemId.Drugs, bagsToggled: false);
                                        rewardSumm += IllegalsItems[item.ItemId] * dCount;
                                    }
                                    else rewardSumm += IllegalsItems[item.ItemId];
                                }
                            }
                        }
                    }
                }

                if (rewardSumm > 0)
                {
                    Chars.Repository.RemoveAllIllegalStuff(player, IsRemoveBag: false);
                    MoneySystem.Wallet.Change(player, rewardSumm);
                    GameLog.Money($"server", $"player({characterData.UUID})", rewardSumm, $"TakeIllegalStuff({rewardSumm})");
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucSdalNelegal, rewardSumm), 3000);
                    Manager.sendFractionMessage((int) Models.Fractions.POLICE, "!{#FF8C00}[F] " + LangFunc.GetText(LangType.Ru, DataName.SdalNelegal, player.Name, rewardSumm), true);
                    Manager.sendFractionMessage((int) Models.Fractions.SHERIFF, "!{#FF8C00}[F] " + LangFunc.GetText(LangType.Ru, DataName.SdalNelegal, player.Name, rewardSumm), true);
                    Manager.sendFractionMessage((int) Models.Fractions.FIB, "!{#FF8C00}[F] " + LangFunc.GetText(LangType.Ru, DataName.SdalNelegal, player.Name, rewardSumm), true);
                }
                else
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoNelegal), 3000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"TakeIllegalStuff Exception: {e.ToString()}");
            }
        }

        public static void Event_PlayerDeath(ExtPlayer player, ExtPlayer killer, uint reason)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (sessionData.WorkData.OnDuty && sessionData.InCpMode)
                {
                    Manager.SetSkin(player);
                    sessionData.InCpMode = false;
                }
            }
            catch (Exception e)
            {
                Log.Write($"Event_PlayerDeath Exception: {e.ToString()}");
            }
        }
        
        [Interaction(ColShapeEnums.FractionPolic)]
        public static void OnFractionPolic(ExtPlayer player, int interact)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
              

                var fractionData = player.GetFractionData();
                if (fractionData == null || fractionData.Id != (int) Models.Fractions.POLICE)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoPolice), 3000);
                    return;
                }
                
                switch (interact)
                {
                    case 2:
                        FractionClothingSets.OpenFractionClothingSetsMenu(player);
                        return;
                    case 3:
                        if (!sessionData.WorkData.OnDuty)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000);
                            return;
                        }
                        if (!is_warg)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoEmergency), 3000);
                            return;
                        }
                        OpenSpecialPoliceMenu(player);
                        return;
                    /*case 5:
                        if (characterData.Licenses[6])
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас уже есть лицензия на оружие", 3000);
                            return;
                        }
                        if (!MoneySystem.Wallet.Change(player, -30000))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас недостаточно средств.", 3000);
                            return;
                        }
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили лицензию на оружие", 3000);
                        characterData.Licenses[6] = true;
                        //Chars.Repository.PlayerStats(player);
                        return;*/
                    case 5:
                        if (!sessionData.WorkData.OnDuty)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000);
                            return;
                        }
                        if (!fractionData.IsOpenGunStock)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Warehouse2Closed), 3000);
                            return;
                        }
                        if (!player.IsFractionAccess(RankToAccess.OpenWeaponStock)) return;
                        sessionData.OnFracStock = 7;
                        Chars.Repository.LoadOtherItemsData(player, "Fraction", "7", 5, Chars.Repository.InventoryMaxSlots["Fraction"]);
                        return;
                    case 6:
                        if (!sessionData.WorkData.OnDuty)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000);
                            return;
                        }
                        if (!player.IsInVehicle || (player.Vehicle.Model != NAPI.Util.GetHashKey("police") &&
                            player.Vehicle.Model != NAPI.Util.GetHashKey("police2") && player.Vehicle.Model != NAPI.Util.GetHashKey("police3") && player.Vehicle.Model != NAPI.Util.GetHashKey("police4")))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustCarPolice), 3000);
                            return;
                        }
                        player.Vehicle.SetSharedData("BOOST", 20);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarBoost), 3000);
                        return;
                    case 9:
                        if (!sessionData.WorkData.OnDuty)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000);
                            return;
                        }
                        if (!player.IsInVehicle)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustInCar), 3000);
                            return;
                        }
                        if (player.Vehicle.Class == 15 || player.Vehicle.Class == 16)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RepairFlatVeh), 3000);
                            return;
                        }
                        if (DateTime.Now < Main.NextFixcarPoliceVeh)
                        {
                            long ticks = Main.NextFixcarPoliceVeh.Ticks - DateTime.Now.Ticks;
                            if (ticks <= 0) return;
                            DateTime g = new DateTime(ticks);
                            if (g.Hour >= 1) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NextRepair1h, g.Hour, g.Minute, g.Second), 3000);
                            else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NextRepair,  g.Minute, g.Second), 3000);
                            return;
                        }
                        var vehicle = (ExtVehicle)player.Vehicle;
                        var vehicleLocalData = vehicle.GetVehicleLocalData();
                        if (vehicleLocalData != null)
                        {
                            if (vehicleLocalData.Access == VehicleAccess.Fraction && vehicleLocalData.Fraction == (int) Models.Fractions.POLICE)
                            {
                                Main.NextFixcarPoliceVeh = DateTime.Now.AddMinutes(3);
                                VehicleManager.RepairCar(vehicle);
                                Commands.RPChat("sme", player, LangFunc.GetText(LangType.Ru, DataName.RepairedVeh));
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucRepairVeh), 3000);
                            }
                            else
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustCarPolice), 3000);
                                return;
                            }
                        }
                        return;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"OnFractionPolic Exception: {e.ToString()}");
            }
        }

        #region shapes
        [Interaction(ColShapeEnums.FractionPolicArrest, In: true)]
        public static void InFractionPolicArrest(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                sessionData.InArrestArea = index;
            }
            catch (Exception e)
            {
                Log.Write($"InFractionPolicArrest Exception: {e.ToString()}");
            }
        }
        [Interaction(ColShapeEnums.FractionPolicArrest, Out: true)]
        public static void OutFractionPolicArrest(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                sessionData.InArrestArea = -1;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.ArrestTime != 0) player.Position = PrisonPosition;
            }
            catch (Exception e)
            {
                Log.Write($"OutFractionPolicArrest Exception: {e.ToString()}");
            }
        }
        #endregion

        public static void onPlayerDisconnectedhandler(ExtPlayer player, DisconnectionType type, string reason)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (sessionData.TimersData.ArrestTimer != null)
                {
                    Timers.Stop(sessionData.TimersData.ArrestTimer);
                    sessionData.TimersData.ArrestTimer = null;
                }
                if (sessionData.Following != null)
                {
                    var target = sessionData.Following;
                    var targetSessionData = target.GetSessionData();
                    if (targetSessionData != null) targetSessionData.Follower = null;
                }
                if (sessionData.Follower != null)
                {
                    var target = sessionData.Follower;
                    var targetSessionData = target.GetSessionData();
                    if (targetSessionData != null)
                    {
                        targetSessionData.Following = null;
                        Trigger.ClientEvent(target, "follow", false);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"onPlayerDisconnectedhandler Exception: {e.ToString()}");
            }
        }

        public static void setPlayerWantedLevel(ExtPlayer player, WantedLevel wantedlevel)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                characterData.WantedLVL = wantedlevel;
                if (wantedlevel != null) 
                    Trigger.ClientEvent(player, "client.charStore.Wanted", wantedlevel.Level);
                else 
                    Trigger.ClientEvent(player, "client.charStore.Wanted", 0);
            }
            catch (Exception e)
            {
                Log.Write($"setPlayerWantedLevel Exception: {e.ToString()}");
            }
        }

        public static bool is_warg = false;

        #region menus
        public static void OpenPoliceGunMenu(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Manager.OpenFractionSM(player, "polic");
            }
            catch (Exception e)
            {
                Log.Write($"OpenPoliceGunMenu Exception: {e.ToString()}");
            }
        }
        public static void OpenSpecialPoliceMenu(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (!is_warg)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Не включен режим ЧП", 3000);
                    return;
                }
                if (player.Position.DistanceTo(CloakroomSpecialPosition) >= 5)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TooFarFromVidacha), 3000);
                    return;
                }
                if (player.GetFractionId() != (int) Models.Fractions.POLICE) return;

                if (!sessionData.InCpMode)
                {
                    bool gender = characterData.Gender;
                    //Customization.ApplyCharacter(player);
                    //ClothesComponents.ClearClothes(player, gender);
                    if (gender)
                    {
                        ClothesComponents.SetSpecialClothes(player, 0,  39, 0);
                        //ClothesComponents.SetSpecialClothes(player, 1, 52, 0);
                        ClothesComponents.SetSpecialClothes(player, 11, 53, 0);
                        ClothesComponents.SetSpecialClothes(player, 4, 31, 0);
                        ClothesComponents.SetSpecialClothes(player, 6, 25, 0);
                        ClothesComponents.SetSpecialClothes(player, 3, 49, 0);
                    }
                    else
                    {
                        ClothesComponents.SetSpecialClothes(player, 0,  38, 0);
                        //ClothesComponents.SetSpecialClothes(player, 1, 57, 0);
                        ClothesComponents.SetSpecialClothes(player, 11, 46, 0);
                        ClothesComponents.SetSpecialClothes(player, 4, 30, 0);
                        ClothesComponents.SetSpecialClothes(player, 6, 25, 0);
                        ClothesComponents.SetSpecialClothes(player, 3, 49, 0);
                    }
                    Chars.Repository.LoadAccessories(player);
                    sessionData.InCpMode = true;
                    return;
                }
                Manager.SetSkin(player);
                sessionData.InCpMode = false;
            }
            catch (Exception e)
            {
                Log.Write($"OpenSpecialPoliceMenu Exception: {e.ToString()}");
            }
        }
        #endregion
    }
}
