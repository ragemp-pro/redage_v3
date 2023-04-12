using System;
using System.Collections.Generic;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Handles;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Functions;
using NeptuneEvo.Table.Tasks.Models;
using NeptuneEvo.Table.Tasks.Player;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;
using Redage.SDK;

namespace NeptuneEvo.Fractions
{
    class CarDelivery : Script
    {
        private static readonly nLog Log = new nLog("Fractions.CrimeMissions");

        private static uint[] GangsVehiclesHashes = new uint[15] {
            (uint)VehicleHash.Emperor,
            (uint)VehicleHash.Ingot,
            (uint)VehicleHash.Asterope,
            (uint)VehicleHash.Blista2,
            (uint)VehicleHash.Seminole,
            (uint)VehicleHash.Fugitive,
            (uint)VehicleHash.Tailgater,
            (uint)VehicleHash.Cog55,
            (uint)VehicleHash.Serrano,
            (uint)VehicleHash.Dominator,
            (uint)VehicleHash.Baller2,
            (uint)VehicleHash.Cavalcade2,
            (uint)VehicleHash.Dubsta2,
            (uint)VehicleHash.Schafter2,
            (uint)VehicleHash.Buffalo2 
        };

        private static Color[] GangsVehiclesColors = new Color[15] { new Color(0, 0, 0), new Color(0, 0, 0), new Color(250, 250, 250), new Color(200, 0, 0), new Color(250, 250, 250), new Color(250, 250, 250), new Color(0, 0, 200), new Color(80, 80, 80), new Color(0, 0, 0), new Color(230, 90, 0), new Color(250, 250, 250), new Color(0, 0, 250), new Color(80, 80, 80), new Color(250, 250, 250), new Color(0, 200, 0) };

        private static uint[] MafiaVehiclesHashes = new uint[3] {
            (uint) VehicleHash.Pounder,
            (uint) VehicleHash.Boxville3,
            (uint) VehicleHash.Mule 
        };

        private static uint[] BikerVehiclesHashes = new uint[3] {
            (uint) VehicleHash.Pounder,
            (uint) VehicleHash.Boxville3,
            (uint) VehicleHash.Mule 
        };

        private static Dictionary<int, DateTime> NextDelivery = new Dictionary<int, DateTime>()
        {
            { (int) Models.Fractions.FAMILY, DateTime.Now },
            { (int) Models.Fractions.BALLAS, DateTime.Now },
            { (int) Models.Fractions.VAGOS, DateTime.Now },
            { (int) Models.Fractions.MARABUNTA, DateTime.Now },
            { (int) Models.Fractions.BLOOD, DateTime.Now },
            { (int) Models.Fractions.LCN, DateTime.Now },
            { (int) Models.Fractions.RUSSIAN, DateTime.Now },
            { (int) Models.Fractions.YAKUZA, DateTime.Now },
            { (int) Models.Fractions.ARMENIAN, DateTime.Now },
            { (int) Models.Fractions.THELOST, DateTime.Now },
        };

        private static Vector3 GangStartDelivery = new Vector3(480.9385, -1302.576, 28.12353);

        public static Vector3[] GangSpawnAutos = new Vector3[16]
        {
            new Vector3(814.4807, -747.8201, 26.8163),
            new Vector3(711.6686, -895.5933, 23.6463),
            new Vector3(731.1492, -1291.567, 26.3728),
            new Vector3(-451.0398, -1691.584, 19.04522),
            new Vector3(-559.7632, -1802.013, 22.71108),
            new Vector3(-702.673, -1138.601, 10.7008),
            new Vector3(331.2978, -996.9423, 29.30805),
            new Vector3(295.5413, -1203.21, 29.24488),
            new Vector3(198.8274, -1244.367, 29.34067),
            new Vector3(-478.4023, -757.7425, 35.46944),
            new Vector3(-313.0926, -771.1686, 34.05299),
            new Vector3(-151.8391, -1030.047, 27.3617),
            new Vector3(1000.476, -1532.942, 29.95521),
            new Vector3(969.8792, -1647.465, 29.93223),
            new Vector3(987.3099, -1832.049, 31.16065),
            new Vector3(1265.665, -2563.195, 42.91005),
        };

