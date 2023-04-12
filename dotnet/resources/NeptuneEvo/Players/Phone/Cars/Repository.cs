using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkMethods;
using NeptuneEvo.Accounts;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions;
using NeptuneEvo.Handles;
using NeptuneEvo.Houses;
using NeptuneEvo.VehicleData.LocalData.Models;
using Newtonsoft.Json;

namespace NeptuneEvo.Players.Phone.Cars
{
    public class Repository
    {
        public static void GetCarData(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null)
                return;
                
            var accountData = player.GetAccountData();
            if (accountData == null) 
                return;
                
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            var ownerName = String.Empty;
            var vehiclesNumber = new List<string>();

            var house = HouseManager.GetHouse(player);
            var garage = house?.GetGarageData();

            var inPark = false;
            var inGarage = false;
            
            if (house != null)
            {
                vehiclesNumber = house.GetVehiclesCarAndAirNumber();
                ownerName = house.Owner;
                
                inPark = garage != null && (garage.Type == -1 || garage.Type == 6);
                
                inGarage = garage != null && garage.InGarage(player);
            }
            else
            {
                vehiclesNumber = VehicleManager.GetVehiclesCarAndAirNumberToPlayer(sessionData.Name);
            }
            var carsOwnerData = new List<List<object>>();
            var car = sessionData.RentData;
            if (car != null)
            {
                var carData = new List<object>();
                
                carData.Add("rent");
                carData.Add(car.Model);
                carData.Add(car.Number);
                carData.Add(car.Date);
                carData.Add(Rentcar.GetRentCarCash(accountData.VipLvl, characterData.LVL, car.Price));
                carData.Add(car.IsJob);
                
                carsOwnerData.Add(carData);
            }
            
            
            carsOwnerData.AddRange(GetCarToHouse(player, ownerName, vehiclesNumber, garage, inPark));
            
            Trigger.ClientEvent(player, "client.phone.cars.init", JsonConvert.SerializeObject(carsOwnerData), inGarage, ownerName == sessionData.Name);
        }
        
        public static List<List<object>> GetCarToHouse(ExtPlayer player, string ownerName, List<string> vehiclesNumber, Garage garage = null, bool inPark = false)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null)
                    return new List<List<object>>();
                
                var carsOwnerData = new List<List<object>>();
                var carsOtherData = new List<List<object>>();
                
                foreach (var number in vehiclesNumber)
                {
                    var vehicleData = VehicleManager.GetVehicleToNumber(number);
                    if (vehicleData == null) 
                        continue;

                    var index = -1;
                    
                    if (garage != null)
                        index = garage.CarSlots
                            .Where(cs => cs.Value == vehicleData.SqlId)
                            .Select(cs => cs.Key)
                            .FirstOrDefault();

                    if (garage == null || 
                        !garage.CarSlots.ContainsKey(index) ||
                        garage.CarSlots[index] != vehicleData.SqlId)
                        index = -1;
                    
                    if (index == -1 && !vehicleData.Holder.Equals(sessionData.Name))
                        continue;
                    
                    if (inPark)
                        index = 0;
                    
                    var carData = new List<object>
                    { 
                        vehicleData.SqlId, //sqlId
                        vehicleData.Model, //model
                        number,
                        Convert.ToBoolean(garage?.IsGarageToNumber(vehicleData.SqlId)),//isCarGarage
                        index, //place
                        Ticket.IsVehicleTickets(vehicleData.SqlId), //ticket
                        VehicleModel.AirAutoRoom.isAirCar(vehicleData.Model), //isAir
                        VehicleData.LocalData.Repository.IsVehicleToNumber(VehicleAccess.Personal, number), //isCreate
                    };
                    
                    if (vehicleData.Components.ColorAdditional > 0)
                        carData.Add(vehicleData.Components.ColorAdditional); // color
                    else if (vehicleData.Components.PrimModColor != -1)
                        carData.Add(vehicleData.Components.PrimModColor); // color
                    else
                        carData.Add(vehicleData.Components.PrimColor); // color

                    if (vehicleData.Holder == sessionData.Name)//sell
                    {
                        var price = 0;

                        if (BusinessManager.BusProductsData.ContainsKey(vehicleData.Model))
                            price = MoneySystem.Wallet.GetPriceToVip(player,
                                BusinessManager.BusProductsData[vehicleData.Model].Price);

                        carData.Add(price); 
                    }

                    if (ownerName == vehicleData.Holder)
                        carsOwnerData.Add(carData);
                    else
                        carsOtherData.Add(carData);
                }
                
                carsOwnerData.AddRange(carsOtherData);

                return carsOwnerData;
            }
            catch
            {
                //Log.Write($"OpenHouseManageMenu Exception: {e.ToString()}");
            }

            return new List<List<object>>();
        }
    }
}