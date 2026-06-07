using System;

namespace RealmOfLegends.Core.Entities.Arena
{
    public class FightLogNew
    {
        public int FightLogNewId { get; set; }
        public int PlayerId { get; set; }
        public int EnemyId { get; set; }
        public int ArenaLevel { get; set; }
        public string Outcome { get; set; } = string.Empty; // Victory/Defeat
        public int TurnsPlayed { get; set; }
        public int XPEarned { get; set; }
        public int GoldEarned { get; set; }
        public int? DroppedItemId { get; set; }
        public DateTime FoughtAt { get; set; } = DateTime.UtcNow;
        
        // Navigation
        public virtual Player Player { get; set; } = null!;
        public virtual Enemy Enemy { get; set; } = null!;
        public virtual Item? DroppedItem { get; set; }
    }
}
