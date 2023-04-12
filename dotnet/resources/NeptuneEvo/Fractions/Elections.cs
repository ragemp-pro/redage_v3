using System;
using System.Collections.Generic;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using MySqlConnector;
using NeptuneEvo.Core;
using Redage.SDK;
using NeptuneEvo.GUI;
using System.Data;
using System.Linq;
using Database;
using LinqToDB;
using Localization;
using NeptuneEvo.Functions;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Players.Popup.List.Models;

namespace NeptuneEvo.Fractions
{
    class ElectionsSystem : Script
    {
        private static readonly nLog Log = new nLog("Fractions.Elections");

        private class ElectionPoints
        { // Точки колшэйпа, через которые производятся выборы
            public int ID { get; set; }
            public uint Election { get; set; }
            public Vector3 Position { get; set; }
            public uint Dimension { get; set; }
            public bool Opened { get; set; }
            public ExtTextLabel Info { get; set; }
            public ExtColShape Point { get; set; } = null;
        }
        private static ElectionPoints ElectionPoint;

        private class Elections
        {
            public int ID { get; set; }
            public uint Election { get; set; } // Уникальный ID Выборов, у разных кандидатов одних выборов - одинаковый election
            public string Name { get; set; }
            public ushort Votes { get; set; }
        }
        private static List<Elections> CandidateList;

        private class Voter
        {
            public uint Election { get; set; }
            public string Login { get; set; }
            public string VotedFor { get; set; }
        }
        private static List<Voter> Voters;
        
