using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using LinqToDB.Tools;
using NeptuneEvo.Houses;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.Players;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Localization;

namespace NeptuneEvo.Accounts.LoadCharacter
{
    class Repository
    {
        private static readonly nLog Log = new nLog("Core.Accounts.LoadCharacter");
        public static async void Load(ExtPlayer player, DateTime TestSpeedLoad, bool isReg = false)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                var accountData = player.GetAccountData();
                if (accountData == null) 
                    return;
                
                if (accountData.Chars == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.Center, LangFunc.GetText(LangType.Ru, DataName.CharDataGetError), 5000);
                    return;
                }
                
                var returnData = new Dictionary<string, object>();
                returnData.Add("Login", accountData.Login);
                returnData.Add("Email", accountData.Email);
                returnData.Add("Ga", accountData.Ga);
                returnData.Add("SocialClub", sessionData.SocialClubName);
                returnData.Add("Redbucks", accountData.RedBucks);
                returnData.Add("Vip", accountData.VipLvl);
                returnData.Add("VipDate", accountData.VipDate);
                returnData.Add("Unique", accountData.Unique);
                returnData.Add("LastSelectCharUUID", accountData.LastSelectCharUUID);

                if (accountData.isSubscribe) returnData.Add("Subscribe", accountData.SubscribeTime);
                else returnData.Add("Subscribe", false);

                var charsData = new Dictionary<int, object>();

                await using var db = new ServerBD("MainDB");//В отдельном потоке
                var characters = await db.Characters
                    .Select(v => new {
                        v.Uuid,
                        v.IsDelete,
                        v.DeleteData,
                        v.Firstname,
                        v.Lastname,
                        v.Lvl,
                        v.Exp,
                        v.Money,
                        v.Bank,
                        v.Gender,
                    })
                    .Where(v => v.Uuid.In(accountData.Chars))
                    .ToListAsync();

                var customizationsData = new Dictionary<int, Customizations>();
                var clothesData = new Dictionary<int, Dictionary<int, ComponentVariation>>();
                var accessoryData = new Dictionary<int, Dictionary<int, ComponentVariation>>();

                int i = 0;
                foreach (var charData in accountData.Chars.ToList())
                {
                    if (charData > 0 && characters.FirstOrDefault(c => c.Uuid == charData) == null)
                    {
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CharDelete, charData), 3000);
                        accountData.Chars[i] = -1;
                    }
                    i++;
                }

                foreach (var character in characters)
                {
                    try
                    {
                        if (accountData.Chars.Contains(character.Uuid))
                        {
                            var playerData = new Dictionary<string, object>();

                            var ban = await db.Banned
                                .Where(v => v.Uuid == character.Uuid)
                                .Where(v => v.Until > DateTime.Now)
                                .FirstOrDefaultAsync();

                            if (ban != null)
                            {
                                playerData.Add("Type", "ban");
                                playerData.Add("ban", new Dictionary<string, object>()
                                {
                                    { "Reason", ban.Reason },
                                    { "Admin", ban.Byadmin },
                                    { "Time", ban.Time.ToShortTimeString() },
                                    { "Until", ban.Until.ToShortTimeString() }
                                });
                            }
                            else playerData.Add("Type", "char");

                            var customizationPlayer = await db.Customization
                                //.Select(v => new
                                //{
                                //    v.Uuid,
                                //    v.Iscreated,
                                //})
                                .Where(c => c.Uuid == character.Uuid)
                                .FirstOrDefaultAsync();

                            //
                            
                            if (customizationPlayer != null)
                            {
                                int index = 0;
                                foreach (var charIndex in accountData.Chars)
                                {
                                    if (character.Uuid == charIndex)
                                    {
                                        sessionData.SelectUUID = character.Uuid;
                                        Chars.Repository.LoadAccessories(player, character.Gender == 1);
                                    
                                        customizationsData[index] = customizationPlayer;
                                        customizationsData[index].Gender = character.Gender;
                                    
                                        if (sessionData.Clothes.ContainsKey(character.Uuid))
                                            clothesData[index] = sessionData.Clothes[character.Uuid];
                                    
                                        if (sessionData.Accessory.ContainsKey(character.Uuid))
                                            accessoryData[index] = sessionData.Accessory[character.Uuid];
                                    
                                        break;
                                    }

                                    index++;
                                }
                            }
                            
                            //
                            
                            var memberFractionData = Fractions.Manager.GetFractionMemberData(character.Uuid);
                            var memberOrganizationData = Organizations.Manager.GetOrganizationMemberData(character.Uuid);
                            var house = HouseManager.GetHouse($"{character.Firstname}_{character.Lastname}");
                            playerData.Add("Data", new Dictionary<string, object>()
                            {
                                { "UUID", character.Uuid },
                                { "DeleteData",  character.IsDelete ? character.DeleteData.ToString() : "-" },
                                { "FirstName", character.Firstname },
                                { "LastName", character.Lastname },
                                { "LVL", character.Lvl },
                                { "EXP", character.Exp },
                                { "FractionID", memberFractionData != null ? memberFractionData.Id : 0 },
                                { "OrganizationID", memberOrganizationData != null ? memberOrganizationData.Id : 0  },
                                { "houseId", house != null ? "+" : "-" },
                                { "Money", character.Money },
                                { "BankMoney", Bank.GetBalance((int)character.Bank) },
                                //{ "CustomIsCreated", customizationPlayer != null ? (sbyte)customizationPlayer.Iscreated : 0 },
                            });
                            charsData.Add(character.Uuid, playerData);
                        }

                    }
                    catch (Exception e)
                    {
                        Log.Write($"LoadSlots({accountData.Login}) Foreach Exception: {e.ToString()}");
                    }
                }
                returnData.Add("charsSlot", accountData.Chars);
                returnData.Add("chars", charsData);
                
                Trigger.ClientEvent(player, "toslots", JsonConvert.SerializeObject(returnData), JsonConvert.SerializeObject(customizationsData), JsonConvert.SerializeObject(clothesData), JsonConvert.SerializeObject(accessoryData));
                sessionData.Clothes.Clear();
                sessionData.Accessory.Clear();
                //
                
                if (isReg)
                    Utils.Analytics.HelperThread.AddEvent("register", accountData.Email, accountData.Ga);
                else
                    Utils.Analytics.HelperThread.AddEvent("login", accountData.Email, accountData.Ga);
                
                //
                Log.Write($"[{DateTime.Now - TestSpeedLoad}] Auth to ({accountData.Login})");
            }
            catch (Exception e)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.Center, LangFunc.GetText(LangType.Ru, DataName.CharDataGetError), 5000);
                Log.Write($"LoadSlots Exception: {e.ToString()}");
            }
        }

    }
}
