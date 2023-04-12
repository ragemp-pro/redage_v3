using GTANetworkAPI;
using NeptuneEvo.Handles;
using System;
using System.Collections.Generic;

namespace NeptuneEvo.Chars.Models
{
    /// <summary>
    /// Данные пользователя
    /// </summary>
    public class RouletteCaseData
    {
        #region Свойства

        public ItemId ItemId { get; set; }

        /// <summary>
        /// Id предмета
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Максимальное значение
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// Минимальное значение
        /// </summary>
        public List<RouletteItemData> RouletteItemsData { get; set; }
        public string Image { get; set; }
        public string Desc { get; set; }

        #endregion

        /// <summary>
        /// Конструктор данных пользователя
        /// </summary>
        /// <param name="Name"><see cref="Name"/></param>
        /// <param name="Price"><see cref="Price"/></param>
        public RouletteCaseData(ItemId ItemId, string Name, int Price, List<RouletteItemData> RouletteItemsData, string Image, string Desc)
        {
            this.ItemId = ItemId;
            this.Name = Name;
            this.Price = Price;
            this.RouletteItemsData = RouletteItemsData;
            this.Image = Image;
            this.Desc = Desc;
        }
    }
}
