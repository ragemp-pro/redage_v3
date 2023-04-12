using GTANetworkAPI; 
using NeptuneEvo.Core; 
using Redage.SDK; 
using NeptuneEvo.GUI; 
using System;
using Localization;
using NeptuneEvo.Chars.Models; 
using NeptuneEvo.Chars; 
using NeptuneEvo.Functions; 
using NeptuneEvo.Accounts; 
using NeptuneEvo.Players.Models; 
using NeptuneEvo.Players; 
using NeptuneEvo.Character.Models; 
using NeptuneEvo.Character;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Handles;
using NeptuneEvo.Table.Models;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;

namespace NeptuneEvo.Fractions 
{ 
    class Fbi : Script
    {

        public static Vector3 GpsPosition = new Vector3(133.7017, - 711.9227, 275.80142);
        
        public static Vector3[] fbiCheckpoints = new Vector3[17] 
        { 
            new Vector3(147.2835, -757.7181, 241.032), // duty              0 
            new Vector3(136.1821, -761.7615, 241.152), // 49 floor          1 
            new Vector3(130.9762, -762.3011, 241.1518), // 49 floor to 53   2 
            new Vector3(156.81, -757.24, 257.05), // 53 floor               3 
            new Vector3(149.2415, -764.2226, 261.7318), // roof             4 
            new Vector3(118.9617, -729.1614, 241.152), // gun menu          5 
            new Vector3(136.0578, -761.8408, 44.75204), // 1 floor          6 
            new Vector3(125.941, -738.790, 32.2047), // garage              7 
            new Vector3(2515.789, -346.95114, 100.89),  // warg mode        8 
            new Vector3(127.0072, -729.1547, 241.032), // fbi stock         9 
            new Vector3(169.7186, -685.5652, 33.27261), // fbi boost        10 
            new Vector3(2521.573, -457.50223, 92.9), // fbi boost           11 
            new Vector3(120.0081, -726.7838, 241.132),  // warg mode        12 
            new Vector3(2502.1575, -425.84903, 94.58), // gun menu          13 
            new Vector3(2511.8054, -441.3076, 99.11), // duty               14 
            new Vector3(2504.2527, -433.34906, 99.11), // 2 floor           15 
            new Vector3(2504.4084, -433.09525, 106.91), // 4 floor          16 
        }; 
        public static bool warg_mode = false; 
 
        private static readonly nLog Log = new nLog("Fractions.Fbi"); 
 
