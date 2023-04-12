using MySqlConnector;
using NeptuneEvo.Fractions;
using Redage.SDK;
using System.Collections.Generic;
using System.Threading;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using System.Linq;
using System;
using NeptuneEvo.Chars;
using System.Data;
using Newtonsoft.Json;
using NeptuneEvo.Chars.Models;
using System.Collections.Concurrent;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using Database;
using LinqToDB;
using System.Threading.Tasks;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;

namespace NeptuneEvo.Core
{
    class SyncThread : Script
    {
        private static readonly nLog Log = new nLog("Core.SyncThread");


        public static async Task PromoSync()
        {
            try
            {
                var promoCodes = new ConcurrentDictionary<string, Main.PromoCodesData>();
                var media = new List<int>();

                await using var db = new ServerBD("MainDB");//В отдельном потоке

                var promocodes = await db.PromocodesNew
                    .ToListAsync();

                string login;
                foreach (var promocode in promocodes)
                {
                    if (!promoCodes.ContainsKey(promocode.Promo.ToLower()))
                    {
                        var _NewItems = JsonConvert.DeserializeObject<List<InventoryItemData>>(promocode.ItemsR);

                        promoCodes.TryAdd(promocode.Promo.ToLower(), 
                            new Main.PromoCodesData(Convert.ToUInt32(promocode.Createdby), 
                            Convert.ToUInt64(promocode.Used), 
                            Convert.ToUInt64(promocode.Rewardreceived), 
                            Convert.ToUInt64(promocode.Rewardlimit), 
                            Convert.ToString(promocode.MsgR), 
                            Convert.ToUInt32(promocode.MoneyR), 
                            Convert.ToByte(promocode.VipR), 
                            Convert.ToUInt16(promocode.VipdaysR), 
                            _NewItems, 
                            Convert.ToDouble(promocode.DonR), 
                            Convert.ToString(promocode.DonloginR), 
                            Convert.ToUInt64(promocode.Donreceived)));

                        if (promocode.Createdby != 0 && !media.Contains(Convert.ToInt32(promocode.Createdby)))
                        {
                            media.Add(Convert.ToInt32(promocode.Createdby));

                            login = Main.GetLoginFromUUID(Convert.ToInt32(promocode.Createdby));

                            if (login == null) continue;

                            var account = await db.Accounts
                                .Select(a => new
                                {
                                    a.Login,
                                    a.Socialclub
                                })
                                .Where(a => a.Login == login)
                                .FirstOrDefaultAsync();

                            if (account == null) continue;

                            if (!Main.MediaSocials.Contains(account.Socialclub)) 
                                Main.MediaSocials.Add(account.Socialclub);
                        }
                    }
                }
                Main.PromoCodes = promoCodes;
                Log.Write($"PromoCodes loaded {promoCodes.Count()}.", nLog.Type.Success);
                Main.Media = media;
            }
            catch (Exception e)
            {
                Log.Write($"PromoSync Exception: {e.ToString()}");
            }
        }

        public static async Task BonusSync()
        {
            try
            {
                var bonusCodes = new ConcurrentDictionary<string, Main.BonusCodesData>();

                await using var db = new ServerBD("MainDB");//В отдельном потоке

                var bonuscodes = await db.Bonuscodes
                    .ToListAsync();

                foreach(var bonuscode in bonuscodes)
                {
                    if (!bonusCodes.ContainsKey(bonuscode.Code.ToLower()))
                    {

                        var _NewItemsSm = JsonConvert.DeserializeObject<List<InventoryItemData>>(bonuscode.ItemsmR);               
                
                        var _NewItemsSf = JsonConvert.DeserializeObject<List<InventoryItemData>>(bonuscode.ItemsfR);           

                        bonusCodes.TryAdd(bonuscode.Code.ToLower(), 
                            new Main.BonusCodesData(Convert.ToUInt64(bonuscode.Used), 
                            Convert.ToUInt64(bonuscode.Limit), 
                            Convert.ToString(bonuscode.MsgR), 
                            Convert.ToByte(bonuscode.ExpR), 
                            Convert.ToUInt32(bonuscode.MoneyR), 
                            Convert.ToByte(bonuscode.VipR), 
                            Convert.ToUInt16(bonuscode.VipdaysR), 
                            _NewItemsSm, 
                            _NewItemsSf));
                    }
                }
                Main.BonusCodes = bonusCodes;
                Log.Write($"BonusCodes loaded {bonusCodes.Count()}.", nLog.Type.Success);
            }
            catch (Exception e)
            {
                Log.Write($"BonusSync Exception: {e.ToString()}");
            }
        }

        public static void FClearBackground(ExtPlayer player, int fracId)
        {
            try
            {
                
                foreach (var foreachPlayer in Character.Repository.GetPlayers())
                {
                    var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                    if (foreachMemberFractionData == null) 
                        continue;
                    
                    if (foreachMemberFractionData.Id != fracId) 
                        continue;

                    foreachPlayer.RemoveFractionMemberData();
                    foreachPlayer.ClearAccessories();
                    Customization.ApplyCharacter(foreachPlayer);
                    
                    Notify.Send(foreachPlayer, NotifyType.Warning, NotifyPosition.BottomCenter, $"Администратор {player.Name} очистил фракцию, в которой Вы находились.", 3000);
                }
                
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.Fracranks
                            .Where(r => r.Id == fracId)
                            .DeleteAsync();
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
                    
                Manager.AllMembers[fracId].Clear();
                
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно очистили {Manager.FractionNames[fracId]}", 3000);

                Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) очистил фракцию {Manager.FractionNames[fracId]}");
            }
            catch (Exception e)
            {
                Log.Write($"FClearBackground Exception: {e.ToString()}");
            }
        }
    }
}
