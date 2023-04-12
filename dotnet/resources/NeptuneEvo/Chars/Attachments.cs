using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Functions;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.IO;
using System.Text;

namespace NeptuneEvo.Chars
{
    class Attachments : Script
    {
        public static class AttachmentsName
        {
            public static uint Vape = NAPI.Util.GetHashKey("vape");
            public static uint Beer = NAPI.Util.GetHashKey("beer");
            public static uint Burger = NAPI.Util.GetHashKey("burger");
            public static uint HotDog = NAPI.Util.GetHashKey("hotdog");
            public static uint Pizza = NAPI.Util.GetHashKey("pizza");
            public static uint Sandwich = NAPI.Util.GetHashKey("sandwich");
            public static uint Crisps = NAPI.Util.GetHashKey("crisps");
            public static uint Joint = NAPI.Util.GetHashKey("joint");
            public static uint eCola = NAPI.Util.GetHashKey("ecola");
            public static uint Sprunk = NAPI.Util.GetHashKey("sprunk");
            public static uint Guitar = NAPI.Util.GetHashKey("guitar");
            public static uint Bongo = NAPI.Util.GetHashKey("bongo");
            public static uint Press1 = NAPI.Util.GetHashKey("press1");
            public static uint Press2 = NAPI.Util.GetHashKey("press2");
            public static uint ElGuitar = NAPI.Util.GetHashKey("elguitar");
            public static uint Cuffs = NAPI.Util.GetHashKey("cuffs");
            public static uint MoneyBag = NAPI.Util.GetHashKey("moneybag");
            public static uint Postal = NAPI.Util.GetHashKey("postalobj");
            public static uint PhoneCall = NAPI.Util.GetHashKey("phonecall");
            public static uint Microphone = NAPI.Util.GetHashKey("microphone");

            public static uint Umbrella = NAPI.Util.GetHashKey("umbrella");
            public static uint Rose = NAPI.Util.GetHashKey("rose");
            public static uint News_camera = NAPI.Util.GetHashKey("news_camera");
            public static uint News_mic = NAPI.Util.GetHashKey("news_mic");
            public static uint Electric_guitar = NAPI.Util.GetHashKey("electric_guitar");
            
            public static uint Binoculars = NAPI.Util.GetHashKey("binoculars");
            public static uint Clipboard = NAPI.Util.GetHashKey("clipboard");
            public static uint Bong = NAPI.Util.GetHashKey("bong");
            public static uint Teddy = NAPI.Util.GetHashKey("teddy");
            public static uint Barbell = NAPI.Util.GetHashKey("barbell");

            public static uint Pickaxe = NAPI.Util.GetHashKey("mine_pickaxe");
            public static uint MineRock = NAPI.Util.GetHashKey("mine_rock");
            public static uint WorkAxeProp = NAPI.Util.GetHashKey("work_axe");

            public static uint Ball = NAPI.Util.GetHashKey("ball");
            public static uint HealthKit = NAPI.Util.GetHashKey("prop_ld_health_pack");
            public static uint VehicleNumber = NAPI.Util.GetHashKey("vehicleNumber");

            public static uint Cocaine = NAPI.Util.GetHashKey("bkr_prop_coke_powder_02");
                
            public static uint Neonstick = NAPI.Util.GetHashKey("neonstick");
            public static uint Neonstickr = NAPI.Util.GetHashKey("neonstickr");
            
            public static uint Glowstick = NAPI.Util.GetHashKey("Glowstick");
            public static uint Glowstickr = NAPI.Util.GetHashKey("Glowstickr");
        }
        private static readonly nLog Log = new nLog("Chars.Attachments");

