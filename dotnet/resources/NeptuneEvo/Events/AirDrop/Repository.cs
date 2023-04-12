using GTANetworkAPI;
using NeptuneEvo.Handles;
using Redage.SDK;
using System;
using System.Collections.Generic;
using NeptuneEvo.Core;
using System.Linq;
using Localization;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Functions;
using NeptuneEvo.Quests;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.Fractions;
using Newtonsoft.Json;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Events.AirDrop.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.Players.Phone.Messages.Models;
using NeptuneEvo.Quests.Models;
using NeptuneEvo.VehicleData.LocalData.Models;

namespace NeptuneEvo.Events.AirDrop
{
    public class Repository : Script
    {
        private static readonly nLog Log = new nLog("Events.AirDrop");

        private static Dictionary<string, TeamData> TeamsInfo = new Dictionary<string, TeamData>();
        private static List<AirDropData> AirDropsData = new List<AirDropData>();
        private static byte AirDropEventStatus = 0;
        private static DateTime AirDropEndEventTime = DateTime.Now;

        private static string AirdropEndTimer = null;

        private static List<Vector3> AirDropsCentralPosition = new List<Vector3>()
        {
            new Vector3(1079.98f, -697.410f, 58.205f), //0
            new Vector3(-1718.506f, -189.346f, 58.246f), //1
            new Vector3(866.1436f, -916.8142f, 26.435665f), //2
            new Vector3(-913.024f, -2527.9153f, 23.3214f), //3 +
            new Vector3(1149.286f, -3273.673f, 5.9f), //4
            new Vector3(-1059.123f, -1028.2855f, 2.1919482f), //5
            new Vector3(-1615.2452f, -3078.2163f, 13.932513f), //6
            new Vector3(-2237.325f, 249.77641f, 176.34125f), //7
            new Vector3(1136.6859f, 16.850683f, 81.89105f) //8 +
        };

