using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Functions;
using NeptuneEvo.MoneySystem;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Players;

namespace NeptuneEvo.Core
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
        public int MaxPlayers { get; set; } = 15;

        [JsonIgnore]
        public int GameStatus { get; set; } = 0;

        [JsonIgnore]
        public int GameStartSeconds { get; set; } = 120;

        [JsonIgnore]
        public int mafiaGameVoteStatus = 0;

        [JsonIgnore]
        public int mafiaGameDiscussionStatus = -1;

        [JsonIgnore]
        public int mafiaGamePlayerTurn = 0;
        
        [JsonIgnore]
        public List<ExtPlayer> MafiaPlayers = new List<ExtPlayer>();
        
        [JsonIgnore]
        public Dictionary<int, MafiaGamePlayerData> MafiaGamePlayersInfo = new Dictionary<int, MafiaGamePlayerData>();

        [JsonIgnore]
        public int totalPlayers = 0;
        
        [JsonIgnore]
        public int totalMafia = 0;

        [JsonIgnore]
        public string mafiaGameTimer = null;

        [JsonIgnore]
        public List<int> mafiaGame_toKickList = new List<int>();
        
        [JsonIgnore]
        public int wasKilled = -1;
        
        [JsonIgnore]
        public int wasHealed = -1;
        
        [JsonIgnore]
        public bool doctorHealedYourself = false;
        
        [JsonIgnore]
        public int doctorLastHealNumber = -1;
        
        [JsonIgnore]
        public sbyte doctorLastHealNumberNightStatus = 0;
        
        [JsonIgnore]
        public int wasFucked = -1;

        public LobbyData(int index, string name, int price, string password, int lobby_map)
        {
            this.LobbyIndex = index;
            this.LobbyName = name;
            this.LobbyPrice = price;
            this.LobbyPassword = password;
            this.LobbyMap = lobby_map;
            this.MaxPlayers = 15;
            this.GameStatus = 1;
        }
    }
    
    class MafiaGamePlayerData
    {
        public int PlayerId = 0;
        public string PlayerName = "None";
        public int PlayerRole = 0;
        public bool PlayerMute = true;
        public bool PlayerLife = true;

        public MafiaGamePlayerData(int Id, string Name, int Role)
        {
            this.PlayerId = Id;
            this.PlayerName = Name;
            this.PlayerRole = Role;
            this.PlayerMute = true;
            this.PlayerLife = true;
        }
    }

    class MafiaGame : Script
    {
        private static readonly nLog Log = new nLog("Events.MafiaGame");
        
        private static Dictionary<int, LobbyData> LobbyList = new Dictionary<int, LobbyData>();
        public static List<ExtPlayer> PlayersInLobbyMenu = new List<ExtPlayer>();

        private static Dictionary<int, string> role_names = new Dictionary<int, string>
        {
            [1] = "Мафия",
            [2] = "Комиссар",
            [3] = "Доктор",
            [4] = "Куртизанка",
            [5] = "Мирный житель"
        };
        
        private static List<List<(Vector3, float)>> MafiaGamePlayersPositions = new List<List<(Vector3, float)>>()
        {
            new List<(Vector3, float)>()
            {
                (new Vector3(2519.336, -238.23717, -70.73713), -96.85236f),
                (new Vector3(2522.4326, -237.52075, -70.73712), 123.83377f),
                (new Vector3(2520.157, -240.01765, -70.7315), -31.202871f),
                (new Vector3(2519.365, -238.89964, -70.73715), -77.21137f),
                (new Vector3(2519.5999, -239.52196, -70.7315), -54.13836f),
                (new Vector3(2521.3218, -236.8215, -70.73712), 165.52219f),
                (new Vector3(2522.793, -238.16722, -70.73712), 100.99569f),
                (new Vector3(2520.757, -240.25385, -70.73715), -7.7746263f),
                (new Vector3(2521.43, -240.24484, -70.73715), 17.733465f),
                (new Vector3(2522.1055, -239.96161, -70.73715), 36.686897f),
                (new Vector3(2522.5256, -239.43985, -70.73715), 65.24883f),
                (new Vector3(2519.6016, -237.58075, -70.73712), - 125.53464f),
                (new Vector3(2520.0696, -237.10812, -70.73712), -142.99265f),
                (new Vector3(2520.6545, -236.81816, -70.73712), -169.7154f),
                (new Vector3(2521.9766, -237.00273, -70.73712), 143.42322f),
            },
            new List<(Vector3, float)>()
            {
                (new Vector3(2934.0652, 5326.258, 100.74636), -84.42491f),
                (new Vector3(2938.2097, 5325.178, 100.84902), 29.284285f),
                (new Vector3(2934.0212, 5327.4595, 100.79833), -101.481865f),
                (new Vector3(2934.764, 5325.197, 100.62395), -49.474487f),
                (new Vector3(2934.6445, 5328.0967, 100.90689), -122.94486f),
                (new Vector3(2935.5889, 5324.6396, 100.628716), -23.359068f),
                (new Vector3(2935.3313, 5328.646, 101.01914), -144.65996f),
                (new Vector3(2937.4714, 5324.8887, 100.77214), 20.09977f),
                (new Vector3(2935.9521, 5328.751, 101.074295), -153.40901f),
                (new Vector3(2936.793, 5328.9453, 101.152794), -166.59279f),
                (new Vector3(2937.8982, 5328.615, 101.138466), 160.07335f),
                (new Vector3(2938.6807, 5327.9272, 101.09124), 127.20851f),
                (new Vector3(2939.2153, 5327.139, 101.03547), 119.46783f),
                (new Vector3(2939.018, 5325.62, 100.91758), 36.618145f),
                (new Vector3(2936.6394, 5324.7236, 100.70417), 18.478296f),
            }
        };
        
        public static void MafiaLobbiesStartInterval()
        {
            try
            {
                if (LobbyList.Count < 1) return;
                
                foreach (var lobby in LobbyList)
                {
                    if (lobby.Value.GameStatus <= 1 && lobby.Value.GameStartSeconds > 0)
                        lobby.Value.GameStartSeconds -= 1;
                }
            }
            catch (Exception e)
            {
                Log.Write($"MafiaLobbiesStartInterval Exception: {e.ToString()}");
            }
        }
        
        [RemoteEvent("MafiaUpdateServerPlayersInLobbyMenuList")]
        public static void MafiaUpdateServerPlayersInLobbyMenuList(ExtPlayer player, int state)
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
                Log.Write($"MafiaUpdateServerPlayersInLobbyMenuList Exception: {e.ToString()}");
            }
        }

        public static void CheckMafiaLobby(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                if (!FunctionsAccess.IsWorking("checkMafiaLobby"))
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

                Trigger.ClientEvent(player, "airsoft_lobbyMenuHandler", 1, JsonConvert.SerializeObject(LobbyList.Values), "mafia");
            }
            catch (Exception e)
            {
                Log.Write($"CheckMafiaLobby Exception: {e.ToString()}");
            }
        }
        
        public static void UpdateMafiaLobbyStats(int index)
        {
            try
            {
                if (LobbyList[index].GameStatus <= 1 && LobbyList[index].MafiaPlayers.Count > 0)
                {
                    foreach (ExtPlayer foreachPlayer in LobbyList[index].MafiaPlayers)
                    {
                        if (foreachPlayer.IsCharacterData())
                            Trigger.ClientEvent(foreachPlayer, "airsoft_lobbyMenuHandler", 3, JsonConvert.SerializeObject(new int[3] { LobbyList[index].MafiaPlayers.Count, 6, LobbyList[index].MaxPlayers }), "mafia", LobbyList[index].GameStartSeconds);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"UpdateMafiaLobbyStats Exception: {e.ToString()}");
            }
        }

        public static void OutMafiaLobbyZone(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                int lobbyId = sessionData.InMafiaLobby;
                if (lobbyId >= 0)
                {
                    if (LobbyList.ContainsKey(lobbyId) && LobbyList[lobbyId].GameStatus <= 1 && LobbyList[lobbyId].MafiaPlayers.Contains(player))
                    {
                        sessionData.InMafiaLobby = -1;
                        LobbyList[lobbyId].MafiaPlayers.Remove(player);
                        Wallet.Change(player, Math.Abs(LobbyList[lobbyId].LobbyPrice));
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LeftLobby), 3000);

                        if (LobbyList[lobbyId].MafiaPlayers.Count < 1)
                        {
                            if (LobbyList[lobbyId].mafiaGameTimer != null)
                                Timers.Stop(LobbyList[lobbyId].mafiaGameTimer);
                            
                            LobbyList.Remove(lobbyId);
                            UpdateLobbyList();
                        }
                        else UpdateMafiaLobbyStats(lobbyId);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"OutMafiaLobbyZone Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("mafia_createLobby_server")]
        public static void CreateMafiaLobby(ExtPlayer player, string lobby_name, int lobby_price, string lobby_password, int gameMap)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                if (!FunctionsAccess.IsWorking("CreateMafiaLobby"))
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

                if (sessionData.InMafiaLobby >= 0)
                {
                    LeaveMafiaGame(player);
                }
                
                LobbyList.Add(player.Value, new LobbyData(player.Value, lobby_name.Length < 1 ? player.Name.Split("_")[0] : lobby_name, lobby_price, lobby_password, gameMap));
                LobbyList[player.Value].MafiaPlayers.Add(player);
                sessionData.InMafiaLobby = player.Value;
                UpdateLobbyList();
                
                if (LobbyList[player.Value].mafiaGameTimer != null)
                    Timers.Stop(LobbyList[player.Value].mafiaGameTimer);

                LobbyList[player.Value].mafiaGameTimer = Timers.StartOnce("mafiaGameTimer", 120000, () => mafiaGameTimerFunction(player.Value), true); // change to 120000

                Wallet.Change(player, -Math.Abs(lobby_price));
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Чтобы игра началась, нужно от 6 участников в лобби. Если вы покините зону, то игра будет отменена.", 5000);
                Trigger.ClientEvent(player, "airsoft_lobbyMenuHandler", 3, JsonConvert.SerializeObject(new int[3] { LobbyList[player.Value].MafiaPlayers.Count, 6, LobbyList[player.Value].MaxPlayers }), "mafia", LobbyList[player.Value].GameStartSeconds);
            }
            catch (Exception e)
            {
                Log.Write($"CreateMafiaLobby Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("mafia_joinLobby_server")]
        public static void RemoteEvent_joinMafiaLobby(ExtPlayer player, int index, string password)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                if (!FunctionsAccess.IsWorking("joinMafiaLobby"))
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
                
                if (!LobbyList.ContainsKey(index))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LobbyNotFound), 3000);
                    return;
                }
                
                if (sessionData.InMafiaLobby >= 0 && sessionData.InMafiaLobby == index)
                {
                    Trigger.ClientEvent(player, "airsoft_lobbyMenuHandler", 3, JsonConvert.SerializeObject(new int[3] { LobbyList[index].MafiaPlayers.Count, 6, LobbyList[index].MaxPlayers }), "mafia", LobbyList[index].GameStartSeconds);
                    return;
                }
                
                if (LobbyList[index].LobbyPassword != null && LobbyList[index].LobbyPassword != password)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LobbyIncorrectPassowrd), 3000);
                    return;
                }
                
                if (LobbyList[index].MafiaPlayers.Count >= LobbyList[index].MaxPlayers)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxPlayersOn), 3000);
                    return;
                }

                if (LobbyList[index].GameStatus == 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoGameStarted), 5000);
                    return;
                }

                if (LobbyList[index].GameStatus >= 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GameIsOn), 5000);
                    return;
                }

                if (LobbyList[index].MafiaPlayers.Contains(player)) return;

                if (characterData.Money < LobbyList[index].LobbyPrice)
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GameStartCost, LobbyList[index].LobbyPrice), 5000);
                    return;
                }
                
                if (sessionData.InMafiaLobby >= 0)
                    LeaveMafiaGame(player);

                LobbyList[index].MafiaPlayers.Add(player);
                sessionData.InMafiaLobby = index;
                Wallet.Change(player, -Math.Abs(LobbyList[index].LobbyPrice));
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GameReg), 8000);

                UpdateMafiaLobbyStats(index);
                
                if (LobbyList[index].MafiaPlayers.Count == 15 && LobbyList[index].mafiaGameTimer != null)
                {
                    Timers.Stop(LobbyList[index].mafiaGameTimer);
                    mafiaGameTimerFunction(index);
                }
            }
            catch (Exception e)
            {
                Log.Write($"joinMafiaLobby Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("voteMafiaGame")]
        public static void RemoteEvent_voteMafiaGame(ExtPlayer player, int target_id)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                int lobbyId = sessionData.InMafiaLobby;
                if (lobbyId >= 0 && LobbyList.ContainsKey(lobbyId))
                {
                    if (player.Value == target_id && player.GetSharedData<int>("mafiaGameRole") != 3)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantVoteYourself), 3000);
                        return;
                    }

                    if (player.Value == target_id && player.GetSharedData<int>("mafiaGameRole") == 3 && LobbyList[lobbyId].doctorHealedYourself)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyHealYourself), 3000);
                        return;
                    }

                    if (!LobbyList[lobbyId].MafiaGamePlayersInfo.ContainsKey(target_id))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoPlayerGame), 3000);
                        return;
                    }

                    if (LobbyList[lobbyId].mafiaGameVoteStatus == 4 && player.GetSharedData<int>("mafiaGameRole") == 1)
                    {
                        LobbyList[lobbyId].mafiaGame_toKickList.Add(target_id);

                        if (LobbyList[lobbyId].mafiaGame_toKickList.Count >= LobbyList[lobbyId].totalMafia)
                        {
                            if (LobbyList[lobbyId].mafiaGameTimer != null)
                                Timers.Stop(LobbyList[lobbyId].mafiaGameTimer);
                            mafiaGameTimerFunction(lobbyId);
                        }

                        SendTextInfoForPlayer(player, LangFunc.GetText(LangType.Ru, DataName.VoteOk));
                        Trigger.ClientEvent(player, "updateVoteStatusMafiaGame", false);
                    }
                    else if (LobbyList[lobbyId].mafiaGameVoteStatus == 6 && player.GetSharedData<int>("mafiaGameRole") == 2)
                    {
                        foreach (ExtPlayer MafiaPlayer in LobbyList[lobbyId].MafiaPlayers)
                        {
                            try
                            {
                                if (MafiaPlayer.IsCharacterData())
                                {
                                    if (MafiaPlayer.Value == target_id)
                                    {
                                        if (MafiaPlayer.GetSharedData<int>("mafiaGameRole") == 1) SendTextInfoForPlayer(player, LangFunc.GetText(LangType.Ru, DataName.PlayerIsMafia), 10000);
                                        else SendTextInfoForPlayer(player, LangFunc.GetText(LangType.Ru, DataName.PlayerNotMafia), 10000);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Write($"voteMafiaGame Foreach #2 Exception: {e.ToString()}");
                            }
                        }

                        Trigger.ClientEvent(player, "mafia_closeGameMenu");

                        if (LobbyList[lobbyId].mafiaGameTimer != null) Timers.Stop(LobbyList[lobbyId].mafiaGameTimer);
                        mafiaGameTimerFunction(lobbyId);
                    }
                    else if (LobbyList[lobbyId].mafiaGameVoteStatus == 7 && player.GetSharedData<int>("mafiaGameRole") == 3)
                    {
                        if (LobbyList[lobbyId].doctorLastHealNumber == target_id && LobbyList[lobbyId].doctorLastHealNumberNightStatus > 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHealedPlayerNight), 3000);
                            return;
                        }
                        
                        foreach (ExtPlayer MafiaPlayer in LobbyList[lobbyId].MafiaPlayers)
                        {
                            try
                            {
                                if (MafiaPlayer.IsCharacterData())
                                {
                                    if (MafiaPlayer.Value == target_id)
                                    {
                                        if (player.Value == target_id)
                                            LobbyList[lobbyId].doctorHealedYourself = true;

                                        LobbyList[lobbyId].wasHealed = target_id;
                                        LobbyList[lobbyId].doctorLastHealNumber = target_id;
                                        LobbyList[lobbyId].doctorLastHealNumberNightStatus = 2;
                                        SendTextInfoForPlayer(player, LangFunc.GetText(LangType.Ru, DataName.VoteOk));
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Write($"voteMafiaGame Foreach #3 Exception: {e.ToString()}");
                            }
                        }

                        Trigger.ClientEvent(player, "mafia_closeGameMenu");

                        if (LobbyList[lobbyId].mafiaGameTimer != null) Timers.Stop(LobbyList[lobbyId].mafiaGameTimer);
                        mafiaGameTimerFunction(lobbyId);
                    }
                    else if (LobbyList[lobbyId].mafiaGameVoteStatus == 8 && player.GetSharedData<int>("mafiaGameRole") == 4)
                    {
                        foreach (ExtPlayer MafiaPlayer in LobbyList[lobbyId].MafiaPlayers)
                        {
                            try
                            {
                                if (MafiaPlayer.IsCharacterData())
                                {
                                    if (MafiaPlayer.Value == target_id)
                                    {
                                        LobbyList[lobbyId].wasFucked = target_id;
                                        SendTextInfoForPlayer(player, LangFunc.GetText(LangType.Ru, DataName.VoteOk));
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Write($"voteMafiaGame Foreach #4 Exception: {e.ToString()}");
                            }
                        }

                        Trigger.ClientEvent(player, "mafia_closeGameMenu");

                        if (LobbyList[lobbyId].mafiaGameTimer != null) Timers.Stop(LobbyList[lobbyId].mafiaGameTimer);
                        mafiaGameTimerFunction(lobbyId);
                    }
                    else if (LobbyList[lobbyId].mafiaGameVoteStatus == 12)
                    {
                        LobbyList[lobbyId].mafiaGame_toKickList.Add(target_id);

                        if (LobbyList[lobbyId].mafiaGame_toKickList.Count >= LobbyList[lobbyId].MafiaPlayers.Count)
                        {
                            if (LobbyList[lobbyId].mafiaGameTimer != null) Timers.Stop(LobbyList[lobbyId].mafiaGameTimer);
                            mafiaGameTimerFunction(lobbyId);
                        }

                        SendTextInfoForPlayer(player, LangFunc.GetText(LangType.Ru, DataName.VoteOk));
                        Trigger.ClientEvent(player, "updateVoteStatusMafiaGame", false);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"voteMafiaGame Exception: {e.ToString()}");
            }
        }

        private static void mafiaGameTimerFunction(int index)
        {
            try
            {
                if (!LobbyList.ContainsKey(index)) return;
                
                if (LobbyList[index].mafiaGameTimer != null) Timers.Stop(LobbyList[index].mafiaGameTimer);

                if (LobbyList[index].GameStatus == 1)
                {
                    if (LobbyList[index].MafiaPlayers.Count < 6) // change to 6
                    {
                        foreach (ExtPlayer MafiaPlayer in LobbyList[index].MafiaPlayers)
                        {
                            try
                            {
                                var foreachSessionData = MafiaPlayer.GetSessionData();
                                
                                if (MafiaPlayer.IsCharacterData() && foreachSessionData != null)
                                {
                                    Notify.Send(MafiaPlayer, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoPlayersMafia), 3000);
                                    Trigger.ClientEvent(MafiaPlayer, "mafiaLobbyFunctions_client", 2);
                                    Wallet.Change(MafiaPlayer, +Math.Abs(LobbyList[index].LobbyPrice));
                                    foreachSessionData.InMafiaLobby = -1;
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Write($"mafiaGameTimerFunction Foreach #1 Exception: {e.ToString()}");
                            }
                        }
                        
                        LobbyList.Remove(index);
                        UpdateLobbyList();
                    }
                    else
                    {
                        sbyte mafiaLimit = 0;
                        sbyte commissarLimit = 0;
                        sbyte doctorLimit = 0;
                        sbyte courtesanLimit = 0;
                        sbyte playerNumber = 0;
                        
                        // if (LobbyList[index].MafiaPlayers.Count == 1) // change to delete
                        // {
                        //     mafiaLimit = 1;
                        //     commissarLimit = 1;
                        //     doctorLimit = 1;
                        // }
                        // else
                        if (LobbyList[index].MafiaPlayers.Count >= 6 && LobbyList[index].MafiaPlayers.Count < 8)
                        {
                            mafiaLimit = 1;
                            commissarLimit = 1;
                            doctorLimit = 1;
                        }
                        else if (LobbyList[index].MafiaPlayers.Count >= 8 && LobbyList[index].MafiaPlayers.Count < 10)
                        {
                            mafiaLimit = 2;
                            commissarLimit = 1;
                            doctorLimit = 1;
                        }
                        else if (LobbyList[index].MafiaPlayers.Count >= 10)
                        {
                            mafiaLimit = 2;
                            commissarLimit = 1;
                            doctorLimit = 1;
                            courtesanLimit = 1;
                        }

                        foreach (ExtPlayer MafiaPlayer in LobbyList[index].MafiaPlayers)
                        {
                            try
                            {
                                if (MafiaPlayer.IsCharacterData())
                                {
                                    int playerRoleIndex = 5;
                                    int roleChance = new Random().Next(1, 5);
                                    playerNumber += 1;
                                    
                                    if (roleChance == 1 && mafiaLimit > 0 || playerNumber == LobbyList[index].MafiaPlayers.Count - 2 && mafiaLimit > 0)
                                    {
                                        playerRoleIndex = 1;
                                        mafiaLimit -= 1;
                                    }
                                    else if (roleChance == 2 && commissarLimit > 0 || playerNumber == LobbyList[index].MafiaPlayers.Count - 2 && commissarLimit > 0)
                                    {
                                        playerRoleIndex = 2;
                                        commissarLimit -= 1;
                                    }
                                    else if (roleChance == 3 && doctorLimit > 0 || playerNumber == LobbyList[index].MafiaPlayers.Count - 2 && doctorLimit > 0)
                                    {
                                        playerRoleIndex = 3;
                                        doctorLimit -= 1;
                                    }
                                    else if (roleChance == 4 && courtesanLimit > 0 || playerNumber == LobbyList[index].MafiaPlayers.Count - 2 && courtesanLimit > 0)
                                    {
                                        playerRoleIndex = 4;
                                        courtesanLimit -= 1;
                                    }
                                    
                                    MafiaPlayer.SetSharedData("mafiaGameRole", playerRoleIndex);

                                    if (!LobbyList[index].MafiaGamePlayersInfo.ContainsKey(MafiaPlayer.Value))
                                        LobbyList[index].MafiaGamePlayersInfo.Add(MafiaPlayer.Value, new MafiaGamePlayerData(MafiaPlayer.Value, MafiaPlayer.Name, playerRoleIndex));
                                    
                                    Trigger.ClientEvent(MafiaPlayer, "freeze", true);
                                    Trigger.Dimension(MafiaPlayer, 150);
                                    NAPI.Entity.SetEntityPosition(MafiaPlayer, MafiaGamePlayersPositions[LobbyList[index].LobbyMap][playerNumber - 1].Item1);
                                    NAPI.Entity.SetEntityRotation(MafiaPlayer, new Vector3(0, 0, MafiaGamePlayersPositions[LobbyList[index].LobbyMap][playerNumber - 1].Item2));

                                    Animations.AnimationPlay(MafiaPlayer, LobbyList[index].LobbyMap == 0 ? "3_16" : "1_2");
                                    
                                    Trigger.ClientEvent(MafiaPlayer, "mafia_startGameStatusUpdate");
                                    Trigger.ClientEvent(MafiaPlayer, "openMafiaHelpStartPage", 145); // change to 145
                                    Trigger.ClientEvent(MafiaPlayer, "mafia_updateMicroStatus", 1);
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Write($"mafiaGameTimerFunction Foreach #2 Exception: {e.ToString()}");
                            }
                        }

                        LobbyList[index].GameStatus = 2;
                        LobbyList[index].mafiaGameTimer = Timers.StartOnce("mafiaGameTimer", 145000, () => mafiaGameTimerFunction(index), true); // change to 145000
                    }
                }
                else if (LobbyList[index].GameStatus == 2)
                {
                    LobbyList[index].totalPlayers = LobbyList[index].MafiaPlayers.Count;
                    LobbyList[index].totalMafia = 0;

                    foreach (ExtPlayer MafiaPlayer in LobbyList[index].MafiaPlayers)
                    {
                        try
                        {
                            if (MafiaPlayer.IsCharacterData() && LobbyList[index].MafiaGamePlayersInfo.ContainsKey(MafiaPlayer.Value))
                            {
                                SendTextInfoForPlayer(MafiaPlayer, $"Ваша роль в этой игре: {role_names[MafiaPlayer.GetSharedData<int>("mafiaGameRole")]}", 10000);
                                Trigger.ClientEvent(MafiaPlayer, "mafia_startSoundSpeech", "role_take");
                                
                                if (MafiaPlayer.GetSharedData<int>("mafiaGameRole") == 1)
                                    LobbyList[index].totalMafia += 1;
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Write($"mafiaGameTimerFunction Foreach #3 Exception: {e.ToString()}");
                        }
                    }

                    LobbyList[index].GameStatus = 3;
                    LobbyList[index].mafiaGameTimer = Timers.StartOnce("mafiaGameTimer", 10000, () => mafiaGameTimerFunction(index), true);
                }
                else if (LobbyList[index].GameStatus == 3) // Первая ночь
                {
                    if (LobbyList[index].totalMafia < 2)
                    {
                        foreach (ExtPlayer MafiaPlayer in LobbyList[index].MafiaPlayers)
                        {
                            try
                            {
                                if (MafiaPlayer.IsCharacterData() && LobbyList[index].MafiaGamePlayersInfo.ContainsKey(MafiaPlayer.Value))
                                {
                                    Trigger.ClientEvent(MafiaPlayer, "mafia_startSoundSpeech", "night_mafia");
                                    Trigger.ClientEvent(MafiaPlayer, "setTimeCmd", 0, 0, 0);

                                    if (MafiaPlayer.GetSharedData<int>("mafiaGameRole") == 1)
                                        Trigger.ClientEvent(MafiaPlayer, "showMafiaGameMenu", JsonConvert.SerializeObject(LobbyList[index].MafiaGamePlayersInfo), true, false, false, LangFunc.GetText(LangType.Ru, DataName.MafiaKillsMaf), LangFunc.GetText(LangType.Ru, DataName.YouHave30Sec));
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Write($"mafiaGameTimerFunction Foreach #4 Exception: {e.ToString()}");
                            }
                        }
                        
                        LobbyList[index].GameStatus = 4;
                        mafiaGameTimerFunction(index);
                    }
                    else
                    {
                        foreach (ExtPlayer MafiaPlayer in LobbyList[index].MafiaPlayers)
                        {
                            try
                            {
                                if (MafiaPlayer.IsCharacterData() && LobbyList[index].MafiaGamePlayersInfo.ContainsKey(MafiaPlayer.Value))
                                {
                                    Trigger.ClientEvent(MafiaPlayer, "mafia_startSoundSpeech", "night_mafia");
                                    Trigger.ClientEvent(MafiaPlayer, "setTimeCmd", 0, 0, 0);

                                    if (MafiaPlayer.GetSharedData<int>("mafiaGameRole") == 1)
                                    {
                                        Trigger.ClientEvent(MafiaPlayer, "mafia_updateMicroStatus", 2);
                                        Trigger.ClientEvent(MafiaPlayer, "showMafiaGameMenu", JsonConvert.SerializeObject(LobbyList[index].MafiaGamePlayersInfo), true, false, false, LangFunc.GetText(LangType.Ru, DataName.MafiaKillsMaf), LangFunc.GetText(LangType.Ru, DataName.YouHave1Min));

                                        SendTextInfoForPlayer(MafiaPlayer, LangFunc.GetText(LangType.Ru, DataName.YouHave1MinDiscuss), 10000);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Write($"mafiaGameTimerFunction Foreach #5 Exception: {e.ToString()}");
                            }
                        }
                        
                        LobbyList[index].GameStatus = 4;
                        LobbyList[index].mafiaGameTimer = Timers.StartOnce("mafiaGameTimer", 60000, () => mafiaGameTimerFunction(index), true);
                    }
                }
                else if (LobbyList[index].GameStatus == 4)
                {
                    LobbyList[index].mafiaGameVoteStatus = 4;

                    LobbyList[index].mafiaGame_toKickList = new List<int>();
                    LobbyList[index].wasKilled = -1;

                    foreach (ExtPlayer MafiaPlayer in LobbyList[index].MafiaPlayers)
                    {
                        try
                        {
                            if (MafiaPlayer.IsCharacterData() && LobbyList[index].MafiaGamePlayersInfo.ContainsKey(MafiaPlayer.Value))
                            {
                                if (MafiaPlayer.GetSharedData<int>("mafiaGameRole") == 1)
                                {
                                    Trigger.ClientEvent(MafiaPlayer, "mafia_updateMicroStatus", 1);
                                    Trigger.ClientEvent(MafiaPlayer, "updateVoteStatusMafiaGame", true);

                                    SendTextInfoForPlayer(MafiaPlayer, LangFunc.GetText(LangType.Ru, DataName.MafiaTakeChoice), 10000);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Write($"mafiaGameTimerFunction Foreach #7 Exception: {e.ToString()}");
                        }
                    }

                    LobbyList[index].GameStatus = 5;
                    LobbyList[index].mafiaGameTimer = Timers.StartOnce("mafiaGameTimer", LobbyList[index].totalMafia > 1 ? 15000 : 30000, () => mafiaGameTimerFunction(index), true);
                }
                else if (LobbyList[index].GameStatus == 5)
                {
                    if (LobbyList[index].mafiaGame_toKickList.Count > 0)
                    {
                        List<(int, int)> mostValues = new List<(int, int)>();
                        
                        for (int i = 0; i < LobbyList[index].MafiaPlayers.Count; i++)
                        {
                            if (LobbyList[index].mafiaGame_toKickList.Count < 1) break;
                
                            var most = LobbyList[index].mafiaGame_toKickList.GroupBy(x => x).OrderByDescending(x => x.Count()).First();
                            LobbyList[index].mafiaGame_toKickList.RemoveAll(r => r == most.Key);
                            LobbyList[index].mafiaGame_toKickList.Remove(most.Key);
                            mostValues.Add((most.Key, most.Count()));
                        }

                        if (mostValues.Count < 1 || mostValues.Count > 1 && mostValues[0].Item2 == mostValues[1].Item2) LobbyList[index].wasKilled = -1;
                        else LobbyList[index].wasKilled = mostValues[0].Item1;
                    }
                    else
                    {
                        LobbyList[index].wasKilled = -1;
                    }

                    foreach (ExtPlayer MafiaPlayer in LobbyList[index].MafiaPlayers)
                    {
                        try
                        {
                            if (MafiaPlayer.IsCharacterData() && LobbyList[index].MafiaGamePlayersInfo.ContainsKey(MafiaPlayer.Value))
                            {
                                if (MafiaPlayer.GetSharedData<int>("mafiaGameRole") == 1)
                                    Trigger.ClientEvent(MafiaPlayer, "mafia_closeGameMenu");
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Write($"mafiaGameTimerFunction Foreach #10 Exception: {e.ToString()}");
                        }
                    }

                    LobbyList[index].GameStatus = 6;
                    LobbyList[index].mafiaGameTimer = Timers.StartOnce("mafiaGameTimer", 3000, () => mafiaGameTimerFunction(index), true);
                }
                else if (LobbyList[index].GameStatus == 6)
                {
                    LobbyList[index].mafiaGameVoteStatus = 6;
                    
                    bool isHere = false;
                    
                    foreach (var item in LobbyList[index].MafiaGamePlayersInfo)
                    {
                        if (item.Value.PlayerRole == 2 && item.Value.PlayerLife)
                            isHere = true;
                    }
                    
                    LobbyList[index].GameStatus = 7;

                    if (isHere)
                    {
                        foreach (ExtPlayer MafiaPlayer in LobbyList[index].MafiaPlayers)
                        {
                            try
                            {
                                if (MafiaPlayer.IsCharacterData() && LobbyList[index].MafiaGamePlayersInfo.ContainsKey(MafiaPlayer.Value))
                                {
                                    Trigger.ClientEvent(MafiaPlayer, "mafia_startSoundSpeech", "night_komisar");
                                    Trigger.ClientEvent(MafiaPlayer, "setTimeCmd", 0, 0, 0);

                                    if (MafiaPlayer.GetSharedData<int>("mafiaGameRole") == 2)
                                    {
                                        Trigger.ClientEvent(MafiaPlayer, "showMafiaGameMenu", JsonConvert.SerializeObject(LobbyList[index].MafiaGamePlayersInfo), false, true, false, LangFunc.GetText(LangType.Ru, DataName.TryToFindMaf), LangFunc.GetText(LangType.Ru, DataName.YouHave30Sec));
                                        SendTextInfoForPlayer(MafiaPlayer, LangFunc.GetText(LangType.Ru, DataName.YouCanProverit), 10000);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Write($"mafiaGameTimerFunction Foreach #13 Exception: {e.ToString()}");
                            }
                        }

                        LobbyList[index].mafiaGameTimer = Timers.StartOnce("mafiaGameTimer", 30000, () => mafiaGameTimerFunction(index), true);
                    }
                    else mafiaGameTimerFunction(index);
                }
                else if (LobbyList[index].GameStatus == 7)
                {
                    LobbyList[index].mafiaGameVoteStatus = 7;
                    LobbyList[index].wasHealed = -1;
                    
                    bool isHere = false;
                    
                    foreach (var item in LobbyList[index].MafiaGamePlayersInfo)
                    {
                        if (item.Value.PlayerRole == 3 && item.Value.PlayerLife)
                            isHere = true;
                    }
                    
                    LobbyList[index].GameStatus = 8;

                    if (isHere)
                    {
                        foreach (ExtPlayer MafiaPlayer in LobbyList[index].MafiaPlayers)
                        {
                            try
                            {
                                if (MafiaPlayer.IsCharacterData())
                                {
                                    Trigger.ClientEvent(MafiaPlayer, "mafia_startSoundSpeech", "night_doctor");
                                    Trigger.ClientEvent(MafiaPlayer, "setTimeCmd", 0, 0, 0);

                                    if (MafiaPlayer.GetSharedData<int>("mafiaGameRole") == 2)
                                        Trigger.ClientEvent(MafiaPlayer, "mafia_closeGameMenu");

                                    if (MafiaPlayer.GetSharedData<int>("mafiaGameRole") == 3)
                                    {
                                        Trigger.ClientEvent(MafiaPlayer, "showMafiaGameMenu", JsonConvert.SerializeObject(LobbyList[index].MafiaGamePlayersInfo), false, true, false, LangFunc.GetText(LangType.Ru, DataName.MafYouCanHeal), LangFunc.GetText(LangType.Ru, DataName.YouHave30Sec));
                                        SendTextInfoForPlayer(MafiaPlayer, LangFunc.GetText(LangType.Ru, DataName.MafTryHeal), 10000);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Write($"mafiaGameTimerFunction Foreach #15 Exception: {e.ToString()}");
                            }
                        }
                        
                        LobbyList[index].mafiaGameTimer = Timers.StartOnce("mafiaGameTimer", 30000, () => mafiaGameTimerFunction(index), true);
                    }
                    else
                    {
                        foreach (ExtPlayer MafiaPlayer in LobbyList[index].MafiaPlayers)
                        {
                            try
                            {
                                if (MafiaPlayer.IsCharacterData())
                                {
                                    if (MafiaPlayer.GetSharedData<int>("mafiaGameRole") == 2)
                                        Trigger.ClientEvent(MafiaPlayer, "mafia_closeGameMenu");
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Write($"mafiaGameTimerFunction Foreach #15 Exception: {e.ToString()}");
                            }
                        }

                        mafiaGameTimerFunction(index);
                    }
                }
                else if (LobbyList[index].GameStatus == 8)
                {
                    LobbyList[index].mafiaGameVoteStatus = 8;
                    LobbyList[index].wasFucked = -1;
                    
                    bool isHere = false;
                    
                    foreach (var item in LobbyList[index].MafiaGamePlayersInfo)
                    {
                        if (item.Value.PlayerRole == 4 && item.Value.PlayerLife)
                            isHere = true;
                    }
                    
                    LobbyList[index].GameStatus = 9;

                    if (isHere)
                    {
                        foreach (ExtPlayer MafiaPlayer in LobbyList[index].MafiaPlayers)
                        {
                            try
                            {
                                if (MafiaPlayer.IsCharacterData())
                                {
                                    Trigger.ClientEvent(MafiaPlayer, "mafia_startSoundSpeech", "night_courtesan");
                                    Trigger.ClientEvent(MafiaPlayer, "setTimeCmd", 0, 0, 0);

                                    if (MafiaPlayer.GetSharedData<int>("mafiaGameRole") == 3)
                                    {
                                        if (LobbyList[index].wasHealed == -1)
                                            SendTextInfoForPlayer(MafiaPlayer, "К сожалению, время истекло. Вы никого не вылечили.");

                                        Trigger.ClientEvent(MafiaPlayer, "mafia_closeGameMenu");
                                    }

                                    if (MafiaPlayer.GetSharedData<int>("mafiaGameRole") == 4)
                                    {
                                        Trigger.ClientEvent(MafiaPlayer, "showMafiaGameMenu", JsonConvert.SerializeObject(LobbyList[index].MafiaGamePlayersInfo), false, true, false, LangFunc.GetText(LangType.Ru, DataName.MafSexGo), LangFunc.GetText(LangType.Ru, DataName.YouHave30Sec));
                                        SendTextInfoForPlayer(MafiaPlayer, LangFunc.GetText(LangType.Ru, DataName.MafSexTry), 10000);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Write($"mafiaGameTimerFunction Foreach #17 Exception: {e.ToString()}");
                            }
                        }
                        
                        LobbyList[index].mafiaGameTimer = Timers.StartOnce("mafiaGameTimer", 30000, () => mafiaGameTimerFunction(index), true);
                    }
                    else
                    {
                        foreach (ExtPlayer MafiaPlayer in LobbyList[index].MafiaPlayers)
                        {
                            try
                            {
                                if (MafiaPlayer.IsCharacterData())
                                {
                                    if (MafiaPlayer.GetSharedData<int>("mafiaGameRole") == 3)
                                    {
                                        if (LobbyList[index].wasHealed == -1)
                                            SendTextInfoForPlayer(MafiaPlayer, LangFunc.GetText(LangType.Ru, DataName.MafHealTimeOut));

                                        Trigger.ClientEvent(MafiaPlayer, "mafia_closeGameMenu");
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Write($"mafiaGameTimerFunction Foreach #17 Exception: {e.ToString()}");
                            }
                        }

                        mafiaGameTimerFunction(index);
                    }
                }
                else if (LobbyList[index].GameStatus == 9)
                {
                    foreach (ExtPlayer MafiaPlayer in LobbyList[index].MafiaPlayers)
                    {
                        try
                        {
                            if (MafiaPlayer.IsCharacterData())
                            {
                                if (MafiaPlayer.GetSharedData<int>("mafiaGameRole") == 4 && LobbyList[index].wasFucked == -1)
                                    SendTextInfoForPlayer(MafiaPlayer, LangFunc.GetText(LangType.Ru, DataName.MafSexTimeout));

                                Trigger.ClientEvent(MafiaPlayer, "mafia_closeGameMenu");
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Write($"mafiaGameTimerFunction Foreach #19 Exception: {e.ToString()}");
                        }
                    }

                    LobbyList[index].GameStatus = 10;
                    LobbyList[index].mafiaGameTimer = Timers.StartOnce("mafiaGameTimer", 5000, () => mafiaGameTimerFunction(index), true);
                }
                else if (LobbyList[index].GameStatus == 10)
                {
                    ExtPlayer playerToKick = null;

                    if (LobbyList[index].doctorLastHealNumberNightStatus > 0)
                        LobbyList[index].doctorLastHealNumberNightStatus -= 1;

                    if (LobbyList[index].wasFucked > -1 && LobbyList[index].MafiaGamePlayersInfo.ContainsKey(LobbyList[index].wasFucked))
                    {
                        if (LobbyList[index].MafiaGamePlayersInfo[LobbyList[index].wasFucked].PlayerRole == 3) LobbyList[index].wasHealed = -1;
                        else if (LobbyList[index].MafiaGamePlayersInfo[LobbyList[index].wasFucked].PlayerRole == 1 && LobbyList[index].totalMafia <= 1) LobbyList[index].wasKilled = -1;
                    }
                    
                    foreach (ExtPlayer MafiaPlayer in LobbyList[index].MafiaPlayers)
                    {
                        try
                        {
                            if (MafiaPlayer.IsCharacterData())
                            {
                                Trigger.ClientEvent(MafiaPlayer, "mafia_startSoundSpeech", "new_day");
                                Trigger.ClientEvent(MafiaPlayer, "updateClientMafiaStandbyStatus", false);
                                Trigger.ClientEvent(MafiaPlayer, "setTimeCmd", 12, 0, 0);

                                if (LobbyList[index].wasKilled > -1 && LobbyList[index].wasKilled != LobbyList[index].wasHealed)
                                {
                                    SendTextInfoForPlayer(MafiaPlayer, LangFunc.GetText(LangType.Ru, DataName.MafDied, LobbyList[index].wasKilled), 10000);

                                    if (MafiaPlayer.Value == LobbyList[index].wasKilled)
                                        playerToKick = MafiaPlayer;
                                    BattlePass.Repository.UpdateReward(MafiaPlayer, 131);
                                }
                                else SendTextInfoForPlayer(MafiaPlayer, LangFunc.GetText(LangType.Ru, DataName.MafNoDied), 10000);
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Write($"mafiaGameTimerFunction Foreach #21 Exception: {e.ToString()}");
                        }
                    }

                    if (playerToKick != null)
                    {
                        var foreachSessionData = playerToKick.GetSessionData();
                        
                        if (playerToKick.IsCharacterData() && foreachSessionData != null)
                        {
                            if (playerToKick.GetSharedData<int>("mafiaGameRole") == 1)
                                LobbyList[index].totalMafia -= 1;

                            LobbyList[index].MafiaPlayers.Remove(playerToKick);

                            playerToKick.SetSharedData("mafiaGameRole", 0);

                            NAPI.Entity.SetEntityPosition(playerToKick, new Vector3(-478.86032, -395.27307, 34.027653));
                            Trigger.Dimension(playerToKick);
                            Trigger.ClientEvent(playerToKick, "freeze", false);
                            Trigger.ClientEvent(playerToKick, "mafia_clearGameInfo");
                            Trigger.ClientEvent(playerToKick, "setTimeCmd", -1, -1, -1);
                            Animations.AnimationPlay(playerToKick, "-1");
                            foreachSessionData.InMafiaLobby = -1;

                            if (LobbyList[index].MafiaGamePlayersInfo.ContainsKey(playerToKick.Value))
                                LobbyList[index].MafiaGamePlayersInfo[playerToKick.Value].PlayerLife = false;

                            if (LobbyList[index].totalMafia >= (LobbyList[index].MafiaPlayers.Count - LobbyList[index].totalMafia))
                            {
                                mafiaGame_finish(index, 1);
                            }
                            else
                            {
                                LobbyList[index].mafiaGameDiscussionStatus = LobbyList[index].MafiaPlayers.Count;
                                LobbyList[index].mafiaGamePlayerTurn = 0;
                                LobbyList[index].GameStatus = 11;
                                LobbyList[index].mafiaGameTimer = Timers.StartOnce("mafiaGameTimer", 10000, () => mafiaGameTimerFunction(index), true);
                            }
                        }
                    }
                    else
                    {
                        LobbyList[index].mafiaGameDiscussionStatus = LobbyList[index].MafiaPlayers.Count;
                        LobbyList[index].mafiaGamePlayerTurn = 0;
                        LobbyList[index].GameStatus = 11;
                        LobbyList[index].mafiaGameTimer = Timers.StartOnce("mafiaGameTimer", 10000, () => mafiaGameTimerFunction(index), true);
                    }
                }
                else if (LobbyList[index].GameStatus == 11)
                {
                    int turnPlayerValue = 0;
                    ExtPlayer turnPlayer = null;

                    if (LobbyList[index].mafiaGameDiscussionStatus > 0 && LobbyList[index].MafiaPlayers.Count > LobbyList[index].mafiaGamePlayerTurn)
                        turnPlayer = LobbyList[index].MafiaPlayers[LobbyList[index].mafiaGamePlayerTurn];

                    foreach (var pMafia in LobbyList[index].MafiaGamePlayersInfo)
                    {
                        if (LobbyList[index].mafiaGameDiscussionStatus <= 0 && pMafia.Value.PlayerLife) pMafia.Value.PlayerMute = false;
                        else pMafia.Value.PlayerMute = true;
                    }

                    if (LobbyList[index].mafiaGameDiscussionStatus > 0 && turnPlayer != null && turnPlayer.IsCharacterData() && LobbyList[index].MafiaGamePlayersInfo.ContainsKey(turnPlayer.Value))
                    {
                        turnPlayerValue = turnPlayer.Value;
                        LobbyList[index].MafiaGamePlayersInfo[turnPlayer.Value].PlayerMute = false;
                    }

                    foreach (ExtPlayer MafiaPlayer in LobbyList[index].MafiaPlayers)
                    {
                        try
                        {
                            if (MafiaPlayer.IsCharacterData())
                            {
                                if (LobbyList[index].mafiaGameDiscussionStatus >= LobbyList[index].MafiaPlayers.Count)
                                {
                                    Trigger.ClientEvent(MafiaPlayer, "mafia_startSoundSpeech", "day_discussion");
                                    Trigger.ClientEvent(MafiaPlayer, "setTimeCmd", 12, 0, 0);
                                    Trigger.ClientEvent(MafiaPlayer, "mafia_updateMicroStatus", 3);
                                    SendTextInfoForPlayer(MafiaPlayer, LangFunc.GetText(LangType.Ru, DataName.EveryHave30Sec), 10000);
                                }

                                if (LobbyList[index].mafiaGameDiscussionStatus > 0)
                                {
                                    Trigger.ClientEvent(MafiaPlayer, "showMafiaGameMenu", JsonConvert.SerializeObject(LobbyList[index].MafiaGamePlayersInfo), true, false, true, LangFunc.GetText(LangType.Ru, DataName.DayMafiaVote), LangFunc.GetText(LangType.Ru, DataName.VseHave30Sec));
                                    
                                    if (MafiaPlayer == turnPlayer) Trigger.ClientEvent(MafiaPlayer, "mafia_updateMicroStatus", 2);
                                    else Trigger.ClientEvent(MafiaPlayer, "mafia_updateMicroStatus", 3);
                                    
                                    Notify.Send(MafiaPlayer, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OcheredChela, turnPlayerValue), 3000);
                                }
                                else
                                {
                                    Trigger.ClientEvent(MafiaPlayer, "showMafiaGameMenu", JsonConvert.SerializeObject(LobbyList[index].MafiaGamePlayersInfo), true, false, true, LangFunc.GetText(LangType.Ru, DataName.DayVoteing), LangFunc.GetText(LangType.Ru, DataName.VseHave1Min));
                                    Trigger.ClientEvent(MafiaPlayer, "mafia_updateMicroStatus", 2);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Write($"mafiaGameTimerFunction Foreach #23 Exception: {e.ToString()}");
                        }
                    }

                    if (LobbyList[index].mafiaGameDiscussionStatus >= 1)
                    {
                        LobbyList[index].mafiaGamePlayerTurn += 1;
                        LobbyList[index].mafiaGameDiscussionStatus -= 1;
                        LobbyList[index].mafiaGameTimer = Timers.StartOnce("mafiaGameTimer", 30000, () => mafiaGameTimerFunction(index), true);
                    }
                    else if (LobbyList[index].mafiaGameDiscussionStatus <= 0)
                    {
                        LobbyList[index].GameStatus = 12;
                        LobbyList[index].mafiaGameTimer = Timers.StartOnce("mafiaGameTimer", 60000, () => mafiaGameTimerFunction(index), true);
                    }
                }
                else if (LobbyList[index].GameStatus == 12)
                {
                    LobbyList[index].mafiaGameVoteStatus = 12;
                    LobbyList[index].mafiaGame_toKickList = new List<int>();
                    
                    foreach (var pMafia in LobbyList[index].MafiaGamePlayersInfo)
                    {
                        pMafia.Value.PlayerMute = true;
                    }

                    foreach (ExtPlayer MafiaPlayer in LobbyList[index].MafiaPlayers)
                    {
                        try
                        {
                            if (MafiaPlayer.IsCharacterData())
                            {
                                Trigger.ClientEvent(MafiaPlayer, "mafia_startSoundSpeech", "day_vote");
                                Trigger.ClientEvent(MafiaPlayer, "setTimeCmd", 12, 0, 0);
                                Trigger.ClientEvent(MafiaPlayer, "showMafiaGameMenu", JsonConvert.SerializeObject(LobbyList[index].MafiaGamePlayersInfo), true, true, true, LangFunc.GetText(LangType.Ru, DataName.DayPlayerChoice), LangFunc.GetText(LangType.Ru, DataName.VseHave15Sec));
                                Trigger.ClientEvent(MafiaPlayer, "mafia_updateMicroStatus", 1);
                                SendTextInfoForPlayer(MafiaPlayer, LangFunc.GetText(LangType.Ru, DataName.VoteJail15), 10000);
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Write($"mafiaGameTimerFunction Foreach #25 Exception: {e.ToString()}");
                        }
                    }

                    LobbyList[index].GameStatus = 13;
                    LobbyList[index].mafiaGameTimer = Timers.StartOnce("mafiaGameTimer", 15000, () => mafiaGameTimerFunction(index), true);
                }
                else if (LobbyList[index].GameStatus == 13)
                {
                    if (LobbyList[index].mafiaGame_toKickList.Count > 0)
                    {
                        int mostCommon = -1;
                        ExtPlayer playerToKick = null;
                        List<(int, int)> mostValues = new List<(int, int)>();
                        
                        for (int i = 0; i < LobbyList[index].MafiaPlayers.Count; i++)
                        {
                            if (LobbyList[index].mafiaGame_toKickList.Count < 1) break;
                
                            var most = LobbyList[index].mafiaGame_toKickList.GroupBy(x => x).OrderByDescending(x => x.Count()).First();
                            LobbyList[index].mafiaGame_toKickList.RemoveAll(r => r == most.Key);
                            LobbyList[index].mafiaGame_toKickList.Remove(most.Key);
                            mostValues.Add((most.Key, most.Count()));
                        }

                        if (mostValues.Count < 1 || mostValues.Count > 1 && mostValues[0].Item2 == mostValues[1].Item2) mostCommon = -1;
                        else mostCommon = mostValues[0].Item1;

                        if (mostCommon > -1)
                        {
                            foreach (ExtPlayer MafiaPlayer in LobbyList[index].MafiaPlayers)
                            {
                                try
                                {
                                    if (MafiaPlayer.IsCharacterData())
                                    {
                                        Trigger.ClientEvent(MafiaPlayer, "mafia_closeGameMenu");

                                        if (mostCommon >= 0)
                                        {
                                            SendTextInfoForPlayer(MafiaPlayer, LangFunc.GetText(LangType.Ru, DataName.SomebodyJail, mostCommon), 10000);
                                            
                                            if (MafiaPlayer.Value == mostCommon)
                                                playerToKick = MafiaPlayer;
                                            BattlePass.Repository.UpdateReward(MafiaPlayer, 131);
                                        }
                                        else SendTextInfoForPlayer(MafiaPlayer, LangFunc.GetText(LangType.Ru, DataName.VoteEquals), 10000);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Log.Write($"mafiaGameTimerFunction Foreach #28 Exception: {e.ToString()}");
                                }
                            }
                            
                            var foreachSessionData = playerToKick.GetSessionData();
                            if (playerToKick != null && playerToKick.IsCharacterData() && foreachSessionData != null)
                            {
                                if (playerToKick.GetSharedData<int>("mafiaGameRole") == 1)
                                    LobbyList[index].totalMafia -= 1;

                                LobbyList[index].MafiaPlayers.Remove(playerToKick);

                                playerToKick.SetSharedData("mafiaGameRole", 0);

                                NAPI.Entity.SetEntityPosition(playerToKick, new Vector3(-478.86032, -395.27307, 34.027653));
                                Trigger.Dimension(playerToKick);
                                Trigger.ClientEvent(playerToKick, "freeze", false);
                                Trigger.ClientEvent(playerToKick, "mafia_clearGameInfo");
                                Trigger.ClientEvent(playerToKick, "setTimeCmd", -1, -1, -1);
                                Animations.AnimationPlay(playerToKick, "-1");
                                foreachSessionData.InMafiaLobby = -1;

                                if (LobbyList[index].MafiaGamePlayersInfo.ContainsKey(playerToKick.Value))
                                    LobbyList[index].MafiaGamePlayersInfo[playerToKick.Value].PlayerLife = false;

                                if (LobbyList[index].totalMafia >= (LobbyList[index].MafiaPlayers.Count - LobbyList[index].totalMafia)) mafiaGame_finish(index, 1);
                                else if (LobbyList[index].totalMafia <= 0) mafiaGame_finish(index, 2);
                                else
                                {
                                    LobbyList[index].GameStatus = 3;
                                    LobbyList[index].mafiaGameTimer = Timers.StartOnce("mafiaGameTimer", 5000, () => mafiaGameTimerFunction(index), true);
                                }
                            }
                            else
                            {
                                LobbyList[index].GameStatus = 3;
                                LobbyList[index].mafiaGameTimer = Timers.StartOnce("mafiaGameTimer", 5000, () => mafiaGameTimerFunction(index), true);
                            }
                        }
                        else
                        {
                            foreach (ExtPlayer MafiaPlayer in LobbyList[index].MafiaPlayers)
                            {
                                try
                                {
                                    if (MafiaPlayer.IsCharacterData())
                                    {
                                        Trigger.ClientEvent(MafiaPlayer, "mafia_closeGameMenu");
                                        SendTextInfoForPlayer(MafiaPlayer, LangFunc.GetText(LangType.Ru, DataName.VoteEquals), 10000);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Log.Write($"mafiaGameTimerFunction Foreach #28 Exception: {e.ToString()}");
                                }
                            }
                            
                            LobbyList[index].GameStatus = 3;
                            LobbyList[index].mafiaGameTimer = Timers.StartOnce("mafiaGameTimer", 5000, () => mafiaGameTimerFunction(index), true);
                        }
                    }
                    else
                    {
                        foreach (ExtPlayer MafiaPlayer in LobbyList[index].MafiaPlayers)
                        {
                            try
                            {
                                if (MafiaPlayer.IsCharacterData())
                                {
                                    SendTextInfoForPlayer(MafiaPlayer, LangFunc.GetText(LangType.Ru, DataName.NoJail));
                                    Trigger.ClientEvent(MafiaPlayer, "mafia_closeGameMenu");
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Write($"mafiaGameTimerFunction Foreach #29 Exception: {e.ToString()}");
                            }
                        }

                        LobbyList[index].GameStatus = 3;
                        LobbyList[index].mafiaGameTimer = Timers.StartOnce("mafiaGameTimer", 5000, () => mafiaGameTimerFunction(index), true);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"mafiaGameTimerFunction Exception: {e.ToString()}");
            }
        }

        private static void mafiaGame_finish(int index, int state)
        {
            try
            {
                if (!LobbyList.ContainsKey(index)) return;
                
                int current_reward = (LobbyList[index].LobbyPrice * LobbyList[index].totalPlayers) / LobbyList[index].MafiaPlayers.Count;

                if (LobbyList[index].mafiaGameTimer != null)
                    Timers.Stop(LobbyList[index].mafiaGameTimer);

                foreach (ExtPlayer MafiaPlayer in LobbyList[index].MafiaPlayers)
                {
                    try
                    {
                        if (MafiaPlayer.IsCharacterData())
                        {
                            var foreachSessionData = MafiaPlayer.GetSessionData();
                            if (foreachSessionData == null) continue;
                            
                            Trigger.ClientEvent(MafiaPlayer, "mafia_clearGameInfo");
                            foreachSessionData.InMafiaLobby = -1;

                            if (state == 1)
                            {
                                Trigger.ClientEvent(MafiaPlayer, "mafia_startSoundSpeech", "mafia_win");
                                SendTextInfoForPlayer(MafiaPlayer, LangFunc.GetText(LangType.Ru, DataName.MafiaWin, current_reward), 10000);

                                if (MafiaPlayer.GetSharedData<int>("mafiaGameRole") == 1)
                                    Wallet.Change(MafiaPlayer, +Math.Abs(current_reward));
                            }
                            else if (state == 2)
                            {
                                Trigger.ClientEvent(MafiaPlayer, "mafia_startSoundSpeech", "peaceful_win");
                                SendTextInfoForPlayer(MafiaPlayer, LangFunc.GetText(LangType.Ru, DataName.MirWin, current_reward), 10000);

                                if (MafiaPlayer.GetSharedData<int>("mafiaGameRole") > 1)
                                    Wallet.Change(MafiaPlayer, +Math.Abs(current_reward));
                            }

                            MafiaPlayer.SetSharedData("mafiaGameRole", 0);
                            
                            NAPI.Entity.SetEntityPosition(MafiaPlayer, new Vector3(-478.86032, -395.27307, 34.027653));
                            Trigger.ClientEvent(MafiaPlayer, "setTimeCmd", -1, -1, -1);
                            Animations.AnimationPlay(MafiaPlayer, "-1");
                            Trigger.Dimension(MafiaPlayer);
                            Trigger.ClientEvent(MafiaPlayer, "freeze", false);         
                            BattlePass.Repository.UpdateReward(MafiaPlayer, 131);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Write($"mafiaGame_finish Foreach Exception: {e.ToString()}");
                    }
                }
                
                LobbyList.Remove(index);
                UpdateLobbyList();
            }
            catch (Exception e)
            {
                Log.Write($"mafiaGame_finish Exception: {e.ToString()}");
            }
        }

        public static void LeaveMafiaGame(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                int index = sessionData.InMafiaLobby;
                if (index >= 0 && LobbyList.ContainsKey(index))
                {
                    if (LobbyList[index].MafiaPlayers.Contains(player))
                    {
                        if (LobbyList[index].MafiaGamePlayersInfo.ContainsKey(player.Value) && LobbyList[index].GameStatus > 1)
                            LobbyList[index].MafiaGamePlayersInfo[player.Value].PlayerLife = false;

                        LobbyList[index].MafiaPlayers.Remove(player);

                        if (LobbyList[index].GameStatus > 1)
                        {
                            player.SetSharedData("mafiaGameRole", 0);

                            NAPI.Entity.SetEntityPosition(player, new Vector3(-478.86032, -395.27307, 34.027653));
                            Trigger.Dimension(player);
                            Trigger.ClientEvent(player, "freeze", false);
                            Trigger.ClientEvent(player, "mafia_clearGameInfo");
                            Trigger.ClientEvent(player, "setTimeCmd", -1, -1, -1);
                            Animations.AnimationPlay(player, "-1");
                            //BattlePass.Repository.UpdateReward(player, 131);
                        }

                        sessionData.InMafiaLobby = -1;

                        Wallet.Change(player, +Math.Abs(LobbyList[index].LobbyPrice));

                        if (LobbyList[index].MafiaPlayers.Count < 6 && LobbyList[index].GameStatus > 1)
                        {
                            if (LobbyList[index].mafiaGameTimer != null)
                                Timers.Stop(LobbyList[index].mafiaGameTimer);

                            foreach (ExtPlayer MafiaPlayer in LobbyList[index].MafiaPlayers)
                            {
                                try
                                {
                                    var foreachSessionData = MafiaPlayer.GetSessionData();
                                    if (MafiaPlayer.IsCharacterData() && foreachSessionData != null)
                                    {
                                        Notify.Send(MafiaPlayer, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OstalosMaloPla), 3000);

                                        MafiaPlayer.SetSharedData("mafiaGameRole", 0);

                                        NAPI.Entity.SetEntityPosition(MafiaPlayer, new Vector3(-478.86032, -395.27307, 34.027653));
                                        Trigger.Dimension(MafiaPlayer);
                                        Trigger.ClientEvent(MafiaPlayer, "freeze", false);
                                        Trigger.ClientEvent(MafiaPlayer, "mafia_clearGameInfo");
                                        Trigger.ClientEvent(MafiaPlayer, "setTimeCmd", -1, -1, -1);
                                        Animations.AnimationPlay(MafiaPlayer, "-1");
                                        foreachSessionData.InMafiaLobby = -1;

                                        Wallet.Change(MafiaPlayer, +Math.Abs(LobbyList[index].LobbyPrice));
                                        
                                        BattlePass.Repository.UpdateReward(MafiaPlayer, 131);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Log.Write($"LeaveMafiaGame Foreach Exception: {e.ToString()}");
                                }
                            }

                            LobbyList.Remove(index);
                            UpdateLobbyList();
                        }
                        else if (LobbyList[index].MafiaPlayers.Count >= 1)
                        {
                            foreach (ExtPlayer MafiaPlayer in LobbyList[index].MafiaPlayers)
                            {
                                try
                                {
                                    if (MafiaPlayer.IsCharacterData())
                                        Trigger.ClientEvent(player, "airsoft_lobbyMenuHandler", 3, JsonConvert.SerializeObject(new int[3] { LobbyList[index].MafiaPlayers.Count, 6, LobbyList[index].MaxPlayers }), "mafia", LobbyList[index].GameStartSeconds);
                                }
                                catch (Exception e)
                                {
                                    Log.Write($"LeaveMafiaGame Foreach Exception: {e.ToString()}");
                                }
                            }
                        }
                        else
                        {
                            LobbyList.Remove(index);
                            UpdateLobbyList();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"LeaveMafiaGame Exception: {e.ToString()}");
            }
        }

        public static void SendTextInfoForPlayer(ExtPlayer player, string msg, int delay = 5000)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                player.SendChatMessage("~o~[Мафия] " + msg);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, msg, delay);
            }
            catch (Exception e)
            {
                Log.Write($"SendTextInfoForPlayer Exception: {e.ToString()}");
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
                            Trigger.ClientEvent(foreachPlayer, "airsoft_updateLobbyList_client", JsonConvert.SerializeObject(LobbyList), "mafia");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"UpdateLobbyList Exception: {e.ToString()}");
            }
        }

        [ServerEvent(Event.PlayerDeath)]
        public void OnPlayerDeath(ExtPlayer player, Player killer, uint reason)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                int index = sessionData.InMafiaLobby;
                if (index >= 0 && LobbyList.ContainsKey(index) && player.HasSharedData("mafiaGameRole") && player.GetSharedData<int>("mafiaGameRole") >= 1)
                {
                    if (LobbyList[index].MafiaPlayers.Contains(player))
                        LeaveMafiaGame(player);
                }
            }
            catch (Exception e)
            {
                Log.Write($"OnPlayerDeath Exception: {e.ToString()}");
            }
        }

        public static void OnPlayerDisconnected(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                int index = sessionData.InMafiaLobby;
                if (index >= 0 && LobbyList.ContainsKey(index) && player.HasSharedData("mafiaGameRole") && player.GetSharedData<int>("mafiaGameRole") >= 1)
                {
                    NAPI.Entity.SetEntityPosition(player, new Vector3(-478.86032, -395.27307, 34.027653));
                    Trigger.Dimension(player);
                    
                    if (LobbyList[index].MafiaPlayers.Contains(player))
                        LeaveMafiaGame(player);
                }

                if (PlayersInLobbyMenu.Contains(player))
                    PlayersInLobbyMenu.Remove(player);
            }
            catch (Exception e)
            {
                Log.Write($"OnPlayerDisconnected Exception: {e.ToString()}");
            }
        }
    }
}