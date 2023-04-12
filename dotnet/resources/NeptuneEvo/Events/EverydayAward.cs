using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Accounts;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Localization;
using NeptuneEvo.Players.Phone.Messages.Models;

namespace NeptuneEvo.Events
{
    class EverydayAward : Script
    {
        private static readonly nLog Log = new nLog("Events.EverydayAward");
        public static DayOfWeek InitDayOfWeek = DateTime.Now.DayOfWeek;// DateTime.Now.DayOfWeek;
        public class EverydayAwardData
        {
            [JsonIgnore]
            public int AutoId { get; set; }
            public int Day { get; set; }
            public bool Active { get; set; }
            public string Title { get; set; }
            public string Desc { get; set; }
            public string Img { get; set; }
            [JsonIgnore]
            public int Type { get; set; }
            public int ItemId { get; set; }
            [JsonIgnore]
            public int Count { get; set; }
            public string Data { get; set; }
        }

        private static List<EverydayAwardData> EverydayAwardsData = new List<EverydayAwardData>(); // item_id, item_name, amount, item_data, is_main_award

        /*
            -5, "Exclusive Case", 1, null),
            -4, "Premium Case", 1, null),
            -3, "VIP Diamond", 2, null),
            -2, "RedBucks", 100, null),
            -1, "Игровая валюта", 150000, null),
        */

