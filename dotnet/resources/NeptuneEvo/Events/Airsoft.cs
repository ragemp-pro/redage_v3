using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using Redage.SDK;
using NeptuneEvo.MoneySystem;
using System.Linq;
using Localization;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Functions;
using NeptuneEvo.Fractions;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Quests;

namespace NeptuneEvo.Events
{
    class LobbyData
    {
        public int LobbyIndex { get; set; } = 0;//
        public string LobbyName { get; set; } = "None";//
        public int LobbyPrice { get; set; } = 0;//
        public string LobbyPassword { get; set; } = null;//
        public int LobbyMode { get; set; } = 0;//
        [JsonIgnore]
        public int LobbyWeapon { get; set; } = 0;
        [JsonIgnore]
        public int LobbyMap { get; set; } = 0;
        [JsonIgnore]
        public int MaxPlayers { get; set; } = 4;

        [JsonIgnore]
        public int GameStatus { get; set; } = 0;
        
        [JsonIgnore]
        public int GameStartSeconds { get; set; } = 120;

        [JsonIgnore]
        public List<int> GameScore { get; set; } = new List<int>() // Для 1x1, 2x2, 3x3, 5x5
        {
            0, // №1
            0  // №2
        };

        [JsonIgnore]
        public Dictionary<string, int> GunGameTop { get; set; } = new Dictionary<string, int>(); // ТОП игроков на GunGame

        [JsonIgnore]
        public int GameFinalReward { get; set; } = 0; // Выплата на каждого члена победившей команды (1x1, 2x2, 3x3, 5x5) / выплата победителю GunGame

        [JsonIgnore]
        public string LobbyGameTimer { get; set; } = null;

        [JsonIgnore]
        public List<ExtPlayer> LobbyPlayers { get; set; } = new List<ExtPlayer>();

        public LobbyData(int index, string name, int price, string password, int mode, int weapon, int lobby_map, int max_players)
        {
            this.LobbyIndex = index;
            this.LobbyName = name;
            this.LobbyPrice = price;
            this.LobbyPassword = password;
            this.LobbyMode = mode;
            this.LobbyWeapon = weapon;
            this.LobbyMap = lobby_map;
            this.MaxPlayers = max_players;
            this.GameStatus = 0;
        }
    }

    class PlayerData
    {
        public int CorrectHealth { set; get; }
        public bool IsGunGamePlayer { set; get; }

        public PlayerData(int health, bool IsGGPlayer)
        {
            CorrectHealth = health;
            IsGunGamePlayer = IsGGPlayer;
        }
    }

    class AirsoftWeaponData
    {
        public ItemId weapon { set; get; }
        public int ammo { set; get; }

        public AirsoftWeaponData(ItemId Weapon, int Ammo)
        {
            weapon = Weapon;
            ammo = Ammo;
        }
    }

    class Airsoft : Script
    {
        private static readonly nLog Log = new nLog("Events.Airsoft");

        private static int DefaultDimension = 2000;

        private static Dictionary<int, LobbyData> LobbyList = new Dictionary<int, LobbyData>();
        public static Dictionary<ExtPlayer, PlayerData> AirsoftPlayerData = new Dictionary<ExtPlayer, PlayerData>();
        private static List<ExtPlayer> PlayersInLobbyMenu = new List<ExtPlayer>();

        private static List<(Vector3, byte, byte)> MapSpawnPoints = new List<(Vector3, byte, byte)>()
        {
            // Дамба:

            (new Vector3(1658.636, -78.4977, 172.0675), 0, 1),
            (new Vector3(1659.426, -79.80431, 172.0705), 0, 1),
            (new Vector3(1660.139, -80.96345, 172.0287), 0, 1),
            (new Vector3(1660.732, -81.92539, 171.9713), 0, 1),
            (new Vector3(1661.434, -83.07571, 171.8733), 0, 1),

            (new Vector3(1659.428, 47.95008, 172.2861), 0, 2),
            (new Vector3(1657.821, 48.8716, 172.3457), 0, 2),
            (new Vector3(1656.366, 49.79003, 172.35), 0, 2),
            (new Vector3(1654.956, 50.64669, 172.3357), 0, 2),
            (new Vector3(1653.523, 51.61367, 172.305), 0, 2),

            // Казик

            (new Vector3(1161.5743, 157.66281, 80.89255), 1, 1),
            (new Vector3(1165.3507, 151.5204, 80.89004), 1, 1),
            (new Vector3(1154.3582, 158.3516, 80.865036), 1, 1),
            (new Vector3(1148.903, 160.01735, 80.82878), 1, 1),
            (new Vector3(1150.7394, 159.21942, 80.86349), 1, 1),

            (new Vector3(1048.4023, 6.725812, 80.890526), 1, 2),
            (new Vector3(1052.0883, 4.631864, 80.86017), 1, 2),
            (new Vector3(1055.7745, 2.7529876, 80.86859), 1, 2),
            (new Vector3(1042.2897, 9.4974575, 80.88516), 1, 2),
            (new Vector3(1045.1577, 16.758682, 80.98129), 1, 2),

           
            // Vinewood

           (new Vector3(787.32544, 1278.7244, 360.29675), 2, 1),
           (new Vector3(784.3018, 1281.0427, 360.29675), 2, 1),
           (new Vector3(784.93805, 1292.2521, 360.29675), 2, 1),
           (new Vector3(784.1233, 1297.648, 360.29675), 2, 1),
           (new Vector3(794.6719, 1279.1412, 360.29675), 2, 1),

            (new Vector3(714.8777, 1293.9065, 360.29584), 2, 2),
            (new Vector3(720.0172, 1296.5592, 360.29584), 2, 2),
            (new Vector3(720.0142, 1298.8542, 360.29584), 2, 2),
            (new Vector3(720.87726, 1290.3125, 360.28217), 2, 2),
            (new Vector3(714.69244, 1295.3906, 360.29858), 2, 2),

            // Обсератория:

            (new Vector3(-386.5515, 1082.893, 324.6272), 3, 1),
            (new Vector3(-386.087, 1084.35, 324.6935), 3, 1),
            (new Vector3(-385.7624, 1085.431, 324.7668), 3, 1),
            (new Vector3(-385.4299, 1086.547, 324.8336), 3, 1),
            (new Vector3(-385.0197, 1087.852, 324.8899), 3, 1),

            (new Vector3(-477.2993, 1116.418, 325.8541), 3, 2),
            (new Vector3(-477.6686, 1115.33, 325.8541), 3, 2),
            (new Vector3(-477.8988, 1114.3, 325.8541), 3, 2),
            (new Vector3(-478.1629, 1113.265, 325.8541), 3, 2),
            (new Vector3(-478.5483, 1112.142, 325.8541), 3, 2),
            

            // Крыша:

            (new Vector3(-292.674, -718.317, 123.9976), 4, 1),

            (new Vector3(-272.2224, -743.1636, 123.9976), 4, 2),
            
            // Лесник
            
            (new Vector3(-676.59625, 5859.6304, 16.796423), 5, 1),
            (new Vector3(-664.17535, 5851.5425, 17.953878), 5, 1),
            (new Vector3(-659.8129, 5848.38, 18.63073), 5, 1),
            (new Vector3(-654.91016, 5843.676, 19.512486), 5, 1),
            (new Vector3(-668.557, 5855.3794, 17.423422), 5, 1),
            
            (new Vector3(-718.62366, 5758.066, 17.937656), 5, 2),
            (new Vector3(-710.37286, 5753.7915, 18.184124), 5, 2),
            (new Vector3(-705.79266, 5751.291, 18.247435), 5, 2),
            (new Vector3(-700.34906, 5748.516, 18.24305), 5, 2),
            (new Vector3(-693.55707, 5745.14, 18.088093), 5, 2),
            
            // Dust
            
            (new Vector3(-3901.8486, 7921.0444, 177.73676), 6, 1),
            (new Vector3(-3904.436, 7924.0757, 177.73676), 6, 1),
            (new Vector3(-3905.0227, 7921.5107, 177.73676), 6, 1),
            (new Vector3(-3898.397, 7921.8306, 177.73676), 6, 1),
            (new Vector3(-3898.026, 7923.9033, 177.73676), 6, 1),
            
            (new Vector3(-3925.0952, 7839.0728, 184.29262), 6, 2),
            (new Vector3(-3935.3145, 7842.4795, 184.13676), 6, 2),
            (new Vector3(-3913.291, 7840.182, 182.03775), 6, 2),
            (new Vector3(-3929.334, 7842.673, 184.1369), 6, 2),
            (new Vector3(-3923.7092, 7843.787, 184.1369), 6, 2)
        };

