using GTANetworkAPI;
using NeptuneEvo.Handles;
using Redage.SDK;
using System;
using System.Collections.Generic;

namespace NeptuneEvo.NewCasino
{
    class LuckyWheel
    {
        /// <summary>
        /// Состояние колеса (вращается / не вращается)
        /// </summary>
        private static bool IsRolling = false;

        private static List<object> LastDrops = new List<object>();

        private static Vector3 TakeWinVehPos = new Vector3(1114.74, 229.3, 80.46);

        private static readonly nLog Log = new nLog("NewCasino.LuckyWheel");

        private Dictionary<string, int> indexes = new Dictionary<string, int>
        {
            ["veh"] = 19,
            ["exp"] = 1,
            ["clothes"] = 20,
            ["money"] = 16,
            ["donate"] = 11
        };

        private static List<Present> Presents = new List<Present>
        {
            new Present("money_100", 13.07f),
            new Present("money_200", 11.61f),
            new Present("money_500", 7.74f),
            new Present("money_10000", 0.1f),
            new Present("money_50000", 0.03f),
            // 32.55
    
            new Present("donate_45", 5.97f),
            new Present("donate_100", 5.34f),
            new Present("donate_500", 2f),

            new Present("exp_50", 13.86f),
            new Present("exp_100", 12.2f),
            new Present("exp_200", 10.38f),
            new Present("exp_300", 9.12f),
            // 58.87
            // 91.42
    
            new Present("veh_zorrusso", 0.05f),
            new Present("veh_xa21", 0.08f),
            new Present("veh_cheburek", 0.82f),
            new Present("veh_btype", 0.25f),
            new Present("veh_stafford", 0.25f),
            // 1.45
    
            // Неновые кросовки
            new Present("clothes_neon_shoes", 2.53f),
            //Маска быка
            new Present("clothes_mask_boll", 2.33f),
            // космические штаны
            new Present("clothes_cosmo_legs", 2.27f),
            // 7.13
        };

        private static Dictionary<string, string> Names = new Dictionary<string, string>
        {
            ["money_100"] = "$100",
            ["money_200"] = "$200",
            ["money_500"] = "$500",
            ["money_10000"] = "$10.000",
            ["money_50000"] = "$50.000",

            ["donate_45"] = "45 рублей",
            ["donate_100"] = "100 рублей",
            ["donate_500"] = "500 рублей",

            ["exp_50"] = "50 EXP",
            ["exp_100"] = "100 EXP",
            ["exp_200"] = "200 EXP",
            ["exp_300"] = "300 EXP",

            ["veh_zorrusso"] = "Автомобиль zorrusso",
            ["veh_xa21"] = "Автомобиль xa21",
            ["veh_cheburek"] = "Автомобиль cheburek",
            ["veh_btype"] = "Автомобиль btype",
            ["veh_stafford"] = "Автомобиль stafford",

            ["clothes_neon_shoes"] = "Неоновые кроссовки",
            ["clothes_mask_boll"] = "Маска быка",
            ["clothes_cosmo_legs"] = "Космические штаны"
        };

        private void StartWheel(ExtPlayer player, Action callback)
        {
            try
            {
                if (IsRolling)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "В данный момент кто-то вращает колесо удачи!", 3000);
                    return;
                }

                NAPI.ClientEvent.TriggerClientEventForAll("CASINO_LUCKYWHEEL:rollFinished");
                IsRolling = true;

                string _randomPrice = randomWithWeight(Presents);

                string itemType = _randomPrice.Split("_")[0];
                int wheelIndex = indexes[itemType];

                string winItemName = Names[_randomPrice];

                callback();

                Timers.StartOnce(4000, () =>
                {
                    IsRolling = false;
                    NAPI.ClientEvent.TriggerClientEventForAll("CASINO_LUCKYWHEEL:rollFinished");

                    // addLastDrop(itemType, winItemName);
                    // TODO:: выдать приз челу
                }, true);
            }
            catch (Exception e)
            {
                Log.Write($"StartWheel Exception: {e.ToString()}");
            }
        }

        private string randomWithWeight(List<Present> presents)
        {
            try
            {
                // Создаем переменные
                float total = 0.0f;
                float currentWeight = 0.0f;
                float rndWeight = 0.0f;

                // Вычисляем общий вес
                foreach (Present item in presents)
                    total += item.Weight;

                // Находим случайный вес
                Random random = new Random();
                rndWeight = Convert.ToSingle(random.NextDouble()) * total;

                // Перебираем все элементы, пока не достигнем нужного веса
                foreach (Present item in presents)
                {
                    currentWeight += item.Weight; // Прибавляем вес текущего элемента массива к временной переменной

                    if (currentWeight >= rndWeight) // Прерываем генерацию и возвращаем букву
                        return item.Word;
                }

                return "";
            }
            catch (Exception e)
            {
                Log.Write($"randomWithWeight Exception: {e.ToString()}");
                return "";
            }
        }
    }
}
