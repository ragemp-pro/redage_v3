using System;
using GTANetworkAPI;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.Table.Tasks.Models;
using NeptuneEvo.Table.Tasks.Player;
using Newtonsoft.Json;

namespace NeptuneEvo.Table.Tasks
{
    public class Events : Script
    {
        [Command("testtable")]
        public void testtable(ExtPlayer player)
        {
            if (Main.ServerNumber != 0)
                return;
            
            Table.Tasks.Repository.UpdateOrg();
            
        }
        [Command("ct")]
        public void cmyt(ExtPlayer player, int id)
        {
            if (Main.ServerNumber != 0)
                return;
            
            player.AddTableScore((TableTaskId)id);
            
        }
        [Command("t3")]
        public void t3(ExtPlayer player, int id, int victoryId)
        {
            if (Main.ServerNumber != 0)
                return;
            
            Organizations.FamilyZones.Repository.Update((byte) id, victoryId);
            
        }
        
        
    }
}