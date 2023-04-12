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
using Localization;
using NeptuneEvo.Quests;

namespace NeptuneEvo.NewCasino
{
    class Spin : Script
    {
        /** Данные rotation для объекта спина */
        List<float> SpinsConvert = new List<float>
        {
            25.3f,//слива
            47.3f,//вишня
            70.3f,//арбуз
            93.3f,//нож
            116.3f,//пила
            160.3f,
            2.3f//7
        };
        private static readonly nLog Log = new nLog("NewCasino.Spin");

        public static bool SpinsWorking = true;

        public static List<bool> SpinsState = new List<bool> {
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false
        };
        /// <summary>
        /// Данные игрока
        /// </summary>
        public static Dictionary<ExtPlayer, SpinPlayerData> PlayerData = new Dictionary<ExtPlayer, SpinPlayerData>();

        /// <summary>
        /// Начать игру
        /// </summary>
        /// <param name="player"></param>
        /// 
        public static int GetRound(List<int> listRandom)
        {
            int maxCount = 0;
            foreach (int list in listRandom)
            {
                maxCount += list;
            }

            int idItem = new Random().Next(0, maxCount);

            maxCount = 0;
            int index = 0;
            foreach (int list in listRandom)
            {
                maxCount += list;
                if (maxCount >= idItem) break;
                else index++;
            }
            return index;
        }

        [RemoteEvent("server.spin.bet")]
        private void spin(ExtPlayer player, int money)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!PlayerData.ContainsKey(player)) return;
                SpinPlayerData sData = PlayerData[player];

                if (sData.Spins.Count > 0) return;
                else if (UpdateData.CanIChange(player, money, true) != 255)
                {
                    Trigger.ClientEvent(player, "client.spin.CLEAR_SPIN");
                    return;
                }
                if (!SpinsWorking)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                GameLog.CasinoSpinsLog(-money);
                MoneySystem.Wallet.Change(player, -money);
                GameLog.Money($"player({characterData.UUID})", $"system", money, $"SpinBet");          
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
                sData.Cash = money;
                sData.Time = DateTime.Now.AddSeconds(2);

                List<int> listRandom = new List<int>
                {
                    1650,//слива
                    1650,//вишня
                    1650,//арбуз
                    1650,//нож
                    1650,//пила
                    1650,
                    100,//7
                };

                for (int i = 0; i < 3; i++)
                {
                    int rand = GetRound(listRandom);
                    listRandom[rand] = Convert.ToInt32(listRandom[rand] / 2.75);
                    sData.Spins.Add(rand);
                }

                Trigger.PlayAnimation(player, player.Model == NAPI.Util.GetHashKey("MP_M_Freemode_01") ? "anim_casino_a@amb@casino@games@slots@male" : "anim_casino_a@amb@casino@games@slots@female", "press_betone_a", 49);
                // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "spin");

                Timers.StartOnce(600, () =>
                {
                    if (!player.IsCharacterData() || !PlayerData.ContainsKey(player))
                        return;
                    Trigger.StopAnimation(player);
                    Trigger.PlayAnimation(player, "anim_casino_b@amb@casino@games@shared@player@", "idle_a", 3);
                    // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "spin2");
                    Trigger.ClientEvent(player, "client.spin.SPIN", SpinsConvert[sData.Spins[0]], SpinsConvert[sData.Spins[1]], SpinsConvert[sData.Spins[2]]);
                });

                sData.Spins.Sort();

                int winner = 0;
                bool seven = false;

                for (int i = 0; i < 2; i++)
                {
                    if (sData.Spins[i] == sData.Spins[i + 1])
                    {
                        winner++;
                        if (sData.Spins[i] == 6) seven = true;
                    }
                }
                if (winner == 1 && !seven) winner = 2;
                else if (winner == 2 && !seven) winner = 6;
                else if (winner == 1 && seven) winner = 4;
                else if (winner == 2 && seven) winner = 10;
                int winMoney = sData.Cash * winner;

                string message = $"Вы {(winner > 0 ? $"выиграли. Ваш выигрыш: ${winMoney}" : "проиграли.")}"; // AA SLOJNO))