        private static List<Vector3> GunGameMapSpawnPoints = new List<Vector3>()
        {
            // Дамба:

            new Vector3(1666.102, -9.457706, 166.5582),
            new Vector3(1664.673, 1.395329, 166.118),
            new Vector3(1663.016, -27.51617, 173.7747),
            new Vector3(1657.58, -56.45653, 167.1683),
            new Vector3(1662.726, -66.63761, 178.6643),
            new Vector3(1658.279, 32.92284, 179.8762),
            new Vector3(1663.529, 35.82963, 171.6807),
            new Vector3(1667.394, -28.58631, 173.7747),
            new Vector3(1652.829, 29.61711, 172.8806),
            new Vector3(1635.391, -82.08132, 168.4442),

            // Казино
            
            new Vector3(1045.1843, 16.743467, 81.03054),
            new Vector3(1059.6597, 33.53331, 80.91723),
            new Vector3(1083.6838, 53.052834, 80.888176),
            new Vector3(1099.5177, 56.421204, 80.86223),
            new Vector3(1123.075, 65.622734, 80.87781),
            new Vector3(1088.408, 102.36342, 81.879684),
            new Vector3(1088.2871, 92.35007, 82.818954),
            new Vector3(1083.0111, 62.51984, 80.87297),
            new Vector3(1117.6268, 122.14422, 80.72259),
            new Vector3(1147.3411, 122.282036, 81.873634),

            // Vinewood 
            new Vector3(699.77594, 1285.7661, 360.29578),
            new Vector3(727.4884, 1280.4703, 360.27124),
            new Vector3(738.9125, 1294.586, 360.22952),
            new Vector3(749.8661, 1305.2965, 360.26987),
            new Vector3(773.7903, 1301.5635, 360.27832),
            new Vector3(786.9482, 1294.3328, 360.28522),
            new Vector3(785.35297, 1280.0895, 360.27637),
            new Vector3(773.82446, 1271.4601, 359.3613),
            new Vector3(747.9243, 1259.5543, 360.27255),
            new Vector3(748.4385, 1275.8284, 360.2885),        
            // Обсерватория:

            new Vector3(-469.1129, 1111.722, 330.0475),
            new Vector3(-471.2209, 1131.914, 325.8823),
            new Vector3(-429.0442, 1112.393, 327.6823),
            new Vector3(-394.2029, 1084.596, 327.0302),
            new Vector3(-437.0843, 1059.543, 327.6837),
            new Vector3(-461.239, 1081.314, 323.8473),
            new Vector3(-435.7644, 1113.301, 332.5475),
            new Vector3(-394.8021, 1091.174, 330.052),
            new Vector3(-451.079, 1084.894, 332.5208),
            new Vector3(-423.0949, 1075.282, 332.521),
            
            // Лесник
            new Vector3(-685.4304, 5814.89, 17.515251),
            new Vector3(-667.4107, 5837.7354, 17.33113),
            new Vector3(-663.419, 5809.9956, 17.51828),
            new Vector3(-677.76215, 5798.1313, 17.330944),
            new Vector3(-672.45953, 5765.8403, 18.032883),
            new Vector3(-694.5105, 5747.647, 18.024246),
            new Vector3(-711.5002, 5770.552, 17.45391),
            new Vector3(-684.36505, 5787.22, 17.33097),
            new Vector3(-647.3315, 5776.4604, 24.878618),
            new Vector3(-722.98834, 5832.7334, 18.494806),
            
            // Dust
            new Vector3(-3903.8345, 7865.768, 180.93658),
            new Vector3(-3873.253, 7865.6265, 176.13666),
            new Vector3(-3859.5747, 7866.6455, 182.93672),
            new Vector3(-3881.8992, 7930.2573, 183.33232),
            new Vector3(-3901.358, 7922.3687, 177.73665),
            new Vector3(-3898.0996, 7901.8184, 180.93657),
            new Vector3(-3934.7007, 7896.412, 178.13689),
            new Vector3(-3961.3838, 7892.385, 181.73671),
            new Vector3(-3948.8547, 7850.9473, 184.13676),
            new Vector3(-3950.3787, 7909.3394, 180.9366)
        };

        private static List<(Vector3, byte)> MainMapsInfo = new List<(Vector3, byte)>()
        {
            (new Vector3(1665, -12, 174), 100), // Дамба
            (new Vector3(1103.8628, 76.44998, 80.89033), 110), // Казино
            (new Vector3(750.90564, 1285.04, 360.2965), 100), // Vinewood
            (new Vector3(-431.7317, 1101.462, 340.4656), 60), // Обсерватория
            (new Vector3(-281.8681, -731.6278, 123.99747), 25), // Крыша
            (new Vector3(-693.9976, 5809.271, 17.331884), 70), // Лесник
            (new Vector3(-3915.7632, 7876.6606, 182.12125), 85) // Dust
        };

        private static AirsoftWeaponData[] WeaponModels = new AirsoftWeaponData[31]
        {
            new AirsoftWeaponData(ItemId.Revolver, 500),
            new AirsoftWeaponData(ItemId.RevolverMk2, 500),
            new AirsoftWeaponData(ItemId.NavyRevolver, 500),
            new AirsoftWeaponData(ItemId.DoubleAction, 500),
            new AirsoftWeaponData(ItemId.MarksmanPistol, 500),
            new AirsoftWeaponData(ItemId.Pistol50, 500),
            
            new AirsoftWeaponData(ItemId.RayPistol, 500),
            new AirsoftWeaponData(ItemId.Railgun, 500),
            new AirsoftWeaponData(ItemId.RPG, 500),

            new AirsoftWeaponData(ItemId.AssaultSMG, 500),
            new AirsoftWeaponData(ItemId.MicroSMG, 500),
            new AirsoftWeaponData(ItemId.MiniSMG, 500),
            new AirsoftWeaponData(ItemId.SMG, 500),
            new AirsoftWeaponData(ItemId.CombatPDW, 500),

           
            new AirsoftWeaponData(ItemId.HeavyShotgun, 500),
            new AirsoftWeaponData(ItemId.Musket, 500),
            new AirsoftWeaponData(ItemId.AssaultShotgun, 500),
           
            
            new AirsoftWeaponData(ItemId.AssaultRifle, 500),
            new AirsoftWeaponData(ItemId.AssaultRifleMk2, 500),
            new AirsoftWeaponData(ItemId.CarbineRifle, 500),
            new AirsoftWeaponData(ItemId.CarbineRifleMk2, 500),
            new AirsoftWeaponData(ItemId.SpecialCarbine, 500),
            new AirsoftWeaponData(ItemId.CompactRifle, 500),
            
            new AirsoftWeaponData(ItemId.CombatMG, 500),
            new AirsoftWeaponData(ItemId.CombatMGMk2, 500),
            new AirsoftWeaponData(ItemId.MG, 500),
            new AirsoftWeaponData(ItemId.Gusenberg, 500),

            new AirsoftWeaponData(ItemId.MarksmanRifle, 500),
            new AirsoftWeaponData(ItemId.SniperRifle, 500),
            new AirsoftWeaponData(ItemId.HeavySniper, 500),
            new AirsoftWeaponData(ItemId.HeavySniperMk2, 500)
        };

        private static AirsoftWeaponData[] GunGameWeaponModels = new AirsoftWeaponData[17]
        {
            new AirsoftWeaponData(ItemId.Pistol, 500),
            new AirsoftWeaponData(ItemId.CombatPistol, 500),
            new AirsoftWeaponData(ItemId.Pistol50, 500),
            new AirsoftWeaponData(ItemId.MarksmanPistol, 500),
            new AirsoftWeaponData(ItemId.RevolverMk2, 500),
            new AirsoftWeaponData(ItemId.MachinePistol, 500),
            new AirsoftWeaponData(ItemId.MiniSMG, 500),
            new AirsoftWeaponData(ItemId.Gusenberg, 500),
            new AirsoftWeaponData(ItemId.DoubleBarrelShotgun, 500),
            new AirsoftWeaponData(ItemId.HeavyShotgun, 500),
            new AirsoftWeaponData(ItemId.CompactRifle, 500),
            new AirsoftWeaponData(ItemId.AssaultRifle, 500),
            new AirsoftWeaponData(ItemId.SpecialCarbineMk2, 500),
            new AirsoftWeaponData(ItemId.MilitaryRifle, 500),
            new AirsoftWeaponData(ItemId.SniperRifle, 500),
            new AirsoftWeaponData(ItemId.MarksmanRifleMk2, 500),
            new AirsoftWeaponData(ItemId.Musket, 500),
        };
        
