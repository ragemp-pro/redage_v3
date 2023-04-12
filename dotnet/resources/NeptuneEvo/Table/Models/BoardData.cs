using System;

namespace NeptuneEvo.Table.Models
{
    public class BoardData
    {
        public int UUId { get; set; } = 0;
        public string Name { get; set; } = "";
        public int Rank { get; set; } = 0;
        public DateTime Time { get; set; } = DateTime.Now;
        public string Title { get; set; } = "";
        public string Text { get; set; } = "";
    }
}