using System;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Handles;
using NeptuneEvo.Table.Models;
using Redage.SDK;

namespace NeptuneEvo.Fractions.Table.Settings
{
    public class Repository
    {
        public static void UpdateStock(ExtPlayer player)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.OpenStock)) return;
                
                var fractionData = player.GetFractionData();
                if (fractionData == null) 
                    return;

                fractionData.IsOpenStock = !fractionData.IsOpenStock;

                if (fractionData.IsOpenStock)
                {
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы разрешили крафт оружия", 3000);
                    Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.OpenStock, "Разрешил крафт оружия");
                    Manager.sendFractionMessage(fractionData.Id, "!{#ADFF2F}[F] " + $"{player.Name} ({player.Value}) разрешил крафт оружия.", true);
                }
                else
                {
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы запретили крафт оружия", 3000);
                    Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.CloseStock, "Запретил крафт оружия");
                    Manager.sendFractionMessage(fractionData.Id, "!{#ADFF2F}[F] " + $"{player.Name} ({player.Value}) запретил крафт оружия.", true);
                }

                Trigger.ClientEvent(player, "client.frac.main.isStock", fractionData.IsOpenStock);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static void UpdateGunStock(ExtPlayer player)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.OpenGunStock)) return;
                
                var fractionData = player.GetFractionData();
                if (fractionData == null) 
                    return;

                fractionData.IsOpenGunStock = !fractionData.IsOpenGunStock;

                if (fractionData.IsOpenGunStock)
                {
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы открыли склад фракции", 3000);
                    Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.OpenStock, "Открыл склад");
                    Manager.sendFractionMessage(fractionData.Id, "!{#ADFF2F}[F] " + $"{player.Name} ({player.Value}) открыл склад.", true);
                }
                else
                {
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы закрыли склад фракции", 3000);
                    Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.CloseStock, "Закрыл склад");
                    Manager.sendFractionMessage(fractionData.Id, "!{#ADFF2F}[F] " + $"{player.Name} ({player.Value}) закрыл склад.", true);
                }

                Trigger.ClientEvent(player, "client.frac.main.isGunStock", fractionData.IsOpenGunStock);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
    }
}