        public static void AirsoftLobbiesStartInterval()
        {
            try
            {
                if (LobbyList.Count < 1) return;
                
                foreach (var lobby in LobbyList)
                {
                    if (lobby.Value.GameStatus <= 1 && lobby.Value.GameStartSeconds > 0)
                    {
                        int lobbyMinPlayers = lobby.Value.LobbyMode == 5 ? 2 : lobby.Value.MaxPlayers;
                        if (lobbyMinPlayers == lobby.Value.MaxPlayers || lobby.Value.LobbyPlayers.Count < lobbyMinPlayers) continue;
                        
                        lobby.Value.GameStartSeconds -= 1;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"AirsoftLobbiesStartInterval Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("UpdateServerPlayersInLobbyMenuList")]
        public static void RemoteEvent_UpdateServerPlayersInLobbyMenuList(ExtPlayer player, int state)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (state == 1 && !PlayersInLobbyMenu.Contains(player))
                {
                    PlayersInLobbyMenu.Add(player);
                }
                else if (state == 2 && PlayersInLobbyMenu.Contains(player))
                {
                    PlayersInLobbyMenu.Remove(player);
                }
            }
            catch (Exception e)
            {
                Log.Write($"UpdateServerPlayersInLobbyMenuList Exception: {e.ToString()}");
            }
        }

        public static void LoadAirsoftLobbyList(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                else if (!FunctionsAccess.IsWorking("LoadAirsoftLobbyList"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                /*else if (characterData.LVL < 3)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.lvl3forgame), 3000);
                    return;
                }*/

                if (sessionData.WorkData.OnDuty || sessionData.WorkData.OnWork)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustEndWorkDay), 3000);
                    return;
                }
                else if (sessionData.CuffedData.Cuffed)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsCuffed), 3000);
                    return;
                }
                else if (sessionData.DeathData.InDeath)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsDying), 3000);
                    return;
                }
                else if (Main.IHaveDemorgan(player, true)) return;

                qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage30, 1, isUpdateHud: true);
                Trigger.ClientEvent(player, "airsoft_lobbyMenuHandler", 1, JsonConvert.SerializeObject(LobbyList.Values), "airsoft");
            }
            catch (Exception e)
            {
                Log.Write($"LoadAirsoftLobbyList Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("airsoft_createLobby_server")]
        public static void RemoteEvent_airsoft_createLobby_server(ExtPlayer player, string lobby_name, int lobby_price, string lobby_password, int lobby_mode, int lobby_weapon, int lobby_map)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                else if (!FunctionsAccess.IsWorking("airsoft_createLobby_server"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                /*if (characterData.LVL < 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.lvl2forgame), 3000);
                    return;
                }*/

                if (sessionData.WorkData.OnDuty || sessionData.WorkData.OnWork)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustEndWorkDay), 3000);
                    return;
                }
                else if (sessionData.CuffedData.Cuffed)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsCuffed), 3000);
                    return;
                }
                else if (sessionData.DeathData.InDeath)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsDying), 3000);
                    return;
                }
                else if (Main.IHaveDemorgan(player, true)) return;

                if (lobby_map == 4 && lobby_mode != 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Map1x1only), 3000);
                    return;
                }

                if (LobbyList.ContainsKey(player.Value))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LobbyAlreadyCreated), 3000);
                    return;
                }

                if (sessionData.InAirsoftLobby >= 0)
                {
                    LeaveLobbyFunction(player);
                }

                if (characterData.Money < lobby_price)
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LobbyNeedMoney, lobby_price), 3000);
                    return;
                }

                int MaxLobbyPlayers = 2;

                if (lobby_mode == 2) MaxLobbyPlayers = 4;
                else if (lobby_mode == 3) MaxLobbyPlayers = 6;
                else if (lobby_mode == 4) MaxLobbyPlayers = 10;
                else if (lobby_mode == 5) MaxLobbyPlayers = 100;

                if (lobby_password == null || lobby_password.Length < 1) lobby_password = "0";

                LobbyList.Add(player.Value, new LobbyData(player.Value, lobby_name.Length < 1 ? player.Name.Split("_")[0] : lobby_name, lobby_price, lobby_password, lobby_mode, lobby_weapon, lobby_map, MaxLobbyPlayers));
                LobbyList[player.Value].LobbyPlayers.Add(player);
                UpdateLobbyList();

                if (lobby_mode != 5)
                {
                    LobbyList[player.Value].GameFinalReward = lobby_price * 2;
                }

                sessionData.InAirsoftLobby = player.Value;
                Trigger.ClientEvent(player, "airsoft_updateAirsoftLobbyValue", sessionData.InAirsoftLobby);

                Wallet.Change(player, -lobby_price);
                GameLog.Money($"player({characterData.UUID})", $"server", lobby_price, $"createLobby({MaxLobbyPlayers})");

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LobbyCreated), 3000);

                Trigger.ClientEvent(player, "airsoft_lobbyMenuHandler", 3, JsonConvert.SerializeObject(new int[3] { LobbyList[player.Value].LobbyPlayers.Count, lobby_mode == 5 ? 2 : MaxLobbyPlayers, LobbyList[player.Value].MaxPlayers }), "airsoft", LobbyList[player.Value].GameStartSeconds);
            }
            catch (Exception e)
            {
                Log.Write($"airsoft_createLobby_server Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("airsoft_joinLobby_server")]
        public static void RemoteEvent_airsoft_joinLobby_server(ExtPlayer player, int index, string password)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                /*if (characterData.LVL < 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.lvl2forgame), 3000);
                    return;
                }*/
                if (sessionData.WorkData.OnDuty || sessionData.WorkData.OnWork)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustEndWorkDay), 3000);
                    return;
                }
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

                if (!LobbyList.ContainsKey(index))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LobbyNotFound), 3000);
                    return;
                }

                if (sessionData.InAirsoftLobby >= 0 && sessionData.InAirsoftLobby == index)
                {
                    Trigger.ClientEvent(player, "airsoft_lobbyMenuHandler", 3, JsonConvert.SerializeObject(new int[3] { LobbyList[index].LobbyPlayers.Count, LobbyList[index].LobbyMode == 5 ? 2 : LobbyList[index].MaxPlayers, LobbyList[index].MaxPlayers }), "airsoft", LobbyList[index].GameStartSeconds);
                    return;
                }

                if (sessionData.InAirsoftLobby >= 0)
                {
                    LeaveLobbyFunction(player);
                }

                if (LobbyList[index].GameStatus != 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MatchStarted), 3000);
                    return;
                }

                if (LobbyList[index].LobbyPassword != null && LobbyList[index].LobbyPassword != password)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LobbyIncorrectPassowrd), 3000);
                    return;
                }

                if (LobbyList[index].LobbyPlayers.Count >= LobbyList[index].MaxPlayers)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LobbyFilled), 3000);
                    return;
                }

                if (characterData.Money < LobbyList[index].LobbyPrice)
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ToEnterNeedMoney, LobbyList[index].LobbyPrice), 1500);
                    return;
                }

                LobbyList[index].LobbyPlayers.Add(player);

                sessionData.InAirsoftLobby = index;
                Trigger.ClientEvent(player, "airsoft_updateAirsoftLobbyValue", sessionData.InAirsoftLobby);

                Wallet.Change(player, -LobbyList[index].LobbyPrice);
                GameLog.Money($"player({characterData.UUID})", $"server", LobbyList[index].LobbyPrice, $"joinLobby");

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LobbyConnected), 3000);

                if (LobbyList[index].LobbyMode == 5) // GunGame
                {
                    LobbyList[index].GameFinalReward += LobbyList[index].LobbyPrice;

                    if (LobbyList[index].LobbyPlayers.Count == 2)
                    {
                        if (LobbyList[index].LobbyGameTimer != null)
                        {
                            Timers.Stop(LobbyList[index].LobbyGameTimer);
                        }

                        LobbyList[index].LobbyGameTimer = Timers.StartOnce($"LobbyGameTimer_{index}", 120000, () => LobbyGameTimerFunction(index), true);

                        foreach (ExtPlayer foreachPlayer in LobbyList[index].LobbyPlayers)
                        {
                            if (foreachPlayer.IsCharacterData())
                            {
                                Trigger.ClientEvent(foreachPlayer, "airsoft_lobbyMenuHandler", 5, JsonConvert.SerializeObject(new int[3] { LobbyList[index].LobbyPlayers.Count, LobbyList[index].LobbyMode == 5 ? 2 : LobbyList[index].MaxPlayers, LobbyList[index].MaxPlayers }));
                                Notify.Send(foreachPlayer, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MinLobbyOk), 10000);
                            }
                        }
                    }

                    foreach (ExtPlayer foreachPlayer in LobbyList[index].LobbyPlayers)
                    {
                        if (foreachPlayer.IsCharacterData())
                        {
                            Trigger.ClientEvent(foreachPlayer, "airsoft_lobbyMenuHandler", 5, JsonConvert.SerializeObject(new int[3] { LobbyList[index].LobbyPlayers.Count, LobbyList[index].LobbyMode == 5 ? 2 : LobbyList[index].MaxPlayers, LobbyList[index].MaxPlayers }));
                        }
                    }
                }
                else if (LobbyList.ContainsKey(index) && LobbyList[index].LobbyMode != 5 && LobbyList[index].LobbyPlayers.Count >= LobbyList[index].MaxPlayers) // 1x1, 2x2, 3x3, 5x5
                {
                    LobbyList[index].GameStatus = 1;

                    int NextPlayerTeam = 0;

                    if (LobbyList[index].LobbyGameTimer != null)
                    {
                        Timers.Stop(LobbyList[index].LobbyGameTimer);
                    }

                    int MatchTime = 300000;

                    if (LobbyList[index].LobbyMode >= 2) MatchTime = 600000;

                    LobbyList[index].LobbyGameTimer = Timers.StartOnce($"LobbyGameTimer_{index}", MatchTime, () => LobbyGameTimerFunction(index), true);

                    foreach (ExtPlayer foreachPlayer in LobbyList[index].LobbyPlayers)
                    {
                        if (foreachPlayer.IsCharacterData())
                        {
                            foreachPlayer.SetSharedData("PlayerAirsoftTeam", NextPlayerTeam);

                            Trigger.ClientEvent(foreachPlayer, "airsoft_updateAreaLimit", 1, new Vector3(MainMapsInfo[LobbyList[index].LobbyMap].Item1.X, MainMapsInfo[LobbyList[index].LobbyMap].Item1.Y, MainMapsInfo[LobbyList[index].LobbyMap].Item1.Z), MainMapsInfo[LobbyList[index].LobbyMap].Item2);

                            int random_point = new Random().Next(1, MapSpawnPoints.Count);

                            if (LobbyList[index].LobbyMap == 0)
                            {
                                if (NextPlayerTeam == 0) random_point = new Random().Next(0, 5);
                                else if (NextPlayerTeam == 1) random_point = new Random().Next(5, 10);
                            }
                            else if (LobbyList[index].LobbyMap == 1)
                            {
                                if (NextPlayerTeam == 0) random_point = new Random().Next(10, 15);
                                else if (NextPlayerTeam == 1) random_point = new Random().Next(15, 20);
                            }
                            else if (LobbyList[index].LobbyMap == 2)
                            {
                                if (NextPlayerTeam == 0) random_point = new Random().Next(20, 25);
                                else if (NextPlayerTeam == 1) random_point = new Random().Next(25, 30);
                            }
                            else if (LobbyList[index].LobbyMap == 3)
                            {
                                if (NextPlayerTeam == 0) random_point = new Random().Next(30, 35);
                                else if (NextPlayerTeam == 1) random_point = new Random().Next(35, 40);
                            }
                            else if (LobbyList[index].LobbyMap == 4)
                            {
                                if (NextPlayerTeam == 0) random_point = 40;
                                else if (NextPlayerTeam == 1) random_point = 41;
                            }
                            else if (LobbyList[index].LobbyMap == 5)
                            {
                                if (NextPlayerTeam == 0)
                                {
                                    random_point = new Random().Next(42, 47);
                                    foreachPlayer.setSkin(PedHash.Zombie01);
                                }
                                else if (NextPlayerTeam == 1)
                                {
                                    random_point = new Random().Next(47, 52);
                                    foreachPlayer.setSkin(PedHash.Cop01SMY);
                                }
                            }
                            else if (LobbyList[index].LobbyMap == 6)
                            {
                                if (NextPlayerTeam == 0)
                                {
                                    random_point = new Random().Next(52, 57);
                                    foreachPlayer.setSkin(PedHash.Swat01SMY);
                                }
                                else if (NextPlayerTeam == 1)
                                {
                                    random_point = new Random().Next(57, 62);
                                    foreachPlayer.setSkin(PedHash.Prisoner01SMY);
                                }
                            }

                            NAPI.Entity.SetEntityPosition(foreachPlayer, new Vector3(MapSpawnPoints[random_point].Item1.X, MapSpawnPoints[random_point].Item1.Y, MapSpawnPoints[random_point].Item1.Z));
                            Trigger.Dimension(foreachPlayer, (uint)(index + DefaultDimension));

                            if (!AirsoftPlayerData.ContainsKey(foreachPlayer)) AirsoftPlayerData.Add(foreachPlayer, new PlayerData(foreachPlayer.Health, false));
                            else AirsoftPlayerData[foreachPlayer].CorrectHealth = foreachPlayer.Health;

                            Chars.Repository.ItemsUse(player, "accessories", 7);
                            foreachPlayer.Health = 100;
                            foreachPlayer.Armor = 0;

                            GiveGun(foreachPlayer, WeaponModels[LobbyList[index].LobbyWeapon]);

                            Trigger.ClientEvent(foreachPlayer, "airsoft_lobbyMenuHandler", 2);
                            Trigger.ClientEvent(foreachPlayer, "airsoft_updateStats_client", 1, 0, 0);
                            Trigger.ClientEvent(foreachPlayer, "airsoft_updateStats_client", 3, (MatchTime / 1000));

                            if (LobbyList[index].LobbyMode >= 2) SendTextInfoForPlayer(foreachPlayer, LangFunc.GetText(LangType.Ru, DataName.MatchOk, NextPlayerTeam + 1));
                            else SendTextInfoForPlayer(foreachPlayer, "Матч начался.");

                            SendTextInfoForPlayer(foreachPlayer, LangFunc.GetText(LangType.Ru, DataName.MexitHelp));

                            if (NextPlayerTeam == 0)
                            {
                                NextPlayerTeam = 1;
                            }
                            else
                            {
                                NextPlayerTeam = 0;
                            }
                        }
                    }
                }
                else
                {
                    foreach (ExtPlayer foreachPlayer in LobbyList[index].LobbyPlayers)
                    {
                        if (foreachPlayer.IsCharacterData())
                        {
                            Trigger.ClientEvent(foreachPlayer, "airsoft_lobbyMenuHandler", 5, JsonConvert.SerializeObject(new int[3] { LobbyList[index].LobbyPlayers.Count, LobbyList[index].LobbyMode == 5 ? 2 : LobbyList[index].MaxPlayers, LobbyList[index].MaxPlayers }));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"airsoft_joinLobby_server Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("airsoft_respawnPlayer")]
        public static void RemoteEvent_airsoft_respawnPlayer(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                int lobbyId = sessionData.InAirsoftLobby;
                if (lobbyId >= 0 && LobbyList.ContainsKey(lobbyId) && LobbyList[lobbyId].LobbyPlayers.Contains(player))
                {
                    if (LobbyList[lobbyId].LobbyMode != 5) // 1x1, 2x2, 3x3, 5x5
                    {
                        int random_point = new Random().Next(1, MapSpawnPoints.Count);

                        if (LobbyList[lobbyId].LobbyMap == 0)
                        {
                            if (player.GetSharedData<int>("PlayerAirsoftTeam") == 0) random_point = new Random().Next(0, 5);
                            else if (player.GetSharedData<int>("PlayerAirsoftTeam") == 1) random_point = new Random().Next(5, 10);
                        }
                        else if (LobbyList[lobbyId].LobbyMap == 1)
                        {
                            if (player.GetSharedData<int>("PlayerAirsoftTeam") == 0) random_point = new Random().Next(10, 15);
                            else if (player.GetSharedData<int>("PlayerAirsoftTeam") == 1) random_point = new Random().Next(15, 20);
                        }
                        else if (LobbyList[lobbyId].LobbyMap == 2)
                        {
                            if (player.GetSharedData<int>("PlayerAirsoftTeam") == 0) random_point = new Random().Next(20, 25);
                            else if (player.GetSharedData<int>("PlayerAirsoftTeam") == 1) random_point = new Random().Next(25, 30);
                        }
                        else if (LobbyList[lobbyId].LobbyMap == 3)
                        {
                            if (player.GetSharedData<int>("PlayerAirsoftTeam") == 0) random_point = new Random().Next(30, 35);
                            else if (player.GetSharedData<int>("PlayerAirsoftTeam") == 1) random_point = new Random().Next(35, 40);
                        }
                        else if (LobbyList[lobbyId].LobbyMap == 4)
                        {
                            if (player.GetSharedData<int>("PlayerAirsoftTeam") == 0) random_point = 40;
                            else if (player.GetSharedData<int>("PlayerAirsoftTeam") == 1) random_point = 41;
                        }
                        else if (LobbyList[lobbyId].LobbyMap == 5)
                        {
                            if (player.GetSharedData<int>("PlayerAirsoftTeam") == 0) random_point = new Random().Next(42, 47);
                            else if (player.GetSharedData<int>("PlayerAirsoftTeam") == 1) random_point = new Random().Next(47, 52);
                        }
                        else if (LobbyList[lobbyId].LobbyMap == 6)
                        {
                            if (player.GetSharedData<int>("PlayerAirsoftTeam") == 0) random_point = new Random().Next(52, 57);
                            else if (player.GetSharedData<int>("PlayerAirsoftTeam") == 1) random_point = new Random().Next(57, 62);
                        }

                        NAPI.Entity.SetEntityPosition(player, new Vector3(MapSpawnPoints[random_point].Item1.X, MapSpawnPoints[random_point].Item1.Y, MapSpawnPoints[random_point].Item1.Z));
                    }
                    else // GunGame
                    {
                        int random_point = new Random().Next(1, GunGameMapSpawnPoints.Count);

                        if (LobbyList[lobbyId].LobbyMap == 0) random_point = new Random().Next(0, 10);
                        else if (LobbyList[lobbyId].LobbyMap == 1) random_point = new Random().Next(10, 20);
                        else if (LobbyList[lobbyId].LobbyMap == 2) random_point = new Random().Next(20, 30);
                        else if (LobbyList[lobbyId].LobbyMap == 3) random_point = new Random().Next(30, 40);
                        else if (LobbyList[lobbyId].LobbyMap == 5) random_point = new Random().Next(40, 50);
                        else if (LobbyList[lobbyId].LobbyMap == 6) random_point = new Random().Next(50, 60);

                        NAPI.Entity.SetEntityPosition(player, new Vector3(GunGameMapSpawnPoints[random_point].X, GunGameMapSpawnPoints[random_point].Y, GunGameMapSpawnPoints[random_point].Z));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"airsoft_respawnPlayer Exception: {e.ToString()}");
            }
        }

        public static void DeathCheck(ExtPlayer player, ExtPlayer killer, uint weapon)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                int lobbyId = sessionData.InAirsoftLobby;
                if (LobbyList.ContainsKey(lobbyId))
                {
                    foreach (ExtPlayer foreachPlayer in LobbyList[lobbyId].LobbyPlayers)
                    {
                        var foreachCharacterData = foreachPlayer.GetCharacterData();
                        if (foreachCharacterData != null && foreachCharacterData.AdminLVL < 1)
                        {
                            Admin.SendKillLog(foreachPlayer, killer, player, weapon);
                        }
                    }                            
                    if (LobbyList[lobbyId].LobbyMode == 5 && LobbyList[lobbyId].GameStatus == 1) // GunGame
                    {
                        if (killer.IsCharacterData() && AirsoftPlayerData.ContainsKey(killer) && AirsoftPlayerData[killer].IsGunGamePlayer == true)
                        {
                            killer.SetSharedData("killsWeapon", Convert.ToInt32(killer.GetSharedData<int>("killsWeapon")) + 1);

                            if ((Convert.ToInt32(killer.GetSharedData<int>("weaponLevel")) + 1) < GunGameWeaponModels.Length)
                            {
                                killer.SetSharedData("weaponLevel", Convert.ToInt32(killer.GetSharedData<int>("weaponLevel")) + 1);

                                GiveGun(killer, GunGameWeaponModels[Convert.ToInt32(killer.GetSharedData<int>("weaponLevel"))]);

                                if (LobbyList[lobbyId].GunGameTop.ContainsKey(killer.Name))
                                {
                                    LobbyList[lobbyId].GunGameTop[killer.Name] += 1;
                                }
                                else
                                {
                                    LobbyList[lobbyId].GunGameTop.Add(killer.Name, 1);
                                }

                                foreach (ExtPlayer foreachPlayer in LobbyList[lobbyId].LobbyPlayers)
                                {
                                    if (foreachPlayer.IsCharacterData())
                                    {
                                        Trigger.ClientEvent(foreachPlayer, "airsoft_updateStats_client", 2);
                                    }
                                }
                            }
                            else
                            {
                                string GunGameWinner = killer.Name;

                                if (LobbyList[lobbyId].LobbyGameTimer != null)
                                {
                                    Timers.Stop(LobbyList[lobbyId].LobbyGameTimer);
                                }

                                foreach (ExtPlayer foreachPlayer in LobbyList[lobbyId].LobbyPlayers)
                                {
                                    var foreachSessionData = foreachPlayer.GetSessionData();
                                    if (foreachSessionData == null) continue;

                                    var foreachCharacterData = foreachPlayer.GetCharacterData();
                                    if (foreachCharacterData != null)
                                    {
                                        foreachPlayer.SetSharedData("killsWeapon", 0);
                                        foreachPlayer.SetSharedData("weaponLevel", 0);

                                        foreachSessionData.InAirsoftLobby = -1;
                                        foreachPlayer.SetSharedData("PlayerAirsoftTeam", -1);

                                        Trigger.ClientEvent(foreachPlayer, "airsoft_updateAirsoftLobbyValue", foreachSessionData.InAirsoftLobby);

                                        if (player != foreachPlayer)
                                            NAPI.Entity.SetEntityPosition(foreachPlayer, new Vector3(-478.86032, -395.27307, 34.027653));
                                        else
                                            NAPI.Player.SpawnPlayer(player, new Vector3(-478.86032, -395.27307, 34.027653));
                                        
                                        Trigger.Dimension(foreachPlayer);

                                        BattlePass.Repository.UpdateReward(foreachPlayer, 0);
                                        
                                        if (AirsoftPlayerData.ContainsKey(foreachPlayer))
                                        {
                                            foreachPlayer.Health = Convert.ToInt32(AirsoftPlayerData[foreachPlayer].CorrectHealth);
                                            AirsoftPlayerData[foreachPlayer].IsGunGamePlayer = false;
                                        }
                                        else foreachPlayer.Health = 100;

                                        ClearGun(foreachPlayer);
                                        foreachPlayer.SetDefaultSkin();
                                        
                                        if (qMain.UpdateQuestsStage(foreachPlayer, Zdobich.QuestName, (int)zdobich_quests.Stage30, 1, 2, isUpdateHud: true))
                                            qMain.UpdateQuestsComplete(foreachPlayer, Zdobich.QuestName, (int) zdobich_quests.Stage30, true);

                                        Trigger.ClientEvent(foreachPlayer, "airsoft_updateAreaLimit", 0);
                                        Trigger.ClientEvent(foreachPlayer, "airsoft_updateStats_client", 0);

                                        if (foreachPlayer.Name == GunGameWinner)
                                        {
                                            int amount = Convert.ToInt32(LobbyList[lobbyId].GameFinalReward * 0.9);
                                            Wallet.Change(foreachPlayer, amount);
                                            GameLog.Money($"server", $"player({foreachCharacterData.UUID})", amount, $"lobbyWin(GG)");
                                            SendTextInfoForPlayer(foreachPlayer, $"Вы выиграли этот матч и получили {amount}$");
                                        }
                                        else
                                        {
                                            if (GunGameWinner == null)
                                            {
                                                Wallet.Change(foreachPlayer, LobbyList[lobbyId].LobbyPrice);
                                                GameLog.Money($"server", $"player({foreachCharacterData.UUID})", LobbyList[lobbyId].LobbyPrice, $"lobbyDraw(GG)");
                                                SendTextInfoForPlayer(foreachPlayer, LangFunc.GetText(LangType.Ru, DataName.MatchNoWinner, LobbyList[lobbyId].LobbyPrice));
                                            }
                                            else
                                            {
                                                SendTextInfoForPlayer(foreachPlayer, LangFunc.GetText(LangType.Ru, DataName.GungameWinner, GunGameWinner));
                                            }

                                        }
                                    }
                                }

                                LobbyList.Remove(lobbyId);
                                UpdateLobbyList();
                            }
                        }

                        if (LobbyList.ContainsKey(lobbyId))
                        {
                            int random_point = new Random().Next(1, GunGameMapSpawnPoints.Count);

                            if (LobbyList[lobbyId].LobbyMap == 0) random_point = new Random().Next(0, 10);
                            else if (LobbyList[lobbyId].LobbyMap == 1) random_point = new Random().Next(10, 20);
                            else if (LobbyList[lobbyId].LobbyMap == 2) random_point = new Random().Next(20, 30);
                            else if (LobbyList[lobbyId].LobbyMap == 3) random_point = new Random().Next(30, 40);
                            else if (LobbyList[lobbyId].LobbyMap == 5) random_point = new Random().Next(40, 50);
                            else if (LobbyList[lobbyId].LobbyMap == 6) random_point = new Random().Next(50, 60);

                            NAPI.Player.SpawnPlayer(player, new Vector3(GunGameMapSpawnPoints[random_point].X, GunGameMapSpawnPoints[random_point].Y, GunGameMapSpawnPoints[random_point].Z));
                            Trigger.Dimension(player, (uint)(lobbyId + DefaultDimension));

                            player.Health = 100;
                            player.Armor = 0;

                            GiveGun(player, GunGameWeaponModels[Convert.ToInt32(player.GetSharedData<int>("weaponLevel"))]);
                        }
                    }
                    else if (LobbyList[lobbyId].LobbyMode != 5 && LobbyList[lobbyId].GameStatus == 1) // 1x1, 2x2, 3x3, 5x5
                    {
                        if (LobbyList[lobbyId].LobbyMode == 0)
                        {
                            Ems.ReviveFunc(player, true);
                            if (killer != null && killer.IsCharacterData())
                            {
                                var killerSessionData = killer.GetSessionData();
                                if (killerSessionData != null)
                                {
                                    if (killerSessionData.InAirsoftLobby == lobbyId && Convert.ToInt32(killer.GetSharedData<int>("PlayerAirsoftTeam")) != Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")))
                                    {
                                        if (!LobbyList.ContainsKey(lobbyId)) return;

                                        if (LobbyList[lobbyId].LobbyGameTimer != null)
                                        {
                                            Timers.Stop(LobbyList[lobbyId].LobbyGameTimer);
                                        }

                                        int winner_team = -1; // -1 если ничья
                                        if (Convert.ToInt32(killer.GetSharedData<int>("PlayerAirsoftTeam")) == 0 || Convert.ToInt32(killer.GetSharedData<int>("PlayerAirsoftTeam")) == 1) winner_team = Convert.ToInt32(killer.GetSharedData<int>("PlayerAirsoftTeam"));

                                        foreach (ExtPlayer foreachPlayer in LobbyList[lobbyId].LobbyPlayers)
                                        {
                                            var foreachSessionData = foreachPlayer.GetSessionData();
                                            if (foreachSessionData == null) continue;

                                            var foreachCharacterData = foreachPlayer.GetCharacterData();
                                            if (foreachCharacterData != null)
                                            {
                                                if (winner_team == Convert.ToInt32(foreachPlayer.GetSharedData<int>("PlayerAirsoftTeam")))
                                                {
                                                    Wallet.Change(foreachPlayer, Convert.ToInt32(LobbyList[lobbyId].GameFinalReward * 0.9));
                                                    GameLog.Money($"server", $"player({foreachCharacterData.UUID})", Convert.ToInt32(LobbyList[lobbyId].GameFinalReward * 0.9), $"lobbyWin(DUEL_PVP)");
                                                    SendTextInfoForPlayer(foreachPlayer, LangFunc.GetText(LangType.Ru, DataName.MatchWinner, Convert.ToInt32(LobbyList[lobbyId].GameFinalReward * 0.9)));
                                                }
                                                else if (winner_team == -1)
                                                {
                                                    Wallet.Change(foreachPlayer, LobbyList[lobbyId].LobbyPrice);
                                                    GameLog.Money($"server", $"player({foreachCharacterData.UUID})", LobbyList[lobbyId].LobbyPrice, $"lobbyDraw(DUEL_PVP)");
                                                    SendTextInfoForPlayer(foreachPlayer, LangFunc.GetText(LangType.Ru, DataName.MatchNichya, LobbyList[lobbyId].LobbyPrice));
                                                }
                                                else
                                                {
                                                    SendTextInfoForPlayer(foreachPlayer, LangFunc.GetText(LangType.Ru, DataName.YouLose));
                                                }

                                                foreachSessionData.InAirsoftLobby = -1;
                                                foreachPlayer.SetSharedData("PlayerAirsoftTeam", -1);

                                                Trigger.ClientEvent(foreachPlayer, "airsoft_updateAirsoftLobbyValue", foreachSessionData.InAirsoftLobby);

                                                if (AirsoftPlayerData.ContainsKey(foreachPlayer)) foreachPlayer.Health = Convert.ToInt32(AirsoftPlayerData[foreachPlayer].CorrectHealth);
                                                else foreachPlayer.Health = 100;

                                                NAPI.Player.SpawnPlayer(foreachPlayer, new Vector3(-478.86032, -395.27307, 34.027653));
                                                Trigger.Dimension(foreachPlayer);

                                                ClearGun(foreachPlayer);
                                                foreachPlayer.SetDefaultSkin();
                                                
                                                if (qMain.UpdateQuestsStage(foreachPlayer, Zdobich.QuestName, (int)zdobich_quests.Stage30, 1, 2, isUpdateHud: true))
                                                    qMain.UpdateQuestsComplete(foreachPlayer, Zdobich.QuestName, (int) zdobich_quests.Stage30, true);

                                                Trigger.ClientEvent(foreachPlayer, "airsoft_updateAreaLimit", 0);
                                                Trigger.ClientEvent(foreachPlayer, "airsoft_updateStats_client", 0);
                                            }
                                        }

                                        LobbyList.Remove(lobbyId);
                                        UpdateLobbyList();
                                    }
                                }
                            }
                            else
                            {
                                int random_point = new Random().Next(1, MapSpawnPoints.Count);

                                if (LobbyList[lobbyId].LobbyMap == 0)
                                {
                                    if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 0) random_point = new Random().Next(0, 5);
                                    else if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 1) random_point = new Random().Next(5, 10);
                                }
                                else if (LobbyList[lobbyId].LobbyMap == 1)
                                {
                                    if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 0) random_point = new Random().Next(10, 15);
                                    else if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 1) random_point = new Random().Next(15, 20);
                                }
                                else if (LobbyList[lobbyId].LobbyMap == 2)
                                {
                                    if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 0) random_point = new Random().Next(20, 25);
                                    else if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 1) random_point = new Random().Next(25, 30);
                                }
                                else if (LobbyList[lobbyId].LobbyMap == 3)
                                {
                                    if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 0) random_point = new Random().Next(30, 35);
                                    else if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 1) random_point = new Random().Next(35, 40);
                                }
                                else if (LobbyList[lobbyId].LobbyMap == 4)
                                {
                                    if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 0) random_point = 40;
                                    else if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 1) random_point = 41;
                                }
                                else if (LobbyList[lobbyId].LobbyMap == 5)
                                {
                                    if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 0) random_point = new Random().Next(42, 47);
                                    else if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 1) random_point = new Random().Next(47, 52);
                                }
                                else if (LobbyList[lobbyId].LobbyMap == 6)
                                {
                                    if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 0) random_point = new Random().Next(52, 57);
                                    else if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 1) random_point = new Random().Next(57, 62);

                                }

                                NAPI.Player.SpawnPlayer(player, new Vector3(MapSpawnPoints[random_point].Item1.X, MapSpawnPoints[random_point].Item1.Y, MapSpawnPoints[random_point].Item1.Z));
                                Trigger.Dimension(player, (uint)(lobbyId + DefaultDimension));

                                player.Health = 100;
                                player.Armor = 0;

                                GiveGun(player, WeaponModels[LobbyList[lobbyId].LobbyWeapon]);
                            }
                        }
                        else
                        {
                            if (killer != null && killer.IsCharacterData())
                            {
                                var killerSessionData = killer.GetSessionData();
                                if (killerSessionData != null)
                                {
                                    if (killerSessionData.InAirsoftLobby == lobbyId && Convert.ToInt32(killer.GetSharedData<int>("PlayerAirsoftTeam")) != Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")))
                                    {
                                        LobbyList[killerSessionData.InAirsoftLobby].GameScore[Convert.ToInt32(killer.GetSharedData<int>("PlayerAirsoftTeam"))] += 1;

                                        foreach (ExtPlayer foreachPlayer in LobbyList[killerSessionData.InAirsoftLobby].LobbyPlayers)
                                        {
                                            if (foreachPlayer.IsCharacterData())
                                            {
                                                Trigger.ClientEvent(foreachPlayer, "airsoft_updateStats_client", 1, LobbyList[killerSessionData.InAirsoftLobby].GameScore[0], LobbyList[killerSessionData.InAirsoftLobby].GameScore[1]);
                                            }
                                        }
                                    }
                                }
                            }

                            int random_point = new Random().Next(1, MapSpawnPoints.Count);

                            if (LobbyList[lobbyId].LobbyMap == 0)
                            {
                                if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 0) random_point = new Random().Next(0, 5);
                                else if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 1) random_point = new Random().Next(5, 10);
                            }
                            else if (LobbyList[lobbyId].LobbyMap == 1)
                            {
                                if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 0) random_point = new Random().Next(10, 15);
                                else if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 1) random_point = new Random().Next(15, 20);
                            }
                            else if (LobbyList[lobbyId].LobbyMap == 2)
                            {
                                if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 0) random_point = new Random().Next(20, 25);
                                else if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 1) random_point = new Random().Next(25, 30);
                            }
                            else if (LobbyList[lobbyId].LobbyMap == 3)
                            {
                                if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 0) random_point = new Random().Next(30, 35);
                                else if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 1) random_point = new Random().Next(35, 40);
                            }
                            else if (LobbyList[lobbyId].LobbyMap == 4)
                            {
                                if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 0) random_point = 40;
                                else if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 1) random_point = 41;
                            }
                            else if (LobbyList[lobbyId].LobbyMap == 5)
                            {
                                if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 0) random_point = new Random().Next(42, 47);
                                else if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 1) random_point = new Random().Next(47, 52);
                            }
                            else if (LobbyList[lobbyId].LobbyMap == 6)
                            {
                                if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 0) random_point = new Random().Next(52, 57);
                                else if (Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) == 1) random_point = new Random().Next(57, 62);
                            }

                            NAPI.Player.SpawnPlayer(player, new Vector3(MapSpawnPoints[random_point].Item1.X, MapSpawnPoints[random_point].Item1.Y, MapSpawnPoints[random_point].Item1.Z));
                            Trigger.Dimension(player, (uint)(lobbyId + DefaultDimension));

                            player.Health = 100;
                            player.Armor = 0;

                            GiveGun(player, WeaponModels[LobbyList[lobbyId].LobbyWeapon]);
                        }
                    }
                    else LeaveLobbyFunction(player);
                }
                else LeaveLobbyFunction(player);
            }
            catch (Exception e)
            {
                Log.Write($"DeathCheck Exception: {e.ToString()}");
            }
        }

        public static void OnPlayerDisconnected(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                if (!player.IsCharacterData() || sessionData.InAirsoftLobby == -1) return;

                int lobbyId = sessionData.InAirsoftLobby;
                if (lobbyId >= 0 && LobbyList.ContainsKey(lobbyId))
                {
                    if (AirsoftPlayerData.ContainsKey(player) && AirsoftPlayerData[player].IsGunGamePlayer == true)
                    {
                        if (LobbyList[lobbyId].GunGameTop.ContainsKey(player.Name))
                        {
                            LobbyList[lobbyId].GunGameTop.Remove(player.Name);
                        }

                        foreach (ExtPlayer foreachPlayer in LobbyList[lobbyId].LobbyPlayers)
                        {
                            if (foreachPlayer.IsCharacterData())
                            {
                                Trigger.ClientEvent(foreachPlayer, "airsoft_updateStats_client", 2);
                            }
                        }
                    }

                    LeaveLobbyFunction(player);
                }

                if (LobbyList.ContainsKey(lobbyId) && LobbyList[lobbyId].LobbyPlayers.Contains(player))
                {
                    LobbyList[lobbyId].LobbyPlayers.Remove(player);

                    if (LobbyList[lobbyId].LobbyPlayers.Count == 0)
                    {
                        if (LobbyList[lobbyId].LobbyGameTimer != null)
                        {
                            Timers.Stop(LobbyList[lobbyId].LobbyGameTimer);
                        }

                        LobbyList.Remove(lobbyId);
                        UpdateLobbyList();
                    }
                }

                if (PlayersInLobbyMenu.Contains(player))
                {
                    PlayersInLobbyMenu.Remove(player);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Event_OnPlayerDisconnected (Airsoft) Exception: {e.ToString()}");
            }
        }

        private static void LobbyGameTimerFunction(int index)
        {
            try
            {
                if (!LobbyList.ContainsKey(index)) return;

                if (LobbyList[index].LobbyGameTimer != null) Timers.Stop(LobbyList[index].LobbyGameTimer);

                if (LobbyList[index].LobbyMode == 5) // GunGame
                {
                    if (LobbyList[index].GameStatus == 0)
                    {
                        if (LobbyList[index].LobbyPlayers.Count < 2)
                        {
                            foreach (ExtPlayer foreachPlayer in LobbyList[index].LobbyPlayers)
                            {
                                try
                                {
                                    if (foreachPlayer.IsCharacterData())
                                    {
                                        var foreachSessionData = foreachPlayer.GetSessionData();
                                        if (foreachSessionData == null) continue;

                                        foreachPlayer.SetSharedData("killsWeapon", 0);
                                        foreachPlayer.SetSharedData("weaponLevel", 0);

                                        foreachSessionData.InAirsoftLobby = -1;
                                        foreachPlayer.SetSharedData("PlayerAirsoftTeam", -1);

                                        Trigger.ClientEvent(foreachPlayer, "airsoft_updateAirsoftLobbyValue", foreachSessionData.InAirsoftLobby);

                                        if (AirsoftPlayerData.ContainsKey(foreachPlayer)) AirsoftPlayerData.Remove(foreachPlayer);

                                        ClearGun(foreachPlayer);
                                        foreachPlayer.SetDefaultSkin();

                                        Trigger.ClientEvent(foreachPlayer, "airsoft_lobbyMenuHandler", 2);
                                        Trigger.ClientEvent(foreachPlayer, "airsoft_updateStats_client", 0);
                                        Notify.Send(foreachPlayer, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoPlayersMatch), 3000);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Log.Write($"LobbyGameTimerFunction Foreach #1 Exception: {e.ToString()}");
                                }
                            }

                            LobbyList.Remove(index);
                            UpdateLobbyList();
                        }
                        else
                        {
                            LobbyList[index].GameStatus = 1;
                            LobbyList[index].LobbyGameTimer = Timers.StartOnce($"LobbyGameTimer_{index}", 900000, () => LobbyGameTimerFunction(index), true);

                            foreach (ExtPlayer foreachPlayer in LobbyList[index].LobbyPlayers)
                            {
                                try
                                {
                                    if (foreachPlayer.IsCharacterData())
                                    {
                                        foreachPlayer.SetSharedData("killsWeapon", 0);
                                        foreachPlayer.SetSharedData("weaponLevel", 0);

                                        Trigger.ClientEvent(foreachPlayer, "airsoft_updateAreaLimit", 1, new Vector3(MainMapsInfo[LobbyList[index].LobbyMap].Item1.X, MainMapsInfo[LobbyList[index].LobbyMap].Item1.Y, MainMapsInfo[LobbyList[index].LobbyMap].Item1.Z), MainMapsInfo[LobbyList[index].LobbyMap].Item2);

                                        int random_point = new Random().Next(1, GunGameMapSpawnPoints.Count);

                                        if (LobbyList[index].LobbyMap == 0) random_point = new Random().Next(0, 10);
                                        else if (LobbyList[index].LobbyMap == 1) random_point = new Random().Next(10, 20);
                                        else if (LobbyList[index].LobbyMap == 2) random_point = new Random().Next(20, 30);
                                        else if (LobbyList[index].LobbyMap == 3) random_point = new Random().Next(30, 40);
                                        else if (LobbyList[index].LobbyMap == 5) random_point = new Random().Next(40, 50);
                                        else if (LobbyList[index].LobbyMap == 6) random_point = new Random().Next(50, 60);

                                        NAPI.Entity.SetEntityPosition(foreachPlayer, new Vector3(GunGameMapSpawnPoints[random_point].X, GunGameMapSpawnPoints[random_point].Y, GunGameMapSpawnPoints[random_point].Z));
                                        Trigger.Dimension(foreachPlayer, (uint) (index + DefaultDimension));

                                        if (!AirsoftPlayerData.ContainsKey(foreachPlayer)) AirsoftPlayerData.Add(foreachPlayer, new PlayerData(foreachPlayer.Health, true));
                                        else
                                        {
                                            AirsoftPlayerData[foreachPlayer].CorrectHealth = foreachPlayer.Health;
                                            AirsoftPlayerData[foreachPlayer].IsGunGamePlayer = true;
                                        }

                                        Chars.Repository.ItemsUse(foreachPlayer, "accessories", 7);

                                        foreachPlayer.Health = 100;
                                        foreachPlayer.Armor = 0;
                                        GiveGun(foreachPlayer, GunGameWeaponModels[foreachPlayer.GetSharedData<int>("weaponLevel")]);

                                        Trigger.ClientEvent(foreachPlayer, "airsoft_lobbyMenuHandler", 2);
                                        Trigger.ClientEvent(foreachPlayer, "airsoft_updateStats_client", 2);
                                        Trigger.ClientEvent(foreachPlayer, "airsoft_updateStats_client", 3, 900);

                                        SendTextInfoForPlayer(foreachPlayer, LangFunc.GetText(LangType.Ru, DataName.MexitHelp));
                                    }
                                }
                                catch (Exception e)
                                {
                                    Log.Write($"LobbyGameTimerFunction Foreach #2 Exception: {e.ToString()}");
                                }
                            }
                        }
                    }
                    else
                    {
                        string GunGameWinner = LobbyList[index].GunGameTop.FirstOrDefault(x => x.Value == LobbyList[index].GunGameTop.Values.Max()).Key;

                        foreach (ExtPlayer foreachPlayer in LobbyList[index].LobbyPlayers)
                        {
                            try
                            {
                                var foreachCharacterData = foreachPlayer.GetCharacterData();
                                if (foreachCharacterData != null)
                                {
                                    var foreachSessionData = foreachPlayer.GetSessionData();
                                    if (foreachSessionData == null) continue;

                                    foreachPlayer.SetSharedData("killsWeapon", 0);
                                    foreachPlayer.SetSharedData("weaponLevel", 0);

                                    foreachSessionData.InAirsoftLobby = -1;
                                    foreachPlayer.SetSharedData("PlayerAirsoftTeam", -1);

                                    Trigger.ClientEvent(foreachPlayer, "airsoft_updateAirsoftLobbyValue", foreachSessionData.InAirsoftLobby);

                                    NAPI.Entity.SetEntityPosition(foreachPlayer, new Vector3(-478.86032, -395.27307, 34.027653));
                                    Trigger.Dimension(foreachPlayer);

                                    if (AirsoftPlayerData.ContainsKey(foreachPlayer))
                                    {
                                        foreachPlayer.Health = Convert.ToInt32(AirsoftPlayerData[foreachPlayer].CorrectHealth);
                                        AirsoftPlayerData.Remove(foreachPlayer);
                                    }
                                    else foreachPlayer.Health = 100;

                                    ClearGun(foreachPlayer);
                                    foreachPlayer.SetDefaultSkin();

                                    if (qMain.UpdateQuestsStage(foreachPlayer, Zdobich.QuestName, (int)zdobich_quests.Stage30, 1, 2, isUpdateHud: true))
                                        qMain.UpdateQuestsComplete(foreachPlayer, Zdobich.QuestName, (int) zdobich_quests.Stage30, true);
                                    
                                    Trigger.ClientEvent(foreachPlayer, "airsoft_updateAreaLimit", 0);
                                    Trigger.ClientEvent(foreachPlayer, "airsoft_updateStats_client", 0);

                                    if (foreachPlayer.Name == GunGameWinner)
                                    {
                                        int amount = Convert.ToInt32(LobbyList[index].GameFinalReward * 0.9);
                                        Wallet.Change(foreachPlayer, amount);
                                        GameLog.Money($"server", $"player({foreachCharacterData.UUID})", amount, $"lobbyWin(GG)");
                                        SendTextInfoForPlayer(foreachPlayer, LangFunc.GetText(LangType.Ru, DataName.MatchWinner, amount));
                                    }
                                    else
                                    {
                                        if (GunGameWinner == null)
                                        {
                                            Wallet.Change(foreachPlayer, LobbyList[index].LobbyPrice);
                                            GameLog.Money($"server", $"player({foreachCharacterData.UUID})", LobbyList[index].LobbyPrice, $"lobbyDraw(GG)");
                                            SendTextInfoForPlayer(foreachPlayer, LangFunc.GetText(LangType.Ru, DataName.MatchNoWinner, LobbyList[index].LobbyPrice));
                                        }
                                        else
                                        {
                                            SendTextInfoForPlayer(foreachPlayer, LangFunc.GetText(LangType.Ru, DataName.GungameWinner, GunGameWinner));
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Write($"LobbyGameTimerFunction Foreach #3 Exception: {e.ToString()}");
                            }
                        }

                        LobbyList.Remove(index);
                        UpdateLobbyList();
                    }
                }
                else // 1x1, 2x2, 3x3, 5x5
                {
                    if (LobbyList[index].GameStatus != 0)
                    {
                        int winner_team = -1; // -1 если ничья

                        if (LobbyList[index].GameScore[0] > LobbyList[index].GameScore[1])
                        {
                            winner_team = 0;
                        }
                        else if (LobbyList[index].GameScore[0] < LobbyList[index].GameScore[1])
                        {
                            winner_team = 1;
                        }

                        foreach (ExtPlayer foreachPlayer in LobbyList[index].LobbyPlayers)
                        {
                            try
                            {
                                var foreachCharacterData = foreachPlayer.GetCharacterData();
                                if (foreachCharacterData != null)
                                {
                                    var foreachSessionData = foreachPlayer.GetSessionData();
                                    if (foreachSessionData == null) continue;

                                    if (winner_team == foreachPlayer.GetSharedData<int>("PlayerAirsoftTeam"))
                                    {
                                        Wallet.Change(foreachPlayer, Convert.ToInt32(LobbyList[index].GameFinalReward * 0.9));
                                        GameLog.Money($"server", $"player({foreachCharacterData.UUID})", Convert.ToInt32(LobbyList[index].GameFinalReward * 0.9), $"lobbyWin(TPVP)");

                                        if (LobbyList[index].LobbyMode == 1) SendTextInfoForPlayer(foreachPlayer, LangFunc.GetText(LangType.Ru, DataName.MatchWinner, Convert.ToInt32(LobbyList[index].GameFinalReward * 0.9)));
                                        else SendTextInfoForPlayer(foreachPlayer, LangFunc.GetText(LangType.Ru, DataName.YouTeamWin, Convert.ToInt32(LobbyList[index].GameFinalReward * 0.9)));
                                    }
                                    else if (winner_team == -1)
                                    {
                                        Wallet.Change(foreachPlayer, LobbyList[index].LobbyPrice);
                                        GameLog.Money($"server", $"player({foreachCharacterData.UUID})", LobbyList[index].LobbyPrice, $"lobbyDraw(TPVP)");
                                        SendTextInfoForPlayer(foreachPlayer, LangFunc.GetText(LangType.Ru, DataName.MatchNichya, LobbyList[index].LobbyPrice));
                                    }
                                    else
                                    {
                                        if (LobbyList[index].LobbyMode == 1) SendTextInfoForPlayer(foreachPlayer, LangFunc.GetText(LangType.Ru, DataName.YouLose));
                                        else SendTextInfoForPlayer(foreachPlayer, LangFunc.GetText(LangType.Ru, DataName.YouTeamLose));
                                    }

                                    foreachSessionData.InAirsoftLobby = -1;
                                    foreachPlayer.SetSharedData("PlayerAirsoftTeam", -1);

                                    Trigger.ClientEvent(foreachPlayer, "airsoft_updateAirsoftLobbyValue", foreachSessionData.InAirsoftLobby);

                                    NAPI.Entity.SetEntityPosition(foreachPlayer, new Vector3(-478.86032, -395.27307, 34.027653));
                                    Trigger.Dimension(foreachPlayer);

                                    if (AirsoftPlayerData.ContainsKey(foreachPlayer)) foreachPlayer.Health = Convert.ToInt32(AirsoftPlayerData[foreachPlayer].CorrectHealth);
                                    else foreachPlayer.Health = 100;

                                    ClearGun(foreachPlayer);
                                    foreachPlayer.SetDefaultSkin();
                                    
                                    if (qMain.UpdateQuestsStage(foreachPlayer, Zdobich.QuestName, (int)zdobich_quests.Stage30, 1, 2, isUpdateHud: true))
                                        qMain.UpdateQuestsComplete(foreachPlayer, Zdobich.QuestName, (int) zdobich_quests.Stage30, true);

                                    Trigger.ClientEvent(foreachPlayer, "airsoft_updateAreaLimit", 0);
                                    Trigger.ClientEvent(foreachPlayer, "airsoft_updateStats_client", 0);
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Write($"LobbyGameTimerFunction Foreach #4 Exception: {e.ToString()}");
                            }
                        }

                        LobbyList.Remove(index);
                        UpdateLobbyList();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"LobbyGameTimerFunction Exception: {e.ToString()}");
            }
        }

        [Command("mexit")]
        public static void CMD_mexit(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                if (sessionData.InAirsoftLobby >= 0)
                {
                    if (player.GetSharedData<int>("PlayerAirsoftTeam") == 0 || player.GetSharedData<int>("PlayerAirsoftTeam") == 1 || AirsoftPlayerData.ContainsKey(player) && AirsoftPlayerData[player].IsGunGamePlayer == true)
                    {
                        Trigger.ClientEvent(player, "airsoft_updateAreaLimit", 0);
                        Trigger.ClientEvent(player, "airsoft_updateStats_client", 0);

                        NAPI.Entity.SetEntityPosition(player, new Vector3(-478.86032, -395.27307, 34.027653));
                        Trigger.Dimension(player);

                        if (AirsoftPlayerData.ContainsKey(player)) player.Health = Convert.ToInt32(AirsoftPlayerData[player].CorrectHealth);
                        else player.Health = 100;

                        ClearGun(player);
                        player.SetDefaultSkin();
                    }

                    int lobbyId = sessionData.InAirsoftLobby;
                    if (lobbyId >= 0 && LobbyList.ContainsKey(lobbyId))
                    {
                        if (AirsoftPlayerData.ContainsKey(player) && AirsoftPlayerData[player].IsGunGamePlayer == true)
                        {
                            if (LobbyList[lobbyId].GunGameTop.ContainsKey(player.Name))
                            {
                                LobbyList[lobbyId].GunGameTop.Remove(player.Name);
                            }

                            foreach (ExtPlayer foreachPlayer in LobbyList[lobbyId].LobbyPlayers)
                            {
                                if (foreachPlayer.IsCharacterData())
                                {
                                    Trigger.ClientEvent(foreachPlayer, "airsoft_updateStats_client", 2);
                                }
                            }
                        }
                    }

                    LeaveLobbyFunction(player);
                }
                else
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.ErrorMatch), 3000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_mexit Exception: {e.ToString()}");
            }
        }

        public static void OutLobbyZone(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                int lobbyId = sessionData.InAirsoftLobby;
                if (lobbyId >= 0 && LobbyList.ContainsKey(lobbyId) && Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) != 0 && Convert.ToInt32(player.GetSharedData<int>("PlayerAirsoftTeam")) != 1)
                {
                    if (!AirsoftPlayerData.ContainsKey(player) || AirsoftPlayerData.ContainsKey(player) && AirsoftPlayerData[player].IsGunGamePlayer != true) LeaveLobbyFunction(player);
                }
            }
            catch (Exception e)
            {
                Log.Write($"OutLobbyZone Exception: {e.ToString()}");
            }
        }

        public static void LeaveLobbyFunction(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                int lobbyId = sessionData.InAirsoftLobby;
                if (lobbyId >= 0 && LobbyList.ContainsKey(lobbyId))
                {
                    if (LobbyList[lobbyId].LobbyPlayers.Contains(player))
                    {
                        if (LobbyList[lobbyId].GameStatus == 0 || LobbyList[lobbyId].LobbyMode == 5)
                        {
                            Wallet.Change(player, LobbyList[lobbyId].LobbyPrice);
                            GameLog.Money($"server", $"player({characterData.UUID})", LobbyList[lobbyId].LobbyPrice, $"lobbyDraw(TPVP)");
                        }

                        if (LobbyList[lobbyId].LobbyMode == 5) // GunGame
                        {
                            LobbyList[lobbyId].GameFinalReward -= LobbyList[lobbyId].LobbyPrice;
                        }

                        LobbyList[lobbyId].LobbyPlayers.Remove(player);

                        if (LobbyList[lobbyId].LobbyPlayers.Count == 0)
                        {
                            if (LobbyList[lobbyId].LobbyGameTimer != null)
                            {
                                Timers.Stop(LobbyList[lobbyId].LobbyGameTimer);
                            }

                            LobbyList.Remove(lobbyId);
                            UpdateLobbyList();
                        }
                        else
                        {
                            if (LobbyList[lobbyId].GameStatus == 0)
                            {
                                foreach (ExtPlayer foreachPlayer in LobbyList[lobbyId].LobbyPlayers)
                                {
                                    if (foreachPlayer.IsCharacterData())
                                    {
                                        Trigger.ClientEvent(foreachPlayer, "airsoft_lobbyMenuHandler", 5, JsonConvert.SerializeObject(new int[3] { LobbyList[lobbyId].LobbyPlayers.Count, LobbyList[lobbyId].LobbyMode == 5 ? 2 : LobbyList[lobbyId].MaxPlayers, LobbyList[lobbyId].MaxPlayers }));
                                    }
                                }
                            }
                            else if (LobbyList[lobbyId].LobbyMode != 5 && LobbyList[lobbyId].GameStatus != 0) // 1x1, 2x2, 3x3, 5x5
                            {
                                int mates_count = 0;

                                foreach (ExtPlayer foreachPlayer in LobbyList[lobbyId].LobbyPlayers)
                                {
                                    if (foreachPlayer.IsCharacterData())
                                    {
                                        if (foreachPlayer.GetSharedData<int>("PlayerAirsoftTeam") == player.GetSharedData<int>("PlayerAirsoftTeam") && foreachPlayer != player)
                                        {
                                            mates_count += 1;
                                        }
                                    }
                                }

                                if (mates_count == 0 || LobbyList[lobbyId].LobbyMode == 0 || LobbyList[lobbyId].LobbyMode == 1)
                                {
                                    if (LobbyList[lobbyId].LobbyGameTimer != null)
                                    {
                                        Timers.Stop(LobbyList[lobbyId].LobbyGameTimer);
                                    }

                                    int winner_team = -1;

                                    if (player.GetSharedData<int>("PlayerAirsoftTeam") == 0) winner_team = 1;
                                    else if (player.GetSharedData<int>("PlayerAirsoftTeam") == 1) winner_team = 0;

                                    foreach (ExtPlayer foreachPlayer in LobbyList[lobbyId].LobbyPlayers)
                                    {
                                        var foreachCharacterData = foreachPlayer.GetCharacterData();
                                        if (foreachCharacterData != null && foreachPlayer != player)
                                        {
                                            var foreachSessionData = foreachPlayer.GetSessionData();
                                            if (foreachSessionData == null) continue;

                                            if (winner_team == foreachPlayer.GetSharedData<int>("PlayerAirsoftTeam"))
                                            {
                                                Wallet.Change(foreachPlayer, Convert.ToInt32(LobbyList[lobbyId].GameFinalReward * 0.9));
                                                GameLog.Money($"server", $"player({foreachCharacterData.UUID})", Convert.ToInt32(LobbyList[lobbyId].GameFinalReward * 0.9), $"lobbyWin(TPVP)");

                                                if (LobbyList[lobbyId].LobbyMode == 0 || LobbyList[lobbyId].LobbyMode == 1) SendTextInfoForPlayer(foreachPlayer, LangFunc.GetText(LangType.Ru, DataName.MatchWinner, Convert.ToInt32(LobbyList[lobbyId].GameFinalReward * 0.9)));
                                                else SendTextInfoForPlayer(foreachPlayer, LangFunc.GetText(LangType.Ru, DataName.YouTeamWin, Convert.ToInt32(LobbyList[lobbyId].GameFinalReward * 0.9)));
                                            }

                                            foreachSessionData.InAirsoftLobby = -1;
                                            foreachPlayer.SetSharedData("PlayerAirsoftTeam", -1);

                                            Trigger.ClientEvent(foreachPlayer, "airsoft_updateAirsoftLobbyValue", foreachSessionData.InAirsoftLobby);

                                            NAPI.Entity.SetEntityPosition(foreachPlayer, new Vector3(-478.86032, -395.27307, 34.027653));
                                            Trigger.Dimension(foreachPlayer);

                                            if (AirsoftPlayerData.ContainsKey(foreachPlayer)) foreachPlayer.Health = Convert.ToInt32(AirsoftPlayerData[foreachPlayer].CorrectHealth);
                                            else foreachPlayer.Health = 100;

                                            ClearGun(foreachPlayer);
                                            foreachPlayer.SetDefaultSkin();

                                            Trigger.ClientEvent(foreachPlayer, "airsoft_updateAreaLimit", 0);
                                            Trigger.ClientEvent(foreachPlayer, "airsoft_updateStats_client", 0);
                                        }
                                    }

                                    LobbyList.Remove(lobbyId);
                                    UpdateLobbyList();
                                }
                            }
                        }
                    }

                    if (player.GetSharedData<int>("PlayerAirsoftTeam") == 0 || player.GetSharedData<int>("PlayerAirsoftTeam") == 1 || AirsoftPlayerData.ContainsKey(player) && AirsoftPlayerData[player].IsGunGamePlayer == true) Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LeftMatch), 3000);
                    else Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LeftLobby), 3000);

                    sessionData.InAirsoftLobby = -1;
                    player.SetSharedData("killsWeapon", 0);
                    player.SetSharedData("weaponLevel", 0);
                    player.SetSharedData("PlayerAirsoftTeam", -1);

                    if (AirsoftPlayerData.ContainsKey(player)) AirsoftPlayerData.Remove(player);

                    Trigger.ClientEvent(player, "airsoft_updateAreaLimit", 0);
                    Trigger.ClientEvent(player, "airsoft_updateStats_client", 0);
                    Trigger.ClientEvent(player, "airsoft_updateAirsoftLobbyValue", sessionData.InAirsoftLobby);
                }
            }
            catch (Exception e)
            {
                Log.Write($"LeaveLobbyFunction Exception: {e.ToString()}");
            }
        }

        public static void UpdateLobbyList()
        {
            try
            {
                lock(PlayersInLobbyMenu)
                {
                    foreach (ExtPlayer foreachPlayer in PlayersInLobbyMenu)
                    {
                        if (foreachPlayer.IsCharacterData())
                        {
                            Trigger.ClientEvent(foreachPlayer, "airsoft_updateLobbyList_client", JsonConvert.SerializeObject(LobbyList), "airsoft");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"UpdateLobbyList Exception: {e.ToString()}");
            }
        }

        public static void GiveGun(ExtPlayer player, AirsoftWeaponData weaponData)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Trigger.ClientEvent(player, "client.weapon.give", (int) WeaponRepository.GetHash(weaponData.weapon.ToString()), weaponData.ammo);
            }
            catch (Exception e)
            {
                Log.Write($"GiveGun (Airsoft) Exception: {e.ToString()}");
            }
        }

        public static void ClearGun(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Trigger.ClientEvent(player, "client.weapon.take");
            }
            catch (Exception e)
            {
                Log.Write($"ClearGun (Airsoft) Exception: {e.ToString()}");
            }
        }

        public static void SendTextInfoForPlayer(ExtPlayer player, string msg)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Trigger.SendChatMessage(player, msg);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, msg, 5000);
            }
            catch (Exception e)
            {
                Log.Write($"SendTextInfoForPlayer Exception: {e.ToString()}");
            }
        }
    }
}