using NeptuneEvo.Handles;
using NeptuneEvo.PedSystem.LivingCity.Models;
using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Database;
using LinqToDB;
using Redage.SDK;
using System.IO;
using Newtonsoft.Json;
using NeptuneEvo.Players;
using NeptuneEvo.Character;
using System.Linq;

namespace NeptuneEvo.PedSystem.LivingCity
{
    internal class Repository : Script
    {
        private static readonly nLog Log = new nLog("PedSystem.LivingCity");

        private static readonly List<uint> DriverHashes = new List<uint>();
        private static readonly List<uint> VehicleHashes = new List<uint>();
        private static readonly List<LivingPed> LivingCity = new List<LivingPed>();

        private static readonly Random Rnd = new Random();
        private static string Timer = null;
        private static bool Enabled = false;

        private const byte MAX_NPC_PER_PLAYER = 6;
        private const byte MAX_NPC_IN_ONE_LOCATION = MAX_NPC_PER_PLAYER * 3;
        private const float MAX_CONTROLLER_DISTANCE = 300.0f;
        private const float MAX_NEW_CONTROLLER_DISTANCE = 275.0f;
        private const float MIN_CONTROLLER_DISTANCE = 150.0f;

        private static void InitializeHashes()
        {
            AddDriverHashes((uint)PedHash.AfriAmer01AMM, (uint)PedHash.ArmGoon01GMM, (uint)PedHash.ArmGoon02GMY, (uint)PedHash.ArmLieut01GMM, (uint)PedHash.Baygor, (uint)PedHash.Bevhills01AFY, (uint)PedHash.Bevhills01AFM, (uint)PedHash.Hipster02AFY);
            AddVehicleHashes((uint)VehicleHash.Prairie, (uint)VehicleHash.Blista, (uint)VehicleHash.Asbo, (uint)VehicleHash.Felon, (uint)VehicleHash.Sentinel, (uint)VehicleHash.Akuma, (uint)VehicleHash.Bagger, (uint)VehicleHash.Blade, (uint)VehicleHash.Deviant, (uint)VehicleHash.Rancherxl, (uint)VehicleHash.Baller, (uint)VehicleHash.Landstalker, (uint)VehicleHash.Novak, (uint)VehicleHash.Ingot, (uint)VehicleHash.Glendale, (uint)VehicleHash.Bus, NAPI.Util.GetHashKey("emsnspeedo"));
        }

        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            if (!Enabled) return;

            InitializeHashes();
            Timer = Timers.Start(5000, SpawnNewNPCs, true);

            Trigger.SetAsyncTask(VehiclePositions.Initialize);
        }

        [ServerEvent(Event.ResourceStop)]
        public void OnResourceStop()
        {
            if (Timer == null) return;
            Timers.Stop(Timer);
            Timer = null;
        }

        public static void OnPlayerDisconnect(ExtPlayer player)
        {
            DestroyAllPlayersNPC(player);
        }

        private static int GetPedsCountNearPoint(Vector3 point, float maxDistance)
        {
            int count = 0;
            foreach (Ped ped in NAPI.Pools.GetAllPeds())
            {
                if (ped == null || ped.Dimension != 0) continue;
                if (ped.Position.DistanceTo2D(point) >= maxDistance) continue;
                count++;
            }
            return count;
        }

        private static void DestroyNPC(LivingPed ped)
        {
            if (!LivingCity.Contains(ped)) return;

            ped.Destroy();
            LivingCity.Remove(ped);
        }

        private static void DestroyAllPlayersNPC(ExtPlayer player)
        {
            List<LivingPed> livingPeds = GetPlayerLivingPeds(player);
            if (livingPeds == null || livingPeds.Count == 0) return;

            foreach (LivingPed livingPed in livingPeds)
            {
                DestroyNPC(livingPed);
            }
        }

        private static void CreateNPC(ExtPlayer player, LivingVehiclePos position)
        {
            uint randomDriver = GetRandomPedHash();
            uint randomVehicle = GetRandomVehicleHash();

            ExtPed ped = (ExtPed)NAPI.Ped.CreatePed(randomDriver, position.Position.Around(2f), 0f, true, controlLocked: true, dimension: 0);
            ExtVehicle veh = (ExtVehicle)NAPI.Vehicle.CreateVehicle(randomVehicle, position.Position, position.Rotation, GetRandomVehicleColor(), GetRandomVehicleColor(), "LC0" + ped.Value.ToString(), locked: true);
            VehiclePositions.RegisterVehiclePosAsSpawned(position);
            LivingCity.Add(new LivingPed(ped, veh, player));
        }

