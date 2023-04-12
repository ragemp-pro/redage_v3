using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Chars;
using NeptuneEvo.GUI;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using Redage.SDK;
using System;
using System.Collections.Generic;
using Localization;
using NeptuneEvo.Core;
using NeptuneEvo.Players.Popup.List.Models;

namespace NeptuneEvo.Events
{
    class StreetRaceData
    {
        public int rate { set; get; }
        public ExtPlayer created { set; get; } = null;
        public ExtPlayer joined { set; get; } = null;
        public Vector3 Position { set; get; } = new Vector3();
        public bool Status { set; get; } = false;

        public StreetRaceData(ExtPlayer Created, ExtPlayer Joined)
        {
            created = Created;
            joined = Joined;
        }
    }
    class StreetRace : Script
    {
        private static readonly nLog Log = new nLog("Events.Race");

        public static Dictionary<int, StreetRaceData> StreetRaceList = new Dictionary<int, StreetRaceData>();
        public static Dictionary<int, int> PlayerToStreetRaceId = new Dictionary<int, int>();



        [RemoteEvent("server.streetrace.open")]
        public static void OpenStreetRaceMenu(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData())
                    return;

                ClearData(player, -1);

                if (PlayerToStreetRaceId.ContainsKey(player.Value))
                    return;

                var frameList = new FrameListData(); 
 
                frameList.Header = $"Уличные гонки"; 
                frameList.Callback = callback_selectChar; 
                

                var characterData = player.GetCharacterData();

                var playersInRange = Main.GetPlayersInRadiusOfPosition(player.Position, 10f, UpdateData.GetPlayerDimension(player));

                foreach (var foreachPlayer in playersInRange)
                {
                    if (!PlayerToStreetRaceId.ContainsKey(foreachPlayer.Value) && foreachPlayer.Value != player.Value && foreachPlayer.IsInVehicle && foreachPlayer.VehicleSeat == (int)VehicleSeat.Driver)
                    {
                        if (characterData.Friends.ContainsKey(foreachPlayer.Name))
                        {
                            frameList.List.Add(new ListData($"{foreachPlayer.Name} - {foreachPlayer.Value} ID", foreachPlayer.Value));
                        }
                        else
                        {
                            frameList.List.Add(new ListData($"{foreachPlayer.Value} ID", foreachPlayer.Value));
                        }
                    }
                }

