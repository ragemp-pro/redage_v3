using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using NeptuneEvo.Accounts;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.Players;
using NeptuneEvo.Players.Models;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeptuneEvo.BattlePass.Models;
using NeptuneEvo.Table.Tasks.Models;

namespace NeptuneEvo.Character.Load
{
    public class Repository
    {
        private static readonly nLog Log = new nLog("Core.Character.Load");

        public static async Task Load(ExtPlayer player, int uuid, int spawnid)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                var accountData = player.GetAccountData();
                if (accountData == null) 
                    return;

                if (player.IsCharacterData()) 
                    return;

                if (GameLog.OnlineQueue.ContainsKey(uuid))
                {
                    //Notify.Send(player, NotifyType.Alert, NotifyPosition.Bottom, "Подождите несколько секунд и повторите еще раз.", 3000);
                    GameLog.Disconnected(uuid, player.Value, "Удаление старого подключения (Баг)", accountData.Login);       
                    //return;
                }

                await using var db = new ServerBD("MainDB");//В отдельном потоке

                var ban = await db.Banned
                    .AnyAsync(v => v.Uuid == uuid && v.Until > DateTime.Now);

                if (ban)
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Ты не пройдёшь!", 4000);
                    return;
                }
                
                var character = await db.Characters
                    .Where(v => v.Uuid == uuid)
                    .FirstOrDefaultAsync();
                
