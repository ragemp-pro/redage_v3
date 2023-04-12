using System;

namespace NeptuneEvo.Players.Phone.News.Models
{
    public class AdvertData
    {
        public int ID { get; set; } // int(11)
        public string Author { get; set; } // varchar(50)
        public int AuthorSIM { get; set; } // int(11)
        public string AD { get; set; } // varchar(150)
        public string Link { get; set; } // varchar(150)
        public string Editor { get; set; } // varchar(50)
        public string EditedAD { get; set; } // varchar(500)
        public DateTime Opened { get; set; } // datetime
        public DateTime Closed { get; set; } // datetime
        public bool Status { get; set; } // tinyint(1)
        public int Type { get; set; } // tinyint(1)
        public bool IsPremium { get; set; } // tinyint(1)
    }
}