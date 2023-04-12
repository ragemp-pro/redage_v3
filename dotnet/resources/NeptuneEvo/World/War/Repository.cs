using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Database;
using GTANetworkAPI;
using LinqToDB;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.Jobs;
using NeptuneEvo.Organizations.Models;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.Players;
using NeptuneEvo.Players.Popup.List.Models;
using NeptuneEvo.Table.Models;
using NeptuneEvo.Table.Tasks.Models;
using NeptuneEvo.Table.Tasks.Player;
using NeptuneEvo.World.War.Models;
using Newtonsoft.Json;
using Redage.SDK;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NeptuneEvo.World.War
{
    public class Repository
    {
        public static ConcurrentDictionary<ushort, WarData> Wars = new ConcurrentDictionary<ushort, WarData>();

        public static uint DefaultDimension = 250_000;
        private static ushort ProtectionCounting = 60 * 1;
        private static ushort WarCounting = 60 * 10;
        
        public static void OnResource()
        {
            
            using var db = new ServerBD("MainDB");//В отдельном потоке
            
            var wars = db.Wars.ToList();

            foreach (var warData in wars)
            {
                var id = Convert.ToUInt16(warData.Id);
                var war = new WarData();
                //
                war.Id = id;
                war.ObjectId = Convert.ToUInt16(warData.ObjectId);
                war.Type = (WarType) Convert.ToSByte(warData.Type);
                war.AttackingId = Convert.ToUInt16(warData.AttackingId);
                war.ProtectingId = Convert.ToUInt16(warData.ProtectingId);
                //
                war.MapName = warData.MapName;
                war.MapId = Convert.ToUInt16(warData.MapId);
                war.Position = JsonConvert.DeserializeObject<Vector3>(warData.Position);
                war.Range = Convert.ToSingle(warData.Range);
                //
                war.GripType = (WarGripType) Convert.ToSByte(warData.GripType);

                war.Composition = Convert.ToSByte(warData.Composition);
                
                war.WeaponsCategory = Convert.ToSByte(warData.WeaponsCategory);
                //
                war.Time = Convert.ToDateTime(warData.Time);
                war.RetiredUuId = new List<int>();
                war.Status = WarStatus.Create;
                
                Wars.TryAdd(id, war);
            }
            
        }
        
        //
        
        private static ushort GetId()
        {
            try
            {
                ushort id = 0;
                do
                {
                    id++;
                } while (Wars.ContainsKey(id));

                return id;
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }

            return UInt16.MaxValue;
        }


        public static int GetTime(DateTime time1, DateTime time2)
        {
            var min1 = (Int32) (time1.Subtract(new DateTime(1970, 1, 1))).TotalMinutes;
            var min2 = (Int32) (time2.Subtract(new DateTime(1970, 1, 1))).TotalMinutes;
            if (min1 > min2)
                return min1 - min2;
            
            return min2 - min1;
        }
        
        private static int GetTimeSec(DateTime time1, DateTime time2)
        {
            var min1 = (Int32) (time1.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var min2 = (Int32) (time2.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            if (min1 > min2)
                return min1 - min2;
            
            return min2 - min1;
        }
        public static void Open(ExtPlayer player, ushort objectId, ushort mapId, string mapName, Vector3 position, float range, WarType type, ushort attackingId, ushort protectingId)
        {
            try
            {
                var sessionData = player.GetSessionData(); 
                if (sessionData == null) 
                    return;
                
                var warData = sessionData.WarData;

                warData.ObjectId = objectId;
                warData.MapId = mapId;
                warData.MapName = mapName;
                warData.Position = position;
                warData.Range = range;
                warData.Type = type;
                warData.AttackingId = attackingId;
                warData.ProtectingId = protectingId;

                var title = "";
                
                if (type == WarType.Gangs)
                    title = $"Нападание на зону {mapName}";
                if (type == WarType.Mafia)
                    title = $"Нападание на бизес {mapName}";
                if (type == WarType.OrgHouse)
                    title = $"Нападание на дом {mapName}";
                if (type == WarType.OrgWarZone)
                    title = $"Нападание на зону {mapName}";
                
                var owner = "";
                if (type == WarType.Gangs || type == WarType.Mafia)
                {
                    var fractionDataAttack = Fractions.Manager.GetFractionData(protectingId);
                    if (fractionDataAttack != null)
                        owner = fractionDataAttack.Name;
                }
                else
                {
                    var organizationDataAttack = Organizations.Manager.GetOrganizationData(warData.AttackingId);
                    if (organizationDataAttack != null)
                        owner = organizationDataAttack.Name;
                }

                Trigger.ClientEvent(player, "client.openWar", title, owner);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public static ushort War(ExtPlayer player, Players.Models.WarData warData, sbyte typeBattle, sbyte composition, sbyte weaponsCategory, sbyte day = 0, sbyte hour = 0, sbyte min = 0, bool isCheckTime = true, bool isWarInterface = true)
        {
            if (!player.IsCharacterData()) return 0;

            ushort id = GetId();
            try
            {
                if (id == UInt16.MaxValue)
                {
                    if (isWarInterface) Trigger.ClientEvent(player, "client.closeWar");
                    return 0;
                }
                
                var time = DateTime.Now;
                if (isCheckTime)
                {
                    if (day != 0 && day != 1)
                        return 0;

                    time = new DateTime(time.Year, time.Month, time.Day, hour, min, 0);
                    time = time.AddDays(day);

                    if (DateTime.Now > time)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Неверное время.", 6000);
                        //if (isWarInterface) Trigger.ClientEvent(player, "client.closeWar");
                        return 0;
                    }

                    if (Main.ServerNumber != 0 && DateTime.Now.AddHours(2) > time)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Стрелку можно забить минимум за два часа", 3000);
                        //if (isWarInterface) Trigger.ClientEvent(player, "client.closeWar");
                        return 0;
                    }
                    
                    if (Main.ServerNumber == 0 && DateTime.Now.AddMinutes(5) > time)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Стрелку можно забить минимум за 5 минут", 3000);
                        //if (isWarInterface) Trigger.ClientEvent(player, "client.closeWar");
                        return 0;
                    }
                }

                if (Wars.Values.Any(w => w.Type == warData.Type && w.ObjectId == warData.ObjectId))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Эта зона уже находится в процессе захвата.", 8000);
                    if (isWarInterface) Trigger.ClientEvent(player, "client.closeWar");
                    return 0;
                }

                if (warData.Type == WarType.OrgWarZone && !Organizations.FamilyZones.Repository.IsAttack(player, time, day, warData.AttackingId, warData.ProtectingId))
                    return 0;
                
                var war = new WarData();
                //
                war.Id = id;
                war.ObjectId = warData.ObjectId;
                war.Type = warData.Type;
                war.AttackingId = warData.AttackingId;
                war.ProtectingId = warData.ProtectingId;
                //
                war.MapName = warData.MapName;
                war.MapId = warData.MapId;
                war.Position = warData.Position;
                war.Range = warData.Range;
                //
                war.GripType = (WarGripType) typeBattle;

                if (composition == 0)
                {
                    war.Composition = -1;
                }
                else
                {
                    war.Composition = composition;
                }
                
                war.WeaponsCategory = weaponsCategory;
                //
                war.Time = time;
                war.RetiredUuId = new List<int>();
                war.Status = WarStatus.Create;
                war.Insert();
                
                Wars.TryAdd(id, war);
                
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы забили стрелку.", 15000);
                Trigger.ClientEvent(player, "client.closeWar");
                
                if (warData.Type == WarType.OrgWarZone) Organizations.FamilyZones.Repository.Open(player);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }

            return id;
        }
        private static List<WarData> GetWars(WarType type, int mapId, int uuid, int fractionId, int organizationId, bool isProtection = true, bool isCreate = false)
        {
            /*var wars = Wars.Values
                .Where(w => w.MapId == mapId && !w.RetiredUuId.Contains(uuid))
                .Where(w => ((w.Type == WarType.Gangs || w.Type == WarType.Mafia) &&
                             (w.AttackingId == fractionId || w.ProtectingId == fractionId))
                            || ((w.Type == WarType.OrgWarZone || w.Type == WarType.OrgHouse) &&
                                (w.AttackingId == organizationId || w.ProtectingId == organizationId)))
                .Where(w => w.Status == WarStatus.War)
                .ToList();*/
            var wars = new List<WarData>();

            foreach (var w in Wars.Values.ToList())
            {
                if (!isCreate && w.Status == WarStatus.Create)
                    continue;
                
                if (isCreate && w.Status != WarStatus.Create)
                    continue;
                
                if (type != WarType.None && w.Type != type)
                    continue;
                
                if (mapId != -1 && w.MapId != mapId)
                    continue;
                
                if (w.RetiredUuId.Contains(uuid))
                    continue;
                
                if (isProtection && w.Status == WarStatus.Protection && w.ProtectingId != fractionId && w.ProtectingId != organizationId)
                    continue;
                
                if ((w.Type == WarType.Gangs || w.Type == WarType.Mafia) && w.AttackingId != fractionId && w.ProtectingId != fractionId)
                    continue;
                
                if ((w.Type == WarType.OrgWarZone || w.Type == WarType.OrgHouse) && w.AttackingId != organizationId && w.ProtectingId != organizationId)
                    continue;
                
                if (w.Composition > 0 && w.AttackingPlayersInZone >= w.Composition && (w.AttackingId == fractionId || w.AttackingId == organizationId))
                    continue;
                
                if (w.Composition > 0 && w.ProtectingPlayersInZone >= w.Composition && (w.ProtectingId == fractionId || w.ProtectingId == organizationId))
                    continue;
                
                wars.Add(w);
            }
            
            
            return wars;
        }
        //
        public static void InWarZone(ExtPlayer player, WarType type, int index)
        {
            try
            {
                var memberFractionData = player.GetFractionMemberData();
                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberFractionData == null && memberOrganizationData == null)
                    return;

                var wars = GetWars(type, index, player.GetUUID(), memberFractionData != null ? memberFractionData.Id : -1,
                    memberOrganizationData != null ? memberOrganizationData.Id : -1);
                
                if (wars == null || wars.Count == 0)
                    return;

                if (wars.Count == 1)
                    EntryZone(player, wars[0].Id);
                else
                {
                    var frameList = new FrameListData();

                    frameList.Header = "Выбор Зоны войны";
                    frameList.Callback = CallbackEntryZone;
                
                    foreach (var war in wars)
                    {
                        var title = "";
                        
                        if (war.Type == WarType.Gangs)
                            title = "Война за зону";
                        else if (war.Type == WarType.Mafia)
                            title = "Война за бизнес";
                        else if (war.Type == WarType.OrgWarZone)
                            title = "Война за зону";
                        else if (war.Type == WarType.OrgHouse)
                            title = "Война за дом";
                        
                        frameList.List.Add(new ListData($"{title} - {war.ObjectId}", war.Id));
                    }
                
                    Players.Popup.List.Repository.Open(player, frameList);
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        private static void CallbackEntryZone(ExtPlayer player, object listItem)
        {
            try
            {
                if (!player.IsCharacterData())
                    return;
                
                if (!(listItem is int) || listItem == null)
                    return;

                EntryZone(player, Convert.ToUInt16(listItem));
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public static void EntryZone(ExtPlayer player, ushort warId)
        {
            try
            {
                var sessionData = player.GetSessionData(); 
                if (sessionData == null) 
                    return;
                
                var warData = sessionData.WarData;
                
                if (!Wars.ContainsKey(warId))
                    return;
                
                var war = Wars[warId];
                
                if (war.RetiredUuId.Contains(player.GetUUID()))
                    return;

                var attackName = "";
                var protectingName = "";
                
                if (war.Type == WarType.Gangs || war.Type == WarType.Mafia)
                {
                    var fractionDataAttack = Fractions.Manager.GetFractionData(war.AttackingId);
                    if (fractionDataAttack != null)
                        attackName = fractionDataAttack.Name;
                    
                    var fractionDataProtecting = Fractions.Manager.GetFractionData(war.ProtectingId);
                    if (fractionDataProtecting != null)
                        protectingName = fractionDataProtecting.Name;
                    
                    //
                    
                    var fractionId = player.GetFractionId();
                    if (fractionId == war.AttackingId)
                        warData.IsAttacking = true;
                    else
                        warData.IsAttacking = false;
                }
                else
                {
                    var organizationDataAttack = Organizations.Manager.GetOrganizationData(war.AttackingId);
                    if (organizationDataAttack != null)
                        attackName = organizationDataAttack.Name;
                    
                    var organizationDataProtecting = Organizations.Manager.GetOrganizationData(war.ProtectingId);
                    if (organizationDataProtecting != null)
                        protectingName = organizationDataProtecting.Name;
                    
                    //
                    
                    var organizationMemberData = player.GetOrganizationMemberData();
                    if (organizationMemberData.Id == war.AttackingId)
                        warData.IsAttacking = true;
                    else
                        warData.IsAttacking = false;
                    
                }
                
                warData.WarId = warId;
                warData.IsWarZone = true;
                
                //

                if (warData.IsAttacking)
                {
                    war.AttackingPlayersInZone++;
                    war.AttackingPlayersInZoneCount++;
                }
                else
                {
                    war.ProtectingPlayersInZone++;
                    war.ProtectingPlayersInZoneCount++;
                }

                if (war.AttackingPlayersInZone > 0 && war.ProtectingPlayersInZone > 0)
                    war.IsStartWar = true;
                
                //
                
                Trigger.Dimension(player, war.GetDimension());

                var time = war.Status == WarStatus.Protection ? war.Counting + WarCounting : war.Counting;
                
                Trigger.ClientEvent(player, "inWarZone", war.Type, war.GripType, attackName, protectingName, time, war.AttackingCount, war.ProtectingCount, war.WeaponsCategory, war.AttackingPlayersInZone, war.ProtectingPlayersInZone);
                
                //
                
                foreach (var foreachPlayer in NeptuneEvo.Character.Repository.GetPlayers())
                {
                    var foreachSessionData = foreachPlayer.GetSessionData();
                    if (foreachSessionData == null) 
                        continue;
                    
                    var foreachWarData = foreachSessionData.WarData;
                    if (!foreachWarData.IsWarZone || warId != foreachWarData.WarId) 
                        continue;
                    
                    Trigger.ClientEvent(foreachPlayer, "playersWarZone", war.AttackingPlayersInZone, war.ProtectingPlayersInZone);
                }
                
                //
                
                if (war.Type == WarType.Gangs)
                    player.AddTableScore(TableTaskId.Item30);
                    
                if (war.Type == WarType.Mafia)
                    player.AddTableScore(TableTaskId.Item24);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        public static void OutWarZone(ExtPlayer player, WarType type, int index)
        {
            try
            {
                var sessionData = player.GetSessionData(); 
                if (sessionData == null) 
                    return;
                
                var warData = sessionData.WarData;
                var warId = warData.WarId;
                if (!warData.IsWarZone) 
                    return;
                
                if (!Wars.ContainsKey(warId))
                    return;
                var war = Wars[warId];
                
                if (war.Type != type)
                    return;
                
                if (war.MapId != index)
                    return;
                
                ExitZone(player, war, type: type);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        private static void ExitZone(ExtPlayer player, WarData war, WarType type = WarType.None, bool isDisconnect = false)
        {
            try
            {
                if (type != WarType.None && type != war.Type)
                    return;
                
                var sessionData = player.GetSessionData(); 
                if (sessionData == null) 
                    return;
                
                var warData = sessionData.WarData;
                if (war.RetiredUuId.Contains(player.GetUUID()))
                    return;

                if (war.GripType == WarGripType.LastSurvivor)
                    war.RetiredUuId.Add(player.GetUUID());

                var warId = warData.WarId;
                
                if (!isDisconnect)
                {
                    Trigger.Dimension(player);
                    if (war.GripType == WarGripType.LastSurvivor || war.GripType == WarGripType.Destroy)
                        Trigger.ClientEvent(player, "outWarZone", warId, true);
                    else
                        Trigger.ClientEvent(player, "outWarZone");
                }

                //
                
                if (warData.IsAttacking)
                    war.AttackingPlayersInZone--;
                else
                    war.ProtectingPlayersInZone--;
                
                //
                if (war.GripType != WarGripType.Destroy)
                {
                    foreach (var foreachPlayer in NeptuneEvo.Character.Repository.GetPlayers())
                    {
                        var foreachSessionData = foreachPlayer.GetSessionData();
                        if (foreachSessionData == null)
                            continue;

                        var foreachWarData = foreachSessionData.WarData;
                        if (!foreachWarData.IsWarZone || warId != foreachWarData.WarId)
                            continue;

                        Trigger.ClientEvent(foreachPlayer, "playersWarZone", war.AttackingPlayersInZone, war.ProtectingPlayersInZone);
                    }
                }


                warData.IsWarZone = false;
                warData.WarId = 0;
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        private static IReadOnlyDictionary<uint, ushort> WeaponPoints = new Dictionary<uint, ushort>()
        {
            { (uint)WeaponHash.Unarmed, 5 },
            /* Handguns */
            { (uint)WeaponHash.Knife, 3 },
            { (uint)WeaponHash.Nightstick, 3 },
            { (uint)WeaponHash.Hammer, 3 },
            { (uint)WeaponHash.Bat, 3 },
            { (uint)WeaponHash.Crowbar, 3 },
            { (uint)WeaponHash.Golfclub, 3 },
            { (uint)WeaponHash.Bottle, 3 },
            { (uint)WeaponHash.Dagger, 3 },
            { (uint)WeaponHash.Hatchet, 3 },
            { (uint)WeaponHash.Knuckle, 3 },
            { (uint)WeaponHash.Machete, 3 },
            { (uint)WeaponHash.Flashlight, 3 },
            { (uint)WeaponHash.Switchblade, 3 },
            { (uint)WeaponHash.Poolcue, 3 },
            { (uint)WeaponHash.Wrench, 3 },
            { (uint)WeaponHash.Battleaxe, 3 },
            { (uint)WeaponHash.Stone_hatchet, 3 },
            /* Pistols */
            { (uint)WeaponHash.Pistol, 2 },
            { (uint)WeaponHash.Combatpistol, 2 },
            { (uint)WeaponHash.Pistol50, 2 },
            { (uint)WeaponHash.Snspistol, 2 },
            { (uint)WeaponHash.Heavypistol, 2 },
            { (uint)WeaponHash.Vintagepistol, 2 },
            { (uint)WeaponHash.Marksmanpistol, 2 },
            { (uint)WeaponHash.Revolver, 2 },
            { (uint)WeaponHash.Appistol, 2 },
            { (uint)WeaponHash.Stungun, 2 },
            { (uint)WeaponHash.Flaregun, 2 },
            { (uint)WeaponHash.Doubleaction, 2 },
            { (uint)WeaponHash.Pistol_mk2, 2 },
            { (uint)WeaponHash.Snspistol_mk2, 2 },
            { (uint)WeaponHash.Revolver_mk2, 2 },
            { (uint)WeaponHash.Raypistol, 2 },
            { (uint)WeaponHash.CeramicPistol, 2 },
            { (uint)WeaponHash.NavyRevolver, 2 },
            /* SMG */
            { (uint)WeaponHash.Microsmg, 1 },
            { (uint)WeaponHash.Machinepistol, 1 },
            { (uint)WeaponHash.Smg, 1 },
            { (uint)WeaponHash.Assaultsmg, 1 },
            { (uint)WeaponHash.Combatpdw, 1 },
            { (uint)WeaponHash.Mg, 1 },
            { (uint)WeaponHash.Combatmg, 1 },
            { (uint)WeaponHash.Gusenberg, 1 },
            { (uint)WeaponHash.Minismg, 1 },
            { (uint)WeaponHash.Smg_mk2, 1 },
            { (uint)WeaponHash.Combatmg_mk2, 1 },
            { (uint)WeaponHash.Raycarbine, 1 },
            /* Rifles */
            { (uint)WeaponHash.Assaultrifle, 1 },
            { (uint)WeaponHash.Carbinerifle, 1 },
            { (uint)WeaponHash.Advancedrifle, 1 },
            { (uint)WeaponHash.Specialcarbine, 1 },
            { (uint)WeaponHash.Bullpuprifle, 1 },
            { (uint)WeaponHash.Compactrifle, 1 },
            { (uint)WeaponHash.Assaultrifle_mk2, 1 },
            { (uint)WeaponHash.Carbinerifle_mk2, 1 },
            { (uint)WeaponHash.Specialcarbine_mk2, 1 },
            { (uint)WeaponHash.Bullpuprifle_mk2, 1 },
            /* Sniper */
            { (uint)WeaponHash.Sniperrifle, 2 },
            { (uint)WeaponHash.Heavysniper, 2 },
            { (uint)WeaponHash.Marksmanrifle, 2 },
            { (uint)WeaponHash.Heavysniper_mk2, 2 },
            { (uint)WeaponHash.Marksmanrifle_mk2, 2 },
            { 0x9D1F17E6, 2 },
            /* Shotguns */
            { (uint)WeaponHash.Pumpshotgun, 2 },
            { (uint)WeaponHash.Sawnoffshotgun, 2 },
            { (uint)WeaponHash.Bullpupshotgun, 2 },
            { (uint)WeaponHash.Assaultshotgun, 2 },
            { (uint)WeaponHash.Musket, 2 },
            { (uint)WeaponHash.Heavyshotgun, 2 },
            { (uint)WeaponHash.Dbshotgun, 2 },
            { (uint)WeaponHash.Autoshotgun, 2 },
            { (uint)WeaponHash.Pumpshotgun_mk2, 2 },
            /* Heavy */
            { (uint)WeaponHash.Grenadelauncher, 1 },
            { (uint)WeaponHash.Rpg, 1 },
            { (uint)WeaponHash.Minigun, 1 },
            { (uint)WeaponHash.Firework, 1 },
            { (uint)WeaponHash.Railgun, 1 },
            { (uint)WeaponHash.Hominglauncher, 1 },

            { (uint)WeaponHash.Grenadelauncher_smoke, 1 },
            { (uint)WeaponHash.Compactlauncher, 1 },
            { (uint)WeaponHash.Rayminigun, 1 },
            /* Throwables & Misc */
            { (uint)WeaponHash.Grenade, 1 },
            { (uint)WeaponHash.Stickybomb, 1 },
            { (uint)WeaponHash.Proximine, 1 },
            { (uint)WeaponHash.Bzgas, 1 },
            { (uint)WeaponHash.Molotov, 1 },
            { (uint)WeaponHash.Fireextinguisher, 1 },
            { (uint)WeaponHash.Petrolcan, 1 },
            { (uint)WeaponHash.Flare, 1 },
            { (uint)WeaponHash.Ball, 1 },
            { (uint)WeaponHash.Snowball, 1 },
            { (uint)WeaponHash.Smokegrenade, 1 },
            { (uint)WeaponHash.Pipebomb, 1 },
            { (uint)WeaponHash.Parachute, 1 }
        };
        private static void AddZonePoint(WarData war, uint weapon, bool isAttacking = false)
        {
            if (war == null)
                return;
            
            if (war.GripType == WarGripType.RetentionTerritory)
                return;
            
            ushort count = 1;
            
            if (war.GripType == WarGripType.WeaponPoints && WeaponPoints.ContainsKey(weapon))
                count = WeaponPoints[weapon];
            
            if (!isAttacking)
                war.AttackingCount += count;
            else
                war.ProtectingCount += count;
            
        }

        public static void OnPlayerSpawn(ExtPlayer player)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.IsWar, false) && !player.IsOrganizationAccess(RankToAccess.IsWar, false))
                    return;
                
                var memberFractionData = player.GetFractionMemberData();
                var memberOrganizationData = player.GetOrganizationMemberData();
                if (memberFractionData == null && memberOrganizationData == null)
                    return;

                var wars = GetWars(WarType.None, -1, player.GetUUID(), memberFractionData != null ? memberFractionData.Id : -1, memberOrganizationData != null ? memberOrganizationData.Id : -1);

                foreach (var war in wars)
                {
                    Trigger.ClientEvent(player, "createWarZone", war.Id, war.Position.X, war.Position.Y, war.Position.Z, war.Range);
                }
                
                wars = GetWars(WarType.None, -1, player.GetUUID(), memberFractionData != null ? memberFractionData.Id : -1, memberOrganizationData != null ? memberOrganizationData.Id : -1, isCreate: true);

                var time = DateTime.Now;
                
                var warsInfo = new List<List<object>>();

                foreach (var war in wars)
                {
                    var warInfo = new List<object>();
                    var isAttack = ((memberFractionData != null && memberFractionData.Id == war.AttackingId) || (memberOrganizationData != null && memberOrganizationData.Id == war.AttackingId));
                    
                    warInfo.Add(isAttack);
                    warInfo.Add(war.MapName);
                    warInfo.Add(war.GripType);
                    warInfo.Add(war.Time.ToString("dd.MM HH:mm"));
                    warInfo.Add(war.Composition);
                    warInfo.Add(war.WeaponsCategory);
                    
                    warsInfo.Add(warInfo);


                    var getTime = GetTimeSec(time, war.Time);
                    if (getTime <= (15 * 60))
                        Notification(player, war, null, getTime, isAttack, war.Position);
                    else
                        Notification(player, war, null, getTime, isAttack);
                }
                
                if (warsInfo.Count > 0)
                    Trigger.ClientEvent(player, "initWarZone", JsonConvert.SerializeObject(warsInfo));
                
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public static void OnPlayerDisconnect(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData(); 
                if (sessionData == null) 
                    return;
                var warData = sessionData.WarData;
                if (!warData.IsWarZone) 
                    return;
                var warId = warData.WarId;
                if (!Wars.ContainsKey(warId))
                    return;
                
                ExitZone(player, Wars[warId], isDisconnect: true);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static bool OnPlayerDeath(ExtPlayer player, ExtPlayer killer, uint weapon)
        {
            try
            {
                var sessionData = player.GetSessionData(); 
                if (sessionData == null) 
                    return false;
                
                var warData = sessionData.WarData;
                if (!warData.IsWarZone) 
                    return false;
                
                var warId = warData.WarId;
                if (!Wars.ContainsKey(warId))
                    return false;

                var war = Wars[warId];
                
                if (killer != null)
                {
                    if (war.Type == WarType.Gangs)
                        killer.AddTableScore(TableTaskId.Item31);
                    
                    if (war.Type == WarType.Mafia)
                        killer.AddTableScore(TableTaskId.Item25);
                }
                
                if (war.Type == WarType.Gangs || war.Type == WarType.Mafia)
                {
                    var fractionMemberData = player.GetFractionMemberData();
                    if (fractionMemberData == null)
                        return false;
                    
                    NAPI.Player.SpawnPlayer(player, Fractions.Manager.FractionSpawns[fractionMemberData.Id]);
                }
                else
                {
                    var organizationData = player.GetOrganizationData();
                    if (organizationData == null)
                        return false;
                    
                    if (organizationData.BlipId != -1) 
                        NAPI.Player.SpawnPlayer(player, organizationData.BlipPosition);
                    else 
                        NAPI.Player.SpawnPlayer(player, new Vector3(-774.045, 311.2569, 85.70606));
                }
                
                AddZonePoint(war, weapon, warData.IsAttacking);

                ExitZone(player, war);

                if (war.GripType == WarGripType.RetentionTerritory)
                    return true;

                /*if (war.GripType == WarGripType.LastSurvivor)
                {
                    war.AttackingCount = 0;
                    war.ProtectingCount = 0;
                    
                    foreach (var foreachPlayer in NeptuneEvo.Character.Repository.GetPlayers())
                    {
                        var foreachSessionData = foreachPlayer.GetSessionData();
                        if (foreachSessionData == null) 
                            continue;
                    
                        var foreachWarData = foreachSessionData.WarData;
                        if (!foreachWarData.IsWarZone || warId != foreachWarData.WarId) 
                            continue;
                        
                        if (foreachWarData.IsAttacking)
                            war.AttackingCount++;
                        else
                            war.ProtectingCount++;
                    }
                }*/

                var killerId = killer != null ? killer.Id : -1;
                var playerId = player.Id;
                foreach (var foreachPlayer in NeptuneEvo.Character.Repository.GetPlayers())
                {
                    var foreachSessionData = foreachPlayer.GetSessionData();
                    if (foreachSessionData == null) 
                        continue;
                    
                    var foreachWarData = foreachSessionData.WarData;
                    if (!foreachWarData.IsWarZone || warId != foreachWarData.WarId) 
                        continue;

                    Trigger.ClientEvent(foreachPlayer, "countWarZone", war.AttackingCount, war.ProtectingCount, killerId, playerId, weapon);
                }
                
                if (war.GripType == WarGripType.LastSurvivor && war.IsStartWar && (war.AttackingPlayersInZone <= 0 || war.ProtectingPlayersInZone <= 0))
                    EndWar(warId);
                
                return true;
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }

            return false;
        }
        
        
        private static void Notification(ushort warId, string text = null, bool isWarZone = false, int minute = 0, Vector3 wayPoint = null)
        {
            try
            {
                if (!Wars.ContainsKey(warId))
                    return;

                var war = Wars[warId];
                
                var isFraction = war.Type == WarType.Gangs || war.Type == WarType.Mafia;
                foreach (var foreachPlayer in NeptuneEvo.Character.Repository.GetPlayers())
                {
                    var isAttack = false;
                    
                    if (isWarZone)
                    {
                        var foreachSessionData = foreachPlayer.GetSessionData();
                        if (foreachSessionData == null) 
                            continue;
                    
                        var foreachWarData = foreachSessionData.WarData;
                        if (!foreachWarData.IsWarZone || warId != foreachWarData.WarId) 
                            continue;
                        
                        isAttack = foreachWarData.WarId == war.AttackingId;
                    }
                    else if (isFraction)
                    {
                        var foreachFractionId = foreachPlayer.GetFractionId();
                        if (foreachFractionId == (int) Fractions.Models.Fractions.None)
                            continue;
                            
                        if (foreachFractionId != war.AttackingId && foreachFractionId != war.ProtectingId)
                            continue;
                                
                        isAttack = foreachFractionId == war.AttackingId;
                    }
                    else
                    {
                        var foreachOrganizationMemberData = foreachPlayer.GetOrganizationMemberData();
                        if (foreachOrganizationMemberData == null)
                            continue;
                            
                        if (foreachOrganizationMemberData.Id != war.AttackingId && foreachOrganizationMemberData.Id != war.ProtectingId)
                            continue;
                        
                        isAttack = foreachOrganizationMemberData.Id == war.AttackingId;
                    }
                    
                    Notification(foreachPlayer, war, text, minute, isAttack, wayPoint);
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        private static void Notification(ExtPlayer player, WarData war, string text = null, int minute = 0, bool isAttack = false, Vector3 wayPoint = null)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                //Уведомление о войне
                if (text == null)
                {
                    var time = war.Time.ToString("dd.MM HH:mm");
                        
                    if (minute == 60)
                        time = "Минута";
                    if (minute == 30)
                        time = "30 секунд";
                    if (minute == 15)
                        time = "15 секунд";
                        
                    if (wayPoint != null) 
                        Trigger.ClientEvent(player, "infoWarZone", -1, isAttack, war.MapName, war.GripType, time, war.Composition, war.WeaponsCategory, wayPoint.X, wayPoint.Y);
                    else
                        Trigger.ClientEvent(player, "infoWarZone", -1, isAttack, war.MapName, war.GripType, time, war.Composition, war.WeaponsCategory);
                }
                else
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, text, 4500);

                if (wayPoint != null)
                {
                    Trigger.ClientEvent(player, "createWaypoint", wayPoint.X, wayPoint.Y);

                    if (!sessionData.IsWarMarker)
                    {
                        sessionData.IsWarMarker = true;
                        
                        player.SetSharedData("warType", (ushort) (isAttack ? 2 : 1));
                    }
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        private static void UpdateWar(ushort warId)
        {
            try
            {
                if (!Wars.ContainsKey(warId))
                    return;

                var time = DateTime.Now;
                
                var war = Wars[warId];
                
                if (war.Status == WarStatus.Create)
                {
                    if (war.Time <= time)
                    {
                        war.Delete();
                        
                        if (war.AttackingId == 0)
                        {
                            EndWar (warId);
                        }
                        else
                        {
                            if (war.GripType == WarGripType.RetentionTerritory)
                            {
                                war.Status = WarStatus.War;
                                war.Counting = WarCounting;
                                Notification(war.Id, "Команда противника может зайти в купол.");
                            }
                            else
                            {
                                war.Status = WarStatus.Protection;
                                war.Counting = ProtectionCounting;
                                Notification(war.Id, "Команда противника может зайти в купол.");
                            }
                            //
                            var isFraction = war.Type == WarType.Gangs || war.Type == WarType.Mafia;
                            foreach (var foreachPlayer in NeptuneEvo.Character.Repository.GetPlayers())
                            {
                                if (isFraction)
                                {
                                    var foreachFractionId = foreachPlayer.GetFractionId();
                                    if (foreachFractionId == (int) Fractions.Models.Fractions.None)
                                        continue;
                            
                                    if (foreachFractionId != war.AttackingId && foreachFractionId != war.ProtectingId)
                                        continue;
                                }
                                else
                                {
                                    var foreachOrganizationMemberData = foreachPlayer.GetOrganizationMemberData();
                                    if (foreachOrganizationMemberData == null)
                                        continue;
                            
                                    if (foreachOrganizationMemberData.Id != war.AttackingId && foreachOrganizationMemberData.Id != war.ProtectingId)
                                        continue;
                                }
                        
                                Trigger.ClientEvent(foreachPlayer, "createWarZone", war.Id, war.Position.X, war.Position.Y, war.Position.Z, war.Range);
                            }
                            
                        }
                    }
                    else
                    {
                        var getTime = GetTimeSec(time, war.Time);
                        if (getTime == 15)
                            Notification(war.Id, minute: getTime, wayPoint: war.Position);
                        if (getTime == 30)
                            Notification(war.Id, minute: getTime, wayPoint: war.Position);
                        if (getTime == (1 * 60))
                            Notification(war.Id, minute: getTime, wayPoint: war.Position);
                        if (getTime == (2 * 60))
                            Notification(war.Id, wayPoint: war.Position);
                        if (getTime == (3 * 60))
                            Notification(war.Id, wayPoint: war.Position);
                        if (getTime == (4 * 60))
                            Notification(war.Id, wayPoint: war.Position);
                        if (getTime == (5 * 60))
                            Notification(war.Id, wayPoint: war.Position);
                        if (getTime == (10 * 60))
                            Notification(war.Id, wayPoint: war.Position);
                        if (getTime == (15 * 60))
                            Notification(war.Id, wayPoint: war.Position);
                        if (getTime == (30 * 60))
                            Notification(war.Id);
                        if (getTime == (60 * 60))
                            Notification(war.Id);
                    
                    }
                        
                }
                else if (war.Status == WarStatus.Protection)
                {

                        war.Status = WarStatus.War;
                        war.Counting = WarCounting;
                        Notification(war.Id, "Команда атаки может зайти в купол.");

                }
                else if (war.Status == WarStatus.War)
                {
                    if (war.GripType == WarGripType.RetentionTerritory)
                        UpdateRetentionTerritory (warId);
                    
                    if (0 >= --war.Counting)
                        EndWar (warId);
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        private static void UpdateRetentionTerritory(ushort warId)
        {
            try
            {
                if (!Wars.ContainsKey(warId))
                    return;

                var war = Wars[warId];

                if ((war.Counting % 10) == 0)
                {
                    foreach (var foreachPlayer in NeptuneEvo.Character.Repository.GetPlayers())
                    {
                        var foreachSessionData = foreachPlayer.GetSessionData();
                        if (foreachSessionData == null) 
                            continue;
                    
                        var foreachWarData = foreachSessionData.WarData;
                        if (!foreachWarData.IsWarZone || warId != foreachWarData.WarId) 
                            continue;

                        if (foreachWarData.IsAttacking)
                            war.AttackingCount++;
                        else
                            war.ProtectingCount++;
                    }
                    
                    foreach (var foreachPlayer in NeptuneEvo.Character.Repository.GetPlayers())
                    {
                        var foreachSessionData = foreachPlayer.GetSessionData();
                        if (foreachSessionData == null) 
                            continue;
                    
                        var foreachWarData = foreachSessionData.WarData;
                        if (!foreachWarData.IsWarZone || warId != foreachWarData.WarId) 
                            continue;
                        
                        Trigger.ClientEvent(foreachPlayer, "countWarZone", war.AttackingCount, war.ProtectingCount);
                    }

                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        private static void EndWar (ushort warId)
        {
            try
            {
                if (!Wars.ContainsKey(warId))
                    return;

                var war = Wars[warId];

                war.GripType = WarGripType.Destroy;
                
                var isFraction = war.Type == WarType.Gangs || war.Type == WarType.Mafia;

                var draw = war.AttackingCount == war.ProtectingCount;
                var victoryAttackers = war.AttackingCount > war.ProtectingCount || war.ProtectingId == 0;

                if (draw && war.ProtectingPlayersInZoneCount == 0 && war.AttackingPlayersInZoneCount > 0)
                    victoryAttackers = true;

                var victoryId = victoryAttackers ? war.AttackingId : war.ProtectingId;

                foreach (var foreachPlayer in NeptuneEvo.Character.Repository.GetPlayers())
                {
                    var foreachSessionData = foreachPlayer.GetSessionData();
                    if (foreachSessionData == null) 
                        continue;
                    
                    var foreachWarData = foreachSessionData.WarData;
                    if (foreachWarData.IsWarZone && warId == foreachWarData.WarId)
                    {
                        ExitZone(foreachPlayer, war);
                        continue;
                    }
                    
                    if (isFraction)
                    {
                        var foreachFractionId = foreachPlayer.GetFractionId();
                        if (foreachFractionId == (int) Fractions.Models.Fractions.None)
                            continue;
                            
                        if (foreachFractionId != war.AttackingId && foreachFractionId != war.ProtectingId)
                            continue;
                    }
                    else
                    {
                        var foreachOrganizationMemberData = foreachPlayer.GetOrganizationMemberData();
                        if (foreachOrganizationMemberData == null)
                            continue;
                            
                        if (foreachOrganizationMemberData.Id != war.AttackingId && foreachOrganizationMemberData.Id != war.ProtectingId)
                            continue;
                    }
                        
                    Trigger.ClientEvent(foreachPlayer, "destroyWarZone", war.Id);

                    if (foreachSessionData.IsWarMarker)
                    {
                        foreachSessionData.IsWarMarker = false;
                        foreachPlayer.SetSharedData("warType", 0);
                    }
                }
                
                switch (war.Type)
                {
                    case WarType.OrgWarZone:
                        if (draw)
                        {
                            Fractions.Manager.SendCoolOrganizationMsg(war.ProtectingId,"Война", "Ничья", "Вы сыграли в ничью.", "", 10000);   
                            Fractions.Manager.SendCoolOrganizationMsg(war.AttackingId,"Война", "Ничья", "Вы сыграли в ничью.", "", 10000); 
                            //Fractions.Manager.sendOrganizationMessage(war.AttackingId, "Вы сыграли в ничью.");
                            //Fractions.Manager.sendOrganizationMessage(war.ProtectingId, "Вы сыграли в ничью.");
                        }
                        else if (victoryAttackers)
                        {
                            Fractions.Manager.SendCoolOrganizationMsg(war.ProtectingId,"Война", "Поражение", "Вы проиграли войну и потеряли территорию :(", "", 10000);   
                            Fractions.Manager.SendCoolOrganizationMsg(war.AttackingId,"Война", "Победа", "Вы победили! Территория переходит под ваш контроль, поздравляем!", "", 10000); 
                            //Fractions.Manager.sendOrganizationMessage(war.AttackingId, "Вы победили! Территория переходит под ваш контроль, поздравляем!");
                           // Fractions.Manager.sendOrganizationMessage(war.ProtectingId, "Вы проиграли войну и потеряли территорию :(");
                        }
                        else
                        {
                            Fractions.Manager.SendCoolOrganizationMsg(war.ProtectingId,"Война", "Победа", "Вы удержали территорию! Поздравляем!", "", 10000);   
                            Fractions.Manager.SendCoolOrganizationMsg(war.AttackingId,"Война", "Поражение", "Вы проиграли! Территория остаётся у стороны защиты.", "", 10000); 
                            //Fractions.Manager.sendOrganizationMessage(war.AttackingId, "Вы проиграли! Территория остаётся у стороны защиты.");
                            //Fractions.Manager.sendOrganizationMessage(war.ProtectingId, "Вы удержали территорию! Поздравляем!");
                        }
                        break;
                    
                    case WarType.Gangs:
                        if (victoryAttackers)
                        {
                            Fractions.Manager.SendCoolFractionMsg(war.ProtectingId,"Капт", "Поражение", LangFunc.GetText(LangType.Ru, DataName.CaptureLoseByDef), "", 10000);   
                            Fractions.Manager.SendCoolFractionMsg(war.AttackingId,"Капт", "Победа", LangFunc.GetText(LangType.Ru, DataName.CaptureWinByAttack), "", 10000); 
                            //Fractions.Manager.sendFractionMessage(war.ProtectingId, LangFunc.GetText(LangType.Ru, DataName.CaptureLoseByDef));
                            //Fractions.Manager.sendFractionMessage(war.AttackingId, LangFunc.GetText(LangType.Ru, DataName.CaptureWinByAttack));
                        }
                        else
                        {
                            Fractions.Manager.SendCoolFractionMsg(war.ProtectingId,"Капт", "Победа", LangFunc.GetText(LangType.Ru, DataName.CaptureWinByDef), "", 10000);   
                            Fractions.Manager.SendCoolFractionMsg(war.AttackingId,"Капт", "Поражение", LangFunc.GetText(LangType.Ru, DataName.CaptureLoseByAttack), "", 10000);   
                            //Fractions.Manager.sendFractionMessage(war.ProtectingId, LangFunc.GetText(LangType.Ru, DataName.CaptureWinByDef));
                           // Fractions.Manager.sendFractionMessage(war.AttackingId, LangFunc.GetText(LangType.Ru, DataName.CaptureLoseByAttack));
                        }
                        
                        foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                        {
                            var foreachSessionData = foreachPlayer.GetSessionData();
                            if (foreachSessionData == null) continue;
                        
                            var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                            if (foreachMemberFractionData == null) continue;
                        
                            if (foreachMemberFractionData.Id == victoryId)
                            {
                                MoneySystem.Wallet.Change(foreachPlayer, Main.CaptureWin);
                                GameLog.Money($"server", $"player({foreachPlayer.GetUUID()})", Main.CaptureWin, $"winCapture");
                            }
                        
                            foreachSessionData.UsedTPOnCaptureOrBizwar = 0;
                        }
                        break;
                    
                    case WarType.Mafia:
                        if (victoryAttackers)
                        {
                            Fractions.Manager.sendFractionMessage(war.ProtectingId, LangFunc.GetText(LangType.Ru, DataName.DefLoseBizWar));
                            Fractions.Manager.sendFractionMessage(war.AttackingId, LangFunc.GetText(LangType.Ru, DataName.AttackWinBizWar));

                        }
                        else
                        {
                            Fractions.Manager.sendFractionMessage(war.ProtectingId, LangFunc.GetText(LangType.Ru, DataName.DefWinBizWar));
                            Fractions.Manager.sendFractionMessage(war.AttackingId, LangFunc.GetText(LangType.Ru, DataName.AttackLoseBizWar));
                        }
                        
                        foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                        {
                            var foreachSessionData = foreachPlayer.GetSessionData();
                            if (foreachSessionData == null) continue;
                        
                            var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                            if (foreachMemberFractionData == null) continue;
                        
                            if (foreachMemberFractionData.Id == victoryId)
                            {
                                MoneySystem.Wallet.Change(foreachPlayer, Main.BizwarWin);
                                GameLog.Money($"server", $"player({foreachPlayer.GetUUID()})", Main.BizwarWin, $"winBiz");
                            }
                        
                            foreachSessionData.UsedTPOnCaptureOrBizwar = 0;
                        }
                        break;
                }


                //if (victoryAttackers)
                //{
                switch (war.Type)
                {
                    case WarType.OrgWarZone:
                        Organizations.FamilyZones.Repository.Update((byte) war.ObjectId, victoryId);
                        break;
                    case WarType.Gangs:
                        Fractions.GangsCapture.Update((byte) war.ObjectId, victoryId, war.AttackingId, war.ProtectingId);
                        break;
                    case WarType.Mafia:
                        Fractions.MafiaWars.Update((byte) war.ObjectId, victoryId, war.MapId, war.AttackingId, war.ProtectingId);
                        break;
                }
                //}
                
                
                Wars.TryRemove(warId, out _);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        public static void Unix()
        {
            try
            {
                foreach (var id in Wars.Keys.ToList())
                {
                    UpdateWar(id);
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
    }
}