using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using NeptuneEvo.Table.Models;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Fractions.Table.Clothes
{
    public class Repository
    {
        public static void ClothesLoad(ExtPlayer player)
        {
            if (!player.IsFractionAccess(RankToAccess.ClothesEdit)) return;
            
            var fractionData = player.GetFractionData();
            if (fractionData == null) 
                return;

            var clothesList = new List<List<object>>();
            
            foreach (var fractionSet in FractionClothingSets.FractionSets[(Models.Fractions) fractionData.Id][true])
            {
                if (clothesList.Any(f => Convert.ToBoolean(f[0]) == true && f[1].ToString() == fractionSet.SetName))
                    continue;
                
                var clothes = new List<object>();
                
                clothes.Add(true);
                clothes.Add(fractionSet.SetName);
                clothes.Add(fractionSet.Rank);
                //clothes.Add(fractionSet.ClothingIndex);
                
                clothesList.Add(clothes);
            }

            foreach (var fractionSet in FractionClothingSets.FractionSets[(Models.Fractions) fractionData.Id][false])
            {
                if (clothesList.Any(f => Convert.ToBoolean(f[0]) == false && f[1].ToString() == fractionSet.SetName))
                    continue;
                
                var clothes = new List<object>();

                clothes.Add(false);
                clothes.Add(fractionSet.SetName);
                clothes.Add(fractionSet.Rank);
                //clothes.Add(fractionSet.ClothingIndex);
                
                clothesList.Add(clothes);
            }

            Trigger.ClientEvent(player, "client.frac.main.clothes", JsonConvert.SerializeObject(clothesList));
        }

        public static void ClothesUpdate(ExtPlayer player, string oldName, string newName, int rank, bool gender)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.ClothesEdit)) return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                var fractionData = player.GetFractionData();
                if (fractionData == null) 
                    return;
                
                if (!FractionClothingSets.FractionMainCloakrooms.ContainsKey(fractionData.Id) && !FractionClothingSets.FractionSecondCloakrooms.ContainsKey(fractionData.Id))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantEditForm), 3000);
                    return;
                }

                var fracId = fractionData.Id;
                if (fracId == 0) return;

                newName = Main.BlockSymbols(newName);
                
                if (FractionClothingSets.FractionMainCloakrooms.ContainsKey(fracId) && player.Position.DistanceTo(FractionClothingSets.FractionMainCloakrooms[fracId]) < 10 || FractionClothingSets.FractionSecondCloakrooms.ContainsKey(fracId) && player.Position.DistanceTo(FractionClothingSets.FractionSecondCloakrooms[fracId]) < 10)
                {

                    if (FractionClothingSets.FractionSets.ContainsKey((Models.Fractions)fractionData.Id) &&
                        FractionClothingSets.FractionSets[(Models.Fractions)fractionData.Id].ContainsKey(gender))
                    {
                        if (!oldName.Equals(newName))
                        {
                            var fractionSets =
                                FractionClothingSets.FractionSets[(Models.Fractions)fractionData.Id][gender];
                            
                            if (fractionSets.Any(f => f.SetName.Equals(newName)))
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, gender ? LangFunc.GetText(LangType.Ru, DataName.ThisNameMaleFormUsed) : LangFunc.GetText(LangType.Ru, DataName.ThisNameFemaleFormUsed), 3000);
                                return;
                            }

                            foreach (var fractionSet in fractionSets
                                .Where(f => f.SetName.Equals(oldName)))
                            {
                                fractionSet.SetName = newName;
                                fractionSet.Rank = rank;
                            }
                            
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucEditFormName), 5000);

                        }
                        
                        sessionData.TempClothingData.NewName = newName;
                        sessionData.TempClothingData.Gender = gender;
                    
                        Trigger.Dimension(player, 10);

                        if (gender != characterData.Gender)
                            player.setSkin((gender) ? PedHash.FreemodeMale01 : PedHash.FreemodeFemale01);

                        FractionClothingSets.SetPlayerFactionClothingSet(player, fracId, newName, gender, isDutySet: false);
                        sessionData.WorkData.OnDuty = false;
                        sessionData.WorkData.OnDutyName = String.Empty;  
                        //player.ClearAccessories();

                        var realClothes = new List<string>();
                        var clothesPriceList = FractionClothingSets.FractionAvailableSets[gender][(Models.Fractions) fracId].ToList();
                        var dataJson = new Dictionary<string, List<List<object>>>();
                        
                        foreach (var components in clothesPriceList)
                        {
                            var name = components.Key.ToString();
                            var clothesData = new List<List<object>>();
                            foreach (var componentData in components.Value)
                            {
                                clothesData.Add(new List<object>
                                {
                                    componentData.DrawableId,
                                    componentData.Textures
                                }); 
                            }

                            dataJson[name] = clothesData;
                            realClothes.Add(name);
                        }
                        
                        Trigger.ClientEvent(player, "client.shop.open", "clothes", characterData.Gender, JsonConvert.SerializeObject(realClothes), JsonConvert.SerializeObject(dataJson), 2);
                        return;
                    }
                }
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TooFar), 3000);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public static void EditClothingSet(ExtPlayer player, string name, int id, int texture)
        {
            try
            {
                if (!player.IsFractionAccess(RankToAccess.ClothesEdit)) return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                var fractionData = Manager.GetFractionData(memberFractionData.Id);
                if (fractionData == null) 
                    return;
                
                if (!FractionClothingSets.FractionMainCloakrooms.ContainsKey(fractionData.Id) && !FractionClothingSets.FractionSecondCloakrooms.ContainsKey(fractionData.Id))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantEditForm), 3000);
                    return;
                }

                if (sessionData.TempClothingData.NewName == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FormEditError), 3000);
                    return;
                }
                
                int fraction = fractionData.Id;
                if (fraction == 0) return;
                
                if (FractionClothingSets.FractionMainCloakrooms.ContainsKey(fraction) && player.Position.DistanceTo(FractionClothingSets.FractionMainCloakrooms[fraction]) < 10 || FractionClothingSets.FractionSecondCloakrooms.ContainsKey(fraction) && player.Position.DistanceTo(FractionClothingSets.FractionSecondCloakrooms[fraction]) < 10)
                {
                    var dictionary = (ClothesComponent)Enum.Parse(typeof(ClothesComponent), name);
                    int rank = memberFractionData.Rank;
                    bool gender = sessionData.TempClothingData.Gender;
                    string setName = sessionData.TempClothingData.NewName;

                    if (FractionClothingSets.FractionSets.ContainsKey((Models.Fractions)fractionData.Id) &&
                        FractionClothingSets.FractionSets[(Models.Fractions)fractionData.Id].ContainsKey(gender))
                    {
                        var fractionSets =
                            FractionClothingSets.FractionSets[(Models.Fractions)fractionData.Id][
                                gender];
                        
                        if (id == -1 && texture == 0)
                        {
                            var toDeleteClothingList = fractionSets
                                .Where(f => f.SetName == setName)
                                .ToList();
                            
                            foreach (var item in toDeleteClothingList)
                            {
                                var aSet = FractionClothingSetsData.AvailableSets[item.ClothingIndex];
                            
                                if (aSet.Component == dictionary)
                                {
                                    fractionSets.Remove(item);             
                                    FractionClothingSets.SetPlayerFactionClothingSet(player, fractionData.Id, setName, gender, isDutySet: false); 
                                    //player.ClearAccessories();
                                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucDeleteFormComponent), 3000);
                                    return;
                                }
                            }
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FormError), 3000);
                            return;
                        }
                        
                        //
                        var availableSet = FractionClothingSetsData.AvailableSets
                            .Select((value, i) => (value, i))
                            .FirstOrDefault(f =>
                                f.value.Gender == Convert.ToByte(gender) && f.value.Fraction == (Models.Fractions)fractionData.Id && f.value.Component == dictionary && f.value.Variation == id && f.value.Color == texture);


                        if (availableSet.value != null)
                        {
                            var isUpdate = false;
                            
                            foreach (var item in fractionSets
                                .Where(f => f.SetName == setName))
                            {
                                rank = item.Rank;

                                if (FractionClothingSetsData.AvailableSets[item.ClothingIndex].Component == dictionary)
                                {
                                    item.ClothingIndex = availableSet.i;
                                    isUpdate = true;
                                    break;
                                }
                            }
                            
                            if (!isUpdate)
                                fractionSets.Add(new FractionSetData(rank, setName, availableSet.i));
                            
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucEditForm), 8000);  
                            FractionClothingSets.SetPlayerFactionClothingSet(player, fractionData.Id, setName, gender, isDutySet: false); 
                            //player.ClearAccessories();
                            
                            return;
                        }
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantEditThisForm), 3000);
                        return;
                    }

                }
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TooFar), 3000);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
    }
}