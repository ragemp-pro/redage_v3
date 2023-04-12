using System;
using System.Data;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using MySqlConnector;
using NeptuneEvo.Fractions;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Core
{
    class Economy : Script
    {
        private static readonly nLog Log = new nLog("Core.Economy");
        public static void ResetToDefault()
        {
            Main.BusinessMinPrice = 0.95f;
            Main.BusinessMaxPrice = 1.75f;
            Main.DrugsPrice = 30;
            Main.BuswaysPayments = new int[6] { 4, 5, 4, 5, 5, 8 };
            Main.CollectorPayment = 7;
            Main.ElectricianPayment = 6;
            Main.PostalPayment = 6;
            Main.LawnmowerPayment = 6;
            Main.GangCarDelivery = 500;
            Main.MafiaCarDelivery = 500;
            Main.PoliceAward = 100;
            Main.MinGunLic = 2500;
            Main.MaxGunLic = 5000;
            Main.MinPMLic = 1500;
            Main.MaxPMLic = 10000;
            Main.TicketLimit = 5000;
            Main.MinHealLimit = 50;
            Main.MaxHealLimit = 500;
            Main.CaptureWin = 1000;
            Main.BizwarWin = 1000;
            Main.MafiaForBiz = 90;
            Main.GangForPoint = 90;
            Main.LicPrices = new int[6] { 200, 500, 1500, 10000, 20000, 20000 };
            Main.HotelRent = 8;
            Main.SMSCost = 7;
            Main.AdSymbCost = 70;
            Main.EvacCar = 200;
            Main.AdEditorCost = 0.7f;
            Main.MinDice = 100;
            Main.MaxDice = 15000;
            Main.BlackMarketDrill = 2000;
            Main.BlackMarketLockPick = 200;
            Main.BlackMarketArmyLockPick = 700;
            Main.BlackMarketCuffs = 200;
            Main.BlackMarketPocket = 100;
            Main.BlackMarketWanted = 500;
            Main.BusPay = 3;
            Main.BlackMarketUnCuff = 400;
            Main.BlackMarketGunLic = 30000;
            Main.BlackMarketMedCard = 15000;
            Main.BlackRadioInterceptord = 20000;
            Main.BlackQrFake = 8000;
        }

        public static void Init()
        {
            using MySqlCommand cmd = new MySqlCommand()
            {
                    CommandText = "SELECT * FROM `economy`"
            };
            using DataTable result = MySQL.QueryRead(cmd);
            if (result != null)
            {
                try
                {
                    DataRow Row = result.Rows[0];
                    Main.BusinessMinPrice = (float)Row[0];
                    Main.BusinessMaxPrice = (float)Row[1];
                    Main.DrugsPrice = Convert.ToInt32(Row[2]);
                    Main.BuswaysPayments = JsonConvert.DeserializeObject<int[]>(Row[3].ToString());
                    Main.CollectorPayment = Convert.ToInt32(Row[4]);
                    Main.ElectricianPayment = Convert.ToInt32(Row[5]);
                    Main.PostalPayment = Convert.ToInt32(Row[6]);
                    Main.LawnmowerPayment = Convert.ToInt32(Row[7]);
                    Main.GangCarDelivery = Convert.ToInt32(Row[8]);
                    Main.MafiaCarDelivery = Convert.ToInt32(Row[9]);
                    Main.PoliceAward = Convert.ToInt32(Row[10]);
                    Main.MinGunLic = Convert.ToInt32(Row[11]);
                    Main.MaxGunLic = Convert.ToInt32(Row[12]);
                    Main.MinPMLic = Convert.ToInt32(Row[13]);
                    Main.MaxPMLic = Convert.ToInt32(Row[14]);
                    Main.TicketLimit = Convert.ToInt32(Row[15]);
                    Main.MinHealLimit = Convert.ToInt32(Row[16]);
                    Main.MaxHealLimit = Convert.ToInt32(Row[17]);
                    Main.CaptureWin = Convert.ToInt32(Row[18]);
                    Main.BizwarWin = Convert.ToInt32(Row[19]);
                    Main.MafiaForBiz = Convert.ToInt32(Row[20]);
                    Main.GangForPoint = Convert.ToInt32(Row[21]);
                    Main.LicPrices = JsonConvert.DeserializeObject<int[]>(Row[22].ToString());
                    Main.HotelRent = Convert.ToInt32(Row[23]);
                    Main.SMSCost = Convert.ToInt32(Row[24]);
                    Main.AdSymbCost = Convert.ToInt32(Row[25]);
                    Main.EvacCar = Convert.ToInt32(Row[26]);
                    Main.AdEditorCost = (float)Row[27];
                    Main.MinDice = Convert.ToInt32(Row[28]);
                    Main.MaxDice = Convert.ToInt32(Row[29]);
                    Main.BlackMarketDrill = Convert.ToInt32(Row[30]);
                    Main.BlackMarketLockPick = Convert.ToInt32(Row[31]);
                    Main.BlackMarketArmyLockPick = Convert.ToInt32(Row[32]);
                    Main.BlackMarketCuffs = Convert.ToInt32(Row[33]);
                    Main.BlackMarketPocket = Convert.ToInt32(Row[34]);
                    Main.BlackMarketWanted = Convert.ToInt32(Row[35]);
                    Main.BusPay = Convert.ToInt32(Row[36]);
                    Main.BlackMarketUnCuff = Convert.ToInt32(Row[37]);
                    Main.BlackMarketGunLic = Convert.ToInt32(Row[39]);
                    Main.BlackMarketMedCard = Convert.ToInt32(Row[40]);
                    Main.BlackRadioInterceptord = Convert.ToInt32(Row[41]);
                    Main.BlackQrFake = Convert.ToInt32(Row[42]);
                    
                    
                    Manager.FractionDataMats[1].Price = $"{Main.BlackMarketDrill}$";
                    Manager.FractionDataMats[2].Price = $"{Main.BlackMarketLockPick}$";
                    Manager.FractionDataMats[3].Price = $"{Main.BlackMarketArmyLockPick}$";
                    Manager.FractionDataMats[4].Price = $"{Main.BlackMarketCuffs}$";
                    Manager.FractionDataMats[5].Price = $"{Main.BlackMarketPocket}$";
                    Manager.FractionDataMats[6].Price = $"{Main.BlackMarketWanted}$";
                    Manager.FractionDataMats[69].Price = $"{Main.BlackMarketUnCuff}$";
                    Manager.FractionDataMats[78].Price = $"{Main.BlackMarketGunLic}$";
                    Manager.FractionDataMats[79].Price = $"{Main.BlackMarketMedCard}$";
                    Manager.FractionDataMats[80].Price = $"{Main.BlackQrFake}$";
                    Manager.FractionDataMats[81].Price = $"{Main.BlackRadioInterceptord}$";
                    
                    Log.Write($"Economy loaded.", nLog.Type.Success);
                }
                catch (Exception e)
                {
                    ResetToDefault();
                    Log.Write($"StartWork Exception: {e.ToString()}");
                }
            }
            else 
            {
                ResetToDefault();
                Log.Write("DB `economy` return null result", nLog.Type.Warn);
            }
        }
    }
}
