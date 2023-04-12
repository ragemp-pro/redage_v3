using GTANetworkAPI;
using NeptuneEvo.Handles;
using System;
using System.Collections.Generic;
using System.Linq;
using Localization;
using Redage.SDK;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.Core;
using NeptuneEvo.Chars;
using NeptuneEvo.Functions;
using NeptuneEvo.Players;
using NeptuneEvo.Character;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;
using Newtonsoft.Json;

namespace NeptuneEvo.Events
{
    class TankRoyale : Script
    {
        class LobbyData
        {
            public int LobbyIndex { get; set; } = 0;
            public string LobbyName { get; set; } = "None";
            public int LobbyPrice { get; set; } = 0;
            public string LobbyPassword { get; set; } = null;
            public int LobbyMode { get; set; } = 0;
            [JsonIgnore]
            public int LobbyMap { get; set; } = 0;
            
            [JsonIgnore]
            public Dictionary<ExtPlayer, TRClass> TR_Players = new Dictionary<ExtPlayer, TRClass>();
            [JsonIgnore]
            public List<ExtVehicle> Vehicles = new List<ExtVehicle>();

            public int TR_Bank = 0;
            [JsonIgnore]
            public byte TR_State = 0; // Closed, Register, OnGoing
            [JsonIgnore]
            public byte GameStartSeconds = 120; // Closed, Register, OnGoing
            [JsonIgnore]
            public bool TR_RandomGun = false; // False = default, True = SuperGun
            [JsonIgnore]
            public ExtMarker TR_Marker = null;
            [JsonIgnore]
            public ExtMarker TR_LMarker = null;
            [JsonIgnore]
            public string TR_ZoneTimer = null;
            [JsonIgnore]
            public string TR_Timer = null;
            [JsonIgnore]
            public int TR_TimerTicks = 0;
            [JsonIgnore]
            public int TR_Stage = 0;

            public LobbyData(int index, string name, int price, string password, int lobby_map)
            {
                this.LobbyIndex = index;
                this.LobbyName = name;
                this.LobbyPrice = price;
                this.LobbyPassword = password;
                this.LobbyMap = lobby_map;
            }
        }
        
        class TRClass
        {
            public ExtVehicle Vehicle { get; set; } = null;
            public int Health { get; set; } = 1;
        }
        
        private static readonly nLog Log = new nLog("Events.TankRoyale");
        
        private static Dictionary<int, LobbyData> LobbyList = new Dictionary<int, LobbyData>();
        public static List<ExtPlayer> PlayersInLobbyMenu = new List<ExtPlayer>();
        
        private static float[] TR_Stages = new float[5] { 400f, 330f, 250f, 195f, 100f };
        private static Vector3 TR_Center = new Vector3(978.3693, -3090.363, 5.9007626);

        private static Dictionary<int, Vector3> TR_Vehicles = new Dictionary<int, Vector3>
        {
            {0, new Vector3(790.40643, -3282.6877, 5.9010344)},
            {1, new Vector3(962.72595, -2913.9973, 5.902138)},
            {2, new Vector3(1264.0676, -3092.2275, 5.903135)},
            {3, new Vector3(1071.1937, -3330.9614, 5.9089413)},
            {4, new Vector3(689.9275, -2829.0178, 5.899025)},
            {5, new Vector3(1262.5812, -3316.303, 5.877872)},
            {6, new Vector3(724.92126, -3178.554, 5.900534)},
            {7, new Vector3(1211.6266, -2970.308, 5.866054)},
            {8, new Vector3(803.6499, -2909.5005, 5.900784)},
            {9, new Vector3(741.02814, -2836.4604, 6.199977)},
            {10, new Vector3(1191.9462, -3201.9702, 6.02804)},
            {11, new Vector3(899.56506, -3329.428, 9.242613)},
            {12, new Vector3(1202.1108, -3046.861, 5.9021406)},
            {13, new Vector3(939.5727, -3007.5347, 5.900763)},
            {14, new Vector3(1168.5721, -3303.3713, 5.905657)},
            {15, new Vector3(1118.6666, -3193.2656, 5.877467)},
            {16, new Vector3(729.9011, -3057.1516, 9.611129)},
            {17, new Vector3(950.8217, -3241.933, 5.8951187)},
            {18, new Vector3(729.9011, -3057.1516, 9.611129)},
            {19, new Vector3(1102.1403, -2950.019, 5.901182)},
            {20, new Vector3(827.0027, -2971.0264, 5.903509)},
            {21, new Vector3(1288.3187, -3220.8394, 5.9037223)},
            {22, new Vector3(763.369, -2981.4387, 5.8007164)},
            {23, new Vector3(840.1113, -3265.7217, 6.0747604)},
            {24, new Vector3(1179.0874, -3141.7935, 5.5373416)},
            {25, new Vector3(1049.4495, -2909.4692, 5.9006004)},
            {26, new Vector3(1202.7927, -3241.3804, 5.9506364)},
            {27, new Vector3(660.46625, -2987.6877, 6.045199)},
            {28, new Vector3(856.28345, -2909.254, 5.9005823)},
            {29, new Vector3(886.813, -3259.28, 5.901121)}
        };
        
