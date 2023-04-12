using System.Collections.Generic;

namespace NeptuneEvo.Players.Popup.List.Models
{
    public class FrameListData
    {
        public string Header;
        public List<ListData> List = new List<ListData>();
        public ListCallback Callback;
    }
}