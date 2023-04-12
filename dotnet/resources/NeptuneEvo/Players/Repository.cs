using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GTANetworkAPI;
using NeptuneEvo.Core;
using NeptuneEvo.GUI;
using NeptuneEvo.Handles;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players.Phone.Models;
using NeptuneEvo.Quests.Models;

namespace NeptuneEvo.Players
{
    public static class Repository
    {
        public static bool IsSessionData(this ExtPlayer player)
        {
            if (player is null)
                return false;
            
            return player.SessionData != null;
        }
        public static bool GetLoginIn(this ExtPlayer player)
        {
            if (player != null && player.IsSessionData())
                return player.SessionData.LoggedIn;

            return false;
        }
        public static SessionData GetSessionData(this ExtPlayer player)
        {
            if (player != null)
                return player.SessionData;

            return null;
        }
        //
        
        public static PlayerCustomization GetCustomization(this ExtPlayer player)
        {
            if (player != null)
                return player.Customization;

            return null;
        }
        
        //
        
        public static PlayerQuestModel GetQuest(this ExtPlayer player)
        {
            if (player != null)
                return player.Quest;

            return null;
        }
        
        //
        
        public static List<ExtColShapeData> GetColShapesData(this ExtPlayer player)
        {
            if (player != null)
                return player.ColShapesData;

            return null;
        }
        
        public static ExtColShapeData GetLastColShapeData(this ExtPlayer player)
        {
            var colShapesData = player.GetColShapesData();
            if (colShapesData != null)
                return colShapesData.LastOrDefault();

            return null;
        }
        
        //
        public static ExtColShapeData GetColShapeData(this ExtColShape shape)
        {
            if (shape != null)
                return shape.ColShapeData;

            return null;
        }
        
        //

        
        public static void setSkin(this ExtPlayer player, PedHash model)
        {
            if (player != null)
            {
                player.Skin = model;
                player.SetSkin(model);
            }
        }

        public static void setKick(this ExtPlayer player, string text)
        {
            Trigger.SetMainTask(() =>
            {
                Trigger.ClientEvent(player, "client.kick", text);
                Disconnect.Repository.OnPlayerDisconnect(player, DisconnectionType.Kicked, text);
            });
        }
        public static void setBan(this ExtPlayer player, string text)
        {
            Trigger.SetMainTask(() =>
            {
                Trigger.ClientEvent(player, "client.ban", text);
                Disconnect.Repository.OnPlayerDisconnect(player, DisconnectionType.Kicked, text);
            });
        }
        
        
        //New Phone
        
        public static PhoneData getPhoneData(this ExtPlayer player)
        {
            if (player != null)
                return player.PhoneData;
            
            return null;
        }
        
        //
        
        public static KeyClampData GetKeyClampData(this ExtPlayer player)
        {
            if (player != null)
                return player.KeyClampData;

            return null;
        }
        
        //
        public static Vector3 GetPosition(this ExtPlayer player)
        {
            if (player != null && Thread.CurrentThread.Name == "Main")
            {
                var position = player.Position;
                
                return new Vector3(position.X, position.Y, position.Z);
            }
            
            return new Vector3();
        }
        
    }
}
