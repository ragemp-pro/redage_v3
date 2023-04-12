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

namespace NeptuneEvo.Players.Phone.Messages.PromoCode
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

            var uuid = 0;
            if (!int.TryParse(text, out uuid))
            {
                text = text.ToLower();

                if (Main.PromoCodes.ContainsKey(text))
                {
                    if (accountData.PromoCodes[0] == "noref")
                    {
                        accountData.PromoCodes[0] = text;
                        Main.PromoCodes[text].UsedTimes++;
                        
                        Trigger.SetTask(async () =>
                        {
                            try
                            {
                                await using var db = new ServerBD("MainDB");//В отдельном потоке

                                await db.PromocodesNew
                                    .Where(v => v.Promo == text)
                                    .Set(v => v.Used, Main.PromoCodes[text].UsedTimes)
                                    .UpdateAsync();
                            }
                            catch (Exception e)
                            {
                                Debugs.Repository.Exception(e);
                            }
                        });
                        
                        return LangFunc.GetText(LangType.Ru, DataName.PromoAddedToAcc);
                    }

                    return LangFunc.GetText(LangType.Ru, DataName.AlreadyPromoEntered);
                }
                else if (Main.RefCodes.ContainsKey(text))
                {
                    if (accountData.RefferalId == 0)
                    {
                        uuid = Main.RefCodes[text];
                        if (accountData.Chars[0] == uuid || accountData.Chars[1] == uuid || accountData.Chars[2] == uuid)
                        {
                            return LangFunc.GetText(LangType.Ru, DataName.CantEnterPromoPersa);
                        }
                        accountData.RefferalId = uuid;
                        
                        Trigger.SetTask(async () =>
                        {
                            try
                            {
                                await using var db = new ServerBD("MainDB");//В отдельном потоке

                                await db.Accounts
                                    .Where(v => v.Login == accountData.Login)
                                    .Set(v => v.RefferalId, uuid)
                                    .UpdateAsync();

                                await db.InsertAsync(new Refferals
                                {
                                    Uuid = characterData.UUID,
                                    Name = sessionData.Name,
                                    Uuidref = uuid,
                                    Success = false,
                                    Cost = 0,
                                    Createdate = DateTime.Now,
                                    Successdate = DateTime.MinValue,
                                    Refcode = text
                                });
                            }
                            catch (Exception e)
                            {
                                Debugs.Repository.Exception(e);
                            }
                        });
                        
                        return LangFunc.GetText(LangType.Ru, DataName.RefCodeUsed, text);
                    }

                    return LangFunc.GetText(LangType.Ru, DataName.RefCodeAlreadyUsed);
                }
                
                return LangFunc.GetText(LangType.Ru, DataName.NoCodeThisServ);
            }
            if (accountData.RefferalId == 0)
            {
                if (!Main.UUIDs.Contains(uuid))
                {
                    return LangFunc.GetText(LangType.Ru, DataName.NoCodeThisServ);
                }
                if (accountData.Chars[0] == uuid || accountData.Chars[1] == uuid || accountData.Chars[2] == uuid)
                {
                    return LangFunc.GetText(LangType.Ru, DataName.CantEnterPromoPersa);
                }
                accountData.RefferalId = uuid;
                
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.Accounts
                            .Where(v => v.Login == accountData.Login)
                            .Set(v => v.RefferalId, uuid)
                            .UpdateAsync();

                        await db.InsertAsync(new Refferals
                        {
                            Uuid = characterData.UUID,
                            Name = sessionData.Name,
                            Uuidref = uuid,
                            Success = false,
                            Cost = 0,
                            Createdate = DateTime.Now,
                            Successdate = DateTime.MinValue,
                            Refcode = text
                        });
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
                
                return LangFunc.GetText(LangType.Ru, DataName.RefCodeUsed, uuid);
            }

            return LangFunc.GetText(LangType.Ru, DataName.RefCodeAlreadyUsed);
        }
        
    }
}