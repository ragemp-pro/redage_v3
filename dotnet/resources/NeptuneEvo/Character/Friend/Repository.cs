using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using NeptuneEvo.Character.Config.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Players.Models;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Localization;
using NeptuneEvo.Quests;

namespace NeptuneEvo.Character.Friend
{
    class Repository
    {
        private static readonly nLog Log = new nLog("Core.Character.Friend");
        public static async Task<Dictionary<string, bool>> Load(ServerBD db, string characterName)
        {
            try
            {
                var characterFriends = await db.Friends
                    .Where(v => v.First == characterName || v.Second == characterName)
                    .ToListAsync();

                var friends = new Dictionary<string, bool>();

                foreach (var friend in characterFriends)
                {
                    var name = friend.Second == characterName ? friend.First : friend.Second;
                    friends[name] = friend.Fullname;
                }

                return friends;
            }
            catch (Exception e)
            {
                Log.Write($"LoadFriends Exception: {e.ToString()}");
            }
            return new Dictionary<string, bool>();
        }
        public static void Init(ExtPlayer player, Dictionary<string, bool> friends)
        {
            if (friends.Count > 0)
            {
                Trigger.ClientEvent(player, "setFriendList", true, friends);
            }
        }
        public static void Handshake(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null)
                    return;

                else if (sessionData.CuffedData.Cuffed || sessionData.DeathData.InDeath || player.IsInVehicle)
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;

                ExtPlayer target = sessionData.RequestData.From;
                sessionData.RequestData = new RequestData();

                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null)
                    return;

                else if (targetSessionData.CuffedData.Cuffed || targetSessionData.DeathData.InDeath || target.IsInVehicle)
                    return;

                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null)
                    return;

                string firstName = player.Name;
                string secondName = target.Name;
                if (characterData.Friends.ContainsKey(secondName) && characterData.Friends[secondName] && targetCharacterData.Friends.ContainsKey(firstName) && targetCharacterData.Friends[firstName])
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyHi), 5000);
                    Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyHi), 5000);
                    return;
                }
                characterData.Handshaked++;
                if (characterData.Handshaked == 5)
                {
                    qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage9, 1, isUpdateHud: true);
                    qMain.UpdateQuestsComplete(player, Zdobich.QuestName, (int) zdobich_quests.Stage9, true);
                }
                        
                targetCharacterData.Handshaked++;
                if (targetCharacterData.Handshaked == 5)
                {
                    qMain.UpdateQuestsStage(target, Zdobich.QuestName, (int)zdobich_quests.Stage9, 1, isUpdateHud: true);
                    qMain.UpdateQuestsComplete(target, Zdobich.QuestName, (int) zdobich_quests.Stage9, true);
                }
                
                if (!characterData.Friends.ContainsKey(secondName) || !targetCharacterData.Friends.ContainsKey(firstName))
                {
                    Trigger.SetTask(async () =>
                    {
                        try
                        {
                            await using var db = new ServerBD("MainDB");//В отдельном потоке

                            await db.InsertAsync(new Friends
                            {
                                First = firstName,
                                Second = secondName,
                                Fullname = false
                            });
                        }
                        catch (Exception e)
                        {
                            Debugs.Repository.Exception(e);
                        }
                    });

                    characterData.Friends[secondName] = false;
                    targetCharacterData.Friends[firstName] = false;

                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.HiSecond, secondName.Split('_')[0]), 5000);
                    Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.HiFirst, firstName.Split('_')[0]), 5000);
                    BattlePass.Repository.UpdateReward(target, 32);
                }
                else
                {
                    Trigger.SetTask(async () =>
                    {
                        try
                        {
	
                            await using var db = new ServerBD("MainDB");//В отдельном потоке

                            await db.Friends
                                .Where(f => (f.First == firstName && f.Second == secondName) || (f.First == secondName && f.Second == firstName))
                                .Set(f => f.Fullname, true)
                                .UpdateAsync();
                        }
                        catch (Exception e)
                        {
                            Debugs.Repository.Exception(e);
                        }
                    });

                    characterData.Friends[secondName] = true;
                    targetCharacterData.Friends[firstName] = true;
                }

                Trigger.ClientEvent(player, "setFriend", secondName, characterData.Friends[secondName]);
                Trigger.ClientEvent(target, "setFriend", firstName, targetCharacterData.Friends[firstName]);

                if (!sessionData.AntiAnimDown)
                {
                    Main.OnAntiAnim(player);
                    Trigger.PlayAnimation(player, "mp_ped_interaction", "handshake_guy_a", 39);
                    // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "handshake");
                    NAPI.Task.Run(() =>
                    {
                        try
                        {
                            if (!player.IsCharacterData()) return;
                            Main.OffAntiAnim(player);
                            Trigger.StopAnimation(player);
                        }
                        catch (Exception e)
                        {
                            Log.Write($"hanshakeTarget Task #1 Exception: {e.ToString()}");
                        }
                    }, 4500);
                }

                if (!targetSessionData.AntiAnimDown)
                {
                    Main.OnAntiAnim(target);
                    Trigger.PlayAnimation(target, "mp_ped_interaction", "handshake_guy_a", 39);
                    // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "handshake");
                    NAPI.Task.Run(() =>
                    {
                        try
                        {
                            if (!target.IsCharacterData()) return;
                            Main.OffAntiAnim(target);
                            Trigger.StopAnimation(target);
                        }
                        catch (Exception e)
                        {
                            Log.Write($"hanshakeTarget Task #2 Exception: {e.ToString()}");
                        }
                    }, 4500);
                }
            }
            catch (Exception e)
            {
                Log.Write($"hanshakeTarget Exception: {e.ToString()}");
            }
        }

        public static void ClearFriends(ExtPlayer player, string oldName, string newName = null)
        {
            try
            {
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        if (newName == null)
                        {
                            await db.Friends
                                .Where(v => v.First == oldName || v.Second == oldName)
                                .DeleteAsync();
                        }
                        else
                        {
                            await db.Friends
                                .Where(v => v.First == oldName)
                                .Set(v => v.First, newName)
                                .UpdateAsync();

                            await db.Friends
                                .Where(v => v.Second == oldName)
                                .Set(v => v.Second, newName)
                                .UpdateAsync();
                        }
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });

                ClearFriendsForPlayer(oldName, newName);

                var characterData = player.GetCharacterData();
                if (characterData != null)
                {
                    characterData.Friends = new Dictionary<string, bool>();
                    Trigger.ClientEvent(player, "setFriendList", true, characterData.Friends);
                }
            }
            catch (Exception e)
            {
                Log.Write($"ClearFriends Exception: {e.ToString()}");
            }
        }

        private static void ClearFriendsForPlayer(string oldName, string newName)
        {
            try
            {
                foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                {
                    var targetCharacterData = foreachPlayer.GetCharacterData();
                    if (targetCharacterData == null) continue;
                    else if (!targetCharacterData.Friends.ContainsKey(oldName)) continue;

                    bool isFullname = targetCharacterData.Friends[oldName];

                    targetCharacterData.Friends.Remove(oldName);

                    if (newName != null)
                        targetCharacterData.Friends[newName] = isFullname;

                    Trigger.ClientEvent(foreachPlayer, "setFriendList", true, targetCharacterData.Friends);
                }
            }
            catch (Exception e)
            {
                Log.Write($"ClearFriends Task #1 Exception: {e.ToString()}");
            }

        }

    }
}
