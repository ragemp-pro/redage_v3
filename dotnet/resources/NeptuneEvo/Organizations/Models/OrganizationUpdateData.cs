namespace NeptuneEvo.Organizations.Models
{
    public class OrganizationUpdateData
    {
        public string type = "";
        public string name = "";
        public string icon = "";
        public bool isRb = false;
        public int price = 0;

        public OrganizationUpdateData(string type, string name, string icon, bool isRb, int price)
        {
            this.type = type;
            this.name = name;
            this.icon = icon;
            this.isRb = isRb;
            this.price = price;
        }
    }
}