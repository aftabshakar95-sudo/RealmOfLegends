using System;

namespace RealmOfLegends.Core.Entities
{
    public class ArenaProgress
    {
        public int ArenaProgressId { get; set; }
        public int PlayerId { get; set; }
        public int CurrentLevel { get; set; } = 1;
        public int HighestLevel { get; set; } = 1;
        public int TotalWins { get; set; } = 0;
        public int TotalLosses { get; set; } = 0;
        public DateTime? LastFightAt { get; set; }
        
        // Navigation Properties
        public virtual Player Player { get; set; } = null!;
    }
}
