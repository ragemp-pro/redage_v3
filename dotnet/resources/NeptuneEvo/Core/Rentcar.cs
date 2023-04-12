using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Functions;
using NeptuneEvo.GUI;
using Newtonsoft.Json;
using Redage.SDK;
using NeptuneEvo.Jobs.Models;
using System.Threading;
using Localization;
using NeptuneEvo.Players.Phone.Messages.Models;
using NeptuneEvo.Players.Popup.List.Models;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;

namespace NeptuneEvo.Core
{
    public class RentCarSpawn
    {
        public int ZoneId { set; get; }
        public Vector3 Position { set; get; }
        public Vector3 Rotation { set; get; }

        public RentCarSpawn(int zoneId, Vector3 position, Vector3 rotation)
        {
            ZoneId = zoneId;
            Position = position;
            Rotation = rotation;
        }
    }
    public enum RentCarId
    {
        None = 0,
        Civilian,
        Cycling,
        OffRoad,
        WaterBased,
        Helicopter,
        Aeroplane,
        Holiday,
        Elite,
        Rally,



        JobLawnmower,
        JobTaxi,
        JobBus,
        JobTrucker,
        JobCollector,
        JobMechanic,
        
        JobPostman
    }
    public enum RentZoneId
    {
        First = 0,
    }
    public class RentZoneData
    {
        public int ZoneId { set; get; }
        public string Skin { set; get; }
        public string Title { set; get; }
        public Vector3 Position { set; get; }
        public float Heading { set; get; }
        public RentCarId Index { set; get; }

        public RentZoneData(int ZoneId, string Skin, string Title, Vector3 Position, float Heading, RentCarId Index)
        {
            this.ZoneId = ZoneId;
            this.Skin = Skin;
            this.Title = Title;
            this.Position = Position;
            this.Heading = Heading;
            this.Index = Index;
        }
    }
    public class RentCarData
    {
        public int Id { set; get; }
        public string Model { set; get; }
        public int Price { set; get; }
        [JsonIgnore]
        public List<RentCarId> CarsId { set; get; }
        [JsonIgnore]
        public JobsId Job { set; get; }

        public RentCarData(int Id, string Model, int Price, List<RentCarId> CarsId, JobsId Job = JobsId.None)
        {
            this.Id = Id;
            this.Model = Model;
            this.Price = Price;
            this.CarsId = CarsId;
            this.Job = Job;
        }
    }
    class Rentcar : Script
    {
        private static readonly nLog Log = new nLog("Core.Rentcar");
   

        [ServerEvent(Event.PlayerEnterVehicle)]
        public void Event_OnPlayerEnterVehicle(ExtPlayer player, ExtVehicle vehicle, sbyte seatid)
        {
            try
            {

                var sessionData = player.GetSessionData();
                if (sessionData == null)
                    return;

                var accountData = player.GetAccountData();
                if (accountData == null)
                    return;



                var characterData = player.GetCharacterData();

                if (characterData == null) 
                    return;

                var vehicleLocalData = vehicle.GetVehicleLocalData();   
                if (seatid == 0 && vehicleLocalData != null && vehicleLocalData.Access == VehicleAccess.Rent)
                {                 
                    if (sessionData.RentData != null && sessionData.RentData.Vehicle == vehicle) return;
                    if (vehicleLocalData.WorkDriver == characterData.UUID) return;
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyRented), 3000);
                    VehicleManager.WarpPlayerOutOfVehicle(player);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Event_OnPlayerEnterVehicle Exception: {e.ToString()}");
            }
        }

