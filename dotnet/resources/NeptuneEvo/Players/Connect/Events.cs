using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.Players.Models;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NeptuneEvo.Players.Connect
{
    class Events : Script
    {
        private static readonly nLog Log = new nLog("Players.Connect.Events");
        
        [ServerEvent(Event.PlayerConnected)]
        private void OnPlayerConnected(ExtPlayer player)
        {
            if (player == null)
                return;
            
            if (player.IsSessionData())
            {
                Log.Write($"OnPlayerConnected error, player ({player.Name} | {player.Value} | {player.SocialClubName}) already contains somehow?!");
                player.Kick();
                return;
            }

            
            Trigger.Dimension(player, Dimensions.RequestPrivateDimension(player.Value));
            
            World.Weather.Repository.Init(player);
            
            Trigger.ClientEvent(player, "client.init",
                 Main.ServerSettings.ServerId,
                 Main.ServerSettings.ServerName,
                 Main.DonateSettings.Multiplier,
                 Main.DonateSettings.Convert,
                 Main.ServerSettings.IsMerger);
            
            if (Admin.IsServerStoping)
            {
                Trigger.ClientEvent(player, "restart");
                player.IsRestartSaveAccountData = true;
                player.IsRestartSaveCharacterData = true;
                return;
            }
            if (Queue.Repository.List.Contains(player))
            {
                Trigger.ClientEvent(player, "queue.text", true, "Ошибка входа на сервер #9461, попробуйте перезайти в игру.");
                return;
            }
            Log.Write($"{player.Name} ({player.SocialClubName}) trying to join the server.");
            
            //

            if (Main.PlayerIdToEntity.ContainsKey(player.Value)) 
                Main.PlayerIdToEntity.TryRemove(player.Value, out _);

            Main.PlayerIdToEntity.TryAdd(player.Value, player);

            //

            var sessionData = new SessionData
            {
                Value = player.Value,
                Name = player.Name,
                IsConnect = true,
                SocialClubName = player.SocialClubName,
                RealSocialClub = player.SocialClubId.ToString(),
                RealHWID = player.Serial,
                Address = player.Address
            };

            player.SetSessionData(sessionData);

            Queue.Repository.AddLogin(player);

            Trigger.SetTask(async () =>
            {
                await Repository.OnPlayerConnected(player);
            });
        }
    }
}
