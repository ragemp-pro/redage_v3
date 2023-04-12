using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.World.Weather.Models;
using Newtonsoft.Json;

namespace NeptuneEvo.World.Weather
{
    public class Repository
    {

        private static IReadOnlyDictionary<int, List<int>> WeatherList = new Dictionary<int, List<int>>
        {
            {0, new List<int>{1, 2, 3, 5, 4}},
            {1, new List<int>{0, 2, 3, 5, 4}},
            {2, new List<int>{1, 5, 6, 7, 4}},
            {3, new List<int>{0, 1, 2, 3, 4}},
            {4, new List<int>{1, 2, 3, 5}},
            {5, new List<int>{1, 2, 3, 4, 5}},
            {6, new List<int>{ 8, 4, 2}},
            {7, new List<int>{ 8, 4, 2}},
            {8, new List<int>{ 1, 3, 2}},
            {10, new List<int>{11, 12}},
            {11, new List<int>{10, 12}},
            {12, new List<int>{10, 11}},
            {13, new List<int>{13}},
        };

        private static IReadOnlyDictionary<int, List<int>> WeatherTemp = new Dictionary<int, List<int>>
        {
            {0, new List<int>{24, 30}},
            {1, new List<int>{23, 29}},
            {2, new List<int>{22, 27}},
            {3, new List<int>{22, 27}},
            {4, new List<int>{21, 26}},
            {5, new List<int>{20, 25}},
            {6, new List<int>{19, 24}},
            {7, new List<int>{18, 23}},
            {8, new List<int>{20, 22}},
            {10, new List<int>{13, 18}},
            {11, new List<int>{12, 17}},
            {12, new List<int>{11, 16}},
            {13, new List<int>{10, 15}},
        };
        
        private static List<WeatherData> WeatherRandom = new List<WeatherData>();
        
        public static string WeatherJson = "";
        
        private static int MaxRandom = 8;
        private static int MaxTimeSeconds = 1800; // 30 мин 
        public static int NextTime = 0;

        private static void UpdateNextTime()
        {
            var dateTime = DateTime.UtcNow.AddSeconds(MaxTimeSeconds);            
            var minute = dateTime.Minute;
            
            if (minute >= 30)
                minute = 30;
            else
                minute = 0;
            
            dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, minute, 0);
            NextTime = (Int32)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        
        public static void Init(int defaultWeather, int hour, int minute)
        {
            if (minute >= 30)
                minute = 30;
            else
                minute = 0;

            if (!WeatherList.ContainsKey(defaultWeather))
                defaultWeather = 1;
            
            var rand = new Random();
            
            var weatherRandom = new List<WeatherData>();
            
            for (var i = 0; i < MaxRandom; i++)
            {
                var weather = WeatherList[defaultWeather];
                defaultWeather = GetRandomWeatherId(weather);
                
                var temp = WeatherTemp[defaultWeather];
                
                weatherRandom.Add(new WeatherData
                {
                    WeatherId = defaultWeather,
                    Hour = hour,
                    Minute = minute,
                    Temp = rand.Next(temp[0], temp[1])
                });
                minute += 30;
                
                if (minute > 30)
                {
                    if (++hour > 23)
                        hour = 0;
                    
                    minute = 0;
                }
            }
            
            var weatherData = weatherRandom[0];
            if (weatherData.WeatherId == (int)GTANetworkAPI.Weather.XMAS) 
                NAPI.World.SetWeather((GTANetworkAPI.Weather) weatherData.WeatherId); // FTW?

            WeatherRandom = weatherRandom;
            UpdateNextTime(); 
            Get();
        }

