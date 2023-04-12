using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using LinqToDB;
using NeptuneEvo.Character;
using NeptuneEvo.Handles;
using NeptuneEvo.Players.Animations.Models;
using NeptuneEvo.Players.Phone.Call.Models;
using NeptuneEvo.Players.Phone.Contacts.Models;
using NeptuneEvo.Players.Phone.Messages.Models;
using NeptuneEvo.Players.Phone.Models;
using NeptuneEvo.Players.Phone.Recents.Models;
using NeptuneEvo.Players.Phone.Settings.Models;
using Newtonsoft.Json;

namespace NeptuneEvo.Players.Phone
{
    public class Repository
    {
        public static void PhoneAnim(ExtPlayer player, int id)
        {
            if (!player.IsCharacterData())
                return;
            
            Trigger.ClientEventInRange(player.Position, 50f, "client.phone.anim", player.Id, id);
        }
        public static void Open(ExtPlayer player)
        {
            var phoneData = player.getPhoneData();
            if (phoneData == null || phoneData.IsOpen) 
                return;

            phoneData.IsOpen = true;

            AnimPlay(player);
        }
        public static void AnimPlay(ExtPlayer player)
        {
            var phoneData = player.getPhoneData();
            if (phoneData == null || !phoneData.IsOpen) 
                return;

            var callData = phoneData.Call;
            
            if (callData != null && (callData.Type == CallType.Calling || callData.Type == CallType.Talk))
                return;
                
            
            Animations.Repository.PlayScenario(player, "cphone_base");
        }
        public static void Close(ExtPlayer player)
        {
            var phoneData = player.getPhoneData();
            if (phoneData == null || !phoneData.IsOpen) 
                return;

            AnimStop(player);
            
            phoneData.IsOpen = false;
        }
        
