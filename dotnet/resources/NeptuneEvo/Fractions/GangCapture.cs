using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Database;
using GTANetworkAPI;
using LinqToDB;
using Localization;
using NeptuneEvo.Handles;
using NeptuneEvo.Core;
using Redage.SDK;
using MySqlConnector;
using NeptuneEvo.Chars;
using NeptuneEvo.Functions;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Table.Models;
using NeptuneEvo.Table.Tasks.Models;
using NeptuneEvo.Table.Tasks.Player;
using NeptuneEvo.World.War.Models;
using Newtonsoft.Json;

namespace NeptuneEvo.Fractions
{
    class GangsCapture : Script
    {
        private static readonly nLog Log = new nLog("Fractions.GangCapture");
        
        public static Dictionary<int, GangPoint> GangPoints = new Dictionary<int, GangPoint>();
        
        public static Dictionary<int, int> gangPointsColor = new Dictionary<int, int>
        {
            { (int) Models.Fractions.FAMILY, 52 }, // families
            { (int) Models.Fractions.BALLAS, 58 }, // ballas
            { (int) Models.Fractions.VAGOS, 70 }, // vagos
            { (int) Models.Fractions.MARABUNTA, 77 }, // marabunta
            { (int) Models.Fractions.BLOOD, 59 }, // blood street
        };
        
        private static Dictionary<int, DateTime> nextCaptDate = new Dictionary<int, DateTime>
        {
            { (int) Models.Fractions.FAMILY, DateTime.Now },
            { (int) Models.Fractions.BALLAS, DateTime.Now },
            { (int) Models.Fractions.VAGOS, DateTime.Now },
            { (int) Models.Fractions.MARABUNTA, DateTime.Now },
            { (int) Models.Fractions.BLOOD, DateTime.Now },
        };
        
        private static Dictionary<int, DateTime> protectDate = new Dictionary<int, DateTime>
        {
            { (int) Models.Fractions.FAMILY, DateTime.Now },
            { (int) Models.Fractions.BALLAS, DateTime.Now },
            { (int) Models.Fractions.VAGOS, DateTime.Now },
            { (int) Models.Fractions.MARABUNTA, DateTime.Now },
            { (int) Models.Fractions.BLOOD, DateTime.Now },
        };
        
