using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Quests.Models
{
    public class PlayerQuestModel
    {
        public string ActorName { set; get; }
        public int Line { set; get; }
        public int Status { set; get; }
        public bool Complete { set; get; }
        public DateTime Time { set; get; }

        public PlayerQuestModel(string ActorName, int Line, int Status, bool Complete, DateTime Time)
        {
            this.ActorName = ActorName;
            this.Line = Line;
            this.Status = Status;
            this.Complete = Complete;
            this.Time = Time;
        }
    }
}
