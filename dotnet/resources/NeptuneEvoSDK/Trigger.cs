using GTANetworkAPI;
using System;
using System.Threading;

namespace Redage.SDK
{
    /// <summary>
    /// 
    /// </summary>
    public enum OwnDisconnectionType
    {
        /// <summary>
        /// 
        /// </summary>
        LeftV1 = 0,
        /// <summary>
        /// 
        /// </summary>
        LeftV2 = 1,
        /// <summary>
        /// 
        /// </summary>
        Timeout = 2,
        /// <summary>
        /// 
        /// </summary>
        Kicked = 3
    }
    /// <summary>
    /// 
    /// </summary>
    public static class Trigger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="eventName"></param>
        /// <param name="args"></param>
        public static void ClientEvent(Player player, string eventName, params object[] args)
        {
            try
            {
                if (Thread.CurrentThread.Name == "Main")
                {
                    if (player == null) return;
                    NAPI.ClientEvent.TriggerClientEvent(player, eventName, args);
                    return;
                }
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        if (player == null) return;
                        NAPI.ClientEvent.TriggerClientEvent(player, eventName, args);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"ClientEvent Task Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"ClientEvent({eventName}) Exception: {e.ToString()}");
            }
        }
    }
}