        private static IReadOnlyDictionary<int, Vector3[]> AirDropsPosition = new Dictionary<int, Vector3[]>()
        {
           { 
                0,
                new Vector3[] {
                    new Vector3(1070.7666f, -712.1477f, 58.49874f),
                    new Vector3(1102.1672f, -707.12476f, 56.7596f),
                    new Vector3(1075.2537f, -760.1058f, 57.8116f),
                    new Vector3(1019.26447f, -711.14923f, 57.7291f),
                    new Vector3(1111.0593f, -759.1508f, 57.7643f),
                    new Vector3(1124.9475f, -663.4413f, 56.7671f),
                    new Vector3(1159.4922f, -718.4323f, 56.826f),
                    new Vector3(1051.5419f, -619.1648f, 56.876f),
                    new Vector3(1014.0885f, -657.589f, 58.3552f),
                    new Vector3(1147.6125f, -644.389f, 56.741f)
                }
            },
            {
                1,
                new Vector3[] {
                new Vector3(-1726.6642f, -188.69617f, 58.2658f),
                new Vector3(-1714.655f, -235.4602f, 55.067192f),
                new Vector3(-1762.959f, -162.465f, 63.9422f),
                new Vector3(-1688.150f, -164.554f, 57.5533f),
                new Vector3(-1671.486f, -219.45708f, 55.1525f),
                new Vector3(-1731.027f, -262.871f, 51.5888f),
                new Vector3(-1803.443f, -128.03717f, 78.786f),
                new Vector3(-1688.511f, -261.73325f, 51.88331f),
                new Vector3(-1771.507f, -257.61105f, 49.332f),
                new Vector3(-1652.858f, -132.6128f, 59.6817f),  
                }
            },
            {
                2,
                new Vector3[] {
                    new Vector3(863.8128f, -868.2756f, 25.62753f),
                    new Vector3(852.917f, -951.6709f, 26.2712f),
                    new Vector3(901.943f, -887.61664f, 41.7496f),
                    new Vector3(915.0499f, -895.1392f, 53.3129f),
                    new Vector3(892.5187f, -901.8795f, 43.91995f),
                    new Vector3(806.24365f, -919.2496f, 25.84587f),
                    new Vector3(881.6852f, -880.0532f, 27.724f),
                    new Vector3(836.23175f, -875.3281f, 25.22759f),
                    new Vector3(926.2606f, -879.1531f, 50.0126f),
                    new Vector3(880.6848f, -934.20575f, 30.78252f), 
                }
            },
            {
                3,
                new Vector3[] {
                    new Vector3(-879.1289f, -2523.5586f, 14.857651f),
                    new Vector3(-841.520f, -2500.98f, 13.830637f),
                    new Vector3(-882.229f, -2584.5405f, 13.827842f),
                    new Vector3(-956.03723f, -2570.7585f, 13.820424f),
                    new Vector3(-936.0609f, -2548.8445f, 14.015688f),
                    new Vector3(-998.2625f, -2527.4517f, 13.801147f),
                    new Vector3(-967.0045f, -2466.0144f, 13.976596f),
                    new Vector3(-914.09424f, -2484.0857f, 14.539599f),
                    new Vector3(-847.76746f, -2577.6829f, 13.759464f),
                    new Vector3(-995.31024f, -2479.891, 13.778991f),
                }
            },
            {
                4,
                new Vector3[] {
                    new Vector3(1150.489f, -3282.368f, 5.900809f),
                    new Vector3(1119.217f, -3260.793f, 5.897873f),
                    new Vector3(1122.711f, -3233.599f, 5.895298f),
                    new Vector3(1158.365f, -3209.473f, 5.900023f),
                    new Vector3(1184.569f, -3239.953f, 6.028767f),
                    new Vector3(1191.856f, -3342.553f, 5.801401f),
                    new Vector3(1065.062f, -3325.472f, 5.915152f),
                    new Vector3(1077.113f, -3217.388f, 5.901025f),
                    new Vector3(1236.264f, -3217.617f, 5.800362f),
                    new Vector3(1164.095f, -3309.951f, 5.924438f)
                }
            },
            {
                5,
                new Vector3[] {
                    new Vector3(-1034.9161f, -1067.9562f, 3.908407f),
                    new Vector3(-1086.1357f, -983.0408f, 3.9817119f),
                    new Vector3(-1109.1017f, -1008.671f, 2.127022f),
                    new Vector3(-1022.54224f, -1006.8227f, 2.0805154f),
                    new Vector3(-1110.4626f, -1100.8438f, 2.152846f),
                    new Vector3(-1130.5438f, -954.1605f, 6.6246314f),
                    new Vector3(-1052.5557f, -942.2641f, 2.147397f),
                    new Vector3(-1141.8715f, -1075.8899f, 2.6645648f),
                    new Vector3(-1021.09644f, -1035.8823f, 2.1503568f),
                    new Vector3(-1057.9869f, -987.12134, 2.1267402f),
                }
            },
            {
                6,
                new Vector3[] {
                    /*new Vector3(-1633.3508f, -3017.593f, 14.120636f),
                    new Vector3(-1714.258f, -2967.838f, 14.134283f),
                    new Vector3(-1642.5552f, -2911.5105f, 14.2871685f),
                    new Vector3(-1503.3135f, -3000.553f, 14.310736f),
                    new Vector3(-1608.9343f, -3141.3035f, 29.566256f),
                    new Vector3(-1618.0619f, -3136.7424f, 31.928312f),
                    new Vector3(-1644.8013f, -3128.7085f, 35.425087f),
                    new Vector3(-1669.2505f, -3149.8252f, 35.423992f),
                    new Vector3(-1655.9427f, -3154.4634f, 35.427883f),
                    new Vector3(-1678.5167f, -3168.2393f, 35.421173f),*/
                    new Vector3(-1566.7665f, -3095.436f, 13.944708f),
                    new Vector3(-1649.9985f, -3091.158f, 13.937061f),
                    new Vector3(-1626.9152f, -3133.253f, 35.413322f),
                    new Vector3(-1674.7384f, -3154.949f, 35.43172f),
                    new Vector3(-1619.1382f, -3173.4817f, 13.936287f),
                    new Vector3(-1672.0951f, -3105.5173f, 29.561073f),
                    new Vector3(-1694.1156f, -3145.3f, 29.553682f),
                    new Vector3(-1591.3275f, -3084.1577f, 13.9358835f),
                    new Vector3(-1622.9893f, -3061.342f, 14.138118f),
                    new Vector3(-1583.3085f, -3112.012f, 13.93775f),
                }
            },
            {
                7,
                new Vector3[] {
                    new Vector3(-2302.4119f, 218.99484f, 167.60162f),
                    new Vector3(-2241.3008f, 274.7148f, 174.60353f),
                    new Vector3(-2212.8687f, 214.99245f, 174.58456f),
                    new Vector3(-2190.8572f, 236.15755f, 184.60193f),
                    new Vector3(-2209.9026f, 200.59213f, 194.59651f),
                    new Vector3(-2243.3413f, 231.83043f, 190.60155f),
                    new Vector3(-2242.393f, 295.31158f, 184.60014f),
                    new Vector3(-2265.6143f, 310.84082f, 174.22559f),
                    new Vector3(-2261.2908f, 195.84755f, 174.59409f),
                    new Vector3(-2335.6958f, 244.32425f, 169.59949f),
                }
            },
            {
                8,
                new Vector3[] {
                new Vector3(1058.4012f, 45.450047f, 81.52423f),
                new Vector3(1088.6041f, 56.241276f, 80.87874f),
                new Vector3(1203.711f, 89.279045f, 81.83645f),
                new Vector3(1030.8582f, -43.58331f, 75.068405f),
                new Vector3(1024.1509f, 29.952518f, 82.15714f),
                new Vector3(1134.4272f, 11.786752f, 81.88025f),
                new Vector3(1114.106f, -30.436384f, 81.94798f),
                new Vector3(1126.9879f, 83.781425f, 80.75532f),
                new Vector3(1104.3988f, 106.91325f, 80.89076f),
                new Vector3(1161.7772f, 107.060036f, 80.68434f)
            }
        }
        };

        private static int minMinutes = 11;
        private static int maxMinutes = 20;
        private static int notificationMinutes = 10;

        [ServerEvent(Event.ResourceStart)]
        public void Event_ResourceStart()
        {
            PedSystem.Repository.CreateQuest("g_m_m_cartelguards_01", new Vector3(-480.33752, -400.36942, 34.5466), 27.42f, title: "~y~NPC~w~ Juan de cartel\nПродавец AirDrop", colShapeEnums: ColShapeEnums.PedAirDrop);
        }

