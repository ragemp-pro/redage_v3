using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Handles;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Functions;
using NeptuneEvo.Table.Models;
using NeptuneEvo.Table.Tasks.Models;
using NeptuneEvo.Table.Tasks.Player;
using NeptuneEvo.World.War.Models;
using Redage.SDK;

namespace NeptuneEvo.Fractions
{
    class MafiaWars : Script
    {
        private static readonly nLog Log = new nLog("Fractions.MafiaWars");
        
        private static Dictionary<int, DateTime> nextCaptDate = new Dictionary<int, DateTime>
        {
            { (int) Models.Fractions.LCN, DateTime.Now },
            { (int) Models.Fractions.RUSSIAN, DateTime.Now },
            { (int) Models.Fractions.YAKUZA, DateTime.Now },
            { (int) Models.Fractions.ARMENIAN, DateTime.Now },
        };
        
        private static Dictionary<int, DateTime> protectDate = new Dictionary<int, DateTime>
        {
            { (int) Models.Fractions.LCN, DateTime.Now },
            { (int) Models.Fractions.RUSSIAN, DateTime.Now },
            { (int) Models.Fractions.YAKUZA, DateTime.Now },
            { (int) Models.Fractions.ARMENIAN, DateTime.Now },
        };
        
        private static Vector3[] warPoints = new Vector3[5]
        {
            new Vector3(1713.4241, -1646.3751, 112.477806),
            new Vector3(33, -2678, 6),
            new Vector3(503.4392, -3163.064, 6.0652146),
            new Vector3(-1271, -2663.0005, 13.921528),
            new Vector3(-481.99838, -1716.0093, 18.688194)
        };


        private static Vector3[] warCenterPoints = new Vector3[5]
              {
            new Vector3(1713.4241, -1646.3751, 112.477806),
            new Vector3(33, -2678, 6),
            new Vector3(503.4392, -3163.064, 6.0652146),
            new Vector3(-1271, -2663.0005, 13.921528),
            new Vector3(-481.99838, -1716.0093, 18.688194)
             };

        private static Vector3[] warNearCenterPoints = new Vector3[5]
            {
            new Vector3(1733.8965, -1953.6902, 116.761795),
            new Vector3(251.92728, -2540.5017, 5.8491406),
            new Vector3(445.48645, -2951.1714, 6.00438),
            new Vector3(-1130.1005, -2918.0957, 13.945399),
            new Vector3(-290.82724, -1827.242, 26.411165)
            };

        private static Dictionary<int, ExtBlip> warBlips = new Dictionary<int, ExtBlip>();

        private static float Range = 150f;
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            try
            {
                int i = 0;
                foreach (var position in warPoints)
                {
                    CustomColShape.CreateSphereColShape(position, Range, NAPI.GlobalDimension, ColShapeEnums.WarPoint, i);
                    warBlips.Add(i, (ExtBlip)NAPI.Blip.CreateBlip(543, position, 1, 40, Main.StringToU16(LangFunc.GetText(LangType.Ru, DataName.BizWar)), 255, 0, true, 0, 0));
                    i++;
                }
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }

