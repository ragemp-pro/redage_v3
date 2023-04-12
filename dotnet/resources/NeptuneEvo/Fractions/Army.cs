using GTANetworkAPI;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.GUI;
using NeptuneEvo.Players;
using NeptuneEvo.VehicleModel;
using Redage.SDK;
using System;
using System.Linq;
using Localization;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Handles;
using NeptuneEvo.Table.Models;
using NeptuneEvo.Table.Tasks.Models;
using NeptuneEvo.Table.Tasks.Player;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;

namespace NeptuneEvo.Fractions
{
    class Army : Script
    {

        public static bool is_warg = false;

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                NAPI.TextLabel.CreateTextLabel("Ketrin Kellerman", new Vector3(-2348.598, 3210.923, 29.224812), 5f, 0.3f, 4, new Color(255, 255, 255), false, 3244522);
                
                CustomColShape.CreateCylinderColShape(ArmyCheckpoints[0], 1, 2, 3244522, ColShapeEnums.FractionArmy, 0);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(ArmyCheckpoints[0].X, ArmyCheckpoints[0].Y, ArmyCheckpoints[0].Z + 1), 5F, 0.3F, 0, new Color(255, 255, 255), dimension: 3244522);

                CustomColShape.CreateCylinderColShape(ArmyCheckpoints[1], 1, 2, 3244522, ColShapeEnums.FractionArmy, 1);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(ArmyCheckpoints[1].X, ArmyCheckpoints[1].Y, ArmyCheckpoints[1].Z + 1), 5F, 0.3F, 0, new Color(255, 255, 255), dimension: 3244522);

                CustomColShape.CreateCylinderColShape(ArmyCheckpoints[2], 5, 6, 0, ColShapeEnums.FractionArmy, 2);

                CustomColShape.CreateCylinderColShape(ArmyCheckpoints[3], 1, 2, 0, ColShapeEnums.FractionArmy, 3);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Лифт"), new Vector3(ArmyCheckpoints[3].X, ArmyCheckpoints[3].Y, ArmyCheckpoints[3].Z + 1), 5F, 0.3F, 0, new Color(255, 255, 255));

                CustomColShape.CreateCylinderColShape(ArmyCheckpoints[4], 1, 2, 0, ColShapeEnums.FractionArmy, 3);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Лифт"), new Vector3(ArmyCheckpoints[4].X, ArmyCheckpoints[4].Y, ArmyCheckpoints[4].Z + 1), 5F, 0.3F, 0, new Color(255, 255, 255));

                CustomColShape.CreateCylinderColShape(ArmyCheckpoints[5], 1, 2, 3244522, ColShapeEnums.FractionArmy, 4);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Открыть оружейный склад"), new Vector3(ArmyCheckpoints[5].X, ArmyCheckpoints[5].Y, ArmyCheckpoints[5].Z + 1), 5F, 0.3F, 0, new Color(255, 255, 255), dimension: 3244522);

                CustomColShape.CreateCylinderColShape(ArmyCheckpoints[6], 3, 3, 0, ColShapeEnums.FractionArmy, 5);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Ремонт наземного транспорта"), new Vector3(ArmyCheckpoints[6].X, ArmyCheckpoints[6].Y, ArmyCheckpoints[6].Z + 1), 5F, 0.3F, 0, new Color(255, 255, 255));

                CustomColShape.CreateCylinderColShape(ArmyCheckpoints[7], 5, 2, 0, ColShapeEnums.FractionArmy, 6);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Ремонт воздушного транспорта"), new Vector3(ArmyCheckpoints[7].X, ArmyCheckpoints[7].Y, ArmyCheckpoints[7].Z + 1), 5F, 0.3F, 0, new Color(255, 255, 255));

                CustomColShape.CreateCylinderColShape(ArmyCheckpoints[8], 1, 2, 0, ColShapeEnums.FractionArmy, 7);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Кнопка общей тревоги"), new Vector3(ArmyCheckpoints[8].X, ArmyCheckpoints[8].Y, ArmyCheckpoints[8].Z + 1), 5F, 0.3F, 0, new Color(255, 255, 255), true);
                                
                CustomColShape.CreateCylinderColShape(ArmyCheckpoints[9], 1, 2, 0, ColShapeEnums.FractionArmy, 8);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(ArmyCheckpoints[9].X, ArmyCheckpoints[9].Y, ArmyCheckpoints[9].Z), 5F, 0.3F, 0, new Color(255, 255, 255));

                CustomColShape.CreateCylinderColShape(ArmyCheckpoints[10], 1, 2, 3244522, ColShapeEnums.FractionArmy, 9);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(ArmyCheckpoints[10].X, ArmyCheckpoints[10].Y, ArmyCheckpoints[10].Z), 5F, 0.3F, 0, new Color(255, 255, 255), dimension: 3244522);

                CustomColShape.CreateCylinderColShape(ArmyCheckpoints[11], 1, 2, 0, ColShapeEnums.FractionArmy, 10);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(ArmyCheckpoints[11].X, ArmyCheckpoints[11].Y, ArmyCheckpoints[11].Z), 5F, 0.3F, 0, new Color(255, 255, 255));

                CustomColShape.CreateCylinderColShape(ArmyCheckpoints[12], 1, 2, 4566544, ColShapeEnums.FractionArmy, 11);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(ArmyCheckpoints[12].X, ArmyCheckpoints[12].Y, ArmyCheckpoints[12].Z), 5F, 0.3F, 0, new Color(255, 255, 255), dimension: 4566544);

                NAPI.Marker.CreateMarker(1, ArmyCheckpoints[0] - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220));
                NAPI.Marker.CreateMarker(1, ArmyCheckpoints[1] - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220));
                NAPI.Marker.CreateMarker(1, ArmyCheckpoints[2], new Vector3(), new Vector3(), 5f, new Color(155, 0, 0));
                NAPI.Marker.CreateMarker(1, ArmyCheckpoints[3] - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220));
                NAPI.Marker.CreateMarker(1, ArmyCheckpoints[4] - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220));
                NAPI.Marker.CreateMarker(1, ArmyCheckpoints[5] - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220));
                NAPI.Marker.CreateMarker(1, ArmyCheckpoints[6] - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 3f, new Color(255, 255, 255, 220));
                NAPI.Marker.CreateMarker(1, ArmyCheckpoints[7] - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 5f, new Color(255, 255, 255, 220));
                NAPI.Marker.CreateMarker(1, ArmyCheckpoints[8] - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220));
                NAPI.Marker.CreateMarker(21, ArmyCheckpoints[9] , new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220));
                NAPI.Marker.CreateMarker(21, ArmyCheckpoints[10], new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220), dimension:3244522);
                NAPI.Marker.CreateMarker(21, ArmyCheckpoints[11], new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220));
                NAPI.Marker.CreateMarker(21, ArmyCheckpoints[12], new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220), dimension:4566544);

                //Main.CreateBlip(new Main.BlipData(305, "National Guard", new Vector3(-2062.0103, 3196.759, 32.795883), 2, true));

                PedSystem.Repository.CreateQuest("s_m_m_marine_01", new Vector3(-276.09488, -2636.1191, 6.0462055), -50.05095f, title: "~y~NPC~w~ Рекрут Астраханский\nВызвать сотрудника", colShapeEnums: ColShapeEnums.CallArmyMember);
            }
            catch (Exception e) { Log.Write("ResourceStart: " + e.Message, nLog.Type.Error); }
        }

        private static readonly nLog Log = new nLog("Fractions.Army");        

        public static Vector3[] ArmyCheckpoints = new Vector3[13]
        {
            new Vector3(-2349.2397, 3217.694, 28.5), // guns     0
            new Vector3(-2340.1667, 3223.0715, 28.5), // dressing room    1
            new Vector3(-108.0619, -2414.873, 5.000001), // army docks mats     2
            new Vector3(-2360.946, 3249.595, 31.81073), // army lift 1 floor     3
            new Vector3(-2360.66, 3249.115, 91.90369), // army lift 9 floor     4
            new Vector3(-2344.1865, 3219.297, 28.5), // army stock    5
            new Vector3(-373.5402, -2780.3647, 6.0003), // Починка наземного транспорта 6
            new Vector3(-524.7707, -2902.6392, 6.0003), // Починка воздушного транспорта 7
            new Vector3(-289.3858, -2663.2488, 5.1595364), // Общая тревога кнопка 8
            new Vector3(-330.44122, -2778.9834, 5.3272867), // Вход из мира в инту 9
            new Vector3(-2345.4094, 3225.2507, 29.22482), // Выход из инты в мир 10
            new Vector3(-315.38025, -2698.401, 7.54995), // Вход из мира в инту 11
            new Vector3(-2352.8345, 3252.8472, 32.810738), // Выход из инты в мир 12
        };

        [RemoteEvent("armygun")]
        public static void callback_armyGuns(ExtPlayer player, int index)
        {
            try
            {
                var fractionData = player.GetFractionData();
                if (fractionData == null)
                    return;
                
                if (fractionData.Id != (int) Models.Fractions.ARMY) return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (player.Position.DistanceTo(ArmyCheckpoints[0]) >= 5) 
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TooFarFromWeaponTake), 3000);
                    return;
                }
                switch (index)
                {
                    case 0: //pistol
                        Manager.giveGun(player, WeaponRepository.Hash.Pistol, "Pistol");
                        return;
                    case 1: //carbine
                        Manager.giveGun(player, WeaponRepository.Hash.CarbineRifle, "CarbineRifle");
                        return;
                    case 2: // combat
                        Manager.giveGun(player, WeaponRepository.Hash.CombatMG, "CombatMG");
                        return;
                    case 3: //armor
                        if (!Manager.canGetWeapon(player, "Armor")) return;
                        if (fractionData.Materials > Manager.matsForArmor && Chars.Repository.itemCount(player, "inventory", ItemId.BodyArmor) < Chars.Repository.maxItemCount)
                        {
                            if (Chars.Repository.AddNewItem(player, $"char_{player.GetUUID()}", "inventory", ItemId.BodyArmor, 1, 100.ToString()) == -1)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                                return;
                            }
                            fractionData.Materials -= Manager.matsForArmor;
                            fractionData.UpdateLabel();
                            GameLog.Stock((int) Models.Fractions.ARMY, player.GetUUID(), player.Name, "armor", 1, "out");
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GetBronik), 3000);
                            Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMats, LangFunc.GetText(LangType.Ru, DataName.CraftedBronik));
                        }
                        return;
                    case 4:
                        if (!Manager.canGetWeapon(player, "Medkits")) return;
                        if (fractionData.MedKits == 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseNoAptekas), 3000);
                            return;
                        }
                        else if (Chars.Repository.isFreeSlots(player, ItemId.HealthKit, 1) != 0) return;
                        Chars.Repository.AddNewItem(player, $"char_{player.GetUUID()}", "inventory", ItemId.HealthKit, 1);
                        fractionData.MedKits--;
                        fractionData.UpdateLabel();
                        GameLog.Stock((int) Models.Fractions.ARMY, player.GetUUID(), player.Name, "medkit", 1, "out");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GotApteka), 3000);
                        Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMedkits, LangFunc.GetText(LangType.Ru, DataName.GetApteka));
                        return;
                    case 5: //pistolammo
                        if (Manager.canGetWeapon(player, "PistolAmmo")) Manager.giveAmmo(player, ItemId.PistolAmmo, 12);
                        return;
                    case 6: //riflesammo
                        if (Manager.canGetWeapon(player, "RiflesAmmo")) Manager.giveAmmo(player, ItemId.RiflesAmmo, 30);
                        return;
                    case 7: //smgammo
                        if (Manager.canGetWeapon(player, "SMGAmmo")) Manager.giveAmmo(player, ItemId.SMGAmmo, 100);
                        return;
                    case 8:
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
                        
                        if (fractionData.Materials + matstoadd > 400000) fractionData.Materials = 400000;
                        else fractionData.Materials += matstoadd;
                        
                        fractionData.UpdateLabel();
                        Chars.Repository.RemoveIndex(player, aItem.Location, aItem.Index, 1);
                        Trigger.ClientEvent(player, "client.isArmor", false);
                        sessionData.ArmorHealth = -1;
                        ClothesComponents.SetSpecialClothes(player, 9, 0, 0);
                        ClothesComponents.UpdateClothes(player);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BronikReturn,armorp, matstoadd), 5000);
                        player.AddTableScore(TableTaskId.Item18);
                        return;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"callback_armyGuns Exception: {e.ToString()}");
            }
        }
        
        [Interaction(ColShapeEnums.CallArmyMember)] 
        public static void OpenCallArmyMemberDialog(ExtPlayer player, int _) 
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
 
                Trigger.ClientEvent(player, "openDialog", "CallArmyMemberDialog", LangFunc.GetText(LangType.Ru, DataName.AreYouWantToCallGov)); 
            } 
            catch (Exception e) 
            { 
                Log.Write($"OpenCallArmyMemberDialog Exception: {e.ToString()}"); 
            } 
        }

        [Interaction(ColShapeEnums.FractionArmy)]
        public static void OnFractionArmy(ExtPlayer player, int interact)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                var fractionData = player.GetFractionData();
                if (fractionData == null)
                    return;
                
                switch (interact)
                {
                    case 0:
                        if (fractionData.Id != (int) Models.Fractions.ARMY)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DostupOn), 3000);
                            return;
                        }
                        if (!fractionData.IsOpenStock)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseClosed), 3000);
                            return;
                        }
                        Manager.OpenFractionSM(player, "army");
                        return;
                    case 1:
                        if (fractionData.Id != (int) Models.Fractions.ARMY)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DostupOff), 3000);
                            return;
                        }
                        FractionClothingSets.OpenFractionClothingSetsMenu(player);
                        return;
                    case 2:
                        if (fractionData.Id != (int) Models.Fractions.ARMY)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DostupOff), 3000);
                            return;
                        }
                        if (!player.IsInVehicle)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustInCar), 3000);
                            return;
                        }
                        var veh = (ExtVehicle)player.Vehicle;
                        var vehicleLocalData = veh.GetVehicleLocalData();
                        if (vehicleLocalData != null)
                        {
                            if (!vehicleLocalData.CanMats)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarCantMoveMats), 3000);
                                return;
                            }
                            if (vehicleLocalData.Fraction != (int) Models.Fractions.ARMY)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantLoadMatsToCar), 3000);
                                return;
                            }
                            if (sessionData.TimersData.LoadMatsTimer != null)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyLoadingMatsToCar), 3000);
                                return;
                            }
                            if (!Stocks.maxMats.ContainsKey(veh.Model)) return;
                            int count = Chars.Repository.getCountItem(VehicleManager.GetVehicleToInventory(veh.NumberPlate), ItemId.Material);
                            if (count >= Stocks.maxMats[veh.Model])
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarMaxMats), 3000);
                                return;
                            }
                            sessionData.TimersData.LoadMatsTimer = Timers.StartOnce(20000, () => loadMaterialsTimer(player), true);
                            vehicleLocalData.LoaderMats = player;
                            sessionData.VehicleMats = veh;
                            sessionData.WhereLoad = "ARMY";
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LoadingMatsStarted), 3000);
                            Trigger.ClientEvent(player, "showLoader", LangFunc.GetText(LangType.Ru, DataName.LoadingMats), 1);
                        }
                        return;
                    case 3:
                        if (player.IsInVehicle) return;
                        if (sessionData.Following != null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsFollowing), 3000);
                            return;
                        }
                        if (player.Position.Z > 50)
                        {
                            player.Position = new Vector3(ArmyCheckpoints[3].X, ArmyCheckpoints[3].Y, ArmyCheckpoints[3].Z + 1);
                            Main.PlayerEnterInterior(player, new Vector3(ArmyCheckpoints[3].X, ArmyCheckpoints[3].Y, ArmyCheckpoints[3].Z + 1));
                        }
                        else
                        {
                            player.Position = new Vector3(ArmyCheckpoints[4].X, ArmyCheckpoints[4].Y, ArmyCheckpoints[4].Z + 1);
                            Main.PlayerEnterInterior(player, new Vector3(ArmyCheckpoints[4].X, ArmyCheckpoints[4].Y, ArmyCheckpoints[4].Z + 1));
                        }
                        return;
                    case 4: // open stock gun
                        if (fractionData.Id != (int) Models.Fractions.ARMY)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DostupOff), 3000);
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
                        sessionData.OnFracStock = 14;
                        Chars.Repository.LoadOtherItemsData(player, "Fraction", "14", 5, Chars.Repository.InventoryMaxSlots["Fraction"]);
                        return;
                    case 5: // Наземный
                        if (fractionData.Id != (int) Models.Fractions.ARMY)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DostupOff), 3000);
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
                        if (DateTime.Now < Main.NextFixcarVeh)
                        {
                            long ticks = Main.NextFixcarVeh.Ticks - DateTime.Now.Ticks;
                            if (ticks <= 0) return;
                            DateTime g = new DateTime(ticks);
                            if (g.Hour >= 1) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NextRepair1h, g.Hour, g.Minute, g.Second), 3000);
                            else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NextRepair,  g.Minute, g.Second), 3000);
                            return;
                        }
                        var vehicle = (ExtVehicle)player.Vehicle;
                        vehicleLocalData = vehicle.GetVehicleLocalData();
                        if (vehicleLocalData != null)
                        {
                            if (vehicleLocalData.Access == VehicleAccess.Fraction && vehicleLocalData.Fraction == (int) Models.Fractions.ARMY)
                            {
                                Main.NextFixcarVeh = DateTime.Now.AddMinutes(3);
                                VehicleManager.RepairCar(vehicle);
                                Commands.RPChat("sme", player, LangFunc.GetText(LangType.Ru, DataName.RepairedVeh));
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucRepairVeh), 3000);
                            }
                            else
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustArmyVeh), 3000);
                                return;
                            }
                        }
                        return;
                    case 6: // Воздушный
                        if (fractionData.Id != (int) Models.Fractions.ARMY)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DostupOff), 3000);
                            return;
                        }
                        if (!sessionData.WorkData.OnDuty)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000);
                            return;
                        }
                        if (!player.IsInVehicle)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustFlyingVeh), 3000);
                            return;
                        }
                        if (player.Vehicle.Class != 15 && player.Vehicle.Class != 16)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RepairOnlyFlyingVeh), 3000);
                            return;
                        }
                        if (DateTime.Now < Main.NextFixcarPlane)
                        {
                            long ticks = Main.NextFixcarPlane.Ticks - DateTime.Now.Ticks;
                            if (ticks <= 0) return;
                            DateTime g = new DateTime(ticks);
                            if (g.Hour >= 1) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NextRepair1h, g.Hour, g.Minute, g.Second), 3000);
                            else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NextRepair,  g.Minute, g.Second), 3000);
                            return;
                        }
                        vehicle = (ExtVehicle)player.Vehicle;
                        vehicleLocalData = vehicle.GetVehicleLocalData();
                        if (vehicleLocalData != null)
                        {
                            if (vehicleLocalData.Access == VehicleAccess.Fraction && vehicleLocalData.Fraction == (int) Models.Fractions.ARMY)
                            {
                                Main.NextFixcarPlane = DateTime.Now.AddMinutes(3);
                                VehicleManager.RepairCar(vehicle);
                                Commands.RPChat("sme", player, LangFunc.GetText(LangType.Ru, DataName.RepairedVeh));
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucRepairVeh), 3000);
                            }
                            else
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustArmyVeh), 3000);
                                return;
                            }
                        }
                        return;
                    case 7: // Общая тревога
                        if (!player.IsCharacterData()) return;
                        if (fractionData.Id != (int) Models.Fractions.ARMY)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.DostupOff), 3000);
                            return;
                        }
                        if (!player.IsFractionAccess(RankToAccess.Warg)) return;
                        string message = "";
                        is_warg = !is_warg;
                        Trigger.ClientEventForAll("alarm", "PORT_OF_LS_HEIST_FORT_ZANCUDO_ALARMS", is_warg);
                        if (is_warg)
                        {
                            message = LangFunc.GetText(LangType.Ru, DataName.EnableWargNG, player.Name);
                            GameLog.FracLog(fractionData.Id, player.GetUUID(), -1, player.Name, "-1", "enableWarg");
                        }
                        else
                        {
                            message = LangFunc.GetText(LangType.Ru, DataName.DisableWargNG, player.Name);
                            GameLog.FracLog(fractionData.Id, player.GetUUID(), -1, player.Name, "-1", "disableWarg");
                        }
                        
                        foreach (var foreachPlayer in Character.Repository.GetPlayers())
                        {
                            var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                            if (foreachMemberFractionData == null) continue;
                            
                            if (foreachMemberFractionData.Id == (int) Models.Fractions.CITY || Configs.IsFractionPolic(foreachMemberFractionData.Id) || foreachMemberFractionData.Id == (int) Models.Fractions.ARMY) 
                                Notify.Send(foreachPlayer, NotifyType.Warning, NotifyPosition.BottomCenter, message, 3000);
                        }
                        return;
                    case 8: {
                        player.Position = ArmyCheckpoints[10];
                        Trigger.Dimension(player, 3244522);
                    }
                        return;
                    case 9: {
                        player.Position = ArmyCheckpoints[9];
                        Trigger.Dimension(player, 0);
                    }
                        return;
                    case 10: {
                        player.Position = ArmyCheckpoints[12];
                        Trigger.Dimension(player, 4566544);
                    }
                        return;
                    case 11: {
                        player.Position = ArmyCheckpoints[11];
                        Trigger.Dimension(player, 0);
                    }
                        return;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"OnFractionArmy Exception: {e.ToString()}");
            }
        }

        #region shapes

        [Interaction(ColShapeEnums.FractionArmy, Out: true)]
        public static void OutFractionArmy(ExtPlayer player, int Index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                else if (Index != 2) return;
                if (sessionData.TimersData.LoadMatsTimer != null)
                {
                    Timers.Stop(sessionData.TimersData.LoadMatsTimer);
                    sessionData.TimersData.LoadMatsTimer = null;
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Загрузка материалов отменена, так как транспорт покинул чекпоинт", 3000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"onEntityExitArmyMats Exception: {e.ToString()}");
            }
        }

        #endregion

        public static void loadMaterialsTimer(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (sessionData.VehicleMats == null) return;
                if (!player.IsInVehicle) return;
                var vehicle = sessionData.VehicleMats;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData == null || !vehicle.Exists) return;
                if (!Stocks.maxMats.ContainsKey(vehicle.Model)) return;

                int itemCount = Chars.Repository.getCountItem(VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), ItemId.Material);
                if (sessionData.WhereLoad == "WAR" && !MatsWar.isWar)
                {
                    if (sessionData.TimersData.LoadMatsTimer != null)
                    {
                        Timers.Stop(sessionData.TimersData.LoadMatsTimer);
                        sessionData.TimersData.LoadMatsTimer = null;
                    }
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TimeOver), 3000);
                    return;
                }
                if (itemCount >= Stocks.maxMats[vehicle.Model])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInCar), 3000);
                    if (sessionData.TimersData.LoadMatsTimer != null) Timers.Stop(sessionData.TimersData.LoadMatsTimer);
                    sessionData.TimersData.LoadMatsTimer = null;
                    return;
                }
                int iCount = 1;
                if (sessionData.WhereLoad == "WAR")
                {
                    int count = Stocks.maxMats[vehicle.Model] - itemCount;
                    if (count >= MatsWar.matsLeft)
                    {
                        iCount = itemCount + MatsWar.matsLeft;
                        MatsWar.matsLeft = 0;
                        MatsWar.endWar();
                    }
                    else
                    {
                        iCount = count;
                        MatsWar.matsLeft -= count;
                    }
                }
                else
                {
                    iCount = Stocks.maxMats[vehicle.Model] - itemCount;
                    if (vehicle.Model == (uint)VehicleHash.Cargobob)
                    {
                        if (Stocks.CargobobMats == 0) // Если Cargobob больше не может загрузить материалы
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CargobobDayLimit, Stocks.MaxCargobobMats), 3000);
                            return;
                        }
                        else // Если может
                        {
                            if (Stocks.CargobobMats - iCount < 0) // Если остаток загрузки меньше, чем мы хотим загрузить, то уменьшаем сколько хотим загрузить до лимита.
                            {
                                iCount = Stocks.CargobobMats;
                                Stocks.CargobobMats = 0;
                            }
                            else Stocks.CargobobMats -= iCount; // Если всё хорошо, то уменьшаем от общего лимита столько, сколько мы загрузили.
                        }
                    }
                    Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMats, LangFunc.GetText(LangType.Ru, DataName.MatLoads, iCount));
                    player.AddTableScore(TableTaskId.Item17);
                }

                Chars.Repository.AddNewItem(null, VehicleManager.GetVehicleToInventory(vehicle.NumberPlate), "vehicle", ItemId.Material, iCount, MaxSlots: vMain.GetMaxSlots(vehicle.Model));
                if (sessionData.TimersData.LoadMatsTimer != null) Timers.Stop(sessionData.TimersData.LoadMatsTimer);
                sessionData.TimersData.LoadMatsTimer = null;
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MatSucLoading), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"loadMaterialsTimer Exception: {e.ToString()}");
            }
        }
        
        public static void Event_PlayerDeath(ExtPlayer player, ExtPlayer entityKiller, uint weapon)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (sessionData.TimersData.LoadMatsTimer != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MatLoadingCancelByDeath), 3000);
                    Timers.Stop(sessionData.TimersData.LoadMatsTimer);
                    sessionData.TimersData.LoadMatsTimer = null;

                    var vehicleLocalData = sessionData.VehicleMats.GetVehicleLocalData();
                    if (vehicleLocalData != null) 
                        vehicleLocalData.LoaderMats = null;
                }
            }
            catch (Exception e)
            {
                Log.Write($"Event_PlayerDeath Exception: {e.ToString()}");
            }
        }

        public static void onPlayerDisconnected(ExtPlayer player, DisconnectionType type, string reason)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (sessionData.TimersData.LoadMatsTimer != null)
                {
                    Timers.Stop(sessionData.TimersData.LoadMatsTimer);
                    sessionData.TimersData.LoadMatsTimer = null;
                    var vehicleLocalData = sessionData.VehicleMats.GetVehicleLocalData();
                    if (vehicleLocalData != null) 
                        vehicleLocalData.LoaderMats = null;
                }
            }
            catch (Exception e)
            {
                Log.Write($"onPlayerDisconnected Exception: {e.ToString()}");
            }
        }

        public static void onVehicleDeath(ExtVehicle vehicle)
        {
            try
            {
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if (vehicleLocalData.LoaderMats != null)
                    {
                        var player = vehicleLocalData.LoaderMats;
                        var sessionData = player.GetSessionData();
                        if (sessionData == null) return;
                        if (!player.IsCharacterData()) return;
                        if (sessionData.TimersData.LoadMatsTimer != null)
                        {
                            Timers.Stop(sessionData.TimersData.LoadMatsTimer);
                            sessionData.TimersData.LoadMatsTimer = null;
                            vehicleLocalData.LoaderMats = null;
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MatLoadingCancelByCheckpoint), 3000);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"onVehicleDeath Exception: {e.ToString()}");
            }
        }
    }
}
