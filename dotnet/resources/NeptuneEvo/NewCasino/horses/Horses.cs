using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using Localization;
using NeptuneEvo.Quests;

namespace NeptuneEvo.NewCasino
{
    class Horses : Script
    {
        public enum horsesStatus
        {
            None = 0,
            ChoiceRaters,
            Racing,
            WinScreen,
        }
        public static horsesStatus GameStatus = horsesStatus.None;
        public static bool StateInWork = false;
        public static string Timer = null;
        public static bool HorsesWorking = true;
        public enum horsesTimers
        {
            ChoiceRaters = 30,
            Racing = 15,
            WinScreen = 5,
        }
        public static DateTime TimeStage;
        public static DateTime GameStart;
        /// <summary>
        /// Данные игрока
        /// </summary>
        public static Dictionary<ExtPlayer, BetData> PlayerData = new Dictionary<ExtPlayer, BetData>();
        public static Vector3 pos = new Vector3(1098.281, 256.4169, -52.36092);
        public static float radius = 250.0f;
        public static int curentScreen = 5;
        public static List<int> curentRandomHours = new List<int>();
        public static int WinHorse = -1;
        private static readonly Random random = new Random();
        private static readonly nLog Log = new nLog("NewCasino.Horses");

        public static void init()
        {
            Timer = Timers.StartTask(1000, () => GameInterval());
        }
        public static void GameInterval()
        {
            try
            {
                if (Timer == null) return;
                if (StateInWork == true) return; // Защита от того, что таймер пойдёт быстрее, чем отработает код
                switch (GameStatus)
                {
                    case horsesStatus.None:
                        StateInWork = true;
                        curentScreen = 0;
                        Trigger.ClientEventInRange(pos, radius, "client.horse.COUNTDOWN_SCREEN");
                        Trigger.ClientEventInRange(pos, radius, "client.horse.START_TIMER", (int)horsesTimers.ChoiceRaters);
                        TimeStage = DateTime.Now.AddSeconds((int)horsesTimers.ChoiceRaters);
                        GameStatus = horsesStatus.ChoiceRaters;
                        StateInWork = false;
                        break;
                    case horsesStatus.ChoiceRaters:
                        if (TimeStage > DateTime.Now) return;
                        StateInWork = true;
                        if (WinHorse == -1) WinHorse = random.Next(1, 7);
                        List<int> horseList = new List<int>() { 1, 2, 3, 4, 5, 6 };
                        horseList.Remove(WinHorse);
                        curentRandomHours = Shuffle(horseList);
                        curentRandomHours.Insert(0, WinHorse);
                        int seed = random.Next(1, 10);
                        GameStart = DateTime.Now.AddSeconds(3);
                        Trigger.ClientEventInRange(pos, radius, "client.horse.START_RACING", seed, curentRandomHours[0], curentRandomHours[1], curentRandomHours[2], curentRandomHours[3], curentRandomHours[4], curentRandomHours[5], 1);
                        curentScreen = 1;
                        TimeStage = DateTime.Now.AddSeconds((int)horsesTimers.Racing);
                        GameStatus = horsesStatus.Racing;
                        StateInWork = false;
                        break;
                    case horsesStatus.Racing:
                        if (TimeStage > DateTime.Now) return;
                        StateInWork = true;
                        curentScreen = 2;
                        List<string> gametags = new List<string>();
                        List<int> bets = new List<int>();
                        lock (PlayerData)
                        {
                            foreach (ExtPlayer foreachPlayer in PlayerData.Keys)
                            {
                                var foreachSessionData = foreachPlayer.GetSessionData();
                                if (foreachSessionData == null) continue;
                                var foreachCharacterData = foreachPlayer.GetCharacterData();
                                if (foreachCharacterData == null) continue;
                                BetData rData = PlayerData[foreachPlayer];
                                if (rData.Spot == WinHorse)
                                {
                                    int payment = rData.Bet + Convert.ToInt32(rData.Bet * 2);
                                    MoneySystem.Wallet.Change(foreachPlayer, payment);
                                    GameLog.CasinoHorsesLog(payment);
                                    GameLog.Money($"system", $"player({foreachCharacterData.UUID})", payment, $"HorseWin({rData.Spot})");
                                    Notify.Send(foreachPlayer, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouWinHorse, MoneySystem.Wallet.Format(payment), rData.Spot), 7000);
                                    bets.Add(payment);
                                }
                                else
                                {
                                    Notify.Send(foreachPlayer, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouLoseHorse, rData.Spot), 5000);
                                    bets.Add(-rData.Bet);
                                }
                                gametags.Add(LangFunc.GetText(LangType.Ru, DataName.Horse, foreachSessionData.Name, rData.Spot));
                            }
                        }
                        Trigger.ClientEventInRange(pos, radius, "client.horse.SHOW_RESULTS", JsonConvert.SerializeObject(gametags), JsonConvert.SerializeObject(bets));
                        TimeStage = DateTime.Now.AddSeconds((int)horsesTimers.WinScreen);
                        GameStatus = horsesStatus.WinScreen;
                        StateInWork = false;
                        break;
                    case horsesStatus.WinScreen:
                        if (TimeStage > DateTime.Now) return;
                        StateInWork = true;
                        TimeStage = DateTime.Now;
                        GameStatus = horsesStatus.None;
                        PlayerData = new Dictionary<ExtPlayer, BetData>();
                        WinHorse = -1;
                        StateInWork = false;
                        break;
                    default:
                        // Not supposed to end up here. 
                        break;

                }
            }
            catch (Exception e)
            {
                Log.Write($"GameInterval Exception: {e.ToString()}");
            }
        }


        public static List<T> Shuffle<T>(List<T> list)
        {
            try
            {
                int n = list.Count;
                while (n > 1)
                {
                    n--;
                    int k = random.Next(n + 1);
                    T value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
                return list;
            }
            catch (Exception e)
            {
                Log.Write($"Shuffle Exception: {e.ToString()}");
                return list;
            }
        }
        /// <summary>
        /// Начать игру
        /// </summary>
        [RemoteEvent("server.horse.SIT_SLOT")]
        private static void sit_slot(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (!FunctionsAccess.IsWorking("horse"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                if (!PlayerData.ContainsKey(player))
                {
                    sessionData.IsCasinoGame = "Horses";
                    Trigger.ClientEvent(player, "client.horse.SLOT", curentScreen);
                }
                else
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.HorseBetAlready, PlayerData[player].Spot), 6000);
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"sit_slot Exception: {e.ToString()}");
            }
        }

        /// <summary>
        /// Начать игру
        /// </summary>
        [RemoteEvent("server.horse.TEARS_SLOT")]
        private static void tears_slot(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                Trigger.ClientEvent(player, "client.horse.TEARS_SLOT");
                sessionData.IsCasinoGame = null;
            }
            catch (Exception e)
            {
                Log.Write($"tears_slot Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.horse.setBet")]
        public static void CMD_SelectHorse(ExtPlayer player, int horse, int money)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (curentScreen != 0)
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantDoStavka), 3000);
                    return;
                }
                if (!HorsesWorking)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                else if (UpdateData.CanIChange(player, money, true) != 255) return;
                PlayerData.Add(player, new BetData(money, horse));
                MoneySystem.Wallet.Change(player, -money);
                GameLog.CasinoHorsesLog(-money);
                GameLog.Money($"player({characterData.UUID})", $"system", money, $"HorseBet({horse})");
                BattlePass.Repository.UpdateReward(player, 55);

                if (money >= 500)
                {
                    qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage23, 2, isUpdateHud: true);
                    qMain.UpdateQuestsComplete(player, Zdobich.QuestName, (int) zdobich_quests.Stage23, true);
                }
                else
                {
                    qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage23, 1, isUpdateHud: true);
                }

                List<string> gametags = new List<string>();
                List<int> bets = new List<int>();
                lock (PlayerData)
                {
                    foreach (ExtPlayer foreachPlayer in PlayerData.Keys)
                    {
                        if (foreachPlayer.IsCharacterData())
                        {
                            gametags.Add($"{foreachPlayer.Name} - Конь #{PlayerData[foreachPlayer].Spot}");
                            bets.Add(PlayerData[foreachPlayer].Bet);
                        }
                    }
                }
                Trigger.ClientEventInRange(player.Position, 100f, "client.horse.SHOW_HORSE", JsonConvert.SerializeObject(gametags), horse, JsonConvert.SerializeObject(bets));
                Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BetOnHorse, horse), 3000);
                tears_slot(player);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_SelectHorse Exception: {e.ToString()}");
            }
        }
        
        [ServerEvent(Event.PlayerDeath)]
        public static void OnPlayerDeath(ExtPlayer player, ExtPlayer killer, uint reason)
        {
            try
            {
                if (!player.IsCharacterData() || !PlayerData.ContainsKey(player)) return;
                Disconnect(player, DisconnectionType.Timeout);
            }
            catch (Exception e)
            {
                Log.Write($"OnPlayerDeath Exception: {e.ToString()}");
            }
        }

        public static void Disconnect(ExtPlayer player, DisconnectionType type)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                if (!PlayerData.ContainsKey(player)) return;

                sessionData.IsCasinoGame = null;
                BetData sData = PlayerData[player];
                if (sData.Bet > 0 && GameStart >= DateTime.Now)
                {
                    MoneySystem.Wallet.Change(player, sData.Bet);
                    GameLog.CasinoHorsesLog(sData.Bet);
                    GameLog.Money($"system", $"player({characterData.UUID})", sData.Bet, $"HorseBetReturn");
                }
                PlayerData.Remove(player);
            }
            catch (Exception e)
            {
                Log.Write($"Disconnect Exception: {e.ToString()}");
            }
        }
    }
}