        public static Vector3[] GangSpawnAutosRot = new Vector3[16]
        {
            new Vector3(-0.1028762, -6.507742E-05, 185.9321),
            new Vector3(-0.03887018, 4.864606, 267.9159),
            new Vector3(0.001559988, 0.06757163, 88.61029),
            new Vector3(-0.4639261, 0.1743369, 164.3215),
            new Vector3(0.1674757, -0.3905835, 331.478),
            new Vector3(0.03815498, 0.0097493, 43.03497),
            new Vector3(-2.474918, 0.1015807, 181.4881),
            new Vector3(0.6146746, 1.624031, 177.8676),
            new Vector3(0.197663, 0.6434172, 85.07928),
            new Vector3(0.02163176, 0.1697985, 88.46164),
            new Vector3(-0.1381722, 0.07952184, 160.3685),
            new Vector3(-0.01273972, 0.0547189, 165.6741),
            new Vector3(0.002720123, -6.338181, 86.91882),
            new Vector3(-5.582367, -0.02341981, 179.5117),
            new Vector3(-0.353688, -1.634855, 354.2626),
            new Vector3(1.573582, 3.525663, 284.0478),
        };

        public static Dictionary<int, Vector3> MafiaStartDelivery = new Dictionary<int, Vector3>()
        {
            { (int) Models.Fractions.LCN, new Vector3(1463.797, 1128.923, 114.3969) },
            { (int) Models.Fractions.RUSSIAN, new Vector3(-128.5574, 994.9902, 235.8243) },
            // { (int) Models.Fractions.YAKUZA, new Vector3(-1454.1617, -56.422596, 52.7) },
            { (int) Models.Fractions.YAKUZA, new Vector3(-1450.6471, -54.422596, 52.55) },
            { (int) Models.Fractions.ARMENIAN, new Vector3(-1795.539, 399.2474, 112.8691) },
        };
        private static Dictionary<int, Vector3> MafiaStartDeliveryRot = new Dictionary<int, Vector3>()
        {
            { (int) Models.Fractions.LCN, new Vector3(1.19556, 0.2941337, 91.12183) },
            { (int) Models.Fractions.RUSSIAN, new Vector3(-0.02024388, 0.4382547, 198.8489) },
            { (int) Models.Fractions.YAKUZA, new Vector3(0.0, 0.0, 177.61) },
            { (int) Models.Fractions.ARMENIAN, new Vector3(-0.3686997, -0.2600957, 286.0435) },
        };

        //public static Dictionary<int, Vector3> BikerStartDelivery = new Dictionary<int, Vector3>()
        //{
        //    { (int) Models.Fractions.THELOST, new Vector3(980.2203, -118.9507, 74.07727) },
        //};
        //private static Dictionary<int, Vector3> BikerStartDeliveryRot = new Dictionary<int, Vector3>()
        //{
        //    { (int) Models.Fractions.THELOST, new Vector3(-1.311226, -2.088267, 148.5165) },
        //};

        public static Vector3[] GangEndDelivery = new Vector3[10]
        {
            new Vector3(-829.0149, -1263.8926, 4.7003767),
            new Vector3(-1242.06, -258.6279, 38.754628),
            new Vector3(-452.39337, 289.41064, 82.85052),
            new Vector3(-71.827614, 211.1988, 96.21683),
            new Vector3(953.3633, -106.82494, 74.05306),
            new Vector3(1100.892, -786.5164, 57.960098),
            new Vector3(1263.9592, -2565.2368, 42.50573),
            new Vector3(323.00296, 267.766, 104.01977),
            new Vector3(-1380.3353, -652.29315, 28.483521),
            new Vector3(-620.63153, -1130.3458, 21.878213),
        };