                Players.Popup.List.Repository.Open(player, frameList); 
            }
            catch (Exception e)
            {
                Log.Write($"OpenStreetRaceMenu Exception: {e.ToString()}");
            }
        }

        private static void callback_selectChar(ExtPlayer player, object listItem)
        {
            try
            {
                if (!(listItem is int))
                    return;
                        
                if (!player.IsCharacterData()) 
                    return;
                
                if (PlayerToStreetRaceId.ContainsKey(player.Value)) return;
                ExtPlayer target = Main.GetPlayerByID(Convert.ToInt32(listItem));
                if (!target.IsCharacterData()) 
                    return;
                if (PlayerToStreetRaceId.ContainsKey(target.Value)) 
                    return;
                StreetRaceList.Add(player.Value, new StreetRaceData(player, target));
                PlayerToStreetRaceId.Add(player.Value, player.Value);
                PlayerToStreetRaceId.Add(target.Value, player.Value);
                Trigger.ClientEvent(player, "openInput", LangFunc.GetText(LangType.Ru, DataName.StreetRace), LangFunc.GetText(LangType.Ru, DataName.EnterStavka0to100k), 6, "streetrace");
            }
            catch (Exception e)
            {
                Log.Write($"callback_selectChar Exception: {e.ToString()}");
            }
        } 

        public static void SelectRate(ExtPlayer player, int rate)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                else if (!PlayerToStreetRaceId.ContainsKey(player.Value)) return;
                else if (!StreetRaceList.ContainsKey(player.Value)) return;
                StreetRaceData streetRaceData = StreetRaceList[PlayerToStreetRaceId[player.Value]];
                var createdCharacterData = streetRaceData.created.GetCharacterData();
                if (createdCharacterData == null || createdCharacterData.Money < rate)
                {
                    ClearData(streetRaceData.created, 0);
                    return;
                }
                var joinedCharacterData = streetRaceData.joined.GetCharacterData();
                if (joinedCharacterData == null || joinedCharacterData.Money < rate)
                {
                    ClearData(streetRaceData.joined, 0);
                    return;
                }
                StreetRaceList[player.Value].rate = rate;
                Trigger.ClientEvent(player, "client.streetrace.position");
            }
            catch (Exception e)
            {
                Log.Write($"SelectRate Exception: {e.ToString()}");
            }
        }

        [ServerEvent(Event.PlayerDeath)]
        public void OnPlayerDeath(ExtPlayer player, ExtPlayer entityKiller, uint weapon)
        {
            try
            {
                ClearData(player, 1);
            }
            catch (Exception e)
            {
                Log.Write($"OnPlayerDeath Exception: {e.ToString()}");
            }
        }
        [ServerEvent(Event.PlayerExitVehicle)]
        public void Event_OnPlayerExitVehicle(ExtPlayer player, ExtVehicle vehicle)
        {
            try
            {
                ClearData(player, 3);
            }
            catch (Exception e)
            {
                Log.Write($"Event_OnPlayerExitVehicle Exception: {e.ToString()}");
            }
        }
        public static void ClearData(ExtPlayer player, int messageId)
        {
            try
            {
                if (!PlayerToStreetRaceId.ContainsKey(player.Value)) return;
                int streetRaceId = PlayerToStreetRaceId[player.Value];
                PlayerToStreetRaceId.Remove(player.Value);
                if (!StreetRaceList.ContainsKey(streetRaceId)) return;
                StreetRaceData streetRaceData = StreetRaceList[streetRaceId];
                ExtPlayer WinPlayer = streetRaceData.created.Value == player.Value ? streetRaceData.joined : streetRaceData.created;
                if (streetRaceData.Status)
                {
                    Wallet.Change(WinPlayer, Convert.ToInt32(streetRaceData.rate * 2 * 0.9));
                    Trigger.ClientEvent(WinPlayer, "client.streetrace.clear");
                }
                if (PlayerToStreetRaceId.ContainsKey(WinPlayer.Value)) PlayerToStreetRaceId.Remove(WinPlayer.Value);
                StreetRaceList.Remove(streetRaceId);
                if (player.IsCharacterData()) Trigger.ClientEvent(player, "client.streetrace.clear");

                switch (messageId)
                {
                    case 0:
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMoney), 3000);
                        Notify.Send(WinPlayer, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerNotEnoughMoney), 3000);
                        break;
                    case 1:
                        if (streetRaceData.Status)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouDiedLose), 3000);
                            Notify.Send(WinPlayer, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VragDiedWin), 3000);
                        }
                        else
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VragDiedCancel), 3000);
                            Notify.Send(WinPlayer, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VragDkSWin), 3000);
                        }
                        break;
                    case 2:
                        Notify.Send(WinPlayer, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VragDkWin), 3000);
                        break;
                    case 3:
                        if (streetRaceData.Status)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouLeftTsLose), 3000);
                            Notify.Send(WinPlayer, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VragLeftTsWin), 3000);
                        }
                        else
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouleftTsCancel), 3000);
                            Notify.Send(WinPlayer, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VragLeftTsCancel), 3000);
                        }
                        break;
                    case 4:
                        if (streetRaceData.Status)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMustTsLose), 3000);
                            Notify.Send(WinPlayer, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMustTsLose), 3000);
                        }
                        else
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VragLeftTsWinS), 3000);
                            Notify.Send(WinPlayer, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouLeftTsCancel), 3000);
                        }
                        break;
                    case 5:
                        if (streetRaceData.Status)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerFarWin), 3000);
                            Notify.Send(WinPlayer, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouFarCancel), 3000);
                        }
                        else
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerFarWin), 3000);
                            Notify.Send(WinPlayer, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouFarCancel), 3000);
                        }
                        break;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"EndWork Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.streetrace.position")]
        public static void StreetRacePosition(ExtPlayer player, float sr_X, float sr_Y, float sr_Z)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                else if (!PlayerToStreetRaceId.ContainsKey(player.Value)) return;
                else if (!StreetRaceList.ContainsKey(player.Value)) return;
                StreetRaceData streetRaceData = StreetRaceList[player.Value];
                streetRaceData.Position = new Vector3(sr_X, sr_Y, sr_Z);
                if (!sessionData.CuffedData.Cuffed && !sessionData.DeathData.InDeath)
                {
                    var targetSessionData = streetRaceData.joined.GetSessionData();
                    if (targetSessionData != null && !targetSessionData.CuffedData.Cuffed && !targetSessionData.DeathData.InDeath)
                    {
                        if (targetSessionData.RequestData.IsRequested)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PersonHavBeenBusy), 7000);
                            return;
                        }
                        targetSessionData.RequestData.IsRequested = true;
                        targetSessionData.RequestData.Request = "StreetRace";
                        targetSessionData.RequestData.From = player;
                        targetSessionData.RequestData.Time = DateTime.Now.AddSeconds(10);
                        //Notify.Send(streetRaceData.joined, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerOFferGonka, player.Value, streetRaceData.rate), 15000);
                        EventSys.SendCoolMsg(streetRaceData.joined,"Гонка", "Вызов на гонку", $"{LangFunc.GetText(LangType.Ru, DataName.PlayerOFferGonka, player.Value, streetRaceData.rate)}", "", 10000);
                        EventSys.SendCoolMsg(player,"Гонка", "Вызов на гонку", $"{LangFunc.GetText(LangType.Ru, DataName.YouOfferGonka, streetRaceData.joined.Value)}", "", 10000);
                        //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouOfferGonka, streetRaceData.joined.Value), 8000);
                    }
                    else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantGonkaNow), 3000);
                }
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantGonkaNow), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"StreetRacePosition Exception: {e.ToString()}");
            }
        }
        public static void Accept(ExtPlayer player)
        {
            try
            {
                if (!PlayerToStreetRaceId.ContainsKey(player.Value)) return;
                int streetRaceId = PlayerToStreetRaceId[player.Value];

                if (!StreetRaceList.ContainsKey(streetRaceId)) return;
                StreetRaceData streetRaceData = StreetRaceList[streetRaceId];

                if (!streetRaceData.created.IsInVehicle || streetRaceData.created.VehicleSeat != (int)VehicleSeat.Driver)
                {
                    ClearData(streetRaceData.created, 4);
                    return;
                }
                else if (!streetRaceData.joined.IsInVehicle || streetRaceData.joined.VehicleSeat != (int)VehicleSeat.Driver)
                {
                    ClearData(streetRaceData.joined, 4);
                    return;
                }
                else if (streetRaceData.created.Position.DistanceTo(streetRaceData.joined.Position) > 10)
                {
                    ClearData(streetRaceData.created, 5);
                    return;
                }
                else if (UpdateData.CanIChange(streetRaceData.created, streetRaceData.rate, true) != 255)
                {
                    ClearData(streetRaceData.created, 0);
                    return;
                }
                else if (UpdateData.CanIChange(streetRaceData.joined, streetRaceData.rate, true) != 255)
                {
                    ClearData(streetRaceData.joined, 0);
                    return;
                }
                var createdSessionData = streetRaceData.created.GetSessionData();
                if (createdSessionData == null) return;
                var joinedSessionData = streetRaceData.joined.GetSessionData();
                if (joinedSessionData == null) return;

                StreetRaceList[streetRaceId].Status = true;
                if (streetRaceData.rate > 0)
                {
                    Wallet.Change(streetRaceData.created, -streetRaceData.rate);
                    Wallet.Change(streetRaceData.joined, -streetRaceData.rate);
                }
                createdSessionData.StreetRaceTime = DateTime.Now.AddSeconds(10);
                joinedSessionData.StreetRaceTime = DateTime.Now.AddSeconds(10);
                Trigger.ClientEvent(streetRaceData.created, "client.streetrace.start", streetRaceData.Position.X, streetRaceData.Position.Y, streetRaceData.Position.Z, true);
                Trigger.ClientEvent(streetRaceData.joined, "client.streetrace.start", streetRaceData.Position.X, streetRaceData.Position.Y, streetRaceData.Position.Z, false);
            }
            catch (Exception e)
            {
                Log.Write($"Accept Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.streetrace.finished")]
        public static void StreetRaceFinished(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                else if (!PlayerToStreetRaceId.ContainsKey(player.Value)) return;
                int streetRaceId = PlayerToStreetRaceId[player.Value];
                if (!StreetRaceList.ContainsKey(streetRaceId)) return;
                StreetRaceData streetRaceData = StreetRaceList[streetRaceId];
                int cost = Convert.ToInt32(streetRaceData.rate * 2 * 0.9);
                Wallet.Change(player, cost);
                //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouWinGonka, cost), 8000);
                EventSys.SendCoolMsg(player,"Гонка", "Победа!", $"{LangFunc.GetText(LangType.Ru, DataName.YouWinGonka, cost)}", "", 10000);
                StreetRaceList[streetRaceId].Status = false;
                ExtPlayer LosePlayer = streetRaceData.created.Value == player.Value ? streetRaceData.joined : streetRaceData.created;
                //Notify.Send(LosePlayer, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouLoseGonka), 8000);
                EventSys.SendCoolMsg(LosePlayer,"Гонка", "Поражение :(", $"{LangFunc.GetText(LangType.Ru, DataName.YouLoseGonka, cost)}", "", 10000);
                ClearData(LosePlayer, -1);
                BattlePass.Repository.UpdateReward(player, 35);
                BattlePass.Repository.UpdateReward(LosePlayer, 35);
            }
            catch (Exception e)
            {
                Log.Write($"StreetRaceFinished Exception: {e.ToString()}");
            }
        }

    }
}
