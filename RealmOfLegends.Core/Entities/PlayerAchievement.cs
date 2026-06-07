using System;

namespace RealmOfLegends.Core.Entities
{
    public class PlayerAchievement
    {
        public int PlayerAchievementId { get; set; }
        public int PlayerId { get; set; }
        public int AchievementId { get; set; }
        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation Properties
        public virtual Player Player { get; set; } = null!;
        public virtual Achievement Achievement { get; set; } = null!;
    }
}
