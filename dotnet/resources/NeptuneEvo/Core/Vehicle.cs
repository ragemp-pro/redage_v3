using GTANetworkAPI;
using NeptuneEvo.Handles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using NeptuneEvo.GUI;
using Newtonsoft.Json;
using System.Linq;
using Redage.SDK;
using NeptuneEvo.Fractions;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Chars;
using System.Threading.Tasks;
using Database;
using LinqToDB;
using System.Collections.Concurrent;
using Localization;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.Players.Phone.Messages.Models;
using NeptuneEvo.Players.Popup.List.Models;
using NeptuneEvo.Quests;
using NeptuneEvo.Table.Models;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;
using NeptuneEvo.VehicleData.Models;

namespace NeptuneEvo.Core
{
    class VehicleManager : Script
    {
        private static readonly nLog Log = new nLog("Core.Vehicle");
        private static Random Rnd = new Random();

        private ExtVehicle CasinoVeh = VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey("Bugatti"), new Vector3(1099.867, 220.1293, -48.076587), 245.9566f, 121, 13, "ROULETTE", locked: true, petrol: 0);

        public static ConcurrentDictionary<string, VehicleData.Models.VehicleData> Vehicles = new ConcurrentDictionary<string, VehicleData.Models.VehicleData>();
        public static List<string> VehicleNumbers = new List<string>();


        public static void AddVehicleNumber(string number)
        {
            if (!VehicleNumbers.Contains(number))
                VehicleNumbers.Add(number);
        }
        
        public static void RemoveVehicleNumber(string number)
        {
            if (VehicleNumbers.Contains(number))
                VehicleNumbers.Remove(number);
        }

        
        public static ConcurrentDictionary<int, string> VehiclesSqlIdToNumber = new ConcurrentDictionary<int, string>();
        public static IReadOnlyDictionary<int, int> VehicleTank = new Dictionary<int, int>()
        {
            { -1, 100 },
            { 0, 120 }, // compacts
            { 1, 150 }, // Sedans
            { 2, 200 }, // SUVs
            { 3, 100 }, // Coupes
            { 4, 130 }, // Muscle
            { 5, 150 }, // Sports
            { 6, 100 }, // Sports (classic?)
            { 7, 150 }, // Super
            { 8, 100 }, // Motorcycles
            { 9, 200 }, // Off-Road
            { 10, 150 }, // Industrial
            { 11, 150 }, // Utility
            { 12, 150 }, // Vans
            { 13, 1   }, // cycles
            { 14, 300 }, // Boats
            { 15, 400 }, // Helicopters
            { 16, 500 }, // Planes
            { 17, 130 }, // Service
            { 18, 200 }, // Emergency
            { 19, 150 }, // Military
            { 20, 150 }, // Commercial
            // 21 trains
        };
        public static IReadOnlyDictionary<int, int> VehicleRepairPrice = new Dictionary<int, int>()
        {
            { -1, 10 }, // compacts
            { 0, 10 }, // compacts
            { 1, 10 }, // Sedans
            { 2, 10 }, // SUVs
            { 3, 10 }, // Coupes
            { 4, 10 }, // Muscle
            { 5, 10 }, // Sports
            { 6, 10 }, // Sports (classic?)
            { 7, 10 }, // Super
            { 8, 10 }, // Motorcycles
            { 9, 10 }, // Off-Road
            { 10, 10 }, // Industrial
            { 11, 10 }, // Utility
            { 12, 10 }, // Vans
            { 13, 10 }, // 13 cycles
            { 14, 10 }, // Boats
            { 15, 10 }, // Helicopters
            { 16, 10 }, // Planes
            { 17, 10 }, // Service
            { 18, 10 }, // Emergency
            { 19, 10 }, // Military
            { 20, 10 }, // Commercial
            // 21 trains
        };
        private static IReadOnlyDictionary<int, int> PetrolRate = new Dictionary<int, int>()
        {
            { -1, 0 },
            { 0, 1 }, // compacts
            { 1, 1 }, // Sedans
            { 2, 1 }, // SUVs
            { 3, 1 }, // Coupes
            { 4, 1 }, // Muscle
            { 5, 1 }, // Sports
            { 6, 1 }, // Sports (classic?)
            { 7, 1 }, // Super
            { 8, 1 }, // Motorcycles
            { 9, 1 }, // Off-Road
            { 10, 1 }, // Industrial
            { 11, 1 }, // Utility
            { 12, 1 }, // Vans
            { 13, 0 }, // Cycles
            { 14, 1 }, // Boats
            { 15, 1 }, // Helicopters
            { 16, 1 }, // Planes
            { 17, 1 }, // Service
            { 18, 1 }, // Emergency
            { 19, 1 }, // Military
            { 20, 1 }, // Commercial
            // 21 trains
        };
        public static void Init()
        {
            try
            {
                Timers.Start("fuel", 30000, () => FuelControl(), true);

                Log.Write("Loading Vehicles...");

                using var db = new ServerBD("MainDB");//При старте сервера

                var vehiclesData = db.Vehicles
                            .ToList();

                var countSwipe = 0;
                foreach (var vehicle in vehiclesData)
                {
                    var vehicleData = new VehicleData.Models.VehicleData
                    {
                        SqlId = vehicle.AutoId,
                        Number = vehicle.Number,
                        Holder = vehicle.Holder,
                        Model = vehicle.Model,
                        Health = vehicle.Health,
                        Fuel = vehicle.Fuel,
                        Components = JsonConvert.DeserializeObject<VehicleCustomization>(vehicle.Components),
                        Position = vehicle.Position,
                        Rotation = vehicle.Rotation,
                        KeyNum = vehicle.Keynum,
                        Dirt = vehicle.Dirt,
                        Tag = vehicle.Tag,
                    };

                    AddVehicleNumber(vehicle.Number);
                    Vehicles[vehicle.Number] = vehicleData;

                    VehiclesSqlIdToNumber[vehicle.AutoId] = vehicle.Number;


                    if (Chars.Repository.ItemsData.ContainsKey($"vehicle_{vehicle.Number}"))
                    {
                        if (Chars.Repository.ItemsData[$"vehicle_{vehicle.Number}"].ContainsKey("vehicle"))
                        {
                            var itemsData = Chars.Repository.ItemsData[$"vehicle_{vehicle.Number}"]["vehicle"].Values.ToList();

                            var locationName = $"vehicle_{vehicle.AutoId}";

                            if (!Chars.Repository.ItemsData.ContainsKey(locationName)) 
                                Chars.Repository.ItemsData.TryAdd(locationName, new ConcurrentDictionary<string, ConcurrentDictionary<int, InventoryItemData>>());

                            if (!Chars.Repository.ItemsData[locationName].ContainsKey("vehicle")) 
                                Chars.Repository.ItemsData[locationName].TryAdd("vehicle", new ConcurrentDictionary<int, InventoryItemData>());

                            var i = 0;
                            while (itemsData.Count > 0)
                            {
                                if (!Chars.Repository.ItemsData[locationName]["vehicle"].ContainsKey(i) || Chars.Repository.ItemsData[locationName]["vehicle"][i].ItemId == ItemId.Debug)
                                {
                                    var item = itemsData.FirstOrDefault();

                                    if (item != null)
                                    {
                                        Chars.Repository.ItemsData[locationName]["vehicle"][i] =
                                            new InventoryItemData(item.SqlId, item.ItemId, item.Count, item.Data, i);

                                        Chars.Repository.UpdateSqlItemData(locationName, "vehicle", i,
                                            Chars.Repository.ItemsData[locationName]["vehicle"][i]);

                                        itemsData.Remove(item);
                                        countSwipe++;
                                    }
                                }

                                i++;
                            }
                        }

                        Chars.Repository.ItemsData.TryRemove($"vehicle_{vehicle.Number}", out _);
                    }
                    
                    
                }

                Log.Write($"Vehicles are loaded ({vehiclesData.Count}) | Перенесли item - {countSwipe} шт.", nLog.Type.Success);
            }
            catch (Exception e)
            {
                Log.Write($"VehicleManager Exception: {e.ToString()}");
            }
        }
        