        public static void StartEvent(bool needStart = false, bool needStop = false)
        {
            try
            {
                var hour = DateTime.Now.Hour;

                //if (hour == 11 || hour == 18 || hour == 0 || isComands)
                if (needStop) 
                    StopAirDropEvent();

                if (hour == 8 || hour == 15 || hour == 21 || needStart)
                {
                    AirDropsData = new List<AirDropData>();
                    AirDropEventStatus = 1;

                    var rand = new Random();

                    var dropCount = 3;
                    var location = rand.Next(0, 9);
                    var dropLocation = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

                    foreach (var foreachPlayer in Character.Repository.GetPlayers())
                    {
                        Trigger.SendChatMessage(foreachPlayer, "~o~" + LangFunc.GetText(LangType.Ru, DataName.AirDropZakazan));
                        EventSys.SendCoolMsg(foreachPlayer,"AirDrop", $"Скоро начнётся!", LangFunc.GetText(LangType.Ru, DataName.AirDropZakazan), "", 10000);

                        if (!IsPlayerToEvents(foreachPlayer)) continue;
                        //Notify.Send(foreachPlayer, NotifyType.Info, NotifyPosition.BottomCenter, $"[AirDrop] В течение 20 минут ожидается сброс {dropCount} " + (dropCount == 1 ? "ящика. " : "ящиков. ") + "Введи /airdrop.", 30000); 
                        //EventSys.SendCoolMsg(foreachPlayer,"AirDrop", $"Внимание!", $"В течение 20 минут ожидается сброс {dropCount} " + (dropCount == 1 ? "ящика. " : "ящиков. ") + "Введи /airdrop, чтобы поставить метку на карте.", "", 10000);//??)
                        Trigger.ClientEvent(foreachPlayer, "phone.notify", (int)DefaultNumber.Informer, $"Гринго!!! В течение 20 минут ожидается сброс ящиков, их будет: {dropCount}. Пиши /airdrop, если туго доходит.", 20);
                    }

                    for (var i = 0; i < dropCount; i++)
                    {
                        var airDropData = new AirDropData(i);

                        var minute = rand.Next(minMinutes, maxMinutes);
                        airDropData.DateTime = DateTime.Now.AddMinutes(minute);
                        airDropData.DateTimeNotification = DateTime.Now.AddMinutes(minute - notificationMinutes);

                        var randLocationNumber = rand.Next(0, dropLocation.Count);
                        airDropData.Position = AirDropsPosition[location][dropLocation[randLocationNumber]];
                        dropLocation.Remove(dropLocation[randLocationNumber]);

                        airDropData.CentralPosition = AirDropsCentralPosition[location];
                        
                        AirDropsData.Add(airDropData);

                        CreateItems($"airdrop_{i}", "airdrop");
                    }

                    if (AirdropEndTimer != null) Timers.Stop(AirdropEndTimer);
                    AirdropEndTimer = Timers.StartOnce("AirdropEndTimer", 50 * 60000, () => StopAirDropEvent(), true);

                    AirDropEndEventTime = DateTime.Now.AddMinutes(50);
                }
            }
            catch (Exception e)
            {
                Log.Write($"StartEvent Exception: {e.ToString()}");
            }
        }

        private static void CreateItems(string locationName, string location)
        {
            var rand = new Random();
            // 100%:
            Chars.Repository.AddNewItem(null, locationName, location, ItemId.Revolver, rand.Next(1, 3), WeaponRepository.GetAirdropSerial(), true, 100);
            Chars.Repository.AddNewItem(null, locationName, location, ItemId.AssaultRifleMk2, rand.Next(1, 3), WeaponRepository.GetAirdropSerial(), true, 100);
            Chars.Repository.AddNewItem(null, locationName, location, ItemId.BodyArmor, rand.Next(20, 40), $"100", true, 100);

            for (int item = 0; item < rand.Next(5, 10); item++)
                Chars.Repository.AddNewItem(null, locationName, location, ItemId.Drugs, 50);
            
            // Рандом:
            int item_chance = rand.Next(1, 101);
            
            if (item_chance >= 80)
            {
                for (int item = 0; item < rand.Next(5, 10); item++)
                {
                    Clothes maskRand = Customization.Masks[rand.Next(0, Customization.Masks.Count)];
                    Chars.Repository.AddNewItem(null, locationName, location, ItemId.Mask, 1, $"{maskRand.Variation}_{maskRand.Colors[0]}_True");
                }
            }
            else if (item_chance >= 70) Chars.Repository.AddNewItem(null, locationName, location, ItemId.APPistol, rand.Next(5, 10), WeaponRepository.GetAirdropSerial(), true, 100);
            else if (item_chance >= 60)
            {
                Chars.Repository.AddNewItem(null, locationName, location, ItemId.SweeperShotgun, rand.Next(1, 3), WeaponRepository.GetAirdropSerial(), true, 100);
                Chars.Repository.AddNewItem(null, locationName, location, ItemId.CarbineRifleMk2, rand.Next(1, 3), WeaponRepository.GetAirdropSerial(), true, 100);
            }
            else if (item_chance >= 50) Chars.Repository.AddNewItem(null, locationName, location, ItemId.Gusenberg, rand.Next(1, 3), WeaponRepository.GetAirdropSerial(), true, 100);
            else if (item_chance >= 40)
            {
                Chars.Repository.AddNewItem(null, locationName, location, ItemId.SniperRifle, rand.Next(1, 5), WeaponRepository.GetAirdropSerial(), true, 100);
                Chars.Repository.AddNewItem(null, locationName, location, ItemId.AssaultShotgun, rand.Next(2, 5), WeaponRepository.GetAirdropSerial(), true, 100);
                Chars.Repository.AddNewItem(null, locationName, location, ItemId.TacticalRifle, rand.Next(2, 4), WeaponRepository.GetAirdropSerial(), true, 100);

            }
            else if (item_chance >= 18)
            {
                if (item_chance >= 30)
                {
                    Chars.Repository.AddNewItem(null, locationName, location, ItemId.MarksmanRifle, rand.Next(1, 3), WeaponRepository.GetAirdropSerial(), true, 100);
                    Chars.Repository.AddNewItem(null, locationName, location, ItemId.Wrench, rand.Next(1, 3), WeaponRepository.GetAirdropSerial(), true, 100);
                }
                
                for (var item = 0; item < rand.Next(10, 20); item++)
                    Chars.Repository.AddNewItem(null, locationName, location, ItemId.Material, 250);

                for (var item = 0; item < rand.Next(10, 30); item++)
                    Chars.Repository.AddNewItem(null, locationName, location, ItemId.HealthKit, 10);
            }
            else if (item_chance >= 15) Chars.Repository.AddNewItem(null, locationName, location, ItemId.CarCoupon, 1, "Phoenix");
            else if (item_chance >= 3)
            {
                Chars.Repository.AddNewItem(null, locationName, location, ItemId.CombatMG, rand.Next(1, 3), WeaponRepository.GetAirdropSerial(), true, 100);
                
                Chars.Repository.AddNewItem(null, locationName, location, ItemId.HeavyShotgun, rand.Next(2, 4), WeaponRepository.GetAirdropSerial(), true, 100);
                Chars.Repository.AddNewItem(null, locationName, location, ItemId.HeavySniper, rand.Next(1, 3), WeaponRepository.GetAirdropSerial(), true, 100);
                
                if (item_chance >= 7)
                    Chars.Repository.AddNewItem(null, locationName, location, ItemId.MarksmanPistol, rand.Next(1, 2), WeaponRepository.GetAirdropSerial(), true, 100);
                
                if (item_chance >= 8 && item_chance < 10) Chars.Repository.AddNewItem(null, locationName, location, ItemId.CarCoupon, 1, "Neon");
                else if (item_chance >= 6 && item_chance < 8) Chars.Repository.AddNewItem(null, locationName, location, ItemId.Mask, rand.Next(1, 2), "197_8_True");
            }
            else if (item_chance == 2) Chars.Repository.AddNewItem(null, locationName, location, ItemId.CarCoupon, 1, "Rapidgt3");
            else if (item_chance == 1) Chars.Repository.AddNewItem(null, locationName, location, ItemId.CarCoupon, 1, "Ferrari488");
        }