        public static Vector3[] MafiaEndDelivery = new Vector3[12]
        {
            new Vector3(-1834.286, 4692.507, 0.6031599),
            new Vector3(-2658.842, 2533.021, 1.898514),
            new Vector3(3304.526, 5401.26, 13.62714),
            new Vector3(2834.238, -724.9515, 0.6343195),
            new Vector3(1413.24, 3647.831, 33.24832),
            new Vector3(-1132.995, 2697.909, 18.37682),
            new Vector3(828.0319, -3206.071, 4.78082),
            new Vector3(-337.131, -2462.448, 4.880634),
            new Vector3(1642.278, 4837.864, 40.90576),
            new Vector3(616.3151, 2799.022, 40.80243),
            new Vector3(-1237.381, -1403.953, 3.844509),
            new Vector3(1378.693, -2077.05, 50.87856),
        };
        private static Vector3 PoliceEndDelivery = new Vector3(479.2215, -1024.153, 26.91038);

        [ServerEvent(Event.ResourceStart)]
        public void Event_ResourceStart()
        {
            try
            {

                CustomColShape.CreateCylinderColShape(GangStartDelivery, 2, 3, NAPI.GlobalDimension, ColShapeEnums.CrimeGang);

                NAPI.TextLabel.CreateTextLabel("~w~Carter Scott", GangStartDelivery + new Vector3(0, 0, 2.5), 5f, 0.4f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension);

                #region MafiaStartDelivery
                int i = 0;
                foreach (KeyValuePair<int, Vector3> pos in MafiaStartDelivery)
                {
                    CustomColShape.CreateCylinderColShape(pos.Value, 3, 3, NAPI.GlobalDimension, ColShapeEnums.CrimeMafia, pos.Key);
                    NAPI.Marker.CreateMarker(1, pos.Value - new Vector3(0, 0, 2.7), new Vector3(), new Vector3(), 3, new Color(255, 0, 0, 220), false, NAPI.GlobalDimension);
                    NAPI.TextLabel.CreateTextLabel("~w~Взять миссию по доставке", pos.Value, 5f, 0.4f, 0, new Color(255, 255, 255));
                }
                #endregion

                #region BikerStartDelivery
                //i = 0;
                //foreach (KeyValuePair<int, Vector3> pos in BikerStartDelivery)
                //{
                //    CustomColShape.CreateCylinderColShape(pos.Value, 3, 3, NAPI.GlobalDimension, ColShapeEnums.CrimeBiker, pos.Key);
                //
                //    NAPI.Marker.CreateMarker(1, pos.Value - new Vector3(0, 0, 2.7), new Vector3(), new Vector3(), 3, new Color(255, 0, 0, 220), false, NAPI.GlobalDimension);
                //    NAPI.TextLabel.CreateTextLabel("~w~Взять миссию по доставке", pos.Value, 5f, 0.4f, 0, new Color(255, 255, 255));
                //}
                #endregion

                #region PoliceDropDelivery
                CustomColShape.CreateCylinderColShape(PoliceEndDelivery, 3, 4, NAPI.GlobalDimension, ColShapeEnums.PoliceDropDelivery);
                NAPI.Marker.CreateMarker(1, PoliceEndDelivery - new Vector3(0, 0, 2.5), new Vector3(), new Vector3(), 3, new Color(255, 0, 0, 220), false, NAPI.GlobalDimension);
                #endregion

                #region GangsDropDelivery
                i = 0;
                foreach (Vector3 pos in GangEndDelivery)
                {
                    CustomColShape.CreateCylinderColShape(pos, 5, 3, NAPI.GlobalDimension, ColShapeEnums.GangEndDelivery, i);
     
                    NAPI.Marker.CreateMarker(1, pos - new Vector3(0, 0, 3.5), new Vector3(), new Vector3(), 4, new Color(255, 0, 0, 220), false, NAPI.GlobalDimension);
                    i++;
                }
                #endregion

                #region MafiaDropDelivery
                i = 0;
                foreach (Vector3 pos in MafiaEndDelivery)
                {
                    CustomColShape.CreateCylinderColShape(pos, 5, 3, NAPI.GlobalDimension, ColShapeEnums.MafiaEndDelivery, i);

                    NAPI.Marker.CreateMarker(1, pos - new Vector3(0, 0, 3.5), new Vector3(), new Vector3(), 4, new Color(255, 0, 0, 220), false, NAPI.GlobalDimension);
                    i++;
                }
                #endregion
            }
            catch (Exception e)
            {
                Log.Write($"Event_ResourceStart Exception: {e.ToString()}");
            }
        }
        [Interaction(ColShapeEnums.PoliceDropDelivery, In: true)]
        public static void InPoliceDropDelivery(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                else if (!player.IsInVehicle) return;
                var vehicleDelivery = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = vehicleDelivery.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if (vehicleLocalData.Access != VehicleAccess.DeliveryGang && vehicleLocalData.Access != VehicleAccess.DeliveryMafia && vehicleLocalData.Access != VehicleAccess.DeliveryBike) return;
                    ExtPlayer target = vehicleLocalData.DeliveryData.WhosVeh;
                    var targetSessionData = target.GetSessionData();
                    if (targetSessionData == null) return;
                    VehicleStreaming.DeleteVehicle(vehicleDelivery);
                    targetSessionData.DeliveryData.Vehicle = null;
                    MoneySystem.Wallet.Change(player, 25);
                    GameLog.Money($"server", $"player({characterData.UUID})", 25, $"arrestCar");
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы арестовали машину", 3000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"InPoliceDropDelivery Exception: {e.ToString()}");
            }
        }