        private static void FuelControl()
        {
            try
            {
                var vehiclesLocalData = RAGE.Entities.Vehicles.All.Cast<ExtVehicle>()
                    .Where(v => v.VehicleLocalData != null)
                    .Where(v => VehicleStreaming.GetEngineState(v))
                    .Where(v => PetrolRate.ContainsKey(v.VehicleLocalData.Class))
                    .Where(v => v.VehicleLocalData.Petrol > 0)
                    .Where(v => v.VehicleLocalData.Number != -2)
                    .ToList();

                foreach (var vehicle in vehiclesLocalData)
                {
                    try 
                    {
                        var vehicleLocalData = vehicle.GetVehicleLocalData();
                        vehicleLocalData.Petrol -= PetrolRate[vehicleLocalData.Class];
                        if (vehicleLocalData.Petrol <= 0)
                        {
                            vehicleLocalData.Petrol = 0;
                            VehicleStreaming.SetEngineState(vehicle, false);
                        }
                        vehicle.SetSharedData("PETROL", vehicleLocalData.Petrol);
                        if (vehicleLocalData.Access == VehicleAccess.Personal)
                        {
                            string number = vehicleLocalData.NumberPlate;
                            var vehicleData = GetVehicleToNumber(number);
                            if (vehicleData != null) 
                                vehicleData.Fuel = vehicleLocalData.Petrol;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Write($"FuelControl foreach Exception: {e.ToString()}");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"FuelControl Exception: {e.ToString()}");
            }
        }

        public static void DeathControler()
        {
            try
            {
                var vehicles = RAGE.Entities.Vehicles.All.Cast<ExtVehicle>()
                    .Where(v => v.VehicleLocalData != null)
                    .Where(v => v.VehicleLocalData.IsDeath == false)
                    .Where(v => v.Health < 1f)
                    .ToList();

                foreach (var vehicle in vehicles)
                {
                    /*if (VehicleModel.AirAutoRoom.isAirCar(vehicle.Model))
                    {
                        Rentcar.Event_vehicleDeath(vehicle);
                        Army.onVehicleDeath(vehicle);
                        Event_vehicleDeath(vehicle);
                        continue;
                    }*/
                    var vehicleLocalData = vehicle.GetVehicleLocalData();
                    if (vehicleLocalData != null)
                    {
                        vehicleLocalData.IsDeath = true;
                        vehicleLocalData.DeathTime = Main.ServerNumber == 0 ? DateTime.Now : DateTime.Now.AddMinutes(10);
                        
                        var vehicleData = GetVehicleToNumber(vehicleLocalData.NumberPlate);
                        if (vehicleData != null) 
                            vehicleData.Health = 0;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"DeathControler Exception: {e.ToString()}");
            }
        }
        public static void VehiclesDestroy()
        {
            try
            {
                var vehicles = RAGE.Entities.Vehicles.All.Cast<ExtVehicle>()
                    .Where(v => v.VehicleLocalData != null)
                    .ToList();

                foreach (var vehicle in vehicles)
                {
                    if (VehicleStreaming.VehiclesDelele.Contains(vehicle))
                        continue;
                    
                    var vehicleLocalData = vehicle.GetVehicleLocalData();
                    if (vehicleLocalData != null)
                    {
                        if (vehicle.Health > 0 && vehicleLocalData.IsDeath)
                        {
                            vehicleLocalData.IsDeath = false;
                            continue;
                        }
                        if (vehicleLocalData.IsDeath && vehicleLocalData.DeathTime < DateTime.Now)
                        {
                            Rentcar.Event_vehicleDeath(vehicle);
                            Army.onVehicleDeath(vehicle);
                            Event_vehicleDeath(vehicle);
                            continue;
                        }
                        if ((vehicleLocalData.Access == VehicleAccess.Rent || vehicleLocalData.Access == VehicleAccess.Work) && vehicleLocalData.IsOwnerExit && vehicleLocalData.RentCarDeleteTime < DateTime.Now)
                        {
                            VehicleStreaming.DeleteVehicle(vehicle);
                            continue;
                        }

                        if (!vehicleLocalData.IsTicket && vehicleLocalData.ExitTime < DateTime.Now && vehicleLocalData.Occupants.Count == 0)
                            VehicleUpdateExitStatus(vehicle, true);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"DeathControler Exception: {e.ToString()}");
            }
        }

        private static void VehicleUpdateExitStatus(ExtVehicle vehicle, bool toggled)
        {
            
            var vehicleLocalData = vehicle.GetVehicleLocalData();
            if (vehicleLocalData != null)
            {
                if (vehicleLocalData.IsTicket != toggled)
                {
                    vehicleLocalData.IsTicket = toggled;
                    vehicle.SetSharedData("isTicket", toggled);
                }
            }
        }
        
        [ServerEvent(Event.PlayerEnterVehicleAttempt)]
        public void onPlayerEnterVehicleAttemptHandler(ExtPlayer player, ExtVehicle vehicle, sbyte seatid)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (vehicle == CasinoVeh || characterData.DemorganTime >= 1) Trigger.StopAnimation(player);
            }
            catch (Exception e)
            {
                Log.Write($"onPlayerEnterVehicleAttemptHandler Exception: {e.ToString()}");
            }
        }

        [ServerEvent(Event.PlayerEnterVehicle)]
        public void onPlayerEnterVehicleHandler(ExtPlayer player, ExtVehicle vehicle, sbyte seatid)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData == null)
                {
                    WarpPlayerOutOfVehicle(player);
                    //Main.RagempCheatDetected(player, -2);
                    return;
                }
                if (vehicle == CasinoVeh)
                {
                    player.setKick(LangFunc.GetText(LangType.Ru, DataName.KickVehNoUsing));
                    return;
                }
                if (characterData.DemorganTime >= 1) 
                    player.setKick(LangFunc.GetText(LangType.Ru, DataName.KickVehWarned));
                
   
                if (!vehicleLocalData.Occupants.Contains(player))
                {
                    switch (vehicle.Class)
                    {
                        case 0: //Compacts
                        case 1: //Sedans
                        case 3: //Coupes
                        case 4: //Muscle
                        case 5: //Sports Classics
                        case 6: //Sports
                        case 7: //Super
                        case 9: //Off-road
                        case 10: //Industrial
                        case 11: //Utility
                        case 14: //Boats
                        case 15: //Helicopters
                        case 20: //Commercial
                            if (vehicleLocalData.Occupants.Count >= 4)
                            {
                                WarpPlayerOutOfVehicle(player);
                                return;
                            }
                            break;
                        case 8: //Motorcycles
                            if (vehicleLocalData.Occupants.Count >= 2)
                            {
                                WarpPlayerOutOfVehicle(player);
                                return;
                            }
                            break;
                        case 13: //Cycles
                            if (vehicleLocalData.Occupants.Count >= 1)
                            {
                                WarpPlayerOutOfVehicle(player);
                                return;
                            }
                            break;
                        case 2: //SUVs
                        case 12: //Vans
                        case 18: //Emergency
                            if (vehicleLocalData.Occupants.Count >= 6)
                            {
                                WarpPlayerOutOfVehicle(player);
                                return;
                            }
                            break;
                    }
                    vehicleLocalData.Occupants.Add(player);
                    VehicleUpdateExitStatus(vehicle, false);
                }
                if (player.VehicleSeat == (int)VehicleSeat.Driver)
                {
                    if (!characterData.Achievements[18])
                    {
                        characterData.Achievements[18] = true;
                        
                        Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Helper, LangFunc.GetText(LangType.Ru, DataName.Podskazka1), DateTime.Now);
                        Players.Phone.Messages.Repository.AddSystemMessage(player, (int) DefaultNumber.Helper, LangFunc.GetText(LangType.Ru, DataName.Podskazka2), DateTime.Now);
                        Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Helper, LangFunc.GetText(LangType.Ru, DataName.Podskazka3), DateTime.Now);
                        
                        
                        //Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Podskazka1), 5000);
                        //Notify.SendToKey(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Podskazka2), 10000, 38);
                        //Notify.SendToKey(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Podskazka3), 10000, 11);

                    }
                    if (vehicleLocalData.Access == VehicleAccess.Fraction)
                    {
                        var memberFractionData = player.GetFractionMemberData();
                        var fracId = memberFractionData != null ? memberFractionData.Id : 0;
                        
                        if (vehicleLocalData.Fraction == (int)Fractions.Models.Fractions.ARMY && (vehicle.Model == (uint)VehicleHash.Barracks || vehicle.Model == (uint)VehicleHash.Brickade || vehicle.Model == (uint)VehicleHash.Cargobob))
                        {
                            if ((fracId >= (int)Fractions.Models.Fractions.FAMILY && fracId <= (int)Fractions.Models.Fractions.BLOOD) || (fracId >= (int)Fractions.Models.Fractions.LCN && fracId <= (int)Fractions.Models.Fractions.ARMENIAN) || fracId == (int)Fractions.Models.Fractions.THELOST)
                            {
                                if (DateTime.Now.Hour < 10)
                                {
                                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FractionCarFrom10), 3000);
                                    WarpPlayerOutOfVehicle(player);
                                    return;
                                }
                                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.EngineONB), 3000);
                                return;
                            }
                            else if (fracId == (int)Fractions.Models.Fractions.ARMY)
                            {
                                if (memberFractionData.Rank < vehicleLocalData.MinRank && characterData.AdminLVL <= 4 || !sessionData.WorkData.OnDuty && characterData.AdminLVL <= 4)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoAccessToVeh), 3000);
                                    WarpPlayerOutOfVehicle(player);
                                    return;
                                }
                                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.EngineONB), 3000);
                                return;
                            }
                            else WarpPlayerOutOfVehicle(player);
                        }
                        if (vehicleLocalData.Fraction == fracId)
                        {
                            if (memberFractionData.Rank < vehicleLocalData.MinRank && characterData.AdminLVL <= 4 || (fracId == (int) Fractions.Models.Fractions.EMS || fracId == (int) Fractions.Models.Fractions.FIB || fracId == (int) Fractions.Models.Fractions.CITY || fracId == (int) Fractions.Models.Fractions.LSNEWS || fracId == (int) Fractions.Models.Fractions.POLICE || fracId == (int) Fractions.Models.Fractions.SHERIFF || fracId == (int) Fractions.Models.Fractions.ARMY) && !sessionData.WorkData.OnDuty && characterData.AdminLVL <= 4)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoAccessToVeh), 3000);
                                WarpPlayerOutOfVehicle(player);
                                return;
                            }
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.EngineONB), 3000);
                        }
                        else if (characterData.AdminLVL <= 4)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoAccessToVeh), 3000);
                            WarpPlayerOutOfVehicle(player);
                            return;
                        }
                        else if (vehicleLocalData.Fraction != fracId && characterData.AdminLVL <= 4)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoAccessToVeh), 3000);
                            WarpPlayerOutOfVehicle(player);
                            return;
                        }
                    }
                    else if (vehicleLocalData.Access == VehicleAccess.Work && vehicleLocalData.WorkDriver == characterData.UUID) 
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.EngineONB), 3000);

                    if (vehicle.Model == (uint)VehicleHash.Flatbed && !sessionData.IsTicketRender && player.IsFractionAccess(RankToAccess.VehicleTicket, false))
                    {
                        sessionData.IsTicketRender = true;
                        Trigger.ClientEvent(player, "client.ticket.addRender");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"onPlayerEnterVehicleHandler Exception: {e.ToString()}");
            }
        }

        [ServerEvent(Event.PlayerExitVehicleAttempt)]
        public void OnPlayerExitVehicleHandler(ExtPlayer player, ExtVehicle vehicle)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if (player.VehicleSeat == (int)VehicleSeat.Driver)
                        vehicleLocalData.ExitTime = DateTime.Now.AddMinutes(10);
                    
                    if (vehicleLocalData.Occupants.Contains(player))
                        vehicleLocalData.Occupants.Remove(player);
                    
                    if (/*vehicle.Model == (uint)VehicleHash.Flatbed && */ sessionData.IsTicketRender)
                    {
                        sessionData.IsTicketRender = false;
                        Trigger.ClientEvent(player, "client.ticket.removeRender");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"onPlayerExitVehicleHandler Exception: {e.ToString()}");
            }
        }

        [ServerEvent(Event.PlayerExitVehicle)]
        public void Event_OnPlayerExitVehicle(ExtPlayer player, ExtVehicle vehicle)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    if (player.VehicleSeat == (int)VehicleSeat.Driver)
                        vehicleLocalData.ExitTime = DateTime.Now.AddMinutes(10);
                    
                    if (vehicleLocalData.Occupants.Contains(player))
                        vehicleLocalData.Occupants.Remove(player);
                    
                    if (/*vehicle.Model == (uint)VehicleHash.Flatbed && */ sessionData.IsTicketRender)
                    {
                        sessionData.IsTicketRender = false;
                        Trigger.ClientEvent(player, "client.ticket.removeRender");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"Event_OnPlayerExitVehicle Exception: {e.ToString()}");
            }
        }
        private static void CheckVehicleState(ExtPlayer player, int wasinveh)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                sessionData.TimersData.CheckInVeh = null;
                if (characterData.IsAlive && player.IsInVehicle)
                {
                    var vehicle = (ExtVehicle) player.Vehicle;
                    if (vehicle != null && wasinveh != -1 && wasinveh == vehicle.Value)
                    {
                        WeaponRepository.PlayerKickAntiCheat(player, 2, false);
                        return;
                    }
                }
                Trigger.ClientEvent(player, "VehicleEnterToggle", true);
            }
            catch (Exception e)
            {
                Log.Write($"CheckVehicleState Exception: {e.ToString()}");
            }
        }
        /// <summary>
        /// Выкинуть игрока из машины
        /// </summary>
        /// <param name="player"></param>
        /// <param name="withtimer"></param>
        public static void WarpPlayerOutOfVehicle(ExtPlayer player, bool withtimer = true)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (player.IsInVehicle)
                {
                    var vehicle = (ExtVehicle) player.Vehicle;
                    if (vehicle == null) return;
                    var vehicleLocalData = vehicle.GetVehicleLocalData();
                    if (vehicleLocalData != null && vehicleLocalData.Occupants.Contains(player)) vehicleLocalData.Occupants.Remove(player);
                    if (withtimer && characterData.IsAlive)
                    {
                        if (sessionData.TimersData.CheckInVeh != null) Timers.Stop(sessionData.TimersData.CheckInVeh);
                        Trigger.ClientEvent(player, "VehicleEnterToggle", false);
                        sessionData.TimersData.CheckInVeh = Timers.StartOnce(2500, () => CheckVehicleState(player, vehicle.Value), true);
                    }
                    if (sessionData.IsConnect) 
                        player.WarpOutOfVehicle();
                }
            }
            catch (Exception e)
            {
                Log.Write($"WarpPlayerOutOfVehicle Exception: {e.ToString()}");
            }
        }
        /// <summary>
        /// Починка авто
        /// </summary>
        /// <param name="vehicle"></param>
        public static void RepairCar(ExtVehicle vehicle)
        {
            try
            {
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null) 
                    vehicle.Repair();
            }
            catch (Exception e)
            {
                Log.Write($"RepairCar Exception: {e.ToString()}");
            }
        }
        public static void Create(ExtPlayer player, string Model, Color Color1, Color Color2, int Health = 1000, int Fuel = 100, string Text = "", string Logs = "")
        {
            if (!player.IsCharacterData()) return;
            
            Trigger.SetTask(() =>
            {
                CreateThread(player, Model, Color1, Color2, Health, Fuel, Text, Logs);
            });
        }
        /// <summary>
        /// Создание авто
        /// </summary>
        /// <param name="Holder"></param>
        /// <param name="Model"></param>
        /// <param name="Color1"></param>
        /// <param name="Color2"></param>
        /// <param name="Health"></param>
        /// <param name="Fuel"></param>
        /// <param name="Price"></param>
        /// <returns></returns>
        public static async void CreateThread(ExtPlayer player, string Model, Color Color1, Color Color2, int Health = 1000, int Fuel = 100, string Text = "", string Logs = "")
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                int uuid = characterData.UUID;
                string playerName = sessionData.Name;

                var vehicleData = new VehicleData.Models.VehicleData
                {
                    Holder = playerName,
                    Model = Model,
                    Health = Health,
                    Fuel = Fuel,
                    Components = new VehicleCustomization()
                    {
                        PrimColor = Color1,
                        SecColor = Color2
                    },
                    Dirt = 0.0F,
                    Tag = "null"
                };
                string number = GenerateNumber();

                await using var db = new ServerBD("MainDB");//В отдельном потоке

                int itemSqlID = await db.InsertWithInt32IdentityAsync(new Vehicles
                {
                    Number = number,
                    Holder = playerName,
                    Model = Model,
                    Health = Health,
                    Fuel = Fuel,
                    Components = "{}",
                    Position = "",
                    Rotation = "",
                    Keynum = 0,
                    Dirt = 0f,
                    Tag = "null"
                });

                vehicleData.SqlId = itemSqlID;
                vehicleData.Number = number;
                Vehicles.TryAdd(number, vehicleData);
                AddVehicleNumber(number);
                VehiclesSqlIdToNumber[itemSqlID] = number;

                if (Logs.Length > 1)
                    GameLog.Money($"system", $"player({uuid})", 1, $"{Logs}, {number}, #{itemSqlID})");

                if (!player.IsCharacterData())
                {
                    Chars.Repository.AddNewItem(null, $"char_{uuid}", "inventory", ItemId.CarKey, 1, $"{itemSqlID}_0");
                }
                else
                {
                    if (Text.Length > 1)
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"{Text} {number}", 3000);

                    Chars.Repository.AddNewItem(player, $"char_{uuid}", "inventory", ItemId.CarKey, 1, $"{itemSqlID}_0");

                    //Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, $"Вы купили транспортное средство. Вызвать его можно в личном гараже, предварительно выбрав слот.", 5000);

                    if (!VehicleModel.AirAutoRoom.isAirCar(Model))
                    {
                        NAPI.Task.Run(() =>
                        {
                            if (!player.IsCharacterData()) return;
                            //
                            qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int) zdobich_quests.Stage26, 1,
                                isUpdateHud: true);
                            qMain.UpdateQuestsComplete(player, Zdobich.QuestName, (int) zdobich_quests.Stage26, true);
                            //
                            var house = Houses.HouseManager.GetHouse(player, true);
                            if (house != null && Houses.GarageManager.Garages.ContainsKey(house.GarageID))
                            {
                                var garage = house.GetGarageData();
                                if (garage != null)
                                {
                                    if (garage.Type != -1 && garage.Type != 6)
                                        garage.SpawnCar(number);
                                    else
                                        garage.GetVehicleFromGarage(number);
                                }
                                
                                EventSys.SendCoolMsg(player,"Транспорт", "Покупка транспорта", $"{LangFunc.GetText(LangType.Ru, DataName.VehSoonGarage)}", "", 12000);
                                //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehSoonGarage), 5000);
                                //Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Bank, LangFunc.GetText(LangType.Ru, DataName.VehSoonGarage), DateTime.Now);
                            }
                            else
                                EventSys.SendCoolMsg(player,"Транспорт", "Покупка транспорта", $"{LangFunc.GetText(LangType.Ru, DataName.VehWhenHome)}", "", 12000);
                                    // Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehWhenHome), 5000);
                            //Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Bank, LangFunc.GetText(LangType.Ru, DataName.VehWhenHome), DateTime.Now);

                        });
                    }
                }

                Log.Debug("Created new vehicle with number: " + number);
            }
            catch (Exception e)
            {
                Log.Write($"Create Task Exception: {e.ToString()}");
            }
        }
        public static void Remove(string number)
        {
            try
            {
                var vehicleData = GetVehicleToNumber(number);
                if (vehicleData == null) return;
                if (!VehicleModel.AirAutoRoom.isAirCar(vehicleData.Model))
                {
                    var house = Houses.HouseManager.GetHouse(vehicleData.Holder);
                    if (house != null)
                    {
                        var garage = house.GetGarageData();
                        if (garage != null)
                            garage.DeleteCar(number, isRevive: true);
                    }
                }
                else
                {
                    var vehiclePlayer = NeptuneEvo.VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Personal, number);
                    if (vehiclePlayer != null)
                        VehicleStreaming.DeleteVehicle(vehiclePlayer);
                }

                if (VehiclesSqlIdToNumber.ContainsKey(vehicleData.SqlId)) 
                    VehiclesSqlIdToNumber.TryRemove(vehicleData.SqlId, out _);

                Vehicles.TryRemove(number, out _);
                RemoveVehicleNumber(number);
                
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.Vehicles
                            .Where(v => v.AutoId == vehicleData.SqlId)
                            .DeleteAsync();
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
                Chars.Repository.RemoveAll(GetVehicleToInventory(number));
            }
            catch (Exception e)
            {
                Log.Write($"Remove Exception: {e.ToString()}");
            }
        }
        public static string GetVehicleToInventory(string number)
        {            
            var vehicleData = GetVehicleToNumber(number);
            
            if (vehicleData != null) 
                return $"vehicle_{vehicleData.SqlId}";
            
            return $"vehicle_{number}";
        }
        public static async Task SaveSql(ServerBD db, string number)
        {
            try
            {
                var vehicleData = GetVehicleToNumber(number);
                if (vehicleData == null) return;

                await db.Vehicles
                    .Where(v => v.AutoId == vehicleData.SqlId)
                    .Set(v => v.Number, number)
                    .Set(v => v.Holder, vehicleData.Holder)
                    .Set(v => v.Model, vehicleData.Model)
                    .Set(v => v.Health, vehicleData.Health)
                    .Set(v => v.Fuel, vehicleData.Fuel)
                    .Set(v => v.Components, JsonConvert.SerializeObject(vehicleData.Components))
                    .Set(v => v.Position, vehicleData.Position == null ? "" : vehicleData.Position)
                    .Set(v => v.Rotation, vehicleData.Rotation == null ? "" : vehicleData.Rotation)
                    .Set(v => v.Keynum, vehicleData.KeyNum)
                    .Set(v => v.Dirt, vehicleData.Dirt)
                    .Set(v => v.Tag, vehicleData.Tag)
                    .UpdateAsync();
            }
            catch (Exception e)
            {
                Log.Write($"SaveSql Exception: {e.ToString()}");
            }
        }
        public static void SaveNumber(string number)
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    var vehicleData = GetVehicleToNumber(number);
                    if (vehicleData == null) 
                        return;
                
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    await db.Vehicles
                        .Where(v => v.AutoId == vehicleData.SqlId)
                        .Set(v => v.Number, vehicleData.Number)
                        .UpdateAsync();
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
        }
        public static void SaveHolder(string number)
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    var vehicleData = GetVehicleToNumber(number);
                    if (vehicleData == null) 
                        return;
                
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    await db.Vehicles
                        .Where(v => v.AutoId == vehicleData.SqlId)
                        .Set(v => v.Holder, vehicleData.Holder) 
                        .UpdateAsync();
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static ExtVehicle getNearestVehicle(ExtPlayer player, int radius, uint model = 0)
        {
            try
            {
                if (!player.IsCharacterData()) return null;
                var vehicles = RAGE.Entities.Vehicles.All.Cast<ExtVehicle>();
                ExtVehicle nearestVehicle = null;
                foreach (var vehicle in vehicles)
                {
                    if (UpdateData.GetVehicleDimension(vehicle) != UpdateData.GetPlayerDimension(player)) 
                        continue;
                    if (model != 0 && model != vehicle.Model)
                        continue;
                    if (nearestVehicle == null && player.Position.DistanceTo(vehicle.Position) <= radius)
                    {
                        nearestVehicle = vehicle;
                        continue;
                    }
                    if (nearestVehicle != null)
                    {
                        if (player.Position.DistanceTo(vehicle.Position) < player.Position.DistanceTo(nearestVehicle.Position))
                        {
                            nearestVehicle = vehicle;
                        }
                    }
                }
                return nearestVehicle;
            }
            catch (Exception e)
            {
                Log.Write($"getNearestVehicle Exception: {e.ToString()}");
                return null;
            }
        }
        public static bool IsVehicleToNumber(string number)
        {
            return Vehicles.ContainsKey(number);
        }
        public static VehicleData.Models.VehicleData GetVehicleToAutoId(int autoId)
        {
            if (VehiclesSqlIdToNumber.ContainsKey(autoId))
                return GetVehicleToNumber (VehiclesSqlIdToNumber [autoId]);
            return null;
        }
        public static VehicleData.Models.VehicleData GetVehicleToNumber(string number)
        {
            if (IsVehicleToNumber(number))
                return Vehicles[number];
            
            return null;
        }
        //
        public static List<string> GetVehiclesCarNumberToPlayer(string name)
        {
            if (name == string.Empty)
                return new List<string>();
            
            return Vehicles
                .Where(v => v.Value.Holder == name)
                .Where(v => !VehicleModel.AirAutoRoom.isAirCar(v.Value.Model))
                .Select(d => d.Key)
                .ToList();
        }
        public static List<string> GetVehiclesCarNumberToPlayer(List<string> names)
        {
            if (names.Count == 0)
                return new List<string>();
            
            return Vehicles
                .Where(v => names.Contains(v.Value.Holder))
                .Where(v => !VehicleModel.AirAutoRoom.isAirCar(v.Value.Model))
                .Select(d => d.Key)
                .ToList();
        }
        public static int GetVehiclesCarCountToPlayer(string playerName)
        {
            return Vehicles
                .Where(v => v.Value.Holder == playerName)
                .Where(v => !VehicleModel.AirAutoRoom.isAirCar(v.Value.Model))
                .Select(d => d.Key)
                .ToList()
                .Count();
        }
        //
        public static int GetVehiclesAirCountToPlayer(string playerName)
        {
            return Vehicles
                .Where(v => v.Value.Holder == playerName)
                .Where(v => VehicleModel.AirAutoRoom.isAirCar(v.Value.Model))
                .Select(d => d.Key)
                .ToList()
                .Count();
        }
        public static List<string> GetVehiclesAirNumberToPlayer(string name)
        {
            if (name == string.Empty)
                return new List<string>();
            
            return Vehicles
                .Where(v => v.Value.Holder == name)
                .Where(v => VehicleModel.AirAutoRoom.isAirCar(v.Value.Model))
                .Select(d => d.Key)
                .ToList();
        }
        public static List<string> GetVehiclesAirNumberToPlayer(List<string> names)
        {
            if (names.Count == 0)
                return new List<string>();
            
            return Vehicles
                .Where(v => names.Contains(v.Value.Holder))
                .Where(v => VehicleModel.AirAutoRoom.isAirCar(v.Value.Model))
                .Select(d => d.Key)
                .ToList();
        }
        //
        public static List<string> GetVehiclesCarAndAirNumberToPlayer(string name)
        {
            if (name == string.Empty)
                return new List<string>();
            
            return Vehicles
                .Where(v => v.Value.Holder == name)
                .Select(d => d.Key)
                .ToList();
        }
        public static List<string> GetVehiclesCarAndAirNumberToPlayer(List<string> names)
        {
            if (names.Count == 0)
                return new List<string>();
            
            return Vehicles
                .Where(v => names.Contains(v.Value.Holder))
                .Select(d => d.Key)
                .ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        public static void sellCar(ExtPlayer player, ExtPlayer target)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (!target.IsCharacterData()) return;
                SellItemData sellItemData = sessionData.SellItemData;
                if ((sellItemData.Buyer != null || sellItemData.Seller != null) && Chars.Repository.TradeGet(player))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantTrade), 3000);
                    return;
                }
                if (Main.IHaveDemorgan(player, true)) return;
                sellItemData.Buyer = target;
                sellItemData.Seller = player;
                OpenSellCarMenu(player);
            }
            catch (Exception e)
            {
                Log.Write($"sellCar Exception: {e.ToString()}");
            }
        }

        #region Selling Menu
        public static void OpenSellCarMenu(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                
                var vehiclesNumber = GetVehiclesCarAndAirNumberToPlayer(player.Name);
                if (vehiclesNumber.Count == 0)
                {
                    sessionData.SellItemData = new SellItemData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoCar), 3000);
                    return;
                }
                
                var frameList = new FrameListData();  
                frameList.Header = LangFunc.GetText(LangType.Ru, DataName.sellcar); 
                frameList.Callback = callback_sellcar; 
                
                foreach (string number in vehiclesNumber)
                {
                    var vehicleData = GetVehicleToNumber(number);
                    if (vehicleData == null) continue;
                    frameList.List.Add(new ListData( vehicleData.Model + " - " + number, number));  /// НЕ УВЕРЕН
                    
                    /*menuItem = new Menu.Item(number, Menu.MenuItem.Button);
                    menuItem.Text = vehicleData.Model + " - " + number;
                    menu.Add(menuItem);*/
                }

                Players.Popup.List.Repository.Open(player, frameList);
            }
            catch (Exception e)
            {
                Log.Write($"OpenSellCarMenu Exception: {e.ToString()}");
            }
        }

        private static void callback_sellcar(ExtPlayer player , object listItem) /// Никитос Чини  
        {
            try
            {
                if (!(listItem is string))
                    return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;

                var number = (string) listItem;
                
                var vehicleData = GetVehicleToNumber(number);
                if (vehicleData == null) return;
                if (Ticket.IsVehicleTickets(vehicleData.SqlId))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehOnShtrafSell, vehicleData.Model, vehicleData.Number), 3000);
                    return;
                }
                
                SellItemData sellItemData = sessionData.SellItemData;
                sellItemData.Seller = player;
                sellItemData.Number = number;
                Trigger.ClientEvent(player, "openInput", LangFunc.GetText(LangType.Ru, DataName.ProdazhaVeh), LangFunc.GetText(LangType.Ru, DataName.VvediteCenu), 8, "sellcar");
            }
            catch (Exception e)
            {
                Log.Write($"callback_sellcar Exception: {e.ToString()}");
            }
        }
        #endregion

        public static void FracApplyCustomization(ExtVehicle veh, int fractionId)
        {
            try
            {
                if (veh != null)
                {	    
                    var fractionData = Manager.GetFractionData(fractionId);
                    if (fractionData == null)
                        return;
                    
                    if (!fractionData.Vehicles.ContainsKey(veh.NumberPlate)) 
                        return;
                    
                    SetCustomization(veh, fractionData.Vehicles[veh.NumberPlate].customization);
                }
            }
            catch (Exception e)
            {
                Log.Write($"FracApplyCustomization Exception: {e.ToString()}");
            }
        }

        public static void OrgApplyCustomization(ExtVehicle veh, VehicleCustomization data)
        {
            try
            {
                if (veh != null)
                    SetCustomization(veh, data);
            }
            catch (Exception e)
            {
                Log.Write($"OrgApplyCustomization Exception: {e.ToString()}");
            }
        }

        private static List<uint> VehicleToWeapon = new List<uint>()
        {
            NAPI.Util.GetHashKey("Brutus"),
            NAPI.Util.GetHashKey("Imperator"),
            NAPI.Util.GetHashKey("ZR380"),
            NAPI.Util.GetHashKey("Deathbike"),
            NAPI.Util.GetHashKey("Comet4"),
            NAPI.Util.GetHashKey("Savestra"),
            NAPI.Util.GetHashKey("Viseris"),
            NAPI.Util.GetHashKey("Revolter"),
            NAPI.Util.GetHashKey("Speedo4"),
            NAPI.Util.GetHashKey("Mule4"),
            NAPI.Util.GetHashKey("Pounder2"),
            NAPI.Util.GetHashKey("Issi4"),
            NAPI.Util.GetHashKey("Pounder"),
        };

        //[RemoteEvent("server.GetVehicleCustomization")]
        public static void GetVehicleCustomization(ExtPlayer player, ExtVehicle vehicle, float range = 250.0f)
        {
            /*try
            {
                if (player != null && !player.IsCharacterData()) return;
                else if (vehicle == null) return;
                    var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    VehicleCustomization VehicleCustom = null;
                    switch (data.Access)
                    {
                        case "GARAGE":
                        case "PERSONAL":
                            if (Vehicles.ContainsKey(vehicle.NumberPlate))
                            {
                                VehicleCustom = Vehicles[vehicle.NumberPlate].Components;
                            }
                            break;
                        case "ORGANIZATION":
                            if (Organizations.Manager.Vehicles.ContainsKey(data.Fraction) && Organizations.Manager.Vehicles[data.Fraction].ContainsKey(vehicle.NumberPlate))
                            {
                                VehicleCustom = Organizations.Manager.Vehicles[data.Fraction][vehicle.NumberPlate].customization;
                            }
                            break;
                        case "FRACTION":
                            if (Configs.FractionVehicles.ContainsKey(data.Fraction) && Configs.FractionVehicles[data.Fraction].ContainsKey(vehicle.NumberPlate))
                            {
                                VehicleCustom = Configs.FractionVehicles[data.Fraction][vehicle.NumberPlate].customization;                            
                            }
                            break;
                    }
                    NAPI.Vehicle.SetVehicleNumberPlate(vehicle, vehicle.NumberPlate);
                    Dictionary<string, object> dataShared = new Dictionary<string, object>();
                    if (VehicleCustom != null)
                    {
                        if (VehicleCustom.NeonColor.Alpha != 0 || VehicleCustom.NeonIndex >= 0)
                        {
                            if (VehicleCustom.NeonIndex == -1)
                            {
                                VehicleCustom.NeonIndex = 7;
                            }
                            dataShared.Add("NeonIndex", VehicleCustom.NeonIndex);
                            dataShared.Add("NeonColor", VehicleCustom.NeonColor);
                            //if (data.NeonIndex >= 0) NAPI.Vehicle.SetVehicleNeonState(veh, true);
                        }
                        dataShared.Add("Spoiler", VehicleCustom.Spoiler);
                        dataShared.Add("FrontBumper", VehicleCustom.FrontBumper);
                        dataShared.Add("RearBumper", VehicleCustom.RearBumper);
                        dataShared.Add("SideSkirt", VehicleCustom.SideSkirt);
                        dataShared.Add("Muffler", VehicleCustom.Muffler);
                        dataShared.Add("Frame", VehicleCustom.Frame);
                        dataShared.Add("Hood", VehicleCustom.Hood);
                        dataShared.Add("Wings", VehicleCustom.Wings);
                        if (Vehicles.ContainsKey(vehicle.NumberPlate) && Vehicles[vehicle.NumberPlate].Model.Equals("MazdaRX7")) dataShared.Add("RWings", VehicleCustom.Wings);
                        if (!VehicleToWeapon.Contains(vehicle.Model)) dataShared.Add("Roof", VehicleCustom.Roof);
                        dataShared.Add("Vinyls", VehicleCustom.Vinyls);

                        dataShared.Add("Engine", VehicleCustom.Engine);
                        dataShared.Add("Turbo", VehicleCustom.Turbo);
                        dataShared.Add("Transmission", VehicleCustom.Transmission);
                        dataShared.Add("Suspension", VehicleCustom.Suspension);
                        dataShared.Add("Brakes", VehicleCustom.Brakes);
                        dataShared.Add("Horn", VehicleCustom.Horn);

                        dataShared.Add("WindowTint", VehicleCustom.WindowTint);
                        dataShared.Add("NumberPlate", VehicleCustom.NumberPlate);

                        if (VehicleCustom.Headlights >= 0) dataShared.Add("Headlights", VehicleCustom.Headlights);

                        if (VehicleCustom.PrimModColor == -1 && VehicleCustom.SecModColor == -1)
                        {
                            dataShared.Add("CPrimCol", VehicleCustom.PrimColor);
                            dataShared.Add("CSecCol", VehicleCustom.SecColor);
                        }
                        else
                        {
                            if (VehicleCustom.PrimModColor != -1) dataShared.Add("PrimCol", VehicleCustom.PrimModColor);
                            if (VehicleCustom.SecModColor != -1) dataShared.Add("SecCol", VehicleCustom.SecModColor);
                        }
                        dataShared.Add("WheelType", VehicleCustom.WheelsType);
                        dataShared.Add("Wheels", VehicleCustom.Wheels);
                        dataShared.Add("WheelsColor", VehicleCustom.WheelsColor);

                        dataShared.Add("ColorAdditional", VehicleCustom.ColorAdditional);
                        dataShared.Add("Cover", VehicleCustom.Cover);
                        dataShared.Add("CoverColor", VehicleCustom.CoverColor);

                        if (player != null) Trigger.ClientEvent(player, "client.SetVehicleCustomization", vehicle, JsonConvert.SerializeObject(dataShared));
                        else Trigger.ClientEventInRange(vehicle.Position, range, "client.SetVehicleCustomization", vehicle, JsonConvert.SerializeObject(dataShared));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"SetVehicleDirt Exception: {e.ToString()}");
            }*/
        }


        public static void SetCustomization(ExtVehicle veh, VehicleCustomization data)
        {
            try
            {
                if (veh != null)
                {
                    var vehicleData = GetVehicleToNumber(veh.NumberPlate);

                    if (!VehicleModel.AirAutoRoom.isAirCar(veh.Model))
                    {
                        if (data.NeonColor.Alpha != 0 || data.NeonIndex >= 0)
                        {
                            if (data.NeonIndex == -1) data.NeonIndex = 7;
                            if (data.NeonIndex > 0)
                                veh.SetSharedData("vNeon",
                                    $"{data.NeonIndex}|{data.NeonColor.Red}|{data.NeonColor.Green}|{data.NeonColor.Blue}");
                            //NAPI.Vehicle.SetVehicleNeonColor(veh, data.NeonColor.Red, data.NeonColor.Green, data.NeonColor.Blue);
                            //dataShared.Add("NeonColor", data.NeonColor);
                            //if (data.NeonIndex >= 0) NAPI.Vehicle.SetVehicleNeonState(veh, true);
                        }

                        veh.SetMod(0, data.Spoiler);
                        veh.SetMod(1, data.FrontBumper);
                        veh.SetMod(2, data.RearBumper);
                        veh.SetMod(3, data.SideSkirt);
                        veh.SetMod(4, data.Muffler);
                        veh.SetMod(5, data.Frame);
                        veh.SetMod(6, data.Lattice);
                        veh.SetMod(7, data.Hood);
                        veh.SetMod(8, data.Wings);
                        if (vehicleData != null && vehicleData.Model.Equals("MazdaRX7")) veh.SetMod(9, data.Wings);
                        if (!VehicleToWeapon.Contains(veh.Model)) veh.SetMod(10, data.Roof);
                        veh.SetMod(48, data.Vinyls);

                        veh.SetMod(11, data.Engine);
                        veh.SetMod(18, data.Turbo);
                        veh.SetMod(13, data.Transmission);
                        veh.SetMod(15, data.Suspension);
                        veh.SetMod(12, data.Brakes);
                        veh.SetMod(14, data.Horn);

                        veh.WindowTint = data.WindowTint;
                        veh.NumberPlateStyle = data.NumberPlate;

                        veh.SetSharedData("vHeadlights", data.Headlights);

                        veh.WheelType = data.WheelsType;
                        veh.SetMod(23, data.Wheels);
                        //veh.WheelColor = data.WheelsColor;

                        veh.SetSharedData("vCover", data.Cover);

                        veh.SetSharedData("vExtraColours", $"{data.ColorAdditional}|{data.WheelsColor}");
                    }

                    if (data.PrimModColor != -1) veh.PrimaryColor = data.PrimModColor;
                    else veh.CustomPrimaryColor = data.PrimColor;

                    if (data.SecModColor != -1) veh.SecondaryColor = data.SecModColor;
                    else veh.CustomSecondaryColor = data.SecColor;
                    
                    if (vehicleData != null)
                        VehicleStreaming.SetVehicleDirt(veh, vehicleData.Dirt);
                }
            }
            catch (Exception e)
            {
                Log.Write($"SetCustomization Exception: {e.ToString()}");
            }
        }

        public static void ApplyCustomization(ExtVehicle veh)
        {
            try
            {
                var vehicleData = GetVehicleToNumber(veh.NumberPlate);
                if (vehicleData == null) return;
                SetCustomization(veh, vehicleData.Components);
            }
            catch (Exception e)
            {
                Log.Write($"ApplyCustomization Exception: {e.ToString()}");
            }
        }
        
        public static void ChangeVehicleDoors(ExtPlayer player, ExtVehicle vehicle)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (vehicle == null) return;
                if (sessionData.CuffedData.Cuffed || sessionData.DeathData.InDeath) return;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    switch (vehicleLocalData.Access)
                    {
                        case VehicleAccess.Hotel:
                            if (vehicleLocalData.Owner != player && characterData.AdminLVL < 3)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoKeysFromVeh), 3000);
                                return;
                            }
                            if (VehicleStreaming.GetLockState(vehicle))
                            {
                                VehicleStreaming.SetLockStatus(vehicle, false);
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehOpenDoor), 3000);
                            }
                            else
                            {
                                VehicleStreaming.SetLockStatus(vehicle, true);
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehCloseDoor), 3000);
                            }
                            break;
                        case VehicleAccess.Rent:
                            if (vehicleLocalData.WorkDriver != characterData.UUID && characterData.AdminLVL < 3)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoKeysFromVeh), 3000);
                                return;
                            }
                            if (VehicleStreaming.GetLockState(vehicle))
                            {
                                VehicleStreaming.SetLockStatus(vehicle, false);
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehOpenDoor), 3000);
                            }
                            else
                            {
                                VehicleStreaming.SetLockStatus(vehicle, true);
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehCloseDoor), 3000);
                            }
                            break;
                        case VehicleAccess.Work:
                            if (vehicleLocalData.WorkDriver != characterData.UUID && characterData.AdminLVL < 3)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoKeysFromVeh), 3000);
                                return;
                            }
                            if (VehicleStreaming.GetLockState(vehicle))
                            {
                                VehicleStreaming.SetLockStatus(vehicle, false);
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehOpenDoor), 3000);
                            }
                            else
                            {
                                VehicleStreaming.SetLockStatus(vehicle, true);
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehCloseDoor), 3000);
                            }
                            break;
                        case VehicleAccess.OrganizationGarage:
                        case VehicleAccess.Organization:
                            if(characterData.AdminLVL < 3)
                            {
                                var memberOrganizationData = player.GetOrganizationMemberData();
                                if (memberOrganizationData == null) 
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoKeysFromVeh), 3000);
                                    return;
                                }
                                if (memberOrganizationData.Id != vehicleLocalData.Fraction)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoKeysFromVeh), 3000);
                                    return;
                                }
                                if (vehicleLocalData.MinRank > memberOrganizationData.Rank)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoAccessToVeh), 3000);
                                    return;
                                }
                            }
                            if (VehicleStreaming.GetLockState(vehicle))
                            {
                                VehicleStreaming.SetLockStatus(vehicle, false);
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehOpenDoor), 3000);
                                return;
                            }
                            else
                            {
                                VehicleStreaming.SetLockStatus(vehicle, true);
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehCloseDoor), 3000);
                                return;
                            }
                        case VehicleAccess.Garage:
                        case VehicleAccess.Personal:
                            bool access = canAccessByNumber(player, vehicle.NumberPlate);
                            if (!access && characterData.AdminLVL < 3)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoKeysFromVeh), 3000);
                                return;
                            }

                            if (VehicleStreaming.GetLockState(vehicle))
                            {
                                VehicleStreaming.SetLockStatus(vehicle, false);
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehOpenDoor), 3000);
                                return;
                            }
                            else
                            {
                                VehicleStreaming.SetLockStatus(vehicle, true);
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehCloseDoor), 3000);
                                return;
                            }
                        case VehicleAccess.Admin:
                            if (characterData.AdminLVL == 0)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoKeysFromVeh), 3000);
                                return;
                            }

                            if (VehicleStreaming.GetLockState(vehicle))
                            {
                                VehicleStreaming.SetLockStatus(vehicle, false);
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehOpenDoor), 3000);
                                return;
                            }
                            else
                            {
                                VehicleStreaming.SetLockStatus(vehicle, true);
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehCloseDoor), 3000);
                                return;
                            }
                        default:
                            if (characterData.AdminLVL <= 3)
                            {
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehNoLock), 3000);
                                return;
                            }
                            if (VehicleStreaming.GetLockState(vehicle))
                            {
                                VehicleStreaming.SetLockStatus(vehicle, false);
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehOpenDoor), 3000);
                                return;
                            }
                            else
                            {
                                VehicleStreaming.SetLockStatus(vehicle, true);
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehCloseDoor), 3000);
                                return;
                            }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"ChangeVehicleDoors Exception: {e.ToString()}");
            }
        }
        public static bool canAccessByNumber(ExtPlayer player, string number)
        {
            try
            {
                if (!player.IsCharacterData()) 
                    return false;
                
                var vehicleData = GetVehicleToNumber(number);
                if (vehicleData == null) 
                    return false;
                
                var needData = $"{vehicleData.SqlId}_{vehicleData.KeyNum}";
                var aItemStruct = Chars.Repository.isItem(player, "inventory", ItemId.CarKey, needData);
                if (aItemStruct == null)
                {
                    aItemStruct = Chars.Repository.isItem(player, "inventory", ItemId.KeyRing);
                    if (aItemStruct != null && Chars.Repository.isItemOther($"CarKey_{aItemStruct.Item.SqlId}", ItemId.CarKey, needData) != null) return true;
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Write($"canAccessByNumber Exception: {e.ToString()}");
                return false;
            }
        }
        public static void onClientEvent(ExtPlayer sender, string eventName)
        {
            try
            {
                var sessionData = sender.GetSessionData();
                if (sessionData == null) 
                    return;

                var characterData = sender.GetCharacterData();
                if (characterData == null)
                    return;
                
                if (Main.IHaveDemorgan(sender)) return;
                
                switch (eventName)
                {
                    case "engineCarPressed":
                        if (!NAPI.Player.IsPlayerInAnyVehicle(sender)) return;
                        if (sender.VehicleSeat != (int)VehicleSeat.Driver)
                        {
                            Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustBeDriver), 3000);
                            return;
                        }
                        var vehicle = (ExtVehicle) sender.Vehicle;
                        if (IsVehicleDeath(vehicle)) return;
                        var vehicleLocalData = vehicle.GetVehicleLocalData();
                        if (vehicleLocalData != null)
                        {
                            if (vehicle.Class == 13 && characterData.InsideGarageID == -1) return;
                            if (vehicleLocalData.Petrol <= 0)
                            {
                                Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoGasVeh), 3000);
                                return;
                            }
                            switch (vehicleLocalData.Access)
                            {
                                case VehicleAccess.Hotel:
                                    if (vehicleLocalData.Owner != sender && characterData.AdminLVL < 3)
                                    {
                                        Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoKeysFromVeh), 3000);
                                        return;
                                    }
                                    break;
                                case VehicleAccess.School:
                                    if (vehicleLocalData.WorkDriver != characterData.UUID && characterData.AdminLVL < 3)
                                    {
                                        Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoKeysFromVeh), 3000);
                                        return;
                                    }
                                    break;
                                case VehicleAccess.Rent:
                                    if (vehicleLocalData.WorkDriver != characterData.UUID && characterData.AdminLVL < 3)
                                    {
                                        Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoKeysFromVeh), 3000);
                                        return;
                                    }
                                    break;
                                case VehicleAccess.Work:
                                    if (vehicleLocalData.WorkDriver != characterData.UUID && characterData.AdminLVL < 3)
                                    {
                                        Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoKeysFromVeh), 3000);
                                        return;
                                    }
                                    break;
                                case VehicleAccess.Fraction:
                                    var fracId = sender.GetFractionId();
                                    if (fracId != vehicleLocalData.Fraction && characterData.AdminLVL < 5)
                                    {
                                        Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoKeysFromVeh), 3000);
                                        return;
                                    }
                                    if (!sessionData.WorkData.OnDuty && Manager.FractionTypes[fracId] == FractionsType.Gov)
                                    {
                                        Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WorkDayNotStarted), 3000);
                                        return;
                                    }
                                    break;
                                case VehicleAccess.Organization:
                                case VehicleAccess.OrganizationGarage:
                                    var memberOrganizationData = sender.GetOrganizationMemberData();
                                    if (memberOrganizationData == null) 
                                    {
                                        Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoKeysFromVeh), 3000);
                                        return;
                                    }
                                    if (memberOrganizationData.Id != vehicleLocalData.Fraction && characterData.AdminLVL < 5)
                                    {
                                        Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoKeysFromVeh), 3000);
                                        return;
                                    }
                                    else if (vehicleLocalData.MinRank > memberOrganizationData.Rank)
                                    {

                                        Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoAccessToVeh), 3000);
                                        return;
                                    }
                                    if (vehicleLocalData.Access == VehicleAccess.OrganizationGarage)
                                    {
                                        Organizations.Manager.GetVehicleFromGarage(sender, vehicle, memberOrganizationData.Id);
                                        //return;
                                    }
                                    break;
                                case VehicleAccess.Personal:

                                    string number = vehicle.NumberPlate;
                                    bool access = canAccessByNumber(sender, number);
                                    if (!access && characterData.AdminLVL < 3)
                                    {
                                        Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoKeysFromVeh), 3000);
                                        return;
                                    }
                                    var vehicleData = GetVehicleToNumber(number);
                                    if (vehicleData == null || vehicleData.Health == 0)
                                    {
                                        Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarDestroyed), 3000);
                                        return;
                                    }
                                    break;
                                case VehicleAccess.Garage:
                                    if (characterData.InsideGarageID == -1) return;
                                    number = vehicle.NumberPlate;
                                    vehicleData = GetVehicleToNumber(number);
                                    if (vehicleData == null || vehicleData.Health == 0)
                                    {
                                        Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CarDestroyed), 3000);
                                        return;
                                    }
                                    var garage = Houses.GarageManager.Garages[characterData.InsideGarageID];
                                    characterData.InsideGarageID = -1;
                                    garage.GetVehicleFromGarage(number, sender);
                                    //Trigger.ClientEvent(sender, "vehicle.teleport");
                                    break;
                                case VehicleAccess.Admin:
                                    if (characterData.AdminLVL == 0)
                                    {
                                        Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoKeysFromVeh), 3000);
                                        return;
                                    }
                                    break;
                            }
                            if (VehicleStreaming.GetEngineState(vehicle))
                            {
                                VehicleStreaming.SetEngineState(vehicle, false);
                                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehEngineON), 3000);
                                Commands.RPChat("sme", sender, $"заглушил" + (characterData.Gender ? "" : "а") + " транспортное средство");
                            }
                            else
                            {
                                VehicleStreaming.SetEngineState(vehicle, true);
                                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.VehEngineOFF), 3000);
                                Commands.RPChat("sme", sender, $"завел" + (characterData.Gender ? "" : "а") + " транспортное средство");
                            }
                        }
                        return;
                    case "lockCarPressed":
                        if (NAPI.Player.IsPlayerInAnyVehicle(sender) && sender.VehicleSeat == (int)VehicleSeat.Driver)
                        {
                            if (IsVehicleDeath((ExtVehicle) sender.Vehicle)) return;
                            ChangeVehicleDoors(sender, (ExtVehicle) sender.Vehicle);
                            return;
                        }
                        var veh = getNearestVehicle(sender, 5);
                        if (veh != null)
                        {
                            if (IsVehicleDeath(veh)) return;
                            ChangeVehicleDoors(sender, veh);
                        }
                        //else Main.DoorControlState(sender);
                        break;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"onClientEvent Exception: {e.ToString()}");
            }
        }
        public static bool IsVehicleDeath(ExtVehicle vehicle)
        {
            var vehicleLocalData = vehicle.GetVehicleLocalData();
            if (vehicleLocalData != null && vehicle.Health < 1f)
            {
                Event_vehicleDeath(vehicle);
                return true;
            }
            return false;
        }
        public static void Event_vehicleDeath(ExtVehicle vehicle, bool isTicket = false)
        {
            try
            {
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null && vehicle.Health < 1f)
                {
                    GameLog.AddInfo($"(Death) vehicle({vehicle.NumberPlate})");

                    if (vehicleLocalData.AttachToPlayer != null)
                    {
                        ExtPlayer target = vehicleLocalData.AttachToPlayer;
                        var targetSessionData = target.GetSessionData();
                        if (targetSessionData == null) return;
                        if (target.IsCharacterData())
                        {
                            targetSessionData.AttachToVehicle = null;
                            Trigger.ClientEventInRange(target.Position, 250f, "client.vehicle.trunk.detachPlayer", target.Value, vehicle.Value, false);
                            Trigger.StopAnimation(target);
                            target.ResetSharedData("AttachToVehicle");
                            Trigger.ClientEvent(target, "setPocketEnabled", false);
                            target.Armor = 0;
                            target.Health = 0;
                        }
                        vehicleLocalData.AttachToPlayer = null;
                    }
                    
                    switch (vehicleLocalData.Access)
                    {
                        case VehicleAccess.Fraction:
                            Chars.Repository.RemoveAll(GetVehicleToInventory(vehicle.NumberPlate));
                            if (!isTicket) 
                                Admin.RespawnFractionCar(vehicle);
                            break;
                        case VehicleAccess.Admin:
                        case VehicleAccess.DeliveryGang:
                        case VehicleAccess.DeliveryMafia:
                        case VehicleAccess.DeliveryBike:
                        case VehicleAccess.School:
                        case VehicleAccess.Rent:
                        case VehicleAccess.Work:
                        case VehicleAccess.AutoRoom:
                        case VehicleAccess.Event:
                            Chars.Repository.RemoveAll(GetVehicleToInventory(vehicle.NumberPlate));
                            if (!isTicket) 
                                VehicleStreaming.DeleteVehicle(vehicle);
                            break;
                        /*ca  se "FRACTION":
                            if (Chars.Repository.getCountItems(GetVehicleToInventory(vehicle.NumberPlate)) >= 1)
                            {
                                Chars.Repository.RemoveAll(GetVehicleToInventory(vehicle.NumberPlate));
                                Manager.sendFractionMessage(data.Fraction, $"~o~Багажник фракционного транспорта с номерами {vehicle.NumberPlate} был утерян из-за уничтожения.", true);
                            }
                            return;
                        case "ORGANIZATION":
                            if (Chars.Repository.getCountItems(GetVehicleToInventory(vehicle.NumberPlate)) >= 1)
                            {
                                Chars.Repository.RemoveAll(GetVehicleToInventory(vehicle.NumberPlate));
                                Manager.sendOrganizationMessage(data.Fraction, $"~o~Багажник организационного транспорта с номерами {vehicle.NumberPlate} был утерян из-за уничтожения.", true);
                            }
                            return;*/
                        case VehicleAccess.Garage:
                        case VehicleAccess.Personal:
                            {
                                if (isTicket) 
                                    return;
                                
                                string number = vehicle.NumberPlate;
                                var vehicleData = GetVehicleToNumber(number);
                                if (vehicleData != null)
                                {
                                    vehicleData.Position = null;
                                    vehicleData.Health = 0;
                                    if (!VehicleModel.AirAutoRoom.isAirCar(vehicleData.Model))
                                    {
                                        var house = Houses.HouseManager.GetHouse(vehicleData.Holder, true);
                                        if (house != null)
                                        {
                                            var garage = house.GetGarageData();
                                            if (garage != null)
                                            {
                                                garage.DeleteCar(number, true);
                                                if (garage.Type != -1 && garage.Type != 6)
                                                    garage.SpawnCar(number);
                                                else
                                                    garage.GetVehicleFromGarage(number);
                                            }
                                        }

                                        var owner = (ExtPlayer) NAPI.Player.GetPlayerFromName(vehicleData.Holder);
                                        if (owner != null)
                                            Notify.Send(owner, NotifyType.Alert, NotifyPosition.BottomCenter,
                                                LangFunc.GetText(LangType.Ru, DataName.CarDestroyed), 3000);
                                        
                                        return;
                                    }
                                    else
                                    {
                                        
                                        var owner = (ExtPlayer) NAPI.Player.GetPlayerFromName(vehicleData.Holder);
                                        if (owner != null)
                                            Notify.Send(owner, NotifyType.Alert, NotifyPosition.BottomCenter,
                                                LangFunc.GetText(LangType.Ru, DataName.CarDestroyed), 3000);
                                    }
                                }
                                VehicleStreaming.DeleteVehicle(vehicle);
                            }
                            return;
                        case VehicleAccess.Hotel:
                            var hotelCharacterData = vehicleLocalData.Owner.GetCharacterData();
                            var hotelSessionData = vehicleLocalData.Owner.GetSessionData();
                            if (hotelSessionData != null && hotelCharacterData != null && hotelSessionData.HotelData != null && hotelSessionData.HotelData.Car == vehicle)
                            {
                                hotelSessionData.HotelData.Car = null;
                            }
                            Chars.Repository.RemoveAll(GetVehicleToInventory(vehicle.NumberPlate));
                            if (!isTicket) 
                                VehicleStreaming.DeleteVehicle(vehicle);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"Event_vehicleDeath Exception: {e.ToString()}");
            }
        }

        public static string GenerateNumber()
        {
            string number;
            do
            {
                number = "";
                number += (char)Rnd.Next(65, 90);
                for (int i = 0; i < 3; i++) number += (char)Rnd.Next(48, 57);
                number += (char)Rnd.Next(65, 90);

            } while (VehicleNumbers.Contains(number));
            return number;
        }
        
        public static string GenerateNumber(VehicleAccess vehicleAccess, string prifix)
        {
            string number;
            do
            {
                number = prifix;
                for (int i = 0; i < 4; i++) 
                    number += (char)Rnd.Next(48, 57);

            } while (NeptuneEvo.VehicleData.LocalData.Repository.VehicleNumberToHandle[vehicleAccess].ContainsKey(number));
            return number;
        }
        public static void changeOwner(string oldName, string newName)
        {
            try
            {
                var toChange = Vehicles
                    .Where(b => b.Value.Holder == oldName)
                    .Select(b => b.Key)
                    .ToList();

                foreach (string number in toChange)
                {
                    var vehicleData = GetVehicleToNumber(number);
                    
                    if (vehicleData != null)
                        vehicleData.Holder = newName;
                }

                Task.Run(async () => {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.Vehicles
                            .Where(v => v.Holder == oldName)
                            .Set(v => v.Holder, newName)
                            .UpdateAsync();
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
            }
            catch (Exception e)
            {
                Log.Write($"changeOwner Exception: {e.ToString()}");
            }
        }
    }
}
