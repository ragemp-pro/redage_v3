using GTANetworkAPI;
using NeptuneEvo.Handles;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Inventory.Tent.Models
{
    class TentData
    {
        public ExtMarker marker { get; set; } = null;
        public ExtTextLabel label { get; set; } = null;
        public ExtColShape shape { get; set; } = null;
        public ExtPlayer player { get; set; } = null;
        public Dictionary<int, int> slotToPrice { get; set; } = new Dictionary<int, int>();
        public bool isBlack { get; set; } = false;
        public DateTime RentTime { get; set; } = DateTime.MinValue;
        public int UUID { get; set; } = -1;
        public TentData(ExtMarker marker, ExtTextLabel label, ExtColShape shape, bool isBlack = false)
        {
            this.marker = marker;
            this.label = label;
            this.shape = shape;
            this.isBlack = isBlack;
        }
    }
}
