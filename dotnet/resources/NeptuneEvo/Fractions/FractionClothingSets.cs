using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Handles;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Players;
using NeptuneEvo.Players.Popup.List.Models;
using NeptuneEvo.Fractions.LSNews;
using Redage.SDK;

namespace NeptuneEvo.Fractions
{
    public class FractionSetData
    {
        public int Rank { get; set; }
        public string SetName { get; set; }
        public int ClothingIndex { get; set; }
        
        public FractionSetData(int rank, string set_name, int clothing_index)
        {
            Rank = rank;
            SetName = set_name;
            ClothingIndex = clothing_index;
        }
    }
    
    public class FractionClothingSets : Script
    {
        public static readonly nLog Log = new nLog("Fractions.FractionClothingSets");

        public static Dictionary<int, Vector3> FractionMainCloakrooms = new Dictionary<int, Vector3>()
        {
            { (int) Models.Fractions.CITY, Cityhall.CloakroomPosition },
            { (int) Models.Fractions.POLICE, Police.CloakroomPosition },
            { (int) Models.Fractions.EMS, Ems.emsCheckpoints[4] },
            { (int) Models.Fractions.FIB, Fbi.fbiCheckpoints[0] },
            { (int) Models.Fractions.ARMY, Army.ArmyCheckpoints[1] },
            { (int) Models.Fractions.LSNEWS, LSNews.LsNewsSystem.LSNewsCoords[0] },
            { (int) Models.Fractions.SHERIFF, Sheriff.CloakroomPosition },
        };

        public static Dictionary<int, Vector3> FractionSecondCloakrooms = new Dictionary<int, Vector3>()
        {
            //{ (int) Models.Fractions.CITY, Cityhall.SecondCloakroomPosition },
            { (int) Models.Fractions.FIB, Fbi.fbiCheckpoints[14] },
            { (int) Models.Fractions.SHERIFF, Sheriff.SecondCloakroomPosition },
        };
        
        public static Dictionary<Models.Fractions, Dictionary<bool, List<FractionSetData>>> FractionSets = new Dictionary<Models.Fractions, Dictionary<bool, List<FractionSetData>>>();
        public static Dictionary<bool, Dictionary<Models.Fractions, Dictionary<ClothesComponent, List<FractionClothesData>>>> FractionAvailableSets = new Dictionary<bool, Dictionary<Models.Fractions, Dictionary<ClothesComponent, List<FractionClothesData>>>>();
        
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            try
            {
                FractionAvailableSets.Add(true, new Dictionary<Models.Fractions, Dictionary<ClothesComponent, List<FractionClothesData>>>());
                FractionAvailableSets.Add(false, new Dictionary<Models.Fractions, Dictionary<ClothesComponent, List<FractionClothesData>>>());

                foreach (var availableSet in FractionClothingSetsData.AvailableSets)
                {
                    if (availableSet.Component == ClothesComponent.None)
                        continue;
                    
                    var gender = Convert.ToBoolean(availableSet.Gender);
                    //
                    if (!FractionAvailableSets[gender].ContainsKey(availableSet.Fraction))
                        FractionAvailableSets[gender][availableSet.Fraction] = new Dictionary<ClothesComponent, List<FractionClothesData>>();
                    //
                    if (!FractionAvailableSets[gender][availableSet.Fraction].ContainsKey(availableSet.Component))
                        FractionAvailableSets[gender][availableSet.Fraction][availableSet.Component] = new List<FractionClothesData>();
                    //
                    var index = FractionAvailableSets[gender][availableSet.Fraction][availableSet.Component]
                        .FindIndex(cd => cd.DrawableId == availableSet.Variation);
                    
                    if (index != -1)
                        FractionAvailableSets[gender][availableSet.Fraction][availableSet.Component][index].Textures.Add(availableSet.Color);
                    else
                    {
                        FractionAvailableSets[gender][availableSet.Fraction][availableSet.Component].Add(new FractionClothesData()
                        {
                            DrawableId = availableSet.Variation,
                            Textures = new List<int>()
                            {
                                availableSet.Color
                            }
                        });
                    }
                }
                
                /*using MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "SELECT * FROM fraction_clothing_sets"
                };

                using DataTable result = MySQL.QueryRead(cmd);
                if (result == null || result.Rows.Count == 0)
                {
                    Log.Write("Table 'fraction_clothing_sets' returns null result");
                }
                else
                {
                    foreach (DataRow Row in result.Rows)
                    {
                        int fraction = Convert.ToInt32(Row["fraction"]);
                        int rank = Convert.ToInt32(Row["rank"]);
                        bool gender = Convert.ToBoolean(Row["gender"]);
                        string name = Convert.ToString(Row["name"]);
                        int clothing_index = Convert.ToInt32(Row["clothing_index"]);

                        if (!FractionSets.ContainsKey((Models.Fractions) fraction))
                            FractionSets[(Models.Fractions) fraction] = new Dictionary<bool, List<FractionSetData>>();

                        if (!FractionSets[(Models.Fractions) fraction].ContainsKey(gender))
                            FractionSets[(Models.Fractions) fraction][gender] = new List<FractionSetData>();
                        
                        
                        
                        FractionSets[(Models.Fractions) fraction][gender].Add(new FractionSetData(rank, name, clothing_index));
                    }
                }

                foreach (var item in FractionSets)
                {
                    Console.WriteLine($"UPDATE fractions SET clothingsets='{JsonConvert.SerializeObject(item.Value)}' WHERE id={(int)item.Key};");
                }*/
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }

