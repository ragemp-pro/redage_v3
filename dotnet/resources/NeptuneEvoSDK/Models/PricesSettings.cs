using System;
namespace Redage.SDK.Models
{
    public class PricesSettings
    {
        /// <summary>
        /// цены фейерверка
        /// </summary>
        public int[] FireworkPrices = new int[]
        {
            150,
            500,
            1000,
            2000
        };
        /// <summary>
        /// цены топора и кирки
        /// </summary>
        public int[] InstrumentPrices = new int[]
        {
            1250, // Топор
            300,
            1250,
            2250
        };

        public int[] FurtinurePrices = new int[]
        {
            1500,
            1500,
            1500,
            10000,
            1000,
            1000,
            1000,
            1000,
            1000,
            10000,
            10000,
            10000,
            500,
            500,
            500,
            500,
            500,
            500,
            750,
            750,
            750,
            750,
            1000,
            750,
            2500,
            10000,
            1000,
            10000,
            25000,
            50000,
            2500,
            100000,
            15000,
            250,
            1500,
            500,
            500,
            500,
            500,
            500
        };
        
        public int[] GaragesPrice = new int[]
        {
            8000,
            12000,
            20000,
            35000,
            50000,
            175000,
            10000,
            25000,
            50000
        };
        
        public int[] DalnoboyMoney = new int[]
        {
            620,
            120
        };
        
        public int CreateOrgPrice = 250000; // 2.5kk $
        public int FirstOrgPrice = 100000; // 1kk $
        public int SecondOrgPrice = 5000; // 5000 RB
        public int CustomsPrice = 75000; // 750k $
        public int StockPrice = 50000; // 500k $
        public int CrimeOptionsPrice = 1999; // 600k $
        
        public int UpdateTypeOrganization = 1999; // 1999 RB
        public int UpdateOrganizationLeader = 250; // 1999 RB
        
        public int AirdropInfoPrice = 25000;
        public int AirdropOrderPrice = 100000;
        
        public int ZapravkaMinPrice = 2;
        public int ZapravkaMaxPrice = 3;
        
        public int ClothesMinPrice = 48;
        public int ClothesMaxPrice = 88;
        
        public int TattooBarberMasksLscMinPrice = 95;
        public int TattooBarberMasksLscMaxPrice = 175;
        
        public int PistolAmmoPrice = 4;
        public int ShotgunAmmoPrice = 8;
        public int SMGAmmoPrice = 8;
        public int RiflesAmmoPrice = 15;
        public int SnniperAmmoPrice = 110;
            
        public int PosobieNew = 25;
        public int PosobieOld = 50;
        
        public int MaxOgrableine = 300;
    }
}