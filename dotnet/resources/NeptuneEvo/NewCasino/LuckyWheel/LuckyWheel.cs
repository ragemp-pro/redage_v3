using System;
using GTANetworkAPI;
using Redage.SDK;
using System.Collections.Generic;
using NeptuneEvo.Handles;
using Localization;
using NeptuneEvo.Core;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Players.Phone.Messages.Models;

namespace NeptuneEvo.NewCasino
{
    class LuckyWheel : Script
    {
        #region Modules
        private static Random Rnd = new Random();
        private static DateTime WaitFor { get; set; }
        private static int BlockTimeSeconds { get; } = 21;

        private static void ComeToLuckyWheel(ExtPlayer player)
        {
            if (DateTime.Now < WaitFor)
            {
                // Ждем пока колесо остановится (Завязано на таймере)
                player.SendNotification("Вам надо немного подождать");
                return;
            }
            else if (player.CharacterData.IsLucky == true)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Крутить колесо можно раз в день!", 3000);
                return;
            }
            else
            {
                // Присваимваем рандомное значение для колеса
                WaitFor = DateTime.Now.AddSeconds(BlockTimeSeconds);
                int value = Rnd.Next(0, 20);
                player.SetSharedData("LUCKY_WHEEL_CALL", true);
                player.SetSharedData("LUCKY_WHEEL_WIN", value);
                player.PlayAnimation("rcmcollect_paperleadinout@", "kneeling_arrest_get_up", 33);
                Main.OnAntiAnim(player);
                Trigger.ClientEvent(player, "luckywheel.cometoluckywheel", value);
            }
        }
        private static void SpinLuckyWheel(ExtPlayer player)
        {
            if (player.HasSharedData("LUCKY_WHEEL_WIN") && player.HasSharedData("LUCKY_WHEEL_CALL"))
            {
                player.SetSharedData("LUCKY_WHEEL_CALL", true);
                Trigger.ClientEventInRange(player.Position, 100, "luckywheel.spin", player.GetSharedData<int>("LUCKY_WHEEL_WIN"));
            }
        }
        private static void FinishSpin(ExtPlayer player)
        {
            if (player.HasSharedData("LUCKY_WHEEL_WIN") && player.HasSharedData("LUCKY_WHEEL_CALL"))
            {
                string resultName = "Приз";
                switch (player.GetSharedData<int>("LUCKY_WHEEL_WIN"))
                {
                    case 0:
                    case 8:
                    case 12:
                    case 16:
                        resultName = "Одежда";
                        GiveOutPrizeClothes(player);
                        break;
                    case 2:
                    case 6:
                    case 14:
                    case 19:
                        int price = Rnd.Next(1780, 62742);
                        resultName = $"Игровая валюта в размере {price}";
                        MoneySystem.Wallet.Change(player, price);
                        break;
                    case 18:
                        resultName = "Эксклюзивная машина";
                        GiveOutPrizeVehicle(player);
                        break;
                    case 1:
                    case 5:
                    case 9:
                    case 13:
                    case 17:
                        resultName = "Мистический предмет";
                        GiveOutPrizeMysticItem(player);
                        break;
                    case 3:
                    case 7:
                    case 10:
                    case 15:
                        resultName = "Оружие";
                        GiveOutPrizeWeapon(player);
                        break;
                    case 11:
                        resultName = "Уникальный костюм";
                        GiveOutPrizeCostume(player);
                        break;
                    case 4:
                        int donateCoins = Rnd.Next(50, 100);
                        resultName = $"Донат валюта в размере {donateCoins}";
                        Chars.UpdateData.RedBucks(player, donateCoins, "Выдача коинов казино");
                        break;
                }
                EventSys.SendPlayersToEvent("LuckyWheel", "Diamond Casino", $"Выигрыш: {resultName}. Поздравляем!", "", 3000);
                Main.OffAntiAnim(player);
                player.ResetSharedData("LUCKY_WHEEL_CALL");
                player.ResetSharedData("LUCKY_WHEEL_WIN");
                player.CharacterData.IsLucky = true;
            }
        }
        #region Compensations
        // Выплачиваемые компенсации, при ошибке выдачи призов
        private static Dictionary<string, int> amountCompensations = new Dictionary<string, int>()
        {
            { "weapon", 30000 },
            { "mystic", 15000 },
            { "vehicle", 50000 },
            { "clothes", 20000 }
        };
        private static void GiveOutPrizeCostume(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();

            Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Feet, 1, "55_0_1");
            Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Leg, 1, "77_0_1");
            Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Top, 1, "178_0_1");
        }
        private static void GiveOutPrizeWeapon(ExtPlayer player)
        {
            int amountCompensation = amountCompensations["weapon"];
            int randomInt = Rnd.Next(0, 4);

            switch (randomInt)
            {
                case 0:
                    if (Chars.Repository.isFreeSlots(player, ItemId.Bat) != 0)
                    {
                        EventSys.SendPlayersToEvent("LuckyWheel", "Diamond Casino", $"Недостаточно места, вам выдана компенсация {amountCompensation}$", "", 3000);
                        MoneySystem.Wallet.Change(player, amountCompensation);
                        return;
                    }
                    WeaponRepository.GiveWeapon(player, ItemId.Bat, "LuckyWheel");
                    break;
                case 1:
                    if (Chars.Repository.isFreeSlots(player, ItemId.HeavyPistol) != 0)
                    {
                        EventSys.SendPlayersToEvent("LuckyWheel", "Diamond Casino", $"Недостаточно места, вам выдана компенсация {amountCompensation}$", "", 3000);
                        MoneySystem.Wallet.Change(player, amountCompensation);
                        return;
                    }
                    WeaponRepository.GiveWeapon(player, ItemId.HeavyPistol, "LuckyWheel");
                    break;
                case 2:
                    if (Chars.Repository.isFreeSlots(player, ItemId.Musket) != 0)
                    {
                        EventSys.SendPlayersToEvent("LuckyWheel", "Diamond Casino", $"Недостаточно места, вам выдана компенсация {amountCompensation}$", "", 3000);
                        MoneySystem.Wallet.Change(player, amountCompensation);
                        return;
                    }
                    WeaponRepository.GiveWeapon(player, ItemId.Musket, "LuckyWheel");
                    break;
                case 3:
                    if (Chars.Repository.isFreeSlots(player, ItemId.AdvancedRifle) != 0)
                    {
                        EventSys.SendPlayersToEvent("LuckyWheel", "Diamond Casino", $"Недостаточно места, вам выдана компенсация {amountCompensation}$", "", 3000);
                        MoneySystem.Wallet.Change(player, amountCompensation);
                        return;
                    }
                    WeaponRepository.GiveWeapon(player, ItemId.AdvancedRifle, "LuckyWheel");
                    break;
            }
        }
        private static void GiveOutPrizeMysticItem(ExtPlayer player)
        {
            int amountCompensation = amountCompensations["mystic"];
            int randomInt = Rnd.Next(0, 5);

            switch (randomInt)
            {
                case 0:
                    if (Chars.Repository.isFreeSlots(player, ItemId.Flashlight) != 0)
                    {
                        EventSys.SendPlayersToEvent("LuckyWheel", "Diamond Casino", $"Недостаточно места, вам выдана компенсация {amountCompensation}$", "", 3000);
                        MoneySystem.Wallet.Change(player, amountCompensation);
                        return;
                    }
                    WeaponRepository.GiveWeapon(player, ItemId.Flashlight, "LuckyWheel");
                    break;
                case 1:
                    if (Chars.Repository.isFreeSlots(player, ItemId.BattleAxe) != 0)
                    {
                        EventSys.SendPlayersToEvent("LuckyWheel", "Diamond Casino", $"Недостаточно места, вам выдана компенсация {amountCompensation}$", "", 3000);
                        MoneySystem.Wallet.Change(player, amountCompensation);
                        return;
                    }
                    WeaponRepository.GiveWeapon(player, ItemId.BattleAxe, "LuckyWheel");
                    break;
                case 2:
                    if (Chars.Repository.isFreeSlots(player, ItemId.FlareGun) != 0)
                    {
                        EventSys.SendPlayersToEvent("LuckyWheel", "Diamond Casino", $"Недостаточно места, вам выдана компенсация {amountCompensation}$", "", 3000);
                        MoneySystem.Wallet.Change(player, amountCompensation);
                        return;
                    }
                    WeaponRepository.GiveWeapon(player, ItemId.FlareGun, "LuckyWheel");
                    break;
                case 3:
                    if (Chars.Repository.isFreeSlots(player, ItemId.StunGun) != 0)
                    {
                        EventSys.SendPlayersToEvent("LuckyWheel", "Diamond Casino", $"Недостаточно места, вам выдана компенсация {amountCompensation}$", "", 3000);
                        MoneySystem.Wallet.Change(player, amountCompensation);
                        return;
                    }
                    WeaponRepository.GiveWeapon(player, ItemId.StunGun, "LuckyWheel");
                    break;
                case 4:
                    if (Chars.Repository.isFreeSlots(player, ItemId.MicroSMG) != 0)
                    {
                        EventSys.SendPlayersToEvent("LuckyWheel", "Diamond Casino", $"Недостаточно места, вам выдана компенсация {amountCompensation}$", "", 3000);
                        MoneySystem.Wallet.Change(player, amountCompensation);
                        return;
                    }
                    WeaponRepository.GiveWeapon(player, ItemId.MicroSMG, "LuckyWheel");
                    break;
            }
        }
        private static void GiveOutPrizeVehicle(ExtPlayer player)
        {
            int amountCompensation = amountCompensations["vehicle"];
            int cars = new Random().Next(0, 4);
            string model = null;
            switch (cars)
            {
                case 0:
                    model = "baller3";
                    break;
                case 1:
                    model = "cheburek";
                    break;
                case 2:
                    model = "furia";
                    break;
                case 3:
                    model = "brioso";
                    break;
            }

            var vehiclesCount = VehicleManager.GetVehiclesCarCountToPlayer(player.Name);
            if (vehiclesCount >= Houses.GarageManager.MaxGarageCars)
            {
                MoneySystem.Wallet.Change(player, amountCompensation);
                EventSys.SendPlayersToEvent("LuckyWheel", "Diamond Casino", $"Вы получили компенсацию в размере {amountCompensation}$ так как у вас максимальное количество авто", "", 3000);
            }
            else
            {
                var house = Houses.HouseManager.GetHouse(player, true);
                if (house != null)
                {
                    if (vehiclesCount >= Houses.GarageManager
                        .GarageTypes[Houses.GarageManager.Garages[house.GarageID].Type].MaxCars)
                    {
                        EventSys.SendPlayersToEvent("LuckyWheel", "Diamond Casino", $"У Вас максимальное кол-во машин, которое поддерживает Ваше место жительства.", "", 3000);
                        return;
                    }
                }

                VehicleManager.Create(player, model, new GTANetworkAPI.Color(225, 225, 225), new GTANetworkAPI.Color(225, 225, 225));
                Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Bank, LangFunc.GetText(LangType.Ru, DataName.YouBuyCarV3, model), DateTime.Now);

                EventSys.SendPlayersToEvent("LuckyWheel", "Diamond Casino", $"Вы получили уникальный автомообиль {model}.", "", 3000);

            }
        }
        private static void GiveOutPrizeClothes(ExtPlayer player)
        {
            int amountCompensation = amountCompensations["clothes"];

            var characterData = player.GetCharacterData();
            if (Chars.Repository.isFreeSlots(player, ItemId.Hat) != 0)
            {
                EventSys.SendPlayersToEvent("LuckyWheel", "Diamond Casino", $"Недостаточно места, вам выдана компенсация {amountCompensation}$", "", 3000);
                MoneySystem.Wallet.Change(player, amountCompensation);
                return;
            }
            int cloth = Rnd.Next(0, 4);
            switch (cloth)
            {
                case 0:
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Hat, 1, "77_0_1");
                    break;
                case 1:
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Hat, 1, "40_0_1");
                    break;
                case 2:
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Hat, 1, "22_0_1");
                    break;
                case 3:
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Hat, 1, "42_0_1");
                    break;
            }
        }
        #endregion
        #endregion

        #region Events
        [RemoteEvent("luckywheel.cometoluckywheel")]
        public static void ComeToLuckyWheel_Event(ExtPlayer player)
        {
            ComeToLuckyWheel(player);
        }

        [RemoteEvent("luckywheel.spin")]
        public static void SpinLuckyWheel_Event(ExtPlayer player)
        {
            SpinLuckyWheel(player);
        }

        [RemoteEvent("luckywheel.finishspin")]
        public static void FinishSpin_Event(ExtPlayer player)
        {
            FinishSpin(player);
        }
        #endregion
    }
}
