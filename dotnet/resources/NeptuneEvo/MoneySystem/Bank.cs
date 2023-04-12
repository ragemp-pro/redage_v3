using GTANetworkAPI;
using NeptuneEvo.Handles;
using System;
using System.Collections.Generic;
using NeptuneEvo.Core;
using Redage.SDK;
using System.Data;
using System.Linq;
using Newtonsoft.Json;
using MySqlConnector;
using Database;
using LinqToDB;
using System.Threading.Tasks;
using NeptuneEvo.Functions;
using System.Collections.Concurrent;
using GTANetworkMethods;
using Localization;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Players.Phone.Messages.Models;
using Task = System.Threading.Tasks.Task;

namespace NeptuneEvo.MoneySystem
{
    class Bank : Script
    {
        public static readonly nLog Log = new nLog("MoneySystem.Bank");
        private static Random Rnd = new Random();

        public static ConcurrentDictionary<int, Data> Accounts = new ConcurrentDictionary<int, Data>();

        public enum BankNotifyType
        {
            PaySuccess,
            PayIn,
            PayOut,
            PayError,
            InputError,
        }
        public static async void Init()
        {
            try
            {
                Log.Write("Loading Bank Accounts...");

                await using var db = new ServerBD("MainDB");//При старте

                var banks = await db.Money
                    .ToListAsync();
                
                foreach (var bank in banks)
                {
                    var data = new Data();
                    data.ID = Convert.ToInt32(bank.Id);
                    data.Type = Convert.ToInt32(bank.Type);
                    data.Holder = bank.Holder;
                    data.Balance = Convert.ToInt64(bank.Balance);
                    Accounts.TryAdd(data.ID, data);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Bank Exception: {e.ToString()}");
            }
        }

        public static long GetBalance(int bankId)
        {

            if (Accounts.ContainsKey(bankId))
                return Accounts[bankId].Balance;

            return 0;
        }
        #region Changing account balance
        public static bool Change(int bankId, long amount, bool notify = true)
        {
            try
            {
                if (!Accounts.ContainsKey(bankId)) 
                    return false;
                
                var bank = Accounts[bankId];
                if (bank.Balance + amount < 0)
                    return false;
                
                if (amount < 0 && Admin.IsServerStoping)
                    return false;
                
                bank.Balance += amount;

                if (bank.Type == 1 && amount >= 10000)
                    bank.IsSave = true;
                else if (bank.Type != 1)
                    bank.IsSave = true;

                if (bank.Type == 1)
                {
                    var target = (ExtPlayer) NAPI.Player.GetPlayerFromName(bank.Holder);
                    if (target != null)
                    {
                        if (notify)
                        {
                            if (amount > 0) BankNotify(target, BankNotifyType.PayIn, amount.ToString());
                            else BankNotify(target, BankNotifyType.PayOut, amount.ToString());
                        }
                        Trigger.ClientEvent(target, "client.charStore.BankMoney", bank.Balance);
                    }
                    else
                        bank.IsSave = true;
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Write($"Change Exception: {e.ToString()}");
                return false;
            }
        }
        #endregion Changing account balance
        #region Transfer money from 1-Acc to 2-Acc
        public static bool Transfer(int firstAccID, int lastAccID, long amount)
        {
            try
            {
                if (firstAccID == 0 || lastAccID == 0)
                {
                    Log.Write($"Account ID error [{firstAccID}->{lastAccID}]", nLog.Type.Error);
                    return false;
                }
                Data firstAcc = Accounts[firstAccID];
                if (!Accounts.ContainsKey(lastAccID))
                {
                    if (firstAcc.Type == 1)
                        BankNotify((ExtPlayer) NAPI.Player.GetPlayerFromName(firstAcc.Holder), BankNotifyType.InputError, "Такого счета не существует!");
                    Log.Write($"Transfer with error. Account does not exist! [{firstAccID.ToString()}->{lastAccID.ToString()}:{amount.ToString()}]", nLog.Type.Warn);
                    return false;
                }
                if (!Change(firstAccID, -amount))
                {
                    if (firstAcc.Type == 1)
                        BankNotify((ExtPlayer) NAPI.Player.GetPlayerFromName(firstAcc.Holder), BankNotifyType.PayError, "Недостаточно средств!");
                    Log.Write($"Transfer with error. Insufficient funds! [{firstAccID.ToString()}->{lastAccID.ToString()}:{amount.ToString()}]", nLog.Type.Warn);
                    return false;
                }
                Change(lastAccID, amount);
                ExtPlayer target = (ExtPlayer) NAPI.Player.GetPlayerFromName(Accounts[lastAccID].Holder);
                if (target != null) target.Eval($"mp.game.audio.playSoundFrontend(-1, \"LOCAL_PLYR_CASH_COUNTER_COMPLETE\", \"DLC_HEISTS_GENERAL_FRONTEND_SOUNDS\", true);");
                
                GameLog.Money($"bank({firstAccID})", $"bank({lastAccID})", amount, "bankTransfer");
                return true;
            }
            catch (Exception e)
            {
                Log.Write($"Transfer Exception: {e.ToString()}");
                return false;
            }
        }
        #endregion Transfer money from 1-Acc to 2-Acc
        #region Save Acc
        
        public static void SetSave(int bankId)
        {
            if (Accounts.ContainsKey(bankId))
                Accounts[bankId].IsSave = true;
        }
        public static async Task Save(ServerBD db, int bankId)
        {
            try
            {
                if (!Accounts.ContainsKey(bankId)) return;

                var bank = Accounts[bankId];
                bank.IsSave = false;
                    
                var updateBank = db.Money
                    .Where(m => m.Id == bankId)
                    .Set(m => m.Balance, Convert.ToInt32(bank.Balance));

                if (bank.IsSaveHolder)
                    updateBank = updateBank.Set(b => b.Holder, bank.Holder);
                
                bank.IsSaveHolder = false;
                        
                await updateBank
                    .UpdateAsync();
            }
            catch (Exception e)
            {
                Log.Write($"Save Exception: {e.ToString()}");
            }
        }
        #endregion Save Acc

        public static void BankNotify(ExtPlayer player, BankNotifyType type, string info)
        {
            try
            {
                switch (type)
                {
                    case BankNotifyType.InputError:
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ErrorInput), 3000);
                        return;
                    case BankNotifyType.PayError:
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ErrorWithdraw), 3000);
                        return;
                    case BankNotifyType.PayIn:
                        int money = Convert.ToInt32(info);
                        //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MoneyIncome, Wallet.Format(money)), 3000);
                        Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Bank, LangFunc.GetText(LangType.Ru, DataName.MoneyIncome, Wallet.Format(money)), DateTime.Now);
                        if (money >= 10)
                            BattlePass.Repository.UpdateReward(player, 5);
                        return;
                    case BankNotifyType.PayOut:
                        money = Convert.ToInt32(info);
                        //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MoneyOutcome, Wallet.Format(money)), 3000);
                        Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Bank, LangFunc.GetText(LangType.Ru, DataName.MoneyOutcome, Wallet.Format(money)), DateTime.Now);
                        return;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"BankNotify Exception: {e.ToString()}");
            }
        }

        public static async Task<int> Create(string holder, int type = 1, long balance = 0)
        {
            try
            {
                await using var db = new ServerBD("MainDB");//В отдельном потоке

                int moneyId = await db.InsertWithInt32IdentityAsync(new Moneys
                {
                    Type = (sbyte)type,
                    Holder = holder,
                    Balance = (int)balance,
                });

                Data data = new Data();
                data.ID = moneyId;
                data.Type = type;
                data.Holder = holder;
                data.Balance = balance;
                Accounts.TryAdd(moneyId, data);

                Log.Write($"Created new Bank Account! ID:{moneyId}", nLog.Type.Success);
                return moneyId;
            }
            catch (Exception e)
            {
                Log.Write($"Create Exception: {e.ToString()}");
                return 0;
            }
        }
        public static void Remove(int bankId)
        {
            try
            {
                if (!Accounts.ContainsKey(bankId))
                    return;
                Accounts.TryRemove(bankId, out _);
                
                Database.Models.Bank.OnDell(bankId);
                
                Log.Write("Bank account deleted! ID:" + bankId, nLog.Type.Warn);
            }
            catch (Exception e)
            {
                Log.Write($"Remove Exception: {e.ToString()}");
            }
        }
        public static Data Get(string holder)
        {
            return Accounts.FirstOrDefault(A => A.Value.Holder == holder).Value;
        }

        public static Data Get(int bankId)
        {
            if (Accounts.ContainsKey(bankId))
                return Accounts[bankId];
            
            return null;
        }

        public static void changeHolder(string oldName, string newName)
        {
            try
            {
                var toChange = Accounts
                    .Where(b => b.Value.Holder == oldName)
                    .Select(b => b.Key)
                    .ToList();

                foreach (int id in toChange)
                {
                    if (Accounts.ContainsKey(id))
                    {
                        Accounts[id].Holder = newName;
                        Accounts[id].IsSaveHolder = true;
                        Accounts[id].IsSave = true;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"changeHolder Exception: {e.ToString()}");
            }
        }

        internal class Data
        {
            public int ID { get; set; }
            public int Type { get; set; }
            public string Holder { get; set; }
            public long Balance { get; set; }
            public bool IsSave { get; set; }
            public bool IsSaveHolder { get; set; }
        }
    }

    class ATM : Script
    {

        #region ATMs List
        public static List<Vector3> ATMs = new List<Vector3>
        {
            new Vector3(-30.28312, -723.7054, 43.10828),
            new Vector3(-846.4784, -340.7381, 37.56028),
            new Vector3(-30.28312, -723.7054, 43.10828),
            new Vector3(-57.79301, -92.57375, 56.65908),
            new Vector3(-203.8796, -861.4044, 29.14762),
            new Vector3(-301.6998, -830.0975, 31.29726),
            new Vector3(-1315.741, -834.8119, 15.84172),
            new Vector3(-526.7958, -1222.796, 17.33497),
            new Vector3(-165.068, 232.6937, 93.80193),
            new Vector3(147.585, -1035.683, 28.22313),
            new Vector3(-2072.433, -317.1329, 12.19597),
            new Vector3(112.6747, -819.3305, 30.21771),
            new Vector3(111.1934, -775.319, 30.31857),
            new Vector3(-254.3221, -692.4096, 32.49045),
            new Vector3(-256.154, -716.0692, 32.39723),
            new Vector3(-258.849, -723.3128, 32.36183),
            new Vector3(-537.8723, -854.4181, 28.16625),
            new Vector3(-594.6927, -1161.374, 21.20427),
            new Vector3(1167.063, -456.2611, 65.6659),
            new Vector3(1138.276, -469.0832, 65.60734),
            new Vector3(-821.5346, -1081.945, 10.01243),
            new Vector3(527.2645, -161.3371, 55.95051), // 30
            new Vector3(158.4433, 234.1823, 105.5114),
            new Vector3(-56.88515, -1752.214, 28.30102),
            new Vector3(33.25563, -1348.147, 28.37702),
            new Vector3(89.62029, 2.412876, 67.18955),
            new Vector3(-1410.304, -98.57402, 51.31698),
            new Vector3(288.7548, -1282.287, 28.52028),
            new Vector3(-1212.692, -330.7367, 36.66656),
            new Vector3(-1205.556, -325.066, 36.73424),
            new Vector3(-611.844, -704.7563, 30.11593),
            new Vector3(-867.6541, -186.0634, 36.72196),
            new Vector3(289.0122, -1256.787, 28.32075), // 50
            new Vector3(-1305.292, -706.3788, 24.20243),
            new Vector3(-1570.267, -546.7006, 33.83642),
            new Vector3(-1430.069, -211.1082, 45.37187),
            new Vector3(-1416.06, -212.0282, 45.38037),
            new Vector3(-1109.778, -1690.661, 3.255033),
            new Vector3(1153.742, -326.8381, 69.08506),
            new Vector3(296.4094, -894.2438, 28.4),
            new Vector3(74.07938, -218.9531, 53.6),
            new Vector3(285.4006, 143.6701, 103.1),
            new Vector3(380.7721, 323.4297, 102.6),
            new Vector3(-721.1078, -415.4442, 34.0),
            new Vector3(1077.621, -776.4483, 57.2),
            new Vector3(-1827.272, 784.8806, 137.2),
            new Vector3(-273.0953, -2024.532, 29.1),
            new Vector3(300.42, -588.85, 43.26),
            new Vector3(-1649.4552, -815.45593, 10.197399),
            new Vector3(920.234, 40.72909, 81.09598),
            new Vector3(751.941, -913.6929, 24.16644),
            new Vector3(468.4216, -990.5975, 26.273373),
            new Vector3(-1285.7714, -572.65314, 30f),
            new Vector3(-433.14172, 265.18628, 83f),
            new Vector3(-717.572, -915.6549, 19f),//new
        };
        #endregion ATMs List

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                for (int i = 0; i < ATMs.Count; i++)
                {
                    if (i != 58) Main.CreateBlip(new Main.BlipData(500, "Банкомат", ATMs[i], 7, true, 0.32f));
                    CustomColShape.CreateCylinderColShape(ATMs[i], 1, 2, 0, ColShapeEnums.Atm, i);
                }
                Bank.Log.Write("ATMs loaded", nLog.Type.Success);
            }
            catch (Exception e)
            {
                Bank.Log.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }

        [Interaction(ColShapeEnums.Atm)]
        public static void OnAtm(ExtPlayer player, int _)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.DemorganTime >= 1) return;
                if (characterData.WantedLVL != null && characterData.WantedLVL.Level > 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WantedNOATM), 3000);
                    return;
                }
                if (characterData.Bank == 0 || !Bank.Accounts.ContainsKey(characterData.Bank))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoBanks), 3000);
                    return;
                }
                Trigger.ClientEvent(player, "setatm", characterData.Bank.ToString(), player.Name, Bank.GetBalance(characterData.Bank).ToString(), "");
                Trigger.ClientEvent(player, "openatm");
                //Trigger.PlayAnimation(player, "amb@prop_human_atm@female@enter", "enter", 2);
                BattlePass.Repository.UpdateReward(player, 39);
            }
            catch (Exception e)
            {
                Bank.Log.Write($"OpenATM Exception: {e.ToString()}");
            }
        }

        public static void AtmBizGen(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                Bank.Log.Debug("Biz count : " + characterData.BizIDs.Count);
                if (characterData.BizIDs.Count > 0)
                {
                    List<string> _Data = new List<string>();
                    foreach (int key in characterData.BizIDs)
                    {
                        Business biz = BusinessManager.BizList[key];
                        string name = BusinessManager.BusinessTypeNames[biz.Type];
                        _Data.Add($"{name}");
                    }
                    Trigger.ClientEvent(player, "atmOpenBiz", JsonConvert.SerializeObject(_Data), "");
                }
                else
                {
                    Trigger.ClientEvent(player, "atmOpen", "[1,0,0]");
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoBusiness), 3000);
                }
            }
            catch (Exception e)
            {
                Bank.Log.Write($"AtmBizGen Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("atmVal")]
        public static void ClientEvent_ATMVAL(ExtPlayer player, params object[] args)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (Admin.IsServerStoping)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ServerCant), 3000);
                    return;
                }
                int type = sessionData.ATMData.Type;
                string data = Convert.ToString(args[0]);
                int amount;
                int myamount;
                if (!int.TryParse(data, out amount)) return;
                switch (type)
                {
                    case 0:
                        Trigger.ClientEvent(player, "atmClose");
                        myamount = Math.Abs(amount);
                        if (myamount <= 0) return;
                        if (myamount > characterData.Money) myamount = (int)characterData.Money;
                        if (myamount > 0)
                        {
                            if (Wallet.Change(player, -myamount))
                            {
                                Bank.Change(characterData.Bank, +myamount);
                                player.Eval($"mp.game.audio.playSoundFrontend(-1, \"Bus_Schedule_Pickup\", \"DLC_PRISON_BREAK_HEIST_SOUNDS\", true);");
                                if (characterData.AdminLVL <= 7) GameLog.Money($"player({characterData.UUID})", $"bank({characterData.Bank})", myamount, $"atmIn");
                            }
                        }
                        break;
                    case 1:
                        myamount = Math.Abs(amount);
                        if (myamount <= 0) return;
                        if (myamount > Bank.GetBalance(characterData.Bank)) 
                            myamount = (int)Bank.GetBalance(characterData.Bank);
                        if (myamount > 0)
                        {
                            if (Bank.Change(characterData.Bank, -myamount))
                            {
                                player.Eval($"mp.game.audio.playSoundFrontend(-1, \"Bus_Schedule_Pickup\", \"DLC_PRISON_BREAK_HEIST_SOUNDS\", true);");
                                Wallet.Change(player, +myamount);
                                if (characterData.AdminLVL <= 7) GameLog.Money($"bank({characterData.Bank})", $"player({characterData.UUID})", myamount, $"atmOut");
                            }
                        }
                        break;
                    case 2:
                        var house = Houses.HouseManager.GetHouse(player, true);
                        if (house == null) return;

                        int maxMoney = 0;

                        switch (accountData.VipLvl)
                        {
                            case 0:
                            case 1:
                                maxMoney = Convert.ToInt32(house.Price / 100 * 0.026 * 24 * 7); // No-VIP / Bronze
                                break;
                            case 2:
                                maxMoney = Convert.ToInt32(house.Price / 100 * 0.026 * 24 * 14); // Silver
                                break;
                            case 3:
                                maxMoney = Convert.ToInt32(house.Price / 100 * 0.026 * 24 * 21); // Gold
                                break;  
                            case 4: // Platinum
                            case 5: // Media Platinum
                                maxMoney = Convert.ToInt32(house.Price / 100 * 0.026 * 24 * 28);
                                break;
                            default:
                                maxMoney = Convert.ToInt32(house.Price / 100 * 0.026 * 24 * 7); // Error Exception
                                break;
                        }

                        myamount = Math.Abs(amount);
                        if (myamount <= 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CanNotTransact), 3000);
                            return;
                        }
                        var bankBalance = Bank.GetBalance(house.BankID);
                        if (bankBalance + myamount > maxMoney)
                            myamount = maxMoney - (int)bankBalance;
                        if (myamount <= 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CanNotTransact), 3000);
                            return;
                        }
                        if (!Wallet.Change(player, -myamount))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMoney), 3000);
                            return;
                        }
                        Bank.Change(house.BankID, +myamount);
                        GameLog.Money($"player({characterData.UUID})", $"bank({house.BankID})", myamount, $"atmHouse");
                        //BattlePass.Repository.UpdateReward(player, 41);
                        //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucTransact), 3000);
                        Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Bank, LangFunc.GetText(LangType.Ru, DataName.SucTransactH, myamount, (bankBalance+myamount), maxMoney), DateTime.Now);

                        switch (accountData.VipLvl)
                        {
                            case 0:
                            case 1:
                                Trigger.ClientEvent(player, "atmOpen", $"[2,'{bankBalance}/{Convert.ToInt32(house.Price / 100 * 0.026 * 24 * 7)}$','Сумма внесения наличных']");
                                break;
                            case 2:
                                Trigger.ClientEvent(player, "atmOpen", $"[2,'{bankBalance}/{Convert.ToInt32(house.Price / 100 * 0.026 * 24 * 14)}$','Сумма внесения наличных']");
                                break;
                            case 3:
                                Trigger.ClientEvent(player, "atmOpen", $"[2,'{bankBalance}/{Convert.ToInt32(house.Price / 100 * 0.026 * 24 * 21)}$','Сумма внесения наличных']");
                                break;
                            case 4: // Platinum
                            case 5: // Media Platinum
                                Trigger.ClientEvent(player, "atmOpen", $"[2,'{bankBalance}/{Convert.ToInt32(house.Price / 100 * 0.026 * 24 * 28)}$','Сумма внесения наличных']");
                                break;
                            default:
                                Trigger.ClientEvent(player, "atmOpen", $"[2,'{bankBalance}/{Convert.ToInt32(house.Price / 100 * 0.026 * 24 * 7)}$','Сумма внесения наличных']");
                                break;
                        }
                        break;
                    case 3:
                        int bid = sessionData.ATMData.BizID;
                        Business biz = BusinessManager.BizList[characterData.BizIDs[bid]];

                        maxMoney = 0;

                        switch (accountData.VipLvl)
                        {
                            case 0:
                            case 1:
                                maxMoney = Convert.ToInt32(biz.SellPrice / 100 * biz.Tax * 24 * 7); // No-VIP / Bronze
                                break;
                            case 2:
                                maxMoney = Convert.ToInt32(biz.SellPrice / 100 * biz.Tax * 24 * 14); // Silver
                                break;
                            case 3:
                                maxMoney = Convert.ToInt32(biz.SellPrice / 100 * biz.Tax * 24 * 21); // Gold
                                break;
                            case 4: // Platinum
                            case 5: // Media Platinum
                                maxMoney = Convert.ToInt32(biz.SellPrice / 100 * biz.Tax * 24 * 28);
                                break;
                            default:
                                maxMoney = Convert.ToInt32(biz.SellPrice / 100 * biz.Tax * 24 * 7); // Error Exception
                                break;
                        }
                        myamount = Math.Abs(amount);
                        if (myamount <= 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CanNotTransact), 3000);
                            return;
                        }

                        bankBalance = Bank.GetBalance(biz.BankID);
                        if (bankBalance + myamount > maxMoney) 
                            myamount = maxMoney - (int)bankBalance;
                        
                        if (myamount <= 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CanNotTransact), 3000);
                            return;
                        }
                        if (!Wallet.Change(player, -myamount))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMoney), 3000);
                            return;
                        }
                        Bank.Change(biz.BankID, +myamount);
                        GameLog.Money($"player({characterData.UUID})", $"bank({biz.BankID})", myamount, $"atmBiz");
                        //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucTransact), 3000);
                        Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Bank, LangFunc.GetText(LangType.Ru, DataName.SucTransactB, myamount, (bankBalance+myamount), maxMoney), DateTime.Now);
                        switch (accountData.VipLvl)
                        {
                            case 0:
                            case 1:
                                Trigger.ClientEvent(player, "atmOpen", $"[2,'{bankBalance}/{Convert.ToInt32(biz.SellPrice / 100 * biz.Tax * 24 * 7)}$','Сумма зачисления']");
                                break;
                            case 2:
                                Trigger.ClientEvent(player, "atmOpen", $"[2,'{bankBalance}/{Convert.ToInt32(biz.SellPrice / 100 * biz.Tax * 24 * 14)}$','Сумма зачисления']");
                                break;
                            case 3:
                                Trigger.ClientEvent(player, "atmOpen", $"[2,'{bankBalance}/{Convert.ToInt32(biz.SellPrice / 100 * biz.Tax * 24 * 21)}$','Сумма зачисления']");
                                break;
                            case 4: // Platinum
                            case 5: // Media Platinum
                                Trigger.ClientEvent(player, "atmOpen", $"[2,'{bankBalance}/{Convert.ToInt32(biz.SellPrice / 100 * biz.Tax * 24 * 28)}$','Сумма зачисления']");
                                break;
                            default:
                                Trigger.ClientEvent(player, "atmOpen", $"[2,'{bankBalance}/{Convert.ToInt32(biz.SellPrice / 100 * biz.Tax * 24 * 7)}$','Сумма зачисления']");
                                break;
                        }
                        break;
                    case 4:
                        if (!Bank.Accounts.ContainsKey(amount) || amount <= 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindBankAccount), 3000);
                            Trigger.ClientEvent(player, "closeatm");
                            Trigger.StopAnimation(player);
                            return;
                        }
                        sessionData.ATMData.Amount = amount; // ATM2ACC
                        Trigger.ClientEvent(player, "atmOpen", "[2,0,'Сумма для перевода']");
                        sessionData.ATMData.Type = 44;
                        break;
                    case 44:
                        if (!FunctionsAccess.IsWorking("atmtransfer"))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                            return;
                        }
                        if (characterData.LVL < 1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.flvltotransact), 3000);
                            return;
                        }   
                        if (DateTime.Now < sessionData.TimingsData.NextBankTransfer)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NextTransactionSoon), 3000);
                            return;
                        }
                        int bank = sessionData.ATMData.Amount;
                        if (!Bank.Accounts.ContainsKey(bank) || bank <= 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindBankAccount), 3000);
                            Trigger.ClientEvent(player, "closeatm");
                            Trigger.StopAnimation(player);
                            return;
                        }
                        if (Bank.Accounts[bank].Type != 1 && characterData.AdminLVL == 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindBankAccount), 3000);
                            Trigger.ClientEvent(player, "closeatm");
                            Trigger.StopAnimation(player);
                            return;
                        }
                        /*if (characterData.Bank == bank || (characterData.AdminLVL >= 1 && characterData.AdminLVL <= 5))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TransactionCancelled), 3000);
                            Trigger.ClientEvent(player, "closeatm");
                            Trigger.StopAnimation(player);
                            return;
                        }*/
                        myamount = Math.Abs(amount);
                        if (myamount <= 0) return;

                        sessionData.CurrentBankTransferSumAccBankInfo = characterData.Bank;
                        sessionData.CurrentBankTransferBankInfo = bank;
                        sessionData.CurrentBankTransferSum = myamount;

                        Trigger.ClientEvent(player, "closeatm");
                        Trigger.StopAnimation(player);
                        Trigger.ClientEvent(player, "openDialog", "AcceptBankTransfer", LangFunc.GetText(LangType.Ru, DataName.TransactionConfirm1, myamount) + (Bank.Accounts[bank].Holder.Length > 3 ? LangFunc.GetText(LangType.Ru, DataName.TransactionConfirm2, Bank.Accounts[bank].Holder) : "на неизвестный счет?"));

                        //Bank.Transfer(acc.Bank, bank, myamount);
                        if (characterData.AdminLVL == 0) sessionData.TimingsData.NextBankTransfer = DateTime.Now.AddSeconds(10);
                        break;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
            }
            catch (Exception e)
            {
                Bank.Log.Write($"ClientEvent_ATMVAL Exception: {e.ToString()}");
            }
        }
        public static void AcceptTransfer(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (sessionData.CurrentBankTransferSumAccBankInfo == 0 || sessionData.CurrentBankTransferBankInfo == 0 || sessionData.CurrentBankTransferSum <= 0) return;
                Bank.Transfer(sessionData.CurrentBankTransferSumAccBankInfo, sessionData.CurrentBankTransferBankInfo, sessionData.CurrentBankTransferSum);
                
                Bank.Data targetAcc = Bank.Accounts[sessionData.CurrentBankTransferBankInfo];
                if (!Bank.Accounts.ContainsKey(sessionData.CurrentBankTransferBankInfo)) return;
                
                // ExtPlayer target = (ExtPlayer) NAPI.Player.GetPlayerFromName(targetAcc.Holder);
                // if (target.IsCharacterData())
                // {
                //     var targetSessionData = target.GetSessionData();
                //     if (targetSessionData != null)
                //     {
                //         if (sessionData.CurrentBankTransferSum >= 1000000)
                //             Admin.AdminsLog(1, $"[ВНИМАНИЕ] Игрок {target.Name}({target.Value}) получил на банковский счет {sessionData.CurrentBankTransferSum}$ от {player.Name}({player.Value})", 1, "#FF0000");
                //
                //         if (sessionData.CurrentBankTransferSum >= 10000 && targetSessionData.LastBankOperationSum == sessionData.CurrentBankTransferSum)
                //         {
                //             Admin.AdminsLog(1, $"[ВНИМАНИЕ] Игрок {target.Name}({target.Value}) два раза подряд получил на банковский счет по {sessionData.CurrentBankTransferSum}$ от {player.Name}({player.Value})", 1, "#FF0000");
                //             targetSessionData.LastBankOperationSum = 0;
                //         }
                //         else
                //         {
                //             targetSessionData.LastBankOperationSum = sessionData.CurrentBankTransferSum;
                //         }
                //     }
                // }
            }
            catch (Exception e)
            {
                Bank.Log.Write($"AcceptTransfer Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("atmDP")]
        public static void ClientEvent_ATMDupe(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Trigger.SendToAdmins(3, $"!{{#d35400}}[ATM-FLOOD] {player.Name} ({player.Value})");
            }
            catch (Exception e)
            {
                Bank.Log.Write($"ClientEvent_ATMDupe Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("atmCB")]
        public static void ClientEvent_ATMCB(ExtPlayer player, params object[] args)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                int type = Convert.ToInt32(args[0]);
                int index = Convert.ToInt32(args[1]);
                if (index == -1)
                {
                    Trigger.ClientEvent(player, "closeatm");
                    Trigger.StopAnimation(player);
                    return;
                }
                switch (type)
                {
                    case 1:
                        switch (index)
                        {
                            case 0:
                                Trigger.ClientEvent(player, "atmOpen", "[2,' ','Сумма внесения наличных']");
                                sessionData.ATMData.Type = index;
                                break;
                            case 1:
                                Trigger.ClientEvent(player, "atmOpen", "[2,' ','Сумма для снятия']");
                                sessionData.ATMData.Type = index;
                                break;
                            case 2:
                                if (Houses.HouseManager.GetHouse(player, true) == null)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoHome), 3000);
                                    return;
                                }
                                var house = Houses.HouseManager.GetHouse(player, true);
                                var bankBalance = Bank.GetBalance(house.BankID);
                                
                                switch (accountData.VipLvl)
                                {
                                    case 0:
                                    case 1:
                                        Trigger.ClientEvent(player, "atmOpen", $"[2,'{bankBalance}/{Convert.ToInt32(house.Price / 100 * 0.026 * 24 * 7)}$','Сумма внесения наличных']");
                                        break;
                                    case 2:
                                        Trigger.ClientEvent(player, "atmOpen", $"[2,'{bankBalance}/{Convert.ToInt32(house.Price / 100 * 0.026 * 24 * 14)}$','Сумма внесения наличных']");
                                        break;
                                    case 3:
                                        Trigger.ClientEvent(player, "atmOpen", $"[2,'{bankBalance}/{Convert.ToInt32(house.Price / 100 * 0.026 * 24 * 21)}$','Сумма внесения наличных']");
                                        break;
                                    case 4: // Platinum
                                    case 5: // Media Platinum
                                        Trigger.ClientEvent(player, "atmOpen", $"[2,'{bankBalance}/{Convert.ToInt32(house.Price / 100 * 0.026 * 24 * 28)}$','Сумма внесения наличных']");
                                        break;
                                    default:
                                        Trigger.ClientEvent(player, "atmOpen", $"[2,'{bankBalance}/{Convert.ToInt32(house.Price / 100 * 0.026 * 24 * 7)}$','Сумма внесения наличных']");
                                        break;
                                }
                                Trigger.ClientEvent(player, "setatm", LangFunc.GetText(LangType.Ru, DataName.House), $"Недвижимость #{house.ID}", bankBalance, "");
                                sessionData.ATMData.Type = index;
                                break;
                            case 3:
                                AtmBizGen(player);
                                sessionData.ATMData.Type = index;
                                break;
                            case 4:
                                Trigger.ClientEvent(player, "atmOpen", "[2,0,'Счет зачисления']");
                                sessionData.ATMData.Type = index;
                                break;
                            default:
                                // Not supposed to end up here. 
                                break;
                        }
                        break;
                    case 2:
                        Trigger.ClientEvent(player, "atmOpen", "[1,0,0]");
                        Trigger.ClientEvent(player, "setatm", characterData.Bank, player.Name, Bank.GetBalance(characterData.Bank), "");
                        break;
                    case 3:
                    {
                        var biz = BusinessManager.BizList[characterData.BizIDs[index]];
                        var bankBalance = Bank.GetBalance(biz.BankID);
                        sessionData.ATMData.BizID = index;

                        switch (accountData.VipLvl)
                        {
                            case 0:
                            case 1:
                                Trigger.ClientEvent(player, "atmOpen",
                                    $"[2,'{bankBalance}/{Convert.ToInt32(biz.SellPrice / 100 * biz.Tax * 24 * 7)}$','Сумма зачисления']");
                                break;
                            case 2:
                                Trigger.ClientEvent(player, "atmOpen",
                                    $"[2,'{bankBalance}/{Convert.ToInt32(biz.SellPrice / 100 * biz.Tax * 24 * 14)}$','Сумма зачисления']");
                                break;
                            case 3:
                                Trigger.ClientEvent(player, "atmOpen",
                                    $"[2,'{bankBalance}/{Convert.ToInt32(biz.SellPrice / 100 * biz.Tax * 24 * 21)}$','Сумма зачисления']");
                                break;
                            case 4: // Platinum
                            case 5: // Media Platinum
                                Trigger.ClientEvent(player, "atmOpen",
                                    $"[2,'{bankBalance}/{Convert.ToInt32(biz.SellPrice / 100 * biz.Tax * 24 * 28)}$','Сумма зачисления']");
                                break;
                            default:
                                Trigger.ClientEvent(player, "atmOpen",
                                    $"[2,'{bankBalance}/{Convert.ToInt32(biz.SellPrice / 100 * biz.Tax * 24 * 7)}$','Сумма зачисления']");
                                break;
                        }

                        Trigger.ClientEvent(player, "setatm",
                            "Бизнес",
                            BusinessManager.BusinessTypeNames[biz.Type],
                            bankBalance, "");
                        break;
                    }

                }
            }
            catch (Exception e)
            {
                Bank.Log.Write($"ClientEvent_ATMCB Exception: {e.ToString()}");
            }
        }
    }
}
