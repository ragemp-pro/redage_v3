using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB.Tools;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Handles;
using NeptuneEvo.Players.Phone.Tinder.Models;
using Newtonsoft.Json;
using Redage.SDK;
using MySqlConnector;
using System.Data;
using System.Threading.Tasks;
using Database;
using LinqToDB;
using NeptuneEvo.Players.Phone.Messages.Models;

namespace NeptuneEvo.Players.Phone.Tinder
{
    public class Repository
    {
        private static Dictionary<int, TinderData> List = new Dictionary<int, TinderData>();

        public static void Init()
        {
            var testSpeedLoad = DateTime.Now;
            
            using MySqlCommand cmd = new MySqlCommand()
            {
                CommandText = "SELECT * FROM `phonetinder`"
            };
            using DataTable result = MySQL.QueryRead(cmd);

            if (result != null)
            {
                int count = 0;
                foreach (DataRow row in result.Rows)
                {
                    count++;
                    
                    var uuid = Convert.ToInt32(row["uuid"]);
                    var avatar = Convert.ToString(row["avatar"]);
                    var text = Convert.ToString(row["text"]);
                    var type = Convert.ToSByte(row["type"]);
                    var isVisible = Convert.ToBoolean(row["isVisible"]);
                    var likes = JsonConvert.DeserializeObject<List<int>>(Convert.ToString(row["likes"]));
                    var noLikes = JsonConvert.DeserializeObject<List<int>>(Convert.ToString(row["noLikes"]));
                    
                    List.Add(uuid, new TinderData
                    {
                        Avatar = avatar,
                        Text = text,
                        Type = (TinderType) type,
                        IsVisible = isVisible,
                        Likes = likes,
                        NoLikes = noLikes
                    });
                }
                Console.WriteLine($"[{DateTime.Now - testSpeedLoad}] Load Tinder ({count})");
            }
        }

        public static async Task Saves(ServerBD db)
        {
            try
            {
                var tinders = List
                    .Where(t => t.Value.IsSave)
                    .Select(t => t.Key)
                    .ToList();
                
                foreach (var uuid in tinders)
                {                
                    if (!List.ContainsKey(uuid))
                        continue;
                    
                    if (!Main.PlayerNames.ContainsKey(uuid))
                        continue;

                    var tinder = List[uuid];
                    
                    await db.Phonetinder
                        .Where(dc => dc.Uuid == uuid)
                        .Set(dc => dc.Avatar, tinder.Avatar)
                        .Set(dc => dc.Text, tinder.Text)
                        .Set(dc => dc.Type, (sbyte)tinder.Type)
                        .Set(dc => dc.IsVisible, tinder.IsVisible)
                        .Set(dc => dc.Likes, JsonConvert.SerializeObject(tinder.Likes))
                        .Set(dc => dc.NoLikes, JsonConvert.SerializeObject(tinder.NoLikes))
                        .UpdateAsync();
                }
            }            
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        public static void Init(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            if (!List.ContainsKey(characterData.UUID) || !Main.ServerSettings.IsJobTinder)
                Trigger.ClientEvent(player, "client.phone.tinder.create");
            else
            {
                var tinderData = List[characterData.UUID];

                var data = new List<object>();
                
                data.Add(tinderData.Avatar);
                data.Add(tinderData.Text);
                data.Add(tinderData.Type);
                data.Add(tinderData.IsVisible);

                var likesData = new List<List<object>>();

                foreach (var uuid in tinderData.Likes)
                {
                    if (!List.ContainsKey(uuid))
                        continue;
                    if (!Main.PlayerNames.ContainsKey(uuid))
                        continue;
                    if (!List[uuid].Likes.Contains(characterData.UUID))
                        continue;
                    
                    var tinder = List[uuid];
                    
                    var sim = Main.SimCards.FirstOrDefault(u => u.Value == uuid).Key;
                    likesData.Add(new List<object>
                    {
                        Main.PlayerNames[uuid],
                        tinder.Avatar,
                        Main.SimCards.ContainsKey(sim) ? sim : -1,
                    });
                }
                
                var likes = tinderData.Likes.ToList();
                likes.AddRange(tinderData.NoLikes.ToList());
                likes.Add(characterData.UUID);
                
                var listData = GetListRandom(likes, tinderData.Type);

                Trigger.ClientEvent(player, "client.phone.tinder.init", JsonConvert.SerializeObject(data), JsonConvert.SerializeObject(likesData), JsonConvert.SerializeObject(listData));
            }
        }

        private static List<List<object>> GetListRandom(List<int> likes, TinderType type, int count = 5)
        {
            var searchType = new List<TinderType>();

            if (type == TinderType.Friends)
            {
                searchType.Add(TinderType.Man);
                searchType.Add(TinderType.Woman);
                searchType.Add(TinderType.Friends);
            }
            else if (type == TinderType.Man)
                searchType.Add(TinderType.Woman);
            else if (type == TinderType.Woman)
                searchType.Add(TinderType.Man);
            
            var tinders = List
                .Where(t => t.Key.NotIn(likes))
                .Where(t => t.Value.IsVisible && searchType.Contains(t.Value.Type))
                .Select(t => t.Key)
                .ToList();
                
            var idList = NewCasino.Horses.Shuffle(tinders);

            var listData = new List<List<object>>();

            foreach (var uuid in idList)
            {
                if (!List.ContainsKey(uuid))
                    continue;
                if (!Main.PlayerNames.ContainsKey(uuid))
                    continue;

                var tinder = List[uuid];
                    
                listData.Add(new List<object>
                {
                    uuid,
                    Main.PlayerNames[uuid],
                    tinder.Avatar,
                    tinder.Text,
                });
                    
                if (listData.Count == count)
                    break;
            }

            return listData;
        }

        public static void OnAction(ExtPlayer player, int uuid, bool isLove)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            if (!List.ContainsKey(characterData.UUID))
            {
                AddOneList(player);
                return;
            }

            if (!List.ContainsKey(uuid))
            {
                AddOneList(player);
                return;
            }
            
            var tinderData = List[characterData.UUID];

            if (isLove)
            {
                tinderData.Likes.Add(uuid);

                if (List[uuid].Likes.Contains(characterData.UUID))
                {
                    var sim = Main.SimCards.FirstOrDefault(u => u.Value == uuid).Key;
                    Trigger.ClientEvent(player, "client.phone.tinder.addLikes", Main.PlayerNames[uuid], List[uuid].Avatar, Main.SimCards.ContainsKey(sim) ? sim : -1);
                    Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Tinder, LangFunc.GetText(LangType.Ru, DataName.TinderMatch), DateTime.Now);
                    var target = Main.GetPlayerByUUID(uuid);
                    if (target != null) 
                        Messages.Repository.AddSystemMessage(target, (int)DefaultNumber.Tinder, LangFunc.GetText(LangType.Ru, DataName.TinderMatch), DateTime.Now); 
                }
            }
            else
                tinderData.NoLikes.Add(uuid);

