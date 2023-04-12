using NeptuneEvo.Handles;
using NeptuneEvo.Core;
using NeptuneEvo.Events;
using NeptuneEvo.Houses;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.Players;
using Redage.SDK;
using System;
using System.Collections.Generic;

namespace NeptuneEvo.Character.Change
{
    public class Repository
    {
        private static readonly nLog Log = new nLog("Core.Character");

        /*public static async void ChangeName(string oldName)
        {
            try
            {
                if (!toChange.ContainsKey(oldName)) return;
                string newName = toChange[oldName];
                int Uuid = Main.PlayerUUIDs.GetValueOrDefault(oldName);
                if (Uuid <= 0) return;
                string[] split = newName.Split("_");

                Main.PlayerNames[Uuid] = newName;
                Main.PlayerUUIDs.TryRemove(oldName, out _);
                Main.PlayerUUIDs.TryAdd(newName, Uuid);
                try
                {
                    if (Main.PlayerBankAccs.ContainsKey(oldName))
                    {
                        int bank = Main.PlayerBankAccs[oldName];
                        Main.PlayerBankAccs.TryRemove(oldName, out _);
                        Main.PlayerBankAccs.TryAdd(newName, bank);
                    }
                }
                catch (Exception e)
                {
                    Log.Write("3ChangeName Exception: " + e.ToString());
                }

                using MySqlCommand cmd = new MySqlCommand("UPDATE `characters` SET `firstname`=@val1, `lastname`=@val2 WHERE `uuid`=@uuid");
                cmd.Parameters.AddWithValue("@val1", split[0]);
                cmd.Parameters.AddWithValue("@val2", split[1]);
                cmd.Parameters.AddWithValue("@uuid", Uuid);
                await MySQL.QueryAsync(cmd);
                VehicleManager.changeOwner(oldName, newName);
                BusinessManager.changeOwner(oldName, newName);
                Bank.changeHolder(oldName, newName);
                HouseManager.ChangeOwner(oldName, newName);
                HouseManager.UpdatePlayerHouseRoommates(oldName, newName);
                Organizations.Manager.changeNick(oldName, newName);
                Character.Friend.Repository.ClearFriends(oldName, newName);
                Log.Debug("Nickname has been changed!", nLog.Type.Success);
                toChange.Remove(oldName);
                GameLog.Name(Uuid, oldName, newName);
            }
            catch (Exception e)
            {
                Log.Write($"changeName Exception: {e.ToString()}");
            }
        }*/
        public static void ChangeName(ExtPlayer player, string newName)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null)
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;

                string oldName = sessionData.Name;

                int Uuid = Main.PlayerUUIDs.GetValueOrDefault(oldName);
                if (Uuid <= 0) 
                    return;

                string[] split = newName.Split("_");

                //
                characterData.FirstName = split[0];
                characterData.LastName = split[1];
                Save.Repository.SaveName(player);

                player.Name = newName;
                player.SetName(newName);
                //

                Main.PlayerNames[Uuid] = newName;
                Main.PlayerUUIDs.TryRemove(oldName, out _);
                Main.PlayerUUIDs.TryAdd(newName, Uuid);

                if (Main.PlayerBankAccs.ContainsKey(oldName))
                {
                    int bank = Main.PlayerBankAccs[oldName];
                    Main.PlayerBankAccs.TryRemove(oldName, out _);
                    Main.PlayerBankAccs.TryAdd(newName, bank);
                }
                VehicleManager.changeOwner(oldName, newName);
                BusinessManager.changeOwner(oldName, newName);
                Bank.changeHolder(oldName, newName);
                //
                HouseManager.ChangeOwner(oldName, newName);
                HouseManager.UpdatePlayerHouseRoommates(oldName, newName);
                //
                Organizations.Player.Repository.SetName(oldName, newName);
                Fractions.Player.Repository.SetName(oldName, newName);
                Fractions.LSNews.LsNewsSystem.OnDisconnect(player);
                Friend.Repository.ClearFriends(null, oldName, newName);
                Airsoft.OnPlayerDisconnected(player);
                MafiaGame.OnPlayerDisconnected(player);
                Admin.RemoveQueue(player, "сменой ника");
                ReportSys.OnAdminDisconnect(player);
                Log.Debug("Nickname has been changed!", nLog.Type.Success);
                GameLog.Name(Uuid, oldName, newName);
                GameLog.UpdateName(newName, characterData.UUID, player.Value);
            }
            catch (Exception e)
            {
                Log.Write($"changeName Exception: {e.ToString()}");
            }
        }
        
        public static void ChangeNameOffline(string oldName, string newName)
        {
            try
            {
                int Uuid = Main.PlayerUUIDs.GetValueOrDefault(oldName);
                if (Uuid <= 0)
                    return;
                
                string[] split = newName.Split("_");

                Main.PlayerNames[Uuid] = newName;
                Main.PlayerUUIDs.TryRemove(oldName, out _);
                Main.PlayerUUIDs.TryAdd(newName, Uuid);

                if (Main.PlayerBankAccs.ContainsKey(oldName))
                {
                    int bank = Main.PlayerBankAccs[oldName];
                    Main.PlayerBankAccs.TryRemove(oldName, out _);
                    Main.PlayerBankAccs.TryAdd(newName, bank);
                }
                
                VehicleManager.changeOwner(oldName, newName);
                BusinessManager.changeOwner(oldName, newName);
                Bank.changeHolder(oldName, newName);
                //
                HouseManager.ChangeOwner(oldName, newName);
                HouseManager.UpdatePlayerHouseRoommates(oldName, newName);
                //
                Organizations.Player.Repository.SetName(oldName, newName);
                Fractions.Player.Repository.SetName(oldName, newName);
                Friend.Repository.ClearFriends(null, oldName, newName);
                GameLog.Name(Uuid, oldName, newName);
                GameLog.UpdateName(newName, Uuid, -1);
                
                Save.Repository.SaveName(Uuid, split[0], split[1]);

                Log.Debug("Nickname has been changed!", nLog.Type.Success);
            }
            catch (Exception e)
            {
                Log.Write($"ChangeNameOffline Exception: {e.ToString()}");
            }
        }
    }
}
