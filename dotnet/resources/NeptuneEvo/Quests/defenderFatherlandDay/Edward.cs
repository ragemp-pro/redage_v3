using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using NeptuneEvo.Chars;

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
    public enum edward_quests
    {
        Error = -99,
        NoMission = -1,
        Start = 0,
        Docs = 3,
        Help = 4,
        End = 11
    };
    class Edward : Script
    {
        private static readonly nLog Log = new nLog("Quests.Pavel");

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
               // PedSystem.Repository.CreateQuest("s_m_y_barman_01", new Vector3(-1692.0126, -743.00085, 10.183417), 148.7835f, questName: "npc_fd_edward", title: "~y~NPC~w~ Эдвард\nКвестовый персонаж", colShapeEnums: ColShapeEnums.QuestPavel, isBlipVisible: false);
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

                edward_quests returnLine = Get(player, PlayerQuestData.Line);
                if (returnLine != edward_quests.Error)
                {
                    qMain.UpdatePerform(player, "npc_fd_edward", (short)returnLine);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Perform Task.Run Exception: {e.ToString()}");
            }
        }
        public static edward_quests Get(ExtPlayer player, int Line, bool Reward = false)
        {
            if (!player.IsCharacterData()) return edward_quests.Error;
            switch ((edward_quests)Line)
            {
                case edward_quests.Docs:
                    qMain.UpdateQuestsLine(player, "npc_fd_zak", (int)zak_quests.NoMission, (int)zak_quests.End);
                    qMain.UpdateDisplayInHood(player, "npc_fd_edward", false);
                    qMain.UpdateDisplayInHood(player, "npc_fd_zak", true);
                    //Награды
                    //UpdateData.Exp(player, 10);
                    //MoneySystem.Wallet.Change(player, 5000);
                    return edward_quests.NoMission;
                default:
                    // Not supposed to end up here. 
                    break;
            }
            return edward_quests.Error;
        }
        public static void Take(ExtPlayer player, int Line)
        {
            if (!player.IsCharacterData()) return;

            switch ((edward_quests)Line)
            {
                case edward_quests.Start:
                    qMain.UpdateQuestsLine(player, "npc_fd_edward", (int)edward_quests.Start, (int)edward_quests.Docs);
                    Trigger.ClientEvent(player, "client.create.npc_dfday_mission", 2);
                    break;
                default:
                    qMain.UpdateQuestsLine(player, "npc_fd_edward", (int)edward_quests.Start, (int)edward_quests.Docs);
                    Trigger.ClientEvent(player, "client.create.npc_dfday_mission", 2);
                    break;
            }
        }

        //[RemoteEvent("server.quest.open.npc_fd_pavel")]
        [Interaction(ColShapeEnums.QuestPavel)]
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

            bool isBool = qMain.SetQuests(player, "npc_fd_edward");
            if (!isBool) return;
            var questData = player.GetQuest();
            if (questData == null) 
                return;

            Trigger.ClientEvent(player, "client.quest.open", index, "npc_fd_edward", questData.Line, questData.Status, questData.Complete);
        }

        [RemoteEvent("server.update.npc_fd_edward")]
        public static void UpdateNpcPavelMission(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                qMain.UpdateQuestsComplete(player, "npc_fd_edward", (int)edward_quests.Docs, true);
            }
            catch (Exception e)
            {
                Log.Write($"UpdateNpcDFDayMission Exception: {e.ToString()}");
            }
        }
    }
}
