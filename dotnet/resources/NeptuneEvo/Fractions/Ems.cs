using System.Collections.Generic;
using GTANetworkAPI;
using NeptuneEvo.Core;
using Redage.SDK;
using System;
using NeptuneEvo.GUI;
using System.Linq;
using Localization;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.VehicleModel;
using NeptuneEvo.Functions;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Handles;
using NeptuneEvo.Players.Phone.Messages.Models;
using NeptuneEvo.Players.Popup.List.Models;
using NeptuneEvo.Quests;
using NeptuneEvo.Quests.Models;
using NeptuneEvo.Table.Models;
using NeptuneEvo.Table.Tasks.Models;
using NeptuneEvo.Table.Tasks.Player;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;
using Newtonsoft.Json;

namespace NeptuneEvo.Fractions
{
    class Ems : Script
    {
        private static readonly nLog Log = new nLog("Fractions.Ems");

        public static Dictionary<ExtPlayer, Blip> EMSCalls = new Dictionary<ExtPlayer, Blip>();//Todo

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                NAPI.TextLabel.CreateTextLabel("~w~Donna Hart", new Vector3(322.785, -586.43, 44.35), 8f, 0.3f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension);                
                NAPI.TextLabel.CreateTextLabel("~w~Gregory Parks", new Vector3(332.85, -594.66, 44.35), 8f, 0.3f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension);

                #region cols
                CustomColShape.CreateCylinderColShape(emsCheckpoints[3], 1, 2, 0, ColShapeEnums.FractionEms, 1); // open ho spital stock
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Открыть склад"), new Vector3(emsCheckpoints[3].X, emsCheckpoints[3].Y, emsCheckpoints[3].Z + 0.3), 5F, 0.3F, 0, new Color(255, 255, 255));

                CustomColShape.CreateCylinderColShape(emsCheckpoints[4], 1, 2, 0, ColShapeEnums.FractionEms, 2); // duty change
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(emsCheckpoints[4].X, emsCheckpoints[4].Y, emsCheckpoints[4].Z + 0.3), 5F, 0.3F, 0, new Color(255, 255, 255));



                CustomColShape.CreateCylinderColShape(emsCheckpoints[6], 1, 2, 0, ColShapeEnums.FractionEms, 4); // tattoo delete
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Удаление татуировки"), new Vector3(emsCheckpoints[6].X, emsCheckpoints[6].Y, emsCheckpoints[6].Z + 0.3), 5F, 0.3F, 0, new Color(255, 255, 255));

                #region Load Medkits
                CustomColShape.CreateCylinderColShape(new Vector3(-53.072685, -2416.0156, 6.000165), 4, 5, 0, ColShapeEnums.FractionEms, 5); // take meds
                NAPI.Marker.CreateMarker(1, new Vector3(-53.072685, -2416.0156, 3.000165), new Vector3(), new Vector3(), 4, new Color(255, 0, 0));

                CustomColShape.CreateCylinderColShape(new Vector3(-34.496403, -2419.0205, 5.994418), 4, 5, 0, ColShapeEnums.FractionEms, 5); // take meds
                NAPI.Marker.CreateMarker(1, new Vector3(-34.496403, -2419.0205, 2.994418), new Vector3(), new Vector3(), 4, new Color(255, 0, 0));
                Main.CreateBlip(new Main.BlipData(499, "Humane Labs", new Vector3(-53.072685, -2416.0156, 6.000165), 4, true));
                #endregion

                CustomColShape.CreateCylinderColShape(emsCheckpoints[7], 1, 2, 0, ColShapeEnums.FractionEms, 6); // roof
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~В больницу"), new Vector3(emsCheckpoints[7].X, emsCheckpoints[7].Y, emsCheckpoints[7].Z + 0.3), 5F, 0.3F, 0, new Color(255, 255, 255));

                CustomColShape.CreateCylinderColShape(emsCheckpoints[8], 1, 2, 0, ColShapeEnums.FractionEms, 6); // to roof
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~На крышу"), new Vector3(emsCheckpoints[8].X, emsCheckpoints[8].Y, emsCheckpoints[8].Z + 0.3), 5F, 0.3F, 0, new Color(255, 255, 255));

                CustomColShape.CreateCylinderColShape(emsCheckpoints[9], 1, 2, 0, ColShapeEnums.FractionEms, 7); // roof

                CustomColShape.CreateCylinderColShape(emsCheckpoints[10], 1, 2, 0, ColShapeEnums.FractionEms, 7); // roof
                #endregion

