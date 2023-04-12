using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using Localization;
using NeptuneEvo.Quests;

namespace NeptuneEvo.NewCasino
{
    class Roullete : Script
    {
        /// <summary>
        /// Максимальное кол-во ставок
        /// </summary>
        private static int MaxBetsLimit = 3;

        /// <summary>
        /// Время, которое дается на ставку (в данном случае - 45 секунд)
        /// </summary>
        private static int Timeout = 15;

        private static readonly nLog Log = new nLog("NewCasino.Roullete");

        public static bool RouletteWorking = true;

        /// <summary>
        /// Общие данные о столах
        /// </summary>
        public static List<Table> RouletteTables = new List<Table>
        {
            new Table(0), new Table(1), new Table(2), new Table(3), new Table(4), new Table(5)
        };

        /// <summary>
        /// Конверт из неправильных значений в правильные
        /// </summary>
        public static Dictionary<int, string> Roulette = new Dictionary<int, string> // Справа правильные цифры рулетки, слева то, что видит система
        {
            [1] = "00", // 00
            [2] = "27",
            [3] = "10",
            [4] = "25",
            [5] = "29",
            [6] = "12",
            [7] = "8",
            [8] = "19",
            [9] = "31",
            [10] = "18",
            [11] = "6",
            [12] = "21",
            [13] = "33",
            [14] = "16",
            [15] = "4",
            [16] = "23",
            [17] = "35",
            [18] = "14",
            [19] = "2",
            [20] = "0", // 0
            [21] = "28",
            [22] = "9",
            [23] = "26",
            [24] = "30",
            [25] = "11",
            [26] = "7",
            [27] = "20",
            [28] = "32",
            [29] = "17",
            [30] = "5",
            [31] = "22",
            [32] = "34",
            [33] = "15",
            [34] = "3",
            [35] = "24",
            [36] = "36",
            [37] = "13",
            [38] = "1"
        };


        public static Dictionary<int, float> coefficientData = new Dictionary<int, float>()
        {
            { 1, 36.0f },
            { 2, 18.0f },
            { 3, 12.0f },
            { 4, 8.0f },
            { 5, 7.0f },
            { 6, 6.0f },
            { 12, 3.0f },
            { 18, 2.0f },
        };

