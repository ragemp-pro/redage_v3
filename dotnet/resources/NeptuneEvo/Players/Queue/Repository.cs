using GTANetworkAPI;
using NeptuneEvo.Handles;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Localization;

namespace NeptuneEvo.Players.Queue
{
    class Repository
    {
        public static List<ExtPlayer> List = new List<ExtPlayer>();
        private static readonly nLog Log = new nLog("Core.Queue");

        public static void AddLogin(ExtPlayer player)
        {
            if (Main.ServerSettings.MaxGameSlots > Main.PlayersOnLogin.Count && !Main.PlayersOnLogin.Contains(player))
                Main.PlayersOnLogin.Add(player);
        }
        
        public static bool AddQueue(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return true;
            
            if (List.Contains(player)) 
                return true;
            
            if (Main.PlayersOnLogin.Contains(player)) 
                return false;
            
            if (Main.ServerSettings.MaxGameSlots > Main.PlayersOnLogin.Count /* ||
                    Main.AdminSocials.Contains(sessionData.RealSocialClub) ||
                    Main.MediaSocials.Contains(sessionData.RealSocialClub)*/)
                return false;
            
            Log.Write($"{sessionData.Name} ({sessionData.SocialClubName} | {sessionData.RealSocialClub}) added to player queue.");
            List.Add(player);
            UpdateList();
            return true;
        }
        
        public static void Start()
        {
            var thread = new Thread(QueueWorker);
            thread.IsBackground = true;
            thread.Name = "Queue";
            thread.Start();
        }

        private static async void QueueWorker()
        {
            while (true)
            {
                if (List.Count > 0)
                {
                    await CheckQueue();
                }
                await Task.Delay(250);
            }
        }

        private static async Task CheckQueue()
        {
            try
            {
                if (List.Count >= 1)
                {
                    if (Main.ServerSettings.MaxGameSlots > Main.PlayersOnLogin.Count)
                    {
                        var player = List[0];
                        var playersCount = List.Count;
                        if (playersCount >= 1)
                        {
                            List.RemoveAt(0);
                            UpdateList();
                        }
                        AddLogin(player);
                        await Connect.Repository.PlayerToAuntidication(player);
                        Thread.Sleep(1000 * 3);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"CheckQueue Exception: {e.ToString()}");
            }
        }

        private static void UpdateList()
        {
            
            var players = List.ToList();
            var playersCount = List.Count;
            foreach (var target in players)
            {
                if (target == null) continue;
                Trigger.ClientEvent(target, "queue.text", false, LangFunc.GetText(LangType.Ru, DataName.YouQueued, players.IndexOf(target) + 1, playersCount));
            }
        }
    }
}
