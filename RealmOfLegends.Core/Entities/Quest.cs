namespace RealmOfLegends.Core.Entities
{
    public class Quest
    {
        public int QuestId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // ClickChallenge, MemoryMatch, Riddle, SliderPuzzle, DailyHunt, ArenaChallenge, Collection
        public string Description { get; set; } = string.Empty;
        public int RewardGold { get; set; }
        public int RewardXP { get; set; }
        public int? RewardItemId { get; set; }
        public bool DailyReset { get; set; } = false;
        public bool WeeklyReset { get; set; } = false;
        public string? QuestData { get; set; } // JSON for quest-specific data (riddle answer, target count, etc.)
        
        // Navigation Properties
        public virtual Item? RewardItem { get; set; }
        public virtual ICollection<PlayerQuest> PlayerQuests { get; set; } = new List<PlayerQuest>();
    }
}
