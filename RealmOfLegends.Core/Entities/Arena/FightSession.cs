using System;

namespace RealmOfLegends.Core.Entities.Arena
{
    public class FightSession
    {
        public string FightSessionId { get; set; } = Guid.NewGuid().ToString(); // GUID
        public int PlayerId { get; set; }
        public int EnemyId { get; set; }
        public int ArenaLevel { get; set; }
        
        // Authoritative Combat State
        public int PlayerCurrentHP { get; set; }
        public int PlayerMaxHP { get; set; }
        public int EnemyCurrentHP { get; set; }
        public int EnemyMaxHP { get; set; }
        
        public int TurnNumber { get; set; } = 0;
        public DateTime SessionStartedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        
        // Temporary Buff Tracking
        public int EnemyATKModifier { get; set; } = 0; // For Warrior taunt
        public int BleedTurnsRemaining { get; set; } = 0; // For Rogue bleed
        public int BleedDamagePerTurn { get; set; } = 0;
        
        // Navigation
        public virtual Player Player { get; set; } = null!;
        public virtual Enemy Enemy { get; set; } = null!;
    }
}
