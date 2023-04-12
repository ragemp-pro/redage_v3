using System;
using System.Linq;
using Database;
using LinqToDB;
using Localization;
using MySqlConnector;
using NeptuneEvo.Accounts;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Handles;
using Redage.SDK;

namespace NeptuneEvo.Players.Phone.Messages.BonusCode
{
    public class Repository
    {
        public static string Enter(ExtPlayer player, string text)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return String.Empty;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return String.Empty;
            
            var accountData = player.GetAccountData();
            if (accountData == null)
                return String.Empty;
            
            if (characterData.LVL <= Main.ServerSettings.BonusCodeLvl)
            {
                return LangFunc.GetText(LangType.Ru, DataName.Bonus1lvl);
            }
            if (DateTime.Now < sessionData.TimingsData.NextBonusCode)
            {
                return LangFunc.GetText(LangType.Ru, DataName.Bonus30minCd);
            }
            text = text.ToLower();
            if (Main.BonusCodes.ContainsKey(text))
            {
                lock (Main.BonusCodes)
                {
                    if (accountData.BonusCodes.Contains(text))
                    {
                        return LangFunc.GetText(LangType.Ru, DataName.BonusUsed);
                    }
                    var pcdata = Main.BonusCodes[text];
                    if (pcdata.UsedLimit == 0 || pcdata.UsedTimes < pcdata.UsedLimit)
                    {
                        sessionData.TimingsData.NextBonusCode = DateTime.Now.AddMinutes(30);
                        accountData.BonusCodes.Add(text);
                        GameLog.Money($"server", $"player({characterData.UUID})", pcdata.RewardMoney, $"BonusReward({text})");
                        //BattlePass.Repository.UpdateReward(player, 123);
                        pcdata.UsedTimes++;
                        
                        Trigger.SetTask(async () =>
                        {
                            try
                            {
                                await using var db = new ServerBD("MainDB");//В отдельном потоке

                                await db.Bonuscodes
                                    .Where(v => v.Code == text)
                                    .Set(v => v.Used, pcdata.UsedTimes)
                                    .UpdateAsync();
                            }
                            catch (Exception e)
                            {
                                Debugs.Repository.Exception(e);
                            }
                        });
                        
                        if (pcdata.RewardMoney != 0)
                        {
                            MoneySystem.Wallet.Change(player, (int)pcdata.RewardMoney);
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"[BONUS] +{pcdata.RewardMoney}$", 15000);
                        }
                        if (pcdata.RewardVipLvl != 0)
                        {
                            if (accountData.VipLvl == 0)
                            {
                                switch (pcdata.RewardVipLvl)
                                {
                                    case 1:
                                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BronzeBonus, pcdata.RewardVipDays), 15000);
                                        break;
                                    case 2:
                                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SilverBonus, pcdata.RewardVipDays), 15000);
                                        break;
                                    case 3:
                                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GoldBonus, pcdata.RewardVipDays), 15000);
                                        break;
                                    case 4:
                                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlatinumBonus, pcdata.RewardVipDays), 15000);
                                        break;
                                    default:
                                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ErrorBonus, pcdata.RewardVipDays), 15000);
                                        break;
                                }
                                accountData.VipLvl = pcdata.RewardVipLvl;
                                accountData.VipDate = DateTime.Now.AddDays(pcdata.RewardVipDays);
                                GameLog.Money($"system", $"player({characterData.UUID})", 0, $"bonusVIP({accountData.VipLvl}lvl, {pcdata.RewardVipDays}d, стало до {accountData.VipDate.ToString("s")})");
                            }
                            else if (accountData.VipLvl == pcdata.RewardVipLvl)
                            {
                                switch (pcdata.RewardVipLvl)
                                {
                                    case 1:
                                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BronzeBonus, pcdata.RewardVipDays), 15000);
                                        break;
                                    case 2:
                                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SilverBonus, pcdata.RewardVipDays), 15000);
                                        break;
                                    case 3:
                                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GoldBonus, pcdata.RewardVipDays), 15000);
                                        break;
                                    case 4:
                                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlatinumBonus, pcdata.RewardVipDays), 15000);
                                        break;
                                    default:
                                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ErrorBonus, pcdata.RewardVipDays), 15000);
                                        break;
                                }
                                accountData.VipDate = accountData.VipDate.AddDays(pcdata.RewardVipDays);
                                GameLog.Money($"system", $"player({characterData.UUID})", 0, $"bonusVIP({accountData.VipLvl}lvl, {pcdata.RewardVipDays}d, стало до {accountData.VipDate.ToString("s")})");
                            }
                            else
                            {
                                uint days = 0;
                                switch (pcdata.RewardVipDays)
                                {
                                    case 1:
                                    case 2:
                                    case 3:
                                        if (accountData.VipLvl > pcdata.RewardVipLvl) days = 1;
                                        else days = 4;
                                        break;
                                    case 4:
                                    case 5:
                                        if (accountData.VipLvl > pcdata.RewardVipLvl) days = 2;
                                        else days = 8;
                                        break;
                                    case 6:
                                    case 7:
                                        if (accountData.VipLvl > pcdata.RewardVipLvl) days = 3;
                                        else days = 12;
                                        break;
                                    case 30:
                                        if (accountData.VipLvl > pcdata.RewardVipLvl) days = 15;
                                        else days = 30;
                                        break;
                                    default:
                                        if (accountData.VipLvl > pcdata.RewardVipLvl) days = 5;
                                        else days = 10;
                                        break;
                                }
                                accountData.VipDate = accountData.VipDate.AddDays(days);
                                GameLog.Money($"system", $"player({characterData.UUID})", 0, $"bonusVIP({accountData.VipLvl}lvl, {days}d, стало до {accountData.VipDate.ToString("s")})");
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DaysDop, days), 15000);
                            }
                            //if (pcdata.RewardExp == 0) Chars.Repository.PlayerStats(player);
                        }


                        if (characterData.Gender)
                        {
                            if (pcdata.RewardItemsMale.Count != 0)
                            {
                                foreach (var item in pcdata.RewardItemsMale)
                                {
                                    WeaponRepository.GiveWeapon(player, item.ItemId, item.Data, item.Count);
                                }
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BonusItems, pcdata.RewardItemsMale.Count), 15000);
                            }
                        }
                        else
                        {
                            if (pcdata.RewardItemsFemale.Count != 0)
                            {
                                foreach (var item in pcdata.RewardItemsFemale)
                                {
                                    WeaponRepository.GiveWeapon(player, item.ItemId, item.Data, item.Count);
                                }
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BonusItems, pcdata.RewardItemsFemale.Count), 15000);
                            }
                        }
                        if (pcdata.RewardExp != 0)
                        {
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"[BONUS] +{pcdata.RewardExp} EXP", 15000);
                            UpdateData.Exp(player, pcdata.RewardExp);
                        }
                        
                        return pcdata.RewardMessage;
                    }
                    if (pcdata.UsedLimit != 0 && pcdata.UsedTimes >= pcdata.UsedLimit) 
                        return LangFunc.GetText(LangType.Ru, DataName.BonusEnd, pcdata.UsedLimit);
                }
            }
            return LangFunc.GetText(LangType.Ru, DataName.NoBonusMore);
        }
    }
}