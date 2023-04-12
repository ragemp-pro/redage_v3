using System.Collections.Generic;

namespace NeptuneEvo.Players.Phone.Gps.Models
{
    public class GpsItem
    {
        public string Name;
        public List<float> Pos;
        public List<List<object>> PosList;

        public GpsItem(string name, float posX, float posY)
        {
            this.Name = name;
            this.Pos = new List<float>
            {
                posX,
                posY
            };
            this.PosList = null;
        }
        public GpsItem(string name, List<List<object>> posList)
        {
            this.Name = name;
            this.Pos = null;
            this.PosList = posList;
        }
    }
}