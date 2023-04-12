using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Character.Models;
using System.Collections.Generic;
using System.Linq;
using NeptuneEvo.BattlePass.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Houses;
using NeptuneEvo.Players;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Character
{
    public static class Repository
    {
        public static string[] LoginsBlck = new string[3] { "sokolyansky", "source1488", "qwelpy"};
        public static bool IsCharacterData(this ExtPlayer player)
        {
            if (player is null)
                return false;

            return player.CharacterData != null;
        }

        public static CharacterData GetCharacterData(this ExtPlayer player)
        {
            if (player != null)
                return player.CharacterData;

            return null;
        }

        public static void SavePlayerPosition(this ExtPlayer player)
        {
            if (player == null) 
                return;
            
            var characterData = player.CharacterData;
            if (characterData == null) 
                return;
            
            characterData.CurPosition = player.Position;
            
            if (!characterData.IsSpawned) 
                characterData.CurPosition = characterData.SpawnPos;
            else if (characterData.InsideHouseID != -1)
            {
                var house = HouseManager.Houses.FirstOrDefault(h => h.ID == characterData.InsideHouseID);
                if (house != null) 
                    characterData.CurPosition = house.Position + new Vector3(0, 0, 1.12);
            }
            else if (characterData.InsideGarageID != -1)
            {
                var garage = GarageManager.Garages[characterData.InsideGarageID];
                characterData.CurPosition = garage.Position + new Vector3(0, 0, 1.12);
            }
            else if (characterData.ExteriorPos != new Vector3())
            {
                Vector3 position = characterData.ExteriorPos;
                characterData.CurPosition = position + new Vector3(0, 0, 1.12);
            }
            else if (characterData.InsideHotelID != -1)
            {
                Vector3 position = Hotel.HotelEnters[characterData.InsideHotelID];
                characterData.CurPosition = position + new Vector3(0, 0, 1.12);
            }
            else if (characterData.TuningShop != -1)
            {
                Vector3 position = BusinessManager.BizList[characterData.TuningShop].EnterPoint;
                characterData.CurPosition = position + new Vector3(0, 0, 1.12);
            }
            else if (characterData.InsideOrganizationID != -1) 
                characterData.CurPosition = new Vector3(-774.045, 311.2569, 85.70606);
            else if (characterData.InCasino)
                characterData.CurPosition = new Vector3(925.845, 52.97, 81.17576);
            //else if (isMainThread && sessionData."InAirsoftLobby" >= 0) pos = JsonConvert.SerializeObject(new Vector3(-478.86032, -395.27307, 34.027653));
            else if (characterData.IsInPostalStock == true) 
                characterData.CurPosition = new Vector3(132.9969, 96.3529, 83.5076);

            if (characterData.IsSpawned && !characterData.IsAlive)
            {
                characterData.CurPosition = Fractions.Ems.emsCheckpoints[2];
                characterData.Health = 20;
                characterData.Armor = 0;
            }
            else
            {
                characterData.Health = player.Health;
                characterData.Armor = player.Armor;
            }
        }

        public static List<ExtPlayer> GetPlayers()
        {
            return Main.Characters.ToList();
        }
        
        public static void SetDefaultSkin(this ExtPlayer player)
        {                
            var characterData = player.GetCharacterData();
            if (characterData == null) return;

            player.setSkin((characterData.Gender) ? PedHash.FreemodeMale01 : PedHash.FreemodeFemale01);
            //
            Customization.ApplyCharacter(player);
        }        
        
        public static MissionData GetMissionData(this ExtPlayer player)
        {
            if (player != null)
                return player.MissionData;

            return null;
        }
    }
}
