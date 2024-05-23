using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Houses;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.Quests;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeptuneEvo.Character.Save
{
    public class Repository
    {
        private static readonly nLog Log = new nLog("Core.Character.Save");
        public static async Task SaveSql(ServerBD db, ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                var missionData = player.GetMissionData();

                Bank.SetSave(characterData.Bank);
                
                await Customization.SaveCharacter(db, player, characterData.UUID);

                await qMain.Save(db, player, characterData.UUID);

                await BindConfig.Repository.Save(db, player, characterData.UUID);

                await Config.Repository.Save(db, player, characterData.UUID);
                
                await BattlePass.Repository.Save(db, player, characterData.UUID);

                await Players.Phone.Repository.SaveSettings(db, player, characterData.UUID);
                

                await db.Characters
                    .Where(v => v.Uuid == characterData.UUID)
                    .Set(v => v.Firstname, characterData.FirstName)
                    .Set(v => v.Lastname, characterData.LastName)
                    .Set(v => v.Pos, JsonConvert.SerializeObject(characterData.CurPosition))
                    .Set(v => v.Gender, Convert.ToSByte(characterData.Gender))
                    .Set(v => v.Health, characterData.Health)
                    .Set(v => v.Armor, characterData.Armor)
                    .Set(v => v.Lvl, characterData.LVL)
                    .Set(v => v.Exp, characterData.EXP)
                    .Set(v => v.Money, Convert.ToInt32(characterData.Money))
                    .Set(v => v.Bank, characterData.Bank)
                    .Set(v => v.Work, characterData.WorkID)
                    .Set(v => v.Drugaddi, characterData.DrugsAddiction)
                    .Set(v => v.Arrest, characterData.ArrestTime)
                    .Set(v => v.Wanted, JsonConvert.SerializeObject(characterData.WantedLVL))
                    .Set(v => v.Biz, JsonConvert.SerializeObject(characterData.BizIDs))
                    .Set(v => v.Licenses, JsonConvert.SerializeObject(characterData.Licenses))
                    .Set(v => v.Unwarn, characterData.Unwarn)
                    .Set(v => v.Unmute, characterData.Unmute)
                    .Set(v => v.Warns, characterData.Warns)
                    .Set(v => v.Hotel, characterData.HotelID)
                    .Set(v => v.Hotelleft, characterData.HotelLeft)
                    .Set(v => v.Onduty, characterData.OnDutyName)
                    .Set(v => v.Lasthour, characterData.LastHourMin)
                    .Set(v => v.Demorgan, characterData.DemorganTime)
                    .Set(v => v.Contacts, JsonConvert.SerializeObject(characterData.Contacts))
                    .Set(v => v.Achiev, JsonConvert.SerializeObject(characterData.Achievements))
                    .Set(v => v.Sim, characterData.Sim)
                    .Set(v => v.PetName, characterData.PetName)
                    .Set(v => v.Demorganinfo, JsonConvert.SerializeObject(characterData.DemorganInfo))
                    .Set(v => v.Warninfo, JsonConvert.SerializeObject(characterData.WarnInfo))
                    .Set(v => v.Time, JsonConvert.SerializeObject(characterData.Time))
                    .Set(v => v.Deaths, characterData.Deaths)
                    .Set(v => v.Kills, characterData.Kills)
                    .Set(v => v.Earnedmoney, characterData.EarnedMoney)
                    .Set(v => v.Eattimes, characterData.EatTimes)
                    .Set(v => v.Revived, characterData.Revived)
                    .Set(v => v.Handshaked, characterData.Handshaked)
                    .Set(v => v.Jobskills, JsonConvert.SerializeObject(characterData.JobSkills))
                    .Set(v => v.Refcode, characterData.RefCode)
                    .Set(v => v.WeddingUUID, characterData.WeddingUUID)
                    .Set(v => v.WeddingName, characterData.WeddingName)
                    .Set(v => v.IsBannedMP, characterData.IsBannedMP)
                    .Set(v => v.BanMPReason, characterData.BanMPReason)
                    .Set(v => v.IsBannedCrime, characterData.IsBannedCrime)
                    .Set(v => v.BanCrimeReason, characterData.BanCrimeReason)
                    .Set(v => v.MissionTask, JsonConvert.SerializeObject(missionData))
                    .Set(v => v.SelectedQuest, characterData.SelectedQuest)
                    .Set(v => v.IsForbesShow, characterData.IsForbesShow)
                    .Set(v => v.FractionTasksData, JsonConvert.SerializeObject(player.FractionTasksData))
                    .Set(v => v.IsLucky, characterData.IsLucky)
                    .UpdateAsync();
                
                if (Admin.IsServerStoping)
                    player.IsRestartSaveCharacterData = true;
            }
            catch (Exception e)
            {
                Log.Write($"SaveSql Exception: {e.ToString()}");
            }
        }
        
        public static void SaveBiz(ExtPlayer player)
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    var characterData = player.GetCharacterData();
                    if (characterData == null) 
                        return;
                
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    await db.Characters
                        .Where(v => v.Uuid == characterData.UUID)
                        .Set(v => v.Biz, JsonConvert.SerializeObject(characterData.BizIDs))
                        .UpdateAsync();
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
        }
        public static void SaveName(ExtPlayer player)
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    var characterData = player.GetCharacterData();
                    if (characterData == null) 
                        return;
                
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    await db.Characters
                        .Where(v => v.Uuid == characterData.UUID)
                        .Set(v => v.Firstname, characterData.FirstName)
                        .Set(v => v.Lastname, characterData.LastName)
                        .UpdateAsync();
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
        }
        public static void SaveName(int uuid, string firstName, string lastName)
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    await db.Characters
                        .Where(v => v.Uuid == uuid)
                        .Set(v => v.Firstname, firstName)
                        .Set(v => v.Lastname, lastName)
                        .UpdateAsync();
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
        }
        public static void SaveAdminLvl(int uuid, int lvl)
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    await db.Characters
                        .Where(v => v.Uuid == uuid)
                        .Set(v => v.Adminlvl, lvl)
                        .UpdateAsync();
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
        }
        public static void SaveUnMute(int uuid, int minute)
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    await db.Characters
                        .Where(v => v.Uuid == uuid)
                        .Set(v => v.Unmute, minute)
                        .UpdateAsync();
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
        }
        public static void AddMoney(int uuid, int money)
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    var character = await db.Characters 
                        .Select(c => new {c.Uuid, c.Money}) 
                        .Where(c => c.Uuid == uuid) 
                        .FirstOrDefaultAsync();

                    if (character != null)
                    {
                        await db.Characters 
                            .Where(c => c.Uuid == character.Uuid) 
                            .Set(c => c.Money, character.Money + money) 
                            .UpdateAsync(); 
                    }
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
        }
        public static async Task ResetLuckyWheel()
        {
            await using var db = new ServerBD("MainDB");

            await db.Characters
                .Set(c => c.IsLucky, false)
                .UpdateAsync();
        }
    }
}
