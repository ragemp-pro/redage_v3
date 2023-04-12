using GTANetworkAPI;
using NeptuneEvo.Handles;
using System.Collections.Generic;

namespace NeptuneEvo.NewCasino
{
    public class Table
    {
        /// <summary>
        /// ID стола
        /// </summary>
        public int TableId;

        /// <summary>
        /// Список игроков за столом
        /// </summary>
        public List<ExtPlayer> Seats = new List<ExtPlayer> { null, null, null, null };

        /// <summary>
        /// Статус если игра в процессе
        /// </summary>
        public bool Process = false;

        /// <summary>
        /// Таймер
        /// </summary>
        public string WaitTimeout = null;

        /// <summary>
        /// Выигранные споты?
        /// </summary>
        public List<int> WinSpots = new List<int>();

        /// <summary>
        /// Стринг правильного победного числа
        /// </summary>
        public string WinNum = "-";

        /// <summary>
        /// Key победного числа от Roulette Dictionary?
        /// </summary>
        public int Win = 0;
        public Table(int table)
        {
            this.TableId = table;
        }
    }
}
