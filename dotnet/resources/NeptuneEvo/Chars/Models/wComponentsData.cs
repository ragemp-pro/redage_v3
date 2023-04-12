using GTANetworkAPI;
using NeptuneEvo.Handles;
using System;
using System.Collections.Generic;

namespace NeptuneEvo.Chars.Models
{
    public enum wComponentsType
    {
        Varmod = 1,
        Clip,
        Suppressor,
        Scope,
        Muzzlebrake,
        Barrel,
        Flashlight,
        Grip,
        Camo
    }
    public class wComponentsData
    {
        /// <summary>
        /// Колличество
        /// </summary>
        public int Count { get; set; }
        //Список компонентов
        public Dictionary<uint, wComponentData> Components { get; set; }

        public wComponentsData(int Count, Dictionary<uint, wComponentData> Components)
        {
            this.Count = Count;
            this.Components = Components;
        }
    }
    public class wComponentData
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public int Price { get; set; }
        public wComponentsType Type { get; set; }

        public wComponentData(string Name, string Desc, int Price, wComponentsType Type)
        {
            this.Name = Name;
            this.Desc = Desc;
            this.Price = Price;
            this.Type = Type;
        }
    }
}
