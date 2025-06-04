using System; 
using System.Collections.Generic; 
using GTANetworkAPI; 
using NeptuneEvo.Core; 
using Redage.SDK; 
using NeptuneEvo.GUI; 
 
using NeptuneEvo.Chars.Models; 
using NeptuneEvo.Chars; 
using NeptuneEvo.Functions; 
using System.Linq;
using Localization;
using NeptuneEvo.Accounts; 
using NeptuneEvo.Players.Models; 
using NeptuneEvo.Players; 
using NeptuneEvo.Character.Models; 
using NeptuneEvo.Character;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Handles;
using NeptuneEvo.Table.Models;

namespace NeptuneEvo.Fractions 
{ 
    class Cityhall : Script 
    { 
        private static readonly nLog Log = new nLog("Fractions.Cityhall"); 
        public static int lastHourTax = 0; 
        public static int canGetMoney = 999999; 
 
        public static Vector3 CloakroomPosition = new Vector3(-1300.8214, -555.6375, 30f); 
        public static Vector3 SecondCloakroomPosition = new Vector3(2522.4473f, -330.00107f, 94f); 
        private static Vector3 StockPosition = new Vector3(-1299.6835, -557.7507, 30f); 
        public static Vector3 GiveGunPosition = new Vector3(-1301.9441, -556.87427, 30.573f); 
        public static Vector3 SecondGiveGunPosition = new Vector3(2487.9102f, -376.6428f, 82.69f); 
        public static Vector3 GpsPosition = new Vector3(-1290.6262, -571.5192, 31.37f); 
        private static Vector3 CallingEmployeesPosition = new Vector3(-1289.8119, -572.5035, 30.573f);

        private static Vector3 JobPosition = new Vector3(-1298.496, -567.3785, 30.570717);
        //private static Vector3 JobPosition1 = new Vector3(727.5652, 132.56389, 80.956436);
        // private static Vector3 JobPosition2 = new Vector3(901.8491, -174.96883, 74.047905); 
        //private static Vector3 JobPosition3 = new Vector3(133.08955, 96.42902, 83.50762); 
        private static Vector3 JobPosition4 = new Vector3(-116.56201, -604.71436, 36.28072);
        //private static Vector3 JobPosition5 = new Vector3(-1331.7507, 44.325035, 53.5806);
        //private static Vector3 JobPosition6 = new Vector3(324.63855, 3425.9546, 36.49877); 
        //  private static Vector3 JobPosition7 = new Vector3(-1464.0969, -500.23605, 32.961685); 
        //private static Vector3 JobPosition8 = new Vector3(472.15143, -1282.2974, 29.554743);


        public static Vector3[] CityhallLiftCheckpoints = new Vector3[9] 
        { 
            new Vector3(-1307.3405, -557.72363, 20.80231), // 1 лифт - 0 этаж 
            new Vector3(-1307.2377, -562.0574, 30.572676), // 1 лифт - 1 этаж 
            new Vector3(-1307.2377, -562.0574, 34.21815), // 1 лифт - 2 этаж 
            new Vector3(-1307.2377, -562.0574, 37.16501), // 1 лифт - 3 этаж 
            new Vector3(-1309.4176, -559.3115, 20.802534), // 2 лифт - 0 этаж 
            new Vector3(-1309.4775, -563.391, 30.572876), // 2 лифт - 1 этаж 
            new Vector3(-1309.4775, -563.391, 34.05086), //  2 лифт - 2 этаж 
            new Vector3(-1309.4775, -563.391, 37.589138), //  2 лифт - 3 этаж 
            new Vector3(-1309.1863, -563.7529, 41.067867) //  2 лифт - 4 этаж 
        }; 
 
        public static Vector3[] CourtCheckpoints = new Vector3[2] 
        { 
            new Vector3(254.4608, -1084.0637, 29.294258), // Лифт - 1 этаж 
            new Vector3(254.84592, -1083.8599, 36.13297), // Лифт - 2 этаж 
        }; 
         
        public static Vector3[] SecondGovLiftCoords = new Vector3[3] 
        { 
            new Vector3(2504.4695, -342.07602, 94), // Лифт - 1 этаж 
            new Vector3(2504.4695, -342.07602, 101.89), // Лифт - 3 этаж 
            new Vector3(2504.4695, -342.07602, 105.68), // Лифт - 4 этаж 
        }; 
 