        [Interaction(ColShapeEnums.GangEndDelivery, In: true)]
        public static void InGangEndDelivery(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                var fractionData = player.GetFractionData();
                if (fractionData == null)
                    return;
                if (!player.IsInVehicle) return;
                var vehicleDelivery = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = vehicleDelivery.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if (vehicleLocalData.Access != VehicleAccess.DeliveryGang) return;
                    if (vehicleLocalData.DeliveryData.End != index) return;
                    if (DateTime.Now < vehicleLocalData.DeliveryData.DataEnd)
                    {
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Попробуйте чуть позже", 3000);
                        return;
                    }
                    VehicleStreaming.DeleteVehicle(vehicleDelivery);
                    sessionData.DeliveryData.Point = -1;
                    sessionData.DeliveryData.Vehicle = null;
                    int halfof = Convert.ToInt32(Main.GangCarDelivery / 2);
                    MoneySystem.Wallet.Change(player, halfof);
                    fractionData.Money += halfof;
                    GameLog.Money($"server", $"frac({fractionData.Id})", halfof, "dropCar");
                    Trigger.SendChatMessage(player, "Сдача машины: !{#00FF00}" + halfof + "$ ~w~были отправлены в общак банды и еще столько же тебе в карман. (" + $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}" + ")");
                    player.AddTableScore(TableTaskId.Item27);
                }
            }
            catch (Exception e)
            {
                Log.Write($"InGangEndDelivery Exception: {e.ToString()}");
            }
        }
        [Interaction(ColShapeEnums.MafiaEndDelivery, In: true)]
        public static void InMafiaEndDelivery(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                var fractionData = player.GetFractionData();
                if (fractionData == null)
                    return;
                
                if (!player.IsInVehicle) return;
                var vehicleDelivery = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = vehicleDelivery.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if (vehicleLocalData.Access != VehicleAccess.DeliveryMafia && vehicleLocalData.Access != VehicleAccess.DeliveryBike) return;
                    if (vehicleLocalData.DeliveryData.End != index) return;
                    if (DateTime.Now < vehicleLocalData.DeliveryData.DataEnd)
                    {
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Попробуйте чуть позже", 3000);
                        return;
                    }
                    VehicleStreaming.DeleteVehicle(vehicleDelivery);
                    sessionData.DeliveryData.Point = -1;
                    sessionData.DeliveryData.Vehicle = null;
                    int halfof = Convert.ToInt32(Main.MafiaCarDelivery / 2);
                    MoneySystem.Wallet.Change(player, halfof);
                    fractionData.Money += halfof;
                    GameLog.Money($"server", $"frac({fractionData.Id})", halfof, "dropCar");
                    Trigger.SendChatMessage(player, "Сдача фургона: !{#00FF00}" + halfof + "$ ~w~были отправлены в общак группировки и еще столько же тебе в карман. (" + $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()})");
                    player.AddTableScore(TableTaskId.Item27);
                }
            }
            catch (Exception e)
            {
                Log.Write($"AddNewItemWarehouse Exception: {e.ToString()}");
            }
        }

        [ServerEvent(Event.PlayerEnterVehicle)]
        public void Event_PlayerEnterVehicle(ExtPlayer player, ExtVehicle vehicle, sbyte seat)
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
                    var fracId = player.GetFractionId();
                    switch (vehicleLocalData.Access)
                    {
                        case VehicleAccess.DeliveryGang:
                            if (Configs.IsFractionPolic(fracId))
                            {
                                Trigger.ClientEvent(player, "createWaypoint", PoliceEndDelivery.X, PoliceEndDelivery.Y);
                                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Отвезите машину в полицейский участок", 3000);
                            }
                            else if (fracId != vehicleLocalData.DeliveryData.Fraction)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoDostup), 3000);
                                VehicleManager.WarpPlayerOutOfVehicle(player);
                            }
                            else
                            {
                                int end = vehicleLocalData.DeliveryData.End;
                                vehicleLocalData.DeliveryData.JStage = true;
                                Trigger.ClientEvent(player, "createWaypoint", GangEndDelivery[end].X, GangEndDelivery[end].Y);
                                sessionData.DeliveryData.Point = end;
                                Trigger.SendChatMessage(player, "Если вдруг Вы сбили точку доставки, то напишите в чат /mypoint");
                                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Доставьте машину в точку, отмеченную на карте", 3000);
                            }
                            return;
                        case VehicleAccess.DeliveryMafia:
                        case VehicleAccess.DeliveryBike:
                            if (Configs.IsFractionPolic(fracId))
                            {
                                Trigger.ClientEvent(player, "createWaypoint", PoliceEndDelivery.X, PoliceEndDelivery.Y);
                                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Отвезите машину в полицейский участок", 3000);
                            }
                            else if (fracId != vehicleLocalData.DeliveryData.Fraction)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoDostup), 3000);
                                VehicleManager.WarpPlayerOutOfVehicle(player);
                            }
                            else
                            {
                                int end = vehicleLocalData.DeliveryData.End;
                                vehicleLocalData.DeliveryData.JStage = true;
                                Trigger.ClientEvent(player, "createWaypoint", MafiaEndDelivery[end].X, MafiaEndDelivery[end].Y);
                                sessionData.DeliveryData.Point = end;
                                Trigger.SendChatMessage(player, "Если вдруг Вы сбили точку доставки, то напишите в чат /mypoint");
                                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Доставьте машину в точку, отмеченную на карте", 3000);
                            }
                            return;
                        default:
                            // Not supposed to end up here. 
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"Event_PlayerEnterVehicle Exception: {e.ToString()}");
            }
        }
        
        public static void Event_PlayerDisconnected(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                var vehicleDelivery = sessionData.DeliveryData.Vehicle;
                if (vehicleDelivery != null)
                {
                    VehicleStreaming.DeleteVehicle(vehicleDelivery);
                    sessionData.DeliveryData.Vehicle = null;
                }
            }
            catch (Exception e)
            {
                Log.Write($"Event_PlayerDisconnected Exception: {e.ToString()}");
            }
        }
        [Interaction(ColShapeEnums.CrimeGang)]
        public static void OnCrimeGang(ExtPlayer player)
        {
            try
            {
                var fracId = player.GetFractionId();
                if (Manager.FractionTypes[fracId] != FractionsType.Gangs) return;
                Manager.OpenFractionSM(player, "gang");
                return;
            }
            catch (Exception e)
            {
                Log.Write($"OnCrimeGang Exception: {e.ToString()}");
            }
        }
        [Interaction(ColShapeEnums.CrimeMafia)]
        public static void OnCrimeMafia(ExtPlayer player, int id)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                if (player.IsInVehicle) return;
                var fracId = player.GetFractionId();
                if (fracId != id) return;
                sessionData.OnMafiaID = id;
                Manager.OpenFractionSM(player, "mafia");
                return;
            }
            catch (Exception e)
            {
                Log.Write($"OnCrimeMafia Exception: {e.ToString()}");
            }
        }
        [Interaction(ColShapeEnums.CrimeBiker)]
        public static void OnCrimeBiker(ExtPlayer player, int id)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (player.IsInVehicle) return;
                var fracId = player.GetFractionId();
                if (fracId != id) return;
                sessionData.OnBikerID = id;
                Manager.OpenFractionSM(player, "biker");
                return;
            }
            catch (Exception e)
            {
                Log.Write($"OnCrimeBiker Exception: {e.ToString()}");
            }
        }

        private static int LastGangSpawn = 0;
        public static void Event_gangMission(ExtPlayer player, int id)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (player.IsInVehicle || player.Position.DistanceTo(GangStartDelivery) > 5) return;
                int fracId = player.GetFractionId();
                switch (id)
                {
                    case 0:
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недоступно на данный момент", 3000);
                        return;
                    case 1:
                        if (DateTime.Now < NextDelivery[fracId])
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Попробуйте позже", 3000);
                            return;
                        }
                        if (sessionData.DeliveryData.Vehicle != null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны сначала доставить предыдущую машину", 3000);
                            return;
                        }
                        Random rnd = new Random();
                        int vehicleNum = rnd.Next(0, 15);
                        var vehicle = (ExtVehicle) VehicleStreaming.CreateVehicle(GangsVehiclesHashes[vehicleNum], GangSpawnAutos[LastGangSpawn], GangSpawnAutosRot[LastGangSpawn].Z, 0, 0, "xxxxx", acc: VehicleAccess.DeliveryGang, petrol: 100);
                        Trigger.ClientEvent(player, "createWaypoint", GangSpawnAutos[LastGangSpawn].X, GangSpawnAutos[LastGangSpawn].Y);
                        sessionData.DeliveryData.Point = LastGangSpawn;
                        LastGangSpawn = (LastGangSpawn + 1 == (int) Models.Fractions.THELOST) ? 0 : LastGangSpawn + 1;
                        vehicle.CustomPrimaryColor = GangsVehiclesColors[vehicleNum];
                        vehicle.CustomSecondaryColor = GangsVehiclesColors[vehicleNum];
                        int end = rnd.Next(0, 10);
                        var vehicleLocalData = vehicle.GetVehicleLocalData();
                        var data = vehicleLocalData.DeliveryData;
                        data.End = end;
                        data.Fraction = fracId;
                        data.DataEnd = DateTime.Now.AddSeconds(vehicle.Position.DistanceTo(GangEndDelivery[end]) / 100 * 2);
                        data.JStage = false;
                        data.WhosVeh = player;
                        VehicleStreaming.SetEngineState(vehicle, true);
                        sessionData.DeliveryData.Vehicle = vehicle;
                        NextDelivery[fracId] = DateTime.Now.AddMinutes(5);
                        Trigger.SendChatMessage(player, "Если вдруг Вы сбили координаты машины, то напишите в чат /mypoint");
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Вы получили координаты машины. Сядьте в неё и отвезите в место, которое отмечено в GPS автомобиля.", 3000);
                        player.AddTableScore(TableTaskId.Item27);
                        return;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"Event_gangMission Exception: {e.ToString()}");
            }
        }

        public static void Event_mafiaMission(ExtPlayer player, int id)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (player.IsInVehicle) return;
                int i = sessionData.OnMafiaID;
                if (i == -1 || player.Position.DistanceTo(MafiaStartDelivery[i]) > 6) return;
                int fracId = player.GetFractionId();
                if (!NextDelivery.ContainsKey(fracId) || DateTime.Now < NextDelivery[fracId])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Попробуйте позже", 3000);
                    return;
                }
                if (sessionData.DeliveryData.Vehicle != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны сначала доставить предыдущую машину", 3000);
                    return;
                }
                Random rnd = new Random();
                var vehicleHash = MafiaVehiclesHashes[0];
                string text = "";
                switch (id)
                {
                    case 0:
                        text = "оружием";
                        break;
                    case 1:
                        text = "деньгами";
                        vehicleHash = MafiaVehiclesHashes[1];
                        break;
                    case 2:
                        text = "трупами";
                        vehicleHash = MafiaVehiclesHashes[2];
                        break;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
                var vehicle = (ExtVehicle) VehicleStreaming.CreateVehicle(vehicleHash, MafiaStartDelivery[i] + new Vector3(0, 0, 1), MafiaStartDeliveryRot[i].Z, 0, 0, "xxxxx", acc: VehicleAccess.DeliveryMafia, petrol: 100);
                vehicle.CustomPrimaryColor = new Color(0, 0, 0);
                vehicle.CustomSecondaryColor = new Color(0, 0, 0);
                int end = rnd.Next(0, 12);
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                var data = vehicleLocalData.DeliveryData;
                data.End = end;
                data.Fraction = fracId;
                data.DataEnd = DateTime.Now.AddSeconds(vehicle.Position.DistanceTo(MafiaEndDelivery[end]) / 100 * 2);
                data.JStage = false;
                data.WhosVeh = player;
                sessionData.DeliveryData.Point = end;
                sessionData.DeliveryData.Vehicle = vehicle;
                VehicleStreaming.SetEngineState(vehicle, true);
                NextDelivery[fracId] = DateTime.Now.AddMinutes(5);
                Trigger.SendChatMessage(player, "Если вдруг Вы сбили координаты машины, то напишите в чат /mypoint");
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы получили фургон с {text} для транспортировки. Отвезите его в указанное на карте место", 3000);
                player.AddTableScore(TableTaskId.Item27);
                Trigger.ClientEvent(player, "createWaypoint", MafiaEndDelivery[end].X, MafiaEndDelivery[end].Y);
                player.SetIntoVehicle(vehicle, (int)VehicleSeat.Driver);
            }
            catch (Exception e)
            {
                Log.Write($"Event_mafiaMission Exception: {e.ToString()}");
            }
        }

        /*public static void Event_bikerMission(Player player, int id)
        {
            try
            {
                if (!player.IsCharacterData() || player.IsInVehicle) return;
                var sessionData = player.GetSessionData();
                int i = sessionData.OnBikerID;
                if (i == -1 || player.Position.DistanceTo(BikerStartDelivery[i]) > 6) return;
                int fraction = memberFractionData.Id;
                if (!NextDelivery.ContainsKey(fraction) || DateTime.Now < NextDelivery[fraction])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Попробуйте позже", 3000);
                    return;
                }
                if (sessionData.DeliveryData.Vehicle != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны сначала доставить предыдущую машину", 3000);
                    return;
                }
                Random rnd = new Random();
                VehicleHash vehicleHash = BikerVehiclesHashes[0];
                string text = "";
                switch (id)
                {
                    case 0:
                        text = "оружием";
                        break;
                    case 1:
                        text = "деньгами";
                        vehicleHash = BikerVehiclesHashes[1];
                        break;
                    case 2:
                        text = "трупами";
                        vehicleHash = BikerVehiclesHashes[2];
                        break;
                }
                Vehicle vehicle = VehicleStreaming.CreateVehicle(vehicleHash, BikerStartDelivery[i] + new Vector3(0, 0, 1), BikerStartDeliveryRot[i], 0, 0, "xxxxx", acc: "BIKERDELIVERY", petrol: 100);
                vehicle.CustomPrimaryColor = new Color(0, 0, 0);
                vehicle.CustomSecondaryColor = new Color(0, 0, 0);
                int end = rnd.Next(0, 12);
                VehicleStreaming.DeliveryData data = vehicleLocalData.DeliveryData;
                data.End = end;
                data.Fraction = fraction;
                data.DataEnd = DateTime.Now.AddSeconds(vehicle.Position.DistanceTo(MafiaEndDelivery[end]) / 100 * 2);
                data.JStage = false;
                data.WhosVeh = player;
                sessionData.DeliveryData.Point = end;
                sessionData.DeliveryData.Vehicle = vehicle;
                VehicleStreaming.SetEngineState(vehicle, true);
                NextDelivery[fraction] = DateTime.Now.AddMinutes(5);
                Trigger.SendChatMessage(player, "Если вдруг Вы сбили координаты машины, то напишите в чат /mypoint");
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы получили фургон с {text} для транспортировки. Отвезите его в указанное на карте место", 3000);
                Trigger.ClientEvent(player, "createWaypoint", MafiaEndDelivery[end].X, MafiaEndDelivery[end].Y);
                player.SetIntoVehicle(vehicle, (int)VehicleSeat.Driver);
            }
            catch (Exception e)
            {
                Log.Write($"Event_bikerMission Exception: {e.ToString()}");
            }
        }*/
    }
}
