using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using LinqToDB;
using NeptuneEvo.Character;
using NeptuneEvo.Handles;
using Redage.SDK;

namespace NeptuneEvo.Players.Phone.BlackList
{
    public class Repository
    {
        private static readonly nLog Log = new nLog("Players.Phone.BlackList.Repository");
        
        public static void AddBlackList(ExtPlayer player, int number)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return;

            var phoneData = player.getPhoneData();
            if (phoneData == null) 
                return;

            var balckList = phoneData.BlackList;

            if (!balckList.Contains(number))
                balckList.Add(number);
        }
        
        public static void DeleteBlackList(ExtPlayer player, int number)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return;

            var phoneData = player.getPhoneData();
            if (phoneData == null) 
                return;

            var blackList = phoneData.BlackList;

            if (blackList.Contains(number))
                blackList.Remove(number);
        }
    }
}