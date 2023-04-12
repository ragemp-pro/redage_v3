using System;

namespace NeptuneEvo.Players.Phone.Messages.Models
{
    public class PhoneMessageListData
    {
        public int Phone;
        public bool IsMe;
        public string Text;
        public MessageType Type;
        public DateTime Date;
        public bool Status;//Прочитал или нет
    }
}