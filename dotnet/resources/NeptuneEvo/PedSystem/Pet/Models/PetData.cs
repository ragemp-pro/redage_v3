using GTANetworkAPI;
using NeptuneEvo.Handles;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.PedSystem.Pet.Models
{
    public class PetData
    {
        public int AutoId { set; get; } = 0;
        public string Name { set; get; } = null;
        public int OwnerUUID { set; get; } = 0;
        public uint Model { set; get; } = 0;
        public int Health { set; get; } = 100;
        public DateTime Death { set; get; } = DateTime.MinValue;
        public bool InGame { set; get; } = false;
        public Vector3 Position { set; get; } = new Vector3();
        public Vector3 Rotation { set; get; } = new Vector3();
        public float Heading { set; get; } = 0f;
        public uint Dimension { set; get; } = 0;
    }
}

