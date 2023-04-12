using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using NeptuneEvo.Accounts;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Core;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.Players;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Localization;
using NeptuneEvo.BattlePass.Models;
using NeptuneEvo.Quests;

namespace NeptuneEvo.Character.Create
{
    public class Repository
    {
        private static readonly nLog Log = new nLog("Core.Character.Create");
        public static async Task<int> Create(ExtPlayer player, string firstName, string lastName)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return -1;
                var accountData = player.GetAccountData();
                if (accountData == null) return -1;
                if (firstName.Length < 1 || lastName.Length < 1)
                {
                    Trigger.ClientEvent(player, "client.characters.create.error", LangFunc.GetText(LangType.Ru, DataName.ErrorLenghtName));
                    return -1;
                }
                if (Main.PlayerNames.Values.Contains($"{firstName}_{lastName}"))
                {
                    Trigger.ClientEvent(player, "client.characters.create.error", LangFunc.GetText(LangType.Ru, DataName.NameUsed), 3000);
                    return -1;
                }

                string fullname = $"{firstName}_{lastName}";
                var characterData = new CharacterData
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Licenses = new List<bool>() { false, false, false, false, false, false, false, false, false },
                    Achievements = new List<bool>(),
                    SpawnPos = Customization.GetSpawnPos(),
                };


                characterData.Bank = await Bank.Create(fullname);

                Main.PlayerBankAccs.TryAdd(fullname, characterData.Bank);

                for (uint i = 0; i != 401; i++) characterData.Achievements.Add(false);

                characterData.Money = Main.MoneySettings.CreateCharMoney;

                await using var db = new ServerBD("MainDB");//В отдельном потоке

                characterData.UUID = await db.InsertWithInt32IdentityAsync(new Characters
                {
                    Firstname = characterData.FirstName,
                    Lastname = characterData.LastName,
                    Gender = (sbyte)Convert.ToInt32(characterData.Gender),
                    Health = characterData.Health,
                    Armor = characterData.Armor,
                    Lvl = characterData.LVL,
                    Exp = characterData.EXP,
                    Money = (int)characterData.Money,
                    Bank = characterData.Bank,
                    Work = characterData.WorkID,
                    Drugaddi = characterData.DrugsAddiction,
                    Arrest = characterData.ArrestTime,
                    Demorgan = characterData.DemorganTime,
                    Wanted = JsonConvert.SerializeObject(characterData.WantedLVL),
                    Biz = JsonConvert.SerializeObject(characterData.BizIDs),
                    Adminlvl = characterData.AdminLVL,
                    Licenses = JsonConvert.SerializeObject(characterData.Licenses),
                    Unwarn = characterData.Unwarn,
                    Unmute = characterData.Unmute,
                    Warns = characterData.Warns,
                    Onduty = characterData.OnDutyName,
                    Lasthour = characterData.LastHourMin,
                    Hotel = characterData.HotelID,
                    Hotelleft = characterData.HotelLeft,
                    Contacts = JsonConvert.SerializeObject(characterData.Contacts),
                    Achiev = JsonConvert.SerializeObject(characterData.Achievements),
                    Sim = characterData.Sim,
                    PetName = "null",
                    Pos = JsonConvert.SerializeObject(characterData.SpawnPos),
                    Createdate = characterData.CreateDate,
                    Demorganinfo = JsonConvert.SerializeObject(new DemorganInfo()),
                    Warninfo = JsonConvert.SerializeObject(new WarnInfo()),
                    Time = JsonConvert.SerializeObject(new TimeInfo()),
                    Deaths = 0,
                    Kills = 0,
                    Earnedmoney = 0,
                    Eattimes = 0,
                    Revived = 0,
                    Handshaked = 0,
                    Jobskills = JsonConvert.SerializeObject(characterData.JobSkills),
                    Refcode = characterData.RefCode,
                    WeddingName = "",
                    IsBannedMP = false,
                    BanMPReason = "",
                    IsBannedCrime = false,
                    BanCrimeReason = "",
                    MissionTask = "{}", 
                    SelectedQuest = Zdobich.QuestName,
                    FractionTasksData = "[]"
                });

                Main.PlayerUUIDs.TryAdd(fullname, characterData.UUID);
                Main.PlayerNames.TryAdd(characterData.UUID, fullname);
                
                player.SetCharacterData(characterData);
                player.SetUUID(characterData.UUID);
                
                var battlePassData = await BattlePass.Repository.Load(db, characterData.UUID);
                player.SetBattlePassData(battlePassData);
                player.SetMissionTask(new MissionData());
                
                if (Character.Repository.LoginsBlck.Contains(accountData.Login)) 
                    GameLog.Connected(fullname, characterData.UUID, sessionData.RealSocialClub, sessionData.RealHWID, sessionData.Value, "-", accountData.Login); // Пошли нахуй дети шлюх, которые сливают IP
                else 
                    GameLog.Connected(fullname, characterData.UUID, sessionData.RealSocialClub, sessionData.RealHWID, sessionData.Value, sessionData.Address, accountData.Login);
                
                if (!Main.Characters.Contains(player))
                    Main.Characters.Add(player);

                //

                var questsData = await Quests.qMain.Load(db, characterData.UUID);

                //
                
                var phoneData = await Players.Phone.Repository.Load(db, characterData.UUID, characterData.Sim, characterData.Contacts);

                NAPI.Task.Run(() =>
                {
                    try
                    {
                        sessionData = player.GetSessionData();
                        if (sessionData == null) 
                            return;
                        if (!sessionData.IsConnect) 
                            return;
                        //
                                                
                        player.Name = fullname;
                        player.SetName(fullname);
                        GameLog.AccountLog(accountData.Login, accountData.HWID, accountData.IP, accountData.SocialClub, $"Создан персонаж {characterData.FirstName} {characterData.LastName}");
                        
                        player.SetSharedData("NewUser", true);
                        
                        //

                        Quests.qMain.InitQuests(player, questsData, isSpawn: true);
                            
                        //
                            
                        Players.Phone.Repository.Init(player, phoneData);

                        //
                        
                        Chars.Repository.LoadCharItemsData(player);
                        
                        //
                    }
                    catch (Exception e)
                    {
                        Log.Write($"Create Task Exception: {e.ToString()}");
                    }
                });
                return characterData.UUID;
            }
            catch (Exception e)
            {
                Log.Write($"Create Exception: {e.ToString()}");
                return -1;
            }
        }

    }
}
