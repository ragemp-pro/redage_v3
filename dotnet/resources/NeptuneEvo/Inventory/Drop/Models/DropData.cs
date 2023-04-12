using System;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Inventory.Drop.Models
{
    public class DropData
    {
        public DateTime DeleteTime { get; set; }
        public ExtPlayer Player { get; set; }
        public InventoryItemData Item { get; set; }
    }
}