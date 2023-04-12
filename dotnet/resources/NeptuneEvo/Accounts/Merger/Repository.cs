using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using LinqToDB.Tools;
using Localization;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Houses;
using Newtonsoft.Json;
using Redage.SDK;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Quests;
using NeptuneEvo.VehicleData.Models;

namespace NeptuneEvo.Accounts.Merger
{
    public class Repository
    {
        private static readonly nLog Log = new nLog("Characters.MergerToServer");
        
        public static async Task MergerAuntification(ExtPlayer player, string pass_, int serverId = 2)
        {
            try
            {
                if (Admin.IsServerStoping)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MergeErrorRestart), 3000);
                    return;
                }
                if (serverId != 2 && serverId != 3)
                {
                    Trigger.ClientEvent(player, "client.merger.progress", -3);
                    return;
                }
                pass_ = Accounts.Repository.GetSha256(pass_);
                
                var sessionData = player.GetSessionData();
                if (sessionData == null)
                {
                    Trigger.ClientEvent(player, "client.merger.progress", -3);
                    return;
                }
                
                var accountData = player.GetAccountData();
                if (accountData == null) 
                {
                    Trigger.ClientEvent(player, "client.merger.progress", -3);
                    return;
                }
                
                int SlotsForMerger = serverId == 2 ? 3 : 6;

                for (var i = 0; i < 3; i++)
                {
                    if (accountData.Chars[i + SlotsForMerger] > 0)
                    {
                        Trigger.ClientEvent(player, "client.merger.progress", -3);
                        return;
                    }
                }

                await using var saveDB = new ServerBD("MainDB");
                await using var db = new ServerBD($"rrp{serverId}");
                // Получаем модель пользователя по логину
                var account = await db.Accounts
                    .Select(a => new
                    {
                        a.Socialclub,
                        a.Password,
                        a.Redbucks,
                        a.Vipdate,
                        a.Viplvl,
                        a.Character1,
                        a.Character2,
                        a.Character3
                    })
                    .Where(v => (v.Socialclub == sessionData.SocialClubName || v.Socialclub == sessionData.RealSocialClub) && v.Password == pass_)
                    .FirstOrDefaultAsync();
                if (account == null)
                {
                    Trigger.ClientEvent(player, "client.merger.progress", -1);
                    return;
                }
                
                var characters = await db.Characters
                    .Select(c => new
                    {
                        c.Uuid,
                        c.Biz,
                        c.Firstname      ,
                        c.Lastname       ,
                        c.Gender         ,
                        c.Health         ,
                        c.Armor          ,
                        c.Lvl            ,
                        c.Exp            ,
                        c.Money          ,
                        c.Work           ,
                        c.Drugaddi       ,
                        c.Arrest         ,
                        c.Demorgan       ,
                        c.Wanted         ,
                        c.Licenses       ,
                        c.Unwarn         ,
                        c.Unmute         ,
                        c.Warns          ,
                        c.Onduty         ,
                        c.Lasthour       ,
                        c.Contacts       ,
                        c.Achiev         ,
                        c.PetName        ,
                        c.Pos            ,
                        c.Createdate     ,
                        c.Demorganinfo   ,
                        c.Warninfo       ,
                        c.Time           ,
                        c.Deaths         ,
                        c.Kills          ,
                        c.Earnedmoney    ,
                        c.Eattimes       ,
                        c.Revived        ,
                        c.Handshaked     ,
                        c.Hotelleft
                    })
                    .Where(v => v.Uuid == account.Character1 || v.Uuid == account.Character2 || v.Uuid == account.Character3)
                    .ToListAsync();

                Dictionary<int, int> CharsToSlotId = new Dictionary<int, int>();
                Dictionary<int, int> SlotsIdToChar = new Dictionary<int, int>();

