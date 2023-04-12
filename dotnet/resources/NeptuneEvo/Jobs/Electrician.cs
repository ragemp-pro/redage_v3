using GTANetworkAPI;
using NeptuneEvo.Handles;
using System.Collections.Generic;
using System;
using Localization;
using NeptuneEvo.Core;
using Redage.SDK;
using NeptuneEvo.Chars;
using NeptuneEvo.Functions;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Jobs.Models;
using NeptuneEvo.Quests;

namespace NeptuneEvo.Jobs
{
    class Electrician : Script
    {
        private static readonly nLog Log = new nLog("Jobs.Electrician");

        [ServerEvent(Event.ResourceStart)]
        public void Event_ResourceStart()
        {
            try
            {
                NAPI.TextLabel.CreateTextLabel("~w~Ryan Nelson", new Vector3(724.8585, 134.1029, 81.95643), 30f, 0.3f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension);

                if (Main.ServerSettings.IsDeleteProp)
                {

                    NAPI.World.DeleteWorldProp(1046551856, new Vector3(732.2359, 133.4224, 79.84549), 30f);
                    NAPI.World.DeleteWorldProp(1046551856, new Vector3(722.1532, 139.4459, 79.84549), 30f);

                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(707.9461, 165.5156, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(706.5385, 161.6642, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(705.1336, 157.7777, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(703.7278, 153.9064, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(702.3203, 150.0551, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(700.8676, 146.1782, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(699.4619, 142.3069, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(698.0561, 138.4357, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(696.6503, 134.5644, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(695.2429, 130.7126, 79.75075), 30f);

                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(691.3967, 132.1314, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(687.5255, 133.5372, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(683.6542, 134.9429, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(679.7828, 136.3487, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(733542368, new Vector3(679.8405, 136.3211, 80.88264), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(672.0446, 139.1718, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(668.1732, 140.5775, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(664.3019, 141.9833, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(660.4498, 143.3886, 79.75075), 30f);

                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(681.2599, 89.09968, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(682.6656, 92.97095, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(684.0714, 96.84223, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(685.4772, 100.7135, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(686.9298, 104.5904, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(733542368, new Vector3(689.8031, 112.401, 80.82464), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(691.2355, 116.2422, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(692.6412, 120.1135, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(694.047, 123.9848, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(695.4528, 127.8561, 79.75075), 30f);

                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(691.5093, 129.2795, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(687.6381, 130.6853, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(683.7667, 132.091, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(679.8954, 133.4968, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(676.0283, 134.9141, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(672.1571, 136.3199, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(668.2858, 137.7256, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(664.4144, 139.1314, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(660.5624, 140.5368, 79.75075), 30f);

                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(732.0546, 125.8921, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(728.1832, 127.2978, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(724.3121, 128.7036, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(720.4269, 130.1242, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(716.5555, 131.5299, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(712.6843, 132.9357, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(708.8131, 134.3415, 79.75075), 30f);

                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(704.9441, 135.7489, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(703.5383, 131.8776, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(702.1326, 128.0063, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(700.712, 124.1212, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(699.3063, 120.2499, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(697.9005, 116.3786, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(696.4948, 112.5074, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(733542368, new Vector3(695.0624, 108.561, 80.85213), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(693.6404, 104.6123, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(692.2346, 100.741, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(690.8289, 96.86977, 79.75075), 30f);
                    NAPI.World.DeleteWorldProp(-1767254195, new Vector3(689.3761, 92.99287, 79.75075), 30f);
                }

                CustomColShape.CreateCylinderColShape(new Vector3(724.9625, 133.9959, 79.83643), 1, 2, 0, ColShapeEnums.Electrician);

                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(724.9625, 133.9959, 80.95643), 30f, 0.4f, 0, new Color(255, 255, 255), true, 0);
                NAPI.Marker.CreateMarker(1, new Vector3(724.9625, 133.9959, 79.83643) - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220));

                int i = 0;
                foreach (Checkpoint Check in Checkpoints)
                {
                    CustomColShape.CreateCylinderColShape(Check.Position, 1, 2, 0, ColShapeEnums.ElectricianPoint, i);
                    i++;
                }
            }
            catch (Exception e)
            {
                Log.Write($"Event_ResourceStart Exception: {e.ToString()}");
            }
        }
        
        [Interaction(ColShapeEnums.Electrician)]
        public static void OnElectrician(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.WorkID != (int)JobsId.Electrician)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoElectrician), 3000);
                    return;
                }

                if (EndWork(player))
                    return;

                StartWork(player);
            }
            catch (Exception e)
            {
                Log.Write($"StartWorkDay Exception: {e.ToString()}");
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
            if (gender)
            {
                ClothesComponents.SetSpecialAccessories(player,1, 24, 2);
                ClothesComponents.SetSpecialClothes(player, 3, 16, 0);
                ClothesComponents.SetSpecialClothes(player, 11, 153, 10);
                ClothesComponents.SetSpecialClothes(player, 4, 0, 5);
                ClothesComponents.SetSpecialClothes(player, 6, 24, 0);
            }
            else
            {
                ClothesComponents.SetSpecialAccessories(player,1, 26, 2);
                ClothesComponents.SetSpecialClothes(player, 3, 17, 0);
                ClothesComponents.SetSpecialClothes(player, 11, 150, 1);
                ClothesComponents.SetSpecialClothes(player, 4, 1, 5);
                ClothesComponents.SetSpecialClothes(player, 6, 52, 0);
            }
            Chars.Repository.LoadAccessories(player);

            int check = WorkManager.rnd.Next(0, Checkpoints.Count - 1);
            sessionData.WorkData.WorkCheck = check;
            sessionData.WorkData.OnWork = true;
            Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checkpoints[check].Position, 1, 0, 255, 0, 0);
            Trigger.ClientEvent(player, "createWorkBlip", Checkpoints[check].Position);
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.StartWorkDay), 3000);
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
                Trigger.ClientEvent(player, "deleteCheckpoint", 15);
                Trigger.ClientEvent(player, "deleteWorkBlip");
                return true;
            }
            return false;
        }
        
        private static List<Checkpoint> Checkpoints = new List<Checkpoint>()
        {
            new Checkpoint(new Vector3(678.6784, 163.7561, 79.80791), 338.0567),
            new Checkpoint(new Vector3(697.9194, 158.3429, 79.8203), 162.1701),
            new Checkpoint(new Vector3(696.8144, 149.2776, 79.83644), 174.3819),
            new Checkpoint(new Vector3(701.6469, 110.9194, 79.81911), 163.4535),
            new Checkpoint(new Vector3(697.663, 104.4758, 79.63456), 162.01),
            new Checkpoint(new Vector3(658.8223, 114.3996, 79.80294), 346.9411),
            new Checkpoint(new Vector3(663.0648, 122.4777, 79.80295), 345.3615),
            new Checkpoint(new Vector3(671.8508, 145.1318, 79.80048), 345.2057),
        };
        [Interaction(ColShapeEnums.ElectricianPoint, In: true)]
        public void InElectricianPoint(ExtPlayer player, int shapeId)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.WorkID != (int)JobsId.Electrician || !sessionData.WorkData.OnWork || shapeId != sessionData.WorkData.WorkCheck) return;
                if (Checkpoints[shapeId].Position.DistanceTo(player.Position) > 3) return;

                int payment = Convert.ToInt32(Main.ElectricianPayment * Group.GroupPayAdd[accountData.VipLvl] * Main.ServerSettings.MoneyMultiplier);

                (byte, float) jobLevelInfo = characterData.JobSkills.ContainsKey(0) ? Main.GetPlayerJobLevelBonus(0, characterData.JobSkills[0]) : (0, 1);
                if (jobLevelInfo.Item1 >= 1) payment = Convert.ToInt32(payment * jobLevelInfo.Item2);
                
                MoneySystem.Wallet.Change(player, payment);
                GameLog.Money($"server", $"player({characterData.UUID})", payment, $"electricianCheck");
                BattlePass.Repository.UpdateReward(player, 22);
                BattlePass.Repository.UpdateReward(player, 156);

                NAPI.Entity.SetEntityPosition(player, Checkpoints[shapeId].Position + new Vector3(0, 0, 1.2));
                NAPI.Entity.SetEntityRotation(player, new Vector3(0, 0, Checkpoints[shapeId].Heading));
                Main.OnAntiAnim(player);
                Trigger.PlayAnimation(player, "amb@prop_human_movie_studio_light@base", "base", 39);
                // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "electric");
                sessionData.WorkData.WorkCheck = -1;

                if (characterData.JobSkills.ContainsKey(0))
                {
                    if (characterData.JobSkills[0] < 15000)
                        characterData.JobSkills[0] += 1;
                }
                else characterData.JobSkills.Add(0, 1);
                
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

                NAPI.Task.Run(() =>
                {
                    try
                    {
                        if (!player.IsCharacterData()) return;

                        sessionData = player.GetSessionData();
                        if (sessionData == null) return;

                        Trigger.StopAnimation(player);
                        Main.OffAntiAnim(player);

                        int nextCheck = WorkManager.rnd.Next(0, Checkpoints.Count - 1);
                        while (nextCheck == shapeId) nextCheck = WorkManager.rnd.Next(0, Checkpoints.Count - 1);

                        sessionData.WorkData.WorkCheck = nextCheck;

                        Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checkpoints[nextCheck].Position, 1, 0, 255, 0, 0);
                        Trigger.ClientEvent(player, "createWorkBlip", Checkpoints[nextCheck].Position);
          
                    }
                    catch (Exception e)
                    {
                        Log.Write($"PlayerEnterCheckpoint Task Exception: {e.ToString()}");
                    }
                }, 4000);
            }
            catch (Exception e)
            {
                Log.Write($"PlayerEnterCheckpoint Exception: {e.ToString()}");
            }
        }

        internal class Checkpoint
        {
            public Vector3 Position { get; }
            public double Heading { get; }

            public Checkpoint(Vector3 pos, double rot)
            {
                Position = pos;
                Heading = rot;
            }
        }
    }
}
