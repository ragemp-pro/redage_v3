using System;
using System.Collections.Generic;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using NeptuneEvo.Quests.Models;
using NeptuneEvo.VehicleData.LocalData.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.VehicleModel
{
    public class AirAutoRoom : Script
    {
        public static readonly nLog Log = new nLog("VehicleModel.AirAutoRoom");


        private static List<string> AirCarList = new List<string>
        {
            "akula",
            "annihilator",
            "annihilator2",
            "buzzard",
            "buzzard2",
            "cargobob",
            "cargobob2",
            "cargobob3",
            "cargobob4",
            "frogger",
            "frogger",
            "havok",
            "hunter",
            "maverick",
            "savage",
            "seasparrow",
            "seasparrow2",
            "seasparrow3",
            "skylift",
            "supervolito",
            "supervolito2",
            "swift",
            "swift2",
            "valkyrie",
            "valkyrie2",
            "volatus",
        };

        public static Vector3 NpcBuyPosition = new Vector3(-895.0768, -2400.8203, 14.024358);
        private static float NpcBuyRotation = 169.37247f;
        //
        public static Vector3 NpcSpawnPosition = new Vector3(-750.6089, -1510.7106, 5.003792);
        private static float NpcSpawnRotation = -22.12805f;
        //
        public static string NpcName = "npc_airshop";
        [ServerEvent(Event.ResourceStart)]
        public void Event_ResourceStart()
        {
 
            Main.CreateBlip(new Main.BlipData(359, "AirShop", NpcBuyPosition, 4, true));
            PedSystem.Repository.CreateQuest("csb_reporter", NpcBuyPosition, NpcBuyRotation, title: "~y~NPC~w~ Продавец воздушного транспорта", colShapeEnums: ColShapeEnums.AirAutoRoom);
            //
            Main.CreateBlip(new Main.BlipData(602, "HeliPort", NpcSpawnPosition, 60, true));
            PedSystem.Repository.CreateQuest("s_m_y_airworker", NpcSpawnPosition, NpcSpawnRotation, title: "~y~NPC~w~ Работник Аэропорта", colShapeEnums: ColShapeEnums.AirSpawn);
        }
        
        public static bool isAirCar(string model)
        {
            model = model.ToLower();
            return AirCarList.Contains(model);
        }
        
        public static bool isAirCar(uint model)
        {
            foreach (var modelName in AirCarList)
                if (NAPI.Util.GetHashKey(modelName) == model)
                    return true;
            return false;
        }
        
        [Interaction(ColShapeEnums.AirAutoRoom)]
        public static void OpenDialog(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (sessionData.CuffedData.Cuffed)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsCuffed), 3000);
                    return;
                }
                if (sessionData.DeathData.InDeath)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsDying), 3000);
                    return;
                }
                if (Main.IHaveDemorgan(player, true)) return;

                player.SelectQuest(new PlayerQuestModel(NpcName, 0, 0, false, DateTime.Now));
                Trigger.ClientEvent(player, "client.quest.open", index, NpcName, 0, 0, 0);
            }
            catch (Exception e)
            {
                Log.Write($"OpenDialog Exception: {e.ToString()}");
            }
        }

        private static List<string> Airs = new List<string>
        {
            "Havok",
            "Seasparrow2",
            "Seasparrow",
            "Buzzard2",
        };
        
        public static void Perform(ExtPlayer player)
        {
            try
            {
                if (!FunctionsAccess.IsWorking("OpenDonateAutoroom"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (sessionData.CuffedData.Cuffed)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsCuffed), 3000);
                    return;
                }
                if (sessionData.DeathData.InDeath)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsDying), 3000);
                    return;
                }
                if (Main.IHaveDemorgan(player, true)) return;

                characterData.ExteriorPos = player.Position;
                //NAPI.Entity.SetEntityPosition(player, new Vector3(CarRoom.CamPosition.X, CarRoom.CamPosition.Y - 2, CarRoom.CamPosition.Z));
                Trigger.UniqueDimension(player);

                CarRoom.OpenCarromMenuGos(player, Airs, false);
            }
            catch (Exception e)
            {
                Log.Write($"Perform Exception: {e.ToString()}");
            }
        }

        private static int SelectAirPosition = 0;
        private static List<(Vector3, Vector3)> AirPositions = new List<(Vector3, Vector3)>
        {
            (new Vector3 ( -721.88293, -1472.1182, 5.000522), new Vector3 (0.00595103, 0.055604856, 139.42119)),
            (new Vector3 (-762.0474,-1453.6086, 5.0005232), new Vector3 (0.01658115, 0.055898327, 140.61893)),
            (new Vector3 (-745.70825, -1433.6156, 5.0005226), new Vector3 (0.00092068175, 0.05345805, 140.28339)),
        };

        public static (Vector3, Vector3) GetSpawnPosition()
        {
            var positionData = AirPositions[SelectAirPosition];
            
            SelectAirPosition++;
            if (SelectAirPosition >= AirPositions.Count)
                SelectAirPosition = 0;
                
            return positionData;
        }
        
        public static bool IsVehicleToSpawn(Vector3 position)
        {
            
            foreach (var data in AirPositions)
            {
                if (position.DistanceTo2D(data.Item1) < 10f)
                    return true;
            }
            
            return false;
        }

        [Interaction(ColShapeEnums.AirSpawn)]
        public static void OpenAirSpawn(ExtPlayer player, int index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            if (!player.IsCharacterData()) return;
            
            if (!FunctionsAccess.IsWorking("airspawn"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                return;
            }
            
            var carsData = new List<Dictionary<string, object>>();
            
            var vehiclesNumber = VehicleManager.GetVehiclesAirNumberToPlayer(player.Name);
            foreach (string number in vehiclesNumber)
            {
                var vehicleData = VehicleManager.GetVehicleToNumber(number);
                if (vehicleData == null) continue;
                var price = 0;
                    
                if (BusinessManager.BusProductsData.ContainsKey(vehicleData.Model))
                    price = MoneySystem.Wallet.GetPriceToVip(player, BusinessManager.BusProductsData[vehicleData.Model].Price);
                
                var carData = new Dictionary<string, object>
                { 
                    {"Model", vehicleData.Model},
                    {"Number", number},
                    {"IsSpawn", VehicleData.LocalData.Repository.IsVehicleToNumber(VehicleAccess.Personal, number)},
                    {"Price", price},
                };
                carsData.Add(carData);
            }
            
            Trigger.ClientEvent(player, "client.vehicleair.open",JsonConvert.SerializeObject(carsData));
        }
    }
}