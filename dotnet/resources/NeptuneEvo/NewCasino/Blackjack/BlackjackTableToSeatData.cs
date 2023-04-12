using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.NewCasino
{
    /// <summary>
    /// Данные пользователя
    /// </summary>
    public class BlackjackTableToSeatData
    {
        #region Свойства

        /// <summary>
        /// Колличество
        /// </summary>
        public Vector3 Position;
        public double Rotation;
        #endregion

        /// <summary>
        /// Конструктор данных пользователя
        /// </summary>
        /// <param name="TableId"><see cref="Index"/></param>
        /// <param name="SlotId"><see cref="SlotId"/></param>
        public BlackjackTableToSeatData(Vector3 Position, double Rotation)
        {
            this.Position = Position;
            this.Rotation = Rotation;
        }
    }
}
