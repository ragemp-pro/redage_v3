using GTANetworkAPI;
using NeptuneEvo.Handles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Localization;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.Fractions;
using NeptuneEvo.GUI;
using Redage.SDK;
using MySqlConnector;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Houses;
using NeptuneEvo.Functions;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Players.Popup.List.Models;
using NeptuneEvo.Quests;
using NeptuneEvo.Table.Tasks.Models;
using NeptuneEvo.Table.Tasks.Player;

namespace NeptuneEvo.Core
{
    public class SafeMain : Script
    {
        // config, use meta.xml instead
        public static int SafeRespawnTime = 10800;
        public static int SafeMinLoot = 150;
        public static int SafeMaxLoot = 500;
        public static string SafeDir = "Safes";
        public static int MaxMoneyInBag = 10000;
        public static DateTime NextRobbery = new DateTime();
        public static int NowRobberyID = -1;
        // main safe door
        public static bool isCracking = false;
        private static ExtTextLabel label;
        private static int secondsLeft = 0;
        public static bool isOpen = false;
        public static bool canBeClosed = true;
        private static GTANetworkAPI.Object safeDrill;
        private static string timer = null;

        // other variables

        public static List<Safe> Safes = new List<Safe>();
        public static Random SafeRNG = new Random();

        /*public static List<Vector3> moneyFlowPoints = new List<Vector3>()
        {
            new Vector3(1395.184, 3613.144, 34.9892),
            new Vector3(166.6278, 2229.249, 90.87845),
            new Vector3(2887.687, 4387.17, 50.85578),
            new Vector3(2192.614, 5596.246, 53.89177),
            new Vector3(-215.4299, 6445.921, 31.43351),
        };
        private static List<string> moneyFlowers = new List<string>()
        {
            "Caleb Baker",
            "Matthew Allen",
            "Owen Nelson",
            "Daniel Roberts",
            "Michael Turner",
        };*/
        //Продажа салютов
        public static List<Vector3> salutePoints = new List<Vector3>()
        {
            new Vector3(-602.0729, -347.30234, 35.24108),
        };
        private static List<string> saluteBotName = new List<string>()
        {
            "Продавец фейерверков",
        };

        public object LogCat { get; private set; }
        public static readonly nLog Log = new nLog("Core.Robbery");

        #region Methods
        public static Vector3 XYInFrontOfPoint(Vector3 pos, float angle, float distance)
        {
            angle *= (float)Math.PI / 180;
            pos.X += (distance * (float)Math.Sin(-angle));
            pos.Y += (distance * (float)Math.Cos(-angle));
            return pos;
        }
        #endregion

        #region Safe Methods
        public static void CreateSafe(int i, Vector3 position, float rotation, int minamount, int maxamount, string address)
        {
            try
            {
                Safe new_safe = new Safe(i, position, rotation, minamount, maxamount, address);
                Safes.Add(new_safe);
                string string_pos = JsonConvert.SerializeObject(position);

                using MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "INSERT INTO safes (minamount, maxamount, pos, address, rotation) VALUES (@val0,@val1,@val2,@val3,@val4)"
                };
                cmd.Parameters.AddWithValue("@val0", minamount);
                cmd.Parameters.AddWithValue("@val1", maxamount);
                cmd.Parameters.AddWithValue("@val2", string_pos);
                cmd.Parameters.AddWithValue("@val3", address);
                cmd.Parameters.AddWithValue("@val4", rotation);
                MySQL.Query(cmd);
                new_safe.Create();
            }
            catch (Exception e)
            {
                Log.Write($"CreateSafe Exception: {e.ToString()}");
            }
        }

