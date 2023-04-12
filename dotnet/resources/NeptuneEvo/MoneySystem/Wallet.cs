using System;
using System.Globalization;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using MySqlConnector;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using Redage.SDK;

namespace NeptuneEvo.MoneySystem
{
    class Wallet : Script
    {
        private static readonly nLog Log = new nLog("MoneySystem.Wallet");

        public static bool Change(ExtPlayer player, int amount, bool isDisconnect = false)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return false;
                int temp = (int)characterData.Money + amount;
                if (temp < 0) return false;
                if (amount < 0 && Admin.IsServerStoping) return false;
                characterData.Money = temp;
                
                if (amount >= 10_000 || amount <= -10_000) 
                    Database.Models.Money.AddMoneyUpdate(characterData.UUID, characterData.Money);
                
                if (amount >= 1) characterData.EarnedMoney += (ulong)amount;
                if (isDisconnect) return true;
                Trigger.ClientEvent(player, "client.charStore.Money", temp);

                return true;
            }
            catch (Exception e)
            {
                Log.Write($"Change Exception: {e.ToString()}");
                return false;
            }
        }

        public static int GetPriceToVip(ExtPlayer player, int price)
        {
            
            var accountData = player.GetAccountData();
            if (accountData == null) 
                return price;
            
            switch (accountData.VipLvl)
            {
                case 0: // None
                case 1: // Bronze
                case 2: // Silver
                    return Convert.ToInt32(price * 0.4);
                case 3: // Gold
                    return Convert.ToInt32(price * 0.5);
                case 4: // Platinum
                case 5: // Media Platinum
                    return Convert.ToInt32(price * 0.6);
            }

            return price;
        }

        public static string Format(int Value)
        {
            if(Value >= -9 && Value <= 9) return Value.ToString();
            CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
            return Value.ToString("0,0", elGR);
        }

        public static string Format(long Value)
        {
            if (Value >= -9 && Value <= 9) return Value.ToString();
            CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
            return Value.ToString("0,0", elGR);
        }
        public static string Format(ulong Value)
        {
            if (Value <= 9) return Value.ToString();
            CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
            return Value.ToString("0,0", elGR);
        }
        public static string Format(uint Value)
        {
            if (Value <= 9) return Value.ToString();
            CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
            return Value.ToString("0,0", elGR);
        }
        public static string Format(short Value)
        {
            if (Value >= -9 && Value <= 9) return Value.ToString();
            CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
            return Value.ToString("0,0", elGR);
        }
        public static string Format(ushort Value)
        {
            if (Value <= 9) return Value.ToString();
            CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
            return Value.ToString("0,0", elGR);
        }
    }
}
