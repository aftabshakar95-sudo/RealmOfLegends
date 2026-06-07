using System;

namespace RealmOfLegends.Core.Entities
{
    public class FightLog
    {
        public int FightLogId { get; set; }
        public int PlayerId { get; set; }
        public int Level { get; set; }
        public string Outcome { get; set; } = string.Empty; // Victory, Defeat
        public int XPEarned { get; set; }
        public int GoldEarned { get; set; }
        public int? DroppedItemId { get; set; }
        public DateTime FoughtAt { get; set; } = DateTime.UtcNow;
        
        // Navigation Properties
        public virtual Player Player { get; set; } = null!;
        public virtual Item? DroppedItem { get; set; }
    }
}
