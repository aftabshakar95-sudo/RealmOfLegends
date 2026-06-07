namespace RealmOfLegends.Core.Entities
{
    public class Item
    {
        public int ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // Weapon, Armour, Accessory, Consumable
        public string Rarity { get; set; } = string.Empty; // Common, Uncommon, Rare, Epic, Legendary
        public int Price { get; set; }
        public string StatJson { get; set; } = "{}"; // JSON: {"ATK": 25, "DEF": 10, "CritChance": 5}
        public string Description { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string SlotType { get; set; } = string.Empty; // Head, Chest, Legs, Boots, Gloves, Weapon, Accessory
        
        // Navigation Properties
        public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
        public virtual ICollection<PurchaseHistory> PurchaseHistories { get; set; } = new List<PurchaseHistory>();
    }
}
