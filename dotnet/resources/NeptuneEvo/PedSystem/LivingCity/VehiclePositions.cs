using Database;
using GTANetworkAPI;
using LinqToDB;
using NeptuneEvo.PedSystem.LivingCity.Models;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeptuneEvo.PedSystem.LivingCity
{
    internal class VehiclePositions
    {
        private static readonly nLog Log = new nLog("PedSystem.VehiclePositions");

        private static readonly List<LivingVehiclePos> LivingVehiclePositions = new List<LivingVehiclePos>();
        private static readonly Random Rnd = new Random();

        public static async Task Initialize()
        {
            await using ServerBD db = new ServerBD("MainDB");
            List<Livingcities> livingCityVehPositions = await db.Livingcity.ToListAsync();
            foreach (Livingcities lcVehPos in livingCityVehPositions)
            {
                if (lcVehPos == null) continue;
                LivingVehiclePositions.Add(new LivingVehiclePos(lcVehPos.VehicleX, lcVehPos.VehicleY, lcVehPos.VehicleZ, lcVehPos.VehicleR));
            }
            Log.Write($"Успешно загружено {LivingVehiclePositions.Count} позиций для NPC живого города.");
        }
        
        public static bool RegisterVehiclePosAsSpawned(LivingVehiclePos positionData)
        {
            LivingVehiclePos livingVehiclePos = LivingVehiclePositions.FirstOrDefault(x => x.Position == positionData.Position);
            if (livingVehiclePos == null) return false;

            livingVehiclePos.SpawnedNow();
            return true;
        }

        public static List<LivingVehiclePos> GetVehiclePositionsFromPoint(Vector3 point, float minDistance, float maxDistance)
        {
            DateTime now = DateTime.Now;
            List<LivingVehiclePos> newList = LivingVehiclePositions.Where(x => x.Position.DistanceTo2D(point) < maxDistance)
                .Where(x => x.Position.DistanceTo2D(point) >= minDistance)
                .Where(x => x.IsAllowedToSpawn(now))
                .ToList();
            return NewCasino.Horses.Shuffle(newList);
        }

        public static LivingVehiclePos GetRandomVehiclePositionFromPoint(Vector3 point, float minDistance, float maxDistance)
        {
            List<LivingVehiclePos> newList = GetVehiclePositionsFromPoint(point, minDistance, maxDistance);
            if (newList.Count == 0) return null;
            return newList.First();
        }

        public static int GetCountOfVehiclePositions() => LivingVehiclePositions.Count;
    }
}
