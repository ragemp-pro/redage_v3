using System.Collections.Generic;

namespace NeptuneEvo.NewCasino
{
    /// <summary>
    /// Данные пользователя
    /// </summary>
    public class BlackjackPlayerInfo
    {
        #region Свойства

        /// <summary>
        /// Колличество
        /// </summary>
        public int Index = 0;
        public int SlotId = 0;
        public List<string> Hand = new List<string>();
        public List<string> SplitHand = new List<string>();        
        public bool Join = false;
        public int Rate = 0;
        public string Move = null;
        public bool Doubled = false;

        #endregion

        /// <summary>
        /// Конструктор данных пользователя
        /// </summary>
        /// <param name="TableId"><see cref="Index"/></param>
        /// <param name="SlotId"><see cref="SlotId"/></param>
        public BlackjackPlayerInfo(int TableId, int SlotId)
        {
            this.Index = TableId;
            this.SlotId = SlotId;
        }
    }
}