                foreach (var character in characters)
                {
                    int SloId = 0;
                    foreach (int SloData in accountData.Chars)
                    {
                        if (SloId >= SlotsForMerger && (SlotsForMerger + 3) > SloId)
                        {
                            if (SloData < 0 && !CharsToSlotId.ContainsKey(character.Uuid) && !SlotsIdToChar.ContainsKey(SloId))
                            {
                                CharsToSlotId.Add(character.Uuid, SloId);
                                SlotsIdToChar.Add(SloId, character.Uuid);
                            }
                        }
                        SloId++;
                    }
                }

                if (characters.Count != CharsToSlotId.Count)
                {
                    Trigger.ClientEvent(player, "client.merger.progress", -3);
                    return;
                }

                Trigger.ClientEvent(player, "client.merger.progress", 1);
                var itemsData = new Dictionary<string, string>();
                int redBucks = 500;
                if (account.Redbucks > 0) redBucks += account.Redbucks;
                if (account.Viplvl > 0)
                {
                    double unixTime = TimeSpan.FromTicks(account.Vipdate.Ticks - DateTime.Now.Ticks).TotalDays;
                    if (unixTime > 0) NeptuneEvo.Chars.Repository.UpdateVipStatus(player, account.Viplvl, (int)unixTime, true, true);
                }

                Trigger.ClientEvent(player, "client.merger.progress", 10);
                int num = 1;
                foreach (var character in characters)
                {
                    itemsData = new Dictionary<string, string>();
                    Trigger.ClientEvent(player, "client.merger.progress", num * 22);
                    List<int> BizIds = JsonConvert.DeserializeObject<List<int>>(character.Biz);
                    int MoneyTo = 0;
                    int MoneyToBiz = await MergerToBiz(db, serverId, character.Biz);
                    if (MoneyToBiz > 0)
                    {
                        MoneyTo += MoneyToBiz;
                        redBucks += 3000;
                    }

                    Trigger.ClientEvent(player, "client.merger.progress", num * 23);

                    var houseData = await db.Houses
                        .Where(v => v.Owner == $"{character.Firstname}_{character.Lastname}")
                        .FirstOrDefaultAsync();

                    
                    if (houseData != null)
                    {
                        MoneyTo += houseData.Price;

                        var houseMoney = await db.Money
                            .Where(v => v.Id == houseData.Bank)
                            .FirstOrDefaultAsync();

                        MoneyTo += houseMoney.Balance;
                        var furniture = await db.Furniture
                                .Where(v => v.Uuid == Convert.ToInt32(houseData.Id))
                                .FirstOrDefaultAsync();
                        if (furniture != null)
                        {
                            var fsd = JsonConvert.DeserializeObject<Dictionary<int, HouseFurniture>>(furniture.Furniture);
                            foreach (HouseFurniture f in fsd.Values)
                            {
                                itemsData.Add($"furniture_{houseData.Id}_{f.Id}", null);
                            }
                        }
                    }

                    Trigger.ClientEvent(player, "client.merger.progress", num * 24);

                    string OldFirstname = character.Firstname;
                    string OldLastname = character.Lastname;

                    MoneyTo += Convert.ToInt32(character.Money);
                    
                    var ch = new Characters
                    {
                        Uuid = character.Uuid,
                        Biz = character.Biz,
                        Firstname = character.Firstname,
                        Lastname = character.Lastname,
                        Gender = character.Gender,
                        Health = character.Health,
                        Armor = character.Armor,
                        Lvl = character.Lvl,
                        Exp = character.Exp,
                        Money = MoneyTo,
                        Work = character.Work,
                        Drugaddi = character.Drugaddi,
                        Arrest = character.Arrest,
                        Demorgan = character.Demorgan,
                        Wanted = character.Wanted,
                        Licenses = character.Licenses,
                        Unwarn = character.Unwarn,
                        Unmute = character.Unmute,
                        Warns = character.Warns,
                        Onduty = character.Onduty,
                        Lasthour = character.Lasthour,
                        Contacts = character.Contacts,
                        Achiev = character.Achiev,
                        PetName = character.PetName,
                        Pos = character.Pos,
                        Createdate = character.Createdate,
                        Demorganinfo = character.Demorganinfo,
                        Warninfo = character.Warninfo,
                        Time = character.Time,
                        Deaths = character.Deaths,
                        Kills = character.Kills,
                        Earnedmoney = character.Earnedmoney,
                        Eattimes = character.Eattimes,
                        Revived = character.Revived,
                        Handshaked = character.Handshaked,
                        Hotelleft = character.Hotelleft,
                        Jobskills = "{}",
                        Refcode = null,
                        WeddingName = "",
                        IsBannedMP = false,
                        BanMPReason = "",
                        SelectedQuest = Zdobich.QuestName,
                        IsBannedCrime = false,
                        BanCrimeReason = "",
                        MissionTask = "{}",
                        FractionTasksData = "[]"
                    };

                    int charId = await MergerToChar(player, db, saveDB, serverId, MoneyTo, ch, num);

                    itemsData.Add($"char_{character.Uuid}", $"char_{charId}");

                    await MergerToCustom(db, saveDB, ch, charId);

                    Dictionary<string, string> VehiclesItemsData = await MergerToVehicles(db, saveDB, serverId, $"{OldFirstname}_{OldLastname}", ch);

                    foreach (KeyValuePair<string, string> vItems in VehiclesItemsData)
                    {
                        itemsData.Add(vItems.Key, vItems.Value);
                    }

                    Trigger.ClientEvent(player, "client.merger.progress", num * 26);

                    var OrganizationItemsData = await MergerToOrganization(player, db, saveDB, serverId, $"{OldFirstname}_{OldLastname}", MoneyTo, charId, ch);

                    foreach (KeyValuePair<string, string> vItems in OrganizationItemsData)
                    {
                        itemsData.Add(vItems.Key, null);
                    }

                    Trigger.ClientEvent(player, "client.merger.progress", num * 27);

                    var ItemsBackpackData = await InitItemsData(db, saveDB, charId, itemsData);

                    while (ItemsBackpackData.Count > 0)
                    {
                        ItemsBackpackData = await InitItemsData(db, saveDB, charId, ItemsBackpackData);
                    }

                    itemsData = new Dictionary<string, string>();

                    accountData.Chars[CharsToSlotId[character.Uuid]] = charId;
                    num++;
                }

