using GTANetworkAPI;
using NeptuneEvo.Handles;
using System;
using NeptuneEvo.Core;
using Redage.SDK;
using NeptuneEvo.Chars;
using NeptuneEvo.Functions;
using System.Linq;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using System.Collections.Generic;
using Localization;
using NeptuneEvo.Fractions;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Table.Tasks.Models;
using NeptuneEvo.Table.Tasks.Player;
using Newtonsoft.Json;

namespace NeptuneEvo.Voice
{
    public class Voice : Script
    {
        private static nLog Log = new nLog("Voice");

        public static int DangerButtonChecker { get; set; } = -1;

        public static void PlayerQuit(ExtPlayer player, string reson)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                Selecting.UnMuteAllList(player);

                VoiceData playerPhoneMeta = sessionData.VoiceData;

                if (playerPhoneMeta.Target != null)
                {
                    ExtPlayer target = playerPhoneMeta.Target;
                    var targetSessionData = target.GetSessionData();
                    if (targetSessionData == null) return;

                    var targetCharacterData = target.GetCharacterData();
                    if (targetCharacterData == null) return;


                    VoiceData targetPhoneMeta = targetSessionData.VoiceData;

                    int pSim = characterData.Sim;
                    string playerName = (targetCharacterData.Contacts.ContainsKey(pSim)) ? targetCharacterData.Contacts[pSim] : pSim.ToString();

                    Notify.Send(target, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerHangPhone, playerName), 3000);
                    targetPhoneMeta.Target = null;
                    targetPhoneMeta.CallingState = "nothing";
                    targetSessionData.AntiAnimDown = false;

                    if (!target.IsInVehicle) Trigger.StopAnimation(target);
                    else targetSessionData.ToResetAnimPhone = true;

                    Attachments.RemoveAttachment(target, Attachments.AttachmentsName.PhoneCall);

