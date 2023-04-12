using GTANetworkAPI;
using NeptuneEvo.Handles;
using System.Collections.Generic;
using System;
using System.Linq;
using Localization;
using NeptuneEvo.Core;
using Redage.SDK;
using NeptuneEvo.Houses;
using NeptuneEvo.Functions;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Jobs.Models;
using NeptuneEvo.Chars;
using NeptuneEvo.Players.Popup.List.Models;
using NeptuneEvo.Quests;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;

namespace NeptuneEvo.Jobs
{
    class Gopostal : Script
    {
        private static readonly nLog Log = new nLog("Jobs.Gopostal");

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                CustomColShape.CreateCylinderColShape(Coords[0], 1, 2, 0, ColShapeEnums.JobGoPosta); // start work
                NAPI.TextLabel.CreateTextLabel(LangFunc.GetText(LangType.Ru, DataName.PostHouse), Coords[0] + new Vector3(0, 0, 0.3), 10F, 0.6F, 0, new Color(0, 180, 0));

                //CustomColShape.CreateCylinderColShape(Coords[1], 3, 2, 0, ColShapeEnums.JobGoPostaCar); // get car
                //NAPI.TextLabel.CreateTextLabel(LangFunc.GetText(LangType.Ru, DataName.TakeWorkVeh), Coords[1] + new Vector3(0, 0, 0.3), 10F, 0.6F, 0, new Color(0, 180, 0));

