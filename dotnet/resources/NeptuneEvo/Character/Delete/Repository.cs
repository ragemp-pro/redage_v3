using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using MySqlConnector;
using NeptuneEvo.Accounts;
using NeptuneEvo.Core;
using NeptuneEvo.MoneySystem;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB.Tools;
using Localization;

namespace NeptuneEvo.Character.Delete
{
    class Repository
    {
        private static readonly nLog Log = new nLog("Accounts.Delete.Repository");
        public static async Task IsDeleteCharacter(ExtPlayer player, int slot)
        {
            try
            {
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                
                int uuid = accountData.Chars[slot];
                if (uuid == -1 || uuid == -2) return;

                await using var db = new ServerBD("MainDB");//В отдельном потоке
                
                var character = await db.Characters
                    .Select(v => new
                    {
                        v.Uuid,
                        v.Lvl,
                        v.IsDelete,
                        v.Demorgan,
                        v.Warns
                    })
                    .Where(v => v.Uuid == uuid)
                    .FirstOrDefaultAsync();
                
                if (character == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RecoveryCantFind), 3000);
                    return;
                }

                if (character.IsDelete)
                    CancelDeleteCharacter(db, player, slot).Wait();
                else
                {
                
                    if (character.Demorgan != 0 || character.Warns != 0)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DeleteError), 3000);
                        return;
                    }
                    
                    DeleteCharacter(db, player, slot).Wait();
                }
            }
            catch (Exception e)
            {
                Log.Write($"DeleteCharacter Exception: {e.ToString()}");
            }
        }
        public static async Task DeleteCharacter(ServerBD db, ExtPlayer player, int slot)
        {
            try
            {
                
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                
                int uuid = accountData.Chars[slot];
                if (uuid == -1 || uuid == -2) return;
                
                //if (character.Lvl <= 2)
                //{
               //     Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Невозможно удалить персонажа до 3 уровня.", 3000);
                //    return;
                //}

                var ban = await db.Banned
                    .AnyAsync(v => v.Uuid == uuid && v.Until > DateTime.Now);
                
                if (ban)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DeleteError), 3000);
                    return;
                }

                var dellData = DateTime.Now.AddMinutes((60 * 72) - 1);

                await db.Characters
                    .Where(c => c.Uuid == uuid)
                    .Set(c => c.IsDelete, true)
                    .Set(c => c.DeleteData, dellData)
                    .UpdateAsync();
                
                Trigger.ClientEvent(player, "client.character.delete", slot, JsonConvert.SerializeObject(dellData));
            }
            catch (Exception e)
            {
                Log.Write($"DeleteCharacter Exception: {e.ToString()}");
            }
        }

        public static async Task CancelDeleteCharacter(ServerBD db, ExtPlayer player, int slot)
        {
            try
            {
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                
                int uuid = accountData.Chars[slot];
                if (uuid == -1 || uuid == -2) return;
                

                await db.Characters
                    .Where(c => c.Uuid == uuid)
                    .Set(c => c.IsDelete, false)
                    .UpdateAsync();
                
                Trigger.ClientEvent(player, "client.character.canceldelete", slot);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DeleteCancel), 10000);
            }
            catch (Exception e)
            {
                Log.Write($"CancelDeleteCharacter Exception: {e.ToString()}");
            }
        }

        public static async Task DeleteCharacters(ServerBD db)
        {
            try
            {
                var chars = await db.Characters                    
                    .Select(v => new
                    {
                        v.Uuid,
                        v.Firstname,
                        v.Lastname,
                        v.IsDelete,
                        v.DeleteData,
                        v.Lvl,
                        v.Biz,
                        v.Sim,
                        v.Bank,
                        v.Demorgan,
                        v.Warns,
                    })
                    .Where(c => c.IsDelete == true && c.DeleteData < DateTime.Now)
                    .ToListAsync();
                
                var charsUUID = new List<int>();

                foreach (var charData in chars)
                {
                    charsUUID.Add(charData.Uuid);
                }
                
                var bans = await db.Banned
                    .Select(b => new
                    {
                        b.Uuid
                    })
                    .Where(v => v.Uuid.In(charsUUID))
                    .ToListAsync();

                
                var bansUUID = new List<int>();
                foreach (var ban in bans)
                {
                    bansUUID.Add(ban.Uuid);
                }

                await db.Characters
                    .Where(c => c.Uuid.In(bansUUID))
                    .Set(c => c.IsDelete, false)
                    .UpdateAsync();


                foreach (var charData in chars)
                {
                    if (!bansUUID.Contains(charData.Uuid))
                    {
                        await db.Characters
                            .Where(c => c.Uuid == charData.Uuid)
                            .DeleteAsync();

                        await db.Customization
                            .Where(c => c.Uuid == charData.Uuid)
                            .DeleteAsync();
                        
                    }
                }
                //
                NAPI.Task.Run(() =>
                {
                    foreach (var charData in chars)
                    {
                        if (!bansUUID.Contains(charData.Uuid))
                        {
                            BusinessManager.changeOwner($"{charData.Firstname}_{charData.Lastname}", "Государство");
                            
                            Chars.Repository.RemoveAll($"char_{charData.Uuid}");
                            Bank.Remove((int)charData.Bank);

                            var vehiclesNumber = VehicleManager.GetVehiclesCarNumberToPlayer($"{charData.Firstname}_{charData.Lastname}");
                            
                            foreach (string number in vehiclesNumber) 
                                VehicleManager.Remove(number);
                            
                            string login = Main.GetLoginFromUUID(charData.Uuid);

                            GameLog.CharacterDelete($"{charData.Firstname}_{charData.Lastname}", charData.Uuid, login, (int)charData.Bank);

                            Main.UUIDs.Remove(charData.Uuid);
                            Main.PlayerNames.TryRemove(charData.Uuid, out _);
                            Main.PlayerUUIDs.TryRemove($"{charData.Firstname}_{charData.Lastname}", out _);
                            Main.PlayerBankAccs.TryRemove($"{charData.Firstname}_{charData.Lastname}", out _);

                            var sim = Convert.ToInt32(charData.Sim);
                            if (sim != -1)
                            {
                                Players.Phone.Sim.Repository.Remove(sim);
                                if (Main.SimCards.ContainsKey(sim))
                                    Main.SimCards.TryRemove(sim, out _);
                            }

                            var target = Accounts.Repository.GetPlayerToLogin(login);
                            var index = -1;
                            if (target != null)
                            {
                                var targetAccountData = target.GetAccountData();
                                if (targetAccountData != null)
                                {
                                    index = targetAccountData.Chars.FindIndex(c => c == charData.Uuid);

                                    if (index != -1)
                                    {
                                        targetAccountData.Chars[index] = -1;
                                        Trigger.ClientEvent(target, "client.character.deleteSuccess", index);
                                    }

                                    GameLog.AccountLog(targetAccountData.Login, targetAccountData.HWID, targetAccountData.IP, targetAccountData.SocialClub, $"Удаление персонажа {charData.Firstname}_{charData.Lastname}");
                                    Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CharDeleted, charData.Firstname, charData.Lastname), 3000);
                                }
                            }

                            if (Main.Usernames.ContainsKey(login))
                            {
                                index = Main.Usernames[login].FindIndex(c => c == charData.Uuid);
                                
                                if (index != -1) 
                                    Main.Usernames[login][index] = -1;
                            }
                        }
                    }
                });
                //
            }
            catch (Exception e)
            {
                Log.Write($"DeleteCharacters Exception: {e.ToString()}");
            }
        }
    }
}
