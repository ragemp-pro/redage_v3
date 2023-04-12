using GTANetworkAPI;
using NeptuneEvo.Character;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.Players.Phone.Auction.Models;
using Redage.SDK;

namespace NeptuneEvo.Players.Phone.Auction
{
    public class Events : Script
    {
        [RemoteEvent("server.phone.auction.load")]
        public void OnLoad(ExtPlayer player) => Repository.Init(player);
        [RemoteEvent("server.phone.auction.close")]
        public void OnClose(ExtPlayer player) => Repository.OnClose(player);
        
        [RemoteEvent("server.phone.auction.getItem")]
        public void GetItem(ExtPlayer player, int type) => Repository.GetItem(player, (AuctionType) type);
        
        [RemoteEvent("server.phone.auction.add")]
        public void OnAdd(ExtPlayer player, int type, int elementId, string text, string image, int price) => 
            Repository.OnCreate(player, (AuctionType) type, elementId, text, image, price);
        
        [RemoteEvent("server.phone.auction.bet")]
        public void OnBet(ExtPlayer player, int id, int price) => 
            Repository.OnBet(player, id, price);

        [Command(AdminCommands.Acancel)]
        public void ACancel(ExtPlayer player, int id)
        {
            if (!CommandsAccess.CanUseCmd(player, AdminCommands.Acancel)) return;
            var characterData = player.GetCharacterData();
            if (characterData == null) return;

            Repository.EndToId(id);
            
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы завершили аукцион {id}", 5000);

        }
    }
}