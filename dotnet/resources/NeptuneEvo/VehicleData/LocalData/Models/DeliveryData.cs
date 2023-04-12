using System;
using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.VehicleData.LocalData.Models
{
    public class DeliveryData
    {
        public int End { get; set; } = -1;
        public int Fraction { get; set; } = -1;
        public DateTime DataEnd { get; set; } = DateTime.MinValue;
        public ExtPlayer WhosVeh { get; set; } = null;
        public bool JStage { get; set; } = false;
    }
}