        private static void Get()
        {
            var weatherList = new List<List<object>>();
            
            foreach (var weatherData in WeatherRandom)
            {
                var minWeatherData = new List<object>();
                
                minWeatherData.Add(weatherData.WeatherId);
                minWeatherData.Add(weatherData.Hour);
                minWeatherData.Add(weatherData.Minute);
                minWeatherData.Add(weatherData.Temp);
                
                weatherList.Add(minWeatherData);
            }

            WeatherJson = JsonConvert.SerializeObject(weatherList);
        }
        private static IReadOnlyDictionary<int, int> WeatherRandomPercent = new Dictionary<int, int>
        {
            {0, 1500},
            {1, 1500},
            {2, 1500},
            {3, 250},
            {4, 250},
            {5, 1500},
            {6, 150},
            {7, 150},
            {8, 1500},
            {10, 1500},
            {11, 1500},
            {12, 1500},
            {13, 1500},
        };
        private static int GetRandomWeatherId(List<int> weatherList)
        {
            var maxCount = 0;
            foreach (var weatherId in weatherList)
                maxCount += WeatherRandomPercent[weatherId];

            var rand = new Random();
            var idItem = rand.Next(0, maxCount);

            maxCount = 0;
            foreach (var weatherId in weatherList)
            {
                maxCount += WeatherRandomPercent[weatherId];
                if (maxCount >= idItem)
                    return weatherId;
            }
            return 1;
        }
        public static void Set()
        {            
            var dateTime = DateTime.Now;
            var hour = dateTime.Hour;
            var minute = dateTime.Minute;
            var weatherData = WeatherRandom[1];

            if (weatherData != null && weatherData.Hour == hour && weatherData.Minute == minute)
            {
                if (new[] {10, 11, 12, 13}.Contains(WeatherRandom[1].WeatherId) == false && new[] {10, 11, 12, 13}.Contains(weatherData.WeatherId)) 
                    NAPI.World.SetWeather(GTANetworkAPI.Weather.CLEAR);
                
                //

                WeatherRandom.Remove(WeatherRandom[0]);
                
                //
                
                UpdateNextTime(); 
                
                Trigger.ClientEventForAll("SetWeather", WeatherRandom[0].WeatherId, WeatherRandom[1].WeatherId, NextTime);
                
                if (new[] {10, 11, 12, 13}.Contains(WeatherRandom[0].WeatherId)) 
                    NAPI.World.SetWeather(GTANetworkAPI.Weather.XMAS); 

                //

                var lastWeatherData = WeatherRandom.LastOrDefault();
                if (lastWeatherData != null)
                {
                    hour = lastWeatherData.Hour;
                    minute = lastWeatherData.Minute + 30;

                    if (minute > 30)
                    {
                        if (++hour > 23)
                            hour = 0;
                        
                        minute = 0;
                    }
                    
                    var rand = new Random();
                    
                    var weather = WeatherList[lastWeatherData.WeatherId];
                    var weatherId = GetRandomWeatherId(weather);
                    
                    var temp = WeatherTemp[lastWeatherData.WeatherId];
                    var tempCount = rand.Next(temp[0], temp[1]);
                    Trigger.ClientEventForAll("client.phone.addWeather", weatherId, hour, minute, tempCount);
                    
                    WeatherRandom.Add(new WeatherData
                    {
                        WeatherId = weatherId,
                        Hour = hour,
                        Minute = minute,
                        Temp = tempCount
                    });
                    
                    Get();
                }
                
                //
            }
        }
        
        private static int CustomWeather = -1;
        
        public static bool Update(int weatherId)
        {
            
            if (weatherId == -1 && CustomWeather == -1)
                return false;

            var weatherData = WeatherRandom[0];
            
            if (weatherId == -1 && CustomWeather != -1)
            {
                if (new[] {10, 11, 12, 13}.Contains(CustomWeather) && new[] {10, 11, 12, 13}.Contains(weatherData.WeatherId) == false) 
                    NAPI.World.SetWeather(GTANetworkAPI.Weather.CLEAR);
                
                CustomWeather = -1;
                
                Trigger.ClientEventForAll("SetWeatherCMD", -1);
                return true;
            }
            
            if (weatherId < 0 || weatherId > 14) 
                return false;
            
            if (new[] {10, 11, 12, 13}.Contains(CustomWeather) && new[] {10, 11, 12, 13}.Contains(weatherId) == false) 
                NAPI.World.SetWeather(GTANetworkAPI.Weather.XMAS);
            
            if (new[] {10, 11, 12, 13}.Contains(weatherId) && new[] {10, 11, 12, 13}.Contains(weatherData.WeatherId) == false) 
                NAPI.World.SetWeather(GTANetworkAPI.Weather.XMAS);
            
            //
            
            if (new[] {10, 11, 12, 13}.Contains(CustomWeather) && new[] {10, 11, 12, 13}.Contains(weatherId) == false) 
                NAPI.World.SetWeather(GTANetworkAPI.Weather.CLEAR);
            
            Trigger.ClientEventForAll("SetWeatherCMD", weatherId);
            CustomWeather = weatherId;

            return true;
        }

        public static void Init(ExtPlayer player)
        {
            Trigger.ClientEvent(player, "DateTime", JsonConvert.SerializeObject(Main.ServerDateTime));
            
            if (CustomWeather != -1) 
                Trigger.ClientEvent(player, "SetWeatherCMD", CustomWeather);
            
            Trigger.ClientEvent(player, "SetWeather", WeatherRandom[0].WeatherId, WeatherRandom[1].WeatherId, NextTime);
        }
    }
}