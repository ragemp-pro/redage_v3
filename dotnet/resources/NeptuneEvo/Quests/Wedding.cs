using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.Players;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Quests.Models;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Localization;

namespace NeptuneEvo.Quests
{
    class Wedding : Script
    {
        private static readonly nLog Log = new nLog("Quests.Wedding");
        public static readonly int WeddingPrice = 400_000;
        public static readonly int DivorcioPrice = 200_000;
        public static readonly int DivorcioOnePrice = 300_000;
        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            Main.CreateBlip(new Main.BlipData(622, "Церковь", new Vector3(-759.22766, -709.2103, 30.061632), 59, true));
            PedSystem.Repository.CreateQuest("cs_priest", new Vector3(-786.86f, -708.91, 30.322592), -94f, title: "~y~NPC~w~ Отец Михаил\nСвященник", colShapeEnums: ColShapeEnums.QuestWedding);
        }
        [Interaction(ColShapeEnums.QuestWedding)]
        public static void Open(ExtPlayer player, int index)
        {
            if (!FunctionsAccess.IsWorking("wedding"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                return;
            }
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            if (sessionData.CuffedData.Cuffed)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsCuffed), 3000);
                return;
            }
            else if (sessionData.DeathData.InDeath)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsDying), 3000);
                return;
            }
            else if (Main.IHaveDemorgan(player, true)) return;

            player.SelectQuest(new PlayerQuestModel("npc_wedding", 0, 0, false, DateTime.Now));
            BattlePass.Repository.UpdateReward(player, 148);
            Trigger.ClientEvent(player, "client.quest.open", index, "npc_wedding", 0, 0, 0);
        }
        
        [Interaction(ColShapeEnums.QuestBonus)]
        public static void OpenBonus(ExtPlayer player, int index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            if (sessionData.CuffedData.Cuffed)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsCuffed), 3000);
                return;
            }
            else if (sessionData.DeathData.InDeath)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsDying), 3000);
                return;
            }
            else if (Main.IHaveDemorgan(player, true)) return;

            player.SelectQuest(new PlayerQuestModel("npc_birthday", 0, 0, false, DateTime.Now));
            BattlePass.Repository.UpdateReward(player, 148);
            Trigger.ClientEvent(player, "client.quest.open", index, "npc_birthday", 0, 0, 0);
        }
        
        public static void Perform(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            int price = 0;
            int type = 0;
            if (characterData.WeddingUUID == 0)
            {
                type = 0;
                price = WeddingPrice;
            }
            else
            {
                type = 1;
                ExtPlayer target = Main.GetPlayerByUUID(characterData.WeddingUUID);
                if (target != null && target.IsCharacterData() && player.Position.DistanceTo(target.Position) <= 5f)
                    price = DivorcioPrice;
                else
                    price = DivorcioOnePrice;
            }
            Trigger.ClientEvent(player, "client.wedding.open", type, price);
        }
        [RemoteEvent("server.wedding.married")]
        public void OpenDialog(ExtPlayer player, string name, int typeSurname)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;

            var characterData = player.GetCharacterData();
            if (characterData == null)
                return;

            int id;
            ExtPlayer target = null;
            if (int.TryParse(name, out id))
                target = Main.GetPlayerByID(id);
            else
                target = (ExtPlayer) NAPI.Player.GetPlayerFromName(name);

            int type = 0;
            int price = 0;
            if (characterData.WeddingUUID == 0)
            {
                var targetCharacterData = target.GetCharacterData();

                if (target != null && player == target) return;
                else if (targetCharacterData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindPlayerWithId), 3000);
                    return;
                }
                else if (typeSurname != 0 && typeSurname != 2 && Main.PlayerNames.Values.Contains($"{characterData.FirstName}_{targetCharacterData.LastName}"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NameExists), 3000);
                    return;
                }
                
                type = 0;
                price = WeddingPrice;
            }
            else
            {
                target = Main.GetPlayerByUUID(characterData.WeddingUUID);

                if (target != null && player == target)
                    return;

                var targetCharacterData = target.GetCharacterData();

                if (targetCharacterData != null && player.Position.DistanceTo(target.Position) <= 5f)
                {
                    type = 1;
                    price = DivorcioPrice;
                }
                else
                {
                    type = 2;
                    price = DivorcioOnePrice;
                }
            }
            if (UpdateData.CanIChange(player, price, true) != 255) return;

            sessionData.WeddingData = new WeddingData
            {
                player = target,
                type = type,
                typeSurname = typeSurname
            };

            switch (type)
            {
                case 0:
                    if (typeSurname == 0 || typeSurname == 2) 
                        Trigger.ClientEvent(player, "openDialog", "Wedding", LangFunc.GetText(LangType.Ru, DataName.YouWantWedding, target.Name, MoneySystem.Wallet.Format(price)));
                    else 
                        Trigger.ClientEvent(player, "openDialog", "Wedding", LangFunc.GetText(LangType.Ru, DataName.YouWantWeddingFamily, target.Name, MoneySystem.Wallet.Format(price), target.Name.Split("_")[1]));
                    break;
                case 1:
                    Trigger.ClientEvent(player, "openDialog", "Wedding", LangFunc.GetText(LangType.Ru, DataName.YouDivorce, target.Name, MoneySystem.Wallet.Format(price)));
                    break;
                case 2:
                    Trigger.ClientEvent(player, "openDialog", "Wedding", LangFunc.GetText(LangType.Ru, DataName.YouDivorceSolo, MoneySystem.Wallet.Format(price)));
                    break;
            }

        }
        public static void OnBoda(ExtPlayer player, ExtPlayer target, int typeSurname)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null)
                return;

            var characterData = player.GetCharacterData();
            if (characterData == null)
                return;
            else if (characterData.WeddingUUID != 0)
                return;

            var targetSessionData = target.GetSessionData();
            if (targetSessionData == null)
                return;

            var targetCharacterData = target.GetCharacterData();
            if (targetCharacterData == null)
                return;
            else if (targetCharacterData.WeddingUUID != 0)
                return;

            if (player.Position.DistanceTo(target.Position) > 5f)
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouTooFar), 3000);
                return;
            }
            
            if (typeSurname == 0 && Main.PlayerNames.Values.Contains($"{targetCharacterData.FirstName}_{characterData.LastName}"))
            {
                Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NameExists), 3000);
                return;
            }
            else if (typeSurname == 1 && Main.PlayerNames.Values.Contains($"{characterData.FirstName}_{targetCharacterData.LastName}"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NameExists), 3000);
                return;
            }

            if (UpdateData.CanIChange(player, WeddingPrice, true) != 255) return;

            Wallet.Change(player, -WeddingPrice);
            GameLog.Money($"player({characterData.UUID})", $"system", WeddingPrice, $"Wedding");

            int UUID = characterData.UUID;
            int targetUUID = targetCharacterData.UUID;

            targetCharacterData.WeddingUUID = UUID;
            characterData.WeddingUUID = targetUUID;
            
            if (characterData.LastName != targetCharacterData.LastName)
            {
                if (typeSurname == 0)
                {
                    string newName = $"{targetCharacterData.FirstName}_{characterData.LastName}";
                    Character.Change.Repository.ChangeName(target, newName);
                }
                else if (typeSurname == 1)
                {
                    string newName = $"{characterData.FirstName}_{targetCharacterData.LastName}";
                    Character.Change.Repository.ChangeName(player, newName);
                }
            }
            targetCharacterData.WeddingName = $"{characterData.FirstName}_{characterData.LastName}";
            characterData.WeddingName = $"{targetCharacterData.FirstName}_{targetCharacterData.LastName}";

            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WeddingSuc, target.Name), 10000);
            Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WeddingSuc, player.Name), 10000);
            NAPI.Chat.SendChatMessageToAll(LangFunc.GetText(LangType.Ru, DataName.SucWedding,Chars.Models.ChatColors.AMP, target.Name, player.Name));

        }
        public static void OnDivorcio(ExtPlayer player, bool isOne = false)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null)
                return;

            var characterData = player.GetCharacterData();
            if (characterData == null || characterData.WeddingUUID == 0)
                return;
            int price = isOne ? DivorcioOnePrice : DivorcioPrice;

            if (UpdateData.CanIChange(player, price, true) != 255) return;

            Wallet.Change(player, -price);
            GameLog.Money($"player({characterData.UUID})", $"system", price, $"Divorcio");

            int uuid = characterData.WeddingUUID;
            ExtPlayer target = Main.GetPlayerByUUID(uuid);

            var targetCharacterData = target.GetCharacterData();

            if (targetCharacterData != null)
            {
                targetCharacterData.WeddingUUID = 0;
                targetCharacterData.WeddingName = "";
                Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YourDivorced, player.Name), 10000);
            }
            else
            {
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке
                    
                        await db.Characters
                            .Where(c => c.Uuid == uuid)
                            .Set(c => c.WeddingUUID, 0)
                            .Set(c => c.WeddingName, "")
                            .UpdateAsync();
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
            }

            characterData.WeddingUUID = 0;
            characterData.WeddingName = "";

            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouDivorced, target.Name), 10000);
        }
    }
}
