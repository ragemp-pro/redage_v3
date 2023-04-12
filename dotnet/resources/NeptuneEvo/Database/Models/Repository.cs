namespace NeptuneEvo.Database.Models
{
    public class Repository
    {
        public static void InitSave()
        {
            Houses.Start();
            Business.Start();
            Bank.Start();
            GameLog.Start();
            Items.Start();
            Phone.Start();
            Money.Start();
            Members.Start();
        }
    }
}