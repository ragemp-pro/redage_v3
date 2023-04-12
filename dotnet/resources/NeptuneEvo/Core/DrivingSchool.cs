using System;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Handles;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Functions;
using NeptuneEvo.GUI;
using NeptuneEvo.Players.Popup.List.Models;
using NeptuneEvo.Quests;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;
using Redage.SDK;

namespace NeptuneEvo.Core
{
    class DrivingSchool : Script
    {
        private static readonly nLog Log = new nLog("Core.DrivingSchool");

        private static Vector3 enterSchool = new Vector3(435.8382, -984.11847, 30.689484);

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                //CustomColShape.CreateCylinderColShape(enterSchool, 1, 2, 0, ColShapeEnums.DriveSchool);

                //NAPI.Marker.CreateMarker(1, enterSchool - new Vector3(0, 0, 0.7f), new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220));
                //NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Автошкола"), enterSchool + new Vector3(0, 0, 1f), 5f, 0.3f, 0, new Color(255, 255, 255));

               //Main.CreateBlip(new Main.BlipData(498, "Автошкола", enterSchool, 2, true));
                PedSystem.Repository.CreateQuest("mp_m_securoguard_01", enterSchool, -60.65183f, title: "~y~NPC~w~ Офицер Бенсон\nВыдача лицензий", colShapeEnums: ColShapeEnums.DriveSchool);

                /*for (int i = 0; i < 72; i++)
                {
                    CustomColShape.CreateCylinderColShape(drivingCoords[i], 7, 5, 0, ColShapeEnums.DriveSchoolCoord, i);
                }*/
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }

