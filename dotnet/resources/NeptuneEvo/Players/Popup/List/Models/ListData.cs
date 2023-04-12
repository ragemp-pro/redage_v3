namespace NeptuneEvo.Players.Popup.List.Models
{
    public class ListData
    {
        public string Name;
        public object Id;

        public ListData(string name, object id)
        {
            this.Name = name;
            this.Id = id;
        }
    }
}