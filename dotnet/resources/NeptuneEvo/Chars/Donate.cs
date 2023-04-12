using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Accounts;

using NeptuneEvo.Players.Models;

using NeptuneEvo.Players;

using NeptuneEvo.Character.Models;

using NeptuneEvo.Character;

using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;

using NeptuneEvo.MoneySystem;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GTANetworkMethods;
using Localization;
using NeptuneEvo.Functions;
using NeptuneEvo.Players.Phone.Messages.Models;

namespace NeptuneEvo.Chars
{
    class Donate : Script
    {
        private static readonly nLog Log = new nLog("Chars.Donate");

        public static string SetUnique(string Unique)
        {
            if (Unique == null || Unique.Split('_').Length < 2)
            {
                Random rand = new Random();
                string category = "packages";
                int index = rand.Next(0, 10);
                /*if (rand.Next(0, 100) > 50)
                {
                    category = "cases";
                    index = rand.Next(3, 9);
                }*/
                
                
                category = "cases";
                index = rand.Next(3, 9);

                Unique = $"{category}_{index}_0";
            }

            return Unique;
        }

        public static int GetPrice(string Unique, string category, int index, int price)
        {
            try
            {
                if (Unique != null && Unique.Split('_').Length > 1 && Unique.Split("_")[0] == category &&
                    Convert.ToInt32(Unique.Split("_")[1]) == index && Convert.ToInt32(Unique.Split("_")[2]) == 0)
                {
                    price = Convert.ToInt32(price * 0.7);
                }

                return price;
            }
            catch (Exception e)
            {
                Log.Write($"GetPrice Exception: {e.ToString()}");
                return price;
            }
        }

        public static void UpdatePrice(ExtPlayer player, string category, int index, int price)
        {
            try
            {

                var accountData = player.GetAccountData();

                if (accountData == null) return;

                if (accountData.Unique != null && accountData.Unique.Split('_').Length > 1 &&
                    accountData.Unique.Split("_")[0] == category &&
                    Convert.ToInt32(accountData.Unique.Split("_")[1]) == index &&
                    Convert.ToInt32(accountData.Unique.Split("_")[2]) == 0)
                {
                    accountData.Unique = $"{accountData.Unique.Split("_")[0]}_{accountData.Unique.Split("_")[1]}_1";
                    Trigger.ClientEvent(player, "client.accountStore.Unique", accountData.Unique, true);
                    price = Convert.ToInt32(price * 0.7);
                }

                UpdateData.RedBucks(player, price, LangFunc.GetText(LangType.Ru, DataName.DonShopBought));
            }
            catch (Exception e)
            {
                Log.Write($"GetPrice Exception: {e.ToString()}");
                return;
            }
        }

        public static DonatePremiumData[] DonatesPremiumData = {
            new DonatePremiumData
            {
                Price = 15000,
                GiveRb = 150,
                GiveMoney = 20000
            },
            new DonatePremiumData
            {
                Price = 1500,
                GiveRb = 0,
                GiveMoney = 50
            },
            new DonatePremiumData
            {
                Price = 3500,
                GiveRb = 0,
                GiveMoney = 70
            },
            new DonatePremiumData
            {
                Price = 5000,
                GiveRb = 0,
                GiveMoney = 110
            },
            new DonatePremiumData
            {
                Price = 10000,
                GiveRb = 0,
                GiveMoney = 200
            },
        };

        public static int AddPayment(int id)
        {
            if (id == 0 || id >= DonatesPremiumData.Length)
                return 0;

            return DonatesPremiumData[id].GiveMoney;
        }

        [RemoteEvent("server.donate.buy.premium")]
        public void BuyPremium(ExtPlayer player, int id)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                var accountData = player.GetAccountData();

                if (accountData == null) return;

                switch (id)

                {

                    case 0:// Подписка

                        if (accountData.isSubscribe)

                        {

                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SubAlreadyBuy), 3000);

                            return;

                        }

                        int price = DonatesPremiumData[0].Price;

                        if (accountData.RedBucks < price)

                        {

                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);

