using System;
using System.Collections.Generic;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Core;
using Redage.SDK;
using System.Data;
using MySqlConnector;
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

namespace NeptuneEvo.Fractions
{
    class AlcoFabrication : Script
    {
        private static readonly nLog Log = new nLog("Fractions.Fabrication");
        private static Dictionary<int, Vector3> EnterAlcoShop = new Dictionary<int, Vector3>()
        {
            { (int) Models.Fractions.LCN, new Vector3(-1388.761, -586.3921, 29.09945) },
            { (int) Models.Fractions.YAKUZA, new Vector3(-564.5512, 275.6993, 81.98249) },
            { (int) Models.Fractions.ARMENIAN, new Vector3() },
        };
        private static Dictionary<int, Vector3> ExitAlcoShop = new Dictionary<int, Vector3>()
        {
            { (int) Models.Fractions.LCN, new Vector3(-1387.458, -588.3003, 29.19951) },
            { (int) Models.Fractions.YAKUZA, new Vector3(-564.487, 277.4747, 82.01633) },
            { (int) Models.Fractions.ARMENIAN, new Vector3() },
        };
        private static Dictionary<int, string> ClubsNames = new Dictionary<int, string>()
        {
            { (int) Models.Fractions.LCN, "Клуб Bahama Mamas West" },
            { (int) Models.Fractions.RUSSIAN, "Клуб Vanilla" },
            { (int) Models.Fractions.YAKUZA, "Клуб Tequilla" },
            { (int) Models.Fractions.ARMENIAN, "Клуб Diamond" },
        };

        public static Dictionary<int, Stock> ClubsStocks = new Dictionary<int, Stock>();
        private static int MaxMats = 4000;
        private static Dictionary<int, Vector3> UnloadPoints = new Dictionary<int, Vector3>()
        {
            { (int) Models.Fractions.LCN, new Vector3(-1404.037, -633.443, 27.55337) },
            { (int) Models.Fractions.RUSSIAN, new Vector3(141.3792, -1281.576, 28.2172) },
            //{ (int) Models.Fractions.YAKUZA, new Vector3(-1451.7233, -52.355694, 52.7) },
            { (int) Models.Fractions.YAKUZA, new Vector3(-562.1907, 302.23157, 83.16) },
            { (int) Models.Fractions.ARMENIAN, new Vector3(975.3054, 7.2511, 80.5784) },
        };
        private static Dictionary<int, Vector3> BuyPoints = new Dictionary<int, Vector3>()
        {
            { (int) Models.Fractions.LCN, new Vector3(-1394.523, -602.7082, 29.19955) },
            { (int) Models.Fractions.RUSSIAN, new Vector3(126.4378, -1282.892, 28.15849) },
            { (int) Models.Fractions.YAKUZA, new Vector3(-560.0757, 286.7839, 81.05637) },
            { (int) Models.Fractions.ARMENIAN, new Vector3(945.4526, 16.871, 115.5442) },
        };

        private static int[] DrinksPrices = new int[3] { 75, 115, 150};
        private static int[] DrinksMats = new int[3] { 5, 7, 10 };
        private static Dictionary<int, List<ItemId>> DrinksInClubs = new Dictionary<int, List<ItemId>>()
        {
            { (int) Models.Fractions.LCN, new List<ItemId>(){ ItemId.LcnDrink1, ItemId.LcnDrink2, ItemId.LcnDrink3} },
            { (int) Models.Fractions.RUSSIAN, new List<ItemId>(){ ItemId.RusDrink1, ItemId.RusDrink2, ItemId.RusDrink3} },
            { (int) Models.Fractions.YAKUZA, new List<ItemId>(){ ItemId.YakDrink1, ItemId.YakDrink2, ItemId.YakDrink3} },
            { (int) Models.Fractions.ARMENIAN, new List<ItemId>(){ ItemId.ArmDrink1, ItemId.ArmDrink2, ItemId.ArmDrink3} },
        };

