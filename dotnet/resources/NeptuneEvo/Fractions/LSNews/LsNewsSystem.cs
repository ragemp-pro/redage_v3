using System;
using System.Collections.Generic;
using GTANetworkAPI;
using NeptuneEvo.Core;
using Redage.SDK;
using NeptuneEvo.GUI;
using System.Data;
using System.Linq;

using NeptuneEvo.Chars;
using NeptuneEvo.Functions;
using Database;
using NeptuneEvo.Quests;
using Newtonsoft.Json;
using LinqToDB;
using System.Threading.Tasks;
using Localization;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Handles;
using NeptuneEvo.Players.Phone.Messages.Models;
using NeptuneEvo.Players.Phone.News.Models;
using NeptuneEvo.Quests.Models;
using NeptuneEvo.Table.Models;
using NeptuneEvo.Table.Tasks.Models;
using NeptuneEvo.Table.Tasks.Player;

namespace NeptuneEvo.Fractions.LSNews
{
    class LsNewsSystem : Script
    {
        private static readonly nLog Log = new nLog("Fractions.LSNews");

        private class CompletedAdvert
        {
            public string Author { get; set; }
            public int AuthorSIM { get; set; }
            public string EditedAD { get; set; }
            public string Editor { get; set; }
            public bool IsPremium { get; set; }

            public CompletedAdvert(string author, int sim, string ad, string editor, bool isPremium)
            {
                Author = author;
                AuthorSIM = sim;
                EditedAD = ad;
                Editor = editor;
                IsPremium = isPremium;
            }
        }

        private static Queue<CompletedAdvert> CompletedAdverts = new Queue<CompletedAdvert>();
        

        public static Dictionary<int, AdvertData> AdvertList = new Dictionary<int, AdvertData>();

        public static Vector3[] LSNewsCoords = new Vector3[2]
        {
            new Vector3(-577.5707, -922.6645, 32.5247), // Переодевалка
            new Vector3(-586.7227, -921.56085, 23.869083),
        };

        public static Vector3[] FirstNewsLiftTP = new Vector3[3]
        {
            new Vector3(-586.22815, -938.64666, 23.864021), // Колшэйп 1 этаж
            new Vector3(-586.0864, -938.83435, 28.18065), // Колшэйп 2 этаж
            new Vector3(-586.119, -938.60675, 32.524727) // Колшэйп 3 этаж
        };

        public static Vector3[] SecondNewsLiftTP = new Vector3[3]
        {
            new Vector3(-590.3382, -938.5576, 23.868172), // Колшэйп 1 этаж
            new Vector3(-590.4067, -938.49384, 28.180655), // Колшэйп 2 этаж
            new Vector3(-590.4124, -938.66785, 32.524727) // Колшэйп 3 этаж
        };

        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStartHandler()
        {
            try
            {
                CustomColShape.CreateCylinderColShape(LSNewsCoords[0], 1f, 2, 0, ColShapeEnums.FractionLSNews, 1);
                NAPI.Marker.CreateMarker(30, LSNewsCoords[0], new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220));
                
                for (int i = 0; i < FirstNewsLiftTP.Length; i++) 
                { 
                    CustomColShape.CreateCylinderColShape(FirstNewsLiftTP[i], 1, 2, 0, ColShapeEnums.FractionLSNews, 2); 
                    NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Лифт"), new Vector3(FirstNewsLiftTP[i].X, FirstNewsLiftTP[i].Y, FirstNewsLiftTP[i].Z), 5F, 0.3F, 0, new Color(255, 255, 255)); 
                    NAPI.Marker.CreateMarker(1, FirstNewsLiftTP[i] - new Vector3(0, 0, 1.25), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220)); 
                }
                
