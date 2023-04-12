using GTANetworkAPI;
using NeptuneEvo.Handles;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.PedSystem.Models
{
    public class PedData
    {
        public ExtPed Ped { set; get; } = null;
        public ExtColShape ColShape { set; get; } = null;
        public ExtTextLabel TextLabel { set; get; } = null;
    }
}
