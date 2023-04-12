using System;
using System.Data;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using MySqlConnector;
using Redage.SDK;
using System.Collections.Generic;
using System.Linq;
using Database;
using LinqToDB;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Functions;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;

namespace NeptuneEvo.Core
{
    class ReportSys : Script
    {
        private static readonly nLog Log = new nLog("Core.Report");
        public class Report
        {
            public int ID { get; set; }
            public string Author { get; set; }
            public string Question { get; set; }
            public string Response { get; set; }
            public string BlockedBy { get; set; }            
            public DateTime OpenedDate { get; set; }
            public DateTime ClosedDate { get; set; }

            public bool Status { get; set; }

            public void Send(ExtPlayer someone = null)
            {
                try
                {
                    if (someone == null)
                    {
                        foreach (ExtPlayer foreachPlayer in Main.AllAdminsOnline)
                        {
                            try
                            {
                                var foreachCharacterData = foreachPlayer.GetCharacterData();
                                if (foreachCharacterData == null) continue;
                                if (foreachCharacterData.AdminLVL < Main.ServerSettings.MinAdminLvlReport) continue;
                                ExtPlayer author = (ExtPlayer) NAPI.Player.GetPlayerFromName(Author);
                                var authorCharacterData = author.GetCharacterData();
                                string authorName;
                                if (authorCharacterData == null) authorName = $"{Author}[Вышел]";
                                else authorName = $"{Author}[{author.Value}]";

                                Trigger.ClientEvent(foreachPlayer, "addreport", ID, authorName, Question);
                            }
                            catch (Exception e)
                            {
                                Log.Write($"Send Foreach Exception: {e.ToString()}");
                            }
                        }
                    }
                    else
                    {
                        var someoneCharacterData = someone.GetCharacterData();
                        if (someoneCharacterData == null) return;
                        if (someoneCharacterData.AdminLVL < Main.ServerSettings.MinAdminLvlReport) return;
                        ExtPlayer author = (ExtPlayer) NAPI.Player.GetPlayerFromName(Author);
                        var authorCharacterData = author.GetCharacterData();
                        string authorName;
                        if (authorCharacterData == null) authorName = $"{Author}[Вышел]";
                        else authorName = $"{Author}[{author.Value}]";

                        Trigger.ClientEvent(someone, "addreport", ID, authorName, Question);
                    }
                }
                catch (Exception e)
                {
                    Log.Write($"Send Exception: {e.ToString()}");
                }
            }
        }
        public static Dictionary<int, Report> Reports;

        public static void Init()
        {
            try
            {
                Reports = new Dictionary<int, Report>();

                string cmd = "SELECT * FROM questions;";
                using DataTable result = MySQL.QueryRead(cmd);
                if (result is null || result.Rows.Count == 0) return;
                foreach(DataRow row in result.Rows)
                {
                    if (Convert.ToBoolean((sbyte)row[7]) != false) continue;

                    Reports.Add((int)row[0], new Report
                    {
                        ID = (int)row[0],
                        Author = row[1].ToString(),
                        Question = Main.BlockSymbols(row[2].ToString()),
                        BlockedBy = "",
                        Response = "",
                        OpenedDate = (DateTime)row[5],
                        ClosedDate = DateTime.MinValue,
                        Status = false
                    });
                }

            }
            catch (Exception e)
            {
                Log.Write($"StartWork Exception: {e.ToString()}");
            }
        }
        public static void onAdminLoad(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                foreach (Report report in Reports.Values)
                {
                    try
                    {
                        report.Send(player);
                    }
                    catch (Exception e)
                    {
                        Log.Write($"onAdminLoad Foreach Exception: {e.ToString()}");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"onAdminLoad Exception: {e.ToString()}");
            }
        }

        public static void OnAdminDisconnect(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.AdminLVL == 0) return;
                foreach (Report report in Reports.Values)
                {
                    try
                    {
                        if (report.BlockedBy.Equals(player.Name))
                        {
                            ReportTake(player, report.ID);
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Write($"OnAdminDisconnect Foreach Exception: {e.ToString()}");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"OnAdminDisconnect Exception: {e.ToString()}");
            }
        }

        public static void onAdminUnLoad(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                foreach (Report report in Reports.Values)
                {
                    try
                    {
                        Remove(report.ID, player);
                    }
                    catch (Exception e)
                    {
                        Log.Write($"onAdminUnLoad Foreach Exception: {e.ToString()}");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"onAdminUnLoad Exception: {e.ToString()}");
            }
        }

