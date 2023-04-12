using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Fractions;
using Redage.SDK;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Localization;
using NeptuneEvo.Core;
using NeptuneEvo.Quests;

namespace NeptuneEvo.Chars
{
    class UpdateData : Script
    {
        /// <summary>
        /// Логгер
        /// </summary>
        private static readonly nLog Log = new nLog("Chars.UpdateData");
        public static void RedBucks(ExtPlayer player, int value, string msg)
        {
            try
            {
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                accountData.RedBucks += value;
                
                Database.Models.Money.AddDonateUpdate(accountData.Login, accountData.RedBucks);
                
                Trigger.ClientEvent(player, "client.accountStore.Redbucks", accountData.RedBucks); 
                
                if (msg != String.Empty)
                    GameLog.AccountLog(accountData.Login, accountData.HWID, accountData.IP, accountData.SocialClub, $"{msg} ({value} RedBucks)");
            }
            catch (Exception e)
            {
                Log.Write($"RedBucks Exception: {e.ToString()}");
            }
        }
        public static byte CanIChange(ExtPlayer player, int value, bool errortext = false)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return 0;

                //if (Main.ServerNumber != 0 && (characterData.AdminLVL >= 1 && characterData.AdminLVL <= 6))
                //{
                //    if(errortext) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Подобные денежные операции недоступны для администрации.", 3000);
                //    return 1;
                //}
                if (Core.Admin.IsServerStoping)
                {
                    if (errortext) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ServRestarting), 3000);
                    return 2;
                }
                if (0 > value)
                    return 3;
                
                if (characterData.Money < value)
                {
                    if (errortext) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMoneyForIt, MoneySystem.Wallet.Format(value - characterData.Money)), 5000);
                    return 3;
                }

                return 255; // Everything is good
            }
            catch (Exception e)
            {
                Log.Write($"Money Exception: {e.ToString()}");
                return 0;
            }
        }
        public static void Work(ExtPlayer player, int value)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
                
                if (characterData.WorkID == value) 
                    return;
                
                characterData.WorkID = value;
                
                if (sessionData.WorkData != null)
                    sessionData.WorkData.PointsCount = 0;
                
                Trigger.ClientEvent(player, "client.charStore.WorkID", value);
            }
            catch (Exception e)
            {
                Log.Write($"WorkId Exception: {e.ToString()}");
            }
        }
        public static void Level(ExtPlayer player, int value)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                characterData.LVL += value;
                if (characterData.LVL == 1) Trigger.DamageDisable(player, false);
                else if (characterData.LVL == 0) Trigger.DamageDisable(player, true);
            }
            catch (Exception e)
            {
                Log.Write($"Level Exception: {e.ToString()}");
            }
        }
        public static void Exp(ExtPlayer player, int value, int salary = 0)
        {
            try
            {
                var accountData = player.GetAccountData();
                if (accountData == null) 
                    return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
                
                //int wasexp = characterData.EXP;
                characterData.EXP += value;
                if (characterData.EXP >= 3 + characterData.LVL * 3)
                {
                    if (characterData.LVL == 5) 
                        player.SetSharedData("NewUser", false);
                    
                    characterData.EXP = characterData.EXP - (3 + characterData.LVL * 3);
                    Level(player, 1);
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NewLvl, characterData.LVL), 15000);
                    Utils.Analytics.HelperThread.AddEvent("levelup", accountData.Email, accountData.Ga, characterData.LVL);
                    //Trigger.ClientEvent(player, "ExpUP", 999999, characterData.EXP, characterData.LVL, (3 + characterData.LVL * 3));
                } 
                //else Trigger.ClientEvent(player, "payday", wasexp, characterData.EXP, characterData.LVL, (3 + characterData.LVL * 3));
                Trigger.ClientEvent(player, "payday", characterData.EXP, characterData.LVL, salary);
            }
            catch (Exception e)
            {
                Log.Write($"Exp Exception: {e.ToString()}");
            }
        }
        public static uint GetPlayerDimension(ExtPlayer player)
        {
            try
            {
                if (player == null) return 0;
                if (Thread.CurrentThread.Name != "Main")
                {
                    SessionData sessionData = player.GetSessionData();
                    if (sessionData != null) return sessionData.Dimension;
                }
                return player.Dimension;
            }
            catch (Exception e)
            {
                Main.Log.Write($"GetPlayerDimension Exception: {e.ToString()}");                
            }
            return 0;
        }
        // НЕТ БЕЗОПАСНОСТИ ПОТОКОВ, При неправильном использовании сервер может падать
        public static uint GetVehicleDimension(ExtVehicle vehicle)
        {
            try
            {
                if (Thread.CurrentThread.Name != "Main") return 0;
                if (vehicle == null) return 0;
                return vehicle.Dimension;
            }
            catch (Exception e)
            {
                Main.Log.Write($"GetVehicleDimension Exception: {e.ToString()}");
            }
            return 0;
        }
    }
}