        public static Vector3[] gangZones = new Vector3[90]
        {
            // left side
            new Vector3(-200.8397, -1431.556, 30.18104),
            new Vector3(-100.8397, -1431.556, 30.18104),
            new Vector3(-0.8397, -1431.556, 30.18104),
            new Vector3(99.1603, -1431.556, 30.18104),
            new Vector3(-200.8397, -1531.556, 30.18104),
            new Vector3(-100.8397, -1531.556, 30.18104),
            new Vector3(-0.8397, -1531.556, 30.18104),
            new Vector3(99.1603, -1531.556, 30.18104),
            new Vector3(199.1603, -1531.556, 30.18104),
            new Vector3(299.1603, -1531.556, 30.18104),
            new Vector3(399.1603, -1531.556, 30.18104),
            new Vector3(499.1603, -1531.556, 30.18104),
            new Vector3(-200.8397, -1631.556, 30.18104),
            new Vector3(-100.8397, -1631.556, 30.18104),
            new Vector3(-0.8397, -1631.556, 30.18104),
            new Vector3(99.1603, -1631.556, 30.18104),
            new Vector3(199.1603, -1631.556, 30.18104),
            new Vector3(299.1603, -1631.556, 30.18104),
            new Vector3(399.1603, -1631.556, 30.18104),
            new Vector3(499.1603, -1631.556, 30.18104),
            new Vector3(599.1603, -1631.556, 30.18104),
            new Vector3(-100.8397, -1731.556, 30.18104),
            new Vector3(-0.8397, -1731.556, 30.18104),
            new Vector3(99.1603, -1731.556, 30.18104),
            new Vector3(199.1603, -1731.556, 30.18104),
            new Vector3(299.1603, -1731.556, 30.18104),
            new Vector3(399.1603, -1731.556, 30.18104),
            new Vector3(499.1603, -1731.556, 30.18104),
            new Vector3(599.1603, -1731.556, 30.18104),
            new Vector3(-0.8397, -1831.556, 30.18104),
            new Vector3(99.1603, -1831.556, 30.18104),
            new Vector3(199.1603, -1831.556, 30.18104),
            new Vector3(299.1603, -1831.556, 30.18104),
            new Vector3(399.1603, -1831.556, 30.18104),
            new Vector3(499.1603, -1831.556, 30.18104),
            new Vector3(599.1603, -1831.556, 30.18104),
            new Vector3(99.1603, -1931.556, 30.18104),
            new Vector3(199.1603, -1931.556, 30.18104),
            new Vector3(299.1603, -1931.556, 30.18104),
            new Vector3(399.1603, -1931.556, 30.18104),
            new Vector3(499.1603, -1931.556, 30.18104),
            new Vector3(599.1603, -1931.556, 30.18104),
            new Vector3(199.1603, -2031.556, 30.18104),
            new Vector3(299.1603, -2031.556, 30.18104),
            new Vector3(399.1603, -2031.556, 30.18104),
            new Vector3(499.1603, -2031.556, 30.18104),
            new Vector3(299.1603, -2131.556, 30.18104),
            new Vector3(399.1603, -2131.556, 30.18104),
            new Vector3(768.8984, -2401.556, 28.17772), //right side
            new Vector3(868.8984, -2401.556, 28.17772),
            new Vector3(968.8984, -2401.556, 28.17772),
            new Vector3(1068.8984, -2401.556, 28.17772),
            new Vector3(768.8984, -2301.556, 28.17772),
            new Vector3(868.8984, -2301.556, 28.17772),
            new Vector3(968.8984, -2301.556, 28.17772),
            new Vector3(1068.8984, -2301.556, 28.17772),
            new Vector3(768.8984, -2201.556, 28.17772),
            new Vector3(868.8984, -2201.556, 28.17772),
            new Vector3(968.8984, -2201.556, 28.17772),
            new Vector3(1068.8984, -2201.556, 28.17772),
            new Vector3(768.8984, -2101.556, 28.17772),
            new Vector3(868.8984, -2101.556, 28.17772),
            new Vector3(968.8984, -2101.556, 28.17772),
            new Vector3(1068.8984, -2101.556, 28.17772),
            new Vector3(768.8984, -2001.556, 28.17772),
            new Vector3(868.8984, -2001.556, 28.17772),
            new Vector3(968.8984, -2001.556, 28.17772),
            new Vector3(1068.8984, -2001.556, 28.17772),
            new Vector3(768.8984, -1901.556, 28.17772),
            new Vector3(868.8984, -1901.556, 28.17772),
            new Vector3(968.8984, -1901.556, 28.17772), 
            new Vector3(1068.8984, -1901.556, 28.17772),
            new Vector3(768.8984, -1801.556, 28.17772),
            new Vector3(868.8984, -1801.556, 28.17772),
            new Vector3(968.8984, -1801.556, 28.17772),
            new Vector3(1168.8984, -1801.556, 28.17772),
            new Vector3(1268.8984, -1801.556, 28.17772),
            new Vector3(768.8984, -1701.556, 28.17772),
            new Vector3(868.8984, -1701.556, 28.17772),
            new Vector3(968.8984, -1701.556, 28.17772),
            new Vector3(1168.8984, -1701.556, 28.17772),
            new Vector3(1268.8984, -1701.556, 28.17772),
            new Vector3(1368.8984, -1701.556, 28.17772),
            new Vector3(768.8984, -1601.556, 28.17772),
            new Vector3(868.8984, -1601.556, 28.17772),
            new Vector3(1168.8984, -1601.556, 28.17772),
            new Vector3(1268.8984, -1601.556, 28.17772),
            new Vector3(1368.8984, -1601.556, 28.17772),
            new Vector3(1268.8984, -1501.556, 28.17772),
            new Vector3(1368.8984, -1501.556, 28.17772),
        };

