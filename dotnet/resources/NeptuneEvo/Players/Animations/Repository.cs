using System;
using System.Collections.Generic;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Handles;
using NeptuneEvo.Players.Animations.Models;

namespace NeptuneEvo.Players.Animations
{
    public class Repository
    {
        private static Dictionary<AnimationList, AnimationData> Animations = new Dictionary<AnimationList, AnimationData>
        {
            { AnimationList.PhoneOpen, new AnimationData
                {
                    AnimDict = "cellphone@str",
                    AnimName = "cellphone_text_press_a",
                    Flag = 49,
                    StopAnimDict = "cellphone@",
                    StopAnimName = "cellphone_text_out",
                    StopFlag = 48,
                    Attachment = Attachments.AttachmentsName.PhoneCall
                }
            },
            { AnimationList.PhoneCall, new AnimationData
                {
                    AnimDict = "anim@cellphone@in_car@ds",
                    AnimName = "cellphone_call_listen_base",
                    Flag = 49,
                    Attachment = Attachments.AttachmentsName.PhoneCall
                }
            },
        };

        public static void Play(ExtPlayer player, AnimationList animationList)
        {
            if (!Animations.ContainsKey(animationList))
                return;

            var anim = Animations[animationList];
            
            Main.OnAntiAnim(player);
            Trigger.PlayAnimation(player, anim.AnimDict, anim.AnimName, anim.Flag);

            if (anim.Attachment > 0) 
                Attachments.AddAttachment(player, anim.Attachment);
        }

        public static void Stop(ExtPlayer player, AnimationList animationList)
        {
            if (!Animations.ContainsKey(animationList))
                return;

            var anim = Animations[animationList];
            
            Main.OffAntiAnim(player);

            if (!player.IsInVehicle)
            {
                Trigger.StopAnimation(player, force: anim.StopAnimDict == String.Empty);
                
                if (anim.StopAnimDict != String.Empty)
                    player.PlayAnimation(anim.StopAnimDict, anim.StopAnimName, anim.StopFlag);
            }

            if (anim.Attachment > 0) 
                Attachments.RemoveAttachment(player, anim.Attachment);
        }
        
        //
        
        public static void PlayScenario(ExtPlayer player, string name)
        {
            Trigger.SetMainTask(() =>
            {
                if (player.IsCharacterData())
                {
                    player.SetSharedData("cSen", name);
                }
            });
        }
        public static void StopScenario(ExtPlayer player)
        {
            Trigger.SetMainTask(() =>
            {
                if (player.IsCharacterData())
                {
                    player.ResetSharedData("cSen");
                }
            });
        }
    }
}