using System;
using Newtonsoft.Json;

namespace NeptuneEvo.Players.Phone.Settings.Models
{
    public class SettingsData
    {
        public string Avatar;
        public string Wallpaper = "https://cloud.redage.net/cloud/img/iphone/wallpapers/1.png";
        public int BellId = 0;
        public int SmsId = 0;
        public bool IsAir = false;
        public DateTime IsAirAntiFlood = DateTime.Now;
        public DateTime SimUpdateAntiFlood = DateTime.Now;
        public bool ForbesVisible = false;
    }
}