        public static ExtPlayer Event_vehicleDeath(ExtVehicle vehicle,  string prefix = "уничтожен") 
        {
            try
            {
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null && vehicleLocalData.Access == VehicleAccess.Rent)
                {
                    var player = Main.GetPlayerByUUID(vehicleLocalData.WorkDriver);

                    var sessionData = player.GetSessionData();
                    if (sessionData == null) return null;

                    var characterData = player.GetCharacterData();
                    if (characterData == null) return null;

                    if (sessionData.RentData != null)
                    {
                        sessionData.RentData = null;
                        if (prefix != String.Empty) 
                            Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RentCarWas, prefix), 3000);

                        return player;
                    } 
                        
                }
            }
            catch (Exception e)
            {
                Log.Write($"Event_vehicleDeath Exception: {e.ToString()}");
            }
            return null;
        }
        private static Color[] RentColorData = new Color[9]
        {
            new Color(0, 0, 0),
            new Color(225, 225, 225),
            new Color(230, 0, 0),
            new Color(255, 115, 0),
            new Color(240, 240, 0),
            new Color(0, 230, 0),
            new Color(0, 205, 255),
            new Color(0, 0, 230),
            new Color(190, 60, 165),
        };

        public static RentCarData[] RentCarsData =
        {
            new RentCarData(0, "Faggio2", 15, new List<RentCarId>() { RentCarId.None }), 
            new RentCarData(1, "Sanchez", 25, new List<RentCarId>() { RentCarId.None }), 
            new RentCarData(2, "Retinue", 65, new List<RentCarId>() { RentCarId.None }), 
            new RentCarData(3, "Washington", 70, new List<RentCarId>() { RentCarId.None }), 
            new RentCarData(4, "Primo", 75, new List<RentCarId>() { RentCarId.None }), 
            new RentCarData(5, "Surge", 85, new List<RentCarId>() { RentCarId.None }), 
            new RentCarData(6, "Asea", 95, new List<RentCarId>() { RentCarId.None }), 
            new RentCarData(7, "Sugoi", 200, new List<RentCarId>() { RentCarId.None }), 
 
            // 
            new RentCarData(8, "Faggio", 180, new List<RentCarId>() { RentCarId.Civilian }), 
            new RentCarData(9, "Sanchez", 400, new List<RentCarId>() { RentCarId.Civilian }),// 
            new RentCarData(10, "Hexer", 510, new List<RentCarId>() { RentCarId.Civilian }),
            new RentCarData(11, "bati2", 550, new List<RentCarId>() { RentCarId.Civilian }), 
            new RentCarData(12, "daemon2", 575, new List<RentCarId>() { RentCarId.Civilian }), 
            new RentCarData(13, "bagger", 600, new List<RentCarId>() { RentCarId.Civilian }), 
            new RentCarData(14, "cliffhanger", 625, new List<RentCarId>() { RentCarId.Civilian }), 
            new RentCarData(15, "fcr", 630, new List<RentCarId>() { RentCarId.Civilian }), 
            new RentCarData(16, "enduro", 690, new List<RentCarId>() { RentCarId.Civilian }), 
            new RentCarData(17, "chimera", 700, new List<RentCarId>() { RentCarId.Civilian }), 
            new RentCarData(18, "stryder", 750, new List<RentCarId>() { RentCarId.Civilian }), 
            new RentCarData(19, "shotaro", 2050, new List<RentCarId>() { RentCarId.Civilian }),
            
            // 
            new RentCarData(20, "Cruiser", 8, new List<RentCarId>() { RentCarId.Cycling }), 
            new RentCarData(21, "Fixter", 15, new List<RentCarId>() { RentCarId.Cycling }), 
            new RentCarData(22, "Scorcher", 25, new List<RentCarId>() { RentCarId.Cycling }), 
            new RentCarData(23, "Tribike", 30, new List<RentCarId>() { RentCarId.Cycling }), 
            new RentCarData(24, "Tribike3", 35, new List<RentCarId>() { RentCarId.Cycling }), 
            new RentCarData(25, "Bmx", 40, new List<RentCarId>() { RentCarId.Cycling }), 
            // 
            new RentCarData(26, "Scorcher", 25, new List<RentCarId>() { RentCarId.OffRoad }),// 
            new RentCarData(27, "Manchez", 50, new List<RentCarId>() { RentCarId.OffRoad }), 
            new RentCarData(28, "Bf400", 75, new List<RentCarId>() { RentCarId.OffRoad }), 
            new RentCarData(29, "Fcr", 85, new List<RentCarId>() { RentCarId.OffRoad }), 
            new RentCarData(30, "Blazer", 90, new List<RentCarId>() { RentCarId.OffRoad }), 
            new RentCarData(31, "Verus", 100, new List<RentCarId>() { RentCarId.OffRoad }), 
            new RentCarData(32, "Patriot", 125, new List<RentCarId>() { RentCarId.OffRoad }), 
            new RentCarData(33, "Everon", 175, new List<RentCarId>() { RentCarId.OffRoad }), 
            new RentCarData(34, "Dubsta3", 250, new List<RentCarId>() { RentCarId.OffRoad }), 
            // 
            new RentCarData(35, "Suntrap", 150, new List<RentCarId>() { RentCarId.WaterBased }), 
            new RentCarData(36, "Seashark", 250, new List<RentCarId>() { RentCarId.WaterBased }), 
            new RentCarData(37, "Tropic", 300, new List<RentCarId>() { RentCarId.WaterBased }), 
            new RentCarData(38, "Squalo", 350, new List<RentCarId>() { RentCarId.WaterBased }), 
            new RentCarData(39, "Jetmax", 375, new List<RentCarId>() { RentCarId.WaterBased }), 
            new RentCarData(40, "Dinghy", 400, new List<RentCarId>() { RentCarId.WaterBased }), 
            new RentCarData(41, "Speeder", 500, new List<RentCarId>() { RentCarId.WaterBased }), 
            new RentCarData(42, "Toro", 550, new List<RentCarId>() { RentCarId.WaterBased }), 
            new RentCarData(43, "Longfin", 600, new List<RentCarId>() { RentCarId.WaterBased }), 
            new RentCarData(44, "Marquis", 800, new List<RentCarId>() { RentCarId.WaterBased }), 
            new RentCarData(45, "Submersible2", 1000, new List<RentCarId>() { RentCarId.WaterBased }), 
            new RentCarData(46, "Avisa", 1250, new List<RentCarId>() { RentCarId.WaterBased }), 
            // 
            new RentCarData(47, "Havok", 350, new List<RentCarId>() { RentCarId.Helicopter }), 
            new RentCarData(48, "Seasparrow", 400, new List<RentCarId>() { RentCarId.Helicopter }), 
            new RentCarData(49, "Seasparrow2", 400, new List<RentCarId>() { RentCarId.Helicopter }), 
            new RentCarData(50, "Buzzard2", 500, new List<RentCarId>() { RentCarId.Helicopter }), 
            new RentCarData(51, "Maverick", 550, new List<RentCarId>() { RentCarId.Helicopter }), 
            new RentCarData(52, "Frogger", 600, new List<RentCarId>() { RentCarId.Helicopter }), 
            new RentCarData(53, "Swift", 800, new List<RentCarId>() { RentCarId.Helicopter }), 
            new RentCarData(54, "Supervolito2", 1000, new List<RentCarId>() { RentCarId.Helicopter }), 
            new RentCarData(55, "Volatus", 1250, new List<RentCarId>() { RentCarId.Helicopter }), 
            // 
            new RentCarData(56, "Microlight", 450, new List<RentCarId>() { RentCarId.Aeroplane }), 
            new RentCarData(57, "Duster", 500, new List<RentCarId>() { RentCarId.Aeroplane }), 
            new RentCarData(58, "Dodo", 550, new List<RentCarId>() { RentCarId.Aeroplane }), 
            new RentCarData(59, "Mammatus", 600, new List<RentCarId>() { RentCarId.Aeroplane }), 
            new RentCarData(60, "Velum", 625, new List<RentCarId>() { RentCarId.Aeroplane }), 
            new RentCarData(61, "Cuban800", 650, new List<RentCarId>() { RentCarId.Aeroplane }), 
            new RentCarData(62, "Seabreeze", 700, new List<RentCarId>() { RentCarId.Aeroplane }), 
            new RentCarData(63, "Vestra", 725, new List<RentCarId>() { RentCarId.Aeroplane }), 
            new RentCarData(64, "Shamal", 900, new List<RentCarId>() { RentCarId.Aeroplane }), 
            new RentCarData(65, "Nimbus", 950, new List<RentCarId>() { RentCarId.Aeroplane }), 
            new RentCarData(66, "Miljet", 1000, new List<RentCarId>() { RentCarId.Aeroplane }), 
            new RentCarData(67, "Luxor", 1250, new List<RentCarId>() { RentCarId.Aeroplane }), 
            // 
            new RentCarData(68, "Superd", 250, new List<RentCarId>() { RentCarId.Holiday }), 
            new RentCarData(69, "Windsor2", 275, new List<RentCarId>() { RentCarId.Holiday }), 
            new RentCarData(70, "Cognoscenti", 300, new List<RentCarId>() { RentCarId.Holiday }), 
            new RentCarData(71, "Stretch", 350, new List<RentCarId>() { RentCarId.Holiday }), 
            new RentCarData(72, "Patriot2", 400, new List<RentCarId>() { RentCarId.Holiday }), 
            new RentCarData(73, "Pbus2", 450, new List<RentCarId>() { RentCarId.Holiday }), 
            new RentCarData(74, "Romero", 750, new List<RentCarId>() { RentCarId.Holiday }), 
            new RentCarData(75, "Hustler", 1000, new List<RentCarId>() { RentCarId.Holiday }), 
            new RentCarData(76, "Stafford", 1250, new List<RentCarId>() { RentCarId.Holiday }), 
            // 
            new RentCarData(77, "Weevil", 250, new List<RentCarId>() { RentCarId.Elite }), 
            new RentCarData(78, "Coquette4", 300, new List<RentCarId>() { RentCarId.Elite }), 
            new RentCarData(79, "Paragon", 350, new List<RentCarId>() { RentCarId.Elite }), 
            new RentCarData(80, "Jb7002", 375, new List<RentCarId>() { RentCarId.Elite }), 
            new RentCarData(81, "Btype3", 500, new List<RentCarId>() { RentCarId.Elite }), 
            new RentCarData(82, "Zentorno", 600, new List<RentCarId>() { RentCarId.Elite }), 
            new RentCarData(83, "Nero2", 750, new List<RentCarId>() { RentCarId.Elite }), 
            new RentCarData(84, "Tyrant", 800, new List<RentCarId>() { RentCarId.Elite }), 
            new RentCarData(85, "Italirsx", 900, new List<RentCarId>() { RentCarId.Elite }), 
            // 
            new RentCarData(86, "Veto", 150, new List<RentCarId>() { RentCarId.Rally }), 
            new RentCarData(87, "Bifta", 175, new List<RentCarId>() { RentCarId.Rally }), 
            new RentCarData(88, "Sultan2", 225, new List<RentCarId>() { RentCarId.Rally }), 
            new RentCarData(89, "Omnis", 250, new List<RentCarId>() { RentCarId.Rally }), 
            new RentCarData(90, "Hotring", 300, new List<RentCarId>() { RentCarId.Rally }), 
            new RentCarData(91, "Comet4", 350, new List<RentCarId>() { RentCarId.Rally }), 
 
            // 
            new RentCarData(92, "taxi", 25, new List<RentCarId>() { RentCarId.JobTaxi }, JobsId.Taxi), 
            new RentCarData(93, "cognoscenti", 100, new List<RentCarId>() { RentCarId.JobTaxi }, JobsId.Taxi), 
            new RentCarData(94, "bmwm5", 800, new List<RentCarId>() { RentCarId.JobTaxi }, JobsId.Taxi), 
 
            // 
            new RentCarData(95, "mower", 0, new List<RentCarId>() { RentCarId.JobLawnmower }, JobsId.Lawnmower), 
 
            // 
            new RentCarData(96, "airbus", 10, new List<RentCarId>() { RentCarId.JobBus }, JobsId.Bus), 
            new RentCarData(97, "bus", 15, new List<RentCarId>() { RentCarId.JobBus }, JobsId.Bus), 
            new RentCarData(98, "coach", 20, new List<RentCarId>() { RentCarId.JobBus }, JobsId.Bus), 
 
            // 
            new RentCarData(99, "pounder", 0, new List<RentCarId>() { RentCarId.JobTrucker }, JobsId.Trucker), 
            new RentCarData(100, "benson", 0, new List<RentCarId>() { RentCarId.JobTrucker }, JobsId.Trucker), 
            new RentCarData(101, "rallytruck", 0, new List<RentCarId>() { RentCarId.JobTrucker }, JobsId.Trucker), 
 
            // 
            new RentCarData(102, "stockade", 10, new List<RentCarId>() { RentCarId.JobCollector }, JobsId.CashCollector), 
            new RentCarData(103, "terbyte", 15, new List<RentCarId>() { RentCarId.JobCollector }, JobsId.CashCollector), 
            new RentCarData(104, "rallytruck", 20, new List<RentCarId>() { RentCarId.JobCollector }, JobsId.CashCollector), 
 
            // 
            new RentCarData(105, "caracara2", 10, new List<RentCarId>() { RentCarId.JobMechanic }, JobsId.CarMechanic), 
             
            // 
            new RentCarData(106, "vapidse", 10, new List<RentCarId>() { RentCarId.JobPostman }, JobsId.Postman), 
        };

        private static RentCarSpawn[] RentCarsSpawn = new RentCarSpawn[]
       {
            new RentCarSpawn(1, new Vector3(-174.5694, -181.5158, 43.18488), new Vector3(-0.05705871f, -0.1541872f, 340.2171f)),
            new RentCarSpawn(1, new Vector3(-177.9232, -180.3944, 43.1904), new Vector3(-0.06142293f, -0.1456173f, 340.7091f)),
            new RentCarSpawn(1, new Vector3(-181.2611, -179.2929, 43.1919), new Vector3(0.1182507f, -0.05557432f, 340.5893f)),
            new RentCarSpawn(1, new Vector3(-184.4611, -178.2069, 43.19278), new Vector3(0.08228108f, -0.0325385f, 340.8163f)),
            new RentCarSpawn(1, new Vector3(-187.6113, -177.1798, 43.19285), new Vector3(0.1143083f, -0.04350877f, 341.3497f)),
            new RentCarSpawn(1, new Vector3(-190.9109, -176.1367, 43.19333), new Vector3(0.1034455f, -0.1197823f, 341.5069f)),
            new RentCarSpawn(1, new Vector3(-171.7888, -167.1982, 43.18843), new Vector3(0.02188119f, -0.04304371f, 340.8701f)),
            new RentCarSpawn(1, new Vector3(-185.1973, -162.5179, 43.18881), new Vector3(0.004956556f, 0.004005647f, 340.1981f)),

            new RentCarSpawn(2, new Vector3(-318.1875, -880.2917, 30.6464), new Vector3(-0.0157926f, 0.06523791f, 167.5566f)),
            new RentCarSpawn(2, new Vector3(-302.3612, -899.517, 30.64738), new Vector3(-0.007157338f, -0.1631727f, 347.8979f)),
            new RentCarSpawn(2, new Vector3(-305.8806, -898.8257, 30.64716), new Vector3(0.04223601f, 0.009893501f, 347.7854f)),
            new RentCarSpawn(2, new Vector3(-298.6348, -900.32, 30.64729), new Vector3(0.01027106f, -0.03159515f, 348.1921f)),
            new RentCarSpawn(2, new Vector3(-309.5218, -898.0364, 30.6459), new Vector3(-0.01406786f, 0.04576958f, 347.7824f)),
            new RentCarSpawn(2, new Vector3(-310.9652, -881.8445, 30.64712), new Vector3(-0.02186359f, -0.007394671f, 168.2584f)),
            new RentCarSpawn(2, new Vector3(-327.623, -894.1865, 30.63894), new Vector3(0.04555815f, 0.001252634f, 347.9704f)),
            new RentCarSpawn(2, new Vector3(-336.3365, -876.4655, 30.63833), new Vector3(-0.07224572f, 0.05279379f, 168.0992f)),
            new RentCarSpawn(2, new Vector3(-308.0478, -908.5814, 30.64372), new Vector3(-0.002515211f, 0.007317593f, 167.9839f)),
            new RentCarSpawn(2, new Vector3(-311.6236, -907.824, 30.64331), new Vector3(0.01597843f, -0.00578986f, 168.8448f)),
            new RentCarSpawn(2, new Vector3(-315.1973, -907.0726, 30.64338), new Vector3(0.01928982f, 0.05517132f, 168.0546f)),
            new RentCarSpawn(2, new Vector3(-326.0757, -904.7607, 30.64476), new Vector3(0.05742029f, -0.01690229f, 168.2157f)),
            new RentCarSpawn(2, new Vector3(-340.6255, -901.6813, 30.64166), new Vector3(0.008807567f, 0.09599815f, 168.0399f)),
            new RentCarSpawn(2, new Vector3(-326.1788, -952.576, 30.64692), new Vector3(-0.03110901f, -0.002700633f, 249.9506f)),
            new RentCarSpawn(2, new Vector3(-322.3165, -942.0166, 30.64689), new Vector3(-0.02742851f, -0.007622322f, 250.173f)),
            new RentCarSpawn(2, new Vector3(-318.5148, -931.6251, 30.64681), new Vector3(-0.04162052f, -0.02506509f, 250.2673f)),
            new RentCarSpawn(2, new Vector3(-329.161, -931.6671, 30.64772), new Vector3(0.05298815f, 0.005889944f, 69.63324f)),
            new RentCarSpawn(2, new Vector3(-333.0364, -942.1367, 30.64668), new Vector3(0.004267561f, 0.0138743f, 69.8418f)),
            new RentCarSpawn(2, new Vector3(-336.8081, -952.5752, 30.647), new Vector3(0.04734382f, 0.001573325f, 70.02142f)),
            new RentCarSpawn(2, new Vector3(-346.6637, -973.3312, 30.64714), new Vector3(-0.02138184f, -0.1341068f, 340.0938f)),

            new RentCarSpawn(3, new Vector3(-50.89787, -2104.142, 16.2709), new Vector3(-0.01726568f, -0.003165648f, 199.6255f)),
            new RentCarSpawn(3, new Vector3(-45.99706, -2096.494, 16.27115), new Vector3(0.051618f, -0.05478308f, 19.47269f)),
            new RentCarSpawn(3, new Vector3(-57.41492, -2106.457, 16.27176), new Vector3(-0.05241027f, 0.06955543f, 199.8726f)),
            new RentCarSpawn(3, new Vector3(-55.92258, -2099.988, 16.27176), new Vector3(0.07695632f, -0.006316445f, 19.3454f)),
            new RentCarSpawn(3, new Vector3(-59.25963, -2101.211, 16.27102), new Vector3(0.01589091f, 0.005760496f, 19.73538f)),
            new RentCarSpawn(3, new Vector3(-62.62508, -2102.37, 16.27156), new Vector3(0.06013114f, -0.09574953f, 19.39182f)),
            new RentCarSpawn(3, new Vector3(-69.12244, -2104.7, 16.27135), new Vector3(0.06034426f, -0.01935717f, 19.37039f)),
            new RentCarSpawn(3, new Vector3(-67.21611, -2109.933, 16.27152), new Vector3(-0.0583914f, -0.01969231f, 199.7879f)),


            new RentCarSpawn(4, new Vector3(-1870.1871, -353.87296, 48.820972), new Vector3(0.03433925, -0.3792338, 51.227726)),
            new RentCarSpawn(4, new Vector3(-1865.5391, -357.62314, 48.811714), new Vector3(0.10642323, -0.36502266, 51.194057)),
            new RentCarSpawn(4, new Vector3(-1859.9497, -362.15063, 48.81001), new Vector3(-0.10591185, -0.7230112, 51.150387)),
            new RentCarSpawn(4, new Vector3(-1854.031, -367.87408, 48.78867), new Vector3(0.2960477, -0.89521986, 44.22902)),
            /*new RentCarSpawn(4, new Vector3(-1891.336, -341.0096, 48.57138), new Vector3(0.04534596f, 0.01980596f, 321.3176f)),
            new RentCarSpawn(4, new Vector3(-1894.487, -338.5184, 48.56805), new Vector3(0.04317809f, -0.00539135f, 321.1061f)),
            new RentCarSpawn(4, new Vector3(-1897.633, -335.9834, 48.56575), new Vector3(0.05004351f, 0.01204853f, 321.8055f)),
            new RentCarSpawn(4, new Vector3(-1900.667, -333.5624, 48.56359), new Vector3(-0.01844393f, -0.05736014f, 321.5709f)),
            new RentCarSpawn(4, new Vector3(-1885.192, -324.7642, 48.573), new Vector3(-0.7650245f, -0.2590694f, 139.7191f)),
            new RentCarSpawn(4, new Vector3(-1888.392, -328.7817, 48.56057), new Vector3(-0.05793509f, 0.003580404f, 141.2988f)),
            new RentCarSpawn(4, new Vector3(-1896.81, -317.7345, 48.56475), new Vector3(0.03059709f, 0.04136239f, 232.9163f)),
            new RentCarSpawn(4, new Vector3(-1894.914, -315.1238, 48.56491), new Vector3(-0.04727536f, 0.04495433f, 232.9227f)),*/

            new RentCarSpawn(5, new Vector3(-897.9762, -2338.008, 6.285205), new Vector3(0.4563178, -0.01475238, 59.53125)),
            new RentCarSpawn(5, new Vector3(-896.0508, -2335.092, 6.284833), new Vector3(0.3837851, -0.07570215, 59.5777)),
            new RentCarSpawn(5, new Vector3(-894.3827, -2332.052, 6.284529), new Vector3(0.0461585, -0.08129423, 58.94998)),
            new RentCarSpawn(5, new Vector3(-892.9431, -2328.928, 6.285657), new Vector3(0.4100015, -0.006029424, 59.83563)),
            new RentCarSpawn(5, new Vector3(-890.9929, -2326.119, 6.284601), new Vector3(0.5029823, -0.008816821, 59.24673)),
            new RentCarSpawn(5, new Vector3(-889.317, -2323.177, 6.285734), new Vector3(-0.4272468, -0.0414784, 239.4405)),
            new RentCarSpawn(5, new Vector3(-902.3571, -2327.442, 6.285857), new Vector3(0.4593074, -0.0718815, 59.3819)),
            new RentCarSpawn(5, new Vector3(-900.6923, -2324.279, 6.285465), new Vector3(-0.4276452, 0.008349739, 239.0667)),
            new RentCarSpawn(5, new Vector3(-899.0521, -2321.255, 6.284715), new Vector3(-0.4067092, 0.08733013, 239.4432)),
            new RentCarSpawn(5, new Vector3(-897.3215, -2318.225, 6.28545), new Vector3(-0.4077923, 0.0009872941, 239.7602)),

            new RentCarSpawn(6, new Vector3(430.771, -1163.992, 28.86712), new Vector3(-0.3971694, -0.1512112, 268.8827)),
            new RentCarSpawn(6, new Vector3(430.7942, -1160.919, 28.86791), new Vector3(-0.3884627, -0.1953789, 269.2471)),
            new RentCarSpawn(6, new Vector3(430.8293, -1157.821, 28.8676), new Vector3(-0.3838955, -0.1580842, 268.8311)),
            new RentCarSpawn(6, new Vector3(431.3412, -1154.808, 28.86795), new Vector3(-0.4068561, -0.185202, 269.0997)),
            new RentCarSpawn(6, new Vector3(450.2335, -1154.799, 28.86864), new Vector3(-0.3733782, -0.2515301, 270.0211)),
            new RentCarSpawn(6, new Vector3(450.963, -1157.918, 28.86827), new Vector3(-0.3490867, -0.2214924, 269.2381)),
            new RentCarSpawn(6, new Vector3(466.7895, -1160.866, 28.867727), new Vector3(0.4831736, 0.1689919, 88.4624)),
            new RentCarSpawn(6, new Vector3(466.7206, -1157.711, 28.8672), new Vector3(0.3911593, 0.1837483, 88.86874)),
            new RentCarSpawn(6, new Vector3(466.8256, -1154.688, 28.86746), new Vector3(0.3727432, 0.1352535, 89.6333)),
            new RentCarSpawn(6, new Vector3(466.7534, -1151.801, 28.86844), new Vector3(0.3878667, 0.2117222, 88.62674)),

            new RentCarSpawn(7, new Vector3(451.8294, 241.589, 102.7857), new Vector3(-0.4219078, -0.06872021, 250.1397)),
            new RentCarSpawn(7, new Vector3(452.911, 245.4417, 102.7855), new Vector3(-0.4284297, -0.05800131, 249.7882)),
            new RentCarSpawn(7, new Vector3(454.3848, 249.2663, 102.7852), new Vector3(-0.4381595, -0.06327292, 249.4981)),
            new RentCarSpawn(7, new Vector3(455.9629, 253.0731, 102.7846), new Vector3(-0.3863182, -0.01275678, 248.7229)),
            new RentCarSpawn(7, new Vector3(457.2606, 257.1321, 102.7847), new Vector3(-0.3928577, 0.009186273, 248.616)),
            new RentCarSpawn(7, new Vector3(458.5437, 261.1554, 102.7775), new Vector3(-0.04348389, -0.1405418, 247.9011)),
            new RentCarSpawn(7, new Vector3(474.82, 258.1346, 102.7508), new Vector3(1.066143, 0.2817946, 67.78595)),
            new RentCarSpawn(7, new Vector3(473.1094, 254.5523, 102.7824), new Vector3(0.7669627, 0.004181491, 67.50043)),
            new RentCarSpawn(7, new Vector3(471.8403, 250.4892, 102.7856), new Vector3(0.4057048, 0.04992635, 68.24734)),
            new RentCarSpawn(7, new Vector3(470.3324, 246.4821, 102.7858), new Vector3(0.4291589, 0.05247339, 69.04037)),

            new RentCarSpawn(8, new Vector3(-1319.002, 279.3514, 63.33144), new Vector3(-3.940038, 1.284945, 216.7388)),
            new RentCarSpawn(8, new Vector3(-1322.048, 276.9469, 63.14417), new Vector3(-3.79128, 0.9342517, 217.3976)),
            new RentCarSpawn(8, new Vector3(-1324.862, 274.4718, 62.95032), new Vector3(-4.34403, 1.377009, 217.6935)),
            new RentCarSpawn(8, new Vector3(-1327.994, 272.187, 62.74102), new Vector3(-4.290639, 1.231431, 218.7312)),
            new RentCarSpawn(8, new Vector3(-1330.795, 269.3916, 62.4989), new Vector3(-4.374604, 1.293092, 221.1572)),
            new RentCarSpawn(8, new Vector3(-1333.715, 266.8156, 62.26476), new Vector3(-4.638453, 1.551632, 221.1018)),
            new RentCarSpawn(8, new Vector3(-1333.715, 266.8156, 62.26476), new Vector3(-4.638453, 1.551632, 221.1018)),
            new RentCarSpawn(8, new Vector3(-1351.321, 245.3186, 59.97367), new Vector3(-5.362645, 1.536691, 9.003052)),
            new RentCarSpawn(8, new Vector3(-1347.455, 246.1217, 60.18134), new Vector3(-5.07542, 1.608242, 7.691864)),
            new RentCarSpawn(8, new Vector3(-1343.763, 246.9039, 60.37434), new Vector3(-5.085855, 1.796465, 9.3909)),
            new RentCarSpawn(8, new Vector3(-1339.83, 247.435, 60.55632), new Vector3(-4.888669, 1.466121, 7.253693)),

            new RentCarSpawn(9, new Vector3(-512.2274, 48.3715, 52.15555), new Vector3(0.3769324, 0.06426719, 84.32648)),
            new RentCarSpawn(9, new Vector3(-511.9635, 51.70662, 52.1563), new Vector3(0.5931334, 0.1350429, 83.948)),
            new RentCarSpawn(9, new Vector3(-511.8048, 55.17993, 52.15588), new Vector3(0.3865346, 0.1851488, 84.5896)),
            new RentCarSpawn(9, new Vector3(-511.2399, 58.76492, 85.27954), new Vector3(0.4060883, 0.09852947, 85.27954)),
            new RentCarSpawn(9, new Vector3(-510.7679, 62.07866, 52.1559), new Vector3(0.3325565, 0.09247382, 85.27368)),
            new RentCarSpawn(9, new Vector3(-510.7644, 65.54324, 52.15617), new Vector3(0.3875904, 0.1701448, 85.24359)),
            new RentCarSpawn(9, new Vector3(-519.9171, 66.44729, 52.16188), new Vector3(0.2810165, 0.1737145, 84.83469)),
            new RentCarSpawn(9, new Vector3(-520.3444, 63.01442, 52.15625), new Vector3(0.3766993, 0.05146263, 85.13681)),
            new RentCarSpawn(9, new Vector3(-522.2057, 56.09792, 52.15612), new Vector3(0.3802727, 0.06624217, 83.50162)),
            new RentCarSpawn(9, new Vector3(-522.3361, 52.66425, 52.15601), new Vector3(0.3874057, 0.1123808, 83.55823)),

            new RentCarSpawn(10, new Vector3(-1110.0714, -1687.2632, 3.9690647), new Vector3(1.414125, -9.993989, 123.23767)),
            new RentCarSpawn(10, new Vector3(-1110.706, -1686.3707, 3.9668412), new Vector3(1.414125, -9.993989, 123.23767)),
            new RentCarSpawn(10, new Vector3(-1111.2255, -1685.5366, 3.9628434), new Vector3(1.414125, -9.993989, 123.23767)),
            new RentCarSpawn(10, new Vector3(-1111.7163, -1684.698, 3.958074), new Vector3(1.414125, -9.993989, 123.23767)),
            new RentCarSpawn(10, new Vector3(-1113.8, -1681.3245, 3.7240539), new Vector3(1.414125, -9.993989, 123.23767)),

            new RentCarSpawn(11, new Vector3(-2033.8209, -144.04314, 26.91546), new Vector3(1.6794965, -3.8138664, 11.867685)),
            new RentCarSpawn(11, new Vector3(-2037.5754, -144.74968, 26.645937), new Vector3(1.6794965, -3.8138664, 11.867685)),
            new RentCarSpawn(11, new Vector3(-2041.4047, -146.07176, 26.258629), new Vector3(1.6794965, -3.8138664, 11.867685)),
            new RentCarSpawn(11, new Vector3(-2045.335, -146.86227, 25.958672), new Vector3(1.6794965, -3.8138664, 11.867685)),
            new RentCarSpawn(11, new Vector3(-2020.643, -140.14137, 27.943287), new Vector3(1.6794965, -3.8138664, 11.867685)),


            new RentCarSpawn(12, new Vector3(-745.3629, -1468.709, 5.656288), new Vector3(-0.036325663, 0.011740752, -39.67728)),
            new RentCarSpawn(12, new Vector3(-722.495, -1440.7362, 5.150613), new Vector3(-0.036325663, 0.011740752, -39.67728)),
            new RentCarSpawn(12, new Vector3(-745.56995, -1434.0243, 5.6562576), new Vector3(-0.036325663, 0.011740752, -39.67728)),
            new RentCarSpawn(12, new Vector3(-760.76086, -1451.5165, 5.1507726), new Vector3(-0.036325663, 0.011740752, -39.67728)),
            new RentCarSpawn(12, new Vector3(-721.09955, -1470.0712, 5.1487603), new Vector3(-0.036325663, 0.011740752, -39.67728)),

            new RentCarSpawn(13, new Vector3(-982.55176, -2974.0996, 14.366255), new Vector3(0, 0, 61.192303)),
            new RentCarSpawn(13, new Vector3(-1003.38635, -3003.5627, 14.57441), new Vector3(0, 0, 61.192303)),
            new RentCarSpawn(13, new Vector3(-951.10834, -2991.7686, 14.12587), new Vector3(0, 0, 61.192303)),
            new RentCarSpawn(13, new Vector3(-971.0648, -3022.2236, 14.377407), new Vector3(0, 0, 61.192303)),

            new RentCarSpawn(14, new Vector3(-1632.236, -1165.9996, 0.6303333), new Vector3(6.93445, -0.1506541, 132.22064)),
            new RentCarSpawn(14, new Vector3(-1628.162, -1171.5198, 0.29829717), new Vector3(6.93445, -0.1506541, 132.22064)),
            new RentCarSpawn(14, new Vector3(-1626.0686, -1178.5404, 0.23963523), new Vector3(6.93445, -0.1506541, 132.22064)),
            new RentCarSpawn(14, new Vector3(-1639.209, -1161.8162, 0.33406627), new Vector3(6.93445, -0.1506541, 132.22064)),

            new RentCarSpawn(15, new Vector3(1704.5836, 3765.2153, 34.47876), new Vector3(0.75567406, 1.393849, -43.4506)),
            new RentCarSpawn(16, new Vector3(1034.3667, -768.3401, 58.108864), new Vector3(0.05719879, 0.018552255, -123.13604)),
            new RentCarSpawn(17, new Vector3(939.675, 152.07137, 80.936646), new Vector3(0.0025635208, 0.045237403, -99.849365)),
            new RentCarSpawn(18, new Vector3(-1528.9316, -441.73447, 35.548016), new Vector3(0.004259539, 0.039786126, -40.206657)),
            new RentCarSpawn(19, new Vector3(-815.7129, -1098.1162, 11.002188), new Vector3(-2.0329742, 0.6184526, -60.136856)),
            new RentCarSpawn(20, new Vector3(666.50916, 677.23474, 129.01698), new Vector3(-0.007891121, 0.07989738, -169.08899)),
            new RentCarSpawn(21, new Vector3(224.46017, 1190.2013, 225.56647), new Vector3(0.0, 0.0587127, -76.008934)),
            new RentCarSpawn(22, new Vector3(-2298.184, 432.209, 174.57285), new Vector3(-0.0010881771, 0.02322577, 83.91831)),

            new RentCarSpawn(23, new Vector3(393.37677, -641.63324, 28.607044), new Vector3(0.029187638, 0.02156517, -90.404854)),
            new RentCarSpawn(23, new Vector3(392.59622, -638.68616, 28.500456), new Vector3(0.029187638, 0.02156517, -90.404854)),
            new RentCarSpawn(23, new Vector3(392.97382, -644.4817, 28.500397), new Vector3(0.029187638, 0.02156517, -90.404854)),
            new RentCarSpawn(23, new Vector3(392.7322, -646.9236, 28.500397), new Vector3(0.029187638, 0.02156517, -90.404854)),
            new RentCarSpawn(23, new Vector3(392.94098, -649.8968, 28.500372), new Vector3(0.029187638, 0.02156517, -90.404854)),
            new RentCarSpawn(23, new Vector3(394.044, -652.29694, 28.500374), new Vector3(0.029187638, 0.02156517, -90.404854)),
            new RentCarSpawn(23, new Vector3(393.366, -654.85095, 28.500374), new Vector3(0.029187638, 0.02156517, -90.404854)),
            new RentCarSpawn(23, new Vector3(393.30997, -657.5077, 28.50092), new Vector3(0.029187638, 0.02156517, -90.404854)),

            new RentCarSpawn(24, new Vector3(581.05286, 2737.7097, 42.094994), new Vector3(0.8488338, -0.13431707, -176.13246)),
            new RentCarSpawn(25, new Vector3(2759.5288, 3451.1345, 55.994274), new Vector3(0.5461421, 0.9669539, 67.04312)),
            new RentCarSpawn(26, new Vector3(-87.695526, 6352.424, 31.5962), new Vector3(-0.0012338621, 0.011630188, -133.93575)),
            new RentCarSpawn(27, new Vector3(-3141.3203, 1117.3033, 20.808395), new Vector3(-0.121122316, 0.10135113, -80.176315)),
            new RentCarSpawn(28, new Vector3(-369.13602, -192.6861, 37.23995), new Vector3(-2.2892237, 1.984975, 114.70944)),
            new RentCarSpawn(29, new Vector3(1691.1672, 4782.2334, 42.027508), new Vector3(-0.0001255144, 0.031130577, 90.62616)),

            new RentCarSpawn(30, new Vector3(433.10574, -602.8735, 28.106157), new Vector3(-0.8563, -1.3178291, 28.106157)),
            new RentCarSpawn(30, new Vector3(433.45776, -605.58044, 28.106354), new Vector3(-0.8563, -1.3178291, 28.106354)),
            new RentCarSpawn(30, new Vector3(433.2148, -608.30786, 26.106222), new Vector3(-0.8563, -1.3178291, 26.106222)),
             new RentCarSpawn(30, new Vector3(432.78452, -610.89014, 28.106138), new Vector3(-0.8563, -1.3178291, 28.106138)),

            new RentCarSpawn(31, new Vector3(-1330.758, 44.09792, 53.48625), new Vector3(1.926815, -0.7770224, 272.733)),
            new RentCarSpawn(31, new Vector3(-1331.057, 45.90104, 53.48906), new Vector3(1.926815, -0.7770224, 272.733)),
            new RentCarSpawn(31, new Vector3(-1331.293, 49.59402, 53.4978), new Vector3(0.06539849, -1.081534, 269.8514)),
            new RentCarSpawn(31, new Vector3(-1331.297, 51.49276, 53.5071), new Vector3(-1.502, -1.38771, 274.4296)),
            new RentCarSpawn(31, new Vector3(-1331.475, 53.58579, 53.53268), new Vector3(0.578413, 0.2098309, 272.2005)),

            new RentCarSpawn(32, new Vector3(461.65768, -611.4021, 28.48324), new Vector3(-0.02752222, 0.001027761, 34.50727)),
            new RentCarSpawn(32, new Vector3(460.96286, -619.0856, 28.486208), new Vector3(-0.0284836, 0.0007392637, 34.50727)),
            new RentCarSpawn(32, new Vector3(460.36304, -626.06683, 28.473759), new Vector3(-0.0414436, -0.04651098, 34.50727)),

            new RentCarSpawn(33, new Vector3(475.4551, -581.0132, 28.572805), new Vector3(-0.02752222, 0.001027761, -95.00352)), //Trucker
            new RentCarSpawn(33, new Vector3(474.9516, -586.224, 28.570614), new Vector3(-0.0284836, 0.0007392637, -95.00352)),
            new RentCarSpawn(33, new Vector3(474.65176, -591.8634, 28.57178), new Vector3(-0.0414436, -0.04651098, -95.00352)),


            new RentCarSpawn(34, new Vector3(408.09454, -651.9381, 28.107466), new Vector3(0, 0, -90.16944)),
            new RentCarSpawn(34, new Vector3(408.2031, -646.3877, 28.105696), new Vector3(0, 0, -89.16944)),
            new RentCarSpawn(34, new Vector3(408.5927, -641.38855, 28.10735), new Vector3(0, 0, -89.16944)),


            new RentCarSpawn(35, new Vector3(432.19687, -616.2206, 28.212759), new Vector3(-0.4174736, -0.398448, -93.879036)),
            new RentCarSpawn(35, new Vector3(431.44775, -620.1358, 28.213644), new Vector3(-1.748471, 0.07867802, -93.879036)),
            new RentCarSpawn(35, new Vector3(431.3113, -624.54877, 28.212446), new Vector3(0.1475076, -0.6247618, -93.879036)),


            new RentCarSpawn(36, new Vector3(1026.7847, 2655.2761, 39.033 ), new Vector3(1.4389486, -10.552636, 3.4545832)),
            new RentCarSpawn(37, new Vector3(1708.8262, 3322.749, 40.63515 ), new Vector3(1.61088, -4.9899263, 104.962944)),
            new RentCarSpawn(38, new Vector3(1724.642, 3032.9932, 61.131077), new Vector3(-8.666602, -11.257494, 28.72162)),
            new RentCarSpawn(39, new Vector3(142.37561, 6642.599, 31.03968), new Vector3(1.6741208, -7.467835, -134.68036)),
            new RentCarSpawn(40, new Vector3(1433.9878, 6327.6343, 23.490828 ), new Vector3(0.41281283, -10.429041, 9.506355)),
            new RentCarSpawn(41, new Vector3( 266.994, 2576.17, 44.550083), new Vector3(1.8884326, -10.119145, 94.80255)),
            new RentCarSpawn(42, new Vector3(-2171.7441, 4284.0054, 48.57687), new Vector3(-0.19050123, -9.797245, -116.29667)),
            new RentCarSpawn(43, new Vector3(2558.0513, 4638.4062, 33.555645), new Vector3(1.4744326, -12.007178, 34.060837)),
            new RentCarSpawn(44, new Vector3(2534.4753, 4112.6577, 38.917324), new Vector3(0, 0, 150.14401)),
            new RentCarSpawn(45, new Vector3(2032.994, 3177.2578, 45.239044), new Vector3(0, 0, -135.07437)),
            new RentCarSpawn(46, new Vector3(-1129.0437, 2695.0698, 18.278246), new Vector3(1.0227525, -14.280303, 132.4395)),
            new RentCarSpawn(47, new Vector3(-2653.5247, 2537.4373, 2.491056), new Vector3(3.386247, -14.864119, -169.03465)),
            new RentCarSpawn(48, new Vector3(-1831.0725, 4717.8604, 4.3654637), new Vector3(10.0039215, -14.434008, -11.664999)),
            new RentCarSpawn(49, new Vector3(1650.6614, 4822.275, 42.01151), new Vector3(0, 0, -79.82406)),
            new RentCarSpawn(50, new Vector3(3300.8755, 5421.423, 14.806988), new Vector3(-0.36866316, -15.634828, 105.4096)),
            new RentCarSpawn(51, new Vector3(2833.4812, -718.2979, 1.2482079), new Vector3(3.0830255, -9.975561, 56.42447)),
            new RentCarSpawn(52, new Vector3(1419.703, 3658.0242, 34.245663), new Vector3(0, 0, -69.4306)),
            new RentCarSpawn(53, new Vector3(839.58594, -3207.73, 5.383651), new Vector3(1.3912914, -8.619342, -177.07631)),
            new RentCarSpawn(54, new Vector3(-329.44315, -2467.455, 6.000638), new Vector3(0, 0, -131.25847)),
            new RentCarSpawn(55, new Vector3(618.7098, 2788.406, 42.219765), new Vector3(0, 0, 9.832371)),
            new RentCarSpawn(56, new Vector3(-1237.633, -1393.4529, 3.5960727), new Vector3(1.3184941, -5.553617, 28.4425)),
            new RentCarSpawn(57, new Vector3(1394.9974, -2070.8127, 51.47276), new Vector3(0.9165503, -14.998191, 42.2178)),
            
            //
            new RentCarSpawn(58, new Vector3(416.29645, -643.93823, 28.281652), new Vector3(-4.6620817, 4.7889757, 89.40946)),
            new RentCarSpawn(58, new Vector3(416.27982, -649.317, 28.283028), new Vector3(-4.6620817, 4.7889757, 89.40946)),
            new RentCarSpawn(58, new Vector3(416.46463, -654.5794, 28.281485), new Vector3(-4.6620817, 4.7889757, 89.40946)),
            //
            new RentCarSpawn(59, new Vector3(-472.08853, -312.73453, 34.352623), new Vector3(2.2234323, 0.110054456, 22.536076)),
            new RentCarSpawn(59, new Vector3(-469.5927, -311.55832, 34.356796), new Vector3(2.2234323, 0.110054456, 22.536076)),
            new RentCarSpawn(59, new Vector3(-481.6924, -316.39682, 34.369534), new Vector3(2.2234323, 0.110054456, 22.536076)),
            new RentCarSpawn(59, new Vector3(-479.127, -315.04065, 34.377766), new Vector3(2.2234323, 0.110054456, 22.536076)),
            
            //
            new RentCarSpawn(60, new Vector3(-229.7352, -2655.4016, 5.561528), new Vector3(0.2472642, 0.0266275, -1.2886565)),
            new RentCarSpawn(60, new Vector3(-225.03607, -2656.1172, 5.561744), new Vector3(0.1952224, -0.0025922605, 0.61881626)),
            
            //
            new RentCarSpawn(61, new Vector3(-2347.691, 3435.4895, 28.729162), new Vector3(-7.3660207, 1.7780894, 139.44058)),
            new RentCarSpawn(61, new Vector3(-2350.7917, 3437.5107, 28.548496), new Vector3(-10.955082, 1.3881285, 134.61066)),
       };

        public static RentZoneData[] RentPedsData = new RentZoneData[]
        {
            //new RentZoneData(1, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.EliteRent), new Vector3(-153.19, -158.982, 43.62119), 112.6956f, RentCarId.Elite),
            new RentZoneData(2, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(-280.0288, -889.7692, 31.0806), 252.4524f, RentCarId.Civilian),
           // new RentZoneData(3, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.RaceRent), new Vector3(-37.98943, -2095.628, 16.70483), 112.7483f, RentCarId.Rally),
            //new RentZoneData(4, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(-1869.13, -359.697, 49.30879), 322.3027f, RentCarId.Civilian),
            // new RentZoneData(5, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.EventRent), new Vector3(-905.1248, -2337.17, 6.709028), 0.0f, RentCarId.Holiday),
            new RentZoneData(6, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(453.2874, -1149.766, 29.29178), 180.0f, RentCarId.Civilian),
            new RentZoneData(7, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(461.9777, 217.6612, 103.1002), 0.0f, RentCarId.Civilian),
            //new RentZoneData(8, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(-1313.945, 282.8484, 64.09132), 0.0f, RentCarId.Civilian),
            //new RentZoneData(9, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(-513.6987, 43.07299, 52.57989), 0.0f, RentCarId.Civilian),
           //new RentZoneData(10, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.BikeRent), new Vector3(-1109.3678, -1694.4069, 4.5645723), 0.0f, RentCarId.Cycling),
            //new RentZoneData(11, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.OffRoadRent), new Vector3(-2040.9161, -130.33661, 27.593317), 175.0f, RentCarId.OffRoad),
            //new RentZoneData(12, "s_m_m_pilot_01", LangFunc.GetText(LangType.Ru, DataName.HeliRent), new Vector3(-736.5372, -1458.0835, 5.00052), 0.0f, RentCarId.Helicopter),
            //new RentZoneData(13, "s_m_m_pilot_01", LangFunc.GetText(LangType.Ru, DataName.PlaneRent), new Vector3(-993.7938, -2947.3257, 13.957196), 0.0f, RentCarId.Aeroplane),
            new RentZoneData(14, "s_m_y_uscg_01", LangFunc.GetText(LangType.Ru, DataName.BoatRent), new Vector3(-1606.993, -1127.9418, 2.1554325), 0.0f, RentCarId.WaterBased),
            //new RentZoneData(15, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(1699.369, 3774.0999, 34.731514), -126.795135f, RentCarId.Civilian),
            //new RentZoneData(16, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(1037.1823, -765.38556, 57.977104), -120.06861f, RentCarId.Civilian),
            //new RentZoneData(17, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(934.37427, 148.65912, 80.83027), -88.601776f, RentCarId.Civilian),
            new RentZoneData(18, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(-1541.2733, -437.8889, 35.596672), -79.82406f, RentCarId.Civilian),
            new RentZoneData(19, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(-820.0671, -1100.6382, 11.156218), -59.84502f, RentCarId.Civilian),
            //new RentZoneData(20, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(657.36163, 687.5908, 129.04634), 160.71507f, RentCarId.Civilian),
            //ew RentZoneData(21, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(219.64946, 1192.6567, 225.55067), -78.82406f, RentCarId.Civilian),
            //new RentZoneData(22, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(-2294.3545, 429.43195, 174.60149), 81.051025f, RentCarId.Civilian),
            new RentZoneData(23, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(394.68338, -633.62024, 28.70693), -88.82406f, RentCarId.Civilian),
            //new RentZoneData(24, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(583.3127, 2739.7866, 42.095203), -171.795135f, RentCarId.Civilian),
            //new RentZoneData(25, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(2752.168, 3467.383, 55.74715), -117.795135f, RentCarId.Civilian),
            //new RentZoneData(26, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(-72.81145, 6346.49, 31.586294), 0.0f, RentCarId.Civilian),
            //new RentZoneData(27, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(-3140.1387, 1130.3076, 20.851465), -120.795135f, RentCarId.Civilian),
            //new RentZoneData(28, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(-389.61102, -196.51826, 36.54216), -159.795135f, RentCarId.Civilian),
            //new RentZoneData(29, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(1695.415, 4785.0933, 41.996357), 52.643776f, RentCarId.Civilian),

            new RentZoneData(30, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.TaxiRent), new Vector3(437.08667, -627.85693, 28.707752), 70.70735f, RentCarId.JobTaxi),
            new RentZoneData(31, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.LandmowerRent), new Vector3(-1330.482, 42.12986, 53.48915), 0.0f, RentCarId.JobLawnmower),
            new RentZoneData(32, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.BusRent), new Vector3(435.23758, -653.01117, 28.730534), 78.42807f, RentCarId.JobBus),
            new RentZoneData(33, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.FuraRent), new Vector3(438.97568, -606.2591, 28.713915), 109.040101f, RentCarId.JobTrucker),
  	        new RentZoneData(34, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.FurgonRent), new Vector3(438.951, -613.8331, 28.71138), 78.51024f, RentCarId.JobCollector),          
            new RentZoneData(35, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.MechanikRent), new Vector3(440.13922, -599.6496, 28.714296), 78.0f, RentCarId.JobMechanic),

         // new RentZoneData(36, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(1024.0071, 2651.1096, 39.55115), 3.2891867f, RentCarId.Civilian),
         // new RentZoneData(37, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(1711.9036, 3320.435, 41.146633), 105.430374f, RentCarId.Civilian),
         // new RentZoneData(38, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(1725.8662, 3029.0952, 62.1895), 70.05024f, RentCarId.Civilian),
         // new RentZoneData(39, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(145.26793, 6646.386, 31.503784), -168.58095f, RentCarId.Civilian),
         // new RentZoneData(40, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(1436.4434, 6331.325, 23.934277), 95.294754f, RentCarId.Civilian),
         // new RentZoneData(41, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(268.53894, 2573.1646, 45.064144), 42.072704f, RentCarId.Civilian),
         // new RentZoneData(42, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(-2172.9595, 4281.6562, 49.09), -121.75311f, RentCarId.Civilian),
         // new RentZoneData(43, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(2553.2017, 4639.9536, 34.07682), -10.622228f, RentCarId.Civilian),
         // new RentZoneData(44, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(2531.4392, 4113.914, 38.756462), -113.949425f, RentCarId.Civilian),
         // new RentZoneData(45, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(2034.2146, 3180.105, 45.25282), 154.2865f, RentCarId.Civilian),
         // new RentZoneData(46, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(-1125.9249, 2694.8164, 18.800394), 93.99415f, RentCarId.Civilian),
         // new RentZoneData(47, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(-2647.9492, 2532.6375, 3.0417547), 33.042896f, RentCarId.Civilian),
         // new RentZoneData(48, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(-1837.5392, 4714.268, 4.317833), -139.0106f, RentCarId.Civilian),
         // new RentZoneData(49, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(1651.5188, 4828.98, 42.023285), -169.2284f, RentCarId.Civilian),
         // new RentZoneData(50, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(3304.9111, 5424.6646, 15.494226), -124.61217f, RentCarId.Civilian),
            new RentZoneData(51, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(2830.9817, -710.8333, 1.97693), 81.07968f, RentCarId.Civilian),
          //  new RentZoneData(52, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(1419.0496, 3654.1768, 34.52287), 116.757744f, RentCarId.Civilian),
          //  new RentZoneData(53, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(834.9719, -3204.43, 5.9008045), -174.2475f, RentCarId.Civilian),
            new RentZoneData(54, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(-330.19974, -2464.6208, 6.0006385), 142.68982f, RentCarId.Civilian),
         //   new RentZoneData(55, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(619.00146, 2785.884, 43.48116), 7.0879054f, RentCarId.Civilian),
            new RentZoneData(56, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(-1237.501, -1395.8168, 4.141041), 122.99152f, RentCarId.Civilian),
            new RentZoneData(57, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(1389.4164, -2073.133, 51.99858), 53.371437f, RentCarId.Civilian),
            //
            new RentZoneData(58, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.FurgonRent2), new Vector3(437.9226, -621.9111, 28.7092), 83.0f, RentCarId.JobPostman),
            new RentZoneData(59, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(-478.59937, -304.3146, 35.113594), -157f, RentCarId.Civilian),
            new RentZoneData(60, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(-227.15689, -2666.8054, 6.0002985),  7.2f, RentCarId.Civilian), // НУПАУАУАУ
          //  new RentZoneData(61, "a_m_y_genstreet_01", LangFunc.GetText(LangType.Ru, DataName.CarRent), new Vector3(-2334.4495, 3422.5938, 29.965755), 104.25f, RentCarId.Civilian),
        };
        private class RentBlipData
        {
            public int BlipId { set; get; }
            public string BlipName { set; get; }
            public int Color { set; get; }

            public RentBlipData(int BlipId, string BlipName, int Color)
            {
                this.BlipId = BlipId;
                this.BlipName = BlipName;
                this.Color = Color;
            }
        }

        private static IReadOnlyDictionary<RentCarId, RentBlipData> RentBlipsData = new Dictionary<RentCarId, RentBlipData>()
        {
            { RentCarId.Civilian, new RentBlipData(76, LangFunc.GetText(LangType.Ru, DataName.CarRent), 9) },
            { RentCarId.Cycling, new RentBlipData(512, LangFunc.GetText(LangType.Ru, DataName.BikeRent), 9) },
            { RentCarId.OffRoad, new RentBlipData(757, LangFunc.GetText(LangType.Ru, DataName.OffRoadRent), 9) },
            { RentCarId.WaterBased, new RentBlipData(76, LangFunc.GetText(LangType.Ru, DataName.BoatRent), 9) },
            { RentCarId.Helicopter, new RentBlipData(753, LangFunc.GetText(LangType.Ru, DataName.HeliRent), 9) },
            { RentCarId.Aeroplane, new RentBlipData(579, LangFunc.GetText(LangType.Ru, DataName.PlaneRent), 9) },
            //{ RentCarId.Holiday, new RentBlipData(747, LangFunc.GetText(LangType.Ru, DataName.EventRent), 9) },
            //{ RentCarId.Elite, new RentBlipData(633, LangFunc.GetText(LangType.Ru, DataName.EliteRent), 9) },
            //{ RentCarId.Rally, new RentBlipData(726, LangFunc.GetText(LangType.Ru, DataName.RaceRent), 9) },
           { RentCarId.JobTaxi, new RentBlipData(2, LangFunc.GetText(LangType.Ru, DataName.Taxi), 46) },
            { RentCarId.JobLawnmower, new RentBlipData(109, LangFunc.GetText(LangType.Ru, DataName.Landmower), 4) },
            { RentCarId.JobBus, new RentBlipData(2, LangFunc.GetText(LangType.Ru, DataName.Bus), 26) },
            { RentCarId.JobTrucker, new RentBlipData(2, LangFunc.GetText(LangType.Ru, DataName.Gruzovik), 4) },
            { RentCarId.JobCollector, new RentBlipData(2, LangFunc.GetText(LangType.Ru, DataName.Inkass), 63) },
            { RentCarId.JobMechanic, new RentBlipData(2, LangFunc.GetText(LangType.Ru, DataName.Mechanik), 4) },
            { RentCarId.JobPostman, new RentBlipData(2, LangFunc.GetText(LangType.Ru, DataName.Postman), 4) },
        };

        private static Dictionary<int, int> PedsToRentCarId = new Dictionary<int, int>();

        [ServerEvent(Event.ResourceStart)]
        public void Init()
        {
            try
            {
                int i = 0;
                foreach(RentZoneData rentPedData in RentPedsData)
                {
                    if (!RentBlipsData.ContainsKey(rentPedData.Index)) continue;
                    RentBlipData rentBlisessionData = RentBlipsData[rentPedData.Index];
                    
                    if (rentPedData.ZoneId < 36 || rentPedData.ZoneId > 57)
                        Main.CreateBlip(new Main.BlipData(rentBlisessionData.BlipId, rentBlisessionData.BlipName, rentPedData.Position, rentBlisessionData.Color, true, 1f));
                    
                    var ped = PedSystem.Repository.CreateQuest(rentPedData.Skin, rentPedData.Position, rentPedData.Heading, title: $"~y~NPC~w~ {rentPedData.Title}", colShapeEnums: ColShapeEnums.RentCar);

                    if (!PedsToRentCarId.ContainsKey(ped.Value)) PedsToRentCarId.Add(ped.Value, i);
                    i++;
                }
            }
            catch (Exception e)
            {
                Log.Write($"StartWork Exception: {e.ToString()}");
            }
        }
        [Interaction(ColShapeEnums.RentCar)]
        public static void OnRentMenu(ExtPlayer player, int Index)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (!PedsToRentCarId.ContainsKey(Index)) return;
                if (!FunctionsAccess.IsWorking("RentCar"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }

                var rentCarId = RentPedsData[PedsToRentCarId[Index]].Index;

                var rentCarsData = new List<List<object>>();
                foreach(var rentCarData in RentCarsData)
                {
                    if (!rentCarData.CarsId.Contains(rentCarId)) 
                        continue;

                    var carData = new List<object>();
                    
                    carData.Add(rentCarData.Id);
                    carData.Add(rentCarData.Model);
                    carData.Add(rentCarData.Price);
                    carData.Add(rentCarData.Job != JobsId.None);

                    rentCarsData.Add(carData);
                }
                Trigger.ClientEvent(player, "client.rentcar.open", JsonConvert.SerializeObject(rentCarsData));
            }
            catch (Exception e)
            {
                Log.Write($"OnRentMenu Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.rentcar.buy")]
        public static void RentCarToInterface(ExtPlayer player, int carId, int colorId, int hour)
        {
            try
            {

                var sessionData = player.GetSessionData();
                if (sessionData == null)
                    return;

                var accountData = player.GetAccountData();
                if (accountData == null)
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
                
                if (player.IsInVehicle)
                {
                    //Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете арендовать т/с находясь в авто.", 3000);
                    Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Rent, LangFunc.GetText(LangType.Ru, DataName.CantRentAuto), DateTime.Now); 
                    return;
                }
                if(sessionData.RentData != null)
                {
                    //Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы уже арендуете т/с.", 3000);
                    Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Rent, LangFunc.GetText(LangType.Ru, DataName.AlreadyRent), DateTime.Now); 
                    return;
                }
                else if(0 > carId || carId >= RentCarsData.Length) return;
                RentCarData rentCarsData = RentCarsData[carId];

                switch (rentCarsData.Job)
                {
                    case JobsId.Taxi:
                        if (Main.ServerSettings.IsCheckJobLicC && !characterData.Licenses[2])
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoLicenceC), 10000);
                            return;
                        }
                        
                        if (characterData.WorkID != (int)JobsId.Taxi)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BeforeNeedToWork), 3000);
                            return;
                        }

                        (byte, float) jobLevelInfo = characterData.JobSkills.ContainsKey((int)JobsId.Taxi) ? Main.GetPlayerJobLevelBonus((int)JobsId.Taxi, characterData.JobSkills[(int)JobsId.Taxi]) : (0, 1);
                        if (jobLevelInfo.Item1 <= 1 && rentCarsData.Model != "taxi" ||
                            jobLevelInfo.Item1 >= 2 && jobLevelInfo.Item1 <= 4 && rentCarsData.Model != "cognoscenti" && rentCarsData.Model != "bmwm5")
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehicleNotAvaible, rentCarsData.Model, jobLevelInfo.Item1), 8000);
                            return;
                        }

                        break;
                    case JobsId.Trucker:
                        if (Main.ServerSettings.IsCheckJobLicC && !characterData.Licenses[2])
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoLicenceC), 10000);
                            return;
                        }

                        if (characterData.WorkID != (int)JobsId.Trucker)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BeforeNeedToWork), 3000);
                            return;
                        }
                        
                        /*if (characterData.BizIDs.Count > 0)
                        {
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantStartWork), 6000);
                            return;
                        }*/
                        
                        jobLevelInfo = characterData.JobSkills.ContainsKey((int)JobsId.Trucker) ? Main.GetPlayerJobLevelBonus((int)JobsId.Trucker, characterData.JobSkills[(int)JobsId.Trucker]) : (0, 1);
                        if (jobLevelInfo.Item1 <= 1 && rentCarsData.Model != "pounder" ||
                            jobLevelInfo.Item1 >= 2 && jobLevelInfo.Item1 <= 3 && rentCarsData.Model != "pounder" && rentCarsData.Model != "benson")
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehicleNotAvaible, rentCarsData.Model, jobLevelInfo.Item1), 8000);
                            return;
                        }

                        break;
                    case JobsId.Bus:
                        if (Main.ServerSettings.IsCheckJobLicC && !characterData.Licenses[2])
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoLicenceC), 10000);
                            return;
                        }

                        if (characterData.WorkID != (int)JobsId.Bus)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BeforeNeedToWork), 3000);
                            return;
                        }

                        jobLevelInfo = characterData.JobSkills.ContainsKey((int)JobsId.Bus) ? Main.GetPlayerJobLevelBonus((int)JobsId.Bus, characterData.JobSkills[(int)JobsId.Bus]) : (0, 1);
                        if (jobLevelInfo.Item1 <= 2 && rentCarsData.Model != "airbus" ||
                            jobLevelInfo.Item1 >= 3 && jobLevelInfo.Item1 <= 4 && rentCarsData.Model != "airbus" && rentCarsData.Model != "bus")
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehicleNotAvaible, rentCarsData.Model, jobLevelInfo.Item1), 8000);
                            return;
                        }

                        break;
                    case JobsId.CashCollector:
                        if (Main.ServerSettings.IsCheckJobLicC && !characterData.Licenses[2])
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoLicenceC), 10000);
                            return;
                        }

                        if (characterData.WorkID != (int)JobsId.CashCollector)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BeforeNeedToWork), 3000);
                            return;
                        }

                        jobLevelInfo = characterData.JobSkills.ContainsKey((int)JobsId.CashCollector) ? Main.GetPlayerJobLevelBonus((int)JobsId.CashCollector, characterData.JobSkills[(int)JobsId.CashCollector]) : (0, 1);
                        if (jobLevelInfo.Item1 <= 2 && rentCarsData.Model != "stockade" ||
                            jobLevelInfo.Item1 >= 3 && jobLevelInfo.Item1 <= 4 && rentCarsData.Model != "stockade" && rentCarsData.Model != "terbyte")
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehicleNotAvaible, rentCarsData.Model, jobLevelInfo.Item1), 8000);
                            return;
                        }

                        break;
                    case JobsId.Lawnmower:
                    case JobsId.CarMechanic:
                    case JobsId.Postman:
                        if (Main.ServerSettings.IsCheckJobLicC && !characterData.Licenses[2])
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoLicenceC), 10000);
                            return;
                        }

                        if (rentCarsData.Job != (JobsId)characterData.WorkID)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BeforeNeedToWork), 3000);
                            return;
                        }

                        break;
                }

                int price = rentCarsData.Price > 0 ? GetRentCarCash(accountData.VipLvl, characterData.LVL, rentCarsData.Price * hour) : 0;
                if (UpdateData.CanIChange(player, price, true) != 255) return;
                int zoneId = CustomColShape.GetDataToEnum(player, ColShapeEnums.RentCar);
                if (zoneId == (int)ColShapeData.Error) return;
                if (!PedsToRentCarId.ContainsKey(zoneId)) return;
                int spawnId = GetRentCarSpawn(RentPedsData[PedsToRentCarId[zoneId]].ZoneId);
                if (spawnId == -1) return;
                var rentCarsSpawn = RentCarsSpawn[spawnId];

                string number = $"RCAR{player.Value}";
                var vehicleCreate = (ExtVehicle) null;

                
                switch (rentCarsData.Job)
                {
                    case JobsId.None:
                        number = VehicleManager.GenerateNumber(VehicleAccess.Rent, "RCAR");
                        vehicleCreate = VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey(rentCarsData.Model), rentCarsSpawn.Position, rentCarsSpawn.Rotation.Z, 1, 1, number, acc: VehicleAccess.Rent, petrol: 9999);
                        break;
                    case JobsId.Lawnmower:
                        number = VehicleManager.GenerateNumber(VehicleAccess.Work, "LAWN-");//$"LAWN-{player.Value}";
                        vehicleCreate = VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey(rentCarsData.Model), rentCarsSpawn.Position, rentCarsSpawn.Rotation.Z, 1, 1, number, acc: VehicleAccess.Work, work: JobsId.Lawnmower, petrol: 9999, workdriv: characterData.UUID);
                        break;
                    case JobsId.Taxi:
                        number = VehicleManager.GenerateNumber(VehicleAccess.Work, "TAXI-");//$"TAXI-{player.Value}";
                        vehicleCreate = VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey(rentCarsData.Model), rentCarsSpawn.Position, rentCarsSpawn.Rotation.Z, 1, 1, number, acc: VehicleAccess.Work, work: JobsId.Taxi, petrol: 9999, workdriv: characterData.UUID);
                        break;
                    case JobsId.Bus:
                        number = VehicleManager.GenerateNumber(VehicleAccess.Work, "BUS-");//$"BUS-{player.Value}";
                        vehicleCreate = VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey(rentCarsData.Model), rentCarsSpawn.Position, rentCarsSpawn.Rotation.Z, 1, 1, number, acc: VehicleAccess.Work, work: JobsId.Bus, petrol: 9999, workdriv: characterData.UUID);
                        break;
                    case JobsId.Trucker:
                        number = VehicleManager.GenerateNumber(VehicleAccess.Work, "TRUCK-");//$"TRUCK-{player.Value}";
                        vehicleCreate = VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey(rentCarsData.Model), rentCarsSpawn.Position, rentCarsSpawn.Rotation.Z, 1, 1, number, acc: VehicleAccess.Work, work: JobsId.Trucker, petrol: 9999, workdriv: characterData.UUID);
                        break;
                    case JobsId.CashCollector:
                        number = VehicleManager.GenerateNumber(VehicleAccess.Work, "CLR-");//$"COLLECTOR-{player.Value}";
                        vehicleCreate = VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey(rentCarsData.Model), rentCarsSpawn.Position, rentCarsSpawn.Rotation.Z, 1, 1, number, acc: VehicleAccess.Work, work: JobsId.CashCollector, petrol: 9999, workdriv: characterData.UUID);
                        break;
                    case JobsId.CarMechanic:
                        number = VehicleManager.GenerateNumber(VehicleAccess.Work, "MECH-");//$"MECH-{player.Value}";
                        vehicleCreate = VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey(rentCarsData.Model), rentCarsSpawn.Position, rentCarsSpawn.Rotation.Z, 1, 1, number, acc: VehicleAccess.Work, work: JobsId.CarMechanic, petrol: 9999, workdriv: characterData.UUID);
                        break;
                    case JobsId.Postman:
                        number = VehicleManager.GenerateNumber(VehicleAccess.Work, "POST-");//$"POSTAL-{player.Value}";
                        vehicleCreate = VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey(rentCarsData.Model), rentCarsSpawn.Position, rentCarsSpawn.Rotation.Z, 111, 111, number, acc: VehicleAccess.Work, work: JobsId.Postman, petrol: 9999, workdriv: characterData.UUID);
                        break;
                }

                if (vehicleCreate == null)
                    return;

                //

                switch (rentCarsData.Job)
                {
                    case JobsId.Lawnmower:
                        Jobs.Repository.OnPlayerExitVehicle(player, vehicleCreate);
                        Jobs.Lawnmower.StartWork(player);
                        break;
                    case JobsId.Postman:               
                        Jobs.Repository.OnPlayerExitVehicle(player, vehicleCreate);
                        vehicleCreate.SetMod(48, 1);
                        vehicleCreate.SetMod(0, 1);
                        break;
                    case JobsId.Taxi:
                        Jobs.Repository.OnPlayerExitVehicle(player, vehicleCreate);
                        Players.Phone.Taxi.Orders.Repository.StartWork(player);
                        break;
                    case JobsId.Bus:
                        Jobs.Repository.OnPlayerExitVehicle(player, vehicleCreate);
                        Jobs.Bus.StartWork(player);
                        break;
                    case JobsId.CarMechanic:
                        Jobs.Repository.OnPlayerExitVehicle(player, vehicleCreate);
                        Players.Phone.Mechanic.Orders.Repository.StartWork(player);
                        break;
                    case JobsId.Trucker:
                        if (rentCarsData.Model == "pounder")
                        {
                            vehicleCreate.SetMod(11, 3);
                            vehicleCreate.SetMod(18, 0);
                            vehicleCreate.SetMod(13, 2);
                        }
                        Jobs.Repository.OnPlayerExitVehicle(player, vehicleCreate);
                        Jobs.Truckers.StartWork(player);
                        break;
                    case JobsId.CashCollector:
                        Jobs.Repository.OnPlayerExitVehicle(player, vehicleCreate);
                        Jobs.Collector.StartWork(player);
                        break;
                }

                //
                vehicleCreate.CustomPrimaryColor = RentColorData[colorId];
                vehicleCreate.CustomSecondaryColor = RentColorData[colorId];

                Trigger.ClientEvent(player, "client.rentcar.point", vehicleCreate.Value);
                //Trigger.ClientEvent(player, "createWaypoint", _rentCarsSpawn.Position.X, _rentCarsSpawn.Position.Y);

                string correctMessage = LangFunc.GetText(LangType.Ru, DataName.hours);
                if (hour == 1) correctMessage = LangFunc.GetText(LangType.Ru, DataName.hour);
                else if (hour == 2 || hour == 3 || hour == 4) correctMessage = LangFunc.GetText(LangType.Ru, DataName.houra);

                //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SuccRentVehJob, hour, correctMessage), 8000);
                Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Rent, LangFunc.GetText(LangType.Ru, DataName.SuccRentVehJob, rentCarsData.Model), DateTime.Now);
                BattlePass.Repository.UpdateReward(player, 26);

                sessionData.RentData = new RentData(rentCarsData.Price, DateTime.Now.AddMinutes(60 * hour), vehicleCreate, rentCarsData.Model, number, rentCarsData.Job != JobsId.None);

                var vehicleLocalData = vehicleCreate.GetVehicleLocalData();
                vehicleLocalData.WorkDriver = characterData.UUID;

                vehicleLocalData.RentCarPrice = sessionData.RentData.Price;
                vehicleLocalData.RentCarTime = sessionData.RentData.Date;
                vehicleLocalData.RentCarModel = sessionData.RentData.Model;

                if (price > 0)
                {
                    MoneySystem.Wallet.Change(player, -price);
                    GameLog.Money($"player({characterData.UUID})", $"server", price, $"rentCar({number})");
                }
            }
            catch (Exception e)
            {
                Log.Write($"RentCar Exception: {e.ToString()}");
            }            
        }

        public static void OnPlayerSpawn(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null)
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            var vehicle = RAGE.Entities.Vehicles.All.Cast<ExtVehicle>()
                .Where(v => v.VehicleLocalData != null)
                .Where(v => v.VehicleLocalData.Access == VehicleAccess.Rent || v.VehicleLocalData.Access == VehicleAccess.Work)
                .Where(v => v.VehicleLocalData.IsOwnerExit)
                .FirstOrDefault(v => v.VehicleLocalData.WorkDriver == characterData.UUID);

            var vehicleLocalData = vehicle.GetVehicleLocalData();
            if (vehicleLocalData != null)
            {
                vehicleLocalData.IsOwnerExit = false;
                sessionData.RentData = new RentData(vehicleLocalData.RentCarPrice, vehicleLocalData.RentCarTime, vehicle, vehicleLocalData.RentCarModel, vehicleLocalData.NumberPlate, vehicleLocalData.WorkId != JobsId.None);
                
                switch (vehicleLocalData.WorkId)
                {
                    case JobsId.Lawnmower:
                        Jobs.Repository.OnPlayerExitVehicle(player, vehicle);
                        Jobs.Lawnmower.StartWork(player);
                        break;
                    case JobsId.Postman:               
                        Jobs.Repository.OnPlayerExitVehicle(player, vehicle);
                        break;
                    case JobsId.Taxi:
                        Jobs.Repository.OnPlayerExitVehicle(player, vehicle);
                        Players.Phone.Taxi.Orders.Repository.StartWork(player);
                        break;
                    case JobsId.Bus:
                        Jobs.Repository.OnPlayerExitVehicle(player, vehicle);
                        Jobs.Bus.StartWork(player);
                        break;
                    case JobsId.CarMechanic:
                        Jobs.Repository.OnPlayerExitVehicle(player, vehicle);
                        Players.Phone.Mechanic.Orders.Repository.StartWork(player);
                        break;
                    case JobsId.Trucker:
                        Jobs.Repository.OnPlayerExitVehicle(player, vehicle);
                        Jobs.Truckers.StartWork(player);
                        break;
                    case JobsId.CashCollector:
                        Jobs.Repository.OnPlayerExitVehicle(player, vehicle);
                        Jobs.Collector.StartWork(player);
                        break;
                }
            }
            
        }
        
        
        public static int GetRentCarCash(int VipLvl, int level, int Price)
        {

            switch (VipLvl)
            {
                case 1:
                    Price = Convert.ToInt32(Price * 0.9);
                    break;
                case 2:
                    Price = Convert.ToInt32(Price * 0.85);
                    break;
                case 3:
                    Price = Convert.ToInt32(Price * 0.8);
                    break;
                case 4:
                case 5:
                    Price = Convert.ToInt32(Price * 0.75);
                    break;
                default:
                    break;
            }
            return GetRentCarCashToLevel (level, Price);
        }
        public static int GetRentCarCashToLevel(int level, int Price)
        {
            if (level <= 2) Price = Convert.ToInt32(Price * 1.0);
            else if (level <= 4) Price = Convert.ToInt32(Price * 1.5);
            else if (level <= 6) Price = Convert.ToInt32(Price * 2.0);
            else if (level <= 9) Price = Convert.ToInt32(Price * 4.5);
            else if (level <= 19) Price = Convert.ToInt32(Price * 6.0);
            else Price = Convert.ToInt32(Price * 8.0);
            return Price;
        }
        private static Dictionary<int, int> RentCarsSpawnCount = new Dictionary<int, int>();
        private static int GetRentCarSpawn(int ZoneId)
        {
            int firstIndex = -1;
            int realIndex = -1;
            int index = -1;
            if (RentCarsSpawnCount.ContainsKey(ZoneId)) realIndex = RentCarsSpawnCount[ZoneId];
            int i = 0;
            foreach (RentCarSpawn _rentCarsSpawn in RentCarsSpawn)
            {
                if (firstIndex == -1 && _rentCarsSpawn.ZoneId == ZoneId)
                    firstIndex = i;

                if (_rentCarsSpawn.ZoneId == ZoneId && i > realIndex)
                {
                    index = i;
                    break;
                }
                i++;
            }

            if (index == -1)
            {
                index = firstIndex;
                if (!RentCarsSpawnCount.ContainsKey(ZoneId)) RentCarsSpawnCount.Add(ZoneId, firstIndex);
                else RentCarsSpawnCount[ZoneId] = firstIndex;
            }
            else if (index != -1)
            {
                if (!RentCarsSpawnCount.ContainsKey(ZoneId)) RentCarsSpawnCount.Add(ZoneId, index);
                else RentCarsSpawnCount[ZoneId] = index;
            }
            else if (realIndex != -1)
            {
                if (!RentCarsSpawnCount.ContainsKey(ZoneId)) RentCarsSpawnCount.Add(ZoneId, realIndex);
                else RentCarsSpawnCount[ZoneId] = realIndex;
                index = realIndex;
            }
            return index;
        }
        
        public static void OnReturnVehicle(ExtPlayer player, bool msg = false)
        {
            Trigger.SetMainTask(() =>
            {
                OnReturnVehicleTask(player, msg);
            });
        }
        private static void OnReturnVehicleTask(ExtPlayer player, bool msg = false)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                if (sessionData.RentData != null)
                {
                    VehicleStreaming.DeleteVehicle(sessionData.RentData.Vehicle);

                    sessionData.RentData = null;

                    if (msg) 
                        Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Rent, LangFunc.GetText(LangType.Ru, DataName.CancelledRent), DateTime.Now);

                    if (sessionData.WorkData.OnWork)
                        Jobs.Repository.JobEnd(player);
                }

            }
            catch (Exception e)
            {
                Log.Write($"OnReturnVehicle Exception: {e.ToString()}");
            }

        }
        
        [RemoteEvent("server.rentcar.func")]
        private static void OnRentCarFunc(ExtPlayer player, string listItem)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                var accountData = player.GetAccountData();
                if (accountData == null) 
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                if (sessionData.RentData == null) return;
                var rent = sessionData.RentData;
                switch (listItem)
                {
                    case "gpstrack":
                        Trigger.ClientEvent(player, "createWaypoint", rent.Vehicle.Position.X, rent.Vehicle.Position.Y);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MarkSelected), 3000);
                        return;
                    case "datetime":
                        int price = GetRentCarCash (accountData.VipLvl, characterData.LVL, rent.Price);
                        Trigger.ClientEvent(player, "openDialog", "RENTCARI_DATETIME", LangFunc.GetText(LangType.Ru, DataName.RentExpandConfirm, rent.Date, MoneySystem.Wallet.Format(price)));
                        return;
                    case "stoprent":
                        Trigger.ClientEvent(player, "openDialog", "RENTCARI_STOPRENT", LangFunc.GetText(LangType.Ru, DataName.RentCancelConfirm, rent.Date));
                        return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"callback_selectedrentcarss Exception: {e.ToString()}");
            }
        }
    }
}
