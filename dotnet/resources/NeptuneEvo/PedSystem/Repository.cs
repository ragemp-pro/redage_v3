using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Accounts;
using NeptuneEvo.Functions;
using NeptuneEvo.PedSystem.Models;
using NeptuneEvo.Players;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NeptuneEvo.PedSystem
{
    class Repository : Script
    {
        public static Dictionary<ExtPed, PedData> PedsData = new Dictionary<ExtPed, PedData>();
        public static ExtPed CreateQuest(string model, Vector3 pos, float heading, uint dimension = 0, string questName = null, ColShapeEnums colShapeEnums = ColShapeEnums.None, string title = null, bool isBlipVisible = true, PedBlipData pedBlipData = null)
        {
            var ped = (ExtPed) NAPI.Ped.CreatePed(NAPI.Util.GetHashKey(model), pos, heading, invincible: false, frozen: true, controlLocked: false, dimension: dimension);
            
            var pedData = new PedData();
            pedData.Ped = ped;

            if (questName != null) 
                ped.SetSharedData("questName", questName);
            else if (colShapeEnums != ColShapeEnums.None) 
                ped.SetSharedData("questName", 1);

            if (colShapeEnums != ColShapeEnums.None) 
                pedData.ColShape = CustomColShape.CreateCylinderColShape(pos, 2.0f, 2, dimension, colShapeEnums, Index: ped.Value);

            if (title != null)
            {
                pedData.TextLabel = (ExtTextLabel) NAPI.TextLabel.CreateTextLabel(title, pos, 7f, 0.3f, 4, new Color(255, 255, 255, 155), false, dimension);

                if (questName != null && isBlipVisible)
                {
                    if (pedBlipData != null)
                        Main.CreateBlip(new Main.BlipData(pedBlipData.BlipId, pedBlipData.BlipName, pos, pedBlipData.BlipColor, true));
                    else
                    {
                        string blipName = title.Split("\n").Length > 1 ? title.Split("\n")[0] : title;
                        Main.CreateBlip(new Main.BlipData(456, blipName.Split("~w~").Length > 1 ? blipName.Split("~w~")[1] : blipName, pos, 68, true));
                    }
                }
            }
            
            PedsData.Add(ped, pedData);

            return ped;
        }
        public static void DestroyQuest(ExtPed ped)
        {
            if (ped != null && PedsData.ContainsKey(ped))
            {
                PedData pedData = PedsData[ped];

                if (pedData.TextLabel != null && pedData.TextLabel.Exists)
                    pedData.TextLabel.Delete();

                CustomColShape.DeleteColShape(pedData.ColShape);

                if (pedData.Ped != null && pedData.Ped.Exists) 
                    pedData.Ped.Delete();

                PedsData.Remove(ped);
            }
        }
        [Command("tv")]
        public void testPeds1(ExtPlayer player, int value)
        {
            if (Main.ServerNumber != 0) return;
            if (value == 0)
                player.ResetSharedData("VoiceDist");
            else
                player.SetSharedData("VoiceDist", value);
        }
        [Command("se")]
        public void se(ExtPlayer player, string propName, string saveName, int isDell)
        {
            if (Main.ServerNumber != 0) return;
            Trigger.ClientEvent(player, "client.editor.start", propName, saveName, isDell);
        }
        [RemoteEvent("server.editor.save")]
        public void sesave(ExtPlayer player, string propName, string saveName, float xPos, float yPos, float zPos, float rPos)
        {
            using(StreamWriter saveCoords = new StreamWriter($"{saveName}.txt", true, Encoding.UTF8))
            {
                saveCoords.Write($"{propName}: new Vector3({xPos}, {yPos}, {zPos}), new Vector3(0.00000000, 0.00000000, {rPos}),\r\n");
                saveCoords.Close();
            }
        }
        [Command("es")]
        public void es(ExtPlayer player)
        {
            if (Main.ServerNumber != 0) return;
            Trigger.ClientEvent(player, "client.editor.sit");
        }
    }
}
