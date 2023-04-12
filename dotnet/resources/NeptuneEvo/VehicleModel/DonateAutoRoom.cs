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
    public class DonateAutoRoom : Script
    {
        public static readonly nLog Log = new nLog("VehicleModel.DonateAutoRoom");
        
        public static Vector3 NpcBuyPosition = new Vector3(-1101.3359, -1351.1681, 5.0338033);
        private static float NpcBuyRotation = -155.546776f;
        //
        public static string NpcName = "npc_donateautoroom";
        [ServerEvent(Event.ResourceStart)]
        public void Event_ResourceStart()
        {
 
            Main.CreateBlip(new Main.BlipData(595, "Exotic DonateRoom", NpcBuyPosition, 1, true));
            PedSystem.Repository.CreateQuest("a_f_m_fatbla_01", NpcBuyPosition, NpcBuyRotation, title: "~y~NPC~w~ Доната Редбаксовна", colShapeEnums: ColShapeEnums.DonateAutoroom);
        }
        [Interaction(ColShapeEnums.DonateAutoroom)]
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

                if (!FunctionsAccess.IsWorking("OpenDonateAutoroom"))
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
                    .Where(b => b.Value.Type == BusinessManager.BusProductToType.Donate)
                    .Where(b => b.Value.OtherPrice > 0)
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
                
                CarRoom.OpenCarromMenuGos(player, donateVehiclesInfo, isDonate: true);
            }
            catch (Exception e)
            {
                Log.Write($"Perform Exception: {e.ToString()}");
            }
        }
    }
}