using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Quests.Models
{
    class GetQuestsStageModel
    {
        public int Status { set; get; }
        public DateTime Time { set; get; }

        public GetQuestsStageModel(int Status, DateTime Time)
        {
            this.Status = Status;
            this.Time = Time;
        }
    }
}