        public static Vector3[] FirstCityhallLiftTP = new Vector3[4] 
        { 
            new Vector3(-1307.3405, -557.72363, 20.80231), // 1 лифт - 0 этаж 
            new Vector3(-1307.2377, -562.0574, 30.572676), // 1 лифт - 1 этаж 
            new Vector3(-1307.2377, -562.0574, 34.21815), // 1 лифт - 2 этаж 
            new Vector3(-1307.2377, -562.0574, 37.16501) // 1 лифт - 3 этаж 
        }; 
 
        public static Vector3[] SecondCityhallLiftTP = new Vector3[5] 
        { 
            new Vector3(-1309.4176, -559.3115, 20.802534), // 2 лифт - 0 этаж 
            new Vector3(-1309.4775, -563.391, 30.572876), // 2 лифт - 1 этаж 
            new Vector3(-1309.4775, -563.391, 34.05086), //  2 лифт - 2 этаж 
            new Vector3(-1309.4775, -563.391, 37.589138), //  2 лифт - 3 этаж 
            new Vector3(-1309.1863, -563.7529, 41.067867) //  2 лифт - 4 этаж 
        }; 
 
        public static Vector3[] CourtLiftTP = new Vector3[2] 
        { 
            new Vector3(254.4608, -1084.0637, 29.294258), // Лифт - 1 этаж 
            new Vector3(254.84592, -1083.8599, 36.13297) // Лифт - 2 этаж 
        }; 
 
        [Interaction(ColShapeEnums.FractionCityhallOld)] 
        public static void OnFractionCityhallOld(ExtPlayer player, int index) 
        { 
            var sessionData = player.GetSessionData(); 
            if (sessionData == null) return; 
            else if (sessionData.CuffedData.Cuffed) 
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
 
            Trigger.ClientEvent(player, "client.quest.open", index, "npc_cityhall", 0, 0, 0); 
 
        } 
        [ServerEvent(Event.ResourceStart)] 
        public void OnResourceStartHandler() 
        { 
            try 
            { 
                //Main.CreateBlip(new Main.BlipData(446, "Старая Мэрия", new Vector3(-545.02136, -204.28348, 38.2), 4, true)); 
                //PedSystem.Repository.CreateQuest("u_f_y_mistress", new Vector3(-545.02136, -204.28348, 38.2), -151.96516f, title: "~y~NPC~w~ Эльнара Каримова\nБудущий гастарбайтер", colShapeEnums: ColShapeEnums.FractionCityhallOld); 
 
                Main.CreateBlip(new Main.BlipData(419, "Правительство", GpsPosition, 4, true, 1.5f)); 
                //Main.CreateBlip(new Main.BlipData(419, "Правительство", new Vector3(2515.921, -357.68134, 94), 4, true, 1.5f)); 
                Main.CreateBlip(new Main.BlipData(181, "Суд", new Vector3(243.40225, -1073.7192, 29), 4, true)); 
 
                CustomColShape.CreateCylinderColShape(CloakroomPosition, 1, 2, 0, ColShapeEnums.FractionCityhallBeginWorkDay, 2); 
                NAPI.Marker.CreateMarker(30, CloakroomPosition, new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220)); 
 
                CustomColShape.CreateCylinderColShape(SecondCloakroomPosition, 1, 2, 0, ColShapeEnums.FractionCityhallBeginWorkDay, 2); 
                NAPI.Marker.CreateMarker(30, SecondCloakroomPosition, new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220)); 
 
                CustomColShape.CreateCylinderColShape(StockPosition, 1, 2, 0, ColShapeEnums.FractionCityhall, 3); 
                NAPI.Marker.CreateMarker(20, StockPosition, new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220)); 
 
