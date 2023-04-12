using GTANetworkAPI;
using Newtonsoft.Json;

namespace NeptuneEvo.World
{
    public static class ChatHeadOverlay
    {
        private const string SendOverlayMessageEvent = "SRV::CL::OverlayMessage";


        public static void SendOverlayMessage(Player player, int senderId, MessageType type, string message, bool result = false)
        {
            var msgInfo = new
            {
                sender = senderId,
                type = (int)type,
                text = message,
                result = result
            };
            
            player.TriggerEvent(SendOverlayMessageEvent, JsonConvert.SerializeObject(msgInfo));
        }
        

        public enum MessageType
        {
            Message = 1,
            Me = 2,
            Do = 3,
            Try = 4
        }
    }
}