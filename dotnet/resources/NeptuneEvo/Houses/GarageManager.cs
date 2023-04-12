using GTANetworkAPI;
using NeptuneEvo.Handles;
using Newtonsoft.Json;
using NeptuneEvo.Core;
using Redage.SDK;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Database;
using LinqToDB;
using Localization;
using MySqlConnector;
using NeptuneEvo.Chars;
using NeptuneEvo.Functions;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Fractions;
using NeptuneEvo.VehicleData.Data;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;

namespace NeptuneEvo.Houses
{
    #region GarageType Class
    class GarageType
    {
        [JsonIgnore]
        public Vector3 Position { get; }
        [JsonIgnore]
        public List<Vector3> VehiclesPositions { get; }
        [JsonIgnore]
        public List<Vector3> VehiclesRotations { get; }
        public int MaxCars { get; }
        public int Price { get; }
        public bool IsDonate { get; }

        public GarageType(Vector3 position, List<Vector3> vehiclesPositions, List<Vector3> vehiclesRotations, int maxCars, int price = -1, bool isDonate = false)
        {
            Position = position;
            VehiclesPositions = vehiclesPositions;
            VehiclesRotations = vehiclesRotations;
            MaxCars = maxCars;
            Price = price;
            IsDonate = isDonate;
        }
    }
    #endregion

    #region Garage Class
    public class Garage
    {
        public int Id { get; }
        [JsonIgnore]
        public int BDType { get; set; }
        [JsonIgnore]
        public bool Upgraded { get; set; } = false;
        public int Type { get; set; }
        public Vector3 Position { get; }
        public Vector3 Rotation { get; }
        [JsonIgnore] 
        public int Dimension { get; set; }
        [JsonIgnore]
        private ExtColShape Shape = null;
        [JsonIgnore]
        private ExtColShape IntShape = null;
        [JsonIgnore]
        private ExtMarker IntMarker = null;
        [JsonIgnore]
        public ConcurrentDictionary<int, int> CarSlots = new ConcurrentDictionary<int, int>();
        [JsonIgnore]
        private bool Locked = false;
        [JsonIgnore]
        public bool IsSave = false;

        public Garage(int id, int type, Vector3 position, Vector3 rotation)
        {
            Id = id;
            Type = type;
            Position = position;
            Rotation = rotation;
        }
        
        public bool InGarage(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return false;
            
            if (Type == -1)
            {
                if (player.Position.DistanceTo(Position) < 4)
                    return true;
            }
            else if (Type == 6)
            {
                if (player.Position.DistanceTo(Position) < 4) 
                    return true;
            }
            else
            {
                if (Id == characterData.InsideGarageID)
                    return true;
            }
            return false;
        }
        public void DelCarSlotMarker(int autoId)
        {
            var index = CarSlots
                .Where(cs => cs.Value == autoId)
                .Select(cs => cs.Key)
                .FirstOrDefault();
             
            if (CarSlots.ContainsKey(index))
            {
                CarSlots.TryRemove(index, out _);
                IsSave = true;
            }
        }
        public int UpdateCarSlot(int autoId)
        {
            try
            {
                var isKey = CarSlots.Values.Contains(autoId);
                
                var key = CarSlots
                    .Where(cs => cs.Value == autoId)
                    .Select(cs => cs.Key)
                    .FirstOrDefault();
                
                var garageType = GarageManager.GarageTypes[Type];

                if (isKey && key >= garageType.MaxCars)
                {
                    CarSlots.TryRemove(key, out _);
                    key = -1;
                }

                //
                if (key == -1 || !CarSlots.ContainsKey(key) || !isKey)
                {
                    key = -1;
                    for (int i = 0; i < garageType.MaxCars; i++)
                    {
                        if (!CarSlots.ContainsKey(i))
                        {
                            key = i;
                            break;
                        }
                    }
                    if (key == -1) 
                        return -1;
                    
                    CarSlots[key] = autoId;
                    IsSave = true;
                    return key;
                }
                //
                return key;
            }
            catch (Exception e)
            {
                GarageManager.Log.Write($"UpdateCarSlot Exception: {e.ToString()}");
                return -1;
            }
        }

        public void DeleteCarToName(string name)
        {
            var isUpdate = false;
            
            foreach(var carSlot in CarSlots.ToList())
            {
                var vehicleData = VehicleManager.GetVehicleToAutoId(carSlot.Value);
                if (vehicleData.Holder == name)
                {
                    DestroyCar(vehicleData.Number);
                    CarSlots.TryRemove(carSlot.Key, out _);
                    isUpdate = true;
                }
            }
            
            if (isUpdate) 
                IsSave = true;
        }
        public bool IsGarageToNumber(int autoId)
        {
            if (!CarSlots.Values.Contains(autoId))
                return false;
            
            var vehicleData = VehicleManager.GetVehicleToAutoId(autoId);
            if (vehicleData == null)
                return false;
            
            var vehicle = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Personal, vehicleData.Number);
            if (vehicle == null)
                return false;
            
            var vehicleLocalData = vehicle.GetVehicleLocalData();
            if (vehicleLocalData == null)
                return false;
            
            return vehicleLocalData.Access == VehicleAccess.Garage;
        }
        public bool IsCarNumber(int autoId)
        {
            if (Type == -1 || Type == 6)
                return true;
            
            return CarSlots.Values.Contains(autoId);
        }

        public void Lock(bool toggled) => Locked = toggled;
        