        public static void OnResourceStart()
        {
            LoadElections();
        }
        [Interaction(ColShapeEnums.ElectionPoint)]
        public static void OnElectionPoint(ExtPlayer player)
        {
            try
            {
                var accountData = player.GetAccountData();
                if (accountData == null) 
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                if (characterData.LVL < Main.ServerSettings.MinVoteLvl)
                {
                    Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.MinLvlVote, Main.ServerSettings.MinVoteLvl));
                    Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MinLvlVote, Main.ServerSettings.MinVoteLvl), 3000);
                    return;
                }
                if (CandidateList.Count >= 2)
                {
                    if (!CheckPlayerVoted(accountData.Login, ElectionPoint.Election)) OpenElectionMenu(player);
                    else Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.AlreadyVoted));
                }
                else Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.NoCandidates));
            }
            catch (Exception e)
            {
                Log.Write($"Interaction Exception: {e.ToString()}");
            }
        }

        public static void OpenElectionMenu(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                
                var frameList = new FrameListData(); 
                frameList.Header = LangFunc.GetText(LangType.Ru, DataName.Elections); 
                frameList.Callback = callback_electionsmenu; 

                for (byte e = 0; e != CandidateList.Count; e++)
                {
                    frameList.List.Add(new ListData(CandidateList[e].Name, e));
                }

                Players.Popup.List.Repository.Open(player, frameList);  
            }
            catch (Exception e)
            {
                Log.Write($"OpenElectionMenu Exception: {e.ToString()}");
            }
        }

        private static void callback_electionsmenu(ExtPlayer player, object listItem)
        {
            try
            {
                if (!(listItem is int))
                    return;
                if (!player.IsCharacterData()) return;
                var elecid = Convert.ToInt32(listItem);
                if (CandidateList.Count <= elecid) return;
                var name = CandidateList[elecid].Name;
                if (player.Position.DistanceTo(ElectionPoint.Position) <= 2)
                {
                    if (ElectionPoint.Opened)
                    {
                        var accountData = player.GetAccountData();
                        if (accountData == null) return;
                        if (!CheckPlayerVoted(accountData.Login, ElectionPoint.Election))
                        {
                            if (CandidateList[elecid].Name.Equals(name))
                            {
                                Voters.Add(new Voter
                                {
                                    Election = CandidateList[elecid].Election,
                                    Login = accountData.Login,
                                    VotedFor = name
                                });
                                CandidateList[elecid].Votes++;
                                
                                Trigger.SetTask(async () =>
                                {
                                    try
                                    {
                                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                                        await db.ECandidates
                                            .Where(v => v.Name == name)
                                            .Set(v => v.Votes, CandidateList[elecid].Votes)
                                            .UpdateAsync();

                                        await db.InsertAsync(new EVoters
                                        {
                                            Election = CandidateList[elecid].Election,
                                            Login = accountData.Login,
                                            TimeVoted = DateTime.Now,
                                            VotedFor = name
                                        });
                                    }
                                    catch (Exception e)
                                    {
                                        Debugs.Repository.Exception(e);
                                    }
                                });
                                
                                Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.MadeVote, ElectionPoint.Election, name));
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MadeVote, ElectionPoint.Election, name), 3000);
                            }
                        }
                    }
                }
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TooFar), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"callback_electionsmenu Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Elections)] // Перезагрузка с базы
        public static void ElectionReload(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Elections)) return;
                Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Reloading), 3000);
                if (ElectionPoint.Opened)
                {
                    CustomColShape.DeleteColShape(ElectionPoint.Point);
                    ElectionPoint.Point = null;
                    if (ElectionPoint.Info != null && ElectionPoint.Info.Exists) 
                        ElectionPoint.Info.Delete();
                    ElectionPoint.Info = null;
                }
                LoadElections();
            }
            catch (Exception e)
            {
                Log.Write($"ElectionReload Exception: {e.ToString()}");
            }
        }

        private static void LoadElections()
        {
            try
            {
                CandidateList = new List<Elections>();
                ElectionPoint = null;
                Voters = new List<Voter>();
                using MySqlCommand cmdCandidates = new MySqlCommand()
                {
                    CommandText = "SELECT * FROM `e_candidates`"
                };
                using DataTable resultCandidates = MySQL.QueryRead(cmdCandidates);
                if (resultCandidates != null && resultCandidates.Rows.Count != 0)
                {
                    foreach (DataRow row in resultCandidates.Rows)
                    {
                        CandidateList.Add(new Elections
                        {
                            ID = (int)row[0],
                            Election = Convert.ToUInt32(row[1]),
                            Name = (string)row[2],
                            Votes = (ushort)Convert.ToUInt32(row[3])
                        });
                    }
                }
                using MySqlCommand cmdPoints = new MySqlCommand()
                {
                    CommandText = "SELECT * FROM `e_points`"
                };
                using DataTable resultPoints = MySQL.QueryRead(cmdPoints);
                if (resultPoints != null && resultPoints.Rows.Count != 0)
                {
                    DataRow row = resultPoints.Rows[0];
                    ElectionPoint = new ElectionPoints
                    {
                        ID = (int)row[0],
                        Election = Convert.ToUInt32(row[1]),
                        Position = new Vector3((float)row[2], (float)row[3], (float)row[4]),
                        Dimension = Convert.ToUInt32(row[5]),
                        Opened = Convert.ToBoolean(row[6])
                    };
                    if (ElectionPoint.Opened)
                    {
                        ElectionPoint.Point = CustomColShape.CreateSphereColShape(ElectionPoint.Position, 3f, 0, ColShapeEnums.ElectionPoint);
                        ElectionPoint.Info = (ExtTextLabel) NAPI.TextLabel.CreateTextLabel($"Точка голосования\nВыборов №{ElectionPoint.Election}\n~w~Нажмите\n~r~'Взаимодействие'", new Vector3(ElectionPoint.Position.X, ElectionPoint.Position.Y, ElectionPoint.Position.Z + 1), 10f, 1f, 0, new Color(255, 255, 255), dimension: 0);
                    }
                }
                using MySqlCommand cmd = new MySqlCommand()
                {
                    CommandText = "SELECT * FROM `e_voters`"
                };
                using DataTable result = MySQL.QueryRead(cmd);
                if (result != null && result.Rows.Count != 0)
                {
                    foreach (DataRow row in result.Rows)
                    {
                        Voters.Add(new Voter
                        {
                            Election = Convert.ToUInt32((uint)row[1]),
                            Login = (string)row[2],
                            VotedFor = (string)row[4]
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"LoadElections Exception: {e.ToString()}");
            }
        }

        private static bool CheckPlayerVoted(string login, uint election)
        {
            try
            {
                foreach (Voter v in Voters)
                {
                    if (v.Election == election && v.Login == login) return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Write($"CheckPlayerVoted Exception: {e.ToString()}");
                return true;
            }
        }
    }
}