                Trigger.ClientEvent(player, "client.merger.progress", 95);

                await Save.Repository.SaveSql(saveDB, player);

                UpdateData.RedBucks(player, redBucks, LangFunc.GetText(LangType.Ru, DataName.Merge));
                
                Trigger.ClientEvent(player, "client.merger.progress", 999);

                LoadCharacter.Repository.Load(player, DateTime.Now);
            }
            catch (Exception e)
            {
                Log.Write($"MergerAuntification Exception: {e.ToString()}");
            }
        }
        public static async Task<int> MergerToBiz(ServerBD db, int serverId, string biz)
        {
            try
            {
                List<int> BizIds = JsonConvert.DeserializeObject<List<int>>(biz);
                int MoneyTo = 0;
                if (BizIds.Count > 0)
                {
                    var businessesData = await db.Businesses
                        .Where(v => v.Id == BizIds[0])
                        .FirstOrDefaultAsync();
                    if (businessesData != null)
                    {
                        MoneyTo += businessesData.Sellprice;

                        List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(businessesData.Orders);

                        foreach (Order order in orders)
                        {
                            if (BusinessManager.BusProductsData.ContainsKey(order.Name) && order.Amount > 0)
                            {
                                MoneyTo += order.Amount * BusinessManager.BusProductsData[order.Name].Price;
                            }
                        }
                        List<Product> prodlist = JsonConvert.DeserializeObject<List<Product>>(businessesData.Products);

                        foreach (Product p in prodlist)
                        {
                            if (BusinessManager.BusProductsData.ContainsKey(p.Name) && p.Lefts > 0)
                            {
                                MoneyTo += p.Lefts * BusinessManager.BusProductsData[p.Name].Price;
                            }

                        }
                        var businessesMoney = await db.Money
                            .Where(v => v.Id == businessesData.Money)
                            .FirstOrDefaultAsync();

                        MoneyTo += businessesMoney.Balance;
                    }
                }
                return MoneyTo;
            }
            catch (Exception e)
            {
                Log.Write($"MergerToBiz(serverId({serverId})) Exception: {e.ToString()}");
                return 0;
            }
        }
        public static async Task<int> MergerToChar(ExtPlayer player, ServerBD db, ServerBD saveDB, int serverId, int MoneyTo, Characters character, int num)
        {
            try
            {
                string OldFirstname = character.Firstname;
                string OldLastname = character.Lastname;
                bool renamed = false;
                while (saveDB.Characters.Any(v => v.Firstname == character.Firstname && v.Lastname == character.Lastname))
                {
                    try
                    {
                        if (character.Lastname.Length >= 25) character.Lastname = character.Uuid.ToString();
                        else
                        {
                            if (serverId == 2) character.Lastname += "W";
                            else character.Lastname += "R";
                            renamed = true;
                        }
                    }
                    catch
                    {
                        character.Lastname = character.Uuid.ToString();
                        break;
                    }
                }
                if (renamed) Notify.Send(player, NotifyType.Success, NotifyPosition.TopCenter, LangFunc.GetText(LangType.Ru, DataName.CharSuccMerged, OldFirstname, OldLastname), 10000);

                var charMoney = await db.Money
                    .Where(v => v.Id == character.Bank)
                    .FirstOrDefaultAsync();
                
                int bankID = 0;
                if (charMoney != null)
                {
                    bankID = await MoneySystem.Bank.Create($"{character.Firstname}_{character.Lastname}", 1, Convert.ToInt32(charMoney.Balance / 10));

                    Main.PlayerBankAccs.TryAdd($"{character.Firstname}_{character.Lastname}", bankID);
                }
                else
                {
                    bankID = await MoneySystem.Bank.Create($"{character.Firstname}_{character.Lastname}", 1, 0);
                }

                Trigger.ClientEvent(player, "client.merger.progress", num * 25);
                if (character.Hotel != -1)
                {
                    MoneyTo += (int)(Main.HotelRent * character.Hotelleft);
                }
                
                int newsimint = Players.Phone.Sim.Repository.GenerateSimCard();
                var charId = await saveDB.InsertWithInt32IdentityAsync(new Characters
                {
                    Firstname = character.Firstname,
                    Lastname = character.Lastname,
                    Gender = character.Gender,
                    Health = character.Health,
                    Armor = character.Armor,
                    Lvl = character.Lvl,
                    Exp = character.Exp,
                    Money = Convert.ToInt32(MoneyTo / 10),
                    Bank = bankID,
                    Work = character.Work,
                    Drugaddi = character.Drugaddi,
                    Arrest = character.Arrest,
                    Demorgan = character.Demorgan,
                    Wanted = character.Wanted,
                    Biz = JsonConvert.SerializeObject(new List<int>()),
                    Adminlvl = 0,
                    Licenses = character.Licenses,
                    Unwarn = character.Unwarn,
                    Unmute = character.Unmute,
                    Warns = character.Warns,
                    Onduty = character.Onduty,
                    Lasthour = character.Lasthour,
                    Hotel = -1,
                    Hotelleft = 0,
                    Contacts = character.Contacts,
                    Achiev = character.Achiev,
                    Sim = Convert.ToInt32(newsimint),
                    PetName = character.PetName,
                    Pos = character.Pos,
                    Createdate = character.Createdate,
                    Demorganinfo = character.Demorganinfo,
                    Warninfo = character.Warninfo,
                    Time = character.Time,
                    Deaths = character.Deaths,
                    Kills = character.Kills,
                    Earnedmoney = character.Earnedmoney,
                    Eattimes = character.Eattimes,
                    Revived = character.Revived,
                    Handshaked = character.Handshaked,
                    Jobskills = "{}",
                    Refcode = null,
                    WeddingName = "",
                    IsBannedMP = false,
                    BanMPReason = "",
                    SelectedQuest = Zdobich.QuestName,
                    IsBannedCrime = false,
                    BanCrimeReason = "",
                    MissionTask = "{}",
                    FractionTasksData = "[]"
                });

                Players.Phone.Sim.Repository.Add(newsimint);
                Main.SimCards.TryAdd(newsimint, charId);
                
                Main.PlayerUUIDs.TryAdd($"{character.Firstname}_{character.Lastname}", (int)charId);
                Main.PlayerNames.TryAdd((int)charId, $"{character.Firstname}_{character.Lastname}");

                return charId;
            }
            catch (Exception e)
            {
                Log.Write($"MergerToChar({character.Uuid} - {character.Firstname}_{character.Lastname}) Exception: {e.ToString()}");
                return 0;
            }
        }

        public static async Task MergerToCustom(ServerBD db, ServerBD saveDB, Characters character, int charId)
        {
            try
            {
                var custom = await db.Customization
                        .Where(v => v.Uuid == character.Uuid)
                        .FirstOrDefaultAsync();

                if (custom != null)
                {
                    await saveDB.InsertWithInt64IdentityAsync(new Customizations
                    {
                        Uuid = charId,
                        Gender = custom.Gender,
                        Parents = custom.Parents,
                        Features = custom.Features,
                        Appearance = custom.Appearance,
                        Hair = custom.Hair,
                        Tattoos = custom.Tattoos,
                        Iscreated = custom.Iscreated,
                    });
                }
            }
            catch (Exception e)
            {
                Log.Write($"MergerToCustom({character.Uuid}) Exception: {e.ToString()}");
            }
        }
        public static async Task<Dictionary<string, string>> MergerToVehicles(ServerBD db, ServerBD saveDB, int serverId, string Name, Characters character)
        {
            try
            {
                var ItemsData = new Dictionary<string, string>();

                var charVehicles = await db.Vehicles
                    .Where(v => v.Holder == Name)
                    .ToListAsync();
                
                foreach (var vehicle in charVehicles)
                {
                    if (VehicleManager.IsVehicleToNumber($"{serverId}{vehicle.Number}")) 
                        continue;
                    
                    var autoID= await saveDB.InsertWithInt32IdentityAsync(new Vehicles
                    {
                        Number = $"{serverId}{vehicle.Number}",
                        Holder = $"{character.Firstname}_{character.Lastname}",
                        Model = vehicle.Model,
                        Health = vehicle.Health,
                        Fuel = vehicle.Fuel,
                        Components = vehicle.Components,
                        Position = vehicle.Position,
                        Rotation = vehicle.Rotation,
                        Keynum = vehicle.Keynum,
                        Dirt = vehicle.Dirt,
                        Tag = "null"
                    });

                    ItemsData.Add($"vehicle_{vehicle.Number}", $"vehicle_{autoID}");
                    
                    VehicleManager.AddVehicleNumber($"{serverId}{vehicle.Number}");
                    VehicleManager.Vehicles.TryAdd($"{serverId}{vehicle.Number}", new VehicleData.Models.VehicleData
                    {
                        SqlId = autoID,
                        Number = $"{serverId}{vehicle.Number}",
                        Holder = $"{character.Firstname}_{character.Lastname}",
                        Model = vehicle.Model,
                        Health = vehicle.Health,
                        Fuel = Convert.ToInt32(vehicle.Fuel),
                        Components = JsonConvert.DeserializeObject<VehicleCustomization>(vehicle.Components),
                        //if (Row["components"].ToString() == "null") data.Components = new VehicleCustomization();
                        Position = vehicle.Position,
                        Rotation = vehicle.Rotation,
                        KeyNum = vehicle.Keynum,
                        Dirt = vehicle.Dirt,
                        Tag = "null"
                    });
                    
                    VehicleManager.VehiclesSqlIdToNumber[autoID] = $"{serverId}{vehicle.Number}";
                }
                return ItemsData;
            }
            catch (Exception e)
            {
                Log.Write($"MergerToVehicles({character.Uuid}) Exception: {e.ToString()}");
                return new Dictionary<string, string>();
            }
        }

        public static async Task<Dictionary<string, string>> MergerToOrganization(ExtPlayer player, ServerBD db, ServerBD saveDB, int serverId, string Name, int moneyTo, int charId, Characters character)
        {
            try
            {
                Dictionary<string, string> ItemsData = new Dictionary<string, string>();

                var orgRank = await db.Orgranks
                    .Select(r => new
                    {
                        r.Name,
                        r.Rank,
                        r.Id,
                    })
                    .Where(v => v.Name == Name && v.Rank == 2)
                    .FirstOrDefaultAsync();

                if (orgRank != null)
                {
                    var orgData = await db.Organizations
                        .Select(o => new
                        {
                            o.Organization,
                            o.PistolScheme,
                            o.PistolMk2Scheme,
                            o.Pistol50Scheme,
                            o.HeavyPistolScheme,
                            o.PumpShotgunScheme,
                            o.DoubleBarrelShotgunScheme,
                            o.SawnOffShotgunScheme,
                            o.MiniSMGScheme,
                            o.SMGMk2Scheme,
                            o.MachinePistolScheme,
                            o.MicroSMGScheme,
                            o.CombatPDWScheme,
                            o.CompactRifleScheme,
                            o.AssaultRifleScheme,
                            o.OfficeUP,
                            o.Customs,
                            o.Stock,
                            o.CrimeOptions,
                        })
                        .Where(v => v.Organization == orgRank.Id)
                        .FirstOrDefaultAsync();
                    
                    if (orgData != null)
                    {
                        moneyTo +=  Main.PricesSettings.CreateOrgPrice;
                        
                        ItemsData.Add($"Organization_{orgRank.Id}", null);

                        var scheme = new Dictionary<string, bool>()
                        {
                            {"Pistol", Convert.ToBoolean(orgData.PistolScheme)},
                            {"PistolMk2", Convert.ToBoolean(orgData.PistolMk2Scheme)},
                            {"Pistol50", Convert.ToBoolean(orgData.Pistol50Scheme)},
                            {"HeavyPistol", Convert.ToBoolean(orgData.HeavyPistolScheme)},
                            {"PumpShotgun", Convert.ToBoolean(orgData.PumpShotgunScheme)},
                            {"DoubleBarrelShotgun", Convert.ToBoolean(orgData.DoubleBarrelShotgunScheme)},
                            {"SawnOffShotgun", Convert.ToBoolean(orgData.SawnOffShotgunScheme)},
                            {"MiniSMG", Convert.ToBoolean(orgData.MiniSMGScheme)},
                            {"SMGMk2", Convert.ToBoolean(orgData.SMGMk2Scheme)},
                            {"MachinePistol", Convert.ToBoolean(orgData.MachinePistolScheme)},
                            {"MicroSMG", Convert.ToBoolean(orgData.MicroSMGScheme)},
                            {"CombatPDW", Convert.ToBoolean(orgData.CombatPDWScheme)},
                            {"CompactRifle", Convert.ToBoolean(orgData.CompactRifleScheme)},
                            {"AssaultRifle", Convert.ToBoolean(orgData.AssaultRifleScheme)},
                        };

                        var officeUpgrade = Convert.ToByte(orgData.OfficeUP);
                        if (officeUpgrade >= 1) moneyTo += Convert.ToInt32(Main.PricesSettings.FirstOrgPrice / 1);
                        if (officeUpgrade >= 2)
                        {
                            UpdateData.RedBucks(player, Convert.ToInt32(Main.PricesSettings.SecondOrgPrice / 1), LangFunc.GetText(LangType.Ru, DataName.CashBackOrgMerge));
                        }
                        if (Convert.ToBoolean(orgData.Customs)) moneyTo += Convert.ToInt32(Main.PricesSettings.CustomsPrice / 1);
                        if (Convert.ToBoolean(orgData.Stock)) moneyTo += Convert.ToInt32(Main.PricesSettings.StockPrice / 1);
                        if (Convert.ToBoolean(orgData.CrimeOptions)) moneyTo += Convert.ToInt32(Main.PricesSettings.CrimeOptionsPrice / 1);

                        var orgVehicles = await db.Orgvehicles
                            .Where(v => v.Organization == orgRank.Id)
                            .ToListAsync();
                    
                        foreach (var vehicle in orgVehicles)
                        {
                            ItemsData.Add($"vehicle_{vehicle.Number}", null);                            
                            if (BusinessManager.BusProductsData.ContainsKey(vehicle.Model))
                            {
                                moneyTo +=  BusinessManager.BusProductsData[vehicle.Model].Price;
                            }
                        }
                        
                        await saveDB.Characters
                            .Where(c => c.Uuid == charId)
                            .Set(c => c.Money, moneyTo)//Доедлать
                            .UpdateAsync();
                    }
                }
                return ItemsData;
            }
            catch (Exception e)
            {
                Log.Write($"MergerToOrganization({character.Uuid}) Exception: {e.ToString()}");
                return new Dictionary<string, string>();
            }
        }
        public static async Task<Dictionary<string, string>> InitItemsData(ServerBD db, ServerBD saveDB, int charId, Dictionary<string, string> ItemsData)
        {
            try
            {
                List<string> itemsList = new List<string>();
                foreach (KeyValuePair<string, string> data in ItemsData)
                {
                    itemsList.Add(data.Key);
                }

                var items = await db.ItemsData
                    .Where(v => v.DataId.In(itemsList))
                    .ToListAsync();

                Dictionary<string, string> ItemsBackpackData = new Dictionary<string, string>();
                if (items != null)
                {
                    short _SlotId = (short)Chars.Repository.GetItemsDataLastId($"warehouse_{charId}", "warehouse");
                    string locationName = $"warehouse_{charId}";
                    string Location = "warehouse";
                    bool isWarehouse = true;
                    foreach (var item in items)
                    {
                        if ((Chars.Models.ItemId)item.ItemId != Chars.Models.ItemId.CarKey)
                        {
                            locationName = $"warehouse_{charId}";
                            Location = "warehouse";
                            isWarehouse = true;
                            short SlotId = _SlotId;
                            
                            if (ItemsData.ContainsKey(item.DataId) && null != ItemsData[item.DataId])
                            {
                                locationName = ItemsData[item.DataId];
                                Location = item.Location;
                                isWarehouse = false;
                                SlotId = (sbyte)item.SlotId;
                            }
                            
                            var itemSqlID = await saveDB.InsertWithInt32IdentityAsync(new ItemsData
                            {
                                DataId = locationName,
                                ItemId = item.ItemId,
                                ItemCount = item.ItemCount,
                                ItemData = item.ItemData,
                                Location = Location,
                                SlotId = SlotId,
                            });
                            ItemId ItemId = (ItemId)item.ItemId;
                            
                            if (!Chars.Repository.ItemsData.ContainsKey(locationName))
                                Chars.Repository.ItemsData.TryAdd(locationName, new ConcurrentDictionary<string, ConcurrentDictionary<int, InventoryItemData>>());
                            
                            if (!Chars.Repository.ItemsData[locationName].ContainsKey(Location))
                                Chars.Repository.ItemsData[locationName].TryAdd(Location, new ConcurrentDictionary<int, InventoryItemData>());
                            
                            Chars.Repository.ItemsData[locationName][Location][SlotId] = new InventoryItemData((int)itemSqlID, ItemId, (int)item.ItemCount, item.ItemData, SlotId);

                            if (isWarehouse) _SlotId++;
                            if ((Chars.Models.ItemId)item.ItemId == ItemId.Bag) ItemsBackpackData.Add($"backpack_{item.AutoId}", $"backpack_{itemSqlID}");

                        }
                    }
                    //if (ItemsBackpackData.Count > 0) await InitItemsData(db, saveDB, charId, ItemsBackpackData);
                }
                return ItemsBackpackData;
            }
            catch (Exception e)
            {
                Log.Write($"InitItemsData({charId}) Exception: {e.ToString()}");
                return new Dictionary<string, string>();
            }
        }
    }
}