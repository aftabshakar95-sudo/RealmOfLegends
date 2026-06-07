using System;

namespace RealmOfLegends.Core.Entities.Arena
{
    public class ArenaProgressNew
    {
        public int ArenaProgressNewId { get; set; }
        public int PlayerId { get; set; }
        public int HighestLevelCleared { get; set; } = 0;
        public int TotalWins { get; set; } = 0;
        public int TotalLosses { get; set; } = 0;
        public DateTime? LastFightAt { get; set; }
        
        // Navigation
        public virtual Player Player { get; set; } = null!;
    }
}
