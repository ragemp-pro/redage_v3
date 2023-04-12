/*
 using GTANetworkAPI;
using Localization;
using NeptuneEvo.Handles;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.MoneySystem;
using Redage.SDK;

namespace NeptuneEvo.Events
{
    class Metro : Script
    {
        private (string, Vector3)[] StationsData = new (string, Vector3)[]
        {
            ("LSIA Terminal", new Vector3(-1102.43, -2730.98, -7.41)),
            ("LSIA Parking", new Vector3(-878.79, -2317.05, -11.73)),
            ("Puerto Del Sol", new Vector3(-541.01, -1288.4, 26.9)),
            ("Strawberry", new Vector3(279f, -1207.1, 38.89)),
            ("Burton", new Vector3(-290.45, -331.06, 10.06)),
            ("Burton", new Vector3(-290.45, -331.06, 10.06)),
            ("Portola Drive", new Vector3(-816.3, -134.11, 19.95)),
            ("Del Perro", new Vector3(-1355.43, -464.75, 15.04)),
            ("Little Seoul", new Vector3(-502.46, -676.59, 11.8)),
            ("Pillbox South", new Vector3(-217.11, -1038.77, 30.14)),
            ("Davis", new Vector3(110.09, -1718.53, 30.11)),
        };
        private int PriceTicket = 50;
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            foreach((string, Vector3) station in StationsData)
            {
                Main.CreateBlip(new Main.BlipData(532, station.Item1, station.Item2, 47, true, 0.75f));
            }
        }
        [RemoteEvent("metro.server.buyTicket")]
        public void OnBuyTicket(ExtPlayer player, string station, int Increase)
        {
            if (!FunctionsAccess.IsWorking("metro"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                return;
            }
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            int totalprice = PriceTicket * Increase;
            if (totalprice < 1) return;
            else if (UpdateData.CanIChange(player, totalprice, true) != 255) return;

            Wallet.Change(player, -totalprice);
            GameLog.Money($"player({characterData.UUID})", $"server", totalprice, $"metro({station})");
            Trigger.ClientEvent(player, "metro.server.buyTicketSuccess", station);
            Trigger.Dimension(player, (uint)(player.Value + 10000));
        }
        [RemoteEvent("metro.server.exit")]
        public void OnExit(ExtPlayer player)
        {
            if (!player.IsCharacterData()) return;
            Trigger.Dimension(player, 0);
        }
    }
}
*/