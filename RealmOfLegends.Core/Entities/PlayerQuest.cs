using System;

namespace RealmOfLegends.Core.Entities
{
    public class PlayerQuest
    {
        public int PlayerQuestId { get; set; }
        public int PlayerId { get; set; }
        public int QuestId { get; set; }
        public string Status { get; set; } = "Available"; // Available, InProgress, Completed, Failed
        public DateTime? CompletedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation Properties
        public virtual Player Player { get; set; } = null!;
        public virtual Quest Quest { get; set; } = null!;
    }
}