        private static void SpawnNewNPCs()
        {
            if (VehiclePositions.GetCountOfVehiclePositions() == 0) return;

            int controllerCityCount;
            Vector3 playerPosition;
            Players.Models.SessionData playerSession;
            List<LivingVehiclePos> vehiclePositions;
            List<LivingPed> livingPeds;
            Player pedActualController;
            foreach (ExtPlayer player in Character.Repository.GetPlayers())
            {
                playerSession = player.GetSessionData();
                if (playerSession == null || !playerSession.LoggedIn) continue;

                livingPeds = GetPlayerLivingPeds(player);
                foreach (LivingPed ped in livingPeds)
                {
                    if (ped == null) continue;
                    if (ped.Ped == null || ped.Ped.Controller == null)
                    {
                        DestroyNPC(ped);
                        continue;
                    }
                    if (!ped.IsSpawned) continue;
                    pedActualController = ped.Ped.Controller;
                    if (pedActualController.Position.DistanceTo2D(ped.Ped.Position) >= MAX_CONTROLLER_DISTANCE)
                    {
                        DestroyNPC(ped);
                        continue;
                    }
                    if (pedActualController != player) ped.Controller = (ExtPlayer)pedActualController;
                    if (NAPI.Vehicle.GetVehicleDriver(ped.Vehicle) == null) DestroyNPC(ped);
                }

                if (playerSession.Dimension != 0 || (player.Vehicle != null && (player.Vehicle.Class == 16 || player.Vehicle.Class == 15 || player.Vehicle.Class == 14))) continue;

                controllerCityCount = livingPeds.Count();
                if (controllerCityCount >= MAX_NPC_PER_PLAYER) continue;

                playerPosition = player.Position;
                if (GetPedsCountNearPoint(playerPosition, MAX_NEW_CONTROLLER_DISTANCE) >= MAX_NPC_IN_ONE_LOCATION) continue;

                vehiclePositions = VehiclePositions.GetVehiclePositionsFromPoint(playerPosition, MIN_CONTROLLER_DISTANCE, MAX_NEW_CONTROLLER_DISTANCE);
                foreach (LivingVehiclePos vehiclePosition in vehiclePositions)
                {
                    if (controllerCityCount >= MAX_NPC_PER_PLAYER) break;
                    if (NAPI.Player.GetPlayersInRadiusOfPosition(MIN_CONTROLLER_DISTANCE, vehiclePosition.Position).Count() >= 1) continue;
                    CreateNPC(player, vehiclePosition);
                    controllerCityCount++;
                }
            }
        }

        private static bool ChangeNPCController(LivingPed livingPed, Player newController, bool force = true)
        {
            if (!LivingCity.Contains(livingPed)) return false;

            ExtPlayer extNewController = (ExtPlayer)newController;
            if (GetCountOfPlayerLivingPeds(extNewController) >= MAX_NPC_PER_PLAYER) return false;
            livingPed.Controller = extNewController;
            return true;
        }

        public static bool AddVehicleHash(uint vehicle)
        {
            if (VehicleHashes.Contains(vehicle)) return false;
            VehicleHashes.Add(vehicle);
            return true;
        }

        public static bool RemoveVehicleHash(uint vehicle)
        {
            if (!VehicleHashes.Contains(vehicle)) return false;
            VehicleHashes.Remove(vehicle);
            return true;
        }

        private static void AddVehicleHashes(params uint[] vehicles)
        {
            foreach (uint vehicle in vehicles)
            {
                if (VehicleHashes.Contains(vehicle)) continue;
                VehicleHashes.Add(vehicle);
            }
        }

        public static bool AddDriverHash(uint driver)
        {
            if (DriverHashes.Contains(driver)) return false;
            DriverHashes.Add(driver);
            return true;
        }

        public static bool RemoveDriverHash(uint driver)
        {
            if (!DriverHashes.Contains(driver)) return false;
            DriverHashes.Remove(driver);
            return true;
        }

        private static void AddDriverHashes(params uint[] drivers)
        {
            foreach (uint driver in drivers)
            {
                if (DriverHashes.Contains(driver)) continue;
                DriverHashes.Add(driver);
            }
        }

        private static List<LivingPed> GetPlayerLivingPeds(ExtPlayer player) => LivingCity.Where(x => x.Controller == player).ToList();
        private static int GetCountOfPlayerLivingPeds(ExtPlayer player) => LivingCity.Where(x => x.Controller == player).Count();
        private static uint GetRandomPedHash() => DriverHashes[Rnd.Next(DriverHashes.Count)];
        private static uint GetRandomVehicleHash() => VehicleHashes[Rnd.Next(VehicleHashes.Count)];
        private static int GetRandomVehicleColor() => Rnd.Next(250);
    }
}
