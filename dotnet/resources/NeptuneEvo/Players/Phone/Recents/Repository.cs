using System;
using System.Linq;
using NeptuneEvo.Handles;
using NeptuneEvo.Players.Phone.Recents.Models;
using Newtonsoft.Json;

namespace NeptuneEvo.Players.Phone.Recents
{
    public class Repository
    {
        private static int MaxRecent = 35;
        
        public static void Add(ExtPlayer player, int number, bool isCall, DateTime time, int duration = -1)
        {
            var phoneData = player.getPhoneData();
            if (phoneData == null || phoneData.Recents == null) 
                return;

            var recent = phoneData.Recents.FirstOrDefault(r => r.Number == number);
            if (recent != null)
                phoneData.Recents.Remove(recent);
            
            phoneData.Recents.Add(new RecentsData
            {
                Number = number,
                IsCall = isCall,
                Time = time,
                Duration = duration
            });

            if (phoneData.Recents.Count >= MaxRecent)
                phoneData.Recents.RemoveAt(0);
            
            Trigger.ClientEvent(player,"client.phone.addRecent", number, isCall, JsonConvert.SerializeObject(time), duration);
        }
        
        public static void UpdateEnd(ExtPlayer player)
        {
            var phoneData = player.getPhoneData();
            if (phoneData == null || phoneData.Recents == null || phoneData.Recents.Count == 0) 
                return;

            var recents = phoneData.Recents.LastOrDefault();
            
            if (recents == null) 
                return;

            recents.Duration = 0;
            Trigger.ClientEvent(player,"client.phone.updateRecent", 0);
        }
        
        public static void UpdateConfirm(ExtPlayer player, DateTime time)
        {
            var phoneData = player.getPhoneData();
            if (phoneData == null || phoneData.Recents == null || phoneData.Recents.Count == 0) 
                return;

            var recents = phoneData.Recents.LastOrDefault();
            
            if (recents == null) 
                return;

            var duration = Convert.ToInt32(TimeSpan.FromTicks(time.Ticks - recents.Time.Ticks).TotalSeconds);

            recents.Duration = duration;
            Trigger.ClientEvent(player,"client.phone.updateRecent", duration);
        }
        
        public static void Clear(ExtPlayer player)
        {
            var phoneData = player.getPhoneData();
            if (phoneData == null) 
                return;

            phoneData.Recents.Clear();
        }
    }
}