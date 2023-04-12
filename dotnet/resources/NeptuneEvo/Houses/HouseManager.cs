using GTANetworkAPI;
using NeptuneEvo.Handles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using NeptuneEvo.Core;
using Redage.SDK;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using Database;
using LinqToDB;
using Localization;
using MySqlConnector;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Functions;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character;
using NeptuneEvo.Fractions;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Jobs.Models;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.Players.Phone.Auction.Models;
using NeptuneEvo.Players.Phone.Messages.Models;
using NeptuneEvo.Quests;
using NeptuneEvo.Table.Models;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;

namespace NeptuneEvo.Houses
{
    #region HouseType Class
    public class HouseType
    {
        public string Name { get; }
        public Vector3 Position { get; }
        public string IPL { get; set; }
        public Vector3 PetPosition { get; }
        public float PetRotation { get; }

        public HouseType(string name, Vector3 position, Vector3 petpos, float rotation, string ipl = "")
        {
            Name = name;
            Position = position;
            IPL = ipl;
            PetPosition = petpos;
            PetRotation = rotation;
        }

        public void Create()
        {
            if (IPL != "") NAPI.World.RequestIpl(IPL);
        }
    }
    #endregion

    public class ResidentData
    {
        public bool isFurniture { get; set; }
        public bool isPark { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }

    #region House Class
    public class House
    {
        public int ID { get; }
        public string Owner { get; private set; }
        public int Type { get; private set; }
        public Vector3 Position { get; }
        public int Price { get; set; }
        public bool Locked { get; private set; }
        [JsonIgnore] 
        public string OpenInterface { get; set; } = String.Empty;
        public int GarageID { get; set; }
        public int BankID { get; set; }
        public Dictionary<string, ResidentData> Roommates { get; set; } = new Dictionary<string, ResidentData>();
        [JsonIgnore] public int Dimension { get; set; }

        [JsonIgnore]
        public ExtBlip blip;
        [JsonIgnore]
        public string PetName;
        [JsonIgnore]
        private ExtTextLabel label;
        [JsonIgnore]
        private ExtColShape shape;

        [JsonIgnore]
        private ExtColShape intshape;
        [JsonIgnore]
        private ExtMarker intmarker;

        [JsonIgnore]
        private Dictionary<int, int> Objects = new Dictionary<int, int>();

        [JsonIgnore]
        private List<ExtPlayer> PlayersInside = new List<ExtPlayer>();
        [JsonIgnore]
        public bool Healkit = false;
        [JsonIgnore]
        private ExtColShape Healkitshape = null;
        [JsonIgnore]
        private ExtMarker Healkitmarker = null;
        [JsonIgnore]
        private ExtTextLabel Healkitlabel;
        [JsonIgnore]
        public DateTime HealkitTime = DateTime.MinValue;
        [JsonIgnore]
        public bool Alarm = false;
        [JsonIgnore]
        public int[] LockAngles = new int[5];
        [JsonIgnore]
        public byte ItemsGot = 5;
        [JsonIgnore]
        public DateTime HijackTime = DateTime.MinValue;
        [JsonIgnore]
        public DateTime Removedall = DateTime.MinValue;
        [JsonIgnore]
        private static readonly nLog Log = new nLog("Houses.HouseManager");
        [JsonIgnore]
        public bool IsAuction { get; set; }
        [JsonIgnore]
        public bool IsSave { get; set; }
        [JsonIgnore]
        public bool IsFurnitureSave { get; set; }
        
        public House(int id, string owner, int type, Vector3 position, int price, bool locked, int garageID, int bank, Dictionary<string, ResidentData> roommates, int dimensionID, bool healkit, bool alarm)
        {
            ID = id;
            Owner = owner;
            Type = type;
            Position = position;
            Price = price;
            Locked = locked;
            GarageID = garageID;
            BankID = bank;
            Roommates = roommates;
            PetName = "null";
            Dimension = dimensionID;
            Healkit = healkit;
            Alarm = alarm;
            IsAuction = Players.Phone.Auction.Repository.IsElement(AuctionType.House, id);
            
            for (int i = 0; i < 5; i++) 
                LockAngles[i] = SafeMain.SafeRNG.Next(10, 351);

            #region Creating Marker & Colshape
            shape = CustomColShape.CreateCylinderColShape(position, 1, 2, 0, ColShapeEnums.EnterHouse, id);

            #endregion

            label = (ExtTextLabel) NAPI.TextLabel.CreateTextLabel(Main.StringToU16($"Недвижимость {id}"), position + new Vector3(0, 0, 1.5), 5f, 0.4f, 0, new Color(255, 255, 255), false, 0);
            UpdateLabel();
            UpdateColShapeHealkit();
            UpdateGarage();
            
            if (Main.ServerSettings.IsHouseBlips)
                blip = (ExtBlip) NAPI.Blip.CreateBlip(374, position, 0.45f, 82, "Дом", shortRange: true);
        }

        public List<string> GetVehiclesCarNumber()
        {
            var names = Roommates
                .Keys
                .ToList();
                    
            names.Add(Owner);
                    
            return VehicleManager.GetVehiclesCarNumberToPlayer(names);
        }
        public List<string> GetVehiclesCarAndAirNumber()
        {
            var names = Roommates
                .Keys
                .ToList();
                    
            names.Add(Owner);
                    
            return VehicleManager.GetVehiclesCarAndAirNumberToPlayer(names);
        }
        public Garage GetGarageData()
        {
            if (GarageManager.Garages.ContainsKey(GarageID))
                return GarageManager.Garages[GarageID];
            
            return null;
        }
        public void UpdateColShapeHealkit()
        {
            try
            {
                if (Type != 7)
                {
                    if (Healkit)
                    {
                        if (Healkitshape != null) return;
                        Healkitmarker = (ExtMarker) NAPI.Marker.CreateMarker(1, HouseManager.HouseHealkitPos[Type - 1] - new Vector3(0, 0, 1.7), new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220), false, (uint)Dimension);
                        Healkitlabel = (ExtTextLabel) NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Аптечка"), HouseManager.HouseHealkitPos[Type - 1], 5f, 0.3f, 0, new Color(255, 255, 255), false, (uint)Dimension);
                        Healkitshape = CustomColShape.CreateCylinderColShape(HouseManager.HouseHealkitPos[Type - 1], 1, 2, (uint)Dimension, ColShapeEnums.HealkitHouse);
                    }
                    else
                    {
                        CustomColShape.DeleteColShape(Healkitshape);
                        Healkitshape = null;
                        if (Healkitmarker != null && Healkitmarker.Exists) 
                            Healkitmarker.Delete();
                        Healkitmarker = null;
                        if (Healkitlabel != null && Healkitlabel.Exists) 
                            Healkitlabel.Delete();
                        Healkitlabel = null;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"UpdateColShapeHealkit Exception: {e.ToString()}");
            }
        }

        public void UpdateLabel()
        {
            Trigger.SetMainTask(() =>
            {
                try
                {
                    string text = "";
                    if (Type != 7) text += $"~w~{HouseManager.HouseTypeList[Type].Name}\n";
                    else text += $"~y~Парковка\n";
                    if (!string.IsNullOrEmpty(Owner))
                    {
                        text += $"~p~{Owner}\n";
                        if (GarageID != 0 && GarageManager.Garages.ContainsKey(GarageID)) 
                            text += $"~w~Гараж: ~o~{GarageManager.GarageTypes[GarageManager.Garages[GarageID].Type].MaxCars} т.с.\n";
                        if (Type != 7) text += (Locked) ? "~r~Закрыто~c~\n" : "~g~Открыто~c~\n";
                    }
                    else if (IsAuction)
                    {
                        text += $"~w~Выставлен на аукцион\n";
                        if (GarageID != 0 && GarageManager.Garages.ContainsKey(GarageID)) 
                            text += $"~w~Гараж: ~o~{GarageManager.GarageTypes[GarageManager.Garages[GarageID].Type].MaxCars} т.с.\n";
                    }
                    else
                    {
                        text += $"~w~Цена: ~g~{MoneySystem.Wallet.Format(Price)}$\n";
                        if (GarageID != 0 && GarageManager.Garages.ContainsKey(GarageID)) 
                            text += $"~w~Гараж: ~o~{GarageManager.GarageTypes[GarageManager.Garages[GarageID].Type].MaxCars} т.с.\n";
                    }
                    text += $"~c~ID{ID}";
                    label.Text = text;
                }
                catch (Exception e)
                {
                    Log.Write($"UpdateLabel Exception: {e.ToString()}");
                }
            });
        }

        public void UpdateGarage()
        {
            try
            {
                if(GarageManager.Garages.ContainsKey(GarageID)) 
                    GarageManager.Garages[GarageID].CreateShape();
            }
            catch (Exception e)
            {
                Log.Write($"UpdateGarage Exception: {e.ToString()}");
            }
        }

