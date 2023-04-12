using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using GTANetworkAPI;
using LinqToDB;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Handles;
using NeptuneEvo.Organizations.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Players;
using NeptuneEvo.Table.Models;
using NeptuneEvo.Utils;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Fractions.Table.Rank
{
    public class Repository
    {
        
        public static void RankAccessLoad(ExtPlayer player, int id)
        {
            try
            {
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null) 
                    return;
                
                if (!fractionData.IsLeader(memberFractionData.Rank))
                    return;
                
                if (!fractionData.Ranks.ContainsKey(id)) 
                    return;
                
                var accessList = NeptuneEvo.Table.Repository.GetAccess(fractionData.Ranks[id].Access);
                
                Trigger.ClientEvent(player, "client.frac.main.rankAccessInit", JsonConvert.SerializeObject(accessList));
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static void UpdateRankAccess(ExtPlayer player, int id, string json)
        {
            try
            {
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null) 
                    return;
                
                if (!fractionData.IsLeader(memberFractionData.Rank))
                    return;
                
                if (!fractionData.Ranks.ContainsKey(id)) 
                    return;

                var access = fractionData.Ranks[id].Access;
                
                var newAccess = JsonConvert.DeserializeObject<Dictionary<int, int>>(json);

                var isSave = false;
                
                foreach (var dict in newAccess)
                {
                    var key = (RankToAccess) dict.Key;
                    var type = (AccessType) dict.Value;

                    if (type == AccessType.Add && !access.Contains(key))
                    {
                        access.Add(key);
                        isSave = true;
                    }
                    else if (type == AccessType.Remove && access.Contains(key))
                    {
                        access.Remove(key);
                        isSave = true;
                    }
                }
                
                if (isSave)
                    fractionData.SaveRank(id);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        public static void UpdateRankName(ExtPlayer player, int id, string name)
        {
            try
            {
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null) 
                    return;
                
                if (!fractionData.IsLeader(memberFractionData.Rank))
                    return;
                
                if (!fractionData.Ranks.ContainsKey(id)) 
                    return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                if (DateTime.Now < sessionData.TimingsData.NextGlobalChat)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Block10Min), 4500);
                    return;
                }
                
                name = Main.BlockSymbols(Main.RainbowExploit(name));
                if (name.Length > 25)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxRankNameLenght), 4500);
                    return;
                }
                
                string testmsg = name.ToLower();
                if (Main.stringGlobalBlock.Any(c => testmsg.Contains(c)))
                {
                    sessionData.TimingsData.NextGlobalChat = DateTime.Now.AddMinutes(10);
                    Trigger.SendToAdmins(3, "!{#636363}[A] " + LangFunc.GetText(LangType.Ru, DataName.AdminAlertFTableNews, player.Name, player.Value, name));
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RestrictedWordsTableNews), 15000);
                    return;
                }

                if (fractionData.Ranks.Values.Any(p => p.Name.ToLower() == name.ToLower()))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ExistNameRank), 4500);
                    return;
                }

                fractionData.Ranks[id].Name = name;
                fractionData.SaveRank();

                var ranksName = NeptuneEvo.Table.Repository.GetRanksData(fractionData.Ranks);
                Trigger.ClientEvent(player, "client.frac.main.setRanks", JsonConvert.SerializeObject(ranksName));
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public static void UpdateRankScore(ExtPlayer player, int id, int score)
        {
            try
            {
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null) 
                    return;
                
                if (!fractionData.IsLeader(memberFractionData.Rank))
                    return;
                
                if (!fractionData.Ranks.ContainsKey(id)) 
                    return;

                if (score > NeptuneEvo.Table.Repository.MaxScore || NeptuneEvo.Table.Repository.MinScore > score)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Минимальное значение {NeptuneEvo.Table.Repository.MinScore} максимальное {NeptuneEvo.Table.Repository.MaxScore}", 4500);
                    return;
                }
                
                fractionData.Ranks[id].MaxScore = score;
                //fractionData.SaveRank();

                var ranksName = NeptuneEvo.Table.Repository.GetRanksData(fractionData.Ranks);
                Trigger.ClientEvent(player, "client.frac.main.setRanks", JsonConvert.SerializeObject(ranksName));
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        //
        
        public static void DeletePlayer(ExtPlayer player, int uuid)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.UnInvite)) return;
                
                var target = Main.GetPlayerByUUID(uuid);
                if (target != null)
                    FractionCommands.UnInviteFromFraction(player, target);
                else
                {
                    var memberFractionData = player.GetFractionMemberData();
                    if (memberFractionData == null) 
                        return;

                    var fracId = memberFractionData.Id;
                    
                    var targetMemberFractionData = Fractions.Manager.GetFractionMemberData(uuid, memberFractionData.Id);
                    if (targetMemberFractionData == null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрока не найдено в Вашей семье", 3000);
                        return;
                    }
                    
                    if (targetMemberFractionData.Rank >= memberFractionData.Rank)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantUvolit), 3000);
                        return;
                    }
                    
                    Fractions.Player.Repository.RemoveFractionMemberData(fracId, uuid);
                    
                    Logs.Repository.AddLogs(player, FractionLogsType.UnInvite, $"Выгнал {targetMemberFractionData.Name} ({targetMemberFractionData.UUID})");
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TiUvolil, targetMemberFractionData.Name), 3000);
                }
                Player.Repository.GetMembers(player);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public static void UpdatePlayerRank(ExtPlayer player, int uuid, int rank)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.SetRank)) return;
                
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null) 
                    return;

                var fracId = memberFractionData.Id;
                
                var target = Main.GetPlayerByUUID(uuid);
                if (target != null)
                    FractionCommands.SetFracRank(player, target, rank);
                else
                    FractionCommands.SetFracRankOffline(player, uuid, rank);
                
                Player.Repository.UpdateMember(player, fracId, uuid);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static void UpdateMemberRankAccess(ExtPlayer player, int uuid, string json)
        {            
            try
            {
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null) 
                    return;
                
                if (!fractionData.IsLeader(memberFractionData.Rank))
                    return;

                var fracId = fractionData.Id;
                
                var targetMemberFractionData = Manager.GetFractionMemberData(uuid, fractionData.Id);
                if (targetMemberFractionData == null)
                    return;

                var access = targetMemberFractionData.Access;
                var locks = targetMemberFractionData.Lock;
                
                var newAccess = JsonConvert.DeserializeObject<Dictionary<int, int>>(json);

                var isSave = false;
                
                foreach (var dict in newAccess)
                {
                    var key = (RankToAccess) dict.Key;
                    var type = (AccessType) dict.Value;


                    if (type == AccessType.Skip)
                    {
                        if (access.Contains(key))
                        {
                            access.Remove(key);
                            isSave = true;
                        }

                        if (locks.Contains(key))
                        {
                            locks.Remove(key);
                            isSave = true;
                        }
                    }
                    else if (type == AccessType.Add)
                    {
                        if (!access.Contains(key))
                        {
                            access.Add(key);
                            isSave = true;
                        }

                        if (locks.Contains(key))
                        {
                            locks.Remove(key);
                            isSave = true;
                        }
                    }
                    else if (type == AccessType.Remove)
                    {
                        if (access.Contains(key))
                        {
                            access.Remove(key);
                            isSave = true;
                        }

                        if (!locks.Contains(key))
                        {
                            locks.Add(key);
                            isSave = true;
                        }
                    }
                }

                if (isSave)
                {
                    Fractions.Player.Repository.UpdateAccess(fracId, uuid, access, locks);
                    Player.Repository.UpdateMember(player, fracId, uuid);
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        
        public static void DefaultRanks(ExtPlayer player)
        {
            try
            {
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null) 
                    return;
                
                var fractionData = player.GetFractionData();
                if (fractionData == null) 
                    return;
                
                if (!fractionData.IsLeader(memberFractionData.Rank)) return;
                
                foreach (var id in fractionData.Ranks.Keys.ToList())
                {
                    if (!fractionData.Ranks.ContainsKey(id))
                        continue;
                    if (!fractionData.DefaultRanks.ContainsKey(id))
                        continue;

                    var rankData = fractionData.Ranks[id];
                    var defaultRankData = fractionData.DefaultRanks[id];

                    rankData.Name = defaultRankData.Name;
                    rankData.Access = defaultRankData.Access;
                    rankData.MaxScore = defaultRankData.MaxScore;
                }
                
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы сбросили настройки рангов", 5000);
                var ranksName = NeptuneEvo.Table.Repository.GetRanksData(fractionData.Ranks);
                Trigger.ClientEvent(player, "client.frac.main.setRanks", JsonConvert.SerializeObject(ranksName));
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
    }
}