            tinderData.IsSave = true;
            
            AddOneList(player);
        }

        private static void AddOneList(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            if (!List.ContainsKey(characterData.UUID))
                return;
            
            var tinderData = List[characterData.UUID];
            
            var likes = tinderData.Likes.ToList();
            likes.AddRange(tinderData.NoLikes.ToList());
            likes.Add(characterData.UUID);
            
            var listData = GetListRandom(likes, tinderData.Type, 1);

            if (listData.Count > 0)
                Trigger.ClientEvent(player, "client.phone.tinder.addList", JsonConvert.SerializeObject(listData));
        }

        public static void OnSave(ExtPlayer player, string avatar, string text, TinderType type, bool isVisible)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null)
                return;
            
            if (text.Length < 5 || text.Length > 150)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Минимальное количество символов - 5, а максимальное - 150", 7000);
                return;
            }
            
            var isCreate = List.ContainsKey(characterData.UUID);
            var isUpdateList = false;
            
            if (isCreate)
                isUpdateList = List[characterData.UUID].Type != type;
            
            var tinderData = new TinderData
            {
                IsVisible = isVisible,
                Avatar = avatar,
                Text = text,
                Type = type,
                IsSave = true
            };

            if (isCreate)
            {
                tinderData.Likes = List[characterData.UUID].Likes;
                tinderData.NoLikes = List[characterData.UUID].NoLikes;
            }
            
            List[characterData.UUID] = tinderData;

            if (!isCreate)
            {
                Init(player);
                
                Trigger.SetTask(async () =>
                {    
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        db.Insert(new Phonetinders
                        {
                            Uuid = characterData.UUID,
                            Avatar = tinderData.Avatar,
                            Text = tinderData.Text,
                            Type = (sbyte) tinderData.Type,
                            IsVisible = tinderData.IsVisible,
                            Likes = JsonConvert.SerializeObject(tinderData.Likes),
                            NoLikes = JsonConvert.SerializeObject(tinderData.NoLikes),
                        });
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
            }
            else if (isUpdateList)
            {
                var likes = tinderData.Likes.ToList();
                likes.AddRange(tinderData.NoLikes.ToList());
                likes.Add(characterData.UUID);
            
                var listData = GetListRandom(likes, tinderData.Type, 5);

                if (listData.Count > 0)
                    Trigger.ClientEvent(player, "client.phone.tinder.addList", JsonConvert.SerializeObject(listData), true);
            }
        }
    }
}