        /// <summary>
        /// Все возможные комбинации 
        /// </summary>
        public static List<List<string>> TableChipsOffsets = new List<List<string>>
        {
            new List<string> { "0" },//0
            new List<string> { "00" },//1 - 00
            new List<string> { "1" },//2
            new List<string> { "2" },//3
            new List<string> { "3" },//4
            new List<string> { "4" },//5
            new List<string> { "5" },//6
            new List<string> { "6" },//7
            new List<string> { "7" },//8
            new List<string> { "8" },//9
            new List<string> { "9" },//"10"
            new List<string> { "10" },//"11"
            new List<string> { "11" },//"12"
            new List<string> { "12" },//"13"
            new List<string> { "13" },//"14"
            new List<string> { "14" },//"15"
            new List<string> { "15" },//"16"
            new List<string> { "16" },//"17"
            new List<string> { "17" },//"18"
            new List<string> { "18" },//"19"
            new List<string> { "19" },//"20"
            new List<string> { "20" },//"21"
            new List<string> { "21" },//"22"
            new List<string> { "22" },//"23"
            new List<string> { "23" },//"24"
            new List<string> { "24" },//"25"
            new List<string> { "25" },//"26"
            new List<string> { "26" },//"27"
            new List<string> { "27" },//"28"
            new List<string> { "28" },//"29"
            new List<string> { "29" },//"30"
            new List<string> { "30" },//"31"
            new List<string> { "31" },//"32"
            new List<string> { "32" },//"33"
            new List<string> { "33" },//"34"
            new List<string> { "34" },//"35"
            new List<string> { "35" },//"36"
            new List<string> { "36" },//37
            new List<string> { "0", "00" },//38
            new List<string> { "1", "2" },
            new List<string> { "2", "3" },
            new List<string> { "4", "5" },
            new List<string> { "5", "6" },
            new List<string> { "7", "8" },
            new List<string> { "8", "9" },
            new List<string> { "10", "11" },
            new List<string> { "11", "12" },
            new List<string> { "13", "14" },
            new List<string> { "14", "15" },
            new List<string> { "16", "17" },
            new List<string> { "17", "18" },
            new List<string> { "19", "20" },
            new List<string> { "20", "21" },
            new List<string> { "22", "23" },
            new List<string> { "23", "24" },
            new List<string> { "25", "26" },
            new List<string> { "26", "27" },
            new List<string> { "28", "29" },
            new List<string> { "29", "30" },
            new List<string> { "31", "32" },
            new List<string> { "32", "33" },
            new List<string> { "34", "35" },
            new List<string> { "35", "36" },
            new List<string> { "1", "4" },
            new List<string> { "2", "5" },
            new List<string> { "3", "6" },
            new List<string> { "4", "7" },
            new List<string> { "5", "8" },
            new List<string> { "6", "9" },
            new List<string> { "7", "10" },
            new List<string> { "8", "11" },
            new List<string> { "9", "12" },
            new List<string> { "10", "13" },
            new List<string> { "11", "14" },
            new List<string> { "12", "15" },
            new List<string> { "13", "16" },
            new List<string> { "14", "17" },
            new List<string> { "15", "18" },
            new List<string> { "16", "19" },
            new List<string> { "17", "20" },
            new List<string> { "18", "21" },
            new List<string> { "19", "22" },
            new List<string> { "20", "23" },
            new List<string> { "21", "24" },
            new List<string> { "22", "25" },
            new List<string> { "23", "26" },
            new List<string> { "24", "27" },
            new List<string> { "25", "28" },
            new List<string> { "26", "29" },
            new List<string> { "27", "30" },
            new List<string> { "28", "31" },
            new List<string> { "29", "32" },
            new List<string> { "30", "33" },
            new List<string> { "31", "34" },
            new List<string> { "32", "35" },
            new List<string> { "33", "36" },
            new List<string> { "1", "2", "3" },
            new List<string> { "4", "5", "6" },
            new List<string> { "7", "8", "9" },
            new List<string> { "10", "11", "12" },
            new List<string> { "13", "14", "15" },
            new List<string> { "16", "17", "18" },
            new List<string> { "19", "20", "21" },
            new List<string> { "22", "23", "24" },
            new List<string> { "25", "26", "27" },
            new List<string> { "28", "29", "30" },
            new List<string> { "31", "32", "33" },
            new List<string> { "34", "35", "36" },
            new List<string> { "1", "2", "4", "5" },
            new List<string> { "2", "3", "5", "6" },
            new List<string> { "4", "5", "7", "8" },
            new List<string> { "5", "6", "8", "9" },
            new List<string> { "7", "8", "10", "11" },
            new List<string> { "8", "9", "11", "12" },
            new List<string> { "10", "11", "13", "14" },
            new List<string> { "11", "12", "14", "15" },
            new List<string> { "13", "14", "16", "17" },
            new List<string> { "14", "15", "17", "18" },
            new List<string> { "16", "17", "19", "20" },
            new List<string> { "17", "18", "20", "21" },
            new List<string> { "19", "20", "22", "23" },
            new List<string> { "20", "21", "23", "24" },
            new List<string> { "22", "23", "25", "26" },
            new List<string> { "23", "24", "26", "27" },
            new List<string> { "25", "26", "28", "29" },
            new List<string> { "26", "27", "29", "30" },
            new List<string> { "28", "29", "31", "32" },
            new List<string> { "29", "30", "32", "33" },
            new List<string> { "31", "32", "34", "35" },
            new List<string> { "32", "33", "35", "36" },
            new List<string> { "0", "00", "1", "2", "3" },
            new List<string> { "1", "2", "3", "4", "5", "6" },
            new List<string> { "4", "5", "6", "7", "8", "9" },
            new List<string> { "7", "8", "9", "10", "11", "12" },
            new List<string> { "10", "11", "12", "13", "14", "15" },
            new List<string> { "13", "14", "15", "16", "17", "18" },
            new List<string> { "16", "17", "18", "19", "20", "21" },
            new List<string> { "19", "20", "21", "22", "23", "24" },
            new List<string> { "22", "23", "24", "25", "26", "27" },
            new List<string> { "25", "26", "27", "28", "29", "30" },
            new List<string> { "28", "29", "30", "31", "32", "33" },
            new List<string> { "31", "32", "33", "34", "35", "36" },
            new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" }, // 143
            new List<string> { "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24" },
            new List<string> { "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36" },
            new List<string> { "1", "4", "7", "10", "13", "16", "19", "22", "25", "28", "31", "34" },
            new List<string> { "2", "5", "8", "11", "14", "17", "20", "23", "26", "29", "32", "35" },
            new List<string> { "3", "6", "9", "12", "15", "18", "21", "24", "27", "30", "33", "36" },
            new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18" },
            new List<string> { "2", "4", "6", "8", "10", "12", "14", "16", "18", "20", "22", "24", "26", "28", "30", "32", "34", "36" },
            new List<string> { "1", "3", "5", "7", "9", "12", "14", "16", "18", "19", "21", "23", "25", "27", "30", "32", "34", "36" },
            new List<string> { "2", "4", "6", "8", "10", "11", "13", "15", "17", "20", "22", "24", "26", "28", "29", "31", "33", "35" },
            new List<string> { "1", "3", "5", "7", "9", "11", "13", "15", "17", "19", "21", "23", "25", "27", "29", "31", "33", "35" },
            new List<string> { "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36" } //154
        };
        /// <summary>
        /// Данные игрока
        /// </summary>
        public static Dictionary<ExtPlayer, RoulettePlayerData> PlayerData = new Dictionary<ExtPlayer, RoulettePlayerData>();

