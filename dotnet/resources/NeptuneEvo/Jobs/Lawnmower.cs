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
using NeptuneEvo.VehicleData.LocalData;

namespace NeptuneEvo.Jobs
{
    class Lawnmower : Script
    {
        private static readonly nLog Log = new nLog("Jobs.Lawnmower");

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStartHandler()
        {
            try
            {
                for (int i = 0; i < MowerWays.Count; i++)
                {
                    for (int d = 0; d < MowerWays[i].Count; d++)
                    {
                        CustomColShape.CreateCylinderColShape(MowerWays[i][d], 4F, 3, 0, ColShapeEnums.MowerWays, i, d);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStartHandler Exception: {e.ToString()}");
            }
        }

        private static List<List<Vector3>> MowerWays = new List<List<Vector3>>()
        {
            new List<Vector3>()
            {
                new Vector3(-1311.801, 47.19352, 53.15438),      // 2
                new Vector3(-1297.115, 44.82426, 51.8745),      // 3
                new Vector3(-1289.672, 41.50377, 51.41702),      // 4
                new Vector3(-1281.616, 33.57293, 50.01316),      // 5
                new Vector3(-1271.753, 25.13823, 48.37608),      // 6
                new Vector3(-1264.546, 20.33339, 48.22725),      // 7
                new Vector3(-1251.953, 16.60849, 48.29678),      // 8
                new Vector3(-1238.87, 6.834239, 47.69267),      // 9
                new Vector3(-1220.066, -0.2359556, 47.76429),      // 10
                new Vector3(-1205.803, -10.85785, 47.54021),      // 11
                new Vector3(-1191.213, -26.15599, 46.0427),      // 12
                new Vector3(-1169.42, -32.93403, 45.1136),      // 13
                new Vector3(-1151.52, -47.59819, 44.79146),      // 14
                new Vector3(-1134.367, -57.45755, 44.24867),      // 15
                new Vector3(-1113.337, -65.23543, 44.08297),      // 16
                new Vector3(-1089.529, -65.04275, 43.71874),      // 17
                new Vector3(-1068.829, -71.01239, 44.14802),      // 18
                new Vector3(-1042.585, -66.32011, 44.12957),      // 19
                new Vector3(-1028.766, -62.31656, 44.31059),      // 20
                new Vector3(-1010.945, -54.24666, 42.86016),      // 21
                new Vector3(-1004.091, -37.87244, 45.26571),      // 22
                new Vector3(-990.7962, -28.25723, 45.11967),      // 23
                new Vector3(-987.4324, -11.62305, 46.72776),      // 24
                new Vector3(-996.9695, 6.893913, 48.91844),      // 25
                new Vector3(-1007.902, 24.14594, 50.23178),      // 26
                new Vector3(-1020.931, 45.43934, 50.92136),      // 27
                new Vector3(-1029.439, 66.24312, 51.80055),      // 28
                new Vector3(-1051.099, 96.99185, 53.38039),      // 29
                new Vector3(-1058.959, 108.3395, 54.89647),      // 30
                new Vector3(-1067.165, 125.1816, 57.015),      // 31
                new Vector3(-1076.025, 142.120, 59.05041),      // 32
                new Vector3(-1090.573, 163.7347, 61.53791),      // 33
                new Vector3(-1110.753, 173.7621, 62.98595),      // 34
                new Vector3(-1125.826, 181.9235, 63.85329),      // 35
                new Vector3(-1168.176, 178.2816, 64.00827),      // 36
                new Vector3(-1199.407, 170.2452, 63.66505),      // 37
                new Vector3(-1223.774, 171.792, 62.43406),      // 38
                new Vector3(-1258.112, 168.5394, 59.59033),      // 39
                new Vector3(-1269.38, 151.660, 58.65258),      // 40
                new Vector3(-1291.592, 144.490, 58.38726),      // 41
                new Vector3(-1293.719, 115.4129, 56.68567),      // 42
                new Vector3(-1266.897, 102.3202, 56.23266),      // 43
                new Vector3(-1247.905, 92.40211, 55.77993),      // 44
                new Vector3(-1220.986, 94.42358, 57.08755),      // 45
                new Vector3(-1198.35, 100.4512, 58.12225),      // 46
                new Vector3(-1171.731, 106.132, 58.92729),      // 47
                new Vector3(-1154.295, 113.0125, 59.21473),      // 48
                new Vector3(-1134.693, 103.8594, 58.01085),      // 49
                new Vector3(-1115.926, 84.72909, 55.9537),      // 50
                new Vector3(-1105.542, 59.41261, 53.21173),      // 51
                new Vector3(-1100.384, 37.19199, 51.35212),      // 52
                new Vector3(-1094.208, 24.42362, 50.99221),      // 53
                new Vector3(-1080.965, 13.15148, 50.65717),      // 54
                new Vector3(-1092.702, 1.099258, 50.87796),      // 55
                new Vector3(-1109.086, -11.22914, 50.53533),      // 56
                new Vector3(-1120.472, -24.12328, 48.84153),      // 57
                new Vector3(-1129.953, -46.12931, 45.39178),      // 58
                new Vector3(-1141.217, -66.69366, 43.90619),      // 59
                new Vector3(-1156.251, -66.09172, 44.83364),      // 60
                new Vector3(-1164.265, -75.3207, 45.51969),      // 61
                new Vector3(-1173.412, -81.57317, 44.9781),      // 62
                new Vector3(-1185.708, -73.71046, 44.91101),      // 63
                new Vector3(-1200.024, -62.89313, 44.89631),      // 64
                new Vector3(-1222.646, -47.62072, 45.85396),      // 65
                new Vector3(-1245.577, -36.77324, 46.27745),      // 66
                new Vector3(-1261.259, -23.22089, 47.47988),      // 67
                new Vector3(-1266.621, -8.019106, 48.37564),      // 68
                new Vector3(-1275.466, -2.052856, 48.95642),      // 69
                new Vector3(-1287.256, -10.6012, 49.94118),      // 70
                new Vector3(-1298.156, -14.02906, 49.41903),      
                new Vector3(-1309.017, 4.108258, 50.99983),
                new Vector3(-1318.991, 25.81007, 53.54743),
                new Vector3(-1297.523, 51.1332, 51.67038),      // 74
                new Vector3(-1269.485, 46.31668, 49.99769),      // 75
                new Vector3(-1248.031, 37.50771, 48.87788),      // 76
                new Vector3(-1223.725, 26.63491, 47.60126),      // 77
                new Vector3(-1200.899, 8.332254, 47.56612),      // 78
                new Vector3(-1184.26, -7.549067, 47.28683),      // 79
                new Vector3(-1156.366, -4.323437, 47.77936),      // 80
                new Vector3(-1156.365, 11.39268, 49.60625),      // 81
                new Vector3(-1153.883, 31.17125, 51.20083),      // 82
                new Vector3(-1169.151, 45.62376, 52.44979),      // 83
                new Vector3(-1196.515, 61.6429, 54.02454),      // 84
                new Vector3(-1220.711, 72.92409, 53.81203),      // 85
                new Vector3(-1244.826, 73.44586, 52.69176),      // 86
                new Vector3(-1270.854, 73.22437, 53.18766),      // 87
                new Vector3(-1285.552, 74.05328, 54.72931),      // 88
                new Vector3(-1298.148, 72.72589, 54.62475),      // 89
                new Vector3(-1321.219, 69.28333, 53.55971),      // 90
            },
        };
        
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
                ClothesComponents.SetSpecialClothes(player, 0,  94, 9);
                ClothesComponents.SetSpecialClothes(player, 11, 82, 4);
                ClothesComponents.SetSpecialClothes(player, 4, 27, 10);
                ClothesComponents.SetSpecialClothes(player, 6, 1, 11);
            }
            else
            {
                ClothesComponents.SetSpecialClothes(player, 0,  93, 9);
                ClothesComponents.SetSpecialClothes(player, 11, 14, 9);
                ClothesComponents.SetSpecialClothes(player, 4, 16, 2);
                ClothesComponents.SetSpecialClothes(player, 6, 1, 3);
            }
            Chars.Repository.LoadAccessories(player);
            
            sessionData.WorkData.WorkWay = 0;
            sessionData.WorkData.WorkCheck = 0;
            sessionData.WorkData.OnWork = true;
            var way = MowerWays[0];
            Trigger.ClientEvent(player, "createCheckpoint", 4, 1, way[0] - new Vector3(0, 0, 1.12), 2, 0, 255, 0, 0, way[1] - new Vector3(0, 0, 1.12));
            Trigger.ClientEvent(player, "createWaypoint", way[0].X, way[0].Y);
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.StartLawnmower), 3000);
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
                Trigger.ClientEvent(player, "deleteCheckpoint", 4, 0);
                return true;
            }
            return false;
        }
        [Interaction(ColShapeEnums.MowerWays, In: true)]
        public void InMowerWays(ExtPlayer player, int index, int listId)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!NAPI.Player.IsPlayerInAnyVehicle(player)) return;
                var vehicle = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if (vehicleLocalData.WorkId != JobsId.Lawnmower || characterData.WorkID != (int)JobsId.Lawnmower || !sessionData.WorkData.OnWork || sessionData.WorkData.WorkWay != index || sessionData.WorkData.WorkCheck != listId) return;

                    int way = sessionData.WorkData.WorkWay;
                    int check = sessionData.WorkData.WorkCheck;
                    if (MowerWays[way][check].DistanceTo(player.Position) > 6) return;

                    int payment = Convert.ToInt32(Main.LawnmowerPayment * Group.GroupPayAdd[accountData.VipLvl] * Main.ServerSettings.MoneyMultiplier);

                    (byte, float) jobLevelInfo = characterData.JobSkills.ContainsKey(1) ? Main.GetPlayerJobLevelBonus(1, characterData.JobSkills[1]) : (0, 1);
                    if (jobLevelInfo.Item1 >= 1) payment = Convert.ToInt32(payment * jobLevelInfo.Item2);

                    MoneySystem.Wallet.Change(player, payment);
                    GameLog.Money($"server", $"player({characterData.UUID})", payment, $"lawnCheck");
                    BattlePass.Repository.UpdateReward(player, 21);
                    BattlePass.Repository.UpdateReward(player, 155);

                    if (characterData.JobSkills.ContainsKey(1))
                    {
                        if (characterData.JobSkills[1] < 40000)
                            characterData.JobSkills[1] += 1;
                    }
                    else characterData.JobSkills.Add(1, 1);
                    
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

                    if (check + 1 != MowerWays[way].Count)
                    {
                        Vector3 direction = (check + 2 != MowerWays[way].Count) ? MowerWays[way][check + 2] : MowerWays[way][0] - new Vector3(0, 0, 1.12);
                        Trigger.ClientEvent(player, "createCheckpoint", 4, 1, MowerWays[way][check + 1] - new Vector3(0, 0, 1.12), 2, 0, 255, 0, 0, direction);
                        Trigger.ClientEvent(player, "createWaypoint", MowerWays[way][check + 1].X, MowerWays[way][check + 1].Y);
                        sessionData.WorkData.WorkCheck = check + 1;
                    }
                    else
                    {
                        int next_way = 0;
                        Trigger.ClientEvent(player, "createCheckpoint", 4, 1, MowerWays[next_way][0] - new Vector3(0, 0, 1.12), 2, 0, 255, 0, 0, MowerWays[next_way][1] - new Vector3(0, 0, 1.12));
                        Trigger.ClientEvent(player, "createWaypoint", MowerWays[next_way][0].X, MowerWays[next_way][0].Y);
                        sessionData.WorkData.WorkCheck = 0;
                        sessionData.WorkData.WorkWay = next_way;

                        int bonusPay = 0;
                        if (jobLevelInfo.Item1 == 3 || jobLevelInfo.Item1 == 4) bonusPay = 50;
                        else if (jobLevelInfo.Item1 >= 5) bonusPay = 75;

                        if (bonusPay > 0)
                        {
                            MoneySystem.Wallet.Change(player, bonusPay);
                            GameLog.Money($"server", $"player({characterData.UUID})", payment, $"lawnBonusWay");
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BonusPay, bonusPay), 3000);
                            
                            if (qMain.GetQuestsLine(player, Zdobich.QuestName) == (int)zdobich_quests.Stage11)
                            {
                                sessionData.WorkData.PointsCount += bonusPay;
                                if (sessionData.WorkData.PointsCount < qMain.GetQuestsData(player, Zdobich.QuestName, (int) zdobich_quests.Stage11))
                                    sessionData.WorkData.PointsCount = qMain.GetQuestsData(player, Zdobich.QuestName, (int) zdobich_quests.Stage11) + bonusPay;
                    
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
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"mowerCheckpointEnterWay Exception: {e.ToString()}");
            }
        }
    }
}
