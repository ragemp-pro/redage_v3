using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;

using NeptuneEvo.Functions;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using Redage.SDK;
using System;
using System.Linq;
using System.Threading.Tasks;
using Localization;
using NeptuneEvo.Quests.Models;

namespace NeptuneEvo.Quests
{
    public enum granny_quests
    {
        Error = -99,
        NoMission = -1,
        Start = 0,
        Beer = 2,
        Introduction = 3,
        Bear = 7
    };
    class Granny : Script
    {
        private static readonly nLog Log = new nLog("Quests.Granny");

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
           // PedSystem.Repository.CreateQuest("a_f_m_eastsa_01", new Vector3(-1661.0814, -1002.26465, 7.3779097), 98.5713f, questName: "npc_granny", title: "~y~NPC~w~ Бабушка Granny\nКвестовый персонаж", colShapeEnums: ColShapeEnums.QuestGranny);
        }
        public static void Perform(ExtPlayer player, PlayerQuestModel PlayerQuestData)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                granny_quests returnLine = Get(player, PlayerQuestData.Line);
                if (returnLine != granny_quests.Error)
                {
                    var questData = player.GetQuest();
                    if (questData != null)
                        questData.Line = (int)returnLine;

                    qMain.UpdatePerform(player, "npc_granny", (short)returnLine);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Perform Task.Run Exception: {e.ToString()}");
            }
        }
        public static granny_quests Get(ExtPlayer player, int Line, bool Reward = false)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return granny_quests.Error;

            switch ((granny_quests)Line)
            {
                case granny_quests.Start:
                    return granny_quests.Beer;
                case granny_quests.Beer:
                    ItemStruct wItemStruct = Chars.Repository.isItem(player, "inventory", ItemId.Beer);
                    if (wItemStruct != null)
                    {
                        Chars.Repository.RemoveIndex(player, wItemStruct.Location, wItemStruct.Index, 1);
                        return granny_quests.Introduction;
                    }
                    break;
                case granny_quests.Bear:
                    if (Chars.Repository.getCountItem($"char_{characterData.UUID}", ItemId.Bear, bagsToggled: false) >= 2)
                    {
                        Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", ItemId.Bear, 2);
                        qMain.UpdateQuestsLine(player, "npc_tracy", (int)tracy_quests.NoMission, (int)tracy_quests.End);
                        qMain.UpdateDisplayInHood(player, "npc_granny", false);
                        qMain.UpdateDisplayInHood(player, "npc_tracy", true);
                        return granny_quests.NoMission;
                    }
                    break;
                default:
                    // Not supposed to end up here. 
                    break;
            }
            return granny_quests.Error;
        }
        public static void Take(ExtPlayer player, int Line)
        {
            if (!player.IsCharacterData()) return;

            switch ((granny_quests)Line)
            {
                case granny_quests.Start:
                    qMain.UpdateQuestsLine(player, "npc_granny", (int)granny_quests.Start, (int)granny_quests.Beer);
                    //Vector3 waypoint = BusinessManager.getNearestBiz(player, 0);
                    //Trigger.ClientEvent(player, "createWaypoint", waypoint.X, waypoint.Y);
                    break;
                case granny_quests.Introduction:
                    qMain.UpdateQuestsLine(player, "npc_granny", (int)granny_quests.Introduction, (int)granny_quests.Bear);
                    Trigger.ClientEvent(player, "client.start.collecting_items", 0);
                    break;
                default:
                    // Not supposed to end up here. 
                    break;
            }
        }
        public static void Action(ExtPlayer player, int Line)
        {
            if (!player.IsCharacterData()) return;

            switch ((granny_quests)Line)
            {
                case granny_quests.Start:
                    player.Health -= 5;
                    break;
                default:
                    // Not supposed to end up here. 
                    break;
            }
        }
        [Interaction(ColShapeEnums.QuestGranny)]
        public static void Open(ExtPlayer player, int index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            if (sessionData.CuffedData.Cuffed)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsCuffed), 3000);
                return;
            }
            else if (sessionData.DeathData.InDeath)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsDying), 3000);
                return;
            }
            else if (Main.IHaveDemorgan(player, true)) return;

            bool isBool = qMain.SetQuests(player, "npc_granny");
            if (!isBool) return;
            var questData = player.GetQuest();
            if (questData == null)
                return;

            if (questData.Line == (int)granny_quests.Beer)
            {

                var isItem = Chars.Repository.isItem(player, "inventory", ItemId.Beer) != null ? true : false;
                if (isItem)
                    questData.Complete = true;
                else
                    questData.Complete = false;
            }

            if (questData.Line == (int)granny_quests.Bear)
            {
                var isItem = Chars.Repository.getCountItem($"char_{characterData.UUID}", ItemId.Bear, bagsToggled: false) >= 5 ? true : false;
                if (isItem)
                {
                    questData.Complete = true;
                    Trigger.ClientEvent(player, "createWaypoint", 357.3562, -586.0162);
                }
                else
                    questData.Complete = false;
            }


            Trigger.ClientEvent(player, "client.quest.open", index, "npc_granny", questData.Line, questData.Status, questData.Complete);
        }
        [RemoteEvent("server.take_quest_item")]
        public void take_quest_item(ExtPlayer player, int index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;

            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            else if (sessionData.AntiAnimDown || (characterData.AdminLVL >= 1 && characterData.AdminLVL <= 5) || Main.IHaveDemorgan(player)) return;
            else if (Main.IHaveDemorgan(player)) return;
            else if (Chars.Repository.isFreeSlots(player, ItemId.Bear, 1) != 0) return;
            Main.OnAntiAnim(player);
            Trigger.PlayAnimation(player, "anim@mp_snowball", "pickup_snowball", 39);
            // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "pickup_snowball");
            Timers.StartOnce(5000, () =>
            {
                try
                {
                    if (!player.IsCharacterData()) return;
                    Trigger.StopAnimation(player);
                    Main.OffAntiAnim(player);
                    if (Chars.Repository.isFreeSlots(player, ItemId.Bear, 1) != 0) return;
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Bear, 1);

                    index++;

                    Trigger.ClientEvent(player, "client.start.collecting_items", index);

                    qMain.UpdateQuestsData(player, "npc_granny", (int)granny_quests.Bear, index.ToString());

                    if (index >= 2)
                    {
                        Trigger.SendChatMessage(player, "Вы собрали всех Мишек, отдайте их Бабушке Granny!");
                    }
                    else if (index == 1)
                    {
                        Trigger.SendChatMessage(player, "Вы собрали 1/2 Мишек. Остался последний Мишка. На карте отмечено следующее примерное расположение последнего Мишки!");
                    }

                }
                catch (Exception e)
                {
                    Log.Write($"EventsCollect Task #1 Exception: {e.ToString()}");
                }
            });

        }
    }
}
