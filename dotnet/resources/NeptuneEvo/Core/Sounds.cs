using GTANetworkAPI;
using NeptuneEvo.Handles;
using System;

namespace NeptuneEvo.Core
{
    public class PannerAttr
    {
        public string panningModel { get; set; } = "equalpower";
        public string distanceModel { get; set; } = "linear";
    }
    public class SoundData
    {
        public object id { get; set; } = null;
        public double volume { get; set; } = 0.3;
        public bool looped { get; set; } = false;
        public int maxDistance { get; set; } = 10;
        public int dimension { get; set; } = 0;
        public int rolloffFactor { get; set; } = 1;
        public int refDistance { get; set; } = 1;
        public int startOffsetPercent { get; set; } = 0;
        public PannerAttr pannerAttr { get; set; } = null;
        public int fade { get; set; } = 1000;
        public int syncAudio { get; set; } = 0;
    }
    class Sounds : Script
    {
        public static void Play3d(Vector3 pos, float range, string name, string _sound, SoundData data = null)
        {
            if (data == null) data = new SoundData();
            Trigger.ClientEventInRange(pos, range, "sounds.playPosAmbient", name, _sound, pos, data);
        }
        public static void PlayPlayer3d(Entity entity, string _sound, SoundData data = null, float range = 10f)
        {
            if (data == null) 
                data = new SoundData();

            data.id = $"{entity.Type}_{entity.Value}";
            
            if (entity.Type == EntityType.Player)
                Trigger.ClientEvent((ExtPlayer)entity, "sounds.playAmbient", _sound, data);
            
            Trigger.ClientEventInRange(entity.Position, range, "sounds.playEntityAmbient", _sound, entity, data);
        }
        public static void Play2d(ExtPlayer player, string _url, float _volume = 0.1f)
        {
            Trigger.ClientEvent(player, "sounds.play2DRadio", _url, _volume);
        }
        public static void Stop2d(ExtPlayer player)
        {
            Trigger.ClientEvent(player, "sounds.stop2DRadio");
        }
        public static void Stop3d(Entity entity, float range = 10f)
        {
            var id = $"{entity.Type}_{entity.Value}";
            
            if (entity.Type == EntityType.Player)
                Trigger.ClientEvent((ExtPlayer)entity, "sounds.stop", id);
            
            Trigger.ClientEventInRange(entity.Position, range, "sounds.trigger", "onEnd", id);
        }
    }
}