        public static Vector3[] tpCaptureCoords = new Vector3[90]
        {
            new Vector3(-200.8397, -1431.556, 31.58104),
            new Vector3(-92.435104, -1420.9907, 29.644262),
            new Vector3(-5.3002477, -1412.8955, 29.287712),
            new Vector3(99.1603, -1431.556, 30.18104),
            new Vector3(-200.8397, -1531.556, 33.78104),
            new Vector3(-100.8397, -1531.556, 33.98104),
            new Vector3(-0.8397, -1531.556, 30.18104),
            new Vector3(99.1603, -1531.556, 30.18104),
            new Vector3(223.01283, -1515.8212, 29.291681),
            new Vector3(299.1603, -1531.556, 30.18104),
            new Vector3(399.1603, -1531.556, 30.18104),
            new Vector3(502.19296, -1536.3683, 29.248728),
            new Vector3(-200.77438, -1633.441, 33.6612),
            new Vector3(-86.67318, -1655.2671, 29.33629),
            new Vector3(-0.8397, -1631.556, 30.18104),
            new Vector3(99.1603, -1631.556, 30.18104),
            new Vector3(199.1603, -1631.556, 30.18104),
            new Vector3(300.8472, -1632.7241, 32.516785),
            new Vector3(399.1603, -1631.556, 30.18104),
            new Vector3(497.1187, -1630.1417, 29.375402),
            new Vector3(599.1603, -1631.556, 25.18104),
            new Vector3(-100.8397, -1731.556, 30.18104),
            new Vector3(-0.8397, -1731.556, 30.18104),
            new Vector3(99.1603, -1731.556, 30.18104),
            new Vector3(199.76115, -1733.3491, 29.16118),
            new Vector3(299.1603, -1731.556, 30.18104),
            new Vector3(399.1603, -1731.556, 30.18104),
            new Vector3(499.1603, -1731.556, 30.18104),
            new Vector3(599.1603, -1731.556, 30.18104),
            new Vector3(-0.8397, -1831.556, 25.18104),
            new Vector3(99.1603, -1831.556, 26.18104),
            new Vector3(193.4253, -1845.384, 27.146263),
            new Vector3(299.1603, -1831.556, 27.18104),
            new Vector3(395.85574, -1829.1324, 28.229649),
            new Vector3(492.4637, -1838.8887, 27.49535),
            new Vector3(599.1603, -1831.556, 25.18104),
            new Vector3(99.1603, -1931.556, 21.18104),
            new Vector3(199.1603, -1931.556, 22.18104),
            new Vector3(299.1603, -1931.556, 26.18104),
            new Vector3(393.26226, -1936.4803, 24.56807),
            new Vector3(488.4064, -1923.7424, 25.242794),
            new Vector3(599.1603, -1931.556, 21.18104),
            new Vector3(199.1603, -2031.556, 19.18104),
            new Vector3(291.71225, -2021.907, 19.655933),
            new Vector3(405.5024, -2037.1438, 22.370207),
            new Vector3(499.1603, -2031.556, 26.18104),
            new Vector3(299.1603, -2131.556, 16.18104),
            new Vector3(399.1603, -2131.556, 19.18104),
            new Vector3(768.8984, -2401.556, 21.17772),
            new Vector3(894.2473, -2408.1538, 29.385946),
            new Vector3(976.2924, -2405.001, 30.509533),
            new Vector3(1048.0906, -2394.8113, 30.183367),
            new Vector3(768.8984, -2301.556, 28.17772),
            new Vector3(855.897, -2306.1804, 30.345802),
            new Vector3(926.09955, -2301.0046, 30.516901),
            new Vector3(1053.4723, -2306.1409, 30.590824),
            new Vector3(770.6354, -2201.574, 29.140207),
            new Vector3(868.8984, -2201.556, 28.17772),
            new Vector3(968.8984, -2201.556, 31.17772),
            new Vector3(1068.8984, -2201.556, 31.17772),
            new Vector3(768.8984, -2101.556, 30.17772),
            new Vector3(868.8984, -2101.556, 31.17772),
            new Vector3(967.56067, -2099.8726, 30.833656),
            new Vector3(1068.8984, -2101.556, 33.17772),
            new Vector3(768.8984, -2001.556, 30.17772),
            new Vector3(868.8984, -2001.556, 31.17772),
            new Vector3(949.22186, -2000.7246, 30.115568),
            new Vector3(1060.9913, -2002.5533, 31.016212),
            new Vector3(768.8984, -1901.556, 30.17772),
            new Vector3(881.3785, -1906.5698, 30.655567),
            new Vector3(969.41516, -1894.0066, 31.144627),
            new Vector3(1068.8984, -1901.556, 32.17772),
            new Vector3(793.1398, -1807.5277, 29.203436),
            new Vector3(868.8984, -1801.556, 30.17772),
            new Vector3(960.30194, -1801.2676, 31.06433),
            new Vector3(1168.8984, -1801.556, 37.17772),
            new Vector3(1268.8984, -1801.556, 43.17772),
            new Vector3(770.3192, -1726.1405, 29.48708),
            new Vector3(853.3811, -1704.9241, 29.25864),
            new Vector3(968.8984, -1701.556, 30.17772),
            new Vector3(1168.8984, -1701.556, 36.17772),
            new Vector3(1268.8984, -1701.556, 56.17772),
            new Vector3(1368.8984, -1701.556, 62.17772),
            new Vector3(770.2724, -1603.4686, 31.10974),
            new Vector3(868.8984, -1601.556, 31.17772),
            new Vector3(1168.8984, -1601.556, 35.17772),
            new Vector3(1268.8984, -1601.556, 54.17772),
            new Vector3(1368.8984, -1601.556, 54.17772),
            new Vector3(1268.8984, -1501.556, 40.17772),
            new Vector3(1368.8984, -1501.556, 58.17772),
        };

