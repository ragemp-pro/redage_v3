using System;
using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Events.AirDrop.Models
{
    public class AirDropData
    {
        public int DropId { set; get; } = 0;
        public DateTime DateTime { set; get; } = DateTime.Now;
        public DateTime DateTimeNotification { set; get; } = DateTime.Now;
        public sbyte Status { set; get; } = 0;
        public Vector3 Position { set; get; } = new Vector3();
        public Vector3 CentralPosition { set; get; } = new Vector3();
        public int AirdropLockHealth { set; get; } = 20;
        public ExtPlayer AirdropHackPlayerInfo { set; get; } = null;

        public AirDropData(int dropId)
        {
            DropId = dropId;
        }
    }
}