        /// <summary>
        /// Старт игры для текущего стола
        /// </summary>
        /// <param name="index"></param>
        public static void StartGame(int index)
        {
            try
            {
                Table table = RouletteTables[index];
                if (table.Process) return;
                if (table.WaitTimeout == null)
                {
                    int timer = Timeout;
                    table.WaitTimeout = Timers.Start(1000, () =>
                    {
                        if (--timer > 0)
                        {
                            lock (table.Seats)
                            {
                                foreach (ExtPlayer foreachPlayer in table.Seats)
                                {
                                    if (foreachPlayer.IsCharacterData() && PlayerData.ContainsKey(foreachPlayer)) Trigger.ClientEvent(foreachPlayer, "client.roullete.timer", timer);
                                }
                            }
                        }
                        else ContinueGame(index);
                    });
                }
            }
            catch (Exception e)
            {
                Log.Write($"StartGame Exception: {e.ToString()}");
            }
        }

        /// <summary>
        /// Продолжает игру для текущего стола
        /// </summary>
        /// <param name="index"></param>
        public static void ContinueGame(int index)
        {
            try
            {
                if (RouletteTables.Count <= index) return;
                Table table = RouletteTables[index];
                Timers.Stop(table.WaitTimeout);
                table.WaitTimeout = null;
                table.WinNum = "-";
                /** Количество игроков, сидящих за столом */
                int current_players_on_table = 0;

                /** Количество ставок, которые находятся на столе */
                int current_players_bets_on_table = 0;

                lock (table.Seats)
                {
                    foreach (ExtPlayer foreachPlayer in table.Seats)
                    {
                        if (foreachPlayer.IsCharacterData() && PlayerData.ContainsKey(foreachPlayer))
                        {

                            RoulettePlayerData rData = PlayerData[foreachPlayer];

                            current_players_on_table++;

                            if (rData.AllBets.Count > 0)
                                current_players_bets_on_table += rData.AllBets.Count;

                        }
                    }
                }

                if (current_players_on_table == 0 || current_players_bets_on_table == 0)
                { // В случае, если игроков и ставок за столом нет
                    table.Win = 0;
                    table.Process = false;
                    lock (table.Seats)
                    {
                        foreach (ExtPlayer foreachPlayer in table.Seats)
                        {
                            if (foreachPlayer.IsCharacterData() && PlayerData.ContainsKey(foreachPlayer))
                            {
                                Trigger.ClientEvent(foreachPlayer, "client.roullete.timer", 0);
                            }
                        }
                    }
                    return;
                }

                if (current_players_on_table > 0 && current_players_bets_on_table > 0)
                { // В случае, если все гуд (и игроки сидят, и ставки стоят)
                    table.Process = true;

                    SendRouletteNotify(index, LangFunc.GetText(LangType.Ru, DataName.CantDoStavka));
                    if (table.Win == 0) table.Win = new Random().Next(1, 39);
                    while (!Roulette.ContainsKey(table.Win))
                    {
                        table.Win = new Random().Next(1, 39);
                    }
                    string bet_string = $"exit_{table.Win.ToString()}_";
                    for (int i = 0; i < TableChipsOffsets.Count; i++)
                    {
                        if (TableChipsOffsets[i].Contains(Roulette[table.Win]))
                        {
                            table.WinSpots.Add(i);
                        }
                    }
                    table.WinNum = Roulette[table.Win];
                    lock (table.Seats)
                    {
                        foreach (ExtPlayer foreachPlayer in table.Seats)
                        {
                            if (foreachPlayer.IsCharacterData())
                            {
                                if (PlayerData.ContainsKey(foreachPlayer)) PlayerData[foreachPlayer].Time = DateTime.Now.AddSeconds(5);
                                Trigger.ClientEvent(foreachPlayer, "client.roullete.START_GAME", bet_string);
                            }
                        }
                    }
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"ContinueGame Exception: {e.ToString()}");
            }
        }

