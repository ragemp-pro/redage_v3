using GTANetworkAPI;
using NeptuneEvo.Character;
using NeptuneEvo.Handles;
using NeptuneEvo.Chars;
namespace NeptuneEvo.Functions
{
    class Other : Script
    {
        public static bool IsPlayerToSquare(ExtPlayer player, float Min_X, float Min_Y, float Max_X, float Max_Y)
        {
            if (!player.IsCharacterData())
                return false;
            if ((Min_X <= player.Position.X && player.Position.X <= Max_X) && (Min_Y <= player.Position.Y && player.Position.Y <= Max_Y)) return true;
            return false;
        }
        /*
         * stock PlayerToKvadrat(playerid, Float:Min_X, Float:Min_Y, Float:Max_X, Float: Max_Y)
{
	new Float: Position[3];
	GetPlayerPos(playerid, Position[0], Position[1], Position[2]);
	if((Min_X <= Position[0] <= Max_X) && (Min_Y <= Position[1] <= Max_Y)) return 1;
	return 0;
}
         */
    }
}