        public static void StopAirDropEvent()
        {
            try
            {
                foreach (var airDropData in AirDropsData)
                    Chars.Repository.RemoveAll($"airdrop_{airDropData.DropId}");

                foreach (var foreachPlayer in Character.Repository.GetPlayers())
                {
                    try
                    {
                        if (!IsPlayerToEvents(foreachPlayer)) continue;
                        var foreachSessionData = foreachPlayer.GetSessionData();
                        if (foreachSessionData == null) continue;

                        foreach (var airDropData in AirDropsData)
                            Trigger.ClientEvent(foreachPlayer, "client.fight.dell", airDropData.DropId);

                        Trigger.ClientEvent(foreachPlayer, "client.blipZone.remove");
                        Trigger.ClientEvent(foreachPlayer, "phone.notify", (int) DefaultNumber.Informer, $"Хола! По моим каналам прошла инфа, что AirDrop закончился. Надеюсь, у тебя всё чики-пуки, до новых встреч!", 15);
                        if (foreachPlayer.Dimension == 100)
                        {
                            Trigger.Dimension(foreachPlayer);
                            foreachSessionData.IsInAirDropZone = false;
                            Trigger.ClientEvent(foreachPlayer, "client.AirDrop.isZone", false);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Write($"StopAirDropEvent Players-foreach Exception: {e.ToString()}");
                    }
                }

                AirDropsData.Clear();
                TeamsInfo.Clear();
                AirDropEventStatus = 0;
            }
            catch (Exception e)
            {
                Log.Write($"StopAirDropEvent Exception: {e.ToString()}");
            }
        }

        public static void CreateEvent()
        {
            try
            {
                foreach (var airDropData in AirDropsData)
                {
                    if (airDropData.Status == 2) 
                        continue;
                    
                    if (airDropData.Status == 0 && airDropData.DateTimeNotification > DateTime.Now) 
                        continue;
                    
                    if (airDropData.Status == 1 && airDropData.DateTime > DateTime.Now) 
                        continue;

                    airDropData.Status++;
                    
                    foreach (var foreachPlayer in Character.Repository.GetPlayers())
                    {
                        if (!IsPlayerToEvents(foreachPlayer)) continue;
                        
                        if (airDropData.Status == 1) 
                            Trigger.ClientEvent(foreachPlayer, "client.airdropZone.create", airDropData.CentralPosition, 100, 1, notificationMinutes);
                        else 
                            Trigger.ClientEvent(foreachPlayer, "client.fight.create", airDropData.DropId, 0, airDropData.Position, 2);
                        
                        Trigger.ClientEvent(foreachPlayer, "createWaypoint", airDropData.CentralPosition.X, airDropData.CentralPosition.Y);
                    }

                    if (airDropData.Status == 2 && AirDropEventStatus < 2) 
                        AirDropEventStatus = 2;
                }
            }
            catch (Exception e)
            {
                Log.Write($"CreateEvent Exception: {e.ToString()}");
            }
        }

        public static bool IsPlayerToEvents(ExtPlayer player)
        {
            try
            {
                if (!TeamsInfo.ContainsKey(Manager.GetName(player.GetFractionId())))
                {
                    var organizationName = player.GetOrganizationName();
                    if (organizationName != String.Empty && !TeamsInfo.ContainsKey(organizationName))
                        return false;
                    
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Write($"IsPlayerToEvents Exception: {e.ToString()}");
                return false;
            }
        }

        [RemoteEvent("AirdropChangePlayerDimension")]
        public static void AirdropChangePlayerDimension(ExtPlayer player, int state)
        {
            try
            {
                if (!IsPlayerToEvents(player)) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (state == 1)
                {
                    Trigger.Dimension(player, 100);
                    sessionData.IsInAirDropZone = true;
                    Trigger.ClientEvent(player, "client.AirDrop.isZone", true);
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AirGo), 15000);

                    if (TeamsInfo.ContainsKey(player.GetFractionName())) 
                        TeamsInfo[player.GetFractionName()].TeammatesInZone += 1;
                    else
                    {
                        var organizationName = player.GetOrganizationName();
                        if (organizationName != String.Empty && TeamsInfo.ContainsKey(organizationName))
                            TeamsInfo[organizationName].TeammatesInZone += 1;
                    }

                    foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                    {
                        if (!IsPlayerToEvents(foreachPlayer) || foreachPlayer.Dimension != 100) continue;
                        Trigger.ClientEvent(foreachPlayer, "airdrop.updateTeamsInfo", JsonConvert.SerializeObject(TeamsInfo.OrderBy(u => u.Value.TeamFrags).Reverse()));
                    }
                }
                else if (state == 2)
                {
                    Trigger.Dimension(player);
                    sessionData.IsInAirDropZone = false;
                    Trigger.ClientEvent(player, "client.AirDrop.isZone", false);

                    if (TeamsInfo.ContainsKey(player.GetFractionName())) 
                        TeamsInfo[player.GetFractionName()].TeammatesInZone -= 1;
                    else
                    {
                        var organizationName = player.GetOrganizationName();
                        if (organizationName != String.Empty && TeamsInfo.ContainsKey(organizationName))
                            TeamsInfo[organizationName].TeammatesInZone -= 1;
                    }

                    foreach (var foreachPlayer in Character.Repository.GetPlayers())
                    {
                        if (!IsPlayerToEvents(foreachPlayer) || foreachPlayer.Dimension != 100) continue;
                        Trigger.ClientEvent(foreachPlayer, "airdrop.updateTeamsInfo", JsonConvert.SerializeObject(TeamsInfo.OrderBy(u => u.Value.TeamFrags).Reverse()));
                    }

                    if (AirDropEventStatus > 0)
                    {
                        foreach (var airDropData in AirDropsData)
                        {
                            Trigger.ClientEvent(player, "client.fight.dell", airDropData.DropId);
                            Trigger.ClientEvent(player, "client.blipZone.remove");
                        }

                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LeaveAirTer), 3000);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"AirdropChangePlayerDimension Exception: {e.ToString()}");
            }
        }
        
