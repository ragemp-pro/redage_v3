using GTANetworkAPI;
using NeptuneEvo.Handles;
using System;
using System.Linq;
using System.Collections.Generic;
using Localization;
using NeptuneEvo.Core;
using Redage.SDK;
using NeptuneEvo.GUI;
using NeptuneEvo.Chars;
using NeptuneEvo.Functions;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Jobs.Models;
using NeptuneEvo.Players.Popup.List.Models;
using NeptuneEvo.Quests;
using Newtonsoft.Json;

namespace NeptuneEvo.Jobs
{
    class WorkManager : Script
    {
        private static readonly nLog Log = new nLog("Jobs.WorkManager");
        public static Random rnd = new Random();

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                //CustomColShape.CreateCylinderColShape(Points[0], 1, 2, 0, ColShapeEnums.JobSelect); // job placement
                //(ExtTextLabel) NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Работы"), new Vector3(Points[0].X, Points[0].Y, Points[0].Z + 0.5), 10F, 0.3F, 0, new Color(255, 255, 255));
                //(ExtMarker) NAPI.Marker.CreateMarker(1, Points[0] - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220));

                // blips

                Main.CreateBlip(new Main.BlipData(354, LangFunc.GetText(LangType.Ru, DataName.Electrostanciya), new Vector3(724.9625, 133.9959, 79.83643), 70, true, 1.2f));

            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }
        public static List<string> JobStats = new List<string>
        {
            LangFunc.GetText(LangType.Ru, DataName.Electrician),
            LangFunc.GetText(LangType.Ru, DataName.Postman),
            LangFunc.GetText(LangType.Ru, DataName.Taximan),
            LangFunc.GetText(LangType.Ru, DataName.Busdriver),
            LangFunc.GetText(LangType.Ru, DataName.Gazonman),
            LangFunc.GetText(LangType.Ru, DataName.Dalnoboy),
            LangFunc.GetText(LangType.Ru, DataName.Inkassman),
            LangFunc.GetText(LangType.Ru, DataName.Mechanicman),
        };
        public static SortedList<int, Vector3> Points = new SortedList<int, Vector3>
        {
            {0, new Vector3(436.5074, -627.4617, 28.70753) },  // Employment center
            {1, new Vector3(724.9625, 133.9959, 79.83643) },  // Electrician job
            {2, new Vector3(436.5074, -627.4617, 28.707539) },  // Postal job
            {3, new Vector3(436.5074, -627.4617, 28.707539) },      // Taxi job
            {4, new Vector3(436.5074, -627.4617, 28.707539) }, // Bus driver job
            {5, new Vector3(-1330.482, 42.12986, 53.48915) },  // Lawnmower job
            {6, new Vector3(436.5074, -627.4617, 28.707539) },  // Trucker job
            {7, new Vector3(436.5074, -627.4617, 28.707539) },  // Collector job
            {8, new Vector3(436.5074, -627.4617, 28.707539) },  // AutoMechanic job
        };
        private static SortedList<int, string> JobList = new SortedList<int, string>
        {
            {1, LangFunc.GetText(LangType.Ru, DataName.Electriciany) },
            {2, LangFunc.GetText(LangType.Ru, DataName.Postmany) },
            {3, LangFunc.GetText(LangType.Ru, DataName.Taximany) },
            {4, LangFunc.GetText(LangType.Ru, DataName.Busdrivery) },
            {5, LangFunc.GetText(LangType.Ru, DataName.Gazonmany) },
            {6, LangFunc.GetText(LangType.Ru, DataName.Dalnoboyy) },
            {7, LangFunc.GetText(LangType.Ru, DataName.Inkassmany) },
            {8, LangFunc.GetText(LangType.Ru, DataName.Mechanicmany) },
        };
        
        public static void Layoff(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (sessionData.WorkData.OnWork == true)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustEndWorkDay), 3000);
                    return;
                }
                if (characterData.WorkID != 0)
                {
                    if (characterData.WorkID == (int)JobsId.CashCollector)
                    {
                        Trigger.ClientEvent(player, "deleteCheckpoint", 16, 0);
                        Trigger.ClientEvent(player, "deleteWorkBlip");
                    }

                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouLeaveJob), 3000);
                    BattlePass.Repository.UpdateReward(player, 10);
                    UpdateData.Work(player, 0);
                }
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouNotJober), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"Layoff Exception: {e.ToString()}");
            }
        }
        /// <summary>
        /// Минимальный уровень на работах
        /// </summary>
        public static SortedList<int, int> JobsMinLvl = new SortedList<int, int>()
        {
            { (int) JobsId.Electrician, 0 },
            { (int) JobsId.Postman, 2 },
            { (int) JobsId.Taxi, 2 },
            { (int) JobsId.Bus, 1 },
            { (int) JobsId.Lawnmower, 0 },
            { (int) JobsId.Trucker, 4 },
            { (int) JobsId.CashCollector, 3 },
            { (int) JobsId.CarMechanic, 2 },
        };
        public static void JobJoin(ExtPlayer player, int job)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                //if (memberFractionData.Id != (int)Fractions.Models.Fractions.None)
                // {
                //    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете устроиться на работу, потому что состоите в организации", 3000);
                //    return;
                // }
                if (characterData.WorkID != 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BeforeLeaveLastJob), 3000);
                    return;
                }
                if (sessionData.WorkData.OnWork == true)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustEndWorkDay), 3000);
                    return;
                }

                if (characterData.WorkID == job) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouAlreadyJob, JobList[job]), 3000);
                else
                {
                    if (characterData.LVL < JobsMinLvl[job])
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NeedJobsLvl, JobsMinLvl[job]), 3000);
                        return;
                    }
                    if ((job == (int)JobsId.Postman || job == (int)JobsId.Taxi || job == (int)JobsId.CarMechanic) && !characterData.Licenses[1])
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.JobLicB), 10000);
                        return;
                    }
                    if ((job == (int)JobsId.Bus || job == (int)JobsId.Trucker || job == (int)JobsId.CashCollector) && !characterData.Licenses[2])
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.JobLicC), 10000);
                        return;
                    }
                    UpdateData.Work(player, job);

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouStartedJob, JobList[job]), 8000);
                    BattlePass.Repository.UpdateReward(player, 9);
                    
                    Trigger.ClientEvent(player, "createWaypoint", Points[job].X, Points[job].Y);
                }
            }
            catch (Exception e)
            {
                Log.Write($"JobJoin Exception: {e.ToString()}");
            }
        }

        [Interaction(ColShapeEnums.JobSelect)]
        public static void InJobSelect(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                Trigger.ClientEvent(player, "showJobMenu", JsonConvert.SerializeObject(JobsMinLvl));
            }
            catch (Exception e)
            {
                Log.Write($"JobMenu_onEntityEnterColShape Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("jobjoin")]
        public static void callback_jobsSelecting(ExtPlayer player, int act)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                switch (act)
                {
                    case -1:
                        Layoff(player);
                        return;
                    default:
                        JobJoin(player, act);
                        return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"callback_jobsSelecting Exception: {e.ToString()}");
            }
        }
    }
}