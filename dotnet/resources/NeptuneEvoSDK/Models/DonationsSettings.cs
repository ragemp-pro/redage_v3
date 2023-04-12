using System.Collections.Generic;
namespace Redage.SDK.Models
{
    public class DonationsSettings : Mysql
    {
        /// <summary>
        /// Включена ли проверка доната
        /// </summary>
        public bool IsCheck = false;
        /// <summary>
        /// Бонус при пополнении
        /// </summary>
        public bool IsSaleEnable = false;
        /// <summary>
        /// Множетель конвертации
        /// </summary>
        public short Convert = 5;
        /// <summary>
        /// Множетель доната
        /// </summary>
        public short Multiplier = 2;
        // Деньги с донат 
        /// <summary>
        /// Рб в часах счастливых
        /// </summary>
        public int HappyHoursRB = 250;
    }
    
}