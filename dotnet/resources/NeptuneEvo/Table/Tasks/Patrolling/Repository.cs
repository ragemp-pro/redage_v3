using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using NeptuneEvo.Table.Tasks.Models;
using NeptuneEvo.Table.Tasks.Patrolling.Models;
using NeptuneEvo.Table.Tasks.Player;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;
using Redage.SDK;

namespace NeptuneEvo.Table.Tasks.Patrolling
{
    public class Repository
    {
        public static List<PatrollingData> Patrollings = new List<PatrollingData>()
        {
            new PatrollingData(new Vector3(-1256.225, -619.0381, 27.727465), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1221.8608, -613.9773, 27.525997), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1117.2256, -737.4412, 20.623884), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1145.1082, -809.20337, 15.982467), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1232.8396, -875.3649, 13.2389345), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1293.7001, -875.4408, 12.447105), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1350.5137, -820.1451, 18.570812), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1409.9117, -822.7926, 19.142765), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1552.2413, -746.20764, 19.65934), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1722.2609, -636.146, 12.262419), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1894.1494, -513.8388, 12.388817), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-2054.2207, -386.6347, 11.76947), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-2131.4531, -260.39923, 15.861945), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1923.3732, -262.78586, 40.565487), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1889.3217, -388.33206, 49.178417), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1728.5354, -527.9081, 38.12846), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1560.5059, -652.8994, 29.846855), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1485.6525, -630.10535, 31.065708), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1395.6578, -574.8828, 30.878578), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1314.813, -525.18713, 33.512352), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1273.3303, -553.7559, 30.985994), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1146.0326, -687.68994, 22.392057), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1093.069, -670.1411, 20.906174), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-854.2096, -557.8402, 22.630304), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-666.24115, -553.05695, 35.23358), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-623.7398, -528.7007, 35.350796), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-621.4846, -404.5589, 35.32605), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-585.6289, -322.15997, 35.52494), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-479.40784, -266.30548, 36.41178), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-426.14148, -199.7059, 36.90124), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-482.0592, -114.708855, 39.525856), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-585.38, -162.46625, 38.513783), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-701.75146, -221.83537, 37.651638), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-778.4432, -256.0173, 37.71119), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-804.5727, -280.76385, 37.521248), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-866.3745, -280.31992, 41.02385), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-942.88464, -306.42532, 39.71852), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1083.3418, -378.84534, 37.42112), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1234.2816, -433.3301, 34.138783), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1326.9275, -513.93945, 33.40167), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1510.5122, -641.054, 30.181929), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1514.51, -698.9061, 28.571983), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1462.9739, -734.75726, 25.160406), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1405.7926, -729.2114, 24.266943), Fractions.Models.Fractions.CITY, false),
            new PatrollingData(new Vector3(-1328.2487, -672.1874, 27.137543), Fractions.Models.Fractions.CITY, false),



            new PatrollingData(new Vector3(-1220.3221, -498.48834, 81.724266), Fractions.Models.Fractions.CITY, true),
            new PatrollingData(new Vector3(-1349.5498, -315.19705, 157.5889), Fractions.Models.Fractions.CITY, true),
            new PatrollingData(new Vector3(-1563.0667, -452.24637, 208.1591), Fractions.Models.Fractions.CITY, true),
            new PatrollingData(new Vector3(-1453.0913, -761.51404, 172.36453), Fractions.Models.Fractions.CITY, true),
            new PatrollingData(new Vector3(-1234.1475, -774.96106, 162.04906), Fractions.Models.Fractions.CITY, true),
            new PatrollingData(new Vector3(-1277.2047, -559.64014, 130.72426), Fractions.Models.Fractions.CITY, true),
            new PatrollingData(new Vector3(-1543.1953, -474.69946, 132.6278), Fractions.Models.Fractions.CITY, true),
            new PatrollingData(new Vector3(-1622.3517, -267.15186, 178.62508), Fractions.Models.Fractions.CITY, true),
            new PatrollingData(new Vector3(-1336.7808, -164.14783, 198.65335), Fractions.Models.Fractions.CITY, true),
            new PatrollingData(new Vector3(-962.5798, -188.27383, 206.66101), Fractions.Models.Fractions.CITY, true),
            new PatrollingData(new Vector3(-637.61237, -297.4172, 170.41583), Fractions.Models.Fractions.CITY, true),
            new PatrollingData(new Vector3(-516.44696, -167.15787, 134.47832), Fractions.Models.Fractions.CITY, true),
            new PatrollingData(new Vector3(-689.80566, -116.51478, 148.64441), Fractions.Models.Fractions.CITY, true),
            new PatrollingData(new Vector3(-956.61383, -291.4547, 155.28107), Fractions.Models.Fractions.CITY, true),
            new PatrollingData(new Vector3(-1022.7365, -704.8358, 159.96431), Fractions.Models.Fractions.CITY, true),
            new PatrollingData(new Vector3(-1295.4429, -1017.6532, 129.7165), Fractions.Models.Fractions.CITY, true),
            new PatrollingData(new Vector3(-1558.5917, -794.69586, 146.5576), Fractions.Models.Fractions.CITY, true),
            new PatrollingData(new Vector3(-1598.921, -496.11444, 162.53188), Fractions.Models.Fractions.CITY, true),
            new PatrollingData(new Vector3(-1367.4949, -535.2654, 141.49738), Fractions.Models.Fractions.CITY, true),

            new PatrollingData(new Vector3(2587.8704, -323.72418, 93.035995), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(2608.2544, -513.911, 75.76144), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(2559.7397, -594.401, 65.07494), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(2509.6414, -652.0637, 61.652615), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(2411.1306, -681.71625, 63.1859), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(2191.0051, -769.05743, 69.44921), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(2048.1372, -883.9932, 79.27947), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(1832.0898, -1061.0728, 79.616325), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(1766.88, -1244.1635, 84.33163), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(1666.2559, -1322.5468, 84.432144), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(1469.1992, -1504.7113, 65.20662), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(1349.2745, -1593.0342, 52.51211), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(1272.7163, -1657.0782, 48.241936), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(1063.0603, -1743.3064, 35.87504), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(974.2689, -1762.6711, 31.44721), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(950.4489, -1828.6461, 31.372053), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(933.8727, -2030.5862, 30.361368), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(921.6136, -2149.8652, 30.50483), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(915.17505, -2224.6343, 30.397541), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(870.294, -2240.9885, 30.579805), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(790.0431, -2234.2156, 29.504557), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(780.58594, -2187.2495, 29.372503), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(783.19305, -2091.5928, 29.413877), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(738.6969, -2051.3455, 29.411428), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(588.6648, -2037.9595, 29.395096), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(496.38977, -2048.073, 26.071198), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(419.00793, -2119.795, 19.397558), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(315.12546, -2136.7659, 14.801137), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(234.1374, -2064.3174, 18.027868), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(122.216034, -2032.6415, 18.427742), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(-22.087063, -2037.1726, 19.215187), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(-167.51878, -2090.139, 25.325207), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(-339.71677, -2081.411, 24.401894), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(-453.83853, -1962.5938, 24.85627), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(-426.15622, -1873.2451, 19.288328), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(-335.4782, -1837.6405, 23.653679), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(-196.71158, -1805.4279, 30.017656), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(-112.05157, -1764.5265, 29.890991), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(24.339111, -1876.9843, 23.051403), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(96.65561, -1864.3822, 24.51664), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(187.37904, -1757.0273, 29.013702), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(268.72293, -1685.4288, 29.39796), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(324.83923, -1713.0541, 29.439709), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(388.29843, -1747.0453, 29.428438), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(451.6025, -1671.8348, 29.313892), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(542.44495, -1557.1002, 29.379076), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(544.76733, -1530.188, 29.356552), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(534.24677, -1477.5365, 29.355875), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(531.1754, -1339.013, 29.427523), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(505.0487, -1267.0164, 29.391926), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(423.65958, -1255.7369, 31.788408), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(345.5041, -1315.5521, 32.424137), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(299.26834, -1369.3699, 32.049805), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(248.43288, -1430.9082, 29.328766), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(178.04486, -1406.9456, 29.49391), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(131.39915, -1412.8684, 29.486158), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(72.70846, -1490.5825, 29.475912), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(-11.677128, -1583.6416, 29.491074), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(-102.422516, -1683.3527, 29.414106), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(-150.92145, -1711.6226, 30.386606), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(-181.24043, -1620.037, 33.658638), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(-135.39761, -1547.5934, 34.418423), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(-161.36505, -1477.4332, 32.70782), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(-164.0326, -1406.2332, 30.787176), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(-52.43073, -1374.6323, 29.471298), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(50.56044, -1375.4227, 29.379879), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(177.8457, -1413.9873, 29.473156), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(303.97656, -1501.5829, 29.42988), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(448.80698, -1452.2335, 29.36154), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(520.6082, -1474.3246, 29.33916), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(597.16144, -1589.3113, 26.99817), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(592.04956, -1759.5514, 21.610128), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(521.812, -1892.2013, 25.546118), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(480.66412, -1835.962, 27.898905), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(495.83817, -1769.2604, 28.562916), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(531.5099, -1708.2988, 29.271553), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(734.4107, -1753.2323, 29.39763), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(882.35284, -1764.7212, 30.126637), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(986.85974, -1768.661, 31.552994), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(1082.5858, -1748.61, 35.892273), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(1122.8676, -1727.7513, 35.875443), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(1116.17, -1656.2091, 32.083534), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(1084.9058, -1412.0577, 29.7182), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(1092.915, -1302.6014, 36.69389), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(1235.471, -1192.1786, 48.0659), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(1461.9601, -1066.3457, 55.33236), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(1594.9803, -983.2419, 60.92069), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(1841.1002, -816.48376, 77.71873), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(2014.0717, -683.8356, 93.36545), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(2202.813, -539.33954, 93.49462), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(2405.4583, -466.37488, 71.82575), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(2530.9731, -580.9519, 66.38175), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(2562.4382, -602.9887, 64.914474), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(2602.1208, -479.64032, 82.648766), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(2594.7502, -350.37732, 93.01242), Fractions.Models.Fractions.FIB, false),
            new PatrollingData(new Vector3(2568.5625, -285.3868, 93.139755), Fractions.Models.Fractions.FIB, false),

            new PatrollingData(new Vector3(2615.3877, -459.32913, 162.08577), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(2403.072, -584.2457, 210.14536), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(2100.017, -737.74725, 252.26953), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(1684.2113, -926.2755, 277.42618), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(1173.384, -1119.7579, 310.67053), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(529.74286, -1261.6879, 317.1905), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(206.69823, -1440.8439, 282.94092), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(48.40742, -1679.5576, 278.55334), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(35.129177, -2103.179, 295.31445), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(385.58502, -2316.6426, 304.53326), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(802.00024, -2294.3076, 302.27667), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(1173.4783, -2064.7942, 248.64912), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(1263.4575, -1640.5392, 241.20265), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(1447.8599, -1364.4722, 236.45181), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(1697.4893, -1593.9614, 303.4415), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(1295.669, -2013.2178, 304.80994), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(962.1449, -2136.6023, 320.29507), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(271.05173, -2221.6763, 293.19125), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(-142.35625, -1940.7778, 281.7868), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(89.51672, -1656.8424, 295.5496), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(464.7904, -1561.048, 263.57858), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(738.9848, -1415.3324, 238.29695), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(1050.8033, -1521.3467, 235.47641), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(1355.318, -1562.2384, 244.14442), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(1731.696, -1253.2634, 233.53502), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(2058.516, -749.12103, 294.93246), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(2362.0164, -100.605545, 352.75598), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(2473.3115, 503.2376, 386.02853), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(2657.0222, 1227.7678, 414.019), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(2903.4539, 1903.4133, 333.93527), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(3151.9148, 2690.9365, 340.97772), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(3405.1008, 3296.3352, 363.83847), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(3619.6384, 4222.388, 382.70776), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(3717.9172, 4562.318, 354.11826), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(4009.8923, 4249.051, 353.32816), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(3313.1714, 3466.242, 378.3916), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(2912.3845, 2606.4949, 390.7772), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(2731.2996, 1998.13, 405.10458), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(2756.3936, 1281.3601, 224.13377), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(2729.664, 582.442, 235.21494), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(2705.285, 244.1118, 233.83148), Fractions.Models.Fractions.FIB, true),
            new PatrollingData(new Vector3(2584.9954, -131.68997, 188.99002), Fractions.Models.Fractions.FIB, true),

            new PatrollingData(new Vector3(405.79575, -950.13104, 29.544193), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(384.40274, -849.6404, 29.460854), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(255.6802, -836.6476, 29.675512), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(192.09918, -798.03503, 31.380325), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(235.55455, -679.12823, 37.340626), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(279.52628, -569.4007, 43.27357), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(329.92438, -451.27438, 43.7585), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(449.54776, -353.31342, 47.46356), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(494.34457, -290.453, 46.59822), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(517.32227, -145.92043, 58.101543), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(623.16364, -62.54233, 75.18666), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(653.9713, 37.344646, 86.18801), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(521.70197, 84.95867, 96.46903), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(364.38348, 140.98186, 103.28243), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(233.7253, 189.15807, 105.43508), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(132.16957, 226.7375, 107.45919), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(2.2810385, 266.87805, 109.308556), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-146.40004, 258.28952, 94.81204), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-328.46127, 254.92203, 86.577736), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-469.0351, 252.60968, 83.16853), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-617.18427, 271.34164, 81.787964), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-710.665, 251.41643, 80.38962), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-835.0287, 223.78731, 74.1838), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-959.27075, 263.7596, 69.671036), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-1209.0089, 245.72504, 67.824905), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-1382.3931, 207.46179, 58.843464), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-1435.5446, -36.928932, 52.804237), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-1524.6296, -135.78793, 52.742287), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-1625.1959, -270.44873, 52.98649), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-1788.9479, -312.6457, 43.792297), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-1997.152, -165.63522, 29.69407), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-2166.579, -318.33334, 13.178037), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-2132.2913, -371.54953, 13.129971), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-1991.4103, -465.53836, 11.667137), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-1740.979, -692.47784, 10.335727), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-1595.752, -783.0592, 11.740644), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-1396.0168, -828.38324, 19.141579), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-1407.981, -763.7598, 22.069786), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-1515.1504, -683.4277, 28.59727), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-1493.2777, -634.72375, 30.281631), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-1397.6532, -576.45776, 30.435358), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-1319.4304, -528.5588, 32.821396), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-1268.4808, -559.77576, 29.948454), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-1169.2819, -675.7398, 22.647078), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-1118.6882, -727.7626, 20.503237), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-1034.9261, -736.0162, 19.591213), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-878.6552, -665.9919, 27.914753), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-750.11475, -666.045, 30.37895), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-656.3889, -663.62726, 31.831333), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-623.11456, -580.8458, 34.70794), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-624.0508, -449.5713, 34.886555), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-592.8198, -325.32684, 35.023697), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-473.25452, -269.03238, 35.868763), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-418.86136, -289.30505, 35.53829), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-261.93622, -403.15875, 30.276148), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-232.08899, -571.91473, 34.66944), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-241.31047, -693.0808, 33.52899), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-275.84738, -813.77234, 31.850292), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-179.96681, -906.4806, 29.430235), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-50.372105, -956.47784, 29.441505), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-100.72259, -1120.7435, 25.894571), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-100.02754, -1332.4106, 29.478003), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(-17.60837, -1375.966, 29.392216), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(125.10415, -1402.6366, 29.333431), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(78.18155, -1492.2083, 29.454636), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(102.23006, -1542.8221, 29.420078), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(202.74962, -1585.4254, 29.452057), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(329.0025, -1536.8617, 29.285374), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(422.07678, -1614.4734, 29.311092), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(372.13348, -1720.2269, 29.345102), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(269.41953, -1851.7512, 26.962564), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(378.32693, -1918.4467, 24.611052), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(500.56345, -1747.6263, 28.798874), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(704.77704, -1749.855, 29.399097), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(858.1609, -1762.7234, 29.54166), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(953.1467, -1792.4792, 31.33528), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(933.8329, -2025.4478, 30.320726), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(916.1713, -2209.5945, 30.463636), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(901.8454, -2396.3147, 29.989588), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(992.5974, -2470.6138, 28.61989), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(1047.6965, -2314.6904, 30.55278), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(1078.116, -2092.367, 34.68709), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(1211.3242, -2062.5342, 44.363335), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(1218.7605, -1932.1528, 38.642147), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(1162.8843, -1732.1437, 35.460114), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(1324.042, -1625.2673, 52.310112), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(1675.2811, -1325.0458, 84.66304), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(1979.2717, -917.36694, 79.24974), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(2388.4307, -688.6832, 63.38377), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(2475.8284, -511.48764, 69.40011), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(2218.5344, -445.60938, 83.75212), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(1456.4261, -1037.2709, 55.67255), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(367.89865, -1187.4032, 39.40922), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(201.26462, -1163.1237, 38.295525), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(81.959496, -1085.4922, 29.389835), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(156.04373, -1029.1683, 29.392017), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(319.95624, -1059.6847, 29.369957), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(395.85782, -1102.3478, 29.49953), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(478.1332, -1134.693, 29.498844), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(503.01398, -1038.462, 28.40939), Fractions.Models.Fractions.POLICE, false),
            new PatrollingData(new Vector3(469.73404, -951.0872, 27.81706), Fractions.Models.Fractions.POLICE, false),

            new PatrollingData(new Vector3(447.4234, -1052.8944, 79.26001), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(232.00293, -1266.8524, 119.492935), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(113.67455, -1583.5947, 142.25027), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(155.47581, -1813.4775, 146.25), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(402.51105, -1999.0619, 135.82672), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(710.10974, -2162.6404, 157.47098), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(1016.523, -2081.224, 157.43584), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(1155.951, -1751.4386, 182.29333), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(945.51764, -1519.4039, 178.45274), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(390.78937, -1351.3792, 192.1405), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(-153.0315, -1182.2238, 218.31299), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(-643.89075, -1153.7217, 242.33711), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(-1110.2135, -1139.6404, 161.29143), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(-1494.8417, -932.0581, 175.5284), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(-1786.151, -644.8892, 175.00935), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(-1773.5159, -344.82724, 174.90718), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(-1632.5208, -27.611002, 204.66183), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(-1399.5554, 374.20297, 277.19138), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(-1000.60297, 508.87808, 281.85645), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(-433.7258, 426.14124, 287.34882), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(-5.9694533, 66.62714, 305.86322), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(-0.65422887, -362.68286, 321.16125), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(-237.28537, -572.1071, 322.78363), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(-630.05524, -501.48517, 282.80014), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(-1124.4049, -455.56717, 236.92079), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(-1232.3433, -773.9763, 222.76161), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(-618.12915, -1017.1297, 216.35843), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(-127.670105, -1095.5613, 228.70741), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(330.80554, -932.4854, 234.63297), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(902.56116, 106.94358, 262.12546), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(772.3645, 487.73227, 370.51892), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(131.16353, 37.834328, 417.81357), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(-142.34837, -564.46954, 401.0163), Fractions.Models.Fractions.POLICE, true),
            new PatrollingData(new Vector3(217.92741, -941.92035, 232.74593), Fractions.Models.Fractions.POLICE, true),

            new PatrollingData(new Vector3(-435.78445, 6039.327, 31.507017), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-412.61078, 6002.9917, 31.723356), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-506.63486, 5839.2812, 34.352345), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-676.1582, 5563.0376, 38.61126), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-936.7863, 5419.5073, 38.133423), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-1042.827, 5352.1626, 43.790836), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-1227.3765, 5268.103, 50.368282), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-1385.5404, 5102.8115, 61.327816), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-1589.6523, 4917.3916, 61.51922), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-1815.1758, 4710.212, 57.17115), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-1989.1313, 4535.5815, 57.193897), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-2198.298, 4385.116, 55.04932), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-2268.13, 4244.123, 44.05079), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-2334.9177, 4096.516, 34.318424), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-2434.3096, 3842.0745, 23.513044), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-2546.675, 3470.0784, 13.70308), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-2596.7063, 3186.28, 14.189835), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-2656.3235, 2698.895, 16.823923), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-2691.6538, 2457.6228, 16.822334), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-2720.8137, 2284.449, 19.321999), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-2582.4685, 2282.826, 30.162163), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-2379.8823, 2257.557, 33.1886), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-2095.6719, 2309.0474, 37.7643), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-1613.742, 2414.9814, 26.279442), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-1281.5988, 2504.8267, 20.73912), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-1066.1753, 2703.2893, 21.441378), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-889.29694, 2747.0261, 23.763391), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-670.0458, 2833.397, 29.861536), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-414.38754, 2859.8787, 38.462803), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-156.54396, 2852.378, 49.051365), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(88.53969, 2709.0332, 54.480194), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(271.41785, 2628.9495, 44.80214), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(274.1446, 2687.3914, 44.39343), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(232.59181, 2973.629, 42.823956), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(219.09012, 3172.857, 42.648033), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(342.5356, 3447.471, 36.057682), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(589.2299, 3508.1836, 34.298233), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(946.054, 3533.931, 34.182854), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(1236.5774, 3533.9392, 35.337925), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(1687.8175, 3499.4612, 36.602943), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(1702.4714, 3572.1174, 35.75426), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(1869.799, 3669.1414, 33.945824), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(2044.6547, 3745.9429, 32.694878), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(2242.522, 3820.661, 34.210323), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(2492.9087, 4088.4153, 38.144405), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(2753.9326, 4394.904, 49.08089), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(2785.618, 4480.214, 47.676727), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(2693.1047, 4870.584, 44.812122), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(2620.5461, 5112.963, 44.961815), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(2448.034, 5127.2114, 47.153122), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(2234.1533, 5201.8354, 61.277824), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(1973.7482, 5142.141, 43.414673), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(2026.266, 5055.8403, 41.86369), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(2152.993, 4924.482, 40.974445), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(2202.6494, 4940.583, 41.180515), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(2389.331, 5129.71, 47.51378), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(2577.186, 5095.751, 44.915466), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(2610.054, 5215.438, 44.859966), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(2493.5417, 5581.8613, 44.989563), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(2336.6506, 5883.064, 47.76558), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(2112.271, 6096.4404, 51.211536), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(1866.3561, 6356.6377, 41.467262), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(1601.7762, 6416.692, 26.387074), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(1215.1986, 6494.7744, 21.012018), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(827.4015, 6502.7095, 22.90509), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(597.04407, 6544.1606, 28.258755), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(345.20547, 6579.3105, 28.686556), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(163.55682, 6547.252, 32.061356), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(89.033966, 6597.265, 31.69093), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-10.954446, 6635.7886, 31.237652), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-166.499, 6496.158, 29.869123), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-151.36891, 6455.022, 31.526047), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-132.91626, 6402.582, 31.574314), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-212.92207, 6325.3115, 31.586805), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-380.28226, 6159.494, 31.467808), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-448.5584, 6074.729, 31.56399), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-580.1345, 6099.1177, 9.577941), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-669.1382, 5997.317, 11.579393), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-707.2044, 5824.845, 17.335377), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-778.2251, 5651.6177, 24.395594), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-780.172, 5510.0996, 34.809193), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-688.65576, 5536.5435, 38.020756), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-544.789, 5727.5137, 36.90692), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-321.9298, 6047.3345, 31.328764), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-239.1121, 6136.8657, 31.366148), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-250.78296, 6181.5894, 31.52383), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-320.07416, 6219.9346, 31.490904), Fractions.Models.Fractions.SHERIFF, false),
            new PatrollingData(new Vector3(-424.5723, 6114.294, 31.985092), Fractions.Models.Fractions.SHERIFF, false),

            new PatrollingData(new Vector3(-391.6895, 6049.8545, 84.425995), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(-225.21538, 6238.1426, 123.72655), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(56.00207, 6526.4, 133.64372), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(832.1185, 6599.836, 202.79483), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(1388.9423, 6472.7017, 182.81348), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(2321.174, 5750.7246, 335.21252), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(2420.1772, 5274.357, 338.58658), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(2663.771, 4599.8604, 319.92807), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(2463.7527, 4028.356, 325.7998), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(1993.2362, 3767.7527, 302.11948), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(1833.775, 3315.5828, 300.33875), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(1946.9122, 2933.3337, 313.0762), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(1756.0028, 2062.2588, 363.10907), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(1577.8777, 1317.6204, 373.36563), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(1781.1454, 774.47034, 392.00214), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(2131.3672, 661.081, 400.23), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(2457.4163, 1179.6558, 383.97168), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(2545.161, 2322.7798, 415.90155), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(2706.0708, 4393.1333, 415.59424), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(2207.5535, 4873.363, 349.6191), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(1468.2725, 4507.2803, 331.6812), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(735.1231, 4307.7393, 302.80655), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(-163.30402, 4439.337, 359.67868), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(-1026.6797, 4508.297, 401.14377), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(-1089.6853, 4943.7803, 370.72287), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(-560.32513, 5629.6733, 383.79056), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(-337.87354, 6110.58, 131.24107), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(-123.263756, 6560.3555, 128.96007), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(-91.34745, 6768.1987, 143.32439), Fractions.Models.Fractions.SHERIFF, true),
            new PatrollingData(new Vector3(-252.48663, 6534.621, 160.19574), Fractions.Models.Fractions.SHERIFF, true),

            new PatrollingData(new Vector3(-2296.6648, 3296.6392, 33.184296), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2254.3242, 3340.5566, 33.251175), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2169.6182, 3364.2, 33.432278), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1984.64, 3278.1184, 33.26332), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1866.3154, 3210.4468, 33.255733), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1703.181, 3041.3127, 33.235607), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1632.937, 2846.415, 22.813646), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1506.643, 2714.632, 17.966871), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1322.4175, 2550.4465, 18.243233), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1297.7814, 2516.284, 21.153284), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1152.725, 2628.8503, 16.279453), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-915.6758, 2749.064, 24.647512), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-502.42432, 2841.6912, 34.199123), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-407.4791, 2887.7878, 36.98719), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-312.06577, 2943.7273, 28.939219), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-542.3082, 3004.5688, 27.087965), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-742.69366, 2948.7014, 26.170906), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-927.79, 2875.0596, 23.693384), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1106.4556, 2865.9954, 14.406521), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1388.3182, 2699.9688, 5.089145), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1587.4458, 2729.9148, 5.9768724), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1763.1683, 2726.7954, 5.5561495), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1925.4122, 2708.563, 4.328912), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2200.956, 2781.1558, 5.1815248), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2475.5068, 2838.694, 4.064602), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2647.2593, 2952.4507, 9.066629), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2853.4744, 3156.019, 10.848058), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2998.8284, 3429.9163, 10.008521), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2797.084, 3505.371, 9.634998), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2603.969, 3480.7837, 15.06531), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2499.218, 3663.9731, 13.643582), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2452.3884, 3648.0425, 14.463591), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2391.4583, 3498.889, 22.200466), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2314.8318, 3391.5183, 31.223684), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2260.2488, 3372.952, 33.299232), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2071.1516, 3344.3135, 32.622623), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1822.4849, 3318.9983, 31.240688), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1685.0353, 3182.9634, 31.786837), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1711.0387, 3093.258, 33.301743), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1716.9033, 3007.1328, 33.456696), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1741.0425, 2950.309, 33.144703), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1703.0844, 2864.6658, 33.238026), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1706.0093, 2834.5847, 32.10231), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1823.0918, 2781.9463, 30.78578), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2067.1406, 2815.8042, 33.15538), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2107.9229, 2842.4487, 33.144737), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2201.5867, 2897.6907, 33.143833), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2374.5205, 2998.0557, 33.134335), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2436.7402, 2991.59, 33.16533), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2419.704, 3009.0095, 33.165333), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2627.5457, 3143.6436, 33.13506), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2718.4045, 3221.1975, 33.15904), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2672.011, 3303.3142, 33.15085), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2543.678, 3304.87, 33.144478), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2410.767, 3340.4316, 33.175755), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2347.3232, 3311.7812, 33.174797), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2359.26, 3363.7793, 33.169155), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2350.79, 3319.5383, 33.185783), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2296.1082, 3297.3477, 33.171486), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2316.6145, 3244.535, 33.182068), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2267.7458, 3195.3503, 33.168484), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2187.7007, 3218.4382, 33.146114), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1977.0166, 3098.6956, 33.12601), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1863.3221, 3034.7778, 33.15768), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-1900.6365, 2969.5415, 33.16337), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2049.7795, 3047.1492, 33.13045), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2240.2341, 3158.129, 33.16112), Fractions.Models.Fractions.ARMY, false),
            new PatrollingData(new Vector3(-2309.7207, 3226.398, 33.178524), Fractions.Models.Fractions.ARMY, false),

            new PatrollingData(new Vector3(-2277.5505, 3338.1738, 91.71429), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-2431.8975, 3479.7378, 106.215576), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-2666.2122, 3395.7527, 134.3259), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-2688.7383, 3095.8047, 148.4455), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-2341.599, 2775.842, 177.56985), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-1912.1864, 2642.4285, 183.37144), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-1428.8734, 2671.7551, 199.39467), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-1042.9724, 2804.635, 224.00052), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-692.20416, 3112.041, 354.85803), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-770.5782, 3464.2798, 459.69977), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-1361.4668, 3291.618, 446.5243), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-1784.2898, 3346.33, 373.4775), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-2052.7742, 3795.5562, 368.18988), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-2027.0206, 4409.1987, 384.96365), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-2288.5469, 4221.686, 452.24057), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-2405.2107, 3671.1658, 351.46582), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-2482.826, 3315.577, 282.9366), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-2266.2288, 3024.8623, 179.64745), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-1899.4855, 2907.427, 125.72236), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-1803.3402, 3096.7312, 148.03017), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-2135.5808, 3359.6653, 156.57767), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-2425.977, 3287.6086, 139.09666), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-2476.207, 3026.8855, 157.77496), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-1928.0222, 2792.3298, 179.42023), Fractions.Models.Fractions.ARMY, true),
            new PatrollingData(new Vector3(-2030.6284, 3109.938, 147.45532), Fractions.Models.Fractions.ARMY, true),
        };

        public static bool IsFractionPatrolling(Fractions.Models.Fractions fraction, bool isAir) =>
            Patrollings.Any(p => p.Fraction == fraction && p.IsAir == isAir);
        
        public static void ResourceInit()
        {
            try
            {
                var index = 0;
                foreach (var patrolling in Patrollings)
                {
                    if (!patrolling.IsAir)
                        CustomColShape.CreateCylinderColShape(patrolling.Position, 4, 3, 0, ColShapeEnums.Patrolling, index);
                    else
                        CustomColShape.CreateSphereColShape(patrolling.Position, 15, 0, ColShapeEnums.Patrolling, index);
                    index++;
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public static int Payment = 400;

        [Interaction(ColShapeEnums.Patrolling, In: true)]
        public void InPatrolling(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                if (!NAPI.Player.IsPlayerInAnyVehicle(player)) 
                    return;
                
                var vehicle = (ExtVehicle) player.Vehicle;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData == null)
                    return;
                
                var fracId = player.GetFractionId();
                if (vehicleLocalData.Access != VehicleAccess.Fraction || vehicleLocalData.Fraction != fracId)
                    return;
                
                var tableTaskData = sessionData.TableTaskData;
                if (!tableTaskData.IsPatrolling)
                    return;
                if (tableTaskData.PatrollingIndex != index)
                    return;

                if (Patrollings.Count <= index)
                    return;
                
                var patrollingData = Patrollings[index];
                var isEnd = Patrollings.Count <= (index + 1) || Patrollings[(index + 1)].Fraction != patrollingData.Fraction || Patrollings[(index + 1)].IsAir != patrollingData.IsAir;
                
                var color = new Color(255, 0, 0);

                if (isEnd)
                {
                    if (patrollingData.IsAir)
                    {   
                        Fractions.Table.Tasks.Repository.MissionAdd(player, 1);
                        player.AddTableScore(TableTaskId.Item4);
                    }
                    else
                    {
                        Fractions.Table.Tasks.Repository.MissionAdd(player, 0);
                        player.AddTableScore(TableTaskId.Item3);
                    }

                    tableTaskData.IsPatrolling = false;
                    tableTaskData.PatrollingIndex = 0;
                    Trigger.ClientEvent(player, "deleteCheckpoint", 9);
                    Trigger.ClientEvent(player, "deleteWorkBlip");
                    
                    MoneySystem.Wallet.Change(player, Payment);
                    GameLog.Money("server", $"player({player.GetUUID()})", Payment, "Patrolling");
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы закончили патруль.", 7000);
                }
                else
                {
                    index += 1;
                    
                    patrollingData = Patrollings[index];
                    tableTaskData.PatrollingIndex = index;
                    
                    var direction = new Vector3();

                    var type = patrollingData.IsAir ? 12 : 1;

                    if (Patrollings.Count <= (index + 1) ||
                        Patrollings[(index + 1)].Fraction != patrollingData.Fraction ||
                        Patrollings[(index + 1)].IsAir != patrollingData.IsAir)
                    {
                        var firstIndex = Patrollings.FindIndex(p => p.Fraction == patrollingData.Fraction && p.IsAir == patrollingData.IsAir);
                        direction = Patrollings[firstIndex].Position - new Vector3(0, 0, 1.12);
                        type = patrollingData.IsAir ? 14 : 4;
                    }
                    else
                    {
                        direction = Patrollings[(index + 1)].Position - new Vector3(0, 0, 1.12);
                    }
                    
                    if (patrollingData.IsAir) 
                        Trigger.ClientEvent(player, "createCheckpoint", 9, type, patrollingData.Position, 15, 0, color.Red, color.Green, color.Blue, direction);
                    else
                        Trigger.ClientEvent(player, "createCheckpoint", 9, type, patrollingData.Position - new Vector3(0, 0, 1.12), 4, 0, color.Red, color.Green, color.Blue, direction);
                    
                    Trigger.ClientEvent(player, "createWaypoint", patrollingData.Position.X, patrollingData.Position.Y);
                    Trigger.ClientEvent(player, "createWorkBlip", patrollingData.Position);
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public static bool CreatePatrolling(ExtPlayer player, bool isAir)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return false;

                var tableTaskData = sessionData.TableTaskData;
                
                var fractionData = player.GetFractionData();
                if (fractionData == null)
                    return false;
                
                
                var index = Patrollings.FindIndex(p => p.Fraction == (Fractions.Models.Fractions) fractionData.Id && p.IsAir == isAir);

                if (Patrollings.Count <= index)
                    return false;

                tableTaskData.IsPatrolling = true;
                tableTaskData.PatrollingIndex = index;
                
                var color = new Color(255, 0, 0);
                var patrollingData = Patrollings[index];
                
                if (patrollingData.IsAir) 
                    Trigger.ClientEvent(player, "createCheckpoint", 9, 12, patrollingData.Position, 15, 0, color.Red, color.Green, color.Blue, Patrollings[(index + 1)].Position);
                else
                    Trigger.ClientEvent(player, "createCheckpoint", 9, 1, patrollingData.Position - new Vector3(0, 0, 1.12), 4, 0, color.Red, color.Green, color.Blue, Patrollings[(index + 1)].Position);
                    
                Trigger.ClientEvent(player, "createWaypoint", patrollingData.Position.X, patrollingData.Position.Y);
                Trigger.ClientEvent(player, "createWorkBlip", patrollingData.Position);
                
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы взяли задание, метка уже стоит на карте.", 7000);
                return true;

            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }

            return false;
        }

    }
}