        public static void AnimStop(ExtPlayer player)
        {
            var phoneData = player.getPhoneData();
            if (phoneData == null || !phoneData.IsOpen) 
                return;

            var callData = phoneData.Call;
            
            if (callData != null && (callData.Type == CallType.Calling || callData.Type == CallType.Talk))
                return;
                
            Animations.Repository.StopScenario(player);
        }
        private static async Task<PhoneData> LoadSettings(ServerBD db, int uuid, Dictionary<int, string> oldContact)
        {
            var phoneData = new PhoneData();
            try
            {
                var phoneInfo = await db.Phoneinfo
                    .Where(pi => pi.Uuid == uuid)
                    .FirstOrDefaultAsync();

                if (phoneInfo != null)
                {
                    phoneData.BlackList = JsonConvert.DeserializeObject<List<int>>(phoneInfo.BlackList);
                
                    var contactsJson = JsonConvert.DeserializeObject<Dictionary<int, List<object>>>(phoneInfo.Contacts);
                    phoneData.PhoneContacts = ContactsUnJson(contactsJson);
                
                    phoneData.Settings = JsonConvert.DeserializeObject<SettingsData>(phoneInfo.Settings);
                
                    var recentsJson = JsonConvert.DeserializeObject<List<List<object>>>(phoneInfo.Recents);
                    phoneData.Recents = RecentsUnJson(recentsJson);
                    phoneData.Gallery = JsonConvert.DeserializeObject<List<List<object>>>(phoneInfo.Gallery);
                }
                else
                {
                    var contacts = new Dictionary<int, PhoneContactsData>();
                    foreach (var contact in oldContact)
                    {
                        contacts.Add(contact.Key, new PhoneContactsData
                        {
                            Name = contact.Value,
                            Avatar = "",
                        });
                    }
                    phoneData.PhoneContacts = contacts;
                
                    await db.InsertAsync(new Phoneinfoes
                    {
                        Uuid = uuid,
                        Contacts = ContactsJson(contacts),
                        BlackList = "[]",
                        Settings = JsonConvert.SerializeObject(phoneData.Settings),
                        Gallery = "[]",
                        Recents = "[]",
                    });
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }

            return phoneData;
        }
        public static async Task SaveSettings(ServerBD db, ExtPlayer player, int uuid)
        {
            try
            {
                var phoneData = player.getPhoneData();

                if (phoneData != null)
                {
                    await db.Phoneinfo
                        .Where(pi => pi.Uuid == uuid)
                        .Set(pi => pi.BlackList, JsonConvert.SerializeObject(phoneData.BlackList))
                        .Set(pi => pi.Contacts, ContactsJson(phoneData.PhoneContacts))
                        .Set(pi => pi.Settings, JsonConvert.SerializeObject(phoneData.Settings))
                        .Set(pi => pi.Recents, RecentsJson(phoneData.Recents))
                        .Set(pi => pi.Gallery, JsonConvert.SerializeObject(phoneData.Gallery))
                        .UpdateAsync();
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static async Task<PhoneData> Load(ServerBD db, int uuid, int phone, Dictionary<int, string> oldContact)
        {
            var phoneData = await LoadSettings(db, uuid, oldContact);
            
            if (!phoneData.Settings.IsAir)
                phoneData.MessagesList = await Messages.Repository.Load(db, uuid, phone);

            return phoneData;
        }
        //
        private static string ContactsJson(Dictionary<int, PhoneContactsData> phoneContacts)
        {
            var contacts = new Dictionary<int, List<object>>();

            foreach (var phoneContact in phoneContacts)
            {
                contacts.Add(phoneContact.Key, new List<object>
                {
                    phoneContact.Value.Name,
                    phoneContact.Value.Avatar
                });
            }

            return JsonConvert.SerializeObject(contacts);
        }
        private static Dictionary<int, PhoneContactsData> ContactsUnJson(Dictionary<int, List<object>> phoneContacts)
        {
            var contacts = new Dictionary<int, PhoneContactsData>();
                
            foreach (var contact in phoneContacts)
            {
                contacts.Add(contact.Key, new PhoneContactsData
                {
                    Name = Convert.ToString(contact.Value[0]),
                    Avatar = Convert.ToString(contact.Value[1]),
                });
            }

            return contacts;
        }
        //
        
        private static string RecentsJson(List<RecentsData> phoneRecents)
        {
            var recents = new List<List<object>>();

            foreach (var recent in phoneRecents)
            {
                recents.Add(new List<object>
                {
                    recent.Number,
                    recent.Time,
                    recent.IsCall,
                    recent.Duration
                });
            }

            return JsonConvert.SerializeObject(recents);
        }
        private static List<RecentsData> RecentsUnJson(List<List<object>> phoneRecents)
        {
            var recents = new List<RecentsData>();

            foreach (var recent in phoneRecents)
            {
                recents.Add(new RecentsData
                {
                    Number = Convert.ToInt32(recent[0]),
                    Time = Convert.ToDateTime(recent[1]),
                    IsCall = Convert.ToBoolean(recent[2]),
                    Duration = Convert.ToInt32(recent[3])
                });
            }

            return recents;
        }
        //
        public static string MessagesJson(List<PhoneMessageListData> phoneMessages)
        {
            var messages = new List<List<object>>();

            foreach (var message in phoneMessages)
            {
                messages.Add(new List<object>
                {
                    message.IsMe,//0
                    message.Phone,//1
                    message.Text,//2
                    message.Type,//3
                    message.Date,//4
                    message.Status//5
                });
            }

            return JsonConvert.SerializeObject(messages);
        }
        public static void Init(ExtPlayer player, PhoneData phoneData)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                player.SetPhoneData(phoneData);

                Trigger.ClientEvent(player, "client.phone.initBalckList", JsonConvert.SerializeObject(phoneData.BlackList));
                
                //
                
                Trigger.ClientEvent(player, "client.phone.initContacts", ContactsJson(phoneData.PhoneContacts));

                //
                
                Trigger.ClientEvent(player, "client.phone.initMessages", MessagesJson(phoneData.MessagesList));
                phoneData.MessagesList = new List<PhoneMessageListData>();

                //
                
                Trigger.ClientEvent(player, "client.phone.gps.init", Gps.Repository.GpsListJson);
                
                //
                
                Trigger.ClientEvent(player, "client.phone.initWeather", World.Weather.Repository.WeatherJson);
                
                //
                
                Trigger.ClientEvent(player, "client.phone.initRecents", RecentsJson(phoneData.Recents));
                
                //
                
                Trigger.ClientEvent(player, "client.phone.settings.init", JsonConvert.SerializeObject(phoneData.Settings));
                
                //
                
                Trigger.ClientEvent(player, "client.phone.initGallery", JsonConvert.SerializeObject(phoneData.Gallery));
            }
            catch (Exception e)
            {
                Console.WriteLine($"StartWork Exception: {e.ToString()}");
            }
        }
    }
}