        public static void RemoveSafe(int ID)
        {
            try
            {
                Safe safe = Safes.FirstOrDefault(s => s.ID == ID);
                if (safe == null) return;
                safe.Destroy(true);
                Safes.Remove(safe);
                using MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "DELETE FROM safes WHERE id=@val0"
                };
                cmd.Parameters.AddWithValue("@val0", ID);
                MySQL.Query(cmd);
            }
            catch (Exception e)
            {
                Log.Write($"RemoveSafe Exception: {e.ToString()}");
            }
        }
        #endregion

        #region Events
        public static void startSafeDoorCracking(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                ItemId Bags = Chars.Repository.GetItemData(player, "accessories", 8).ItemId;
                if (Bags != ItemId.BagWithDrill)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет дрели для взлома", 3000);
                    return;
                }
                if (isCracking)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Взлом уже начат", 3000);
                    return;
                }
                if (isOpen)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Дверь в хранилище уже открыта", 3000);
                    return;
                }
                if (timer != null) return;
                Chars.Repository.RemoveIndex(player, "accessories", 8);
                isCracking = true;
                safeDrill = NAPI.Object.CreateObject(-443429795, new Vector3(253.9534, 225.2, 102.22), new Vector3(0, 0, -18), 255, 0);
                label = (ExtTextLabel) NAPI.TextLabel.CreateTextLabel("~r~8:00", new Vector3(253.9534, 225.2, 102.22), 4F, 0.3F, 0, new Color(255, 255, 255));
                secondsLeft = 480;
                timer = Timers.Start("DoorCracking", 1000, () => updateDoorCracking(), true);
                canBeClosed = false;
                Manager.sendFractionMessage((int)Fractions.Models.Fractions.CITY, "Кто-то пытается взломать дверь в хранилище мэрии.");
                Manager.sendFractionMessage((int)Fractions.Models.Fractions.POLICE, "Кто-то пытается взломать дверь в хранилище мэрии.");
                Manager.sendFractionMessage((int)Fractions.Models.Fractions.SHERIFF, "Кто-то пытается взломать дверь в хранилище мэрии.");
                Manager.sendFractionMessage((int)Fractions.Models.Fractions.FIB, "Кто-то пытается взломать дверь в хранилище мэрии.");
                Manager.sendFractionMessage((int)Fractions.Models.Fractions.ARMY, "Кто-то пытается взломать дверь в хранилище мэрии.");
            }
            catch (Exception e)
            {
                Log.Write($"startSafeDoorCracking Exception: {e.ToString()}");
            }
        }

        private static void updateDoorCracking()
        {
            try
            {
                secondsLeft--;
                if (secondsLeft == 0)
                {
                    try
                    {
                        if (label != null && label.Exists) label.Delete();
                        label = null;
                        if (safeDrill != null && safeDrill.Exists) safeDrill.Delete();
                        safeDrill = null;
                    }
                    catch (Exception e)
                    {
                        Log.Write($"updateDoorCracking #2 Exception: {e.ToString()}");
                    }
                    isCracking = false;
                    Timers.StartOnce("bankTimer", 600000, () =>
                    {
                        canBeClosed = true;
                    });
                    Doormanager.SetDoorLocked(2, true, 0.5f);
                    if (timer != null)
                    {
                        Timers.Stop(timer);
                        timer = null;
                    }
                    return;
                }
                int minutes = secondsLeft / 60;
                int seconds = secondsLeft % 60;
                label.Text = $"~r~{minutes}:{seconds}";
            }
            catch (Exception e)
            {
                Log.Write($"updateDoorCracking Exception: {e.ToString()}");
            }
        }

        [ServerEvent(Event.PlayerEnterVehicle)]
        public void onPlayerEnterVehicle(ExtPlayer player, ExtVehicle vehicle, sbyte seatid)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                ItemId Bags = Chars.Repository.GetItemData(player, "accessories", 8).ItemId;
                if ((Bags == ItemId.BagWithDrill || Bags == ItemId.BagWithMoney) && player.VehicleSeat == (int)VehicleSeat.Driver)
                {
                    VehicleManager.WarpPlayerOutOfVehicle(player);
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете сесть на водительское место с {Chars.Repository.ItemsInfo[Bags].Name}.", 3000);
                }
                if (!characterData.Gender)
                {
                    InventoryItemData PlayerTopUp = Chars.Repository.GetItemData(player, "accessories", 5);
                    Dictionary<string, int> PlayerTopUsessionData = PlayerTopUp.GetData();
                    if (PlayerTopUsessionData["Variation"] == 394 && !PlayerTopUp.GetGender())
                    {
                        PlayerTopUp.Data = $"407_{PlayerTopUsessionData["Texture"]}_False";
                        Chars.Repository.SetItemData(player, "accessories", 5, PlayerTopUp, true);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"onPlayerEnterVehicle Exception: {e.ToString()}");
            }
        }
        [ServerEvent(Event.PlayerExitVehicle)]
        public void onPlayerExitVehicle(ExtPlayer player, ExtVehicle vehicle)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (!characterData.Gender)
                {
                    InventoryItemData PlayerTopUp = Chars.Repository.GetItemData(player, "accessories", 5);
                    Dictionary<string, int> PlayerTopUsessionData = PlayerTopUp.GetData();
                    if (PlayerTopUsessionData["Variation"] == 407 && !PlayerTopUp.GetGender())
                    {
                        PlayerTopUp.Data = $"394_{PlayerTopUsessionData["Texture"]}_False";
                        Chars.Repository.SetItemData(player, "accessories", 5, PlayerTopUp, true);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"Event_OnPlayerExitVehicle Exception: {e.ToString()}");
            }
        }

        [ServerEvent(Event.ResourceStart)]
        public void SafeCracker_Init()
        {
            try
            {
                /*for (int b = 0; b < moneyFlowPoints.Count; b++)
                {
                    CustomColShape.CreateCylinderColShape(moneyFlowPoints[b], 1.5F, 2, 0, ColShapeEnums.BlackMarket);

                    Main.CreateBlip(new Main.BlipData(586, "Чёрный рынок", moneyFlowPoints[b], 32, true));

                    NAPI.TextLabel.CreateTextLabel(Main.StringToU16($"Нажмите \n'Взаимодействие'\n~g~{moneyFlowers[b]}"), moneyFlowPoints[b] + new Vector3(0, 0, 1.125), 5F, 0.8F, 0, new Color(255, 255, 255));
                }*/

                CustomColShape.CreateCylinderColShape(new Vector3(-2.1323678, -1821.9778, 29.543238), 1.5F, 2, 0, ColShapeEnums.BlackMarket);

                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("Нажмите \n'Взаимодействие'\n~g~Caleb Baker"), new Vector3(-2.1323678, -1821.9778, 29.543238), 5F, 0.8F, 0, new Color(255, 255, 255));

                for (int b = 0; b < salutePoints.Count; b++)
                {
                    PedSystem.Repository.CreateQuest("ig_ramp_hic", salutePoints[b], 105.93525f, 0, title: "~y~NPC~w~ Сергей\nПродавец фейерверков", colShapeEnums: ColShapeEnums.SaluteShop);
                    //CustomColShape.CreateCylinderColShape(salutePoints[b], 1.5F, 2, 0, ColShapeEnums.SaluteShop);

                    Main.CreateBlip(new Main.BlipData(654, "Салют", salutePoints[b], 32, true));

                    //NAPI.TextLabel.CreateTextLabel(Main.StringToU16($"Нажмите \n'Взаимодействие'\n~g~{saluteBotName[b]}"), salutePoints[b] + new Vector3(0, 0, 1.125), 5F, 0.8F, 0, new Color(255, 255, 255));
                }


                using MySqlCommand cmd = new MySqlCommand()
                {
                    CommandText = "SELECT * FROM `safes`"
                };
                using DataTable result = MySQL.QueryRead(cmd);
                if (result == null || result.Rows.Count == 0)
                {
                    Log.Write("DB return null result.", nLog.Type.Warn);
                    return;
                }
                int i = 0;
                foreach (DataRow Row in result.Rows)
                {
                    try
                    {
                        Vector3 safePos = JsonConvert.DeserializeObject<Vector3>(Row["pos"].ToString());
                        float safeRot = Convert.ToSingle(Row["rotation"]);

                        Safe safe = new Safe(i, safePos, safeRot, Convert.ToInt32(Row["minamount"]), Convert.ToInt32(Row["maxamount"]), Row["address"].ToString());
                        Safes.Add(safe);
                        safe.Create();
                        i++;
                    }
                    catch (Exception e)
                    {
                        Log.Write($"SafeCracker_Init Foreach Exception: {e.ToString()}");
                        continue;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"SafeCracker_Init Exception: {e.ToString()}");
            }

        }
        [RemoteEvent("dialPress")]
        public static void openSafe(ExtPlayer player, params object[] arguments)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (sessionData.TempSafeID == -1)
                {
                    HouseManager.houseHijack(player, arguments);
                    return;
                }
                Safe safe = Safes.FirstOrDefault(s => s.ID == sessionData.TempSafeID);
                if (safe == null) return;
                if (sessionData.CurrentStage == -1)
                {
                    Trigger.ClientEvent(player, "dial", "close");
                    return;
                }

                if (!(bool)arguments[0])
                {
                    Trigger.ClientEvent(player, "dial", "close");
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Неправильный пароль", 2000);
                    Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", ItemId.Lockpick, 1);
                    safe.Occupier = null;
                }
                else
                {
                    int stage = sessionData.CurrentStage;
                    if (stage == 2)
                    {
                        safe.SafeLoot = SafeRNG.Next(safe.MinAmount * Main.ServerSettings.MoneyMultiplier, safe.MaxAmount * Main.ServerSettings.MoneyMultiplier);
                        safe.SetDoorOpen(true);
                        safe.Occupier = null;
                        Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", ItemId.Lockpick, 1);
                        Trigger.ClientEvent(player, "dial", "close");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно взломали сейф", 2000);
                        player.Eval($"mp.game.audio.playSoundFrontend(-1, \"Drill_Pin_Break\", \"DLC_HEIST_FLEECA_SOUNDSET\", true);");
                    }
                    else
                    {
                        stage++;
                        sessionData.CurrentStage = stage;
                        Trigger.ClientEvent(player, "dial", "open", safe.LockAngles[stage], true);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы подобрали {stage} из 3 паролей", 2000);
                        player.Eval($"mp.game.audio.playSoundFrontend(-1, \"Player_Enter_Line\", \"GTAO_FM_Cross_The_Line_Soundset\", true);");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"openSafe Exception: {e.ToString()}");
            }
        }
        [Interaction(ColShapeEnums.HouseSafe)]
        public static void OnHouseSafe(ExtPlayer player, int _)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                if (sessionData.TempSafeID == -1) return;
                Safe safe = Safes.FirstOrDefault(s => s.ID == sessionData.TempSafeID);
                if (safe == null) return;
                if (safe.IsOpen) safe.Loot(player);
                else
                {
                    if (!player.HasSharedData("IS_MASK") || !player.GetSharedData<bool>("IS_MASK"))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Взлом возможен только в маске", 3000);
                        return;
                    }
                    if (safe.Occupier != null && NAPI.Player.GetPlayerFromHandle(safe.Occupier) != null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Этот сейф уже взламывают", 3000);
                        return;
                    }
                    var fracId = player.GetFractionId();
                    if (Manager.FractionTypes[fracId] != FractionsType.Gangs && Manager.FractionTypes[fracId] != FractionsType.Bikers)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Доступно только для банд", 5000);
                        return;
                    }
                    ItemStruct lockpick = Chars.Repository.isItem(player, "inventory", ItemId.Lockpick);
                    int count = (lockpick == null) ? 0 : lockpick.Item.Count;
                    if (count == 0)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Для взлома нужна отмычка. Купить её можно на черном рынке. (GPS-Прочее)", 5000);
                        return;
                    }
                    if (DateTime.Now < NextRobbery && NowRobberyID != safe.ID)
                    {
                        long ticks = NextRobbery.Ticks - DateTime.Now.Ticks;
                        if (ticks <= 0) return;
                        DateTime g = new DateTime(ticks);
                        if (g.Hour >= 1) Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Попробуйте через {g.Hour}:{g.Minute}:{g.Second}", 3000);
                        else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Попробуйте через {g.Minute}:{g.Second}", 3000);
                        return;
                    }

                    List<ExtPlayer> nearestPlayers = Main.GetPlayersInRadiusOfPosition(player.Position, 7);
                    int gangsters = 0;
                    foreach (ExtPlayer foreachPlayer in nearestPlayers)
                    {
                        var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                        if (foreachMemberFractionData == null) 
                            continue;
                        
                        if (player == foreachPlayer) 
                            continue;
                        
                        if (Manager.FractionTypes[foreachMemberFractionData.Id] == FractionsType.Gangs || Manager.FractionTypes[foreachMemberFractionData.Id] == FractionsType.Bikers) gangsters++;
                    }
                    if (gangsters == 0)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Для начала нужен минимум еще 1 человек.", 3000);
                        return;
                    }

                    safe.Occupier = player;
                    sessionData.CurrentStage = 0;
                    Trigger.ClientEvent(player, "dial", "open", safe.LockAngles[0]);
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"С минуты на минуту сюда прибудут копы.", 3000);
                    Manager.sendFractionMessage((int)Fractions.Models.Fractions.POLICE, "!{#F08080}[F] " + $"Сейф №{safe.ID} по адресу {safe.Address} пытаются взломать.", true);
                    Manager.sendFractionMessage((int)Fractions.Models.Fractions.SHERIFF, "!{#F08080}[F] " + $"Сейф №{safe.ID} по адресу {safe.Address} пытаются взломать.", true);
                    Manager.sendFractionMessage((int)Fractions.Models.Fractions.FIB, "!{#F08080}[F] " + $"Сейф №{safe.ID} по адресу {safe.Address} пытаются взломать.", true);

                    if (NowRobberyID != safe.ID) NextRobbery = DateTime.Now.AddMinutes(15);
                    NowRobberyID = safe.ID;

                    if (DateTime.Now >= safe.BlipSet)
                    {
                        safe.Blip = (ExtBlip) NAPI.Blip.CreateBlip(0, safe.Position, 1, 38, "Robbery", 0, 0, true, 0, 0);
                        safe.Blip.Transparency = 0;
                        Police.PoliceSafesCalls[safe.ID] = safe.Blip;
                        foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                        {
                            var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                            if (foreachMemberFractionData == null) 
                                continue;

                            if (!Configs.IsFractionPolic(foreachMemberFractionData.Id)) 
                                continue;

                            Trigger.ClientEvent(foreachPlayer, "changeBlipAlpha", safe.Blip, 255);
                        }
                        safe.BlipSet = DateTime.Now.AddMinutes(15);
                        player.AddTableScore(TableTaskId.Item32);
                        NAPI.Task.Run(() =>
                        {
                            try
                            {
                                if (safe.Blip != null && safe.Blip.Exists) safe.Blip.Delete();
                                safe.Blip = null;
                            }
                            catch (Exception e)
                            {
                                Log.Write($"interactSafe Task Exception: {e.ToString()}");
                            }
                        }, 900000);
                    }

                    if (player.HasSharedData("IS_MASK") && !player.GetSharedData<bool>("IS_MASK"))
                    {
                        WantedLevel wantedLevel = new WantedLevel(4, LangFunc.GetText(LangType.Ru, DataName.Police), DateTime.Now, "Ограбление сейфа");
                        Police.setPlayerWantedLevel(player, wantedLevel);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"OnHouseSafe Exception: {e.ToString()}");
            }
        }
        [Interaction(ColShapeEnums.HouseSafe, In: true)]
        public static void InHouseSafe(ExtPlayer player, int index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            sessionData.TempSafeID = index;
        }
        [Interaction(ColShapeEnums.HouseSafe, Out: true)]
        public static void OutHouseSafe(ExtPlayer player, int index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            Safe safe = Safes.FirstOrDefault(s => s.ID == index);

            if (safe != null && player == safe.Occupier)
                safe.Occupier = null;

            Trigger.ClientEvent(player, "dial", "close");
            sessionData.TempSafeID = -1;

        }
        public static void MoneyFlow(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                InventoryItemData Bags = Chars.Repository.GetItemData(player, "accessories", 8);
                if (Bags.ItemId != ItemId.BagWithMoney) return;
                Chars.Repository.RemoveIndex(player, "accessories", 8);
                Wallet.Change(player, (int)(Convert.ToInt32(Bags.Data) * 0.97));
                GameLog.Money($"server", $"player({characterData.UUID})", (int)(Convert.ToInt32(Bags.Data) * 0.97), $"moneyFlow");
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы отмыли {(int)(Convert.ToInt32(Bags.Data) * 0.97)}$. Человек забрал {(int)(Convert.ToInt32(Bags.Data) * 0.03)}$ за свои услуги", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"MoneyFlow Exception: {e.ToString()}");
            }
        }

        public static void SafeCracker_Disconnect(ExtPlayer player, DisconnectionType type, string reason)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                if (Chars.Repository.GetItemData(player, "accessories", 8).ItemId == ItemId.BagWithMoney) 
                    Chars.Repository.ItemsDropToIndex(player, "accessories", 8);

                Safe safe = Safes.FirstOrDefault(s => s.Occupier == player);
                if (safe != null)
                {
                    Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", ItemId.Lockpick, 1);
                    safe.Occupier = null;
                }
            }
            catch (Exception e)
            {
                Log.Write($"SafeCracker_Disconnect Exception: {e.ToString()}");
            }
        }

        [ServerEvent(Event.ResourceStop)]
        public void SafeCracker_Exit()
        {
            try
            {
                foreach (Safe safe in Safes) safe.Destroy();
                Safes.Clear();
            }
            catch (Exception e)
            {
                Log.Write($"SafeCracker_Exit Exception: {e.ToString()}");
            }
        }
        #endregion

        #region commands
        public static void CMD_CreateSafe(ExtPlayer player, int id, float distance, int minamount, int maxamount, string address)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.createsafe)) return;
                Safe safe = Safes.FirstOrDefault(s => s.ID == id);
                if (safe != null)
                {
                    NAPI.Chat.SendChatMessageToPlayer(player, "~r~[Ошибка] ~w~Сейф с таким ID уже существует.");
                    return;
                }

                Vector3 position = XYInFrontOfPoint(player.Position, player.Rotation.Z, distance) - new Vector3(0.0, 0.0, 0.25);
                CreateSafe(id, position, player.Rotation.Z, minamount, maxamount, address);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_CreateSafe Exception: {e.ToString()}");
            }
        }

        public static void CMD_RemoveSafe(ExtPlayer player)
        {
            try
            {
                if (!CommandsAccess.CanUseCmd(player, AdminCommands.removesafe)) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (sessionData.TempSafeID == -1)
                {
                    Trigger.SendChatMessage(player, "~r~[Ошибка] ~w~Вы должны быть возле сейфа.");
                    return;
                }

                RemoveSafe(sessionData.TempSafeID);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_RemoveSafe Exception: {e.ToString()}");
            }
        }
        #endregion
        
        public static void onPlayerDeathHandler(ExtPlayer player, ExtPlayer entityKiller, uint weapon)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                ItemId Bags = Chars.Repository.GetItemData(player, "accessories", 8).ItemId;
                if (Bags == ItemId.BagWithMoney || Bags == ItemId.BagWithDrill) Chars.Repository.ItemsDropToIndex(player, "accessories", 8);
                if (sessionData.TempSafeID == -1) return;
                Safe safe = Safes.FirstOrDefault(s => s.ID == sessionData.TempSafeID);
                if (safe == null) return;
                safe.Occupier = null;
                return;
            }
            catch (Exception e)
            {
                Log.Write($"onPlayerDeathHandler Exception: {e.ToString()}");
            }
        }

        #region Menus
        [Interaction(ColShapeEnums.BlackMarket)]
        public static void OnBlackMarket(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (Manager.FractionDataMats.Count == 0) return;
                List<int> ListItems = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 69, 78, 79, 80, 81};

                List<Manager.FracMatsData> _JsonData = new List<Manager.FracMatsData>();

                foreach (int i in ListItems)
                {
                    if (Manager.FractionDataMats.ContainsKey(i)) 
                        _JsonData.Add(Manager.FractionDataMats[i]);
                }

                Trigger.ClientEvent(player, "client.sm.openBlack", JsonConvert.SerializeObject(_JsonData));
            }
            catch (Exception e)
            {
                Log.Write($"OnBlackMarket Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.sm.black")]
        public static void callback_moneyflow(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                BattlePass.Repository.UpdateReward(player, 153);
                switch (index)
                {
                    case 0:
                        MoneyFlow(player);
                        return;
                    case 1:
                        if (Chars.Repository.isFreeSlots(player, ItemId.BagWithDrill) != 0) return;
                        else if (UpdateData.CanIChange(player, Main.BlackMarketDrill, true) != 255) return;
                        Chars.Repository.ChangeAccessoriesItem(player, 8, "", true, ItemId.BagWithDrill);
                        Chars.Repository.AccessoriesUse(player, 8);
                        ClothesComponents.UpdateClothes(player);
                        GameLog.Money($"player({characterData.UUID})", $"server", Main.BlackMarketDrill, $"buyMavr(drill)");
                        Wallet.Change(player, -Main.BlackMarketDrill);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили сумку с дрелью для ограблений", 3000);
                        return;
                    case 2:
                        if (characterData.Money < Main.BlackMarketLockPick)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно денег", 3000);
                            return;
                        }
                        else if (Chars.Repository.isFreeSlots(player, ItemId.Lockpick) != 0) return;
                        Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Lockpick, 1);
                        Wallet.Change(player, -Main.BlackMarketLockPick);
                        GameLog.Money($"player({characterData.UUID})", $"server", Main.BlackMarketLockPick, $"buyMavr(lockpick)");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили отмычку для замков", 3000);
                        return;
                    case 3:
                        if (UpdateData.CanIChange(player, Main.BlackMarketArmyLockPick, true) != 255) return;
                        else if (Chars.Repository.isFreeSlots(player, ItemId.ArmyLockpick) != 0) return;
                        Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.ArmyLockpick, 1);
                        Wallet.Change(player, -Main.BlackMarketArmyLockPick);
                        GameLog.Money($"player({characterData.UUID})", $"server", Main.BlackMarketArmyLockPick, $"buyMavr(armylockpick)");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили военную отмычку", 3000);
                        return;
                    case 4:
                        if (UpdateData.CanIChange(player, Main.BlackMarketCuffs, true) != 255) return;
                        else if (Chars.Repository.isFreeSlots(player, ItemId.Cuffs) != 0) return;
                        Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Cuffs, 1);
                        Wallet.Change(player, -Main.BlackMarketCuffs);
                        GameLog.Money($"player({characterData.UUID})", $"server", Main.BlackMarketCuffs, $"buyMavr(cuffs)");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили стяжки для рук", 3000);
                        return;
                    case 5:
                        if (UpdateData.CanIChange(player, Main.BlackMarketPocket, true) != 255) return;
                        else if (Chars.Repository.isFreeSlots(player, ItemId.Pocket) != 0) return;
                        Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Pocket, 1);
                        Wallet.Change(player, -Main.BlackMarketPocket);
                        GameLog.Money($"player({characterData.UUID})", $"server", Main.BlackMarketPocket, $"buyMavr(pocket)");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили мешок на голову", 3000);
                        return;
                    case 6:
                        if (characterData.WantedLVL == null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не находитесь в розыске", 3000);
                            return;
                        }
                        else if (UpdateData.CanIChange(player, Main.BlackMarketWanted, true) != 255) return;
                        Wallet.Change(player, -Main.BlackMarketWanted);
                        GameLog.Money($"player({characterData.UUID})", $"server", Main.BlackMarketWanted, $"buyMavr(wanted)");
                        characterData.WantedLVL.Level--;
                        if (characterData.WantedLVL.Level == 0) characterData.WantedLVL = null;
                        Police.setPlayerWantedLevel(player, characterData.WantedLVL);
                        return;
                    case 7:
                        Trigger.ClientEvent(player, "client.sm.exit");
                        Trigger.ClientEvent(player, "openDialog", "CONFIRM_BUY_BODYARMOUR", "Вы действительно хотите скрафтить бронежилет, стомостью 150 материалов?");
                        return;
                    case 69:
                        if (!sessionData.CuffedData.Cuffed)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"На Вас нет наручников", 3000);
                            return;
                        }
                        else if (UpdateData.CanIChange(player, Main.BlackMarketUnCuff, true) != 255) return;
                        Wallet.Change(player, -Main.BlackMarketUnCuff);
                        GameLog.Money($"player({characterData.UUID})", $"server", Main.BlackMarketUnCuff, $"buyMavr(uncuff)");
                        FractionCommands.unCuffPlayer(player);
                        sessionData.CuffedData.CuffedByCop = false;
                        sessionData.CuffedData.CuffedByMafia = false;
                        return;
                    case 78:
                        if (characterData.Licenses[6]) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас уже есть лицензия на оружие", 3000); 
                            return; 
                        }
                        else if (UpdateData.CanIChange(player, Main.BlackMarketGunLic, true) != 255) return;
                        Wallet.Change(player, -Main.BlackMarketGunLic);
                        qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage33, 2, isUpdateHud: true);
                        qMain.UpdateQuestsComplete(player, Zdobich.QuestName, (int) zdobich_quests.Stage33, true);
                        GameLog.Money($"player({characterData.UUID})", $"server", Main.BlackMarketGunLic, $"buyMavr(gunlic)");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили лицензию на оружие", 3000); 
                        characterData.Licenses[6] = true;
                        return;
                    case 79:
                        if (characterData.Licenses[7]) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас уже есть мед. карта", 3000); 
                            return; 
                        }
                        else if (UpdateData.CanIChange(player, Main.BlackMarketMedCard, true) != 255) return;
                        Wallet.Change(player, -Main.BlackMarketMedCard);
                        GameLog.Money($"player({characterData.UUID})", $"server", Main.BlackMarketMedCard, $"buyMavr(medcard)");
                        qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage31, 2, isUpdateHud: true);
                        qMain.UpdateQuestsComplete(player, Zdobich.QuestName, (int) zdobich_quests.Stage31, true);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили мед. карту", 3000); 
                        characterData.Licenses[7] = true;
                        return;
                    case 80:
                        if (Chars.Repository.isItem(player, "inventory", ItemId.QrFake) != null) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouAlreadyHaveQrMavr), 5000); 
                            return; 
                        }
                        else if (UpdateData.CanIChange(player, Main.BlackQrFake, true) != 255) return;
                        
                        if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.QrFake, 1) == -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                        
                        Wallet.Change(player, -Main.BlackQrFake);
                        GameLog.Money($"player({characterData.UUID})", $"server", Main.BlackQrFake, $"buyMavr(qr)");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили поддельный QR-код", 5000);
                        return;
                    case 81:
                        if (Chars.Repository.isItem(player, "inventory", ItemId.RadioInterceptor) != null) 
                        { 
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас уже есть Радиоперехватчик.", 5000); 
                            return; 
                        }
                        else if (UpdateData.CanIChange(player, Main.BlackRadioInterceptord, true) != 255) return;
                        
                        if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.RadioInterceptor, 1) == -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                        
                        Wallet.Change(player, -Main.BlackRadioInterceptord);
                        GameLog.Money($"player({characterData.UUID})", $"server", Main.BlackRadioInterceptord, $"buyMavr(RadioInterceptor)");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили Радиоперехватчик", 5000);
                        return;
                    default:
                        // Not supposed to end up here. 
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"callback_moneyflow Exception: {e.ToString()}");
            }
        }
        [Interaction(ColShapeEnums.SaluteShop)]
        public static void OnSaluteShop(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (Manager.FractionDataMats.Count == 0) return;
                List<int> ListItems = new List<int>() { 70, 71, 72, 73 };

                List<Manager.FracMatsData> _JsonData = new List<Manager.FracMatsData>();

                foreach (int i in ListItems)
                {
                    if (Manager.FractionDataMats.ContainsKey(i)) _JsonData.Add(Manager.FractionDataMats[i]);
                }

                Trigger.ClientEvent(player, "client.sm.openSaluteShop", JsonConvert.SerializeObject(_JsonData));
            }
            catch (Exception e)
            {
                Log.Write($"OnSaluteShop Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.sm.saluteShop")]
        public static void callback_SaluteShop(ExtPlayer player, int index)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                BattlePass.Repository.UpdateReward(player, 150);
                switch (index)
                {
                    case 70:
                        if (UpdateData.CanIChange(player, Main.PricesSettings.FireworkPrices[0], true) != 255) return;
                        else if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Firework1, 1) == -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                        Wallet.Change(player, -Main.PricesSettings.FireworkPrices[0]);
                        GameLog.Money($"player({characterData.UUID})", $"server", Main.PricesSettings.FireworkPrices[0], $"buySaluteShop(1)");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили фейерверк", 3000);
                        BattlePass.Repository.UpdateReward(player, 28);
                        return;
                    case 71:
                        if (UpdateData.CanIChange(player, Main.PricesSettings.FireworkPrices[1], true) != 255) return;
                        else if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Firework2, 1) == -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                        Wallet.Change(player, -Main.PricesSettings.FireworkPrices[1]);
                        GameLog.Money($"player({characterData.UUID})", $"server", Main.PricesSettings.FireworkPrices[1], $"buySaluteShop(2)");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили фейерверк", 3000);
                        BattlePass.Repository.UpdateReward(player, 28);
                        return;
                    case 72:
                        if (UpdateData.CanIChange(player, Main.PricesSettings.FireworkPrices[2], true) != 255) return;
                        else if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Firework3, 1) == -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                        Wallet.Change(player, -Main.PricesSettings.FireworkPrices[2]);
                        GameLog.Money($"player({characterData.UUID})", $"server", Main.PricesSettings.FireworkPrices[2], $"buySaluteShop(3)");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили фейерверк", 3000);
                        BattlePass.Repository.UpdateReward(player, 28);
                        return;
                    case 73:
                        if (UpdateData.CanIChange(player, Main.PricesSettings.FireworkPrices[3], true) != 255) return;
                        else if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Firework4, 1) == -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                            return;
                        }
                        Wallet.Change(player, -Main.PricesSettings.FireworkPrices[3]);
                        GameLog.Money($"player({characterData.UUID})", $"server", Main.PricesSettings.FireworkPrices[3], $"buySaluteShop(4)");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили фейерверк", 3000);
                        BattlePass.Repository.UpdateReward(player, 28);
                        return;
                       
                    default:
                        // Not supposed to end up here. 
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"callback_moneyflow Exception: {e.ToString()}");
            }
        }

        public static void OpenSafedoorMenu(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                var frameList = new FrameListData();

                frameList.Header = "Дверь хранилища";
                frameList.Callback = callback_safedoor;
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.OpenClose), "change"));
                frameList.List.Add(new ListData(LangFunc.GetText(LangType.Ru, DataName.Hack), "crack"));
                
                Players.Popup.List.Repository.Open(player, frameList); 
            }
            catch (Exception e)
            {
                Log.Write($"OpenSafedoorMenu Exception: {e.ToString()}");
            }
        }
        private static void callback_safedoor(ExtPlayer player, object listItem)
        {
            try
            {
                if (!(listItem is string)) 
                    return;
                
                if (!player.IsCharacterData()) return;
                switch (listItem)
                {
                    case "change":
                        var memberFractionData = player.GetFractionMemberData();
                        if (memberFractionData != null && memberFractionData.Id == (int)Fractions.Models.Fractions.CITY && memberFractionData.Rank >= 19)
                        {
                            if (isCracking)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantDoThisNow), 3000);
                                return;
                            }
                            if (!canBeClosed)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantDoThisNow), 3000);
                                return;
                            }
                            if (isOpen)
                            {
                                isOpen = false;
                                Doormanager.SetDoorLocked(2, true, 0);
                            }
                            else
                            {
                                isOpen = true;
                                Doormanager.SetDoorLocked(2, true, 45f);
                            }
                            string msg = "Вы закрыли дверь";
                            if (isOpen) msg = "Вы открыли дверь";
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, msg, 3000);
                        }
                        else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantDoThisNow), 3000);
                        return;
                    case "crack":
                        startSafeDoorCracking(player);
                        return;
                }
            }
            catch (Exception e)
            {
                Log.Write($"callback_safedoor Exception: {e.ToString()}");
            }
        }
        #endregion
    }

    public class Safe
    {
        public int ID { get; private set; }
        public Vector3 Position { get; private set; }
        public float Rotation { get; private set; }
        public int MinAmount { get; private set; }
        public int MaxAmount { get; private set; }
        public string Address { get; private set; }

        [JsonIgnore]
        public bool IsOpen { get; private set; }

        [JsonIgnore]
        public List<int> LockAngles { get; private set; } = new List<int>();

        [JsonIgnore]
        public ExtPlayer Occupier { get; set; }

        [JsonIgnore]
        public GTANetworkAPI.Object Object { get; private set; }

        [JsonIgnore]
        private GTANetworkAPI.Object DoorObject;

        [JsonIgnore]
        public ExtTextLabel Label;

        [JsonIgnore]
        private ExtColShape colShape;

        [JsonIgnore]
        public int SafeLoot = 0;

        [JsonIgnore]
        private int RemainingSeconds;

        [JsonIgnore]
        private string Timer;

        [JsonIgnore]
        public ExtBlip Blip { get; set; } = null;

        [JsonIgnore]
        public DateTime BlipSet { get; set; } = DateTime.Now;

        public Safe(int id, Vector3 position, float rotation, int minamount, int maxamount, string address)
        {
            ID = id;
            Position = position;
            Rotation = rotation;
            MinAmount = minamount;
            MaxAmount = maxamount;
            Address = address;
        }

        public void Create()
        {
            try
            {
                Object = NAPI.Object.CreateObject(NAPI.Util.GetHashKey("v_ilev_gangsafe"), Position, new Vector3(0.0, 0.0, Rotation), 255, 0);
                DoorObject = NAPI.Object.CreateObject(NAPI.Util.GetHashKey("v_ilev_gangsafedoor"), Position, new Vector3(0.0, 0.0, Rotation), 255, 0);
                colShape = CustomColShape.CreateCylinderColShape(Position, 1.25f, 1.0f, 0, ColShapeEnums.HouseSafe, ID);

                Label = (ExtTextLabel) NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Сейф"), Position + new Vector3(0, 0, 1.05), 5f, 0.65f, 0, new Color(255, 255, 255), false);

                for (int i = 0; i < 3; i++) LockAngles.Add(SafeMain.SafeRNG.Next(0, 361));
            }
            catch (Exception e)
            {
                SafeMain.Log.Write($"Create Exception: {e.ToString()}");
            }
        }
        public void Loot(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                InventoryItemData Bags = Chars.Repository.GetItemData(player, "accessories", 8);
                if (Bags.ItemId == ItemId.BagWithDrill)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveSumka), 3000);
                    return;
                }

                if (SafeLoot == 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SafeNoMoney), 3000);
                    return;
                }

                int money = (SafeLoot >= SafeMain.MaxMoneyInBag) ? SafeMain.MaxMoneyInBag : SafeLoot;
                if (Bags.ItemId == ItemId.BagWithMoney)
                {
                    int lefts = Convert.ToInt32(Bags.Data.ToString());
                    if (lefts == SafeMain.MaxMoneyInBag)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.SumkaFullMoney), 3000);
                        return;
                    }
                    if (money + lefts > SafeMain.MaxMoneyInBag) money = (SafeMain.MaxMoneyInBag - lefts);
                    lefts += money;
                    Bags.Data = $"{lefts}";
                    Chars.Repository.SetItemData(player, "accessories", 8, Bags, true);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.InSumkeLefts, lefts), 3000);
                }
                else
                {
                    if (Chars.Repository.isFreeSlots(player, ItemId.BagWithMoney) != 0)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.EstSumkaIliDrill), 3000);
                        return;
                    }
                    Chars.Repository.ChangeAccessoriesItem(player, 8, $"{money}", true, ItemId.BagWithMoney);
                    Chars.Repository.AccessoriesUse(player, 8);
                    ClothesComponents.UpdateClothes(player);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGetSumka, money), 3000);
                }
                SafeLoot -= money;
            }
            catch (Exception e)
            {
                SafeMain.Log.Write($"Loot Exception: {e.ToString()}");
            }
        }

        public void Countdown()
        {
            try
            {
                RemainingSeconds--;

                if (RemainingSeconds < 1)
                {
                    Label.Text = "~w~Сейф";
                    for (int i = 0; i < 3; i++) LockAngles[i] = SafeMain.SafeRNG.Next(10, 351);
                    SetDoorOpen(false);
                }
                else
                {
                    TimeSpan time = TimeSpan.FromSeconds(RemainingSeconds);
                    Label.Text = string.Format("~r~Сейф ~n~~w~{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
                    Label.Text += $"\n~r~{SafeLoot}$";
                }
            }
            catch (Exception e)
            {
                SafeMain.Log.Write($"Countdown Exception: {e.ToString()}");
            }
        }

        public void SetDoorOpen(bool is_open)
        {
            try
            {
                IsOpen = is_open;
                DoorObject.Rotation = new Vector3(0.0, 0.0, (is_open) ? Rotation + 105.0 : Rotation);

                if (is_open)
                {
                    RemainingSeconds = SafeMain.SafeRespawnTime;

                    Timer = Timers.Start(1000, () => {
                        Countdown();
                    }, true);
                }
                else
                {
                    SafeLoot = 0;

                    if (Timer != null)
                    {
                        Timers.Stop(Timer);
                        Timer = null;
                    }
                }
            }
            catch (Exception e)
            {
                SafeMain.Log.Write($"SetDoorOpen Exception: {e.ToString()}");
            }
        }

        public void Destroy(bool check_players = false)
        {
            try
            {
                if (check_players)
                {
                    foreach (ExtPlayer foreachPlayer in NeptuneEvo.Character.Repository.GetPlayers())
                    {
                        var foreachSessionData = foreachPlayer.GetSessionData();
                        if (foreachSessionData == null) continue;
                        if (foreachPlayer.Position.DistanceTo(colShape.Position) > 1.5f) continue;
                        Trigger.ClientEvent(foreachPlayer, "SetSafeNearby", false);
                        foreachSessionData.TempSafeID = -1;
                    }
                }
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        if (Object != null && Object.Exists) Object.Delete();
                        Object = null;
                        if (DoorObject != null && DoorObject.Exists) DoorObject.Delete();
                        DoorObject = null;
                        if (Label != null && Label.Exists) Label.Delete();
                        Label = null;
                        CustomColShape.DeleteColShape(colShape);
                        colShape = null;
                    }
                    catch (Exception e)
                    {
                        SafeMain.Log.Write($"Destroy Task Exception: {e.ToString()}");
                    }
                });
                if (Timer != null) Timers.Stop(Timer);
            }
            catch (Exception e)
            {
                SafeMain.Log.Write($"Destroy Exception: {e.ToString()}");
            }
        }
    }
}
