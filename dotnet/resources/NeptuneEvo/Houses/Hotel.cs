using NeptuneEvo.Core;
using Redage.SDK;
using NeptuneEvo.GUI;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using System;
using Localization;
using NeptuneEvo.Chars;
using NeptuneEvo.Functions;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Players.Popup.List.Models;
using NeptuneEvo.VehicleData.LocalData.Models;

namespace NeptuneEvo.Houses
{
    class Hotel : Script
    {
        private static readonly nLog Log = new nLog("Houses.Hotel");

        public static Vector3[] HotelEnters = new Vector3[4]
        {
            new Vector3(435.7797, 215.2411, 102.0459),
            new Vector3(-1274.113, 315.5634, 64.39182),
            new Vector3(-877.9172, -2178.256, 8.689036),
            new Vector3(416.5617, -1108.615, 28.93262),
        };
        private static Vector3[] CarsGet = new Vector3[4]
        {
            new Vector3(463.2156, 222.7217, 101.9851),
            new Vector3(-1297.033, 251.5936, 61.63777),
            new Vector3(-889.4531, -2180.477, 7.47327),
            new Vector3(407.5778, -1100.066, 28.28551),
        };
        private static Vector3 InteriorDoor = new Vector3(151.2052, -1008.007, -100.12);

        //[ServerEvent(Event.ResourceStart)]
        public void Event_ResourceStart()
        {
            try
            {
                int HotelID = 0;
                foreach (Vector3 pos in HotelEnters)
                {
                    /*Blip blip = (ExtBlip) NAPI.Blip.CreateBlip(pos);
                    blip.ShortRange = true;
                    blip.Sprite = 475;
                    blip.Color = 59;
                    blip.Name = "Hotel";*/

                    Main.CreateBlip(new Main.BlipData(475, LangFunc.GetText(LangType.Ru, DataName.Hotel), pos, 16, true));

                    CustomColShape.CreateCylinderColShape(pos, 1.5f, 5f, 0, ColShapeEnums.EnterHotel, HotelID);

                    NAPI.Marker.CreateMarker(1, pos - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220));
                    NAPI.TextLabel.CreateTextLabel("~w~Отель", pos + new Vector3(0, 0, 0.5), 5f, 0.4f, 0, new Color(255, 255, 255));

                    CustomColShape.CreateCylinderColShape(InteriorDoor, 1.5f, 3, (uint)HotelID, ColShapeEnums.ExitHotel, HotelID);
                    NAPI.Marker.CreateMarker(1, InteriorDoor, new Vector3(), new Vector3(), 1, new Color(255, 255, 255), false, (uint)HotelID);

                    HotelID++;
                }

                HotelID = 0;
                foreach (Vector3 pos in CarsGet)
                {
                    CustomColShape.CreateCylinderColShape(pos, 1.5f, 5f, 0, ColShapeEnums.CarRentHotel, HotelID);

                    NAPI.Marker.CreateMarker(1, pos - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1f, new Color(255, 255, 255, 220));
                    NAPI.TextLabel.CreateTextLabel("~w~Скутер Отеля", pos + new Vector3(0, 0, 0.5), 5f, 0.4f, 0, new Color(255, 255, 255));
                    HotelID++;
                }
            }
            catch (Exception e)
            {
                Log.Write($"Event_ResourceStart Exception: {e.ToString()}");
            }
        }
        [Interaction(ColShapeEnums.EnterHotel)]
        public static void OnEnterHotel(ExtPlayer player, int Index)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            if (characterData.HotelID != -1 && characterData.HotelID == Index) SendToRoom(player, Index);
            else if (characterData.HotelID == -1) OpenHotelBuyMenu(player);
            else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы уже арендовали отель", 3000);
        }

        [Interaction(ColShapeEnums.ExitHotel)]
        public static void OnExitHotel(ExtPlayer player, int Index)
        {
            if (!player.IsCharacterData()) return;
            NAPI.Entity.SetEntityPosition(player, HotelEnters[Index] + new Vector3(0, 0, 1.5));
            Trigger.Dimension(player, 0);
        }

        [Interaction(ColShapeEnums.CarRentHotel)]
        public static void OnCarRentHotel(ExtPlayer player, int Index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            var characterData = player.GetCharacterData();
            if (characterData == null) return;

            if (player.IsInVehicle)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Невозможно использовать, сидя в машине.", 3000);
                return;
            }
            if (Index == characterData.HotelID)
            {
                if (sessionData.HotelData.Car != null)
                {
                    ExtVehicle hotveh = sessionData.HotelData.Car;
                    VehicleStreaming.DeleteVehicle(hotveh);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы успешно отдали скутер.", 3000);
                    sessionData.HotelData.Car = null;
                    return;
                }
                var vehicle = VehicleStreaming.CreateVehicle((uint)VehicleHash.Faggio2, player.Position, player.Heading, 0, 0, "HOTEL", engine: true, locked: true, acc: VehicleAccess.Hotel, petrol: 100);
                sessionData.HotelData.Car = vehicle;
            }
            return;
        }


        public static void Event_OnPlayerDisconnected(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (sessionData.HotelData.Car != null)
                {
                    VehicleStreaming.DeleteVehicle(sessionData.HotelData.Car);
                    sessionData.HotelData.Car = null;
                }
            }
            catch (Exception e)
            {
                Log.Write($"Event_OnPlayerDisconnected Exception: {e.ToString()}");
            }
        }

        public static void SendToRoom(ExtPlayer player, int Index)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.HotelID == -1) return;
                uint dim = (uint)Index;
                NAPI.Entity.SetEntityPosition(player, InteriorDoor + new Vector3(0, 0, 1.12));
                Trigger.Dimension(player, dim);
                characterData.InsideHotelID = characterData.HotelID;
            }
            catch (Exception e)
            {
                Log.Write($"SendToRoom Exception: {e.ToString()}");
            }
        }

        public static void ExtendHotelRent(ExtPlayer player, int hours)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (characterData.HotelID == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не поселены ни в один отель", 3000);
                    return;
                }

                if (characterData.HotelLeft + hours > 10)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Аренда может быть оплачена только на 10 часов", 3000);
                    return;
                }
                int price = Main.HotelRent * hours;
                if (UpdateData.CanIChange(player, price, true) != 255) return;
                MoneySystem.Wallet.Change(player, -price);
                GameLog.Money($"player({characterData.UUID})", $"server", price, $"hotelRent");
                characterData.HotelLeft += hours;
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы продлили аренду на {hours} часов, Вас выселят через {characterData.HotelLeft} часов", 10000);
            }
            catch (Exception e)
            {
                Log.Write($"ExtendHotelRent Exception: {e.ToString()}");
            }
        }

        public static void MoveOutPlayer(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.InsideHotelID != -1) 
                    NAPI.Entity.SetEntityPosition(player, HotelEnters[characterData.InsideHotelID] + new Vector3(0, 0, 1.12));
                characterData.HotelID = -1;
                characterData.HotelLeft = 0;
                characterData.InsideHotelID = -1;
                if (sessionData.HotelData.Car != null)
                {
                    VehicleStreaming.DeleteVehicle(sessionData.HotelData.Car);
                    sessionData.HotelData.Car = null;
                }
            }
            catch (Exception e)
            {
                Log.Write($"MoveOutPlayer Exception: {e.ToString()}");
            }
        }

        public static void OpenHotelBuyMenu(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                
                var frameList = new FrameListData();
                frameList.Header = LangFunc.GetText(LangType.Ru, DataName.Hotel);
                frameList.Callback = callback_hotelbuy;
                
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.HotelPayday), "info"));
                
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.HotelRent, Main.HotelRent), "rent"));

                Players.Popup.List.Repository.Open(player, frameList); 
            }
            catch (Exception e)
            {
                Log.Write($"OpenHotelBuyMenu Exception: {e.ToString()}");
            }
        }
        private static void callback_hotelbuy(ExtPlayer player, object listItem)
        {
            try
            {
                if (!(listItem is string))
                    return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                switch (listItem)
                {
                    case "rent":
                        if (HouseManager.GetHouse(player) != null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы проживаете в доме и не можете арендовать комнату в отеле", 3000);
                            return;
                        }
                        if (UpdateData.CanIChange(player, Main.HotelRent, true) != 255) return;
                        MoneySystem.Wallet.Change(player, -Main.HotelRent);
                        int HotelId = CustomColShape.GetDataToEnum(player, ColShapeEnums.EnterHotel);
                        if (HotelId == (int)ColShapeData.Error) return;
                        GameLog.Money($"player({characterData.UUID})", $"server", Main.HotelRent, $"hotelRent");
                        characterData.HotelID = HotelId;
                        characterData.HotelLeft = 1;
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы арендовали комнату в отеле на 1ч. Продлить аренду можно в телефоне (M)", 3000);
                        SendToRoom(player, HotelId);
                        return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"callback_hotelbuy Exception: {e.ToString()}");
            }
        }

        public static void OpenHotelManageMenu(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                var frameList = new FrameListData();
                frameList.Header = LangFunc.GetText(LangType.Ru, DataName.Hotel);
                frameList.Callback = callback_hotelmanage;

                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.HotelPayday), "info"));
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.ExpandRent), "extend"));
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.Viselitsya), "moveout"));
                Players.Popup.List.Repository.Open(player, frameList); 
            }
            catch (Exception e)
            {
                Log.Write($"OpenHotelManageMenu Exception: {e.ToString()}");
            }
        }
        private static void callback_hotelmanage(ExtPlayer player, object listItem)
        {
            try
            {
                if (!(listItem is string))
                    return;
                
                if (!player.IsCharacterData()) return;

                switch (listItem)
                {
                    case "extend":
                        Trigger.ClientEvent(player, "openInput", $"Продлить аренду ({Main.HotelRent}$/ч)", "Введите количество часов", 1, "extend_hotel_rent");
                        return;
                    case "moveout":
                        MoveOutPlayer(player);
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Вы выселились из отеля", 3000);
                        return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"callback_hotelmanage Exception: {e.ToString()}");
            }
        }
    }
}
