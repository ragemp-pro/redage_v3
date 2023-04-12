using GTANetworkAPI;
using NeptuneEvo.Handles;
using System;
using System.Collections.Generic;

namespace NeptuneEvo.Chars.Models
{
    /// <summary>
    /// Данные пользователя
    /// </summary>
    public class RefferalData
    {
        #region Свойства

        /// <summary>
        /// Id випки
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Колличество приглашенных
        /// </summary>
        public int Count { get; set; } = 0;
        /// <summary>
        /// Id випки
        /// </summary>
        public int VipId { get; set; } = 0;
        /// <summary>
        /// На сколько дней випки
        /// </summary>
        public int VipDays { get; set; } = 0;
        /// <summary>
        /// Обнулять или нет
        /// </summary>
        public bool FreeCoin { get; set; } = false;
        /// <summary>
        /// Деньги
        /// </summary>
        public int Money { get; set; } = 0;
        /// <summary>
        /// RB
        /// </summary>
        public int RB { get; set; } = 0;

        #endregion

        /// <summary>
        /// Конструктор данных пользователя
        /// </summary>
        /// <param name="Name"><see cref="Name"/></param>
        /// <param name="Count"><see cref="Count"/></param>
        /// <param name="VipId"><see cref="VipId"/></param>
        /// <param name="VipDays"><see cref="VipDays"/></param>
        /// <param name="FreeCoin"><see cref="FreeCoin"/></param>
        /// <param name="Money"><see cref="Money"/></param>
        /// <param name="RB"><see cref="RB"/></param>
        public RefferalData(string Name, int Count, int VipId, int VipDays, bool FreeCoin, int Money, int RB)
        {
            this.Name = Name;
            this.Count = Count;
            this.VipId = VipId;
            this.VipDays = VipDays;
            this.FreeCoin = FreeCoin;
            this.Money = Money;
            this.RB = RB;
        }
    }
}
