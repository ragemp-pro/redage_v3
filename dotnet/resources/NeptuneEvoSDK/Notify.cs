using GTANetworkAPI;

namespace Redage.SDK
{
    /// <summary>
    /// 
    /// </summary>
    public enum NotifyType
    {
        /// <summary>
        /// 
        /// </summary>
        Alert,
        /// <summary>
        /// 
        /// </summary>
        Error,
        /// <summary>
        /// 
        /// </summary>
        Success,
        /// <summary>
        /// 
        /// </summary>
        Info,
        /// <summary>
        /// 
        /// </summary>
        Warning
    }
    /// <summary>
    /// 
    /// </summary>
    public enum NotifyPosition
    {
        /// <summary>
        /// 
        /// </summary>
        Top,
        /// <summary>
        /// 
        /// </summary>
        TopLeft,
        /// <summary>
        /// 
        /// </summary>
        TopCenter,
        /// <summary>
        /// 
        /// </summary>
        TopRight,
        /// <summary>
        /// 
        /// </summary>
        Center,
        /// <summary>
        /// 
        /// </summary>
        CenterLeft,
        /// <summary>
        /// 
        /// </summary>
        CenterRight,
        /// <summary>
        /// 
        /// </summary>
        Bottom,
        /// <summary>
        /// 
        /// </summary>
        BottomLeft,
        /// <summary>
        /// 
        /// </summary>
        BottomCenter,
        /// <summary>
        /// 
        /// </summary>
        BottomRight
    }
    /// <summary>
    /// 
    /// </summary>
    public static class Notify
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="type"></param>
        /// <param name="pos"></param>
        /// <param name="msg"></param>
        /// <param name="time"></param>
        public static void Send(Player player, NotifyType type, NotifyPosition pos, string msg, int time)
        {
            Trigger.ClientEvent(player, "notify", type, pos, msg, time);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="type"></param>
        /// <param name="pos"></param>
        /// <param name="msg"></param>
        /// <param name="time"></param>
        /// <param name="keyCode"></param>
        public static void SendToKey(Player player, NotifyType type, NotifyPosition pos, string msg, int time, int keyCode)
        {
            Trigger.ClientEvent(player, "notifyToKey", type, pos, msg, time, keyCode);
        }
    }
}
