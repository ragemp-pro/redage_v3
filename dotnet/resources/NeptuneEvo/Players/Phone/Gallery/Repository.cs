using System;
using System.Collections.Generic;
using System.Linq;
using NeptuneEvo.Handles;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Players.Phone.Gallery
{
    public class Repository
    {
        private static int MaxGallery = 25;
        
        public static void AddGallery(ExtPlayer player, string link)
        {
            var phoneData = player.getPhoneData();
            if (phoneData == null) 
                return;
            
            if (phoneData.Gallery.Count >= MaxGallery)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "На телефоне нет места", 3000);
                return;
            }
            

            var newPhoto = new List<object>
            {
                link,
                DateTime.Now
            };
            
            phoneData.Gallery.Add(newPhoto);
            
            Trigger.ClientEvent(player, "client.phone.pushGallery", JsonConvert.SerializeObject(newPhoto));

        }
        public static void DellGallery(ExtPlayer player, string link)
        {
            var phoneData = player.getPhoneData();

            var photo = phoneData?.Gallery.FirstOrDefault(g => g[0].Equals(link));

            if (photo != null && phoneData.Gallery.Contains(photo))
                phoneData.Gallery.Remove(photo);

        }
    }
}