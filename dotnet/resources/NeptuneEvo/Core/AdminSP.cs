using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Functions;
using Redage.SDK;
using System;

namespace NeptuneEvo.Core
{
    class AdminSP : Script
    {
        private static readonly nLog Log = new nLog("Core.AdminSP");

        [RemoteEvent("SpectateSelect")]
        public static void SpectatePrevNext(ExtPlayer player, bool state)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Sp)) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                int target = sessionData.AdminData.SPPlayer;
                if (target != -1)
                {
                    int id;
                    if (!state)
                    {
                        id = (target - 1);
                        if (id == player.Value) id--;
                    }
                    else
                    {
                        id = (target + 1);
                        if (id == player.Value) id++;
                    }
                    Spectate(player, id);
                }
            }
            catch (Exception e)
            {
                Log.Write($"SpectatePrevNext Exception: {e.ToString()}");
            }
        }
        
        public static void Spectate(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Sp)) return;
                ExtPlayer target = Main.GetPlayerByID(id);
                if (!target.IsCharacterData())
                {
                    Trigger.SendChatMessage(player, "Игрок под ID " + id + " отсутствует.");
                    return;
                }
                if (target == player) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) 
                    return;

                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) 
                    return;

                if (!targetSessionData.AdminData.SpMode)
                {
                    if (targetCharacterData.AdminLVL >= 6)
                    {
                        var targetAdminConfig = targetCharacterData.ConfigData.AdminOption;
                        if (targetAdminConfig.HideMe && characterData.AdminLVL < targetCharacterData.AdminLVL)
                        {
                            Trigger.SendChatMessage(player, "Игрок под ID " + id + " отсутствует.");
                            Notify.Send(target, NotifyType.Alert, NotifyPosition.BottomCenter, $"{player.Name} ({player.Value}) попытался следить за Вами (/sp)", 3000);
                            return;
                        }
                    }
                    var adminData = sessionData.AdminData;
                    if (!adminData.SpMode)
                    {
                        adminData.SPPosition = player.Position;
                        adminData.SPDimension = UpdateData.GetPlayerDimension(player);
                        adminData.SpInvise = BasicSync.GetInvisible(player);
                    }
                    else Trigger.ClientEvent(player, "spmode", null, false);
                    player.Transparency = 0;
                    player.Position = new Vector3(target.Position.X, target.Position.Y, (target.Position.Z - 10));
                    Trigger.Dimension(player, UpdateData.GetPlayerDimension(target));
                    player.SetSharedData("INVISIBLE", true);
                    Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) начал слежение {target.Name} ({target.Value})");
                    adminData.SpMode = true;
                    adminData.SPPlayer = target.Value;
                    Trigger.ClientEvent(player, "spmode", target, true);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы наблюдаете за {target.Name} [ID: {target.Value}].", 1000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Spectate Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("UnSpectate")]
        public static void RemoteUnSpectate(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Sp)) return;
                UnSpectate(player);
            }
            catch (Exception e)
            {
                Log.Write($"RemoteUnSpectate Exception: {e.ToString()}");
            }
        }
        
        public static void UnSpectate(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var adminData = sessionData.AdminData;
                if (adminData.SpMode)
                {
                    Trigger.ClientEvent(player, "spmode", null, false);
                    NAPI.Task.Run(() =>
                    {
                        try
                        {
                            var characterData = player.GetCharacterData();
                            if (characterData == null) return;
                            player.Position = adminData.SPPosition; // Сначала возвращаем игрока на исходное местоположение, а только потом восстанавливаем прозрачность
                            Trigger.Dimension(player, adminData.SPDimension);
                            player.Transparency = (adminData.SpInvise) ? 0 : 255;
                            player.SetSharedData("INVISIBLE", adminData.SpInvise); // Включаем видимость ника и отключаем отображение хп всех игроков рядом
                            adminData.SPPlayer = -1;
                            adminData.SpMode = false;
                            Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) вышел из режима слежения");
                            Trigger.SendChatMessage(player, "Вы вышли из режима наблюдателя.");
                        }
                        catch (Exception e)
                        {
                            Log.Write($"UnSpectate Task Exception: {e.ToString()}");
                        }
                    }, 400);
                }
            }
            catch (Exception e)
            {
                Log.Write($"UnSpectate Exception: {e.ToString()}");
            }
        }
    }
}