        /// <summary>
        /// Когда вошел в зону
        /// </summary>
        /// <param name="player"></param>
        /// <param name="ShapeId"></param>

        [RemoteEvent("CheckAirdropLockStatus")]
        public static void CheckAirdropLockStatus(ExtPlayer player, int ShapeId)
        {
            try
            {
                if (!IsPlayerToEvents(player)) return;

                foreach (AirDropData airDropData in AirDropsData)
                {
                    if (airDropData.DropId == ShapeId)
                    {
                        if (airDropData.AirdropLockHealth > 0)
                        {
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PressEtoLockPick), 3000);
                            Trigger.ClientEvent(player, "client.updateAirdropHackStatus", true, airDropData.AirdropLockHealth);
                        }
                        else Trigger.ClientEvent(player, "client.updateAirdropHackStatus", false, 0);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"CheckAirdropLockStatus Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("AirdropChangeLockStatus")]
        public static void AirdropChangeLockStatus(ExtPlayer player, int ShapeId, int health)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!IsPlayerToEvents(player) || !sessionData.IsAirDropHacking) return;
                Trigger.StopAnimation(player);
                foreach (AirDropData airDropData in AirDropsData)
                {
                    if (airDropData.DropId == ShapeId)
                    {
                        airDropData.AirdropLockHealth = health;
                        airDropData.AirdropHackPlayerInfo = null;
                        sessionData.IsAirDropHacking = false;

                        if (airDropData.AirdropLockHealth <= 0)
                        {
                            int hack_chance = new Random().Next(1, 101);

                            if (hack_chance <= 50)
                            {
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucZamok), 3000);
                                var position = airDropData.Position;
                                ParticleFx.PlayFXonPos(position, 500f, position.X, position.Y, position.Z, "scr_indep_fireworks", "scr_indep_firework_shotburst", 5000);
                                Trigger.ClientEventInRange(position, 2.5f, "client.updateAirdropHackStatus", false, 0);
                            }
                            else
                            {
                                airDropData.AirdropLockHealth = 20;
                                Trigger.ClientEvent(player, "client.updateAirdropHackStatus", true, airDropData.AirdropLockHealth);
                                Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FailZamok), 3000);
                            }

                            ItemStruct armylockpick = Chars.Repository.isItem(player, "inventory", ItemId.ArmyLockpick);
                            int count = (armylockpick == null) ? 0 : armylockpick.Item.Count;
                            if (count > 0) Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", ItemId.ArmyLockpick, 1);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"AirdropChangeLockStatus Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.fight.open")]
        public static void OpenInventory(ExtPlayer player, int ShapeId)
        {
            try
            {
                if (!IsPlayerToEvents(player)) return;

                foreach (AirDropData airDropData in AirDropsData)
                {
                    if (airDropData.DropId == ShapeId)
                    {
                        if (airDropData.AirdropLockHealth > 0)
                        {
                            if (airDropData.AirdropHackPlayerInfo != null)
                            {
                                var targetSessionData = airDropData.AirdropHackPlayerInfo.GetSessionData();
                                if (targetSessionData == null || !targetSessionData.IsAirDropHacking) airDropData.AirdropHackPlayerInfo = null;
                                else if (airDropData.Position.DistanceTo(airDropData.AirdropHackPlayerInfo.Position) > 3) airDropData.AirdropHackPlayerInfo = null;
                                else
                                {
                                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomebodyHacking), 3000);
                                    return;
                                }
                            }
                                
                            Trigger.ClientEvent(player, "client.updateAirdropHackStatus", true, airDropData.AirdropLockHealth);
                        }
                        else
                        {
                            Trigger.ClientEvent(player, "client.updateAirdropHackStatus", false, 0);
                            Chars.Repository.LoadOtherItemsData(player, "airdrop", ShapeId.ToString(), 10);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"OpenInventory Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.fight.player.start.hack")]
        public static void StartedHack(ExtPlayer player, int ShapeId)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!IsPlayerToEvents(player)) return;