        public static Dictionary<ItemId, Vector3> AlcoPosOffset = new Dictionary<ItemId, Vector3>()
        {
            { ItemId.LcnDrink1, new Vector3(0.15, -0.25, -0.1) },
            { ItemId.LcnDrink2, new Vector3(0.15, -0.25, -0.1) },
            { ItemId.LcnDrink3, new Vector3(0.15, -0.23, -0.1) },
            { ItemId.RusDrink1, new Vector3(0.15, -0.23, -0.1) },
            { ItemId.RusDrink2, new Vector3(0.15, -0.23, -0.1) },
            { ItemId.RusDrink3, new Vector3(0.15, -0.23, -0.1) },
            { ItemId.YakDrink1, new Vector3(0.12, -0.02, -0.03) },
            { ItemId.YakDrink2, new Vector3(0.15, -0.23, -0.10) },
            { ItemId.YakDrink3, new Vector3(0.15, 0.03, -0.06) },
            { ItemId.ArmDrink1, new Vector3(0.15, -0.18, -0.10) },
            { ItemId.ArmDrink2, new Vector3(0.15, -0.18, -0.10) },
            { ItemId.ArmDrink3, new Vector3(0.15, -0.18, -0.10) },
        };
        public static Dictionary<ItemId, Vector3> AlcoRotOffset = new Dictionary<ItemId, Vector3>()
        {
            { ItemId.LcnDrink1, new Vector3(-80, 0, 0) },
            { ItemId.LcnDrink2, new Vector3(-80, 0, 0) },
            { ItemId.LcnDrink3, new Vector3(-80, 0, 0) },
            { ItemId.RusDrink1, new Vector3(-80, 0, 0) },
            { ItemId.RusDrink2, new Vector3(-80, 0, 0) },
            { ItemId.RusDrink3, new Vector3(-80, 0, 0) },
            { ItemId.YakDrink1, new Vector3(-80, 0, 0) },
            { ItemId.YakDrink2, new Vector3(-80, 0, 0) },
            { ItemId.YakDrink3, new Vector3(-80, 0, 0) },
            { ItemId.ArmDrink1, new Vector3(-80, 0, 0) },
            { ItemId.ArmDrink2, new Vector3(-80, 0, 0) },
            { ItemId.ArmDrink3, new Vector3(-80, 0, 0) },
        };

