using System;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Handles;
using Redage.SDK;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.Players;
using NeptuneEvo.Character;
using NeptuneEvo.Quests;

namespace NeptuneEvo.Events
{
    public class EventsMenu : Script
    {
        private static readonly nLog Log = new nLog("Events.EventsMenu");
        
        [ServerEvent(Event.ResourceStart)]
        public void Event_ResourceStart()
        {
            try
            {
                CustomColShape.CreateSphereColShape(new Vector3(-488.47617, -401.0276, 34.79613), 7f, 0, ColShapeEnums.EventsMenu);
                PedSystem.Repository.CreateQuest("a_m_m_afriamer_01", new Vector3(-486.10715, -399.39944, 34.546597), -47.65183f, title: "~y~NPC~w~ Саня\nПрофессиональный игроман");
                //NAPI.Marker.CreateMarker(1, new Vector3(-478.86032, -395.27307, 34.027653 - 1.25), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220));
                Main.CreateBlip(new Main.BlipData(491, LangFunc.GetText(LangType.Ru, DataName.Events), new Vector3(-483.149, -400.09946, 34.546608), 6, true));
            }
            catch (Exception e)
            {
                Log.Write($"Event_ResourceStart Exception: {e.ToString()}");
            }
        }
        
        [Interaction(ColShapeEnums.EventsMenu)]
        public static void EventsMenuInteraction(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                if (!FunctionsAccess.IsWorking("EventsMenuInteraction"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                /*else if (characterData.LVL < 3)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.lvl3forgame), 3000);
                    return;
                }*/
                else if (sessionData.WorkData.OnDuty || sessionData.WorkData.OnWork)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustEndWorkDay), 3000);
                    return;
                }
                else if (sessionData.CuffedData.Cuffed)
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

                qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage30, 1, isUpdateHud: true);
                Trigger.ClientEvent(player, "eventsMenuShow");
            }
            catch (Exception e)
            {
                Log.Write($"EventsMenuInteraction Exception: {e.ToString()}");
            }
        }
        
        [Interaction(ColShapeEnums.EventsMenu, Out: true)]
        public static void OutEventsMenuZone(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                TankRoyale.OutTanksLobbyZone(player);
                MafiaGame.OutMafiaLobbyZone(player);
                Airsoft.OutLobbyZone(player);
            }
            catch (Exception e)
            {
                Log.Write($"OutEventsMenuZone Exception: {e.ToString()}");
            }
        }
        
        [RemoteEvent("selectEventServer")]
        public static void SelectEventServer(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                if (index == 0 && sessionData.InAirsoftLobby == -1 && sessionData.InTanksLobby == -1) MafiaGame.CheckMafiaLobby(player);
                else if (index == 1 && sessionData.InAirsoftLobby == -1 && sessionData.InMafiaLobby == -1) TankRoyale.CheckLobby(player);
                else if (index == 2 && sessionData.InMafiaLobby == -1 && sessionData.InTanksLobby == -1) Airsoft.LoadAirsoftLobbyList(player);
            }
            catch (Exception e)
            {
                Log.Write($"SelectEventServer Exception: {e.ToString()}");
            }
        }
    }
}