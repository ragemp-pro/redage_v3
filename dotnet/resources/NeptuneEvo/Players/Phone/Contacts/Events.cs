using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Phone.Contacts
{
    public class Events : Script
    {
        [RemoteEvent("server.phone.addContact")]
        public void AddContact(ExtPlayer player, int number, string name, string avatar) => Repository.AddContact(player, number, name, avatar);
        
        [RemoteEvent("server.phone.updateContact")]
        public void UpdateContact(ExtPlayer player, int number, string name, string avatar) => Repository.UpdateContact(player, number, name, avatar);
        
        [RemoteEvent("server.phone.dellContact")]
        public void DeleteContact(ExtPlayer player, int number) => Repository.DeleteContact(player, number);
    }
}