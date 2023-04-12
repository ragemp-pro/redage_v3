using GTANetworkAPI;
using NeptuneEvo.Handles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Redage.SDK;
using NeptuneEvo.Chars;
using NeptuneEvo.Functions;
using NeptuneEvo.Fractions;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Localization;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Jobs.Models;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;

//Disapproved by god himself

//Just use the API functions, you have nothing else to worry about

//Things to note
//More things like vehicle mods will be added in the next version

/* API FUNCTIONS:
public static void SetVehicleWindowState(Vehicle veh, WindowID window, WindowState state)
public static WindowState GetVehicleWindowState(Vehicle veh, WindowID window)
public static void SetVehicleWheelState(Vehicle veh, WheelID wheel, WheelState state)
public static WheelState GetVehicleWheelState(Vehicle veh, WheelID wheel)
public static void SetVehicleDirt(Vehicle veh, float dirt)
public static float GetVehicleDirt(Vehicle veh)
public static void SetDoorState(Vehicle veh, DoorID door, DoorState state)
public static DoorState GetDoorState(Vehicle veh, DoorID door)
public static void SetEngineState(Vehicle veh, bool status)
public static bool GetEngineState(Vehicle veh)
public static void SetLockStatus(Vehicle veh, bool status)
public static bool GetLockState(Vehicle veh)
*/

namespace NeptuneEvo.Core
{
    public class VehicleStreaming : Script
    {
        private static readonly nLog Log = new nLog("Core.VehicleSync");
        //This is the data object which will be synced to vehicle
        public static void DeleteVehicleData(ExtVehicle vehicle, VehicleAccess vehicleAccess, string numberPlate = null)
        {
            try
            {
                var vehicleLocalData = vehicle.GetVehicleLocalData();

                if (vehicleLocalData != null)
                {
                    numberPlate = vehicleLocalData.NumberPlate;
                    
                    if (vehicleLocalData.Access == VehicleAccess.Fraction && Configs.FractionDrones.ContainsKey(vehicleLocalData.Fraction) && Configs.FractionDrones[vehicleLocalData.Fraction].ContainsKey(vehicle))
                    {
                        Configs.FractionDrones[vehicleLocalData.Fraction].Remove(vehicle);
                    }
                }
                VehicleData.LocalData.Repository.Delete(vehicleAccess, numberPlate);
            }
            catch (Exception e)
            {
                Log.Write($"DeleteVehicleData Task Exception: {e.ToString()}");
            }
        }

        public static List<ExtVehicle> VehiclesDelele = new List<ExtVehicle>();
        
