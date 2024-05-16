using System; 
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Core; 
using Redage.SDK; 
using NeptuneEvo.GUI; 
 
using NeptuneEvo.Chars.Models; 
using NeptuneEvo.Chars; 
using NeptuneEvo.Functions; 
using NeptuneEvo.Quests; 
using NeptuneEvo.Accounts; 
using NeptuneEvo.Players.Models; 
using NeptuneEvo.Players; 
using NeptuneEvo.Character.Models; 
using NeptuneEvo.Character;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Handles;
using NeptuneEvo.Quests.Models;
using NeptuneEvo.Table.Models;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;

namespace NeptuneEvo.Fractions 
{ 
    class Sheriff : Script 
    { 
        private static readonly nLog Log = new nLog("Fractions.Sheriff"); 
 
        private static int PedSecretaryId = 0; 
        private static int PedSecondSecretaryId = 0; 
 
        private static Vector3 FirstArrestPosition = new Vector3(1860.3392, 3692.5254, 30.25941); 
        private static Vector3 SecondArrestPosition = new Vector3(-444.3758, 6011.2217, 27.985634); 
         
        public static Vector3 FirstPrisonPosition = new Vector3(1862.5636, 3693.7375, 30.25940); 
        public static Vector3 SecondPrisonPosition = new Vector3(-444.60986, 6014.0884, 27.985611); 
         
        public static Vector3 FirstExitPrisonPosition = new Vector3(1856.585, 3705.3735, 34.26975); 
        public static Vector3 SecondExitPrisonPosition = new Vector3(-428.31613, 5988.3354, 31.490107); 
         
        public static Vector3 FirstPosition = new Vector3(1846.0203, 3692.7175, 34.26693); 
        public static Vector3 CloakroomPosition = new Vector3(1850.7942, 3693.731, 34.26696); 
        private static Vector3 CloakroomSpecialPosition = new Vector3(1848.5563, 3695.9812, 34.26695); 
        private static Vector3 VehicleBoostPosition = new Vector3(1866.0756, 3698.5723, 33.59682); 
         
        public static Vector3 SecondPosition = new Vector3(-445.67395, 6013.7183, 31); 
        public static Vector3 SecondCloakroomPosition = new Vector3(-454.50473, 6011.8906, 31.7); 
        private static Vector3 SecondCloakroomSpecialPosition = new Vector3(-456.57333, 6013.906, 31.7); 
        private static Vector3 SecondStockPosition = new Vector3(-437.08975, 5997.4136, 31.7); 
        private static Vector3 VehicleRepairPosition = new Vector3(-448.36835, 5994.5537, 31.32247); 
         
        public static Vector3 FirstGunsPosition = new Vector3(1844.3673, 3692.2087, 34.266937); 
        public static Vector3 SecondGunsPosition = new Vector3(-436.12234, 5999.6006, 31.7); 
         
 
        [ServerEvent(Event.ResourceStart)] 
        public void onResourceStart() 
        { 
            try 
            { 
                //Main.CreateBlip(new Main.BlipData(526, "Sheriff", FirstPosition, 38, true)); 
                //Main.CreateBlip(new Main.BlipData(526, "Sheriff", SecondPosition, 38, true)); 
 
                CustomColShape.CreateCylinderColShape(FirstArrestPosition, 6, 3, 0, ColShapeEnums.FractionSheriffArrest, 1); 
                CustomColShape.CreateCylinderColShape(SecondArrestPosition, 7, 3, 0, ColShapeEnums.FractionSheriffArrest, 2); 
                //CustomColShape.CreateCylinderColShape(policeCheckpoints[1], 1, 2, 0, ColShapeEnums.FractionSheriff, 1); 
                CustomColShape.CreateCylinderColShape(CloakroomPosition, 1, 2, 0, ColShapeEnums.FractionSheriff, 2); 
                CustomColShape.CreateCylinderColShape(CloakroomSpecialPosition, 1, 2, 0, ColShapeEnums.FractionSheriff, 3); 
                //CustomColShape.CreateCylinderColShape(policeCheckpoints[7], 1, 2, 0, ColShapeEnums.FractionSheriff, 4); 
                CustomColShape.CreateCylinderColShape(VehicleBoostPosition, 3, 5, 0, ColShapeEnums.FractionSheriff, 6); 
 
                NAPI.Marker.CreateMarker(30, CloakroomPosition, new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220)); 
                NAPI.Marker.CreateMarker(30, CloakroomSpecialPosition, new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220)); 
                NAPI.Marker.CreateMarker(44, VehicleBoostPosition, new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220)); 
                 
