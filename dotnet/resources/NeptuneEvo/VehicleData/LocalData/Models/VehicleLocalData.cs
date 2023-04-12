using System;
using System.Collections.Generic;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Jobs.Models;

namespace NeptuneEvo.VehicleData.LocalData.Models
{
    public class VehicleLocalData
    {
        public int Petrol { get; set; } = -1;
        public int Class { get; set; } = -1;
        public int MaxPetrol { get; set; } = 0;
        public ExtPlayer Owner { get; set; } = null;
        public int WorkDriver { get; set; } = -1;
        public bool CanMats { get; set; } = false;
        public bool CanDrugs { get; set; } = false;
        public bool CanMedKits { get; set; } = false;
        public string By { get; set; } = "NONE";
        public VehicleAccess Access { get; set; } = VehicleAccess.None;
        public string NumberPlate { get; set; } = "null";
        public int Fraction { get; set; } = -1;
        public int MinRank { get; set; } = 100;
        public bool BagInUse { get; set; } = false;
        public ExtPlayer AttachToPlayer { get; set; } = null;
        public int Number { get; set; } = -1;
        public ExtPlayer LoaderMats { get; set; } = null;
        public DeliveryData DeliveryData { get; set; } = new DeliveryData();
        public List<ExtPlayer> Occupants { get; set; } = new List<ExtPlayer>();
        public JobsId WorkId { get; set; } = JobsId.None;
        public bool SpecialTaxiVeh { get; set; } = false;
        public DateTime VehHijackTime { get; set; } = DateTime.MinValue;
        public bool IsOwnerExit { get; set; } = false;
        public DateTime RentCarDeleteTime { get; set; } = DateTime.Now;
        public int RentCarPrice { get; set; } = 0;
        public DateTime RentCarTime { get; set; } = DateTime.Now;
        public string RentCarModel { get; set; } = null;
        public int VehLoadedFuel { get; set; } = 0;   
        public bool IsDeath { get; set; } = false;     
        public DateTime DeathTime { get; set; } = DateTime.Now;
        public bool IsFbAttach { get; set; } = false;   
        public DateTime ExitTime { get; set; } = DateTime.Now;   
        public bool IsTicket { get; set; } = false;   
        
        public uint Centimeters { get; set; } = 0;   
        public int Mileage { get; set; } = 0;   

    }
}