using System;

namespace RealmOfLegends.Core.Entities
{
    public class Inventory
    {
        public int InventoryId { get; set; }
        public int PlayerId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; } = 1;
        public bool IsEquipped { get; set; } = false;
        public string? SlotType { get; set; } // Head, Chest, Weapon, etc.
        public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;
        
        // Navigation Properties
        public virtual Player Player { get; set; } = null!;
        public virtual Item Item { get; set; } = null!;
    }
}
