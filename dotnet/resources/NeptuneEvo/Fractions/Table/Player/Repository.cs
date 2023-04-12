using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Handles;
using NeptuneEvo.Table.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Fractions.Table.Player
{
    public class Repository
    {
        public static void MainLoad(ExtPlayer player)
        {
            var memberFractionData = player.GetFractionMemberData();
            if (memberFractionData == null) 
                return;
            
            var fractionData = Fractions.Manager.GetFractionData(memberFractionData.Id);
            if (fractionData == null) 
                return;
            
            
            var fracId = fractionData.Id;
            var leaderRank = fractionData.LeaderRank();

            var membersOnlineCount = 0;
            var membersOfflineCount = 0;
            var membersCount = 0;
            
            var ranksMembersCount = new Dictionary<int, int>();
            var leaderData = NeptuneEvo.Table.Repository.GetMember(null);
            foreach (var foreachMemberFractionData in Manager.AllMembers[fracId].ToList())
            {
                if (!ranksMembersCount.ContainsKey(foreachMemberFractionData.Rank))
                    ranksMembersCount[foreachMemberFractionData.Rank] = 0;

                ranksMembersCount[foreachMemberFractionData.Rank]++;
                
                membersCount++;
                if (foreachMemberFractionData.PlayerId != -1)
                    membersOnlineCount++;
                else
                    membersOfflineCount++;
                
                if (foreachMemberFractionData.Rank != leaderRank) 
                    continue;

                leaderData = NeptuneEvo.Table.Repository.GetMember(foreachMemberFractionData);
            }

            var membersOnlineStats = new int[]
            {
                membersOnlineCount,
                membersOfflineCount,
                membersCount
            };
            
            var gangZonesCount = -1;
            if (Manager.FractionTypes[fractionData.Id] == FractionsType.Gangs)
                gangZonesCount = GangsCapture.GangPoints.Values.Count(g => g.GangOwner == fractionData.Id);
            
            var bizCount = -1;
            if (Manager.FractionTypes[fractionData.Id] == FractionsType.Mafia)
                bizCount = BusinessManager.BizList.Values.Count(g => g.Mafia == fractionData.Id);
            
            var stats = new int[]
            {
                fractionData.Drugs,
                fractionData.Materials,
                fractionData.MaxMats,
                fractionData.MedKits,
                fractionData.Money,
                -1,
                gangZonesCount,
                bizCount
                
            };

            var isLog = player.IsFractionAccess(RankToAccess.Logs, false);

            var info = new object[]
            {
                leaderRank == memberFractionData.Rank,//0
                fractionData.Id,//1
                fractionData.Name,//2
                fractionData.Discord,//3
                null,//4
                null,//5
                null,//6
                null,//7
                null,//8
                fractionData.IsOpenStock,//9
                player.IsFractionAccess(RankToAccess.OpenStock, false),//10
                false,//11
                isLog,//12
                player.IsFractionAccess(RankToAccess.SetRank, false),//13
                player.IsFractionAccess(RankToAccess.SetVehicleRank, false),//14
                player.IsFractionAccess(RankToAccess.Invite, false),//15
                player.IsFractionAccess(RankToAccess.UnInvite, false),//16
                player.IsFractionAccess(RankToAccess.TableWall, false),//17
                player.IsFractionAccess(RankToAccess.EditAllTabletWall, false),//18
                player.IsFractionAccess(RankToAccess.CreateDepartment, false),//19
                player.IsFractionAccess(RankToAccess.DeleteDepartment, false),//20
                player.IsFractionAccess(RankToAccess.Reprimand, false),//21
                player.IsFractionAccess(RankToAccess.ClothesEdit, false),//22
                //
                fractionData.IsOpenGunStock,//23
                player.IsFractionAccess(RankToAccess.OpenGunStock, false),//24
                false,//25
                false
            };
            
            var board = NeptuneEvo.Table.Repository.GetBoard(fractionData.BoardsList, 1);

            var ranksName = NeptuneEvo.Table.Repository.GetRanksData(fractionData.Ranks, ranksMembersCount);

            if (isLog)
                Logs.Repository.GetLogs(player, memberFractionData.UUID);
            
            var defaultAccess = NeptuneEvo.Table.Repository.GetAccess(fractionData.DefaultAccess);

            //
            
            var departments = new Dictionary<int, DepartmentData>();
            foreach (var department in fractionData.Departments)
            {
                if (player.IsFractionDepartmentAccess(department.Key, 1))
                {
                    departments.Add(department.Key, department.Value);
                }
            }

            var departmentsTag = NeptuneEvo.Table.Repository.GetDepartmentsTag(departments);
            
            //
            
            Trigger.ClientEvent(player, "client.frac.main.mainInit", 
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

            var memberFractionData = player.GetFractionMemberData();
            if (memberFractionData == null) 
                return;

            var fractionData = Fractions.Manager.GetFractionData(memberFractionData.Id);
            if (fractionData == null) 
                return;
            
            var fracId = fractionData.Id;
            
            var playersList = new List<List<object>>();
            var playerData = new List<object>();
            foreach (var foreachMemberFractionData in Manager.AllMembers[fracId].ToList())
            {
                if (foreachMemberFractionData.UUID == memberFractionData.UUID)
                    playerData = NeptuneEvo.Table.Repository.GetMember(foreachMemberFractionData);
                //else
                playersList.Add(NeptuneEvo.Table.Repository.GetMember(foreachMemberFractionData));
            }

            Trigger.ClientEvent(player, "client.frac.main.members", JsonConvert.SerializeObject(playerData), JsonConvert.SerializeObject(playersList));
        }

        public static void UpdateMember(ExtPlayer player, int fracId, int uuid)
        {
            var memberFractionData = Manager.GetFractionMemberData(uuid, fracId);
            if (memberFractionData == null)
                return;
            
            var playerData =  NeptuneEvo.Table.Repository.GetMember(memberFractionData);
            
            Trigger.ClientEvent(player, "client.frac.main.updateMember", JsonConvert.SerializeObject(playerData));
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
                    FractionCommands.InviteToFraction(player, target);
                }
                else
                {
                    ExtPlayer target = (ExtPlayer) NAPI.Player.GetPlayerFromName(name);
                    if (!target.IsCharacterData())
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindMan), 3000);
                        return;
                    }
                    FractionCommands.InviteToFraction(player, target);
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

                targetMemberFractionData.Score += value;
                
                UpdateMember(player, fracId, uuid);
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
                if (!player.IsFractionAccess(RankToAccess.Reprimand)) return;
                if (text.Length > 100)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxVigovorLength), 4500);
                    return;
                }

                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null) 
                    return;
                
                var targetMemberFractionData = Manager.GetFractionMemberData(uuid, memberFractionData.Id);
                if (targetMemberFractionData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerFraction, name), 3000);
                    return;
                }
                if (targetMemberFractionData.Rank >= memberFractionData.Rank)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantVigovor, name), 3000);
                    return;
                };
                Logs.Repository.AddLogs(player, FractionLogsType.Reprimand, LangFunc.GetText(LangType.Ru, DataName.GivenVigovor, name, uuid, text));
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
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null) 
                    return;

                Fractions.Player.Repository.SetAvatar(memberFractionData.Id, memberFractionData.UUID, url);
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
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
	    
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null)
                    return;

                    
                if (fractionData.IsLeader(memberFractionData.Rank))
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LeaderCantUval), 5000);
                    return;
                }
                Manager.sendFractionMessage(memberFractionData.Id, "!{#FF8C00}[F] " + $"{player.Name} ({player.Value}) уволился по собственному желанию.", true);
                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.UnInvite, LangFunc.GetText(LangType.Ru, DataName.SelfUval));
                    
                player.RemoveFractionMemberData();
                player.ClearAccessories();
                Customization.ApplyCharacter(player);
                    
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouLeaveFraction), 3000);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

    }
}