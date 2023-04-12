using System;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using Newtonsoft.Json;

namespace NeptuneEvo.Players.Phone.Messages
{
    public class Events : Script
    {
        
        [RemoteEvent("server.phone.getMsg")]
        public void getMessage(ExtPlayer player, int number) 
        {
            Trigger.SetTask(async () =>
            {
                var messages = await Repository.getMessage(player, number);
                
                Trigger.ClientEvent(player, "client.phone.setMsg", number, JsonConvert.SerializeObject(messages));
            });
        }

        [RemoteEvent("server.phone.sendMsg")]
        public void SendMessage(ExtPlayer player, int number, int key, string text, int type) =>
            Repository.SendMessage(player, number, key, text, type);
        
        [RemoteEvent("server.phone.sendSystemMsg")]
        public void SendSystemMessage(ExtPlayer player, int number, int key, string text, int type) =>
            Repository.SendSystemMessage(player, number, key, text, type);
        
        
        
        
        
        [RemoteEvent("server.phone.sNumber")]
        public void SelectedNumber(ExtPlayer player, int number) =>
            Repository.SelectedNumber(player, number);       
        
        [RemoteEvent("server.phone.getChatStatus")]
        public void GetChatStatus(ExtPlayer player, int number, bool status) =>
            Repository.GetChatStatus(player, number, status);      
        
        [RemoteEvent("server.phone.write")]
        public void UpdateWrite(ExtPlayer player, int number, bool toggled) =>
            Repository.UpdateWrite(player, number, toggled);   
    }
}