                for (int i = 0; i < SecondNewsLiftTP.Length; i++) 
                { 
                    CustomColShape.CreateCylinderColShape(SecondNewsLiftTP[i], 1, 2, 0, ColShapeEnums.FractionLSNews, 3); 
                    NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Лифт"), new Vector3(SecondNewsLiftTP[i].X, SecondNewsLiftTP[i].Y, SecondNewsLiftTP[i].Z), 5F, 0.3F, 0, new Color(255, 255, 255)); 
                    NAPI.Marker.CreateMarker(1, SecondNewsLiftTP[i] - new Vector3(0, 0, 1.25), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220)); 
                }

                PedSystem.Repository.CreateQuest("ig_molly", new Vector3(-586.7227, -921.56085, 23.869083), 137.71083f, title: "~y~NPC~w~ Дженнифер", colShapeEnums: ColShapeEnums.FracLSNews);
                PedSystem.Repository.CreateQuest("ig_englishdave_02", new Vector3(-588.256, -936.6138, 23.867977), -4.159257f, title: "~y~NPC~w~ Журналист Шаманчик\nВызвать сотрудника", colShapeEnums: ColShapeEnums.CallNewsMember);
            }
            catch (Exception e)
            {
                Log.Write($"OnResourceStartHandler Exception: {e.ToString()}");
            }
        }

        [Interaction(ColShapeEnums.FracLSNews)]
        public static void Open(ExtPlayer player, int index)
        {
            if (!player.IsCharacterData()) return;

            player.SelectQuest(new PlayerQuestModel("npc_fracnews", 0, 0, false, DateTime.Now));
            Trigger.ClientEvent(player, "client.quest.open", index, "npc_fracnews", 0, 0, 0);
        }
        
        [Interaction(ColShapeEnums.FractionLSNews)]
        public static void OnFractionLSNews(ExtPlayer player, int interact)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                switch (interact)
                {
                    case 1:
                        if (player.GetFractionId() == (int) Models.Fractions.LSNEWS) FractionClothingSets.OpenFractionClothingSetsMenu(player);
                        else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoNEWS), 3000);
                        return;
                    case 2:
                    case 3:
                        if (sessionData.Following != null) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsFollowing), 3000); 
                            return; 
                        } 
 
                        Trigger.ClientEvent(player, "openNewsLiftMenu", interact);
                        return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"OnFractionLSNews Exception: {e.ToString()}");
            }
        }
        
        [RemoteEvent("server.useNewsLift")] 
        public static void UseNewsLift(ExtPlayer player, byte lift, int index) 
        { 
            try 
            { 
                var sessionData = player.GetSessionData(); 
                if (sessionData == null) return; 
 
                var characterData = player.GetCharacterData(); 
                if (characterData == null) return; 
 
                if (characterData.DemorganTime >= 1 || sessionData.CuffedData.Cuffed || sessionData.DeathData.InDeath || sessionData.AntiAnimDown || !characterData.IsAlive) return; 
                 
                if (lift == 1) 
                { 
                    if (player.Position.DistanceTo(FirstNewsLiftTP[index]) <= 2) 
                    { 
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyAtFloor), 3000); 
                        return; 
                    } 
 
                    player.Position = FirstNewsLiftTP[index]; 
                } 
                else if (lift == 2) 
                { 
                    if (player.Position.DistanceTo(SecondNewsLiftTP[index]) <= 2) 
                    { 
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyAtFloor), 3000); 
                        return; 
                    } 
 
                    player.Position = SecondNewsLiftTP[index]; 
                }
 
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GoingToFloor, index + 1), 3000); 
            } 
            catch (Exception e) 
            { 
                Log.Write($"UseNewsLift Exception: {e.ToString()}"); 
            } 
        }
        
        public static async void Init()
        {
            try
            {
                await using var db = new ServerBD("MainDB");//При старте сервера

                var list = db.Advertised
                    .Where(a => a.Status == false)
                    .OrderByDescending(a => a.ID)
                    .ToList();

                foreach (var advert in list)
                {
                    AdvertData _advert = new AdvertData()
                    {
                        ID = advert.ID,
                        Author = advert.Author,
                        AuthorSIM = advert.AuthorSIM,
                        AD = advert.AD,
                        Editor = advert.Editor,
                        EditedAD = advert.EditedAD,
                        Opened = advert.Opened,
                        Closed = advert.Closed,
                        Status = advert.Status
                    };
                    AdvertList.Add(_advert.ID, _advert);
                }
                Timers.Start("advertsCompleted", 10000, () => CheckCompletedAds(), true);
            }
            catch (Exception e)
            {
                Log.Write($"StartWork Exception: {e.ToString()}");
            }
        }
        public static bool IsAdvertToName(string Name)
        {
            try
            {
                return AdvertList.Values.Any(x => x.Author == Name);
            }
            catch (Exception e)
            {
                Log.Write($"IsAdvertToName Exception: {e.ToString()}");
                return false;
            }
        }
        public static void onLSNPlayerLoad(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                //foreach (var ad in AdvertList.Values)
                //    Trigger.ClientEvent(player, "client.advert.add", JsonConvert.SerializeObject(ad));
                Trigger.ClientEvent(player, "client.advert.init", JsonConvert.SerializeObject(AdvertList.Values));
            }
            catch (Exception e)
            {
                Log.Write($"onLSNPlayerLoad Exception: {e.ToString()}");
            }
        }
        
        [Interaction(ColShapeEnums.CallNewsMember)] 
        public static void OpenCallNewsMemberDialog(ExtPlayer player, int _) 
        { 
            try 
            { 
                var sessionData = player.GetSessionData(); 
                if (sessionData == null) return; 
 
                if (sessionData.CuffedData.Cuffed) 
                { 
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsCuffed), 3000); 
                    return; 
                } 
                else if (sessionData.DeathData.InDeath) 
                { 
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsDying), 3000); 
                    return; 
                } 
                else if (Main.IHaveDemorgan(player, true)) return; 
 
                Trigger.ClientEvent(player, "openDialog", "CallNewsMemberDialog", LangFunc.GetText(LangType.Ru, DataName.AreYouWantToCallGov)); 
            } 
            catch (Exception e) 
            { 
                Log.Write($"OpenCallNewsMemberDialog Exception: {e.ToString()}"); 
            } 
        }

        #region Remote Events
        [RemoteEvent("server.advert.take")]
        public static void AdvertTake(ExtPlayer player, int index)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (!player.IsFractionAccess(RankToAccess.Ads)) return;
                
                if (!AdvertList.ContainsKey(index))
                {
                    Remove(index, player);
                    return;
                }
                var advert = AdvertList[index];
                if (advert.Editor != null && advert.Editor.Length > 1 && !advert.Editor.Equals(player.Name)) return;
                if (advert.Status)
                {
                    Remove(index, player);
                    return;
                }
                else if (AdvertList.Values.Any(a => a.Editor == player.Name && a.ID != index))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AdAlreadyTaken), 3000);
                    return;
                }

                if (advert.Editor != null && advert.Editor.Length > 1 && advert.Editor.Equals(player.Name)) 
                    advert.Editor = "";
                else 
                    advert.Editor = player.Name;

                foreach (var foreachPlayer in Character.Repository.GetPlayers())
                {
                    try
                    {
                        var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                        if (foreachMemberFractionData == null) 
                            continue;
                        
                        if (foreachMemberFractionData.Id != (int) Models.Fractions.LSNEWS) 
                            continue;
                        
                        Trigger.ClientEvent(foreachPlayer, "client.advert.update", index, advert.Editor);
                    }
                    catch (Exception e)
                    {
                        Log.Write($"AdvertTake Foreach Exception: {e.ToString()}");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"AdvertTake Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.advert.send")]
        public static void AdvertSend(ExtPlayer player, int ID, string answer)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                else if (!player.IsFractionAccess(RankToAccess.Ads)) return;
                if (!AdvertList.ContainsKey(ID)) return;
                if (!AdvertList[ID].Status) UpdateAnswer(player, ID, answer);
                else
                {
                    Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.AdUnavaibleForEdit));
                    Remove(ID, player);
                }
            }
            catch (Exception e)
            {
                Log.Write($"AdvertSend Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.advert.delete")]
        public static void AdvertDelete(ExtPlayer player, int ID, string reason)
        {
            try
            {
                if (player.GetFractionId() == (int) Models.Fractions.LSNEWS)
                {
                    if (!player.IsFractionAccess(RankToAccess.Delad)) return;
                    UpdateAnswer(player, ID, reason, true);
                }
                else if (CommandsAccess.CanUseCmd(player, AdminCommands.Delad)) UpdateAnswer(player, ID, reason, true);
            }
            catch (Exception e)
            {
                Log.Write($"AdvertDelete Exception: {e.ToString()}");
            }
        }
        #endregion

        public static async Task AddAdvert(ExtPlayer player, string text, string link, int type, bool isPremium, int price)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                text = Main.BlockSymbols(text);
                GameLog.Money($"bank({characterData.Bank})", $"server", price, "ad");
                sessionData.TimingsData.NextAD = DateTime.Now.AddMinutes(15);
                //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AdDone), 3000);
                Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.News, LangFunc.GetText(LangType.Ru, DataName.AdDone), DateTime.Now);
                BattlePass.Repository.UpdateReward(player, 11);
                await using var db = new ServerBD("MainDB");//В отдельном потоке

                var advert = new AdvertData
                {
                    Author = sessionData.Name,
                    AuthorSIM = characterData.Sim,
                    AD = text,
                    Link = link,
                    Editor = "",
                    EditedAD = "",
                    Opened = DateTime.Now,
                    Closed = DateTime.MinValue,
                    Status = false,
                    Type = type,
                    IsPremium = isPremium
                };
                int itemSqlID = await db.InsertWithInt32IdentityAsync(new Advertiseds
                {
                    Author = sessionData.Name,
                    AuthorSIM = characterData.Sim,
                    AD = text,
                    Link = link,
                    Editor = "",
                    EditedAD = "",
                    Opened = DateTime.Now,
                    Closed = DateTime.MinValue,
                    Status = false,
                    Type = (sbyte) type,
                    IsPremium = isPremium
                });
                advert.ID = itemSqlID;
                AdvertList.Add(advert.ID, advert);
                AddPlayer(JsonConvert.SerializeObject(advert));
            }
            catch (Exception e)
            {
                Log.Write($"AddAdvert Exception: {e.ToString()}");
            }
        }

        public static void UpdateAnswer(ExtPlayer player, int repID, string response, bool deleted = false)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                		
                var memberFractionData = player.GetFractionMemberData();
                if (characterData.AdminLVL == 0 && memberFractionData == null)
                    return;
                
                if (characterData.AdminLVL == 0 && memberFractionData.Id != (int) Models.Fractions.LSNEWS) return;
                if (characterData.AdminLVL == 0 && memberFractionData.Id == (int) Models.Fractions.LSNEWS && memberFractionData.Rank <= 1)
                {
                    AdvertTake(player, repID);
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoAccess), 3000);
                    return;
                }
                if (!AdvertList.ContainsKey(repID))
                {
                    if (deleted) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindAdNumber), 3000);
                    return;
                }
                response = Main.BlockSymbols(response);
                if (response.Length >= 150)
                {
                    AdvertTake(player, repID);
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AdMustBe150), 3000);
                    return;
                }

                var advert = AdvertList[repID];
                if (!deleted)
                {
                    int moneyad = Convert.ToInt32((advert.AD.Length / 7 * Main.AdSymbCost) * Main.AdEditorCost);
                    MoneySystem.Bank.Change(characterData.Bank, moneyad, false);
                    CompletedAdverts.Enqueue(new CompletedAdvert(advert.Author.Replace('_', ' '), advert.AuthorSIM, response, player.Name.Replace('_', ' '), advert.IsPremium));
                    Players.Phone.News.Repository.AddList(advert, response);
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AdToList, advert.Author, CompletedAdverts.Count), 3000);
                    player.AddTableScore(TableTaskId.Item34);
                    var target = (ExtPlayer) NAPI.Player.GetPlayerFromName(advert.Author);
                    if (target.IsCharacterData()) 
                        Players.Phone.Messages.Repository.AddSystemMessage(target, (int)DefaultNumber.News, LangFunc.GetText(LangType.Ru, DataName.AdToList, advert.Author, CompletedAdverts.Count), DateTime.Now);
                }
                else
                {
                    if (characterData.AdminLVL != 0) GameLog.Admin($"{player.Name}", $"delAd", $"{advert.Author}");
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DelAd, advert.Author), 3000);
                    var target = (ExtPlayer) NAPI.Player.GetPlayerFromName(advert.Author);
                    response += LangFunc.GetText(LangType.Ru, DataName.deletedad);
                    if (target.IsCharacterData())
                        Players.Phone.Messages.Repository.AddSystemMessage(target, (int)DefaultNumber.News, LangFunc.GetText(LangType.Ru, DataName.DelAdReason, player.Name, response), DateTime.Now);
                }
                Remove(repID);
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке
                    
                        await db.Advertised
                            .Where(a => a.ID == repID)
                            .Set(a => a.Editor, sessionData.Name)
                            .Set(a => a.EditedAD, response)
                            .Set(a => a.Closed, DateTime.Now)
                            .Set(a => a.Status, true)
                            .UpdateAsync();
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
            }
            catch (Exception e)
            {
                Log.Write($"AddAnswer Exception: {e.ToString()}");
            }
        }
        public static void OnDisconnect(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                var fracId = player.GetFractionId();
                if (characterData.AdminLVL == 0 && fracId != (int) Models.Fractions.LSNEWS) return;
                if (AdvertList.Count > 0)
                {
                    foreach (AdvertData ad in AdvertList.Values)
                    {
                        try
                        {
                            if (ad == null) continue;
                            else if (ad.Editor != null && ad.Editor.Equals(player.Name))
                            {
                                AdvertTake(player, ad.ID);
                                break;
                            }
                            else if (ad.Status)
                            {
                                Remove(ad.ID);
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Write($"OnDisconnect Foreach Exception: {e.ToString()}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"OnDisconnect Exception: {e.ToString()}");
            }
        }
        public static void CheckCompletedAds()
        {
            try
            {
                if (CompletedAdverts.Count >= 1)
                {
                    var ad = CompletedAdverts.Dequeue();

                    var color = ad.IsPremium ? "!{#f5a952}" : "!{#00CC00}";
                    var color2 = ad.IsPremium ? "!{#bf7c39}" : "!{#009900}";

                    if (ad.AuthorSIM >= 1) NAPI.Chat.SendChatMessageToAll(color + LangFunc.GetText(LangType.Ru, DataName.AdWithPhone, ad.EditedAD, ad.AuthorSIM));
                    else NAPI.Chat.SendChatMessageToAll(color + LangFunc.GetText(LangType.Ru, DataName.Ad, ad.EditedAD));
                    NAPI.Chat.SendChatMessageToAll(color2 + LangFunc.GetText(LangType.Ru, DataName.AdRewriter, ad.Editor));
                }
                
                //
                
                Players.Phone.News.Repository.Dell();
            }
            catch (Exception e)
            {
                Log.Write($"CheckCompletedAds Exception: {e.ToString()}");
            }
        }
        private static void AddPlayer(string advert)
        {
            try
            {
                foreach (var foreachPlayer in Character.Repository.GetPlayers())
                {
                    try
                    {
                        var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                        if (foreachMemberFractionData == null) 
                            continue;
                        
                        if (foreachMemberFractionData.Id != (int) Models.Fractions.LSNEWS) 
                            continue;
                        
                        Trigger.ClientEvent(foreachPlayer, "client.advert.add", advert);
                    }
                    catch (Exception e)
                    {
                        Log.Write($"AddPlayer Foreach Exception: {e.ToString()}");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"AddPlayer Exception: {e.ToString()}");
            }
        }
        private static void Remove(int id, ExtPlayer player = null)
        {
            try
            {
                if (AdvertList.ContainsKey(id)) 
                    AdvertList.Remove(id);
                
                if (player == null)
                {
                    foreach (var foreachPlayer in Character.Repository.GetPlayers())
                    {
                        try
                        {
                            var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                            if (foreachMemberFractionData == null) 
                                continue;
                            
                            if (foreachMemberFractionData.Id != (int) Models.Fractions.LSNEWS) 
                                continue;
                            
                            Trigger.ClientEvent(foreachPlayer, "client.advert.remove", id);
                        }
                        catch (Exception e)
                        {
                            Log.Write($"Remove Foreach Exception: {e.ToString()}");
                        }
                    }
                }
                else
                {
                    var memberFractionData = player.GetFractionMemberData();
                    if (memberFractionData == null)
                        return;
                    if (memberFractionData.Id != (int) Models.Fractions.LSNEWS) 
                        return;
                    
                    Trigger.ClientEvent(player, "client.advert.remove", id);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Remove Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.advert.getlogs")]
        public static void getLogs(ExtPlayer player, int pageId)
        {
            if (!player.IsCharacterData()) return;
            else if (!player.IsFractionAccess(RankToAccess.Ads)) return;
            Trigger.SetTask(async () =>
            {
                try
                {
                    int skip = 20;
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    var logs = await db.Advertised
                        .Where(v => v.Status == true)
                        .OrderByDescending(v => v.ID)
                        .Skip(pageId * skip)
                        .Take(skip)
                        .ToListAsync();

                    if (logs != null && logs.Count > 0)
                    {
                        Trigger.ClientEvent(player, "client.advert.logs", JsonConvert.SerializeObject(logs));
                    }
                }
                catch (Exception e)
                {
                    Log.Write($"getLogs Exception: {e.ToString()}");
                }
            });
        }
        [RemoteEvent("server.advert.phone")]
        public static void OnPhone(ExtPlayer player, int index)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                else if (!player.IsFractionAccess(RankToAccess.Ads)) return;
                if (!AdvertList.ContainsKey(index))
                {
                    Remove(index, player);
                    return;
                }
                int number = AdvertList[index].AuthorSIM;
                Trigger.ClientEvent(player, "escape");
                Players.Phone.Call.Repository.OnCall(player, number);
            }
            catch (Exception e)
            {
                Log.Write($"OnPhone Exception: {e.ToString()}");
            }
        }

        [Command("tabs")]
        public void openAbs(ExtPlayer player)
        {
            
            Trigger.ClientEvent(player, "client.advert.open");
        }

        //DateTime.Now.Subtract(new TimeSpan(3, 0, 0, 0))
        /*[RemoteEvent("server.advert.getStats")]
        public static void OnStats(ExtPlayer player, int index)
        {
            if (!player.IsCharacterData()) return;
            else if (!Manager.canUseCommand(player, RankToAccess.Ads)) return;
            using (var db = new ServerBD("MainDB"))
            {
                List<AdvertisedsTable> logs = db.Advertiseds
                    .Where(v => v.Status == true && v.Closed >= DateTime.Now.Subtract(new TimeSpan(index, 0, 0, 0)))
                    .ToList();

                Dictionary<string, Dictionary<string, object>> PlayerData = new Dictionary<string, Dictionary<string, object>>();
                if (logs != null && logs.Count > 0)
                {
                    foreach(AdvertisedsTable log in logs)
                    {
                        if (!PlayerData.ContainsKey(log.Editor))
                        {
                            PlayerData.Add(log.Editor, new Dictionary<string, object>());

                            PlayerData[log.Editor].Add("Name", log.Editor);
                            PlayerData[log.Editor].Add("Count", 0);
                        }
                        PlayerData[log.Editor]["Count"] = Convert.ToInt32(PlayerData[log.Editor]["Count"]) + 1;
                    }
                    Trigger.ClientEvent(player, "client.advert.stats", JsonConvert.SerializeObject(PlayerData));
                }
            }
        }*/
    }
}