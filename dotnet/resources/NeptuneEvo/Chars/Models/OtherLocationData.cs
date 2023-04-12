using Redage.SDK;

namespace NeptuneEvo.Chars.Models
{
    /// <summary>
    /// Данные пользователя
    /// </summary>
    public class OtherLocationData
    {
        #region Свойства

        /// <summary>
        /// Id в бд
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// Item Id
        /// </summary>
        public int OtherId { get; set; } = 0;

        #endregion

        /// <summary>
        /// Конструктор данных пользователя
        /// </summary>
        /// <param name="Location"><see cref="Location"/></param>
        /// <param name="OtherId"><see cref="OtherId"/></param>
        public OtherLocationData(string Location, int OtherId)
        {
            this.Location = Location;
            this.OtherId = OtherId;
        }
    }
}
