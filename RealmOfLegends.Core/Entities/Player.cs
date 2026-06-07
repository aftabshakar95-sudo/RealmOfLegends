using System;
using System.Collections.Generic;

namespace RealmOfLegends.Core.Entities
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string CharacterName { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty; // Warrior, Mage, Rogue, Paladin
        public int Level { get; set; } = 1;
        public int XP { get; set; } = 0;
        public int Gold { get; set; } = 100;
        
        // Base Stats
        public int HP { get; set; } = 100;
        public int MP { get; set; } = 50;
        public int Strength { get; set; } = 10;
        public int Agility { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public int Endurance { get; set; } = 10;
        public int Luck { get; set; } = 10;
        
        // Derived Stats (calculated from base stats + equipment)
        public int MaxHP { get; set; } = 100;
        public int MaxMP { get; set; } = 50;
        public int AttackPower { get; set; } = 10;
        public int DefenseRating { get; set; } = 5;
        public double CriticalHitChance { get; set; } = 0.05;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;
        
        // Navigation Properties
        public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
        public virtual ICollection<PlayerQuest> PlayerQuests { get; set; } = new List<PlayerQuest>();
        public virtual ICollection<PurchaseHistory> PurchaseHistories { get; set; } = new List<PurchaseHistory>();
        public virtual ArenaProgress? ArenaProgress { get; set; }
        public virtual ICollection<FightLog> FightLogs { get; set; } = new List<FightLog>();
        public virtual ICollection<PlayerAchievement> PlayerAchievements { get; set; } = new List<PlayerAchievement>();
    }
}
