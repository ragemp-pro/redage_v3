using NeptuneEvo.Handles;

namespace NeptuneEvo.Players.Phone.Call.Models
{
    public class CallData
    {
        public int Number;
        public CallType Type = CallType.Talk;
        public bool IsCall = false;
        public ExtPlayer Target = null;
    }
}