        public static void DeleteVehicle(ExtVehicle vehicle, Action action = null)
        {
            try
            {
                if (vehicle == null) 
                    return;
                
                if (!VehiclesDelele.Contains(vehicle))
                    VehiclesDelele.Add(vehicle);

                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                    DeleteVehicleData(vehicle, vehicleLocalData.Access);

                NAPI.Task.Run(() => 
                {
                    try
                    {
                        if (VehiclesDelele.Contains(vehicle))
                            VehiclesDelele.Remove(vehicle);
                        
                        if (vehicle.Exists) 
                            vehicle.Delete();
                    }
                    catch (Exception e)
                    {
                        Timers.Log.Write($"DeleteVehicle Task Exception: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                Timers.Log.Write($"DeleteVehicle Exception: {e.ToString()}");
            }
        }
        public static ExtVehicle CreateVehicle(uint model, Vector3 pos, float rot, int color1, int color2, string numberPlate = "", byte alpha = 255, bool locked = false, bool engine = false, uint dimension = 0, int petrol = 0, int workdriv = -1, bool cm = false, bool cd = false, bool cmk = false, string by = "NONE", VehicleAccess acc = VehicleAccess.None, int fr = -1, int numb = -1, JobsId work = JobsId.None, float dirt = 0.0f, int minrank = 100)
        {
            try
            {
                var veh = (ExtVehicle) NAPI.Vehicle.CreateVehicle(model, pos, rot, color1, color2, numberPlate, alpha, locked, engine, dimension);

                if (veh == null || !veh.Exists) return null;
                DeleteVehicleData(veh, acc, numberPlate);

                VehicleData.LocalData.Repository.VehicleNumberToHandle[acc][numberPlate] = veh;

                NAPI.Vehicle.SetVehicleNumberPlate(veh, numberPlate);
                Trigger.Dimension(veh, dimension);
                
                if (petrol <= 0)
                    petrol = 0;
                
                veh.SetVehicleLocalData(new VehicleLocalData
                {
                    Petrol = (petrol == 9999 && VehicleManager.VehicleTank.ContainsKey(veh.Class)) ? VehicleManager.VehicleTank[veh.Class] : petrol,
                    Class = veh.Class,
                    WorkDriver = workdriv,
                    //Owner = owner,
                    CanMats = cm,
                    CanDrugs = cd,
                    CanMedKits = cmk,
                    By = by,
                    Access = acc,
                    NumberPlate = numberPlate,
                    Fraction = fr,
                    MinRank = minrank,
                    BagInUse = false,
                    Number = numb,
                    MaxPetrol = (VehicleManager.VehicleTank.ContainsKey(veh.Class)) ? VehicleManager.VehicleTank[veh.Class] : 110,
                    WorkId = work,
                    ExitTime = DateTime.Now.AddMinutes(10)
                });
                veh.SetSharedData("SIRENSOUND", false);
                veh.SetSharedData("vehradio", 255);
                veh.SetSharedData("PETROL", (petrol == 9999 && VehicleManager.VehicleTank.ContainsKey(veh.Class)) ? VehicleManager.VehicleTank[veh.Class] : petrol);

                veh.SetVehicleLocalStateData(new VehicleLocalStateData());
                SetLockStatus(veh, locked);
                SetEngineState(veh, engine);
                SetVehicleDirt(veh, dirt);
                return veh;
            }
            catch (Exception e)
            {
                Log.Write($"CreateVehicle #2 Exception: {e.ToString()}");
                return null;
            }
        }

        [ServerEvent(Event.PlayerEnterVehicle)]
        public void VehStreamEnter(ExtPlayer player, ExtVehicle vehicle, sbyte seat) 
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                var vehicleStateData = vehicle.GetVehicleLocalStateData();
                if (vehicleStateData != null)
                {
                    if (vehicleStateData.Locked) 
                        VehicleManager.WarpPlayerOutOfVehicle(player);
                    /*else
                    {
                        if (sessionData.ActiveWeap.Item != null) 
                            WeaponRepository.RemoveHands(player);
                    }*/
                }
            }
            catch (Exception e)
            {
                Log.Write($"VehStreamEnter Exception: {e.ToString()}");
            }
        }

        public static void SetDoorState(ExtVehicle vehicle, DoorId door, DoorState state)
        {
            try
            {
                var vehicleStateData = vehicle.GetVehicleLocalStateData();
                if (vehicleStateData != null)
                {
                    vehicleStateData.Door[(int)door] = (int)state;

                    Trigger.ClientEventInRange(vehicle.Position, 250f, "client.vehicle.door", vehicle, door, state);

                    vehicle.SetSharedData("vDoor", JsonConvert.SerializeObject(vehicleStateData.Door));
                    //veh.SetSharedData("VehicleSyncData", JsonConvert.SerializeObject(data));
                }
            }
            catch (Exception e)
            {
                Log.Write($"SetDoorState Exception: {e.ToString()}");
            }
        }

        public static DoorState GetDoorState(ExtVehicle vehicle, DoorId door)
        {
            try
            {
                var vehicleStateData = vehicle.GetVehicleLocalStateData();
                if (vehicleStateData != null) 
                    return (DoorState)vehicleStateData.Door[(int)door];
                return DoorState.DoorClosed;
            }
            catch (Exception e)
            {
                Log.Write($"GetDoorState Exception: {e.ToString()}");
                return DoorState.DoorClosed;
            }
        }

        public static void SetEngineState(ExtVehicle vehicle, bool status)
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    var vehicleStateData = vehicle.GetVehicleLocalStateData();
                    if (vehicleStateData != null)
                    {
                        vehicle.EngineStatus = status;

                        vehicleStateData.Engine = status;
                        vehicleStateData.RightIL = false;
                        vehicleStateData.LeftIL = false;
                        vehicle.SetSharedData("vEngine", Convert.ToInt32(status));
                        vehicle.SetSharedData("vIL", $"0|0");
                        //veh.SetSharedData("VehicleSyncData", JsonConvert.SerializeObject(data));
                    }
                }
                catch (Exception e)
                {
                    Log.Write($"SetEngineState Task Exception: {e.ToString()}");
                }
            });
        }

        public static bool GetEngineState(ExtVehicle veh)
        {
            try
            {                            
                var vehicleStateData = veh.GetVehicleLocalStateData();
                if (vehicleStateData != null) 
                    return vehicleStateData.Engine;
                return false;
            }
            catch (Exception e)
            {
                Log.Write($"GetEngineState Exception: {e.ToString()}");
                return false;
            }
            
        }
        public static bool GetVehicleIndicatorLights(ExtVehicle veh, bool light) 
        {
            try
            {
                var vehicleStateData = veh.GetVehicleLocalStateData();
                if (vehicleStateData != null)
                {
                    if (!light) 
                        return vehicleStateData.RightIL;
                    return vehicleStateData.LeftIL;
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Write($"GetVehicleIndicatorLights Exception: {e.ToString()}");
                return false;
            }
        }

        public static void SetLockStatus(ExtVehicle veh, bool status)
        {
            try
            {
                var vehicleStateData = veh.GetVehicleLocalStateData();
                if (vehicleStateData != null)
                {
                    vehicleStateData.Locked = status;
                    veh.Locked = status;

                    veh.SetSharedData("vLock", Convert.ToInt32(status));
                    //veh.SetSharedData("VehicleSyncData", JsonConvert.SerializeObject(data));
                    //Trigger.ClientEventInRange(veh.Position, 250, "VehStream_SetLockStatus", veh, status);
                }
            }
            catch (Exception e)
            {
                Log.Write($"SetLockStatus Exception: {e.ToString()}");
            }
        }

        public static bool GetLockState(ExtVehicle veh)
        {
            try
            {
                var vehicleStateData = veh.GetVehicleLocalStateData();
                if (vehicleStateData != null) 
                    return vehicleStateData.Locked;
                return false;
            }
            catch (Exception e)
            {
                Log.Write($"GetLockState Exception: {e.ToString()}");
                return false;
            }
        }
        
        [RemoteEvent("server.vehicle.updateMileage")]
        public void UpdateMileage(ExtPlayer player, int vehicleId, int centimeters)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            var vehicle = (ExtVehicle) player.Vehicle;
                
            var vehicleLocalData = vehicle.GetVehicleLocalData();
            if (vehicleLocalData == null || vehicle.Value != vehicleId)
                return;

            vehicleLocalData.Centimeters += (uint) centimeters;
            var mileage = Convert.ToInt32(centimeters / 100000);

            if (mileage > 0)
            {
                if (mileage > 3)
                    mileage = 3;
                
                vehicleLocalData.Mileage += mileage;
                

                switch (vehicleLocalData.WorkId)
                {
                    case JobsId.Taxi:
                        if (characterData.UUID != vehicleLocalData.WorkDriver)
                            return;
                        
                        Players.Phone.Taxi.Orders.Repository.TaxiPay(player, mileage);
                        
                        break;
                }
            }
        }
        
        [RemoteEvent("VehStream_RadioChange")]
        public void VehStreamRadioChange(ExtPlayer player, short index)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (!player.IsInVehicle) return;
                NAPI.Data.SetEntitySharedData(player.Vehicle, "vehradio", index);
            }
            catch (Exception e)
            {
                Log.Write($"VehStreamRadioChange Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("VehStream_SetVehicleDirt")]
        public void SetVehicleDirtLevel(ExtPlayer player, ExtVehicle vehicle, float dirt)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (vehicle != null) SetVehicleDirt(vehicle, dirt);
            }
            catch (Exception e)
            {
                Log.Write($"SetVehicleDirtLevel Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("VehStream_SetIndicatorLightsData")]
        public void VehStreamSetIndicatorLightsData(ExtPlayer player, ExtVehicle veh, bool isLeft, bool isRight)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                var vehicleStateData = veh.GetVehicleLocalStateData();
                if (vehicleStateData != null)
                {
                    if (vehicleStateData.Engine)
                    {
                        if(isRight && !isLeft) 
                        {
                            vehicleStateData.RightIL = !vehicleStateData.RightIL;
                            vehicleStateData.LeftIL = false;
                        }
                        else if(isLeft && !isRight) 
                        {
                            vehicleStateData.LeftIL = !vehicleStateData.LeftIL;
                            vehicleStateData.RightIL = false;
                        }
                        else if(isLeft && isRight) 
                        {
                            if(vehicleStateData.RightIL && vehicleStateData.LeftIL)
                            {
                                vehicleStateData.RightIL = false;
                                vehicleStateData.LeftIL = false;
                            } 
                            else
                            {
                                vehicleStateData.RightIL = true;
                                vehicleStateData.LeftIL = true;
                            }
                        }
                        veh.SetSharedData("vIL", $"{Convert.ToInt32(vehicleStateData.RightIL)}|{Convert.ToInt32(vehicleStateData.LeftIL)}");

                        //veh.SetSharedData("VehicleSyncData", JsonConvert.SerializeObject(data));

                        //Trigger.ClientEventInRange(veh.Position, 250, "VehStream_SetVehicleIndicatorLights", veh.Handle, leftstate, rightstate);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"VehStreamSetIndicatorLightsData Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.setvehdirt)]
        public static void setvehdirt(ExtPlayer player, float dirt)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.setvehdirt)) return;
                if (player.Vehicle != null) 
                    SetVehicleDirt((ExtVehicle)player.Vehicle, dirt);
            }
            catch (Exception e)
            {
                Log.Write($"setvehdirt Exception: {e.ToString()}");
            }
        }

        public static void SetVehicleDirt(ExtVehicle veh, float dirt)
        {
            try
            {
                var vehicleStateData = veh.GetVehicleLocalStateData();
                if (vehicleStateData != null)
                {
                    vehicleStateData.Dirt = dirt;
                    
                    var vehicleData = VehicleManager.GetVehicleToNumber(veh.NumberPlate);
                    if (vehicleData != null)
                        vehicleData.Dirt = dirt;
                    
                    veh.SetSharedData("vDirt", dirt);
                    //trailer.SetSharedData("VehicleSyncData", JsonConvert.SerializeObject(data));
                }
            }
            catch (Exception e)
            {
                Log.Write($"SetVehicleDirt Exception: {e.ToString()}");
            }
        }

        public static float GetVehicleDirt(ExtVehicle veh)
        {
            try
            {
                var vehicleStateData = veh.GetVehicleLocalStateData();
                if (vehicleStateData != null) 
                    return vehicleStateData.Dirt;
                return 0.0f;
            }
            catch (Exception e)
            {
                Log.Write($"GetVehicleDirt Exception: {e.ToString()}");
                return 0.0f;
            }
        }
        /*[ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            CustomColShape.CreateCylinderColShape(new Vector3(501.410, -1336.284, 28.5), 3f, 2, 0, ColShapeEnums.Repair);
            NAPI.Marker.CreateMarker(1, new Vector3(501.410, -1336.284, 27.3), new Vector3(), new Vector3(), 3, new Color(255, 255, 255, 220));
            NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Нажмите\n~r~'Взаимодействие'"), new Vector3(501.410, -1336.284, 29.0), 5F, 0.4F, 0, new Color(255, 255, 255));
        }
        [Interaction(ColShapeEnums.Repair)]
        public static void OnRepair(ExtPlayer player)
        {
            if (!player.IsCharacterData()) return;
            else if (!player.IsInVehicle) return;
            Trigger.ClientEvent(player, "openDialog", "RepairMyVeh", LangFunc.GetText(LangType.Ru, DataName.RepairSelf));
            return;
        }*/
    }
}