        public static void AddAttachment(ExtPlayer player, uint attachmentHash, bool send = true)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData != null && !sessionData.Attachments.Contains(attachmentHash))
                {
                    sessionData.Attachments.Add(attachmentHash);
                    if (send) Trigger.SetSharedData(player, "attachmentsData", JsonConvert.SerializeObject(sessionData.Attachments));
                }
            } 
            catch(Exception e)
            {
                Log.Write($"AddAttachment Exception: {e.ToString()}");
            }
        }
        public static void RemoveAttachment(ExtPlayer player, uint attachmentHash, bool send = true)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData != null && sessionData.Attachments.Contains(attachmentHash))
                {
                    sessionData.Attachments.Remove(attachmentHash);
                    if (send) Trigger.SetSharedData(player, "attachmentsData", JsonConvert.SerializeObject(sessionData.Attachments));
                }
            }
            catch (Exception e)
            {
                Log.Write($"AddAttachment Exception: {e.ToString()}");
            }
        }
        public static bool HasAttachment(ExtPlayer player, uint attachmentHash)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData != null && sessionData.Attachments.Contains(attachmentHash)) return true;
                return false;
            }
            catch (Exception e)
            {
                Log.Write($"HasAttachment Exception: {e.ToString()}");
                return false;
            }
        }

        [RemoteEvent("staticAttachments.Add")]
        public void staticAttachmentsAdd(ExtPlayer player, string hash)
        {
            try
            {
                AddAttachment(player, (uint)Convert.ToInt64(hash));
            }
            catch (Exception e)
            {
                Log.Write($"staticAttachmentsAdd Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("staticAttachments.Save")]
        public void staticAttachmentsSave(ExtPlayer player, string model, int BoneId, string sPos, string sRot)
        {
            try
            {
                Vector3 pos = JsonConvert.DeserializeObject<Vector3>(sPos);
                Vector3 rot = JsonConvert.DeserializeObject<Vector3>(sRot);
                using (StreamWriter saveCoords = new StreamWriter("attachments.txt", true, Encoding.UTF8))
                {
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

                    int rand = new Random().Next(1, 9);
                    saveCoords.Write($"mp.attachments.register('{model}-{rand}', '{model}', {BoneId}, new mp.Vector3({pos.X}, {pos.Y}, {pos.Z}), new mp.Vector3({rot.X}, {rot.Y}, {rot.Z}), true);\r\n");
                    saveCoords.Close();
                    Trigger.SendChatMessage(player, $"Saved {model}-{rand}");
                }
            }
            catch (Exception e)
            {
                Log.Write($"staticAttachmentsSave Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("staticAttachments.Remove")]
        public void staticAttachmentsRemove(ExtPlayer player, string hash)
        {
            try
            {
                RemoveAttachment(player, (uint)Convert.ToInt64(hash));
            }
            catch (Exception e)
            {
                Log.Write($"staticAttachmentsRemove Exception: {e.ToString()}");
            }
        }
        [Command("natt")]
        public void CMD_NAttachments(ExtPlayer player, string id, int boneName)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Att) && Main.ServerNumber != 0) return;
                Trigger.ClientEvent(player, "objecteditor:start", id, boneName);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_NAttachments Exception: {e.ToString()}");
            }
        }
        [Command("createanimlist")]
        public void CMD_animplayer(ExtPlayer player, string text)
        {
            try
            {
                if (Main.ServerNumber != 0) return;
                Trigger.ClientEvent(player, "createAnimList", text);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_animFlag Exception: {e.ToString()}");
            }
        }
        [Command("findanim")]
        public void CMD_animplayer(ExtPlayer player, int dist, string name)
        {
            try
            {
                if (Main.ServerNumber != 0) return;
                Trigger.ClientEvent(player, "findAnim", dist, name);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_animFlag Exception: {e.ToString()}");
            }
        }
        [Command("animflag")]
        public void CMD_findAnim(ExtPlayer player, int flag)
        {
            try
            {
                if (Main.ServerNumber != 0) return;
                Trigger.ClientEvent(player, "animFlag", flag);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_findAnim Exception: {e.ToString()}");
            }
        }
        /*[Command("animflag")]
        public static void CMD_findAnim(Player player, string flag)
        {
            try
            {
                if (Main.ServerNumber != 0) return;
                if (flag == "up" || flag == "down") Trigger.ClientEvent(player, "animFlag", flag);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_findAnim Exception: {e.ToString()}");
            }
        }*/
    }
}
