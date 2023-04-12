using NeptuneEvo.Handles;
using NeptuneEvo.Players.Popup.Info.Models;
using NeptuneEvo.Players.Popup.Input.Models;
using NeptuneEvo.Players.Popup.List.Models;

namespace NeptuneEvo.Players.Popup.Models
{
    public class PopupData
    {
        public InputCallback Input { get; set; }
        
        public InfoCallback Info { get; set; }
        
        public ListCallback List { get; set; }
    }
}