        public void CreateAllFurnitures()
        {
            try
            {
                if (FurnitureManager.HouseFurnitures.ContainsKey(ID))
                {
                    if (FurnitureManager.HouseFurnitures[ID].Count >= 1)
                    {
                        foreach (HouseFurniture f in FurnitureManager.HouseFurnitures[ID].Values) 
                            if (f.IsSet) 
                                CreateFurniture(f);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"CreateAllFurnitures Exception: {e.ToString()}");
            }
        }
        public void CreateFurniture(HouseFurniture f)
        {
            try
            {
                GTANetworkAPI.Object obj = f.Create((uint)Dimension);
                int entityid = obj.Value;
                Objects.Add(entityid, f.Id);
                string name = null;                    
                Dictionary<string, object> ItemData = new Dictionary<string, object>
                {
                    { "Id", obj.Id },
                };
                switch (f.Name)
                {
                    case "Оружейный сейф":
                    case "Шкаф с одеждой":
                    case "Шкаф с предметами":
                    case "Взломостойкий сейф":
                        if (f.Name == "Оружейный сейф") name = "WeaponSafe";
                        else if (f.Name == "Шкаф с одеждой") name = "ClothesSafe";
                        else if (f.Name == "Шкаф с предметами") name = "SubjectSafe";
                        else if (f.Name == "Взломостойкий сейф") name = "BurglarProofSafe";
                        //obj.SetSharedData("InteriorItem", false);
                        obj.SetSharedData("furniture", true);
                        break;
                }

                
                if (Selecting.Objects.ContainsKey(entityid))
                {
                    Selecting.Objects[entityid].Type = name;
                    Selecting.Objects[entityid].Id = f.Id;
                }
            }
            catch (Exception e)
            {
                Log.Write($"CreateFurniture Exception: {e.ToString()}");
            }
        }
        public void DestroyFurnitures()
        {
            Trigger.SetMainTask(() =>
            {
                try
                {
                    lock (Objects)
                    {
                        foreach (int objdata in Objects.Keys)
                        {
                            Selecting.ObjData odata = Selecting.FindObjectByID(objdata);
                            if (odata != null)
                            {
                                Selecting.Objects.TryRemove(objdata, out _);
                                if (odata.entity != null && odata.entity.Exists) odata.entity.Delete();
                            }
                        }
                    }
                    Objects = new Dictionary<int, int>();
                }
                catch (Exception e)
                {
                    Log.Write($"DestroyFurnitures Exception: {e.ToString()}");
                } 
            });
        }
        public void DestroyFurniture(int id)
        {
            try
            {
                if (Objects.Count >= 1)
                {
                    //var obj = Objects.Where(o => o.Value == id).FirstOrDefault().Key;
                    int entityid = Objects.FirstOrDefault(o => o.Value == id).Key;
                    /*if (obj != null)
                    {
                        myid = -1;
                    }
                    lock (Objects)
                    {
                        foreach (KeyValuePair<int, int> obj in Objects)
                        {
                            if (obj.Value == id)
                            {
                                myid = obj.Key;
                                break;
                            }
                        }
                    }*/
                    if (entityid >= 0)
                    {
                        Selecting.ObjData odata = Selecting.FindObjectByID(entityid);
                        if (odata != null)
                        {
                            Selecting.Objects.TryRemove(entityid, out _);
                            if (odata.entity != null && odata.entity.Exists) odata.entity.Delete();
                        }
                        Objects.Remove(entityid);
                    }
                    else Log.Write($"Can not destroy furniture with ID {id} in house {ID}");
                }
            }
            catch (Exception e)
            {
                Log.Write($"DestroyFurniture Exception: {e.ToString()}");
            }
        }
        public void Create()
        {
            try
            {
                using MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "INSERT INTO `houses`(`id`,`owner`,`type`,`position`,`price`,`locked`,`garage`,`bank`,`roommates`) VALUES (@val0,@val1,@val2,@val3,@val4,@val5,@val6,@val7,@val8)"
                };
                cmd.Parameters.AddWithValue("@val0", ID);
                cmd.Parameters.AddWithValue("@val1", Owner);
                cmd.Parameters.AddWithValue("@val2", Type);
                cmd.Parameters.AddWithValue("@val3", JsonConvert.SerializeObject(Position));
                cmd.Parameters.AddWithValue("@val4", Price);
                cmd.Parameters.AddWithValue("@val5", Locked);
                cmd.Parameters.AddWithValue("@val6", GarageID);
                cmd.Parameters.AddWithValue("@val7", BankID);
                cmd.Parameters.AddWithValue("@val8", JsonConvert.SerializeObject(Roommates));
                MySQL.Query(cmd);
            }
            catch (Exception e)
            {
                Log.Write($"Create Exception: {e.ToString()}");
            }
        }
        public async Task Save(ServerBD db)
        {
            try
            {
                IsSave = false;
                
                MoneySystem.Bank.SetSave(BankID);

                await db.Houses
                    .Where(h => h.Id == ID.ToString())
                    .Set(h => h.Owner, Owner)
                    .Set(h => h.Type, Type)
                    .Set(h => h.Position, JsonConvert.SerializeObject(Position))
                    .Set(h => h.Price, Price)
                    .Set(h => h.Locked, Convert.ToSByte(Locked))
                    .Set(h => h.Garage, GarageID)
                    .Set(h => h.Bank, BankID)
                    .Set(h => h.Healkit, Convert.ToSByte(Healkit))
                    .Set(h => h.Roommates, JsonConvert.SerializeObject(Roommates))
                    .Set(h => h.Alarm, Convert.ToSByte(Alarm))
                    .UpdateAsync();

            }
            catch (Exception e)
            {
                Log.Write($"Save Exception: {e.ToString()}");
            }
        }
        public void Destroy()
        {
            try
            {
                RemoveAllPlayers();
                CustomColShape.DeleteColShape(shape);
                shape = null;
                CustomColShape.DeleteColShape(intshape);
                intshape = null;
                if (label != null && label.Exists) label.Delete();
                label = null;
                if (intmarker != null && intmarker.Exists) intmarker.Delete();
                intmarker = null;
                CustomColShape.DeleteColShape(Healkitshape);
                Healkitshape = null;
                if (Healkitmarker != null && Healkitmarker.Exists) Healkitmarker.Delete();
                Healkitmarker = null;
                if (Healkitlabel != null && Healkitlabel.Exists) Healkitlabel.Delete();
                Healkitlabel = null;
                DestroyFurnitures();
            }
            catch (Exception e)
            {
                Log.Write($"Destroy Exception: {e.ToString()}");
            }
        }
        public void SetLock(bool locked)
        {
            Locked = locked;
            UpdateLabel();
        }
        public void ClearOwner(bool isClearUpgraded = true, bool isSave = true)
        {
            Trigger.SetMainTask(() =>
            {
                try
                {
                    
                    var garage = GetGarageData();
                    if (garage != null)
                    {
                        var vehiclesNumber = GetVehiclesCarNumber();

                        garage?.DestroyCars(vehiclesNumber);

                        if (garage.Upgraded && isClearUpgraded)
                        {
                            garage.Destroy(false);
                            garage.Type = garage.BDType;
                            garage.Upgraded = false;               
                            
                            if (garage.Type != -1 && garage.Type != 6)
                                garage.CreateInterior();

                        }
                        
                        garage.CarSlots.Clear();
                        garage.IsSave = true;
                    }
                    
                    var names = Roommates
                        .Keys
                        .ToList();
                    
                    names.Add(Owner);

                    var ownerName = Owner;
                    
                    Owner = string.Empty;
                    Roommates.Clear();

                    if (isClearUpgraded)
                    {
                        Healkit = false;
                        Alarm = false;
                        HealkitTime = DateTime.MinValue;
                        PetName = "null";

                        UpdateColShapeHealkit();
                    }

                    UpdateLabel();
                    if (isSave)
                        IsSave = true;  
                    
                    foreach (var name in names)
                    {
                        var player = (ExtPlayer) NAPI.Player.GetPlayerFromName(name);
                        if (player != null)
                        {
                            if (ownerName != name) 
                                Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Вы были выселены из дома", 3000);
                            
                            Trigger.ClientEvent(player, "deleteCheckpoint", 333);
                            Trigger.ClientEvent(player, "deleteGarageBlip");
                        }
                    }
                    
                }
                catch (Exception e)
                {
                    Log.Write($"ClearOwner Task Exception: {e.ToString()}");
                }
            });
        }
        /// <summary>
        /// Действие при покупки/продаже дома
        /// </summary>
        /// <param name="name"></param>
        public void SetOwner(string name)
        {
            Trigger.SetMainTask(() =>
            {
                try
                {
                    var player = (ExtPlayer) NAPI.Player.GetPlayerFromName(name);
                    
                    Owner = name;
                    IsAuction = false;
                    Hotel.MoveOutPlayer(player);
                    
                    var garage = GetGarageData();
                    if (garage != null)
                    {
                        Trigger.ClientEvent(player, "createCheckpoint", 333, 1, garage.Position - new Vector3(0, 0, 1.12), 1.5f, 0, 220, 220, 0);
                        Trigger.ClientEvent(player, "createGarageBlip", garage.Position);
                        
                        var vehiclesNumber = GetVehiclesCarNumber();
                        garage.SpawnCars(vehiclesNumber, this);
                    }
                    UpdateLabel();
                    
                    IsSave = true;
                }
                catch (Exception e)
                {
                    Log.Write($"SetOwner Task Exception: {e.ToString()}");
                }
            });
        }
        public void GaragePlayerExit(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData != null && OpenInterface == sessionData.Name)
                    OpenInterface = String.Empty;
                
                var garage = GetGarageData();
                if (garage == null)
                    return;

                var names = Roommates
                    .Keys
                    .ToList();
                
                names.Add(Owner);
                
                var residents = Character.Repository.GetPlayers()
                    .Where(p => names.Contains(p.Name))
                    .ToList();

                if (residents.Count == 0) 
                    garage.SendVehiclesInsteadNearest(GetVehiclesCarNumber());
            }
            catch (Exception e)
            {
                Log.Write($"GaragePlayerExit Exception: {e.ToString()}");
            }
        }
        public void SendPlayer(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!PlayersInside.Contains(player)) PlayersInside.Add(player);
                NAPI.Entity.SetEntityPosition(player, HouseManager.HouseTypeList[Type].Position + new Vector3(0, 0, 1.12));
                Trigger.Dimension(player, Convert.ToUInt32(Dimension));
                characterData.InsideHouseID = ID;
                if (HouseManager.HouseTypeList[Type].PetPosition != null)
                {
                    if (!PetName.Equals("null")) 
                        Trigger.ClientEvent(player, "petinhouse", PetName, HouseManager.HouseTypeList[Type].PetPosition.X, HouseManager.HouseTypeList[Type].PetPosition.Y, HouseManager.HouseTypeList[Type].PetPosition.Z, HouseManager.HouseTypeList[Type].PetRotation, Dimension);
                }
                
                
                var names = Roommates
                    .Keys
                    .ToList();
                    
                names.Add(Owner);

