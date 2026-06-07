namespace RealmOfLegends.Core.Entities
{
    public class Achievement
    {
        public int AchievementId { get; set; }
        public string Code { get; set; } = string.Empty; // FirstBlood, Level5, BossSlayer, etc.
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
        
        // Navigation Properties
        public virtual ICollection<PlayerAchievement> PlayerAchievements { get; set; } = new List<PlayerAchievement>();
    }
}
