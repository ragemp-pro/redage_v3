using System.Collections.Generic;
using GTANetworkAPI;
using NeptuneEvo.Core;
using NeptuneEvo.Handles;

namespace NeptuneEvo.PritonCode
{
    public class RadioPoints : Script
    {
        private static readonly Dictionary<Vector3, (string, float)> _radioPoints = new Dictionary<Vector3, (string, float)>()
        {
            [new Vector3(-1709.9996, -973.14764, 8.646633)] = ("http://radio-srv1.11one.ru/record192k.mp3", 40f),
            [new Vector3(-1497.6427, -1484.7079, 5.7574663)] = ("http://radio-srv1.11one.ru/record192k.mp3", 20f),
            [new Vector3(119.54091, -1287.8678, 28.267773)] = ("http://radio-srv1.11one.ru/record192k.mp3", 20f),
            [new Vector3(-555.4366, 286.321, 82.15151)] = ("http://radio-srv1.11one.ru/record192k.mp3", 20f),
            [new Vector3(-1393.5845, -613.67633, 30.81432)] = ("http://radio-srv1.11one.ru/record192k.mp3", 30f),
            [new Vector3(-440.06192, 272.55994, 83.01594)] = ("http://radio-srv1.11one.ru/record192k.mp3", 30f),
            [new Vector3(-159.20576, 293.2761, 93.76221)] = ("http://radio-srv1.11one.ru/record192k.mp3", 30f)
        };

        public RadioPoints()
        {
            foreach ((Vector3 position, (string radioUrl, float range)) in _radioPoints)
            {
                var colShape = NAPI.ColShape.CreateSphereColShape(position, range, 0);
                
                colShape.OnEntityEnterColShape += (shape, player) =>
                {
                    Sounds.Play2d((ExtPlayer)player, radioUrl);
                };
                colShape.OnEntityExitColShape += (shape, player) =>
                {
                    Sounds.Stop2d((ExtPlayer)player);
                };
            }
        }
    }
}