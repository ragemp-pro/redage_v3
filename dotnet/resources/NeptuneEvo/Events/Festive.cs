using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using Redage.SDK;
using System;

using Newtonsoft.Json;
using NeptuneEvo.Functions;
using System.Linq;
using Localization;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using Repository = NeptuneEvo.Chars.Repository;

namespace NeptuneEvo.Events
{
    class GiftsListModel
    {
        public int Id { set; get; } = 0;
        public Vector3 Position { set; get; } = new Vector3();
        public Vector3 Rotation { set; get; } = new Vector3();

        public GiftsListModel(int Id, Vector3 Position, Vector3 Rotation)
        {
            this.Id = Id;
            this.Position = Position;
            this.Rotation = Rotation;
        }
    }
    class Festive : Script
    {
        private static readonly nLog Log = new nLog("Events.Festive");

        
        public static DateTime DateTimeEnd = new DateTime(2023, 1, 30);
        
        public static bool isEvent = DateTime.Now < DateTimeEnd ? true : false;
        
        public static ItemId EventCoins = ItemId.Giftcoin;
        public static string EventCoinsName = Repository.ItemsInfo[EventCoins].Name;
        

        public static int MaxGiftsCount = 160;

        private static GiftsListModel[] GiftsListData = new GiftsListModel[]
        {
                      new GiftsListModel(0, new Vector3(-2285.59131, 353.3171, 175.404083), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(1, new Vector3(-25.6684418, -1423.82117, 30.62255), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(2, new Vector3(5.11801147, 41.33201, 70.53516), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(3, new Vector3(-1360.6825, -757.5558, 21.3029785), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(4, new Vector3(-1108.24548, -1641.03394, 3.639184), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(5, new Vector3(-442.255768, -2181.66772, 9.327274), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(6, new Vector3(-1045.17847, -2750.61133, 20.3546486), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(7, new Vector3(-1045.598, -2870.05444, 33.4073944), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(8, new Vector3(-935.886047, -2925.91528, 12.9438238), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(9, new Vector3(-1592.049, -3232.96338, 25.3173656), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(10, new Vector3(-162.626923, -1635.883, 36.2500153), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(11, new Vector3(-144.394333, -1614.15417, 35.04739), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(12, new Vector3(-1.07603192, -1822.08984, 28.5452137), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(13, new Vector3(21.4368477, -1898.93591, 21.9697266), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(14, new Vector3(-777.652161, -1323.97449, 8.599793), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(15, new Vector3(136.423538, -1048.01172, 28.14889), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(16, new Vector3(435.487823, -975.7568, 42.6903877), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(17, new Vector3(309.0615, -680.6187, 43.1051369), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(18, new Vector3(436.25473, -1181.47449, 28.33012), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(19, new Vector3(-1559.392, -416.0319, 37.09769), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(20, new Vector3(-57.83982, -2522.21851, 6.39838266), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(21, new Vector3(35.56981, -2676.66675, 16.1530914), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(22, new Vector3(605.5882, -3091.20972, 5.06743145), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(23, new Vector3(455.523926, -3109.37476, 5.06558228), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(24, new Vector3(487.437317, -3226.44238, 5.068811), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(25, new Vector3(590.1931, -3277.43677, 5.068617), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(26, new Vector3(677.118042, -2680.32056, 5.276535), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(27, new Vector3(1148.539, -1641.9137, 35.3292656), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(28, new Vector3(964.1327, -1856.23279, 30.1949978), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(29, new Vector3(870.9944, -2309.143, 29.584919), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(30, new Vector3(857.784668, -2514.64136, 39.5241165), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(31, new Vector3(1383.81946, -2079.25024, 50.99814), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(32, new Vector3(970.0033, -1629.98291, 29.1003151), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(33, new Vector3(944.7944, -1697.87256, 29.0732269), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(34, new Vector3(-482.0476, -1761.39038, 17.7108288), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(35, new Vector3(-559.0829, -1678.00427, 18.3107777), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(36, new Vector3(-610.01825, -1625.40247, 40.0062637), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(37, new Vector3(915.795166, -1702.83313, 50.26177), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(38, new Vector3(149.137329, 322.0253, 111.138184), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(39, new Vector3(517.0291, 169.785, 98.36717), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(40, new Vector3(987.5901, -137.965439, 72.08561), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(41, new Vector3(728.0148, -703.0981, 48.1357841), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(42, new Vector3(890.340149, -956.5558, 43.1844), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(43, new Vector3(172.943329, 472.119141, 140.89888), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(44, new Vector3(-402.296356, 508.968658, 119.196182), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(45, new Vector3(-690.7569, 512.8079, 109.362053), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(46, new Vector3(-677.6922, 901.5272, 229.577347), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(47, new Vector3(-488.137878, 183.216553, 82.1596451), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(48, new Vector3(1665.09668, -28.555212, 195.941071), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(49, new Vector3(-575.5828, -199.905045, 51.19797), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(50, new Vector3(-589.0226, -285.089, 49.3275), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(51, new Vector3(515.651245, 210.4317, 103.740707), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(52, new Vector3(-1201.01, -234.799561, 36.9467926), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(53, new Vector3(-1461.05383, 183.271591, 54.9214859), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(54, new Vector3(-1541.25488, 92.58407, 52.8968048), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(55, new Vector3(-986.076843, 429.8875, 79.571), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(56, new Vector3(-116.00853, -1034.48438, 72.25801), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(57, new Vector3(-204.288879, -1115.46619, 67.7606354), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(58, new Vector3(-461.853546, -975.881042, 68.39226), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(59, new Vector3(47.0696144, -460.737976, 98.8152542), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(60, new Vector3(127.964653, -347.85965, 101.547256), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(61, new Vector3(-467.353821, -331.041962, 41.2199326), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(62, new Vector3(361.6658, -560.2961, 38.0533), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(63, new Vector3(734.28, -966.3397, 35.85987), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(64, new Vector3(1354.08325, -762.9812, 65.75043), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(65, new Vector3(1111.69, -646.89, 55.8175125), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(66, new Vector3(1064.80127, -684.5379, 55.52116), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(67, new Vector3(886.8206, -171.905853, 76.1085739), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(68, new Vector3(645.5456, 100.199905, 79.74796), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(69, new Vector3(-1078.42529, 9.781026, 49.8942642), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(70, new Vector3(-2041.64343, -373.974548, 47.0958939), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(71, new Vector3(-1974.1604, -474.752258, 18.4418411), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(72, new Vector3(-1839.373, -579.7105, 18.4564381), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(73, new Vector3(-1763.48914, -710.7362, 16.6431389), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(74, new Vector3(-2059.97949, -1025.65918, 13.8954773), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(75, new Vector3(-2201.891, -400.096, 8.488818), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(76, new Vector3(-1646.21765, -1125.19092, 17.3389759), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(77, new Vector3(-1857.0531, -1236.66431, 7.6191287), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(78, new Vector3(-1379.2406, -1394.10156, 2.814476), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(79, new Vector3(-1352.92664, -1474.18616, 4.609598), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(80, new Vector3(-1202.005, -1669.86682, 3.37286854), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(81, new Vector3(-1155.53186, -2031.05591, 12.1598558), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(82, new Vector3(-1163.80542, -2324.58667, 17.76651), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(83, new Vector3(-245.167725, -2024.01208, 28.9448566), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(84, new Vector3(32.4365349, -1785.2572, 46.69942), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(85, new Vector3(51.9879227, -1479.62061, 33.11683), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(86, new Vector3(393.825958, -1655.591, 47.30642), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(87, new Vector3(338.019562, -1435.23413, 45.5085144), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(88, new Vector3(166.732834, -1050.88806, 70.74112), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(89, new Vector3(317.799622, -916.8191, 55.4805), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(90, new Vector3(466.5501, -737.997864, 36.38231), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(91, new Vector3(478.3094, -570.3662, 27.499155), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(92, new Vector3(547.777161, -637.0469, 24.8954563), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(93, new Vector3(-630.3044, -222.634171, 52.5483055), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(94, new Vector3(-590.6441, -112.950562, 48.04123), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(95, new Vector3(-768.7893, 23.0241051, 39.6531), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(96, new Vector3(-934.757751, 398.0674, 76.72518), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(97, new Vector3(-787.557068, 324.645782, 84.85981), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(98, new Vector3(-153.375153, 296.8286, 102.149666), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(99, new Vector3(-29.0716572, 321.2933, 112.159256), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(100, new Vector3(-1092.78882, 319.999634, 65.76625), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(101, new Vector3(-565.1096, -919.098755, 22.87948), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(102, new Vector3(-1227.6582, -921.8499, 1.14825058), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(103, new Vector3(-1000.64374, -1411.98718, 0.598439336), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(104, new Vector3(-333.099182, -444.048462, 30.80543), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(105, new Vector3(-222.807144, -280.875641, 48.079567), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(106, new Vector3(389.8069, -326.3287, 49.19999), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(107, new Vector3(830.396851, -507.570282, 55.8046875), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(108, new Vector3(1297.59741, -1121.78613, 38.97583), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(109, new Vector3(1152.7, -1567.89917, 38.3240433), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(110, new Vector3(883.6751, -2166.256, 31.2695427), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(111, new Vector3(453.2912, -1476.341, 34.089016), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(112, new Vector3(79.93804, -2039.36951, 5.487273), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(113, new Vector3(1211.99048, -3279.779, 12.40456), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(114, new Vector3(1242.61011, -2912.4314, 28.678339), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(115, new Vector3(1010.47845, -2868.44116, 38.1613121), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(116, new Vector3(344.108276, -2295.56274, 1.624), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(117, new Vector3(206.159592, -1666.64563, 43.8237), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(118, new Vector3(39.3081131, -1404.525, 28.3532467), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(119, new Vector3(133.016037, -1328.92224, 33.02357), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(120, new Vector3(231.490051, -1358.36987, 27.64064), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(121, new Vector3(305.66333, -1238.03662, 28.6928272), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(122, new Vector3(229.110657, -1204.13147, 38.02399), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(123, new Vector3(539.2798, -1035.818, 26.7238121), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(124, new Vector3(211.778214, -934.973145, 23.2753067), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(125, new Vector3(-70.97297, -1232.86719, 27.9826069), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(126, new Vector3(-1147.686, -1465.3623, 9.586084), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(127, new Vector3(-1224.94214, -1479.44458, 3.33128023), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(128, new Vector3(-932.742, -985.4539, 1.150939), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(129, new Vector3(-793.685364, -724.36084, 26.2779446), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(130, new Vector3(-177.8133, -601.068054, 48.1143074), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(131, new Vector3(-105.2487, -856.32196, 40.0671654), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(132, new Vector3(-319.246765, -772.8692, 53.6214256), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(133, new Vector3(-17.0624218, -681.4704, 48.4661446), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(134, new Vector3(-1177.8291, -480.7991, 34.7999725), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(135, new Vector3(693.8229, 589.077454, 136.2909), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(136, new Vector3(181.828064, 1107.42773, 225.78299), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(137, new Vector3(751.3668, 1316.69678, 358.85257), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(138, new Vector3(137.2379, -3293.635, 18.8478088), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(139, new Vector3(-1929.60962, -3041.247, 20.4377441), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(140, new Vector3(-1320.92236, -2644.648, 25.3602638), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(141, new Vector3(835.9977, -3194.115, 13.4947128), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(142, new Vector3(1244.73413, -2571.91113, 42.0591164), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(143, new Vector3(1610.29248, -2364.09131, 99.6306839), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(144, new Vector3(1717.44177, -1607.26819, 111.489037), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(145, new Vector3(-48.6891937, 776.404236, 226.232239), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(146, new Vector3(-451.12088, 1084.89539, 331.535065), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(147, new Vector3(-1358.146, 538.676453, 123.674515), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(148, new Vector3(-2006.75769, 543.404663, 109.154716), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(149, new Vector3(-1723.91418, -191.353073, 57.52448), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(150, new Vector3(-973.944153, -3559.28125, 0.474510431), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(151, new Vector3(141.608063, -2903.60449, 15.4484835), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(152, new Vector3(-438.008484, -1134.48022, 23.34731), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(153, new Vector3(-900.3673, -1165.46277, 31.7516823), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(154, new Vector3(-602.9927, 233.449158, 118.179955), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(155, new Vector3(48.5523949, 111.491791, 79.23983), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(156, new Vector3(342.9349, -204.293167, 53.224884), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(157, new Vector3(145.741287, -84.25832, 71.4717941), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(158, new Vector3(552.571167, -167.733536, 53.4863739), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(159, new Vector3(1964.63892, -960.2291, 78.0430145), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(160, new Vector3(1849.65808, -239.012177, 293.42), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(161, new Vector3(-541.532043, -2001.39319, 14.09728), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(162, new Vector3(-882.1271, -2389.39771, 19.078661), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(163, new Vector3(-2267.44678, -26.68034, 111.26033), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(164, new Vector3(87.32546, 810.3899, 210.124146), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(165, new Vector3(1299.88635, 318.252075, 80.98725), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(166, new Vector3(1009.05109, 98.13045, 89.23909), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(167, new Vector3(694.783264, -1199.832, 23.4370975), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(168, new Vector3(1223.74622, -455.78244, 77.89811), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(169, new Vector3(721.1825, -1928.78833, 28.4412956), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(170, new Vector3(-1166.38635, -694.9654, 34.53424), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(171, new Vector3(-1272.4104, -1120.08081, 9.14989), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(172, new Vector3(-1451.35022, -644.462952, 36.3069725), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(173, new Vector3(-1309.331, -192.709839, 59.6527176), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(174, new Vector3(-1808.34082, 316.258148, 88.36661), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(175, new Vector3(-1361.57715, 87.7027054, 59.6298828), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(176, new Vector3(-1342.72278, 323.9802, 64.48315), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(177, new Vector3(-75.17522, -819.006348, 325.1664), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(178, new Vector3(158.431076, -744.8573, 265.814331), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(179, new Vector3(151.235779, -633.440369, 265.811035), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(180, new Vector3(-144.653061, -593.510437, 210.7691), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(181, new Vector3(-597.841248, -716.907654, 130.069153), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(182, new Vector3(-947.9846, -726.221252, 19.8327618), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(183, new Vector3(-546.3202, -1292.68945, 25.9007187), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(184, new Vector3(484.107239, -2283.343, 17.71023), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(185, new Vector3(427.7381, -1888.02185, 28.9498081), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(186, new Vector3(-217.563385, -1366.56506, 30.2572861), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(187, new Vector3(602.6172, -1435.84753, 8.727696), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(188, new Vector3(1028.49048, -1433.25208, 25.8465118), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(189, new Vector3(-727.2744, -982.7646, 32.3965034), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(190, new Vector3(-856.9114, -224.245087, 60.013855), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(191, new Vector3(-1124.05835, -350.190643, 48.14103), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(192, new Vector3(-501.276, -1438.72839, 13.4704008), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(193, new Vector3(-1005.0755, -2008.8324, 35.9225235), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(194, new Vector3(-715.0092, -1889.16858, 9.239815), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(195, new Vector3(-1552.08521, -2399.309, 6.031447), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(196, new Vector3(-552.1727, -2238.12549, 121.368004), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(197, new Vector3(-275.309723, -2432.89233, 121.366592), new Vector3(0.00, 0.00, 0.00)),
            new GiftsListModel(198, new Vector3(-51.34083, -2255.71362, 6.810603), new Vector3(0.00, 0.00, 0.00)),

        };
            

        public static void Init ()
        {
            if (isEvent)
            {
                PedSystem.Repository.CreateQuest("dedmoroz", new Vector3(-1640.6166, -1096.9696, 13.024864), -42f, title: "~y~NPC~w~ Санта Клаус\nОбмен подарков", colShapeEnums: ColShapeEnums.Festive);
                Main.CreateBlip(new Main.BlipData(607, "Санта Клаус", new Vector3(-1640.6166, -1096.9696, 13.024864), 0, true));

            }
        }

        public static void InitPlayerData(ExtPlayer player)
        {
            try
            {
                if (!isEvent) 
                    return;

                var accountData = player.GetAccountData();
                if (accountData == null) 
                    return;

                var collectionGifts = accountData.CollectionGifts;
                if (accountData.CollectionGifts.Count == MaxGiftsCount) 
                    return;

                var randomGiftsListData = GiftsListData.ToList();
                foreach (int index in collectionGifts)
                {
                    randomGiftsListData.Remove(GiftsListData[index]);
                }
                randomGiftsListData = NewCasino.Horses.Shuffle(randomGiftsListData);
                randomGiftsListData.RemoveRange(MaxGiftsCount - collectionGifts.Count, (randomGiftsListData.Count - MaxGiftsCount + collectionGifts.Count));

                Trigger.ClientEvent(player, "client.events.open", JsonConvert.SerializeObject(randomGiftsListData));
            }
            catch (Exception e)
            {
                Log.Write($"InitPlayerData Exception: {e.ToString()}");
            }
        }
        [Interaction(ColShapeEnums.Festive)]
        public static void OnFestive(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Trigger.ClientEvent(player, "openInput", $"Обмен {EventCoinsName} | 3 = 1 RB", "Вводите числа, кратные 3 - 3, 6, 9 и т.д.", 3, "sell_festive");
            }
            catch (Exception e)
            {
                Log.Write($"OnFestive Exception: {e.ToString()}");
            }
        }
         

        [RemoteEvent("server.events.collect")]
        public static void EventsCollect(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (sessionData.AntiAnimDown || (characterData.AdminLVL >= 1 && characterData.AdminLVL <= 5) || Main.IHaveDemorgan(player)) return;
                if (Main.IHaveDemorgan(player)) return;
                if (accountData.CollectionGifts.Contains(index))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы уже собрали эту коробку, поищите другие!", 8000);
                    return;
                }
                int rand = new Random().Next(4, 14);
                if (Repository.isFreeSlots(player, EventCoins, rand) != 0) return;
                Main.OnAntiAnim(player);
                Trigger.PlayAnimation(player, "anim@mp_snowball", "pickup_snowball", 39);
                // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "pickup_snowball");

                NAPI.Task.Run(() =>
                {
                    try
                    {
                        if (!player.IsCharacterData()) return;
                        Trigger.StopAnimation(player);
                        Main.OffAntiAnim(player);
                        if (Repository.isFreeSlots(player, EventCoins, rand) != 0) return;
                        accountData.CollectionGifts.Add(index);
                        Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", EventCoins, rand);
                        Trigger.ClientEvent(player, "client.events.confirming", rand, accountData.CollectionGifts.Count, MaxGiftsCount);
                    }
                    catch (Exception e)
                    {
                        Log.Write($"EventsCollect Task #1 Exception: {e.ToString()}");
                    }
                }, 5000);
            }
            catch (Exception e)
            {
                Log.Write($"EventsCollect Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.givehc)]
        public static void CMD_givehc(ExtPlayer player, int id, int amount)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.givehc)) return;
                ExtPlayer target = Main.GetPlayerByID(id);
                var targetCharacterData = target.GetCharacterData();
                if (targetCharacterData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }
                Chars.Repository.AddNewItem(target, $"char_{targetCharacterData.UUID}", "inventory", ItemId.Giftcoin, amount);
                //GameLog.Money($"player({characterData.UUID})", $"player({targetCharacterData.UUID})", amount, "admin");
                GameLog.Admin($"{player.Name}", $"givehc({amount})", $"{target.Name}");
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы выдали человеку {target.Name} {amount} Подарков", 5000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_givehc Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.givehcrad)]
        public static void CMD_givehcrad(ExtPlayer player, int radius, int amount)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.givehcrad)) return;
                foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(player.Position, (float)radius, UpdateData.GetPlayerDimension(player)))
                {
                    var foreachCharacterData = foreachPlayer.GetCharacterData();
                    if (foreachCharacterData == null) continue;
                    if (foreachPlayer.Value != player.Value)
                    {
                        if (Chars.Repository.AddNewItem(foreachPlayer, $"char_{foreachCharacterData.UUID}", "inventory", ItemId.Giftcoin, amount) == -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 5000);
                            return;
                        }
                        //GameLog.Money($"player({characterData.UUID})", $"player({targetCharacterData.UUID})", amount, "admin");
                        GameLog.Admin($"{player.Name}", $"givehcrad({amount})", $"{foreachPlayer.Name}");

                    }
                }
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы выдали игрокам в радиусе {radius} {amount} Подарков", 6000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_givehcrad Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("sever.events.buyItem")]
        public static void buyItem(ExtPlayer player, int index)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                int count = Chars.Repository.getCountItem($"char_{characterData.UUID}", EventCoins, bagsToggled:false);
                Random rand = new Random();
                switch (index)
                {
                    case 0:
                        if (99999 > count)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нет столько подарков", 3000);
                            return;
                        }
                        if (Chars.Repository.isFreeSlots(player, ItemId.Leg, 1) != 0) return;
                        Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", EventCoins, 99999);
                        Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Leg, 1, $"202_{rand.Next(0, 3)}_True");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно обменяли свои подарки на рождественские штаны", 6000);
                        break;
                    case 1:
                        if (650 > count)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нет столько подарков", 3000);
                            return;
                        }
                        if (Chars.Repository.isFreeSlots(player, ItemId.Top, 1) != 0) return;
                        Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", EventCoins, 650);
                        Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Top, 1, $"578_{rand.Next(0, 3)}_True");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно обменяли свои подарки на рождественский топ", 6000);
                        break;
                    case 2:
                        if (1000 > count)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нет столько подарков", 3000);
                            return;
                        }
                        if (Chars.Repository.isFreeSlots(player, ItemId.Jewelry, 1) != 0) return;
                        Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", EventCoins, 1000);
                        Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Jewelry, 1, $"214_{rand.Next(0, 3)}_True");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно обменяли свои подарки на леденцы", 6000);
                        break;
                    case 3:
                        if (650 > count)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нет столько подарков", 3000);
                            return;
                        }
                        if (Chars.Repository.isFreeSlots(player, ItemId.Leg, 1) != 0) return;
                        Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", EventCoins, 650);
                        Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Leg, 1, $"223_{rand.Next(0, 3)}_False");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно обменяли свои подарки на рождественские штаны", 6000);
                        break;
                    case 4:
                        if (650 > count)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нет столько подарков", 3000);
                            return;
                        }
                        if (Chars.Repository.isFreeSlots(player, ItemId.Jewelry, 1) != 0) return;
                        Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", EventCoins, 650);
                        Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Top, 1, $"605_{rand.Next(0, 3)}_False");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно обменяли свои подарки на рождественский топ", 6000);
                        break;
                    case 5:
                        if (1000 > count)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нет столько подарков", 3000);
                            return;
                        }
                        if (Chars.Repository.isFreeSlots(player, ItemId.Jewelry, 1) != 0) return;
                        Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", EventCoins, 1000);
                        Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Jewelry, 1, $"176_{rand.Next(0, 3)}_False");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно обменяли свои подарки на леденцы", 6000);
                        break;
                    case 6:
                        if (1500 > count)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нет столько коинов", 3000);
                            return;
                        }
                        if (Chars.Repository.isFreeSlots(player, ItemId.CarCoupon, 1) != 0) return;
                        Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", EventCoins, 1500);
                        Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.CarCoupon, 1, $"snowbike");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно обменяли свои подарки на снегоход!", 6000);
                        break;
                    default:
                        return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"buyItem Exception: {e.ToString()}");
            }
        }
    }
}
