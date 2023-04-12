using System;
using System.Collections.Generic;
using GTANetworkAPI;
using NeptuneEvo.Accounts;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Handles;
using NeptuneEvo.Houses;
using Newtonsoft.Json;

namespace NeptuneEvo.Players.Phone.Property.House
{
    public class Repository
    {
        public static void GetHouseData(ExtPlayer player)
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
            
            var menuAccess = new List<string>()
            {
                "Garage"
            };
            
            var residentsData = new Dictionary<string, ResidentData>();
            var houseData = new Dictionary<string, object>();
            var housePos = new Vector3();
            var garagesData = new Dictionary<int, GarageType>();
            var ownerName = String.Empty;
            var houseFurnitures = new List<Dictionary<string, object>>();
            
            //
            
            var vehiclesNumber = new List<string>();
            
            //
            
            var house = HouseManager.GetHouse(player);
            
            var garage = house?.GetGarageData();
            var inPark = false;
            var isOwner = false;
            var residentData = new ResidentData();
            
            if (house != null)
            {
                vehiclesNumber = house.GetVehiclesCarAndAirNumber();
                
                ownerName = house.Owner;
                
                isOwner = house.Owner.Equals(sessionData.Name);
                
                houseData.Add("isOwner", isOwner);
                
                var inGarage = garage != null && garage.InGarage(player);

                if (!isOwner)
                {
                    if (house.Roommates.ContainsKey(sessionData.Name))
                        residentData = house.Roommates[sessionData.Name];
                }
                
                if ((isOwner || residentData.isPark) && inGarage)
                {
                    menuAccess.Add("inGarage");
                    if (garage.Type != 6)
                    {
                        if (house.OpenInterface == String.Empty)
                        {
                            menuAccess.Add("inPark");
                            house.OpenInterface = sessionData.Name;
                        }
                        else
                        {
                            houseData.Add("useGarage", house.OpenInterface);
                        }
                    }
                }

                //

                if (garage != null)
                {
                    houseData.Add("garageType", garage.Type);
                    garagesData = GarageManager.GarageTypes;
                }
            }
            else
            {
                vehiclesNumber = VehicleManager.GetVehiclesCarAndAirNumberToPlayer(sessionData.Name);
            }
            
            if (house != null)
            {
                inPark = garage != null && (garage.Type == -1 || garage.Type == 6);
                
                var inHouse = house.ID == characterData.InsideHouseID;

                if (inHouse && FurnitureManager.HouseFurnitures.ContainsKey(house.ID))
                {
                    foreach (var houseFurniture in FurnitureManager.HouseFurnitures[house.ID].Values)
                    {
                        houseFurnitures.Add(new Dictionary<string, object>()
                        {
                            { "Id", houseFurniture.Id },
                            { "Name", houseFurniture.Name },
                            { "IsSet", houseFurniture.IsSet },
                            { "Model", houseFurniture.Model },
                        });
                    }
                }
                
                if (!isOwner)
                    menuAccess.Add("leavehouse");
                
                housePos = house.Position;
                
                houseData.Add("price", house.Price);
                houseData.Add("sellPrice", MoneySystem.Wallet.GetPriceToVip(player, house.Price));

                //house.GarageID != characterData.InsideGarageID
                
                if (inHouse && isOwner)
                    menuAccess.Add("upgrades");
                
                if (inHouse && (isOwner || residentData.isFurniture))
                    menuAccess.Add("furniture");

                if (inHouse && isOwner)
                    menuAccess.Add("removeall");

                if (isOwner)
                {
                    menuAccess.Add("roommates");
                    residentsData = house.Roommates;
                    menuAccess.Add("sell");
                }
                
                //
                
                if (inHouse && house.Healkit)
                    houseData.Add("Healkit", true);
                
                if (inHouse && house.Alarm)
                    houseData.Add("Alarm", true);
            }

            //
            
            
            var carsOwnerData = Cars.Repository.GetCarToHouse(player, ownerName, vehiclesNumber, garage, inPark);

            Trigger.ClientEvent(player, "client.phone.house.init", 
                JsonConvert.SerializeObject(menuAccess), 
                JsonConvert.SerializeObject(carsOwnerData), 
                JsonConvert.SerializeObject(residentsData),
                JsonConvert.SerializeObject(houseData), 
                JsonConvert.SerializeObject(housePos),
                JsonConvert.SerializeObject(garagesData),
                JsonConvert.SerializeObject(houseFurnitures));
        }
    }
}