                if (!FunctionsAccess.IsWorking("AirDrop_StartedHack"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }

                if (characterData.AdminLVL >= 1 && characterData.AdminLVL <= 5)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RestrictedForAdmin), 3000);
                    return;
                }

                ItemStruct armylockpick = Chars.Repository.isItem(player, "inventory", ItemId.ArmyLockpick);
                int count = (armylockpick == null) ? 0 : armylockpick.Item.Count;

                if (count == 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoArmyLockpick), 3000);
                    return;
                }

                foreach (AirDropData airDropData in AirDropsData)
                {
                    if (airDropData.DropId == ShapeId)
                    {
                        if (airDropData.AirdropLockHealth > 0)
                        {
                            if (airDropData.AirdropHackPlayerInfo != null)
                            {
                                var targetSessionData = airDropData.AirdropHackPlayerInfo.GetSessionData();
                                if (targetSessionData == null || !targetSessionData.IsAirDropHacking) airDropData.AirdropHackPlayerInfo = null;
                                else if (airDropData.Position.DistanceTo(airDropData.AirdropHackPlayerInfo.Position) > 3) airDropData.AirdropHackPlayerInfo = null;
                                else
                                {
                                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomebodyHacking), 3000);
                                    return;
                                }
                            }

                            Trigger.PlayAnimation(player, "mp_weapons_deal_sting", "crackhead_bag_loop", 39);
                            // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "airdropvzlom");
                            Trigger.ClientEvent(player, "airdrop_hackStatus", 1);

                            airDropData.AirdropHackPlayerInfo = player;
                            sessionData.IsAirDropHacking = true;
                        }
                        else
                        {
                            Trigger.ClientEvent(player, "client.updateAirdropHackStatus", false, 0);
                            Chars.Repository.LoadOtherItemsData(player, "airdrop", ShapeId.ToString(), 10);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"StartedHack Exception: {e.ToString()}");
            }
        }

        [Interaction(ColShapeEnums.PedAirDrop)]
        public static void Open(ExtPlayer player, int index)
        {
            try
            {
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
                if (!FunctionsAccess.IsWorking("PedAirDrop_Open"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }

                if (player.IsFractionMemberData())
                {
                    int random_dialog = new Random().Next(0, 2);
                    player.SelectQuest(new PlayerQuestModel("npc_airdrop", random_dialog, 0, false, DateTime.Now));
                    Trigger.ClientEvent(player, "client.quest.open", index, "npc_airdrop", random_dialog, 0, 0);
                }
                else
                {
                    if (player.IsOrganizationMemberData())
                    {
                        int random_dialog = new Random().Next(0, 2);
                        player.SelectQuest(new PlayerQuestModel("npc_airdrop", random_dialog, 0, false, DateTime.Now));
                        Trigger.ClientEvent(player, "client.quest.open", index, "npc_airdrop", random_dialog, 0, 0);
                    }
                    else
                    {
                        player.SelectQuest(new PlayerQuestModel("npc_airdrop", -1, 0, false, DateTime.Now));
                        Trigger.ClientEvent(player, "client.quest.open", index, "npc_airdrop", -1, 0, 0);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"Open Exception: {e.ToString()}");
            }
        }
        public static void BuyInfo(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                else if (!FunctionsAccess.IsWorking("PedAirDrop_BuyInfo"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                else if (IsPlayerToEvents(player))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AirdropBuyed), 3000);
                    return;
                }
                else if (AirDropEventStatus == 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AirdropAlready), 3000);
                    return;
                }
                else if (UpdateData.CanIChange(player, Main.PricesSettings.AirdropInfoPrice, true) != 255) return;

                if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour <= 23 || DateTime.Now.Hour == 0 && DateTime.Now.Minute <= 10)
                    Trigger.ClientEvent(player, "openDialog", "PAY_AIRDROP_INFO", LangFunc.GetText(LangType.Ru, DataName.AidropOffer, Main.PricesSettings.AirdropInfoPrice));
                else
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AidropTime), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"Perform Exception: {e.ToString()}");
            }
        }
        public static void Perform(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                else if (!FunctionsAccess.IsWorking("PedAirDrop_Perform"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                else if (UpdateData.CanIChange(player, Main.PricesSettings.AirdropOrderPrice, true) != 255) return;
                else if (AirDropEventStatus > 0)
                {
                    string nextAirDropTime = "Следующий AirDrop можно будет заказать в " + (AirDropEndEventTime.Hour < 10 ? $"0{AirDropEndEventTime.Hour}" : $"{AirDropEndEventTime.Hour}") + ":" + (AirDropEndEventTime.Minute < 10 ? $"0{AirDropEndEventTime.Minute}" : $"{AirDropEndEventTime.Minute}");
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AirdropWaiting, nextAirDropTime), 3000);
                    return;
                }

                if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour <= 23)
                    Trigger.ClientEvent(player, "openDialog", "PAY_AIRDROP_ORDER", LangFunc.GetText(LangType.Ru, DataName.AirdropBuyingg, Main.PricesSettings.AirdropOrderPrice));
                else
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AidropTime), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"Perform Exception: {e.ToString()}");
            }
        }

        public static void PayAirdropInfo(ExtPlayer player)
        {
            try
            {
                if (!FunctionsAccess.IsWorking("PedAirDrop_BuyInfo"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                else if (IsPlayerToEvents(player))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AirdropBuyed), 3000);
                    return;
                }
                else if (AirDropEventStatus == 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AirdropAlready), 3000);
                    return;
                }
                else if (UpdateData.CanIChange(player, Main.PricesSettings.AirdropInfoPrice, true) != 255) return;

                if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour <= 23 || DateTime.Now.Hour == 0 && DateTime.Now.Minute <= 10)
                {
                    var fracId = player.GetFractionId();
                    if (fracId > (int)Fractions.Models.Fractions.None)
                    {
                        TeamsInfo.Add(player.GetFractionName(), new TeamData(0, 0));

                        foreach (AirDropData airDropData in AirDropsData)
                        {
                            if ((airDropData.Status == 0 || airDropData.Status == 1) && DateTime.Now.AddMinutes(10) > airDropData.DateTime)
                            {
                                foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                                {
                                    var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                                    if (foreachMemberFractionData == null) 
                                        continue;
                                    
                                    if (foreachMemberFractionData.Id == fracId)
                                    {
                                        Trigger.ClientEvent(foreachPlayer, "client.airdropZone.create", airDropData.CentralPosition, 100, 1, notificationMinutes);
                                        Trigger.ClientEvent(foreachPlayer, "createWaypoint", airDropData.CentralPosition.X, airDropData.CentralPosition.Y);
                                    }
                                }
                            }

                            if (airDropData.Status == 1 && AirDropEventStatus < 2) AirDropEventStatus = 2;
                        }
                    }
                    else
                    {
                        var organizationData = player.GetOrganizationData();
                        if (organizationData != null) 
                        {
                            TeamsInfo[organizationData.Name] = new TeamData(0, 0);

                            foreach (AirDropData airDropData in AirDropsData)
                            {
                                if ((airDropData.Status == 0 || airDropData.Status == 1) && DateTime.Now.AddMinutes(10) > airDropData.DateTime)
                                {
                                    foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                                    {
                                        var foreachMemberOrganizationData = foreachPlayer.GetOrganizationMemberData();
                                        if (foreachMemberOrganizationData == null) continue;
                                        if (foreachMemberOrganizationData.Id == organizationData.Id)
                                        {
                                            Trigger.ClientEvent(foreachPlayer, "client.airdropZone.create", airDropData.CentralPosition, 100, 1, notificationMinutes);
                                            Trigger.ClientEvent(foreachPlayer, "createWaypoint", airDropData.CentralPosition.X, airDropData.CentralPosition.Y);
                                        }
                                    }
                                }

                                if (airDropData.Status == 1 && AirDropEventStatus < 2) AirDropEventStatus = 2;
                            }
                        }
                        else
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AirdropSub), 3000);
                            return;
                        }
                    }

                    Wallet.Change(player, -Main.PricesSettings.AirdropInfoPrice);
                    GameLog.Money($"player({characterData.UUID})", $"server", Main.PricesSettings.AirdropInfoPrice, $"buyAirDropInfo");

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AirdropSubbed), 3000);

                    if (AirDropEventStatus == 1)
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AidropFractions), 10000);
                }
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AidropTime), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"Perform Exception: {e.ToString()}");
            }
        }
        public static void PayAirdropOrder(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                else if (!FunctionsAccess.IsWorking("PedAirDrop_Perform"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                else if (UpdateData.CanIChange(player, Main.PricesSettings.AirdropOrderPrice, true) != 255) return;
                else if (AirDropEventStatus > 0)
                {
                    string nextAirDropTime = "Следующий AirDrop можно будет заказать в " + (AirDropEndEventTime.Hour < 10 ? $"0{AirDropEndEventTime.Hour}" : $"{AirDropEndEventTime.Hour}") + ":" + (AirDropEndEventTime.Minute < 10 ? $"0{AirDropEndEventTime.Minute}" : $"{AirDropEndEventTime.Minute}");
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AirdropWaiting, nextAirDropTime), 3000);
                    return;
                }

                if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour <= 23)
                {
                    if (!IsPlayerToEvents(player))
                    {
                        var fractionName = player.GetFractionName();
                        if (fractionName != String.Empty)
                        {
                            TeamsInfo.Add(fractionName, new TeamData(0, 0));
                        }
                        else
                        {
                            var organizationName = player.GetOrganizationName();
                            if (organizationName != String.Empty)
                            {
                                TeamsInfo[organizationName] = new TeamData(0, 0);
                            }
                            else
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AirdropSub), 3000);
                                return;
                            }
                        }
                    }

                    Wallet.Change(player, -Main.PricesSettings.AirdropOrderPrice);
                    GameLog.Money($"player({characterData.UUID})", $"server", Main.PricesSettings.AirdropOrderPrice, $"buyAirDropOrder");

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AirdropZakazan), 3000);
                    
                    //Trigger.ClientEvent(player, "phone.notify", 99999999, $"Прррррриветик! Спасибо за заказ! Подготовлю для тебя AirDrop в лучшем виде. Скоро дам инфу, пока просто жди :innocent:", 13);

                    StartEvent(true, false);
                }
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AidropTime), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"Perform Exception: {e.ToString()}");
            }
        }

        public static void AirDropDeathHandler(ExtPlayer player, ExtPlayer entityKiller)
        {
            try
            {
                if (!IsPlayerToEvents(player) || !IsPlayerToEvents(entityKiller) || player.Dimension != 100 || entityKiller.Dimension != 100) return;
                
                var fracId = player.GetFractionId();
                var targetFracId = entityKiller.GetFractionId();
                if (fracId != 0 && fracId == targetFracId) return;
                
                var memberOrganizationData = player.GetOrganizationMemberData();
                
                if (memberOrganizationData != null)
                {
                    var killerMemberOrganizationData = entityKiller.GetOrganizationMemberData();
                    
                    if (killerMemberOrganizationData != null && memberOrganizationData.Id == killerMemberOrganizationData.Id)
                        return;
                }

                if (TeamsInfo.ContainsKey(Manager.GetName(targetFracId))) 
                    TeamsInfo[Manager.GetName(targetFracId)].TeamFrags += 1;
                else if (memberOrganizationData != null)
                {
                    var organizationData = Organizations.Manager.GetOrganizationData(memberOrganizationData.Id);
                    if (organizationData != null && TeamsInfo.ContainsKey(organizationData.Name))
                        TeamsInfo[organizationData.Name].TeamFrags += 1;
                }

                foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                {
                    if (!IsPlayerToEvents(foreachPlayer) || foreachPlayer.Dimension != 100) continue;
                    Trigger.ClientEvent(foreachPlayer, "airdrop.updateTeamsInfo", JsonConvert.SerializeObject(TeamsInfo.OrderBy(u => u.Value.TeamFrags).Reverse()));
                }
            }
            catch (Exception e)
            {
                Log.Write($"AirDropDeathHandler Exception: {e.ToString()}");
            }
        }

        public static void onPlayerDisconnectedhandler(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!IsPlayerToEvents(player) || player.Dimension != 100) return;

                if (TeamsInfo.ContainsKey(player.GetFractionName())) 
                    TeamsInfo[player.GetFractionName()].TeammatesInZone -= 1;
                else
                {
                    var organizationName = player.GetOrganizationName();
                    if (organizationName != String.Empty && TeamsInfo.ContainsKey(organizationName)) 
                        TeamsInfo[organizationName].TeammatesInZone -= 1;
                }

                foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                {
                    if (!IsPlayerToEvents(foreachPlayer) || foreachPlayer.Dimension != 100) continue;
                    Trigger.ClientEvent(foreachPlayer, "airdrop.updateTeamsInfo", JsonConvert.SerializeObject(TeamsInfo.OrderBy(u => u.Value.TeamFrags).Reverse()));
                }

                Trigger.Dimension(player);
            }
            catch (Exception e)
            {
                Log.Write($"onPlayerDisconnectedhandler Exception: {e.ToString()}");
            }
        }
        
        
        [Command("t2")]
        public static void CMD_t2(ExtPlayer player)
        {
            try
            {
                if (Main.ServerNumber != 0) return;
                var vehicle = player.Vehicle;
                if (vehicle == null)
                    return;
                Sounds.PlayPlayer3d(vehicle, "sounds/pd.ogg", new SoundData
                {
                    maxDistance = 35,
                    volume = 1,
                }, range: 250f);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_t1 Exception: {e.ToString()}");
            }
        }
        
        
        //Trigger.ClientEvent(foreachPlayer, "client.fight.create", AirDropData.DropId, 0, AirDropData.Position, 2);
        [Command("airlist")]
        public static void CMD_airlist(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.AdminLVL < 1) return;

                if (AirDropEventStatus == 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AirNeZapushen), 3000);
                    return;
                }

                Trigger.SendChatMessage(player, "=== AIRLIST ===");

                foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                {
                    var foreachSessionData = foreachPlayer.GetSessionData();
                    if (foreachSessionData == null) continue;
                    if (!IsPlayerToEvents(foreachPlayer) || !foreachSessionData.IsInAirDropZone) continue;
                    Trigger.SendChatMessage(player, $"[{foreachPlayer.Value}] {foreachPlayer.Name}");
                }

                Trigger.SendChatMessage(player, "=== AIRLIST ===");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_airlist Exception: {e.ToString()}");
            }
        }
        [Command("starttestairdrop")]
        public static void CMD_starttestairdrop(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.AdminLVL < 8) return;
                StartEvent(true, true);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Adminstopaird), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_starttestairdrop Exception: {e.ToString()}");
            }
        }
        [Command("stopairdrop")]
        public static void CMD_stopairdrop(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.AdminLVL < 8) return;
                StopAirDropEvent();
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Adminstopped), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_stopairdrop Exception: {e.ToString()}");
            }
        }
        [Command("airdrop")]
        public static void CMD_airdrop(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                if (!IsPlayerToEvents(player))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoUchastnik), 3000);
                    return;
                }

                List<DateTime> airDropsTimeInfo = new List<DateTime>();

                foreach (AirDropData airDropData in AirDropsData)
                    airDropsTimeInfo.Add(airDropData.DateTime);

                //airDropsTimeInfo.Sort((x, y) => x.Date.CompareTo(y.Date));
                airDropsTimeInfo = airDropsTimeInfo.OrderBy(x => x.TimeOfDay).ToList();

                if (airDropsTimeInfo.Count > 0)
                {
                    Trigger.SendChatMessage(player, "=== AIRDROP TIME ===");

                    for (int i = 0; i < airDropsTimeInfo.Count; i++)
                        Trigger.SendChatMessage(player, $"№{i + 1} | " + (airDropsTimeInfo[i].Hour < 10 ? $"0{airDropsTimeInfo[i].Hour}" : $"{airDropsTimeInfo[i].Hour}") + ":" + (airDropsTimeInfo[i].Minute < 10 ? $"0{airDropsTimeInfo[i].Minute}" : $"{airDropsTimeInfo[i].Minute}"));

                    Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.Air1));
                    Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.Air2));
                    Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.Air3));

                    Trigger.SendChatMessage(player, "=== AIRDROP TIME ===");
                }
                else Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoInfo), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_airdrop Exception: {e.ToString()}");
            }
        }
    }
}