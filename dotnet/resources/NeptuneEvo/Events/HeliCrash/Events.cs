using GTANetworkAPI;
using NeptuneEvo.Core;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Events.HeliCrash
{
    public class Events : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart() => Repository.OnResourceStart();

        [Command("t1c")]
        public void CMD_animplayer(ExtPlayer player)
        {
            if (Main.ServerNumber != 0) return;
            Repository.Create();
        }
        [Command("t1")]
        public static void CMD_t1(ExtPlayer player, string name)
        {
            if (Main.ServerNumber != 0) return;
            
            var veh = (ExtVehicle) NAPI.Vehicle.CreateVehicle(NAPI.Util.GetHashKey("adder"), new Vector3(-4, 15, 71), 0, 1, 1, "numberPlate", 0, true, true, 0);
            NAPI.Task.Run(() =>
            {
                veh.Delete();
                NAPI.Vehicle.CreateVehicle(NAPI.Util.GetHashKey("adder"), new Vector3(7, 15, 71), 0, 33, 33, "numberPlate", 0, true, true, 0);
                
            }, 1);
        }
        [Command("zvuk228")]
        public static void CMD_zvuk228(ExtPlayer player, int dist)
        {
            if (Main.ServerNumber != 0) return;
            Sounds.Play3d(player.Position, dist, "heliCrash", "cloud/sound/heliCrash.mp3", new SoundData
            {
                maxDistance = dist,
                fade = 1000,
                volume = 1f
            });
        }
    }
}