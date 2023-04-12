using System;
using System.Collections.Generic;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using Newtonsoft.Json;

namespace NeptuneEvo.Businesses
{
    public class Events : Script
    {

        /*[RemoteEvent("client.businessmanage.sellBiz")]
        public void SellBizMenu (ExtPlayer player)
        {
            
        }*/

        [Command("businessmanage")]
        public void openBusinessManage(ExtPlayer player)
        {
            Businesses.Repository.Open(player);
        }
    }
}       