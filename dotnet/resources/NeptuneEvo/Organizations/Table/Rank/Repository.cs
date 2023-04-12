using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using GTANetworkAPI;
using LinqToDB;
using Localization;
using NeptuneEvo.Accounts;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Handles;
using NeptuneEvo.Organizations.Models;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.Players;
using NeptuneEvo.Table.Models;
using NeptuneEvo.Utils;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Organizations.Table.Rank
{
    public class Repository
    {
        
        public static void RankAccessLoad(ExtPlayer player, int id)
        {
            try
            {
                if (!player.IsOrganizationAccess(RankToAccess.SetRank))
                    return;
                
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
            
                if (!organizationData.Ranks.ContainsKey(id)) 
                    return;
                
                var accessList = NeptuneEvo.Table.Repository.GetAccess(organizationData.Ranks[id].Access);
                
                Trigger.ClientEvent(player, "client.org.main.rankAccessInit", JsonConvert.SerializeObject(accessList));
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static void UpdateRanksId(ExtPlayer player, string json)
        {
            try
            {
                if (!player.IsOrganizationAccess(RankToAccess.SetRank))
                    return;
                
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
            
                var orgId = organizationData.Id;
                
                var updateRankId = JsonConvert.DeserializeObject<Dictionary<int, int>>(json);

                var ranks = new Dictionary<int, RankData>();

                foreach (var rank in updateRankId)
                {
                    var item1 = organizationData.Ranks[rank.Key].Clone();
                    var item2 = organizationData.Ranks[rank.Value].Clone();

                    item1.MaxScore = item2.MaxScore;

                    ranks[rank.Value] = item1;
                }
                
                foreach (var rank in ranks) {
                    organizationData.Ranks[rank.Key] = rank.Value;
                }
                
                foreach (var foreachMemberOrganizationData in Manager.AllMembers[orgId].ToList())
                {
                    if (updateRankId.ContainsKey(foreachMemberOrganizationData.Rank))
                    {
                        Organizations.Player.Repository.SetRank(orgId, foreachMemberOrganizationData.UUID, updateRankId[foreachMemberOrganizationData.Rank]);
                    }
                }
                
                organizationData.SaveRank();
                
                var ranksName = NeptuneEvo.Table.Repository.GetRanksData(organizationData.Ranks);
                Trigger.ClientEvent(player, "client.org.main.setRanks", JsonConvert.SerializeObject(ranksName));
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
                if (!player.IsOrganizationAccess(RankToAccess.SetRank))
                    return;
                
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
            
                if (!organizationData.Ranks.ContainsKey(id)) 
                    return;

                var access = organizationData.Ranks[id].Access;
                
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
                    organizationData.SaveRank();
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        public static void CreateRank(ExtPlayer player, string name, int score)
        {
            try
            {
                if (!player.IsOrganizationAccess(RankToAccess.SetRank))
                    return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                if (DateTime.Now < sessionData.TimingsData.NextGlobalChat)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Block10Min), 4500);
                    return;
                }
                
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
            
                if (!organizationData.IsLeader(player.GetUUID()))
                    return;
                
                if (organizationData.Ranks.Count >= NeptuneEvo.Table.Repository.MaxRankCount)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxRanksAmount), 4500);
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

                if (organizationData.Ranks.Values.Any(p => p.Name.ToLower() == name.ToLower()))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ExistNameRank), 4500);
                    return;
                }
                
                int index = -1;
                int maxIndex = 0;
                for (int i = 1; i <= NeptuneEvo.Table.Repository.MaxRankCount; i++)
                {
                    if (index == -1 && !organizationData.Ranks.ContainsKey(i))
                    {
                        maxIndex = i;
                        index = i;
                    }

                    if (i > maxIndex && organizationData.Ranks.ContainsKey(i))
                        maxIndex = i;
                }

                var rankData = new RankData
                {
                    Name = name,
                    Salary = 0,
                    MaxScore = score,
                    Access = new List<RankToAccess>(),
                };
                
                organizationData.Ranks.Add(index, rankData);
                organizationData.SaveRank();

                var ranksName = NeptuneEvo.Table.Repository.GetRanksData(organizationData.Ranks);
                Trigger.ClientEvent(player, "client.org.main.setRanks", JsonConvert.SerializeObject(ranksName));
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public static void RemoveRank(ExtPlayer player, int id)
        {
            try
            {
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
            
                if (!organizationData.IsLeader(player.GetUUID()))
                    return;
                
                if (!organizationData.Ranks.ContainsKey(id)) 
                    return;
                
                organizationData.Ranks.Remove(id);
                
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.Organizations
                            .Where(v => v.Organization == organizationData.Id)
                            .Set(v => v.Ranks, JsonConvert.SerializeObject(organizationData.Ranks))
                            .UpdateAsync();

                        await db.Orgvehicles
                            .Where(v => v.Organization == organizationData.Id && v.Rank == id)
                            .Set(v => v.Rank, 0)
                            .UpdateAsync();

                        await db.Orgranks
                            .Where(v => v.Id == organizationData.Id && v.Rank == id)
                            .Set(v => v.Rank, (sbyte)0)
                            .UpdateAsync();
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
                
                var vehiclesNumber = new List<string>();

                foreach (var vehicleData in organizationData.Vehicles)
                {
                    if (!VehicleData.LocalData.Repository.IsVehicleToNumber(VehicleAccess.Organization, vehicleData.Key)) continue;
                    vehiclesNumber.Add(vehicleData.Key);
                }
                
                foreach (string vehicleNumber in vehiclesNumber)
                {
                    if (!organizationData.Vehicles.ContainsKey(vehicleNumber)) continue;
                    if (organizationData.Vehicles[vehicleNumber].rank != id) continue;
                    var vehicle = VehicleData.LocalData.Repository.GetVehicleToNumber (VehicleAccess.Organization, vehicleNumber);
                    var vehicleLocalData = vehicle.GetVehicleLocalData();
                    if (vehicleLocalData == null) continue;
                    
                    if (vehicleLocalData.Access == VehicleAccess.Organization && vehicleLocalData.Fraction == organizationData.Id && vehicle.NumberPlate == vehicleNumber)
                        vehicleLocalData.MinRank = 0;
                    
                    organizationData.Vehicles[vehicleNumber].rank = 0;
                }
                
                var orgId = organizationData.Id;
                
                foreach (var foreachMemberOrganizationData in Manager.AllMembers[orgId].ToList())
                {
                    if (foreachMemberOrganizationData.Rank != id) 
                        continue;
                    
                    Organizations.Player.Repository.SetRank(orgId, foreachMemberOrganizationData.UUID, 0, false);
                }
                
                var ranksName = NeptuneEvo.Table.Repository.GetRanksData(organizationData.Ranks);
                Trigger.ClientEvent(player, "client.org.main.setRanks", JsonConvert.SerializeObject(ranksName));
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
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                if (DateTime.Now < sessionData.TimingsData.NextGlobalChat)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Block10Min), 4500);
                    return;
                }
                
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
            
                if (!organizationData.IsLeader(player.GetUUID()))
                    return;
                
                if (!organizationData.Ranks.ContainsKey(id)) 
                    return;
                
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

                if (organizationData.Ranks.Values.Any(p => p.Name.ToLower() == name.ToLower()))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ExistNameRank), 4500);
                    return;
                }

                organizationData.Ranks[id].Name = name;
                organizationData.SaveRank();

                var ranksName = NeptuneEvo.Table.Repository.GetRanksData(organizationData.Ranks);
                Trigger.ClientEvent(player, "client.org.main.setRanks", JsonConvert.SerializeObject(ranksName));
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
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
            
                if (!organizationData.IsLeader(player.GetUUID()))
                    return;
                
                if (!organizationData.Ranks.ContainsKey(id)) 
                    return;

                if (score > NeptuneEvo.Table.Repository.MaxScore || NeptuneEvo.Table.Repository.MinScore > score)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Минимальное значение: {NeptuneEvo.Table.Repository.MinScore}, а максимальное: {NeptuneEvo.Table.Repository.MaxScore}", 5500);
                    return;
                }
                
                organizationData.Ranks[id].MaxScore = score;
                //organizationData.SaveRank();

                var ranksName = NeptuneEvo.Table.Repository.GetRanksData(organizationData.Ranks);
                Trigger.ClientEvent(player, "client.org.main.setRanks", JsonConvert.SerializeObject(ranksName));
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
                if (!player.IsOrganizationAccess(RankToAccess.UnInvite)) return;
                
                var target = Main.GetPlayerByUUID(uuid);
                if (target != null)
                    Manager.UnInviteFromOrganization(player, target);
                else
                {
                    var memberOrganizationData = player.GetOrganizationMemberData();
                    if (memberOrganizationData == null) 
                        return;

                    var orgId = memberOrganizationData.Id;
                    
                    var targetMemberOrganizationData = Organizations.Manager.GetOrganizationMemberData(uuid, memberOrganizationData.Id);
                    if (targetMemberOrganizationData == null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрока не найдено в Вашей семье", 3000);
                        return;
                    }
                    
                    if (targetMemberOrganizationData.Rank >= memberOrganizationData.Rank)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantUvolit), 3000);
                        return;
                    }
                    
                    Organizations.Player.Repository.RemoveOrganizationMemberData(orgId, uuid);
                    
                    Logs.Repository.AddLogs(player, OrganizationLogsType.UnInvite, $"Выгнал {targetMemberOrganizationData.Name} ({targetMemberOrganizationData.UUID})");
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TiUvolil, targetMemberOrganizationData.Name), 3000);
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
                if (!player.IsOrganizationAccess(RankToAccess.SetRank)) return;
                
                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberOrganizationData == null) 
                    return;

                var orgId = memberOrganizationData.Id;
                
                var target = Main.GetPlayerByUUID(uuid);
                if (target != null)
                    Manager.SetFracRank(player, target, rank);
                else
                    Manager.SetFracRankOffline(player, uuid, rank);
                
                Player.Repository.UpdateMember(player, orgId, uuid);
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
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
            
                if (!organizationData.IsLeader(player.GetUUID()))
                    return;

                var orgId = organizationData.Id;
                
                var memberOrganizationData = Manager.GetOrganizationMemberData(uuid, organizationData.Id);
                if (memberOrganizationData == null)
                    return;

                var access = memberOrganizationData.Access;
                var locks = memberOrganizationData.Lock;
                
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
                    Organizations.Player.Repository.UpdateAccess(orgId, uuid, access, locks);
                    Player.Repository.UpdateMember(player, orgId, uuid);
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
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;

                var orgId = organizationData.Id;
                
                if (!organizationData.IsLeader(player.GetUUID())) 
                    return;

                var ranks = Manager.OrganizationDefaultRanks.Clone();

                if (organizationData.Stock)
                {
                    organizationData.Ranks[1].Access.Add(RankToAccess.OpenStock);
                    organizationData.Ranks[2].Access.Add(RankToAccess.OpenStock);
                }
                
                if (organizationData.CrimeOptions)
                {
                    ranks[1].Access.Add(RankToAccess.OrgCrime);
                    ranks[1].Access.Add(RankToAccess.InCar);
                    ranks[1].Access.Add(RankToAccess.Pull);
                    ranks[1].Access.Add(RankToAccess.Cuff);
                    ranks[1].Access.Add(RankToAccess.FamilyZone);
                    ranks[1].Access.Add(RankToAccess.IsWar);
                    //
                    ranks[2].Access.Add(RankToAccess.OrgCrime);
                    ranks[2].Access.Add(RankToAccess.InCar);
                    ranks[2].Access.Add(RankToAccess.Pull);
                    ranks[2].Access.Add(RankToAccess.Cuff);
                    ranks[2].Access.Add(RankToAccess.FamilyZone);
                    ranks[2].Access.Add(RankToAccess.IsWar);
                }

                organizationData.Ranks = ranks;

                Vehicle.Repository.DefaultRanks(player);
                
                foreach (var foreachMemberOrganizationData in Manager.AllMembers[orgId].ToList())
                {
                    if (foreachMemberOrganizationData.UUID == organizationData.OwnerUUID)
                        Organizations.Player.Repository.SetRank(orgId, foreachMemberOrganizationData.UUID, 2, false);
                    else
                        Organizations.Player.Repository.SetRank(orgId, foreachMemberOrganizationData.UUID, 0, false);
                }
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucResetDefaultRanks), 3000);
                
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.Organizations
                            .Where(v => v.Organization == orgId)
                            .Set(v => v.Ranks, JsonConvert.SerializeObject(organizationData.Ranks))
                            .UpdateAsync();

                        await db.Orgranks
                            .Where(v => v.Id == orgId)
                            .Set(v => v.Rank, (sbyte)0)
                            .UpdateAsync();
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы сбросили настройки рангов", 5000);
                var ranksName = NeptuneEvo.Table.Repository.GetRanksData(organizationData.Ranks);
                Trigger.ClientEvent(player, "client.org.main.setRanks", JsonConvert.SerializeObject(ranksName));
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public static void SetLeader(ExtPlayer player, int uuid)
        {
            try
            {
                var accountData = player.GetAccountData();
                if (accountData == null) 
                    return;
                
                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberOrganizationData == null) 
                    return;
                
                var organizationData = Manager.GetOrganizationData(memberOrganizationData.Id);
                if (organizationData == null) 
                    return;

                var orgId = organizationData.Id;
                
                if (!organizationData.IsLeader(player.GetUUID())) 
                    return;
                
                if (accountData.RedBucks < Main.PricesSettings.UpdateOrganizationLeader)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У Вас не хватает RedBucks для смены лидера.", 3000);
                    return;
                }
                UpdateData.RedBucks(player, -Main.PricesSettings.UpdateOrganizationLeader, msg:"Смена лидера");
                GameLog.Money($"player({player.GetUUID()})", $"server", Main.PricesSettings.UpdateOrganizationLeader, $"NewLeader({organizationData.Id})");
                
                var targetMemberOrganizationData = Manager.GetOrganizationMemberData(uuid, orgId);
                if (targetMemberOrganizationData == null)
                    return;

                var leaderRank = memberOrganizationData.Rank; 
                var rank = targetMemberOrganizationData.Rank; 
                
                Organizations.Player.Repository.SetRank(organizationData.Id, memberOrganizationData.UUID, rank);
                Organizations.Player.Repository.SetRank(organizationData.Id, targetMemberOrganizationData.UUID, leaderRank);
                
                
                organizationData.OwnerUUID = uuid;
                organizationData.SaveLeader();
                
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы передали лидерство организации игроку {targetMemberOrganizationData.Name}", 5000);
                
                //Logs.Repository.AddLogs(player, OrganizationLogsType.None, "");
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
    }
}