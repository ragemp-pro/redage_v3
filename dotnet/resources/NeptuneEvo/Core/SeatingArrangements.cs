using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Functions;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using Localization;

namespace NeptuneEvo.Core
{
    class SeatingArrangements : Script
    {
      
        private static readonly nLog Log = new nLog("Core.SeatingArrangements");
        private static Dictionary<ExtPlayer, Vector3> LandingData = new Dictionary<ExtPlayer, Vector3>();
         
        [RemoteEvent("server.landing.sit")]
        public static void LandingSit(ExtPlayer player, float posX, float posY, float posZ, float heading)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                var position = new Vector3(posX, posY, posZ);
                if (LandingData.Values.Any(p => p.DistanceTo2D(position) < 0.5))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantSitNear), 3000);
                    return;
                }

                LandingData[player] = position;
                
                //Trigger.ClientEventInRange(sitdata.Item1, 250f, "setClientRotation", player.Value, sitdata.Item2.Z);                
                //player.Rotation = position;
                //player.Rotation = new Vector3(0, 0, heading);
                //BattlePass.Repository.UpdateReward(player, 71);
                Trigger.ClientEvent(player, "client.seat.yes", posX, posY, posZ, heading);
                Trigger.StopAnimation(player);
                player.SetSharedData("AnimToKey", $"sit");
            }
            catch (Exception e)
            {
                Log.Write($"PlayerFinishedMining Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.landing.end")]
        public static void LandingEnd(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) 
                    return;
                
                if (!LandingData.ContainsKey(player))
                    return;
                
                LandingData.Remove(player);
                
                player.SetSharedData("AnimToKey", 0);
            }
            catch (Exception e)
            {
                Log.Write($"PlayerFinishedMining Exception: {e.ToString()}");
            }
        }

        [ServerEvent(Event.PlayerDeath)]
        public void onPlayerDeathHandler(ExtPlayer player, ExtPlayer entityKiller, uint weapon) => LandingEnd(player);
        
        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnected(ExtPlayer player, DisconnectionType type, string reason) => LandingEnd(player);


        /*private static readonly nLog Log = new nLog("Core.SeatingArrangements");
        public static Dictionary<int, int> SitToPlayerId = new Dictionary<int, int>();
        public static (Vector3, Vector3, float, byte)[] SitingPos = new (Vector3, Vector3, float, byte)[365] // Position, Rotation, Distance, Type (1 = сидеть, 2 = лежать, 3 = лежать v2)
        {
            (new Vector3(-1112.59, -103.22, 41.8), new Vector3(0, 0, 123.25), 0.4f, 1),
            (new Vector3(-1113.68, -101.70, 41.8), new Vector3(0, 0, 126.38), 0.4f, 1),
            (new Vector3(-982.85, -100.84, 40.57), new Vector3(0, 0, 193.31), 0.4f, 1),
            (new Vector3(-984.52, -101.22, 40.57), new Vector3(0, 0, 192.91), 0.4f, 1),
            (new Vector3(-1135.25, -0.199, 48.98), new Vector3(0, 0, 120.79), 0.4f, 1),
            (new Vector3(-1136.19, 1.48, 48.98), new Vector3(0, 0, 121.19), 0.4f, 1),
            (new Vector3(-1372.03, 170.14, 58.01), new Vector3(0, 0, 279.11), 0.4f, 1),
            (new Vector3(-1371.68, 168.34, 58.01), new Vector3(0, 0, 280.45), 0.4f, 1),
            (new Vector3(-1314.71, 122.74, 57.12), new Vector3(0, 0, 332.30), 0.4f, 1),
            (new Vector3(-1313.26, 121.85, 57.15), new Vector3(0, 0, 333.24), 0.4f, 1),
            (new Vector3(-1106.79, 154.40, 63.03), new Vector3(0, 0, 2.49), 0.4f, 1),
            (new Vector3(-1104.98, 154.49, 63.03), new Vector3(0, 0, 3.61), 0.4f, 1),
            (new Vector3(-1101.987, 70.00, 54.17), new Vector3(0, 0, 290.98), 0.4f, 1),
            (new Vector3(-1102.57, 71.75, 54.19), new Vector3(0, 0, 287.05), 0.4f, 1),
            (new Vector3(-70.59, -818.91, 243.385), new Vector3(0, 0, 344.286), 0.4f, 1), // Организация Low
            (new Vector3(-69.25, -819.40, 243.385), new Vector3(0, 0, 341.57), 0.4f, 1),
            (new Vector3(-66.60, -821.72, 243.385), new Vector3(0, 0, 340.54), 0.4f, 1),
            (new Vector3(-65.34, -822.183, 243.385), new Vector3(0, 0, 343.83), 0.4f, 1),
            (new Vector3(-64.13, -822.183, 243.385), new Vector3(0, 0, 70.92), 0.4f, 1),
            (new Vector3(-63.69, -820.84, 243.385), new Vector3(0, 0, 71.11), 0.4f, 1),
            (new Vector3(-67.86, -807.89, 243.385), new Vector3(0, 0, 26.23), 0.4f, 1),
            (new Vector3(-67.74, -805.03, 243.385), new Vector3(0, 0, 155.51), 0.4f, 1),
            (new Vector3(-69.44, -804.41, 243.385), new Vector3(0, 0, 162.92), 0.4f, 1),
            (new Vector3(-84.36, -813.82, 243.385), new Vector3(0, 0, 250.01), 0.4f, 1),
            (new Vector3(-71.07, -806.73, 243.385), new Vector3(0, 0, 294.17), 0.4f, 1),
            (new Vector3(-78.136, -802.04, 243.385), new Vector3(0, 0, 121.38), 0.4f, 1),
            (new Vector3(-78.47, -802.97, 243.385), new Vector3(0, 0, 34.71), 0.4f, 1),
            (new Vector3(-82.61, -806.55, 243.385), new Vector3(0, 0, 261.44), 0.4f, 1),
            (new Vector3(-83.93, -809.43, 244.08), new Vector3(0, 0, 342.53), 0.7f, 2),
            (new Vector3(-82.61, -806.55, 243.385), new Vector3(0, 0, 261.44), 0.4f, 1), // Организация Low
            (new Vector3(-1567.985, -582.096, 108.5287), new Vector3(0, 0, 262.99), 0.7f, 1), // Организация Medium
            (new Vector3(-1564.14, -582.7026, 108.5287), new Vector3(0, 0, 36.9757), 0.7f, 1),
            (new Vector3(-1565.66, -583.80, 108.5287), new Vector3(0, 0, 36.9757), 0.7f, 1),
            (new Vector3(-1565.186, -580.1352, 108.5287), new Vector3(0, 0, 166.2324), 0.4f, 1),
            (new Vector3(-1557.995, -576.043, 108.5287), new Vector3(0, 0, 268.421), 0.4f, 1),
            (new Vector3(-1557.398, -576.890, 108.5287), new Vector3(0, 0, 356.757), 0.4f, 1),
            (new Vector3(-1558.563, -570.712, 108.5287), new Vector3(0, 0, 134.28), 0.7f, 1),
            (new Vector3(-1560.346, -567.86, 109.2177), new Vector3(0, 0, 217.6152), 0.7f, 2),
            (new Vector3(-1575.598, -573.6992, 108.5287), new Vector3(0, 0, 214.7873), 0.4f, 1),
            (new Vector3(-1576.698, -574.5002, 108.5287), new Vector3(0, 0, 214.7873), 0.4f, 1),
            (new Vector3(-1580.156, -575.3942, 108.5287), new Vector3(0, 0, 214.7873), 0.7f, 1),
            (new Vector3(-1581.528, -576.3942, 108.5287), new Vector3(0, 0, 214.7873), 0.4f, 1),
            (new Vector3(-1563.70, -565.11, 108.5287), new Vector3(0, 0, 124.327), 0.4f, 1),
            (new Vector3(-1581.873, -577.4492, 108.5287), new Vector3(0, 0, 307.4873), 0.4f, 1),
            (new Vector3(-1580.987, -578.6645, 108.5287), new Vector3(0, 0, 307.4873), 0.4f, 1), // Организация Medium
            (new Vector3(-142.38, -630.476, 168.82), new Vector3(0, 0, 185.146), 0.4f, 1), // Организация High
            (new Vector3(-143.73, -630.614, 168.82), new Vector3(0, 0, 187.07), 0.4f, 1),
            (new Vector3(-147.093, -629.71, 168.82), new Vector3(0, 0, 187.68), 0.4f, 1),
            (new Vector3(-148.64, -629.87, 168.82), new Vector3(0, 0, 184.63), 0.4f, 1),
            (new Vector3(-149.648, -630.66, 168.82), new Vector3(0, 0, 276.52), 0.4f, 1),
            (new Vector3(-149.47, -632.10, 168.82), new Vector3(0, 0, 278.83), 0.4f, 1),
            (new Vector3(-139.94, -641.59, 168.82), new Vector3(0, 0, 230.11), 0.4f, 1),
            (new Vector3(-138.78, -644.10, 168.82), new Vector3(0, 0, 7.91), 0.4f, 1),
            (new Vector3(-136.96, -643.90, 168.82), new Vector3(0, 0, 2.98), 0.4f, 1),
            (new Vector3(-136.62, -641.31, 168.82), new Vector3(0, 0, 139.13), 0.4f, 1),
            (new Vector3(-128.324, -642.30, 168.82), new Vector3(0, 0, 325.04), 0.4f, 1),
            (new Vector3(-128.36, -641.43, 168.82), new Vector3(0, 0, 234.65), 0.4f, 1),
            (new Vector3(-126.16, -636.39, 168.82), new Vector3(0, 0, 106.73), 0.4f, 1),
            (new Vector3(-126.28, -633.09, 169.51), new Vector3(0, 0, 182.58), 0.7f, 2),
            (new Vector3(-127.82, -628.98, 168.82), new Vector3(0, 0, 99.70), 0.4f, 1), // Организация High
            (new Vector3(-1386.397, -488.408, 57.107), new Vector3(0, 0, 6.545), 0.4f, 1), // Garage организации
            (new Vector3(-1389.249, -487.6375, 57.107), new Vector3(0, 0, 277.89), 0.4f, 1),
            (new Vector3(-1389.493, -486.0416, 57.107), new Vector3(0, 0, 278.53), 0.4f, 1),
            (new Vector3(-1387.619, -488.5798, 57.107), new Vector3(0, 0, 11.03), 0.4f, 1), // Garage организации
            (new Vector3(910.8136, 51.53099, 111.7806), new Vector3(0, 0, 32.2395), 0.7f, 1), // Крыша казино Diamond
            (new Vector3(908.6894, 51.97491, 111.781), new Vector3(0, 0, 304.8076), 0.7f, 1),
            (new Vector3(909.2809, 54.26945, 111.7804), new Vector3(0, 0, 203.5826), 0.7f, 1),
            (new Vector3(911.3334, 53.66261, 111.7744), new Vector3(0, 0, 125.3744), 0.7f, 1),
            (new Vector3(957.5586, 49.95117, 112.6328), new Vector3(0, 0, 147.8262), 0.7f, 1),
            (new Vector3(957.1338, 48.13849, 112.6319), new Vector3(0, 0, 58.32444), 0.7f, 1),
            (new Vector3(955.9371, 47.01624, 112.6329), new Vector3(0, 0, 325.2851), 0.7f, 1),
            (new Vector3(954.5969, 44.98684, 112.6354), new Vector3(0, 0, 147.8944), 0.7f, 1),
            (new Vector3(954.0871, 43.34811, 112.633), new Vector3(0, 0, 62.13251), 0.7f, 1),
            (new Vector3(952.8796, 42.11836, 112.6329), new Vector3(0, 0, 326.3849), 0.7f, 1),
            (new Vector3(951.5282, 39.96761, 112.6328), new Vector3(0, 0, 149.6315), 0.7f, 1),
            (new Vector3(951.0334, 38.36804, 112.6328), new Vector3(0, 0, 57.49799), 0.7f, 1),
            (new Vector3(949.9182, 37.18125, 112.6329), new Vector3(0, 0, 327.9693), 0.7f, 1),
            (new Vector3(948.3087, 35.3149, 112.6329), new Vector3(0, 0, 149.4803), 0.7f, 1),
            (new Vector3(948.0137, 33.52831, 112.6156), new Vector3(0, 0, 58.90583), 0.7f, 1),
            (new Vector3(946.726, 32.36529, 112.6327), new Vector3(0, 0, 330.8376), 0.7f, 1),
            (new Vector3(945.2398, 30.41574, 112.6327), new Vector3(0, 0, 150.1806), 0.7f, 1),
            (new Vector3(944.9413, 28.62748, 112.6327), new Vector3(0, 0, 60.03614), 0.7f, 1),
            (new Vector3(943.7213, 27.45283, 112.6327), new Vector3(0, 0, 326.0468), 0.7f, 1),
            (new Vector3(942.3178, 25.43105, 112.6328), new Vector3(0, 0, 149.0738), 0.7f, 1),
            (new Vector3(941.839, 23.65498, 112.6328), new Vector3(0, 0, 60.2403), 0.7f, 1),
            (new Vector3(940.7545, 22.46269, 112.6328), new Vector3(0, 0, 326.5145), 0.7f, 1),
            (new Vector3(939.1323, 20.66275, 112.6328), new Vector3(0, 0, 152.9032), 0.7f, 1),
            (new Vector3(938.9226, 18.98779, 112.6329), new Vector3(0, 0, 58.18721), 0.7f, 1),
            (new Vector3(937.5244, 17.72029, 112.633), new Vector3(0, 0, 326.4078), 0.7f, 1),
            (new Vector3(944.9989, 41.96312, 112.6325), new Vector3(0, 0, 327.4434), 0.7f, 1),
            (new Vector3(947.2051, 40.58259, 112.6326), new Vector3(0, 0, 325.6417), 0.7f, 1),
            (new Vector3(944.4639, 35.94167, 112.6265), new Vector3(0, 0, 149.2072), 0.7f, 1),
            (new Vector3(942.1684, 37.39515, 112.6328), new Vector3(0, 0, 147.0568), 0.7f, 1),
            (new Vector3(932.8333, 22.3193, 112.6327), new Vector3(0, 0, 328.8099), 0.7f, 1),
            (new Vector3(935.1406, 20.88023, 112.6328), new Vector3(0, 0, 327.3107), 0.7f, 1),
            (new Vector3(922.8158, 52.28637, 111.7812), new Vector3(0, 0, 17.60457), 0.7f, 1),
            (new Vector3(920.3684, 51.35282, 111.7811), new Vector3(0, 0, 20.02403), 0.7f, 1),
            (new Vector3(924.2679, 54.10461, 111.7812), new Vector3(0, 0, 149.4119), 0.7f, 1),
            (new Vector3(921.9033, 55.56437, 111.7812), new Vector3(0, 0, 146.3887), 0.7f, 1),
            (new Vector3(919.8051, 56.43701, 111.7812), new Vector3(0, 0, 203.3725), 0.7f, 1),
            (new Vector3(918.7014, 53.81808, 111.7811), new Vector3(0, 0, 256.9608), 0.7f, 1),
            (new Vector3(912.0148, 43.58101, 111.781), new Vector3(0, 0, 302.9391), 0.7f, 1),
            (new Vector3(913.4932, 41.21424, 111.7813), new Vector3(0, 0, 300.2731), 0.7f, 1),
            (new Vector3(914.9825, 39.38923, 111.7798), new Vector3(0, 0, 356.9037), 0.7f, 1),
            (new Vector3(917.0373, 41.01896, 111.7813), new Vector3(0, 0, 47.72642), 0.7f, 1),
            (new Vector3(960.7443, 54.78387, 112.6328), new Vector3(0, 0, 149.6896), 0.7f, 1),
            (new Vector3(960.2824, 53.17041, 112.6328), new Vector3(0, 0, 57.16651), 0.7f, 1),
            (new Vector3(958.9903, 51.96869, 112.6328), new Vector3(0, 0, 324.4964), 0.7f, 1),
            (new Vector3(956.5916, 55.58085, 112.6332), new Vector3(0, 0, 147.2889), 0.7f, 1),
            (new Vector3(954.4771, 56.96658, 112.6329), new Vector3(0, 0, 148.6538), 0.7f, 1),
            (new Vector3(954.1741, 54.24842, 111.8219), new Vector3(0, 0, 149.821), 0.7f, 1),
            (new Vector3(953.7046, 52.91698, 111.8192), new Vector3(0, 0, 326.3333), 0.7f, 1),
            (new Vector3(948.6268, 43.56395, 111.8201), new Vector3(0, 0, 55.8907), 0.7f, 1),
            (new Vector3(947.3071, 44.26879, 111.8175), new Vector3(0, 0, 233.7568), 0.7f, 1),
            (new Vector3(942.1659, 34.66357, 111.9229), new Vector3(0, 0, 145.3848), 0.7f, 1),
            (new Vector3(941.4244, 33.36376, 111.8201), new Vector3(0, 0, 326.2771), 0.7f, 1),
            (new Vector3(936.3639, 23.89992, 111.8648), new Vector3(0, 0, 57.10396), 0.7f, 1),
            (new Vector3(935.2349, 24.61301, 111.8429), new Vector3(0, 0, 236.1113), 0.7f, 1),
            (new Vector3(943.6993, 60.03538, 114.4933), new Vector3(0, 0, 236.2385), 2.0f, 2),
            (new Vector3(937.5967, 50.10545, 114.4915), new Vector3(0, 0, 235.752), 2.0f, 2),
            (new Vector3(931.4426, 40.36805, 114.4924), new Vector3(0, 0, 232.7516), 2.0f, 2),
            (new Vector3(925.5209, 30.50761, 114.4899), new Vector3(0, 0, 235.8794), 2.0f, 2), // Крыша казино Diamond
            (new Vector3(233.1002, 217.6429, 110.3674), new Vector3(0, 0, 293.0068), 0.7f, 1), // Инта мэрии
            (new Vector3(233.8786, 216.0568, 110.362), new Vector3(0, 0, 292.3122), 0.7f, 1),
            (new Vector3(234.57, 214.593, 110.3588), new Vector3(0, 0, 295.5534), 0.7f, 1),
            (new Vector3(257.2032, 212.0639, 106.3724), new Vector3(0, 0, 68.07445), 0.7f, 1),
            (new Vector3(240.5251, 217.5032, 106.3733), new Vector3(0, 0, 296.1818), 0.7f, 1),
            (new Vector3(239.2035, 220.3922, 106.3661), new Vector3(0, 0, 302.7957), 0.7f, 1), // Инта мэрии
            (new Vector3(972.6113, 77.45037, 116.9423), new Vector3(0, 0, 141.3944), 0.7f, 2), // Penthouse казино Diamond 
            (new Vector3(971.2979, 78.09745, 116.9423), new Vector3(0, 0, 142.4414), 0.7f, 2),
            (new Vector3(969.4558, 79.28532, 116.2437), new Vector3(0, 0, 240.179), 0.7f, 1),
            (new Vector3(970.5175, 80.98527, 116.241), new Vector3(0, 0, 237.5208), 0.7f, 1),
            (new Vector3(972.9722, 82.03258, 116.2433), new Vector3(0, 0, 145.6618), 0.7f, 1),
            (new Vector3(987.2839, 72.51646, 116.9421), new Vector3(0, 0, 326.6612), 0.7f, 2),
            (new Vector3(990.1711, 70.58713, 116.9422), new Vector3(0, 0, 324.3047), 0.7f, 2),
            (new Vector3(990.4, 68.08937, 116.2437), new Vector3(0, 0, 56.31409), 0.7f, 1),
            (new Vector3(988.5365, 65.132, 116.2441), new Vector3(0, 0, 51.91209), 0.7f, 1),
            (new Vector3(987.1129, 62.81881, 116.2448), new Vector3(0, 0, 57.97158), 0.7f, 1),
            (new Vector3(963.9448, 51.13269, 116.2561), new Vector3(0, 0, 147.636), 0.4f, 1),
            (new Vector3(962.3878, 49.90618, 116.2561), new Vector3(0, 0, 239.9625), 0.4f, 1),
            (new Vector3(961.3384, 48.21856, 116.2561), new Vector3(0, 0, 233.6215), 0.4f, 1),
            (new Vector3(959.3212, 44.94305, 116.2561), new Vector3(0, 0, 238.2659), 0.4f, 1),
            (new Vector3(960.7056, 43.07932, 116.2561), new Vector3(0, 0, 322.6851), 0.4f, 1),
            (new Vector3(955.6862, 38.33283, 116.4442), new Vector3(0, 0, 236.6611), 0.4f, 1),
            (new Vector3(954.763, 36.81363, 116.4442), new Vector3(0, 0, 234.5269), 0.4f, 1),
            (new Vector3(953.8918, 35.50513, 116.4442), new Vector3(0, 0, 239.2269), 0.4f, 1),
            (new Vector3(953.1655, 34.31731, 116.4442), new Vector3(0, 0, 233.4357), 0.4f, 1),
            (new Vector3(952.3293, 33.0033, 116.4442), new Vector3(0, 0, 237.1495), 0.4f, 1),
            (new Vector3(951.546, 31.72139, 116.4442), new Vector3(0, 0, 236.6766), 0.4f, 1),
            (new Vector3(954.9444, 33.18817, 116.2442), new Vector3(0, 0, 237.2384), 0.4f, 1),
            (new Vector3(955.8591, 34.67036, 116.2442), new Vector3(0, 0, 237.4069), 0.4f, 1),
            (new Vector3(947.8051, 24.1959, 116.2442), new Vector3(0, 0, 326.9961), 0.4f, 1),
            (new Vector3(946.3718, 25.09615, 116.2442), new Vector3(0, 0, 328.7419), 0.4f, 1), // Penthouse казино Diamond 
            (new Vector3(-2024.161, -1039.781, 5.656947), new Vector3(0, 0, 354.3705), 0.7f, 1), // Корабль на пляже
            (new Vector3(-2022.277, -1038.568, 5.661452), new Vector3(0, 0, 78.84772), 0.7f, 1),
            (new Vector3(-2023.159, -1036.303, 5.659049), new Vector3(0, 0, 155.3187), 0.7f, 1),
            (new Vector3(-2089.371, -1022.898, 5.987804), new Vector3(0, 0, 342.0814), 0.4f, 1),
            (new Vector3(-2090.883, -1022.39, 5.987804), new Vector3(0, 0, 342.7255), 0.4f, 1),
            (new Vector3(-2097.229, -1020.32, 5.960461), new Vector3(0, 0, 339.2211), 0.4f, 1),
            (new Vector3(-2083.299, -1015.877, 5.964128), new Vector3(0, 0, 64.29179), 0.4f, 1),
            (new Vector3(-2124.94, -1005.803, 8.650864), new Vector3(0, 0, 302.5227), 0.4f, 1),
            (new Vector3(-2088.515, -1011.628, 9.092273), new Vector3(0, 0, 167.3026), 0.4f, 1),
            (new Vector3(-2087.191, -1012.347, 9.051127), new Vector3(0, 0, 132.0729), 0.4f, 1),
            (new Vector3(-2086.847, -1013.487, 9.051119), new Vector3(0, 0, 80.84064), 0.4f, 1),
            (new Vector3(-2088.425, -1022.208, 9.050717), new Vector3(0, 0, 348.2245), 0.4f, 1),
            (new Vector3(-2080.541, -1017.142, 9.051137), new Vector3(0, 0, 253.8865), 0.4f, 1),
            (new Vector3(-2080.151, -1015.84, 9.051137), new Vector3(0, 0, 253.6785), 0.4f, 1),
            (new Vector3(-2082.052, -1021.662, 9.051151), new Vector3(0, 0, 254.3518), 0.4f, 1),
            (new Vector3(-2082.468, -1022.935, 9.051151), new Vector3(0, 0, 248.9526), 0.4f, 1),
            (new Vector3(-2055.171, -1028.467, 9.051496), new Vector3(0, 0, 252.2709), 0.4f, 1),
            (new Vector3(-2054.687, -1026.989, 9.051496), new Vector3(0, 0, 253.3522), 0.4f, 1),
            (new Vector3(-2037.131, -1028.367, 9.049433), new Vector3(0, 0, 123.0036), 0.4f, 1),
            (new Vector3(-2039.118, -1037.229, 9.051499), new Vector3(0, 0, 38.37067), 0.4f, 1),
            (new Vector3(-2063.237, -1023.989, 11.98824), new Vector3(0, 0, 162.2618), 0.4f, 1),
            (new Vector3(-2064.569, -1021.644, 11.9889), new Vector3(0, 0, 72.96848), 0.4f, 1),
            (new Vector3(-2066.25, -1020.312, 11.98952), new Vector3(0, 0, 163.5244), 0.4f, 1),
            (new Vector3(-2064.157, -1026.019, 11.98835), new Vector3(0, 0, 345.3992), 0.4f, 1),
            (new Vector3(-2066.417, -1027.246, 11.9889), new Vector3(0, 0, 68.68312), 0.4f, 1),
            (new Vector3(-2069.016, -1027.228, 11.98793), new Vector3(0, 0, 342.0817), 0.4f, 1), // Корабль на пляже
            (new Vector3(1968.757, 3816.947, 34.09149), new Vector3(0, 0, 31.47821), 0.7f, 2), // Интерьер Trailer 
            (new Vector3(154.2344, -1002.779, -99.02), new Vector3(0, 0, 108.9226), 0.7f, 1), // Интерьер дома Econom
            (new Vector3(154.2024, -1005.229, -98.33931), new Vector3(0, 0, 264.799), 0.7f, 2), // Интерьер дома Econom
            (new Vector3(260.5116, -996.6968, -99.0287), new Vector3(0, 0, 96.64272), 0.7f, 1), // Интерьер дома Econom+
            (new Vector3(257.0592, -998.3367, -99.0287), new Vector3(0, 0, 357.7034), 0.7f, 1),
            (new Vector3(259.1831, -995.6998, -99.0287), new Vector3(0, 0, 175.5242), 0.7f, 1),
            (new Vector3(262.3006, -1003.767, -98.21188), new Vector3(0, 0, 268.2592), 0.7f, 2), // Интерьер дома Econom+
            (new Vector3(350.094, -996.1761, -98.45984), new Vector3(0, 0, 90.15872), 0.7f, 2), // Интерьер дома Comfort
            (new Vector3(338.8177, -998.6385, -99.1163), new Vector3(0, 0, 316.9797), 0.7f, 1),
            (new Vector3(339.812, -994.9097, -99.1163),  new Vector3(0, 0, 177.802), 0.7f, 1),
            (new Vector3(342.0492, -994.9193, -99.1163), new Vector3(0, 0, 177.5264), 0.7f, 1),
            (new Vector3(342.3287, -997.376, -99.1162), new Vector3(0, 0, 87.59706), 0.7f, 1), // Интерьер дома Comfort
            (new Vector3(-23.42951, -584.3572, 79.34034), new Vector3(0, 0, 339.8787), 0.7f, 1), // Интерьер дома Comfort+
            (new Vector3(-25.46402, -583.3936, 79.31127), new Vector3(0, 0, 246.592), 0.7f, 1),
            (new Vector3(-26.75363, -578.762, 79.31076), new Vector3(0, 0, 242.6363), 0.7f, 1),
            (new Vector3(-21.48416, -582.4904, 79.31076), new Vector3(0, 0, 71.16576), 0.7f, 1),
            (new Vector3(-22.04705, -584.1982, 79.31076), new Vector3(0, 0, 65.65177), 0.7f, 1),
            (new Vector3(-35.84179, -583.2993, 79.58081), new Vector3(0, 0, 65.62753), 0.7f, 2), // Интерьер дома Comfort+
            (new Vector3(-31.79673, -578.1633, 88.79229), new Vector3(0, 0, 12.20056), 0.7f, 1), // Интерьер дома Premium
            (new Vector3(-33.79584, -577.5692, 88.79228), new Vector3(0, 0, 312.3738), 0.7f, 1),
            (new Vector3(-33.13091, -575.0956, 88.79227), new Vector3(0, 0, 202.5942), 0.7f, 1),
            (new Vector3(-38.23046, -575.2223, 88.81314), new Vector3(0, 0, 66.67116), 0.7f, 1),
            (new Vector3(-40.24874, -577.2188, 88.81314), new Vector3(0, 0, 336.9978), 0.7f, 1),
            (new Vector3(-30.67855, -576.8135, 83.99729), new Vector3(0, 0, 69.69758), 0.7f, 1),
            (new Vector3(-31.49006, -575.174, 83.99738), new Vector3(0, 0, 154.9051), 0.7f, 1),
            (new Vector3(-36.52985, -577.4648, 84.72961), new Vector3(0, 0, 66.80305), 0.7f, 2),
            (new Vector3(-36.06441, -576.1142, 84.70415), new Vector3(0, 0, 68.66934), 0.7f, 2), // Интерьер дома Premium
            (new Vector3(-164.5555, 484.0437, 137.3453), new Vector3(0, 0, 140.3974), 0.7f, 1), // Интерьер дома Premium+
            (new Vector3(-165.663, 481.1551, 137.3453), new Vector3(0, 0, 11.54526), 0.7f, 1),
            (new Vector3(-167.0874, 482.44, 137.3453), new Vector3(0, 0, 279.4397), 0.7f, 1),
            (new Vector3(-163.5424, 479.7116, 133.9241), new Vector3(0, 0, 140.6945), 0.7f, 1),
            (new Vector3(-163.6568, 482.6699, 134.6408), new Vector3(0, 0, 282.9413), 0.7f, 2),
            (new Vector3(-163.8307, 484.0913, 134.6408), new Vector3(0, 0, 282.9413), 0.7f, 2), // Интерьер дома Premium+
            //(new Vector3(267.2737, -1352.634, 24.61928), new Vector3(0, 0, 136.6982), 0.7f, 1), // Интерьер EMS
            //(new Vector3(269.0605, -1354.086, 24.61781), new Vector3(0, 0, 138.8217), 0.7f, 1), // Интерьер EMS
            (new Vector3(-507.1049, -244.0915, 35.9686), new Vector3(0, 0, 297.827), 0.3f, 1), // Улица мэрии
            (new Vector3(-506.0705, -245.9754, 35.91119), new Vector3(0, 0, 296.3301), 0.3f, 1),
            (new Vector3(-516.7624, -247.338, 35.79747), new Vector3(0, 0, 208.0985), 0.3f, 1),
            (new Vector3(-516.5984, -249.3799, 35.76442), new Vector3(0, 0, 257.7004), 0.3f, 1),
            (new Vector3(-519.5103, -251.4794, 35.75371), new Vector3(0, 0, 172.0593), 0.3f, 1),
            (new Vector3(-521.8481, -250.3458, 35.80121), new Vector3(0, 0, 207.4373), 0.3f, 1),
            (new Vector3(-543.2169, -229.9959, 37.68633), new Vector3(0, 0, 29.33541), 0.3f, 1),
            (new Vector3(-544.8109, -230.907, 37.69157), new Vector3(0, 0, 31.24411), 0.3f, 1),
            (new Vector3(-546.5527, -231.9019, 37.69157), new Vector3(0, 0, 30.13134), 0.3f, 1),
            (new Vector3(-548.2082, -232.8438, 37.69157), new Vector3(0, 0, 30.51971), 0.3f, 1),
            (new Vector3(-549.8681, -233.7165, 37.69157), new Vector3(0, 0, 30.12765), 0.3f, 1),
            (new Vector3(-523.4792, -218.3726, 37.68925), new Vector3(0, 0, 28.35603), 0.3f, 1),
            (new Vector3(-521.9556, -217.4952, 37.69152), new Vector3(0, 0, 26.62707), 0.3f, 1),
            (new Vector3(-520.238, -216.498, 37.69152), new Vector3(0, 0, 25.33274), 0.3f, 1),
            (new Vector3(-518.4054, -215.3863, 37.69152), new Vector3(0, 0, 25.84767), 0.3f, 1),
            (new Vector3(-516.4008, -214.222, 37.69152), new Vector3(0, 0, 26.10501), 0.3f, 1),
            (new Vector3(-455.4898, -326.9427, 34.58211), new Vector3(0, 0, 170.6101), 0.25f, 1),
            (new Vector3(-454.0421, -327.1746, 34.58196), new Vector3(0, 0, 171.9569), 0.25f, 1),
            (new Vector3(-451.7261, -327.4619, 34.59062), new Vector3(0, 0, 170.8979), 0.25f, 1),
            (new Vector3(-450.3082, -327.6694, 34.58229), new Vector3(0, 0, 174.0088), 0.25f, 1),
            (new Vector3(-452.5229, -353.1263, 34.58231), new Vector3(0, 0, 347.7034), 0.25f, 1),
            (new Vector3(-454.2354, -352.8871, 34.58241), new Vector3(0, 0, 351.2257), 0.25f, 1),
            (new Vector3(-456.6526, -352.5455, 34.58153), new Vector3(0, 0, 351.0021), 0.25f, 1),
            (new Vector3(-458.3289, -352.3105, 34.58181), new Vector3(0, 0, 352.081), 0.25f, 1), // Улица мэрии
            (new Vector3(3291.016, 5190.123, 18.48667), new Vector3(0, 0, 194.6641), 0.5f, 1), // SPAWN с квестами
            (new Vector3(-1368.194, -1414.618, 3.468614), new Vector3(0, 0, 154.5377), 0.25f, 1), // Скамейки у пляжа
            (new Vector3(-1369.562, -1413.916, 3.441031), new Vector3(0, 0, 155.6581), 0.25f, 1),
            (new Vector3(-1372.018, -1413.421, 3.442034), new Vector3(0, 0, 164.2935), 0.25f, 1),
            (new Vector3(-1373.443, -1413.233, 3.450541), new Vector3(0, 0, 163.2313), 0.25f, 1),
            (new Vector3(-1375.178, -1431.089, 3.653083), new Vector3(0, 0, 83.60991), 0.25f, 1),
            (new Vector3(-1375.219, -1432.879, 3.753905), new Vector3(0, 0, 89.17176), 0.25f, 1),
            (new Vector3(-1375.249, -1432.881, 3.655956), new Vector3(0, 0, 89.17176), 0.25f, 1),
            (new Vector3(-1375.857, -1435.228, 3.670147), new Vector3(0, 0, 71.05381), 0.25f, 1),
            (new Vector3(-1376.45, -1436.844, 3.683812), new Vector3(0, 0, 70.15248), 0.25f, 1),
            (new Vector3(-1378.342, -1439.369, 3.823837), new Vector3(0, 0, 37.06582), 0.25f, 1),
            (new Vector3(-1379.731, -1440.406, 3.947208), new Vector3(0, 0, 39.19855), 0.25f, 1),
            (new Vector3(-1680.472, -1030.658, 12.99744), new Vector3(0, 0, 53.65477), 0.25f, 1),
            (new Vector3(-1681.872, -1032.383, 12.9974), new Vector3(0, 0, 50.15931), 0.25f, 1),
            (new Vector3(-1691.04, -1043.447, 12.9974), new Vector3(0, 0, 50.03595), 0.25f, 1),
            (new Vector3(-1692.45, -1045.126, 12.9974), new Vector3(0, 0, 53.3396), 0.25f, 1),
            (new Vector3(-1690.256, -1050.226, 12.99736), new Vector3(0, 0, 50.50749), 0.25f, 1),
            (new Vector3(-1691.557, -1051.936, 12.99736), new Vector3(0, 0, 48.94553), 0.25f, 1),
            (new Vector3(-1688.748, -1051.443, 12.95932), new Vector3(0, 0, 226.7898), 0.25f, 1),
            (new Vector3(-1690.041, -1053.016, 12.94821), new Vector3(0, 0, 231.9322), 0.25f, 1),
            (new Vector3(-1696.554, -1057.92, 12.99736), new Vector3(0, 0, 46.80936), 0.25f, 1),
            (new Vector3(-1697.842, -1059.447, 12.99736), new Vector3(0, 0, 48.30764), 0.25f, 1),
            (new Vector3(-1695.152, -1059.125, 12.913), new Vector3(0, 0, 228.3774), 0.25f, 1),
            (new Vector3(-1696.545, -1060.775, 12.91272), new Vector3(0, 0, 228.317), 0.25f, 1),
            (new Vector3(-1713.425, -1070.69, 12.99736), new Vector3(0, 0, 48.2088), 0.25f, 1),
            (new Vector3(-1714.767, -1072.282, 12.99736), new Vector3(0, 0, 50.29316), 0.25f, 1),
            (new Vector3(-1726.771, -1086.595, 12.99744), new Vector3(0, 0, 48.05569), 0.25f, 1),
            (new Vector3(-1728.132, -1088.2, 12.99744), new Vector3(0, 0, 47.59551), 0.25f, 1),
            (new Vector3(-1738.294, -1100.405, 12.99744), new Vector3(0, 0, 48.47839), 0.25f, 1),
            (new Vector3(-1739.724, -1102.119, 12.99744), new Vector3(0, 0, 50.60358), 0.25f, 1),
            (new Vector3(-1749.777, -1114.013, 12.99926), new Vector3(0, 0, 48.85592), 0.25f, 1),
            (new Vector3(-1751.091, -1115.574, 12.99939), new Vector3(0, 0, 48.7722), 0.25f, 1),
            (new Vector3(-1760.715, -1127.104, 12.99917), new Vector3(0, 0, 47.54485), 0.25f, 1),
            (new Vector3(-1762.076, -1128.619, 12.99925), new Vector3(0, 0, 46.62714), 0.25f, 1),
            (new Vector3(-1772.254, -1140.891, 12.9992), new Vector3(0, 0, 51.55354), 0.25f, 1),
            (new Vector3(-1773.599, -1142.489, 12.99894), new Vector3(0, 0, 48.58392), 0.25f, 1),
            (new Vector3(-1783.815, -1154.738, 12.99868), new Vector3(0, 0, 46.50961),  0.25f, 1),
            (new Vector3(-1785.284, -1156.335, 12.99872), new Vector3(0, 0, 46.11556), 0.25f, 1),
            (new Vector3(-1795.505, -1168.509, 12.99763), new Vector3(0, 0, 48.97336), 0.25f, 1),
            (new Vector3(-1796.839, -1170.097, 12.99761), new Vector3(0, 0, 47.42951), 0.25f, 1),
            (new Vector3(-1784.803, -1180.086, 12.98921), new Vector3(0, 0, 230.4366), 0.25f, 1),
            (new Vector3(-1783.545, -1178.583, 12.99771), new Vector3(0, 0, 228.6959), 0.25f, 1),
            (new Vector3(-1773.292, -1166.374, 12.99798), new Vector3(0, 0, 233.1375), 0.25f, 1),
            (new Vector3(-1771.867, -1164.667, 12.998), new Vector3(0, 0, 227.1054), 0.25f, 1),
            (new Vector3(-1761.67, -1152.503, 12.9994), new Vector3(0, 0, 231.723), 0.25f, 1),
            (new Vector3(-1760.291, -1150.873, 12.99823), new Vector3(0, 0, 229.6537), 0.25f, 1),
            (new Vector3(-1750.134, -1138.835, 12.99466), new Vector3(0, 0, 229.9133), 0.25f, 1),
            (new Vector3(-1748.794, -1137.185, 12.99846), new Vector3(0, 0, 229.6521), 0.25f, 1),
            (new Vector3(-1738.543, -1124.953, 12.99792), new Vector3(0, 0, 230.6734), 0.25f, 1),
            (new Vector3(-1737.186, -1123.336, 12.99764), new Vector3(0, 0, 226.7657), 0.25f, 1), // Скамейки у пляжа

            //(new Vector3(249.6037, -1355.289, 25.55439), new Vector3(0, 0, 226.06), 2f, 3), // Операционные столы EMS
            //(new Vector3(262.0228, -1339.945, 25.55439), new Vector3(0, 0, 226.06), 2f, 3), // Операционные столы EMS
            (new Vector3(-525.555, -237.95, 36.07), new Vector3(0, 0, 210.51), 1f, 4), // Мэрия
            (new Vector3(-556.1727, -267.5518, 35.33778), new Vector3(0, 0, 322.0088), 1f, 4),
            (new Vector3(-553.7198, -268.4829, 35.3008), new Vector3(0, 0, 8.469508), 1f, 5), // Мэрия
            (new Vector3(-1201.239, -1575.078, 4.086289), new Vector3(0, 0, 214.0948), 0.5f, 6), // Пляж
            (new Vector3(-1197.888, -1568.199, 4.08697), new Vector3(0, 0, 301.7134), 0.5f, 6),
            (new Vector3(-1200.752, -1562.193, 4.089676), new Vector3(0, 0, 122.7432), 0.5f, 6),
            (new Vector3(-1207.029, -1560.949, 4.097784), new Vector3(0, 0, 212.489), 0.5f, 6),
            (new Vector3(-1200.026, -1571.181, 4.209402), new Vector3(0, 0, 212.0657), 0.5f, 7),
            (new Vector3(-1204.75, -1564.306, 4.209529), new Vector3(0, 0, 33.87825), 0.5f, 7),
            (new Vector3(-1197.03, -1572.891, 4.29253), new Vector3(0, 0, 34.95492), 0.5f, 8),
            (new Vector3(-1199.09, -1574.349, 4.289591), new Vector3(0, 0, 33.97555), 0.5f, 8),
            (new Vector3(-1202.857, -1565.315, 4.291592), new Vector3(0, 0, 215.2443), 0.5f, 8),
            (new Vector3(-1210.476, -1561.323, 4.287892), new Vector3(0, 0, 254.0813), 0.5f, 8),
            (new Vector3(-1204.549, -1557.316, 4.298352), new Vector3(0, 0, 338.1349), 0.5f, 9),
            (new Vector3(-1198.543, -1566.187, 4.297987), new Vector3(0, 0, 304.2291), 0.5f, 9),
            (new Vector3(-1194.741, -1570.596, 4.299261), new Vector3(0, 0, 304.8279), 0.5f, 9),
            (new Vector3(-1207.855, -1566.285, 4.287902), new Vector3(0, 0, 124.7011), 0.5f, 10),
            (new Vector3(-1204.562, -1560.703, 4.29422), new Vector3(0, 0, 29.44464), 0.5f, 10), // Пляж
            (new Vector3(-1964.171, 3341.724, 32.04028), new Vector3(0, 0, 240.8465), 0.5f, 7), // Армия
            (new Vector3(-1967.376, 3336.494, 32.04028), new Vector3(0, 0, 239.6491), 0.5f, 7),
            (new Vector3(-1970.769, 3330.298, 32.04022), new Vector3(0, 0, 240.4763), 0.5f, 7), // Армия
            (new Vector3(-551.5037, 282.7206, 82.80671), new Vector3(0, 0, 79.2663), 0.7f, 11), // Tequi-la-la
            (new Vector3(-550.739, 286.2113, 82.80671), new Vector3(0, 0, 98.59238), 0.7f, 11), // Tequi-la-la
            (new Vector3(-525.5313, -246.77213, 35.79249), new Vector3(0, 0, 115.86966), 1f, 1), // Мэрия
            (new Vector3(-527.9317, -242.51056, 35.842697), new Vector3(0, 0, 123.09466), 1f, 1), // Мэрия
            (new Vector3(-520.35876, -237.71443, 35.853596), new Vector3(0, 0, -43.81939), 1f, 1), // Мэрия
            (new Vector3(-517.90356, -242.17949, 35.799976), new Vector3(0, 0, -43.81939), 1f, 1), // Мэрия
            
            (new Vector3(245.63101, -1095.6752, 29.294406), new Vector3(0, 0, 178.59708), 1f, 1), // Суд
            (new Vector3(247.37993, -1095.6162, 29.294092), new Vector3(0, 0, 178.59708), 1f, 1), // Суд
            (new Vector3(245.51399, -1097.6045, 29.272692), new Vector3(0, 0, 178.59708), 1f, 1), // Суд
            (new Vector3(247.4201, -1097.6577, 29.294062), new Vector3(0, 0, 178.59708), 1f, 1), // Суд
            
            (new Vector3(253.73897, -1095.6892, 29.294054), new Vector3(0, 0, 178.63147), 1f, 1), // Суд
            (new Vector3(255.5613, -1095.6162, 29.294054), new Vector3(0, 0, 178.63147), 1f, 1), // Суд
            (new Vector3(255.45453, -1097.5977, 29.33004), new Vector3(0, 0, 178.63147), 1f, 1), // Суд
            (new Vector3(253.75241, -1097.686, 29.294046), new Vector3(0, 0, 178.63147), 1f, 1), // Суд
            
            (new Vector3(258.78708, -1098.5331, 29.62799), new Vector3(0, 0, 89.59381), 1f, 1), // Суд
            (new Vector3(258.74976, -1100.292, 29.624151), new Vector3(0, 0, 89.59381), 1f, 1), // Суд
            (new Vector3(256.97693, -1100.337, 29.294052), new Vector3(0, 0, 89.59381), 1f, 1), // Суд
            (new Vector3(256.96606, -1098.6709, 29.294058), new Vector3(0, 0, 89.59381), 1f, 1), // Суд
            
            (new Vector3(258.7765, -1104.2365, 29.62417), new Vector3(0, 0, 89.59381), 1f, 1), // Суд
            (new Vector3(258.77292, -1102.4226, 29.62417), new Vector3(0, 0, 89.59381), 1f, 1), // Суд
            (new Vector3(256.95359, -1102.3887, 29.294031), new Vector3(0, 0, 89.59381), 1f, 1), // Суд
            (new Vector3(256.97687, -1104.195, 29.294098), new Vector3(0, 0, 89.59381), 1f, 1), // Суд
            
            (new Vector3(248.00903, -1099.4596, 29.294065), new Vector3(0, 0, 178.64291), 1f, 1), // Суд
            (new Vector3(246.63104, -1099.4838, 29.294065), new Vector3(0, 0, 177.12475), 1f, 1), // Суд
            
            (new Vector3(253.2124, -1099.4165, 29.294052), new Vector3(0, 0, -175.3877), 1f, 1), // Суд
            (new Vector3(254.55447, -1099.4263, 29.294064), new Vector3(0, 0, -178.34415), 1f, 1), // Суд
            
            (new Vector3(245.1042, -1104.775, 29.461792), new Vector3(0, 0, -94.9616), 1f, 1), // Суд
            
            (new Vector3(250.26012, -1109.0529, 29.627295), new Vector3(0, 0, -3.0763717), 1f, 1), // Суд
            (new Vector3(252.59921, -1108.6757, 30.081997), new Vector3(0, 0, 19.710163), 1f, 1), // Суд
            (new Vector3(254.78953, -1108.837, 29.827332), new Vector3(0, 0, 1.570315), 1f, 1), // Суд
            
            (new Vector3(-510.63446, -224.39435, 36.679146), new Vector3(0, 0, -101.304245), 1f, 1), // Мэрия
            (new Vector3(-510.8661, -225.701, 36.633587), new Vector3(0, 0, -100.80577), 1f, 1), // Мэрия
            (new Vector3(-510.10718, -228.32625, 36.529552), new Vector3(0, 0, -37.900745), 1f, 1), // Мэрия
            (new Vector3(-509.0776, -229.05573, 36.5485), new Vector3(0, 0, -30.360613), 1f, 1), // Мэрия
            (new Vector3(-506.75638, -229.13724, 36.483128), new Vector3(0, 0, 34.085674), 1f, 1), // Мэрия
            (new Vector3(-505.6481, -228.4775, 36.489376), new Vector3(0, 0, 33.26634), 1f, 1), // Мэрия
            (new Vector3(-504.93524, -226.30446, 36.540863), new Vector3(0, 0, 105.20692), 1f, 1), // Мэрия
            (new Vector3(-505.1972, -225.0365, 36.556145), new Vector3(0, 0, 97.90744), 1f, 1), // Мэрия
            
            (new Vector3(-1278.9905, -571.75714, 31.712172), new Vector3(0, 0, -48.070747), 1f, 1), // Новая Мэрия
            (new Vector3(-1277.9745, -573.0601, 31.712172), new Vector3(0, 0, -49.6521), 1f, 1), // Новая Мэрия
            (new Vector3(-1270.8517, -583.0032, 29.32755), new Vector3(0, 0, -38.794556), 1f, 1), // Новая Мэрия
            (new Vector3(-1269.7611, -584.27704, 29.302074), new Vector3(0, 0, -51.743393), 1f, 1), // Новая Мэрия
            (new Vector3(-1263.9956, -584.8861, 29.302065), new Vector3(0, 0, 130.503), 1f, 1), // Новая Мэрия
            (new Vector3(-1262.8053, -586.2113, 29.269066), new Vector3(0, 0, 132.11302), 1f, 1), // Новая Мэрия
            (new Vector3(-1263.1339, -592.18414, 29.275679), new Vector3(0, 0, -50.001614), 1f, 1), // Новая Мэрия
            (new Vector3(-1261.9701, -593.5378, 29.235434), new Vector3(0, 0, -55.83432), 1f, 1), // Новая Мэрия
            (new Vector3(-1256.639, -593.54755, 29.33059), new Vector3(0, 0, 128.81279), 1f, 1), // Новая Мэрия
            (new Vector3(-1255.5455, -594.87274, 29.302027), new Vector3(0, 0, 128.6065), 1f, 1), // Новая Мэрия
            
            (new Vector3(-1288.768, -560.0561, 31.684565), new Vector3(0, 0, -57.805294), 1f, 1), // Новая Мэрия
            (new Vector3(-1289.8661, -558.6991, 31.712177), new Vector3(0, 0, -51.600163), 1f, 1), // Новая Мэрия
            (new Vector3(-1290.5563, -553.1663, 31.741482), new Vector3(0, 0, 141.95645), 1f, 1), // Новая Мэрия
            (new Vector3(-1291.548, -551.9449, 31.712126), new Vector3(0, 0, 127.44915), 1f, 1), // Новая Мэрия
            (new Vector3(-1296.6676, -552.22406, 31.71208), new Vector3(0, 0, -56.321327), 1f, 1), // Новая Мэрия
            (new Vector3(-1297.6648, -551.0398, 31.64346), new Vector3(0, 0, -45.372112), 1f, 1), // Новая Мэрия
            (new Vector3(-1297.629, -544.6461, 31.712173), new Vector3(0, 0, 131.34526), 1f, 1), // Новая Мэрия
            (new Vector3(-1298.6826, -543.4861, 31.712173), new Vector3(0, 0, 123.501465), 1f, 1), // Новая Мэрия
            (new Vector3(-1304.6913, -542.74396, 31.74369), new Vector3(0, 0, -55.570076), 1f, 1), // Новая Мэрия
            (new Vector3(-1305.7024, -541.5306, 31.711874), new Vector3(0, 0, -55.12958), 1f, 1), // Новая Мэрия
        };
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            for (int i = 0; i < SitingPos.Length; i++)
            {
                CustomColShape.CreateCylinderColShape(SitingPos[i].Item1, SitingPos[i].Item3, 2, colShapeEnums: ColShapeEnums.SeatingArrangements, Index: i);
            }
        }
        [Interaction(ColShapeEnums.SeatingArrangements)]
        public static void OnSeatingArrangements(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                else if (player.IsInVehicle) return;
                //else if (SitToPlayerId.ContainsKey(index) && SitToPlayerId.) return;
                if (sessionData.Following != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вас кто-то тащит за собой", 3000);
                    return;
                }
                int nearest = index;
                (Vector3, Vector3, float, byte) sitdata = SitingPos[nearest];
                byte sittype = sitdata.Item4;
                if (!SitToPlayerId.ContainsKey(index))
                {
                    if (sessionData.AntiAnimDown)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Чтобы взаимодействовать - нужно перестать использовать другие анимации.", 3000);
                        return;
                    }
                    SitToPlayerId.Add(index, player.Value);
                    sessionData.SitPos = index;
                    Trigger.ClientEventInRange(sitdata.Item1, 250f, "setClientRotation", player.Value, sitdata.Item2.Z);
                    player.Rotation = sitdata.Item2;
                    if (sittype == 6) Trigger.ClientEvent(player, "freeze", true);
                    player.Position = sitdata.Item1;
                    Main.OnAntiAnim(player);
                    switch (sittype)
                    {
                        case 1: // Сидеть облокотившись 
                            Trigger.PlayAnimation(player, "switch@michael@sitting", "idle", 39);
                            // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "sit");
                            Trigger.ClientEvent(player, "freeze", true);
                            break;
                        case 2: // Лежать
                            Trigger.PlayAnimation(player, "amb@world_human_sunbathe@male@back@base", "base", 39);
                            // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "lezhat");
                            Trigger.ClientEvent(player, "freeze", true);
                            break;
                        case 3: // Лежать на операционном столе
                            Trigger.PlayAnimation(player, "missheistfbi3b_ig8_2", "cower_loop_victim", 39);
                            // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "olezhat");
                            Trigger.ClientEvent(player, "freeze", true);
                            break;
                        case 4: // Играть на гитаре
                            Commands.RPChat("sme", player, "играет на гитаре");
                            Trigger.PlayAnimation(player, "amb@world_human_musician@guitar@male@base", "base", 49);
                            // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "pguitar");
                            Attachments.AddAttachment(player, Attachments.AttachmentsName.Guitar);
                            Trigger.ClientEvent(player, "freeze", true);
                            break;
                        case 5: // Играть на бонго
                            Commands.RPChat("sme", player, "играет на бонго");
                            Trigger.PlayAnimation(player, "amb@world_human_musician@bongos@male@base", "base", 49);
                            // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "pbongo");
                            Attachments.AddAttachment(player, Attachments.AttachmentsName.Bongo);
                            Trigger.ClientEvent(player, "freeze", true);
                            break;
                        case 6: // Жим лёжа
                            Trigger.PlayAnimation(player, "amb@prop_human_seat_muscle_bench_press@idle_a", "idle_a", 39);
                            // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "jim");
                            Attachments.AddAttachment(player, Attachments.AttachmentsName.Press1);
                            break;
                        case 7: // Подтягивание
                            Trigger.PlayAnimation(player, "amb@prop_human_muscle_chin_ups@male@base", "base", 39);
                            // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "tyag");
                            Trigger.ClientEvent(player, "fullblockMove", true);
                            break;
                        case 8: // Жим стоя
                            Trigger.PlayAnimation(player, "amb@world_human_muscle_free_weights@male@barbell@base", "base", 39);
                            // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "jimstoya");
                            Attachments.AddAttachment(player, Attachments.AttachmentsName.Press2);
                            Trigger.ClientEvent(player, "fullblockMove", true);
                            break;
                        case 9: // Качать пресс
                            Trigger.PlayAnimation(player, "amb@world_human_sit_ups@male@base", "base", 39);
                            // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "press");
                            Trigger.ClientEvent(player, "freeze", true);
                            break;
                        case 10: // Отжиматься
                            Trigger.PlayAnimation(player, "amb@world_human_push_ups@male@base", "base", 39);
                            // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "otjim");
                            Trigger.ClientEvent(player, "freeze", true);
                            break;
                        case 11: // Играть на электрической гитаре
                            Commands.RPChat("sme", player, "играет на электрогитаре");
                            Trigger.PlayAnimation(player, "amb@world_human_musician@guitar@male@base", "base", 49);
                            // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "playeguitar");
                            Attachments.AddAttachment(player, Attachments.AttachmentsName.ElGuitar);
                            Trigger.ClientEvent(player, "freeze", true);
                            break;
                        default:
                            // Not supposed to end up here. 
                            break;
                    }
                    if (sittype >= 6 && sittype <= 10)
                    {
                        if (sessionData.TimersData.HealTimer != null)
                        {
                            Timers.Stop(sessionData.TimersData.HealTimer);
                            sessionData.TimersData.HealTimer = null;
                        }
                        if (player.Health <= 99) sessionData.TimersData.HealTimer = Timers.Start(3750, () => gymHealTimer(player), true);
                        if (!characterData.Achievements[22])
                        {
                            characterData.Achievements[22] = true;
                            Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, "Спорт укрепляет здоровье!", 5000);
                        }
                    }
                    return;
                }
                else if (SitToPlayerId[index] == player.Value)
                {
                    switch (sittype)
                    {
                        case 4:
                            Attachments.RemoveAttachment(player, Attachments.AttachmentsName.Guitar);
                            break;
                        case 5:
                            Attachments.RemoveAttachment(player, Attachments.AttachmentsName.Bongo);
                            break;
                        case 6:
                            Attachments.RemoveAttachment(player, Attachments.AttachmentsName.Press1);
                            break;
                        case 8:
                            Attachments.RemoveAttachment(player, Attachments.AttachmentsName.Press2);
                            break;
                        case 11:
                            Attachments.RemoveAttachment(player, Attachments.AttachmentsName.ElGuitar);
                            break;
                        default:
                            // Not supposed to end up here. 
                            break;
                    }
                    SitToPlayerId.Remove(index);
                    sessionData.SitPos = -1;
                    player.Position = player.Position + new Vector3(0, 0, 0.5);
                    Trigger.StopAnimation(player);
                    if (sittype == 7 || sittype == 8) Trigger.ClientEvent(player, "fullblockMove", false);
                    else Trigger.ClientEvent(player, "freeze", false);
                    if (sittype >= 6 && sittype <= 8)
                    {
                        if (sessionData.TimersData.HealTimer != null)
                        {
                            Timers.Stop(sessionData.TimersData.HealTimer);
                            sessionData.TimersData.HealTimer = null;
                        }
                    }
                    Main.OffAntiAnim(player);
                }
                else
                {
                    ExtPlayer target = Main.GetPlayerByID(SitToPlayerId[index]);
                    var targetSessionData = target.GetSessionData();
                    if (target != null && targetSessionData != null)
                    {
                        if (target.Position.DistanceTo(sitdata.Item1) <= 10 && Fractions.Configs.IsFractionPolic(memberFractionData.Id))
                        {
                            switch (sittype)
                            {
                                case 4:
                                    Attachments.RemoveAttachment(target, Attachments.AttachmentsName.Guitar);
                                    break;
                                case 5:
                                    Attachments.RemoveAttachment(target, Attachments.AttachmentsName.Bongo);
                                    break;
                                case 6:
                                    Attachments.RemoveAttachment(target, Attachments.AttachmentsName.Press1);
                                    break;
                                case 8:
                                    Attachments.RemoveAttachment(target, Attachments.AttachmentsName.Press2);
                                    break;
                                case 11:
                                    Attachments.RemoveAttachment(target, Attachments.AttachmentsName.ElGuitar);
                                    break;
                                default:
                                    // Not supposed to end up here. 
                                    break;
                            }
                            SitToPlayerId.Remove(index);
                            targetSessionData.SitPos = -1;
                            target.Position = player.Position + new Vector3(0, 0, 0.5);
                            Trigger.StopAnimation(target);
                            if (sittype == 7 || sittype == 8) Trigger.ClientEvent(target, "fullblockMove", false);
                            else Trigger.ClientEvent(target, "freeze", false);
                            if (sittype >= 6 && sittype <= 8)
                            {
                                if (targetSessionData != null && targetSessionData.TimersData.HealTimer != null)
                                {
                                    Timers.Stop(targetSessionData.TimersData.HealTimer);
                                    targetSessionData.TimersData.HealTimer = null;
                                }
                            }
                            Main.OffAntiAnim(target);
                            Commands.RPChat("sme", player, "поднял {name} с места.", target);
                            Notify.Send(target, NotifyType.Alert, NotifyPosition.BottomCenter, $"Человек ({player.Value}) поднял Вас с места.", 1000);
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы подняли человека ({target.Value}) с места.", 1000);
                        }
                        else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "К сожалению, это место сейчас занято другим человеком", 1500);
                    }
                    else
                    {
                        SitToPlayerId.Remove(index);
                        OnSeatingArrangements(player, index);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"OpenSeatingArrangements Exception: {e.ToString()}");
            }
        }
        private static void gymHealTimer(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                if (!player.IsCharacterData()) return;
                if (player.Health == 100)
                {
                    if (sessionData.TimersData.HealTimer != null)
                    {
                        Timers.Stop(sessionData.TimersData.HealTimer);
                        sessionData.TimersData.HealTimer = null;
                    }
                    return;
                }
                switch (accountData.VipLvl)
                {
                    case 2:
                    case 3:
                        if (player.Health + 2 > 100) player.Health = 100;
                        else player.Health = player.Health + 2;
                        break;
                    case 4:
                    case 5:
                        if (player.Health + 3 > 100) player.Health = 100;
                        else player.Health = player.Health + 3;
                        break;
                    default:
                        if (player.Health + 1 > 100) player.Health = 100;
                        player.Health = player.Health + 1;
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"gymHealTimer Exception: {e.ToString()}");
            }
        }*/
    }
}
