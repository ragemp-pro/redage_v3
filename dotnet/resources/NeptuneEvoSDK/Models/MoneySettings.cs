namespace Redage.SDK.Models
{
    public class MoneySettings
    {
        /// <summary>
        /// Сколько денег давать при создании чара
        /// </summary>
        public int CreateCharMoney = 2500;
        /// <summary>
        /// Сколько редбаксов давать при регистрации
        /// </summary>
        public int CreateAccountRedBucks = 0;
        /// <summary>
        /// Давать ли випку при регистрании ее уровень если 0 то нет
        /// </summary>
        public int CreateVipLvl = 0;
        /// <summary>
        /// На сколько дней выдовать випку
        /// </summary>
        public int CreateVipDay = 0;
    }
}