        public static void OpenFractionClothingSetsMenu(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                if (!FractionMainCloakrooms.ContainsKey(memberFractionData.Id) && !FractionSecondCloakrooms.ContainsKey(memberFractionData.Id))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantGetForm), 3000);
                    return;
                }
                
                var frameList = new FrameListData();
                frameList.Header = LangFunc.GetText(LangType.Ru, DataName.WorkClothes);
                frameList.Callback = callback_fraction_clothing_sets_menu;
                
                if (FractionSets.ContainsKey((Models.Fractions) memberFractionData.Id) && FractionSets[(Models.Fractions) memberFractionData.Id].ContainsKey(characterData.Gender))
                {
                    var clothingList = FractionSets[(Models.Fractions) memberFractionData.Id][characterData.Gender]
                        .Where(f => f.Rank <= memberFractionData.Rank)
                        .ToList();

                    var nameToList = new List<string>();
                    foreach (var item in clothingList)
                    {
                        if (nameToList.Contains(item.SetName))
                            continue;
                        
                        frameList.List.Add(new ListData(item.SetName, item.SetName)); // НЕ УВЕРЕН
                        nameToList.Add(item.SetName);
                    }
                }

                if (sessionData.WorkData.OnDuty) 
                    frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.UnwearForm), "takeoff"));

                Players.Popup.List.Repository.Open(player, frameList);   
            }
            catch (Exception e)
            {
                Log.Write($"OpenFractionClothingSetsMenu Exception: {e.ToString()}");
            }
        }

        private static void callback_fraction_clothing_sets_menu(ExtPlayer player, object listItem) /// Никитос Чини
        {
            try
            {
                if (!(listItem is string))
                    return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;

                if (!FractionMainCloakrooms.ContainsKey(memberFractionData.Id) && !FractionSecondCloakrooms.ContainsKey(memberFractionData.Id))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantGetForm), 3000);
                    return;
                }

                var type = (string) listItem;
                if (FractionMainCloakrooms.ContainsKey(memberFractionData.Id) && player.Position.DistanceTo(FractionMainCloakrooms[memberFractionData.Id]) < 5 || FractionSecondCloakrooms.ContainsKey(memberFractionData.Id) && player.Position.DistanceTo(FractionSecondCloakrooms[memberFractionData.Id]) < 5)
                {
                    switch (type)
                    {
                        case "takeoff":
                            if (!sessionData.WorkData.OnDuty) return;
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,
                                LangFunc.GetText(LangType.Ru, DataName.EndWorkDay), 3000);
                            sessionData.WorkData.OnDuty = false;
                            sessionData.WorkData.OnDutyName = String.Empty;  
                            player.ClearAccessories();
                            Customization.ApplyCharacter(player);
                            return;
                        default:
                            if (FractionSets.ContainsKey((Models.Fractions) memberFractionData.Id) && FractionSets[(Models.Fractions) memberFractionData.Id].ContainsKey(characterData.Gender))
                            {
                               
                                var clothingList = FractionSets[(Models.Fractions) memberFractionData.Id][characterData.Gender]
                                    .Where(f => f.Rank <= memberFractionData.Rank)
                                    .Where(f => f.SetName == type)
                                    .ToList();

                                if (clothingList.Count > 0)
                                {
                                    // Чек на готовность сета, временно так...
                                    byte setReadyCount = 0;

                                    foreach (var cItem in clothingList)
                                    {
                                        switch (@FractionClothingSetsData.AvailableSets[cItem.ClothingIndex].Component)
                                        {
                                            case ClothesComponent.Undershort:
                                                setReadyCount += 1;
                                                break;
                                            case ClothesComponent.Tops:
                                                setReadyCount += 1;
                                                break;
                                            case ClothesComponent.Legs:
                                                setReadyCount += 1;
                                                break;
                                            case ClothesComponent.Shoes:
                                                setReadyCount += 1;
                                                break;
                                        }

                                        if (setReadyCount >= 3)
                                            break;
                                    }

                                    if (setReadyCount < 3)
                                    {
                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                                            LangFunc.GetText(LangType.Ru, DataName.FormNotReady),
                                            3000);
                                        return;
                                    }
                                    // Чек на готовность сета, временно так...
                                    sessionData.WorkData.OnDutyName = type;

                                    if (SetPlayerFactionClothingSet(player, memberFractionData.Id, type,
                                        characterData.Gender, true))
                                    {
                                        sessionData.WorkData.OnDutyName = type;
                                    }
                                    return;
                                    
                                }
                            }
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantUseThisForm), 3000);
                            return;
                    }
                }
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TooFar), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"callback_fraction_clothing_sets_menu Exception: {e.ToString()}");
            }
        }
        
        public static bool SetPlayerFactionClothingSet(ExtPlayer player, int fracid, string setname, bool gender, bool isDutySet = false)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return false;
                				
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return false;

                if (FractionSets.ContainsKey((Models.Fractions) fracid) &&
                    FractionSets[(Models.Fractions) memberFractionData.Id].ContainsKey(gender))
                {
                    
                    var clothingList = FractionSets[(Models.Fractions) fracid][gender]
                        .Where(f => f.Rank <= memberFractionData.Rank)
                        .Where(f => f.SetName == setname)
                        .ToList();

                    
                    if (clothingList.Count > 0)
                    {
                       var onDuty = sessionData.WorkData.OnDuty;

                        sessionData.WorkData.OnDuty = false;
                        //player.ClearAccessories();
                        //ClothesComponents.ClearClothes(player, characterData.Gender);
                        
                        foreach (var cItem in clothingList)
                        {
                            var availableSet = FractionClothingSetsData.AvailableSets[cItem.ClothingIndex];

                            if (availableSet.Fraction != (Models.Fractions) fracid)
                                continue;
                            
                            if (Chars.Repository.ClothesComponentToComponentId.ContainsKey(availableSet.Component))
                                ClothesComponents.SetSpecialClothes(player, Chars.Repository.ClothesComponentToComponentId[availableSet.Component].SlotId, availableSet.Variation, availableSet.Color); 
                            
                            if (Chars.Repository.ClothesComponentToPropId.ContainsKey(availableSet.Component))
                                ClothesComponents.SetSpecialAccessories(player, Chars.Repository.ClothesComponentToPropId[availableSet.Component].SlotId, availableSet.Variation, availableSet.Color); 
                            
                            /*if (Chars.Repository.ClothesComponentToItemId.ContainsKey(availableSet.Component))
                            {
                                var itemId = Chars.Repository.ClothesComponentToItemId[availableSet.Component];

                                var slotId = Chars.Repository.AccessoriesInfo
                                    .Where(ai => ai.Value == itemId)
                                    .Select(ai => ai.Key)
                                    .FirstOrDefault();
                                
                                var data = $"{availableSet.Variation}_{availableSet.Color}_{gender}";
                                
                                var item = new InventoryItemData(ItemId: itemId, Data: data);
                                
                                player.SetSpecialAccessories(slotId, item);
                            }*/
                        } 
                        
                        Chars.Repository.LoadAccessories(player);

                        sessionData.WorkData.OnDuty = onDuty;
                        if (isDutySet && !onDuty)
                        {
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.StartWorkDay), 3000);
                            sessionData.WorkData.OnDuty = true;
                        }
                        return true;
                    }
                }
                
                if (isDutySet)
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantUseThisForm), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"SetPlayerFactionClothingSet Exception: {e.ToString()}");
            }
            return false;
        }
        
    }
}