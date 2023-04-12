using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using NeptuneEvo.Accounts;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Handles;
using Redage.SDK;

namespace NeptuneEvo.MoneySystem
{
    public class DonatePack : Script
    {
        private static readonly nLog Log = new nLog("MoneySystem.DonatePack");
        
        class PackBuy
        {
            public int Id;
            public string Login;
            public int UUID;
            public DateTime Time;
        }
        
        class Pack
        {
            public int Id;
            public string Title;
            public int Donate;
            public int Money;

            public Pack(int id, string title, int donate, int money)
            {
                Id = id;
                Title = title;
                Donate = donate;
                Money = money;
            }
        }

        private static List<PackBuy> PacksBuy = new List<PackBuy>();

        private static List<Pack> Packs = new List<Pack>
        {
            new Pack(0, "Покупка BattlePass", BattlePass.Repository.PricePremium, 1999),
        };

        [RemoteEvent("server.donatepack.donate")]
        public void ConfirmDonate(ExtPlayer player, int id)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            var accountData = player.GetAccountData();
            if (accountData == null)
                return;

            var packBuy = PacksBuy.FirstOrDefault(pb => pb.Login.Equals(accountData.Login.ToLower()));

            if (packBuy != null)
                PacksBuy.Remove(packBuy);
            
            PacksBuy.Add(new PackBuy
            {
                Id = id,
                Login = accountData.Login.ToLower(),
                UUID = characterData.UUID,
                Time = DateTime.Now.AddMinutes(30)
            });
        }
        public static bool IsDonate(ExtPlayer player, string login, int money)
        {
            var packBuy = PacksBuy.FirstOrDefault(pb => pb.Login.Equals(login));

            if (packBuy != null)
            {
                var accountData = player.GetAccountData();
                if (accountData == null)
                    return false;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return false;

                if (characterData.UUID != packBuy.UUID) 
                    return false;
                
                var pack = Packs[packBuy.Id];
                
                if (pack.Money != money)
                    return false;
                
                PacksBuy.Remove(packBuy);
                
                Confirm(player, packBuy.Id, true);
                
                GameLog.AccountLog(accountData.Login, accountData.HWID, accountData.IP, accountData.SocialClub, pack.Title);
                
                return true;
            }

            return false;
        }
        public static bool IsDonate(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return false;
            
            var accountData = player.GetAccountData();
            if (accountData == null)
                return false;

            var packBuy = PacksBuy.FirstOrDefault(pb => pb.Login.Equals(accountData.Login.ToLower()));

            return packBuy != null;
        }
        
        public static void DeleteTime()
        {
            try
            {
                var packsBuy = PacksBuy
                    .Where(pb => DateTime.Now > pb.Time)
                    .ToList();

                foreach (var packBuy in packsBuy)
                {
                    if (PacksBuy.Contains(packBuy))
                        PacksBuy.Remove(packBuy);
                }
            }
            catch (Exception e)
            {
                Log.Write($"DeleteTime Exception: {e.ToString()}");
            }
        }
        
        [RemoteEvent("server.donatepack.rb")]
        public static void ConfirmRB(ExtPlayer player, int id)
        {
            if (!player.IsCharacterData())
                return;

            switch (id)
            {
                case 0:
                    Confirm(player, id);
                    break;
            }
        }
        public static void Confirm(ExtPlayer player, int id, bool isBuyDonate = false)
        {
            if (!player.IsCharacterData())
                return;

            switch (id)
            {
                case 0:
                    BattlePass.Repository.BuyPremium(player, isBuyDonate);
                    break;
            }
        }
        
        [RemoteEvent("server.donatepack.open")]
        public static void Open(ExtPlayer player, int id)
        {
            if (!player.IsCharacterData())
                return;

            var pack = Packs[id];
            
            Trigger.ClientEvent(player, "client.donatepack.show", pack.Id, pack.Title, pack.Donate, pack.Money);
        }
        
        
        
    }
}