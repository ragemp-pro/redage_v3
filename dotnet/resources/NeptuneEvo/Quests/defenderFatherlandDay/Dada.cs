using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;

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
    public enum data_quests
    {
        Error = -99,
        NoMission = -1,
        Start = 0,
        Tools = 4,
        Boat = 5,
        End = 11
    };

    class Dada : Script
    {
        private static readonly nLog Log = new nLog("Quests.Dada");

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
               // PedSystem.Repository.CreateQuest("a_m_o_beach_01", new Vector3(450.9351, -633.97015, 28.522999), -112.8584f, questName: "npc_fd_dada", title: "~y~NPC~w~ Дядюшка\nКвестовый персонаж", colShapeEnums: ColShapeEnums.QuestDada);
            }
            catch (Exception e)
            {
                Log.Write($"Event_ResourceStart Exception: {e.ToString()}");
            }
        }

        public static void Perform(ExtPlayer player, PlayerQuestModel PlayerQuestData)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                data_quests returnLine = Get(player, PlayerQuestData.Line);
                if (returnLine != data_quests.Error)
                {
                    qMain.UpdatePerform(player, "npc_fd_dada", (short)returnLine);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Perform Task.Run Exception: {e.ToString()}");
            }
        }
        public static data_quests Get(ExtPlayer player, int Line, bool Reward = false)
        {
            if (!player.IsCharacterData()) return data_quests.Error;

            switch ((data_quests)Line)
            {
                case data_quests.Tools:
                    return data_quests.Boat;
                case data_quests.Boat:
                    qMain.SetQuests(player, "npc_fd_edward", true, 0, isReturn: false);
                    qMain.UpdateDisplayInHood(player, "npc_fd_dada", false);
                    qMain.UpdateDisplayInHood(player, "npc_fd_edward", true);
                    return data_quests.NoMission;
                default:
                    // Not supposed to end up here. 
                    break;
            }
            return data_quests.Error;
        }
        public static void Take(ExtPlayer player, int Line)
        {
            if (!player.IsCharacterData()) return;

            switch ((data_quests)Line)
            {
                case data_quests.Start:
                    qMain.UpdateQuestsLine(player, "npc_fd_dada", (int)data_quests.Start, (int)data_quests.Tools);
                    if (Chars.Repository.isItem(player, "inventory", ItemId.Hammer) == null && Chars.Repository.isItem(player, "inventory", ItemId.Wrench) == null)
                    {
                        Vector3 waypoint = BusinessManager.getNearestBiz(player, 0);
                        Trigger.ClientEvent(player, "createWaypoint", waypoint.X, waypoint.Y);
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "На карте был отмечен ближайший 24/7.", 3000);
                    }
                    break;
                case data_quests.Tools:
                    qMain.UpdateQuestsLine(player, "npc_fd_dada", (int)data_quests.Tools, (int)data_quests.Boat);
                    Trigger.ClientEvent(player, "client.create.npc_dfday_mission", 1);
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Почини лодку!", 3000);
                    break;
                default:
                    // Not supposed to end up here. 
                    break;
            }
        }

        //[RemoteEvent("server.quest.open.npc_fd_dada")]
        [Interaction(ColShapeEnums.QuestDada)]
        public static void Open(ExtPlayer player, int index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            if (!player.IsCharacterData()) return;
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

            bool isBool = qMain.SetQuests(player, "npc_fd_dada");
            if (!isBool) return;
            var questData = player.GetQuest();
            if (questData == null) 
                return;

            if (questData.Line == (int)data_quests.Tools && !questData.Complete && Chars.Repository.isItem(player, "inventory", ItemId.Hammer) != null && Chars.Repository.isItem(player, "inventory", ItemId.Wrench) != null)
            {
                qMain.UpdateQuestsComplete(player, "npc_fd_dada", (int)data_quests.Tools, true);
                questData.Complete = true;
            }

            Trigger.ClientEvent(player, "client.quest.open", index, "npc_fd_dada", questData.Line, questData.Status, questData.Complete);
        }

        [RemoteEvent("server.update.npc_dfday_mission")]
        public static void UpdateNpcDFDayMission(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                qMain.UpdateQuestsComplete(player, "npc_fd_dada", (int)data_quests.Boat, true);
            }
            catch (Exception e)
            {
                Log.Write($"UpdateNpcDFDayMission Exception: {e.ToString()}");
            }
        }
    }
}
