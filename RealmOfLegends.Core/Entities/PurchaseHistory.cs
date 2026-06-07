using System;

namespace RealmOfLegends.Core.Entities
{
    public class PurchaseHistory
    {
        public int PurchaseId { get; set; }
        public int PlayerId { get; set; }
        public int ItemId { get; set; }
        public int GoldSpent { get; set; }
        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation Properties
        public virtual Player Player { get; set; } = null!;
        public virtual Item Item { get; set; } = null!;
    }
}