        [ServerEvent(Event.ResourceStart)]
        public void Event_ResourceStart()
        {
            try
            {
                UpdateEverydayAwardItems(IsStart: true);
            }
            catch (Exception e)
            {
                Log.Write($"Event_ResourceStart Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.RefresEverydayAward)]
        public void CMD_RefresEverydayAward(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                else if (!CommandsAccess.CanUseCmd(player, AdminCommands.RefresEverydayAward)) return;
                UpdateEverydayAwardItems(IsStart: true);
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы обнеовили еж задачи.", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_Refreshclothes Exception: {e.ToString()}");
            }
        }
        public static string GetTitle(int Type, int ItemId)
        {
            switch (Type)
            {
                case 0:
                    return Chars.Repository.ItemsInfo[(ItemId)ItemId].Name;
                case 1:
                    switch (ItemId)
                    {
                        case 1:
                            return "VIP Silver";
                        case 2:
                            return "VIP Gold";
                        case 3:
                            return "VIP Diamond";
                        case 4:
                            return "VIP Diamond";
                        default:
                            // Not supposed to end up here. 
                            break;
                    }
                    break;
                case 2:
                    return "Игровая валюта";
                case 3:
                    return "RedBucks";
            }
            return "";
        }


        public static void UpdateEverydayAwardItems(bool IsStart = false)
        {
            try
            {
                bool IsInit = false;
                if (IsStart)
                {
                    using (var db = new ConfigBD("ConfigDB"))
                    {
                        List<Everydayawards> Everydayawards = db.Everydayaward
                                                        .Where(ed => ed.Active == true)
                                                        .ToList();
                        
                        if (Everydayawards.Count == 0) 
                            IsInit = true;
                        else
                        {
                            EverydayAwardsData = new List<EverydayAwardData>();
                            foreach (Everydayawards award in Everydayawards)
                            {
                                EverydayAwardData everydayAwardData = new EverydayAwardData();
                                everydayAwardData.Day = award.Day;
                                everydayAwardData.Active = award.Active;
                                everydayAwardData.Title = GetTitle(award.Type, award.ItemId);
                                everydayAwardData.Desc = award.Desc;
                                everydayAwardData.Img = award.PngUrl;
                                everydayAwardData.Type = award.Type;
                                everydayAwardData.ItemId = award.ItemId;
                                everydayAwardData.Count = award.Count;
                                everydayAwardData.Data = award.Data;
                                EverydayAwardsData.Add(everydayAwardData);
                            }
                        }
                    }
                }

                if (InitDayOfWeek != DateTime.Now.DayOfWeek)
                    IsInit = true;

                if (IsInit)
                {
                    InitDayOfWeek = DateTime.Now.DayOfWeek;
                    /*using (ConfigBD db = new ConfigBD("ConfigDB"))
                    {
                        db.Everydayawards
                            .Set(ed => ed.Active, false)
                            .Update();

                        Dictionary<int, List<EverydayAwardData>> _EverydayAwardsData = new Dictionary<int, List<EverydayAwardData>>();

                        List<EverydayawardsTable> Everydayawards = db.Everydayawards
                                                                        .OrderByDescending(ed => ed.Day)
                                                                        .ToList();
                        Everydayawards.Reverse();
                        //Выгружаем
                        foreach (EverydayawardsTable award in Everydayawards)
                        {
                            if (!_EverydayAwardsData.ContainsKey(award.Day)) _EverydayAwardsData.Add(award.Day, new List<EverydayAwardData>());

                            EverydayAwardData everydayAwardData = new EverydayAwardData();
                            everydayAwardData.AutoId = award.AutoId;
                            everydayAwardData.Day = award.Day;
                            everydayAwardData.Active = award.Active;
                            everydayAwardData.Title = GetTitle(award);
                            everydayAwardData.Desc = award.Desc;
                            everydayAwardData.Img = award.PngUrl;
                            everydayAwardData.Type = award.Type;
                            everydayAwardData.ItemId = award.ItemId;
                            everydayAwardData.Count = award.Count;
                            everydayAwardData.Data = award.Data;
                            _EverydayAwardsData[award.Day].Add(everydayAwardData);
                        }
                        Random rand = new Random();
                        //Обновляем
                        EverydayAwardsData = new List<EverydayAwardData>();
                        foreach (List<EverydayAwardData> awardDay in _EverydayAwardsData.Values)
                        {
                            EverydayAwardData awardData = awardDay[rand.Next(0, awardDay.Count)];
                            db.Everydayawards
                                .Where(ed => ed.AutoId == awardData.AutoId)
                                .Set(ed => ed.Active, true)
                                .Update();
                            EverydayAwardsData.Add(awardData);
                        }
                    }*/
                }
            }
            catch (Exception e)
            {
                Log.Write($"UpdateEverydayAwardItems Exception: {e.ToString()}");
            }
        }
        public static IReadOnlyDictionary<DayOfWeek, int> WeekToInt = new Dictionary<DayOfWeek, int>()
        {
            { DayOfWeek.Monday, 0 },
            { DayOfWeek.Tuesday, 1 },
            { DayOfWeek.Wednesday, 2 },
            { DayOfWeek.Thursday, 3 },
            { DayOfWeek.Friday, 4 },
            { DayOfWeek.Saturday, 5 },
            { DayOfWeek.Sunday, 6 }
        };
        public static IReadOnlyDictionary<DayOfWeek, int> WeekToIntNextDay = new Dictionary<DayOfWeek, int>()
        {
            { DayOfWeek.Monday, 1 },
            { DayOfWeek.Tuesday, 2 },
            { DayOfWeek.Wednesday, 3 },
            { DayOfWeek.Thursday, 4 },
            { DayOfWeek.Friday, 5 },
            { DayOfWeek.Saturday, 6 },
            { DayOfWeek.Sunday, 0 }
        };
        private static int WeekBonusSlotId = 7; // Бонус на 80 часов
        public static int IsInitOpenSlotId = 8; //Проверка на показ интерфеса при каждом входе
        private static int GameTime = 60 * 6; //Проверка на показ интерфеса при каждом входе
        private static int WeekGameTime = 60 * 70; //Проверка на показ интерфеса при каждом входе

        public static IReadOnlyDictionary<int, int> DonateToBonus = new Dictionary<int, int>()
        {
            { 9, 15000 },
            { 10, 30000 },
            { 11, 60000 },
            { 12, 100000 },
            { 13, 140000 },
        };

        //Новое


        public static void OnPlayerSpawn(ExtPlayer player)
        {
            var accountData = player.GetAccountData();
            if (accountData == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            if (Convert.ToBoolean(accountData.ReceivedAward[WeekBonusSlotId]))
            {
                Trigger.ClientEvent(player, "initAwards", false);
            }
            else
            {
                var award = new List<object>();
                
                var everyDayAward = EverydayAwardsData[WeekBonusSlotId];
                award.Add(everyDayAward.Desc);//0
                award.Add(everyDayAward.Type);//1
                award.Add(everyDayAward.ItemId);//2
                award.Add(everyDayAward.Data);//3
                award.Add(everyDayAward.Img);//4
                
                if (characterData.Time.WeekTime >= WeekGameTime) 
                    award.Add(0); //5
                else
                    award.Add(WeekGameTime - characterData.Time.WeekTime); //6
                
                Trigger.ClientEvent(player, "initAwards", JsonConvert.SerializeObject(award));
            }
        }
        
        
        [RemoteEvent("server.rl.day.load")]
        public static void OnRewardsList(ExtPlayer player)
        {
            if (!FunctionsAccess.IsWorking("everydayaward2"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                return;
            }
            
            var accountData = player.GetAccountData();
            if (accountData == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            characterData.Time = Main.GetCurrencyTime(player, characterData.Time);
            if (Main.WeekInfo != accountData.ReceivedAwardWeek)
            {
                accountData.ReceivedAwardWeek = Main.WeekInfo;
                accountData.ReceivedAward = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, accountData.ReceivedAward[IsInitOpenSlotId], 0, 0, 0, 0, 0, 0 };
                accountData.ReceivedAwardDonate = 0;
            }
            
            //
            int slotId = WeekToInt[InitDayOfWeek];
            var playerAwards = new List<List<object>>();
            for(var week = 0; week <= WeekBonusSlotId; week++)
            {
                var everyDayAward = EverydayAwardsData[week];
                
                var award = new List<object>();
                
                award.Add(week);//0
                //award.Add(everyDayAward.Title);//1
                award.Add(everyDayAward.Desc);//2
                award.Add(everyDayAward.Type);//3
                award.Add(everyDayAward.ItemId);//4
                award.Add(everyDayAward.Data);//5
                award.Add(everyDayAward.Img);//6

                if (Convert.ToBoolean(accountData.ReceivedAward[week]))
                    award.Add(1);//7
                else if (slotId == week)
                {
                    if (characterData.Time.TodayTime >= GameTime) award.Add(2); //7
                    else
                    {
                        award.Add(4); //7
                        award.Add(GameTime - characterData.Time.TodayTime); //8
                    }
                }
                else if (WeekBonusSlotId == week)
                {
                    if (characterData.Time.WeekTime >= WeekGameTime) award.Add(2); //7
                    else
                    {
                        award.Add(4); //7
                        award.Add(WeekGameTime - characterData.Time.WeekTime); //8
                    }
                }
                else if (slotId > week)
                    award.Add(3);//7
                
                playerAwards.Add(award);
            }
            
            Trigger.ClientEvent(player, "client.rewardslist.day.init", JsonConvert.SerializeObject(playerAwards));
        }

        [RemoteEvent("server.rl.day.take")]
        public static void OnTakeDay(ExtPlayer player, int slotId)
        {
            if (!FunctionsAccess.IsWorking("everydayaward2"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                return;
            }
            
            var accountData = player.GetAccountData();
            if (accountData == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            if (!Convert.ToBoolean(accountData.ReceivedAward[slotId]))
            {
                if (characterData.Time.TodayTime >= GameTime)
                {
                    accountData.ReceivedAward[slotId] = 1;
                    GiveBonus(player, slotId);
                    OnRewardsList(player);
                    Accounts.Save.Repository.SaveReceived(player);
                }
                else 
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Everyday6h), 3000);
            }
            else 
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyEveryDayGet), 3000);
        }
        
        //

        [RemoteEvent("server.rl.donate.load")]
        public static void OnDonateList(ExtPlayer player)
        {
            if (!FunctionsAccess.IsWorking("everydayaward2"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                return;
            }
            
            var accountData = player.GetAccountData();
            if (accountData == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            var playerAwards = new List<List<object>>();
            foreach (var donateData in DonateToBonus)
            {                    
                var index = donateData.Key;
                var everyDayAward = EverydayAwardsData[index - 1];
                
                var award = new List<object>();
                
                award.Add(index);//0
                //award.Add(everyDayAward.Title);//1
                award.Add(everyDayAward.Desc);//2
                award.Add(everyDayAward.Type);//3
                award.Add(everyDayAward.ItemId);//4
                award.Add(everyDayAward.Data);//5
                award.Add(everyDayAward.Img);//6
                
                if (!Convert.ToBoolean(accountData.ReceivedAward[index]))
                {
                    if (accountData.ReceivedAwardDonate >= donateData.Value) 
                    {
                        award.Add(2); //7
                        award.Add(0); //8
                    }
                    else
                    {
                        award.Add(4); //7
                        award.Add(donateData.Value - accountData.ReceivedAwardDonate); //8
                    }
                }
                else
                {
                    award.Add(1); //7
                    award.Add(0); //8
                }
                award.Add(donateData.Value); //9

                playerAwards.Add(award);
            }
            
            Trigger.ClientEvent(player, "client.rewardslist.donate.init", JsonConvert.SerializeObject(playerAwards));
        }

        [RemoteEvent("server.rl.donate.take")]
        public static void OnTakeDonate(ExtPlayer player, int slotId)
        {
            if (!FunctionsAccess.IsWorking("everydayaward2"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                return;
            }
            
            var accountData = player.GetAccountData();
            if (accountData == null) 
                return;

            var donateData = DonateToBonus[slotId];
            if (!Convert.ToBoolean(accountData.ReceivedAward[slotId]))
            {
                if (accountData.ReceivedAwardDonate >= donateData)
                {
                    accountData.ReceivedAward[slotId] = 1;
                    GiveBonus(player, slotId - 1);
                    OnDonateList(player);
                    Accounts.Save.Repository.SaveReceived(player);
                    Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.RedAge, LangFunc.GetText(LangType.Ru, DataName.YouGetPopBonus, MoneySystem.Wallet.Format(donateData)), DateTime.Now);
                }
                else 
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Everyday6h), 3000);
            }
            else 
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyEveryDayGet), 3000);
        }
        
       
        //

