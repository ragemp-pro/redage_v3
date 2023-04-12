using GTANetworkAPI;
using NeptuneEvo.Handles;
using System;
using System.Collections.Generic;
using NeptuneEvo.Core;
using Redage.SDK;
using System.Linq;
using Localization;
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
    class Bus : Script
    {
        private static readonly nLog Log = new nLog("Jobs.Bus");

        private static List<string> BusWaysNames = new List<string>
        {
            "[Городской 1] EMS - Old City Hall - Market - Driving School - NEWS - LSPD",
            "[Городской 2] EMS - Old City Hall - Market - Driving School - NEWS - LSPD",
            "[Городской 3] EMS - Old City Hall - Market - Driving School - NEWS - LSPD",
        };
        private static List<List<BusCheck>> BusWays = new List<List<BusCheck>>()
        {
            new List<BusCheck>() // busway1
            {
                new BusCheck(new Vector3(463.78735, -674.06415, 27.818863)),
                new BusCheck(new Vector3(353.81128, -673.3719, 29.834314)),
                new BusCheck(new Vector3(308.50708, -764.00635, 29.754377), true),
                new BusCheck(new Vector3(298.45306, -812.08545, 29.841228)),
                new BusCheck(new Vector3(241.48691, -832.1899, 30.405945)),
                new BusCheck(new Vector3(214.81517, -733.6068, 34.60165)),
                new BusCheck(new Vector3(274.30054, -586.10626, 43.644268), true),
                new BusCheck(new Vector3(313.15225, -485.85574, 43.750698)),
                new BusCheck(new Vector3(451.86758, -351.58432, 47.82203)),
                new BusCheck(new Vector3(522.57794, -205.16173, 52.396324)),
                new BusCheck(new Vector3(575.04987, -89.98442, 69.04095)),
                new BusCheck(new Vector3(694.20636, -2.577138, 84.63317)),
                new BusCheck(new Vector3(769.78326, 106.87052, 79.26235)),
                new BusCheck(new Vector3(761.2202, 176.63538, 82.423584), true),
                new BusCheck(new Vector3(614.37085, 232.57607, 102.10711)),
                new BusCheck(new Vector3(436.92944, 293.80518, 103.45449)),
                new BusCheck(new Vector3(267.4122, 341.5739, 105.97788)),
                new BusCheck(new Vector3(89.18705, 336.86612, 112.95764)),
                new BusCheck(new Vector3(19.09471, 233.38876, 109.88524)),
                new BusCheck(new Vector3(-35.34923, 83.759674, 75.335236)),
                new BusCheck(new Vector3(-70.66857, -54.56637, 61.304157)),
                new BusCheck(new Vector3(-115.73929, -84.75978, 57.15646)),
                new BusCheck(new Vector3(-251.53116, -56.768105, 50.025047)),
                new BusCheck(new Vector3(-319.9071, -177.89638, 39.713936)),
                new BusCheck(new Vector3(-517.8612, -269.98218, 35.961834), true),
                new BusCheck(new Vector3(-637.0907, -340.11807, 35.28972)),
                new BusCheck(new Vector3(-745.4498, -326.02478, 36.69483)),
                new BusCheck(new Vector3(-968.3309, -216.46997, 38.240185)),
                new BusCheck(new Vector3(-1287.9657, -53.025146, 47.223648), true),
                new BusCheck(new Vector3(-1477.9679, -99.27132, 51.35809)),
                new BusCheck(new Vector3(-1607.1597, -212.67621, 55.260513)),
                new BusCheck(new Vector3(-1679.347, -341.61438, 49.420418)),
                new BusCheck(new Vector3(-1770.7251, -441.78616, 42.275208)),
                new BusCheck(new Vector3(-1882.2374, -380.65005, 48.945312), true),
                new BusCheck(new Vector3(-1929.1748, -315.1801, 45.75602)),
                new BusCheck(new Vector3(-1919.2212, -213.98195, 36.42688)),
                new BusCheck(new Vector3(-2077.2607, -181.1639, 23.561241)),
                new BusCheck(new Vector3(-2165.0835, -313.60608, 13.513528)),
                new BusCheck(new Vector3(-2116.118, -377.2931, 13.309739)),
                new BusCheck(new Vector3(-1886.9792, -559.1868, 12.175919)),
                new BusCheck(new Vector3(-1731.1263, -700.6112, 10.700001), true),
                new BusCheck(new Vector3(-1636.2358, -763.39386, 10.722627)),
                new BusCheck(new Vector3(-1447.6786, -841.2925, 16.146486)),
                new BusCheck(new Vector3(-1290.3981, -900.5671, 11.873873)),
                new BusCheck(new Vector3(-1154.48, -832.8742, 14.824876)),
                new BusCheck(new Vector3(-1033.4685, -806.7502, 17.812508)),
                new BusCheck(new Vector3(-857.21796, -971.69055, 15.455125)),
                new BusCheck(new Vector3(-754.7794, -1160.3079, 11.13419)),
                new BusCheck(new Vector3(-688.2619, -1249.8187, 11.083057), true),
                new BusCheck(new Vector3(-648.5486, -1293.1781, 11.160974)),
                new BusCheck(new Vector3(-544.2863, -1165.2975, 19.551844)),
                new BusCheck(new Vector3(-521.17126, -937.24036, 24.423046), true),
                new BusCheck(new Vector3(-457.6673, -845.75977, 31.06772)),
                new BusCheck(new Vector3(-252.32465, -881.33875, 31.207403), true),
                new BusCheck(new Vector3(-126.848404, -919.5824, 29.803246)),
                new BusCheck(new Vector3(1.3415923, -967.2384, 29.886044)),
                new BusCheck(new Vector3(354.1879, -1060.4279, 29.867931), true),
                new BusCheck(new Vector3(402.01562, -998.7102, 29.867247)),
                new BusCheck(new Vector3(454.75784, -958.1483, 28.884958)),
                new BusCheck(new Vector3(503.65225, -890.3943, 26.0824)),
                new BusCheck(new Vector3(507.85663, -750.52423, 25.299847)),

            }, 
            new List<BusCheck>() // busway2
            {
                new BusCheck(new Vector3(463.78735, -674.06415, 27.818863)),
                new BusCheck(new Vector3(353.81128, -673.3719, 29.834314)),
                new BusCheck(new Vector3(308.50708, -764.00635, 29.754377), true),
                new BusCheck(new Vector3(298.45306, -812.08545, 29.841228)),
                new BusCheck(new Vector3(241.48691, -832.1899, 30.405945)),
                new BusCheck(new Vector3(214.81517, -733.6068, 34.60165)),
                new BusCheck(new Vector3(274.30054, -586.10626, 43.644268), true),
                new BusCheck(new Vector3(313.15225, -485.85574, 43.750698)),
                new BusCheck(new Vector3(451.86758, -351.58432, 47.82203)),
                new BusCheck(new Vector3(522.57794, -205.16173, 52.396324)),
                new BusCheck(new Vector3(575.04987, -89.98442, 69.04095)),
                new BusCheck(new Vector3(694.20636, -2.577138, 84.63317)),
                new BusCheck(new Vector3(769.78326, 106.87052, 79.26235)),
                new BusCheck(new Vector3(761.2202, 176.63538, 82.423584), true),
                new BusCheck(new Vector3(614.37085, 232.57607, 102.10711)),
                new BusCheck(new Vector3(436.92944, 293.80518, 103.45449)),
                new BusCheck(new Vector3(267.4122, 341.5739, 105.97788)),
                new BusCheck(new Vector3(89.18705, 336.86612, 112.95764)),
                new BusCheck(new Vector3(19.09471, 233.38876, 109.88524)),
                new BusCheck(new Vector3(-35.34923, 83.759674, 75.335236)),
                new BusCheck(new Vector3(-70.66857, -54.56637, 61.304157)),
                new BusCheck(new Vector3(-115.73929, -84.75978, 57.15646)),
                new BusCheck(new Vector3(-251.53116, -56.768105, 50.025047)),
                new BusCheck(new Vector3(-319.9071, -177.89638, 39.713936)),
                new BusCheck(new Vector3(-517.8612, -269.98218, 35.961834), true),
                new BusCheck(new Vector3(-637.0907, -340.11807, 35.28972)),
                new BusCheck(new Vector3(-745.4498, -326.02478, 36.69483)),
                new BusCheck(new Vector3(-968.3309, -216.46997, 38.240185)),
                new BusCheck(new Vector3(-1287.9657, -53.025146, 47.223648), true),
                new BusCheck(new Vector3(-1477.9679, -99.27132, 51.35809)),
                new BusCheck(new Vector3(-1607.1597, -212.67621, 55.260513)),
                new BusCheck(new Vector3(-1679.347, -341.61438, 49.420418)),
                new BusCheck(new Vector3(-1770.7251, -441.78616, 42.275208)),
                new BusCheck(new Vector3(-1882.2374, -380.65005, 48.945312), true),
                new BusCheck(new Vector3(-1929.1748, -315.1801, 45.75602)),
                new BusCheck(new Vector3(-1919.2212, -213.98195, 36.42688)),
                new BusCheck(new Vector3(-2077.2607, -181.1639, 23.561241)),
                new BusCheck(new Vector3(-2165.0835, -313.60608, 13.513528)),
                new BusCheck(new Vector3(-2116.118, -377.2931, 13.309739)),
                new BusCheck(new Vector3(-1886.9792, -559.1868, 12.175919)),
                new BusCheck(new Vector3(-1731.1263, -700.6112, 10.700001), true),
                new BusCheck(new Vector3(-1636.2358, -763.39386, 10.722627)),
                new BusCheck(new Vector3(-1447.6786, -841.2925, 16.146486)),
                new BusCheck(new Vector3(-1290.3981, -900.5671, 11.873873)),
                new BusCheck(new Vector3(-1154.48, -832.8742, 14.824876)),
                new BusCheck(new Vector3(-1033.4685, -806.7502, 17.812508)),
                new BusCheck(new Vector3(-857.21796, -971.69055, 15.455125)),
                new BusCheck(new Vector3(-754.7794, -1160.3079, 11.13419)),
                new BusCheck(new Vector3(-688.2619, -1249.8187, 11.083057), true),
                new BusCheck(new Vector3(-648.5486, -1293.1781, 11.160974)),
                new BusCheck(new Vector3(-544.2863, -1165.2975, 19.551844)),
                new BusCheck(new Vector3(-521.17126, -937.24036, 24.423046), true),
                new BusCheck(new Vector3(-457.6673, -845.75977, 31.06772)),
                new BusCheck(new Vector3(-252.32465, -881.33875, 31.207403), true),
                new BusCheck(new Vector3(-126.848404, -919.5824, 29.803246)),
                new BusCheck(new Vector3(1.3415923, -967.2384, 29.886044)),
                new BusCheck(new Vector3(354.1879, -1060.4279, 29.867931), true),
                new BusCheck(new Vector3(402.01562, -998.7102, 29.867247)),
                new BusCheck(new Vector3(454.75784, -958.1483, 28.884958)),
                new BusCheck(new Vector3(503.65225, -890.3943, 26.0824)),
                new BusCheck(new Vector3(507.85663, -750.52423, 25.299847)),

            },
            new List<BusCheck>() // busway3
            {
                new BusCheck(new Vector3(463.78735, -674.06415, 27.818863)),
                new BusCheck(new Vector3(353.81128, -673.3719, 29.834314)),
                new BusCheck(new Vector3(308.50708, -764.00635, 29.754377), true),
                new BusCheck(new Vector3(298.45306, -812.08545, 29.841228)),
                new BusCheck(new Vector3(241.48691, -832.1899, 30.405945)),
                new BusCheck(new Vector3(214.81517, -733.6068, 34.60165)),
                new BusCheck(new Vector3(274.30054, -586.10626, 43.644268), true),
                new BusCheck(new Vector3(313.15225, -485.85574, 43.750698)),
                new BusCheck(new Vector3(451.86758, -351.58432, 47.82203)),
                new BusCheck(new Vector3(522.57794, -205.16173, 52.396324)),
                new BusCheck(new Vector3(575.04987, -89.98442, 69.04095)),
                new BusCheck(new Vector3(694.20636, -2.577138, 84.63317)),
                new BusCheck(new Vector3(769.78326, 106.87052, 79.26235)),
                new BusCheck(new Vector3(761.2202, 176.63538, 82.423584), true),
                new BusCheck(new Vector3(614.37085, 232.57607, 102.10711)),
                new BusCheck(new Vector3(436.92944, 293.80518, 103.45449)),
                new BusCheck(new Vector3(267.4122, 341.5739, 105.97788)),
                new BusCheck(new Vector3(89.18705, 336.86612, 112.95764)),
                new BusCheck(new Vector3(19.09471, 233.38876, 109.88524)),
                new BusCheck(new Vector3(-35.34923, 83.759674, 75.335236)),
                new BusCheck(new Vector3(-70.66857, -54.56637, 61.304157)),
                new BusCheck(new Vector3(-115.73929, -84.75978, 57.15646)),
                new BusCheck(new Vector3(-251.53116, -56.768105, 50.025047)),
                new BusCheck(new Vector3(-319.9071, -177.89638, 39.713936)),
                new BusCheck(new Vector3(-517.8612, -269.98218, 35.961834), true),
                new BusCheck(new Vector3(-637.0907, -340.11807, 35.28972)),
                new BusCheck(new Vector3(-745.4498, -326.02478, 36.69483)),
                new BusCheck(new Vector3(-968.3309, -216.46997, 38.240185)),
                new BusCheck(new Vector3(-1287.9657, -53.025146, 47.223648), true),
                new BusCheck(new Vector3(-1477.9679, -99.27132, 51.35809)),
                new BusCheck(new Vector3(-1607.1597, -212.67621, 55.260513)),
                new BusCheck(new Vector3(-1679.347, -341.61438, 49.420418)),
                new BusCheck(new Vector3(-1770.7251, -441.78616, 42.275208)),
                new BusCheck(new Vector3(-1882.2374, -380.65005, 48.945312), true),
                new BusCheck(new Vector3(-1929.1748, -315.1801, 45.75602)),
                new BusCheck(new Vector3(-1919.2212, -213.98195, 36.42688)),
                new BusCheck(new Vector3(-2077.2607, -181.1639, 23.561241)),
                new BusCheck(new Vector3(-2165.0835, -313.60608, 13.513528)),
                new BusCheck(new Vector3(-2116.118, -377.2931, 13.309739)),
                new BusCheck(new Vector3(-1886.9792, -559.1868, 12.175919)),
                new BusCheck(new Vector3(-1731.1263, -700.6112, 10.700001), true),
                new BusCheck(new Vector3(-1636.2358, -763.39386, 10.722627)),
                new BusCheck(new Vector3(-1447.6786, -841.2925, 16.146486)),
                new BusCheck(new Vector3(-1290.3981, -900.5671, 11.873873)),
                new BusCheck(new Vector3(-1154.48, -832.8742, 14.824876)),
                new BusCheck(new Vector3(-1033.4685, -806.7502, 17.812508)),
                new BusCheck(new Vector3(-857.21796, -971.69055, 15.455125)),
                new BusCheck(new Vector3(-754.7794, -1160.3079, 11.13419)),
                new BusCheck(new Vector3(-688.2619, -1249.8187, 11.083057), true),
                new BusCheck(new Vector3(-648.5486, -1293.1781, 11.160974)),
                new BusCheck(new Vector3(-544.2863, -1165.2975, 19.551844)),
                new BusCheck(new Vector3(-521.17126, -937.24036, 24.423046), true),
                new BusCheck(new Vector3(-457.6673, -845.75977, 31.06772)),
                new BusCheck(new Vector3(-252.32465, -881.33875, 31.207403), true),
                new BusCheck(new Vector3(-126.848404, -919.5824, 29.803246)),
                new BusCheck(new Vector3(1.3415923, -967.2384, 29.886044)),
                new BusCheck(new Vector3(354.1879, -1060.4279, 29.867931), true),
                new BusCheck(new Vector3(402.01562, -998.7102, 29.867247)),
                new BusCheck(new Vector3(454.75784, -958.1483, 28.884958)),
                new BusCheck(new Vector3(503.65225, -890.3943, 26.0824)),
                new BusCheck(new Vector3(507.85663, -750.52423, 25.299847)),

            },
            /*new List<BusCheck>() // busway6
            {
                new BusCheck(new Vector3(452.2434, -587.5063, 27.37679), false), // 1 
                new BusCheck(new Vector3(421.4142, -639.011, 27.3762), false), // 2 
                new BusCheck(new Vector3(421.622, -663.5898, 27.70913), false), // 3 
                new BusCheck(new Vector3(386.1311, -672.2119, 28.09329), false), // 4 
                new BusCheck(new Vector3(339.8356, -689.7584, 28.22546), false), // 5 
                new BusCheck(new Vector3(293.2329, -827.4853, 28.20639), false), // 6 
                new BusCheck(new Vector3(256.9976, -926.5665, 28.07068), false), // 7 
                new BusCheck(new Vector3(221.0342, -1010.805, 28.11632), false), // 8 
                new BusCheck(new Vector3(205.3955, -1111.734, 28.21861), false), // 9 
                new BusCheck(new Vector3(209.8023, -1262.885, 28.11355), false), // 10 
                new BusCheck(new Vector3(167.6845, -1368.563, 28.22132), false), // 11 
                new BusCheck(new Vector3(124.418, -1420.834, 28.21667), false), // 12 
                new BusCheck(new Vector3(75.05229, -1488.073, 28.21492), false), // 13 
                new BusCheck(new Vector3(21.88153, -1532.233, 28.06308), true), // 14 
                new BusCheck(new Vector3(-13.4475, -1576.497, 28.12444), false), // 15 
                new BusCheck(new Vector3(-46.7314, -1573.934, 28.49072), false), // 16 
                new BusCheck(new Vector3(-94.3346, -1533.805, 32.48144), false), // 17 
                new BusCheck(new Vector3(-182.0512, -1465.002, 30.57711), false), // 18 
                new BusCheck(new Vector3(-250.7604, -1420.468, 30.13886), false), // 19 
                new BusCheck(new Vector3(-270.9648, -1193.92, 22.72335), false), // 20 
                new BusCheck(new Vector3(-275.1719, -1166.776, 21.96469), false), // 21 
                new BusCheck(new Vector3(-301.226, -1137.125, 22.2746), false), // 22 
                new BusCheck(new Vector3(-508.1477, -1075.151, 21.74836), false), // 23 
                new BusCheck(new Vector3(-528.0355, -1045.954, 21.44039), false), // 24 
                new BusCheck(new Vector3(-539.4933, -986.8386, 22.23119), false), // 25 
                new BusCheck(new Vector3(-562.0121, -956.3507, 22.30779), false), // 26 
                new BusCheck(new Vector3(-612.4644, -954.7506, 20.56333), false), // 27 
                new BusCheck(new Vector3(-630.6671, -935.9827, 21.25985), false), // 28 
                new BusCheck(new Vector3(-630.2991, -863.0883, 23.76212), false), // 29 
                new BusCheck(new Vector3(-629.4188, -684.2758, 30.03441), false), // 30 
                new BusCheck(new Vector3(-624.2628, -573.6647, 33.73887), false), // 31 
                new BusCheck(new Vector3(-623.3481, -492.4886, 33.62532), false), // 32 
                new BusCheck(new Vector3(-621.5667, -397.3529, 33.61706), false), // 33 
                new BusCheck(new Vector3(-593.8776, -377.0666, 33.70655), false), // 34 
                new BusCheck(new Vector3(-548.8806, -372.2225, 34.00932), false), // 35 
                new BusCheck(new Vector3(-529.6252, -326.6782, 33.90096), true), // 36 
                new BusCheck(new Vector3(-573.9676, -302.2077, 34.03878), false), // 37 
                new BusCheck(new Vector3(-612.9118, -324.7812, 33.72791), false), // 38 
                new BusCheck(new Vector3(-672.2838, -353.6512, 33.52838), false), // 39 
                new BusCheck(new Vector3(-773.0165, -320.6183, 35.66534), false), // 40 
                new BusCheck(new Vector3(-954.8832, -228.7624, 36.89107), false), // 41 
                new BusCheck(new Vector3(-1005.331, -203.8067, 36.71843), false), // 42 
                new BusCheck(new Vector3(-1195.436, -99.55115, 39.7301), false), // 43 
                new BusCheck(new Vector3(-1292.869, -51.96901, 45.95277), false), // 44 
                new BusCheck(new Vector3(-1354.426, -44.17735, 50.02855), true), // 45 
                new BusCheck(new Vector3(-1398.493, -43.50904, 51.48283), false), // 46 
                new BusCheck(new Vector3(-1417.898, 19.73363, 51.39941), false), // 47 
                new BusCheck(new Vector3(-1403.442, 170.0919, 56.16145), false), // 48 
                new BusCheck(new Vector3(-1278.684, 210.5041, 58.91022), false), // 49 
                new BusCheck(new Vector3(-1109.947, 256.7668, 63.53265), false), // 50 
                new BusCheck(new Vector3(-1058.386, 254.2274, 62.98872), false), // 51 
                new BusCheck(new Vector3(-875.33, 219.8016, 72.23686), false), // 52 
                new BusCheck(new Vector3(-777.5953, 211.3047, 74.71775), false), // 53 
                new BusCheck(new Vector3(-675.9929, 248.9641, 80.24601), false), // 54 
                new BusCheck(new Vector3(-564.4936, 255.0852, 81.94671), false), // 55 
                new BusCheck(new Vector3(-245.4244, 258.0541, 90.93124), false), // 56 
                new BusCheck(new Vector3(-128.1201, 249.4092, 95.06844), false), // 57 
                new BusCheck(new Vector3(9.522894, 260.4031, 108.3304), false), // 58 
                new BusCheck(new Vector3(48.06821, 281.604, 108.6653), false), // 59 
                new BusCheck(new Vector3(229.6882, 348.335, 104.4106), false), // 60 
                new BusCheck(new Vector3(398.7303, 300.6967, 101.8343), false), // 61 
                new BusCheck(new Vector3(543.4553, 249.2258, 101.9547), false), // 62 
                new BusCheck(new Vector3(739.5217, 180.306, 83.11983), false), // 63 
                new BusCheck(new Vector3(743.294, 101.0865, 78.70757), true), // 64 
                new BusCheck(new Vector3(708.2929, 39.3074, 83.12298), false), // 65 
                new BusCheck(new Vector3(716.9739, -11.14128, 82.48751), false), // 66 
                new BusCheck(new Vector3(849.0188, -94.86089, 79.06285), false), // 67 
                new BusCheck(new Vector3(921.6854, -146.491, 74.49681), false), // 68 
                new BusCheck(new Vector3(916.2418, -192.2234, 71.97924), true), // 69 
                new BusCheck(new Vector3(917.4128, -258.0396, 67.56607), false), // 70 
                new BusCheck(new Vector3(943.6136, -308.0845, 65.74582), false), // 71 
                new BusCheck(new Vector3(782.7349, -341.8244, 48.74839), false), // 72 
                new BusCheck(new Vector3(676.6306, -382.3899, 40.20858), false), // 73 
                new BusCheck(new Vector3(647.972, -382.5377, 41.45459), false), // 74 
                new BusCheck(new Vector3(495.7778, -322.7706, 44.19924), false), // 75 
                new BusCheck(new Vector3(452.1121, -337.3417, 46.41497), false), // 76 
                new BusCheck(new Vector3(340.6479, -391.3669, 44.09866), false), // 77 
                new BusCheck(new Vector3(305.6043, -412.3622, 43.92403), false), // 78 
                new BusCheck(new Vector3(288.7418, -497.2232, 42.19519), false), // 79 
                new BusCheck(new Vector3(278.995, -524.0405, 42.161), false), // 80 
                new BusCheck(new Vector3(247.0715, -602.6978, 41.56906), false), // 81 
                new BusCheck(new Vector3(182.9187, -790.1367, 30.46082), false), // 82 
                new BusCheck(new Vector3(200.5206, -833.4717, 29.77912), false), // 83 
                new BusCheck(new Vector3(262.3922, -850.6533, 28.2874), false), // 84 
                new BusCheck(new Vector3(304.0861, -828.0737, 28.20625), false), // 85 
                new BusCheck(new Vector3(356.7018, -692.379, 28.12796), false), // 86 
                new BusCheck(new Vector3(441.0269, -679.8569, 27.62088), false), // 87 
                new BusCheck(new Vector3(468.3851, -626.2559, 27.36605), false), // 88 
                new BusCheck(new Vector3(421.9406, -618.7638, 27.37685), false), // 1 
                new BusCheck(new Vector3(421.8709, -657.6866, 27.53283), false), // 2 
                new BusCheck(new Vector3(393.6165, -673.4533, 28.07841), false), // 3 
                new BusCheck(new Vector3(346.8577, -690.0728, 28.22062), false), // 5 
                new BusCheck(new Vector3(298.8752, -809.5145, 28.21675), false), // 6 
                new BusCheck(new Vector3(258.0472, -924.6455, 28.04573), false), // 7 
                new BusCheck(new Vector3(225.6352, -1013.022, 28.19472), false), // 8 
                new BusCheck(new Vector3(208.7699, -1064.417, 28.13343), false), // 9 
                new BusCheck(new Vector3(205.5882, -1111.78, 28.21763), false), // 10 
                new BusCheck(new Vector3(202.4902, -1154.195, 28.1399), false), // 11 
                new BusCheck(new Vector3(209.3809, -1266.229, 28.10783), false), // 12 
                new BusCheck(new Vector3(168.1012, -1368.395, 28.22142), false), // 13 
                new BusCheck(new Vector3(100.2492, -1457.411, 28.21803), false), // 14 
                new BusCheck(new Vector3(75.45205, -1487.638, 28.21903), false), // 15 
                new BusCheck(new Vector3(20.91828, -1533.513, 28.06536), true), // 16 
                new BusCheck(new Vector3(-10.86836, -1580.971, 28.21745), false), // 17 
                new BusCheck(new Vector3(-50.68108, -1621.147, 28.14725), false), // 18 
                new BusCheck(new Vector3(-120.4083, -1712.595, 28.59435), false), // 19 
                new BusCheck(new Vector3(-109.4427, -1765.048, 28.65141), false), // 20 
                new BusCheck(new Vector3(-38.42766, -1824.238, 25.06485), false), // 21 
                new BusCheck(new Vector3(42.46912, -1891.335, 20.86017), false), // 22 
                new BusCheck(new Vector3(75.74171, -1889.607, 21.11408), false), // 23 
                new BusCheck(new Vector3(154.3352, -1796.16, 27.77178), false), // 24 
                new BusCheck(new Vector3(154.3352, -1796.16, 27.77178), false), // 24 
                new BusCheck(new Vector3(193.6535, -1753.086, 27.6894), false), // 25 
                new BusCheck(new Vector3(249.6708, -1708.615, 27.93861), false), // 26 
                new BusCheck(new Vector3(266.8677, -1687.659, 28.10419), false), // 27 
                new BusCheck(new Vector3(259.3495, -1655.42, 28.13954), false), // 28 
                new BusCheck(new Vector3(218.8917, -1618.119, 28.133), false), // 29 
                new BusCheck(new Vector3(220.5459, -1581.312, 28.1322), false), // 30 
                new BusCheck(new Vector3(295.3427, -1527.888, 28.2114), false), // 31 
                new BusCheck(new Vector3(306.0768, -1495.258, 28.21554), false), // 32 
                new BusCheck(new Vector3(261.2666, -1450.514, 28.14244), false), // 33 
                new BusCheck(new Vector3(261.593, -1425.938, 28.17628), false), // 34 
                new BusCheck(new Vector3(325.1118, -1348.65, 31.24809), false), // 35 
                new BusCheck(new Vector3(412.9283, -1262.451, 31.1344), false), // 36 
                new BusCheck(new Vector3(469.9149, -1264.976, 28.43671), true), // 37 
                new BusCheck(new Vector3(500.1995, -1286.848, 28.15525), false), // 38 
                new BusCheck(new Vector3(524.0394, -1337.825, 28.09183), false), // 39 
                new BusCheck(new Vector3(532.9698, -1407.265, 28.13147), false), // 40 
                new BusCheck(new Vector3(574.0267, -1440.002, 28.55653), false), // 41 
                new BusCheck(new Vector3(677.1075, -1445.318, 29.79763), false), // 42 
                new BusCheck(new Vector3(765.7747, -1440.348, 26.55122), false), // 43 
                new BusCheck(new Vector3(800.4399, -1410.418, 26.14221), false), // 44 
                new BusCheck(new Vector3(806.0775, -1332.82, 25.07283), false), // 45 
                new BusCheck(new Vector3(806.1255, -1330.809, 25.07724), false), // 45 
                new BusCheck(new Vector3(807.5174, -1262.249, 25.22744), false), // 46 
                new BusCheck(new Vector3(807.001, -1196.299, 26.16691), true), // 47 
                new BusCheck(new Vector3(804.5488, -1167.321, 27.55787), false), // 48 
                new BusCheck(new Vector3(801.2711, -1119.821, 28.02073), false), // 49 
                new BusCheck(new Vector3(816.2949, -1088.402, 27.44702), false), // 50 
                new BusCheck(new Vector3(939.2841, -1088.803, 34.32776), false), // 51 
                new BusCheck(new Vector3(997.1431, -1012.801, 40.98442), false), // 52 
                new BusCheck(new Vector3(1013.027, -990.4582, 41.29606), false), // 53 
                new BusCheck(new Vector3(1132.458, -955.3203, 46.6568), false), // 54 
                new BusCheck(new Vector3(1151.959, -997.3826, 44.16554), false), // 55 
                new BusCheck(new Vector3(1187.216, -1100.158, 38.48834), false), // 56 
                new BusCheck(new Vector3(1237.02, -1224.887, 34.41777), false), // 57 
                new BusCheck(new Vector3(1224.449, -1377.012, 34.01226), false), // 58 
                new BusCheck(new Vector3(1203.998, -1421.756, 33.91041), false), // 59 
                new BusCheck(new Vector3(1097.563, -1428.605, 35.23575), false), // 60 
                new BusCheck(new Vector3(824.2581, -1438.221, 26.24022), false), // 61 
                new BusCheck(new Vector3(804.0295, -1484.91, 26.7456), false), // 62 
                new BusCheck(new Vector3(816.4278, -1721.253, 28.14004), false), // 63 
                new BusCheck(new Vector3(763.9579, -2035.383, 28.1831), false), // 64 
                new BusCheck(new Vector3(742.4531, -2162.879, 28.06165), true), // 65 
                new BusCheck(new Vector3(746.2746, -2225.885, 28.21369), false), // 66 
                new BusCheck(new Vector3(725.7681, -2395.195, 19.78458), false), // 67 
                new BusCheck(new Vector3(624.7829, -2492.191, 15.96053), false), // 68 
                new BusCheck(new Vector3(526.284, -2435.844, 13.34998), false), // 69 
                new BusCheck(new Vector3(529.1344, -2542.544, 4.877124), false), // 70 
                new BusCheck(new Vector3(668.3756, -2826.031, 5.04612), false), // 71 
                new BusCheck(new Vector3(630.8576, -3003.073, 4.919397), false), // 72 
                new BusCheck(new Vector3(582.1158, -3010.819, 4.921851), true), // 73 
                new BusCheck(new Vector3(485.5909, -3012.602, 4.917102), false), // 74 
                new BusCheck(new Vector3(478.0564, -2983.276, 4.925123), false), // 75 
                new BusCheck(new Vector3(639.1627, -2983.211, 4.921716), false), // 76 
                new BusCheck(new Vector3(672.9695, -2871.464, 5.066907), false), // 77 
                new BusCheck(new Vector3(570.3653, -2560.852, 5.390557), false), // 78 
                new BusCheck(new Vector3(612.9646, -2510.968, 15.77748), false), // 79 
                new BusCheck(new Vector3(706.2491, -2485.117, 19.0746), false), // 80 
                new BusCheck(new Vector3(759.6096, -2356.076, 22.56039), false), // 81 
                new BusCheck(new Vector3(782.4882, -2094.873, 28.1325), false), // 82 
                new BusCheck(new Vector3(815.0789, -1888.762, 28.10884), false), // 83 
                new BusCheck(new Vector3(839.2156, -1701.102, 28.21691), false), // 84 
                new BusCheck(new Vector3(817.5537, -1503.17, 27.2326), false), // 85 
                new BusCheck(new Vector3(765.5114, -1428.388, 26.49767), false), // 86 
                new BusCheck(new Vector3(566.6123, -1428.193, 28.39441), false), // 87 
                new BusCheck(new Vector3(534.1582, -1344.483, 28.14856), false), // 88 
                new BusCheck(new Vector3(510.3243, -708.2892, 23.78441), false), // 89 
                new BusCheck(new Vector3(479.5815, -674.241, 25.14934), false), // 90 
                new BusCheck(new Vector3(467.0784, -634.6727, 27.36381), false), // 91 
            },
            new List<BusCheck>()
            {
                new BusCheck(new Vector3(443.6877, -585.6654, 26.96806), false), // 1 
                new BusCheck(new Vector3(421.5607, -646.3228, 26.96887), false), // 2 
                new BusCheck(new Vector3(383.5445, -670.6879, 27.66392), false), // 3 
                new BusCheck(new Vector3(347.9878, -687.4364, 27.812), false), // 4 
                new BusCheck(new Vector3(293.0769, -829.8888, 27.79329), false), // 5 
                new BusCheck(new Vector3(256.3426, -929.2927, 27.70035), false), // 6 
                new BusCheck(new Vector3(224.5502, -1015.327, 27.79179), false), // 7 
                new BusCheck(new Vector3(205.1135, -1114.731, 27.80716), false), // 8 
                new BusCheck(new Vector3(209.7603, -1268.978, 27.72885), false), // 9 
                new BusCheck(new Vector3(165.0832, -1372.029, 27.81084), false), // 10 
                new BusCheck(new Vector3(131.8338, -1412.747, 27.80566), false), // 11 
                new BusCheck(new Vector3(72.20358, -1492.503, 27.79235), false), // 12 
                new BusCheck(new Vector3(20.02134, -1534.584, 27.66285), true), // 13 
                new BusCheck(new Vector3(-8.335826, -1585.989, 27.80775), false), // 14 
                new BusCheck(new Vector3(-13.36246, -1633.336, 27.75887), false), // 15 
                new BusCheck(new Vector3(24.70691, -1665.503, 27.75342), false), // 16 
                new BusCheck(new Vector3(94.80923, -1724.198, 27.39403), false), // 17 
                new BusCheck(new Vector3(150.4926, -1771.009, 27.41319), false), // 18 
                new BusCheck(new Vector3(247.373, -1852.126, 25.20545), false), // 19 
                new BusCheck(new Vector3(294.5547, -1891.913, 25.38673), false), // 20 
                new BusCheck(new Vector3(337.9873, -1928.695, 23.1462), false), // 21 
                new BusCheck(new Vector3(394.4581, -1978.488, 22.099), false), // 22 
                new BusCheck(new Vector3(456.8414, -2044.886, 22.73396), false), // 23 
                new BusCheck(new Vector3(456.2177, -2077.327, 21.36689), false), // 24 
                new BusCheck(new Vector3(433.944, -2113.04, 18.84294), false), // 25 
                new BusCheck(new Vector3(394.7001, -2157.282, 14.93311), false), // 26 
                new BusCheck(new Vector3(353.9064, -2214.09, 10.36401), false), // 27 
                new BusCheck(new Vector3(346.6998, -2345.49, 8.602489), false), // 28 
                new BusCheck(new Vector3(333.6717, -2447.544, 5.654027), false), // 29 
                new BusCheck(new Vector3(329.6519, -2494.301, 3.927794), false), // 30 
                new BusCheck(new Vector3(347.2268, -2508.839, 4.478691), false), // 31 
                new BusCheck(new Vector3(412.9386, -2510.853, 11.871), false), // 32 
                new BusCheck(new Vector3(498.1788, -2536.203, 4.948865), false), // 33 
                new BusCheck(new Vector3(564.9095, -2569.475, 5.0106), false), // 34 
                new BusCheck(new Vector3(664.5588, -2765.938, 4.646408), false), // 35 
                new BusCheck(new Vector3(665.5758, -2939.92, 4.517673), false), // 36 
                new BusCheck(new Vector3(648.3298, -2946.5, 4.510792), false), // 37 
                new BusCheck(new Vector3(592.355, -2958.057, 4.512614), false), // 38 
                new BusCheck(new Vector3(602.8403, -3018.18, 4.511153), true), // 39 
                new BusCheck(new Vector3(637.4865, -3004.125, 4.513565), false), // 40 
                new BusCheck(new Vector3(673.1204, -2898.62, 4.676573), false), // 41 
                new BusCheck(new Vector3(640.3093, -2680.34, 4.542529), false), // 42 
                new BusCheck(new Vector3(585.0902, -2576.085, 4.593889), false), // 43 
                new BusCheck(new Vector3(603.8317, -2513.08, 15.27206), false), // 44 
                new BusCheck(new Vector3(709.5429, -2484.908, 18.66641), false), // 45 
                new BusCheck(new Vector3(753.3414, -2433.371, 18.45189), false), // 46 
                new BusCheck(new Vector3(776.8031, -2215.839, 27.72613), false), // 47 
                new BusCheck(new Vector3(795.0637, -2020.518, 27.74448), false), // 48 
                new BusCheck(new Vector3(837.6479, -1807.675, 27.48327), true), // 49 
                new BusCheck(new Vector3(843.0119, -1716.738, 27.75781), false), // 50 
                new BusCheck(new Vector3(853.5858, -1603.594, 30.37821), false), // 51 
                new BusCheck(new Vector3(830.1688, -1522.482, 27.31706), false), // 52 
                new BusCheck(new Vector3(806.5134, -1455.481, 25.64765), false), // 53 
                new BusCheck(new Vector3(805.7725, -1398.135, 25.62822), false), // 55 
                new BusCheck(new Vector3(800.9803, -1255.748, 24.88928), false), // 56 
                new BusCheck(new Vector3(807.4732, -1195.799, 25.7757), true), // 57 
                new BusCheck(new Vector3(799.62, -1165.082, 27.27798), false), // 58 
                new BusCheck(new Vector3(780.0538, -1025.732, 24.65733), false), // 59 
                new BusCheck(new Vector3(761.7024, -1003.466, 24.61902), false), // 60 
                new BusCheck(new Vector3(568.8751, -1025.474, 35.50324), false), // 61 
                new BusCheck(new Vector3(416.2618, -1045.127, 28.02459), false), // 62 
                new BusCheck(new Vector3(394.9145, -1084.221, 27.82005), false), // 63 
                new BusCheck(new Vector3(395.4652, -1118.03, 27.88565), false), // 64 
                new BusCheck(new Vector3(430.8953, -1134.403, 27.86587), false), // 65 
                new BusCheck(new Vector3(485.5118, -1134.136, 27.87874), false), // 66 
                new BusCheck(new Vector3(498.7965, -1151.116, 27.74828), false), // 67 
                new BusCheck(new Vector3(498.8574, -1245.841, 27.70419), false), // 68 
                new BusCheck(new Vector3(449.797, -1249.931, 28.60692), true), // 69 
                new BusCheck(new Vector3(415.6601, -1257.142, 30.52675), false), // 70 
                new BusCheck(new Vector3(353.4924, -1306.899, 30.78134), false), // 71 
                new BusCheck(new Vector3(332.9001, -1306.49, 30.30584), false), // 72 
                new BusCheck(new Vector3(244.375, -1294.459, 27.67779), false), // 73 
                new BusCheck(new Vector3(233.0126, -1269.025, 27.66193), false), // 74 
                new BusCheck(new Vector3(218.3602, -1148.535, 27.80059), false), // 75 
                new BusCheck(new Vector3(218.8137, -1066.479, 27.68471), false), // 76 
                new BusCheck(new Vector3(237.1028, -1016.565, 27.77794), false), // 77 
                new BusCheck(new Vector3(253.5884, -966.584, 27.79571), false), // 78 
                new BusCheck(new Vector3(286.5269, -878.4503, 27.74542), false), // 79 
                new BusCheck(new Vector3(303.7062, -831.5664, 27.7911), false), // 80 
                new BusCheck(new Vector3(325.1037, -786.3784, 27.6881), false), // 81 
                new BusCheck(new Vector3(345.5882, -723.6211, 27.70319), false), // 82 
                new BusCheck(new Vector3(358.49, -687.8741, 27.71537), false), // 83 
                new BusCheck(new Vector3(378.0378, -679.6871, 27.64757), false), // 84 
                new BusCheck(new Vector3(434.3148, -679.9254, 27.51877), false), // 85 
                new BusCheck(new Vector3(462.9022, -661.4355, 25.96049), false), // 86 
                new BusCheck(new Vector3(469.7245, -615.9445, 26.9684), false), // 87 
            },
            new List<BusCheck>()
            {
                new BusCheck(new Vector3(446.6861, -585.6976, 27.00236), false), // 1 
                new BusCheck(new Vector3(421.7016, -644.7448, 27.00387), false), // 2 
                new BusCheck(new Vector3(421.5157, -662.2115, 27.32454), false), // 3 
                new BusCheck(new Vector3(382.6215, -670.8714, 27.72272), false), // 4 
                new BusCheck(new Vector3(347.8421, -684.2863, 27.84402), false), // 5 
                new BusCheck(new Vector3(286.7675, -830.9046, 27.73273), false), // 6 
                new BusCheck(new Vector3(261.4495, -838.7652, 27.92509), false), // 7 
                new BusCheck(new Vector3(197.8796, -822.0228, 29.5722), false), // 8 
                new BusCheck(new Vector3(59.43143, -771.5594, 30.21667), false), // 9 
                new BusCheck(new Vector3(9.114623, -753.1033, 30.33282), false), // 10 
                new BusCheck(new Vector3(-94.35464, -711.0247, 33.25034), false), // 11 
                new BusCheck(new Vector3(-230.318, -660.0676, 31.82929), false), // 12 
                new BusCheck(new Vector3(-238.2605, -632.8104, 32.12958), false), // 13 
                new BusCheck(new Vector3(-202.9594, -508.6712, 33.27271), false), // 14 
                new BusCheck(new Vector3(-205.3311, -444.3093, 31.53565), false), // 15 
                new BusCheck(new Vector3(-284.5208, -395.4593, 30.25545), false), // 16 
                new BusCheck(new Vector3(-323.7327, -390.3764, 30.22332), false), // 17 
                new BusCheck(new Vector3(-437.1243, -369.5344, 33.25035), false), // 18 
                new BusCheck(new Vector3(-524.6459, -355.003, 35.25411), false), // 19 
                new BusCheck(new Vector3(-530.0774, -326.6232, 33.89759), true), // 20 
                new BusCheck(new Vector3(-571.4929, -301.2676, 35.25404), false), // 21 
                new BusCheck(new Vector3(-612.0635, -330.7424, 33.34597), false), // 22 
                new BusCheck(new Vector3(-636.6931, -394.1144, 33.32231), false), // 23 
                new BusCheck(new Vector3(-639.257, -458.1576, 33.32069), false), // 24 
                new BusCheck(new Vector3(-659.2593, -470.9107, 33.16987), false), // 25 
                new BusCheck(new Vector3(-886.3428, -504.7376, 20.08286), false), // 26 
                new BusCheck(new Vector3(-1098.359, -609.2435, 14.11171), false), // 27 
                new BusCheck(new Vector3(-1612.887, -718.8878, 9.712423), false), // 28 
                new BusCheck(new Vector3(-1853.955, -550.7711, 10.09708), false), // 29 
                new BusCheck(new Vector3(-2146.804, -349.8192, 11.70847), false), // 30 
                new BusCheck(new Vector3(-2304.239, -314.5258, 12.22392), false), // 31 
                new BusCheck(new Vector3(-2646.875, -82.55535, 16.42738), false), // 32 
                new BusCheck(new Vector3(-3014.273, 166.0826, 14.14081), false), // 33 
                new BusCheck(new Vector3(-2984.149, 417.7342, 13.45377), false), // 34 
                new BusCheck(new Vector3(-3093.051, 793.4437, 17.45447), false), // 35 
                new BusCheck(new Vector3(-3104.435, 1097.097, 18.97407), true), // 36 
                new BusCheck(new Vector3(-3100.077, 1180.312, 18.8608), false), // 37 
                new BusCheck(new Vector3(-3013.796, 1439.467, 24.82408), false), // 38 
                new BusCheck(new Vector3(-3032.274, 1731.693, 35.25812), false), // 39 
                new BusCheck(new Vector3(-2980.151, 2022.194, 33.36716), false), // 40 
                new BusCheck(new Vector3(-2717.431, 2277.759, 17.9179), false), // 41 
                new BusCheck(new Vector3(-2675.636, 2476.249, 15.16318), false), // 42 
                new BusCheck(new Vector3(-2626.239, 2814.853, 15.16363), false), // 43 
                new BusCheck(new Vector3(-2554.164, 3398.625, 11.73912), false), // 44 
                new BusCheck(new Vector3(-2461.012, 3709.885, 13.72708), true), // 45 
                new BusCheck(new Vector3(-2408.56, 3864.183, 22.7857), false), // 46 
                new BusCheck(new Vector3(-2283.584, 4198.521, 39.39719), false), // 47 
                new BusCheck(new Vector3(-2169.977, 4419.877, 58.82072), false), // 48 
                new BusCheck(new Vector3(-2011.749, 4501.045, 55.55472), false), // 49 
                new BusCheck(new Vector3(-1763.015, 4748.355, 55.62331), false), // 50 
                new BusCheck(new Vector3(-1417.936, 5065.487, 59.68685), false), // 51 
                new BusCheck(new Vector3(-1295.887, 5230.222, 52.87307), false), // 52 
                new BusCheck(new Vector3(-1118.212, 5300.123, 49.21911), false), // 53 
                new BusCheck(new Vector3(-915.248, 5414.611, 35.58352), false), // 54 
                new BusCheck(new Vector3(-753.3878, 5495.157, 33.70341), false), // 55 
                new BusCheck(new Vector3(-448.4627, 5901.2, 31.3485), false), // 56 
                new BusCheck(new Vector3(-232.4834, 6136.175, 29.69926), false), // 57 
                new BusCheck(new Vector3(-151.3444, 6211.825, 29.69864), true), // 58 
                new BusCheck(new Vector3(-107.5252, 6257.996, 29.68674), false), // 59 
                new BusCheck(new Vector3(260.1705, 6560.667, 29.34554), false), // 60 
                new BusCheck(new Vector3(573.419, 6536.126, 26.44849), false), // 61 
                new BusCheck(new Vector3(926.0767, 6483.094, 19.65213), false), // 62 
                new BusCheck(new Vector3(1270.545, 6484.433, 18.93892), false), // 64 
                new BusCheck(new Vector3(1571.767, 6402.569, 23.46802), false), // 65 
                new BusCheck(new Vector3(1936.01, 6250.097, 42.06968), false), // 66 
                new BusCheck(new Vector3(2126.284, 6017.464, 49.66409), false), // 67 
                new BusCheck(new Vector3(2402.757, 5710.199, 43.88626), false), // 68 
                new BusCheck(new Vector3(2546.62, 5334.474, 43.07608), false), // 69 
                new BusCheck(new Vector3(2643.089, 4966.605, 43.23901), false), // 70 
                new BusCheck(new Vector3(2760.365, 4427.607, 46.93263), false), // 71 
                new BusCheck(new Vector3(2710.661, 4383.659, 46.01754), false), // 72 
                new BusCheck(new Vector3(2502.408, 4118.359, 36.95085), false), // 73 
                new BusCheck(new Vector3(2284.848, 3852.638, 33.02863), false), // 74 
                new BusCheck(new Vector3(2074.018, 3730.402, 31.54861), false), // 75 
                new BusCheck(new Vector3(2047.203, 3756.253, 30.8865), false), // 76 
                new BusCheck(new Vector3(1984.971, 3739.775, 30.95483), false), // 77 
                new BusCheck(new Vector3(1856.94, 3669.111, 32.49074), true), // 78 
                new BusCheck(new Vector3(1750.066, 3599.518, 33.45465), false), // 79 
                new BusCheck(new Vector3(1688.499, 3518.189, 34.85237), false), // 80 
                new BusCheck(new Vector3(1781.543, 3354.392, 38.96047), false), // 81 
                new BusCheck(new Vector3(2071.827, 3062.378, 44.71134), false), // 82 
                new BusCheck(new Vector3(2264.338, 3002.298, 44.05603), false), // 83 
                new BusCheck(new Vector3(2359.519, 2951.845, 47.47613), false), // 84 
                new BusCheck(new Vector3(2248.539, 2812.104, 41.71938), false), // 85 
                new BusCheck(new Vector3(1918.114, 2502.823, 53.05881), false), // 86 
                new BusCheck(new Vector3(1722.671, 1638.278, 81.25053), false), // 87 
                new BusCheck(new Vector3(1451.71, 769.3271, 75.82732), false), // 88 
                new BusCheck(new Vector3(1188.885, 490.0342, 80.19238), false), // 89 
                new BusCheck(new Vector3(707.7386, -114.3767, 51.56673), false), // 90 
                new BusCheck(new Vector3(522.7889, -396.2439, 30.5106), false), // 91 
                new BusCheck(new Vector3(363.8746, -645.1902, 27.79677), false), // 92 
                new BusCheck(new Vector3(381.0358, -678.856, 27.75814), false), // 93 
                new BusCheck(new Vector3(440.7911, -680.0443, 27.26101), false), // 94 
                new BusCheck(new Vector3(465.142, -655.6293, 26.28402), false), // 95 
                new BusCheck(new Vector3(470.0535, -604.1998, 27.00337), false), // 96 
            },*/
        };

        #region BusStations
        public static Dictionary<string, Vector3> BusStations = new Dictionary<string, Vector3>()
        {
            { "LSPD", new Vector3(394.8946, -990.8792, 30.60689) },
            { "Main Square", new Vector3(-528.8386, -328.6082, 36.34783) },
            { "FIB", new Vector3(-1621.519, -532.9644, 35.70459) },
            { "West Side", new Vector3(19.75618, -1533.853, 30.54906) },
            { "Airport", new Vector3(-1032.82, -2723.92, 14.99705) },
            { "Airport Hotel", new Vector3(-888.1733, -2186.11, 9.900888) },
            { "Driving School", new Vector3(-663.498, -1244.046, 11.90458) },
            { "Lawn Mower", new Vector3(-1354.111, -43.5153, 52.53339) },
            { "Power Station", new Vector3(740.4898, 100.5469, 81.29053) },
            { "Taxi", new Vector3(918.1192, -188.8451, 74.84467) },
            { "Truck Station", new Vector3(602.8403, -3018.18, 6.131153) },
            { "East Side", new Vector3(837.6479, -1807.675, 29.10327) },
            { "Collector", new Vector3(807.4769, -1195.802, 27.39124) },
            { "Mechanic", new Vector3(449.7962, -1249.931, 30.22602) },
            { "Chumash", new Vector3(-3104.435, 1097.097, 20.59407) },
            { "Paleto Bay", new Vector3(-151.3444, 6211.825, 31.31864) },
            { "Sandy Shores", new Vector3(1856.94, 3669.111, 34.11074) },
        };
        #endregion

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStartHandler()
        {
            try
            {
                for (int a = 0; a < BusWays.Count; a++)
                {
                    for (int x = 0; x < BusWays[a].Count; x++)
                    {
                        CustomColShape.CreateCylinderColShape(BusWays[a][x].Pos, 4, 3, 0, ColShapeEnums.BusWays, a, x);
                    }
                }

                foreach (var station in BusStations) 
                    NAPI.TextLabel.CreateTextLabel($"~w~Автобусная остановка\n~o~{station.Key}", station.Value, 30f, 0.4f, 0, new Color(255, 255, 255), true, 0);
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStartHandler Exception: {e.ToString()}");
            }
        }

        #region BusWays
        [Interaction(ColShapeEnums.BusWays, In: true)]
        public static void InBusWays(ExtPlayer player, int Index, int ListId)
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
                    if (vehicleLocalData.WorkId != JobsId.Bus) return;
                    if (characterData.WorkID != (int)JobsId.Bus || !sessionData.WorkData.OnWork || sessionData.WorkData.WorkWay != Index) return;
                    int way = sessionData.WorkData.WorkWay;
                    if (ListId != sessionData.WorkData.WorkCheck) return;
                    int check = sessionData.WorkData.WorkCheck;

                    if (sessionData.WorkData.BusOnStop) return;
                    if (!BusWays[way][check].IsStop)
                    {
                        if (sessionData.WorkData.WorkCheck != check) return;
                        if (check + 1 != BusWays[way].Count) check++;
                        else check = 0;

                        Vector3 direction = (check + 1 != BusWays[way].Count) ? BusWays[way][check + 1].Pos - new Vector3(0, 0, 0.12) : BusWays[way][0].Pos - new Vector3(0, 0, 1.12);
                        Color color = (BusWays[way][check].IsStop) ? new Color(255, 255, 255) : new Color(255, 0, 0);
                        Trigger.ClientEvent(player, "createCheckpoint", 3, 1, BusWays[way][check].Pos - new Vector3(0, 0, 1.12), 4, 0, color.Red, color.Green, color.Blue, direction);
                        Trigger.ClientEvent(player, "createWaypoint", BusWays[way][check].Pos.X, BusWays[way][check].Pos.Y);
                        Trigger.ClientEvent(player, "createWorkBlip", BusWays[way][check].Pos);
                        sessionData.WorkData.WorkCheck = check;
                        int payment = Convert.ToInt32(Main.BuswaysPayments[way] * Group.GroupPayAdd[accountData.VipLvl] * Main.ServerSettings.MoneyMultiplier);

                        (byte, float) jobLevelInfo = characterData.JobSkills.ContainsKey((int)JobsId.Bus) ? Main.GetPlayerJobLevelBonus((int)JobsId.Bus, characterData.JobSkills[(int)JobsId.Bus]) : (0, 1);
                        if (jobLevelInfo.Item1 >= 1) payment = Convert.ToInt32(payment * jobLevelInfo.Item2);

                        MoneySystem.Wallet.Change(player, payment);
                        GameLog.Money($"server", $"player({characterData.UUID})", payment, $"busCheck");
                        BattlePass.Repository.UpdateReward(player, 64);
                        BattlePass.Repository.UpdateReward(player, 160);

                        if (characterData.JobSkills.ContainsKey((int)JobsId.Bus))
                        {
                            if (characterData.JobSkills[(int)JobsId.Bus] < 70000)
                                characterData.JobSkills[(int)JobsId.Bus] += 1;
                        }
                        else characterData.JobSkills.Add((int)JobsId.Bus, 1);
                        
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
                    }
                    else
                    {
                        if (sessionData.WorkData.WorkCheck != check) return;
                        Trigger.ClientEvent(player, "deleteCheckpoint", 3, 0);
                        Trigger.ClientEvent(player, "deleteWorkBlip");
                        Trigger.ClientEvent(player, "freeze", true);
                        sessionData.WorkData.BusOnStop = true;
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BusStop), 6000);
                        var mypos = player.Position;
                        sessionData.TimersData.BusTimer = Timers.StartOnce(5000, () => timer_busStop(player, way, check, mypos), true);
                        foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(mypos, 30))
                        {
                            if (!foreachPlayer.IsCharacterData()) continue;
                            Trigger.SendChatMessage(foreachPlayer, LangFunc.GetText(LangType.Ru, DataName.NextBus, BusWaysNames[way]));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"busCheckpointEnterWay Exception: {e.ToString()}");
            }
        }

        private static void timer_busStop(ExtPlayer player, int way, int check, Vector3 mypos)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (sessionData.TimersData.BusTimer != null)
                {
                    Timers.Stop(sessionData.TimersData.BusTimer);
                    sessionData.TimersData.BusTimer = null;
                }
                sessionData.WorkData.BusOnStop = false;
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCanGoNext), 3000);
                int payment = Convert.ToInt32(Main.BuswaysPayments[way] * Group.GroupPayAdd[accountData.VipLvl] * Main.ServerSettings.MoneyMultiplier);

                (byte, float) jobLevelInfo = characterData.JobSkills.ContainsKey((int)JobsId.Bus) ? Main.GetPlayerJobLevelBonus((int)JobsId.Bus, characterData.JobSkills[(int)JobsId.Bus]) : (0, 1);
                if (jobLevelInfo.Item1 >= 1) payment = Convert.ToInt32(payment * jobLevelInfo.Item2);

                MoneySystem.Wallet.Change(player, payment);
                GameLog.Money($"server", $"player({characterData.UUID})", payment, $"busCheck");
                if (check + 1 != BusWays[way].Count) check++;
                else check = 0;
                Vector3 direction = (check + 1 < BusWays[way].Count) ? BusWays[way][check + 1].Pos - new Vector3(0, 0, 0.12) : BusWays[way][0].Pos - new Vector3(0, 0, 1.12);
                Color color = (BusWays[way][check].IsStop) ? new Color(255, 255, 255) : new Color(255, 0, 0);
                Trigger.ClientEvent(player, "createCheckpoint", 3, 1, BusWays[way][check].Pos - new Vector3(0, 0, 1.12), 4, 0, color.Red, color.Green, color.Blue, direction);
                Trigger.ClientEvent(player, "createWaypoint", BusWays[way][check].Pos.X, BusWays[way][check].Pos.Y);
                Trigger.ClientEvent(player, "createWorkBlip", BusWays[way][check].Pos);
                
                Trigger.ClientEvent(player, "freeze", false);
                sessionData.WorkData.WorkCheck = check;

                if (characterData.JobSkills.ContainsKey((int)JobsId.Bus))
                {
                    if (characterData.JobSkills[(int)JobsId.Bus] < 70000)
                        characterData.JobSkills[(int)JobsId.Bus] += 1;
                }
                else characterData.JobSkills.Add((int)JobsId.Bus, 1);
                
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

                foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(mypos, 30))
                {
                    if (!foreachPlayer.IsCharacterData()) continue;
                    Notify.Send(foreachPlayer, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BusStartsWay, BusWaysNames[way]), 1000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"timer_busStop Exception: {e.ToString()}");
            }
        }
        #endregion


        
        public static bool EndWork(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return false;
            
            if (sessionData.WorkData.OnWork)
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.EndWorkDay), 3000);
                sessionData.WorkData.OnWork = false;
                Trigger.ClientEvent(player, "deleteCheckpoint", 3, 0);
                Trigger.ClientEvent(player, "deleteWorkBlip");
                return true;
            }
            return false;
        }

        public static void StartWork(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            var ways = new Dictionary<int, int>
            {
                { 0, 0 },
                { 1, 0 },
                { 2, 0 },
            };
            foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
            {
                var foreachSessionData = foreachPlayer.GetSessionData();
                if (foreachSessionData == null) continue;
                var foreachCharacterData = foreachPlayer.GetCharacterData();
                if (foreachCharacterData == null) continue;
                if (foreachCharacterData.WorkID != (int)JobsId.Bus || !foreachSessionData.WorkData.OnWork) continue;
                ways[foreachSessionData.WorkData.WorkWay]++;
            }

            int way = -1;
            for (int i = 0; i < ways.Count; i++)
                if (ways[i] == 0)
                {
                    way = i;
                    break;
                }
            if (way == -1)
            {
                for (int i = 0; i < ways.Count; i++)
                    if (ways[i] == 1)
                    {
                        way = i;
                        break;
                    }
            }
            if (way == -1) way = 0;
            
            sessionData.WorkData.OnWork = true;
            sessionData.WorkData.WorkWay = way;
            sessionData.WorkData.WorkCheck = 0;
            
            Trigger.ClientEvent(player, "createCheckpoint", 3, 1, BusWays[way][0].Pos - new Vector3(0, 0, 1.12), 4, 0, 255, 0, 0, BusWays[way][1].Pos - new Vector3(0, 0, 1.12));
            Trigger.ClientEvent(player, "createWaypoint", BusWays[way][0].Pos.X, BusWays[way][0].Pos.Y);
            Trigger.ClientEvent(player, "createWorkBlip", BusWays[way][0].Pos);
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouRentBus, BusWaysNames[way]), 10000);
        }

        [ServerEvent(Event.PlayerEnterVehicle)]
        public static void OnPlayerEnterVehicle(ExtPlayer player, ExtVehicle vehicle, sbyte seatId)
        {
            if (seatId == (int)VehicleSeat.Driver)
                return;
            
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            var vehicleLocalData = vehicle.GetVehicleLocalData();
            if (vehicleLocalData == null || vehicleLocalData.WorkId != JobsId.Bus)
                return;
            
            if (characterData.Money >= Main.BusPay)
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BusPayed, Main.BusPay), 3000);
                BattlePass.Repository.UpdateReward(player, 63);
                MoneySystem.Wallet.Change(player, -Main.BusPay);
                
                var fractionData = Fractions.Manager.GetFractionData((int) Fractions.Models.Fractions.CITY);
                if (fractionData != null)
                    fractionData.Money += Main.BusPay;
                
                GameLog.Money($"player({characterData.UUID})", $"frac(6)", Main.BusPay, $"busPay");
            }
            else
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMoney), 3000);
                VehicleManager.WarpPlayerOutOfVehicle(player);
            }
        }
        
        internal class BusCheck
        {
            public Vector3 Pos { get; }
            public bool IsStop { get; }

            public BusCheck(Vector3 pos, bool isStop = false)
            {
                Pos = pos;
                IsStop = isStop;
            }
        }
    }
}
