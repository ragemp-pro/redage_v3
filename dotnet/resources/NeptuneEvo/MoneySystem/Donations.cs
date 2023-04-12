using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Handles;
using MySqlConnector;
using NeptuneEvo.Core;
using Redage.SDK;
using Newtonsoft.Json;
using NeptuneEvo.Chars;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Players.Phone.Messages.Models;
using Redage.SDK.Models;

namespace NeptuneEvo.MoneySystem
{
    class Donations : Script
    {
        private static readonly nLog Log = new nLog("MoneySystem.Donations");
        private static Timer scanTimer;

        private static string SYNCSTR;

        private static string Connection;

        private static void LoadDonations()
        {
            Connection =
                $"Host={Main.DonateSettings.Server};" +
                $"User={Main.DonateSettings.User};" +
                $"Password={Main.DonateSettings.Password};" +
                $"Database={Main.DonateSettings.DataBase};" +
                $"SslMode=None;";
            
            SYNCSTR = $"select * from completed where srv={Main.ServerNumber}";
        }
        #region Работа с таймером
        public static void Start()
        {
            LoadDonations();
            
            scanTimer = new Timer(new TimerCallback(Tick), null, 1000 * 60, 1000 * 60);
        }

        public static void Stop()
        {
            scanTimer.Change(Timeout.Infinite, 0);
        }
        #endregion

        private static void CheckPromoDonate(string login, ulong reds, ExtPlayer player)
        {
            try
            {
                var accountData = player.GetAccountData();
                string dlogin = null;
                string promo = null;
                if (accountData == null)
                {
                    using MySqlCommand cmd = new MySqlCommand()
                    {
                        CommandText = $"SELECT promocodes FROM `accounts` WHERE `login`=@val0"
                    };
                    cmd.Parameters.AddWithValue("@val0", login);
                    using DataTable result = MySQL.QueryRead(cmd);
                    if (result != null)
                    {
                        promo = JsonConvert.DeserializeObject<List<string>>(result.Rows[0]["promocodes"].ToString())[0];
                        if (Main.PromoCodes.ContainsKey(promo) && Main.PromoCodes[promo].CreatorUUID != 0 && Main.PromoCodes[promo].DonatePercent > 0) dlogin = Main.PromoCodes[promo].DonateLogin;
                    }
                }
                else
                {
                    if (accountData.PromoCodes.Count >= 1 && !accountData.PromoCodes[0].Equals("noref"))
                    {
                        promo = accountData.PromoCodes[0];
                        if (Main.PromoCodes.ContainsKey(promo) && Main.PromoCodes[promo].CreatorUUID != 0 && Main.PromoCodes[promo].DonatePercent > 0) dlogin = Main.PromoCodes[promo].DonateLogin;
                    }
                }
                if (string.IsNullOrEmpty(promo) || promo == null || !Main.PromoCodes.ContainsKey(promo)) return;
                using MySqlCommand cmdPromocodes_new = new MySqlCommand
                {
                    CommandText = "UPDATE `promocodes_new` SET `donated`=`donated`+@val0 WHERE `promo`=@val1"
                };
                cmdPromocodes_new.Parameters.AddWithValue("@val0", reds);
                cmdPromocodes_new.Parameters.AddWithValue("@val1", promo);
                MySQL.Query(cmdPromocodes_new);
                if (string.IsNullOrEmpty(dlogin) || dlogin == null || dlogin.Equals("null")) return;
                reds = Convert.ToUInt64(reds * Main.PromoCodes[promo].DonatePercent);
                if (reds >= 1)
                {
                    Main.PromoCodes[promo].DonateReceivedByStreamer += reds;
                    ExtPlayer target = Accounts.Repository.GetPlayerToLogin(dlogin);
                    var targetAccountData = target.GetAccountData();
                    if (targetAccountData == null)
                    {
                        GameLog.AccountLog(dlogin, "-", "-", "-", LangFunc.GetText(LangType.Ru, DataName.RefTopUp, login, reds));
                        using MySqlCommand cmd = new MySqlCommand
                        {
                            CommandText = "update `accounts` set `redbucks`=`redbucks`+@val0 where `login`=@val1; UPDATE `promocodes_new` SET `donreceived`=`donreceived`+@val2 WHERE `promo`=@val3"
                        };
                        cmd.Parameters.AddWithValue("@val0", reds);
                        cmd.Parameters.AddWithValue("@val1", dlogin);
                        cmd.Parameters.AddWithValue("@val2", reds);
                        cmd.Parameters.AddWithValue("@val3", promo);
                        MySQL.Query(cmd);
                    }
                    else
                    {
                        UpdateData.RedBucks(target, (int)reds, msg: "Пополнение за реферала");
                        Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.RedAge, LangFunc.GetText(LangType.Ru, DataName.ComeRbFromRef, reds, login), DateTime.Now);
                        //Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ComeRbFromRef, reds, login), 3000);
                        using MySqlCommand cmd = new MySqlCommand
                        {
                            CommandText = "update `accounts` set `redbucks`=@val0 where `login`=@val1; UPDATE `promocodes_new` SET `donreceived`=`donreceived`+@val2 WHERE `promo`=@val3"
                        };
                        cmd.Parameters.AddWithValue("@val0", targetAccountData.RedBucks);
                        cmd.Parameters.AddWithValue("@val1", targetAccountData.Login);
                        cmd.Parameters.AddWithValue("@val2", reds);
                        cmd.Parameters.AddWithValue("@val3", promo);
                        MySQL.Query(cmd);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"CheckPromoDonate Exception: {e.ToString()}");
            }
        }