                if (!names.Contains(player.Name))
                    BattlePass.Repository.UpdateReward(player, 25);
                
                
            }
            catch (Exception e)
            {
                Log.Write($"SendPlayer Exception: {e.ToString()}");
            }
        }
        public void RemovePlayer(ExtPlayer player, bool exit = true)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (exit)
                {
                    Trigger.Dimension(player);
                    player.Position = Position + new Vector3(0, 0, 1.12);
                    characterData.InsideHouseID = -1;
                }
                sessionData.HouseData.InvitedHouseID = -1;
                if (PlayersInside.Contains(player)) PlayersInside.Remove(player);
            }
            catch (Exception e)
            {
                Log.Write($"RemovePlayer Exception: {e.ToString()}");
            }
        }
        public void RemoveFromList(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (PlayersInside.Contains(player)) PlayersInside.Remove(player);
            }
            catch (Exception e)
            {
                Log.Write($"RemoveFromList Exception: {e.ToString()}");
            }
        }
        public void RemoveAllPlayers(ExtPlayer requster = null)
        {
            try
            {
                for (int i = PlayersInside.Count - 1; i >= 0; i--)
                {
                    ExtPlayer player = PlayersInside[i];
                    var sessionData = player.GetSessionData();
                    if (sessionData == null) continue;
                    if (requster != null && player == requster) continue;
                    var characterData = player.GetCharacterData();
                    if (characterData == null) continue;
                    if (sessionData.ActiveWeap.Item != null)
                    {
                        if (sessionData.ActiveWeap.Index == -1) continue;
                        InventoryItemData Item = Chars.Repository.GetItemData(player, "fastSlots", sessionData.ActiveWeap.Index);
                        if (Chars.Repository.ItemsInfo[Item.ItemId].functionType == newItemType.Weapons) continue;
                    }
                    NAPI.Entity.SetEntityPosition(player, Position + new Vector3(0, 0, 1.12));
                    Trigger.Dimension(player, 0);
                    sessionData.HouseData.InvitedHouseID = -1;
                    characterData.InsideHouseID = -1;
                    PlayersInside.Remove(player);
                }
            }
            catch (Exception e)
            {
                Log.Write($"RemoveAllPlayers Exception: {e.ToString()}");
            }
        }
        public void CreateInterior()
        {
            try
            {
                intmarker = (ExtMarker) NAPI.Marker.CreateMarker(1, HouseManager.HouseTypeList[Type].Position - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220), false, (uint)Dimension);

                intshape = CustomColShape.CreateCylinderColShape(HouseManager.HouseTypeList[Type].Position, 2f, 1.5f, (uint)Dimension, ColShapeEnums.ExitHouse);
            }
            catch (Exception e)
            {
                Log.Write($"CreateInterior Exception: {e.ToString()}");
            }
        }

        public void changeOwner(string newName)
        {
            Owner = newName;
            UpdateLabel();
            IsSave = true;
        }
    }
    #endregion

    class HouseManager : Script
    {
        private static readonly nLog Log = new nLog("Houses.HouseManager");

        public static List<House> Houses = new List<House>();

        public static double HouseTax = 0.026;
        
        public static List<HouseType> HouseTypeList = new List<HouseType>
        {
            // name, position
            new HouseType(LangFunc.GetText(LangType.Ru, DataName.Trailer), new Vector3(1973.056, 3816.448, 32.728), new Vector3(), 0.0f, "trevorstrailer"),
            new HouseType(LangFunc.GetText(LangType.Ru, DataName.Economy), new Vector3(151.0683, -1007.486, -99.70), new Vector3(), 0.0f,"hei_hw1_blimp_interior_v_motel_mp_milo_"),
            new HouseType(LangFunc.GetText(LangType.Ru, DataName.EcenomyPlus), new Vector3(265.9691, -1007.078, -102.0758), new Vector3(), 0.0f,"hei_hw1_blimp_interior_v_studio_lo_milo_"),
            new HouseType(LangFunc.GetText(LangType.Ru, DataName.Comfort), new Vector3(346.6991, -1013.023, -100.3162), new Vector3(349.5223, -994.5601, -99.7562), 264.0f, "hei_hw1_blimp_interior_v_apart_midspaz_milo_"),
            new HouseType(LangFunc.GetText(LangType.Ru, DataName.ComfortPlus), new Vector3(-31.35483, -594.9686, 78.9109),  new Vector3(-25.42115, -581.4933, 79.12776), 159.84f, "hei_hw1_blimp_interior_32_dlc_apart_high2_new_milo_"),
            new HouseType(LangFunc.GetText(LangType.Ru, DataName.Premium), new Vector3(-17.85757, -589.0983, 88.99482), new Vector3(-38.84652, -578.466, 88.58952), 50.8f, "hei_hw1_blimp_interior_10_dlc_apart_high_new_milo_"),
            new HouseType(LangFunc.GetText(LangType.Ru, DataName.PremiumPlus), new Vector3(-173.9419, 497.8622, 136.8341), new Vector3(-164.9799, 480.7568, 137.1526), 40.0f, "apa_ch2_05e_interior_0_v_mp_stilts_b_milo_"),
            new HouseType(LangFunc.GetText(LangType.Ru, DataName.Parking), new Vector3(), new Vector3(), 0.0f, ""),
            new HouseType(LangFunc.GetText(LangType.Ru, DataName.PremiumPlusPlus), new Vector3(373.53, 423.47, 145.05), new Vector3(373.5995, 404.09, 145.407), 234.0f, ""),
            new HouseType(LangFunc.GetText(LangType.Ru, DataName.Lux), new Vector3(-774.0425, 342.072, 195.7), new Vector3(-778.88, 320.96, 195.32), 255.0f, "apa_v_mp_h_05_b"),
        };



        public static Vector3[] HouseHealkitPos = new Vector3[9]
        {
            new Vector3(154.0405, -1000.846, -100.1333 + 1.12f),
            new Vector3(255.6072, -1000.668, -100.1422 + 1.12f),
            new Vector3(347.653, -994.1946, -100.3163 + 1.12f),
            new Vector3(-41.51153, -584.3834, 77.71024 + 1.12f),
            new Vector3(-29.57273, -585.0214, 82.78747 + 1.12f),
            new Vector3(-165.624, 495.3055, 132.7345 + 1.12f),
            new Vector3(-165.624, 495.3055, 132.7345 + 1.12f),
            new Vector3(379.2031, 417.466, 140.98 + 1.12f),
            new Vector3(-754.38, 325.07, 198.91 + 1.12f),
        };

        private static List<int> MaxRoommates = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 0, 15, 30 };

        private static int GetUID()
        {
            int newUID = 0;
            while (Houses.FirstOrDefault(h => h.ID == newUID) != null) newUID++;
            return newUID;
        }

        public static int DimensionID = 10000;

        #region Events
        
        public static void Init()
        {
            try
            {
                foreach (var houseType in HouseTypeList) 
                    houseType.Create();

                using MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "SELECT * FROM `houses`"
                };

                using DataTable result = MySQL.QueryRead(cmd);
                if (result == null || result.Rows.Count == 0)
                {
                    Log.Write("DB return null result.", nLog.Type.Warn);
                    return;
                }

                var housesSave = new List<House>();
                foreach (DataRow Row in result.Rows)
                {
                    try
                    {
                        int id = Convert.ToInt32(Row["id"].ToString());
                        string owner = Convert.ToString(Row["owner"]);
                        int type = Convert.ToInt32(Row["type"]);
                        Vector3 position = JsonConvert.DeserializeObject<Vector3>(Row["position"].ToString());
                        int price = Convert.ToInt32(Row["price"]);
                        bool locked = Convert.ToBoolean(Row["locked"]);
                        int garage = Convert.ToInt32(Row["garage"]);
                        int bank = Convert.ToInt32(Row["bank"]);
                        bool healkit = Convert.ToBoolean(Row["healkit"]);
                        var roommates = new Dictionary<string, ResidentData>();

                        var isSave = false;
                        try
                        {
                            roommates = JsonConvert.DeserializeObject<Dictionary<string, ResidentData>>(Row["roommates"].ToString());
                        }
                        catch
                        {
                            var oldRoommates = JsonConvert.DeserializeObject<List<string>>(Row["roommates"].ToString());
                            foreach(var r in oldRoommates) 
                               roommates.Add(r, new ResidentData());

                            isSave = true;
                        }
                        
                        bool alarm = Convert.ToBoolean(Row["alarm"]);

                        
                        var house = new House(id, owner, type, position, price, locked, garage, bank, roommates, DimensionID, healkit, alarm);
                    
                        if (type != 7)
                        {
                            house.CreateInterior();
                            FurnitureManager.Create(id);
                            house.CreateAllFurnitures();
                        }

                        Houses.Add(house);
                        
                        if (isSave)
                            housesSave.Add(house);
                        
                        DimensionID++;

                    }
                    catch (Exception e)
                    {
                        Log.Write($"onResourceStart Foreach Exception: {e.ToString()}");
                    }
                }

                foreach (var house in housesSave)
                    house.IsSave = true;
                
                NAPI.Object.CreateObject(0x07e08443, new Vector3(1972.76892, 3815.36694, 33.6632576), new Vector3(0, 0, -109.999962), 255, NAPI.GlobalDimension);
                NAPI.Object.CreateObject(NAPI.Util.GetHashKey("v_ilev_moteldoorcso"), new Vector3(150.8389, -1008.352, -98.85), new Vector3(0.00, 0.00, -1.0), 255, NAPI.GlobalDimension);
                Log.Write($"Loaded {Houses.Count} houses.", nLog.Type.Success);
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }
        
        public static void Event_OnPlayerDeath(ExtPlayer player, ExtPlayer entityKiller, uint weapon)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (sessionData.IsHicjacking)
                {
                    sessionData.IsHicjacking = false;
                    Trigger.ClientEvent(player, "dial", "close");
                    Trigger.ClientEvent(player, "fullblockMove", false);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Event_OnPlayerDeath Exception: {e.ToString()}");
            }
        }

        public static void Event_OnPlayerDisconnected(ExtPlayer player, DisconnectionType type, string reason)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                RemovePlayerFromHouseList(player);
            }
            catch (Exception e)
            {
                Log.Write($"Event_OnPlayerDisconnected Exception: {e.ToString()}");
            }
        }

        public static void SavingHouses(bool isRestart = false)
        {
            foreach (var house in Houses)
            {
                house.IsSave = true;
                
                if (isRestart)
                    house.IsFurnitureSave = false;
            }
        }
        #endregion

        #region Methods
        public static House GetHouse(ExtPlayer player, bool checkOwner = false)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return null;
                string playerName = sessionData.Name;
                var house = Houses.FirstOrDefault(h => h.Owner == playerName);
                if (house != null) 
                    return house;
                if (!checkOwner)
                {
                    house = Houses.FirstOrDefault(h => h.Roommates.ContainsKey(playerName));
                    return house;
                }
                return null;
            }
            catch (Exception e)
            {
                Log.Write($"GetHouse Exception: {e.ToString()}");
                return null;
            }
        }

        public static House GetHouse(string name, bool checkOwner = false)
        {
            try
            {
                var house = Houses.FirstOrDefault(h => h.Owner == name);
                if (house != null) return house;
                else if (!checkOwner)
                {
                    house = Houses.FirstOrDefault(h => h.Roommates.ContainsKey(name));
                    return house;
                }
                else return null;
            }
            catch (Exception e)
            {
                Log.Write($"GetHouse Exception: {e.ToString()}");
                return null;
            }
        }

        public static void RemovePlayerFromHouseList(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.InsideHouseID != -1)
                {
                    var house = Houses.FirstOrDefault(h => h.ID == characterData.InsideHouseID);
                    if (house == null) return;
                    house.RemoveFromList(player);
                }
            }
            catch (Exception e)
            {
                Log.Write($"RemovePlayerFromHouseList Exception: {e.ToString()}");
            }
        }

        public static void CheckAndKick(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                var house = GetHouse(player);
                if (house == null) return;
                if (house.Roommates.ContainsKey(player.Name))
                {
                    house.Roommates.Remove(player.Name);                    
                    var garage = house.GetGarageData();
                    garage?.DeleteCarToName(player.Name);
                }
            }
            catch (Exception e)
            {
                Log.Write($"CheckAndKick Exception: {e.ToString()}");
            }
        }

        public static void ChangeOwner(string oldName, string newName)
        {
            try
            {
                lock (Houses)
                {
                    foreach (var house in Houses)
                    {
                        if (house.Owner != oldName) continue;
                        house.changeOwner(newName);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"ChangeOwner Exception: {e.ToString()}");
            }
        }

        public static void UpdatePlayerHouseRoommates(string oldName, string newName)
        {
            try
            {
                lock (Houses)
                {
                    foreach (var house in Houses)
                    {
                        if (!house.Roommates.ContainsKey(oldName)) continue;
                        var roommateData = house.Roommates[oldName];
                        house.Roommates.Remove(oldName);
                        house.Roommates.Add(newName, roommateData);
                        house.IsSave = true;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"UpdatePlayerHouseRoommates Exception: {e.ToString()}");
            }
        }
        #endregion

        [Interaction(ColShapeEnums.HealkitHouse)]
        public static void OnHealkitHouse(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            int houseID = characterData.InsideHouseID;
            if (houseID == -1) return;
            var house = Houses.FirstOrDefault(h => h.ID == houseID);
            if (house == null) return;
            else if (!house.Owner.Equals(player.Name) && !house.Roommates.ContainsKey(player.Name))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHasNoHouse), 3000);
                return;
            }
            else if (house.HealkitTime > DateTime.Now)
            {
                string left = Convert.ToDateTime((house.HealkitTime - DateTime.Now).ToString()).ToString("mm:ss");
                Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.HealthkitUseCan, left), 1500);
                return;
            }
            house.HealkitTime = DateTime.Now.AddHours(1);
            player.Health = 100;
            Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.HealthkitUse), 3000);
        }

        [Interaction(ColShapeEnums.EnterHouse, In: true)]
        public static void InEnterHouse(ExtPlayer player, int index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            sessionData.HouseID = index;


            if (characterData.WorkID == (int) JobsId.Postman && sessionData.WorkData.OnWork)
            {
                var house = Houses.FirstOrDefault(h => h.ID == index);
                
                if (house != null && house.Type != 7)
                    Jobs.Gopostal.GoPostal_onEntityEnterColShape(player);
            }
        }
        [Interaction(ColShapeEnums.EnterHouse, Out: true)]
        public static void OutEnterHouse(ExtPlayer player, int _)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            sessionData.HouseID = -1;
            Trigger.ClientEvent(player, "client.houseinfo.close");
        }


        [Interaction(ColShapeEnums.EnterHouse)]
        public static void OnEnterHouse(ExtPlayer player, int index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            if (player.IsInVehicle) return;
            if (sessionData.HouseID == -1) return;

            var house = Houses.FirstOrDefault(h => h.ID == sessionData.HouseID);
            if (house == null) return;
            if (string.IsNullOrEmpty(house.Owner))
            {
                OpenHouseBuyMenu(player);
                return;
            }
            else
            {
                if (house.Type != 7)
                {
                    if (house.Locked)
                    {
                        if (sessionData.Following != null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SomebodyYouFollow), 3000);
                            return;
                        }
                        if (sessionData.Follower != null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OtpustiteChela), 3000);
                            return;
                        }
                        var playerHouse = GetHouse(player);
                        if (playerHouse != null && playerHouse.ID == house.ID) house.SendPlayer(player);
                        else if (sessionData.HouseData.InvitedHouseID == house.ID) house.SendPlayer(player);
                        else
                        {
                            if (!FunctionsAccess.IsWorking("crowbar"))
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                                return;
                            }
                            if (characterData.AdminLVL < 5)
                            {
                                if (house.Price == 0) return;
                                if (house.HijackTime > DateTime.Now)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NedavnoGrabili), 3000);
                                    return;
                                }
                                if (sessionData.ActiveWeap.Item != null)
                                {
                                    if (sessionData.ActiveWeap.Index == -1) return;
                                    var itemData = Chars.Repository.GetItemData(player, "fastSlots", sessionData.ActiveWeap.Index);
                                    if (itemData.ItemId == ItemId.Debug)
                                    {
                                        sessionData.ActiveWeap = new ItemStruct("", -1, null);
                                        return;
                                    }
                                    if (itemData.ItemId == ItemId.Crowbar)
                                    {
                                        var memberFractionData = player.GetFractionMemberData();
                                        if (memberFractionData != null)
                                        {
                                            switch (Fractions.Manager.FractionTypes[memberFractionData.Id])
                                            {
                                                case FractionsType.Mafia: // Mafia
                                                case FractionsType.Gangs: // Gangs
                                                case FractionsType.Bikers: // Bikers
                                                    if (memberFractionData.Rank < 3)
                                                    {
                                                        Notify.Send(player, NotifyType.Error,
                                                            NotifyPosition.BottomCenter,
                                                            LangFunc.GetText(LangType.Ru, DataName.HomeGrabit3Rank),
                                                            3000);
                                                        return;
                                                    }

                                                    break;
                                                default: // Организации с улучшением крайм-принадлежностей
                                                    if (!player.IsOrganizationAccess(RankToAccess.OrgCrime)) return;
                                                    break;
                                            }
                                        }

                                        if (sessionData.IsHicjacking) return;
                                        sessionData.IsHicjacking = true;
                                        if (house.Alarm) 
                                            Selecting.CallPoliceHijack(player, 0, house.Owner);
                                        
                                        Trigger.PlayAnimation(player, "mini@safe_cracking", "idle_base", 39);
                                        // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "vzlomhouse");
                                        Trigger.ClientEvent(player, "fullblockMove", true);
                                        Trigger.ClientEvent(player, "freeze", true);
                                        sessionData.CurrentStage = 0;
                                        Trigger.ClientEvent(player, "dial", "open", house.LockAngles[0]);
                                        Commands.RPChat("sme", player, LangFunc.GetText(LangType.Ru, DataName.HackingHome));
                                    }
                                }
                            }
                            else 
                                house.SendPlayer(player);
                        }
                    }
                    else 
                        house.SendPlayer(player);
                }
                else 
                    GarageManager.OnEnterGarage(player, index);
            }
        }
        [Interaction(ColShapeEnums.ExitHouse)]
        public static void OnExitHouse(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            if (characterData.InsideHouseID == -1) return;
            var house = Houses.FirstOrDefault(h => h.ID == characterData.InsideHouseID);
            if (house == null) return;
            if (sessionData.HouseData.Editing)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustFinishEdit), 3000);
                return;
            }
            house.RemovePlayer(player);
        }
        private static void HouseHijackStop(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                sessionData.IsHicjacking = false;
                Trigger.ClientEvent(player, "dial", "close");
                Trigger.ClientEvent(player, "fullblockMove", false);
                Trigger.ClientEvent(player, "freeze", false);
                Trigger.StopAnimation(player);
            }
            catch (Exception e)
            {
                Log.Write($"HouseHijackStop Exception: {e.ToString()}");
            }
        }
        public static void houseHijack(ExtPlayer player, params object[] arguments)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (sessionData.HouseID == -1 || sessionData.CurrentStage == -1 || !sessionData.IsHicjacking)
                {
                    HouseHijackStop(player);
                    return;
                }

                if (sessionData.ActiveWeap.Item != null)
                {
                    if (sessionData.ActiveWeap.Index == -1)
                    {
                        HouseHijackStop(player);
                        return;
                    }
                    var item = Chars.Repository.GetItemData(player, "fastSlots", sessionData.ActiveWeap.Index);
                    if (item.ItemId != ItemId.Crowbar)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustHaveLom), 2000);
                        HouseHijackStop(player);
                        return;
                    }
                }
                else
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustHaveLom), 2000);
                    HouseHijackStop(player);
                    return;
                }

                if (!(bool)arguments[0])
                {
                    var itemStruct = sessionData.ActiveWeap;
                    Chars.Repository.RemoveIndex(player, "fastSlots", itemStruct.Index);
                    HouseHijackStop(player);
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.LomBroken), 2000);
                    return;
                }
                var house = Houses.FirstOrDefault(h => h.ID == sessionData.HouseID);
                if (house == null)
                {
                    HouseHijackStop(player);
                    return;
                }
                if (string.IsNullOrEmpty(house.Owner))
                {
                    HouseHijackStop(player);
                    return;
                }
                int stage = sessionData.CurrentStage;
                if (stage == 4)
                {
                    sessionData.IsHicjacking = false;
                    Trigger.StopAnimation(player);
                    Trigger.ClientEvent(player, "dial", "close");
                    Trigger.ClientEvent(player, "fullblockMove", false);
                    Trigger.ClientEvent(player, "freeze", false);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucVzlomDver), 2000);
                    player.Eval($"mp.game.audio.playSoundFrontend(-1, \"Drill_Pin_Break\", \"DLC_HEIST_FLEECA_SOUNDSET\", true);");
                    house.SetLock(false);
                    house.HijackTime = DateTime.Now.AddHours(3);
                }
                else
                {
                    stage++;
                    if(stage <= 4)
                    {
                        sessionData.CurrentStage = stage;
                        Trigger.ClientEvent(player, "dial", "open", house.LockAngles[stage], true);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Stage5Suc, stage), 2000);
                        player.Eval($"mp.game.audio.playSoundFrontend(-1, \"Player_Enter_Line\", \"GTAO_FM_Cross_The_Line_Soundset\", true);");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"houseHijack Exception: {e.ToString()}");
            }
        }

        #region Menus
        public static void OpenHouseBuyMenu(ExtPlayer player)
        {
            try
            {

                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                var house = Houses.FirstOrDefault(h => h.ID == sessionData.HouseID);
                if (house == null) return;
                
                var garage = house.GetGarageData();
                
                var houseData = new Dictionary<string, object>()
                {
                    { "id", house.ID },
                    { "type", house.Type },
                };

                if (garage != null && GarageManager.GarageTypes.ContainsKey(garage.Type))
                    houseData.Add("cars", GarageManager.GarageTypes[garage.Type].MaxCars);
                
                if (house.Owner == String.Empty)
                {
                    houseData.Add("tax", Convert.ToInt32(house.Price / 100 * 0.026));
                    houseData.Add("price", house.Price);
                }
                else
                {
                    houseData.Add("owner", house.Owner);
                    houseData.Add("door", house.Locked);
                }
                
                
                Trigger.ClientEvent(player, "client.houseinfo.open", JsonConvert.SerializeObject(houseData));
            }
            catch (Exception e)
            {
                Log.Write($"OpenHouseBuyMenu Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.houseinfo.action")]
        private static void OnHouseInfoAction(ExtPlayer player, string action)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
                
                switch (action)
                {
                    case "buy":
                        if (sessionData.HouseID == -1) 
                            return;

                        var house = Houses.FirstOrDefault(h => h.ID == sessionData.HouseID);
                        
                        if (house == null || house.GarageID == 0) 
                            return;
                        
                        if (!string.IsNullOrEmpty(house.Owner))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У недвижимости уже имеется хозяин", 3000);
                            return;
                        }
                        if (house.IsAuction)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете зайти зайти в дом, так как он выставлен на торги на аукционе.", 7000);
                            return;
                        }
                        if (Players.Phone.Auction.Repository.IsBet(characterData.UUID, AuctionType.House))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете приобрести недвижимость, так как ваш дом находится на аукционе.", 6000);
                            return;
                        }
                        if (house.Price == 0 && characterData.AdminLVL <= 5)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Дом недоступен для покупки.", 3000);
                            return;
                        }
                        if (house.Price > characterData.Money)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас не хватает средств для покупки", 3000);
                            return;
                        }
                        if (Houses.Count(h => h.Owner == player.Name) >= 1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете купить больше одной недвижимости", 3000);
                            return;
                        }
                        /*var vehiclesCount = VehicleManager.GetVehiclesCountToPlayer(player.Name);
                        int maxcars = GarageManager.GarageTypes[GarageManager.Garages[house.GarageID].Type].MaxCars;
                        if (vehiclesCount > maxcars)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недвижимость, которую Вы покупаете, имеет {maxcars} машиномест, продайте лишние машины", 3000);
                            return;
                        }*/
                        Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Bank, LangFunc.GetText(LangType.Ru, DataName.BuyHouse, house.Price), DateTime.Now);
                        //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили эту недвижимость, не забудьте внести налог за неё в банкомате", 3000);
                       // Notify.Send(player, NotifyType.Success, NotifyPosition.Center, $"НЕ ЗАБУДЬТЕ ВНЕСТИ НАЛОГИ В БЛИЖАЙШЕМ БАНКОМАТЕ!", 8000);
                        CheckAndKick(player);
                        if (house.Type != 7)
                        {
                            house.SetLock(true);
                            if (HouseTypeList[house.Type].PetPosition != null) house.PetName = characterData.PetName;
                            house.SendPlayer(player);
                            house.HealkitTime = DateTime.MinValue;
                        }
                        house.SetOwner(player.Name);
                        Trigger.ClientEvent(player, "client.rieltagency.delBlip", 374, house.ID);
                        
                        var houseBalance = MoneySystem.Bank.Accounts[house.BankID];
                        houseBalance.Balance = Convert.ToInt32(house.Price / 100f * HouseManager.HouseTax) * 2;
                        houseBalance.IsSave = true;
                        
                        MoneySystem.Wallet.Change(player, -house.Price);
                        //Chars.Repository.PlayerStats(player);
                        if (house.Type == 7) GameLog.Money($"player({characterData.UUID})", $"server", house.Price, $"parkBuy({house.ID})");
                        else GameLog.Money($"player({characterData.UUID})", $"server", house.Price, $"houseBuy({house.ID})");
                        
                        qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage24, 1, isUpdateHud: true);
                        qMain.UpdateQuestsComplete(player, Zdobich.QuestName, (int) zdobich_quests.Stage24, true);
                        return;
                    case "int":
                        if (sessionData.HouseID == -1) return;
                        house = Houses.FirstOrDefault(h => h.ID == sessionData.HouseID);
                        if (house == null) return;
                        if (house.Type != 7)
                        {
                            if (!string.IsNullOrEmpty(house.Owner))
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"В этом доме уже имеется хозяин", 3000);
                                return;
                            }
                            house.SendPlayer(player);
                        }
                        else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У парковочного места нет интерьера", 3000);
                        return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"callback_housebuy Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.house.close")]
        private void CloseHouseManageMenu(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null)
                    return;
                
                var house = GetHouse(player);

                if (house != null && house.OpenInterface == sessionData.Name)
                    house.OpenInterface = String.Empty;
            }
            catch (Exception e)
            {
                Log.Write($"OpenHouseManageMenu Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.house.action")]
        private void OnHouseAction(ExtPlayer player, string action)
        {
            try
            {
                var accountData = player.GetAccountData();
                if (accountData == null) 
                    return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                House house;
                if (!action.Equals("openhouse") && !action.Equals("leavehouse"))
                {
                    house = GetHouse(player, true);
                    if (house == null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет дома", 3000);
                        return;
                    }
                }
                else
                {
                    house = GetHouse(player);
                    if (house == null)
                    {
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы не живете в доме", 3000);
                        return;
                    }
                }
                switch (action)
                {
                    case "leavehouse":
                        {
                            if (house.Roommates.ContainsKey(player.Name))
                                house.Roommates.Remove(player.Name);
                            
                            Trigger.ClientEvent(player, "deleteCheckpoint", 333);
                            Trigger.ClientEvent(player, "deleteGarageBlip");
                            
                            var garage = house.GetGarageData();
                            garage?.DeleteCarToName(player.Name);
                        }
                        return;
                    case "removeall":
                        if (characterData.InsideHouseID == -1 || house.ID != characterData.InsideHouseID)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться у себя дома для этого действия", 3000);
                            return;
                        }
                        if (house.Removedall > DateTime.Now)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы недавно выгоняли игроков из дома, следующий раз можно будет использовать в течении 5 минут.", 3000);
                            return;
                        }
                        house.Removedall = DateTime.Now.AddMinutes(5);
                        house.RemoveAllPlayers(player);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы выгнали всех из дома", 3000);
                        return;
                    case "sell":
                        int price = 0;
                        switch (accountData.VipLvl)
                        {
                            case 0: // None
                            case 1: // Bronze
                            case 2: // Silver
                                price = Convert.ToInt32(house.Price * 0.4);
                                break;
                            case 3: // Gold
                                price = Convert.ToInt32(house.Price * 0.5);
                                break;
                            case 4: // Platinum
                            case 5: // Media Platinum
                                price = Convert.ToInt32(house.Price * 0.6);
                                break;
                        }
                        Trigger.ClientEvent(player, "openDialog", "HOUSE_SELL_TOGOV", $"Вы действительно хотите продать недвижимость за ${MoneySystem.Wallet.Format(price)}?");                        
                        return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"callback_housemanage Exception: {e.ToString()}");
            }
        }
        public static void acceptHouseSellToGov(ExtPlayer player)
        {
            try
            {
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                var house = GetHouse(player, true);
                if (house == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoHome), 3000);
                    return;
                }

                if (characterData.InsideGarageID != -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны выйти из гаража", 3000);
                    return;
                }
                house.RemoveAllPlayers();
                house.ClearOwner();
                Rieltagency.Repository.OnPayDay(new List<House>()
                {
                    house
                }, new List<Business>());
                int price = 0;
                switch (accountData.VipLvl)
                {
                    case 0: // None
                    case 1: // Bronze
                    case 2: // Silver
                        price = Convert.ToInt32(house.Price * 0.4);
                        break;
                    case 3: // Gold
                        price = Convert.ToInt32(house.Price * 0.5);
                        break;
                    case 4: // Platinum
                    case 5: // Media Platinum
                        price = Convert.ToInt32(house.Price * 0.6);
                        break;
                }
                MoneySystem.Wallet.Change(player, price);
                if (house.Type == 7) 
                    GameLog.Money($"server", $"player({characterData.UUID})", Convert.ToInt32(price), $"parkSell({house.ID})");
                else 
                    GameLog.Money($"server", $"player({characterData.UUID})", Convert.ToInt32(price), $"houseSell({house.ID})");
                
                //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы продали свою недвижимость государству за {MoneySystem.Wallet.Format(price)}$", 3000);
                Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Bank, LangFunc.GetText(LangType.Ru, DataName.SellDomGos, MoneySystem.Wallet.Format(price)), DateTime.Now);
                
                // if (price >= 1000000)
                //     Admin.AdminsLog(1, $"[ВНИМАНИЕ] Игрок {player.Name}({player.Value}) продал свою недвижимость ({house.ID}) государству за {MoneySystem.Wallet.Format(price)}$", 1, "#FF0000");
                //
                // if (price >= 10000 && sessionData.LastSellOperationSum == price)
                // {
                //     Admin.AdminsLog(1, $"[ВНИМАНИЕ] Игрок {player.Name}({player.Value}) два раза подряд получил по {price}$ от продажи недвижимости ({house.ID}) государству", 1, "#FF0000");
                //     sessionData.LastSellOperationSum = 0;
                // }
                // else
                // {
                //     sessionData.LastSellOperationSum = price;
                // }
            }
            catch (Exception e)
            {
                Log.Write($"acceptHouseSellToGov Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.garage.update")]
        private void GarageUpdateLvl(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var house = GetHouse(player, true);
                if (house == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет дома", 3000);
                    return;
                }
                var garage = house.GetGarageData();
                if (garage != null && garage.Type != 6)
                {
                    if (garage.Type == 9)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Ваш дом имеет наибольшее количество гаражных мест.", 3000);
                        return;
                    }
                    
                    var type = (garage.Type + 1 == 6) ? garage.Type + 2 : garage.Type + 1;
                    
                    if (!GarageManager.GarageTypes.ContainsKey(type)) 
                        return;

                    var garageType = GarageManager.GarageTypes[type];
                    
                    if (!garageType.IsDonate)
                    {
                        if (UpdateData.CanIChange(player, garageType.Price, true) != 255) return;
                        MoneySystem.Wallet.Change(player, -garageType.Price);
                        GameLog.Money($"player({characterData.UUID})", "server", garageType.Price, $"garageUpgrade({garage.Id})");
                    } 
                    else
                    {
                        if(accountData.RedBucks < garageType.Price)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У Вас недостаточно RedBucks.", 3000);
                            return;
                        }
                        UpdateData.RedBucks(player, -garageType.Price, msg: "Улучшение гаража");
                    }

                    garage.Lock(true);
                    foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                    {
                        var foreachCharacterData = foreachPlayer.GetCharacterData();
                        if (foreachCharacterData == null) continue;
                        if (foreachCharacterData.InsideGarageID == garage.Id) 
                            garage.RemovePlayer(foreachPlayer);
                    }
                    
                    var vehiclesNumber = house.GetVehiclesCarNumber();
                    
                    garage.DestroyCars(vehiclesNumber);

                    garage.Destroy(false);
                    garage.Type = type;
                    
                    if (garage.Type != -1 && garage.Type != 6) 
                        garage.CreateInterior();

                    garage.SpawnCars(vehiclesNumber, house);

                    garage.Lock(false);

                    garage.Upgraded = true;
                    garage.IsSave = true;

                    house.UpdateLabel();

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно улучшили свой гараж до {garageType.MaxCars} мест.", 5000);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"При продаже дома все улучшения гаража будут сброшены, помните об этом!", 5000);
                }
                
            }
            catch (Exception e)
            {
                Log.Write($"GarageUpdateLvl Exception: {e.ToString()}");
            }
        }

        public static void OpenFurniture(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                var houseFurnitures = new List<Dictionary<string, object>>();
                
                foreach (var model in FurnitureManager.NameModels)
                {
                    houseFurnitures.Add(new Dictionary<string, object>()
                    {
                        { "name", model.Key },
                        { "model", model.Value.Prop },
                        { "type", model.Value.Type },
                        { "price", model.Value.Price },
                        { "items", model.Value.Items.Select(i => new { itemId = (int)i.Key, price = i.Value }) },
                    });
                }
                
                Trigger.ClientEvent(player, "client.furniture.open", 
                    JsonConvert.SerializeObject(houseFurnitures));
                
            }
            catch (Exception e)
            {
                Log.Write($"OpenFurniture Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.house.furniture.buy")]
        private void OnHouseFurnitureBuy(ExtPlayer player, string name)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if(name == "Домашняя аптечка" || name == "Сигнализация")
                {
                    if (characterData.InsideHouseID == -1)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться дома для этого действия", 3000);
                        return;
                    }
                    var house = GetHouse(player, true);
                    if (house == null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет дома", 3000);
                        return;
                    }
                    if (house.ID != characterData.InsideHouseID)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться у себя дома для этого действия", 3000);
                        return;
                    }
                    if (name == "Домашняя аптечка")
                    {
                        if (house.Type == 0 || house.Type == 7) return;
                        if (house.Healkit)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У Вас уже куплено данное улучшение.", 3000);
                            return;
                        }
                        if (UpdateData.CanIChange(player, 2500, true) != 255) return;
                        house.Healkit = true;
                        house.UpdateColShapeHealkit();
                        MoneySystem.Wallet.Change(player, -2500);
                        GameLog.Money($"player({characterData.UUID})", "server", 2500, $"buyFurn({house.ID} | Домашняя аптечка)");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Поздравляем с успешной покупкой домашней аптечки!", 3000);
                    }
                    else if (name == "Сигнализация")
                    {
                        if (house.Type == 0 || house.Type == 7) return;
                        if (house.Alarm)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У Вас уже куплено данное улучшение.", 3000);
                            return;
                        }
                        if (UpdateData.CanIChange(player, 3000, true) != 255) return;
                        house.Alarm = true;
                        MoneySystem.Wallet.Change(player, -3000);
                        GameLog.Money($"player({characterData.UUID})", "server", 3000, $"buyFurn({house.ID} | Сигнализация)");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Поздравляем с успешной покупкой сигнализации!", 3000);
                    }
                    return;
                }

                OnFurnitureBuy(player, name);
            }
            catch (Exception e)
            {
                Log.Write($"OnHouseFurnitureBuy Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.furniture.buy")]
        private void OnFurnitureBuy(ExtPlayer player, string name, int type = 0)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
  
                if (FurnitureManager.FurnitureBuyPos.DistanceTo(player.Position) > 3 && characterData.InsideHouseID == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться дома или у магазина для этого действия", 3000);
                    return;
                }
                
                var house = GetHouse(player, true);
                if (house == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет дома", 3000);
                    return;
                }
                if (FurnitureManager.FurnitureBuyPos.DistanceTo(player.Position) > 3 && house.ID != characterData.InsideHouseID)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться у себя дома для этого действия", 3000);
                    return;
                }
                if (!FurnitureManager.HouseFurnitures.ContainsKey(house.ID))
                    return;
                
                var houseFurniture = FurnitureManager.HouseFurnitures[house.ID];
                
                if (houseFurniture.Count() >= 100)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "В Вашей квартире уже слишком много мебели, продайте что-то", 3000);
                    return;
                }

                var furnitureData = FurnitureManager.NameModels.FirstOrDefault(x => x.Key == name);

                if(furnitureData.Key != null)
                {
                    if  (name == "Взломостойкий сейф" && houseFurniture.Values.Any(x => x.Name == name))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "В доме может быть только один взломостойкий сейф.", 3000);
                        return;
                    }

                    if (type == 0)
                    {
                        int money = furnitureData.Value.Price;
                        if (UpdateData.CanIChange(player, money, true) != 255) return;
                        MoneySystem.Wallet.Change(player, -money);
                        GameLog.Money($"player({characterData.UUID})", "server", money, $"buyFurn({house.ID} | {name})");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно купили {name}", 3000);
                    }
                    else
                    {
                        foreach (var itemData in furnitureData.Value.Items)
                        {
                            var count = Chars.Repository.getCountItem($"char_{characterData.UUID}", itemData.Key, bagsToggled: false);
                            if (itemData.Value > count)
                            {
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"У вас не хватает {Chars.Repository.ItemsInfo[itemData.Key].Name} {itemData.Value - count} шт.", 5000);
                                return;
                            }
                        }
                        foreach (var itemData in furnitureData.Value.Items)
                        {
                            Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", itemData.Key, itemData.Value);
                        }
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно скрафтили {name}", 3000);
                    }
                    
                    FurnitureManager.NewFurniture(house.ID, name);
                    house.IsFurnitureSave = true;
                    qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage25, 1, isUpdateHud: true);
                    qMain.UpdateQuestsComplete(player, Zdobich.QuestName, (int) zdobich_quests.Stage25, true);
                } 
                else 
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Произошла ошибка выбора мебели", 3000);

            }
            catch (Exception e)
            {
                Log.Write($"callback_furniture Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.house.furniture.use")]
        private void OnHouseFurnitureSell(ExtPlayer player, int id, int type)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                if (characterData.InsideHouseID == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться дома для этого действия", 3000);
                    return;
                }
                var house = GetHouse(player, true);
                if (house == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет дома", 3000);
                    return;
                }
                if (house.ID != characterData.InsideHouseID)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться у себя дома для этого действия", 3000);
                    return;
                }
                
                if (!FurnitureManager.HouseFurnitures.ContainsKey(house.ID))
                    return;
                
                var houseFurniture = FurnitureManager.HouseFurnitures[house.ID];
                
                if (houseFurniture.Count() == 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет мебели", 3000);
                    return;
                }

                if (!houseFurniture.ContainsKey(id))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ошибка выбора мебели, обратитесь в тех.раздел.", 3000);
                    return;
                }
                
                var furniture = houseFurniture[id];

                switch (type)
                {
                    case 0:
                        if (furniture.IsSet)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Уберите мебель перед продажей.", 3000);
                            return;
                        }

                        if (FurnitureManager.NameModels.ContainsKey(furniture.Name))
                        {
                            int price = Convert.ToInt32(FurnitureManager.NameModels[furniture.Name].Price * 0.75);
                            GameLog.Money($"player({characterData.UUID})", "server", price, $"sellFurn({house.ID} | {furniture.Name})");
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно продали {furniture.Name} за {MoneySystem.Wallet.Format(price)}$", 3000);
                            MoneySystem.Wallet.Change(player, price);
                            house.DestroyFurniture(furniture.Id);
                            houseFurniture.Remove(id);
                            
                            if(FurnitureManager.NameModels[furniture.Name].Type.Equals("Хранилища")) 
                                Chars.Repository.RemoveAll($"furniture_{house.ID}_{id}");
                            
                            house.IsFurnitureSave = true;
                            // if (price >= 1000000)
                            //     Admin.AdminsLog(1, $"[ВНИМАНИЕ] Игрок {player.Name}({player.Value}) продал {furniture.Name} за {MoneySystem.Wallet.Format(price)}$", 1, "#FF0000");
                            //
                            // if (price >= 10000 && sessionData.LastSellOperationSum == price)
                            // {
                            //     Admin.AdminsLog(1, $"[ВНИМАНИЕ] Игрок {player.Name}({player.Value}) два раза подряд получил по {price}$ от продажи {furniture.Name}", 1, "#FF0000");
                            //     sessionData.LastSellOperationSum = 0;
                            // }
                            // else
                            // {
                            //     sessionData.LastSellOperationSum = price;
                            // }
                        }
                        break;
                    case 1:
                        if (furniture.IsSet)
                        {
                            if (Chars.Repository.ItemsData.ContainsKey($"furniture_{house.ID}_{id}") && Chars.Repository.ItemsData[$"furniture_{house.ID}_{id}"].ContainsKey("furniture") && Chars.Repository.ItemsData[$"furniture_{house.ID}_{id}"]["furniture"].Count != 0)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нельзя убрать мебель пока в ней есть предметы.", 3000);
                                return;
                            }

                            house.DestroyFurniture(furniture.Id);
                            furniture.IsSet = false;
                            house.IsFurnitureSave = true;
                        }
                        else
                        {
                            if (sessionData.HouseData.Editing)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны закончить редактирование", 3000);
                                return;
                            }
                            sessionData.HouseData.Editing = true;
                            sessionData.HouseData.EditID = furniture.Id;
                            Trigger.ClientEvent(player, "startEditing", furniture.Model);
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"callback_furniture Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.house.resident.access")]
        private void OnResidentAccess(ExtPlayer player, string name, string action)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                var house = GetHouse(player);
                if (house == null)
                    return;
                
                if (house.Roommates.ContainsKey(name))
                {
                    var resident = house.Roommates[name];
                    switch (action)
                    {
                        case "isFurniture":
                            resident.isFurniture = !resident.isFurniture;
                            break;
                        case "isPark":
                            resident.isPark = !resident.isPark;
                            if (!resident.isPark)
                            {
                                var garage = house.GetGarageData();
                                garage?.DeleteCarToName(name);
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"OnResidentAccess Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.house.resident.dell")]
        private void OnResidentDell(ExtPlayer player, string name)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                var house = GetHouse(player);
                if (house == null)
                    return;
                
                if (house.Roommates.ContainsKey(name))
                {
                    house.Roommates.Remove(name);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы выселили {name} из своего дома", 3000);

                    var garage = house.GetGarageData();
                    garage?.DeleteCarToName(name);
                }
            }
            catch (Exception e)
            {
                Log.Write($"OnResidentDell Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.vehicle.tuning")]
        public void VehicleTuning(ExtPlayer player, ExtVehicle vehicle)
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
            
            if (!player.IsInVehicle)
                return;
            
            var vehicleLocalData = vehicle.GetVehicleLocalData();
            if (vehicleLocalData == null)
                return;
            
            var vehicleData = VehicleManager.GetVehicleToNumber(vehicleLocalData.NumberPlate);

            if (vehicleData == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете въехать в гараж на этой машине", 3000);
                return;
            }
            
            string model = vehicleData.Model;
            uint dim = Dimensions.RequestPrivateDimension(player.Value);
            Trigger.Dimension(vehicle, dim);
            vehicle.Position = new Vector3(-1145.9514, -2864.3853, 13.845062);
            vehicle.Rotation = new Vector3(0.01658115, 0.055898327, 149.61893);
            int modelPrice = BusinessManager.BusProductsData[model].Price;
            modelPrice = BusinessManager.CarsNames[2].Contains(model) ? modelPrice * 10 : modelPrice;

            Trigger.ClientEvent(player, "client.custom.open", 100, modelPrice, JsonConvert.SerializeObject(vehicleData.Components), 2);
        }
        
        [RemoteEvent("server.vehicle.action")]
        public void VehicleAction(ExtPlayer player, string number, string action)
        {
            try
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

                var vehicleData = VehicleManager.GetVehicleToNumber(number);

                if (vehicleData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете въехать в гараж на этой машине", 3000);
                    return;
                }

                var isAir = VehicleModel.AirAutoRoom.isAirCar(vehicleData.Model);
                
                Garage garage = null;
                
                if (!isAir)
                {
                    var house = GetHouse(player);
                    garage = house?.GetGarageData();
                    if (action != "sell" && action != "gps" && (garage == null || !garage.IsCarNumber(vehicleData.SqlId)))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете въехать в гараж на этой машине", 3000);
                        return;
                    }
                }
                switch (action)
                {
                    case "tune": 
                        if (!isAir)
                            return;
                       
                        if (player.IsInVehicle)
                            return;
                        
                        if (player.Position.DistanceTo2D(VehicleModel.AirAutoRoom.NpcSpawnPosition) > 5f)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы можете починить авто рядом с NPC", 3000);
                            return;
                        }
                       
                        var vehiclePlayer = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Personal, number);
                        var vehicleLocalData = vehiclePlayer.GetVehicleLocalData();
                        if (vehicleLocalData == null)
                            return;
                       
                        if (!VehicleModel.AirAutoRoom.IsVehicleToSpawn (vehiclePlayer.Position))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны быть рядом с парковкой", 3000);
                            return;
                        }
                        if (VehicleStreaming.GetLockState(vehiclePlayer))
                            VehicleStreaming.SetLockStatus(vehiclePlayer, false);
                        
                        Trigger.ClientEvent(player, "screenFadeOut", 0);
                        
                        //Trigger.ClientEvent(player, "setIntoVehicle", vehicle, VehicleSeat.Driver - 1, "server.vehicle.tuning");
                        Trigger.ClientEvent(player, "setIntoVehicle", vehiclePlayer, VehicleSeat.Driver - 1);
                        
                        Timers.StartOnce(1000, () =>
                        {
                            try
                            {
                                if (!player.IsCharacterData()) return;
                                
                                string model = vehicleData.Model;
                                uint dim = Dimensions.RequestPrivateDimension(player.Value);
                                Trigger.Dimension(vehiclePlayer, dim);
                                vehiclePlayer.Position = new Vector3(-1145.9514, -2864.3853, 13.845062);
                                vehiclePlayer.Rotation = new Vector3(0.01658115, 0.055898327, 149.61893);
                                int modelPrice = BusinessManager.BusProductsData[model].Price;
                                modelPrice = BusinessManager.CarsNames[2].Contains(model) ? modelPrice * 10 : modelPrice;

                                Trigger.ClientEvent(player, "client.custom.open", 100, modelPrice, JsonConvert.SerializeObject(vehicleData.Components), 2);
                            }
                            catch (Exception e)
                            {
                                Log.Write($"UseArmor Task Exception: {e.ToString()}");
                            }
                        }, true);
                        
                        break;
                    case "spawn":
                        if (vehicleData.Holder != sessionData.Name)
                        {
                            //Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "", 3000);
                            return;
                        }    
                        if (!isAir)
                            return;
                        
                        if (player.Position.DistanceTo2D(VehicleModel.AirAutoRoom.NpcSpawnPosition) > 5f)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы можете починить авто рядом с NPC", 3000);
                            return;
                        }
                        
                        var vehiclesNumber = VehicleManager.GetVehiclesAirNumberToPlayer(player.Name);
                        foreach (string numberDell in vehiclesNumber)
                            VehicleStreaming.DeleteVehicle(VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Personal, numberDell));

                        var positionData = VehicleModel.AirAutoRoom.GetSpawnPosition();
                        
                        vehicleData.Health = 1000;
                        vehiclePlayer = VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey(vehicleData.Model), positionData.Item1, positionData.Item2.Z, 0, 0, number, petrol: vehicleData.Fuel, acc: VehicleAccess.Personal, locked: true, dirt: vehicleData.Dirt);
                        VehicleManager.ApplyCustomization(vehiclePlayer);
                        
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Ваш вертолет ожидает вас на взлетной площадке.", 3000);
                        return;
                    case "sell":
                        if (vehicleData.Holder != sessionData.Name)
                        {
                            //Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "", 3000);
                            return;
                        }
                        if (Fractions.Ticket.IsVehicleTickets(vehicleData.SqlId))
                            return;
                        
                        sessionData.CarSellGov = number;
                        
                        int price = 0;
                        if (BusinessManager.BusProductsData.ContainsKey(vehicleData.Model))
                        {
                            switch (accountData.VipLvl)
                            {
                                case 0: // None
                                case 1: // Bronze
                                case 2: // Silver
                                    price = Convert.ToInt32(BusinessManager.BusProductsData[vehicleData.Model].Price * 0.4);
                                    break;
                                case 3: // Gold
                                    price = Convert.ToInt32(BusinessManager.BusProductsData[vehicleData.Model].Price * 0.5);
                                    break;
                                case 4: // Platinum
                                case 5: // Media Platinum
                                    price = Convert.ToInt32(BusinessManager.BusProductsData[vehicleData.Model].Price * 0.6);
                                    break;
                                default:
                                    price = Convert.ToInt32(BusinessManager.BusProductsData[vehicleData.Model].Price * 0.4);
                                    break;
                            }
                        }
                        Trigger.ClientEvent(player, "openDialog", "CAR_SELL_TOGOV", $"Вы действительно хотите продать государству {vehicleData.Model} ({number}) за ${MoneySystem.Wallet.Format(price)}?");
                        return;
                    case "repair":
                        if (isAir && player.Position.DistanceTo2D(VehicleModel.AirAutoRoom.NpcSpawnPosition) > 15f)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы можете починить авто рядом с NPC", 3000);
                            return;
                        }
                        if (Ticket.IsVehicleTickets(vehicleData.SqlId))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Машина {vehicleData.Model} ({number}) была отогнана на штрафстоянку", 3000);
                            return;
                        }
                        
                        vehiclePlayer = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Personal, number);
                        if (vehiclePlayer == null) return;
                        
                        if (vehiclePlayer.Health > 0 && vehicleData.Health > 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Машина {vehicleData.Model} ({number}) не нуждается в восстановлении", 3000);
                            return;
                        }

                        
                        int vClass = NAPI.Vehicle.GetVehicleClass(NAPI.Util.VehicleNameToModel(vehicleData.Model));
                        if (!MoneySystem.Wallet.Change(player, -VehicleManager.VehicleRepairPrice[vClass]))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoMoney), 3000);
                            return;
                        }
                        //vData.Items = new List<nItem>();
                        //Chars.Repository.RemoveAll(VehicleManager.GetVehicleToInventory(number));
                        GameLog.Money($"player({characterData.UUID})", $"server", VehicleManager.VehicleRepairPrice[vClass], $"carRepair({vehicleData.Model}, {number})");
                        vehiclePlayer.Repair();
                        vehicleData.Health = 1000;
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы восстановили {vehicleData.Model} ({number})", 3000);
                        return;
                    case "evac":

                        if (Ticket.IsVehicleTickets(vehicleData.SqlId))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Машина {vehicleData.Model} ({number}) была отогнана на штрафстоянку", 3000);
                            return;
                        }
                        if (characterData.Money < Main.EvacCar)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно средств (не хватает {Main.EvacCar - characterData.Money}$)", 3000);
                            return;
                        }
                        
                        if (isAir)
                        {
                            if (player.Position.DistanceTo2D(VehicleModel.AirAutoRoom.NpcSpawnPosition) > 5f)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы можете починить авто рядом с NPC", 3000);
                                return;
                            }
                        }
                        else
                        {
                            if (garage == null) return;
                            if (!garage.InGarage(player))
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться около гаража", 3000);
                                return;
                            }

                            if (garage.IsGarageToNumber(vehicleData.SqlId))
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Эта машина стоит в гараже", 3000);
                                return;
                            }
                        }
                        vehiclePlayer = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Personal, number);
                        if (vehiclePlayer == null) 
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Нельзя эвакуировать машину.", 3000);
                            return;
                        }                   
                        if (isAir)
                        {
                            if (VehicleModel.AirAutoRoom.IsVehicleToSpawn (vehiclePlayer.Position))
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"т/с и так стоит на парковке", 3000);
                                return;
                            }
                        }
                        vehicleLocalData = vehiclePlayer.GetVehicleLocalData();
                        if (vehicleLocalData == null || vehicleLocalData.Occupants.Count >= 1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Нельзя эвакуировать машину, пока в ней кто-то находится.", 3000);
                            return;
                        }
                        vehicleData.Fuel = vehicleLocalData.Petrol;

                        if (isAir)
                        {
                            VehicleStreaming.DeleteVehicle(vehiclePlayer);

                            positionData = VehicleModel.AirAutoRoom.GetSpawnPosition();
                        
                            vehicleData.Health = 1000;
                            vehiclePlayer = VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey(vehicleData.Model), positionData.Item1, positionData.Item2.Z, 0, 0, number, petrol: vehicleData.Fuel, acc: VehicleAccess.Personal, locked: true, dirt: vehicleData.Dirt);
                            VehicleManager.ApplyCustomization(vehiclePlayer);
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Ваш вертолет эвакуирован", 3000);
                        }
                        else
                        {
                            
                            if (garage.Type != -1 && garage.Type != 6) garage.SendVehicleIntoGarage(number);
                            else
                            {
                                garage.DeleteCar(number, true);
                                garage.GetVehicleFromGarage(number);
                            }
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Ваша машина была отогнана в гараж", 3000);
                        }
                        
                        MoneySystem.Wallet.Change(player, -Main.EvacCar);
                        BattlePass.Repository.UpdateReward(player, 46);
                        GameLog.Money($"player({characterData.UUID})", $"server", Main.EvacCar, $"carEvac");
                        return;
                    /*case "evac_pos":
                        if (!player.IsCharacterData()) return;

                        garage = GarageManager.Garages[GetHouse(player).GarageID];
                        if (garage.Type == -1)
                        {
                            if (player.Position.DistanceTo(garage.Position) > 4)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться около гаража", 3000);
                                return;
                            }
                        }
                        else if (garage.Type == 6)
                        {
                            if (player.Position.DistanceTo(garage.Position) > 4)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться около парковочного места", 3000);
                                return;
                            }
                        }
                        else
                        {
                            if (characterData.InsideGarageID == -1)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться в гараже", 3000);
                                return;
                            }
                        }
                        number = menu.Items[0].Text;

                        if(!VehicleManager.Vehicles.ContainsKey(number)) return;
                        if(vehicleData.Health == 0) return;

                        if (string.IsNullOrEmpty(vehicleData.Position))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Машина не нуждается в эвакуации", 3000);
                            return;
                        }

                        vehicleData.Position = null;
                        VehicleManager.Save(number);

                        garage.SendVehicleIntoGarage(number);
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Ваша машина была эвакуирована в гараж", 3000);
                        return;*/
                    case "gps":
                        if (!isAir)
                        {
                            if (garage == null) return;
                            if (garage.IsGarageToNumber(vehicleData.SqlId))
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                                    "Эта машина стоит в гараже", 3000);
                                return;
                            }
                        }

                        if (Ticket.IsVehicleTickets(vehicleData.SqlId))
                        {
                            Trigger.ClientEvent(player, "createWaypoint", Ticket.PedPos.X, Ticket.PedPos.Y);
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,
                                "Ваш автомобиль находится на штрафстоянке. Метка установлена на карте.", 3000);
                        }
                        else
                        {
                            vehiclePlayer = VehicleData.LocalData.Repository.GetVehicleToNumber(VehicleAccess.Personal, number);
                            if (vehiclePlayer == null)
                            {
                                if (isAir)
                                {
                                    Trigger.ClientEvent(player, "createWaypoint", VehicleModel.AirAutoRoom.NpcSpawnPosition.X, VehicleModel.AirAutoRoom.NpcSpawnPosition.Y);
                                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,
                                        "В GPS было отмечено расположение вызова т/с", 3000);
                                }
                                return;
                            }

                            if (vehiclePlayer.Health == 0) return;
                            
                            Trigger.ClientEvent(player, "createWaypoint", vehiclePlayer.Position.X, vehiclePlayer.Position.Y);
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,
                                "В GPS было отмечено расположение Вашей машины", 3000);
                        }

                        return;
                    case "key":
                        if (vehicleData.Holder != sessionData.Name)
                        {
                            //Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "", 3000);
                            return;
                        }
                        if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.CarKey, 1, $"{vehicleData.SqlId}_{vehicleData.KeyNum}") == -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре", 3000);
                            return;
                        }
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили ключ от машины с номером {number}", 3000);
                        BattlePass.Repository.UpdateReward(player, 96);
                        return;
                    case "changekey":
                        if (vehicleData.Holder != sessionData.Name)
                        {
                            //Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "", 3000);
                            return;
                        }

                        if (!isAir)
                        {
                            if (garage?.Type == -1)
                            {
                                if (player.Position.DistanceTo(garage.Position) > 4)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                                        $"Вы должны находиться около гаража", 3000);
                                    return;
                                }
                            }
                            else if (garage?.Type == 6)
                            {
                                if (player.Position.DistanceTo(garage.Position) > 4)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                                        $"Вы должны находиться около парковочного места", 3000);
                                    return;
                                }
                            }
                            else
                            {
                                if (characterData.InsideGarageID == -1)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                                        LangFunc.GetText(LangType.Ru, DataName.MustBeInGarage), 3000);
                                    return;
                                }
                            }
                        }

                        if (!MoneySystem.Wallet.Change(player, -100))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Смена замков стоит $100", 3000);
                            return;
                        }

                        vehicleData.KeyNum++;
                        Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.CarKey, 1, $"{vehicleData.SqlId}_{vehicleData.KeyNum}");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы сменили замки на машине {number}. Теперь старые ключи не могут быть использованы", 3000);
                        return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"callback_selectedcar Exception: {e.ToString()}");
            }
        }

        
        #endregion

        #region Commands
        public static void InviteToRoom(ExtPlayer player, ExtPlayer guest)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var guestSessionData = guest.GetSessionData();
                if (guestSessionData == null) return;

                var house = GetHouse(player, true);
                if (house == null || house.Type == 7)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoHome), 3000);
                    return;
                }
                if ((sessionData.SellItemData.Buyer != null || sessionData.SellItemData.Seller != null) && Chars.Repository.TradeGet(player))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недоступно в данный момент.", 3000);
                    return;
                }
                if (characterData.AdminLVL != 9 && house.Roommates.Count >= MaxRoommates[house.Type])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас в доме проживает максимальное кол-во игроков", 3000);
                    return;
                }
                if (GetHouse(guest) != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок уже живет в доме", 3000);
                    return;
                }
                if ((guestSessionData.SellItemData.Buyer != null || guestSessionData.SellItemData.Seller != null) && Chars.Repository.TradeGet(guest))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недоступно в данный момент.", 3000);
                    return;
                }
                sessionData.SellItemData.Seller = player;
                sessionData.SellItemData.Seller = guest;

                guestSessionData.SellItemData.Seller = player;
                guestSessionData.SellItemData.Buyer = guest;
                Trigger.ClientEvent(guest, "openDialog", "ROOM_INVITE", $"Игрок ({player.Value}) предложил Вам подселиться к нему");

                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы предложили игроку ({guest.Value}) подселиться к Вам", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"InviteToRoom Exception: {e.ToString()}");
            }
        }

        public static void acceptRoomInvite(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;

                ExtPlayer owner = sessionData.SellItemData.Seller;
                var ownerSessionData = owner.GetSessionData();
                if (ownerSessionData == null) return;
                var ownerCharacterData = owner.GetCharacterData();
                if (ownerCharacterData == null)
                {
                    sessionData.SellItemData = new SellItemData();
                    return;
                }
                var house = GetHouse(owner, true);
                if (house == null || house.Type == 7)
                {
                    ownerSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока нет личного дома", 3000);
                    return;
                }
                if (ownerCharacterData.AdminLVL != 9 && house.Roommates.Count >= MaxRoommates[house.Type])
                {
                    ownerSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"В доме проживает максимальное кол-во игроков", 3000);
                    return;
                }
                if (house.Roommates.ContainsKey(player.Name))
                {
                    ownerSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы уже подселены в этот дом.", 3000);
                    return;
                }
                house.Roommates.Add(player.Name, new ResidentData());
                Trigger.ClientEvent(player, "createCheckpoint", 333, 1, GarageManager.Garages[house.GarageID].Position - new Vector3(0, 0, 1.12), 1.5f, 0, 220, 220, 0);
                Trigger.ClientEvent(player, "createGarageBlip", GarageManager.Garages[house.GarageID].Position);

                Notify.Send(owner, NotifyType.Info, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) подселился к Вам", 3000);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы подселились к игроку ({owner.Value})", 3000);

                ownerSessionData.SellItemData = new SellItemData();
                sessionData.SellItemData = new SellItemData();
            }
            catch (Exception e)
            {
                Log.Write($"acceptRoomInvite Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.createhouse)]
        public static async void CMD_CreateHouse(ExtPlayer player, int type, int price)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.createhouse)) return;
                if (type < 0 || type >= HouseTypeList.Count)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Неправильный тип", 3000);
                    return;
                }
                int bankId = await MoneySystem.Bank.Create(string.Empty, 2, 0);
                DimensionID++;
                House new_house = new House(GetUID(), string.Empty, type, player.Position - new Vector3(0, 0, 1.12), price, false, 0, bankId, new Dictionary<string, ResidentData>(), DimensionID, false, false);
                new_house.Create();
                FurnitureManager.Create(new_house.ID);
                new_house.CreateInterior();
                Houses.Add(new_house);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_CreateHouse Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.setparkplace)]
        public static async void CMD_CreateParkPlace(ExtPlayer player, int price)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.setparkplace)) return;
                if (!player.IsInVehicle)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны сидеть в машине", 3000);
                    return;
                }
                int bankId = await MoneySystem.Bank.Create(string.Empty, 2, 0);
                DimensionID++;
                int id = 0;
                do
                {
                    id++;
                } while (GarageManager.Garages.ContainsKey(id));
                var garage = new Garage(id, 6, player.Vehicle.Position, player.Vehicle.Rotation);
                garage.Dimension = DimensionID;
                garage.Create();
                GarageManager.Garages.Add(garage.Id, garage);
                var house = new House(GetUID(), string.Empty, 7, player.Position - new Vector3(0, 0, 1.12), price, false, garage.Id, bankId, new Dictionary<string, ResidentData>(), DimensionID, false, false);
                house.Create();
                Houses.Add(house);
                house.UpdateLabel();
            }
            catch (Exception e)
            {
                Log.Write($"CMD_CreateParkPlace Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.removehouse)]
        public static void CMD_RemoveHouse(ExtPlayer player, int id, bool withgarage = false)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.removehouse)) return;

                var house = Houses.FirstOrDefault(h => h.ID == id);
                if (house == null) return;

                if (withgarage)
                {
                    var garage = house.GetGarageData();
                    if (garage != null)
                    {
                        garage.Destroy();
                        garage.Delete();
                        GarageManager.Garages.Remove(house.GarageID);
                    }
                }

                house.Destroy();
                Houses.Remove(house);

                using MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "DELETE FROM `houses` WHERE `id`=@val0"
                };
                cmd.Parameters.AddWithValue("@val0", house.ID);
                MySQL.Query(cmd);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_RemoveHouse Exception: {e.ToString()}");
            }
        }
        [Command(AdminCommands.housechange)]
        public static void CMD_HouseOwner(ExtPlayer player, string newOwner)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.housechange)) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (sessionData.HouseID == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться на маркере дома", 3000);
                    return;
                }
                var house = Houses.FirstOrDefault(h => h.ID == sessionData.HouseID);
                if (house == null) return;

                house.changeOwner(newOwner);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_HouseOwner Exception: {e.ToString()}");
            }
        }

        [Command("myguest")]
        public static void CMD_InvitePlayerToHouse(ExtPlayer player, int id)
        {
            try
            {
                ExtPlayer guest = Main.GetPlayerByID(id);
                if (guest == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок не найден", 3000);
                    return;
                }
                if (player.Position.DistanceTo(guest.Position) > 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы находитесь слишком далеко", 3000);
                    return;
                }
                InvitePlayerToHouse(player, guest);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_InvitePlayerToHouse Exception: {e.ToString()}");
            }
        }

        public static void InvitePlayerToHouse(ExtPlayer player, ExtPlayer guest)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                var guestSessionData = guest.GetSessionData();
                if (guestSessionData == null) return;

                var house = GetHouse(player);
                if (house == null || house.Type == 7)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет дома", 3000);
                    return;
                }
                guestSessionData.HouseData.InvitedHouseID = house.ID;
                Notify.Send(guest, NotifyType.Info, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) пригласил Вас в свой дом", 3000);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы пригласили игрока ({guest.Value}) в свой дом", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"InvitePlayerToHouse Exception: {e.ToString()}");
            }
        }

        [Command("sellhouse")]
        public static void CMD_sellHouse(ExtPlayer player, int id, int price)
        {
            try
            {
                OfferHouseSell(player, Main.GetPlayerByID(id), price);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_sellHouse Exception: {e.ToString()}");
            }
        }

        public static void OfferHouseSell(ExtPlayer player, ExtPlayer target, int price)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null) return;
                if (!target.IsCharacterData()) return;

                if ((sessionData.SellItemData.Seller != null || sessionData.SellItemData.Buyer != null) && Chars.Repository.TradeGet(player))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantTrade), 3000);
                    return;
                }
                if (player.Position.DistanceTo(target.Position) > 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы находитесь слишком далеко от покупателя", 3000);
                    return;
                }
                var house = GetHouse(player, true);
                if (house == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoHome), 3000);
                    return;
                }
                if (GetHouse(target, true) != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока уже есть недвижимость", 3000);
                    return;
                }
                int lowprice = house.Price / 2;
                if (house.Type <= 2)
                {
                    int highprice = house.Price * 3;
                    if (price > highprice || price < lowprice)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Цена должна быть не меньше {lowprice} и не больше {highprice}", 5000);
                        return;
                    }
                }
                else if (price < lowprice)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Цена должна быть не меньше {lowprice}", 5000);
                    return;
                }
                if (player.Position.DistanceTo(house.Position) > 30)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы находитесь слишком далеко от недвижимости", 3000);
                    return;
                }
                if ((targetSessionData.SellItemData.Seller != null || targetSessionData.SellItemData.Buyer != null) && Chars.Repository.TradeGet(target))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerAlreadyTraded), 3000);
                    return;
                }
                targetSessionData.SellItemData.Seller = player;
                targetSessionData.SellItemData.Buyer = target;
                targetSessionData.SellItemData.Price = price;
                sessionData.SellItemData.Seller = player;
                sessionData.SellItemData.Buyer = target;
                sessionData.SellItemData.Price = price;
                
                var garage = house.GetGarageData();
                if (garage == null)
                    return;
                
                int maxCarsAmount = GarageManager.GarageTypes[garage.Type].MaxCars;
                Trigger.ClientEvent(target, "openDialog", "HOUSE_SELL", $"Игрок ({player.Value}) предложил Вам купить свою недвижимость на {maxCarsAmount} гаражных мест за ${MoneySystem.Wallet.Format(price)}");
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы предложили игроку ({target.Value}) купить Вашу недвижимость на {maxCarsAmount} гаражных мест за ${MoneySystem.Wallet.Format(price)}", 5000);
            }
            catch (Exception e)
            {
                Log.Write($"OfferHouseSell Exception: {e.ToString()}");
            }
        }

        public static void acceptHouseSell(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                ExtPlayer seller = sessionData.SellItemData.Seller;
                var sellerSessionData = seller.GetSessionData();
                if (sellerSessionData == null) return;
                int price = sessionData.SellItemData.Price; 
                var sellerCharacterData = player.GetCharacterData();
                if (sellerCharacterData == null)
                {
                    sessionData.SellItemData = new SellItemData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SellerNotOnline), 3000);
                    return;
                }
                if (GetHouse(player, true) != null)
                {
                    sellerSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас уже есть недвижимость", 3000);
                    return;
                }
                var house = GetHouse(seller, true);
                if (house == null || house.Owner != seller.Name)
                {
                    sellerSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    return;
                }
                int tax = Convert.ToInt32(house.Price / 100 * 0.026);
                if (MoneySystem.Bank.GetBalance(house.BankID) < (tax * 2))
                {
                    sellerSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Продавец должен оплатить налоги минимум на 2 часа перед продажей недвижимости Вам.", 4000);
                    return;
                }

                var vehiclesCount = VehicleManager.GetVehiclesCarCountToPlayer(player.Name);
                if (vehiclesCount > GarageManager.GarageTypes[GarageManager.Garages[house.GarageID].BDType].MaxCars)
                {
                    sellerSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недвижимость, которую Вы хотите купить, имеет гараж на меньшее количество машин.", 3000);
                    return;
                }
                if (UpdateData.CanIChange(player, price, true) != 255)
                {
                    sellerSessionData.SellItemData = new SellItemData();
                    sessionData.SellItemData = new SellItemData();
                    return;
                }
                else if (Main.ServerNumber != 0 && (characterData.AdminLVL >= 1 && characterData.AdminLVL <= 6))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AdminTransferRestricted), 3000);
                    return;
                }
                MoneySystem.Wallet.Change(player, -price);
                CheckAndKick(seller);
                MoneySystem.Wallet.Change(seller, price);
                if (house.Type == 7) GameLog.Money($"player({characterData.UUID})", $"player({sellerCharacterData.UUID})", price, $"parkSell({house.ID})");
                else GameLog.Money($"player({characterData.UUID})", $"player({sellerCharacterData.UUID})", price, $"houseSell({house.ID})");
                house.ClearOwner(false, false);
                house.PetName = characterData.PetName;
                house.SetOwner(player.Name);
                Notify.Send(seller, NotifyType.Info, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) купил у Вас недвижимость", 3000);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы купили недвижимость у игрока ({seller.Value})", 3000);
                //Chars.Repository.PlayerStats(seller);
                //Chars.Repository.PlayerStats(player);
                sellerSessionData.SellItemData = new SellItemData();
                sessionData.SellItemData = new SellItemData();
            }
            catch (Exception e)
            {
                Log.Write($"acceptHouseSell Exception: {e.ToString()}");
            }
        }
        #endregion
    }
}
