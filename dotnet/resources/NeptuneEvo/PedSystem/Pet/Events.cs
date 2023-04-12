using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Accounts;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.Players;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using Localization;

namespace NeptuneEvo.PedSystem.Pet
{
    class Events : Script
    {
        
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            Repository.LoadInGame();
        }
        [RemoteEvent("server.pet.getBall")]
        public void Save(ExtPlayer player, ExtPed ped)
        {
            if (ped != null)
            {
                ped.SetSharedData("attachmentsData", JsonConvert.SerializeObject(new List<uint>()));
            }
            Repository.AddBall(player);
        }
        [RemoteEvent("server.pet.sniff")]
        public void OnSniff(ExtPlayer player, ExtPlayer target)
        {
            Repository.OnSniff(player, target);
        }
        [RemoteEvent("server.pet.topos")]
        public void OnToPos(ExtPlayer player)
        {
            Repository.OnToPos(player);
        }
        [RemoteEvent("server.pet.dellball")]
        public void petshopbuy(ExtPlayer player, ExtPed ped, float xPos, float yPos, float zPos)
        {
            if (ped != null)
            {
                ped.SetSharedData("attachmentsData", JsonConvert.SerializeObject(new List<uint> { Attachments.AttachmentsName.Ball }));
            }
            Trigger.ClientEventInRange(new Vector3(xPos, yPos, zPos), 200f, "client.pet.dellball", xPos, yPos, zPos);
        }
        [RemoteEvent("server.pet.setEat")]
        public void SetEat(ExtPlayer player)
        {
            Repository.SetEat(player);
        }

        [RemoteEvent("server.pet.updateDim")]
        public void OnUpdateDim(ExtPlayer player)
        {
            //Repository.OnUpdateDim(player);
        }




        [RemoteEvent("server.petshop.buy")]
        public void petshopbuy(ExtPlayer player, int index)
        {
            if (!FunctionsAccess.IsWorking("pet"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                return;
            }
            Repository.Create(player, index);
        }
    }
}