                PedSystem.Repository.CreateQuest("s_m_y_devinsec_01", GiveGunPosition, -139.39449f, title: "~y~NPC~w~ Энтони Янг\nВыдача оружия", colShapeEnums: ColShapeEnums.FractionCityhallGunMenu);
                PedSystem.Repository.CreateQuest("s_m_y_devinsec_01", SecondGiveGunPosition, -129.7344f, title: "~y~NPC~w~ Джонни Ганз\nВыдача оружия", colShapeEnums: ColShapeEnums.FractionCityhallGunMenu); 
                PedSystem.Repository.CreateQuest("mp_f_execpa_02", JobPosition, -112.454f, title: "~y~NPC~w~ Эмма Смит\nУстройство на работу", colShapeEnums: ColShapeEnums.JobSelect);
                //PedSystem.Repository.CreateQuest("mp_f_execpa_02", JobPosition1, -50.89626f, title: "~y~NPC~w~ Эмма Смит\nУстройство на работу", colShapeEnums: ColShapeEnums.JobSelect);
                //  PedSystem.Repository.CreateQuest("mp_f_execpa_02", JobPosition2, -159.75824f, title: "~y~NPC~w~ Эмма Смит\nУстройство на работу", colShapeEnums: ColShapeEnums.JobSelect); 
                // PedSystem.Repository.CreateQuest("mp_f_execpa_02", JobPosition3, -81.50282f, title: "~y~NPC~w~ Эмма Смит\nУстройство на работу", colShapeEnums: ColShapeEnums.JobSelect); 
                PedSystem.Repository.CreateQuest("mp_f_execpa_02", JobPosition4, -111.755f, title: "~y~NPC~w~ Эмма Смит\nУстройство на работу", colShapeEnums: ColShapeEnums.JobSelect);
                //PedSystem.Repository.CreateQuest("mp_f_execpa_02", JobPosition5, -90.612854f, title: "~y~NPC~w~ Эмма Смит\nУстройство на работу", colShapeEnums: ColShapeEnums.JobSelect);
                //PedSystem.Repository.CreateQuest("mp_f_execpa_02", JobPosition6, -96.25076f, title: "~y~NPC~w~ Эмма Смит\nУстройство на работу", colShapeEnums: ColShapeEnums.JobSelect); 
                //PedSystem.Repository.CreateQuest("mp_f_execpa_02", JobPosition7, -9.802892f, title: "~y~NPC~w~ Эмма Смит\nУстройство на работу", colShapeEnums: ColShapeEnums.JobSelect); 
                //PedSystem.Repository.CreateQuest("mp_f_execpa_02", JobPosition8, -87.92568f, title: "~y~NPC~w~ Эмма Смит\nУстройство на работу", colShapeEnums: ColShapeEnums.JobSelect);



