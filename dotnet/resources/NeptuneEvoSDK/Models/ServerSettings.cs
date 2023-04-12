using System.Collections.Generic;

namespace Redage.SDK.Models
{
    public class ServerSettings
    {
        /// <summary>
        /// Название сервера
        /// </summary>
        public string ServerName = "";
        /// <summary>
        /// Номер сервера - 0 - тестовый
        /// </summary>
        public byte ServerId = 0;
        /// <summary>
        /// Множетель зарплат
        /// </summary>
        public int MoneyMultiplier = 1;
        /// <summary>
        /// Множетель exp в payday
        /// </summary>
        public int ExpMultiplier = 1;
        /// <summary>
        /// Снимать ли на логи на биз
        /// </summary>
        public bool IsBusinessTax = true;
        /// <summary>
        /// Снимать ли на логи на дом
        /// </summary>
        public bool IsHouseTax = true;
        /// <summary>
        /// Минимальный уровень админов которым будет видно уведомления о репортах
        /// </summary>
        public int MinAdminLvlReport = 1;
        /// <summary>
        /// Минимальный уверовень голосования 
        /// </summary>
        public int MinVoteLvl = 5;
        /// <summary>
        /// Максимальная цена на мероприятиях
        /// </summary>
        public int EventRewardLimit = 100000;
        /// <summary>
        /// Время через которая можно начать новый капт в минутах
        /// </summary>
        public int CaptureNextTimeMinutes = 5;
        /// <summary>
        /// Включен ли перенос
        /// </summary>
        public bool IsMerger = false;
        /// <summary>
        /// Блипы на карте домов
        /// </summary>
        public bool IsHouseBlips = false;
        /// <summary>
        /// Кол-во победителей счастливых часов
        /// </summary>
        public int NumberWinners = 3;
        /// <summary>
        /// 
        /// </summary>
        public int MaxGameSlots = 1350;
        /// <summary>
        /// 
        /// </summary>
        public string SiteUrl = "https://redage.net/"; // UA-XXXXXXXXX-XX
        /// <summary>
        /// 
        /// </summary>
        public string GoogleTrackingId = "UA-138889592-2"; // UA-XXXXXXXXX-XX
        /// <summary>
        /// 
        /// </summary>
        public string GoogleCategory = "ra_game"; // 555 - any user identifier

        public string ClientInterfaceUrl = "";
        public bool IsEmailConfirmed = false; // 555 - any user identifier
        public int BonusCodeLvl = 0;
        public bool IsCheckJobLicC = true; // 555 - any user identifier
        public bool IsJobTinder = true;
        public bool IsCheckOnlineLogin = true;
        public bool IsCheckCmdGov = false;
        public bool IsDeleteProp = true;
        public bool IsCreateProp = true;
        public bool IsHeliCrash = true;
        //
        public bool IsAcceptExit = true;
        
    }
}