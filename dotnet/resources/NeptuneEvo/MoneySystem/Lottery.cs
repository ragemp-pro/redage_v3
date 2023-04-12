using GTANetworkAPI;
using NeptuneEvo.Handles;
using System.Collections.Generic;
using Redage.SDK;
using System.Linq;
using System.Data;
using System;
using Localization;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;

namespace NeptuneEvo.MoneySystem
{
    class Lottery : Script
    {
        private static readonly nLog Log = new nLog("MoneySystem.Lottery");

        public static uint ID;
        public static uint Price = 0;
        public static uint Bonus = 0;
        //public static byte Step = 0;
        public static Dictionary<uint, int> LotteryBought; // Lottery Ticket | Player UUID

        public static void OnResourceStart()
        {
            try
            {
                LotteryBought = new Dictionary<uint, int>();
                using DataTable result = MySQL.QueryRead($"SELECT * FROM `lottery`");
                if (result is null || result.Rows.Count == 0) return;
                DataRow row = result.Rows[0];
                ID = (uint)row[0];
                using DataTable resultLottery_players = MySQL.QueryRead($"SELECT * FROM `lottery_players` WHERE `number`={ID}");
                if (resultLottery_players is null || resultLottery_players.Rows.Count == 0)
                {
                    Log.Write("Lottery successfully started", nLog.Type.Success);
                    return;
                }
                int pname = 0;
                foreach (DataRow rows in resultLottery_players.Rows)
                {
                    pname = (int)rows[2];
                    LotteryBought.Add((uint)rows[1], pname);
                }
                Price = 350 * (uint)resultLottery_players.Rows.Count;
                Log.Write("Lottery successfully started", nLog.Type.Success);
            }
            catch (Exception e)
            {
                Log.Write($"OnResourceStart Exception: {e.ToString()}");
            }
        }

        [Command("lottery")]
        public static void CMD_CheckLottery(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.AdminLVL != 0)
                {
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LotteryAnnounce, LotteryBought.Count(), Wallet.Format(Price)), 3000);
                    return;
                }
                else
                {
                    if (LotteryBought.Count == 0)
                    {
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ZeroTicketsLottery), 7000);
                        return;
                    }
                    int mytotal = LotteryBought.Where(p => p.Value == characterData.UUID).Count();
                    if (mytotal == 0)
                    {
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ZeroTicketsLottery), 7000);
                        return;
                    }
                    double shans = mytotal * 100.0 / LotteryBought.Count();
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LotteryMy, mytotal, shans.ToString("0.##"), Wallet.Format(Price)), 7000);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.JackPotLottery), 7000);
                    if (shans >= 70.0) Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Chance70Lottery), 7000);
                    else if (shans >= 50.0) Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Chance50Lottery), 7000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_CheckLottery Exception: {e.ToString()}");
            }
        }
    }
}
