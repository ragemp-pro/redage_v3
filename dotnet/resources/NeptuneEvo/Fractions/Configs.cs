using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using MySqlConnector;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Functions;
using NeptuneEvo.Table.Models;
using NeptuneEvo.Table.Tasks.Models;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;
using NeptuneEvo.VehicleData.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Fractions
{
    class Configs : Script
    {
        private static readonly nLog Log = new nLog("Fractions.Configs");
        public static int FractionCount = Enum.GetNames(typeof(Models.Fractions)).Length - 1;
        // fractionid - vehicle number - vehiclemodel, position, rotation, min rank, color1, color2, model
        public static Dictionary<int, Dictionary<ExtVehicle, string>> FractionDrones = new Dictionary<int, Dictionary<ExtVehicle, string>>();


        public static Dictionary<int, Dictionary<string, int>> FractionWeapons = new Dictionary<int, Dictionary<string, int>>();

        
        public static void InitFractionDefaultAccessRanks()
        {
            var fractionData = Manager.GetFractionData((int)Models.Fractions.FAMILY);
            fractionData.DefaultAccess = new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenStock, RankToAccess.StealGuns, RankToAccess.Pocket, RankToAccess.Capture, RankToAccess.CaptureJoin, RankToAccess.Cuff, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMoney, RankToAccess.BuyDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.SetVehicleRank, RankToAccess.DoorControl, RankToAccess.TableWall, RankToAccess.EditAllTabletWall, RankToAccess.Logs, RankToAccess.Reprimand, RankToAccess.CreateDepartment, RankToAccess.DeleteDepartment };

            fractionData.RanksDefaultAccess.Add(1, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(2, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(3, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(4, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(5, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(6, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(7, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Capture, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(8, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Capture, RankToAccess.CaptureJoin, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(9, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Capture, RankToAccess.CaptureJoin, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(10, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Capture, RankToAccess.CaptureJoin, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.Invite, RankToAccess.UnInvite });
            
            fractionData = Manager.GetFractionData((int)Models.Fractions.BALLAS);
            fractionData.DefaultAccess = new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenStock, RankToAccess.StealGuns, RankToAccess.Pocket, RankToAccess.Capture, RankToAccess.CaptureJoin, RankToAccess.Cuff, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMoney, RankToAccess.BuyDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.SetVehicleRank, RankToAccess.DoorControl, RankToAccess.TableWall, RankToAccess.EditAllTabletWall, RankToAccess.Logs, RankToAccess.Reprimand, RankToAccess.CreateDepartment, RankToAccess.DeleteDepartment, };
            
            fractionData.RanksDefaultAccess.Add(1, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(2, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(3, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(4, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(5, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(6, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(7, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Capture, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(8, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Capture, RankToAccess.CaptureJoin, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(9, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Capture, RankToAccess.CaptureJoin, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(10, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Capture, RankToAccess.CaptureJoin, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.Invite, RankToAccess.UnInvite });
            
            fractionData = Manager.GetFractionData((int)Models.Fractions.VAGOS);
            fractionData.DefaultAccess = new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenStock, RankToAccess.StealGuns, RankToAccess.Pocket, RankToAccess.Capture, RankToAccess.CaptureJoin, RankToAccess.Cuff, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMoney, RankToAccess.BuyDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.SetVehicleRank, RankToAccess.DoorControl, RankToAccess.TableWall, RankToAccess.EditAllTabletWall, RankToAccess.Logs, RankToAccess.Reprimand, RankToAccess.CreateDepartment, RankToAccess.DeleteDepartment, };
            
            fractionData.RanksDefaultAccess.Add(1, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(2, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(3, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(4, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(5, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(6, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(7, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Capture, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(8, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Capture, RankToAccess.CaptureJoin, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(9, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Capture, RankToAccess.CaptureJoin, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(10, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Capture, RankToAccess.CaptureJoin, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.Invite, RankToAccess.UnInvite });
            
            fractionData = Manager.GetFractionData((int)Models.Fractions.MARABUNTA);
            fractionData.DefaultAccess = new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenStock, RankToAccess.StealGuns, RankToAccess.Pocket, RankToAccess.Capture, RankToAccess.CaptureJoin, RankToAccess.Cuff, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMoney, RankToAccess.BuyDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.SetVehicleRank, RankToAccess.DoorControl, RankToAccess.TableWall, RankToAccess.EditAllTabletWall, RankToAccess.Logs, RankToAccess.Reprimand, RankToAccess.CreateDepartment, RankToAccess.DeleteDepartment, };
            
            fractionData.RanksDefaultAccess.Add(1, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(2, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(3, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(4, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(5, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(6, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(7, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Capture, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(8, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Capture, RankToAccess.CaptureJoin, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(9, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Capture, RankToAccess.CaptureJoin, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(10, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Capture, RankToAccess.CaptureJoin, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.Invite, RankToAccess.UnInvite });
            
            fractionData = Manager.GetFractionData((int)Models.Fractions.BLOOD);
            fractionData.DefaultAccess = new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenStock, RankToAccess.StealGuns, RankToAccess.Pocket, RankToAccess.Capture, RankToAccess.CaptureJoin, RankToAccess.Cuff, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMoney, RankToAccess.BuyDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.SetVehicleRank, RankToAccess.DoorControl, RankToAccess.TableWall, RankToAccess.EditAllTabletWall, RankToAccess.Logs, RankToAccess.Reprimand, RankToAccess.CreateDepartment, RankToAccess.DeleteDepartment, };

            fractionData.RanksDefaultAccess.Add(1, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(2, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(3, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(4, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(5, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(6, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(7, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Capture, RankToAccess.CaptureJoin });
            fractionData.RanksDefaultAccess.Add(8, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Capture, RankToAccess.CaptureJoin, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(9, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Capture, RankToAccess.CaptureJoin, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(10, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Capture, RankToAccess.CaptureJoin, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.Invite, RankToAccess.UnInvite });
            
            fractionData = Manager.GetFractionData((int)Models.Fractions.CITY);
            fractionData.DefaultAccess = new List<RankToAccess>() { RankToAccess.OpenStock, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.Gov, RankToAccess.Dep, RankToAccess.OpenWeaponStock, RankToAccess.SetVehicleRank, RankToAccess.DoorControl, RankToAccess.TableWall, RankToAccess.EditAllTabletWall, RankToAccess.Logs, RankToAccess.Reprimand, RankToAccess.Dron, RankToAccess.Su, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.CreateDepartment, RankToAccess.DeleteDepartment, RankToAccess.ClothesEdit, RankToAccess.OpenGunStock };

            fractionData.RanksDefaultAccess.Add(1, new List<RankToAccess>() { RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(2, new List<RankToAccess>() { RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(3, new List<RankToAccess>() { RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(4, new List<RankToAccess>() { RankToAccess.OpenWeaponStock, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Follow });
            fractionData.RanksDefaultAccess.Add(5, new List<RankToAccess>() { RankToAccess.OpenWeaponStock, RankToAccess.Su, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Follow });
            fractionData.RanksDefaultAccess.Add(6, new List<RankToAccess>() { RankToAccess.OpenWeaponStock, RankToAccess.Su, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Follow });
            fractionData.RanksDefaultAccess.Add(7, new List<RankToAccess>() { RankToAccess.OpenWeaponStock, RankToAccess.Su, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.Dep });
            fractionData.RanksDefaultAccess.Add(8, new List<RankToAccess>() { RankToAccess.OpenWeaponStock, RankToAccess.Su, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.Dep });
            fractionData.RanksDefaultAccess.Add(9, new List<RankToAccess>() { RankToAccess.OpenWeaponStock, RankToAccess.Su, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.Dep, RankToAccess.Invite, RankToAccess.UnInvite });
            fractionData.RanksDefaultAccess.Add(10, new List<RankToAccess>() { RankToAccess.OpenWeaponStock, RankToAccess.Su, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.Dep, RankToAccess.Invite, RankToAccess.UnInvite });
            fractionData.RanksDefaultAccess.Add(11, new List<RankToAccess>() { RankToAccess.OpenWeaponStock, RankToAccess.Su, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.Dep, RankToAccess.Invite, RankToAccess.UnInvite });
            fractionData.RanksDefaultAccess.Add(12, new List<RankToAccess>() { RankToAccess.OpenWeaponStock, RankToAccess.Su, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.Dep, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.Gov });
            fractionData.RanksDefaultAccess.Add(13, new List<RankToAccess>() { RankToAccess.OpenWeaponStock, RankToAccess.Su, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.Dep, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.Gov, RankToAccess.SetRank });
            fractionData.RanksDefaultAccess.Add(14, new List<RankToAccess>() { RankToAccess.OpenWeaponStock, RankToAccess.Su, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.Dep, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.Gov, RankToAccess.SetRank });
            fractionData.RanksDefaultAccess.Add(15, new List<RankToAccess>() { RankToAccess.OpenWeaponStock, RankToAccess.Su, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.Dep, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.Gov, RankToAccess.SetRank });
            fractionData.RanksDefaultAccess.Add(16, new List<RankToAccess>() { RankToAccess.OpenWeaponStock, RankToAccess.Su, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.Dep, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.Gov, RankToAccess.SetRank });
            fractionData.RanksDefaultAccess.Add(17, new List<RankToAccess>() { RankToAccess.OpenWeaponStock, RankToAccess.Su, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.Dep, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.Gov, RankToAccess.SetRank });
            fractionData.RanksDefaultAccess.Add(18, new List<RankToAccess>() { RankToAccess.OpenWeaponStock, RankToAccess.Su, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.Dep, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.Gov, RankToAccess.SetRank });
            fractionData.RanksDefaultAccess.Add(19, new List<RankToAccess>() { RankToAccess.OpenWeaponStock, RankToAccess.Su, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.Dep, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.Gov, RankToAccess.SetRank });
            fractionData.RanksDefaultAccess.Add(20, new List<RankToAccess>() { RankToAccess.OpenWeaponStock, RankToAccess.Su, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.Dep, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.Gov, RankToAccess.SetRank, RankToAccess.OpenStock });
            
            fractionData = Manager.GetFractionData((int)Models.Fractions.POLICE);
            fractionData.DefaultAccess = new List<RankToAccess>()
            {
                RankToAccess.TakeHelLic, RankToAccess.TakePlaneLic, RankToAccess.OpenStock, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.Arrest, RankToAccess.Rfp, RankToAccess.Warg, RankToAccess.TakeGuns, RankToAccess.Gov, RankToAccess.Dep, RankToAccess.TakeGunLic, RankToAccess.GiveGunLic, RankToAccess.OpenWeaponStock, RankToAccess.Ticket, RankToAccess.SetVehicleRank, RankToAccess.DoorControl, RankToAccess.TableWall, RankToAccess.EditAllTabletWall, RankToAccess.Logs, RankToAccess.Reprimand, RankToAccess.Dron, RankToAccess.VehicleTicket,
                RankToAccess.CreateDepartment, RankToAccess.DeleteDepartment, RankToAccess.ClothesEdit, RankToAccess.OpenGunStock
            };
            
            fractionData.RanksDefaultAccess.Add(1, new List<RankToAccess>() { RankToAccess.Arrest });
            fractionData.RanksDefaultAccess.Add(2, new List<RankToAccess>() { RankToAccess.Arrest });
            fractionData.RanksDefaultAccess.Add(3, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket });
            fractionData.RanksDefaultAccess.Add(4, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(5, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns });
            fractionData.RanksDefaultAccess.Add(6, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns });
            fractionData.RanksDefaultAccess.Add(7, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns });
            fractionData.RanksDefaultAccess.Add(8, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns });
            fractionData.RanksDefaultAccess.Add(9, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns });
            fractionData.RanksDefaultAccess.Add(10, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Rfp });
            fractionData.RanksDefaultAccess.Add(11, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Rfp, RankToAccess.TakeHelLic, RankToAccess.TakePlaneLic, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.Warg, RankToAccess.Dep, RankToAccess.GiveGunLic });
            fractionData.RanksDefaultAccess.Add(12, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Rfp, RankToAccess.TakeHelLic, RankToAccess.TakePlaneLic, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.Warg, RankToAccess.Dep, RankToAccess.GiveGunLic });
            fractionData.RanksDefaultAccess.Add(13, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Rfp, RankToAccess.TakeHelLic, RankToAccess.TakePlaneLic, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.Warg, RankToAccess.Dep, RankToAccess.GiveGunLic, RankToAccess.Gov });
            fractionData.RanksDefaultAccess.Add(14, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Rfp, RankToAccess.TakeHelLic, RankToAccess.TakePlaneLic, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.Warg, RankToAccess.Dep, RankToAccess.GiveGunLic, RankToAccess.Gov, RankToAccess.OpenStock });
            fractionData.RanksDefaultAccess.Add(15, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Rfp, RankToAccess.TakeHelLic, RankToAccess.TakePlaneLic, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.Warg, RankToAccess.Dep, RankToAccess.GiveGunLic, RankToAccess.Gov, RankToAccess.OpenStock, RankToAccess.OpenGunStock });

            fractionData = Manager.GetFractionData((int)Models.Fractions.EMS);
            fractionData.DefaultAccess = new List<RankToAccess>() { RankToAccess.OpenStock, RankToAccess.GiveMedLic, RankToAccess.GivePmLic, RankToAccess.Ems, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.Heal, RankToAccess.Medkit, RankToAccess.Gov, RankToAccess.Dep, RankToAccess.SetVehicleRank, RankToAccess.DoorControl, RankToAccess.TableWall, RankToAccess.EditAllTabletWall, RankToAccess.Logs, RankToAccess.Reprimand, RankToAccess.Dron, RankToAccess.CreateDepartment, RankToAccess.DeleteDepartment, RankToAccess.ClothesEdit, };

            fractionData.RanksDefaultAccess.Add(1, new List<RankToAccess>() { RankToAccess.Heal });
            fractionData.RanksDefaultAccess.Add(2, new List<RankToAccess>() { RankToAccess.Heal });
            fractionData.RanksDefaultAccess.Add(3, new List<RankToAccess>() { RankToAccess.Heal, RankToAccess.Ems });
            fractionData.RanksDefaultAccess.Add(4, new List<RankToAccess>() { RankToAccess.Heal, RankToAccess.Ems, RankToAccess.Medkit });
            fractionData.RanksDefaultAccess.Add(5, new List<RankToAccess>() { RankToAccess.Heal, RankToAccess.Ems, RankToAccess.Medkit, RankToAccess.GiveMedLic });
            fractionData.RanksDefaultAccess.Add(6, new List<RankToAccess>() { RankToAccess.Heal, RankToAccess.Ems, RankToAccess.Medkit, RankToAccess.GiveMedLic });
            fractionData.RanksDefaultAccess.Add(7, new List<RankToAccess>() { RankToAccess.Heal, RankToAccess.Ems, RankToAccess.Medkit, RankToAccess.GiveMedLic });
            fractionData.RanksDefaultAccess.Add(8, new List<RankToAccess>() { RankToAccess.Heal, RankToAccess.Ems, RankToAccess.Medkit, RankToAccess.GiveMedLic });
            fractionData.RanksDefaultAccess.Add(9, new List<RankToAccess>() { RankToAccess.Heal, RankToAccess.Ems, RankToAccess.Medkit, RankToAccess.GiveMedLic, RankToAccess.Dep });
            fractionData.RanksDefaultAccess.Add(10, new List<RankToAccess>() { RankToAccess.Heal, RankToAccess.Ems, RankToAccess.Medkit, RankToAccess.GiveMedLic, RankToAccess.Dep });
            fractionData.RanksDefaultAccess.Add(11, new List<RankToAccess>() { RankToAccess.Heal, RankToAccess.Ems, RankToAccess.Medkit, RankToAccess.GiveMedLic, RankToAccess.Dep, RankToAccess.Invite });
            fractionData.RanksDefaultAccess.Add(12, new List<RankToAccess>() { RankToAccess.Heal, RankToAccess.Ems, RankToAccess.Medkit, RankToAccess.GiveMedLic, RankToAccess.Dep, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.Gov });
            fractionData.RanksDefaultAccess.Add(13, new List<RankToAccess>() { RankToAccess.Heal, RankToAccess.Ems, RankToAccess.Medkit, RankToAccess.GiveMedLic, RankToAccess.Dep, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.Gov, RankToAccess.OpenStock, RankToAccess.GivePmLic });
            fractionData.RanksDefaultAccess.Add(14, new List<RankToAccess>() { RankToAccess.Heal, RankToAccess.Ems, RankToAccess.Medkit, RankToAccess.GiveMedLic, RankToAccess.Dep, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.Gov, RankToAccess.OpenStock, RankToAccess.GivePmLic });
            fractionData.RanksDefaultAccess.Add(15, new List<RankToAccess>() { RankToAccess.Heal, RankToAccess.Ems, RankToAccess.Medkit, RankToAccess.GiveMedLic, RankToAccess.Dep, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.Gov, RankToAccess.OpenStock, RankToAccess.GivePmLic });
            
            fractionData = Manager.GetFractionData((int)Models.Fractions.FIB);
            fractionData.DefaultAccess = new List<RankToAccess>() { RankToAccess.TakeHelLic, RankToAccess.TakePlaneLic, RankToAccess.OpenStock, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.Arrest, RankToAccess.Rfp, RankToAccess.Warg, RankToAccess.TakeGuns, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Gov, RankToAccess.Dep, RankToAccess.TakeGunLic, RankToAccess.GiveGunLic, RankToAccess.OpenWeaponStock, RankToAccess.SetVehicleRank, RankToAccess.DoorControl, RankToAccess.TableWall, RankToAccess.EditAllTabletWall, RankToAccess.Logs, RankToAccess.Reprimand, RankToAccess.Dron, RankToAccess.CreateDepartment, RankToAccess.DeleteDepartment, RankToAccess.ClothesEdit, RankToAccess.OpenGunStock };

            fractionData.RanksDefaultAccess.Add(1, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.TakeGuns });
            fractionData.RanksDefaultAccess.Add(2, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.TakeGuns, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.TakeGunLic });
            fractionData.RanksDefaultAccess.Add(3, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.TakeGuns, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.TakeGunLic, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(4, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.TakeGuns, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.TakeGunLic, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(5, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.TakeGuns, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.TakeGunLic, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(6, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.TakeGuns, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.TakeGunLic, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(7, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.TakeGuns, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.TakeGunLic, RankToAccess.OpenWeaponStock, RankToAccess.Dep });
            fractionData.RanksDefaultAccess.Add(8, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.TakeGuns, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.TakeGunLic, RankToAccess.OpenWeaponStock, RankToAccess.Dep, RankToAccess.Rfp, RankToAccess.GiveGunLic });
            fractionData.RanksDefaultAccess.Add(9, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.TakeGuns, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.TakeGunLic, RankToAccess.OpenWeaponStock, RankToAccess.Dep, RankToAccess.Rfp, RankToAccess.GiveGunLic });
            fractionData.RanksDefaultAccess.Add(10, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.TakeGuns, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.TakeGunLic, RankToAccess.OpenWeaponStock, RankToAccess.Dep, RankToAccess.Rfp, RankToAccess.GiveGunLic, RankToAccess.TakeHelLic, RankToAccess.TakePlaneLic });
            fractionData.RanksDefaultAccess.Add(11, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.TakeGuns, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.TakeGunLic, RankToAccess.OpenWeaponStock, RankToAccess.Dep, RankToAccess.Rfp, RankToAccess.GiveGunLic, RankToAccess.TakeHelLic, RankToAccess.TakePlaneLic, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.Warg });
            fractionData.RanksDefaultAccess.Add(12, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.TakeGuns, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.TakeGunLic, RankToAccess.OpenWeaponStock, RankToAccess.Dep, RankToAccess.Rfp, RankToAccess.GiveGunLic, RankToAccess.TakeHelLic, RankToAccess.TakePlaneLic, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.Warg, RankToAccess.SetRank });
            fractionData.RanksDefaultAccess.Add(13, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.TakeGuns, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.TakeGunLic, RankToAccess.OpenWeaponStock, RankToAccess.Dep, RankToAccess.Rfp, RankToAccess.GiveGunLic, RankToAccess.TakeHelLic, RankToAccess.TakePlaneLic, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.Warg, RankToAccess.SetRank, RankToAccess.OpenStock });
            fractionData.RanksDefaultAccess.Add(14, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.TakeGuns, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.TakeGunLic, RankToAccess.OpenWeaponStock, RankToAccess.Dep, RankToAccess.Rfp, RankToAccess.GiveGunLic, RankToAccess.TakeHelLic, RankToAccess.TakePlaneLic, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.Warg, RankToAccess.SetRank, RankToAccess.OpenStock, RankToAccess.Gov });
            fractionData.RanksDefaultAccess.Add(15, new List<RankToAccess>() { RankToAccess.Follow, RankToAccess.TakeGuns, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.TakeGunLic, RankToAccess.OpenWeaponStock, RankToAccess.Dep, RankToAccess.Rfp, RankToAccess.GiveGunLic, RankToAccess.TakeHelLic, RankToAccess.TakePlaneLic, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.Warg, RankToAccess.SetRank, RankToAccess.OpenStock, RankToAccess.Gov });
            
            fractionData = Manager.GetFractionData((int)Models.Fractions.LCN);
            fractionData.DefaultAccess = new List<RankToAccess>() { RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.OpenStock, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.BizWar, RankToAccess.BizwarJoin, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMoney, RankToAccess.TakeMedkits, RankToAccess.Pocket, RankToAccess.OpenWeaponStock, RankToAccess.SetVehicleRank, RankToAccess.DoorControl, RankToAccess.TableWall, RankToAccess.EditAllTabletWall, RankToAccess.Logs, RankToAccess.Reprimand, RankToAccess.CreateDepartment, RankToAccess.DeleteDepartment, };

            fractionData.RanksDefaultAccess.Add(1, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket });
            fractionData.RanksDefaultAccess.Add(2, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull });
            fractionData.RanksDefaultAccess.Add(3, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull });
            fractionData.RanksDefaultAccess.Add(4, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns });
            fractionData.RanksDefaultAccess.Add(5, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns });
            fractionData.RanksDefaultAccess.Add(6, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Invite });
            fractionData.RanksDefaultAccess.Add(7, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Invite, RankToAccess.TakeDrugs });
            fractionData.RanksDefaultAccess.Add(8, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Invite, RankToAccess.TakeDrugs, RankToAccess.OpenStock, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.BizWar, RankToAccess.TakeStock, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(9, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Invite, RankToAccess.TakeDrugs, RankToAccess.OpenStock, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.BizWar, RankToAccess.TakeStock, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.TakeMats });
            fractionData.RanksDefaultAccess.Add(10, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Invite, RankToAccess.TakeDrugs, RankToAccess.OpenStock, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.BizWar, RankToAccess.TakeStock, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.TakeMats, RankToAccess.TakeMoney });
            
            fractionData = Manager.GetFractionData((int)Models.Fractions.RUSSIAN);
            fractionData.DefaultAccess = new List<RankToAccess>() { RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.OpenStock, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.BizWar, RankToAccess.BizwarJoin, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMoney, RankToAccess.TakeMedkits, RankToAccess.Pocket, RankToAccess.OpenWeaponStock, RankToAccess.SetVehicleRank, RankToAccess.DoorControl, RankToAccess.TableWall, RankToAccess.EditAllTabletWall, RankToAccess.Logs, RankToAccess.Reprimand, RankToAccess.CreateDepartment, RankToAccess.DeleteDepartment, };

            fractionData.RanksDefaultAccess.Add(1, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket });
            fractionData.RanksDefaultAccess.Add(2, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull });
            fractionData.RanksDefaultAccess.Add(3, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull });
            fractionData.RanksDefaultAccess.Add(4, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns });
            fractionData.RanksDefaultAccess.Add(5, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns });
            fractionData.RanksDefaultAccess.Add(6, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Invite });
            fractionData.RanksDefaultAccess.Add(7, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Invite, RankToAccess.TakeDrugs });
            fractionData.RanksDefaultAccess.Add(8, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Invite, RankToAccess.TakeDrugs, RankToAccess.OpenStock, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.BizWar, RankToAccess.TakeStock, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(9, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Invite, RankToAccess.TakeDrugs, RankToAccess.OpenStock, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.BizWar, RankToAccess.TakeStock, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.TakeMats });
            fractionData.RanksDefaultAccess.Add(10, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Invite, RankToAccess.TakeDrugs, RankToAccess.OpenStock, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.BizWar, RankToAccess.TakeStock, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.TakeMats, RankToAccess.TakeMoney });
            
            fractionData = Manager.GetFractionData((int)Models.Fractions.YAKUZA);
            fractionData.DefaultAccess = new List<RankToAccess>() { RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.OpenStock, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.BizWar, RankToAccess.BizwarJoin, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMoney, RankToAccess.TakeMedkits, RankToAccess.Pocket, RankToAccess.OpenWeaponStock, RankToAccess.SetVehicleRank, RankToAccess.DoorControl, RankToAccess.TableWall, RankToAccess.EditAllTabletWall, RankToAccess.Logs, RankToAccess.Reprimand, RankToAccess.CreateDepartment, RankToAccess.DeleteDepartment, };

            fractionData.RanksDefaultAccess.Add(1, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket });
            fractionData.RanksDefaultAccess.Add(2, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull });
            fractionData.RanksDefaultAccess.Add(3, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull });
            fractionData.RanksDefaultAccess.Add(4, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns });
            fractionData.RanksDefaultAccess.Add(5, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns });
            fractionData.RanksDefaultAccess.Add(6, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Invite });
            fractionData.RanksDefaultAccess.Add(7, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Invite, RankToAccess.TakeDrugs });
            fractionData.RanksDefaultAccess.Add(8, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Invite, RankToAccess.TakeDrugs, RankToAccess.OpenStock, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.BizWar, RankToAccess.TakeStock, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(9, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Invite, RankToAccess.TakeDrugs, RankToAccess.OpenStock, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.BizWar, RankToAccess.TakeStock, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.TakeMats });
            fractionData.RanksDefaultAccess.Add(10, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Invite, RankToAccess.TakeDrugs, RankToAccess.OpenStock, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.BizWar, RankToAccess.TakeStock, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.TakeMats, RankToAccess.TakeMoney });
            
            fractionData = Manager.GetFractionData((int)Models.Fractions.ARMENIAN);
            fractionData.DefaultAccess = new List<RankToAccess>() { RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.OpenStock, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.BizWar, RankToAccess.BizwarJoin, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMoney, RankToAccess.TakeMedkits, RankToAccess.Pocket, RankToAccess.OpenWeaponStock, RankToAccess.SetVehicleRank, RankToAccess.DoorControl, RankToAccess.TableWall, RankToAccess.EditAllTabletWall, RankToAccess.Logs, RankToAccess.Reprimand, RankToAccess.CreateDepartment, RankToAccess.DeleteDepartment, };

            fractionData.RanksDefaultAccess.Add(1, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket });
            fractionData.RanksDefaultAccess.Add(2, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull });
            fractionData.RanksDefaultAccess.Add(3, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull });
            fractionData.RanksDefaultAccess.Add(4, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns });
            fractionData.RanksDefaultAccess.Add(5, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns });
            fractionData.RanksDefaultAccess.Add(6, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Invite });
            fractionData.RanksDefaultAccess.Add(7, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Invite, RankToAccess.TakeDrugs });
            fractionData.RanksDefaultAccess.Add(8, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Invite, RankToAccess.TakeDrugs, RankToAccess.OpenStock, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.BizWar, RankToAccess.TakeStock, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(9, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Invite, RankToAccess.TakeDrugs, RankToAccess.OpenStock, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.BizWar, RankToAccess.TakeStock, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.TakeMats });
            fractionData.RanksDefaultAccess.Add(10, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.BizwarJoin, RankToAccess.Follow, RankToAccess.Pocket, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.StealGuns, RankToAccess.Invite, RankToAccess.TakeDrugs, RankToAccess.OpenStock, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.BizWar, RankToAccess.TakeStock, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.TakeMats, RankToAccess.TakeMoney });
            
            fractionData = Manager.GetFractionData((int)Models.Fractions.ARMY);
            fractionData.DefaultAccess = new List<RankToAccess>() { RankToAccess.TakeGuns, RankToAccess.OpenStock, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.TakeStock, RankToAccess.Gov, RankToAccess.Dep, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.Warg, RankToAccess.OpenWeaponStock, RankToAccess.SetVehicleRank, RankToAccess.DoorControl, RankToAccess.TableWall, RankToAccess.EditAllTabletWall, RankToAccess.Logs, RankToAccess.Reprimand, RankToAccess.Dron, RankToAccess.CreateDepartment, RankToAccess.DeleteDepartment, RankToAccess.ClothesEdit, RankToAccess.OpenGunStock};

            fractionData.RanksDefaultAccess.Add(1, new List<RankToAccess>() { RankToAccess.TakeStock });
            fractionData.RanksDefaultAccess.Add(2, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.InCar, RankToAccess.Pull });
            fractionData.RanksDefaultAccess.Add(3, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(4, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(5, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(6, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(7, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(8, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(9, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(10, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Dep, RankToAccess.Cuff, RankToAccess.Follow });
            fractionData.RanksDefaultAccess.Add(11, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Dep, RankToAccess.Cuff, RankToAccess.Follow });
            fractionData.RanksDefaultAccess.Add(12, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Dep, RankToAccess.Cuff, RankToAccess.Follow });
            fractionData.RanksDefaultAccess.Add(13, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Dep, RankToAccess.Cuff, RankToAccess.Follow });
            fractionData.RanksDefaultAccess.Add(14, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Dep, RankToAccess.Cuff, RankToAccess.Follow });
            fractionData.RanksDefaultAccess.Add(15, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Dep, RankToAccess.Cuff, RankToAccess.Follow });
            fractionData.RanksDefaultAccess.Add(16, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Dep, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.Invite, RankToAccess.Gov });
            fractionData.RanksDefaultAccess.Add(17, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Dep, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.Invite, RankToAccess.Gov, RankToAccess.UnInvite, RankToAccess.SetRank });
            fractionData.RanksDefaultAccess.Add(18, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Dep, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.Invite, RankToAccess.Gov, RankToAccess.UnInvite, RankToAccess.SetRank });
            fractionData.RanksDefaultAccess.Add(19, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Dep, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.Invite, RankToAccess.Gov, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.OpenStock });
            fractionData.RanksDefaultAccess.Add(20, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Dep, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.Invite, RankToAccess.Gov, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.OpenStock, RankToAccess.Warg });
            fractionData.RanksDefaultAccess.Add(21, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Dep, RankToAccess.Cuff, RankToAccess.Follow, RankToAccess.Invite, RankToAccess.Gov, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.OpenStock, RankToAccess.Warg });
            
            fractionData = Manager.GetFractionData((int)Models.Fractions.LSNEWS);
            fractionData.DefaultAccess = new List<RankToAccess>() { RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.Delad, RankToAccess.Gov, RankToAccess.Dep, RankToAccess.DoorControl, RankToAccess.SetVehicleRank, RankToAccess.TableWall, RankToAccess.EditAllTabletWall, RankToAccess.Logs, RankToAccess.Reprimand, RankToAccess.Ads, RankToAccess.Dron, RankToAccess.CreateDepartment, RankToAccess.DeleteDepartment, RankToAccess.ClothesEdit, RankToAccess.StartLiveStream };
            
            fractionData.RanksDefaultAccess.Add(1, new List<RankToAccess>() { RankToAccess.Delad, RankToAccess.Ads });
            fractionData.RanksDefaultAccess.Add(2, new List<RankToAccess>() { RankToAccess.Delad, RankToAccess.Ads });
            fractionData.RanksDefaultAccess.Add(3, new List<RankToAccess>() { RankToAccess.Delad, RankToAccess.Ads });
            fractionData.RanksDefaultAccess.Add(4, new List<RankToAccess>() { RankToAccess.Delad, RankToAccess.Ads });
            fractionData.RanksDefaultAccess.Add(5, new List<RankToAccess>() { RankToAccess.Delad, RankToAccess.Ads });
            fractionData.RanksDefaultAccess.Add(6, new List<RankToAccess>() { RankToAccess.Delad, RankToAccess.Ads });
            fractionData.RanksDefaultAccess.Add(7, new List<RankToAccess>() { RankToAccess.Delad, RankToAccess.Ads });
            fractionData.RanksDefaultAccess.Add(8, new List<RankToAccess>() { RankToAccess.Delad, RankToAccess.Ads });
            fractionData.RanksDefaultAccess.Add(9, new List<RankToAccess>() { RankToAccess.Delad, RankToAccess.Ads });
            fractionData.RanksDefaultAccess.Add(10, new List<RankToAccess>() { RankToAccess.Delad, RankToAccess.Ads });
            fractionData.RanksDefaultAccess.Add(11, new List<RankToAccess>() { RankToAccess.Delad, RankToAccess.Ads });
            fractionData.RanksDefaultAccess.Add(12, new List<RankToAccess>() { RankToAccess.Delad, RankToAccess.Ads, RankToAccess.StartLiveStream });
            fractionData.RanksDefaultAccess.Add(13, new List<RankToAccess>() { RankToAccess.Delad, RankToAccess.Ads, RankToAccess.StartLiveStream });
            fractionData.RanksDefaultAccess.Add(14, new List<RankToAccess>() { RankToAccess.Delad, RankToAccess.Invite, RankToAccess.StartLiveStream, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.Dep, RankToAccess.Ads });
            fractionData.RanksDefaultAccess.Add(15, new List<RankToAccess>() { RankToAccess.Delad, RankToAccess.Invite, RankToAccess.StartLiveStream, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.Dep, RankToAccess.Gov, RankToAccess.Ads });
            fractionData.RanksDefaultAccess.Add(16, new List<RankToAccess>() { RankToAccess.Delad, RankToAccess.Invite, RankToAccess.StartLiveStream, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.Dep, RankToAccess.Gov, RankToAccess.Ads });
            fractionData.RanksDefaultAccess.Add(17, new List<RankToAccess>() { RankToAccess.Delad, RankToAccess.Invite, RankToAccess.StartLiveStream, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.Dep, RankToAccess.Gov, RankToAccess.Ads });
            fractionData.RanksDefaultAccess.Add(18, new List<RankToAccess>() { RankToAccess.Delad, RankToAccess.Invite, RankToAccess.StartLiveStream, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.Dep, RankToAccess.Gov, RankToAccess.Ads });
            
            fractionData = Manager.GetFractionData((int)Models.Fractions.THELOST);
            fractionData.DefaultAccess = new List<RankToAccess>() { RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Pocket, RankToAccess.Follow, RankToAccess.StealGuns, RankToAccess.BuyDrugs, RankToAccess.OpenStock, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMoney, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.SetVehicleRank, RankToAccess.DoorControl, RankToAccess.TableWall, RankToAccess.EditAllTabletWall, RankToAccess.Logs, RankToAccess.Reprimand, RankToAccess.CreateDepartment, RankToAccess.DeleteDepartment, RankToAccess.ClothesEdit, };

            fractionData.RanksDefaultAccess.Add(1, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.Pocket, RankToAccess.Follow, RankToAccess.StealGuns, RankToAccess.BuyDrugs });
            fractionData.RanksDefaultAccess.Add(2, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.Pocket, RankToAccess.Follow, RankToAccess.StealGuns, RankToAccess.BuyDrugs });
            fractionData.RanksDefaultAccess.Add(3, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.Pocket, RankToAccess.Follow, RankToAccess.StealGuns, RankToAccess.BuyDrugs });
            fractionData.RanksDefaultAccess.Add(4, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.Pocket, RankToAccess.Follow, RankToAccess.StealGuns, RankToAccess.BuyDrugs, RankToAccess.InCar, RankToAccess.Pull });
            fractionData.RanksDefaultAccess.Add(5, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.Pocket, RankToAccess.Follow, RankToAccess.StealGuns, RankToAccess.BuyDrugs, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMedkits });
            fractionData.RanksDefaultAccess.Add(6, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.Pocket, RankToAccess.Follow, RankToAccess.StealGuns, RankToAccess.BuyDrugs, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMedkits });
            fractionData.RanksDefaultAccess.Add(7, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.Pocket, RankToAccess.Follow, RankToAccess.StealGuns, RankToAccess.BuyDrugs, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMedkits });
            fractionData.RanksDefaultAccess.Add(8, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.Pocket, RankToAccess.Follow, RankToAccess.StealGuns, RankToAccess.BuyDrugs, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenStock, RankToAccess.OpenWeaponStock, RankToAccess.Invite });
            fractionData.RanksDefaultAccess.Add(9, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.Pocket, RankToAccess.Follow, RankToAccess.StealGuns, RankToAccess.BuyDrugs, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenStock, RankToAccess.OpenWeaponStock, RankToAccess.Invite, RankToAccess.SetRank, RankToAccess.UnInvite });
            fractionData.RanksDefaultAccess.Add(10, new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.Pocket, RankToAccess.Follow, RankToAccess.StealGuns, RankToAccess.BuyDrugs, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMedkits, RankToAccess.OpenStock, RankToAccess.OpenWeaponStock, RankToAccess.Invite, RankToAccess.SetRank, RankToAccess.UnInvite, RankToAccess.TakeMoney });
            
            fractionData = Manager.GetFractionData((int)Models.Fractions.MERRYWEATHER);
            fractionData.DefaultAccess = new List<RankToAccess>() { RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Pocket, RankToAccess.Follow, RankToAccess.OpenStock, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.TakeStock, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.TakeMoney, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.Dep, RankToAccess.SetVehicleRank, RankToAccess.DoorControl, RankToAccess.TableWall, RankToAccess.EditAllTabletWall, RankToAccess.Logs, RankToAccess.Reprimand, RankToAccess.CreateDepartment, RankToAccess.DeleteDepartment, RankToAccess.ClothesEdit, };

            fractionData.RanksDefaultAccess.Add(1, new List<RankToAccess>() { RankToAccess.TakeStock });
            fractionData.RanksDefaultAccess.Add(2, new List<RankToAccess>() { RankToAccess.TakeStock });
            fractionData.RanksDefaultAccess.Add(3, new List<RankToAccess>() { RankToAccess.TakeStock });
            fractionData.RanksDefaultAccess.Add(4, new List<RankToAccess>() { RankToAccess.TakeStock });
            fractionData.RanksDefaultAccess.Add(5, new List<RankToAccess>() { RankToAccess.TakeStock });
            fractionData.RanksDefaultAccess.Add(6, new List<RankToAccess>() { RankToAccess.TakeStock });
            fractionData.RanksDefaultAccess.Add(7, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Pocket, RankToAccess.Follow, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(8, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Pocket, RankToAccess.Follow, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(9, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Pocket, RankToAccess.Follow, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(10, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Pocket, RankToAccess.Follow, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.SetRank, RankToAccess.Invite, RankToAccess.UnInvite });
            fractionData.RanksDefaultAccess.Add(11, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Pocket, RankToAccess.Follow, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.SetRank, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.Dep });
            fractionData.RanksDefaultAccess.Add(12, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Pocket, RankToAccess.Follow, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.SetRank, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.Dep });
            fractionData.RanksDefaultAccess.Add(13, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Pocket, RankToAccess.Follow, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.SetRank, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.Dep, RankToAccess.OpenStock });
            fractionData.RanksDefaultAccess.Add(14, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Pocket, RankToAccess.Follow, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.SetRank, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.Dep, RankToAccess.OpenStock });
            fractionData.RanksDefaultAccess.Add(15, new List<RankToAccess>() { RankToAccess.TakeStock, RankToAccess.Cuff, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Pocket, RankToAccess.Follow, RankToAccess.TakeMedkits, RankToAccess.OpenWeaponStock, RankToAccess.SetRank, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.TakeMats, RankToAccess.TakeDrugs, RankToAccess.Dep, RankToAccess.OpenStock, RankToAccess.TakeMoney });

            fractionData = Manager.GetFractionData((int)Models.Fractions.SHERIFF);
            fractionData.DefaultAccess = new List<RankToAccess>()
            {
                RankToAccess.TakeHelLic, RankToAccess.TakePlaneLic, RankToAccess.OpenStock, RankToAccess.Invite,
                RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff,
                RankToAccess.Su, RankToAccess.Follow, RankToAccess.Arrest, RankToAccess.Rfp, RankToAccess.Warg,
                RankToAccess.TakeGuns, RankToAccess.Gov, RankToAccess.Dep, RankToAccess.TakeGunLic,
                RankToAccess.GiveGunLic, RankToAccess.OpenWeaponStock, RankToAccess.Ticket, RankToAccess.SetVehicleRank,
                RankToAccess.DoorControl, RankToAccess.TableWall, RankToAccess.EditAllTabletWall, RankToAccess.Logs,
                RankToAccess.Reprimand, RankToAccess.Dron, RankToAccess.VehicleTicket, RankToAccess.CreateDepartment, RankToAccess.DeleteDepartment, RankToAccess.ClothesEdit, RankToAccess.OpenGunStock
            };
            
            fractionData.RanksDefaultAccess.Add(1, new List<RankToAccess>() { RankToAccess.Arrest });
            fractionData.RanksDefaultAccess.Add(2, new List<RankToAccess>() { RankToAccess.Arrest });
            fractionData.RanksDefaultAccess.Add(3, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket });
            fractionData.RanksDefaultAccess.Add(4, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock });
            fractionData.RanksDefaultAccess.Add(5, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns });
            fractionData.RanksDefaultAccess.Add(6, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns });
            fractionData.RanksDefaultAccess.Add(7, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns });
            fractionData.RanksDefaultAccess.Add(8, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns });
            fractionData.RanksDefaultAccess.Add(9, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns });
            fractionData.RanksDefaultAccess.Add(10, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Rfp });
            fractionData.RanksDefaultAccess.Add(11, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Rfp, RankToAccess.TakeHelLic, RankToAccess.TakePlaneLic, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.Warg, RankToAccess.Dep, RankToAccess.GiveGunLic });
            fractionData.RanksDefaultAccess.Add(12, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Rfp, RankToAccess.TakeHelLic, RankToAccess.TakePlaneLic, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.Warg, RankToAccess.Dep, RankToAccess.GiveGunLic });
            fractionData.RanksDefaultAccess.Add(13, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Rfp, RankToAccess.TakeHelLic, RankToAccess.TakePlaneLic, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.Warg, RankToAccess.Dep, RankToAccess.GiveGunLic, RankToAccess.Gov });
            fractionData.RanksDefaultAccess.Add(14, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Rfp, RankToAccess.TakeHelLic, RankToAccess.TakePlaneLic, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.Warg, RankToAccess.Dep, RankToAccess.GiveGunLic, RankToAccess.Gov, RankToAccess.OpenStock });
            fractionData.RanksDefaultAccess.Add(15, new List<RankToAccess>() { RankToAccess.Arrest, RankToAccess.InCar, RankToAccess.Pull, RankToAccess.Cuff, RankToAccess.Su, RankToAccess.Follow, RankToAccess.TakeGunLic, RankToAccess.Ticket, RankToAccess.OpenWeaponStock, RankToAccess.TakeGuns, RankToAccess.Rfp, RankToAccess.TakeHelLic, RankToAccess.TakePlaneLic, RankToAccess.Invite, RankToAccess.UnInvite, RankToAccess.SetRank, RankToAccess.Warg, RankToAccess.Dep, RankToAccess.GiveGunLic, RankToAccess.Gov, RankToAccess.OpenStock });
        }

        private static void AddFractionRanksData(int fractionId, int rank, string name, int salary, int maxScore, List<RankToAccess> access)
        {
            var fractionData = Manager.GetFractionData(fractionId);
            if (!fractionData.Ranks.ContainsKey(rank))
            {
                var rankData = new RankData
                {
                    Name = name,
                    Salary = salary,
                    MaxScore = maxScore,
                    Access = access,
                };
                
                fractionData.Ranks.Add(rank, rankData);
            }

            if (fractionData.Ranks[rank].MaxScore == 0)//Временно
                fractionData.Ranks[rank].MaxScore = maxScore;

            if (!fractionData.DefaultRanks.ContainsKey(rank))
            {
                var rankData = new RankData
                {
                    Name = name,
                    Salary = salary,
                    MaxScore = maxScore,
                    Access = access,
                };
                
                fractionData.DefaultRanks.Add(rank, rankData);
            }
        }
        public static void InitFractionDefaultRanksName()
        {
            List<int> fractions = new List<int>();
            int fractionId = (int)Models.Fractions.FAMILY;

            var fractionData = Manager.GetFractionData(fractionId);
            if (fractionData.Ranks.Count == 0) fractions.Add(fractionId);
            AddFractionRanksData(fractionId, 1, "Beginner", 0, 1, fractionData.RanksDefaultAccess[1]);
            AddFractionRanksData(fractionId, 2, "Rapper", 0, 2, fractionData.RanksDefaultAccess[2]);
            AddFractionRanksData(fractionId, 3, "Regular", 0, 3, fractionData.RanksDefaultAccess[3]);
            AddFractionRanksData(fractionId, 4, "Gangstar", 0, 4, fractionData.RanksDefaultAccess[4]);
            AddFractionRanksData(fractionId, 5, "Drug Dealer", 0, 5, fractionData.RanksDefaultAccess[5]);
            AddFractionRanksData(fractionId, 6, "Legend", 0, 6, fractionData.RanksDefaultAccess[6]);
            AddFractionRanksData(fractionId, 7, "O.G.", 0, 7, fractionData.RanksDefaultAccess[7]);
            AddFractionRanksData(fractionId, 8, "Lieutenant/OG", 0, 8, fractionData.RanksDefaultAccess[8]);
            AddFractionRanksData(fractionId, 9, "Bro", 0, 9, fractionData.RanksDefaultAccess[9]);
            AddFractionRanksData(fractionId, 10, "Big Bro", 0, 10, fractionData.RanksDefaultAccess[10]);

            fractionId = (int)Models.Fractions.BALLAS;
            fractionData = Manager.GetFractionData(fractionId);
            
            if (fractionData.Ranks.Count == 0) fractions.Add(fractionId);
            AddFractionRanksData(fractionId, 1, "Beginner", 0, 1, fractionData.RanksDefaultAccess[1]);
            AddFractionRanksData(fractionId, 2, "Rapper", 0, 2, fractionData.RanksDefaultAccess[2]);
            AddFractionRanksData(fractionId, 3, "Regular", 0, 3, fractionData.RanksDefaultAccess[3]);
            AddFractionRanksData(fractionId, 4, "Gangstar", 0, 4, fractionData.RanksDefaultAccess[4]);
            AddFractionRanksData(fractionId, 5, "Drug Dealer", 0, 5, fractionData.RanksDefaultAccess[5]);
            AddFractionRanksData(fractionId, 6, "Legend", 0, 6, fractionData.RanksDefaultAccess[6]);
            AddFractionRanksData(fractionId, 7, "O.G.", 0, 7, fractionData.RanksDefaultAccess[7]);
            AddFractionRanksData(fractionId, 8, "Lieutenant/OG", 0, 8, fractionData.RanksDefaultAccess[8]);
            AddFractionRanksData(fractionId, 9, "Bro", 0, 9, fractionData.RanksDefaultAccess[9]);
            AddFractionRanksData(fractionId, 10, "Big Bro", 0, 10, fractionData.RanksDefaultAccess[10]);

            fractionId = (int)Models.Fractions.VAGOS;
            fractionData = Manager.GetFractionData(fractionId);

            if (fractionData.Ranks.Count == 0) fractions.Add(fractionId);
            AddFractionRanksData(fractionId, 1, "Beginner", 0, 1, fractionData.RanksDefaultAccess[1]);
            AddFractionRanksData(fractionId, 2, "Rapper", 0, 2, fractionData.RanksDefaultAccess[2]);
            AddFractionRanksData(fractionId, 3, "Regular", 0, 3, fractionData.RanksDefaultAccess[3]);
            AddFractionRanksData(fractionId, 4, "Gangstar", 0, 4, fractionData.RanksDefaultAccess[4]);
            AddFractionRanksData(fractionId, 5, "Drug Dealer", 0, 5, fractionData.RanksDefaultAccess[5]);
            AddFractionRanksData(fractionId, 6, "Legend", 0, 6, fractionData.RanksDefaultAccess[6]);
            AddFractionRanksData(fractionId, 7, "O.G.", 0, 7, fractionData.RanksDefaultAccess[7]);
            AddFractionRanksData(fractionId, 8, "Lieutenant/OG", 0, 8, fractionData.RanksDefaultAccess[8]);
            AddFractionRanksData(fractionId, 9, "Daddy", 0, 9, fractionData.RanksDefaultAccess[9]);
            AddFractionRanksData(fractionId, 10, "Big Daddy", 0, 10, fractionData.RanksDefaultAccess[10]);

            fractionId = (int)Models.Fractions.MARABUNTA;
            fractionData = Manager.GetFractionData(fractionId);

            if (fractionData.Ranks.Count == 0) fractions.Add(fractionId);
            AddFractionRanksData(fractionId, 1, "Beginner", 0, 1, fractionData.RanksDefaultAccess[1]);
            AddFractionRanksData(fractionId, 2, "Rapper", 0, 2, fractionData.RanksDefaultAccess[2]);
            AddFractionRanksData(fractionId, 3, "Regular", 0, 3, fractionData.RanksDefaultAccess[3]);
            AddFractionRanksData(fractionId, 4, "Gangstar", 0, 4, fractionData.RanksDefaultAccess[4]);
            AddFractionRanksData(fractionId, 5, "Drug Dealer", 0, 5, fractionData.RanksDefaultAccess[5]);
            AddFractionRanksData(fractionId, 6, "Legend", 0, 6, fractionData.RanksDefaultAccess[6]);
            AddFractionRanksData(fractionId, 7, "O.G.", 0, 7, fractionData.RanksDefaultAccess[7]);
            AddFractionRanksData(fractionId, 8, "Lieutenant/OG", 0, 8, fractionData.RanksDefaultAccess[8]);
            AddFractionRanksData(fractionId, 9, "Daddy", 0, 9, fractionData.RanksDefaultAccess[9]);
            AddFractionRanksData(fractionId, 10, "Big Daddy", 0, 10, fractionData.RanksDefaultAccess[10]);

            fractionId = (int)Models.Fractions.BLOOD;
            fractionData = Manager.GetFractionData(fractionId);

            if (fractionData.Ranks.Count == 0) fractions.Add(fractionId);
            AddFractionRanksData(fractionId, 1, "Beginner", 0, 1, fractionData.RanksDefaultAccess[1]);
            AddFractionRanksData(fractionId, 2, "Rapper", 0, 2, fractionData.RanksDefaultAccess[2]);
            AddFractionRanksData(fractionId, 3, "Regular", 0, 3, fractionData.RanksDefaultAccess[3]);
            AddFractionRanksData(fractionId, 4, "Gangstar", 0, 4, fractionData.RanksDefaultAccess[4]);
            AddFractionRanksData(fractionId, 5, "Drug Dealer", 0, 5, fractionData.RanksDefaultAccess[5]);
            AddFractionRanksData(fractionId, 6, "Legend", 0, 6, fractionData.RanksDefaultAccess[6]);
            AddFractionRanksData(fractionId, 7, "O.G.", 0, 7, fractionData.RanksDefaultAccess[7]);
            AddFractionRanksData(fractionId, 8, "Lieutenant/OG", 0, 8, fractionData.RanksDefaultAccess[8]);
            AddFractionRanksData(fractionId, 9, "Bro", 0, 9, fractionData.RanksDefaultAccess[9]);
            AddFractionRanksData(fractionId, 10, "Big Bro", 0, 10, fractionData.RanksDefaultAccess[10]);

            fractionId = (int)Models.Fractions.CITY;
            fractionData = Manager.GetFractionData(fractionId);

            if (fractionData.Ranks.Count == 0) fractions.Add(fractionId);
            AddFractionRanksData(fractionId, 1, "Trainee", 200, 1, fractionData.RanksDefaultAccess[1]);
            AddFractionRanksData(fractionId, 2, "Jurist", 245, 2, fractionData.RanksDefaultAccess[2]);
            AddFractionRanksData(fractionId, 3, "Officer USSS", 290, 3, fractionData.RanksDefaultAccess[3]);
            AddFractionRanksData(fractionId, 4, "Senior Officer USSS", 335, 4, fractionData.RanksDefaultAccess[4]);
            AddFractionRanksData(fractionId, 5, "Agent USSS", 380, 5, fractionData.RanksDefaultAccess[5]);
            AddFractionRanksData(fractionId, 6, "Senior Specialist", 425, 6, fractionData.RanksDefaultAccess[6]);
            AddFractionRanksData(fractionId, 7, "Deputy of Department Manager", 470, 7, fractionData.RanksDefaultAccess[7]);
            AddFractionRanksData(fractionId, 8, "Department Manager", 515, 8, fractionData.RanksDefaultAccess[8]);
            AddFractionRanksData(fractionId, 9, "Head of Administration", 560, 9, fractionData.RanksDefaultAccess[9]);
            AddFractionRanksData(fractionId, 10, "Deputy Mayor", 605, 10, fractionData.RanksDefaultAccess[10]);
            AddFractionRanksData(fractionId, 11, "Mayor", 650, 11, fractionData.RanksDefaultAccess[11]);
            AddFractionRanksData(fractionId, 12, "Deputy Minister", 695, 12, fractionData.RanksDefaultAccess[12]);
            AddFractionRanksData(fractionId, 13, "Minister", 740, 13, fractionData.RanksDefaultAccess[13]);
            AddFractionRanksData(fractionId, 14, "Vice Governor", 785, 14, fractionData.RanksDefaultAccess[14]);
            AddFractionRanksData(fractionId, 15, "Governor", 830, 15, fractionData.RanksDefaultAccess[15]);

            fractionId = (int)Models.Fractions.POLICE;
            fractionData = Manager.GetFractionData(fractionId);

            if (fractionData.Ranks.Count == 0) fractions.Add(fractionId);
            AddFractionRanksData(fractionId, 1, "Trainee", 200, 1, fractionData.RanksDefaultAccess[1]);
            AddFractionRanksData(fractionId, 2, "Cadet", 270, 2, fractionData.RanksDefaultAccess[2]);
            AddFractionRanksData(fractionId, 3, "Officer I", 340, 3, fractionData.RanksDefaultAccess[3]);
            AddFractionRanksData(fractionId, 4, "Officer II", 410, 4, fractionData.RanksDefaultAccess[4]);
            AddFractionRanksData(fractionId, 5, "Officer III", 480, 5, fractionData.RanksDefaultAccess[5]);
            AddFractionRanksData(fractionId, 6, "Senior Lead Officer", 550, 6, fractionData.RanksDefaultAccess[6]);
            AddFractionRanksData(fractionId, 7, "Srg. I/Det.I", 620, 7, fractionData.RanksDefaultAccess[7]);
            AddFractionRanksData(fractionId, 8, "Srg. II/Det.II", 690, 8, fractionData.RanksDefaultAccess[8]);
            AddFractionRanksData(fractionId, 9, "Srg. III/Det.III", 760, 9, fractionData.RanksDefaultAccess[9]);
            AddFractionRanksData(fractionId, 10, "Lieutenant", 830, 10, fractionData.RanksDefaultAccess[10]);
            AddFractionRanksData(fractionId, 11, "Captain", 900, 11, fractionData.RanksDefaultAccess[11]);
            AddFractionRanksData(fractionId, 12, "Commander", 970, 12, fractionData.RanksDefaultAccess[12]);
            AddFractionRanksData(fractionId, 13, "Deputy Chief", 1040, 13, fractionData.RanksDefaultAccess[13]);
            AddFractionRanksData(fractionId, 14, "Assistent Chief", 1120, 14, fractionData.RanksDefaultAccess[14]);
            AddFractionRanksData(fractionId, 15, "Chief", 1200, 15, fractionData.RanksDefaultAccess[15]);

            fractionId = (int)Models.Fractions.EMS;
            fractionData = Manager.GetFractionData(fractionId);

            if (fractionData.Ranks.Count == 0) fractions.Add(fractionId);
            AddFractionRanksData(fractionId, 1, "Student", 190, 1, fractionData.RanksDefaultAccess[1]);
            AddFractionRanksData(fractionId, 2, "Trainee", 255, 2, fractionData.RanksDefaultAccess[2]);
            AddFractionRanksData(fractionId, 3, "Intern", 320, 3, fractionData.RanksDefaultAccess[3]);
            AddFractionRanksData(fractionId, 4, "Paramedic", 385, 4, fractionData.RanksDefaultAccess[4]);
            AddFractionRanksData(fractionId, 5, "Intensivist", 450, 5, fractionData.RanksDefaultAccess[5]);
            AddFractionRanksData(fractionId, 6, "Physiologist", 515, 6, fractionData.RanksDefaultAccess[6]);
            AddFractionRanksData(fractionId, 7, "Therapist", 580, 7, fractionData.RanksDefaultAccess[7]);
            AddFractionRanksData(fractionId, 8, "Physician", 645, 8, fractionData.RanksDefaultAccess[8]);
            AddFractionRanksData(fractionId, 9, "Psychologist", 710, 9, fractionData.RanksDefaultAccess[9]);
            AddFractionRanksData(fractionId, 10, "Surgeon", 775, 10, fractionData.RanksDefaultAccess[10]);
            AddFractionRanksData(fractionId, 11, "Specialist", 840, 11, fractionData.RanksDefaultAccess[11]);
            AddFractionRanksData(fractionId, 12, "Deputy Chief Department", 905, 12, fractionData.RanksDefaultAccess[12]);
            AddFractionRanksData(fractionId, 13, "Chief Department", 970, 13, fractionData.RanksDefaultAccess[13]);
            AddFractionRanksData(fractionId, 14, "Deputy Head Physician", 1035, 14, fractionData.RanksDefaultAccess[14]);
            AddFractionRanksData(fractionId, 15, "Head Physician", 1090, 15, fractionData.RanksDefaultAccess[15]);

            fractionId = (int)Models.Fractions.FIB;
            fractionData = Manager.GetFractionData(fractionId);

            if (fractionData.Ranks.Count == 0) fractions.Add(fractionId);
            AddFractionRanksData(fractionId, 1, "Trainee", 200, 1, fractionData.RanksDefaultAccess[1]);
            AddFractionRanksData(fractionId, 2, "Jr. Agent", 270, 2, fractionData.RanksDefaultAccess[2]);
            AddFractionRanksData(fractionId, 3, "Agent", 340, 3, fractionData.RanksDefaultAccess[3]);
            AddFractionRanksData(fractionId, 4, "Senior Agent", 410, 4, fractionData.RanksDefaultAccess[4]);
            AddFractionRanksData(fractionId, 5, "Special Agent", 480, 5, fractionData.RanksDefaultAccess[5]);
            AddFractionRanksData(fractionId, 6, "Secret Agent", 550, 6, fractionData.RanksDefaultAccess[6]);
            AddFractionRanksData(fractionId, 7, "Agent NAT. Securities", 620, 7, fractionData.RanksDefaultAccess[7]);
            AddFractionRanksData(fractionId, 8, "Managing Agency", 690, 8, fractionData.RanksDefaultAccess[8]);
            AddFractionRanksData(fractionId, 9, "FIB Inspector", 760, 9, fractionData.RanksDefaultAccess[9]);
            AddFractionRanksData(fractionId, 10, "Deputy Chief Department", 830, 10, fractionData.RanksDefaultAccess[10]);
            AddFractionRanksData(fractionId, 11, "Chief Department", 900, 11, fractionData.RanksDefaultAccess[11]);
            AddFractionRanksData(fractionId, 12, "Head", 970, 12, fractionData.RanksDefaultAccess[12]);
            AddFractionRanksData(fractionId, 13, "Head of FIB Academy", 1040, 13, fractionData.RanksDefaultAccess[13]);
            AddFractionRanksData(fractionId, 14, "Deputy Director", 1120, 14, fractionData.RanksDefaultAccess[14]);
            AddFractionRanksData(fractionId, 15, "Director", 1200, 15, fractionData.RanksDefaultAccess[15]);

            fractionId = (int)Models.Fractions.LCN;
            fractionData = Manager.GetFractionData(fractionId);

            if (fractionData.Ranks.Count == 0) fractions.Add(fractionId);
            AddFractionRanksData(fractionId, 1, "Novizio", 0, 1, fractionData.RanksDefaultAccess[1]);
            AddFractionRanksData(fractionId, 2, "Testato", 0, 2, fractionData.RanksDefaultAccess[2]);
            AddFractionRanksData(fractionId, 3, "Associato", 0, 3, fractionData.RanksDefaultAccess[3]);
            AddFractionRanksData(fractionId, 4, "Controllato", 0, 4, fractionData.RanksDefaultAccess[4]);
            AddFractionRanksData(fractionId, 5, "Soldato", 0, 5, fractionData.RanksDefaultAccess[5]);
            AddFractionRanksData(fractionId, 6, "Aiutante", 0, 6, fractionData.RanksDefaultAccess[6]);
            AddFractionRanksData(fractionId, 7, "Capo", 0, 7, fractionData.RanksDefaultAccess[7]);
            AddFractionRanksData(fractionId, 8, "Under Boss", 0, 8, fractionData.RanksDefaultAccess[8]);
            AddFractionRanksData(fractionId, 9, "Consigliere", 0, 9, fractionData.RanksDefaultAccess[9]);
            AddFractionRanksData(fractionId, 10, "God Father", 0, 10, fractionData.RanksDefaultAccess[10]);

            fractionId = (int)Models.Fractions.RUSSIAN;
            fractionData = Manager.GetFractionData(fractionId);

            if (fractionData.Ranks.Count == 0) fractions.Add(fractionId);
            AddFractionRanksData(fractionId, 1, "Охранник", 0, 1, fractionData.RanksDefaultAccess[1]);
            AddFractionRanksData(fractionId, 2, "Разнорабочий", 0, 2, fractionData.RanksDefaultAccess[2]);
            AddFractionRanksData(fractionId, 3, "Старший охранник", 0, 3, fractionData.RanksDefaultAccess[3]);
            AddFractionRanksData(fractionId, 4, "Жулик", 0, 4, fractionData.RanksDefaultAccess[4]);
            AddFractionRanksData(fractionId, 5, "Шулер", 0, 5, fractionData.RanksDefaultAccess[5]);
            AddFractionRanksData(fractionId, 6, "Блатной", 0, 6, fractionData.RanksDefaultAccess[6]);
            AddFractionRanksData(fractionId, 7, "Авторитет", 0, 7, fractionData.RanksDefaultAccess[7]);
            AddFractionRanksData(fractionId, 8, "Смотрящий", 0, 8, fractionData.RanksDefaultAccess[8]);
            AddFractionRanksData(fractionId, 9, "Положенец", 0, 9, fractionData.RanksDefaultAccess[9]);
            AddFractionRanksData(fractionId, 10, "Вор в законе", 0, 10, fractionData.RanksDefaultAccess[10]);

            fractionId = (int)Models.Fractions.YAKUZA;
            fractionData = Manager.GetFractionData(fractionId);

            if (fractionData.Ranks.Count == 0) fractions.Add(fractionId);
            AddFractionRanksData(fractionId, 1, "Wakasu", 0, 1, fractionData.RanksDefaultAccess[1]);
            AddFractionRanksData(fractionId, 2, "Ke Dai", 0, 2, fractionData.RanksDefaultAccess[2]);
            AddFractionRanksData(fractionId, 3, "Sata gasira", 0, 3, fractionData.RanksDefaultAccess[3]);
            AddFractionRanksData(fractionId, 4, "Vaka gasira", 0, 4, fractionData.RanksDefaultAccess[4]);
            AddFractionRanksData(fractionId, 5, "Co kubintu", 0, 5, fractionData.RanksDefaultAccess[5]);
            AddFractionRanksData(fractionId, 6, "Kambu", 0, 6, fractionData.RanksDefaultAccess[6]);
            AddFractionRanksData(fractionId, 7, "Oazi", 0, 7, fractionData.RanksDefaultAccess[7]);
            AddFractionRanksData(fractionId, 8, "Saiko Komon", 0, 8, fractionData.RanksDefaultAccess[8]);
            AddFractionRanksData(fractionId, 9, "Oyabun", 0, 9, fractionData.RanksDefaultAccess[9]);
            AddFractionRanksData(fractionId, 10, "Kumite", 0, 10, fractionData.RanksDefaultAccess[10]);

            fractionId = (int)Models.Fractions.ARMENIAN;
            fractionData = Manager.GetFractionData(fractionId);

            if (fractionData.Ranks.Count == 0) fractions.Add(fractionId);
            AddFractionRanksData(fractionId, 1, "Gael", 0, 1, fractionData.RanksDefaultAccess[1]);
            AddFractionRanksData(fractionId, 2, "Lav Tha", 0, 2, fractionData.RanksDefaultAccess[2]);
            AddFractionRanksData(fractionId, 3, "Hardah", 0, 3, fractionData.RanksDefaultAccess[3]);
            AddFractionRanksData(fractionId, 4, "Anchors", 0, 4, fractionData.RanksDefaultAccess[4]);
            AddFractionRanksData(fractionId, 5, "Jepkir", 0, 5, fractionData.RanksDefaultAccess[5]);
            AddFractionRanksData(fractionId, 6, "Ehpair", 0, 6, fractionData.RanksDefaultAccess[6]);
            AddFractionRanksData(fractionId, 7, "Naioh", 0, 7, fractionData.RanksDefaultAccess[7]);
            AddFractionRanksData(fractionId, 8, "Goh", 0, 8, fractionData.RanksDefaultAccess[8]);
            AddFractionRanksData(fractionId, 9, "Kerop", 0, 9, fractionData.RanksDefaultAccess[9]);
            AddFractionRanksData(fractionId, 10, "Kevor", 0, 10, fractionData.RanksDefaultAccess[10]);

            fractionId = (int)Models.Fractions.ARMY;
            fractionData = Manager.GetFractionData(fractionId);

            if (fractionData.Ranks.Count == 0) fractions.Add(fractionId);
            AddFractionRanksData(fractionId, 1, "Recruit", 200, 1, fractionData.RanksDefaultAccess[1]);
            AddFractionRanksData(fractionId, 2, "Corporal", 250, 2, fractionData.RanksDefaultAccess[2]);
            AddFractionRanksData(fractionId, 3, "Sergeant", 300, 3, fractionData.RanksDefaultAccess[3]);
            AddFractionRanksData(fractionId, 4, "Sergeant First Class", 350, 4, fractionData.RanksDefaultAccess[4]);
            AddFractionRanksData(fractionId, 5, "Sergeant Major", 400, 5, fractionData.RanksDefaultAccess[5]);
            AddFractionRanksData(fractionId, 6, "Warrant Officer", 450, 6, fractionData.RanksDefaultAccess[6]);
            AddFractionRanksData(fractionId, 7, "Chief Warrant Officer", 500, 7, fractionData.RanksDefaultAccess[7]);
            AddFractionRanksData(fractionId, 8, "Second Lieutenant", 550, 8, fractionData.RanksDefaultAccess[8]);
            AddFractionRanksData(fractionId, 9, "First Lieutenant", 600, 9, fractionData.RanksDefaultAccess[9]);
            AddFractionRanksData(fractionId, 10, "Captain", 650, 10, fractionData.RanksDefaultAccess[10]);
            AddFractionRanksData(fractionId, 11, "Major", 700, 11, fractionData.RanksDefaultAccess[11]);
            AddFractionRanksData(fractionId, 12, "Lieutenant Colonel", 750, 12, fractionData.RanksDefaultAccess[12]);
            AddFractionRanksData(fractionId, 13, "Colonel", 800, 13, fractionData.RanksDefaultAccess[13]);
            AddFractionRanksData(fractionId, 14, "Brigadier General", 850, 14, fractionData.RanksDefaultAccess[14]);
            AddFractionRanksData(fractionId, 15, "Major General", 900, 15, fractionData.RanksDefaultAccess[15]);

            fractionId = (int)Models.Fractions.LSNEWS;
            fractionData = Manager.GetFractionData(fractionId);

            if (fractionData.Ranks.Count == 0) fractions.Add(fractionId);
            AddFractionRanksData(fractionId, 1, "Trainee", 190, 1, fractionData.RanksDefaultAccess[1]);
            AddFractionRanksData(fractionId, 2, "Journalist", 245, 2, fractionData.RanksDefaultAccess[2]);
            AddFractionRanksData(fractionId, 3, "Specialist", 300, 3, fractionData.RanksDefaultAccess[3]);
            AddFractionRanksData(fractionId, 4, "Mentor", 355, 4, fractionData.RanksDefaultAccess[4]);
            AddFractionRanksData(fractionId, 5, "Photographer", 410, 5, fractionData.RanksDefaultAccess[5]);
            AddFractionRanksData(fractionId, 6, "Rukurter", 465, 6, fractionData.RanksDefaultAccess[6]);
            AddFractionRanksData(fractionId, 7, "Rewriter", 520, 7, fractionData.RanksDefaultAccess[7]);
            AddFractionRanksData(fractionId, 8, "Operator", 575, 8, fractionData.RanksDefaultAccess[8]);
            AddFractionRanksData(fractionId, 9, "Senior Editor", 630, 9, fractionData.RanksDefaultAccess[9]);
            AddFractionRanksData(fractionId, 10, "HR Manager", 685, 10, fractionData.RanksDefaultAccess[10]);
            AddFractionRanksData(fractionId, 11, "Chief Editor", 740, 11, fractionData.RanksDefaultAccess[11]);
            AddFractionRanksData(fractionId, 12, "Cheif Presenter", 795, 12, fractionData.RanksDefaultAccess[12]);
            AddFractionRanksData(fractionId, 13, "Cheif Manager", 850, 13, fractionData.RanksDefaultAccess[13]);
            AddFractionRanksData(fractionId, 14, "Deputy Director", 905, 14, fractionData.RanksDefaultAccess[14]);
            AddFractionRanksData(fractionId, 15, "General Director", 950, 15, fractionData.RanksDefaultAccess[15]);

            fractionId = (int)Models.Fractions.THELOST;
            fractionData = Manager.GetFractionData(fractionId);

            if (fractionData.Ranks.Count == 0) fractions.Add(fractionId);
            AddFractionRanksData(fractionId, 1, "Trainee", 0, 1, fractionData.RanksDefaultAccess[1]);
            AddFractionRanksData(fractionId, 2, "Prospect", 0, 2, fractionData.RanksDefaultAccess[2]);
            AddFractionRanksData(fractionId, 3, "Recruit", 0, 3, fractionData.RanksDefaultAccess[3]);
            AddFractionRanksData(fractionId, 4, "Loner", 0, 4, fractionData.RanksDefaultAccess[4]);
            AddFractionRanksData(fractionId, 5, "Support", 0, 5, fractionData.RanksDefaultAccess[5]);
            AddFractionRanksData(fractionId, 6, "Nomad", 0, 6, fractionData.RanksDefaultAccess[6]);
            AddFractionRanksData(fractionId, 7, "Hang Around", 0, 7, fractionData.RanksDefaultAccess[7]);
            AddFractionRanksData(fractionId, 8, "Road Captain", 0, 8, fractionData.RanksDefaultAccess[8]);
            AddFractionRanksData(fractionId, 9, "Vice-President", 0, 9, fractionData.RanksDefaultAccess[9]);
            AddFractionRanksData(fractionId, 10, "President", 0, 10, fractionData.RanksDefaultAccess[10]);

            fractionId = (int)Models.Fractions.MERRYWEATHER;
            fractionData = Manager.GetFractionData(fractionId);

            if (fractionData.Ranks.Count == 0) fractions.Add(fractionId);
            AddFractionRanksData(fractionId, 1, "Trainee", 210, 1, fractionData.RanksDefaultAccess[1]);
            AddFractionRanksData(fractionId, 2, "Driver", 220, 2, fractionData.RanksDefaultAccess[2]);
            AddFractionRanksData(fractionId, 3, "Operator", 230, 3, fractionData.RanksDefaultAccess[3]);
            AddFractionRanksData(fractionId, 4, "Security guard", 240, 4, fractionData.RanksDefaultAccess[4]);
            AddFractionRanksData(fractionId, 5, "Escort", 250, 5, fractionData.RanksDefaultAccess[5]);
            AddFractionRanksData(fractionId, 6, "Warder", 275, 6, fractionData.RanksDefaultAccess[6]);
            AddFractionRanksData(fractionId, 7, "Overseer", 300, 7, fractionData.RanksDefaultAccess[7]);
            AddFractionRanksData(fractionId, 8, "Sentry", 325, 8, fractionData.RanksDefaultAccess[8]);
            AddFractionRanksData(fractionId, 9, "Senior Security guard", 350, 9, fractionData.RanksDefaultAccess[9]);
            AddFractionRanksData(fractionId, 10, "Superviser", 375, 10, fractionData.RanksDefaultAccess[10]);
            AddFractionRanksData(fractionId, 11, "Intendent", 400, 11, fractionData.RanksDefaultAccess[11]);
            AddFractionRanksData(fractionId, 12, "Deputy Head of Security", 425, 12, fractionData.RanksDefaultAccess[12]);
            AddFractionRanksData(fractionId, 13, "Head of Security", 450, 13, fractionData.RanksDefaultAccess[13]);
            AddFractionRanksData(fractionId, 14, "Deputy Superintendent", 550, 14, fractionData.RanksDefaultAccess[14]);
            AddFractionRanksData(fractionId, 15, "Superintendent", 650, 15, fractionData.RanksDefaultAccess[15]);

            fractionId = (int)Models.Fractions.SHERIFF;
            fractionData = Manager.GetFractionData(fractionId);

            if (fractionData.Ranks.Count == 0) fractions.Add(fractionId);
            AddFractionRanksData(fractionId, 1, "Cadet", 210, 1, fractionData.RanksDefaultAccess[1]);
            AddFractionRanksData(fractionId, 2, "Recruit Officer", 220, 2, fractionData.RanksDefaultAccess[2]);
            AddFractionRanksData(fractionId, 3, "Sheriff Officer", 230, 3, fractionData.RanksDefaultAccess[3]);
            AddFractionRanksData(fractionId, 4, "Detective", 240, 4, fractionData.RanksDefaultAccess[4]);
            AddFractionRanksData(fractionId, 5, "Sergeant", 250, 5, fractionData.RanksDefaultAccess[5]);
            AddFractionRanksData(fractionId, 6, "Lieutenant", 275, 6, fractionData.RanksDefaultAccess[6]);
            AddFractionRanksData(fractionId, 7, "Captain", 300, 7, fractionData.RanksDefaultAccess[7]);
            AddFractionRanksData(fractionId, 8, "Deputy Inspector", 325, 8, fractionData.RanksDefaultAccess[8]);
            AddFractionRanksData(fractionId, 9, "Inspector Captain", 350, 9, fractionData.RanksDefaultAccess[9]);
            AddFractionRanksData(fractionId, 10, "Deputy Chief Captain", 375, 10, fractionData.RanksDefaultAccess[10]);
            AddFractionRanksData(fractionId, 11, "Chief Captain", 400, 11, fractionData.RanksDefaultAccess[11]);
            AddFractionRanksData(fractionId, 12, "Deputy Chief of Staff", 425, 12, fractionData.RanksDefaultAccess[12]);
            AddFractionRanksData(fractionId, 13, "Chief of Staff", 450, 13, fractionData.RanksDefaultAccess[13]);
            AddFractionRanksData(fractionId, 14, "Deputy Sheriff", 550, 14, fractionData.RanksDefaultAccess[14]);
            AddFractionRanksData(fractionId, 15, "Sheriff", 650, 15, fractionData.RanksDefaultAccess[15]);

            InsertFractionRanks(fractions);
        }
        public static async void InsertFractionRanks(List<int> fractions)
        {
            try
            {
                await using var db = new ServerBD("MainDB");//При старте сервера

                foreach (int fractionId in fractions)
                {
                    var fractionData = Manager.GetFractionData(fractionId);
                    if (fractionData == null) continue;

                    foreach(var fractionRankData in fractionData.Ranks)
                    {
                        await db.InsertAsync(new Fractionranks
                        {
                            Fraction = fractionId,
                            Rank = fractionRankData.Key,
                            Payday = fractionRankData.Value.Salary,
                            Name = fractionRankData.Value.Name,
                            Access = JsonConvert.SerializeObject(fractionRankData.Value.Access)
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static void LoadFractionConfigs()
        {
            try
            {
                for (int i = (int)Models.Fractions.FAMILY; i <= FractionCount; i++)
                {
                    if (!Manager.Fractions.ContainsKey(i))
                    {
                        var fractionData = new FractionData();
                        fractionData.Id = i;
                        fractionData.Name = Manager.FractionNames[i];
                        Manager.Fractions.Add(i, fractionData);
                    }

                    FractionDrones.Add(i, new Dictionary<ExtVehicle, string>());
                    FractionWeapons.Add(i, new Dictionary<string, int>());

                    if (!Manager.AllMembers.ContainsKey(i)) 
                        Manager.AllMembers.TryAdd(i, new List<FractionMemberData>());
                }
                InitFractionDefaultAccessRanks();

                //
                
                using MySqlCommand cmdFraction = new MySqlCommand
                {
                    CommandText = "SELECT * FROM fractions"
                };
                using DataTable resultFraction = MySQL.QueryRead(cmdFraction);
                if (resultFraction != null && resultFraction.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in resultFraction.Rows)
                    {
                        int fractionId = Convert.ToInt32(dataRow["id"]);
                    
                        var fractionData = new FractionData();
                        if (Manager.Fractions.ContainsKey(fractionId))
                            fractionData = Manager.Fractions[fractionId];

                        fractionData.Drugs = Convert.ToInt32(dataRow["drugs"]);
                        fractionData.Money = Convert.ToInt32(dataRow["money"]);
                        fractionData.Materials = Convert.ToInt32(dataRow["mats"]);
                        fractionData.MedKits = Convert.ToInt32(dataRow["medkits"]);
                        fractionData.CoalOre = Convert.ToInt32(dataRow["coalore"]);
                        fractionData.IronOre = Convert.ToInt32(dataRow["ironore"]);
                        fractionData.SulfurOre = Convert.ToInt32(dataRow["sulfurore"]);
                        fractionData.PreciousOre = Convert.ToInt32(dataRow["preciousore"]);
                        fractionData.IsOpenStock = Convert.ToBoolean(dataRow["isopen"]);
                        fractionData.IsOpenGunStock = Convert.ToBoolean(dataRow["isopengunstock"]);
                        fractionData.FuelLimit = Convert.ToInt32(dataRow["fuellimit"]);
                        fractionData.FuelLeft = Convert.ToInt32(dataRow["fuelleft"]);
                        fractionData.HaveSpecialStock = Convert.ToInt32(dataRow["id"]) == 14;
                        fractionData.Discord = dataRow["discord"].ToString();
                        fractionData.Departments = JsonConvert.DeserializeObject<Dictionary<int, DepartmentData>>(dataRow["departments"].ToString());
                        fractionData.TasksData = JsonConvert.DeserializeObject<TableTaskPlayerData[]>(dataRow["tasksData"].ToString());

                        var clothingsets = dataRow["clothingsets"].ToString();
                        if (clothingsets != String.Empty && clothingsets.Length > 0)
                        {
                            FractionClothingSets.FractionSets[(Models.Fractions) fractionId] =
                                JsonConvert.DeserializeObject<Dictionary<bool, List<FractionSetData>>>(clothingsets);
                        }
                        
                        WeaponRepository.FractionsLastSerial[fractionId] = Convert.ToInt32(dataRow["lastserial"]);

                        if (fractionId == 16 || fractionId == 17) continue;

                        #region label Creating
                        if (Stocks.matsCoords.ContainsKey(fractionId))
                        {
                            fractionData.StockLabel = (ExtTextLabel) NAPI.TextLabel.CreateTextLabel("~w~Склад пуст", Stocks.matsCoords[fractionId] + new Vector3(0, 0, 1.5), 10f, 0.4f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension);
                            fractionData.MaxMats = Stocks.GetMaxStock(fractionId);
                            fractionData.UpdateLabel();
                        }

                        #endregion
                        if (!Manager.Fractions.ContainsKey(fractionId))
                        {
                            fractionData.Id = fractionId;
                            Manager.Fractions.Add(fractionId, fractionData);
                        }

                        CustomColShape.CreateCylinderColShape(Stocks.stockCoords[fractionId], 1, 2, 0, ColShapeEnums.FractionStock, fractionId, 2); // stock colshape
                        NAPI.Marker.CreateMarker(1, Stocks.stockCoords[fractionId] - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220));
                        NAPI.TextLabel.CreateTextLabel(Main.StringToU16($"~w~Склад ~r~{Manager.GetName(fractionId)}"), new Vector3(Stocks.stockCoords[fractionId].X, Stocks.stockCoords[fractionId].Y, Stocks.stockCoords[fractionId].Z + 0.6), 5F, 0.5F, 0, new Color(255, 255, 255));

                        CustomColShape.CreateCylinderColShape(Stocks.matsCoords[fractionId], 3, 5, 0, ColShapeEnums.FractionStock, fractionId, 1); // garage colshape
                        NAPI.Marker.CreateMarker(1, Stocks.matsCoords[fractionId], new Vector3(), new Vector3(), 3f, new Color(155, 0, 0));
                        
                        var ids = new List<TableTaskId>();
                        var fracId = (Fractions.Models.Fractions) fractionData.Id;
                        if (fracId != Fractions.Models.Fractions.None && NeptuneEvo.Table.Tasks.Repository.FractionTaskIds.ContainsKey(fracId))
                            ids = NeptuneEvo.Table.Tasks.Repository.FractionTaskIds[fracId].ToList();
                            
                        fractionData.TasksData = NeptuneEvo.Table.Tasks.Repository.GetData(JsonConvert.SerializeObject(fractionData.TasksData), ids);
                        fractionData.SaveTasksData();
                    }
                }

                using MySqlCommand cmdSelect = new MySqlCommand()
                {
                    CommandText = "SELECT * FROM `fracranks`"
                };
                using DataTable resultSelect = MySQL.QueryRead(cmdSelect);
                if (resultSelect != null && resultSelect.Rows.Count > 0)
                {
                    foreach (DataRow row in resultSelect.Rows)
                    {
                        var fracId = Convert.ToInt32(row["id"]);
                    
                        if (!Manager.AllMembers.ContainsKey(fracId))
                            Manager.AllMembers.TryAdd(fracId, new List<FractionMemberData>());

                        var memberFractionData = new FractionMemberData
                        {
                            UUID = Convert.ToInt32(row["uuid"]),
                            Name = row["name"].ToString(),
                            Id = fracId,
                            Rank = Convert.ToByte(row["rank"]),
                            Date = Convert.ToDateTime(row["date"]),
                            LastLoginDate = Convert.ToDateTime(row["lastLoginDate"]),
                            Avatar = row["avatar"].ToString(),
                            DepartmentId = Convert.ToInt32(row["departmentId"]),
                            DepartmentRank = Convert.ToInt32(row["departmentRank"]),
                            Score = Convert.ToInt32(row["score"]),
                            Access = JsonConvert.DeserializeObject<List<RankToAccess>>(row["access"].ToString()),
                            Lock = JsonConvert.DeserializeObject<List<RankToAccess>>(row["lock"].ToString()),
                        };
                    
                        Manager.AllMembers[fracId].Add(memberFractionData);
                    }
                }

                //
                
                // loading fraction vehicle configs and spawn
                using MySqlCommand cmdFractionVehicles = new MySqlCommand()
                {
                    CommandText = "SELECT * FROM `fractionvehicles`"
                };
                using DataTable resultFractionVehicles = MySQL.QueryRead(cmdFractionVehicles);
                if (resultFractionVehicles != null && resultFractionVehicles.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in resultFractionVehicles.Rows)
                    {
                        var fraction = Convert.ToInt32(dataRow["fraction"]);
                        var fractionData = Manager.GetFractionData(fraction);
                        if (fractionData == null) continue;
                        var number = dataRow["number"].ToString();
                        var model = dataRow["model"].ToString().ToLower();
                        var position = JsonConvert.DeserializeObject<Vector3>(dataRow["position"].ToString());
                        var rotation = JsonConvert.DeserializeObject<Vector3>(dataRow["rotation"].ToString());
                        var minrank = Convert.ToInt32(dataRow["rank"]);
                        var defaultrank = Convert.ToInt32(dataRow["defaultrank"]);
                        var color1 = Convert.ToInt32(dataRow["colorprim"]);
                        var color2 = Convert.ToInt32(dataRow["colorsec"]);
                        var components =
                            JsonConvert.DeserializeObject<VehicleCustomization>(dataRow["components"].ToString());
                        var isDimension = Convert.ToBoolean(dataRow["isDimension"]);
                        components.PrimModColor = color1;
                        components.SecModColor = color2;
                        fractionData.Vehicles.Add(number,
                            new FractionVehicleData(model, position, rotation, minrank, defaultrank, color1, color2,
                                components));

                        if (isDimension)
                            fractionData.Vehicles[number].Dimension = (uint) ((1750) + fraction);

                        if (defaultrank == -1)
                        {
                            fractionData.Vehicles[number].defaultRank = minrank;
                            using MySqlCommand ucmd = new MySqlCommand
                            {
                                CommandText =
                                    "UPDATE `fractionvehicles` SET `defaultrank`=@defaultrank WHERE `number`=@number"
                            };
                            ucmd.Parameters.AddWithValue("@defaultrank", minrank);
                            ucmd.Parameters.AddWithValue("@number", number);
                            MySQL.Query(ucmd);
                        }
                    }

                    SpawnFractionCars();
                }

                // load fraction ranks configs
                using MySqlCommand cmdFractionRanksSetting = new MySqlCommand()
                {
                    CommandText = "SELECT * FROM `fractionranks`"
                };
                using DataTable resultFractionRanksSetting = MySQL.QueryRead(cmdFractionRanksSetting);
                if (resultFractionRanksSetting != null && resultFractionRanksSetting.Rows.Count != 0)
                {
                    foreach (DataRow dataRow in resultFractionRanksSetting.Rows)
                    {
                        var fraction = Convert.ToInt32(dataRow["fraction"]);
                        var fractionData = Manager.GetFractionData(fraction);
                        if (fractionData == null) 
                            continue;
                        
                        var rank = Convert.ToInt32(dataRow["rank"]);
                        var salary = Convert.ToInt32(dataRow["payday"]);
                        var name = dataRow["name"].ToString();
                        var access = dataRow["access"].ToString();

                        var rankData = new RankData
                        {
                            Name = name,
                            Salary = salary,
                            MaxScore = 0,
                        };
                        
                        if (access == null || access == "null" || access == "NULL" || access == "-1" || access.Length <= 1)
                        {
                            rankData.Access = fractionData.RanksDefaultAccess[rank].ToList();
                            using MySqlCommand ucmd = new MySqlCommand
                            {
                                CommandText = "UPDATE `fractionranks` SET `access`=@access WHERE `fraction`=@fraction AND `rank`=@rank"
                            };
                            ucmd.Parameters.AddWithValue("@access", JsonConvert.SerializeObject(fractionData.Ranks[rank].Access));
                            ucmd.Parameters.AddWithValue("@fraction", fraction);
                            ucmd.Parameters.AddWithValue("@rank", rank);
                            MySQL.Query(ucmd);
                        }
                        else
                        {
                            rankData.Access = JsonConvert.DeserializeObject<List<RankToAccess>>(access);
                        }

                        fractionData.Ranks.Add(rank, rankData);
                    }
                }
                InitFractionDefaultRanksName();
                using MySqlCommand cmdFractionaccess = new MySqlCommand()
                {
                    CommandText = "SELECT * FROM `fractionaccess`"
                };
                using DataTable resultFractionAccess = MySQL.QueryRead(cmdFractionaccess);
                if (resultFractionAccess != null && resultFractionAccess.Rows.Count != 0)
                {
                    foreach (DataRow dataRow in resultFractionAccess.Rows)
                    {
                        var fraction = Convert.ToInt32(dataRow["fraction"]);
                        var dictionaryWeap = JsonConvert.DeserializeObject<Dictionary<string, int>>(dataRow["weapons"].ToString());

                        //FractionCommands[fraction] = dictionaryCmd;
                        FractionWeapons[fraction] = dictionaryWeap;
                    }
                }
                Manager.onResourceStart();
            }
            catch (Exception e)
            {
                Log.Write($"LoadFractionConfigs Exception: {e.ToString()}");
            }
        }
        
        public static void SpawnFractionCars()
        {
            try
            {
                foreach (var fractionData in Manager.Fractions)
                {
                    foreach (var vehicle in fractionData.Value.Vehicles)
                    {
                        string vehname = vehicle.Value.model;
                        bool canmats = (vehname.Equals("barracks") || vehname.Equals("cargobob") ||
                                        vehname.Equals("brickade") || vehname.Equals("gburrito2") ||
                                        vehname.Equals("youga") || vehname.Equals("burrito3") ||
                                        vehname.Equals("gburrito") || vehname.Equals("terbyte"));
                        bool candrugs = (vehname.Equals("youga") || vehname.Equals("burrito3") ||
                                         vehname.Equals("vapidse") || vehname.Equals("gburrito"));
                        bool canmeds = (vehname.Equals("ambulance") || vehname.Equals("vapidse") ||
                                        vehname.Equals("rumpo2") || vehname.Equals("emsnspeedo") ||
                                        vehname.Equals("emsroamer"));
                        var veh = (ExtVehicle) VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey(vehname),
                            vehicle.Value.position, vehicle.Value.rotation.Z, vehicle.Value.color1,
                            vehicle.Value.color2, vehicle.Key, dimension: vehicle.Value.Dimension,
                            acc: VehicleAccess.Fraction, fr: fractionData.Key, minrank: vehicle.Value.rank, cm: canmats,
                            cd: candrugs, cmk: canmeds, petrol: 9999);
                        VehicleManager.FracApplyCustomization(veh, fractionData.Key);
                        if (vehname.Equals("submersible") || vehname.Equals("Thruster")) veh.SetSharedData("PETROL", 0);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"SpawnFractionCars Exception: {e.ToString()}");
            }
        }
        public static void RespawnFractionCar(ExtVehicle vehicle)
        {
            try
            {
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    var number = vehicle.NumberPlate;
                    var fractionId = vehicleLocalData.Fraction;
                    var fractionData = Manager.GetFractionData(fractionId);
                    if (fractionData == null)
                        return;
                    
                    var fractionVehicleData = fractionData.Vehicles[number];

                    string vehname = fractionVehicleData.model;
                    bool canmats = (vehname.Equals("barracks") || vehname.Equals("cargobob") || vehname.Equals("brickade") || vehname.Equals("youga") || vehname.Equals("gburrito2") || vehname.Equals("burrito3") || vehname.Equals("gburrito") || vehname.Equals("terbyte"));
                    bool candrugs = (vehname.Equals("youga") || vehname.Equals("burrito3") || vehname.Equals("gburrito"));
                    bool canmeds = (vehname.Equals("ambulance") || vehname.Equals("vapidse") || vehname.Equals("rumpo2") || vehname.Equals("emsnspeedo") || vehname.Equals("emsroamer"));

                    vehicle.Position = fractionVehicleData.position;
                    vehicle.Rotation = fractionVehicleData.rotation;
                    vehicle.Dimension = fractionVehicleData.Dimension;
                    vehicleLocalData.MinRank = fractionVehicleData.rank;

                    VehicleManager.RepairCar(vehicle);

                    if (canmats) vehicleLocalData.CanMats = true;
                    if (candrugs) vehicleLocalData.CanDrugs = true;
                    if (canmeds) vehicleLocalData.CanMedKits = true;
                    NAPI.Vehicle.SetVehicleNumberPlate(vehicle, number);
                    VehicleStreaming.SetEngineState(vehicle, false);
                    VehicleManager.FracApplyCustomization(vehicle, fractionId);
                }
            }
            catch (Exception e)
            {
                Log.Write($"RespawnFractionCar Exception: {e.ToString()}");
            }
        }

        public static bool IsFractionPolic(int fracid)
        {
            switch (fracid)
            {
                case (int)Models.Fractions.POLICE:
                case (int)Models.Fractions.FIB:
                case (int)Models.Fractions.SHERIFF:
                    return true;
                default:
                    // Not supposed to end up here. 
                    return false;
            }
        }
    }
}
