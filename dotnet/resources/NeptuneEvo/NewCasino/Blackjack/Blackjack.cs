using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Chars;
using Redage.SDK;
using System;
using System.Collections.Generic;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using System.Linq;
using Localization;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Quests;

namespace NeptuneEvo.NewCasino
{
    class Blackjack : Script
    {
        public class BJTableData
        {
            public string Timer { get; set; } = null;
            public byte State { get; set; } = 0;
            public byte setState { get; set; } = 0;
            public byte AdditionalState { get; set; } = 0;
            public int Value { get; set; } = 0;
            public int SlotIDReceive { get; set; } = 0;
            public ExtPlayer DealerFocus { get; set; } = null;
            public List<ExtPlayer> DealerFocused { get; set; } = new List<ExtPlayer>();
            public List<ExtPlayer> CardsGiven { get; set; } = new List<ExtPlayer>();
            public bool StateInWork { get; set; } = false;
            public string CardInHand { get; set; } = "";
            public string CardSplitInHand { get; set; } = "";
            public int Count { get; set; } = 0;
            public int CountSplit { get; set; } = 0;
            public byte Time { get; set; } = 15;
        };
        private static readonly byte Tables = 8;

        private static readonly nLog Log = new nLog("NewCasino.Blackjack");
        public static List<BJTableData> BlackjackData = new List<BJTableData>();
        public static bool BlackJackWorking = true;
        public static List<string> DefaultDeck = new List<string>();
        public static Dictionary<int, List<string>> PlayerHand = new Dictionary<int, List<string>>();

        public static Dictionary<int, BlackjackDealerInfo> DealerData = new Dictionary<int, BlackjackDealerInfo>();
        public static Dictionary<ExtPlayer, BlackjackPlayerInfo> PlayerData = new Dictionary<ExtPlayer, BlackjackPlayerInfo>();
        