        [ServerEvent(Event.ResourceStart)] 
        public void onResourceStart() 
        { 
            try 
            { 
                NAPI.TextLabel.CreateTextLabel("~w~Jack Briggs", new Vector3(149.1317, -758.3485, 243.152), 5f, 0.3f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension); 
                NAPI.TextLabel.CreateTextLabel("~w~Harry Hodge", new Vector3(120.0836, -726.7773, 243.152), 5f, 0.3f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension); 
 
                NAPI.Marker.CreateMarker(1, fbiCheckpoints[0] - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220)); 
                NAPI.Marker.CreateMarker(1, fbiCheckpoints[1] - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220)); 
                NAPI.Marker.CreateMarker(1, fbiCheckpoints[2] - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220)); 
                NAPI.Marker.CreateMarker(1, fbiCheckpoints[3] - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220)); 
                NAPI.Marker.CreateMarker(1, fbiCheckpoints[4] - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220)); 
                NAPI.Marker.CreateMarker(1, fbiCheckpoints[5] - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220)); 
                NAPI.Marker.CreateMarker(1, fbiCheckpoints[6] - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220)); 
                NAPI.Marker.CreateMarker(1, fbiCheckpoints[7] - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220)); 
                NAPI.Marker.CreateMarker(1, fbiCheckpoints[8] - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220)); 
                NAPI.Marker.CreateMarker(1, fbiCheckpoints[9] - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220)); 
                NAPI.Marker.CreateMarker(1, fbiCheckpoints[12] - new Vector3(0, 0, 1.25), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220)); 
                NAPI.Marker.CreateMarker(1, fbiCheckpoints[13] - new Vector3(0, 0, 1.25), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220)); 
                NAPI.Marker.CreateMarker(1, fbiCheckpoints[14] - new Vector3(0, 0, 1.25), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220)); 
                NAPI.Marker.CreateMarker(1, fbiCheckpoints[15] - new Vector3(0, 0, 1.25), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220)); 
                NAPI.Marker.CreateMarker(1, fbiCheckpoints[16] - new Vector3(0, 0, 1.25), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220)); 
                #region cols 
                CustomColShape.CreateCylinderColShape(fbiCheckpoints[0], 1, 2, 0, ColShapeEnums.FractionFbi, 1); // duty fbi 
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(fbiCheckpoints[0].X, fbiCheckpoints[0].Y, fbiCheckpoints[0].Z + 0.3), 5F, 0.3F, 0, new Color(255, 255, 255)); 
                 
                CustomColShape.CreateCylinderColShape(fbiCheckpoints[14], 1, 2, 0, ColShapeEnums.FractionFbi, 1); // duty fbi 
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(fbiCheckpoints[14].X, fbiCheckpoints[14].Y, fbiCheckpoints[14].Z + 0.3), 5F, 0.3F, 0, new Color(255, 255, 255)); 
 
                CustomColShape.CreateCylinderColShape(fbiCheckpoints[1], 1, 2, 0, ColShapeEnums.FractionFbi, 2); // 49 floor to 53 
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(fbiCheckpoints[1].X, fbiCheckpoints[1].Y, fbiCheckpoints[1].Z + 0.3), 5F, 0.3F, 0, new Color(255, 255, 255)); 
 
                CustomColShape.CreateCylinderColShape(fbiCheckpoints[2], 1, 2, 0, ColShapeEnums.FractionFbi, 4); // 49 floor 
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(fbiCheckpoints[2].X, fbiCheckpoints[2].Y, fbiCheckpoints[2].Z + 0.3), 5F, 0.3F, 0, new Color(255, 255, 255)); 
 
                CustomColShape.CreateCylinderColShape(fbiCheckpoints[3], 1, 2, 0, ColShapeEnums.FractionFbi, 5); // 53 floor to 49 
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(fbiCheckpoints[3].X, fbiCheckpoints[3].Y, fbiCheckpoints[3].Z + 0.3), 5F, 0.3F, 0, new Color(255, 255, 255)); 
 
                CustomColShape.CreateCylinderColShape(fbiCheckpoints[4], 1, 2, 0, ColShapeEnums.FractionFbi, 2); // roof 
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(fbiCheckpoints[4].X, fbiCheckpoints[4].Y, fbiCheckpoints[4].Z + 0.3), 5F, 0.3F, 0, new Color(255, 255, 255)); 
 
                CustomColShape.CreateCylinderColShape(fbiCheckpoints[5], 1, 2, 0, ColShapeEnums.FractionFbi, 3); // gun menu 
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(fbiCheckpoints[5].X, fbiCheckpoints[5].Y, fbiCheckpoints[5].Z + 0.3), 5F, 0.3F, 0, new Color(255, 255, 255)); 
                 
                CustomColShape.CreateCylinderColShape(fbiCheckpoints[13], 1, 2, 0, ColShapeEnums.FractionFbi, 3); // gun menu 
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(fbiCheckpoints[13].X, fbiCheckpoints[13].Y, fbiCheckpoints[13].Z + 0.3), 5F, 0.3F, 0, new Color(255, 255, 255)); 
 
                CustomColShape.CreateCylinderColShape(fbiCheckpoints[6], 1, 2, 0, ColShapeEnums.FractionFbi, 2); // 1 floor 
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(fbiCheckpoints[6].X, fbiCheckpoints[6].Y, fbiCheckpoints[6].Z + 0.3), 5F, 0.3F, 0, new Color(255, 255, 255)); 
 
                CustomColShape.CreateCylinderColShape(fbiCheckpoints[7], 1, 2, 0, ColShapeEnums.FractionFbi, 2); // garage 
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(fbiCheckpoints[7].X, fbiCheckpoints[7].Y, fbiCheckpoints[7].Z + 0.3), 5F, 0.3F, 0, new Color(255, 255, 255)); 
 
                CustomColShape.CreateCylinderColShape(fbiCheckpoints[8], 1, 2, 0, ColShapeEnums.FractionFbi, 6); // warg 
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(fbiCheckpoints[8].X, fbiCheckpoints[8].Y, fbiCheckpoints[8].Z + 0.3), 5F, 0.3F, 0, new Color(255, 255, 255)); 
                 
                CustomColShape.CreateCylinderColShape(fbiCheckpoints[12], 1, 2, 0, ColShapeEnums.FractionFbi, 6); // warg 
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(fbiCheckpoints[12].X, fbiCheckpoints[12].Y, fbiCheckpoints[12].Z + 0.3), 5F, 0.3F, 0, new Color(255, 255, 255)); 
 
                CustomColShape.CreateCylinderColShape(fbiCheckpoints[9], 1, 2, 0, ColShapeEnums.FractionFbi, 7); // stock 
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Открыть оружейный склад"), new Vector3(fbiCheckpoints[9].X, fbiCheckpoints[9].Y, fbiCheckpoints[9].Z + 0.3), 5F, 0.3F, 0, new Color(255, 255, 255)); 
 
                CustomColShape.CreateCylinderColShape(fbiCheckpoints[10], 3, 5, 0, ColShapeEnums.FractionFbi, 8); // repair 
                NAPI.Marker.CreateMarker(1, new Vector3(fbiCheckpoints[10].X, fbiCheckpoints[10].Y, fbiCheckpoints[10].Z - 1.75), new Vector3(), new Vector3(), 3f, new Color(255, 255, 255, 220)); 
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Ремонт наземного транспорта"), fbiCheckpoints[10], 5F, 0.3F, 0, new Color(255, 255, 255)); 
                 
                CustomColShape.CreateCylinderColShape(fbiCheckpoints[11], 3, 5, 0, ColShapeEnums.FractionFbi, 8); // repair 
                NAPI.Marker.CreateMarker(1, new Vector3(fbiCheckpoints[11].X, fbiCheckpoints[11].Y, fbiCheckpoints[11].Z - 1.75), new Vector3(), new Vector3(), 3f, new Color(255, 255, 255, 220)); 
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Ремонт наземного транспорта"), fbiCheckpoints[11], 5F, 0.3F, 0, new Color(255, 255, 255)); 
                 
                CustomColShape.CreateCylinderColShape(fbiCheckpoints[15], 1, 2, 0, ColShapeEnums.FractionFbi, 9); // 2 floor 
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(fbiCheckpoints[15].X, fbiCheckpoints[15].Y, fbiCheckpoints[15].Z), 5F, 0.3F, 0, new Color(255, 255, 255)); 
                 
                CustomColShape.CreateCylinderColShape(fbiCheckpoints[16], 1, 2, 0, ColShapeEnums.FractionFbi, 10); // 4 floor 
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(fbiCheckpoints[16].X, fbiCheckpoints[16].Y, fbiCheckpoints[16].Z), 5F, 0.3F, 0, new Color(255, 255, 255)); 
                #endregion 
                
                PedSystem.Repository.CreateQuest("s_m_m_fibsec_01", new Vector3(117.0207, -747.3272, 45.75158), 109.83642f, title: "~y~NPC~w~ Агент Андрей\nВызвать сотрудника", colShapeEnums: ColShapeEnums.CallFibMember);
            } 
            catch (Exception e) 
            { 
                Log.Write($"onResourceStart Exception: {e.ToString()}"); 
            } 
        } 
        
        [Interaction(ColShapeEnums.CallFibMember)] 
        public static void OpenCallFibMemberDialog(ExtPlayer player, int _) 
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
 
                Trigger.ClientEvent(player, "openDialog", "CallFibMemberDialog", LangFunc.GetText(LangType.Ru, DataName.AreYouWantToCallGov)); 
            } 
            catch (Exception e) 
            { 
                Log.Write($"OpenCallFibMemberDialog Exception: {e.ToString()}"); 
            } 
        }
        
        [Interaction(ColShapeEnums.FractionFbi)] 
        public static void OnFractionFbi(ExtPlayer player, int interact) 
        { 
            try 
            { 
                var sessionData = player.GetSessionData(); 
                if (sessionData == null) 
                    return; 
                
                var characterData = player.GetCharacterData(); 
                if (characterData == null) 
                    return; 
                
                var fractionData = player.GetFractionData();
                
                switch (interact) 
                { 
                    case 1: 
                        if (fractionData != null && fractionData.Id == (int) Models.Fractions.FIB) FractionClothingSets.OpenFractionClothingSetsMenu(player);
                        else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NotFIB), 3000); 
                        return; 
                    case 2: 
                        if (player.IsInVehicle) return; 
                        if (sessionData.Following != null) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsFollowing), 3000); 
                            return; 
                        } 
                        OpenFbiLiftMenu(player); 
                        return; 
                    case 3: 
                        if (fractionData == null || fractionData.Id != (int) Models.Fractions.FIB) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NotFIB), 3000); 
                            return; 
                        } 
                        if (!fractionData.IsOpenStock) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseClosed), 3000); 
                            return; 
                        } 
                        OpenFbiGunMenu(player); 
                        return; 
                    case 4: 
                        NAPI.Entity.SetEntityPosition(player, fbiCheckpoints[3] + new Vector3(0, 0, 1.12)); 
                        return; 
                    case 5: 
                        NAPI.Entity.SetEntityPosition(player, fbiCheckpoints[2] + new Vector3(0, 0, 1.12)); 
                        return; 
                    case 6: 
                        if (fractionData == null || fractionData.Id != (int) Models.Fractions.FIB) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NotFIB), 3000); 
                            return; 
                        } 
                        if (!sessionData.WorkData.OnDuty) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000); 
                            return; 
                        } 
                        if (sessionData.InCpMode) 
                        { 
                            Manager.SetSkin(player); 
                            sessionData.InCpMode = false; 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WorkClothesSuccess), 3000); 
                        } 
                        else 
                        { 
                            if (!warg_mode) 
                            { 
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoEmergency), 3000); 
                                return; 
                            } 
                            //ClothesComponents.ClearClothes(player, characterData.Gender); 
                            if (characterData.Gender) 
                            { 
                                ClothesComponents.SetSpecialClothes(player, 0,  39, 2); 
                                ClothesComponents.SetSpecialClothes(player, 11, 53, 1); 
                                ClothesComponents.SetSpecialClothes(player, 4, 31, 2); 
                                ClothesComponents.SetSpecialClothes(player, 6, 25, 0); 
                                ClothesComponents.SetSpecialClothes(player, 8, 130, 0); 
                                ClothesComponents.SetSpecialClothes(player, 3, 49, 0); 
                            } 
                            else 
                            { 
                                ClothesComponents.SetSpecialClothes(player, 0,  38, 2); 
                                ClothesComponents.SetSpecialClothes(player, 11, 46, 1); 
                                ClothesComponents.SetSpecialClothes(player, 4, 30, 2); 
                                ClothesComponents.SetSpecialClothes(player, 6, 25, 0); 
                                ClothesComponents.SetSpecialClothes(player, 8, 160, 0); 
                                ClothesComponents.SetSpecialClothes(player, 3, 49, 0); 
                            } 
                            Chars.Repository.LoadAccessories(player);
                            sessionData.InCpMode = true; 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SpecialWorkClothesSuccess), 3000); 
                        } 
                        return; 
                    case 7: 
                        if (fractionData == null || fractionData.Id != (int) Models.Fractions.FIB) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NotFIB), 3000); 
                            return; 
                        } 
                        if (!sessionData.WorkData.OnDuty) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000); 
                            return; 
                        } 
                        if (!fractionData.IsOpenGunStock) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Warehouse2Closed), 3000); 
                            return; 
                        } 
                        if (!player.IsFractionAccess(RankToAccess.OpenWeaponStock)) return; 
                        sessionData.OnFracStock = 9; 
                        Chars.Repository.LoadOtherItemsData(player, "Fraction", "9", 5, Chars.Repository.InventoryMaxSlots["Fraction"]); 
                        return; 
                    case 8: 
                        if (fractionData == null || fractionData.Id != (int) Models.Fractions.FIB) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoPolice), 3000); 
                            return; 
                        } 
                        if (!sessionData.WorkData.OnDuty) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000); 
                            return; 
                        } 
                        if (!player.IsInVehicle) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustInCar), 3000); 
                            return; 
                        } 
                        if (player.Vehicle.Class == 15 || player.Vehicle.Class == 16) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RepairFlatVeh), 3000); 
                            return; 
                        } 
                        if (DateTime.Now < Main.NextFixcarFIBVeh) 
                        { 
                            long ticks = Main.NextFixcarFIBVeh.Ticks - DateTime.Now.Ticks; 
                            if (ticks <= 0) return; 
                            DateTime g = new DateTime(ticks); 
                            if (g.Hour >= 1) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NextRepair1h, g.Hour, g.Minute, g.Second), 7000); 
                            else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NextRepair, g.Minute, g.Second), 7000); 
                            return; 
                        } 
                        var vehicle = (ExtVehicle)player.Vehicle; 
                        var vehicleLocalData = vehicle.GetVehicleLocalData(); 
                        if (vehicleLocalData != null) 
                        {
                            if (vehicleLocalData.Access == VehicleAccess.Fraction && vehicleLocalData.Fraction == (int) Models.Fractions.FIB) 
                            { 
                                Main.NextFixcarFIBVeh = DateTime.Now.AddMinutes(3); 
                                VehicleManager.RepairCar(vehicle); 
                                Commands.RPChat("sme", player, LangFunc.GetText(LangType.Ru, DataName.RepairedVeh)); 
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucRepairVeh), 3000); 
                            } 
                            else 
                            { 
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkCar), 3000); 
                                return; 
                            } 
                        } 
                        return; 
                    case 9: 
                        NAPI.Entity.SetEntityPosition(player, fbiCheckpoints[16]); 
                        return; 
                    case 10: 
                        NAPI.Entity.SetEntityPosition(player, fbiCheckpoints[15]); 
                        return; 
                } 
            } 
            catch (Exception e) 
            { 
                Log.Write($"OnFractionFbi Exception: {e.ToString()}"); 
            } 
        } 
 
        #region menus 
        public static void OpenFbiLiftMenu(ExtPlayer player) 
        { 
            try 
            { 
                if (!player.IsCharacterData()) return; 
                Trigger.ClientEvent(player, "openlift", 0, "fbilift"); 
            } 
            catch (Exception e) 
            { 
                Log.Write($"OpenFbiLiftMenu Exception: {e.ToString()}"); 
            } 
        } 
        [RemoteEvent("fbilift")] 
        public static void callback_fbilift(ExtPlayer player, int floor) 
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
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsFollowing), 3000); 
                    return; 
                } 
 
                string data = (characterData.Gender) ? "128_0_true" : "98_0_false"; 
 
                if (Chars.Repository.isItem(player, "inventory", ItemId.Jewelry, data) == null && player.GetFractionId() != (int) Models.Fractions.FIB) 
                { 
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsBadgeFIBorPD), 5000); 
                    return; 
                } 
                switch (floor) 
                { 
                    case 0: //garage 
                        NAPI.Entity.SetEntityPosition(player, fbiCheckpoints[7] + new Vector3(0, 0, 1.12)); 
                        Main.PlayerEnterInterior(player, fbiCheckpoints[7] + new Vector3(0, 0, 1.12)); 
                        return; 
                    case 1: //floor1 
                        NAPI.Entity.SetEntityPosition(player, fbiCheckpoints[6] + new Vector3(0, 0, 1.12)); 
                        Main.PlayerEnterInterior(player, fbiCheckpoints[6] + new Vector3(0, 0, 1.12)); 
                        return; 
                    case 2: //floor49 
                        NAPI.Entity.SetEntityPosition(player, fbiCheckpoints[1] + new Vector3(0, 0, 1.12)); 
                        Main.PlayerEnterInterior(player, fbiCheckpoints[1] + new Vector3(0, 0, 1.12)); 
                        return; 
                    case 3: //roof 
                        NAPI.Entity.SetEntityPosition(player, fbiCheckpoints[4] + new Vector3(0, 0, 1.12)); 
                        Main.PlayerEnterInterior(player, fbiCheckpoints[4] + new Vector3(0, 0, 1.12)); 
                        return; 
                    default: 
                        // Not supposed to end up here.  
                        break; 
                } 
            } 
            catch (Exception e) 
            { 
                Log.Write($"callback_fbilift Exception: {e.ToString()}"); 
            } 
        } 
 
        public static void OpenFbiGunMenu(ExtPlayer player) 
        { 
            try 
            { 
                if (!player.IsCharacterData()) return; 
                Manager.OpenFractionSM(player, "fbi"); 
            } 
            catch (Exception e) 
            { 
                Log.Write($"OpenFbiGunMenu Exception: {e.ToString()}"); 
            } 
        } 
        [RemoteEvent("fbigun")] 
        public static void callback_fbiguns(ExtPlayer player, int index) 
        { 
            try 
            { 
                var sessionData = player.GetSessionData(); 
                if (sessionData == null) return; 
                var characterData = player.GetCharacterData(); 
                if (characterData == null) return; 
                if (player.Position.DistanceTo(fbiCheckpoints[5]) >= 5 && player.Position.DistanceTo(fbiCheckpoints[13]) >= 5) 
                { 
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TooFarFromVidacha), 3000); 
                    return; 
                } 
                var fractionData = player.GetFractionData();
                if (fractionData == null || fractionData.Id != (int) Models.Fractions.FIB) return; 
                switch (index) 
                { 
                    case 0: 
                        Manager.giveGun(player, WeaponRepository.Hash.StunGun, "StunGun"); 
                        return; 
                    case 1: 
                        Manager.giveGun(player, WeaponRepository.Hash.CombatPistol, "CombatPistol"); 
                        return; 
                    case 2: 
                        Manager.giveGun(player, WeaponRepository.Hash.CombatPDW, "CombatPDW"); 
                        return; 
                    case 3: 
                        Manager.giveGun(player, WeaponRepository.Hash.CarbineRifle, "CarbineRifle"); 
                        return; 
                    case 4: 
                        Manager.giveGun(player, WeaponRepository.Hash.HeavySniper, "HeavySniper"); 
                        return; 
                    case 5: 
                        if (!Manager.canGetWeapon(player, "Armor")) return; 
                        if (fractionData.Materials < Manager.matsForArmor) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseNoMats), 3000); 
                            return; 
                        } 
                        if (Chars.Repository.itemCount(player, "inventory", ItemId.BodyArmor) >= Chars.Repository.maxItemCount) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyHaveBronik), 3000); 
                            return; 
                        } 
                        if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.BodyArmor, 1, 100.ToString()) == -1) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000); 
                            return; 
                        } 
                        fractionData.Materials -= Manager.matsForArmor; 
                        fractionData.UpdateLabel(); 
                        GameLog.Stock(fractionData.Id, characterData.UUID, player.Name, "armor", 1, "out"); 
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GetBronik), 3000); 
                        Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMats, LangFunc.GetText(LangType.Ru, DataName.CraftedBronik)); 
                        return; 
                    case 6: // medkit 
                        if (!Manager.canGetWeapon(player, "Medkits")) return; 
                        if (fractionData.MedKits == 0) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseNoAptekas), 3000); 
                            return; 
                        } 
                        if (Chars.Repository.isFreeSlots(player, ItemId.HealthKit) != 0) return; 
                        Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.HealthKit, 1); 
                        fractionData.MedKits--; 
                        fractionData.UpdateLabel(); 
                        GameLog.Stock(fractionData.Id, characterData.UUID, player.Name, "medkit", 1, "out"); 
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GotApteka), 3000); 
                        Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMedkits, LangFunc.GetText(LangType.Ru, DataName.GetApteka)); 
                        return; 
                    case 7: 
                        if (!Manager.canGetWeapon(player, "PistolAmmo")) return; 
                        Manager.giveAmmo(player, ItemId.PistolAmmo, 12); 
                        return; 
                    case 8: 
                        if (!Manager.canGetWeapon(player, "SMGAmmo")) return; 
                        Manager.giveAmmo(player, ItemId.SMGAmmo, 30); 
                        return; 
                    case 9: 
                        if (!Manager.canGetWeapon(player, "RiflesAmmo")) return; 
                        Manager.giveAmmo(player, ItemId.RiflesAmmo, 30); 
                        return; 
                    case 10: 
                        if (!Manager.canGetWeapon(player, "SniperAmmo")) return; 
                        Manager.giveAmmo(player, ItemId.SniperAmmo, 5); 
                        return; 
                    case 11: 
                        if (!Manager.canGetWeapon(player, "FIBB")) return; 
                        string data = (characterData.Gender) ? "128_0_true" : "98_0_false"; 
                        if (Chars.Repository.isItem(player, "inventory", ItemId.Jewelry, data) != null) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouAlreadyHaveBadge), 3000); 
                            return; 
                        } 
 
                        if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Jewelry, 1, data) == -1) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000); 
                            return; 
                        } 
 
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGotFibBadge), 3000); 
                        return; 
                    case 12: 
                        if (!Manager.canGetWeapon(player, "Armor")) return; 
                        ItemStruct aItem = Chars.Repository.isItem(player, "inventory", ItemId.BodyArmor); 
                        if (aItem == null) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoBronik), 3000); 
                            return; 
                        } 
                        int armorp; 
                        if (aItem.Location == "accessories") 
                        { 
                            if (sessionData.ArmorHealth != -1 && player.Armor > sessionData.ArmorHealth) WeaponRepository.PlayerKickAntiCheat(player, 3, true); 
                            armorp = player.Armor; 
                            player.Armor = 0; 
                        } 
                        else armorp = Convert.ToInt32(aItem.Item.Data); 
                        int matstoadd; 
                        if (armorp >= 76) matstoadd = 150; 
                        else if (armorp >= 50) matstoadd = 100; 
                        else if (armorp >= 1) matstoadd = 50; 
                        else matstoadd = 0; 
                        fractionData.Materials = fractionData.Materials + matstoadd > 300000 ? 300000 : fractionData.Materials + matstoadd; 
                        fractionData.UpdateLabel(); 
                        Chars.Repository.RemoveIndex(player, aItem.Location, aItem.Index, 1); 
                        Trigger.ClientEvent(player, "client.isArmor", false); 
                        sessionData.ArmorHealth = -1; 
                        ClothesComponents.SetSpecialClothes(player, 9, 0, 0); 
                        ClothesComponents.UpdateClothes(player); 
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BronikReturn, armorp, matstoadd), 5000); 
                        return; 
                    default: 
                        // Not supposed to end up here.  
                        break; 
                } 
            } 
            catch (Exception e) 
            { 
                Log.Write($"callback_fbiguns Exception: {e.ToString()}"); 
            } 
        } 
        #endregion 
    } 
} 
