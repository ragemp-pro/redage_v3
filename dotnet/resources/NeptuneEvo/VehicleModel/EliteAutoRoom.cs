using System;
using System.Linq;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using NeptuneEvo.Quests.Models;
using Redage.SDK;

namespace NeptuneEvo.VehicleModel
{
    public class EliteAutoRoom : Script
    {
        public static readonly nLog Log = new nLog("VehicleModel.EliteAutoRoom");
        public static Vector3 NpcBuyPosition = new Vector3(-1380.4304, -256.1946, 42.821304);
        private static float NpcBuyRotation = -97.236244f;
        //
        public static string NpcName = "npc_eliteroom";
        [ServerEvent(Event.ResourceStart)]
        public void Event_ResourceStart()
        {
 
            Main.CreateBlip(new Main.BlipData(669, "Elite AutoRoom", NpcBuyPosition, 4, true));
            PedSystem.Repository.CreateQuest("s_m_m_movprem_01", NpcBuyPosition, NpcBuyRotation, title: "~y~NPC~w~ Продавец элитного транспорта", colShapeEnums: ColShapeEnums.EliteAutoRoom);
        }
        [Interaction(ColShapeEnums.EliteAutoRoom)]
        public static void OpenDialog(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (sessionData.CuffedData.Cuffed)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsCuffed), 3000);
                    return;
                }
                if (sessionData.DeathData.InDeath)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsDying), 3000);
                    return;
                }
                if (Main.IHaveDemorgan(player, true)) return;

                player.SelectQuest(new PlayerQuestModel(NpcName, 0, 0, false, DateTime.Now));
                Trigger.ClientEvent(player, "client.quest.open", index, NpcName, 0, 0, 0);
            }
            catch (Exception e)
            {
                Log.Write($"OpenDialog Exception: {e.ToString()}");
            }
        }
        public static void Perform(ExtPlayer player)
        {
            try
            {

                if (!FunctionsAccess.IsWorking("OpenEliteAutoRoom"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (sessionData.CuffedData.Cuffed)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsCuffed), 3000);
                    return;
                }
                if (sessionData.DeathData.InDeath)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsDying), 3000);
                    return;
                }
                if (Main.IHaveDemorgan(player, true)) return;

                var donateVehiclesInfo = BusinessManager.BusProductsData
                    .Where(b => b.Value.Type == BusinessManager.BusProductToType.Elite)
                    .Where(b => b.Value.Price > 0)
                    .Select(b => b.Key)
                    .ToList();
                
                if (donateVehiclesInfo.Count < 1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoVehAccessed), 3000);
                    return;
                }

                characterData.ExteriorPos = player.Position;
                //NAPI.Entity.SetEntityPosition(player, new Vector3(CamPosition.X, CamPosition.Y - 2, CamPosition.Z));
                Trigger.UniqueDimension(player);
                
                CarRoom.OpenCarromMenuGos(player, donateVehiclesInfo, isDonate: false);
            }
            catch (Exception e)
            {
                Log.Write($"Perform Exception: {e.ToString()}");
            }
        }
    }
}