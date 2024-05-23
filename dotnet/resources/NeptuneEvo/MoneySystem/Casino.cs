using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Functions;
using Redage.SDK;
using System;
using Localization;
using NeptuneEvo.Fractions.Player;

namespace NeptuneEvo.MoneySystem
{
    class Casino : Script
    {
        private static readonly nLog Log = new nLog("MoneySystem.Casino");

        public static int[] rednums = new int[18] { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 }; // Все красные числа на рулетке

        //private static Blip blip;

        public static Vector3[] casinoChecks = new Vector3[7]
                {
            new Vector3(926.2809, 49.090824, 81.10633), // Маркер входа с улицы в казино
            new Vector3(1090.358, 206.7677, -50.11972), // Маркер выхода на улицу из казино
            new Vector3(1110.729, 225.6086, -50.56078), // позиция начала игры в кости
            new Vector3(964.423, 58.9255, 111.4331), // Маркер выхода с крыши казино
            new Vector3(1085.918, 214.6145, -50.32042), // Маркер входа на крышу казино
            new Vector3(967.8069, 63.79589, 111.433), // Маркер входа в penthouse с крыши казино
            new Vector3(969.7232, 63.0152, 111.4363) // Маркер выхода из penthouse на крышу казино
                };

        #region Events
        public static void OnResourceStart()
        {
            try
            {
                CustomColShape.CreateCylinderColShape(casinoChecks[0], 1, 2, 0, ColShapeEnums.Casino, 2);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(casinoChecks[0].X, casinoChecks[0].Y, casinoChecks[0].Z), 5F, 0.3F, 0, new Color(255, 255, 255));
                NAPI.Marker.CreateMarker(21, casinoChecks[0] + new Vector3(0, 0, 0), new Vector3(), new Vector3(), 0.8f, new Color(255, 255, 255, 60));

                CustomColShape.CreateCylinderColShape(casinoChecks[1], 1, 2, 0, ColShapeEnums.Casino, 3);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(casinoChecks[1].X, casinoChecks[1].Y, casinoChecks[1].Z + 1), 5F, 0.3F, 0, new Color(255, 255, 255));
                NAPI.Marker.CreateMarker(21, casinoChecks[1] + new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 0.8f, new Color(255, 255, 255, 60));

                CustomColShape.CreateCylinderColShape(casinoChecks[3], 1, 2, 0, ColShapeEnums.Casino, 4);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(casinoChecks[3].X, casinoChecks[3].Y, casinoChecks[3].Z + 1), 5F, 0.3F, 0, new Color(255, 255, 255));
                NAPI.Marker.CreateMarker(21, casinoChecks[3] + new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 0.8f, new Color(255, 255, 255, 60));

                CustomColShape.CreateCylinderColShape(casinoChecks[4], 1, 2, 0, ColShapeEnums.Casino, 5);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(casinoChecks[4].X, casinoChecks[4].Y, casinoChecks[4].Z + 1), 5F, 0.3F, 0, new Color(255, 255, 255));
                NAPI.Marker.CreateMarker(21, casinoChecks[4] + new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 0.8f, new Color(255, 255, 255, 60));

                CustomColShape.CreateCylinderColShape(casinoChecks[5], 1, 2, 0, ColShapeEnums.Casino, 6);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(casinoChecks[5].X, casinoChecks[5].Y, casinoChecks[5].Z + 1), 5F, 0.3F, 0, new Color(255, 255, 255));
                NAPI.Marker.CreateMarker(21, casinoChecks[5] + new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 0.8f, new Color(255, 255, 255, 60));

                CustomColShape.CreateCylinderColShape(casinoChecks[6], 1, 2, 0, ColShapeEnums.Casino, 7);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(casinoChecks[6].X, casinoChecks[6].Y, casinoChecks[6].Z + 1), 5F, 0.3F, 0, new Color(255, 255, 255));
                NAPI.Marker.CreateMarker(21, casinoChecks[6] + new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 0.8f, new Color(255, 255, 255, 60));

                Main.CreateBlip(new Main.BlipData(681, "Казино", casinoChecks[0], 18, true));
            }
            catch (Exception e)
            {
                Log.Write($"OnResourceStart Exception: {e.ToString()}");
            }
        }
        #endregion
        [Interaction(ColShapeEnums.Casino)]
        public static void OnCasino(ExtPlayer player, int interact)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (Main.IHaveDemorgan(player, true)) return;
                switch (interact)
                {
                    case 2:
                        if (player.IsInVehicle) return;
                        if (sessionData.Following != null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomebodyYouFollow), 3000);
                            return;
                        }
                        characterData.InCasino = true;     
                        NAPI.Entity.SetEntityPosition(player, casinoChecks[1] + new Vector3(0, 0, 1.12));
                        return;
                    case 3:
                        if (player.IsInVehicle) return;
                        if (sessionData.Following != null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomebodyYouFollow), 3000);
                            return;
                        }
                        characterData.InCasino = false;
                        NAPI.Entity.SetEntityPosition(player, casinoChecks[0] + new Vector3(0, 0, 1.12));
                        return;
                    case 4:
                        if (player.IsInVehicle) return;
                        if (sessionData.Following != null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomebodyYouFollow), 3000);
                            return;
                        }
                        characterData.InCasino = true;
                        NAPI.Entity.SetEntityPosition(player, casinoChecks[4] + new Vector3(0, 0, 1.12));
                        return;
                    case 5:
                        if (player.IsInVehicle) return;
                        if (sessionData.Following != null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomebodyYouFollow), 3000);
                            return;
                        }
                        characterData.InCasino = false;
                        NAPI.Entity.SetEntityPosition(player, casinoChecks[3] + new Vector3(0, 0, 1.12));
                        return;
                    case 6:
                        if (player.IsInVehicle) return;
                        if (sessionData.Following != null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomebodyYouFollow), 3000);
                            return;
                        }
                        if ((!Main.DoorsControl.ContainsKey("am_3") || Main.DoorsControl["am_3"]) && player.GetFractionId() != (int)Fractions.Models.Fractions.ARMENIAN)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ClubDoorClosed), 3000);
                            return;
                        }
                        Trigger.ClientEvent(player, "pentload");
                        NAPI.Entity.SetEntityPosition(player, casinoChecks[6] + new Vector3(0, 0, 1.12));
                        return;
                    case 7:
                        if (player.IsInVehicle) return;
                        if (sessionData.Following != null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomebodyYouFollow), 3000);
                            return;
                        }
                        if ((!Main.DoorsControl.ContainsKey("am_3") || Main.DoorsControl["am_3"]) && player.GetFractionId() != (int)Fractions.Models.Fractions.ARMENIAN)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ClubDoorClosed), 3000);
                            return;
                        }
                        NAPI.Entity.SetEntityPosition(player, casinoChecks[5] + new Vector3(0, 0, 1.12));
                        return;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"Interact Exception: {e.ToString()}");
            }
        }
    }
}
