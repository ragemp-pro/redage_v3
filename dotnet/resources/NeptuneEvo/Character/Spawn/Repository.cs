using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Houses;
using NeptuneEvo.Players;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using Localization;
using NeptuneEvo.Quests;

namespace NeptuneEvo.Character.Spawn
{
    public class Repository
    {
        private static readonly nLog Log = new nLog("Core.Character.Spawn");
        public static void Spawn(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                if (sessionData.LoggedIn) 
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                sessionData.LoggedIn = true;

                player.SetSharedData("IS_MASK", false);

                player.SetDefaultSkin();

                player.Health = (characterData.Health > 5) ? characterData.Health : 5;

                player.Armor = characterData.Armor;

                sessionData.ArmorHealth = (characterData.Armor == 0) ? -1 : characterData.Armor;

                Fractions.GangsCapture.LoadBlips(player);

                if (Main.Media.Contains(characterData.UUID)) 
                    player.SetSharedData("IS_MEDIA", true);
                
                NeptuneEvo.Events.Festive.InitPlayerData(player);

                //

                if (characterData.LVL == 0) 
                    Trigger.DamageDisable(player, true);
                
                if (characterData.LVL <= 5) 
                    player.SetSharedData("NewUser", true);
                
                if (characterData.LVL >= 100 && characterData.LVL <= 149) 
                    player.SetSharedData("Old1", true);
                
                if (characterData.LVL >= 150 && characterData.LVL <= 199) 
                    player.SetSharedData("Old3", true);
                
                if (characterData.LVL >= 200)
                    player.SetSharedData("Old2", true);
                
                //
                
                var battlePassData = player.BattlePassData;
                                
                if (battlePassData.TasksDay.Count == 0)
                    BattlePass.Repository.UpdateDay(player);
                            
                if (battlePassData.TasksWeek.Count == 0)
                    BattlePass.Repository.UpdateWeek(player);
                
                //
                
                var house = HouseManager.GetHouse(player);
                if (house != null)
                {        
                    if (house.Type != 7) 
                        house.PetName = characterData.PetName;

                    var garage = house.GetGarageData();
                    if (garage != null)
                    {
                        Trigger.ClientEvent(player, "createCheckpoint", 333, 1,
                            garage.Position - new Vector3(0, 0, 1.12), 1.5f, 0, 220, 220, 0);
                        Trigger.ClientEvent(player, "createGarageBlip", garage.Position);
                    }
                }

                //

                Fractions.Player.Repository.Init(player);
                
                Organizations.Player.Repository.Init(player);

                //
                
                World.War.Repository.OnPlayerSpawn(player);               
                NeptuneEvo.Events.EverydayAward.OnPlayerSpawn(player);

                //

                Rentcar.OnPlayerSpawn(player);
                
                //
                
          
                if (characterData.Time.TodayTime >= 300)
                    Trigger.ClientEvent(player, "client.roullete.updateCase", 2);
                else if (characterData.Time.TodayTime >= 180)
                    Trigger.ClientEvent(player, "client.roullete.updateCase", 1);
                else
                    Trigger.ClientEvent(player, "client.roullete.updateCase", 0);
         
                    
                //
                
                if (characterData.Warns > 0 && DateTime.Now > characterData.Unwarn)
                {
                    characterData.Warns--;
                    if (characterData.Warns > 0) characterData.Unwarn = DateTime.Now.AddDays(14);
                    characterData.WarnInfo.Admin[characterData.Warns] = "-1";
                    characterData.WarnInfo.Reason[characterData.Warns] = "-1";
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.OneWarnSnyat, characterData.Warns), 3000);
                }
                
                //

                Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", ItemId.BagWithMoney, 1);

                Chars.Repository.LoadCharItemsData(player);

                if (characterData.WantedLVL != null) 
                    Trigger.ClientEvent(player, "client.charStore.Wanted", characterData.WantedLVL.Level);
            }
            catch (Exception e)
            {
                Log.Write($"Spawn Exception: {e.ToString()}");
            }
        }

    }
}
