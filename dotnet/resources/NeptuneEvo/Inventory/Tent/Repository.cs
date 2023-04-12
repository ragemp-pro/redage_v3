using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.GUI;
using NeptuneEvo.Inventory.Tent.Models;
using NeptuneEvo.Players;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Localization;
using NeptuneEvo.Players.Phone.Messages.Models;
using NeptuneEvo.Players.Popup.List.Models;

namespace NeptuneEvo.Inventory.Tent
{
    class Repository
    {
        private static readonly nLog Log = new nLog("Inventory.Tent.Repository");

        public static readonly int TentPrice = 500;
        public static Dictionary<int, TentData> TentsData = new Dictionary<int, TentData>();
        public static void OnResourceStart()
        {
            if (Main.ServerSettings.IsDeleteProp)
            {
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02j"),
                    new Vector3(-1712.405, -724.9124, 10.33788), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02l"),
                    new Vector3(-1721.525, -736.0424, 9.298393), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02l"),
                    new Vector3(-1729.268, -745.6074, 9.298393), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02l"),
                    new Vector3(-1719.96, -753.6482, 9.298393), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02l"),
                    new Vector3(-1710.612, -761.6642, 9.298393), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02l"),
                    new Vector3(-1701.294, -769.6531, 9.298393), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02l"),
                    new Vector3(-1691.945, -777.6699, 9.298393), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02l"),
                    new Vector3(-1682.632, -785.7361, 9.298393), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02l"),
                    new Vector3(-1673.324, -793.7986, 9.298393), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02l"),
                    new Vector3(-1663.975, -801.8085, 9.298393), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02l"),
                    new Vector3(-1654.568, -809.7322, 9.286438), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02i"),
                    new Vector3(-1651.426, -812.3784, 9.286438), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02i"),
                    new Vector3(-1648.245, -814.9832, 9.276375), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02j"),
                    new Vector3(-1630.439, -790.2651, 10.51931), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02l"),
                    new Vector3(-1641.382, -781.1859, 9.193226), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02l"),
                    new Vector3(-1650.919, -773.3159, 8.784565), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02l"),
                    new Vector3(-1660.669, -765.5093, 9.006096), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02l"),
                    new Vector3(-1670.161, -757.679, 9.274288), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02l"),
                    new Vector3(-1679.637, -749.8625, 9.201832), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02l"),
                    new Vector3(-1689.141, -742.0229, 9.232605), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02l"),
                    new Vector3(-1698.427, -733.9775, 9.17775), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02l"),
                    new Vector3(-1707.743, -725.9158, 9.141079), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02j"),
                    new Vector3(-1710.937, -723.3264, 9.121449), 10f);
                NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("prop_fnclink_02i"),
                    new Vector3(-1710.937, -723.3264, 9.121449), 10f);
            }

            int index = 0;
            foreach(var data in TentList.PositionsData)
            {
                var shape = CustomColShape.CreateCylinderColShape(data.tradePosition, 2f, 3, 0, ColShapeEnums.Tent, index);
                var label = (ExtTextLabel) NAPI.TextLabel.CreateTextLabel($"#{index + 1}", data.shopPosition + new Vector3(0, 0, 1f), 7f, 0.5F, 0, new Color(255, 255, 255), true, 0);
                var marker = (ExtMarker) NAPI.Marker.CreateMarker(1, data.tradePosition, new Vector3(), new Vector3(), 0.4f, new Color(255, 255, 255, 100), false, 0);

                TentsData.Add(index, new TentData(marker, label, shape, data.isBlack));

                index++;
            }

            Main.CreateBlip(new Main.BlipData(565, LangFunc.GetText(LangType.Ru, DataName.GlavRinok), TentList.PositionGps[0], 36, true));
            CustomColShape.CreateCylinderColShape(TentList.PositionGps[0], 90, 30, 0, colShapeEnums: ColShapeEnums.SafeZoneTent);

            Main.CreateBlip(new Main.BlipData(484, LangFunc.GetText(LangType.Ru, DataName.CherRinok), TentList.PositionGps[1], 54, true));
            CustomColShape.CreateCylinderColShape(TentList.PositionGps[1], 90, 30, 0, colShapeEnums: ColShapeEnums.SafeZoneTent);
         