        public void DeleteCar(string number, bool resetPosition = true, bool isRevive = false)
        {
            try
            {
                var vehicleData = VehicleManager.GetVehicleToNumber(number);
                
                if (isRevive && vehicleData != null) 
                    DelCarSlotMarker(vehicleData.SqlId);
                
                var vehiclePlayer = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Personal, number);
                if (vehiclePlayer != null)
                    VehicleStreaming.DeleteVehicle(vehiclePlayer);

                if (resetPosition && vehicleData != null)
                    vehicleData.Position = null;
            }
            catch (Exception e)
            {
                GarageManager.Log.Write($"DeleteCar Exception: {e.ToString()}");
            }
        }
        public void CreateShape()
        {
            try
            {
                if(Shape != null) return;
                Shape = CustomColShape.CreateCylinderColShape(Position - new Vector3(0, 0, 1), 2f, 4f, 0, ColShapeEnums.EnterGarage, Id);
            }
            catch (Exception e)
            {
                GarageManager.Log.Write($"CreateShape Exception: {e.ToString()}");
            }
        }
        public void Create()
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");

                    await db.InsertAsync(new Garages
                    {
                        Id = Id,
                        Type = Type,
                        Position = JsonConvert.SerializeObject(Position),
                        Rotation = JsonConvert.SerializeObject(Rotation),
                        CarSlots = JsonConvert.SerializeObject(new ConcurrentDictionary<int, int>())
                    });
                }
                catch (Exception e)
                {
                    GarageManager.Log.Write($"Create Exception: {e.ToString()}");
                }
            });
        }
        public async Task Save(ServerBD db)
        {
            try
            {
                IsSave = false;
                
                await db.Garages
                    .Where(g => g.Id == Id)
                    .Set(g => g.Upgraded, Upgraded ? Type : -1)
                    .Set(g => g.CarSlots, JsonConvert.SerializeObject(CarSlots))
                    .UpdateAsync();
            }
            catch (Exception e)
            {
                GarageManager.Log.Write($"Save Exception: {e.ToString()}");
            }
        }
        public void Delete()
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");
                
                    await db.Garages
                        .Where(g => g.Id == Id)
                        .DeleteAsync();
                }
                catch (Exception e)
                {
                    GarageManager.Log.Write($"Delete Exception: {e.ToString()}");
                }
            });
        }
        public void Destroy(bool withShape = true)
        {
            try
            {
                if (withShape) 
                {
                    CustomColShape.DeleteColShape(Shape);
                    Shape = null;
                }
                
                CustomColShape.DeleteColShape(IntShape);
                
                IntShape = null;
                
                if (IntMarker != null && IntMarker.Exists) 
                    IntMarker.Delete();
                
                IntMarker = null;
            }
            catch (Exception e)
            {
                GarageManager.Log.Write($"Destroy Exception: {e.ToString()}");
            }
        }
        public void SpawnCar(string number, bool check = false)
        {
            try
            {
                var vehicleData = VehicleManager.GetVehicleToNumber(number);
                if (vehicleData == null) 
                    return;
                if (VehicleModel.AirAutoRoom.isAirCar(vehicleData.Model))
                    return;
                if (Ticket.IsVehicleTickets(vehicleData.SqlId))
                    return;
                
                var index = UpdateCarSlot(vehicleData.SqlId);
                
                if (index == -1) 
                    return;
                
                var garageType = GarageManager.GarageTypes[Type];
                var vehicle = VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey(vehicleData.Model), 
                    garageType.VehiclesPositions[index], 
                    garageType.VehiclesRotations[index].Z, 0, 0, 
                    number, dimension: (uint)Dimension, locked: true, engine: false, acc: VehicleAccess.Garage, petrol: vehicleData.Fuel, dirt: vehicleData.Dirt);
                
                vehicleData.Position = null;
                VehicleManager.ApplyCustomization(vehicle);
            }
            catch (Exception e)
            {
                GarageManager.Log.Write($"SpawnCar Exception: {e.ToString()}");
            }
        }
        /// <summary>
        /// Заспавнить все автомобили
        /// </summary>
        /// <param name="vehiclesNumber"></param>
        public void SpawnCars(List<string> vehiclesNumber, House house = null)
        {
            try
            {              
                //
                if (house != null)
                {
                    var isUpdate = false;
                    foreach(var carSlot in CarSlots.ToList())
                    {
                        var vehicleData = VehicleManager.GetVehicleToAutoId(carSlot.Value);
                        if (vehicleData == null || !vehiclesNumber.Contains(vehicleData.Number) || (vehicleData.Holder != house.Owner && !house.Roommates.ContainsKey(vehicleData.Holder)))
                        {
                            if (vehicleData != null)
                                DestroyCar(vehicleData.Number);
                            
                            CarSlots.TryRemove(carSlot.Key, out _);
                            isUpdate = true;
                        }
                    }
                    if (isUpdate) 
                        IsSave = true;
                }
                //
                foreach (var vehicleNumber in vehiclesNumber)
                {
                    SpawnCarToPos(vehicleNumber, house);
                    
                    if (Type == -1 || Type == 6) 
                        break;
                }
            }
            catch (Exception e)
            {
                GarageManager.Log.Write($"SpawnCars Exception: {e.ToString()}");
            }
        }
        public void SpawnCarToPos(string vehicleNumber, House house = null)
        {
            try
            {
                var vehicleData = VehicleManager.GetVehicleToNumber(vehicleNumber);
        
                if (vehicleData == null) 
                    return;
                    
                if (VehicleData.LocalData.Repository.IsVehicleToNumber(VehicleAccess.Personal, vehicleData.Number))
                    return;

                if (house != null && house.Roommates.ContainsKey(vehicleData.Holder) && !CarSlots.Values.Contains(vehicleData.SqlId))
                    return;
                
                if (!string.IsNullOrEmpty(vehicleData.Position)) 
                    SpawnCarAtPosition(vehicleData.Number, JsonConvert.DeserializeObject<Vector3>(vehicleData.Position), JsonConvert.DeserializeObject<Vector3>(vehicleData.Rotation));
                else
                {
                    if (Type != -1 && Type != 6) 
                        SpawnCar(vehicleData.Number);
                    else 
                        GetVehicleFromGarage(vehicleData.Number);
                }
            }
            catch (Exception e)
            {
                GarageManager.Log.Write($"SpawnCarToPos Exception: {e.ToString()}");
            }
        }
        public void DestroyCars(List<string> vehiclesNumber)
        {
            foreach (var vehicleNumber in vehiclesNumber)
            {
                var vehicleData = VehicleManager.GetVehicleToNumber(vehicleNumber);
                
                if (vehicleData != null) 
                    DestroyCar(vehicleData.Number);
            }
        }

        public void DestroyCar(string number)
        {
            var vehiclePlayer = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Personal, number);
            if (vehiclePlayer == null)
                return;
            
            var vehicleStateData = vehiclePlayer.GetVehicleLocalStateData();
            var vehicleData = VehicleManager.GetVehicleToNumber(number);
            if (vehicleData != null && vehicleStateData != null)
                vehicleData.Dirt = vehicleStateData.Dirt;

            VehicleStreaming.DeleteVehicle(vehiclePlayer);
        }
        
        public void SpawnCarAtPosition(string number, Vector3 position, Vector3 rotation)
        {
            try
            {
                var vehicleData = VehicleManager.GetVehicleToNumber(number);
                if (vehicleData == null) 
                    return;
                if (VehicleModel.AirAutoRoom.isAirCar(vehicleData.Model))
                    return;
                if (Ticket.IsVehicleTickets(vehicleData.SqlId))
                    return;
                
                if (Type != -1 && Type != 6)
                {
                    if (GarageManager.GarageTypes[Type].Position.DistanceTo(position) <= 50)
                    {
                        vehicleData.Position = null;
                        SpawnCar(number);
                        return;
                    }
                }
                
                var index = UpdateCarSlot(vehicleData.SqlId);
                if (index == -1) 
                    return;
                
                var vehicle = VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey(vehicleData.Model), position, rotation.Z, 0, 0, number, petrol: vehicleData.Fuel, acc: VehicleAccess.Personal, locked: true, dirt: vehicleData.Dirt);
                VehicleManager.ApplyCustomization(vehicle);
            }
            catch (Exception e)
            {
                GarageManager.Log.Write($"SpawnCarAtPosition Exception: {e.ToString()}");
            }
        }
        public void GetVehicleFromGarage(string number, ExtPlayer sender = null)
        {
            try
            {
                var vehicleData = VehicleManager.GetVehicleToNumber(number);
                if (vehicleData == null) 
                    return;
                
                if (VehicleModel.AirAutoRoom.isAirCar(vehicleData.Model))
                    return;
                
                if (Ticket.IsVehicleTickets(vehicleData.SqlId))
                    return;
                
                if (Type != -1 && Type != 6) // Other Garages
                {
                    UpdateCarSlot(vehicleData.SqlId);
                    var vehicle = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Personal, number);
                    var vehicleLocalData = vehicle.GetVehicleLocalData();
                    
                    if (vehicleLocalData != null)
                    {
                        vehicleLocalData.Access = VehicleAccess.Personal;

                        vehicle.Dimension = 0;
                        vehicle.Position = Position;
                        vehicle.Rotation = Rotation;

                        VehicleManager.ApplyCustomization(vehicle);
                    }
                    if (sender != null) Trigger.Dimension(sender);
                }
                else
                {
                    var vehicle = VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey(vehicleData.Model), Position + new Vector3(0, 0, 0.3), Rotation.Z, 0, 0, number, petrol: vehicleData.Fuel, acc: VehicleAccess.Personal, dirt: vehicleData.Dirt);
                    VehicleStreaming.SetLockStatus(vehicle, true);
                    VehicleManager.ApplyCustomization(vehicle);
                }
            }
            catch (Exception e)
            {
                GarageManager.Log.Write($"GetVehicleFromGarage Exception: {e.ToString()}");
            }
        }

        public void SendVehicleIntoGarage(string number)
        {
            try
            {
                var vehicleData = VehicleManager.GetVehicleToNumber(number);
                if (vehicleData == null) return;
                if (vehicleData.Health == 0) return;
                
                vehicleData.Position = null;
                
                var vehiclePlayer = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Personal, number);
                if (vehiclePlayer != null)
                    VehicleStreaming.DeleteVehicle(vehiclePlayer);
                
                if (Type != -1 && Type != 6) 
                    SpawnCar(number);
                
            }
            catch (Exception e)
            {
                GarageManager.Log.Write($"SendVehicleIntoGarage Exception: {e.ToString()}");
            }
        }
        public void SendPlayer(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                if (Locked) return;
                
                if (Type == 7) 
                    Trigger.ClientEvent(player, "garageload");
                
                Trigger.Dimension(player, Convert.ToUInt32(Dimension));
                NAPI.Entity.SetEntityPosition(player, GarageManager.GarageTypes[Type].Position);
                characterData.InsideGarageID = Id;
            }
            catch (Exception e)
            {
                GarageManager.Log.Write($"SendPlayer Exception: {e.ToString()}");
            }
        }
        public void RemovePlayer(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                Trigger.Dimension(player);
                player.Position = Position;
                characterData.InsideGarageID = -1;
            }
            catch (Exception e)
            {
                GarageManager.Log.Write($"RemovePlayer Exception: {e.ToString()}");
            }
        }

        public void SendVehiclesInsteadNearest(List<string> vehiclesNumber)
        {
            try
            {
                foreach (var number in vehiclesNumber)
                {
                    var vehiclePlayer = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Personal, number);
                    if (vehiclePlayer == null) continue;
                    vehiclePlayer.SavePosition();
                    VehicleStreaming.DeleteVehicle(vehiclePlayer);
                }
                
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке
                    
                        foreach (var number in vehiclesNumber)
                            await VehicleManager.SaveSql(db, number);
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
            }
            catch (Exception e)
            {
                GarageManager.Log.Write($"SendVehiclesInsteadNearest Exception: {e.ToString()}");
            }
        }
        public void CreateInterior()
        {
            try
            {
                if (Type != 7) IntMarker = (ExtMarker) NAPI.Marker.CreateMarker(1, GarageManager.GarageTypes[Type].Position - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 2f, new Color(255, 255, 255, 220), false, (uint)Dimension);

                IntShape = CustomColShape.CreateCylinderColShape(GarageManager.GarageTypes[Type].Position - new Vector3(0, 0, 1.12), 2f, 4f, (uint)Dimension, ColShapeEnums.ExitGarage);
            }
            catch (Exception e)
            {
                GarageManager.Log.Write($"CreateInterior Exception: {e.ToString()}");
            }
        }
    }
    #endregion

    class GarageManager : Script
    {
        public static readonly nLog Log = new nLog("Houses.GarageManager");
        public static Dictionary<int, Garage> Garages = new Dictionary<int, Garage>();
        public static Dictionary<int, GarageType> GarageTypes = new Dictionary<int, GarageType>()
        {
            { -1, new GarageType(new Vector3(), new List<Vector3>(), new List<Vector3>(), 1) },
            { 0, new GarageType(new Vector3(178.9925, -1005.661, -99.5995),
                new List<Vector3>(){
                    new Vector3(171.0881, -1004.269, -99.41191),
                    new Vector3(174.7538, -1004.269, -99.41191),
                },
                new List<Vector3>(){
                    new Vector3(-0.1147747, 0.02747092, 183.3471),
                    new Vector3(-0.1147747, 0.02747092, 183.3471),
                }, 2, price: Main.PricesSettings.GaragesPrice[0])},
            { 1, new GarageType(new Vector3(206.9094, -999.0917, -100),
                new List<Vector3>(){
                    new Vector3(200.5315, -997.5886, -99.41107),
                    new Vector3(196.7847, -997.5886, -99.41107),
                    new Vector3(193.0101, -997.5886, -99.41107),
                },
                new List<Vector3>(){
                    new Vector3(-0.1146501, -0.03047129, 165.095),
                    new Vector3(-0.1146501, -0.03047129, 165.095),
                    new Vector3(-0.1146501, -0.03047129, 165.095),
                }, 3, price: Main.PricesSettings.GaragesPrice[1])},
            { 2, new GarageType(new Vector3(206.9094, -999.0917, -100),
                new List<Vector3>(){
                    new Vector3(200.5315, -997.5886, -99.41107),
                    new Vector3(196.7847, -997.5886, -99.41107),
                    new Vector3(193.0101, -997.5886, -99.41107),
                    //
                    new Vector3(193.0101, -1004.212, -99.42397),
                },
                new List<Vector3>(){
                    new Vector3(-0.1146501, -0.03047129, 165.095),
                    new Vector3(-0.1146501, -0.03047129, 165.095),
                    new Vector3(-0.1146501, -0.03047129, 165.095),
                    new Vector3(-0.1146501, -0.03047129, 165.095),
                }, 4, price: Main.PricesSettings.GaragesPrice[2])},
            { 3, new GarageType(new Vector3(206.9094, -999.0917, -100),
                new List<Vector3>(){
                    new Vector3(200.5315, -997.5886, -99.41107),
                    new Vector3(196.7847, -997.5886, -99.41107),
                    new Vector3(193.0101, -997.5886, -99.41107),
                    //
                    new Vector3(193.0101, -1004.212, -99.41107),
                    new Vector3(196.7847, -1004.212, -99.41107),
                },
                new List<Vector3>(){
                    new Vector3(-0.1146501, -0.03047129, 165.095),
                    new Vector3(-0.1146501, -0.03047129, 165.095),
                    new Vector3(-0.1146501, -0.03047129, 165.095),
                    new Vector3(-0.1146501, -0.03047129, 165.095),
                    new Vector3(-0.1146501, -0.03047129, 165.095),
                }, 5, price: Main.PricesSettings.GaragesPrice[3])},
            { 4, new GarageType(new Vector3(206.9094, -999.0917, -100),
                new List<Vector3>(){
                    new Vector3(200.5315, -997.5886, -99.41107),
                    new Vector3(196.7847, -997.5886, -99.41107),
                    new Vector3(193.0101, -997.5886, -99.41107),
                    //
                    new Vector3(193.0101, -1004.212, -99.41107),
                    new Vector3(196.7847, -1004.212, -99.41107),
                    new Vector3(200.5315, -1004.212, -99.41107),
                },
                new List<Vector3>(){
                    new Vector3(-0.1146501, -0.03047129, 165.095),
                    new Vector3(-0.1146501, -0.03047129, 165.095),
                    new Vector3(-0.1146501, -0.03047129, 165.095),
                    new Vector3(-0.1146501, -0.03047129, 165.095),
                    new Vector3(-0.1146501, -0.03047129, 165.095),
                    new Vector3(-0.1146501, -0.03047129, 165.095),
                }, 6, price: Main.PricesSettings.GaragesPrice[4])},
            { 5, new GarageType(new Vector3(240.411, -1004.753, -100),
                new List<Vector3>(){
                    new Vector3(223.6285, -981.4785, -99.4215),
                    new Vector3(223.6285, -985.4429, -99.4215),
                    new Vector3(223.6285, -989.4567, -99.4215),
                    new Vector3(223.6285, -993.5819, -99.4215),
                    new Vector3(223.6285, -997.4755, -99.4215),
                    //
                    new Vector3(233.3275, -997.4755, -99.4215),
                    new Vector3(233.3275, -993.5819, -99.4215),
                    new Vector3(233.3276, -989.4567, -99.4215),
                    new Vector3(233.3275, -985.4429, -99.4215),
                    new Vector3(233.3275, -981.4785, -99.4215),
                },
                new List<Vector3>(){
                    new Vector3(0, 0, -87.35272),
                    new Vector3(0, 0, -87.35272),
                    new Vector3(0, 0, -87.35272),
                    new Vector3(0, 0, -87.35272),
                    new Vector3(0, 0, -87.35272),
                    new Vector3(0, 0, 91.82264),
                    new Vector3(0, 0, 91.82264),
                    new Vector3(0, 0, 91.82264),
                    new Vector3(0, 0, 91.82264),
                    new Vector3(0, 0, 91.82264),
                }, 10, price:  Main.PricesSettings.GaragesPrice[5])},
            { 6, new GarageType(new Vector3(), new List<Vector3>(), new List<Vector3>(), 1) },
            { 7, new GarageType(new Vector3(-1396.274, -480.7186, 57.100), // Херня в эдиторе
                new List<Vector3>(){
                    new Vector3(-1389.815, -477.88, 57.31421),
                    new Vector3(-1385.699, -472.8283, 57.31421),
                    new Vector3(-1377.489, -472.0624, 57.31421),
                    new Vector3(-1371.994, -475.9771, 57.31421),
                    new Vector3(-1370.637, -482.9035, 57.31421),
                    //
                    new Vector3(-1389.815, -477.88, 62.65846),
                    new Vector3(-1385.699, -472.8283, 62.65846),
                    new Vector3(-1377.489, -472.0624, 62.65846),
                    new Vector3(-1371.994, -475.9771, 62.65846),
                    new Vector3(-1370.637, -482.9035, 62.65846),
                    //
                    new Vector3(-1389.815, -477.88, 68.0045),
                    new Vector3(-1385.699, -472.8283, 68.0045),
                    new Vector3(-1377.489, -472.0624, 68.0045),
                    new Vector3(-1371.994, -475.9771, 68.0045),
                    new Vector3(-1370.637, -482.9035, 68.0045),
                },
                new List<Vector3>() {
                    new Vector3(0.0, 0.0, 252.1797),
                    new Vector3(0.0, 0.0, 223.5804),
                    new Vector3(0.0, 0.0, 168.2739),
                    new Vector3(0.0, 0.0, 122.8532),
                    new Vector3(0.0, 0.0, 74.71387),
                    new Vector3(0.0, 0.0, 252.1797),
                    new Vector3(0.0, 0.0, 223.5804),
                    new Vector3(0.0, 0.0, 168.2739),
                    new Vector3(0.0, 0.0, 122.8532),
                    new Vector3(0.0, 0.0, 74.71387),
                    new Vector3(0.0, 0.0, 252.1797),
                    new Vector3(0.0, 0.0, 223.5804),
                    new Vector3(0.0, 0.0, 168.2739),
                    new Vector3(0.0, 0.0, 122.8532),
                    new Vector3(0.0, 0.0, 74.71387),
                }, 15, price: Main.PricesSettings.GaragesPrice[6], isDonate: true)},
            { 8, new GarageType(new Vector3(1295.379, 264.5836, -49.875),
                new List<Vector3>(){
                    new Vector3(1308.695, 259.915, -49.16245),//0
                    new Vector3(1308.695, 256.3404, -49.16245),//1
                    new Vector3(1308.695, 251.7303, -49.16245),//2
                    new Vector3(1308.695, 248.1548, -49.16245),//3
                    new Vector3(1308.695, 243.0133, -49.16245),//4
                    new Vector3(1308.695, 239.4378, -49.16245),//5
                    new Vector3(1308.695, 233.3687, -49.16245),//6
                    new Vector3(1308.695, 229.7931, -49.16245),//7
                    //
                    new Vector3(1295.229, 229.7931, -49.16245),
                    new Vector3(1295.229, 233.3687, -49.16245),
                    new Vector3(1295.264, 239.4377, -49.16245),
                    new Vector3(1295.264, 243.0134, -49.16245),
                    new Vector3(1295.191, 248.1548, -49.16245),
                    new Vector3(1295.191, 251.7303, -49.16245),
                    //
                    new Vector3(1280.856, 260.2309, -49.16245),
                    new Vector3(1280.856, 256.6555, -49.16245),
                    new Vector3(1280.856, 251.7303, -49.16245),
                    new Vector3(1280.856, 248.1548, -49.16245),
                    new Vector3(1280.856, 243.0134, -49.16245),
                    new Vector3(1280.856, 239.4377, -49.16245),
                    //
                    new Vector3(1279.068, 223.5146, -49.16245),
                    new Vector3(1282.643, 223.5145, -49.16245),
                    new Vector3(1286.972, 223.5145, -49.16245),
                    //
                    new Vector3(1308.695, 223.2435, -49.16245),
                },
                new List<Vector3>(){
                    new Vector3(0, 0, 90.8486),//0
                    new Vector3(0, 0, 90.8486),//1
                    new Vector3(0, 0, 90.8486),//2
                    new Vector3(0, 0, 90.8486),//3
                    new Vector3(0, 0, 90.8486),//4
                    new Vector3(0, 0, 90.8486),//5
                    new Vector3(0, 0, 90.8486),//6
                    new Vector3(0, 0, 90.8486),//7
                    //
                    new Vector3(0, 0, -90.8486),//0
                    new Vector3(0, 0, -90.8486),
                    new Vector3(0, 0, -90.8486),
                    new Vector3(0, 0, -90.8486),
                    new Vector3(0, 0, -90.8486),
                    new Vector3(0, 0, -90.8486),
                    new Vector3(0, 0, -90.8486),
                    new Vector3(0, 0, -90.8486),
                    new Vector3(0, 0, -90.8486),
                    new Vector3(0, 0, -90.8486),
                    new Vector3(0, 0, -90.8486),
                    new Vector3(0, 0, -90.8486),
                    new Vector3(0, 0, 0.3728),
                    new Vector3(0, 0, 0.3728),
                    new Vector3(0, 0, 0.3728),
                    //
                    new Vector3(0, 0, 90.8486),//7
                }, 23, price: Main.PricesSettings.GaragesPrice[7], isDonate: true)},
            { 9, new GarageType(new Vector3(1380.1096, 259.4516, -48.99376),
                new List<Vector3>(){
                    //
                    new Vector3(1395.148, 254.5341, -48.90687),//37
                    new Vector3(1395.148, 250.4407, -48.90687),//36
                    new Vector3(1395.148, 246.3205, -48.90687),//35
                    new Vector3(1395.148, 242.2271, -48.90687),//34
                    new Vector3(1395.148, 237.8307, -48.90687),//33
                    new Vector3(1395.148, 233.7374, -48.90687),//32
                    new Vector3(1395.148, 229.6171, -48.90687),//31
                    new Vector3(1395.148, 225.5238, -48.90687),//30
                    new Vector3(1395.148, 221.188, -48.90687),//29
                    new Vector3(1395.148, 217.0946, -48.90687),//28
                    new Vector3(1395.148, 212.9744, -48.90687),//27
                    new Vector3(1395.148, 208.881, -48.90687),//26
                    new Vector3(1395.148, 204.4276, -48.90687),//25
                    new Vector3(1395.148, 200.3342, -48.90687),//24
                    //
                    new Vector3(1380.183, 208.881, -48.90687),//23
                    new Vector3(1380.183, 212.9744, -48.90687),//22
                    new Vector3(1380.183, 217.0946, -48.90687),//21
                    new Vector3(1380.183, 221.188, -48.90687),//20
                    new Vector3(1380.183, 225.5238, -48.90687),//19
                    new Vector3(1380.183, 229.6171, -48.90687),//18
                    new Vector3(1380.183, 233.7374, -48.90687),//17
                    new Vector3(1380.183, 237.8307, -48.90687),//16
                    new Vector3(1380.183, 242.2271, -48.90687),//15
                    new Vector3(1380.183, 246.3205, -48.90687),//14
                    //
                    new Vector3(1366.289, 254.5341, -48.90687),//37
                    new Vector3(1366.289, 250.4407, -48.90687),//36
                    new Vector3(1366.289, 246.3205, -48.90687),//35
                    new Vector3(1366.289, 242.2271, -48.90687),//34
                    new Vector3(1366.289, 237.8307, -48.90687),//33
                    new Vector3(1366.289, 233.7374, -48.90687),//32
                    new Vector3(1366.289, 229.6171, -48.90687),//31
                    new Vector3(1366.289, 225.5238, -48.90687),//30
                    new Vector3(1366.289, 221.188, -48.90687),//29
                    new Vector3(1366.289, 217.0946, -48.90687),//28
                    new Vector3(1366.289, 212.9744, -48.90687),//27
                    new Vector3(1366.289, 208.881, -48.90687),//26
                    new Vector3(1366.289, 204.4276, -48.90687),//25
                    new Vector3(1366.289, 200.3342, -48.90687),//24
                    //
                    new Vector3(1366.289, 196.2408, -48.90687),//24
                    new Vector3(1366.289, 192.1474, -48.90687),//24
                },
                new List<Vector3>(){
                    //
                    new Vector3(0, 0, 88.9513),//0
                    new Vector3(0, 0, 88.9513),//1
                    new Vector3(0, 0, 88.9513),//2
                    new Vector3(0, 0, 88.9513),//3
                    new Vector3(0, 0, 88.9513),//4
                    new Vector3(0, 0, 88.9513),//5
                    new Vector3(0, 0, 88.9513),//6
                    new Vector3(0, 0, 88.9513),//7
                    new Vector3(0, 0, 88.9513),//8
                    new Vector3(0, 0, 88.9513),//9
                    new Vector3(0, 0, 88.9513),//10
                    new Vector3(0, 0, 88.9513),//11
                    new Vector3(0, 0, 88.9513),//12
                    new Vector3(0, 0, 88.9513),//13
                    //
                    new Vector3(0, 0, -88.9513),//14
                    new Vector3(0, 0, -88.9513),//15
                    new Vector3(0, 0, -88.9513),//16
                    new Vector3(0, 0, -88.9513),//17
                    new Vector3(0, 0, -88.9513),//18
                    new Vector3(0, 0, -88.9513),//19
                    new Vector3(0, 0, -88.9513),//20
                    new Vector3(0, 0, -88.9513),//21
                    new Vector3(0, 0, -88.9513),//22
                    //
                    new Vector3(0, 0, -88.9513),//23
                    new Vector3(0, 0, -88.9513),//24
                    new Vector3(0, 0, -88.9513),//25
                    new Vector3(0, 0, -88.9513),//26
                    new Vector3(0, 0, -88.9513),//27
                    new Vector3(0, 0, -88.9513),//28
                    new Vector3(0, 0, -88.9513),//29
                    new Vector3(0, 0, -88.9513),//30
                    new Vector3(0, 0, -88.9513),//31
                    new Vector3(0, 0, -88.9513),//32
                    new Vector3(0, 0, -88.9513),//33
                    new Vector3(0, 0, -88.9513),//34
                    new Vector3(0, 0, -88.9513),//35
                    new Vector3(0, 0, -88.9513),//36
                    //
                    new Vector3(0, 0, -88.9513),//35
                    new Vector3(0, 0, -88.9513),//36
                }, 38, price: Main.PricesSettings.GaragesPrice[8], isDonate: true)},
        };
        public static int DimensionId = 100000;
        public static readonly int MaxGarageCars = 38;

        public static async void Init()
        {
            try
            {
                await using var db = new ServerBD("MainDB");

                var garages = db.Garages
                    .ToList();

                foreach (var garageData in garages)
                {
                    var garage = new Garage(
                        garageData.Id, 
                        garageData.Upgraded != -1 ? garageData.Upgraded : garageData.Type, 
                        JsonConvert.DeserializeObject<Vector3>(garageData.Position),
                        JsonConvert.DeserializeObject<Vector3>(garageData.Rotation));
                    
                    garage.BDType = garageData.Type;
                    garage.Upgraded = garageData.Upgraded != -1;
                    garage.Dimension = DimensionId;
                    garage.CarSlots = JsonConvert.DeserializeObject<ConcurrentDictionary<int, int>>(garageData.CarSlots);
                    
                    if (garage.Type != -1 && garage.Type != 6) 
                        garage.CreateInterior();
                    
                    Garages.Add(garageData.Id, garage);

                    DimensionId++;
                }
                Log.Write($"Loaded {Garages.Count} garages.", nLog.Type.Success);
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }

        [Interaction(ColShapeEnums.EnterGarage)]
        public static void OnEnterGarage(ExtPlayer player, int index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            if (!player.IsCharacterData()) return;
            var house = HouseManager.GetHouse(player);
            var garage = house?.GetGarageData();
            if (garage == null) 
                return;
            if (house.GarageID != garage.Id) 
                return;
            if (house.GarageID != index) 
                return;
            
            if (player.IsInVehicle)
            {
                var vehicle = (ExtVehicle) player.Vehicle;
                var number = vehicle.NumberPlate;
                var vehicleData = VehicleManager.GetVehicleToNumber(number);
                if (vehicleData == null)
                    return;
                
                if (!garage.IsCarNumber(vehicleData.SqlId))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantEnterGarageVeh), 3000);
                    return;
                }
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData == null)
                    return;
            
                if (vehicleData.Health < 1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarDestroyed), 3000);
                    return;
                }
                VehicleManager.WarpPlayerOutOfVehicle(player, false);
                //vehicleData.Items = vehicleLocalData.Items;
                vehicleData.Position = null;
                
                var vehicleStateData = vehicle.GetVehicleLocalStateData();
                if (vehicleStateData != null) 
                    vehicleData.Dirt = vehicleStateData.Dirt;

                if (garage.Type != -1 && garage.Type != 6)
                {
                    garage.SendVehicleIntoGarage(number);
                    garage.SendPlayer(player);
                }
                else
                {
                    garage.DeleteCar(number, true);
                    garage.GetVehicleFromGarage(number);
                }
                return;
            }
            

            if (garage.Type == -1 || garage.Type == 6)
            {
                var vehiclesNumber = VehicleManager.GetVehiclesCarNumberToPlayer(house.Owner);
                if (vehiclesNumber.Count == 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoCars), 3000);
                    return;
                }
                var number = vehiclesNumber[0];
                var vehicleData = VehicleManager.GetVehicleToNumber(number);
                if (vehicleData == null || vehicleData.Health < 1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarDestroyed), 3000);
                    return;
                }
                if (!garage.IsGarageToNumber(vehicleData.SqlId))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarInState), 3000);
                    return;
                }
                if (player.IsInVehicle) return;
                
                garage.GetVehicleFromGarage(number);
            }
            else
            {
                if (sessionData.Following != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomebodyYouFollow), 3000);
                    return;
                }
                if (sessionData.Follower != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OtpustiteChela), 3000);
                    return;
                }
                garage.SendPlayer(player);
            }
        }
        [Interaction(ColShapeEnums.ExitGarage)]
        private void OnExitGarage(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            if (characterData.InsideGarageID == -1) return;
            var garage = Garages[characterData.InsideGarageID];
            garage.RemovePlayer(player);
        }
        #region Commands
        [Command(AdminCommands.setgarage)]
        public static void CMD_SetGarage(ExtPlayer player, int ID)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.setgarage)) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (sessionData.HouseID == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны стоять на маркере недвижимости", 3000);
                    return;
                }

                var house = HouseManager.Houses.FirstOrDefault(h => h.ID == sessionData.HouseID);
                if (house == null) return;

                if (!Garages.ContainsKey(ID)) return;
                house.GarageID = ID;
                house.UpdateLabel();
                house.UpdateGarage();
                house.IsSave = true;
            }
            catch (Exception e)
            {
                Log.Write($"CMD_SetGarage Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.creategarage)]
        public static void CMD_CreateGarage(ExtPlayer player, int type)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.creategarage)) return;
                if (!GarageTypes.ContainsKey(type) || type == 6) return;
                if (!player.IsInVehicle)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны сидеть в машине", 3000);
                    return;
                }
                int id = 0;
                do
                {
                    id++;
                } while (Garages.ContainsKey(id));

                var garage = new Garage(id, type, player.Vehicle.Position, player.Vehicle.Rotation);
                garage.Dimension = DimensionId;
                garage.Create();
                
                if (type != -1) 
                    garage.CreateInterior();

                Garages.Add(garage.Id, garage);
                NAPI.Chat.SendChatMessageToPlayer(player, garage.Id.ToString());
            }
            catch (Exception e)
            {
                Log.Write($"CMD_CreateGarage Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.tphouse)]
        public static void CMD_TpHouse(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.tphouse)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var house = HouseManager.Houses.FirstOrDefault(h => h.ID == id);
                if (house == null) return;
                NAPI.Entity.SetEntityPosition(player, house.Position + new Vector3(0.0, 0.0, 1.0));
                Trigger.Dimension(player, 0);
                Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) телепортировался к дому {id}");
                GameLog.Admin($"{player.Name}", $"tpHouse({id})", $"null");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_TpHouse Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.tpbiz)]
        public static void CMD_tpBiz(ExtPlayer player, int id)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.tpbiz)) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!BusinessManager.BizList.ContainsKey(id)) return;
                Business biz = BusinessManager.BizList[id];
                if (biz == null) return;
                NAPI.Entity.SetEntityPosition(player, biz.EnterPoint + new Vector3(0.0, 0.0, 1.0));
                Trigger.Dimension(player, 0);
                Admin.AdminLog(characterData.AdminLVL, $"{player.Name} ({player.Value}) телепортировался к бизнесу {id}");
                GameLog.Admin($"{player.Name}", $"tpBiz({id})", $"null");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_tpBiz Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.removegarage)]
        public static void CMD_RemoveGarage(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.removegarage)) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (sessionData.GarageID == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны стоять на маркере гаража", 3000);
                    return;
                }
                if (!Garages.ContainsKey(sessionData.GarageID)) return;
                var garage = Garages[sessionData.GarageID];

                garage.Destroy();
                garage.Delete();
                Garages.Remove(sessionData.GarageID);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_RemoveGarage Exception: {e.ToString()}");
            }
        }

        #endregion
        [RemoteEvent("server.garage.parking")]
        private void UpdateCarSlots(ExtPlayer player, int autoId, int index)
        {           
            var sessionData = player.GetSessionData();
            if (sessionData == null)
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            var house = HouseManager.GetHouse(player);
            if (house == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GarageError), 3000);
                return;
            }
            
            if (!Garages.ContainsKey(house.GarageID))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GarageError), 3000);
                return;
            }
            
            if (house.GarageID != characterData.InsideGarageID)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GarageError), 3000);
                return;
            }
            
            var garage = house.GetGarageData();

            if (garage == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GarageError), 3000);
                return;
            }

            if (!GarageTypes.ContainsKey(garage.Type)) return; 

            var carSlotIndex = garage.CarSlots
                .Where(cs => cs.Value == autoId)
                .Select(cs => cs.Key)
                .FirstOrDefault();

            if (garage.CarSlots.ContainsKey(index))
            {
                if (garage.CarSlots[index] == autoId)
                {
                    Trigger.ClientEvent(player, "client.phone.cars.error");
                    return;
                }

                var vehicleData1 = VehicleManager.GetVehicleToAutoId(autoId);
                if (vehicleData1 == null)
                {
                    Trigger.ClientEvent(player, "client.phone.cars.error");
                    return;
                }

                var vehicleData2 = VehicleManager.GetVehicleToAutoId(garage.CarSlots[index]);
                if (vehicleData2 == null)
                {
                    Trigger.ClientEvent(player, "client.phone.cars.error");
                    return;
                }

                var vehicle1 = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Personal, vehicleData1.Number);
                var vehicleLocalData1 = vehicle1.GetVehicleLocalData();
                /*if (vehicle1 == null || vehicleLocalData1 == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Такой машины больше не существует.", 3000);
                    Trigger.ClientEvent(player, "client.phone.cars.error");
                    return;
                }*/
                var vehicle2 = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Personal, vehicleData2.Number);
                var vehicleLocalData2 = vehicle2.GetVehicleLocalData();
                /*if (vehicle2 == null || vehicleLocalData2 == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Такой машины больше не существует.", 3000);
                    Trigger.ClientEvent(player, "client.phone.cars.error");
                    return;
                }*/
                //if ((vehicle1 != null && vehicleLocalData1.Access != VehicleAccess.Garage) || (vehicle2 != null && vehicleLocalData2.Access != VehicleAccess.Garage))
    
                    
                if ((vehicle1 != null && vehicleLocalData1.Access != VehicleAccess.Garage) || (vehicle2 != null && vehicleLocalData2.Access != VehicleAccess.Garage))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TwoCarsInGarage), 3000);
                    Trigger.ClientEvent(player, "client.phone.cars.error");
                    return;
                }

                if (garage.CarSlots.ContainsKey(carSlotIndex) && garage.CarSlots[carSlotIndex] == vehicleData1.SqlId)
                    garage.CarSlots[carSlotIndex] = vehicleData2.SqlId;
                
                garage.CarSlots[index] = vehicleData1.SqlId;

                garage.DestroyCar(vehicleData1.Number);
                garage.DestroyCar(vehicleData2.Number);

                garage.SpawnCar(vehicleData1.Number);
                garage.SpawnCar(vehicleData2.Number);
            }
            else
            {
                var vehicleData = VehicleManager.GetVehicleToAutoId(autoId);
                if (vehicleData == null)
                {
                    Trigger.ClientEvent(player, "client.phone.cars.error");
                    return;
                }

                var vehicle = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Personal, vehicleData.Number);
                var localData = vehicle.GetVehicleLocalData();
                /*if (vehicle1 == null || vehicleLocalData1 == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Такой машины больше не существует.", 3000);
                    Trigger.ClientEvent(player, "client.phone.cars.error");
                    return;
                }*/
                if (vehicle != null && localData?.Access != VehicleAccess.Garage)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarMustInGarage), 3000);
                    Trigger.ClientEvent(player, "client.phone.cars.error");
                    return;
                }
                
                if (garage.CarSlots.ContainsKey(carSlotIndex) && garage.CarSlots[carSlotIndex] == vehicleData.SqlId)
                    garage.CarSlots.TryRemove(carSlotIndex, out _);
                
                garage.CarSlots[index] = vehicleData.SqlId;
                garage.DestroyCar(vehicleData.Number);
                garage.SpawnCar(vehicleData.Number);
            }

            var carsOwnerData = Players.Phone.Cars.Repository.GetCarToHouse(player, house?.Owner, house?.GetVehiclesCarNumber(), garage, false);
            Trigger.ClientEvent(player, "client.phone.cars.init", JsonConvert.SerializeObject(carsOwnerData), true, house.Owner == sessionData.Name);
            
            //Trigger.ClientEvent(player, "client.parking.confirm", autoId, index);
            garage.IsSave = true;
            
            
            BattlePass.Repository.UpdateReward(player, 83);
        }

    }
}