                    Trigger.ClientEvent(target, "voice.phoneStop");
                }
            }
            catch (Exception e)
            {
                Log.Write($"PlayerQuit Exception: {e.ToString()}");
            }
        }
        
        [Command("v_reload")]
        public void voiceDebugReload(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.Vreload));
                Trigger.ClientEvent(player, "v_reload");
                Selecting.UnMuteAllList(player);
            }
            catch (Exception e)
            {
                Log.Write($"voiceDebugReload Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("add_voice_listener")]
        public static void add_voice_listener(ExtPlayer player, ExtPlayer target)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!target.IsCharacterData()) return;
                if (sessionData.LoggedIn)
                {
                    player.EnableVoiceTo(target);
                    /*if (!player.OutgoingSyncDisabled)
                        player.EnableVoiceTo(target);
                    else
                        target.EnableVoiceTo(player);*/
                }
            }
            catch (Exception e)
            {
                Log.Write($"add_voice_listener Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("add_voice_listener_sync")]
        public static void add_voice_listener_sync(ExtPlayer player, ExtPlayer target)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!target.IsCharacterData()) return;
                if (sessionData.LoggedIn)
                {
                    target.EnableVoiceTo(player);
                }
            }
            catch (Exception e)
            {
                Log.Write($"add_voice_listener Exception: {e.ToString()}");
            }
        }
        
        [RemoteEvent("remove_voice_listener_sync")]
        public static void remove_voice_listener_sync(ExtPlayer player, ExtPlayer target)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (!target.IsCharacterData()) return;

                target.DisableVoiceTo(player);
            }
            catch (Exception e)
            {
                Log.Write($"remove_voice_listener Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("remove_voice_listener")]
        public static void remove_voice_listener(ExtPlayer player, ExtPlayer target)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (!target.IsCharacterData()) return;
                player.DisableVoiceTo(target);
            }
            catch (Exception e)
            {
                Log.Write($"remove_voice_listener Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("set_polygon")]
        public void set_polygon(ExtPlayer player, int idPolygon)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                player.SetSharedData("VoiceZone", idPolygon);
            }
            catch (Exception e)
            {
                Log.Write($"set_polygon Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("set_whisper")]
        public void set_whisper(ExtPlayer player, bool toggled)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                player.SetSharedData("isWhisper", toggled);
            }
            catch (Exception e)
            {
                Log.Write($"set_whisper Exception: {e.ToString()}");
            }
        }

        public static bool isGov(int fracId)
        {
            return (fracId == (int)Fractions.Models.Fractions.CITY || fracId == (int)Fractions.Models.Fractions.POLICE || fracId == (int)Fractions.Models.Fractions.EMS || fracId == (int)Fractions.Models.Fractions.FIB || fracId == (int)Fractions.Models.Fractions.ARMY || fracId == (int)Fractions.Models.Fractions.MERRYWEATHER || fracId == (int)Fractions.Models.Fractions.SHERIFF || fracId == (int)Fractions.Models.Fractions.LSNEWS);
        }
        [RemoteEvent("TakeWalkieTalkie_server")]
        public static void TakeWalkieTalkie_server(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var memberFractionData = player.GetFractionMemberData();
                
                int access = 0;

                if (memberFractionData != null && isGov(memberFractionData.Id)) access = 1;
                else if (sessionData.WalkieTalkieFrequency != -99) access = 2;

                if (access > 0) Trigger.ClientEvent(player, "openWalkieTalkieMenu", access == 1, sessionData.WalkieTalkieFrequency);
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoWalkieTalkie), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"TakeWalkieTalkie_server Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("PressedDangerButton")]
        public static void PressedDangerButton(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;

                if (memberFractionData.Id == 7 || memberFractionData.Id == 9 || memberFractionData.Id == 18)
                {
                    if (DangerButtonChecker != -1)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Panic15sec), 3000);
                        return;
                    }

                    DangerButtonChecker = player.Value;
                    sessionData.IsPressedDangerButton = true;

                    foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                    {
                        var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                        if (foreachMemberFractionData == null) continue;

                        if (foreachMemberFractionData.Id == (int)Fractions.Models.Fractions.POLICE || foreachMemberFractionData.Id == (int)Fractions.Models.Fractions.FIB || foreachMemberFractionData.Id == (int)Fractions.Models.Fractions.SHERIFF)
                        {
                            Trigger.ClientEvent(foreachPlayer, "createWaypoint", player.Position.X, player.Position.Y);
                            Trigger.ClientEvent(foreachPlayer, "StartDangerButtonSound_client", "sounds/panic.mp3");
                            Trigger.SendChatMessage(foreachPlayer, "!{#F08080}[F] " + LangFunc.GetText(LangType.Ru, DataName.PanicActivator, player.Name, player.Value));
                        }
                        else if (foreachPlayer.Position.DistanceTo(player.Position) < 3)
                        {
                            Sounds.Play2d(foreachPlayer, "sounds/panic.mp3",  2.3f / 100);
                        }
                    }

                    NAPI.Task.Run(() =>
                    {
                        try
                        {
                            DangerButtonChecker = -1;
                        }
                        catch (Exception e)
                        {
                            Log.Write($"PressedDangerButton Task Exception: {e.ToString()}");
                        }
                    }, 15000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"PressedDangerButton Exception: {e.ToString()}");
            }
        }
        [Command("to")]
        public static void CMD_to(ExtPlayer player, int sender_id)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                if (memberFractionData.Id == 7 || memberFractionData.Id == 9 || memberFractionData.Id == 18)
                {
                    if (!sessionData.WorkData.OnDuty && Fractions.Manager.FractionTypes[memberFractionData.Id] == FractionsType.Gov)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WorkDayNotStarted), 3000);
                        return;
                    }
                    
                    ExtPlayer target = Main.GetPlayerByID(sender_id);
                    var targetSessionData = target.GetSessionData();
                    if (targetSessionData == null) return;
                    if (targetSessionData.IsPressedDangerButton != true) return;

                    Trigger.ClientEvent(player, "createWaypoint", target.Position.X, target.Position.Y);
                    
                    if (sessionData.AcceptedDangerCall == sender_id) return;
                    sessionData.AcceptedDangerCall = sender_id;

                    foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                    {
                        var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                        if (foreachMemberFractionData == null) continue;

                        if (foreachMemberFractionData.Id == (int)Fractions.Models.Fractions.POLICE || foreachMemberFractionData.Id == (int)Fractions.Models.Fractions.FIB || foreachMemberFractionData.Id == (int)Fractions.Models.Fractions.SHERIFF)
                            Trigger.SendChatMessage(foreachPlayer, "!{#F08080}[F] " + LangFunc.GetText(LangType.Ru, DataName.Code0accept, player.Name, player.Value, sender_id));
                        player.AddTableScore(TableTaskId.Item5);
                        player.AddTableScore(TableTaskId.Item6);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_to Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("UpdateWalkieTalkieFrequency")]
        public static void UpdateWalkieTalkieFrequency(ExtPlayer player, int value)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                Trigger.ClientEvent(player, "LeaveRadio");
                sessionData.WalkieTalkieFrequency = value;
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WalkieWave, value), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"UpdateWalkieTalkieFrequency Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("addRadio")]
        public static void AddRadio(ExtPlayer player)
        {
            try
            {
                if (!FunctionsAccess.IsWorking("radio"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                var memberFractionData = player.GetFractionMemberData();

                int fracId = memberFractionData != null ? memberFractionData.Id : 0;
                int walkieTalkieFrequency = sessionData.WalkieTalkieFrequency;
                bool _isGov = isGov(fracId);
                
                if ((!_isGov && walkieTalkieFrequency == -1) || walkieTalkieFrequency == -99)
                    return;

                Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, true, "radio");
                Main.OnAntiAnim(player);

                var removeVoicePlayer = new List<int>();

                foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                {
                    var foreachSessionData = foreachPlayer.GetSessionData();
                    if (foreachSessionData == null) 
                        continue;
                    
                    if (foreachPlayer == player) 
                        continue;
                    
                    var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();

                    if (walkieTalkieFrequency > -1 && foreachSessionData.WalkieTalkieFrequency == walkieTalkieFrequency)
                    {
                        Trigger.ClientEvent(foreachPlayer, "AddRadio", player.Value);
                        player.EnableVoiceTo(foreachPlayer);
                        removeVoicePlayer.Add(foreachPlayer.Value);
                    }
                    else if (_isGov && fracId > (int)Fractions.Models.Fractions.None && fracId == foreachMemberFractionData?.Id)
                    {
                        if (foreachSessionData.WalkieTalkieFrequency != walkieTalkieFrequency) continue;
                        Trigger.ClientEvent(foreachPlayer, "AddRadio", player.Value);
                        player.EnableVoiceTo(foreachPlayer);
                        removeVoicePlayer.Add(foreachPlayer.Value);
                    }
                }
                Trigger.ClientEvent(player, "StartRadio", JsonConvert.SerializeObject(removeVoicePlayer));
                BattlePass.Repository.UpdateReward(player, 74);
            }
            catch (Exception e)
            {
                Log.Write($"addRadio Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("disableRadio")]
        public static void DisableRadio(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!FunctionsAccess.IsWorking("radio"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                var memberFractionData = player.GetFractionMemberData();
                var fracId = memberFractionData != null ? memberFractionData.Id : 0;
                var walkieTalkieFrequency = sessionData.WalkieTalkieFrequency;
                var _isGov =  isGov(fracId);
                
                if ((!_isGov && walkieTalkieFrequency == -1) || walkieTalkieFrequency == -99)
                    return;

                Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "radio");
                Main.OffAntiAnim(player);

                foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                {
                    if (foreachPlayer.Value == player.Value) continue;
                    
                    var foreachSessionData = foreachPlayer.GetSessionData();
                    if (foreachSessionData == null) continue;
                    var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                    
                    
                    if (walkieTalkieFrequency > -1 && foreachSessionData.WalkieTalkieFrequency == walkieTalkieFrequency)
                    {
                        player.EnableVoiceTo(foreachPlayer);
                        Trigger.ClientEvent(foreachPlayer, "RemoveRadio", player.Value);
                    }
                    else if (_isGov && fracId > (int)Fractions.Models.Fractions.None && fracId == foreachMemberFractionData?.Id)
                    {
                        player.DisableVoiceTo(foreachPlayer);
                        Trigger.ClientEvent(foreachPlayer, "RemoveRadio", player.Value);
                    }
                }

            }
            catch (Exception e)
            {
                Log.Write($"addRadio Exception: {e.ToString()}");
            }
        }
        /*[RemoteEvent("addRadio")]
        public static void AddRadio(Player player)
         {
             try
             {
                 if (!player.IsCharacterData()) return;
                 int fracId = memberFractionData.Id;
                 int orgId = Organizations.Manager.Members.ContainsKey(player) ? Organizations.Manager.Members[player].OrganizationID : -1;
                 if (fracId == 0 && orgId == -1)
                     return;
                 //Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "radio");
                 //Main.OnAntiAnim(player);

                 foreach (Player foreachPlayer in Character.Repository.GetPlayers())
                 {
                     if (!Chars.Repository.isOnlineCharacter(p)) continue;
                     else if (p.Value == player.Value) continue;
                     if (orgId != -1 && Organizations.Manager.Members.ContainsKey(p) && Organizations.Manager.Members[p].OrganizationID == orgId)
                     {
                         Trigger.ClientEvent(p, "AddRadio", player.Value);
                         player.EnableVoiceTo(p);
                     }
                     else if (fracId > (int)Fractions.Models.Fractions.None && fracId == targetMemberFractionData.Id)
                     {
                         Trigger.ClientEvent(p, "AddRadio", player.Value);
                         player.EnableVoiceTo(p);
                     }

                 }
             }
             catch (Exception e)
             {
                 Log.Write($"addRadio Exception: {e.ToString()}");
             }
         }
         [RemoteEvent("disableRadio")]
         public static void DisableRadio(Player player)
         {
             try
             {
                 if (!player.IsCharacterData()) return;
                 int fracId = memberFractionData.Id;
                 int orgId = Organizations.Manager.Members.ContainsKey(player) ? Organizations.Manager.Members[player].OrganizationID : -1;
                 if (fracId == 0 && orgId == -1)
                     return;
                 //Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "radio");
                 //Main.OffAntiAnim(player);

                 foreach (Player foreachPlayer in Character.Repository.GetPlayers())
                 {
                     if (!Chars.Repository.isOnlineCharacter(p)) continue;
                     else if (p.Value == player.Value) continue;
                     if (orgId != -1 && Organizations.Manager.Members.ContainsKey(p) && Organizations.Manager.Members[p].OrganizationID == orgId)
                     {
                         player.DisableVoiceTo(p);
                         Trigger.ClientEvent(p, "RemoveRadio", player.Value);
                     }
                     else if (fracId > (int)Fractions.Models.Fractions.None && fracId == targetMemberFractionData.Id)
                     {
                         player.DisableVoiceTo(p);
                         Trigger.ClientEvent(p, "RemoveRadio", player.Value);
                     }

                 }
             }
             catch (Exception e)
             {
                 Log.Write($"addRadio Exception: {e.ToString()}");
             }
         }*/

        public static void OnPlayerDisconnected(ExtPlayer player)
        {
            try
            {
                DisableRadio(player);
            }
            catch (Exception e)
            {
                Log.Write($"Event_PlayerDisconnected Exception: {e.ToString()}");
            }
        }
        /*[RemoteEvent("toggleInput")]
        public void toggleInput(Player player, bool toggled)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                player.SetSharedData("isToggleInput", toggled);
            }
            catch (Exception e)
            {
                Log.Write($"toggleInput Exception: {e.ToString()}");
            }
        }*/

        [ServerEvent(Event.PlayerExitVehicle)]
        public void Event_PlayerExitVehicle(ExtPlayer player, ExtVehicle veh)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (sessionData.ToResetAnimPhone)
                {
                    Trigger.StopAnimation(player);
                    sessionData.ToResetAnimPhone = false;
                }
            }
            catch (Exception e)
            {
                Log.Write($"Event_PlayerExitVehicle Exception: {e.ToString()}");
            }
        }
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            //CustomColShape.CreateCylinderColShape(new Vector3(252.185, -1108.75, 30.082), 6, 3, 0, ColShapeEnums.VoiceZone);
        }

        [Interaction(ColShapeEnums.VoiceZone, In: true)]
        public static void InFractionSheriffArrest(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                SetVoiceZone(player, 35, true);
            }
            catch (Exception e)
            {
                Log.Write($"InFractionSheriffArrest Exception: {e.ToString()}");
            }
        }
        [Interaction(ColShapeEnums.VoiceZone, Out: true)]
        public static void OutFractionSheriffArrest(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                SetVoiceZone(player);
            }
            catch (Exception e)
            {
                Log.Write($"OutFractionSheriffArrest Exception: {e.ToString()}");
            }
        }
        public static void SetVoiceZone(ExtPlayer player, int value = 0, bool addMicroToHand = false)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            if (value == 0)
            {
                player.ResetSharedData("VoiceDist");
                if (Attachments.HasAttachment(player, Attachments.AttachmentsName.News_mic))
                {
                    Attachments.RemoveAttachment(player, Attachments.AttachmentsName.News_mic);
                    if (!player.IsInVehicle) Trigger.StopAnimation(player);
                    Main.OffAntiAnim(player);
                }
            }
            else
            {
                player.SetSharedData("VoiceDist", value);
                if (addMicroToHand)
                {
                    Commands.RPChat("sme", player, $"достал" + (characterData.Gender ? "" : "а") + $" Микрофон");
                    Attachments.AddAttachment(player, Attachments.AttachmentsName.News_mic);
                    if (!player.IsInVehicle) player.PlayAnimation("anim@heists@humane_labs@finale@keycards", "ped_b_enter_loop", 49);
                    // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "microphone");
                }

            }
        }
        [Command(AdminCommands.setmicrophone)]
        public static void CMD_setmicrophone(ExtPlayer player, int id, int value)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.setmicrophone)) return;
                ExtPlayer target = Main.GetPlayerByID(id);
                if (!target.IsCharacterData()) return;

                player.SetSharedData("VoiceDist", value);
            }
            catch (Exception e)
            {
                Log.Write($"setmicrophone Exception: {e.ToString()}");
            }

        }
    }
}