        #region Проверка никнеймов и донатов
        public static void Tick(object state)
        {
            try
            {
                if (!Main.DonateSettings.IsCheck)
                    return;
                
                Log.Debug($"Donate time");
                using (MySqlConnection connection = new MySqlConnection(Connection))
                {
                    connection.Open();
                    using MySqlCommand command = new MySqlCommand();
                    command.Connection = connection;
                    command.CommandText = SYNCSTR;
                    MySqlDataReader reader = command.ExecuteReader();
                    using DataTable result = new DataTable();
                    result.Load(reader);
                    reader.Close();
                    foreach (DataRow Row in result.Rows)
                    {
                        int id = Convert.ToInt32(Row["id"]);
                        string name = Convert.ToString(Row["account"]).ToLower();
                        long reds = Convert.ToInt64(Row["amount"]);
                        //
                        Utils.Analytics.HelperThread.AddEvent("donate", Accounts.Repository.GetEmailToLogin(name), "", (int)reds);
                        //
                        
                        try
                        {                       
                            if (!Main.Usernames.ContainsKey(name))
                            {
                                Log.Write($"Can't find registred name for {name}!", nLog.Type.Warn);
                                continue;
                            }
                            var player = Accounts.Repository.GetPlayerToLogin(name);
                            CheckPromoDonate(name, (ulong)reds, player);
                        
                            if (!DonatePack.IsDonate(player, name.ToLower(), (int)reds))
                            {
                                if (Main.DonateSettings.IsSaleEnable) 
                                    reds = SaleEvent(reds);
                                else if (Main.DonateSettings.Multiplier > 1) 
                                    reds = reds * Main.DonateSettings.Multiplier;
                                
                                var accountData = player.GetAccountData();
                                if (accountData == null)
                                {
                                    GameLog.AccountLog(name, "-", "-", "-", $"Пополнение (+{reds} RedBucks)");
                                    using MySqlCommand cmd = new MySqlCommand
                                    {
                                        CommandText = "update `accounts` set `redbucks`=`redbucks`+@val0, `ReceivedAwardDonate`=`ReceivedAwardDonate`+@val0 where `login`=@val1"
                                    };
                                    cmd.Parameters.AddWithValue("@val0", reds);
                                    cmd.Parameters.AddWithValue("@val1", name);
                                    MySQL.Query(cmd);
                                }
                                else
                                {
                                    UpdateData.RedBucks(player, (int)reds, msg: "Пополнение");
                                    accountData.ReceivedAwardDonate += (int)reds;
                                    using MySqlCommand cmd = new MySqlCommand
                                    {
                                        CommandText = "update `accounts` set `redbucks`=@val0 where `login`=@val1"
                                    };
                                    cmd.Parameters.AddWithValue("@val0", accountData.RedBucks);
                                    cmd.Parameters.AddWithValue("@val1", accountData.Login);
                                    MySQL.Query(cmd);
                                    //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вам пришли {reds} RedBucks", 3000);
                                    Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.RedAge, LangFunc.GetText(LangType.Ru, DataName.UvGotReds, reds), DateTime.Now);
                                }
                            }
                            command.CommandText = $"delete from completed where id={id}";
                            command.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Log.Write($"Tick Foreach Exception: {e.ToString()}");
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Log.Write($"Tick Exception: {e.ToString()}");
            }
        }
        #endregion

        #region Действия в донат-меню
        internal enum Type
        {
            Character,
            Nickname,
            Convert,
            SilverVIP,
            GoldVIP,
            PlatinumVIP,
            DiamondVIP,
            Warn,
            Slot,
            BagStart,
            BagAdvanced,
            BagCharged,
            BagBigPlans,
            BagFatCheck,
            BagLegend,
            BagDaddy,
            BagGoodStart,
            BagExcellentStart,
            BagPerfectStart,
            BagOnFoot,
        }
        [RemoteEvent("giftdonate")]
        public void GiftDonate(ExtPlayer player, string login, int sum)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                login = login.ToLower();
                if (login.Equals(accountData.Login)) return;
                if (sum < 25 || sum > 9999)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RbTransferLimit), 3000);
                    return;
                }
                if (accountData.RedBucks < sum)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);
                    return;
                }
                if (DateTime.Now < sessionData.TimingsData.NextGiftDonate)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Cooldown5min), 3000);
                    return;
                }
                if (characterData.AdminLVL <= 5) sessionData.TimingsData.NextGiftDonate = DateTime.Now.AddMinutes(5);
                else sessionData.TimingsData.NextGiftDonate = DateTime.Now.AddSeconds(10);
                using MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "SELECT redbucks FROM `accounts` WHERE `login`=@lg"
                };
                cmd.Parameters.AddWithValue("@lg", login);
                using DataTable result = MySQL.QueryRead(cmd);
                if (result == null || result.Rows.Count == 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RecoveryCantFind), 3000);
                    return;
                }
                int targetrb = Convert.ToInt32(result.Rows[0]["redbucks"]);
                UpdateData.RedBucks(player, -sum, msg: $"Подарок {login}");

                using MySqlCommand cmdAccounts = new MySqlCommand
                {
                    CommandText = "UPDATE `accounts` SET `redbucks`=@val0 WHERE `login`=@val1; UPDATE `accounts` SET `redbucks`=@val2 WHERE `login`=@val3"
                };
                cmdAccounts.Parameters.AddWithValue("@val0", accountData.RedBucks);
                cmdAccounts.Parameters.AddWithValue("@val1", accountData.Login);
                ExtPlayer target = Accounts.Repository.GetPlayerToLogin(login);
                var targetAccountData = target.GetAccountData();
                if (targetAccountData == null)
                {
                    UpdateData.RedBucks(target, sum, msg: $"Подарок от {accountData.Login}");
                    cmdAccounts.Parameters.AddWithValue("@val2", targetAccountData.RedBucks);
                    cmdAccounts.Parameters.AddWithValue("@val3", targetAccountData.Login);
                    Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили подарок: {sum} RedBucks от {player.Name.Replace('_', ' ')} ({player.Value})!", 5000);
                }
                else
                {

                    targetrb += sum;
                    cmdAccounts.Parameters.AddWithValue("@val2", targetrb);
                    cmdAccounts.Parameters.AddWithValue("@val3", login);
                }
                MySQL.Query(cmdAccounts);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucGiftRb, sum, login), 5000);
            }
            catch (Exception e)
            {
                Log.Write($"GiftDonate Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("donate")]
        public void MakeDonate(ExtPlayer player, int id, string data)
        {
            try
            {
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                Type type = (Type)id;
                if (type != Type.Slot && !player.IsCharacterData()) return;
                switch (type)
                {
                    case Type.Slot:
                        {
                            int slotId = 0;
                            if (!Int32.TryParse(data, out slotId))
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RecoveryError), 3000);
                                return;
                            }
                            slotId -= 1;
                            if (accountData.Chars[slotId] != -2)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Already3Slot), 3000);
                                return;
                            }
                            if (accountData.RedBucks < 1000)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);
                                return;
                            }
                            UpdateData.RedBucks(player, -1000, msg: LangFunc.GetText(LangType.Ru, DataName.Buy3Slot));
                            if (accountData.VipLvl <= 3)
                            {
                                accountData.VipDate = (accountData.VipLvl == 0) ? DateTime.Now.AddDays(30) : accountData.VipDate.AddDays(30);
                                accountData.VipLvl = 3;
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlatinumVIP30d), 3000);
                            }
                            else
                            {
                                accountData.VipDate = accountData.VipDate.AddDays(7);
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlattoVip7d), 5000);
                            }

                            accountData.Chars[slotId] = -1;
                            Trigger.ClientEvent(player, "unlockSlot", slotId);

                            using MySqlCommand cmd1 = new MySqlCommand
                            {
                                CommandText = "update `accounts` set `redbucks`=@val0 where `login`=@val1"
                            };
                            cmd1.Parameters.AddWithValue("@val0", accountData.RedBucks);
                            cmd1.Parameters.AddWithValue("@val1", accountData.Login);
                            MySQL.Query(cmd1);
                            return;
                        }
                    default:
                        // Not supposed to end up here. 
                        break;
                }

                using MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "update `accounts` set `redbucks`=@val0 where `login`=@val1"
                };
                cmd.Parameters.AddWithValue("@val0", accountData.RedBucks);
                cmd.Parameters.AddWithValue("@val1", accountData.Login);
                MySQL.Query(cmd);
            }
            catch (Exception e)
            {
                Log.Write($"MakeDonate Exception: {e.ToString()}");
            }
        }
        #endregion

        public static long SaleEvent(long input)
        {
            if (input < 1000) return input;
            if (input < 3000) return input + (input / 100 * 10);
            if (input < 10000) return input + (input / 100 * 20);
            if (input < 20000) return input + (input / 100 * 30);
            if (input >= 20000) return input + (input / 100 * 50);
            return input;
        }
    }
}
