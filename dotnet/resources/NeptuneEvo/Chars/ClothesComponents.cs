using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Text;
using LinqToDB;
using Localization;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Quests;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NeptuneEvo.Chars
{
    public enum ClothesComponent
    {
        Hat = 0,
        Tops = 1,
        Undershort = 2,
        Legs = 3,
        Shoes = 4,
        Watches = 5,
        Glasses = 6,
        Accessories = 7,
        Ears = 8,
        Torsos = 10,
        BodyArmors = 11,
        Undershirts = 12,
        Masks = 13,
        Bugs = 14,
        Bracelets,
        Decals,
        None,
        //
    }

    class ClothesComponentId
    {
        public int SlotId;
        public ItemId ItemId;
        public int AccessoriesSlotId;

        public ClothesComponentId(int slotId, ItemId itemId, int accessoriesSlotId)
        {
            SlotId = slotId;
            ItemId = itemId;
            AccessoriesSlotId = accessoriesSlotId;
        }
        
    }
    class ClothesData
    {
        /// <summary>
        /// Уникальнай id Одежды
        /// </summary>
        public int Variation { get; set; } = 0;
        /// <summary>
        /// Торс который будет одет при использовании
        /// </summary>
        public int Torso { get; set; } = 0;
        [JsonIgnore]
        public bool IsHair { get; set; } = false;
        [JsonIgnore]
        public bool IsHat { get; set; } = false;
        [JsonIgnore]
        public bool IsGlasses { get; set; } = false;
        [JsonIgnore]
        public int MaxSlots { get; set; } = 0;
        /// <summary>
        /// Текстуры одежды
        /// </summary>
        public string TName { get; set; } = "";
        public List<int> Textures { get; set; }
        [JsonIgnore]
        public int Similar { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int Type { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int Undershirt { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int UndershirtButtoned { get; set; } = 0;
        [JsonIgnore]
        public int UndershirtTorso { get; set; } = 0;
        [JsonIgnore]
        public int UndershirtButtonedTorso { get; set; } = 0;
        [JsonIgnore]
        public bool IsClearLegs { get; set; } = false;
        /// <summary>
        /// Цена
        /// </summary>
        public int Price { get; set; } = 0;
        public int Donate { get; set; } = 0;
        [JsonIgnore]
        public sbyte Gender { get; set; } = -1;
        public Dictionary<int, int> Torsos { get; set; }
    }

    class BarberData
    {
        /// <summary>
        /// Уникальнай id Одежды
        /// </summary>
        public int Variation { get; set; } = 0;
        public string Name { get; set; } = "";
        public string TName { get; set; } = "";
        public int Similar { get; set; } = 0;
        public int Price { get; set; } = 0;
        public int Donate { get; set; } = 0;
    }
    class TattooData
    {
        public string Name { get; set; } = "";
        public string Dictionary { get; set; } = "";
        public string MaleHash { get; set; } = "";
        public string FemaleHash { get; set; } = "";
        public List<int> Slots { get; set; }
        public int Price { get; set; } = 0;
        public int Donate { get; set; } = 0;
    }


    class ClothesComponents : Script
    {
        private static readonly nLog Log = new nLog("Chars.ClothesComponents");

        public static ConcurrentDictionary<bool, ConcurrentDictionary<ClothesComponent, ConcurrentDictionary<int, ClothesData>>> ClothesComponentData = new ConcurrentDictionary<bool, ConcurrentDictionary<ClothesComponent, ConcurrentDictionary<int, ClothesData>>>();

        public static ConcurrentDictionary<bool, Dictionary<string, List<List<object>>>> ClothesComponentPriceData =
            new ConcurrentDictionary<bool, Dictionary<string, List<List<object>>>>();
        
        public static ConcurrentDictionary<bool, Dictionary<string, List<List<object>>>> ClothesComponentDonateData =
            new ConcurrentDictionary<bool, Dictionary<string, List<List<object>>>>();

        public static ConcurrentDictionary<int, ClothesData> ClothesBugsData = new ConcurrentDictionary<int, ClothesData>();
        [Command(AdminCommands.Refresh)]
        public void CMD_Refreshclothes(ExtPlayer player, string name)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.Refresh)) return;

                switch (name)
                {
                    case "clothes":
                        OnResourceStart();
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы обновили цены на одежду.", 3000);
                        break;
                    
                    case "prod":
                        BusinessManager.InitBusProducts();
                        
                        foreach (int b in BusinessManager.BizList.Keys)
                        {
                            BusinessManager.UpdateBusProd(BusinessManager.BizList[b]);
                        }
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Refreshprod), 3000);
                        break;
                    case "ores":
                        Jobs.Miner.UpdateOres();
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.Refreshore), 3000);
                        break;
                    case "settings":
                    case "st":
                        Main.LoadServerSettings();
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы обновили настройки", 3000);
                        break;
                    case "roulette":
                        Repository.InitRoulette();
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы обновили рулетку", 3000);

                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"CMD_Refreshclothes Exception: {e.ToString()}");
            }
        }
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            LoadBarber();
            LoadTattoo();
            using (var db = new ConfigBD("ConfigDB"))
            {
                try
                {
                    (List<int>, string) TextureData;
                    //
                    ConcurrentDictionary<int, ClothesData> _ClothesBugsData = new ConcurrentDictionary<int, ClothesData>();
                    foreach (var item in db.ClothesBugs.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, false, ClothesComponent.Bugs);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesData.MaxSlots = item.MaxSlots;
                        _ClothesBugsData.TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Bugs", _ClothesBugsData);
                    ClothesBugsData = _ClothesBugsData;
                    Log.Write($"Load Bugs");

                    var clothesComponentData = new ConcurrentDictionary<bool, ConcurrentDictionary<ClothesComponent, ConcurrentDictionary<int, ClothesData>>>();
                    clothesComponentData.TryAdd(false, new ConcurrentDictionary<ClothesComponent, ConcurrentDictionary<int, ClothesData>>());
                    clothesComponentData.TryAdd(true, new ConcurrentDictionary<ClothesComponent, ConcurrentDictionary<int, ClothesData>>());


                    //
                    clothesComponentData[false].TryAdd(ClothesComponent.Masks, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesFemaleMasks.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, false, ClothesComponent.Masks);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.IsHair = item.IsHair;
                        clothesData.IsHat = item.IsHat;
                        clothesData.IsGlasses = item.IsGlasses;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesData.Gender = item.Gender;
                        clothesComponentData[false][ClothesComponent.Masks].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Female_Masks", clothesComponentData[false][ClothesComponent.Masks], IsHair: true, IsHat: true, IsGlasses: true);

                    clothesComponentData[true].TryAdd(ClothesComponent.Masks, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesMaleMasks.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, true, ClothesComponent.Masks);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.IsHair = item.IsHair;
                        clothesData.IsHat = item.IsHat;
                        clothesData.IsGlasses = item.IsGlasses;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesData.Gender = item.Gender;
                        clothesComponentData[true][ClothesComponent.Masks].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Male_Masks", clothesComponentData[true][ClothesComponent.Masks], IsHair: true, IsHat: true, IsGlasses: true);
                    Log.Write($"Load Masks");

                    //  










         
                    
                    
                    

                    //
                    clothesComponentData[false].TryAdd(ClothesComponent.Hat, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesFemaleHats.ToList())
                    {
                        var clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, false, ClothesComponent.Hat);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesData.IsHair = item.CleanHair;
                        clothesComponentData[false][ClothesComponent.Hat].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Female_Hats", clothesComponentData[false][ClothesComponent.Hat], IsHair: true);

                    clothesComponentData[true].TryAdd(ClothesComponent.Hat, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesMaleHats.ToList())
                    {
                        var clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, true, ClothesComponent.Hat);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesData.IsHair = item.CleanHair;
                        clothesComponentData[true][ClothesComponent.Hat].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Male_Hats", clothesComponentData[true][ClothesComponent.Hat], IsHair: true);
                    Log.Write($"Load Hats");

                    //
                    clothesComponentData[false].TryAdd(ClothesComponent.Accessories, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesFemaleAccessories.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, false, ClothesComponent.Accessories);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesComponentData[false][ClothesComponent.Accessories].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Female_Accessories", clothesComponentData[false][ClothesComponent.Accessories]);

                    clothesComponentData[true].TryAdd(ClothesComponent.Accessories, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesMaleAccessories.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, true, ClothesComponent.Accessories);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesComponentData[true][ClothesComponent.Accessories].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Male_Accessories", clothesComponentData[true][ClothesComponent.Accessories]);
                    Log.Write($"Load Accessories");

                    //
                    clothesComponentData[false].TryAdd(ClothesComponent.Ears, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesFemaleEars.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, false, ClothesComponent.Ears);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesComponentData[false][ClothesComponent.Ears].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Female_Ears", clothesComponentData[false][ClothesComponent.Ears]);

                    clothesComponentData[true].TryAdd(ClothesComponent.Ears, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesMaleEars.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, true, ClothesComponent.Ears);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesComponentData[true][ClothesComponent.Ears].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Male_Ears", clothesComponentData[true][ClothesComponent.Ears]);
                    Log.Write($"Load Ears");

                    //
                    clothesComponentData[false].TryAdd(ClothesComponent.Glasses, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesFemaleGlasses.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, false, ClothesComponent.Glasses);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesComponentData[false][ClothesComponent.Glasses].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Female_Glasses", clothesComponentData[false][ClothesComponent.Glasses]);

                    clothesComponentData[true].TryAdd(ClothesComponent.Glasses, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesMaleGlasses.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, true, ClothesComponent.Glasses);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesComponentData[true][ClothesComponent.Glasses].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Male_Glasses", clothesComponentData[true][ClothesComponent.Glasses]);
                    Log.Write($"Load Glasses");

                    //
                    clothesComponentData[false].TryAdd(ClothesComponent.Legs, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesFemaleLegs.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, false, ClothesComponent.Legs);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesComponentData[false][ClothesComponent.Legs].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Female_Legs", clothesComponentData[false][ClothesComponent.Legs]);

                    clothesComponentData[true].TryAdd(ClothesComponent.Legs, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesMaleLegs.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, true, ClothesComponent.Legs);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesComponentData[true][ClothesComponent.Legs].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Male_Legs", clothesComponentData[true][ClothesComponent.Legs]);
                    Log.Write($"Load Legs");

                    //
                    clothesComponentData[false].TryAdd(ClothesComponent.Shoes, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesFemaleShoes.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, false, ClothesComponent.Shoes);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesComponentData[false][ClothesComponent.Shoes].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Female_Shoes", clothesComponentData[false][ClothesComponent.Shoes]);

                    clothesComponentData[true].TryAdd(ClothesComponent.Shoes, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesMaleShoes.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, true, ClothesComponent.Shoes);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesComponentData[true][ClothesComponent.Shoes].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Male_Shoes", clothesComponentData[true][ClothesComponent.Shoes]);
                    Log.Write($"Load Shoes");

                    //
                    clothesComponentData[false].TryAdd(ClothesComponent.Tops, new ConcurrentDictionary<int, ClothesData>());
                    clothesComponentData[false].TryAdd(ClothesComponent.Undershort, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesFemaleTops.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, false, ClothesComponent.Tops);
                        clothesData.Torso = item.Torso;
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Similar = item.Similar;
                        clothesData.Type = item.Type;
                        clothesData.Undershirt = item.Undershirt;
                        clothesData.UndershirtButtoned = item.UndershirtButtoned;
                        clothesData.UndershirtTorso = item.UndershirtTorso;
                        clothesData.UndershirtButtonedTorso = item.UndershirtButtonedTorso;
                        clothesData.IsClearLegs = item.IsClearLegs;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesComponentData[false][ClothesComponent.Tops].TryAdd(item.Id, clothesData);
                        
                        if (item.Type == -1)
                            clothesComponentData[false][ClothesComponent.Undershort].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Female_Tops", clothesComponentData[false][ClothesComponent.Tops], isTorso: true, type: (int)ClothesComponent.Tops);
                    OnSaveJsonClothes("Female_Undershort", clothesComponentData[false][ClothesComponent.Tops], isTorso: true, type: (int)ClothesComponent.Undershort);

                    clothesComponentData[true].TryAdd(ClothesComponent.Tops, new ConcurrentDictionary<int, ClothesData>());
                    clothesComponentData[true].TryAdd(ClothesComponent.Undershort, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesMaleTops.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, true, ClothesComponent.Tops);
                        clothesData.Torso = item.Torso;
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Similar = item.Similar;
                        clothesData.Type = item.Type;
                        clothesData.Undershirt = item.Undershirt;
                        clothesData.UndershirtButtoned = item.UndershirtButtoned;
                        clothesData.UndershirtTorso = item.UndershirtTorso;
                        clothesData.UndershirtButtonedTorso = item.UndershirtButtonedTorso;
                        clothesData.IsClearLegs = item.IsClearLegs;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesComponentData[true][ClothesComponent.Tops].TryAdd(item.Id, clothesData);
                        
                        if (item.Type == -1)
                            clothesComponentData[true][ClothesComponent.Undershort].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Male_Tops", clothesComponentData[true][ClothesComponent.Tops], isTorso: true, type: (int)ClothesComponent.Tops);
                    OnSaveJsonClothes("Male_Undershort", clothesComponentData[true][ClothesComponent.Tops], isTorso: true, type: (int)ClothesComponent.Undershort);
                    Log.Write($"Load Tops");

                    //
                    clothesComponentData[false].TryAdd(ClothesComponent.Watches, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesFemaleWatches.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, false, ClothesComponent.Watches);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesComponentData[false][ClothesComponent.Watches].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Female_Watches", clothesComponentData[false][ClothesComponent.Watches]);

                    clothesComponentData[true].TryAdd(ClothesComponent.Watches, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesMaleWatches.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, true, ClothesComponent.Watches);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesComponentData[true][ClothesComponent.Watches].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Male_Watches", clothesComponentData[true][ClothesComponent.Watches]);
                    Log.Write($"Load Watches");

                    //
                    clothesComponentData[false].TryAdd(ClothesComponent.Bracelets, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesFemaleBracelets.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, false, ClothesComponent.Bracelets);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesComponentData[false][ClothesComponent.Bracelets].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Female_Bracelets", clothesComponentData[false][ClothesComponent.Bracelets]);

                    clothesComponentData[true].TryAdd(ClothesComponent.Bracelets, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesMaleBracelets.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, true, ClothesComponent.Bracelets);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesComponentData[true][ClothesComponent.Bracelets].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Male_Bracelets", clothesComponentData[true][ClothesComponent.Bracelets]);

                    Log.Write($"Load Bracelets");
                    //

                    clothesComponentData[false].TryAdd(ClothesComponent.Torsos, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesFemaleTorsos.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, false, ClothesComponent.Torsos);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesData.Torsos = JsonConvert.DeserializeObject<Dictionary<int, int>>(item.Torso);
                        clothesComponentData[false][ClothesComponent.Torsos].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Female_Torsos", clothesComponentData[false][ClothesComponent.Torsos], isTorsos: true);

                    clothesComponentData[true].TryAdd(ClothesComponent.Torsos, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesMaleTorsos.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, true, ClothesComponent.Torsos);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesData.Torsos = JsonConvert.DeserializeObject<Dictionary<int, int>>(item.Torso);
                        clothesComponentData[true][ClothesComponent.Torsos].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Male_Torsos", clothesComponentData[true][ClothesComponent.Torsos], isTorsos: true);

                    Log.Write($"Load Torsos");

                    //

                    clothesComponentData[false].TryAdd(ClothesComponent.BodyArmors, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesFemaleBodyarmors.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, false, ClothesComponent.BodyArmors);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesComponentData[false][ClothesComponent.BodyArmors].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Female_BodyArmors", clothesComponentData[false][ClothesComponent.BodyArmors]);

                    clothesComponentData[true].TryAdd(ClothesComponent.BodyArmors, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesMaleBodyarmors.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, true, ClothesComponent.BodyArmors);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesComponentData[true][ClothesComponent.BodyArmors].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Male_BodyArmors", clothesComponentData[true][ClothesComponent.BodyArmors]);

                    Log.Write($"Load BodyArmors");
                    
                    
                    //

                    clothesComponentData[false].TryAdd(ClothesComponent.Decals, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesFemaleDecals.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, false, ClothesComponent.Decals);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesComponentData[false][ClothesComponent.Decals].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Female_Decals", clothesComponentData[false][ClothesComponent.BodyArmors]);

                    clothesComponentData[true].TryAdd(ClothesComponent.Decals, new ConcurrentDictionary<int, ClothesData>());
                    foreach (var item in db.ClothesMaleDecals.ToList())
                    {
                        ClothesData clothesData = new ClothesData();
                        clothesData.Variation = GetRealVariation(item.Variation, item.Cvariation, true, ClothesComponent.Decals);
                        TextureData = LoadTextures(item.Textures);
                        clothesData.Textures = TextureData.Item1;
                        clothesData.TName = TextureData.Item2;
                        clothesData.Price = item.Price;
                        clothesData.Donate = item.Donate;
                        clothesComponentData[true][ClothesComponent.Decals].TryAdd(item.Id, clothesData);
                    }
                    OnSaveJsonClothes("Male_Decals", clothesComponentData[true][ClothesComponent.Decals]);

                    Log.Write($"Load Decals");
                    
                    clothesComponentData[true][ClothesComponent.Bugs] = ClothesBugsData;
                    clothesComponentData[false][ClothesComponent.Bugs] = ClothesBugsData;
                    
                    ClothesComponentPriceData = GetPrice(clothesComponentData, false);
                    ClothesComponentDonateData = GetPrice(clothesComponentData, true);
                    
                    //
                    ClothesComponentData = clothesComponentData;
                    Log.Write($"Load Clothes");
                    
                    
                    //

                    foreach (var availableSet in Fractions.Models.FractionClothingSetsData.AvailableSets)
                    {
                        if (availableSet.Component == ClothesComponent.Tops)
                        {
                            var gender = Convert.ToBoolean(availableSet.Gender);
                            if (clothesComponentData[gender][ClothesComponent.Tops].ContainsKey(availableSet.Variation) && clothesComponentData[gender][ClothesComponent.Tops][availableSet.Variation].Type == -1)
                            {
                                Console.WriteLine($"{availableSet.Variation}: " + JsonConvert.SerializeObject(availableSet));
                            }
                        }
                    }
                }
                catch
                {
                    Log.Write($"No Connect To Main Config");
                }
            }
        }

        public static ConcurrentDictionary<bool, Dictionary<string, List<List<object>>>> GetPrice(ConcurrentDictionary<bool, ConcurrentDictionary<ClothesComponent, ConcurrentDictionary<int, ClothesData>>> clothesComponentData, bool isDonate)
        {
            var clothesComponentPriceData = new ConcurrentDictionary<bool, Dictionary<string, List<List<object>>>>();

            foreach (var componentsGender in clothesComponentData)
            {
                var componentPriceData = new Dictionary<string, List<List<object>>>();
                var componentDonateData = new Dictionary<string, List<List<object>>>();

                foreach (var componentName in componentsGender.Value)
                {
                    var clothesPriceData = new List<List<object>>();
                    var clothesDonateData = new List<List<object>>();
                    
                    foreach (var clothesData in componentName.Value)
                    {
                        if (clothesData.Value.Price == 0 && clothesData.Value.Donate == 0)
                            continue;
                        
                        if (clothesData.Value.Price == 0 && !isDonate)
                            continue;
                        
                        if (clothesData.Value.Donate == 0 && isDonate)
                            continue;


                        var price = clothesData.Value.Price > 0
                            ? clothesData.Value.Price
                            : clothesData.Value.Donate;
                        
                        clothesPriceData.Add(new List<object>
                        {
                            clothesData.Key,
                            price
                        });
                        
                    }

                    if (!componentPriceData.ContainsKey(componentName.Key.ToString()))
                        componentPriceData.Add(componentName.Key.ToString(), new List<List<object>>());
                    
                    componentPriceData[componentName.Key.ToString()] = clothesPriceData;
                    
                    //
                    
                    if (!componentDonateData.ContainsKey(componentName.Key.ToString()))
                        componentDonateData.Add(componentName.Key.ToString(), new List<List<object>>());

                    componentDonateData[componentName.Key.ToString()] = clothesDonateData;
                }

                clothesComponentPriceData[componentsGender.Key] = componentPriceData;
            }

            return clothesComponentPriceData;
        }
        
        public (List<int>, string) LoadTextures(string list)
        {

            try
            {
                List<List<string>> jsonObject = JsonConvert.DeserializeObject<List<List<string>>>(list);
                List<int> _NewData = new List<int>();
                string newTexture = "";
                if (jsonObject != null && jsonObject.Count > 0)
                {
                    for (int i = 0; i < jsonObject.Count; i++)
                    {
                        if (jsonObject[i].Count == 2)
                        {
                            if (jsonObject[i][1].ToString().Length == 0 || (jsonObject[i][1].ToString().Length != 0 && jsonObject[i][1].ToString() != "NO_LABEL"))
                                _NewData.Add(Convert.ToInt32(jsonObject[i][0]));
                            if (newTexture.Length == 0 && jsonObject[i][1].ToString().Length != 0 && jsonObject[i][1].ToString() != "NO_LABEL")
                            {
                                newTexture = jsonObject[i][1].ToString();
                                newTexture = newTexture.Substring(0, newTexture.Length - 1);
                            }
                        }
                    }

                }
                return (_NewData, newTexture);
            }
            catch (Exception e)
            {
                Log.Write($"LoadTextures({list}) Exception: {e.ToString()}");
                return (new List<int>(), "null");
            }
        }
        /// <summary>
        /// Все эти json идут на интерфейс в папку json
        /// </summary>
        /// <param name="name"></param>
        /// <param name="clothesData"></param>
        /// <param name="IsHair"></param>
        /// <param name="IsHat"></param>
        /// <param name="IsGlasses"></param>
        /// <param name="isTorso"></param>
        private void OnSaveJsonClothes(string name, ConcurrentDictionary<int, ClothesData> clothesData, bool IsHair = false, bool IsHat = false, bool IsGlasses = false, bool isTorso = false, int type = -1, bool isTorsos = false)
        {
            try
            {
                var saveData = new Dictionary<int, Dictionary<string, object>>();

                foreach (var clothes in clothesData)
                {
                    if (type == (int)ClothesComponent.Tops && clothes.Value.Type == -1) continue;
                    else if (type == (int)ClothesComponent.Undershort && clothes.Value.Type != -1) continue;
                    //else if (clothes.Value.Textures.Count == 0) continue;
                    var data = new Dictionary<string, object>();
                    data.Add("Id", clothes.Key);
                    data.Add("Variation", clothes.Value.Variation);
                    data.Add("TName", clothes.Value.TName);
                    data.Add("Textures", clothes.Value.Textures);

                    if (IsHair)
                        data.Add("IsHair", clothes.Value.IsHair);
                    if (IsHat)
                        data.Add("IsHat", clothes.Value.IsHat);
                    if (IsGlasses)
                        data.Add("IsGlasses", clothes.Value.IsGlasses);
                    if (isTorso)
                        data.Add("Torso", clothes.Value.Torso);
                    if (isTorsos)
                        data.Add("Torsos", clothes.Value.Torsos);


                    //data.Add("Price", clothes.Value.Price);
                    //data.Add("Donate", clothes.Value.Donate);

                    saveData.Add(clothes.Key, data);
                }

                File.WriteAllText(@$"json/clothes_{name}.json", string.Empty);
                using (var saveCoords = new StreamWriter(@$"json/clothes_{name}.json", true, Encoding.UTF8))
                {
                    saveCoords.Write("{\n");
                    int index = 0;
                    foreach (var clothes in saveData)
                    {
                        index++;

                        if (saveData.Count == index)
                            saveCoords.Write($"\t\"{clothes.Key}\": {JsonConvert.SerializeObject(clothes.Value)}\r\n");
                        else
                            saveCoords.Write($"\t\"{clothes.Key}\": {JsonConvert.SerializeObject(clothes.Value)},\r\n");

                        //saveData.Add(clothes.Key, data);
                    }
                    saveCoords.Write("}");
                    saveCoords.Close();
                }
            }
            catch
            {
                Log.Write($"OnSaveJsonClothes");
            }
        }

        /// <summary>
        /// Проверка на вернюю одежду
        /// </summary>
        /// <param name="Cloth"></param>
        /// <returns></returns>
        public static bool IsTopUp(ClothesData Cloth)
        {
            return
            (
                !(Cloth.Type == -1) // Или если же это и не жакет или батонед, и у этой одежды нет undershirt`а, то это верх.
            );
        }
        /// <summary>
        /// Проверка на нижнюю одежду
        /// </summary>
        /// <param name="cloth"></param>
        /// <returns></returns>
        public static bool IsTopDown(ClothesData cloth)
        {
            return !IsTopUp(cloth);
        }
        
        public static IReadOnlyDictionary<bool, Dictionary<ClothesComponent, int>> MaxClothesComponent = new Dictionary<bool, Dictionary<ClothesComponent, int>>() 
        { 
            { true, new Dictionary<ClothesComponent, int>() 
                {//Man 
                    { ClothesComponent.Hat, 186 }, //+
                    { ClothesComponent.Torsos, 197 }, 
                    { ClothesComponent.Legs, 159 },  //+
                    { ClothesComponent.Shoes, 125 }, //+
                    { ClothesComponent.Accessories, 166 }, //+
                    { ClothesComponent.BodyArmors, 60 }, 
                    { ClothesComponent.Tops, 441 }, // +
                    { ClothesComponent.Undershirts, 188 }, 
                    { ClothesComponent.Masks, 215 },//+
                    { ClothesComponent.Ears, 42 }, 
                    { ClothesComponent.Watches, 57}, //+
                    { ClothesComponent.Glasses, 46 }, 

                    { ClothesComponent.Bugs,110 }, 

                    { ClothesComponent.Bracelets, 20 }, 
     
                    { ClothesComponent.Decals, 133 }, 
                } 
            }, 
            { false, new Dictionary<ClothesComponent, int>() 
                { 
                    { ClothesComponent.Hat, 185 }, //+
                    { ClothesComponent.Torsos, 243 }, 
                    { ClothesComponent.Legs, 168 },  //+
                    { ClothesComponent.Shoes, 129 }, //+
                    { ClothesComponent.Accessories, 135 }, //a
                    { ClothesComponent.BodyArmors, 58 }, 
                    { ClothesComponent.Tops, 472 }, //+
                    { ClothesComponent.Undershirts, 233 }, 
                    { ClothesComponent.Masks,  216}, //+
                    { ClothesComponent.Ears, 23 }, 
                    { ClothesComponent.Watches, 70 }, //+
                    { ClothesComponent.Glasses, 48 }, //+

                    { ClothesComponent.Bugs, 110 },     

                    { ClothesComponent.Bracelets, 29 }, 
     
                    { ClothesComponent.Decals, 153 }, 
                }
            },
        }; 
        public static int GetRealVariation(int Variation, int VariationCustom, bool gender, ClothesComponent ClothesComponent)
        {
            try
            {
                return Variation != -1 ? Variation : (MaxClothesComponent[gender][ClothesComponent] + VariationCustom);
            }
            catch (Exception e)
            {
                Log.Write($"GetRealVariation Exception: {e.ToString()}");
                return Variation;
            }
        }
        public static void SetHat(ExtPlayer player, bool gender)
        {
            try
            {
                var isCharacterData = player.IsCharacterData();
                
                var playerMask = GetItemData(player, "accessories", 1);
                var playerMaskData = playerMask.GetData();

                var hatData = ClothesComponentData[gender][ClothesComponent.Hat];
                var playerHat = GetItemData(player, "accessories", 0);
                var playerHatData = playerHat.GetData();

                var maskData = ClothesComponentData[gender][ClothesComponent.Masks];
                var isMask = playerMask.ItemId != ItemId.Debug && maskData.ContainsKey(playerMaskData["Variation"]);
                
                if ((isMask && maskData[playerMaskData["Variation"]].IsHair) || (playerHat.ItemId != ItemId.Debug && hatData.ContainsKey(playerHatData["Variation"]) && hatData[playerHatData["Variation"]].IsHair))
                {
                    SetClothes(player, 2, 0, 0);
                }
                else
                {
                    //int HairId = Customization.CustomPlayerData[characterData.UUID].Hair.Hair;
                    //List<cshai> TopsData = !gender ? CsTops.Female : CsTops.Male;
                    //int HairVariation = GetRealVariation(TopsData[HairId].Variation, TopsData[HairId].VariationCustom, gender, ClothesComponent.Hair);
                    if (isCharacterData)
                    {
                        var custom = player.GetCustomization();
                        if (custom != null)
                        {
                            var hairData = BarberComponentData[gender][BarberComponent.Hair];

                            if (hairData.ContainsKey(custom.Hair.Hair))
                                SetClothes(player, 2, hairData[custom.Hair.Hair].Variation, 0);
                            else
                                SetClothes(player, 2, custom.Hair.Hair, 0);
                        }
                    }
                }

                if ((isMask && maskData[playerMaskData["Variation"]].IsHat) || playerHat.ItemId == ItemId.Debug)
                {
                    ClearAccessory(player,0, isBlock: false);
                }
                else
                {
                    SetAccessories(player, 0, hatData[playerHatData["Variation"]].Variation, playerHatData["Texture"]);
                }

                var playerEars = GetItemData(player, "accessories", 2);
                var playerEarsData = playerEars.GetData();

                if ((isMask && maskData[playerMaskData["Variation"]].IsGlasses) || playerEars.ItemId == ItemId.Debug)
                {
                    ClearAccessory(player,2, isBlock: false);
                }
                else
                {
                    var earsData = ClothesComponentData[gender][ClothesComponent.Hat];
                    SetAccessories(player, 2, earsData[playerEarsData["Variation"]].Variation, playerEarsData["Texture"]);
                }

                var playerGlasses = GetItemData(player, "accessories", 3);
                var playerGlassesData = playerGlasses.GetData();

                if ((isMask && maskData[playerMaskData["Variation"]].IsGlasses) || playerGlasses.ItemId == ItemId.Debug)
                {
                    ClearAccessory(player,1, isBlock: false);
                }
                else
                {
                    var glassesData = ClothesComponentData[gender][ClothesComponent.Glasses];
                    SetAccessories(player, 1, glassesData[playerGlassesData["Variation"]].Variation, playerGlassesData["Texture"]);
                }

                if (!isMask || playerMask.ItemId == ItemId.Debug)
                {
                    SetClothes(player, 1, 0, 0);
                    if (isCharacterData)
                    {
                        player.SetSharedData("IS_MASK", false);
                        player.Eval("mp.game.graphics.setNightvision(false);");
                    }
                }
                else
                {
                    var maskVariation = maskData[playerMaskData["Variation"]].Variation;
                    SetClothes(player, 1, maskVariation, playerMaskData["Texture"]);
                    if (isCharacterData)
                    {
                        int Variation = -1; 
                        if (playerMask.ItemId == ItemId.Mask) Variation = Convert.ToInt32(playerMask.Data.Split('_')[0]); 
 
                        var MaskData = Chars.ClothesComponents.ClothesComponentData[gender][Chars.ClothesComponent.Masks]; 
                        
                        if (Variation != -1 && MaskData.ContainsKey(Variation) && MaskData[Variation].Donate > 0 || Repository.IsBeard(gender, playerMask)) 
                            player.SetSharedData("IS_MASK", true);
                        else 
                            player.SetSharedData("IS_MASK", false);
                        //
                        if (playerMaskData["Variation"] == 132) 
                            player.Eval("mp.game.graphics.setNightvision(true);");
                        else 
                            player.Eval("mp.game.graphics.setNightvision(false);");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"SetHat Exception: {e.ToString()}");
            }
        }

        /// <summary>
        /// Установка верхней нижней одежды и рук
        /// </summary>
        /// <param name="player"></param>
        public static void SetTop(ExtPlayer player, bool gender)
        {
            try
            {
                var topsData = ClothesComponentData[gender][ClothesComponent.Tops];

                var playerTopUp = GetItemData(player, "accessories", 5);
                var playerTopUpData = playerTopUp.GetData();
                
                var playerTopDown = GetItemData(player, "accessories", 6);
                var playerTopDownData = playerTopDown.GetData();

                var playerTorsos = GetItemData(player, "accessories", 12);
                var playerTorsosData = playerTorsos.GetData();

                var topVariation = Customization.EmtptySlots[gender][11];
                var undershirtsVariation = -1;

                var torsosId = -1;

                var isClearLegs = false;

                //Сначала одеваем нижнюю одежду
                if (playerTopDown.ItemId != ItemId.Debug && topsData.ContainsKey(playerTopDownData["Variation"]) && IsTopDown(topsData[playerTopDownData["Variation"]]))
                {
                    undershirtsVariation = topsData[playerTopDownData["Variation"]].Variation;

                    //if (!TopsData[PlayerTopDownData["Variation"]].Textures.Contains(PlayerTopDownData["Texture"])) PlayerTopDownData["Texture"] = 0;
                    // Проверяем, если это нижняя одежда
                    if (playerTopUp.ItemId == ItemId.Debug)
                    {
                        // Проверяем, если на человеке нет верхней одежды, то удаляем undershirt и ставим "нижнюю" как верхнюю.

                        topVariation = topsData[playerTopDownData["Variation"]].Variation;
                        isClearLegs = topsData[playerTopDownData["Variation"]].IsClearLegs;
                        playerTopUpData["Texture"] = playerTopDownData["Texture"];

                        torsosId = playerTopDownData["Variation"];

                        // Удаляем undershirt
                        undershirtsVariation = -1;
                    }
                }

                //Одеваем верхнюю


                var typeUndershirt = 0;
                if (playerTopUp.ItemId != ItemId.Debug && topsData.ContainsKey(playerTopUpData["Variation"]) && IsTopUp(topsData[playerTopUpData["Variation"]]))
                {
                    topVariation = topsData[playerTopUpData["Variation"]].Variation;
                    isClearLegs = topsData[playerTopUpData["Variation"]].IsClearLegs;

                    //if (!TopsData[PlayerTopUsessionData["Variation"]].Textures.Contains(PlayerTopUsessionData["Texture"])) PlayerTopUsessionData["Texture"] = 0;

                    var typeClothes = topsData[playerTopUpData["Variation"]].Type;
                    // Проверяем, если текущий верх отсутствует, то мы просто надеваем на человека верхнюю одежду
                    if (playerTopDown.ItemId == ItemId.Debug || typeClothes == 0) undershirtsVariation = -1;
                    else if (typeClothes == 1)
                    {
                        undershirtsVariation = topsData[playerTopDownData["Variation"]].Variation != -1
                            ? topsData[playerTopDownData["Variation"]].Undershirt
                            : (MaxClothesComponent[gender][ClothesComponent.Undershirts] +
                               topsData[playerTopDownData["Variation"]].Undershirt);
                        
                        typeUndershirt = 1;
                    }
                    else if (typeClothes == 2)
                    {
                        undershirtsVariation = topsData[playerTopDownData["Variation"]].Variation != -1
                            ? topsData[playerTopDownData["Variation"]].UndershirtButtoned
                            : (MaxClothesComponent[gender][ClothesComponent.Undershirts] +
                               topsData[playerTopDownData["Variation"]].UndershirtButtoned);
                        
                        typeUndershirt = 2;
                    }

                    torsosId = playerTopUpData["Variation"];
                }

                if (playerTopUp.ItemId == ItemId.Debug && playerTopDown.ItemId == ItemId.Debug)
                {
                    undershirtsVariation = -1;
                    topVariation = Customization.EmtptySlots[gender][11];
                    isClearLegs = false;
                    playerTopUpData["Texture"] = 0;
                }

                if (undershirtsVariation == -1)
                {
                    undershirtsVariation = Customization.EmtptySlots[gender][8];
                    playerTopDownData["Texture"] = 0;
                }

                SetClothes(player, 8, undershirtsVariation, playerTopDownData["Texture"]);
                SetClothes(player, 11, topVariation, playerTopUpData["Texture"]);

                if (torsosId == -1) 
                    torsosId = 15;


                var topData = topsData[torsosId];
                
                var torsosVariation = topData.Torso;
                if (typeUndershirt == 1 && topData.UndershirtTorso > 0)
                    torsosVariation = topData.UndershirtTorso;
                if (typeUndershirt == 2 && topData.UndershirtButtonedTorso > 0)
                    torsosVariation = topData.UndershirtButtonedTorso;
                
                
                var torsosData = ClothesComponentData[gender][ClothesComponent.Torsos];
                if (playerTorsos.ItemId == ItemId.Debug || playerTorsosData["Variation"] == -1 || !torsosData.ContainsKey(playerTorsosData["Variation"]) || !torsosData[playerTorsosData["Variation"]].Torsos.ContainsKey(torsosVariation))
                    SetClothes(player, 3, torsosVariation, 0);
                else
                    SetClothes(player, 3, torsosData[playerTorsosData["Variation"]].Torsos[torsosVariation], playerTorsosData["Texture"]);
                
                
                //
                
                var playerLeg = GetItemData(player, "accessories", 9);
                var playerLegData = playerLeg.GetData();
                var legVariation = playerLegData["Variation"];
                
                if (!isClearLegs)
                {
                    var legsData = ClothesComponentData[gender][ClothesComponent.Legs];
                    if (legsData.ContainsKey(legVariation))
                        legVariation = legsData[legVariation].Variation;
                    else
                    {
                        legVariation = Customization.EmtptySlots[gender][4];
                        playerLegData["Texture"] = 0;
                    }
                }
                else
                {
                    if (gender)
                        legVariation = 11;
                    else
                        legVariation = 13;
                    
                    playerLegData["Texture"] = 0;
                }

                SetClothes(player, 4, legVariation, playerLegData["Texture"]);
                
            }
            catch (Exception e)
            {
                Log.Write($"SetTop Exception: {e.ToString()}");
            }
        }

        public static void ClearAccessory(ExtPlayer player, int slot, bool isBlock = true)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            
            if (!characterData.Accessory.ContainsKey(slot))
                return;
            
            var component = Repository.ClothesComponentToPropId.Values
                .FirstOrDefault(c => c.SlotId == slot);
            
            //if (!isBlock && component != null && player.IsAccessories(component.AccessoriesSlotId))
            //    return;
            
            //if (isBlock && !characterData.AccessoryBlock.Contains(slot))
            //    characterData.AccessoryBlock.Add(slot);

            NAPI.Player.ClearPlayerAccessory(player, slot);
            characterData.Accessory.Remove(slot);
            
            if (component != null)
                player.DeleteAccessories(component.AccessoriesSlotId);
        }
        public static void SetSpecialAccessories(ExtPlayer player, int slot, int drawable, int texture)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;
            
                var isClothes = false;
                var clothes = new ComponentVariation();
                if (characterData.Accessory.ContainsKey(slot))
                {
                    isClothes = true;
                    clothes = characterData.Accessory[slot];
                }

                if (isClothes && clothes.Drawable == drawable && clothes.Texture == texture) 
                    return;

                characterData.Accessory[slot] = new ComponentVariation(drawable, texture);  
                NAPI.Player.SetPlayerAccessory(player, slot, drawable, texture); 
                
                var component = Repository.ClothesComponentToPropId.Values
                    .FirstOrDefault(c => c.SlotId == slot);
		
                if (component != null)
                {
                    var data = $"{drawable}_{texture}_{characterData.Gender}";

                    var item = new InventoryItemData(ItemId: component.ItemId, Data: data);

                    player.SetAccessories(component.AccessoriesSlotId, item);
                }
            }
            catch (Exception e)
            {
                Log.Write($"SetSpecialAccessories Exception: {e.ToString()}");
            }
        }
        public static void SetAccessories(ExtPlayer player, int slot, int drawable, int texture)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null)
                {
                    var uuid = sessionData.SelectUUID;
                    if (!sessionData.Accessory.ContainsKey(uuid))
                        sessionData.Accessory[uuid] = new Dictionary<int, ComponentVariation>();
                    
                    sessionData.Accessory[uuid][slot] = new ComponentVariation()
                    {
                        Drawable = drawable,
                        Texture = texture
                    };
                }
                else
                {
                    var isClothes = false;
                    var clothes = new ComponentVariation();
                    if (characterData.Accessory.ContainsKey(slot))
                    {
                        isClothes = true;
                        clothes = characterData.Accessory[slot];
                    }
                    
                    if (isClothes && clothes.Drawable == drawable && clothes.Texture == texture) 
                        return;
                    
                    characterData.Accessory[slot] = new ComponentVariation(drawable, texture);   
                    NAPI.Player.SetPlayerAccessory(player, slot, drawable, texture);
                }
            }
            catch (Exception e)
            {
                Log.Write($"SetAccessories Exception: {e.ToString()}");
            }
        }
        public static void SetSpecialClothes(ExtPlayer player, int slot, int drawable, int texture)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;
                
                var isClothes = false;
                var clothes = new ComponentVariation();
                if (characterData.Clothes.ContainsKey(slot))
                {
                    isClothes = true;
                    clothes = characterData.Clothes[slot];
                }
                
                if (isClothes && clothes.Drawable == drawable && clothes.Texture == texture) 
                    return;
                
                characterData.Clothes[slot] = new ComponentVariation(drawable, texture);

                var component = Repository.ClothesComponentToComponentId.Values
                    .FirstOrDefault(c => c.SlotId == slot);
                
                if (component != null)
                {
                    var data = $"{drawable}_{texture}_{characterData.Gender}";

                    var item = new InventoryItemData(ItemId: component.ItemId, Data: data);

                    player.SetAccessories(component.AccessoriesSlotId, item);
                }
            }
            catch (Exception e)
            {
                Log.Write($"SetSpecialClothes Exception: {e.ToString()}");
            }
        }
        public static void SetClothes(ExtPlayer player, int slot, int drawable, int texture)
        {
            try
            {
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null)
                {
                    var uuid = sessionData.SelectUUID;
                    if (!sessionData.Clothes.ContainsKey(uuid))
                        sessionData.Clothes[uuid] = new Dictionary<int, ComponentVariation>();
                    
                    sessionData.Clothes[uuid][slot] = new ComponentVariation()
                    {
                        Drawable = drawable,
                        Texture = texture
                    };
                }
                else
                {
                    var isClothes = false;
                    var clothes = new ComponentVariation();
                    if (characterData.Clothes.ContainsKey(slot))
                    {
                        isClothes = true;
                        clothes = characterData.Clothes[slot];
                    }

                    if (isClothes && clothes.Drawable == drawable && clothes.Texture == texture) 
                        return;

                    characterData.Clothes[slot] = new ComponentVariation(drawable, texture);
                }
            }
            catch (Exception e)
            {
                Log.Write($"SetSpecialClothes Exception: {e.ToString()}");
            }
        }

        public static void ClearClothes(ExtPlayer player, int slot, bool gender)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null)
                return;
            
            characterData.Clothes[slot] = new ComponentVariation(Customization.EmtptySlots[gender][slot], 0);
            //NAPI.Player.SetPlayerClothes(player, slot, Customization.EmtptySlots[gender][slot], 0);
            
            var component = Repository.ClothesComponentToComponentId.Values
                .FirstOrDefault(c => c.SlotId == slot);
                    
            if (component != null)
                player.DeleteAccessories(component.AccessoriesSlotId);
            //if (characterData.ClothesBlock.Contains(slot))
            //    characterData.ClothesBlock.Remove(slot);
        }

        public static void UpdateClothes(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                if (!characterData.IsSpawned) return;
                
                if (characterData.Clothes.Count == 0) return;
                
                qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage2, 1, isUpdateHud: true);
                qMain.UpdateQuestsComplete(player, Zdobich.QuestName, (int)zdobich_quests.Stage2, true);

                var useClothes = new Dictionary<int, ComponentVariation>();

                foreach (var clothes in characterData.Clothes)
                {
                    if (characterData.UsedClothes.ContainsKey(clothes.Key) && characterData.UsedClothes[clothes.Key].Drawable == clothes.Value.Drawable && characterData.UsedClothes[clothes.Key].Texture == clothes.Value.Texture)
                        continue;

                    useClothes[clothes.Key] = clothes.Value;
                    characterData.UsedClothes[clothes.Key] = clothes.Value;
                }
                
                if (useClothes.Count > 0)
                    NAPI.Player.SetPlayerClothes(player, useClothes);

                if (useClothes.ContainsKey(2))
                {
                    var custom = player.GetCustomization();
                    if (custom != null)
                    {
                        NAPI.Player.SetPlayerHairColor(player, (byte) custom.Hair.Color,
                            (byte) custom.Hair.HighlightColor);
                    }
                }

                if (useClothes.ContainsKey(1))
                {
                    switch (useClothes[1].Drawable)
                    {
                        case 32:
                        case 35:
                        case 37:
                        case 47:
                        case 48:
                        case 51:
                        case 52:
                        case 53:
                        case 54:
                        case 55:
                        case 56:
                        case 57:
                        case 58:
                        case 89:
                        case 102:
                        case 104:
                        case 111:
                        case 112:
                        case 113:
                        case 117:
                        case 118:
                        case 119:
                        case 123:
                        case 126:
                        case 130:
                        case 132:
                        case 146:
                        case 169:
                        case 170:
                        case 174:
                            //Customization.ApplyMaskFace(player);
                            break;
                    }

                }
                
                
                characterData.Clothes.Clear();
            }
            catch (Exception e)
            {
                Log.Write($"UpdateClothes Exception: {e.ToString()}");
            }
        }

        public static void ClearClothes(ExtPlayer player, bool gender)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                characterData.UsedClothes.Clear();
                
                for (var i = 0; i <= 8; i++) 
                    ClearAccessory(player, i);

                foreach (var slotid in Customization.EmtptySlots[gender].Keys)
                    ClearClothes(player, slotid, gender);
                
                /*ClearClothes(player, 3, gender);
                ClearClothes(player, 4, gender);
                ClearClothes(player, 6, gender);
                ClearClothes(player, 8, gender);
                ClearClothes(player, 11, gender);
                
                if (sessionData.HeadPocket)
                {
                    ClearClothes(player, 1, gender);
                }*/
            }
            catch (Exception e)
            {
                Log.Write($"ClearClothes Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Tsc)]
        public static void CMD_clothesEditor(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                else if (!CommandsAccess.CanUseCmd(player, AdminCommands.Tsc)) return;

                /*bool gender = true;
                Dictionary<int, List<List<object>>> _ShosessionData = new Dictionary<int, List<List<object>>>();
                Dictionary<int, ClothesData> _ShoesData = null;
                List<object> _ClothesData = null;
                foreach (int type in BusinessManager.ClothesShopToType.ToList())
                {
                    _ShosessionData.Add(type, new List<List<object>>());
                    if (type == (int)ClothesComponent.Undershort) _ShoesData = ClothesComponentData[gender][ClothesComponent.Tops];
                    else _ShoesData = ClothesComponentData[gender][(ClothesComponent)type];
                    foreach (KeyValuePair<int, ClothesData> clData in _ShoesData.ToList())
                    {
                        if (type == (int)ClothesComponent.Tops && clData.Value.Type == -1) continue;
                        else if (type == (int)ClothesComponent.Undershort && clData.Value.Type != -1) continue;
                        _ClothesData = new List<object>();
                        _ClothesData.Add(clData.Key);//0
                        _ClothesData.Add(clData.Value.Variation);//1
                        _ClothesData.Add(clData.Value.Torso);//2
                        _ClothesData.Add(clData.Value.TName);//3
                        _ClothesData.Add(clData.Value.Textures);//4
                        _ClothesData.Add(clData.Value.Price);//5
                        _ClothesData.Add(clData.Value.Donate);//6
                        _ShosessionData[type].Add(_ClothesData);
                    }
                }
                Trigger.ClientEvent(player, "client.clothesEditor.data", 1, JsonConvert.SerializeObject(_ShosessionData));

                gender = false;
                _ShosessionData = new Dictionary<int, List<List<object>>>();
                _ShoesData = null;
                _ClothesData = null;
                foreach (int type in BusinessManager.ClothesShopToType.ToList())
                {
                    _ShosessionData.Add(type, new List<List<object>>());
                    if (type == (int)ClothesComponent.Undershort) _ShoesData = ClothesComponentData[gender][ClothesComponent.Tops];
                    else _ShoesData = ClothesComponentData[gender][(ClothesComponent)type];
                    foreach (KeyValuePair<int, Chars.ClothesData> clData in _ShoesData.ToList())
                    {
                        if (type == (int)ClothesComponent.Tops && clData.Value.Type == -1) continue;
                        else if (type == (int)ClothesComponent.Undershort && clData.Value.Type != -1) continue;
                        _ClothesData = new List<object>();
                        _ClothesData.Add(clData.Key);//0
                        _ClothesData.Add(clData.Value.Variation);//1
                        _ClothesData.Add(clData.Value.Torso);//2
                        _ClothesData.Add(clData.Value.TName);//3
                        _ClothesData.Add(clData.Value.Textures);//4
                        _ClothesData.Add(clData.Value.Price);//5
                        _ClothesData.Add(clData.Value.Donate);//6
                        _ShosessionData[type].Add(_ClothesData);
                    }
                }
                Trigger.ClientEvent(player, "client.clothesEditor.data", 0, JsonConvert.SerializeObject(_ShosessionData));*/

                Trigger.ClientEvent(player, "client.clothesEditor.open");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_clothesEditor Exception: {e.ToString()}");
            }

        }
        [RemoteEvent("server.clothesEditor.close")]
        public static void clothesEditorClose(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Customization.ApplyCharacter(player);
            }
            catch (Exception e)
            {
                Log.Write($"clothesEditorClose Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.clothes.update")]
        public static void clothesUpdate(ExtPlayer player, int slot)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                bool gender = characterData.Gender;
                ConcurrentDictionary<int, ClothesData> TopsData = ClothesComponentData[gender][ClothesComponent.Tops];
                switch (slot)
                {
                    case 5:
                        var PlayerTopUp = GetItemData(player, "accessories", 5);
                        Dictionary<string, int> PlayerTopUsessionData = PlayerTopUp.GetData();
                        bool PlayerTopUsessionDataGender = PlayerTopUp.GetGender();

                        if (TopsData.ContainsKey(PlayerTopUsessionData["Variation"]) && TopsData[PlayerTopUsessionData["Variation"]].Similar > 0)
                        {
                            PlayerTopUp.Data = $"{TopsData[PlayerTopUsessionData["Variation"]].Similar}_{PlayerTopUsessionData["Texture"]}_{PlayerTopUsessionDataGender}";
                            Repository.SetItemData(player, "accessories", 5, PlayerTopUp, true);
                        }
                        break;
                    case 6:
                        var PlayerTopDown = GetItemData(player, "accessories", 6);
                        Dictionary<string, int> PlayerTopDownData = PlayerTopDown.GetData();
                        bool PlayerTopDownGender = PlayerTopDown.GetGender();

                        if (TopsData.ContainsKey(PlayerTopDownData["Variation"]) && TopsData[PlayerTopDownData["Variation"]].Similar > 0)
                        {
                            PlayerTopDown.Data = $"{TopsData[PlayerTopDownData["Variation"]].Similar}_{PlayerTopDownData["Texture"]}_{PlayerTopDownGender}";
                            Repository.SetItemData(player, "accessories", 6, PlayerTopDown, true);
                        }
                        break;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"clothesEditorClose Exception: {e.ToString()}");
            }
        }
        ///

        public enum BarberComponent
        {
            Hair = 0,
            Beard,
            Body,
            Eyebrows,
            Eyes,
            Lips,
            Makeup,
            Palette
        }

        public static IReadOnlyDictionary<bool, Dictionary<BarberComponent, int>> MaxBarberComponent = new Dictionary<bool, Dictionary<BarberComponent, int>>() 
        { 
            { true, new Dictionary<BarberComponent, int>() 
                {//Man 
                    { BarberComponent.Hair, 78 }, 
                    { BarberComponent.Beard, 183 }, 
                    { BarberComponent.Body, 132 }, 
                    { BarberComponent.Eyebrows, 97 }, 
                    { BarberComponent.Eyes, 150 }, 
                    { BarberComponent.Lips, 55 }, 
                    { BarberComponent.Makeup, 361 }, 
                    { BarberComponent.Palette, 177 }, 
                } 
            }, 
            { false, new Dictionary<BarberComponent, int>() 
                { 
                    { BarberComponent.Hair, 83 }, 
                    { BarberComponent.Beard, 183 }, 
                    { BarberComponent.Body, 132 }, 
                    { BarberComponent.Eyebrows, 97 }, 
                    { BarberComponent.Eyes, 150 }, 
                    { BarberComponent.Lips, 55 }, 
                    { BarberComponent.Makeup, 361 }, 
                    { BarberComponent.Palette, 177 }, 
                }
            }
        };
        public static ConcurrentDictionary<bool, ConcurrentDictionary<BarberComponent, ConcurrentDictionary<int, BarberData>>> BarberComponentData = new ConcurrentDictionary<bool, ConcurrentDictionary<BarberComponent, ConcurrentDictionary<int, BarberData>>>();
        public static ConcurrentDictionary<bool, Dictionary<string, List<List<object>>>> BarberComponentPriceData =
            new ConcurrentDictionary<bool, Dictionary<string, List<List<object>>>>();
        
        public static ConcurrentDictionary<bool, Dictionary<string, List<List<object>>>> BarberComponentDonateData =
            new ConcurrentDictionary<bool, Dictionary<string, List<List<object>>>>();
        
        public static int GetRealBarberVariation(int Variation, int VariationCustom, bool gender, BarberComponent BarberComponent)
        {
            try
            {
                return Variation != -1 ? Variation : (MaxBarberComponent[gender][BarberComponent] + VariationCustom);

            }
            catch (Exception e)
            {
                Log.Write($"GetRealVariation Exception: {e.ToString()}");
                return Variation;
            }
        }
        private void LoadBarber()
        {
            using (var db = new ConfigBD("ConfigDB"))
            {
                var barberComponentData = new ConcurrentDictionary<bool, ConcurrentDictionary<BarberComponent, ConcurrentDictionary<int, BarberData>>>();
                barberComponentData.TryAdd(false, new ConcurrentDictionary<BarberComponent, ConcurrentDictionary<int, BarberData>>());
                barberComponentData.TryAdd(true, new ConcurrentDictionary<BarberComponent, ConcurrentDictionary<int, BarberData>>());

                barberComponentData[false].TryAdd(BarberComponent.Hair, new ConcurrentDictionary<int, BarberData>());
                foreach (var item in db.BarberFemaleHair.ToList())
                {
                    var clothesData = new BarberData();
                    clothesData.Variation = GetRealBarberVariation(item.Variation, item.Cvariation, false, BarberComponent.Hair);
                    clothesData.Name = item.Name;
                    clothesData.TName = item.Tname;
                    clothesData.Price = item.Price;
                    clothesData.Donate = item.Donate;
                    barberComponentData[false][BarberComponent.Hair].TryAdd(item.Id, clothesData);
                }
                OnSaveJsonBarber("Female_Hair", barberComponentData[false][BarberComponent.Hair]);

                barberComponentData[true].TryAdd(BarberComponent.Hair, new ConcurrentDictionary<int, BarberData>());
                foreach (var item in db.BarberMaleHair.ToList())
                {
                    var clothesData = new BarberData();
                    clothesData.Variation = GetRealBarberVariation(item.Variation, item.Cvariation, true, BarberComponent.Hair);
                    clothesData.Name = item.Name;
                    clothesData.TName = item.Tname;
                    clothesData.Price = item.Price;
                    clothesData.Donate = item.Donate;
                    barberComponentData[true][BarberComponent.Hair].TryAdd(item.Id, clothesData);
                }
                OnSaveJsonBarber("Male_Hair", barberComponentData[true][BarberComponent.Hair]);
                Log.Write($"Load Hair");

                //

                barberComponentData[false].TryAdd(BarberComponent.Beard, new ConcurrentDictionary<int, BarberData>());
                foreach (var item in db.BarberFemaleBeard.ToList())
                {
                    var clothesData = new BarberData();
                    clothesData.Variation = GetRealBarberVariation(item.Variation, item.Cvariation, false, BarberComponent.Beard);
                    clothesData.Name = item.Name;
                    clothesData.TName = item.Tname;
                    clothesData.Price = item.Price;
                    clothesData.Donate = item.Donate;
                    barberComponentData[false][BarberComponent.Beard].TryAdd(item.Id, clothesData);
                }
                OnSaveJsonBarber("Female_Beard", barberComponentData[false][BarberComponent.Beard]);

                barberComponentData[true].TryAdd(BarberComponent.Beard, new ConcurrentDictionary<int, BarberData>());
                foreach (var item in db.BarberMaleBeard.ToList())
                {
                    var clothesData = new BarberData();
                    clothesData.Variation = GetRealBarberVariation(item.Variation, item.Cvariation, true, BarberComponent.Beard);
                    clothesData.Name = item.Name;
                    clothesData.TName = item.Tname;
                    clothesData.Price = item.Price;
                    clothesData.Donate = item.Donate;
                    barberComponentData[true][BarberComponent.Beard].TryAdd(item.Id, clothesData);
                }
                OnSaveJsonBarber("Male_Beard", barberComponentData[true][BarberComponent.Beard]);
                Log.Write($"Load Beard");

                //

                barberComponentData[false].TryAdd(BarberComponent.Body, new ConcurrentDictionary<int, BarberData>());
                foreach (var item in db.BarberFemaleBody.ToList())
                {
                    var clothesData = new BarberData();
                    clothesData.Variation = GetRealBarberVariation(item.Variation, item.Cvariation, false, BarberComponent.Body);
                    clothesData.Name = item.Name;
                    clothesData.TName = item.Tname;
                    clothesData.Price = item.Price;
                    clothesData.Donate = item.Donate;
                    barberComponentData[false][BarberComponent.Body].TryAdd(item.Id, clothesData);
                }
                OnSaveJsonBarber("Female_Body", barberComponentData[false][BarberComponent.Body]);

                barberComponentData[true].TryAdd(BarberComponent.Body, new ConcurrentDictionary<int, BarberData>());
                foreach (var item in db.BarberMaleBody.ToList())
                {
                    var clothesData = new BarberData();
                    clothesData.Variation = GetRealBarberVariation(item.Variation, item.Cvariation, true, BarberComponent.Body);
                    clothesData.Name = item.Name;
                    clothesData.TName = item.Tname;
                    clothesData.Price = item.Price;
                    clothesData.Donate = item.Donate;
                    barberComponentData[true][BarberComponent.Body].TryAdd(item.Id, clothesData);
                }
                OnSaveJsonBarber("Male_Body", barberComponentData[true][BarberComponent.Body]);
                Log.Write($"Load Body");

                //

                barberComponentData[false].TryAdd(BarberComponent.Eyebrows, new ConcurrentDictionary<int, BarberData>());
                foreach (var item in db.BarberFemaleEyebrows.ToList())
                {
                    var clothesData = new BarberData();
                    clothesData.Variation = GetRealBarberVariation(item.Variation, item.Cvariation, false, BarberComponent.Eyebrows);
                    clothesData.Name = item.Name;
                    clothesData.TName = item.Tname;
                    clothesData.Price = item.Price;
                    clothesData.Donate = item.Donate;
                    barberComponentData[false][BarberComponent.Eyebrows].TryAdd(item.Id, clothesData);
                }
                OnSaveJsonBarber("Female_Eyebrows", barberComponentData[false][BarberComponent.Eyebrows]);

                barberComponentData[true].TryAdd(BarberComponent.Eyebrows, new ConcurrentDictionary<int, BarberData>());
                foreach (var item in db.BarberMaleEyebrows.ToList())
                {
                    var clothesData = new BarberData();
                    clothesData.Variation = GetRealBarberVariation(item.Variation, item.Cvariation, true, BarberComponent.Eyebrows);
                    clothesData.Name = item.Name;
                    clothesData.TName = item.Tname;
                    clothesData.Price = item.Price;
                    clothesData.Donate = item.Donate;
                    barberComponentData[true][BarberComponent.Eyebrows].TryAdd(item.Id, clothesData);
                }
                OnSaveJsonBarber("Male_Eyebrows", barberComponentData[true][BarberComponent.Eyebrows]);
                Log.Write($"Load Eyebrows");

                //

                barberComponentData[false].TryAdd(BarberComponent.Eyes, new ConcurrentDictionary<int, BarberData>());
                foreach (var item in db.BarberFemaleEyes.ToList())
                {
                    var clothesData = new BarberData();
                    clothesData.Variation = GetRealBarberVariation(item.Variation, item.Cvariation, false, BarberComponent.Eyes);
                    clothesData.Name = item.Name;
                    clothesData.TName = item.Tname;
                    clothesData.Price = item.Price;
                    clothesData.Donate = item.Donate;
                    barberComponentData[false][BarberComponent.Eyes].TryAdd(item.Id, clothesData);
                }
                OnSaveJsonBarber("Female_Eyes", barberComponentData[false][BarberComponent.Eyes]);

                barberComponentData[true].TryAdd(BarberComponent.Eyes, new ConcurrentDictionary<int, BarberData>());
                foreach (var item in db.BarberMaleEyes.ToList())
                {
                    var clothesData = new BarberData();
                    clothesData.Variation = GetRealBarberVariation(item.Variation, item.Cvariation, true, BarberComponent.Eyes);
                    clothesData.Name = item.Name;
                    clothesData.TName = item.Tname;
                    clothesData.Price = item.Price;
                    clothesData.Donate = item.Donate;
                    barberComponentData[true][BarberComponent.Eyes].TryAdd(item.Id, clothesData);
                }
                OnSaveJsonBarber("Male_Eyes", barberComponentData[true][BarberComponent.Eyes]);
                Log.Write($"Load Eyes");

                //

                barberComponentData[false].TryAdd(BarberComponent.Lips, new ConcurrentDictionary<int, BarberData>());
                foreach (var item in db.BarberFemaleLips.ToList())
                {
                    var clothesData = new BarberData();
                    clothesData.Variation = GetRealBarberVariation(item.Variation, item.Cvariation, false, BarberComponent.Lips);
                    clothesData.Name = item.Name;
                    clothesData.TName = item.Tname;
                    clothesData.Price = item.Price;
                    clothesData.Donate = item.Donate;
                    barberComponentData[false][BarberComponent.Lips].TryAdd(item.Id, clothesData);
                }
                OnSaveJsonBarber("Female_Lips", barberComponentData[false][BarberComponent.Lips]);

                barberComponentData[true].TryAdd(BarberComponent.Lips, new ConcurrentDictionary<int, BarberData>());
                foreach (var item in db.BarberMaleLips.ToList())
                {
                    var clothesData = new BarberData();
                    clothesData.Variation = GetRealBarberVariation(item.Variation, item.Cvariation, true, BarberComponent.Lips);
                    clothesData.Name = item.Name;
                    clothesData.TName = item.Tname;
                    clothesData.Price = item.Price;
                    clothesData.Donate = item.Donate;
                    barberComponentData[true][BarberComponent.Lips].TryAdd(item.Id, clothesData);
                }
                OnSaveJsonBarber("Male_Lips", barberComponentData[true][BarberComponent.Lips]);
                Log.Write($"Load Lips");

                //

                barberComponentData[false].TryAdd(BarberComponent.Makeup, new ConcurrentDictionary<int, BarberData>());
                foreach (var item in db.BarberFemaleMakeup.ToList())
                {
                    var clothesData = new BarberData();
                    clothesData.Variation = GetRealBarberVariation(item.Variation, item.Cvariation, false, BarberComponent.Makeup);
                    clothesData.Name = item.Name;
                    clothesData.TName = item.Tname;
                    clothesData.Price = item.Price;
                    clothesData.Donate = item.Donate;
                    barberComponentData[false][BarberComponent.Makeup].TryAdd(item.Id, clothesData);
                }
                OnSaveJsonBarber("Female_Makeup", barberComponentData[false][BarberComponent.Makeup]);

                barberComponentData[true].TryAdd(BarberComponent.Makeup, new ConcurrentDictionary<int, BarberData>());
                foreach (var item in db.BarberMaleMakeup.ToList())
                {
                    var clothesData = new BarberData();
                    clothesData.Variation = GetRealBarberVariation(item.Variation, item.Cvariation, true, BarberComponent.Makeup);
                    clothesData.Name = item.Name;
                    clothesData.TName = item.Tname;
                    clothesData.Price = item.Price;
                    clothesData.Donate = item.Donate;
                    barberComponentData[true][BarberComponent.Makeup].TryAdd(item.Id, clothesData);
                }
                OnSaveJsonBarber("Male_Makeup", barberComponentData[true][BarberComponent.Makeup]);
                Log.Write($"Load Makeup");

                //

                barberComponentData[false].TryAdd(BarberComponent.Palette, new ConcurrentDictionary<int, BarberData>());
                foreach (var item in db.BarberFemalePalette.ToList())
                {
                    var clothesData = new BarberData();
                    clothesData.Variation = GetRealBarberVariation(item.Variation, item.Cvariation, false, BarberComponent.Palette);
                    clothesData.Name = item.Name;
                    clothesData.TName = item.Tname;
                    clothesData.Price = item.Price;
                    clothesData.Donate = item.Donate;
                    barberComponentData[false][BarberComponent.Palette].TryAdd(item.Id, clothesData);
                }
                OnSaveJsonBarber("Female_Palette", barberComponentData[false][BarberComponent.Palette]);

                barberComponentData[true].TryAdd(BarberComponent.Palette, new ConcurrentDictionary<int, BarberData>());
                foreach (var item in db.BarberMalePalette.ToList())
                {
                    var clothesData = new BarberData();
                    clothesData.Variation = GetRealBarberVariation(item.Variation, item.Cvariation, true, BarberComponent.Palette);
                    clothesData.Name = item.Name;
                    clothesData.TName = item.Tname;
                    clothesData.Price = item.Price;
                    clothesData.Donate = item.Donate;
                    barberComponentData[true][BarberComponent.Palette].TryAdd(item.Id, clothesData);
                }
                OnSaveJsonBarber("Male_Palette", barberComponentData[true][BarberComponent.Palette]);
                Log.Write($"Load Palette");

                //
                BarberComponentData = barberComponentData;
                Log.Write($"Load Barber");
                
                
                BarberComponentPriceData = GetPrice(barberComponentData, false);
                BarberComponentDonateData = GetPrice(barberComponentData, true);
            }
        } 
        public static ConcurrentDictionary<bool, Dictionary<string, List<List<object>>>> GetPrice(ConcurrentDictionary<bool, ConcurrentDictionary<BarberComponent, ConcurrentDictionary<int, BarberData>>> clothesComponentData, bool isDonate)
        {
            var clothesComponentPriceData = new ConcurrentDictionary<bool, Dictionary<string, List<List<object>>>>();

            foreach (var componentsGender in clothesComponentData)
            {
                var componentPriceData = new Dictionary<string, List<List<object>>>();
                var componentDonateData = new Dictionary<string, List<List<object>>>();

                foreach (var componentName in componentsGender.Value)
                {
                    var clothesPriceData = new List<List<object>>();
                    var clothesDonateData = new List<List<object>>();
                    
                    foreach (var clothesData in componentName.Value)
                    {
                        if (clothesData.Value.Price == 0 && clothesData.Value.Donate == 0)
                            continue;
                        
                        if (clothesData.Value.Price == 0 && !isDonate)
                            continue;
                        
                        if (clothesData.Value.Donate == 0 && isDonate)
                            continue;


                        var price = clothesData.Value.Price > 0
                            ? clothesData.Value.Price
                            : clothesData.Value.Donate;
                        
                        clothesPriceData.Add(new List<object>
                        {
                            clothesData.Key,
                            price
                        });
                        
                    }

                    if (!componentPriceData.ContainsKey(componentName.Key.ToString()))
                        componentPriceData.Add(componentName.Key.ToString(), new List<List<object>>());
                    
                    componentPriceData[componentName.Key.ToString()] = clothesPriceData;
                    
                    //
                    
                    if (!componentDonateData.ContainsKey(componentName.Key.ToString()))
                        componentDonateData.Add(componentName.Key.ToString(), new List<List<object>>());

                    componentDonateData[componentName.Key.ToString()] = clothesDonateData;
                }

                clothesComponentPriceData[componentsGender.Key] = componentPriceData;
            }

            return clothesComponentPriceData;
        }
        private void OnSaveJsonBarber(string name, ConcurrentDictionary<int, BarberData> clothesData)
        {
            try
            {
                var saveData = new Dictionary<int, Dictionary<string, object>>();

                foreach (var clothes in clothesData)
                {
                    var data = new Dictionary<string, object>();
                    data.Add("Id", clothes.Key);
                    data.Add("Variation", clothes.Value.Variation);
                    data.Add("Name", clothes.Value.Name);
                    data.Add("TName", clothes.Value.TName);
                    data.Add("Price", clothes.Value.Price);
                    data.Add("Donate", clothes.Value.Donate);

                    saveData.Add(clothes.Key, data);
                }

                File.WriteAllText(@$"json/barber_{name}.json", string.Empty);
                using (var saveCoords = new StreamWriter(@$"json/barber_{name}.json", true, Encoding.UTF8))
                {
                    saveCoords.Write("{\n");
                    int index = 0;
                    foreach (var clothes in saveData)
                    {
                        index++;

                        if (saveData.Count == index)
                            saveCoords.Write($"\t\"{clothes.Key}\": {JsonConvert.SerializeObject(clothes.Value)}\r\n");
                        else
                            saveCoords.Write($"\t\"{clothes.Key}\": {JsonConvert.SerializeObject(clothes.Value)},\r\n");

                        //saveData.Add(clothes.Key, data);
                    }
                    saveCoords.Write("}");
                    saveCoords.Close();
                }
            }
            catch
            {
                Log.Write($"OnSaveJsonClothes");
            }
        }

        /////
        ///

        public enum TattooComponent
        {
            Head = 0,
            Torso,
            LeftArm,
            RightArm,
            LeftLeg,
            RightLeg,
        }

        public static ConcurrentDictionary<TattooComponent, ConcurrentDictionary<int, TattooData>> TattooComponentData = new ConcurrentDictionary<TattooComponent, ConcurrentDictionary<int, TattooData>>();
        public static ConcurrentDictionary<string, List<List<object>>> TattooComponentPriceData =
            new ConcurrentDictionary<string, List<List<object>>>();
        
        private void LoadTattoo()
        {
            using (var db = new ConfigBD("ConfigDB"))
            {
                var tattooComponentData = new ConcurrentDictionary<TattooComponent, ConcurrentDictionary<int, TattooData>>();

                tattooComponentData.TryAdd(TattooComponent.Head, new ConcurrentDictionary<int, TattooData>());
                foreach (var item in db.TattooHead.ToList())
                {
                    var clothesData = new TattooData();
                    clothesData.Name = item.Name;
                    clothesData.Dictionary = item.Dictionary;
                    clothesData.MaleHash = item.MaleHash;
                    clothesData.FemaleHash = item.FemaleHash;
                    clothesData.Slots = JsonConvert.DeserializeObject<List<int>>(item.Slots);
                    clothesData.Price = item.Price;
                    clothesData.Donate = item.Donate;
                    tattooComponentData[TattooComponent.Head].TryAdd(item.Id, clothesData);
                }
                OnSaveJsonTattoo("Head", tattooComponentData[TattooComponent.Head]);

                //
                tattooComponentData.TryAdd(TattooComponent.Torso, new ConcurrentDictionary<int, TattooData>());
                foreach (var item in db.TattooTorso.ToList())
                {
                    var clothesData = new TattooData();
                    clothesData.Name = item.Name;
                    clothesData.Dictionary = item.Dictionary;
                    clothesData.MaleHash = item.MaleHash;
                    clothesData.FemaleHash = item.FemaleHash;
                    clothesData.Slots = JsonConvert.DeserializeObject<List<int>>(item.Slots);
                    clothesData.Price = item.Price;
                    clothesData.Donate = item.Donate;
                    tattooComponentData[TattooComponent.Torso].TryAdd(item.Id, clothesData);
                }
                OnSaveJsonTattoo("Torso", tattooComponentData[TattooComponent.Torso]);

                //
                tattooComponentData.TryAdd(TattooComponent.LeftArm, new ConcurrentDictionary<int, TattooData>());
                foreach (var item in db.TattooLeftarm.ToList())
                {
                    var clothesData = new TattooData();
                    clothesData.Name = item.Name;
                    clothesData.Dictionary = item.Dictionary;
                    clothesData.MaleHash = item.MaleHash;
                    clothesData.FemaleHash = item.FemaleHash;
                    clothesData.Slots = JsonConvert.DeserializeObject<List<int>>(item.Slots);
                    clothesData.Price = item.Price;
                    clothesData.Donate = item.Donate;
                    tattooComponentData[TattooComponent.LeftArm].TryAdd(item.Id, clothesData);
                }
                OnSaveJsonTattoo("LeftArm", tattooComponentData[TattooComponent.LeftArm]);

                //
                tattooComponentData.TryAdd(TattooComponent.RightArm, new ConcurrentDictionary<int, TattooData>());
                foreach (var item in db.TattooRightarm.ToList())
                {
                    var clothesData = new TattooData();
                    clothesData.Name = item.Name;
                    clothesData.Dictionary = item.Dictionary;
                    clothesData.MaleHash = item.MaleHash;
                    clothesData.FemaleHash = item.FemaleHash;
                    clothesData.Slots = JsonConvert.DeserializeObject<List<int>>(item.Slots);
                    clothesData.Price = item.Price;
                    clothesData.Donate = item.Donate;
                    tattooComponentData[TattooComponent.RightArm].TryAdd(item.Id, clothesData);
                }
                OnSaveJsonTattoo("RightArm", tattooComponentData[TattooComponent.RightArm]);

                //
                tattooComponentData.TryAdd(TattooComponent.LeftLeg, new ConcurrentDictionary<int, TattooData>());
                foreach (var item in db.TattooLeftleg.ToList())
                {
                    var clothesData = new TattooData();
                    clothesData.Name = item.Name;
                    clothesData.Dictionary = item.Dictionary;
                    clothesData.MaleHash = item.MaleHash;
                    clothesData.FemaleHash = item.FemaleHash;
                    clothesData.Slots = JsonConvert.DeserializeObject<List<int>>(item.Slots);
                    clothesData.Price = item.Price;
                    clothesData.Donate = item.Donate;
                    tattooComponentData[TattooComponent.LeftLeg].TryAdd(item.Id, clothesData);
                }
                OnSaveJsonTattoo("LeftLeg", tattooComponentData[TattooComponent.LeftLeg]);

                //
                tattooComponentData.TryAdd(TattooComponent.RightLeg, new ConcurrentDictionary<int, TattooData>());
                foreach (var item in db.TattooRightleg.ToList())
                {
                    var clothesData = new TattooData();
                    clothesData.Name = item.Name;
                    clothesData.Dictionary = item.Dictionary;
                    clothesData.MaleHash = item.MaleHash;
                    clothesData.FemaleHash = item.FemaleHash;
                    clothesData.Slots = JsonConvert.DeserializeObject<List<int>>(item.Slots);
                    clothesData.Price = item.Price;
                    clothesData.Donate = item.Donate;
                    tattooComponentData[TattooComponent.RightLeg].TryAdd(item.Id, clothesData);
                }
                OnSaveJsonTattoo("RightLeg", tattooComponentData[TattooComponent.RightLeg]);

                //

                TattooComponentData = tattooComponentData;

                TattooComponentPriceData = GetPrice(tattooComponentData);
                
                Log.Write($"Load Tattoo");
            }
        }
        public static ConcurrentDictionary<string, List<List<object>>> GetPrice(ConcurrentDictionary<TattooComponent, ConcurrentDictionary<int, TattooData>> componentsGender)
        {
            var componentPriceData = new ConcurrentDictionary<string, List<List<object>>>();

            foreach (var componentName in componentsGender)
            {
                var clothesPriceData = new List<List<object>>();
                
                foreach (var clothesData in componentName.Value)
                {
                    var price = clothesData.Value.Price;
                    
                    clothesPriceData.Add(new List<object>
                    {
                        clothesData.Key,
                        price
                    });
                }

                if (!componentPriceData.ContainsKey(componentName.Key.ToString()))
                    componentPriceData.TryAdd(componentName.Key.ToString(), new List<List<object>>());
                
                componentPriceData[componentName.Key.ToString()] = clothesPriceData;
            }

            return componentPriceData;
        }
        private void OnSaveJsonTattoo(string name, ConcurrentDictionary<int, TattooData> clothesData)
        {
            try
            {
                var saveData = new Dictionary<int, Dictionary<string, object>>();

                foreach (var clothes in clothesData)
                {
                    var data = new Dictionary<string, object>();
                    data.Add("Id", clothes.Key);
                    data.Add("Name", clothes.Value.Name);
                    data.Add("Dictionary", clothes.Value.Dictionary);
                    data.Add("MaleHash", clothes.Value.MaleHash);
                    data.Add("FemaleHash", clothes.Value.FemaleHash);
                    data.Add("Slots", clothes.Value.Slots);
                    data.Add("Price", clothes.Value.Price);
                    data.Add("Donate", clothes.Value.Donate);

                    saveData.Add(clothes.Key, data);
                }

                File.WriteAllText(@$"json/tattoo_{name}.json", string.Empty);
                using (var saveCoords = new StreamWriter(@$"json/tattoo_{name}.json", true, Encoding.UTF8))
                {
                    saveCoords.Write("{\n");
                    int index = 0;
                    foreach (var clothes in saveData)
                    {
                        index++;

                        if (saveData.Count == index)
                            saveCoords.Write($"\t\"{clothes.Key}\": {JsonConvert.SerializeObject(clothes.Value)}\r\n");
                        else
                            saveCoords.Write($"\t\"{clothes.Key}\": {JsonConvert.SerializeObject(clothes.Value)},\r\n");

                        //saveData.Add(clothes.Key, data);
                    }
                    saveCoords.Write("}");
                    saveCoords.Close();
                }
            }
            catch
            {
                Log.Write($"OnSaveJsonClothes");
            }
        }
        public static InventoryItemData GetItemData(ExtPlayer player, string location, int slotId)
        {
            try
            {
                //player.IsInstanceAlive
                var locationName = Chars.Repository.GetLocationName(player, location);
                
                if (location == "other") 
                    location = locationName.Split('_')[0];
                
                if (location == "accessories" && player.Accessories != null)
                {
                    if (player.Accessories.ContainsKey(slotId))
                        return player.Accessories[slotId];
                    
                    //if (slotId == 5 && player.Accessories.ContainsKey(6))
                    //    return new InventoryItemData();
                }
                
                if (locationName != null && Repository.ItemsData.ContainsKey(locationName) && Repository.ItemsData[locationName].ContainsKey(location) && Repository.ItemsData[locationName][location].ContainsKey(slotId))
                    return Repository.ItemsData[locationName][location][slotId];
                
                return new InventoryItemData();
            }
            catch (Exception e)
            {
                Log.Write($"GetItemData Exception: {e.ToString()}");
                return new InventoryItemData();
            }
        }
    }
}