        #region Remote Events
        //Админ взял репорт на себя
        [RemoteEvent("takereport")]
        public static void ReportTake(ExtPlayer player, int id)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.AdminLVL <= 0) return;
                if (!Reports.ContainsKey(id))
                {
                    Remove(id, player);
                    return;
                }
                Report report = Reports[id];
                if (report.BlockedBy.Length > 1 && !report.BlockedBy.Equals(player.Name)) return;
                else if(report.Status)
                {
                    Remove(id, player);
                    return;
                }
                else if (Reports.Values.Any(a => a.BlockedBy == player.Name && a.ID != id))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы уже взяли один репорт.", 3000);
                    return;
                }

                if (report.BlockedBy.Length > 1 && report.BlockedBy.Equals(player.Name)) report.BlockedBy = "";
                else report.BlockedBy = player.Name;
                foreach (ExtPlayer foreachPlayer in Main.AllAdminsOnline)
                {
                    try
                    {
                        var foreachCharacterData = foreachPlayer.GetCharacterData();
                        if (foreachCharacterData == null) continue;
                        if (foreachCharacterData.AdminLVL < Main.ServerSettings.MinAdminLvlReport) continue;
                        Trigger.ClientEvent(foreachPlayer, "setreport", id, report.BlockedBy);
                    }
                    catch (Exception e)
                    {
                        Log.Write($"ReportTake Foreach Exception: {e.ToString()}");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"ReportTake Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("funcreport")]
        public static void ReportFunc(ExtPlayer player, int ID, string funcName)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.AdminLVL <= 0) return;
                if (!Reports.ContainsKey(ID)) return;
                ExtPlayer target = (ExtPlayer) NAPI.Player.GetPlayerFromName(Reports[ID].Author);
                if (!target.IsCharacterData()) Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Игрок не найден!", 3000);
                else
                {
                    switch (funcName)
                    {
                        case "tp":
                            Commands.CMD_teleport(player, target.Value);
                            return;
                        case "metp":
                            Admin.teleportTargetToPlayer(player, target);
                            return;
                        case "sp":
                            AdminSP.Spectate(player, target.Value);
                            return;
                        case "stats":
                            Commands.CMD_showPlayerStats(player, target.Value);
                            return;
                        case "kill":
                            Admin.killTarget(player, target);
                            return;
                        case "ptime":
                            Commands.CMD_pcheckPrisonTime(player, target.Value);
                            return;
                        case "checkdim":
                            Commands.CMD_checkDim(player, target.Value);
                            return;
                        case "nhistory":
                            Commands.CMD_NickHistory(player, target.Value);
                            return;
                        default:
                            // Not supposed to end up here. 
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"ReportFunc Exception: {e.ToString()}");
            }
        }
        
        [RemoteEvent("sendreport")]
        public static void ReportSend(ExtPlayer player, int ID, string answer)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.AdminLVL <= 0) return;
                if (!Reports.ContainsKey(ID)) return;
                if (!Reports[ID].Status) AddAnswer(player, ID, answer);
                else
                {
                    Trigger.SendChatMessage(player, "Эта жалоба более недоступна для изменения.");
                    Remove(ID, player);
                }
            }
            catch (Exception e)
            {
                Log.Write($"ReportSend Exception: {e.ToString()}");
            }
        }
        #endregion
        
        public static void AddReport(ExtPlayer player, string question, string name)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (question.Length >= 150)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вопрос содержит более 150 символов, задайте вопрос более коротко.", 5000);
                    return;
                }
                question = Main.BlockSymbols(question);
                
                var accountData = player.GetAccountData();
                if (accountData?.VipLvl != 4)
                {
                    sessionData.TimingsData.NextReport = DateTime.Now.AddMinutes(1);
                }

                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Ваш вопрос {Reports.Count + 1} в очереди.", 5000);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Ваш вопрос: {question}", 5000);

                if (Main.AllAdminsOnline.Count <= 2) 
                    Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, "В данный момент администрация может отвечать чуть дольше обычного, извиняемся за возможное ожидание и благодарим за понимание.", 8000);

                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        var id = await db.InsertWithInt32IdentityAsync(new Questions
                        {
                            Author = name,
                            Question = question,
                            Respondent = "",
                            Response = "",
                            Opened = DateTime.Now,
                            Closed = DateTime.MinValue,
                            Status = 0
                        });

                        Trigger.SetMainTask(() =>
                        {
                            try
                            {
                                var report = new Report
                                {
                                    ID = id,
                                    Author = name,
                                    Question = question,
                                    BlockedBy = "",
                                    Response = "",
                                    Status = false,
                                    OpenedDate = DateTime.Now,
                                    ClosedDate = DateTime.MinValue
                                };
                                report.Send();
                                Reports.Add(id, report);

                                if (Reports.Count >= 1)
                                {
                                    foreach (ExtPlayer foreachPlayer in Main.AllAdminsOnline)
                                    {
                                        var foreachCharacterData = foreachPlayer.GetCharacterData();
                                        if (foreachCharacterData == null) continue;
                                        if (foreachCharacterData.AdminLVL == 0) continue;
                                        NAPI.Notification.SendNotificationToPlayer(foreachPlayer, $"В системе ~r~{Reports.Count} ~s~репортов!", true);
                                        //Sounds.Play2d(foreachPlayer, "sounds/icq.mp3",  2.3f / 100);
                                        Trigger.ClientEvent(foreachPlayer, "StartDangerButtonSound_client", "sounds/icq.mp3");
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Debugs.Repository.Exception(e);
                            }
                        });
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
                
            }
            catch (Exception e)
            {
                Log.Write($"AddReport Exception: {e.ToString()}");
            }
        }

        /*
        private static void CalculateTimeReport(Player player, int hrs, int min, int sec)
        {
            string answertime = "~r~Время ответа: ";
            answertime += (hrs == 1 || hrs == 21) ? $"{hrs} час" : ((hrs >= 2 && hrs <= 4) || (hrs >= 22 && hrs <= 24)) ? $"{hrs} часа" : $"{hrs} часов";
            answertime += (min == 1 || min == 21 || min == 31 || min == 41 || min == 51) ? $" {min} минута" : ((min >= 2 && min <= 4) || (min >= 22 && min <= 24) || (min >= 32 && min <= 34) || (min >= 42 && min <= 44) || (min >= 52 && min <= 54)) ? $" {min} минуты" : $" {min} минут";
            answertime += (sec == 1 || sec == 21 || sec == 31 || sec == 41 || sec == 51) ? $" {sec} секунда" : ((sec >= 2 && sec <= 4) || (sec >= 22 && sec <= 24) || (sec >= 32 && sec <= 34) || (sec >= 42 && sec <= 44) || (sec >= 52 && sec <= 54)) ? $" {sec} секунды" : $" {sec} секунд";
            if (!player.IsCharacterData()) return;
            Trigger.SendChatMessage(player, answertime);
        }
        */

        private static void AddAnswer(ExtPlayer player, int repID, string response)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                response = Main.BlockSymbols(response);
                if (characterData.AdminLVL < Main.ServerSettings.MinAdminLvlReport) return;
                if (!Reports.ContainsKey(repID)) return;
                if (response.Length >= 300)
                {
                    ReportTake(player, repID);
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Ответ слишком длинный (более 300 символов).", 3000);
                    return;
                }
                DateTime now = DateTime.Now;
                ExtPlayer target = (ExtPlayer) NAPI.Player.GetPlayerFromName(Reports[repID].Author);
                if (!target.IsCharacterData()) Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Игрок не найден!", 3000);
                else
                {
                    Trigger.SendChatMessage(target, $"~o~Ваш вопрос: {Reports[repID].Question}");
                    Trigger.SendChatMessage(target, $"{ChatColors.Report}Администратор {player.Name} ({player.Value}): {response}");
                    TimeSpan responsetime = Reports[repID].OpenedDate - now;
                    //Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Ответ от {player.Name}: {response}", 7000);
                    EventSys.SendCoolMsg(target,"Ответ на репорт", player.Name, $"{response}", "", 10000);
                    Trigger.ClientEvent(target, "StartDangerButtonSound_client", "sounds/icq.mp3");
                    
                    Trigger.SendToAdmins(Main.ServerSettings.MinAdminLvlReport, $"{ChatColors.Report}[A][{responsetime.ToString("mm\\:ss")}] {player.Name}({player.Value}) для {target.Name}({target.Value}): {response}");
                }

                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.Questions
                            .Where(v => v.ID == repID)
                            .Set(v => v.Respondent, sessionData.Name)
                            .Set(v => v.Response, response)
                            .Set(v => v.Status, (sbyte) 1)
                            .Set(v => v.Closed, now)
                            .UpdateAsync();
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
                
                Remove(repID);
            }
            catch (Exception e)
            {
                Log.Write($"AddAnswer Exception: {e.ToString()}");
            }
        }
        
        private static void Remove(int ID_, ExtPlayer someone = null, bool force = false)
        {
            try
            {
                if (someone == null)
                {
                    foreach (ExtPlayer foreachPlayer in Main.AllAdminsOnline)
                    {
                        try
                        {
                            var foreachCharacterData = foreachPlayer.GetCharacterData();
                            if (foreachCharacterData == null) continue;
                            if (foreachCharacterData.AdminLVL < Main.ServerSettings.MinAdminLvlReport) continue;
                            Trigger.ClientEvent(foreachPlayer, "delreport", ID_);
                        }
                        catch (Exception e)
                        {
                            Log.Write($"Remove Foreach Exception: {e.ToString()}");
                        }
                    }
                    if (Reports.ContainsKey(ID_)) Reports.Remove(ID_);
                }
                else
                {
                    var someoneCharacterData = someone.GetCharacterData();
                    if (someoneCharacterData == null) return;
                    if (someoneCharacterData.AdminLVL < Main.ServerSettings.MinAdminLvlReport) return;
                    Trigger.ClientEvent(someone, "delreport", ID_);
                    if (force && Reports.ContainsKey(ID_)) Reports.Remove(ID_);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Remove Exception: {e.ToString()}");
            }
        }
    }
}