        [Command("bizwar")]
        public static void CMD_startBizwar(ExtPlayer player)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.BizWar)) return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (!FunctionsAccess.IsWorking("CMD_startBizwar"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                
                if (characterData.IsBannedCrime == true)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BizWarBan, characterData.BanCrimeReason), 5000);
                    return;
                }

                var bizId = CustomColShape.GetDataToEnum(player, ColShapeEnums.BusinessAction);
                if (bizId == (int)ColShapeData.Error)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouNotOnBiz), 3000);
                    return;
                }
                
                if (!BusinessManager.BizList.ContainsKey(bizId)) return;
                
                Business biz = BusinessManager.BizList[bizId];
                var fracId = player.GetFractionId();
                if (biz.Mafia == fracId)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantCaptYourBiz), 3000);
                    return;
                }
                
                if (biz.Mafia == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BizNotBelongsToMaf), 3000);
                    return;
                }
                
                if (Main.ServerNumber != 0 && (DateTime.Now.Hour < 13 || DateTime.Now.Hour >= 23))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MafiaTimeWar), 3000);
                    return;
                }
                
                if (Main.ServerNumber != 0 && DateTime.Now < nextCaptDate[fracId])
                {
                    long ticks = nextCaptDate[fracId].Ticks - DateTime.Now.Ticks;
                    if (ticks <= 0) return;
                    
                    DateTime g = new DateTime(ticks);
                    
                    if (g.Minute >= 5)
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarTimeMafHours, g.Hour, g.Minute, g.Second), 6000);
                    else
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarTimeMaf, g.Minute, g.Second), 6000);
                    
                    return;
                }
                
                if (Main.ServerNumber != 0 && DateTime.Now < protectDate[biz.Mafia])
                {
                    long ticks = protectDate[biz.Mafia].Ticks - DateTime.Now.Ticks;
                    if (ticks <= 0) return;
                    
                    DateTime g = new DateTime(ticks);
                    
                    if (g.Minute >= 5)
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarTimeMafHours, g.Hour, g.Minute, g.Second), 6000);
                    else
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarTimeMaf, g.Minute, g.Second), 6000);
                   
                    return;
                }
                
                if (Main.ServerNumber != 0 && (Manager.FractionMembersCount(biz.Mafia) < 4))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoOnline), 3000);
                    return;
                }
                
                if (World.War.Repository.Wars.Values.Any(w => w.Type == WarType.Mafia))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarBizGoing), 3000);
                    return;
                }
                
                var mapId = Jobs.WorkManager.rnd.Next(0, 5);
                
                var warData = new Players.Models.WarData
                {
                    ObjectId = (ushort) biz.ID,
                    Type = WarType.Mafia,
                    MapName = String.Empty,
                    MapId = (ushort) mapId,
                    Position = warCenterPoints[mapId],
                    Range = Range,
                    AttackingId = (ushort) fracId,
                    ProtectingId = (ushort) biz.Mafia
                };
                
                ushort warId = World.War.Repository.War(player, warData, (sbyte) WarGripType.LastSurvivor, 0, 0, isCheckTime: false, isWarInterface: false);
                if (warId == 0) return;

                warBlips[mapId].Color = 49;

                Trigger.SendToAdmins(1, LangFunc.GetText(LangType.Ru, DataName.AdminBizWar, ChatColors.Red, player.Name, player.Value, Manager.GetName(fracId), Manager.GetName(biz.Mafia)));
                GameLog.FracLog(fracId, characterData.UUID, -1, player.Name, "-1", $"bizwar({biz.ID}, {BusinessManager.BusinessTypeNames[biz.Type]}, {Manager.FractionNames[biz.Mafia]})");

                Manager.sendFractionMessage(biz.Mafia, LangFunc.GetText(LangType.Ru, DataName.BizWarDefendStart, Manager.GetName(fracId)));
                Manager.sendFractionMessage(fracId, LangFunc.GetText(LangType.Ru, DataName.YouStartBizWar));

                foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                {
                    if (foreachPlayer.Position.DistanceTo2D(warData.Position) > warData.Range) continue;
                    World.War.Repository.EntryZone(foreachPlayer, warId);
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_startBizwar Exception: {e.ToString()}");
            }
        }

        public static void Update(byte id, int fractionId, int mapId, ushort attackingId, ushort protectingId)
        {
            try
            {
                protectDate[protectingId] = DateTime.Now.AddMinutes(20);
                protectDate[attackingId] = DateTime.Now.AddMinutes(20);
                
                var nextCapt = DateTime.Now.AddMinutes(Main.ServerSettings.CaptureNextTimeMinutes);
                nextCaptDate[attackingId] = nextCapt;
                
                if (!BusinessManager.BizList.ContainsKey(id)) 
                    return;

                var biz = BusinessManager.BizList[id];
                if (biz.Mafia == fractionId)
                    return;

                biz.Mafia = fractionId;
                biz.UpdateLabel();
                
                Trigger.SetMainTask(() =>
                {
                    try
                    {
                        if (!warBlips.ContainsKey(mapId)) return;
                        warBlips[mapId].Color = 40;
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        [Interaction(ColShapeEnums.WarPoint, In: true)]
        public static void InWarPoint(ExtPlayer player, int index)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.BizwarJoin, false)) return;
                var fracId = player.GetFractionId();
                
                if (fracId >= (int) Models.Fractions.LCN && fracId <= (int) Models.Fractions.ARMENIAN)
                    World.War.Repository.InWarZone(player, WarType.Mafia, index);
            }
            catch (Exception e)
            {
                Log.Write($"InWarPoint Exception: {e.ToString()}");
            }
        }
        
        [Interaction(ColShapeEnums.WarPoint, Out: true)]
        public static void OutWarPoint(ExtPlayer player, int index)
            => World.War.Repository.OutWarZone(player, WarType.Mafia, index);
        
        [Command("tpbizwar")]
        public static void CMD_tpbizwar(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                if (characterData.IsBannedCrime == true)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,LangFunc.GetText(LangType.Ru, DataName.BizWarBan, characterData.BanCrimeReason), 5000);
                    return;
                }

                if (sessionData.CuffedData.Cuffed)
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
                else if (!player.IsFractionAccess(RankToAccess.BizwarJoin)) return;

                var fracId = player.GetFractionId();
                
                var war = World.War.Repository.Wars.Values
                    .FirstOrDefault(w => w.Type == WarType.Mafia);
                
                if (war != null && (fracId == war.ProtectingId || fracId == war.AttackingId))
                {
                    if (sessionData.UsedTPOnCaptureOrBizwar == 1)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyGoToWarZone), 3000);
                        return;
                    }
                    if (sessionData.UsedTPOnCaptureOrBizwar == 2)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyInWarZone), 3000);
                        return;
                    }
                    if (sessionData.WarData.IsWarZone)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyInWarZone), 3000);
                        return;
                    }

                    if (fracId == war.ProtectingId)
                    {
                        sessionData.PositionCaptureOrBizwar = player.GetPosition();
                        player.Position = warCenterPoints[war.MapId];
                        sessionData.UsedTPOnCaptureOrBizwar = 1;
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WastpCenterWar), 5000);
                    }
                    else
                    {
                        sessionData.PositionCaptureOrBizwar = player.GetPosition();
                        player.Position = warNearCenterPoints[war.MapId];
                        sessionData.UsedTPOnCaptureOrBizwar = 1;
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WasTPNearWar), 7000);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_tpbizwar Exception: {e.ToString()}");
            }
        }
    }
}
