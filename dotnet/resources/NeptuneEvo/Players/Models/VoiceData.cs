using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Models
{
    public class VoiceData
    {
        public ExtPlayer Target { get; set; } = null;
        public string CallingState { get; set; } = "nothing";
    }
}