                CustomColShape.CreateCylinderColShape(SecondCloakroomPosition, 1, 2, 0, ColShapeEnums.FractionSheriff, 2); 
                CustomColShape.CreateCylinderColShape(SecondCloakroomSpecialPosition, 1, 2, 0, ColShapeEnums.FractionSheriff, 3); 
                CustomColShape.CreateCylinderColShape(SecondStockPosition, 1, 2, 0, ColShapeEnums.FractionSheriff, 5); 
                 
                NAPI.Marker.CreateMarker(30, SecondCloakroomPosition, new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220)); 
                NAPI.Marker.CreateMarker(30, SecondCloakroomSpecialPosition, new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220)); 
                NAPI.Marker.CreateMarker(20, SecondStockPosition, new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220)); 
                 
                CustomColShape.CreateCylinderColShape(VehicleRepairPosition, 3, 5, 0, ColShapeEnums.FractionSheriff, 7); // repair 
                NAPI.Marker.CreateMarker(1, new Vector3(VehicleRepairPosition.X, VehicleRepairPosition.Y, VehicleRepairPosition.Z - 1.75), new Vector3(), new Vector3(), 3f, new Color(255, 255, 255, 220)); 
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Ремонт наземного транспорта"), VehicleRepairPosition, 5F, 0.3F, 0, new Color(255, 255, 255)); 
 
                Ped ped = PedSystem.Repository.CreateQuest("s_f_y_sheriff_01", new Vector3(1852.9309, 3688.9907, 34.266937), -150.65309f, title: "~y~NPC~w~ Виктория Ньютон\nСекретарь", colShapeEnums: ColShapeEnums.FracSheriff); 
                PedSecretaryId = ped.Value; 
                 
                Ped secondPed = PedSystem.Repository.CreateQuest("s_f_y_sheriff_01", new Vector3(-448.6393, 6012.3833, 31.7), -45.60703f, title: "~y~NPC~w~ Виктория Элиз\nСекретарь", colShapeEnums: ColShapeEnums.FracSheriff); 
                PedSecondSecretaryId = secondPed.Value; 
 
                PedSystem.Repository.CreateQuest("s_m_m_fibsec_01", FirstGunsPosition, -87.25532f, title: "~y~NPC~w~ Агент Александр\nСкладской", colShapeEnums: ColShapeEnums.FracSheriff); 
                PedSystem.Repository.CreateQuest("s_m_m_fibsec_01", SecondGunsPosition, 40.875225f, title: "~y~NPC~w~ Агент Роуз\nСкладской", colShapeEnums: ColShapeEnums.FracSheriff); 
                PedSystem.Repository.CreateQuest("s_m_m_armoured_02", new Vector3(-444.31284, 6010.7974, 31.716427), 47.103275f, title: "~y~NPC~w~ Сержант Соболь\nВызвать сотрудника", colShapeEnums: ColShapeEnums.CallSheriffMember);
            } 
            catch (Exception e) 
            { 
                Log.Write($"onResourceStart Exception: {e.ToString()}"); 
            } 
        } 
        
        [Interaction(ColShapeEnums.CallSheriffMember)] 
        public static void OpenCallSheriffMemberDialog(ExtPlayer player, int _) 
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
 
                Trigger.ClientEvent(player, "openDialog", "CallSheriffMemberDialog", LangFunc.GetText(LangType.Ru, DataName.AreYouWantToCallGov)); 
            } 
            catch (Exception e) 
            { 
                Log.Write($"OpenCallSheriffMemberDialog Exception: {e.ToString()}"); 
            } 
        }
        
        [Interaction(ColShapeEnums.FracSheriff)] 
        public static void Open(ExtPlayer player, int index) 
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
            if (PedSecretaryId == index || PedSecondSecretaryId == index) 
            {
                BattlePass.Repository.UpdateReward(player, 84);
                player.SelectQuest(new PlayerQuestModel("npc_fracsheriff", 0, 0, false, DateTime.Now)); 
                Trigger.ClientEvent(player, "client.quest.open", index, "npc_fracsheriff", 0, 0, 0); 
            } 
            else 
            { 
                var fractionData = player.GetFractionData();
                if (fractionData == null || fractionData.Id != (int) Models.Fractions.SHERIFF) 
                { 
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoPolice), 3000); 
                    return; 
                } 
                if (!fractionData.IsOpenStock) 
                { 
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseClosed), 3000); 
                    return; 
                } 
                if (!sessionData.WorkData.OnDuty) 
                { 
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000); 
                    return; 
                } 
                OpenPoliceGunMenu(player); 
                return; 
            } 
        } 
        public static void Perform(ExtPlayer player) 
        { 
            var characterData = player.GetCharacterData(); 
            if (characterData == null) return; 
            InventoryItemData Bags = Chars.Repository.GetItemData(player, "accessories", 8); 
            if (Bags.ItemId != ItemId.BagWithDrill && Bags.ItemId != ItemId.BagWithMoney) 
            { 
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoBagDrillMoney), 3000); 
                return; 
            } 
            Chars.Repository.RemoveIndex(player, "accessories", 8); 
            MoneySystem.Wallet.Change(player, Main.PoliceAward); 
            GameLog.Money($"server", $"player({characterData.UUID})", Main.PoliceAward, $"policeAward"); 
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Reward, Main.PoliceAward), 3000); 
            return; 
        } 
        [ServerEvent(Event.PlayerExitVehicle)] 
        public void Event_OnPlayerExitVehicle(ExtPlayer player, ExtVehicle vehicle) 
        { 
            try 
            { 
                var characterData = player.GetCharacterData(); 
                if (characterData == null) return; 
                if (player.VehicleSeat != (int)VehicleSeat.Driver || player.VehicleSeat != (int)VehicleSeat.RightFront) return;
                var fracId = player.GetFractionId();
                if (fracId != (int) Models.Fractions.SHERIFF && fracId != (int) Models.Fractions.FIB) return; 
                Trigger.ClientEvent(player, "closePc"); 
            } 
            catch (Exception e) 
            { 
                Log.Write($"Event_OnPlayerExitVehicle Exception: {e.ToString()}"); 
            } 
        } 
        
        [Interaction(ColShapeEnums.FractionSheriff)] 
        public static void OnFractionSheriff(ExtPlayer player, int interact) 
        { 
            try 
            { 
                var sessionData = player.GetSessionData(); 
                if (sessionData == null) return; 
                
                var fractionData = player.GetFractionData();
                if (fractionData == null || fractionData.Id != (int) Models.Fractions.SHERIFF) 
                { 
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoPolice), 3000); 
                    return; 
                } 
                switch (interact) 
                { 
                    case 2: 
                        FractionClothingSets.OpenFractionClothingSetsMenu(player);
                        return; 
                    case 3: 
                        if (!sessionData.WorkData.OnDuty) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000); 
                            return; 
                        } 
                        if (!is_warg) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoEmergency), 3000); 
                            return; 
                        } 
                        OpenSpecialPoliceMenu(player); 
                        return; 
                    /*case 5: 
                        if (characterData.Licenses[6]) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас уже есть лицензия на оружие", 3000); 
                            return; 
                        } 
                        if (!MoneySystem.Wallet.Change(player, -30000)) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас недостаточно средств.", 3000); 
                            return; 
                        } 
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили лицензию на оружие", 3000); 
                        characterData.Licenses[6] = true; 
                        //Chars.Repository.PlayerStats(player); 
                        return;*/ 
                    case 5: 
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
                        sessionData.OnFracStock = 18; 
                        Chars.Repository.LoadOtherItemsData(player, "Fraction", "18", 5, Chars.Repository.InventoryMaxSlots["Fraction"]); 
                        return; 
                    case 6: 
                        if (!sessionData.WorkData.OnDuty) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000); 
                            return; 
                        } 
                        if (!player.IsInVehicle || (player.Vehicle.Model != NAPI.Util.GetHashKey("police") && 
                            player.Vehicle.Model != NAPI.Util.GetHashKey("police2") && player.Vehicle.Model != NAPI.Util.GetHashKey("police3") && player.Vehicle.Model != NAPI.Util.GetHashKey("police4"))) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustCarPolice), 3000); 
                            return; 
                        } 
                        player.Vehicle.SetSharedData("BOOST", 20); 
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarBoost), 3000); 
                        return; 
                    case 7: 
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
                        if (DateTime.Now < Main.NextFixcarSheriffVeh) 
                        { 
                            long ticks = Main.NextFixcarSheriffVeh.Ticks - DateTime.Now.Ticks; 
                            if (ticks <= 0) return; 
                            DateTime g = new DateTime(ticks); 
                            if (g.Hour >= 1) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NextRepair1h, g.Hour, g.Minute, g.Second), 3000); 
                            else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NextRepair,  g.Minute, g.Second), 3000); 
                            return; 
                        } 
                        var vehicle = (ExtVehicle)player.Vehicle; 
                        var vehicleLocalData = vehicle.GetVehicleLocalData(); 
                        if (vehicleLocalData != null) 
                        { 
                            if (vehicleLocalData.Access == VehicleAccess.Fraction && vehicleLocalData.Fraction == (int) Models.Fractions.SHERIFF) 
                            { 
                                Main.NextFixcarSheriffVeh = DateTime.Now.AddMinutes(3); 
                                VehicleManager.RepairCar(vehicle); 
                                Commands.RPChat("sme", player, LangFunc.GetText(LangType.Ru, DataName.RepairedVeh)); 
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucRepairVeh), 3000); 
                            } 
                            else 
                            { 
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustCarPolice), 3000); 
                                return; 
                            } 
                        } 
                        return; 
                } 
            } 
            catch (Exception e) 
            { 
                Log.Write($"OnFractionSheriff Exception: {e.ToString()}"); 
            } 
        } 
 
        #region shapes 
        [Interaction(ColShapeEnums.FractionSheriffArrest, In: true)] 
        public static void InFractionSheriffArrest(ExtPlayer player, int index) 
        { 
            try 
            { 
                var sessionData = player.GetSessionData(); 
                if (sessionData == null) return; 
                sessionData.InArrestArea = index; 
            } 
            catch (Exception e) 
            { 
                Log.Write($"InFractionSheriffArrest Exception: {e.ToString()}"); 
            } 
        } 
        [Interaction(ColShapeEnums.FractionSheriffArrest, Out: true)] 
        public static void OutFractionSheriffArrest(ExtPlayer player) 
        { 
            try 
            { 
                var sessionData = player.GetSessionData(); 
                if (sessionData == null) return; 
                var characterData = player.GetCharacterData(); 
                if (characterData == null) return; 
                sessionData.InArrestArea = -1; 
                if (characterData.ArrestTime != 0) 
                { 
                    if (characterData.ArrestType == 2) player.Position = SecondPrisonPosition; 
                    else player.Position = FirstPrisonPosition; 
                } 
            } 
            catch (Exception e) 
            { 
                Log.Write($"OutFractionSheriffArrest Exception: {e.ToString()}"); 
            } 
        } 
        #endregion 
 
        public static void onPlayerDisconnectedhandler(ExtPlayer player, DisconnectionType type, string reason) 
        { 
            try 
            { 
                var sessionData = player.GetSessionData(); 
                if (sessionData == null) return; 
                if (!player.IsCharacterData()) return; 
                if (sessionData.TimersData.ArrestTimer != null) 
                { 
                    Timers.Stop(sessionData.TimersData.ArrestTimer); 
                    sessionData.TimersData.ArrestTimer = null; 
                } 
                if (sessionData.Following != null) 
                { 
                    var target = sessionData.Following; 
                    var targetSessionData = target.GetSessionData(); 
                    if (targetSessionData != null) targetSessionData.Follower = null; 
                } 
                if (sessionData.Follower != null) 
                { 
                    var target = sessionData.Follower; 
                    var targetSessionData = target.GetSessionData(); 
                    if (targetSessionData != null) 
                    { 
                        targetSessionData.Following = null; 
                        Trigger.ClientEvent(target, "follow", false); 
                    } 
                } 
            } 
            catch (Exception e) 
            { 
                Log.Write($"onPlayerDisconnectedhandler Exception: {e.ToString()}"); 
            } 
        } 
 
        public static bool is_warg = false; 
 
        #region menus 
        public static void OpenPoliceGunMenu(ExtPlayer player) 
        { 
            try 
            { 
                if (!player.IsCharacterData()) return; 
                Manager.OpenFractionSM(player, "polic"); 
            } 
            catch (Exception e) 
            { 
                Log.Write($"OpenPoliceGunMenu Exception: {e.ToString()}"); 
            } 
        } 
        public static void OpenSpecialPoliceMenu(ExtPlayer player) 
        { 
            try 
            { 
                var sessionData = player.GetSessionData(); 
                if (sessionData == null) return; 
                var characterData = player.GetCharacterData(); 
                if (characterData == null) return; 
 
                if (!is_warg) 
                { 
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Не включен режим ЧП", 3000); 
                    return; 
                } 
                if (player.Position.DistanceTo(CloakroomSpecialPosition) >= 5 && player.Position.DistanceTo(SecondCloakroomSpecialPosition) >= 5) 
                { 
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TooFarFromVidacha), 3000); 
                    return; 
                } 
                if (player.GetFractionId() != (int) Models.Fractions.SHERIFF) return; 
 
                if (!sessionData.InCpMode) 
                { 
                    bool gender = characterData.Gender; 
                    //Customization.ApplyCharacter(player); 
                    //ClothesComponents.ClearClothes(player, gender); 
                    if (gender) 
                    { 
                        ClothesComponents.SetSpecialClothes(player, 0,  39, 0); 
                        //ClothesComponents.SetSpecialClothes(player, 1, 52, 0); 
                        ClothesComponents.SetSpecialClothes(player, 11, 53, 0); 
                        ClothesComponents.SetSpecialClothes(player, 4, 31, 0); 
                        ClothesComponents.SetSpecialClothes(player, 6, 25, 0); 
                        ClothesComponents.SetSpecialClothes(player, 3, 49, 0); 
                    } 
                    else 
                    { 
                        ClothesComponents.SetSpecialClothes(player, 0,  38, 0); 
                        //ClothesComponents.SetSpecialClothes(player, 1, 57, 0); 
                        ClothesComponents.SetSpecialClothes(player, 11, 46, 0); 
                        ClothesComponents.SetSpecialClothes(player, 4, 30, 0); 
                        ClothesComponents.SetSpecialClothes(player, 6, 25, 0); 
                        ClothesComponents.SetSpecialClothes(player, 3, 49, 0); 
                    } 
                    Chars.Repository.LoadAccessories(player);
                    sessionData.InCpMode = true; 
                    return; 
                } 
                Manager.SetSkin(player); 
                sessionData.InCpMode = false; 
            } 
            catch (Exception e) 
            { 
                Log.Write($"OpenSpecialPoliceMenu Exception: {e.ToString()}"); 
            } 
        } 
        #endregion 
    } 
} 
