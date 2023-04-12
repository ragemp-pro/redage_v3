using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Quests.Models
{
    class SetQuestsStageModel
    {
        public int Status { set; get; }
        public int Line { set; get; }
        public DateTime Time { set; get; }
        public bool Complete { set; get; }
        public string Data { set; get; }

        public SetQuestsStageModel(int Status, int Line, DateTime Time, bool Complete, string Data)
        {
            this.Status = Status;
            this.Line = Line;
            this.Time = Time;
            this.Complete = Complete;
            this.Data = Data;
        }
    }
}