        /*[ServerEvent(Event.PlayerEnterVehicle)]
        public void onPlayerEnterVehicleHandler(ExtPlayer player, ExtVehicle vehicle, sbyte seatid)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if (vehicleLocalData.Access != VehicleAccess.School) return;
                    if (vehicleLocalData.WorkDriver != characterData.UUID)
                    {
                        VehicleManager.WarpPlayerOutOfVehicle(player);
                        Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"Эта учебная машина не предназначается для Вас.", 3000);
                        return;
                    }
                    if (sessionData.TimersData.SchoolTimer != null)
                    {
                        Timers.Stop(sessionData.TimersData.SchoolTimer);
                        sessionData.TimersData.SchoolTimer = null;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"onPlayerEnterVehicleHandler Exception: {e.ToString()}");
            }
        }

        [ServerEvent(Event.PlayerExitVehicle)]
        public void Event_OnPlayerExitVehicle(ExtPlayer player, ExtVehicle vehicle)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (sessionData.DSchoolData.Vehicle == vehicle)
                {
                    if (sessionData.TimersData.SchoolTimer == null) sessionData.TimersData.SchoolTimer = Timers.StartOnce(60000, () => timer_exitVehicle(player), true);
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FailExamIfNotSit), 7000);
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"Event_OnPlayerExitVehicle Exception: {e.ToString()}");
            }
        }

        private void timer_exitVehicle(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (sessionData.DSchoolData.Vehicle == null) return;
                if (player.IsInVehicle && player.Vehicle == sessionData.DSchoolData.Vehicle) return;
                VehicleStreaming.DeleteVehicle(sessionData.DSchoolData.Vehicle);
                Trigger.ClientEvent(player, "deleteCheckpoint", 12, 0);
                sessionData.DSchoolData.IsDriving = false;
                sessionData.DSchoolData.Vehicle = null;
                if (sessionData.TimersData.SchoolTimer != null)
                {
                    Timers.Stop(sessionData.TimersData.SchoolTimer);
                    sessionData.TimersData.SchoolTimer = null;
                }
                Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FailExam), 5000);
            }
            catch (Exception e)
            {
                Log.Write($"timer_exitVehicle Exception: {e.ToString()}");
            }
        }

        public static void onPlayerDisconnected(ExtPlayer player, DisconnectionType type, string reason)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (sessionData.DSchoolData.Vehicle != null)
                {
                    VehicleStreaming.DeleteVehicle(sessionData.DSchoolData.Vehicle);
                    sessionData.DSchoolData.Vehicle = null;
                    if (sessionData.TimersData.SchoolTimer != null)
                    {
                        Timers.Stop(sessionData.TimersData.SchoolTimer);
                        sessionData.TimersData.SchoolTimer = null;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"onPlayerDisconnected Exception: {e.ToString()}");
            }
        }*/
        public static void startDrivingCourse(ExtPlayer player, int index)
        {
            try
            {
                if (!FunctionsAccess.IsWorking("startDrivingCourse"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (sessionData.DSchoolData.IsDriving || sessionData.WorkData.OnWork)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantDoThisNow), 3000);
                    return;
                }
                if (characterData.Licenses[index])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouAlreadyHaveLic), 3000);
                    return;
                }
                var fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.CITY);
                if (fractionData == null)
                    return;
                switch (index)
                {
                    case 0:
                        if (Chars.UpdateData.CanIChange(player, Main.LicPrices[0], true) != 255) return;
                        characterData.Licenses[0] = true;
                        MoneySystem.Wallet.Change(player, -Main.LicPrices[0]);
                        //fractionData.Money += Main.LicPrices[3];
                        //GameLog.Money($"player({characterData.UUID})", $"frac(6)", Main.LicPrices[3], $"buyLic");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucBuyMotoLic), 3000);
                        //Repository.PlayerStats(player);
                        return;
                    case 1:
                        if (Chars.UpdateData.CanIChange(player, Main.LicPrices[1], true) != 255) return;
                        characterData.Licenses[1] = true;
                        qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage7, 2, isUpdateHud: true);
                        qMain.UpdateQuestsComplete(player, Zdobich.QuestName, (int)zdobich_quests.Stage7, true);
                        MoneySystem.Wallet.Change(player, -Main.LicPrices[1]);
                        //fractionData.Money += Main.LicPrices[3];
                        //GameLog.Money($"player({characterData.UUID})", $"frac(6)", Main.LicPrices[3], $"buyLic");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucBuyVehLic), 3000);
                        //Repository.PlayerStats(player);
                        return;
                    case 2:
                        if (Chars.UpdateData.CanIChange(player, Main.LicPrices[2], true) != 255) return;
                        characterData.Licenses[2] = true;
                        MoneySystem.Wallet.Change(player, -Main.LicPrices[2]);
                        //fractionData.Money += Main.LicPrices[3];
                        //GameLog.Money($"player({characterData.UUID})", $"frac(6)", Main.LicPrices[3], $"buyLic");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucBuyGruzLic), 3000);
                        //Repository.PlayerStats(player);
                        return;
                    /*case 0:
                        try
                        {
                            if (Chars.UpdateData.CanIChange(player, Main.LicPrices[0], true) != 255) return;
                            Vehicle vehicle = VehicleStreaming.CreateVehicle(VehicleHash.Bagger, startCourseCoord[Step], startCourseRot[Step], 30, 30, $"LICA{player.Value}", acc: "SCHOOL", workdriv: player, petrol: 9999);
                            if (Step++ >= 13) Step = 0;
                            sessionData.DSchoolData.Vehicle = vehicle;
                            sessionData.DSchoolData.IsDriving = true;
                            sessionData.DSchoolData.License = 0;
                            sessionData.DSchoolData.Check = 0;
                            //player.SetIntoVehicle(vehicle, (int)VehicleSeat.Driver);
                            Trigger.ClientEvent(player, "setIntoVehicle", vehicle, VehicleSeat.Driver - 1);
                            Trigger.ClientEvent(player, "createCheckpoint", 12, 1, drivingCoords[0] - new Vector3(0, 0, 2), 4, 0, 255, 0, 0);
                            Trigger.ClientEvent(player, "createWaypoint", drivingCoords[0].X, drivingCoords[0].Y);
                            MoneySystem.Wallet.Change(player, -Main.LicPrices[0]);
                            fractionData.Money += Main.LicPrices[0];
                            GameLog.Money($"player({characterData.UUID})", $"frac(6)", Main.LicPrices[0], $"buyLic");
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Чтобы завести транспорт, нажмите B", 3000);
                        }
                        catch (Exception e)
                        {
                            Log.Write($"startDrivingCourse case 0 Exception: {e.ToString()}");
                        }
                        return;
                    case 1:
                        try
                        {
                            if (Chars.UpdateData.CanIChange(player, Main.LicPrices[1], true) != 255) return;
                            Vehicle vehicle = VehicleStreaming.CreateVehicle(VehicleHash.Dilettante, startCourseCoord[Step], startCourseRot[Step], 30, 30, $"LICB{player.Value}", acc: "SCHOOL", workdriv: player, petrol: 9999);
                            if (Step++ >= 13) Step = 0;
                            sessionData.DSchoolData.Vehicle = vehicle;
                            sessionData.DSchoolData.IsDriving = true;
                            sessionData.DSchoolData.License = 1;
                            sessionData.DSchoolData.Check = 0;
                            //player.SetIntoVehicle(vehicle, (int)VehicleSeat.Driver);
                            Trigger.ClientEvent(player, "setIntoVehicle", vehicle, VehicleSeat.Driver - 1);
                            Trigger.ClientEvent(player, "createCheckpoint", 12, 1, drivingCoords[0] - new Vector3(0, 0, 2), 4, 0, 255, 0, 0);
                            Trigger.ClientEvent(player, "createWaypoint", drivingCoords[0].X, drivingCoords[0].Y);
                            MoneySystem.Wallet.Change(player, -Main.LicPrices[1]);
                            fractionData.Money += Main.LicPrices[1];
                            GameLog.Money($"player({characterData.UUID})", $"frac(6)", Main.LicPrices[1], $"buyLic");
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Чтобы завести транспорт, нажмите B", 3000);
                        }
                        catch (Exception e)
                        {
                            Log.Write($"startDrivingCourse case 1 Exception: {e.ToString()}");
                        }
                        return;
                    case 2:
                        try
                        {
                            if (Chars.UpdateData.CanIChange(player, Main.LicPrices[2], true) != 255) return;
                            Vehicle vehicle = VehicleStreaming.CreateVehicle(VehicleHash.Flatbed, startCourseCoord[Step], startCourseRot[Step], 30, 30, $"LICC{player.Value}", acc: "SCHOOL", workdriv: player, petrol: 9999);
                            if (Step++ >= 13) Step = 0;
                            sessionData.DSchoolData.Vehicle = vehicle;
                            sessionData.DSchoolData.IsDriving = true;
                            sessionData.DSchoolData.License = 2;
                            sessionData.DSchoolData.Check = 0;
                            //player.SetIntoVehicle(vehicle, (int)VehicleSeat.Driver);
                            Trigger.ClientEvent(player, "setIntoVehicle", vehicle, VehicleSeat.Driver - 1);
                            Trigger.ClientEvent(player, "createCheckpoint", 12, 1, drivingCoords[0] - new Vector3(0, 0, 2), 4, 0, 255, 0, 0);
                            Trigger.ClientEvent(player, "createWaypoint", drivingCoords[0].X, drivingCoords[0].Y);
                            MoneySystem.Wallet.Change(player, -Main.LicPrices[2]);
                            fractionData.Money += Main.LicPrices[2];
                            GameLog.Money($"player({characterData.UUID})", $"frac(6)", Main.LicPrices[2], $"buyLic");
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Чтобы завести транспорт, нажмите B", 3000);
                        }
                        catch (Exception e)
                        {
                            Log.Write($"startDrivingCourse case 2 Exception: {e.ToString()}");
                        }
                        return;*/
                    case 3:
                        if (Chars.UpdateData.CanIChange(player, Main.LicPrices[3], true) != 255) return;
                        characterData.Licenses[3] = true;
                        MoneySystem.Wallet.Change(player, -Main.LicPrices[3]);
                        fractionData.Money += Main.LicPrices[3];
                        GameLog.Money($"player({characterData.UUID})", $"frac(6)", Main.LicPrices[3], $"buyLic");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucBuySeaLic), 3000);
                        //Repository.PlayerStats(player);
                        return;
                    case 4:
                        if (Chars.UpdateData.CanIChange(player, Main.LicPrices[4], true) != 255) return;
                        if (characterData.LVL < 20)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LicMustBe20), 3000);
                            return;
                        }
                        characterData.Licenses[4] = true;
                        MoneySystem.Wallet.Change(player, -Main.LicPrices[4]);
                        fractionData.Money += Main.LicPrices[4];
                        GameLog.Money($"player({characterData.UUID})", $"frac(6)", Main.LicPrices[4], $"buyLic");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucBuyHeliLic), 3000);
                        //Repository.PlayerStats(player);
                        return;
                    case 5:
                        if (Chars.UpdateData.CanIChange(player, Main.LicPrices[5], true) != 255) return;
                        if (characterData.LVL < 20)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LicMustBe20), 3000);
                            return;
                        }
                        characterData.Licenses[5] = true;
                        MoneySystem.Wallet.Change(player, -Main.LicPrices[5]);
                        fractionData.Money += Main.LicPrices[5];
                        GameLog.Money($"player({characterData.UUID})", $"frac(6)", Main.LicPrices[5], $"buyLic");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucBuyPlaneLic), 3000);
                        //Repository.PlayerStats(player);
                        return;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"startDrivingCourse Exception: {e.ToString()}");
            }
        }

        #region menu
        [Interaction(ColShapeEnums.DriveSchool)]
        public static void OnDriveSchool(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                var frameList = new FrameListData();

                frameList.Header = "Лицензии";
                frameList.Callback = callback_driveschool;
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.MotoLic, Main.LicPrices[0]), 0));
                
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.LegLic, Main.LicPrices[1]), 1));
                
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.GruzLic, Main.LicPrices[2]), 2));
                
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.VodLic, Main.LicPrices[3]), 3));
                
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.VertLic, Main.LicPrices[4]), 4));
                
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.SamLic, Main.LicPrices[5]), 5));
                
                Players.Popup.List.Repository.Open(player, frameList); 
                BattlePass.Repository.UpdateReward(player, 149);
            }
            catch (Exception e)
            {
                Log.Write($"OnDriveSchool Exception: {e.ToString()}");
            }
        }
        private static void callback_driveschool(ExtPlayer player, object listItem)
        {
            if (!(listItem is int)) 
                return;
            
            if (!player.IsCharacterData()) 
                return;
            
            startDrivingCourse(player, Convert.ToInt32(listItem));
        }
        #endregion
    }
}