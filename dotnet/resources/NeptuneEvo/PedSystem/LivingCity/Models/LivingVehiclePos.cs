using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.PedSystem.LivingCity.Models
{
    internal class LivingVehiclePos
    {
        public Vector3 Position;
        public float Rotation;

        private DateTime Used = DateTime.Now;

        public LivingVehiclePos(float x, float y, float z, float rotation)
        {
            Position = new Vector3(x, y, z);
            Rotation = rotation;
        }

        public bool IsAllowedToSpawn(DateTime now) => Used.AddSeconds(10) < now;

        public void SpawnedNow()
        {
            Used = DateTime.Now;
        }
    }
}
