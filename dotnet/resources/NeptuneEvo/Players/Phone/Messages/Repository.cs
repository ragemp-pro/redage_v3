using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using LinqToDB;
using NeptuneEvo.Character;
using NeptuneEvo.Fractions;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.Players.Phone.Messages.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Players.Phone.Messages
{
    public class Repository
    {
        private static readonly nLog Log = new nLog("Players.Phone.Messages.Repository");
        public static async Task<List<PhoneMessageListData>> Load(ServerBD db, int uuid, int phone)
        {
            var phoneMessageListData = new List<PhoneMessageListData>();
            try
            {
                var phoneMessages = await db.Phonemessage
                    .Where(pm => (pm.FromPhone == phone && pm.FromUuid == uuid) || (pm.ToPhone == phone && pm.ToUuid == uuid))
                    .OrderByDescending(pm => pm.AutoId)
                    .Select(pm => new
                    {
                        Phone = pm.FromPhone == phone ? pm.ToPhone : pm.FromPhone,
                        IsMe = pm.FromPhone == phone,
                        Text = pm.Text,
                        Type = pm.Type,
                        Date = pm.Date,
                        Status = pm.FromPhone == phone ? pm.FromStatus : pm.ToStatus,
                    
                    })
                    .ToListAsync();
            
                foreach (var phoneMessage in phoneMessages)
                {
                    if (!phoneMessageListData.Any(pm => pm.Phone == phoneMessage.Phone))
                    {
                        phoneMessageListData.Add(new PhoneMessageListData
                        {
                            Phone = phoneMessage.Phone,
                            IsMe = phoneMessage.IsMe,
                            Text = phoneMessage.Text,
                            Type = (MessageType) phoneMessage.Type,
                            Date = phoneMessage.Date,
                            Status = phoneMessage.Status
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
            
            return phoneMessageListData;
        }
        
        //

        public static async Task<List<object[]>> getMessage(ExtPlayer player, int number)
        {
            var returnMessages = new List<object[]>();
            
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return returnMessages;
            
                await using var db = new ServerBD("MainDB");//В отдельном потоке

                var messages = await db.Phonemessage
                    .Where(pm =>
                        (pm.FromUuid == characterData.UUID && pm.FromPhone == characterData.Sim && pm.ToPhone == number) ||
                        (pm.ToUuid == characterData.UUID && pm.ToPhone == characterData.Sim && pm.FromPhone == number))
                    .OrderByDescending(pm => pm.AutoId)
                
                    .ToArrayAsync();

                foreach (var message in messages)
                {
                    var isMe = message.FromUuid == characterData.UUID;
                
                    returnMessages.Insert(0, new object[]
                    {
                        message.AutoId,
                        message.Text,
                        message.Date, 
                        isMe,
                        message.Type
                    });
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
            
            return returnMessages;
        }
        
        //
        private static List<Phonemessages> InsertMessagesList = new List<Phonemessages>();
        public static bool IsCountInsertMessages() => InsertMessagesList.Count > 0;
        public static async Task InsertMessages(ServerBD db)
        {
            try
            {
                var messages = InsertMessagesList.ToList();
                InsertMessagesList.Clear();

                foreach (var message in messages)
                {
                    await db.InsertAsync(message);
                }
            }
            catch (Exception e)
            {
                Log.Write($"InsertMessages Exception: {e.ToString()}");
            }
        }
        //
        //
        private static List<List<object>> UpdateToStatusMessagesList = new List<List<object>>();
        public static bool IsCountUpdateMessages() => UpdateToStatusMessagesList.Count > 0;
        public static async Task UpdateMessages(ServerBD db)
        {
            try
            {
                var messages = UpdateToStatusMessagesList.ToList();
                UpdateToStatusMessagesList.Clear();

                foreach (var message in messages)
                {
                    var fromUuid = Convert.ToInt32(message[0]);
                    var fromPhone = Convert.ToInt32(message[1]);
                    var toUuid = Convert.ToInt32(message[2]);
                    var toPhone = Convert.ToInt32(message[3]);

                    await db.Phonemessage
                        .Where(pm =>
                            pm.FromUuid == fromUuid && pm.FromPhone == fromPhone && pm.ToUuid == toUuid &&
                            pm.ToPhone == toPhone)
                        .Set(pm => pm.ToStatus, true)
                        .UpdateAsync();
                }
            }
            catch (Exception e)
            {
                Log.Write($"InsertMessages Exception: {e.ToString()}");
            }
        }
        //
        private static void UpdateMessageStatus(ExtPlayer player, int number, int key, string date = null, MessageStatus status = MessageStatus.Error)
        {
            if (date == null)
                date = JsonConvert.SerializeObject(DateTime.Now);

            Trigger.ClientEvent(player, "client.phone.updMsgStatus", number, key, date, (int) status);
        }
        
        public static void AddSystemMessageToUuid(int uuid, int number, string text, DateTime date)
        {
            var sim = -1;
            var player = Main.GetPlayerByUUID(uuid);
            
            var characterData = player.GetCharacterData();
            var phoneData = player.getPhoneData();
            if (characterData != null && phoneData != null)
            {
                sim = characterData.Sim;
                var dateJson = JsonConvert.SerializeObject(date);
            
                Trigger.ClientEvent(player, "client.phone.msgAdd", number, text, dateJson, 0);
            
                if (phoneData.SelectedNumber != characterData.Sim)
                    Settings.Repository.PlayNotify(player);
            }
            else
            {
                sim = Main.SimCards.FirstOrDefault(u => u.Value == uuid).Key;
            }

            if (Main.SimCards.ContainsKey(sim))
            {
                InsertMessagesList.Add(new Phonemessages
                {
                    FromUuid = 0,
                    FromPhone = number,
                    ToUuid = uuid,
                    ToPhone = sim,
                    Date = date,
                    Type = 0,
                    Text = text,
                    FromStatus = true,
                    ToStatus = true
                });
            }
        }

        public static void AddSystemMessage(ExtPlayer player, int number, string text, DateTime date)
        {
            Trigger.SetMainTask(() =>
            {
                try
                {
                    var characterData = player.GetCharacterData();
                    if (characterData == null) 
                        return;
                    
                    var phoneData = player.getPhoneData();
                    if (phoneData == null) 
                        return;
            
                    var dateJson = JsonConvert.SerializeObject(date);
            
                    Trigger.ClientEvent(player, "client.phone.msgAdd", number, text, dateJson, 0);
            
                    if (phoneData.SelectedNumber != characterData.Sim)
                        Settings.Repository.PlayNotify(player);
                
                    InsertMessagesList.Add(new Phonemessages
                    {
                        FromUuid = 0,
                        FromPhone = number,
                        ToUuid = characterData.UUID,
                        ToPhone = characterData.Sim,
                        Date = date,
                        Type = 0,
                        Text = text,
                        FromStatus = true,
                        ToStatus = true
                    });
                }
                catch (Exception e)
                {
                    Log.Write($"CMD_offunwarn SetTask Exception: {e.ToString()}");
                }
            });
        }



        public static void SendSystemMessage(ExtPlayer player, int number, int key, string text, int type)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null)
                return;

            var date = DateTime.Now;
            var dateJson = JsonConvert.SerializeObject(date);

            /*if (characterData.Sim == -1)
            {
                UpdateMessageStatus(player, number, key, dateJson);
                return;
            }*/

            var errorMessage = "";
            if (number == (int) DefaultNumber.Polic)
            {
                errorMessage = Police.OnCallPolice(player, text);
                //UpdateMessageStatus(player, number, key, dateJson);
            }
            else if (number == (int) DefaultNumber.Ems)
            {
                errorMessage = Ems.OnCallEms(player);
                //UpdateMessageStatus(player, number, key, dateJson);
            }
            else if (number == (int) DefaultNumber.RedAge)
            {
                var isActive = false;

                errorMessage = BonusCode.Repository.Enter(player, text);
                if (errorMessage != String.Empty)
                {
                    isActive = true;
                    AddSystemMessage(player, number, errorMessage, date);
                }

                errorMessage = PromoCode.Repository.Enter(player, text);
                if (errorMessage != String.Empty)
                {
                    isActive = true;
                    AddSystemMessage(player, number, errorMessage, date);
                }

                errorMessage = null;

                if (!isActive)
                    errorMessage = "Что то не так :-(";

                //"Что то не так :-("
            }


            UpdateMessageStatus(player, number, key, dateJson, status: MessageStatus.Received);

            InsertMessagesList.Add(new Phonemessages
            {
                FromUuid = characterData.UUID,
                FromPhone = characterData.Sim,
                ToUuid = 0,
                ToPhone = number,
                Date = date,
                Type = (sbyte) type,
                Text = text,
                FromStatus = true,
                ToStatus = true
            });


            if (errorMessage != null)
                AddSystemMessage(player, number, errorMessage, date);
        }

        public static void SendMessage(ExtPlayer player, int number, int key, string text, int type)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            var phoneData = player.getPhoneData();
            if (phoneData == null) 
                return;
            
            var date = DateTime.Now;
            var dateJson = JsonConvert.SerializeObject(date);
            
            if (!FunctionsAccess.IsWorking("phonesms"))
            {
                UpdateMessageStatus(player, number, key, dateJson);
                return;
            }
            
            if (characterData.Sim == -1 || Settings.Repository.IsAir(phoneData.Settings))
            {
                UpdateMessageStatus(player, number, key, dateJson);
                return;
            }
            
            if (!Main.SimCards.ContainsKey(number))
            {
                UpdateMessageStatus(player, number, key, dateJson);
                return;
            }

            var isSelected = false;
            
            var targetUuid = Main.SimCards[number];
            
            var target = Main.GetPlayerByUUID(targetUuid);
            
            var targetCharacterData = target.GetCharacterData();
            if (targetCharacterData != null)
            {
                if (characterData.Sim == targetCharacterData.Sim)
                {
                    UpdateMessageStatus(player, number, key, dateJson);
                    return;
                }
                
                var targetPhoneData = target.getPhoneData();
                if (targetPhoneData != null && targetPhoneData.BlackList.Contains(characterData.Sim))
                {
                    UpdateMessageStatus(player, number, key, dateJson);
                    return;
                }

                if (targetPhoneData != null && !Settings.Repository.IsAir(targetPhoneData.Settings))
                {
                    isSelected = targetPhoneData.SelectedNumber == characterData.Sim; //В переписке или нет

                    Trigger.ClientEvent(target, "client.phone.msgAdd", characterData.Sim, text, dateJson, type);
                    BattlePass.Repository.UpdateReward(player, 77);

                    if (!isSelected)
                        Settings.Repository.PlayNotify(target);
                }
            }
            
            UpdateMessageStatus(player, number, key, dateJson, status: MessageStatus.Received);
            
            InsertMessagesList.Add(new Phonemessages
            {
                FromUuid = characterData.UUID,
                FromPhone = characterData.Sim,
                ToUuid = targetUuid,
                ToPhone = number,
                Date = date,
                Type = (sbyte) type,
                Text = text,
                FromStatus = true,
                ToStatus = isSelected
            });
        }

        public static void SelectedNumber(ExtPlayer player, int number)
        {
            var phoneData = player.getPhoneData();
            if (phoneData == null) 
                return;

            phoneData.SelectedNumber = number;
        }

        public static void GetChatStatus(ExtPlayer player, int number, bool status)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            if (!Main.SimCards.ContainsKey(number))
            {
                Trigger.ClientEvent(player, "client.phone.setPhoneChatStatus", number, 2);
                return;
            }
            var targetUuid = Main.SimCards[number];

            if (!status)
            {
                UpdateToStatusMessagesList.Add(new List<object>
                {
                    targetUuid,
                    number,
                    characterData.UUID,
                    characterData.Sim
                });
            }
            
            
            var target = Main.GetPlayerByUUID(targetUuid);
            
            var targetPhoneData = target.getPhoneData();
            if (targetPhoneData != null)
            {
                if (targetPhoneData.BlackList.Contains(characterData.Sim) || Settings.Repository.IsAir(targetPhoneData.Settings))
                {
                    Trigger.ClientEvent(player, "client.phone.setPhoneChatStatus", number, 2);
                    return;
                }
                
                Trigger.ClientEvent(player, "client.phone.setPhoneChatStatus", number, 1);
                return;
            }
            
            Trigger.ClientEvent(player, "client.phone.setPhoneChatStatus", number, 2);
        }
        public static void UpdateWrite(ExtPlayer player, int number, bool toggled)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            if (!Main.SimCards.ContainsKey(number))
                return;

            var targetUuid = Main.SimCards[number];
            
            var target = Main.GetPlayerByUUID(targetUuid);
            
            var targetPhoneData = target.getPhoneData();
            if (targetPhoneData != null)
            {
                if (targetPhoneData.BlackList.Contains(characterData.Sim) || Settings.Repository.IsAir(targetPhoneData.Settings)) 
                    return;
                
                Trigger.ClientEvent(target, "client.phone.setPhoneChatWrite", characterData.Sim, toggled);
            }
        }
    }
}