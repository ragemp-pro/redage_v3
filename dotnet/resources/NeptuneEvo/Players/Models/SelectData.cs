using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Models
{
    public class SelectData
    {
        public string SelectedStock { get; set; } = null;
        public ExtPlayer SelectedPlayer { get; set; } = null;
        public int SelectedBiz { get; set; } = -1;
        public ExtVehicle SelectedVeh { get; set; } = null;
        public string SelectedProd { get; set; } = null;
    }
}
