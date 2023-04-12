using System;
using System.Linq;
using GTANetworkAPI;
using NeptuneEvo.Character;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using NeptuneEvo.Players.Models;
using Redage.SDK;

namespace NeptuneEvo.Functions
{
    public class KeyClamp : Script
    {
        public static void SetKeyClamp(ExtPlayer player, KeyClampData keyClampData)
        {
            try
            {
                ClearKeyClamp(player);
            
                Trigger.ClientEvent(player, $"keyClamp.bind", keyClampData.Name);
                player.SetKeyClampData(keyClampData);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        public static void ClearKeyClamp(ExtPlayer player)
        {
            try
            {
                var keyClampData = player.GetKeyClampData();

                if (keyClampData == null) 
                    return;
            
                Trigger.ClientEvent(player, $"keyClamp.unbind", keyClampData.Name);
            
                player.SetKeyClampData();
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        public static void ClearKeyClamp(ExtPlayer player, ColShapeEnums colShapeId)
        {
            try
            {
                var keyClampData = player.GetKeyClampData();

                if (keyClampData == null) 
                    return;
            
                if (keyClampData.ColShapesData == null)
                    return;
            
                if (keyClampData.ColShapesData.ColShapeId != colShapeId) 
                    return;
            
                Trigger.ClientEvent(player, $"keyClamp.unbind", keyClampData.Name);
            
                player.SetKeyClampData();
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        public static void ClearKeyClamp(ExtPlayer player, ColShapeEnums colShapeId, int index)
        {
            try
            {
                var keyClampData = player.GetKeyClampData();

                if (keyClampData == null) 
                    return;
            
                if (keyClampData.ColShapesData == null)
                    return;
            
                if (keyClampData.ColShapesData.ColShapeId != colShapeId || keyClampData.ColShapesData.Index != index) 
                    return;
            
                Trigger.ClientEvent(player, $"keyClamp.unbind", keyClampData.Name);
            
                player.SetKeyClampData();
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public static void ClearKeyClamp(ColShapeEnums colShapeId, int index = (int) ColShapeData.Error, int listId = (int) ColShapeData.Error)
        {
            try
            {
                foreach (var foreachPlayer in RAGE.Entities.Players.All.Cast<ExtPlayer>())
                {
                    if (!foreachPlayer.IsCharacterData())
                        continue;
                
                    var foreachKeyClampData = foreachPlayer.GetKeyClampData();
                    if (foreachKeyClampData == null)
                        continue;
                
                    if (foreachKeyClampData.ColShapesData == null)
                        continue;
            
                    if (foreachKeyClampData.ColShapesData.ColShapeId != colShapeId || foreachKeyClampData.ColShapesData.Index != index || foreachKeyClampData.ColShapesData.ListId != listId) 
                        continue;
                
                    ClearKeyClamp(foreachPlayer);
                    CustomColShape.SetColShapesData(foreachPlayer, foreachKeyClampData.ColShapesData.ColShapeId,
                        foreachKeyClampData.ColShapesData.Index, foreachKeyClampData.ColShapesData.ListId, true);
                
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        [RemoteEvent("keyClamp.keyDown")]
        public void OnKeyDown(ExtPlayer player)
        {
            try
            {
                var keyClampData = player.GetKeyClampData();

                if (keyClampData == null) 
                    return;

                var value = keyClampData.GetHealthCB(player);

                if (value.Item1 == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Что то пошло не так", 3000);
                    return;
                }

                if (value.Item1 == 0)
                    return;
            
                Trigger.ClientEvent(player, $"keyClamp.value", keyClampData.Name, value.Item1, value.Item2);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        [RemoteEvent("keyClamp.end")]
        public void OnEnd(ExtPlayer player, int value)
        {
            try
            {
                var keyClampData = player.GetKeyClampData();

                if (keyClampData == null) 
                    return;

                if (0 > value)
                    value = 0;
            
                //if (value == 0 && keyClampData.ColShapesData != null)
                //    CustomColShape.SetColShapesData(player, keyClampData.ColShapesData.ColShapeId, keyClampData.ColShapesData.Index, keyClampData.ColShapesData.ListId, true); 
            
                keyClampData.EndCB(player, value);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
    }
}