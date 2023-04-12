using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using GTANetworkAPI;
using LinqToDB;
using Localization;
using NeptuneEvo.Accounts;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.Houses;
using NeptuneEvo.Organizations;
using NeptuneEvo.Organizations.Models;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.PedSystem.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Quests.Models;
using NeptuneEvo.Table.Models;
using NeptuneEvo.Table.Tasks.Models;
using NeptuneEvo.Table.Tasks.Player;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;
using NeptuneEvo.VehicleData.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Fractions
{
    class Ticket : Script
    {
        private static readonly nLog Log = new nLog("Fractions.Ticket");

        public static string QuestName = "npc_carevac";
        //Координаты
        private static Vector3 ImpoundLotPos = new Vector3(463.67065, -1070.8586, 29.211897);
        //
        public static Vector3 PedPos = new Vector3(470.6959, -1059.139, 29.211658);
        private static float PedRot = 175.875656f;
        //
        private static int Cost = 10;
        
        public static List<VehicleTicket> VehicleTickets = new List<VehicleTicket>();
        
        public static async void OnResourceStart()
        {
            await using var db = new ServerBD("MainDB");

            var data = db.Vehicleticket
                .Where(vt => !vt.Toggled)
                .ToList();

            foreach (var vehicleTicket in data)
            {
                VehicleTickets.Add(new VehicleTicket
                {
                    AutoId = vehicleTicket.AutoId,
                    VehAutoId = vehicleTicket.VehAutoId,
                    VehNumber = vehicleTicket.VehNumber,
                    Model = vehicleTicket.Model,
                    HolderAutoId = vehicleTicket.HolderAutoId,
                    HolderName = vehicleTicket.HolderName,
                    PolicAutoId = vehicleTicket.PolicAutoId,
                    PolicName = vehicleTicket.PolicName,
                    Text = vehicleTicket.Text,
                    Link = vehicleTicket.Link,
                    Time = vehicleTicket.Time,
                    Price = vehicleTicket.Price,
                    IsEvac = vehicleTicket.IsEvac,
                    Type = (VehicleTicketType) vehicleTicket.Type
                });
            }
            
            //
            
            CustomColShape.CreateSphereColShape(ImpoundLotPos, 5f, 0, ColShapeEnums.ImpoundLot);
            NAPI.Marker.CreateMarker(36, ImpoundLotPos + new Vector3(0, 0, 0.5f), new Vector3(), new Vector3(), 3.5f,
                new Color(255, 255, 255, 220));
            
            //
            PedSystem.Repository.CreateQuest("s_m_m_ciasec_01", PedPos, PedRot, questName: QuestName, title: "~y~NPC~w~ Роберт\nСотрудник полиции", colShapeEnums: ColShapeEnums.ImpoundLotPed, pedBlipData: new PedBlipData
            {
                BlipId = 783,
                BlipName = "Штрафстоянка",
                BlipColor = 59

            });
        
        }
        [Interaction(ColShapeEnums.ImpoundLot, In: true)]
        public static void OnImpoundLot(ExtPlayer player)
        {
            try
            {
            
                if (!player.IsFractionAccess(RankToAccess.VehicleTicket)) 
                    return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                var accountData = player.GetAccountData();
                if (accountData == null) 
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                var vehicle = (ExtVehicle) player.Vehicle;
                
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData == null)
                    return;
                
                if (vehicle.Model != NAPI.Util.GetHashKey("flatbed"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustEvac), 3000);
                    return;
                }
                if (!vehicleLocalData.IsFbAttach)
                    return;

                vehicleLocalData.IsFbAttach = false;
                vehicle.SetSharedData("fbAttach", false);

                int payment = Convert.ToInt32(sessionData.TicketCost * Cost * Main.ServerSettings.MoneyMultiplier);

                int maxpayment = 4000 * Main.ServerSettings.MoneyMultiplier;
                if (payment > maxpayment) payment = maxpayment;
                
                
                MoneySystem.Wallet.Change(player, payment);
                GameLog.Money($"player({characterData.UUID})", $"server", payment, $"ImpoundLot");
                
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucEvac,payment), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"Interaction Exception: {e.ToString()}");
            }
        }
        [Interaction(ColShapeEnums.ImpoundLotPed)]
        private static void Open(ExtPlayer player, int index)
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

            player.SelectQuest(new PlayerQuestModel(QuestName, 0, 0, false, DateTime.Now));
            Trigger.ClientEvent(player, "client.quest.open", index, QuestName, 0, 0, 0);
        }

        private static IReadOnlyList<uint> CarsException = new List<uint>
        {
            NAPI.Util.GetHashKey("dinghy"),
            NAPI.Util.GetHashKey("dinghy2"),
            NAPI.Util.GetHashKey("dinghy3"),
            NAPI.Util.GetHashKey("dinghy4"),
            NAPI.Util.GetHashKey("jetmax"),
            NAPI.Util.GetHashKey("marquis"),
            NAPI.Util.GetHashKey("seashark"),
            NAPI.Util.GetHashKey("seashark2"),
            NAPI.Util.GetHashKey("seashark3"),
            NAPI.Util.GetHashKey("speeder"),
            NAPI.Util.GetHashKey("speeder2"),
            NAPI.Util.GetHashKey("squalo"),
            NAPI.Util.GetHashKey("submersible"),
            NAPI.Util.GetHashKey("submersible2"),
            NAPI.Util.GetHashKey("suntrap"),
            NAPI.Util.GetHashKey("toro"),
            NAPI.Util.GetHashKey("toro2"),
            NAPI.Util.GetHashKey("tropic"),
            NAPI.Util.GetHashKey("tropic2"),
            NAPI.Util.GetHashKey("tug"),
            NAPI.Util.GetHashKey("avisa"),
            NAPI.Util.GetHashKey("dinghy5"),
            NAPI.Util.GetHashKey("kosatka"),
            NAPI.Util.GetHashKey("longfin"),
            NAPI.Util.GetHashKey("patrolboat"),
            NAPI.Util.GetHashKey("benson"),
            NAPI.Util.GetHashKey("biff"),
            NAPI.Util.GetHashKey("cerberus"),
            NAPI.Util.GetHashKey("cerberus2"),
            NAPI.Util.GetHashKey("cerberus3"),
            NAPI.Util.GetHashKey("hauler"),
            NAPI.Util.GetHashKey("hauler2"),
            NAPI.Util.GetHashKey("mule"),
            NAPI.Util.GetHashKey("mule2"),
            NAPI.Util.GetHashKey("mule3"),
            NAPI.Util.GetHashKey("mule4"),
            NAPI.Util.GetHashKey("packer"),
            NAPI.Util.GetHashKey("phantom"),
            NAPI.Util.GetHashKey("phantom2"),
            NAPI.Util.GetHashKey("phantom3"),
            NAPI.Util.GetHashKey("pounder"),
            NAPI.Util.GetHashKey("pounder2"),
            NAPI.Util.GetHashKey("stockade3"),
            NAPI.Util.GetHashKey("terbyte"),
            NAPI.Util.GetHashKey("predator"),
            NAPI.Util.GetHashKey("firetruk"),
            NAPI.Util.GetHashKey("akula"),
            NAPI.Util.GetHashKey("annihilator"),
            NAPI.Util.GetHashKey("cargobob"),
            NAPI.Util.GetHashKey("cargobob2"),
            NAPI.Util.GetHashKey("cargobob3"),
            NAPI.Util.GetHashKey("cargobob4"),
            NAPI.Util.GetHashKey("frogger"),
            NAPI.Util.GetHashKey("hunter"),
            NAPI.Util.GetHashKey("savage"),
            NAPI.Util.GetHashKey("skylift"),
            NAPI.Util.GetHashKey("swift"),
            NAPI.Util.GetHashKey("swift2"),
            NAPI.Util.GetHashKey("valkyrie"),
            NAPI.Util.GetHashKey("valkyrie2"),
            NAPI.Util.GetHashKey("volatus"),
            NAPI.Util.GetHashKey("annihilator2"),
            NAPI.Util.GetHashKey("bulldozer"),
            NAPI.Util.GetHashKey("cutter"),
            NAPI.Util.GetHashKey("dump"),
            NAPI.Util.GetHashKey("flatbed"),
            NAPI.Util.GetHashKey("handler"),
            NAPI.Util.GetHashKey("mixer"),
            NAPI.Util.GetHashKey("mixer2"),
            NAPI.Util.GetHashKey("rubble"),
            NAPI.Util.GetHashKey("tiptruck"),
            NAPI.Util.GetHashKey("tiptruck2"),
            NAPI.Util.GetHashKey("barracks"),
            NAPI.Util.GetHashKey("barracks2"),
            NAPI.Util.GetHashKey("barracks3"),
            NAPI.Util.GetHashKey("chernobog"),
            NAPI.Util.GetHashKey("vetir"),
            NAPI.Util.GetHashKey("avenger"),
            NAPI.Util.GetHashKey("avenger2"),
            NAPI.Util.GetHashKey("besra"),
            NAPI.Util.GetHashKey("blimp"),
            NAPI.Util.GetHashKey("blimp2"),
            NAPI.Util.GetHashKey("blimp3"),
            NAPI.Util.GetHashKey("bombushka"),
            NAPI.Util.GetHashKey("cargoplane"),
            NAPI.Util.GetHashKey("cuban800"),
            NAPI.Util.GetHashKey("dodo"),
            NAPI.Util.GetHashKey("hydra"),
            NAPI.Util.GetHashKey("jet"),
            NAPI.Util.GetHashKey("lazer"),
            NAPI.Util.GetHashKey("luxor"),
            NAPI.Util.GetHashKey("luxor2"),
            NAPI.Util.GetHashKey("miljet"),
            NAPI.Util.GetHashKey("nimbus"),
            NAPI.Util.GetHashKey("shamal"),
            NAPI.Util.GetHashKey("strikeforce"),
            NAPI.Util.GetHashKey("titan"),
            NAPI.Util.GetHashKey("tula"),
            NAPI.Util.GetHashKey("volatol"),
            NAPI.Util.GetHashKey("alkonost"),
            NAPI.Util.GetHashKey("airbus"),
            NAPI.Util.GetHashKey("brickade"),
            NAPI.Util.GetHashKey("bus"),
            NAPI.Util.GetHashKey("coach"),
            NAPI.Util.GetHashKey("pbus2"),
            NAPI.Util.GetHashKey("rallytruck"),
            NAPI.Util.GetHashKey("rentalbus"),
            NAPI.Util.GetHashKey("tourbus"),
            NAPI.Util.GetHashKey("trash"),
            NAPI.Util.GetHashKey("trash2"),
            NAPI.Util.GetHashKey("wastelander"),
            NAPI.Util.GetHashKey("armytanker"),
            NAPI.Util.GetHashKey("armytrailer"),
            NAPI.Util.GetHashKey("armytrailer2"),
            NAPI.Util.GetHashKey("baletrailer"),
            NAPI.Util.GetHashKey("boattrailer"),
            NAPI.Util.GetHashKey("cablecar"),
            NAPI.Util.GetHashKey("docktrailer"),
            NAPI.Util.GetHashKey("freighttrailer"),
            NAPI.Util.GetHashKey("graintrailer"),
            NAPI.Util.GetHashKey("proptrailer"),
            NAPI.Util.GetHashKey("raketrailer"),
            NAPI.Util.GetHashKey("tr2"),
            NAPI.Util.GetHashKey("tr3"),
            NAPI.Util.GetHashKey("tr4"),
            NAPI.Util.GetHashKey("trflat"),
            NAPI.Util.GetHashKey("tvtrailer"),
            NAPI.Util.GetHashKey("tanker"),
            NAPI.Util.GetHashKey("tanker2"),
            NAPI.Util.GetHashKey("trailerlarge"),
            NAPI.Util.GetHashKey("trailerlogs"),
            NAPI.Util.GetHashKey("trailersmall"),
            NAPI.Util.GetHashKey("trailers"),
            NAPI.Util.GetHashKey("trailers2"),
            NAPI.Util.GetHashKey("trailers3"),
            NAPI.Util.GetHashKey("trailers4"),
            NAPI.Util.GetHashKey("freight"),
            NAPI.Util.GetHashKey("freightcar"),
            NAPI.Util.GetHashKey("freightcont1"),
            NAPI.Util.GetHashKey("freightcont2"),
            NAPI.Util.GetHashKey("freightgrain"),
            NAPI.Util.GetHashKey("metrotrain"),
            NAPI.Util.GetHashKey("tankercar"),
            NAPI.Util.GetHashKey("slamtruck"),
            NAPI.Util.GetHashKey("scrap"),
        };
        
        public static bool CarException(uint model) => CarsException.Contains(model);
        
        public static void OpenTicket(ExtPlayer player, ExtVehicle vehicle)
        {
            
            if (!player.IsFractionAccess(RankToAccess.VehicleTicket)) 
                return;
            
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;

            var vehicleLocalData = vehicle.GetVehicleLocalData();
            if (vehicleLocalData == null)
                return;
            
            if (vehicle.Model == NAPI.Util.GetHashKey("flatbed"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoEvacEvac), 5000);
                return;
            }
            if (CarException(vehicle.Model))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantEvac), 5000);
                return;
            }

            var flatbed = VehicleManager.getNearestVehicle(player, 10, NAPI.Util.GetHashKey("flatbed"));
            if (flatbed == null || flatbed == vehicle)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoEvac), 3000);
                return;
            }

            var flatbedLocalData = flatbed.GetVehicleLocalData();
            if (flatbedLocalData == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoEvac), 3000);
                return;
            }
            
            if (flatbedLocalData.IsFbAttach)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoEvacPlace), 3000);
                return;
            }
            
            var data = new Dictionary<string, object>()
            {
                { "number", vehicle.NumberPlate },
                { "model", vehicle.Model },
            };
            
            switch (vehicleLocalData.Access)
            {
                case VehicleAccess.Garage:
                case VehicleAccess.Personal:
                    var vehicleData = VehicleManager.GetVehicleToNumber(vehicleLocalData.NumberPlate);
                    
                    if (vehicleData == null)
                        return;
                    
                    //data["model"] = vehicleData.Model;
                    data["holder"] = vehicleData.Holder;
                    break;
                
                case VehicleAccess.Fraction:
                    var fractionData = Manager.GetFractionData(vehicleLocalData.Fraction);
                    if (fractionData == null)
                        return;

                    if (!fractionData.Vehicles.ContainsKey(vehicle.NumberPlate)) return;
                    //data["model"] = Configs.FractionVehicles[fractionid][vehicle.NumberPlate].model;
                    data["holder"] = (Models.Fractions) fractionData.Id;
                    
                    switch ((SafeZones.ZoneName) sessionData.InsideSafeZone)
                    {
                        case SafeZones.ZoneName.EMS:
                        case SafeZones.ZoneName.EMSpark:
                            if ((Models.Fractions)fractionData.Id == Models.Fractions.EMS)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantEvac), 3000);
                                return;
                            }
                            break;
                        
                        case SafeZones.ZoneName.Cityhall:
                            if ((Models.Fractions)fractionData.Id == Models.Fractions.CITY)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantEvac), 3000);
                                return;
                            }
                            break;
                        case SafeZones.ZoneName.lspd:
                            if ((Models.Fractions)fractionData.Id == Models.Fractions.POLICE)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantEvac), 3000);
                                return;
                            }
                            break;
                        case SafeZones.ZoneName.govfib:
                            if ((Models.Fractions)fractionData.Id == Models.Fractions.FIB || (Models.Fractions)fractionData.Id == Models.Fractions.CITY)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantEvac), 3000);
                                return;
                            }
                            break;
                        case SafeZones.ZoneName.lsnews:
                            if ((Models.Fractions)fractionData.Id == Models.Fractions.LSNEWS)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantEvac), 3000);
                                return;
                            }
                            break;
                        case SafeZones.ZoneName.Sheriffs:
                        case SafeZones.ZoneName.Sheriffs2:
                            if ((Models.Fractions)fractionData.Id == Models.Fractions.SHERIFF)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantEvac), 3000);
                                return;
                            }
                            break;
                        case SafeZones.ZoneName.RM:
                            if ((Models.Fractions)fractionData.Id == Models.Fractions.RUSSIAN)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantEvac), 3000);
                                return;
                            }
                            break;
                        case SafeZones.ZoneName.AM:
                            if ((Models.Fractions)fractionData.Id == Models.Fractions.ARMENIAN)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantEvac), 3000);
                                return;
                            }
                            break;
                        case SafeZones.ZoneName.YAK:
                            if ((Models.Fractions)fractionData.Id == Models.Fractions.YAKUZA)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantEvac), 3000);
                                return;
                            }
                            break;
                        case SafeZones.ZoneName.LCN:
                            if ((Models.Fractions)fractionData.Id == Models.Fractions.LCN)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantEvac), 3000);
                                return;
                            }
                            break;
                        case SafeZones.ZoneName.Ballas:
                            if ((Models.Fractions)fractionData.Id == Models.Fractions.BALLAS)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantEvac), 3000);
                                return;
                            }
                            break;
                        case SafeZones.ZoneName.MG13:
                            if ((Models.Fractions)fractionData.Id == Models.Fractions.MARABUNTA)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantEvac), 3000);
                                return;
                            }
                            break;
                        case SafeZones.ZoneName.Families:
                            if ((Models.Fractions)fractionData.Id == Models.Fractions.FAMILY)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantEvac), 3000);
                                return;
                            }
                            break;
                        case SafeZones.ZoneName.Bloods:
                            if ((Models.Fractions)fractionData.Id == Models.Fractions.BLOOD)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantEvac), 3000);
                                return;
                            }
                            break;
                        case SafeZones.ZoneName.vagos:
                            if ((Models.Fractions)fractionData.Id == Models.Fractions.VAGOS)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantEvac), 3000);
                                return;
                            }
                            break;
                    }
                    
                    break;
                 
                case VehicleAccess.Organization:
                    int orgId = vehicleLocalData.Fraction; 
                    var organizationData = Organizations.Manager.GetOrganizationData(orgId);
                    if (organizationData == null) 
                        return;
                    
                    //if (!Organizations.Manager.VehiclesData.ContainsKey(orgId))
                     //   return;
                    
                    //var orgVehicles = Organizations.Manager.VehiclesData[orgId];
                    //if (!orgVehicles.ContainsKey(vehicleLocalData.NumberPlate))
                    //    return;

                    //data["model"] = orgVehicles[vehicle.NumberPlate].model;
                    data["holder"] = LangFunc.GetText(LangType.Ru, DataName.OrgWithName, organizationData.Name);
                    break;
                default:
                    data["holder"] = vehicleLocalData.Access.ToString();
                    break;
                
            }

            sessionData.TicketVehicle = vehicle;
            Trigger.ClientEvent(player, "client.ticket.open", JsonConvert.SerializeObject(data));
        }

        [RemoteEvent("server.ticket.end")]
        public void OnTicketEnd(ExtPlayer player, string ticketText, int ticketPrice, string cameraLink, bool isEvac)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var vehicleTicket = sessionData.TicketVehicle;
            sessionData.TicketVehicle = null;
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            if (!player.IsFractionAccess(RankToAccess.VehicleTicket)) 
                return;
            
            if (ticketPrice < 100 || ticketPrice > 500) {
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,LangFunc.GetText(LangType.Ru, DataName.EvacSum), 5000);
                return;
            }
            if (cameraLink.Length < 1 || ticketText.Length >= 45) {
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,LangFunc.GetText(LangType.Ru, DataName.EvacSymbols), 3000);
                return;
            }
            ticketText = Main.BlockSymbols(ticketText);
            
            var vehicleLocalData = vehicleTicket.GetVehicleLocalData();
            var vehicleStateData = vehicleTicket.GetVehicleLocalStateData();
            if (vehicleLocalData == null)
                return;
            
            ExtVehicle flatbed = null;
            VehicleLocalData flatbedLocalData = null;
            if (isEvac)
            {

                if (vehicleLocalData.ExitTime > DateTime.Now)
                {                    
                    long ticks = vehicleLocalData.ExitTime.Ticks - DateTime.Now.Ticks;
                    if (ticks <= 0) return;
                    DateTime g = new DateTime(ticks);
                    if (g.Hour >= 1) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoEvac1Hours, g.Hour, g.Minute, g.Second), 10000);
                    else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoEvac0Hours, g.Minute, g.Second), 10000);
                    return;
                }
                if (vehicleLocalData.Occupants.Count > 0 || player.Position.DistanceTo(vehicleTicket.Position) > 10)
                {                    
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoEvacSidit), 7000);
                    return;
                }
                
                flatbed = VehicleManager.getNearestVehicle(player, 10, NAPI.Util.GetHashKey("flatbed"));
                
                if (flatbed == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoEvac), 3000);
                    return;
                }

                flatbedLocalData = flatbed.GetVehicleLocalData();
                if (flatbedLocalData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoEvac), 3000);
                    return;
                }

                if (flatbedLocalData.IsFbAttach)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoEvacPlace), 3000);
                    return;
                }
            }

            var fbAttach = new VehicleCustomization();

            ExtPlayer target = null;
            
            switch (vehicleLocalData.Access)
            {
                case VehicleAccess.Garage:
                case VehicleAccess.Personal:
                    var vehicleData = VehicleManager.GetVehicleToNumber(vehicleLocalData.NumberPlate);
                    if (vehicleData == null)
                        return;

                    int uuid = Main.PlayerUUIDs.GetValueOrDefault(vehicleData.Holder);
                    if (uuid > 0)
                    {
                        var login = Main.GetLoginFromUUID(uuid);
                        if (login != null)
                            target = Accounts.Repository.GetPlayerToLogin(login);
                    }
                    
                    Insert(player, ticketText, ticketPrice, cameraLink, isEvac, VehicleTicketType.Player, uuid, vehicleData.Holder,
                        vehicleData.SqlId, vehicleLocalData.NumberPlate, vehicleTicket.Model.ToString());
                    
                    if (vehicleStateData != null)
                        vehicleData.Dirt = vehicleStateData.Dirt;
                    
                    if (isEvac) 
                    {
                        fbAttach = vehicleData.Components;
                        fbAttach.Hash = vehicleTicket.Model;
                        VehicleStreaming.DeleteVehicle(vehicleTicket);
                    }
                    break;
                case VehicleAccess.Rent:  
                case VehicleAccess.Work:    
                    var name = Main.PlayerNames.ContainsKey(vehicleLocalData.WorkDriver) ? Main.PlayerNames[vehicleLocalData.WorkDriver] : "";
                    
                    Insert(player, ticketText, ticketPrice, cameraLink, isEvac, VehicleTicketType.Player, vehicleLocalData.WorkDriver, name,
                        -1, vehicleLocalData.NumberPlate, vehicleTicket.Model.ToString());
                    
                    target = Rentcar.Event_vehicleDeath(vehicleTicket, String.Empty);
                    if (isEvac)
                    {
                        fbAttach.Hash = vehicleTicket.Model;
                        VehicleStreaming.DeleteVehicle(vehicleTicket);
                    }
                    break;
                case VehicleAccess.Hotel:
                    target = vehicleLocalData.Owner;
                    var hotelCharacterData = vehicleLocalData.Owner.GetCharacterData();
                    var hotelSessionData = vehicleLocalData.Owner.GetSessionData();
                    if (hotelSessionData != null && hotelCharacterData != null && hotelSessionData.HotelData != null && hotelSessionData.HotelData.Car == vehicleTicket)
                    {
                        hotelSessionData.HotelData.Car = null;
                        Insert(player, ticketText, ticketPrice, cameraLink, isEvac, VehicleTicketType.Player, hotelCharacterData.UUID, $"{hotelCharacterData.FirstName}_{hotelCharacterData.LastName}",
                            -1, vehicleLocalData.NumberPlate, vehicleTicket.Model.ToString());
                    }
                    if (isEvac)
                    {
                        fbAttach.Hash = vehicleTicket.Model;
                        VehicleStreaming.DeleteVehicle(vehicleTicket);
                    }
                    break;
                case VehicleAccess.Fraction:
                    var fractionData = Manager.GetFractionData(vehicleLocalData.Fraction);
                    if (fractionData == null)
                        return;
                    if (!fractionData.Vehicles.ContainsKey(vehicleTicket.NumberPlate)) return;
                    
                    if (isEvac)
                    {
                        fbAttach = fractionData.Vehicles[vehicleTicket.NumberPlate].customization;
                        fbAttach.Hash = vehicleTicket.Model;
                        Admin.RespawnFractionCar(vehicleTicket);
                        Manager.sendFractionMessage(fractionData.Id, LangFunc.GetText(LangType.Ru, DataName.EvacShtrafS, player.Name.Replace("_"," "), vehicleTicket.NumberPlate));
                    }
                    else 
                        Manager.sendFractionMessage(fractionData.Id, LangFunc.GetText(LangType.Ru, DataName.EvacShtraf, ticketPrice));
                    player.AddTableScore(TableTaskId.Item9);
                    player.AddTableScore(TableTaskId.Item15);
                    break;
                case VehicleAccess.Organization:
                    int orgId = vehicleLocalData.Fraction;
  
                    var organizationData = Organizations.Manager.GetOrganizationData(orgId);
                    if (organizationData == null) 
                        return;
                    
                    if (!organizationData.Vehicles.ContainsKey(vehicleLocalData.NumberPlate))
                        return;

                    Insert(player, ticketText, ticketPrice, cameraLink, isEvac, VehicleTicketType.Organization, orgId, organizationData.Name,
                        0, vehicleLocalData.NumberPlate, vehicleTicket.Model.ToString());
                    
                    //data["model"] = orgVehicles[vehicle.NumberPlate].model;
                    if (isEvac)
                    {
                        fbAttach = organizationData.Vehicles[vehicleLocalData.NumberPlate].customization;
                        fbAttach.Hash = vehicleTicket.Model;
                        VehicleStreaming.DeleteVehicle(vehicleTicket);
                        Manager.sendOrganizationMessage(orgId, LangFunc.GetText(LangType.Ru, DataName.EvacShtrafS, player.Name.Replace("_"," "), vehicleTicket.NumberPlate));
                    }
                    else
                        Manager.sendOrganizationMessage(orgId, LangFunc.GetText(LangType.Ru, DataName.EvacShtraf, ticketPrice));
                    player.AddTableScore(TableTaskId.Item9);
                    player.AddTableScore(TableTaskId.Item15);

                    break;
                default:
                    if (isEvac)
                    {
                        fbAttach.Hash = vehicleTicket.Model;
                        VehicleStreaming.DeleteVehicle(vehicleTicket);
                    }

                    break;
                
            }
            
            if (target.IsCharacterData()) 
                Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.EvacShtraf, ticketPrice), 6000);
            player.AddTableScore(TableTaskId.Item9);
            player.AddTableScore(TableTaskId.Item15);
            
            if (isEvac)
            {
                if (target.IsCharacterData()) 
                    Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.EvacShtrafS, player.Name.Replace("_"," "), vehicleTicket.NumberPlate), 10000);
                
                sessionData.TicketCost = Convert.ToInt32(player.Position.DistanceTo2D(PedPos) / 100);
                Army.onVehicleDeath(vehicleTicket);
                VehicleManager.Event_vehicleDeath(vehicleTicket);
                flatbed.SetSharedData("fbAttach", JsonConvert.SerializeObject(fbAttach));
                flatbedLocalData.IsFbAttach = true;
                Trigger.ClientEvent(player, "createWaypoint", Ticket.PedPos.X, Ticket.PedPos.Y);
            }
            
        }

        private static void Insert(ExtPlayer player, string ticketText, int ticketPrice, string cameraLink, bool isEvac, VehicleTicketType type, int uuid, string holder, int vehSqlId, string vehNumber, string model)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    var time = DateTime.Now;
                    model = model.ToUpper();
                    
                    int sqlId = await db.InsertWithInt32IdentityAsync(new Vehicletickets
                    {
                        VehAutoId = vehSqlId,
                        VehNumber = vehNumber,
                        Model = model,
                        HolderAutoId = uuid,
                        HolderName = holder,
                        PolicAutoId = characterData.UUID,
                        PolicName = sessionData.Name,
                        Text = ticketText,
                        Link = cameraLink,
                        Time = time,
                        Price = ticketPrice,
                        IsEvac = isEvac,
                        Toggled = false,
                        Type = (sbyte)type
                    });       
                    
                    VehicleTickets.Add(new VehicleTicket
                    {
                        AutoId = sqlId,
                        VehAutoId = vehSqlId,
                        VehNumber = vehNumber,
                        Model = model,
                        HolderAutoId = uuid,
                        HolderName = holder,
                        PolicAutoId = characterData.UUID,
                        PolicName = sessionData.Name,
                        Text = ticketText,
                        Link = cameraLink,
                        Time = time,
                        Price = ticketPrice,
                        IsEvac = isEvac,
                        Type = type
                    });
                }
                catch (Exception e)
                {
                    Log.Write($"AddAdvert Exception: {e.ToString()}");
                }
            });
        }
        private static void Delete(int autoId)
        {
            
            var vehicleTicket = VehicleTickets
                .FirstOrDefault(vt => vt.AutoId == autoId);
            
            if (vehicleTicket == null)
                return;

            VehicleTickets.Remove(vehicleTicket);
            
            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    await db.Vehicleticket
                        .Where(vt => vt.AutoId == autoId)
                        .Set(vt => vt.Toggled, true)
                        .UpdateAsync();
                }
                catch (Exception e)
                {
                    Log.Write($"AddAdvert Exception: {e.ToString()}");
                }
            });
        }
        public static bool IsVehicleTickets(int sqlId, VehicleTicketType type = VehicleTicketType.Player)
        {
            return VehicleTickets.Any(vt => vt.Type == type && vt.VehAutoId == sqlId && vt.IsEvac);
        }
        public static bool IsVehicleTickets(string number, VehicleTicketType type = VehicleTicketType.Player)
        {
            return VehicleTickets.Any(vt => vt.Type == type && vt.VehNumber == number && vt.IsEvac);
        }

        private static List<string> GetMyVehicleNumber(ExtPlayer player)
        {
            var vehiclesNumber = new List<string>();
            
            var sessionData = player.GetSessionData();
            if (sessionData != null)
            {
                var house = HouseManager.GetHouse(player);
                var garage = house?.GetGarageData();


                if (house != null)
                    vehiclesNumber = house.GetVehiclesCarAndAirNumber();
                else
                    vehiclesNumber = VehicleManager.GetVehiclesCarAndAirNumberToPlayer(sessionData.Name);
                
            }

            return vehiclesNumber;
        }
        
        private static List<int> PlayersUUID(ExtPlayer player)
        {
            var playerUUID = new List<int>();
            var characterData = player.GetCharacterData();
            if (characterData != null)
            {
                var playersName = new List<string>();
            
                playerUUID.Add(characterData.UUID);
            
                var house = HouseManager.GetHouse(player);

                if (house != null)
                {
                    playersName.Add(house.Owner);
                    playersName.AddRange(house.Roommates.Keys);
                }

                foreach (var name in playersName)
                {
                    if (Main.PlayerUUIDs.ContainsKey(name))
                    {
                        var uuid = Main.PlayerUUIDs[name];
                        if (!playerUUID.Contains(uuid))
                            playerUUID.Add(uuid);
                    }
                }
                
            }

            return playerUUID;
        }
        public static void OpenTickets(ExtPlayer player, VehicleTicketType type = VehicleTicketType.Player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            //var playersUUID = PlayersUUID(player);

            var vehiclesNumber = GetMyVehicleNumber(player);
            
            var playersName = new List<string>();
            
            var orgid = -1;
            var orgrank = 0;
            var vehiclesOrgData =
                new Dictionary<string, OrganizationVehicleData>();
            
            var memberOrganizationData = player.GetOrganizationMemberData();
            if (memberOrganizationData != null) 
            {
                var organizationData = Organizations.Manager.GetOrganizationData(memberOrganizationData.Id);
                if (organizationData != null)
                {
                    orgid = memberOrganizationData.Id;
                    orgrank = memberOrganizationData.Rank;
                    vehiclesOrgData = organizationData.Vehicles;
                }
                /*var uuid = Organizations.Manager.Organizations[orgid].OwnerUUID;
                if (!playersUUID.Contains(uuid))
                    playersUUID.Add(uuid);*/
            }

            var vehicleTickets = VehicleTickets
                .Where(vt => (vt.Type == VehicleTicketType.Player && vehiclesNumber.Contains(vt.VehNumber)) || (vt.Type == VehicleTicketType.Organization && vt.HolderAutoId == orgid && vehiclesOrgData.ContainsKey(vt.VehNumber) && orgrank >= vehiclesOrgData[vt.VehNumber].rank))
                .ToList();

            Trigger.ClientEvent(player, "client.ticket.myOpen", JsonConvert.SerializeObject(vehicleTickets));
        }

        private int SelectSpawnIndex = 0;
        private IReadOnlyList<(Vector3, Vector3)> VehicleSpawnPos = new List<(Vector3, Vector3)>
        {
            (new Vector3(389.17102, -1613.0327, 28.867743), new Vector3(-0.12968554, 0.36919695, -129.99524)),
            (new Vector3(391.03085, -1610.603, 28.867403), new Vector3(-0.1645448, 0.41398662, -128.57216)),
            (new Vector3(393.03793, -1608.1964, 28.867956), new Vector3(-0.14803357, 0.37509206, -130.45446)),
            (new Vector3(384.66913, -1634.6737, 28.870203), new Vector3(-0.10575577, 0.37968248, -39.703247)),
            (new Vector3(387.37485, -1636.9976, 28.867495), new Vector3(-0.19618103, 0.39239803, -41.425964)),
            (new Vector3(392.9341, -1628.6107, 28.867752), new Vector3(-0.12949154, 0.3730297, 49.459354)),
            (new Vector3(395.52106, -1626.6489, 28.870615), new Vector3(-0.091591954, 0.3875459, 49.467834)),
            (new Vector3(397.4613, -1624.0946, 28.867586), new Vector3(-0.2415579, 0.3786682, 51.428703)),
            (new Vector3(399.61435, -1621.7349, 28.870033), new Vector3(-0.11096678, 0.38071325, 50.469673)),
            (new Vector3(401.41287, -1619.4174, 28.867847), new Vector3(-0.19966689, 0.3825015, 51.588836)),
            (new Vector3(403.65228, -1617.2141, 28.870659), new Vector3(-0.089471556, 0.39236227, 50.03569)),
            (new Vector3(370.08325, -1623.0472, 28.870173), new Vector3(-0.10700274, 0.38090214, -42.122757)),
            (new Vector3(372.53174, -1624.8871, 28.870184), new Vector3(-0.105711415, 0.38273543, -42.05801)),
        };

        [RemoteEvent("server.ticket.payment")]
        public void OnTicketPayment(ExtPlayer player, int autoId)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            var vehicleTicket = VehicleTickets
                .FirstOrDefault(vt => vt.AutoId == autoId);

            if (vehicleTicket != null)
            {
                if (vehicleTicket.Type == VehicleTicketType.Player)
                {
                    var vehiclesNumber = GetMyVehicleNumber(player);
                    if (vehiclesNumber.Contains(vehicleTicket.VehNumber))
                    {
                        if (UpdateData.CanIChange(player, vehicleTicket.Price, true) != 255) return;
                        MoneySystem.Wallet.Change(player, -vehicleTicket.Price);
                        GameLog.Money($"player({characterData.UUID})", $"system", vehicleTicket.Price,
                            $"VehicleTicketType.Player({vehicleTicket.Price})");
                        Delete(autoId);
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter,
                            $"Вы выкупили авто со штрафстоянки.", 3000);
                        var vehicleData = VehicleManager.GetVehicleToNumber(vehicleTicket.VehNumber);
                        if (vehicleData == null)
                            return;
                        vehicleData.Position = JsonConvert.SerializeObject(VehicleSpawnPos[SelectSpawnIndex].Item1);
                        vehicleData.Rotation = JsonConvert.SerializeObject(VehicleSpawnPos[SelectSpawnIndex].Item2);
                        if (++SelectSpawnIndex >= VehicleSpawnPos.Count)
                            SelectSpawnIndex = 0;

                        var house = HouseManager.GetHouse(player, checkOwner: true);
                        var garage = house?.GetGarageData();
                        garage?.SpawnCar(vehicleTicket.VehNumber);
                        return;
                    }
                }

                if (vehicleTicket.Type == VehicleTicketType.Organization)
                {
                    var memberOrganizationData = player.GetOrganizationMemberData();
                    if (memberOrganizationData != null) 
                    {
                        var organizationData = Organizations.Manager.GetOrganizationData(memberOrganizationData.Id);
                        if (organizationData != null)
                        {
                            if (organizationData.Vehicles.ContainsKey(vehicleTicket.VehNumber) &&
                                memberOrganizationData.Rank >= organizationData.Vehicles[vehicleTicket.VehNumber].rank)
                            {
                                if (UpdateData.CanIChange(player, vehicleTicket.Price, true) != 255) return;
                                MoneySystem.Wallet.Change(player, -vehicleTicket.Price);
                                GameLog.Money($"player({characterData.UUID})", $"system", vehicleTicket.Price,
                                    $"VehicleTicketType.Organization(OrgId:{organizationData.Id})({vehicleTicket.Price})");
                                Delete(autoId);
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                                    $"Вы выкупили авто со штрафстоянки.", 3000);
                                Organizations.Manager.SpawnOrganizationCar(organizationData.Id, vehicleTicket.VehNumber,
                                    organizationData.Vehicles[vehicleTicket.VehNumber]);
                                if (++SelectSpawnIndex >= VehicleSpawnPos.Count)
                                    SelectSpawnIndex = 0;
                                return;
                            }
                        }
                    }
                }
            }
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomethingWrong), 5000);
        }
        
        
    }
}