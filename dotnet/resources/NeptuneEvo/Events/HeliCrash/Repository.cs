using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Events.HeliCrash.Models;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using NeptuneEvo.Players.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Events.HeliCrash
{
    public class Repository
    {
        public static void OnResourceStart()
        {
            /*foreach (var heliCord in Cords)
            {
                NAPI.Object.CreateObject(_heliHash, heliCord.HeliPosition, heliCord.HeliRotation, dimension: 0);
                
                for (var i = 0; i < heliCord.BoxPositions.Length; i++)
                {
                    NAPI.Object.CreateObject(_boxHash, heliCord.BoxPositions[i], heliCord.BoxRotations[i],
                        dimension: 0);
                    
                }
            }*/
            Create();
        }
        
        private static uint _heliHash = NAPI.Util.GetHashKey("p_crahsed_heli_s");
        private static uint _boxHash = NAPI.Util.GetHashKey("prop_box_ammo03a");

        private static int _startBlockHour = 12;
        private static int MaxHealth = 1000;
        
        private static Dictionary<HeliType, int> HeliChances = new Dictionary<HeliType, int>()
        {
            {HeliType.Medical, 60},
            {HeliType.Army, 40}
        };

        private static Dictionary<HeliType, string> HeliDisplayName = new Dictionary<HeliType, string>()
        {
            {HeliType.Medical, "спасательный"},
            {HeliType.Army, "военный"}
        };

        private static List<HeliCords> Cords = new List<HeliCords>()
        {
         new HeliCords("Pacific Bluffs", new Vector3(-2219.26855, 113.428825, 159.576553), new Vector3(-4.92, 12.4, -0.87),
                new Vector3[]
                {
                    new Vector3(-2210.06641, 113.37027, 161.878754),
                    new Vector3(-2219.11133, 94.99838, 157.797379),
                    new Vector3(-2230.553, 106.5631, 157.4491)
                },
                new Vector3[]
                {
                    new Vector3(12.65, 4.25, -93.45),
                    new Vector3(-12.12, 8.13, -2.49),
                    new Vector3(-11.45, -4.20, 81.36)
                }
            ),

            new HeliCords("VineWood Hills", new Vector3(-1263.03662, 1142.36719, 280.522), new Vector3(-0.27, 8.07, 0.15),
                new Vector3[]
                {
                    new Vector3(-1275.26941, 1141.41138, 280.544037),
                    new Vector3(-1261.62476, 1126.43945, 276.4463),
                    new Vector3(-1249.79041, 1148.19873, 281.681061)
                },
                new Vector3[]
                {
                    new Vector3(4.38, -9.66, -92.18),
                    new Vector3(-20.54, -15.68, -0.08),
                    new Vector3(4.45, -9.29, 80.21)
                }
            ),

            new HeliCords("VineWood Hills", new Vector3(427.456573, 1467.124, 335.605652), new Vector3(14.29, 10.41, 2.26),
                new Vector3[]
                {
                    new Vector3(414.8403, 1466.51758, 333.894867),
                    new Vector3(428.476685, 1450.70569, 333.4321),
                    new Vector3(443.601624, 1468.78149, 338.3224)
                },
                new Vector3[]
                {
                    new Vector3(4.38, -2.94, -92.18),
                    new Vector3(-15.67, -1.90, -0.45),
                    new Vector3(-7.11, 4.90, 78.79)
                }
            ),

            new HeliCords("Land Act DAM", new Vector3(1795.37866, -249.620255, 291.150543), new Vector3(0.48, 4.07, -0.25),
                new Vector3[]
                {
                    new Vector3(1782.42419, -250.06604, 291.0833),
                    new Vector3(1795.90564, -233.938766, 286.6399),
                    new Vector3(1807.67737, -246.777039, 292.33136)
                },
                new Vector3[]
                {
                    new Vector3(4.38, -0.69, -92.18),
                    new Vector3(30.00, 7.07, -2.03),
                    new Vector3(-5.13, 4.02, 78.88)
                }
            ),

            new HeliCords("Palomino Highlands", new Vector3(1912.22266, -1704.93311, 197.946442), new Vector3(-5.65, -1.90, -0.69),
                new Vector3[]
                {
                    new Vector3(1898.74365, -1705.46277, 195.517609),
                    new Vector3(1902.80078, -1723.1062, 197.924942),
                    new Vector3(1913.08093, -1690.19153, 199.007324)
                },
                new Vector3[]
                {
                    new Vector3(16.74, -11.78, -93.66),
                    new Vector3(4.18, 1.33, 30.52),
                    new Vector3(-4.22, 11.08, 18.37)
                }
            ),

            new HeliCords("Los Santos Airport", new Vector3(-1306.67957, -2515.31787, 12.7466946), new Vector3(0, 1.1, 135.97),
                new Vector3[]
                {
                    new Vector3(-1293.585, -2514.79761, 12.9408665),
                    new Vector3(-1307.525, -2499.9585, 12.9418993),
                    new Vector3(-1316.6803, -2526.281, 12.94414)
                },
                new Vector3[]
                {
                    new Vector3(0, 0, -96),
                    new Vector3(0, 0, 0),
                    new Vector3(0, 0, 39)
                }
            )
        };
        
        /// <summary>
        /// 
        /// </summary>
        
        private static ExtObject _heliObj;
        private static HeliType _heliType;
        private static HeliCords _heliCords;
        private static List<HeliBox> HeliBoxs = new List<HeliBox>();
        private static List<string> TimersList = new List<string>();
        private static string MainTimer = String.Empty;
        
        private static HeliType GetRandomHeliType()
        {
            try
            {
                var maxCount = 0;
                foreach (var key in HeliChances.Keys)
                    maxCount += HeliChances[key] * 1000;
                
                var rand = new Random();
                var count = rand.Next(0, maxCount);
                
                
                maxCount = 0;
                foreach (var key in HeliChances.Keys)
                {
                    maxCount += HeliChances[key] * 1000;
                    
                    if (maxCount >= count)
                        return key;

                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }

            return HeliType.Medical;
        }
        
        
        private static HeliCords GetRandomHeliCords()
        {
            try
            {
                var rand = new Random();
                return Cords[rand.Next(0, Cords.Count)];
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
            return Cords[0];
        }
        
        public static void Create()
        {
            try
            {
	            if (MainTimer != String.Empty)
                    Timers.Stop(MainTimer);
                
                MainTimer = String.Empty;
                
                var i = 0;
                foreach (var heliBox in HeliBoxs)
                {
                    Chars.Repository.RemoveAll($"helicrash_{i + 1}");
                    KeyClamp.ClearKeyClamp(ColShapeEnums.HeliCrash, i + 1);
                    
                    i++;
                    
                    if (heliBox.Obj != null && heliBox.Obj.Exists)
                        heliBox.Obj.Delete();
                    
                    CustomColShape.DeleteColShape(heliBox.Shape);
                    
                }
                HeliBoxs.Clear();
                
                if (_heliObj != null && _heliObj.Exists)
                    _heliObj.Delete();
                
                
                foreach (var timer in TimersList)
                    Timers.Stop(timer);
                
                TimersList.Clear();

                //
                
                var time = DateTime.Now;
                if (Main.ServerNumber != 0 && time.Hour < _startBlockHour)
                    return;
                
                _heliType = GetRandomHeliType();
                _heliCords = GetRandomHeliCords();
                
                foreach (var foreachPlayer in Character.Repository.GetPlayers())
                {
                    var foreachSessionData = foreachPlayer.GetSessionData();
                    if (foreachSessionData == null) continue;
                        
                    Trigger.SendChatMessage(foreachPlayer, "~o~[HeliCrash] Через 10 минут над " +
                                                           $"{(foreachSessionData.IsRadioInterceptor ? _heliCords.Name : "штатом")} пролетит {HeliDisplayName[_heliType]} вертолёт, который передает сигнал бедствия.");
                    EventSys.SendCoolMsg(foreachPlayer,"HeliCrash", $"Через 10 минут!", $"Над {(foreachSessionData.IsRadioInterceptor ? _heliCords.Name : "штатом")} пролетит {HeliDisplayName[_heliType]} вертолёт, который передает сигнал бедствия.", "", 10000);
                }
                
                MainTimer = Timers.StartOnce(Main.ServerNumber == 0 ? 1000 * 60 : (1000 * 60 * 10), () =>
                {
                    try
                    {
                        MainTimer = String.Empty;
                
                        _heliObj = (ExtObject) NAPI.Object.CreateObject(NAPI.Util.GetHashKey("bkr_prop_meth_ammonia"), new Vector3(
                            _heliCords.HeliPosition.X, _heliCords.HeliPosition.Y,
                            _heliCords.HeliPosition.Z - 10), _heliCords.HeliRotation, 0, 0);
                
                        _heliObj.SetSharedData("HeliCrash", ObjectType.Target);
                
                        var timerName = Timers.StartOnce(1000 * 25, () =>
                        {
                            try
                            {
                                _heliObj.SetSharedData("HeliCrash", ObjectType.StartCrash);
                            }
                            catch (Exception e)
                            {
                                Debugs.Repository.Exception(e);
                            }
                        }, true);
                        TimersList.Add(timerName);
                
                        timerName = Timers.StartOnce(1000 * 30, () =>
                        {
                            try
                            {
                                _heliObj.SetSharedData("HeliCrash", ObjectType.Explode);
                            }
                            catch (Exception e)
                            {
                                Debugs.Repository.Exception(e);
                            }
                        }, true);
                        TimersList.Add(timerName);

                        timerName  = Timers.StartOnce(1000 * 38, () =>
                        {
                            try
                            {
                                if (_heliObj != null && _heliObj.Exists)
                                    _heliObj.Delete();

                                _heliObj = null;
                    
                                SpawnObjects();
                            }
                            catch (Exception e)
                            {
                                Debugs.Repository.Exception(e);
                            }
                        }, true);
                        TimersList.Add(timerName);
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                }, true);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        private static void SpawnObjects()
        {
            try
            {
                var heli = (ExtObject) NAPI.Object.CreateObject(_heliHash, _heliCords.HeliPosition, _heliCords.HeliRotation, dimension: 0);
                heli.SetSharedData("HeliCrash", ObjectType.Heli);
            
                HeliBoxs.Add(new HeliBox
                {
                    Obj = heli
                });
            
                //NewCasino.Horses.Shuffle(randomGiftsListData);

                var boxRandom = new List<int>();
                for (var i = 0; i < _heliCords.BoxPositions.Length; i++)
                    boxRandom.Add(i);
            
                boxRandom = NewCasino.Horses.Shuffle(boxRandom);
            
                for (var i = 0; i < _heliCords.BoxPositions.Length; i++)
                {
                    CreateItems($"helicrash_{i + 1}", "helicrash", boxRandom[i]);

                    var heliBox = new HeliBox();

                    heliBox.Obj = (ExtObject) NAPI.Object.CreateObject(_boxHash, _heliCords.BoxPositions[i], _heliCords.BoxRotations[i],
                        dimension: 0);
                
                    heliBox.Shape = CustomColShape.CreateCylinderColShape(_heliCords.BoxPositions[i], 1.5f, 2, 0, ColShapeEnums.HeliCrash, i + 1);

                    heliBox.Health = MaxHealth;
                    //
                
                    HeliBoxs.Add(heliBox);
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
                
        private static void CreateItems(string locationName, string location, int index)
        {
            try
            {
	            var rand = new Random();

                if (_heliType == HeliType.Medical)
                {

                    if (index == 0)
                    {
                        for (var item = 0; item < rand.Next(5, 20); item++)
                            Chars.Repository.AddNewItem(null, locationName, location, ItemId.HealthKit, 1, stack: false);   
                    }
                    else if (index == 1)
                    {
                        Chars.Repository.AddNewItem(null, locationName, location, ItemId.FlareGun, rand.Next(1, 3), WeaponRepository.GetHaliSerial(), false, 100);
                        Chars.Repository.AddNewItem(null, locationName, location, ItemId.Mask, rand.Next(1, 2), "186_0_True", stack: false);
                        Chars.Repository.AddNewItem(null, locationName, location, ItemId.Mask, 1, "38_0_True", stack: false);
                        Chars.Repository.AddNewItem(null, locationName, location, ItemId.Mask, 1, "46_0_True", stack: false);
                        Chars.Repository.AddNewItem(null, locationName, location, ItemId.Mask, 1, "177_0_True", stack: false);
                        
                        if (rand.Next(0, 100) >= 97)
                            Chars.Repository.AddNewItem(null, locationName, location, ItemId.Mask, 1, "36_0_True", stack: false);
                    }
                    else
                    {
                        Chars.Repository.AddNewItem(null, locationName, location, ItemId.GasCan, rand.Next(3, 10), stack: false);
                        
                        for (var item = 0; item < rand.Next(0, 3); item++)
                            Chars.Repository.AddNewItem(null, locationName, location, ItemId.Epinephrine, 10, stack: false);   
                    }
                    
                }
                else
                {
                    if (index == 0)
                    {
                        Chars.Repository.AddNewItem(null, locationName, location, ItemId.BodyArmor, rand.Next(3, 10), $"100", false, 100);
                        
                        var ammo = rand.Next(0, 6);
                        
                        if (ammo == 0)
                            Chars.Repository.AddNewItem(null, locationName, location, ItemId.PistolAmmo, rand.Next(20, 100), "", false, 100);
                        else if (ammo == 1)
                            Chars.Repository.AddNewItem(null, locationName, location, ItemId.RiflesAmmo, rand.Next(20, 250), "", false, 100);
                        else if (ammo == 2)
                            Chars.Repository.AddNewItem(null, locationName, location, ItemId.ShotgunsAmmo, rand.Next(20, 50), "", false, 100);
                        else if (ammo == 3)
                            Chars.Repository.AddNewItem(null, locationName, location, ItemId.SMGAmmo, rand.Next(20, 300), "", false, 100);
                        else if (ammo == 4)
                            Chars.Repository.AddNewItem(null, locationName, location, ItemId.SniperAmmo, rand.Next(20, 48), "", false, 100);
                        
                        if (rand.Next(0, 100) >= 97)
                            Chars.Repository.AddNewItem(null, locationName, location, ItemId.CarCoupon, 1, "Winky", stack: false);
                            

                    }
                    else if (index == 1)
                    {
                        for (var item = 0; item < rand.Next(4, 12); item++)
                            Chars.Repository.AddNewItem(null, locationName, location, ItemId.Material, 170, stack: false);
                        
                        Chars.Repository.AddNewItem(null, locationName, location, ItemId.HeavyShotgun, 1, WeaponRepository.GetHaliSerial(), false, 100);
                        
                        if (rand.Next(0, 100) >= 98)    
                            Chars.Repository.AddNewItem(null, locationName, location, ItemId.HeavySniperMk2, 1, WeaponRepository.GetHaliSerial(), false, 100);

                    }
                    else
                    {
                        Chars.Repository.AddNewItem(null, locationName, location, ItemId.Revolver, rand.Next(2, 8), WeaponRepository.GetHaliSerial(), false, 100);
                        Chars.Repository.AddNewItem(null, locationName, location, ItemId.MG, rand.Next(1, 3), WeaponRepository.GetHaliSerial(), false, 100);
                        Chars.Repository.AddNewItem(null, locationName, location, ItemId.AssaultRifle, rand.Next(3, 5), WeaponRepository.GetHaliSerial(), false, 100);
                        Chars.Repository.AddNewItem(null, locationName, location, ItemId.MilitaryRifle, rand.Next(1, 3), WeaponRepository.GetHaliSerial(), false, 100);
                        Chars.Repository.AddNewItem(null, locationName, location, ItemId.AssaultShotgun, rand.Next(1, 2), WeaponRepository.GetHaliSerial(), false, 100);
                        Chars.Repository.AddNewItem(null, locationName, location, ItemId.MarksmanRifle, rand.Next(1, 2), WeaponRepository.GetHaliSerial(), false, 100);
                    }
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        [Interaction(ColShapeEnums.HeliCrash)]
        public void OpenHeliCrash(ExtPlayer player, int index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            Chars.Repository.LoadOtherItemsData(player, "helicrash", index.ToString(), 12);
        }

        [Interaction(ColShapeEnums.HeliCrash, In: true)]
        public void InHeliCrash(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                if (HeliBoxs.Count <= index)
                    return;
            
                var heliBox = HeliBoxs [index];

                if (heliBox.Health == 0)
                {
                    CustomColShape.SetColShapesData(player, ColShapeEnums.HeliCrash, index, isAddColShapeData: true); 
                    return;
                }

                var keyClampData = new KeyClampData();

                keyClampData.SetName("heliCrash");
                keyClampData.ColShapesData = new ExtColShapeData(ColShapeEnums.HeliCrash, index, (int) ColShapeData.Error);
                keyClampData.EndCB = End;
                keyClampData.GetHealthCB = GetHealth;
            
                KeyClamp.SetKeyClamp(player, keyClampData);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        
        private static void End(ExtPlayer player, int value)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;
                
                var keyClampData = player.GetKeyClampData();

                if (keyClampData == null) 
                    return;
                
                if (keyClampData.ColShapesData == null)
                    return;
                
                if (HeliBoxs.Count <= keyClampData.ColShapesData.Index)
                    return;
                
                var heliBox = HeliBoxs [keyClampData.ColShapesData.Index];

                heliBox.Health = value;
                Trigger.StopAnimation(player);

                if (heliBox.Health == 0)
                {
                    var hack_chance = new Random().Next(1, 101);

                    if (hack_chance <= 50)
                    {
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,
                            LangFunc.GetText(LangType.Ru, DataName.SucZamok), 3000);
                        var position = heliBox.Obj.Position;
                        ParticleFx.PlayFXonPos(position, 500f, position.X, position.Y, position.Z, "scr_indep_fireworks",
                            "scr_indep_firework_shotburst", 5000);
                        
                        KeyClamp.ClearKeyClamp(keyClampData.ColShapesData.ColShapeId, keyClampData.ColShapesData.Index, keyClampData.ColShapesData.ListId);
                    }
                    else
                    {
                        heliBox.Health = MaxHealth;
                        Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter,
                            LangFunc.GetText(LangType.Ru, DataName.FailZamok), 3000);
                    }

                    var armylockpick = Chars.Repository.isItem(player, "inventory", ItemId.ArmyLockpick);
                    int count = (armylockpick == null) ? 0 : armylockpick.Item.Count;
                    if (count > 0)
                        Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", ItemId.ArmyLockpick, 1);
                }
                heliBox.IsHack = null;
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        private static (int, int) GetHealth(ExtPlayer player)
        {
            try
            {
                var keyClampData = player.GetKeyClampData();

                if (keyClampData == null) 
                    return (-1, 0);
            
                if (keyClampData.ColShapesData == null)
                    return (-1, 0);
            
                if (HeliBoxs.Count <= keyClampData.ColShapesData.Index)
                    return (-1, 0);
            
                var heliBox = HeliBoxs [keyClampData.ColShapesData.Index];

                if (heliBox.IsHack != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Уже кто то взламывает", 3000);
                    return (0, 0);
                }
            
                if (Chars.Repository.isItem(player, "inventory", ItemId.ArmyLockpick) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoArmyLockpick), 3000);
                    return (0, 0);
                }
            
                heliBox.IsHack = player;
                Trigger.PlayAnimation(player, "mp_weapons_deal_sting", "crackhead_bag_loop", 39);
            
                return (heliBox.Health, MaxHealth);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
            return (-1, 0);
        }
        
        [Interaction(ColShapeEnums.HeliCrash, Out: true)]
        public void OutHeliCrash(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                ClearBox(player);
                KeyClamp.ClearKeyClamp(player, ColShapeEnums.HeliCrash, index);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public static void ClearBox(ExtPlayer player)
        {
            try
            {
                if (player == null)
                    return;

                var haliBox = HeliBoxs
                    .FirstOrDefault(h => h.IsHack == player);

                if (haliBox != null)
                    haliBox.IsHack = null;
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
    }
}