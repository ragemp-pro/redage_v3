using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;

using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Localization;
using MySqlConnector;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Table.Models;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;

namespace NeptuneEvo.Fractions
{
    class fTable : Script
    {
        public static readonly nLog Log = new nLog("Fractions.Table");
        public static Dictionary<int, List<BoardData>> BoardList = new Dictionary<int, List<BoardData>>();

        public static void Init()
        {
            for (int i = (int) Models.Fractions.FAMILY; i <= Configs.FractionCount; i++)
            {
                BoardList.Add(i, new List<BoardData>());
            }
        }

        /*public static void OpenFraction(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
		
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                var fracId = memberFractionData.Id;

                var playersList = new List<TablePlayerData>();

                foreach (var foreachMemberFractionData in Manager.AllMembers[fracId].ToList())
                {
                    if (foreachMemberFractionData.PlayerId == -1) 
                        playersList.Add(new TablePlayerData(false, foreachMemberFractionData.Name, "", foreachMemberFractionData.Rank, foreachMemberFractionData.UUID));
                    else
                        playersList.Add(new TablePlayerData(true, foreachMemberFractionData.Name, "", foreachMemberFractionData.Rank, foreachMemberFractionData.UUID, foreachMemberFractionData.PlayerId));
                }

                var vehiclesList = new List<TableVehicleData>();

                if (Configs.FractionVehicles.ContainsKey(fracId))
                {
                    foreach (KeyValuePair<string, FractionVehicleData> v in Configs.FractionVehicles[fracId])
                    {
                        string model = v.Value.model;
                        if (fracId == (int) Models.Fractions.MERRYWEATHER && (model.Equals("SUBMERSIBLE") || model.Equals("THRUSTER"))) continue;
                        var vehicleLocalData = VehicleData.LocalData.Repository.GetVehicleLocalDataToNumber(VehicleAccess.Fraction, v.Key);
                        if (vehicleLocalData == null) continue;                        
                        vehiclesList.Add(new TableVehicleData(model, v.Key, vehicleLocalData.MinRank));
                    }
                }
                string JsonPlayers = JsonConvert.SerializeObject(playersList);
                string JsonVehicles = JsonConvert.SerializeObject(vehiclesList);
                string JsonBoardList = JsonConvert.SerializeObject(BoardList[fracId]);

                Dictionary<string, int> _MyAccess = new Dictionary<string, int>();

                bool isLeader = memberFractionData.Rank == Configs.FractionRanks[fracId].Count;

                foreach (RankToAccess listAccess in isLeader ? Configs.FractionDefaultAccess[fracId] : Configs.FractionRanks[fracId][memberFractionData.Rank].Access)
                {
                    _MyAccess.Add(listAccess.ToString(), (int)listAccess);
                }
                string JsonSettings = JsonConvert.SerializeObject(_MyAccess);

                string JsonDefaultAccess = JsonConvert.SerializeObject(new List<RankToAccess>());
                string JsonAccess = JsonConvert.SerializeObject(new Dictionary<int, FractionRankData>());
                string JsonClothesData = JsonConvert.SerializeObject(new List<FractionSetData>());

                if (isLeader)
                {
                    List<string> _AllDefaultAccess = new List<string>();

                    foreach (RankToAccess listAccess in Configs.FractionDefaultAccess[fracId])
                    {
                        _AllDefaultAccess.Add(listAccess.ToString());
                    }

                    JsonDefaultAccess = JsonConvert.SerializeObject(_AllDefaultAccess);

                    Dictionary<int, Dictionary<string, int>> allAccess = new Dictionary<int, Dictionary<string, int>>();
                    foreach (KeyValuePair<int, FractionRankData> listRank in Configs.FractionRanks[fracId])
                    {
                        allAccess.Add(listRank.Key, new Dictionary<string, int>());
                        foreach (RankToAccess listAccess in listRank.Value.Access)
                        {
                            allAccess[listRank.Key].Add(listAccess.ToString(), (int)listAccess);
                        }
                    }

                    JsonAccess = JsonConvert.SerializeObject(allAccess);

                    if (FractionClothingSets.FractionSets.ContainsKey((Models.Fractions) fracId))
                    {
                        var clothesData = new Dictionary<bool, List<FractionSetData>>();
                        clothesData[true] = new List<FractionSetData>();

                        foreach (var fractionSet in FractionClothingSets.FractionSets[(Models.Fractions) fracId][true])
                        {
                            if (clothesData[true].Any(f => f.SetName == fractionSet.SetName))
                                continue;

                            clothesData[true].Add(fractionSet);
                        }
                        
                        clothesData[false] = new List<FractionSetData>();

                        foreach (var fractionSet in FractionClothingSets.FractionSets[(Models.Fractions) fracId][false])
                        {
                            
                            if (clothesData[false].Any(f => f.SetName == fractionSet.SetName))
                                continue;

                            clothesData[false].Add(fractionSet);
                        }
                        JsonClothesData = JsonConvert.SerializeObject(clothesData);
                    }
                }

                Trigger.ClientEvent(player, "client.table.open", JsonPlayers, JsonVehicles, JsonBoardList, JsonSettings, JsonDefaultAccess, JsonAccess, "[]", JsonClothesData, false);
            }
            catch (Exception e)
            {
                Log.Write($"OpenFraction Exception: {e.ToString()}");
            }
        }
        public static void fracad(ExtPlayer player, string text)
        {

        }
        public static void ufracad(ExtPlayer player, int index, string text)
        {
 
        }
        public static void dfracad(ExtPlayer player, int index)
        {

        }
        public static void rank(ExtPlayer player, bool isUp, string name)
        {

        }
        public static void irank(ExtPlayer player, int rank, string name)
        {

        }
        public static void invite(ExtPlayer player, string name)
        {

        }
        public static void uninvite(ExtPlayer player, string name)
        {

        }
        public static void evacuation(ExtPlayer player, string number)
        {
   
        }
        public static void gps(ExtPlayer player, string number)
        {

        }
        public static void vrank(ExtPlayer player, bool isUp, string number)

        }

        [RemoteEvent("server.table.clothingSetRank")]
        public void ClothingSetRank(ExtPlayer player, bool isUp, string name, bool gender)
        {
            try
            {				
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;

                if (Configs.FractionRanks.ContainsKey(memberFractionData.Id) && memberFractionData.Rank < Configs.FractionRanks[memberFractionData.Id].Count)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoAccess), 3000);
                    return;
                }

                if (FractionClothingSets.FractionSets.ContainsKey((Models.Fractions) memberFractionData.Id) &&
                    FractionClothingSets.FractionSets[(Models.Fractions) memberFractionData.Id].ContainsKey(gender))
                {
                    sbyte actionResult = 0;
                    
                    foreach (var fractionSet in FractionClothingSets.FractionSets[(Models.Fractions) memberFractionData.Id][gender]
                        .Where(f => f.SetName.Equals(name)))
                    {
                        
                        if (isUp)
                        {
                            if (fractionSet.Rank + 1 > memberFractionData.Rank)
                            {
                                actionResult = 1;
                            }
                            else
                            {
                                fractionSet.Rank += 1;
                                actionResult = 2;
                            }
                        }
                        else
                        {
                            if (fractionSet.Rank - 1 < 1)
                            {
                                actionResult = 1;
                            }
                            else
                            {
                                fractionSet.Rank -= 1;
                                actionResult = 2;
                            }
                        }
                    }
                    if (actionResult == 0)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantEditRankFormToThis), 3000);
                        return;
                    }
                    if (actionResult == 1)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindForm), 3000);
                        return;
                    };
                
                
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucEditFormRank), 3000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"ClothingSetRank Exception: {e.ToString()}");
            }
        }
        
        [RemoteEvent("server.table.startEditClothingSet")]
        public void StartEditClothingSet(ExtPlayer player, string oldName, string newName, bool gender)
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
                
                if (!FractionClothingSets.FractionMainCloakrooms.ContainsKey(memberFractionData.Id) && !FractionClothingSets.FractionSecondCloakrooms.ContainsKey(memberFractionData.Id))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantEditForm), 3000);
                    return;
                }

                if (Configs.FractionRanks.ContainsKey(memberFractionData.Id) && memberFractionData.Rank < Configs.FractionRanks[memberFractionData.Id].Count)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoAccess), 3000);
                    return;
                }

                int fracid = memberFractionData.Id;
                if (fracid == 0) return;

                newName = Main.BlockSymbols(newName);
                
                if (FractionClothingSets.FractionMainCloakrooms.ContainsKey(fracid) && player.Position.DistanceTo(FractionClothingSets.FractionMainCloakrooms[fracid]) < 10 || FractionClothingSets.FractionSecondCloakrooms.ContainsKey(fracid) && player.Position.DistanceTo(FractionClothingSets.FractionSecondCloakrooms[fracid]) < 10)
                {

                    if (FractionClothingSets.FractionSets.ContainsKey((Models.Fractions) memberFractionData.Id) &&
                        FractionClothingSets.FractionSets[(Models.Fractions) memberFractionData.Id].ContainsKey(gender))
                    {
                        if (!oldName.Equals(newName))
                        {
                            var fractionSets =
                                FractionClothingSets.FractionSets[(Models.Fractions) memberFractionData.Id][
                                    gender];
                            
                            if (fractionSets.Any(f => f.SetName.Equals(newName)))
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, gender ? LangFunc.GetText(LangType.Ru, DataName.ThisNameMaleFormUsed) : LangFunc.GetText(LangType.Ru, DataName.ThisNameFemaleFormUsed), 3000);
                                return;
                            }

                            foreach (var fractionSet in fractionSets
                                .Where(f => f.SetName.Equals(oldName)))
                            {
                                fractionSet.SetName = newName;
                            }
                            
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucEditFormName), 3000);

                        }
                        
                        sessionData.TempClothingData.NewName = newName;
                        sessionData.TempClothingData.Gender = gender;
                    
                        Trigger.Dimension(player, 10);

                        if (gender != characterData.Gender)
                            player.setSkin((gender) ? PedHash.FreemodeMale01 : PedHash.FreemodeFemale01);

                        FractionClothingSets.SetPlayerFactionClothingSet(player, fracid, newName, gender, isDutySet: false);
                        sessionData.WorkData.OnDuty = false;
                        sessionData.WorkData.OnDutyName = String.Empty;  
                        //player.ClearAccessories();

                        var realClothes = new List<string>();
                        var clothesPriceList = FractionClothingSets.FractionAvailableSets[gender][(Models.Fractions)  fracid].ToList();
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
                Log.Write($"StartEditClothingSet Exception: {e.ToString()}");
            }
        }
        
        [RemoteEvent("server.table.editClothingSet")]
        public void EditClothingSet(ExtPlayer player, string name, int id, int texture)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                var memberFractionData = player.GetFractionMemberData();
                if (memberFractionData == null)
                    return;
                
                if (!FractionClothingSets.FractionMainCloakrooms.ContainsKey(memberFractionData.Id) && !FractionClothingSets.FractionSecondCloakrooms.ContainsKey(memberFractionData.Id))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantEditForm), 3000);
                    return;
                }

                if (Configs.FractionRanks.ContainsKey(memberFractionData.Id) && memberFractionData.Rank < Configs.FractionRanks[memberFractionData.Id].Count)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoAccess), 3000);
                    return;
                }

                if (sessionData.TempClothingData.NewName == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FormEditError), 3000);
                    return;
                }
                
                int fraction = memberFractionData.Id;
                if (fraction == 0) return;
                
                if (FractionClothingSets.FractionMainCloakrooms.ContainsKey(fraction) && player.Position.DistanceTo(FractionClothingSets.FractionMainCloakrooms[fraction]) < 10 || FractionClothingSets.FractionSecondCloakrooms.ContainsKey(fraction) && player.Position.DistanceTo(FractionClothingSets.FractionSecondCloakrooms[fraction]) < 10)
                {
                    var dictionary = (ClothesComponent)Enum.Parse(typeof(ClothesComponent), name);
                    int rank = memberFractionData.Rank;
                    bool gender = sessionData.TempClothingData.Gender;
                    string setName = sessionData.TempClothingData.NewName;

                    if (FractionClothingSets.FractionSets.ContainsKey((Models.Fractions) memberFractionData.Id) &&
                        FractionClothingSets.FractionSets[(Models.Fractions) memberFractionData.Id].ContainsKey(gender))
                    {
                        var fractionSets =
                            FractionClothingSets.FractionSets[(Models.Fractions) memberFractionData.Id][
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
                                    FractionClothingSets.SetPlayerFactionClothingSet(player, memberFractionData.Id, setName, gender, isDutySet: false); 
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
                                f.value.Gender == Convert.ToByte(gender) && f.value.Fraction == (Models.Fractions) memberFractionData.Id && f.value.Component == dictionary && f.value.Variation == id && f.value.Color == texture);


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
                            
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SucEditForm), 3000);  
                            FractionClothingSets.SetPlayerFactionClothingSet(player, memberFractionData.Id, setName, gender, isDutySet: false); 
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
                Log.Write($"EditClothingSet Exception: {e.ToString()}");
            }
        }
        
        public static void accessdelete(ExtPlayer player, int fraclvl, int accessIndex)
        {

        }
        public static void accessadd(ExtPlayer player, int fraclvl, int accessIndex)
        {
            
        }
        public static void editrank(ExtPlayer player, int index, string rankName)
        {

        }
        public static void defaultrank(ExtPlayer player)
        {

        }
        public static void defaultvrank(ExtPlayer player)
        {
        }

        public static void getLogs(ExtPlayer player, int uuid, int pageId, int skip)
        {
            
        }
        public static void reprimand(ExtPlayer player, int uuid, string name, string text)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                else if (!Manager.canUseCommand(player, RankToAccess.Reprimand)) return;
                if (text.Length > 100)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MaxVigovorLength), 4500);
                    return;
                }

                var memberFractionData = player.GetFractionMemberData();
                var targetMemberFractionData = Manager.GetFractionMemberData(name, memberFractionData.Id);
                if (targetMemberFractionData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerFraction, name), 3000);
                    return;
                }
                else if (targetMemberFractionData.Rank >= memberFractionData.Rank)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouCantVigovor, name), 3000);
                    return;
                };
                addLogs(player, FractionLogsType.Reprimand, LangFunc.GetText(LangType.Ru, DataName.GivenVigovor, name, uuid, text));
            }
            catch (Exception e)
            {
                Log.Write($"reprimand Exception: {e.ToString()}");
            }
        }*/
    }
}
