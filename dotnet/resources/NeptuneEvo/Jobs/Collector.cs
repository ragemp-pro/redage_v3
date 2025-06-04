using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using Redage.SDK;
using System;
using Localization;
using NeptuneEvo.Jobs.Models;
using NeptuneEvo.Quests;

namespace NeptuneEvo.Jobs
{
    class Collector : Script
    {
        private static readonly nLog Log = new nLog("Jobs.Collector");

        private static Vector3 TakeMoneyPos = new Vector3(915.9069, -1265.255, 24.50912);

        [ServerEvent(Event.ResourceStart)]
        public void Event_ResourceStart()
        {
            try
            {
                CustomColShape.CreateCylinderColShape(TakeMoneyPos, 1, 3, 0, ColShapeEnums.TakeMoney);
                NAPI.Marker.CreateMarker(1, TakeMoneyPos - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220), false, 0);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16(LangFunc.GetText(LangType.Ru, DataName.TakeMeshki)), TakeMoneyPos + new Vector3(0, 0, 0.3), 30f, 0.4f, 0, new Color(255, 255, 255), true, 0);
            }
            catch (Exception e)
            {
                Log.Write($"Event_ResourceStart Exception: {e}");
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

            var gender = characterData.Gender;
            //ClothesComponents.ClearClothes(player, gender);
            if (characterData.Gender)
            {
                ClothesComponents.SetSpecialClothes(player, 0,  63, 9);
                ClothesComponents.SetSpecialClothes(player, 11, 132, 0);
                ClothesComponents.SetSpecialClothes(player, 4, 33, 0);
                ClothesComponents.SetSpecialClothes(player, 6, 24, 0);
                ClothesComponents.SetSpecialClothes(player, 8, 129, 0);
            }
            else
            {
                ClothesComponents.SetSpecialClothes(player, 0,  63, 9);
                ClothesComponents.SetSpecialClothes(player, 11, 129, 0);
                ClothesComponents.SetSpecialClothes(player, 4, 32, 0);
                ClothesComponents.SetSpecialClothes(player, 6, 24, 0);
                ClothesComponents.SetSpecialClothes(player, 8, 159, 0);
            }
            Chars.Repository.LoadAccessories(player);
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.InkassStart), 3000);
            
            sessionData.WorkData.Packages = Main.ServerNumber == 0 ? 2 : 15;
            Attachments.AddAttachment(player, Attachments.AttachmentsName.MoneyBag);
            
            sessionData.WorkData.OnWork = true;
            SetAtmPoint(player);
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
                Customization.ApplyCharacter(player);                    
                Trigger.ClientEvent(player, "deleteCheckpoint", 16, 0);
                Trigger.ClientEvent(player, "deleteWorkBlip");
                Attachments.RemoveAttachment(player, Attachments.AttachmentsName.MoneyBag);
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
            
            if (characterData.WorkID != (int)JobsId.CashCollector) 
                return;
            
            if (sessionData.WorkData.Packages > 0)
                Attachments.RemoveAttachment(player, Attachments.AttachmentsName.MoneyBag);
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
            
            if (characterData.WorkID != (int)JobsId.CashCollector) 
                return;
            
            if (sessionData.WorkData.Packages > 0)
                Attachments.AddAttachment(player, Attachments.AttachmentsName.MoneyBag);
        }

        private static void SetAtmPoint(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;

            var playerPosition = player.Position;
            
            var rand = new Random();
            int atmId = WorkManager.rnd.Next(0, MoneySystem.ATM.ATMs.Count - 1);
            while (atmId == 58 || atmId == 36 || MoneySystem.ATM.ATMs[atmId].DistanceTo2D(playerPosition) < 200) 
                atmId = rand.Next(0, MoneySystem.ATM.ATMs.Count - 1);
            
            var atm = MoneySystem.ATM.ATMs[atmId];

            sessionData.WorkData.Position = player.Position;
            sessionData.WorkData.Time = DateTime.Now;
            sessionData.WorkData.WorkCheck = atmId;

            Trigger.ClientEvent(player, "createCheckpoint", 16, 29, atm + new Vector3(0, 0, 1.12), 1, 0, 220, 220, 0);
            Trigger.ClientEvent(player, "createWaypoint", atm.X, atm.Y);
            Trigger.ClientEvent(player, "createWorkBlip", atm);
        }
        
        [Interaction(ColShapeEnums.TakeMoney)]
        public static void OnTakeMoney(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (player.IsInVehicle || characterData.WorkID != (int)JobsId.CashCollector || !sessionData.WorkData.OnWork) return;
                if (sessionData.WorkData.Packages != 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveBags, sessionData.WorkData.Packages), 3000);
                    return;
                }
                
                sessionData.WorkData.Packages = Main.ServerNumber == 0 ? 2 : 15;
                Attachments.AddAttachment(player, Attachments.AttachmentsName.MoneyBag);
                SetAtmPoint(player);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GotNewBags), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CollectorTakeMoney Exception: {e}");
            }
        }
        [Interaction(ColShapeEnums.Atm, In: true)]
        public static void InAtm(ExtPlayer player, int Index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var rentData = sessionData.RentData;
                
                if (rentData == null || player.IsInVehicle || characterData.WorkID != (int)JobsId.CashCollector || !sessionData.WorkData.OnWork || sessionData.WorkData.WorkCheck != Index) return;

                if (sessionData.WorkData.Packages == 0 || !Attachments.HasAttachment(player, Attachments.AttachmentsName.MoneyBag))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нет сумки с деньгами в руке", 3000);
                    return;
                }
                
                int coef = Convert.ToInt32(player.Position.DistanceTo2D(sessionData.WorkData.Position) / 100);                
                DateTime lastTime = sessionData.WorkData.Time;
                if (Main.ServerNumber != 0 && DateTime.Now < lastTime.AddSeconds(coef * 2))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AtmFull), 3000);
                    return;
                }
                
                var MyColCar = rentData.Vehicle;

                if (MyColCar == null || !MyColCar.Exists) return;
                if (player.Position.DistanceTo(MyColCar.Position) >= 50)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.JobVehFar), 3000);
                    return;
                }

                sessionData.WorkData.Packages -= 1;

                int payment = Convert.ToInt32(coef * Main.CollectorPayment * Group.GroupPayAdd[accountData.VipLvl] * Main.ServerSettings.MoneyMultiplier);

                int maxpayment = 4000 * Main.ServerSettings.MoneyMultiplier;
                if (payment > maxpayment) payment = maxpayment;

                (byte, float) jobLevelInfo = characterData.JobSkills.ContainsKey((int)JobsId.CashCollector) ? Main.GetPlayerJobLevelBonus((int)JobsId.CashCollector, characterData.JobSkills[(int)JobsId.CashCollector]) : (0, 1);
                if (jobLevelInfo.Item1 >= 1) payment = Convert.ToInt32(payment * jobLevelInfo.Item2);
                
                MoneySystem.Wallet.Change(player, payment);
                GameLog.Money($"server", $"player({characterData.UUID})", payment, $"collectorCheck");
                BattlePass.Repository.UpdateReward(player, 92);
                BattlePass.Repository.UpdateReward(player, 157);
                
                Attachments.RemoveAttachment(player, Attachments.AttachmentsName.MoneyBag);
                if (sessionData.WorkData.Packages == 0)
                {
                    Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BackToBaza), 3000);
                    Trigger.ClientEvent(player, "createWaypoint", TakeMoneyPos.X, TakeMoneyPos.Y);
                    Trigger.ClientEvent(player, "deleteCheckpoint", 16);
                    Trigger.ClientEvent(player, "deleteWorkBlip");
                }
                else
                {
                    SetAtmPoint(player);
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GoNextAtm), 3000);
                }
                
                //
                
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

                if (characterData.JobSkills.ContainsKey((int)JobsId.CashCollector))
                {
                    if (characterData.JobSkills[(int)JobsId.CashCollector] < 3000)
                        characterData.JobSkills[(int)JobsId.CashCollector] += 1;
                }
                else characterData.JobSkills.Add((int)JobsId.CashCollector, 1);
            }
            catch (Exception e)
            {
                Log.Write($"CollectorEnterATM Exception: {e}");
            }
        }
    }
}
