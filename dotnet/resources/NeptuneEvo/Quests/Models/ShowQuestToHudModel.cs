using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Quests.Models
{
    class ShowQuestToHudModel
    {
        public string ActorName { set; get; }
        public int Line { set; get; }
        public sbyte Stage { set; get; }

        public ShowQuestToHudModel(string ActorName, int Line, sbyte Stage)
        {
            this.ActorName = ActorName;
            this.Line = Line;
            this.Stage = Stage;
        }
    }
}
