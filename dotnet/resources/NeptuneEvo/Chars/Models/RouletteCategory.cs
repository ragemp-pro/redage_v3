namespace NeptuneEvo.Chars.Models
{
    public class RouletteCategory
    {
        public string Name = "";
        public string Image = "";
        public int[] CaseList;

        public RouletteCategory(string name, string image, int[] caseList)
        {
            this.Name = name;
            this.Image = image;
            this.CaseList = caseList;
        }
    }
}