                            return;

                        }

                        accountData.isSubscribe = true;

                        accountData.SubscribeEndTime = DateTime.Now.AddDays(30);

                        UpdateData.RedBucks(player, -price, LangFunc.GetText(LangType.Ru, DataName.SubBought));

                        RewardSubscribe(player);

                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouBoughtSub), 3000);

                        break;

                    case 1:// Type.SilverVIP:

                        if (accountData.VipLvl >= 1)

                        {

                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyBoughtVIP), 3000);

                            return;

                        }

                        if (accountData.RedBucks < DonatesPremiumData[1].Price)

                        {

                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);

                            return;

                        }

                        UpdateData.RedBucks(player, -DonatesPremiumData[1].Price, msg: LangFunc.GetText(LangType.Ru, DataName.BoughtSilverVIP));

                        accountData.VipLvl = 1;

                        accountData.VipDate = DateTime.Now.AddDays(30);

                        //Repository.PlayerStats(player);

                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BuySilverVIP), 3000);

                        break;

                    case 2:// Type.GoldVIP:

                        if (accountData.VipLvl >= 1)

                        {

                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyBoughtVIP), 3000);

                            return;

                        }

                        if (accountData.RedBucks < DonatesPremiumData[2].Price)

                        {

                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);

                            return;

                        }

                        UpdateData.RedBucks(player, -DonatesPremiumData[2].Price, msg: LangFunc.GetText(LangType.Ru, DataName.BoughtGoldVIP));

                        accountData.VipLvl = 2;

                        accountData.VipDate = DateTime.Now.AddDays(30);

                        //Repository.PlayerStats(player);

                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BuyGoldVIP), 3000);

                        break;

                    case 3:// Type.PlatinumVIP:

                        if (accountData.VipLvl >= 1)

                        {

                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyBoughtVIP), 3000);

                            return;

                        }

                        if (accountData.RedBucks < DonatesPremiumData[3].Price)

                        {

                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);

                            return;

                        }

                        UpdateData.RedBucks(player, -DonatesPremiumData[3].Price, msg: LangFunc.GetText(LangType.Ru, DataName.BoughtPlatinumVIP));

                        accountData.VipLvl = 3;

                        accountData.VipDate = DateTime.Now.AddDays(30);

                        //Repository.PlayerStats(player);

                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BuyPlatinumVIP), 3000);

                        break;

                    case 4:// Type.DiamondVIP:

                        if (accountData.VipLvl >= 1)

                        {

                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyBoughtVIP), 3000);

                            return;

                        }

                        if (accountData.RedBucks < DonatesPremiumData[4].Price)

                        {

                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);

                            return;

                        }

                        UpdateData.RedBucks(player, -DonatesPremiumData[4].Price, LangFunc.GetText(LangType.Ru, DataName.BoughtDiamondVIP));

                        accountData.VipLvl = 4;

                        accountData.VipDate = DateTime.Now.AddDays(30);

                        //Repository.PlayerStats(player);

                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BuyDiamondVIP), 3000);

                        break;
                }

            }

            catch (Exception e)

            {

                Log.Write($"BuyPremium Exception: {e.ToString()}");

            }            
        }
        [RemoteEvent("server.donate.reward")]
        public void RewardSubscribe(ExtPlayer player)
        {
            try

            {

                var accountData = player.GetAccountData();

                if (accountData == null) return;

                if (!accountData.isSubscribe)

                {

                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSubscription), 3000);

                    return;

                }

                else if (accountData.SubscribeTime > DateTime.Now)

                {

                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Only24hPrize), 3000);

                    return;

                }

                var characterData = player.GetCharacterData();

                if (characterData == null) return;

                int moneyCash = DonatesPremiumData[0].GiveMoney;

                Wallet.Change(player, moneyCash);

                GameLog.Money($"system", $"player({characterData.UUID})", moneyCash, $"rewardSubscribe");



                int donateCash = DonatesPremiumData[0].GiveRb;

                UpdateData.RedBucks(player, donateCash, msg: LangFunc.GetText(LangType.Ru, DataName.SubBonus));

                accountData.SubscribeTime = DateTime.Now.AddDays(1);

                Repository.AddNewItemWarehouse(player, ItemId.Case5, 1);

                Trigger.ClientEvent(player, "client.accountStore.Subscribe", JsonConvert.SerializeObject(accountData.SubscribeTime));

            }

            catch (Exception e)

            {

                Log.Write($"RewardSubscribe Exception: {e.ToString()}");

            }
        }

        [RemoteEvent("server.donate.load")]
        public void LoadPack(ExtPlayer player)
        {
            if (!player.IsCharacterData()) return;
            
            Trigger.ClientEvent(player, "client.donate.init", 
                JsonConvert.SerializeObject(DonatesPremiumData), 
                JsonConvert.SerializeObject(Main.DonatePack.PriceRB), JsonConvert.SerializeObject(Main.DonatePack.List),
                Chars.Repository.RoulleteCategoryListJson, JsonConvert.SerializeObject(Repository.RouletteCasesDataJson));
        }
        
        [RemoteEvent("server.donate.buy.set")]
        public void BuySet(ExtPlayer player, int id)
        {

            try

            {

                var accountData = player.GetAccountData();

                if (accountData == null) return;

                var characterData = player.GetCharacterData();

                if (characterData == null) return;

                switch (id)

                {

                    case 0: //Type.BagOnFoot:

                        {



                            if (accountData.RedBucks < GetPrice(accountData.Unique, "packages", id, Main.DonatePack.PriceRB[0]))

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);

                                return;

                            }
                             if (Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.CarCoupon, 1, "Casco") == -1)

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);

                                return;

                            }
                            
                            if (Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Case3, 2) == -1) // КЕЙС

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);

                                return;

                            }

                            UpdatePrice(player, "packages", id, -Main.DonatePack.PriceRB[0]);

                            Wallet.Change(player, Main.DonatePack.GiveMoney[0]);

                            if (accountData.VipLvl != 2) accountData.VipDate = DateTime.Now.AddDays(5);

                            else accountData.VipDate = accountData.VipDate.AddDays(5);

                            accountData.VipLvl = 2;

                            UpdateData.Exp(player, 30);

                            characterData.Licenses[1] = true; // легковой

                            characterData.Licenses[7] = true; // мед

                            //Repository.PlayerStats(player);

                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NaHodDonPack, Main.DonatePack.PriceRB[0]), 3000);

                            GameLog.AccountLog(accountData.Login, accountData.HWID, accountData.IP, accountData.SocialClub, LangFunc.GetText(LangType.Ru, DataName.NaHodDonPack, Main.DonatePack.PriceRB[0]));

                            break;

                        }

                    case 1: //Type.BagStart:

                        {

                            if (accountData.RedBucks < GetPrice(accountData.Unique, "packages", id, Main.DonatePack.PriceRB[1]))

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);

                                return;

                            }
                             if (Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Mask, 1, "208_0_True") == -1)

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceForAccessory), 3000);

                                return;

                            }

                            UpdatePrice(player, "packages", id, -Main.DonatePack.PriceRB[1]);

                            Wallet.Change(player, Main.DonatePack.GiveMoney[1]);

                            if (accountData.VipLvl != 2) accountData.VipDate = DateTime.Now.AddDays(7); 

                            else accountData.VipDate = accountData.VipDate.AddDays(7);

                            accountData.VipLvl = 2;
                            
                            characterData.Licenses[1] = true; // легковой

                            characterData.Licenses[7] = true; // мед
                            
                            if (Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.CarCoupon, 1, "Iwagen") == -1)

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);

                                return;

                            }
                            
                            if (Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Case4, 5) == -1) // КЕЙС

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);

                                return;

                            }

                            UpdateData.Exp(player, 35);

                            //Repository.PlayerStats(player);

                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.StarterDonPack, Main.DonatePack.PriceRB[1]), 3000);

                            GameLog.AccountLog(accountData.Login, accountData.HWID, accountData.IP, accountData.SocialClub, LangFunc.GetText(LangType.Ru, DataName.StarterDonPack, Main.DonatePack.PriceRB[1]));

                            break;

                        }

                    case 2: //Type.BagAdvanced:

                        {

                            if (accountData.RedBucks < GetPrice(accountData.Unique, "packages", id, Main.DonatePack.PriceRB[2]))

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);

                                return;

                            }
                            
                            if (Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.CarCoupon, 1, "MarkII") == -1)

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);

                                return;

                            }
                             if (Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Mask, 1, "197_1_True") == -1)

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceForAccessory), 3000);

                                return;

                            }
                            
                            if (Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Case5, 5) == -1) // КЕЙС ПОТОМ ГЛЯНУТЬ

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);

                                return;

                            }
                            
                            

                            UpdatePrice(player, "packages", id, -Main.DonatePack.PriceRB[2]);

                            Wallet.Change(player, Main.DonatePack.GiveMoney[2]);

                            if (accountData.VipLvl != 3) accountData.VipDate = DateTime.Now.AddDays(2);

                            else accountData.VipDate = accountData.VipDate.AddDays(2);

                            accountData.VipLvl = 3;
                            
                            

                            characterData.Licenses[1] = true; // легковой

                            characterData.Licenses[7] = true; // мед
                            
                            characterData.Licenses[6] = true; // gun lic

                           

                            UpdateData.Exp(player, 40);

                            //Repository.PlayerStats(player);

                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PromotedDonPack, Main.DonatePack.PriceRB[2]), 3000);

                            GameLog.AccountLog(accountData.Login, accountData.HWID, accountData.IP, accountData.SocialClub, LangFunc.GetText(LangType.Ru, DataName.PromotedDonPack, Main.DonatePack.PriceRB[2]));

                            break;

                        }

                    case 3: //Type.BagCharged:

                        {

                            if (accountData.RedBucks < GetPrice(accountData.Unique, "packages", id, Main.DonatePack.PriceRB[3]))

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);

                                return;

                            }

                            if (Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.CarCoupon, 1, "Jeepsrt8") == -1)

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);

                                return;

                            }
                            
                             if (Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Mask, 1, "213_0_True") == -1)

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceForAccessory), 3000);

                                return;

                            }
                            if (Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Case6, 2) == -1) // КЕЙС ПОТОМ ГЛЯНУТЬ

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);

                                return;

                            }
                        

                            UpdatePrice(player, "packages", id, -Main.DonatePack.PriceRB[3]);

                            Wallet.Change(player, Main.DonatePack.GiveMoney[3]);

                            if (accountData.VipLvl != 3) accountData.VipDate = DateTime.Now.AddDays(5); // ВИПКУ ГЛЯНУТЬ

                            else accountData.VipDate = accountData.VipDate.AddDays(5);

                            accountData.VipLvl = 3;
                            


                            characterData.Licenses[4] = true; // helicopter
                    
                            characterData.Licenses[5] = true; // самолет
                            
                            characterData.Licenses[6] = true; // gun lic

     

                            UpdateData.Exp(player, 70);

                            //Repository.PlayerStats(player);

                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LoadedDonPack, Main.DonatePack.PriceRB[3]), 3000);

                            GameLog.AccountLog(accountData.Login, accountData.HWID, accountData.IP, accountData.SocialClub, LangFunc.GetText(LangType.Ru, DataName.LoadedDonPack, Main.DonatePack.PriceRB[3]));

                            break;

                        }

                    case 4: //Type.BagGoodStart:

                        {

                            if (accountData.RedBucks < GetPrice(accountData.Unique, "packages", id, Main.DonatePack.PriceRB[4]))

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);

                                return;

                            }

                            int vehiclesCount = VehicleManager.GetVehiclesCarCountToPlayer(player.Name);

                            if (vehiclesCount >= Houses.GarageManager.MaxGarageCars)

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxCarsDonPack), 6000);

                                return;

                            }

                            var house = Houses.HouseManager.GetHouse(player, true);

                            if (house != null)
                            {
                                var garage = house.GetGarageData();
                                if (garage == null || vehiclesCount >= Houses.GarageManager.GarageTypes[garage.Type].MaxCars)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxCarsDonPack), 3000);
                                    return;
                                }
                            }

                            UpdatePrice(player, "packages", id, -Main.DonatePack.PriceRB[4]);

                            VehicleManager.Create(player, "Komoda", new Color(225, 225, 225), new Color(225, 225, 225));

                            Wallet.Change(player, Main.DonatePack.GiveMoney[4]);

                            if (accountData.VipLvl != 3) accountData.VipDate = DateTime.Now.AddDays(30);

                            else accountData.VipDate = accountData.VipDate.AddDays(30);

                            accountData.VipLvl = 3;

                            UpdateData.Exp(player, 35);

                            //Repository.PlayerStats(player);

                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GoodStartDonPack, Main.DonatePack.PriceRB[4]), 3000);

                            GameLog.AccountLog(accountData.Login, accountData.HWID, accountData.IP, accountData.SocialClub, LangFunc.GetText(LangType.Ru, DataName.GoodStartDonPack, Main.DonatePack.PriceRB[4]));

                            break;

                        }

                    case 5: //Type.BagBigPlans:

                        {

                            if (accountData.RedBucks < GetPrice(accountData.Unique, "packages", id, Main.DonatePack.PriceRB[5]))

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);

                                return;

                            }

                            UpdatePrice(player, "packages", id, -Main.DonatePack.PriceRB[5]);

                            Wallet.Change(player, Main.DonatePack.GiveMoney[5]);

                            if (accountData.VipLvl != 3) accountData.VipDate = DateTime.Now.AddDays(45);

                            else accountData.VipDate = accountData.VipDate.AddDays(45);

                            accountData.VipLvl = 3;

                            characterData.Licenses[4] = true;

                            characterData.Licenses[5] = true;

                            UpdateData.Exp(player, 30);

                            //Repository.PlayerStats(player);

                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BigPlansDonPack, Main.DonatePack.PriceRB[5]), 3000);

                            GameLog.AccountLog(accountData.Login, accountData.HWID, accountData.IP, accountData.SocialClub, LangFunc.GetText(LangType.Ru, DataName.BigPlansDonPack, Main.DonatePack.PriceRB[5]));

                            break;

                        }

                    case 6: //Type.BagFatCheck:

                        {

                            if (accountData.RedBucks < GetPrice(accountData.Unique, "packages", id, Main.DonatePack.PriceRB[6]))

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);

                                return;

                            }
                            
                            if (Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Case6, 5) == -1) 

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);

                                return;

                            }

                             if (Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Bag, 1, "92_0_True") == -1)

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceForAccessory), 3000);

                                return;

                            }
                            
                            if (Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.CarCoupon, 1, "MLEvoX") == -1)

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);

                                return;

                            }

                            UpdatePrice(player, "packages", id, -Main.DonatePack.PriceRB[6]);

                            Wallet.Change(player, Main.DonatePack.GiveMoney[6]);

                            if (accountData.VipLvl != 3) accountData.VipDate = DateTime.Now.AddDays(7); 

                            else accountData.VipDate = accountData.VipDate.AddDays(7);

                            accountData.VipLvl = 3;

                      

                            characterData.Licenses[4] = true; // helicopter
                    
                            characterData.Licenses[5] = true; // самолет
                            
                            characterData.Licenses[6] = true; // gun lic

                            characterData.Licenses[8] = true; // paramedic
                            

                            UpdateData.Exp(player, 150);

                            //Repository.PlayerStats(player);

                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BigCheckDonPack, Main.DonatePack.PriceRB[6]), 3000);

                            GameLog.AccountLog(accountData.Login, accountData.HWID, accountData.IP, accountData.SocialClub, LangFunc.GetText(LangType.Ru, DataName.BigCheckDonPack, Main.DonatePack.PriceRB[6]));

                            break;

                        }

                    case 7: //Type.BagExcellentStart:

                        {

                            if (accountData.RedBucks < GetPrice(accountData.Unique, "packages", id, Main.DonatePack.PriceRB[7]))
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);
                                return;
                            }

                            int vehiclesCount = VehicleManager.GetVehiclesCarCountToPlayer(player.Name);

                            if (vehiclesCount >= Houses.GarageManager.MaxGarageCars)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxCarsDonPack), 6000);
                                return;
                            }

                            var house = Houses.HouseManager.GetHouse(player, true);

                            if (house != null)
                            {
                                var garage = house.GetGarageData();

                                if (garage == null || vehiclesCount >= Houses.GarageManager.GarageTypes[garage.Type].MaxCars)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxCarsDonPack), 3000);
                                    return;
                                }
                            }

                            UpdatePrice(player, "packages", id, -Main.DonatePack.PriceRB[7]);

                            VehicleManager.Create(player, "Pariah", new Color(225, 225, 225), new Color(225, 225, 225));

                            Wallet.Change(player, Main.DonatePack.GiveMoney[7]);

                            if (accountData.VipLvl != 3) accountData.VipDate = DateTime.Now.AddDays(60);

                            else accountData.VipDate = accountData.VipDate.AddDays(60);

                            accountData.VipLvl = 3;

                            UpdateData.Exp(player, 45);

                            //Repository.PlayerStats(player);

                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.GreatDonPack, Main.DonatePack.PriceRB[7]), 3000);

                            GameLog.AccountLog(accountData.Login, accountData.HWID, accountData.IP, accountData.SocialClub, LangFunc.GetText(LangType.Ru, DataName.GreatDonPack, Main.DonatePack.PriceRB[7]));

                            break;

                        }

                    case 8: //Type.BagLegend:

                        {

                            if (accountData.RedBucks < GetPrice(accountData.Unique, "packages", id, Main.DonatePack.PriceRB[8]))

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);

                                return;

                            }

                            
                            if (Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.CarCoupon, 1, "DodgeViper") == -1)

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);

                                return;

                            }
                            
                            if (Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Bag, 1, "118_0_True") == -1)

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceForAccessory), 3000);

                                return;

                            }
                            
                            if (Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Case7, 5) == -1)

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);

                                return;

                            }

                            UpdatePrice(player, "packages", id, -Main.DonatePack.PriceRB[8]);
                            

                            Wallet.Change(player, Main.DonatePack.GiveMoney[8]);

                            if (accountData.VipLvl != 3) accountData.VipDate = DateTime.Now.AddDays(15);

                            else accountData.VipDate = accountData.VipDate.AddDays(15);
                            
                           
                           characterData.Licenses[4] = true; // helicopter
                    
                            characterData.Licenses[5] = true; // самолет
                            
                            characterData.Licenses[6] = true; // gun lic

                            characterData.Licenses[8] = true; // paramedic

                            accountData.VipLvl = 3;

                            
                            var number = Players.Phone.Sim.Repository.GenerateSimCard(10000, 99999);
                            
                            Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.SimCard, 1, number.ToString());
                            
                            UpdateData.Exp(player, 230);

                            //Repository.PlayerStats(player);

                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LegendPackDon, Main.DonatePack.PriceRB[8]), 3000);

                            GameLog.AccountLog(accountData.Login, accountData.HWID, accountData.IP, accountData.SocialClub, LangFunc.GetText(LangType.Ru, DataName.LegendPackDon, Main.DonatePack.PriceRB[8]));

                            break;

                        }

                    case 9: //Type.BagPerfectStart:

                        {

                            if (accountData.RedBucks < GetPrice(accountData.Unique, "packages", id, Main.DonatePack.PriceRB[9]))

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);

                                return;

                            }

                            int vehiclesCount = VehicleManager.GetVehiclesCarCountToPlayer(player.Name);

                            if (vehiclesCount >= Houses.GarageManager.MaxGarageCars)

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxCarsDonPack), 6000);

                                return;

                            }

                            var house = Houses.HouseManager.GetHouse(player, true);

                            if (house != null)

                            {

                                var garage = house.GetGarageData();

                                if (garage == null || vehiclesCount >= Houses.GarageManager.GarageTypes[garage.Type].MaxCars)

                                {

                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxCarsDonPack), 3000);

                                    return;

                                }

                            }

                            UpdatePrice(player, "packages", id, -Main.DonatePack.PriceRB[9]);

                            VehicleManager.Create(player, "Neon", new Color(225, 225, 225), new Color(225, 225, 225));

                            var number = Players.Phone.Sim.Repository.GenerateSimCard(10000, 99999);

                            Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.SimCard, 1, number.ToString());

                            Wallet.Change(player, Main.DonatePack.GiveMoney[9]);

                            if (accountData.VipLvl != 3) accountData.VipDate = DateTime.Now.AddDays(120);

                            else accountData.VipDate = accountData.VipDate.AddDays(120);

                            accountData.VipLvl = 3;

                            UpdateData.Exp(player, 60);

                            //Repository.PlayerStats(player);

                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.BuyDonateGreatStart, Main.DonatePack.PriceRB[9]), 3000);

                            GameLog.AccountLog(accountData.Login, accountData.HWID, accountData.IP, accountData.SocialClub, LangFunc.GetText(LangType.Ru, DataName.BuyDonateGreatStart, Main.DonatePack.PriceRB[9]));

                            break;

                        }

                    case 10: //Type.BagDaddy: +

                        {

                            if (accountData.RedBucks < GetPrice(accountData.Unique, "packages", id, Main.DonatePack.PriceRB[10]))

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);

                                return;

                            }

                            if (Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Hat, 1, "210_0_True") == -1)

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceForAccessory), 3000);

                                return;

                            }
                            
                            if (Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.CarCoupon, 1, "Lambo580") == -1)

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);

                                return;

                            }

                            UpdatePrice(player, "packages", id, -Main.DonatePack.PriceRB[10]);

                            Wallet.Change(player, Main.DonatePack.GiveMoney[10]);

                            if (accountData.VipLvl != 3) accountData.VipDate = DateTime.Now.AddDays(30);  
                            else accountData.VipDate = accountData.VipDate.AddDays(30);

                            accountData.VipLvl = 3;
                            
                            var number = Players.Phone.Sim.Repository.GenerateSimCard(1000, 9999);

                            Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.SimCard, 1, number.ToString());

                            characterData.Licenses[4] = true; // helicopter
                    
                            characterData.Licenses[5] = true; // самолет
                            
                            characterData.Licenses[6] = true; // gun lic

                            characterData.Licenses[8] = true; // paramedic


                            UpdateData.Exp(player, 350);
 
                            if (Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Case14, 2) == -1)

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);

                                return;

                            }
                            if (Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Case15, 1) == -1)

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);

                                return;

                            }

                            //Repository.PlayerStats(player);

                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно приобрели набор «Батя»!", 3000);

                            GameLog.AccountLog(accountData.Login, accountData.HWID, accountData.IP, accountData.SocialClub, $"Куплен набор «Батя» (-{Main.DonatePack.GiveMoney[10]} RedBucks)");

                            break;

                        }

                    default:

                        // Not supposed to end up here. 
                        break;

                }

            }

            catch (Exception e)

            {

                Log.Write($"BuySet Exception: {e.ToString()}");

            }            
        }
        [RemoteEvent("server.donate.buy.char")]
        public void BuyChar(ExtPlayer player, int id, string name)
        {
            try

            {

                var accountData = player.GetAccountData();

                if (accountData == null) 

                    return;



                var characterData = player.GetCharacterData();

                if (characterData == null) 

                    return;



                switch (id)

                {

                    case 1:
                        
                        var sessionData = player.GetSessionData();
                        if (sessionData == null) return;
                        if (!player.IsCharacterData()) return;
                        if (sessionData.CuffedData.Cuffed)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsCuffed), 3000);
                            return;
                        }
                        if (sessionData.DeathData.InDeath)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsDying), 3000);
                            return;
                        }

                        if (accountData.RedBucks < 1000)

                        {

                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);

                            return;

                        }

                        if (characterData.DemorganTime != 0 || characterData.ArrestTime != 0)

                        {

                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoCreatorDemorgan), 5000);

                            return;

                        }

                        UpdateData.RedBucks(player, -1000, msg:LangFunc.GetText(LangType.Ru, DataName.SendCreator));

                        Character.Friend.Repository.ClearFriends(player, player.Name);

                        Customization.SendToCreator(player);

                        break;

                    case 0:

                        if (accountData.RedBucks < 800)

                        {

                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);

                            return;

                        }



                        if (!Main.PlayerNames.Values.Contains(player.Name)) return;

                        try

                        {

                            string[] split = name.Split("_");

                            Log.Debug($"SPLIT: {split[0]} {split[1]}");



                            if (split[0] == "null" || string.IsNullOrEmpty(split[0]))

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoName), 3000);

                                return;

                            }

                            else if (split[1] == "null" || string.IsNullOrEmpty(split[1]))

                            {

                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoFamily), 3000);

                                return;

                            }

                        }

                        catch (Exception e)

                        {

                            Log.Write("ERROR ON CHANGENAME DONATION\n" + e.ToString());

                            return;

                        }



                        if (Main.PlayerNames.Values.Contains(name))

                        {

                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NameExists), 3000);

                            return;

                        }



                        ExtPlayer target = (ExtPlayer) NAPI.Player.GetPlayerFromName(player.Name);



                        if (target == null || target.IsNull) return;

                        else

                        {

                            Admin.AdminsLog(1, LangFunc.GetText(LangType.Ru, DataName.ChangeNameAdmins, player.Name, player.Value, name), 1);

                            UpdateData.RedBucks(player, -800, msg: LangFunc.GetText(LangType.Ru, DataName.ChangeName));

                            Character.Change.Repository.ChangeName(player, name);

                        }

                        break;



                    case 2:

                        if (characterData.Warns <= 0)

                        {

                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Nowarn), 3000);

                            return;

                        }

                        if (accountData.RedBucks < 1000)

                        {

                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);

                            return;

                        }

                        UpdateData.RedBucks(player, -1000, msg: LangFunc.GetText(LangType.Ru, DataName.SnyatieWarn));

                        characterData.Warns -= 1;

                        //Repository.PlayerStats(player);

                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucUnwarn), 3000);

                        break;

                    default:

                        // Not supposed to end up here. 
                        break;

                }

            }

            catch (Exception e)

            {

                Log.Write($"BuyChar Exception: {e.ToString()}");

            }       
        }
        [RemoteEvent("server.donate.change")]
        public void Change(ExtPlayer player, int amount)
        {
            try

            {

                if (!player.IsCharacterData()) return;

                var accountData = player.GetAccountData();

                if (accountData == null) return;

                if (amount <= 0)

                {

                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Amount1OrMore), 3000);

                    return;

                }

                if (accountData.RedBucks < amount)

                {

                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);

                    return;

                }

                int totalamount = amount * Convert.ToInt32(10 * Main.DonateSettings.Convert);
                
                Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.RedAge, LangFunc.GetText(LangType.Ru, DataName.SucConvertRbToMoney, amount, totalamount), DateTime.Now);
                //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucConvertRbToMoney, amount, totalamount), 3000);

                UpdateData.RedBucks(player, -amount, msg: LangFunc.GetText(LangType.Ru, DataName.ConvertRbTo, totalamount));

                Wallet.Change(player, +totalamount);

            }

            catch (Exception e)

            {

                Log.Write($"Change Exception: {e.ToString()}");

            }
        }
        [RemoteEvent("server.buySlots")]
        public void BuyChar(ExtPlayer player, int slotId)
        {
            try
            {
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                if (accountData.Chars[slotId] != -2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Already3Slot), 3000);
                    return;
                }

                if (accountData.RedBucks < 1000)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);

                    return;

                }

                UpdateData.RedBucks(player, -1000, msg: LangFunc.GetText(LangType.Ru, DataName.Buy3Slot));

                if (accountData.VipLvl <= 3)

                {

                    accountData.VipDate = (accountData.VipLvl == 0) ? DateTime.Now.AddDays(30) : accountData.VipDate.AddDays(30);

                    accountData.VipLvl = 3;

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlatinumVIP30d), 3000);

                }

                else

                {

                    accountData.VipDate = accountData.VipDate.AddDays(7);

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlattoVip7d), 5000);

                }



                accountData.Chars[slotId] = -1;

                Trigger.ClientEvent(player, "unlockSlot", slotId);

            }

            catch (Exception e)

            {

                Log.Write($"BuyChar Exception: {e.ToString()}");

            }
        }

        class DonateClothesData
        {
            public int PriceRB { get; set; } = 0;
            public ItemId ItemId { get; set; } = ItemId.Debug;
            public string Data { get; set; } = "";
            public DonateClothesData(int PriceRB, ItemId ItemId, string Data)
            {
                this.PriceRB = PriceRB;
                this.ItemId = ItemId;
                this.Data = Data;
            }
        }
        private static IReadOnlyDictionary<int, DonateClothesData> DonateClothesList = new Dictionary<int, DonateClothesData>()
        {
            {0, new DonateClothesData(30000, ItemId.Mask, "127_0_True")},
            {1, new DonateClothesData(1000, ItemId.Mask, "182_0_True")},
            {2, new DonateClothesData(2000, ItemId.Mask, "183_0_True")},
            {3, new DonateClothesData(35000, ItemId.Mask, "190_0_True")},
            {4, new DonateClothesData(10000, ItemId.Glasses, "30_0_True")},
            {5, new DonateClothesData(30000, ItemId.Hat, "91_0_True")},
            {6, new DonateClothesData(1500, ItemId.Hat, "149_0_True")},
            {7, new DonateClothesData(4200, ItemId.Hat, "153_0_True")},
            {8, new DonateClothesData(25000, ItemId.Leg, "77_0_True")},
            {9, new DonateClothesData(20000, ItemId.Leg, "85_0_True")},
            {10, new DonateClothesData(20000, ItemId.Leg, "106_4_True")},
            {11, new DonateClothesData(5000, ItemId.Leg, "132_0_True")},
            {12, new DonateClothesData(30000, ItemId.Leg, "138_0_True")},
            {13, new DonateClothesData(25000, ItemId.Feet, "55_0_True")},
            {14, new DonateClothesData(3000, ItemId.Feet, "67_4_True")},
            {15, new DonateClothesData(15000, ItemId.Top, "178_0_True")},
            {16, new DonateClothesData(10000, ItemId.Top, "201_0_True")},
            {17, new DonateClothesData(10000, ItemId.Top, "274_4_True")},
            {18, new DonateClothesData(30000, ItemId.Top, "362_0_True")},            
            {19, new DonateClothesData(5000, ItemId.Top, "381_2_True")},
            //
            {20, new DonateClothesData(1000, ItemId.Mask, "182_0_False")},
            {21, new DonateClothesData(2000, ItemId.Mask, "183_0_False")},
            {22, new DonateClothesData(35000, ItemId.Mask, "190_0_False")},
            {23, new DonateClothesData(10000, ItemId.Glasses, "32_0_False")},
            {24, new DonateClothesData(30000, ItemId.Hat, "90_0_False")},
            {25, new DonateClothesData(1500, ItemId.Hat, "148_0_False")},
            {26, new DonateClothesData(25000, ItemId.Leg, "79_0_False")},
            {27, new DonateClothesData(20000, ItemId.Leg, "88_0_False")},
            {28, new DonateClothesData(20000, ItemId.Leg, "113_5_False")},
            {29, new DonateClothesData(10000, ItemId.Feet, "58_0_False")},
            {30, new DonateClothesData(3000, ItemId.Feet, "70_0_False")},
            {31, new DonateClothesData(15000, ItemId.Top, "180_0_False")},
            {32, new DonateClothesData(10000, ItemId.Top, "203_0_False")},
            {33, new DonateClothesData(10000, ItemId.Top, "287_5_False")},
            {34, new DonateClothesData(2000, ItemId.Top, "372_0_False")},
            {35, new DonateClothesData(4000, ItemId.Top, "400_22_False")},
        };
        [RemoteEvent("server.donate.buy.clothes")]
        public void BuyClothes(ExtPlayer player, int id)
        {

            try

            {

                if (!DonateClothesList.ContainsKey(id)) return;

                DonateClothesData cData = DonateClothesList[id];



                var accountData = player.GetAccountData();

                if (accountData == null) return;

                if (accountData.RedBucks < cData.PriceRB)

                {

                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);

                    return;

                }

                var characterData = player.GetCharacterData();

                if (characterData == null) return;

                else if (Repository.isFreeSlots(player, cData.ItemId, 1) != 0) return;

                Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", cData.ItemId, 1, cData.Data);

                UpdateData.RedBucks(player, -cData.PriceRB, msg: LangFunc.GetText(LangType.Ru, DataName.LKLogBuyClothes, id));

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PremiumClothesBuy), 3000);

            }

            catch (Exception e)

            {

                Log.Write($"BuyClothes Exception: {e.ToString()}");

            }
        }

        private List<List<object>> VehicleNumberData = new List<List<object>>
        {
            new List<object>{500000, "$", "Обычный"},//0
            new List<object>{5000, "RB", "Редкий"},//1
            new List<object>{7000, "RB", "Уникальный"},//2
            new List<object>{10000, "RB", "Уникальный"},//3
            new List<object>{15000, "RB", "Люкс"},//4
            new List<object>{35000, "RB", "Люкс"}//5
            
        };
        public static int GetVehicleNumberPrice(string number)
        {
            object[] test = {30000, "RB", "Люкс"};
            
            var rg = new Regex(@"([A-Za-z]{1,1})([0-9]{1,1})([0-9]{1,1})([0-9]{1,1})([A-Za-z]{1,1})", RegexOptions.IgnoreCase);
            
            if (rg.IsMatch(number))
            {
                var coincidence = 0;
                for (var i = 1; i < 4; i++) {
                    for (var x = i + 1; x < 4; x++) {
                        if (number[i] == number[x]) coincidence++;
                    }
                }

                if (number[0] != number[4]) {
                    if (coincidence == 0)
                        return 0;//вирты
                    if (coincidence == 1 && number[1] != number[3])
                        return 1;
                    if (coincidence == 1 && number[1] == number[3])
                        return 2;
                    if (coincidence == 3)
                        return 3;
                } else if (number[0] == number[4]) {
                    if (coincidence == 0)
                        return 1;
                    if (coincidence == 1 && number[1] != number[3])
                        return 2;
                    if (coincidence == 1 && number[1] == number[3])
                        return 3;
                    if (coincidence == 3)
                        return 4;
                }
            }

            return 5;
        }
        [RemoteEvent("server.donate.buyVehNumber")]
        public void BuyVehNumber(ExtPlayer player, string number)
        {
            var accountData = player.GetAccountData();
            if (accountData == null) return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            
            if (!FunctionsAccess.IsWorking("donatebuyVehNumber"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                return;
            }
            number = number.ToUpper();
            
            var rg = new Regex(@"^[A-Z0-9]+$", RegexOptions.IgnoreCase);
            if (!rg.IsMatch(number))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IncorrectInputNewNumber), 3000);
                return;
            }
            
            if (VehicleManager.VehicleNumbers.Contains(number))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Номер уже существует", 3000);
                return;
            }
            var numberData = VehicleNumberData[GetVehicleNumberPrice(number)];

            var money = Convert.ToInt32(numberData[0]);
            
            if (numberData[1].ToString() == "$" && UpdateData.CanIChange(player, money, true) != 255) return;
            if (numberData[1].ToString() == "RB" && accountData.RedBucks < money)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);
                return;
            }
            if (Chars.Repository.isFreeSlots(player, ItemId.VehicleNumber, 1) != 0) 
                return; 
            
            Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.VehicleNumber, 1, number);
            
            if (numberData[1].ToString() == "$")
            {
                GameLog.Money($"player({characterData.UUID})", $"buyVehNumber", money, $"buyVehNumber");
                Wallet.Change(player, -money);
            }
            else
            {
                Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.RedAge, LangFunc.GetText(LangType.Ru, DataName.DonNumBuy, number, money), DateTime.Now);
                UpdateData.RedBucks(player, -money, LangFunc.GetText(LangType.Ru, DataName.DonNumBuy, number, money));
            }
            //client.donate.close
            Trigger.ClientEvent(player, "client.donate.close");
        }
        
        private List<List<object>> SimData = new List<List<object>>
        {
            new List<object>{55000, "RB", "Люкс"},//0
            new List<object>{45000, "RB", "Люкс"},//1
            new List<object>{35000, "RB", "Люкс"},//2
            new List<object>{25000, "RB", "Редкий"},//3
            new List<object>{20000, "RB", "Редкий"},//4
            new List<object>{15000, "RB", "Уникальный"},//5
            new List<object>{10000, "RB", "Уникальный"},//6
            new List<object>{7500, "RB", "Уникальный"},//7
            new List<object>{5000, "RB", "Обычный"},//8
            
        };
        
        [RemoteEvent("server.donate.buySim")]
        public void BuyVehNumber(ExtPlayer player, int sim)
        {
            var accountData = player.GetAccountData();
            if (accountData == null) return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            
            if (!FunctionsAccess.IsWorking("donatebuySim"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                return;
            }

            if (sim < 1 || sim > 999_999_999)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Неверный формат номера", 3000);
                return;
            }
            
            if (Players.Phone.Sim.Repository.Contains(sim))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Номер уже существует", 3000);
                return;
            }

            var numberData = SimData[sim.ToString().Length - 1];

            var money = Convert.ToInt32(numberData[0]);
            
            if (accountData.RedBucks < money)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);
                return;
            }
            if (Chars.Repository.isFreeSlots(player, ItemId.SimCard, 1) != 0) 
                return; 
            
            Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.SimCard, 1, sim.ToString());
            
            Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.RedAge, LangFunc.GetText(LangType.Ru, DataName.DonSimBuy, sim, money), DateTime.Now);
            UpdateData.RedBucks(player, -money, LangFunc.GetText(LangType.Ru, DataName.DonSimBuy, sim, money));
            //client.donate.close
            Trigger.ClientEvent(player, "client.donate.close");
        }

    }
}
