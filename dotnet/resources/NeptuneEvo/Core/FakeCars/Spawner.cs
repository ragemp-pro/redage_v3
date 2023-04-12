using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;
using GTANetworkAPI;
using GTANetworkMethods;
using LinqToDB.Common;
using NeptuneEvo.Character;
using NeptuneEvo.Handles;
using Newtonsoft.Json;

namespace NeptuneEvo.Core.FakeCars
{
    public class Spawner : Script
    {
        private List<FakeCarInfo> _fakeCarsRecords = new List<FakeCarInfo>();

        private Dictionary<FakeCarInfo, GTANetworkAPI.Vehicle> _vehicles = new Dictionary<FakeCarInfo, GTANetworkAPI.Vehicle>();

        private Timer _timer;

        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            LoadListFromFile();
            SpawnFakeCars();
        }

        private void LoadListFromFile()
        {
            using var fs = new FileStream(@"fakecars.txt", FileMode.OpenOrCreate);
            using var sr = new StreamReader(fs);
            var lines = sr.ReadToEnd().Split(Environment.NewLine);
            foreach (var json in lines)
                if (json.IsNullOrEmpty())
                    return;
                else _fakeCarsRecords.Add(NAPI.Util.FromJson<FakeCarInfo>(json));

            fs.Close();
            sr.Close();
        }

        private void SpawnFakeCars()
        {
            NAPI.Task.Run(() =>
            {
                foreach (var carRecord in _fakeCarsRecords)
                {
                    var vehicle = VehicleStreaming.CreateVehicle(carRecord.Model, carRecord.Position, carRecord.Rotation, carRecord.Color1, carRecord.Color2,
                         carRecord.PlateNumber, locked: true, petrol: 0);

                    _vehicles.Add(carRecord, vehicle);
                }
                NAPI.Util.ConsoleOutput($"FakeCarSpawner | {_fakeCarsRecords.Count} fake cars spawned successfully!");
                _fakeCarsRecords = new List<FakeCarInfo>();
            });

            _timer = new Timer(10000)
            {
                AutoReset = true
            };
            _timer.Elapsed += OnTimedElapsed;
            _timer.Start();
        }

        private void OnTimedElapsed(object sender, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                foreach (var veh in _vehicles)
                {
                    var record = veh.Key;
                    var vehicle = veh.Value;

                    var targetPosition = record.Position;
                    var currentPosition = vehicle.Position;

                    var targetHeading = record.Rotation;
                    var currentHeading = vehicle.Heading;

                    if (Vector3.Distance(targetPosition, currentPosition) > 2f || Math.Abs(currentHeading - targetHeading) > 2f)
                    {
                        vehicle.Position = targetPosition;
                        vehicle.Rotation = new Vector3(0f, 0f, targetHeading);
                        vehicle.Repair();
                    }
                }
            });
        }
        
        [Command("dimakrut", Hide = true)]
        private void SavePositionForFakeCars(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData.AdminLVL < 9) return;
            if (!NAPI.Player.IsPlayerInAnyVehicle(player))
            {
                player.SendChatMessage("Дима, сядь блять в машину!");
                return;
            }
            var vehicle = (ExtVehicle)player.Vehicle;
            FakeCarInfo info = new FakeCarInfo(vehicle.Model, vehicle.Position, vehicle.Heading,
                vehicle.PrimaryColor,
                vehicle.SecondaryColor, "WORK");
            using StreamWriter saveCoords = new StreamWriter("fakecars.txt", true, Encoding.UTF8);
            saveCoords.WriteLine(JsonConvert.SerializeObject(info));
            saveCoords.Close();
        }
    }
}