        private static float Range = 150f;
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            try
            {
                using MySqlCommand cmd = new MySqlCommand()
                {
                    CommandText = "SELECT * FROM `gangspoints`"
                };
                
                using DataTable result = MySQL.QueryRead(cmd);
                if (result == null || result.Rows.Count == 0) return;
                
                foreach (DataRow Row in result.Rows)
                {
                    var region = new GangPoint();
                    
                    region.ID = Convert.ToInt32(Row["id"]);
                    region.GangOwner = Convert.ToInt32(Row["gangid"]);
                    
                    if (region.ID >= 90) break;
                    
                    GangPoints.Add(region.ID, region);
                }

                foreach (var region in GangPoints.Values)
                {
                    CustomColShape.Create2DColShape(gangZones[region.ID].X, gangZones[region.ID].Y, 100, 100, 0, ColShapeEnums.GangZone, region.ID);
                    CustomColShape.CreateSphereColShape(gangZones[region.ID], Range, NAPI.GlobalDimension, ColShapeEnums.WarGangZone, region.ID);
                }
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }

        public static void CMD_startCapture(ExtPlayer player)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.Capture)) return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                if (characterData.IsBannedCrime == true)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вам ограничили доступ к системам capture и bizwar. Причина: {characterData.BanCrimeReason}.", 5000);
                    return;
                }
                
                var shapeData = CustomColShape.GetData(player, ColShapeEnums.GangZone);
                if (shapeData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouAreNotInRegion), 3000);
                    return;
                }
                
                switch (shapeData.Index)
                {
                    case 11:
                    case 12:
                    case 36:
                    case 57:
                    case 89:
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DontClaimTitulRegion), 3000);
                        return;
                }
                

                var fracId = player.GetFractionId();
                var region = GangPoints[shapeData.Index];
                if (region.GangOwner == fracId)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantClaimYourRegion), 3000);
                    return;
                }
                
                if (Main.ServerNumber != 0 && (DateTime.Now.Hour < 13 || DateTime.Now.Hour >= 23))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TimeYouCanClaimRegion), 3000);
                    return;
                }
                
                if (Main.ServerNumber != 0 && DateTime.Now < nextCaptDate[fracId])
                {
                    long ticks = nextCaptDate[fracId].Ticks - DateTime.Now.Ticks;
                    if (ticks <= 0) return;
                    
                    DateTime g = new DateTime(ticks);
                    
                    if (g.Minute >= 5)
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCanClaim1h, g.Hour, g.Minute, g.Second), 3000);
                    else
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCanClaim,  g.Minute, g.Second), 3000);
                    
                    return;
                }
                
                if (Main.ServerNumber != 0 && DateTime.Now < protectDate[region.GangOwner])
                {
                    long ticks = protectDate[region.GangOwner].Ticks - DateTime.Now.Ticks;
                    if (ticks <= 0) return;
                    
                    DateTime g = new DateTime(ticks);
                    
                    if (g.Minute >= 5)
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCanClaimGangRegion1h, g.Hour, g.Minute, g.Second), 3000);
                    else
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCanClaimGangRegion,  g.Minute, g.Second), 3000);
                    
                    return;
                }
                
                if (Main.ServerNumber != 0 && (Manager.FractionMembersCount(region.GangOwner) < 3))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DontHaveOnlineEnemyGang), 3000);
                    return;
                }
                
                if (World.War.Repository.Wars.Values.Any(w => w.Type == WarType.Gangs))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CaptIsGoingSomeGang), 3000);
                    return;
                }

                var warData = new Players.Models.WarData
                {
                    ObjectId = (ushort) region.ID,
                    Type = WarType.Gangs,
                    MapName = String.Empty,
                    MapId = (ushort) region.ID,
                    Position = gangZones[region.ID],
                    Range = Range,
                    AttackingId = (ushort) fracId,
                    ProtectingId = (ushort) region.GangOwner
                };
                
                ushort warId = World.War.Repository.War(player, warData, (sbyte) WarGripType.LastSurvivor, 0, 0, isCheckTime: false, isWarInterface: false);
                if (warId == 0) return;

                Trigger.ClientEventForAll("setZoneFlash", warData.ObjectId, true, gangPointsColor[warData.ProtectingId]);

                Trigger.SendToAdmins(1, $"{ChatColors.Red}[A] {player.Name} ({player.Value}) из {Manager.GetName(warData.AttackingId)} спровоцировал capture за территорию {Manager.GetName(warData.ProtectingId)}.");
                GameLog.FracLog(fracId, characterData.UUID, -1, player.Name, "-1", $"capture({region.ID}, {Manager.FractionNames[region.GangOwner]})");

                Manager.SendCoolFractionMsg(warData.ProtectingId, "Капт", "На вас напали!", LangFunc.GetText(LangType.Ru, DataName.CaptureAlertDefend, Manager.GetName(warData.AttackingId)), "", 10000);
                Manager.SendCoolFractionMsg(warData.AttackingId, "Капт", "Нападение", LangFunc.GetText(LangType.Ru, DataName.CaptureAlertAttack), "", 10000);
                //Manager.sendFractionMessage(warData.ProtectingId, LangFunc.GetText(LangType.Ru, DataName.CaptureAlertDefend, Manager.GetName(warData.AttackingId)));
                //Manager.sendFractionMessage(warData.AttackingId, LangFunc.GetText(LangType.Ru, DataName.CaptureAlertAttack));

                foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                {
                    if (foreachPlayer.Position.DistanceTo2D(warData.Position) > warData.Range) continue;
                    World.War.Repository.EntryZone(foreachPlayer, warId);
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_startCapture Exception: {e.ToString()}");
            }
        }
        public static void Update(byte id, int fractionId, ushort attackingId, ushort protectingId)
        {
            try
            {
                protectDate[protectingId] = DateTime.Now.AddMinutes(20);
                protectDate[attackingId] = DateTime.Now.AddMinutes(20);
                
                var nextCapt = DateTime.Now.AddMinutes(Main.ServerSettings.CaptureNextTimeMinutes);
                nextCaptDate[attackingId] = nextCapt;

                if (!GangPoints.ContainsKey(id))
                    return;

                var region = GangPoints[id];
                if (region.GangOwner == fractionId)
                    return;

                region.GangOwner = fractionId;
                region.Save();
                
                Trigger.ClientEventForAll("setZoneFlash", region.ID, false);
                Trigger.ClientEventForAll("setZoneColor", region.ID, gangPointsColor[region.GangOwner]);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        [Interaction(ColShapeEnums.WarGangZone, In: true)]
        public void InWarGangZone(ExtPlayer player, int index)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.CaptureJoin, false)) return;
                var fracId = player.GetFractionId();

                if (fracId >= (int) Models.Fractions.FAMILY && fracId <= (int) Models.Fractions.BLOOD)
                    World.War.Repository.InWarZone(player, WarType.Gangs, index);
            }
            catch (Exception e)
            {
                Log.Write($"InWarGangZone Exception: {e.ToString()}");
            }
        }

        [Interaction(ColShapeEnums.WarGangZone, Out: true)]
        public void OutWarGangZone(ExtPlayer player, int index) 
            => World.War.Repository.OutWarZone(player, WarType.Gangs, index);
        
        
        [Command("tpcapture")]
        public static void CMD_tpcapture(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                if (characterData.IsBannedCrime == true)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вам ограничили доступ к системам capture и bizwar. Причина: {characterData.BanCrimeReason}.", 5000);
                    return;
                }
                
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
                if (!player.IsFractionAccess(RankToAccess.CaptureJoin)) return;

                var fracId = player.GetFractionId();
                
                var war = World.War.Repository.Wars.Values
                    .FirstOrDefault(w => w.Type == WarType.Gangs);
                
                if (war != null && (fracId == war.ProtectingId || fracId == war.AttackingId))
                
                    if (sessionData.UsedTPOnCaptureOrBizwar == 1)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyTpToClaimRegion), 3000);
                        return;
                    }
                    if (sessionData.UsedTPOnCaptureOrBizwar == 2)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouWasInClaimRegion), 3000);
                        return;
                    }
                    if (sessionData.WarData.IsWarZone)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouAlreadyInClaimRegion), 3000);
                        return;
                    }

                    if (fracId != war.ProtectingId)
                    {
                        sessionData.PositionCaptureOrBizwar = player.GetPosition();
                        player.Position = war.Position;
                        sessionData.UsedTPOnCaptureOrBizwar = 1;
                        EventSys.SendCoolMsg(player,"Капт", "Телепорт", LangFunc.GetText(LangType.Ru, DataName.YouWasTpCenterClaimRegion), "", 5000);   
                        //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouWasTpCenterClaimRegion), 3000);
                    }
                    else
                    {
                        Vector3 nearestCaptureCoord = null;
                        foreach (var captureCoord in tpCaptureCoords)
                        {
                            if (war.Position.DistanceTo(captureCoord) <= 150)
                                continue;
                            
                            if (nearestCaptureCoord == null) 
                                nearestCaptureCoord = captureCoord;
                            
                            if (war.Position.DistanceTo(captureCoord) < war.Position.DistanceTo(nearestCaptureCoord)) 
                                nearestCaptureCoord = captureCoord;
                        }

                        if (nearestCaptureCoord != null)
                        {
                            sessionData.PositionCaptureOrBizwar = player.GetPosition();
                            player.Position = nearestCaptureCoord;
                            sessionData.UsedTPOnCaptureOrBizwar = 1;
                            EventSys.SendCoolMsg(player,"Капт", "Телепорт", LangFunc.GetText(LangType.Ru, DataName.YouWasTpNearClaimRegion), "", 5000);   
                            //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouWasTpNearClaimRegion), 3000);
                        }
                        else
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindNearClaimRegion), 3000);
                        
                    }
                
            }
            catch (Exception e)
            {
                Log.Write($"CMD_tpcapture Exception: {e.ToString()}");
            }
        }

        public static void LoadBlips(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                
                var colors = new List<int>();
                foreach (var g in GangPoints.Values)
                    colors.Add(gangPointsColor[g.GangOwner]);
                
                Trigger.ClientEvent(player, "loadCaptureBlips", JsonConvert.SerializeObject(colors));
                
                var wars = World.War.Repository.Wars.Values
                    .FirstOrDefault(w => w.Type == WarType.Gangs);
                
                if (wars != null)
                    Trigger.ClientEvent(player, "setZoneFlash", wars.ObjectId, true);
            }
            catch (Exception e)
            {
                Log.Write($"LoadBlips Exception: {e.ToString()}");
            }
        }
        
        public class GangPoint
        {
            public int ID { get; set; }
            public int GangOwner { get; set; }

            public void Save()
            {
                Trigger.SetTask(async () =>
                {    
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке
                        
                        await db.Gangspoints
                            .Where(g => g.Id == ID)
                            .Set(g => g.Gangid, (sbyte) GangOwner)
                            .UpdateAsync();
                    }
                    catch (Exception e)
                    {
                        Log.Write($"Gangspoints Save Exception: {e.ToString()}");
                    }

                });
            }
        }
    }
}