                for (int i = 0; i < CityhallLiftCheckpoints.Length; i++) 
                { 
                    CustomColShape.CreateCylinderColShape(CityhallLiftCheckpoints[i], 1, 2, 0, ColShapeEnums.FractionCityhall, i < 4 ? 10 : 11); 
                    NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Лифт"), new Vector3(CityhallLiftCheckpoints[i].X, CityhallLiftCheckpoints[i].Y, CityhallLiftCheckpoints[i].Z), 5F, 0.3F, 0, new Color(255, 255, 255)); 
                    NAPI.Marker.CreateMarker(1, CityhallLiftCheckpoints[i] - new Vector3(0, 0, 1.25), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220)); 
                } 
                 
                for (int i = 0; i < CourtCheckpoints.Length; i++) 
                { 
                    CustomColShape.CreateCylinderColShape(CourtCheckpoints[i], 1, 2, 0, ColShapeEnums.FractionCityhall, 12); 
                    NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Лифт"), new Vector3(CourtCheckpoints[i].X, CourtCheckpoints[i].Y, CourtCheckpoints[i].Z), 5F, 0.3F, 0, new Color(255, 255, 255)); 
                    NAPI.Marker.CreateMarker(1, CourtCheckpoints[i] - new Vector3(0, 0, 1.25), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220)); 
                } 
                 
                for (int i = 0; i < SecondGovLiftCoords.Length; i++) 
                { 
                    CustomColShape.CreateCylinderColShape(SecondGovLiftCoords[i], 1, 2, 0, ColShapeEnums.FractionCityhall, 13); 
                    NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Лифт"), new Vector3(SecondGovLiftCoords[i].X, SecondGovLiftCoords[i].Y, SecondGovLiftCoords[i].Z), 5F, 0.3F, 0, new Color(255, 255, 255)); 
                    NAPI.Marker.CreateMarker(1, SecondGovLiftCoords[i] - new Vector3(0, 0, 1.25), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220)); 
                } 
            } 
            catch (Exception e) 
            { 
                Log.Write($"OnResourceStartHandler Exception: {e.ToString()}"); 
            } 
        } 
 
        [Interaction(ColShapeEnums.CallGovMember)] 
        public static void OpenCallGovMemberDialog(ExtPlayer player, int _) 
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
 
                Trigger.ClientEvent(player, "openDialog", "CallGovMemberDialog", LangFunc.GetText(LangType.Ru, DataName.AreYouWantToCallGov)); 
            } 
            catch (Exception e) 
            { 
                Log.Write($"OpenCallGovMemberDialog Exception: {e.ToString()}"); 
            } 
        }

        [Interaction(ColShapeEnums.FractionCityhall)] 
        public static void OnFractionCityhall(ExtPlayer player, int interact) 
        { 
            try 
            { 
                var sessionData = player.GetSessionData(); 
                if (sessionData == null) return; 
					
                switch (interact) 
                { 
                    case 2: 
                        SafeMain.OpenSafedoorMenu(player); 
                        return; 
                    case 3: 

                        var fractionData = player.GetFractionData();
                        if (fractionData == null || fractionData.Id != (int) Models.Fractions.CITY) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NotGOV), 3000); 
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
                        sessionData.OnFracStock = 6; 
                        Chars.Repository.LoadOtherItemsData(player, "Fraction", "6", 5, Chars.Repository.InventoryMaxSlots["Fraction"]); 
                        return; 
                    case 10: 
                    case 11: 
                    case 12: 
                    case 13: 
                        if (sessionData.Following != null) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsFollowing), 3000); 
                            return; 
                        } 
 
                        Trigger.ClientEvent(player, "openCityhallLiftMenu", interact); 
                        return; 
                    default: 
                        // Not supposed to end up here.  
                        break; 
                } 
            } 
            catch (Exception e) 
            { 
                Log.Write($"interactPressed Exception: {e.ToString()}"); 
            } 
        } 
 
        [RemoteEvent("server.useCityhallLift")] 
        public static void UseCityhallLift(ExtPlayer player, byte lift, int index) 
        { 
            try 
            { 
                var sessionData = player.GetSessionData(); 
                if (sessionData == null) return; 
 
                var characterData = player.GetCharacterData(); 
                if (characterData == null) return; 
 
                if (characterData.DemorganTime >= 1 || sessionData.CuffedData.Cuffed || sessionData.DeathData.InDeath || sessionData.AntiAnimDown || !characterData.IsAlive) return; 
                 
                if (lift == 1) 
                { 
                    if (player.Position.DistanceTo(FirstCityhallLiftTP[index]) <= 2) 
                    { 
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyAtFloor), 3000); 
                        return; 
                    } 
 
                    player.Position = FirstCityhallLiftTP[index]; 
                } 
                else if (lift == 2) 
                { 
                    if (player.Position.DistanceTo(SecondCityhallLiftTP[index]) <= 2) 
                    { 
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyAtFloor), 3000); 
                        return; 
                    } 
 
                    player.Position = SecondCityhallLiftTP[index]; 
                } 
                else if (lift == 3) 
                { 
                    if (player.Position.DistanceTo(CourtLiftTP[index]) <= 2) 
                    { 
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyAtFloor), 3000); 
                        return; 
                    } 
 
                    player.Position = CourtLiftTP[index]; 
                    index += 1; 
                } 
                else if (lift == 4) 
                { 
                    if (player.Position.DistanceTo(SecondGovLiftCoords[index]) <= 2) 
                    { 
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyAtFloor), 3000); 
                        return; 
                    } 
 
                    player.Position = SecondGovLiftCoords[index]; 
                    index = index == 0 ? index += 1 : index += 2; 
                } 
 
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GoingToFloor, index), 3000); 
            } 
            catch (Exception e) 
            { 
                Log.Write($"UseCityhallLift Exception: {e.ToString()}"); 
            } 
        } 
        
        [Interaction(ColShapeEnums.FractionCityhallBeginWorkDay)] 
        public static void OnFractionCityhallBeginWorkDay(ExtPlayer player) 
        { 
            try 
            { 
                if (player.GetFractionId() == (int) Models.Fractions.CITY) FractionClothingSets.OpenFractionClothingSetsMenu(player);
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NotGOV), 3000); 
            } 
            catch (Exception e) 
            { 
                Log.Write($"beginWorkDay Exception: {e.ToString()}"); 
            } 
        } 
 
        [Interaction(ColShapeEnums.FractionCityhallGunMenu)] 
        public static void OnFractionCityhallGunMenu(ExtPlayer player) 
        { 
            try 
            { 
                var fractionData = player.GetFractionData();
                if (fractionData == null || fractionData.Id != (int) Models.Fractions.CITY) 
                { 
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoDostup), 3000); 
                    return; 
                } 
                if (!fractionData.IsOpenStock) 
                { 
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseClosed), 3000); 
                    return; 
                } 
                Manager.OpenFractionSM(player, "gov"); 
            } 
            catch (Exception e) 
            { 
                Log.Write($"OnFractionCityhallGunMenu Exception: {e.ToString()}"); 
            } 
        } 
        [RemoteEvent("govgun")] 
        public static void callback_cityhallGuns(ExtPlayer player, int index) 
        { 
            try 
            { 
                var sessionData = player.GetSessionData(); 
                if (sessionData == null) return; 
 
                if (player.Position.DistanceTo(GiveGunPosition) >= 5) 
                { 
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TooFar), 3000); 
                    return; 
                } 

                var fractionData = player.GetFractionData();
                if (fractionData == null)
                    return;
                
                if (fractionData.Id != (int) Models.Fractions.CITY) 
                    return; 
                
                switch (index) 
                { 
                    case 0: //"stungun": 
                        Manager.giveGun(player, WeaponRepository.Hash.StunGun, "StunGun"); 
                        return; 
                    case 1: //"pistol": 
                        Manager.giveGun(player, WeaponRepository.Hash.Pistol, "Pistol"); 
                        return; 
                    case 2: //"assaultrifle": 
                        Manager.giveGun(player, WeaponRepository.Hash.AdvancedRifle, "AssaultRifle"); 
                        return; 
                    case 3: //"gusenberg": 
                        Manager.giveGun(player, WeaponRepository.Hash.Gusenberg, "Gusenberg"); 
                        return; 
                    case 4: //"armor": 
                        if (!Manager.canGetWeapon(player, "Armor")) return; 
                        if (fractionData.Materials < 250) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseNoMats), 3000); 
                            return; 
                        } 
                        else if (Chars.Repository.itemCount(player, "inventory", ItemId.BodyArmor) >= Chars.Repository.maxItemCount) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyHaveBronik), 3000); 
                            return; 
                        } 
                        else if (Chars.Repository.AddNewItem(player, $"char_{player.GetUUID()}", "inventory", ItemId.BodyArmor, 1, 100.ToString()) == -1) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000); 
                            return; 
                        } 
                        fractionData.Materials -= 250; 
                        fractionData.UpdateLabel(); 
                        GameLog.Stock((int) Models.Fractions.CITY, player.GetUUID(), player.Name, "armor", 1, "out"); 
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GetBronik), 3000); 
                        Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMats, LangFunc.GetText(LangType.Ru, DataName.CraftedBronik)); 
                        return; 
                    case 5: 
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
                        GameLog.Stock((int) Models.Fractions.CITY, player.GetUUID(), player.Name, "medkit", 1, "out"); 
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GotApteka), 3000); 
                        Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMedkits, LangFunc.GetText(LangType.Ru, DataName.GetApteka)); 
                        return; 
                    case 6: 
                        if (Manager.canGetWeapon(player, "PistolAmmo")) Manager.giveAmmo(player, ItemId.PistolAmmo, 12); 
                        return; 
                    case 7: 
                        if (Manager.canGetWeapon(player, "SMGAmmo")) Manager.giveAmmo(player, ItemId.SMGAmmo, 30); 
                        return; 
                    case 8: 
                        if (Manager.canGetWeapon(player, "RiflesAmmo")) Manager.giveAmmo(player, ItemId.RiflesAmmo, 30); 
                        return; 
                    case 9: 
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
                        Chars.Repository.RemoveIndex(player, "inventory", aItem.Index, 1); 
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
                Log.Write($"callback_cityhallGuns Exception: {e.ToString()}"); 
            } 
        }
    } 
} 
