using GTANetworkAPI;
using NeptuneEvo.Handles;
using System;

namespace NeptuneEvo.Players.Models
{
    public class WorkData
    {
        public bool OnWork { get; set; } = false;
        public bool BusOnStop { get; set; } = false;
        public int Packages { get; set; } = 0;
        public int WorkWay { get; set; } = -1;
        public int WorkCheck { get; set; } = -1;
        public bool OnDuty { get; set; } = false;
        public string OnDutyName { get; set; } = String.Empty;
        public ExtPlayer Player { get; set; } = null;
        public int TimerCount { get; set; } = 0;
        public Vector3 Position { get; set; } = new Vector3();
        public DateTime Time { get; set; } = DateTime.Now;
        public int PointsCount { get; set; } = 0;
        public DateTime LastClientTime { get; set; } = DateTime.Now;
    }
}
