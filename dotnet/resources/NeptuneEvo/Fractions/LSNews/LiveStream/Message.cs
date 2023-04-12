using System;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Fractions.LSNews.LiveStream
{
    public class Message
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public bool IsOwner { get; set; }
        public DateTime PublishTime { get; set; }
        public Message(string name, string text, bool isOwner)
        {
            this.Name = name;
            this.Text = text;
            this.PublishTime = DateTime.Now;
            this.IsOwner = isOwner;
        }
    }
}