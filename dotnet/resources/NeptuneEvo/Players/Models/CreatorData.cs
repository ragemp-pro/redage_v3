using NeptuneEvo.Core;
using System.Collections.Generic;

namespace NeptuneEvo.Players.Models
{
    public class CreatorData
    {
        public bool Inside { get; set; } = false;
        public bool Changed { get; set; } = false;
        public bool IsCreate { get; set; } = true;
        public Dictionary<int, List<Tattoo>> Tattoos { get; set; } = new Dictionary<int, List<Tattoo>>();
    }
}
