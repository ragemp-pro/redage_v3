using System;
using System.Collections.Generic;
using NeptuneEvo.Handles;
using NeptuneEvo.Players.Popup.List.Models;
using Newtonsoft.Json;

namespace NeptuneEvo.Players.Popup.List
{
    public class Repository
    {
        public static void Open(ExtPlayer player, FrameListData frameListData)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null)
                return;
            
            if (frameListData.List.Count == 0)
                return;

            var sendList = new List<List<object>>();
            
            foreach (var listData in frameListData.List)
            {
                sendList.Add(new List<object>
                {
                    listData.Name,
                    listData.Id
                });
            }

            sessionData.PopupData.List = frameListData.Callback;
            
            Trigger.ClientEvent(player, "popup.list.open", frameListData.Header, JsonConvert.SerializeObject(sendList));
        }

        public static void Callback(ExtPlayer player, object listItem)
        {
            var sessionData = player.GetSessionData();

            if (sessionData?.PopupData == null)
                return;

            if (listItem == null || listItem.ToString() == "null")
                listItem = null;
            
            var callback = sessionData.PopupData.List;
            sessionData.PopupData.List = null;
            callback?.Invoke(player, listItem);
        }
    }
}