        /// <summary>
        /// Удаляет все ставки игрока
        /// </summary>
        /// <param name="player"></param>
        public static void DestroyAllPlayerBets(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData() || !PlayerData.ContainsKey(player)) return;

                RoulettePlayerData rData = PlayerData[player];

                if (rData.FBetObject != null && rData.FBetObject.Exists) rData.FBetObject.Delete();
                rData.FBetObject = null;
                if (rData.SBetObject != null && rData.SBetObject.Exists) rData.SBetObject.Delete();
                rData.SBetObject = null;
                if (rData.TBetObject != null && rData.TBetObject.Exists) rData.TBetObject.Delete();
                rData.TBetObject = null;
            }
            catch (Exception e)
            {
                Log.Write($"DestroyAllPlayerBets Exception: {e.ToString()}");
            }
        }

        /// <summary>
        /// Отправить оповещение игрокам, находящимся за конкретным столом
        /// </summary>
        /// <param name="index"></param>
        /// <param name="message"></param>
        public static void SendRouletteNotify(int index, string message)
        {
            try
            {
                if (RouletteTables.Count <= index) return;
                Table table = RouletteTables[index];
                lock (table.Seats)
                {
                    foreach (ExtPlayer foreachPlayer in table.Seats)
                    {
                        if (foreachPlayer.IsCharacterData() && PlayerData.ContainsKey(foreachPlayer)) Notify.Send(foreachPlayer, NotifyType.Warning, NotifyPosition.BottomCenter, message, 3000);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"SendRouletteNotify Exception: {e.ToString()}");
            }
        }

        /// <summary>
        /// Занять место за столом
        /// </summary>
        /// <param name="player"></param>
        /// <param name="table"></param>
        /// <param name="place"></param>
        [RemoteEvent("server.roullete.CHARACTER_OCCUPY_PLACE")]
        public static void OnCharacterOccupyPlace(ExtPlayer player, int tableIndex, int placeID)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData() || PlayerData.ContainsKey(player)) return;
                if (!FunctionsAccess.IsWorking("roullete"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                if (RouletteTables.Count <= tableIndex) return;
                Table table = RouletteTables[tableIndex];

                if (table.Seats[placeID] != null)
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlaceOwned), 3000);
                    return;
                }
                PlayerData.Add(player, new RoulettePlayerData(tableIndex));
                sessionData.IsCasinoGame = "Roulette";

                Trigger.StopAnimation(player);

                Trigger.ClientEvent(player, "client.roullete.CHARACTER_OCCUPY_PLACE", tableIndex, placeID, table.Process ? 0 : 1);

                Trigger.PlayAnimation(player, "anim_casino_b@amb@casino@games@shared@player@", "sit_enter_left_side", 3);
                // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "sitting");

                Timers.StartOnce(4000, () =>
                {
                    if (!player.IsCharacterData() || !PlayerData.ContainsKey(player))
                        return;
                    Trigger.PlayAnimation(player, "anim_casino_b@amb@casino@games@shared@player@", "idle_a", 3);
                    // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "sitting1");
                });

                table.Seats[placeID] = player;
            }
            catch (Exception e)
            {
                Log.Write($"OnCharacterOccupyPlace Exception: {e.ToString()}");
            }
        }

        /// <summary>
        /// Выйти из-за стола
        /// </summary>
        /// <param name="player"></param>
        [RemoteEvent("server.roullete.CHARACTER_LEAVE_PLACE")]
        public static void OnCharacterLeavePlace(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData() || !PlayerData.ContainsKey(player)) return;
                RoulettePlayerData rData = PlayerData[player];

                if (RouletteTables.Count <= rData.SelectedTable) return;
                Table table = RouletteTables[rData.SelectedTable];

                if (table.Process && rData.AllBets.Count > 0)
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantBetByGame), 5000);
                    return;
                }

                if (rData.AllBets.Count > 0)
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NowDeleteStavkas), 3000);
                    return;
                }
                PlayerData.Remove(player);

                int seat_index = table.Seats.IndexOf(player);
                if (seat_index != -1) table.Seats[seat_index] = null;
                sessionData.IsCasinoGame = null;

                Trigger.PlayAnimation(player, "anim_casino_b@amb@casino@games@shared@player@", "sit_exit_left", 3);
                // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "sitting2");
                Timers.StartOnce(4000, () =>
                {
                    if (!player.IsCharacterData())
                        return;
                    Trigger.ClientEvent(player, "client.roullete.CHARACTER_LEAVE_PLACE");
                    Trigger.StopAnimation(player);
                });
            }
            catch (Exception e)
            {
                Log.Write($"OnCharacterLeavePlace Exception: {e.ToString()}");
            }
        }

        /// <summary>
        /// Создать новую ставку
        /// </summary>
        /// <param name="player"></param>
        /// <param name="bet"></param>
        /// <param name="spotNumber"></param>
        /// <param name="posJSON"></param>
        [RemoteEvent("server.roullete.CREATE_BET")]
        public static void OnCreateBet(ExtPlayer player, int bet, int spot, float x, float y, float z)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!PlayerData.ContainsKey(player) || bet < 20) return;
                else if (UpdateData.CanIChange(player, bet, true) != 255) return;
                if (!RouletteWorking)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                RoulettePlayerData rData = PlayerData[player];

                if (rData.AllBets.Count >= MaxBetsLimit)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OneGameMaxBets, MaxBetsLimit), 3000);
                    return;
                }
                if (RouletteTables.Count <= rData.SelectedTable) return;
                Table table = RouletteTables[rData.SelectedTable];

                if (table.Process)
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WaitUntilGameEnds), 3000);
                    return;
                }

                for (int i = 0; i < rData.AllBets.Count; i++)
                {
                    if (rData.AllBets[i].Spot == spot) return;
                }
                int casenum = rData.AllBets.Count;
                switch (rData.AllBets.Count)
                {
                    case 0:
                        rData.FBetObject = NAPI.Object.CreateObject(NAPI.Util.GetHashKey("vw_prop_chip_100dollar_x1"), new Vector3(x, y, z), new Vector3());
                        break;
                    case 1:
                        rData.SBetObject = NAPI.Object.CreateObject(NAPI.Util.GetHashKey("vw_prop_chip_100dollar_x1"), new Vector3(x, y, z), new Vector3());
                        break;
                    case 2:
                        rData.TBetObject = NAPI.Object.CreateObject(NAPI.Util.GetHashKey("vw_prop_chip_100dollar_x1"), new Vector3(x, y, z), new Vector3());
                        break;

                    default: break;
                }
                MoneySystem.Wallet.Change(player, -bet);
                GameLog.CasinoRouletteLog(-bet);
                GameLog.Money($"player({characterData.UUID})", $"system", bet, $"RouletteBet({spot}, {casenum})");       
                BattlePass.Repository.UpdateReward(player, 55);         
                if (bet >= 500)
                {
                    qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage23, 2, isUpdateHud: true);
                    qMain.UpdateQuestsComplete(player, Zdobich.QuestName, (int) zdobich_quests.Stage23, true);
                }
                else
                {
                    qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage23, 1, isUpdateHud: true);
                }
                rData.AllBets.Add(new BetData(bet, spot));
                Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouBet, bet), 3000);
                StartGame(rData.SelectedTable);
            }
            catch (Exception e)
            {
                Log.Write($"OnCreateBet Exception: {e.ToString()}");
            }
        }

        /// <summary>
        /// Удаляет последнюю ставку игрока
        /// </summary>
        /// <param name="player"></param>
        [RemoteEvent("server.roullete.DESTROY_LAST_BET")]
        public static void OnDestroyLastBet(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!PlayerData.ContainsKey(player)) return;
                RoulettePlayerData rData = PlayerData[player];
                int casenum = rData.AllBets.Count;
                switch (rData.AllBets.Count)
                {
                    case 0:
                        return;
                    case 1:
                        if (rData.FBetObject != null && rData.FBetObject.Exists) rData.FBetObject.Delete();
                        rData.FBetObject = null;
                        break;
                    case 2:
                        if (rData.SBetObject != null && rData.SBetObject.Exists) rData.SBetObject.Delete();
                        rData.SBetObject = null;
                        break;
                    case 3:
                        if (rData.TBetObject != null && rData.TBetObject.Exists) rData.TBetObject.Delete();
                        rData.TBetObject = null;
                        break;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
                int mybet = PlayerData[player].AllBets[rData.AllBets.Count - 1].Bet;
                MoneySystem.Wallet.Change(player, mybet);
                GameLog.CasinoRouletteLog(mybet);
                GameLog.Money($"system", $"player({characterData.UUID})", mybet, $"RouletteBetReturn({casenum})");
                if (rData.AllBets.Any()) rData.AllBets.RemoveAt(rData.AllBets.Count - 1);
            }
            catch (Exception e)
            {
                Log.Write($"OnDestroyLastBet Exception: {e.ToString()}");
            }
        }

        /// <summary>
        /// Очищает стол после игры
        /// </summary>
        /// <param name="player"></param>
        [RemoteEvent("server.roullete.CLEAR_TABLE")]
        public static void OnClearTable(ExtPlayer player)
        {
            try
            {
                if (!PlayerData.ContainsKey(player)) return;
                int index = PlayerData[player].SelectedTable;
                if (RouletteTables.Count <= index) return;
                Table table = RouletteTables[index];

                if (!table.Process) return;
                table.Win = 0;
                table.Process = false;
                lock (table.Seats)
                {
                    foreach (ExtPlayer foreachPlayer in table.Seats)
                    {
                        var foreachCharacterData = foreachPlayer.GetCharacterData();
                        if (foreachCharacterData != null && PlayerData.ContainsKey(foreachPlayer))
                        {

                            RoulettePlayerData targetrData = PlayerData[foreachPlayer];
                            if (targetrData.AllBets.Count > 0)
                            {
                                targetrData.WinMoney = 0;

                                List<int> WinSpots = new List<int>();

                                for (int x = 0; x < targetrData.AllBets.Count; x++)
                                {
                                    BetData target_data = targetrData.AllBets[x];

                                    if (table.WinSpots.Contains(target_data.Spot) && !WinSpots.Contains(target_data.Spot))
                                    {
                                        WinSpots.Add(target_data.Spot);
                                        targetrData.WinMoney += Convert.ToInt32(target_data.Bet * (coefficientData.ContainsKey(TableChipsOffsets[target_data.Spot].Count) ? coefficientData[TableChipsOffsets[target_data.Spot].Count] : 1.25f));
                                    }
                                }

                                string message = $"Выпало [{table.WinNum}]. Вы {(targetrData.WinMoney > 0 ? $"выиграли ${MoneySystem.Wallet.Format(targetrData.WinMoney)}" : "проиграли.")}"; // harddd

                                Notify.Send(foreachPlayer, NotifyType.Warning, NotifyPosition.BottomCenter, message, 3000);

                                Trigger.ClientEvent(foreachPlayer, "client.roullete.TOGGLE_BET", targetrData.WinMoney);


                                if (targetrData.WinMoney > 0)
                                {
                                    GameLog.CasinoRouletteLog(targetrData.WinMoney);
                                    MoneySystem.Wallet.Change(foreachPlayer, targetrData.WinMoney);
                                    GameLog.Money($"system", $"player({foreachCharacterData.UUID})", targetrData.WinMoney, $"RouletteWin");
                                }
                                else
                                {
                                    GameLog.Money($"system", $"player({foreachCharacterData.UUID})", 0, $"RouletteWin");
                                }
                                DestroyAllPlayerBets(foreachPlayer);
                                targetrData.AllBets = new List<BetData>();
                            }
                            else if (foreachPlayer.IsCharacterData())
                            {
                                Trigger.ClientEvent(foreachPlayer, "client.roullete.TOGGLE_BET", 0);
                            }
                        }
                    }
                }

                table.WinSpots = new List<int>();
            }
            catch (Exception e)
            {
                Log.Write($"OnClearTable Exception: {e.ToString()}");
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
                RoulettePlayerData rData = PlayerData[player];

                DestroyAllPlayerBets(player);

                PlayerData.Remove(player);
                sessionData.IsCasinoGame = null;


                if (RouletteTables.Count <= rData.SelectedTable) return;

                Table table = RouletteTables[rData.SelectedTable];
                int money = 0;
                for (int x = 0; x < rData.AllBets.Count; x++) money += rData.AllBets[x].Bet;
                if (money > 0 && (!table.Process || rData.Time >= DateTime.Now))
                {
                    MoneySystem.Wallet.Change(player, money);
                    GameLog.Money($"system", $"player({characterData.UUID})", money, $"RouletteBetsReturn");
                }

                int seat_index = table.Seats.IndexOf(player);
                if (seat_index != -1) table.Seats[seat_index] = null;

                if (table.Process)
                {
                    bool gameEnd = false;
                    lock (table.Seats)
                    {
                        foreach (ExtPlayer foreachPlayer in table.Seats)
                        {
                            if (foreachPlayer.IsCharacterData() && PlayerData.ContainsKey(foreachPlayer))
                            {
                                gameEnd = true;
                                break;
                            }
                        }
                    }
                    if (!gameEnd)
                    {
                        table.Win = 0;
                        table.Process = false;
                        table.WinSpots = new List<int>();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"Disconnect Exception: {e.ToString()}");
            }
        }
    }
}
