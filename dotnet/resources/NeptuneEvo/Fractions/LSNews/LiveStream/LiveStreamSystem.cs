using System.Linq;
using GTANetworkAPI;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Handles;
using NeptuneEvo.Table.Models;

namespace NeptuneEvo.Fractions.LSNews.LiveStream
{
    public class LiveStreamSystem : Script
    {
        private static LiveStream _stream = null;
        
        [RemoteEvent("server.ls.startStream")]
        public static void StartStream(ExtPlayer player)
        {
            if (_stream == null)
            {
                if (player.IsFractionAccess(RankToAccess.StartLiveStream) == false)
                {
                    // ответить, что нет доступа к созданию эфира
                    return;
                }
                _stream = new LiveStream(player);
                _stream.Start();
            }
            else
            {
                // ответить, что стрим уже идёт
            }
        }
        
        [RemoteEvent("server.ls.stopStream")]
        public static void StopStream(ExtPlayer player)
        {
            if (_stream != null)
            {
                if (_stream.IsOwner(player) == false)
                {
                    // ответить, что не владелец
                }
                _stream.Stop();
                _stream = null;
            }
            else
            {
                // ответить, что стрим не идёт
            }
        }

        [RemoteEvent("server.stream.acceptCaller")]
        public static void AcceptCaller(ExtPlayer player, int id)
        {
            if (_stream != null)
            {
                if (_stream.IsOwner(player) == false)
                {
                    // ответить, что не участвует в эфире
                    return;
                }
                _stream.AcceptCaller(player);
            }
            else
            {
                // ответить, что стрим не идёт
            }
        }
        [RemoteEvent("server.stream.disableCaller")]
        public static void DisableCaller(ExtPlayer player, int id)
        {
            if (_stream != null)
            {
                if (_stream.IsOwner(player) == false)
                {
                    // ответить, что не участвует в эфире
                    return;
                }
                _stream.DisableCaller(player);
            }
            else
            {
                // ответить, что стрим не идёт
            }
        }
        
        [RemoteEvent("server.stream.addToCallers")]
        public static void AddToCallers(ExtPlayer player)
        {
            if (_stream != null)
            {
                _stream.AddToCallersList(player);
            }
            else
            {
                // ответить, что стрим не идёт
            }
        }
        [RemoteEvent("server.stream.removeFromCallers")]
        public static void RemoveFromCallers(ExtPlayer player, int id)
        {
            if (_stream != null)
            {
                _stream.RemoveFromCallersList(player);
            }
            else
            {
                // ответить, что стрим не идёт
            }
        }
        
        [RemoteEvent("server.stream.sendMessage")]
        public static void SendMessage(ExtPlayer player, string message)
        {
            if (IsMessageAcceptable(message) == false)
            {
                // ответить player что сообщение содержит запрещенные штуки
                return;
            }
            if (_stream != null)
            {
                if (_stream.IsInLiveStream(player) == false)
                {
                    // ответить, что не участвует в эфире
                    return;
                }
                _stream.SendMessage(player, message);
            }
            else
            {
                // ответить, что стрим не идёт
            }
        }
        
        [ServerEvent(Event.PlayerDisconnected)]
        public static void OnPlayerDisconnected(ExtPlayer player, DisconnectionType type, string reason)
        {
            if (_stream != null)
            {
                if (_stream.IsOwner(player))
                {
                    // останавливаем стрим т.к игрок вышел
                    _stream = null;
                    return;
                }
                _stream.RemoveFromCallersList(player);
            }
        }
        
        private static bool IsMessageAcceptable(string msg)
        {
            var messages = msg.Split(' ');
            foreach (string s in messages)
                if (Main.stringGlobalBlock.Contains(s))
                    return false;
            return true;
        }
        
    }
}