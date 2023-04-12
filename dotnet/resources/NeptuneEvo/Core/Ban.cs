using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using MySqlConnector;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using Redage.SDK;
using Database;
using System.Threading.Tasks;
using LinqToDB;

namespace NeptuneEvo.Core
{
    class Ban
    {
        private static readonly nLog Log = new nLog("Core.Ban");

        public static async Task<Banneds> GetIsHard(string hwid)
        {
            try
            {
                await using var db = new ServerBD("MainDB");//В отдельном потоке

                return await db.Banned
                    .Where(b => b.Hwid == hwid)
                    .Where(b => b.Ishard > 0)
                    .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Log.Write($"Delete Exception: {e.ToString()}");
            }
            return null;
        }

        // Поиск по UUID персонажа
        public static async Task<Banneds> GetBanToUUID(ServerBD db, int UUID)
        {
            try
            {
                return await db.Banned
                    .Where(b => b.Uuid == UUID)
                    .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Log.Write($"Delete Exception: {e.ToString()}");
            }
            return null;
        }

        public static async Task<Banneds> GetBanToLogin(string Login)
        {
            try
            {
                await using var db = new ServerBD("MainDB");//В отдельном потоке

                return await db.Banned
                    .Where(b => b.Account.ToLower() == Login.ToLower())
                    .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Log.Write($"Delete Exception: {e.ToString()}");
            }
            return null;
        }

        public static void Online(ExtPlayer player, DateTime until, bool ishard, string reason, string admin)
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    var sessionData = player.GetSessionData();
                    if (sessionData == null) return;

                    var accountData = player.GetAccountData();
                    if (accountData == null) return;

                    var characterData = player.GetCharacterData();
                    if (characterData == null)
                    {
                        Log.Write($"Can't ban player {sessionData.Name}", nLog.Type.Error);
                        return;
                    }

                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    await db.InsertAsync(new Banneds
                    {
                        Uuid = characterData.UUID,
                        Name = sessionData.Name,
                        Account = accountData.Login,
                        Time = DateTime.Now,
                        Until = until,
                        Ishard = Convert.ToSByte(ishard),
                        Ip = sessionData.Address,
                        Socialclub = sessionData.RealSocialClub,
                        Hwid = sessionData.RealHWID,
                        Reason = reason,
                        Byadmin = admin
                    });
                }
                catch (Exception e)
                {
                    Log.Write($"Online Exception: {e.ToString()}");
                }
            });
        }
        public static void OfflineBanToNickName(string nickname, DateTime until, bool ishard, string reason, string admin)
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    if (!Main.PlayerUUIDs.ContainsKey(nickname)) return;

                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    var isBan = await db.Banned
                        .AnyAsync(b => b.Name.ToLower() == nickname.ToLower());

                    if (isBan) return;

                    int uuid = Main.PlayerUUIDs[nickname];
                    if (uuid == -1) return;

                    string ip = "";
                    string socialclub = "";
                    string login = "";
                    string hwid = "";

                    if (ishard)
                    {
                        login = Main.GetLoginFromUUID(uuid);

                        if (login == null) return;

                        var account = await db.Accounts
                            .Select(a => new
                            {
                                a.Hwid,
                                a.Socialclub,
                                a.Ip,
                                a.Login,
                            })
                            .Where(a => a.Login.ToLower() == login.ToLower())
                            .FirstOrDefaultAsync();

                        if (account == null) return;

                        ip = account.Ip;
                        socialclub = account.Socialclub;
                        login = account.Login;
                        hwid = account.Hwid;
                    }

                    await db.InsertAsync(new Banneds
                    {
                        Uuid = uuid,
                        Name = nickname,
                        Account = login,
                        Time = DateTime.Now,
                        Until = until,
                        Ishard = Convert.ToSByte(ishard),
                        Ip = ip,
                        Socialclub = socialclub,
                        Hwid = hwid,
                        Reason = reason,
                        Byadmin = admin
                    });
                }
                catch (Exception e)
                {
                    Log.Write($"Online Exception: {e.ToString()}");
                }
            });
         }

        public static void OfflineBanToLogin(string login, DateTime until, bool ishard, string reason, string admin)
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    var isBan = await db.Banned
                        .AnyAsync(b => b.Account.ToLower() == login.ToLower());

                    if (isBan) return;

                    string ip = "";
                    string socialclub = "";
                    string hwid = "";

                    if (ishard)
                    {
                        var account = await db.Accounts
                            .Select(a => new
                            {
                                a.Hwid,
                                a.Socialclub,
                                a.Ip,
                                a.Login,
                            })
                            .Where(a => a.Login.ToLower() == login.ToLower())
                            .FirstOrDefaultAsync();

                        if (account == null) return;

                        ip = account.Ip;
                        socialclub = account.Socialclub;
                        login = account.Login;
                        hwid = account.Hwid;
                    }
                    
                    await db.InsertAsync(new Banneds
                    {
                        Uuid = -1,
                        Name = "-",
                        Account = login,
                        Time = DateTime.Now,
                        Until = until,
                        Ishard = Convert.ToSByte(ishard),
                        Ip = ip,
                        Socialclub = socialclub,
                        Hwid = hwid,
                        Reason = reason,
                        Byadmin = admin
                    });
                }
                catch (Exception e)
                {
                    Log.Write($"Online Exception: {e.ToString()}");
                }
            });
        }

        public static async Task<bool> PardonHard(string nickname)
        {
            try
            {
                await using var db = new ServerBD("MainDB");//В отдельном потоке

                var isBan = await db.Banned
                    .AnyAsync(b => b.Name.ToLower() == nickname.ToLower());

                if (!isBan) return false;

                await db.Banned
                    .Where(b => b.Name.ToLower() == nickname.ToLower())
                    .Set(b => b.Ishard, Convert.ToSByte(0))
                    .UpdateAsync();

                return true;
            }
            catch (Exception e)
            {
                Log.Write($"Delete Exception: {e.ToString()}");
            }
            return false;
        }

        public static async Task<bool> Pardon(string nickname)
        {
            try
            {
                await using var db = new ServerBD("MainDB");//В отдельном потоке

                var isBan = await db.Banned
                    .AnyAsync(b => b.Name.ToLower() == nickname.ToLower());

                if (!isBan) return false;

                await db.Banned
                    .Where(b => b.Name.ToLower() == nickname.ToLower())
                    .DeleteAsync();

                return true;
            }
            catch (Exception e)
            {
                Log.Write($"Delete Exception: {e.ToString()}");
            }
            return false;
        }
        public static async Task<bool> PardonLogin(string login)
        {
            try
            {
                await using var db = new ServerBD("MainDB");//В отдельном потоке

                var isBan = await db.Banned
                    .AnyAsync(b => b.Account.ToLower() == login.ToLower());

                if (!isBan) return false;

                await db.Banned
                    .Where(b => b.Account.ToLower() == login.ToLower())
                    .DeleteAsync();

                return true;
            }
            catch (Exception e)
            {
                Log.Write($"Delete Exception: {e.ToString()}");
            }
            return false;
        }
        public static async void Delete()
        {
            try
            {
                await using var db = new ServerBD("MainDB");//В отдельном потоке

                db.Banned
                    .Where(b => DateTime.Now > b.Until)
                    .Delete();

                Log.Write($"Banned Deleted");
            }
            catch (Exception e)
            {
                Log.Write($"Delete Exception: {e.ToString()}");
            }
        }
    }
}