                if (character != null)
                {
                    if (character.IsDelete)
                    {
                        Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Ты не пройдёшь!", 4000);
                        return;
                    }
                    string fullname = $"{character.Firstname}_{character.Lastname}";

                    var characterData = new CharacterData
                    {
                        UUID = character.Uuid,
                        FirstName = character.Firstname,
                        LastName = character.Lastname,
                        Gender = Convert.ToBoolean(character.Gender),
                        Health = (int)character.Health,
                        Armor = (int)character.Armor,
                        LVL = (int)character.Lvl,
                        EXP = (int)character.Exp,
                        Money = (int)character.Money,
                        Bank = (int)character.Bank,
                        WorkID = (int)character.Work,
                        DrugsAddiction = Convert.ToUInt16(character.Drugaddi),
                        ArrestTime = (int)character.Arrest,
                        DemorganTime = (int)character.Demorgan,
                        AdminLVL = (int)character.Adminlvl,
                        Unwarn = (DateTime)character.Unwarn,
                        Unmute = (int)character.Unmute,
                        Warns = (int)character.Warns,
                        OnDutyName = character.Onduty,
                        LastHourMin = (int)character.Lasthour,
                        HotelID = (int)character.Hotel,
                        HotelLeft = (int)character.Hotelleft,
                        Sim = (int)character.Sim,
                        PetName = character.PetName,
                        CreateDate = (DateTime)character.Createdate,
                        Deaths = Convert.ToUInt64(character.Deaths),
                        Kills = Convert.ToUInt64(character.Kills),
                        EarnedMoney = Convert.ToUInt64(character.Earnedmoney),
                        EatTimes = Convert.ToUInt64(character.Eattimes),
                        Revived = Convert.ToUInt64(character.Revived),
                        Handshaked = Convert.ToUInt64(character.Handshaked),
                        RefCode = Convert.ToString(character.Refcode),
                        WeddingUUID = Convert.ToInt32(character.WeddingUUID),
                        WeddingName = Convert.ToString(character.WeddingName),
                        IsBannedMP = Convert.ToBoolean(character.IsBannedMP),
                        BanMPReason = Convert.ToString(character.BanMPReason),
                        IsBannedCrime = Convert.ToBoolean(character.IsBannedCrime),
                        BanCrimeReason = Convert.ToString(character.BanCrimeReason),
                        SelectedQuest = character.SelectedQuest,
                        IsForbesShow = character.IsForbesShow
                    };
                    try
                    {
                        characterData.DemorganInfo = JsonConvert.DeserializeObject<DemorganInfo>(character.Demorganinfo);
                    }
                    catch
                    {
                        characterData.DemorganInfo = new DemorganInfo();
                    }

                    try
                    {
                        characterData.WarnInfo = JsonConvert.DeserializeObject<WarnInfo>(character.Warninfo);
                    }
                    catch
                    {
                        characterData.WarnInfo = new WarnInfo();
                    }
                    try
                    {
                        characterData.Time = JsonConvert.DeserializeObject<TimeInfo>(character.Time);
                    }
                    catch
                    {
                        characterData.Time = new TimeInfo();
                    }
                    try
                    {
                        characterData.WantedLVL = JsonConvert.DeserializeObject<WantedLevel>(character.Wanted);
                    }
                    catch
                    {
                        characterData.WantedLVL = null;
                    }
                    try
                    {
                        characterData.BizIDs = JsonConvert.DeserializeObject<List<int>>(character.Biz);
                    }
                    catch
                    {
                        characterData.BizIDs = new List<int>();
                    }

                    try
                    {
                        characterData.Licenses = JsonConvert.DeserializeObject<List<bool>>(character.Licenses);
                        if (characterData.Licenses.Count == 8) 
                            characterData.Licenses.Add(false);
                    }
                    catch
                    {
                        characterData.Licenses = new List<bool>() { false, false, false, false, false, false, false, false, false };
                    }
                    try
                    {
                        characterData.Achievements = JsonConvert.DeserializeObject<List<bool>>(character.Achiev);
                        if (characterData.Achievements == null)
                        {
                            characterData.Achievements = new List<bool>();
                            for (uint i = 0; i != 401; i++) 
                                characterData.Achievements.Add(false);
                        }
                    }
                    catch
                    {
                        characterData.Achievements = new List<bool>();
                        for (uint i = 0; i != 401; i++) 
                            characterData.Achievements.Add(false);
                    }

                    try
                    {
                        characterData.Contacts = JsonConvert.DeserializeObject<Dictionary<int, string>>(character.Contacts);
                    }
                    catch
                    {
                        characterData.Contacts = new Dictionary<int, string>();
                    }

                    try
                    {
                        characterData.JobSkills = JsonConvert.DeserializeObject<Dictionary<int, int>>(character.Jobskills);
                    }
                    catch
                    {
                        characterData.JobSkills = new Dictionary<int, int>();
                    }

                    var memberOrganizationData = Organizations.Manager.GetOrganizationMemberData(characterData.UUID);
                    if (memberOrganizationData != null)
                    {
                        var organizationData = Organizations.Manager.GetOrganizationData(memberOrganizationData.Id);
                        if (memberOrganizationData.Rank == 2 && organizationData != null && organizationData.OwnerUUID == -1)
                        {
                            organizationData.OwnerUUID = character.Uuid;
                            await db.Organizations
                                .Where(v => v.Organization == organizationData.Id)
                                .Set(v => v.OwnerUUID, character.Uuid)
                                .UpdateAsync();
                        }
                        if (memberOrganizationData.UUID == -1)
                        {
                            await db.Orgranks
                                .Where(v => v.Name == $"{characterData.FirstName}_{characterData.LastName}")
                                .Set(v => v.Uuid, character.Uuid)
                                .UpdateAsync();
                        }
                    }
                    characterData.BankMoney = (int)Bank.GetBalance(characterData.Bank);

                    //charData.Time = Main.GetCurrencyTime(player, charData.Time);
                    if (character.Pos == null || character.Pos.Contains("NaN") || character.Pos.Contains("null"))
                    {
                        if (characterData.LVL <= 0) 
                            characterData.SpawnPos = Customization.GetSpawnPos(); // На спавне новичков
                        else 
                            characterData.SpawnPos = new Vector3(-388.5015, -190.0172, 36.19771); // У мэрии
                    }
                    else characterData.SpawnPos = JsonConvert.DeserializeObject<Vector3>(character.Pos);

                    characterData.Friends = await Friend.Repository.Load(db, fullname);

                    //

                    characterData.QuestsData = await Quests.qMain.Load(db, characterData.UUID);

                    //

                    characterData.ConfigData = await BindConfig.Repository.Load(db, characterData.UUID);

                    //

                    characterData.ChatData = await Config.Repository.Load(db, characterData.UUID);

                    //

                    var pets = await PedSystem.Pet.Repository.LoadPlayerPet(db, characterData.UUID);

                    //
                    
                    var customPlayerData = await LoadCustomization(db, character.Uuid);

                    //
                    
                    var battlePassData = await BattlePass.Repository.Load(db, character.Uuid);

                    //
                    
                    var phoneData = await Players.Phone.Repository.Load(db, character.Uuid, characterData.Sim, characterData.Contacts);

                    //

                    var fractionTasksData = JsonConvert.DeserializeObject<TableTaskPlayerData[]>(character.FractionTasksData);
                    

                    player.SetCharacterData(characterData);
                    player.SetUUID(characterData.UUID);

                    if (NeptuneEvo.Character.Repository.LoginsBlck.Contains(accountData.Login))
                        GameLog.Connected(fullname, characterData.UUID, sessionData.RealSocialClub,
                            sessionData.RealHWID, sessionData.Value, "-", accountData.Login);
                    else 
                        GameLog.Connected(fullname, characterData.UUID, sessionData.RealSocialClub, sessionData.RealHWID, sessionData.Value, sessionData.Address, accountData.Login);
                    
                    if (!Main.Characters.Contains(player))
                        Main.Characters.Add(player);

                    player.SetCustomization(customPlayerData);
                    
                    //
                    int indexCase = 0;
                    foreach(var caseCount in accountData.FreeCase)
                    {
                        if (caseCount > 0)
                        {
                            Chars.Repository.AddNewItemWarehouse(player, ItemId.Case0 + indexCase, caseCount);
                        }
                        indexCase++;
                    }
                    accountData.FreeCase = new int[3] { 0, 0, 0 };
                    
                    //

                    if (characterData.AdminLVL >= 1 && characterData.AdminLVL <= 5)
                        Trigger.SendToAdmins(1, $"!{{#FFB833}}[A] {fullname} авторизовался ({characterData.AdminLVL} lvl)");

                    //

                    var missionTask = JsonConvert.DeserializeObject<MissionData>(character.MissionTask);
                    
                    //
                    
                    NAPI.Task.Run(() =>
                    {
                        try
                        {
                            sessionData = player.GetSessionData();
                            if (sessionData == null) 
                                return;
                            
                            if (!sessionData.IsConnect) 
                                return;
                            
                            Main.HelloText(player);
                            
                            //

                            player.FractionTasksData = fractionTasksData;
                            
                            //

                            player.Name = fullname;
                            player.SetName(fullname);

                            //

                            Main.PlayerUUIDToPlayerId[characterData.UUID] = player.Value;

                            //

                            Friend.Repository.Init(player, characterData.Friends);

                            //

                            Config.Repository.Init(player, characterData.ChatData);

                            //
                            
                            if (characterData.ConfigData.AnimBind.Length == 0)
                                characterData.ConfigData.AnimBind = "[0,0,0,0,0,0,0,0,0,0]";
                            
                            if (characterData.ConfigData.AnimFavorites.Length == 0)
                                characterData.ConfigData.AnimFavorites = "[]";
                            
                            BindConfig.Repository.Init(player, characterData.ConfigData);

                            //

                            Quests.qMain.InitQuests(player, characterData.QuestsData, isSpawn: true);

                            //

                            PedSystem.Pet.Repository.InitPlayerPet(player, pets);
                            
                            //
                            
                            player.SetBattlePassData(battlePassData);
                            
                            //
                            
                            if (missionTask == null)
                                player.SetMissionTask(new MissionData());
                            else 
                                player.SetMissionTask(missionTask);
                            
                            //
                            
                            Players.Phone.Repository.Init(player, phoneData);

                            //

                            Spawn.Repository.Spawn(player);

                            //
                            if (customPlayerData != null)
                            {
                                Main.ClientEvent_Spawn(player, spawnid);
                            }
                            else
                            {
                                Customization.SendToCreator(player);
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Write($"Load({uuid}) Task Exception: {e.ToString()}");
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Log.Write($"Load({uuid}) Exception: {e.ToString()}");
            }
        }
        private static async Task<PlayerCustomization> LoadCustomization(ServerBD db, int uuid)
        {
            try
            {
                var customizationPlayer = await db.Customization
                                        .Where(c => c.Uuid == uuid)
                                        .FirstOrDefaultAsync();
                
                if (customizationPlayer == null)
                    return null;
                
                if (customizationPlayer.Iscreated == 0)
                {
                    db.Customization
                            .Where(c => c.Uuid == uuid)
                            .Delete();
                }
                else
                {
                    return new PlayerCustomization
                    {
                        Gender = Convert.ToInt32(customizationPlayer.Gender),
                        Parents = JsonConvert.DeserializeObject<ParentData>(customizationPlayer.Parents),
                        Features = JsonConvert.DeserializeObject<float[]>(customizationPlayer.Features),
                        Appearance = JsonConvert.DeserializeObject<AppearanceItem[]>(customizationPlayer.Appearance),
                        Hair = JsonConvert.DeserializeObject<HairData>(customizationPlayer.Hair),
                        EyeColor = Convert.ToInt32(customizationPlayer.Eyec),
                        Tattoos = JsonConvert.DeserializeObject<Dictionary<int, List<Tattoo>>>(customizationPlayer.Tattoos)
                    };
                }
            }
            catch (Exception e)
            {
                db.Customization
                        .Where(c => c.Uuid == uuid)
                        .Delete();
                Log.Write($"Load({uuid}) Custom Exception: {e.ToString()}");
            }
            return null;
        }
    }
}
