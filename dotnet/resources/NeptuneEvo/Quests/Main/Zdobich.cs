using System;
using System.Collections.Generic;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.Houses;
using NeptuneEvo.Jobs.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Quests.Models;
using Redage.SDK;

namespace NeptuneEvo.Quests
{    
    public enum zdobich_quests
    {
        Error = -99,
        NoMission = -1,
        Start = 0,
        Stage2 = 2,
        Stage3 = 3,
        Stage7 = 7,
        Stage8 = 8,
        Stage9 = 9,
        Stage10 = 10,
        Stage11 = 11,
        Stage12 = 12,//
        Stage13 = 13,//
        Stage14 = 14,//
        Stage15 = 15,//
        Stage16 = 16,//
        Stage17 = 17,//
        Stage18 = 18,//
        Stage19 = 19,//
        Stage20 = 20,
        Stage21 = 21,
        Stage23 = 23,
        Stage24 = 24,
        Stage25 = 25,
        Stage26 = 26,
        Stage28 = 28,
        Stage29 = 29,
        Stage30 = 30,
        Stage31 = 31,
        Stage33 = 33,
        Stage34 = 34,
        End = 35,
    };
    
    public class Zdobich : Script
    {
        private static readonly nLog Log = new nLog("Quests.Zdobich");
        public static string QuestName = "npc_zdobich";

        public static void Perform(ExtPlayer player, PlayerQuestModel playerQuestData)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                var returnLine = Get(player, playerQuestData.Line);
                
                if (returnLine != zdobich_quests.Error)
                {
                    var questData = player.GetQuest();
                    if (questData != null)
                        questData.Line = (int)returnLine;
                    
                    qMain.UpdatePerform(player, QuestName, (short)returnLine);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Perform Task.Run Exception: {e.ToString()}");
            }
        }
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            PedSystem.Repository.CreateQuest("u_m_m_partytarget", new Vector3(-480.28732, -305.2201, 35.106697), -157f, questName: QuestName, title: "~y~NPC~w~ Виталий Дебич\nКвесты новичков", colShapeEnums: ColShapeEnums.QuestZdobich);
            
