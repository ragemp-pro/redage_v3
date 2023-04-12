using MySqlConnector;
using NeptuneEvo.Fractions;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NeptuneEvo.Core
{
    public class GameLog
    {
        private static readonly nLog Log = new nLog("Core.GameLog");
        private static List<List<string>> CmdQueue = new List<List<string>>();
        public static Dictionary<int, DateTime> OnlineQueue = new Dictionary<int, DateTime>();
        public static void Stock(int Frac, int Uuid, string Name, string Type, int Amount, string In)
        {
            var cmd = new List<string>();
            cmd.Add("CALL `addLogsData`(@var0, @var1, @var2)");
            cmd.Add("`stocklog`");
            cmd.Add("`time`,`frac`,`uuid`,`name`,`type`,`amount`,`in`");
            cmd.Add($"'{DateTime.Now.ToString("s")}',{Frac},{Uuid},'{Name}','{Type}',{Amount},'{In}'");
            CmdQueue.Add(cmd);
        }
        public static void Admin(string Admin, string Action, string Player)
        {
            if (string.IsNullOrEmpty(Player)) Player = "null";
            Action = Main.BlockSymbols(Action);
            var cmd = new List<string>();
            cmd.Add("CALL `addLogsData`(@var0, @var1, @var2)");
            cmd.Add("`adminlog`");
            cmd.Add("`time`,`admin`,`action`,`player`");
            cmd.Add($"'{DateTime.Now.ToString("s")}','{Admin}','{Action}','{Player}'");
            CmdQueue.Add(cmd);
        }

        public static void Uniques(int count, int maxplayers)
        {
            var cmd = new List<string>();
            cmd.Add("CALL `addLogsData`(@var0, @var1, @var2)");
            cmd.Add("`unique`");
            cmd.Add("`time`,`count`,`maxplayers`");
            cmd.Add($"'{DateTime.Now.ToString("s")}',{count},{maxplayers}");
            CmdQueue.Add(cmd);
        }
        public static void Money(string From, string To, long Amount, string Comment)
        {
            var cmd = new List<string>();
            cmd.Add("CALL `addLogsData`(@var0, @var1, @var2)");
            cmd.Add("`moneylog`");
            cmd.Add("`time`,`from`,`to`,`amount`,`comment`");
            cmd.Add($"'{DateTime.Now.ToString("s")}','{From}','{To}',{Amount.ToString()},'{Comment}'");
            CmdQueue.Add(cmd);
        }
        public static void Kills(string Killer, string Weapon, string Victim, string Pos)
        {
            var cmd = new List<string>();
            cmd.Add("CALL `addLogsData`(@var0, @var1, @var2)");
            cmd.Add("`killlog`");
            cmd.Add("`time`,`killer`,`weapon`,`victim`,`pos`");
            cmd.Add($"'{DateTime.Now.ToString("s")}','{Killer}','{Weapon}','{Victim}','{Pos}'");
            CmdQueue.Add(cmd);
        }
        public static void ClientTryCatch(string path, string callback, string message)
        {
            message = Main.BlockSymbols(message);
            var cmd = new List<string>();
            cmd.Add("CALL `addLogsData`(@var0, @var1, @var2)");
            cmd.Add("`client_tc`");
            cmd.Add("`time`,`path`,`callback`,`message`");
            cmd.Add($"'{DateTime.Now.ToString("s")}','{path}','{callback}','{message}'");
            CmdQueue.Add(cmd);
        }
        public static void FracLog(int Frac, int Me, int He, string Me1, string He1, string Action)
        {
            Action = Main.BlockSymbols(Action);
            var cmd = new List<string>();
            cmd.Add("CALL `addLogsData`(@var0, @var1, @var2)");
            cmd.Add("`fraclog`");
            cmd.Add("`time`,`frac`,`player`,`target`,`pname`,`tname`,`action`");
            cmd.Add($"'{DateTime.Now.ToString("s")}','{Manager.FractionNames[Frac]}',{Me},{He},'{Me1}','{He1}','{Action}'");
            CmdQueue.Add(cmd);
        }
        public static void Items(string From, string To, int Type, int Amount, string Data)
        {
            var cmd = new List<string>();
            cmd.Add("CALL `addLogsData`(@var0, @var1, @var2)");
            cmd.Add("`itemslog`");
            cmd.Add("`time`,`from`,`to`,`type`,`amount`,`data`");
            cmd.Add($"'{DateTime.Now.ToString("s")}','{From}','{To}',{Type},{Amount},'{Data}'");
            CmdQueue.Add(cmd);
        }
        public static void Name(int Uuid, string Old, string New)
        {
            var cmd = new List<string>();
            cmd.Add("CALL `addLogsData`(@var0, @var1, @var2)");
            cmd.Add("`namelog`");
            cmd.Add("`time`,`uuid`,`old`,`new`");
            cmd.Add($"'{DateTime.Now.ToString("s")}',{Uuid},'{Old}','{New}'");
            CmdQueue.Add(cmd);
        }
        public static void Ban(int Admin, int Player, string Login, DateTime Until, string Reason, bool isHard)
        {
            Reason = Main.BlockSymbols(Reason);
            var cmd = new List<string>();
            cmd.Add("CALL `addLogsData`(@var0, @var1, @var2)");
            cmd.Add("`banlog`");
            cmd.Add("`time`,`admin`,`player`,`login`,`until`,`reason`,`ishard`");
            cmd.Add($"'{DateTime.Now.ToString("s")}',{Admin},{Player},'{Login}','{Until.ToString("s")}','{Reason}',{isHard}");
            CmdQueue.Add(cmd);
        }
        public static void Ticket(int player, int target, int sum, string reason, string pnick, string tnick)
        {
            reason = Main.BlockSymbols(reason);

            var cmd = new List<string>();
            cmd.Add("CALL `addLogsData`(@var0, @var1, @var2)");
            cmd.Add("`ticketlog`");
            cmd.Add("`time`,`player`,`target`,`sum`,`reason`,`pnick`,`tnick`");
            cmd.Add($"'{DateTime.Now.ToString("s")}',{player},{target},{sum},'{reason}','{pnick}','{tnick}'");
            CmdQueue.Add(cmd);
        }
        public static void Arrest(int player, int target, string reason, int stars, string pnick, string tnick)
        {
            reason = Main.BlockSymbols(reason);
            var cmd = new List<string>();
            cmd.Add("CALL `addLogsData`(@var0, @var1, @var2)");
            cmd.Add("`arrestlog`");
            cmd.Add("`time`,`player`,`target`,`reason`,`stars`,`pnick`,`tnick`");
            cmd.Add($"'{DateTime.Now.ToString("s")}',{player},{target},'{reason}',{stars},'{pnick}','{tnick}'");
            CmdQueue.Add(cmd);
        }
        public static void Connected(string Name, int Uuid, string SClub, string Hwid, int Id, string ip, string login)
        {
            if (OnlineQueue.ContainsKey(Uuid)) return;
            var now = DateTime.Now;
            OnlineQueue[Uuid] = now;
            var cmd = new List<string>();
            cmd.Add("CALL `addLogsData`(@var0, @var1, @var2)");
            cmd.Add("`idlog`");
            cmd.Add("`in`,`out`,`uuid`,`id`,`name`,`sclub`,`hwid`,`ip`,`login`");
            cmd.Add($"'{now.ToString("s")}',null,'{Uuid}','{Id}','{Name}','{SClub}','{Hwid}','{ip}','{login}'");
            CmdQueue.Add(cmd);
        }
        public static void Disconnected(int Uuid, int Id, string Reason, string login)
        {
            if (!OnlineQueue.ContainsKey(Uuid)) return;
            var conn = OnlineQueue[Uuid];
            OnlineQueue.Remove(Uuid);
            var cmd = new List<string>();
            cmd.Add("CALL `updLogsData`(@var0, @var1, @var2)");
            cmd.Add("`idlog`");
            cmd.Add($"`out`='{DateTime.Now.ToString("s")}',`reason`='{Reason}'");
            cmd.Add($"`in`='{conn.ToString("s")}' AND `uuid`={Uuid} AND `id`={Id}");
            CmdQueue.Add(cmd);
            
            var ticks = DateTime.Now.Ticks - conn.Ticks;
            if (ticks > 0)
            {
                var time = TimeSpan.FromTicks(ticks).Minutes;
                if (time > 0)
                    Utils.Analytics.HelperThread.AddEvent("session_time", Accounts.Repository.GetEmailToLogin(login), "", time);
            }
                    
        }
        public static void UpdateName(string Name, int Uuid, int Id)
        {
            if (!OnlineQueue.ContainsKey(Uuid)) return;
            DateTime conn = OnlineQueue[Uuid];
            var cmd = new List<string>();
            cmd.Add("CALL `updLogsData`(@var0, @var1, @var2)");
            cmd.Add("`idlog`");
            cmd.Add($"`name`='{Name}'");
            if (Id >= 0) cmd.Add($"`in`='{conn.ToString("s")}' AND `uuid`={Uuid} AND `id`={Id}");
            else cmd.Add($"`in`='{conn.ToString("s")}' AND `uuid`={Uuid}");
            CmdQueue.Add(cmd);
        }
        public static void DisconnectAll()
        {
            var cmd = new List<string>();
            cmd.Add("CALL `updLogsData`(@var0, @var1, @var2)");
            cmd.Add("`idlog`");
            cmd.Add($"`out`='{DateTime.Now.ToString("s")}',`reason`='Restart'");
            cmd.Add($"`out` IS NULL");
            CmdQueue.Add(cmd);
        }
        public static void AddInfo(string action)
        {
            action = Main.BlockSymbols(action);
            var cmd = new List<string>();
            cmd.Add("CALL `addLogsData`(@var0, @var1, @var2)");
            cmd.Add("`addinfo`");
            cmd.Add("`time`,`action`");
            cmd.Add($"'{DateTime.Now.ToString("s")}','{action}'");
            CmdQueue.Add(cmd);
        }
        public static void CharacterDelete(string name, int uuid, string account, int bank)
        {
            var cmd = new List<string>();
            cmd.Add("CALL `addLogsData`(@var0, @var1, @var2)");
            cmd.Add("`deletelog`");
            cmd.Add("`time`,`uuid`,`name`,`account`,`bank`");
            cmd.Add($"'{DateTime.Now.ToString("s")}',{uuid},'{name}','{account}',{bank}");
            CmdQueue.Add(cmd);
        }
        public static void AccountLog(string account, string hwid, string ip, string sclub, string action)
        {
            action = Main.BlockSymbols(action);
            var cmd = new List<string>();
            cmd.Add("CALL `addLogsData`(@var0, @var1, @var2)");
            cmd.Add("`acclog`");
            cmd.Add("`time`,`login`,`hwid`,`ip`,`sclub`,`action`");
            cmd.Add($"'{DateTime.Now.ToString("s")}','{account}','{hwid}','{ip}','{sclub}','{action}'");
            CmdQueue.Add(cmd);
        }
        public static void EventLogAdd(string AdmName, string EventName, ushort MembersLimit, string Started)
        {
            var cmd = new List<string>();
            cmd.Add("CALL `addLogsData`(@var0, @var1, @var2)");
            cmd.Add("`eventslog`");
            cmd.Add("`AdminStarted`,`EventName`,`MembersLimit`,`Started`");
            cmd.Add($"'{AdmName}','{EventName}','{MembersLimit}','{Started}'");
            CmdQueue.Add(cmd);
        }
        public static void EventLogUpdate(string AdmName, int MembCount, string WinName, uint Reward, string Time, int RewardLimit, ushort MemLimit, string EvName)
        {
            EvName = Main.BlockSymbols(EvName);
            var cmd = new List<string>();
            cmd.Add("CALL `updLogsData`(@var0, @var1, @var2)");
            cmd.Add("`eventslog`");
            cmd.Add($"`AdminClosed`='{AdmName}',`Members`={MembCount},`Winner`='{WinName}',`Reward`={Reward},`Ended`='{Time}',`RewardLimit`={RewardLimit}");
            cmd.Add($"`Winner`='Undefined' AND `MembersLimit`={MemLimit} AND `EventName`='{EvName}'");
            CmdQueue.Add(cmd);
        }
        public static void CasinoRouletteLog(long total)
        {
            MySQL.LogsQuery($"update casinolog set `roulette`=`roulette`+{total}");
        }
        public static void CasinoHorsesLog(long total)
        {
            MySQL.LogsQuery($"update casinolog set `horses`=`horses`+{total}");
        }
        public static void CasinoSpinsLog(long total)
        {
            MySQL.LogsQuery($"update casinolog set `spins`=`spins`+{total}");
        }
        public static void CasinoBJLog(long total)
        {
            MySQL.LogsQuery($"update casinolog set `bj`=`bj`+{total}");
        }
        public static void AddRam(string playerName, string name, long ram)
        {
            var cmd = new List<string>();
            cmd.Add("CALL `addLogsData`(@var0, @var1, @var2)");
            cmd.Add("`ram`");
            cmd.Add("`time`,`player`,`name`,`ram`");
            cmd.Add($"'{DateTime.Now.ToString("s")}','{playerName}','{name}','{ram}'");
            CmdQueue.Add(cmd);
        }
        #region Логика потока

        public static async Task Save()
        {          
            try
            {
                var listCmd = CmdQueue.ToList();
                CmdQueue.Clear();
                foreach (var cmdData in listCmd)
                {
                    using MySqlCommand cmd = new MySqlCommand()
                    {
                        CommandText = cmdData[0]
                    };
                    cmd.Parameters.AddWithValue("@var0", cmdData[1]);
                    cmd.Parameters.AddWithValue("@var1", cmdData[2]);
                    cmd.Parameters.AddWithValue("@var2", cmdData[3]);
                    await MySQL.LogsQueryAsync(cmd);
                }
            }
            catch (Exception e)
            {
                Log.Write($"LogsWorker Exception: {e.ToString()}");
            }
        }
        #endregion
    }
}