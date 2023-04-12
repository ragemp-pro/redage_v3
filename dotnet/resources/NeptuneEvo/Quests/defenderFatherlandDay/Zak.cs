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
    public enum zak_quests
    {
        Error = -99,
        NoMission = -1,
        Phone = 0,
        Dialog = 2,
        End = 11,
    };
    class Zak : Script
    {
        private static readonly nLog Log = new nLog("Quests.Tracy");

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
               // PedSystem.Repository.CreateQuest("ig_car3guy2", new Vector3(-1390.7114, -597.82434, 30.319658), 80.87341f, questName: "npc_fd_zak", title: "~y~NPC~w~ Зак Цукерберг\nКвестовый персонаж", colShapeEnums: ColShapeEnums.QuestZak);
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

                zak_quests returnLine = Get(player, PlayerQuestData.Line);

                if (returnLine != zak_quests.Error)
                {
                    qMain.UpdatePerform(player, "npc_fd_zak", (short)returnLine);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Task.Run Exception: {e.ToString()}");
            }
        }
        public static zak_quests Get(ExtPlayer player, int Line, bool Reward = false)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return zak_quests.Error;

            switch ((zak_quests)Line)
            {
                case zak_quests.Phone:
                    qMain.SetQuests(player, "npc_fd_dada", true, 0, isReturn: false);
                    UpdateData.Exp(player, 10);
                    MoneySystem.Wallet.Change(player, 2500);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Mask, 1, "191_0_True"); 
                    qMain.UpdateDisplayInHood(player, "npc_fd_zak", false); 
                    qMain.UpdateDisplayInHood(player, "npc_fd_dada", true);
                    return zak_quests.NoMission;
                case zak_quests.End:
                    //Награды
                    UpdateData.Exp(player, 10);
                    MoneySystem.Wallet.Change(player, 5000);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Case0, 1);
                    return zak_quests.NoMission;
                default:
                    // Not supposed to end up here. 
                    break;
            }        
            return zak_quests.Error;
        }

        public static void Action(ExtPlayer player, int Line)
        {
            if (!player.IsCharacterData()) return;

            switch ((zak_quests)Line)
            {
                case zak_quests.Dialog:
                    Trigger.ClientEvent(player, "startScreenEffect", "PPFilter", 15 * 1000, false);
                    break;
                default:
                    // Not supposed to end up here. 
                    break;
            }
        }

        //[RemoteEvent("server.quest.open.npc_fd_zak")]
        [Interaction(ColShapeEnums.QuestZak)]
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

            bool isBool = qMain.SetQuests(player, "npc_fd_zak", isInsert: true);
            if (!isBool) return;
            var questData = player.GetQuest();
            if (questData == null) 
                return;

            Trigger.ClientEvent(player, "client.quest.open", index, "npc_fd_zak", questData.Line, questData.Status, questData.Complete);
        }
    }
}
