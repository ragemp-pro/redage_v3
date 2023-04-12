namespace NeptuneEvo.Chars.Models
{
    /// <summary>
    /// Событие для трейда персонажа
    /// </summary>
    public enum TradeStage
    {
        /// <summary>
        /// Ошибка
        /// </summary>
        none,
        /// <summary>
        /// Проверка на активные предложения
        /// </summary>
        choose,
        /// <summary>
        /// Успешно
        /// </summary>
        confirmed,
    }
}
