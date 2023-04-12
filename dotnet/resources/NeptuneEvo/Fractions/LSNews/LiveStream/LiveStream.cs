using System;
using System.Collections.Generic;
using System.Linq;
using NeptuneEvo.Handles;
using Newtonsoft.Json;

namespace NeptuneEvo.Fractions.LSNews.LiveStream
{
    public class LiveStream
    {
        private class Events
        {
            public const string UpdateHud = "client.livestream.updateHud";
        }

        private ExtPlayer _owner;
        private ExtPlayer _cooperator;

        private DateTime _ownerLastMessageTime = DateTime.Now;
        private DateTime _coopLastMessageTime = DateTime.Now;

        private List<Message> _messages = new List<Message>();
        private List<ExtPlayer> _callers = new List<ExtPlayer>();

        private static int LsNewsFracid = 15;

        public LiveStream(ExtPlayer owner)
        {
            _owner = owner;
        }

        public void Start()
        {
            Manager.sendFractionMessage(LsNewsFracid, $"FF8C00 [F] {_owner.Name} ({_owner.Id}) начал прямой эфир.");
        }

        public void Stop()
        {
            Manager.sendFractionMessage(LsNewsFracid, $"FF8C00 [F] {_owner.Name} ({_owner.Id}) закончил прямой эфир.");
        }

        public void SendMessage(ExtPlayer player, string msg)
        {
            _messages.Add(new Message("111", msg, IsOwner(player)));
            SendMessageToListeners(GetListeners(), msg, IsOwner(player));
            UpdateStreamInfo();
        }

        public void AddToCallersList(ExtPlayer player)
        {
            if (_callers.Contains(player)) return;
            _callers.Add(player);
            UpdateStreamInfo();
        }

        public void RemoveFromCallersList(ExtPlayer player)
        {
            if (_cooperator == player) DisableCaller(player);
            if (_callers.Remove(player)) UpdateStreamInfo();
            
        }

        public void AcceptCaller(ExtPlayer player)
        {
            if (_callers.Contains(player) == false) return;
            _cooperator = player;
            UpdateStreamInfo();
        }

        public void DisableCaller(ExtPlayer player)
        {
            _cooperator = null;
            RemoveFromCallersList(player);
        }

        public bool IsOwner(ExtPlayer player) => (player == _owner);
        public bool IsCooperator(ExtPlayer player) => (player == _cooperator);
        public bool IsInLiveStream(ExtPlayer player) => (IsOwner(player) || IsCooperator(player));

        private void SendMessageToListeners(IEnumerable<ExtPlayer> listeners, string msg, bool isOwner)
        {
            foreach (var listener in listeners)
            {
                string prefix = isOwner ? "#FAFAD2 [Weazel News] Ведущий: " : " #FAFAD2 [Weazel News] Гость: ";
                listener.SendChatMessage(prefix + msg);
            }
        }

        private void UpdateStreamInfo()
        {
            var callers = _callers.Where(x => x.CharacterData != null).Select(caller => new
            {
                caller.Name,
                caller.Id,
                caller.CharacterData.Sim
            });
            Trigger.ClientEvent(_owner, Events.UpdateHud, JsonConvert.SerializeObject(_messages),
                JsonConvert.SerializeObject(callers));
            Trigger.ClientEvent(_cooperator, Events.UpdateHud, JsonConvert.SerializeObject(_messages));
        }

        private IEnumerable<ExtPlayer> GetListeners()
        {
            var players = Character.Repository.GetPlayers();
            return players;
        }
    }
}