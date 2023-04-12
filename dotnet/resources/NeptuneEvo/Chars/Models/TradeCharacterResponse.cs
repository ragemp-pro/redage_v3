namespace NeptuneEvo.Chars.Models
{
    /// <summary>
    /// Событие для трейда персонажа
    /// </summary>
    public enum TradeCharacterResponse
    {
        /// <summary>
        /// Ошибка
        /// </summary>
        Error,
        /// <summary>
        /// Проверка на активные предложения
        /// </summary>
        ErrorTrade,
        /// <summary>
        /// Успешно
        /// </summary>
        Fine,
    }
}