        [Command("trl")]
        public static void Test(ExtPlayer player, int type, int value)
        {
            if (Main.ServerNumber != 0)
                return;
            
            var accountData = player.GetAccountData();
            if (accountData == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            switch (type)
            {
                case 0:
                    accountData.ReceivedAwardDonate = value;
                    break;
                case 1:
                    characterData.Time.TodayTime = value;
                    break;
                case 2:
                    characterData.Time.WeekTime = value;
                    break;
                case 3:
                    InitDayOfWeek = WeekToInt.Keys.ToList()[value];
                    break;
            }
        }
        
        //-------------------------
        
        [RemoteEvent("server.everydayaward.open")]
        public static void LoadEverydayAwardInfo(ExtPlayer player, bool isInit = false)
        {
            try
            {
                if (!FunctionsAccess.IsWorking("everydayaward"))
                {
                    if (isInit) Main.Compensation(player);
                    else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                if (isInit && !Convert.ToBoolean(accountData.ReceivedAward[IsInitOpenSlotId]))
                {
                    Main.Compensation(player);
                    return;
                }
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                //
                /*if (aData.ReceivedAward.Count == 9 || !Convert.ToBoolean(aData.ReceivedAward[9])) {
                    if (aData.ReceivedAward.Count == 9) aData.ReceivedAward.Add(1);
                    else aData.ReceivedAward[9] = 1;
                    foreach (KeyValuePair<DayOfWeek, int> week in WeekToIntNextDay)
                    {
                        if (week.Key >= InitDayOfWeek && week.Key != DayOfWeek.Sunday)
                        {
                            aData.ReceivedAward[week.Value] = 0;
                        }
                    }
                    if (characterData.Time.WeekTime < WeekGameTime) aData.ReceivedAward[WeekBonusSlotId] = 0;
                }*/
                characterData.Time = Main.GetCurrencyTime(player, characterData.Time);
                if (Main.WeekInfo != accountData.ReceivedAwardWeek)
                {
                    accountData.ReceivedAwardWeek = Main.WeekInfo;
                    accountData.ReceivedAward = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, accountData.ReceivedAward[IsInitOpenSlotId], 0, 0, 0, 0, 0, 0 };
                    accountData.ReceivedAwardDonate = 0;
                }
                //
                var playerAwards = new List<EverydayAwardData>();
                var playerAward = new EverydayAwardData();
                foreach (int week in WeekToInt.Values)
                {
                    playerAward = EverydayAwardsData[week];
                    playerAward.Active = Convert.ToBoolean(accountData.ReceivedAward[week]);
                    playerAwards.Add(playerAward);
                }
                //
                playerAward = EverydayAwardsData[WeekBonusSlotId];
                playerAward.Active = Convert.ToBoolean(accountData.ReceivedAward[WeekBonusSlotId]);
                playerAwards.Add(playerAward);
                //
                playerAward = new EverydayAwardData();
                playerAward.Active = Convert.ToBoolean(accountData.ReceivedAward[IsInitOpenSlotId]);
                playerAwards.Add(playerAward);//8

                int donate = -1;
                var donateAward = new EverydayAwardData();
                foreach (var donateData in DonateToBonus)
                {
                    int day = donateData.Key;
                    playerAward = EverydayAwardsData[day - 1];

                    if (donate == -1 && accountData.ReceivedAward[day] == 0/* && accountData.ReceivedAwardDonate >= donateData.Value*/)
                    {
                        playerAward.Active = true;
                        donate = donateData.Value - accountData.ReceivedAwardDonate;
                        donateAward = playerAward;
                        if (donate < 1)
                            donate = 0;
                    }
                    else
                    {
                        playerAward.Active = false;
                    }
                    playerAwards.Add(playerAward);
                }
                playerAwards.Add(donateAward);

                if (donate == -1)
                    donate = 0;

                Trigger.ClientEvent(player, "client.everydayawards", isInit, WeekToInt[InitDayOfWeek] + 1, JsonConvert.SerializeObject(playerAwards), characterData.Time.WeekTime, donate);
            }
            catch (Exception e)
            {
                Log.Write($"LoadEverydayAwardInfo Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.everydayaward.checkbox")]
        public static void CheckBox(ExtPlayer player)
        {
            try
            {
                var accountData = player.GetAccountData();
                if (accountData == null) 
                    return;

                accountData.ReceivedAward[IsInitOpenSlotId] = Convert.ToBoolean(accountData.ReceivedAward[IsInitOpenSlotId]) ? 0 : 1;

                Accounts.Save.Repository.SaveReceived(player);
            }
            catch (Exception e)
            {
                Log.Write($"TakeDayAward Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.everydayaward.take")]
        public static void TakeDayAward(ExtPlayer player)
        {
            try
            {
                int SlotId = WeekToInt[InitDayOfWeek];

                var accountData = player.GetAccountData();
                if (accountData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                bool isUpdate = false;
                if (!Convert.ToBoolean(accountData.ReceivedAward[SlotId]))
                {
                    if (characterData.Time.TodayTime >= GameTime)
                    {
                        accountData.ReceivedAward[SlotId] = 1;
                        GiveBonus(player, SlotId);
                        isUpdate = true;
                    }
                    else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Everyday6h), 3000);
                }
                if (!Convert.ToBoolean(accountData.ReceivedAward[WeekBonusSlotId]))
                {
                    if (characterData.Time.WeekTime >= WeekGameTime)
                    {
                        accountData.ReceivedAward[WeekBonusSlotId] = 1;
                        GiveBonus(player, WeekBonusSlotId);
                        isUpdate = true;
                    }
                    //else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, ".", 3000);
                }

                foreach (var donateData in DonateToBonus)
                {
                    int day = donateData.Key;

                    if (accountData.ReceivedAward[day] == 0 && accountData.ReceivedAwardDonate >= donateData.Value)
                    {
                        accountData.ReceivedAward[day] = 1;
                        GiveBonus(player, day - 1);
                        isUpdate = true;
                        //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGetPopBonus, MoneySystem.Wallet.Format(donateData.Value)), 10000);
                        Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.RedAge, LangFunc.GetText(LangType.Ru, DataName.YouGetPopBonus, MoneySystem.Wallet.Format(donateData.Value)), DateTime.Now);
                        break;
                    }                    
                }


                if (isUpdate)
                    Accounts.Save.Repository.SaveReceived(player);
                else
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyEveryDayGet), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"TakeDayAward Exception: {e.ToString()}");
            }
        }

        public static void GiveBonus(ExtPlayer player, int slotId)
        {
            try
            {
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                EverydayAwardData award = EverydayAwardsData[slotId];
                switch (award.Type)
                {
                    case 0:
                        if ((ItemId)award.ItemId == ItemId.BodyArmor && award.Count > 1)
                        {
                            for (int i = 0; i < award.Count; i++)
                            {
                                Chars.Repository.AddNewItemWarehouse(player, (ItemId)award.ItemId, 1, award.Data);
                            }
                        }
                        else
                        {
                            Chars.Repository.AddNewItemWarehouse(player, (ItemId)award.ItemId, award.Count, award.Data);
                        }

                        GameLog.Money($"system", $"player({characterData.UUID})", 1, $"GiveBonus({award.ItemId},{award.Data})");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGetItemSklad, Chars.Repository.ItemsInfo[(ItemId)award.ItemId].Name), 3000);
                        break;
                    case 1:
                        Chars.Repository.UpdateVipStatus(player, award.ItemId, award.Count, true, true, "BonusVIP");
                        GameLog.Money($"system", $"player({characterData.UUID})", 1, $"GiveBonus(VIP,{award.ItemId},{award.Data})");
                        break;
                    case 2:
                        MoneySystem.Wallet.Change(player, +award.Count);
                        GameLog.Money($"system", $"player({characterData.UUID})", award.Count, $"GiveBonus");
                        //PlayerStats(player);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouWonMoneyAmount, MoneySystem.Wallet.Format(award.Count)), 3000);
                        break;
                    case 3:
                        UpdateData.RedBucks(player, award.Count, msg: LangFunc.GetText(LangType.Ru, DataName.EverydayOnlineLog));
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouWinRb, award.Count), 5000);
                        break;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"GiveBonus Exception: {e.ToString()}");
            }
        }
        /*[RemoteEvent("TakeDayAward")]
        public static void TakeDayAward(Player player)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                Account adata = player.GetAccountData();
                if (adata.ReceivedDayAward == true)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы уже получили ежедневную награду.", 3000);
                    return;
                }
                else if (characterData.Time.TodayTime < 1) // todo 60
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы сегодня не отыграли 1 час.", 3000);
                    return;
                }

                if (EverydayAwards.Count >= 7)
                {
                    DateTime now = DateTime.Now;
                    byte now_day = (byte)now.Day;
                    while (now.DayOfWeek != DayOfWeek.Monday) now = now.AddDays(-1);
                    byte date_from = (byte)now.Day;
                    byte correct_index = (byte)(now_day - date_from);

                    if (correct_index >= 0 && correct_index < 8)
                    {
                        if (GiveEverydayAward(player, EverydayAwards[correct_index].Item1, EverydayAwards[correct_index].Item2, EverydayAwards[correct_index].Item3, EverydayAwards[correct_index].Item4) == false) return;
                    }
                    else return;
                }
                else return;

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы успешно получили ежедневную награду.", 3000);

                adata.ReceivedDayAward = true;
                adata.Save().Wait();
            }
            catch (Exception e)
            {
                Log.Write($"TakeDayAward Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("TakeWeekAward")]
        public static void TakeWeekAward(Player player)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                Account adata = player.GetAccountData();
                if (adata.ReceivedWeekAward == true)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы уже получили главную награду за эту неделю.", 3000);
                    return;
                }
                else if (characterData.Time.WeekTime < 2) // todo 4800
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не отыграли 80 часов за эту неделю.", 3000);
                    return;
                }

                if (EverydayAwards.Count >= 8 && EverydayAwards[7].Item5 == true)
                {
                    if (GiveEverydayAward(player, EverydayAwards[7].Item1, EverydayAwards[7].Item2, EverydayAwards[7].Item3, EverydayAwards[7].Item4) == false) return;
                }
                else return;

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы успешно получили главную награду.", 3000);

                adata.ReceivedWeekAward = true;
                adata.Save().Wait();
            }
            catch (Exception e)
            {
                Log.Write($"TakeWeekAward Exception: {e.ToString()}");
            }
        }

        public static bool GiveEverydayAward(Player player, short ItemId, string ItemName, int Amount, string ItemData)
        {
            try
            {
                if (!player.IsCharacterData()) return false;

                bool success = false;

                if (ItemId == -2)
                {
                    var acc = player.GetAccountData();
                    UpdateData.RedBucks(player, Amount);
                    GameLog.AccountLog(acc.Login, acc.HWID, acc.IP, acc.SocialClub, $"Получение награды за ежедневный онлайн (+{Amount} RedBucks)");
                    success = true;
                }
                else if (ItemId == -1)
                {
                    GameLog.Money($"system", $"player({characterData.UUID})", 3000, $"GiveEverydayAward({ItemId},{ItemName},{Amount})");
                    MoneySystem.Wallet.Change(player, Amount);
                    success = true;
                }
                else if (ItemId > 0)
                {
                    if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", (ItemId)ItemId, Amount, ItemData) == -1)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно места в инвентаре.", 3000);
                        success = false;
                    }
                }
                else
                {
                    success = false;
                }

                return success;
            }
            catch (Exception e)
            {
                Log.Write($"GiveEverydayAward Exception: {e.ToString()}");
                return false;
            }
        }*/
    }
}