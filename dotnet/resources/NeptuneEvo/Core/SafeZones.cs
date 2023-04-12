using System;
using System.Collections.Generic;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Handles;

using NeptuneEvo.Players;
using NeptuneEvo.Functions;
using NeptuneEvo.Quests;
using Redage.SDK;

namespace NeptuneEvo.Core
{
    class SafeZones : Script
    {
        private static readonly nLog Log = new nLog("Core.SafeZones");

        private static List<int> GreenZoneDisabled = new List<int>();
        
        public enum ZoneName
        {
            lspd = 1,
            MafiaGame2 = 2,
            MafiaGame1 = 3,
            Ticket = 4,
            EMS = 5,
            EMSpark = 6,
            Bloods = 7,
            Families = 8,
            MG13 = 9,
            Ballas = 10,
            LCN = 11,
            PDPARK = 12,
            YAK = 13,
            Cityhall = 14,
            ArmenianPark = 15,
            AM = 16,
            RM = 17,
            Agency = 18,
            Tequila = 19,
            ByVitalya = 20,
            Sheriffs2 = 21,
            Sheriffs = 22,
            govfib = 23,
            Court = 24,
            Iglesia = 25,
            SaleTrees = 26,
            SaleOres = 27,
            GovMine = 29,
            Tent2 = 30,
            TentBeach = 31,
            BySource3 = 32,
            BySource2 = 33,
            BySource1 = 34,
            kazikulica = 35,
            lesnik3 = 37,
            lesnik = 38,
            bagama = 39,
            vanilla = 40,
            lesnik2 = 41,
            lesnik4 = 42,
            lesink5 = 43,
            lesnik6 = 44,
            mine4 = 45,
            mine3 = 46,
            mine2 = 47,
            mine1 = 48,
            govmineold = 49,
            lsnews = 50,
            questems2 = 51,
            questems = 52,
            eventsarena = 53,
            DonateShopNew = 54,
            BUS1 = 55,
            MatWarZone = 56,
            Spawn = 57,
            Organization = 58,
            authschool = 59,
            TAXI = 60,
            Hotel3 = 61,
            Hotel2 = 62,
            Hotel1 = 63,
            electric = 64,
            automech = 65,
            collector = 66,
            trucker = 67,
            gopostal = 68,
            kazikinside = 69,
            vagos = 70,
            new1 = 71,
            newarena = 80

        }

        public static bool IsSafeZone(int index, ZoneName name) => index == (int) name;

        [RemoteEvent("IsSafeZone")]
        public void IsSafeZone(ExtPlayer player, bool toggled)
        {
            try
            {
                var sessionData = player.GetSessionData();

                if (sessionData == null) return;
                
                sessionData.IsSafeZone = toggled;
                
                player.SetSharedData("SZ", toggled);
            }
            catch (Exception e)
            {
                Log.Write($"OpenInventory Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("inGreenZone")]
        public void InSafeZone(ExtPlayer player, int index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            if (!GreenZoneDisabled.Contains(index) && !sessionData.WarData.IsWarZone) 
                Trigger.ClientEvent(player, "safeZone", true);
                
            if (IsSafeZone(index, ZoneName.EMS))
                qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage31, 1, isUpdateHud: true);
                
            if (IsSafeZone(index, ZoneName.lspd))
                qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage33, 1, isUpdateHud: true);
                
            sessionData.InsideSafeZone = index;
        }
        
        [RemoteEvent("outGreenZone")]
        public void OutSafeZone(ExtPlayer player, int index)
        {

            var sessionData = player.GetSessionData();

            if (sessionData == null) return;
            
            if (!GreenZoneDisabled.Contains(index)) 
                Trigger.ClientEvent(player, "safeZone", false);
            
            if (sessionData.InsideSafeZone == index) 
                sessionData.InsideSafeZone = -1;

        }
        [Command(AdminCommands.Szstate)]
        public static void CMD_SZState(ExtPlayer player, int sz)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Szstate)) return;
                
                var state = GreenZoneDisabled.Contains(sz);
                
                ChangeDamageState(sz, state);
                
                if (!state)
                    Trigger.SendToAdmins(2, $"{Chars.Models.ChatColors.StrongOrange}[A] {player.Name} ({player.Value}) выключил возможность драться и получать урон в зоне #{sz}");
                else 
                    Trigger.SendToAdmins(2, $"{Chars.Models.ChatColors.StrongOrange}[A] {player.Name} ({player.Value}) включил возможность драться и получать урон в зоне #{sz}");
                
                if (!state)
                    GreenZoneDisabled.Add(sz);
                else
                    GreenZoneDisabled.Remove(sz);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_SZState Exception: {e.ToString()}");
            }
        }

        public static void ChangeDamageState(int sz, bool state) // Позволяет по ключу открыть или закрыть возможность драться в любой доступной сэйфзоне.
        {
            try
            {
                foreach (ExtPlayer foreachPlayer in NeptuneEvo.Character.Repository.GetPlayers())
                {
                    try
                    {

                        var foreachSessionData = foreachPlayer.GetSessionData();

                        if (foreachSessionData == null) continue;
                        if (foreachSessionData.InsideSafeZone == sz)
                        {
                            if (state) 
                                Trigger.ClientEvent(foreachPlayer, "safeZone", true);
                            else 
                                Trigger.ClientEvent(foreachPlayer, "safeZone", false);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Write($"ChangeDamageState Foreach Exception: {e.ToString()}");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"ChangeDamageState Exception: {e.ToString()}");
            }
        }
    }
}
