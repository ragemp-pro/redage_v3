using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using GTANetworkAPI;
using LinqToDB;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Database.Models;
using NeptuneEvo.Handles;
using NeptuneEvo.Organizations.Models;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.Table.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Organizations.Table.Player
{
    public class Repository
    {
        public static void MainLoad(ExtPlayer player, bool isLoadLogs = true)
        {
            var memberOrganizationData = player.GetOrganizationMemberData();
            if (memberOrganizationData == null) 
                return;
            
            var organizationData = Organizations.Manager.GetOrganizationData(memberOrganizationData.Id);
            if (organizationData == null) 
                return;
            
            var orgId = organizationData.Id;

            var membersOnlineCount = 0;
            var membersOfflineCount = 0;
            var membersCount = 0;
            
            var ranksMembersCount = new Dictionary<int, int>();
            var leaderData = NeptuneEvo.Table.Repository.GetMember(null);
            foreach (var foreachMemberOrganizationData in Manager.AllMembers[orgId].ToList())
            {
                if (!ranksMembersCount.ContainsKey(foreachMemberOrganizationData.Rank))
                    ranksMembersCount[foreachMemberOrganizationData.Rank] = 0;

                ranksMembersCount[foreachMemberOrganizationData.Rank]++;
                
                membersCount++;
                if (foreachMemberOrganizationData.PlayerId != -1)
                    membersOnlineCount++;
                else
                    membersOfflineCount++;
                
                if (foreachMemberOrganizationData.UUID != organizationData.OwnerUUID) 
                    continue;

                leaderData = NeptuneEvo.Table.Repository.GetMember(foreachMemberOrganizationData);
            }

            var membersOnlineStats = new int[]
            {
                membersOnlineCount,
                membersOfflineCount,
                membersCount
            };

            var warCount = Organizations.FamilyZones.Repository.FamilyZones.Values
                .Count(f => f.OrganizationId == organizationData.Id);
            
            var stats = new int[]
            {
                organizationData.Drugs,
                organizationData.Materials,
                Manager.MaxMats,
                organizationData.MedKits,
                organizationData.Money,
                warCount,
                -1,
                -1
            };

            var isUpgrate = Upgrate.Repository.Get(player, organizationData).Count > 0;
            var isLog = player.IsOrganizationAccess(RankToAccess.Logs, false);
            
            var info = new object[]
            {
                organizationData.OwnerUUID == player.GetUUID(),//0
                organizationData.Id,//1
                organizationData.Name,//2
                organizationData.Discord,//3
                organizationData.Date,//4
                organizationData.Slogan,//5
                organizationData.Salary,//6
                new [] {organizationData.Color.Red, organizationData.Color.Green, organizationData.Color.Blue},//7
                null,//8
                organizationData.IsOpenStock,//9
                player.IsOrganizationAccess(RankToAccess.OpenStock, false),//10
                isUpgrate,//11
                isLog,//12
                player.IsOrganizationAccess(RankToAccess.SetRank, false),//13
                player.IsOrganizationAccess(RankToAccess.SetVehicleRank, false),//14
                player.IsOrganizationAccess(RankToAccess.Invite, false),//15
                player.IsOrganizationAccess(RankToAccess.UnInvite, false),//16
                player.IsOrganizationAccess(RankToAccess.TableWall, false),//17
                player.IsOrganizationAccess(RankToAccess.EditAllTabletWall, false),//18
                player.IsOrganizationAccess(RankToAccess.CreateDepartment, false),//19
                player.IsOrganizationAccess(RankToAccess.DeleteDepartment, false),//20
                player.IsOrganizationAccess(RankToAccess.Reprimand, false),//21
                false,//22 - ClothesEdit
                false,//23
                false,//24
                player.IsOrganizationAccess(RankToAccess.FamilyZone, false),//25
                organizationData.CrimeOptions
            };
            
            var board = NeptuneEvo.Table.Repository.GetBoard(organizationData.BoardsList, 1);

            var ranksName = NeptuneEvo.Table.Repository.GetRanksData(organizationData.Ranks, ranksMembersCount);

            if (isLog && isLoadLogs)
                Logs.Repository.GetLogs(player, memberOrganizationData.UUID);
            
            var defaultAccess = NeptuneEvo.Table.Repository.GetAccess(organizationData.DefaultAccess);

            
            var departments = new Dictionary<int, DepartmentData>();
            foreach (var department in organizationData.Departments)
            {
                if (player.IsOrganizationDepartmentAccess(department.Key, 1))
                {
                    departments.Add(department.Key, department.Value);
                }
            }

            var departmentsTag = NeptuneEvo.Table.Repository.GetDepartmentsTag(departments);

            Trigger.ClientEvent(player, "client.org.main.mainInit", 
                JsonConvert.SerializeObject(leaderData), 
                JsonConvert.SerializeObject(membersOnlineStats), 
                JsonConvert.SerializeObject(stats), 
                JsonConvert.SerializeObject(info), 
                JsonConvert.SerializeObject(board), 
                JsonConvert.SerializeObject(ranksName), 
                JsonConvert.SerializeObject(defaultAccess), 
                JsonConvert.SerializeObject(departmentsTag));
        }

        public static void GetMembers(ExtPlayer player)
        {

            var memberOrganizationData = player.GetOrganizationMemberData();
            if (memberOrganizationData == null) 
                return;

            var organizationData = Organizations.Manager.GetOrganizationData(memberOrganizationData.Id);
            if (organizationData == null) 
                return;
            
            var orgId = organizationData.Id;
            
            var playersList = new List<List<object>>();
            var playerData = new List<object>();
            foreach (var foreachMemberOrganizationData in Manager.AllMembers[orgId].ToList())
            {
                if (foreachMemberOrganizationData.UUID == memberOrganizationData.UUID)
                    playerData = NeptuneEvo.Table.Repository.GetMember(foreachMemberOrganizationData);
                //else
                playersList.Add(NeptuneEvo.Table.Repository.GetMember(foreachMemberOrganizationData));
            }

            Trigger.ClientEvent(player, "client.org.main.members", JsonConvert.SerializeObject(playerData), JsonConvert.SerializeObject(playersList));
        }

        public static void UpdateMember(ExtPlayer player, int orgId, int uuid)
        {
            var memberOrganizationData = Manager.GetOrganizationMemberData(uuid, orgId);
            if (memberOrganizationData == null)
                return;
            
            var playerData =  NeptuneEvo.Table.Repository.GetMember(memberOrganizationData);
            
            Trigger.ClientEvent(player, "client.org.main.updateMember", JsonConvert.SerializeObject(playerData));
        }
        
        public static void InvitePlayer(ExtPlayer player, string name)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                int id;
                if (int.TryParse(name, out id))
                {
                    ExtPlayer target = Main.GetPlayerByID(id);
                    if (!target.IsCharacterData())
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                        return;
                    }
                    Manager.InviteToOrganization(player, target);
                }
                else
                {
                    ExtPlayer target = (ExtPlayer) NAPI.Player.GetPlayerFromName(name);
                    if (!target.IsCharacterData())
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindMan), 3000);
                        return;
                    }
                    Manager.InviteToOrganization(player, target);
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public static void AddPlayerScore(ExtPlayer player, int uuid, int value)
        {
            try
            {
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
            
                if (organizationData.OwnerUUID != player.GetUUID())
                    return;
                
                var orgId = organizationData.Id;
                
                var memberOrganizationData = Manager.GetOrganizationMemberData(uuid, organizationData.Id);
                if (memberOrganizationData == null)
                    return;

                memberOrganizationData.Score += value;
                
                UpdateMember(player, orgId, uuid);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static void Reprimand(ExtPlayer player, int uuid, string name, string text)
        {
            try
            {
                if (!player.IsOrganizationAccess(RankToAccess.Reprimand)) return;
                if (text.Length > 100)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxVigovorLength), 4500);
                    return;
                }

                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberOrganizationData == null) 
                    return;
                
                var targetMemberOrganizationData = Manager.GetOrganizationMemberData(uuid, memberOrganizationData.Id);
                if (targetMemberOrganizationData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerFraction, name), 3000);
                    return;
                }
                if (targetMemberOrganizationData.Rank >= memberOrganizationData.Rank)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantVigovor, name), 3000);
                    return;
                };
                Logs.Repository.AddLogs(player, OrganizationLogsType.Reprimand, LangFunc.GetText(LangType.Ru, DataName.GivenVigovor, name, uuid, text));
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        public static void Avatar(ExtPlayer player, string url)
        {
            try
            {
                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberOrganizationData == null) 
                    return;

                Organizations.Player.Repository.SetAvatar(memberOrganizationData.Id, memberOrganizationData.UUID, url);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        
        public static void Leave(ExtPlayer player)
        {
            try
            {
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
                    
                if (organizationData.IsLeader(player.GetUUID()))
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OwnerCantLeave), 5000);
                    return;
                }

                player.RemoveOrganizationMemberData();

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouLeftFamily), 6000);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static void Disolver(ExtPlayer player)
        {
            try
            {
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
                
                if (!organizationData.IsLeader(player.GetUUID()))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не являетесь владельцем семьи.", 3000);
                    return;
                }
                
                if (organizationData.Vehicles.Count >= 1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Чтобы распустить семью Вы должны продать все транспортные средства семьи", 3000);
                    return;
                }

                var orgId = organizationData.Id;
                
                var price = Convert.ToInt32(Main.PricesSettings.CreateOrgPrice / 2);
                if (organizationData.OfficeUpgrade == 1 || organizationData.OfficeUpgrade == 2) 
                    price += Convert.ToInt32(Main.PricesSettings.FirstOrgPrice / 2);

                if (organizationData.Stock) 
                    price += Convert.ToInt32(Main.PricesSettings.StockPrice / 2);
                
                MoneySystem.Wallet.Change(player, price);

                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.Organizations
                            .Where(o => o.Organization == orgId)
                            .Set(o => o.BlipID, -1)
                            .Set(o => o.Status, (sbyte) 0)
                            .UpdateAsync();
                    
                        await db.Orgranks
                            .Where(o => o.Id == orgId)
                            .DeleteAsync();
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
                
                if (organizationData.StockLabel != null && organizationData.StockLabel.Exists) 
                    organizationData.StockLabel.Delete();
                organizationData.StockLabel = null;
                
                if(organizationData.Blip != null && organizationData.Blip.Exists) 
                    organizationData.Blip.Delete();
                organizationData.Blip = null;
                
                organizationData.BlipId = -1;
                organizationData.Status = false;
                
                foreach (var foreachPlayer in Character.Repository.GetPlayers())
                {
                    var foreachMemberOrganizationData = foreachPlayer.GetOrganizationMemberData();
                    if (foreachMemberOrganizationData == null) 
                        continue;
                    
                    if (orgId != foreachMemberOrganizationData.Id)
                        continue;
                    
                    NeptuneEvo.Organizations.Player.Repository.RemoveOrganizationMemberData(orgId, foreachPlayer.GetUUID());

                    if (foreachPlayer != player) 
                        Notify.Send(foreachPlayer, NotifyType.Warning, NotifyPosition.BottomCenter, $"Вас выгнали из семьи {organizationData.Name}", 3000);
                }
                
                if (Manager.AllMembers.ContainsKey(orgId))
                    Manager.AllMembers[orgId].Clear();
                
                //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы распустили семью, деньги были выданы на руки.", 3000);
                EventSys.SendCoolMsg(player,"Семья", "Семья распущена!", $"Вы распустили семью, деньги были выданы на руки.", "", 10000);
                //GameLog.Money($"server", $"player({characterData.UUID})", price, $"UnactiveOrg({organizationData.Id})");
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
    }
}