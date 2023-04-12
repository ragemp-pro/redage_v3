using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Accounts.Models
{
    public class AccountData
    {
        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; set; }
        /// <summary>
        /// Почта
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Уникальный индификатор пользоваеля
        /// </summary>
        public string HWID { get; set; }
        /// <summary>
        /// Ип
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// Ск
        /// </summary>
        public string SocialClub { get; set; }
        /// <summary>
        /// Донат валюта
        /// </summary>
        public int RedBucks { get; set; }
        /// <summary>
        /// Тип випки
        /// </summary>
        public int VipLvl { get; set; }
        /// <summary>
        /// Дата окончания випки
        /// </summary>
        public DateTime VipDate { get; set; } = DateTime.Now;
        /// <summary>
        /// Промокод
        /// </summary>
        public List<string> PromoCodes { get; set; }
        /// <summary>
        /// Бонус код
        /// </summary>
        public List<string> BonusCodes { get; set; }
        /// <summary>
        /// Ид реферала
        /// </summary>
        public int RefferalId { get; set; } = 0;
        /// <summary>
        /// Список чаров
        /// </summary>
        public List<int> Chars { get; set; } // characters uuids
        /// <summary>
        /// 
        /// </summary>
        public bool PresentGet { get; set; } = false;
        /// <summary>
        /// 
        /// </summary>
        public bool RefPresentGet { get; set; } = false;
        /// <summary>
        /// 
        /// </summary>
        public int[] FreeCase { get; set; } = new int[3] { 0, 0, 0 };
        /// <summary>
        /// 
        /// </summary>
        public bool isSubscribe { get; set; } = false;
        /// <summary>
        /// 
        /// </summary>
        public DateTime SubscribeEndTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 
        /// </summary>
        public DateTime SubscribeTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 
        /// </summary>
        public List<int> CollectionGifts { get; set; } = new List<int>();
        /// <summary>
        /// 
        /// </summary>
        public List<int> ReceivedAward { get; set; } = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 };
        /// <summary>
        /// 
        /// </summary>
        public int ReceivedAwardWeek { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public int ReceivedAwardDonate { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public string Unique { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int LastSelectCharUUID { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public string Ga { get; set; }
    }
}
