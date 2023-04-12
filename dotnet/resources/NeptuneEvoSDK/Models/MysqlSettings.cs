using System.Collections.Generic;

namespace Redage.SDK.Models
{
    public class MysqlSettings : Mysql
    {
        public List<Mysql> OtherList = new List<Mysql>();
    }
}