        public static void Init()
        {
            try
            {
                for (byte i = 0; i != Tables; i++) BlackjackData.Add(new BJTableData());
                string[] ranks = new string[13] { "02", "03", "04", "05", "06", "07", "08", "09", "10", "jack", "queen", "king", "ace" };
                string[] suits = new string[4] { "spd", "hrt", "dia", "club" };
                List<string> _List = new List<string>();
                foreach (string rank in ranks)
                {
                    foreach (string suit in suits)
                    {
                        _List.Add($"{suit}_{rank}");
                    }
                }
                DefaultDeck = _List; // Дабы не пересоздавать Дэку каждую новую игру, так как она всё равно каждый раз одна
            }
            catch (Exception e)
            {
                Log.Write($"StartWork Exception: {e.ToString()}");
            }
        }
        public static void GetDeck(int index)
        {
            try
            {
                if (!DealerData.ContainsKey(index)) return;
                if (DefaultDeck.Count == 0)
                {
                    string[] ranks = new string[13] { "02", "03", "04", "05", "06", "07", "08", "09", "10", "jack", "queen", "king", "ace" };
                    string[] suits = new string[4] { "spd", "hrt", "dia", "club" };
                    List<string> _List = new List<string>();
                    foreach (string rank in ranks)
                    {
                        foreach (string suit in suits)
                        {
                            _List.Add($"{suit}_{rank}");
                        }
                    }
                    DefaultDeck = _List; // Если вдруг что-то пойдёт не так, то мы перестрахуемся и тут еще раз пропишем DefaultDeck
                }
                DealerData[index].Cards = Shuffle(new List<string>(DefaultDeck));
            }
            catch (Exception e)
            {
                Log.Write($"GetDeck Exception: {e.ToString()}");
            }
        }
        public static List<T> Shuffle<T>(List<T> list)
        {
            try
            {
                int n = list.Count;
                Random rand = new Random();
                while (n > 1)
                {
                    n--;
                    int k = rand.Next(n + 1);
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
        private static string takeCard(int index)
        {
            try
            {
                if (!DealerData.ContainsKey(index)) return null;
                List<string> List = DealerData[index].Cards;

                string card = List[0];
                List.RemoveAt(0);

                DealerData[index].Cards = List;
                return card;
            }
            catch (Exception e)
            {
                Log.Write($"takeCard Exception: {e.ToString()}");
                return null;
            }
        }
        private static int CardValue(string card)
        {
            try
            {
                int rank = 10;
                for (int i = 2; i <= 11; i++)
                {
                    if (card.IndexOf($"{i}") != -1)
                    {

                        rank = i;
                        break;
                    }
                }
                if (card.IndexOf("ace") != -1) rank = 11;
                return rank;
            }
            catch (Exception e)
            {
                Log.Write($"CardValue Exception: {e.ToString()}");
                return 10;
            }
        }

        private static int handValue(List<string> List)
        {
            try
            {
                int tmpValue = 0;
                int numAces = 0;

                foreach (string v in List)
                {
                    tmpValue += CardValue(v);
                }

                foreach (string v in List)
                {
                    if (v.IndexOf("ace") != -1) numAces++;
                }
                if (tmpValue > 21)
                {
                    for (int i = 0; i < numAces; i++) tmpValue = tmpValue - 10;
                }
                return tmpValue;
            }
            catch (Exception e)
            {
                Log.Write($"handValue Exception: {e.ToString()}");
                return 0;
            }
        }

        private static int AddDealerHand(int index, string card)
        {
            try
            {
                if (!DealerData.ContainsKey(index)) return 0;
                DealerData[index].Hand.Add(card);
                return DealerData[index].Hand.Count;
            }
            catch (Exception e)
            {
                Log.Write($"AddDealerHand Exception: {e.ToString()}");
                return 0;
            }
        }
        private static int DealerHandValue(int index)
        {
            try
            {
                if (!DealerData.ContainsKey(index)) return 0;
                return handValue(DealerData[index].Hand);
            }
            catch (Exception e)
            {
                Log.Write($"DealerHandValue Exception: {e.ToString()}");
                return 0;
            }
        }
        private static int AddPlayerHand(ExtPlayer player, string card)
        {
            try
            {
                if (!player.IsCharacterData()) return 0;
                else if (!PlayerData.ContainsKey(player)) return 0;
                PlayerData[player].Hand.Add(card);
                return PlayerData[player].Hand.Count;
            }
            catch (Exception e)
            {
                Log.Write($"AddPlayerHand Exception: {e.ToString()}");
                return 0;
            }
        }
        private static int AddPlayerSplitHand(ExtPlayer player, string card)
        {
            try
            {
                if (!player.IsCharacterData()) return 0;
                else if (!PlayerData.ContainsKey(player)) return 0;
                PlayerData[player].SplitHand.Add(card);
                return PlayerData[player].SplitHand.Count;
            }
            catch (Exception e)
            {
                Log.Write($"AddPlayerSplitHand Exception: {e.ToString()}");
                return 0;
            }
        }
        private static int PlayerHandValue(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return 0;
                else if (!PlayerData.ContainsKey(player)) return 0;
                return handValue(PlayerData[player].Hand);
            }
            catch (Exception e)
            {
                Log.Write($"PlayerHandValue Exception: {e.ToString()}");
                return 0;
            }
        }
        private static int PlayerSplitHandValue(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return 0;
                else if (!PlayerData.ContainsKey(player)) return 0;
                return handValue(PlayerData[player].SplitHand);
            }
            catch (Exception e)
            {
                Log.Write($"PlayerSplitHandValue Exception: {e.ToString()}");
                return 0;
            }
        }
        [RemoteEvent("server.blackjack.character_occupy_place")]
        public static void BlackjackOpen(ExtPlayer player, int Index, int SlotId, float PosX, float PosY, float PosZ, float Rotation)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!FunctionsAccess.IsWorking("blackjack"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                if (!DealerData.ContainsKey(Index)) DealerData.Add(Index, new BlackjackDealerInfo());
                if (!characterData.InCasino || !characterData.IsAlive || sessionData.CuffedData.Cuffed || DealerData[Index].Players.Contains(player)) return;
                if (characterData.Money < 500)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Table500Bucks), 3000);
                    return;
                }
                if (DealerData[Index].Chairs.Contains(SlotId))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlaceOwned), 3000);
                    return;
                }
                qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage23, 2, isUpdateHud: true);
                qMain.UpdateQuestsComplete(player, Zdobich.QuestName, (int) zdobich_quests.Stage23, true);
                DealerData[Index].Chairs.Add(SlotId);
                DealerData[Index].Players.Add(player);
                sessionData.IsCasinoGame = "BlackJack";
                player.Position = new Vector3(PosX, PosY, PosZ);
                player.Rotation = new Vector3(0, 0, Rotation);
                Trigger.ClientEventInRange(new Vector3(PosX, PosY, PosZ), 250f, "setClientRotation", player.Value, Rotation);
                Trigger.PlayAnimation(player, "anim_casino_b@amb@casino@games@shared@player@", "sit_enter_left", 3);
                // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "casino1");
                Trigger.ClientEvent(player, "client.blackjack.character_occupy_place", Index, SlotId, !DealerData[Index].GameRunning ? 1 : 0, 0);
                Timers.StartOnce(4000, () =>
                {
                    if (!player.IsCharacterData())
                    {
                        if (DealerData[Index].Chairs.Contains(SlotId)) DealerData[Index].Chairs.Remove(SlotId);
                        if (DealerData[Index].Players.Contains(player)) DealerData[Index].Players.Remove(player);
                        return;
                    }
                    Trigger.PlayAnimation(player, "anim_casino_b@amb@casino@games@shared@player@", "idle_a", 3);
                    // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "casino2");
                    Trigger.ClientEvent(player, "client.blackjack.PlayDealerSpeech", Index, "MINIGAME_DEALER_GREET");
                    if (PlayerData.ContainsKey(player)) PlayerData.Remove(player);
                    PlayerData.Add(player, new BlackjackPlayerInfo(Index, SlotId));
                });
            }
            catch (Exception e)
            {
                Log.Write($"BlackjackOpen Exception: {e.ToString()}");
            }
        }
        private static void PlayDealerAnim(int Index, Vector3 Position, string animDict, string anim)
        {
            try
            {
                Trigger.ClientEventInRange(Position, 250f, "client.blackjack.PlayDealerAnim", Index, animDict, anim);
            }
            catch (Exception e)
            {
                Log.Write($"PlayDealerAnim Exception: {e.ToString()}");
            }
        }
        private static void PlayDealerSpeech(int Index, Vector3 Position, string speech)
        {
            try
            {
                Trigger.ClientEventInRange(Position, 250f, "client.blackjack.PlayDealerSpeech", Index, speech);
            }
            catch (Exception e)
            {
                Log.Write($"PlayDealerSpeech Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.blackjack.setBet")]
        public static void SetBet(ExtPlayer player, int money)
        {

            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!PlayerData.ContainsKey(player) || money < 50) return;
                else if (UpdateData.CanIChange(player, money, true) != 255) return;
                if (!BlackJackWorking)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                BlackjackPlayerInfo bInfo = PlayerData[player];
                if (DealerData[bInfo.Index].GameRunning)
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantBetByGame), 5000);
                    return;
                }
                Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BetSuc), 1000);
                MoneySystem.Wallet.Change(player, -money);
                GameLog.CasinoBJLog(-money);
                GameLog.Money($"player({characterData.UUID})", $"system", money, $"BJBet");
                BattlePass.Repository.UpdateReward(player, 55);
                bInfo.Join = true;
                bInfo.Rate = money;
                Trigger.ClientEventInRange(player.Position, 250f, "client.blackjack.PlaceBetChip", bInfo.Index, bInfo.SlotId, money, false, false);
                Trigger.PlayAnimation(player, "anim_casino_b@amb@casino@games@blackjack@player", "place_bet_small", 3);
                // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "casinobet");
                Timers.StartOnce(1000, () =>
                {
                    if (player.IsCharacterData()) Trigger.PlayAnimation(player, "anim_casino_b@amb@casino@games@shared@player@", $"idle_var_01", 3);
                    // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "casinobet1");
                });
                if (BlackjackData[bInfo.Index].Timer == null)
                {
                    BlackjackData[bInfo.Index] = new BJTableData(); // Полная очистка стола
                    Vector3 mypos = player.Position;
                    BlackjackData[bInfo.Index].Timer = Timers.Start(1000, () => {
                        BlackJackGame(bInfo.Index, mypos);
                    });
                }
            }
            catch (Exception e)
            {
                Log.Write($"SetBet Exception: {e.ToString()}");
            }
        }

        [ServerEvent(Event.PlayerDeath)]
        public void OnPlayerDeath(ExtPlayer player, ExtPlayer killer, uint reason)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                BlackjackLeave(player, false);
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
                if (!player.IsCharacterData()) return;
                BlackjackLeave(player, true);
            }
            catch (Exception e)
            {
                Log.Write($"Disconnect Exception: {e.ToString()}");
            }
        }

        private static void BlackJackGame(int Index, Vector3 Position)
        {
            try
            {
                if (BlackjackData.Count <= Index) return;
                BJTableData Table = BlackjackData[Index];
                if (Table.StateInWork) return; // Защита от того, что таймер пойдёт быстрее, чем отработает код

                switch (Table.State)
                {
                    case 0: // Время после первой ставки и до начала раздачи карт
                        Table.StateInWork = true;
                        if (--Table.Time > 0)
                        {
                            lock (DealerData[Index].Players)
                            {
                                foreach (ExtPlayer foreachPlayer in DealerData[Index].Players)
                                {
                                    if (!foreachPlayer.IsCharacterData()) continue;
                                    if (PlayerData.ContainsKey(foreachPlayer) && PlayerData[foreachPlayer].Join)
                                    {
                                        Trigger.ClientEvent(foreachPlayer, "client.blackjack.SyncTimer", Table.Time);
                                        if (Table.Time == 1)
                                        {
                                            Trigger.ClientEvent(foreachPlayer, "client.blackjack.ExitBtn", 0);
                                            Trigger.ClientEvent(foreachPlayer, "client.blackjack.RetrieveCards", Index, 0);
                                        }
                                    }
                                }
                            }
                        }
                        else if (Table.Time == 0)
                        {
                            DealerData[Index].GamePlayers = new List<ExtPlayer>();
                            lock (DealerData[Index].Players)
                            {
                                foreach (ExtPlayer foreachPlayer in DealerData[Index].Players)
                                {
                                    if (!foreachPlayer.IsCharacterData()) continue;
                                    if (PlayerData.ContainsKey(foreachPlayer))
                                    {
                                        Trigger.ClientEvent(foreachPlayer, "client.blackjack.betWin", 0);
                                        Trigger.ClientEvent(foreachPlayer, "client.blackjack.SyncTimer", 0);
                                        if (PlayerData[foreachPlayer].Join && !DealerData[Index].GamePlayers.Contains(foreachPlayer)) DealerData[Index].GamePlayers.Add(foreachPlayer);
                                    }
                                }
                            }
                            if (DealerData[Index].GamePlayers.Count == 0) // Если вдруг игроков не оказалось - отключаем таймер стола
                            {
                                DealerData[Index].Hand = new List<string>();
                                DealerData[Index].Cards = new List<string>();
                                DealerData[Index].GameRunning = false;
                                BlackjackData[Index].DealerFocus = null;
                                if (BlackjackData[Index].Timer != null)
                                {
                                    Timers.Stop(BlackjackData[Index].Timer);
                                    BlackjackData[Index].Timer = null;
                                }
                                Table.StateInWork = false;
                                return;
                            }
                            Table.AdditionalState = 0;
                            Table.State = 1;
                            Table.Time = 0;
                            DealerData[Index].GameRunning = true;
                        }
                        Table.StateInWork = false;
                        break;
                    case 1: // Раздача первых карт всем игрокам за столом
                        Table.StateInWork = true;
                        if (Table.Time > 0) Table.Time--;
                        else if (Table.Time == 0)
                        {
                            switch (Table.AdditionalState)
                            {
                                case 0: // Дилер получает первую карту
                                    GetDeck(Index);
                                    Table.CardInHand = takeCard(Index);
                                    Table.Count = AddDealerHand(Index, Table.CardInHand);
                                    Trigger.ClientEventInRange(Position, 250f, "client.blackjack.GiveCard", Index, 0, Table.Count - 1, Table.CardInHand, (bool)(Table.Count == 1));
                                    PlayDealerAnim(Index, Position, "anim_casino_b@amb@casino@games@blackjack@dealer", "deal_card_self");
                                    Table.Time = 1;
                                    Table.AdditionalState = 1;
                                    break;
                                default: // Дилер раздаёт игрокам первую карту
                                    bool givecard = false;
                                    lock (DealerData[Index].GamePlayers)
                                    {
                                        foreach (ExtPlayer foreachPlayer in DealerData[Index].GamePlayers)
                                        {
                                            if (foreachPlayer.IsCharacterData() && PlayerData.ContainsKey(foreachPlayer) && !Table.CardsGiven.Contains(foreachPlayer))
                                            {
                                                givecard = true;
                                                Table.CardsGiven.Add(foreachPlayer);
                                                BlackjackPlayerInfo bInfo = PlayerData[foreachPlayer];
                                                Table.CardInHand = takeCard(Index);
                                                Table.Count = AddPlayerHand(foreachPlayer, Table.CardInHand);
                                                Trigger.ClientEventInRange(Position, 250f, "client.blackjack.GiveCard", Index, bInfo.SlotId, Table.Count - 1, Table.CardInHand);
                                                PlayDealerAnim(Index, Position, "anim_casino_b@amb@casino@games@blackjack@dealer", $"deal_card_player_0{bInfo.SlotId}");
                                                PlayDealerSpeech(Index, Position, $"MINIGAME_BJACK_DEALER_{PlayerHandValue(foreachPlayer)}");
                                                Table.Time = 1;
                                                break;
                                            }
                                        }
                                    }
                                    if (!givecard) // Если выдавать первую карту больше некому, то переходим к раздаче второй карты.
                                    {
                                        Table.CardsGiven = new List<ExtPlayer>();
                                        Table.State = 2;
                                        Table.Time = 0;
                                        Table.AdditionalState = 0;
                                    }
                                    break;
                            }
                        }
                        Table.StateInWork = false;
                        break;
                    case 2: // Раздача вторых карт всем игрокам за столом
                        Table.StateInWork = true;
                        if (Table.Time > 0) Table.Time--;
                        else if (Table.Time == 0)
                        {
                            switch (Table.AdditionalState)
                            {
                                case 0: // Дилер получает вторую карту
                                    Table.CardInHand = takeCard(Index);
                                    Table.Count = AddDealerHand(Index, Table.CardInHand);
                                    Trigger.ClientEventInRange(Position, 250f, "client.blackjack.GiveCard", Index, 0, Table.Count - 1, Table.CardInHand, (bool)(Table.Count == 1));
                                    PlayDealerAnim(Index, Position, "anim_casino_b@amb@casino@games@blackjack@dealer", "deal_card_self_second_card");
                                    if (Table.Count > 1) PlayDealerSpeech(Index, Position, $"MINIGAME_BJACK_DEALER_{CardValue(Table.CardInHand)}");
                                    Table.Time = 1;
                                    Table.AdditionalState = 1;
                                    break;
                                default: // Дилер раздаёт игрокам вторую карту
                                    bool givecard = false;
                                    lock (DealerData[Index].GamePlayers)
                                    {
                                        foreach (ExtPlayer foreachPlayer in DealerData[Index].GamePlayers)
                                        {
                                            if (foreachPlayer.IsCharacterData() && PlayerData.ContainsKey(foreachPlayer) && !Table.CardsGiven.Contains(foreachPlayer))
                                            {
                                                givecard = true;
                                                Table.CardsGiven.Add(foreachPlayer);
                                                BlackjackPlayerInfo bInfo = PlayerData[foreachPlayer];
                                                Table.CardInHand = takeCard(Index);
                                                Table.Count = AddPlayerHand(foreachPlayer, Table.CardInHand);
                                                Trigger.ClientEventInRange(Position, 250f, "client.blackjack.GiveCard", Index, bInfo.SlotId, Table.Count - 1, Table.CardInHand);
                                                PlayDealerAnim(Index, Position, "anim_casino_b@amb@casino@games@blackjack@dealer", $"deal_card_player_0{bInfo.SlotId}");
                                                PlayDealerSpeech(Index, Position, $"MINIGAME_BJACK_DEALER_{PlayerHandValue(foreachPlayer)}");
                                                Table.Time = 1;
                                                break;
                                            }
                                        }
                                    }
                                    if (!givecard) // После раздачи всех начальных карт переходим к следующему этапу
                                    {
                                        Table.CardsGiven = new List<ExtPlayer>();
                                        Table.State = 3;
                                        Table.AdditionalState = 0;
                                    }
                                    break;
                            }
                        }
                        Table.StateInWork = false;
                        break;
                    case 3: // Небольшой интерактив, если у дилера карта 10 или 11, то он проверяет свою первую карту
                        Table.StateInWork = true;
                        PlayDealerAnim(Index, Position, "anim_casino_b@amb@casino@games@blackjack@dealer", "check_card");
                        Trigger.ClientEventInRange(Position, 250f, "client.blackjack.DealerTurnOverCard", Index, false);
                        Table.Time = 1;
                        Table.State = 4;
                        Table.StateInWork = false;
                        break;
                    case 4: // Начинаем взаимодействие с игроками, фокусируемся на одном из них
                        Table.StateInWork = true;
                        if (Table.Time > 0) Table.Time--;
                        else if (Table.Time == 0)
                        {
                            Table.DealerFocus = null;
                            ExtPlayer target = null;
                            lock (DealerData[Index].GamePlayers)
                            {
                                foreach (ExtPlayer foreachPlayer in DealerData[Index].GamePlayers)
                                {
                                    if (foreachPlayer.IsCharacterData() && PlayerData.ContainsKey(foreachPlayer) && !Table.DealerFocused.Contains(foreachPlayer)) // Не опрашиваем игроков, которые уже были опрошены
                                    {
                                        if (PlayerHandValue(foreachPlayer) < 21) // Опрашиваем всех игроков, которые не имеют 21 на столе
                                        {
                                            target = foreachPlayer;
                                            break;
                                        }
                                        else if (!Table.DealerFocused.Contains(foreachPlayer)) Table.DealerFocused.Add(foreachPlayer); // Добавляем в лист опрошенных, потому что у игрока уже 21, его не нужно опрашивать повторно
                                    }
                                }
                            }
                            if (target == null) // Все игроки за столом опрошены, заканчиваем игру
                            {
                                Table.DealerFocus = null;
                                Table.State = 6;
                                Table.StateInWork = false;
                                return;
                            }
                            Table.DealerFocus = target;
                            Table.DealerFocused.Add(target);
                            PlayDealerAnim(Index, Position, "anim_casino_b@amb@casino@games@blackjack@dealer", $"dealer_focus_player_0{PlayerData[target].SlotId}_idle_intro");
                            Table.Time = 1;
                            Table.State = 5;
                        }
                        Table.StateInWork = false;
                        break;
                    case 5: // Получаем решение игрока, на котором мы фокусируемся
                        Table.StateInWork = true;
                        switch (Table.AdditionalState)
                        {
                            case 0: // Начинаем обработку первого решения игрока
                                if (Table.DealerFocus == null || !Table.DealerFocus.IsCharacterData() || !PlayerData.ContainsKey(Table.DealerFocus)) // Если игрок вышел со стола (вылет или еще что-то), то берём нового игрока из списка
                                {
                                    Table.State = 4;
                                    Table.Time = 0;
                                    Table.StateInWork = false;
                                    return;
                                }
                                PlayDealerSpeech(Index, Position, "MINIGAME_BJACK_DEALER_ANOTHER_CARD");
                                Trigger.ClientEvent(Table.DealerFocus, "client.blackjack.isBtn", 0, 1);
                                Table.AdditionalState = 1;
                                Table.Time = 15;
                                break;
                            case 1: // Обрабатываем первое решение игрока
                                if (Table.DealerFocus != null)
                                {
                                    var targetCharacterData = Table.DealerFocus.GetCharacterData();
                                    if (targetCharacterData != null && PlayerData.ContainsKey(Table.DealerFocus) && Table.DealerFocused.Contains(Table.DealerFocus))
                                    {
                                        BlackjackPlayerInfo bInfo = PlayerData[Table.DealerFocus];
                                        if (bInfo.Join && bInfo.Hand.Count < 5)
                                        {
                                            if (bInfo.Move == null)
                                            {
                                                if (Table.Time > 0) Table.Time--;
                                                else if (Table.Time == 0)
                                                {
                                                    Table.State = 4;
                                                    Table.AdditionalState = 0;
                                                    Table.Time = 0;
                                                    Trigger.ClientEvent(Table.DealerFocus, "client.blackjack.isBtn", 0, 0);
                                                }
                                                else if (Table.Time == 5) PlayDealerSpeech(Index, Position, "MINIGAME_DEALER_COMMENT_SLOW");
                                                Trigger.ClientEvent(Table.DealerFocus, "client.blackjack.SyncTimer", Table.Time);
                                            }
                                            else if (bInfo.Move == "stand")
                                            {
                                                Table.State = 4;
                                                Table.AdditionalState = 0;
                                                Table.Time = 0;
                                            }
                                            else if (bInfo.Move == "hit")
                                            {
                                                bInfo.Move = null;
                                                Table.CardInHand = takeCard(Index);
                                                Table.Count = AddPlayerHand(Table.DealerFocus, Table.CardInHand);
                                                Trigger.ClientEventInRange(Position, 250f, "client.blackjack.GiveCard", Index, bInfo.SlotId, Table.Count - 1, Table.CardInHand);
                                                PlayDealerAnim(Index, Position, "anim_casino_b@amb@casino@games@blackjack@dealer", $"hit_card_player_0{bInfo.SlotId}");
                                                PlayDealerSpeech(Index, Position, $"MINIGAME_BJACK_DEALER_{PlayerHandValue(Table.DealerFocus)}");
                                                Table.Time = 1;
                                                if (bInfo.Hand.Count < 5 && PlayerHandValue(Table.DealerFocus) < 21 && !bInfo.Doubled) Table.AdditionalState = 2; // Если меньше 5 карт и общее число меньше 21 и это не дабл
                                                else
                                                {
                                                    Table.State = 4;
                                                    Table.AdditionalState = 0;
                                                }
                                            }
                                            else if (bInfo.Move == "double")
                                            {
                                                bInfo.Move = null;
                                                if (MoneySystem.Wallet.Change(Table.DealerFocus, -bInfo.Rate))
                                                {
                                                    GameLog.CasinoBJLog(-bInfo.Rate);
                                                    GameLog.Money($"player({targetCharacterData.UUID})", "system", bInfo.Rate, $"BJDouble");
                                                    bInfo.Rate = bInfo.Rate * 2;
                                                    Trigger.ClientEventInRange(Position, 250f, "client.blackjack.PlaceBetChip", bInfo.Index, bInfo.SlotId, bInfo.Rate, true, false);
                                                    bInfo.Doubled = true;
                                                    Table.CardInHand = takeCard(Index);
                                                    Table.Count = AddPlayerHand(Table.DealerFocus, Table.CardInHand);
                                                    Trigger.ClientEventInRange(Position, 250f, "client.blackjack.GiveCard", Index, bInfo.SlotId, Table.Count - 1, Table.CardInHand);
                                                    PlayDealerAnim(Index, Position, "anim_casino_b@amb@casino@games@blackjack@dealer", $"hit_card_player_0{bInfo.SlotId}");
                                                    PlayDealerSpeech(Index, Position, $"MINIGAME_BJACK_DEALER_{PlayerHandValue(Table.DealerFocus)}");
                                                    Table.Time = 1;
                                                    Table.State = 4;
                                                    Table.AdditionalState = 0;

                                                }
                                                else
                                                {
                                                    Table.State = 4;
                                                    Table.AdditionalState = 0;
                                                    Table.Time = 0;
                                                }
                                            }
                                            else if (bInfo.Move == "split")
                                            {
                                                bInfo.Move = null;
                                                if (MoneySystem.Wallet.Change(Table.DealerFocus, -bInfo.Rate))
                                                {
                                                    GameLog.CasinoBJLog(-bInfo.Rate);
                                                    GameLog.Money($"player({targetCharacterData.UUID})", "system", bInfo.Rate, $"BJSplit");
                                                    bInfo.Rate = bInfo.Rate * 2;
                                                    Trigger.ClientEventInRange(Position, 250f, "client.blackjack.PlaceBetChip", bInfo.Index, bInfo.SlotId, bInfo.Rate, false, true);
                                                    PlayDealerAnim(Index, Position, "anim_casino_b@amb@casino@games@blackjack@dealer", $"split_card_player_0{bInfo.SlotId}");
                                                    bInfo.SplitHand.Add(bInfo.Hand[bInfo.Hand.Count - 1]);
                                                    bInfo.Hand.RemoveAt(bInfo.Hand.Count - 1);
                                                    Table.Time = 0;
                                                    Table.AdditionalState = 3;
                                                }
                                                else
                                                {
                                                    Table.State = 4;
                                                    Table.AdditionalState = 0;
                                                    Table.Time = 0;
                                                }
                                            }
                                            else // Если вдруг что-то идёт не так (В нормальной игре так быть не должно)
                                            {
                                                bInfo.Move = null;
                                                Table.AdditionalState = 0;
                                                Table.State = 4;
                                                Table.Time = 0;
                                                Trigger.ClientEvent(Table.DealerFocus, "client.blackjack.isBtn", 0, 0);
                                            }
                                        }
                                        else
                                        {
                                            Table.State = 4;
                                            Table.Time = 0;
                                            Table.StateInWork = false;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        Table.State = 4;
                                        Table.Time = 0;
                                        Table.StateInWork = false;
                                        return;
                                    }
                                }
                                else
                                {
                                    Table.State = 4;
                                    Table.Time = 0;
                                    Table.StateInWork = false;
                                    return;
                                }
                                break;
                            case 2: // Пережидаем анимацию дилера
                                if (Table.Time > 0) Table.Time--;
                                else if (Table.Time == 0) Table.AdditionalState = 0;
                                break;
                            case 3: // Выдаем к
                                if (Table.DealerFocus != null)
                                {
                                    if (Table.DealerFocus.IsCharacterData() && PlayerData.ContainsKey(Table.DealerFocus) && Table.DealerFocused.Contains(Table.DealerFocus))
                                    {
                                        BlackjackPlayerInfo bInfo = PlayerData[Table.DealerFocus];
                                        Trigger.ClientEventInRange(Position, 250f, "client.blackjack.SplitHand", Index, bInfo.SlotId, bInfo.SplitHand.Count - 1);
                                        Table.Time = 0;
                                        Table.AdditionalState = 9;
                                    }
                                    else
                                    {
                                        Table.State = 4;
                                        Table.Time = 0;
                                        Table.StateInWork = false;
                                        return;
                                    }
                                }
                                else
                                {
                                    Table.State = 4;
                                    Table.Time = 0;
                                    Table.StateInWork = false;
                                    return;
                                }
                                break;
                            case 4: // Пережидаем анимацию дилера
                                if (Table.DealerFocus != null)
                                {
                                    if (Table.DealerFocus.IsCharacterData() && PlayerData.ContainsKey(Table.DealerFocus) && Table.DealerFocused.Contains(Table.DealerFocus))
                                    {
                                        BlackjackPlayerInfo bInfo = PlayerData[Table.DealerFocus];
                                        Table.CardInHand = takeCard(Index);
                                        Table.Count = AddPlayerHand(Table.DealerFocus, Table.CardInHand);
                                        Trigger.ClientEventInRange(Position, 250f, "client.blackjack.GiveCard", Index, bInfo.SlotId, Table.Count - 1, Table.CardInHand);
                                        PlayDealerAnim(Index, Position, "anim_casino_b@amb@casino@games@blackjack@dealer", $"hit_card_player_0{bInfo.SlotId}");
                                        PlayDealerSpeech(Index, Position, $"MINIGAME_BJACK_DEALER_{PlayerHandValue(Table.DealerFocus)}");
                                        Table.Time = 1;
                                        Table.AdditionalState = 5;
                                    }
                                    else
                                    {
                                        Table.State = 4;
                                        Table.Time = 0;
                                        Table.StateInWork = false;
                                        return;
                                    }
                                }
                                else
                                {
                                    Table.State = 4;
                                    Table.Time = 0;
                                    Table.StateInWork = false;
                                    return;
                                }
                                break;
                            case 5: // Пережидаем анимацию дилера
                                if (Table.Time > 0) Table.Time--;
                                else if (Table.Time == 0) Table.AdditionalState = 6;
                                break;
                            case 6: // Пережидаем анимацию дилера
                                if (Table.DealerFocus != null)
                                {
                                    if (Table.DealerFocus.IsCharacterData() && PlayerData.ContainsKey(Table.DealerFocus) && Table.DealerFocused.Contains(Table.DealerFocus))
                                    {
                                        BlackjackPlayerInfo bInfo = PlayerData[Table.DealerFocus];
                                        Table.CardSplitInHand = takeCard(Index);
                                        Table.CountSplit = AddPlayerSplitHand(Table.DealerFocus, Table.CardSplitInHand);
                                        Trigger.ClientEventInRange(Position, 250f, "client.blackjack.GiveCard", Index, bInfo.SlotId, Table.CountSplit - 1, Table.CardSplitInHand, false, true);
                                        PlayDealerAnim(Index, Position, "anim_casino_b@amb@casino@games@blackjack@dealer", $"hit_second_card_player_0{bInfo.SlotId}");
                                        PlayDealerSpeech(Index, Position, $"MINIGAME_BJACK_DEALER_{PlayerSplitHandValue(Table.DealerFocus)}");
                                        Table.AdditionalState = 8;
                                        Table.Time = 1;
                                    }
                                    else
                                    {
                                        Table.State = 4;
                                        Table.Time = 0;
                                        Table.StateInWork = false;
                                        return;
                                    }
                                }
                                else
                                {
                                    Table.State = 4;
                                    Table.Time = 0;
                                    Table.StateInWork = false;
                                    return;
                                }
                                break;
                            case 7: // Раздали в сплит 2 карты и начинаем новое действие
                                if (Table.DealerFocus != null)
                                {
                                    if (Table.DealerFocus.IsCharacterData() && PlayerData.ContainsKey(Table.DealerFocus) && Table.DealerFocused.Contains(Table.DealerFocus))
                                    {
                                        BlackjackPlayerInfo bInfo = PlayerData[Table.DealerFocus];
                                        Table.AdditionalState = 0;
                                        if (bInfo.Hand.Count < 5 && PlayerHandValue(Table.DealerFocus) < 21)
                                        {
                                            PlayDealerSpeech(Index, Position, "MINIGAME_BJACK_DEALER_ANOTHER_CARD");
                                            Table.State = 9; // Если меньше 5 карт и общее число меньше 21
                                            PlayDealerAnim(Index, Position, "anim_casino_b@amb@casino@games@blackjack@dealer", $"dealer_focus_player_0{bInfo.SlotId}_idle");
                                        }
                                        else if (bInfo.SplitHand.Count < 5 && PlayerSplitHandValue(Table.DealerFocus) < 21)
                                        {
                                            PlayDealerSpeech(Index, Position, "MINIGAME_BJACK_DEALER_ANOTHER_CARD");
                                            Table.State = 10; // Если меньше 5 карт и общее число меньше 21
                                            PlayDealerAnim(Index, Position, "anim_casino_b@amb@casino@games@blackjack@dealer", $"dealer_focus_player_0{bInfo.SlotId}_idle_split");
                                        }
                                        else Table.State = 4;
                                    }
                                    else
                                    {
                                        Table.State = 4;
                                        Table.Time = 0;
                                        Table.StateInWork = false;
                                        return;
                                    }
                                }
                                else
                                {
                                    Table.State = 4;
                                    Table.Time = 0;
                                    Table.StateInWork = false;
                                    return;
                                }
                                break;
                            case 8:
                                if (Table.Time > 0) Table.Time--;
                                else if (Table.Time == 0) Table.AdditionalState = 7;
                                break;
                            case 9:
                                if (Table.Time > 0) Table.Time--;
                                else if (Table.Time == 0) Table.AdditionalState = 4;
                                break;
                            default:
                                // Not supposed to end up here. 
                                break;
                        }
                        Table.StateInWork = false;
                        break;
                    case 6: // Дилер работает со своими картами
                        Table.StateInWork = true;
                        switch (Table.AdditionalState)
                        {
                            case 0:
                                PlayDealerAnim(Index, Position, "anim_casino_b@amb@casino@games@blackjack@dealer", "turn_card");
                                Trigger.ClientEventInRange(Position, 250f, "client.blackjack.DealerTurnOverCard", Index);
                                Table.Time = 1;
                                Table.AdditionalState = 1;
                                break;
                            case 1: // Дилер набирает карты до тех пор, пока не получит число 17 или выше
                                if (Table.Time > 0) Table.Time--;
                                else if (Table.Time == 0)
                                {
                                    Table.Value = DealerHandValue(Index);
                                    PlayDealerSpeech(Index, Position, $"MINIGAME_BJACK_DEALER_{Table.Value}");
                                    if (Table.Value < 17)
                                    {
                                        Table.CardInHand = takeCard(Index);
                                        Table.Count = AddDealerHand(Index, Table.CardInHand);
                                        Trigger.ClientEventInRange(Position, 250f, "client.blackjack.GiveCard", Index, 0, Table.Count - 1, Table.CardInHand, (bool)(Table.Count == 1));
                                        PlayDealerAnim(Index, Position, "anim_casino_b@amb@casino@games@blackjack@dealer", "deal_card_self_second_card");
                                        Table.Time = 1;
                                    }
                                    else
                                    {
                                        Table.AdditionalState = 0;
                                        Table.State = 7;
                                    }
                                }
                                break;
                            default:
                                // Not supposed to end up here. 
                                break;
                        }
                        Table.StateInWork = false;
                        break;
                    case 7: // Работаем по каждому игроку, который есть за столом отдельно
                        Table.StateInWork = true;
                        if (Table.AdditionalState == 0 && DealerHandValue(Index) > 21)
                        {
                            Table.AdditionalState = 1;
                            PlayDealerSpeech(Index, Position, "MINIGAME_DEALER_BUSTS");
                        }
                        Table.DealerFocus = null;
                        ExtPlayer winner = null;
                        lock (DealerData[Index].GamePlayers)
                        {
                            foreach (ExtPlayer foreachPlayer in DealerData[Index].GamePlayers)
                            {
                                if (foreachPlayer.IsCharacterData() && PlayerData.ContainsKey(foreachPlayer) && PlayerData[foreachPlayer].Join && Table.DealerFocused.Contains(foreachPlayer))
                                {
                                    winner = foreachPlayer;
                                    break;
                                }
                            }
                        }
                        if (winner == null) // Все игроки за столом получили призы, очищаем стол дилера и игру
                        {
                            lock (DealerData[Index].GamePlayers)
                            {
                                foreach (ExtPlayer foreachPlayer in DealerData[Index].GamePlayers)
                                {
                                    if (foreachPlayer.IsCharacterData() && PlayerData.ContainsKey(foreachPlayer))
                                    {
                                        BlackjackPlayerInfo bInfo = PlayerData[foreachPlayer];
                                        bInfo.Join = false;
                                        bInfo.Rate = 0;
                                        bInfo.Doubled = false;
                                        bInfo.Move = null;
                                        bInfo.Hand = new List<string>();
                                        bInfo.SplitHand = new List<string>();
                                        //Trigger.ClientEvent(target, "client.blackjack.ExitBtn", 1);
                                    }
                                }
                            }
                            Trigger.ClientEventInRange(Position, 250f, "client.blackjack.RetrieveCards", Index, 0);
                            List<ExtPlayer> newPlayers = new List<ExtPlayer>();
                            foreach (ExtPlayer foreachPlayer in DealerData[Index].Players)
                            {
                                if (foreachPlayer.IsCharacterData() && PlayerData.ContainsKey(foreachPlayer)) 
                                {
                                    Trigger.ClientEvent(foreachPlayer, "client.blackjack.isBtn", 1, 0);
                                    Trigger.ClientEvent(foreachPlayer, "client.blackjack.ExitBtn", 1);
                                }
                            }
                            PlayDealerAnim(Index, Position, "anim_casino_b@amb@casino@games@blackjack@dealer", "retrieve_own_cards_and_remove");
                            DealerData[Index].Hand = new List<string>();
                            DealerData[Index].Cards = new List<string>();
                            DealerData[Index].GamePlayers = new List<ExtPlayer>();
                            DealerData[Index].GameRunning = false;
                            BlackjackData[Index].DealerFocus = null;
                            if (BlackjackData[Index].Timer != null)
                            {
                                Timers.Stop(BlackjackData[Index].Timer);
                                BlackjackData[Index].Timer = null;
                            }
                            Table.StateInWork = false;
                            return;
                        }
                        Table.DealerFocus = winner;
                        Table.State = 8;
                        Table.AdditionalState = 0;
                        Table.StateInWork = false;
                        break;
                    case 8: // Забираем карты у игрока и выдаем выигрышь
                        Table.StateInWork = true;
                        switch (Table.AdditionalState)
                        {
                            case 0:
                                if (Table.DealerFocus != null)
                                {
                                    var targetCharacterData = Table.DealerFocus.GetCharacterData();
                                    if (targetCharacterData != null && PlayerData.ContainsKey(Table.DealerFocus) && PlayerData[Table.DealerFocus].Join)
                                    {
                                        BlackjackPlayerInfo bInfo = PlayerData[Table.DealerFocus];

                                        int pHandValue;
                                        int wincount = 0;
                                        int rounded = 0;
                                        if (bInfo.SplitHand.Count > 0)
                                        {
                                            bInfo.Rate /= 2;
                                            pHandValue = PlayerSplitHandValue(Table.DealerFocus);
                                            if (pHandValue <= 21)
                                            {
                                                if (pHandValue == 21)
                                                {
                                                    if (bInfo.SplitHand.Count == 2) // BlackJack
                                                    {
                                                        rounded = (int)Math.Round(bInfo.Rate * 2.5);
                                                        wincount += rounded;
                                                        GameLog.Money("system", $"player({targetCharacterData.UUID})", rounded, $"BJWinSplit");
                                                        MoneySystem.Wallet.Change(Table.DealerFocus, rounded);
                                                        GameLog.CasinoBJLog(rounded);
                                                    }
                                                    else
                                                    {
                                                        wincount += bInfo.Rate * 2;
                                                        GameLog.Money("system", $"player({targetCharacterData.UUID})", bInfo.Rate * 2, $"BJWinSplit");
                                                        MoneySystem.Wallet.Change(Table.DealerFocus, bInfo.Rate * 2);
                                                        GameLog.CasinoBJLog(bInfo.Rate * 2);
                                                    }
                                                }
                                                else
                                                {
                                                    if (pHandValue == Table.Value)
                                                    {
                                                        wincount += bInfo.Rate;
                                                        GameLog.Money("system", $"player({targetCharacterData.UUID})", bInfo.Rate, $"BJDrawSplit");
                                                        MoneySystem.Wallet.Change(Table.DealerFocus, bInfo.Rate);
                                                        GameLog.CasinoBJLog(bInfo.Rate);
                                                    }
                                                    else if (pHandValue > Table.Value || Table.Value > 21)
                                                    {
                                                        wincount += bInfo.Rate * 2;
                                                        GameLog.Money("system", $"player({targetCharacterData.UUID})", bInfo.Rate * 2, $"BJWinSplit");
                                                        MoneySystem.Wallet.Change(Table.DealerFocus, bInfo.Rate * 2);
                                                        GameLog.CasinoBJLog(bInfo.Rate * 2);
                                                    }
                                                }
                                            }
                                        }
                                        pHandValue = PlayerHandValue(Table.DealerFocus);
                                        if (pHandValue <= 21) // У игрока 21 или меньше
                                        {
                                            if (pHandValue == 21)
                                            {
                                                if (bInfo.Hand.Count == 2) // BlackJack
                                                {
                                                    rounded = (int)Math.Round(bInfo.Rate * 2.5);
                                                    wincount += rounded;
                                                    GameLog.Money("system", $"player({targetCharacterData.UUID})", rounded, $"BJWin");
                                                    AnimationGameEnd(Table.DealerFocus, "good");
                                                    MoneySystem.Wallet.Change(Table.DealerFocus, rounded);
                                                    GameLog.CasinoBJLog(rounded);
                                                }
                                                else
                                                {
                                                    wincount += bInfo.Rate * 2;
                                                    GameLog.Money("system", $"player({targetCharacterData.UUID})", bInfo.Rate * 2, $"BJWin");
                                                    MoneySystem.Wallet.Change(Table.DealerFocus, bInfo.Rate * 2);
                                                    GameLog.CasinoBJLog(bInfo.Rate * 2);
                                                    AnimationGameEnd(Table.DealerFocus, "impartial");
                                                }
                                            }
                                            else
                                            {
                                                if (Table.Value == 21 || (Table.Value < 21 && Table.Value > pHandValue)) // Проигрышь
                                                {
                                                    AnimationGameEnd(Table.DealerFocus, "bad");
                                                }
                                                else if (pHandValue == Table.Value) // Ничья
                                                {
                                                    wincount += bInfo.Rate;
                                                    GameLog.Money("system", $"player({targetCharacterData.UUID})", bInfo.Rate, $"BJDraw");
                                                    MoneySystem.Wallet.Change(Table.DealerFocus, bInfo.Rate);
                                                    GameLog.CasinoBJLog(bInfo.Rate);
                                                    AnimationGameEnd(Table.DealerFocus, "impartial");
                                                }
                                                else if (pHandValue > Table.Value || Table.Value > 21) // Победа
                                                {
                                                    wincount += bInfo.Rate * 2;
                                                    GameLog.Money("system", $"player({targetCharacterData.UUID})", bInfo.Rate * 2, $"BJWin");
                                                    MoneySystem.Wallet.Change(Table.DealerFocus, bInfo.Rate * 2);
                                                    GameLog.CasinoBJLog(bInfo.Rate * 2);
                                                    AnimationGameEnd(Table.DealerFocus, "good");
                                                }
                                            }
                                        }
                                        else // Проигрышь
                                        {
                                            AnimationGameEnd(Table.DealerFocus, "bad");
                                        }
                                        bInfo.Join = false;
                                        if (wincount > 0) Trigger.ClientEvent(Table.DealerFocus, "client.blackjack.betWin", wincount);
                                        Table.SlotIDReceive = bInfo.SlotId;
                                        PlayDealerAnim(Index, Position, "anim_casino_b@amb@casino@games@blackjack@dealer", $"retrieve_cards_player_0{Table.SlotIDReceive}");
                                    }
                                    else
                                    {
                                        Table.State = 7;
                                        Table.AdditionalState = 1;
                                        Table.StateInWork = false;
                                        return;
                                    }
                                }
                                else
                                {
                                    Table.State = 7;
                                    Table.AdditionalState = 1;
                                    Table.StateInWork = false;
                                    return;
                                }
                                Table.AdditionalState = 1;
                                break;
                            case 1: // Забираем карты со стола игрока и переходим к следующему игроку
                                Trigger.ClientEventInRange(Position, 250f, "client.blackjack.RetrieveCards", Index, Table.SlotIDReceive);
                                Table.State = 7;
                                Table.AdditionalState = 1;
                                break;
                            default:
                                // Not supposed to end up here. 
                                break;
                        }
                        Table.StateInWork = false;
                        break;
                    case 9: // Работаем с первой рукой сплит-системы
                        Table.StateInWork = true;
                        switch (Table.AdditionalState)
                        {
                            case 0: // Начинаем обработку первого решения игрока
                                if (Table.DealerFocus == null || !Table.DealerFocus.IsCharacterData() || !PlayerData.ContainsKey(Table.DealerFocus)) // Если игрок вышел со стола (вылет или еще что-то), то берём нового игрока из списка
                                {
                                    Table.State = 4;
                                    Table.Time = 0;
                                    Table.StateInWork = false;
                                    return;
                                }
                                PlayDealerSpeech(Index, Position, "MINIGAME_BJACK_DEALER_ANOTHER_CARD");
                                Trigger.ClientEvent(Table.DealerFocus, "client.blackjack.isBtn", 0, 1);
                                Table.AdditionalState = 1;
                                Table.Time = 15;
                                break;
                            case 1: // Обрабатываем первое решение игрока
                                if (Table.DealerFocus != null)
                                {
                                    if (Table.DealerFocus.IsCharacterData() && PlayerData.ContainsKey(Table.DealerFocus) && Table.DealerFocused.Contains(Table.DealerFocus))
                                    {
                                        BlackjackPlayerInfo bInfo = PlayerData[Table.DealerFocus];
                                        if (bInfo.Join && bInfo.Hand.Count < 5)
                                        {
                                            if (bInfo.Move == null)
                                            {
                                                if (Table.Time > 0) Table.Time--;
                                                else if (Table.Time == 0)
                                                {
                                                    Table.AdditionalState = 0;
                                                    if (bInfo.SplitHand.Count < 5 && PlayerSplitHandValue(Table.DealerFocus) < 21) Table.State = 10;
                                                    else
                                                    {
                                                        Table.State = 4;
                                                        Table.Time = 0;
                                                    }
                                                    Trigger.ClientEvent(Table.DealerFocus, "client.blackjack.isBtn", 0, 0);
                                                }
                                                else if (Table.Time == 5) PlayDealerSpeech(Index, Position, "MINIGAME_DEALER_COMMENT_SLOW");
                                                Trigger.ClientEvent(Table.DealerFocus, "client.blackjack.SyncTimer", Table.Time);
                                            }
                                            else if (bInfo.Move == "stand")
                                            {
                                                bInfo.Move = null;
                                                Table.AdditionalState = 0;
                                                if (bInfo.SplitHand.Count < 5 && PlayerSplitHandValue(Table.DealerFocus) < 21) Table.State = 10;
                                                else
                                                {
                                                    Table.State = 4;
                                                    Table.Time = 0;
                                                }
                                            }
                                            else if (bInfo.Move == "hit")
                                            {
                                                bInfo.Move = null;
                                                Table.CardInHand = takeCard(Index);
                                                Table.Count = AddPlayerHand(Table.DealerFocus, Table.CardInHand);
                                                Trigger.ClientEventInRange(Position, 250f, "client.blackjack.GiveCard", Index, bInfo.SlotId, Table.Count - 1, Table.CardInHand);
                                                PlayDealerAnim(Index, Position, "anim_casino_b@amb@casino@games@blackjack@dealer", $"hit_card_player_0{bInfo.SlotId}");
                                                PlayDealerSpeech(Index, Position, $"MINIGAME_BJACK_DEALER_{PlayerHandValue(Table.DealerFocus)}");
                                                Table.Time = 1;
                                                if (bInfo.Hand.Count < 5 && PlayerHandValue(Table.DealerFocus) < 21) Table.AdditionalState = 2; // Если меньше 5 карт и общее число меньше 21 и это не дабл
                                                else
                                                {
                                                    if (bInfo.SplitHand.Count < 5 && PlayerSplitHandValue(Table.DealerFocus) < 21) Table.AdditionalState = 3;
                                                    else
                                                    {
                                                        Table.AdditionalState = 0;
                                                        Table.State = 4;
                                                    }
                                                }
                                            }
                                            else // Если вдруг что-то идёт не так (В нормальной игре так быть не должно)
                                            {
                                                bInfo.Move = null;
                                                Table.AdditionalState = 0;
                                                if (bInfo.SplitHand.Count < 5 && PlayerSplitHandValue(Table.DealerFocus) < 21) Table.State = 10;
                                                else
                                                {
                                                    Table.State = 4;
                                                    Table.Time = 0;
                                                }
                                                Trigger.ClientEvent(Table.DealerFocus, "client.blackjack.isBtn", 0, 0);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Table.State = 4;
                                        Table.Time = 0;
                                        Table.StateInWork = false;
                                        return;
                                    }
                                }
                                else
                                {
                                    Table.State = 4;
                                    Table.Time = 0;
                                    Table.StateInWork = false;
                                    return;
                                }
                                break;
                            case 2: // Пережидаем анимацию дилера
                                if (Table.Time > 0) Table.Time--;
                                else if (Table.Time == 0) Table.AdditionalState = 0;
                                break;
                            case 3: // Пережидаем анимацию дилера
                                if (Table.Time > 0) Table.Time--;
                                else if (Table.Time == 0)
                                {
                                    Table.AdditionalState = 0;
                                    Table.State = 10;
                                }
                                break;
                            default:
                                // Not supposed to end up here. 
                                break;
                        }
                        Table.StateInWork = false;
                        break;
                    case 10: // Работаем со второй рукой сплит-системы
                        Table.StateInWork = true;
                        switch (Table.AdditionalState)
                        {
                            case 0: // Начинаем обработку первого решения игрока
                                if (Table.DealerFocus == null || !Table.DealerFocus.IsCharacterData() || !PlayerData.ContainsKey(Table.DealerFocus)) // Если игрок вышел со стола (вылет или еще что-то), то берём нового игрока из списка
                                {
                                    Table.State = 4;
                                    Table.Time = 0;
                                    Table.StateInWork = false;
                                    return;
                                }
                                Trigger.ClientEvent(Table.DealerFocus, "client.blackjack.isBtn", 0, 1);
                                PlayDealerSpeech(Index, Position, "MINIGAME_BJACK_DEALER_ANOTHER_CARD");
                                PlayDealerAnim(Index, Position, "anim_casino_b@amb@casino@games@blackjack@dealer", $"dealer_focus_player_0{PlayerData[Table.DealerFocus].SlotId}_idle_split");
                                Table.AdditionalState = 1;
                                Table.Time = 15;
                                break;
                            case 1: // Обрабатываем первое решение игрока
                                if (Table.DealerFocus != null)
                                {
                                    if (Table.DealerFocus.IsCharacterData() && PlayerData.ContainsKey(Table.DealerFocus) && Table.DealerFocused.Contains(Table.DealerFocus))
                                    {
                                        BlackjackPlayerInfo bInfo = PlayerData[Table.DealerFocus];
                                        if (bInfo.Join && bInfo.SplitHand.Count < 5)
                                        {
                                            if (bInfo.Move == null)
                                            {
                                                if (Table.Time > 0) Table.Time--;
                                                else if (Table.Time == 0)
                                                {
                                                    Table.State = 4;
                                                    Table.AdditionalState = 0;
                                                    Table.Time = 0;
                                                    Trigger.ClientEvent(Table.DealerFocus, "client.blackjack.isBtn", 0, 0);
                                                }
                                                else if (Table.Time == 5) PlayDealerSpeech(Index, Position, "MINIGAME_DEALER_COMMENT_SLOW");
                                                Trigger.ClientEvent(Table.DealerFocus, "client.blackjack.SyncTimer", Table.Time);
                                            }
                                            else if (bInfo.Move == "stand")
                                            {
                                                bInfo.Move = null;
                                                Table.State = 4;
                                                Table.AdditionalState = 0;
                                                Table.Time = 0;
                                            }
                                            else if (bInfo.Move == "hit")
                                            {
                                                bInfo.Move = null;
                                                Table.CardSplitInHand = takeCard(Index);
                                                Table.CountSplit = AddPlayerSplitHand(Table.DealerFocus, Table.CardSplitInHand);
                                                Trigger.ClientEventInRange(Position, 250f, "client.blackjack.GiveCard", Index, bInfo.SlotId, Table.CountSplit - 1, Table.CardSplitInHand, false, true);
                                                PlayDealerAnim(Index, Position, "anim_casino_b@amb@casino@games@blackjack@dealer", $"hit_second_card_player_0{bInfo.SlotId}");
                                                PlayDealerSpeech(Index, Position, $"MINIGAME_BJACK_DEALER_{PlayerSplitHandValue(Table.DealerFocus)}");
                                                Table.Time = 1;
                                                if (bInfo.SplitHand.Count < 5 && PlayerSplitHandValue(Table.DealerFocus) < 21) Table.AdditionalState = 2; // Если меньше 5 карт и общее число меньше 21
                                                else
                                                {
                                                    Table.State = 4;
                                                    Table.AdditionalState = 0;
                                                }
                                            }
                                            else // Если вдруг что-то идёт не так (В нормальной игре так быть не должно)
                                            {
                                                bInfo.Move = null;
                                                Table.AdditionalState = 0;
                                                Table.State = 4;
                                                Table.Time = 0;
                                                Trigger.ClientEvent(Table.DealerFocus, "client.blackjack.isBtn", 0, 0);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Table.State = 4;
                                        Table.Time = 0;
                                        Table.StateInWork = false;
                                        return;
                                    }
                                }
                                else
                                {
                                    Table.State = 4;
                                    Table.Time = 0;
                                    Table.StateInWork = false;
                                    return;
                                }
                                break;
                            case 2: // Пережидаем анимацию дилера
                                if (Table.Time > 0) Table.Time--;
                                else if (Table.Time == 0) Table.AdditionalState = 0;
                                break;
                            default:
                                // Not supposed to end up here. 
                                break;
                        }
                        Table.StateInWork = false;
                        break;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"BlackJackGame Exception: {e.ToString()}");
            }
        }

        private static void AnimationGameEnd(ExtPlayer player, string anim)
        {
            try
            {
                if (player.IsCharacterData())
                {
                    Random rand = new Random();
                    Trigger.PlayAnimation(player, "anim_casino_b@amb@casino@games@shared@player@", $"reaction_{anim}_var_0{rand.Next(1, 5)}", 3);
                    // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "animationgameend");
                    Timers.StartOnce(4000, () =>
                    {
                        if (player.IsCharacterData()) Trigger.PlayAnimation(player, "anim_casino_b@amb@casino@games@shared@player@", $"idle_var_01", 3);
                        // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "casinobet1");
                    });
                }
            }
            catch (Exception e)
            {
                Log.Write($"AnimationGameEnd Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.blackjack.move")]
        public static void BlackjackMove(ExtPlayer player, string move)
        {
            try
            {
                if (player.IsCharacterData() && PlayerData.ContainsKey(player)) PlayerData[player].Move = move;
            }
            catch (Exception e)
            {
                Log.Write($"BlackjackMove Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.blackjack.character_leave_place")]
        public static void BlackjackLeave(ExtPlayer player)
        {
            try
            {
                if (player.IsCharacterData() && PlayerData.ContainsKey(player))
                {
                    BlackjackPlayerInfo bInfo = PlayerData[player];
                    if (DealerData[bInfo.Index].GameRunning && bInfo.Rate >= 1) return;
                    BlackjackLeave(player, false);
                }
            }
            catch (Exception e)
            {
                Log.Write($"BlackjackLeave Exception: {e.ToString()}");
            }
        }
        public static void BlackjackLeave(ExtPlayer player, bool type)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!PlayerData.ContainsKey(player)) return;
                BlackjackPlayerInfo bInfo = PlayerData[player];
                PlayerData.Remove(player);
                if (DealerData.ContainsKey(bInfo.Index))
                {
                    if (BlackjackData[bInfo.Index].DealerFocus == player) BlackjackData[bInfo.Index].DealerFocus = null;
                    if (!type && !DealerData[bInfo.Index].GameRunning)
                    {
                        MoneySystem.Wallet.Change(player, bInfo.Rate);
                        GameLog.CasinoBJLog(bInfo.Rate);
                        GameLog.Money("system", $"player({characterData.UUID})", bInfo.Rate, $"BJBetReturn");
                    }
                    if (DealerData[bInfo.Index].Chairs.Contains(bInfo.SlotId)) DealerData[bInfo.Index].Chairs.Remove(bInfo.SlotId);
                    if (DealerData[bInfo.Index].Players.Contains(player)) DealerData[bInfo.Index].Players.Remove(player);

                    Trigger.ClientEventInRange(player.Position, 250f, "client.blackjack.RetrieveCards", bInfo.Index, bInfo.SlotId);
                    if (DealerData[bInfo.Index].GamePlayers.Contains(player)) DealerData[bInfo.Index].GamePlayers.Remove(player);

                    if ((DealerData[bInfo.Index].GameRunning && DealerData[bInfo.Index].GamePlayers.Count == 0) || (!DealerData[bInfo.Index].GameRunning && DealerData[bInfo.Index].Players.Count == 0))
                    {
                        // Если игра начата и активных игроков 0, то отключаем таймер стола. Если игра не начата и никто не сидит за столом, тоже отключаем.
                        Trigger.ClientEventInRange(player.Position, 250f, "client.blackjack.RetrieveCards", bInfo.Index, 0);
                        DealerData[bInfo.Index].Hand = new List<string>();
                        DealerData[bInfo.Index].Cards = new List<string>();
                        DealerData[bInfo.Index].GameRunning = false;
                        BlackjackData[bInfo.Index].DealerFocus = null;
                        if (BlackjackData[bInfo.Index].Timer != null)
                        {
                            Timers.Stop(BlackjackData[bInfo.Index].Timer);
                            BlackjackData[bInfo.Index].Timer = null;
                        }
                    }
                }
                if (!type)
                {
                    sessionData.IsCasinoGame = null;
                    Trigger.ClientEvent(player, "client.blackjack.betWin", 0);
                    Trigger.PlayAnimation(player, "anim_casino_b@amb@casino@games@shared@player@", "sit_exit_left", 39);
                    // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "exitcasino");
                    Trigger.ClientEvent(player, "client.blackjack.PlayDealerSpeech", bInfo.Index, "MINIGAME_DEALER_LEAVE_NEUTRAL_GAME");
                    Trigger.ClientEvent(player, "client.blackjack.successLeave");
                    Timers.StartOnce(4000, () => 
                    {
                        if (player.IsCharacterData()) Trigger.StopAnimation(player);
                    });
                }
            }
            catch (Exception e)
            {
                Log.Write($"BlackjackLeave Exception: {e.ToString()}");
            }
        }
    }
}
