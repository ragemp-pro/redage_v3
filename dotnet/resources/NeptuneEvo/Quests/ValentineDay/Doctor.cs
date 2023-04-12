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
    public enum doctor_quests
    {
        Error = -99,
        NoMission = -1,
        Start = 0,
    };
    class Doctor : Script
    {
        private static readonly nLog Log = new nLog("Quests.Doctor");
        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
           // PedSystem.Repository.CreateQuest("s_m_m_doctor_01", new Vector3(356.51773, -583.74817, 43.261223), -174f, questName: "npc_doctor", title: "~y~NPC~w~ Доктор Шульц\nКвестовый персонаж", colShapeEnums: ColShapeEnums.QuestDoctor, isBlipVisible: false);
        }
        public static void Perform(ExtPlayer player, PlayerQuestModel PlayerQuestData)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                doctor_quests returnLine = Get(player, PlayerQuestData.Line);
                if (returnLine != doctor_quests.Error)
                {
                    qMain.UpdatePerform(player, "npc_doctor", (short)returnLine);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Perform Task.Run Exception: {e.ToString()}");
            }
        }
        public static doctor_quests Get(ExtPlayer player, int Line, bool Reward = false)
        {
            if (!player.IsCharacterData()) return doctor_quests.Error;

            switch ((doctor_quests)Line)
            {
                case doctor_quests.Start:
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "На карте отмечены примерные координаты Бабушки Granny.", 3000);
                    Trigger.ClientEvent(player, "createWaypoint", -1661.0814f, -1002.26465f);
                    qMain.SetQuests(player, "npc_granny", true, 0, isReturn: false);
                    qMain.UpdateDisplayInHood(player, "npc_doctor", false);
                    qMain.UpdateDisplayInHood(player, "npc_granny", true);
                    return doctor_quests.NoMission;
                default:
                    // Not supposed to end up here. 
                    break;
            }
            return doctor_quests.Error;
        }
        [Interaction(ColShapeEnums.QuestDoctor)]
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

            bool isBool = qMain.SetQuests(player, "npc_doctor");
            if (!isBool) return;
            var questData = player.GetQuest();
            if (questData == null) 
                return;

            Trigger.ClientEvent(player, "client.quest.open", index, "npc_doctor", questData.Line, questData.Status, questData.Complete);
        }
    }
}
