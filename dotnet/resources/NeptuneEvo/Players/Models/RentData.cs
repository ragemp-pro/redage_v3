using GTANetworkAPI;
using NeptuneEvo.Handles;
using System;

namespace NeptuneEvo.Players.Models
{
    public class RentData
    {
        public int Price { get; set; }
        public DateTime Date { get; set; }
        public ExtVehicle Vehicle { get; set; }
        public string Model { get; set; }
        public string Number { get; set; }
        public bool IsJob { get; set; }

        public RentData(int price, DateTime date, ExtVehicle vehicle, string model, string number, bool isJob)
        {
            Price = price;
            Date = date;
            Vehicle = vehicle;
            Model = model;
            Number = number;
            IsJob = isJob;
        }
    }
}
