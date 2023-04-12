/* using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Accounts;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Functions;
using NeptuneEvo.GUI;
using NeptuneEvo.Players;
using Redage.SDK;
using System;
using System.Collections.Generic;
using Localization;

namespace NeptuneEvo.Events
{
    public class BoomboxData
    {
        public int OwnerID { get; set; }
        public string MusicURL { get; set; }
        public bool BoomboxPlayStatus { get; set; }
        public ExtColShape BoomboxShape { get; set; }
        public GTANetworkAPI.Object BoomboxObject { get; set; }
        public Vector3 BoomboxPosition { get; set; }
        public ExtTextLabel BoomboxLabel { get; set; }
    }

    class Boombox : Script
    {
        private static readonly nLog Log = new nLog("Events.Boombox");

        public static Dictionary<int, BoomboxData> BoomboxSpots = new Dictionary<int, BoomboxData>();

        public static void UseBoombox(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                Chars.Repository.ItemsClose(player, true);

                if (!FunctionsAccess.IsWorking("UseBoombox"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                else if (Main.IHaveDemorgan(player, true)) return;

                if (sessionData.IsInBoomboxShape != -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Здесь нельзя поставить ваш предмет.", 3000);
                    return;
                }
                else if (BoomboxSpots.ContainsKey(player.Value))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы уже поставили бумбокс.", 3000);
                    return;
                }

                Trigger.ClientEvent(player, "openInput", "Бумбокс", "Ваша ссылка на трек", 256, "boombox");
            }
            catch (Exception e)
            {
                Log.Write($"UseBoombox Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("setFirstBoomboxURL")]
        public static void SetFirstBoomboxURL(ExtPlayer player, string url)
        {
            try
            {
                var accountData = player.GetAccountData();
                if (accountData == null) return;

                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                if (!FunctionsAccess.IsWorking("SetFirstBoomboxURL"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                    return;
                }
                else if (Main.IHaveDemorgan(player, true)) return;

                if (sessionData.IsInBoomboxShape != -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Здесь нельзя поставить ваш предмет.", 3000);
                    return;
                }
                else if (BoomboxSpots.ContainsKey(player.Value))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы уже поставили бумбокс.", 3000);
                    return;
                }

                if (!url.Contains(".mp3"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы вставляете неправильную ссылку.", 3000);
                    return;
                }

                uint dim = UpdateData.GetPlayerDimension(player);
                Vector3 pos = player.Position;

                BoomboxSpots.Add(player.Value, new BoomboxData
                {
                    OwnerID = player.Value,
                    MusicURL = url,
                    BoomboxPlayStatus = false,
                    BoomboxShape = CustomColShape.CreateSphereColShape(player.Position, 10f, UpdateData.GetPlayerDimension(player), ColShapeEnums.BoomboxShape, player.Value),
                    BoomboxObject = (ExtObject)NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_ghettoblast_01"), pos + new Vector3(0.0, 0.0, -0.92), player.Rotation, 255, dim),
                    BoomboxLabel = (ExtTextLabel)(ExtTextLabel) NAPI.TextLabel.CreateTextLabel(Main.StringToU16($"Бумбокс\n~g~(( {player.Name.Replace('_', ' ')} ))"), pos + new Vector3(0.0, 0.0, -0.92), 10F, 0.5F, 0, new Color(255, 255, 255), true, 0),
                    BoomboxPosition = player.Position
                });

                Trigger.ClientEvent(player, "setBoomboxInfo", true, player.Position);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Вы поставили бумбокс. Нажмите Е для взаимодействия с бумбоксом.", 3000);

                foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(player.Position, 10f, UpdateData.GetPlayerDimension(player)))
                {
                    if (!foreachPlayer.IsCharacterData()) continue;
                    sessionData.IsInBoomboxShape = player.Value;
                }

                string message = "!{#636363}[A] " + $"Игрок {player.Name} ({player.Value}) поставил бумбокс с ссылкой: " + url;
                Trigger.SendToAdmins(1, message);
            }
            catch (Exception e)
            {
                Log.Write($"SetFirstBoomboxURL Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("boomboxManageMenu")]
        public static void BoomboxManageMenu(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                if (sessionData.IsInBoomboxShape != -1 && BoomboxSpots.ContainsKey(sessionData.IsInBoomboxShape))
                {
                    if (BoomboxSpots[sessionData.IsInBoomboxShape].OwnerID != player.Value) return;
                    
                    Menu menu = new Menu("boomboxmenu", false, false);
                    menu.Callback = callback_BoomboxManageMenu;

                    Menu.Item menuItem = new Menu.Item("header", Menu.MenuItem.Header);
                    menuItem.Text = "Управление бумбоксом";
                    menu.Add(menuItem);

                    menuItem = new Menu.Item("update_boombox_status", Menu.MenuItem.Button);
                    menuItem.Text = BoomboxSpots[sessionData.IsInBoomboxShape].BoomboxPlayStatus == false ? "Включить" : "Выключить" + " бумбокс";
                    menu.Add(menuItem);

                    menuItem = new Menu.Item("update_boombox_url", Menu.MenuItem.Button);
                    menuItem.Text = "Изменить трек";
                    menu.Add(menuItem);

                    menuItem = new Menu.Item("take_boombox", Menu.MenuItem.Button);
                    menuItem.Text = "Убрать бумбокс";
                    menu.Add(menuItem);

                    menuItem = new Menu.Item("close", Menu.MenuItem.Button);
                    menuItem.Text = LangFunc.GetText(LangType.Ru, DataName.Close);
                    menu.Add(menuItem);

                    menu.Open(player);
                }
            }
            catch (Exception e)
            {
                Log.Write($"BoomboxManageMenu Exception: {e.ToString()}");
            }
        }

        private static void callback_BoomboxManageMenu(ExtPlayer player, Menu menu, Menu.Item item, string eventName, dynamic data)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                
                MenuManager.Close(player);
                if (item.ID == "close") return;

                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                if (sessionData.IsInBoomboxShape != -1 && BoomboxSpots.ContainsKey(sessionData.IsInBoomboxShape))
                {
                    if (BoomboxSpots[sessionData.IsInBoomboxShape].OwnerID != player.Value) return;

                    if (item.ID == "update_boombox_status")
                    {
                        if (BoomboxSpots[sessionData.IsInBoomboxShape].BoomboxPlayStatus == true)
                        {
                            BoomboxSpots[sessionData.IsInBoomboxShape].BoomboxPlayStatus = false;

                            foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(BoomboxSpots[sessionData.IsInBoomboxShape].BoomboxPosition, 10f, UpdateData.GetPlayerDimension(player)))
                            {
                                if (!foreachPlayer.IsCharacterData()) continue;
                                Trigger.ClientEvent(foreachPlayer, "stopBoomboxMusic");
                            }

                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы выключили бумбокс.", 3000);
                        }
                        else
                        {
                            BoomboxSpots[sessionData.IsInBoomboxShape].BoomboxPlayStatus = true;

                            foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(BoomboxSpots[sessionData.IsInBoomboxShape].BoomboxPosition, 10f, UpdateData.GetPlayerDimension(player)))
                            {
                                if (!foreachPlayer.IsCharacterData()) continue;
                                Trigger.ClientEvent(foreachPlayer, "playBoomboxMusic", BoomboxSpots[sessionData.IsInBoomboxShape].MusicURL);
                            }

                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы включили бумбокс.", 3000);
                        }
                    }
                    else if (item.ID == "update_boombox_url") 
                        Trigger.ClientEvent(player, "openInput", "Бумбокс", "Ваша ссылка на трек", 256, "update_boombox_url");
                    else if (item.ID == "take_boombox") 
                        TakeBoombox(player);
                }
            }
            catch (Exception e)
            {
                Log.Write($"callback_BoomboxManageMenu Exception: {e.ToString()}");
            }
        }

        private static void TakeBoombox(ExtPlayer player, bool isExit = false)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                if (sessionData.IsInBoomboxShape != -1 && BoomboxSpots.ContainsKey(sessionData.IsInBoomboxShape))
                {
                    if (BoomboxSpots[sessionData.IsInBoomboxShape].OwnerID != player.Value) return;

                    int currentIndex = sessionData.IsInBoomboxShape;

                    foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(BoomboxSpots[currentIndex].BoomboxPosition, 10f, UpdateData.GetPlayerDimension(player)))
                    {
                        if (!foreachPlayer.IsCharacterData()) continue;

                        var foreachPlayerSessionData = player.GetSessionData();
                        if (foreachPlayerSessionData == null) continue;

                        Trigger.ClientEvent(foreachPlayer, "stopBoomboxMusic");

                        if (foreachPlayerSessionData.IsInBoomboxShape != -1 && foreachPlayerSessionData.IsInBoomboxShape == currentIndex)
                            foreachPlayerSessionData.IsInBoomboxShape = -1;
                    }

                    BoomboxSpots[currentIndex].BoomboxShape.Delete();
                    BoomboxSpots[currentIndex].BoomboxObject.Delete();
                    BoomboxSpots[currentIndex].BoomboxLabel.Delete();

                    BoomboxSpots.Remove(currentIndex);

                    if (sessionData.IsInBoomboxShape != -1)
                        sessionData.IsInBoomboxShape = -1;

                    if (!isExit)
                    {
                        Trigger.ClientEvent(player, "setBoomboxInfo", false);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы убрали бумбокс.", 3000);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"callback_BoomboxManageMenu Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("updateBoomboxURL")]
        public static void UpdateBoomboxURL(ExtPlayer player, string url)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                if (sessionData.IsInBoomboxShape != -1 && BoomboxSpots.ContainsKey(sessionData.IsInBoomboxShape))
                {
                    if (BoomboxSpots[sessionData.IsInBoomboxShape].OwnerID != player.Value) return;

                    if (!url.Contains(".mp3"))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы вставляете неправильную ссылку.", 3000);
                        return;
                    }

                    if (BoomboxSpots[sessionData.IsInBoomboxShape].BoomboxPlayStatus == true)
                    {
                        BoomboxSpots[sessionData.IsInBoomboxShape].BoomboxPlayStatus = false;
                        foreach (ExtPlayer foreachPlayer in Main.GetPlayersInRadiusOfPosition(BoomboxSpots[sessionData.IsInBoomboxShape].BoomboxPosition, 10f, UpdateData.GetPlayerDimension(player)))
                        {
                            if (!foreachPlayer.IsCharacterData()) continue;
                            Trigger.ClientEvent(foreachPlayer, "stopBoomboxMusic");
                        }
                    }

                    BoomboxSpots[sessionData.IsInBoomboxShape].MusicURL = url;

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы успешно сменили трек.", 3000);
                }
            }
            catch (Exception e)
            {
                Log.Write($"UpdateBoomboxURL Exception: {e.ToString()}");
            }
        }

        [Interaction(ColShapeEnums.BoomboxShape, In: true)]
        public static void EnterBoomboxShape(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                if (BoomboxSpots.ContainsKey(index))
                {
                    sessionData.IsInBoomboxShape = index;
                    if (BoomboxSpots[index].BoomboxPlayStatus == true) Trigger.ClientEvent(player, "playBoomboxMusic", BoomboxSpots[index].MusicURL);
                }
            }
            catch (Exception e)
            {
                Log.Write($"EnterBoomboxShape Exception: {e.ToString()}");
            }
        }

        [Interaction(ColShapeEnums.BoomboxShape, Out: true)]
        public static void OutBoomboxShape(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                if (sessionData.IsInBoomboxShape == player.Value) TakeBoombox(player, true);
                else sessionData.IsInBoomboxShape = -1;

                Trigger.ClientEvent(player, "stopBoomboxMusic");
            }
            catch (Exception e)
            {
                Log.Write($"OutBoomboxShape Exception: {e.ToString()}");
            }
        }

        public static void OnPlayerDisconnected(ExtPlayer player)
        {
            try
            {
                TakeBoombox(player, true);
            }
            catch (Exception e)
            {
                Log.Write($"Event_PlayerDisconnected Exception: {e.ToString()}");
            }
        }
    }
}
*/