                for (int i = 3; i < 11; i++)
                {
                    if (emsCheckpoints[i] == new Vector3()) continue;
                    Marker marker = NAPI.Marker.CreateMarker(1, emsCheckpoints[i] - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220));
                }
                
                PedSystem.Repository.CreateQuest("s_m_m_scientist_01", new Vector3(315.63, -585.912, 43.26), 75.82f, title: "~y~NPC~w~ Эммануэль\nДежурный врач", colShapeEnums: ColShapeEnums.FracEms);
                PedSystem.Repository.CreateQuest("s_m_y_autopsy_01", new Vector3(318.3019, -590.71857, 43.266674), 164.38774f, title: "~y~NPC~w~ Михаил\nВетеринар", colShapeEnums: ColShapeEnums.FracEmsVeterinarian);
                PedSystem.Repository.CreateQuest("s_m_m_doctor_01", new Vector3(315.7822, -588.0856, 43.261223), 107.24092f, title: "~y~NPC~w~ Мед.брат Станислав\nВызвать сотрудника", colShapeEnums: ColShapeEnums.CallEmsMember);
                PedSystem.Repository.CreateQuest("a_f_y_business_04", new Vector3(-1289.9895, -572.3277, 30.572992), -45.4007f, title: "~y~NPC~w~ Сара Каримова\nВызвать сотрудника", colShapeEnums: ColShapeEnums.CallGovMember);
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }

        public static Vector3[] emsCheckpoints = new Vector3[11]
        {
            new Vector3(297.672, -583.996, 43.26), // ems blip       0
            new Vector3(336.80, -574.18, 42.76), // spawn after death      1
            new Vector3(347.141, -576.04, 42.76), // spawn after death      2
            new Vector3(322.8342, -587.07, 42.76), // open hospital stock   3
            new Vector3(324.593, -593.27, 42.76), // duty change            4
            new Vector3(), // start heal course      5
            new Vector3(332.703, -593.75, 42.76), // tattoo delete          6
            new Vector3(339.0645, -584.0509, 73.04563), // метка на крыше для тп во внутрь 7
            new Vector3(334.643, -580.83, 47.84), // to roof                8
            new Vector3(346.2935, -597.68, 42.76), // выход на улицу к каретам 9
            new Vector3(319.4704, -559.7971, 27.62377), // метка с улицы тп назад 10
        };

        public static string OnCallEms(ExtPlayer player, bool death = false)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return "Что то не так :-(";
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return "Что то не так :-(";
                
                if (EMSCalls.ContainsKey(player)) 
                    return "Вы уже сделали вызов";
                
                if (!death)
                {
                    if (Manager.FractionMembersCount((int) Models.Fractions.EMS) == 0)
                        return LangFunc.GetText(LangType.Ru, DataName.NoMedicsNear);
                    
                    if (characterData.AdminLVL == 0 && DateTime.Now < sessionData.TimingsData.NextCallEMS)
                        return LangFunc.GetText(LangType.Ru, DataName.AlreadyCallMedic);
                    
                    sessionData.TimingsData.NextCallEMS = DateTime.Now.AddMinutes(7);
                }
                else
                {
                    if (characterData.InsideHouseID != -1 || characterData.InsideGarageID != -1 || characterData.InsideOrganizationID != -1) 
                        return "Вы не можете вызвать сюда медиков";
                }

                Blip EMSBlip = NAPI.Blip.CreateBlip(0, player.Position, 1, 70, $"EMS Call from [{player.Value}]", 0, 0, false, 0, NAPI.GlobalDimension);
                NAPI.Blip.SetBlipTransparency(EMSBlip, 0);
                EMSCalls.Add(player, EMSBlip);
                foreach (var foreachPlayer in Character.Repository.GetPlayers())
                {
                    var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                    if (foreachMemberFractionData == null) 
                        continue;
                    
                    if (foreachMemberFractionData.Id != (int) Models.Fractions.EMS) 
                        continue;
                    
                    Trigger.ClientEvent(foreachPlayer, "changeBlipAlpha", EMSBlip, 255);
                    Notify.Send(foreachPlayer, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MedicCallFrom, player.Name, player.Value), 5000);
                    NAPI.Chat.SendChatMessageToPlayer(foreachPlayer, "!{#F08080}" + LangFunc.GetText(LangType.Ru, DataName.MedicCallFromDistance, player.Name, player.Value, Math.Round(player.Position.DistanceTo(foreachPlayer.Position), 2)));
                }

                return LangFunc.GetText(LangType.Ru, DataName.SuccessCallMedic);
            }
            catch (Exception e)
            {
                Log.Write($"OnCallEms Exception: {e.ToString()}");
            }
            return "Вы уже сделали вызов";
        }

        public static void acceptCall(ExtPlayer player, ExtPlayer target)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (!target.IsCharacterData()) return;
                if (!player.IsFractionAccess(RankToAccess.Ems)) return;
                if (!EMSCalls.ContainsKey(target))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DoesntCallEmsOrAlready), 3000);
                    return;
                }
                var blip = EMSCalls[target];
                if (blip != null)
                {
                    Trigger.ClientEvent(player, "createWaypoint", blip.Position.X, blip.Position.Y);
                    if (blip.Exists) 
                        blip.Delete();
                }
                EMSCalls.Remove(target);
                Manager.sendFractionMessage((int) Models.Fractions.EMS, LangFunc.GetText(LangType.Ru, DataName.CallAccept, player.Name.Replace("_"," "), target.Value));
                Manager.sendFractionMessage((int) Models.Fractions.EMS, "!{#F08080}[F] " + LangFunc.GetText(LangType.Ru, DataName.CallAccept, player.Name.Replace("_"," "), target.Value), true);
                Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCallAccepted, player.Value), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"acceptCall Exception: {e.ToString()}");
            }
        }

        public static void onPlayerDisconnectedhandler(ExtPlayer player, DisconnectionType type, string reason)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (sessionData.TimersData.HealTimer != null)
                {
                    Timers.Stop(sessionData.TimersData.HealTimer);
                    sessionData.TimersData.HealTimer = null;
                }
                if (sessionData.TimersData.DeathTimer != null)
                {
                    Timers.Stop(sessionData.TimersData.DeathTimer);
                    sessionData.TimersData.DeathTimer = null;
                }
                if (EMSCalls.ContainsKey(player))
                {
                    var blip = EMSCalls[player];
                    if (blip != null && blip.Exists) 
                        blip.Delete();
                    EMSCalls.Remove(player);
                    Manager.sendFractionMessage((int) Models.Fractions.EMS, LangFunc.GetText(LangType.Ru, DataName.CallCancel, player.Value));
                }
            }
            catch (Exception e)
            {
                Log.Write($"onPlayerDisconnectedhandler Exception: {e.ToString()}");
            }
        }

        [ServerEvent(Event.PlayerDeath)]
        public void OnPlayerDeathHandler(ExtPlayer player, ExtPlayer entityKiller, uint weapon)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                sessionData.PositionCaptureOrBizwar = null;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (sessionData.KilledData.Killed != null)
                {
                    entityKiller = sessionData.KilledData.Killed;
                    weapon = sessionData.KilledData.Weapon;
                    sessionData.KilledData.Killed = null;
                }
                
                Admin.onPlayerDeathHandler(player, entityKiller, weapon, player.Position);

                if (Attachments.HasAttachment(player, Attachments.AttachmentsName.Binoculars))
                {
                    Trigger.ClientEvent(player, "binoculars.stop");
                    Attachments.RemoveAttachment(player, Attachments.AttachmentsName.Binoculars);
                }

                try
                {
                    if (entityKiller.IsCharacterData() && Events.AirDrop.Repository.IsPlayerToEvents(player) && Events.AirDrop.Repository.IsPlayerToEvents(entityKiller))
                    {
                        Events.AirDrop.Repository.AirDropDeathHandler(player, entityKiller);
                    }
                }
                catch (Exception e)
                {
                    Log.Write($"onPlayerDeathHandler - AirDropTeamsInfoUpdate Exception: {e.ToString()}");
                }

                var killerCharacterData = entityKiller.GetCharacterData();
                var killerSessionData = entityKiller.GetSessionData();

                try
                {
                    if (killerCharacterData != null && killerSessionData != null && !Voice.Voice.isGov(entityKiller.GetFractionId()) && player != entityKiller)
                    {
                        if ((!entityKiller.HasSharedData("IS_MASK") || !entityKiller.GetSharedData<bool>("IS_MASK")) && (killerSessionData.InAirsoftLobby == -1) && killerSessionData.InTanksLobby == -1 && !killerSessionData.WarData.IsWarZone)
                        {
                            byte correctLevel = 2;
                            if (killerCharacterData.WantedLVL != null)
                            {
                                if (killerCharacterData.WantedLVL.Level + 2 >= 6) correctLevel = 6;
                                else correctLevel = (byte)(killerCharacterData.WantedLVL.Level + 2);
                            }

                            WantedLevel wantedLevel = new WantedLevel(correctLevel, LangFunc.GetText(LangType.Ru, DataName.Police), DateTime.Now, LangFunc.GetText(LangType.Ru, DataName.Murder));
                            Police.setPlayerWantedLevel(entityKiller, wantedLevel);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Write($"onPlayerDeathHandler - setPlayerWantedLevel Exception: {e.ToString()}");
                }

                if (sessionData.InTanksLobby > -1)
                {
                    Events.TankRoyale.DeathCheck(player);
                    return;
                }

                if (sessionData.InAirsoftLobby >= 0)
                {
                    if (player.GetSharedData<int>("PlayerAirsoftTeam") == 0 || player.GetSharedData<int>("PlayerAirsoftTeam") == 1 || Events.Airsoft.AirsoftPlayerData.ContainsKey(player) && Events.Airsoft.AirsoftPlayerData[player].IsGunGamePlayer == true)
                    {
                        Events.Airsoft.DeathCheck(player, entityKiller, weapon);
                        return;
                    }
                    else
                    {
                        Events.Airsoft.DeathCheck(player, entityKiller, weapon);
                    }
                }
                if (EventSys.AdminEvent.EventState != 0 && EventSys.ExitFromMP(player))
                {                    
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MPfail), 7000);
                    return;
                }
                if (CarRoom.OnExitTestDrive (player, isDeath: true)) // Если вдруг что-то пойдёт не так и игрок в тест-драйве умрёт с GM'ом каким-то чудом.
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TestDriveExpired), 5000);
                    return;
                }
                
                Chars.Repository.Event_PlayerDeath(player);
                
                if (World.War.Repository.OnPlayerDeath(player, entityKiller, weapon))
                    return;
                
                FractionCommands.onPlayerDeathHandler(player, entityKiller, weapon);
                SafeMain.onPlayerDeathHandler(player, entityKiller, weapon);
                Army.Event_PlayerDeath(player, entityKiller, weapon);
                Police.Event_PlayerDeath(player, entityKiller, weapon);
                Houses.HouseManager.Event_OnPlayerDeath(player, entityKiller, weapon);
                //Jobs.Collector.Event_PlayerDeath(player, entityKiller, weapon);
                //Jobs.Gopostal.Event_PlayerDeath(player, entityKiller, weapon);
                EventSys.Event_PlayerDeath(player, entityKiller, weapon);
                characterData.IsAlive = false;

                Trigger.ClientEvent(player, "UpdateTime");

                if (characterData.DemorganTime >= 1 || characterData.ArrestTime >= 1)
                {
                    sessionData.DeathData.IsDying = true;
                    ReviveFunc(player);
                }
                else
                {
                    //
                    NAPI.Player.SpawnPlayer(player, player.Position);
                    /*NAPI.Task.Run(() =>
                    {
                        try
                        {
                            if (!player.IsCharacterData()) return;
                            Trigger.PlayAnimation(player, "dead", deadAnims[Main.rnd.Next(8)], 39);
                            // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "dead");
                        }
                        catch (Exception e)
                        {
                            Log.Write($"DeathConfirm Task Exception: {e.ToString()}");
                        }
                    }, 500);*/
                    //
                    if (!sessionData.DeathData.IsDying)
                    {
                        DeathConfirm(player);

                        //var fracId = player.GetFractionId();
                        
                        /*if ((Manager.FractionTypes[fracId] == FractionsType.Mafia && (MafiaWars.warIsGoing || MafiaWars.warStarting)) ||
                            (Manager.FractionTypes[fracId] == FractionsType.Gangs && (GangsCapture.captureIsGoing || GangsCapture.captureStarting)))
                        {
                            sessionData.DeathData.InDeath = true;
                            player.SetSharedData("InDeath", true);
                        }
                        else
                        {*/
                            sessionData.DeathData.InDeath = true;
                            player.SetSharedData("InDeath", true);
                            int medics = Manager.FractionMembersCount ((int) Models.Fractions.EMS);

                            if (entityKiller.IsCharacterData() && entityKiller != player) Trigger.ClientEvent(player, "openHospitalDialog", LangFunc.GetText(LangType.Ru, DataName.YouAreDeadFromPlayer, entityKiller.Value, medics));
                            else Trigger.ClientEvent(player, "openHospitalDialog", LangFunc.GetText(LangType.Ru, DataName.YouAreDead, medics));
                        //}
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"onPlayerDeathHandler Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server:OnHospitalDialogCallback")]
        public void OnHospitalDialogCallback(ExtPlayer player, int state)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                if (!sessionData.DeathData.IsDying) return;
                
                if (!player.IsCharacterData()) return;
                
                if (sessionData.TimersData.DeathTimer != null)
                {
                    Timers.Stop(sessionData.TimersData.DeathTimer);
                    sessionData.TimersData.DeathTimer = null;
                }
                
                //Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.InfoDead), 15000) //old

                int reviveTimerMS = 300000;

                if (state == 1)
                {
                    Commands.RPChat("sb", player, LangFunc.GetText(LangType.Ru, DataName.BolnicaAdmNearWait));
                }
                
                if (state == 2)
                {
                    OnCallEms(player, true);
                    reviveTimerMS = 600000;
                    Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Ems,LangFunc.GetText(LangType.Ru, DataName.SuccessCallMedic), DateTime.Now);
                    Commands.RPChat("sb", player, LangFunc.GetText(LangType.Ru, DataName.BolnicaAdmNearVrach));
                }
                else if (state == 3)
                {
                    reviveTimerMS = 60000;
                    //Trigger.SendToAdmins(1, LangFunc.GetText(LangType.Ru, DataName.BolnicaAdm, player.Name, player.Value));
                    Commands.RPChat("sb", player, LangFunc.GetText(LangType.Ru, DataName.BolnicaAdmNear));
                }
                
                Trigger.ClientEvent(player, "DeathTimer", reviveTimerMS);
                sessionData.TimersData.DeathTimer = Timers.StartOnce(reviveTimerMS, () => ReviveFunc(player), true);
            }
            catch (Exception e)
            {
                Log.Write($"OnHospitalDialogCallback Exception: {e.ToString()}");
            }
        }

        public static void DeathConfirm(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                Main.OnAntiAnim(player);
                player.SetSharedData("vmuted", true);
                sessionData.DeathData.IsDying = true;
                sessionData.DeathData.IsReviving = false;
                characterData.IsAlive = false;
                characterData.CurPosition = (characterData.Gender == true) ? emsCheckpoints[1] : emsCheckpoints[2];
                Players.Phone.Call.Repository.OnPut(player);
                if (sessionData.TimersData.DeathTimer != null)
                {
                    Timers.Stop(sessionData.TimersData.DeathTimer);
                    sessionData.TimersData.DeathTimer = null;
                }
                //NAPI.Player.SpawnPlayer(player, player.Position);
                sessionData.TimersData.DeathTimer = Timers.StartOnce(60000, () => ReviveFunc(player), true);
                Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.InfoDead), 20000);
                Trigger.ClientEvent(player, "DeathTimer", 60000);
            }
            catch (Exception e)
            {
                Log.Write($"DeathConfirm Exception: {e.ToString()}");
            }
        }

        public static void ReviveFunc(ExtPlayer player, bool reviveonplace = false)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                if (!sessionData.DeathData.IsDying) return;

                if (sessionData.TimersData.DeathTimer != null)
                {
                    Timers.Stop(sessionData.TimersData.DeathTimer);
                    sessionData.TimersData.DeathTimer = null;
                }
                if (sessionData.DeathData.IsReviving) return;
                sessionData.DeathData.IsDying = false;
                if (EMSCalls.ContainsKey(player))
                {
                    var blip = EMSCalls[player];
                    if (blip != null && blip.Exists) 
                        blip.Delete();
                    
                    EMSCalls.Remove(player);
                }
                Trigger.ClientEvent(player, "freeze", false);
                Trigger.ClientEvent(player, "DeathTimer", false);
                player.SetSharedData("InDeath", false);
                Vector3 spawnPos;
                if (!reviveonplace)
                {
                    var spawnDimension = 0;
                    var fracId = player.GetFractionId();
                    if (characterData.DemorganTime >= 1)
                    {
                        spawnPos = Admin.DemorganPositions[Main.rnd.Next(55)] + new Vector3(0, 0, 1.12);
                        player.SetSkin(Admin.DemorganSkins[Main.rnd.Next(14)]);
                    }
                    else if (characterData.ArrestTime >= 1)
                    {
                        if (characterData.ArrestType == 1) spawnPos = Sheriff.FirstPrisonPosition;
                        else if (characterData.ArrestType == 2) spawnPos = Sheriff.SecondPrisonPosition;
                        else spawnPos = Police.PrisonPosition;
                    }
                    else if (fracId == (int) Models.Fractions.ARMY) 
                    {
                        spawnPos = Manager.FractionSpawns[14] + new Vector3(0, 0, 1.12);
                        spawnDimension = 3244522;
                    }
                    else if (fracId == (int) Models.Fractions.SHERIFF) spawnPos = Manager.FractionSpawns[18] + new Vector3(0, 0, 1.12);
                    else if (fracId == (int) Models.Fractions.FIB) spawnPos = Manager.FractionSpawns[(int) Models.Fractions.FIB] + new Vector3(0, 0, 1.12);
                    else spawnPos = (characterData.Gender == true) ? emsCheckpoints[1] : emsCheckpoints[2];
                    characterData.InCasino = false;
                    characterData.InsideGarageID = -1;
                    characterData.InsideHotelID = -1;
                    characterData.InsideOrganizationID = -1;
                    characterData.InsideHouseID = -1;
                    Trigger.Dimension(player, (uint)spawnDimension);

                    if (sessionData.CuffedData.Cuffed)
                    {
                        FractionCommands.unCuffPlayer(player, false);
                        sessionData.CuffedData.CuffedByCop = false;
                        sessionData.CuffedData.CuffedByMafia = false;
                    }

                }
                else spawnPos = player.Position + new Vector3(0, 0, 0.5);
                characterData.CurPosition = spawnPos;
                NAPI.Player.SpawnPlayer(player, spawnPos);
                Trigger.StopAnimation(player);
                if (characterData.DemorganTime <= 0) NAPI.Player.SetPlayerHealth(player, 20);
                else NAPI.Player.SetPlayerHealth(player, 3);
                characterData.IsAlive = true;
                sessionData.DeathData.InDeath = false;
                if (characterData.DemorganTime <= 0 && characterData.Unmute <= 0) player.SetSharedData("vmuted", false);
                Main.OffAntiAnim(player);
            }
            catch (Exception e)
            {
                Log.Write($"ReviveFunc Exception: {e.ToString()}");
            }
        }

        public static void payMedkit(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (sessionData.SellItemData.Buyer != player) return;
                var target = sessionData.SellItemData.Seller;
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                int price = sessionData.SellItemData.Price;
                if (!target.IsCharacterData() || player.Position.DistanceTo(target.Position) > 2)
                {
                    sessionData.SellItemData = new SellItemData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouFarFromSeller), 3000);
                    return;
                }
                if (UpdateData.CanIChange(player, sessionData.SellItemData.Price, true) != 255)
                {
                    targetSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    return;
                }
                ItemStruct item = Chars.Repository.isItem(target, "inventory", ItemId.HealthKit);
                if (item == null || item.Item.Count < 1)
                {
                    targetSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MedicNoApteka), 3000);
                    return;
                }
                if (Chars.Repository.isFreeSlots(player, ItemId.HealthKit, 1) != 0)
                {
                    targetSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    return;
                }
                Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.HealthKit);
                Chars.Repository.RemoveIndex(target, item.Location, item.Index, 1);

                var fractionData = Manager.GetFractionData((int) Models.Fractions.CITY);
                if (fractionData != null)
                    fractionData.Money += Convert.ToInt32(price * 0.85);
                
                MoneySystem.Wallet.Change(player, -price);
                MoneySystem.Wallet.Change(target, Convert.ToInt32(price * 0.15));
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouBuyApteka), 3000);
                Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouSellApteka, player.Value), 3000);
                targetSessionData.SellItemData = new SellItemData();
                sessionData.SellItemData = new SellItemData();
            }
            catch (Exception e)
            {
                Log.Write($"payMedkit Exception: {e.ToString()}");
            }
        }

        public static void payHeal(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return; 
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                var target = sessionData.SellItemData.Seller;
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null) return;
                int price = sessionData.SellItemData.Price;
                if (!target.IsCharacterData() || player.Position.DistanceTo(target.Position) > 2)
                {
                    sessionData.SellItemData = new SellItemData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouFarFromSeller), 3000);
                    return;
                }
                if (UpdateData.CanIChange(player, sessionData.SellItemData.Price, true) != 255)
                {
                    targetSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    return;
                }
                if (NAPI.Player.IsPlayerInAnyVehicle(target) && NAPI.Player.IsPlayerInAnyVehicle(player))
                {
                    var pveh = (ExtVehicle)target.Vehicle;
                    var tveh = (ExtVehicle)player.Vehicle;
                    if (pveh != tveh)
                    {
                        targetSessionData.SellItemData = new SellItemData();
                        sessionData.SellItemData = new SellItemData();
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerInOtherVehicle), 3000);
                        return;
                    }
                    var vehicleLocalData = pveh.GetVehicleLocalData();
                    if (vehicleLocalData != null)
                    {
                        if (vehicleLocalData.Access == VehicleAccess.Fraction && vehicleLocalData.Fraction == (int) Models.Fractions.EMS && (pveh.Model == NAPI.Util.GetHashKey("emsnspeedo") || pveh.Model == NAPI.Util.GetHashKey("emsroamer") || pveh.Model == NAPI.Util.GetHashKey("vapidse") || pveh.Model == (uint)VehicleHash.Ambulance || pveh.Model == (uint)VehicleHash.Frogger || pveh.Model == (uint)VehicleHash.Supervolito || pveh.Model == (uint)VehicleHash.Maverick || pveh.Model == (uint)VehicleHash.Lguard))
                        {
                            Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHealSomeone, player.Value), 4000);
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomeoneHealYou, target.Value), 4000);
                            Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");
                            NAPI.Player.SetPlayerHealth(player, 100);
                            MoneySystem.Wallet.Change(player, -price);
                            MoneySystem.Wallet.Change(target, price);
                            GameLog.Money($"player({characterData.UUID})", $"player({targetCharacterData.UUID})", price, $"payHeal");
                            targetSessionData.SellItemData = new SellItemData();
                            sessionData.SellItemData = new SellItemData();
                        }
                        else
                        {
                            targetSessionData.SellItemData = new SellItemData();
                            sessionData.SellItemData = new SellItemData();
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NotEmsCar), 3000);
                            return;
                        }
                    }
                    return;
                }
                if (SafeZones.IsSafeZone(sessionData.InsideSafeZone, SafeZones.ZoneName.EMS) && SafeZones.IsSafeZone(targetSessionData.InsideSafeZone, SafeZones.ZoneName.EMS))
                {
                    BattlePass.Repository.UpdateReward(player, 15);
                    BattlePass.Repository.UpdateReward(player, 98);
                    Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHealSomeone, player.Value), 4000);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomeoneHealYou, target.Value), 4000);
                    target.AddTableScore(TableTaskId.Item20);
                    NAPI.Player.SetPlayerHealth(player, 100);
                    MoneySystem.Wallet.Change(player, -price);
                    MoneySystem.Wallet.Change(target, price);
                    GameLog.Money($"player({characterData.UUID})", $"player({targetCharacterData.UUID})", price, $"payHeal");
                    Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");
                    targetSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    return;
                }
                targetSessionData.SellItemData = new SellItemData();
                sessionData.SellItemData = new SellItemData();
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NotEmsCar), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"payHeal Exception: {e.ToString()}");
            }
        }
        
        [Interaction(ColShapeEnums.CallEmsMember)] 
        public static void OpenCallEmsMemberDialog(ExtPlayer player, int _) 
        { 
            try 
            { 
                var sessionData = player.GetSessionData(); 
                if (sessionData == null) return; 
 
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
 
                Trigger.ClientEvent(player, "openDialog", "CallEmsMemberDialog", LangFunc.GetText(LangType.Ru, DataName.AreYouWantToCallGov)); 
            } 
            catch (Exception e) 
            { 
                Log.Write($"OpenCallEmsMemberDialog Exception: {e.ToString()}"); 
            } 
        }

        [Interaction(ColShapeEnums.FracEms)]
        public static void Open(ExtPlayer player, int index)
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
            else if (sessionData.DeathData.InDeath)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsDying), 3000);
                return;
            }
            else if (Main.IHaveDemorgan(player, true)) return;
            
            BattlePass.Repository.UpdateReward(player, 85);
            player.SelectQuest(new PlayerQuestModel("npc_fracems", 0, 0, false, DateTime.Now));
            Trigger.ClientEvent(player, "client.quest.open", index, "npc_fracems", 0, 0, 0);
        }
        [Interaction(ColShapeEnums.FracEmsVeterinarian)]
        public static void OpenVeterinarian(ExtPlayer player, int index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;

            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

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


            player.SelectQuest(new PlayerQuestModel("npc_pet", 0, 0, false, DateTime.Now));
            Trigger.ClientEvent(player, "client.quest.open", index, "npc_pet", 0, 0, 0);
        }
        public static void Perform(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            if (NAPI.Player.GetPlayerHealth(player) > 99)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoNeedHeal), 3000);
                return;
            }
            if (sessionData.TimersData.HealTimer != null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyHealing), 3000);
                return;
            }
            //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.HealStarted), 7000);
           EventSys.SendCoolMsg(player,"Больница", "Лечение", $"{LangFunc.GetText(LangType.Ru, DataName.HealStarted)}", "", 5000); 
           BattlePass.Repository.UpdateReward(player, 15);
            BattlePass.Repository.UpdateReward(player, 98);
            sessionData.TimersData.HealTimer = Timers.Start(3750, () => healTimer(player), true);
        }
        [Interaction(ColShapeEnums.FractionEms)]
        public static void OnFractionEms(ExtPlayer player, int interact)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var memberFractionData = player.GetFractionMemberData();
                var fractionData = Manager.GetFractionData((int) Models.Fractions.EMS);
                switch (interact)
                {
                    case 1:
                        if (memberFractionData == null || memberFractionData.Id != (int) Models.Fractions.EMS)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NotEMS), 3000);
                            return;
                        }
                        if (!sessionData.WorkData.OnDuty)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000);
                            return;
                        }
                        if (fractionData == null || !fractionData.IsOpenStock)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseClosed), 3000);
                            return;
                        }
                        OpenHospitalStockMenu(player);
                        return;
                    case 2:
                        if (memberFractionData != null && memberFractionData.Id == (int) Models.Fractions.EMS) FractionClothingSets.OpenFractionClothingSetsMenu(player);
                        else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NotEMS), 3000);
                        return;
                    /*case 3:
                        if (NAPI.Player.GetPlayerHealth(player) > 99)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не нуждаетесь в лечении", 3000);
                            break;
                        }
                        if (sessionData.TimersData.HealTimer != null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы уже лечитесь", 3000);
                            break;
                        }
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы начали лечение, но если вы отойдите слишком далеко, то лечение будет прервано", 3000);
                        sessionData.TimersData.HealTimer = Timers.Start(3750, () => healTimer(player), true);
                        return;*/
                    case 4:
                        OpenTattooDeleteMenu(player);
                        return;
                    case 5:
                        if (memberFractionData == null || memberFractionData.Id != (int) Models.Fractions.EMS)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NotEMS), 3000);
                            break;
                        }
                        if (!player.IsInVehicle)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarCantMoveAptekas), 3000);
                            break;
                        }
                        var vehicle = (ExtVehicle)player.Vehicle;
                        var vehicleLocalData = vehicle.GetVehicleLocalData();
                        if (vehicleLocalData != null)
                        {
                            if (!vehicleLocalData.CanMedKits)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarCantMoveAptekas), 3000);
                                break;
                            }

                            int medCount = Chars.Repository.getCountItem(VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), ItemId.HealthKit);
                            if (medCount >= vMain.GetMaxSlots(vehicle.Model))
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxAptekas), 3000);
                                break;
                            }
                            int zagruz = 75 - medCount;
                            GameLog.FracLog(memberFractionData.Id, player.GetUUID(), -1, player.Name, "-1", $"HumaneLabs({zagruz})");

                            Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMedkits, LangFunc.GetText(LangType.Ru, DataName.FillAptekas, zagruz));

                            Chars.Repository.AddNewItem(null, VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), "vehicle", ItemId.HealthKit, zagruz, MaxSlots: vMain.GetMaxSlots(vehicle.Model));
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FulledAptekas), 3000);
                        }
                        return;
                    case 6:
                        if (player.IsInVehicle) return;
                        if (player.Position.Z > 60)
                        {
                            if (sessionData.Following != null)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsFollowing), 3000);
                                return;
                            }
                            player.Position = emsCheckpoints[8] + new Vector3(0, 0, 1.12);
                            Main.PlayerEnterInterior(player, emsCheckpoints[8] + new Vector3(0, 0, 1.12));
                        }
                        else
                        {
                            if (memberFractionData == null || (memberFractionData.Id != (int) Models.Fractions.EMS && memberFractionData.Id != (int) Models.Fractions.CITY))
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NotEMS), 3000);
                                break;
                            }
                            if (memberFractionData.Id == (int) Models.Fractions.CITY && memberFractionData.Rank <= (int) Models.Fractions.THELOST)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoDostup), 3000);
                                break;
                            }
                            if (sessionData.Following != null)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsFollowing), 3000);
                                return;
                            }
                            player.Position = emsCheckpoints[7] + new Vector3(0, 0, 1.12);
                            Main.PlayerEnterInterior(player, emsCheckpoints[7] + new Vector3(0, 0, 1.12));
                        }
                        return;
                    case 7:
                        if (memberFractionData == null || (memberFractionData.Id != (int) Models.Fractions.EMS && memberFractionData.Id != (int) Models.Fractions.CITY))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NotEMS), 3000);
                            break;
                        }
                        if (memberFractionData.Id == (int) Models.Fractions.CITY && memberFractionData.Rank <= 17)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoDostup), 3000);
                            break;
                        }
                        if (player.IsInVehicle) return;
                        if (player.Position.Z > 35)
                        {
                            if (sessionData.Following != null)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsFollowing), 3000);
                                return;
                            }
                            player.Position = emsCheckpoints[10] + new Vector3(0, 0, 1.12);
                            Main.PlayerEnterInterior(player, emsCheckpoints[10] + new Vector3(0, 0, 1.12));
                        }
                        else
                        {
                            if (sessionData.Following != null)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsFollowing), 3000);
                                return;
                            }
                            player.Position = emsCheckpoints[9] + new Vector3(0, 0, 1.12);
                            Main.PlayerEnterInterior(player, emsCheckpoints[9] + new Vector3(0, 0, 1.12));
                        }
                        return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"OnFractionEms Exception: {e.ToString()}");
            }
        }

        private static void healTimer(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                if (!player.IsCharacterData()) return;
                if (!SafeZones.IsSafeZone(sessionData.InsideSafeZone, SafeZones.ZoneName.EMS) || player.Health == 100)
                {
                    if (sessionData.TimersData.HealTimer != null)
                    {
                        Timers.Stop(sessionData.TimersData.HealTimer);
                        sessionData.TimersData.HealTimer = null;
                    }
                    Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.HealEnded), 3000);
                    return;
                }
                switch (accountData.VipLvl)
                {
                    case 2:
                    case 3:
                        if (player.Health + 2 > 100) player.Health = 100;
                        else player.Health = player.Health + 2;
                        break;
                    case 4:
                    case 5:
                        if (player.Health + 3 > 100) player.Health = 100;
                        else player.Health = player.Health + 3;
                        break;
                    default:
                        if (player.Health + 1 > 100) player.Health = 100;
                        player.Health = player.Health + 1;
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"healTimer Exception: {e.ToString()}");
            }
        }
        #region menus
        public static void OpenHospitalStockMenu(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                var fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.EMS);
                if (fractionData == null)
                    return;

                var frameList = new FrameListData();
                frameList.Header = LangFunc.GetText(LangType.Ru, DataName.SkladEmsShtuk, fractionData.MedKits);
                frameList.Callback = callback_hospitalstock;
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.TakeApteka), "takemed"));
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.GetsApteka), "putmed"));
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.GetElektoshok), "tazer"));
                Players.Popup.List.Repository.Open(player, frameList); 
            }
            catch (Exception e)
            {
                Log.Write($"OpenHospitalStockMenu Exception: {e.ToString()}");
            }
        }
        private static void callback_hospitalstock(ExtPlayer player, object listItem)
        {
            try
            {
                if (!(listItem is string))
                    return;
                
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null || memberFractionData.Id != (int) Models.Fractions.EMS)
                    return;
                
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null)
                    return;
                
                if (player.Position.DistanceTo(emsCheckpoints[3]) >= 5)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TooFarFromVidacha), 3000);
                    return;
                }
                switch (listItem)
                {
                    case "takemed":
                        if (!Manager.canGetWeapon(player, "Medkits")) return;
                        if (fractionData.MedKits <= 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"На складе не осталось аптечек", 3000);
                            return;
                        }
                        else if (Chars.Repository.isFreeSlots(player, ItemId.HealthKit, 1) != 0) return;
                        Chars.Repository.AddNewItem(player, $"char_{player.GetUUID()}", "inventory", ItemId.HealthKit);
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы взяли аптечку. У Вас {Chars.Repository.getCountItem($"char_{player.GetUUID()}", ItemId.HealthKit)} штук", 3000);
                        fractionData.MedKits--;
                        fractionData.UpdateLabel();
                        GameLog.Stock(memberFractionData.Id, player.GetUUID(), player.Name, "medkit", 1, "out");
                        Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMedkits, LangFunc.GetText(LangType.Ru, DataName.GetApteka));
                        break;
                    case "putmed":
                        ItemStruct ItemStruct = Chars.Repository.isItem(player, "inventory", ItemId.HealthKit);
                        if (ItemStruct == null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет аптечек", 3000);
                            return;
                        }
                        Chars.Repository.RemoveIndex(player, ItemStruct.Location, ItemStruct.Index, 1);
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы положили аптечку. У Вас осталось {Chars.Repository.getCountItem($"char_{player.GetUUID()}", ItemId.HealthKit)} штук", 3000);
                        fractionData.MedKits++;
                        fractionData.UpdateLabel();
                        GameLog.Stock(memberFractionData.Id, player.GetUUID(), player.Name, "medkit", 1, "in");
                        Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMedkits, $"Положил на склад аптечку");
                        break;
                    case "tazer":
                        if (!Configs.FractionWeapons[8].ContainsKey("StunGun"))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ErrorDostupWeapon), 3000);
                            return;
                        }
                        if (memberFractionData.Rank < Configs.FractionWeapons[8]["StunGun"])
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoDostupWeapon), 3000);
                            return;
                        }

                        string serial = WeaponRepository.GetSerial(true, 8);
                        Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeSpecial, $"Создал StunGun ({serial})");
                        WeaponRepository.GiveWeapon(player, ItemId.StunGun, serial);
                        return;
                }

                //Menu.Item menuItem = new Menu.Item("header", Menu.MenuItem.Header);
                //menuItem.Text = $"Склад ({Stocks.fracStocks[(int) Models.Fractions.EMS].Medkits}шт)";
                //menu.Change(player, 0, menuItem);
            }
            catch (Exception e)
            {
                Log.Write($"callback_hospitalstock Exception: {e.ToString()}");
            }
        }

        public static void OpenTattooDeleteMenu(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                
                var frameList = new FrameListData();
                frameList.Header = LangFunc.GetText(LangType.Ru, DataName.TattooSved);//LangFunc.GetText(LangType.Ru, DataName.SelectZoneTattoo)
                frameList.Callback = callback_tattoodelete;
                
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.Torso), "Torso"));
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.Head), "Head"));
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.LeftArm), "LeftArm"));
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.RightArm), "RightArm"));
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.LeftLeg), "LeftLeg"));
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.RightLeg), "RightLeg"));

                Players.Popup.List.Repository.Open(player, frameList); 
            }
            catch (Exception e)
            {
                Log.Write($"OpenTattooDeleteMenu Exception: {e.ToString()}");
            }
        }

        private static string[] TattooZonesNames = new string[6] { "торса", "головы", "левой руки", "правой руки", "левой ноги", "правой ноги" };
        private static void callback_tattoodelete(ExtPlayer player, object listItem)
        {
            try
            {
                if (!(listItem is string))
                    return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                var zone = Enum.Parse<TattooZones>((string)listItem);
                var custom = player.GetCustomization();
                if (custom == null)
                    return;
                
                if (custom.Tattoos[Convert.ToInt32(zone)].Count == 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У Вас нет татуировок в этой зоне", 3000);
                    return;
                }
                if (UpdateData.CanIChange(player, 300, true) != 255) return;
                MoneySystem.Wallet.Change(player, -300);
                GameLog.Money($"player({characterData.UUID})", $"server", 300, $"tattooRemove");
                
                var fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.CITY);
                if (fractionData != null)
                    fractionData.Money += 150;

                foreach (Tattoo tattoo in custom.Tattoos[Convert.ToInt32(zone)])
                {
                    if (tattoo == null) continue;
                    Decoration decoration = new Decoration();
                    decoration.Collection = NAPI.Util.GetHashKey(tattoo.Dictionary);
                    decoration.Overlay = NAPI.Util.GetHashKey(tattoo.Hash);
                    player.RemoveDecoration(decoration);
                }

                custom.Tattoos[Convert.ToInt32(zone)] = new List<Tattoo>();
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы свели татуировки с " + TattooZonesNames[Convert.ToInt32(zone)], 3000);
            }
            catch (Exception e)
            {
                Log.Write($"callback_tattoodelete Exception: {e.ToString()}");
            }
        }
        #endregion
    }
}