        public static void TanksLobbiesStartInterval()
        {
            try
            {
                if (LobbyList.Count < 1) return;
                
                foreach (var lobby in LobbyList)
                {
                    if (lobby.Value.TR_State == 1 && lobby.Value.GameStartSeconds > 0)
                        lobby.Value.GameStartSeconds -= 1;
                }
            }
            catch (Exception e)
            {
                Log.Write($"TanksLobbiesStartInterval Exception: {e.ToString()}");
            }
        }
        
        public static void CheckLobby(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                if (!FunctionsAccess.IsWorking("CheckLobby"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                /*else if (characterData.LVL < 3)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.lvl3forgame), 3000);
                    return;
                }*/

                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                if (sessionData.WorkData.OnDuty || sessionData.WorkData.OnWork)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustEndWorkDay), 3000);
                    return;
                }
                else if (sessionData.CuffedData.Cuffed)
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

                Trigger.ClientEvent(player, "airsoft_lobbyMenuHandler", 1, JsonConvert.SerializeObject(LobbyList.Values), "tanks");
            }
            catch (Exception e)
            {
                Log.Write($"CheckLobby Exception: {e.ToString()}");
            }
        }
        
        [RemoteEvent("tanks_createLobby_server")]
        public static void CreateLobby(ExtPlayer player, string lobby_name, int lobby_price, string lobby_password, int gameMap)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                if (!FunctionsAccess.IsWorking("CreateLobby"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                /*else if (characterData.LVL < 3)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.lvl3forgame), 3000);
                    return;
                }*/

                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                if (sessionData.WorkData.OnDuty || sessionData.WorkData.OnWork)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustEndWorkDay), 3000);
                    return;
                }
                else if (sessionData.CuffedData.Cuffed)
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
                
                if (LobbyList.ContainsKey(player.Value))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LobbyAlreadyCreated), 3000);
                    return;
                }

                if (characterData.Money < lobby_price)
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GameStartPrice, lobby_price), 3000);
                    return;
                }

                if (sessionData.InTanksLobby >= 0)
                {
                    OutTanksLobbyZone(player, false);
                }
                
                LobbyList.Add(player.Value, new LobbyData(player.Value, lobby_name.Length < 1 ? player.Name.Split("_")[0] : lobby_name, lobby_price, lobby_password, gameMap));
                LobbyList[player.Value].TR_Players.Add(player, new TRClass());
                sessionData.InTanksLobby = player.Value;
                UpdateLobbyList();
                
                LobbyList[player.Value].TR_State = 1;
                LobbyList[player.Value].TR_Stage = 0;
                LobbyList[player.Value].TR_TimerTicks = 0;
                LobbyList[player.Value].TR_Timer = Timers.Start("TR_Tick", 30000, () => TankRoyale_Tick(player.Value), true);
                
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NowWaitOthers), 5000);
                Trigger.ClientEvent(player, "airsoft_lobbyMenuHandler", 3, JsonConvert.SerializeObject(new int[3] { LobbyList[player.Value].TR_Players.Count, 2, 30 }), "tanks", LobbyList[player.Value].GameStartSeconds);
            }
            catch (Exception e)
            {
                Log.Write($"CreateLobby Exception: {e.ToString()}");
            }
        }

        public static void DeathCheck(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                int lobbyId = sessionData.InTanksLobby;
                if (lobbyId >= 0 && LobbyList.ContainsKey(lobbyId) && LobbyList[lobbyId].TR_Players.ContainsKey(player) && LobbyList[lobbyId].TR_State == 2)
                {
                    player.Health = 100;
                    player.Armor = 0;
                    PlayerExitFromTR(player, killlist: true);
                    if (LobbyList[lobbyId].TR_Players.Count == 1) Winner(lobbyId);
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouLeftFromMp), 3000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"DeathCheck Exception: {e.ToString()}");
            }
        }

        public static void PlayerExitFromTR(ExtPlayer player, bool dc = false, bool restoremoney = false, bool checkwinner = false, bool killlist = false)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                int lobbyId = sessionData.InTanksLobby;
                if (lobbyId >= 0 && LobbyList.ContainsKey(lobbyId) && LobbyList[lobbyId].TR_Players.ContainsKey(player))
                {
                    if (LobbyList[lobbyId].TR_State == 2)
                    {
                        if (restoremoney)
                        {
                            Wallet.Change(player, LobbyList[lobbyId].LobbyPrice);
                            GameLog.Money($"server", $"player({characterData.UUID})", LobbyList[lobbyId].LobbyPrice, $"TankRoyaleRestore");
                        }
                        if (LobbyList[lobbyId].TR_Players[player].Vehicle != null)
                        {
                            VehicleStreaming.DeleteVehicle(LobbyList[lobbyId].TR_Players[player].Vehicle);
                            
                            if (LobbyList[lobbyId].Vehicles.Contains(LobbyList[lobbyId].TR_Players[player].Vehicle))
                                LobbyList[lobbyId].Vehicles.Remove(LobbyList[lobbyId].TR_Players[player].Vehicle);
                            
                            LobbyList[lobbyId].TR_Players[player].Vehicle = null;
                        }
                        
                        if(!dc)
                        {
                            VehicleManager.WarpPlayerOutOfVehicle(player);
                            Trigger.Dimension(player, 0);
                            BattlePass.Repository.UpdateReward(player, 132);
                            BattlePass.Repository.UpdateReward(player, 131);
                            NAPI.Player.SpawnPlayer(player, new Vector3(-478.86032, -395.27307, 34.027653));
                            //player.Health = 100;
                            if (killlist)
                            {
                                lock (LobbyList[lobbyId].TR_Players.Keys)
                                {
                                    foreach (ExtPlayer foreachPlayer in LobbyList[lobbyId].TR_Players.Keys)
                                    {
                                        if (!foreachPlayer.IsCharacterData()) continue;
                                        NAPI.Notification.SendNotificationToPlayer(foreachPlayer, $"~r~{player.Name.Replace('_', ' ')} ~s~выбыл из игры", true);
                                    }
                                }
                            }
                        }
                    }
                    
                    LobbyList[lobbyId].TR_Players.Remove(player);
                    sessionData.InTanksLobby = -1;
                    
                    if (checkwinner && LobbyList[lobbyId].TR_Players.Count == 1) Winner(lobbyId);
                    UpdateTanksLobbyStats(lobbyId);
                }
            }
            catch (Exception e)
            {
                Log.Write($"PlayerExitFromTR Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("tanks_joinLobby_server")]
        public static void RegisterAccept(ExtPlayer player, int index, string password)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                if (!LobbyList.ContainsKey(index))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LobbyNotFound), 3000);
                    return;
                }
                
                if (sessionData.InTanksLobby >= 0 && sessionData.InTanksLobby == index)
                {
                    Trigger.ClientEvent(player, "airsoft_lobbyMenuHandler", 3, JsonConvert.SerializeObject(new int[3] { LobbyList[index].TR_Players.Count, 2, 30 }), "tanks", LobbyList[index].GameStartSeconds);
                    return;
                }
                
                if (LobbyList[index].LobbyPassword != null && LobbyList[index].LobbyPassword != password)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LobbyIncorrectPassowrd), 3000);
                    return;
                }
                
                if (LobbyList[index].TR_Players.Count >= 30)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TankRoyaleMaxP), 5000);
                    return;
                }
                
                if (characterData.Money < LobbyList[index].LobbyPrice)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RegisterRoyaleCost, Wallet.Format(LobbyList[index].LobbyPrice)), 3000);
                    return;
                }

                if (LobbyList[index].TR_State != 1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GameIsOn), 3000);
                    return;
                }
                
                if (sessionData.InTanksLobby >= 0)
                    OutTanksLobbyZone(player, false);
                
                LobbyList[index].TR_Players.Add(player, new TRClass());
                sessionData.InTanksLobby = index;
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TankSucRegister), 5000);

                UpdateTanksLobbyStats(index);

                if (LobbyList[index].TR_TimerTicks < 18)
                    LobbyList[index].TR_TimerTicks = 18;
            }
            catch (Exception e)
            {
                Log.Write($"RegisterAccept Exception: {e.ToString()}");
            }
        }
        
        [RemoteEvent("TanksUpdateServerPlayersInLobbyMenuList")]
        public static void TanksUpdateServerPlayersInLobbyMenuList(ExtPlayer player, int state)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (state == 1 && !PlayersInLobbyMenu.Contains(player))
                {
                    PlayersInLobbyMenu.Add(player);
                }
                else if (state == 2 && PlayersInLobbyMenu.Contains(player))
                {
                    PlayersInLobbyMenu.Remove(player);
                }
            }
            catch (Exception e)
            {
                Log.Write($"TanksUpdateServerPlayersInLobbyMenuList Exception: {e.ToString()}");
            }
        }
        
        public static void UpdateLobbyList()
        {
            try
            {
                lock(PlayersInLobbyMenu)
                {
                    foreach (ExtPlayer foreachPlayer in PlayersInLobbyMenu)
                    {
                        if (foreachPlayer.IsCharacterData())
                        {
                            Trigger.ClientEvent(foreachPlayer, "airsoft_updateLobbyList_client", JsonConvert.SerializeObject(LobbyList), "tanks");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"UpdateLobbyList Exception: {e.ToString()}");
            }
        }
        
        public static void UpdateTanksLobbyStats(int index)
        {
            try
            {
                if (LobbyList.ContainsKey(index) && LobbyList[index].TR_State == 1 && LobbyList[index].TR_Players.Count >= 1)
                {
                    foreach (ExtPlayer foreachPlayer in LobbyList[index].TR_Players.Keys)
                    {
                        if (foreachPlayer.IsCharacterData())
                            Trigger.ClientEvent(foreachPlayer, "airsoft_lobbyMenuHandler", 3, JsonConvert.SerializeObject(new int[3] { LobbyList[index].TR_Players.Count, 2, 30 }), "tanks", LobbyList[index].GameStartSeconds);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"UpdateTanksLobbyStats Exception: {e.ToString()}");
            }
        }
        
        public static void OutTanksLobbyZone(ExtPlayer player, bool needMsg = true)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                int lobbyId = sessionData.InTanksLobby;
                if (lobbyId >= 0 && LobbyList.ContainsKey(lobbyId) && LobbyList[lobbyId].TR_State == 1)
                {
                    if (LobbyList[lobbyId].TR_Players.ContainsKey(player))
                    {
                        sessionData.InTanksLobby = -1;
                        LobbyList[lobbyId].TR_Players.Remove(player);
                        
                        if (needMsg)
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LeftLobby), 3000);

                        if (LobbyList[lobbyId].TR_Players.Count < 1)
                        {
                            if (LobbyList[lobbyId].TR_Timer != null)
                            {
                                Timers.Stop(LobbyList[lobbyId].TR_Timer);
                                LobbyList[lobbyId].TR_Timer = null;
                            }
                            
                            if (LobbyList[lobbyId].TR_ZoneTimer != null)
                            {
                                Timers.Stop(LobbyList[lobbyId].TR_ZoneTimer);
                                LobbyList[lobbyId].TR_ZoneTimer = null;
                            }
                            
                            LobbyList.Remove(lobbyId);
                            UpdateLobbyList();
                        }
                        else UpdateTanksLobbyStats(lobbyId);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"OutTanksLobbyZone Exception: {e.ToString()}");
            }
        }

        [ServerEvent(Event.VehicleDamage)]
        public static void OnVehicleDamageHandler(ExtVehicle vehicle, float bodyHealthLoss, float engineHealthLoss)
        {
            try
            {
                foreach (var lobby in LobbyList)
                {
                    KeyValuePair<ExtPlayer, TRClass> PlayerInfo = LobbyList[lobby.Key].TR_Players.FirstOrDefault(p => p.Value.Vehicle == vehicle);
                    if (lobby.Key >= 0 && LobbyList.ContainsKey(lobby.Key) && LobbyList[lobby.Key].TR_State == 2 && LobbyList[lobby.Key].Vehicles.Contains(vehicle))
                    {
                        if(PlayerInfo.Value == default(TRClass) || PlayerInfo.Value.Vehicle == null) return;
                        PlayerInfo.Value.Health -= 1;
                        ExtPlayer target = PlayerInfo.Key;
                        if (target.IsCharacterData())
                        {
                            if (!LobbyList[lobby.Key].TR_RandomGun)
                            {
                                if (target.Health - 2 >= 5) target.Health -= 2;
                            }
                            else
                            {
                                if (target.Health - 1 >= 5) target.Health -= 1;
                            }

                            if (target.Health <= 7 || PlayerInfo.Value.Health <= 5)
                            {
                                PlayerExitFromTR(target, killlist: true);
                                
                                if (target != null)
                                    Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouLeftFromMp), 5000);
                                
                                lock (LobbyList[lobby.Key].TR_Players.Keys)
                                {
                                    int count = LobbyList[lobby.Key].TR_Players.Count;
                                    if (count == 0)
                                    {
                                        if (LobbyList[lobby.Key].TR_Players.Keys.Count >= 1)
                                        {
                                            List<ExtPlayer> Players = LobbyList[lobby.Key].TR_Players.Keys.ToList();
                                            foreach (ExtPlayer foreachPlayer in Players)
                                            {
                                                if (!foreachPlayer.IsCharacterData()) continue;
                                                Trigger.SendChatMessage(foreachPlayer,
                                                    "~o~[Tank Royale] Мероприятие было завершено без победителя.");
                                            }
                                        }

                                        EndGame(lobby.Key);
                                        return;
                                    }
                                    else if (count == 1) Winner(lobby.Key);
                                    else if (count >= 2)
                                    {
                                        foreach (ExtPlayer foreachPlayer in LobbyList[lobby.Key].TR_Players.Keys)
                                        {
                                            if (!foreachPlayer.IsCharacterData()) continue;
                                            Trigger.SendChatMessage(foreachPlayer, $"~o~[Tank Royale] В игре осталось {LobbyList[lobby.Key].TR_Players.Count} танков!");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"OnVehicleDamageHandler Exception: {e.ToString()}");
            }
        }

        private static void Winner(int index)
        {
            try
            {
                if (!LobbyList.ContainsKey(index)) return;
                ExtPlayer winner = LobbyList[index].TR_Players.ElementAt(0).Key;
                var winnerCharacterData = winner.GetCharacterData();
                if (winnerCharacterData == null)
                {
                    if (LobbyList[index].TR_Players.Keys.Count >= 1)
                    {
                        List<ExtPlayer> Players = LobbyList[index].TR_Players.Keys.ToList();
                        foreach (ExtPlayer foreachPlayer in Players)
                        {
                            if (!foreachPlayer.IsCharacterData()) continue;
                            Trigger.SendChatMessage(foreachPlayer, "~o~[Tank Royale] Мероприятие было завершено без победителя.");
                        }
                    }
                    EndGame(index);
                    return;
                }
                Wallet.Change(winner, LobbyList[index].TR_Bank);
                GameLog.Money($"system", $"player({winnerCharacterData.UUID})", LobbyList[index].TR_Bank, $"TankRoyaleWinner");
                if (LobbyList[index].TR_Players.Keys.Count >= 1)
                {
                    List<ExtPlayer> Players = LobbyList[index].TR_Players.Keys.ToList();
                    foreach (ExtPlayer foreachPlayer in Players)
                    {
                        if (!foreachPlayer.IsCharacterData()) continue;
                        Trigger.SendChatMessage(foreachPlayer, $"~o~[Tank Royale] Победитель {winner.Name.Replace('_', ' ')} выжил с {LobbyList[index].TR_Players[winner].Health}HP и получил {Wallet.Format(LobbyList[index].TR_Bank)}$.");
                    }
                }
                EndGame(index);
            }
            catch (Exception e)
            {
                Log.Write($"Winner Exception: {e.ToString()}");
            }
        }
        private static void EndGame(int index, bool withplayers = true)
        {
            try
            {
                if (!LobbyList.ContainsKey(index)) return;
                if(withplayers && LobbyList[index].TR_Players.Keys.Count >= 1)
                {
                    List<ExtPlayer> Players = LobbyList[index].TR_Players.Keys.ToList();
                    foreach (ExtPlayer foreachPlayer in Players)
                    {
                        if (!foreachPlayer.IsCharacterData()) continue;
                        PlayerExitFromTR(foreachPlayer);
                    }
                }
                DeleteAllVehicles(index);
                if (LobbyList[index].TR_Marker != null && LobbyList[index].TR_Marker.Exists) LobbyList[index].TR_Marker.Delete();
                LobbyList[index].TR_Marker = null;
                if (LobbyList[index].TR_LMarker != null && LobbyList[index].TR_LMarker.Exists) LobbyList[index].TR_LMarker.Delete();
                LobbyList[index].TR_LMarker = null;
                if (LobbyList[index].TR_Timer != null)
                {
                    Timers.Stop(LobbyList[index].TR_Timer);
                    LobbyList[index].TR_Timer = null;
                }
                if (LobbyList[index].TR_ZoneTimer != null)
                {
                    Timers.Stop(LobbyList[index].TR_ZoneTimer);
                    LobbyList[index].TR_ZoneTimer = null;
                }
                LobbyList[index].TR_TimerTicks = 0;
                LobbyList[index].TR_Stage = 0;
                LobbyList[index].TR_State = 0;
                LobbyList[index].TR_Bank = 0;
                LobbyList[index].TR_Players = new Dictionary<ExtPlayer, TRClass>();
                
                LobbyList.Remove(index);
                UpdateLobbyList();
            }
            catch (Exception e)
            {
                Log.Write($"EndGame Exception: {e.ToString()}");
            }
        }
        private static void DeleteAllVehicles(int index)
        {
            try
            {
                if(LobbyList[index].Vehicles.Count >= 1)
                {
                    foreach (var vehicleTank in LobbyList[index].Vehicles) 
                        VehicleStreaming.DeleteVehicle(vehicleTank);
                }
                LobbyList[index].Vehicles = new List<ExtVehicle>();
            } 
            catch(Exception e)
            {
                Log.Write($"DeleteAllVehicles Exception: {e.ToString()}");
            }
        }

        private static void TankRoyale_Tick(int index)
        {
            try
            {
                if (!LobbyList.ContainsKey(index)) return;
                LobbyList[index].TR_TimerTicks++;
                switch (LobbyList[index].TR_State)
                {
                    case 0:
                        if(LobbyList[index].TR_Timer != null)
                        {
                            Timers.Stop(LobbyList[index].TR_Timer);
                            LobbyList[index].TR_Timer = null;
                        }
                        if (LobbyList[index].TR_ZoneTimer != null)
                        {
                            Timers.Stop(LobbyList[index].TR_ZoneTimer);
                            LobbyList[index].TR_ZoneTimer = null;
                        }
                        break;
                    case 1:
                        if(LobbyList[index].TR_TimerTicks == 19) // 9:30
                        {
                            List<ExtPlayer> players = LobbyList[index].TR_Players.Keys.ToList();
                            foreach (ExtPlayer foreachPlayer in players)
                            {
                                var foreachSessionData = foreachPlayer.GetSessionData();
                                if (foreachSessionData == null) continue;
                                if (!SafeZones.IsSafeZone(foreachSessionData.InsideSafeZone, SafeZones.ZoneName.newarena)) Notify.Send(foreachPlayer, NotifyType.Info, NotifyPosition.BottomCenter, $"Если Вы будете вне Зелёной Зоны мероприятия, то не попадёте на мероприятие.", 3000);
                                else if (foreachPlayer.IsInVehicle) Notify.Send(foreachPlayer, NotifyType.Info, NotifyPosition.BottomCenter, $"Если Вы будете сидеть в машине, то не попадёте на мероприятие.", 3000);
                            }

                        }
                        else if(LobbyList[index].TR_TimerTicks == 20) // 10
                        {
                            if (LobbyList[index].TR_Players.Count >= 2)
                            {
                                List<ExtPlayer> players = LobbyList[index].TR_Players.Keys.ToList();
                                foreach (ExtPlayer foreachPlayer in players)
                                {
                                    if (!LobbyList[index].TR_Players.ContainsKey(foreachPlayer)) continue;
                                    var foreachSessionData = foreachPlayer.GetSessionData();
                                    if (foreachSessionData == null) continue;
                                    var foreachCharacterData = foreachPlayer.GetCharacterData();
                                    if (foreachCharacterData == null)
                                    {
                                        LobbyList[index].TR_Players.Remove(foreachPlayer);
                                        foreachSessionData.InTanksLobby = -1;
                                        continue;
                                    }
                                    if (!SafeZones.IsSafeZone(foreachSessionData.InsideSafeZone, SafeZones.ZoneName.newarena))
                                    {
                                        Notify.Send(foreachPlayer, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MpLeftCuzFar), 6000);
                                        LobbyList[index].TR_Players.Remove(foreachPlayer);
                                        foreachSessionData.InTanksLobby = -1;
                                        continue;
                                    }
                                    if (foreachPlayer.IsInVehicle)
                                    {
                                        Notify.Send(foreachPlayer, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MpLeftCuzVeh), 6000);
                                        LobbyList[index].TR_Players.Remove(foreachPlayer);
                                        foreachSessionData.InTanksLobby = -1;
                                        continue;
                                    }
                                    else if (UpdateData.CanIChange(foreachPlayer, LobbyList[index].LobbyPrice, true) != 255)
                                    {
                                        LobbyList[index].TR_Players.Remove(foreachPlayer);
                                        foreachSessionData.InTanksLobby = -1;
                                        continue;
                                    }
                                    else
                                    {
                                        Wallet.Change(foreachPlayer, -LobbyList[index].LobbyPrice);
                                        LobbyList[index].TR_Bank += Convert.ToInt32(LobbyList[index].LobbyPrice * 0.7);
                                        GameLog.Money($"player({foreachCharacterData.UUID})", $"system", LobbyList[index].LobbyPrice, $"TankRoyaleRegister");
                                        Notify.Send(foreachPlayer, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TargetLastAlive), 3000);
                                        Notify.Send(foreachPlayer, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LoseIfDamage), 5000);
                                    }

                                }
                                if (LobbyList[index].TR_Players.Count >= 2)
                                {
                                    LobbyList[index].TR_Stage = 0;
                                    LobbyList[index].TR_ZoneTimer = Timers.Start(1000, () =>
                                    {
                                        if(LobbyList[index].TR_State == 2)
                                        {
                                            players = LobbyList[index].TR_Players.Keys.ToList();
                                            foreach (ExtPlayer foreachPlayer in players)
                                            {
                                                if (foreachPlayer.Position.DistanceTo2D(TR_Center) > TR_Stages[LobbyList[index].TR_Stage] / 2)
                                                {
                                                    if (LobbyList[index].TR_Players.ContainsKey(foreachPlayer))
                                                        OnVehicleDamageHandler(LobbyList[index].TR_Players[foreachPlayer].Vehicle, 0f, 0f);
                                                }
                                            }
                                        }
                                    }, true);
                                    LobbyList[index].TR_Marker = (ExtMarker) NAPI.Marker.CreateMarker(28, TR_Center, new Vector3(), new Vector3(), TR_Stages[0], new Color(255, 0, 0, 50), dimension: 10);
                                    LobbyList[index].TR_State = 2;
                                    LobbyList[index].TR_RandomGun = (new Random().Next(0, 10) <= 7) ? false : true;
                                    int id = 0;
                                    lock (LobbyList[index].TR_Players.Keys)
                                    {
                                        foreach (ExtPlayer foreachPlayer in LobbyList[index].TR_Players.Keys)
                                        {
                                            var foreachCharacterData = foreachPlayer.GetCharacterData();
                                            if (foreachCharacterData == null) continue;
                                            Trigger.ClientEvent(foreachPlayer, "airsoft_lobbyMenuHandler", 2);
                                            Trigger.ClientEvent(foreachPlayer, "VehicleEnterToggle", false);
                                            Trigger.ClientEvent(foreachPlayer, "freeze", true);
                                            Trigger.Dimension(foreachPlayer, 10);
                                            var number = VehicleManager.GenerateNumber(VehicleAccess.Tank, "TANK-");
                                            var veh = VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey("rhino"), TR_Vehicles[id], 0f, 0, 0, numberPlate: number, numb: id, acc: VehicleAccess.Tank, petrol: 200, engine: true, dimension: 10);
                                            if (LobbyList[index].TR_RandomGun) veh.SetMod(10, 2);
                                            veh.Repair();
                                            foreachPlayer.Position = TR_Vehicles[id];
                                            foreachPlayer.Health = 100;
                                            LobbyList[index].TR_Players[foreachPlayer].Vehicle = veh;
                                            LobbyList[index].TR_Players[foreachPlayer].Health = (!LobbyList[index].TR_RandomGun) ? 50 : 100;
                                            LobbyList[index].Vehicles.Add(veh);
                                            Trigger.ClientEvent(foreachPlayer, "setIntoVehicle", veh, VehicleSeat.Driver - 1);
                                            if (!foreachCharacterData.Achievements[27])
                                            {
                                                foreachCharacterData.Achievements[27] = true;
                                                Notify.Send(foreachPlayer, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.HowToJump), 5000);
                                            }
                                            id++;
                                        }
                                    }
                                    Timers.StartOnce(2000, () =>
                                    {
                                        foreach (ExtPlayer foreachPlayer in LobbyList[index].TR_Players.Keys)
                                        {
                                            if (!foreachPlayer.IsCharacterData()) continue;
                                            Trigger.ClientEvent(foreachPlayer, "VehicleEnterToggle", true);
                                            Trigger.ClientEvent(foreachPlayer, "freeze", false);
                                            //NAPI.Player.SetPlayerIntoVehicle(p, TR_Players[p].Vehicle, (int)VehicleSeat.Driver);
                                            var vehicleLocalData = LobbyList[index].TR_Players[foreachPlayer].Vehicle.GetVehicleLocalData();
                                            if (foreachPlayer.Position.DistanceTo2D(TR_Center) > TR_Stages[0] / 2)
                                            {
                                                LobbyList[index].TR_Players[foreachPlayer].Vehicle.Position = TR_Vehicles[vehicleLocalData.Number];
                                                Trigger.ClientEvent(foreachPlayer, "setIntoVehicle", LobbyList[index].TR_Players[foreachPlayer].Vehicle, VehicleSeat.Driver - 1);
                                            }
                                        }
                                    }, true);
                                }
                                else
                                {
                                    if (LobbyList[index].TR_Players.Keys.Count >= 1)
                                    {
                                        List<ExtPlayer> Players = LobbyList[index].TR_Players.Keys.ToList();
                                        foreach (ExtPlayer foreachPlayer in Players)
                                        {
                                            if (!foreachPlayer.IsCharacterData()) continue;
                                            Trigger.SendChatMessage(foreachPlayer, LangFunc.GetText(LangType.Ru, DataName.TooNotManyPlayers));
                                        }
                                    }
                                    
                                    EndGame(index, false);
                                }
                            }
                            else
                            {
                                if (LobbyList[index].TR_Players.Keys.Count >= 1)
                                {
                                    List<ExtPlayer> Players = LobbyList[index].TR_Players.Keys.ToList();
                                    foreach (ExtPlayer foreachPlayer in Players)
                                    {
                                        if (!foreachPlayer.IsCharacterData()) continue;
                                        Trigger.SendChatMessage(foreachPlayer, LangFunc.GetText(LangType.Ru, DataName.TooNotManyPlayers));
                                    }
                                }
                                
                                EndGame(index, false);
                            }
                        }
                        break;
                    case 2:
                        switch (LobbyList[index].TR_TimerTicks)
                        {
                            case 21: // 10:30
                                LobbyList[index].TR_LMarker = (ExtMarker) NAPI.Marker.CreateMarker(28, TR_Center, new Vector3(), new Vector3(), TR_Stages[1], new Color(0, 255, 0, 50), dimension: 10);
                                lock (LobbyList[index].TR_Players.Keys)
                                {
                                    foreach (ExtPlayer foreachPlayer in LobbyList[index].TR_Players.Keys)
                                    {
                                        if (!foreachPlayer.IsCharacterData()) continue;
                                        Notify.Send(foreachPlayer, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RedCircleHp), 10000);
                                    }
                                }
                                break;
                            case 22: // 11
                                if (LobbyList[index].TR_Marker != null && LobbyList[index].TR_Marker.Exists) LobbyList[index].TR_Marker.Delete();
                                LobbyList[index].TR_Marker = null;
                                LobbyList[index].TR_Stage = 1;
                                LobbyList[index].TR_Marker = (ExtMarker) NAPI.Marker.CreateMarker(28, TR_Center, new Vector3(), new Vector3(), TR_Stages[1], new Color(255, 0, 0, 50), dimension: 10);
                                break;
                            case 23: // 11:30
                                if (LobbyList[index].TR_LMarker != null && LobbyList[index].TR_LMarker.Exists) LobbyList[index].TR_LMarker.Delete();
                                LobbyList[index].TR_LMarker = null;
                                
                                LobbyList[index].TR_LMarker = (ExtMarker) NAPI.Marker.CreateMarker(28, TR_Center, new Vector3(), new Vector3(), TR_Stages[2], new Color(0, 255, 0, 50), dimension: 10);
                                break;
                            case 24: // 12
                                if (LobbyList[index].TR_Marker != null && LobbyList[index].TR_Marker.Exists) LobbyList[index].TR_Marker.Delete();
                                LobbyList[index].TR_Marker = null;
                                LobbyList[index].TR_Stage = 2;
                                LobbyList[index].TR_Marker = (ExtMarker) NAPI.Marker.CreateMarker(28, TR_Center, new Vector3(), new Vector3(), TR_Stages[2], new Color(255, 0, 0, 50), dimension: 10);
                                break;
                            case 25: // 12:30
                                if (LobbyList[index].TR_LMarker != null && LobbyList[index].TR_LMarker.Exists) LobbyList[index].TR_LMarker.Delete();
                                LobbyList[index].TR_LMarker = null;
                                
                                LobbyList[index].TR_LMarker = (ExtMarker) NAPI.Marker.CreateMarker(28, TR_Center, new Vector3(), new Vector3(), TR_Stages[3], new Color(0, 255, 0, 50), dimension: 10);
                                break;
                            case 26: // 13
                                if (LobbyList[index].TR_Marker != null && LobbyList[index].TR_Marker.Exists) LobbyList[index].TR_Marker.Delete();
                                LobbyList[index].TR_Marker = null;
                                LobbyList[index].TR_Stage = 3;
                                LobbyList[index].TR_Marker = (ExtMarker) NAPI.Marker.CreateMarker(28, TR_Center, new Vector3(), new Vector3(), TR_Stages[3], new Color(255, 0, 0, 50), dimension: 10);
                                break;
                            case 27: // 13:30
                                if (LobbyList[index].TR_LMarker != null && LobbyList[index].TR_LMarker.Exists) LobbyList[index].TR_LMarker.Delete();
                                LobbyList[index].TR_LMarker = null;
                                
                                LobbyList[index].TR_LMarker = (ExtMarker) NAPI.Marker.CreateMarker(28, TR_Center, new Vector3(), new Vector3(), TR_Stages[4], new Color(0, 255, 0, 50), dimension: 10);
                                break;
                            case 28: // 14
                                if (LobbyList[index].TR_Marker != null && LobbyList[index].TR_Marker.Exists) LobbyList[index].TR_Marker.Delete();
                                LobbyList[index].TR_Marker = null;
                                LobbyList[index].TR_Stage = 4;
                                LobbyList[index].TR_Marker = (ExtMarker) NAPI.Marker.CreateMarker(28, TR_Center, new Vector3(), new Vector3(), TR_Stages[4], new Color(255, 0, 0, 50), dimension: 10);
                                break;
                            case 40: // 20
                                if (LobbyList[index].TR_Marker != null && LobbyList[index].TR_Marker.Exists) LobbyList[index].TR_Marker.Delete();
                                LobbyList[index].TR_Marker = null;
                                
                                if (LobbyList[index].TR_LMarker != null && LobbyList[index].TR_LMarker.Exists) LobbyList[index].TR_LMarker.Delete();
                                LobbyList[index].TR_LMarker = null;
                                
                                if (LobbyList[index].TR_Players.Keys.Count >= 1)
                                {
                                    List<ExtPlayer> Players = LobbyList[index].TR_Players.Keys.ToList();
                                    foreach (ExtPlayer foreachPlayer in Players)
                                    {
                                        if (!foreachPlayer.IsCharacterData()) continue;
                                        Trigger.SendChatMessage(foreachPlayer, LangFunc.GetText(LangType.Ru, DataName.NoWinnerWin));
                                    }
                                }
                                EndGame(index);
                                break;
                            default:
                                // Not supposed to end up here. 
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"TankRoyale_Tick Exception: {e.ToString()}");
            }
        }
    }
}
