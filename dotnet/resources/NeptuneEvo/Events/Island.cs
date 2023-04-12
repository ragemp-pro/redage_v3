using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using NeptuneEvo.Character;
using NeptuneEvo.Handles;
using NeptuneEvo.Chars;
using NeptuneEvo.Functions;
using Redage.SDK;

namespace NeptuneEvo.Events
{
    class Island : Script
    {
        private static readonly nLog Log = new nLog("Events.Island");
        
        private static Vector3 MiddleOfIsland = new Vector3(4945, -4687, 10);

        public static void Init()
        {
            CustomColShape.CreateCylinderColShape(MiddleOfIsland, 1800, 1000, colShapeEnums: ColShapeEnums.Island);

            Log.Write("Island loaded");
        }
        [Interaction(ColShapeEnums.Island, In: true)]
        public static void InIsland(ExtPlayer player)
        {
            if (!player.IsCharacterData()) return;
            player.Eval("mp.game.invoke(\"0x9A9D1BA639675CF1\", \"HeistIsland\", true);mp.game.invoke(\"0x5E1460624D194A38\", true);");
        }
        [Interaction(ColShapeEnums.Island, Out: true)]
        public static void OutIsland(ExtPlayer player)
        {
            if (!player.IsCharacterData()) return;
            player.Eval("mp.game.invoke(\"0x9A9D1BA639675CF1\", \"HeistIsland\", false);mp.game.invoke(\"0x5E1460624D194A38\", false);");
        }
    }
}