        [ServerEvent(Event.ResourceStart)]
        public void Event_ResourceStart()
        {
            try
            {
                if (Main.ServerSettings.IsDeleteProp)
                {
                    NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_strip_door_01"),
                        new Vector3(127.9552, -1298.503, 29.41962), 30f); //X:127,9552 Y:-1298,503 Z:29,41962
                }

                if (Main.ServerSettings.IsCreateProp)
                {
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("v_ilev_ph_gendoor006"),
                        new Vector3(-1386.99683, -586.663208, 30.4694996), new Vector3(0, 0, 33.9277153), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("v_ilev_ph_gendoor006"),
                        new Vector3(-1389.17236, -588.086914, 30.4694996), new Vector3(0, -0, -147.719879), 255,
                        NAPI.GlobalDimension);

                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("apa_mp_h_stn_chairarm_03"),
                        new Vector3(-1397.15088, -598.213379, 29.3224068), new Vector3(0, 0, -18.1152821), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("apa_mp_h_stn_chairarm_03"),
                        new Vector3(-1397.08069, -600.813477, 29.3224068), new Vector3(0, -0, -138.115219), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("apa_mp_h_stn_chairarm_03"),
                        new Vector3(-1399.99353, -600.623291, 29.3224068), new Vector3(0, -0, 119.884583), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("apa_mp_h_stn_chairarm_03"),
                        new Vector3(-1401.09326, -601.223145, 29.3224068), new Vector3(0, 0, -24.1148987), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("apa_mp_h_stn_chairarm_03"),
                        new Vector3(-1399.75366, -602.2229, 29.3224068), new Vector3(0, 0, -41.9143143), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("apa_mp_h_stn_chairarm_03"),
                        new Vector3(-1399.51343, -604.222656, 29.3224068), new Vector3(0, -0, -94.9140701), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("apa_mp_h_stn_chairarm_03"),
                        new Vector3(-1401.00488, -606.364746, 29.3224068), new Vector3(0, -0, -161.913498), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("apa_mp_h_stn_chairarm_03"),
                        new Vector3(-1403.46729, -604.663086, 29.3224068), new Vector3(0, -0, 129.486343), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("apa_mp_h_stn_chairarm_03"),
                        new Vector3(-1404.37708, -603.463379, 29.3224068), new Vector3(0, -0, 124.486282), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("apa_mp_h_stn_chairarm_03"),
                        new Vector3(-1400.23792, -610.231201, 29.3224068), new Vector3(0, 0, 55.4857788), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("apa_mp_h_stn_chairarm_03"),
                        new Vector3(-1397.53857, -613.230469, 29.3224068), new Vector3(0, -0, -174.514252), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("apa_mp_h_stn_chairarm_03"),
                        new Vector3(-1395.72827, -611.990723, 29.3224068), new Vector3(0, -0, -104.513588), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("apa_mp_h_stn_chairarm_03"),
                        new Vector3(-1396.07861, -609.95874, 29.3224068), new Vector3(0, 0, -55.5132561), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("apa_mp_h_stn_chairarm_03"),
                        new Vector3(-1397.49976, -608.927734, 29.3224068), new Vector3(0, 0, -20.513237), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("apa_mp_h_stn_chairarm_03"),
                        new Vector3(-1396.10718, -615.662598, 29.3224068), new Vector3(0, -0, -127.513763), 255,
                        NAPI.GlobalDimension);

                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_huge_display_01"),
                        new Vector3(371.9039, -990.349854, -98.0589447), new Vector3(0, 0, -89.7690125), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_huge_display_01"),
                        new Vector3(376.57428, -990.049927, -96.4589691), new Vector3(0, -0, -179.769073), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_huge_display_01"),
                        new Vector3(372.103912, -1004.65002, -98.0589447), new Vector3(0, 0, -89.7690048), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_huge_display_01"),
                        new Vector3(377.604248, -1004.15015, -98.0589447), new Vector3(0, 0, 0.23099421), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_huge_display_01"),
                        new Vector3(383.774689, -1004.15015, -98.0589447), new Vector3(0, -0, 90.2309723), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_huge_display_01"),
                        new Vector3(381.704498, -992.852905, -98.0589447), new Vector3(0, -0, 90.2309647), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_huge_display_01"),
                        new Vector3(388.004883, -998.751465, -98.0589447), new Vector3(0, -0, -179.769104), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_huge_display_01"),
                        new Vector3(370.503998, -998.349854, -98.0589447), new Vector3(0, 0, -89.7690048), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_huge_display_01"),
                        new Vector3(365.674225, -996.549805, -98.4589691), new Vector3(0, -0, -179.769073), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_huge_display_01"),
                        new Vector3(365.594147, -998.883057, -98.4589691), new Vector3(0, 0, 0.231002808), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("lr_prop_clubstool_01"),
                        new Vector3(376.016602, -999.739929, -100.028435), new Vector3(0, 0, 89.774147), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("lr_prop_clubstool_01"),
                        new Vector3(375.938019, -1000.87671, -100.028435), new Vector3(0, 0, 81.4817657), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("lr_prop_clubstool_01"),
                        new Vector3(375.919434, -1001.87073, -100.028435), new Vector3(0, -0, 109.426132), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("lr_prop_clubstool_01"),
                        new Vector3(376.111938, -1003.08606, -100.028435), new Vector3(0, -0, 136.909775), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("lr_prop_clubstool_01"),
                        new Vector3(375.815247, -990.539917, -99.7284622), new Vector3(0, 0, -4.8109498), 255,
                        NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("p_yoga_mat_03_s"),
                        new Vector3(373.065887, -999.187195, -98.9689713),
                        new Vector3(8.40430744e-07, 89.9999466, 179.998123), 255, NAPI.GlobalDimension);
                    NAPI.Object.CreateObject(NAPI.Util.GetHashKey("hei_heist_kit_bin_01"),
                        new Vector3(373.325378, -999.457397, -99.9999771), new Vector3(0, 0, 47.6214714), 255,
                        NAPI.GlobalDimension);

                }

                Main.CreateBlip(new Main.BlipData(121, "Клуб Bahama Mamas West", new Vector3(-1388.761, -586.3921, 29.09945), 0, true));
                Main.CreateBlip(new Main.BlipData(121, "Клуб Vanilla", new Vector3(141.3792, -1281.576, 28.2172), 0, true));
                Main.CreateBlip(new Main.BlipData(121, "Клуб Tequilla", new Vector3(-564.5512, 275.6993, 81.98249), 0, true));
                //Main.CreateBlip(new Main.BlipData(121, "Клуб Diamond", new Vector3(945.4526, 16.871, 115.0442), 0, true));
                
                Main.CreateBlip(new Main.BlipData(136, "Кафе Stand Up", new Vector3(-439.601, 271.704, 82.015), 0, true));
                Main.CreateBlip(new Main.BlipData(136, "Китайский ресторан", new Vector3(-158.242, 303.829,98.822), 0, true));

                Main.CreateBlip(new Main.BlipData(674, "White Company", new Vector3(-858.5887, -424.25665, 84.235405), 73, true));

                /*NAPI.Blip.CreateBlip(93, new Vector3(-1388.761, -586.3921, 29.09945), 1, 0, "Club", 255, 0, true);
                NAPI.Blip.CreateBlip(93, new Vector3(141.3792, -1281.576, 28.2172), 1, 0, "Club", 255, 0, true);
                NAPI.Blip.CreateBlip(93, new Vector3(-564.5512, 275.6993, 81.98249), 1, 0, "Club", 255, 0, true);
                NAPI.Blip.CreateBlip(93, new Vector3(945.4526, 16.871, 115.0442), 1, 0, "Club", 255, 0, true);*/

                using MySqlCommand cmd = new MySqlCommand()
                {
                    CommandText = "SELECT * FROM `alcoclubs`"
                };
                using DataTable result = MySQL.QueryRead(cmd);
                if (result == null || result.Rows.Count == 0)
                {
                    Log.Write("DB alcoclubs return null result.", nLog.Type.Warn);
                    return;
                }
                int id;
                foreach (DataRow Row in result.Rows)
                {
                    id = Convert.ToInt32(Row["id"]);
                    ClubsStocks.Add(id, new Stock(Convert.ToInt32(Row["mats"]), Convert.ToInt32(Row["alco1"]), Convert.ToInt32(Row["alco2"]), Convert.ToInt32(Row["alco3"]), Convert.ToSingle(Convert.ToInt32(Row["pricemod"]) / 100), UnloadPoints[id] + new Vector3(0, 0, 0.8)));
                }

                #region Enter AlcoShops
                foreach (KeyValuePair<int, Vector3> pair in EnterAlcoShop)
                {
                    CustomColShape.CreateCylinderColShape(pair.Value, 1f, 2, NAPI.GlobalDimension, ColShapeEnums.EnterAlcoShop, pair.Key);

                    NAPI.Marker.CreateMarker(1, pair.Value - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220), false, NAPI.GlobalDimension);
                    NAPI.TextLabel.CreateTextLabel($"~w~Club\n\"{ClubsNames[pair.Key]}\"", pair.Value + new Vector3(0, 0, 0.5), 5f, 0.3f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension);
                }
                #endregion
                #region Exit AlcoShops
                foreach (KeyValuePair<int, Vector3> pair in ExitAlcoShop)
                {
                    CustomColShape.CreateCylinderColShape(pair.Value, 1f, 2, NAPI.GlobalDimension, ColShapeEnums.ExitAlcoShop, pair.Key);


                    NAPI.Marker.CreateMarker(1, pair.Value - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220), false, NAPI.GlobalDimension);
                    NAPI.TextLabel.CreateTextLabel($"~w~Выход", pair.Value + new Vector3(0, 0, 0.5), 5f, 0.3f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension);
                }
                #endregion
                #region Unloadpoints
                foreach (KeyValuePair<int, Vector3> pair in UnloadPoints)
                {
                    CustomColShape.CreateCylinderColShape(pair.Value, 5, 5, NAPI.GlobalDimension, ColShapeEnums.UnloadPoints, pair.Key);

                    NAPI.Marker.CreateMarker(1, pair.Value - new Vector3(0, 0, 4.5), new Vector3(), new Vector3(), 5, new Color(255, 0, 0, 220), false, NAPI.GlobalDimension);
                    NAPI.TextLabel.CreateTextLabel($"~w~Разгрузить материалы", pair.Value + new Vector3(0, 0, 0.5), 5f, 0.3f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension);
                }
                #endregion
                #region BuyPoints
                foreach (KeyValuePair<int, Vector3> pair in BuyPoints)
                {
                    CustomColShape.CreateCylinderColShape(pair.Value, 1.5f, 2, NAPI.GlobalDimension, ColShapeEnums.BuyPoints, pair.Key);

                    NAPI.Marker.CreateMarker(1, pair.Value - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220), false, NAPI.GlobalDimension);
                    NAPI.TextLabel.CreateTextLabel($"~w~Купить алкоголь", pair.Value + new Vector3(0, 0, 0.5), 5f, 0.3f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension);
                }
                #endregion
            }
            catch (Exception e)
            {
                Log.Write($"Event_ResourceStart Exception: {e.ToString()}");
            }
        }

        [Interaction(ColShapeEnums.EnterAlcoShop)]
        public static void OnEnterAlcoShop(ExtPlayer player, int index)
        {
            try
            {
                if (player.GetFractionId() != index)
                {
                    switch (index)
                    {
                        case 10:
                            if (!Main.DoorsControl.ContainsKey("lcn_0") || Main.DoorsControl["lcn_0"])
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ClubDoorClosed), 3000);
                                return;
                            }
                            break;
                        case 12:
                            if (!Main.DoorsControl.ContainsKey("yk_2") || Main.DoorsControl["yk_2"])
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ClubDoorClosed), 3000);
                                return;
                            }
                            break;
                        default:
                            break;
                    }
                }
                NAPI.Entity.SetEntityPosition(player, ExitAlcoShop[index] + new Vector3(0, 0, 1.2));
            }
            catch (Exception e)
            {
                Log.Write($"OnEnterAlcoShop Exception: {e.ToString()}");
            }
        }
        [Interaction(ColShapeEnums.ExitAlcoShop)]
        public static void OnExitAlcoShop(ExtPlayer player, int index)
        {
            try
            {
                if (player.GetFractionId() != index)
                {
                    switch (index)
                    {
                        case 10:
                            if (!Main.DoorsControl.ContainsKey("lcn_0") || Main.DoorsControl["lcn_0"])
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ClubDoorClosed), 3000);
                                return;
                            }
                            break;
                        case 12:
                            if (!Main.DoorsControl.ContainsKey("yk_2") || Main.DoorsControl["yk_2"])
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ClubDoorClosed), 3000);
                                return;
                            }
                            break;
                        default:
                            break;
                    }
                }
                NAPI.Entity.SetEntityPosition(player, EnterAlcoShop[index] + new Vector3(0, 0, 1.2));
            }
            catch (Exception e)
            {
                Log.Write($"OnExitAlcoShop Exception: {e.ToString()}");
            }
        }
        [Interaction(ColShapeEnums.UnloadPoints)]
        public static void OnUnloadPoints(ExtPlayer player, int index)
        {
            try
            {
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                if (memberFractionData.Id != index)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ClubNotInFraction, Manager.GetName(index)), 3000);
                    return;
                }
                if (!player.IsInVehicle)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustInCar), 3000);
                    return;
                }
                int club = index;
                int matCount = Chars.Repository.getCountItem(VehicleManager.GetVehicleToInventory(player.Vehicle.NumberPlate), ItemId.Material);
                if (matCount == 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarNoMats), 3000);
                    return;
                }
                if (ClubsStocks[club].Materials >= MaxMats)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WarehouseFilled), 3000);
                    return;
                }
                Chars.Repository.Remove(null, VehicleManager.GetVehicleToInventory(player.Vehicle.NumberPlate), "vehicle", ItemId.Material, matCount);
                ClubsStocks[club].Materials += matCount;
                ClubsStocks[club].UpdateLabel();
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FromCarToClubWarehouse), 3000);
                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.TakeMedkits, LangFunc.GetText(LangType.Ru, DataName.FtableLoadedFromCarToClubWarehouse, matCount));
            }
            catch (Exception e)
            {
                Log.Write($"OnUnloadPoints Exception: {e.ToString()}");
            }
        }
        public static void ResistTimer(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                ResistData resistData = sessionData.ResistData;
                if (resistData.Time-- <= 0)
                {
                    if (sessionData.TimersData.ResistTimer != null) Timers.Stop(sessionData.TimersData.ResistTimer);
                    sessionData.TimersData.ResistTimer = null;
                    Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");
                    Trigger.ClientEvent(player, "setResistStage", 0);
                    resistData.Time = 0;
                    resistData.Ban = false;
                }
            }
            catch (Exception e)
            {
                Log.Write($"ResistTimer Exception: {e.ToString()}");
            }
        }

        public static void SaveAlco()
        {
            try
            {
                foreach (KeyValuePair<int, Stock> club in ClubsStocks)
                {
                    try
                    {
                        using MySqlCommand cmd = new MySqlCommand
                        {
                            CommandText = "UPDATE alcoclubs SET alco1=@val0,alco2=@val1,alco3=@val2,pricemod=@val3,mats=@val4 WHERE id=@val5"
                        };
                        cmd.Parameters.AddWithValue("@val0", club.Value.Alco1);
                        cmd.Parameters.AddWithValue("@val1", club.Value.Alco2);
                        cmd.Parameters.AddWithValue("@val2", club.Value.Alco3);
                        cmd.Parameters.AddWithValue("@val3", Convert.ToInt32(club.Value.PriceModifier * 100));
                        cmd.Parameters.AddWithValue("@val4", club.Value.Materials);
                        cmd.Parameters.AddWithValue("@val5", club.Key);
                        MySQL.Query(cmd);
                    }
                    catch (Exception e)
                    {
                        Log.Write($"SaveAlco Foreach Exception: {e.ToString()}");
                    }
                }
                Log.Write("Alco has been saved to DB", nLog.Type.Success);
            }
            catch (Exception e)
            {
                Log.Write($"SaveAlco Exception: {e.ToString()}");
            }
        }

        #region Buy Menu
        [Interaction(ColShapeEnums.BuyPoints)]
        public static void OnBuyPoints(ExtPlayer player, int club)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                var memberFractionData = player.GetFractionMemberData();
                var isOwner = false;
                if (memberFractionData != null)
                {
                    var fractionData = Manager.GetFractionData(memberFractionData.Id);
                    if (fractionData != null)
                        isOwner = (fractionData.Id == club && memberFractionData.Rank >= (fractionData.LeaderRank() - 1));
                }
               
                var stock = new List<int>()
                {
                    ClubsStocks[club].Materials,
                    ClubsStocks[club].Alco1,
                    ClubsStocks[club].Alco2,
                    ClubsStocks[club].Alco3,
                };
                Trigger.ClientEvent(player, "openAlco", club, ClubsStocks[club].PriceModifier, isOwner, stock);
            }
            catch (Exception e)
            {
                Log.Write($"OpenBuyAlcoholMenu Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("menu_alco")]
        public static void RemoteEvent_alcoMenu(ExtPlayer player, int action, int index)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var fractionId = CustomColShape.GetDataToEnum(player, ColShapeEnums.BuyPoints);
                if (fractionId == (int)ColShapeData.Error) return;
                var fractionData = Manager.GetFractionData(fractionId);
                if (fractionData == null)
                    return;
                
                List<int> alcoCounts = new List<int>() { ClubsStocks[fractionId].Alco1, ClubsStocks[fractionId].Alco2, ClubsStocks[fractionId].Alco3 };
                ItemId invItem = DrinksInClubs[fractionId][index];

                switch (action)
                {
                    case 0: // buy
                        if (alcoCounts[index] <= 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно товара", 3000);
                            return;
                        }
                        if (Chars.Repository.isFreeSlots(player, invItem) != 0) return;
                        int price = Convert.ToInt32(DrinksPrices[index] * ClubsStocks[fractionId].PriceModifier);
                        if (UpdateData.CanIChange(player, price, true) != 255) return;
                        MoneySystem.Wallet.Change(player, -price);
                        fractionData.Money += price;
                        GameLog.Money($"player({characterData.UUID})", $"frac({fractionId})", price, $"buyAlco");
                        Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", invItem);
                        BattlePass.Repository.UpdateReward(player, 37);

                        switch (index)
                        {
                            case 0:
                                if (ClubsStocks[fractionId].Alco1 <= 0)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetTovaraNaSkladeBiza), 3000);
                                    return;
                                }
                                ClubsStocks[fractionId].Alco1--;
                                break;
                            case 1:
                                if (ClubsStocks[fractionId].Alco2 <= 0)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetTovaraNaSkladeBiza), 3000);
                                    return;
                                }
                                ClubsStocks[fractionId].Alco2--;
                                break;
                            case 2:
                                if (ClubsStocks[fractionId].Alco3 <= 0)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetTovaraNaSkladeBiza), 3000);
                                    return;
                                }
                                ClubsStocks[fractionId].Alco3--;
                                break;
                            default:
                                // Not supposed to end up here. 
                                break;
                        }
                        ClubsStocks[fractionId].UpdateLabel();
                        //OpenBuyAlcoholMenu(player);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили {Chars.Repository.ItemsInfo[invItem].Name}", 3000);
                        return;
                    case 1: // take
                        if (alcoCounts[index] <= 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно товара", 3000);
                            return;
                        }
                        if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", invItem) == -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }

                        switch (index)
                        {
                            case 0:
                                if (ClubsStocks[fractionId].Alco1 <= 0)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetTovaraNaSkladeBiza), 3000);
                                    return;
                                }
                                ClubsStocks[fractionId].Alco1--;
                                break;
                            case 1:
                                if (ClubsStocks[fractionId].Alco2 <= 0)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetTovaraNaSkladeBiza), 3000);
                                    return;
                                }
                                ClubsStocks[fractionId].Alco2--;
                                break;
                            case 2:
                                if (ClubsStocks[fractionId].Alco3 <= 0)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetTovaraNaSkladeBiza), 3000);
                                    return;
                                }
                                ClubsStocks[fractionId].Alco3--;
                                break;
                            default:
                                // Not supposed to end up here. 
                                break;
                        }
                        ClubsStocks[fractionId].UpdateLabel();
                        //OpenBuyAlcoholMenu(player);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы взяли {Chars.Repository.ItemsInfo[invItem].Name}. На складе {alcoCounts[index] - 1}шт", 3000);
                        return;
                    case 2: // craft
                        if (alcoCounts[index] >= 80)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"На складе максимум {Chars.Repository.ItemsInfo[invItem].Name}", 3000);
                            return;
                        }
                        if (ClubsStocks[fractionId].Materials < DrinksMats[index])
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"На складе недостаточно материалов", 3000);
                            return;
                        }

                        ClubsStocks[fractionId].Materials -= DrinksMats[index];
                        switch (index)
                        {
                            case 0:
                                ClubsStocks[fractionId].Alco1++;
                                break;
                            case 1:
                                ClubsStocks[fractionId].Alco2++;
                                break;
                            case 2:
                                ClubsStocks[fractionId].Alco3++;
                                break;
                            default:
                                // Not supposed to end up here. 
                                break;
                        }

                        ClubsStocks[fractionId].UpdateLabel();
                        //OpenBuyAlcoholMenu(player);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы скрафтили {Chars.Repository.ItemsInfo[invItem].Name}. На складе {alcoCounts[index] + 1}шт", 3000);
                        return;
                    case 3: // setprice
                        Trigger.ClientEvent(player, "openInput", "Установить цену", "Введите цену для алкоголя в процентах", 3, "club_setprice");
                        return;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"RemoteEvent_alcoMenu Exception: {e.ToString()}");
            }
        }
        public static void SetAlcoholPrice(ExtPlayer player, int price)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                var fracId = player.GetFractionId();
                if (!player.IsFractionLeader() || !ClubsStocks.ContainsKey(fracId)) return;
                if (price < 50 || price > 150)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Установите цену от 50% до 150%", 3000);
                    return;
                }
                ClubsStocks[fracId].PriceModifier = price / 100.0f;
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы изменили цену алкогольной продукции до {price}%", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"SetAlcoholPrice Exception: {e.ToString()}");
            }
        }
        #endregion

        internal class Stock
        {
            public int Materials { get; set; }
            public int Alco1 { get; set; }
            public int Alco2 { get; set; }
            public int Alco3 { get; set; }
            public float PriceModifier { get; set; }

            public ExtTextLabel Label { get; set; }

            public Stock(int mats, int a1, int a2, int a3, float price, Vector3 pos)
            {
                Label = (ExtTextLabel) NAPI.TextLabel.CreateTextLabel($"~w~Материалы: ~r~{mats}\n~w~Алкоголь: ~r~{a1 + a2 + a3}", pos, 30f, 0.3f, 0, new Color());

                Materials = mats;
                Alco1 = a1;
                Alco2 = a2;
                Alco3 = a3;
                PriceModifier = price;
            }

            public void UpdateLabel()
            {
                Label.Text = $"~w~Материалы: ~r~{Materials}\n~w~Алкоголь: ~r~{Alco1 + Alco2 + Alco3}";
            }
        }
    }
}