            Log.Write($"Load Tent {index}");
        }
        /*[Interaction(ColShapeEnums.SafeZoneTent, Out: true)]
        public static void OutSafeZoneTent(Player player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            if (sessionData.TentIndex == -1) return;

            if (!TentsData.ContainsKey(sessionData.TentIndex)) return;

            RemoveTent(sessionData.TentIndex, "SafeZoneTent");
        }*/
        [Interaction(ColShapeEnums.Tent)]
        public static void OpenTent(ExtPlayer player, int index)
        {
            if (!FunctionsAccess.IsWorking("tent"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                return;
            }
            if (!TentsData.ContainsKey(index)) return;

            var tentData = TentsData[index];

            var target = tentData.player;

            if (target == null)
            {
                OpenRentTent(player, index);
                return;
            }
            else if (target == player)
            {
                OpenMenuTent(player);
                return;
            }

            var targetCharacterData = target.GetCharacterData();
            if (targetCharacterData == null) return;
            //Trigger.ClientEvent(player, "client.inventory.SlotToPrice", JsonConvert.SerializeObject(tentData.slotToPrice));

            Chars.Repository.LoadOtherItemsData(player, "tent", targetCharacterData.UUID.ToString(), 11, Chars.Repository.InventoryMaxSlots["tent"], isMyTent: false);            
        }

        public static void OpenMenuTent(ExtPlayer player, bool isPhone = false)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                if (sessionData.TentIndex == -1) return;

                if (!TentsData.ContainsKey(sessionData.TentIndex)) return;

                var tentData = TentsData[sessionData.TentIndex];

                var frameList = new FrameListData();
                frameList.Header = LangFunc.GetText(LangType.Ru, DataName.Palatka);
                frameList.Callback = callback_menutent;
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.RentTentDo, tentData.RentTime), "info"));
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.RentTentExp, MoneySystem.Wallet.Format(TentPrice)), "rentTime"));
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.CancelRentTent), "remove"));
                

                if (!isPhone)
                {
                    frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.Naming), "name"));
                    frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.Inventory), "inventory"));
                }

                Players.Popup.List.Repository.Open(player, frameList); 
            }
            catch (Exception e)
            {
                Log.Write($"OpenRentcarMenu Exception: {e.ToString()}");
            }
        }

        private static void callback_menutent(ExtPlayer player,  object listItem) /// Никитос Чини
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (sessionData.TentIndex == -1) return;

                if (!TentsData.ContainsKey(sessionData.TentIndex)) return;

                var tentData = TentsData[sessionData.TentIndex];

                if (tentData.player != player) return;

                switch (listItem)
                {
                    case "remove":
                        RemoveTent(sessionData.TentIndex, "remove");
                        break;
                    case "inventory":
                        //Trigger.ClientEvent(player, "client.inventory.SlotToPrice", JsonConvert.SerializeObject(tentData.slotToPrice));
                        Chars.Repository.LoadOtherItemsData(player, "tent", characterData.UUID.ToString(), 11, Chars.Repository.InventoryMaxSlots["tent"], isMyTent: true);
                        break;
                    case "rentTime":
                        if (UpdateData.CanIChange(player, TentPrice, true) != 255) return;

                        GameLog.Money($"player({characterData.UUID})", $"tent({sessionData.TentIndex})", TentPrice, "rentTimeTent");

                        MoneySystem.Wallet.Change(player, -TentPrice);

                        EventSys.SendCoolMsg(player,"Рынок", "Аренда палатки", $"{LangFunc.GetText(LangType.Ru, DataName.RentTentExpand)}", "", 7000);
                            //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RentTentExpand), 3000);
                        BattlePass.Repository.UpdateReward(player, 49);

                        tentData.RentTime = tentData.RentTime.AddHours(1);                            
                        break;
                    case "name":
                        Trigger.ClientEvent(player, "openInput", LangFunc.GetText(LangType.Ru, DataName.TentName), LangFunc.GetText(LangType.Ru, DataName.NameTentInput), 35, "rentname");
                        break;
                }

            }
            catch (Exception e)
            {
                Log.Write($"callback_menutent Exception: {e.ToString()}");
            }
        }

        public static void UpdateTentLabel(int index, string text = "")
        {
            if (!TentsData.ContainsKey(index)) return;

            var tentData = TentsData[index];

            var player = tentData.player;

            if (text.Length > 0) text = $"{text}\n";

            if (player.IsCharacterData()) tentData.label.Text = LangFunc.GetText(LangType.Ru, DataName.SellerTent, text, player.Name);
            else tentData.label.Text = $"#{index + 1}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="index"></param>

        public static void OpenRentTent(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                
                var frameList = new FrameListData(); 
                frameList.Header = LangFunc.GetText(LangType.Ru, DataName.Tent); 
                frameList.Callback = callback_renttent;
                
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.ArendaTelephone, MoneySystem.Wallet.Format(TentPrice)), index)); // почему-то не уверен

                Players.Popup.List.Repository.Open(player, frameList);   
            }
            catch (Exception e)
            {
                Log.Write($"OpenRentcarMenu Exception: {e.ToString()}");
            }
        }


        private static void callback_renttent(ExtPlayer player, object listItem) /// Никитос Чини 
        {
            try
            {
                if (!(listItem is int))
                    return;
                
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                
                int index = Convert.ToInt32(listItem);

                if (!TentsData.ContainsKey(index)) return;
                
                var tentData = TentsData[index];

                if (sessionData.TentIndex != -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyTentYou), 3000);
                    return;
                }
                else if (tentData.player != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyTent), 3000);
                    return;
                }
                else if (UpdateData.CanIChange(player, TentPrice, true) != 255) return;

                GameLog.Money($"player({characterData.UUID})", $"tent({index})", TentPrice, "rentTent");

                MoneySystem.Wallet.Change(player, -TentPrice);

                //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouRentedTent), 10000);
                Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Tent, LangFunc.GetText(LangType.Ru, DataName.YouRentedTent), DateTime.Now);  
                BattlePass.Repository.UpdateReward(player, 47);
                

                tentData.player = player;

                tentData.UUID = characterData.UUID;

                tentData.RentTime = DateTime.Now.AddHours(1);

                sessionData.TentIndex = index;

                UpdateTentLabel(index);
            }
            catch (Exception e)
            {
                Log.Write($"callback_renttent Exception: {e.ToString()}");
            }
        }


        public static void MinuteTimer()
        {
            var buyToTent = TentsData
                .Where(t => t.Value.player != null)
                .Select(d => d.Key)
                .ToList();

            foreach(var index in buyToTent)
            {
                if (!TentsData.ContainsKey(index)) continue;

                var tentData = TentsData[index];

                if (tentData.RentTime > DateTime.Now) continue;

                RemoveTent(index, "time");
            }
        }

        public static void Disconnect(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            else if (sessionData.TentIndex == -1) return;

            RemoveTent(sessionData.TentIndex, "disconnect");
        }

        public static void RemoveTent(int index, string type)
        {
            try
            {
                if (!TentsData.ContainsKey(index)) return;

                var tentData = TentsData[index];

                var player = tentData.player;

                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                switch (type)
                {
                    case "time":
                        //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TentRentGo), 10000);
                        Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Tent, LangFunc.GetText(LangType.Ru, DataName.TentRentGo), DateTime.Now);
                        break;
                    case "remove":
                        //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TentRentCancel), 10000);
                        Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Tent, LangFunc.GetText(LangType.Ru, DataName.TentRentCancel), DateTime.Now);
                        break;
                    /*case "SafeZoneTent":
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.TentZoneCancel), 10000);
                        break;*/
                }

                var Location = "tent";
                var locationName = $"{Location}_{characterData.UUID}";

                if (Chars.Repository.ItemsData.ContainsKey(locationName) && Chars.Repository.ItemsData[locationName].ContainsKey(Location))
                {
                    foreach (var Item in Chars.Repository.ItemsData[locationName][Location].Values.ToList())
                    {
                        if (Item.ItemId == ItemId.Debug) continue;
                        Chars.Repository.AddItemWarehouse(player, Item, 10000);
                    }
                }

                if (Chars.Repository.ItemsData.ContainsKey(locationName))
                {
                    Chars.Repository.ItemsData.TryRemove(locationName, out _);
                }

                Chars.Repository.ItemsAllClose(locationName);

                if (Chars.Repository.InventoryOtherPlayers.ContainsKey(locationName))
                    Chars.Repository.InventoryOtherPlayers.TryRemove(locationName, out _);

                tentData.player = null;
                tentData.RentTime = DateTime.MinValue;
                tentData.slotToPrice = new Dictionary<int, int>();
                tentData.UUID = -1;

                sessionData.TentIndex = -1;

                UpdateTentLabel(index);

            }
            catch (Exception e)
            {
                Log.Write($"RemoveTent Exception: {e.ToString()}");
            }
        }
        public static int GetUUIDToIndex(int uuid)
        {
            return TentsData
                .Where(t => t.Value.UUID == uuid)
                .Select(d => d.Key)
                .FirstOrDefault();
        }
    }
}
