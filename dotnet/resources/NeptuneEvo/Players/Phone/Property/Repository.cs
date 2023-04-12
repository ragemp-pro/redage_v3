using System;
using System.Collections.Generic;
using NeptuneEvo.Character;
using NeptuneEvo.Handles;
using NeptuneEvo.Houses;
using Newtonsoft.Json;

namespace NeptuneEvo.Players.Phone.Property
{
    public class Repository
    {
        public static void LoadProperty(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null)
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            var propertyList = new List<List<object>>();
            
            var house = HouseManager.GetHouse(player);

            if (house != null)
            {
                propertyList.Add(new List<object>
                {
                    0,
                    house.Owner.Equals(sessionData.Name),
                    house.ID
                });
            }

            if (characterData.BizIDs.Count > 0)
            {
                foreach (var bizId in characterData.BizIDs)
                {
                    propertyList.Add(new List<object>
                    {
                        1,
                        true,
                        bizId,
                        
                    });
                }
            }
            
            Trigger.ClientEvent(player, "client.phone.propertyInit", JsonConvert.SerializeObject(propertyList));
        }
    }
}