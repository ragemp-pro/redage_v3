using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using LinqToDB;
using NeptuneEvo.Character;
using NeptuneEvo.Handles;
using NeptuneEvo.Players.Phone.Contacts.Models;
using Redage.SDK;

namespace NeptuneEvo.Players.Phone.Contacts
{
    public class Repository
    {
        private static readonly nLog Log = new nLog("Players.Phone.Contacts.Repository");

        //
        
        public static void AddContact(ExtPlayer player, int number, string name, string avatar)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return;

            var phoneData = player.getPhoneData();
            if (phoneData == null) 
                return;

            var contacts = phoneData.PhoneContacts;
            
            BattlePass.Repository.UpdateReward(player, 76);

            if (avatar == null)
                avatar = String.Empty;
            
            if (!contacts.ContainsKey(number))
                contacts.Add(number, new PhoneContactsData
                {
                    Name = name,
                    Avatar = avatar
                });
        }

        //
        
        public static void UpdateContact(ExtPlayer player, int number, string name, string avatar)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return;

            var phoneData = player.getPhoneData();
            if (phoneData == null) 
                return;

            if (avatar == null)
                avatar = String.Empty;
            
            var contacts = phoneData.PhoneContacts;
            
            BattlePass.Repository.UpdateReward(player, 78);

            if (contacts.ContainsKey(number))
                contacts[number] = new PhoneContactsData
                {
                    Name = name,
                    Avatar = avatar
                };
        }

        //
        
        public static void DeleteContact(ExtPlayer player, int number)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return;

            var phoneData = player.getPhoneData();
            if (phoneData == null) 
                return;

            var contacts = phoneData.PhoneContacts;

            if (contacts.ContainsKey(number))
                contacts.Remove(number);
        }
    }
}