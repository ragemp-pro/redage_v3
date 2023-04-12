using System;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Handles;
using NeptuneEvo.Players.Animations.Models;
using NeptuneEvo.Players.Phone.Call.Models;
using Redage.SDK;

namespace NeptuneEvo.Players.Phone.Call
{
    public class Repository
    {
        public static void OnCall(ExtPlayer player, int number)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            if (characterData.Sim == -1)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSimcard), 3000);
                Trigger.ClientEvent(player, "client.phone.callError");
                return;
            }
            
            var phoneData = player.getPhoneData();
            if (phoneData == null || phoneData.Call != null) 
                return;
            
            var time = DateTime.Now;
            
            if (characterData.Unmute > 0)
            {
                Recents.Repository.Add(player, number, true, time, 0);
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouMutedMins, characterData.Unmute / 60), 3000);
                Trigger.ClientEvent(player, "client.phone.callError");
                return;
            }

            if (Main.IHaveDemorgan(player, true))
            {
                Recents.Repository.Add(player, number, true, time, 0);
                Trigger.ClientEvent(player, "client.phone.callError");
                return;
            }

            if (Main.SimCards.ContainsKey(number))
            {
                var callData = new CallData();

                var targetUuid = Main.SimCards[number];
                var target = Main.GetPlayerByUUID(targetUuid);
            
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData != null)
                {
                    if (characterData.Sim == targetCharacterData.Sim)
                    {
                        Recents.Repository.Add(player, number, true, time, 0);
                        Trigger.ClientEvent(player, "client.phone.callError");
                        return;
                    }

                    var targetPhoneData = target.getPhoneData();
                    if (targetPhoneData != null)
                    {
                        if (!targetPhoneData.BlackList.Contains(characterData.Sim) && !Settings.Repository.IsAir(targetPhoneData.Settings))
                        {
                            if (targetPhoneData.Call != null)//Линия занята
                            {
                                Recents.Repository.Add(player, number, true, time, 0);
                                Trigger.ClientEvent(player, "client.phone.callError");
                                return;
                            }
                            
                            Trigger.ClientEvent(target, "client.phone.bell", characterData.Sim);

                            targetPhoneData.Call = new CallData
                            {
                                IsCall = false,
                                Number = characterData.Sim,
                                Type = CallType.Call,
                                Target = player
                            };
                    
                            callData.Target = target;
                            
                            Settings.Repository.PlayCall(target);
                            Recents.Repository.Add(target, characterData.Sim, false, time);
                        }
                    }
                }

                callData.IsCall = true;
                callData.Number = number;
                callData.Type = CallType.Calling;
            
                phoneData.Call = callData;
            
                WeaponRepository.RemoveHands(player);

                Animations.Repository.PlayScenario(player, "cphone_call");

                Trigger.ClientEvent(player, "client.phone.callStart");
                
                Recents.Repository.Add(player, number, true, time);
            }
            
        }

        public static void OnTake(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            if (characterData.Sim == -1)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSimcard), 3000);
                Trigger.ClientEvent(player, "client.phone.callError");
                return;
            }
            
            var phoneData = player.getPhoneData();
            if (phoneData == null || phoneData.Call == null) 
                return;

            var callData = phoneData.Call;
            
            if (callData.Type != CallType.Call || callData.IsCall) 
                return;

            var target = callData.Target;
            
            var targetCharacterData = target.GetCharacterData();
            if (targetCharacterData == null)
            {
                Trigger.ClientEvent(player, "client.phone.callError");
                return;
            }
            
            var targetPhoneData = target.getPhoneData();
            if (targetPhoneData == null)
            {
                Trigger.ClientEvent(player, "client.phone.callError");
                return;
            }
            
            var targetCallData = targetPhoneData.Call;
            if (targetCallData == null || targetCallData.Target != player)
            {
                Trigger.ClientEvent(player, "client.phone.callError");
                return;
            }
            
            if (callData.Type == CallType.Call)
                Settings.Repository.StopCall(player);
            
            if (targetCallData.Type == CallType.Call)
                Settings.Repository.StopCall(target);
            
            callData.Type = CallType.Talk;
            targetCallData.Type = CallType.Talk;
            
            TalkPhone(player, target);
            TalkPhone(target, player);
            
            WeaponRepository.RemoveHands(player);

            Animations.Repository.PlayScenario(player, "cphone_call");
        }
        
        public static void TalkPhone(ExtPlayer player, ExtPlayer target)
        {
            if (!player.IsCharacterData()) return;
            if (!target.IsCharacterData()) return;
            player.SetSharedData("PhoneTalk", target.Value + 1);
            player.EnableVoiceTo(target);
        }

        public static void OnPut(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            var phoneData = player.getPhoneData();
            if (phoneData == null || phoneData.Call == null) 
                return;
            
            var callData = phoneData.Call;
            
            Clear(callData.Target, true);
            Clear(player);
            
            var isTalk = callData.Type == CallType.Talk;
            var time = DateTime.Now;
            
            if (isTalk)
            {
                Recents.Repository.UpdateConfirm(callData.Target, time);
                Recents.Repository.UpdateConfirm(player, time);
            }
            else
            {
                Recents.Repository.UpdateEnd(callData.Target);
                Recents.Repository.UpdateEnd(player);
            }
        }
        
        public static void Clear(ExtPlayer player, bool isClear = false)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            var phoneData = player.getPhoneData();
            if (phoneData == null || phoneData.Call == null) 
                return;

            if (isClear)
                Trigger.ClientEvent(player, "client.phone.callError");
            
            var callData = phoneData.Call;

            if (callData.Type == CallType.Talk)
            {
                player.SetSharedData("PhoneTalk", 0);
                player.DisableVoiceTo(callData.Target);

            }
            if (callData.Type == CallType.Talk || callData.Type == CallType.Calling) 
                Animations.Repository.PlayScenario(player, "cphone_base");
            
            if (callData.Type == CallType.Call)
                Settings.Repository.StopCall(player);
            
            
            phoneData.Call = null;
            
            //Animations.Repository.Stop(player, AnimationList.PhoneCall);
            
        }
    }
}