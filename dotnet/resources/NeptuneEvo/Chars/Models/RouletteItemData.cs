using GTANetworkAPI;
using NeptuneEvo.Handles;
using System;

namespace NeptuneEvo.Chars.Models
{
    public enum RouletteColor
    {
        Blue,
        Yellow,
        Pink,
        Red,
        White,
    }
    /// <summary>
    /// Данные пользователя
    /// </summary>
    public class RouletteItemData
    {
        #region Свойства


        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }
        public string Desc { get; set; }
        public string Image { get; set; }


        /// <summary>
        /// Минимальное значение
        /// </summary>
        public int ValueMin { get; set; }

        /// <summary>
        /// Максимальное значение
        /// </summary>
        public int ValueMax { get; set; }

        /// <summary>
        /// Сколько вернется в случае продажи
        /// </summary>
        public int ReturnRB { get; set; }

        /// <summary>
        /// Процент который должен выпасть
        /// </summary>
        public int Count { get; set; }

        public RouletteColor Color { get; set; }
        public bool IsChatMessage { get; set; }
        public bool IsHudMessage { get; set; }
        public ItemId ItemId { get; set; } = ItemId.Debug;
        public string ItemData { get; set; }

        #endregion

        /// <summary>
        /// Конструктор данных пользователя
        /// </summary>
        /// <param name="Id"><see cref="Id"/></param>
        /// <param name="Name"><see cref="Name"/></param>
        /// <param name="ValueMin"><see cref="ValueMin"/></param>
        /// <param name="ValueMax"><see cref="ValueMax"/></param>
        /// <param name="ReturnRB"><see cref="ReturnRB"/></param>
        /// <param name="Percent"><see cref="Count"/></param>
        public RouletteItemData(string Name, string Desc, string Image, int ValueMin, int ValueMax, int ReturnRB, int Percent, RouletteColor Color = RouletteColor.Blue, bool IsChatMessage = false, bool IsHudMessage = false, ItemId ItemId = ItemId.Debug, string ItemData = "")
        {
            this.Name = Name;
            this.Desc = Desc;
            this.Image = Image;
            this.ValueMin = ValueMin;
            this.ValueMax = ValueMax + 1;
            this.ReturnRB = ReturnRB;
            this.Count = Percent;
            this.Color = Color;
            this.IsChatMessage = IsChatMessage;
            this.IsHudMessage = IsHudMessage;
            this.ItemId = ItemId;
            this.ItemData = ItemData;
        }
    }
}
