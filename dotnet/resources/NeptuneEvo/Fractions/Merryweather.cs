using System;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Handles;
using NeptuneEvo.Core;
using Redage.SDK;
using NeptuneEvo.GUI;

using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Players.Popup.List.Models;

namespace NeptuneEvo.Fractions
{
    class Merryweather : Script
    {
        private static readonly nLog Log = new nLog("Fractions.Merryweather");

        public static Vector3[] Coords = new Vector3[3]
        {
            new Vector3(2036.511, 2941.482, -63.021739), // Колшэйп входа на другой этаж // 2036.511, 2941.482, -61.90174
            new Vector3(2157.496, 2921.067, -82.19534), // Колшэйп изнутри этажа, чтобы вернуться назад // 2157.496, 2921.067, -81.07534
            new Vector3(2038.304, 2935.693, -62.90551), // Колшэйп раздевалки :yes:
        };

        public static Vector3[] CoordsToLift = new Vector3[4]
        {
            new Vector3(-144.151, -576.9016, 31.42646),
            new Vector3(2151.268, 2920.965, -62.899879),
            new Vector3(-133.7187, -584.309, 200.7373),
            new Vector3(1048.357, -3097.174, -39.99794),
        };
        public static void OpenLiftMenu(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Trigger.ClientEvent(player, "openlift", 1, "fraction.lift.merryweather");
            }
            catch (Exception e)
            {
                Log.Write($"OpenLiftMenu Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("fraction.lift.merryweather")]
        public static void callback_lift(ExtPlayer player, int floor)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (player.IsInVehicle) return;

                if (sessionData.Following != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsFollowing), 5000);
                    return;
                }
                string data = (characterData.Gender) ? "128_0_true" : "98_0_false";

                if (player.GetFractionId() != (int) Models.Fractions.MERRYWEATHER && Chars.Repository.isItem(player, "inventory", ItemId.Jewelry, data) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsBadgeFIBorPD), 5000);
                    return;
                }
                NAPI.Entity.SetEntityPosition(player, CoordsToLift[floor] + new Vector3(0, 0, 1.12));
                Main.PlayerEnterInterior(player, CoordsToLift[floor] + new Vector3(0, 0, 1.12));
            }
            catch (Exception e)
            {
                Log.Write($"callback_lift Exception: {e.ToString()}");
            }
        }
    }
}