                //PedSystem.Repository.CreateQuest("a_c_rhesus", new Vector3(-592.5721, -1598.3741, 27.003511), 163f, title: "~y~NPC~w~ Праздничная обезьянка\nБонус-код", colShapeEnums: ColShapeEnums.QuestBonus);
            /*  PedSystem.Repository.CreateQuest("s_f_y_cop_01", new Vector3(1407.8423, 680.5664, 78.88384), -16f, questName: "s_f_y_cop_01", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_m_dockwork_01", new Vector3(1449.9875, 742.5421, 77.59714), -104f, questName: "s_m_m_dockwork_01", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_y_dockwork_01", new Vector3(1395.494, 666.13245, 79.42329), 83f, questName: "s_m_y_dockwork_01", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_y_cop_01", new Vector3(795.9713, -2617.3062, 52.69027), -157f, questName: "s_m_y_cop_01", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("cs_floyd", new Vector3(780.4684, -2618.7764, 54.796406), -79f, questName: "cs_floyd", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_y_dwservice_02", new Vector3(744.6778, -2622.7385, 52.310837), 5f, questName: "s_m_y_dwservice_02", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_m_gardener_01", new Vector3(-1866.3303, -555.9034, 11.6730585), 75f, questName: "s_m_m_gardener_01", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_y_garbage", new Vector3(-1836.678, -579.55316, 11.465273), -148f, questName: "s_m_y_garbage", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_m_lathandy_01", new Vector3(-1878.8616, -570.582, 11.647385), -79f, questName: "s_m_m_lathandy_01", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_m_gaffer_01", new Vector3(1880.0299, 3602.4546, 34.28307), 115f, questName: "s_m_m_gaffer_01", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_m_janitor", new Vector3(1899.0159, 3608.6033, 34.17873), 20f, questName: "s_m_m_janitor", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_y_dwservice_01", new Vector3(1917.1903, 3625.2102, 33.830017), 119f, questName: "s_m_y_dwservice_01", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_m_ccrew_01", new Vector3(1229.4062, 6501.2583, 20.808804), -129f, questName: "s_m_m_ccrew_01", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("csb_trafficwarden", new Vector3(1173.5403, 6503.1914, 21.028954), 165f, questName: "csb_trafficwarden", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_m_autoshop_02", new Vector3(1180.2682, 6500.5273, 21.022112), 18f, questName: "s_m_m_autoshop_02", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_y_armymech_01", new Vector3(-442.57883, -1579.5824, 26.209757), -45f, questName: "s_m_y_armymech_01", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_y_airworker", new Vector3(-427.3042, -1579.2301, 25.961311), -68f, questName: "s_m_y_airworker", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_y_construct_01", new Vector3(-2687.807, 2335.939, 16.87326), 158f, questName: "s_m_y_construct_01", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_y_construct_01", new Vector3(-2677.4092, 2321.246, 18.283552), 99f, questName: "s_m_y_construct_01", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("csb_prolsec", new Vector3(-2496.321, 3561.1511, 15.015672), -113f, questName: "csb_prolsec", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("cs_prolsec_02", new Vector3(-2481.4668, 3554.9783, 14.840784), 68f, questName: "cs_prolsec_02", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_y_hwaycop_01", new Vector3(-2497.9534, 3570.3162, 14.781774), 42f, questName: "s_m_y_hwaycop_01", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_f_y_ranger_01", new Vector3(-2499.8394, 3598.4458, 14.4654665), -47f, questName: "s_f_y_ranger_01", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("csb_cop", new Vector3(-2505.556, 3540.326, 14.859877), -150f, questName: "csb_cop", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_m_armoured_01", new Vector3(-841.3982, -2015.4567, 27.954214), -36f, questName: "s_m_m_armoured_01", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_m_armoured_01", new Vector3(-854.94354, -2034.3693, 27.963247), -31f, questName: "s_m_m_armoured_01", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_y_swat_01", new Vector3(-843.1828, -2041.7688, 27.976416), 18f, questName: "s_m_y_swat_01", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_m_fibsec_01", new Vector3(-840.5013, -2008.0188, 27.648382), 150f, questName: "s_m_m_fibsec_01", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_m_ciasec_01", new Vector3(-844.27655, -2013.892, 27.714827), 52f, questName: "s_m_m_ciasec_01", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_y_swat_01", new Vector3(-843.1828, -2041.7688, 27.976416), 18f, questName: "s_m_y_swat_01", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_m_fibsec_01", new Vector3(-840.5013, -2008.0188, 27.648382), 150f, questName: "s_m_m_fibsec_01", isBlipVisible: false);						
  PedSystem.Repository.CreateQuest("s_m_m_ciasec_01", new Vector3(-844.27655, -2013.892, 27.714827), 52f, questName: "s_m_m_ciasec_01", isBlipVisible: false);						
  
              //PedSystem.Repository.CreateQuest("u_m_m_partytarget", new Vector3(440.4283, 227.84183, 103.16548), 160f, questName: QuestName, title: "~y~NPC~w~ Виталий Здобич\nКвесты новичков", colShapeEnums: ColShapeEnums.QuestZdobich);
              //PedSystem.Repository.CreateQuest("u_m_y_chip", new Vector3(249.6037, -1355.289, 25.55439), 226.06f, questName: "u_m_y_chip");
              
              
              PedSystem.Repository.CreateQuest("s_m_y_grip_01", new Vector3(894.2328, -180.24213, 74.70020), -61.5f, questName: "s_m_y_grip_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_ccrew_01", new Vector3(901.3334, -172.75513, 74.07588), -61.1f, questName: "s_m_m_ccrew_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_m_bevhills_01", new Vector3(902.55005, -171.59924, 74.07551), 131.5f, questName: "a_f_m_bevhills_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_chin_goon", new Vector3(345.33765, 3405.4438, 36.471214), 25.7f, questName: "csb_chin_goon", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_gaffer_01", new Vector3(351.66125,3403.6042,42.27361), 109.7f, questName: "s_m_m_gaffer_01", isBlipVisible: false);
              
              PedSystem.Repository.CreateQuest("a_m_m_hasjew_01", new Vector3(-1333.5615, 40.259937, 53.618484), -176.5f, questName: "a_m_m_hasjew_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_armymech_01", new Vector3(-1295.6888, 58.088093, 51.88434), -141.1f, questName: "s_m_y_armymech_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_golfer_01", new Vector3(-1115.0702, -107.10068, 41.84054), 106.5f, questName: "a_m_m_golfer_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_golfer_01", new Vector3(-1117.3242, -107.713234, 41.8439), -73.7f, questName: "a_m_y_golfer_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_martinmadrazo", new Vector3(-1341.37,76.05736, 60.544456), -107.7f, questName: "cs_martinmadrazo", isBlipVisible: false);
              
              PedSystem.Repository.CreateQuest("ig_andreas", new Vector3(129.9188, 97.812614, 83.50762), 160.5f, questName: "ig_andreas", isBlipVisible: false);
              //PedSystem.Repository.CreateQuest("s_m_y_garbage", new Vector3(735.5687, 131.32744, 80.71421), -113.1f, questName: "s_m_y_garbage", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_airworker", new Vector3(687.67175, 113.46483, 80.754486), 67.5f, questName: "s_m_y_airworker", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_dockwork_01", new Vector3(713.1452,159.66035, 80.754555), 160.7f, questName: "s_m_y_dockwork_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_m_m_edtoh", new Vector3(712.16815,157.47467, 80.754555), -24.7f, questName: "u_m_m_edtoh", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_s_m_armoured_01", new Vector3(-1463.3981, -501.06802, 32.961708), -50f, questName: "mp_s_m_armoured_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_f_y_princess", new Vector3(-1475.923, -516.15015, 34.73668), 3f, questName: "u_f_y_princess", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_bevhills_01", new Vector3(453.74634, -609.8368, 28.562132), -96f, questName: "a_m_m_bevhills_01", isBlipVisible: false); 
  
             PedSystem.Repository.CreateQuest("mp_s_m_armoured_01", new Vector3(-1463.3981, -501.06802, 32.961708), -50f, questName: "mp_s_m_armoured_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_f_y_princess", new Vector3(-1475.923, -516.15015, 34.73668), 3f, questName: "u_f_y_princess", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_bevhills_01", new Vector3(453.74634, -609.8368, 28.562132), -96f, questName: "a_m_m_bevhills_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_eastsa_03", new Vector3(452.65424, -596.26263, 28.727549 ), -5f, questName: "a_f_y_eastsa_03", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_paparazzi_01", new Vector3(486.35062, -1290.8723, 29.541498 ), -71f, questName: "a_m_m_paparazzi_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_prolhost_01", new Vector3(468.59912, -1271.2595, 29.708939), 138f, questName: "a_m_m_prolhost_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_c_poodle", new Vector3(340.8246, -1973.3387, 23.683811), -31f, questName: "a_c_poodle", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_skater_01", new Vector3(313.78955, -1865.8561, 26.892767), -35f, questName: "a_m_m_skater_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_wade", new Vector3(114.56376, -561.9772, 31.631151 ), -16f, questName: "ig_wade", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_denise", new Vector3(236.33472, -1956.6381, 23.011354 ), 27f, questName: "ig_denise", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_m_y_babyd", new Vector3(-98.49335, -1781.4663, 29.12598), -59f, questName: "u_m_y_babyd", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_y_ballaorig_01", new Vector3(-98.69043, -1781.9897, 29.139635), -78f, questName: "g_m_y_ballaorig_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_y_ballaeast_01", new Vector3(-97.87904, -1780.8462, 29.09918), 141f, questName: "g_m_y_ballaeast_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_airworker", new Vector3(-409.71536, -1703.9547, 19.398516), -137f, questName: "s_m_y_airworker", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_y_armgoon_02", new Vector3(-410.49387, -1705.3748, 19.464085 ), -98f, questName: "g_m_y_armgoon_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_fireman_01", new Vector3(221.16182, -1643.278, 29.569368 ), 144f, questName: "s_m_y_fireman_01", isBlipVisible: false);
              
              PedSystem.Repository.CreateQuest("ig_hao", new Vector3(88.08468, -1437.138, 29.311644), -59f, questName: "ig_hao", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_cop_01", new Vector3(86.12889, -1438.4209, 29.311632 ), -60f, questName: "s_m_y_cop_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_o_salton_01", new Vector3(117.423134, -1461.4086, 29.296028), -139f, questName: "a_f_o_salton_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_f_deadhooker", new Vector3( 100.23757, -1317.7104, 29.290413), 143f, questName: "mp_f_deadhooker", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_f_y_hooker_02", new Vector3(97.75625, -1313.0685, 29.288301), 128f, questName: "s_f_y_hooker_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_f_y_hooker_01", new Vector3(90.88431, -1312.4987, 29.292175), 152f, questName: "s_f_y_hooker_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_indian_01 ", new Vector3(-251.64357, -885.30896, 30.690388), -59f, questName: "a_m_m_indian_01 ", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_janet ", new Vector3(-248.39497, -888.166, 30.554988), 24f, questName: "cs_janet ", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_jimmyboston ", new Vector3 (-1214.6748, -1219.47, 7.6873646), -70f, questName: "ig_jimmyboston ", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_y_korlieut_01 ", new Vector3(-696.4007, -0.85255075, 38.648804), -157f, questName: "g_m_y_korlieut_01 ", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_salton_03", new Vector3(342.86948, -1075.1805, 29.544144), -162f, questName: "a_m_m_salton_03", isBlipVisible: false);
  
              PedSystem.Repository.CreateQuest("a_m_y_stwhi_01", new Vector3(290.46497, -1029.7036, 44.64717), -149f, questName: "a_m_y_stwhi_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_gaffer_01", new Vector3(170.40768, -1043.9097, 38.348385), 152f, questName: "s_m_m_gaffer_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_dealer_01", new Vector3(304.5049, -943.20886, 29.41466), -157f, questName: "s_m_y_dealer_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_c_shepherd", new Vector3(305.774, -945.82214, 28.873257), 114f, questName: "a_c_shepherd", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_s_m_armoured_01", new Vector3(212.71321, -809.0236, 31.013863), -32f, questName: "mp_s_m_armoured_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_solomon", new Vector3(114.10088, -776.3899, 31.41963), -20f, questName: "cs_solomon", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_popov", new Vector3(-303.3214, -829.8951, 32.417244), -7f, questName: "ig_popov", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_solomon", new Vector3(-1314.7924, -836.119, 16.960007), -48f, questName: "cs_solomon ", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_tanisha", new Vector3(-1204.9055, -326.44028, 37.836452 ), 114f, questName: "ig_tanisha", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_tom", new Vector3(-866.56116, -187.92842, 37.8426), 122f, questName: "cs_tom", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_taocheng", new Vector3(1409.5605, -100.47425, 52.38431), 107f, questName: "ig_taocheng", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_f_y_migrant_01", new Vector3(-75.66631, -454.9523, 37.25719), 149f, questName: "s_f_y_migrant_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_migrant_01", new Vector3(-141.70757, -455.62387, 34.098953), 96f, questName: "s_m_m_migrant_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_omega", new Vector3(-85.62212, -399.71185, 36.9588 ), -56f, questName: "ig_omega", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_hipster_02", new Vector3(-135.42009, -112.696976, 56.157063), -152f, questName: "a_f_y_hipster_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_gurk", new Vector3(-134.85669, -114.70875, 56.349926), 25f, questName: "cs_gurk", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_hipster_01", new Vector3(-556.029, 300.8321, 83.16788), 72f,questName: "a_m_y_hipster_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_y_korean_01", new Vector3(-555.91003, 300.64008, 83.142456 ), 41f, questName: "g_m_y_korean_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_chengsr", new Vector3(-556.8343, 301.3989, 83.18353), -172f, questName: "ig_chengsr", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_jimmydisanto", new Vector3(-379.09442, 220.15878, 83.95134), 179f, questName: "ig_jimmydisanto", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_vagspeak", new Vector3(-379.21362, 217.97987, 83.65949), -1f, questName: "ig_vagspeak", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_terry", new Vector3(-87.66504, 235.2788, 100.56348), -46f, questName: "ig_terry", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_m_o_taphillbilly", new Vector3(-86.724815, 236.94829, 100.56348), -139f, questName: "u_m_o_taphillbilly", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_m_y_tattoo_01", new Vector3(-85.0321, 236.5122, 100.56348), 114f, questName: "u_m_y_tattoo_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_strperf_01", new Vector3(-1307.1971, -1556.6187, 4.641102), 9f, questName: "s_m_m_strperf_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_strperf_01", new Vector3(-1304.593, -1553.9961, 4.31134), 120f, questName: "s_m_m_strperf_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_strperf_01", new Vector3(-1308.0176, -1555.0743, 4.3113422), 14f, questName: "a_m_m_salton_03", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_mime", new Vector3(-1238.6486, -1504.2113, 4.2902093), -57f, questName: "s_m_y_mime", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_maryann", new Vector3(-1236.9951, -1499.5435, 4.3594475), 140f, questName: "ig_maryann", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_ramp_hipster", new Vector3(-1235.826, -1503.5863, 4.352159), 100f, questName: "csb_ramp_hipster", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_runner_01", new Vector3(-1221.0339, -1514.0067, 4.2443185), 93f, questName: "a_m_y_runner_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_chrisformage", new Vector3(-128.1365, -1409.6647, 4.369832), 67f, questName: "cs_chrisformage", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_m_m_jesus_01", new Vector3(-1170.8943, -15170.8937, 4.663622), 130f, questName: "u_m_m_jesus_01", isBlipVisible: false);
              
              PedSystem.Repository.CreateQuest("a_m_y_jetski_01", new Vector3(-1170.8943, -15170.8937, 4.663622), 130f, questName: "a_m_y_jetski_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_skater_01", new Vector3(-1110.6576, -1688.7606, 4.375522), 127f, questName: "a_m_y_skater_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_f_y_baywatch_01", new Vector3(-1542.5625, -1217.907, 1.8718107), 165f, questName: "s_f_y_baywatch_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_beach_02", new Vector3(-1478.32228, -1349.6534, 2.5625129), 122f, questName: "a_f_y_beach_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_m_beach_01", new Vector3(-1521.618, -1240.5365, 2.3650465), -42f, questName: "a_f_m_beach_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_beach_01", new Vector3(-1503.9092, -1288.9095, 2.3594458), 106f, questName: "a_f_y_beach_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_beach_01", new Vector3(-1477.7963, -1350.6866, 2.5617132 ), 151f, questName: "a_m_m_salton_03", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_beach_02", new Vector3(-1460.561,-1376.4357, 2.77737 ), 109f, questName: "a_m_m_beach_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_m_fatcult_01", new Vector3( 1492.0626, -1377.5724, 2.1505024), 112f, questName: "a_f_m_fatcult_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_beach_02", new Vector3(-1830.3512, -830.49304, 5.117562), 176f, questName: "a_f_y_beach_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_m_beach_01", new Vector3(-1814.9127, -837.9151, 6.3383584), 44f, questName: "a_f_m_beach_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_f_y_baywatch_01", new Vector3(-1843.4478, -801.5308, 6.0699615), -16f, questName: "s_f_y_baywatch_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_beach_02", new Vector3(-1884.6891, -766.3012, 4.9572515 ), 24f, questName: "a_m_m_beach_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_drfriedlander", new Vector3(-1641.3367, -1097.63, 13.404464 ), -39f, questName: "cs_drfriedlander", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_y_famdnf_01", new Vector3(-1654.7164, -1096.5288, 13.121691), 48f, questName: "g_m_y_famdnf_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("hc_hacker", new Vector3(-1637.9878, -1121.8873, 13.021938), 80f, questName: "hc_hacker", isBlipVisible: false);
  
              PedSystem.Repository.CreateQuest("s_m_y_grip_01", new Vector3(-1643.6023, -1119.8114, 13.028572 ), -43f, questName: "s_m_y_grip_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_hugh", new Vector3(-1643.4635, -1117.613, 13.029805), 137f, questName: "csb_hugh", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_f_m_miranda", new Vector3(-1642.7357, -1116.8212, 13.029331), 137f, questName: "u_f_m_miranda", isBlipVisible: false);
  
              PedSystem.Repository.CreateQuest("a_m_m_paparazzi_01", new Vector3( -1437.7948, -209.5031, 48.02639 ), 20f, questName: "a_m_m_paparazzi_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_m_m_jewelsec_01", new Vector3(-1381.6742, -585.2174, 30.11637), -34f, questName: "u_m_m_jewelsec_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_paparazzi_01", new Vector3(-1337.0369, -407.85202, 36.47848), 103f, questName: "a_m_m_paparazzi_01", isBlipVisible: false);
  
              PedSystem.Repository.CreateQuest("a_f_y_genhot_01", new Vector3(-726.59, -72.17336, 37.475693), 163f, questName: "a_f_y_genhot_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_y_mexgang_01", new Vector3(5.1889725, -1605.1497, 29.381365), -82f, questName: "g_m_y_mexgang_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_beachvesp_02", new Vector3(4.945608, -1603.8969, 29.29402), -152f, questName: "a_m_y_beachvesp_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_lamardavis", new Vector3(215.81998, -1519.4285, 29.291662), -104f, questName: "ig_lamardavis", isBlipVisible: false);
  
              PedSystem.Repository.CreateQuest("ig_lazlow", new Vector3(217.41907, -1520.0427, 29.291668), 44f, questName: "ig_lazlow", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_m_y_party_01", new Vector3(217.42831, -1518.2742, 29.29166), 132f, questName: "u_m_y_party_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_f_execpa_02", new Vector3(-622.5282, -229.81062, 38.05702), 131f, questName: "mp_f_execpa_02", isBlipVisible: false);
  
              PedSystem.Repository.CreateQuest("cs_debra", new Vector3(-627.4016, -233.3839, 38.057022), -134f, questName: "cs_debra", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_security_01", new Vector3(633.37915, -236.75769, 38.02592), 132f, questName: "s_m_m_security_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_vinewood_02", new Vector3(-643.8222, -226.61403, 37.735764 ), -19f, questName: "a_m_y_vinewood_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_gustavo", new Vector3(-631.4283, -260.0102, 38.680878), 362f, questName: "ig_gustavo", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_patricia_02", new Vector3(-631.6881, -258.89612, 38.659355), -144f, questName: "ig_patricia_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_ccrew_01", new Vector3(-367.77853, 102.81236, 39.543034), -19f, questName: "s_m_m_ccrew_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_clubcust_04", new Vector3(-1545.9663, -440.99182, 35.882008), 1282f, questName: "a_m_y_clubcust_04", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_y_famdnf_01", new Vector3(-1546.7996, -441841, 35.881992), 138f, questName: "g_m_y_famdnf_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_m_soucentmc_01", new Vector3( 397.9121, -354.8956, 46.81525 ), 97f, questName: "a_f_m_soucentmc_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_o_busker_01", new Vector3(376.9302, -337.8432, 46.588306), -123f, questName: "s_m_o_busker_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_stlat_02", new Vector3(561.7877, 83.19379, 95.63555), -161f, questName: "a_m_m_stlat_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_c_chop", new Vector3(562.73975, 80.95526, 95.00341), -162f, questName: "a_c_chop", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_m_famdd_01", new Vector3(-351.4603, -49.473484, 49.042576), -161f, questName: "mp_m_famdd_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_y_famdnf_01", new Vector3(-353.889, -48.87826, 49.039627), 70f, questName: "g_m_y_famdnf_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_bankman", new Vector3(-351.22778, -51.40374, 49.036472), 26f, questName: "ig_bankman", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_stlat_02", new Vector3(-354.88434, -46.356045, 49.036327), -113f, questName: "a_m_m_stlat_02", isBlipVisible: false);
  
              PedSystem.Repository.CreateQuest("a_f_y_soucent_01", new Vector3(-356.43018, -47.95753, 49.036434), -48f, questName: "a_f_y_soucent_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_patricia_02 ", new Vector3(-1023.6058, -2740.7065, 20.16929), -16f, questName: "ig_patricia_02 ", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_gustavo", new Vector3(1022.85077, -2739.3782, 20.169287), 154f, questName: "ig_gustavo", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_paparazzi_01", new Vector3(-1048.6816, -2724.278, 20.169302), -53f, questName: "a_m_m_paparazzi_01", isBlipVisible: false);
  
              PedSystem.Repository.CreateQuest("a_m_y_runner_01", new Vector3(-1039.2896, -2729.4116, 20.2102), -48f, questName: "a_m_y_runner_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_armymech_01", new Vector3(674.5899, 584.8564, 130.46143), 36f, questName: "s_m_y_armymech_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_gaffer_01", new Vector3(699.7758, 573.45557, 130.46126), -83f, questName: "s_m_m_gaffer_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_waiter_01", new Vector3(-2057.7092, -1023.4596, 11.907534), -126f, questName: "s_m_y_waiter_01", isBlipVisible: false);
  
              PedSystem.Repository.CreateQuest("s_m_y_barman_01", new Vector3(-2052.782, -1025.7571, 11.907589 ), -36f, questName: "s_m_y_barman_01", isBlipVisible: false);
  
              PedSystem.Repository.CreateQuest("csb_money", new Vector3(-2045.1505, -1024.3041, 11.907634), -31f, questName: "csb_money", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_miguelmadrazo", new Vector3(-2063.7021, -1018.10114, 11.937146), -31f, questName: "ig_miguelmadrazo", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_bevhills_01", new Vector3(-2974.786, 377.62698, 15.008176 ), -147f, questName: "a_m_m_bevhills_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_car3guy2", new Vector3(-3082.5618, 764.52924, 31.267597), -54f, questName: "ig_car3guy2", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_m_exarmy_01", new Vector3(-2692.8848, 2724.6953, 1.0130664), 86f, questName: "mp_m_exarmy_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_m_m_filmdirector", new Vector3(-2733.109, 2730.914, 0.8294439), -141f, questName: "u_m_m_filmdirector", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_floyd", new Vector3(-2342.3418, 4120.4087, 35.889786), 77f, questName: "ig_floyd", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_gardener_01", new Vector3(-2355.8303, 3987.6606, 26.147955), -166f, questName: "s_m_m_gardener_01", isBlipVisible: false);
  
              PedSystem.Repository.CreateQuest("g_m_y_salvagoon_03", new Vector3(-768.0155, 5597.0537, 33.621284), 72f, questName: "g_m_y_salvagoon_03", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_screen_writer", new Vector3(-770.7401, 5597.7153, 33.60426), -101f, questName: "ig_screen_writer", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_ammucity_01", new Vector3(-290.7202, 6263.998, 31.49338), 75f, questName: "s_m_y_ammucity_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_ramp_hic", new Vector3(-291.4485, 6264.192, 31.493399), -140f, questName: "ig_ramp_hic", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_f_y_migrant_01", new Vector3(324.05774, 6617.752, 28.675766), 56f, questName: "s_f_y_migrant_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_migrant_01", new Vector3(334.50262, 6600.9316, 28.746479), 56f, questName: "s_m_m_migrant_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_omega", new Vector3(344.8239, 6597.4624, 28.88244), -2f, questName: "ig_omega", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_hasjew_01", new Vector3(165.76013, 6628.393, 31.698046), 42f, questName: "a_m_m_hasjew_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_screen_writer", new Vector3(164.63829, 6629.623, 31.655027), -135f, questName: "ig_screen_writer", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_f_deadhooker", new Vector3(1555.4127, 6446.682, 23.970549), -136f, questName: "mp_f_deadhooker", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_isldj_00", new Vector3(1587.4791, 6449.5435, 25.284512), -26f, questName: "csb_isldj_00", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_gaffer_01", new Vector3(2592.3193, 5066.0864, 44.919292), -163f, questName: "s_m_m_gaffer_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_m_soucentmc_01", new Vector3(2742.4326, 4409.125, 48.34615), 11f, questName: "a_f_m_soucentmc_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_stlat_01", new Vector3(2683.8909, 3291.33, 55.24063), -91f, questName: "a_m_y_stlat_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_bevhills_01", new Vector3(2686.0845, 3288.779, 55.409157), -57f, questName: "a_m_m_bevhills_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_m_o_taphillbilly", new Vector3(1651.5739, 4868.334, 41.954353), 136f, questName: "u_m_o_taphillbilly", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_strvend_01", new Vector3(1651.0502, 4867.52, 41.920258), -75f, questName: "s_m_m_strvend_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_tramp_01", new Vector3(1655.2891, 4893.791, 42.090813), 176f, questName: "a_m_m_tramp_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_o_tramp_01", new Vector3(1654.1537, 4892.8613, 42.06915), -102f, questName: "a_m_o_tramp_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_isldj_00", new Vector3(1699.6775, 4791.589, 41.92238), 179f, questName: "csb_isldj_00", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_m_o_tramp_01", new Vector3(1536.5464, 3781.6394, 34.446804), -162f, questName: "u_m_o_tramp_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_isldj_00", new Vector3(1999.7019, 3777.4656, 32.180813), 33f, questName: "csb_isldj_00", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_beach_04", new Vector3(1956.4562, 3836.0793, 32.17849), -148f, questName: "a_m_y_beach_04", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_jio", new Vector3(1957.1659, 3835.3167, 32.183865), 26f, questName: "csb_jio", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_isldj_00", new Vector3(1047.6267, 2663.014, 39.551144), 178f, questName: "csb_isldj_00", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_mjo", new Vector3(1411.2577, 3601.0781, 34.97784), -176f, questName: "csb_mjo", isBlipVisible: false);
              
              PedSystem.Repository.CreateQuest("a_m_y_eastsa_02", new Vector3(-531.9538, -206.8865, 37.649254), 166f, questName: "a_m_y_eastsa_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_m_fatwhite_01", new Vector3(-532.75134, -209.40392, 37.649105), -17f, questName: "a_f_m_fatwhite_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_f_m_fembarber", new Vector3(-527.2538, -206.85866, 37.6493075), 76f, questName: "s_f_m_fembarber", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_fos_rep", new Vector3(-529.73553, -206.23575, 37.648987), -104f, questName: "csb_fos_rep", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_hipster_03", new Vector3(-550.45953, -222.14845, 37.64906), -100f, questName: "a_f_y_hipster_03", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_hipster_04", new Vector3(-548.1665, -222.65167, 37.648724), 79f, questName: "a_f_y_hipster_04", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_hugh", new Vector3(-509.6403, -224.80261, 36.65281), 179f, questName: "csb_hugh", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_jay_norris", new Vector3(-509.72897, -226.87096, 36.58674), -2f, questName: "ig_jay_norris", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_jimmyboston", new Vector3(-528.7146, -265.91412, 35.41853), -147f, questName: "cs_jimmyboston", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_o_ktown_01", new Vector3(-531.17755, -266.95956, 35.396545), -142f, questName: "a_m_o_ktown_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_lazlow", new Vector3(-544.01184, -157.84657, 38.523727), -17f, questName: "ig_lazlow", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_mrs_thornhill", new Vector3(-388.8744, -321.21988, 33.1814), -132f, questName: "cs_mrs_thornhill", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_f_y_movprem_01", new Vector3(-392.0245, -318.95435, 33.324825), -126f, questName: "s_f_y_movprem_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_m_m_filmdirector", new Vector3(-534.688, -253.911, 35.7653), 136f, questName: "u_m_m_filmdirector", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_f_y_hooker_01", new Vector3(-539.61115, -252.70259, 35.974834), -96f, questName: "s_f_y_hooker_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_indian_01", new Vector3(-540.55664, -255.7073, 35.89807), -69f, questName: "a_f_y_indian_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_janet", new Vector3(-537.98926, -258.332, 35.765587), -33f, questName: "ig_janet", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_ktown_01", new Vector3(-540.44086, -259.06885, 35.78435), -49f, questName: "a_m_y_ktown_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_m_ktown_01", new Vector3(-554.43317, -250.85815, 36.66837), -152f, questName: "a_f_m_ktown_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_ktown_02", new Vector3(-553.21173, -253.12708, 36.61987), 29f, questName: "a_m_y_ktown_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_lifeinvad_01", new Vector3(-552.02325, -251.35283, 36.57159), 121f, questName: "ig_lifeinvad_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_y_lost_03", new Vector3(-547.0195, -334.32516, 35.17038), -61f, questName: "g_m_y_lost_03", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_marine_02", new Vector3(-1260.2373, -588.37823, 29.302237), 76f, questName: "s_m_m_marine_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_martinmadrazo", new Vector3(-1261.5283, -586.9353, 29.302069), 174f, questName: "cs_martinmadrazo", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_marnie", new Vector3(-1262.7672, -589.12067, 29.302069), -84f, questName: "ig_marnie", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_milton", new Vector3(-1298.1271, -549.84216, 31.712091), 36f, questName: "ig_milton", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("player_zero", new Vector3(-1299.3677, -548.18494, 31.712091), -141f, questName: "player_zero", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_paper", new Vector3(-1300.3888, -598.4392, 29.302032), 45f, questName: "cs_paper", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_solomon", new Vector3(-1301.8248, -596.7037, 29.302032), -135f, questName: "cs_solomon", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_f_execpa_01", new Vector3(-1302.7831, -598.6383, 29.302065), -36f, questName: "mp_f_execpa_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_airworker", new Vector3(-1278.1492, -586.73834, 42.074787), 36f, questName: "s_m_y_airworker", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_f_y_airhostess_01", new Vector3(-1279.8893, -584.3497, 42.074787), -143f, questName: "s_f_y_airhostess_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_m_m_aldinapoli", new Vector3(-1227.7119, -560.1341, 28.176504), 132f, questName: "u_m_m_aldinapoli", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_lathandy_01", new Vector3(-495.73785, -975.99457, 23.549929), 176f, questName: "s_m_m_gardener_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_gardener_01", new Vector3(-468.97888, -963.36945, 38.88125), 93f, questName: "s_m_m_gardener_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_eastsa_02", new Vector3(-626.8821, 291.58917, 81.83558), 69f, questName: "a_m_m_eastsa_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_m_exarmy_01", new Vector3(-628.43933, 292.14352, 81.81409), -106f, questName: "mp_m_exarmy_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_hao", new Vector3(47.084854, 326.6607, 112.103004), 166f, questName: "ig_hao", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_gurk", new Vector3(46.14116, 323.8547, 111.9371), -19f, questName: "cs_gurk", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_hasjew_01", new Vector3(86.99133, 270.61902, 110.15293), 58f, questName: "a_m_m_hasjew_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_hasjew_01", new Vector3(85.17594, 271.63306, 110.18332), -120f, questName: "a_m_y_hasjew_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_f_y_jewelass_01", new Vector3(87.331055, 272.58594, 110.20478), 136f, questName: "u_f_y_jewelass_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_jetski_01", new Vector3(-1904.2472, -710.7931, 8.83258), 126f, questName: "a_m_y_jetski_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_f_y_baywatch_01", new Vector3(-1796.9269, -855.4657, 9.199987), 109f, questName: "s_f_y_baywatch_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_baywatch_01", new Vector3(-1843.0288, -1256.501, 8.815775), 142f, questName: "s_m_y_baywatch_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_o_beach_01", new Vector3(-1849.8666, -1250.7561, 8.615774), 138f, questName: "a_m_o_beach_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_bevhills_01", new Vector3(-1834.405, -1238.5979, 13.017223), -150f, questName: "a_m_y_bevhills_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_beachvesp_01", new Vector3(-1833.3358, -1240.9021, 13.017258), 23f, questName: "a_m_y_beachvesp_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_beachvesp_02", new Vector3(-1832.6329, -1239.1996, 13.017272), 166f, questName: "a_m_y_beachvesp_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_beach_01", new Vector3(-1799.6205, -884.08453, 4.8132496), 156f, questName: "a_m_y_beach_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_m_beach_01", new Vector3(-1810.6022, -888.7593, 3.6923802), 145f, questName: "a_f_m_beach_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_beach_01", new Vector3(-1896.9652, -748.4155, 4.9459844), 132f, questName: "a_f_y_beach_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_beach_02", new Vector3(-1896.2456, -749.40204, 4.9518323), 131f, questName: "a_m_m_beach_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_car3guy1", new Vector3(-379.9438, 6031.669, 31.49885), -123f, questName: "csb_car3guy1", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_ranger_01", new Vector3(-378.8688, 6030.922, 31.49915), 55f, questName: "s_m_y_ranger_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_m_chicold_01", new Vector3(-442.26282, 6144.092, 31.47832), 18f, questName: "g_m_m_chicold_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_chin_goon", new Vector3(-442.45758, 6145.0117, 31.478355), 166f, questName: "csb_chin_goon", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_sheriff_01", new Vector3(-486.241, 5998.706, 31.309017), -131f, questName: "s_m_y_sheriff_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_skater_02", new Vector3(-433.21497, 6010.847, 31.490093), 46f, questName: "a_m_y_skater_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_f_y_shop_low", new Vector3(-434.55295, 6012.275, 31.490093), -137f, questName: "s_f_y_shop_low", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_f_y_sheriff_01", new Vector3(-367.96945, 6099.456, 35.4397), -134f, questName: "s_f_y_sheriff_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_robber_01", new Vector3(-285.84836, 6022.41, 31.473577), 136f, questName: "s_m_y_robber_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_stevehains", new Vector3(-289.48972, 6026.283, 31.484575), 134f, questName: "ig_stevehains", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_salton_03", new Vector3(-298.31366, 6152.441, 32.23108), -44f, questName: "a_m_m_salton_03", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_runner_02", new Vector3(-67.388054, 6265.286, 31.090158), -64f, questName: "a_m_y_runner_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_soucent_03", new Vector3(100.793564, 6367.4956, 31.375854), 122f, questName: "a_f_y_soucent_03", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_socenlat_01", new Vector3(99.27623, 6366.263, 31.375854), -52f, questName: "a_m_m_socenlat_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_y_strpunk_01", new Vector3(16.505484, 3719.073, 39.647568), -161f, questName: "g_m_y_strpunk_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_taocheng", new Vector3(16.625568, 3718.373, 39.63906), -27f, questName: "ig_taocheng", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_stwhi_01", new Vector3(56.051704, 3724.043, 39.726185), -72f, questName: "a_m_y_stwhi_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_strvend_01", new Vector3(57.385067, 3724.4211, 39.72685), 107f, questName: "s_m_y_strvend_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_stretch", new Vector3(153.2551, 3716.3386, 32.241066), -74f, questName: "ig_stretch", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_m_tourist_01", new Vector3(461.50626, 3566.098, 33.23861), -100f, questName: "a_f_m_tourist_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_vinewood_02", new Vector3(463.7202, 3565.6453, 33.23861), 76f, questName: "a_f_y_vinewood_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_vinewood_03", new Vector3(913.359, 3643.4119, 32.661163), 4f, questName: "a_m_y_vinewood_03", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_isldj_03", new Vector3(1384.1721, 3617.655, 38.920963), 21f, questName: "csb_isldj_03", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_beach_04", new Vector3(1429.6128, 3671.6865, 39.728413), 26f, questName: "a_m_y_beach_04", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_yoga_01", new Vector3(1432.5676, 3668.5479, 39.728413), -69f, questName: "a_m_y_yoga_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_undercover", new Vector3(1422.3624, 3666.0085, 39.728436), -71f, questName: "csb_undercover", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_soucent_03", new Vector3(1424.2651, 3666.67, 39.728436), 108f, questName: "a_m_m_soucent_03", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_soucent_01", new Vector3(1656.2114, 3803.337, 38.666866), -145f, questName: "a_f_y_soucent_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_o_soucent_02", new Vector3(1976.9113, 3814.3005, 33.427315), -152f, questName: "a_f_o_soucent_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_ramp_hic", new Vector3(2227.5046, 3896.3665, 31.194635), 16f, questName: "ig_ramp_hic", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_farmer_01", new Vector3(2461.855, 4844.297, 36.61788), -132f, questName: "a_m_m_farmer_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_cntrybar_01", new Vector3(2472.731, 4838.107, 35.744385), -131f, questName: "s_m_m_cntrybar_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_cyclist_01", new Vector3(2484.9834, 4841.072, 35.612736), 45f, questName: "a_m_y_cyclist_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_f_y_migrant_01", new Vector3(2488.7183, 4861.9614, 37.174007), 45f, questName: "s_f_y_migrant_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_old_man2", new Vector3(2291.9814, 5065.988, 46.22202), 42f, questName: "ig_old_man2", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_runner_01", new Vector3(2283.0042, 5075.1553, 46.741405), 42f, questName: "a_f_y_runner_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_salton_02", new Vector3(2256.2292, 5069.431, 45.892185), 46f, questName: "a_m_m_salton_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_sunbathe_01", new Vector3(2253.2983, 5065.827, 45.632523), -139f, questName: "a_m_y_sunbathe_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_trucker_01", new Vector3(2163.0215, 5108.358, 46.962147), -173f, questName: "s_m_m_trucker_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_m_o_taphillbilly", new Vector3(2004.7324, 4978.261, 41.577682), 41f, questName: "u_m_o_taphillbilly", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_o_soucent_02", new Vector3(2455.6099, 4965.39, 46.571552), -134f, questName: "a_m_o_soucent_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_stwhi_02", new Vector3(3345.0964, 5142.567, 18.405684), -144f, questName: "a_m_y_stwhi_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_talina", new Vector3(3857.165, 4423.7593, 1.8353842), 17f, questName: "ig_talina", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_terry", new Vector3(3426.9692, 3762.7617, 30.832048), 26f, questName: "ig_terry", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_terry", new Vector3(3430.8826, 3763.7854, 30.832048), -153f, questName: "cs_terry", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_tom", new Vector3(2545.1619, 2608.1028, 37.9563375), -163f, questName: "cs_tom", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_ups_01", new Vector3(2519.3772, 2633.6685, 37.94489), -174f, questName: "s_m_m_ups_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_hillbilly_01", new Vector3(2525.6746, 2621.1252, 37.94488), -153f, questName: "a_m_m_hillbilly_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_eastsa_02", new Vector3(2686.865, 1593.8192, 32.50677), 6f, questName: "a_f_y_eastsa_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_taocheng", new Vector3(2835.4885, 1562.2747, 24.728706), 164f, questName: "cs_taocheng", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_hipster_03", new Vector3(2784.183, 1236.1409, 2.7777066), -95f, questName: "a_m_y_hipster_03", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_m_chigoon_02", new Vector3(1686.8112, -1671.8069, 117.220795), -80f, questName: "g_m_m_chigoon_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_bevhills_04", new Vector3(1712.7992, -1721.3615, 112.50236), 60f, questName: "a_f_y_bevhills_04", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_o_busker_01", new Vector3(1711.2834, -1720.6096, 112.46224), -116f, questName: "s_m_o_busker_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_car3guy2", new Vector3(1016.47876, -2517.4412, 28.30368), 3f, questName: "csb_car3guy2", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_ballasog", new Vector3(1015.59174, -2514.5369, 28.303682), 178f, questName: "csb_ballasog", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_m_claude_01", new Vector3(786.23553, -2214.6292, 29.455584), -6f, questName: "mp_m_claude_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_f_y_comjane", new Vector3(786.5007, -2212.577, 29.459444), 173f, questName: "u_f_y_comjane", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_tourist_02", new Vector3(-88.23238, -438.54388, 35.993977), -10f, questName: "a_f_y_tourist_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_tracydisanto", new Vector3(-87.801414, -436.23615, 36.078083), -146f, questName: "ig_tracydisanto", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_garbage", new Vector3(21.005016, -397.48956, 45.50067), -10f, questName: "s_m_y_garbage", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_genstreet_01", new Vector3(15.691645, -415.24454, 45.500675), 146f, questName: "a_m_y_genstreet_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_grove_str_dlr", new Vector3(102.8638, -1389.0305, 29.291555), 65f, questName: "csb_grove_str_dlr", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_o_genstreet_01", new Vector3(100.6658, -1388.2377, 29.291555), -111f, questName: "s_m_m_gardener_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_groom", new Vector3(-801.8569, -96.40084, 37.614834), -166f, questName: "csb_groom", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_genhot_01", new Vector3(-799.3887, -97.38822, 37.626686), 95f, questName: "a_f_y_genhot_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_m_y_hippie_01", new Vector3(-801.0205, -98.8494, 37.581154), 1f, questName: "u_m_y_hippie_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_josh", new Vector3(-862.3251, -65.72027, 37.85317), -153f, questName: "ig_josh", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_m_ktown_02", new Vector3(-861.308, -67.41755, 37.853104), 30f, questName: "a_f_m_ktown_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_movpremf_01", new Vector3(-293.53284, -426.13535, 30.237543), -31f, questName: "cs_movpremf_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_nigel", new Vector3(-292.233, -423.74835, 30.237543), 149f, questName: "ig_nigel", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_paparazzi_01", new Vector3(-294.1261, -424.31406, 30.237543), -106f, questName: "a_m_m_paparazzi_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_reporter", new Vector3(-1076.5802, -261.37323, 37.807636), 114f, questName: "csb_reporter", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_englishdave_02", new Vector3(-1078.5468, -262.23523, 37.79968), -67f, questName: "ig_englishdave_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_m_armgoon_01", new Vector3(-934.2093, -212.11493, 38.071655), 170f, questName: "g_m_m_armgoon_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_amandatownley", new Vector3(-934.6105, -214.98436, 38.176132), -5f, questName: "ig_amandatownley", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_m_forgery_01", new Vector3(393.27084, -229.91699, 55.826366), 73f, questName: "mp_m_forgery_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_m_o_tramp_01", new Vector3(382.7329, -245.83194, 54.294994), 105f, questName: "u_m_o_tramp_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_runner_01", new Vector3(-106.754036, -432.9191, 36.061237), 14f, questName: "a_m_y_runner_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_musclbeac_02", new Vector3(107.59873, -429.40982, 36.063366), -167f, questName: "a_m_y_musclbeac_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_chef", new Vector3(152.82918, -1469.6074, 29.14168), -26f, questName: "ig_chef", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_fatlatin_01", new Vector3(-571.38916, -29.43992, 43.515648), 160f, questName: "a_m_m_fatlatin_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_m_fatbla_01", new Vector3(-572.0538, -32.073322, 43.300175), -17f, questName: "a_f_m_fatbla_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_f_y_families_01", new Vector3(-517.3461, -66.083916, 40.836334), -22f, questName: "g_f_y_families_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_anton", new Vector3(-1657.0737, 82.029396, 63.739304), -122f, questName: "csb_anton", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_ashley", new Vector3(-1655.29, 80.90253, 63.671368), 58f, questName: "cs_ashley", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_y_ballaeast_01", new Vector3(-1612.6636, 165.85732, 60.0769), 25f, questName: "g_m_y_ballaeast_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_f_y_bartender_01", new Vector3(-1613.7714, 168.0226, 60.114216), -153f, questName: "s_f_y_bartender_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_beverly", new Vector3(-1422.135, 224.95505, 59.534977), -147f, questName: "ig_beverly", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_bevhills_01", new Vector3(-1420.996, 223.21568, 59.479122), 36f, questName: "a_f_y_bevhills_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_m_boatstaff_01", new Vector3(-1423.8385, 223.21524, 59.463524), -49f, questName: "mp_m_boatstaff_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_business_01", new Vector3(307.58472, -510.2991, 43.261196), 155f, questName: "a_f_y_business_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_business_01", new Vector3(306.7837, -512.2036, 43.257492), -20f, questName: "a_m_m_business_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_m_y_chip", new Vector3(227.79533, -576.44586, 43.872852), -124f, questName: "u_m_y_chip", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_dale", new Vector3(229.5713, -577.7785, 43.872852), 55f, questName: "cs_dale", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_paramedic_01", new Vector3(249.20322, -560.64996, 43.276463), 74f, questName: "s_m_m_paramedic_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_m_y_chip", new Vector3(227.79533, -576.44586, 43.872852), -124f, questName: "u_m_y_chip", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_dale", new Vector3(229.5713, -577.7785, 43.872852), 55f, questName: "cs_dale", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_paramedic_01", new Vector3(249.20322, -560.64996, 43.276463), 74f, questName: "s_m_m_paramedic_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_f_y_scrubs_01", new Vector3(246.81767, -559.89874, 43.2788), -111f, questName: "s_f_y_scrubs_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_jackie", new Vector3(306.2465, 180.9515, 103.98713), 84f, questName: "ig_jackie", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_miguelmadrazo", new Vector3(303.97495, 181.56172, 104.040924), -112f, questName: "ig_miguelmadrazo", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_highsec_04", new Vector3(282.4667, 188.62526, 104.50531), 97f, questName: "s_m_m_highsec_04", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_isldj_00", new Vector3(283.75974, 187.75511, 104.480255), -91f, questName: "csb_isldj_00", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_mjo", new Vector3(282.66022, 187.15373, 104.495605), -77f, questName: "ig_mjo", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_tenniscoach", new Vector3(473.1014, -106.11972, 63.157883), -18f, questName: "cs_tenniscoach", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_vagspeak", new Vector3(-135.1294, 1059.0337, 229.46187), -57f, questName: "ig_vagspeak", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_m_g_vagfun_01", new Vector3(-136.38538, 1060.218, 229.54732), -89f, questName: "mp_m_g_vagfun_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_m_weapexp_01", new Vector3(-132.90973, 1060.1964, 229.28949), 117f, questName: "mp_m_weapexp_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_devinsec_01", new Vector3(-595.17615, 21.943, 43.30214), -90f, questName: "s_m_y_devinsec_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_downtown_01", new Vector3(-593.0486, 21.943842, 43.42567), 92f, questName: "a_m_y_downtown_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_milton", new Vector3(929.14386, 55.106667, 81.096115), 142f, questName: "cs_milton", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_f_o_moviestar", new Vector3(927.3289, 52.900253, 81.096115), -40f, questName: "u_f_o_moviestar", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_highsec_02", new Vector3(927.8944, 36.084507, 81.095795), -13f, questName: "s_m_m_highsec_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_fieldworker_01", new Vector3(889.196, 6.1735764, 78.902855), 58f, questName: "s_m_m_fieldworker_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_isldj_04", new Vector3(886.9147, 7.2470493, 78.90321), -121f, questName: "ig_isldj_04", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_clubcust_04", new Vector3(887.1309, -1.5501292, 78.76493), 162f, questName: "a_f_y_clubcust_04", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_skater_01", new Vector3(405.83588, -1612.5979, 29.291546), -38f, questName: "a_m_m_skater_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_soucent_01", new Vector3(407.72235, -1613.4849, 29.291533), 62f, questName: "a_m_m_soucent_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_f_y_poppymich", new Vector3(406.39655, -1668.308, 29.287334), -64f, questName: "u_f_y_poppymich", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_y_mexgoon_02", new Vector3(408.65033, -1667.2733, 29.282953), 113f, questName: "g_m_y_mexgoon_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_porndudes", new Vector3(-1512.6487, -1482.5742, 4.709971), -115f, questName: "csb_porndudes", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_f_deadhooker", new Vector3(-1512.7838, -1494.5887, 4.2609615), 124f, questName: "mp_f_deadhooker", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_isldj_04", new Vector3(-1507.2302, -1504.7749, 3.92179), -39f, questName: "csb_isldj_04", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_y_lost_01", new Vector3(31.531927, -1394.4601, 29.333052), 23f, questName: "g_m_y_lost_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_y_mexgoon_03", new Vector3(30.618042, -1389.5458, 29.30986), -175f, questName: "g_m_y_mexgoon_03", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_mexlabor_01", new Vector3(-697.81067, -924.491, 19.013891), 148f, questName: "a_m_m_mexlabor_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_salton_01", new Vector3(1360.8215, 3595.6782, 34.9063), -157f, questName: "a_m_y_salton_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_floyd", new Vector3(-975.0091, 389.06137, 74.75022), -26f, questName: "cs_floyd", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_stevehains", new Vector3(-974.5217, 392.0036, 74.78856), 162f, questName: "cs_stevehains", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_stripper_01", new Vector3(-1376.2372, -626.3977, 30.819578), -155f, questName: "csb_stripper_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_tonya", new Vector3(-1375.4229, -628.0889, 30.81959), 32f, questName: "ig_tonya", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_f_y_hooker_02", new Vector3(-1383.4548, -612.2369, 31.757858), 118f, questName: "s_f_y_hooker_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_lifeinvad_01", new Vector3(-1380.0496, -617.6269, 31.75785), 120f, questName: "cs_lifeinvad_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_maryann", new Vector3(-1383.3131, -617.95496, 30.81959), 125f, questName: "ig_maryann", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_michelle", new Vector3(-2030.0334, -465.8425, 11.6039715), -126f, questName: "ig_michelle", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_mexthug_01", new Vector3(-2021.1794, -455.30646, 11.502577), -38f, questName: "a_m_y_mexthug_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_f_counterfeit_01", new Vector3(-1659.1781, -784.56494, 10.20246), -146f, questName: "mp_f_counterfeit_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_f_weed_01", new Vector3(-1682.662, -767.8269, 10.189976), 134f, questName: "mp_f_weed_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_vinewood_03", new Vector3(-1656.8059, -980.88367, 8.167339), 48f, questName: "a_f_y_vinewood_03", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_f_y_vagos_01", new Vector3(-1660.371, -980.33734, 7.338462), -103f, questName: "g_f_y_vagos_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_m_m_willyfist", new Vector3(-1658.2124, -980.69244, 7.332384), 79f, questName: "u_m_m_willyfist", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_ramp_marine", new Vector3(-2380.2395, 3320.5183, 33.044643), 60f, questName: "csb_ramp_marine", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_xmech_02", new Vector3(-1802.2788, 3088.8132, 32.841785), 40f, questName: "s_m_y_xmech_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_pilot_01", new Vector3(-1803.2012, 3089.727, 32.841793), -146f, questName: "s_m_y_pilot_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_ramp_mex", new Vector3(-2007.7566, 3067.8757, 32.810276), -119f, questName: "csb_ramp_mex", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_m_waremech_01", new Vector3(-1855.8115, 3088.7935, 32.81023), -20f, questName: "mp_m_waremech_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_valet_01", new Vector3(-212.76343, -1027.7328, 30.140703), 155f, questName: "s_m_y_valet_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_zimbor", new Vector3(-213.89381, -1030.312, 30.140411), -21f, questName: "cs_zimbor", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_m_execpa_01", new Vector3(-211.36555, -1023.029, 30.140818), 69f, questName: "mp_m_execpa_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_agent", new Vector3(-112.81429, -599.9258, 36.280704), 159f, questName: "ig_agent", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_f_execpa_02", new Vector3(-113.77422, -601.82434, 36.280704), -24f, questName: "mp_f_execpa_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_o_beach_02", new Vector3(-820.60425, -114.73502, 37.582344), 27f, questName: "a_m_o_beach_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_jio", new Vector3(-821.7464, -112.624176, 37.5821), -152f, questName: "csb_jio", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_patricia_02", new Vector3(-1770.8867, 116.8128, 68.783295), -142f, questName: "ig_patricia_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_m_cartelguards_02", new Vector3(-1769.5697, 115.33751, 68.78072), 38f, questName: "g_m_m_cartelguards_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_vinewood_01", new Vector3(-3335.3252, 970.83014, 8.291516), -94f, questName: "a_m_y_vinewood_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_m_tramp_01", new Vector3(-3332.9448, 970.85333, 8.291516), 90f, questName: "a_f_m_tramp_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_soucent_01", new Vector3(181.98192, -734.6464, 33.749077), 160f, questName: "a_m_y_soucent_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_o_soucent_01", new Vector3(180.22467, -736.1497, 33.704006), -111f, questName: "a_m_o_soucent_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_soucent_02", new Vector3(181.8855, -738.2628, 33.596256), 20f, questName: "a_f_y_soucent_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_y_armgoon_02", new Vector3(-33.35361, -765.71045, 44.28329), 144f, questName: "g_m_y_armgoon_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_busicas_01", new Vector3(-34.58239, -767.5183, 44.26624), -32f, questName: "a_m_y_busicas_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_m_chiboss_01", new Vector3(-35.252853, -765.5122, 44.274574), -119f, questName: "g_m_m_chiboss_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_chef2", new Vector3(53.340904, -1001.0676, 29.357405), 24f, questName: "csb_chef2", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_tylerdix", new Vector3(-928.3731, -750.5542, 19.822252), -105f, questName: "ig_tylerdix", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_tourist_01", new Vector3(-924.8858, -746.315, 19.867884), 113f, questName: "a_f_y_tourist_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_tennis_01", new Vector3(-926.7953, -747.0847, 19.859556), -67f, questName: "a_f_y_tennis_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_wade", new Vector3(-889.6149, -853.5374, 20.566114), -73f, questName: "cs_wade", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_epsilon_01", new Vector3(-786.47485, -946.1147, 16.623915), -113f, questName: "a_m_y_epsilon_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_c_pug", new Vector3(-785.15063, -946.8193, 15.93922), 63f, questName: "a_c_pug", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_f_o_prolhost_01", new Vector3(-787.6224, -937.97015, 17.467697), -163f, questName: "u_f_o_prolhost_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_beverly", new Vector3(-653.5877, -934.0927, 22.492641), -3f, questName: "ig_beverly", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_bride", new Vector3(-653.5498, -931.87225, 22.627737), 178f, questName: "csb_bride", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_f_forgery_01", new Vector3(-488.0462, -750.9928, 32.14924), 52f, questName: "mp_f_forgery_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_oldrichguy", new Vector3(-490.26486, -749.3958, 32.197838), -125f, questName: "ig_oldrichguy", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_juanstrickler", new Vector3(-490.18726, -751.2669, 32.149933), -28f, questName: "csb_juanstrickler", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_vinewood_02", new Vector3(-496.34592, -743.00574, 33.21599), -98f, questName: "a_m_y_vinewood_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_amandatownley", new Vector3(-605.68304, -611.8486, 34.67582), -4f, questName: "cs_amandatownley", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_bestmen", new Vector3(-605.5736, -609.77716, 34.67582), 175f, questName: "ig_bestmen", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_debra", new Vector3(-734.0442, -742.0746, 27.3965), 89f, questName: "cs_debra", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_davenorton", new Vector3(-736.9713, -746.30994, 27.122301), 97f, questName: "cs_davenorton", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_m_eastsa_01", new Vector3(-735.8995, -754.4803, 26.620096), 145f, questName: "a_f_m_eastsa_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_dom", new Vector3(-737.0703, -755.9155, 26.5332), -39f, questName: "cs_dom", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_drfriedlander", new Vector3(-609.82227, -937.6525, 23.859465), 94f, questName: "cs_drfriedlander", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_f_y_shop_mid", new Vector3(944.25134, 14.657372, 116.16427), -117f, questName: "s_f_y_shop_mid", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_shop_mask", new Vector3(945.6955, 13.984481, 116.16414), 58f, questName: "s_m_y_shop_mask", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_iSLdj_02", new Vector3(940.38135, 7.1980076, 116.16415), -84f, questName: "csb_iSLdj_02", isBlipVisible: false);
             // PedSystem.Repository.CreateQuest("ig_gustavo", new Vector3(939.2704, 12.30669, 116.16415), -54f, questName: "ig_gustavo", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_kaylee", new Vector3(940.62915, 14.339485, 116.16415), 170f, questName: "ig_kaylee", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_f_y_hooker_03", new Vector3(127.5136, -1283.9282, 29.278578), -56f, questName: "s_f_y_hooker_03", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_hippy_01", new Vector3(128.94493, -1283.1321, 29.272997), 119f, questName: "a_m_y_hippy_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_f_y_lost_01", new Vector3(110.593, -1290.938, 28.26094), 27f, questName: "g_f_y_lost_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_beach_02", new Vector3(106.71639, -1287.913, 28.260942), -138f, questName: "a_f_y_beach_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_y_vinewood_01", new Vector3(119.7107, -1285.5454, 28.274803), 141f, questName: "a_f_y_vinewood_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_m_y_burgerdrug_01", new Vector3(-560.374, 285.19415, 85.176384), 89f, questName: "u_m_y_burgerdrug_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_f_m_maid_01", new Vector3(-561.7639, 286.783, 82.176506), -96f, questName: "s_f_m_maid_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_f_y_hotposh_01", new Vector3(-552.867, 286.72098, 82.1763), 100f, questName: "u_f_y_hotposh_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_fabien", new Vector3(-553.33124, 281.57117, 82.1763), 56f, questName: "ig_fabien", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_gay_01", new Vector3(-555.5472, 287.37225, 82.1763), -148f, questName: "a_m_y_gay_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_chengsr", new Vector3(578.3765, -1254.2994, 9.805184), 37f, questName: "ig_chengsr", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_f_m_trampbeac_01", new Vector3(577.7039, -1253.056, 9.8088665), -144f, questName: "a_f_m_trampbeac_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_y_cop_01", new Vector3(478.6745, -968.0655, 27.661184), -104f, questName: "s_m_y_cop_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_m_m_fibsec_01", new Vector3(481.0197, -968.5562, 27.630657), 78f, questName: "s_m_m_fibsec_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("s_f_y_beachbarstaff_01", new Vector3(389.6823, -996.37445, 29.41767), -50f, questName: "s_f_y_beachbarstaff_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_f_chbar_01", new Vector3(389.2445, -994.05475, 29.417627), -86f, questName: "mp_f_chbar_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("mp_m_securoguard_01", new Vector3(391.3896, -994.5897, 29.41673), 106f, questName: "mp_m_securoguard_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_m_prolhost_01", new Vector3(389.8079, -989.87134, 29.418055), -90f, questName: "a_m_m_prolhost_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_taostranslator", new Vector3(-69.961845, -1759.9214, 29.534023), 71f, questName: "cs_taostranslator", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_stbla_02", new Vector3(171.25653, -1560.0239, 29.320124), -115f, questName: "a_m_y_stbla_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_russiandrunk", new Vector3(256.43985, -1258.9534, 29.14287), -90f, questName: "ig_russiandrunk", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_russiandrunk", new Vector3(-312.22534, -1467.2571, 30.546358), 114f, questName: "cs_russiandrunk", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_ramp_mex", new Vector3(-523.5283, -1207.1218, 18.18481), 157f, questName: "ig_ramp_mex", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("a_m_y_polynesian_01", new Vector3(-723.6481, -931.11017, 19.21251), -128f, questName: "a_m_y_polynesian_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_popov", new Vector3(612.0537, 269.0573, 103.0895), -91f, questName: "ig_popov", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_oneil", new Vector3(826.2385, -1032.7644, 26.47577), 38f, questName: "ig_oneil", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_y_pologoon_01", new Vector3(1183.0242, -329.95773, 69.17447), 13f, questName: "g_m_y_pologoon_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("g_m_y_pologoon_02", new Vector3(2588.2583, 361.38275, 108.4688), 84f, questName: "g_m_y_pologoon_02", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_m_m_partytarget", new Vector3(2008.5488, 3783.4895, 32.18084), 169f, questName: "u_m_m_partytarget", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("u_m_y_party_01", new Vector3(-2554.7346, 2326.9414, 33.07803), 5f, questName: "u_m_y_party_01", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_oscar", new Vector3(180.09914, 6601.4204, 32.04737), -79f, questName: "csb_oscar", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("csb_paige", new Vector3(-1441.801, -277.62643, 46.20764), -49f, questName: "csb_paige", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("ig_paige", new Vector3(1208.004, -1408.195, 35.224125), 14f, questName: "ig_paige", isBlipVisible: false);
              PedSystem.Repository.CreateQuest("cs_patricia", new Vector3(-1807.5126, 798.75854, 138.50688), -49f, questName: "cs_patricia", isBlipVisible: false);
              */
        }

        public static zdobich_quests Get(ExtPlayer player, int line, bool reward = false)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return zdobich_quests.Error;

            switch ((zdobich_quests)line)
            {
                case zdobich_quests.Stage2:
                    MoneySystem.Wallet.Change(player, 6500);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.HealthKit, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage3;
                
                case zdobich_quests.Stage3:
                    return zdobich_quests.Stage7;
                
                case zdobich_quests.Stage7:
                    UpdateData.Exp(player, 1);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Firework2, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage8;
                
                case zdobich_quests.Stage8:
                    MoneySystem.Wallet.Change(player, 4500);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Case0, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage9;
                
                case zdobich_quests.Stage9:
                    UpdateData.Exp(player, 1);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Bear, 1, addInWarehouse:true); 
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Note, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage10;
                
                case zdobich_quests.Stage10:
                    MoneySystem.Wallet.Change(player, 4800);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Rose, 1, addInWarehouse:true); 
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Firework4, 1, addInWarehouse:true);
                    return zdobich_quests.Stage11;

                case zdobich_quests.Stage11:
                    MoneySystem.Wallet.Change(player, 15000);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Case2, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage20;
                
                case zdobich_quests.Stage20:
                    MoneySystem.Wallet.Change(player, 10000);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Bong, 1, "420", addInWarehouse:true); 
                    return zdobich_quests.Stage21;
                
                case zdobich_quests.Stage21:
                    UpdateData.Exp(player, 1);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.HealthKit, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage23;
                
                case zdobich_quests.Stage23:
                    MoneySystem.Wallet.Change(player, 10000);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Case1, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage24;
                
                case zdobich_quests.Stage24:
                    UpdateData.Exp(player, 1);
                    MoneySystem.Wallet.Change(player, 15000);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Firework3, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage25;
                
                case zdobich_quests.Stage25:
                    MoneySystem.Wallet.Change(player, 5000);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Ball, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage26;
                
                case zdobich_quests.Stage26:
                    UpdateData.Exp(player, 1);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.BodyArmor, 1, "100", addInWarehouse:true); 
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Bear, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage28;
                
                case zdobich_quests.Stage28:
                    MoneySystem.Wallet.Change(player, 3500);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Vape, 1, "100", addInWarehouse:true); 
                    return zdobich_quests.Stage29;
                
                case zdobich_quests.Stage29:
                    UpdateData.Exp(player, 1);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Guitar, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage30;
                
                case zdobich_quests.Stage30:
                    MoneySystem.Wallet.Change(player, 3500);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.LoveNote, 1, "1000", addInWarehouse:true); 
                    return zdobich_quests.Stage31;
                
                case zdobich_quests.Stage31:
                    UpdateData.Exp(player, 1);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Firework4, 3, addInWarehouse:true); 
                    return zdobich_quests.Stage33;
                
                case zdobich_quests.Stage33:
                    UpdateData.Exp(player, 1);
                    MoneySystem.Wallet.Change(player, 8500);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Case0, 1, addInWarehouse:true); 
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Binoculars, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage34;
                
                case zdobich_quests.Stage34:
                    return zdobich_quests.NoMission;
                /*case zdobich_quests.End:
                    //Награды
                    UpdateData.Exp(player, 10);
                    MoneySystem.Wallet.Change(player, 50000);
                    return zdobich_quests.NoMission;*/
            }
            return zdobich_quests.Error;
        }
        public static void Take(ExtPlayer player, int Line)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return;

            switch ((zdobich_quests)Line)
            {
                case zdobich_quests.Start:
                    if (characterData.Gender)
                        Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Top, 1, "210_0_True"); 
                    else
                        Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Top, 1, "65_0_False"); 
    
                    qMain.UpdateQuestsLine(player, QuestName, (int)zdobich_quests.Start, (int)zdobich_quests.Stage2);
                    break;
                case zdobich_quests.Stage3:
                    qMain.UpdateQuestsLine(player, QuestName, (int)zdobich_quests.Stage3, (int)zdobich_quests.Stage7);
                    break;
                case zdobich_quests.Stage7:
                    if (characterData.Licenses[1])
                    {
                        Timers.StartOnce(50, () =>
                        {
                            qMain.UpdateQuestsComplete(player, QuestName, (int)zdobich_quests.Stage7, true);
                            OpenSuccess(player); 
                        });
                    }
                    break;
                case zdobich_quests.Stage9:
                    characterData.Handshaked++;
                    if (characterData.Handshaked >= 5)
                    {
                        Timers.StartOnce(50, () =>
                        {
                            qMain.UpdateQuestsStage(player, QuestName, (int)zdobich_quests.Stage9, 1, isUpdateHud: true);
                            qMain.UpdateQuestsComplete(player, QuestName, (int) zdobich_quests.Stage9, true);
                            OpenSuccess(player); 
                        });
                    }
                    break;
                case zdobich_quests.Stage24:
                    var house = HouseManager.GetHouse(player, checkOwner: true);
                    if (house != null)
                    {
                        Timers.StartOnce(50, () =>
                        {
                            qMain.UpdateQuestsStage(player, QuestName, (int)zdobich_quests.Stage24, 1, isUpdateHud: true);
                            qMain.UpdateQuestsComplete(player, QuestName, (int) zdobich_quests.Stage24, true);
                            OpenSuccess(player); 
                        });
                    }
                    break;
                case zdobich_quests.Stage25:
                    house = HouseManager.GetHouse(player, checkOwner: true);
                    if (house != null && (FurnitureManager.HouseFurnitures.ContainsKey(house.ID) || house.Type == 7))
                    {
                        if (house.Type != 7)
                        {
                            var houseFurnitureCount = FurnitureManager.HouseFurnitures[house.ID].Count;
                            if (houseFurnitureCount > 0)
                            {
                                Timers.StartOnce(50, () =>
                                {
                                    qMain.UpdateQuestsStage(player, QuestName, (int)zdobich_quests.Stage25, 1,
                                        isUpdateHud: true);
                                    qMain.UpdateQuestsComplete(player, QuestName, (int)zdobich_quests.Stage25, true);
                                    OpenSuccess(player);
                                });
                            }
                        }
                        else
                        {
                            Timers.StartOnce(50, () =>
                            {
                                qMain.UpdateQuestsLine(player, QuestName, (int)zdobich_quests.Stage25,
                                    (int)zdobich_quests.Stage26);
                                //qMain.UpdateQuestsStatus(player, QuestName, (int)zdobich_quests.Stage26, 1);
                                OpenSuccess(player);
                            });
                        }
                    }
                    break;
                case zdobich_quests.Stage26:
                    var vehiclesCount = VehicleManager.GetVehiclesCarCountToPlayer(player.Name);
                    if (vehiclesCount > 0)
                    {        
                        Timers.StartOnce(50, () =>
                        {
                            qMain.UpdateQuestsStage(player, QuestName, (int) zdobich_quests.Stage26, 1, isUpdateHud: true);
                            qMain.UpdateQuestsComplete(player, QuestName, (int) zdobich_quests.Stage26, true);
                            OpenSuccess(player); 
                        });
                    }
                    break;
                case zdobich_quests.Stage31:
                    if (characterData.Licenses[7])
                    {
                        Timers.StartOnce(50, () =>
                        {
                            qMain.UpdateQuestsComplete(player, QuestName, (int)zdobich_quests.Stage31, true);
                            OpenSuccess(player); 
                        });
                    }
                    break;
                case zdobich_quests.Stage33:
                    if (characterData.Licenses[6])
                    {
                        Timers.StartOnce(50, () =>
                        {
                            qMain.UpdateQuestsComplete(player, QuestName, (int)zdobich_quests.Stage33, true);
                            OpenSuccess(player); 
                        });
                    }
                    break;
            }
        }

        private static void OpenSuccess(ExtPlayer player)
        {
            var shapeData = CustomColShape.GetData(player, ColShapeEnums.QuestZdobich);

            if (shapeData != null)
                QuestZdobich(player, shapeData.Index, 1);
        }

        [Interaction(ColShapeEnums.QuestZdobich)]
        private static void Open(ExtPlayer player, int index)
        {
            QuestZdobich(player, index);
        }
        private static void QuestZdobich(ExtPlayer player, int index, int speed = 0)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            if (sessionData.CuffedData.Cuffed)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsCuffed), 3000);
                return;
            }
            if (sessionData.DeathData.InDeath)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsDying), 3000);
                return;
            }
            
            if (Main.IHaveDemorgan(player, true)) return;
            bool isBool = qMain.SetQuests(player, QuestName, isInsert: true);
            if (!isBool) return;
            var questData = player.GetQuest();
            if (questData == null) 
                return;

            if (questData.Line == (int)zdobich_quests.Stage8 && !questData.Complete && Chars.Repository.isItem(player, "inventory", ItemId.GasCan) != null && Chars.Repository.isItem(player, "inventory", ItemId.Wrench) != null)
            {
                qMain.UpdateQuestsComplete(player, QuestName, (int)zdobich_quests.Stage8, true);
                questData.Complete = true;
            }
            
            if (questData.Line == (int)zdobich_quests.Stage26 && characterData.BizIDs.Count > 0)
            {
                questData.Line = (int)zdobich_quests.Stage28;
                
                qMain.UpdateQuestsLine(player, QuestName, (int)zdobich_quests.Stage26,
                    (int)zdobich_quests.Stage28);
                qMain.UpdateQuestsStatus(player, QuestName, (int)zdobich_quests.Stage28, 1);
            }

            if (questData.Line == (int) zdobich_quests.Stage33 && !questData.Complete && characterData.Licenses[6])
            {
                qMain.UpdateQuestsStage(player, QuestName, (int)zdobich_quests.Stage33, 2, isUpdateHud: true);
                qMain.UpdateQuestsComplete(player, QuestName, (int) zdobich_quests.Stage33, true);
                questData.Complete = true;
            }
            

            Trigger.ClientEvent(player, "client.quest.open", index, QuestName, questData.Line, questData.Status, questData.Complete, speed);
        }
    }
}