                Timers.StartOnce(10000, () => {
                    if (!player.IsCharacterData() || !PlayerData.ContainsKey(player))
                        return;
                    sData.Spins = new List<int>();
                    sData.Cash = 0;
                    Trigger.ClientEvent(player, "client.spin.CLEAR_SPIN");
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, message, 3000);
                    BattlePass.Repository.UpdateReward(player, 4);

                    if (winner > 0)
                    {
                        GameLog.CasinoSpinsLog(winMoney);
                        MoneySystem.Wallet.Change(player, winMoney);
                        GameLog.Money($"system", $"player({characterData.UUID})", winMoney, $"SpinWin");    
                        
                        Trigger.PlayAnimation(player, characterData.Gender ? "anim_casino_a@amb@casino@games@slots@male" : "anim_casino_a@amb@casino@games@slots@female", "win_spinning_wheel", 49);
                        // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "spin3");
                        Timers.StartOnce(4000, () =>
                        {
                            Trigger.StopAnimation(player);
                            Trigger.PlayAnimation(player, "anim_casino_b@amb@casino@games@shared@player@", "idle_a", 3);
                            // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "spin2");
                        });
                    }
                });
            }
            catch (Exception e)
            {
                Log.Write($"spin Exception: {e.ToString()}");
            }
        }

        /// <summary>
        /// Занять автомат
        /// </summary>
        /// <param name="player"></param>
        /// <param name="id"></param>
        /// <param name="PosX"></param>
        /// <param name="PosY"></param>
        /// <param name="PosZ"></param>
        /// <param name="Rotation"></param>
        [RemoteEvent("server.spin.OCCUPY_SLOT")]
        private void occupy_slot(ExtPlayer player, int id, float PosX, float PosY, float PosZ, float Rotation)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData() || PlayerData.ContainsKey(player))
                    return;
                if (!FunctionsAccess.IsWorking("spin"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                if (SpinsState[id])
                {
                    Trigger.ClientEvent(player, "client.spin.OCCUPY_SLOT", null);
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlaceOwned), 3000);
                    return;
                }

                PlayerData.Add(player, new SpinPlayerData(id));
                sessionData.IsCasinoGame = "Spin";

                Trigger.StopAnimation(player);

                SpinsState[id] = true;
                player.Position = new Vector3(PosX, PosY, PosZ);
                player.Rotation = new Vector3(0, 0, Rotation);
                Trigger.ClientEventInRange(new Vector3(PosX, PosY, PosZ), 250f, "setClientRotation", player.Value, Rotation);

                Trigger.PlayAnimation(player, "anim_casino_b@amb@casino@games@shared@player@", "sit_enter_left_side", 3);
                // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "spinenter");
                Trigger.ClientEvent(player, "client.spin.OCCUPY_SLOT", id);

                Timers.StartOnce(4000, () =>
                {
                    if (!player.IsCharacterData() || !PlayerData.ContainsKey(player))
                        return;
                    Trigger.PlayAnimation(player, "anim_casino_b@amb@casino@games@shared@player@", "idle_a", 3);
                    // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "spin2");
                    Trigger.ClientEvent(player, "freeze", true);
                });
            }
            catch (Exception e)
            {
                Log.Write($"occupy_slot Exception: {e.ToString()}");
            }
        }

        /// <summary>
        /// Освободить автомат
        /// </summary>
        /// <param name=""></param>
        [RemoteEvent("server.spin.LEAVE_SLOT")]
        private void leave_slot(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData() || !PlayerData.ContainsKey(player))
                    return;
                SpinPlayerData sData = PlayerData[player];
                Disconnect(player, DisconnectionType.Timeout);

                Trigger.PlayAnimation(player, "anim_casino_b@amb@casino@games@shared@player@", "sit_exit_left", 39);
                // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "spinleft");
                Trigger.ClientEvent(player, "freeze", false);
                Timers.StartOnce(4000, () => {
                    if (!player.IsCharacterData())
                        return;
                    Trigger.ClientEvent(player, "client.spin.LEAVE_SLOT", sData.SelectSpin);
                    Trigger.StopAnimation(player);
                });
            }
            catch (Exception e)
            {
                Log.Write($"leave_slot Exception: {e.ToString()}");
            }
        }

        [ServerEvent(Event.PlayerDeath)]
        public void OnPlayerDeath(ExtPlayer player, ExtPlayer killer, uint reason)
        {
            try
            {
                if (!player.IsCharacterData() || !PlayerData.ContainsKey(player))
                    return;
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
                SpinPlayerData sData = PlayerData[player];

                if (sData.Cash > 0 && sData.Time >= DateTime.Now)
                {
                    GameLog.CasinoSpinsLog(sData.Cash);
                    MoneySystem.Wallet.Change(player, sData.Cash);
                    GameLog.Money($"system", $"player({characterData.UUID})", sData.Cash, $"SpinBetReturn");
                }
                SpinsState[sData.SelectSpin] = false;
                PlayerData.Remove(player);

                sessionData.IsCasinoGame = null;
            }
            catch (Exception e)
            {
                Log.Write($"Disconnect Exception: {e.ToString()}");
            }
        }
    }
}
