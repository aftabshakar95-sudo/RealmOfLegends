using System;

namespace RealmOfLegends.Core.Entities
{
    public class RiddleAttempt
    {
        public int RiddleAttemptId { get; set; }
        public int PlayerId { get; set; }
        public int RiddleId { get; set; }
        public int Attempts { get; set; } = 0;
        public DateTime LastAttemptAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual Player Player { get; set; } = null!;
    }
}