                NAPI.Marker.CreateMarker(1, Coords[0] - new Vector3(0, 0, 1.5), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220));
                //NAPI.Marker.CreateMarker(1, Coords[1] - new Vector3(0, 0, 2.0), new Vector3(), new Vector3(), 3f, new Color(255, 255, 255, 220));
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }

        public static List<Vector3> Coords = new List<Vector3>()
        {
            new Vector3(437.70697, -624.46344, 27.608513 + 1.12f), // start work
            new Vector3(117.6746, 100.341, 80.98235), // get car
        };


        [Interaction(ColShapeEnums.JobGoPosta)]
        public static void OnJobGoPosta(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                if (characterData.WorkID != (int)JobsId.Postman)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не работаете курьером.", 3000);
                    return;
                }
                
                var frameList = new FrameListData(); 
                frameList.Header = LangFunc.GetText(LangType.Ru, DataName.Warehouse); 
                frameList.Callback = callback_gpStartMenu;

                if (sessionData.WorkData.OnWork)
                {
                    frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.EndJob), "finish"));
                    frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.TakeParcels), "get"));
                    
                }else
                    frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.StartJob), "start"));

                Players.Popup.List.Repository.Open(player, frameList);  
                
            }
            catch (Exception e)
            {
                Log.Write($"openGoPostalStart Exception: {e.ToString()}");
            }
        }

        private static void callback_gpStartMenu(ExtPlayer player, object listItem)
        {
            try
            {
                if (!(listItem is string))
                    return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (player.Position.DistanceTo(Gopostal.Coords[0]) > 15)
                    return;
                
                switch (listItem)
                {
                    case "start":
                        StartWork(player);
                        return;
                    case "get":
                        if (characterData.WorkID != (int)JobsId.Postman)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoPostman), 3000);
                            return;
                        }
                        if (!sessionData.WorkData.OnWork)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000);
                            return;
                        }
                        if (sessionData.WorkData.Packages != 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouNotRazdaliPosilki, sessionData.WorkData.Packages), 3000);
                            return;
                        }          
                        
                        byte count = 10;
                        (byte, float) jobLevelInfo = characterData.JobSkills.ContainsKey((int)JobsId.Postman) ? Main.GetPlayerJobLevelBonus((int)JobsId.Postman, characterData.JobSkills[(int)JobsId.Postman]) : (0, 1);
                        if (jobLevelInfo.Item1 == 3) count = 15;
                        else if (jobLevelInfo.Item1 >= 4) count = 20;

                        sessionData.WorkData.Packages = Main.ServerNumber == 0 ? 2 : count;
                        
                        SetHousePoint(player);

                        Attachments.AddAttachment(player, Attachments.AttachmentsName.Postal);
                        return;
                    case "finish":
                        EndWork(player);
                        return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"callback_gpStartMenu Exception: {e.ToString()}");
            }
        }
        public static void StartWork(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            if (Houses.HouseManager.Houses.Count == 0) return;

            byte count = 10;
            (byte, float) jobLevelInfo = characterData.JobSkills.ContainsKey((int)JobsId.Postman) ? Main.GetPlayerJobLevelBonus((int)JobsId.Postman, characterData.JobSkills[(int)JobsId.Postman]) : (0, 1);
            if (jobLevelInfo.Item1 == 3) count = 15;
            else if (jobLevelInfo.Item1 >= 4) count = 20;

            sessionData.WorkData.Packages = Main.ServerNumber == 0 ? 2 : count;
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PostmanStartJob, count), 3000);
            sessionData.WorkData.OnWork = true;
            
            SetHousePoint(player);

            var gender = characterData.Gender;
            //ClothesComponents.ClearClothes(player, gender);
            if (gender)
            {
                ClothesComponents.SetSpecialClothes(player, 0,  76, 10);
                ClothesComponents.SetSpecialClothes(player, 8, 38, 3);
                ClothesComponents.SetSpecialClothes(player, 4, 17, 0);
                ClothesComponents.SetSpecialClothes(player, 6, 1, 7);
            }
            else
            {
                ClothesComponents.SetSpecialClothes(player, 0,  75, 10);
                ClothesComponents.SetSpecialClothes(player, 8, 0, 6);
                ClothesComponents.SetSpecialClothes(player, 4, 25, 2);
                ClothesComponents.SetSpecialClothes(player, 6, 1, 2);
            }
            Attachments.AddAttachment(player, Attachments.AttachmentsName.Postal);
            Chars.Repository.LoadAccessories(player);
        }
        public static bool EndWork(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return false;
            
            if (sessionData.WorkData.OnWork)
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.EndWorkDay), 3000);
                sessionData.WorkData.OnWork = false;
                sessionData.WorkData.Packages = 0;
                Customization.ApplyCharacter(player);
                Attachments.RemoveAttachment(player, Attachments.AttachmentsName.Postal);

                Trigger.ClientEvent(player, "deleteCheckpoint", 1, 0);
                Trigger.ClientEvent(player, "deleteWorkBlip");
                return true;
            }
            return false;
        }
        
        [ServerEvent(Event.PlayerEnterVehicle)]
        public void OnPlayerEnterVehicle(ExtPlayer player, ExtVehicle vehicle, sbyte seatId)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            if (characterData.WorkID != (int)JobsId.Postman) 
                return;
            
            if (sessionData.WorkData.Packages > 0)
                Attachments.RemoveAttachment(player, Attachments.AttachmentsName.Postal);
        }
        
        [ServerEvent(Event.PlayerExitVehicle)]
        public void OnPlayerExitVehicle(ExtPlayer player, ExtVehicle vehicle)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            if (characterData.WorkID != (int)JobsId.Postman) 
                return;
            
            if (sessionData.WorkData.Packages > 0)
                Attachments.AddAttachment(player, Attachments.AttachmentsName.Postal);
        }
        
        private static void SetHousePoint(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;

            var playerPosition = player.Position;

            var houses = HouseManager.Houses
                .Where(h => h.Type != 7)
                .Where(h => h.Position.DistanceTo2D(playerPosition) >= 200)
                .ToList();
            
            var rand = new Random();
            var houseId = rand.Next(0, houses.Count - 1);
            var house = houses[houseId];

            sessionData.WorkData.Position = player.Position;
            sessionData.WorkData.Time = DateTime.Now;
            sessionData.WorkData.WorkWay = house.ID;

            Trigger.ClientEvent(player, "createCheckpoint", 1, 1,house.Position, 1, 0, 255, 0, 0);
            Trigger.ClientEvent(player, "createWaypoint", house.Position.X, house.Position.Y);
            Trigger.ClientEvent(player, "createWorkBlip", house.Position);
        }
        
        public static void GoPostal_onEntityEnterColShape(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                var accountData = player.GetAccountData();
                if (accountData == null) 
                    return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
                
                if (HouseManager.Houses.Count == 0) 
                    return;
                
                if (characterData.WorkID != (int)JobsId.Postman || !sessionData.WorkData.OnWork) 
                    return;
                
                if (sessionData.WorkData.WorkWay != sessionData.HouseID) 
                    return;
                
                if (NAPI.Player.IsPlayerInAnyVehicle(player))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LeaveJobVeh), 3000);
                    return;
                }
                if (sessionData.WorkData.Packages == 0 || !Attachments.HasAttachment(player, Attachments.AttachmentsName.Postal))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нет посылки в руке", 3000);
                    return;
                }

                (byte, float) jobLevelInfo = characterData.JobSkills.ContainsKey((int)JobsId.Postman) ? Main.GetPlayerJobLevelBonus((int)JobsId.Postman, characterData.JobSkills[(int)JobsId.Postman]) : (0, 1);

                if (jobLevelInfo.Item1 <= 4)
                {
                    var vehiclePost = sessionData.RentData?.Vehicle;
                    if (vehiclePost == null || !vehiclePost.Exists || player.Position.DistanceTo(vehiclePost.Position) >= 50)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.JobVehFar), 3000);
                        return;
                    }
                }
                
                int coef = Convert.ToInt32(player.Position.DistanceTo2D(sessionData.WorkData.Position) / 100);
                DateTime lastTime = sessionData.WorkData.Time;
                if (Main.ServerNumber != 0 && DateTime.Now < lastTime.AddSeconds(coef * 2))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoHozyain), 3000);
                    return;
                }
                int payment = Convert.ToInt32(coef * Main.PostalPayment * Group.GroupPayAdd[accountData.VipLvl] * Main.ServerSettings.MoneyMultiplier);
                
                int maxpayment = 3500 * Main.ServerSettings.MoneyMultiplier;
                if (payment > maxpayment) 
                    payment = maxpayment;
                
                if (jobLevelInfo.Item1 >= 1) 
                    payment = Convert.ToInt32(payment * jobLevelInfo.Item2);

                MoneySystem.Wallet.Change(player, payment);
                GameLog.Money($"server", $"player({characterData.UUID})", payment, $"postalCheck");
                
                sessionData.WorkData.Packages -= 1;
                BattlePass.Repository.UpdateReward(player, 20);
                BattlePass.Repository.UpdateReward(player, 154);
                Attachments.RemoveAttachment(player, Attachments.AttachmentsName.Postal);
                
                if (sessionData.WorkData.Packages >= 1)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MailsLeft, sessionData.WorkData.Packages), 8000);
                    
                    SetHousePoint(player);

                    Trigger.PlayAnimation(player, "anim@heists@narcotics@trash", "drop_side", -1, false);
                    // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "gopostal");
                }
                else
                {
                    Trigger.ClientEvent(player, "deleteCheckpoint", 1, 0);
                    Trigger.ClientEvent(player, "deleteWorkBlip");
                    Trigger.ClientEvent(player, "createWaypoint", 436.5074f, -627.4617f);
                    
                    Trigger.PlayAnimation(player, "anim@heists@narcotics@trash", "drop_side", -1, false);
                    sessionData.WorkData.Packages = 0;
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMails), 3000);
                    
                }
                
                if (qMain.GetQuestsLine(player, Zdobich.QuestName) == (int)zdobich_quests.Stage11)
                {
                    sessionData.WorkData.PointsCount += payment;
                    if (sessionData.WorkData.PointsCount < qMain.GetQuestsData(player, Zdobich.QuestName, (int) zdobich_quests.Stage11))
                        sessionData.WorkData.PointsCount = qMain.GetQuestsData(player, Zdobich.QuestName, (int) zdobich_quests.Stage11) + payment;
                
                    if (sessionData.WorkData.PointsCount >= 500)
                    {
                        qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage11, 1, isUpdateHud: true);
                        qMain.UpdateQuestsComplete(player, Zdobich.QuestName, (int) zdobich_quests.Stage11, true);
                        Trigger.SendChatMessage(player, "!{#fc0}" + LangFunc.GetText(LangType.Ru, DataName.QuestPartComplete));
                    }
                    else
                    {
                        qMain.UpdateQuestsData(player, Zdobich.QuestName, (int)zdobich_quests.Stage11, sessionData.WorkData.PointsCount.ToString());
                        //todo translate (было DataName.PointsQuestGot)
                        Trigger.SendChatMessage(player, LangFunc.GetText(LangType.Ru, DataName.YouEarnedJob, sessionData.WorkData.PointsCount, 500 - sessionData.WorkData.PointsCount));
                    }
                }
                
                if (characterData.JobSkills.ContainsKey((int)JobsId.Postman))
                {
                    if (characterData.JobSkills[(int)JobsId.Postman] < 4000)
                        characterData.JobSkills[(int)JobsId.Postman] += 1;
                }
                else characterData.JobSkills.Add((int)JobsId.Postman, 1);
            }
            catch (Exception e)
            {
                Log.Write($"GoPostal_onEntityEnterColShape Exception: {e.ToString()}");
            }
        }
    }
}
