using Newtonsoft.Json;

namespace NeptuneEvo.PedSystem.Pet.Models
{
    class PetShop
    {
        public int Price { set; get; } = 0;
        public bool isDonate { set; get; } = false;
        public uint Ped { set; get; } = 0;

        public PetShop(int Price, bool isDonate, uint Ped)
        {
            this.Price = Price;
            this.isDonate = isDonate;
            this.Ped = Ped;
        }

    }
}
